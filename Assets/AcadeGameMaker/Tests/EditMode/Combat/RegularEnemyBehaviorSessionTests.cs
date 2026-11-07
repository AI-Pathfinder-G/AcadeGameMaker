using System;
using System.Collections.Generic;
using System.Linq;
using AcadeGameMaker.Core;
using NUnit.Framework;

namespace AcadeGameMaker.Combat.Tests
{
    public sealed class RegularEnemyBehaviorSessionTests
    {
        // REQ-COM-004; partial AC-COM-003: the M3B2A substrate remains an internal, engine-free Combat type.
        [Test]
        public void SurfaceIsInternalAndStartsWithoutPublication()
        {
            var names = new[] { nameof(RegularEnemyBehaviorSession), nameof(RegularEnemyBehaviorPairInput), nameof(RegularEnemyBehaviorPairSnapshot), nameof(WalkerRecoveryIntent), nameof(SurveyorShotIntent) };
            var types = typeof(RegularEnemyBehaviorSession).Assembly.GetTypes().Where(type => names.Contains(type.Name)).ToArray();
            Assert.That(types.Length, Is.EqualTo(names.Length));
            Assert.That(types.All(type => !type.IsPublic && !type.IsNestedPublic), Is.True);
            var session = Session();
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(0)));
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-001: 18/12/60 post-transition boundaries and the t -> t+1 recovery intent are exact.
        [Test]
        public void WalkerPhasesCandidateAndEntrySampleHaveExactBoundaries()
        {
            var session = Session();
            var telegraph = session.Process(Game(0, playerX: 4000, walkerLos: true));
            AssertWalker(telegraph, WalkerBehaviorPhase.DashTelegraph, 18);
            Advance(session, 1, 17);
            AssertWalker(session.LatestSnapshot.Value, WalkerBehaviorPhase.DashTelegraph, 18);
            var dash = session.Process(Game(18, playerX: -9000));
            AssertWalker(dash, WalkerBehaviorPhase.DashActive, 30);
            Assert.That(dash.Walker.Direction, Is.EqualTo(1));
            Advance(session, 19, 29);
            var finalDash = session.LatestSnapshot.Value;
            Assert.That(finalDash.WalkerRecoveryIntent.HasValue, Is.True);
            Assert.That(finalDash.WalkerRecoveryIntent.Value.AttackCycleOrdinal, Is.EqualTo(0));
            Assert.That(finalDash.WalkerRecoveryIntent.Value.ScheduledTick, Is.EqualTo(Tick(30)));
            Assert.That(finalDash.Walker.AttackCycleOrdinal, Is.EqualTo(1));
            var recovery = session.Process(Game(30, walkerHeavy: true, walkerRevision: 1));
            AssertWalker(recovery, WalkerBehaviorPhase.Recovery, 90);
            Assert.That(recovery.WalkerRecoveryIntent.HasValue, Is.False);
            Advance(session, 31, 89, walkerHeavy: true, walkerRevision: 1);
            AssertWalker(session.LatestSnapshot.Value, WalkerBehaviorPhase.Recovery, 90);
            AssertWalker(session.Process(Game(90)), WalkerBehaviorPhase.Approach, -1);

            var baseline = Session();
            baseline.Process(Game(0, playerX: 1, walkerLos: true));
            Advance(baseline, 1, 29);
            AssertWalker(baseline.Process(Game(30)), WalkerBehaviorPhase.Recovery, 70);
            Advance(baseline, 31, 69);
            AssertWalker(baseline.Process(Game(70)), WalkerBehaviorPhase.Approach, -1);
        }

