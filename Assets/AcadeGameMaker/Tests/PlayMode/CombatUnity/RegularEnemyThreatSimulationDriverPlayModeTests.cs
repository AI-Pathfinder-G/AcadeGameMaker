// AC-COM-001, AC-COM-003; affected AC-MOV-001, AC-WT-005. VD-03 M3D2 focused Unity integration evidence.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    internal sealed class RegularEnemyThreatSimulationDriverPlayModeTests
    {
        public enum RegistrationFault { FirstSource, CombatTick, AttackTick, PlayerTick, PriorAttempt }
        public enum DeliveryFault { StaleQueue, FutureQueue, ForeignCandidate, StaleCandidate, FutureCandidate }
        public enum RequestFault { Capacity, WalkerId, WalkerSource, WalkerTarget, WalkerAmount, WalkerKind, WalkerTick, WalkerDuplicate, LineId, LineSource, LineTarget, LineAmount, LineKind, LineTick, LineOrdinalOverflow, LineOrdinalNegative, LineOrdinalShort, LineOrdinalNonDigit, LineDuplicate, ReversedOrder }
        public enum PublicationFault { CombatMissing, CombatStale, CombatFuture, BehaviorMissing, BehaviorStale, BehaviorFuture, MotionMissing, MotionStale, MotionFuture, MovementStale, MovementFuture, MixedReset, MalformedSecondRole }
        public enum RoleDeathFault { Player, Walker, Surveyor }
        public enum GeometryFault { PlayerInactive, PlayerTrigger, PlayerDisabled, PlayerHorizontal, PlayerOffset, PlayerSizeZero, PlayerSizeNaN, PlayerSizeInfinity, PlayerSizeSubQuantum, PlayerScale, PlayerScaleY, PlayerRotation, PlayerMissingBody, PlayerDynamic, PlayerUnsimulated, PlayerRootCrosswire, PlayerColliderCrosswire, PlayerExtraSolid, WalkerInactive, WalkerTrigger, WalkerDisabled, WalkerOffset, WalkerSizeZero, WalkerSizeNaN, WalkerSizeInfinity, WalkerSizeSubQuantum, WalkerScale, WalkerScaleY, WalkerRotation, WalkerMissingBody, WalkerDynamic, WalkerUnsimulated, WalkerConstraint, WalkerInterpolation, WalkerFullContacts, WalkerLinearVelocity, WalkerLinearVelocityNaN, WalkerAngularVelocity, WalkerBodyRotation, WalkerEdgeRadius, WalkerRootCrosswire, WalkerColliderCrosswire, WalkerExtraSolid, PlayerHealth, PlayerInvulnerability, PlayerAttackable, PlayerTransfer, WalkerHealth, WalkerInvulnerability, WalkerAttackable, WalkerTransfer, SurveyorHealth, SurveyorInvulnerability, SurveyorAttackable, SurveyorTransfer, SurveyorColliderMissing, SurveyorRootMissing }
        private readonly List<GameObject> _objects = new List<GameObject>();
        private SimulationMode2D _priorMode;

        [SetUp]
        public void SetUp() { _priorMode = Physics2D.simulationMode; Physics2D.simulationMode = SimulationMode2D.Script; }

        [TearDown]
        public void TearDown()
        {
            for (var index = _objects.Count - 1; index >= 0; index--) UnityEngine.Object.DestroyImmediate(_objects[index]);
            _objects.Clear();
            Physics2D.simulationMode = _priorMode;
        }

        [Test]
        public void AC_COM_001_AC_COM_003_OrderBootstrapAndImmutablePublicationAreExact()
        {
            Assert.That(Order<TransferSimulationDriver>(), Is.EqualTo(-200));
            Assert.That(Order<CombatSimulationDriver>(), Is.EqualTo(-190));
            Assert.That(Order<RegularEnemyReactionSimulationDriver>(), Is.EqualTo(-180));
            Assert.That(Order<RegularEnemyBehaviorSimulationDriver>(), Is.EqualTo(-170));
            Assert.That(Order<RegularEnemyLocomotionSimulationDriver>(), Is.EqualTo(-160));
            Assert.That(Attribute.GetCustomAttribute(typeof(PlayerMovementController), typeof(DefaultExecutionOrder)), Is.Null);
            Assert.That(Order<RegularEnemyThreatSimulationDriver>(), Is.EqualTo(100));

            var rig = CreateRig(new Vector2(0.25f, 0f));
            Assert.That(rig.Combat.ThreatDeliveryLaneRegisteredForTests, Is.True);
            AdvanceThroughThreat(rig);

            Assert.That(rig.Threat.LatestCarriedViewForTests.Value.Tick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(rig.Threat.LatestCarriedViewForTests.Value.Snapshot.NextExpectedTick, Is.EqualTo(new SimulationTick(1)));
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.EqualTo(1));
        }

        [Test]
        public void AC_COM_001_ContactFromSourceTickAppearsOnlyAtNextCombatTick()
        {
            var rig = CreateRig(new Vector2(0.25f, 0f));
            AdvanceThroughThreat(rig);
            Assert.That(Snapshot(rig.Combat.LatestOutcomeForTests, "player").CurrentHealth, Is.EqualTo(5));

            rig.Transfer.AdvanceTransferPhaseForTests();
            var delivered = rig.Combat.AdvanceCombatPhaseForTests();

            Assert.That(Snapshot(delivered, "player").CurrentHealth, Is.EqualTo(4));
            Assert.That(delivered.CanonicalRequestTrace.Count, Is.EqualTo(1));
            Assert.That(delivered.CanonicalRequestTrace[0].RequestId, Is.EqualTo("enemy.contact/walker/0000000000"));
            Assert.That(delivered.CanonicalRequestTrace[0].Tick, Is.EqualTo(new SimulationTick(1)));
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
        }

        [Test]
        public void AC_COM_003_EmptyPublicationIsRequiredAndMissingNextBatchPreservesCombatOutcome()
        {
            var rig = CreateRig(new Vector2(20f, 0f));
            AdvanceThroughThreat(rig);
            rig.Transfer.AdvanceTransferPhaseForTests();
            var tickOne = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(tickOne.CanonicalRequestTrace.Count, Is.Zero);
            rig.Behavior.AdvanceBehaviorPhaseForTests();
            var motion = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            ApplyEnemyBodies(rig, motion.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();

            rig.Transfer.AdvanceTransferPhaseForTests();
            Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
            Assert.That(rig.Combat.LatestOutcomeForTests.Tick, Is.EqualTo(new SimulationTick(1)));
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
        }

        [Test]
        public void AC_COM_003_DeliveryPreflightCopiesInputAndRejectsDuplicateOrMalformedRequests()
        {
            var rig = CreateRig(new Vector2(20f, 0f));
            AdvanceToPostMovementWithoutThreat(rig);
            var source = new SimulationTick(0);
            var delivery = new SimulationTick(1);
            var requests = new[] { WalkerRequest(source, delivery) };
            var candidate = rig.Combat.PreflightThreatDelivery(source, requests);
            requests[0] = new DamageRequest("mutated", "walker", "player", 1, DamageKind.EnemyContact, delivery);
            rig.Combat.CommitPreflightedThreatDelivery(candidate);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.EqualTo(1));
            Assert.Throws<InvalidOperationException>(() => rig.Combat.CommitPreflightedThreatDelivery(candidate));

            var another = CreateRig(new Vector2(20f, 0f));
            AdvanceToPostMovementWithoutThreat(another);
            Assert.Throws<ArgumentException>(() => another.Combat.PreflightThreatDelivery(source, new[]
            {
                new DamageRequest("enemy.contact/walker/0000000001", "walker", "player", 1, DamageKind.EnemyContact, delivery)
            }));
            Assert.That(another.Combat.ThreatDeliveryCountForTests, Is.Zero);
        }

        [Test]
        public void AC_COM_003_WalkerThenLineOrderingAndNineteenDigitOrdinalAreExact()
        {
            var rig = CreateRig(new Vector2(20f, 0f));
            AdvanceToPostMovementWithoutThreat(rig);
            var source = new SimulationTick(0);
            var delivery = new SimulationTick(1);
            var line = new DamageRequest("enemy.line/surveyor/0000000000000000000/0000000000", "surveyor", "player", 1, DamageKind.EnemyRanged, delivery);
            Assert.DoesNotThrow(() => rig.Combat.PreflightThreatDelivery(source, new[] { WalkerRequest(source, delivery), line }));
            Assert.Throws<ArgumentException>(() => rig.Combat.PreflightThreatDelivery(source, new[] { line, WalkerRequest(source, delivery) }));
            Assert.Throws<ArgumentException>(() => rig.Combat.PreflightThreatDelivery(source, new[]
            {
                new DamageRequest("enemy.line/surveyor/9223372036854775808/0000000000", "surveyor", "player", 1, DamageKind.EnemyRanged, delivery)
            }));
        }

        [Test]
        public void AC_COM_003_GeometryDriftRejectsBeforeFirstThreatSessionAndQueueMutation()
        {
            var rig = CreateRig(new Vector2(20f, 0f));
            var extra = rig.Player.gameObject.AddComponent<BoxCollider2D>();
            AdvanceToPostMovementWithoutThreat(rig);

            Assert.Throws<InvalidOperationException>(() => rig.Threat.AdvanceThreatPhaseForTests());
            Assert.That(rig.Threat.HasSessionForTests, Is.False);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
            UnityEngine.Object.DestroyImmediate(extra);
        }

        [Test]
        public void AC_COM_003_NonzeroBootstrapUsesExactGlobalTick()
        {
            var rig = CreateRig(new Vector2(20f, 0f), 7);
            AdvanceThroughThreat(rig);
            Assert.That(rig.Threat.LatestCarriedViewForTests.Value.Tick, Is.EqualTo(new SimulationTick(7)));
            Assert.That(rig.Threat.NextExpectedTickForTests, Is.EqualTo(new SimulationTick(8)));
        }

        [Test]
        public void AC_COM_003_DuplicateAndLateRegistrationRejectWithoutWeakeningLegacyCombat()
        {
            var legacy = CreateRig(new Vector2(20f, 0f), includeThreat: false);
            legacy.Transfer.AdvanceTransferPhaseForTests();
            Assert.DoesNotThrow(() => legacy.Combat.AdvanceCombatPhaseForTests());
            Assert.Throws<InvalidOperationException>(() => legacy.Combat.RegisterThreatDeliveryLane(new SimulationTick(1)));

            var registered = CreateRig(new Vector2(20f, 0f));
            Assert.Throws<InvalidOperationException>(() => registered.Combat.RegisterThreatDeliveryLane(new SimulationTick(0)));
            Assert.That(registered.Combat.ThreatDeliveryCountForTests, Is.Zero);
        }

        [Test]
        public void AC_COM_003_ResetConsumesAndDiscardsPriorThreatThenPublishesRequiredEmptyBatch()
        {
            var rig = CreateRig(new Vector2(0.25f, 0f));
            AdvanceThroughThreat(rig);
            var resetTick = new SimulationTick(1);
            rig.Combat.SubmitForTests(new CombatSimulationInput(resetTick, null, null, null, Array.Empty<DamageRequest>(), true));

            rig.Transfer.AdvanceTransferPhaseForTests();
            var resetCombat = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(resetCombat.EncounterReset, Is.True);
            Assert.That(resetCombat.CanonicalRequestTrace.Count, Is.Zero);
            Assert.That(Snapshot(resetCombat, "player").CurrentHealth, Is.EqualTo(5));
            Assert.That(resetCombat.Attack.Snapshot.Tick, Is.EqualTo(resetTick));
            Assert.That(resetCombat.Attack.Snapshot.NextExpectedTick, Is.EqualTo(new SimulationTick(2)));
            Assert.That(resetCombat.Attack.Snapshot.ConsumedSequenceCount, Is.Zero);
            rig.Behavior.AdvanceBehaviorPhaseForTests();
            var resetMotion = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            ApplyEnemyBodies(rig, resetMotion.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();
            var resetThreat = rig.Threat.AdvanceThreatPhaseForTests();

            Assert.That(resetThreat.Snapshot.HasPriorCenters, Is.False);
            Assert.That(resetThreat.Snapshot.HasActiveLine, Is.False);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.EqualTo(1));
            rig.Transfer.AdvanceTransferPhaseForTests();
            Assert.DoesNotThrow(() => rig.Combat.AdvanceCombatPhaseForTests());
        }

        [Test]
        public void AC_COM_003_RegistrationUsesDistinctCombatHorizonBoundary()
        {
            var lastSource = int.MaxValue - 46; // H=max(21, player invulnerability 45)
            var accepted = CreateRig(new Vector2(20f, 0f), lastSource);
            Assert.That(accepted.Combat.ThreatDeliveryLaneRegisteredForTests, Is.True);

            var rejected = CreateRig(new Vector2(20f, 0f), int.MaxValue - 45, includeThreat: false);
            rejected.Combat.PrepareThreatDeliveryRegistration();
            Assert.Throws<InvalidOperationException>(() => rejected.Combat.RegisterThreatDeliveryLane(new SimulationTick(int.MaxValue - 45)));
            Assert.That(rejected.Combat.ThreatDeliveryLaneRegisteredForTests, Is.False);
        }

        [Test]
        public void AC_COM_003_PreparationIsInactiveGeometryFailureCanRetryAndUnpreparedRegistrationRejects()
        {
            var rig = CreateRig(new Vector2(20f, 0f), includeThreat: false);
            Assert.Throws<InvalidOperationException>(() => rig.Combat.RegisterThreatDeliveryLane(new SimulationTick(0)));
            Assert.That(rig.Combat.ThreatDeliveryLaneRegisteredForTests, Is.False);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);

            var threatObject = Track(new GameObject("RetryThreatDriver"));
            threatObject.SetActive(false);
            var threat = threatObject.AddComponent<RegularEnemyThreatSimulationDriver>();
            var extra = rig.Player.gameObject.AddComponent<BoxCollider2D>();
            Assert.Throws<InvalidOperationException>(() => threat.ConfigureForTests(rig.Player, rig.Combat, rig.Behavior, rig.Locomotion));
            Assert.That(rig.Combat.ThreatDeliveryLaneRegisteredForTests, Is.False);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
            UnityEngine.Object.DestroyImmediate(extra);

            Assert.DoesNotThrow(() => threat.ConfigureForTests(rig.Player, rig.Combat, rig.Behavior, rig.Locomotion));
            Assert.That(rig.Combat.ThreatDeliveryLaneRegisteredForTests, Is.True);
            Assert.Throws<InvalidOperationException>(() => rig.Combat.RegisterThreatDeliveryLane(new SimulationTick(0)));
        }

        [TestCase("ThreatPlayer")]
        [TestCase("ThreatCombat")]
        [TestCase("ThreatBehavior")]
        [TestCase("ThreatLocomotion")]
        [TestCase("BehaviorReaction")]
        [TestCase("BehaviorPlayer")]
        [TestCase("BehaviorCombat")]
        [TestCase("LocomotionReaction")]
        [TestCase("LocomotionBehavior")]
        [TestCase("LocomotionPlayer")]
        [TestCase("LocomotionCombat")]
        public void AC_COM_003_CrossWiredGraphRejectsBeforeLaneActivation(string fault)
        {
            var intended = CreateRig(new Vector2(20f, 0f), includeThreat: false);
            var other = CreateRig(new Vector2(30f, 0f), includeThreat: false);
            var player = fault == "ThreatPlayer" ? other.Player : intended.Player;
            var combat = fault == "ThreatCombat" ? other.Combat : intended.Combat;
            var behavior = fault == "ThreatBehavior" ? other.Behavior : intended.Behavior;
            var locomotion = fault == "ThreatLocomotion" ? other.Locomotion : intended.Locomotion;
            if (fault == "BehaviorReaction") SetPrivate(intended.Behavior, "_reactionDriver", other.Reaction);
            if (fault == "BehaviorPlayer") SetPrivate(intended.Behavior, "_player", other.Player);
            if (fault == "BehaviorCombat") SetPrivate(intended.Behavior, "_combatDriver", other.Combat);
            if (fault == "LocomotionReaction") SetPrivate(intended.Locomotion, "_reactionDriver", other.Reaction);
            if (fault == "LocomotionBehavior") SetPrivate(intended.Locomotion, "_behaviorDriver", other.Behavior);
            if (fault == "LocomotionPlayer") SetPrivate(intended.Locomotion, "_player", other.Player);
            if (fault == "LocomotionCombat") SetPrivate(intended.Locomotion, "_combatDriver", other.Combat);
            var threatObject = Track(new GameObject("CrossWiredThreat"));
            threatObject.SetActive(false);
            var threat = threatObject.AddComponent<RegularEnemyThreatSimulationDriver>();

            Assert.Throws<InvalidOperationException>(() => threat.ConfigureForTests(player, combat, behavior, locomotion));
            Assert.That(intended.Combat.ThreatDeliveryLaneRegisteredForTests, Is.False);
            Assert.That(intended.Combat.ThreatDeliveryCountForTests, Is.Zero);
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-003. The production lifecycle, not only the test seam, completes registration before FixedUpdate.
        [Test]
        public void AC_COM_003_ActiveProductionAwakeRegistersTheExactLane()
        {
            var rig = CreateRig(new Vector2(20f, 0f), includeThreat: false);
            var value = Track(new GameObject("ProductionAwakeThreat"));
            value.SetActive(false);
            var threat = value.AddComponent<RegularEnemyThreatSimulationDriver>();
            SetPrivate(threat, "_player", rig.Player);
            SetPrivate(threat, "_combatDriver", rig.Combat);
            SetPrivate(threat, "_behaviorDriver", rig.Behavior);
            SetPrivate(threat, "_locomotionDriver", rig.Locomotion);

            value.SetActive(true);

            Assert.That(rig.Combat.ThreatDeliveryLaneRegisteredForTests, Is.True);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
            Assert.That(threat.HasSessionForTests, Is.False);
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-003. Registration is all-or-nothing across every exact owner clock and prior-attempt guard.
        [TestCase(RegistrationFault.FirstSource)]
        [TestCase(RegistrationFault.CombatTick)]
        [TestCase(RegistrationFault.AttackTick)]
        [TestCase(RegistrationFault.PlayerTick)]
        [TestCase(RegistrationFault.PriorAttempt)]
        public void AC_COM_003_RegistrationClockAndPriorAttemptFaultsAreAtomic(RegistrationFault fault)
        {
            var rig = CreateRig(new Vector2(20f, 0f), includeThreat: false);
            rig.Combat.PrepareThreatDeliveryRegistration();
            var source = new SimulationTick(0);
            if (fault == RegistrationFault.FirstSource) source = new SimulationTick(1);
            if (fault == RegistrationFault.CombatTick) SetPrivate(rig.Combat, "_combatSession", new CombatSession(new SimulationTick(1)));
            if (fault == RegistrationFault.AttackTick) SetPrivate(rig.Combat, "_attackSession", new BasicAttackSession(new SimulationTick(1)));
            if (fault == RegistrationFault.PlayerTick) SetPrivate(rig.Player, "_motor", new PlayerMovementMotor(new SimulationTick(1)));
            if (fault == RegistrationFault.PriorAttempt)
            {
                rig.Transfer.AdvanceTransferPhaseForTests();
                rig.Combat.AdvanceCombatPhaseForTests();
                source = new SimulationTick(1);
            }

            Assert.Throws<InvalidOperationException>(() => rig.Combat.RegisterThreatDeliveryLane(source));
            Assert.That(rig.Combat.ThreatDeliveryLaneRegisteredForTests, Is.False);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-003. Stale/future batches and forged candidates reject before either deterministic owner advances.
        [TestCase(DeliveryFault.StaleQueue)]
        [TestCase(DeliveryFault.FutureQueue)]
        [TestCase(DeliveryFault.ForeignCandidate)]
        [TestCase(DeliveryFault.StaleCandidate)]
        [TestCase(DeliveryFault.FutureCandidate)]
        public void AC_COM_003_StaleFutureAndForgedDeliveryPreserveOwners(DeliveryFault fault)
        {
            var rig = CreateRig(new Vector2(20f, 0f));
            AdvanceToPostMovementWithoutThreat(rig);
            var source = new SimulationTick(0);
            var delivery = new SimulationTick(1);
            var queue = GetPrivate<SortedDictionary<int, RegularEnemyThreatDeliveryBatch>>(rig.Combat, "_threatDeliveries");
            var attack = GetPrivate<BasicAttackSession>(rig.Combat, "_attackSession");
            var combat = GetPrivate<CombatSession>(rig.Combat, "_combatSession");
            if (fault == DeliveryFault.StaleQueue)
                queue.Add(0, new RegularEnemyThreatDeliveryBatch(new SimulationTick(-1), new SimulationTick(0), Array.Empty<DamageRequest>()));
            else if (fault == DeliveryFault.FutureQueue)
                queue.Add(2, new RegularEnemyThreatDeliveryBatch(new SimulationTick(1), new SimulationTick(2), Array.Empty<DamageRequest>()));
            else
            {
                var owner = fault == DeliveryFault.ForeignCandidate ? new object() : GetPrivate<object>(rig.Combat, "_threatDeliveryOwner");
                var candidateSource = fault == DeliveryFault.StaleCandidate ? new SimulationTick(-1) : fault == DeliveryFault.FutureCandidate ? new SimulationTick(1) : source;
                var candidateDelivery = fault == DeliveryFault.StaleCandidate ? new SimulationTick(0) : fault == DeliveryFault.FutureCandidate ? new SimulationTick(2) : delivery;
                var candidate = new RegularEnemyThreatDeliveryCandidate(owner, candidateSource, candidateDelivery, Array.Empty<DamageRequest>());
                Assert.Throws<InvalidOperationException>(() => rig.Combat.CommitPreflightedThreatDelivery(candidate));
            }

            if (fault == DeliveryFault.StaleQueue || fault == DeliveryFault.FutureQueue)
                Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
            Assert.That(attack.NextExpectedTick, Is.EqualTo(delivery));
            Assert.That(combat.NextExpectedTick, Is.EqualTo(delivery));
            Assert.That(rig.Combat.LatestOutcomeForTests.Tick, Is.EqualTo(source));
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.EqualTo(fault == DeliveryFault.StaleQueue || fault == DeliveryFault.FutureQueue ? 1 : 0));
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-003. A later Combat preflight failure retains the distinct threat queue but consumes the legacy input.
        [Test]
        public void AC_COM_003_LaterCombatPreflightPreservesThreatQueueButConsumesLegacyInput()
        {
            var rig = CreateRig(new Vector2(0.25f, 0f));
            AdvanceThroughThreat(rig);
            var tick = new SimulationTick(1);
            rig.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null,
                new[] { new DamageRequest("legacy-consumed", "hazard", "walker", 1, DamageKind.HeavyImpact, tick) }, false));
            var originalAttack = GetPrivate<BasicAttackSession>(rig.Combat, "_attackSession");
            SetPrivate(rig.Combat, "_attackSession", new BasicAttackSession(new SimulationTick(2)));
            rig.Transfer.AdvanceTransferPhaseForTests();

            Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.EqualTo(1));
            Assert.That(rig.Combat.LatestOutcomeForTests.Tick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(GetPrivate<CombatSession>(rig.Combat, "_combatSession").NextExpectedTick, Is.EqualTo(tick));
            SetPrivate(rig.Combat, "_attackSession", originalAttack);
            var retried = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(Snapshot(retried, "walker").CurrentHealth, Is.EqualTo(9), "legacy external input must stay consumed");
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-003. Every field of both canonical M3D request forms is validated independently.
        [TestCase(RequestFault.Capacity)]
        [TestCase(RequestFault.WalkerId)]
        [TestCase(RequestFault.WalkerSource)]
        [TestCase(RequestFault.WalkerTarget)]
        [TestCase(RequestFault.WalkerAmount)]
        [TestCase(RequestFault.WalkerKind)]
        [TestCase(RequestFault.WalkerTick)]
        [TestCase(RequestFault.WalkerDuplicate)]
        [TestCase(RequestFault.LineId)]
        [TestCase(RequestFault.LineSource)]
        [TestCase(RequestFault.LineTarget)]
        [TestCase(RequestFault.LineAmount)]
        [TestCase(RequestFault.LineKind)]
        [TestCase(RequestFault.LineTick)]
        [TestCase(RequestFault.LineOrdinalOverflow)]
        [TestCase(RequestFault.LineOrdinalNegative)]
        [TestCase(RequestFault.LineOrdinalShort)]
        [TestCase(RequestFault.LineOrdinalNonDigit)]
        [TestCase(RequestFault.LineDuplicate)]
        [TestCase(RequestFault.ReversedOrder)]
        public void AC_COM_003_MalformedThreatFieldMatrixRejectsWithoutQueueMutation(RequestFault fault)
        {
            var rig = CreateRig(new Vector2(20f, 0f));
            AdvanceToPostMovementWithoutThreat(rig);
            var source = new SimulationTick(0); var delivery = new SimulationTick(1);
            var walker = WalkerRequest(source, delivery);
            var line = LineRequest(source, delivery, 0);
            IReadOnlyList<DamageRequest> requests;
            switch (fault)
            {
                case RequestFault.Capacity: requests = new[] { walker, line, line }; break;
                case RequestFault.WalkerId: requests = new[] { new DamageRequest("enemy.contact/walker/0000000001", "walker", "player", 1, DamageKind.EnemyContact, delivery) }; break;
                case RequestFault.WalkerSource: requests = new[] { new DamageRequest(walker.RequestId, "surveyor", "player", 1, DamageKind.EnemyContact, delivery) }; break;
                case RequestFault.WalkerTarget: requests = new[] { new DamageRequest(walker.RequestId, "walker", "walker", 1, DamageKind.EnemyContact, delivery) }; break;
                case RequestFault.WalkerAmount: requests = new[] { new DamageRequest(walker.RequestId, "walker", "player", 2, DamageKind.EnemyContact, delivery) }; break;
                case RequestFault.WalkerKind: requests = new[] { new DamageRequest(walker.RequestId, "walker", "player", 1, DamageKind.EnemyRanged, delivery) }; break;
                case RequestFault.WalkerTick: requests = new[] { new DamageRequest(walker.RequestId, "walker", "player", 1, DamageKind.EnemyContact, source) }; break;
                case RequestFault.WalkerDuplicate: requests = new[] { walker, walker }; break;
                case RequestFault.LineId: requests = new[] { new DamageRequest("enemy.line/surveyor/0000000000000000000/0000000001", "surveyor", "player", 1, DamageKind.EnemyRanged, delivery) }; break;
                case RequestFault.LineSource: requests = new[] { new DamageRequest(line.RequestId, "walker", "player", 1, DamageKind.EnemyRanged, delivery) }; break;
                case RequestFault.LineTarget: requests = new[] { new DamageRequest(line.RequestId, "surveyor", "walker", 1, DamageKind.EnemyRanged, delivery) }; break;
                case RequestFault.LineAmount: requests = new[] { new DamageRequest(line.RequestId, "surveyor", "player", 2, DamageKind.EnemyRanged, delivery) }; break;
                case RequestFault.LineKind: requests = new[] { new DamageRequest(line.RequestId, "surveyor", "player", 1, DamageKind.EnemyContact, delivery) }; break;
                case RequestFault.LineTick: requests = new[] { new DamageRequest(line.RequestId, "surveyor", "player", 1, DamageKind.EnemyRanged, source) }; break;
                case RequestFault.LineOrdinalOverflow: requests = new[] { new DamageRequest("enemy.line/surveyor/9223372036854775808/0000000000", "surveyor", "player", 1, DamageKind.EnemyRanged, delivery) }; break;
                case RequestFault.LineOrdinalNegative: requests = new[] { new DamageRequest("enemy.line/surveyor/-000000000000000000/0000000000", "surveyor", "player", 1, DamageKind.EnemyRanged, delivery) }; break;
                case RequestFault.LineOrdinalShort: requests = new[] { new DamageRequest("enemy.line/surveyor/000000000000000000/0000000000", "surveyor", "player", 1, DamageKind.EnemyRanged, delivery) }; break;
                case RequestFault.LineOrdinalNonDigit: requests = new[] { new DamageRequest("enemy.line/surveyor/000000000000000000x/0000000000", "surveyor", "player", 1, DamageKind.EnemyRanged, delivery) }; break;
                case RequestFault.LineDuplicate: requests = new[] { line, line }; break;
                default: requests = new[] { line, walker }; break;
            }
            Assert.Catch<Exception>(() => rig.Combat.PreflightThreatDelivery(source, requests));
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
            Assert.That(GetPrivate<CombatSession>(rig.Combat, "_combatSession").NextExpectedTick, Is.EqualTo(delivery));
            Assert.That(GetPrivate<BasicAttackSession>(rig.Combat, "_attackSession").NextExpectedTick, Is.EqualTo(delivery));
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-003. Empty and full-capacity batches are both exact immutable publications.
        [TestCase(0)]
        [TestCase(2)]
        public void AC_COM_003_ZeroAndTwoRequestCapacityCommitExactly(int count)
        {
            var rig = CreateRig(new Vector2(20f, 0f));
            AdvanceToPostMovementWithoutThreat(rig);
            var source = new SimulationTick(0); var delivery = new SimulationTick(1);
            var requests = count == 0 ? Array.Empty<DamageRequest>() : new[] { WalkerRequest(source, delivery), LineRequest(source, delivery, 0) };
            var candidate = rig.Combat.PreflightThreatDelivery(source, requests);
            rig.Combat.CommitPreflightedThreatDelivery(candidate);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.EqualTo(1));
            Assert.That(candidate.RequestCount, Is.EqualTo(count));
        }

        // REQ-COM-002, REQ-COM-004; AC-COM-003. Upstream absence, clock skew, mixed reset, and second-role corruption preserve the pair boundary.
        [TestCase(PublicationFault.BehaviorMissing)]
        [TestCase(PublicationFault.CombatMissing)]
        [TestCase(PublicationFault.CombatStale)]
        [TestCase(PublicationFault.CombatFuture)]
        [TestCase(PublicationFault.BehaviorStale)]
        [TestCase(PublicationFault.BehaviorFuture)]
        [TestCase(PublicationFault.MotionMissing)]
        [TestCase(PublicationFault.MotionStale)]
        [TestCase(PublicationFault.MotionFuture)]
        [TestCase(PublicationFault.MovementStale)]
        [TestCase(PublicationFault.MovementFuture)]
        [TestCase(PublicationFault.MixedReset)]
        [TestCase(PublicationFault.MalformedSecondRole)]
        public void AC_COM_003_UpstreamPublicationFaultMatrixPreservesThreatPair(PublicationFault fault)
        {
            var rig = CreateRig(new Vector2(20f, 0f));
            AdvanceToPostMovementWithoutThreat(rig);
            var behavior = rig.Behavior.LatestCarriedViewForThreatBridge.Value;
            var motion = rig.Locomotion.LatestCarriedViewForM3D.Value;
            if (fault == PublicationFault.CombatMissing) SetPrivate(rig.Combat, "_hasLatestOutcome", false);
            if (fault == PublicationFault.CombatStale || fault == PublicationFault.CombatFuture)
            {
                var current = rig.Combat.LatestOutcomeForTests;
                var tick = new SimulationTick(fault == PublicationFault.CombatStale ? -1 : 1);
                SetPrivate(rig.Combat, "_latestOutcome", new CombatSimulationOutcome(tick, current.EncounterReset, current.Attack,
                    current.CanonicalRequestTrace, current.DamageResults, current.Combatants));
            }
            if (fault == PublicationFault.BehaviorMissing) SetPrivate(rig.Behavior, "_carried", (CarriedBehaviorView?)null);
            if (fault == PublicationFault.BehaviorStale) SetPrivate(rig.Behavior, "_carried", (CarriedBehaviorView?)RetickBehavior(behavior, -1));
            if (fault == PublicationFault.BehaviorFuture) SetPrivate(rig.Behavior, "_carried", (CarriedBehaviorView?)RetickBehavior(behavior, 1));
            if (fault == PublicationFault.MotionMissing) SetPrivate(rig.Locomotion, "_carried", (CarriedEnemyMotionView?)null);
            if (fault == PublicationFault.MotionStale) SetPrivate(rig.Locomotion, "_carried", (CarriedEnemyMotionView?)RetickMotion(motion, -1));
            if (fault == PublicationFault.MotionFuture) SetPrivate(rig.Locomotion, "_carried", (CarriedEnemyMotionView?)RetickMotion(motion, 1));
            if (fault == PublicationFault.MovementStale) SetPrivate(rig.Player, "_motor", new PlayerMovementMotor(new SimulationTick(0)));
            if (fault == PublicationFault.MovementFuture) SetPrivate(rig.Player, "_motor", new PlayerMovementMotor(new SimulationTick(2)));
            if (fault == PublicationFault.MixedReset)
            {
                var current = rig.Combat.LatestOutcomeForTests;
                SetPrivate(rig.Combat, "_latestOutcome", new CombatSimulationOutcome(current.Tick, true, current.Attack,
                    current.CanonicalRequestTrace, current.DamageResults, current.Combatants));
            }
            if (fault == PublicationFault.MalformedSecondRole)
            {
                var pair = motion.Snapshot;
                var s = pair.Surveyor;
                var malformed = new RegularEnemyMotionSnapshot("walker", s.Position, s.Velocity, s.XPositionRemainder,
                    s.YPositionRemainder, s.GravityRemainder, s.Grounded, s.Dead);
                var changed = new RegularEnemyMotionPairSnapshot(pair.Tick, pair.Walker, malformed, pair.NextExpectedTick);
                SetPrivate(rig.Locomotion, "_carried", (CarriedEnemyMotionView?)new CarriedEnemyMotionView(motion.Tick, changed));
            }

            Assert.Catch<Exception>(() => rig.Threat.AdvanceThreatPhaseForTests());
            Assert.That(rig.Threat.HasSessionForTests, Is.False);
            Assert.That(rig.Threat.LatestCarriedViewForTests.HasValue, Is.False);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
        }

        // REQ-COM-002, REQ-COM-004; AC-COM-003. Complete frozen scalar and player/walker authoring mutations reject before lane activation.
        [TestCase(GeometryFault.PlayerTrigger)]
        [TestCase(GeometryFault.PlayerInactive)]
        [TestCase(GeometryFault.PlayerDisabled)]
        [TestCase(GeometryFault.PlayerHorizontal)]
        [TestCase(GeometryFault.PlayerOffset)]
        [TestCase(GeometryFault.PlayerSizeZero)]
        [TestCase(GeometryFault.PlayerSizeSubQuantum)]
        [TestCase(GeometryFault.PlayerScale)]
        [TestCase(GeometryFault.PlayerScaleY)]
        [TestCase(GeometryFault.PlayerRotation)]
        [TestCase(GeometryFault.PlayerDynamic)]
        [TestCase(GeometryFault.PlayerUnsimulated)]
        [TestCase(GeometryFault.PlayerRootCrosswire)]
        [TestCase(GeometryFault.PlayerColliderCrosswire)]
        [TestCase(GeometryFault.PlayerExtraSolid)]
        [TestCase(GeometryFault.WalkerTrigger)]
        [TestCase(GeometryFault.WalkerInactive)]
        [TestCase(GeometryFault.WalkerDisabled)]
        [TestCase(GeometryFault.WalkerOffset)]
        [TestCase(GeometryFault.WalkerSizeZero)]
        [TestCase(GeometryFault.WalkerSizeSubQuantum)]
        [TestCase(GeometryFault.WalkerScale)]
        [TestCase(GeometryFault.WalkerScaleY)]
        [TestCase(GeometryFault.WalkerRotation)]
        [TestCase(GeometryFault.WalkerMissingBody)]
        [TestCase(GeometryFault.WalkerDynamic)]
        [TestCase(GeometryFault.WalkerUnsimulated)]
        [TestCase(GeometryFault.WalkerConstraint)]
        [TestCase(GeometryFault.WalkerInterpolation)]
        [TestCase(GeometryFault.WalkerFullContacts)]
        [TestCase(GeometryFault.WalkerLinearVelocity)]
        [TestCase(GeometryFault.WalkerBodyRotation)]
        [TestCase(GeometryFault.WalkerEdgeRadius)]
        [TestCase(GeometryFault.WalkerRootCrosswire)]
        [TestCase(GeometryFault.WalkerColliderCrosswire)]
        [TestCase(GeometryFault.WalkerExtraSolid)]
        [TestCase(GeometryFault.PlayerHealth)]
        [TestCase(GeometryFault.PlayerInvulnerability)]
        [TestCase(GeometryFault.PlayerAttackable)]
        [TestCase(GeometryFault.PlayerTransfer)]
        [TestCase(GeometryFault.WalkerHealth)]
        [TestCase(GeometryFault.WalkerInvulnerability)]
        [TestCase(GeometryFault.WalkerAttackable)]
        [TestCase(GeometryFault.WalkerTransfer)]
        [TestCase(GeometryFault.SurveyorHealth)]
        [TestCase(GeometryFault.SurveyorInvulnerability)]
        [TestCase(GeometryFault.SurveyorAttackable)]
        [TestCase(GeometryFault.SurveyorTransfer)]
        [TestCase(GeometryFault.SurveyorColliderMissing)]
        [TestCase(GeometryFault.SurveyorRootMissing)]
        public void AC_COM_003_FrozenGeometryAndScalarMutationMatrixRejectsAtomically(GeometryFault fault)
        {
            var rig = CreateRig(new Vector2(20f, 0f), includeThreat: false);
            ApplyGeometryFault(rig, fault);
            var value = Track(new GameObject("MalformedThreat")); value.SetActive(false);
            var threat = value.AddComponent<RegularEnemyThreatSimulationDriver>();

            Assert.Catch<Exception>(() => threat.ConfigureForTests(rig.Player, rig.Combat, rig.Behavior, rig.Locomotion));
            Assert.That(rig.Combat.ThreatDeliveryLaneRegisteredForTests, Is.False);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
            Assert.That(threat.HasSessionForTests, Is.False);
        }

        // REQ-COM-002; AC-COM-003. These assignments cannot persist as malformed Unity authoring: Unity sanitizes/clamps them or preserves required components/constraints.
        [TestCase(GeometryFault.PlayerSizeNaN)]
        [TestCase(GeometryFault.PlayerSizeInfinity)]
        [TestCase(GeometryFault.PlayerMissingBody)]
        [TestCase(GeometryFault.WalkerSizeNaN)]
        [TestCase(GeometryFault.WalkerSizeInfinity)]
        [TestCase(GeometryFault.WalkerLinearVelocityNaN)]
        [TestCase(GeometryFault.WalkerAngularVelocity)]
        public void AC_COM_003_UnitySanitizedOrPreventedAuthoringIsNotAThreatDriverRejection(GeometryFault fault)
        {
            var rig = CreateRig(new Vector2(20f, 0f), includeThreat: false);
            ExpectUnityBoundaryRejection(fault);
            ApplyGeometryFault(rig, fault);
            var playerCapsule = rig.Player.GetComponent<CapsuleCollider2D>();
            var walkerBody = rig.WalkerBody;
            if (fault == GeometryFault.PlayerSizeNaN || fault == GeometryFault.PlayerSizeInfinity)
                Assert.That(FinitePositive(playerCapsule.size.x) && FinitePositive(playerCapsule.size.y), Is.True);
            if (fault == GeometryFault.PlayerMissingBody) Assert.That(rig.Player.GetComponent<Rigidbody2D>(), Is.Not.Null);
            if (fault == GeometryFault.WalkerSizeNaN || fault == GeometryFault.WalkerSizeInfinity)
            {
                var size = walkerBody.GetComponent<BoxCollider2D>().size;
                Assert.That(FinitePositive(size.x) && FinitePositive(size.y), Is.True);
            }
            if (fault == GeometryFault.WalkerLinearVelocityNaN) Assert.That(walkerBody.linearVelocity, Is.EqualTo(Vector2.zero));
            if (fault == GeometryFault.WalkerAngularVelocity) Assert.That(walkerBody.angularVelocity, Is.Zero);
            var value = Track(new GameObject("SanitizedThreat")); value.SetActive(false);
            var threat = value.AddComponent<RegularEnemyThreatSimulationDriver>();
            Assert.DoesNotThrow(() => threat.ConfigureForTests(rig.Player, rig.Combat, rig.Behavior, rig.Locomotion));
        }

        // REQ-COM-004; AC-COM-003. Reset failure retains the current owners; success adopts the exact staged M2A owner and clears M1 dedupe.
        [Test]
        public void AC_COM_003_ResetFailurePreservesThenSuccessAdoptsStagedOwnerAndClearsDedupe()
        {
            var rig = CreateRig(new Vector2(20f, 0f));
            var seedTick = new SimulationTick(0);
            rig.Combat.SubmitForTests(new CombatSimulationInput(seedTick, null, null, null,
                new[] { new DamageRequest("reset-dedupe", "hazard", "walker", 1, DamageKind.HeavyImpact, seedTick) }, false));
            AdvanceThroughThreat(rig);
            Assert.That(GetPrivate<HashSet<string>>(GetPrivate<CombatSession>(rig.Combat, "_combatSession"), "_consumedRequestIds").Count, Is.EqualTo(1));
            var priorAttack = GetPrivate<BasicAttackSession>(rig.Combat, "_attackSession");
            var resetTick = new SimulationTick(1);
            rig.Combat.SubmitForTests(new CombatSimulationInput(resetTick, null, null, null, Array.Empty<DamageRequest>(), true));
            SetPrivate(rig.Combat, "_attackSession", new BasicAttackSession(new SimulationTick(2)));
            rig.Transfer.AdvanceTransferPhaseForTests();
            Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.EqualTo(1));
            SetPrivate(rig.Combat, "_attackSession", priorAttack);
            rig.Combat.SubmitForTests(new CombatSimulationInput(resetTick, null, null, null, Array.Empty<DamageRequest>(), true));

            var reset = rig.Combat.AdvanceCombatPhaseForTests();
            var adopted = GetPrivate<BasicAttackSession>(rig.Combat, "_attackSession");
            Assert.That(adopted, Is.Not.SameAs(priorAttack));
            Assert.That(adopted.NextExpectedTick, Is.EqualTo(new SimulationTick(2)));
            Assert.That(reset.Attack.Snapshot.HighlightedTargetId, Is.Empty);
            Assert.That(reset.Attack.Snapshot.CooldownEndTick, Is.EqualTo(-1));
            Assert.That(reset.Attack.Snapshot.ConsumedSequenceCount, Is.Zero);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
            Assert.That(GetPrivate<HashSet<string>>(GetPrivate<CombatSession>(rig.Combat, "_combatSession"), "_consumedRequestIds").Count, Is.Zero);
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-003. The largest admissible source publishes and is actually consumed at t+1.
        [Test]
        public void AC_COM_003_LargestDeliverableSourceReachesExactNextCombatTick()
        {
            var source = int.MaxValue - 46;
            var rig = CreateRig(Vector2.zero, source);
            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.AdvanceCombatPhaseForTests();
            PublishSyntheticThreatSources(rig, source);
            rig.Player.AdvanceOneFixedTickForTests();
            var carried = rig.Threat.AdvanceThreatPhaseForTests();
            Assert.That(carried.Tick, Is.EqualTo(new SimulationTick(source)));
            Assert.That(carried.Snapshot.NextExpectedTick, Is.EqualTo(new SimulationTick(source + 1)));
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.EqualTo(1));
            rig.Transfer.AdvanceTransferPhaseForTests();
            var delivered = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(delivered.Tick, Is.EqualTo(new SimulationTick(int.MaxValue - 45)));
            Assert.That(delivered.CanonicalRequestTrace.Count, Is.Zero);
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.Zero);
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-003. Exhausted source/delivery and caller-array mutation cannot alter an accepted candidate.
        [Test]
        public void AC_COM_003_ExhaustedAndCallerMutatedCandidatesAreAtomic()
        {
            var rejected = CreateRig(new Vector2(20f, 0f), int.MaxValue - 45, includeThreat: false);
            rejected.Combat.PrepareThreatDeliveryRegistration();
            Assert.Throws<InvalidOperationException>(() => rejected.Combat.RegisterThreatDeliveryLane(new SimulationTick(int.MaxValue - 45)));
            Assert.That(rejected.Combat.ThreatDeliveryCountForTests, Is.Zero);

            var rig = CreateRig(new Vector2(20f, 0f));
            AdvanceToPostMovementWithoutThreat(rig);
            var requests = new[] { WalkerRequest(new SimulationTick(0), new SimulationTick(1)) };
            var candidate = rig.Combat.PreflightThreatDelivery(new SimulationTick(0), requests);
            requests[0] = LineRequest(new SimulationTick(0), new SimulationTick(1), 0);
            rig.Combat.CommitPreflightedThreatDelivery(candidate);
            Assert.That(candidate.GetRequest(0).Kind, Is.EqualTo(DamageKind.EnemyContact));
            Assert.That(rig.Combat.ThreatDeliveryCountForTests, Is.EqualTo(1));
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-001, AC-COM-003. All three producer lanes merge once; simultaneous player/enemy damage remains independent and M1 orders it.
        [Test]
        public void AC_COM_001_ExternalWalkerLineAndBasicAttackMergeExactlyOnce()
        {
            var rig = CreateRig(new Vector2(0.25f, 0f));
            rig.SurveyorBody.position = Vector2.zero;
            rig.SurveyorBody.transform.position = Vector2.zero;
            Physics2D.SyncTransforms();
            AdvanceThroughThreat(rig);
            var queue = GetPrivate<SortedDictionary<int, RegularEnemyThreatDeliveryBatch>>(rig.Combat, "_threatDeliveries");
            queue.Clear();
            var source = new SimulationTick(0); var delivery = new SimulationTick(1);
            var deliveryCandidate = rig.Combat.PreflightThreatDelivery(source,
                new[] { WalkerRequest(source, delivery), LineRequest(source, delivery, 0) });
            rig.Combat.CommitPreflightedThreatDelivery(deliveryCandidate);
            var camera = Camera(0); var aim = Aim(1, 50, 960);
            rig.Transfer.SubmitForTests(new TransferSimulationInput(delivery, camera, aim, null, Array.Empty<TransferTargetRemoved>(), null));
            rig.Combat.SubmitForTests(new CombatSimulationInput(delivery, camera, aim,
                new BasicAttackPressed(50, 50, delivery),
                new[] { new DamageRequest("external", "hazard", "surveyor", 1, DamageKind.HeavyImpact, delivery) }, false));

            rig.Transfer.AdvanceTransferPhaseForTests();
            var outcome = rig.Combat.AdvanceCombatPhaseForTests();

            Assert.That(outcome.Attack.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredHit));
            Assert.That(outcome.CanonicalRequestTrace.Count, Is.EqualTo(4));
            Assert.That(outcome.CanonicalRequestTrace[0].Kind, Is.EqualTo(DamageKind.BasicAttack));
            Assert.That(outcome.CanonicalRequestTrace[1].Kind, Is.EqualTo(DamageKind.EnemyContact));
            Assert.That(outcome.CanonicalRequestTrace[2].Kind, Is.EqualTo(DamageKind.EnemyRanged));
            Assert.That(outcome.CanonicalRequestTrace[3].RequestId, Is.EqualTo("external"));
            Assert.That(Snapshot(outcome, "player").CurrentHealth, Is.EqualTo(4));
            Assert.That(Snapshot(outcome, "walker").CurrentHealth, Is.EqualTo(9));
            Assert.That(Snapshot(outcome, "surveyor").CurrentHealth, Is.EqualTo(2));
            Assert.That(outcome.DamageResults[1].Result, Is.EqualTo(DamageResultCode.Applied));
            Assert.That(outcome.DamageResults[2].Result, Is.EqualTo(DamageResultCode.Invulnerable));
        }

        // REQ-COM-001; AC-COM-003. M3D never owns invulnerability: age 44 rejects and age 45 applies under M1's exact player window.
        [Test]
        public void AC_COM_003_PlayerInvulnerabilityAgesFortyFourAndFortyFiveRemainM1Owned()
        {
            var rig = CreateRig(new Vector2(20f, 0f));
            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.AdvanceCombatPhaseForTests();
            rig.Player.AdvanceOneFixedTickForTests();
            CommitManualDelivery(rig, 0, true);
            CombatSimulationOutcome outcome = default;
            for (var tick = 1; tick <= 46; tick++)
            {
                rig.Transfer.AdvanceTransferPhaseForTests();
                outcome = rig.Combat.AdvanceCombatPhaseForTests();
                rig.Player.AdvanceOneFixedTickForTests();
                if (tick < 46) CommitManualDelivery(rig, tick, tick == 44 || tick == 45);
                if (tick == 45) Assert.That(outcome.DamageResults[0].Result, Is.EqualTo(DamageResultCode.Invulnerable));
                if (tick == 46) Assert.That(outcome.DamageResults[0].Result, Is.EqualTo(DamageResultCode.Applied));
            }
            Assert.That(Snapshot(outcome, "player").CurrentHealth, Is.EqualTo(3));
        }

        // REQ-COM-001, REQ-COM-002; AC-COM-001. A dash crossing is absent from source Combat, then its exact contact ID resolves only at t+1.
        [Test]
        public void AC_COM_001_StationaryThenDashCrossingContactDeliversOnlyNextTick()
        {
            var rig = CreateRig(new Vector2(2f, 0f));
            DamageRequest? pending = null;
            for (var source = 0; source < 50 && !pending.HasValue; source++)
            {
                var combat = AdvanceFullSourceTick(rig);
                var batch = CurrentDeliveryBatch(rig);
                for (var index = 0; index < batch.RequestCount; index++)
                {
                    if (batch.GetRequest(index).Kind != DamageKind.EnemyContact) continue;
                    pending = batch.GetRequest(index);
                    Assert.That(ContainsRequest(combat, pending.Value.RequestId), Is.False, "source Combat must not see +100 contact");
                }
            }
            Assert.That(pending.HasValue, Is.True, "the authored telegraph/dash crossing must eventually touch");
            rig.Transfer.AdvanceTransferPhaseForTests();
            var delivered = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(ContainsRequest(delivered, pending.Value.RequestId), Is.True);
            Assert.That(pending.Value.Tick, Is.EqualTo(delivered.Tick));
        }

        // REQ-COM-001, REQ-COM-004, REQ-COM-005; AC-COM-001, AC-COM-003. Spawned line touches for its exact lifetime and survives Surveyor death until expiry.
        [Test]
        public void AC_COM_001_LineSpawnTouchExpiryAndPostSurveyorDeathPersistenceAreExact()
        {
            var rig = CreateRig(new Vector2(20f, 0f));
            var firstSource = -1; var lastSource = -1; var killed = false; var persistedAfterDeath = false;
            string pendingLineId = null; var deliveredGeneratedLine = false;
            for (var source = 0; source < 220; source++)
            {
                if (killed && source == firstSource + 2)
                {
                    var tick = rig.Player.NextExpectedTick;
                    rig.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null,
                        new[] { new DamageRequest("kill-surveyor", "hazard", "surveyor", 6, DamageKind.HeavyImpact, tick) }, false));
                }
                var combat = AdvanceFullSourceTick(rig);
                if (pendingLineId != null && !deliveredGeneratedLine)
                {
                    Assert.That(ContainsRequest(combat, pendingLineId), Is.True, "M3D1-generated line must enter exact next Combat");
                    deliveredGeneratedLine = true;
                }
                var batch = CurrentDeliveryBatch(rig);
                var hasLine = false;
                for (var index = 0; index < batch.RequestCount; index++) hasLine |= batch.GetRequest(index).Kind == DamageKind.EnemyRanged;
                if (hasLine)
                {
                    if (firstSource < 0)
                    {
                        firstSource = source;
                        for (var index = 0; index < batch.RequestCount; index++)
                        {
                            var request = batch.GetRequest(index);
                            if (request.Kind != DamageKind.EnemyRanged) continue;
                            pendingLineId = request.RequestId;
                            Assert.That(request.RequestId, Is.EqualTo("enemy.line/surveyor/" +
                                rig.Threat.LatestCarriedViewForTests.Value.Snapshot.LineShotOrdinal.ToString("D19", CultureInfo.InvariantCulture) + "/" +
                                source.ToString("D10", CultureInfo.InvariantCulture)));
                        }
                    }
                    lastSource = source;
                    if (killed && Snapshot(rig.Combat.LatestOutcomeForTests, "surveyor").IsDead) persistedAfterDeath = true;
                }
                if (firstSource >= 0 && !killed) killed = true;
                if (firstSource >= 0 && source > firstSource + 35) break;
            }
            Assert.That(firstSource, Is.GreaterThanOrEqualTo(0));
            Assert.That(lastSource, Is.EqualTo(firstSource + 29));
            Assert.That(deliveredGeneratedLine, Is.True);
            Assert.That(persistedAfterDeath, Is.True);
            Assert.That(CurrentDeliveryBatch(rig).RequestCount, Is.Zero, "post-expiry source publishes an exact empty batch");
        }

        // REQ-COM-001, REQ-COM-005; AC-COM-003. Source-tick death suppresses the owned next publication while an already queued prior batch still resolves through M1.
        [TestCase(RoleDeathFault.Player)]
        [TestCase(RoleDeathFault.Walker)]
        [TestCase(RoleDeathFault.Surveyor)]
        public void AC_COM_003_RoleDeathSuppressesNewThreatButPriorBatchStillResolves(RoleDeathFault fault)
        {
            if (fault == RoleDeathFault.Surveyor)
            {
                var alive = CreateRig(new Vector2(20f, 0f));
                var spawnSource = FindFirstGeneratedLineSource(alive, 220);
                Assert.That(spawnSource, Is.GreaterThan(0), "alive Surveyor control must generate a new line");
                DestroyTrackedObjects();

                var rig = CreateRig(new Vector2(20f, 0f));
                for (var source = 0; source < spawnSource; source++) AdvanceFullSourceTick(rig);
                var delivery = new SimulationTick(spawnSource);
                var priorSource = new SimulationTick(spawnSource - 1);
                var prior = LineRequest(priorSource, delivery, 99);
                GetPrivate<SortedDictionary<int, RegularEnemyThreatDeliveryBatch>>(rig.Combat, "_threatDeliveries").Clear();
                var priorCandidate = rig.Combat.PreflightThreatDelivery(priorSource, new[] { prior });
                rig.Combat.CommitPreflightedThreatDelivery(priorCandidate);
                rig.Combat.SubmitForTests(new CombatSimulationInput(delivery, null, null, null,
                    new[] { new DamageRequest("aaa-kill-surveyor", "hazard", "surveyor", 6, DamageKind.HeavyImpact, delivery) }, false));
                var resolved = AdvanceFullSourceTick(rig);
                Assert.That(ContainsRequest(resolved, prior.RequestId), Is.True, "already queued line still resolves");
                Assert.That(HasKind(CurrentDeliveryBatch(rig), DamageKind.EnemyRanged), Is.False, "dead Surveyor suppresses a new spawn");
                Assert.That(rig.Threat.LatestCarriedViewForTests.Value.Snapshot.HasActiveLine, Is.False,
                    "this path killed before spawn; active-line persistence is covered separately");
                return;
            }

            var aliveContact = CreateRig(new Vector2(0.25f, 0f));
            AdvanceFullSourceTick(aliveContact);
            Assert.That(HasKind(CurrentDeliveryBatch(aliveContact), DamageKind.EnemyContact), Is.True,
                "alive player/walker control must generate contact");
            DestroyTrackedObjects();

            var deathRig = CreateRig(new Vector2(0.25f, 0f));
            AdvanceThroughThreat(deathRig);
            var priorContact = CurrentDeliveryBatch(deathRig).GetRequest(0);
            var target = fault == RoleDeathFault.Player ? "player" : "walker";
            var amount = fault == RoleDeathFault.Player ? 5 : 9;
            var tick = new SimulationTick(1);
            deathRig.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null,
                new[] { new DamageRequest("aaa-kill-" + target, "hazard", target, amount, DamageKind.HeavyImpact, tick) }, false));
            var deathResolved = AdvanceFullSourceTick(deathRig);
            Assert.That(ContainsRequest(deathResolved, priorContact.RequestId), Is.True, "already queued contact still resolves");
            Assert.That(HasKind(CurrentDeliveryBatch(deathRig), DamageKind.EnemyContact), Is.False,
                fault == RoleDeathFault.Player ? "dead player suppresses all new threats" : "dead walker suppresses new contact");
        }

        // REQ-COM-001, REQ-COM-002, REQ-COM-004; AC-COM-003. Render grouping and an actual physics step cannot perturb value-only logical/delivery traces.
        [Test]
        public void AC_COM_003_ThirtySixtyOneFortyFourAndPhysicsStepThreatTracesMatch()
        {
            var atThirty = RunGroupedTrace(30, false);
            DestroyTrackedObjects();
            var atSixty = RunGroupedTrace(60, false);
            DestroyTrackedObjects();
            var atOneFortyFour = RunGroupedTrace(144, false);
            DestroyTrackedObjects();
            var withPhysicsStep = RunGroupedTrace(60, true);
            Assert.That(atThirty, Is.EqualTo(atSixty));
            Assert.That(atSixty, Is.EqualTo(atOneFortyFour));
            Assert.That(withPhysicsStep, Is.EqualTo(atSixty));
        }

        private static CarriedBehaviorView RetickBehavior(CarriedBehaviorView value, int tick)
        {
            var snapshot = value.Snapshot;
            var changed = new RegularEnemyBehaviorPairSnapshot(new SimulationTick(tick), snapshot.Walker, snapshot.Surveyor,
                snapshot.WalkerRecoveryIntent, snapshot.SurveyorShotIntent, new SimulationTick(checked(tick + 1)));
            return new CarriedBehaviorView(new SimulationTick(tick), changed);
        }

        private static CarriedEnemyMotionView RetickMotion(CarriedEnemyMotionView value, int tick)
        {
            var snapshot = value.Snapshot;
            var changed = new RegularEnemyMotionPairSnapshot(new SimulationTick(tick), snapshot.Walker, snapshot.Surveyor,
                new SimulationTick(checked(tick + 1)));
            return new CarriedEnemyMotionView(new SimulationTick(tick), changed);
        }

        private void ApplyGeometryFault(Rig rig, GeometryFault fault)
        {
            var playerCapsule = rig.Player.GetComponent<CapsuleCollider2D>();
            var playerBody = rig.Player.GetComponent<Rigidbody2D>();
            var walkerBox = rig.WalkerBody.GetComponent<BoxCollider2D>();
            switch (fault)
            {
                case GeometryFault.PlayerInactive: rig.Player.gameObject.SetActive(false); return;
                case GeometryFault.PlayerTrigger: playerCapsule.isTrigger = true; return;
                case GeometryFault.PlayerDisabled: playerCapsule.enabled = false; return;
                case GeometryFault.PlayerHorizontal: playerCapsule.direction = CapsuleDirection2D.Horizontal; return;
                case GeometryFault.PlayerOffset: playerCapsule.offset = Vector2.right; return;
                case GeometryFault.PlayerSizeZero: playerCapsule.size = Vector2.zero; return;
                case GeometryFault.PlayerSizeNaN: playerCapsule.size = new Vector2(float.NaN, 1f); return;
                case GeometryFault.PlayerSizeInfinity: playerCapsule.size = new Vector2(float.PositiveInfinity, 1f); return;
                case GeometryFault.PlayerSizeSubQuantum: playerCapsule.size = new Vector2(0.0001f, 1f); return;
                case GeometryFault.PlayerScale: rig.Player.transform.localScale = new Vector3(2f, 1f, 1f); return;
                case GeometryFault.PlayerScaleY: rig.Player.transform.localScale = new Vector3(1f, 2f, 1f); return;
                case GeometryFault.PlayerRotation: rig.Player.transform.eulerAngles = new Vector3(0f, 0f, 1f); return;
                case GeometryFault.PlayerMissingBody: UnityEngine.Object.DestroyImmediate(playerBody); return;
                case GeometryFault.PlayerDynamic: playerBody.bodyType = RigidbodyType2D.Dynamic; return;
                case GeometryFault.PlayerUnsimulated: playerBody.simulated = false; return;
                case GeometryFault.PlayerExtraSolid: rig.Player.gameObject.AddComponent<BoxCollider2D>(); return;
                case GeometryFault.WalkerInactive: rig.WalkerBody.gameObject.SetActive(false); return;
                case GeometryFault.WalkerTrigger: walkerBox.isTrigger = true; return;
                case GeometryFault.WalkerDisabled: walkerBox.enabled = false; return;
                case GeometryFault.WalkerOffset: walkerBox.offset = Vector2.right; return;
                case GeometryFault.WalkerSizeZero: walkerBox.size = Vector2.zero; return;
                case GeometryFault.WalkerSizeNaN: walkerBox.size = new Vector2(float.NaN, 1f); return;
                case GeometryFault.WalkerSizeInfinity: walkerBox.size = new Vector2(float.PositiveInfinity, 1f); return;
                case GeometryFault.WalkerSizeSubQuantum: walkerBox.size = new Vector2(0.0001f, 1f); return;
                case GeometryFault.WalkerScale: rig.WalkerBody.transform.localScale = new Vector3(2f, 1f, 1f); return;
                case GeometryFault.WalkerScaleY: rig.WalkerBody.transform.localScale = new Vector3(1f, 2f, 1f); return;
                case GeometryFault.WalkerRotation: rig.WalkerBody.transform.eulerAngles = new Vector3(0f, 0f, 1f); return;
                case GeometryFault.WalkerMissingBody: UnityEngine.Object.DestroyImmediate(rig.WalkerBody); return;
                case GeometryFault.WalkerDynamic: rig.WalkerBody.bodyType = RigidbodyType2D.Dynamic; return;
                case GeometryFault.WalkerUnsimulated: rig.WalkerBody.simulated = false; return;
                case GeometryFault.WalkerConstraint: rig.WalkerBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; return;
                case GeometryFault.WalkerInterpolation: rig.WalkerBody.interpolation = RigidbodyInterpolation2D.Interpolate; return;
                case GeometryFault.WalkerFullContacts: rig.WalkerBody.useFullKinematicContacts = true; return;
                case GeometryFault.WalkerLinearVelocity: rig.WalkerBody.linearVelocity = Vector2.right; return;
                case GeometryFault.WalkerLinearVelocityNaN: rig.WalkerBody.linearVelocity = new Vector2(float.NaN, 0f); return;
                case GeometryFault.WalkerAngularVelocity: rig.WalkerBody.angularVelocity = 1f; return;
                case GeometryFault.WalkerBodyRotation: rig.WalkerBody.rotation = 1f; return;
                case GeometryFault.WalkerEdgeRadius: walkerBox.edgeRadius = 0.1f; return;
                case GeometryFault.WalkerExtraSolid: rig.WalkerBody.gameObject.AddComponent<CircleCollider2D>(); return;
            }

            var roster = rig.Combat.FrozenTargetsForThreatBridge;
            var changed = new FrozenCombatTarget[roster.Count];
            for (var index = 0; index < roster.Count; index++) changed[index] = roster[index];
            for (var index = 0; index < changed.Length; index++)
            {
                var value = changed[index];
                if (fault == GeometryFault.PlayerRootCrosswire && value.TargetId == "player") changed[index] = CloneFrozen(value, root: rig.WalkerBody.transform);
                if (fault == GeometryFault.PlayerColliderCrosswire && value.TargetId == "player") changed[index] = CloneFrozen(value, collider: walkerBox);
                if (fault == GeometryFault.WalkerRootCrosswire && value.TargetId == "walker") changed[index] = CloneFrozen(value, root: rig.Player.transform);
                if (fault == GeometryFault.WalkerColliderCrosswire && value.TargetId == "walker") changed[index] = CloneFrozen(value, collider: playerCapsule);
                if (fault == GeometryFault.PlayerHealth && value.TargetId == "player") changed[index] = CloneFrozen(value, maxHealth: 6);
                if (fault == GeometryFault.PlayerInvulnerability && value.TargetId == "player") changed[index] = CloneFrozen(value, invulnerability: 44);
                if (fault == GeometryFault.PlayerAttackable && value.TargetId == "player") changed[index] = CloneFrozen(value, attackable: true);
                if (fault == GeometryFault.PlayerTransfer && value.TargetId == "player") changed[index] = CloneFrozen(value, transfer: true);
                if (fault == GeometryFault.WalkerHealth && value.TargetId == "walker") changed[index] = CloneFrozen(value, maxHealth: 8);
                if (fault == GeometryFault.WalkerInvulnerability && value.TargetId == "walker") changed[index] = CloneFrozen(value, invulnerability: 1);
                if (fault == GeometryFault.WalkerAttackable && value.TargetId == "walker") changed[index] = CloneFrozen(value, attackable: false);
                if (fault == GeometryFault.WalkerTransfer && value.TargetId == "walker") changed[index] = CloneFrozen(value, transfer: false);
                if (fault == GeometryFault.SurveyorHealth && value.TargetId == "surveyor") changed[index] = CloneFrozen(value, maxHealth: 5);
                if (fault == GeometryFault.SurveyorInvulnerability && value.TargetId == "surveyor") changed[index] = CloneFrozen(value, invulnerability: 1);
                if (fault == GeometryFault.SurveyorAttackable && value.TargetId == "surveyor") changed[index] = CloneFrozen(value, attackable: false);
                if (fault == GeometryFault.SurveyorTransfer && value.TargetId == "surveyor") changed[index] = CloneFrozen(value, transfer: false);
                if (fault == GeometryFault.SurveyorColliderMissing && value.TargetId == "surveyor")
                    changed[index] = new FrozenCombatTarget(value.TargetId, value.MaxHealth, value.InvulnerabilityTicks, value.CanReceivePlayerBasicAttack,
                        value.LocalAimPointXQ1000, value.LocalAimPointYQ1000, value.LocalAimShapeCenterXQ1000, value.LocalAimShapeCenterYQ1000,
                        value.LocalAimShapeHalfExtentXQ1000, value.LocalAimShapeHalfExtentYQ1000, null, value.TargetPoseRoot, value.HasMatchingTransferTarget);
                if (fault == GeometryFault.SurveyorRootMissing && value.TargetId == "surveyor")
                    changed[index] = new FrozenCombatTarget(value.TargetId, value.MaxHealth, value.InvulnerabilityTicks, value.CanReceivePlayerBasicAttack,
                        value.LocalAimPointXQ1000, value.LocalAimPointYQ1000, value.LocalAimShapeCenterXQ1000, value.LocalAimShapeCenterYQ1000,
                        value.LocalAimShapeHalfExtentXQ1000, value.LocalAimShapeHalfExtentYQ1000, value.TargetCollider, null, value.HasMatchingTransferTarget);
            }
            SetPrivate(rig.Combat, "_frozenTargets", changed);
        }

        private static FrozenCombatTarget CloneFrozen(FrozenCombatTarget value, int? maxHealth = null,
            int? invulnerability = null, bool? attackable = null, Collider2D collider = null, Transform root = null, bool? transfer = null) =>
            new FrozenCombatTarget(value.TargetId, maxHealth ?? value.MaxHealth, invulnerability ?? value.InvulnerabilityTicks,
                attackable ?? value.CanReceivePlayerBasicAttack, value.LocalAimPointXQ1000, value.LocalAimPointYQ1000,
                value.LocalAimShapeCenterXQ1000, value.LocalAimShapeCenterYQ1000, value.LocalAimShapeHalfExtentXQ1000,
                value.LocalAimShapeHalfExtentYQ1000, collider == null ? value.TargetCollider : collider,
                root == null ? value.TargetPoseRoot : root, transfer ?? value.HasMatchingTransferTarget);

        private Rig CreateRig(Vector2 walkerPosition, int firstTick = 0, bool includeThreat = true)
        {
            var playerObject = Track(new GameObject("player"));
            var playerBody = playerObject.AddComponent<Rigidbody2D>();
            playerBody.bodyType = RigidbodyType2D.Kinematic;
            var playerCollider = playerObject.AddComponent<CapsuleCollider2D>();
            playerCollider.direction = CapsuleDirection2D.Vertical;
            var player = playerObject.AddComponent<PlayerMovementController>();
            if (firstTick != 0)
                typeof(PlayerMovementController).GetField("_motor", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(player, new PlayerMovementMotor(new SimulationTick(firstTick)));
            var playerCombat = playerObject.AddComponent<CombatTarget>();
            playerCombat.ConfigureForTests("player", 5, 45, false, 0, 0, 0, 0, 100, 100, playerCollider, playerObject.transform);

            var walker = CreateEnemy("walker", walkerPosition, 9);
            var surveyor = CreateEnemy("surveyor", new Vector2(10f, 0f), 6);
            var transferRegistry = Track(new GameObject("TransferRegistry")).AddComponent<TransferTargetRegistry>();
            transferRegistry.ConfigureForTests(surveyor.Transfer, walker.Transfer);
            var transfer = Track(new GameObject("TransferDriver")).AddComponent<TransferSimulationDriver>();
            transfer.ConfigureForTests(player, transferRegistry);
            transfer.InitializeForTests();
            var combatRegistry = Track(new GameObject("CombatRegistry")).AddComponent<CombatTargetRegistry>();
            combatRegistry.ConfigureForTests(playerCombat, surveyor.Combat, walker.Combat);
            var combat = Track(new GameObject("CombatDriver")).AddComponent<CombatSimulationDriver>();
            var reaction = Track(new GameObject("ReactionDriver")).AddComponent<RegularEnemyReactionSimulationDriver>();
            reaction.ConfigureForTests(player, transfer, combat);
            combat.ConfigureForTests(player, combatRegistry, transfer, reaction);
            var behavior = Track(new GameObject("BehaviorDriver")).AddComponent<RegularEnemyBehaviorSimulationDriver>();
            behavior.ConfigureForTests(player, combat, reaction);
            var locomotion = Track(new GameObject("LocomotionDriver")).AddComponent<RegularEnemyLocomotionSimulationDriver>();
            locomotion.ConfigureForTests(player, combat, reaction, behavior);

            RegularEnemyThreatSimulationDriver threat = null;
            if (includeThreat)
            {
                var threatObject = Track(new GameObject("ThreatDriver"));
                threatObject.SetActive(false);
                threat = threatObject.AddComponent<RegularEnemyThreatSimulationDriver>();
                threat.ConfigureForTests(player, combat, behavior, locomotion);
            }
            else combat.InitializeForTests();
            return new Rig(player, transfer, combat, reaction, behavior, locomotion, threat, walker.Body, surveyor.Body);
        }

        private Enemy CreateEnemy(string id, Vector2 position, int health)
        {
            var value = Track(new GameObject(id));
            value.transform.position = position;
            var body = value.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.interpolation = RigidbodyInterpolation2D.None;
            body.useFullKinematicContacts = false;
            var box = value.AddComponent<BoxCollider2D>();
            var sink = value.AddComponent<TransferSandboxOwnerStub>();
            sink.ConfigureForTests(id, "Target.Enemy.Heavy.v1");
            var transfer = value.AddComponent<TransferTarget>();
            transfer.ConfigureForTests(id, TransferTargetKind.Enemy, 0, 0, 0, 0, 100, 100, "base", box, value.transform, sink);
            var combat = value.AddComponent<CombatTarget>();
            combat.ConfigureForTests(id, health, 0, true, 0, 0, 0, 0, 100, 100, box, value.transform, transfer);
            return new Enemy(body, transfer, combat);
        }

        private static void AdvanceThroughThreat(Rig rig)
        {
            AdvanceToPostMovementWithoutThreat(rig);
            rig.Threat.AdvanceThreatPhaseForTests();
        }

        private static CombatSimulationOutcome AdvanceFullSourceTick(Rig rig, bool simulateBeforeThreat = false)
        {
            rig.Transfer.AdvanceTransferPhaseForTests();
            var combat = rig.Combat.AdvanceCombatPhaseForTests();
            rig.Behavior.AdvanceBehaviorPhaseForTests();
            var motion = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            ApplyEnemyBodies(rig, motion.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();
            if (simulateBeforeThreat) Physics2D.Simulate(1f / 60f);
            rig.Threat.AdvanceThreatPhaseForTests();
            return combat;
        }

        private static RegularEnemyThreatDeliveryBatch CurrentDeliveryBatch(Rig rig)
        {
            var queue = GetPrivate<SortedDictionary<int, RegularEnemyThreatDeliveryBatch>>(rig.Combat, "_threatDeliveries");
            Assert.That(queue.Count, Is.EqualTo(1));
            foreach (var pair in queue) return pair.Value;
            throw new AssertionException("Missing exact threat batch.");
        }

        private static bool HasKind(RegularEnemyThreatDeliveryBatch batch, DamageKind kind)
        {
            for (var index = 0; index < batch.RequestCount; index++) if (batch.GetRequest(index).Kind == kind) return true;
            return false;
        }

        private static int FindFirstGeneratedLineSource(Rig rig, int exclusiveLimit)
        {
            for (var source = 0; source < exclusiveLimit; source++)
            {
                AdvanceFullSourceTick(rig);
                if (HasKind(CurrentDeliveryBatch(rig), DamageKind.EnemyRanged)) return source;
            }
            return -1;
        }

        private static void PublishSyntheticThreatSources(Rig rig, int sourceTick)
        {
            var tick = new SimulationTick(sourceTick); var next = new SimulationTick(checked(sourceTick + 1));
            var walker = new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Approach, -1, true,
                RegularEnemyMovementIntent.Horizontal, 1, false, false, 0, 0);
            var surveyor = new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Suppressed, -1, true,
                RegularEnemyMovementIntent.None, 0, false, false, 0, 0);
            var behavior = new RegularEnemyBehaviorPairSnapshot(tick, walker, surveyor, null, null, next);
            SetPrivate(rig.Behavior, "_carried", (CarriedBehaviorView?)new CarriedBehaviorView(tick, behavior));
            var walkerMotion = new RegularEnemyMotionSnapshot("walker", new PositionQ4096(81920, 0), new PositionQ4096(0, 0),
                0, 0, 0, true, false);
            var surveyorMotion = new RegularEnemyMotionSnapshot("surveyor", new PositionQ4096(40960, 0), new PositionQ4096(0, 0),
                0, 0, 0, true, false);
            var motion = new RegularEnemyMotionPairSnapshot(tick, walkerMotion, surveyorMotion, next);
            SetPrivate(rig.Locomotion, "_carried", (CarriedEnemyMotionView?)new CarriedEnemyMotionView(tick, motion));
        }

        private static bool ContainsRequest(CombatSimulationOutcome outcome, string requestId)
        {
            for (var index = 0; index < outcome.CanonicalRequestTrace.Count; index++)
                if (outcome.CanonicalRequestTrace[index].RequestId == requestId) return true;
            return false;
        }

        private static bool FinitePositive(float value) => !float.IsNaN(value) && !float.IsInfinity(value) && value > 0f;

        private static void ExpectUnityBoundaryRejection(GeometryFault fault)
        {
            string pattern = null;
            if (fault == GeometryFault.PlayerSizeNaN || fault == GeometryFault.PlayerSizeInfinity)
                pattern = "CapsuleCollider2D\\.size assign attempt.*not valid";
            else if (fault == GeometryFault.PlayerMissingBody)
                pattern = "Can't remove Rigidbody2D because PlayerMovementController \\(Script\\) depends on it";
            else if (fault == GeometryFault.WalkerSizeNaN || fault == GeometryFault.WalkerSizeInfinity)
                pattern = "BoxCollider2D\\.size assign attempt.*not valid";
            else if (fault == GeometryFault.WalkerLinearVelocityNaN)
                pattern = "Rigidbody2D\\.linearVelocity assign attempt.*not valid";
            if (pattern != null) LogAssert.Expect(LogType.Error, new Regex(pattern, RegexOptions.CultureInvariant));
        }

        private string RunGroupedTrace(int renderRate, bool simulateBeforeThreat)
        {
            var rig = CreateRig(new Vector2(3f, 0f));
            var builder = new StringBuilder();
            var fixedTicks = 0; var accumulator = 0;
            while (fixedTicks < 60)
            {
                accumulator = checked(accumulator + 60);
                while (accumulator >= renderRate && fixedTicks < 60)
                {
                    accumulator -= renderRate;
                    var combat = AdvanceFullSourceTick(rig, simulateBeforeThreat);
                    var batch = CurrentDeliveryBatch(rig);
                    var threat = rig.Threat.LatestCarriedViewForTests.Value.Snapshot;
                    builder.Append(combat.Tick.Value).Append(':').Append(Snapshot(combat, "player").CurrentHealth).Append(':')
                        .Append(threat.PlayerDead).Append(':').Append(threat.WalkerDead).Append(':').Append(threat.SurveyorDead).Append(':')
                        .Append(threat.HasPriorCenters).Append(':').Append(threat.ExpectedShotOrdinal).Append(':')
                        .Append(threat.HasActiveLine).Append(':').Append(threat.LineCenterXQ4096).Append(':').Append(threat.LineShotOrdinal).Append(':')
                        .Append(threat.LineSpawnTick).Append(':').Append(threat.LineExclusiveEndTick).Append('[');
                    for (var index = 0; index < batch.RequestCount; index++)
                    {
                        var request = batch.GetRequest(index);
                        builder.Append(request.RequestId).Append(':').Append(request.SourceId).Append(':').Append(request.TargetId)
                            .Append(':').Append(request.Amount).Append(':').Append(request.Kind).Append(':').Append(request.Tick.Value).Append('|');
                    }
                    builder.Append(']');
                    fixedTicks++;
                }
            }
            return builder.ToString();
        }

        private void DestroyTrackedObjects()
        {
            for (var index = _objects.Count - 1; index >= 0; index--) UnityEngine.Object.DestroyImmediate(_objects[index]);
            _objects.Clear();
        }

        private static void AdvanceToPostMovementWithoutThreat(Rig rig)
        {
            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.AdvanceCombatPhaseForTests();
            rig.Behavior.AdvanceBehaviorPhaseForTests();
            var motion = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            ApplyEnemyBodies(rig, motion.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();
        }

        private static void ApplyEnemyBodies(Rig rig, RegularEnemyMotionPairSnapshot snapshot)
        {
            rig.WalkerBody.position = new Vector2(snapshot.Walker.Position.X / 4096f, snapshot.Walker.Position.Y / 4096f);
            rig.SurveyorBody.position = new Vector2(snapshot.Surveyor.Position.X / 4096f, snapshot.Surveyor.Position.Y / 4096f);
        }

        private static DamageRequest WalkerRequest(SimulationTick source, SimulationTick delivery) =>
            new DamageRequest("enemy.contact/walker/" + source.Value.ToString("D10", CultureInfo.InvariantCulture),
                "walker", "player", 1, DamageKind.EnemyContact, delivery);
        private static DamageRequest LineRequest(SimulationTick source, SimulationTick delivery, long ordinal) =>
            new DamageRequest("enemy.line/surveyor/" + ordinal.ToString("D19", CultureInfo.InvariantCulture) + "/" +
                source.Value.ToString("D10", CultureInfo.InvariantCulture), "surveyor", "player", 1, DamageKind.EnemyRanged, delivery);
        private static SimulationCameraPoseSnapshot Camera(int tick) =>
            new SimulationCameraPoseSnapshot(new SimulationTick(tick), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
        private static AimSample Aim(int tick, long sampleId, int screenX) =>
            new AimSample(sampleId, AimSource.MousePointer, screenX, 540, true, 4096, 0,
                new SimulationTick(tick - 1), new SimulationTick(tick));
        private static void CommitManualDelivery(Rig rig, int sourceTick, bool contact)
        {
            var source = new SimulationTick(sourceTick); var delivery = new SimulationTick(checked(sourceTick + 1));
            var requests = contact ? new[] { WalkerRequest(source, delivery) } : Array.Empty<DamageRequest>();
            var candidate = rig.Combat.PreflightThreatDelivery(source, requests);
            rig.Combat.CommitPreflightedThreatDelivery(candidate);
        }
        private static CombatantSnapshot Snapshot(CombatSimulationOutcome outcome, string id)
        {
            for (var index = 0; index < outcome.Combatants.Count; index++) if (outcome.Combatants[index].TargetId == id) return outcome.Combatants[index];
            throw new AssertionException("Missing combatant " + id);
        }
        private static int Order<T>() => ((DefaultExecutionOrder)Attribute.GetCustomAttribute(typeof(T), typeof(DefaultExecutionOrder))).order;
        private static void SetPrivate(object target, string field, object value) =>
            target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
        private static T GetPrivate<T>(object target, string field) =>
            (T)target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
        private GameObject Track(GameObject value) { _objects.Add(value); return value; }

        private readonly struct Enemy
        {
            internal Enemy(Rigidbody2D body, TransferTarget transfer, CombatTarget combat) { Body = body; Transfer = transfer; Combat = combat; }
            internal Rigidbody2D Body { get; }
            internal TransferTarget Transfer { get; }
            internal CombatTarget Combat { get; }
        }

        private readonly struct Rig
        {
            internal Rig(PlayerMovementController player, TransferSimulationDriver transfer, CombatSimulationDriver combat,
                RegularEnemyReactionSimulationDriver reaction,
                RegularEnemyBehaviorSimulationDriver behavior, RegularEnemyLocomotionSimulationDriver locomotion,
                RegularEnemyThreatSimulationDriver threat, Rigidbody2D walkerBody, Rigidbody2D surveyorBody)
            { Player = player; Transfer = transfer; Combat = combat; Reaction = reaction; Behavior = behavior; Locomotion = locomotion; Threat = threat; WalkerBody = walkerBody; SurveyorBody = surveyorBody; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Transfer { get; }
            internal CombatSimulationDriver Combat { get; }
            internal RegularEnemyReactionSimulationDriver Reaction { get; }
            internal RegularEnemyBehaviorSimulationDriver Behavior { get; }
            internal RegularEnemyLocomotionSimulationDriver Locomotion { get; }
            internal RegularEnemyThreatSimulationDriver Threat { get; }
            internal Rigidbody2D WalkerBody { get; }
            internal Rigidbody2D SurveyorBody { get; }
        }
    }
}
