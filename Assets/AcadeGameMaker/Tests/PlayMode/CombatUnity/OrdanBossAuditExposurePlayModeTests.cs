// AC-COM-002, AC-COM-003; AC-WT-003, AC-WT-005.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Camera.Unity;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    internal sealed class OrdanBossAuditExposurePlayModeTests
    {
        private const string GraphPath = "Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab";

        // AC-COM-002, AC-WT-005. The authored four-target production graph
        // consumes the M4A audit forecast only at the legal Execute horizon.
        [UnityTest]
        public IEnumerator AuthoredGraphExposesAuditAtBalanceAuditExecuteHorizonOnly()
        {
            var root = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath));
            try
            {
                var player = root.GetComponentInChildren<PlayerMovementController>();
                var transfer = root.GetComponentInChildren<TransferSimulationDriver>();
                var combat = root.GetComponentInChildren<OrdanBossCombatSimulationDriver>();
                var bridge = root.GetComponentInChildren<OrdanBossSimulationDriver>();
                var scheduler = root.GetComponentInChildren<OrdanBossTransferExposureScheduler>();
                var hostile = root.GetComponentInChildren<OrdanBossHostileDamageProducer>();
                var auditPull = root.GetComponentInChildren<OrdanBossBalanceAuditPullProducer>();
                var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                var audit = root.GetComponentInChildren<OrdanBossAuditTransferModifierSink>(true);
                Assert.That(audit, Is.Not.Null); Assert.That(audit.IsExposed, Is.False);
                DisableAuthoredAutoDrivers(root);
                player.enabled = false; transfer.enabled = false; combat.enabled = false; bridge.enabled = false; scheduler.enabled = false; hostile.enabled = false; auditPull.enabled = false; handoff.enabled = false;

                for (var index = 0; index <= 455; index++)
                {
                    Assert.That(transfer.AdvanceTransferPhaseForTests().Snapshot.Tick.Value, Is.EqualTo(index));
                    combat.AdvanceCombatPhaseForTests(); bridge.AdvanceBridgePhaseForTests();
                    var adopted = scheduler.AdvanceSchedulerPhaseForTests();
                    if (index == 455)
                    {
                        Assert.That(adopted.ExposedTargetId, Is.Empty);
                        Assert.That(scheduler.LatestAuditOutcome.ExposedTargetId, Is.EqualTo("BossAuditBox-0"));
                        Assert.That(audit.IsExposed, Is.True);
                    }
                    player.AdvanceOneFixedTickForTests();
                }
                Assert.That(bridge.LatestOutcome.AuditForecast.TargetId, Is.EqualTo("BossAuditBox-0"));
                yield return null;
            }
            finally { UnityEngine.Object.Destroy(root); }
        }

        // AC-COM-003, AC-WT-005. Terminal input retains the exact completed
        // four-ID proof while the two immutable scheduler projections stay split.
        [Test]
        public void CompletedRemovalIdsAreExactAndDefensivelyRead()
        {
            var root = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath));
            try
            {
                var player = root.GetComponentInChildren<PlayerMovementController>();
                var transfer = root.GetComponentInChildren<TransferSimulationDriver>();
                var combat = root.GetComponentInChildren<OrdanBossCombatSimulationDriver>();
                var bridge = root.GetComponentInChildren<OrdanBossSimulationDriver>();
                var scheduler = root.GetComponentInChildren<OrdanBossTransferExposureScheduler>();
                var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                DisableAuthoredAutoDrivers(root);
                player.enabled = false; transfer.enabled = false; combat.enabled = false; bridge.enabled = false; scheduler.enabled = false; handoff.enabled = false;
                for (var index = 0; index < 2; index++)
                {
                    transfer.AdvanceTransferPhaseForTests(); combat.AdvanceCombatPhaseForTests(); bridge.AdvanceBridgePhaseForTests(); scheduler.AdvanceSchedulerPhaseForTests(); player.AdvanceOneFixedTickForTests();
                }
                var tick = player.NextExpectedTick;
                transfer.AdvanceTransferPhaseForTests();
                combat.Submit(new OrdanBossCombatInput(tick, null, null, null,
                    new[] { new DamageRequest("ordan.audit-test.lethal", "test", "ordan", 60, DamageKind.BossPattern, tick) }));
                combat.AdvanceCombatPhaseForTests(); bridge.AdvanceBridgePhaseForTests();
                var terminal = scheduler.AdvanceSchedulerPhaseForTests();
                Assert.That(terminal.PermanentlyRemovedTargetIds, Is.EqualTo(new[] { "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
                Assert.That(scheduler.LatestAuditOutcome.PermanentlyRemovedTargetIds, Is.EqualTo(new[] { "BossAuditBox-0" }));
                player.AdvanceOneFixedTickForTests(); transfer.AdvanceTransferPhaseForTests();
                var read = transfer.LatestCompletedInputRemovalIds;
                Assert.That(read, Is.EqualTo(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
                Assert.Throws<NotSupportedException>(() => ((IList<string>)read).Add("forged"));
                Assert.That(transfer.LatestCompletedInputRemovalIds, Is.EqualTo(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
        }

        // AC-COM-003, AC-WT-005. A terminal production batch cannot silently
        // absorb an unrelated pre-queued removal: fail before the one allowed
        // queue replacement and preserve the foreign input for its own phase.
        [Test]
        public void ForeignQueuedRemovalRejectsTerminalBatchBeforeSchedulerPublication()
        {
            var root = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath));
            try
            {
                var player = root.GetComponentInChildren<PlayerMovementController>();
                var transfer = root.GetComponentInChildren<TransferSimulationDriver>();
                var combat = root.GetComponentInChildren<OrdanBossCombatSimulationDriver>();
                var bridge = root.GetComponentInChildren<OrdanBossSimulationDriver>();
                var scheduler = root.GetComponentInChildren<OrdanBossTransferExposureScheduler>();
                var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                DisableAuthoredAutoDrivers(root);
                player.enabled = false; transfer.enabled = false; combat.enabled = false; bridge.enabled = false; scheduler.enabled = false; handoff.enabled = false;
                for (var index = 0; index < 2; index++)
                {
                    transfer.AdvanceTransferPhaseForTests(); combat.AdvanceCombatPhaseForTests(); bridge.AdvanceBridgePhaseForTests(); scheduler.AdvanceSchedulerPhaseForTests(); player.AdvanceOneFixedTickForTests();
                }
                var tick = player.NextExpectedTick;
                transfer.AdvanceTransferPhaseForTests();
                transfer.SubmitForTests(new TransferSimulationInput(new SimulationTick(tick.Value + 1), null, null, null,
                    new[] { new TransferTargetRemoved("foreign", new SimulationTick(tick.Value + 1)) }, null));
                combat.Submit(new OrdanBossCombatInput(tick, null, null, null,
                    new[] { new DamageRequest("ordan.audit-test.foreign", "test", "ordan", 60, DamageKind.BossPattern, tick) }));
                combat.AdvanceCombatPhaseForTests(); bridge.AdvanceBridgePhaseForTests();
                var priorSourceTick = scheduler.LatestOutcome.SourceTick;
                Assert.Throws<InvalidOperationException>(() => scheduler.AdvanceSchedulerPhaseForTests());
                Assert.That(scheduler.HasLatestOutcome, Is.True);
                Assert.That(scheduler.LatestOutcome.SourceTick, Is.EqualTo(priorSourceTick));
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
        }

        // AC-COM-003, AC-WT-005. An active audit target is part of the one
        // ordered terminal batch and is cleared exactly once at the next -200.
        [Test]
        public void ActiveAuditDeathQueuesFourIdsAndClearsAuditOnce()
        {
            var rig = CreateManualRig();
            try
            {
                AdvanceThrough(rig, 455);
                var tick = rig.Player.NextExpectedTick;
                MovePlayerIntoAuditRange(rig);
                rig.Transfer.SubmitForTests(Press(tick));
                var transfer = rig.Transfer.AdvanceTransferPhaseForTests();
                Assert.That(transfer.PressResult.Value.Success, Is.True); Assert.That(transfer.PressResult.Value.SelectedTargetId, Is.EqualTo("BossAuditBox-0"));
                Assert.That(rig.Audit.IsApplied, Is.True);
                SubmitLethal(rig.Combat, tick, "ordan.audit-test.active");
                rig.Combat.AdvanceCombatPhaseForTests(); rig.Bridge.AdvanceBridgePhaseForTests();
                rig.Scheduler.AdvanceSchedulerPhaseForTests();
                Assert.That(rig.Scheduler.LatestAuditOutcome.PermanentlyRemovedTargetIds, Is.EqualTo(new[] { "BossAuditBox-0" }));
                rig.Player.AdvanceOneFixedTickForTests();
                var cleanup = rig.Transfer.AdvanceTransferPhaseForTests();
                Assert.That(cleanup.RemovalResult.Value.Cleared.Value.Reason, Is.EqualTo(TransferClearReason.TargetRemoved));
                Assert.That(cleanup.RemovalResult.Value.Cleared.Value.PreviousTargetId, Is.EqualTo("BossAuditBox-0"));
                Assert.That(rig.Audit.IsApplied, Is.False); Assert.That(rig.Audit.LastAcceptedRevision, Is.EqualTo(2));
                Assert.That(rig.Transfer.LatestCompletedInputRemovalIds, Is.EqualTo(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
        }

        // AC-COM-003, AC-WT-005. Lifecycle-first keeps the requested terminal
        // batch observable but produces only the pre-existing lifecycle clear.
        [TestCase(TransferClearReason.RoomLeaving)]
        [TestCase(TransferClearReason.RunFailed)]
        [TestCase(TransferClearReason.Cutscene)]
        [TestCase(TransferClearReason.DemoCompleted)]
        public void EverySupportedLifecycleWinsOverActiveAuditTerminalRemoval(TransferClearReason lifecycle)
        {
            var rig = CreateManualRig();
            try
            {
                AdvanceThrough(rig, 455);
                var tick = rig.Player.NextExpectedTick;
                MovePlayerIntoAuditRange(rig);
                rig.Transfer.SubmitForTests(new TransferSimulationInput(new SimulationTick(tick.Value + 1), null, null, null,
                    Array.Empty<TransferTargetRemoved>(), lifecycle));
                rig.Transfer.SubmitForTests(Press(tick));
                var transfer = rig.Transfer.AdvanceTransferPhaseForTests();
                Assert.That(transfer.PressResult.Value.Success, Is.True); Assert.That(rig.Audit.IsApplied, Is.True);
                SubmitLethal(rig.Combat, tick, "ordan.audit-test.lifecycle." + lifecycle);
                rig.Combat.AdvanceCombatPhaseForTests(); rig.Bridge.AdvanceBridgePhaseForTests(); rig.Scheduler.AdvanceSchedulerPhaseForTests();
                rig.Player.AdvanceOneFixedTickForTests();
                var lifecycleOutcome = rig.Transfer.AdvanceTransferPhaseForTests();
                Assert.That(lifecycleOutcome.RemovalResult.Value.Cleared.Value.Reason, Is.EqualTo(lifecycle));
                Assert.That(lifecycleOutcome.RemovalResult.Value.Cleared.Value.PreviousTargetId, Is.EqualTo("BossAuditBox-0"));
                Assert.That(rig.Audit.LastAcceptedRevision, Is.EqualTo(2));
                Assert.That(rig.Transfer.LatestCompletedInputRemovalIds, Is.EqualTo(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
        }

        // AC-COM-002, AC-WT-005. The normal audit withdrawal retains its
        // registration: the same stable ID is selectable in the next audit window.
        [Test]
        public void AuditTemporaryClearRetainsSameIdForNextBalanceAuditReuse()
        {
            var rig = CreateManualRig();
            try
            {
                AdvanceThrough(rig, 455);
                var firstTick = rig.Player.NextExpectedTick;
                MovePlayerIntoAuditRange(rig);
                rig.Transfer.SubmitForTests(Press(firstTick));
                Assert.That(rig.Transfer.AdvanceTransferPhaseForTests().PressResult.Value.Success, Is.True);
                rig.Combat.AdvanceCombatPhaseForTests(); rig.Bridge.AdvanceBridgePhaseForTests(); rig.Scheduler.AdvanceSchedulerPhaseForTests();
                Assert.That(rig.Scheduler.LatestAuditOutcome.TemporarilyEndedTargetIds, Is.EqualTo(new[] { "BossAuditBox-0" }));
                rig.Player.AdvanceOneFixedTickForTests();
                var clear = rig.Transfer.AdvanceTransferPhaseForTests();
                Assert.That(clear.RemovalResult.Value.Cleared.Value.Reason, Is.EqualTo(TransferClearReason.TargetRemoved));
                Assert.That(rig.Audit.IsStillAvailable(), Is.False);
                rig.Combat.AdvanceCombatPhaseForTests(); rig.Bridge.AdvanceBridgePhaseForTests(); rig.Scheduler.AdvanceSchedulerPhaseForTests(); rig.Player.AdvanceOneFixedTickForTests();
                while (rig.Player.NextExpectedTick.Value < 1051) ManualStep(rig);
                var reuseTick = rig.Player.NextExpectedTick;
                MovePlayerIntoAuditRange(rig);
                rig.Transfer.SubmitForTests(Press(reuseTick));
                var reuse = rig.Transfer.AdvanceTransferPhaseForTests();
                Assert.That(reuse.PressResult.Value.Success, Is.True); Assert.That(reuse.PressResult.Value.SelectedTargetId, Is.EqualTo("BossAuditBox-0"));
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
        }

        private static void SubmitLethal(OrdanBossCombatSimulationDriver combat, SimulationTick tick, string requestId) =>
            combat.Submit(new OrdanBossCombatInput(tick, null, null, null, new[] { new DamageRequest(requestId, "test", "ordan", 60, DamageKind.BossPattern, tick) }));
        private static void AdvanceThrough(ManualRig rig, int inclusiveTick)
        {
            while (rig.Player.NextExpectedTick.Value <= inclusiveTick) ManualStep(rig);
        }
        private static void ManualStep(ManualRig rig)
        {
            if (rig.Player.Snapshot.PositionXQ4096 < -5 * 4096)
                rig.Player.SubmitCommand(new MovementCommand(rig.Player.NextExpectedTick, 4096, 0, false, false, false));
            rig.Transfer.AdvanceTransferPhaseForTests(); rig.Combat.AdvanceCombatPhaseForTests(); rig.Bridge.AdvanceBridgePhaseForTests(); rig.Scheduler.AdvanceSchedulerPhaseForTests(); rig.Player.AdvanceOneFixedTickForTests();
        }
        private static void MovePlayerIntoAuditRange(ManualRig rig) => Assert.That(rig.Player.Snapshot.PositionXQ4096, Is.GreaterThanOrEqualTo(-5 * 4096));
        private static TransferSimulationInput Press(SimulationTick tick)
        {
            var camera = new SimulationCameraPoseSnapshot(new SimulationTick(tick.Value - 1), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
            var aim = new AimSample(9901, AimSource.GamepadStick, null, null, null, 4096, 0, camera.CameraPoseTick, tick);
            return new TransferSimulationInput(tick, camera, aim, new TransferPressed(9901, tick), Array.Empty<TransferTargetRemoved>(), null);
        }
        private static void DisableAuthoredAutoDrivers(GameObject root)
        {
            var camera = root.GetComponentInChildren<SimulationCameraDriver>();
            var router = root.GetComponentInChildren<InputRouter>();
            var requester = root.GetComponentInChildren<OrdanBossTerminalTransitionRequester>();
            Assert.That(camera, Is.Not.Null);
            Assert.That(router, Is.Not.Null);
            Assert.That(requester, Is.Not.Null);
            camera.enabled = false;
            router.enabled = false;
            requester.enabled = false;
        }
        private static ManualRig CreateManualRig()
        {
            var root = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath));
            var player = root.GetComponentInChildren<PlayerMovementController>(); var transfer = root.GetComponentInChildren<TransferSimulationDriver>();
            var combat = root.GetComponentInChildren<OrdanBossCombatSimulationDriver>(); var bridge = root.GetComponentInChildren<OrdanBossSimulationDriver>();
            var scheduler = root.GetComponentInChildren<OrdanBossTransferExposureScheduler>(); var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
            DisableAuthoredAutoDrivers(root);
            player.enabled = false; transfer.enabled = false; combat.enabled = false; bridge.enabled = false; scheduler.enabled = false; handoff.enabled = false;
            return new ManualRig(root, player, transfer, combat, bridge, scheduler, root.GetComponentInChildren<OrdanBossAuditTransferModifierSink>(true));
        }
        private readonly struct ManualRig
        {
            internal ManualRig(GameObject root, PlayerMovementController player, TransferSimulationDriver transfer, OrdanBossCombatSimulationDriver combat, OrdanBossSimulationDriver bridge, OrdanBossTransferExposureScheduler scheduler, OrdanBossAuditTransferModifierSink audit)
            { Root = root; Player = player; Transfer = transfer; Combat = combat; Bridge = bridge; Scheduler = scheduler; Audit = audit; }
            internal GameObject Root { get; } internal PlayerMovementController Player { get; } internal TransferSimulationDriver Transfer { get; }
            internal OrdanBossCombatSimulationDriver Combat { get; } internal OrdanBossSimulationDriver Bridge { get; } internal OrdanBossTransferExposureScheduler Scheduler { get; } internal OrdanBossAuditTransferModifierSink Audit { get; }
        }
    }
}