        // REQ-COM-001; partial AC-COM-001: guard follows M3A attackability only and never resizes a behavior deadline.
        [Test]
        public void WalkerGuardFollowsM3AAttackabilityWithoutChangingPhase()
        {
            var session = Session();
            var guarded = session.Process(Game(0));
            Assert.That(guarded.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.Approach));
            Assert.That(guarded.Walker.GuardRaised, Is.True);
            var open = session.Process(Game(1, walkerShieldEnd: 31));
            Assert.That(open.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.Approach));
            Assert.That(open.Walker.GuardRaised, Is.False);
        }

        // REQ-COM-002, REQ-COM-005; partial AC-COM-001: 48/30/1/60 surveyor boundaries latch the exact entry target and alternate ordinal direction.
        [Test]
        public void SurveyorPhasesLatchShotAndAlternateRelocationExactly()
        {
            var session = Session();
            var relocate = session.Process(Game(0, playerX: 111));
            AssertSurveyor(relocate, SurveyorBehaviorPhase.Relocate, 48);
            Assert.That(relocate.Surveyor.Direction, Is.EqualTo(-1));
            Advance(session, 1, 47);
            var telegraph = session.Process(Game(48, playerX: 777));
            AssertSurveyor(telegraph, SurveyorBehaviorPhase.FireTelegraph, 78);
            Assert.That(telegraph.Surveyor.LatchedTargetXQ1000, Is.EqualTo(777));
            Advance(session, 49, 77, playerX: -444);
            var fire = session.Process(Game(78, playerX: -999));
            AssertSurveyor(fire, SurveyorBehaviorPhase.Fire, 79);
            Assert.That(fire.SurveyorShotIntent.Value.ShotOrdinal, Is.EqualTo(0));
            Assert.That(fire.SurveyorShotIntent.Value.TargetXQ1000, Is.EqualTo(777));
            Assert.That(fire.Surveyor.ShotOrdinal, Is.EqualTo(1));
            AssertSurveyor(session.Process(Game(79)), SurveyorBehaviorPhase.Cooldown, 139);
            Advance(session, 80, 138);
            var nextRelocate = session.Process(Game(139));
            AssertSurveyor(nextRelocate, SurveyorBehaviorPhase.Relocate, 187);
            Assert.That(nextRelocate.Surveyor.Direction, Is.EqualTo(1));
        }

        // REQ-COM-002, REQ-COM-005; affected AC-WT-005: Heavy/descent and residual fire-lock cancel telegraph, then release into a complete new Relocate.
        [Test]
        public void SurveyorSuppressionHasPriorityAndResidualLockRestartsFullRelocate()
        {
            var session = Session();
            session.Process(Game(0));
            var descent = session.Process(Game(1, surveyorHeavy: true, surveyorRevision: 1));
            AssertSurveyor(descent, SurveyorBehaviorPhase.Suppressed, -1);
            Assert.That(descent.Surveyor.Movement, Is.EqualTo(RegularEnemyMovementIntent.Downward));
            var residual = session.Process(Game(2, surveyorFireLockEnd: 76));
            AssertSurveyor(residual, SurveyorBehaviorPhase.Suppressed, -1);
            Assert.That(residual.Surveyor.Movement, Is.EqualTo(RegularEnemyMovementIntent.None));
            for (var tick = 3; tick < 76; tick++)
            {
                var held = session.Process(Game(tick, surveyorFireLockEnd: 76));
                AssertSurveyor(held, SurveyorBehaviorPhase.Suppressed, -1);
            }
            var released = session.Process(Game(76));
            AssertSurveyor(released, SurveyorBehaviorPhase.Relocate, 124);
            Assert.That(released.SurveyorShotIntent.HasValue, Is.False);
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003: death is role-local/inert, resurrection is rejected, and reset publishes no gameplay action.
        [Test]
        public void DeathLatchAndExactResetAreRoleLocalAndAtomic()
        {
            var session = Session();
            session.Process(Game(0));
            var deadWalker = session.Process(Game(1, walkerAlive: false));
            Assert.That(deadWalker.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.Dead));
            Assert.That(deadWalker.Surveyor.IsAlive, Is.True);
            Assert.That(deadWalker.Surveyor.Phase, Is.EqualTo(SurveyorBehaviorPhase.Relocate));
            var repeatedDead = session.Process(Game(2, walkerAlive: false));
            Assert.That(repeatedDead.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.Dead));
            Assert.That(repeatedDead.WalkerRecoveryIntent.HasValue, Is.False);
            Assert.Throws<InvalidOperationException>(() => session.Process(Game(3)));
            Assert.That(session.LatestSnapshot.Value.Tick, Is.EqualTo(Tick(2)));
            var reset = session.Process(Reset(3));
            Assert.That(reset.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.Reset));
            Assert.That(reset.Surveyor.Phase, Is.EqualTo(SurveyorBehaviorPhase.Reset));
            Assert.That(reset.WalkerRecoveryIntent.HasValue || reset.SurveyorShotIntent.HasValue, Is.False);
            var restarted = session.Process(Game(4));
            AssertWalker(restarted, WalkerBehaviorPhase.Approach, -1);
            AssertSurveyor(restarted, SurveyorBehaviorPhase.Relocate, 52);
        }

        // REQ-COM-004; partial AC-COM-003: malformed/default views, revision zero, Heavy without a positive revision and first-tick reset do not commit either role.
        [Test]
        public void InvalidViewsAndFirstTickResetFailBeforeEitherRoleCommits()
        {
            var session = Session();
            Assert.Throws<InvalidOperationException>(() => session.Process(Reset(0)));
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            Assert.Throws<ArgumentException>(() => session.Process(RegularEnemyBehaviorPairInput.Gameplay(Tick(0), true, true, default, SurveyorReaction(0), 0, 0, 0, false, false)));
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            Assert.Throws<ArgumentOutOfRangeException>(() => session.Process(Game(0, walkerRevision: 0)));
            Assert.Throws<InvalidOperationException>(() => session.Process(Game(0, walkerHeavy: true, walkerRevision: -1)));
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
        }

        // REQ-COM-004; partial AC-COM-003: live walker M3A multiplier is a bidirectional Heavy derivation and either malformed direction preserves both role states.
        [Test]
        public void LiveWalkerHeavyMultiplierMismatchIsBidirectionalAndAtomic()
        {
            var session = Session();
            var baseline = session.Process(Game(0));
            var heavyWithBaselineMultiplier = RawWalkerReaction(1, true, true, 1, 1000);
            Assert.Throws<InvalidOperationException>(() => session.Process(GameWithWalkerReaction(1, heavyWithBaselineMultiplier)));
            Assert.That(session.LatestSnapshot.Value.Tick, Is.EqualTo(baseline.Tick));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(1)));

            var baselineWithHeavyMultiplier = RawWalkerReaction(1, true, false, -1, 1500);
            Assert.Throws<InvalidOperationException>(() => session.Process(GameWithWalkerReaction(1, baselineWithHeavyMultiplier)));
            Assert.That(session.LatestSnapshot.Value.Tick, Is.EqualTo(baseline.Tick));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(1)));
        }

        // REQ-COM-004; partial AC-COM-003: stale/wrong/default/invalid inputs and malformed reset publications reject before a pair mutation.
        [Test]
        public void StaleRoleDefaultDiscriminantAndMalformedResetFailuresAreAtomic()
        {
            var session = Session();
            var baseline = session.Process(Game(0));
            Assert.Throws<ArgumentException>(() => session.Process(RegularEnemyBehaviorPairInput.Gameplay(Tick(1), true, true, WalkerReaction(0), SurveyorReaction(1), 0, 0, 0, false, false)));
            AssertUnchanged(session, baseline, 1);

            Assert.Throws<ArgumentException>(() => session.Process(RegularEnemyBehaviorPairInput.Gameplay(Tick(1), true, true, WalkerReaction(1), WalkerReaction(1), 0, 0, 0, false, false)));
            AssertUnchanged(session, baseline, 1);
            var defaultSession = Session();
            Assert.Throws<ArgumentException>(() => defaultSession.Process(default));
            Assert.That(defaultSession.LatestSnapshot.HasValue, Is.False);
            Assert.That(defaultSession.NextExpectedTick, Is.EqualTo(Tick(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => RegularEnemyBehaviorPairInput.AssertInvalidDiscriminantRejectedForTests());
            AssertUnchanged(session, baseline, 1);

            var malformedReset = RegularEnemyBehaviorPairInput.EncounterReset(Tick(1), ResetReaction("walker", 1), SurveyorReaction(1));
            Assert.Throws<InvalidOperationException>(() => session.Process(malformedReset));
            AssertUnchanged(session, baseline, 1);
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003: clone validation and a failed second-role structural derivation preserve the complete pair publication/ordinals.
        [Test]
        public void PairValidateAndSecondRoleFailureAreMutationSafe()
        {
            var session = Session();
            var baseline = session.Process(Game(0));
            session.ValidateNext(Game(1));
            Assert.That(session.LatestSnapshot.Value.Tick, Is.EqualTo(baseline.Tick));
            var malformedSurveyor = new RegularEnemyReactionSnapshot(Tick(1), "surveyor", true, false, -1, -1, -1, false, 1000, false, false, Tick(2));
            Assert.Throws<InvalidOperationException>(() => session.Process(RegularEnemyBehaviorPairInput.Gameplay(Tick(1), true, true, WalkerReaction(1), malformedSurveyor, 0, 0, 0, false, false)));
            Assert.That(session.LatestSnapshot.Value.Tick, Is.EqualTo(baseline.Tick));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(1)));
        }

        // REQ-COM-004: both action ordinal increments are checked before commit; overflow produces neither intent nor partial pair mutation.
        [Test]
        public void WalkerAndSurveyorOrdinalOverflowAreAtomic()
        {
            var walker = Session();
            walker.Process(Game(0, playerX: 1, walkerLos: true));
            Advance(walker, 1, 28);
            walker.SetOrdinalsForTests(long.MaxValue, 0);
            var beforeWalker = walker.LatestSnapshot.Value;
            Assert.Throws<OverflowException>(() => walker.Process(Game(29)));
            Assert.That(walker.LatestSnapshot.Value.Tick, Is.EqualTo(beforeWalker.Tick));
            Assert.That(walker.NextExpectedTick, Is.EqualTo(Tick(29)));

            var surveyor = Session();
            Advance(surveyor, 0, 77);
            surveyor.SetOrdinalsForTests(0, long.MaxValue);
            var beforeSurveyor = surveyor.LatestSnapshot.Value;
            Assert.Throws<OverflowException>(() => surveyor.Process(Game(78)));
            Assert.That(surveyor.LatestSnapshot.Value.Tick, Is.EqualTo(beforeSurveyor.Tick));
            Assert.That(surveyor.NextExpectedTick, Is.EqualTo(Tick(78)));
        }

        // REQ-COM-004: signed Q1000 distance stays in long space, including the complete int range, and does not affect the other staged role.
        [Test]
        public void HorizontalQ1000UsesLongDeltaAndZeroDirectionOnlyOnExactEquality()
        {
            var session = Session();
            var far = session.Process(Game(0, playerX: int.MaxValue, walkerX: int.MinValue));
            Assert.That(far.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.Approach));
            Assert.That(far.Walker.Direction, Is.EqualTo(1));
            var aligned = session.Process(Game(1, playerX: 77, walkerX: 77));
            Assert.That(aligned.Walker.Direction, Is.EqualTo(0));
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003: logical output is independent of a 30/60/144 render grouping because every step is exact-tick only.
        [TestCase(30)]
        [TestCase(60)]
        [TestCase(144)]
        public void ReplayTraceIsDeterministicAcrossRenderGroupings(int renderRate)
        {
            var expected = RunTraceWithRenderAccumulator(60);
            Assert.That(RunTraceWithRenderAccumulator(renderRate), Is.EqualTo(expected));
        }

        private static RegularEnemyBehaviorSession Session() => new RegularEnemyBehaviorSession(Tick(0));
        private static SimulationTick Tick(int value) => new SimulationTick(value);

        private static RegularEnemyBehaviorPairInput Game(int tick, bool walkerAlive = true, bool surveyorAlive = true, int playerX = 0, int walkerX = 0, int surveyorX = 0, bool walkerLos = false, bool surveyorLos = false, bool walkerHeavy = false, int walkerRevision = -1, int walkerShieldEnd = -1, bool surveyorHeavy = false, int surveyorRevision = -1, int surveyorFireLockEnd = -1) =>
            RegularEnemyBehaviorPairInput.Gameplay(Tick(tick), walkerAlive, surveyorAlive,
                WalkerReaction(tick, walkerAlive, walkerHeavy, walkerRevision, walkerShieldEnd),
                SurveyorReaction(tick, surveyorAlive, surveyorHeavy, surveyorRevision, surveyorFireLockEnd),
                playerX, walkerX, surveyorX, walkerLos, surveyorLos);

        private static RegularEnemyBehaviorPairInput Reset(int tick) => RegularEnemyBehaviorPairInput.EncounterReset(Tick(tick), ResetReaction("walker", tick), ResetReaction("surveyor", tick));

        private static RegularEnemyBehaviorPairInput GameWithWalkerReaction(int tick, RegularEnemyReactionSnapshot walkerReaction) =>
            RegularEnemyBehaviorPairInput.Gameplay(Tick(tick), true, true, walkerReaction, SurveyorReaction(tick), 0, 0, 0, false, false);

        private static RegularEnemyReactionSnapshot WalkerReaction(int tick, bool alive = true, bool heavy = false, int revision = -1, int shieldEnd = -1)
        {
            var allows = alive && shieldEnd >= 0 && tick < shieldEnd;
            return new RegularEnemyReactionSnapshot(Tick(tick), "walker", alive, heavy, revision, shieldEnd, -1, allows, alive && heavy ? 1500 : 1000, false, false, Tick(checked(tick + 1)));
        }

        private static RegularEnemyReactionSnapshot SurveyorReaction(int tick, bool alive = true, bool heavy = false, int revision = -1, int fireLockEnd = -1)
        {
            var canFire = alive && !heavy && !(fireLockEnd >= 0 && tick < fireLockEnd);
            return new RegularEnemyReactionSnapshot(Tick(tick), "surveyor", alive, heavy, revision, -1, fireLockEnd, false, 1000, alive && heavy, canFire, Tick(checked(tick + 1)));
        }

        private static RegularEnemyReactionSnapshot ResetReaction(string id, int tick) => new RegularEnemyReactionSnapshot(Tick(tick), id, true, false, -1, -1, -1, false, 1000, false, false, Tick(checked(tick + 1)));

        private static RegularEnemyReactionSnapshot RawWalkerReaction(int tick, bool alive, bool heavy, int revision, int multiplierQ1000) =>
            new RegularEnemyReactionSnapshot(Tick(tick), "walker", alive, heavy, revision, -1, -1, false, multiplierQ1000, false, false, Tick(checked(tick + 1)));

        private static void Advance(RegularEnemyBehaviorSession session, int start, int end, bool walkerHeavy = false, int walkerRevision = -1, int playerX = 0)
        {
            for (var tick = start; tick <= end; tick++) session.Process(Game(tick, playerX: playerX, walkerHeavy: walkerHeavy, walkerRevision: walkerRevision));
        }

        private static void AssertWalker(RegularEnemyBehaviorPairSnapshot snapshot, WalkerBehaviorPhase phase, int endTick)
        {
            Assert.That(snapshot.Walker.Phase, Is.EqualTo(phase));
            Assert.That(snapshot.Walker.ExclusiveEndTick, Is.EqualTo(endTick));
        }

        private static void AssertSurveyor(RegularEnemyBehaviorPairSnapshot snapshot, SurveyorBehaviorPhase phase, int endTick)
        {
            Assert.That(snapshot.Surveyor.Phase, Is.EqualTo(phase));
            Assert.That(snapshot.Surveyor.ExclusiveEndTick, Is.EqualTo(endTick));
        }

        private static void AssertUnchanged(RegularEnemyBehaviorSession session, RegularEnemyBehaviorPairSnapshot expected, int nextTick)
        {
            var actual = session.LatestSnapshot.Value;
            Assert.That(actual.Tick, Is.EqualTo(expected.Tick));
            Assert.That(actual.NextExpectedTick, Is.EqualTo(expected.NextExpectedTick));
            Assert.That(actual.Walker.Phase, Is.EqualTo(expected.Walker.Phase));
            Assert.That(actual.Walker.ExclusiveEndTick, Is.EqualTo(expected.Walker.ExclusiveEndTick));
            Assert.That(actual.Walker.AttackCycleOrdinal, Is.EqualTo(expected.Walker.AttackCycleOrdinal));
            Assert.That(actual.Surveyor.Phase, Is.EqualTo(expected.Surveyor.Phase));
            Assert.That(actual.Surveyor.ExclusiveEndTick, Is.EqualTo(expected.Surveyor.ExclusiveEndTick));
            Assert.That(actual.Surveyor.ShotOrdinal, Is.EqualTo(expected.Surveyor.ShotOrdinal));
            Assert.That(actual.WalkerRecoveryIntent.HasValue, Is.EqualTo(expected.WalkerRecoveryIntent.HasValue));
            Assert.That(actual.SurveyorShotIntent.HasValue, Is.EqualTo(expected.SurveyorShotIntent.HasValue));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(nextTick)));
        }

        private static string RunTraceWithRenderAccumulator(int renderRate)
        {
            if (renderRate <= 0) throw new ArgumentOutOfRangeException(nameof(renderRate));
            var session = Session();
            var trace = new List<string>();
            var tick = 0;
            var accumulator = 0;
            while (tick < 144)
            {
                accumulator = checked(accumulator + 60);
                while (accumulator >= renderRate && tick < 144)
                {
                    accumulator -= renderRate;
                    var snapshot = session.Process(Game(tick, playerX: tick == 0 ? 10 : 0, walkerLos: tick == 0));
                    trace.Add(string.Join("/", snapshot.Tick.Value, (int)snapshot.Walker.Phase, snapshot.Walker.ExclusiveEndTick, snapshot.Walker.AttackCycleOrdinal, (int)snapshot.Surveyor.Phase, snapshot.Surveyor.ExclusiveEndTick, snapshot.Surveyor.ShotOrdinal, snapshot.WalkerRecoveryIntent.HasValue ? snapshot.WalkerRecoveryIntent.Value.AttackCycleOrdinal : -1, snapshot.SurveyorShotIntent.HasValue ? snapshot.SurveyorShotIntent.Value.ShotOrdinal : -1, snapshot.NextExpectedTick.Value));
                    tick++;
                }
            }
            return string.Join("|", trace);
        }
    }
}
