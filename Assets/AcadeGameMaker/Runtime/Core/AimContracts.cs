using System;

namespace AcadeGameMaker.Core
{
    // REQ-WT-006, REQ-WT-008. Shared immutable inputs for future VD-07 publication.
    public enum AimSource { MousePointer = 0, GamepadStick = 1 }

    public readonly struct SimulationCameraPoseSnapshot
    {
        public SimulationCameraPoseSnapshot(SimulationTick cameraPoseTick, int positionXQ100, int positionYQ100, int orthoSizeQ1000, int rotationQ10, int viewportWidth, int viewportHeight, int gameplayRectX, int gameplayRectY, int gameplayRectWidth, int gameplayRectHeight, int integerScale)
        {
            if (orthoSizeQ1000 <= 0 || rotationQ10 < 0 || rotationQ10 > 3599 || viewportWidth < 640 || viewportHeight < 360) throw new ArgumentOutOfRangeException();
            var scale = Math.Min(viewportWidth / 640, viewportHeight / 360); if (integerScale != scale || gameplayRectWidth != checked(640 * scale) || gameplayRectHeight != checked(360 * scale) || gameplayRectX != (viewportWidth - gameplayRectWidth) / 2 || gameplayRectY != (viewportHeight - gameplayRectHeight) / 2) throw new ArgumentException("Gameplay rectangle must be the exact centered integer scale.");
            CameraPoseTick = cameraPoseTick; PositionXQ100 = positionXQ100; PositionYQ100 = positionYQ100; OrthoSizeQ1000 = orthoSizeQ1000; RotationQ10 = rotationQ10;
            ViewportWidth = viewportWidth; ViewportHeight = viewportHeight; GameplayRectX = gameplayRectX; GameplayRectY = gameplayRectY; GameplayRectWidth = gameplayRectWidth; GameplayRectHeight = gameplayRectHeight; IntegerScale = integerScale;
        }
        public SimulationTick CameraPoseTick { get; } public int PositionXQ100 { get; } public int PositionYQ100 { get; } public int OrthoSizeQ1000 { get; } public int RotationQ10 { get; }
        public int ViewportWidth { get; } public int ViewportHeight { get; } public int GameplayRectX { get; } public int GameplayRectY { get; } public int GameplayRectWidth { get; } public int GameplayRectHeight { get; } public int IntegerScale { get; }
    }

    public readonly struct AimSample
    {
        public AimSample(long sampleId, AimSource source, int? screenPixelX, int? screenPixelY, bool? isPointerInsideGameplayRect, int aimVectorXQ4096, int aimVectorYQ4096, SimulationTick cameraPoseTick, SimulationTick sampleTick)
        {
            if (sampleId < 0 || (source != AimSource.MousePointer && source != AimSource.GamepadStick)) throw new ArgumentOutOfRangeException();
            var mouse = source == AimSource.MousePointer;
            if (mouse != (screenPixelX.HasValue && screenPixelY.HasValue && isPointerInsideGameplayRect.HasValue)) throw new ArgumentException("Mouse screen fields are coupled to AimSource.");
            if (!mouse && (screenPixelX.HasValue || screenPixelY.HasValue || isPointerInsideGameplayRect.HasValue)) throw new ArgumentException("Gamepad has no pointer fields.");
            if (mouse && (screenPixelX.Value < 0 || screenPixelX.Value > 1919 || screenPixelY.Value < 0 || screenPixelY.Value > 1079)) throw new ArgumentOutOfRangeException();
            if (cameraPoseTick.Value != checked(sampleTick.Value - 1)) throw new ArgumentException("Camera pose must precede sample by one tick.");
            AimVectorXQ4096 = Clamp(aimVectorXQ4096); AimVectorYQ4096 = Clamp(aimVectorYQ4096);
            if (AimVectorXQ4096 == 0 && AimVectorYQ4096 == 0) throw new ArgumentException("Aim vector cannot be zero.");
            SampleId = sampleId; Source = source; ScreenPixelX = screenPixelX; ScreenPixelY = screenPixelY; IsPointerInsideGameplayRect = isPointerInsideGameplayRect; CameraPoseTick = cameraPoseTick; SampleTick = sampleTick;
        }
        public long SampleId { get; } public AimSource Source { get; } public int? ScreenPixelX { get; } public int? ScreenPixelY { get; } public bool? IsPointerInsideGameplayRect { get; } public int AimVectorXQ4096 { get; } public int AimVectorYQ4096 { get; } public SimulationTick CameraPoseTick { get; } public SimulationTick SampleTick { get; }
        private static int Clamp(int value) => value < -4096 ? -4096 : value > 4096 ? 4096 : value;
    }

    public readonly struct TransferPressed
    {
        public TransferPressed(long? aimSampleId, SimulationTick tick) { if (aimSampleId.HasValue && aimSampleId.Value < 0) throw new ArgumentOutOfRangeException(nameof(aimSampleId)); AimSampleId = aimSampleId; Tick = tick; }
        public long? AimSampleId { get; } public SimulationTick Tick { get; }
    }
}
