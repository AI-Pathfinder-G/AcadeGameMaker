using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AcadeGameMaker.Camera.Authoring
{
    // REQ-ART-009/010/011. Editor-only authoring seam for the approved M5C2 fixed-room profile.
    internal static class SimulationCameraProfileAuthoring
    {
        private const string FilterModePropertyName = "m_FilterMode";

        internal static void ApplyApprovedProfile(UnityEngine.Camera camera, PixelPerfectCamera pixelPerfectCamera)
        {
            ValidateReferencesAndCoLocation(camera, pixelPerfectCamera);
            ValidateActiveAndEnabled(camera, pixelPerfectCamera);
            ValidateExactIdentityRotation(camera.transform.rotation);
            ValidateFiniteZAndUnitWorldScale(camera);

            var serializedPixelPerfectCamera = new SerializedObject(pixelPerfectCamera);
            FindPointFilterMode(serializedPixelPerfectCamera);

            camera.orthographic = true;
            camera.orthographicSize = 10f;

            pixelPerfectCamera.assetsPPU = 18;
            pixelPerfectCamera.refResolutionX = 640;
            pixelPerfectCamera.refResolutionY = 360;
            pixelPerfectCamera.cropFrame = PixelPerfectCamera.CropFrame.Windowbox;
            pixelPerfectCamera.gridSnapping = PixelPerfectCamera.GridSnapping.UpscaleRenderTexture;

            serializedPixelPerfectCamera.Update();
            var filterMode = FindPointFilterMode(serializedPixelPerfectCamera);
            filterMode.enumValueIndex = (int)PixelPerfectCamera.PixelPerfectFilterMode.Point;
            serializedPixelPerfectCamera.ApplyModifiedPropertiesWithoutUndo();

            ValidateApprovedProfile(camera, pixelPerfectCamera);
        }

        internal static void ValidateApprovedProfile(UnityEngine.Camera camera, PixelPerfectCamera pixelPerfectCamera)
        {
            ValidateReferencesAndCoLocation(camera, pixelPerfectCamera);

            ValidateActiveAndEnabled(camera, pixelPerfectCamera);

            if (!camera.orthographic || camera.orthographicSize != 10f)
            {
                throw new InvalidOperationException("Camera must use the approved orthographic size of 10.");
            }

            ValidateExactIdentityRotation(camera.transform.rotation);

            ValidateFiniteZAndUnitWorldScale(camera);

            if (pixelPerfectCamera.assetsPPU != 18 ||
                pixelPerfectCamera.refResolutionX != 640 ||
                pixelPerfectCamera.refResolutionY != 360 ||
                pixelPerfectCamera.cropFrame != PixelPerfectCamera.CropFrame.Windowbox ||
                pixelPerfectCamera.gridSnapping != PixelPerfectCamera.GridSnapping.UpscaleRenderTexture)
            {
                throw new InvalidOperationException("PixelPerfectCamera does not match the approved 18 PPU, 640x360 Windowbox/Upscale profile.");
            }

            var serializedPixelPerfectCamera = new SerializedObject(pixelPerfectCamera);
            var filterMode = FindPointFilterMode(serializedPixelPerfectCamera);
            if (filterMode.enumValueIndex != (int)PixelPerfectCamera.PixelPerfectFilterMode.Point)
            {
                throw new InvalidOperationException("PixelPerfectCamera must use the approved Point filter mode.");
            }
        }

        private static void ValidateReferencesAndCoLocation(UnityEngine.Camera camera, PixelPerfectCamera pixelPerfectCamera)
        {
            if (camera == null)
            {
                throw new ArgumentNullException(nameof(camera));
            }

            if (pixelPerfectCamera == null)
            {
                throw new ArgumentNullException(nameof(pixelPerfectCamera));
            }

            if (camera.gameObject != pixelPerfectCamera.gameObject)
            {
                throw new InvalidOperationException("Camera and PixelPerfectCamera must be co-located on the exact same GameObject.");
            }
        }

        private static SerializedProperty FindPointFilterMode(SerializedObject serializedPixelPerfectCamera)
        {
            var filterMode = serializedPixelPerfectCamera.FindProperty(FilterModePropertyName);
            if (filterMode == null || filterMode.propertyType != SerializedPropertyType.Enum)
            {
                throw new InvalidOperationException("Installed PixelPerfectCamera does not expose an enum m_FilterMode serialized property.");
            }

            var pointIndex = (int)PixelPerfectCamera.PixelPerfectFilterMode.Point;
            if (pointIndex < 0 || pointIndex >= filterMode.enumNames.Length || filterMode.enumNames[pointIndex] != "Point")
            {
                throw new InvalidOperationException("Installed PixelPerfectCamera Point filter enum does not match the approved profile.");
            }

            return filterMode;
        }

        private static void ValidateFiniteZAndUnitWorldScale(UnityEngine.Camera camera)
        {
            var scale = camera.transform.lossyScale;
            if (scale.x != 1f || scale.y != 1f || scale.z != 1f)
            {
                throw new InvalidOperationException("Camera must retain exact unit world scale; a scaled parent is not an approved profile.");
            }

            var z = camera.transform.position.z;
            if (float.IsNaN(z) || float.IsInfinity(z))
            {
                throw new InvalidOperationException("Camera Z must be finite.");
            }
        }

        private static bool HasExactIdentityRotation(Quaternion rotation)
        {
            return rotation.x == 0f && rotation.y == 0f && rotation.z == 0f && rotation.w == 1f;
        }

        private static void ValidateActiveAndEnabled(UnityEngine.Camera camera, PixelPerfectCamera pixelPerfectCamera)
        {
            if (!camera.gameObject.activeInHierarchy || !camera.isActiveAndEnabled || !pixelPerfectCamera.isActiveAndEnabled)
            {
                throw new InvalidOperationException("Camera and PixelPerfectCamera must be active and enabled.");
            }
        }

        private static void ValidateExactIdentityRotation(Quaternion rotation)
        {
            if (!HasExactIdentityRotation(rotation))
            {
                throw new InvalidOperationException("Camera must retain exact zero world rotation.");
            }
        }
    }
}
