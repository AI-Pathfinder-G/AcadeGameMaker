using System;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

using static AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-UX-004/006/007/008/013; AC-M5B5-001..004/007/008.
    // Independent adversarial coverage for the router's typed test seams.
    // Terminal retirement remains owned by the authored terminal fixture.
    internal sealed class InputRouterAdversarialPlayModeTests : InputTestFixture
    {
        [TestCase(InputMode.UIOnly)]
        [TestCase(InputMode.Cutscene)]
        [TestCase(InputMode.Transition)]
        [TestCase(InputMode.Ended)]
        public void EveryLockedModePublishesOneEpochAndExactlyOneEnabledMap(InputMode target)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out _);
                router.StepForTests();
                AdvanceFrame(rig);
                var requester = new object();
                router.RegisterModeRequester(requester);
                router.RequestMode(requester, target, "adversarial.mode", rig.Player.NextExpectedTick);
                router.StepForTests();

                Assert.That(router.IsFaulted, Is.False);
                Assert.That(router.EffectiveMode, Is.EqualTo(target));
                Assert.That(router.CurrentReceipt.Value.Tick, Is.EqualTo(new SimulationTick(1)));
                Assert.That(router.CurrentReceipt.Value.FrameOrdinal, Is.EqualTo(1));
                Assert.That(router.CurrentReceipt.Value.ModeEpoch, Is.EqualTo(1));
                var actions = GetPrivate<GameInputActions>(router, "_actions");
                Assert.That(actions.Gameplay.enabled, Is.EqualTo(target == InputMode.GameplayEnabled));
                Assert.That(actions.UI.enabled, Is.EqualTo(target == InputMode.UIOnly));
                Assert.That(rig.Player.IsInputGateLocked, Is.True);
                Assert.That(rig.Transfer.IsInputGateLocked, Is.True);
                Assert.That(rig.Combat.IsInputGateLocked, Is.True);
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void HeldJumpIsClearedByLockAndDoesNotReplayAfterFreshUnlock()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out _);
                router.StepForTests();
                AdvanceFrame(rig);
                var keyboard = InputSystem.AddDevice<Keyboard>();
                Press(keyboard.spaceKey);
                router.StepForTests();
                var first = MovementQueue(rig);
                Assert.That(first.ContainsKey(1), Is.True);
                Assert.That(first[1].JumpPressed, Is.True);
                AdvanceFrame(rig);

                var requester = new object();
                router.RegisterModeRequester(requester);
                router.RequestMode(requester, InputMode.UIOnly, "lock", rig.Player.NextExpectedTick);
                router.StepForTests();
                Assert.That(MovementQueue(rig).ContainsKey(2), Is.False);
                AdvanceFrame(rig);

                router.RequestMode(requester, InputMode.GameplayEnabled, "unlock", rig.Player.NextExpectedTick);
                router.StepForTests();
                var afterUnlock = MovementQueue(rig);
                Assert.That(afterUnlock.ContainsKey(3), Is.False, "A held button cannot synthesize a fresh Jump press after re-enable.");
                AdvanceFrame(rig);

                // Release the untracked held-through-lock button, then perform
                // a genuinely new press and release before the next fixed tick.
                Release(keyboard.spaceKey);
                Press(keyboard.spaceKey);
                Release(keyboard.spaceKey);
                router.StepForTests();
                var refreshed = MovementQueue(rig)[4];
                Assert.That(refreshed.JumpPressed, Is.True);
                Assert.That(refreshed.JumpReleased, Is.True);
                Release(keyboard.spaceKey);
            }
            finally { DestroyAll(objects); }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void DisabledOrRemovedBoundDeviceCallbackCannotInjectMovement(bool removeDevice)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out _);
                router.StepForTests();
                AdvanceFrame(rig);

                var actions = GetPrivate<GameInputActions>(router, "_actions");
                var keyboard = InputSystem.AddDevice<Keyboard>();
                var performedCount = 0;
                void Capture(InputAction.CallbackContext context) { performedCount++; }
                actions.Gameplay.Move.performed += Capture;
                // CallbackContext is valid only during its callback. Queue a
                // real device event, then retire the device before processing it;
                // never retain/replay an invalid native callback context.
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.D));
                if (removeDevice) InputSystem.RemoveDevice(keyboard);
                else InputSystem.DisableDevice(keyboard);
                InputSystem.Update();
                actions.Gameplay.Move.performed -= Capture;
                Assert.That(performedCount, Is.Zero);
                router.StepForTests();
                var command = MovementQueue(rig)[1];
                Assert.That(command.MoveXQ4096, Is.EqualTo(0));
                Assert.That(command.MoveYQ4096, Is.EqualTo(0));
                Assert.That(command.JumpPressed, Is.False);
                Assert.That(command.JumpReleased, Is.False);
                Assert.That(command.DashPressed, Is.False);
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void MissingCameraSuppressesMouseAttackAndTransferButCommitsFrame()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out var camera);
                router.StepForTests();
                AdvanceFrame(rig);
                var mouse = InputSystem.AddDevice<Mouse>();
                Set(mouse.position, new Vector2(640, 540));
                Press(mouse.rightButton);
                Press(mouse.leftButton);
                router.StepForTests();

                Assert.That(router.IsFaulted, Is.False);
                Assert.That(router.LastDiagnostic.Camera, Is.EqualTo(InputCameraDisposition.CameraUnavailable));
                Assert.That(TransferQueue(rig).ContainsKey(1), Is.False);
                Assert.That(CombatQueue(rig).ContainsKey(1), Is.False);
                Assert.That(router.CurrentReceipt.Value.Tick, Is.EqualTo(new SimulationTick(1)));
                Release(mouse.rightButton);
                Release(mouse.leftButton);
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void ValidGamepadAimSharesOneCurrentTickSampleWithTransferAndCombat()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out var camera);
                router.StepForTests();
                AdvanceFrame(rig);
                camera.Available = true;
                camera.Camera = CameraAt(0);
                var gamepad = InputSystem.AddDevice<Gamepad>();
                Set(gamepad.rightStick, new Vector2(.75f, 0));
                Press(gamepad.leftTrigger);
                Press(gamepad.rightTrigger);
                router.StepForTests();

                var transfer = TransferQueue(rig)[1];
                var combat = CombatQueue(rig)[1];
                Assert.That(transfer.Aim.HasValue, Is.True);
                Assert.That(combat.Aim.HasValue, Is.True);
                Assert.That(transfer.Aim.Value.SampleTick, Is.EqualTo(new SimulationTick(1)));
                Assert.That(combat.Aim.Value.SampleTick, Is.EqualTo(transfer.Aim.Value.SampleTick));
                Assert.That(combat.Aim.Value.SampleId, Is.EqualTo(transfer.Aim.Value.SampleId));
                Assert.That(transfer.Press.HasValue, Is.True);
                Assert.That(combat.Press.HasValue, Is.True);
                Assert.That(combat.Press.Value.AimSampleId, Is.EqualTo(transfer.Aim.Value.SampleId));
                Release(gamepad.leftTrigger);
                Release(gamepad.rightTrigger);
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void OutsideMousePointRetainsClampedAimButSuppressesBothMousePresses()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out var camera);
                router.StepForTests();
                AdvanceFrame(rig);
                camera.Available = true;
                camera.Camera = CameraAt(0);
                var mouse = InputSystem.AddDevice<Mouse>();
                Set(mouse.position, new Vector2(2500, 540));
                Press(mouse.rightButton);
                Press(mouse.leftButton);
                router.StepForTests();

                var transfer = TransferQueue(rig)[1];
                var combat = CombatQueue(rig)[1];
                Assert.That(transfer.Aim.HasValue, Is.True);
                Assert.That(transfer.Aim.Value.IsPointerInsideGameplayRect, Is.False);
                Assert.That(transfer.Aim.Value.ScreenPixelX, Is.EqualTo(1919));
                Assert.That(transfer.Press.HasValue, Is.False);
                Assert.That(combat.Aim.HasValue, Is.True);
                Assert.That(combat.Press.HasValue, Is.False);
                Release(mouse.rightButton);
                Release(mouse.leftButton);
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void PrepareFailurePreservesGlobalReceiptAndAllConsumerStateForRetry()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out _);
                router.StepForTests();
                AdvanceFrame(rig);
                var oldReceipt = router.CurrentReceipt.Value;
                var movementQueue = GetPrivate<object>(rig.Player, "_commands");
                var transferQueue = GetPrivate<object>(rig.Transfer, "_inputs");
                var combatQueue = GetPrivate<object>(rig.Combat, "_inputs");
                router.SetFaultStageForTests(InputRouterFaultStage.PrepareTransfer);
                Assert.Throws<InvalidOperationException>(() => router.StepForTests());
                Assert.That(router.IsFaulted, Is.False);
                Assert.That(router.CurrentReceipt.Value, Is.EqualTo(oldReceipt));
                Assert.That(GetPrivate<object>(rig.Player, "_commands"), Is.SameAs(movementQueue));
                Assert.That(GetPrivate<object>(rig.Transfer, "_inputs"), Is.SameAs(transferQueue));
                Assert.That(GetPrivate<object>(rig.Combat, "_inputs"), Is.SameAs(combatQueue));
                router.SetFaultStageForTests(null);
                Assert.DoesNotThrow(() => router.StepForTests());
                Assert.That(router.CurrentReceipt.Value.Tick, Is.EqualTo(new SimulationTick(1)));
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void PostCommitFailureFaultsRouterAndRetainsOldPublishedReceipt()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out _);
                router.StepForTests();
                AdvanceFrame(rig);
                var oldReceipt = router.CurrentReceipt.Value;
                router.SetFaultStageForTests(InputRouterFaultStage.AfterTransferCommit);
                Assert.Throws<InvalidOperationException>(() => router.StepForTests());
                Assert.That(router.IsFaulted, Is.True);
                Assert.That(router.CurrentReceipt.Value, Is.EqualTo(oldReceipt));
                var actions = GetPrivate<GameInputActions>(router, "_actions");
                Assert.That(actions.Gameplay.enabled, Is.False);
                Assert.That(actions.UI.enabled, Is.False);
                Assert.Throws<InvalidOperationException>(() => router.StepForTests());
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void EndedModeRejectsAnyUnlockRequestAfterTerminalModePublication()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out _);
                router.StepForTests();
                AdvanceFrame(rig);
                var requester = new object();
                router.RegisterModeRequester(requester);
                router.RequestMode(requester, InputMode.Ended, "end", rig.Player.NextExpectedTick);
                router.StepForTests();
                Assert.That(router.EffectiveMode, Is.EqualTo(InputMode.Ended));
                Assert.Throws<InvalidOperationException>(() => router.RequestMode(requester, InputMode.GameplayEnabled, "illegal", rig.Player.NextExpectedTick));
                Assert.That(router.EffectiveMode, Is.EqualTo(InputMode.Ended));
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void ModeRequesterRejectsWrongTickAndConflictWithoutStagingMutation()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out _);
                router.StepForTests();
                AdvanceFrame(rig);
                var requester = new object();
                var foreign = new object();
                router.RegisterModeRequester(requester);
                Assert.Throws<InvalidOperationException>(() => router.RequestMode(foreign, InputMode.UIOnly, "foreign", rig.Player.NextExpectedTick));
                Assert.Throws<InvalidOperationException>(() => router.RequestMode(requester, InputMode.UIOnly, "late", new SimulationTick(99)));
                Assert.Throws<InvalidOperationException>(() => router.RequestMode(requester, InputMode.UIOnly, "", rig.Player.NextExpectedTick));
                router.RequestMode(requester, InputMode.UIOnly, "same", rig.Player.NextExpectedTick);
                router.RequestMode(requester, InputMode.UIOnly, "same", rig.Player.NextExpectedTick);
                Assert.Throws<InvalidOperationException>(() => router.RequestMode(requester, InputMode.Cutscene, "conflict", rig.Player.NextExpectedTick));
                router.StepForTests();
                Assert.That(router.EffectiveMode, Is.EqualTo(InputMode.UIOnly));
                Assert.That(router.CurrentReceipt.Value.ModeEpoch, Is.EqualTo(1));
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void UnboundGameplayActionIsDiagnosticOnlyAndLeavesMovementNeutral()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out _);
                router.StepForTests();
                AdvanceFrame(rig);
                var keyboard = InputSystem.AddDevice<Keyboard>();
                Press(keyboard.eKey);
                router.StepForTests();
                Assert.That(router.LastDiagnostic.UnboundGameplay, Is.True);
                // Gameplay publishes a neutral MovementCommand on an otherwise
                // empty frame; the diagnostic must not turn it into movement or
                // an edge command.
                Assert.That(MovementQueue(rig).ContainsKey(1), Is.True);
                var command = MovementQueue(rig)[1];
                Assert.That(command.MoveXQ4096, Is.EqualTo(0));
                Assert.That(command.MoveYQ4096, Is.EqualTo(0));
                Assert.That(command.JumpPressed, Is.False);
                Assert.That(command.JumpReleased, Is.False);
                Assert.That(command.DashPressed, Is.False);
                Release(keyboard.eKey);
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void ForeignActionCallbackCannotInjectMovementIntoTheRegisteredRouter()
        {
            var objects = new List<GameObject>();
            InputAction foreign = null;
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, out _);
                router.StepForTests();
                AdvanceFrame(rig);
                var requester = new object();
                router.RegisterModeRequester(requester);
                router.RequestMode(requester, InputMode.UIOnly, "lock", rig.Player.NextExpectedTick);
                router.StepForTests();
                AdvanceFrame(rig);

                foreign = new InputAction("foreign-move");
                foreign.AddBinding("<Keyboard>/d");
                foreign.performed += context => router.OnMove(context);
                foreign.Enable();
                var keyboard = InputSystem.AddDevice<Keyboard>();
                Press(keyboard.dKey);
                Release(keyboard.dKey);
                foreign.Disable();
                foreign.Dispose();
                foreign = null;

                router.RequestMode(requester, InputMode.GameplayEnabled, "unlock", rig.Player.NextExpectedTick);
                router.StepForTests();
                Assert.That(MovementQueue(rig).ContainsKey(3), Is.False);
            }
            finally
            {
                if (foreign != null) { foreign.Disable(); foreign.Dispose(); }
                DestroyAll(objects);
            }
        }

        private static InputRouter CreateRouter(ICollection<GameObject> objects, Rig rig, InputMode mode, out TestCameraProvider camera)
        {
            var cameraObject = Track(objects, "adversarial-camera-provider");
            camera = cameraObject.AddComponent<TestCameraProvider>();
            camera.Player = rig.Player;
            var routerObject = Track(objects, "adversarial-input-router");
            routerObject.SetActive(false);
            var router = routerObject.AddComponent<InputRouter>();
            router.ConfigureForAuthoring(rig.Player, rig.Transfer, rig.Combat, camera, mode);
            routerObject.SetActive(true);
            router.InitializeForTests();
            return router;
        }

        private static SortedDictionary<int, MovementCommand> MovementQueue(Rig rig) =>
            GetPrivate<SortedDictionary<int, MovementCommand>>(rig.Player, "_commands");
        private static SortedDictionary<int, TransferSimulationInput> TransferQueue(Rig rig) =>
            GetPrivate<SortedDictionary<int, TransferSimulationInput>>(rig.Transfer, "_inputs");
        private static SortedDictionary<int, OrdanBossCombatInput> CombatQueue(Rig rig) =>
            GetPrivate<SortedDictionary<int, OrdanBossCombatInput>>(rig.Combat, "_inputs");

        private static SimulationCameraPoseSnapshot CameraAt(int tick) =>
            new SimulationCameraPoseSnapshot(new SimulationTick(tick), 0, 0, 10000, 0,
                1920, 1080, 0, 0, 1920, 1080, 3);

        private sealed class TestCameraProvider : MonoBehaviour, IInputCameraSnapshotProvider
        {
            internal PlayerMovementController Player { get; set; }
            internal bool Available { get; set; }
            internal SimulationCameraPoseSnapshot Camera { get; set; }
            public PlayerMovementController BoundPlayer => Player;
            public bool TryGetCompletedCamera(SimulationTick expectedPoseTick, out SimulationCameraPoseSnapshot camera)
            {
                camera = Camera;
                return Available && camera.CameraPoseTick == expectedPoseTick;
            }
        }
    }
}
