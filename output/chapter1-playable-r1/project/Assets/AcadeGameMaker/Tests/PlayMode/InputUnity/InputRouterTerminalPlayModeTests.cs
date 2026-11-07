using System;
using System.Collections;
using System.Reflection;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-UX-004/006/008; AC-M5B5-002/007/008. This fixture reuses the
    // authored terminal graph instead of manufacturing a terminal receipt.
    internal sealed class InputRouterTerminalPlayModeTests : InputTestFixture
    {
        [UnityEngine.SerializeField] private AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope _runtimeFixture=new AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope();
        [UnityEngine.TestTools.UnitySetUp] public System.Collections.IEnumerator BeginRuntimeCase(){_runtimeFixture.PrepareEnter();yield return new UnityEngine.TestTools.EnterPlayMode();_runtimeFixture.ConfirmEntered();yield return _runtimeFixture.RunBody(_runtimeFixture.BeginCase(false));}
        public override void TearDown(){}
        [UnityEngine.TestTools.UnityTearDown] public System.Collections.IEnumerator CloseRuntimeCase(){try{if(_runtimeFixture.OwnsRuntimeFixture && UnityEngine.Application.isPlaying)yield return _runtimeFixture.RunBody(_runtimeFixture.CloseCase(),true);}finally{_runtimeFixture.CaptureCleanupFailure(()=>base.TearDown());}if(_runtimeFixture.OwnsRuntimeFixture){if(UnityEngine.Application.isPlaying)yield return new UnityEngine.TestTools.ExitPlayMode();_runtimeFixture.CaptureCleanupFailure(_runtimeFixture.CompleteExit);}_runtimeFixture.ThrowCleanupFailure();}
        

        [Test]
        public void TerminalDiscardOverflowDoesNotCloseProofOrRemoveEmptyInputAndDelivery()
        {
            var rig = PrepareTerminal();
            try
            {
                rig.Combat.Submit(new OrdanBossCombatInput(rig.CleanupTick, null, null, null, Array.Empty<DamageRequest>()));
                var router = CreateRouter(rig, InputMode.Transition);
                router.StepForTests();
                rig.Transfer.AdvanceCoordinatedInputPhaseForTests();
                var field = typeof(OrdanBossCombatSimulationDriver).GetField("_inputGeneration", BindingFlags.Instance | BindingFlags.NonPublic);
                var generation = (long)field.GetValue(rig.Combat);
                field.SetValue(rig.Combat, long.MaxValue);
                var receipt = router.CurrentReceipt;
                var failure = Assert.Catch<Exception>(() => InvokeFixed(rig.Teardown));
                Assert.That(failure.GetBaseException(), Is.TypeOf<OverflowException>());
                Assert.That(rig.Combat.IsCoordinatedInputLaneClosed, Is.False);
                Assert.That(rig.Combat.enabled, Is.True);
                Assert.That(rig.Teardown.LatestReceipt.HasValue, Is.False);
                Assert.That(Queue(rig.Combat).Count, Is.EqualTo(1));
                Assert.That(((IDictionary)typeof(OrdanBossCombatSimulationDriver).GetField("_deliveries", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(rig.Combat)).Count, Is.EqualTo(1));
                Assert.That(typeof(OrdanBossCombatSimulationDriver).GetField("_pendingCoordinatedReceipt", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(rig.Combat), Is.EqualTo(receipt));
                field.SetValue(rig.Combat, generation);
                InvokeFixed(rig.Teardown);
                rig.Player.AdvanceOneFixedTickForTests();
                Assert.That(rig.Combat.IsCoordinatedInputLaneClosed, Is.True);
            }
            finally { DestroyRoot(rig.Root); }
        }

        [Test]
        public void CompletedBossDeathRejectsGameplayBeforeAnyTerminalFrameMutation()
        {
            var rig = PrepareTerminal();
            try
            {
                var router = CreateRouter(rig, InputMode.GameplayEnabled);
                var transferQueue = Queue(rig.Transfer);
                var combatQueue = Queue(rig.Combat);

                Assert.Throws<InvalidOperationException>(() => router.StepForTests());
                Assert.That(router.CurrentReceipt.HasValue, Is.False);
                Assert.That(Queue(rig.Transfer), Is.SameAs(transferQueue));
                Assert.That(Queue(rig.Combat), Is.SameAs(combatQueue));
                Assert.That(rig.Combat.enabled, Is.True);
            }
            finally { DestroyRoot(rig.Root); }
        }

        [Test]
        public void LockedCleanupClosesCombatThenRouterRetiresItWhileMovementAndTransferContinueLocked()
        {
            var rig = PrepareTerminal();
            try
            {
                var router = CreateRouter(rig, InputMode.Transition);
                router.StepForTests();
                Assert.That(router.CurrentReceipt.Value.Tick, Is.EqualTo(rig.CleanupTick));

                rig.Transfer.AdvanceCoordinatedInputPhaseForTests();
                InvokeFixed(rig.Teardown);
                rig.Player.AdvanceOneFixedTickForTests();

                Assert.That(rig.Combat.enabled, Is.False);
                Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
                Assert.That(rig.Teardown.MatchesRetiredInputGraph(rig.Player, rig.Transfer, rig.Combat, rig.Player.NextExpectedTick), Is.True);

                router.StepForTests();
                rig.Transfer.AdvanceCoordinatedInputPhaseForTests();
                rig.Player.AdvanceOneFixedTickForTests();
                Assert.That(rig.Player.IsInputGateLocked && rig.Transfer.IsInputGateLocked, Is.True);

                var requester = new object();
                router.RegisterModeRequester(requester);
                router.RequestMode(requester, InputMode.GameplayEnabled, "terminal-reentry", rig.Player.NextExpectedTick);
                Assert.Throws<InvalidOperationException>(() => router.StepForTests());
            }
            finally { DestroyRoot(rig.Root); }
        }

        private static TerminalRig PrepareTerminal()
        {
            var assembly = Array.Find(AppDomain.CurrentDomain.GetAssemblies(), value =>
                string.Equals(value.GetName().Name, "AcadeGameMaker.Combat.Unity.PlayMode.Tests", StringComparison.Ordinal));
            Assert.That(assembly, Is.Not.Null, "The existing authored terminal fixture assembly must be loaded.");
            var fixture = assembly.GetType("AcadeGameMaker.Tests.PlayMode.CombatUnity.OrdanBossEncounterAuthoredGraphPlayModeTests", true);
            var create = fixture.GetMethod("CreateManualRig", BindingFlags.Static | BindingFlags.NonPublic);
            var fixedStep = fixture.GetMethod("ManualFixedStep", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(create, Is.Not.Null);
            Assert.That(fixedStep, Is.Not.Null);
            var raw = create.Invoke(null, null);
            var root = Value<GameObject>(raw, "Root");
            var player = Value<PlayerMovementController>(raw, "Player");
            var transfer = Value<TransferSimulationDriver>(raw, "Transfer");
            var combat = Value<OrdanBossCombatSimulationDriver>(raw, "Combat");
            var bridge = Value<OrdanBossSimulationDriver>(raw, "Bridge");
            var scheduler = Value<OrdanBossTransferExposureScheduler>(raw, "Scheduler");
            var hostile = Value<OrdanBossHostileDamageProducer>(raw, "Hostile");
            var pull = Value<OrdanBossBalanceAuditPullProducer>(raw, "AuditPull");
            var handoff = Value<OrdanBossEncounterHandoffAdapter>(raw, "Handoff");

            // Exact cut point: the original helper's final
            // `transfer.AdvanceTransferPhaseForTests()` is intentionally not run.
            fixedStep.Invoke(null, new[] { raw });
            var death = player.NextExpectedTick;
            transfer.AdvanceCoordinatedInputPhaseForTests();
            combat.Submit(new OrdanBossCombatInput(death, null, null, null,
                new[] { new DamageRequest("terminal-router-lethal", "test", "ordan", 60, DamageKind.BossPattern, death) }));
            combat.AdvanceCombatPhaseForTests();
            bridge.AdvanceBridgePhaseForTests();
            scheduler.AdvanceSchedulerPhaseForTests();
            hostile.AdvanceForTests();
            pull.AdvanceForTests();
            player.AdvanceOneFixedTickForTests();
            InvokeHandoff(handoff);
            var cleanup = player.NextExpectedTick;
            Assert.That(cleanup.Value, Is.EqualTo(death.Value + 1));

            var teardown = root.GetComponentInChildren<OrdanBossTerminalTeardown>();
            player.enabled = true; transfer.enabled = true; combat.enabled = true; bridge.enabled = true; scheduler.enabled = true; hostile.enabled = true; pull.enabled = true; handoff.enabled = true; teardown.enabled = true;
            foreach (var target in root.GetComponentsInChildren<CombatTarget>(true))
                if (target.Freeze().TargetId == "ordan") { target.enabled = true; target.Freeze().TargetCollider.enabled = true; }
            return new TerminalRig(
                root, player, transfer, combat, teardown, cleanup);
        }

        private static InputRouter CreateRouter(TerminalRig rig, InputMode mode)
        {
            var cameraObject = new GameObject("terminal-router-camera");
            cameraObject.transform.SetParent(rig.Root.transform);
            var camera = cameraObject.AddComponent<NoCameraProvider>();
            camera.Player = rig.Player;
            var routerObject = new GameObject("terminal-router");
            routerObject.transform.SetParent(rig.Root.transform);
            routerObject.SetActive(false);
            var router = routerObject.AddComponent<InputRouter>();
            router.ConfigureForAuthoring(rig.Player, rig.Transfer, rig.Combat, camera, mode);
            router.ConfigureTerminalTeardownForAuthoring(rig.Teardown);
            routerObject.SetActive(true);
            router.InitializeForTests();
            return router;
        }

        private static T Value<T>(object value, string name) => (T)value.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(value);
        private static IDictionary Queue(object driver) => (IDictionary)driver.GetType().GetField("_inputs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(driver);
        private static void InvokeFixed(OrdanBossTerminalTeardown teardown) => typeof(OrdanBossTerminalTeardown).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(teardown, null);
        private static void InvokeHandoff(OrdanBossEncounterHandoffAdapter handoff) => handoff.GetType().GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(handoff, null);
        private static void DestroyRoot(GameObject root) { if (root != null) UnityEngine.Object.DestroyImmediate(root); }

        private sealed class NoCameraProvider : MonoBehaviour, IInputCameraSnapshotProvider
        {
            internal PlayerMovementController Player { get; set; }
            public PlayerMovementController BoundPlayer => Player;
            public bool TryGetCompletedCamera(SimulationTick expectedPoseTick, out SimulationCameraPoseSnapshot camera)
            {
                camera = default(SimulationCameraPoseSnapshot);
                return false;
            }
        }

        private readonly struct TerminalRig
        {
            internal TerminalRig(GameObject root, PlayerMovementController player, TransferSimulationDriver transfer,
                OrdanBossCombatSimulationDriver combat, OrdanBossTerminalTeardown teardown, SimulationTick cleanupTick)
            { Root = root; Player = player; Transfer = transfer; Combat = combat; Teardown = teardown; CleanupTick = cleanupTick; }
            internal GameObject Root { get; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Transfer { get; }
            internal OrdanBossCombatSimulationDriver Combat { get; }
            internal OrdanBossTerminalTeardown Teardown { get; }
            internal SimulationTick CleanupTick { get; }
        }
    }
}
