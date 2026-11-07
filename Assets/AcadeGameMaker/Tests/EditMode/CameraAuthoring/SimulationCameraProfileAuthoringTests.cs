using System;
using AcadeGameMaker.Camera.Authoring;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AcadeGameMaker.Tests.EditMode.CameraAuthoring
{
    // AC-M5C2-002, AC-M5C2-005: editor authoring certifies the approved profile boundary.
    public sealed class SimulationCameraProfileAuthoringTests
    {
        [Test]
        public void ApplyApprovedProfile_ConfiguresExactCameraAndPixelPerfectProfile()
        {
            var cameraObject = new GameObject("Camera profile");
            try
            {
                var camera = cameraObject.AddComponent<UnityEngine.Camera>();
                var pixelPerfectCamera = cameraObject.AddComponent<PixelPerfectCamera>();
                pixelPerfectCamera.assetsPPU = 1;
                pixelPerfectCamera.refResolutionX = 1;
                pixelPerfectCamera.refResolutionY = 1;
                pixelPerfectCamera.cropFrame = PixelPerfectCamera.CropFrame.None;
                pixelPerfectCamera.gridSnapping = PixelPerfectCamera.GridSnapping.None;

                SetFilterMode(pixelPerfectCamera, PixelPerfectCamera.PixelPerfectFilterMode.RetroAA);
                SimulationCameraProfileAuthoring.ApplyApprovedProfile(camera, pixelPerfectCamera);

                Assert.DoesNotThrow(() => SimulationCameraProfileAuthoring.ValidateApprovedProfile(camera, pixelPerfectCamera));
                Assert.That(camera.orthographicSize, Is.EqualTo(10f));
                Assert.That(pixelPerfectCamera.assetsPPU, Is.EqualTo(18));
                Assert.That(pixelPerfectCamera.refResolutionX, Is.EqualTo(640));
                Assert.That(pixelPerfectCamera.refResolutionY, Is.EqualTo(360));
                Assert.That(ReadFilterMode(pixelPerfectCamera), Is.EqualTo(PixelPerfectCamera.PixelPerfectFilterMode.Point));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(cameraObject);
            }
        }

        [Test]
        public void ValidateApprovedProfile_IsReadOnlyAndRejectsSerializedFilterDrift()
        {
            var cameraObject = new GameObject("Camera profile");
            try
            {
                var camera = cameraObject.AddComponent<UnityEngine.Camera>();
                var pixelPerfectCamera = cameraObject.AddComponent<PixelPerfectCamera>();
                SimulationCameraProfileAuthoring.ApplyApprovedProfile(camera, pixelPerfectCamera);
                SetFilterMode(pixelPerfectCamera, PixelPerfectCamera.PixelPerfectFilterMode.RetroAA);
                var before = CaptureProfile(camera, pixelPerfectCamera);

                Assert.Throws<InvalidOperationException>(() => SimulationCameraProfileAuthoring.ValidateApprovedProfile(camera, pixelPerfectCamera));

                Assert.That(CaptureProfile(camera, pixelPerfectCamera), Is.EqualTo(before));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(cameraObject);
            }
        }

        [Test]
        public void ApplyApprovedProfile_RejectsDifferentCameraAndPixelPerfectObjectsBeforeWriting()
        {
            var cameraObject = new GameObject("Camera");
            var pixelPerfectObject = new GameObject("Pixel perfect");
            try
            {
                var camera = cameraObject.AddComponent<UnityEngine.Camera>();
                var pixelPerfectCamera = pixelPerfectObject.AddComponent<PixelPerfectCamera>();
                camera.orthographicSize = 7f;
                pixelPerfectCamera.assetsPPU = 3;

                Assert.Throws<InvalidOperationException>(() => SimulationCameraProfileAuthoring.ApplyApprovedProfile(camera, pixelPerfectCamera));

                Assert.That(camera.orthographicSize, Is.EqualTo(7f));
                Assert.That(pixelPerfectCamera.assetsPPU, Is.EqualTo(3));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(cameraObject);
                UnityEngine.Object.DestroyImmediate(pixelPerfectObject);
            }
        }

        [Test]
        public void ApplyApprovedProfile_RejectsScaledParentBeforeWriting()
        {
            var parent = new GameObject("Scaled parent");
            var cameraObject = new GameObject("Camera profile");
            cameraObject.transform.SetParent(parent.transform, false);
            parent.transform.localScale = new Vector3(2f, 1f, 1f);
            try
            {
                var camera = cameraObject.AddComponent<UnityEngine.Camera>();
                var pixelPerfectCamera = cameraObject.AddComponent<PixelPerfectCamera>();
                camera.orthographicSize = 7f;
                pixelPerfectCamera.assetsPPU = 3;

                Assert.Throws<InvalidOperationException>(() => SimulationCameraProfileAuthoring.ApplyApprovedProfile(camera, pixelPerfectCamera));

                Assert.That(camera.orthographicSize, Is.EqualTo(7f));
                Assert.That(pixelPerfectCamera.assetsPPU, Is.EqualTo(3));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(parent);
            }
        }

        [Test]
        public void ApplyApprovedProfile_RejectsTinyRotationBeforeWriting()
        {
            var cameraObject = new GameObject("Camera profile");
            try
            {
                var camera = cameraObject.AddComponent<UnityEngine.Camera>();
                var pixelPerfectCamera = cameraObject.AddComponent<PixelPerfectCamera>();
                camera.orthographicSize = 7f;
                camera.transform.rotation = Quaternion.Euler(0f, 0f, 0.001f);
                pixelPerfectCamera.assetsPPU = 3;

                Assert.Throws<InvalidOperationException>(() => SimulationCameraProfileAuthoring.ApplyApprovedProfile(camera, pixelPerfectCamera));

                Assert.That(camera.orthographicSize, Is.EqualTo(7f));
                Assert.That(pixelPerfectCamera.assetsPPU, Is.EqualTo(3));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(cameraObject);
            }
        }

        [Test]
        public void ApplyApprovedProfile_RejectsDisabledCameraBeforeWriting()
        {
            var cameraObject = new GameObject("Camera profile");
            try
            {
                var camera = cameraObject.AddComponent<UnityEngine.Camera>();
                var pixelPerfectCamera = cameraObject.AddComponent<PixelPerfectCamera>();
                camera.orthographicSize = 7f;
                camera.enabled = false;
                pixelPerfectCamera.assetsPPU = 3;

                Assert.Throws<InvalidOperationException>(() => SimulationCameraProfileAuthoring.ApplyApprovedProfile(camera, pixelPerfectCamera));

                Assert.That(camera.orthographicSize, Is.EqualTo(7f));
                Assert.That(pixelPerfectCamera.assetsPPU, Is.EqualTo(3));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(cameraObject);
            }
        }

        private static PixelPerfectCamera.PixelPerfectFilterMode ReadFilterMode(PixelPerfectCamera pixelPerfectCamera)
        {
            var serializedPixelPerfectCamera = new SerializedObject(pixelPerfectCamera);
            return (PixelPerfectCamera.PixelPerfectFilterMode)serializedPixelPerfectCamera.FindProperty("m_FilterMode").enumValueIndex;
        }

        private static void SetFilterMode(PixelPerfectCamera pixelPerfectCamera, PixelPerfectCamera.PixelPerfectFilterMode filterMode)
        {
            var serializedPixelPerfectCamera = new SerializedObject(pixelPerfectCamera);
            serializedPixelPerfectCamera.FindProperty("m_FilterMode").enumValueIndex = (int)filterMode;
            serializedPixelPerfectCamera.ApplyModifiedPropertiesWithoutUndo();
        }

        private static CameraProfile CaptureProfile(UnityEngine.Camera camera, PixelPerfectCamera pixelPerfectCamera)
        {
            return new CameraProfile(
                camera.orthographic,
                camera.orthographicSize,
                camera.transform.rotation,
                camera.transform.lossyScale,
                pixelPerfectCamera.assetsPPU,
                pixelPerfectCamera.refResolutionX,
                pixelPerfectCamera.refResolutionY,
                pixelPerfectCamera.cropFrame,
                pixelPerfectCamera.gridSnapping,
                ReadFilterMode(pixelPerfectCamera));
        }

        private readonly struct CameraProfile : IEquatable<CameraProfile>
        {
            private readonly bool _orthographic;
            private readonly float _orthographicSize;
            private readonly Quaternion _rotation;
            private readonly Vector3 _scale;
            private readonly int _assetsPpu;
            private readonly int _resolutionX;
            private readonly int _resolutionY;
            private readonly PixelPerfectCamera.CropFrame _cropFrame;
            private readonly PixelPerfectCamera.GridSnapping _gridSnapping;
            private readonly PixelPerfectCamera.PixelPerfectFilterMode _filterMode;

            internal CameraProfile(bool orthographic, float orthographicSize, Quaternion rotation, Vector3 scale,
                int assetsPpu, int resolutionX, int resolutionY, PixelPerfectCamera.CropFrame cropFrame,
                PixelPerfectCamera.GridSnapping gridSnapping, PixelPerfectCamera.PixelPerfectFilterMode filterMode)
            {
                _orthographic = orthographic;
                _orthographicSize = orthographicSize;
                _rotation = rotation;
                _scale = scale;
                _assetsPpu = assetsPpu;
                _resolutionX = resolutionX;
                _resolutionY = resolutionY;
                _cropFrame = cropFrame;
                _gridSnapping = gridSnapping;
                _filterMode = filterMode;
            }

            public bool Equals(CameraProfile other)
            {
                return _orthographic == other._orthographic &&
                       _orthographicSize.Equals(other._orthographicSize) &&
                       _rotation.Equals(other._rotation) &&
                       _scale.Equals(other._scale) &&
                       _assetsPpu == other._assetsPpu &&
                       _resolutionX == other._resolutionX &&
                       _resolutionY == other._resolutionY &&
                       _cropFrame == other._cropFrame &&
                       _gridSnapping == other._gridSnapping &&
                       _filterMode == other._filterMode;
            }

            public override bool Equals(object obj)
            {
                return obj is CameraProfile other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = _orthographic.GetHashCode();
                    hash = (hash * 397) ^ _orthographicSize.GetHashCode();
                    hash = (hash * 397) ^ _rotation.GetHashCode();
                    hash = (hash * 397) ^ _scale.GetHashCode();
                    hash = (hash * 397) ^ _assetsPpu;
                    hash = (hash * 397) ^ _resolutionX;
                    hash = (hash * 397) ^ _resolutionY;
                    hash = (hash * 397) ^ (int)_cropFrame;
                    hash = (hash * 397) ^ (int)_gridSnapping;
                    return (hash * 397) ^ (int)_filterMode;
                }
            }
        }
    }
}
