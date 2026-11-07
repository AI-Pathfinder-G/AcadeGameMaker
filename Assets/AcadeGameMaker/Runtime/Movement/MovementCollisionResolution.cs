using AcadeGameMaker.Core;

namespace AcadeGameMaker.Movement
{
    // REQ-MOV-001~004, REQ-MOV-006~010. Internal M2 transaction payload; never a public contract.
    internal readonly struct MovementPrediction
    {
        internal MovementPrediction(
            int startPositionXQ4096,
            int startPositionYQ4096,
            int startVelocityXQ4096,
            int startVelocityYQ4096,
            PlayerMotionSnapshot snapshot,
            bool isScheduledDash)
        {
            StartPositionXQ4096 = startPositionXQ4096;
            StartPositionYQ4096 = startPositionYQ4096;
            StartVelocityXQ4096 = startVelocityXQ4096;
            StartVelocityYQ4096 = startVelocityYQ4096;
            Snapshot = snapshot;
            IsScheduledDash = isScheduledDash;
        }

        internal SimulationTick Tick => Snapshot.Tick;
        internal int StartPositionXQ4096 { get; }
        internal int StartPositionYQ4096 { get; }
        internal int StartVelocityXQ4096 { get; }
        internal int StartVelocityYQ4096 { get; }
        internal int PredictedPositionXQ4096 => Snapshot.PositionXQ4096;
        internal int PredictedPositionYQ4096 => Snapshot.PositionYQ4096;
        internal int PredictedVelocityXQ4096 => Snapshot.VelocityXQ4096;
        internal int PredictedVelocityYQ4096 => Snapshot.VelocityYQ4096;
        internal MovementActionState ActionState => Snapshot.ActionState;
        internal PlayerMotionSnapshot Snapshot { get; }
        internal bool IsScheduledDash { get; }
    }

    // REQ-MOV-001, REQ-MOV-002, REQ-MOV-005. The Unity adapter may only correct position and block axes.
    internal readonly struct MovementCollisionResolution
    {
        internal MovementCollisionResolution(
            SimulationTick tick,
            int positionXQ4096,
            int positionYQ4096,
            bool isXBlocked,
            bool isYBlocked)
        {
            Tick = tick;
            PositionXQ4096 = positionXQ4096;
            PositionYQ4096 = positionYQ4096;
            IsXBlocked = isXBlocked;
            IsYBlocked = isYBlocked;
        }

        internal SimulationTick Tick { get; }
        internal int PositionXQ4096 { get; }
        internal int PositionYQ4096 { get; }
        internal bool IsXBlocked { get; }
        internal bool IsYBlocked { get; }

        internal static MovementCollisionResolution Identity(MovementPrediction prediction) =>
            new MovementCollisionResolution(
                prediction.Snapshot.Tick,
                prediction.Snapshot.PositionXQ4096,
                prediction.Snapshot.PositionYQ4096,
                false,
                false);
    }
}
