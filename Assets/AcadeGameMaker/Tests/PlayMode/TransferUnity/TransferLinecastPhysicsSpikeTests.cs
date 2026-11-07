// AC-WT-006. Unity 6000.3.21f1 finite Linecast endpoint feasibility spike for REQ-WT-007.
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.TransferUnity
{
    internal sealed class TransferLinecastPhysicsSpikeTests
    {
        private const int TransferLineOfSightLayer = 8;
        private readonly List<GameObject> _objects = new List<GameObject>();
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[8];
        private SimulationMode2D _previousSimulationMode;

        [SetUp]
        public void UseScriptPhysics()
        {
            _previousSimulationMode = Physics2D.simulationMode;
            Physics2D.simulationMode = SimulationMode2D.Script;
        }

        [TearDown]
        public void RestorePhysics()
        {
            for (var index = _objects.Count - 1; index >= 0; index--)
            {
                Object.DestroyImmediate(_objects[index]);
            }

            _objects.Clear();
            Physics2D.simulationMode = _previousSimulationMode;
            Physics2D.SyncTransforms();
        }

        // AC-WT-006. A layer-8 origin overlap is a fraction-zero block.
        [Test]
        public void LinecastOriginOverlapReturnsFractionZero()
        {
            var origin = new Vector2(0f, 0f);
            var endpoint = new Vector2(4f, 0f);
            var originBlocker = CreateBlocker("Origin", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f));
            var originHits = Cast(origin, endpoint);
            Assert.That(originHits, Is.GreaterThan(0));
            Assert.That(_hits[0].collider, Is.EqualTo(originBlocker));
            Assert.That(_hits[0].fraction, Is.EqualTo(0f));
        }

        // AC-WT-006. A finite segment blocks on an interior collider.
        [Test]
        public void LinecastInteriorHitReturnsStrictlyInteriorFraction()
        {
            var origin = new Vector2(0f, 0f);
            var endpoint = new Vector2(4f, 0f);
            var interiorBlocker = CreateBlocker("Interior", new Vector2(2f, 0f), new Vector2(0.5f, 0.5f));
            var interiorHits = Cast(origin, endpoint);
            Assert.That(interiorHits, Is.GreaterThan(0));
            Assert.That(_hits[0].collider, Is.EqualTo(interiorBlocker));
            Assert.That(_hits[0].fraction, Is.GreaterThan(0f).And.LessThan(1f));
        }

        // AC-WT-006. A collider whose near edge only touches the endpoint remains a fraction-one block.
        [Test]
        public void LinecastEndpointTouchReturnsFractionOne()
        {
            var origin = new Vector2(0f, 0f);
            var endpoint = new Vector2(4f, 0f);
            var endpointBlocker = CreateBlocker("Endpoint", new Vector2(4.25f, 0f), new Vector2(0.5f, 0.5f));
            var endpointHits = Cast(origin, endpoint);
            Assert.That(endpointHits, Is.GreaterThan(0));
            Assert.That(_hits[0].collider, Is.EqualTo(endpointBlocker));
            Assert.That(_hits[0].fraction, Is.EqualTo(1f));
        }

        // AC-WT-006. The finite query cannot report a blocker beyond its endpoint.
        [Test]
        public void LinecastExcludesBlockerBeyondEndpoint()
        {
            var origin = new Vector2(0f, 0f);
            var endpoint = new Vector2(4f, 0f);
            CreateBlocker("Behind", new Vector2(5f, 0f), new Vector2(0.5f, 0.5f));
            Assert.That(Cast(origin, endpoint), Is.EqualTo(0));
        }

        private int Cast(Vector2 origin, Vector2 endpoint)
        {
            var filter = new ContactFilter2D();
            filter.NoFilter();
            filter.useLayerMask = true;
            filter.layerMask = 1 << TransferLineOfSightLayer;
            filter.useTriggers = false;
            Physics2D.SyncTransforms();
            return Physics2D.Linecast(origin, endpoint, filter, _hits);
        }

        private BoxCollider2D CreateBlocker(string name, Vector2 position, Vector2 size)
        {
            var gameObject = new GameObject(name) { layer = TransferLineOfSightLayer };
            gameObject.transform.position = position;
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = size;
            _objects.Add(gameObject);
            Physics2D.SyncTransforms();
            return collider;
        }
    }
}
