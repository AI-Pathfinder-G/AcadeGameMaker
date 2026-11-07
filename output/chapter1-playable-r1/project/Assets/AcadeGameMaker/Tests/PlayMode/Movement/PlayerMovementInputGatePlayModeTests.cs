// AC-M5B1-001..006. Independent PlayMode coverage for the real movement input-gate sink.
using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.Movement
{
    internal sealed class PlayerMovementInputGatePlayModeTests
    {
        private readonly List<GameObject> _createdObjects = new List<GameObject>();
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
            DestroyTrackedObjects();
            Physics2D.simulationMode = _previousSimulationMode;
            Physics2D.SyncTransforms();
        }

        // REQ-UX-004/008, REQ-MOV-001/002/007/008; AC-M5B1-001.
        [Test]
        public void LockDropsExactAndDistantCommandsAndRejectsLockedSubmission()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("GateGround", new Vector2(0f, -0.5f), new Vector2(16f, 1f));
            var owner = new object();
            player.RegisterInputGateOwner(owner);
            player.SubmitCommand(Command(0, moveXQ4096: 4096));
            player.SubmitCommand(Command(7, dashPressed: true));

            Assert.That(player.ApplyInputGate(owner, new SimulationTick(0), true), Is.EqualTo(2));
            Assert.Throws<InvalidOperationException>(() => player.SubmitCommand(Command(0, moveXQ4096: 4096)));
            Assert.That(player.ApplyInputGate(owner, new SimulationTick(0), false), Is.EqualTo(0));

            player.SubmitCommand(Command(0, moveXQ4096: 4096));
            Step(player);
            for (var tick = 1; tick <= 7; tick++)
            {
                Step(player);
            }

            Assert.That(player.Snapshot.ActionState, Is.Not.EqualTo(MovementActionState.Dashing));
        }

        // REQ-UX-004/008; AC-M5B1-002. Invalid registration/owner/tick calls precede a
        // valid mutation, whose drop count proves that they were mutation-free.
        [Test]
        public void GateOwnerAndTickValidationAreMutationFreeAndIdempotent()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            var owner = new object();
            var otherOwner = new object();
            player.SubmitCommand(Command(4, dashPressed: true));

            Assert.Throws<ArgumentNullException>(() => player.RegisterInputGateOwner(null));
            Assert.Throws<InvalidOperationException>(() => player.ApplyInputGate(null, new SimulationTick(0), true));
            Assert.Throws<InvalidOperationException>(() => player.ApplyInputGate(owner, new SimulationTick(0), true));
            player.RegisterInputGateOwner(owner);
            player.RegisterInputGateOwner(owner);
            Assert.Throws<InvalidOperationException>(() => player.RegisterInputGateOwner(otherOwner));
            Assert.Throws<InvalidOperationException>(() => player.ApplyInputGate(otherOwner, new SimulationTick(0), true));
            Assert.Throws<InvalidOperationException>(() => player.ApplyInputGate(owner, new SimulationTick(-1), true));
            Assert.Throws<InvalidOperationException>(() => player.ApplyInputGate(owner, new SimulationTick(1), true));
            Assert.Throws<InvalidOperationException>(() => player.SetInputLockedForTests(true));

            Assert.That(player.ApplyInputGate(owner, new SimulationTick(0), true), Is.EqualTo(1));
            Assert.That(player.ApplyInputGate(owner, new SimulationTick(0), true), Is.EqualTo(0));
            Assert.That(player.ApplyInputGate(owner, new SimulationTick(0), false), Is.EqualTo(0));
        }

        // REQ-UX-004/008, REQ-MOV-001/002; AC-M5B1-003.
        [Test]
        public void LockedTicksRemainMonotonicAndKeepPlayerActiveAndContinuous()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("ContinuousGround", new Vector2(0f, -0.5f), new Vector2(16f, 1f));
            player.PreflightTransferModifier(new SimulationTick(0));
            player.ApplyPreflightedTransferModifier(PlayerMovementModifierKind.Lightweight, new SimulationTick(0));
            var owner = new object();
            player.RegisterInputGateOwner(owner);
            var gravityBefore = player.GetComponent<Rigidbody2D>().gravityScale;
            var before = player.Snapshot;
            player.ApplyInputGate(owner, new SimulationTick(0), true);

            for (var tick = 0; tick < 4; tick++)
            {
                Step(player);
                Assert.That(player.Snapshot.Tick.Value, Is.EqualTo(tick));
                Assert.That(player.Snapshot.ActionState, Is.EqualTo(MovementActionState.InputLocked));
                Assert.That(player.gameObject.activeSelf, Is.True);
                Assert.That(player.Snapshot.PositionXQ4096, Is.EqualTo(before.PositionXQ4096));
                Assert.That(player.Snapshot.PositionYQ4096, Is.EqualTo(before.PositionYQ4096));
            }

            Assert.That(player.GetComponent<Rigidbody2D>().gravityScale, Is.EqualTo(gravityBefore));
            Assert.That(gravityBefore, Is.EqualTo(3.1315f * 0.65f));

            player.PreflightTransferModifier(new SimulationTick(4));
            player.ApplyPreflightedTransferModifier(PlayerMovementModifierKind.Baseline, new SimulationTick(4));
            Assert.That(player.GetComponent<Rigidbody2D>().gravityScale, Is.EqualTo(3.1315f));
        }

        // REQ-UX-004/008; AC-M5B1-004. Same-state unlock is a no-op after the fresh
        // command is queued; the pre-lock distant dash never reappears.
        [Test]
        public void UnlockAcceptsFreshCommandOnceAndNeverReplaysOldFutureCommand()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("UnlockGround", new Vector2(0f, -0.5f), new Vector2(16f, 1f));
            var owner = new object();
            player.RegisterInputGateOwner(owner);
            player.SubmitCommand(Command(4, dashPressed: true));
            player.ApplyInputGate(owner, new SimulationTick(0), true);
            player.ApplyInputGate(owner, new SimulationTick(0), false);

            var startX = player.Snapshot.PositionXQ4096;
            player.SubmitCommand(Command(0, moveXQ4096: 4096));
            Assert.That(player.ApplyInputGate(owner, new SimulationTick(0), false), Is.EqualTo(0));
            Assert.Throws<InvalidOperationException>(() => player.SubmitCommand(Command(0, moveXQ4096: 4096)));
            Step(player);
            Assert.That(player.Snapshot.PositionXQ4096, Is.GreaterThan(startX));

            for (var tick = 1; tick <= 4; tick++)
            {
                Step(player);
            }

            Assert.That(player.Snapshot.ActionState, Is.Not.EqualTo(MovementActionState.Dashing));
        }

        // REQ-UX-004/008, REQ-MOV-007/008; AC-M5B1-005. The lock-time reset cancels
        // both a motor jump buffer and active dash, even when lock/unlock is a pulse.
        [Test]
        public void LockPulseClearsBufferedJumpAndActiveDash()
        {
            var control = CreatePlayer(new Vector2(0f, 2f));
            var pulsed = CreatePlayer(new Vector2(3f, 2f));
            var jumpOwner = new object();
            pulsed.RegisterInputGateOwner(jumpOwner);
            control.SubmitCommand(Command(0, jumpPressed: true));
            pulsed.SubmitCommand(Command(0, jumpPressed: true));
            Step(control);
            Step(pulsed);
            Assert.That(control.Snapshot.VelocityYQ4096, Is.LessThanOrEqualTo(0));
            Assert.That(pulsed.Snapshot.VelocityYQ4096, Is.LessThanOrEqualTo(0));

            var controlGroundTop = control.Snapshot.PositionYQ4096 / 4096f - 0.8f - 0.01f;
            var pulsedGroundTop = pulsed.Snapshot.PositionYQ4096 / 4096f - 0.8f - 0.01f;
            CreateBox("ControlArrivalGround", new Vector2(0f, controlGroundTop - 0.5f), new Vector2(2f, 1f));
            CreateBox("PulsedArrivalGround", new Vector2(3f, pulsedGroundTop - 0.5f), new Vector2(2f, 1f));
            pulsed.ApplyInputGate(jumpOwner, new SimulationTick(1), true);
            pulsed.ApplyInputGate(jumpOwner, new SimulationTick(1), false);
            Step(control);
            Step(pulsed);
            Assert.That(control.Snapshot.VelocityYQ4096, Is.GreaterThan(0), "Control must consume the buffered jump when ground arrives.");
            Assert.That(pulsed.Snapshot.VelocityYQ4096, Is.LessThanOrEqualTo(0), "Lock pulse must clear the buffered jump.");
            Assert.That(pulsed.Snapshot.ActionState, Is.Not.EqualTo(MovementActionState.WallJumping));

            var dashing = CreatePlayer(new Vector2(6f, 0.81f));
            CreateBox("DashGround", new Vector2(6f, -0.5f), new Vector2(2f, 1f));
            var dashOwner = new object();
            dashing.RegisterInputGateOwner(dashOwner);
            dashing.SubmitCommand(Command(0, dashPressed: true));
            Step(dashing);
            Assert.That(dashing.Snapshot.ActionState, Is.EqualTo(MovementActionState.Dashing));
            dashing.ApplyInputGate(dashOwner, new SimulationTick(1), true);
            dashing.ApplyInputGate(dashOwner, new SimulationTick(1), false);
            Step(dashing);
            Assert.That(dashing.Snapshot.ActionState, Is.Not.EqualTo(MovementActionState.Dashing));
        }

        // REQ-UX-004/008, REQ-MOV-001/002; AC-M5B1-005. The movement gate does not
        // clear the separately-owned external movement directive queue or receipt.
        [Test]
        public void LockPreservesExternalMovementDirectiveQueueAndReceipt()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            var directiveOwner = new object();
            player.RegisterExternalMovementDirectiveProducer(directiveOwner);
            var directiveId = "ordan.audit-pull/0000000000000000020/0000000001";
            var candidate = player.PreflightExternalMovementDirective(
                directiveOwner,
                20,
                new SimulationTick(1),
                directiveId,
                28672,
                4915,
                410);
            player.CommitPreflightedExternalMovementDirective(directiveOwner, candidate);

            var gateOwner = new object();
            player.RegisterInputGateOwner(gateOwner);
            player.ApplyInputGate(gateOwner, new SimulationTick(0), true);
            Step(player);
            player.ApplyInputGate(gateOwner, new SimulationTick(1), false);
            Step(player);

            Assert.That(player.TryGetExternalMovementDirectiveReceipt(new SimulationTick(1), out var receipt), Is.True);
            Assert.That(receipt.HadDirective, Is.True);
            Assert.That(receipt.ConsumedDirectiveId, Is.EqualTo(directiveId));
        }

        // REQ-UX-004/008; AC-M5B1-006. The same fixed-tick script is grouped by
        // each render cadence; traces include gate outcomes and authoritative motion.
        [Test]
        public void RenderGroupingAt30_60_And144ProducesIdenticalGateTrace()
        {
            var thirty = ReplayGateScriptAtRenderRate(30);
            var sixty = ReplayGateScriptAtRenderRate(60);
            var oneFortyFour = ReplayGateScriptAtRenderRate(144);
            CollectionAssert.AreEqual(thirty, sixty);
            CollectionAssert.AreEqual(thirty, oneFortyFour);
            TestContext.WriteLine("AC-M5B1-006 cadence=30,60,144 firstDivergence=none tracesEqual=True");
        }

        private List<string> ReplayGateScriptAtRenderRate(int renderFramesPerSecond)
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("CadenceGround", new Vector2(0f, -0.5f), new Vector2(24f, 1f));
            var owner = new object();
            player.RegisterInputGateOwner(owner);
            var trace = new List<string>();
            var fixedTicks = 0;
            var renderAccumulator = 0;
            var lockDrop = 0;
            var repeatDrop = 0;

            while (fixedTicks < 8)
            {
                renderAccumulator += 60;
                while (renderAccumulator >= renderFramesPerSecond && fixedTicks < 8)
                {
                    lockDrop = 0;
                    repeatDrop = 0;
                    if (fixedTicks == 0)
                    {
                        player.SubmitCommand(Command(0, moveXQ4096: 4096));
                    }
                    else if (fixedTicks == 1)
                    {
                        player.SubmitCommand(Command(4, dashPressed: true));
                        lockDrop = player.ApplyInputGate(owner, new SimulationTick(1), true);
                    }
                    else if (fixedTicks == 2)
                    {
                        player.ApplyInputGate(owner, new SimulationTick(2), false);
                        player.SubmitCommand(Command(2, moveXQ4096: 4096));
                        repeatDrop = player.ApplyInputGate(owner, new SimulationTick(2), false);
                    }
                    else if (fixedTicks == 3)
                    {
                        player.SubmitCommand(Command(3, jumpPressed: true));
                    }

                    Step(player);
                    trace.Add(string.Format(
                        "{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}",
                        fixedTicks,
                        player.Snapshot.Tick.Value,
                        player.Snapshot.PositionXQ4096,
                        player.Snapshot.PositionYQ4096,
                        player.Snapshot.VelocityXQ4096,
                        player.Snapshot.VelocityYQ4096,
                        player.Snapshot.ActionState,
                        lockDrop,
                        repeatDrop));
                    fixedTicks++;
                    renderAccumulator -= renderFramesPerSecond;
                }
            }

            DestroyTrackedObjects();
            return trace;
        }

        private PlayerMovementController CreatePlayer(Vector2 position)
        {
            var gameObject = Track(new GameObject("MovementInputGatePlayer"));
            gameObject.transform.position = position;
            var body = gameObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 3.1315f;
            body.useFullKinematicContacts = false;
            gameObject.AddComponent<CapsuleCollider2D>().size = new Vector2(0.8f, 1.6f);
            var controller = gameObject.AddComponent<PlayerMovementController>();
            Physics2D.SyncTransforms();
            return controller;
        }

        private BoxCollider2D CreateBox(string name, Vector2 position, Vector2 size)
        {
            var gameObject = Track(new GameObject(name));
            gameObject.transform.position = position;
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = size;
            Physics2D.SyncTransforms();
            return collider;
        }

        private void Step(PlayerMovementController player)
        {
            player.AdvanceOneFixedTickForTests();
            Physics2D.Simulate(1f / 60f);
            var body = player.GetComponent<Rigidbody2D>();
            Assert.That(Quantize(body.position.x), Is.EqualTo(player.Snapshot.PositionXQ4096));
            Assert.That(Quantize(body.position.y), Is.EqualTo(player.Snapshot.PositionYQ4096));
        }

        private GameObject Track(GameObject gameObject)
        {
            _createdObjects.Add(gameObject);
            return gameObject;
        }

        private void DestroyTrackedObjects()
        {
            for (var index = _createdObjects.Count - 1; index >= 0; index--)
            {
                UnityEngine.Object.DestroyImmediate(_createdObjects[index]);
            }

            _createdObjects.Clear();
            Physics2D.SyncTransforms();
        }

        private static MovementCommand Command(
            int tick,
            int moveXQ4096 = 0,
            bool jumpPressed = false,
            bool dashPressed = false) =>
            new MovementCommand(new SimulationTick(tick), moveXQ4096, 0, jumpPressed, false, dashPressed);

        private static int Quantize(float value) => checked((int)Math.Round(value * 4096d, MidpointRounding.AwayFromZero));
    }
}
