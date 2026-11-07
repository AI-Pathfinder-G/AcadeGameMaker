// REQ-UX-004/006/008, REQ-MOV-001/002/007/008; AC-M5B4-001/002/004.
// Independent candidate-boundary coverage. This does not claim InputRouter ownership.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.Movement
{
    internal sealed class PlayerMovementInputFrameCandidatePlayModeTests
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

        // REQ-UX-004/006/008; AC-M5B4-001/002/004.
        [Test]
        public void LockCandidateIsMutationFreeUntilCommitAndCannotBeReused()
        {
            var player = CreatePlayer();
            var owner = new object();
            player.RegisterInputGateOwner(owner);
            player.SubmitCommand(Command(4, dashPressed: true));
            var beforeQueue = GetPrivate(player, "_commands");
            var beforeSnapshot = player.Snapshot;

            var fakeIssuer = new object();
            var forged = new PlayerMovementController.InputFrameCandidate(
                player, fakeIssuer, owner, new SimulationTick(0),
                (long)GetPrivate(player, "_inputGeneration"),
                (long)GetPrivate(player, "_inputGeneration"),
                new SortedDictionary<int, MovementCommand>(), false, 0, false);
            Assert.Throws<InvalidOperationException>(() => forged.ApplyToMatchingConsumer(player, fakeIssuer, owner));
            Assert.That(GetPrivate(player, "_commands"), Is.SameAs(beforeQueue));

            var candidate = player.PrepareInputFrame(owner, new SimulationTick(0), true, null);
            Assert.That(GetPrivate(player, "_commands"), Is.SameAs(beforeQueue));
            Assert.That(player.Snapshot, Is.EqualTo(beforeSnapshot));
            Assert.DoesNotThrow(() => player.ValidateInputFrameCandidate(owner, candidate));
            Assert.Throws<InvalidOperationException>(() => player.ValidateInputFrameCandidate(new object(), candidate));
            Assert.That(GetPrivate(player, "_commands"), Is.SameAs(beforeQueue));

            Assert.That(player.CommitInputFrame(owner, candidate), Is.EqualTo(1));
            Assert.That(GetPrivate(player, "_inputLocked"), Is.True);
            Assert.Throws<InvalidOperationException>(() => player.CommitInputFrame(owner, candidate));
            Assert.That(GetPrivate(player, "_inputLocked"), Is.True);
        }

        // REQ-UX-004/006/008, REQ-MOV-001/002; AC-M5B4-001/002/004.
        [Test]
        public void UnlockCandidateAcceptsOnlyFreshExactCommandAndInterveningSubmitMakesItStale()
        {
            var player = CreatePlayer();
            var owner = new object();
            player.RegisterInputGateOwner(owner);
            player.CommitInputFrame(owner, player.PrepareInputFrame(owner, new SimulationTick(0), true, null));

            var stale = player.PrepareInputFrame(owner, new SimulationTick(0), false, null);
            player.ApplyInputGate(owner, new SimulationTick(0), false);
            Assert.Throws<InvalidOperationException>(() => player.ValidateInputFrameCandidate(owner, stale));
            Assert.That(GetPrivate(player, "_inputLocked"), Is.False);

            var fresh = player.PrepareInputFrame(owner, new SimulationTick(0), false, Command(0, moveXQ4096: 4096));
            Assert.That(player.CommitInputFrame(owner, fresh), Is.Zero);
            Assert.That(GetPrivate(player, "_inputLocked"), Is.False);
            Assert.Throws<InvalidOperationException>(() => player.PrepareInputFrame(owner, new SimulationTick(0), false,
                Command(1, moveXQ4096: 4096)));

            var queue = GetPrivate(player, "_commands");
            var queueCount = ((IDictionary)queue).Count;
            SetPrivate(player, "_inputGeneration", long.MaxValue);
            Assert.Throws<OverflowException>(() => player.SubmitCommand(Command(8, moveXQ4096: 4096)));
            Assert.That(GetPrivate(player, "_commands"), Is.SameAs(queue));
            Assert.That(((IDictionary)GetPrivate(player, "_commands")).Count, Is.EqualTo(queueCount));
        }

        private PlayerMovementController CreatePlayer()
        {
            var value = new GameObject("InputFrameMovementPlayer");
            _objects.Add(value);
            value.transform.position = new Vector2(0f, 0.81f);
            var body = value.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 3.1315f;
            value.AddComponent<CapsuleCollider2D>().size = new Vector2(0.8f, 1.6f);
            Physics2D.SyncTransforms();
            return value.AddComponent<PlayerMovementController>();
        }

        private static MovementCommand Command(int tick, int moveXQ4096 = 0, bool dashPressed = false) =>
            new MovementCommand(new SimulationTick(tick), moveXQ4096, 0, false, false, dashPressed);

        private static object GetPrivate(object value, string fieldName) =>
            value.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(value);

        private static void SetPrivate(object value, string fieldName, object fieldValue) =>
            value.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(value, fieldValue);
    }
}
