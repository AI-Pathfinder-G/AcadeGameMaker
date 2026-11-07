using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AcadeGameMaker.Camera.Unity
{
    // REQ-ART-009/010/011, REQ-UX-013. +10 is the sole completed-camera publisher.
    [DefaultExecutionOrder(10)]
    [DisallowMultipleComponent]
    internal sealed class SimulationCameraDriver : MonoBehaviour, IInputCameraSnapshotProvider
    {
        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private PixelPerfectCamera _pixelPerfect;
        [SerializeField] private int _initialCenterXPixel18;
        [SerializeField] private int _initialCenterYPixel18;
        [SerializeField] private int _minXPixel18;
        [SerializeField] private int _maxXPixel18;
        [SerializeField] private int _minYPixel18;
        [SerializeField] private int _maxYPixel18;
        private SimulationCameraSession _core;
        private bool _initialized, _faulted, _available, _hasHistorical;
        private SimulationCameraPoseSnapshot _latest;
        private SimulationCameraDriverDiagnostic _diagnostic;
        private CameraDriverFaultStage? _testFault;
        private float _authoredZ;
        private PlayerMovementController _frozenPlayer;
        private UnityEngine.Camera _frozenCamera;
        private PixelPerfectCamera _frozenPixelPerfect;

        public PlayerMovementController BoundPlayer => _player;
        internal bool IsFaulted => _faulted;
        internal bool HasLatestPublication => _available;
        internal SimulationCameraPoseSnapshot LatestPublication => _available ? _latest : throw new InvalidOperationException("Camera publication unavailable.");
        internal SimulationCameraDriverDiagnostic LatestDiagnostic => _diagnostic;
        internal CameraFollowSnapshot LatestCoreSnapshot => _core == null ? throw new InvalidOperationException("Camera core unavailable.") : _core.LatestSnapshot;
        internal SimulationTick NextExpectedTick => _core == null ? throw new InvalidOperationException("Camera core unavailable.") : _core.NextExpectedTick;

        internal void ConfigureForAuthoring(PlayerMovementController player, UnityEngine.Camera camera, PixelPerfectCamera pixelPerfect,
            int initialCenterXPixel18, int initialCenterYPixel18, CameraCenterBounds bounds)
        {
            if (_initialized || _faulted) throw new InvalidOperationException("Camera driver cannot be reconfigured.");
            _player=player; _camera=camera; _pixelPerfect=pixelPerfect; _initialCenterXPixel18=initialCenterXPixel18; _initialCenterYPixel18=initialCenterYPixel18;
            _minXPixel18=bounds.MinXPixel18; _maxXPixel18=bounds.MaxXPixel18; _minYPixel18=bounds.MinYPixel18; _maxYPixel18=bounds.MaxYPixel18;
        }

        private void Start() { if (!_initialized) Initialize(); }
        internal void InitializeForTests() { if (_initialized || _faulted) throw new InvalidOperationException("Camera driver is already initialized or faulted."); Initialize(); }
        internal void AdvanceForTests() { if (!_initialized || _faulted) throw new InvalidOperationException("Camera driver is unavailable."); Advance(); }
        internal void SetFaultStageForTests(CameraDriverFaultStage? stage) => _testFault=stage;
        private void FixedUpdate() { if (_initialized && !_faulted) Advance(); }

        private void Initialize()
        {
            if (_initialized || _faulted) throw new InvalidOperationException("Camera driver has a closed initialization lifecycle.");
            try
            {
                ValidateProfile();
                var bounds=new CameraCenterBounds(_minXPixel18,_maxXPixel18,_minYPixel18,_maxYPixel18);
                if (_player.Snapshot.Tick != _player.NextExpectedTick) throw new InvalidOperationException("Late camera initialization cannot invent a completed pose.");
                var core=new SimulationCameraSession(_player.NextExpectedTick,_initialCenterXPixel18,_initialCenterYPixel18,bounds);
                var authoredZ=_camera.transform.position.z;
                WriteTransform(_initialCenterXPixel18,_initialCenterYPixel18);
                _frozenPlayer=_player; _frozenCamera=_camera; _frozenPixelPerfect=_pixelPerfect; _core=core; _authoredZ=authoredZ;
                _diagnostic=new SimulationCameraDriverDiagnostic(core.NextExpectedTick,CameraDriverDisposition.BootstrapPending,0,0,0,0,0,0,0,false);
                _initialized=true;
            }
            catch { LatchFault(); throw; }
        }

        private void Advance()
        {
            try { ValidateProfile(); }
            catch { LatchFault(); throw; }
            var snapshot=_player.Snapshot;
            // Exact phase rejection is not an engine failure: preserve all prior publication/state.
            if (snapshot.Tick != _core.NextExpectedTick || _player.NextExpectedTick.Value != checked(snapshot.Tick.Value+1))
                throw new InvalidOperationException("Camera requires one actual completed Movement tick.");
            try
            {
                var viewport=CaptureViewport(); CheckFault(CameraDriverFaultStage.BeforeCoreAdvance);
                var result=_core.Advance(snapshot); CheckFault(CameraDriverFaultStage.AfterCoreAdvance);
                if (!viewport.Supported) { _available=false; _diagnostic=new SimulationCameraDriverDiagnostic(result.Tick,CameraDriverDisposition.OutputUnavailable,viewport.Width,viewport.Height,0,0,0,0,0,_hasHistorical); return; }
                CheckFault(CameraDriverFaultStage.BeforeTransformWrite); WriteTransform(result.CenterXPixel18,result.CenterYPixel18);
                CheckFault(CameraDriverFaultStage.BeforePublication);
                _latest=new SimulationCameraPoseSnapshot(result.Tick,result.CenterXQ100,result.CenterYQ100,10000,0,viewport.Width,viewport.Height,viewport.X,viewport.Y,viewport.RectWidth,viewport.RectHeight,viewport.Scale);
                _available=true; _hasHistorical=true; _diagnostic=new SimulationCameraDriverDiagnostic(result.Tick,CameraDriverDisposition.Available,viewport.Width,viewport.Height,viewport.Scale,viewport.X,viewport.Y,viewport.RectWidth,viewport.RectHeight,true);
            }
            catch { LatchFault(); throw; }
        }

        public bool TryGetCompletedCamera(SimulationTick expectedPoseTick, out SimulationCameraPoseSnapshot camera)
        { if (!_faulted && _available && _latest.CameraPoseTick==expectedPoseTick) { camera=_latest; return true; } camera=default(SimulationCameraPoseSnapshot); return false; }
        private void OnDisable() { if (_initialized && !_faulted) LatchFault(); }
        private void OnEnable() { if (_initialized) throw new InvalidOperationException("Camera driver cannot re-enable."); }
        private void CheckFault(CameraDriverFaultStage stage) { if (_testFault.HasValue && _testFault.Value==stage) throw new InvalidOperationException("Injected camera driver fault."); }
        private void LatchFault() { _faulted=true; _available=false; var tick=_core==null?default(SimulationTick):_core.NextExpectedTick; _diagnostic=new SimulationCameraDriverDiagnostic(tick,CameraDriverDisposition.Faulted,0,0,0,0,0,0,0,_hasHistorical); }

        private void ValidateProfile()
        {
            var rotation=_camera==null?default(Quaternion):_camera.transform.rotation; var scale=_camera==null?default(Vector3):_camera.transform.lossyScale;
            if (_player==null||_camera==null||_pixelPerfect==null||(_initialized&&(!ReferenceEquals(_player,_frozenPlayer)||!ReferenceEquals(_camera,_frozenCamera)||!ReferenceEquals(_pixelPerfect,_frozenPixelPerfect)))||_camera.gameObject!=_pixelPerfect.gameObject||!_player.isActiveAndEnabled||!_camera.isActiveAndEnabled||!_pixelPerfect.isActiveAndEnabled||!_camera.orthographic||_camera.orthographicSize!=10f||_pixelPerfect.assetsPPU!=18||_pixelPerfect.refResolutionX!=640||_pixelPerfect.refResolutionY!=360||_pixelPerfect.cropFrame!=PixelPerfectCamera.CropFrame.Windowbox||_pixelPerfect.gridSnapping!=PixelPerfectCamera.GridSnapping.UpscaleRenderTexture||rotation.x!=0f||rotation.y!=0f||rotation.z!=0f||rotation.w!=1f||scale.x!=1f||scale.y!=1f||scale.z!=1f||float.IsNaN(_camera.transform.position.z)||float.IsInfinity(_camera.transform.position.z)||(_initialized&&_camera.transform.position.z!=_authoredZ)) throw new InvalidOperationException("Camera graph/profile drift is invalid.");
        }
        private void WriteTransform(int x,int y) { var p=_camera.transform.position; _camera.transform.position=new Vector3(x/18f,y/18f,p.z); }
        private Viewport CaptureViewport()
        {
            var target=_camera.targetTexture; var width=target!=null?target.width:Screen.width; var height=target!=null?target.height:Screen.height;
            if(width<640||height<360)return new Viewport(width,height,false,0,0,0,0,0);
            var scale=Math.Min(width/640,height/360); var rw=checked(640*scale);var rh=checked(360*scale);return new Viewport(width,height,true,scale,(width-rw)/2,(height-rh)/2,rw,rh);
        }
        private readonly struct Viewport { internal Viewport(int w,int h,bool s,int scale,int x,int y,int rw,int rh){Width=w;Height=h;Supported=s;Scale=scale;X=x;Y=y;RectWidth=rw;RectHeight=rh;} internal int Width{get;}internal int Height{get;}internal bool Supported{get;}internal int Scale{get;}internal int X{get;}internal int Y{get;}internal int RectWidth{get;}internal int RectHeight{get;} }
    }
}
