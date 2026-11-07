// AC-WT-003, AC-WT-006. M2B1 validates explicit target authoring and ordinal registry authority only.
using System;
using System.Collections.Generic;
using System.IO;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.TransferUnity
{
    internal sealed class TransferTargetAuthoringTests
    {
        private const string TemporaryScenePath = "Assets/AcadeGameMaker/Tests/EditMode/TransferUnity/__TempTransferTargetAuthoringScene.unity";
        private readonly List<GameObject> _objects = new List<GameObject>();

        [SetUp]
        public void CreateNamedAuthoringScene()
        {
            AssetDatabase.DeleteAsset(TemporaryScenePath);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Assert.That(EditorSceneManager.SaveScene(scene, TemporaryScenePath), Is.True);
        }

        [TearDown]
        public void DestroyObjects()
        {
            for (var index = _objects.Count - 1; index >= 0; index--)
            {
                UnityEngine.Object.DestroyImmediate(_objects[index]);
            }

            _objects.Clear();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AssetDatabase.DeleteAsset(TemporaryScenePath);
            AssetDatabase.Refresh();
        }

        // AC-WT-006. User-layer slot 8 is the sole M2B1 ProjectSettings change and has its frozen exact name.
        [Test]
        public void TransferLineOfSightIsExactlyUserLayerEight()
        {
            var lines = File.ReadAllLines(Path.Combine(ProjectRoot, "ProjectSettings", "TagManager.asset"));
            var layersIndex = Array.FindIndex(lines, line => line.Trim() == "layers:");
            Assert.That(layersIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(lines[layersIndex + 9].Trim(), Is.EqualTo("- TransferLineOfSight"));
            Assert.That(TransferLineOfSight.Mask, Is.EqualTo(256));
        }

        // AC-WT-003. The serialized sink is a MonoBehaviour reference but must implement the engine-free exact sink interface.
        [Test]
        public void TargetRejectsMissingOrMismatchedSinkAndInvalidColliderAuthoring()
        {
            var target = CreateTarget("box");
            target.ConfigureForTests("box", TransferTargetKind.Box, 0, 0, 0, 0, 1, 1, "base", target.TargetCollider, target.TargetPoseRoot, target.gameObject.AddComponent<PlainBehaviour>());
            Assert.Throws<InvalidOperationException>(() => target.ValidateForAuthoring());

            var mismatch = CreateTarget("mismatch");
            mismatch.ConfigureForTests("other", TransferTargetKind.Box, 0, 0, 0, 0, 1, 1, "base", mismatch.TargetCollider, mismatch.TargetPoseRoot, mismatch.gameObject.GetComponent<TestSinkBehaviour>());
            Assert.Throws<InvalidOperationException>(() => mismatch.ValidateForAuthoring());

            var layered = CreateTarget("layered");
            layered.TargetCollider.gameObject.layer = TransferLineOfSight.Layer;
            Assert.Throws<InvalidOperationException>(() => layered.ValidateForAuthoring());
        }

        // AC-WT-003. Extents, pose transform and descendants are all validated before a descriptor is emitted.
        [Test]
        public void TargetRejectsInvalidGeometryTransformAndLineOfSightDescendant()
        {
            var target = CreateTarget("box");
            target.ConfigureForTests("box", TransferTargetKind.Box, 0, 0, 0, 0, 0, 1, "base", target.TargetCollider, target.TargetPoseRoot, target.gameObject.GetComponent<TestSinkBehaviour>());
            Assert.Throws<ArgumentException>(() => target.ValidateForAuthoring());

            target = CreateTarget("rotated");
            target.TargetPoseRoot.localEulerAngles = new Vector3(0f, 0f, 90f);
            Assert.Throws<InvalidOperationException>(() => target.ValidateForAuthoring());

            target = CreateTarget("child-layer");
            var child = Track(new GameObject("ChildCollider"));
            child.transform.SetParent(target.TargetPoseRoot);
            child.layer = TransferLineOfSight.Layer;
            child.AddComponent<BoxCollider2D>();
            Assert.Throws<InvalidOperationException>(() => target.ValidateForAuthoring());
        }

        // AC-WT-006. The serialized registry is explicit and must already be ordinal by TargetId; it never sorts discovery output.
        [Test]
        public void RegistryRejectsUnsortedDuplicateAndMissingTargets()
        {
            var registry = Track(new GameObject("Registry")).AddComponent<TransferTargetRegistry>();
            var alpha = CreateTarget("alpha");
            var bravo = CreateTarget("bravo");
            registry.ConfigureForTests(bravo, alpha);
            Assert.Throws<InvalidOperationException>(() => registry.ValidateForAuthoring());

            registry.ConfigureForTests(alpha, alpha);
            Assert.Throws<InvalidOperationException>(() => registry.ValidateForAuthoring());

            registry.ConfigureForTests(alpha, bravo);
            Assert.DoesNotThrow(() => registry.ValidateForAuthoring());

            var duplicateCollider = CreateTarget("charlie");
            duplicateCollider.ConfigureForTests("charlie", TransferTargetKind.Box, 0, 0, 0, 0, 1, 1, "base", alpha.TargetCollider, duplicateCollider.TargetPoseRoot, duplicateCollider.gameObject.GetComponent<TestSinkBehaviour>());
            registry.ConfigureForTests(alpha, duplicateCollider);
            var duplicateKey = Assert.Throws<InvalidOperationException>(() => registry.ValidateForAuthoring());
            Assert.That(duplicateKey.Message, Does.Contain("stable collider key"));

            registry.ConfigureForTests(alpha, null);
            Assert.Throws<InvalidOperationException>(() => registry.ValidateForAuthoring());
        }

        // AC-WT-006. Diagnostics compare explicit hierarchy/type/ordinal tuple fields, not a delimiter-concatenated key.
        [Test]
        public void DiagnosticsUseTupleOrderAndNumericComponentOrdinal()
        {
            var earlierSceneDiagnostic = new TransferLineOfSightDiagnostic(500000,
                new ColliderIdentity("AlphaIdentityScene", new[] { "ZetaPath" }, typeof(BoxCollider2D).FullName, 0));
            var laterSceneDiagnostic = new TransferLineOfSightDiagnostic(500000,
                new ColliderIdentity("TransferTargetAuthoringTests", new[] { "AlphaPath" }, typeof(CircleCollider2D).FullName, 0));
            Assert.That(TransferLineOfSightDiagnostic.Compare(earlierSceneDiagnostic, laterSceneDiagnostic), Is.LessThan(0));

            var alpha = Track(new GameObject("Alpha"));
            var beta = Track(new GameObject("Beta"));
            var alphaDiagnostic = new TransferLineOfSightDiagnostic(500000, ColliderIdentity.Create(alpha.AddComponent<BoxCollider2D>()));
            var betaDiagnostic = new TransferLineOfSightDiagnostic(500000, ColliderIdentity.Create(beta.AddComponent<CircleCollider2D>()));
            Assert.That(TransferLineOfSightDiagnostic.Compare(alphaDiagnostic, betaDiagnostic), Is.LessThan(0));

            var typeOrder = Track(new GameObject("TypeOrder"));
            var circle = typeOrder.AddComponent<CircleCollider2D>();
            var box = typeOrder.AddComponent<BoxCollider2D>();
            var circleDiagnostic = new TransferLineOfSightDiagnostic(500000, ColliderIdentity.Create(circle));
            var boxDiagnostic = new TransferLineOfSightDiagnostic(500000, ColliderIdentity.Create(box));
            Assert.That(TransferLineOfSightDiagnostic.Compare(boxDiagnostic, circleDiagnostic), Is.LessThan(0));

            var ordinalOrder = Track(new GameObject("OrdinalOrder"));
            var colliders = new List<BoxCollider2D>();
            for (var index = 0; index <= 10; index++)
            {
                colliders.Add(ordinalOrder.AddComponent<BoxCollider2D>());
            }

            var ordinalTwo = new TransferLineOfSightDiagnostic(500000, ColliderIdentity.Create(colliders[2]));
            var ordinalTen = new TransferLineOfSightDiagnostic(500000, ColliderIdentity.Create(colliders[10]));
            Assert.That(TransferLineOfSightDiagnostic.Compare(ordinalTwo, ordinalTen), Is.LessThan(0));
        }

        // AC-WT-006. Delimiter-bearing authored names produce distinct length-prefixed canonical identities.
        [Test]
        public void DelimiterBearingHierarchyNamesCannotCollide()
        {
            var firstRoot = Track(new GameObject("A:B/C"));
            var firstChild = Track(new GameObject("D"));
            firstChild.transform.SetParent(firstRoot.transform);
            var secondRoot = Track(new GameObject("A:B"));
            var secondChild = Track(new GameObject("C/D"));
            secondChild.transform.SetParent(secondRoot.transform);

            var firstKey = TransferLineOfSight.StableColliderKey(firstChild.AddComponent<BoxCollider2D>());
            var secondKey = TransferLineOfSight.StableColliderKey(secondChild.AddComponent<BoxCollider2D>());
            Assert.That(firstKey, Is.Not.EqualTo(secondKey));
            Assert.That(TransferLineOfSight.HierarchyPath(firstChild.transform), Is.Not.EqualTo(TransferLineOfSight.HierarchyPath(secondChild.transform)));
        }

        private TransferTarget CreateTarget(string targetId)
        {
            var gameObject = Track(new GameObject(targetId));
            var collider = gameObject.AddComponent<BoxCollider2D>();
            var sink = gameObject.AddComponent<TestSinkBehaviour>();
            sink.BoundId = targetId;
            var target = gameObject.AddComponent<TransferTarget>();
            target.ConfigureForTests(targetId, TransferTargetKind.Box, 0, 0, 0, 0, 1, 1, "base", collider, gameObject.transform, sink);
            return target;
        }

        private GameObject Track(GameObject gameObject)
        {
            _objects.Add(gameObject);
            return gameObject;
        }

        private static string ProjectRoot => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

        public sealed class TestSinkBehaviour : MonoBehaviour, ITransferTargetModifierSink
        {
            public string BoundId;
            public string BoundTargetId => BoundId;
            public bool IsStillAvailable() => true;
            public bool TryApply(TransferModifierRequest request) => true;
            public void Clear(TransferClearReason reason, SimulationTick tick, int revision) { }
        }

        public sealed class PlainBehaviour : MonoBehaviour { }
    }
}
