using System;
using System.Collections.Generic;
using System.Globalization;
using AcadeGameMaker.Camera;
using AcadeGameMaker.Camera.Authoring;
using AcadeGameMaker.Camera.Unity;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AcadeGameMaker.Combat.Authoring
{
    // REQ-COM-001, REQ-COM-003, REQ-COM-004, REQ-COM-006; REQ-WT-003/005.
    // This validator is root-bounded and read-only: no discovery, repair, or initialization.
    internal static class OrdanBossEncounterAuthoringValidator
    {
        internal static void Validate(GameObject root, bool requireScenePrefabInstance)
        {
            if (root == null || root.name != "OrdanBossEncounterGraph" || !root.activeInHierarchy || root.layer != 0) throw new InvalidOperationException("Boss graph root is invalid.");
            AssertTransform(root.transform, Vector3.zero); AssertComponents(root, typeof(Transform));
            if (root.transform.childCount != 9) throw new InvalidOperationException("Boss graph requires exactly nine children.");
            var player = Child(root.transform, 0, "CombatEncounterPlayer", new Vector3(-7f, .81f, 0f));
            var ordanRoot = Child(root.transform, 1, "Ordan", new Vector3(7f, 1.2f, 0f));
            var auditRoot = Child(root.transform, 2, "BossAuditBox-0", new Vector3(0f, .75f, 0f));
            var payloadRoots = new[] { Child(root.transform, 3, "BossWeight-0", new Vector3(3f, 6f, 0f)), Child(root.transform, 4, "BossWeight-1", new Vector3(5f, 6f, 0f)), Child(root.transform, 5, "BossWeight-2", new Vector3(7f, 6f, 0f)) };
            var systems = Child(root.transform, 6, "Systems", Vector3.zero); var camera = Child(root.transform, 7, "Camera", new Vector3(0f, 4.5f, -10f)); var environment = Child(root.transform, 8, "Environment", Vector3.zero);
            ValidatePlayer(player, requireScenePrefabInstance); var ordan = ValidateOrdan(ordanRoot); var audit = ValidateAudit(auditRoot);
            var payloads = new TransferTarget[3]; var sinks = new OrdanBossPayloadTransferModifierSink[3]; var colliders = new Collider2D[3];
            for (var i = 0; i < 3; i++) ValidatePayload(payloadRoots[i], i, out payloads[i], out sinks[i], out colliders[i]);
            ValidateEnvironment(environment);
            ValidateSystems(root, systems, player.gameObject, ordan, audit.Target, audit.Sink, payloads, sinks, colliders, camera);
            ValidatePrefabLink(root, requireScenePrefabInstance);
        }

        private static void ValidatePlayer(Transform value, bool requireScenePrefabInstance)
        {
            var capsule = value.GetComponent<CapsuleCollider2D>();
            if (value.childCount != 0 || value.gameObject.layer != 0 || value.GetComponent<PlayerMovementController>() == null || value.GetComponent<CombatTarget>() == null ||
                capsule == null || capsule.offset != Vector2.zero || capsule.size != new Vector2(.8f, 1.6f) || capsule.isTrigger || !capsule.enabled)
                throw new InvalidOperationException("Boss graph player is malformed.");
            var source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(value.gameObject);
            if (source == null ||
                AssetDatabase.GetAssetPath(source) != OrdanBossEncounterAuthoringBuilder.PlayerPrefabPath || source.name != "CombatEncounterPlayer")
                throw new InvalidOperationException("Boss graph must reuse the exact approved player prefab source.");
            if (requireScenePrefabInstance) ValidatePlayerInstanceOverrides(value, source);
        }

        // REQ-COM-001, REQ-COM-004. The nested player is intentionally placed
        // in the boss room; its transform defaults are the only permitted overrides.
        private static void ValidatePlayerInstanceOverrides(Transform player, GameObject sourceRoot)
        {
            if (PrefabUtility.GetAddedComponents(player.gameObject).Count != 0 || PrefabUtility.GetRemovedComponents(player.gameObject).Count != 0 ||
                PrefabUtility.GetAddedGameObjects(player.gameObject).Count != 0 || PrefabUtility.GetRemovedGameObjects(player.gameObject).Count != 0)
                throw new InvalidOperationException("Player nested prefab component or object override is forbidden.");
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var modifications = PrefabUtility.GetPropertyModifications(player.gameObject) ?? Array.Empty<PropertyModification>();
            for (var index = 0; index < modifications.Length; index++)
            {
                var modification = modifications[index];
                if (modification == null || modification.target == null || modification.objectReference != null || string.IsNullOrEmpty(modification.propertyPath))
                    throw new InvalidOperationException("Player nested prefab override is malformed.");
                var originalTarget = PrefabUtility.GetCorrespondingObjectFromOriginalSource(modification.target) ?? modification.target;
                if (ReferenceEquals(originalTarget, sourceRoot))
                {
                    if (!seen.Add("GameObject:" + modification.propertyPath)) throw new InvalidOperationException("Player nested prefab override is duplicated.");
                    if (modification.propertyPath != "m_Name" || modification.value != "CombatEncounterPlayer") throw new InvalidOperationException("Player name override is invalid.");
                    continue;
                }
                if (ReferenceEquals(originalTarget, sourceRoot.transform))
                {
                    if (!seen.Add("Transform:" + modification.propertyPath)) throw new InvalidOperationException("Player nested prefab override is duplicated.");
                    if (!IsAllowedPlayerTransformOverride(modification.propertyPath, modification.value))
                        throw new InvalidOperationException("Player nested prefab has a foreign transform override: " + modification.propertyPath + "=" + modification.value);
                    continue;
                }
                // Scene-level graph modifications are validated separately as
                // an exact zero-override set. Only modifications whose original
                // source belongs to the nested player are in scope here.
                if (AssetDatabase.GetAssetPath(originalTarget) == OrdanBossEncounterAuthoringBuilder.PlayerPrefabPath)
                    throw new InvalidOperationException("Player nested prefab has a foreign override: " + originalTarget.GetType().Name + "/" + originalTarget.name + "/" + modification.propertyPath + "=" + modification.value);
            }
            // Unity 6000.6 may collapse redundant nested default-property
            // entries when queried through the outer scene instance. Child()
            // already proves the exact live pose; every surfaced nested-player
            // modification is still restricted to the allowlist above.
        }
        private static bool IsAllowedPlayerTransformOverride(string path, string value)
        {
            if (path == "m_LocalPosition.x") return ExactFloat(value, -7f);
            if (path == "m_LocalPosition.y") return ExactFloat(value, .81f);
            if (path == "m_LocalPosition.z") return ExactFloat(value, 0f);
            if (path == "m_LocalRotation.x" || path == "m_LocalRotation.y" || path == "m_LocalRotation.z" || path == "m_LocalEulerAnglesHint.x" || path == "m_LocalEulerAnglesHint.y" || path == "m_LocalEulerAnglesHint.z") return ExactFloat(value, 0f);
            if (path == "m_LocalRotation.w") return ExactFloat(value, 1f);
            return false;
        }
        private static bool ExactFloat(string value, float expected) => float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) && parsed == expected;

        private static CombatTarget ValidateOrdan(Transform value)
        {
            AssertLeaf(value); AssertComponents(value.gameObject, typeof(Transform), typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CombatTarget));
            var body = value.GetComponent<Rigidbody2D>(); var collider = value.GetComponent<BoxCollider2D>(); var target = value.GetComponent<CombatTarget>();
            AssertBody(body); AssertBox(collider, new Vector2(1.6f, 2.4f), true); target.ValidateForAuthoring();
            var frozen = target.Freeze();
            if (frozen.TargetId != "ordan" || frozen.MaxHealth != 60 || frozen.InvulnerabilityTicks != 0 || !frozen.CanReceivePlayerBasicAttack || frozen.HasMatchingTransferTarget ||
                frozen.TargetCollider != collider || frozen.TargetPoseRoot != value || frozen.LocalAimPointXQ1000 != 0 || frozen.LocalAimPointYQ1000 != 0 || frozen.LocalAimShapeCenterXQ1000 != 0 || frozen.LocalAimShapeCenterYQ1000 != 0 || frozen.LocalAimShapeHalfExtentXQ1000 != 800 || frozen.LocalAimShapeHalfExtentYQ1000 != 1200)
                throw new InvalidOperationException("Ordan Combat authoring is not exact.");
            return target;
        }

        private static (TransferTarget Target, OrdanBossAuditTransferModifierSink Sink) ValidateAudit(Transform value)
        {
            AssertLeaf(value); AssertComponents(value.gameObject, typeof(Transform), typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(OrdanBossAuditTransferModifierSink), typeof(TransferTarget));
            var collider = value.GetComponent<BoxCollider2D>(); var sink = value.GetComponent<OrdanBossAuditTransferModifierSink>(); var target = value.GetComponent<TransferTarget>();
            AssertBody(value.GetComponent<Rigidbody2D>()); AssertBox(collider, new Vector2(1.2f, 1.2f), false); target.ValidateForAuthoring();
            var descriptor = target.BuildDescriptorFromValidatedAuthoring();
            if (descriptor.TargetId != OrdanBossAuditTransferModifierSink.RequiredTargetId || descriptor.Kind != TransferTargetKind.Box || descriptor.BaseModifierProfileId != OrdanBossAuditTransferModifierSink.RequiredBaseProfileId ||
                descriptor.AimShapeHalfExtentXQ1000 != 600 || descriptor.AimShapeHalfExtentYQ1000 != 600 || descriptor.IsAvailable || sink.IsExposed || sink.IsApplied || sink.LastAcceptedRevision != 0 || sink.BaseModifierProfileId != OrdanBossAuditTransferModifierSink.RequiredBaseProfileId || collider.enabled)
                throw new InvalidOperationException("Audit target initial state is not exact.");
            return (target, sink);
        }

        private static void ValidatePayload(Transform value, int index, out TransferTarget target, out OrdanBossPayloadTransferModifierSink sink, out Collider2D collider)
        {
            AssertLeaf(value); AssertComponents(value.gameObject, typeof(Transform), typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(OrdanBossPayloadTransferModifierSink), typeof(TransferTarget));
            var box = value.GetComponent<BoxCollider2D>(); sink = value.GetComponent<OrdanBossPayloadTransferModifierSink>(); target = value.GetComponent<TransferTarget>(); collider = box;
            AssertBody(value.GetComponent<Rigidbody2D>()); AssertBox(box, Vector2.one, false); target.ValidateForAuthoring(); var descriptor = target.BuildDescriptorFromValidatedAuthoring();
            if (descriptor.TargetId != "BossWeight-" + index || descriptor.Kind != TransferTargetKind.BossPayload || descriptor.BaseModifierProfileId != OrdanBossPayloadTransferModifierSink.RequiredBaseProfileId ||
                descriptor.AimShapeHalfExtentXQ1000 != 500 || descriptor.AimShapeHalfExtentYQ1000 != 500 || descriptor.IsAvailable || sink.IsExposed || sink.IsApplied || sink.LastAcceptedRevision != 0 || !string.Equals(sink.BoundTargetId, descriptor.TargetId, StringComparison.Ordinal) || !ReferenceEquals(target.TargetOwnerSink, sink) || box.enabled)
                throw new InvalidOperationException("Payload target initial state is not exact.");
        }

        private static void ValidateSystems(GameObject root, Transform value, GameObject playerRoot, CombatTarget ordan, TransferTarget audit, OrdanBossAuditTransferModifierSink auditSink,
            TransferTarget[] payloads, OrdanBossPayloadTransferModifierSink[] sinks, Collider2D[] colliders, Transform cameraRoot)
        {
            AssertLeaf(value); AssertComponents(value.gameObject, typeof(Transform), typeof(TransferTargetRegistry), typeof(CombatTargetRegistry), typeof(TransferSimulationDriver), typeof(OrdanBossCombatSimulationDriver), typeof(OrdanBossSimulationDriver), typeof(OrdanBossTransferExposureScheduler), typeof(OrdanBossHostileDamageProducer), typeof(OrdanBossBalanceAuditPullProducer), typeof(OrdanBossTerminalTeardown), typeof(OrdanBossEncounterHandoffAdapter), typeof(InputRouter), typeof(OrdanBossTerminalTransitionRequester));
            var transferRegistry = value.GetComponent<TransferTargetRegistry>(); var combatRegistry = value.GetComponent<CombatTargetRegistry>(); var transfer = value.GetComponent<TransferSimulationDriver>();
            var combat = value.GetComponent<OrdanBossCombatSimulationDriver>(); var bridge = value.GetComponent<OrdanBossSimulationDriver>(); var scheduler = value.GetComponent<OrdanBossTransferExposureScheduler>(); var hostile = value.GetComponent<OrdanBossHostileDamageProducer>(); var auditPull = value.GetComponent<OrdanBossBalanceAuditPullProducer>(); var teardown = value.GetComponent<OrdanBossTerminalTeardown>(); var handoff = value.GetComponent<OrdanBossEncounterHandoffAdapter>(); var router = value.GetComponent<InputRouter>(); var requester = value.GetComponent<OrdanBossTerminalTransitionRequester>();
            transferRegistry.ValidateForAuthoring(); var targets = transferRegistry.Targets;
            if (targets.Count != 4 || targets[0] != audit || targets[1] != payloads[0] || targets[2] != payloads[1] || targets[3] != payloads[2]) throw new InvalidOperationException("Boss Transfer registry is not exact.");
            var roster = combatRegistry.ReadValidatedAuthoringRoster(); if (roster.Count != 2 || roster[0].TargetId != "ordan" || roster[1].TargetId != "player" || roster[0].TargetCollider != ordan.GetComponent<BoxCollider2D>()) throw new InvalidOperationException("Boss Combat registry is not exact.");
            var player = playerRoot.GetComponent<PlayerMovementController>();
            if (transfer.BoundPlayer != player || transfer.BoundRegistry != transferRegistry || combat.BoundPlayer != player || combat.BoundTransferDriver != transfer || combat.BoundRegistry != combatRegistry || combat.BoundHostileProducer != hostile || bridge.BoundTransferDriver != transfer || scheduler.BoundBridge != bridge || scheduler.BoundTransferDriver != transfer || scheduler.ConfigurationModeForAuthoring != OrdanBossTransferExposureConfigurationMode.ProductionAuditAndPayload || scheduler.BoundAuditTarget != audit || scheduler.BoundAuditSink != auditSink || scheduler.BoundAuditCollider != audit.TargetCollider || hostile.BoundPlayer != player || hostile.BoundTransfer != transfer || hostile.BoundCombat != combat || hostile.BoundBridge != bridge || hostile.BoundScheduler != scheduler || auditPull.BoundPlayer != player || auditPull.BoundTransfer != transfer || auditPull.BoundCombat != combat || auditPull.BoundBridge != bridge || auditPull.BoundScheduler != scheduler)
                throw new InvalidOperationException("Boss systems references are cross-wired.");
            ValidateHostileGeometryConstants();
            ValidateBalanceAuditPullConstants(auditPull);
            teardown.ValidateForAuthoring(root);
            handoff.ValidateEncounterGraph(root, player, transfer, combat, bridge, scheduler, ordan, audit, auditSink, payloads, sinks, colliders);
            ValidateCamera(root, cameraRoot, player, router);
            if (router == null || requester == null || router.BoundPlayerForAuthoring != player || router.BoundTransferForAuthoring != transfer ||
                router.BoundCombatForAuthoring != combat || router.BoundTerminalTeardownForAuthoring != teardown ||
                router.BoundCameraProviderForAuthoring != cameraRoot.GetComponent<SimulationCameraDriver>() || router.AuthoredInitialMode != InputMode.GameplayEnabled)
                throw new InvalidOperationException("Boss InputRouter authored bindings are not exact.");
            requester.ValidateForAuthoring(root, router, player, combat, handoff);
            AssertOrder(typeof(OrdanBossTerminalTransitionRequester), -211); AssertOrder(typeof(InputRouter), -210); AssertOrder(typeof(TransferSimulationDriver), -200); AssertOrder(typeof(OrdanBossTerminalTeardown), -195); AssertOrder(typeof(OrdanBossCombatSimulationDriver), -190); AssertOrder(typeof(OrdanBossSimulationDriver), -185); AssertOrder(typeof(OrdanBossTransferExposureScheduler), -180); AssertOrder(typeof(OrdanBossHostileDamageProducer), -170); AssertOrder(typeof(OrdanBossBalanceAuditPullProducer), -165); AssertOrder(typeof(OrdanBossEncounterHandoffAdapter), 110);
        }

        private static void ValidateCamera(GameObject root, Transform value, PlayerMovementController player, InputRouter router)
        {
            AssertLeaf(value); AssertComponents(value.gameObject, typeof(Transform), typeof(UnityEngine.Camera), typeof(PixelPerfectCamera), typeof(SimulationCameraDriver));
            var camera = value.GetComponent<UnityEngine.Camera>(); var pixelPerfect = value.GetComponent<PixelPerfectCamera>(); var driver = value.GetComponent<SimulationCameraDriver>();
            if (value.root.gameObject != root || driver.BoundPlayer != player || router == null || router.BoundCameraProviderForAuthoring != driver ||
                driver.transform.localPosition != new Vector3(0f, 4.5f, -10f)) throw new InvalidOperationException("Boss camera bindings are cross-wired.");
            SimulationCameraProfileAuthoring.ValidateApprovedProfile(camera, pixelPerfect);
            var serialized = new SerializedObject(driver);
            if (serialized.FindProperty("_initialCenterXPixel18").intValue != 0 || serialized.FindProperty("_initialCenterYPixel18").intValue != 81 ||
                serialized.FindProperty("_minXPixel18").intValue != 0 || serialized.FindProperty("_maxXPixel18").intValue != 0 ||
                serialized.FindProperty("_minYPixel18").intValue != 81 || serialized.FindProperty("_maxYPixel18").intValue != 81)
                throw new InvalidOperationException("Boss camera fixed-room bounds are not exact.");
        }

        private static void ValidateBalanceAuditPullConstants(OrdanBossBalanceAuditPullProducer auditPull)
        {
            if (OrdanBossBalanceAuditPullProducer.ApprovedAnchorXQ4096 != 28672 ||
                OrdanBossBalanceAuditPullProducer.ApprovedAnchorYQ4096 != 4915 ||
                OrdanBossBalanceAuditPullProducer.ApprovedMaximumStepQ4096 != 410 ||
                auditPull.AuthoredAnchorXQ4096 != 28672 || auditPull.AuthoredAnchorYQ4096 != 4915 ||
                auditPull.AuthoredMaximumStepQ4096 != 410)
                throw new InvalidOperationException("Boss BalanceAudit pull constants are not exact.");
        }

        private static void ValidateHostileGeometryConstants()
        {
            var occluders = OrdanBossHostileDamageProducer.ApprovedOccluders;
            var starts = OrdanBossHostileDamageProducer.ApprovedSeizureStartXs;
            var normal = OrdanBossHostileDamageProducer.ApprovedNormalDurations;
            var accelerated = OrdanBossHostileDamageProducer.ApprovedAcceleratedDurations;
            if (OrdanBossHostileDamageProducer.ApprovedPlayerHalfWidth != 1639 || OrdanBossHostileDamageProducer.ApprovedPlayerHalfHeight != 3277 ||
                OrdanBossHostileDamageProducer.ApprovedProjectileHalfExtent != 1024 || OrdanBossHostileDamageProducer.ApprovedSourceX != 25395 ||
                OrdanBossHostileDamageProducer.ApprovedSourceY != 4915 || OrdanBossHostileDamageProducer.ApprovedDebtEndX != -45056 ||
                !OrdanBossHostileDamageProducer.ApprovedArena.Equals(new OrdanBossHostileDamageProducer.ClosedAabbQ4096(-47104, 0, 47104, 40960)) ||
                !OrdanBossHostileDamageProducer.ApprovedOrdanBody.Equals(OrdanBossHostileDamageProducer.ClosedAabbQ4096.FromCenterHalf(28672, 4915, 3277, 4915)) ||
                !OrdanBossHostileDamageProducer.ApprovedDebtLineA.Equals(new OrdanBossHostileDamageProducer.ClosedAabbQ4096(-45056, 3891, 25395, 5939)) ||
                !OrdanBossHostileDamageProducer.ApprovedDebtLineB.Equals(new OrdanBossHostileDamageProducer.ClosedAabbQ4096(-45056, 11264, 25395, 13312)) ||
                starts.Count != 3 || starts[0] != 12288 || starts[1] != 20480 || starts[2] != 28672 ||
                OrdanBossHostileDamageProducer.ApprovedSeizureStartY != 24576 || OrdanBossHostileDamageProducer.ApprovedSeizurePivotX != 8192 ||
                OrdanBossHostileDamageProducer.ApprovedSeizurePivotY != 12288 || OrdanBossHostileDamageProducer.ApprovedSeizureEndX != -28672 ||
                OrdanBossHostileDamageProducer.ApprovedSeizureEndY != 4915 || OrdanBossHostileDamageProducer.ApprovedNormalPathPointCount != 3 ||
                OrdanBossHostileDamageProducer.ApprovedAcceleratedPathPointCount != 2 || normal.Count != 3 || normal[0] != 72 || normal[1] != 66 || normal[2] != 60 ||
                accelerated.Count != 3 || accelerated[0] != 24 || accelerated[1] != 22 || accelerated[2] != 20 ||
                OrdanBossHostileDamageProducer.ApprovedAuditCenterX != 28672 || OrdanBossHostileDamageProducer.ApprovedAuditCenterY != 4915 ||
                OrdanBossHostileDamageProducer.ApprovedAuditInnerRadius != 49152 || OrdanBossHostileDamageProducer.ApprovedAuditOuterRadius != 65536 || occluders.Count != 3 ||
                !occluders[0].Equals(OrdanBossHostileDamageProducer.ClosedAabbQ4096.FromCenterHalf(0, -2048, 49152, 2048)) ||
                !occluders[1].Equals(OrdanBossHostileDamageProducer.ClosedAabbQ4096.FromCenterHalf(-49152, 18432, 2048, 22528)) ||
                !occluders[2].Equals(OrdanBossHostileDamageProducer.ClosedAabbQ4096.FromCenterHalf(49152, 18432, 2048, 22528)))
                throw new InvalidOperationException("Boss hostile Q4096 descriptor is not exact.");
        }

        private static void ValidateEnvironment(Transform value)
        {
            AssertComponents(value.gameObject, typeof(Transform)); if (value.childCount != 3) throw new InvalidOperationException("Environment needs exact bounds.");
            ValidateEnvironmentBox(Child(value, 0, "Ground", new Vector3(0f, -.5f, 0f), 8), new Vector2(24f, 1f));
            ValidateEnvironmentBox(Child(value, 1, "LeftBound", new Vector3(-12f, 4.5f, 0f), 8), new Vector2(1f, 11f));
            ValidateEnvironmentBox(Child(value, 2, "RightBound", new Vector3(12f, 4.5f, 0f), 8), new Vector2(1f, 11f));
        }
        private static void ValidateEnvironmentBox(Transform value, Vector2 size) { AssertLeaf(value); if (value.gameObject.layer != 8) throw new InvalidOperationException("Environment layer is invalid."); AssertComponents(value.gameObject, typeof(Transform), typeof(BoxCollider2D)); AssertBox(value.GetComponent<BoxCollider2D>(), size, true); }
        private static void ValidatePrefabLink(GameObject root, bool requireScenePrefabInstance)
        {
            if (!requireScenePrefabInstance) return;
            var source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(root);
            if (PrefabUtility.GetPrefabInstanceStatus(root) != PrefabInstanceStatus.Connected || source == null ||
                AssetDatabase.GetAssetPath(source) != OrdanBossEncounterAuthoringBuilder.GraphPrefabPath || root.transform.parent != null ||
                PrefabUtility.GetAddedComponents(root).Count != 0 || PrefabUtility.GetRemovedComponents(root).Count != 0 ||
                PrefabUtility.GetAddedGameObjects(root).Count != 0 || PrefabUtility.GetRemovedGameObjects(root).Count != 0)
                throw new InvalidOperationException("Boss scene requires one connected graph prefab instance without structural overrides.");
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var modifications = PrefabUtility.GetPropertyModifications(root) ?? Array.Empty<PropertyModification>();
            for (var index = 0; index < modifications.Length; index++)
            {
                var modification = modifications[index];
                if (modification == null || modification.target == null || modification.objectReference != null || string.IsNullOrEmpty(modification.propertyPath))
                    throw new InvalidOperationException("Boss scene prefab override is malformed.");
                var originalTarget = PrefabUtility.GetCorrespondingObjectFromOriginalSource(modification.target) ?? modification.target;
                if (ReferenceEquals(originalTarget, source))
                {
                    if (!seen.Add("GameObject:" + modification.propertyPath)) throw new InvalidOperationException("Boss scene prefab override is duplicated.");
                    if (modification.propertyPath != "m_Name" || modification.value != "OrdanBossEncounterGraph") throw new InvalidOperationException("Boss scene root name override is invalid.");
                    continue;
                }
                if (ReferenceEquals(originalTarget, source.transform) && !seen.Add("Transform:" + modification.propertyPath))
                    throw new InvalidOperationException("Boss scene prefab override is duplicated.");
                if (!ReferenceEquals(originalTarget, source.transform) || !IsAllowedRootTransformOverride(modification.propertyPath, modification.value))
                    throw new InvalidOperationException("Boss scene prefab has a foreign property override.");
            }
            if (seen.Count != 11) throw new InvalidOperationException("Boss scene prefab requires the exact eleven default name and local-pose modifications.");
        }
        private static bool IsAllowedRootTransformOverride(string path, string value)
        {
            if (path == "m_LocalPosition.x" || path == "m_LocalPosition.y" || path == "m_LocalPosition.z" ||
                path == "m_LocalRotation.x" || path == "m_LocalRotation.y" || path == "m_LocalRotation.z" ||
                path == "m_LocalEulerAnglesHint.x" || path == "m_LocalEulerAnglesHint.y" || path == "m_LocalEulerAnglesHint.z") return ExactFloat(value, 0f);
            if (path == "m_LocalRotation.w") return ExactFloat(value, 1f);
            return false;
        }
        private static Transform Child(Transform parent, int index, string name, Vector3 position, int layer = 0) { var child = parent.GetChild(index); if (child.name != name || !child.gameObject.activeSelf || child.gameObject.layer != layer) throw new InvalidOperationException("Boss graph child identity is invalid."); AssertTransform(child, position); return child; }
        private static void AssertLeaf(Transform value) { if (value.childCount != 0) throw new InvalidOperationException("Actor target must be a leaf."); }
        private static void AssertTransform(Transform value, Vector3 position) { if (value.localPosition != position || value.localEulerAngles.z != 0f || value.localScale.x != 1f || value.localScale.y != 1f) throw new InvalidOperationException("Authored transform is invalid."); }
        private static void AssertBody(Rigidbody2D body) { if (body == null || body.bodyType != RigidbodyType2D.Kinematic || body.gravityScale != 0f || body.constraints != RigidbodyConstraints2D.FreezeRotation || body.interpolation != RigidbodyInterpolation2D.None) throw new InvalidOperationException("Kinematic body profile is invalid."); }
        private static void AssertBox(BoxCollider2D collider, Vector2 size, bool enabled) { if (collider == null || collider.isTrigger || collider.offset != Vector2.zero || collider.size != size || collider.enabled != enabled) throw new InvalidOperationException("Collider profile is invalid."); }
        private static void AssertComponents(GameObject value, params Type[] expected) { var actual = value.GetComponents<Component>(); if (actual.Length != expected.Length) throw new InvalidOperationException("Component count is invalid."); for (var i = 0; i < expected.Length; i++) if (actual[i] == null || actual[i].GetType() != expected[i]) throw new InvalidOperationException("Component order is invalid."); }
        private static void AssertOrder(Type type, int expected) { var attribute = Attribute.GetCustomAttribute(type, typeof(DefaultExecutionOrder)) as DefaultExecutionOrder; if (attribute == null || attribute.order != expected) throw new InvalidOperationException("Execution order is invalid."); }
    }
}
