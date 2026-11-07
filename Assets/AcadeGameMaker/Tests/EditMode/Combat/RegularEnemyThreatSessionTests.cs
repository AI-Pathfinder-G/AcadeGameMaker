using System;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Core;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Combat
{
    public sealed class RegularEnemyThreatSessionTests
    {
        // REQ-COM-001, REQ-COM-004; partial AC-COM-003.
        [Test]
        public void EnemyRangedExtendsDamageAbiWithoutChangingExistingValues()
        {
            Assert.That((int)DamageKind.BasicAttack, Is.EqualTo(0));
            Assert.That((int)DamageKind.BossPayloadTransfer, Is.EqualTo(5));
            Assert.That((int)DamageKind.EnemyRanged, Is.EqualTo(6));
            var request = new DamageRequest("line", "surveyor", "player", 1, DamageKind.EnemyRanged, Tick(1));
            Assert.That(request.Kind, Is.EqualTo(DamageKind.EnemyRanged));
            var combat = new CombatSession(Tick(1));
            combat.Register(new CombatantRegistration("player", 5, 0));
            var result = combat.Process(Tick(1), new[] { request })[0];
            Assert.That(result.Result, Is.EqualTo(DamageResultCode.Applied));
            Assert.That(result.RequestId, Is.EqualTo("line"));
        }

        [Test]
        public void EnemyRangedParticipatesInMixedCanonicalM1SortingAndResultsEchoEveryField()
        {
            var session = new CombatSession(Tick(7));
            session.Register(new CombatantRegistration("player", 10, 0));
            var submitted = new[]
            {
                new DamageRequest("z-contact", "walker", "player", 3, DamageKind.EnemyContact, Tick(7)),
                new DamageRequest("a-ranged", "surveyor", "player", 2, DamageKind.EnemyRanged, Tick(7)),
                new DamageRequest("m-boss", "ordan", "player", 1, DamageKind.BossPattern, Tick(7))
            };
            var results = session.Process(Tick(7), submitted);
            Assert.That(results.Count, Is.EqualTo(3));
            AssertResult(results[0], "a-ranged", "player", 2, DamageResultCode.Applied, 7);
            AssertResult(results[1], "m-boss", "player", 1, DamageResultCode.Applied, 7);
            AssertResult(results[2], "z-contact", "player", 3, DamageResultCode.Applied, 7);
            Assert.That(submitted[1].Kind, Is.EqualTo(DamageKind.EnemyRanged));
        }

        // REQ-COM-001, REQ-COM-002; partial AC-COM-001, AC-COM-003.
        [Test]
        public void FirstTickUsesStaticOverlapAndPublishesExactWalkerRequest()
        {
            var session = Session();
            var miss = session.PreviewNext(Game(0, new PositionQ4096(-1000, 0), new PositionQ4096(1000, 0)));
            Assert.That(miss.RequestCount, Is.Zero);
            session.CommitNext(Game(0, new PositionQ4096(-1000, 0), new PositionQ4096(1000, 0)), miss);
            var crossing = session.PreviewNext(Game(1, new PositionQ4096(1000, 0), new PositionQ4096(-1000, 0)));
            Assert.That(crossing.RequestCount, Is.EqualTo(1));
            AssertRequest(crossing.GetRequest(0), "enemy.contact/walker/0000000001", "walker", DamageKind.EnemyContact, 2);
        }

        [Test]
        public void ClosedSweptAabbCountsBoundaryAndRejectsOneQ4096Miss()
        {
            Assert.That(Session().PreviewNext(Game(0, new PositionQ4096(0, 0), new PositionQ4096(200, 0))).RequestCount, Is.EqualTo(1));
            Assert.That(Session().PreviewNext(Game(0, new PositionQ4096(0, 0), new PositionQ4096(201, 0))).RequestCount, Is.Zero);
            Assert.That(Session().PreviewNext(Game(0, new PositionQ4096(0, 0), new PositionQ4096(0, 201))).RequestCount, Is.Zero);
        }

        [Test]
        public void PlayerWalkerAndOpposedMotionAllUseRelativeSweeps()
        {
            AssertCrosses(new PositionQ4096(-500, 0), new PositionQ4096(500, 0), new PositionQ4096(0, 0), new PositionQ4096(0, 0));
            AssertCrosses(new PositionQ4096(0, 0), new PositionQ4096(0, 0), new PositionQ4096(-500, 0), new PositionQ4096(500, 0));
            AssertCrosses(new PositionQ4096(-500, 0), new PositionQ4096(500, 0), new PositionQ4096(500, 0), new PositionQ4096(-500, 0));
        }

        [Test]
        public void DiagonalNearMissAndExtremeCoordinatesUseExactRationals()
        {
            var diagonal = Session();
            Commit(diagonal, Game(0, new PositionQ4096(-400, 401), new PositionQ4096(0, 0)));
            Assert.That(diagonal.PreviewNext(Game(1, new PositionQ4096(400, 401), new PositionQ4096(0, 0))).RequestCount, Is.Zero);

            var extreme = Session();
            Commit(extreme, Game(0, new PositionQ4096(int.MinValue, 0), new PositionQ4096(int.MaxValue, 0)));
            Assert.That(extreme.PreviewNext(Game(1, new PositionQ4096(int.MaxValue, 0), new PositionQ4096(int.MinValue, 0))).RequestCount, Is.EqualTo(1));
        }

        [Test]
        public void EveryAliveWalkerPhaseThreatensButDeadSuppressesAndLatches()
        {
            foreach (WalkerBehaviorPhase phase in new[] { WalkerBehaviorPhase.Approach, WalkerBehaviorPhase.DashTelegraph, WalkerBehaviorPhase.DashActive, WalkerBehaviorPhase.Recovery })
                Assert.That(Session().PreviewNext(Game(0, new PositionQ4096(0, 0), new PositionQ4096(0, 0), walkerPhase: phase)).RequestCount, Is.EqualTo(1), phase.ToString());
            var dead = Session();
            Commit(dead, Game(0, new PositionQ4096(0, 0), new PositionQ4096(0, 0), walkerAlive: false));
            Assert.That(dead.LatestSnapshot.Value.WalkerDead, Is.True);
            Assert.Throws<InvalidOperationException>(() => dead.PreviewNext(Game(1, new PositionQ4096(0, 0), new PositionQ4096(0, 0))));
        }

        // REQ-COM-001, REQ-COM-002; partial AC-COM-001, AC-COM-003. Approved M3B2A may
        // publish direction zero at aligned X; M3D1 accepts that exact telegraph->active trace,
        // while contact remains solely the existing static/swept geometry result.
        [Test]
        public void EqualXBehaviorTelegraphToActiveAcceptsZeroDirectionAndKeepsGeometryAuthoritative()
        {
            var behavior = new RegularEnemyBehaviorSession(Tick(0));
            var overlap = Session();
            var separated = Session();
            for (var tick = 0; tick <= 18; tick++)
            {
                var publication = behavior.Process(EqualXBehaviorInput(tick));
                if (tick == 0)
                {
                    Assert.That(publication.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.DashTelegraph));
                    Assert.That(publication.Walker.LatchedDashDirection, Is.Zero);
                }
                if (tick == 18)
                {
                    Assert.That(publication.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.DashActive));
                    Assert.That(publication.Walker.Direction, Is.Zero);
                    Assert.That(publication.Walker.LatchedDashDirection, Is.Zero);
                }

                var touchingInput = FromBehavior(publication, default, default);
                var touching = overlap.PreviewNext(touchingInput);
                Assert.That(touching.RequestCount, Is.EqualTo(1), "overlap tick " + tick);
                overlap.CommitNext(touchingInput, touching);

                var separatedInput = FromBehavior(publication, new PositionQ4096(0, 1000), default);
                var miss = separated.PreviewNext(separatedInput);
                Assert.That(miss.RequestCount, Is.Zero, "separated tick " + tick);
                separated.CommitNext(separatedInput, miss);
            }
        }

        // REQ-COM-004; partial AC-COM-003. The compatibility widening is exactly -1/0/1;
        // both adjacent out-of-domain values reject without mutating a populated session.
        [TestCase(-2)]
        [TestCase(2)]
        public void EqualXCompatibilityStillRejectsOutOfDomainDirectionsAtomically(int invalidDirection)
        {
            foreach (var phase in new[] { WalkerBehaviorPhase.DashTelegraph, WalkerBehaviorPhase.DashActive })
            {
                var session = Session();
                Commit(session, Game(0, new PositionQ4096(0, 1000), default));
                var input = Game(1, new PositionQ4096(0, 1000), default, walkerPhase: phase);
                var original = input.Behavior.Walker;
                var malformed = new WalkerBehaviorSnapshot(original.Phase, original.ExclusiveEndTick, original.IsAlive,
                    original.Movement, phase == WalkerBehaviorPhase.DashActive ? invalidDirection : 0,
                    original.GuardRaised, original.Telegraph, invalidDirection, original.AttackCycleOrdinal);
                AssertAtomicReject(session, WithWalker(input, malformed));
            }
        }

        [Test]
        public void SustainedWalkerContactEmitsOneDistinctRequestPerSourceTick()
        {
            var session = Session();
            for (var tick = 0; tick < 4; tick++)
            {
                var input = Game(tick, default, default);
                var candidate = session.PreviewNext(input);
                Assert.That(candidate.RequestCount, Is.EqualTo(1));
                Assert.That(candidate.GetRequest(0).RequestId, Is.EqualTo("enemy.contact/walker/" + tick.ToString("D10")));
                Assert.That(candidate.GetRequest(0).Tick.Value, Is.EqualTo(tick + 1));
                session.CommitNext(input, candidate);
            }
        }

        [Test]
        public void LineSpawnsBeforeTouchAndPublishesAfterWalkerInFixedOrder()
        {
            var candidate = Session().PreviewNext(Game(0, new PositionQ4096(0, 0), new PositionQ4096(0, 0), fire: true));
            Assert.That(candidate.RequestCount, Is.EqualTo(2));
            AssertRequest(candidate.GetRequest(0), "enemy.contact/walker/0000000000", "walker", DamageKind.EnemyContact, 1);
            AssertRequest(candidate.GetRequest(1), "enemy.line/surveyor/0000000000000000000/0000000000", "surveyor", DamageKind.EnemyRanged, 1);
            Assert.That(candidate.Snapshot.LineExclusiveEndTick, Is.EqualTo(30));
        }

        [Test]
        public void LineHitsThroughTick29AndExpiresBeforeTick30Touch()
        {
            var session = Session();
            Commit(session, Game(0, new PositionQ4096(0, 1000), new PositionQ4096(5000, 0), fire: true));
            for (var tick = 1; tick <= 30; tick++)
            {
                var candidate = session.PreviewNext(Game(tick, new PositionQ4096(0, 1000), new PositionQ4096(5000, 0), ordinal: 1));
                Assert.That(candidate.RequestCount, Is.EqualTo(tick < 30 ? 1 : 0), "tick " + tick);
                Commit(session, Game(tick, new PositionQ4096(0, 1000), new PositionQ4096(5000, 0), ordinal: 1), candidate);
            }
            Assert.That(session.LatestSnapshot.Value.HasActiveLine, Is.False);
        }

        [Test]
        public void LineUsesClosedSweptHorizontalIntervalAndOneUnitMiss()
        {
            var boundary = Session();
            Commit(boundary, Game(0, new PositionQ4096(1224, 1000), new PositionQ4096(5000, 0), fire: true));
            Assert.That(boundary.LatestSnapshot.Value.HasActiveLine, Is.True);
            var miss = Session();
            Assert.That(miss.PreviewNext(Game(0, new PositionQ4096(1225, 1000), new PositionQ4096(5000, 0), fire: true)).RequestCount, Is.Zero);

            var crossing = Session();
            Commit(crossing, Game(0, new PositionQ4096(-2000, 1000), new PositionQ4096(5000, 0), fire: true));
            Assert.That(crossing.PreviewNext(Game(1, new PositionQ4096(2000, 1000), new PositionQ4096(5000, 0), ordinal: 1)).RequestCount, Is.EqualTo(1));
        }

        [Test]
        public void ShotPublicationMutationsAndActiveOverlapRejectAtomically()
        {
            var session = Session();
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Game(0, new PositionQ4096(0, 0), new PositionQ4096(5000, 0), fire: true, intentOrdinal: 1)));
            Assert.That(session.NextExpectedTick.Value, Is.Zero);
            Commit(session, Game(0, new PositionQ4096(0, 0), new PositionQ4096(5000, 0), fire: true));
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Game(1, new PositionQ4096(0, 0), new PositionQ4096(5000, 0), fire: true, ordinal: 2, intentOrdinal: 1)));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(1));
            Assert.That(session.LatestSnapshot.Value.ExpectedShotOrdinal, Is.EqualTo(1));
        }

        [Test]
        public void CompleteShotMutationMatrixRejectsWithFullStatePreservation()
        {
            var initial = Session();
            var nonFire = Game(0, default, new PositionQ4096(5000, 0));
            AssertAtomicReject(initial, WithSurveyor(nonFire, nonFire.Behavior.Surveyor, new SurveyorShotIntent("surveyor", 0, 0)));

            var fire = Game(0, default, new PositionQ4096(5000, 0), fire: true);
            AssertAtomicReject(initial, WithSurveyor(fire, fire.Behavior.Surveyor, null));
            AssertAtomicReject(initial, WithSurveyor(fire, fire.Behavior.Surveyor, new SurveyorShotIntent("surveyor", 0, 1)));

            var active = Session();
            Commit(active, fire);
            var overlap = Game(1, default, new PositionQ4096(5000, 0), fire: true, ordinal: 1);
            AssertAtomicReject(active, overlap);

            var expiredAtCandidateBoundary = Session();
            Commit(expiredAtCandidateBoundary, fire);
            for (var tick = 1; tick < 30; tick++)
                Commit(expiredAtCandidateBoundary, Game(tick, new PositionQ4096(5000, 0), new PositionQ4096(5000, 0), ordinal: 1));

            var staleAndDuplicate = Game(30, default, new PositionQ4096(5000, 0), fire: true, ordinal: 0, intentOrdinal: 0);
            AssertAtomicReject(expiredAtCandidateBoundary, staleAndDuplicate);
            var skipped = Game(30, default, new PositionQ4096(5000, 0), fire: true, ordinal: 2, intentOrdinal: 2);
            AssertAtomicReject(expiredAtCandidateBoundary, skipped);
            var validShape = Game(30, default, new PositionQ4096(5000, 0), fire: true, ordinal: 1);
            var maxSurveyor = new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Fire, 31, true,
                RegularEnemyMovementIntent.None, 0, false, true, 0, long.MaxValue);
            AssertAtomicReject(expiredAtCandidateBoundary,
                WithSurveyor(validShape, maxSurveyor, new SurveyorShotIntent("surveyor", long.MaxValue, 0)));
        }

        [Test]
        public void ShotTargetUsesExactAwayFromZeroQ1000ConversionAndOverflowRejects()
        {
            var positive = Session().PreviewNext(Game(0, new PositionQ4096(5000, 0), new PositionQ4096(9000, 0), fire: true, targetXQ1000: 250));
            Assert.That(positive.Snapshot.LineCenterXQ4096, Is.EqualTo(1024));
            var negative = Session().PreviewNext(Game(0, new PositionQ4096(-5000, 0), new PositionQ4096(9000, 0), fire: true, targetXQ1000: -250));
            Assert.That(negative.Snapshot.LineCenterXQ4096, Is.EqualTo(-1024));
            var overflow = Session();
            Assert.Throws<OverflowException>(() => overflow.PreviewNext(Game(0, default, new PositionQ4096(9000, 0), fire: true, targetXQ1000: int.MaxValue)));
            Assert.That(overflow.NextExpectedTick.Value, Is.Zero);
        }

        [Test]
        public void SurveyorDeathRetainsExistingLineWhilePlayerDeathSuppressesAllRequests()
        {
            var session = Session();
            Commit(session, Game(0, new PositionQ4096(0, 1000), new PositionQ4096(5000, 0), fire: true));
            var afterShooterDeath = session.PreviewNext(Game(1, new PositionQ4096(0, 1000), new PositionQ4096(5000, 0), surveyorAlive: false, ordinal: 1));
            Assert.That(afterShooterDeath.RequestCount, Is.EqualTo(1));
            Commit(session, Game(1, new PositionQ4096(0, 1000), new PositionQ4096(5000, 0), surveyorAlive: false, ordinal: 1), afterShooterDeath);
            var playerDead = session.PreviewNext(Game(2, new PositionQ4096(0, 1000), new PositionQ4096(5000, 0), playerAlive: false, surveyorAlive: false, ordinal: 1));
            Assert.That(playerDead.RequestCount, Is.Zero);
        }

        [Test]
        public void ExistingLineContinuesAfterSurveyorDeathThroughTick29()
        {
            var session = Session();
            Commit(session, Game(0, new PositionQ4096(0, 1000), new PositionQ4096(5000, 0), fire: true));
            for (var tick = 1; tick <= 30; tick++)
            {
                var input = Game(tick, new PositionQ4096(0, 1000), new PositionQ4096(5000, 0), surveyorAlive: false, ordinal: 1);
                var candidate = session.PreviewNext(input);
                Assert.That(candidate.RequestCount, Is.EqualTo(tick < 30 ? 1 : 0), "dead shooter tick " + tick);
                session.CommitNext(input, candidate);
            }
        }

        [Test]
        public void ExistingLinePersistsAcrossBothSuppressedShapesWithoutNewShot()
        {
            var session = Session();
            Commit(session, Game(0, default, new PositionQ4096(5000, 0), fire: true));
            var baseline = Game(1, default, new PositionQ4096(5000, 0), ordinal: 1);
            var downward = new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Suppressed, -1, true,
                RegularEnemyMovementIntent.Downward, 0, false, false, 0, 1);
            var downInput = WithSurveyor(baseline, downward, null);
            var downCandidate = session.PreviewNext(downInput);
            Assert.That(downCandidate.RequestCount, Is.EqualTo(1));
            Assert.That(downCandidate.Snapshot.HasActiveLine, Is.True);
            Assert.That(downCandidate.Snapshot.ExpectedShotOrdinal, Is.EqualTo(1));
            session.CommitNext(downInput, downCandidate);

            baseline = Game(2, default, new PositionQ4096(5000, 0), ordinal: 1);
            var stationary = new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Suppressed, -1, true,
                RegularEnemyMovementIntent.None, 0, false, false, 0, 1);
            var stillInput = WithSurveyor(baseline, stationary, null);
            var stillCandidate = session.PreviewNext(stillInput);
            Assert.That(stillCandidate.RequestCount, Is.EqualTo(1));
            Assert.That(stillCandidate.Snapshot.HasActiveLine, Is.True);
            Assert.That(stillCandidate.Snapshot.ExpectedShotOrdinal, Is.EqualTo(1));
        }

        [Test]
        public void RepeatedDeathsAdvanceButEachIdentityRejectsResurrectionUntilReset()
        {
            var player = Session();
            Commit(player, Game(0, default, new PositionQ4096(5000, 0), playerAlive: false));
            Commit(player, Game(1, default, new PositionQ4096(5000, 0), playerAlive: false));
            AssertAtomicReject(player, Game(2, default, new PositionQ4096(5000, 0), playerAlive: true));

            var walker = Session();
            Commit(walker, Game(0, default, default, walkerAlive: false));
            Commit(walker, Game(1, default, default, walkerAlive: false));
            AssertAtomicReject(walker, Game(2, default, default, walkerAlive: true));

            var surveyor = Session();
            Commit(surveyor, Game(0, default, new PositionQ4096(5000, 0), surveyorAlive: false));
            Commit(surveyor, Game(1, default, new PositionQ4096(5000, 0), surveyorAlive: false));
            AssertAtomicReject(surveyor, Game(2, default, new PositionQ4096(5000, 0), surveyorAlive: true));
        }

        [Test]
        public void ResetClearsLineLatchesOrdinalAndSweepHistory()
        {
            var session = Session();
            Commit(session, Game(0, new PositionQ4096(-1000, 0), new PositionQ4096(1000, 0), playerAlive: false, walkerAlive: false, surveyorAlive: false));
            var reset = Reset(1);
            var resetCandidate = session.PreviewNext(reset);
            Assert.That(resetCandidate.RequestCount, Is.Zero);
            session.CommitNext(reset, resetCandidate);
            Assert.That(resetCandidate.Snapshot.HasPriorCenters, Is.False);
            Assert.That(resetCandidate.Snapshot.ExpectedShotOrdinal, Is.Zero);
            Assert.That(resetCandidate.Snapshot.HasActiveLine, Is.False);
            var reseed = session.PreviewNext(Game(2, new PositionQ4096(1000, 0), new PositionQ4096(-1000, 0)));
            Assert.That(reseed.RequestCount, Is.Zero);
            session.CommitNext(Game(2, new PositionQ4096(1000, 0), new PositionQ4096(-1000, 0)), reseed);
            var postResetShot = session.PreviewNext(Game(3, new PositionQ4096(0, 1000), new PositionQ4096(5000, 0), fire: true));
            Assert.That(postResetShot.Snapshot.ExpectedShotOrdinal, Is.EqualTo(1));
            Assert.That(postResetShot.GetRequest(0).RequestId, Does.StartWith("enemy.line/surveyor/0000000000000000000/"));
        }

        [Test]
        public void ResetFromActiveLineNonzeroOrdinalAndDeathsClearsEveryEncounterState()
        {
            var session = Session();
            Commit(session, Game(0, default, new PositionQ4096(5000, 0), fire: true));
            Commit(session, Game(1, default, new PositionQ4096(5000, 0), playerAlive: false, walkerAlive: false, surveyorAlive: false, ordinal: 1));
            var before = session.LatestSnapshot.Value;
            Assert.That(before.HasActiveLine, Is.True);
            Assert.That(before.ExpectedShotOrdinal, Is.EqualTo(1));
            Assert.That(before.PlayerDead && before.WalkerDead && before.SurveyorDead, Is.True);
            var reset = Reset(2);
            var candidate = session.PreviewNext(reset);
            session.CommitNext(reset, candidate);
            var after = session.LatestSnapshot.Value;
            Assert.That(after.PlayerDead || after.WalkerDead || after.SurveyorDead || after.HasActiveLine || after.HasPriorCenters, Is.False);
            Assert.That(after.ExpectedShotOrdinal, Is.Zero);
            Assert.That(after.LineSpawnTick, Is.EqualTo(-1));
            Assert.That(after.LineExclusiveEndTick, Is.EqualTo(-1));
        }

        [Test]
        public void PreviewAndForgedCandidateAreMutationFreeAndCommitRecomputesEveryField()
        {
            var session = Session();
            var input = Game(0, new PositionQ4096(0, 0), new PositionQ4096(0, 0));
            var preview = session.PreviewNext(input);
            Assert.That(session.NextExpectedTick.Value, Is.Zero);
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
            var s = preview.Snapshot;
            var forgedSnapshot = new RegularEnemyThreatSnapshot(s.Tick, Tick(9), s.PlayerDead, s.WalkerDead, s.SurveyorDead,
                s.HasPriorCenters, s.PriorPlayerCenter, s.PriorWalkerCenter, s.ExpectedShotOrdinal, s.HasActiveLine,
                s.LineCenterXQ4096, s.LineShotOrdinal, s.LineSpawnTick, s.LineExclusiveEndTick);
            Assert.Throws<InvalidOperationException>(() => session.CommitNext(input, new RegularEnemyThreatCandidate(forgedSnapshot, preview.CopyRequests())));
            Assert.That(session.NextExpectedTick.Value, Is.Zero);
        }

        [Test]
        public void RequestCopiesAndMalformedSecondRoleCannotMutateOrPartiallyCommit()
        {
            var session = Session();
            var input = Game(0, default, default);
            var candidate = session.PreviewNext(input);
            var escaped = candidate.CopyRequests();
            escaped[0] = new DamageRequest("forged", "walker", "player", 1, DamageKind.EnemyContact, Tick(1));
            Assert.That(candidate.GetRequest(0).RequestId, Is.EqualTo("enemy.contact/walker/0000000000"));
            var badMotion = new RegularEnemyMotionPairSnapshot(input.Motion.Tick, input.Motion.Walker,
                Motion("wrong", input.Motion.Surveyor.Position, false), input.Motion.NextExpectedTick);
            var malformed = RegularEnemyThreatInput.Gameplay(input.Tick, input.PlayerFinalAlive, input.PlayerCenter, input.Behavior, badMotion);
            Assert.Throws<ArgumentException>(() => session.PreviewNext(malformed));
            Assert.That(session.NextExpectedTick.Value, Is.Zero);
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
        }

        [Test]
        public void ForgedCandidateRequestContentRejectsWithoutAnyMutation()
        {
            var session = Session();
            var input = Game(0, default, default);
            var preview = session.PreviewNext(input);
            var forged = new[] { new DamageRequest("forged", "walker", "player", 1, DamageKind.EnemyContact, Tick(1)) };
            Assert.Throws<InvalidOperationException>(() => session.CommitNext(input, new RegularEnemyThreatCandidate(preview.Snapshot, forged)));
            Assert.That(session.NextExpectedTick.Value, Is.Zero);
            Assert.That(session.LatestSnapshot.HasValue, Is.False);
        }

        [Test]
        public void LogicalTraceIsIndependentOfRenderGrouping()
        {
            Assert.That(RunTrace(30), Is.EqualTo(RunTrace(60)));
            Assert.That(RunTrace(60), Is.EqualTo(RunTrace(144)));
        }

        [Test]
        public void TickAndDeadlineHorizonsRejectWithoutMutation()
        {
            Assert.DoesNotThrow(() => new RegularEnemyThreatSession(Tick(int.MaxValue - 2), 1, 1, 1, 1, 1024));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RegularEnemyThreatSession(Tick(int.MaxValue - 1), 1, 1, 1, 1, 1024));
            var session = new RegularEnemyThreatSession(Tick(int.MaxValue - 2), 100, 100, 100, 100, 1024);
            Assert.Throws<OverflowException>(() => session.PreviewNext(Game(int.MaxValue - 2, new PositionQ4096(0, 0), new PositionQ4096(5000, 0), fire: true)));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(int.MaxValue - 2));
        }

        [Test]
        public void MaximumValidSourceTickPublishesAtIntMaxValueMinusOne()
        {
            var source = int.MaxValue - 2;
            var next = Tick(int.MaxValue - 1);
            var behavior = new RegularEnemyBehaviorPairSnapshot(Tick(source),
                new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Approach, -1, true, RegularEnemyMovementIntent.Horizontal, 0, false, false, 0, 0),
                new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Suppressed, -1, true, RegularEnemyMovementIntent.None, 0, false, false, 0, 0), null, null, next);
            var motion = new RegularEnemyMotionPairSnapshot(Tick(source), Motion("walker", default, false),
                Motion("surveyor", new PositionQ4096(5000, 0), false), next);
            var input = RegularEnemyThreatInput.Gameplay(Tick(source), true, default, behavior, motion);
            var session = new RegularEnemyThreatSession(Tick(source), 100, 100, 100, 100, 1024);
            var candidate = session.PreviewNext(input);
            Assert.That(candidate.GetRequest(0).Tick.Value, Is.EqualTo(int.MaxValue - 1));
            session.CommitNext(input, candidate);
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(int.MaxValue - 1));
        }

        [Test]
        public void SourceIntMaxValueMinusOneRejectsAtomicallyAtExhaustedSentinel()
        {
            var source = int.MaxValue - 2;
            var next = Tick(int.MaxValue - 1);
            var behavior = new RegularEnemyBehaviorPairSnapshot(Tick(source),
                new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Approach, -1, true, RegularEnemyMovementIntent.Horizontal, 0, false, false, 0, 0),
                new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Suppressed, -1, true, RegularEnemyMovementIntent.None, 0, false, false, 0, 0), null, null, next);
            var motion = new RegularEnemyMotionPairSnapshot(Tick(source), Motion("walker", new PositionQ4096(5000, 0), false),
                Motion("surveyor", new PositionQ4096(5000, 0), false), next);
            var first = RegularEnemyThreatInput.Gameplay(Tick(source), true, default, behavior, motion);
            var session = new RegularEnemyThreatSession(Tick(source), 100, 100, 100, 100, 1024);
            Commit(session, first);
            var state = State(session);

            var exhaustedTick = int.MaxValue - 1;
            var exhaustedBehavior = new RegularEnemyBehaviorPairSnapshot(Tick(exhaustedTick),
                new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Approach, -1, true, RegularEnemyMovementIntent.Horizontal, 0, false, false, 0, 0),
                new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Suppressed, -1, true, RegularEnemyMovementIntent.None, 0, false, false, 0, 0),
                null, null, Tick(int.MaxValue));
            var exhaustedMotion = new RegularEnemyMotionPairSnapshot(Tick(exhaustedTick), Motion("walker", default, false),
                Motion("surveyor", default, false), Tick(int.MaxValue));
            var exhausted = RegularEnemyThreatInput.Gameplay(Tick(exhaustedTick), true, default, exhaustedBehavior, exhaustedMotion);
            Assert.Throws<ArgumentOutOfRangeException>(() => session.PreviewNext(exhausted));
            Assert.That(State(session), Is.EqualTo(state));
        }

        [Test]
        public void ConstructorRejectsInvalidExtentsAndDenialWidth()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RegularEnemyThreatSession(Tick(0), 0, 1, 1, 1, 1024));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RegularEnemyThreatSession(Tick(0), 1, 1, 1, 1, 1023));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RegularEnemyThreatSession(Tick(-1), 1, 1, 1, 1, 1024));
        }

        private static void AssertCrosses(PositionQ4096 player0, PositionQ4096 player1, PositionQ4096 walker0, PositionQ4096 walker1)
        {
            var session = Session(); Commit(session, Game(0, player0, walker0));
            Assert.That(session.PreviewNext(Game(1, player1, walker1)).RequestCount, Is.EqualTo(1));
        }

        private static string RunTrace(int renderRate)
        {
            var session = Session();
            var accumulator = 0;
            var trace = string.Empty;
            var tick = 0;
            while (tick < 45)
            {
                accumulator += 60;
                while (accumulator >= renderRate && tick < 45)
                {
                    var input = Game(tick, new PositionQ4096(tick * 20 - 400, 1000), new PositionQ4096(0, 0),
                        fire: tick == 0, ordinal: tick == 0 ? 0 : 1);
                    var candidate = session.PreviewNext(input);
                    trace += tick + ":" + candidate.RequestCount + ":" + candidate.Snapshot.ExpectedShotOrdinal + ":" + candidate.Snapshot.HasActiveLine + ";";
                    session.CommitNext(input, candidate);
                    tick++;
                    accumulator -= renderRate;
                }
            }
            return trace;
        }

        private static void AssertRequest(DamageRequest value, string id, string source, DamageKind kind, int tick)
        {
            Assert.That(value.RequestId, Is.EqualTo(id)); Assert.That(value.SourceId, Is.EqualTo(source));
            Assert.That(value.TargetId, Is.EqualTo("player")); Assert.That(value.Amount, Is.EqualTo(1));
            Assert.That(value.Kind, Is.EqualTo(kind)); Assert.That(value.Tick.Value, Is.EqualTo(tick));
        }

        private static void AssertResult(DamageResult value, string requestId, string targetId, int appliedAmount,
            DamageResultCode result, int tick)
        {
            Assert.That(value.RequestId, Is.EqualTo(requestId)); Assert.That(value.TargetId, Is.EqualTo(targetId));
            Assert.That(value.AppliedAmount, Is.EqualTo(appliedAmount)); Assert.That(value.Result, Is.EqualTo(result));
            Assert.That(value.Tick.Value, Is.EqualTo(tick));
        }

        private static RegularEnemyThreatInput WithSurveyor(RegularEnemyThreatInput input,
            SurveyorBehaviorSnapshot surveyor, SurveyorShotIntent? intent)
        {
            var behavior = new RegularEnemyBehaviorPairSnapshot(input.Behavior.Tick, input.Behavior.Walker, surveyor,
                input.Behavior.WalkerRecoveryIntent, intent, input.Behavior.NextExpectedTick);
            return RegularEnemyThreatInput.Gameplay(input.Tick, input.PlayerFinalAlive, input.PlayerCenter, behavior, input.Motion);
        }

        private static RegularEnemyThreatInput WithWalker(RegularEnemyThreatInput input, WalkerBehaviorSnapshot walker)
        {
            var behavior = new RegularEnemyBehaviorPairSnapshot(input.Behavior.Tick, walker, input.Behavior.Surveyor,
                input.Behavior.WalkerRecoveryIntent, input.Behavior.SurveyorShotIntent, input.Behavior.NextExpectedTick);
            return RegularEnemyThreatInput.Gameplay(input.Tick, input.PlayerFinalAlive, input.PlayerCenter, behavior, input.Motion);
        }

        private static RegularEnemyBehaviorPairInput EqualXBehaviorInput(int tick)
        {
            var next = Tick(checked(tick + 1));
            var walkerReaction = new RegularEnemyReactionSnapshot(Tick(tick), "walker", true, false, -1, -1, -1,
                false, 1000, false, false, next);
            var surveyorReaction = new RegularEnemyReactionSnapshot(Tick(tick), "surveyor", true, false, -1, -1, -1,
                false, 1000, false, true, next);
            return RegularEnemyBehaviorPairInput.Gameplay(Tick(tick), true, true, walkerReaction, surveyorReaction,
                0, 0, 5000, true, false);
        }

        private static RegularEnemyThreatInput FromBehavior(RegularEnemyBehaviorPairSnapshot behavior,
            PositionQ4096 player, PositionQ4096 walker)
        {
            var motion = new RegularEnemyMotionPairSnapshot(behavior.Tick, Motion("walker", walker, false),
                Motion("surveyor", new PositionQ4096(5000, 5000), false), behavior.NextExpectedTick);
            return RegularEnemyThreatInput.Gameplay(behavior.Tick, true, player, behavior, motion);
        }

        private static void AssertAtomicReject(RegularEnemyThreatSession session, RegularEnemyThreatInput input)
        {
            var before = State(session);
            Assert.Catch<Exception>(() => session.PreviewNext(input));
            Assert.That(State(session), Is.EqualTo(before));
        }

        private static string State(RegularEnemyThreatSession session)
        {
            if (!session.LatestSnapshot.HasValue) return session.NextExpectedTick.Value + "/none";
            var s = session.LatestSnapshot.Value;
            return string.Join("/", session.NextExpectedTick.Value, s.Tick.Value, s.NextExpectedTick.Value, s.PlayerDead,
                s.WalkerDead, s.SurveyorDead, s.HasPriorCenters, s.PriorPlayerCenter.X, s.PriorPlayerCenter.Y,
                s.PriorWalkerCenter.X, s.PriorWalkerCenter.Y, s.ExpectedShotOrdinal, s.HasActiveLine,
                s.LineCenterXQ4096, s.LineShotOrdinal, s.LineSpawnTick, s.LineExclusiveEndTick);
        }

        private static RegularEnemyThreatSession Session() => new RegularEnemyThreatSession(Tick(0), 100, 100, 100, 100, 1024);
        private static void Commit(RegularEnemyThreatSession session, RegularEnemyThreatInput input) => session.CommitNext(input, session.PreviewNext(input));
        private static void Commit(RegularEnemyThreatSession session, RegularEnemyThreatInput input, RegularEnemyThreatCandidate candidate) => session.CommitNext(input, candidate);

        private static RegularEnemyThreatInput Game(int tick, PositionQ4096 player, PositionQ4096 walker,
            bool playerAlive = true, bool walkerAlive = true, bool surveyorAlive = true, bool fire = false,
            long ordinal = 0, long? intentOrdinal = null, WalkerBehaviorPhase walkerPhase = WalkerBehaviorPhase.Approach,
            int targetXQ1000 = 0)
        {
            var next = Tick(checked(tick + 1));
            var effectiveWalkerPhase = walkerAlive ? walkerPhase : WalkerBehaviorPhase.Dead;
            var walkerEnd = effectiveWalkerPhase == WalkerBehaviorPhase.Approach || effectiveWalkerPhase == WalkerBehaviorPhase.Dead ? -1 : checked(tick + 10);
            var walkerMovement = effectiveWalkerPhase == WalkerBehaviorPhase.Approach ? RegularEnemyMovementIntent.Horizontal :
                effectiveWalkerPhase == WalkerBehaviorPhase.DashActive ? RegularEnemyMovementIntent.DashHorizontal : RegularEnemyMovementIntent.None;
            var walkerDirection = effectiveWalkerPhase == WalkerBehaviorPhase.DashActive ? 1 : 0;
            var walkerSnapshot = new WalkerBehaviorSnapshot(effectiveWalkerPhase, walkerEnd, walkerAlive,
                walkerMovement, walkerDirection, false, effectiveWalkerPhase == WalkerBehaviorPhase.DashTelegraph,
                effectiveWalkerPhase == WalkerBehaviorPhase.DashTelegraph || effectiveWalkerPhase == WalkerBehaviorPhase.DashActive ? 1 : 0, 0);
            SurveyorShotIntent? intent = fire ? new SurveyorShotIntent("surveyor", intentOrdinal ?? ordinal, targetXQ1000) : null;
            var surveyorSnapshot = new SurveyorBehaviorSnapshot(surveyorAlive ? (fire ? SurveyorBehaviorPhase.Fire : SurveyorBehaviorPhase.Relocate) : SurveyorBehaviorPhase.Dead,
                surveyorAlive ? checked(tick + (fire ? 1 : 10)) : -1, surveyorAlive, surveyorAlive && !fire ? RegularEnemyMovementIntent.Horizontal : RegularEnemyMovementIntent.None,
                surveyorAlive && !fire ? -1 : 0, false, fire, fire ? targetXQ1000 : 0, fire ? checked(ordinal + 1) : ordinal);
            var behavior = new RegularEnemyBehaviorPairSnapshot(Tick(tick), walkerSnapshot, surveyorSnapshot, null, intent, next);
            var motion = new RegularEnemyMotionPairSnapshot(Tick(tick), Motion("walker", walker, !walkerAlive),
                Motion("surveyor", new PositionQ4096(5000, 5000), !surveyorAlive), next);
            return RegularEnemyThreatInput.Gameplay(Tick(tick), playerAlive, player, behavior, motion);
        }

        private static RegularEnemyThreatInput Reset(int tick)
        {
            var next = Tick(checked(tick + 1));
            var behavior = new RegularEnemyBehaviorPairSnapshot(Tick(tick),
                new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Reset, -1, true, RegularEnemyMovementIntent.None, 0, false, false, 0, 0),
                new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Reset, -1, true, RegularEnemyMovementIntent.None, 0, false, false, 0, 0), null, null, next);
            var motion = new RegularEnemyMotionPairSnapshot(Tick(tick), Motion("walker", default, false), Motion("surveyor", default, false), next);
            return RegularEnemyThreatInput.EncounterReset(Tick(tick), true, behavior, motion);
        }

        private static RegularEnemyMotionSnapshot Motion(string id, PositionQ4096 position, bool dead) =>
            new RegularEnemyMotionSnapshot(id, position, default, 0, 0, 0, true, dead);
        private static SimulationTick Tick(int value) => new SimulationTick(value);
    }
}
