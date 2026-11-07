using System;
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
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AcadeGameMaker.Combat.Authoring
{
    // REQ-COM-001, REQ-COM-003, REQ-COM-004, REQ-COM-006. The sole M4B3A
    // asset writer; it never discovers, repairs, or writes an existing graph.
    internal static class OrdanBossEncounterAuthoringBuilder
    {
        internal const string ScenePath = "Assets/Scenes/OrdanBossEncounterSandbox.unity";
        internal const string GraphPrefabPath = "Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab";
        internal const string PlayerPrefabPath = "Assets/Prefabs/Player/CombatEncounterPlayer.prefab";

        [MenuItem("AcadeGameMaker/Build Ordan Boss Encounter")]
        private static void BuildFromMenu() => Build();

        internal static void Build()
        {
            EnsureFolder("Assets/Prefabs"); EnsureFolder("Assets/Prefabs/Combat");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            if (!EditorSceneManager.SaveScene(scene, ScenePath)) throw new InvalidOperationException("Could not establish the named Ordan scene.");
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (playerPrefab == null) throw new InvalidOperationException("The approved player prefab is absent.");
            var graph = BuildGraph(playerPrefab);
            OrdanBossEncounterAuthoringValidator.Validate(graph, false);
            var prefab = PrefabUtility.SaveAsPrefabAsset(graph, GraphPrefabPath);
            UnityEngine.Object.DestroyImmediate(graph);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            OrdanBossEncounterAuthoringValidator.Validate(instance, true);
            if (!EditorSceneManager.SaveScene(scene, ScenePath)) throw new InvalidOperationException("Could not save the Ordan scene.");
            AssetDatabase.SaveAssets();
        }

        private static GameObject BuildGraph(GameObject playerPrefab)
        {
            var root = new GameObject("OrdanBossEncounterGraph"); root.SetActive(false);
            var player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, root.scene); player.transform.SetParent(root.transform, false);
            player.transform.localPosition = new Vector3(-7f, .81f, 0f);
            var ordan = CreateOrdan(root.transform);
            var audit = CreateAudit(root.transform);
            var weights = new TransferTarget[3]; var sinks = new OrdanBossPayloadTransferModifierSink[3]; var colliders = new Collider2D[3];
            for (var i = 0; i < 3; i++) CreatePayload(root.transform, i, out weights[i], out sinks[i], out colliders[i]);
            var systems = new GameObject("Systems"); systems.transform.SetParent(root.transform, false);
            var transferRegistry = systems.AddComponent<TransferTargetRegistry>();
            var combatRegistry = systems.AddComponent<CombatTargetRegistry>();
            var transfer = systems.AddComponent<TransferSimulationDriver>();
            var combat = systems.AddComponent<OrdanBossCombatSimulationDriver>();
            var bridge = systems.AddComponent<OrdanBossSimulationDriver>();
            var scheduler = systems.AddComponent<OrdanBossTransferExposureScheduler>();
            var hostile = systems.AddComponent<OrdanBossHostileDamageProducer>();
            var auditPull = systems.AddComponent<OrdanBossBalanceAuditPullProducer>();
            var teardown = systems.AddComponent<OrdanBossTerminalTeardown>();
            var handoff = systems.AddComponent<OrdanBossEncounterHandoffAdapter>();
            var router = systems.AddComponent<InputRouter>();
            var requester = systems.AddComponent<OrdanBossTerminalTransitionRequester>();
            var playerMovement = player.GetComponent<PlayerMovementController>(); var playerCombat = player.GetComponent<CombatTarget>();
            if (playerMovement == null || playerCombat == null) throw new InvalidOperationException("Approved player prefab is malformed.");
            transferRegistry.ConfigureForAuthoring(audit.Target, weights[0], weights[1], weights[2]);
            combatRegistry.ConfigureForAuthoring(ordan, playerCombat);
            transfer.ConfigureForAuthoring(playerMovement, transferRegistry);
            combat.ConfigureForAuthoring(playerMovement, combatRegistry, transfer, combat);
            combat.ConfigureHostileProducerForAuthoring(hostile);
            bridge.ConfigureForAuthoring(playerMovement, transfer, combat);
            scheduler.ConfigureForAuthoring(bridge, transfer, weights, sinks, audit.Target, audit.Sink, audit.Target.TargetCollider as BoxCollider2D);
            hostile.ConfigureForAuthoring(playerMovement, transfer, combat, bridge, scheduler);
            auditPull.ConfigureForAuthoring(playerMovement, transfer, combat, bridge, scheduler);
            handoff.ConfigureForAuthoring(playerMovement, transfer, combat, bridge, scheduler, ordan, audit.Target, audit.Sink, weights, sinks, colliders);
            handoff.ConfigureHostileProducerForAuthoring(hostile);
            handoff.ConfigureBalanceAuditPullProducerForAuthoring(auditPull);
            teardown.ConfigureForAuthoring(transfer, combat, bridge, scheduler, hostile, auditPull, handoff, ordan,
                ordan.Freeze().TargetCollider, new[] { audit.Target.TargetCollider, colliders[0], colliders[1], colliders[2] });
            var cameraDriver = CreateCamera(root.transform, playerMovement);
            router.ConfigureForAuthoring(playerMovement, transfer, combat, cameraDriver, InputMode.GameplayEnabled);
            router.ConfigureTerminalTeardownForAuthoring(teardown);
            requester.ConfigureForAuthoring(router, playerMovement, combat, handoff);
            CreateEnvironment(root.transform);
            root.SetActive(true);
            return root;
        }

        // REQ-M5D1-001/005. This is a dedicated fixed-room camera child; it
        // never moves an encounter actor or the Systems transform.
        private static SimulationCameraDriver CreateCamera(Transform parent, PlayerMovementController player)
        {
            var value = new GameObject("Camera"); value.transform.SetParent(parent, false);
            value.transform.localPosition = new Vector3(0f, 4.5f, -10f);
            var camera = value.AddComponent<UnityEngine.Camera>();
            var pixelPerfect = value.AddComponent<PixelPerfectCamera>();
            var driver = value.AddComponent<SimulationCameraDriver>();
            // The profile seam requires active/enabled components and is still
            // Editor-only; root is enabled only after all graph references exist.
            parent.gameObject.SetActive(true);
            SimulationCameraProfileAuthoring.ApplyApprovedProfile(camera, pixelPerfect);
            driver.ConfigureForAuthoring(player, camera, pixelPerfect, 0, 81, new CameraCenterBounds(0, 0, 81, 81));
            return driver;
        }

        private static CombatTarget CreateOrdan(Transform parent)
        {
            var value = new GameObject("Ordan"); value.transform.SetParent(parent, false); value.transform.localPosition = new Vector3(7f, 1.2f, 0f);
            var body = value.AddComponent<Rigidbody2D>(); body.bodyType = RigidbodyType2D.Kinematic; body.gravityScale = 0f; body.constraints = RigidbodyConstraints2D.FreezeRotation; body.interpolation = RigidbodyInterpolation2D.None;
            var collider = value.AddComponent<BoxCollider2D>(); collider.size = new Vector2(1.6f, 2.4f); collider.offset = Vector2.zero; collider.isTrigger = false;
            var target = value.AddComponent<CombatTarget>(); target.ConfigureForAuthoring("ordan", 60, 0, true, 0, 0, 0, 0, 800, 1200, collider, value.transform);
            return target;
        }

        private static (TransferTarget Target, OrdanBossAuditTransferModifierSink Sink) CreateAudit(Transform parent)
        {
            var value = new GameObject("BossAuditBox-0"); value.transform.SetParent(parent, false); value.transform.localPosition = new Vector3(0f, .75f, 0f);
            var body = value.AddComponent<Rigidbody2D>(); body.bodyType = RigidbodyType2D.Kinematic; body.gravityScale = 0f; body.constraints = RigidbodyConstraints2D.FreezeRotation; body.interpolation = RigidbodyInterpolation2D.None;
            var collider = value.AddComponent<BoxCollider2D>(); collider.size = new Vector2(1.2f, 1.2f); collider.offset = Vector2.zero; collider.isTrigger = false; collider.enabled = false;
            var sink = value.AddComponent<OrdanBossAuditTransferModifierSink>(); sink.ConfigureForAuthoring(OrdanBossAuditTransferModifierSink.RequiredTargetId, OrdanBossAuditTransferModifierSink.RequiredBaseProfileId);
            var target = value.AddComponent<TransferTarget>(); target.ConfigureForAuthoring(OrdanBossAuditTransferModifierSink.RequiredTargetId, TransferTargetKind.Box, 0, 0, 0, 0, 600, 600,
                OrdanBossAuditTransferModifierSink.RequiredBaseProfileId, collider, value.transform, sink);
            return (target, sink);
        }

        private static void CreatePayload(Transform parent, int index, out TransferTarget target, out OrdanBossPayloadTransferModifierSink sink, out Collider2D collider)
        {
            var value = new GameObject("BossWeight-" + index); value.transform.SetParent(parent, false); value.transform.localPosition = new Vector3(3f + index * 2f, 6f, 0f);
            var body = value.AddComponent<Rigidbody2D>(); body.bodyType = RigidbodyType2D.Kinematic; body.gravityScale = 0f; body.constraints = RigidbodyConstraints2D.FreezeRotation; body.interpolation = RigidbodyInterpolation2D.None;
            var box = value.AddComponent<BoxCollider2D>(); box.size = Vector2.one; box.offset = Vector2.zero; box.isTrigger = false; box.enabled = false;
            sink = value.AddComponent<OrdanBossPayloadTransferModifierSink>(); sink.ConfigureForAuthoring("BossWeight-" + index);
            target = value.AddComponent<TransferTarget>(); target.ConfigureForAuthoring("BossWeight-" + index, TransferTargetKind.BossPayload, 0, 0, 0, 0, 500, 500,
                OrdanBossPayloadTransferModifierSink.RequiredBaseProfileId, box, value.transform, sink);
            collider = box;
        }

        private static void CreateEnvironment(Transform parent)
        {
            var environment = new GameObject("Environment"); environment.transform.SetParent(parent, false);
            CreateEnvironmentBox(environment.transform, "Ground", new Vector2(0f, -.5f), new Vector2(24f, 1f));
            CreateEnvironmentBox(environment.transform, "LeftBound", new Vector2(-12f, 4.5f), new Vector2(1f, 11f));
            CreateEnvironmentBox(environment.transform, "RightBound", new Vector2(12f, 4.5f), new Vector2(1f, 11f));
        }

        private static void CreateEnvironmentBox(Transform parent, string name, Vector2 position, Vector2 size)
        { var value = new GameObject(name) { layer = 8 }; value.transform.SetParent(parent, false); value.transform.localPosition = position; var collider = value.AddComponent<BoxCollider2D>(); collider.size = size; collider.offset = Vector2.zero; collider.isTrigger = false; }
        private static void EnsureFolder(string path)
        { if (AssetDatabase.IsValidFolder(path)) return; var parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/'); var name = System.IO.Path.GetFileName(path); if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name)) throw new InvalidOperationException(); AssetDatabase.CreateFolder(parent, name); }
    }
}
