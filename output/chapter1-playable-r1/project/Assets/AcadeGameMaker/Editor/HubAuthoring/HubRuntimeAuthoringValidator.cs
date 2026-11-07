using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using UnityEditor;
using UnityEngine;

namespace AcadeGameMaker.Hub.Authoring.Editor
{
    // REQ-M5D7Q0-008/009. Observation-only validator: it never repairs a
    // candidate and never reaches into runtime private fields by string.
    internal static class HubRuntimeAuthoringValidator
    {
        internal static void ValidateCanonicalAsset(GameObject root)
        {
            if (root == null || AssetDatabase.GetAssetPath(root) != HubRuntimeAuthoringBuilder.PrefabPath || !PrefabUtility.IsPartOfPrefabAsset(root))
                throw new InvalidOperationException("Hub canonical prefab path is malformed.");
            if (PrefabUtility.GetPrefabAssetType(root) != PrefabAssetType.Regular)
                throw new InvalidOperationException("Hub canonical prefab must be a regular prefab, not a variant.");
            if (root.name != "HubRuntimeRoot" || !root.activeSelf)
                throw new InvalidOperationException("Hub canonical prefab root is malformed.");
            if (root.transform.childCount != 0)
                throw new InvalidOperationException("Hub canonical prefab child matrix is malformed.");
            var modifications = PrefabUtility.GetPropertyModifications(root);
            if (modifications != null && modifications.Length != 0)
                throw new InvalidOperationException("Hub canonical prefab must not have overrides.");
            var components = root.GetComponents<Component>();
            if (Array.Exists(components, component => component == null))
                throw new InvalidOperationException("Hub canonical prefab has a missing script.");
            // Check the forbidden-reference matrix before component cardinality
            // and order. A same-root production component is deliberately used
            // by the drift fixtures so its serialized router reference survives
            // prefab save/load and remains diagnostically distinct.
            var router = root.GetComponent<InputRouter>();
            if (router != null && router.HasAnyForbiddenHubReferenceForAuthoring)
                throw new InvalidOperationException("Hub canonical forbidden-reference matrix is malformed.");
            // The canonical root admits no component beyond the exact runtime
            // cohort.  Keep an added Camera/UI/gameplay/transition component
            // diagnostically distinct from a permutation of that cohort.
            if (components.Length != 4)
                throw new InvalidOperationException("Hub canonical forbidden component is malformed.");
            if (!(components[0] is Transform) || !(components[1] is InputRouter) ||
                !(components[2] is DesktopProfileLaunchAdapterV1) || !(components[3] is HubEntryHandoffLatchV1))
                throw new InvalidOperationException("Hub canonical component order is malformed.");
            router = (InputRouter)components[1];
            var adapter = (DesktopProfileLaunchAdapterV1)components[2];
            var latch = (HubEntryHandoffLatchV1)components[3];
            if (!router.enabled || !adapter.enabled || !latch.enabled) throw new InvalidOperationException("Hub canonical enabled matrix is malformed.");
            if (!router.AuthoredHubUiOnly || !router.HubUiOnlyIntentProofForAuthoring) throw new InvalidOperationException("Hub canonical discriminator is malformed.");
            if (router.AuthoredInitialMode != InputMode.UIOnly) throw new InvalidOperationException("Hub canonical mode is malformed.");
            if (!ReferenceEquals(adapter.BoundRouter, router) || !ReferenceEquals(latch.BoundLaunchAdapterForAuthoring, adapter) || !ReferenceEquals(latch.BoundRouterForAuthoring, router))
                throw new InvalidOperationException("Hub canonical sibling identity is malformed.");
        }
    }
}
