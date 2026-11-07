// AC-MOV-001, AC-MOV-002, AC-MOV-004, AC-MOV-005, AC-MOV-006.
using System;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.Movement
{
    internal sealed class PlayerMovementControllerPlayModeTests
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

        // AC-MOV-001. A missing tick becomes neutral, and that consumed tick rejects a later command.
        [Test]
        public void NeutralTickIsIrrevocablyConsumedAndLateTickZeroIsRejected()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("Ground", new Vector2(0f, -0.5f), new Vector2(12f, 1f));

            Step(player);
            Assert.That(player.Snapshot.Tick.Value, Is.EqualTo(0));
            Assert.Throws<InvalidOperationException>(() => player.SubmitCommand(Command(0)));
        }

        // AC-MOV-001. Exact future commands are accepted once and duplicate future ticks fail before simulation.
        [Test]
        public void DuplicateFutureCommandIsRejected()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("Ground", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            player.SubmitCommand(Command(4, dashPressed: true));
            Assert.Throws<InvalidOperationException>(() => player.SubmitCommand(Command(4, dashPressed: true)));

            for (var tick = 0; tick < 5; tick++)
            {
                Step(player);
            }

            Assert.That(player.Snapshot.Tick.Value, Is.EqualTo(4));
        }

        // AC-MOV-001, AC-MOV-005. The staged snapshot is not published until the one valid commit.
        [Test]
        public void MotorTransactionRejectsSecondBeginWrongCommitAndSecondCommit()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            var ground = new MovementContacts(true, string.Empty, WallSide.None, false);
            var prediction = motor.BeginTick(Command(0), ground, PlayerMovementModifierKind.Baseline, false);
            Assert.That(motor.Snapshot.Tick.Value, Is.EqualTo(0));
            Assert.Throws<InvalidOperationException>(() => motor.BeginTick(Command(1), ground, PlayerMovementModifierKind.Baseline, false));
            Assert.Throws<InvalidOperationException>(() => motor.CommitTick(new MovementCollisionResolution(new SimulationTick(1), 0, 0, false, false)));
            Assert.That(motor.CommitTick(MovementCollisionResolution.Identity(prediction)).Tick.Value, Is.EqualTo(0));
            Assert.Throws<InvalidOperationException>(() => motor.CommitTick(MovementCollisionResolution.Identity(prediction)));
        }

        // AC-MOV-001, AC-MOV-004. A cast-first landing is grounded, has zero vertical speed, and never penetrates.
        [Test]
        public void FallingOntoGroundUsesResolvedPositionWithoutOverlap()
        {
            var player = CreatePlayer(new Vector2(0f, 2f));
            var ground = CreateBox("Ground", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            for (var tick = 0; tick < 180 && !player.Snapshot.IsGrounded; tick++)
            {
                Step(player);
            }

            Assert.That(player.Snapshot.IsGrounded, Is.True);
            Assert.That(player.Snapshot.VelocityYQ4096, Is.EqualTo(0));
            Assert.That(player.GetComponent<Collider2D>().Distance(ground).isOverlapped, Is.False);
            AssertBodyMatchesSnapshot(player);
        }

        // AC-MOV-001, AC-MOV-004. A grounded jump hits the ceiling through the Y-axis cast and is not allowed to overlap.
        [Test]
        public void GroundJumpHitsCeilingWithZeroVerticalVelocityAndNoOverlap()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("Ground", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            var ceiling = CreateBox("Ceiling", new Vector2(0f, 2.5f), new Vector2(12f, 1f));
            player.SubmitCommand(Command(0, jumpPressed: true));
            Step(player);
            Assert.That(player.Snapshot.VelocityYQ4096, Is.GreaterThan(0));

            for (var tick = 1; tick < 30 && player.Snapshot.VelocityYQ4096 > 0; tick++)
            {
                Step(player);
            }

            Assert.That(player.Snapshot.VelocityYQ4096, Is.EqualTo(0));
            AssertNotOverlapped(player, ceiling);
            AssertBodyMatchesSnapshot(player);
        }

        // AC-MOV-001, AC-MOV-005. Rightward motion reaches a cast-resolved wall with no penetration or residual X velocity.
        [Test]
        public void RightwardMotionStopsAtRightWall()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("Ground", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            var wall = CreateBox("RightWall", new Vector2(1f, 1f), new Vector2(0.5f, 4f));
            MoveUntilBlocked(player, 4096);

            Assert.That(player.Snapshot.VelocityXQ4096, Is.EqualTo(0));
            AssertNotOverlapped(player, wall);
            AssertBodyMatchesSnapshot(player);
        }

        // AC-MOV-001, AC-MOV-005. Leftward motion is independently resolved against its own wall.
        [Test]
        public void LeftwardMotionStopsAtLeftWall()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("Ground", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            var wall = CreateBox("LeftWall", new Vector2(-1f, 1f), new Vector2(0.5f, 4f));
            MoveUntilBlocked(player, -4096);

            Assert.That(player.Snapshot.VelocityXQ4096, Is.EqualTo(0));
            AssertNotOverlapped(player, wall);
            AssertBodyMatchesSnapshot(player);
        }

        // AC-MOV-005. A blocked dash keeps its exact start X for 15 scheduled ticks and observes cooldown age 35/36.
        [Test]
        public void BlockedDashPreservesScheduleVelocityPositionAndCooldownBoundary()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("Ground", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            CreateBox("DashWall", new Vector2(0.7f, 1f), new Vector2(0.2f, 4f));
            var startX = player.Snapshot.PositionXQ4096;
            player.SubmitCommand(Command(0, dashPressed: true));
            for (var tick = 0; tick < 15; tick++)
            {
                Step(player);
                Assert.That(player.Snapshot.ActionState, Is.EqualTo(MovementActionState.Dashing));
                Assert.That(player.Snapshot.PositionXQ4096, Is.EqualTo(startX));
                Assert.That(player.Snapshot.VelocityXQ4096, Is.EqualTo(FixedQ4096.DashVelocity));
            }

            for (var tick = 15; tick <= 49; tick++)
            {
                player.SubmitCommand(Command(tick, dashPressed: tick == 49));
                Step(player);
            }

            Assert.That(player.Snapshot.ActionState, Is.Not.EqualTo(MovementActionState.Dashing));
            player.SubmitCommand(Command(50, dashPressed: true));
            Step(player);
            Assert.That(player.Snapshot.ActionState, Is.EqualTo(MovementActionState.Dashing));
        }

        // REQ-MOV-001, REQ-MOV-004, REQ-MOV-006; AC-MOV-001, AC-MOV-004.
        // The shared raw-hit filter applies to the downward ground probe, and false restores eligibility.
        [Test]
        public void IgnoredKinematicBoxIsSkippedByGroundProbeAndFalseRestoresNextTick()
        {
            const float IsolatedX = 1000f;
            var player = CreatePlayer(new Vector2(IsolatedX, 0.84f));
            var walker = CreateKinematicBox("walker", new Vector2(IsolatedX, -0.1f), new Vector2(0.8f, 0.2f));
            var playerCollider = player.GetComponent<Collider2D>();

            Step(player);
            Assert.That(player.Snapshot.IsGrounded, Is.True);
            Physics2D.IgnoreCollision(playerCollider, walker, true);
            Assert.That(Physics2D.GetIgnoreCollision(playerCollider, walker), Is.True);
            AssertRawCastContains(playerCollider, Vector2.down, 205f / 4096f, walker);
            Step(player);
            Assert.That(player.Snapshot.IsGrounded, Is.False, "The ignored pair must not remain authoritative ground.");

            Physics2D.IgnoreCollision(playerCollider, walker, false);
            Step(player);
            Assert.That(player.Snapshot.IsGrounded, Is.True, "Pair false must restore ground eligibility on the next query.");
            Assert.That(playerCollider.Distance(walker).isOverlapped, Is.False);
        }

        // REQ-MOV-001, REQ-MOV-006; AC-MOV-001, AC-MOV-005. The existing dash
        // obstruction and horizontal resolution both consume the same ignored-pair filter.
        [Test]
        public void IgnoredKinematicBoxDoesNotBlockDashOrHorizontalResolution()
        {
            var control = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("ControlGround", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            CreateKinematicBox("walker", new Vector2(0.7f, 1f), new Vector2(0.2f, 4f));
            var controlStart = control.Snapshot.PositionXQ4096;
            control.SubmitCommand(Command(0, dashPressed: true));
            Step(control);
            Assert.That(control.Snapshot.PositionXQ4096, Is.EqualTo(controlStart));

            DestroyTrackedObjects();
            var ignored = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("IgnoredGround", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            var obstacle = CreateKinematicBox("walker", new Vector2(0.7f, 1f), new Vector2(0.2f, 4f));
            var ignoredCollider = ignored.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(ignoredCollider, obstacle, true);
            var ignoredStart = ignored.Snapshot.PositionXQ4096;
            ignored.SubmitCommand(Command(0, dashPressed: true));
            Step(ignored);

            Assert.That(ignored.Snapshot.ActionState, Is.EqualTo(MovementActionState.Dashing));
            Assert.That(ignored.Snapshot.PositionXQ4096, Is.GreaterThan(ignoredStart));
            Assert.That(Physics2D.GetIgnoreCollision(ignoredCollider, obstacle), Is.True);
        }

        // REQ-MOV-001, REQ-MOV-006; AC-MOV-001, AC-MOV-004. Upward Y-axis
        // resolution must skip the ignored raw hit without changing jump prediction.
        [Test]
        public void IgnoredKinematicBoxDoesNotBlockUpwardYAxisResolution()
        {
            var control = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("ControlGround", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            CreateKinematicBox("surveyor", new Vector2(0f, 2.05f), new Vector2(0.8f, 0.8f));
            var controlStart = control.Snapshot.PositionYQ4096;
            control.SubmitCommand(Command(0, jumpPressed: true));
            Step(control);
            var controlBlockedY = control.Snapshot.PositionYQ4096;
            Assert.That(controlBlockedY, Is.GreaterThanOrEqualTo(controlStart));
            Assert.That(control.Snapshot.VelocityYQ4096, Is.EqualTo(0));

            DestroyTrackedObjects();
            var ignored = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("IgnoredGround", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            var ceiling = CreateKinematicBox("surveyor", new Vector2(0f, 2.05f), new Vector2(0.8f, 0.8f));
            var ignoredCollider = ignored.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(ignoredCollider, ceiling, true);
            var ignoredStart = ignored.Snapshot.PositionYQ4096;
            ignored.SubmitCommand(Command(0, jumpPressed: true));
            Step(ignored);

            Assert.That(ignored.Snapshot.PositionYQ4096, Is.GreaterThan(controlBlockedY));
            Assert.That(ignored.Snapshot.PositionYQ4096, Is.GreaterThan(ignoredStart));
            Assert.That(ignored.Snapshot.VelocityYQ4096, Is.GreaterThan(0));
            Assert.That(Physics2D.GetIgnoreCollision(ignoredCollider, ceiling), Is.True);
        }

        // AC-MOV-005. Stable IDs are authored scene name plus ordinal transform hierarchy path, never instance IDs.
        [Test]
        public void StableWallIdIsExactlySceneNameAndHierarchyPath()
        {
            var root = Track(new GameObject("StableRoot"));
            var wallObject = Track(new GameObject("StableWall"));
            wallObject.transform.SetParent(root.transform);
            var wall = wallObject.AddComponent<BoxCollider2D>();
            var expected = wallObject.scene.name + ":StableRoot/StableWall";
            Assert.That(PlayerMovementController.GetStableAuthoredId(wall), Is.EqualTo(expected));
        }

        // REQ-MOV-004, REQ-MOV-006; AC-MOV-002, AC-MOV-004, AC-MOV-006. Kinematic compatibility value follows modifier entry and exit.
        [Test]
        public void CompatibilityGravityScaleTracksLightweightAndRestoresBaseline()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("Ground", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            var body = player.GetComponent<Rigidbody2D>();

            Step(player);
            Assert.That(body.gravityScale, Is.EqualTo(3.1315f).Within(0.00001f));
            player.SetModifierForTests(PlayerMovementModifierKind.Lightweight);
            Step(player);
            Assert.That(body.gravityScale, Is.EqualTo(3.1315f * 0.65f).Within(0.00001f));
            player.SetModifierForTests(PlayerMovementModifierKind.Baseline);
            Step(player);
            Assert.That(body.gravityScale, Is.EqualTo(3.1315f).Within(0.00001f));
        }

        // REQ-WT-006, REQ-MOV-004; AC-MOV-004. The transfer preflight is read-only, then the existing modifier reflects before the motor tick.
        [Test]
        public void TransferModifierPreflightRejectsWrongTickWithoutMutationThenReflectsGravity()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            var body = player.GetComponent<Rigidbody2D>();
            var initial = player.Snapshot;
            var gravity = body.gravityScale;

            Assert.Throws<InvalidOperationException>(() => player.PreflightTransferModifier(new SimulationTick(1)));
            Assert.That(player.Snapshot.Tick, Is.EqualTo(initial.Tick));
            Assert.That(player.Snapshot.PositionXQ4096, Is.EqualTo(initial.PositionXQ4096));
            Assert.That(body.gravityScale, Is.EqualTo(gravity));

            player.PreflightTransferModifier(new SimulationTick(0));
            player.ApplyPreflightedTransferModifier(PlayerMovementModifierKind.Lightweight, new SimulationTick(0));
            Assert.That(body.gravityScale, Is.EqualTo(3.1315f * 0.65f).Within(0.00001f));
            player.ApplyPreflightedTransferModifier(PlayerMovementModifierKind.Baseline, new SimulationTick(0));
            Assert.That(body.gravityScale, Is.EqualTo(3.1315f).Within(0.00001f));
        }

        // AC-M4B3B3-003, AC-M4B3B3-005, AC-M4B3B3-006. The controller owns the
        // future exact-tick queue, combines pull with the existing cast path, and
        // publishes the receipt only at the same successful commit boundary.
        [Test]
        public void BalanceAuditPullQueueIsOwnerBoundAndPublishesAnAtomicReceipt()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("PullGround", new Vector2(0f, -0.5f), new Vector2(24f, 1f));
            var owner = new object();
            player.RegisterExternalMovementDirectiveProducer(owner);
            Assert.Throws<InvalidOperationException>(() => player.RegisterExternalMovementDirectiveProducer(new object()));

            const long Ordinal = 7;
            const string DeliveryOneId = "ordan.audit-pull/0000000000000000007/0000000001";
            var candidate = player.PreflightExternalMovementDirective(
                owner,
                Ordinal,
                new SimulationTick(1),
                DeliveryOneId,
                28672,
                4915,
                410);
            Assert.Throws<InvalidOperationException>(() => player.CommitPreflightedExternalMovementDirective(new object(), candidate));
            player.CommitPreflightedExternalMovementDirective(owner, candidate);
            Assert.Throws<InvalidOperationException>(() => player.CommitPreflightedExternalMovementDirective(owner, candidate));

            Step(player);
            Assert.That(player.TryGetExternalMovementDirectiveReceipt(new SimulationTick(0), out var tickZeroReceipt), Is.True);
            Assert.That(tickZeroReceipt.HadDirective, Is.False);
            Assert.That(tickZeroReceipt.ConsumedDirectiveId, Is.Empty);

            var beforePull = player.Snapshot;
            Step(player);
            Assert.That(player.TryGetExternalMovementDirectiveReceipt(new SimulationTick(1), out var receipt), Is.True);
            Assert.That(receipt.HadDirective, Is.True);
            Assert.That(receipt.ConsumedDirectiveId, Is.EqualTo(DeliveryOneId));
            Assert.That((long)receipt.AppliedDeltaXQ4096 * receipt.AppliedDeltaXQ4096 +
                        (long)receipt.AppliedDeltaYQ4096 * receipt.AppliedDeltaYQ4096,
                Is.LessThanOrEqualTo(410L * 410L));
            Assert.That(player.Snapshot.PositionXQ4096, Is.GreaterThan(beforePull.PositionXQ4096));
            AssertBodyMatchesSnapshot(player);

            Assert.Throws<InvalidOperationException>(() => player.CancelExternalMovementDirective(
                owner,
                Ordinal,
                new SimulationTick(1),
                DeliveryOneId));
        }

        [Test]
        public void BalanceAuditPullCombinedPathResolvesWallAndCeilingWithoutTunneling()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("PullGround", new Vector2(0f, -0.5f), new Vector2(24f, 1f));
            var wall = CreateBox("PullWall", new Vector2(0.52f, 1f), new Vector2(0.2f, 4f));
            var ceiling = CreateBox("PullCeiling", new Vector2(0f, 1.75f), new Vector2(4f, 0.2f));
            var owner = new object();
            player.RegisterExternalMovementDirectiveProducer(owner);
            var candidate = player.PreflightExternalMovementDirective(owner, 8, new SimulationTick(1),
                "ordan.audit-pull/0000000000000000008/0000000001", 28672, 4915, 410);
            player.CommitPreflightedExternalMovementDirective(owner, candidate);

            Step(player);
            var before = player.Snapshot;
            player.SubmitCommand(new MovementCommand(new SimulationTick(1), 0, 0, true, false, false));
            Step(player);

            Assert.That(player.TryGetExternalMovementDirectiveReceipt(new SimulationTick(1), out var receipt), Is.True);
            Assert.That(receipt.HadDirective, Is.True);
            Assert.That(receipt.AppliedDeltaXQ4096, Is.GreaterThan(0));
            Assert.That(receipt.AppliedDeltaYQ4096, Is.GreaterThan(0));
            Assert.That(player.Snapshot.PositionXQ4096 - before.PositionXQ4096, Is.LessThan(receipt.AppliedDeltaXQ4096));
            Assert.That(player.Snapshot.VelocityYQ4096, Is.Zero);
            AssertNotOverlapped(player, wall);
            AssertNotOverlapped(player, ceiling);
        }

        [Test]
        public void FailedDueTickDoesNotPublishOrReplaceDirectiveReceipt()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("PullGround", new Vector2(0f, -0.5f), new Vector2(24f, 1f));
            var owner = new object();
            player.RegisterExternalMovementDirectiveProducer(owner);
            var candidate = player.PreflightExternalMovementDirective(owner, 9, new SimulationTick(1),
                "ordan.audit-pull/0000000000000000009/0000000001", 28672, 4915, 410);
            player.CommitPreflightedExternalMovementDirective(owner, candidate);
            Step(player);
            Assert.That(player.TryGetExternalMovementDirectiveReceipt(new SimulationTick(0), out var prior), Is.True);

            var motorField = typeof(PlayerMovementController).GetField("_motor", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(motorField, Is.Not.Null);
            var motor = (PlayerMovementMotor)motorField.GetValue(player);
            motor.Advance(new MovementCommand(new SimulationTick(1), 0, 0, false, false, false),
                new MovementContacts(true, string.Empty, WallSide.None, false), PlayerMovementModifierKind.Baseline, false);

            Assert.Throws<InvalidOperationException>(() => player.AdvanceOneFixedTickForTests());
            Assert.That(player.TryGetExternalMovementDirectiveReceipt(new SimulationTick(1), out _), Is.False);
            Assert.That(player.TryGetExternalMovementDirectiveReceipt(new SimulationTick(0), out var retained), Is.True);
            Assert.That(retained.HadDirective, Is.EqualTo(prior.HadDirective));
            Assert.That(retained.ConsumedDirectiveId, Is.EqualTo(prior.ConsumedDirectiveId));
        }

        [Test]
        public void BaselineAndLightweightApplySamePullAfterTransferModifierReflection()
        {
            var baseline = RunPullWithTransferModifier(PlayerMovementModifierKind.Baseline, 10);
            DestroyTrackedObjects();
            var lightweight = RunPullWithTransferModifier(PlayerMovementModifierKind.Lightweight, 11);

            Assert.That(lightweight.Receipt.AppliedDeltaXQ4096, Is.EqualTo(baseline.Receipt.AppliedDeltaXQ4096));
            Assert.That(lightweight.Receipt.AppliedDeltaYQ4096, Is.EqualTo(baseline.Receipt.AppliedDeltaYQ4096));
            Assert.That(lightweight.Consumed.VelocityYQ4096, Is.Not.EqualTo(baseline.Consumed.VelocityYQ4096));
            Assert.That(lightweight.AfterClear.Tick.Value, Is.EqualTo(lightweight.Consumed.Tick.Value + 1));
            Assert.That(lightweight.AfterClearReceipt.HadDirective, Is.False,
                "modifier clear must not create a second movement publication or replay the pull");
        }

        [Test]
        public void LaterAttackOrdinalCanReuseLaneAfterPriorDirectiveIsConsumed()
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            var owner = new object();
            player.RegisterExternalMovementDirectiveProducer(owner);
            var first = player.PreflightExternalMovementDirective(owner, 20, new SimulationTick(1),
                "ordan.audit-pull/0000000000000000020/0000000001", 28672, 4915, 410);
            player.CommitPreflightedExternalMovementDirective(owner, first);
            Step(player);
            Step(player);

            var second = player.PreflightExternalMovementDirective(owner, 21, new SimulationTick(3),
                "ordan.audit-pull/0000000000000000021/0000000003", 28672, 4915, 410);
            player.CommitPreflightedExternalMovementDirective(owner, second);
            Step(player);
            Step(player);

            Assert.That(player.TryGetExternalMovementDirectiveReceipt(new SimulationTick(3), out var receipt), Is.True);
            Assert.That(receipt.ConsumedDirectiveId,
                Is.EqualTo("ordan.audit-pull/0000000000000000021/0000000003"));
        }

        // AC-MOV-001, AC-MOV-002, AC-MOV-004, AC-MOV-005. Render cadence only groups preloaded 60 Hz commands.
        [Test]
        public void ThirtySixtyAndOneFortyFourRenderCadencesReplayIdenticalControllerSnapshots()
        {
            var commands = new List<MovementCommand>();
            for (var tick = 0; tick < 120; tick++)
            {
                commands.Add(new MovementCommand(
                    new SimulationTick(tick),
                    tick < 30 ? 4096 : 0,
                    0,
                    tick == 2,
                    tick == 10,
                    tick == 45));
            }

            var thirty = ReplayControllerAtRenderRate(commands, 30);
            var sixty = ReplayControllerAtRenderRate(commands, 60);
            var oneFortyFour = ReplayControllerAtRenderRate(commands, 144);
            for (var index = 0; index < thirty.Count; index++)
            {
                AssertSnapshotEqual(thirty[index], sixty[index]);
                AssertSnapshotEqual(thirty[index], oneFortyFour[index]);
            }
        }

        private List<PlayerMotionSnapshot> ReplayControllerAtRenderRate(IReadOnlyList<MovementCommand> commands, int renderFramesPerSecond)
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            CreateBox("ReplayGround", new Vector2(0f, -0.5f), new Vector2(24f, 1f));
            CreateBox("ReplayWall", new Vector2(5f, 1f), new Vector2(0.5f, 4f));
            foreach (var command in commands)
            {
                player.SubmitCommand(command);
            }

            var snapshots = new List<PlayerMotionSnapshot>();
            var fixedTicksPerRender = 0d;
            while (snapshots.Count < commands.Count)
            {
                fixedTicksPerRender += 60d / renderFramesPerSecond;
                while (fixedTicksPerRender >= 1d && snapshots.Count < commands.Count)
                {
                    Step(player);
                    snapshots.Add(player.Snapshot);
                    fixedTicksPerRender -= 1d;
                }
            }

            DestroyTrackedObjects();
            return snapshots;
        }

        private (ExternalMovementDirectiveReceipt Receipt, PlayerMotionSnapshot Consumed,
            ExternalMovementDirectiveReceipt AfterClearReceipt, PlayerMotionSnapshot AfterClear)
            RunPullWithTransferModifier(PlayerMovementModifierKind modifier, long ordinal)
        {
            var player = CreatePlayer(new Vector2(0f, 0.81f));
            var owner = new object();
            player.RegisterExternalMovementDirectiveProducer(owner);
            var id = $"ordan.audit-pull/{ordinal:D19}/0000000001";
            var candidate = player.PreflightExternalMovementDirective(owner, ordinal, new SimulationTick(1),
                id, 28672, 4915, 410);
            player.CommitPreflightedExternalMovementDirective(owner, candidate);
            Step(player);

            player.PreflightTransferModifier(new SimulationTick(1));
            player.ApplyPreflightedTransferModifier(modifier, new SimulationTick(1));
            Step(player);
            Assert.That(player.TryGetExternalMovementDirectiveReceipt(new SimulationTick(1), out var receipt), Is.True);
            var consumed = player.Snapshot;

            player.PreflightTransferModifier(new SimulationTick(2));
            player.ApplyPreflightedTransferModifier(PlayerMovementModifierKind.Baseline, new SimulationTick(2));
            Step(player);
            Assert.That(player.TryGetExternalMovementDirectiveReceipt(new SimulationTick(2), out var afterClearReceipt), Is.True);
            return (receipt, consumed, afterClearReceipt, player.Snapshot);
        }

        private void MoveUntilBlocked(PlayerMovementController player, int moveXQ4096)
        {
            for (var tick = 0; tick < 90; tick++)
            {
                player.SubmitCommand(new MovementCommand(player.NextExpectedTick, moveXQ4096, 0, false, false, false));
                Step(player);
                if (player.Snapshot.VelocityXQ4096 == 0 && tick > 0)
                {
                    return;
                }
            }

            Assert.Fail("Motion did not reach the authored wall within 90 ticks.");
        }

        // AC-AUTOCH1-SUPPORT-001: synthetic isolated component fixture; never a human playthrough.
        private sealed class SupportInputSource:IInputFrameCommitSource
        {public InputFrameCommitReceipt? CurrentReceipt{get;set;}public bool IsFaulted{get;set;}}
        private void SupportStep(PlayerMovementController player,SupportInputSource source,MovementCommand command)
        {
            var receipt=new InputFrameCommitReceipt(command.Tick,command.Tick.Value,0,InputMode.GameplayEnabled);source.CurrentReceipt=receipt;
            var candidate=player.PrepareInputFrame(source,command.Tick,false,command,receipt);player.CommitInputFrame(source,candidate);Step(player);
        }
        // AC-AUTOCH1-SUPPORT-001: native setter observations are distinct from the synthetic exact-comparison checks.
        private sealed class TinyGeometryProbe
        {
            private readonly PlayerMovementController _player;private readonly Collider2D _support;private readonly InputFrameCommitReceipt _receipt;
            private int _rows,_bytes,_dropped,_recordFailures,_unchanged,_tiny,_larger;private bool _closed;
            internal TinyGeometryProbe(PlayerMovementController player,Collider2D support,InputFrameCommitReceipt receipt){_player=player;_support=support;_receipt=receipt;}
            private static bool Same(float[] a,float[] b){if(a==null||b==null||a.Length!=b.Length)return false;for(var i=0;i<a.Length;i++)if(a[i]!=b[i])return false;return true;}
            private static string Values(float[] value){if(value==null)return "unobserved";var result="";for(var i=0;i<value.Length;i++)result+=(i==0?"":",")+value[i].ToString("R",System.Globalization.CultureInfo.InvariantCulture);return result;}
            private static bool Approximately(float[] a,float[] b)
            {if(a.Length==2)return new Vector2(a[0],a[1])==new Vector2(b[0],b[1]);if(a.Length==3)return new Vector3(a[0],a[1],a[2])==new Vector3(b[0],b[1],b[2]);return new Quaternion(a[0],a[1],a[2],a[3])==new Quaternion(b[0],b[1],b[2],b[3]);}
            internal void Check(string label,Func<float[]> getter,Action<float[]> setter,int component)
            {
                var baseline=getter();var changedAndRejected=false;
                foreach(var delta in new[]{.000001f,.000004f,.000008f,1f})
                {
                    var requested=(float[])baseline.Clone();requested[component]+=delta;float[] actual=null,restored=null;Exception primary=null;
                    var category="NotAttempted";var baselineSupport=false;var restoreGood=false;var restoredSupport=false;var restoreError="none";
                    try
                    {
                        baselineSupport=_player.TryReadCompletedSupport(_receipt.Tick,_support,_receipt);
                        Assert.That(baselineSupport,Is.True,"변경 전 원본 지지 증거가 없습니다: "+label);
                        setter(requested);actual=getter();
                        if(Same(baseline,actual)){category="SetterUnchanged";_unchanged++;}
                        else
                        {
                            category="ActualChangedNotYetRejected";
                            Assert.That(_player.TryReadCompletedSupport(_receipt.Tick,_support,_receipt),Is.False,"실제 getter 변경이 지지 증거에서 거절되지 않았습니다: "+label);
                            changedAndRejected=true;
                            if(Approximately(baseline,actual)){category="ActualTinyRejected";_tiny++;}else{category="ActualLargerRejected";_larger++;}
                        }
                    }
                    catch(Exception error){primary=error;throw;}
                    finally
                    {
                        try{setter(baseline);restored=getter();restoreGood=Same(baseline,restored);if(restoreGood)restoredSupport=_player.TryReadCompletedSupport(_receipt.Tick,_support,_receipt);}catch(Exception error){restoreError=error.GetType().Name;}
                        Emit("target="+label+" category="+category+" baseline="+Values(baseline)+" requested="+Values(requested)+" actual="+Values(actual)+" restored="+Values(restored)+" baselineSupport="+baselineSupport+" restoreExact="+restoreGood+" restoredSupport="+restoredSupport+" restoreError="+restoreError+" primary="+(primary==null?"none":primary.GetType().Name),false);
                        if(primary==null){Assert.That(restoreGood,Is.True,"정확 baseline 복원이 실패하여 다음 후보로 진행하지 않습니다: "+label);Assert.That(restoredSupport,Is.True,"복원 후 원본 지지 증거가 회복되지 않아 다음 후보로 진행하지 않습니다: "+label);}
                    }
                    if(changedAndRejected)break;
                }
                Assert.That(changedAndRejected,Is.True,"고정 후보 네 개 모두 실제 getter를 바꾸지 못했습니다: "+label);
            }
            private void Emit(string raw,bool summary)
            {
                try
                {
                    var text="[CH1TinyGeometry] "+raw;var truncated=false;while(System.Text.Encoding.UTF8.GetByteCount(text)>1850){text=text.Substring(0,text.Length-1);truncated=true;}text+=" truncated="+truncated;
                    var bytes=System.Text.Encoding.UTF8.GetByteCount(text)+1;
                    if(!summary && (_rows>=32 || _bytes+bytes>65536-1900)){_dropped++;return;}
                    if(_rows>=33 || bytes>1900 || _bytes+bytes>65536){_dropped++;return;}_rows++;_bytes+=bytes;Debug.Log(text);
                }catch{_recordFailures++;}
            }
            internal void Close(){if(_closed)return;_closed=true;Emit("stage=finally-summary attempts="+_rows+" bytesBefore="+_bytes+" setterUnchanged="+_unchanged+" actualTinyRejected="+_tiny+" actualLargerRejected="+_larger+" dropped="+_dropped+" limitExceeded="+(_dropped>0)+" recordingFailures="+_recordFailures+" syntheticExact19Separate=true",true);}
        }
        [Test] public void CompletedSupportUsesOriginalGroundOrDownwardHitAndRejectsForeignOrExpiredReceipt()
        {
            var player=CreatePlayer(new Vector2(0,.81f));var platform=CreateBox("ExactSupport",new Vector2(0,-.5f),new Vector2(2,1));
            var foreign=CreateBox("ForeignSupport",new Vector2(5,-.5f),new Vector2(2,1));var source=new SupportInputSource();
            var zero=new InputFrameCommitReceipt(new SimulationTick(0),0,0,InputMode.GameplayEnabled);
            Assert.That(player.TryReadCompletedSupport(new SimulationTick(0),platform,zero),Is.False);
            player.RegisterCoordinatedInputOwner(source,source,new SimulationTick(0));SupportStep(player,source,Command(0));
            Assert.That(player.GetComponent<Rigidbody2D>().bodyType,Is.EqualTo(RigidbodyType2D.Kinematic));Assert.That(player.GetComponent<Rigidbody2D>().useFullKinematicContacts,Is.False);Assert.That(platform.attachedRigidbody,Is.Null);
            Assert.That(player.TryReadCompletedSupport(new SimulationTick(0),platform,zero),Is.True);
            Assert.That(player.TryReadCompletedSupport(new SimulationTick(0),foreign,zero),Is.False);
            var overlap=CreateBox("ZOtherAtSameHeight",new Vector2(0,-.5f),new Vector2(2,1));Assert.That(player.TryReadCompletedSupport(new SimulationTick(0),overlap,zero),Is.False);
            foreach(var wrong in new[]{new InputFrameCommitReceipt(new SimulationTick(1),0,0,InputMode.GameplayEnabled),new InputFrameCommitReceipt(new SimulationTick(0),1,0,InputMode.GameplayEnabled),new InputFrameCommitReceipt(new SimulationTick(0),0,1,InputMode.GameplayEnabled),new InputFrameCommitReceipt(new SimulationTick(0),0,0,InputMode.Transition)})
                Assert.That(player.TryReadCompletedSupport(new SimulationTick(0),platform,wrong),Is.False);
            platform.enabled=false;Assert.That(player.TryReadCompletedSupport(new SimulationTick(0),platform,zero),Is.False);platform.enabled=true;
            var originalSize=platform.size;platform.size+=Vector2.right;Assert.That(player.TryReadCompletedSupport(new SimulationTick(0),platform,zero),Is.False);platform.size=originalSize;
            var capsule=player.GetComponent<CapsuleCollider2D>();var tiny=new TinyGeometryProbe(player,platform,zero);
            try
            {
                tiny.Check("support.size",()=>new[]{platform.size.x,platform.size.y},v=>platform.size=new Vector2(v[0],v[1]),0);
                tiny.Check("support.position",()=>new[]{platform.transform.position.x,platform.transform.position.y,platform.transform.position.z},v=>platform.transform.position=new Vector3(v[0],v[1],v[2]),0);
                tiny.Check("support.offset",()=>new[]{platform.offset.x,platform.offset.y},v=>platform.offset=new Vector2(v[0],v[1]),0);
                tiny.Check("support.rotation",()=>new[]{platform.transform.rotation.x,platform.transform.rotation.y,platform.transform.rotation.z,platform.transform.rotation.w},v=>platform.transform.rotation=new Quaternion(v[0],v[1],v[2],v[3]),2);
                tiny.Check("player.size",()=>new[]{capsule.size.x,capsule.size.y},v=>capsule.size=new Vector2(v[0],v[1]),0);
                tiny.Check("player.offset",()=>new[]{capsule.offset.x,capsule.offset.y},v=>capsule.offset=new Vector2(v[0],v[1]),0);
                tiny.Check("player.scale",()=>new[]{player.transform.localScale.x,player.transform.localScale.y,player.transform.localScale.z},v=>player.transform.localScale=new Vector3(v[0],v[1],v[2]),0);
                tiny.Check("player.rotation",()=>new[]{player.transform.rotation.x,player.transform.rotation.y,player.transform.rotation.z,player.transform.rotation.w},v=>player.transform.rotation=new Quaternion(v[0],v[1],v[2],v[3]),2);
            }finally{tiny.Close();}
            SupportStep(player,source,Command(1,true));Assert.That(player.TryReadCompletedSupport(new SimulationTick(0),platform,zero),Is.False);Assert.That(player.TryReadCompletedSupport(new SimulationTick(1),platform,source.CurrentReceipt.Value),Is.False);
            var landed=false;for(var tick=2;tick<120;tick++){SupportStep(player,source,Command(tick));if(player.TryReadCompletedSupport(new SimulationTick(tick),platform,source.CurrentReceipt.Value)){landed=true;break;}}Assert.That(landed,Is.True,"실제 하강 해결/정지의 원본 지지가 없습니다.");
            var receipt=source.CurrentReceipt.Value;source.IsFaulted=true;Assert.That(player.TryReadCompletedSupport(receipt.Tick,platform,receipt),Is.False);source.IsFaulted=false;
            player.enabled=false;player.enabled=true;Assert.That(player.TryReadCompletedSupport(receipt.Tick,platform,receipt),Is.False);
            var next=player.NextExpectedTick.Value;SupportStep(player,source,Command(next));
            var left=false;var roundedEdgeObserved=false;for(var tick=next+1;tick<next+90;tick++){SupportStep(player,source,new MovementCommand(new SimulationTick(tick),4096,0,false,false,false));var x=player.Snapshot.PositionXQ4096/4096f;if(x>1f && x<1.4f){Assert.That(player.TryReadCompletedSupport(new SimulationTick(tick),platform,source.CurrentReceipt.Value),Is.False,"둥근 캡슐 끝부분만 겹치는 경우 지지 증거가 아닙니다.");roundedEdgeObserved=true;}if(x>1.4f){Assert.That(player.TryReadCompletedSupport(new SimulationTick(tick),platform,source.CurrentReceipt.Value),Is.False);left=true;break;}}Assert.That(left,Is.True);Assert.That(roundedEdgeObserved,Is.True);
            var second=CreatePlayer(new Vector2(20,.81f));var secondSource=new SupportInputSource();var secondFloor=CreateBox("SecondFloor",new Vector2(20,-.5f),new Vector2(4,1));
            var ceiling=CreateBox("SecondCeiling",new Vector2(20,2.5f),new Vector2(4,1));second.RegisterCoordinatedInputOwner(secondSource,secondSource,new SimulationTick(0));SupportStep(second,secondSource,Command(0));
            Assert.That(second.TryReadCompletedSupport(new SimulationTick(0),platform,zero),Is.False);
            var hitCeiling=false;for(var tick=1;tick<40;tick++){SupportStep(second,secondSource,Command(tick,tick==1));Assert.That(second.TryReadCompletedSupport(new SimulationTick(tick),ceiling,secondSource.CurrentReceipt.Value),Is.False);if(tick>1 && second.Snapshot.VelocityYQ4096==0 && !second.Snapshot.IsGrounded){hitCeiling=true;break;}}Assert.That(hitCeiling,Is.True);
            // A failed advance clears the latest proof and preserves the original coordinated-receipt error.
            Assert.Throws<InvalidOperationException>(()=>player.AdvanceOneFixedTickForTests());Assert.That(player.TryReadCompletedSupport(source.CurrentReceipt.Value.Tick,platform,source.CurrentReceipt.Value),Is.False);
        }
        private void Step(PlayerMovementController player)
        {
            player.AdvanceOneFixedTickForTests();
            Physics2D.Simulate(1f / 60f);
            AssertBodyMatchesSnapshot(player);
        }

        private PlayerMovementController CreatePlayer(Vector2 position)
        {
            var gameObject = Track(new GameObject("MovementSandboxPlayer"));
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

        private BoxCollider2D CreateKinematicBox(string name, Vector2 position, Vector2 size)
        {
            var gameObject = Track(new GameObject(name));
            gameObject.transform.position = position;
            var body = gameObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.simulated = true;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.interpolation = RigidbodyInterpolation2D.None;
            body.useFullKinematicContacts = false;
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = size;
            Physics2D.SyncTransforms();
            return collider;
        }

        private static void AssertRawCastContains(Collider2D source, Vector2 direction, float distance, Collider2D expected)
        {
            var filter = new ContactFilter2D();
            filter.NoFilter();
            filter.useTriggers = false;
            var hits = new RaycastHit2D[32];
            var count = source.Cast(direction, filter, hits, distance);
            Assert.That(count, Is.LessThan(hits.Length), "The direct raw-cast oracle must not saturate.");
            for (var index = 0; index < count; index++)
            {
                if (hits[index].collider == expected) return;
            }

            Assert.Fail("Unity raw Cast must retain the exact ignored pair so the production filter is exercised.");
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

        private static MovementCommand Command(int tick, bool jumpPressed = false, bool dashPressed = false) =>
            new MovementCommand(new SimulationTick(tick), 0, 0, jumpPressed, false, dashPressed);

        private static void AssertBodyMatchesSnapshot(PlayerMovementController controller)
        {
            var body = controller.GetComponent<Rigidbody2D>();
            Assert.That(Quantize(body.position.x), Is.EqualTo(controller.Snapshot.PositionXQ4096));
            Assert.That(Quantize(body.position.y), Is.EqualTo(controller.Snapshot.PositionYQ4096));
        }

        private static void AssertNotOverlapped(PlayerMovementController controller, Collider2D obstacle)
        {
            var playerCollider = controller.GetComponent<Collider2D>();
            var distance = playerCollider.Distance(obstacle);
            Assert.That(
                distance.isOverlapped,
                Is.False,
                $"distance={distance.distance:R}, playerBody={controller.GetComponent<Rigidbody2D>().position}, " +
                $"playerTransform={controller.transform.position}, obstacle={obstacle.bounds}, " +
                $"snapshot=({controller.Snapshot.PositionXQ4096},{controller.Snapshot.PositionYQ4096})");
        }

        private static int Quantize(float value) => checked((int)Math.Round(value * 4096d, MidpointRounding.AwayFromZero));

        private static void AssertSnapshotEqual(PlayerMotionSnapshot expected, PlayerMotionSnapshot actual)
        {
            Assert.That(actual.Tick, Is.EqualTo(expected.Tick));
            Assert.That(actual.PositionXQ4096, Is.EqualTo(expected.PositionXQ4096));
            Assert.That(actual.PositionYQ4096, Is.EqualTo(expected.PositionYQ4096));
            Assert.That(actual.VelocityXQ4096, Is.EqualTo(expected.VelocityXQ4096));
            Assert.That(actual.VelocityYQ4096, Is.EqualTo(expected.VelocityYQ4096));
            Assert.That(actual.IsGrounded, Is.EqualTo(expected.IsGrounded));
            Assert.That(actual.WallId, Is.EqualTo(expected.WallId));
            Assert.That(actual.WallSide, Is.EqualTo(expected.WallSide));
            Assert.That(actual.ActionState, Is.EqualTo(expected.ActionState));
            Assert.That(actual.IsAirDashAvailable, Is.EqualTo(expected.IsAirDashAvailable));
        }
    }
}
