// AC-MOV-001, AC-MOV-002, AC-MOV-004, AC-MOV-005, AC-MOV-006.
using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Movement
{
    internal sealed class PlayerMovementMotorTests
    {
        private static readonly MovementContacts Ground = new MovementContacts(true, string.Empty, WallSide.None, false);
        private static readonly MovementContacts Air = new MovementContacts(false, string.Empty, WallSide.None, false);

        [Test]
        public void SequentialTicksFailFastAndDoNotAdvanceState()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            var first = motor.Advance(Command(0), Ground, PlayerMovementModifierKind.Baseline, false);

            Assert.Throws<InvalidOperationException>(() => motor.Advance(Command(0), Ground, PlayerMovementModifierKind.Baseline, false));
            AssertSnapshotEqual(first, motor.Snapshot);
            Assert.That(motor.Advance(Command(1), Ground, PlayerMovementModifierKind.Baseline, false).Tick.Value, Is.EqualTo(1));

            var skipped = new PlayerMovementMotor(new SimulationTick(0));
            skipped.Advance(Command(0), Ground, PlayerMovementModifierKind.Baseline, false);
            var beforeSkipped = skipped.Snapshot;
            Assert.Throws<InvalidOperationException>(() => skipped.Advance(Command(2), Ground, PlayerMovementModifierKind.Baseline, false));
            AssertSnapshotEqual(beforeSkipped, skipped.Snapshot);
            Assert.That(skipped.Advance(Command(1), Ground, PlayerMovementModifierKind.Baseline, false).Tick.Value, Is.EqualTo(1));

            var reversed = new PlayerMovementMotor(new SimulationTick(0));
            reversed.Advance(Command(0), Ground, PlayerMovementModifierKind.Baseline, false);
            reversed.Advance(Command(1), Ground, PlayerMovementModifierKind.Baseline, false);
            var beforeReversed = reversed.Snapshot;
            Assert.Throws<InvalidOperationException>(() => reversed.Advance(Command(0), Ground, PlayerMovementModifierKind.Baseline, false));
            AssertSnapshotEqual(beforeReversed, reversed.Snapshot);
            Assert.That(reversed.Advance(Command(2), Ground, PlayerMovementModifierKind.Baseline, false).Tick.Value, Is.EqualTo(2));
        }

        [Test]
        public void CoyoteAndJumpBufferAcceptAgeSixAndRejectAgeSeven()
        {
            var coyoteAtSix = new PlayerMovementMotor(new SimulationTick(0));
            coyoteAtSix.Advance(Command(0), Ground, PlayerMovementModifierKind.Baseline, false);
            for (var tick = 1; tick <= 6; tick++)
            {
                coyoteAtSix.Advance(Command(tick), Air, PlayerMovementModifierKind.Baseline, false);
            }

            Assert.That(coyoteAtSix.Advance(Command(7, jumpPressed: true), Air, PlayerMovementModifierKind.Baseline, false).VelocityYQ4096,
                Is.EqualTo(56844));

            var coyoteAtSeven = new PlayerMovementMotor(new SimulationTick(0));
            coyoteAtSeven.Advance(Command(0), Ground, PlayerMovementModifierKind.Baseline, false);
            for (var tick = 1; tick <= 7; tick++)
            {
                coyoteAtSeven.Advance(Command(tick), Air, PlayerMovementModifierKind.Baseline, false);
            }

            Assert.That(coyoteAtSeven.Advance(Command(8, jumpPressed: true), Air, PlayerMovementModifierKind.Baseline, false).VelocityYQ4096,
                Is.LessThan(FixedQ4096.JumpVelocity));

            var bufferAtSix = new PlayerMovementMotor(new SimulationTick(0));
            bufferAtSix.Advance(Command(0, jumpPressed: true), Air, PlayerMovementModifierKind.Baseline, false);
            for (var tick = 1; tick <= 5; tick++)
            {
                bufferAtSix.Advance(Command(tick), Air, PlayerMovementModifierKind.Baseline, false);
            }

            Assert.That(bufferAtSix.Advance(Command(6), Ground, PlayerMovementModifierKind.Baseline, false).VelocityYQ4096,
                Is.EqualTo(56844));

            var bufferAtSeven = new PlayerMovementMotor(new SimulationTick(0));
            bufferAtSeven.Advance(Command(0, jumpPressed: true), Air, PlayerMovementModifierKind.Baseline, false);
            for (var tick = 1; tick <= 6; tick++)
            {
                bufferAtSeven.Advance(Command(tick), Air, PlayerMovementModifierKind.Baseline, false);
            }

            Assert.That(bufferAtSeven.Advance(Command(7), Ground, PlayerMovementModifierKind.Baseline, false).VelocityYQ4096,
                Is.EqualTo(0));
        }

        [Test]
        public void GroundJumpDoesNotCreateASecondCoyoteJump()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            var first = motor.Advance(Command(0, jumpPressed: true), Ground, PlayerMovementModifierKind.Baseline, false);
            Assert.That(first.IsGrounded, Is.False);
            Assert.That(FixedQ4096.JumpVelocity, Is.EqualTo(58941));
            Assert.That(first.VelocityYQ4096, Is.EqualTo(56844));
            Assert.That(first.PositionYQ4096, Is.GreaterThan(0));

            for (var tick = 1; tick <= 6; tick++)
            {
                motor.Advance(Command(tick), Air, PlayerMovementModifierKind.Baseline, false);
            }

            Assert.That(motor.Advance(Command(7, jumpPressed: true), Air, PlayerMovementModifierKind.Baseline, false).VelocityYQ4096,
                Is.Not.EqualTo(FixedQ4096.JumpVelocity));
        }

        [Test]
        public void DashKeepsFifteenTickScheduleAndCooldownStartsAfterIt()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            PlayerMotionSnapshot snapshot = default;
            for (var tick = 0; tick < 15; tick++)
            {
                snapshot = motor.Advance(Command(tick, moveX: 4096, dashPressed: tick == 0), Ground, PlayerMovementModifierKind.Baseline, false);
                Assert.That(snapshot.ActionState, Is.EqualTo(MovementActionState.Dashing));
            }

            Assert.That(snapshot.PositionXQ4096, Is.EqualTo(5 * FixedQ4096.Units));
            for (var tick = 15; tick <= 49; tick++)
            {
                snapshot = motor.Advance(Command(tick, dashPressed: tick == 49), Ground, PlayerMovementModifierKind.Baseline, false);
            }

            Assert.That(snapshot.ActionState, Is.Not.EqualTo(MovementActionState.Dashing));
            Assert.That(motor.Advance(Command(50, dashPressed: true), Ground, PlayerMovementModifierKind.Baseline, false).ActionState,
                Is.EqualTo(MovementActionState.Dashing));
        }

        [Test]
        public void DashObstructionStopsOnlyDisplacement()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            var blocked = new MovementContacts(true, string.Empty, WallSide.None, true);
            PlayerMotionSnapshot snapshot = default;
            for (var tick = 0; tick < 15; tick++)
            {
                snapshot = motor.Advance(Command(tick, dashPressed: tick == 0), blocked, PlayerMovementModifierKind.Baseline, false);
            }

            Assert.That(snapshot.PositionXQ4096, Is.EqualTo(0));
            Assert.That(snapshot.ActionState, Is.EqualTo(MovementActionState.Dashing));
            Assert.That(motor.Advance(Command(15, dashPressed: true), Ground, PlayerMovementModifierKind.Baseline, false).ActionState,
                Is.Not.EqualTo(MovementActionState.Dashing));
        }

        [Test]
        public void WallReattachBlocksAgesOneThroughFiveAndAllowsAgeSix()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            var rightWall = new MovementContacts(false, "authored-wall-r01", WallSide.Right, false);
            var jump = motor.Advance(Command(0, jumpPressed: true), rightWall, PlayerMovementModifierKind.Baseline, false);
            Assert.That(jump.ActionState, Is.EqualTo(MovementActionState.WallJumping));
            Assert.That(jump.VelocityXQ4096, Is.EqualTo(-FixedQ4096.WallJumpX));
            Assert.That(jump.VelocityYQ4096, Is.EqualTo(FixedQ4096.WallJumpY));

            for (var tick = 1; tick <= 5; tick++)
            {
                var locked = motor.Advance(Command(tick), rightWall, PlayerMovementModifierKind.Baseline, false);
                Assert.That(locked.WallSide, Is.EqualTo(WallSide.None));
                Assert.That(locked.WallId, Is.Empty);
            }

            var eligible = motor.Advance(Command(6), rightWall, PlayerMovementModifierKind.Baseline, false);
            Assert.That(eligible.WallSide, Is.EqualTo(WallSide.Right));
            Assert.That(eligible.WallId, Is.EqualTo("authored-wall-r01"));
        }

        [Test]
        public void AirborneWallJumpTakesPriorityOverCoyoteJump()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            motor.Advance(Command(0), Ground, PlayerMovementModifierKind.Baseline, false);
            var wall = new MovementContacts(false, "authored-wall-r02", WallSide.Left, false);

            var jump = motor.Advance(Command(1, jumpPressed: true), wall, PlayerMovementModifierKind.Baseline, false);
            Assert.That(jump.ActionState, Is.EqualTo(MovementActionState.WallJumping));
            Assert.That(jump.VelocityXQ4096, Is.EqualTo(FixedQ4096.WallJumpX));
            Assert.That(jump.VelocityYQ4096, Is.EqualTo(FixedQ4096.WallJumpY));
        }

        [Test]
        public void LightweightUsesApprovedConstantsAndRestoresBaselineDashValues()
        {
            Assert.That(FixedQ4096.Gravity, Is.EqualTo(125829));
            Assert.That(FixedQ4096.LightweightAirAcceleration, Is.EqualTo(204800));
            Assert.That(FixedQ4096.LightweightMaxFallSpeed, Is.EqualTo(63078));

            var baseline = RunDash(PlayerMovementModifierKind.Baseline);
            var lightweight = RunDash(PlayerMovementModifierKind.Lightweight);
            Assert.That(baseline.PositionXQ4096, Is.EqualTo(lightweight.PositionXQ4096));
            Assert.That(baseline.VelocityXQ4096, Is.EqualTo(lightweight.VelocityXQ4096));

            var motor = new PlayerMovementMotor(new SimulationTick(0));
            motor.Advance(Command(0, jumpPressed: true), Ground, PlayerMovementModifierKind.Baseline, false);
            var light = motor.Advance(Command(1), Air, PlayerMovementModifierKind.Lightweight, false);
            var restored = motor.Advance(Command(2), Air, PlayerMovementModifierKind.Baseline, false);
            Assert.That(light.VelocityYQ4096, Is.GreaterThan(restored.VelocityYQ4096));
            Assert.That(restored.VelocityYQ4096, Is.LessThan(FixedQ4096.JumpVelocity));
        }

        [Test]
        public void JumpReleaseAppliesPointFourFiveExactlyOnce()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            var beforeRelease = motor.Advance(Command(0, jumpPressed: true), Ground, PlayerMovementModifierKind.Baseline, false);
            Assert.That(beforeRelease.VelocityYQ4096, Is.EqualTo(56844));
            Assert.That(FixedQ4096.DivideRoundAwayFromZero(checked((long)beforeRelease.VelocityYQ4096 * 45), 100),
                Is.EqualTo(25580));

            var released = motor.Advance(Command(1, jumpReleased: true), Air, PlayerMovementModifierKind.Baseline, false);
            var repeated = motor.Advance(Command(2, jumpReleased: true), Air, PlayerMovementModifierKind.Baseline, false);
            Assert.That(released.VelocityYQ4096, Is.EqualTo(23483));
            Assert.That(repeated.VelocityYQ4096 - released.VelocityYQ4096, Is.InRange(-2098, -2097));
        }

        [Test]
        public void AirDashConsumesOnceAndValidatedGroundRestoresIt()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            var first = motor.Advance(Command(0, dashPressed: true), Air, PlayerMovementModifierKind.Baseline, false);
            Assert.That(first.ActionState, Is.EqualTo(MovementActionState.Dashing));
            Assert.That(first.IsAirDashAvailable, Is.False);
            for (var tick = 1; tick < 15; tick++)
            {
                motor.Advance(Command(tick), Air, PlayerMovementModifierKind.Baseline, false);
            }

            for (var tick = 15; tick <= 50; tick++)
            {
                motor.Advance(Command(tick, dashPressed: tick == 50), Air, PlayerMovementModifierKind.Baseline, false);
            }

            Assert.That(motor.Snapshot.ActionState, Is.Not.EqualTo(MovementActionState.Dashing));
            var restored = motor.Advance(Command(51), Ground, PlayerMovementModifierKind.Baseline, false);
            Assert.That(restored.IsAirDashAvailable, Is.True);
            Assert.That(motor.Advance(Command(52, dashPressed: true), Air, PlayerMovementModifierKind.Baseline, false).ActionState,
                Is.EqualTo(MovementActionState.Dashing));
        }

        [Test]
        public void WallJumpImpulseIsInvariantForLightweight()
        {
            var wall = new MovementContacts(false, "authored-wall-r03", WallSide.Left, false);
            var baseline = new PlayerMovementMotor(new SimulationTick(0))
                .Advance(Command(0, jumpPressed: true), wall, PlayerMovementModifierKind.Baseline, false);
            var lightweight = new PlayerMovementMotor(new SimulationTick(0))
                .Advance(Command(0, jumpPressed: true), wall, PlayerMovementModifierKind.Lightweight, false);

            Assert.That(baseline.VelocityXQ4096, Is.EqualTo(FixedQ4096.WallJumpX));
            Assert.That(baseline.VelocityYQ4096, Is.EqualTo(FixedQ4096.WallJumpY));
            Assert.That(lightweight.VelocityXQ4096, Is.EqualTo(baseline.VelocityXQ4096));
            Assert.That(lightweight.VelocityYQ4096, Is.EqualTo(baseline.VelocityYQ4096));
        }

        [Test]
        public void BaselineNumericMovementMeetsRunJumpApexAndFallCap()
        {
            var running = new PlayerMovementMotor(new SimulationTick(0));
            for (var tick = 0; tick < 60; tick++)
            {
                running.Advance(Command(tick, moveX: 4096), Ground, PlayerMovementModifierKind.Baseline, false);
            }

            Assert.That(running.Snapshot.VelocityXQ4096, Is.EqualTo(FixedQ4096.RunSpeed));
            Assert.That(FixedQ4096.JumpVelocity, Is.EqualTo(58941));
            var baselineJump = MeasureJump(PlayerMovementModifierKind.Baseline);
            Assert.That(baselineJump.PeakPositionQ4096, Is.InRange(13312 - 205, 13312 + 205));
            Assert.That(baselineJump.ApexTicks, Is.InRange(27, 29));
            Assert.That(MeasureFallCap(PlayerMovementModifierKind.Baseline), Is.EqualTo(-FixedQ4096.MaxFallSpeed));
        }

        [Test]
        public void LightweightNumericMovementMeetsApexAirAccelerationAndFallCap()
        {
            var lightweightJump = MeasureJump(PlayerMovementModifierKind.Lightweight);
            Assert.That(lightweightJump.PeakPositionQ4096, Is.InRange(20480 - 287, 20480 + 287));
            Assert.That(lightweightJump.ApexTicks, Is.InRange(42, 44));
            Assert.That(MeasureFallCap(PlayerMovementModifierKind.Lightweight), Is.EqualTo(-FixedQ4096.LightweightMaxFallSpeed));

            var motor = new PlayerMovementMotor(new SimulationTick(0));
            var firstAirTick = motor.Advance(Command(0, moveX: 4096), Air, PlayerMovementModifierKind.Lightweight, false);
            Assert.That(firstAirTick.VelocityXQ4096, Is.EqualTo(3413));
        }

        [Test]
        public void RadialDeadzoneRejectsBelowAndAcceptsAtQuantizedBoundary()
        {
            var below = new PlayerMovementMotor(new SimulationTick(0))
                .Advance(Command(0, moveX: 818), Air, PlayerMovementModifierKind.Baseline, false);
            var at = new PlayerMovementMotor(new SimulationTick(0))
                .Advance(Command(0, moveX: FixedQ4096.Deadzone), Air, PlayerMovementModifierKind.Baseline, false);

            Assert.That(below.VelocityXQ4096, Is.EqualTo(0));
            Assert.That(at.VelocityXQ4096, Is.GreaterThan(0));
        }

        [Test]
        public void WallSlideCapAppliesBeforeVerticalPositionIntegration()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            PlayerMotionSnapshot beforeWall = default;
            for (var tick = 0; tick < 30; tick++)
            {
                beforeWall = motor.Advance(Command(tick), Air, PlayerMovementModifierKind.Baseline, false);
            }

            var wall = new MovementContacts(false, "authored-wall-r04", WallSide.Right, false);
            var sliding = motor.Advance(Command(30), wall, PlayerMovementModifierKind.Baseline, false);
            Assert.That(sliding.ActionState, Is.EqualTo(MovementActionState.WallSliding));
            Assert.That(sliding.VelocityYQ4096, Is.EqualTo(-FixedQ4096.WallSlideFallSpeed));
            Assert.That(sliding.PositionYQ4096 - beforeWall.PositionYQ4096, Is.InRange(-410, -409));
        }

        [Test]
        public void InputLockCancelsDashAndLifecycleClearsStaleEdges()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            motor.Advance(Command(0, dashPressed: true), Ground, PlayerMovementModifierKind.Baseline, false);
            var locked = motor.Advance(Command(1, dashPressed: true, jumpPressed: true), Air, PlayerMovementModifierKind.Baseline, true);
            Assert.That(locked.ActionState, Is.EqualTo(MovementActionState.InputLocked));
            Assert.That(locked.VelocityXQ4096, Is.EqualTo(0));

            motor.ResetForLifecycle();
            var resumed = motor.Advance(Command(2), Air, PlayerMovementModifierKind.Baseline, false);
            Assert.That(resumed.ActionState, Is.EqualTo(MovementActionState.Normal));
            Assert.That(resumed.WallId, Is.Empty);
        }

        [Test]
        public void LifecycleResetCannotRecreateCoyoteFromPriorGround()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            motor.Advance(Command(0), Ground, PlayerMovementModifierKind.Baseline, false);
            motor.ResetForLifecycle();

            var postResetAirJump = motor.Advance(Command(1, jumpPressed: true), Air, PlayerMovementModifierKind.Baseline, false);
            Assert.That(postResetAirJump.VelocityYQ4096, Is.LessThan(0));
            Assert.That(postResetAirJump.ActionState, Is.EqualTo(MovementActionState.Normal));
        }

        [Test]
        public void InputLockWithoutDashPreservesPhysicsAndDoesNotCreateCooldown()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            var moving = motor.Advance(Command(0, moveX: 4096), Ground, PlayerMovementModifierKind.Baseline, false);
            var locked = motor.Advance(Command(1), Ground, PlayerMovementModifierKind.Baseline, true);
            Assert.That(locked.PositionXQ4096, Is.GreaterThanOrEqualTo(moving.PositionXQ4096));
            Assert.That(locked.VelocityXQ4096, Is.EqualTo(0));

            var dash = motor.Advance(Command(2, dashPressed: true), Ground, PlayerMovementModifierKind.Baseline, false);
            Assert.That(dash.ActionState, Is.EqualTo(MovementActionState.Dashing));
        }

        [Test]
        public void ContactsRejectUndefinedWallSide()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MovementContacts(false, "bad-wall", (WallSide)7, false));
        }

        // AC-M4B3B3-002. The pull delta is exact Q4096: ceil-sqrt avoids the
        // (410,1) radial overrun and component division always truncates to zero.
        [Test]
        public void BalanceAuditPullUsesCeilSqrtTowardZeroAndRadialPostcondition()
        {
            var nearAxis = new ExternalMovementDirective(new SimulationTick(0), "axis", 410, 1, 410);
            var nearAxisDelta = PlayerMovementMotor.CalculateExternalDelta(0, 0, nearAxis);
            Assert.That(nearAxisDelta.XQ4096, Is.EqualTo(409));
            Assert.That(nearAxisDelta.YQ4096, Is.EqualTo(0));

            var diagonal = new ExternalMovementDirective(new SimulationTick(0), "diagonal", 600, 600, 410);
            var diagonalDelta = PlayerMovementMotor.CalculateExternalDelta(0, 0, diagonal);
            Assert.That(diagonalDelta.XQ4096, Is.EqualTo(289));
            Assert.That(diagonalDelta.YQ4096, Is.EqualTo(289));
            Assert.That(
                (long)diagonalDelta.XQ4096 * diagonalDelta.XQ4096 + (long)diagonalDelta.YQ4096 * diagonalDelta.YQ4096,
                Is.LessThanOrEqualTo(410L * 410L));

            var mirrored = PlayerMovementMotor.CalculateExternalDelta(
                0,
                0,
                new ExternalMovementDirective(new SimulationTick(0), "mirrored", -600, -600, 410));
            Assert.That(mirrored.XQ4096, Is.EqualTo(-diagonalDelta.XQ4096));
            Assert.That(mirrored.YQ4096, Is.EqualTo(-diagonalDelta.YQ4096));

            var exactAnchor = PlayerMovementMotor.CalculateExternalDelta(
                28672,
                4915,
                new ExternalMovementDirective(new SimulationTick(0), "zero", 28672, 4915, 410));
            Assert.That(exactAnchor.XQ4096, Is.Zero);
            Assert.That(exactAnchor.YQ4096, Is.Zero);
            Assert.Throws<OverflowException>(() => PlayerMovementMotor.CalculateExternalDelta(
                int.MinValue,
                0,
                new ExternalMovementDirective(new SimulationTick(0), "overflow", int.MaxValue, 0, 410)));
        }

        // AC-M4B3B3-003, AC-M4B3B3-004. A preflighted delta is consumed once at
        // the motor seam after ordinary motion and cannot be reused on a later tick.
        [Test]
        public void BalanceAuditPullCandidateComposesOnceAndRejectsStaleReuse()
        {
            var ordinary = new PlayerMovementMotor(new SimulationTick(0));
            var pulled = new PlayerMovementMotor(new SimulationTick(0));
            var directive = new ExternalMovementDirective(new SimulationTick(0), "compose", 600, 600, 410);
            var candidate = pulled.PreflightExternalMovementDirective(directive);
            var ordinarySnapshot = ordinary.Advance(Command(0, moveX: 4096), Air, PlayerMovementModifierKind.Baseline, false);
            var prediction = pulled.BeginTick(Command(0, moveX: 4096), Air, PlayerMovementModifierKind.Baseline, false, candidate);
            var pulledSnapshot = pulled.CommitTick(MovementCollisionResolution.Identity(prediction));

            Assert.That(pulledSnapshot.PositionXQ4096 - ordinarySnapshot.PositionXQ4096, Is.EqualTo(candidate.Delta.XQ4096));
            Assert.That(pulledSnapshot.PositionYQ4096 - ordinarySnapshot.PositionYQ4096, Is.EqualTo(candidate.Delta.YQ4096));
            Assert.That(pulledSnapshot.VelocityXQ4096, Is.EqualTo(ordinarySnapshot.VelocityXQ4096));
            Assert.That(pulledSnapshot.VelocityYQ4096, Is.EqualTo(ordinarySnapshot.VelocityYQ4096));

            var stale = new PlayerMovementMotor(new SimulationTick(0));
            var staleCandidate = stale.PreflightExternalMovementDirective(directive);
            stale.Advance(Command(0), Air, PlayerMovementModifierKind.Baseline, false);
            var beforeRejectedReuse = stale.Snapshot;
            Assert.Throws<InvalidOperationException>(() =>
                stale.BeginTick(Command(1), Air, PlayerMovementModifierKind.Baseline, false, staleCandidate));
            AssertSnapshotEqual(beforeRejectedReuse, stale.Snapshot);
        }

        // AC-M4B3B3-004, AC-M4B3B3-006. Every protected motion branch computes
        // its ordinary state first; pull changes position only, including while
        // input is locked and under the Lightweight gravity profile.
        [TestCase("neutral")]
        [TestCase("move")]
        [TestCase("jump")]
        [TestCase("release")]
        [TestCase("dash")]
        [TestCase("wall-slide")]
        [TestCase("wall-jump")]
        [TestCase("input-locked")]
        [TestCase("lightweight")]
        public void BalanceAuditPullComposesAfterProtectedMotionBranches(string scenario)
        {
            var ordinary = new PlayerMovementMotor(new SimulationTick(0));
            var pulled = new PlayerMovementMotor(new SimulationTick(0));
            var tick = 0;
            var contacts = Air;
            var command = Command(0);
            var modifier = PlayerMovementModifierKind.Baseline;
            var inputLocked = false;

            if (scenario == "release")
            {
                ordinary.Advance(Command(0, jumpPressed: true), Ground, modifier, false);
                pulled.Advance(Command(0, jumpPressed: true), Ground, modifier, false);
                tick = 1;
                command = Command(1, jumpReleased: true);
            }
            else if (scenario == "move") command = Command(0, moveX: 4096);
            else if (scenario == "jump") { contacts = Ground; command = Command(0, jumpPressed: true); }
            else if (scenario == "dash") { contacts = Ground; command = Command(0, dashPressed: true); }
            else if (scenario == "wall-slide") contacts = new MovementContacts(false, "wall", WallSide.Right, false);
            else if (scenario == "wall-jump")
            {
                contacts = new MovementContacts(false, "wall", WallSide.Right, false);
                command = Command(0, jumpPressed: true);
            }
            else if (scenario == "input-locked")
            {
                command = Command(0, moveX: 4096, jumpPressed: true, dashPressed: true);
                inputLocked = true;
            }
            else if (scenario == "lightweight") modifier = PlayerMovementModifierKind.Lightweight;

            var directive = new ExternalMovementDirective(new SimulationTick(tick), "branch-" + scenario, 600, 600, 410);
            var candidate = pulled.PreflightExternalMovementDirective(directive);
            var ordinarySnapshot = ordinary.Advance(command, contacts, modifier, inputLocked);
            var prediction = pulled.BeginTick(command, contacts, modifier, inputLocked, candidate);
            var pulledSnapshot = pulled.CommitTick(MovementCollisionResolution.Identity(prediction));

            Assert.That(pulledSnapshot.PositionXQ4096 - ordinarySnapshot.PositionXQ4096, Is.EqualTo(candidate.Delta.XQ4096));
            Assert.That(pulledSnapshot.PositionYQ4096 - ordinarySnapshot.PositionYQ4096, Is.EqualTo(candidate.Delta.YQ4096));
            AssertNonPositionStateEqual(ordinarySnapshot, pulledSnapshot);

            var nextContacts = scenario == "wall-slide" || scenario == "wall-jump" ? contacts : Air;
            var nextOrdinary = ordinary.Advance(Command(tick + 1), nextContacts, modifier, false);
            var nextPulled = pulled.Advance(Command(tick + 1), nextContacts, modifier, false);
            AssertNonPositionStateEqual(nextOrdinary, nextPulled);
        }

        [Test]
        public void BalanceAuditPullRejectsCoordinateOverflowWithoutPublishingSnapshot()
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            motor.SeedAuthoritativePosition(int.MaxValue - 5, 0);
            var before = motor.Snapshot;
            var directive = new ExternalMovementDirective(new SimulationTick(0), "coordinate-overflow", int.MaxValue, 0, 410);
            var candidate = motor.PreflightExternalMovementDirective(directive);

            Assert.Throws<OverflowException>(() => motor.BeginTick(
                Command(0, moveX: 4096), Air, PlayerMovementModifierKind.Baseline, false, candidate));
            AssertSnapshotEqual(before, motor.Snapshot);

            var squareOverflow = new PlayerMovementMotor(new SimulationTick(0));
            squareOverflow.SeedAuthoritativePosition(int.MaxValue - 5, 0);
            var beforeSquareOverflow = squareOverflow.Snapshot;
            Assert.Throws<OverflowException>(() => squareOverflow.PreflightExternalMovementDirective(
                new ExternalMovementDirective(new SimulationTick(0), "square-overflow", int.MinValue, int.MinValue, 410)));
            AssertSnapshotEqual(beforeSquareOverflow, squareOverflow.Snapshot);
        }

        [Test]
        public void ThreeIdenticalReplaysProduceTickByTickEqualSnapshots()
        {
            var commands = new List<MovementCommand>();
            var contacts = new List<MovementContacts>();
            for (var tick = 0; tick < 90; tick++)
            {
                commands.Add(Command(
                    tick,
                    moveX: tick < 30 ? 4096 : 0,
                    jumpPressed: tick == 2,
                    jumpReleased: tick == 10,
                    dashPressed: tick == 45));
                contacts.Add(tick < 2 || tick >= 70 ? Ground : Air);
            }

            var first = Replay(commands, contacts);
            var second = Replay(commands, contacts);
            var third = Replay(commands, contacts);
            for (var index = 0; index < first.Count; index++)
            {
                AssertSnapshotEqual(first[index], second[index]);
                AssertSnapshotEqual(first[index], third[index]);
            }
        }

        private static PlayerMotionSnapshot RunDash(PlayerMovementModifierKind modifier)
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            PlayerMotionSnapshot result = default;
            for (var tick = 0; tick < 15; tick++)
            {
                result = motor.Advance(Command(tick, moveX: 4096, dashPressed: tick == 0), Ground, modifier, false);
            }

            return result;
        }

        private static List<PlayerMotionSnapshot> Replay(IReadOnlyList<MovementCommand> commands, IReadOnlyList<MovementContacts> contacts)
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            var results = new List<PlayerMotionSnapshot>();
            for (var index = 0; index < commands.Count; index++)
            {
                results.Add(motor.Advance(commands[index], contacts[index], PlayerMovementModifierKind.Baseline, false));
            }

            return results;
        }

        private static (int PeakPositionQ4096, int ApexTicks) MeasureJump(PlayerMovementModifierKind modifier)
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            var snapshot = motor.Advance(Command(0, jumpPressed: true), Ground, modifier, false);
            var peak = snapshot.PositionYQ4096;
            for (var tick = 1; tick <= 120; tick++)
            {
                snapshot = motor.Advance(Command(tick), Air, modifier, false);
                peak = Math.Max(peak, snapshot.PositionYQ4096);
                if (snapshot.VelocityYQ4096 <= 0)
                {
                    return (peak, tick + 1);
                }
            }

            throw new AssertionException("Jump did not reach an apex.");
        }

        private static int MeasureFallCap(PlayerMovementModifierKind modifier)
        {
            var motor = new PlayerMovementMotor(new SimulationTick(0));
            for (var tick = 0; tick < 200; tick++)
            {
                motor.Advance(Command(tick), Air, modifier, false);
            }

            return motor.Snapshot.VelocityYQ4096;
        }

        private static MovementCommand Command(
            int tick,
            int moveX = 0,
            int moveY = 0,
            bool jumpPressed = false,
            bool jumpReleased = false,
            bool dashPressed = false) =>
            new MovementCommand(new SimulationTick(tick), moveX, moveY, jumpPressed, jumpReleased, dashPressed);

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

        private static void AssertNonPositionStateEqual(PlayerMotionSnapshot expected, PlayerMotionSnapshot actual)
        {
            Assert.That(actual.Tick, Is.EqualTo(expected.Tick));
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
