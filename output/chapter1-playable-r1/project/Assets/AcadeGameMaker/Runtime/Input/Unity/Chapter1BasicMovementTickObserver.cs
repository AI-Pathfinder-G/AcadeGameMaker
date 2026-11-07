#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-C1B-G3OBS-004/005/012: 이 타입 전체는 Player 조건에서 제외된다.
    // 정상 생산자 200 이후 자기 callback에서 읽고 기록한다. 게임 진행 권한은 없다.
    [DefaultExecutionOrder(210)]
    [DisallowMultipleComponent]
    internal sealed class Chapter1BasicMovementTickObserver : MonoBehaviour
    {
        internal const int MaxSamples=108001, MaxRowBytes=32768, MaxIdentities=32;
        internal const long MaxSampleBytes=134217728, MaxEventBytes=8388608, MaxNativeBytes=2147483648;
        internal const string ApprovedContractSha256="39C64FF77AA6F8B89EAEAD430C0BAE5B19E780F9842182F37CDA61B29E8E222F";
        [SerializeField] private Chapter1EncounterLifetimeOwner _authoringHost;
        [SerializeField] private string _configurationJson;
        [SerializeField] private string _configuredSessionId;
        [SerializeField] private bool _configured;
        private Configuration _configuration;
        private FileStream _samples,_events,_receivedInput;
        [SerializeField] private bool _receivedPermanentlyBlocked;
        private IReceivedInputContext _receivedContext;
        // REQ/AC-R01PLAY-001..004: same Unity owner survives one expected Play reload.
        [SerializeField] private UnityEngine.Object _receivedContextOwner;
        [SerializeField] private int _playPhase,_playRestoreCount,_playTraceRows;
        [SerializeField] private int _playUpdateMode=-1,_playKeyboardId,_playMouseId;
        [SerializeField] private long _editorUpdateCount,_noneUpdateCount,_beforeRenderUpdateCount;
        private int _seenUpdateTypes;
        private bool _guardResumed;
        private bool _reviewBeforeObserved,_reviewAfterObserved,_receivedAfterSubscribed;
        private ulong _reviewDynamicOrdinal;
        internal bool VerifyReviewGateAuthorityForAuthoring()=>!_sealed&&!_receivedPermanentlyBlocked&&_receivedReady&&_receivedBound&&_receivedAfterSubscribed&&_receivedContext!=null&&_receivedContext.VerifyCurrentAuthority()&&CheckReceivedBoundary();
        internal bool ExpectedPlaySuspendForAuthoring=>(_playPhase==1||_playPhase==2)&&!_guardResumed;
        internal bool ReviewSceneRestoreAuthorizedForAuthoring=>_playPhase==2&&_guardResumed&&!_receivedBoundary&&!_receivedPermanentlyBlocked;
        private static bool PlayRestoreAllowed(int phase,int restored,bool boundary,bool blocked,long subscriptions,long unsubscriptions,bool streamExists)
        {return phase==2&&restored==0&&!boundary&&!blocked&&subscriptions==1&&unsubscriptions==1&&!streamExists;}
        private static bool PlayBoundaryReady(bool ready,bool stream,bool subscribed,long registrations,int restored,bool context,bool playing,bool compiling,bool updating)
        {return ready&&stream&&subscribed&&registrations==2&&restored==1&&context&&playing&&!compiling&&!updating;}
        private static bool PlayTraceAllowed(int rows,int bytes)
        {return rows>=0&&rows<16&&bytes>=0&&bytes<=8192;}
        private static bool PlayWriterAllowed(bool boundary,bool closed,bool blocked,bool registrationReady,bool playing,bool context,bool paused,bool compiling,bool updating,bool focused,bool devices)
        {return !boundary&&!closed&&!blocked&&registrationReady&&playing&&context&&!paused&&!compiling&&!updating&&focused&&devices;}
        internal void RecordPlayTransitionForAuthoring(string stage,string details)
        {
            if(_playTraceRows>=16)throw new IOException("Play 수명 원시 16행 상한입니다.");
            var line="R01PLAY stage="+stage+" qpc="+Stopwatch.GetTimestamp().ToString(CultureInfo.InvariantCulture)+" phase="+_playPhase+" restores="+_playRestoreCount+" subscriptions="+_receivedSubscriptions+" unsubscriptions="+_receivedUnsubscriptions+" bound="+_receivedBound+" ready="+_receivedReady+" writer="+(_receivedInput!=null)+" boundary="+_receivedBoundary+" actualUpdateMode="+InputSystem.settings.updateMode+" fixedUpdateMode="+_playUpdateMode+" updateType="+InputState.currentUpdateType+" editorUpdates="+_editorUpdateCount+" noneUpdates="+_noneUpdateCount+" beforeRenderUpdates="+_beforeRenderUpdateCount+" "+details.Replace("\r","\\r").Replace("\n","\\n");
            if(!PlayTraceAllowed(_playTraceRows,_utf8.GetByteCount(line)))throw new IOException("Play 수명 원시 행8192 또는 16행 상한입니다.");
            _playTraceRows++;UnityEngine.Debug.Log(line);
        }
        internal void ExpectFirstPlayForAuthoring()
        {
            RequirePlayerUpdateMode();
            if(Application.isPlaying||_playPhase!=0||!_configured||!_receivedBound||!_receivedSubscribed||_receivedSubscriptions!=1||_receivedUnsubscriptions!=0||_receivedReady||_receivedBoundary||_started||_sealed||_receivedPermanentlyBlocked||_receivedInput!=null)
                throw new IOException("최초 Play 전환의 pristine 상태가 아닙니다.");
            _playPhase=1;RecordPlayTransitionForAuthoring("expected","sameOwner="+(_receivedContextOwner!=null));
        }
        internal void SuspendExpectedPlayForAuthoring()
        {
            if(_playPhase==2)return;
            RequirePlayerUpdateMode();
            if(_playPhase!=1||_receivedBoundary||_receivedInput!=null||_receivedReady||_started||_sealed||_receivedPermanentlyBlocked)
                throw new IOException("예상 Play 중단 상태가 아닙니다.");
            UnregisterReceivedCallbacks();
            if(_receivedSubscribed||_receivedEventSubscribed||_receivedUpdateSubscribed||_receivedUnsubscriptions!=1)throw new IOException("Play 이전 구독 해제가 완료되지 않았습니다.");
            _playPhase=2;RecordPlayTransitionForAuthoring("suspended","writerNeverOpened=true");
        }
        private void RequirePlayerUpdateMode()
        {if(InputSystem.settings==null||InputSystem.settings.updateMode!=InputSettings.UpdateMode.ProcessEventsInDynamicUpdate||(int)InputSystem.settings.updateMode!=_playUpdateMode)throw new IOException("고정 Dynamic 입력 설정이 변경되었거나 미확인입니다.");}
        private static int PlayerUpdateDisposition(int update,int none,int editor,int dynamic,int beforeRender,bool beforeRenderSafe)
        {if(update==none||update==editor)return 0;if(update==dynamic)return 1;if(update==beforeRender&&beforeRenderSafe)return 0;return -1;}
        private bool IsPlayerInputUpdate(bool observation=false)
        {
            RequirePlayerUpdateMode();var update=InputState.currentUpdateType;
            var keyboard=Keyboard.current;var mouse=Mouse.current;
            var beforeSafe=keyboard!=null&&mouse!=null&&keyboard.deviceId==_playKeyboardId&&mouse.deviceId==_playMouseId&&keyboard.added&&mouse.added&&!keyboard.updateBeforeRender&&!mouse.updateBeforeRender&&InputSystem.devices.Count==2;
            var disposition=PlayerUpdateDisposition((int)update,(int)InputUpdateType.None,(int)InputUpdateType.Editor,(int)InputUpdateType.Dynamic,(int)InputUpdateType.BeforeRender,beforeSafe);
            if(observation){if(update==InputUpdateType.Dynamic)_routeDynamicUpdates++;else if(update==InputUpdateType.Editor)_editorUpdateCount++;else if(update==InputUpdateType.None)_noneUpdateCount++;else if(update==InputUpdateType.BeforeRender)_beforeRenderUpdateCount++;}
            var bit=update==InputUpdateType.None?1:update==InputUpdateType.Editor?2:update==InputUpdateType.Dynamic?4:update==InputUpdateType.BeforeRender?8:16;
            if(observation&&(_seenUpdateTypes&bit)==0){_seenUpdateTypes|=bit;RecordPlayTransitionForAuthoring("first-update-type","type="+update+" disposition="+disposition+" beforeRenderSafe="+beforeSafe);}
            if(disposition<0)throw new IOException("고정 Dynamic 정책 밖의 입력 갱신형식입니다: "+update);
            return disposition==1;
        }
        private bool _receivedBound,_receivedSubscribed,_receivedEventSubscribed,_receivedUpdateSubscribed,_receivedReady,_receivedBoundary,_receivedFileClosed;
        private long _receivedBytes,_receivedRows,_receivedOrdinal;
        [SerializeField] private long _receivedErrors,_receivedSubscriptions,_receivedUnsubscriptions;
        private int _receivedKnownMask,_receivedPressedMask;
        private bool _receivedPaused;
        private long _receivedPauseCount;
        private Keyboard _receivedKeyboard;
        private Mouse _receivedMouse;
        private bool _started,_live,_hasSample;
        [SerializeField] private bool _sealed;
        private long _startStamp,_fixedCalls,_displayCalls,_cleanupCalls,_sampleCount,_duplicates,_unavailable;
        private long _sampleBytes,_eventBytes,_publicationSerial;
        private int _lastTick=-1,_exitTick=-1,_identityCount;
        private string _runKey;
        private long _generation;
        private InputFrameCommitReceipt _lastReceipt;
        private Chapter1FailurePublication _health,_kill,_crush;
        private InputRouter _router;
        private Chapter1HazardObservation _hazards;
        private PlayerMovementController _player;
        private CombatSimulationDriver _combat;
        private TransferSimulationDriver _transfer;
        private readonly object[] _identities=new object[MaxIdentities];
        private long[] _sourceWriteTicks;
        private FileInfo[] _sourceMetadata;
        private string[] _sourceMetadataPaths;
        private double _sourceMetadataMilliseconds;
        private static bool SourceMetadataMatches(bool exists,long length,long expectedLength,long writeTicks,long expectedWriteTicks)=>exists&&length==expectedLength&&writeTicks==expectedWriteTicks;
        private readonly UTF8Encoding _utf8=new UTF8Encoding(false,true);
        private string _latestJson,_firstFailureJson,_sealError;
        private double _previousWriteMilliseconds;

        internal bool ReceivedCallbacksRegisteredForAuthoring=>_receivedSubscribed;
        internal bool Live=>_live;
        internal bool IsSealed=>_sealed;
        internal string LatestRowJson=>_latestJson;
        internal string FirstFailureJson=>_firstFailureJson;
        internal string SealError=>_sealError;
        internal string CaseId=>_configuration==null?null:_configuration.caseId;
        internal string ConfiguredSessionIdForAuthoring=>_configuredSessionId;
        internal long DistinctSampleCount=>_sampleCount;

        // REQ/AC-C1B-G3OBS-005/006/007/012: 읽기 전용 Editor 경계. Player에서는 타입 전체가 없다.
        internal interface IReceivedInputContext
        {
            bool TryReadReceivedInputContext(out ReceivedInputContext context);
            bool TryRestoreReceivedInputContext(Chapter1BasicMovementTickObserver collector);
            bool IsActualPlayPaused();
            bool VerifyCurrentAuthority();
            bool TryPauseFirstSample(Chapter1BasicMovementTickObserver collector,FirstPauseRequest request,out bool before,out bool after,out int setterCount);
        }
        internal readonly struct FirstPauseRequest
        {
            internal readonly string sessionId,caseId,runKey,approvalSha;
            internal readonly long generation,sampleCount,sampleBytes;
            internal readonly int tick,health;
            internal readonly bool flushSucceeded,healthTriggered,killTriggered,crushTriggered,isDead;
            internal FirstPauseRequest(string session,string selected,string run,string sha,long gen,int completed,long count,long bytes,int hp,bool flushed,bool healthFlag,bool killFlag,bool crushFlag,bool dead)
            {sessionId=session;caseId=selected;runKey=run;approvalSha=sha;generation=gen;tick=completed;sampleCount=count;sampleBytes=bytes;health=hp;flushSucceeded=flushed;healthTriggered=healthFlag;killTriggered=killFlag;crushTriggered=crushFlag;isDead=dead;}
        }
        private int _pausePhase,_pauseInvocationCount,_pauseSetterCount,_pauseTraceRows;
        private bool _pauseRequested,_pauseBefore,_pauseAfter,_pauseFlushSucceeded,_diagnosticsAccepted,_resumeRequested,_pauseFailureRecorded;
        private int _pauseObservedTick=-1,_pauseObservedHealth=-1;
        private FirstPauseRequest _firstPauseRequest;
        private int _routeStageMask,_routeRows;
        private long _routeDynamicUpdates,_routeNativeEvents,_routeNativeDynamic,_routeNativeEditor,_routeNativeOther,_routeStateEvents,_routeDeltaEvents,_routeControlBefore,_routeControlAfter,_routeLastNativeStamp;
        [Serializable] private sealed class RouteDevice
        {public bool available,sameReference,added,enabled;public int deviceId;}
        [Serializable] private sealed class RouteEvents
        {public long dynamicUpdates,editorUpdates,nativeEvents,nativeDynamic,nativeEditor,nativeOther,stateEvents,deltaEvents,controlBefore,controlAfter,lastNativeStopwatchTicks;}
        [Serializable] private sealed class RouteSnapshot
        {
            public bool available,applicationFocused,gameViewFocused,isPlaying,isPaused;
            public string unavailableReason;
            public RouteDevice keyboard,mouse;
            public InputRouter.ReviewInputRoute router;
            public RouteEvents events;
        }
        [Serializable] private sealed class RouteObservation
        {
            public int schemaVersion=1;public string kind="input-route-observation",stage,sessionId,caseId,approvalSHA256,runKey,updateType;
            public long generation,observedStopwatchTicks,stopwatchFrequency;public ulong updateCount;public int frameCount;public RouteSnapshot route;
        }
        private static bool RouteStageAllowed(int mask,int bit,int rows)=>bit>0&&(bit==1||bit==2||bit==4||bit==8)&&(mask&bit)==0&&rows>=0&&rows<4;
        private static bool OriginalRouteDevice(bool present,bool keyboard,bool mouse)=>present&&(keyboard||mouse);
        private static bool RouteReady(bool context,bool keyboard,bool mouse,bool owner,bool wrapper,bool maps,bool registrations,bool gameplay,bool ui,bool initialized,bool started,bool faulted,bool disposed,bool closed,bool suppressed)=>context&&keyboard&&mouse&&owner&&wrapper&&maps&&registrations&&gameplay&&!ui&&initialized&&started&&!faulted&&!disposed&&!closed&&!suppressed;
        private RouteSnapshot ReadInputRoute()
        {
            var context=ReadReceivedContext(out var state);var keyboard=_receivedKeyboard;var mouse=_receivedMouse;
            var row=new RouteSnapshot{available=context,unavailableReason=context?"":"current-context-unavailable",applicationFocused=Application.isFocused,gameViewFocused=context&&state.gameViewFocused,isPlaying=Application.isPlaying,isPaused=_receivedContext!=null&&_receivedContext.IsActualPlayPaused(),
                keyboard=new RouteDevice{available=keyboard!=null,deviceId=keyboard==null?0:keyboard.deviceId,sameReference=keyboard!=null&&ReferenceEquals(keyboard,Keyboard.current),added=keyboard!=null&&keyboard.added,enabled=keyboard!=null&&keyboard.enabled},
                mouse=new RouteDevice{available=mouse!=null,deviceId=mouse==null?0:mouse.deviceId,sameReference=mouse!=null&&ReferenceEquals(mouse,Mouse.current),added=mouse!=null&&mouse.added,enabled=mouse!=null&&mouse.enabled},
                events=new RouteEvents{dynamicUpdates=_routeDynamicUpdates,editorUpdates=_editorUpdateCount,nativeEvents=_routeNativeEvents,nativeDynamic=_routeNativeDynamic,nativeEditor=_routeNativeEditor,nativeOther=_routeNativeOther,stateEvents=_routeStateEvents,deltaEvents=_routeDeltaEvents,controlBefore=_routeControlBefore,controlAfter=_routeControlAfter,lastNativeStopwatchTicks=_routeLastNativeStamp}};
            if(_router!=null&&_authoringHost!=null&&_authoringHost.CurrentRouter==_router)row.router=_router.ReadReviewInputRouteForAuthoring(_authoringHost);else{row.router.available=false;row.router.unavailableReason="original-router-unavailable";}
            return row;
        }
        private void GuardInputRoute()
        {
            var row=ReadInputRoute();var router=row.router;
            if(!RouteReady(row.available,row.keyboard.available&&row.keyboard.sameReference&&row.keyboard.added&&row.keyboard.enabled,row.mouse.available&&row.mouse.sameReference&&row.mouse.added&&row.mouse.enabled,router.available&&router.ownerSame,router.wrapperSame,router.mapSame,router.registrationSame,router.gameplayMapEnabled,router.uiMapEnabled,router.initialized,router.startCompleted,router.faulted,router.disposed,router.closed,router.captureSuppressed))throw new IOException("원래 입력 장치 enabled 또는 wrapper/map/등록 경로가 다릅니다.");
        }
        private void WriteInputRoute(string stage,int bit)
        {
            if((_routeStageMask&bit)!=0)return;
            if(!RouteStageAllowed(_routeStageMask,bit,_routeRows))throw new IOException("입력 경로 원시4행 상한입니다.");
            var row=new RouteObservation{stage=stage,sessionId=_configuration.sessionId,caseId=_configuration.caseId,approvalSHA256=_configuration.executionApproval.sha256,runKey=_runKey,generation=_generation,observedStopwatchTicks=Stopwatch.GetTimestamp(),stopwatchFrequency=Stopwatch.Frequency,frameCount=Time.frameCount,updateType=InputState.currentUpdateType.ToString(),updateCount=InputState.updateCount,route=ReadInputRoute()};
            var json=JsonUtility.ToJson(row);if(_utf8.GetByteCount(json)+1>8192)throw new IOException("입력 경로 원시 행8192 상한입니다.");
            _routeStageMask|=bit;_routeRows++;Append(_events,json,MaxEventBytes,ref _eventBytes);
        }
        internal void VerifyResumeInputRouteForAuthoring()
        {WriteInputRoute("resume-before",1);GuardInputRoute();}
        private static bool FirstPauseAllowed(bool live,bool sealedValue,int phase,long count,int tick,int health,bool dead,bool hazard,bool flushed,bool bound,bool boundary)
        {return live&&!sealedValue&&phase==0&&count==1&&tick==0&&health>0&&!dead&&!hazard&&flushed&&bound&&boundary;}
        private static bool PauseWaitAllowed(bool live,bool sealedValue,bool sameAuthority,bool paused,long samples,int firstTick,int completedTick,bool hazard)
        {return live&&!sealedValue&&sameAuthority&&paused&&samples==1&&firstTick==0&&completedTick==0&&!hazard;}
        private static bool ResumeAllowed(int phase,bool waitingValid,bool diagnosticAccepted,bool sameApproval,bool requested)
        {return phase==2&&waitingValid&&diagnosticAccepted&&sameApproval&&!requested;}
        internal bool FirstPauseWaitingForAuthoring=>_configuration!=null&&_configuration.caseId=="R01-A"&&(_pausePhase==2||_pausePhase==3||_pausePhase==4);
        internal bool FirstPauseDiagnosticReadyForAuthoring=>FirstPauseWaitingForAuthoring&&_pausePhase==2&&_sampleCount==1&&_lastTick==0&&_pauseAfter&&_pauseFlushSucceeded&&_live&&!_sealed;
        internal bool ExactFirstPauseRequestForAuthoring(FirstPauseRequest request)=>_pausePhase==1&&_pauseInvocationCount==1&&request.sessionId==_configuration.sessionId&&request.caseId==_configuration.caseId&&request.runKey==_runKey&&request.generation==_generation&&request.tick==_lastTick&&request.sampleCount==_sampleCount&&request.sampleBytes==_sampleBytes&&request.approvalSha==_configuration.executionApproval.sha256&&request.flushSucceeded&&_pauseFlushSucceeded&&request.health==_pauseObservedHealth&&!request.isDead&&!request.healthTriggered&&!request.killTriggered&&!request.crushTriggered;
        private void WritePauseTransition(string reason)
        {
            if(_pauseTraceRows>=8)throw new IOException("일시정지 원시 8전이 상한입니다.");
            var row=new PauseTransitionRow{utc=DateTime.UtcNow.ToString("O"),sessionId=_configuration.sessionId,caseId=_configuration.caseId,reason=reason,runKey=_runKey,generation=_generation,monotonicStamp=Stopwatch.GetTimestamp(),fixedCall=_fixedCalls,displayCall=_displayCalls,frameCount=Time.frameCount,lastTick=_lastTick,samples=_sampleCount,elapsedSeconds=Elapsed,live=_live,phase=_pausePhase,pauseRequested=_pauseRequested,pauseInvocationCount=_pauseInvocationCount,pauseSetterInvocationCount=_pauseSetterCount,actualIsPausedBefore=_pauseBefore,actualIsPausedAfter=_pauseAfter,lastCompletedTick=_pauseObservedTick,sampleCount=_sampleCount,sampleBytes=_sampleBytes,health=_pauseObservedHealth,flushSucceeded=_pauseFlushSucceeded,diagnosticsAccepted=_diagnosticsAccepted,resumeRequested=_resumeRequested,approvalSHA256=_configuration.executionApproval.sha256};
            var body=JsonUtility.ToJson(row);if(_runKey==null)body=body.Replace("\"runKey\":\"\"","\"runKey\":null");var json=body.Substring(0,body.Length-1)+",\"configuration\":null}";
            if(_utf8.GetByteCount(json)>8192)throw new IOException("일시정지 원시 행8192 상한입니다.");
            _pauseTraceRows++;Append(_events,json,MaxEventBytes,ref _eventBytes);
        }
        [Serializable] private sealed class PauseTransitionRow
        {
            public int schemaVersion=1;public string kind="first-sample-pause",utc,sessionId,caseId,reason,runKey,approvalSHA256;
            public long generation,monotonicStamp,fixedCall,displayCall,samples,sampleCount,sampleBytes;
            public int frameCount,lastTick,phase,pauseInvocationCount,pauseSetterInvocationCount,firstTick,lastCompletedTick,health;
            public double elapsedSeconds;public bool live,pauseRequested,actualIsPausedBefore,actualIsPausedAfter,flushSucceeded,diagnosticsAccepted,resumeRequested;
        }
        private void RequestFirstSamplePause(SampleRow row)
        {
            if(!FirstPauseAllowed(_live,_sealed,_pausePhase,_sampleCount,row.tick,row.health,row.isDead,row.healthPublication.triggered||row.killPlanePublication.triggered||row.lethalCrushPublication.triggered,_pauseFlushSucceeded,_receivedBound,_receivedBoundary))throw new IOException("첫 표본의 동기 Pause 조건이 다릅니다.");
            _firstPauseRequest=new FirstPauseRequest(_configuration.sessionId,_configuration.caseId,_runKey,_configuration.executionApproval.sha256,_generation,row.tick,_sampleCount,_sampleBytes,row.health,true,row.healthPublication.triggered,row.killPlanePublication.triggered,row.lethalCrushPublication.triggered,row.isDead);
            _pauseRequested=true;_pauseInvocationCount++;_pausePhase=1;_pauseObservedTick=row.tick;_pauseObservedHealth=row.health;_pauseBefore=_receivedContext.IsActualPlayPaused();_pauseAfter=_pauseBefore;WritePauseTransition("first-sample-flushed-request-consumed");
            if(!_receivedContext.TryPauseFirstSample(this,_firstPauseRequest,out _pauseBefore,out _pauseAfter,out _pauseSetterCount)||_pauseBefore||!_pauseAfter||_pauseSetterCount!=1)throw new IOException("동일 Window의 첫 표본 Pause가 실제 확인되지 않았습니다.");
            _pausePhase=2;WritePauseTransition("actual-pause-confirmed-waiting-diagnostics");
        }
        internal void VerifyFirstPauseWaitForAuthoring()
        {
            if(!FirstPauseWaitingForAuthoring)return;
            try
            {
                if(_configuration.caseElapsedSecondsAtInstall+Elapsed>=_configuration.caseBudgetSeconds||_configuration.totalElapsedSecondsAtInstall+Elapsed>=_configuration.totalBudgetSeconds)throw new IOException("일시정지 대기 중 원래 기한이 만료됐습니다.");
                CheckSourceMetadata();
                var same=_authoringHost!=null&&_authoringHost.CurrentRouter==_router&&_router!=null&&_receivedContext!=null&&ReadReceivedContext(out var state)&&state.sessionId==_configuration.sessionId;
                var hazard=true;_pauseObservedTick=-1;_pauseObservedHealth=-1;
                if(same&&_router.TryReadChapter1PublishedObservationForAuthoring(out var receipt,out var health)&&_authoringHost.CurrentHazards!=null&&_authoringHost.CurrentHazards.TryReadPublishedHazardsForAuthoring(out var kill,out var crush))
                {_pauseObservedTick=receipt.Tick.Value;hazard=health.Triggered||kill.Triggered||crush.Triggered||health.RunKey!=_runKey||health.Generation!=_generation;if(_authoringHost.TryGetCompletedEvidence(out var completed)){_pauseObservedHealth=completed.Health;hazard|=completed.Health<=0||completed.Receipt.Tick.Value!=_pauseObservedTick;}else hazard=true;}
                if(!PauseWaitAllowed(_live,_sealed,same,_receivedContext!=null&&_receivedContext.IsActualPlayPaused(),_sampleCount,_lastTick,_pauseObservedTick,hazard))throw new IOException("Pause 대기 중 추가 완료 tick·위험·신원·정지 상태가 다릅니다.");
            }
            catch(Exception error){FailReceived(error.Message);}
        }
        internal void ObserveActualPauseForAuthoring(bool paused)
        {if(FirstPauseWaitingForAuthoring&&!paused&&_pausePhase!=4)FailReceived("진단 수용·명시 재개 전 외부 unpause가 관측됐습니다.");}
        internal void BeginAcceptedResumeForAuthoring(string approvalSha)
        {
            VerifyFirstPauseWaitForAuthoring();
            if(!ResumeAllowed(_pausePhase,FirstPauseDiagnosticReadyForAuthoring,true,approvalSha==_configuration.executionApproval.sha256,_resumeRequested))throw new IOException("진단 독립 수용 뒤 동일 승인 재개 단회가 아닙니다.");
            _diagnosticsAccepted=true;_pausePhase=3;WritePauseTransition("root-request-attests-independent-diagnostic-acceptance");
            _resumeRequested=true;_pausePhase=4;WritePauseTransition("explicit-resume-consumed");
        }
        internal void CompleteAcceptedResumeForAuthoring()
        {
            if(_pausePhase!=4||!_resumeRequested||!_diagnosticsAccepted||_receivedContext==null||_receivedContext.IsActualPlayPaused()||_sealed||!_live)throw new IOException("명시 재개 실제 상태를 확인하지 못했습니다.");
            _pauseAfter=false;_pausePhase=5;WritePauseTransition("explicit-resume-confirmed");
        }
        internal readonly struct ReceivedInputContext
        {
            internal string sessionId{get;} internal string caseId{get;} internal string gameViewInstanceIdDecimal{get;}
            internal bool isPlaying{get;} internal bool isPaused{get;} internal bool isCompiling{get;} internal bool isUpdating{get;} internal bool gameViewFocused{get;}
            internal ReceivedInputContext(string session,string selected,string id,bool playing,bool paused,bool compiling,bool updating,bool focused)
            {sessionId=session;caseId=selected;gameViewInstanceIdDecimal=id;isPlaying=playing;isPaused=paused;isCompiling=compiling;isUpdating=updating;gameViewFocused=focused;}
        }
        [Serializable] private sealed class ReceivedInputRow
        {
            public int schemaVersion=1; public string kind="received-input",sessionId,caseId,inputEvidenceKind,performer,gameViewInstanceIdDecimal,deviceKind;
            public long receivedOrdinal,observedStopwatchTicks,stopwatchFrequency,lastCompletedTick=-1;
            public double eventTime; public int controlMask,pressedMask,knownBeforeMask,changedMask; public bool sampleCollectorLive;
        }
        [Serializable] private sealed class ReceivedInputStatusRow
        {
            public int schemaVersion=1; public string kind,utc,sessionId,caseId,inputEvidenceKind,performer,gameViewInstanceIdDecimal,reason;
            public long observedStopwatchTicks,stopwatchFrequency,receivedOrdinal,rows,bytes,errors,subscriptions,unsubscriptions;
            public bool registered,incomplete,sourceReady=false,independentAcceptance=false;
        }
        internal static ulong ParseGameViewIdDecimal(string value)
        {
            if(string.IsNullOrEmpty(value) || value[0]=='0')throw new IOException("GameView 식별 십진문자열이 없습니다.");
            foreach(var c in value)if(c<'0' || c>'9')throw new IOException("GameView 식별값은 정규 ASCII 십진문자열이어야 합니다.");
            if(!ulong.TryParse(value,NumberStyles.None,CultureInfo.InvariantCulture,out var id) || id==0 ||
                id.ToString(CultureInfo.InvariantCulture)!=value)throw new IOException("GameView 식별값의 범위·왕복이 다릅니다.");
            return id;
        }
        internal static void RequireDistinctRawPaths(params string[] paths)
        {
            if(paths==null || paths.Length!=5)throw new IOException("정확5 원시 경로가 필요합니다.");
            for(var i=0;i<paths.Length;i++)for(var j=0;j<i;j++)
                if(SamePath(paths[i],paths[j]))throw new IOException("원시 경로5의 정규화 결과가 겹칩니다.");
        }

        [Serializable] internal sealed class FilePin
        { public string path; public long length; public string sha256; }
        [Serializable] internal sealed class Configuration
        {
            public int schemaVersion=1;
            public string sessionId,caseId,roomId,socketId,reviewMenu,sceneName,evidenceRoot,samplesPath,eventsPath,sealPath;
            public string projectPath,performer,performerModel,performerRole,unityExe,unityVersion,windowIdentity,inputTarget;
            public string inputEvidenceKind,gameViewInstanceIdDecimal;
            public int unityPid;
            // REQ/AC-C1B-G3OBS-010, REQ/AC-M1COMP-R1-001: 설치·표본 식별 원문만 UInt64로 보존한다.
            public ulong installedInstanceId,installedSceneHandle;
            public int lifetimeAttributeOrder,collectorAttributeOrder,lifetimeImporterOrder,collectorImporterOrder;
            public long unityStartTicks;
            public double caseBudgetSeconds,totalBudgetSeconds,caseElapsedSecondsAtInstall,totalElapsedSecondsAtInstall;
            public bool inputNeutralObserved;
            public string nativeEvidencePath,inputEvidencePath,requiredPathJson,authoringHostHierarchy;
            public FilePin sourceCheckpoint,approvedContract,executionApproval,toolObservation,observerSource,observerMeta,windowSource,windowMeta,prefab,prefabMeta;
            public FilePin[] sourceFiles;
        }
        [Serializable] internal sealed class PublicationRow
        {
            public long localPublicationNumber,revision;
            public int issuerNumber,tokenNumber,routerNumber;
            public string kind;
            public bool triggered,ownedByStoredSlot,issuerSame,routerSame;
        }
        [Serializable] internal sealed class SampleRow
        {
            public int schemaVersion=1;
            public string kind="sample",utc,sessionId,caseId,roomId,socketId,runKey,mode,wallId,wallSide,actionState;
            public string hostHierarchy,graphHierarchy,transferHighlightedTarget,transferActiveTarget,transferPlayerState,transferLastFailure,transferLifecycleReason,transferLifecycleClear,transferLifecyclePreviousTarget;
            public long monotonicStamp,generation,frameOrdinal,modeEpoch,fixedCall,displayCall,distinctCount,duplicateCount,unavailableCount,streamBytes;
            public int frameCount,startCall=1;
            public ulong hostId,routerId,hazardsId,playerId,combatId,transferId,collectorId,sceneHandle;
            public int tick,nextExpectedTick,health,invulnerabilityEndTick,positionXQ4096,positionYQ4096,velocityXQ4096,velocityYQ4096,transferRevision,transferTransitionLockEndTick,transferRemovalCount,transferLifecycleTick=-1,transferLifecycleRevision=-1;
            public bool isDead,isGrounded,airDashAvailable,hadAim,hadPress,sameSourcesBeforeAfter,firstTickZero,exitCandidate,independentAcceptance=false;
            public float bodyCenterX,bodyCenterY,bodyWidth,bodyHeight,bodyFootY;
            public double elapsedSeconds,readMilliseconds,previousAppendFlushMilliseconds,sourceMetadataMilliseconds;
            public PublicationRow healthPublication,killPlanePublication,lethalCrushPublication;
        }
        [Serializable] internal sealed class EventRow
        {
            public int schemaVersion=1;
            public string kind,utc,sessionId,caseId,reason,runKey;
            public long monotonicStamp,fixedCall,displayCall,cleanupCall,generation,samples,duplicates,unavailable;
            public long frameOrdinal=-1,modeEpoch=-1,healthRevision,killRevision,crushRevision;
            public int frameCount,lastTick;
            public double elapsedSeconds,previousAppendFlushMilliseconds;
            public bool live,independentAcceptance=false;
            public Configuration configuration;
        }
        [Serializable] internal sealed class SealRow
        {
            public int schemaVersion=1;
            public string kind="seal",utc,sessionId,caseId,reason,runKey,completionKey,firstFailureJson,lastSampleJson,ioLimitation;
            public string sourceCheckpointSha256,observerHelperSha256,prefabSha256,prefabMetaSha256,sealError;
            public long generation,samples,duplicates,unavailable,fixedCalls,displayCalls,cleanupCalls;
            public int firstTick=-1,lastTick=-1,exitCandidateTick=-1,gapCount;
            public bool live=false,sourcePinsUnchanged,continuousFromZero,independentAcceptance=false,movementAccepted=false;
            public double elapsedSeconds;
            public FilePin samplesFile,eventsFile,inputFile;
            public bool inputFileClosed,inputObservationComplete,inputIncomplete;
            public long inputRows,inputBytes,receivedCount,inputErrors,inputSubscriptions,inputUnsubscriptions,inputPauseCount;
            public string inputObservationScope;
        }

        // 재생 전 자기 스칼라와 실제 저작 host 참조1만 구성한다.
        internal void ConfigureForAuthoring(Chapter1EncounterLifetimeOwner host,Configuration configuration)
        {
            if(Application.isPlaying || _configured || _started || host==null || configuration==null || transform.parent!=null ||
                host.gameObject.scene!=gameObject.scene || GetComponents<Chapter1BasicMovementTickObserver>().Length!=1)
                throw new InvalidOperationException("관측기는 같은 미저장 장면의 별도 임시 root에서 한 번 구성합니다.");
            _authoringHost=host;_configurationJson=JsonUtility.ToJson(configuration);_configuredSessionId=configuration.sessionId;_configured=true;
            _playUpdateMode=(int)InputSystem.settings.updateMode;RequirePlayerUpdateMode();
            if(Keyboard.current==null||Mouse.current==null)throw new IOException("준비 시 허용 입력 장치가 없습니다.");
            _playKeyboardId=Keyboard.current.deviceId;_playMouseId=Mouse.current.deviceId;
            host.BindReviewPlayObserverForAuthoring(this);
            _configuration=configuration;RegisterReceivedCallbacks();
        }

        private void OnEnable()
        {
            if(!_configured || _sealed)return;
            try
            {
                if(_receivedPermanentlyBlocked)throw new IOException("이 관측은 이미 불완전으로 폐쇄됐습니다.");
                if(_configuration==null)_configuration=JsonUtility.FromJson<Configuration>(_configurationJson);
                if(Application.isPlaying)
                {
                    var allowed=PlayRestoreAllowed(_playPhase,_playRestoreCount,_receivedBoundary,_receivedPermanentlyBlocked,_receivedSubscriptions,_receivedUnsubscriptions,_receivedInput!=null);
                    if(!allowed)throw new IOException("최초 Play 정상 복원1 권위가 없습니다.");
                    RequirePlayerUpdateMode();
                    // Only the first-input guard is active here; loaded scene/owner are not assumed.
                    RegisterReceivedCallbacks();
                    _guardResumed=true;
                }
                else
                {
                    if(_playPhase!=0)throw new IOException("예상 Play 전환이 아닌 재활성입니다.");
                    RegisterReceivedCallbacks();
                }
            }
            catch(Exception error){FailReceived("입력 등록 실패: "+error.Message);}
        }
        private void RegisterReceivedCallbacks()
        {
            if(_receivedSubscribed)return;
            if((_receivedSubscriptions>0&&!PlayRestoreAllowed(_playPhase,_playRestoreCount,_receivedBoundary,_receivedPermanentlyBlocked,_receivedSubscriptions,_receivedUnsubscriptions,_receivedInput!=null)) || _receivedPermanentlyBlocked || _sealed || !_configured)throw new IOException("폐쇄된 관측의 재등록은 금지됩니다.");
            if(_receivedEventSubscribed || _receivedUpdateSubscribed)throw new IOException("부분 등록은 재사용하지 않습니다.");
            InputSystem.onEvent+=ObserveReceivedInput;_receivedEventSubscribed=true;
            InputSystem.onBeforeUpdate+=ObserveBeforeInputUpdate;_receivedUpdateSubscribed=true;
            InputSystem.onAfterUpdate+=ObserveAfterInputUpdate;_receivedAfterSubscribed=true;
            _receivedSubscribed=true;_receivedSubscriptions++;
        }
        internal void BindReceivedInputContextForAuthoring(IReceivedInputContext context)
        {
            if(_configuration==null && _configured)_configuration=JsonUtility.FromJson<Configuration>(_configurationJson);
            if(context==null || _receivedBound || _receivedBoundary || _receivedPermanentlyBlocked || _sealed || _started ||
                !_configured || !context.TryReadReceivedInputContext(out var state) || !SameReceivedTuple(state))
                throw new IOException("pristine 관측기의 동일 승인 bridge 단1회 결속이 아닙니다.");
            _receivedContextOwner=context as UnityEngine.Object;
            if(_receivedContextOwner==null)throw new IOException("bridge는 동일 Unity 객체여야 합니다.");
            _receivedContext=context;_receivedBound=true;RecordPlayTransitionForAuthoring("bridge-prepared","sameUnityOwner=true");
            if(Application.isPlaying && _receivedSubscribed)PrepareReceivedInputForAuthoring();
        }
        private static bool SceneRestoreAllowed(int phase,int restored,bool boundary,bool blocked,long registrations,long unregistrations,bool subscribed,bool writer)
        {return phase==2&&restored==0&&!boundary&&!blocked&&registrations==2&&unregistrations==1&&subscribed&&!writer;}
        internal void RestoreSceneLoadedForAuthoring(IReceivedInputContext context,UnityEngine.SceneManagement.Scene scene)
        {
            try
            {
                RequirePlayerUpdateMode();
                if(!SceneRestoreAllowed(_playPhase,_playRestoreCount,_receivedBoundary,_receivedPermanentlyBlocked,_receivedSubscriptions,_receivedUnsubscriptions,_receivedSubscribed,_receivedInput!=null)||_sealed||context==null||scene!=gameObject.scene||!scene.isLoaded)
                    throw new IOException("sceneLoaded의 정확 중단·첫 경계 이전 복원 상태가 아닙니다.");
                _authoringHost.CompleteReviewSceneLoadedForAuthoring(scene);
                _receivedContextOwner=context as UnityEngine.Object;_receivedContext=context;
                if(_receivedContextOwner==null||!context.TryRestoreReceivedInputContext(this))throw new IOException("같은 원래 Window의 sceneLoaded bridge 검증 실패입니다.");
                _receivedBound=true;_playRestoreCount=1;_playPhase=3;
                PrepareReceivedInputForAuthoring();
                if(_sealed||!_receivedReady)throw new IOException("sceneLoaded writer 준비 실패입니다.");
                _authoringHost.PrepareReviewGraphForAuthoring(this,Stopwatch.GetTimestamp(),Time.frameCount);
                RecordPlayTransitionForAuthoring("restored","sceneLoaded=true ownerExact=true");
            }
            catch(Exception error){FailReceived("sceneLoaded 복원 실패: "+error.Message);}
        }
        internal void PrepareReceivedInputForAuthoring()
        {
            if(_receivedReady)return;
            if(Application.isPlaying && _startStamp==0)_startStamp=Stopwatch.GetTimestamp();
            try
            {
                var hasContext=ReadReceivedContext(out var state);var registrationReady=_receivedSubscribed||(_playPhase==3&&_playRestoreCount==1&&_receivedSubscriptions==1&&_receivedUnsubscriptions==1);
                var devices=Keyboard.current!=null&&Mouse.current!=null;
                var allowed=PlayWriterAllowed(_receivedBoundary,_sealed,_receivedPermanentlyBlocked,registrationReady,Application.isPlaying&&state.isPlaying,hasContext,state.isPaused,state.isCompiling,state.isUpdating,state.gameViewFocused,devices);
                if(!allowed)RecordPlayTransitionForAuthoring("writer-check","allowed=false registrationReady="+registrationReady+" context="+hasContext+" playing="+state.isPlaying+" paused="+state.isPaused+" compiling="+state.isCompiling+" updating="+state.isUpdating+" focused="+state.gameViewFocused+" devices="+devices);
                if(!allowed)
                    throw new IOException("첫 입력 경계 전 실제 bridge·포커스·재생 준비가 없습니다.");
                ValidateConfiguration();
                if(_authoringHost==null||!_authoringHost.isActiveAndEnabled||_authoringHost.gameObject.scene!=gameObject.scene||
                    Hierarchy(_authoringHost.transform)!=_configuration.authoringHostHierarchy||EntityId.ToULong(GetEntityId())!=_configuration.installedInstanceId||
                    gameObject.scene.handle.GetRawData()!=_configuration.installedSceneHandle)
                    throw new IOException("첫 경계 전 동일 host·collector·scene 신원이 다릅니다.");
                foreach(var pin in _configuration.sourceFiles)VerifyPin(pin,_configuration.projectPath);
                RequireNewRawPath(_configuration.inputEvidencePath,_configuration.evidenceRoot);
                if(_startStamp==0)_startStamp=Stopwatch.GetTimestamp();
                _receivedKeyboard=Keyboard.current;_receivedMouse=Mouse.current;
                if(_receivedKeyboard==null || _receivedMouse==null)throw new IOException("허용 keyboard/mouse 원본이 없습니다.");
                _receivedInput=new FileStream(_configuration.inputEvidencePath,FileMode.CreateNew,FileAccess.Write,FileShare.Read);
                WriteReceivedStatus(_receivedSubscribed?"input-registered":"input-writer-ready",null);
                _receivedReady=true;
                RecordPlayTransitionForAuthoring("writer-ready","fileCreatedNew=true");
            }
            catch(Exception error){FailReceived("입력 작성자 준비 실패: "+error.Message);}
        }
        private bool SameReceivedTuple(ReceivedInputContext state)=>_configuration!=null &&
            state.sessionId==_configuration.sessionId && state.caseId==_configuration.caseId &&
            state.gameViewInstanceIdDecimal==_configuration.gameViewInstanceIdDecimal;
        private bool ReadReceivedContext(out ReceivedInputContext state)
        {
            state=default;
            return _receivedContext!=null && _receivedContext.TryReadReceivedInputContext(out state) && SameReceivedTuple(state);
        }
        private bool CheckReceivedBoundary(bool actualPlayerBoundary=false)
        {
            if(_sealed || _receivedPermanentlyBlocked)return false;
            try
            {
                RequirePlayerUpdateMode();
                var hasContext=ReadReceivedContext(out var state);
                if(FirstPauseWaitingForAuthoring&&!state.isPaused&&_pausePhase!=4)throw new IOException("명시 재개 전 입력 경계가 재개됐습니다.");
                var ready=PlayBoundaryReady(_receivedReady,_receivedInput!=null,_receivedSubscribed,_receivedSubscriptions,_playRestoreCount,hasContext,state.isPlaying,state.isCompiling,state.isUpdating);
                if(actualPlayerBoundary&&!state.isPaused&&!_receivedBoundary){RecordPlayTransitionForAuthoring("first-boundary","ready="+ready+" context="+hasContext+" playing="+state.isPlaying+" compiling="+state.isCompiling+" updating="+state.isUpdating);_receivedBoundary=true;_playPhase=4;}
                if(!ready)
                    throw new IOException("최초 입력 경계까지 동일 bridge·작성자가 준비되지 않았습니다.");
                if(state.isPaused)
                {
                    if(!_receivedPaused)
                    {
                        _receivedPaused=true;_receivedPauseCount++;_receivedKnownMask=0;_receivedPressedMask=0;
                        WriteReceivedStatus("input-paused","실제 Pause의 수신 범위 중단: 이 구간의 입력·변화·누락 여부는 관측하지 않습니다.");
                    }
                    return false;
                }
                if(!state.gameViewFocused)throw new IOException("게임 포커스가 소실됐습니다. 자동 재개하지 않습니다.");
                if(_pausePhase==5)GuardInputRoute();
                if(_receivedPaused)
                {
                    _receivedPaused=false;_receivedKnownMask=0;_receivedPressedMask=0;
                    WriteReceivedStatus("input-resumed","실제 Pause 종료: 이전 상태 unknown이며 다음 실제 사건만 기록합니다.");
                }
                if(_configuration.caseElapsedSecondsAtInstall+Elapsed>=1800 || _configuration.totalElapsedSecondsAtInstall+Elapsed>=18000)
                    throw new IOException("입력 관측의 실제 시간 상한입니다.");
                if(!ReferenceEquals(_receivedKeyboard,Keyboard.current) || !ReferenceEquals(_receivedMouse,Mouse.current) ||
                    !_receivedKeyboard.added || !_receivedMouse.added)throw new IOException("허용 입력 장치 원본이 변경·제거됐습니다.");
                return true;
            }
            catch(Exception error){FailReceived(error.Message);return false;}
        }
        private void ObserveBeforeInputUpdate()
        {
            if(!Application.isPlaying||_sealed||_receivedPermanentlyBlocked)return;
            try{if(IsPlayerInputUpdate(true)&&CheckReceivedBoundary(true)&&!_reviewBeforeObserved)
                {_reviewDynamicOrdinal=InputState.updateCount;_authoringHost.ObserveReviewDynamicBeforeForAuthoring(_reviewDynamicOrdinal);_reviewBeforeObserved=true;}}
            catch(Exception error){FailReceived("입력 갱신형식 실패: "+error.Message);}
        }
        private void ObserveAfterInputUpdate()
        {
            if(!Application.isPlaying||_sealed||_receivedPermanentlyBlocked)return;
            try{if(IsPlayerInputUpdate()&&CheckReceivedBoundary())
                {if(_pausePhase==5){GuardInputRoute();WriteInputRoute("first-dynamic-after-resume",2);}
                    if(_reviewBeforeObserved&&!_reviewAfterObserved){if((ulong)InputState.updateCount!=_reviewDynamicOrdinal)throw new IOException("첫 Dynamic before/after 서수가 다릅니다.");_authoringHost.ObserveReviewDynamicAfterForAuthoring(InputState.updateCount);_reviewAfterObserved=true;}}}
            catch(Exception error){FailReceived("입력 갱신 종료 실패: "+error.Message);}
        }
        private void ObserveReceivedInput(InputEventPtr inputEvent,InputDevice device)
        {
            if(!Application.isPlaying||_sealed||_receivedPermanentlyBlocked)return;
            if(OriginalRouteDevice(device!=null,ReferenceEquals(device,_receivedKeyboard),ReferenceEquals(device,_receivedMouse)))
            {
                try{checked{_routeNativeEvents++;if(InputState.currentUpdateType==InputUpdateType.Dynamic)_routeNativeDynamic++;else if(InputState.currentUpdateType==InputUpdateType.Editor)_routeNativeEditor++;else _routeNativeOther++;if(inputEvent.IsA<StateEvent>())_routeStateEvents++;if(inputEvent.IsA<DeltaStateEvent>())_routeDeltaEvents++;}_routeLastNativeStamp=Stopwatch.GetTimestamp();if(_pausePhase==5)WriteInputRoute("first-native-event-after-resume",4);}
                catch(Exception error){FailReceived("입력 경로 원시 실패: "+error.Message);return;}
            }
            try{if(!IsPlayerInputUpdate()||!CheckReceivedBoundary(true))return;}catch(Exception error){FailReceived("입력 사건 갱신형식 실패: "+error.Message);return;}
            if(!ReferenceEquals(device,_receivedKeyboard) && !ReferenceEquals(device,_receivedMouse))return;
            var full=inputEvent.IsA<StateEvent>();var delta=inputEvent.IsA<DeltaStateEvent>();
            if(!full && !delta)return;
            try
            {
                if(inputEvent.deviceId!=device.deviceId)throw new IOException("허용 장치 사건 신원이 다릅니다.");
                var time=inputEvent.time;if(double.IsNaN(time) || double.IsInfinity(time))throw new IOException("입력 event 시각이 유한값이 아닙니다.");
                var controls=0;var pressed=0;
                checked{_routeControlBefore++;}
                if(ReferenceEquals(device,_receivedKeyboard))
                {
                    ReadReceivedButton(_receivedKeyboard.aKey,0,inputEvent,full,ref controls,ref pressed);
                    ReadReceivedButton(_receivedKeyboard.dKey,1,inputEvent,full,ref controls,ref pressed);
                    ReadReceivedButton(_receivedKeyboard.spaceKey,2,inputEvent,full,ref controls,ref pressed);
                    ReadReceivedButton(_receivedKeyboard.leftShiftKey,3,inputEvent,full,ref controls,ref pressed);
                    ReadReceivedButton(_receivedKeyboard.rightShiftKey,4,inputEvent,full,ref controls,ref pressed);
                    ReadReceivedButton(_receivedKeyboard.qKey,5,inputEvent,full,ref controls,ref pressed);
                    ReadReceivedButton(_receivedKeyboard.eKey,6,inputEvent,full,ref controls,ref pressed);
                }
                else
                {
                    ReadReceivedButton(_receivedMouse.leftButton,7,inputEvent,full,ref controls,ref pressed);
                    ReadReceivedButton(_receivedMouse.rightButton,8,inputEvent,full,ref controls,ref pressed);
                }
                if(controls==0)
                {
                    FailReceived("delta 허용 control 전부 미판독: 미포함 또는 형식 불일치 원인은 미확정이며 완전성을 복구하지 않습니다.");
                    return;
                }
                var known=_receivedKnownMask & controls;
                checked{_routeControlAfter++;}
                var changed=known & (_receivedPressedMask ^ pressed);
                WriteReceivedRow(new ReceivedInputRow{sessionId=_configuration.sessionId,caseId=_configuration.caseId,
                    inputEvidenceKind=_configuration.inputEvidenceKind,performer=_configuration.performer,gameViewInstanceIdDecimal=_configuration.gameViewInstanceIdDecimal,
                    receivedOrdinal=_receivedOrdinal+1,observedStopwatchTicks=Stopwatch.GetTimestamp(),stopwatchFrequency=Stopwatch.Frequency,eventTime=time,
                    lastCompletedTick=_hasSample?_lastTick:-1,sampleCollectorLive=_live,deviceKind=ReferenceEquals(device,_receivedKeyboard)?"keyboard":"mouse",
                    controlMask=controls,pressedMask=pressed,knownBeforeMask=known,changedMask=changed});
                _receivedOrdinal++;_receivedKnownMask|=controls;_receivedPressedMask=(_receivedPressedMask & ~controls)|pressed;
            }
            catch(Exception error){FailReceived("허용 입력 판독·기록 실패: "+error.Message);}
        }
        private static void ReadReceivedButton(ButtonControl control,int bit,InputEventPtr inputEvent,bool full,ref int controls,ref int pressed)
        {
            if(!control.ReadValueFromEvent(inputEvent,out var value))
            {
                if(full)throw new IOException("정상 상태 사건의 허용 control을 읽지 못했습니다.");
                // delta 미포함은 상태를 바꾸지 않는다. 공개 safe API의 format 구별 한계는 미수용이다.
                return;
            }
            if(float.IsNaN(value) || float.IsInfinity(value))throw new IOException("허용 control 값이 유한값이 아닙니다.");
            controls|=1<<bit;if(control.IsValueConsideredPressed(value))pressed|=1<<bit;
        }
        private void WriteReceivedRow(object row)
        {
            if(_receivedInput==null)throw new IOException("입력 단일 작성자가 없습니다.");
            var bytes=_utf8.GetBytes(JsonUtility.ToJson(row)+"\n");
            if(bytes.Length>MaxRowBytes || _receivedBytes>MaxEventBytes-bytes.Length)throw new IOException("입력 행·파일 상한입니다.");
            _receivedInput.Write(bytes,0,bytes.Length);_receivedInput.Flush();_receivedBytes+=bytes.Length;_receivedRows++;
        }
        private void WriteReceivedStatus(string kind,string reason)=>WriteReceivedRow(new ReceivedInputStatusRow
        {kind=kind,reason=reason,utc=DateTime.UtcNow.ToString("O"),sessionId=_configuration.sessionId,caseId=_configuration.caseId,
            inputEvidenceKind=_configuration.inputEvidenceKind,performer=_configuration.performer,gameViewInstanceIdDecimal=_configuration.gameViewInstanceIdDecimal,
            observedStopwatchTicks=Stopwatch.GetTimestamp(),stopwatchFrequency=Stopwatch.Frequency,receivedOrdinal=_receivedOrdinal,
            rows=_receivedRows,bytes=_receivedBytes,errors=_receivedErrors,subscriptions=_receivedSubscriptions,unsubscriptions=_receivedUnsubscriptions,
            registered=_receivedSubscribed,incomplete=_receivedPermanentlyBlocked});
        internal void RejectReceivedInputForAuthoring(string reason){if(!_sealed)FailReceived(reason);}
        private void FailReceived(string reason)
        {
            _receivedPermanentlyBlocked=true;_receivedErrors++;
            if(_startStamp==0)_startStamp=Stopwatch.GetTimestamp();
            try{if(_receivedInput!=null)WriteReceivedStatus("input-incomplete",reason);}catch(Exception error){_sealError=error.Message;}
            Fail(reason);
        }
        private void UnregisterReceivedCallbacks()
        {
            var pair=_receivedEventSubscribed && _receivedUpdateSubscribed && _receivedAfterSubscribed;
            if(_receivedAfterSubscribed)
            {try{InputSystem.onAfterUpdate-=ObserveAfterInputUpdate;_receivedAfterSubscribed=false;}catch(Exception error){_receivedPermanentlyBlocked=true;_receivedErrors++;_sealError="입력 종료 경계 해제 실패: "+error.Message;}}
            if(_receivedEventSubscribed)
            {
                try{InputSystem.onEvent-=ObserveReceivedInput;_receivedEventSubscribed=false;}
                catch(Exception error){_receivedPermanentlyBlocked=true;_receivedErrors++;_sealError="입력 사건 구독 해제 실패: "+error.Message;}
            }
            if(_receivedUpdateSubscribed)
            {
                try{InputSystem.onBeforeUpdate-=ObserveBeforeInputUpdate;_receivedUpdateSubscribed=false;}
                catch(Exception error){_receivedPermanentlyBlocked=true;_receivedErrors++;_sealError="입력 경계 구독 해제 실패: "+error.Message;}
            }
            _receivedSubscribed=_receivedEventSubscribed || _receivedUpdateSubscribed;
            if(pair && !_receivedSubscribed)_receivedUnsubscriptions++;
        }
        private void CloseReceivedInput(string reason)
        {
            UnregisterReceivedCallbacks();
            var stream=_receivedInput;if(stream==null)return;
            try{RecordPlayTransitionForAuthoring("input-close","finalCounts=true");}catch(Exception error){_receivedPermanentlyBlocked=true;_receivedErrors++;_sealError=error.Message;}
            try{WriteReceivedStatus("input-end",reason);}catch(Exception error){_receivedErrors++;_receivedPermanentlyBlocked=true;_sealError=error.Message;}
            _receivedInput=null;
            var closed=true;
            try{stream.Flush();}catch(Exception error){closed=false;_receivedErrors++;_receivedPermanentlyBlocked=true;_sealError=error.Message;}
            try{stream.Dispose();}catch(Exception error){closed=false;_receivedErrors++;_receivedPermanentlyBlocked=true;_sealError=error.Message;}
            _receivedFileClosed=closed;
        }

        private void Start()
        {
            if(!Application.isPlaying || _started)return;
            if(!CheckReceivedBoundary())return;
            _started=true;if(_startStamp==0)_startStamp=Stopwatch.GetTimestamp();
            try
            {
                if(!_configured || string.IsNullOrEmpty(_configurationJson) || _authoringHost==null ||
                    transform.parent!=null || !isActiveAndEnabled || _authoringHost.gameObject.scene!=gameObject.scene ||
                    gameObject.scene.GetRootGameObjects().Length==0 ||
                    FindObjectsByType<Chapter1BasicMovementTickObserver>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length!=1)
                    throw new InvalidOperationException("재로드된 관측 bootstrap이 없거나 중복입니다.");
                _configuration=JsonUtility.FromJson<Configuration>(_configurationJson);
                ValidateConfiguration();
                if(_configuration.sourceFiles.Length!=590)throw new IOException("현재 소스590개가 아닙니다.");
                _sourceWriteTicks=new long[590];_sourceMetadata=new FileInfo[590];_sourceMetadataPaths=new string[590];
                for(var i=0;i<_configuration.sourceFiles.Length;i++)
                {var pin=_configuration.sourceFiles[i];VerifyPin(pin,_configuration.projectPath);var info=new FileInfo(pin.path);info.Refresh();if(!info.Exists||info.Length!=pin.length)throw new IOException("시작 소스 스냅샷이 다릅니다.");_sourceMetadata[i]=info;_sourceMetadataPaths[i]=info.FullName;_sourceWriteTicks[i]=info.LastWriteTimeUtc.Ticks;}
                _samples=new FileStream(_configuration.samplesPath,FileMode.CreateNew,FileAccess.Write,FileShare.Read);
                _events=new FileStream(_configuration.eventsPath,FileMode.CreateNew,FileAccess.Write,FileShare.Read);
                _live=true;WriteEvent("start",null,true);
                if(_configuration.caseId=="R01-A")WritePauseTransition("first-sample-wait");
            }
            catch(Exception error){Fail("시작 기록 실패: "+error.Message);}
        }

        private void ValidateConfiguration()
        {
            var c=_configuration;
            if(c==null || c.schemaVersion!=1 || string.IsNullOrEmpty(c.sessionId) || !ValidCase(c.caseId,c.roomId,c.socketId) ||
                c.sceneName!=gameObject.scene.name || c.collectorAttributeOrder!=210 || c.lifetimeAttributeOrder!=200 ||
                c.caseBudgetSeconds!=1800 || c.totalBudgetSeconds!=18000 || c.caseElapsedSecondsAtInstall<0 || c.caseElapsedSecondsAtInstall>=1800 || c.totalElapsedSecondsAtInstall<0 ||
                c.totalElapsedSecondsAtInstall>=c.totalBudgetSeconds || !c.inputNeutralObserved || c.performer!="User" ||
                c.unityPid<=0 || c.unityStartTicks<=0 || string.IsNullOrEmpty(c.windowIdentity) || string.IsNullOrEmpty(c.inputTarget) ||
                c.performerModel!="" || c.performerRole!="사용자 직접 플레이" || c.inputEvidenceKind!="UnityReceivedAllowedControls" || string.IsNullOrEmpty(c.unityExe) ||
                c.sourceFiles==null || c.sourceFiles.Length!=590 || c.unityVersion!="6000.6.0f1")
                throw new InvalidOperationException("승인된 관측 스칼라와 실제 창·예산 결속이 없습니다.");
            ParseGameViewIdDecimal(c.gameViewInstanceIdDecimal);
            var project=Path.GetFullPath(Path.Combine(Application.dataPath,".."));
            if(!SamePath(project,c.projectPath))throw new InvalidOperationException("실제 프로젝트가 다릅니다.");
            using(var current=Process.GetCurrentProcess())
                if(current.Id!=c.unityPid || current.StartTime.ToUniversalTime().Ticks!=c.unityStartTicks ||
                    !SamePath(current.MainModule.FileName,c.unityExe) || Application.unityVersion!=c.unityVersion)
                    throw new InvalidOperationException("실제 Unity PID·StartTicks·EXE·버전이 승인 원문과 다릅니다.");
            var root=Path.GetFullPath(Path.Combine(project,"..","evidence"));
            if(!SamePath(root,c.evidenceRoot))throw new InvalidOperationException("원시 루트가 다릅니다.");
            RequireNewRawPath(c.samplesPath,root);RequireNewRawPath(c.eventsPath,root);RequireNewRawPath(c.sealPath,root);
            RequireInside(c.nativeEvidencePath,root);RequireInside(c.inputEvidencePath,root);
            RequireDistinctRawPaths(c.samplesPath,c.eventsPath,c.sealPath,c.nativeEvidencePath,c.inputEvidencePath);
            VerifyPin(c.sourceCheckpoint,root);VerifyPin(c.executionApproval,Path.GetDirectoryName(root));
            VerifyPin(c.toolObservation,Path.GetDirectoryName(root));VerifyApprovedContract(c.approvedContract,project);
            VerifyPin(c.observerSource,project);VerifyPin(c.observerMeta,project);VerifyPin(c.windowSource,project);VerifyPin(c.windowMeta,project);
            VerifyPin(c.prefab,project);VerifyPin(c.prefabMeta,project);
            if(c.reviewMenu!="AcadeGameMaker/Chapter1/Review Basic Movement/"+MenuSuffix(c.caseId))
                throw new InvalidOperationException("실제 구성 메뉴가 다릅니다.");
        }

        private void FixedUpdate()
        {
            if(FirstPauseWaitingForAuthoring){VerifyFirstPauseWaitForAuthoring();return;}
            if(_authoringHost!=null&&_authoringHost.ReviewGraphWaitingForAuthoring)
            {try{_authoringHost.VerifyReviewWaitForAuthoring();}catch(Exception error){FailReceived(error.Message);}return;}
            if(Application.isPlaying && !CheckReceivedBoundary())return;
            if(!_live || _sealed)return;
            _fixedCalls++;
            var began=Stopwatch.GetTimestamp();
            try
            {
                if(_configuration.caseElapsedSecondsAtInstall+Elapsed>=_configuration.caseBudgetSeconds || _configuration.totalElapsedSecondsAtInstall+Elapsed>=_configuration.totalBudgetSeconds)
                    throw new InvalidOperationException("관측 시간 상한에 도달했습니다.");
                CheckSourceMetadata();
                if(_authoringHost==null || !_authoringHost.isActiveAndEnabled || _authoringHost.IsFailed || _authoringHost.IsReplacementPending ||
                    _authoringHost.gameObject.scene!=gameObject.scene ||
                    FindObjectsByType<Chapter1EncounterLifetimeOwner>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length!=1 ||
                    FindObjectsByType<Chapter1BasicMovementTickObserver>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length!=1)
                    throw new InvalidOperationException("같은 활성 검수 host가 폐쇄·교체·중복됐습니다.");
                var router=_authoringHost.CurrentRouter;var hazards=_authoringHost.CurrentHazards;
                if(router==null || hazards==null)
                { InitialUnavailable(false,"아직 실제 graph가 없습니다.");return; }
                if(!router.isActiveAndEnabled || !router.IsChapter1Regular || router.IsFaulted || !hazards.isActiveAndEnabled)
                    throw new InvalidOperationException("실제 regular producer가 비활성 또는 실패 상태입니다.");
                var player=router.BoundPlayerForAuthoring;var combat=router.BoundChapter1Combat;var transfer=router.BoundTransferForAuthoring;
                if(player==null || combat==null || transfer==null || !player.isActiveAndEnabled || !combat.isActiveAndEnabled || !transfer.isActiveAndEnabled ||
                    !ReferenceEquals(combat.BoundPlayer,player) || !ReferenceEquals(combat.BoundTransferDriver,transfer) || !ReferenceEquals(transfer.BoundPlayer,player) ||
                    router.transform.root!=hazards.transform.root || router.transform.root!=player.transform.root || router.transform.root!=combat.transform.root ||
                    router.transform.root!=transfer.transform.root || router.transform.root==transform ||
                    router.transform.root.GetComponentsInChildren<InputRouter>(true).Length!=1 ||
                    router.transform.root.GetComponentsInChildren<PlayerMovementController>(true).Length!=1 ||
                    router.transform.root.GetComponentsInChildren<CombatSimulationDriver>(true).Length!=1 ||
                    router.transform.root.GetComponentsInChildren<TransferSimulationDriver>(true).Length!=1 ||
                    router.transform.root.GetComponentsInChildren<Chapter1HazardObservation>(true).Length!=1)
                    throw new InvalidOperationException("실제 graph의 활성·유일성·소비자 참조가 다릅니다.");
                var gotInput=router.TryReadChapter1PublishedObservationForAuthoring(out var receipt,out var health);
                var gotHazards=hazards.TryReadPublishedHazardsForAuthoring(out var kill,out var crush);
                var phase=transfer.LatestPhasePublication;var gotCombat=combat.HasLatestOutcomeForEncounterHandoff;
                if(!gotInput || !gotHazards || !gotCombat || !phase.HasValue)
                { InitialUnavailable(gotInput || gotHazards || gotCombat || phase.HasValue,"필수 완료 원문이 없습니다.");return; }
                if(!player.TryReadExistingMotionForAuthoring(out var motion,out var next))
                    throw new InvalidOperationException("기존 모터의 저장 관측이 없습니다.");
                var outcome=combat.LatestOutcomeForEncounterHandoff;
                var state=phase.Value.Snapshot;
                var tick=receipt.Tick.Value;
                if(tick<0 || tick==int.MaxValue || motion.Tick!=receipt.Tick || outcome.Tick!=receipt.Tick || phase.Value.Tick!=receipt.Tick ||
                    state.Tick!=receipt.Tick || next.Value!=tick+1 || _authoringHost.Generation!=health.Generation ||
                    string.IsNullOrEmpty(health.RunKey) || health.Generation<=0 || !PublicationMatches(health,router,health.RunKey,health.Generation,receipt) ||
                    !PublicationMatches(kill,router,health.RunKey,health.Generation,receipt) || !PublicationMatches(crush,router,health.RunKey,health.Generation,receipt) ||
                    !ReferenceEquals(health.Issuer,combat) || !ReferenceEquals(kill.Issuer,hazards) || !ReferenceEquals(crush.Issuer,hazards))
                    throw new InvalidOperationException("완료 tick·키·세대·세 원본 신원이 일치하지 않습니다.");
                CombatantSnapshot? playerHealth=null;
                foreach(var item in outcome.Combatants)if(item.TargetId=="player")
                { if(playerHealth.HasValue)throw new InvalidOperationException("완료 player가 중복입니다.");playerHealth=item; }
                if(!playerHealth.HasValue || health.Triggered!=(playerHealth.Value.CurrentHealth==0))
                    throw new InvalidOperationException("완료 player 체력과 원래 Health가 다릅니다.");
                if(!router.TryReadChapter1PublishedObservationForAuthoring(out var receiptAfter,out var healthAfter) ||
                    !hazards.TryReadPublishedHazardsForAuthoring(out var killAfter,out var crushAfter) || !receipt.Equals(receiptAfter) ||
                    !ReferenceEquals(health,healthAfter) || !ReferenceEquals(kill,killAfter) || !ReferenceEquals(crush,crushAfter) ||
                    health.Revision!=healthAfter.Revision || kill.Revision!=killAfter.Revision || crush.Revision!=crushAfter.Revision ||
                    !ReferenceEquals(router,_authoringHost.CurrentRouter) || !ReferenceEquals(hazards,_authoringHost.CurrentHazards) ||
                    _authoringHost.Generation!=health.Generation || _authoringHost.IsFailed || _authoringHost.IsReplacementPending ||
                    !ReferenceEquals(player,router.BoundPlayerForAuthoring) || !ReferenceEquals(combat,router.BoundChapter1Combat) ||
                    !ReferenceEquals(transfer,router.BoundTransferForAuthoring))
                    throw new InvalidOperationException("전후 반복 읽기의 실제 원문이 달라졌습니다.");
                if(_hasSample && (!ReferenceEquals(_router,router) || !ReferenceEquals(_hazards,hazards) || !ReferenceEquals(_player,player) ||
                    !ReferenceEquals(_combat,combat) || !ReferenceEquals(_transfer,transfer) || _runKey!=health.RunKey || _generation!=health.Generation))
                    throw new InvalidOperationException("같은 관측 사례의 source·키·세대가 교체됐습니다.");
                if(_hasSample && tick==_lastTick)
                {
                    if(!_lastReceipt.Equals(receipt) || !ReferenceEquals(_health,health) || !ReferenceEquals(_kill,kill) || !ReferenceEquals(_crush,crush))
                        throw new InvalidOperationException("중복 tick의 원본이 바뀌었습니다.");
                    _duplicates++;WriteEvent("duplicate",null,false);return;
                }
                if((!_hasSample && tick!=0) || (_hasSample && tick!=_lastTick+1))
                    throw new InvalidOperationException("첫 tick0 또는 이후 연속 tick을 놓쳤습니다.");
                if(_sampleCount>=MaxSamples)throw new InvalidOperationException("서로 다른 표본 수 상한에 도달했습니다.");
                var capsule=player.GetComponent<CapsuleCollider2D>();
                if(capsule==null)throw new InvalidOperationException("실제 player capsule이 없습니다.");
                _router=router;_hazards=hazards;_player=player;_combat=combat;_transfer=transfer;
                _runKey=health.RunKey;_generation=health.Generation;_lastTick=tick;_lastReceipt=receipt;
                _health=health;_kill=kill;_crush=crush;_hasSample=true;_sampleCount++;
                var size=Vector2.Scale(capsule.size,new Vector2(player.transform.lossyScale.x,player.transform.lossyScale.y));
                var center=new Vector2(motion.PositionXQ4096/4096f,motion.PositionYQ4096/4096f)+
                    Vector2.Scale(capsule.offset,new Vector2(player.transform.lossyScale.x,player.transform.lossyScale.y));
                var graphPosition=router.transform.root.InverseTransformPoint(new Vector3(motion.PositionXQ4096/4096f,motion.PositionYQ4096/4096f,0));
                var exit=graphPosition.x>=30 && graphPosition.x<=32 && graphPosition.y>=0 && graphPosition.y<=3 &&
                    playerHealth.Value.CurrentHealth>0 && !playerHealth.Value.IsDead && !health.Triggered && !kill.Triggered && !crush.Triggered &&
                    state.PlayerState==TransferPlayerState.Baseline && string.IsNullOrEmpty(state.ActiveTargetId) && !phase.Value.Input.HadPress;
                if(exit && _exitTick<0)_exitTick=tick;
                var row=new SampleRow
                {
                    utc=DateTime.UtcNow.ToString("O"),monotonicStamp=Stopwatch.GetTimestamp(),elapsedSeconds=Elapsed,
                    sessionId=_configuration.sessionId,caseId=_configuration.caseId,roomId=_configuration.roomId,socketId=_configuration.socketId,
                    runKey=_runKey,generation=_generation,tick=tick,frameOrdinal=receipt.FrameOrdinal,modeEpoch=receipt.ModeEpoch,mode=receipt.Mode.ToString(),
                    fixedCall=_fixedCalls,displayCall=_displayCalls,distinctCount=_sampleCount,duplicateCount=_duplicates,unavailableCount=_unavailable,
                    frameCount=Time.frameCount,hostId=EntityId.ToULong(_authoringHost.GetEntityId()),routerId=EntityId.ToULong(router.GetEntityId()),hazardsId=EntityId.ToULong(hazards.GetEntityId()),
                    playerId=EntityId.ToULong(player.GetEntityId()),combatId=EntityId.ToULong(combat.GetEntityId()),transferId=EntityId.ToULong(transfer.GetEntityId()),collectorId=EntityId.ToULong(GetEntityId()),sceneHandle=gameObject.scene.handle.GetRawData(),
                    hostHierarchy=Hierarchy(_authoringHost.transform),graphHierarchy=Hierarchy(router.transform.root),nextExpectedTick=next.Value,
                    health=playerHealth.Value.CurrentHealth,isDead=playerHealth.Value.IsDead,invulnerabilityEndTick=playerHealth.Value.InvulnerabilityEndTick,
                    positionXQ4096=motion.PositionXQ4096,positionYQ4096=motion.PositionYQ4096,velocityXQ4096=motion.VelocityXQ4096,velocityYQ4096=motion.VelocityYQ4096,
                    isGrounded=motion.IsGrounded,wallId=motion.WallId,wallSide=motion.WallSide.ToString(),actionState=motion.ActionState.ToString(),airDashAvailable=motion.IsAirDashAvailable,
                    bodyCenterX=center.x,bodyCenterY=center.y,bodyWidth=size.x,bodyHeight=size.y,bodyFootY=center.y-size.y/2,
                    transferHighlightedTarget=state.HighlightedTargetId,transferActiveTarget=state.ActiveTargetId,transferPlayerState=state.PlayerState.ToString(),
                    transferRevision=state.Revision,transferTransitionLockEndTick=state.TransitionLockEndTick,transferLastFailure=state.LastFailureReason.ToString(),
                    hadAim=phase.Value.Input.HadAim,hadPress=phase.Value.Input.HadPress,transferRemovalCount=phase.Value.Input.RemovalCount,
                    transferLifecycleReason=phase.Value.Input.LifecycleReason.HasValue?phase.Value.Input.LifecycleReason.Value.ToString():null,
                    transferLifecycleClear=phase.Value.LifecycleClear.HasValue?phase.Value.LifecycleClear.Value.Reason.ToString():null,
                    transferLifecyclePreviousTarget=phase.Value.LifecycleClear.HasValue?phase.Value.LifecycleClear.Value.PreviousTargetId:null,
                    transferLifecycleTick=phase.Value.LifecycleClear.HasValue?phase.Value.LifecycleClear.Value.Tick.Value:-1,
                    transferLifecycleRevision=phase.Value.LifecycleClear.HasValue?phase.Value.LifecycleClear.Value.Revision:-1,
                    sameSourcesBeforeAfter=true,firstTickZero=true,exitCandidate=exit,
                    healthPublication=Publication(health,router.OwnsChapter1HealthPublication(health),combat,router),
                    killPlanePublication=Publication(kill,hazards.Owns(kill),hazards,router),lethalCrushPublication=Publication(crush,hazards.Owns(crush),hazards,router),
                    readMilliseconds=Milliseconds(began),sourceMetadataMilliseconds=_sourceMetadataMilliseconds,previousAppendFlushMilliseconds=_previousWriteMilliseconds,streamBytes=_sampleBytes
                };
                var json=BoundedJson(row);_latestJson=json;
                Append(_samples,json,MaxSampleBytes,ref _sampleBytes);
                if(health.Triggered || kill.Triggered || crush.Triggered || row.isDead || row.health<=0)
                    Fail("실제 위험 원본 또는 사망이 처음 관측됐습니다.");
                else if(row.hadPress || row.transferPlayerState!=TransferPlayerState.Baseline.ToString() || !string.IsNullOrEmpty(row.transferActiveTarget))
                    Fail("기본 이동 범위 밖 전이 입력·상태가 관측됐습니다.");
                else if(_configuration.caseId=="R01-A"&&_sampleCount==1)
                {_pauseFlushSucceeded=true;RequestFirstSamplePause(row);}
            }
            catch(Exception error){Fail("완료 관측 실패: "+error.Message);}
        }

        private void InitialUnavailable(bool anyCompleted,string reason)
        {
            if(_hasSample || anyCompleted || Elapsed>60)throw new InvalidOperationException(reason+" 기다림으로 완료를 보충하지 않습니다.");
            _unavailable++;WriteEvent("unavailable",reason,false);
        }
        private static bool PublicationMatches(Chapter1FailurePublication value,InputRouter router,string key,long generation,InputFrameCommitReceipt receipt)=>
            value!=null && ReferenceEquals(value.Router,router) && value.RunKey==key && value.Generation==generation && value.Revision>0 && value.Receipt.Equals(receipt);
        private PublicationRow Publication(Chapter1FailurePublication publication,bool owned,object issuer,InputRouter router)=>new PublicationRow
        {localPublicationNumber=checked(++_publicationSerial),revision=publication.Revision,issuerNumber=Identity(publication.Issuer),tokenNumber=Identity(publication.Token),
            routerNumber=Identity(publication.Router),kind=publication.Kind.ToString(),triggered=publication.Triggered,ownedByStoredSlot=owned,
            issuerSame=ReferenceEquals(publication.Issuer,issuer),routerSame=ReferenceEquals(publication.Router,router)};
        private int Identity(object value)
        {
            if(value==null)throw new InvalidOperationException("고정 원본 신원이 없습니다.");
            for(var i=0;i<_identityCount;i++)if(ReferenceEquals(_identities[i],value))return i+1;
            if(_identityCount==MaxIdentities)throw new InvalidOperationException("고정 식별표 상한에 도달했습니다.");
            _identities[_identityCount]=value;return ++_identityCount;
        }
        internal void ObserveDisplayUpdateForAuthoring()
        {if(_started&&!_sealed){_displayCalls++;try{if(_authoringHost!=null)_authoringHost.VerifyReviewWaitForAuthoring();}catch(Exception error){FailReceived(error.Message);}}}
        internal void SealForAuthoring(string reason,bool normalStop)
        {
            if((!_started && _receivedInput==null) || _sealed)return;
            if(!normalStop || !_hasSample || _exitTick<0 || _firstFailureJson!=null)Fail(reason);
            else Seal(reason);
        }
        private void Fail(string reason)
        {
            if(!_pauseFailureRecorded&&_configuration!=null&&_configuration.caseId=="R01-A"&&_events!=null)
            {_pauseFailureRecorded=true;_pausePhase=6;try{_pauseAfter=_receivedContext!=null&&_receivedContext.IsActualPlayPaused();WritePauseTransition(reason);}catch(Exception error){_sealError=error.Message;}}
            _live=false;
            if(_firstFailureJson==null)
            {
                try{_firstFailureJson=BoundedJson(Event("firstFailure",reason,false));}
                catch{_firstFailureJson="{\"kind\":\"firstFailure\",\"reason\":\"실패 행 직렬화 소실\"}";}
                try{if(_events!=null)Append(_events,_firstFailureJson,MaxEventBytes,ref _eventBytes);}catch(Exception error){_sealError=error.Message;}
            }
            Seal(reason);
        }
        private void Seal(string reason)
        {
            if(_sealed)return;
            try{if(_events!=null&&_configuration!=null)WriteInputRoute("failure-or-seal",8);}catch(Exception error){_sealError=error.Message;}
            if(_configuration!=null&&_configuration.caseId=="R01-A"&&_pausePhase==5&&_events!=null)
            {_pausePhase=7;try{WritePauseTransition("closed-after-explicit-resume");}catch(Exception error){_sealError=error.Message;}}
            _sealed=true;_live=false;
            try{if(_authoringHost!=null)_authoringHost.CloseReviewGateForAuthoring();}catch(Exception error){_sealError=error.Message;}
            CloseReceivedInput(reason);
            var sourcesUnchanged=false;
            try
            {
                if(_configuration!=null && _configuration.sourceFiles!=null)
                { foreach(var pin in _configuration.sourceFiles)VerifyPin(pin,_configuration.projectPath);sourcesUnchanged=true; }
            }
            catch(Exception error)
            {
                if(_firstFailureJson==null)_firstFailureJson=BoundedJson(Event("firstFailure","봉인 원문 변경: "+error.Message,false));
                _sealError=error.Message;
            }
            try{if(_events!=null)WriteEvent("end",reason,false);}catch(Exception error){_sealError=error.Message;}
            CloseStream(ref _samples);CloseStream(ref _events);
            if(_sealError!=null && _firstFailureJson==null)_firstFailureJson=BoundedJson(Event("firstFailure","원시 기록·정리 오류: "+_sealError,false));
            if(_configuration==null)return;
            try
            {
                var row=new SealRow
                {utc=DateTime.UtcNow.ToString("O"),sessionId=_configuration.sessionId,caseId=_configuration.caseId,reason=reason,
                    runKey=_runKey,generation=_generation,samples=_sampleCount,duplicates=_duplicates,unavailable=_unavailable,
                    firstTick=_hasSample?0:-1,lastTick=_lastTick,exitCandidateTick=_exitTick,continuousFromZero=_hasSample && _sampleCount==(long)_lastTick+1,
                    sourcePinsUnchanged=sourcesUnchanged,firstFailureJson=_firstFailureJson,lastSampleJson=_latestJson,sealError=_sealError,
                    sourceCheckpointSha256=_configuration.sourceCheckpoint==null?null:_configuration.sourceCheckpoint.sha256,
                    observerHelperSha256=_configuration.observerSource==null?null:_configuration.observerSource.sha256,
                    prefabSha256=_configuration.prefab==null?null:_configuration.prefab.sha256,prefabMetaSha256=_configuration.prefabMeta==null?null:_configuration.prefabMeta.sha256,
                    fixedCalls=_fixedCalls,displayCalls=_displayCalls,cleanupCalls=_cleanupCalls,elapsedSeconds=Elapsed,
                    completionKey=_hasSample && _exitTick>=0 && _firstFailureJson==null && sourcesUnchanged && _receivedFileClosed && !_receivedPermanentlyBlocked && _receivedReady && _receivedBoundary && _receivedErrors==0 && _receivedSubscriptions==2 && _receivedUnsubscriptions==2?
                        _configuration.sessionId+"|"+_configuration.caseId+"|"+_runKey+"|"+_generation+"|0|"+_lastTick+"|"+_lastReceipt.FrameOrdinal+"|"+_lastReceipt.ModeEpoch:null,
                    samplesFile=ExistingPin(_configuration.samplesPath),eventsFile=ExistingPin(_configuration.eventsPath),
                    inputFile=_receivedFileClosed?ExistingPin(_configuration.inputEvidencePath):null,inputFileClosed=_receivedFileClosed,
                    inputObservationComplete=_receivedReady && _receivedBoundary && !_receivedPermanentlyBlocked && _receivedFileClosed && _receivedErrors==0 && _receivedSubscriptions==2 && _receivedUnsubscriptions==2,
                    inputIncomplete=_receivedPermanentlyBlocked,inputRows=_receivedRows,inputBytes=_receivedBytes,receivedCount=_receivedOrdinal,
                    inputErrors=_receivedErrors,inputSubscriptions=_receivedSubscriptions,inputUnsubscriptions=_receivedUnsubscriptions,
                    inputPauseCount=_receivedPauseCount,inputObservationScope="Play&&!Pause",
                    ioLimitation="동기 append/flush는 실제 게임 실행을 지연할 수 있으며 입력·tick·시각을 보정하지 않습니다. inputObservationComplete는 Play&&!Pause 활성 구간의 등록·폐쇄 상태에만 한정하며 Pause 입력·누락0 또는 전구간 완전성 증명이 아닙니다. 출구 후보와 완주키는 독립 경로·입력 수용이 아닙니다."};
                RequireNewRawPath(_configuration.sealPath,_configuration.evidenceRoot);
                using(var stream=new FileStream(_configuration.sealPath,FileMode.CreateNew,FileAccess.Write,FileShare.Read))
                {var bytes=_utf8.GetBytes(JsonUtility.ToJson(row)+"\n");if(bytes.Length>MaxEventBytes)throw new IOException("봉인 원시 상한입니다.");stream.Write(bytes,0,bytes.Length);stream.Flush();}
            }
            catch(Exception error){_sealError=error.Message;}
        }
        private void OnDisable()
        {
            if(ExpectedPlaySuspendForAuthoring&&!_sealed)
            {try{SuspendExpectedPlayForAuthoring();return;}catch(Exception error){FailReceived("정상 Play 중단 실패: "+error.Message);return;}}
            _cleanupCalls++;if((_receivedSubscriptions>0 || _receivedEventSubscribed || _receivedUpdateSubscribed || _started || _receivedInput!=null) && !_sealed)FailReceived("관측기 비활성 또는 Play 종료");else UnregisterReceivedCallbacks();
        }
        private void OnDestroy(){_cleanupCalls++;if((_receivedSubscriptions>0 || _receivedEventSubscribed || _receivedUpdateSubscribed || _started || _receivedInput!=null) && !_sealed)FailReceived("관측기 파기");else UnregisterReceivedCallbacks();}
        private void CloseStream(ref FileStream stream)
        {var current=stream;stream=null;if(current==null)return;try{current.Flush();}catch(Exception error){_sealError=error.Message;}try{current.Dispose();}catch(Exception error){_sealError=error.Message;}}
        private EventRow Event(string kind,string reason,bool includeConfiguration)=>new EventRow
        {kind=kind,reason=reason,utc=DateTime.UtcNow.ToString("O"),monotonicStamp=Stopwatch.GetTimestamp(),elapsedSeconds=Elapsed,
            sessionId=_configuration==null?null:_configuration.sessionId,caseId=_configuration==null?null:_configuration.caseId,
            runKey=_runKey,generation=_generation,fixedCall=_fixedCalls,displayCall=_displayCalls,cleanupCall=_cleanupCalls,
            frameOrdinal=_hasSample?_lastReceipt.FrameOrdinal:-1,modeEpoch=_hasSample?_lastReceipt.ModeEpoch:-1,
            healthRevision=_health==null?0:_health.Revision,killRevision=_kill==null?0:_kill.Revision,crushRevision=_crush==null?0:_crush.Revision,
            samples=_sampleCount,duplicates=_duplicates,unavailable=_unavailable,lastTick=_lastTick,frameCount=Time.frameCount,live=_live,
            previousAppendFlushMilliseconds=_previousWriteMilliseconds,configuration=includeConfiguration?_configuration:null};
        private void WriteEvent(string kind,string reason,bool configuration)
        {
            // 시작 행은 전체 현재 파일 핀을 담아 별도의 유한 사건 상한을 사용한다.
            var json=JsonUtility.ToJson(Event(kind,reason,configuration));
            if(!configuration && _utf8.GetByteCount(json)>MaxRowBytes)throw new IOException("사건 행 상한입니다.");
            Append(_events,json,MaxEventBytes,ref _eventBytes);
        }
        private string BoundedJson(object row)
        {var json=JsonUtility.ToJson(row);if(_utf8.GetByteCount(json)>MaxRowBytes)throw new IOException("보존 행 32KiB 상한입니다.");return json;}
        private void Append(FileStream stream,string json,long limit,ref long count)
        {
            if(stream==null)throw new IOException("단일 원시 작성자를 잃었습니다.");
            var bytes=_utf8.GetBytes(json+"\n");if(count>limit-bytes.Length)throw new IOException("원시 파일 크기 상한입니다.");
            var began=Stopwatch.GetTimestamp();stream.Write(bytes,0,bytes.Length);stream.Flush();count+=bytes.Length;_previousWriteMilliseconds=Milliseconds(began);
        }
        private void CheckSourceMetadata()
        {
            var began=Stopwatch.GetTimestamp();
            try{
                if(_configuration.sourceFiles.Length!=590||_sourceMetadata==null||_sourceMetadata.Length!=590||_sourceWriteTicks.Length!=590||_sourceMetadataPaths.Length!=590)throw new IOException("소스 스냅샷 배열 결속이 다릅니다.");
                for(var i=0;i<590;i++)
                {var pin=_configuration.sourceFiles[i];var info=_sourceMetadata[i];if(info==null||!string.Equals(info.FullName,_sourceMetadataPaths[i],StringComparison.OrdinalIgnoreCase)||!string.Equals(pin.path,_sourceMetadataPaths[i],StringComparison.OrdinalIgnoreCase))throw new IOException("소스 스냅샷 경로 결속이 다릅니다.");
                    info.Refresh();if(!info.Exists||!SourceMetadataMatches(info.Exists,info.Length,pin.length,info.LastWriteTimeUtc.Ticks,_sourceWriteTicks[i]))throw new IOException("수집 중 원문 파일이 변경됐습니다: "+pin.path);}
            }finally{_sourceMetadataMilliseconds=Milliseconds(began);}
            if(File.Exists(_configuration.nativeEvidencePath) && new FileInfo(_configuration.nativeEvidencePath).Length>MaxNativeBytes ||
                File.Exists(_configuration.inputEvidencePath) && new FileInfo(_configuration.inputEvidencePath).Length>MaxEventBytes)
                throw new IOException("실제 native 또는 입력 원시 크기 상한입니다.");
        }
        private double Elapsed=>(Stopwatch.GetTimestamp()-_startStamp)/(double)Stopwatch.Frequency;
        private static double Milliseconds(long began)=>(Stopwatch.GetTimestamp()-began)*1000d/Stopwatch.Frequency;
        internal static string Hierarchy(Transform value)
        {var result=value.name;for(var parent=value.parent;parent!=null;parent=parent.parent)result=parent.name+"/"+result;return result;}
        internal static string MenuSuffix(string caseId)=>caseId.EndsWith("-B",StringComparison.Ordinal)?caseId:caseId.Substring(0,3);
        internal static bool ValidCase(string caseId,string roomId,string socketId)=>roomId!=null && roomId.Length==3 && roomId[0]=='R' && roomId[1]=='0' && roomId[2]>='1' && roomId[2]<='6' &&
            (caseId==roomId+"-A" || (roomId!="R01" && roomId!="R06" && caseId==roomId+"-B")) && socketId=="S"+roomId.Substring(1)+caseId.Substring(3);
        internal static string RepositoryRoot(string project)=>Path.GetFullPath(Path.Combine(project,"..","..",".."));
        internal static void VerifyApprovedContract(FilePin pin,string project)
        {
            var root=RepositoryRoot(project);var expected=Path.Combine(root,"docs","specs","work-contracts","2026-10-08-b1-basic-movement-observation-and-acceptance.md");
            if(pin==null || pin.sha256!=ApprovedContractSha256 || !SamePath(pin.path,expected))throw new IOException("승인된 정확7 계약 결속이 다릅니다.");
            VerifyPin(pin,root);
        }
        internal static bool SamePath(string left,string right)=>string.Equals(Path.GetFullPath(left).TrimEnd(Path.DirectorySeparatorChar),Path.GetFullPath(right).TrimEnd(Path.DirectorySeparatorChar),StringComparison.OrdinalIgnoreCase);
        internal static void RequireInside(string path,string root)
        {
            var full=Path.GetFullPath(path);var boundary=Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;
            if(!full.StartsWith(boundary,StringComparison.OrdinalIgnoreCase))throw new IOException("허용한 일반 파일 경계 밖입니다.");
            for(var current=Path.GetDirectoryName(full);current!=null;current=Path.GetDirectoryName(current))
            {if(!Directory.Exists(current) || (File.GetAttributes(current)&FileAttributes.ReparsePoint)!=0)throw new IOException("원시 경로의 디렉터리가 없거나 재분석 지점입니다.");if(SamePath(current,root))break;}
            if(File.Exists(full) && (File.GetAttributes(full)&FileAttributes.ReparsePoint)!=0)throw new IOException("파일이 재분석 지점입니다.");
        }
        private static void RequireNewRawPath(string path,string root)
        {RequireInside(path,root);if(File.Exists(path) || Directory.Exists(path))throw new IOException("기존 원시는 덮어쓰거나 재개하지 않습니다.");}
        internal static void VerifyPin(FilePin pin,string root)
        {
            if(pin==null || !IsSha(pin.sha256))throw new IOException("원문 지문이 없습니다.");RequireInside(pin.path,root);
            if(!File.Exists(pin.path) || new FileInfo(pin.path).Length!=pin.length || Sha(pin.path)!=pin.sha256)
                throw new IOException("원문 길이·지문 불일치: "+pin.path);
        }
        internal static bool IsSha(string value)
        {if(value==null || value.Length!=64)return false;foreach(var c in value)if(!((c>='0'&&c<='9')||(c>='A'&&c<='F')))return false;return true;}
        internal static string Sha(string path)
        {using(var hash=SHA256.Create())using(var stream=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read))return BitConverter.ToString(hash.ComputeHash(stream)).Replace("-","");}
        private static FilePin ExistingPin(string path)=>File.Exists(path)?new FilePin{path=path,length=new FileInfo(path).Length,sha256=Sha(path)}:null;
    }
}
#endif
