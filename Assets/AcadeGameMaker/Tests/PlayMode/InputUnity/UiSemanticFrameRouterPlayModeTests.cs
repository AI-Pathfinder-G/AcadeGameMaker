using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-M5D7PA-001..004; AC-M5D7PA-001..004. Real generated-wrapper
    // callbacks reach only InputRouter and publish at the next fixed receipt.
    internal sealed class UiSemanticFrameRouterPlayModeTests : InputTestFixture
    {
        [Test]
        public void AC001_AC002_AC003_UiCallbacksPublishOneReceiptBoundFrame()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out var requester);
                Assert.That(router.CurrentUiFrame.HasValue, Is.True);
                Assert.That(router.CurrentUiFrame.Value.HasPoint, Is.False);
                // REQ-M5D7PA-005: creation consumes the existing UI receipt as
                // a baseline, so only the exact consecutive frame is delivered.
                var cursor = UiSemanticFrameCursorV1.Create(router);

                var keyboard = InputSystem.AddDevice<Keyboard>();
                var mouse = InputSystem.AddDevice<Mouse>();
                Press(keyboard.dKey); Set(mouse.position, new Vector2(-17.5f, 2048.5f));
                Press(mouse.leftButton); Press(keyboard.enterKey); Press(keyboard.escapeKey);
                InputSystem.QueueDeltaStateEvent(mouse.scroll, new Vector2(1.5f, -2.5f)); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);

                var frame = router.CurrentUiFrame.Value;
                UiSemanticFrameV1 delivered;
                Assert.That(cursor.TryAdvance(router, out delivered), Is.True);
                Assert.That(delivered, Is.EqualTo(frame));
                Assert.That(frame.Receipt, Is.EqualTo(router.CurrentReceipt.Value));
                Assert.That(frame.NavigateXQ4096, Is.EqualTo(4096)); Assert.That(frame.NavigateChanged, Is.True);
                Assert.That(frame.HasPoint, Is.True); Assert.That(frame.PointXActualPx, Is.EqualTo(-18)); Assert.That(frame.PointYActualPx, Is.EqualTo(2049));
                Assert.That(frame.ClickPressed, Is.True); Assert.That(frame.HasClickPoint, Is.True);
                Assert.That(frame.ClickPointXActualPx, Is.EqualTo(-18)); Assert.That(frame.ClickPointYActualPx, Is.EqualTo(2049));
                Assert.That(frame.ScrollXQ4096, Is.EqualTo(6144)); Assert.That(frame.ScrollYQ4096, Is.EqualTo(-10240));
                Assert.That(frame.SubmitPressed, Is.True); Assert.That(frame.CancelPressed, Is.True);
                Assert.That(router.LastDiagnostic.UnboundUi, Is.False);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [Test]
        public void AC004_ExitClearsPublicationAndReentryStartsEmptyEpoch()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out var requester);
                var staged = CaptureUiBatch(router);
                Assert.That(staged.HasPoint, Is.True);
                Assert.That(staged.ClickPressed, Is.True);
                Assert.That(staged.SubmitPressed, Is.True);
                Assert.That(staged.CancelPressed, Is.True);
                router.RequestMode(requester, InputMode.Transition, "test exit", rig.Player.NextExpectedTick);
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                Assert.That(router.CurrentUiFrame.HasValue, Is.False);
                Assert.That(router.CurrentReceipt.Value.Mode, Is.EqualTo(InputMode.Transition));
                AssertUiPendingEmpty(router);
                Assert.That(InputRouterPlayModeTests.GetPrivate<UiSemanticFrameV1?>(router, "_uiFramePublicationProof"), Is.Null);
                router.RequestMode(requester, InputMode.UIOnly, "test reentry", rig.Player.NextExpectedTick);
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                // UI Navigate/Point use Value actions. Their held old state is
                // eligible for the next update's initial-state check, which
                // must not repopulate this fresh mode epoch.
                InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var frame = router.CurrentUiFrame.Value;
                Assert.That(frame.NavigateXQ4096, Is.Zero); Assert.That(frame.NavigateYQ4096, Is.Zero); Assert.That(frame.NavigateChanged, Is.False);
                Assert.That(frame.HasPoint, Is.False); Assert.That(frame.PointXActualPx, Is.Zero); Assert.That(frame.PointYActualPx, Is.Zero); Assert.That(frame.PointChanged, Is.False); Assert.That(frame.ClickPressed, Is.False);
                Assert.That(frame.HasClickPoint, Is.False); Assert.That(frame.ScrollXQ4096, Is.Zero); Assert.That(frame.ScrollYQ4096, Is.Zero);
                Assert.That(frame.ClickPointXActualPx, Is.Zero); Assert.That(frame.ClickPointYActualPx, Is.Zero);
                Assert.That(frame.SubmitPressed, Is.False); Assert.That(frame.CancelPressed, Is.False);
                AssertUiPendingEmpty(router);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [Test]
        public void AC001_PointReturnToFrameStartIsStickyThenPersistsWithoutReplay()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                var mouse = InputSystem.AddDevice<Mouse>();
                var start = new Vector2(-17.5f, 2048.5f);
                Set(mouse.position, start); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var baseline = router.CurrentUiFrame.Value;
                Assert.That(baseline.HasPoint, Is.True); Assert.That(baseline.PointXActualPx, Is.EqualTo(-18)); Assert.That(baseline.PointYActualPx, Is.EqualTo(2049));

                Set(mouse.position, new Vector2(313.5f, -9.5f)); InputSystem.Update();
                Set(mouse.position, start); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var returned = router.CurrentUiFrame.Value;
                Assert.That(returned.PointXActualPx, Is.EqualTo(baseline.PointXActualPx));
                Assert.That(returned.PointYActualPx, Is.EqualTo(baseline.PointYActualPx));
                Assert.That(returned.PointChanged, Is.True, "A within-frame return to the prior current value remains observable.");

                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var retained = router.CurrentUiFrame.Value;
                Assert.That(retained.HasPoint, Is.True);
                Assert.That(retained.PointXActualPx, Is.EqualTo(baseline.PointXActualPx));
                Assert.That(retained.PointYActualPx, Is.EqualTo(baseline.PointYActualPx));
                Assert.That(retained.PointChanged, Is.False);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [Test]
        public void AC004_UiReentryQuarantinesHeldAndSameUpdateFreshPointThenAcceptsFollowingFreshInput()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out var requester);
                var keyboard = InputSystem.AddDevice<Keyboard>();
                var mouse = InputSystem.AddDevice<Mouse>();
                Set(mouse.position, new Vector2(41.5f, -8.5f)); Press(keyboard.dKey); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                Assert.That(router.CurrentUiFrame.Value.NavigateXQ4096, Is.EqualTo(4096));
                Assert.That(router.CurrentUiFrame.Value.HasPoint, Is.True);

                router.RequestMode(requester, InputMode.Transition, "quarantine exit", rig.Player.NextExpectedTick);
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                router.RequestMode(requester, InputMode.UIOnly, "quarantine reentry", rig.Player.NextExpectedTick);
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiEnableQuarantinePending"), Is.True);
                Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiEnableQuarantineSubscribed"), Is.True);
                Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_captureSuppressed"), Is.True);

                // Held D and its old point both receive the UI Value action's
                // initial-state check.  Keep D down through this update; the
                // new Point fact intentionally shares it and is dropped too.
                Set(mouse.position, new Vector2(99.5f, -11.5f)); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var quarantined = router.CurrentUiFrame.Value;
                Assert.That(quarantined.NavigateXQ4096, Is.Zero); Assert.That(quarantined.NavigateYQ4096, Is.Zero); Assert.That(quarantined.NavigateChanged, Is.False);
                Assert.That(quarantined.HasPoint, Is.False); Assert.That(quarantined.PointChanged, Is.False);
                Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiEnableQuarantinePending"), Is.False);
                Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiEnableQuarantineSubscribed"), Is.False);
                Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_captureSuppressed"), Is.False);

                // The first post-quarantine release is a canceled Value
                // callback with current already zero; it cannot replay D.
                Release(keyboard.dKey); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var released = router.CurrentUiFrame.Value;
                Assert.That(released.NavigateXQ4096, Is.Zero); Assert.That(released.NavigateYQ4096, Is.Zero); Assert.That(released.NavigateChanged, Is.False);
                Assert.That(released.HasPoint, Is.False); Assert.That(released.PointChanged, Is.False);

                Press(keyboard.dKey); Set(mouse.position, new Vector2(-17.5f, 2048.5f)); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var fresh = router.CurrentUiFrame.Value;
                Assert.That(fresh.NavigateXQ4096, Is.EqualTo(4096)); Assert.That(fresh.NavigateChanged, Is.True);
                Assert.That(fresh.HasPoint, Is.True); Assert.That(fresh.PointXActualPx, Is.EqualTo(-18)); Assert.That(fresh.PointYActualPx, Is.EqualTo(2049)); Assert.That(fresh.PointChanged, Is.True);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [Test]
        public void AC002_TwoMouseClickKeepsCallbackDevicePointWhenLaterPointChanges()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                var first = InputSystem.AddDevice<Mouse>();
                var later = InputSystem.AddDevice<Mouse>();

                Set(first.position, new Vector2(-17.5f, 2048.5f)); InputSystem.Update();
                Press(first.leftButton); InputSystem.Update();
                // This later callback is intentionally from another device,
                // which also becomes the global current pointer in InputSystem.
                Set(later.position, new Vector2(313.5f, -9.5f)); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);

                var frame = router.CurrentUiFrame.Value;
                Assert.That(frame.ClickPressed, Is.True);
                Assert.That(frame.ClickPointXActualPx, Is.EqualTo(-18));
                Assert.That(frame.ClickPointYActualPx, Is.EqualTo(2049));
                Assert.That(frame.PointXActualPx, Is.EqualTo(314));
                Assert.That(frame.PointYActualPx, Is.EqualTo(-10));
                Assert.That(frame.ClickPointXActualPx, Is.Not.EqualTo(frame.PointXActualPx));
                Assert.That(frame.ClickPointYActualPx, Is.Not.EqualTo(frame.PointYActualPx));
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [Test]
        public void AC007_MalformedExpectedClickControlFaultsBeforeClickBatchMutation()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                var before = router.CurrentReceipt.Value;
                var actions = InputRouterPlayModeTests.GetPrivate<GameInputActions>(router, "_actions");
                actions.UI.Click.ApplyBindingOverride("<Mouse>/rightButton");
                var mouse = InputSystem.AddDevice<Mouse>();
                Set(mouse.position, new Vector2(77.5f, -31.5f)); InputSystem.Update();
                Press(mouse.rightButton); InputSystem.Update();

                Assert.That(router.IsFaulted, Is.True);
                Assert.That(router.LastDiagnostic.Fault, Is.EqualTo(InputRouterFaultStage.UiCapture));
                Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiClickPressed"), Is.False);
                Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiHasClickPoint"), Is.False);
                Assert.That(router.CurrentReceipt, Is.EqualTo(before));
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [Test]
        public void AC007_AlreadyFaultedGetterKeepsPriorStageButRejectsForgedPublication()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                var before = router.CurrentReceipt.Value;
                router.SetFaultStageForTests(InputRouterFaultStage.AfterMovementCommit);
                Assert.Throws<System.InvalidOperationException>(() => router.StepForTests());
                Assert.That(router.IsFaulted, Is.True);
                Assert.That(router.LastDiagnostic.Fault, Is.EqualTo(InputRouterFaultStage.AfterMovementCommit));
                // A coherent forensic row remains readable after the unrelated
                // terminal failure.
                Assert.That(router.CurrentReceipt, Is.EqualTo(before));

                var forged = DifferentValidFrame(before);
                typeof(InputRouter).GetField("_currentUiFrame", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(router, forged);
                Assert.Throws<System.InvalidOperationException>(() => { var ignored = router.CurrentReceipt; });
                Assert.That(router.LastDiagnostic.Fault, Is.EqualTo(InputRouterFaultStage.AfterMovementCommit));
                Assert.That(InputRouterPlayModeTests.GetPrivate<UiSemanticFrameV1?>(router, "_currentUiFrame"), Is.EqualTo(forged));
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [Test]
        public void AC001_AC004_NavigateCanceledStickyAndSameEpochCurrentAreExact()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                var keyboard = InputSystem.AddDevice<Keyboard>();
                var gamepad = InputSystem.AddDevice<Gamepad>();

                Press(keyboard.dKey); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var first = router.CurrentUiFrame.Value;
                Assert.That(first.NavigateXQ4096, Is.EqualTo(4096)); Assert.That(first.NavigateYQ4096, Is.Zero);
                Assert.That(first.NavigateChanged, Is.True);

                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var retained = router.CurrentUiFrame.Value;
                Assert.That(retained.NavigateXQ4096, Is.EqualTo(4096));
                Assert.That(retained.NavigateYQ4096, Is.Zero);
                Assert.That(retained.NavigateChanged, Is.False);

                Release(keyboard.dKey); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var canceled = router.CurrentUiFrame.Value;
                Assert.That(canceled.NavigateXQ4096, Is.Zero);
                Assert.That(canceled.NavigateYQ4096, Is.Zero);
                Assert.That(canceled.NavigateChanged, Is.True);

                Set(gamepad.leftStick, new Vector2(0f, 1f)); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var gamepadFrame = router.CurrentUiFrame.Value;
                Assert.That(gamepadFrame.NavigateXQ4096, Is.Zero); Assert.That(gamepadFrame.NavigateYQ4096, Is.EqualTo(4096));
                Assert.That(gamepadFrame.NavigateChanged, Is.True);
                Set(gamepad.leftStick, Vector2.zero); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                Assert.That(router.CurrentUiFrame.Value.NavigateYQ4096, Is.Zero);

                // REQ-M5D7PA-002: a same-frame return to the frame-start
                // current value is still an observable navigation change.
                Press(keyboard.dKey); InputSystem.Update(); Release(keyboard.dKey); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                Assert.That(router.CurrentUiFrame.Value.NavigateXQ4096, Is.Zero);
                Assert.That(router.CurrentUiFrame.Value.NavigateChanged, Is.True);
                Set(gamepad.leftStick, new Vector2(0f, 1f)); InputSystem.Update(); Set(gamepad.leftStick, Vector2.zero); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                Assert.That(router.CurrentUiFrame.Value.NavigateYQ4096, Is.Zero);
                Assert.That(router.CurrentUiFrame.Value.NavigateChanged, Is.True);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [Test]
        public void AC003_MultipleOpposingScrollAndCanceledEdgesCoalesceOnce()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                var keyboard = InputSystem.AddDevice<Keyboard>();
                var mouse = InputSystem.AddDevice<Mouse>();
                InputSystem.QueueDeltaStateEvent(mouse.scroll, new Vector2(1.5f, -2.5f)); InputSystem.Update();
                InputSystem.QueueDeltaStateEvent(mouse.scroll, new Vector2(-1.5f, 2.5f)); InputSystem.Update();
                Press(keyboard.enterKey); InputSystem.Update(); Release(keyboard.enterKey); InputSystem.Update();
                Press(keyboard.escapeKey); InputSystem.Update(); Release(keyboard.escapeKey); InputSystem.Update();
                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                var frame = router.CurrentUiFrame.Value;
                Assert.That(frame.ScrollXQ4096, Is.Zero); Assert.That(frame.ScrollYQ4096, Is.Zero);
                Assert.That(frame.SubmitPressed, Is.True); Assert.That(frame.CancelPressed, Is.True);

                router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                frame = router.CurrentUiFrame.Value;
                Assert.That(frame.ScrollXQ4096, Is.Zero); Assert.That(frame.ScrollYQ4096, Is.Zero);
                Assert.That(frame.SubmitPressed, Is.False); Assert.That(frame.CancelPressed, Is.False);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [TestCase(InputRouterFaultStage.PrepareMovement, false)]
        [TestCase(InputRouterFaultStage.PrepareTransfer, false)]
        [TestCase(InputRouterFaultStage.PrepareCombat, false)]
        [TestCase(InputRouterFaultStage.ValidateMovement, false)]
        [TestCase(InputRouterFaultStage.ValidateTransfer, false)]
        [TestCase(InputRouterFaultStage.ValidateCombat, false)]
        [TestCase(InputRouterFaultStage.CommitMovement, true)]
        [TestCase(InputRouterFaultStage.CommitTransfer, true)]
        [TestCase(InputRouterFaultStage.CommitCombat, true)]
        [TestCase(InputRouterFaultStage.AfterMovementCommit, true)]
        [TestCase(InputRouterFaultStage.AfterTransferCommit, true)]
        [TestCase(InputRouterFaultStage.AfterCombatCommit, true)]
        [TestCase(InputRouterFaultStage.MapSwitch, true)]
        [TestCase(InputRouterFaultStage.MapsDisabled, true)]
        [TestCase(InputRouterFaultStage.TargetMapEnabled, true)]
        public void AC005_FailureBoundariesPreservePreviousPublicationAndUiPendingBatch(InputRouterFaultStage stage, bool terminal)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out var requester);
                var previousReceipt = router.CurrentReceipt;
                var previousFrame = router.CurrentUiFrame;
                var pending = CaptureUiBatch(router);
                if (stage == InputRouterFaultStage.MapSwitch || stage == InputRouterFaultStage.MapsDisabled || stage == InputRouterFaultStage.TargetMapEnabled)
                {
                    router.RequestMode(requester, InputMode.Transition, "map failure", rig.Player.NextExpectedTick);
                }
                router.SetFaultStageForTests(stage);
                Assert.Throws<System.InvalidOperationException>(() => router.StepForTests());
                Assert.That(router.CurrentReceipt, Is.EqualTo(previousReceipt));
                Assert.That(router.CurrentUiFrame, Is.EqualTo(previousFrame));
                AssertPending(router, pending);
                if (terminal)
                {
                    Assert.That(router.IsFaulted, Is.True);
                    router.SetFaultStageForTests(null);
                    Assert.Throws<System.InvalidOperationException>(() => router.StepForTests());
                    Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiClickPressed"), Is.True);
                }
                else
                {
                    router.SetFaultStageForTests(null);
                    router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                    var recovered = router.CurrentUiFrame.Value;
                    Assert.That(recovered.NavigateXQ4096, Is.EqualTo(pending.NavigateXQ4096)); Assert.That(recovered.NavigateYQ4096, Is.EqualTo(pending.NavigateYQ4096));
                    Assert.That(recovered.NavigateChanged, Is.EqualTo(pending.NavigateChanged));
                    Assert.That(recovered.HasPoint, Is.EqualTo(pending.HasPoint)); Assert.That(recovered.PointXActualPx, Is.EqualTo(pending.PointXActualPx)); Assert.That(recovered.PointYActualPx, Is.EqualTo(pending.PointYActualPx)); Assert.That(recovered.PointChanged, Is.EqualTo(pending.PointChanged));
                    Assert.That(recovered.ClickPressed, Is.EqualTo(pending.ClickPressed)); Assert.That(recovered.HasClickPoint, Is.EqualTo(pending.HasClickPoint)); Assert.That(recovered.ClickPointXActualPx, Is.EqualTo(pending.ClickPointXActualPx)); Assert.That(recovered.ClickPointYActualPx, Is.EqualTo(pending.ClickPointYActualPx));
                    Assert.That(recovered.ScrollXQ4096, Is.EqualTo(pending.ScrollXQ4096)); Assert.That(recovered.ScrollYQ4096, Is.EqualTo(pending.ScrollYQ4096));
                    Assert.That(recovered.SubmitPressed, Is.EqualTo(pending.SubmitPressed)); Assert.That(recovered.CancelPressed, Is.EqualTo(pending.CancelPressed));
                }
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [TestCase("_currentReceipt", false)]
        [TestCase("_uiPublicationProof", false)]
        [TestCase("_currentUiFrame", true)]
        [TestCase("_uiFramePublicationProof", true)]
        [TestCase("_uiFramePublicationProof", false)]
        public void AC007_EachRouterPublicationPresenceRowFailsClosedWithoutNormalization(string field, bool validFrameReplacement)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                object corrupt = field == "_currentReceipt"
                    ? new InputFrameCommitReceipt(new SimulationTick(router.CurrentReceipt.Value.Tick.Value + 1), router.CurrentReceipt.Value.FrameOrdinal + 1, router.CurrentReceipt.Value.ModeEpoch, InputMode.UIOnly)
                    : validFrameReplacement ? DifferentValidFrame(router.CurrentReceipt.Value) : null;
                typeof(InputRouter).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(router, corrupt);
                Assert.Throws<System.InvalidOperationException>(() => { var ignored = router.CurrentReceipt; }, field);
                Assert.That(router.IsFaulted, Is.True, field);
                Assert.That(router.LastDiagnostic.Fault, Is.EqualTo(InputRouterFaultStage.UiCapture), field);
                Assert.That(typeof(InputRouter).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router), Is.EqualTo(corrupt), field + " forensic row");
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [Test]
        public void AC007_AlternateWellFormedUiPublicationProofFailsClosedAndIsPreserved()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                var previousReceipt = router.CurrentReceipt;
                var previousFrame = router.CurrentUiFrame;
                var forged = new InputFrameCommitReceipt(new SimulationTick(previousReceipt.Value.Tick.Value + 1), previousReceipt.Value.FrameOrdinal + 1, previousReceipt.Value.ModeEpoch, InputMode.UIOnly);
                SetPrivate(router, "_uiPublicationProof", forged);
                Assert.Throws<System.InvalidOperationException>(() => { var ignored = router.CurrentReceipt; });
                Assert.That(router.IsFaulted, Is.True);
                Assert.That(router.LastDiagnostic.Fault, Is.EqualTo(InputRouterFaultStage.UiCapture));
                Assert.That(InputRouterPlayModeTests.GetPrivate<InputFrameCommitReceipt?>(router, "_currentReceipt"), Is.EqualTo(previousReceipt));
                Assert.That(InputRouterPlayModeTests.GetPrivate<UiSemanticFrameV1?>(router, "_currentUiFrame"), Is.EqualTo(previousFrame));
                Assert.That(InputRouterPlayModeTests.GetPrivate<InputFrameCommitReceipt?>(router, "_uiPublicationProof"), Is.EqualTo((InputFrameCommitReceipt?)forged));
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [TestCase("Point", 0)]
        [TestCase("Point", 1)]
        [TestCase("Point", -1)]
        [TestCase("Scroll", 0)]
        [TestCase("Scroll", 1)]
        [TestCase("Scroll", -1)]
        public void AC007_NonFiniteActualUiPointAndScrollCallbacksFaultWithoutMutatingPendingBatch(string callback, int sign)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                var beforeReceipt = router.CurrentReceipt;
                var beforeFrame = router.CurrentUiFrame;
                var pending = CaptureUiBatch(router);
                var value = NonFinite(sign);
                var mouse = InputSystem.AddDevice<Mouse>();
                if (callback == "Point") Set(mouse.position, new Vector2(value, 0f));
                else InputSystem.QueueDeltaStateEvent(mouse.scroll, new Vector2(value, 0f));
                InputSystem.Update();
                AssertUiCaptureTerminal(router, beforeReceipt, beforeFrame, pending);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        public void AC007_ActualUiNavigateNonFiniteProcessorCallbackFaultsWithoutMutatingPendingBatch(int sign)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                ForcedNonFiniteNavigateProcessor.ResetInstrumentation();
                // Gamepad state injection sanitizes non-finite values before a
                // Value action receives them.  Instead, use a test-only
                // processor on exactly the Gamepad Navigate binding: a finite
                // raw stick event selects the real binding, then the action
                // callback receives the requested non-finite Vector2.
                InputSystem.RegisterProcessor<ForcedNonFiniteNavigateProcessor>("m5d7paForcedNonFiniteNavigate");
                var navigate = InputRouterPlayModeTests.GetPrivate<GameInputActions>(router, "_actions").UI.Navigate;
                navigate.ApplyBindingOverride(new InputBinding
                {
                    path = "<Gamepad>/leftStick",
                    overrideProcessors = "m5d7paForcedNonFiniteNavigate(sign=" + sign + ")"
                });
                var gamepad = InputSystem.AddDevice<Gamepad>();
                Set(gamepad.leftStick, Vector2.zero); InputSystem.Update();

                var beforeReceipt = router.CurrentReceipt;
                var beforeFrame = router.CurrentUiFrame;
                // A held Keyboard D has the same raw magnitude as up and wins
                // the Value-action tie-breaker, which would leave the Gamepad
                // binding unselected.  Keep all other semantic facts but omit
                // Navigate so the finite Gamepad event is the sole driver.
                var pending = CaptureUiBatchExcluding(router, "Navigate");
                var processCallsBeforeTarget = ForcedNonFiniteNavigateProcessor.ProcessCallCount;
                var outputsBeforeTarget = ForcedNonFiniteNavigateProcessor.NonFiniteOutputCount;
                Set(gamepad.leftStick, Vector2.up); InputSystem.Update();
                Assert.That(ForcedNonFiniteNavigateProcessor.ProcessCallCount, Is.GreaterThan(processCallsBeforeTarget), "Navigate binding processor was invoked for the target event.");
                Assert.That(ForcedNonFiniteNavigateProcessor.NonFiniteOutputCount, Is.GreaterThan(outputsBeforeTarget), "Navigate callback ReadValue received the test processor's non-finite output.");
                AssertUiCaptureTerminal(router, beforeReceipt, beforeFrame, pending);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [TestCase(float.MaxValue)]
        [TestCase(float.MinValue)]
        public void AC007_UnrepresentableActualUiPixelsFaultWithoutMutatingPendingBatch(float value)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                var beforeReceipt = router.CurrentReceipt; var beforeFrame = router.CurrentUiFrame; var pending = CaptureUiBatch(router);
                var mouse = InputSystem.AddDevice<Mouse>();
                Set(mouse.position, new Vector2(value, 0f)); InputSystem.Update();
                AssertUiCaptureTerminal(router, beforeReceipt, beforeFrame, pending);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [Test]
        public void AC007_ActualUiScrollConversionAndAggregateOverflowFaultWithoutPartialMutation()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                var beforeReceipt = router.CurrentReceipt; var beforeFrame = router.CurrentUiFrame; var pending = CaptureUiBatch(router);
                var mouse = InputSystem.AddDevice<Mouse>();
                InputSystem.QueueDeltaStateEvent(mouse.scroll, new Vector2(float.MaxValue, 0f)); InputSystem.Update();
                AssertUiCaptureTerminal(router, beforeReceipt, beforeFrame, pending);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }

            objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                var beforeReceipt = router.CurrentReceipt; var beforeFrame = router.CurrentUiFrame; CaptureUiBatch(router);
                SetPrivate(router, "_uiScrollXQ4096", long.MaxValue);
                var pending = SnapshotPending(router);
                var mouse = InputSystem.AddDevice<Mouse>();
                InputSystem.QueueDeltaStateEvent(mouse.scroll, new Vector2(1f, 0f)); InputSystem.Update();
                AssertUiCaptureTerminal(router, beforeReceipt, beforeFrame, pending);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [TestCase("Navigate")]
        [TestCase("Point")]
        [TestCase("Click")]
        [TestCase("Scroll")]
        [TestCase("Submit")]
        [TestCase("Cancel")]
        public void AC007_CallbackOrdinalOverflowOnEveryUiCallbackPreservesEarlierPendingFacts(string callback)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                var router = EnterUi(objects, rig, out _);
                // All row devices and their initial-state checks settle before
                // the frozen batch and before the ordinal reaches MaxValue.
                // The only callback after MaxValue is the named target below.
                var keyboard = InputSystem.AddDevice<Keyboard>();
                var mouse = InputSystem.AddDevice<Mouse>();
                Gamepad gamepad = callback == "Navigate" ? InputSystem.AddDevice<Gamepad>() : null;
                Set(mouse.position, Vector2.zero);
                if (gamepad != null) Set(gamepad.leftStick, Vector2.zero);
                InputSystem.Update();

                var beforeReceipt = router.CurrentReceipt; var beforeFrame = router.CurrentUiFrame;
                CaptureUiBatchExcluding(router, callback, keyboard, mouse);
                if (callback != "Navigate") Release(keyboard.dKey);
                if (callback != "Click") Release(mouse.leftButton);
                if (callback != "Submit") Release(keyboard.enterKey);
                if (callback != "Cancel") Release(keyboard.escapeKey);
                // Return every stateful baseline control to neutral before
                // freezing the pending facts.  The callbacks remain strictly
                // before MaxValue; their resulting valid snapshot is the one
                // that must survive the target callback's failed successor.
                Set(mouse.position, Vector2.zero);
                InputSystem.QueueDeltaStateEvent(mouse.scroll, Vector2.zero);
                InputSystem.Update();
                var pending = SnapshotPending(router);
                SetPrivate(router, "_callbackOrdinal", long.MaxValue);
                if (callback == "Navigate") Set(gamepad.leftStick, new Vector2(0f, 1f));
                else if (callback == "Point") Set(mouse.position, new Vector2(99.5f, -11.5f));
                else if (callback == "Click") Press(mouse.leftButton);
                else if (callback == "Scroll") InputSystem.QueueDeltaStateEvent(mouse.scroll, new Vector2(1f, 0f));
                else if (callback == "Submit") Press(keyboard.enterKey);
                else Press(keyboard.escapeKey);
                InputSystem.Update();
                AssertUiCaptureTerminal(router, beforeReceipt, beforeFrame, pending);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        [TestCase("Disable")]
        [TestCase("Destroy")]
        [TestCase("MapSwitchFault")]
        [TestCase("UiExit")]
        public void AC004_UiEnableQuarantineHookCleansUpBeforeItsAfterUpdateBoundary(string boundary)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = InputRouterPlayModeTests.CreateRig(objects);
                InputRouter router;
                if (boundary == "MapSwitchFault")
                {
                    router = InputRouterPlayModeTests.CreateRouter(objects, rig, InputMode.GameplayEnabled);
                    router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                    var requester = new object(); router.RegisterModeRequester(requester);
                    router.RequestMode(requester, InputMode.UIOnly, "faulting ui enable", rig.Player.NextExpectedTick);
                    router.SetFaultStageForTests(InputRouterFaultStage.TargetMapEnabled);
                    Assert.Throws<System.InvalidOperationException>(() => router.StepForTests());
                }
                else
                {
                    router = EnterUi(objects, rig, out var requester, false);
                    if (boundary == "Disable") router.gameObject.SetActive(false);
                    else if (boundary == "Destroy") UnityEngine.Object.DestroyImmediate(router.gameObject);
                    else
                    {
                        // A successful exit must remove its pending hook before
                        // the next entry registers one new, independent hook.
                        router.RequestMode(requester, InputMode.Transition, "quarantine cleanup exit", rig.Player.NextExpectedTick);
                        router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                        Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiEnableQuarantinePending"), Is.False, "exit pending");
                        Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiEnableQuarantineSubscribed"), Is.False, "exit subscribed");
                        Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_captureSuppressed"), Is.False, "exit capture");

                        router.RequestMode(requester, InputMode.UIOnly, "quarantine cleanup reentry", rig.Player.NextExpectedTick);
                        router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
                        Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiEnableQuarantinePending"), Is.True, "fresh pending");
                        Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiEnableQuarantineSubscribed"), Is.True, "fresh subscribed");
                        Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_captureSuppressed"), Is.True, "fresh capture");
                    }
                }
                InputSystem.Update();
                Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiEnableQuarantinePending"), Is.False, boundary);
                Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiEnableQuarantineSubscribed"), Is.False, boundary);
                Assert.That(InputRouterPlayModeTests.GetPrivate<bool>(router, "_captureSuppressed"), Is.False, boundary);
            }
            finally { InputRouterPlayModeTests.DestroyAll(objects); }
        }

        private UiPendingSnapshot CaptureUiBatch(InputRouter router)
        {
            return CaptureUiBatchExcluding(router, null);
        }

        // Keep a nonempty, valid pending batch while leaving the target edge
        // untouched.  This ensures the subsequent real callback reaches its
        // checked ordinal successor rather than returning because the edge was
        // already coalesced in the current fixed-frame batch.
        private UiPendingSnapshot CaptureUiBatchExcluding(InputRouter router, string excluded, Keyboard keyboard = null, Mouse mouse = null)
        {
            var batchKeyboard = keyboard ?? InputSystem.AddDevice<Keyboard>();
            var batchMouse = mouse ?? InputSystem.AddDevice<Mouse>();
            if (excluded != "Navigate") { Press(batchKeyboard.dKey); InputSystem.Update(); }
            Set(batchMouse.position, new Vector2(31.5f, -7.5f)); InputSystem.Update();
            if (excluded != "Click") { Press(batchMouse.leftButton); InputSystem.Update(); }
            InputSystem.QueueDeltaStateEvent(batchMouse.scroll, new Vector2(1f, -2f)); InputSystem.Update();
            if (excluded != "Submit") { Press(batchKeyboard.enterKey); InputSystem.Update(); }
            if (excluded != "Cancel") { Press(batchKeyboard.escapeKey); InputSystem.Update(); }
            return SnapshotPending(router);
        }

        private static void AssertUiCaptureTerminal(InputRouter router, InputFrameCommitReceipt? receipt, UiSemanticFrameV1? frame, UiPendingSnapshot pending)
        {
            Assert.That(router.IsFaulted, Is.True);
            Assert.That(router.LastDiagnostic.Fault, Is.EqualTo(InputRouterFaultStage.UiCapture));
            Assert.That(router.CurrentReceipt, Is.EqualTo(receipt)); Assert.That(router.CurrentUiFrame, Is.EqualTo(frame));
            AssertPending(router, pending);
            Assert.Throws<System.InvalidOperationException>(() => router.StepForTests());
        }

        private static void AssertUiPendingEmpty(InputRouter router)
        {
            AssertPending(router, new UiPendingSnapshot(0, 0, false, false, 0, 0, false, false, false, 0, 0, 0, 0, false, false));
        }

        private static void AssertPending(InputRouter router, UiPendingSnapshot expected)
        {
            Assert.That(SnapshotPending(router), Is.EqualTo(expected));
        }

        private static UiPendingSnapshot SnapshotPending(InputRouter router) => new UiPendingSnapshot(
            InputRouterPlayModeTests.GetPrivate<int>(router, "_uiNavigateXQ4096"), InputRouterPlayModeTests.GetPrivate<int>(router, "_uiNavigateYQ4096"), InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiNavigateChanged"),
            InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiHasPoint"), InputRouterPlayModeTests.GetPrivate<int>(router, "_uiPointXActualPx"), InputRouterPlayModeTests.GetPrivate<int>(router, "_uiPointYActualPx"), InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiPointChanged"),
            InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiClickPressed"), InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiHasClickPoint"), InputRouterPlayModeTests.GetPrivate<int>(router, "_uiClickPointXActualPx"), InputRouterPlayModeTests.GetPrivate<int>(router, "_uiClickPointYActualPx"),
            InputRouterPlayModeTests.GetPrivate<long>(router, "_uiScrollXQ4096"), InputRouterPlayModeTests.GetPrivate<long>(router, "_uiScrollYQ4096"), InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiSubmitPressed"), InputRouterPlayModeTests.GetPrivate<bool>(router, "_uiCancelPressed"));

        private static float NonFinite(int sign) => sign == 0 ? float.NaN : sign > 0 ? float.PositiveInfinity : float.NegativeInfinity;
        private static void SetPrivate(InputRouter router, string field, object value) => typeof(InputRouter).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(router, value);

        private readonly struct UiPendingSnapshot : System.IEquatable<UiPendingSnapshot>
        {
            internal readonly int NavigateXQ4096, NavigateYQ4096, PointXActualPx, PointYActualPx, ClickPointXActualPx, ClickPointYActualPx;
            internal readonly long ScrollXQ4096, ScrollYQ4096;
            internal readonly bool NavigateChanged, HasPoint, PointChanged, ClickPressed, HasClickPoint, SubmitPressed, CancelPressed;
            internal UiPendingSnapshot(int navigateXQ4096, int navigateYQ4096, bool navigateChanged, bool hasPoint, int pointXActualPx, int pointYActualPx, bool pointChanged, bool clickPressed, bool hasClickPoint, int clickPointXActualPx, int clickPointYActualPx, long scrollXQ4096, long scrollYQ4096, bool submitPressed, bool cancelPressed)
            {
                NavigateXQ4096 = navigateXQ4096; NavigateYQ4096 = navigateYQ4096; NavigateChanged = navigateChanged; HasPoint = hasPoint; PointXActualPx = pointXActualPx; PointYActualPx = pointYActualPx; PointChanged = pointChanged; ClickPressed = clickPressed; HasClickPoint = hasClickPoint; ClickPointXActualPx = clickPointXActualPx; ClickPointYActualPx = clickPointYActualPx; ScrollXQ4096 = scrollXQ4096; ScrollYQ4096 = scrollYQ4096; SubmitPressed = submitPressed; CancelPressed = cancelPressed;
            }
            public bool Equals(UiPendingSnapshot other) => NavigateXQ4096 == other.NavigateXQ4096 && NavigateYQ4096 == other.NavigateYQ4096 && NavigateChanged == other.NavigateChanged && HasPoint == other.HasPoint && PointXActualPx == other.PointXActualPx && PointYActualPx == other.PointYActualPx && PointChanged == other.PointChanged && ClickPressed == other.ClickPressed && HasClickPoint == other.HasClickPoint && ClickPointXActualPx == other.ClickPointXActualPx && ClickPointYActualPx == other.ClickPointYActualPx && ScrollXQ4096 == other.ScrollXQ4096 && ScrollYQ4096 == other.ScrollYQ4096 && SubmitPressed == other.SubmitPressed && CancelPressed == other.CancelPressed;
            public override bool Equals(object obj) => obj is UiPendingSnapshot other && Equals(other);
            public override int GetHashCode() => NavigateXQ4096;
        }

        private static UiSemanticFrameV1 DifferentValidFrame(InputFrameCommitReceipt receipt) =>
            new UiSemanticFrameV1(receipt, 1, 0, true, false, 0, 0, false,
                false, false, 0, 0, 0, 0, false, false);

        private static InputRouter EnterUi(ICollection<GameObject> objects, InputRouterPlayModeTests.Rig rig, out object requester, bool settleUiEnableQuarantine = true)
        {
            var router = InputRouterPlayModeTests.CreateRouter(objects, rig, InputMode.GameplayEnabled);
            router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
            requester = new object(); router.RegisterModeRequester(requester);
            router.RequestMode(requester, InputMode.UIOnly, "test ui", rig.Player.NextExpectedTick);
            router.StepForTests(); InputRouterPlayModeTests.AdvanceFrame(rig);
            if (settleUiEnableQuarantine) InputSystem.Update();
            return router;
        }
    }

    // Registered only by AC007.  A public parameterless test type is required
    // because Input System creates processors through Activator.
    public sealed class ForcedNonFiniteNavigateProcessor : InputProcessor<Vector2>
    {
        public int sign;
        public static int ProcessCallCount { get; private set; }
        public static int NonFiniteOutputCount { get; private set; }

        public static void ResetInstrumentation()
        {
            ProcessCallCount = 0;
            NonFiniteOutputCount = 0;
        }

        public override Vector2 Process(Vector2 value, InputControl control)
        {
            ProcessCallCount++;
            if (value == Vector2.zero) return value;
            NonFiniteOutputCount++;
            var nonFinite = sign == 0 ? float.NaN : sign > 0 ? float.PositiveInfinity : float.NegativeInfinity;
            return new Vector2(nonFinite, 0f);
        }
    }
}
