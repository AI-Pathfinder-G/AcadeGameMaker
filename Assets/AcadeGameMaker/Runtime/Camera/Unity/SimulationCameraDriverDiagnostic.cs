using AcadeGameMaker.Core;
namespace AcadeGameMaker.Camera.Unity
{
    internal enum CameraDriverDisposition { BootstrapPending, Available, OutputUnavailable, Faulted }
    internal enum CameraDriverFaultStage { BeforeCoreAdvance, AfterCoreAdvance, BeforeTransformWrite, BeforePublication }
    internal readonly struct SimulationCameraDriverDiagnostic
    {
        internal SimulationCameraDriverDiagnostic(SimulationTick tick, CameraDriverDisposition disposition, int width, int height, int scale, int x, int y, int rectWidth, int rectHeight, bool hasHistorical)
        { Tick=tick; Disposition=disposition; OutputWidth=width; OutputHeight=height; IntegerScale=scale; GameplayRectX=x; GameplayRectY=y; GameplayRectWidth=rectWidth; GameplayRectHeight=rectHeight; HasHistoricalPublication=hasHistorical; }
        internal SimulationTick Tick { get; } internal CameraDriverDisposition Disposition { get; } internal int OutputWidth { get; } internal int OutputHeight { get; } internal int IntegerScale { get; } internal int GameplayRectX { get; } internal int GameplayRectY { get; } internal int GameplayRectWidth { get; } internal int GameplayRectHeight { get; } internal bool HasHistoricalPublication { get; }
    }
}
