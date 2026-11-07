using System;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Movement
{
    public readonly struct MovementCommand
    {
        public MovementCommand(
            SimulationTick tick,
            int moveXQ4096,
            int moveYQ4096,
            bool jumpPressed,
            bool jumpReleased,
            bool dashPressed)
        {
            Tick = tick;
            MoveXQ4096 = ClampAxis(moveXQ4096);
            MoveYQ4096 = ClampAxis(moveYQ4096);
            JumpPressed = jumpPressed;
            JumpReleased = jumpReleased;
            DashPressed = dashPressed;
        }

        public SimulationTick Tick { get; }
        public int MoveXQ4096 { get; }
        public int MoveYQ4096 { get; }
        public bool JumpPressed { get; }
        public bool JumpReleased { get; }
        public bool DashPressed { get; }

        private static int ClampAxis(int value) => value < -4096 ? -4096 : value > 4096 ? 4096 : value;
    }

    public enum WallSide
    {
        None = 0,
        Left = -1,
        Right = 1
    }

    public readonly struct MovementContacts
    {
        public MovementContacts(bool isGrounded, string wallId, WallSide wallSide, bool isDashPathBlocked)
        {
            wallId = wallId ?? string.Empty;
            if (wallSide != WallSide.None && wallSide != WallSide.Left && wallSide != WallSide.Right)
            {
                throw new ArgumentOutOfRangeException(nameof(wallSide));
            }

            if ((wallSide == WallSide.None) != (wallId.Length == 0))
            {
                throw new ArgumentException("WallId is empty exactly when WallSide is None.");
            }

            IsGrounded = isGrounded;
            WallId = wallId;
            WallSide = wallSide;
            IsDashPathBlocked = isDashPathBlocked;
        }

        public bool IsGrounded { get; }
        public string WallId { get; }
        public WallSide WallSide { get; }
        public bool IsDashPathBlocked { get; }
    }

    public enum PlayerMovementModifierKind
    {
        Baseline,
        Lightweight
    }

    public enum MovementActionState
    {
        Normal,
        Dashing,
        WallSliding,
        WallJumping,
        InputLocked
    }

    // REQ-MOV-001, REQ-MOV-005. Internal immutable authored displacement; Combat.Unity
    // submits primitive fields to the controller and never constructs this value itself.
    internal readonly struct ExternalMovementDirective
    {
        internal ExternalMovementDirective(
            SimulationTick movementTick,
            string directiveId,
            int anchorXQ4096,
            int anchorYQ4096,
            int maximumStepQ4096)
        {
            if (movementTick.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(movementTick));
            }

            if (string.IsNullOrEmpty(directiveId) || directiveId.Length > 96)
            {
                throw new ArgumentException("An external movement directive requires a stable ID of at most 96 characters.", nameof(directiveId));
            }

            for (var index = 0; index < directiveId.Length; index++)
            {
                if (directiveId[index] > 0x7f)
                {
                    throw new ArgumentException("An external movement directive ID must be ASCII.", nameof(directiveId));
                }
            }

            if (maximumStepQ4096 <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumStepQ4096));
            }

            MovementTick = movementTick;
            DirectiveId = directiveId;
            AnchorXQ4096 = anchorXQ4096;
            AnchorYQ4096 = anchorYQ4096;
            MaximumStepQ4096 = maximumStepQ4096;
        }

        internal SimulationTick MovementTick { get; }
        internal string DirectiveId { get; }
        internal int AnchorXQ4096 { get; }
        internal int AnchorYQ4096 { get; }
        internal int MaximumStepQ4096 { get; }
    }

    // REQ-MOV-001, REQ-MOV-005. Published only on a successful controller commit.
    internal readonly struct ExternalMovementDirectiveReceipt
    {
        internal ExternalMovementDirectiveReceipt(
            SimulationTick movementTick,
            bool hadDirective,
            string consumedDirectiveId,
            int appliedDeltaXQ4096,
            int appliedDeltaYQ4096)
        {
            if (!hadDirective && (!string.IsNullOrEmpty(consumedDirectiveId) || appliedDeltaXQ4096 != 0 || appliedDeltaYQ4096 != 0))
            {
                throw new ArgumentException("A no-directive receipt must have an empty ID and zero delta.");
            }

            if (hadDirective && string.IsNullOrEmpty(consumedDirectiveId))
            {
                throw new ArgumentException("A consumed directive receipt requires its stable ID.", nameof(consumedDirectiveId));
            }

            MovementTick = movementTick;
            HadDirective = hadDirective;
            ConsumedDirectiveId = consumedDirectiveId ?? string.Empty;
            AppliedDeltaXQ4096 = appliedDeltaXQ4096;
            AppliedDeltaYQ4096 = appliedDeltaYQ4096;
        }

        internal SimulationTick MovementTick { get; }
        internal bool HadDirective { get; }
        internal string ConsumedDirectiveId { get; }
        internal int AppliedDeltaXQ4096 { get; }
        internal int AppliedDeltaYQ4096 { get; }
    }
    internal readonly struct ExternalMovementDelta
    {
        internal ExternalMovementDelta(int xQ4096, int yQ4096)
        {
            XQ4096 = xQ4096;
            YQ4096 = yQ4096;
        }

        internal int XQ4096 { get; }
        internal int YQ4096 { get; }
    }

    public readonly struct PlayerMotionSnapshot
    {
        public PlayerMotionSnapshot(
            SimulationTick tick,
            int positionXQ4096,
            int positionYQ4096,
            int velocityXQ4096,
            int velocityYQ4096,
            bool isGrounded,
            string wallId,
            WallSide wallSide,
            MovementActionState actionState,
            bool isAirDashAvailable)
        {
            Tick = tick;
            PositionXQ4096 = positionXQ4096;
            PositionYQ4096 = positionYQ4096;
            VelocityXQ4096 = velocityXQ4096;
            VelocityYQ4096 = velocityYQ4096;
            IsGrounded = isGrounded;
            WallId = wallId ?? string.Empty;
            WallSide = wallSide;
            ActionState = actionState;
            IsAirDashAvailable = isAirDashAvailable;
        }

        public SimulationTick Tick { get; }
        public int PositionXQ4096 { get; }
        public int PositionYQ4096 { get; }
        public int VelocityXQ4096 { get; }
        public int VelocityYQ4096 { get; }
        public bool IsGrounded { get; }
        public string WallId { get; }
        public WallSide WallSide { get; }
        public MovementActionState ActionState { get; }
        public bool IsAirDashAvailable { get; }
    }
}
