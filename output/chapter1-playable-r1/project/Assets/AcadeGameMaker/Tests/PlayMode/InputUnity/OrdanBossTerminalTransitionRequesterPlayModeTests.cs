using System;
using System.Collections;
using System.Reflection;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-M5D1-002/003/004. These drive the authored graph phase-by-phase so
    // the requester is proven against actual handoff, combat, and router
    // receipts rather than a fabricated terminal view.
    internal sealed class OrdanBossTerminalTransitionRequesterPlayModeTests
    {
        [UnityEngine.SerializeField] private AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope _runtimeFixture=new AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope();
        [UnityEngine.TestTools.UnitySetUp] public System.Collections.IEnumerator BeginRuntimeCase(){_runtimeFixture.PrepareEnter();yield return new UnityEngine.TestTools.EnterPlayMode();_runtimeFixture.ConfirmEntered();yield return _runtimeFixture.RunBody(_runtimeFixture.BeginCase(false));}
        [UnityEngine.TestTools.UnityTearDown] public System.Collections.IEnumerator CloseRuntimeCase(){try{if(_runtimeFixture.OwnsRuntimeFixture && UnityEngine.Application.isPlaying)yield return _runtimeFixture.RunBody(_runtimeFixture.CloseCase(),true);}finally{}if(_runtimeFixture.OwnsRuntimeFixture){if(UnityEngine.Application.isPlaying)yield return new UnityEngine.TestTools.ExitPlayMode();_runtimeFixture.CaptureCleanupFailure(_runtimeFixture.CompleteExit);}_runtimeFixture.ThrowCleanupFailure();}
        

        private const string GraphPath = "Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab";
        private static readonly string[] TerminalCleanupTargetIds = { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" };

        [Test]
        public void AcM5D1002And003_ExactDeathRequestsOnlyTransitionAtNextTickAndCommitsLockedEmptyFrame()
        {
            var rig = CreateRig();
            try
            {
                InitializeRouterAndRequester(rig);

                AdvanceNormalTick(rig);
                var deathTick = rig.Player.NextExpectedTick;
                AdvanceDeathTick(rig, deathTick);
                Assert.That(rig.Handoff.LatestPresentationView.Value.IsTerminal, Is.True);
                Assert.That(rig.Router.CurrentReceipt.Value, Is.EqualTo(new InputFrameCommitReceipt(deathTick, 1, 0, InputMode.GameplayEnabled)));

                var requestedTick = rig.Player.NextExpectedTick;
                rig.Requester.AdvanceForTests();
                rig.Router.StepForTests();

                var receipt = rig.Router.CurrentReceipt;
                Assert.That(receipt.HasValue, Is.True);
                Assert.That(receipt.Value.Tick, Is.EqualTo(requestedTick));
                Assert.That(receipt.Value.Mode, Is.EqualTo(InputMode.Transition));
                Assert.That(receipt.Value.ModeEpoch, Is.EqualTo(1));
                Assert.That(rig.Player.IsInputGateLocked && rig.Transfer.IsInputGateLocked && rig.Combat.IsInputGateLocked, Is.True);
                Assert.That(((IDictionary)GetPrivate(rig.Player, "_commands")).Count, Is.Zero);
                var transferInputs = (IDictionary)GetPrivate(rig.Transfer, "_inputs");
                Assert.That(transferInputs.Count, Is.EqualTo(1));
                Assert.That(transferInputs.Contains(requestedTick.Value), Is.True);
                var transferInput = (TransferSimulationInput)transferInputs[requestedTick.Value];
                Assert.That(transferInput.Tick, Is.EqualTo(requestedTick));
                Assert.That(transferInput.Camera.HasValue, Is.False);
                Assert.That(transferInput.Aim.HasValue, Is.False);
                Assert.That(transferInput.Press.HasValue, Is.False);
                Assert.That(transferInput.Removals.Count, Is.EqualTo(TerminalCleanupTargetIds.Length));
                for (var index = 0; index < TerminalCleanupTargetIds.Length; index++)
                {
                    Assert.That(transferInput.Removals[index].Tick, Is.EqualTo(requestedTick));
                    Assert.That(transferInput.Removals[index].TargetId, Is.EqualTo(TerminalCleanupTargetIds[index]));
                }
                Assert.That(((ICollection)transferInput.ExposureEnds).Count, Is.Zero);
                Assert.That(transferInput.Lifecycle.HasValue, Is.False);

                var combatInputs = (IDictionary)GetPrivate(rig.Combat, "_inputs");
                Assert.That(combatInputs.Count, Is.Zero);

                rig.Transfer.AdvanceCoordinatedInputPhaseForTests();
                Assert.That(rig.Transfer.LatestPhasePublication.Value.Tick, Is.EqualTo(requestedTick));
                Assert.That(((IDictionary)GetPrivate(rig.Transfer, "_inputs")).Count, Is.Zero);

                // The requester observes only the router's authoritative final
                // receipt; neither the terminal teardown nor handoff is used as
                // a substitute completion authority.
                rig.Requester.AdvanceForTests();
                Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.True);
                var completion = rig.Requester.LatestCompletionReceipt.Value;
                Assert.That(completion.EncounterKey, Is.EqualTo("OrdanBossEncounterGraph"));
                Assert.That(completion.DeathSourceTick, Is.EqualTo(deathTick));
                Assert.That(completion.RequestedTick, Is.EqualTo(requestedTick));
                Assert.That(completion.CommittedTick, Is.EqualTo(requestedTick));
                Assert.That(completion.Mode, Is.EqualTo(InputMode.Transition));
                Assert.That(completion.ModeEpoch, Is.EqualTo(1));
                Assert.That(rig.Requester.IsClosed, Is.True);
            }
            finally { DestroyRig(rig); }
        }

        [Test]
        public void AcM5D1002And004_IncompleteOrConflictingEvidenceNeverProducesCompletionOrRetry()
        {
            var rig = CreateRig();
            try
            {
                InitializeRouterAndRequester(rig);
                AdvanceNormalTick(rig);

                // A normal authored handoff is not terminal evidence and must
                // leave the mode receipt untouched.
                rig.Requester.AdvanceForTests();
                Assert.That(rig.Router.CurrentReceipt.Value.Mode, Is.EqualTo(InputMode.GameplayEnabled));
                Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.False);

                var deathTick = rig.Player.NextExpectedTick;
                AdvanceDeathTick(rig, deathTick);
                var requestedTick = rig.Player.NextExpectedTick;
                rig.Router.RequestMode(rig.Requester, InputMode.Cutscene, "test-conflict", requestedTick);
                rig.Requester.AdvanceForTests();

                Assert.That(rig.Requester.IsClosed, Is.True);
                Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.False);
                rig.Router.StepForTests();
                Assert.That(rig.Router.CurrentReceipt.Value.Mode, Is.EqualTo(InputMode.Cutscene));
                Assert.That(rig.Router.CurrentReceipt.Value.Tick, Is.EqualTo(requestedTick));
                Assert.Throws<InvalidOperationException>(() => rig.Requester.AdvanceForTests());
                Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.False);
            }
            finally { DestroyRig(rig); }
        }

        [Test]
        public void AcM5D1001_TransferCrossWireRejectsRequesterInitializationWithoutRepair()
        {
            var rig = CreateRig();
            var foreignRoot = new GameObject("foreign-transfer-root");
            try
            {
                var foreignTransfer = foreignRoot.AddComponent<TransferSimulationDriver>();
                SetPrivate(rig.Router, "_transfer", foreignTransfer);

                Assert.Throws<InvalidOperationException>(() => rig.Requester.InitializeForTests());
                Assert.That(GetPrivate(rig.Router, "_transfer"), Is.SameAs(foreignTransfer));
                Assert.That(rig.Router.CurrentReceipt.HasValue, Is.False);
                Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.False);
                Assert.That(rig.Requester.IsClosed, Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(foreignRoot);
                DestroyRig(rig);
            }
        }

        [Test]
        public void AcM5D1001_RuntimeCloneStartAcceptsExactBindingsWithoutPrematureAuthority()
        {
            var rig = CreateRig(false);
            try
            {
                Assert.That(rig.Root.name, Is.EqualTo(OrdanBossTerminalTransitionRequester.EncounterKey + "(Clone)"));
                InvokeLifecycle(rig.Requester, "Start");

                Assert.That(rig.Requester.IsClosed, Is.False);
                Assert.That(GetPrivate(rig.Router, "_modeRequester"), Is.SameAs(rig.Requester));
                Assert.That(rig.Router.CurrentReceipt.HasValue, Is.False);
                rig.Requester.AdvanceForTests();
                Assert.That(GetPrivate(rig.Router, "_pendingModeRequest"), Is.Null);
                Assert.That(GetPrivate(rig.Requester, "_requestIssued"), Is.False);
                Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.False);
            }
            finally { DestroyRig(rig); }
        }

        [Test]
        public void AcM5D1002_RuntimeCrossWireStartClosesWithoutRegistrationOrRequest()
        {
            var rig = CreateRig(false);
            var foreignRoot = new GameObject("foreign-runtime-transfer-root");
            try
            {
                var foreignTransfer = foreignRoot.AddComponent<TransferSimulationDriver>();
                SetPrivate(rig.Router, "_transfer", foreignTransfer);

                InvokeLifecycle(rig.Requester, "Start");
                Assert.That(GetPrivate(rig.Router, "_transfer"), Is.SameAs(foreignTransfer));
                Assert.That(rig.Requester.IsClosed, Is.True);
                Assert.That(GetPrivate(rig.Router, "_modeRequester"), Is.Null);
                Assert.That(GetPrivate(rig.Router, "_pendingModeRequest"), Is.Null);
                Assert.That(GetPrivate(rig.Requester, "_requestIssued"), Is.False);
                Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(foreignRoot);
                DestroyRig(rig);
            }
        }

        [Test]
        public void AcM5D1004_DuplicateAdvanceBeforeRouterCommitIsOneShotAndPublishesNoForgedReceipt()
        {
            var rig = CreateTerminalReadyRig(out var deathTick);
            try
            {
                var requestedTick = rig.Player.NextExpectedTick;
                rig.Requester.AdvanceForTests();
                // Re-entry before -210 must only observe the still-current
                // receipt; it cannot submit a second mode request or close.
                rig.Requester.AdvanceForTests();
                Assert.That(rig.Requester.IsClosed, Is.False);
                Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.False);

                rig.Router.StepForTests();
                rig.Requester.AdvanceForTests();
                Assert.That(rig.Router.CurrentReceipt.Value.Tick, Is.EqualTo(requestedTick));
                Assert.That(rig.Router.CurrentReceipt.Value.Mode, Is.EqualTo(InputMode.Transition));
                Assert.That(rig.Requester.LatestCompletionReceipt.Value.DeathSourceTick, Is.EqualTo(deathTick));
                Assert.That(rig.Requester.IsClosed, Is.True);
                Assert.Throws<InvalidOperationException>(() => rig.Requester.AdvanceForTests());
            }
            finally { DestroyRig(rig); }
        }

        [Test]
        public void AcM5D1004_DisableThenReenableClosesWithoutCompletionOrModeRetry()
        {
            var rig = CreateTerminalReadyRig(out _);
            try
            {
                InvokeLifecycle(rig.Requester, "OnDisable");
                Assert.That(rig.Requester.IsClosed, Is.True);
                Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.False);
                Assert.Throws<TargetInvocationException>(() => InvokeLifecycle(rig.Requester, "OnEnable"));
                Assert.Throws<InvalidOperationException>(() => rig.Requester.AdvanceForTests());
                Assert.That(rig.Router.CurrentReceipt.Value.Mode, Is.EqualTo(InputMode.GameplayEnabled));
                Assert.That(GetPrivate(rig.Router, "_pendingModeRequest"), Is.Null);
            }
            finally { DestroyRig(rig); }
        }

        [Test]
        public void AcM5D1004_SourceTickOverflowClosesWithoutRequestOrCompletion()
        {
            var rig = CreateTerminalReadyRig(out _);
            try
            {
                RewriteTerminalEvidenceTick(rig, new SimulationTick(int.MaxValue));
                var prior = rig.Router.CurrentReceipt;

                rig.Requester.AdvanceForTests();

                Assert.That(rig.Requester.IsClosed, Is.True);
                Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.False);
                Assert.That(rig.Router.CurrentReceipt, Is.EqualTo(prior));
                Assert.That(GetPrivate(rig.Router, "_pendingModeRequest"), Is.Null);
                Assert.That(GetPrivate(rig.Requester, "_requestIssued"), Is.False);
            }
            finally { DestroyRig(rig); }
        }

        [Test]
        public void AcM5D1004_RouterFaultNeverStagesARequestOrForgedCompletion()
        {
            var rig = CreateTerminalReadyRig(out _);
            try
            {
                SetPrivate(rig.Router, "_faulted", true);
                var prior = rig.Router.CurrentReceipt;
                rig.Requester.AdvanceForTests();
                rig.Requester.AdvanceForTests();

                Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.False);
                Assert.That(rig.Router.CurrentReceipt, Is.EqualTo(prior));
                Assert.That(GetPrivate(rig.Router, "_pendingModeRequest"), Is.Null);
                Assert.That(GetPrivate(rig.Requester, "_requestIssued"), Is.False);
            }
            finally { DestroyRig(rig); }
        }

        private static RuntimeRig CreateRig(bool restoreAuthoredRootName = true)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath);
            Assert.That(asset, Is.Not.Null);
            var root = UnityEngine.Object.Instantiate(asset);
            if (restoreAuthoredRootName) root.name = OrdanBossTerminalTransitionRequester.EncounterKey;
            var player = root.GetComponentInChildren<PlayerMovementController>();
            var transfer = root.GetComponentInChildren<TransferSimulationDriver>();
            var combat = root.GetComponentInChildren<OrdanBossCombatSimulationDriver>();
            var bridge = root.GetComponentInChildren<OrdanBossSimulationDriver>();
            var scheduler = root.GetComponentInChildren<OrdanBossTransferExposureScheduler>();
            var hostile = root.GetComponentInChildren<OrdanBossHostileDamageProducer>();
            var pull = root.GetComponentInChildren<OrdanBossBalanceAuditPullProducer>();
            var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
            var router = root.GetComponentInChildren<InputRouter>();
            var requester = root.GetComponentInChildren<OrdanBossTerminalTransitionRequester>();
            Assert.That(player, Is.Not.Null); Assert.That(transfer, Is.Not.Null); Assert.That(combat, Is.Not.Null);
            Assert.That(bridge, Is.Not.Null); Assert.That(scheduler, Is.Not.Null); Assert.That(hostile, Is.Not.Null); Assert.That(pull, Is.Not.Null);
            Assert.That(handoff, Is.Not.Null); Assert.That(router, Is.Not.Null); Assert.That(requester, Is.Not.Null);

            // Manual phase driving prevents Unity scheduling from masking the
            // required -211 -> -210 edge.
            player.enabled = false; transfer.enabled = false; combat.enabled = false; bridge.enabled = false;
            scheduler.enabled = false; hostile.enabled = false; pull.enabled = false; handoff.enabled = false;
            router.enabled = false; requester.enabled = false;
            return new RuntimeRig(root, player, transfer, combat, bridge, scheduler, hostile, pull, handoff, router, requester);
        }

        private static void InitializeRouterAndRequester(RuntimeRig rig)
        {
            rig.Router.InitializeForTests();
            rig.Requester.InitializeForTests();
            Assert.That(rig.Router.CurrentReceipt.HasValue, Is.False);
            Assert.That(rig.Requester.LatestCompletionReceipt.HasValue, Is.False);
        }

        private static RuntimeRig CreateTerminalReadyRig(out SimulationTick deathTick)
        {
            var rig = CreateRig();
            try
            {
                InitializeRouterAndRequester(rig);
                AdvanceNormalTick(rig);
                deathTick = rig.Player.NextExpectedTick;
                AdvanceDeathTick(rig, deathTick);
                Assert.That(rig.Handoff.LatestPresentationView.Value.IsTerminal, Is.True);
                return rig;
            }
            catch
            {
                DestroyRig(rig);
                throw;
            }
        }

        private static void AdvanceNormalTick(RuntimeRig rig)
        {
            var tick = rig.Player.NextExpectedTick;
            rig.Requester.AdvanceForTests();
            rig.Router.StepForTests();
            AdvancePhases(rig, tick);
            Assert.That(rig.Handoff.LatestPresentationView.Value.IsTerminal, Is.False);
        }

        private static void AdvanceDeathTick(RuntimeRig rig, SimulationTick deathTick)
        {
            rig.Requester.AdvanceForTests();
            rig.Router.StepForTests();
            rig.Transfer.AdvanceCoordinatedInputPhaseForTests();
            Assert.That(rig.Transfer.LatestPhasePublication.Value.Tick, Is.EqualTo(deathTick));
            rig.Combat.Submit(new OrdanBossCombatInput(deathTick, null, null, null,
                new[] { new DamageRequest("m5d1/requester-terminal", "test", "ordan", 60, DamageKind.BossPattern, deathTick) }));
            Assert.That(rig.Combat.AdvanceCombatPhaseForTests().Tick, Is.EqualTo(deathTick));
            Assert.That(rig.Bridge.AdvanceBridgePhaseForTests().SourceTick, Is.EqualTo(deathTick));
            Assert.That(rig.Scheduler.AdvanceSchedulerPhaseForTests().SourceTick, Is.EqualTo(deathTick));
            Assert.That(rig.Hostile.AdvanceForTests().Tick, Is.EqualTo(deathTick));
            Assert.That(rig.Pull.AdvanceForTests().SourceTick, Is.EqualTo(deathTick));
            Assert.That(rig.Player.AdvanceOneFixedTickForTests().Tick, Is.EqualTo(deathTick));
            InvokeFixed(rig.Handoff);
        }

        private static void AdvancePhases(RuntimeRig rig, SimulationTick tick)
        {
            rig.Transfer.AdvanceCoordinatedInputPhaseForTests();
            Assert.That(rig.Transfer.LatestPhasePublication.Value.Tick, Is.EqualTo(tick));
            Assert.That(rig.Combat.AdvanceCombatPhaseForTests().Tick, Is.EqualTo(tick));
            Assert.That(rig.Bridge.AdvanceBridgePhaseForTests().SourceTick, Is.EqualTo(tick));
            Assert.That(rig.Scheduler.AdvanceSchedulerPhaseForTests().SourceTick, Is.EqualTo(tick));
            Assert.That(rig.Hostile.AdvanceForTests().Tick, Is.EqualTo(tick));
            Assert.That(rig.Pull.AdvanceForTests().SourceTick, Is.EqualTo(tick));
            Assert.That(rig.Player.AdvanceOneFixedTickForTests().Tick, Is.EqualTo(tick));
            InvokeFixed(rig.Handoff);
        }

        private static void InvokeFixed(MonoBehaviour value)
        {
            var method = value.GetType().GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(value, null);
        }

        private static void InvokeLifecycle(MonoBehaviour value, string methodName)
        {
            var method = value.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(value, null);
        }

        private static void RewriteTerminalEvidenceTick(RuntimeRig rig, SimulationTick tick)
        {
            var outcome = RewriteAutoProperty(rig.Combat.LatestOutcome, "<Tick>k__BackingField", tick);
            SetPrivate(rig.Combat, "_latestOutcome", outcome);

            var view = rig.Handoff.LatestPresentationView.Value;
            var handoffs = new OrdanBossHandoff[view.Handoffs.Count];
            for (var index = 0; index < handoffs.Length; index++)
                handoffs[index] = RewriteAutoProperty(view.Handoffs[index], "<Tick>k__BackingField", tick);
            view = RewriteAutoProperty(view, "<Tick>k__BackingField", tick);
            view = RewriteAutoProperty(view, "<Handoffs>k__BackingField", Array.AsReadOnly(handoffs));
            SetPrivate(rig.Handoff, "_latest", (OrdanBossEncounterPresentationView?)view);
            // REQ-M5D7PA-006: this is an intentional coherent overflow source
            // row, not a forged single-field receipt. Keep the new independent
            // publication proof exactly aligned with the test receipt.
            var receipt = (InputFrameCommitReceipt?)new InputFrameCommitReceipt(tick, 1, 0, InputMode.GameplayEnabled);
            SetPrivate(rig.Router, "_uiPublicationProof", receipt);
            SetPrivate(rig.Router, "_currentReceipt", receipt);
        }

        private static T RewriteAutoProperty<T>(T value, string field, object replacement) where T : struct
        {
            object boxed = value;
            var backing = typeof(T).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(backing, Is.Not.Null);
            backing.SetValue(boxed, replacement);
            return (T)boxed;
        }

        private static object GetPrivate(object target, string field)
        {
            var value = target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(value, Is.Not.Null);
            return value.GetValue(target);
        }

        private static void SetPrivate(object target, string field, object value)
        {
            var targetField = target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(targetField, Is.Not.Null);
            targetField.SetValue(target, value);
        }

        private static void DestroyRig(RuntimeRig rig)
        {
            if (rig.Root != null) UnityEngine.Object.DestroyImmediate(rig.Root);
        }

        private readonly struct RuntimeRig
        {
            internal RuntimeRig(GameObject root, PlayerMovementController player, TransferSimulationDriver transfer,
                OrdanBossCombatSimulationDriver combat, OrdanBossSimulationDriver bridge, OrdanBossTransferExposureScheduler scheduler,
                OrdanBossHostileDamageProducer hostile, OrdanBossBalanceAuditPullProducer pull, OrdanBossEncounterHandoffAdapter handoff,
                InputRouter router, OrdanBossTerminalTransitionRequester requester)
            {
                Root = root; Player = player; Transfer = transfer; Combat = combat; Bridge = bridge; Scheduler = scheduler;
                Hostile = hostile; Pull = pull; Handoff = handoff; Router = router; Requester = requester;
            }

            internal GameObject Root { get; } internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Transfer { get; } internal OrdanBossCombatSimulationDriver Combat { get; }
            internal OrdanBossSimulationDriver Bridge { get; } internal OrdanBossTransferExposureScheduler Scheduler { get; }
            internal OrdanBossHostileDamageProducer Hostile { get; } internal OrdanBossBalanceAuditPullProducer Pull { get; }
            internal OrdanBossEncounterHandoffAdapter Handoff { get; } internal InputRouter Router { get; }
            internal OrdanBossTerminalTransitionRequester Requester { get; }
        }
    }
}
