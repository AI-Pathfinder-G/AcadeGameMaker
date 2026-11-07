using System;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Combat.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Combat.Authoring
{
    // REQ-COM-001. B0 and B1 are caller-selected stages; asset content never infers a stage.
    internal enum RegularEnemyEncounterAuthoringStage { B0 = 0, B1 = 1 }

    internal static class RegularEnemyEncounterAuthoringBuilder
    {
        internal const string ScenePath = "Assets/Scenes/CombatEncounterSandbox.unity";
        internal const string GraphPrefabPath = "Assets/Prefabs/Combat/CombatEncounterGraph.prefab";
        internal const string PlayerPrefabPath = "Assets/Prefabs/Player/CombatEncounterPlayer.prefab";
        internal const string WalkerPrefabPath = "Assets/Prefabs/Enemies/Walker.prefab";
        internal const string SurveyorPrefabPath = "Assets/Prefabs/Enemies/Surveyor.prefab";

        [MenuItem("AcadeGameMaker/Build Combat Encounter B0")]
        private static void BuildFromMenu() => Build(RegularEnemyEncounterAuthoringStage.B0);

        [MenuItem("AcadeGameMaker/Build Combat Encounter B1")]
        private static void BuildB1FromMenu() => Build(RegularEnemyEncounterAuthoringStage.B1);

        // AC-COM-001, AC-COM-003; AC-WT-003, AC-WT-005 (partial B0 evidence).
        internal static void Build(RegularEnemyEncounterAuthoringStage stage)
        {
            RequireKnownStage(stage);
            EnsureFolder("Assets/Prefabs");
            EnsureFolder("Assets/Prefabs/Combat");
            EnsureFolder("Assets/Prefabs/Enemies");
            EnsureFolder("Assets/Prefabs/Player");

            // The allowlisted final path is saved before any graph validation so
            // existing Combat/Transfer authoring invariants observe a stable name.
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new InvalidOperationException("Could not establish the named Combat Encounter scene before authoring.");
            var playerPrefab = SavePlayerPrefab();
            var walkerPrefab = SaveEnemyPrefab("Walker", "walker", WalkerPrefabPath);
            var surveyorPrefab = SaveEnemyPrefab("Surveyor", "surveyor", SurveyorPrefabPath);
            var graph = BuildGraph(playerPrefab, walkerPrefab, surveyorPrefab, stage);
            RegularEnemyEncounterAuthoringValidator.Validate(graph, stage, false);
            var graphPrefab = PrefabUtility.SaveAsPrefabAsset(graph, GraphPrefabPath);
            UnityEngine.Object.DestroyImmediate(graph);

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(graphPrefab, scene);
            RegularEnemyEncounterAuthoringValidator.Validate(instance, stage, false);
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new InvalidOperationException("Could not save the final Combat Encounter scene graph.");
            AssetDatabase.SaveAssets();
        }

        private static GameObject SavePlayerPrefab()
        {
            var root = new GameObject("CombatEncounterPlayer");
            root.SetActive(false);
            root.layer = 0;
            var body = root.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 3.1315f;
            body.useFullKinematicContacts = false;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.interpolation = RigidbodyInterpolation2D.None;
            var capsule = root.AddComponent<CapsuleCollider2D>();
            capsule.direction = CapsuleDirection2D.Vertical;
            capsule.size = new Vector2(0.8f, 1.6f);
            capsule.offset = Vector2.zero;
            capsule.isTrigger = false;
            var movement = root.AddComponent<PlayerMovementController>();
            var combat = root.AddComponent<CombatTarget>();
            combat.ConfigureForAuthoring("player", 5, 45, false, 0, 0, 0, 0, 400, 800, capsule, root.transform);
            root.SetActive(true);
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject SaveEnemyPrefab(string name, string role, string path)
        {
            var root = new GameObject(name);
            root.SetActive(false);
            root.layer = 0;
            var body = root.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.useFullKinematicContacts = false;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.interpolation = RigidbodyInterpolation2D.None;
            var box = root.AddComponent<BoxCollider2D>();
            box.size = new Vector2(0.8f, 1.6f);
            box.offset = Vector2.zero;
            box.edgeRadius = 0f;
            box.isTrigger = false;
            var sink = root.AddComponent<RegularEnemyTransferModifierSink>();
            sink.ConfigureForAuthoring(role, RegularEnemyTransferModifierSink.RequiredBaseProfileId);
            var transfer = root.AddComponent<TransferTarget>();
            transfer.ConfigureForAuthoring(role, TransferTargetKind.Enemy, 0, 0, 0, 0, 400, 800,
                RegularEnemyTransferModifierSink.RequiredBaseProfileId, box, root.transform, sink);
            var combat = root.AddComponent<CombatTarget>();
            combat.ConfigureForAuthoring(role, role == "walker" ? 9 : 6, 0, true, 0, 0, 0, 0, 400, 800,
                box, root.transform, transfer);
            root.SetActive(true);
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject BuildGraph(GameObject playerPrefab, GameObject walkerPrefab, GameObject surveyorPrefab,
            RegularEnemyEncounterAuthoringStage stage)
        {
            var root = new GameObject("CombatEncounterGraph");
            root.SetActive(false);
            var playerRoot = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, root.scene);
            var walkerRoot = (GameObject)PrefabUtility.InstantiatePrefab(walkerPrefab, root.scene);
            var surveyorRoot = (GameObject)PrefabUtility.InstantiatePrefab(surveyorPrefab, root.scene);
            playerRoot.transform.SetParent(root.transform, false);
            walkerRoot.transform.SetParent(root.transform, false);
            surveyorRoot.transform.SetParent(root.transform, false);
            playerRoot.transform.localPosition = new Vector3(0f, 0.81f, 0f);
            surveyorRoot.transform.localPosition = new Vector3(6f, 0.81f, 0f);
            walkerRoot.transform.localPosition = new Vector3(3f, 0.81f, 0f);

            var systems = new GameObject("Systems");
            systems.transform.SetParent(root.transform, false);
            var transferRegistry = systems.AddComponent<TransferTargetRegistry>();
            var combatRegistry = systems.AddComponent<CombatTargetRegistry>();
            var transferDriver = systems.AddComponent<TransferSimulationDriver>();
            var combatDriver = systems.AddComponent<CombatSimulationDriver>();
            var reaction = systems.AddComponent<RegularEnemyReactionSimulationDriver>();
            var behavior = systems.AddComponent<RegularEnemyBehaviorSimulationDriver>();
            var locomotion = systems.AddComponent<RegularEnemyLocomotionSimulationDriver>();
            var threat = systems.AddComponent<RegularEnemyThreatSimulationDriver>();
            RegularEnemyEncounterHandoffAdapter handoff = null;
            if (stage == RegularEnemyEncounterAuthoringStage.B1)
                handoff = systems.AddComponent<RegularEnemyEncounterHandoffAdapter>();

            var player = playerRoot.GetComponent<PlayerMovementController>();
            var playerCombat = playerRoot.GetComponent<CombatTarget>();
            var surveyorTransfer = surveyorRoot.GetComponent<TransferTarget>();
            var walkerTransfer = walkerRoot.GetComponent<TransferTarget>();
            var surveyorCombat = surveyorRoot.GetComponent<CombatTarget>();
            var walkerCombat = walkerRoot.GetComponent<CombatTarget>();
            transferRegistry.ConfigureForAuthoring(surveyorTransfer, walkerTransfer);
            combatRegistry.ConfigureForAuthoring(playerCombat, surveyorCombat, walkerCombat);
            transferDriver.ConfigureForAuthoring(player, transferRegistry);
            combatDriver.ConfigureForAuthoring(player, combatRegistry, transferDriver, reaction);
            reaction.ConfigureForAuthoring(player, transferDriver, combatDriver);
            behavior.ConfigureForAuthoring(player, combatDriver, reaction);
            locomotion.ConfigureForAuthoring(player, combatDriver, reaction, behavior);
            threat.ConfigureForAuthoring(player, combatDriver, behavior, locomotion);
            if (handoff != null)
                handoff.ConfigureForAuthoring(player, transferDriver, combatDriver, reaction, behavior, locomotion, threat,
                    playerRoot.GetComponent<CapsuleCollider2D>(), surveyorTransfer, walkerTransfer,
                    surveyorRoot.GetComponent<BoxCollider2D>(), walkerRoot.GetComponent<BoxCollider2D>());

            var environment = new GameObject("Environment");
            environment.transform.SetParent(root.transform, false);
            CreateEnvironmentBox(environment.transform, "Ground", new Vector2(0f, -0.5f), new Vector2(20f, 1f));
            CreateEnvironmentBox(environment.transform, "LeftBound", new Vector2(-10f, 4f), new Vector2(1f, 10f));
            CreateEnvironmentBox(environment.transform, "RightBound", new Vector2(10f, 4f), new Vector2(1f, 10f));
            root.SetActive(true);
            return root;
        }

        private static void CreateEnvironmentBox(Transform parent, string name, Vector2 position, Vector2 size)
        {
            var value = new GameObject(name) { layer = 8 };
            value.transform.SetParent(parent, false);
            value.transform.localPosition = position;
            value.AddComponent<BoxCollider2D>().size = size;
        }

        private static void RequireKnownStage(RegularEnemyEncounterAuthoringStage stage)
        {
            if (stage != RegularEnemyEncounterAuthoringStage.B0 && stage != RegularEnemyEncounterAuthoringStage.B1)
                throw new ArgumentOutOfRangeException(nameof(stage));
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            var name = System.IO.Path.GetFileName(path);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name)) throw new InvalidOperationException();
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
