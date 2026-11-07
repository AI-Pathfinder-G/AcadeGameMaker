using System;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    internal sealed class OrdanBossCombatBridgeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private const string TemporaryScenePath = "Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/__TempOrdanBossBridgeScene.unity";

        [SetUp]
        public void SetUp()
        {
            AssetDatabase.DeleteAsset(TemporaryScenePath);
            Assert.That(EditorSceneManager.SaveScene(EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single), TemporaryScenePath), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            for (var index = _objects.Count - 1; index >= 0; index--) UnityEngine.Object.DestroyImmediate(_objects[index]);
            _objects.Clear(); EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AssetDatabase.DeleteAsset(TemporaryScenePath); AssetDatabase.Refresh();
        }
        // AC-COM-002, AC-COM-003: the boss pipeline is fixed-order and does
        // not introduce a second physics or time authority.
        [Test]
        public void BossDriversHaveTheExactFixedOrders()
        {
            Assert.That(ExecutionOrder(typeof(TransferSimulationDriver)), Is.EqualTo(-200));
            Assert.That(ExecutionOrder(typeof(OrdanBossCombatSimulationDriver)), Is.EqualTo(-190));
            Assert.That(ExecutionOrder(typeof(OrdanBossSimulationDriver)), Is.EqualTo(-185));
            Assert.That(ExecutionOrder(typeof(TransferSimulationDriver)), Is.LessThan(ExecutionOrder(typeof(OrdanBossCombatSimulationDriver))));
            Assert.That(ExecutionOrder(typeof(OrdanBossCombatSimulationDriver)), Is.LessThan(ExecutionOrder(typeof(OrdanBossSimulationDriver))));
        }

        // AC-COM-004, AC-WT-005: the M4A-owned DTO accepts only a checked
        // next-tick payload identity and refuses Unity-side guessed targets.
        [Test]
        public void ExposureForecastHasExactHorizonAndPayloadDomain()
        {
            var forecast = new OrdanTransferExposureForecast(new SimulationTick(9), new SimulationTick(10), "BossWeight-2");
            Assert.That(forecast.IsExposed, Is.True);
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrdanTransferExposureForecast(new SimulationTick(9), new SimulationTick(11), "BossWeight-2"));
            Assert.Throws<ArgumentException>(() => new OrdanTransferExposureForecast(new SimulationTick(9), new SimulationTick(10), "BossAuditBox-0"));
        }

        // AC-COM-004: private-owner queue candidates cannot be committed by a
        // different owner reference even before any scene graph is constructed.
        [Test]
        public void BossDeliveryCandidateRetainsItsPrivateOwner()
        {
            var owner = new object();
            var candidate = new OrdanBossDeliveryCandidate(owner, new SimulationTick(4), new SimulationTick(5), null);
            Assert.That(candidate.IsOwnedBy(owner), Is.True);
            Assert.That(candidate.IsOwnedBy(new object()), Is.False);
        }

        // AC-COM-003: source/delivery identity is retained even for mandatory
        // empty batches, so a later tick cannot reinterpret bootstrap state.
        [Test]
        public void EmptyDeliveryBatchRetainsExactTickIdentity()
        {
            var batch = new OrdanBossDeliveryBatch(new SimulationTick(12), new SimulationTick(13), null);
            Assert.That(batch.SourceTick, Is.EqualTo(new SimulationTick(12)));
            Assert.That(batch.DeliveryTick, Is.EqualTo(new SimulationTick(13)));
            Assert.That(batch.Request.HasValue, Is.False);
        }

        // AC-COM-004: caller-owned external arrays are copied before M1 can
        // sort them, preventing post-submit request/result correlation drift.
        [Test]
        public void BossCombatInputDefensivelyCopiesExternalRequests()
        {
            var original = new[] { new DamageRequest("external/0", "test", "ordan", 1, DamageKind.BasicAttack, new SimulationTick(3)) };
            var input = new OrdanBossCombatInput(new SimulationTick(3), null, null, null, original);
            original[0] = new DamageRequest("external/changed", "test", "ordan", 1, DamageKind.BasicAttack, new SimulationTick(3));
            Assert.That(input.ExternalDamageRequests[0].RequestId, Is.EqualTo("external/0"));
        }

        // AC-COM-003: explicit MonoBehaviour owner data is the only graph
        // authority; Combat does not discover a regular driver at runtime.
        [Test]
        public void OwnerListIsExplicitAndNoRuntimeDiscoveryMethodIsPresent()
        {
            var field = typeof(OrdanBossCombatSimulationDriver).GetField("_bossCombatOwners", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            Assert.That(field.FieldType, Is.EqualTo(typeof(List<MonoBehaviour>)));
            Assert.That(typeof(OrdanBossCombatSimulationDriver).GetMethod("GetComponent", BindingFlags.Instance | BindingFlags.NonPublic), Is.Null);
        }

        // AC-COM-004: delivery candidates preserve an exact M4A payload value
        // and cannot alter its source, target, kind, amount, or tick fields.
        [Test]
        public void DeliveryCandidateRetainsPayloadValueFields()
        {
            var request = new DamageRequest("ordan.payload/0000000000000000002/0000000042", "BossWeight-2", "ordan", 8, DamageKind.BossPayloadTransfer, new SimulationTick(42));
            var candidate = new OrdanBossDeliveryCandidate(new object(), new SimulationTick(41), new SimulationTick(42), request);
            Assert.That(candidate.Request.Value.RequestId, Is.EqualTo(request.RequestId));
            Assert.That(candidate.Request.Value.SourceId, Is.EqualTo("BossWeight-2"));
            Assert.That(candidate.Request.Value.TargetId, Is.EqualTo("ordan"));
            Assert.That(candidate.Request.Value.Amount, Is.EqualTo(8));
            Assert.That(candidate.Request.Value.Kind, Is.EqualTo(DamageKind.BossPayloadTransfer));
            Assert.That(candidate.Request.Value.Tick, Is.EqualTo(new SimulationTick(42)));
        }

        // AC-COM-004, AC-WT-005: every reserved Transfer ID projects only
        // through its matching successful press/state-change identity.
        [TestCase("BossWeight-0", "Target.BossPayload.ScriptedHeavy.v1", OrdanTransferTargetKind.BossPayload)]
        [TestCase("BossWeight-1", "Target.BossPayload.ScriptedHeavy.v1", OrdanTransferTargetKind.BossPayload)]
        [TestCase("BossWeight-2", "Target.BossPayload.ScriptedHeavy.v1", OrdanTransferTargetKind.BossPayload)]
        [TestCase("BossAuditBox-0", "Target.Box.Heavy.v1", OrdanTransferTargetKind.Box)]
        public void AllRelevantTransferIdsProjectTheirExactKind(string id, string modifier, OrdanTransferTargetKind expectedKind)
        {
            var edge = Project(Press(id, id, modifier, 7));
            Assert.That(edge.HasValue, Is.True); Assert.That(edge.Value.TargetKind, Is.EqualTo(expectedKind));
            Assert.That(edge.Value.Revision, Is.EqualTo(7));
        }

        // AC-COM-003, AC-WT-005: a forged press selection cannot be smuggled
        // into M4A even when its nested StateChanged value otherwise looks valid.
        [Test]
        public void MismatchedPressSelectionIsRejectedBeforeM4AMutation()
        {
            Assert.Throws<TargetInvocationException>(() => Project(Press("other", "BossWeight-0", "Target.BossPayload.ScriptedHeavy.v1", 1)));
        }

        // AC-COM-003, AC-WT-005: a relevant authored ID with another valid
        // Transfer modifier is a contract error, never an irrelevant silence.
        [TestCase("BossWeight-0", "Target.Box.Heavy.v1")]
        [TestCase("BossWeight-1", "Target.Box.Heavy.v1")]
        [TestCase("BossWeight-2", "Target.Box.Heavy.v1")]
        [TestCase("BossAuditBox-0", "Target.BossPayload.ScriptedHeavy.v1")]
        public void AllRelevantModifierMismatchesReject(string id, string wrongModifier)
        {
            Assert.Throws<TargetInvocationException>(() => Project(Press(id, id, wrongModifier, 1)));
        }

        // AC-COM-003, AC-WT-005: absent/failed/clear-or-recall (no press) and
        // unrelated successful attempts are silence, not fabricated M4A edges.
        [Test]
        public void ProjectionSilencesAbsentFailedAndUnrelatedTransferAttempts()
        {
            Assert.That(Project(null), Is.Null);
            var tick = new SimulationTick(5);
            var failed = new TransferAttemptResult(tick, null, "BossWeight-0", false, TransferFailureReason.TargetUnavailable,
                new TransferSessionSnapshot(tick, string.Empty, string.Empty, TransferPlayerState.Baseline, 26, TransferFailureReason.TargetUnavailable, 0), null, null);
            Assert.That(Project(failed), Is.Null);
            Assert.That(Project(Press("unrelated", "unrelated", "Target.Box.Heavy.v1", 1)), Is.Null);
        }

        // AC-COM-003, AC-COM-004: the serialized owner list is authoritative;
        // null, absent, duplicate, extra, and wrong owner lists fail before M1.
        [TestCaseSource(nameof(InvalidOwnerCases))]
        public void InvalidOwnerListsRejectBeforeAnyCombatPublication(string mode)
        {
            var rig = CreateValidRig(); var extra = Track(new GameObject("extra-owner")).AddComponent<OrdanBossSimulationDriver>();
            MonoBehaviour[] owners = mode == "null" ? null : mode == "empty" ? Array.Empty<MonoBehaviour>() :
                mode == "duplicate" ? new MonoBehaviour[] { rig.Combat, rig.Combat } : mode == "extra" ? new MonoBehaviour[] { rig.Combat, extra } : new MonoBehaviour[] { extra };
            rig.Combat.ConfigureForTests(rig.Player, rig.Registry, rig.Transfer, owners);
            Assert.Throws<InvalidOperationException>(() => rig.Combat.PrepareBossDeliveryRegistration());
            Assert.That(rig.Combat.HasLatestOutcome, Is.False);
        }

        // AC-COM-003: missing, unsorted, wrong authored values, and an Ordan
        // Transfer body each reject during staging without a public outcome.
        [TestCase("missing")]
        [TestCase("unsorted")]
        [TestCase("wrong-health")]
        [TestCase("ordan-transfer-body")]
        public void InvalidBossRostersRejectBeforeAnyCombatPublication(string mode)
        {
            var rig = CreateValidRig();
            if (mode == "missing") rig.Registry.ConfigureForTests(rig.Ordan);
            else if (mode == "unsorted") rig.Registry.ConfigureForTests(rig.PlayerTarget, rig.Ordan);
            else if (mode == "wrong-health")
            {
                var wrong = CreateTarget("ordan", 59, 0, true, null); rig.Registry.ConfigureForTests(wrong, rig.PlayerTarget);
            }
            else
            {
                var body = CreateTarget("ordan", 60, 0, true, CreateTransferBody("ordan")); rig.Registry.ConfigureForTests(body, rig.PlayerTarget);
            }
            rig.Combat.ConfigureForTests(rig.Player, rig.Registry, rig.Transfer, rig.Combat);
            Assert.Throws<InvalidOperationException>(() => rig.Combat.PrepareBossDeliveryRegistration());
            Assert.That(rig.Combat.HasLatestOutcome, Is.False);
        }

        // AC-COM-003, AC-COM-004: bridge bindings must retain the exact player
        // and Transfer owner that the staged Combat driver was configured with.
        [TestCase("player")]
        [TestCase("transfer")]
        public void CrossWiredBridgeRejectsBeforeCombatLatestPublication(string mode)
        {
            var rig = CreateValidRig(); rig.Combat.ConfigureForTests(rig.Player, rig.Registry, rig.Transfer, rig.Combat);
            var bridge = Track(new GameObject("bridge")).AddComponent<OrdanBossSimulationDriver>();
            if (mode == "player") bridge.ConfigureForTests(CreatePlayer("other-player"), rig.Transfer, rig.Combat);
            else bridge.ConfigureForTests(rig.Player, Track(new GameObject("other-transfer")).AddComponent<TransferSimulationDriver>(), rig.Combat);
            Assert.Throws<InvalidOperationException>(() => bridge.InitializeForTests());
            Assert.That(rig.Combat.HasLatestOutcome, Is.False);
        }

        // AC-COM-004, AC-WT-006: aim-bearing Combat rejects an absent, stale,
        // or wrong-sample Transfer receipt before M2A/M1 or the bootstrap lane
        // can consume state.
        [TestCase("missing")]
        [TestCase("stale")]
        [TestCase("sample-id")]
        public void ReceiptFailuresPreserveProvisionalSessionsAndDeliveryLane(string mode)
        {
            var rig = CreateValidRig(); rig.Combat.ConfigureForTests(rig.Player, rig.Registry, rig.Transfer, rig.Combat);
            var bridge = Track(new GameObject("bridge")).AddComponent<OrdanBossSimulationDriver>(); bridge.ConfigureForTests(rig.Player, rig.Transfer, rig.Combat); bridge.InitializeForTests();
            if (mode == "stale") SetPrivate(rig.Transfer, "_latestCaptureReceipt", new TransferCaptureReceipt(new SimulationTick(1), 7, new SimulationTick(1), new SimulationTick(0)));
            if (mode == "sample-id") SetPrivate(rig.Transfer, "_latestCaptureReceipt", new TransferCaptureReceipt(new SimulationTick(0), 8, new SimulationTick(0), new SimulationTick(-1)));
            var camera = new SimulationCameraPoseSnapshot(new SimulationTick(-1), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
            var aim = new AimSample(7, AimSource.GamepadStick, null, null, null, 4096, 0, new SimulationTick(-1), new SimulationTick(0));
            rig.Combat.Submit(new OrdanBossCombatInput(new SimulationTick(0), camera, aim, null, Array.Empty<DamageRequest>()));
            Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
            Assert.That(rig.Combat.HasLatestOutcome, Is.False); Assert.That(rig.Combat.HasCurrentOutcome, Is.False);
            var staged = (CombatSession)GetPrivate(rig.Combat, "_provisionalCombatSession"); Assert.That(staged.NextExpectedTick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(((System.Collections.IDictionary)GetPrivate(rig.Combat, "_deliveries")).Count, Is.EqualTo(0));
        }

        // AC-COM-003, AC-COM-004: first-use bridge state is staged, not
        // adopted. A missing current Transfer publication discards both the
        // provisional Combat pair and the staged M4A session.
        [Test]
        public void BootstrapBridgeFailureDiscardsEveryProvisionalSessionAndPublishesNothing()
        {
            var rig = CreateValidRig(); rig.Combat.ConfigureForTests(rig.Player, rig.Registry, rig.Transfer, rig.Combat);
            var bridge = Track(new GameObject("bridge")).AddComponent<OrdanBossSimulationDriver>();
            bridge.ConfigureForTests(rig.Player, rig.Transfer, rig.Combat); bridge.InitializeForTests();
            rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(rig.Combat.HasCurrentOutcome, Is.True); Assert.That(rig.Combat.HasLatestOutcome, Is.False);
            Assert.That(GetPrivate(bridge, "_stagedSession"), Is.Not.Null); Assert.That(GetPrivate(bridge, "_session"), Is.Null);

            Assert.Throws<InvalidOperationException>(() => bridge.AdvanceBridgePhaseForTests());

            Assert.That(rig.Combat.HasCurrentOutcome, Is.False); Assert.That(rig.Combat.HasLatestOutcome, Is.False);
            Assert.That(bridge.HasLatestOutcome, Is.False); Assert.That(GetPrivate(bridge, "_stagedSession"), Is.Null);
            Assert.That(GetPrivate(bridge, "_session"), Is.Null); Assert.That(GetPrivate(bridge, "_initialized"), Is.False);
        }

        private static int ExecutionOrder(Type type)
        {
            var attribute = (DefaultExecutionOrder)Attribute.GetCustomAttribute(type, typeof(DefaultExecutionOrder));
            Assert.That(attribute, Is.Not.Null);
            return attribute.order;
        }

        private static OrdanRelevantTransferEdge? Project(TransferAttemptResult? press) =>
            (OrdanRelevantTransferEdge?)typeof(OrdanBossSimulationDriver).GetMethod("ProjectRelevantEdge", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { new SimulationTick(5), press });

        private static TransferAttemptResult Press(string selected, string changedTarget, string modifier, int revision)
        {
            var tick = new SimulationTick(5);
            var snapshot = new TransferSessionSnapshot(tick, string.Empty, changedTarget, TransferPlayerState.Lightweight, 26, TransferFailureReason.None, revision);
            var changed = new TransferStateChanged(changedTarget, "Player.Lightweight.v1", modifier, tick, revision);
            return new TransferAttemptResult(tick, null, selected, true, TransferFailureReason.None, snapshot, changed, null);
        }

        private static IEnumerable<object[]> InvalidOwnerCases()
        {
            yield return new object[] { "null" }; yield return new object[] { "empty" }; yield return new object[] { "duplicate" };
            yield return new object[] { "extra" }; yield return new object[] { "wrong" };
        }

        private Rig CreateValidRig()
        {
            var player = CreatePlayer("player-root"); var playerTarget = CreateTarget("player", 5, 45, false, null, player.gameObject.GetComponent<Collider2D>(), player.transform);
            var ordan = CreateTarget("ordan", 60, 0, true, null);
            var registry = Track(new GameObject("combat-registry")).AddComponent<CombatTargetRegistry>(); registry.ConfigureForTests(ordan, playerTarget);
            var transfer = Track(new GameObject("transfer")).AddComponent<TransferSimulationDriver>();
            var combat = Track(new GameObject("combat")).AddComponent<OrdanBossCombatSimulationDriver>();
            return new Rig(player, playerTarget, ordan, registry, transfer, combat);
        }

        private PlayerMovementController CreatePlayer(string name)
        {
            var value = Track(new GameObject(name)); value.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; value.AddComponent<BoxCollider2D>(); return value.AddComponent<PlayerMovementController>();
        }
        private CombatTarget CreateTarget(string id, int health, int invulnerability, bool attackable, TransferTarget transfer, Collider2D collider = null, Transform root = null)
        {
            var value = collider == null ? Track(new GameObject(id)) : collider.gameObject;
            if (collider == null) collider = value.AddComponent<BoxCollider2D>(); if (root == null) root = value.transform;
            var target = value.AddComponent<CombatTarget>(); target.ConfigureForTests(id, health, invulnerability, attackable, 0, 0, 0, 0, 100, 100, collider, root, transfer); return target;
        }
        private TransferTarget CreateTransferBody(string id)
        {
            var value = Track(new GameObject(id + "-transfer")); var collider = value.AddComponent<BoxCollider2D>(); var sink = value.AddComponent<Sink>(); sink.Id = id;
            var transfer = value.AddComponent<TransferTarget>(); transfer.ConfigureForTests(id, TransferTargetKind.BossPayload, 0, 0, 0, 0, 100, 100, "profile", collider, value.transform, sink); return transfer;
        }
        private GameObject Track(GameObject value) { _objects.Add(value); return value; }
        private static object GetPrivate(object value, string name) => value.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(value);
        private static void SetPrivate(object value, string name, object fieldValue) => value.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(value, fieldValue);
        private readonly struct Rig { internal Rig(PlayerMovementController player, CombatTarget playerTarget, CombatTarget ordan, CombatTargetRegistry registry, TransferSimulationDriver transfer, OrdanBossCombatSimulationDriver combat) { Player = player; PlayerTarget = playerTarget; Ordan = ordan; Registry = registry; Transfer = transfer; Combat = combat; } internal PlayerMovementController Player { get; } internal CombatTarget PlayerTarget { get; } internal CombatTarget Ordan { get; } internal CombatTargetRegistry Registry { get; } internal TransferSimulationDriver Transfer { get; } internal OrdanBossCombatSimulationDriver Combat { get; } }
        private sealed class Sink : MonoBehaviour, ITransferTargetModifierSink { internal string Id; public string BoundTargetId => Id; public bool IsStillAvailable() => false; public bool TryApply(TransferModifierRequest request) => false; public void Clear(TransferClearReason reason, SimulationTick tick, int revision) { } }
    }
}
