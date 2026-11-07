// AC-WT-003, AC-WT-006; AC-MOV-004. M2B2A observes the pre-movement pose and reflects only existing movement modifiers.
using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.TransferUnity
{
    internal sealed class TransferObservationBuilderPlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
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
                UnityEngine.Object.DestroyImmediate(_objects[index]);
            }

            _objects.Clear();
            Physics2D.simulationMode = _previousSimulationMode;
            Physics2D.SyncTransforms();
        }

        // AC-WT-003, AC-WT-006. Player Q4096 conversion, one target AwayFromZero capture, local additions, and removed-ID filtering are all fixed-tick data.
        [Test]
        public void CaptureUsesPriorPoseQuantizesTargetOnceAndExcludesRemovedIds()
        {
            var player = Track(new GameObject("ObservationPlayer"));
            var alpha = CreateTarget("alpha", new Vector2(1.2346f, -2.3456f), 10, -20);
            var bravo = CreateTarget("bravo", new Vector2(-1.2346f, 2.3456f), 0, 0);
            Physics2D.SyncTransforms(); // The externally controlled, single capture-phase synchronization.

            var snapshot = new PlayerMotionSnapshot(new SimulationTick(0), 205, -205, 0, 0, false, string.Empty, WallSide.None, MovementActionState.Normal, true);
            var observations = new TransferObservationBuilder().Capture(
                new SimulationTick(1), snapshot, Camera(0), Mouse(1), new[] { alpha, bravo }, new[] { "bravo" }, player.transform);

            Assert.That(observations.Count, Is.EqualTo(1));
            var observation = observations[0];
            Assert.That(observation.Descriptor.TargetId, Is.EqualTo("alpha"));
            Assert.That(observation.WorldAimPointXQ1000, Is.EqualTo(1245));
            Assert.That(observation.WorldAimPointYQ1000, Is.EqualTo(-2366));
            // Player (205,-205) Q4096 becomes (50,-50) Q1000 by RoundDivAway; the checked range key is then 6792.
            Assert.That(observation.PlayerDistanceKey, Is.EqualTo(6792));
            Assert.That(observation.ObservationTick.Value, Is.EqualTo(1));
        }

        private TransferTarget CreateTarget(string id, Vector2 position, int localX, int localY)
        {
            var gameObject = Track(new GameObject(id));
            gameObject.transform.position = position;
            var collider = gameObject.AddComponent<BoxCollider2D>();
            var sink = gameObject.AddComponent<TestSink>();
            sink.BoundId = id;
            var target = gameObject.AddComponent<TransferTarget>();
            target.ConfigureForTests(id, TransferTargetKind.Box, localX, localY, 0, 0, 100, 100, "base", collider, gameObject.transform, sink);
            return target;
        }

        private GameObject Track(GameObject gameObject)
        {
            _objects.Add(gameObject);
            return gameObject;
        }

        private static SimulationCameraPoseSnapshot Camera(int tick) => new SimulationCameraPoseSnapshot(new SimulationTick(tick), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
        private static AimSample Mouse(int tick) => new AimSample(1, AimSource.MousePointer, 960, 540, true, 4096, 0, new SimulationTick(tick - 1), new SimulationTick(tick));

        public sealed class TestSink : MonoBehaviour, ITransferTargetModifierSink
        {
            internal string BoundId;
            public string BoundTargetId => BoundId;
            public bool IsStillAvailable() => true;
            public bool TryApply(TransferModifierRequest request) => true;
            public void Clear(TransferClearReason reason, SimulationTick tick, int revision) { }
        }
    }
}
