using AcadeGameMaker.Core;

namespace AcadeGameMaker.Input.Unity
{
    internal enum InputRouterFaultStage
    {
        None = 0, Initialization, Preflight, PrepareMovement, PrepareTransfer,
        PrepareCombat, ValidateMovement, ValidateTransfer, ValidateCombat,
        CommitMovement, CommitTransfer, CommitCombat, MapSwitch, Disabled,
        AfterMovementCommit, AfterTransferCommit, AfterCombatCommit, MapsDisabled, TargetMapEnabled,
        TransitionRequired,
        // REQ-M5D7PA-006. Semantic UI capture/publication corruption is a
        // terminal router boundary; it must not be normalized into input.
        UiCapture,
        // REQ-M5D7Q0-008. Appended so every historical fault value remains
        // numerically stable; HubUIOnly graph/lifecycle containment is distinct
        // from semantic callback/publication corruption.
        HubUiOnly
    }

    internal enum InputCameraDisposition
    {
        NoAim = 0, BootstrapPending, CameraUnavailable, CameraRejected, Valid
    }

    // AC-M5B5-005/007. A bounded immutable observation, rather than an event log.
    internal readonly struct InputRouterDiagnostic
    {
        internal InputRouterDiagnostic(InputMode mode, SimulationTick tick, InputRouterFaultStage fault,
            InputCameraDisposition camera, bool gameplayMapEnabled, bool uiMapEnabled,
            bool unboundGameplay, bool unboundUi)
        {
            Mode = mode; Tick = tick; Fault = fault; Camera = camera; GameplayMapEnabled = gameplayMapEnabled;
            UiMapEnabled = uiMapEnabled; UnboundGameplay = unboundGameplay; UnboundUi = unboundUi;
        }
        internal InputMode Mode { get; }
        internal SimulationTick Tick { get; }
        internal InputRouterFaultStage Fault { get; }
        internal InputCameraDisposition Camera { get; }
        internal bool GameplayMapEnabled { get; }
        internal bool UiMapEnabled { get; }
        internal bool UnboundGameplay { get; }
        internal bool UnboundUi { get; }
    }
}
