using System;
using System.Collections.Generic;
using NUnit.Framework;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Combat.Tests
{
    public sealed class RegularEnemyLocomotionSessionTests
    {
        // REQ-COM-004; partial AC-COM-003.
        [Test]
        public void FirstConsecutiveAndTerminalTicksAreExactAndFailuresPreservePublication()
        {
            var session = Session(7);
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Game(6)));
            var first = Commit(session, Game(7));
            Assert.That(first.Tick, Is.EqualTo(Tick(7)));
            Assert.That(first.NextExpectedTick, Is.EqualTo(Tick(8)));
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Game(7)));
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Game(9)));
            Assert.That(session.LatestSnapshot.Value.Tick, Is.EqualTo(Tick(7)));

            var terminal = Session(int.MaxValue - 1);
            var committed = Commit(terminal, Game(int.MaxValue - 1));
            Assert.That(committed.NextExpectedTick, Is.EqualTo(Tick(int.MaxValue)));
            var terminalInput = RegularEnemyLocomotionPairInput.Gameplay(Tick(int.MaxValue), default, default, default);
            Assert.Throws<ArgumentOutOfRangeException>(() => terminal.PreviewNext(terminalInput));
            AssertPairUnchanged(terminal, committed);
        }

        // REQ-COM-004; partial AC-COM-003.
        [Test]
        public void DefaultWrongRoleAndInvalidDiscriminantPublicationsAreRejectedWithoutMutation()
        {
            var session = Session();
            Assert.Throws<ArgumentException>(() => session.PreviewNext(default));
            Assert.Throws<ArgumentOutOfRangeException>(() => RegularEnemyLocomotionPairInput.AssertInvalidDiscriminantRejectedForTests());
            var wrong = RegularEnemyLocomotionPairInput.Gameplay(Tick(0), Behavior(0), SurveyorReaction(0), SurveyorReaction(0));
            Assert.Throws<ArgumentException>(() => session.PreviewNext(wrong));
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(0)));
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-001.
        [Test]
        public void ApproachDashAndRelocateRetainExactRemaindersAcrossReachableWindows()
        {
            var approach = Session();
            for (var t = 0; t < 60; t++) Commit(approach, Game(t));
            AssertMotion(approach.LatestSnapshot.Value.Walker, 12288, 0, 12288, 0, 0);

            var dash = Session();
            for (var t = 0; t < 12; t++) Commit(dash, Game(t, walker: Walker(WalkerBehaviorPhase.DashActive, RegularEnemyMovementIntent.DashHorizontal, 1, latched: 1)));
            AssertMotion(dash.LatestSnapshot.Value.Walker, 9830, 0, 49152, 24, 0);

            var relocate = Session();
            for (var t = 0; t < 48; t++) Commit(relocate, Game(t));
            AssertMotion(relocate.LatestSnapshot.Value.Surveyor, 8192, 40960, 10240, 0, 0);
        }

        // REQ-COM-002, REQ-COM-005; affected AC-WT-002, AC-WT-005.
        [Test]
        public void HeavyRatesRespectGroundedAirborneAndClearWithoutStaleRemainders()
        {
            var grounded = Session();
            var groundedPreview = grounded.PreviewNext(Game(0, walkerReaction: WalkerReaction(0, heavy: true, revision: 1)));
            Assert.That(groundedPreview.Walker.DesiredVelocity.X, Is.EqualTo(9216));
            var groundedDash = Session().PreviewNext(Game(0,
                walker: Walker(WalkerBehaviorPhase.DashActive, RegularEnemyMovementIntent.DashHorizontal, 1, latched: 1),
                walkerReaction: WalkerReaction(0, heavy: true, revision: 1)));
            Assert.That(groundedDash.Walker.DesiredVelocity.X, Is.EqualTo(36864));

            var airborne = Session(walkerGrounded: false);
            var airbornePreview = airborne.PreviewNext(Game(0, walkerReaction: WalkerReaction(0, heavy: true, revision: 1)));
            Assert.That(airbornePreview.Walker.DesiredVelocity.X, Is.EqualTo(3226));
            Assert.That(airbornePreview.Walker.DesiredVelocity.Y, Is.EqualTo(-3003));
            Assert.That(airbornePreview.Walker.GravityRemainder, Is.EqualTo(-44));
            Commit(airborne, Game(0, walkerReaction: WalkerReaction(0, heavy: true, revision: 1)));
            var cleared = airborne.PreviewNext(Game(1, walker: Walker(WalkerBehaviorPhase.Recovery, RegularEnemyMovementIntent.None, 0), walkerReaction: WalkerReaction(1)));
            Assert.That(cleared.Walker.DesiredVelocity.X, Is.Zero);
            Assert.That(cleared.Walker.XPositionRemainder, Is.Zero);

            var airborneDash = Session(walkerGrounded: false).PreviewNext(Game(0,
                walker: Walker(WalkerBehaviorPhase.DashActive, RegularEnemyMovementIntent.DashHorizontal, 1, latched: 1),
                walkerReaction: WalkerReaction(0, heavy: true, revision: 1)));
            Assert.That(airborneDash.Walker.DesiredVelocity.X, Is.EqualTo(12902));

            var alignedDash = Session();
            Commit(alignedDash, Game(0));
            Assert.That(alignedDash.LatestSnapshot.Value.Walker.XPositionRemainder, Is.Not.Zero);
            var zeroDash = alignedDash.PreviewNext(Game(1,
                walker: Walker(WalkerBehaviorPhase.DashActive, RegularEnemyMovementIntent.DashHorizontal, 0, latched: 0)));
            Assert.That(zeroDash.Walker.DesiredVelocity.X, Is.Zero);
            Assert.That(zeroDash.Walker.XPositionRemainder, Is.Zero);

            var surveyor = Session(surveyorGrounded: false);
            var descentInput = Game(0, surveyor: Surveyor(SurveyorBehaviorPhase.Suppressed, RegularEnemyMovementIntent.Downward, 0),
                surveyorReaction: SurveyorReaction(0, heavy: true, revision: 1));
            Assert.That(surveyor.PreviewNext(descentInput).Surveyor.DesiredVelocity.Y, Is.EqualTo(-3003));
            Commit(surveyor, descentInput);
            var released = surveyor.PreviewNext(Game(1, surveyor: Surveyor(SurveyorBehaviorPhase.Suppressed, RegularEnemyMovementIntent.None, 0),
                surveyorReaction: SurveyorReaction(1, revision: 2, fireLockEnd: 10)));
            Assert.That(released.Surveyor.DesiredVelocity.Y, Is.Zero);
            Assert.That(released.Surveyor.YPositionRemainder, Is.Zero);
            Assert.That(released.Surveyor.GravityRemainder, Is.Zero);
        }

        // REQ-COM-002, REQ-COM-005; affected AC-WT-005.
        [Test]
        public void HeavyGravityClampsAndClearsOnlyGravityRemainder()
        {
            var session = Session(walkerGrounded: false);
            for (var t = 0; t < 17; t++) Commit(session, Game(t, walker: Walker(WalkerBehaviorPhase.Recovery, RegularEnemyMovementIntent.None, 0), walkerReaction: WalkerReaction(t, heavy: true, revision: 1)));
            var walker = session.LatestSnapshot.Value.Walker;
            Assert.That(walker.Velocity.Y, Is.EqualTo(-49152));
            Assert.That(walker.GravityRemainder, Is.Zero);
            Assert.That(Math.Abs(walker.YPositionRemainder), Is.LessThan(60));
        }

        // REQ-COM-001, REQ-COM-002; partial AC-COM-001.
        [TestCase(0, 1, 1, 0)]
        [TestCase(0, 1, 0, 0)]
        [TestCase(0, 1, -1, 0)]
        [TestCase(1, 0, 0, -1)]
        [TestCase(2, 2, -1, -1)]
        [TestCase(2, 2, 0, 0)]
        [TestCase(2, 2, 1, 1)]
        [TestCase(3, 0, 0, -1)]
        public void EveryLiveWalkerPhaseIntentRowPreviews(int phaseValue, int movementValue, int direction, int latched)
        {
            var session = Session();
            var phase = (WalkerBehaviorPhase)phaseValue;
            var movement = (RegularEnemyMovementIntent)movementValue;
            Assert.DoesNotThrow(() => session.PreviewNext(Game(0, walker: Walker(phase, movement, direction, latched: latched))));
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-003.
        [Test]
        public void WalkerInvalidPhaseIntentDirectionAndAliveMutationsAreAtomic()
        {
            var session = Session();
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Game(0, walker: Walker(WalkerBehaviorPhase.Approach, RegularEnemyMovementIntent.None, 1))));
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Game(0, walker: Walker(WalkerBehaviorPhase.DashActive, RegularEnemyMovementIntent.DashHorizontal, 1, latched: -1))));
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Game(0, walker: Walker(WalkerBehaviorPhase.Approach, RegularEnemyMovementIntent.Horizontal, 2))));
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Game(0, walkerReaction: WalkerReaction(0, alive: false))));
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
        }

        // REQ-COM-002, REQ-COM-005; partial AC-COM-001, affected AC-WT-005.
        [Test]
        public void EverySurveyorPhaseReactionRowPreviewsAndForbiddenRelocateHeavyRejects()
        {
            var phases = new[] { SurveyorBehaviorPhase.Relocate, SurveyorBehaviorPhase.FireTelegraph, SurveyorBehaviorPhase.Fire, SurveyorBehaviorPhase.Cooldown };
            foreach (var phase in phases)
            {
                var movement = phase == SurveyorBehaviorPhase.Relocate ? RegularEnemyMovementIntent.Horizontal : RegularEnemyMovementIntent.None;
                var direction = phase == SurveyorBehaviorPhase.Relocate ? 1 : 0;
                Assert.DoesNotThrow(() => Session().PreviewNext(Game(0, surveyor: Surveyor(phase, movement, direction))));
            }
            Assert.DoesNotThrow(() => Session().PreviewNext(Game(0,
                surveyor: Surveyor(SurveyorBehaviorPhase.Relocate, RegularEnemyMovementIntent.Horizontal, -1))));
            Assert.DoesNotThrow(() => Session(surveyorGrounded: false).PreviewNext(Game(0,
                surveyor: Surveyor(SurveyorBehaviorPhase.Suppressed, RegularEnemyMovementIntent.Downward, 0),
                surveyorReaction: SurveyorReaction(0, heavy: true, revision: 1))));
            Assert.DoesNotThrow(() => Session().PreviewNext(Game(0,
                surveyor: Surveyor(SurveyorBehaviorPhase.Suppressed, RegularEnemyMovementIntent.None, 0),
                surveyorReaction: SurveyorReaction(0, fireLockEnd: 10))));
            Assert.Throws<InvalidOperationException>(() => Session().PreviewNext(Game(0,
                surveyorReaction: SurveyorReaction(0, heavy: true, revision: 1))));
            Assert.DoesNotThrow(() => Session().PreviewNext(Game(0,
                surveyor: Surveyor(SurveyorBehaviorPhase.Dead, RegularEnemyMovementIntent.None, 0, alive: false),
                surveyorReaction: SurveyorReaction(0, alive: false))));
            Assert.Throws<InvalidOperationException>(() => Session().PreviewNext(Game(0,
                surveyor: Surveyor(SurveyorBehaviorPhase.Relocate, RegularEnemyMovementIntent.Horizontal, 0))));
            Assert.Throws<InvalidOperationException>(() => Session().PreviewNext(Game(0,
                surveyor: Surveyor(SurveyorBehaviorPhase.Relocate, RegularEnemyMovementIntent.Horizontal, 1, alive: false),
                surveyorReaction: SurveyorReaction(0, alive: false))));
        }

        // REQ-COM-004; partial AC-COM-003.
        [Test]
        public void PreviewValidationAndCommitAreMutationFreeUntilExactCandidateCommit()
        {
            var session = Session();
            var input = Game(0);
            var previewA = session.PreviewNext(input);
            var previewB = session.PreviewNext(input);
            AssertPrediction(previewA, previewB);
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            var resolution = Unblocked(previewA);
            var candidate = session.ValidateResolvedNext(input, resolution);
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            var committed = session.CommitResolvedNext(input, resolution, candidate);
            Assert.That(committed.Tick, Is.EqualTo(Tick(0)));
        }

        // REQ-COM-004; partial AC-COM-003.
        [Test]
        public void MalformedEitherRoleResolutionAndForgedCandidatePreserveWholePair()
        {
            var session = Session();
            var baseline = Commit(session, Game(0));
            var input = Game(1);
            var preview = session.PreviewNext(input);
            var good = Unblocked(preview);
            var badSurveyor = new RegularEnemyCollisionResolution(Tick(1), "surveyor", preview.Surveyor.StartPosition,
                preview.Surveyor.PredictedPosition, new PositionQ4096(preview.Surveyor.PredictedPosition.X + 1, preview.Surveyor.PredictedPosition.Y), false, false, true, true);
            Assert.Throws<InvalidOperationException>(() => session.ValidateResolvedNext(input, new RegularEnemyCollisionPairResolution(good.Walker, badSurveyor)));
            var candidate = session.ValidateResolvedNext(input, good);
            var forgedSnapshot = new RegularEnemyMotionPairSnapshot(candidate.Snapshot.Tick,
                new RegularEnemyMotionSnapshot("walker", new PositionQ4096(99, 0), candidate.Snapshot.Walker.Velocity, 0, 0, 0, true, false),
                candidate.Snapshot.Surveyor, candidate.Snapshot.NextExpectedTick);
            Assert.Throws<InvalidOperationException>(() => session.CommitResolvedNext(input, good, new RegularEnemyResolvedPairCandidate(forgedSnapshot)));
            AssertPairUnchanged(session, baseline);
        }

        // REQ-COM-004; partial AC-COM-003.
        [Test]
        public void ResolutionEnvelopeRequiresBlockedTruncationAndClearsOnlyBlockedAxis()
        {
            var session = Session();
            var input = Game(0);
            var preview = session.PreviewNext(input);
            var w = preview.Walker;
            var truncated = new RegularEnemyCollisionResolution(Tick(0), "walker", w.StartPosition, w.PredictedPosition,
                new PositionQ4096(w.StartPosition.X + 100, w.PredictedPosition.Y), false, false, true, true);
            var normal = Unblocked(preview);
            Assert.Throws<InvalidOperationException>(() => session.ValidateResolvedNext(input, new RegularEnemyCollisionPairResolution(truncated, normal.Surveyor)));
            var blocked = new RegularEnemyCollisionResolution(Tick(0), "walker", w.StartPosition, w.PredictedPosition,
                new PositionQ4096(w.StartPosition.X + 100, w.PredictedPosition.Y), true, false, true, true);
            var candidate = session.ValidateResolvedNext(input, new RegularEnemyCollisionPairResolution(blocked, normal.Surveyor));
            Assert.That(candidate.Snapshot.Walker.Velocity.X, Is.Zero);
            Assert.That(candidate.Snapshot.Walker.XPositionRemainder, Is.Zero);
            Assert.That(candidate.Snapshot.Walker.Grounded, Is.True);
        }

        // REQ-COM-004; partial AC-COM-003. Every integer-envelope and support mutation is rejected pair-atomically.
        [Test]
        public void ResolutionEnvelopeRejectsOppositeExcessUnmovedDuplicateAndInvalidSupportVariants()
        {
            var session = Session();
            var input = Game(0);
            var preview = session.PreviewNext(input);
            var normal = Unblocked(preview);
            var w = preview.Walker;

            RegularEnemyCollisionPairResolution WithWalker(PositionQ4096 resolved, bool xBlocked, bool yBlocked,
                bool support, bool grounded) => new RegularEnemyCollisionPairResolution(
                    new RegularEnemyCollisionResolution(w.Tick, w.RoleId, w.StartPosition, w.PredictedPosition,
                        resolved, xBlocked, yBlocked, support, grounded), normal.Surveyor);

            Assert.Throws<InvalidOperationException>(() => session.ValidateResolvedNext(input,
                WithWalker(new PositionQ4096(w.StartPosition.X - 1, w.PredictedPosition.Y), true, false, true, true)));
            Assert.Throws<InvalidOperationException>(() => session.ValidateResolvedNext(input,
                WithWalker(new PositionQ4096(w.PredictedPosition.X + 1, w.PredictedPosition.Y), true, false, true, true)));
            Assert.Throws<InvalidOperationException>(() => session.ValidateResolvedNext(input,
                WithWalker(new PositionQ4096(w.PredictedPosition.X, w.StartPosition.Y + 1), false, true, false, false)));
            Assert.Throws<ArgumentException>(() => session.ValidateResolvedNext(input,
                new RegularEnemyCollisionPairResolution(normal.Walker, normal.Walker)));

            var airborne = Session(walkerGrounded: false);
            var airborneInput = Game(0, walker: Walker(WalkerBehaviorPhase.Recovery, RegularEnemyMovementIntent.None, 0));
            var airbornePreview = airborne.PreviewNext(airborneInput);
            var airborneNormal = Unblocked(airbornePreview);
            var aw = airbornePreview.Walker;
            var falseSupport = new RegularEnemyCollisionResolution(aw.Tick, aw.RoleId, aw.StartPosition, aw.PredictedPosition,
                aw.PredictedPosition, false, false, true, true);
            Assert.Throws<InvalidOperationException>(() => airborne.ValidateResolvedNext(airborneInput,
                new RegularEnemyCollisionPairResolution(falseSupport, airborneNormal.Surveyor)));

            var supported = Session();
            var supportedInput = Game(0);
            var supportedPreview = supported.PreviewNext(supportedInput);
            var supportedNormal = Unblocked(supportedPreview);
            var sw = supportedPreview.Walker;
            var supportWithoutGround = new RegularEnemyCollisionResolution(sw.Tick, sw.RoleId, sw.StartPosition, sw.PredictedPosition,
                sw.PredictedPosition, false, false, true, false);
            Assert.Throws<InvalidOperationException>(() => supported.ValidateResolvedNext(supportedInput,
                new RegularEnemyCollisionPairResolution(supportWithoutGround, supportedNormal.Surveyor)));
            var supportWithBlock = new RegularEnemyCollisionResolution(sw.Tick, sw.RoleId, sw.StartPosition, sw.PredictedPosition,
                sw.PredictedPosition, false, true, true, true);
            Assert.Throws<InvalidOperationException>(() => supported.ValidateResolvedNext(supportedInput,
                new RegularEnemyCollisionPairResolution(supportWithBlock, supportedNormal.Surveyor)));
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(0)));
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-001, AC-COM-003.
        [Test]
        public void DownwardBlockAndStationarySupportExactlyOwnGrounded()
        {
            var falling = Session(walkerGrounded: false);
            var input = Game(0, walker: Walker(WalkerBehaviorPhase.Recovery, RegularEnemyMovementIntent.None, 0));
            var preview = falling.PreviewNext(input);
            var w = preview.Walker;
            var floor = new RegularEnemyCollisionResolution(Tick(0), "walker", w.StartPosition, w.PredictedPosition,
                new PositionQ4096(w.PredictedPosition.X, w.StartPosition.Y), false, true, false, true);
            var pair = Unblocked(preview);
            var candidate = falling.ValidateResolvedNext(input, new RegularEnemyCollisionPairResolution(floor, pair.Surveyor));
            Assert.That(candidate.Snapshot.Walker.Velocity.Y, Is.Zero);
            Assert.That(candidate.Snapshot.Walker.YPositionRemainder, Is.Zero);
            Assert.That(candidate.Snapshot.Walker.GravityRemainder, Is.Zero);

            var supported = Session();
            var supportedPreview = supported.PreviewNext(Game(0));
            var unsupported = new RegularEnemyCollisionResolution(Tick(0), "walker", supportedPreview.Walker.StartPosition,
                supportedPreview.Walker.PredictedPosition, supportedPreview.Walker.PredictedPosition, false, false, false, true);
            var supportedPair = Unblocked(supportedPreview);
            Assert.Throws<InvalidOperationException>(() => supported.ValidateResolvedNext(Game(0), new RegularEnemyCollisionPairResolution(unsupported, supportedPair.Surveyor)));
        }

        // REQ-COM-004; partial AC-COM-003.
        [Test]
        public void DeathIsIdentityRepeatedInertAndResurrectionRequiresCollisionFreeReset()
        {
            var session = Session();
            Commit(session, Game(0));
            var deadInput = Game(1, walker: Walker(WalkerBehaviorPhase.Dead, RegularEnemyMovementIntent.None, 0, alive: false), walkerReaction: WalkerReaction(1, alive: false));
            var dead = Commit(session, deadInput);
            Assert.That(dead.Walker.Position.X, Is.EqualTo(204));
            Assert.That(dead.Walker.Grounded, Is.True);
            Assert.That(dead.Walker.Dead, Is.True);
            var repeated = Commit(session, Game(2, walker: Walker(WalkerBehaviorPhase.Dead, RegularEnemyMovementIntent.None, 0, alive: false), walkerReaction: WalkerReaction(2, alive: false)));
            Assert.That(repeated.Walker.Position.X, Is.EqualTo(dead.Walker.Position.X));
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Game(3)));

            var resetInput = Reset(3);
            var resetCandidate = session.PreflightReset(resetInput);
            Assert.That(session.LatestSnapshot.Value.Tick, Is.EqualTo(Tick(2)));
            var reset = session.CommitReset(resetInput, resetCandidate);
            AssertMotion(reset.Walker, 0, 0, 0, 0, 0);
            Assert.That(reset.Walker.Grounded, Is.True);
            AssertMotion(reset.Surveyor, 0, 40960, 0, 0, 0);
            Assert.That(reset.Surveyor.Grounded, Is.True);
            Assert.DoesNotThrow(() => session.PreviewNext(Game(4)));
        }

        // REQ-COM-004; partial AC-COM-003.
        [Test]
        public void FirstTickResetAndForgedResetCandidateAreAtomic()
        {
            var session = Session();
            Assert.Throws<InvalidOperationException>(() => session.PreflightReset(Reset(0)));
            var baseline = Commit(session, Game(0));
            var input = Reset(1);
            var candidate = session.PreflightReset(input);
            var forged = new RegularEnemyMotionPairSnapshot(candidate.Snapshot.Tick, candidate.Snapshot.Walker,
                new RegularEnemyMotionSnapshot("surveyor", new PositionQ4096(1, 1), candidate.Snapshot.Surveyor.Velocity, 0, 0, 0, true, false), candidate.Snapshot.NextExpectedTick);
            Assert.Throws<InvalidOperationException>(() => session.CommitReset(input, new RegularEnemyResetCandidate(forged)));
            AssertPairUnchanged(session, baseline);
        }

        // REQ-COM-004; partial AC-COM-003. Forged upstream reset fields and intents cannot clear locomotion state.
        [Test]
        public void EveryMalformedResetBehaviorFieldAndIntentPreservesWholePair()
        {
            var session = Session();
            var baseline = Commit(session, Game(0));
            var tick = 1;
            var walker = WalkerReset();
            var surveyor = SurveyorReset();
            var malformed = new[]
            {
                ResetBehavior(tick, new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Approach, -1, true,
                    RegularEnemyMovementIntent.None, 0, false, false, 0, 0), surveyor),
                ResetBehavior(tick, new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Reset, -1, true,
                    RegularEnemyMovementIntent.Horizontal, 1, false, false, 0, 0), surveyor),
                ResetBehavior(tick, WalkerReset(end: 0), surveyor),
                ResetBehavior(tick, WalkerReset(alive: false), surveyor),
                ResetBehavior(tick, WalkerReset(guard: true), surveyor),
                ResetBehavior(tick, WalkerReset(telegraph: true), surveyor),
                ResetBehavior(tick, WalkerReset(latched: 1), surveyor),
                ResetBehavior(tick, WalkerReset(ordinal: 1), surveyor),
                ResetBehavior(tick, walker, new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Relocate, -1, true,
                    RegularEnemyMovementIntent.None, 0, false, false, 0, 0)),
                ResetBehavior(tick, walker, new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Reset, -1, true,
                    RegularEnemyMovementIntent.Horizontal, 1, false, false, 0, 0)),
                ResetBehavior(tick, walker, SurveyorReset(end: 0)),
                ResetBehavior(tick, walker, SurveyorReset(alive: false)),
                ResetBehavior(tick, walker, SurveyorReset(telegraph: true)),
                ResetBehavior(tick, walker, SurveyorReset(hasLatch: true)),
                ResetBehavior(tick, walker, SurveyorReset(latchedX: 1)),
                ResetBehavior(tick, walker, SurveyorReset(ordinal: 1)),
                new RegularEnemyBehaviorPairSnapshot(Tick(tick), walker, surveyor,
                    new WalkerRecoveryIntent("walker", 0, Tick(tick + 1)), null, Tick(tick + 1)),
                new RegularEnemyBehaviorPairSnapshot(Tick(tick), walker, surveyor,
                    null, new SurveyorShotIntent("surveyor", 0, 0), Tick(tick + 1))
            };

            foreach (var behavior in malformed)
            {
                var input = ResetWithBehavior(tick, behavior);
                Assert.Throws<InvalidOperationException>(() => session.PreflightReset(input));
                AssertPairUnchanged(session, baseline);
            }
        }

        // REQ-COM-004; partial AC-COM-003.
        [Test]
        public void CheckedPositionOverflowRejectsBeforeMutationAndInactiveAxisDoesNotOverflow()
        {
            var overflow = new RegularEnemyLocomotionSession(Tick(0), new PositionQ4096(int.MaxValue, 0), true, new PositionQ4096(0, 0), true);
            Assert.Throws<OverflowException>(() => overflow.PreviewNext(Game(0)));
            Assert.That(overflow.LatestSnapshot.HasValue, Is.False);

            var inactive = new RegularEnemyLocomotionSession(Tick(0), new PositionQ4096(int.MaxValue, int.MaxValue), true, new PositionQ4096(int.MinValue, int.MinValue), true);
            var input = Game(0, walker: Walker(WalkerBehaviorPhase.Recovery, RegularEnemyMovementIntent.None, 0),
                surveyor: Surveyor(SurveyorBehaviorPhase.Cooldown, RegularEnemyMovementIntent.None, 0));
            Assert.DoesNotThrow(() => inactive.PreviewNext(input));
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003.
        [TestCase(30)]
        [TestCase(60)]
        [TestCase(144)]
        public void PairTraceIsIdenticalAcrossRenderGrouping(int renderRate)
        {
            Assert.That(Trace(renderRate), Is.EqualTo(Trace(60)));
        }

        private static RegularEnemyLocomotionSession Session(int firstTick = 0, bool walkerGrounded = true, bool surveyorGrounded = true) =>
            new RegularEnemyLocomotionSession(Tick(firstTick), new PositionQ4096(0, 0), walkerGrounded, new PositionQ4096(0, 40960), surveyorGrounded);
        private static SimulationTick Tick(int value) => new SimulationTick(value);

        private static RegularEnemyLocomotionPairInput Game(int tick, WalkerBehaviorSnapshot? walker = null, SurveyorBehaviorSnapshot? surveyor = null,
            RegularEnemyReactionSnapshot? walkerReaction = null, RegularEnemyReactionSnapshot? surveyorReaction = null)
        {
            var w = walker ?? Walker(WalkerBehaviorPhase.Approach, RegularEnemyMovementIntent.Horizontal, 1);
            var s = surveyor ?? Surveyor(SurveyorBehaviorPhase.Relocate, RegularEnemyMovementIntent.Horizontal, 1);
            var wr = walkerReaction ?? WalkerReaction(tick, w.IsAlive);
            var sr = surveyorReaction ?? SurveyorReaction(tick, s.IsAlive);
            return RegularEnemyLocomotionPairInput.Gameplay(Tick(tick), Behavior(tick, w, s), wr, sr);
        }

        private static RegularEnemyLocomotionPairInput Reset(int tick) => ResetWithBehavior(tick,
            ResetBehavior(tick, WalkerReset(), SurveyorReset()));
        private static RegularEnemyLocomotionPairInput ResetWithBehavior(int tick, RegularEnemyBehaviorPairSnapshot behavior) =>
            RegularEnemyLocomotionPairInput.EncounterReset(Tick(tick), behavior,
                WalkerReaction(tick), SurveyorReaction(tick, canFireOverride: false));
        private static RegularEnemyBehaviorPairSnapshot ResetBehavior(int tick, WalkerBehaviorSnapshot walker, SurveyorBehaviorSnapshot surveyor) =>
            new RegularEnemyBehaviorPairSnapshot(Tick(tick), walker, surveyor, null, null, Tick(checked(tick + 1)));

        private static RegularEnemyBehaviorPairSnapshot Behavior(int tick, WalkerBehaviorSnapshot? walker = null, SurveyorBehaviorSnapshot? surveyor = null) =>
            new RegularEnemyBehaviorPairSnapshot(Tick(tick), walker ?? Walker(WalkerBehaviorPhase.Approach, RegularEnemyMovementIntent.Horizontal, 1),
                surveyor ?? Surveyor(SurveyorBehaviorPhase.Relocate, RegularEnemyMovementIntent.Horizontal, 1), null, null, Tick(checked(tick + 1)));

        private static WalkerBehaviorSnapshot Walker(WalkerBehaviorPhase phase, RegularEnemyMovementIntent movement, int direction, bool alive = true, int latched = 0) =>
            new WalkerBehaviorSnapshot(phase, -1, alive, movement, direction, false, phase == WalkerBehaviorPhase.DashTelegraph, latched, 0);
        private static SurveyorBehaviorSnapshot Surveyor(SurveyorBehaviorPhase phase, RegularEnemyMovementIntent movement, int direction, bool alive = true) =>
            new SurveyorBehaviorSnapshot(phase, -1, alive, movement, direction, false, false, 0, 0);
        private static WalkerBehaviorSnapshot WalkerReset(int end = -1, bool alive = true, bool guard = false,
            bool telegraph = false, int latched = 0, long ordinal = 0) =>
            new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Reset, end, alive, RegularEnemyMovementIntent.None, 0,
                guard, telegraph, latched, ordinal);
        private static SurveyorBehaviorSnapshot SurveyorReset(int end = -1, bool alive = true, bool telegraph = false,
            bool hasLatch = false, int latchedX = 0, long ordinal = 0) =>
            new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Reset, end, alive, RegularEnemyMovementIntent.None, 0,
                telegraph, hasLatch, latchedX, ordinal);

        private static RegularEnemyReactionSnapshot WalkerReaction(int tick, bool alive = true, bool heavy = false, int revision = -1) =>
            new RegularEnemyReactionSnapshot(Tick(tick), "walker", alive, heavy, revision, -1, -1, false, alive && heavy ? 1500 : 1000, false, false, Tick(checked(tick + 1)));
        private static RegularEnemyReactionSnapshot SurveyorReaction(int tick, bool alive = true, bool heavy = false, int revision = -1, int fireLockEnd = -1, bool? canFireOverride = null) =>
            new RegularEnemyReactionSnapshot(Tick(tick), "surveyor", alive, heavy, revision, -1, fireLockEnd, false, 1000,
                alive && heavy, canFireOverride ?? (alive && !heavy && !(fireLockEnd >= 0 && tick < fireLockEnd)), Tick(checked(tick + 1)));

        private static RegularEnemyMotionPairSnapshot Commit(RegularEnemyLocomotionSession session, RegularEnemyLocomotionPairInput input)
        {
            var preview = session.PreviewNext(input);
            var resolution = Unblocked(preview);
            var candidate = session.ValidateResolvedNext(input, resolution);
            return session.CommitResolvedNext(input, resolution, candidate);
        }

        private static RegularEnemyCollisionPairResolution Unblocked(RegularEnemyMotionPairPrediction prediction) =>
            new RegularEnemyCollisionPairResolution(Unblocked(prediction.Walker), Unblocked(prediction.Surveyor));
        private static RegularEnemyCollisionResolution Unblocked(RegularEnemyMotionPrediction p)
        {
            if (!p.Alive) return new RegularEnemyCollisionResolution(p.Tick, p.RoleId, p.StartPosition, p.PredictedPosition, p.PredictedPosition, false, false, false, p.PriorGrounded);
            var support = p.PriorGrounded && p.PredictedPosition.Y == p.StartPosition.Y;
            return new RegularEnemyCollisionResolution(p.Tick, p.RoleId, p.StartPosition, p.PredictedPosition, p.PredictedPosition, false, false, support, support);
        }

        private static void AssertMotion(RegularEnemyMotionSnapshot value, int x, int y, int velocityX, long xRemainder, long yRemainder)
        {
            Assert.That(value.Position.X, Is.EqualTo(x)); Assert.That(value.Position.Y, Is.EqualTo(y));
            Assert.That(value.Velocity.X, Is.EqualTo(velocityX)); Assert.That(value.XPositionRemainder, Is.EqualTo(xRemainder));
            Assert.That(value.YPositionRemainder, Is.EqualTo(yRemainder));
        }
        private static void AssertPairUnchanged(RegularEnemyLocomotionSession session, RegularEnemyMotionPairSnapshot expected)
        {
            Assert.That(session.LatestSnapshot.HasValue, Is.True);
            var actual = session.LatestSnapshot.Value;
            Assert.That(actual.Tick, Is.EqualTo(expected.Tick));
            Assert.That(actual.NextExpectedTick, Is.EqualTo(expected.NextExpectedTick));
            AssertMotionSnapshotEqual(actual.Walker, expected.Walker);
            AssertMotionSnapshotEqual(actual.Surveyor, expected.Surveyor);
            Assert.That(session.NextExpectedTick, Is.EqualTo(expected.NextExpectedTick));
        }
        private static void AssertMotionSnapshotEqual(RegularEnemyMotionSnapshot actual, RegularEnemyMotionSnapshot expected)
        {
            Assert.That(actual.RoleId, Is.EqualTo(expected.RoleId));
            Assert.That(actual.Position.X, Is.EqualTo(expected.Position.X));
            Assert.That(actual.Position.Y, Is.EqualTo(expected.Position.Y));
            Assert.That(actual.Velocity.X, Is.EqualTo(expected.Velocity.X));
            Assert.That(actual.Velocity.Y, Is.EqualTo(expected.Velocity.Y));
            Assert.That(actual.XPositionRemainder, Is.EqualTo(expected.XPositionRemainder));
            Assert.That(actual.YPositionRemainder, Is.EqualTo(expected.YPositionRemainder));
            Assert.That(actual.GravityRemainder, Is.EqualTo(expected.GravityRemainder));
            Assert.That(actual.Grounded, Is.EqualTo(expected.Grounded));
            Assert.That(actual.Dead, Is.EqualTo(expected.Dead));
        }
        private static void AssertPrediction(RegularEnemyMotionPairPrediction a, RegularEnemyMotionPairPrediction b)
        {
            Assert.That(a.Tick, Is.EqualTo(b.Tick)); Assert.That(a.Walker.PredictedPosition.X, Is.EqualTo(b.Walker.PredictedPosition.X));
            Assert.That(a.Surveyor.PredictedPosition.X, Is.EqualTo(b.Surveyor.PredictedPosition.X));
        }
        private static string Trace(int renderRate)
        {
            var session = Session(); var trace = new List<string>(); var tick = 0; var accumulator = 0;
            while (tick < 60)
            {
                accumulator = checked(accumulator + 60);
                while (accumulator >= renderRate && tick < 60)
                {
                    accumulator -= renderRate; var snapshot = Commit(session, Game(tick));
                    trace.Add(string.Join("/", snapshot.Tick.Value, snapshot.Walker.Position.X, snapshot.Walker.XPositionRemainder,
                        snapshot.Surveyor.Position.X, snapshot.Surveyor.XPositionRemainder, snapshot.NextExpectedTick.Value)); tick++;
                }
            }
            return string.Join("|", trace);
        }
    }
}
