// AC-WT-003, AC-WT-006. M2B1 production LOS filter and deterministic diagnostic ordering evidence.
using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.TransferUnity
{
    internal sealed class TransferLineOfSightTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private SimulationMode2D _previousSimulationMode;
        private Transform _playerRoot;
        private Transform _targetRoot;

        [SetUp]
        public void UseScriptPhysics()
        {
            _previousSimulationMode = Physics2D.simulationMode;
            Physics2D.simulationMode = SimulationMode2D.Script;
            _playerRoot = Track(new GameObject("PlayerRoot")).transform;
            _targetRoot = Track(new GameObject("TargetRoot")).transform;
            _targetRoot.position = new Vector2(4f, 0f);
        }

        [TearDown]
        public void RestorePhysics()
        {
            for (var index = _objects.Count - 1; index >= 0; index--)
            {
                UnityEngine.Object.DestroyImmediate(_objects[index]);
            }

            _objects.Clear();
            Physics2D.simulationMode = _previousSimulationMode;
            Physics2D.SyncTransforms();
        }

        // AC-WT-003, AC-WT-006. Trigger queries are explicitly off while an ordinary layer-8 blocker closes LOS.
        [Test]
        public void TriggerIsIgnoredButNonTriggerLayerEightBlockerClosesLineOfSight()
        {
            CreateBox("Trigger", new Vector2(2f, 0f), true);
            Assert.That(Evaluate().IsOpen, Is.True);

            CreateBox("Blocker", new Vector2(2f, 0f), false);
            Assert.That(Evaluate().IsOpen, Is.False);
        }

        // AC-WT-006. Player/target roots and deep children are ignored even when incorrectly placed on layer 8.
        [Test]
        public void PlayerAndTargetRootsAndChildrenAreIgnored()
        {
            CreateRootBox(_playerRoot);
            CreateRootBox(_targetRoot);
            CreateChildBox(_playerRoot, "PlayerChild", new Vector2(1f, 0f));
            CreateChildBox(_targetRoot, "TargetChild", new Vector2(-1f, 0f));
            Assert.That(Evaluate().IsOpen, Is.True);

            CreateBox("Unrelated", new Vector2(2f, 0f), false);
            Assert.That(Evaluate().IsOpen, Is.False);
        }

        // AC-WT-006. A CompositeCollider2D is a normal layer-8 LOS blocker.
        [Test]
        public void CompositeColliderBlocksLineOfSight()
        {
            var compositeObject = Track(new GameObject("Composite"));
            compositeObject.layer = TransferLineOfSight.Layer;
            compositeObject.transform.position = new Vector2(2f, 0f);
            var body = compositeObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;
            compositeObject.AddComponent<CompositeCollider2D>();
            var source = compositeObject.AddComponent<PolygonCollider2D>();
            source.points = new[]
            {
                new Vector2(-0.25f, -0.25f), new Vector2(0.25f, -0.25f),
                new Vector2(0.25f, 0.25f), new Vector2(-0.25f, 0.25f)
            };
            source.compositeOperation = Collider2D.CompositeOperation.Merge;
            Physics2D.SyncTransforms();

            var result = Evaluate();
            Assert.That(result.IsOpen, Is.False);
            Assert.That(result.Diagnostics[0].StableColliderKey, Does.Contain(typeof(CompositeCollider2D).FullName));
        }

        // AC-WT-006. Equal physics fractions are sorted by authored stable key rather than query order or instance ID.
        [Test]
        public void EqualFractionDiagnosticsUseStableOrdinalOrder()
        {
            var beta = CreateBox("Beta", new Vector2(2f, 0f), false);
            var alpha = CreateBox("Alpha", new Vector2(2f, 0f), false);
            var result = Evaluate();
            Assert.That(result.IsOpen, Is.False);
            Assert.That(result.Diagnostics.Count, Is.EqualTo(2));
            Assert.That(result.Diagnostics[0].StableColliderKey, Is.EqualTo(TransferLineOfSight.StableColliderKey(alpha)));
            Assert.That(result.Diagnostics[1].StableColliderKey, Is.EqualTo(TransferLineOfSight.StableColliderKey(beta)));
            Assert.That(result.Diagnostics[0].FractionKey, Is.EqualTo(result.Diagnostics[1].FractionKey));
        }

        // AC-WT-003. A full nonalloc buffer is a target/tick-attributed fail-closed contract error, even if every raw hit is ignorable.
        [Test]
        public void FullLinecastBufferFailsClosedWithTargetAndTick()
        {
            for (var index = 0; index < 64; index++)
            {
                CreateBox("Saturation" + index, new Vector2(0.1f + (index * 0.05f), 0f), false);
            }

            var exception = Assert.Throws<InvalidOperationException>(() => Evaluate());
            Assert.That(exception.Message, Does.Contain("target"));
            Assert.That(exception.Message, Does.Contain("7"));
        }

        private TransferLineOfSightResult Evaluate()
        {
            Physics2D.SyncTransforms();
            return TransferLineOfSight.Evaluate(0, 0, 4000, 0, _playerRoot, _targetRoot, "target", new SimulationTick(7));
        }

        private BoxCollider2D CreateBox(string name, Vector2 position, bool isTrigger)
        {
            var gameObject = Track(new GameObject(name));
            gameObject.layer = TransferLineOfSight.Layer;
            gameObject.transform.position = position;
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.1f, 0.5f);
            collider.isTrigger = isTrigger;
            Physics2D.SyncTransforms();
            return collider;
        }

        private void CreateChildBox(Transform root, string name, Vector2 localPosition)
        {
            var child = Track(new GameObject(name));
            child.layer = TransferLineOfSight.Layer;
            child.transform.SetParent(root);
            child.transform.localPosition = localPosition;
            child.AddComponent<BoxCollider2D>().size = new Vector2(0.1f, 0.5f);
            Physics2D.SyncTransforms();
        }

        private static void CreateRootBox(Transform root)
        {
            root.gameObject.layer = TransferLineOfSight.Layer;
            root.gameObject.AddComponent<BoxCollider2D>().size = new Vector2(0.1f, 0.5f);
        }

        private GameObject Track(GameObject gameObject)
        {
            _objects.Add(gameObject);
            return gameObject;
        }
    }
}
