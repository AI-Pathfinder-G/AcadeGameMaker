using System.Collections.Generic;
using AcadeGameMaker.Camera;
using AcadeGameMaker.Camera.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using static AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-ART-009/010/011, REQ-UX-008/013; AC-M5C2-001/004.
    // Actual Movement + Camera driver + device router; no fabricated camera provider.
    internal sealed class InputRouterActualCameraPlayModeTests : InputTestFixture
    {
        [Test]
        public void ActualCompletedCameraFeedsNextTickAimAcrossAnUnavailableOutputGap()
        {
            var objects = new List<GameObject>();
            var output = new RenderTexture(2560, 1440, 0);
            var smallOutput = new RenderTexture(639, 360, 0);
            try
            {
                var rig = CreateRig(objects);
                var cameraObject = Track(objects, "actual-simulation-camera");
                cameraObject.transform.position = new Vector3(0, 0, -10);
                var camera = cameraObject.AddComponent<UnityEngine.Camera>();
                camera.targetTexture = output;
                var pixelPerfect = cameraObject.AddComponent<PixelPerfectCamera>();
                pixelPerfect.assetsPPU = 18;
                pixelPerfect.refResolutionX = 640;
                pixelPerfect.refResolutionY = 360;
                pixelPerfect.cropFrame = PixelPerfectCamera.CropFrame.Windowbox;
                pixelPerfect.gridSnapping = PixelPerfectCamera.GridSnapping.UpscaleRenderTexture;
                camera.orthographic = true;
                camera.orthographicSize = 10;
                var driver = cameraObject.AddComponent<SimulationCameraDriver>();
                driver.ConfigureForAuthoring(rig.Player, camera, pixelPerfect, 0, 0,
                    new CameraCenterBounds(-1000, 1000, -1000, 1000));
                var routerObject = Track(objects, "actual-camera-router");
                var router = routerObject.AddComponent<InputRouter>();
                router.ConfigureForAuthoring(rig.Player, rig.Transfer, rig.Combat, driver, InputMode.GameplayEnabled);
                // Router starts earlier than +10 camera, and can bind before its initialization.
                router.InitializeForTests();
                driver.InitializeForTests();
                Assert.That(driver.TryGetCompletedCamera(new SimulationTick(0), out _), Is.False);
                var mouse = InputSystem.AddDevice<Mouse>();
                Set(mouse.position, new Vector2(2300, 720));
                router.StepForTests();
                Assert.That(router.LastDiagnostic.Camera, Is.EqualTo(InputCameraDisposition.BootstrapPending));
                AdvanceFrame(rig);
                driver.AdvanceForTests();
                Assert.That(driver.TryGetCompletedCamera(new SimulationTick(0), out var camera0), Is.True);

                router.StepForTests();
                var input1 = Queued(rig, 1);
                Assert.That(input1.Aim.HasValue, Is.True);
                Assert.That(input1.Camera.Value, Is.EqualTo(camera0));
                Assert.That(input1.Aim.Value.CameraPoseTick.Value, Is.Zero);
                Assert.That(input1.Aim.Value.SampleTick.Value, Is.EqualTo(1));
                var sample1 = input1.Aim.Value.SampleId;
                AdvanceFrame(rig);
                var priorTransform = camera.transform.position;
                camera.targetTexture = smallOutput;
                driver.AdvanceForTests();
                Assert.That(camera.transform.position, Is.EqualTo(priorTransform));
                Assert.That(driver.TryGetCompletedCamera(new SimulationTick(0), out _), Is.False);
                Assert.That(driver.TryGetCompletedCamera(new SimulationTick(1), out _), Is.False);

                Press(mouse.leftButton);
                router.StepForTests();
                Assert.That(router.LastDiagnostic.Camera, Is.EqualTo(InputCameraDisposition.CameraUnavailable));
                Assert.That(GetPrivate<SortedDictionary<int, TransferSimulationInput>>(rig.Transfer, "_inputs").ContainsKey(2), Is.False,
                    "No aim or Transfer press means no queued Transfer payload, while the coordinated neutral frame remains valid.");
                Assert.That(GetPrivate<bool>(router, "_committedAttack"), Is.False);
                AdvanceFrame(rig);
                camera.targetTexture = output;
                driver.AdvanceForTests();
                Assert.That(driver.TryGetCompletedCamera(new SimulationTick(2), out var camera2), Is.True);
                Assert.That(driver.TryGetCompletedCamera(new SimulationTick(1), out _), Is.False);
                router.StepForTests();
                var input3 = Queued(rig, 3);
                Assert.That(input3.Camera.Value, Is.EqualTo(camera2));
                Assert.That(input3.Aim.Value.SampleTick.Value, Is.EqualTo(3));
                Assert.That(input3.Aim.Value.SampleId, Is.EqualTo(sample1 + 1));
                Assert.That(GetPrivate<bool>(router, "_committedAttack"), Is.False,
                    "A press suppressed during the camera gap must not replay on recovery.");
                Assert.That(router.IsFaulted, Is.False);
                AdvanceFrame(rig);
                driver.AdvanceForTests();
            }
            finally
            {
                DestroyAll(objects);
                Object.DestroyImmediate(output);
                Object.DestroyImmediate(smallOutput);
            }
        }

        private static TransferSimulationInput Queued(Rig rig, int tick) =>
            GetPrivate<SortedDictionary<int, TransferSimulationInput>>(rig.Transfer, "_inputs")[tick];
    }
}
