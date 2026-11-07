using System;
using System.Collections.Generic;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Core;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Combat
{
    public sealed class BasicAttackSessionTests
    {
        // REQ-COM-001, REQ-UX-007, REQ-UX-008; partial AC-COM-003, AC-UX-007: the shared command accepts the entire nonnegative signed-64 range only.
        [Test]
        public void PublicPressAcceptsZeroAndLongMaxButRejectsNegativeIds()
        {
            Assert.DoesNotThrow(() => new BasicAttackPressed(0, 0, Tick(0)));
            Assert.DoesNotThrow(() => new BasicAttackPressed(long.MaxValue, long.MaxValue, Tick(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BasicAttackPressed(-1, 0, Tick(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BasicAttackPressed(0, -1, Tick(0)));
        }

        // REQ-COM-001; partial AC-COM-003: malformed candidate construction is rejected before a session can consume it.
        [Test]
        public void CandidateConstructorRejectsInvalidIdsAndKeys()
        {
            Assert.Throws<ArgumentException>(() => Candidate(string.Empty, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BasicAttackCandidateObservation("target", Tick(0), true, true, -1, false, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BasicAttackCandidateObservation("target", Tick(0), true, true, 0, false, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BasicAttackCandidateObservation("target", Tick(0), true, true, 0, false, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BasicAttackCandidateObservation("target", Tick(0), true, true, 0, false, 0, 1801));
        }

        // REQ-COM-004, REQ-UX-008; partial AC-COM-003: observation failures precede aim, press, sequence, and all attack-state mutation.
        [Test]
        public void ObservationValidationIsExactAtomicAndDefensivelyCopied()
        {
            var session = Session();
            Assert.Throws<ArgumentNullException>(() => session.Process(Tick(0), Mouse(0, 0), null, null));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(0));
            Assert.Throws<ArgumentException>(() => session.Process(Tick(0), Mouse(0, 0), new[] { Candidate("target", 1) }, null));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(0));
            Assert.Throws<ArgumentException>(() => session.Process(Tick(0), Mouse(0, 0), new[] { Candidate("same", 0), Candidate("same", 0) }, null));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(0));

            var mutable = new List<BasicAttackCandidateObservation> { Candidate("fixed", 0, screenDistanceSquaredKey: 0) };
            var outcome = session.Process(Tick(0), Mouse(0, 0), mutable, null);
            mutable[0] = Candidate("changed", 0, screenDistanceSquaredKey: 0);
            Assert.That(outcome.Selection.TargetId, Is.EqualTo("fixed"));
            Assert.That(outcome.Snapshot.HighlightedTargetId, Is.EqualTo("fixed"));
        }

        // REQ-COM-001, REQ-UX-013; partial AC-UX-012: mouse comfort accepts exactly 24 normalized pixels but outside pointer never acquires or fires.
        [Test]
        public void MouseInsideAndProximityBoundariesAreExact()
        {
            Assert.That(AttemptAtMouseDistance(23), Is.EqualTo(BasicAttackAttemptCode.FiredHit));
            Assert.That(AttemptAtMouseDistance(24), Is.EqualTo(BasicAttackAttemptCode.FiredHit));
            Assert.That(AttemptAtMouseDistance(25), Is.EqualTo(BasicAttackAttemptCode.FiredMiss));

            var outside = Session().Process(Tick(0), Mouse(9, 0, false), new[] { Candidate("target", 0, mouseInsideShape: true) }, Press(0, 9, 0));
            Assert.That(outside.Selection.HasTarget, Is.False);
            Assert.That(outside.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.InvalidAim));
            Assert.That(outside.DamageRequest.HasValue, Is.False);
        }

        // REQ-COM-001, REQ-UX-007; partial AC-COM-003: gamepad acquire and retain thresholds preserve the exact tenths-degree boundary.
        [Test]
        public void GamepadAcquireAndRetainBoundariesAreExact()
        {
            Assert.That(GamepadNewTarget(179), Is.EqualTo("target"));
            Assert.That(GamepadNewTarget(180), Is.EqualTo("target"));
            Assert.That(GamepadNewTarget(181), Is.EqualTo(string.Empty));

            Assert.That(GamepadRetainedTarget(259), Is.EqualTo("retained"));
            Assert.That(GamepadRetainedTarget(260), Is.EqualTo("retained"));
            Assert.That(GamepadRetainedTarget(261), Is.EqualTo(string.Empty));
        }

        // REQ-COM-001, REQ-UX-007; partial AC-COM-003: a new gamepad candidate replaces only at the full four-degree advantage boundary.
        [Test]
        public void GamepadSwitchBoundaryIsExact()
        {
            Assert.That(GamepadSwitchedTarget(161), Is.EqualTo("retained")); // 3.9 degrees better than 20.0.
            Assert.That(GamepadSwitchedTarget(160), Is.EqualTo("new"));      // exactly 4.0 degrees better.
            Assert.That(GamepadSwitchedTarget(159), Is.EqualTo("new"));      // 4.1 degrees better.
        }

        // REQ-COM-001; partial AC-COM-003: common six-unit targeting remains inclusive while unavailable and blocked candidates are excluded.
        [Test]
        public void RangeAttackabilityAndLineOfSightBoundariesAreExact()
        {
            Assert.That(FireAt(Candidate("target", 0, playerDistanceKey: 35999)).Code, Is.EqualTo(BasicAttackAttemptCode.FiredHit));
            Assert.That(FireAt(Candidate("target", 0, playerDistanceKey: 36000)).Code, Is.EqualTo(BasicAttackAttemptCode.FiredHit));
            Assert.That(FireAt(Candidate("target", 0, playerDistanceKey: 36001)).Code, Is.EqualTo(BasicAttackAttemptCode.FiredMiss));
            Assert.That(FireAt(Candidate("target", 0, isAttackable: false)).Code, Is.EqualTo(BasicAttackAttemptCode.FiredMiss));
            Assert.That(FireAt(Candidate("target", 0, isLineOfSightOpen: false)).Code, Is.EqualTo(BasicAttackAttemptCode.FiredMiss));
        }

        // REQ-COM-004; partial AC-COM-003: canonical authored target IDs, not observation order, resolve complete spatial ties.
        [Test]
        public void CandidateTieBreakIgnoresReversedObservationOrder()
        {
            var alpha = Candidate("alpha", 0, screenDistanceSquaredKey: 0, gamepadAngleKey: 0);
            var beta = Candidate("beta", 0, screenDistanceSquaredKey: 0, gamepadAngleKey: 0);
            var mouseFirst = Session().Process(Tick(0), Mouse(0, 0), new[] { beta, alpha }, null).Selection.TargetId;
            var mouseSecond = Session().Process(Tick(0), Mouse(0, 0), new[] { alpha, beta }, null).Selection.TargetId;
            var gamepadFirst = Session().Process(Tick(0), Gamepad(0, 0), new[] { beta, alpha }, null).Selection.TargetId;
            var gamepadSecond = Session().Process(Tick(0), Gamepad(0, 0), new[] { alpha, beta }, null).Selection.TargetId;
            Assert.That(mouseFirst, Is.EqualTo("alpha"));
            Assert.That(mouseFirst, Is.EqualTo(mouseSecond));
            Assert.That(gamepadFirst, Is.EqualTo("alpha"));
            Assert.That(gamepadFirst, Is.EqualTo(gamepadSecond));
        }

        // REQ-COM-004, REQ-UX-008; partial AC-COM-003: semantic precedence and sequence consumption are frozen independently of cooldown publication.
        [Test]
        public void SemanticFailurePrecedenceConsumesIdsWithoutStartingCooldown()
        {
            var session = Session();
            var fired = session.Process(Tick(0), Mouse(0, 0), Array.Empty<BasicAttackCandidateObservation>(), Press(0, 0, 0));
            Assert.That(fired.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredMiss));
            Assert.That(fired.Snapshot.CooldownEndTick, Is.EqualTo(21));

            var stale = session.Process(Tick(1), Mouse(1, 1, false), Array.Empty<BasicAttackCandidateObservation>(), Press(1, 99, 1));
            var invalid = session.Process(Tick(2), Mouse(2, 2, false), Array.Empty<BasicAttackCandidateObservation>(), Press(2, 2, 2));
            var cooldown = session.Process(Tick(3), Mouse(3, 3), new[] { Candidate("target", 3) }, Press(3, 3, 3));
            var missing = session.Process(Tick(4), null, Array.Empty<BasicAttackCandidateObservation>(), Press(4, 4, 4));
            Assert.That(stale.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.StaleAim));
            Assert.That(invalid.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.InvalidAim));
            Assert.That(cooldown.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.Cooldown));
            Assert.That(missing.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.StaleAim));
            Assert.That(stale.DamageRequest.HasValue, Is.False);
            Assert.That(invalid.DamageRequest.HasValue, Is.False);
            Assert.That(cooldown.DamageRequest.HasValue, Is.False);
            Assert.That(missing.DamageRequest.HasValue, Is.False);
            Assert.That(stale.Snapshot.CooldownEndTick, Is.EqualTo(21));
            Assert.That(invalid.Snapshot.CooldownEndTick, Is.EqualTo(21));
            Assert.That(cooldown.Snapshot.CooldownEndTick, Is.EqualTo(21));
            Assert.That(missing.Snapshot.CooldownEndTick, Is.EqualTo(21));
            Assert.That(missing.Snapshot.ConsumedSequenceCount, Is.EqualTo(5));
            Assert.Throws<InvalidOperationException>(() => session.Process(Tick(5), null, Array.Empty<BasicAttackCandidateObservation>(), Press(2, 5, 5)));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(5));
            var afterDuplicate = session.Snapshot(Tick(4));
            Assert.That(afterDuplicate.CooldownEndTick, Is.EqualTo(21));
            Assert.That(afterDuplicate.ConsumedSequenceCount, Is.EqualTo(5));
        }

        // REQ-COM-001; partial AC-COM-003: both hit and miss lock the same interval; only a hit creates the exact M1 request.
        [Test]
        public void HitAndMissShareCooldownBoundaryAndOnlyHitBuildsExactDamageRequest()
        {
            var hit = Session();
            var atZero = hit.Process(Tick(0), Mouse(7, 0), new[] { Candidate("walker", 0) }, Press(7, 7, 0));
            Assert.That(atZero.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredHit));
            Assert.That(RequestTrace(atZero.DamageRequest), Is.EqualTo("Player.BasicAttack/7:player:walker:3:BasicAttack:0"));
            AdvanceEmpty(hit, 1, 19);
            Assert.That(hit.Process(Tick(20), Mouse(8, 20), new[] { Candidate("walker", 20) }, Press(8, 8, 20)).Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.Cooldown));
            Assert.That(hit.Process(Tick(21), Mouse(9, 21), new[] { Candidate("walker", 21) }, Press(9, 9, 21)).Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredHit));

            var miss = Session();
            Assert.That(miss.Process(Tick(0), Mouse(0, 0), Array.Empty<BasicAttackCandidateObservation>(), Press(0, 0, 0)).Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredMiss));
            AdvanceEmpty(miss, 1, 19);
            Assert.That(miss.Process(Tick(20), Mouse(1, 20), Array.Empty<BasicAttackCandidateObservation>(), Press(1, 1, 20)).Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.Cooldown));
            var missAtBoundary = miss.Process(Tick(21), Mouse(2, 21), Array.Empty<BasicAttackCandidateObservation>(), Press(2, 2, 21));
            Assert.That(missAtBoundary.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredMiss));
            Assert.That(missAtBoundary.DamageRequest.HasValue, Is.False);
        }

        // REQ-COM-001, REQ-UX-007; partial AC-UX-007: equivalent admitted mouse and gamepad intent can select and fire at the same target.
        [Test]
        public void EquivalentMouseAndGamepadIntentSelectTheSameTarget()
        {
            var mouse = Session().Process(Tick(0), Mouse(0, 0), new[] { Candidate("shared", 0, mouseInsideShape: true, gamepadAngleKey: 0) }, Press(0, 0, 0));
            var gamepad = Session().Process(Tick(0), Gamepad(0, 0), new[] { Candidate("shared", 0, mouseInsideShape: true, gamepadAngleKey: 0) }, Press(0, 0, 0));
            Assert.That(mouse.Selection.TargetId, Is.EqualTo("shared"));
            Assert.That(gamepad.Selection.TargetId, Is.EqualTo("shared"));
            Assert.That(mouse.DamageRequest.Value.TargetId, Is.EqualTo(gamepad.DamageRequest.Value.TargetId));
        }

        // REQ-COM-004; partial AC-COM-003: repeated, stale, wrong, and exhausted ticks cannot partly consume a sequence or advance the session.
        [Test]
        public void TickAndOverflowFailuresAreAtomic()
        {
            var session = Session();
            Assert.Throws<InvalidOperationException>(() => session.Process(Tick(1), null, Array.Empty<BasicAttackCandidateObservation>(), null));
            Assert.Throws<ArgumentException>(() => session.Process(Tick(0), Mouse(0, 1), Array.Empty<BasicAttackCandidateObservation>(), null));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(0));
            session.Process(Tick(0), null, Array.Empty<BasicAttackCandidateObservation>(), null);
            Assert.Throws<InvalidOperationException>(() => session.Process(Tick(0), null, Array.Empty<BasicAttackCandidateObservation>(), null));

            var exhausted = new BasicAttackSession(Tick(int.MaxValue - 1));
            exhausted.Process(Tick(int.MaxValue - 1), null, Array.Empty<BasicAttackCandidateObservation>(), null);
            Assert.That(exhausted.NextExpectedTick.Value, Is.EqualTo(int.MaxValue));
            Assert.Throws<OverflowException>(() => exhausted.Process(Tick(int.MaxValue), null, Array.Empty<BasicAttackCandidateObservation>(), null));
            Assert.That(exhausted.NextExpectedTick.Value, Is.EqualTo(int.MaxValue));
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-003: fired hit and miss cooldown arithmetic must overflow before publishing any session state.
        [Test]
        public void CooldownOverflowOnHitAndMissIsAtomic()
        {
            var tick = int.MaxValue - 1;
            var hit = new BasicAttackSession(Tick(tick));
            Assert.Throws<OverflowException>(() => hit.Process(Tick(tick), Mouse(0, tick), new[] { Candidate("target", tick) }, Press(0, 0, tick)));
            AssertUnpublishedOverflow(hit, tick);

            var miss = new BasicAttackSession(Tick(tick));
            Assert.Throws<OverflowException>(() => miss.Process(Tick(tick), Mouse(0, tick), Array.Empty<BasicAttackCandidateObservation>(), Press(0, 0, tick)));
            AssertUnpublishedOverflow(miss, tick);
        }

        // REQ-COM-001, REQ-COM-004, REQ-UX-007; partial AC-COM-003, AC-UX-007: render grouping cannot alter 60Hz attack intent, selection, cooldown, or request traces.
        [Test]
        public void SameSixtyHertzAttackScriptIsIdenticalAtThirtySixtyAndOneFortyFourRenderGrouping()
        {
            var atThirty = Replay(30);
            var atSixty = Replay(60);
            var atOneFortyFour = Replay(144);
            Assert.That(atThirty, Is.EqualTo(atSixty));
            Assert.That(atSixty, Is.EqualTo(atOneFortyFour));
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-003. M2A preview is mutation-free and exact commit preserves legacy Process behavior.
        [Test]
        public void PreviewCommitParityCoversNeutralAimHitMissAndCooldown()
        {
            var legacy = Session();
            var staged = Session();
            for (var tick = 0; tick < 5; tick++)
            {
                AimSample? aim = tick == 0 ? null : Mouse(tick, tick);
                IReadOnlyList<BasicAttackCandidateObservation> observations = tick == 2
                    ? new[] { Candidate("target", tick) }
                    : Array.Empty<BasicAttackCandidateObservation>();
                BasicAttackPressed? press = tick == 2 || tick == 3 || tick == 4 ? Press(tick, tick, tick) : (BasicAttackPressed?)null;
                var expected = legacy.Process(Tick(tick), aim, observations, press);
                var before = staged.Snapshot(Tick(Math.Max(0, tick - 1)));
                var candidate = staged.PreviewNext(Tick(tick), aim, observations, press);
                Assert.That(staged.NextExpectedTick, Is.EqualTo(Tick(tick)));
                var retained = staged.Snapshot(Tick(Math.Max(0, tick - 1)));
                Assert.That(retained.HighlightedTargetId, Is.EqualTo(before.HighlightedTargetId));
                Assert.That(retained.CooldownEndTick, Is.EqualTo(before.CooldownEndTick));
                Assert.That(retained.ConsumedSequenceCount, Is.EqualTo(before.ConsumedSequenceCount));
                Assert.That(retained.NextExpectedTick, Is.EqualTo(before.NextExpectedTick));
                var actual = staged.CommitPreflighted(candidate);
                Assert.That(OutcomeTrace(actual), Is.EqualTo(OutcomeTrace(expected)));
            }

            var legacyMiss = Session().Process(Tick(0), Mouse(9, 0), Array.Empty<BasicAttackCandidateObservation>(), Press(9, 9, 0));
            var stagedMissOwner = Session();
            var stagedMiss = stagedMissOwner.CommitPreflighted(stagedMissOwner.PreviewNext(Tick(0), Mouse(9, 0),
                Array.Empty<BasicAttackCandidateObservation>(), Press(9, 9, 0)));
            Assert.That(stagedMiss.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredMiss));
            Assert.That(OutcomeTrace(stagedMiss), Is.EqualTo(OutcomeTrace(legacyMiss)));
        }

        // REQ-COM-004; AC-COM-003. Candidate ownership and exact tick state prevent forged, stale, and double commits.
        [Test]
        public void ForgedStaleAndDoubleCandidatesRejectAtomically()
        {
            var session = Session();
            var foreign = Session().PreviewNext(Tick(0), null, Array.Empty<BasicAttackCandidateObservation>(), null);
            Assert.Throws<InvalidOperationException>(() => session.CommitPreflighted(foreign));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(0)));

            var stale = session.PreviewNext(Tick(0), null, Array.Empty<BasicAttackCandidateObservation>(), null);
            var committed = session.PreviewNext(Tick(0), Mouse(1, 0), Array.Empty<BasicAttackCandidateObservation>(), null);
            session.CommitPreflighted(committed);
            Assert.Throws<InvalidOperationException>(() => session.CommitPreflighted(stale));
            Assert.Throws<InvalidOperationException>(() => session.CommitPreflighted(committed));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(1)));
            Assert.That(session.Snapshot(Tick(0)).ConsumedSequenceCount, Is.Zero);
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-003. Preview owns duplicate-sequence rejection without a Combat shadow store.
        [Test]
        public void DuplicateSequencePreviewRejectsWithoutMutationAndMatchesProcess()
        {
            var staged = Session();
            var legacy = Session();
            var firstStaged = staged.PreviewNext(Tick(0), Mouse(7, 0), Array.Empty<BasicAttackCandidateObservation>(), Press(41, 7, 0));
            staged.CommitPreflighted(firstStaged);
            legacy.Process(Tick(0), Mouse(7, 0), Array.Empty<BasicAttackCandidateObservation>(), Press(41, 7, 0));
            var before = staged.Snapshot(Tick(0));

            Assert.Throws<InvalidOperationException>(() => staged.PreviewNext(Tick(1), Mouse(8, 1),
                Array.Empty<BasicAttackCandidateObservation>(), Press(41, 8, 1)));
            Assert.Throws<InvalidOperationException>(() => legacy.Process(Tick(1), Mouse(8, 1),
                Array.Empty<BasicAttackCandidateObservation>(), Press(41, 8, 1)));
            var retained = staged.Snapshot(Tick(0));
            Assert.That(retained.HighlightedTargetId, Is.EqualTo(before.HighlightedTargetId));
            Assert.That(retained.CooldownEndTick, Is.EqualTo(before.CooldownEndTick));
            Assert.That(retained.ConsumedSequenceCount, Is.EqualTo(before.ConsumedSequenceCount));
            Assert.That(retained.NextExpectedTick, Is.EqualTo(before.NextExpectedTick));
        }

        private static BasicAttackAttemptCode AttemptAtMouseDistance(int pixels)
        {
            var outcome = Session().Process(Tick(0), Mouse(0, 0), new[] { Candidate("target", 0, screenDistanceSquaredKey: pixels * pixels) }, Press(0, 0, 0));
            return outcome.Attempt.Value.Code;
        }

        private static string GamepadNewTarget(int angleKey) =>
            Session().Process(Tick(0), Gamepad(0, 0), new[] { Candidate("target", 0, gamepadAngleKey: angleKey) }, null).Selection.TargetId;

        private static string GamepadRetainedTarget(int retainedAngle)
        {
            var session = Session();
            session.Process(Tick(0), Gamepad(0, 0), new[] { Candidate("retained", 0, gamepadAngleKey: 170) }, null);
            return session.Process(Tick(1), Gamepad(1, 1), new[] { Candidate("retained", 1, gamepadAngleKey: retainedAngle) }, null).Selection.TargetId;
        }

        private static string GamepadSwitchedTarget(int newAngle)
        {
            var session = Session();
            session.Process(Tick(0), Gamepad(0, 0), new[] { Candidate("retained", 0, gamepadAngleKey: 170) }, null);
            return session.Process(Tick(1), Gamepad(1, 1), new[]
            {
                Candidate("retained", 1, gamepadAngleKey: 200),
                Candidate("new", 1, gamepadAngleKey: newAngle)
            }, null).Selection.TargetId;
        }

        private static BasicAttackAttempt FireAt(BasicAttackCandidateObservation candidate) =>
            Session().Process(Tick(0), Mouse(0, 0), new[] { candidate }, Press(0, 0, 0)).Attempt.Value;

        private static string Replay(int renderFramesPerSecond)
        {
            var session = Session();
            var trace = new List<string>();
            var accumulator = 0;
            var tick = 0;
            while (tick < 60)
            {
                accumulator += 60;
                while (accumulator >= renderFramesPerSecond && tick < 60)
                {
                    accumulator -= renderFramesPerSecond;
                    var sample = tick % 2 == 0 ? Mouse(tick, tick) : Gamepad(tick, tick);
                    var observations = new[] { Candidate("target", tick, mouseInsideShape: true, gamepadAngleKey: 0) };
                    BasicAttackPressed? press = tick == 0 || tick == 20 || tick == 21 || tick == 42 ? Press(tick, tick, tick) : (BasicAttackPressed?)null;
                    var outcome = session.Process(Tick(tick), sample, observations, press);
                    trace.Add(
                        tick + ":selection=" + outcome.Selection.Tick.Value + "," + outcome.Selection.TargetId
                        + ":attempt=" + AttemptTrace(outcome.Attempt)
                        + ":request=" + RequestTrace(outcome.DamageRequest)
                        + ":snapshot=" + outcome.Snapshot.Tick.Value + "," + outcome.Snapshot.HighlightedTargetId + "," + outcome.Snapshot.CooldownEndTick + "," + outcome.Snapshot.ConsumedSequenceCount + "," + outcome.Snapshot.NextExpectedTick.Value);
                    tick++;
                }
            }

            return string.Join("|", trace);
        }

        private static void AdvanceEmpty(BasicAttackSession session, int firstTick, int lastTick)
        {
            for (var tick = firstTick; tick <= lastTick; tick++) session.Process(Tick(tick), null, Array.Empty<BasicAttackCandidateObservation>(), null);
        }

        private static string RequestTrace(DamageRequest? request)
        {
            if (!request.HasValue) return "none";
            var value = request.Value;
            return value.RequestId + ":" + value.SourceId + ":" + value.TargetId + ":" + value.Amount + ":" + value.Kind + ":" + value.Tick.Value;
        }

        private static string AttemptTrace(BasicAttackAttempt? attempt)
        {
            if (!attempt.HasValue) return "none";
            var value = attempt.Value;
            return value.AttackSequenceId + "," + value.Tick.Value + "," + value.Code + "," + value.SelectedTargetId;
        }

        private static string OutcomeTrace(BasicAttackTickOutcome value) =>
            value.Selection.Tick.Value + ":" + value.Selection.TargetId + ":" + AttemptTrace(value.Attempt) + ":" +
            RequestTrace(value.DamageRequest) + ":" + value.Snapshot.Tick.Value + ":" + value.Snapshot.HighlightedTargetId + ":" +
            value.Snapshot.CooldownEndTick + ":" + value.Snapshot.ConsumedSequenceCount + ":" + value.Snapshot.NextExpectedTick.Value;

        private static void AssertUnpublishedOverflow(BasicAttackSession session, int tick)
        {
            var snapshot = session.Snapshot(Tick(tick));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(tick));
            Assert.That(snapshot.HighlightedTargetId, Is.EqualTo(string.Empty));
            Assert.That(snapshot.CooldownEndTick, Is.EqualTo(-1));
            Assert.That(snapshot.ConsumedSequenceCount, Is.EqualTo(0));
        }

        private static BasicAttackSession Session() => new BasicAttackSession(Tick(0));
        private static SimulationTick Tick(int value) => new SimulationTick(value);
        private static BasicAttackPressed Press(long sequenceId, long aimSampleId, int tick) => new BasicAttackPressed(sequenceId, aimSampleId, Tick(tick));
        private static AimSample Mouse(long sampleId, int tick, bool inside = true) => new AimSample(sampleId, AimSource.MousePointer, 960, 540, inside, 4096, 0, Tick(tick - 1), Tick(tick));
        private static AimSample Gamepad(long sampleId, int tick) => new AimSample(sampleId, AimSource.GamepadStick, null, null, null, 4096, 0, Tick(tick - 1), Tick(tick));

        private static BasicAttackCandidateObservation Candidate(
            string targetId,
            int tick,
            bool isAttackable = true,
            bool isLineOfSightOpen = true,
            int playerDistanceKey = 0,
            bool mouseInsideShape = false,
            int screenDistanceSquaredKey = 0,
            int gamepadAngleKey = 0) =>
            new BasicAttackCandidateObservation(targetId, Tick(tick), isAttackable, isLineOfSightOpen, playerDistanceKey, mouseInsideShape, screenDistanceSquaredKey, gamepadAngleKey);
    }
}
