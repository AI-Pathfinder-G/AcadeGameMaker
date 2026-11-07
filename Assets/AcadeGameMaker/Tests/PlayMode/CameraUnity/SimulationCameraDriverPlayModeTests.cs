using System;
using System.Collections.Generic;
using AcadeGameMaker.Camera;
using AcadeGameMaker.Camera.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AcadeGameMaker.Tests.PlayMode.CameraUnity
{
    // REQ-ART-009/010/011, REQ-UX-013; AC-M5C2-001..005.
    // These tests exercise the real driver seams and real RenderTexture dimensions;
    // they do not replace the render target with an output-size override.
    internal sealed class SimulationCameraDriverPlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private readonly List<RenderTexture> _textures = new List<RenderTexture>();

        [TearDown]
        public void TearDown()
        {
            for (var index = _objects.Count - 1; index >= 0; index--)
            {
                if (_objects[index] != null) UnityEngine.Object.DestroyImmediate(_objects[index]);
            }
            for (var index = _textures.Count - 1; index >= 0; index--)
            {
                if (_textures[index] != null) UnityEngine.Object.DestroyImmediate(_textures[index]);
            }
            _objects.Clear();
            _textures.Clear();
        }

        [Test]
        public void BootstrapHasNoSeedThenPublishesOnlyTheRealCompletedMovementTick()
        {
            var rig = CreateRig(640, 360);
            Assert.That(rig.Driver.HasLatestPublication, Is.False);
            Assert.That(rig.Driver.TryGetCompletedCamera(new SimulationTick(0), out _), Is.False);
            Assert.That(rig.Driver.LatestDiagnostic.Disposition, Is.EqualTo(CameraDriverDisposition.BootstrapPending));

            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();

            Assert.That(rig.Driver.TryGetCompletedCamera(new SimulationTick(0), out var camera), Is.True);
            Assert.That(camera.CameraPoseTick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(camera.ViewportWidth, Is.EqualTo(640));
            Assert.That(camera.ViewportHeight, Is.EqualTo(360));
            Assert.That(camera.IntegerScale, Is.EqualTo(1));
            Assert.That(camera.GameplayRectX, Is.EqualTo(0));
            Assert.That(camera.GameplayRectY, Is.EqualTo(0));
        }

        [Test]
        public void DuplicateAndSkippedCompletedTicksRejectBeforePublicationMutation()
        {
            var rig = CreateRig(640, 360);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();
            var first = rig.Driver.LatestPublication;
            var next = rig.Driver.NextExpectedTick;

            Assert.Throws<InvalidOperationException>(() => rig.Driver.AdvanceForTests());
            Assert.That(rig.Driver.IsFaulted, Is.False);
            Assert.That(rig.Driver.HasLatestPublication, Is.True);
            Assert.That(rig.Driver.LatestPublication.CameraPoseTick, Is.EqualTo(first.CameraPoseTick));
            Assert.That(rig.Driver.NextExpectedTick, Is.EqualTo(next));

            TearDown();
            rig = CreateRig(640, 360);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();
            CompleteMovementTick(rig.Player);
            CompleteMovementTick(rig.Player);
            Assert.Throws<InvalidOperationException>(() => rig.Driver.AdvanceForTests());
            Assert.That(rig.Driver.IsFaulted, Is.False);
            Assert.That(rig.Driver.HasLatestPublication, Is.True);
            Assert.That(rig.Driver.LatestPublication.CameraPoseTick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(rig.Driver.NextExpectedTick, Is.EqualTo(new SimulationTick(1)));
        }

        [Test]
        public void PristineSeedCannotBeConsumedAsACompletedCameraTick()
        {
            var rig = CreateRig(640, 360);
            Assert.Throws<InvalidOperationException>(() => rig.Driver.AdvanceForTests());
            Assert.That(rig.Driver.IsFaulted, Is.False);
            Assert.That(rig.Driver.HasLatestPublication, Is.False);
            Assert.That(rig.Driver.NextExpectedTick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(rig.Driver.LatestDiagnostic.Disposition, Is.EqualTo(CameraDriverDisposition.BootstrapPending));
        }

        [TestCase(640, 360, 1, 0, 0, 640, 360)]
        [TestCase(1280, 720, 2, 0, 0, 1280, 720)]
        [TestCase(1920, 1080, 3, 0, 0, 1920, 1080)]
        [TestCase(2560, 1440, 4, 0, 0, 2560, 1440)]
        [TestCase(1366, 768, 2, 43, 24, 1280, 720)]
        [TestCase(1367, 769, 2, 43, 24, 1280, 720)]
        public void SupportedOutputUsesExactCenteredIntegerGameplayRectangle(int width, int height, int scale,
            int rectX, int rectY, int rectWidth, int rectHeight)
        {
            var rig = CreateRig(width, height);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();

            Assert.That(rig.Driver.LatestPublication.ViewportWidth, Is.EqualTo(width));
            Assert.That(rig.Driver.LatestPublication.ViewportHeight, Is.EqualTo(height));
            Assert.That(rig.Driver.LatestPublication.IntegerScale, Is.EqualTo(scale));
            Assert.That(rig.Driver.LatestPublication.GameplayRectX, Is.EqualTo(rectX));
            Assert.That(rig.Driver.LatestPublication.GameplayRectY, Is.EqualTo(rectY));
            Assert.That(rig.Driver.LatestPublication.GameplayRectWidth, Is.EqualTo(rectWidth));
            Assert.That(rig.Driver.LatestPublication.GameplayRectHeight, Is.EqualTo(rectHeight));
            Assert.That(rig.Camera.orthographicSize, Is.EqualTo(10f));
            Assert.That(rig.PixelPerfect.assetsPPU, Is.EqualTo(18));
            Assert.That(rig.PixelPerfect.refResolutionX, Is.EqualTo(640));
            Assert.That(rig.PixelPerfect.refResolutionY, Is.EqualTo(360));
            Assert.That(rig.PixelPerfect.cropFrame, Is.EqualTo(PixelPerfectCamera.CropFrame.Windowbox));
            Assert.That(rig.PixelPerfect.gridSnapping, Is.EqualTo(PixelPerfectCamera.GridSnapping.UpscaleRenderTexture));
        }

        [Test]
        public void TransformMutationCannotSteerNextLogicalCameraCalculation()
        {
            var rig = CreateRig(640, 360);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();
            var first = rig.Driver.LatestPublication;
            rig.Camera.transform.position = new Vector3(99f, -99f, rig.Camera.transform.position.z);

            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();

            Assert.That(rig.Driver.LatestPublication.CameraPoseTick, Is.EqualTo(new SimulationTick(1)));
            Assert.That(rig.Driver.LatestPublication.PositionXQ100, Is.EqualTo(first.PositionXQ100));
            Assert.That(rig.Driver.LatestPublication.PositionYQ100, Is.EqualTo(first.PositionYQ100));
            Assert.That(rig.Camera.transform.position.x, Is.EqualTo(0f));
            Assert.That(rig.Camera.transform.position.y, Is.EqualTo(0f));
        }

        [Test]
        public void UnsupportedViewportAdvancesCoreWithoutUsablePublicationAndRecoversCurrentTick()
        {
            var rig = CreateRig(640, 360);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();
            var previousPosition = rig.Camera.transform.position;

            ReplaceTexture(rig, 639, 360);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();

            Assert.That(rig.Driver.LatestDiagnostic.Disposition, Is.EqualTo(CameraDriverDisposition.OutputUnavailable));
            Assert.That(rig.Driver.TryGetCompletedCamera(new SimulationTick(1), out _), Is.False);
            Assert.That(rig.Camera.transform.position, Is.EqualTo(previousPosition));
            Assert.That(rig.Driver.LatestCoreSnapshot.Tick, Is.EqualTo(new SimulationTick(1)));
            Assert.That(rig.Driver.NextExpectedTick, Is.EqualTo(new SimulationTick(2)));

            ReplaceTexture(rig, 1280, 720);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();

            Assert.That(rig.Driver.TryGetCompletedCamera(new SimulationTick(2), out var recovered), Is.True);
            Assert.That(recovered.CameraPoseTick, Is.EqualTo(new SimulationTick(2)));
            Assert.That(rig.Driver.NextExpectedTick, Is.EqualTo(new SimulationTick(3)));
        }

        [Test]
        public void PublicProfileDriftFaultsBeforeCoreMutation()
        {
            var rig = CreateRig(640, 360);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();
            var first = rig.Driver.LatestPublication;
            rig.PixelPerfect.assetsPPU = 19;
            CompleteMovementTick(rig.Player);

            Assert.Throws<InvalidOperationException>(() => rig.Driver.AdvanceForTests());
            Assert.That(rig.Driver.IsFaulted, Is.True);
            Assert.That(rig.Driver.TryGetCompletedCamera(new SimulationTick(1), out _), Is.False);
            Assert.That(rig.Driver.LatestDiagnostic.Disposition, Is.EqualTo(CameraDriverDisposition.Faulted));
            Assert.That(first.CameraPoseTick, Is.EqualTo(new SimulationTick(0)));
        }

        [Test]
        public void CameraRotationAndScaleDriftFaultBeforeTheNextCoreAdvance()
        {
            var rig = CreateRig(640, 360);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();
            rig.Camera.transform.rotation = Quaternion.Euler(0f, 0f, 1f);
            CompleteMovementTick(rig.Player);

            Assert.Throws<InvalidOperationException>(() => rig.Driver.AdvanceForTests());
            Assert.That(rig.Driver.IsFaulted, Is.True);

            TearDown();
            rig = CreateRig(640, 360);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();
            rig.Camera.transform.localScale = new Vector3(2f, 1f, 1f);
            CompleteMovementTick(rig.Player);

            Assert.Throws<InvalidOperationException>(() => rig.Driver.AdvanceForTests());
            Assert.That(rig.Driver.IsFaulted, Is.True);
        }

        [Test]
        public void BindingCannotBeReconfiguredAfterInitialization()
        {
            var rig = CreateRig(640, 360);
            var other = Track(new GameObject("camera-driver-other-player"));
            var body = other.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            other.AddComponent<BoxCollider2D>();
            var otherPlayer = other.AddComponent<PlayerMovementController>();

            Assert.Throws<InvalidOperationException>(() => rig.Driver.ConfigureForAuthoring(
                otherPlayer, rig.Camera, rig.PixelPerfect, 0, 0, new CameraCenterBounds(-180, 180, -180, 180)));
            Assert.That(rig.Driver.IsFaulted, Is.False);
            Assert.That(rig.Driver.BoundPlayer, Is.SameAs(rig.Player));
        }

        [TestCase(CameraDriverFaultStage.BeforeCoreAdvance)]
        [TestCase(CameraDriverFaultStage.AfterCoreAdvance)]
        [TestCase(CameraDriverFaultStage.BeforeTransformWrite)]
        [TestCase(CameraDriverFaultStage.BeforePublication)]
        public void EngineFailureStagesFaultClosedWithoutMisleadingNewPublication(CameraDriverFaultStage stage)
        {
            var rig = CreateRig(640, 360);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();
            rig.Driver.SetFaultStageForTests(stage);
            CompleteMovementTick(rig.Player);

            Assert.Throws<InvalidOperationException>(() => rig.Driver.AdvanceForTests());
            Assert.That(rig.Driver.IsFaulted, Is.True);
            Assert.That(rig.Driver.HasLatestPublication, Is.False);
            Assert.That(rig.Driver.TryGetCompletedCamera(new SimulationTick(1), out _), Is.False);
            Assert.That(rig.Driver.LatestDiagnostic.Disposition, Is.EqualTo(CameraDriverDisposition.Faulted));
        }

        [Test]
        public void LateInitializationRejectsCompletedHistory()
        {
            var rig = CreateRigWithoutDriver(640, 360);
            CompleteMovementTick(rig.Player);
            var driver = AddDriver(rig);
            Assert.Throws<InvalidOperationException>(() => driver.InitializeForTests());
            Assert.That(driver.IsFaulted, Is.True);
            Assert.That(driver.HasLatestPublication, Is.False);
        }

        [Test]
        public void DisabledDriverFaultsAndCannotBeReinitialized()
        {
            var rig = CreateRig(640, 360);
            rig.Driver.enabled = false;
            Assert.That(rig.Driver.IsFaulted, Is.True);
            Assert.Throws<InvalidOperationException>(() => rig.Driver.InitializeForTests());
            Assert.That(rig.Driver.TryGetCompletedCamera(new SimulationTick(0), out _), Is.False);
        }

        [TestCase(-1, -6)]
        [TestCase(1, 6)]
        [TestCase(9, 50)]
        public void RenderPoseUsesPixel18RatherThanTheQuantizedDto(int pixel18, int q100)
        {
            var rig = CreateRigWithoutDriver(640, 360);
            var driver = rig.Camera.gameObject.AddComponent<SimulationCameraDriver>();
            driver.ConfigureForAuthoring(rig.Player, rig.Camera, rig.PixelPerfect, pixel18, pixel18,
                new CameraCenterBounds(pixel18, pixel18, pixel18, pixel18));
            driver.InitializeForTests();
            CompleteMovementTick(rig.Player);
            driver.AdvanceForTests();
            Assert.That(rig.Camera.transform.position.x, Is.EqualTo(pixel18 / 18f));
            Assert.That(rig.Camera.transform.position.y, Is.EqualTo(pixel18 / 18f));
            Assert.That(driver.LatestPublication.PositionXQ100, Is.EqualTo(q100));
            Assert.That(driver.LatestPublication.PositionYQ100, Is.EqualTo(q100));
            if (Math.Abs(pixel18) == 1)
                Assert.That(rig.Camera.transform.position.x, Is.Not.EqualTo(q100 / 100f));
        }

        [TestCase("_player")]
        [TestCase("_camera")]
        [TestCase("_pixelPerfect")]
        public void SerializedBindingDriftFaultsWithoutAdvancingTheOriginalCore(string field)
        {
            var rig = CreateRig(640, 360);
            var other = CreateRigWithoutDriver(640, 360);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();
            CompleteMovementTick(rig.Player);
            CompleteMovementTick(other.Player);
            CompleteMovementTick(other.Player);
            object replacement = field == "_player" ? (object)other.Player :
                field == "_camera" ? other.Camera : (object)other.PixelPerfect;
            typeof(SimulationCameraDriver).GetField(field,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(rig.Driver, replacement);
            Assert.Throws<InvalidOperationException>(() => rig.Driver.AdvanceForTests());
            Assert.That(rig.Driver.IsFaulted, Is.True);
            Assert.That(rig.Driver.NextExpectedTick.Value, Is.EqualTo(1));
            Assert.That(rig.Driver.LatestCoreSnapshot.Tick.Value, Is.Zero);
            Assert.That(rig.Driver.TryGetCompletedCamera(new SimulationTick(0), out _), Is.False);
        }

        [Test]
        public void ActualReenableCannotResurrectAPublication()
        {
            var rig = CreateRig(640, 360);
            CompleteMovementTick(rig.Player);
            rig.Driver.AdvanceForTests();
            rig.Driver.enabled = false;
            UnityEngine.TestTools.LogAssert.Expect(LogType.Exception,
                new System.Text.RegularExpressions.Regex("InvalidOperationException: Camera driver cannot re-enable"));
            rig.Driver.enabled = true;
            Assert.That(rig.Driver.IsFaulted, Is.True);
            Assert.That(rig.Driver.TryGetCompletedCamera(new SimulationTick(0), out _), Is.False);
            Assert.Throws<InvalidOperationException>(() => rig.Driver.AdvanceForTests());
        }

        private Rig CreateRig(int width, int height)
        {
            var rig = CreateRigWithoutDriver(width, height);
            rig.Driver = AddDriver(rig);
            rig.Driver.InitializeForTests();
            return rig;
        }

        private Rig CreateRigWithoutDriver(int width, int height)
        {
            var playerObject = Track(new GameObject("camera-driver-player"));
            var body = playerObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            playerObject.AddComponent<CapsuleCollider2D>().size = new Vector2(0.8f, 1.6f);
            var player = playerObject.AddComponent<PlayerMovementController>();

            var cameraObject = Track(new GameObject("camera-driver-camera"));
            var camera = cameraObject.AddComponent<UnityEngine.Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 10f;
            camera.transform.rotation = Quaternion.identity;
            camera.transform.localScale = Vector3.one;
            var pixelPerfect = cameraObject.AddComponent<PixelPerfectCamera>();
            pixelPerfect.assetsPPU = 18;
            pixelPerfect.refResolutionX = 640;
            pixelPerfect.refResolutionY = 360;
            pixelPerfect.cropFrame = PixelPerfectCamera.CropFrame.Windowbox;
            pixelPerfect.gridSnapping = PixelPerfectCamera.GridSnapping.UpscaleRenderTexture;
            var texture = NewTexture(width, height);
            camera.targetTexture = texture;

            return new Rig(player, camera, pixelPerfect, texture);
        }

        private SimulationCameraDriver AddDriver(Rig rig)
        {
            var driver = rig.Camera.gameObject.AddComponent<SimulationCameraDriver>();
            driver.ConfigureForAuthoring(rig.Player, rig.Camera, rig.PixelPerfect, 0, 0,
                new CameraCenterBounds(-180, 180, -180, 180));
            return driver;
        }

        private RenderTexture NewTexture(int width, int height)
        {
            var texture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            _textures.Add(texture);
            return texture;
        }

        private void ReplaceTexture(Rig rig, int width, int height)
        {
            rig.Camera.targetTexture = null;
            if (rig.Texture != null)
            {
                rig.Texture.Release();
                UnityEngine.Object.DestroyImmediate(rig.Texture);
            }
            rig.Texture = NewTexture(width, height);
            rig.Camera.targetTexture = rig.Texture;
        }

        private static void CompleteMovementTick(PlayerMovementController player)
        {
            player.AdvanceOneFixedTickForTests();
        }

        private GameObject Track(GameObject value)
        {
            _objects.Add(value);
            return value;
        }

        private sealed class Rig
        {
            internal Rig(PlayerMovementController player, UnityEngine.Camera camera, PixelPerfectCamera pixelPerfect, RenderTexture texture)
            { Player = player; Camera = camera; PixelPerfect = pixelPerfect; Texture = texture; }
            internal PlayerMovementController Player { get; }
            internal UnityEngine.Camera Camera { get; }
            internal PixelPerfectCamera PixelPerfect { get; }
            internal RenderTexture Texture { get; set; }
            internal SimulationCameraDriver Driver { get; set; }
        }
    }
}
