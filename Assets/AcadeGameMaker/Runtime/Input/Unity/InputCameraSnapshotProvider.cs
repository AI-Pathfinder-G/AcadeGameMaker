using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-UX-013. The router reads completed camera state through this bound,
    // presentation-only seam; it never asks a camera to synthesize player state.
    internal interface IInputCameraSnapshotProvider
    {
        PlayerMovementController BoundPlayer { get; }
        bool TryGetCompletedCamera(SimulationTick expectedPoseTick, out SimulationCameraPoseSnapshot camera);
    }
}
