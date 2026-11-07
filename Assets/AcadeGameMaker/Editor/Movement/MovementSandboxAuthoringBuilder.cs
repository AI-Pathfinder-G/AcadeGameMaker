// REQ-MOV-001, REQ-MOV-002, REQ-MOV-005. Generates only the approved sandbox scene and player prefab.
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AcadeGameMaker.Movement.Authoring
{
    internal static class MovementSandboxAuthoringBuilder
    {
        private const string ScenePath = "Assets/Scenes/MovementSandbox.unity";
        private const string PrefabPath = "Assets/Prefabs/Player/MovementSandboxPlayer.prefab";
        internal const string TestAssetRoot = "Assets/AcadeGameMaker/Tests/MovementSandboxAuthoringTemp";
        internal const string TestScenePath = TestAssetRoot + "/MovementSandbox.unity";
        internal const string TestPrefabPath = TestAssetRoot + "/MovementSandboxPlayer.prefab";

        [MenuItem("AcadeGameMaker/Build Movement Sandbox")]
        private static void BuildFromMenu()
        {
            Build();
        }

        // AC-MOV-001, AC-MOV-002, AC-MOV-005. Sol may invoke this after importing the source; it has no runtime role.
        internal static void Build()
        {
            BuildAtPaths(ScenePath, PrefabPath, true);
        }

        // REQ-MOV-001, REQ-MOV-002, REQ-MOV-005; AC-MOV-001, AC-MOV-002,
        // AC-MOV-005. Automated verification may author only beneath the exact
        // test-temporary root and never calls the production-path entry point.
        internal static void BuildForTests(string scenePath, string prefabPath)
        {
            RequireTestAssetPath(scenePath, ".unity");
            RequireTestAssetPath(prefabPath, ".prefab");
            BuildAtPaths(scenePath, prefabPath, false);
        }

        private static void BuildAtPaths(string scenePath, string prefabPath, bool saveAllAssets)
        {
            EnsureFolder(Path.GetDirectoryName(prefabPath)?.Replace('\\', '/'));
            EnsureFolder(Path.GetDirectoryName(scenePath)?.Replace('\\', '/'));
            var prefab = BuildPlayerPrefab(prefabPath);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SceneManager.SetActiveScene(scene);
            PrefabUtility.InstantiatePrefab(prefab);
            CreateBox("Ground", new Vector2(0f, -1f), new Vector2(24f, 1f));
            CreateBox("Ceiling", new Vector2(0f, 7f), new Vector2(24f, 1f));
            CreateBox("LeftWall", new Vector2(-6f, 3f), new Vector2(1f, 8f));
            CreateBox("RightWall", new Vector2(6f, 3f), new Vector2(1f, 8f));
            EditorSceneManager.SaveScene(scene, scenePath);
            ValidateAuthoredCollisionIds();
            EditorSceneManager.SaveScene(scene, scenePath);
            if (saveAllAssets) AssetDatabase.SaveAssets();
        }

        // AC-MOV-005. Empty or duplicate stable authored IDs are invalid sandbox authoring.
        internal static void ValidateAuthoredCollisionIds()
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var collider in UnityEngine.Object.FindObjectsByType<Collider2D>(FindObjectsSortMode.None))
            {
                if (collider.GetComponent<PlayerMovementController>() != null)
                {
                    continue;
                }

                var id = PlayerMovementController.GetStableAuthoredId(collider);
                if (!seen.Add(id))
                {
                    throw new InvalidOperationException($"Duplicate MovementSandbox stable collision ID: {id}.");
                }
            }
        }

        private static GameObject BuildPlayerPrefab(string prefabPath)
        {
            var player = new GameObject("MovementSandboxPlayer");
            player.transform.position = new Vector3(0f, 1f, 0f);
            var body = player.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 3.1315f;
            body.useFullKinematicContacts = false;
            var collider = player.AddComponent<CapsuleCollider2D>();
            collider.isTrigger = false;
            collider.size = new Vector2(0.8f, 1.6f);
            player.AddComponent<PlayerMovementController>();
            var prefab = PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
            UnityEngine.Object.DestroyImmediate(player);
            return prefab;
        }

        private static void CreateBox(string name, Vector2 position, Vector2 size)
        {
            var box = new GameObject(name);
            box.transform.position = position;
            box.AddComponent<BoxCollider2D>().size = size;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            var name = System.IO.Path.GetFileName(path);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException($"Invalid Unity asset folder: {path}.");
            }

            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static void RequireTestAssetPath(string path, string extension)
        {
            if (string.IsNullOrEmpty(path) || path.IndexOf('\\') >= 0 || path.IndexOf("..", StringComparison.Ordinal) >= 0 ||
                !path.StartsWith(TestAssetRoot + "/", StringComparison.Ordinal) ||
                !path.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(path, ScenePath, StringComparison.Ordinal) || string.Equals(path, PrefabPath, StringComparison.Ordinal))
                throw new ArgumentException("Test authoring paths must remain beneath the exact temporary asset root.", nameof(path));
        }
    }
}
