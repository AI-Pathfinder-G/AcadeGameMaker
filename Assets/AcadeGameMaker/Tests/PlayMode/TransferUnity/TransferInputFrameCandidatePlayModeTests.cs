// REQ-UX-004/006/008, REQ-WT-005/006; AC-M5B4-001/003/004.
// Independent candidate-boundary coverage for the Transfer consumer.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.TransferUnity
{
    internal sealed class TransferInputFrameCandidatePlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private SimulationMode2D _previousSimulationMode;

        [SetUp]
        public void SetUp()
        {
            _previousSimulationMode = Physics2D.simulationMode;
            Physics2D.simulationMode = SimulationMode2D.Script;
        }

        [TearDown]
        public void TearDown()
        {
            for (var index = _objects.Count - 1; index >= 0; index--)
                UnityEngine.Object.DestroyImmediate(_objects[index]);
            _objects.Clear();
            Physics2D.simulationMode = _previousSimulationMode;
            Physics2D.SyncTransforms();
        }

        // REQ-UX-004/006/008, REQ-WT-005/006; AC-M5B4-001/003.
        [Test]
        public void LockCandidateIsMutationFreeAndPreservesMaintenanceFields()
        {
            var rig = CreateRig();
            Bootstrap(rig);
            var owner = new object();
            rig.Driver.RegisterInputGateOwner(owner);
            var tick = new SimulationTick(1);
            var future = new SimulationTick(4);
            rig.Driver.Submit(Input(tick));
            rig.Driver.Submit(Input(future, new[] { new TransferTargetRemoved("box", future) }));
            var queue = GetPrivate(rig.Driver, "_inputs");

            var fakeIssuer = new object();
            var forged = new TransferSimulationDriver.InputFrameCandidate(
                rig.Driver, fakeIssuer, owner, tick,
                (long)GetPrivate(rig.Driver, "_inputGeneration"),
                (long)GetPrivate(rig.Driver, "_inputGeneration"),
                new SortedDictionary<int, TransferSimulationInput>(), false, 0);
            Assert.Throws<InvalidOperationException>(() => forged.ApplyToMatchingConsumer(rig.Driver, fakeIssuer, owner));
            Assert.That(GetPrivate(rig.Driver, "_inputs"), Is.SameAs(queue));

            var candidate = rig.Driver.PrepareInputFrame(owner, tick, true, null);
            Assert.That(GetPrivate(rig.Driver, "_inputs"), Is.SameAs(queue));
            Assert.Throws<InvalidOperationException>(() => rig.Driver.ValidateInputFrameCandidate(new object(), candidate));
            Assert.That(GetPrivate(rig.Driver, "_inputGateLocked"), Is.False);
            Assert.That(rig.Driver.CommitInputFrame(owner, candidate), Is.EqualTo(2));
            Assert.That(GetPrivate(rig.Driver, "_inputGateLocked"), Is.True);

            var locked = Queued(rig.Driver, 1);
            var retained = Queued(rig.Driver, 4);
            Assert.That(locked.Camera.HasValue, Is.False);
            Assert.That(locked.Aim.HasValue, Is.False);
            Assert.That(locked.Press.HasValue, Is.False);
            Assert.That(retained.Removals.Count, Is.EqualTo(1));
            Assert.That(retained.Removals[0].TargetId, Is.EqualTo("box"));
            Assert.Throws<InvalidOperationException>(() => rig.Driver.CommitInputFrame(owner, candidate));
        }

        // REQ-UX-004/006/008, REQ-WT-005/006; AC-M5B4-001/003/004.
        [Test]
        public void InteractiveOnlyInputRejectsAuthorityAndPhaseAdvanceStalesCandidate()
        {
            var rig = CreateRig();
            Bootstrap(rig);
            var owner = new object();
            rig.Driver.RegisterInputGateOwner(owner);
            var tick = new SimulationTick(1);
            var queue = GetPrivate(rig.Driver, "_inputs");
            Assert.Throws<InvalidOperationException>(() => rig.Driver.PrepareInputFrame(owner, tick, false,
                Input(tick, new[] { new TransferTargetRemoved("box", tick) })));
            Assert.That(GetPrivate(rig.Driver, "_inputs"), Is.SameAs(queue));

            var valid = rig.Driver.PrepareInputFrame(owner, tick, false, Input(tick));
            Assert.That(rig.Driver.CommitInputFrame(owner, valid), Is.Zero);
            Assert.That(Queued(rig.Driver, tick.Value).Camera.HasValue, Is.True);

            var stale = rig.Driver.PrepareInputFrame(owner, tick, false, null);
            rig.Driver.AdvanceTransferPhaseForTests();
            rig.Player.AdvanceOneFixedTickForTests();
            Assert.Throws<InvalidOperationException>(() => rig.Driver.ValidateInputFrameCandidate(owner, stale));
            Assert.That(GetPrivate(rig.Driver, "_inputGateLocked"), Is.False);

            SetPrivate(rig.Driver, "_inputGeneration", long.MaxValue);
            var afterPhase = GetPrivate(rig.Driver, "_inputs");
            var afterPhaseCount = ((IDictionary)afterPhase).Count;
            Assert.Throws<OverflowException>(() => rig.Driver.Submit(Input(new SimulationTick(8))));
            Assert.That(GetPrivate(rig.Driver, "_inputs"), Is.SameAs(afterPhase));
            Assert.That(((IDictionary)GetPrivate(rig.Driver, "_inputs")).Count, Is.EqualTo(afterPhaseCount));
        }

        private void Bootstrap(Rig rig)
        {
            rig.Driver.AdvanceTransferPhaseForTests();
            rig.Player.AdvanceOneFixedTickForTests();
        }

        private Rig CreateRig()
        {
            var playerObject = Track(new GameObject("InputFrameTransferPlayer"));
            var body = playerObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            playerObject.AddComponent<CapsuleCollider2D>();
            var player = playerObject.AddComponent<PlayerMovementController>();

            var targetObject = Track(new GameObject("box"));
            targetObject.transform.position = Vector2.right;
            var targetBody = targetObject.AddComponent<Rigidbody2D>();
            targetBody.bodyType = RigidbodyType2D.Kinematic;
            var collider = targetObject.AddComponent<BoxCollider2D>();
            var sink = targetObject.AddComponent<TransferSandboxOwnerStub>();
            sink.ConfigureForTests("box", "Target.Box.Heavy.v1");
            var target = targetObject.AddComponent<TransferTarget>();
            target.ConfigureForTests("box", TransferTargetKind.Box, 0, 0, 0, 0, 100, 100,
                "base", collider, targetObject.transform, sink);

            var registryObject = Track(new GameObject("InputFrameTransferRegistry"));
            var registry = registryObject.AddComponent<TransferTargetRegistry>();
            registry.ConfigureForTests(target);
            var driver = Track(new GameObject("InputFrameTransferDriver")).AddComponent<TransferSimulationDriver>();
            driver.ConfigureForTests(player, registry);
            driver.InitializeForTests();
            return new Rig(player, driver);
        }

        private TransferSimulationInput Input(SimulationTick tick, IReadOnlyList<TransferTargetRemoved> removals = null) =>
            new TransferSimulationInput(tick, Camera(tick.Value - 1), Mouse(tick.Value),
                new TransferPressed(1, tick), removals ?? Array.Empty<TransferTargetRemoved>(), null);

        private static SimulationCameraPoseSnapshot Camera(int tick) =>
            new SimulationCameraPoseSnapshot(new SimulationTick(tick), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);

        private static AimSample Mouse(int tick) =>
            new AimSample(1, AimSource.MousePointer, 1014, 540, true, 4096, 0,
                new SimulationTick(tick - 1), new SimulationTick(tick));

        private static TransferSimulationInput Queued(TransferSimulationDriver driver, int tick) =>
            (TransferSimulationInput)((IDictionary)GetPrivate(driver, "_inputs"))[tick];

        private GameObject Track(GameObject value) { _objects.Add(value); return value; }

        private static object GetPrivate(object value, string fieldName) =>
            value.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(value);

        private static void SetPrivate(object value, string fieldName, object fieldValue) =>
            value.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(value, fieldValue);

        private readonly struct Rig
        {
            internal Rig(PlayerMovementController player, TransferSimulationDriver driver) { Player = player; Driver = driver; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Driver { get; }
        }
    }
}
