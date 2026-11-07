using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using static AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-UX-006/013, REQ-WT-006; AC-M5B5-001/003/004/006.
    internal sealed class InputRouterAimPlayModeTests : InputTestFixture
    {
        [TestCase("_nextAimSampleId")]
        [TestCase("_nextAttackSequenceId")]
        public void PublicationIdOverflowPreservesQueuesAndPendingMousePress(string field)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateWithCamera(objects, rig, out _);
                router.StepForTests(); AdvanceFrame(rig);
                var receipt = router.CurrentReceipt;
                var mouse = InputSystem.AddDevice<Mouse>();
                Set(mouse.position, new Vector2(2400, 720)); Press(mouse.leftButton);
                typeof(InputRouter).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(router, long.MaxValue);
                var mg = GetPrivate<long>(rig.Player, "_inputGeneration");
                var tg = GetPrivate<long>(rig.Transfer, "_inputGeneration");
                var cg = GetPrivate<long>(rig.Combat, "_inputGeneration");
                Assert.Throws<System.OverflowException>(() => router.StepForTests());
                Assert.That(router.CurrentReceipt, Is.EqualTo(receipt));
                Assert.That(router.IsFaulted, Is.False);
                Assert.That(GetPrivate<bool>(router, "_attackPressed"), Is.True);
                Assert.That(GetPrivate<long>(router, field), Is.EqualTo(long.MaxValue));
                Assert.That(GetPrivate<long>(rig.Player, "_inputGeneration"), Is.EqualTo(mg));
                Assert.That(GetPrivate<long>(rig.Transfer, "_inputGeneration"), Is.EqualTo(tg));
                Assert.That(GetPrivate<long>(rig.Combat, "_inputGeneration"), Is.EqualTo(cg));
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void TooShortMouseDirectionCannotLeaveAnOrphanCameraInNullableTransfer()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateWithCamera(objects, rig, out _);
                router.StepForTests(); AdvanceFrame(rig);
                var mouse = InputSystem.AddDevice<Mouse>();
                Set(mouse.position, new Vector2(1280, 720));
                var pad = InputSystem.AddDevice<Gamepad>(); Press(pad.leftTrigger);
                Assert.DoesNotThrow(() => router.StepForTests());
                var input = GetPrivate<SortedDictionary<int, TransferSimulationInput>>(rig.Transfer, "_inputs")[1];
                Assert.That(input.Aim.HasValue, Is.False);
                Assert.That(input.Camera.HasValue, Is.False);
                Assert.That(input.Press.HasValue, Is.True);
                Assert.That(GetPrivate<long>(router, "_nextAimSampleId"), Is.Zero);
                AdvanceFrame(rig);
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void HeldDeviceRecordingMatchesAtThirtySixtyAndOneFortyFourRenderGroupings()
        {
            // Scripted render grouping, not a physical frame-rate/hardware claim.
            var baseline = ReplayHeldInputs(60);
            CollectionAssert.AreEqual(baseline, ReplayHeldInputs(30));
            CollectionAssert.AreEqual(baseline, ReplayHeldInputs(144));
        }

        private List<string> ReplayHeldInputs(int renderRate)
        {
            var objects = new List<GameObject>();
            Keyboard keyboard = null;
            Gamepad pad = null;
            try
            {
                var rig = CreateRig(objects);
                var router = CreateWithCamera(objects, rig, out _);
                keyboard = InputSystem.AddDevice<Keyboard>();
                pad = InputSystem.AddDevice<Gamepad>();
                Press(keyboard.dKey);
                Set(pad.rightStick, new Vector2(0.7f, 0.3f));
                var records = new List<string>();
                var accumulator = 0;
                while (records.Count < 30)
                {
                    accumulator += 60;
                    while (accumulator >= renderRate && records.Count < 30)
                    {
                        accumulator -= renderRate;
                        router.StepForTests(); AdvanceFrame(rig);
                        var pose = rig.Player.Snapshot;
                        var receipt = router.CurrentReceipt.Value;
                        records.Add($"{receipt.Tick.Value}/{receipt.FrameOrdinal}/{receipt.ModeEpoch}/{pose.PositionXQ4096}/{pose.PositionYQ4096}/{GetPrivate<long>(router, "_nextAimSampleId")}");
                    }
                }
                return records;
            }
            finally
            {
                DestroyAll(objects);
                if (keyboard != null && keyboard.added) InputSystem.RemoveDevice(keyboard);
                if (pad != null && pad.added) InputSystem.RemoveDevice(pad);
            }
        }

        [TestCase(2560, 1440, 0, 0, 0, 0, true)]
        [TestCase(2560, 1440, 2559, 1439, 1919, 1079, true)]
        [TestCase(2560, 1440, -1, 0, 0, 0, false)]
        [TestCase(2560, 1440, 2560, 1439, 1919, 1079, false)]
        [TestCase(2560, 1440, 2559, 1440, 1919, 1079, false)]
        [TestCase(1366, 768, 43, 24, 0, 0, true)]
        [TestCase(1366, 768, 1322, 743, 1919, 1079, true)]
        [TestCase(1366, 768, 42, 24, 0, 0, false)]
        [TestCase(1366, 768, 1323, 744, 1919, 1079, false)]
        public void MouseUsesCompletedPoseAndClampedGridWithExactPressContainment(
            int width, int height, int x, int y, int gridX, int gridY, bool inside)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateWithCamera(objects, rig, out var camera);
                camera.Width = width; camera.Height = height;
                router.StepForTests(); AdvanceFrame(rig);
                var mouse = InputSystem.AddDevice<Mouse>();
                Set(mouse.position, new Vector2(10, 10));
                Set(mouse.position, new Vector2(x, y));
                Press(mouse.leftButton); Press(mouse.rightButton);
                router.StepForTests();
                var transfer = GetPrivate<SortedDictionary<int, TransferSimulationInput>>(rig.Transfer, "_inputs")[1];
                var combat = GetPrivate<SortedDictionary<int, OrdanBossCombatInput>>(rig.Combat, "_inputs")[1];
                Assert.That(transfer.Aim.HasValue, Is.True);
                Assert.That(combat.Aim, Is.EqualTo(transfer.Aim));
                Assert.That(transfer.Aim.Value.SampleTick.Value, Is.EqualTo(1));
                Assert.That(transfer.Aim.Value.CameraPoseTick.Value, Is.EqualTo(0));
                Assert.That(transfer.Aim.Value.ScreenPixelX, Is.EqualTo(gridX));
                Assert.That(transfer.Aim.Value.ScreenPixelY, Is.EqualTo(gridY));
                Assert.That(transfer.Aim.Value.IsPointerInsideGameplayRect, Is.EqualTo(inside));
                Assert.That(transfer.Press.HasValue, Is.EqualTo(inside));
                Assert.That(combat.Press.HasValue, Is.EqualTo(inside));
                Assert.That(GetPrivate<long>(router, "_nextAimSampleId"), Is.EqualTo(1));
                Assert.That(GetPrivate<long>(router, "_nextAttackSequenceId"), Is.EqualTo(inside ? 1 : 0));
            }
            finally { DestroyAll(objects); }
        }

        [TestCase(false, 0, InputCameraDisposition.CameraUnavailable)]
        [TestCase(true, 1, InputCameraDisposition.CameraRejected)]
        public void MissingOrWrongTickCameraSuppressesMouseButAllowsNullableGamepadTransfer(
            bool available, int tickOffset, InputCameraDisposition disposition)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateWithCamera(objects, rig, out var camera);
                router.StepForTests(); AdvanceFrame(rig);
                camera.Available = available; camera.TickOffset = tickOffset;
                var mouse = InputSystem.AddDevice<Mouse>();
                Set(mouse.position, new Vector2(2400, 720));
                Press(mouse.leftButton); Press(mouse.rightButton);
                router.StepForTests();
                Assert.That(router.LastDiagnostic.Camera, Is.EqualTo(disposition));
                Assert.That(GetPrivate<SortedDictionary<int, TransferSimulationInput>>(rig.Transfer, "_inputs").Count, Is.Zero);
                Assert.That(GetPrivate<SortedDictionary<int, OrdanBossCombatInput>>(rig.Combat, "_inputs").Count, Is.Zero);
                Assert.That(GetPrivate<long>(router, "_nextAimSampleId"), Is.Zero);
                AdvanceFrame(rig);
                Release(mouse.leftButton); Release(mouse.rightButton);
                var pad = InputSystem.AddDevice<Gamepad>();
                Press(pad.leftTrigger);
                router.StepForTests();
                var transfer = GetPrivate<SortedDictionary<int, TransferSimulationInput>>(rig.Transfer, "_inputs")[2];
                Assert.That(transfer.Aim.HasValue, Is.False);
                Assert.That(transfer.Camera.HasValue, Is.False);
                Assert.That(transfer.Press.HasValue, Is.True);
                Assert.That(transfer.Press.Value.AimSampleId.HasValue, Is.False);
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void SeedDoesNotAuthorizeAimAndRetainedStickPublishesFreshSharedSamples()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateWithCamera(objects, rig, out _);
                var pad = InputSystem.AddDevice<Gamepad>();
                Set(pad.rightStick, new Vector2(0.25f, 0));
                Press(pad.rightTrigger);
                router.StepForTests();
                Assert.That(router.LastDiagnostic.Camera, Is.EqualTo(InputCameraDisposition.BootstrapPending));
                Assert.That(GetPrivate<long>(router, "_nextAimSampleId"), Is.Zero);
                Assert.That(GetPrivate<long>(router, "_nextAttackSequenceId"), Is.Zero);
                AdvanceFrame(rig);
                for (var tick = 1; tick <= 3; tick++)
                {
                    if (tick == 2) Set(pad.rightStick, new Vector2(0.01f, 0));
                    router.StepForTests();
                    var transfer = GetPrivate<SortedDictionary<int, TransferSimulationInput>>(rig.Transfer, "_inputs")[tick];
                    var combat = GetPrivate<SortedDictionary<int, OrdanBossCombatInput>>(rig.Combat, "_inputs")[tick];
                    Assert.That(transfer.Aim, Is.EqualTo(combat.Aim));
                    Assert.That(transfer.Aim.Value.SampleId, Is.EqualTo(tick - 1));
                    Assert.That(transfer.Aim.Value.SampleTick.Value, Is.EqualTo(tick));
                    Assert.That(transfer.Aim.Value.AimVectorXQ4096, Is.EqualTo(4096));
                    Assert.That(transfer.Aim.Value.AimVectorYQ4096, Is.Zero);
                    Assert.That(combat.Press.HasValue, Is.False, "Bootstrap attack cannot replay from a held trigger.");
                    AdvanceFrame(rig);
                }
            }
            finally { DestroyAll(objects); }
        }

        private static InputRouter CreateWithCamera(List<GameObject> objects, Rig rig, out FrozenCameraProvider camera)
        {
            camera = Track(objects, "frozen-camera").AddComponent<FrozenCameraProvider>();
            camera.Player = rig.Player;
            var owner = Track(objects, "aim-router");
            owner.SetActive(false);
            var router = owner.AddComponent<InputRouter>();
            router.ConfigureForAuthoring(rig.Player, rig.Transfer, rig.Combat, camera, InputMode.GameplayEnabled);
            owner.SetActive(true);
            router.InitializeForTests();
            return router;
        }

        private sealed class FrozenCameraProvider : MonoBehaviour, IInputCameraSnapshotProvider
        {
            internal AcadeGameMaker.Movement.PlayerMovementController Player;
            internal int Width = 2560, Height = 1440, TickOffset;
            internal bool Available = true;
            public AcadeGameMaker.Movement.PlayerMovementController BoundPlayer => Player;
            public bool TryGetCompletedCamera(SimulationTick expectedPoseTick, out SimulationCameraPoseSnapshot camera)
            {
                var scale = System.Math.Min(Width / 640, Height / 360);
                camera = new SimulationCameraPoseSnapshot(new SimulationTick(expectedPoseTick.Value + TickOffset),
                    0, 0, 10000, 0, Width, Height, (Width - scale * 640) / 2, (Height - scale * 360) / 2,
                    scale * 640, scale * 360, scale);
                return Available;
            }
        }
    }
}
