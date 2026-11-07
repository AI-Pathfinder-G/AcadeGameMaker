// AC-MOV-001, AC-MOV-005. M2 authoring validation for the approved scene and prefab only.
using System;
using System.IO;
using System.Security.Cryptography;
using AcadeGameMaker.Movement.Authoring;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.Movement
{
    internal sealed class MovementSandboxAuthoringTests
    {
        private const string ProductionScenePath = "Assets/Scenes/MovementSandbox.unity";
        private const string ProductionPrefabPath = "Assets/Prefabs/Player/MovementSandboxPlayer.prefab";

        // REQ-MOV-001, REQ-MOV-002, REQ-MOV-005; AC-MOV-001, AC-MOV-002,
        // AC-MOV-005. Automated builder evidence is isolated to disposable assets;
        // neither approved production artifact may change even transiently.
        [Test]
        public void TestBuilderCreatesAndCleansTemporaryAssetsWithoutChangingProductionHashes()
        {
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(ProductionScenePath), Is.Not.Null);
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>(ProductionPrefabPath), Is.Not.Null);
            var sceneHash = Sha256(ProductionScenePath);
            var prefabHash = Sha256(ProductionPrefabPath);
            CleanupTemporaryAssets();
            try
            {
                MovementSandboxAuthoringBuilder.BuildForTests(
                    MovementSandboxAuthoringBuilder.TestScenePath,
                    MovementSandboxAuthoringBuilder.TestPrefabPath);
                Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(MovementSandboxAuthoringBuilder.TestScenePath), Is.Not.Null);
                Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>(MovementSandboxAuthoringBuilder.TestPrefabPath), Is.Not.Null);
                Assert.DoesNotThrow(() => MovementSandboxAuthoringBuilder.ValidateAuthoredCollisionIds());
                Assert.That(Sha256(ProductionScenePath), Is.EqualTo(sceneHash));
                Assert.That(Sha256(ProductionPrefabPath), Is.EqualTo(prefabHash));
            }
            finally
            {
                CleanupTemporaryAssets();
                Assert.That(AssetDatabase.IsValidFolder(MovementSandboxAuthoringBuilder.TestAssetRoot), Is.False);
                Assert.That(Sha256(ProductionScenePath), Is.EqualTo(sceneHash));
                Assert.That(Sha256(ProductionPrefabPath), Is.EqualTo(prefabHash));
            }
        }

        // AC-MOV-001, AC-MOV-002, AC-MOV-005. The test seam is fail-closed
        // against either approved production destination and path traversal.
        [Test]
        public void TestBuilderRejectsProductionAndEscapingAssetPaths()
        {
            Assert.Throws<ArgumentException>(() => MovementSandboxAuthoringBuilder.BuildForTests(
                ProductionScenePath, MovementSandboxAuthoringBuilder.TestPrefabPath));
            Assert.Throws<ArgumentException>(() => MovementSandboxAuthoringBuilder.BuildForTests(
                MovementSandboxAuthoringBuilder.TestScenePath, ProductionPrefabPath));
            Assert.Throws<ArgumentException>(() => MovementSandboxAuthoringBuilder.BuildForTests(
                MovementSandboxAuthoringBuilder.TestAssetRoot + "/../MovementSandbox.unity",
                MovementSandboxAuthoringBuilder.TestPrefabPath));
        }

        [Test]
        public void ValidationRejectsDuplicateAndEmptyStableIdCandidates()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var duplicate = new GameObject("DuplicateWall");
            duplicate.AddComponent<BoxCollider2D>();
            duplicate.AddComponent<BoxCollider2D>();
            Assert.Throws<InvalidOperationException>(() => MovementSandboxAuthoringBuilder.ValidateAuthoredCollisionIds());
            UnityEngine.Object.DestroyImmediate(duplicate);

            var empty = new GameObject(string.Empty);
            empty.AddComponent<BoxCollider2D>();
            Assert.Throws<InvalidOperationException>(() => MovementSandboxAuthoringBuilder.ValidateAuthoredCollisionIds());
            UnityEngine.Object.DestroyImmediate(empty);
        }

        private static string Sha256(string assetPath)
        {
            using (var hash = SHA256.Create())
            {
                return BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(assetPath))).Replace("-", string.Empty);
            }
        }

        private static void CleanupTemporaryAssets()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            if (AssetDatabase.IsValidFolder(MovementSandboxAuthoringBuilder.TestAssetRoot))
                Assert.That(AssetDatabase.DeleteAsset(MovementSandboxAuthoringBuilder.TestAssetRoot), Is.True);
        }
    }
}
