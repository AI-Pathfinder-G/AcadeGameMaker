using System;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Movement
{
    // REQ-MOV-001~004, REQ-MOV-006~010. Pure, single-tick owner of movement state.
    internal sealed class PlayerMovementMotor
    {
        // REQ-MOV-001, REQ-MOV-005; AC-M4B3B3-002. This opaque candidate is
        // intentionally creatable only by this motor's mutation-free preflight.
        internal sealed class ExternalMovementDeltaCandidate
        {
            private readonly PlayerMovementMotor _owner;
            private readonly ExternalMovementDirective _directive;
            private readonly int _positionXQ4096;
            private readonly int _positionYQ4096;

            internal ExternalMovementDeltaCandidate(
                PlayerMovementMotor owner,
                ExternalMovementDirective directive,
                int positionXQ4096,
                int positionYQ4096,
                ExternalMovementDelta delta)
            {
                _owner = owner;
                _directive = directive;
                _positionXQ4096 = positionXQ4096;
                _positionYQ4096 = positionYQ4096;
                Delta = delta;
            }

            internal ExternalMovementDelta Delta { get; }

            internal bool Matches(PlayerMovementMotor owner, SimulationTick tick, int positionXQ4096, int positionYQ4096) =>
                ReferenceEquals(_owner, owner) &&
                _directive.MovementTick == tick &&
                _positionXQ4096 == positionXQ4096 &&
                _positionYQ4096 == positionYQ4096;
        }

        // REQ-MOV-001, REQ-MOV-005; AC-M4B3B3-002. All arithmetic is checked,
        // uses a ceil integer square root, and rounds components toward zero.
        internal ExternalMovementDeltaCandidate PreflightExternalMovementDirective(ExternalMovementDirective directive)
        {
            if (directive.MovementTick != NextExpectedTick)
            {
                throw new InvalidOperationException("The external movement directive does not target the next motor tick.");
            }

            var delta = CalculateExternalDelta(_positionX, _positionY, directive);
            // The checked Int32 addition is part of preflight, not a later fallback.
            _ = checked(_positionX + delta.XQ4096);
            _ = checked(_positionY + delta.YQ4096);
            return new ExternalMovementDeltaCandidate(this, directive, _positionX, _positionY, delta);
        }

        internal static ExternalMovementDelta CalculateExternalDelta(
            int positionXQ4096,
            int positionYQ4096,
            ExternalMovementDirective directive)
        {
            var dx = checked((long)directive.AnchorXQ4096 - positionXQ4096);
            var dy = checked((long)directive.AnchorYQ4096 - positionYQ4096);
            var distanceSquared = checked(checked(dx * dx) + checked(dy * dy));
            var step = (long)directive.MaximumStepQ4096;
            var stepSquared = checked(step * step);

            if (distanceSquared < 0)
            {
                throw new OverflowException("External movement distance squared cannot be negative.");
            }

            if (distanceSquared == 0)
            {
                return new ExternalMovementDelta(0, 0);
            }

            if (distanceSquared <= stepSquared)
            {
                return VerifyRadialPostcondition(
                    checked((int)dx),
                    checked((int)dy),
                    stepSquared);
            }

            var distanceCeil = CeilIntegerSqrt(distanceSquared);
            var deltaX = ScaleTowardZero(dx, step, distanceCeil);
            var deltaY = ScaleTowardZero(dy, step, distanceCeil);
            return VerifyRadialPostcondition(deltaX, deltaY, stepSquared);
        }

        private static long CeilIntegerSqrt(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            var floor = IntegerSqrt(value);
            return checked(floor * floor) == value ? floor : checked(floor + 1);
        }

        private static long IntegerSqrt(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            var low = 0L;
            var high = Math.Min(value, 3037000499L);
            while (low < high)
            {
                var middle = low + (high - low + 1) / 2;
                if (middle <= value / middle)
                {
                    low = middle;
                }
                else
                {
                    high = middle - 1;
                }
            }

            return low;
        }

        private static int ScaleTowardZero(long component, long step, long distanceCeil)
        {
            if (distanceCeil <= 0 || step <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(distanceCeil));
            }

            var absoluteComponent = component < 0 ? checked(-component) : component;
            var magnitude = checked(absoluteComponent * step) / distanceCeil;
            var signed = component < 0 ? checked(-magnitude) : magnitude;
            return checked((int)signed);
        }

        private static ExternalMovementDelta VerifyRadialPostcondition(int deltaX, int deltaY, long stepSquared)
        {
            var resultSquared = checked(checked((long)deltaX * deltaX) + checked((long)deltaY * deltaY));
            if (resultSquared > stepSquared)
            {
                throw new InvalidOperationException("External movement delta exceeds its authored radial step.");
            }

            return new ExternalMovementDelta(deltaX, deltaY);
        }
        private int _expectedTick;
        private int _positionX;
        private int _positionY;
        private int _velocityX;
        private int _velocityY;
        private long _positionXRemainder;
        private long _positionYRemainder;
        private long _horizontalRemainder;
        private long _gravityRemainder;
        private bool _wasGrounded;
        private int _coyoteAge = -1;
        private int _jumpBufferAge = -1;
        private bool _jumpReleaseApplied;
        private bool _jumpActive;
        private bool _airDashAvailable = true;
        private bool _dashActive;
        private int _dashTicks;
        private int _dashDirection = 1;
        private int _dashCooldownAge = -1;
        private int _lastFacing = 1;
        private string _lockedWallId = string.Empty;
        private int _wallLockAge = -1;
        private bool _inputLocked;
        private PlayerMovementModifierKind _modifier;
        private PlayerMotionSnapshot _snapshot;
        private bool _hasPublishedTick;
        private bool _hasPendingPrediction;
        private PlayerMotionSnapshot _pendingSnapshot;
        private bool _pendingScheduledDash;
        private int _pendingStartPositionX;
        private int _pendingStartPositionY;
        private int _pendingStartVelocityX;
        private int _pendingStartVelocityY;

        internal PlayerMovementMotor(SimulationTick firstTick)
        {
            _expectedTick = firstTick.Value;
            _modifier = PlayerMovementModifierKind.Baseline;
            _snapshot = CreateSnapshot(firstTick, false, string.Empty, WallSide.None, MovementActionState.Normal);
        }

        internal PlayerMotionSnapshot Snapshot => _snapshot;
        internal SimulationTick NextExpectedTick => new SimulationTick(_expectedTick);

        internal PlayerMotionSnapshot Advance(
            MovementCommand command,
            MovementContacts contacts,
            PlayerMovementModifierKind modifier,
            bool inputLocked)
        {
            var prediction = BeginTick(command, contacts, modifier, inputLocked);
            return CommitTick(MovementCollisionResolution.Identity(prediction));
        }

        // REQ-MOV-001, REQ-MOV-002, REQ-MOV-005. Phase one stages, but never publishes, one tick.
        internal MovementPrediction BeginTick(
            MovementCommand command,
            MovementContacts contacts,
            PlayerMovementModifierKind modifier,
            bool inputLocked)
        {
            return BeginTick(command, contacts, modifier, inputLocked, null);
        }

        // REQ-MOV-001, REQ-MOV-005; AC-M4B3B3-004. The preflighted external
        // displacement is composed exactly once after ordinary motion state work.
        internal MovementPrediction BeginTick(
            MovementCommand command,
            MovementContacts contacts,
            PlayerMovementModifierKind modifier,
            bool inputLocked,
            ExternalMovementDeltaCandidate externalMovement)
        {
            if (_hasPendingPrediction)
            {
                throw new InvalidOperationException("A collision resolution is required before another movement prediction.");
            }

            if (command.Tick.Value != _expectedTick)
            {
                throw new InvalidOperationException(
                    $"Expected SimulationTick {_expectedTick}, received {command.Tick.Value}.");
            }

            if (externalMovement != null &&
                !externalMovement.Matches(this, command.Tick, _positionX, _positionY))
            {
                throw new InvalidOperationException("The external movement candidate is stale, cross-wired, or forged.");
            }

            _pendingStartPositionX = _positionX;
            _pendingStartPositionY = _positionY;
            _pendingStartVelocityX = _velocityX;
            _pendingStartVelocityY = _velocityY;
            _expectedTick = checked(_expectedTick + 1);
            if (_modifier != modifier)
            {
                _modifier = modifier;
                _gravityRemainder = 0;
                _horizontalRemainder = 0;
            }

            UpdateGroundingAndTimers(contacts);
            if (inputLocked)
            {
                if (!_inputLocked)
                {
                    CancelDashForLock();
                    ClearBufferedCommandsAndWallAttachment();
                    _inputLocked = true;
                }

                ApplyLockedPhysics(contacts);
                return StagePrediction(command.Tick, contacts.IsGrounded, string.Empty, WallSide.None, MovementActionState.InputLocked, externalMovement);
            }

            _inputLocked = false;
            if (_dashActive)
            {
                return AdvanceDash(command.Tick, contacts, externalMovement);
            }

            AgeDashCooldown();
            var hasMeaningfulMove = HasMeaningfulMove(command);
            if (hasMeaningfulMove && command.MoveXQ4096 != 0)
            {
                _lastFacing = command.MoveXQ4096 < 0 ? -1 : 1;
            }

            if (command.JumpReleased)
            {
                ApplyJumpRelease();
            }

            if (command.DashPressed && CanStartDash(contacts))
            {
                BeginDash(command, contacts);
                return AdvanceDash(command.Tick, contacts, externalMovement);
            }

            var effectiveWall = GetEffectiveWall(contacts);
            var action = MovementActionState.Normal;
            var startedJump = false;
            if (!contacts.IsGrounded && command.JumpPressed && effectiveWall.WallSide != WallSide.None)
            {
                StartWallJump(effectiveWall);
                FixedQ4096.Integrate(ref _positionX, _velocityX, ref _positionXRemainder);
                FixedQ4096.Integrate(ref _positionY, _velocityY, ref _positionYRemainder);
                return StagePrediction(command.Tick, false, string.Empty, WallSide.None, MovementActionState.WallJumping, externalMovement);
            }

            if (CanStartStandardJump(command, contacts))
            {
                StartStandardJump();
                startedJump = true;
            }
            else if (command.JumpPressed)
            {
                _jumpBufferAge = 0;
            }

            ApplyHorizontalMovement(command.MoveXQ4096, command.MoveYQ4096, contacts.IsGrounded);
            if (startedJump)
            {
                ApplyVerticalPhysics(false, effectiveWall.WallSide != WallSide.None);
            }
            else
            {
                ApplyVerticalPhysics(contacts.IsGrounded, effectiveWall.WallSide != WallSide.None);
            }

            if (!contacts.IsGrounded && effectiveWall.WallSide != WallSide.None && _velocityY < 0)
            {
                if (_velocityY < -FixedQ4096.WallSlideFallSpeed)
                {
                    _velocityY = -FixedQ4096.WallSlideFallSpeed;
                    _gravityRemainder = 0;
                }

                action = MovementActionState.WallSliding;
            }

            return StagePrediction(
                command.Tick,
                startedJump ? false : contacts.IsGrounded,
                effectiveWall.WallId,
                effectiveWall.WallSide,
                action,
                externalMovement);
        }

        // REQ-MOV-001, REQ-MOV-002, REQ-MOV-005. Phase two publishes the authoritative resolved state.
        internal PlayerMotionSnapshot CommitTick(MovementCollisionResolution resolution)
        {
            if (!_hasPendingPrediction)
            {
                throw new InvalidOperationException("No movement prediction is awaiting collision resolution.");
            }

            if (resolution.Tick != _pendingSnapshot.Tick)
            {
                throw new InvalidOperationException(
                    $"Resolution tick {resolution.Tick.Value} does not match predicted tick {_pendingSnapshot.Tick.Value}.");
            }

            if (_positionX != resolution.PositionXQ4096 || resolution.IsXBlocked)
            {
                _positionX = resolution.PositionXQ4096;
                _positionXRemainder = 0;
            }

            if (_positionY != resolution.PositionYQ4096 || resolution.IsYBlocked)
            {
                _positionY = resolution.PositionYQ4096;
                _positionYRemainder = 0;
            }

            if (resolution.IsXBlocked && !_pendingScheduledDash)
            {
                _velocityX = 0;
                _horizontalRemainder = 0;
            }

            if (resolution.IsYBlocked)
            {
                _velocityY = 0;
                _gravityRemainder = 0;
            }

            _snapshot = new PlayerMotionSnapshot(
                _pendingSnapshot.Tick,
                _positionX,
                _positionY,
                _velocityX,
                _velocityY,
                _pendingSnapshot.IsGrounded,
                _pendingSnapshot.WallId,
                _pendingSnapshot.WallSide,
                _pendingSnapshot.ActionState,
                _pendingSnapshot.IsAirDashAvailable);
            _hasPendingPrediction = false;
            _hasPublishedTick = true;
            return _snapshot;
        }

        // REQ-MOV-002. Adapter-only query; facing remains solely owned by the motor.
        internal int GetDashDirectionForCommand(MovementCommand command)
        {
            if (_hasPendingPrediction || command.Tick.Value != _expectedTick)
            {
                throw new InvalidOperationException("Dash direction may be queried only for the next unpredicted tick.");
            }

            if (_dashActive)
            {
                return _dashDirection;
            }

            return HasMeaningfulMove(command) && command.MoveXQ4096 != 0
                ? command.MoveXQ4096 < 0 ? -1 : 1
                : _lastFacing;
        }

        // REQ-MOV-002. A blocked flag is harmless for a rejected dash request and is consumed only by a scheduled dash.
        internal bool IsDashScheduledOrRequested(MovementCommand command)
        {
            if (_hasPendingPrediction || command.Tick.Value != _expectedTick)
            {
                throw new InvalidOperationException("Dash schedule may be queried only for the next unpredicted tick.");
            }

            return _dashActive || command.DashPressed;
        }

        // REQ-MOV-001. Controller seeds the first authoritative Q4096 position from its kinematic body.
        internal void SeedAuthoritativePosition(int positionXQ4096, int positionYQ4096)
        {
            if (_hasPublishedTick || _hasPendingPrediction)
            {
                throw new InvalidOperationException("The authoritative position can be seeded only before the first tick.");
            }

            _positionX = positionXQ4096;
            _positionY = positionYQ4096;
            _positionXRemainder = 0;
            _positionYRemainder = 0;
            _snapshot = CreateSnapshot(_snapshot.Tick, false, string.Empty, WallSide.None, MovementActionState.Normal);
        }

        internal void ResetForLifecycle()
        {
            CancelDashForLock();
            ClearBufferedCommandsAndWallAttachment();
            _wasGrounded = false;
            _inputLocked = false;
        }

        private void UpdateGroundingAndTimers(MovementContacts contacts)
        {
            if (contacts.IsGrounded)
            {
                _coyoteAge = 0;
                _airDashAvailable = true;
                _jumpActive = false;
                _jumpReleaseApplied = false;
                if (_velocityY < 0)
                {
                    _velocityY = 0;
                    _gravityRemainder = 0;
                }
            }
            else if (_wasGrounded)
            {
                _coyoteAge = 0;
            }
            else if (_coyoteAge >= 0)
            {
                _coyoteAge = checked(_coyoteAge + 1);
            }

            if (_jumpBufferAge >= 0)
            {
                _jumpBufferAge = checked(_jumpBufferAge + 1);
                if (_jumpBufferAge > 6)
                {
                    _jumpBufferAge = -1;
                }
            }

            if (_wallLockAge >= 0)
            {
                _wallLockAge = checked(_wallLockAge + 1);
            }

            _wasGrounded = contacts.IsGrounded;
        }

        private void ApplyLockedPhysics(MovementContacts contacts)
        {
            ApplyHorizontalMovement(0, 0, contacts.IsGrounded);
            if (contacts.IsGrounded)
            {
                _velocityY = 0;
                _gravityRemainder = 0;
                return;
            }

            ApplyVerticalPhysics(false, GetEffectiveWall(contacts).WallSide != WallSide.None);
        }

        private MovementPrediction AdvanceDash(
            SimulationTick tick,
            MovementContacts contacts,
            ExternalMovementDeltaCandidate externalMovement)
        {
            _velocityX = checked(_dashDirection * FixedQ4096.DashVelocity);
            _velocityY = 0;
            if (!contacts.IsDashPathBlocked)
            {
                FixedQ4096.Integrate(ref _positionX, _velocityX, ref _positionXRemainder);
            }

            _dashTicks = checked(_dashTicks + 1);
            if (_dashTicks == 15)
            {
                _dashActive = false;
                _dashCooldownAge = 0;
            }

            return StagePrediction(tick, contacts.IsGrounded, string.Empty, WallSide.None, MovementActionState.Dashing, externalMovement);
        }

        private void AgeDashCooldown()
        {
            if (_dashCooldownAge >= 0)
            {
                _dashCooldownAge = checked(_dashCooldownAge + 1);
            }
        }

        private bool CanStartDash(MovementContacts contacts)
        {
            if (_dashCooldownAge >= 0 && _dashCooldownAge < 36)
            {
                return false;
            }

            return contacts.IsGrounded || _airDashAvailable;
        }

        private void BeginDash(MovementCommand command, MovementContacts contacts)
        {
            _dashActive = true;
            _dashTicks = 0;
            _dashDirection = HasMeaningfulMove(command) && command.MoveXQ4096 != 0
                ? command.MoveXQ4096 < 0 ? -1 : 1
                : _lastFacing;
            _dashCooldownAge = -1;
            _jumpActive = false;
            _jumpReleaseApplied = false;
            _velocityY = 0;
            _gravityRemainder = 0;
            if (!contacts.IsGrounded)
            {
                _airDashAvailable = false;
            }
        }

        private bool CanStartStandardJump(MovementCommand command, MovementContacts contacts)
        {
            if (!HasBufferedOrPressedJump(command))
            {
                return false;
            }

            return contacts.IsGrounded || (_coyoteAge >= 0 && _coyoteAge <= 6);
        }

        private bool HasBufferedOrPressedJump(MovementCommand command)
        {
            return command.JumpPressed || (_jumpBufferAge >= 0 && _jumpBufferAge <= 6);
        }

        private void StartStandardJump()
        {
            _velocityY = FixedQ4096.JumpVelocity;
            _gravityRemainder = 0;
            _jumpActive = true;
            _jumpReleaseApplied = false;
            _jumpBufferAge = -1;
            _coyoteAge = -1;
            _wasGrounded = false;
        }

        private void StartWallJump(MovementContacts wall)
        {
            _velocityX = wall.WallSide == WallSide.Left ? FixedQ4096.WallJumpX : -FixedQ4096.WallJumpX;
            _velocityY = FixedQ4096.WallJumpY;
            _horizontalRemainder = 0;
            _gravityRemainder = 0;
            _jumpActive = true;
            _jumpReleaseApplied = false;
            _jumpBufferAge = -1;
            _coyoteAge = -1;
            _lockedWallId = wall.WallId;
            _wallLockAge = 0;
        }

        private void ApplyJumpRelease()
        {
            if (_jumpActive && !_jumpReleaseApplied && _velocityY > 0)
            {
                _velocityY = FixedQ4096.DivideRoundAwayFromZero(checked((long)_velocityY * 45), 100);
                _jumpReleaseApplied = true;
            }
        }

        private void ApplyHorizontalMovement(int moveXQ4096, int moveYQ4096, bool grounded)
        {
            var target = HasMeaningfulMove(moveXQ4096, moveYQ4096)
                ? FixedQ4096.DivideRoundAwayFromZero(checked((long)FixedQ4096.RunSpeed * moveXQ4096), FixedQ4096.Units)
                : 0;
            var accelerating = target != 0 && Math.Sign(target) != Math.Sign(_velocityX) || Math.Abs(target) > Math.Abs(_velocityX);
            var rate = grounded
                ? accelerating ? FixedQ4096.GroundAcceleration : FixedQ4096.GroundDeceleration
                : accelerating
                    ? _modifier == PlayerMovementModifierKind.Lightweight
                        ? FixedQ4096.LightweightAirAcceleration
                        : FixedQ4096.AirAcceleration
                    : FixedQ4096.AirDeceleration;
            var direction = target == _velocityX ? 0 : target > _velocityX ? 1 : -1;
            if (direction != 0)
            {
                FixedQ4096.ApplyPerTick(ref _velocityX, checked(direction * rate), ref _horizontalRemainder, 1, 1);
                if ((direction > 0 && _velocityX > target) || (direction < 0 && _velocityX < target))
                {
                    _velocityX = target;
                    _horizontalRemainder = 0;
                }
            }

            FixedQ4096.Integrate(ref _positionX, _velocityX, ref _positionXRemainder);
        }

        private void ApplyVerticalPhysics(bool grounded, bool clampForWallSlide)
        {
            if (grounded)
            {
                _velocityY = 0;
                _gravityRemainder = 0;
                return;
            }

            var multiplierNumerator = _modifier == PlayerMovementModifierKind.Lightweight ? 65 : 100;
            FixedQ4096.ApplyPerTick(
                ref _velocityY,
                -FixedQ4096.Gravity,
                ref _gravityRemainder,
                multiplierNumerator,
                100);
            var maximumFall = _modifier == PlayerMovementModifierKind.Lightweight
                ? FixedQ4096.LightweightMaxFallSpeed
                : FixedQ4096.MaxFallSpeed;
            if (_velocityY < -maximumFall)
            {
                _velocityY = -maximumFall;
                _gravityRemainder = 0;
            }

            if (clampForWallSlide && _velocityY < -FixedQ4096.WallSlideFallSpeed)
            {
                _velocityY = -FixedQ4096.WallSlideFallSpeed;
                _gravityRemainder = 0;
            }

            if (_velocityY <= 0)
            {
                _jumpActive = false;
            }

            FixedQ4096.Integrate(ref _positionY, _velocityY, ref _positionYRemainder);
        }

        private MovementContacts GetEffectiveWall(MovementContacts contacts)
        {
            if (contacts.WallSide == WallSide.None ||
                (string.Equals(contacts.WallId, _lockedWallId, StringComparison.Ordinal) && _wallLockAge >= 0 && _wallLockAge < 6))
            {
                return new MovementContacts(contacts.IsGrounded, string.Empty, WallSide.None, contacts.IsDashPathBlocked);
            }

            return contacts;
        }

        private void CancelDashForLock()
        {
            if (!_dashActive)
            {
                return;
            }

            _dashActive = false;
            _dashTicks = 0;
            _dashCooldownAge = 0;
            _velocityX = 0;
            _positionXRemainder = 0;
        }

        private void ClearBufferedCommandsAndWallAttachment()
        {
            _jumpBufferAge = -1;
            _coyoteAge = -1;
            _lockedWallId = string.Empty;
            _wallLockAge = -1;
        }

        private static bool HasMeaningfulMove(MovementCommand command) =>
            HasMeaningfulMove(command.MoveXQ4096, command.MoveYQ4096);

        private static bool HasMeaningfulMove(int moveXQ4096, int moveYQ4096)
        {
            var magnitudeSquared = checked((long)moveXQ4096 * moveXQ4096 +
                                         (long)moveYQ4096 * moveYQ4096);
            return magnitudeSquared >= (long)FixedQ4096.Deadzone * FixedQ4096.Deadzone;
        }

        private MovementPrediction StagePrediction(
            SimulationTick tick,
            bool grounded,
            string wallId,
            WallSide wallSide,
            MovementActionState action,
            ExternalMovementDeltaCandidate externalMovement)
        {
            if (externalMovement != null)
            {
                if (!externalMovement.Matches(this, tick, _pendingStartPositionX, _pendingStartPositionY))
                {
                    throw new InvalidOperationException("The external movement candidate is stale, cross-wired, or forged.");
                }

                // The normal command/modifier/jump/dash/gravity state above remains untouched.
                _positionX = checked(_positionX + externalMovement.Delta.XQ4096);
                _positionY = checked(_positionY + externalMovement.Delta.YQ4096);
            }

            _pendingSnapshot = CreateSnapshot(tick, grounded, wallId, wallSide, action);
            _pendingScheduledDash = action == MovementActionState.Dashing;
            _hasPendingPrediction = true;
            return new MovementPrediction(
                _pendingStartPositionX,
                _pendingStartPositionY,
                _pendingStartVelocityX,
                _pendingStartVelocityY,
                _pendingSnapshot,
                _pendingScheduledDash);
        }

        private PlayerMotionSnapshot CreateSnapshot(
            SimulationTick tick,
            bool grounded,
            string wallId,
            WallSide wallSide,
            MovementActionState action) =>
            new PlayerMotionSnapshot(
                tick,
                _positionX,
                _positionY,
                _velocityX,
                _velocityY,
                grounded,
                wallId,
                wallSide,
                action,
                _airDashAvailable);
    }
}
