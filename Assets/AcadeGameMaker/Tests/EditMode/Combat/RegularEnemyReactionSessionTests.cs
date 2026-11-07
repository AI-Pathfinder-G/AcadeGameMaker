using System;
using System.Linq;
using AcadeGameMaker.Core;
using NUnit.Framework;

namespace AcadeGameMaker.Combat.Tests
{
    public sealed class RegularEnemyReactionSessionTests
    {
        // REQ-COM-004; partial AC-COM-003: M3A adds only internal engine-free types and publishes no synthetic constructor snapshot.
        [Test]
        public void SurfaceIsInternalAndLatestSnapshotStartsAbsent()
        {
            var names = new[]
            {
                nameof(RegularEnemyReactionSession), nameof(RegularEnemyReactionSnapshot), nameof(RegularEnemyReactionInput),
                nameof(RegularEnemyTransferTransition), nameof(WalkerReactionEvent), nameof(RegularEnemyTransferState),
                nameof(WalkerReactionEventKind), nameof(RegularEnemyReactionInputKind)
            };
            var types = typeof(RegularEnemyReactionSession).Assembly.GetTypes().Where(type => names.Contains(type.Name)).ToArray();
            Assert.That(types.Length, Is.EqualTo(names.Length));
            Assert.That(types.All(type => !type.IsPublic && !type.IsNestedPublic), Is.True);

            var session = Walker();
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(0)));
        }

        // REQ-COM-004; partial AC-COM-003: target and exact-tick failures preserve the complete previously published state.
        [Test]
        public void ExactTickAndTargetMismatchAreAtomic()
        {
            Assert.Throws<ArgumentException>(() => new RegularEnemyReactionSession("enemy", Tick(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RegularEnemyReactionSession("walker", Tick(-1)));
            var session = Walker();
            Assert.Throws<InvalidOperationException>(() => session.Process(Gameplay("walker", 1, true)));
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            Assert.Throws<ArgumentException>(() => session.Process(Gameplay("surveyor", 0, true)));
            Assert.That(session.LatestSnapshot.HasValue, Is.False);

            var baseline = session.Process(Gameplay("walker", 0, true));
            Assert.Throws<InvalidOperationException>(() => session.Process(Gameplay("walker", 0, true)));
            AssertSnapshotUnchanged(session, baseline);
        }

        // REQ-WT-005, REQ-COM-004; partial AC-WT-005, AC-COM-003: revisions are positive, strictly increasing, gapped, edge-only and may reach int.MaxValue.
        [Test]
        public void TransferRevisionBoundariesGapsAndNoOpEdgesAreExactAndAtomic()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Transition("walker", RegularEnemyTransferState.Heavy, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Transition("walker", RegularEnemyTransferState.Heavy, -1, 0));
            var session = Walker();
            var heavy = session.Process(Gameplay("walker", 0, true, Transition("walker", RegularEnemyTransferState.Heavy, 3, 0)));
            Assert.That(heavy.IsHeavy, Is.True);
            Assert.That(heavy.LastTransferRevision, Is.EqualTo(3));
            Assert.That(heavy.RecoveryMultiplierQ1000, Is.EqualTo(1500));
            Assert.That(heavy.ReactionAllowsBasicAttack, Is.False);

            Assert.Throws<InvalidOperationException>(() => session.Process(Gameplay("walker", 1, true, Transition("walker", RegularEnemyTransferState.Baseline, 2, 1))));
            AssertSnapshotUnchanged(session, heavy);
            Assert.Throws<InvalidOperationException>(() => session.Process(Gameplay("walker", 1, true, Transition("walker", RegularEnemyTransferState.Heavy, 4, 1))));
            AssertSnapshotUnchanged(session, heavy);
            var cleared = session.Process(Gameplay("walker", 1, true, Transition("walker", RegularEnemyTransferState.Baseline, int.MaxValue, 1)));
            Assert.That(cleared.IsHeavy, Is.False);
            Assert.That(cleared.LastTransferRevision, Is.EqualTo(int.MaxValue));
            Assert.That(cleared.RecoveryMultiplierQ1000, Is.EqualTo(1000));
            Assert.Throws<InvalidOperationException>(() => session.Process(Gameplay("walker", 2, true, Transition("walker", RegularEnemyTransferState.Heavy, int.MaxValue, 2))));
            AssertSnapshotUnchanged(session, cleared);
        }

        // REQ-COM-001, REQ-COM-005; partial AC-COM-001: natural and heavy-impact openings cover exact exclusive 30/90-tick windows and never shorten overlap.
        [Test]
        public void WalkerWindowsHaveExactExclusiveBoundariesAndOverlapNeverShortens()
        {
            var natural = Walker();
            Assert.That(natural.Process(Gameplay("walker", 0, true, walkerEvent: Recovery(10))).ReactionAllowsBasicAttack, Is.True);
            Advance(natural, "walker", 1, 29);
            Assert.That(natural.LatestSnapshot.Value.ReactionAllowsBasicAttack, Is.True);
            Assert.That(natural.Process(Gameplay("walker", 30, true)).ReactionAllowsBasicAttack, Is.False);
            Assert.That(natural.LatestSnapshot.Value.ShieldWindowEndTick, Is.EqualTo(30));

            var impact = Walker();
            Assert.That(impact.Process(Gameplay("walker", 0, true, walkerEvent: Impact("impact/0"))).ShieldWindowEndTick, Is.EqualTo(90));
            Advance(impact, "walker", 1, 89);
            Assert.That(impact.LatestSnapshot.Value.ReactionAllowsBasicAttack, Is.True);
            Assert.That(impact.Process(Gameplay("walker", 90, true)).ReactionAllowsBasicAttack, Is.False);

            var overlap = Walker();
            overlap.Process(Gameplay("walker", 0, true, walkerEvent: Impact("impact/long")));
            var shorter = overlap.Process(Gameplay("walker", 1, true, walkerEvent: Recovery(11)));
            Assert.That(shorter.ShieldWindowEndTick, Is.EqualTo(90));
            var extended = overlap.Process(Gameplay("walker", 2, true, walkerEvent: Impact("impact/later")));
            Assert.That(extended.ShieldWindowEndTick, Is.EqualTo(92));
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003: malformed and duplicate singular events cannot consume IDs or alter windows.
        [Test]
        public void WalkerEventDiscriminantsAndTombstonesAreMutationSafe()
        {
            Assert.Throws<ArgumentException>(() => new WalkerReactionEvent(WalkerReactionEventKind.None, 0, null));
            Assert.Throws<ArgumentException>(() => new WalkerReactionEvent(WalkerReactionEventKind.AttackRecoveryStarted, -1, null));
            Assert.Throws<ArgumentException>(() => new WalkerReactionEvent(WalkerReactionEventKind.AttackRecoveryStarted, 0, "request"));
            Assert.Throws<ArgumentException>(() => new WalkerReactionEvent(WalkerReactionEventKind.HeavyImpactApplied, null, "bad id!"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new WalkerReactionEvent((WalkerReactionEventKind)99, null, null));
            var surveyor = Surveyor();
            Assert.Throws<ArgumentException>(() => surveyor.Process(Gameplay("surveyor", 0, true, walkerEvent: Recovery(0))));
            Assert.That(surveyor.LatestSnapshot.HasValue, Is.False);

            var session = Walker();
            var impact = session.Process(Gameplay("walker", 0, true, walkerEvent: Impact("impact/once")));
            Assert.Throws<InvalidOperationException>(() => session.Process(Gameplay("walker", 1, true, walkerEvent: Impact("impact/once"))));
            AssertSnapshotUnchanged(session, impact);
            session.Process(Gameplay("walker", 1, true, walkerEvent: Recovery(7)));
            var beforeCycleDuplicate = session.LatestSnapshot.Value;
            Assert.Throws<InvalidOperationException>(() => session.Process(Gameplay("walker", 2, true, walkerEvent: Recovery(7))));
            AssertSnapshotUnchanged(session, beforeCycleDuplicate);
        }

        // REQ-COM-002, REQ-COM-005, REQ-WT-003; partial AC-COM-001, AC-WT-002: surveyor Heavy descent and fire lock use one edge and preserve residual overheat after recall.
        [Test]
        public void SurveyorHeavyAndRecallHaveExactSeventyFiveTickLock()
        {
            var session = Surveyor();
            var heavy = session.Process(Gameplay("surveyor", 0, true, Transition("surveyor", RegularEnemyTransferState.Heavy, 1, 0)));
            Assert.That(heavy.IsHeavy, Is.True);
            Assert.That(heavy.ForcedDescent, Is.True);
            Assert.That(heavy.CanFire, Is.False);
            Assert.That(heavy.FireLockEndTick, Is.EqualTo(75));

            Advance(session, "surveyor", 1, 10);
            Assert.That(session.LatestSnapshot.Value.FireLockEndTick, Is.EqualTo(75));
            var recalled = session.Process(Gameplay("surveyor", 11, true, Transition("surveyor", RegularEnemyTransferState.Baseline, 2, 11)));
            Assert.That(recalled.ForcedDescent, Is.False);
            Assert.That(recalled.CanFire, Is.False);
            Assert.That(recalled.FireLockEndTick, Is.EqualTo(75));
            Advance(session, "surveyor", 12, 74);
            Assert.That(session.LatestSnapshot.Value.CanFire, Is.False);
            Assert.That(session.Process(Gameplay("surveyor", 75, true)).CanFire, Is.True);
            Assert.That(session.LatestSnapshot.Value.FireLockEndTick, Is.EqualTo(75));
        }

        // REQ-COM-004, REQ-WT-005; partial AC-COM-003, AC-WT-005: death suppresses directives, mirrors the same-tick edge, and accepts only the expected next-tick cleanup.
        [Test]
        public void DeathAndTransferCleanupPreserveRawAndEffectiveAuthorityBoundaries()
        {
            var surveyor = Surveyor();
            var deadHeavy = surveyor.Process(Gameplay("surveyor", 0, false, Transition("surveyor", RegularEnemyTransferState.Heavy, 4, 0)));
            Assert.That(deadHeavy.IsAlive, Is.False);
            Assert.That(deadHeavy.IsHeavy, Is.True);
            Assert.That(deadHeavy.FireLockEndTick, Is.EqualTo(-1));
            Assert.That(deadHeavy.ForcedDescent, Is.False);
            Assert.That(deadHeavy.CanFire, Is.False);
            Assert.Throws<InvalidOperationException>(() => surveyor.Process(Gameplay("surveyor", 1, false, Transition("surveyor", RegularEnemyTransferState.Heavy, 5, 1))));
            AssertSnapshotUnchanged(surveyor, deadHeavy);
            var cleaned = surveyor.Process(Gameplay("surveyor", 1, false, Transition("surveyor", RegularEnemyTransferState.Baseline, 5, 1)));
            Assert.That(cleaned.IsHeavy, Is.False);
            Assert.That(cleaned.LastTransferRevision, Is.EqualTo(5));
            Assert.Throws<InvalidOperationException>(() => surveyor.Process(Gameplay("surveyor", 2, true)));
            AssertSnapshotUnchanged(surveyor, cleaned);
            Assert.Throws<InvalidOperationException>(() => surveyor.Process(Gameplay("surveyor", 2, false, Transition("surveyor", RegularEnemyTransferState.Baseline, 6, 2))));
            AssertSnapshotUnchanged(surveyor, cleaned);

            var walker = Walker();
            Assert.Throws<ArgumentException>(() => walker.Process(Gameplay("walker", 0, false, walkerEvent: Impact("death/impact"))));
            Assert.That(walker.LatestSnapshot.HasValue, Is.False);
        }

        // REQ-COM-001, REQ-COM-004, REQ-WT-005; partial AC-COM-001, AC-COM-003, AC-WT-005: Walker death preserves raw history but neutralizes both effective directives.
        [Test]
        public void WalkerDeathPreservesRawWindowAndHeavyUntilCleanupButSuppressesDirectives()
        {
            var session = Walker();
            session.Process(Gameplay("walker", 0, true, Transition("walker", RegularEnemyTransferState.Heavy, 1, 0), Impact("walker/death")));
            var dead = session.Process(Gameplay("walker", 1, false));
            Assert.That(dead.IsAlive, Is.False);
            Assert.That(dead.IsHeavy, Is.True);
            Assert.That(dead.ShieldWindowEndTick, Is.EqualTo(90));
            Assert.That(dead.ReactionAllowsBasicAttack, Is.False);
            Assert.That(dead.RecoveryMultiplierQ1000, Is.EqualTo(1000));

            var cleaned = session.Process(Gameplay("walker", 2, false, Transition("walker", RegularEnemyTransferState.Baseline, 2, 2)));
            Assert.That(cleaned.IsHeavy, Is.False);
            Assert.That(cleaned.ShieldWindowEndTick, Is.EqualTo(90));
            Assert.That(cleaned.ReactionAllowsBasicAttack, Is.False);
            Assert.That(cleaned.RecoveryMultiplierQ1000, Is.EqualTo(1000));
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003: dead processing performs neither reaction horizon arithmetic nor tombstone consumption.
        [Test]
        public void DeadTickSkipsReactionHorizonAndRejectedEventDoesNotConsumeItsId()
        {
            var surveyor = new RegularEnemyReactionSession("surveyor", Tick(int.MaxValue - 74));
            var dead = surveyor.Process(Gameplay("surveyor", int.MaxValue - 74, false,
                Transition("surveyor", RegularEnemyTransferState.Heavy, 1, int.MaxValue - 74)));
            Assert.That(dead.IsAlive, Is.False);
            Assert.That(dead.IsHeavy, Is.True);
            Assert.That(dead.FireLockEndTick, Is.EqualTo(-1));

            var walker = Walker();
            Assert.Throws<ArgumentException>(() => walker.Process(Gameplay("walker", 0, false, walkerEvent: Impact("reusable/dead"))));
            Assert.That(walker.LatestSnapshot.HasValue, Is.False);
            Assert.DoesNotThrow(() => walker.Process(Gameplay("walker", 0, true, walkerEvent: Impact("reusable/dead"))));
            Assert.That(walker.LatestSnapshot.Value.ShieldWindowEndTick, Is.EqualTo(90));
        }

        // REQ-COM-004, REQ-WT-005; partial AC-COM-003, AC-WT-005: reset consumes one global tick, requires baseline, and clears every encounter-local latch and tombstone.
        [Test]
        public void EncounterResetIsExclusiveBaselineOnlyAndPublishesSpecifiedLiveEmptySnapshot()
        {
            var session = Walker();
            Assert.Throws<InvalidOperationException>(() => session.Process(RegularEnemyReactionInput.EncounterReset(Tick(0))));
            session.Process(Gameplay("walker", 0, true, Transition("walker", RegularEnemyTransferState.Heavy, 1, 0), Impact("before/reset")));
            Assert.Throws<InvalidOperationException>(() => session.Process(RegularEnemyReactionInput.EncounterReset(Tick(1))));
            session.Process(Gameplay("walker", 1, true, Transition("walker", RegularEnemyTransferState.Baseline, 2, 1)));
            var reset = session.Process(RegularEnemyReactionInput.EncounterReset(Tick(2)));
            Assert.That(Trace(reset), Is.EqualTo("2:walker:True:False:-1:-1:-1:False:1000:False:False:3"));
            Assert.Throws<InvalidOperationException>(() => session.Process(RegularEnemyReactionInput.EncounterReset(Tick(3))));
            var next = session.Process(Gameplay("walker", 3, true,
                Transition("walker", RegularEnemyTransferState.Heavy, 1, 3), Impact("before/reset")));
            Assert.That(next.ReactionAllowsBasicAttack, Is.True);
            Assert.That(next.ShieldWindowEndTick, Is.EqualTo(93));
            Assert.That(next.LastTransferRevision, Is.EqualTo(1));

            var cycle = Walker();
            cycle.Process(Gameplay("walker", 0, true, walkerEvent: Recovery(99)));
            cycle.Process(RegularEnemyReactionInput.EncounterReset(Tick(1)));
            Assert.DoesNotThrow(() => cycle.Process(Gameplay("walker", 2, true, walkerEvent: Recovery(99))));
        }

        // REQ-COM-002, REQ-COM-004; partial AC-COM-003: Surveyor reset is an empty live publication and ordinary baseline fire resumes next tick.
        [Test]
        public void SurveyorResetPublishesEmptyExceptionThenBaselineCanFire()
        {
            var session = Surveyor();
            session.Process(Gameplay("surveyor", 0, true));
            var reset = session.Process(RegularEnemyReactionInput.EncounterReset(Tick(1)));
            Assert.That(Trace(reset), Is.EqualTo("1:surveyor:True:False:-1:-1:-1:False:1000:False:False:2"));

            var baseline = session.Process(Gameplay("surveyor", 2, true));
            Assert.That(baseline.CanFire, Is.True);
            Assert.That(baseline.ForcedDescent, Is.False);
        }

        // REQ-COM-002, REQ-COM-005, REQ-WT-003; partial AC-COM-001, AC-WT-002: only a later Heavy edge extends the Surveyor lock.
        [Test]
        public void SurveyorLaterHeavyEdgeExtendsButNeverShortensFireLock()
        {
            var session = Surveyor();
            Assert.That(session.Process(Gameplay("surveyor", 0, true,
                Transition("surveyor", RegularEnemyTransferState.Heavy, 1, 0))).FireLockEndTick, Is.EqualTo(75));
            session.Process(Gameplay("surveyor", 1, true,
                Transition("surveyor", RegularEnemyTransferState.Baseline, 2, 1)));
            Advance(session, "surveyor", 2, 9);
            var secondHeavy = session.Process(Gameplay("surveyor", 10, true,
                Transition("surveyor", RegularEnemyTransferState.Heavy, 3, 10)));
            Assert.That(secondHeavy.FireLockEndTick, Is.EqualTo(85));
            Assert.That(secondHeavy.ForcedDescent, Is.True);
        }

        // REQ-WT-005, REQ-COM-004; partial AC-WT-005, AC-COM-003: an initial Baseline-to-Baseline pseudo-edge is rejected without publication.
        [Test]
        public void InitialBaselineNoOpTransitionIsAtomic()
        {
            var session = Walker();
            Assert.Throws<InvalidOperationException>(() => session.Process(Gameplay("walker", 0, true,
                Transition("walker", RegularEnemyTransferState.Baseline, 1, 0))));
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(0)));
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003: checked next/window arithmetic fails before snapshot, revision, or tombstone publication.
        [Test]
        public void CheckedWindowAndNextTickOverflowAreAtomic()
        {
            AssertFirstFailureAtomic(new RegularEnemyReactionSession("walker", Tick(int.MaxValue - 29)),
                Gameplay("walker", int.MaxValue - 29, true, walkerEvent: Recovery(1)));
            AssertFirstFailureAtomic(new RegularEnemyReactionSession("walker", Tick(int.MaxValue - 89)),
                Gameplay("walker", int.MaxValue - 89, true, walkerEvent: Impact("overflow/impact")));
            AssertFirstFailureAtomic(new RegularEnemyReactionSession("surveyor", Tick(int.MaxValue - 74)),
                Gameplay("surveyor", int.MaxValue - 74, true, Transition("surveyor", RegularEnemyTransferState.Heavy, 1, int.MaxValue - 74)));
            AssertFirstFailureAtomic(new RegularEnemyReactionSession("walker", Tick(int.MaxValue)), Gameplay("walker", int.MaxValue, true));

            var populated = new RegularEnemyReactionSession("walker", Tick(int.MaxValue - 90));
            var beforeOverflow = populated.Process(Gameplay("walker", int.MaxValue - 90, true,
                Transition("walker", RegularEnemyTransferState.Heavy, 1, int.MaxValue - 90), Impact("overflow/existing")));
            Assert.Throws<OverflowException>(() => populated.Process(Gameplay("walker", int.MaxValue - 89, true,
                walkerEvent: Impact("overflow/new"))));
            AssertSnapshotUnchanged(populated, beforeOverflow);

            var resetOverflow = new RegularEnemyReactionSession("walker", Tick(int.MaxValue - 1));
            var beforeResetOverflow = resetOverflow.Process(Gameplay("walker", int.MaxValue - 1, true));
            Assert.Throws<OverflowException>(() => resetOverflow.Process(RegularEnemyReactionInput.EncounterReset(Tick(int.MaxValue))));
            AssertSnapshotUnchanged(resetOverflow, beforeResetOverflow);
        }

        // REQ-COM-004; AC-COM-003: M3B's validate-both-before-process seam must
        // accept/reject exactly as Process does and cannot publish/tombstone on its own.
        [Test]
        public void ValidateNextMatchesProcessWithoutChangingObservableState()
        {
            var session = Walker();
            var input = Gameplay("walker", 0, true, walkerEvent: Recovery(42));
            Assert.DoesNotThrow(() => session.ValidateNext(input));
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(0)));
            var committed = session.Process(input);
            Assert.That(committed.ShieldWindowEndTick, Is.EqualTo(30));

            var rejected = Gameplay("walker", 1, false, walkerEvent: Impact("dead/impact"));
            Assert.Throws<ArgumentException>(() => session.ValidateNext(rejected));
            AssertSnapshotUnchanged(session, committed);
            Assert.Throws<ArgumentException>(() => session.Process(rejected));
            AssertSnapshotUnchanged(session, committed);
        }

        // REQ-COM-002, REQ-COM-004; partial AC-COM-001, AC-COM-003: render grouping cannot alter any 60Hz reaction scalar or clock.
        [Test]
        public void SameSixtyHertzReactionScriptMatchesThirtySixtyAndOneFortyFourRenderGrouping()
        {
            Assert.That(Replay(30), Is.EqualTo(Replay(60)));
            Assert.That(Replay(60), Is.EqualTo(Replay(144)));
        }

        private static string Replay(int renderFramesPerSecond)
        {
            var session = Surveyor();
            var trace = string.Empty;
            var accumulator = 0;
            var tick = 0;
            while (tick < 120)
            {
                accumulator += 60;
                while (accumulator >= renderFramesPerSecond && tick < 120)
                {
                    accumulator -= renderFramesPerSecond;
                    RegularEnemyTransferTransition? transition = null;
                    if (tick == 0) transition = Transition("surveyor", RegularEnemyTransferState.Heavy, 1, tick);
                    if (tick == 20) transition = Transition("surveyor", RegularEnemyTransferState.Baseline, 2, tick);
                    if (tick == 40) transition = Transition("surveyor", RegularEnemyTransferState.Heavy, 9, tick);
                    if (tick == 50) transition = Transition("surveyor", RegularEnemyTransferState.Baseline, 10, tick);
                    var snapshot = session.Process(Gameplay("surveyor", tick, true, transition));
                    trace += Trace(snapshot) + "|";
                    tick++;
                }
            }

            return trace;
        }

        private static void AssertFirstFailureAtomic(RegularEnemyReactionSession session, RegularEnemyReactionInput input)
        {
            var expectedTick = session.NextExpectedTick;
            Assert.Throws<OverflowException>(() => session.Process(input));
            Assert.That(session.NextExpectedTick, Is.EqualTo(expectedTick));
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
        }

        private static void AssertSnapshotUnchanged(RegularEnemyReactionSession session, RegularEnemyReactionSnapshot expected)
        {
            Assert.That(session.LatestSnapshot.HasValue, Is.True);
            Assert.That(Trace(session.LatestSnapshot.Value), Is.EqualTo(Trace(expected)));
            Assert.That(session.NextExpectedTick, Is.EqualTo(expected.NextExpectedTick));
        }

        private static void Advance(RegularEnemyReactionSession session, string targetId, int firstTick, int lastTick)
        {
            for (var tick = firstTick; tick <= lastTick; tick++) session.Process(Gameplay(targetId, tick, true));
        }

        private static string Trace(RegularEnemyReactionSnapshot value) =>
            value.Tick.Value + ":" + value.TargetId + ":" + value.IsAlive + ":" + value.IsHeavy + ":" + value.LastTransferRevision + ":" +
            value.ShieldWindowEndTick + ":" + value.FireLockEndTick + ":" + value.ReactionAllowsBasicAttack + ":" + value.RecoveryMultiplierQ1000 + ":" +
            value.ForcedDescent + ":" + value.CanFire + ":" + value.NextExpectedTick.Value;

        private static RegularEnemyReactionSession Walker() => new RegularEnemyReactionSession("walker", Tick(0));
        private static RegularEnemyReactionSession Surveyor() => new RegularEnemyReactionSession("surveyor", Tick(0));
        private static SimulationTick Tick(int value) => new SimulationTick(value);
        private static RegularEnemyTransferTransition Transition(string targetId, RegularEnemyTransferState state, int revision, int tick) =>
            new RegularEnemyTransferTransition(targetId, state, revision, Tick(tick));
        private static WalkerReactionEvent Recovery(long attackCycleId) => WalkerReactionEvent.AttackRecoveryStarted(attackCycleId);
        private static WalkerReactionEvent Impact(string requestId) => WalkerReactionEvent.HeavyImpactApplied(requestId);
        private static RegularEnemyReactionInput Gameplay(string targetId, int tick, bool alive, RegularEnemyTransferTransition? transition = null, WalkerReactionEvent? walkerEvent = null) =>
            RegularEnemyReactionInput.Gameplay(Tick(tick), targetId, alive, transition, walkerEvent ?? WalkerReactionEvent.None);
    }
}
