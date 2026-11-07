using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    internal sealed class OrdanBossCombatBridgePlayModeTests
    {
        // AC-COM-002, AC-COM-004: the boss-only bridge remains strictly
        // between existing Transfer (-200) and later fixed phases.
        [Test]
        public void BossCombatAndBridgeFixedOrderIsStrict()
        {
            Assert.That(Order(typeof(OrdanBossCombatSimulationDriver)), Is.EqualTo(-190));
            Assert.That(Order(typeof(OrdanBossSimulationDriver)), Is.EqualTo(-185));
        }

        // AC-COM-002, AC-COM-004: the real three-owner pipeline yields the
        // same fixed-tick trace when a render accumulator groups it at
        // 30, 60, or 144 frames per second.
        [Test]
        public void RealOwnerTickTraceIsIdenticalAtThirtySixtyAndOneFortyFourRenderGrouping()
        {
            Assert.That(TraceOwnersUnderRenderGrouping(30), Is.EqualTo(TraceOwnersUnderRenderGrouping(60)));
            Assert.That(TraceOwnersUnderRenderGrouping(144), Is.EqualTo(TraceOwnersUnderRenderGrouping(60)));
        }

        // AC-COM-003: current Combat death always clears future exposure and
        // leaves the three one-shot M4A handoffs for later consumers only.
        [Test]
        public void DeathClearsForecastAndPublishesOnlyOneShotHandoffs()
        {
            var session = new OrdanBossSession(new SimulationTick(0));
            session.Step(Input(0, 60));
            var output = session.Step(Input(1, 0));
            Assert.That(output.TransferExposureForecast.IsExposed, Is.False);
            Assert.That(output.Handoffs.Count, Is.EqualTo(3));
            Assert.That(session.Step(Input(2, 0)).Handoffs.Count, Is.EqualTo(0));
        }

        // AC-COM-002, AC-COM-003, AC-COM-004, AC-WT-005: this is a manual
        // PlayMode trace of the real three owners: completed -200 Transfer,
        // then -190 Combat, then -185 bridge. It proves bootstrap privacy,
        // empty t+1 continuity, and current-Combat death priority; it does
        // not claim that Unity's render grouping drives fixed simulation.
        [Test]
        public void ManualPhaseTracePromotesBootstrapContinuesEmptyLaneAndHonorsCombatDeath()
        {
            var objects = new List<GameObject>();
            try
            {
                var playerObject = Track(objects, "ordan-test-player");
                var playerBody = playerObject.AddComponent<Rigidbody2D>(); playerBody.bodyType = RigidbodyType2D.Kinematic;
                var playerCollider = playerObject.AddComponent<BoxCollider2D>();
                var player = playerObject.AddComponent<PlayerMovementController>();
                var ordanObject = Track(objects, "ordan-test-body"); var ordanCollider = ordanObject.AddComponent<BoxCollider2D>();
                var ordanTarget = ordanObject.AddComponent<CombatTarget>();
                ordanTarget.ConfigureForTests("ordan", 60, 0, true, 0, 0, 0, 0, 100, 100, ordanCollider, ordanObject.transform);
                var playerTarget = playerObject.AddComponent<CombatTarget>();
                playerTarget.ConfigureForTests("player", 5, 45, false, 0, 0, 0, 0, 100, 100, playerCollider, playerObject.transform);
                var transferRegistry = Track(objects, "ordan-transfer-registry").AddComponent<TransferTargetRegistry>(); transferRegistry.ConfigureForTests();
                var transfer = Track(objects, "ordan-transfer").AddComponent<TransferSimulationDriver>(); transfer.ConfigureForTests(player, transferRegistry); transfer.InitializeForTests();
                var combatRegistry = Track(objects, "ordan-combat-registry").AddComponent<CombatTargetRegistry>(); combatRegistry.ConfigureForTests(ordanTarget, playerTarget);
                var combatObject = Track(objects, "ordan-combat"); combatObject.SetActive(false);
                var combat = combatObject.AddComponent<OrdanBossCombatSimulationDriver>(); combat.ConfigureForTests(player, combatRegistry, transfer, combat); combatObject.SetActive(true);
                var bridgeObject = Track(objects, "ordan-bridge"); bridgeObject.SetActive(false);
                var bridge = bridgeObject.AddComponent<OrdanBossSimulationDriver>(); bridge.ConfigureForTests(player, transfer, combat); bridge.InitializeForTests(); bridgeObject.SetActive(true);
                var trace = new List<string>();
                var transferZero = transfer.AdvanceTransferPhaseForTests(); trace.Add("-200:" + transferZero.Snapshot.Tick.Value);
                var combatZero = combat.AdvanceCombatPhaseForTests(); trace.Add("-190:" + combatZero.Tick.Value);
                Assert.That(combat.HasLatestOutcome, Is.False); Assert.That(combat.HasCurrentOutcome, Is.True);
                // AC-COM-004: first-use staging has not mutated the shared
                // registry's freeze latch before bridge promotion.
                Assert.Throws<InvalidOperationException>(() => { var unused = combatRegistry.FrozenTargets; _ = unused.Count; });
                var bridgeZero = bridge.AdvanceBridgePhaseForTests(); trace.Add("-185:" + bridgeZero.SourceTick.Value);
                Assert.That(combat.HasLatestOutcome, Is.True); Assert.That(combat.LatestOutcome.Tick, Is.EqualTo(new SimulationTick(0)));
                Assert.That(bridge.LatestOutcome.Forecast.TransferTick, Is.EqualTo(new SimulationTick(1)));
                // AC-COM-004: the first promoted empty batch is mandatory at
                // t+1 and keeps the two owner sessions in one exact horizon.
                player.AdvanceOneFixedTickForTests();
                var transferOne = transfer.AdvanceTransferPhaseForTests(); trace.Add("-200:" + transferOne.Snapshot.Tick.Value);
                var combatOne = combat.AdvanceCombatPhaseForTests(); trace.Add("-190:" + combatOne.Tick.Value);
                var bridgeOne = bridge.AdvanceBridgePhaseForTests(); trace.Add("-185:" + bridgeOne.SourceTick.Value);
                Assert.That(combat.LatestOutcome.Tick, Is.EqualTo(new SimulationTick(1)));
                Assert.That(combatOne.CanonicalRequestTrace, Is.Empty);
                Assert.That(bridge.LatestOutcome.Forecast.TransferTick, Is.EqualTo(new SimulationTick(2)));

                // The empty delivery at t=1 was consumed by -190. At t=2,
                // the same real phase order accepts an external lethal M1
                // request; -185 receives the already-clamped death result.
                player.AdvanceOneFixedTickForTests();
                var transferTwo = transfer.AdvanceTransferPhaseForTests(); trace.Add("-200:" + transferTwo.Snapshot.Tick.Value);
                var tickTwo = new SimulationTick(2);
                combat.Submit(new OrdanBossCombatInput(tickTwo, null, null, null, new[]
                {
                    new DamageRequest("ordan.lethal", "test", "ordan", 60, DamageKind.BossPattern, tickTwo)
                }));
                var combatTwo = combat.AdvanceCombatPhaseForTests(); trace.Add("-190:" + combatTwo.Tick.Value);
                var bridgeTwo = bridge.AdvanceBridgePhaseForTests(); trace.Add("-185:" + bridgeTwo.SourceTick.Value);
                Assert.That(combatTwo.Combatants[0].IsDead, Is.True);
                Assert.That(bridgeTwo.Output.Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Defeated));
                Assert.That(bridgeTwo.Forecast.IsExposed, Is.False);
                Assert.That(bridgeTwo.Handoffs.Count, Is.EqualTo(3));
                Assert.That(trace, Is.EqualTo(new[]
                {
                    "-200:0", "-190:0", "-185:0", "-200:1", "-190:1", "-185:1", "-200:2", "-190:2", "-185:2"
                }));
            }
            finally { for (var index = objects.Count - 1; index >= 0; index--) UnityEngine.Object.DestroyImmediate(objects[index]); }
        }

        // AC-COM-003, AC-COM-004, AC-WT-005: M4B1 intentionally does not
        // author a transferable payload body. This test therefore replaces
        // only the immutable -200 proof at the Seizure tick; the Transfer,
        // -190 Combat, -185 bridge, M1 delivery queue, and result-correlation
        // owners execute normally on every tick around it.
        [Test]
        public void PayloadDeliveryUsesExactNextTickAndCanonicalM1Merge()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                for (var value = 0; value <= 192; value++)
                {
                    var tick = rig.Player.NextExpectedTick;
                    Assert.That(tick, Is.EqualTo(new SimulationTick(value)));
                    if (value == 192)
                    {
                        var camera = CameraFor(tick);
                        var aim = new AimSample(700, AimSource.GamepadStick, null, null, null, 4096, 0, camera.CameraPoseTick, tick);
                        rig.Transfer.SubmitForTests(new TransferSimulationInput(tick, camera, aim, null, Array.Empty<TransferTargetRemoved>(), null));
                        rig.Combat.Submit(new OrdanBossCombatInput(tick, camera, aim, new BasicAttackPressed(7, 700, tick), new[]
                        {
                            new DamageRequest("external.zz", "external", "ordan", 1, DamageKind.BossPattern, tick)
                        }));
                    }

                    rig.Transfer.AdvanceTransferPhaseForTests();
                    if (value == 108) PublishRelevantTransferPress(rig.Transfer, tick, "BossWeight-0", 1);
                    var combat = rig.Combat.AdvanceCombatPhaseForTests();
                    var bridge = rig.Bridge.AdvanceBridgePhaseForTests();

                    if (value == 191)
                    {
                        Assert.That(bridge.Output.Intents.Count(intent => intent.Kind == OrdanBossIntentKind.PayloadDamageRequest), Is.EqualTo(1));
                        Assert.That(bridge.Output.Intents.Single(intent => intent.Kind == OrdanBossIntentKind.PayloadDamageRequest).DeliveryTick, Is.EqualTo(new SimulationTick(192)));
                    }
                    if (value == 192)
                    {
                        Assert.That(combat.Attack.DamageRequest.HasValue, Is.True);
                        Assert.That(combat.CanonicalRequestTrace.Select(request => request.RequestId), Is.EqualTo(new[]
                        {
                            "Player.BasicAttack/7", "external.zz", "ordan.payload/0000000000000000000/0000000192"
                        }));
                        var payloadIndex = combat.CanonicalRequestTrace.ToList().FindIndex(request => request.Kind == DamageKind.BossPayloadTransfer);
                        Assert.That(payloadIndex, Is.GreaterThanOrEqualTo(0));
                        Assert.That(combat.DamageResults[payloadIndex].Result, Is.EqualTo(DamageResultCode.Applied));
                        Assert.That(combat.DamageResults[payloadIndex].AppliedAmount, Is.EqualTo(8));
                        Assert.That(bridge.Output.Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Vulnerable));
                    }
                    rig.Player.AdvanceOneFixedTickForTests();
                }
            }
            finally { DestroyAll(objects); }
        }

        // AC-COM-003, AC-COM-004, AC-WT-005: a later -185 rejection after a
        // successful -190 publication is phase-local. It preserves previously
        // adopted M4A/bridge state and never rolls Combat back or adds a lane.
        [Test]
        public void LaterBridgeRejectionPreservesPreviousBridgeAndCombatPublication()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                rig.Transfer.AdvanceTransferPhaseForTests(); rig.Combat.AdvanceCombatPhaseForTests(); rig.Bridge.AdvanceBridgePhaseForTests();
                var previous = rig.Bridge.LatestOutcome;
                rig.Player.AdvanceOneFixedTickForTests();

                var tick = rig.Player.NextExpectedTick;
                rig.Transfer.AdvanceTransferPhaseForTests(); rig.Combat.AdvanceCombatPhaseForTests();
                Assert.That(rig.Combat.LatestOutcome.Tick, Is.EqualTo(tick));
                Assert.That(((System.Collections.IDictionary)GetPrivate(rig.Combat, "_deliveries")).Count, Is.EqualTo(0));
                PublishRelevantTransferPress(rig.Transfer, tick, "BossWeight-0", 1); // DebtLineA is not a payload window.

                Assert.Throws<InvalidOperationException>(() => rig.Bridge.AdvanceBridgePhaseForTests());

                Assert.That(rig.Combat.LatestOutcome.Tick, Is.EqualTo(tick));
                Assert.That(rig.Bridge.LatestOutcome.SourceTick, Is.EqualTo(previous.SourceTick));
                Assert.That(rig.Bridge.LatestOutcome.Output.Snapshot.NextExpectedTick, Is.EqualTo(new SimulationTick(1)));
                Assert.That(((System.Collections.IDictionary)GetPrivate(rig.Combat, "_deliveries")).Count, Is.EqualTo(0));
            }
            finally { DestroyAll(objects); }
        }

        // AC-COM-003, AC-COM-004: forged, stale, wrong-horizon and altered
        // candidates cannot mutate the private delivery queue; a duplicate
        // exact empty candidate leaves the single committed batch intact.
        [Test]
        public void InvalidAndDuplicateDeliveryCandidatesPreserveQueueState()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                rig.Transfer.AdvanceTransferPhaseForTests(); rig.Combat.AdvanceCombatPhaseForTests();
                var queue = (System.Collections.IDictionary)GetPrivate(rig.Combat, "_deliveries");
                var owner = GetPrivate(rig.Combat, "_deliveryOwner");
                Assert.That(queue.Count, Is.EqualTo(0));

                Assert.Throws<InvalidOperationException>(() => rig.Combat.CommitPreflightedBossDelivery(
                    new OrdanBossDeliveryCandidate(new object(), new SimulationTick(0), new SimulationTick(1), null)));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.CommitPreflightedBossDelivery(
                    new OrdanBossDeliveryCandidate(owner, new SimulationTick(1), new SimulationTick(2), null)));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.CommitPreflightedBossDelivery(
                    new OrdanBossDeliveryCandidate(owner, new SimulationTick(0), new SimulationTick(2), null)));
                var altered = new DamageRequest("ordan.payload/0000000000000000000/0000000001", "BossWeight-0", "ordan", 7, DamageKind.BossPayloadTransfer, new SimulationTick(1));
                Assert.Throws<ArgumentException>(() => rig.Combat.CommitPreflightedBossDelivery(
                    new OrdanBossDeliveryCandidate(owner, new SimulationTick(0), new SimulationTick(1), altered)));
                Assert.That(queue.Count, Is.EqualTo(0));

                var exact = new OrdanBossDeliveryCandidate(owner, new SimulationTick(0), new SimulationTick(1), null);
                rig.Combat.CommitPreflightedBossDelivery(exact); Assert.That(queue.Count, Is.EqualTo(1));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.CommitPreflightedBossDelivery(exact));
                Assert.That(queue.Count, Is.EqualTo(1));
            }
            finally { DestroyAll(objects); }
        }

        // AC-COM-003, AC-COM-004: the bridge's pending-result extractor
        // preserves M1 lethal clamp and TargetDead evidence exactly.
        [TestCase(DamageResultCode.Applied, 3)]
        [TestCase(DamageResultCode.TargetDead, 0)]
        public void PendingPayloadExtractionPassesThroughLegalLethalResults(DamageResultCode code, int amount)
        {
            var tick = new SimulationTick(8);
            var pending = new DamageRequest("ordan.payload/0000000000000000000/0000000008", "BossWeight-0", "ordan", 8, DamageKind.BossPayloadTransfer, tick);
            var result = new DamageResult(pending.RequestId, "ordan", amount, code, tick);
            var outcome = new OrdanBossCombatOutcome(tick, default(BasicAttackTickOutcome), new[] { pending }, new[] { result }, new[]
            {
                new CombatantSnapshot("ordan", 0, true, -1, 1), new CombatantSnapshot("player", 5, false, -1, 0)
            });
            var extracted = (OrdanCombatState)typeof(OrdanBossSimulationDriver).GetMethod("ExtractCombatState", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { tick, outcome, (DamageRequest?)pending });
            Assert.That(extracted.PendingPayloadDamageResult.HasValue, Is.True);
            Assert.That(extracted.PendingPayloadDamageResult.Value.Result, Is.EqualTo(code));
            Assert.That(extracted.PendingPayloadDamageResult.Value.AppliedAmount, Is.EqualTo(amount));
        }

        private static int Order(Type type)
        {
            var attribute = (DefaultExecutionOrder)Attribute.GetCustomAttribute(type, typeof(DefaultExecutionOrder));
            Assert.That(attribute, Is.Not.Null);
            return attribute.order;
        }

        private static string TraceOwnersUnderRenderGrouping(int renderHz)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects); var values = new List<string>(); var accumulator = 0; var ticks = 0;
                while (ticks < 120)
                {
                    accumulator += 60;
                    while (accumulator >= renderHz && ticks < 120)
                    {
                        accumulator -= renderHz;
                        var transfer = rig.Transfer.AdvanceTransferPhaseForTests();
                        var combat = rig.Combat.AdvanceCombatPhaseForTests();
                        var bridge = rig.Bridge.AdvanceBridgePhaseForTests();
                        values.Add(transfer.Snapshot.Tick.Value + ":" + combat.Tick.Value + ":" + bridge.SourceTick.Value + ":" + bridge.Forecast.TargetId);
                        rig.Player.AdvanceOneFixedTickForTests(); ticks++;
                    }
                }
                return string.Join("/", values);
            }
            finally { DestroyAll(objects); }
        }

        private static OrdanBossTickInput Input(int tick, int health) => new OrdanBossTickInput(new SimulationTick(tick),
            new OrdanCombatState(new SimulationTick(tick), "ordan", health, health == 0, null), null);

        private static SimulationCameraPoseSnapshot CameraFor(SimulationTick tick) =>
            new SimulationCameraPoseSnapshot(new SimulationTick(checked(tick.Value - 1)), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);

        private static void PublishRelevantTransferPress(TransferSimulationDriver transfer, SimulationTick tick, string targetId, int revision)
        {
            var prior = transfer.LatestPhasePublication;
            Assert.That(prior.HasValue, Is.True); Assert.That(prior.Value.Tick, Is.EqualTo(tick));
            var changed = new TransferStateChanged(targetId, "Player.Lightweight.v1", "Target.BossPayload.ScriptedHeavy.v1", tick, revision);
            var press = new TransferAttemptResult(tick, null, targetId, true, TransferFailureReason.None,
                new TransferSessionSnapshot(tick, string.Empty, targetId, TransferPlayerState.Lightweight, 26, TransferFailureReason.None, revision), changed, null);
            SetPrivate(transfer, "_latestPhasePublication", new TransferPhasePublication(
                new TransferTickOutcome(prior.Value.Outcome.RemovalResult, press, prior.Value.Outcome.Snapshot), prior.Value.Input));
        }

        private static BossRig CreateRig(ICollection<GameObject> objects)
        {
            var playerObject = Track(objects, "ordan-lifecycle-player");
            var playerBody = playerObject.AddComponent<Rigidbody2D>(); playerBody.bodyType = RigidbodyType2D.Kinematic;
            var playerCollider = playerObject.AddComponent<BoxCollider2D>();
            var player = playerObject.AddComponent<PlayerMovementController>();
            var ordanObject = Track(objects, "ordan-lifecycle-body"); ordanObject.transform.position = Vector3.right;
            var ordanCollider = ordanObject.AddComponent<BoxCollider2D>();
            var ordan = ordanObject.AddComponent<CombatTarget>(); ordan.ConfigureForTests("ordan", 60, 0, true, 0, 0, 0, 0, 100, 100, ordanCollider, ordanObject.transform);
            var playerTarget = playerObject.AddComponent<CombatTarget>(); playerTarget.ConfigureForTests("player", 5, 45, false, 0, 0, 0, 0, 100, 100, playerCollider, playerObject.transform);
            var transferRegistry = Track(objects, "ordan-lifecycle-transfer-registry").AddComponent<TransferTargetRegistry>(); transferRegistry.ConfigureForTests();
            var transfer = Track(objects, "ordan-lifecycle-transfer").AddComponent<TransferSimulationDriver>(); transfer.ConfigureForTests(player, transferRegistry); transfer.InitializeForTests();
            var combatRegistry = Track(objects, "ordan-lifecycle-combat-registry").AddComponent<CombatTargetRegistry>(); combatRegistry.ConfigureForTests(ordan, playerTarget);
            var combatObject = Track(objects, "ordan-lifecycle-combat"); combatObject.SetActive(false);
            var combat = combatObject.AddComponent<OrdanBossCombatSimulationDriver>(); combat.ConfigureForTests(player, combatRegistry, transfer, combat); combatObject.SetActive(true);
            var bridgeObject = Track(objects, "ordan-lifecycle-bridge"); bridgeObject.SetActive(false);
            var bridge = bridgeObject.AddComponent<OrdanBossSimulationDriver>(); bridge.ConfigureForTests(player, transfer, combat); bridge.InitializeForTests(); bridgeObject.SetActive(true);
            return new BossRig(player, transfer, combat, bridge);
        }

        private static object GetPrivate(object value, string fieldName) => value.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(value);
        private static void SetPrivate(object value, string fieldName, object fieldValue) => value.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(value, fieldValue);
        private static void DestroyAll(IReadOnlyList<GameObject> objects) { for (var index = objects.Count - 1; index >= 0; index--) UnityEngine.Object.DestroyImmediate(objects[index]); }

        private readonly struct BossRig
        {
            internal BossRig(PlayerMovementController player, TransferSimulationDriver transfer, OrdanBossCombatSimulationDriver combat, OrdanBossSimulationDriver bridge)
            { Player = player; Transfer = transfer; Combat = combat; Bridge = bridge; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Transfer { get; }
            internal OrdanBossCombatSimulationDriver Combat { get; }
            internal OrdanBossSimulationDriver Bridge { get; }
        }

        private static GameObject Track(ICollection<GameObject> objects, string name) { var value = new GameObject(name); objects.Add(value); return value; }
    }
}
