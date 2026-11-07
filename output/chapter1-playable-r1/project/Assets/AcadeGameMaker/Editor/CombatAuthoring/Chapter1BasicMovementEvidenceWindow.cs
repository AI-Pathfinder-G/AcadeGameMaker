using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Run.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using Observer=AcadeGameMaker.Input.Unity.Chapter1BasicMovementTickObserver;

namespace AcadeGameMaker.Combat.Authoring
{
    // REQ-C1B-G3OBS-004/006/008/009/012: 창은 수집기의 보존 행과 자기 수명만 읽는다.
    // 실제 GUI 실행·입력·원시 독립 검수·수용 소유는 각 후속 승인 원문이 필요하다.
    internal sealed class Chapter1BasicMovementEvidenceWindow : EditorWindow,Observer.IReceivedInputContext
    {
        private const string EvidenceMenu="AcadeGameMaker/Chapter1/Basic Movement Evidence";
        private const string ApplyMenu="AcadeGameMaker/Chapter1/Apply Approved Basic Movement Evidence";
        private const string ObserverAsset="Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs";
        private const string WindowAsset="Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs";
        private const string LifetimeAsset="Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs";
        [SerializeField] private string _executionPath,_caseId="R01-A",_sessionId,_rootName,_sealPath,_latestJson,_firstFailureJson,_message;
        // REQ/AC-C1B-G3OBS-005/006/007/012; M1 §10.10.31: 미무장 단회 읽기 결과만 보존한다.
        [SerializeField] private bool _r01ReadAttempted,_r01ReadPassed;
        [SerializeField] private string _r01ReadResult,_r01ReadError;
        [SerializeField] private bool _r01ReadComplete,_r01SessionConflict;
        private const string R01SessionKey="AcadeGameMaker.Chapter1.BasicMovement.UnarmedR01.Once.";
        private Vector2 _r01ReadScroll;
        [SerializeField] private bool _armed,_playEntered,_applyAttempted;
        // REQ-M1COMP-R1-002, REQ-C1B-G3OBS-005/008/010/012: 최초 승인 창의 단회 수명이다.
        [SerializeField] private bool diagnosticsPrepared,diagnosticsAttempted,diagnosticsPermanentlyBlocked;
        [SerializeField] private string diagnosticsPreparedSessionId,diagnosticsPreparedExecutionSha256,diagnosticsPreparedWindowId,diagnosticsAttemptedSessionId;
        [SerializeField] private string _diagnosticsConfigurationJson;
        private UInt64DiagnosticResult _diagnosticsResult;
        private int _firstPauseSetterCount,_explicitResumeCount;
        private Observer.FilePin _closedDiagnosticsPin;
        private const string DiagnosticsCase="R01-A";
        private const string DiagnosticsPath=@"H:\GPT-workspace\AcadeGameMaker\output\chapter1-playable-r1\evidence\b1-basic-movement-uint64-runtime-r2.json";
        private static readonly string[] DiagnosticsFields={"installedInstanceId","installedSceneHandle","hostId","routerId","hazardsId","playerId","combatId","transferId","collectorId","sceneHandle"};
        private static readonly ulong[] DiagnosticsValues={0UL,4294967296UL,9223372036854775808UL,18446744073709551615UL};
        [SerializeField] private EditorWindow _receivedGameView;
        [SerializeField] private string _receivedGameViewId,_receivedExecutionSha;
        [SerializeField] private long _receivedExecutionLength;
        [SerializeField] private bool _expectedPlayTransition;
        [SerializeField] private ulong _expectedPlaySceneHandle;
        [SerializeField] private bool _sceneLoadedConsumed;
        private bool _sceneLoadedSubscribed;
        private bool _receivedBridgeBound;
        private Observer _collector;
        private Vector2 _scroll;
        // REQ/AC-R01BIND-001..006: runtime-only connection; serialized history is never authority.
        private string _bindingMessage="현재 세션 승인 미연결",_boundPath,_boundSha,_boundSession;
        private long _boundLength;
        private int _bindingAttempts;
        private bool _initialAttempted,_initialPassed,_prepareRequested,_prepareConsumed,_neutralActive,_neutralGuard,_neutralSubscribed,_neutralClosed;
        private string _neutralStage,_neutralFailure,_neutralPath;
        private long _neutralStarted,_neutralFinished,_neutralAfterQpc;
        private int _editorOrdinal,_neutralAfterOrdinal;
        private Keyboard _neutralKeyboard; private Mouse _neutralMouse;
        private ButtonControl[] _neutralControls;
        private readonly float[] _neutralValues=new float[9];
        private readonly bool[] _neutralKnown=new bool[9];
        private readonly bool[] _neutralPressed=new bool[9];
        private readonly double[] _requestTimes=new double[2],_eventTimes=new double[2];
        private readonly long[] _requestQpc=new long[2],_eventQpc=new long[2];
        private readonly int[] _requestResults={-1,-1},_eventIds=new int[2];
        private readonly bool[] _fullSeen=new bool[2];
        private bool _syncIssued;
        private bool _placementReady;
        private Vector2 _detailsScroll;
        private const float DockActionWidth=420,DockActionRow=22;
        private static int DockWindowClass(int count,bool hasReference,bool hasId)
        {if(count<0||count>1)return -1;if(count==0)return 0;if(hasReference)return 1;return hasId?-1:2;}
        private static bool DockCloseComplete(bool destroyed,int remaining)
        {return destroyed&&remaining==0;}
        private static bool UnusedDockCandidate(params bool[] used)
        {foreach(var value in used)if(value)return false;return true;}
        private static bool DockRectValid(double x,double y,double width,double height)
        {return !double.IsNaN(x)&&!double.IsInfinity(x)&&!double.IsNaN(y)&&!double.IsInfinity(y)&&!double.IsNaN(width)&&!double.IsInfinity(width)&&!double.IsNaN(height)&&!double.IsInfinity(height)&&width>0&&height>0;}
        private static bool DockConditions(bool docked,bool mainSame,bool gameSame,bool identitySame,bool windowIdValid,bool finite,double width,double height)
        {return docked&&mainSame&&gameSame&&identitySame&&windowIdValid&&finite&&width>=420&&height>=370;}
        private static string DockRectRaw(Rect value)
        {return string.Join(",",value.x.ToString("R",CultureInfo.InvariantCulture),value.y.ToString("R",CultureInfo.InvariantCulture),value.width.ToString("R",CultureInfo.InvariantCulture),value.height.ToString("R",CultureInfo.InvariantCulture));}
        // REQ/AC-R01PLACE-001/003: this host profile uses screen pixels, then converts to Editor points.
        private static bool TryInitialPlacement(double mx,double my,double mw,double mh,double scale,bool armed,bool alreadyBound,out double x,out double y,out double width,out double height)
        {
            x=y=width=height=0;
            if(armed||alreadyBound||double.IsNaN(mx)||double.IsInfinity(mx)||double.IsNaN(my)||double.IsInfinity(my)||double.IsNaN(mw)||double.IsInfinity(mw)||double.IsNaN(mh)||double.IsInfinity(mh)||(scale!=1&&scale!=1.25)||mw<=0||mh<=0||mx<0||my<0)return false;
            var screenWidth=2560/scale;var screenHeight=1392/scale;var margin=16/scale;
            if(mx+mw>screenWidth||my+mh>screenHeight)return false;
            x=mx+mw+32/scale;y=64/scale;width=900/scale;height=1100/scale;
            return height>=820&&x>=margin&&y>=margin&&x+width<=screenWidth-margin&&y+height<=screenHeight-margin&&x>=mx+mw;
        }
        private static long Now()=>System.Diagnostics.Stopwatch.GetTimestamp();
        private static void BindRequire(bool ok,string why){if(!ok)throw new IOException(why);}
        private static string Digest(byte[] bytes){using(var hash=System.Security.Cryptography.SHA256.Create())return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-","");}
        private static bool BindingIdentityMatches(int expectedPid,long expectedTicks,ulong expectedGame,ulong expectedWindow,int actualPid,long actualTicks,ulong actualGame,ulong actualWindow)
        {return expectedPid>0&&expectedTicks>0&&expectedGame!=0&&expectedWindow!=0&&expectedPid==actualPid&&expectedTicks==actualTicks&&expectedGame==actualGame&&expectedWindow==actualWindow;}
        private static bool BindingEnvelopeMatches(int count,string schema,string owner,string status,string selectedCase)
        {return count==13&&schema=="r01-execution-binding.v1"&&owner=="Root"&&status=="Approved"&&selectedCase=="R01-A";}
        private static bool ValidSessionToken(string session)
        {return !string.IsNullOrEmpty(session)&&session.Length<=128&&System.Text.RegularExpressions.Regex.IsMatch(session,"\\A[A-Za-z0-9_-]+\\z");}
        private static bool PinnedPayloadMatches(byte[] bytes,long length,string sha)
        {return bytes!=null&&length>0&&bytes.LongLength==length&&sha!=null&&sha.Length==64&&Digest(bytes)==sha;}
        private static bool FreshFullEvent(double requested,double current,double observed,int expectedDevice,int actualDevice,bool full)
        {return full&&expectedDevice>0&&expectedDevice==actualDevice&&!double.IsNaN(requested)&&!double.IsInfinity(requested)&&!double.IsNaN(current)&&!double.IsInfinity(current)&&!double.IsNaN(observed)&&!double.IsInfinity(observed)&&requested<=observed&&observed<=current;}
        private static bool SnapshotReady(bool requests,bool full,bool focus,bool devices,bool editor,bool known,bool released,bool equal,bool inTime)
        {return requests&&full&&focus&&devices&&editor&&known&&released&&equal&&inTime;}
        private static bool PrepareReady(bool bound,bool initial,bool requested,bool consumed,bool guard,bool closed,bool failureFree)
        {return bound&&initial&&requested&&!consumed&&guard&&closed&&failureFree;}
        private static void CurrentProcess(out int pid,out long ticks,out string exe)
        {using(var process=System.Diagnostics.Process.GetCurrentProcess()){pid=process.Id;ticks=process.StartTime.ToUniversalTime().Ticks;exe=process.MainModule.FileName;}}
        private string CurrentTuple()
        {CurrentProcess(out var pid,out var ticks,out _);return "PID "+pid+" / "+ticks+" / "+(_boundSession??"미연결");}
        private string ProcessEvidencePath(string suffix)
        {CurrentProcess(out var pid,out var ticks,out _);return Path.Combine(EvidenceRoot(),"r01-"+suffix+"-p"+pid+"-t"+ticks+".json");}
        private void RequireBindingIdle()
        {RequireEditorIdle();BindRequire(_placementReady,"최초 관측창 배치가 완료되지 않았습니다.");BindRequire(!_armed&&!_prepareConsumed&&!diagnosticsPrepared&&!diagnosticsAttempted&&!diagnosticsPermanentlyBlocked,"준비 단회 상태가 이미 소비됐습니다.");BindRequire(_receivedGameView!=null&&!string.IsNullOrEmpty(_receivedGameViewId),"보유 GameView 원본이 없습니다.");}
        private ExecutionApproval ReadBoundApproval()
        {
            BindRequire(!string.IsNullOrEmpty(_boundPath)&&!string.IsNullOrEmpty(_boundSha)&&_caseId==DiagnosticsCase,"현재 세션의 검증된 R01 연결이 없습니다.");
            var approval=ReadLockedApproval(_boundPath,_boundLength,_boundSha);BindRequire(approval.sessionId==_boundSession&&Observer.SamePath(_boundPath,Path.Combine(EvidenceRoot(),_boundSession,"execution-approval.json")),"현재 연결 세션이 바뀌었습니다.");return approval;
        }
        private ExecutionApproval ReadLockedApproval(string path,long length,string sha)
        {
            Observer.RequireInside(path,EvidenceRoot());BindRequire(length>0&&length<=Observer.MaxEventBytes,"승인 길이 상한이 다릅니다.");
            using(var file=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read))
            {
                BindRequire(file.Length==length,"승인 길이가 바뀌었습니다.");var bytes=new byte[checked((int)length)];var offset=0;
                while(offset<bytes.Length){var n=file.Read(bytes,offset,bytes.Length-offset);BindRequire(n>0,"승인 원문이 중단됐습니다.");offset+=n;}
                BindRequire(file.ReadByte()==-1&&PinnedPayloadMatches(bytes,length,sha),"승인 원문 지문이 바뀌었습니다.");
                // REQ-R01BIND-002: hash, strict decode and projection consume this exact byte array.
                using(var document=ParseStrictBytes(bytes,Observer.MaxEventBytes))
                {var approval=ProjectExecutionApproval(document.RootElement);ValidateBindingTuple(approval);return approval;}
            }
        }
        private void ValidateBindingTuple(ExecutionApproval approval)
        {
            CurrentProcess(out var pid,out var ticks,out var exe);
            BindRequire(approval.schemaVersion==1&&approval.status=="Approved"&&approval.finalApproval&&approval.guiExecutionAuthorizedNow&&CurrentUserActor(approval)&&Astra(approval.participant),"현재 승인 담당·권위가 다릅니다.");
            BindRequire(approval.unityPid==pid&&approval.unityStartTicks==ticks&&Observer.SamePath(exe,approval.unityExe)&&approval.unityVersion==Application.unityVersion,"현재 Unity 신원이 다릅니다.");
            BindRequire(_receivedGameView!=null&&approval.inputGameViewWindowId==EntityId.ToULong(_receivedGameView.GetEntityId())&&approval.inputGameViewWindowId.ToString(System.Globalization.CultureInfo.InvariantCulture)==_receivedGameViewId&&approval.uint64DiagnosticsWindowId==EntityId.ToULong(GetEntityId()),"현재 보유 창 신원이 다릅니다.");
            RequireExactCases(approval.cases);
        }
        private void ConnectCurrentApproval()
        {
            if(_bindingAttempts>=2)return;_bindingAttempts++;_boundPath=null;_boundSha=null;_boundSession=null;
            try
            {
                RequireBindingIdle();BindRequire(!_neutralActive,"입력 상태 관측 중입니다.");
                using(var document=ReadStrictDocument(ProcessEvidencePath("execution-binding"),EvidenceRoot(),32768))
                {
                    var value=document.RootElement;var count=0;foreach(var property in value.EnumerateObject())count++;
                    BindRequire(BindingEnvelopeMatches(count,Text(value,"schema"),Text(value,"owner"),Text(value,"status"),Text(value,"caseId")),"연결 스키마가 다릅니다.");
                    CurrentProcess(out var pid,out var ticks,out var exe);var session=Text(value,"sessionId");
                    BindRequire(ValidSessionToken(session),"세션 경로 토큰이 다릅니다.");
                    BindRequire(Integer(value,"unityPid")==pid&&LongInteger(value,"unityStartTicks")==ticks&&Observer.SamePath(Text(value,"project"),ProjectRoot())&&Observer.SamePath(Text(value,"unityExecutable"),exe)&&Text(value,"unityVersion")==Application.unityVersion,"연결 프로세스·프로젝트가 다릅니다.");
                    BindRequire(_receivedGameView!=null&&BindingIdentityMatches(pid,ticks,EntityId.ToULong(_receivedGameView.GetEntityId()),EntityId.ToULong(GetEntityId()),Integer(value,"unityPid"),LongInteger(value,"unityStartTicks"),UInt64Integer(value,"gameViewInstanceId"),UInt64Integer(value,"evidenceWindowInstanceId")),"연결 창 원본이 다릅니다.");
                    var pin=FilePin(value,"executionApproval");BindRequire(Observer.SamePath(pin.path,Path.Combine(EvidenceRoot(),session,"execution-approval.json")),"현재 세션의 정확 승인 경로가 아닙니다.");
                    var approval=ReadLockedApproval(pin.path,pin.length,pin.sha256);BindRequire(approval.sessionId==session,"연결과 승인 세션이 다릅니다.");ValidateDiagnosticsApproval(approval);
                    _boundPath=pin.path;_boundLength=pin.length;_boundSha=pin.sha256;_boundSession=session;_executionPath=pin.path;_caseId=DiagnosticsCase;
                    _bindingMessage="현재 승인 연결 완료 · 준비는 아직 미소비";
                }
            }
            catch(Exception error){_boundPath=null;_boundSha=null;_bindingMessage="현재 승인 연결 미완료: "+error.Message;}
        }
        private void SubscribeNeutral()
        {if(_neutralSubscribed)return;InputSystem.onEvent+=ObserveNeutralEvent;InputSystem.onAfterUpdate+=ObserveNeutralAfterUpdate;InputSystem.onDeviceChange+=ObserveNeutralDevice;EditorApplication.update+=ObserveNeutralLifetime;_neutralSubscribed=true;}
        private void UnsubscribeNeutral()
        {if(!_neutralSubscribed)return;InputSystem.onEvent-=ObserveNeutralEvent;InputSystem.onAfterUpdate-=ObserveNeutralAfterUpdate;InputSystem.onDeviceChange-=ObserveNeutralDevice;EditorApplication.update-=ObserveNeutralLifetime;_neutralSubscribed=false;}
        private bool CurrentNeutralContext()=>Application.isFocused&&_receivedGameView!=null&&EditorWindow.focusedWindow==_receivedGameView&&!EditorApplication.isPlayingOrWillChangePlaymode&&!EditorApplication.isCompiling&&!EditorApplication.isUpdating;
        private void RequireNeutralDevices()
        {BindRequire(ReferenceEquals(_neutralKeyboard,Keyboard.current)&&ReferenceEquals(_neutralMouse,Mouse.current)&&_neutralKeyboard!=null&&_neutralMouse!=null&&_neutralKeyboard.native&&_neutralMouse.native&&_neutralKeyboard.added&&_neutralMouse.added&&_neutralKeyboard.enabled&&_neutralMouse.enabled,"입력 장치 원본·활성 상태가 바뀌었습니다.");}
        private void BeginNeutral(bool prepare)
        {
            try
            {
                RequireBindingIdle();BindRequire(!_neutralActive,"입력 상태 확인이 진행 중입니다.");
                if(prepare){BindRequire(_initialPassed&&!_prepareRequested,"준비 요청은 초기 성공 뒤 한 번입니다.");_prepareRequested=true;ReadBoundApproval();}
                else {BindRequire(!_initialAttempted,"초기 입력 관측은 한 번입니다.");_initialAttempted=true;}
                _neutralStage=prepare?"prepare":"initial";_neutralPath=ProcessEvidencePath("neutral").Replace(".json","-"+_neutralStage+".json");
                Observer.RequireInside(_neutralPath,EvidenceRoot());BindRequire(!File.Exists(_neutralPath)&&!Directory.Exists(_neutralPath),"같은 입력 관측 원문은 덮어쓰지 않습니다.");
                _neutralActive=true;_neutralGuard=false;_neutralClosed=false;_neutralFailure=null;_neutralStarted=Now();_neutralFinished=0;_neutralAfterOrdinal=0;_neutralAfterQpc=0;_syncIssued=false;
                Array.Clear(_neutralKnown,0,9);Array.Clear(_neutralPressed,0,9);Array.Clear(_neutralValues,0,9);Array.Clear(_fullSeen,0,2);Array.Clear(_requestTimes,0,2);Array.Clear(_eventTimes,0,2);Array.Clear(_requestQpc,0,2);Array.Clear(_eventQpc,0,2);Array.Clear(_eventIds,0,2);_requestResults[0]=_requestResults[1]=-1;
                _neutralKeyboard=Keyboard.current;_neutralMouse=Mouse.current;RequireNeutralDevices();
                _neutralControls=new ButtonControl[]{_neutralKeyboard.aKey,_neutralKeyboard.dKey,_neutralKeyboard.spaceKey,_neutralKeyboard.leftShiftKey,_neutralKeyboard.rightShiftKey,_neutralKeyboard.qKey,_neutralKeyboard.eKey,_neutralMouse.leftButton,_neutralMouse.rightButton};
                SubscribeNeutral();_receivedGameView.Focus();_bindingMessage="현재 앱 입력 상태 관측 중 · "+_neutralStage;
            }
            catch(Exception error){if(_neutralActive)FinishNeutral(false,error.Message);else _bindingMessage="입력 상태 확인 거절: "+error.Message;}
        }
        private void ObserveNeutralLifetime()
        {
            if(!_neutralActive&&!_neutralGuard)return;
            try
            {
                BindRequire(Now()-_neutralStarted<5*System.Diagnostics.Stopwatch.Frequency,"입력 상태 관측 5초 상한입니다.");RequireNeutralDevices();
                if(!_syncIssued)
                {
                    if(!CurrentNeutralContext())return;_syncIssued=true;
                    for(var i=0;i<2;i++){_requestTimes[i]=InputState.currentTime;BindRequire(!double.IsNaN(_requestTimes[i])&&!double.IsInfinity(_requestTimes[i]),"요청 입력 시각이 유한값이 아닙니다.");_requestQpc[i]=Now();_requestResults[i]=InputSystem.TrySyncDevice(i==0?(InputDevice)_neutralKeyboard:_neutralMouse)?1:0;}
                    BindRequire(_requestResults[0]==1&&_requestResults[1]==1,"현재 장치의 상태 동기화가 지원되지 않습니다.");
                }
                BindRequire(CurrentNeutralContext(),"입력 관측의 GameView 포커스가 소실됐습니다.");
                if(_neutralGuard)RequireNeutralGuard();
            }
            catch(Exception error){if(_neutralActive)FinishNeutral(false,error.Message);else {_neutralGuard=false;_neutralFailure=error.Message;UnsubscribeNeutral();}}
        }
        private void ObserveNeutralDevice(InputDevice device,InputDeviceChange change)
        {if(ReferenceEquals(device,_neutralKeyboard)||ReferenceEquals(device,_neutralMouse)){if(_neutralActive)FinishNeutral(false,"관측 장치 변경: "+change);else {_neutralGuard=false;_neutralFailure="관측 장치 변경";UnsubscribeNeutral();}}}
        private void ObserveNeutralEvent(InputEventPtr inputEvent,InputDevice device)
        {
            if(!_syncIssued||(!_neutralActive&&!_neutralGuard))return;
            var index=ReferenceEquals(device,_neutralKeyboard)?0:ReferenceEquals(device,_neutralMouse)?1:-1;if(index<0)return;
            try
            {
                RequireNeutralDevices();BindRequire(CurrentNeutralContext(),"허용 입력 수신 시 포커스가 다릅니다.");
                BindRequire(InputState.currentUpdateType==InputUpdateType.Editor,"Editor 입력 버퍼의 사건이 아닙니다.");var time=inputEvent.time;
                if(!inputEvent.IsA<StateEvent>()){if(_neutralGuard)throw new IOException("준비 감시 중 전체 상태가 아닌 허용 입력 사건입니다.");return;}
                BindRequire(FreshFullEvent(_requestTimes[index],InputState.currentTime,time,device.deviceId,inputEvent.deviceId,true),"입력 사건 시각·장치가 불명확합니다.");
                var start=index==0?0:7;var end=index==0?7:9;
                for(var i=start;i<end;i++){BindRequire(_neutralControls[i].ReadValueFromEvent(inputEvent,out var value)&&!float.IsNaN(value)&&!float.IsInfinity(value),"전체 사건의 허용 control이 미판독입니다.");var pressed=value>=_neutralControls[i].pressPointOrDefault;if(_neutralGuard)BindRequire(_neutralKnown[i]&&value==_neutralValues[i],"준비 전 허용 control 상태가 바뀌었습니다.");_neutralKnown[i]=true;_neutralValues[i]=value;_neutralPressed[i]=pressed;BindRequire(!pressed,"허용 control 눌림이 관측됐습니다.");}
                _fullSeen[index]=true;_eventIds[index]=inputEvent.id;_eventTimes[index]=time;_eventQpc[index]=Now();
            }
            catch(Exception error){if(_neutralActive)FinishNeutral(false,error.Message);else {_neutralGuard=false;_neutralFailure=error.Message;UnsubscribeNeutral();}}
        }
        private void RequireNeutralGuard()
        {
            BindRequire(_neutralGuard&&_neutralClosed&&string.IsNullOrEmpty(_neutralFailure)&&CurrentNeutralContext(),"준비 직전 현재 중립 관측이 무효입니다.");RequireNeutralDevices();
            for(var i=0;i<9;i++)BindRequire(_neutralKnown[i]&&!_neutralPressed[i]&&!_neutralControls[i].isPressed&&_neutralControls[i].ReadValue()==_neutralValues[i],"준비 직전 control 상태가 바뀌었습니다.");
        }
        private void ObserveNeutralAfterUpdate()
        {
            if(InputState.currentUpdateType!=InputUpdateType.Editor)return;_editorOrdinal++;
            if(!_neutralActive||!_syncIssued||!_fullSeen[0]||!_fullSeen[1])return;
            try
            {
                RequireNeutralDevices();var known=true;var released=true;var equal=true;
                for(var i=0;i<9;i++){known&=_neutralKnown[i];released&=!_neutralPressed[i]&&!_neutralControls[i].isPressed;equal&=_neutralControls[i].ReadValue()==_neutralValues[i];}
                BindRequire(SnapshotReady(_requestResults[0]==1&&_requestResults[1]==1,_fullSeen[0]&&_fullSeen[1],CurrentNeutralContext(),true,InputState.currentUpdateType==InputUpdateType.Editor,known,released,equal,Now()-_neutralStarted<5*System.Diagnostics.Stopwatch.Frequency),"현재 Editor 버퍼·수신 상태·포커스·기한이 다릅니다.");
                _neutralAfterOrdinal=_editorOrdinal;_neutralAfterQpc=Now();FinishNeutral(true,null);
            }
            catch(Exception error){if(_neutralActive)FinishNeutral(false,error.Message);}
        }
        private void FinishNeutral(bool passed,string error)
        {
            if(!_neutralActive)return;_neutralActive=false;_neutralFinished=Now();_neutralFailure=error;var prepare=_neutralStage=="prepare";
            try{WriteNeutralObservation(passed);_neutralClosed=true;}catch(Exception writeError){passed=false;_neutralFailure=(error??"")+" 원문 폐쇄 실패: "+writeError.Message;}
            if(!passed){_neutralGuard=false;UnsubscribeNeutral();_bindingMessage="현재 입력 관측 실패: "+_neutralFailure;return;}
            if(!prepare){_initialPassed=true;UnsubscribeNeutral();_bindingMessage="초기 앱 입력 상태 관측·원문 폐쇄 완료 · 현재 승인 연결은 별도";return;}
            _neutralGuard=true;
            try{RequireNeutralGuard();ReadBoundApproval();TryPrepare();_bindingMessage=_armed?"현재 세션 관측기 준비 완료 · 실제 플레이/진단은 별도":"현재 세션 관측기 준비 실패: "+_message;}
            catch(Exception failure){_bindingMessage="현재 준비 진입 거절: "+failure.Message;}
            finally{_neutralGuard=false;UnsubscribeNeutral();}
        }
        private void WriteNeutralObservation(bool passed)
        {
            CurrentProcess(out var pid,out var ticks,out var exe);
            using(var memory=new MemoryStream())
            {
                using(var json=new Utf8JsonWriter(memory))
                {
                    json.WriteStartObject();json.WriteString("schema","r01-editor-neutral-observation.v1");json.WriteString("stage",_neutralStage);json.WriteNumber("unityPid",pid);json.WriteNumber("unityStartTicks",ticks);json.WriteString("unityExecutable",exe);json.WriteNumber("evidenceWindowInstanceId",EntityId.ToULong(GetEntityId()));if(_receivedGameView!=null)json.WriteNumber("gameViewInstanceId",EntityId.ToULong(_receivedGameView.GetEntityId()));else json.WriteNull("gameViewInstanceId");
                    if(_neutralStage=="prepare"){json.WriteString("sessionId",_boundSession);json.WriteString("approvalSHA256",_boundSha);}else{json.WriteNull("sessionId");json.WriteNull("approvalSHA256");}
                    json.WriteNumber("startedQpc",_neutralStarted);json.WriteNumber("finishedQpc",_neutralFinished);json.WriteNumber("frequency",System.Diagnostics.Stopwatch.Frequency);json.WriteBoolean("passed",passed);json.WriteString("failure",_neutralFailure);json.WriteBoolean("gameViewFocused",CurrentNeutralContext());json.WriteNumber("editorAfterUpdateOrdinal",_neutralAfterOrdinal);json.WriteNumber("editorAfterUpdateQpc",_neutralAfterQpc);json.WriteString("updateType",InputState.currentUpdateType.ToString());json.WriteBoolean("editorBufferMatched",passed&&InputState.currentUpdateType==InputUpdateType.Editor);
                    json.WriteStartArray("devices");for(var d=0;d<2;d++){InputDevice device=d==0?(InputDevice)_neutralKeyboard:_neutralMouse;json.WriteStartObject();json.WriteString("kind",d==0?"keyboard":"mouse");json.WriteNumber("deviceId",device!=null?device.deviceId:0);json.WriteBoolean("native",device!=null&&device.native);json.WriteBoolean("added",device!=null&&device.added);json.WriteBoolean("enabled",device!=null&&device.enabled);json.WriteNumber("requestResult",_requestResults[d]);json.WriteNumber("requestInputTime",_requestTimes[d]);json.WriteNumber("requestQpc",_requestQpc[d]);json.WriteBoolean("fullEventSeen",_fullSeen[d]);json.WriteNumber("eventId",_eventIds[d]);json.WriteNumber("eventInputTime",_eventTimes[d]);json.WriteNumber("eventReceivedQpc",_eventQpc[d]);json.WriteEndObject();}json.WriteEndArray();
                    var names=new[]{"A","D","Space","LeftShift","RightShift","Q","E","MouseLeft","MouseRight"};json.WriteStartArray("controls");for(var i=0;i<9;i++){json.WriteStartObject();json.WriteString("name",names[i]);json.WriteBoolean("known",_neutralKnown[i]);json.WriteBoolean("pressed",_neutralPressed[i]);json.WriteNumber("value",_neutralValues[i]);json.WriteEndObject();}json.WriteEndArray();
                    json.WriteString("closureResult","작성 후 실제 폐쇄 결과는 현재 창과 Root의 닫힌 원문 핀으로 확인");json.WriteEndObject();json.Flush();
                }
                BindRequire(memory.Length<=32768,"입력 관측 원문 상한입니다.");var bytes=memory.ToArray();using(var file=new FileStream(_neutralPath,FileMode.CreateNew,FileAccess.Write,FileShare.Read)){file.Write(bytes,0,bytes.Length);file.Flush(true);}
            }
        }
        private bool _subscribed;

        [Serializable] private sealed class Participant
        { public string model,reasoningEffort,role; }
        [Serializable] private sealed class SourceCheckpoint
        { public int fileCount; public Observer.FilePin[] files; }
        // 정해진 scalar 스키마는 사실을 인증하지 않는다. 명명된 승인/도구 원문을 그대로 결속한다.
        [Serializable] private sealed class ExecutionApproval
        {
            public int schemaVersion;
            public string status,owner,sessionId,performer,performerModel,performerRole,unityExe,unityVersion,windowIdentity,inputTarget;
            public bool finalApproval,guiExecutionAuthorizedNow,inputNeutralObserved;
            public Participant participant;
            public string inputEvidenceKind;
            public ulong inputGameViewWindowId;
            public int unityPid;
            public long unityStartTicks;
            public double caseBudgetSeconds,totalBudgetSeconds,caseElapsedSecondsAtInstall,totalElapsedSecondsAtInstall;
            public Observer.FilePin sourceCheckpoint,approvedContract,toolObservation,observerSource,observerMeta,windowSource,windowMeta;
            public CaseApproval[] cases;
            public bool uint64DiagnosticsAuthorizedNow,uint64DiagnosticsNativeInputAuthorizedNow;
            public string uint64DiagnosticsCaseId,uint64DiagnosticsPath;
            public int uint64DiagnosticsCallLimitSeconds,uint64DiagnosticsFileLimitBytes,uint64DiagnosticsInvocationLimit;
            public ulong uint64DiagnosticsWindowId;
            public Observer.FilePin uint64DiagnosticsNativeInputAuthority;
            public bool firstSamplePauseAuthorizedNow,resumeAfterUInt64AcceptedAuthorizedNow;
            public string firstSamplePauseCaseId;
            public int firstSamplePauseInvocationLimit;
        }
        [Serializable] private sealed class CaseApproval
        {
            public string caseId,roomId,socketId,reviewMenu,sceneName,samplesPath,eventsPath,sealPath,nativeEvidencePath,inputEvidencePath;
            public Observer.FilePin prefab,prefabMeta;
            public bool uint64DiagnosticsAuthorizedNow;
            public string uint64DiagnosticsCaseId,uint64DiagnosticsPath;
            public int uint64DiagnosticsCallLimitSeconds,uint64DiagnosticsFileLimitBytes,uint64DiagnosticsInvocationLimit;
        }
        [Serializable] private sealed class ApplyAuthorization
        {
            public int schemaVersion;
            public string status,fromOwner,toOwner,applyOwner;
            public Participant participant;
            public bool finalApproval,roomAcceptanceWriteAuthorizedNow;
            public Observer.FilePin sourceCheckpoint,observerSource,observerMeta,windowSource,windowMeta,approvedContract;
            public string[] exactPrefabPaths;
            public RoomAuthorization[] rooms;
        }
        [Serializable] private sealed class RoomAuthorization
        {
            public string roomId;
            public Observer.FilePin prefab,prefabMeta,lunaRoomReview,astraRoomApproval;
            public string[] requiredCaseIds;
        }
        [Serializable] private sealed class AstraRoomApproval
        {
            public int schemaVersion;
            public string status,sourceCheckpointSha256,observerHelperSha256,lunaRoomReviewSha256,roomId,expectedPrefabSha256,expectedMetaSha256,applyOwner;
            public string[] requiredCaseIds;
            public Participant participant;
            public bool finalApproval;
        }
        [Serializable] private sealed class LunaRoomReview
        {
            public int schemaVersion,p0Count,p1Count;
            public Participant participant;
            public string sourceCheckpointSha256,observerHelperSha256,roomId,expectedPrefabSha256,expectedMetaSha256;
            public string[] requiredCaseIds;
            public ReviewedCase[] cases;
        }
        [Serializable] private sealed class ReviewedCase
        {
            public string caseId,socketId,runKey,completionKey;
            public long generation,continuousSampleCount;
            public int firstTick,finalTick,gapCount;
            public bool endpointRead,pathRead,supportRead,survivalRead,prohibitedInputAbsent,sourceIdentityRead,closureRead;
            public Observer.FilePin rawManifest,nativeEvidence,inputEvidence;
        }

        [MenuItem(EvidenceMenu)] private static void Open()
        {
            // REQ/AC-C1B-G3OBS-005/006/012: 기존 메뉴 진입에서 실제 원본을 단1회 보유한다.
            if(!Application.isFocused || EditorWindow.focusedWindow==null)
                throw new InvalidOperationException("메뉴 진입 전 실제 게임 포커스 원본이 없습니다. 대상을 검색하거나 재선택하지 않습니다.");
            var gameView=EditorWindow.focusedWindow;
            var id=EntityId.ToULong(gameView.GetEntityId()).ToString(System.Globalization.CultureInfo.InvariantCulture);
            Observer.ParseGameViewIdDecimal(id);
            var mainPosition=EditorGUIUtility.GetMainWindowPosition();
            var gamePosition=gameView.position;
            var displayScale=EditorGUIUtility.pixelsPerPoint;
            var existing=Resources.FindObjectsOfTypeAll<Chapter1BasicMovementEvidenceWindow>();
            var classification=DockWindowClass(existing.Length,existing.Length==1&&existing[0]._receivedGameView!=null,existing.Length==1&&!string.IsNullOrEmpty(existing[0]._receivedGameViewId));
            if(classification<0)throw new InvalidOperationException("관측창 중복 또는 보유 원본 소실입니다. 새 창을 만들지 않습니다.");
            Chapter1BasicMovementEvidenceWindow window=null;
            if(existing.Length==1)window=existing[0];
            if(window!=null)
            {
            if(window._armed)
                throw new InvalidOperationException("이미 준비된 관측 창은 같은 원본이라도 재결속하지 않습니다.");
            if(window._receivedGameView!=null)
            {
                if(window._receivedGameView!=gameView || window._receivedGameViewId!=id)
                    throw new InvalidOperationException("보유한 실제 게임 창 원본과 다릅니다. 기존 원본을 교체하지 않습니다.");
                return;
            }
            if(!string.IsNullOrEmpty(window._receivedGameViewId))
                throw new InvalidOperationException("보유 원본이 소실됐습니다. 다른 객체로 재결속하지 않습니다.");
                if(!UnusedDockCandidate(window._armed,window._collector!=null,window._receivedGameView!=null,!string.IsNullOrEmpty(window._receivedGameViewId),!string.IsNullOrEmpty(window._receivedExecutionSha),window._initialAttempted,window._initialPassed,window._prepareRequested,window._prepareConsumed,window._r01ReadAttempted,window._r01ReadPassed,window._r01ReadComplete,window._r01SessionConflict,window.diagnosticsPrepared,window.diagnosticsAttempted,window.diagnosticsPermanentlyBlocked,window._applyAttempted,window._playEntered,window._neutralActive,window._neutralGuard,window._neutralSubscribed,window._neutralClosed,window._bindingAttempts!=0,!string.IsNullOrEmpty(window._boundPath),!string.IsNullOrEmpty(window._boundSha),!string.IsNullOrEmpty(window._boundSession),window._boundLength!=0,!string.IsNullOrEmpty(window._sessionId),!string.IsNullOrEmpty(window._rootName),!string.IsNullOrEmpty(window._sealPath),!string.IsNullOrEmpty(window._executionPath),!string.IsNullOrEmpty(window.diagnosticsPreparedSessionId),!string.IsNullOrEmpty(window.diagnosticsAttemptedSessionId),!string.IsNullOrEmpty(window._neutralStage),SessionState.GetBool(R01SessionKey+"Attempted",false)))
                    throw new InvalidOperationException("복원창의 미사용 상태가 확정되지 않았습니다. 닫거나 재생성하지 않습니다.");
                var restored=window;restored.Close();
                if(!DockCloseComplete(restored==null,Resources.FindObjectsOfTypeAll<Chapter1BasicMovementEvidenceWindow>().Length))
                    throw new InvalidOperationException("복원창 닫기가 완료되지 않았습니다. 생성하지 않습니다.");
            }
            window=CreateWindow<Chapter1BasicMovementEvidenceWindow>("기본 이동 관측",typeof(UnityEditor.SceneView));
            window._receivedGameView=gameView;window._receivedGameViewId=id;
            window._placementReady=false;
            var actual=window.position;
            var mainAfter=EditorGUIUtility.GetMainWindowPosition();var gameAfter=gameView.position;
            var currentId=EntityId.ToULong(gameView.GetEntityId()).ToString(CultureInfo.InvariantCulture);
            var windowId=EntityId.ToULong(window.GetEntityId());
            var identitySame=window._receivedGameView==gameView&&ReferenceEquals(window._receivedGameView,gameView)&&window._receivedGameViewId==id&&currentId==id;
            var isDocked=window.docked;var mainSame=mainPosition.Equals(mainAfter);var gameSame=gamePosition.Equals(gameAfter);
            var finite=DockRectValid(actual.x,actual.y,actual.width,actual.height)&&DockRectValid(mainAfter.x,mainAfter.y,mainAfter.width,mainAfter.height)&&DockRectValid(gameAfter.x,gameAfter.y,gameAfter.width,gameAfter.height);
            window._placementReady=DockConditions(isDocked,mainSame,gameSame,identitySame,windowId!=0,finite,actual.width,actual.height);
            Debug.Log("R01DOCK mainBefore="+DockRectRaw(mainPosition)+" mainAfter="+DockRectRaw(mainAfter)+" gameBefore="+DockRectRaw(gamePosition)+" gameAfter="+DockRectRaw(gameAfter)+" actual="+DockRectRaw(actual)+" scale="+displayScale.ToString("R",CultureInfo.InvariantCulture)+" gameId="+id+" currentGameId="+currentId+" retainedGameId="+window._receivedGameViewId+" windowId="+windowId.ToString(CultureInfo.InvariantCulture)+" unityReferenceEqual="+(window._receivedGameView==gameView)+" managedReferenceEqual="+ReferenceEquals(window._receivedGameView,gameView)+" docked="+isDocked+" mainSame="+mainSame+" gameSame="+gameSame+" identitySame="+identitySame+" finite="+finite+" widthReady="+(actual.width>=DockActionWidth)+" heightReady="+(actual.height>=370)+" passed="+window._placementReady);
            if(!window._placementReady){window._bindingMessage="최초 도킹의 신원·기하·최소 동작 영역 검증 실패. 재배치하지 않습니다.";throw new InvalidOperationException(window._bindingMessage);}
            window._bindingMessage="최초 도킹 속성 확인 · 실제 탭과 전체 버튼 접근은 Root 확인 필요";
        }
        [MenuItem(ApplyMenu)] private static void ApplyApprovedEvidence()
        {
            var window=GetWindow<Chapter1BasicMovementEvidenceWindow>("기본 이동 관측");
            window.ApplyEvidence();
        }
        private void OnEnable()
        {
            if(_subscribed)return;
            EditorApplication.update+=ObserveEditorLifetime;
            EditorApplication.playModeStateChanged+=ObservePlayLifetime;
            EditorApplication.pauseStateChanged+=ObservePauseLifetime;
            _subscribed=true;
            if(_armed&&!_expectedPlayTransition){FindOwnedCollector();TryBindReceivedContext();}
            else if(_armed&&_expectedPlayTransition&&!_sceneLoadedConsumed)
            {SceneManager.sceneLoaded+=OnExpectedSceneLoaded;_sceneLoadedSubscribed=true;}
        }
        private void OnDisable()
        {
            RemoveExpectedSceneLoaded();
            if(_neutralActive)FinishNeutral(false,"관측 창 종료 또는 재로드");_neutralGuard=false;UnsubscribeNeutral();_boundPath=null;_boundSha=null;
            if(diagnosticsPrepared && _playEntered)diagnosticsPermanentlyBlocked=true;
            if(_collector!=null)
            {
                if(_expectedPlayTransition&&!_playEntered&&_collector.ExpectedPlaySuspendForAuthoring&&EditorApplication.isPlayingOrWillChangePlaymode&&!EditorApplication.isCompiling&&!EditorApplication.isUpdating)
                {try{_collector.SuspendExpectedPlayForAuthoring();}catch(Exception error){_collector.RejectReceivedInputForAuthoring("예상 전환 중단 거절: "+error.Message);}}
                else _collector.SealForAuthoring("관측 창이 닫히거나 재로드됐습니다.",false);
            }
            if(!_subscribed)return;
            EditorApplication.update-=ObserveEditorLifetime;
            EditorApplication.playModeStateChanged-=ObservePlayLifetime;
            EditorApplication.pauseStateChanged-=ObservePauseLifetime;
            _subscribed=false;
        }
        private void ObserveEditorLifetime()
        {
            if(!_armed)return;
            // 수집기 자신의 재로드 신원만 찾는다. 게임 getter로 이전 행을 보충하지 않는다.
            if(_collector==null)FindOwnedCollector();
            if(_collector!=null)
            {
                if(EditorApplication.isCompiling || EditorApplication.isUpdating)
                {
                    if(diagnosticsPrepared)diagnosticsPermanentlyBlocked=true;
                    _collector.SealForAuthoring("수집 도중 컴파일 또는 가져오기가 시작됐습니다.",false);
                }
                _collector.ObserveDisplayUpdateForAuthoring();
                if(_collector.FirstPauseWaitingForAuthoring)
                {try{if(!SameCurrentPlayAuthority())throw new IOException("정지 대기 중 현재 승인이 다릅니다.");}catch(Exception error){_collector.RejectReceivedInputForAuthoring(error.Message);}}
                _collector.VerifyFirstPauseWaitForAuthoring();
                CopyDisplay();Repaint();
            }
            else if(_playEntered && EditorApplication.isPlaying)
            { _message="재로드된 임시 수집기가 없습니다. 현재 사례는 미수용이며 자동 설치하지 않습니다.";Repaint(); }
        }
        private void ObservePlayLifetime(PlayModeStateChange state)
        {
            if(!_armed)return;
            if(state==PlayModeStateChange.ExitingEditMode)
            {
                try
                {
                    if(_expectedPlayTransition||EditorApplication.isCompiling||EditorApplication.isUpdating)throw new IOException("최초 Play 이외 전환 또는 컴파일 중입니다.");
                    FindOwnedCollector();if(_collector==null)throw new IOException("정확 승인 수집기가 없습니다.");
                    if(!SameCurrentPlayAuthority())throw new IOException("중단 전 현재 승인 신원이 다릅니다.");
                    _collector.ExpectFirstPlayForAuthoring();_collector.SuspendExpectedPlayForAuthoring();_expectedPlayTransition=true;
                }
                catch(Exception error){if(_collector!=null)_collector.RejectReceivedInputForAuthoring("Play 전환 준비 거절: "+error.Message);_message=error.Message;}
            }
            else if(state==PlayModeStateChange.EnteredPlayMode)
            {
                _playEntered=true;FindOwnedCollector();
                // Observer owns one restore before callback activation. This event never rebinds it.
                if(_collector==null)_message="재생 전 관측 bootstrap이 소실됐습니다. 실제 Stop은 실행 담당자가 처리합니다.";
            }
            else if(state==PlayModeStateChange.ExitingPlayMode)
            {
                FindOwnedCollector();
                if(_collector!=null){_collector.SealForAuthoring("실행 담당자의 실제 Play 종료",true);CopyDisplay();}
            }
            else if(state==PlayModeStateChange.EnteredEditMode)
            {
                _playEntered=false;FindOwnedCollector();
                // 봉인된 자기 원시만 읽는다. 편집 장면의 게임 객체로 forensic 값을 재구성하지 않는다.
                if(!string.IsNullOrEmpty(_sealPath) && File.Exists(_sealPath))
                {
                    try
                    {
                        var seal=ReadSealRow(_sealPath,EvidenceRoot(),Observer.MaxEventBytes);
                        if(seal==null || seal.sessionId!=_sessionId || seal.caseId!=_caseId || seal.live)throw new IOException("봉인 신원이 다릅니다.");
                        if(!string.IsNullOrEmpty(seal.lastSampleJson))ReadSampleProjection(seal.lastSampleJson);
                        _latestJson=seal.lastSampleJson;_firstFailureJson=seal.firstFailureJson;
                        _message=seal.completionKey==null?"원시가 종료됐습니다. 완주키가 없으며 미수용입니다.":"출구 후보 원시와 완주키가 봉인됐습니다. 독립 검수는 아직 필요합니다.";
                    }
                    catch(Exception error){_latestJson=null;_message="봉인 원시 판독 실패: "+error.Message;}
                }
                else _message="종료 봉인 원시가 없습니다. 마지막 표본을 정상 완주로 보충하지 않습니다.";
            }
            Repaint();
        }
        private void FindOwnedCollector()
        {
            _collector=null;var count=0;
            foreach(var candidate in UnityEngine.Object.FindObjectsByType<Observer>(FindObjectsInactive.Include,FindObjectsSortMode.None))
                if(candidate.ConfiguredSessionIdForAuthoring==_sessionId && candidate.transform.parent==null && candidate.gameObject.name==_rootName)
                {_collector=candidate;count++;}
            if(count>1){foreach(var candidate in UnityEngine.Object.FindObjectsByType<Observer>(FindObjectsInactive.Include,FindObjectsSortMode.None))
                if(candidate.ConfiguredSessionIdForAuthoring==_sessionId)candidate.SealForAuthoring("같은 관측 수집기가 중복입니다.",false);
                _collector=null;_message="같은 관측 수집기가 중복입니다. 미수용으로 종료합니다.";}
        }
        private void CopyDisplay()
        {
            if(_collector==null)return;
            if(!string.IsNullOrEmpty(_collector.LatestRowJson))_latestJson=_collector.LatestRowJson;
            if(!string.IsNullOrEmpty(_collector.FirstFailureJson))_firstFailureJson=_collector.FirstFailureJson;
            if(!string.IsNullOrEmpty(_collector.SealError))_message="원시 기록·봉인 오류: "+_collector.SealError;
        }
        // REQ/AC-C1B-G3OBS-005/006/007/012; §32: 고정 세션 키가 창 수명보다 우선한다.
        private void RestoreR01SessionRead()
        {
            if(_r01SessionConflict)return;
            try
            {
                var attempted=SessionState.GetBool(R01SessionKey+"Attempted",false);
                if(!attempted && !_r01ReadAttempted)return;
                if(!attempted)
                {
                    SessionState.SetBool(R01SessionKey+"Attempted",true);
                    throw new InvalidOperationException("현 창의 소비 상태와 세션 잠금이 다릅니다. 재관측하지 않습니다.");
                }
                var complete=SessionState.GetBool(R01SessionKey+"Complete",false);
                var passed=SessionState.GetBool(R01SessionKey+"Passed",false);
                var text=SessionState.GetString(R01SessionKey+"Result","");
                var error=SessionState.GetString(R01SessionKey+"Error","");
                if(_r01ReadAttempted && _r01ReadComplete &&
                    (!complete || _r01ReadPassed!=passed || _r01ReadResult!=text || _r01ReadError!=error))
                    throw new InvalidOperationException("현 창과 세션의 고정 결과가 다릅니다. 재관측하지 않습니다.");
                _r01ReadAttempted=true;
                _r01ReadComplete=complete && !string.IsNullOrEmpty(text);
                _r01ReadPassed=_r01ReadComplete && passed && string.IsNullOrEmpty(error);
                _r01ReadError=_r01ReadComplete?error:"세션 시도는 소비됐으나 결과가 미완료입니다. 재관측하지 않습니다.";
                _r01ReadResult=!string.IsNullOrEmpty(text)?text:"결과 미관측 또는 미완료 · 재읽기 허용 없음";
            }
            catch(Exception error)
            {
                _r01ReadAttempted=true;_r01ReadPassed=false;_r01ReadComplete=false;_r01SessionConflict=true;
                _r01ReadError="세션 상태 복원 실패: "+error.Message;
                if(string.IsNullOrEmpty(_r01ReadResult))_r01ReadResult="결과 미관측 · 재읽기 허용 없음";
            }
        }

        // REQ/AC-C1B-G3OBS-005/006/007/012: 잠금 선행, 실제 읽은 값만 고정; 객체/파일/권위 생성0.
        private void ReadUnarmedR01Once()
        {
            RestoreR01SessionRead();
            if(_r01ReadAttempted || _r01SessionConflict)return;
            _r01ReadAttempted=true;
            _r01ReadPassed=false;_r01ReadComplete=false;_r01ReadError="";
            var result=new StringBuilder("최초 R01-A 단회 관측 · 아래에 없는 값은 미관측\n");
            try
            {
                SessionState.SetBool(R01SessionKey+"Attempted",true);
                SessionState.SetBool(R01SessionKey+"Complete",false);
                if(_armed || _caseId!=DiagnosticsCase || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
                    throw new InvalidOperationException("미무장 R01-A 편집 상태가 아닙니다.");
                result.AppendLine("관측 창 식별값: "+EntityId.ToULong(GetEntityId()).ToString(System.Globalization.CultureInfo.InvariantCulture));
                result.AppendLine("보유 게임 창 식별값: "+(_receivedGameViewId??"미관측"));
                if(_receivedGameView==null)throw new InvalidOperationException("보유 게임 창 원본이 없습니다.");
                Observer.ParseGameViewIdDecimal(_receivedGameViewId);
                var gameId=EntityId.ToULong(_receivedGameView.GetEntityId()).ToString(System.Globalization.CultureInfo.InvariantCulture);
                result.AppendLine("실제 게임 창 식별값: "+gameId);
                if(gameId!=_receivedGameViewId)throw new InvalidOperationException("보유 게임 창 원본 식별값이 다릅니다.");
                var scene=SceneManager.GetActiveScene();
                result.AppendLine("장면 유효: "+scene.IsValid()+" · 로드: "+scene.isLoaded);
                result.AppendLine("장면 이름: "+scene.name);
                result.AppendLine("장면 경로: ["+scene.path+"] · 장면 핸들: "+scene.handle.GetRawData().ToString(System.Globalization.CultureInfo.InvariantCulture));
                var hosts=UnityEngine.Object.FindObjectsByType<Chapter1EncounterLifetimeOwner>(FindObjectsInactive.Include,FindObjectsSortMode.None);
                result.AppendLine("비활성 포함 호스트 수: "+hosts.Length);
                var collectors=UnityEngine.Object.FindObjectsByType<Observer>(FindObjectsInactive.Include,FindObjectsSortMode.None);
                result.AppendLine("비활성 포함 수집기 수: "+collectors.Length);
                if(hosts.Length!=1)throw new InvalidOperationException("호스트가 정확히 하나가 아닙니다. 호스트 세부값은 미관측입니다.");
                var host=hosts[0];
                result.AppendLine("호스트 식별값: "+EntityId.ToULong(host.GetEntityId()).ToString(System.Globalization.CultureInfo.InvariantCulture));
                result.AppendLine("호스트 활성/사용: "+host.isActiveAndEnabled+" · 자체 활성: "+host.gameObject.activeSelf+" · 계층 활성: "+host.gameObject.activeInHierarchy);
                result.AppendLine("호스트 장면 핸들: "+host.gameObject.scene.handle.GetRawData().ToString(System.Globalization.CultureInfo.InvariantCulture)+" · 활성 장면 동일: "+(host.gameObject.scene==scene));
                var serialized=new SerializedObject(host);
                var prefabProperty=serialized.FindProperty("_reviewPrefab");var roomProperty=serialized.FindProperty("_reviewRoom");var socketProperty=serialized.FindProperty("_reviewSocket");
                result.AppendLine("프리팹/방/소켓 속성 존재: "+(prefabProperty!=null)+" / "+(roomProperty!=null)+" / "+(socketProperty!=null));
                if(prefabProperty==null || roomProperty==null || socketProperty==null)throw new InvalidOperationException("검수 속성이 없습니다. 누락 속성값은 미관측입니다.");
                var actualPrefab=prefabProperty.objectReferenceValue as GameObject;
                result.AppendLine("실제 프리팹 참조 존재: "+(actualPrefab!=null));
                result.AppendLine("호스트 방: "+roomProperty.stringValue+" · 소켓: "+socketProperty.stringValue);
                if(actualPrefab==null)throw new InvalidOperationException("실제 프리팹 참조가 없습니다. 자산 경로와 방 세부값은 미관측입니다.");
                result.AppendLine("실제 프리팹 자산 경로: "+AssetDatabase.GetAssetPath(actualPrefab));
                var expectedPrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/R01.prefab");
                result.AppendLine("기준 R01 참조 존재: "+(expectedPrefab!=null)+" · 참조 동일: "+(expectedPrefab!=null && actualPrefab==expectedPrefab));
                var room=actualPrefab.GetComponent<Chapter1RoomInstance>();
                result.AppendLine("방 구성요소 존재: "+(room!=null));
                if(room==null)throw new InvalidOperationException("방 구성요소가 없습니다. 방 세부값은 미관측입니다.");
                var supports=room.SupportsSocketPlan("S01-A");var accepted=room.IsBasicMovementAccepted;
                result.AppendLine("실제 방 ID: "+room.RoomId+" · S01-A 지원: "+supports+" · 기본 이동 수용: "+accepted);
                if(!scene.IsValid() || !scene.isLoaded || scene.name!="Chapter1BasicMovementReviewR01" || !string.IsNullOrEmpty(scene.path) ||
                    !host.isActiveAndEnabled || !host.gameObject.activeSelf || !host.gameObject.activeInHierarchy || host.gameObject.scene!=scene || collectors.Length!=0 ||
                    expectedPrefab==null || actualPrefab!=expectedPrefab || roomProperty.stringValue!="R01" || socketProperty.stringValue!="S01-A" || room.RoomId!="R01" || !supports || accepted)
                    throw new InvalidOperationException("실제 R01 미저장 장면·호스트·수집기·참조 구성의 성공 기준과 다릅니다.");
                _r01ReadPassed=true;
                result.AppendLine("판정: 성공 · 읽기 당시 R01 구성만 확인");
            }
            catch(Exception error){_r01ReadError=error.Message;result.AppendLine("최초 실패: "+error.Message);}
            finally
            {
                _r01ReadResult=result.ToString();
                try
                {
                    if(!SessionState.GetBool(R01SessionKey+"Attempted",false))
                        throw new InvalidOperationException("세션 시도 잠금 기록을 확인하지 못했습니다.");
                    SessionState.SetString(R01SessionKey+"Result",_r01ReadResult);
                    SessionState.SetString(R01SessionKey+"Error",_r01ReadError??"");
                    SessionState.SetBool(R01SessionKey+"Passed",_r01ReadPassed);
                    SessionState.SetBool(R01SessionKey+"Complete",true);
                    _r01ReadComplete=true;
                }
                catch(Exception error)
                {
                    _r01ReadPassed=false;_r01ReadComplete=false;_r01SessionConflict=true;
                    _r01ReadError=(string.IsNullOrEmpty(_r01ReadError)?"":_r01ReadError+"; ")+"세션 결과 보존 실패: "+error.Message;
                }
            }
        }

        private void OnGUI()
        {
            // REQ/AC-R01PLACE-001..003: 필수 동작은 높이370 안, 상세 원문은 아래 스크롤이다.
            EditorGUILayout.LabelField("기본 이동 관측",EditorStyles.boldLabel,GUILayout.Height(DockActionRow));
            EditorGUILayout.LabelField("현재 상태",_armed?"준비됨":(_initialPassed?"초기 확인됨":"미준비"),GUILayout.Height(DockActionRow));
            RestoreR01SessionRead();
            using(new EditorGUI.DisabledScope(_r01ReadAttempted || _r01SessionConflict || _armed || _caseId!=DiagnosticsCase || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating))
                if(GUILayout.Button("미무장 R01 상태 단회 읽기",GUILayout.Height(DockActionRow)))ReadUnarmedR01Once();
            using(new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode || _armed))
            {
                using(new EditorGUI.DisabledScope(_neutralActive||_bindingAttempts>=2||_prepareRequested))if(GUILayout.Button("현재 승인 파일 연결",GUILayout.Height(DockActionRow)))ConnectCurrentApproval();
                using(new EditorGUI.DisabledScope(_initialAttempted||_neutralActive||_prepareRequested))if(GUILayout.Button("현재 입력 상태 확인",GUILayout.Height(DockActionRow)))BeginNeutral(false);
                _caseId=EditorGUILayout.TextField("방·소켓 구성",_caseId,GUILayout.Height(DockActionRow));
                using(new EditorGUI.DisabledScope(string.IsNullOrEmpty(_boundPath)||!_initialPassed||_neutralActive||_prepareRequested||_prepareConsumed))if(GUILayout.Button("승인 원문 확인 후 관측기 준비",GUILayout.Height(DockActionRow)))BeginNeutral(true);
            }
            using(new EditorGUI.DisabledScope(!_armed || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating))
                if(GUILayout.Button("종료 후 임시 관측기 정리",GUILayout.Height(DockActionRow)))CleanupOwnedRoot();
            using(new EditorGUI.DisabledScope(!diagnosticsPrepared || diagnosticsAttempted || diagnosticsPermanentlyBlocked || !_armed || !_playEntered || _caseId!=DiagnosticsCase || !EditorApplication.isPlaying || !EditorApplication.isPaused || EditorApplication.isCompiling || EditorApplication.isUpdating))
                if(GUILayout.Button("숫자 원시 형식 검사",GUILayout.Height(DockActionRow)))RunUInt64DiagnosticsOnce();
            using(new EditorGUI.DisabledScope(_explicitResumeCount!=0||_closedDiagnosticsPin==null||_diagnosticsResult==null||!_diagnosticsResult.passed||_collector==null||!_collector.FirstPauseDiagnosticReadyForAuthoring))
                if(GUILayout.Button("독립 수용 판정 뒤 재개",GUILayout.Height(DockActionRow)))ResumeAfterIndependentAcceptanceOnce();
            _detailsScroll=EditorGUILayout.BeginScrollView(_detailsScroll);
            EditorGUILayout.LabelField("관측 창 식별값",EntityId.ToULong(GetEntityId()).ToString(System.Globalization.CultureInfo.InvariantCulture));
            EditorGUILayout.LabelField("보유 GameView 식별값",_receivedGameViewId??"미관측");
            EditorGUILayout.LabelField("현재 세션",CurrentTuple());
            EditorGUILayout.SelectableLabel(_boundPath??"현재 승인 미연결",GUILayout.Height(36));
            EditorGUILayout.HelpBox(_bindingMessage,MessageType.Info);
            EditorGUILayout.HelpBox("기존 검수 메뉴로 방을 연 다음 승인 원문을 결속하고 관측기를 준비합니다. 실제 재생·입력·종료는 실행 담당자가 조작합니다.",MessageType.Info);
            if(_r01ReadAttempted)
            {
                EditorGUILayout.LabelField("R01 상태 읽기",!_r01ReadComplete?"시도됨 · 결과 미완료":(_r01ReadPassed?"시도됨 · 성공":"시도됨 · 실패"));
                if(!string.IsNullOrEmpty(_r01ReadError))EditorGUILayout.HelpBox(_r01ReadError,MessageType.Error);
                _r01ReadScroll=EditorGUILayout.BeginScrollView(_r01ReadScroll,GUILayout.Height(280));
                var fixedText=_r01ReadResult??"결과 미관측 · 재읽기 허용 없음";
                EditorGUILayout.SelectableLabel(fixedText,GUILayout.Height(Math.Max(280,(fixedText.Split('\n').Length+2)*18)));
                EditorGUILayout.EndScrollView();
                EditorGUILayout.HelpBox("단회 읽기 당시의 결과입니다. 실행 승인·소스 준비 완료·방 수용을 뜻하지 않습니다. 후속 준비 검사는 그대로 필요합니다.",MessageType.Info);
            }
            if(_diagnosticsResult!=null)
            {
                EditorGUILayout.LabelField("단회 진단 최종 상태",_diagnosticsResult.passed?"완료":"미완료 또는 실패");
                EditorGUILayout.LabelField("진단 파일 실제 폐쇄",_diagnosticsResult.fileClosed?"확인됨":"미확인");
                EditorGUILayout.LabelField("진단 전체 경과 초",_diagnosticsResult.elapsedSeconds.ToString("R",System.Globalization.CultureInfo.InvariantCulture));
                EditorGUILayout.LabelField("진단 시도 세션",diagnosticsAttemptedSessionId);
            }
            if(!string.IsNullOrEmpty(_message))EditorGUILayout.HelpBox("보존 기록 · 현재 세션 성공 권위가 아님: "+_message,MessageType.Info);
            if(!string.IsNullOrEmpty(_firstFailureJson))EditorGUILayout.HelpBox(_firstFailureJson,MessageType.Error);
            if(!string.IsNullOrEmpty(_latestJson))
            {
                Observer.SampleRow row=null;
                try{row=ReadSampleProjection(_latestJson);}
                catch(Exception error){_message="보존 표본 판독 실패: "+error.Message;EditorGUILayout.HelpBox(_message,MessageType.Error);}
                if(row!=null)
                {
                    EditorGUILayout.LabelField("방·소켓",row.caseId+" / "+row.socketId);
                    EditorGUILayout.LabelField("체력·완료 틱",row.health+" / "+row.tick);
                    EditorGUILayout.LabelField("지지·벽·동작",(row.isGrounded?"지지 있음":"공중")+" / "+row.wallId+" / "+row.actionState);
                    EditorGUILayout.LabelField("전이 상태",row.transferPlayerState+" / "+row.transferActiveTarget);
                    EditorGUILayout.LabelField("연속 표본·중복",row.distinctCount+" / "+row.duplicateCount);
                    EditorGUILayout.LabelField("현재 관측",_collector!=null && _collector.Live?"수집 중":"종료 또는 미관측");
                }
                _scroll=EditorGUILayout.BeginScrollView(_scroll);EditorGUILayout.SelectableLabel(_latestJson,EditorStyles.wordWrappedLabel,GUILayout.MinHeight(160));EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndScrollView();
        }
        private static bool PauseApprovalAllowed(bool authorized,string caseId,int limit,bool resumeAuthorized)=>authorized&&caseId=="R01-A"&&limit==1&&resumeAuthorized;
        private static bool PauseApprovalAllowed(ExecutionApproval approval)=>approval!=null&&PauseApprovalAllowed(approval.firstSamplePauseAuthorizedNow,approval.firstSamplePauseCaseId,approval.firstSamplePauseInvocationLimit,approval.resumeAfterUInt64AcceptedAuthorizedNow);
        bool Observer.IReceivedInputContext.IsActualPlayPaused()=>EditorApplication.isPaused;
        bool Observer.IReceivedInputContext.VerifyCurrentAuthority()=>SameCurrentPlayAuthority();
        bool Observer.IReceivedInputContext.TryPauseFirstSample(Observer collector,Observer.FirstPauseRequest request,out bool before,out bool after,out int setterCount)
        {
            before=EditorApplication.isPaused;after=before;setterCount=_firstPauseSetterCount;
            try
            {
                DiagnosticRequire(_armed&&_placementReady&&ReferenceEquals(collector,_collector)&&collector!=null&&collector.ExactFirstPauseRequestForAuthoring(request)&&request.caseId==DiagnosticsCase&&request.sessionId==_sessionId&&request.approvalSha==_receivedExecutionSha&&SameCurrentPlayAuthority(),"첫 표본 정지의 현재 창·수집기·승인 권위가 다릅니다.");
                var approval=ReadLockedApproval(_executionPath,_receivedExecutionLength,_receivedExecutionSha);
                DiagnosticRequire(PauseApprovalAllowed(approval)&&_firstPauseSetterCount==0&&EditorApplication.isPlaying&&!before&&!EditorApplication.isCompiling&&!EditorApplication.isUpdating,"첫 표본 단회 정지 조건이 다릅니다.");
                VerifyToolPins(approval.observerSource,approval.observerMeta,approval.windowSource,approval.windowMeta);
                _firstPauseSetterCount++;setterCount=_firstPauseSetterCount;EditorApplication.isPaused=true;after=EditorApplication.isPaused;
                return after&&EditorApplication.isPlaying;
            }
            catch(Exception error){after=EditorApplication.isPaused;setterCount=_firstPauseSetterCount;_message="첫 표본 자동 정지 실패: "+error.Message;return false;}
        }
        private void ObservePauseLifetime(PauseState state)
        {if(_armed&&_collector!=null&&EditorApplication.isPlaying)_collector.ObserveActualPauseForAuthoring(state==PauseState.Paused);}
        private void ResumeAfterIndependentAcceptanceOnce()
        {
            try
            {
                DiagnosticLive();
                DiagnosticRequire(_explicitResumeCount==0&&_diagnosticsResult!=null&&_diagnosticsResult.passed&&_diagnosticsResult.validationPassed&&_diagnosticsResult.fileClosed&&_closedDiagnosticsPin!=null&&!diagnosticsPermanentlyBlocked,"독립 수용 뒤 명시 재개의 폐쇄 진단 조건이 다릅니다.");
                var approval=ReadLockedApproval(_executionPath,_receivedExecutionLength,_receivedExecutionSha);
                DiagnosticRequire(PauseApprovalAllowed(approval)&&SameCurrentPlayAuthority(),"재개 승인 권위가 다릅니다.");
                Observer.VerifyPin(_closedDiagnosticsPin,EvidenceRoot());DiagnosticExistingBudget(_diagnosticsConfigurationJson,_collector.LatestRowJson);
                _receivedGameView.Focus();DiagnosticRequire(EditorWindow.focusedWindow==_receivedGameView,"명시 재개 직전 원래 GameView 포커스를 확인하지 못했습니다.");
                _collector.VerifyResumeInputRouteForAuthoring();
                _explicitResumeCount++;_collector.BeginAcceptedResumeForAuthoring(_receivedExecutionSha);
                EditorApplication.isPaused=false;_collector.CompleteAcceptedResumeForAuthoring();
                _message="독립 수용 판정 뒤 담당자의 공개 재개 요청을 단회 기록했습니다.";
            }
            catch(Exception error){_message="명시 재개 최초 실패: "+error.Message;if(_collector!=null)_collector.RejectReceivedInputForAuthoring(_message);}
        }
        bool Observer.IReceivedInputContext.TryReadReceivedInputContext(out Observer.ReceivedInputContext context)
        {
            context=default;
            if(!_armed || _collector==null || _receivedGameView==null ||
                _collector.ConfiguredSessionIdForAuthoring!=_sessionId || _collector.CaseId!=_caseId ||
                string.IsNullOrEmpty(_receivedGameViewId) ||
                EntityId.ToULong(_receivedGameView.GetEntityId()).ToString(System.Globalization.CultureInfo.InvariantCulture)!=_receivedGameViewId)
                return false;
            context=new Observer.ReceivedInputContext(_sessionId,_caseId,_receivedGameViewId,EditorApplication.isPlaying,
                EditorApplication.isPaused,EditorApplication.isCompiling,EditorApplication.isUpdating,
                Application.isFocused && EditorWindow.focusedWindow==_receivedGameView);
            return true;
        }
        bool Observer.IReceivedInputContext.TryRestoreReceivedInputContext(Observer collector)
        {
            // REQ/AC-R01PLAY-001/002: no alternate window search and no second logical Bind.
            if(!_armed||!_expectedPlayTransition||collector==null||collector.IsSealed||
                collector.ConfiguredSessionIdForAuthoring!=_sessionId||collector.transform.parent!=null||collector.gameObject.name!=_rootName||
                _receivedGameView==null||string.IsNullOrEmpty(_receivedExecutionSha)||_receivedExecutionLength<=0)
            {if(collector!=null)collector.RecordPlayTransitionForAuthoring("bridge-check","armed="+_armed+" expected="+_expectedPlayTransition+" sealed="+collector.IsSealed+" sessionSame="+(collector.ConfiguredSessionIdForAuthoring==_sessionId)+" rootOnly="+(collector.transform.parent==null)+" rootNameSame="+(collector.gameObject.name==_rootName)+" gameView="+(_receivedGameView!=null)+" shaPresent="+!string.IsNullOrEmpty(_receivedExecutionSha)+" lengthPositive="+(_receivedExecutionLength>0));return false;}
            var approval=ReadLockedApproval(_executionPath,_receivedExecutionLength,_receivedExecutionSha);
            collector.RecordPlayTransitionForAuthoring("bridge-tuple","user="+CurrentUserActor(approval)+" sessionSame="+(approval.sessionId==_sessionId)+" gameIdSame="+(approval.inputGameViewWindowId==EntityId.ToULong(_receivedGameView.GetEntityId()))+" gameIdTextSame="+(approval.inputGameViewWindowId.ToString(CultureInfo.InvariantCulture)==_receivedGameViewId)+" windowIdSame="+(approval.uint64DiagnosticsWindowId==EntityId.ToULong(GetEntityId())));
            if(!CurrentUserActor(approval)||approval.sessionId!=_sessionId||
                approval.inputGameViewWindowId!=EntityId.ToULong(_receivedGameView.GetEntityId())||
                approval.inputGameViewWindowId.ToString(CultureInfo.InvariantCulture)!=_receivedGameViewId||
                approval.uint64DiagnosticsWindowId!=EntityId.ToULong(GetEntityId()))return false;
            using(var current=System.Diagnostics.Process.GetCurrentProcess())
                if(!BindingIdentityMatches(approval.unityPid,approval.unityStartTicks,approval.inputGameViewWindowId,approval.uint64DiagnosticsWindowId,current.Id,current.StartTime.ToUniversalTime().Ticks,EntityId.ToULong(_receivedGameView.GetEntityId()),EntityId.ToULong(GetEntityId()))||!Observer.SamePath(current.MainModule.FileName,approval.unityExe))return false;
            var found=0;foreach(var candidate in UnityEngine.Object.FindObjectsByType<Observer>(FindObjectsInactive.Include,FindObjectsSortMode.None))
                if(candidate.ConfiguredSessionIdForAuthoring==_sessionId&&candidate.transform.parent==null&&candidate.gameObject.name==_rootName){if(candidate!=collector)return false;found++;}
            if(found!=1)return false;
            _collector=collector;_receivedBridgeBound=true;return true;
        }
        private bool SameCurrentPlayAuthority()
        {
            if(_receivedGameView==null||_receivedExecutionLength<=0)return false;
            var approval=ReadLockedApproval(_executionPath,_receivedExecutionLength,_receivedExecutionSha);
            using(var current=System.Diagnostics.Process.GetCurrentProcess())
                return CurrentUserActor(approval)&&approval.sessionId==_sessionId&&approval.inputGameViewWindowId.ToString(CultureInfo.InvariantCulture)==_receivedGameViewId&&
                    BindingIdentityMatches(approval.unityPid,approval.unityStartTicks,approval.inputGameViewWindowId,approval.uint64DiagnosticsWindowId,current.Id,current.StartTime.ToUniversalTime().Ticks,EntityId.ToULong(_receivedGameView.GetEntityId()),EntityId.ToULong(GetEntityId()))&&Observer.SamePath(current.MainModule.FileName,approval.unityExe);
        }
        private void RemoveExpectedSceneLoaded()
        {if(_sceneLoadedSubscribed){SceneManager.sceneLoaded-=OnExpectedSceneLoaded;_sceneLoadedSubscribed=false;}}
        private void OnExpectedSceneLoaded(Scene scene,LoadSceneMode mode)
        {
            try
            {
                if(!_armed||!_expectedPlayTransition||_sceneLoadedConsumed||!_sceneLoadedSubscribed)throw new IOException("중복 또는 승인 밖 sceneLoaded입니다.");
                _sceneLoadedConsumed=true;
                if(!scene.IsValid()||!scene.isLoaded||scene.handle.GetRawData()!=_expectedPlaySceneHandle||!SameCurrentPlayAuthority())throw new IOException("sceneLoaded 실제 장면 또는 승인 신원이 다릅니다.");
                FindOwnedCollector();if(_collector==null||_collector.gameObject.scene!=scene)throw new IOException("sceneLoaded의 동일 승인 collector가 유일하지 않습니다.");
                _collector.RestoreSceneLoadedForAuthoring(this,scene);
                if(_collector.IsSealed)throw new IOException("sceneLoaded 복원은 실패로 봉인됐습니다.");
            }
            catch(Exception error){if(_collector!=null)_collector.RejectReceivedInputForAuthoring(error.Message);_message=error.Message;}
            finally{RemoveExpectedSceneLoaded();}
        }
        private void TryBindReceivedContext()
        {
            if(!_armed || _collector==null || _collector.IsSealed)return;
            try
            {
                if(!_receivedBridgeBound)
                {
                    var approval=ReadLockedApproval(_executionPath,_receivedExecutionLength,_receivedExecutionSha);
                    if(approval.sessionId!=_sessionId ||
                        !CurrentUserActor(approval) || _receivedGameView==null ||
                        approval.inputGameViewWindowId!=EntityId.ToULong(_receivedGameView.GetEntityId()) ||
                        approval.inputGameViewWindowId.ToString(System.Globalization.CultureInfo.InvariantCulture)!=_receivedGameViewId)
                        throw new IOException("재로드 후 같은 승인 tuple·보유 GameView 원본이 아닙니다.");
                    _collector.BindReceivedInputContextForAuthoring(this);_receivedBridgeBound=true;
                }
                if(EditorApplication.isPlaying && _collector.ReceivedCallbacksRegisteredForAuthoring)_collector.PrepareReceivedInputForAuthoring();
            }
            catch(Exception error)
            {_collector.RejectReceivedInputForAuthoring("입력 bridge 결속 실패: "+error.Message);_message=error.Message;}
        }
        private static bool CurrentUserActor(ExecutionApproval approval)=>approval!=null && approval.owner=="Root" &&
            approval.performer=="User" && approval.performerModel=="" && approval.performerRole=="사용자 직접 플레이" &&
            approval.inputEvidenceKind=="UnityReceivedAllowedControls" && approval.inputGameViewWindowId!=0;

        private void TryPrepare()
        {
            GameObject temporary=null;
            try
            {
                BindRequire(PrepareReady(!string.IsNullOrEmpty(_boundPath),_initialPassed,_prepareRequested,_prepareConsumed,_neutralGuard,_neutralClosed,string.IsNullOrEmpty(_neutralFailure))&&_neutralStage=="prepare","현재 공개 준비 요청의 단회 진입이 아닙니다.");_prepareConsumed=true;RequireNeutralGuard();
                RequireEditorIdle();
                if(_armed)throw new InvalidOperationException("기존 사례의 종료와 자기 임시 객체 정리가 먼저입니다.");
                var approval=ReadBoundApproval();
                ValidateDiagnosticsApproval(approval);
                if(_caseId==DiagnosticsCase)
                {
                    if(diagnosticsPrepared || diagnosticsAttempted || diagnosticsPermanentlyBlocked)throw new IOException("최초 숫자 진단 준비는 재무장할 수 없습니다.");
                    diagnosticsPrepared=true;diagnosticsPreparedSessionId=approval.sessionId;
                    diagnosticsPreparedExecutionSha256=Observer.Sha(_executionPath);
                    diagnosticsPreparedWindowId=approval.uint64DiagnosticsWindowId.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                if(approval==null || approval.schemaVersion!=1 || approval.status!="Approved" || !approval.finalApproval || !approval.guiExecutionAuthorizedNow ||
                    !CurrentUserActor(approval) || !Astra(approval.participant) || string.IsNullOrEmpty(approval.sessionId) ||
                    approval.caseBudgetSeconds!=1800 || approval.totalBudgetSeconds!=18000 || !approval.inputNeutralObserved ||
                    approval.caseElapsedSecondsAtInstall<0 || approval.caseElapsedSecondsAtInstall>=1800 || approval.totalElapsedSecondsAtInstall<0 || approval.totalElapsedSecondsAtInstall>=18000 ||
                    approval.unityPid<=0 || approval.unityStartTicks<=0 || string.IsNullOrEmpty(approval.windowIdentity) || string.IsNullOrEmpty(approval.inputTarget) ||
                    string.IsNullOrEmpty(approval.unityExe) ||
                    approval.unityVersion!="6000.6.0f1")throw new InvalidOperationException("명명된 실제 GUI 실행 승인·담당·입력 중립이 없습니다.");
                RequireExactCases(approval.cases);
                if(_receivedGameView==null || approval.inputGameViewWindowId!=EntityId.ToULong(_receivedGameView.GetEntityId()) ||
                    _receivedGameViewId!=approval.inputGameViewWindowId.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    throw new IOException("실제로 게임 포커스였던 보유 GameView 원본과 승인 식별값이 다릅니다.");
                Observer.ParseGameViewIdDecimal(_receivedGameViewId);
                _receivedExecutionSha=Observer.Sha(_executionPath);
                _receivedExecutionLength=new FileInfo(_executionPath).Length;
                CaseApproval selected=null;foreach(var item in approval.cases)if(item.caseId==_caseId)selected=item;
                if(selected==null)throw new InvalidOperationException("정확10 구성의 지정 사례가 아닙니다.");
                using(var current=System.Diagnostics.Process.GetCurrentProcess())
                    if(current.Id!=approval.unityPid || current.StartTime.ToUniversalTime().Ticks!=approval.unityStartTicks ||
                        !Observer.SamePath(current.MainModule.FileName,approval.unityExe) || Application.unityVersion!=approval.unityVersion)
                        throw new InvalidOperationException("실제 Unity 프로세스와 창 승인 원문이 다릅니다.");
                foreach(var item in approval.cases)
                {
                    if(item==selected)break;
                    var previous=ReadSealRow(item.sealPath,EvidenceRoot(),Observer.MaxEventBytes);
                    if(previous==null || previous.sessionId!=approval.sessionId || previous.caseId!=item.caseId || previous.live ||
                        !previous.sourcePinsUnchanged || !previous.continuousFromZero || previous.firstTick!=0 || previous.exitCandidateTick<0 ||
                        !string.IsNullOrEmpty(previous.firstFailureJson) || string.IsNullOrEmpty(previous.completionKey) ||
                        previous.sourceCheckpointSha256!=approval.sourceCheckpoint.sha256 || previous.observerHelperSha256!=approval.observerSource.sha256)
                        throw new InvalidOperationException("앞선 정확 구성의 정상 종료 원시가 없습니다. 나머지 사례를 진행하지 않습니다.");
                    Observer.VerifyPin(previous.samplesFile,EvidenceRoot());Observer.VerifyPin(previous.eventsFile,EvidenceRoot());
                    RequireReceivedSeal(previous,item.inputEvidencePath);
                }
                foreach(var item in approval.cases)
                {
                    Observer.RequireDistinctRawPaths(item.samplesPath,item.eventsPath,item.sealPath,item.nativeEvidencePath,item.inputEvidencePath);
                    foreach(var raw in new[]{item.samplesPath,item.eventsPath,item.sealPath,item.nativeEvidencePath,item.inputEvidencePath})Observer.RequireInside(raw,EvidenceRoot());
                }
                foreach(var raw in new[]{selected.samplesPath,selected.eventsPath,selected.sealPath,selected.inputEvidencePath})
                {Observer.RequireInside(raw,EvidenceRoot());if(File.Exists(raw) || Directory.Exists(raw))throw new IOException("같은 사례 원시는 재실행하거나 덮어쓰지 않습니다.");}
                var source=ReadCheckpoint(approval.sourceCheckpoint);VerifyAllSources(source.files);
                VerifyToolPins(approval.observerSource,approval.observerMeta,approval.windowSource,approval.windowMeta);
                if(_caseId==DiagnosticsCase)PrepareDiagnosticByteBudget(approval,selected,_executionPath);
                Observer.VerifyApprovedContract(approval.approvedContract,ProjectRoot());Observer.VerifyPin(approval.toolObservation,DeliveryRoot());
                Observer.VerifyPin(selected.prefab,ProjectRoot());Observer.VerifyPin(selected.prefabMeta,ProjectRoot());
                VerifyExpectedAsset(selected.prefab,"Assets/Prefabs/Chapter1/"+selected.roomId+".prefab");
                VerifyExpectedAsset(selected.prefabMeta,"Assets/Prefabs/Chapter1/"+selected.roomId+".prefab.meta");
                var scene=SceneManager.GetActiveScene();
                if(!scene.IsValid() || !scene.isLoaded || !string.IsNullOrEmpty(scene.path) || scene.name!=selected.sceneName ||
                    scene.name!="Chapter1BasicMovementReview"+selected.roomId)throw new InvalidOperationException("기존 메뉴가 만든 지정 미저장 scene이 아닙니다.");
                var hosts=UnityEngine.Object.FindObjectsByType<Chapter1EncounterLifetimeOwner>(FindObjectsInactive.Include,FindObjectsSortMode.None);
                if(hosts.Length!=1 || !hosts[0].isActiveAndEnabled || hosts[0].gameObject.scene!=scene ||
                    UnityEngine.Object.FindObjectsByType<Observer>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length!=0)
                    throw new InvalidOperationException("실제 저작 host1 또는 임시 수집기0 조건이 다릅니다.");
                var host=hosts[0];var serialized=new SerializedObject(host);
                var prefabProperty=serialized.FindProperty("_reviewPrefab");var roomProperty=serialized.FindProperty("_reviewRoom");var socketProperty=serialized.FindProperty("_reviewSocket");
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/"+selected.roomId+".prefab");
                if(prefabProperty==null || roomProperty==null || socketProperty==null || prefab==null ||
                    prefabProperty.objectReferenceValue!=prefab || roomProperty.stringValue!=selected.roomId || socketProperty.stringValue!=selected.socketId)
                    throw new InvalidOperationException("실제 저작 prefab·방·소켓이 승인 구성과 다릅니다.");
                var room=prefab.GetComponent<Chapter1RoomInstance>();
                if(room==null || room.RoomId!=selected.roomId || !room.SupportsSocketPlan(selected.socketId) || room.IsBasicMovementAccepted)
                    throw new InvalidOperationException("미수용 방 후보 또는 지원 소켓이 아닙니다.");
                // 구조/RequiredPath는 기존 저작 읽기 경로다. ApplyModifiedProperties/저작 setter는 호출하지 않는다.
                room.ValidateStructure();
                var observerScript=AssetDatabase.LoadAssetAtPath<MonoScript>(ObserverAsset);var lifetimeScript=AssetDatabase.LoadAssetAtPath<MonoScript>(LifetimeAsset);
                if(observerScript==null || observerScript.GetClass()!=typeof(Observer) || lifetimeScript==null || lifetimeScript.GetClass()!=typeof(Chapter1EncounterLifetimeOwner) ||
                    typeof(Observer).Assembly.GetName().Name!="AcadeGameMaker.Input.Unity" || typeof(Chapter1BasicMovementEvidenceWindow).Assembly.GetName().Name!="AcadeGameMaker.Combat.Authoring.Editor")
                    throw new InvalidOperationException("MonoScript 타입·파일·기존 조립체가 다릅니다.");
                var observerAttribute=Attribute.GetCustomAttribute(typeof(Observer),typeof(DefaultExecutionOrder)) as DefaultExecutionOrder;
                var lifetimeAttribute=Attribute.GetCustomAttribute(typeof(Chapter1EncounterLifetimeOwner),typeof(DefaultExecutionOrder)) as DefaultExecutionOrder;
                var observerOrder=MonoImporter.GetExecutionOrder(observerScript);var lifetimeOrder=MonoImporter.GetExecutionOrder(lifetimeScript);
                if(observerAttribute==null || observerAttribute.order!=210 || lifetimeAttribute==null || lifetimeAttribute.order!=200 ||
                    (observerOrder!=0 && observerOrder!=210) || (lifetimeOrder!=0 && lifetimeOrder!=200))
                    throw new InvalidOperationException("기대 순서 밖의 importer 설정입니다. 순서를 변경하지 않습니다.");
                var pathJson=JsonUtility.ToJson(new RequiredPathProjection{points=room.RequiredPath});
                _rootName="Chapter1BasicMovementEvidence_"+selected.caseId+"_"+approval.sessionId;
                temporary=new GameObject(_rootName);
                if(temporary.scene!=scene)SceneManager.MoveGameObjectToScene(temporary,scene);
                var collector=temporary.AddComponent<Observer>();
                var configuration=new Observer.Configuration
                {
                    sessionId=approval.sessionId,caseId=selected.caseId,roomId=selected.roomId,socketId=selected.socketId,reviewMenu=selected.reviewMenu,sceneName=scene.name,
                    projectPath=ProjectRoot(),evidenceRoot=EvidenceRoot(),samplesPath=selected.samplesPath,eventsPath=selected.eventsPath,sealPath=selected.sealPath,
                    performer=approval.performer,performerModel=approval.performerModel,performerRole=approval.performerRole,
                    inputEvidenceKind=approval.inputEvidenceKind,gameViewInstanceIdDecimal=_receivedGameViewId,
                    unityExe=approval.unityExe,unityVersion=approval.unityVersion,unityPid=approval.unityPid,unityStartTicks=approval.unityStartTicks,windowIdentity=approval.windowIdentity,inputTarget=approval.inputTarget,
                    caseBudgetSeconds=approval.caseBudgetSeconds,totalBudgetSeconds=approval.totalBudgetSeconds,caseElapsedSecondsAtInstall=approval.caseElapsedSecondsAtInstall,totalElapsedSecondsAtInstall=approval.totalElapsedSecondsAtInstall,
                    inputNeutralObserved=approval.inputNeutralObserved,nativeEvidencePath=selected.nativeEvidencePath,inputEvidencePath=selected.inputEvidencePath,
                    sourceCheckpoint=approval.sourceCheckpoint,approvedContract=approval.approvedContract,executionApproval=Pin(_executionPath),toolObservation=approval.toolObservation,
                    observerSource=approval.observerSource,observerMeta=approval.observerMeta,windowSource=approval.windowSource,windowMeta=approval.windowMeta,
                    prefab=selected.prefab,prefabMeta=selected.prefabMeta,sourceFiles=source.files,requiredPathJson=pathJson,authoringHostHierarchy=Observer.Hierarchy(host.transform),
                    installedInstanceId=EntityId.ToULong(collector.GetEntityId()),installedSceneHandle=scene.handle.GetRawData(),collectorAttributeOrder=210,lifetimeAttributeOrder=200,
                    collectorImporterOrder=observerOrder,lifetimeImporterOrder=lifetimeOrder
                };
                if(_caseId==DiagnosticsCase)_diagnosticsConfigurationJson=JsonUtility.ToJson(configuration);
                RequireNeutralGuard();ReadBoundApproval();
                collector.ConfigureForAuthoring(host,configuration);
                _expectedPlaySceneHandle=configuration.installedSceneHandle;
                _collector=collector;_sessionId=approval.sessionId;_sealPath=selected.sealPath;_armed=true;_playEntered=false;_latestJson=null;_firstFailureJson=null;
                _receivedBridgeBound=false;TryBindReceivedContext();
                if(collector.IsSealed)throw new IOException("입력 bridge 최초 결속이 실패했습니다.");
                temporary=null;_message="관측기 준비를 마쳤습니다. 승인된 게임 포커스 확인 후 사용자가 직접 플레이합니다.";
            }
            catch(Exception error){if(diagnosticsPrepared && !_armed)diagnosticsPermanentlyBlocked=true;if(temporary!=null)UnityEngine.Object.DestroyImmediate(temporary);_message="관측기 준비 거절: "+error.Message;}
        }
        [Serializable] private sealed class RequiredPathProjection { public Vector2[] points; }
        [Serializable] private sealed class UInt64DiagnosticResult
        {
            public bool validationPassed,passed,fileClosed;
            public string firstFailureType,firstFailureMessage,startedUtc,endedUtc;
            public double elapsedSeconds;
            public int[][] negatives=new int[190][];
            public string[][] positives=new string[40][];
            public string[][] normal=new string[10][];
            public string metadataJson="{}";
            public UInt64DiagnosticResult()
            {
                for(var v=0;v<4;v++)for(var f=0;f<10;f++)positives[v*10+f]=new[]{f.ToString(),v.ToString(),"","","","0"};
                for(var n=0;n<19;n++)for(var f=0;f<10;f++)negatives[n*10+f]=new[]{f,n,0,0};
                for(var f=0;f<10;f++)normal[f]=new[]{f.ToString(),"","","0"};
            }
        }
        private sealed class DiagnosticRange
        {
            public string path,sha256,observedUtc;
            public long offset,length,readLengthBefore,readLengthAfter;
            public bool includeLf;
        }
        // 같은 handle의 실제 bytes만 해시한다. 활성 writer 원문을 재열지 않는다.
        private sealed class DiagnosticPrefixReader : IDisposable
        {
            private readonly FileStream stream;
            private readonly System.Security.Cryptography.SHA256 hash=System.Security.Cryptography.SHA256.Create();
            private readonly System.Diagnostics.Stopwatch clock;
            private readonly byte[] buffer=new byte[65536];
            private int available,index;
            private long consumed;
            private bool finished;
            public readonly DiagnosticRange range;
            public DiagnosticPrefixReader(string path,long limit,System.Diagnostics.Stopwatch clock)
            {
                this.clock=clock;DiagnosticTime(clock);
                Observer.RequireInside(path,EvidenceRoot());
                stream=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
                try
                {
                    var length=stream.Length;
                    if(length<=0 || length>limit)throw new IOException("진단 원문 prefix 상한 또는 빈 원문입니다.");
                    range=new DiagnosticRange{path=path,offset=0,length=length,readLengthBefore=length,observedUtc=DateTime.UtcNow.ToString("O")};
                }
                catch{stream.Dispose();hash.Dispose();throw;}
            }
            private int Next()
            {
                if(consumed==range.length)return -1;
                if(index==available)
                {
                    DiagnosticTime(clock);
                    available=stream.Read(buffer,0,(int)Math.Min(buffer.Length,range.length-consumed));index=0;
                    if(available==0)throw new IOException("진단 원문 읽기가 소실됐습니다.");
                    hash.TransformBlock(buffer,0,available,buffer,0);DiagnosticTime(clock);
                }
                consumed++;return buffer[index++];
            }
            public string Line(int limit,out DiagnosticRange row)
            {
                DiagnosticTime(clock);var offset=consumed;
                using(var bytes=new MemoryStream())
                {
                    for(;;)
                    {
                        var next=Next();
                        if(next<0)
                        {
                            if(bytes.Length!=0)throw new IOException("진단 원문에 미완료 LF 행이 있습니다.");
                            row=null;return null;
                        }
                        bytes.WriteByte((byte)next);
                        if(next==10)break;
                        if(bytes.Length>limit)throw new IOException("진단 원문 행 상한입니다.");
                    }
                    var raw=bytes.ToArray();
                    if(raw.Length==1 || raw[raw.Length-2]==13)throw new IOException("진단 원문 행 경계가 LF 형식과 다릅니다.");
                    row=new DiagnosticRange{path=range.path,offset=offset,length=raw.Length,includeLf=true,sha256=DiagnosticHash(raw),observedUtc=DateTime.UtcNow.ToString("O")};
                    var text=new UTF8Encoding(false,true).GetString(raw,0,raw.Length-1);DiagnosticTime(clock);return text;
                }
            }
            public void Drain(){while(Next()>=0){}Finish();}
            public void Finish()
            {
                if(finished)return;DiagnosticTime(clock);
                if(consumed!=range.length)throw new IOException("진단 원문 prefix를 전부 읽지 않았습니다.");
                range.readLengthAfter=stream.Length;
                if(range.readLengthAfter!=range.readLengthBefore || stream.ReadByte()!=-1)throw new IOException("진단 판독 도중 원문 길이가 바뀌었습니다.");
                hash.TransformFinalBlock(Array.Empty<byte>(),0,0);range.sha256=DiagnosticHex(hash.Hash);finished=true;DiagnosticTime(clock);
            }
            public void Dispose(){try{stream.Dispose();}finally{hash.Dispose();}}
        }
        private static string DiagnosticHex(byte[] bytes)=>BitConverter.ToString(bytes).Replace("-","");
        private static string DiagnosticHash(byte[] bytes)
        {using(var hash=System.Security.Cryptography.SHA256.Create())return DiagnosticHex(hash.ComputeHash(bytes));}
        private static void DiagnosticTime(System.Diagnostics.Stopwatch clock)
        {if(clock.Elapsed.TotalSeconds>=5)throw new IOException("숫자 진단의 전체 5초 상한에 도달했습니다.");}
        private static void DiagnosticRequire(bool condition,string reason)
        {if(!condition)throw new IOException(reason);}
        private static string DiagnosticNumber(ulong value)=>value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        private void ValidateDiagnosticsApproval(ExecutionApproval approval)
        {
            DiagnosticRequire(approval.uint64DiagnosticsCaseId==DiagnosticsCase && approval.uint64DiagnosticsPath==DiagnosticsPath &&
                Path.GetFullPath(approval.uint64DiagnosticsPath)==DiagnosticsPath && approval.uint64DiagnosticsCallLimitSeconds==5 &&
                approval.uint64DiagnosticsFileLimitBytes==32768 && approval.uint64DiagnosticsInvocationLimit==1 &&
                approval.uint64DiagnosticsWindowId!=0 && approval.uint64DiagnosticsWindowId==EntityId.ToULong(GetEntityId()),"숫자 진단의 승인 창·정확 경로·상수가 다릅니다.");
            Observer.RequireInside(DiagnosticsPath,EvidenceRoot());
            foreach(var item in approval.cases)
                DiagnosticRequire(item.uint64DiagnosticsCaseId==DiagnosticsCase && item.uint64DiagnosticsPath==DiagnosticsPath && Path.GetFullPath(item.uint64DiagnosticsPath)==DiagnosticsPath &&
                    item.uint64DiagnosticsCallLimitSeconds==5 && item.uint64DiagnosticsFileLimitBytes==32768 && item.uint64DiagnosticsInvocationLimit==1 &&
                    item.uint64DiagnosticsAuthorizedNow==(item.caseId==DiagnosticsCase && approval.uint64DiagnosticsAuthorizedNow),"사례별 숫자 진단 필드·권한이 다릅니다.");
            Observer.VerifyPin(approval.uint64DiagnosticsNativeInputAuthority,DeliveryRoot());
        }
        private void DiagnosticLive()
        {
            if(_collector!=null)_collector.VerifyFirstPauseWaitForAuthoring();
            DiagnosticRequire(_armed && _playEntered && _caseId==DiagnosticsCase && _collector!=null && _collector.CaseId==DiagnosticsCase &&
                _collector.FirstPauseDiagnosticReadyForAuthoring &&
                _collector.ConfiguredSessionIdForAuthoring==_sessionId && _collector.Live && !_collector.IsSealed &&
                string.IsNullOrEmpty(_collector.FirstFailureJson) && string.IsNullOrEmpty(_collector.SealError) && string.IsNullOrEmpty(_firstFailureJson) &&
                EditorApplication.isPlaying && EditorApplication.isPaused && !EditorApplication.isCompiling && !EditorApplication.isUpdating,"최초 Pause·같은 수집기·기존 오류0 조건이 다릅니다.");
        }
        private void RunUInt64DiagnosticsOnce()
        {
            if(!diagnosticsPrepared || diagnosticsAttempted || diagnosticsPermanentlyBlocked || _sessionId!=diagnosticsPreparedSessionId ||
                _caseId!=DiagnosticsCase || diagnosticsPreparedWindowId!=DiagnosticNumber(EntityId.ToULong(GetEntityId())))
            {_message="숫자 진단의 최초 준비 tuple이 없거나 이미 소비됐습니다.";return;}
            var clock=System.Diagnostics.Stopwatch.StartNew();var startedUtc=DateTime.UtcNow.ToString("O");
            diagnosticsAttempted=true;diagnosticsAttemptedSessionId=_sessionId;
            var result=new UInt64DiagnosticResult{startedUtc=startedUtc};_diagnosticsResult=result;
            FileStream output=null;var closed=false;
            try
            {
                DiagnosticTime(clock);DiagnosticLive();
                var approval=ReadExecutionApproval(_executionPath,DeliveryRoot(),Observer.MaxEventBytes);DiagnosticTime(clock);
                ValidateDiagnosticsApproval(approval);DiagnosticTime(clock);
                VerifyToolPins(approval.observerSource,approval.observerMeta,approval.windowSource,approval.windowMeta);DiagnosticTime(clock);
                DiagnosticRequire(approval.sessionId==_sessionId && approval.schemaVersion==1 && approval.status=="Approved" && approval.finalApproval && approval.guiExecutionAuthorizedNow &&
                    CurrentUserActor(approval) && Astra(approval.participant) && approval.inputNeutralObserved &&
                    approval.caseBudgetSeconds==1800 && approval.totalBudgetSeconds==18000 && approval.uint64DiagnosticsAuthorizedNow && approval.uint64DiagnosticsNativeInputAuthorizedNow &&
                    approval.unityVersion==Application.unityVersion && approval.uint64DiagnosticsWindowId==EntityId.ToULong(GetEntityId()),"같은 실제 GUI·숫자 진단·native/input 승인이 없습니다.");
                CaseApproval selected=null;foreach(var item in approval.cases)if(item.caseId==DiagnosticsCase)selected=item;
                DiagnosticRequire(selected!=null && selected.uint64DiagnosticsAuthorizedNow,"최초 R01-A 진단 사례 승인이 없습니다.");
                using(var prepared=ParseStrictDocument(_diagnosticsConfigurationJson))
                {
                    var config=prepared.RootElement;var oldPin=FilePin(config,"executionApproval");
                    Observer.VerifyPin(oldPin,DeliveryRoot());DiagnosticTime(clock);
                    DiagnosticRequire(Observer.SamePath(oldPin.path,_executionPath) && oldPin.sha256==diagnosticsPreparedExecutionSha256 &&
                        Text(config,"sessionId")==_sessionId && Text(config,"caseId")==DiagnosticsCase &&
                        Text(config,"projectPath")==ProjectRoot() && Text(config,"samplesPath")==selected.samplesPath && Text(config,"eventsPath")==selected.eventsPath &&
                        Text(config,"sealPath")==selected.sealPath && Text(config,"nativeEvidencePath")==selected.nativeEvidencePath && Text(config,"inputEvidencePath")==selected.inputEvidencePath &&
                        Text(config,"performer")==approval.performer && Text(config,"performerModel")==approval.performerModel && Text(config,"performerRole")==approval.performerRole &&
                        Text(config,"windowIdentity")==approval.windowIdentity && Text(config,"inputTarget")==approval.inputTarget &&
                        Text(config,"unityExe")==approval.unityExe && Text(config,"unityVersion")==approval.unityVersion && Integer(config,"unityPid")==approval.unityPid &&
                        LongInteger(config,"unityStartTicks")==approval.unityStartTicks && SameDiagnosticPin(FilePin(config,"sourceCheckpoint"),approval.sourceCheckpoint) &&
                        SameDiagnosticPin(FilePin(config,"observerSource"),approval.observerSource) && SameDiagnosticPin(FilePin(config,"windowSource"),approval.windowSource),"최초 전달 설정·승인·원문 경로·소스 핀이 달라졌습니다.");
                }
                using(var current=System.Diagnostics.Process.GetCurrentProcess())
                    DiagnosticRequire(current.Id==approval.unityPid && current.StartTime.ToUniversalTime().Ticks==approval.unityStartTicks && Observer.SamePath(current.MainModule.FileName,approval.unityExe),"실제 Unity 프로세스 신원이 다릅니다.");
                DiagnosticTime(clock);
                result.metadataJson=BuildDiagnosticFailureMetadata(approval,selected);DiagnosticTime(clock);
                output=new FileStream(DiagnosticsPath,FileMode.CreateNew,FileAccess.Write,FileShare.Read);
                var owned=_collector;var latest=owned.LatestRowJson;var count=owned.DistinctSampleCount;
                DiagnosticRequire(!string.IsNullOrEmpty(latest) && count>0,"첫 실제 표본이 없습니다.");
                var metadata=ReadDiagnosticContinuity(approval,selected,latest,count,clock,out var configurationJson,out var sampleJson);
                DiagnosticTime(clock);DiagnosticLive();
                DiagnosticRequire(ReferenceEquals(owned,_collector) && owned.LatestRowJson==latest && owned.DistinctSampleCount==count,"Pause 판독 전후 수집 원문이 바뀌었습니다.");
                result=RunUInt64Diagnostics(configurationJson,sampleJson,clock);_diagnosticsResult=result;result.metadataJson=metadata;
                result.startedUtc=startedUtc;
                result.endedUtc=DateTime.UtcNow.ToString("O");result.elapsedSeconds=clock.Elapsed.TotalSeconds;
                DiagnosticTime(clock);DiagnosticLive();DiagnosticExistingBudget(_diagnosticsConfigurationJson,latest);
                DiagnosticRequire(ReferenceEquals(owned,_collector) && owned.LatestRowJson==latest && owned.DistinctSampleCount==count,"진단 종료 수집 원문이 바뀌었습니다.");
                result.metadataJson=CompleteDiagnosticPauseMetadata(result.metadataJson);DiagnosticTime(clock);
                var bytes=SerializeDiagnosticResult(result);DiagnosticTime(clock);
                output.Write(bytes,0,bytes.Length);output.Flush();DiagnosticTime(clock);output.Dispose();output=null;closed=true;
                DiagnosticTime(clock);DiagnosticExistingBudget(_diagnosticsConfigurationJson,latest);DiagnosticTime(clock);clock.Stop();result.fileClosed=true;
                if(!result.validationPassed)throw new IOException("숫자 형식 첫 실패: "+result.firstFailureType+" / "+result.firstFailureMessage);
                _closedDiagnosticsPin=Pin(DiagnosticsPath);result.passed=true;
                result.endedUtc=DateTime.UtcNow.ToString("O");result.elapsedSeconds=clock.Elapsed.TotalSeconds;
                _message="숫자 형식 단회 판독과 진단 파일 폐쇄를 확인했습니다. 독립 검수·방 수용은 미완료입니다.";
            }
            catch(Exception error)
            {
                diagnosticsPermanentlyBlocked=true;result.passed=false;SetDiagnosticFailure(result,error);
                if(output!=null)
                {
                    try
                    {
                        if(output.Length==0 && clock.Elapsed.TotalSeconds<5)
                        {result.endedUtc=DateTime.UtcNow.ToString("O");result.elapsedSeconds=clock.Elapsed.TotalSeconds;var raw=SerializeDiagnosticResult(result);DiagnosticTime(clock);output.Write(raw,0,raw.Length);output.Flush();}
                    }
                    catch(Exception writeError){_message="진단 원시 작성 실패: "+writeError.Message;}
                    finally{try{output.Dispose();closed=true;}catch{closed=false;}output=null;}
                }
                result.fileClosed=closed;
                _message="숫자 진단 최초 실패: "+error.Message+" 실제 재생 종료는 실행 담당자가 처리합니다.";
                try{if(_collector!=null)_collector.SealForAuthoring("숫자 진단 최초 실패: "+error.Message,false);}catch(Exception sealError){_message+=" 봉인 오류: "+sealError.Message;}
            }
            finally{clock.Stop();result.elapsedSeconds=clock.Elapsed.TotalSeconds;result.endedUtc=DateTime.UtcNow.ToString("O");Repaint();}
        }
        private static bool SameDiagnosticPin(Observer.FilePin a,Observer.FilePin b)=>a!=null && b!=null && a.path==b.path && a.length==b.length && a.sha256==b.sha256;
        private static string CompleteDiagnosticPauseMetadata(string json)
        {
            using(var document=ParseStrictDocument(json))using(var bytes=new MemoryStream())
            {
                using(var writer=new Utf8JsonWriter(bytes))
                {
                    writer.WriteStartObject();
                    foreach(var property in document.RootElement.EnumerateObject())
                        if(property.Name!="pauseAtExit" && property.Name!="pauseExitUtc" && property.Name!="pauseExitStamp")property.WriteTo(writer);
                    writer.WriteBoolean("pauseAtExit",EditorApplication.isPaused);writer.WriteString("pauseExitUtc",DateTime.UtcNow.ToString("O"));writer.WriteNumber("pauseExitStamp",System.Diagnostics.Stopwatch.GetTimestamp());
                    writer.WriteString("pauseExitMeaning","승인·연속성·형식 검사 뒤 직렬화 전 실제 관측이며 출력 폐쇄 이후의 미래 사건은 포함하지 않는다.");writer.WriteEndObject();
                }
                return new UTF8Encoding(false,true).GetString(bytes.ToArray());
            }
        }
        private string BuildDiagnosticFailureMetadata(ExecutionApproval approval,CaseApproval selected)
        {
            using(var bytes=new MemoryStream())
            {
                using(var writer=new Utf8JsonWriter(bytes))using(var configuration=ParseStrictDocument(_diagnosticsConfigurationJson))
                {
                    writer.WriteStartObject();writer.WriteString("sessionId",_sessionId);writer.WriteString("caseId",DiagnosticsCase);writer.WriteString("project",ProjectRoot());
                    writer.WriteNumber("unityPid",approval.unityPid);writer.WriteNumber("unityStartTicks",approval.unityStartTicks);writer.WriteString("unityExe",approval.unityExe);writer.WriteString("unityVersion",approval.unityVersion);
                    writer.WriteString("windowId",DiagnosticNumber(approval.uint64DiagnosticsWindowId));writer.WriteBoolean("pauseAtEntry",EditorApplication.isPaused);writer.WriteString("pauseEntryUtc",DateTime.UtcNow.ToString("O"));writer.WriteNumber("pauseEntryStamp",System.Diagnostics.Stopwatch.GetTimestamp());
                    WriteDiagnosticPin(writer,"checkpoint",approval.sourceCheckpoint);WriteDiagnosticPin(writer,"windowSource",approval.windowSource);WriteDiagnosticPin(writer,"observerSource",approval.observerSource);
                    WriteDiagnosticPin(writer,"executionApproval",FilePin(configuration.RootElement,"executionApproval"));WriteDiagnosticPin(writer,"diagnosticScopeAuthority",approval.approvedContract);WriteDiagnosticPin(writer,"nativeInputAuthority",approval.uint64DiagnosticsNativeInputAuthority);
                    writer.WriteString("samplesPath",selected.samplesPath);writer.WriteString("eventsPath",selected.eventsPath);writer.WriteString("nativeEvidencePath",selected.nativeEvidencePath);writer.WriteString("inputEvidencePath",selected.inputEvidencePath);
                    foreach(var name in new[]{"samplesPrefix","eventsPrefix","start","firstSample","lastSample","nativePrefix","inputPrefix","firstTick","currentTick","gapCount","nativePauseEventRange","inputPauseEventRange"})writer.WriteNull(name);
                    writer.WriteEndObject();
                }
                return new UTF8Encoding(false,true).GetString(bytes.ToArray());
            }
        }
        private static void PrepareDiagnosticByteBudget(ExecutionApproval approval,CaseApproval selected,string executionPath)
        {
            // 비실행 길이 산정: JsonUtility 왕복·반례 판독·실제 원시 읽기를 호출하지 않는다.
            var result=new UInt64DiagnosticResult{startedUtc=new string('x',33),endedUtc=new string('x',33),firstFailureType=new string('x',254),firstFailureMessage=new string('x',510)};
            for(var f=0;f<10;f++)result.normal[f]=new[]{f.ToString(),new string('9',20),new string('9',20),"1"};
            for(var v=0;v<4;v++)for(var f=0;f<10;f++)result.positives[v*10+f]=new[]{f.ToString(),v.ToString(),new string('9',20),new string('9',20),new string('9',20),"1"};
            for(var n=0;n<19;n++)for(var f=0;f<10;f++)result.negatives[n*10+f]=new[]{f,n,3,2};
            using(var bytes=new MemoryStream())
            {
                using(var writer=new Utf8JsonWriter(bytes))
                {
                    writer.WriteStartObject();
                    foreach(var pair in new[]{new[]{"sessionId",approval.sessionId},new[]{"caseId",DiagnosticsCase},new[]{"project",ProjectRoot()},new[]{"unityExe",approval.unityExe},new[]{"unityVersion",approval.unityVersion},new[]{"windowIdentity",approval.windowIdentity},new[]{"inputTarget",approval.inputTarget},new[]{"runKey",new string('x',254)}})
                    {ValidateDiagnosticText(pair[1],pair[0]=="project" || pair[0]=="unityExe"?1536:256);writer.WriteString(pair[0],pair[1]);}
                    WriteDiagnosticPin(writer,"checkpoint",approval.sourceCheckpoint);WriteDiagnosticPin(writer,"windowSource",approval.windowSource);WriteDiagnosticPin(writer,"observerSource",approval.observerSource);
                    WriteDiagnosticPin(writer,"executionApproval",new Observer.FilePin{path=Path.GetFullPath(executionPath),length=long.MaxValue,sha256=new string('F',64)});
                    // 실제 승인 경로의 더 긴 JSON escape 길이를 별도로 확보한다.
                    ValidateDiagnosticText(selected.samplesPath,1536);ValidateDiagnosticText(selected.eventsPath,1536);ValidateDiagnosticText(selected.nativeEvidencePath,1536);ValidateDiagnosticText(selected.inputEvidencePath,1536);
                    WriteDiagnosticPin(writer,"diagnosticScopeAuthority",approval.approvedContract);WriteDiagnosticPin(writer,"nativeInputAuthority",approval.uint64DiagnosticsNativeInputAuthority);
                    foreach(var pair in new[]{new[]{"samplesPrefix",selected.samplesPath},new[]{"eventsPrefix",selected.eventsPath},new[]{"start",selected.eventsPath},new[]{"firstSample",selected.samplesPath},new[]{"lastSample",selected.samplesPath},new[]{"nativePrefix",selected.nativeEvidencePath},new[]{"inputPrefix",selected.inputEvidencePath}})
                        WriteDiagnosticRange(writer,pair[0],new DiagnosticRange{path=pair[1],sha256=new string('F',64),observedUtc=new string('x',33),offset=long.MaxValue,length=long.MaxValue,readLengthBefore=long.MaxValue,readLengthAfter=long.MaxValue,includeLf=false});
                    // 실제 고정 메타 필드의 숫자·시각·불리언·null·구문 최대 길이를 유한 2048바이트로 예약한다.
                    writer.WriteString("remainingFixedMetadataBudget",new string('x',2048));writer.WriteEndObject();
                }
                result.metadataJson=new UTF8Encoding(false,true).GetString(bytes.ToArray());
            }
            SerializeDiagnosticResult(result);
        }
        private static void DiagnosticExistingBudget(string configurationJson,string latest)
        {
            using(var configuration=ParseStrictDocument(configurationJson))
            {
                var sample=ReadSampleProjection(latest);
                var elapsed=sample.elapsedSeconds+(System.Diagnostics.Stopwatch.GetTimestamp()-sample.monotonicStamp)/(double)System.Diagnostics.Stopwatch.Frequency;
                DiagnosticRequire(Real(configuration.RootElement,"caseElapsedSecondsAtInstall")+elapsed<1800 && Real(configuration.RootElement,"totalElapsedSecondsAtInstall")+elapsed<18000,"Pause·진단·폐쇄를 포함한 사례/전체 시간 상한입니다.");
            }
        }
        private static void SetDiagnosticFailure(UInt64DiagnosticResult result,Exception error)
        {if(result.firstFailureType==null){result.firstFailureType=error.GetType().FullName;result.firstFailureMessage=error.Message;}}
        private static UInt64DiagnosticResult RunUInt64Diagnostics(string configurationJson,string sampleJson)=>RunUInt64Diagnostics(configurationJson,sampleJson,System.Diagnostics.Stopwatch.StartNew());
        private static UInt64DiagnosticResult RunUInt64Diagnostics(string configurationJson,string sampleJson,System.Diagnostics.Stopwatch clock)
        {
            var result=new UInt64DiagnosticResult();
            try
            {
                DiagnosticTime(clock);
                var configuration=JsonUtility.FromJson<Observer.Configuration>(configurationJson);
                var sample=JsonUtility.FromJson<Observer.SampleRow>(sampleJson);DiagnosticTime(clock);
                using(var configDoc=ParseStrictDocument(configurationJson))using(var sampleDoc=ParseStrictDocument(sampleJson))
                {
                    var projection=ReadSampleProjection(sampleJson);
                    for(var f=0;f<10;f++)
                    {
                        DiagnosticTime(clock);var value=UInt64Integer(f<2?configDoc.RootElement:sampleDoc.RootElement,DiagnosticsFields[f]);
                        var dto=f<2?ConfigurationIdentity(configuration,f):SampleIdentity(sample,f-2);
                        var projected=f<2?value:SampleIdentity(projection,f-2);
                        result.normal[f]=new[]{f.ToString(),DiagnosticNumber(value),DiagnosticNumber(dto),value==dto && dto==projected?"1":"2"};
                        DiagnosticRequire(value==dto && dto==projected,"실제 재로드·표본 숫자 형식이 다릅니다.");DiagnosticTime(clock);
                    }
                }
                for(var v=0;v<4;v++)
                {
                    DiagnosticTime(clock);var value=DiagnosticsValues[v];
                    // 정상 원문으로부터 분리한 DTO만 바꾼다. 수집기·실제 설정에 경계값을 대입하지 않는다.
                    var c=configuration;var s=sample;
                    c.installedInstanceId=value;c.installedSceneHandle=value;
                    s.hostId=value;s.routerId=value;s.hazardsId=value;s.playerId=value;s.combatId=value;s.transferId=value;s.collectorId=value;s.sceneHandle=value;
                    DiagnosticTime(clock);var cj=JsonUtility.ToJson(c);DiagnosticTime(clock);var sj=JsonUtility.ToJson(s);DiagnosticTime(clock);
                    var cr=JsonUtility.FromJson<Observer.Configuration>(cj);DiagnosticTime(clock);var sr=JsonUtility.FromJson<Observer.SampleRow>(sj);DiagnosticTime(clock);
                    using(var cd=ParseStrictDocument(cj))using(var sd=ParseStrictDocument(sj))
                    {
                        var strictSample=ReadSampleProjection(sj);DiagnosticTime(clock);
                        for(var f=0;f<10;f++)
                        {
                            DiagnosticTime(clock);var root=f<2?cd.RootElement:sd.RootElement;
                            var literal=Required(root,DiagnosticsFields[f]).GetRawText();
                            var round=f<2?ConfigurationIdentity(cr,f):SampleIdentity(sr,f-2);
                            var strict=f<2?UInt64Integer(root,DiagnosticsFields[f]):SampleIdentity(strictSample,f-2);
                            var passed=literal==DiagnosticNumber(value) && round==value && strict==value;
                            result.positives[v*10+f]=new[]{f.ToString(),v.ToString(),literal,DiagnosticNumber(round),DiagnosticNumber(strict),passed?"1":"2"};
                            DiagnosticRequire(passed,"경계 사본의 직렬화·재로드·strict 숫자가 다릅니다.");DiagnosticTime(clock);
                        }
                    }
                }
                for(var n=0;n<19;n++)for(var f=0;f<10;f++)
                {
                    DiagnosticTime(clock);var code=0;
                    try{using(var doc=ParseStrictDocument(DiagnosticCounterexample(n,DiagnosticsFields[f])))UInt64Integer(doc.RootElement,DiagnosticsFields[f]);}
                    catch(IOException){code=1;}
                    catch(JsonException){code=2;}
                    catch(Exception){result.negatives[n*10+f]=new[]{f,n,3,2};throw;}
                    result.negatives[n*10+f]=new[]{f,n,code,code==0?2:1};
                    DiagnosticRequire(code!=0,"엄격 숫자 반례가 무예외로 수용됐습니다.");DiagnosticTime(clock);
                }
                result.validationPassed=true;
            }
            catch(Exception error){SetDiagnosticFailure(result,error);result.passed=false;}
            return result;
        }
        private static ulong ConfigurationIdentity(Observer.Configuration value,int field)=>field==0?value.installedInstanceId:value.installedSceneHandle;
        private static ulong SampleIdentity(Observer.SampleRow value,int field)
        {
            switch(field){case 0:return value.hostId;case 1:return value.routerId;case 2:return value.hazardsId;case 3:return value.playerId;case 4:return value.combatId;case 5:return value.transferId;case 6:return value.collectorId;case 7:return value.sceneHandle;default:throw new IOException("식별 필드 번호가 다릅니다.");}
        }
        private static string DiagnosticCounterexample(int n,string field)
        {
            var head="{\""+field+"\":";
            switch(n)
            {
                case 0:return "{}";case 1:return head+"null}";case 2:return head+"\"0\"}";case 3:return head+"true}";
                case 4:return head+"[]}";case 5:return head+"{}}";case 6:return head+"-1}";case 7:return head+"-0}";
                case 8:return head+"0.0}";case 9:return head+"1e0}";case 10:return head+"18446744073709551616}";
                case 11:return head+"01}";case 12:return head+"+1}";case 13:return head+"0,\""+field+"\":1}";
                case 14:return head+"0,\"\\u"+((int)field[0]).ToString("x4")+field.Substring(1)+"\":1}";
                case 15:return "[]";case 16:return head;case 17:return head+"0,}";case 18:return head+"/*진단*/0}";
                default:throw new IOException("반례 번호가 다릅니다.");
            }
        }
        private static bool UnissuedStartKeyAllowed(string key,long generation,int lastTick,long fixedCall,long samples)=>string.IsNullOrEmpty(key)&&generation==0&&lastTick==-1&&fixedCall==0&&samples==0;
        private static bool MetadataCostAllowed(double value)=>!double.IsNaN(value)&&!double.IsInfinity(value)&&value>=0;
        private static int InputRouteStageBit(string stage)=>stage=="resume-before"?1:stage=="first-dynamic-after-resume"?2:stage=="first-native-event-after-resume"?4:stage=="failure-or-seal"?8:0;
        private static bool InputRouteOrderAllowed(int mask,int bit,int rows)=>rows>=0&&rows<4&&bit!=0&&(mask&bit)==0&&(bit==1?mask==0:bit==8||(mask&1)!=0&&(mask&8)==0);
        private static void RouteBooleans(JsonElement value,string names)
        {foreach(var name in names.Split(','))Boolean(value,name);}
        private static void ValidateRouteDevice(JsonElement value)
        {RequireExactKeys(value,"available,sameReference,added,enabled,deviceId");RouteBooleans(value,"available,sameReference,added,enabled");var id=Integer(value,"deviceId");if(id<0||Boolean(value,"available")&&id==0)throw new IOException("원래 입력 장치 식별값이 다릅니다.");}
        private int ValidateInputRoute(JsonElement value,int mask,int rows,Observer.SampleRow first,int byteCount)
        {
            RequireExactKeys(value,"schemaVersion,kind,stage,sessionId,caseId,approvalSHA256,runKey,generation,observedStopwatchTicks,stopwatchFrequency,frameCount,updateType,updateCount,route");
            var bit=InputRouteStageBit(Text(value,"stage"));var key=Text(value,"runKey",true);var generation=LongInteger(value,"generation");
            DiagnosticRequire(Integer(value,"schemaVersion")==1&&Text(value,"kind")=="input-route-observation"&&Text(value,"sessionId")==_sessionId&&Text(value,"caseId")==_caseId&&Text(value,"approvalSHA256")==_receivedExecutionSha&&InputRouteOrderAllowed(mask,bit,rows)&&byteCount<=8192&&LongInteger(value,"observedStopwatchTicks")>0&&LongInteger(value,"stopwatchFrequency")==System.Diagnostics.Stopwatch.Frequency&&Integer(value,"frameCount")>=0,"입력 경로 원시 스키마·단계·신원·한도가 다릅니다.");
            DiagnosticRequire(first==null?bit==8&&string.IsNullOrEmpty(key)&&generation==0:key==first.runKey&&generation==first.generation,"입력 경로의 현재 발행 키·세대가 다릅니다.");UInt64Integer(value,"updateCount");
            var update=Text(value,"updateType");DiagnosticRequire(update=="None"||update=="Editor"||update=="Dynamic"||update=="Fixed"||update=="BeforeRender"||update=="Manual","입력 경로 갱신형식이 다릅니다.");
            var route=RequiredObject(value,"route");RequireExactKeys(route,"available,applicationFocused,gameViewFocused,isPlaying,isPaused,unavailableReason,keyboard,mouse,router,events");RouteBooleans(route,"available,applicationFocused,gameViewFocused,isPlaying,isPaused");Text(route,"unavailableReason");
            ValidateRouteDevice(RequiredObject(route,"keyboard"));ValidateRouteDevice(RequiredObject(route,"mouse"));
            var router=RequiredObject(route,"router");RequireExactKeys(router,"available,ownerSame,wrapperSame,mapSame,registrationSame,gameplayMapEnabled,uiMapEnabled,initialized,startCompleted,faulted,disposed,closed,captureSuppressed,mode,unavailableReason");RouteBooleans(router,"available,ownerSame,wrapperSame,mapSame,registrationSame,gameplayMapEnabled,uiMapEnabled,initialized,startCompleted,faulted,disposed,closed,captureSuppressed");Text(router,"mode",true);Text(router,"unavailableReason",true);
            var events=RequiredObject(route,"events");RequireExactKeys(events,"dynamicUpdates,editorUpdates,nativeEvents,nativeDynamic,nativeEditor,nativeOther,stateEvents,deltaEvents,controlBefore,controlAfter,lastNativeStopwatchTicks");
            foreach(var field in new[]{"dynamicUpdates","editorUpdates","nativeEvents","nativeDynamic","nativeEditor","nativeOther","stateEvents","deltaEvents","controlBefore","controlAfter","lastNativeStopwatchTicks"})if(LongInteger(events,field)<0)throw new IOException("입력 경로 계수는 음수일 수 없습니다.");
            var total=LongInteger(events,"nativeEvents");var dynamic=LongInteger(events,"nativeDynamic");var editor=LongInteger(events,"nativeEditor");DiagnosticRequire(dynamic<=total&&editor<=total-dynamic&&LongInteger(events,"nativeOther")==total-dynamic-editor&&LongInteger(events,"controlAfter")<=LongInteger(events,"controlBefore")&&LongInteger(events,"controlBefore")<=dynamic,"입력 경로 필터 전후 계수가 다릅니다.");
            return bit;
        }
        private static bool PausePrefixAllowed(int phase,long samples,long fixedCall,int lastTick,int completedTick,long count,bool requested,int invocations,int setters,bool before,bool after,bool flushed,bool accepted,bool resume)
        {
            if(phase<0||phase>2||count!=samples||accepted||resume||before)return false;
            if(phase==0)return samples==0&&fixedCall==0&&lastTick==-1&&completedTick==-1&&!requested&&invocations==0&&setters==0&&!after&&!flushed;
            return samples==1&&fixedCall>=1&&lastTick==0&&completedTick==0&&requested&&invocations==1&&flushed&&setters==(phase==1?0:1)&&after==(phase==2);
        }
        private void ValidatePausePrefix(JsonElement value,int expectedPhase,long samples,long fixedCall,Observer.SampleRow first)
        {
            RequireExactKeys(value,"schemaVersion,kind,utc,sessionId,caseId,reason,runKey,approvalSHA256,generation,monotonicStamp,fixedCall,displayCall,samples,sampleCount,sampleBytes,frameCount,lastTick,phase,pauseInvocationCount,pauseSetterInvocationCount,firstTick,lastCompletedTick,health,elapsedSeconds,live,pauseRequested,actualIsPausedBefore,actualIsPausedAfter,flushSucceeded,diagnosticsAccepted,resumeRequested,configuration");
            var phase=Integer(value,"phase");
            DiagnosticRequire(phase==expectedPhase&&Integer(value,"firstTick")==0&&Text(value,"approvalSHA256")==_receivedExecutionSha&&
                LongInteger(value,"fixedCall")==fixedCall&&LongInteger(value,"samples")==samples&&
                PausePrefixAllowed(phase,samples,fixedCall,Integer(value,"lastTick"),Integer(value,"lastCompletedTick"),LongInteger(value,"sampleCount"),Boolean(value,"pauseRequested"),Integer(value,"pauseInvocationCount"),Integer(value,"pauseSetterInvocationCount"),Boolean(value,"actualIsPausedBefore"),Boolean(value,"actualIsPausedAfter"),Boolean(value,"flushSucceeded"),Boolean(value,"diagnosticsAccepted"),Boolean(value,"resumeRequested")),"첫 표본 정지 전이·실제 상태·소비 수가 다릅니다.");
            if(phase==0)DiagnosticRequire(Text(value,"runKey",true)==null&&LongInteger(value,"generation")==0&&LongInteger(value,"sampleBytes")==0&&Integer(value,"health")==-1,"첫 표본 대기의 미발행 값이 다릅니다.");
            else DiagnosticRequire(first!=null&&Text(value,"runKey")==first.runKey&&LongInteger(value,"generation")==first.generation&&Integer(value,"health")==first.health&&Integer(value,"health")>0&&LongInteger(value,"sampleBytes")>0,"첫 표본 정지의 실제 발행 값이 다릅니다.");
        }
        private string ReadDiagnosticContinuity(ExecutionApproval approval,CaseApproval selected,string latest,long expectedCount,System.Diagnostics.Stopwatch clock,out string configurationJson,out string sampleJson)
        {
            configurationJson=null;sampleJson=null;
            var pauseUtc=DateTime.UtcNow.ToString("O");var pauseStamp=System.Diagnostics.Stopwatch.GetTimestamp();
            DiagnosticRange startPin=null,firstPin=null,lastPin=null;
            Observer.SampleRow previous=null,first=null;
            long samples=0,duplicates=0,unavailable=0,fixedCall=0,lastStamp=0,lastDisplay=0,lastUtc=0;
            int lastFrame=0,pauseEvents=0,routeMask=0,routeRows=0;double lastElapsed=0;
            using(var events=new DiagnosticPrefixReader(selected.eventsPath,Observer.MaxEventBytes,clock))
            using(var stream=new DiagnosticPrefixReader(selected.samplesPath,Observer.MaxSampleBytes,clock))
            {
                var start=events.Line((int)Observer.MaxEventBytes,out startPin);
                DiagnosticRequire(start!=null,"start 원문이 없습니다.");
                using(var document=ParseStrictDocument(start))
                {
                    var root=document.RootElement;
                    DiagnosticRequire(Integer(root,"schemaVersion")==1 && Text(root,"kind")=="start" && Text(root,"sessionId")==_sessionId && Text(root,"caseId")==DiagnosticsCase &&
                        UnissuedStartKeyAllowed(Text(root,"runKey",true),LongInteger(root,"generation"),Integer(root,"lastTick"),LongInteger(root,"fixedCall"),LongInteger(root,"samples")) &&
                        LongInteger(root,"fixedCall")==0 && LongInteger(root,"samples")==0 && LongInteger(root,"duplicates")==0 && LongInteger(root,"unavailable")==0 && Boolean(root,"live"),"최초 start 신원·미발행 키·카운터가 다릅니다.");
                    configurationJson=RequiredObject(root,"configuration").GetRawText();
                    DiagnosticRequire(configurationJson==_diagnosticsConfigurationJson,"start 재로드 설정의 실제 원문이 최초 전달 설정과 다릅니다.");
                    lastStamp=LongInteger(root,"monotonicStamp");lastDisplay=LongInteger(root,"displayCall");lastFrame=Integer(root,"frameCount");lastElapsed=Real(root,"elapsedSeconds");
                    lastUtc=DiagnosticUtc(Text(root,"utc"));
                }
                var eventJson=events.Line(Observer.MaxRowBytes,out var eventPin);
                var rowJson=stream.Line(Observer.MaxRowBytes,out var rowPin);
                Observer.SampleRow row=rowJson==null?null:ReadSampleProjection(rowJson);
                while(eventJson!=null || row!=null)
                {
                    DiagnosticTime(clock);long eventFixed=long.MaxValue,routeStamp=0;var pauseEvent=false;var routeEvent=false;
                    if(eventJson!=null)using(var doc=ParseStrictDocument(eventJson)){var kind=Text(doc.RootElement,"kind");routeEvent=kind=="input-route-observation";if(routeEvent){routeStamp=LongInteger(doc.RootElement,"observedStopwatchTicks");eventFixed=fixedCall;}else eventFixed=LongInteger(doc.RootElement,"fixedCall");pauseEvent=kind=="first-sample-pause";}
                    if(row!=null && (eventJson==null || (routeEvent?row.monotonicStamp<routeStamp:row.fixedCall<eventFixed || (pauseEvent&&row.fixedCall==eventFixed))))
                    {
                        DiagnosticRequire(samples<Observer.MaxSamples && row.schemaVersion==1 && row.kind=="sample" && row.tick==samples && row.distinctCount==samples+1 &&
                            row.nextExpectedTick==row.tick+1 && row.startCall==1 && row.firstTickZero && row.sameSourcesBeforeAfter && row.streamBytes==rowPin.offset &&
                            row.fixedCall==fixedCall+1 && row.fixedCall==samples+1+duplicates+unavailable && row.duplicateCount==duplicates && row.unavailableCount==unavailable &&
                            row.sessionId==_sessionId && row.caseId==DiagnosticsCase && row.roomId==selected.roomId && row.socketId==selected.socketId &&
                            !string.IsNullOrEmpty(row.runKey) && row.generation>0 && row.frameOrdinal>=0 && row.modeEpoch>=0 && row.monotonicStamp>=lastStamp && row.displayCall>=lastDisplay && row.frameCount>=lastFrame && row.elapsedSeconds>=lastElapsed && DiagnosticUtc(row.utc)>=lastUtc,
                            "실제 sample의 tick·서수·고정 호출·원시 offset·사례 연속성이 다릅니다.");
                        if(first==null){first=row;firstPin=rowPin;sampleJson=rowJson;}
                        else
                        {
                            DiagnosticRequire(row.runKey==first.runKey && row.generation==first.generation && row.frameOrdinal==previous.frameOrdinal+1 &&
                                row.modeEpoch>=previous.modeEpoch && (row.mode==previous.mode?row.modeEpoch==previous.modeEpoch:row.modeEpoch==previous.modeEpoch+1),"실제 sample의 키·세대·입력 서수·모드 순서가 다릅니다.");
                            for(var f=0;f<8;f++)DiagnosticRequire(SampleIdentity(row,f)==SampleIdentity(first,f),"실제 sample 식별값이 사례 안에서 교체됐습니다.");
                        }
                        samples++;fixedCall=row.fixedCall;previous=row;lastPin=rowPin;
                        lastStamp=row.monotonicStamp;lastDisplay=row.displayCall;lastFrame=row.frameCount;lastElapsed=row.elapsedSeconds;lastUtc=DiagnosticUtc(row.utc);
                        if(samples==expectedCount)DiagnosticRequire(rowJson==latest,"마지막 실제 sample과 수집기 원문이 바이트 동일하지 않습니다.");
                        rowJson=stream.Line(Observer.MaxRowBytes,out rowPin);row=rowJson==null?null:ReadSampleProjection(rowJson);
                    }
                    else
                    {
                        using(var document=ParseStrictDocument(eventJson))
                        {
                            var value=document.RootElement;var kind=Text(value,"kind");
                            if(kind=="input-route-observation")
                            {var bit=ValidateInputRoute(value,routeMask,routeRows,first,new UTF8Encoding(false,true).GetByteCount(eventJson)+1);DiagnosticRequire(routeStamp>=lastStamp&&Integer(value,"frameCount")>=lastFrame,"입력 경로 사건 시각·프레임이 역행했습니다.");routeMask|=bit;routeRows++;lastStamp=routeStamp;lastFrame=Integer(value,"frameCount");eventJson=events.Line(Observer.MaxRowBytes,out eventPin);continue;}
                            if(kind=="first-sample-pause")
                            {
                                DiagnosticRequire(Integer(value,"schemaVersion")==1&&Text(value,"sessionId")==_sessionId&&Text(value,"caseId")==DiagnosticsCase&&Boolean(value,"live")&&
                                    LongInteger(value,"monotonicStamp")>=lastStamp&&LongInteger(value,"displayCall")>=lastDisplay&&Integer(value,"frameCount")>=lastFrame&&Real(value,"elapsedSeconds")>=lastElapsed&&DiagnosticUtc(Text(value,"utc"))>=lastUtc&&Required(value,"configuration").ValueKind==JsonValueKind.Null,"첫 표본 정지 사건 신원·시간이 다릅니다.");
                                ValidatePausePrefix(value,pauseEvents,samples,fixedCall,first);pauseEvents++;
                                lastStamp=LongInteger(value,"monotonicStamp");lastDisplay=LongInteger(value,"displayCall");lastFrame=Integer(value,"frameCount");lastElapsed=Real(value,"elapsedSeconds");lastUtc=DiagnosticUtc(Text(value,"utc"));
                                eventJson=events.Line(Observer.MaxRowBytes,out eventPin);continue;
                            }
                            DiagnosticRequire(Integer(value,"schemaVersion")==1 && Text(value,"sessionId")==_sessionId && Text(value,"caseId")==DiagnosticsCase && Boolean(value,"live") &&
                                (kind=="duplicate" || kind=="unavailable") && eventFixed==fixedCall+1 && LongInteger(value,"samples")==samples &&
                                LongInteger(value,"monotonicStamp")>=lastStamp && LongInteger(value,"displayCall")>=lastDisplay && Integer(value,"frameCount")>=lastFrame &&
                                Real(value,"elapsedSeconds")>=lastElapsed && DiagnosticUtc(Text(value,"utc"))>=lastUtc && Required(value,"configuration").ValueKind==JsonValueKind.Null,"실제 사건의 고정 호출·종류·시간 연속성이 다릅니다.");
                            if(kind=="unavailable")
                            {
                                unavailable++;
                                DiagnosticRequire(samples==0 && duplicates==0 && Real(value,"elapsedSeconds")<=60 && Text(value,"runKey",true)==null && LongInteger(value,"generation")==0 &&
                                    Integer(value,"lastTick")==-1 && LongInteger(value,"frameOrdinal")==-1 && LongInteger(value,"modeEpoch")==-1,"첫 표본 전 미발행 사건의 조건이 다릅니다.");
                            }
                            else
                            {
                                duplicates++;
                                DiagnosticRequire(previous!=null && Text(value,"runKey")==first.runKey && LongInteger(value,"generation")==first.generation &&
                                    Integer(value,"lastTick")==previous.tick && LongInteger(value,"frameOrdinal")==previous.frameOrdinal && LongInteger(value,"modeEpoch")==previous.modeEpoch,
                                    "duplicate 사건이 직전 실제 sample의 키·세대·receipt와 다릅니다.");
                            }
                            DiagnosticRequire(LongInteger(value,"duplicates")==duplicates && LongInteger(value,"unavailable")==unavailable && eventFixed==samples+duplicates+unavailable,"사건 누적 카운터가 다릅니다.");
                            fixedCall=eventFixed;lastStamp=LongInteger(value,"monotonicStamp");lastDisplay=LongInteger(value,"displayCall");lastFrame=Integer(value,"frameCount");lastElapsed=Real(value,"elapsedSeconds");lastUtc=DiagnosticUtc(Text(value,"utc"));
                        }
                        eventJson=events.Line(Observer.MaxRowBytes,out eventPin);
                    }
                    DiagnosticTime(clock);
                }
                DiagnosticRequire(pauseEvents==3&&samples==1&&samples==expectedCount && previous.tick==samples-1 && fixedCall==samples+duplicates+unavailable,"자기 prefix의 정지 전이·전체 표본·최종 틱·고정 호출 수가 다릅니다.");
                using(var config=ParseStrictDocument(configurationJson))
                {
                    var now=(System.Diagnostics.Stopwatch.GetTimestamp()-lastStamp)/(double)System.Diagnostics.Stopwatch.Frequency+lastElapsed;
                    DiagnosticRequire(Real(config.RootElement,"caseElapsedSecondsAtInstall")+now+clock.Elapsed.TotalSeconds<1800 &&
                        Real(config.RootElement,"totalElapsedSecondsAtInstall")+now+clock.Elapsed.TotalSeconds<18000,"Pause·진단을 합산한 기존 사례/전체 시간 상한입니다.");
                }
                stream.Finish();events.Finish();DiagnosticTime(clock);
                DiagnosticRange native,input;
                using(var reader=new DiagnosticPrefixReader(selected.nativeEvidencePath,Observer.MaxNativeBytes,clock)){reader.Drain();native=reader.range;}
                using(var reader=new DiagnosticPrefixReader(selected.inputEvidencePath,Observer.MaxEventBytes,clock)){reader.Drain();input=reader.range;}
                DiagnosticTime(clock);DiagnosticLive();
                using(var bytes=new MemoryStream())
                {
                    using(var writer=new Utf8JsonWriter(bytes))
                    {
                        writer.WriteStartObject();writer.WriteString("sessionId",_sessionId);writer.WriteString("caseId",DiagnosticsCase);writer.WriteString("project",ProjectRoot());
                        writer.WriteNumber("unityPid",approval.unityPid);writer.WriteNumber("unityStartTicks",approval.unityStartTicks);writer.WriteString("unityExe",approval.unityExe);writer.WriteString("unityVersion",approval.unityVersion);
                        writer.WriteString("windowId",DiagnosticNumber(approval.uint64DiagnosticsWindowId));writer.WriteString("windowIdentity",approval.windowIdentity);writer.WriteString("inputTarget",approval.inputTarget);
                        WriteDiagnosticPin(writer,"checkpoint",approval.sourceCheckpoint);WriteDiagnosticPin(writer,"windowSource",approval.windowSource);WriteDiagnosticPin(writer,"observerSource",approval.observerSource);
                        using(var document=ParseStrictDocument(_diagnosticsConfigurationJson))WriteDiagnosticPin(writer,"executionApproval",FilePin(document.RootElement,"executionApproval"));
                        WriteDiagnosticPin(writer,"diagnosticScopeAuthority",approval.approvedContract);WriteDiagnosticPin(writer,"nativeInputAuthority",approval.uint64DiagnosticsNativeInputAuthority);
                        WriteDiagnosticRange(writer,"samplesPrefix",stream.range);WriteDiagnosticRange(writer,"eventsPrefix",events.range);WriteDiagnosticRange(writer,"start",startPin);
                        WriteDiagnosticRange(writer,"firstSample",firstPin);WriteDiagnosticRange(writer,"lastSample",lastPin);WriteDiagnosticRange(writer,"nativePrefix",native);WriteDiagnosticRange(writer,"inputPrefix",input);
                        writer.WriteNumber("firstTick",0);writer.WriteNumber("currentTick",previous.tick);writer.WriteNumber("samples",samples);writer.WriteNumber("duplicates",duplicates);writer.WriteNumber("unavailable",unavailable);
                        writer.WriteNumber("fixedCall",fixedCall);writer.WriteNumber("gapCount",0);writer.WriteString("runKey",first.runKey);writer.WriteNumber("generation",first.generation);
                        writer.WriteBoolean("pauseAtEntry",true);writer.WriteBoolean("pauseAtExit",EditorApplication.isPaused);writer.WriteString("pauseEntryUtc",pauseUtc);writer.WriteString("pauseExitUtc",DateTime.UtcNow.ToString("O"));
                        writer.WriteNumber("pauseEntryStamp",pauseStamp);writer.WriteNumber("pauseExitStamp",System.Diagnostics.Stopwatch.GetTimestamp());
                        writer.WriteNull("nativePauseEventRange");writer.WriteNull("inputPauseEventRange");writer.WriteEndObject();
                    }
                    ValidateDiagnosticText(_sessionId,256);ValidateDiagnosticText(first.runKey,256);ValidateDiagnosticText(approval.windowIdentity,256);ValidateDiagnosticText(approval.inputTarget,256);
                    ValidateDiagnosticText(approval.unityExe,1536);ValidateDiagnosticText(ProjectRoot(),1536);
                    DiagnosticTime(clock);var json=new UTF8Encoding(false,true).GetString(bytes.ToArray());
                    if(bytes.Length>12288)throw new IOException("진단 메타데이터 바이트 상한입니다.");return json;
                }
            }
        }
        private static void WriteDiagnosticPin(Utf8JsonWriter writer,string name,Observer.FilePin pin)
        {
            ValidateDiagnosticText(pin.path,1536);ValidateDiagnosticText(pin.sha256,256);
            writer.WritePropertyName(name);writer.WriteStartObject();writer.WriteString("path",pin.path);writer.WriteNumber("length",pin.length);writer.WriteString("sha256",pin.sha256);writer.WriteEndObject();
        }
        private static long DiagnosticUtc(string text)
        {
            if(!DateTime.TryParseExact(text,"O",System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.RoundtripKind,out var value) || value.Kind!=DateTimeKind.Utc)
                throw new IOException("진단 원문 시각이 명시 UTC 형식과 다릅니다.");
            return value.Ticks;
        }
        private static void WriteDiagnosticRange(Utf8JsonWriter writer,string name,DiagnosticRange range)
        {
            ValidateDiagnosticText(range.path,1536);
            writer.WritePropertyName(name);writer.WriteStartObject();writer.WriteString("path",range.path);writer.WriteNumber("offset",range.offset);writer.WriteNumber("length",range.length);
            writer.WriteString("sha256",range.sha256);writer.WriteBoolean("includeLf",range.includeLf);writer.WriteNumber("readLengthBefore",range.readLengthBefore);writer.WriteNumber("readLengthAfter",range.readLengthAfter);
            writer.WriteString("observedUtc",range.observedUtc);writer.WriteEndObject();
        }
        private static void ValidateDiagnosticText(string text,int limit)
        {
            using(var bytes=new MemoryStream())
            {using(var writer=new Utf8JsonWriter(bytes))writer.WriteStringValue(text);if(bytes.Length>limit)throw new IOException("진단 필수 문자열의 UTF-8/escape 상한입니다.");}
        }
        private static byte[] SerializeDiagnosticResult(UInt64DiagnosticResult result)
        {
            if(result.firstFailureType!=null)ValidateDiagnosticText(result.firstFailureType,256);
            if(result.firstFailureMessage!=null)ValidateDiagnosticText(result.firstFailureMessage,512);
            using(var bytes=new MemoryStream())
            {
                using(var writer=new Utf8JsonWriter(bytes))
                {
                    writer.WriteStartObject();writer.WriteNumber("schemaVersion",1);writer.WriteString("kind","uint64Diagnostic");
                    writer.WriteBoolean("validationPassed",result.validationPassed);writer.WriteBoolean("passed",false);writer.WriteBoolean("fileClosed",false);
                    writer.WriteString("closureMeaning","본문은 직렬화 전 판독 사실만 보존하며 실제 폐쇄·최종 성공은 원래 창 메모리와 실행 담당자의 원장 관측으로 확인한다.");
                    writer.WriteBoolean("attempted",true);writer.WriteNumber("invocationLimit",1);writer.WriteNumber("callLimitSeconds",5);writer.WriteNumber("fileLimitBytes",32768);
                    writer.WriteString("startedUtc",result.startedUtc);writer.WriteString("endedUtcBeforeWrite",result.endedUtc);writer.WriteNumber("elapsedSecondsBeforeWrite",result.elapsedSeconds);
                    writer.WriteString("firstFailureType",result.firstFailureType);writer.WriteString("firstFailureMessage",result.firstFailureMessage);
                    foreach(var name in new[]{"runtimeSourceAccepted","independentAcceptance","actualRoomAccepted","gameplayAccepted","nativeInputContinuityAccepted","nativePauseInputCorrespondenceAccepted"})writer.WriteBoolean(name,false);
                    writer.WritePropertyName("metadata");using(var metadata=ParseStrictDocument(result.metadataJson))metadata.RootElement.WriteTo(writer);
                    writer.WritePropertyName("fields");writer.WriteStartArray();foreach(var name in DiagnosticsFields)writer.WriteStringValue(name);writer.WriteEndArray();
                    writer.WritePropertyName("boundaryValues");writer.WriteStartArray();foreach(var value in DiagnosticsValues)writer.WriteStringValue(DiagnosticNumber(value));writer.WriteEndArray();
                    writer.WritePropertyName("counterexampleRules");writer.WriteStartArray();
                    foreach(var rule in new[]{"missing","null","string0","true","array","object","minus1","minus0","decimal0","exponent1","overflow","leadingZero","plus1","duplicate","escapedDuplicateFirstAscii","rootArray","incomplete","trailingComma","comment"})writer.WriteStringValue(rule);
                    writer.WriteEndArray();writer.WritePropertyName("codes");writer.WriteStartObject();writer.WriteNumber("notExecuted",0);writer.WriteNumber("pass",1);writer.WriteNumber("fail",2);writer.WriteNumber("IOException",1);writer.WriteNumber("JsonException",2);writer.WriteNumber("unexpectedException",3);writer.WriteEndObject();
                    writer.WritePropertyName("normal");WriteDiagnosticStrings(writer,result.normal);
                    writer.WritePropertyName("positive");writer.WriteStartArray();foreach(var row in result.positives){writer.WriteStartArray();writer.WriteNumberValue(int.Parse(row[0],System.Globalization.CultureInfo.InvariantCulture));writer.WriteNumberValue(int.Parse(row[1],System.Globalization.CultureInfo.InvariantCulture));for(var i=2;i<5;i++)writer.WriteStringValue(row[i]);writer.WriteNumberValue(int.Parse(row[5],System.Globalization.CultureInfo.InvariantCulture));writer.WriteEndArray();}writer.WriteEndArray();
                    writer.WritePropertyName("negative");writer.WriteStartArray();foreach(var row in result.negatives){writer.WriteStartArray();foreach(var item in row)writer.WriteNumberValue(item);writer.WriteEndArray();}writer.WriteEndArray();
                    writer.WriteEndObject();
                }
                bytes.WriteByte(10);var output=bytes.ToArray();
                // REQ-M1COMP-R1-002: BOM 없음, LF 포함 파일 전체 상한이며 초과를 잘라 성공으로 만들지 않는다.
                var positiveBytes=DiagnosticRowsByteCount(result.positives,null);var negativeBytes=DiagnosticRowsByteCount(null,result.negatives);
                if(output.Length-positiveBytes-negativeBytes>12288+128+1 || output.Length>29217 || output.Length>32768)throw new IOException("진단 JSON의 승인된 메타데이터 최악 예산 또는 파일 전체 상한입니다.");
                return output;
            }
        }
        private static void WriteDiagnosticStrings(Utf8JsonWriter writer,string[][] rows)
        {writer.WriteStartArray();foreach(var row in rows){writer.WriteStartArray();foreach(var value in row)writer.WriteStringValue(value);writer.WriteEndArray();}writer.WriteEndArray();}
        private static int DiagnosticRowsByteCount(string[][] positives,int[][] negatives)
        {
            var total=2;
            var count=positives!=null?positives.Length:negatives.Length;
            for(var n=0;n<count;n++)using(var bytes=new MemoryStream())
            {
                using(var writer=new Utf8JsonWriter(bytes))
                {
                    writer.WriteStartArray();
                    if(positives!=null)
                    {
                        var row=positives[n];writer.WriteNumberValue(int.Parse(row[0],System.Globalization.CultureInfo.InvariantCulture));writer.WriteNumberValue(int.Parse(row[1],System.Globalization.CultureInfo.InvariantCulture));
                        for(var i=2;i<5;i++){if(row[i].Length>20)throw new IOException("경계 숫자의 최대20자리 상한입니다.");writer.WriteStringValue(row[i]);}writer.WriteNumberValue(int.Parse(row[5],System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else foreach(var number in negatives[n])writer.WriteNumberValue(number);
                    writer.WriteEndArray();
                }
                if(bytes.Length+1>(positives!=null?192:48))throw new IOException("진단 최소 결과 행의 바이트 상한입니다.");
                total+=checked((int)bytes.Length)+(n==0?0:1);
            }
            return total;
        }
        private void CleanupOwnedRoot()
        {
            try
            {
                RequireEditorIdle();FindOwnedCollector();
                if(_collector!=null && _collector.gameObject.name==_rootName && _collector.transform.parent==null && _collector.ConfiguredSessionIdForAuthoring==_sessionId)
                    UnityEngine.Object.DestroyImmediate(_collector.gameObject);
                _collector=null;_armed=false;_message="종료된 자기 임시 root만 정리했습니다. 다음 사례는 새 메뉴와 새 승인 구성부터 시작합니다.";
            }
            catch(Exception error){_message="임시 객체 정리 거절: "+error.Message;}
        }

        // REQ-C1B-G3OBS-008/009: 정확6 전체 사전 검사 후 방마다 기존 API를 단1회 직접 호출한다.
        private void ApplyEvidence()
        {
            try
            {
                RequireEditorIdle();
                if(_armed || _applyAttempted)throw new InvalidOperationException("수집 종료·정리 전이거나 이 창의 적용 시도가 이미 있었습니다.");
                var path=EditorUtility.OpenFilePanel("정확 여섯 방 수용 쓰기 소유 승인 선택",DeliveryRoot(),"json");
                if(string.IsNullOrEmpty(path))return;
                var approval=ReadApplyAuthorization(path,DeliveryRoot(),Observer.MaxEventBytes);
                if(approval==null || approval.schemaVersion!=1 || approval.status!="Approved" || !Astra(approval.participant) ||
                    !approval.finalApproval || !approval.roomAcceptanceWriteAuthorizedNow || approval.fromOwner!="Sol" || approval.toOwner!="Root" || approval.applyOwner!="Root" ||
                    approval.rooms==null || approval.rooms.Length!=6 || approval.exactPrefabPaths==null || approval.exactPrefabPaths.Length!=6)
                    throw new InvalidOperationException("정확6의 실제 Sol→Root 수용 쓰기 소유가 없습니다.");
                var checkpoint=ReadCheckpoint(approval.sourceCheckpoint);
                VerifyAllSources(checkpoint.files);VerifyToolPins(approval.observerSource,approval.observerMeta,approval.windowSource,approval.windowMeta);
                Observer.VerifyApprovedContract(approval.approvedContract,ProjectRoot());
                for(var i=0;i<6;i++)
                {
                    var id="R0"+(i+1);var item=approval.rooms[i];var expectedPath="Assets/Prefabs/Chapter1/"+id+".prefab";
                    if(item==null || item.roomId!=id || !Observer.SamePath(approval.exactPrefabPaths[i],AbsoluteAsset(expectedPath)))
                        throw new InvalidOperationException("적용 순서·소유 파일이 정확 R01~R06이 아닙니다.");
                    VerifyRoomAuthorization(item,approval);
                }
                _applyAttempted=true;
                for(var i=0;i<6;i++)
                {
                    RequireEditorIdle();
                    // 앞서 API가 실제 저장한 prefab만 delta로 제외한다. 미적용/meta/다른 원문은 매번 유지한다.
                    for(var j=0;j<checkpoint.files.Length;j++)
                    {
                        var skip=false;for(var prior=0;prior<i;prior++)if(Observer.SamePath(checkpoint.files[j].path,approval.rooms[prior].prefab.path))skip=true;
                        if(!skip)Observer.VerifyPin(checkpoint.files[j],ProjectRoot());
                    }
                    var item=approval.rooms[i];
                    Chapter1ExpeditionAuthoringBuilder.AcceptBasicMovementReviewForAuthoring(item.roomId,item.prefab.sha256,item.lunaRoomReview.path,item.lunaRoomReview.sha256);
                    Observer.VerifyPin(item.prefabMeta,ProjectRoot());
                    Debug.Log("기본 이동 수용 API 기록 완료 "+item.roomId+" · 전 원문 "+item.prefab.sha256+" · 후 원문 "+Observer.Sha(item.prefab.path)+" · 독립 보고 "+item.lunaRoomReview.sha256);
                }
                _message="여섯 방의 기존 수용 입구 호출이 끝났습니다. 실제 delta 원장과 독립 사후 판정은 실행 담당자가 결속합니다.";
            }
            catch(Exception error)
            {
                _message="수용 적용 중단: "+error.Message+" 앞서 실제 기록된 결과를 보존하며 남은 호출·되돌림·재호출은 하지 않습니다.";
                Debug.LogError(_message);
            }
        }
        private static void VerifyRoomAuthorization(RoomAuthorization item,ApplyAuthorization approval)
        {
            var required=ExpectedCases(item.roomId);ExactStrings(item.requiredCaseIds,required);
            VerifyExpectedAsset(item.prefab,"Assets/Prefabs/Chapter1/"+item.roomId+".prefab");VerifyExpectedAsset(item.prefabMeta,"Assets/Prefabs/Chapter1/"+item.roomId+".prefab.meta");
            Observer.VerifyPin(item.prefab,ProjectRoot());Observer.VerifyPin(item.prefabMeta,ProjectRoot());
            Observer.VerifyPin(item.lunaRoomReview,EvidenceRoot());Observer.VerifyPin(item.astraRoomApproval,DeliveryRoot());
            var luna=ReadLunaRoomReview(item.lunaRoomReview.path,EvidenceRoot(),Observer.MaxEventBytes);
            var astra=ReadAstraRoomApproval(item.astraRoomApproval.path,DeliveryRoot(),Observer.MaxEventBytes);
            if(luna==null || luna.schemaVersion!=1 || !Luna(luna.participant) ||
                luna.p0Count!=0 || luna.p1Count!=0 || luna.roomId!=item.roomId || luna.sourceCheckpointSha256!=approval.sourceCheckpoint.sha256 ||
                luna.observerHelperSha256!=approval.observerSource.sha256 || luna.expectedPrefabSha256!=item.prefab.sha256 || luna.expectedMetaSha256!=item.prefabMeta.sha256 ||
                luna.cases==null || luna.cases.Length!=required.Length)
                throw new InvalidOperationException("동일 방·소스·관측기의 실제 Luna 원문 결속이 없습니다.");
            ExactStrings(luna.requiredCaseIds,required);
            if(astra==null || astra.schemaVersion!=1 || !Astra(astra.participant) || astra.status!="Approved" || !astra.finalApproval || astra.applyOwner!="Root" ||
                astra.roomId!=item.roomId || astra.sourceCheckpointSha256!=approval.sourceCheckpoint.sha256 || astra.observerHelperSha256!=approval.observerSource.sha256 ||
                astra.lunaRoomReviewSha256!=item.lunaRoomReview.sha256 || astra.expectedPrefabSha256!=item.prefab.sha256 || astra.expectedMetaSha256!=item.prefabMeta.sha256)
                throw new InvalidOperationException("같은 독립 보고에 대한 Astra 방별 한정 승인이 없습니다.");
            ExactStrings(astra.requiredCaseIds,required);
            for(var i=0;i<required.Length;i++)
            {
                var c=luna.cases[i];
                if(c==null || c.caseId!=required[i] || c.socketId!="S"+item.roomId.Substring(1)+c.caseId.Substring(3) ||
                    string.IsNullOrEmpty(c.runKey) || c.generation<=0 || c.firstTick!=0 || c.finalTick<0 || c.gapCount!=0 || c.continuousSampleCount!=(long)c.finalTick+1 ||
                    !c.endpointRead || !c.pathRead || !c.supportRead || !c.survivalRead || !c.prohibitedInputAbsent || !c.sourceIdentityRead || !c.closureRead || string.IsNullOrEmpty(c.completionKey))
                    throw new InvalidOperationException("필수 사례의 실제 경로·지지·입력·생존·연속성 판독이 없습니다.");
                Observer.VerifyPin(c.rawManifest,EvidenceRoot());Observer.VerifyPin(c.nativeEvidence,EvidenceRoot());Observer.VerifyPin(c.inputEvidence,EvidenceRoot());
                if(c.nativeEvidence.length>Observer.MaxNativeBytes || c.inputEvidence.length>Observer.MaxEventBytes)throw new IOException("실제 native 원시 상한입니다.");
                var seal=ReadSealRow(c.rawManifest.path,EvidenceRoot(),Observer.MaxEventBytes);
                if(seal==null || seal.schemaVersion!=1 || seal.kind!="seal" || seal.live || !seal.sourcePinsUnchanged || !seal.continuousFromZero ||
                    seal.caseId!=c.caseId || seal.runKey!=c.runKey || seal.generation!=c.generation || seal.firstTick!=0 || seal.lastTick!=c.finalTick ||
                    seal.samples!=c.continuousSampleCount || seal.gapCount!=0 || !string.IsNullOrEmpty(seal.firstFailureJson) || !string.IsNullOrEmpty(seal.sealError) || seal.completionKey!=c.completionKey || seal.exitCandidateTick<0 ||
                    seal.sourceCheckpointSha256!=approval.sourceCheckpoint.sha256 || seal.observerHelperSha256!=approval.observerSource.sha256 ||
                    seal.prefabSha256!=item.prefab.sha256 || seal.prefabMetaSha256!=item.prefabMeta.sha256)
                    throw new IOException("완주키·첫tick·연속 원문·원시 봉인이 다릅니다.");
                Observer.VerifyPin(seal.samplesFile,EvidenceRoot());Observer.VerifyPin(seal.eventsFile,EvidenceRoot());
                RequireReceivedSeal(seal,c.inputEvidence.path);
                if(seal.inputFile.length!=c.inputEvidence.length || seal.inputFile.sha256!=c.inputEvidence.sha256)throw new IOException("독립 입력 원문과 봉인 핀이 다릅니다.");
                if(seal.samplesFile.length>Observer.MaxSampleBytes || seal.eventsFile.length>Observer.MaxEventBytes)throw new IOException("표본·사건 원시 상한입니다.");
            }
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/"+item.roomId+".prefab");
            if(prefab==null || prefab.GetComponent<Chapter1RoomInstance>()==null || prefab.GetComponent<Chapter1RoomInstance>().IsBasicMovementAccepted)
                throw new InvalidOperationException("같은 미수용 방 후보가 아닙니다.");
        }
        private static void RequireReceivedSeal(Observer.SealRow seal,string path)
        {
            if(!seal.inputFileClosed || !seal.inputObservationComplete || seal.inputIncomplete || seal.inputErrors!=0 ||
                seal.inputObservationScope!="Play&&!Pause" || seal.inputPauseCount<0 ||
                seal.inputSubscriptions!=2 || seal.inputUnsubscriptions!=2 || seal.inputRows<2 || seal.receivedCount<0 ||
                seal.inputBytes<=0 || seal.inputBytes>Observer.MaxEventBytes || seal.inputFile==null ||
                !Observer.SamePath(seal.inputFile.path,path) || seal.inputFile.length!=seal.inputBytes)
                throw new IOException("입력 등록·불완전·원시 폐쇄·봉인 핀이 다릅니다.");
            Observer.VerifyPin(seal.inputFile,EvidenceRoot());
        }
        private static void RequireEditorIdle()
        {if(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)throw new InvalidOperationException("Play·컴파일·가져오기가 꺼진 편집 상태에서만 가능합니다.");}
        private static bool Astra(Participant participant)=>participant!=null && participant.model=="gpt-6-astra" && participant.reasoningEffort=="도구 기본값, override 없음" && !string.IsNullOrWhiteSpace(participant.role);
        private static bool Luna(Participant participant)=>participant!=null && participant.model=="gpt-6-luna" && participant.reasoningEffort=="도구 기본값, override 없음" && !string.IsNullOrWhiteSpace(participant.role);
        private static string ProjectRoot()=>Path.GetFullPath(Path.Combine(Application.dataPath,".."));
        private static string DeliveryRoot()=>Path.GetFullPath(Path.Combine(ProjectRoot(),".."));
        private static string EvidenceRoot()=>Path.Combine(DeliveryRoot(),"evidence");
        private static string AbsoluteAsset(string path)=>Path.GetFullPath(Path.Combine(ProjectRoot(),path));
        private static Observer.FilePin Pin(string path)=>new Observer.FilePin{path=Path.GetFullPath(path),length=new FileInfo(path).Length,sha256=Observer.Sha(path)};
        // REQ-C1B-G3OBS-005/008, P1-001: 누락과 명시0/false/null을 구별한다.
        // 현재 여섯 문서만 명시 투영하며 추가 메타데이터를 닫힌 스키마로 금지하지 않는다.
        private static JsonDocument ReadStrictDocument(string path,string root,long limit)
        {
            Observer.RequireInside(path,root);
            if(limit<0 || limit>Observer.MaxEventBytes)throw new IOException("JSON 판독 상한이 다릅니다.");
            byte[] bytes;
            using(var stream=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read))
            {
                if(stream.Length>limit)throw new IOException("원문 읽기 상한입니다.");
                bytes=new byte[checked((int)stream.Length)];var offset=0;
                while(offset<bytes.Length)
                {var count=stream.Read(bytes,offset,bytes.Length-offset);if(count==0)throw new IOException("JSON 원문이 중간에 끝났습니다.");offset+=count;}
                if(stream.ReadByte()!=-1)throw new IOException("판독 도중 JSON 길이가 바뀌었습니다.");
            }
            return ParseStrictBytes(bytes,limit);
        }
        private static JsonDocument ParseStrictBytes(byte[] bytes,long limit)
        {
            if(bytes==null || limit<0 || bytes.LongLength>limit)throw new IOException("JSON 바이트 판독 상한입니다.");
            // BOM은 인코딩 표지만 제거한다. 호출자의 지문은 원래 바이트 그대로다.
            var start=bytes.Length>=3 && bytes[0]==0xEF && bytes[1]==0xBB && bytes[2]==0xBF?3:0;
            var json=new UTF8Encoding(false,true).GetString(bytes,start,bytes.Length-start);
            return ParseStrictDocument(json);
        }
        private static JsonDocument ParseStrictDocument(string json)
        {
            var document=JsonDocument.Parse(json,new JsonDocumentOptions
            {AllowTrailingCommas=false,CommentHandling=JsonCommentHandling.Disallow,MaxDepth=64});
            try
            {
                if(document.RootElement.ValueKind!=JsonValueKind.Object)throw new IOException("JSON 루트는 명시 객체여야 합니다.");
                ValidateEscapedSurrogatePairs(json);
                RejectDuplicateNamesAndInvalidStrings(document.RootElement);return document;
            }
            catch{document.Dispose();throw;}
        }
        private static void RejectDuplicateNamesAndInvalidStrings(JsonElement value)
        {
            if(value.ValueKind==JsonValueKind.Object)
            {
                var names=new HashSet<string>(StringComparer.Ordinal);
                foreach(var property in value.EnumerateObject())
                {
                    ValidateUnicode(property.Name);
                    if(!names.Add(property.Name))throw new IOException("중복 JSON 키입니다: "+property.Name);
                    RejectDuplicateNamesAndInvalidStrings(property.Value);
                }
            }
            else if(value.ValueKind==JsonValueKind.Array)
                foreach(var item in value.EnumerateArray())RejectDuplicateNamesAndInvalidStrings(item);
            else if(value.ValueKind==JsonValueKind.String)ValidateUnicode(value.GetString());
        }
        // JSON 문법은 JsonDocument가 판정한다. 원래 문자열 토큰의 유니코드 escape만
        // 추가 확인해 잘못된 surrogate가 디코더의 대체 문자로 수선되는 것도 거절한다.
        private static void ValidateEscapedSurrogatePairs(string json)
        {
            for(var i=0;i<json.Length;i++)
            {
                if(json[i]!='"')continue;
                for(i++;i<json.Length && json[i]!='"';i++)
                {
                    if(json[i]!='\\')continue;
                    i++;if(json[i]!='u')continue;
                    var code=EscapedCodeUnit(json,i+1);i+=4;
                    if(code>=0xD800 && code<=0xDBFF)
                    {
                        if(i+6>=json.Length || json[i+1]!='\\' || json[i+2]!='u')throw new IOException("짝이 없는 유니코드 escape입니다.");
                        var low=EscapedCodeUnit(json,i+3);
                        if(low<0xDC00 || low>0xDFFF)throw new IOException("짝이 없는 유니코드 escape입니다.");
                        i+=6;
                    }
                    else if(code>=0xDC00 && code<=0xDFFF)throw new IOException("짝이 없는 유니코드 escape입니다.");
                }
            }
        }
        private static int EscapedCodeUnit(string json,int start)
        {
            var result=0;
            for(var i=start;i<start+4;i++)
            {
                var c=json[i];var digit=c>='0' && c<='9'?c-'0':c>='A' && c<='F'?c-'A'+10:c>='a' && c<='f'?c-'a'+10:-1;
                if(digit<0)throw new IOException("잘못된 유니코드 escape입니다.");result=result*16+digit;
            }
            return result;
        }
        private static void ValidateUnicode(string text)
        {
            for(var i=0;i<text.Length;i++)
            {
                if(char.IsHighSurrogate(text[i]))
                {if(i+1==text.Length || !char.IsLowSurrogate(text[i+1]))throw new IOException("잘못된 유니코드 문자열입니다.");i++;}
                else if(char.IsLowSurrogate(text[i]))throw new IOException("잘못된 유니코드 문자열입니다.");
            }
        }
        private static JsonElement Required(JsonElement value,string name)
        {
            if(value.ValueKind!=JsonValueKind.Object || !value.TryGetProperty(name,out var result))
                throw new IOException("필수 JSON 키가 없습니다: "+name);
            return result;
        }
        private static JsonElement RequiredObject(JsonElement value,string name)
        {var result=Required(value,name);if(result.ValueKind!=JsonValueKind.Object)throw new IOException("명시 객체가 필요합니다: "+name);return result;}
        private static JsonElement RequiredArray(JsonElement value,string name,int maximum)
        {var result=Required(value,name);if(result.ValueKind!=JsonValueKind.Array || result.GetArrayLength()>maximum)throw new IOException("명시 배열·수량 상한이 다릅니다: "+name);return result;}
        private static string Text(JsonElement value,string name,bool nullable=false)
        {
            var result=Required(value,name);
            if(nullable && result.ValueKind==JsonValueKind.Null)return null;
            if(result.ValueKind!=JsonValueKind.String)throw new IOException("명시 문자열이 필요합니다: "+name);
            return result.GetString();
        }
        private static bool Boolean(JsonElement value,string name)
        {
            var result=Required(value,name);
            if(result.ValueKind==JsonValueKind.True)return true;if(result.ValueKind==JsonValueKind.False)return false;
            throw new IOException("명시 불리언이 필요합니다: "+name);
        }
        private static bool IntegerLiteral(JsonElement value)
        {
            if(value.ValueKind!=JsonValueKind.Number)return false;
            var raw=value.GetRawText();var start=raw.Length>0 && raw[0]=='-'?1:0;
            if(start==raw.Length)return false;
            for(var i=start;i<raw.Length;i++)if(raw[i]<'0' || raw[i]>'9')return false;
            return true;
        }
        private static int Integer(JsonElement value,string name)
        {var result=Required(value,name);if(!IntegerLiteral(result) || !result.TryGetInt32(out var number))throw new IOException("명시32비트 정수 리터럴이 필요합니다: "+name);return number;}
        private static long LongInteger(JsonElement value,string name)
        {var result=Required(value,name);if(!IntegerLiteral(result) || !result.TryGetInt64(out var number))throw new IOException("명시64비트 정수 리터럴이 필요합니다: "+name);return number;}
        // REQ/AC-C1B-G3OBS-010, REQ/AC-M1COMP-R1-002: 식별 원문8만 비음수64비트 정수로 읽는다.
        private static ulong UInt64Integer(JsonElement value,string name)
        {
            var result=Required(value,name);
            if(result.ValueKind!=JsonValueKind.Number)throw new IOException("명시 비음수64비트 정수 리터럴이 필요합니다: "+name);
            var raw=result.GetRawText();
            if(raw.Length==0 || (raw.Length>1 && raw[0]=='0'))throw new IOException("명시 비음수64비트 정수 리터럴이 필요합니다: "+name);
            for(var i=0;i<raw.Length;i++)if(raw[i]<'0' || raw[i]>'9')throw new IOException("명시 비음수64비트 정수 리터럴이 필요합니다: "+name);
            if(!result.TryGetUInt64(out var number))throw new IOException("명시 비음수64비트 정수 리터럴이 필요합니다: "+name);
            return number;
        }
        private static double Real(JsonElement value,string name)
        {
            var result=Required(value,name);
            if(result.ValueKind!=JsonValueKind.Number || !result.TryGetDouble(out var number) || double.IsNaN(number) || double.IsInfinity(number))
                throw new IOException("명시 유한 숫자가 필요합니다: "+name);
            return number;
        }
        private static float FloatReal(JsonElement value,string name)
        {var result=Real(value,name);if(result < -float.MaxValue || result>float.MaxValue)throw new IOException("32비트 실수 범위 밖입니다: "+name);return (float)result;}
        private static Participant ProjectParticipant(JsonElement value)=>new Participant
        {model=Text(value,"model"),reasoningEffort=Text(value,"reasoningEffort"),role=Text(value,"role")};
        private static Observer.FilePin ProjectFilePin(JsonElement value)
        {
            if(value.ValueKind!=JsonValueKind.Object)throw new IOException("FilePin은 명시 객체여야 합니다.");
            return new Observer.FilePin{path=Text(value,"path"),length=LongInteger(value,"length"),sha256=Text(value,"sha256")};
        }
        private static Observer.FilePin FilePin(JsonElement value,string name,bool nullable=false)
        {var result=Required(value,name);if(nullable && result.ValueKind==JsonValueKind.Null)return null;return ProjectFilePin(result);}
        private static Observer.FilePin[] FilePins(JsonElement value,string name,int maximum)
        {
            var array=RequiredArray(value,name,maximum);var result=new Observer.FilePin[array.GetArrayLength()];var index=0;
            foreach(var item in array.EnumerateArray())result[index++]=ProjectFilePin(item);return result;
        }
        private static string[] Strings(JsonElement value,string name,int maximum)
        {
            var array=RequiredArray(value,name,maximum);var result=new string[array.GetArrayLength()];var index=0;
            foreach(var item in array.EnumerateArray())
            {if(item.ValueKind!=JsonValueKind.String)throw new IOException("문자열 배열 원소가 다릅니다: "+name);result[index++]=item.GetString();}
            return result;
        }

        private static ExecutionApproval ReadExecutionApproval(string path,string root,long limit)
        {
            using(var document=ReadStrictDocument(path,root,limit))
                return ProjectExecutionApproval(document.RootElement);
        }
        private static void RequireExactKeys(JsonElement value,string names)
        {
            if(value.ValueKind!=JsonValueKind.Object)throw new IOException("정확 키 집합의 객체가 아닙니다.");
            var expected=new HashSet<string>(names.Split(','),StringComparer.Ordinal);var seen=new HashSet<string>(StringComparer.Ordinal);
            foreach(var property in value.EnumerateObject())if(!expected.Contains(property.Name)||!seen.Add(property.Name))throw new IOException("허용되지 않거나 중복된 키입니다: "+property.Name);
            if(seen.Count!=expected.Count)throw new IOException("필수 키가 누락됐습니다.");
        }
        private static ExecutionApproval ProjectExecutionApproval(JsonElement value)
        {
                RequireExactKeys(value,"schemaVersion,status,owner,sessionId,performer,performerModel,performerRole,unityExe,unityVersion,windowIdentity,inputTarget,finalApproval,guiExecutionAuthorizedNow,inputNeutralObserved,participant,inputEvidenceKind,inputGameViewWindowId,unityPid,unityStartTicks,caseBudgetSeconds,totalBudgetSeconds,caseElapsedSecondsAtInstall,totalElapsedSecondsAtInstall,sourceCheckpoint,approvedContract,toolObservation,observerSource,observerMeta,windowSource,windowMeta,cases,uint64DiagnosticsAuthorizedNow,uint64DiagnosticsNativeInputAuthorizedNow,uint64DiagnosticsCaseId,uint64DiagnosticsPath,uint64DiagnosticsCallLimitSeconds,uint64DiagnosticsFileLimitBytes,uint64DiagnosticsInvocationLimit,uint64DiagnosticsWindowId,uint64DiagnosticsNativeInputAuthority,firstSamplePauseAuthorizedNow,firstSamplePauseCaseId,firstSamplePauseInvocationLimit,resumeAfterUInt64AcceptedAuthorizedNow");
                var pauseAuthorized=Boolean(value,"firstSamplePauseAuthorizedNow");var pauseCase=Text(value,"firstSamplePauseCaseId");var pauseLimit=Integer(value,"firstSamplePauseInvocationLimit");var resumeAuthorized=Boolean(value,"resumeAfterUInt64AcceptedAuthorizedNow");
                if(!PauseApprovalAllowed(pauseAuthorized,pauseCase,pauseLimit,resumeAuthorized))throw new IOException("첫 표본 정지·명시 재개 승인 필드가 다릅니다.");
                var array=RequiredArray(value,"cases",10);var cases=new CaseApproval[array.GetArrayLength()];var index=0;
                foreach(var item in array.EnumerateArray())cases[index++]=ProjectCaseApproval(item);
                return new ExecutionApproval
                {
                    schemaVersion=Integer(value,"schemaVersion"),status=Text(value,"status"),owner=Text(value,"owner"),sessionId=Text(value,"sessionId"),
                    performer=Text(value,"performer"),performerModel=Text(value,"performerModel"),performerRole=Text(value,"performerRole"),
                    inputEvidenceKind=Text(value,"inputEvidenceKind"),inputGameViewWindowId=UInt64Integer(value,"inputGameViewWindowId"),
                    unityExe=Text(value,"unityExe"),unityVersion=Text(value,"unityVersion"),windowIdentity=Text(value,"windowIdentity"),inputTarget=Text(value,"inputTarget"),
                    finalApproval=Boolean(value,"finalApproval"),guiExecutionAuthorizedNow=Boolean(value,"guiExecutionAuthorizedNow"),inputNeutralObserved=Boolean(value,"inputNeutralObserved"),
                    participant=ProjectParticipant(RequiredObject(value,"participant")),unityPid=Integer(value,"unityPid"),unityStartTicks=LongInteger(value,"unityStartTicks"),
                    caseBudgetSeconds=Real(value,"caseBudgetSeconds"),totalBudgetSeconds=Real(value,"totalBudgetSeconds"),
                    caseElapsedSecondsAtInstall=Real(value,"caseElapsedSecondsAtInstall"),totalElapsedSecondsAtInstall=Real(value,"totalElapsedSecondsAtInstall"),
                    sourceCheckpoint=FilePin(value,"sourceCheckpoint"),approvedContract=FilePin(value,"approvedContract"),toolObservation=FilePin(value,"toolObservation"),
                    observerSource=FilePin(value,"observerSource"),observerMeta=FilePin(value,"observerMeta"),windowSource=FilePin(value,"windowSource"),windowMeta=FilePin(value,"windowMeta"),cases=cases
                    ,uint64DiagnosticsAuthorizedNow=Boolean(value,"uint64DiagnosticsAuthorizedNow"),uint64DiagnosticsNativeInputAuthorizedNow=Boolean(value,"uint64DiagnosticsNativeInputAuthorizedNow"),
                    uint64DiagnosticsCaseId=Text(value,"uint64DiagnosticsCaseId"),uint64DiagnosticsPath=Text(value,"uint64DiagnosticsPath"),
                    uint64DiagnosticsCallLimitSeconds=Integer(value,"uint64DiagnosticsCallLimitSeconds"),uint64DiagnosticsFileLimitBytes=Integer(value,"uint64DiagnosticsFileLimitBytes"),
                    uint64DiagnosticsInvocationLimit=Integer(value,"uint64DiagnosticsInvocationLimit"),uint64DiagnosticsWindowId=UInt64Integer(value,"uint64DiagnosticsWindowId"),
                    uint64DiagnosticsNativeInputAuthority=FilePin(value,"uint64DiagnosticsNativeInputAuthority"),
                    firstSamplePauseAuthorizedNow=pauseAuthorized,firstSamplePauseCaseId=pauseCase,firstSamplePauseInvocationLimit=pauseLimit,resumeAfterUInt64AcceptedAuthorizedNow=resumeAuthorized
                };
        }
        private static CaseApproval ProjectCaseApproval(JsonElement value)=>new CaseApproval
        {
            caseId=Text(value,"caseId"),roomId=Text(value,"roomId"),socketId=Text(value,"socketId"),reviewMenu=Text(value,"reviewMenu"),sceneName=Text(value,"sceneName"),
            samplesPath=Text(value,"samplesPath"),eventsPath=Text(value,"eventsPath"),sealPath=Text(value,"sealPath"),
            nativeEvidencePath=Text(value,"nativeEvidencePath"),inputEvidencePath=Text(value,"inputEvidencePath"),prefab=FilePin(value,"prefab"),prefabMeta=FilePin(value,"prefabMeta"),
            uint64DiagnosticsAuthorizedNow=Boolean(value,"uint64DiagnosticsAuthorizedNow"),uint64DiagnosticsCaseId=Text(value,"uint64DiagnosticsCaseId"),
            uint64DiagnosticsPath=Text(value,"uint64DiagnosticsPath"),uint64DiagnosticsCallLimitSeconds=Integer(value,"uint64DiagnosticsCallLimitSeconds"),
            uint64DiagnosticsFileLimitBytes=Integer(value,"uint64DiagnosticsFileLimitBytes"),uint64DiagnosticsInvocationLimit=Integer(value,"uint64DiagnosticsInvocationLimit")
        };
        private static ApplyAuthorization ReadApplyAuthorization(string path,string root,long limit)
        {
            using(var document=ReadStrictDocument(path,root,limit))
            {
                var value=document.RootElement;var array=RequiredArray(value,"rooms",6);var rooms=new RoomAuthorization[array.GetArrayLength()];var index=0;
                foreach(var item in array.EnumerateArray())rooms[index++]=ProjectRoomAuthorization(item);
                return new ApplyAuthorization
                {
                    schemaVersion=Integer(value,"schemaVersion"),status=Text(value,"status"),fromOwner=Text(value,"fromOwner"),toOwner=Text(value,"toOwner"),applyOwner=Text(value,"applyOwner"),
                    participant=ProjectParticipant(RequiredObject(value,"participant")),finalApproval=Boolean(value,"finalApproval"),roomAcceptanceWriteAuthorizedNow=Boolean(value,"roomAcceptanceWriteAuthorizedNow"),
                    sourceCheckpoint=FilePin(value,"sourceCheckpoint"),observerSource=FilePin(value,"observerSource"),observerMeta=FilePin(value,"observerMeta"),
                    windowSource=FilePin(value,"windowSource"),windowMeta=FilePin(value,"windowMeta"),approvedContract=FilePin(value,"approvedContract"),
                    exactPrefabPaths=Strings(value,"exactPrefabPaths",6),rooms=rooms
                };
            }
        }
        private static RoomAuthorization ProjectRoomAuthorization(JsonElement value)=>new RoomAuthorization
        {
            roomId=Text(value,"roomId"),prefab=FilePin(value,"prefab"),prefabMeta=FilePin(value,"prefabMeta"),
            lunaRoomReview=FilePin(value,"lunaRoomReview"),astraRoomApproval=FilePin(value,"astraRoomApproval"),requiredCaseIds=Strings(value,"requiredCaseIds",2)
        };
        private static LunaRoomReview ReadLunaRoomReview(string path,string root,long limit)
        {
            using(var document=ReadStrictDocument(path,root,limit))
            {
                var value=document.RootElement;var array=RequiredArray(value,"cases",2);var cases=new ReviewedCase[array.GetArrayLength()];var index=0;
                foreach(var item in array.EnumerateArray())cases[index++]=ProjectReviewedCase(item);
                return new LunaRoomReview
                {
                    schemaVersion=Integer(value,"schemaVersion"),p0Count=Integer(value,"p0Count"),p1Count=Integer(value,"p1Count"),participant=ProjectParticipant(RequiredObject(value,"participant")),
                    sourceCheckpointSha256=Text(value,"sourceCheckpointSha256"),observerHelperSha256=Text(value,"observerHelperSha256"),roomId=Text(value,"roomId"),
                    expectedPrefabSha256=Text(value,"expectedPrefabSha256"),expectedMetaSha256=Text(value,"expectedMetaSha256"),requiredCaseIds=Strings(value,"requiredCaseIds",2),cases=cases
                };
            }
        }
        private static ReviewedCase ProjectReviewedCase(JsonElement value)=>new ReviewedCase
        {
            caseId=Text(value,"caseId"),socketId=Text(value,"socketId"),runKey=Text(value,"runKey"),completionKey=Text(value,"completionKey"),
            generation=LongInteger(value,"generation"),continuousSampleCount=LongInteger(value,"continuousSampleCount"),
            firstTick=Integer(value,"firstTick"),finalTick=Integer(value,"finalTick"),gapCount=Integer(value,"gapCount"),
            endpointRead=Boolean(value,"endpointRead"),pathRead=Boolean(value,"pathRead"),supportRead=Boolean(value,"supportRead"),survivalRead=Boolean(value,"survivalRead"),
            prohibitedInputAbsent=Boolean(value,"prohibitedInputAbsent"),sourceIdentityRead=Boolean(value,"sourceIdentityRead"),closureRead=Boolean(value,"closureRead"),
            rawManifest=FilePin(value,"rawManifest"),nativeEvidence=FilePin(value,"nativeEvidence"),inputEvidence=FilePin(value,"inputEvidence")
        };
        private static AstraRoomApproval ReadAstraRoomApproval(string path,string root,long limit)
        {
            using(var document=ReadStrictDocument(path,root,limit))
            {
                var value=document.RootElement;
                return new AstraRoomApproval
                {
                    schemaVersion=Integer(value,"schemaVersion"),status=Text(value,"status"),participant=ProjectParticipant(RequiredObject(value,"participant")),finalApproval=Boolean(value,"finalApproval"),
                    sourceCheckpointSha256=Text(value,"sourceCheckpointSha256"),observerHelperSha256=Text(value,"observerHelperSha256"),lunaRoomReviewSha256=Text(value,"lunaRoomReviewSha256"),
                    roomId=Text(value,"roomId"),expectedPrefabSha256=Text(value,"expectedPrefabSha256"),expectedMetaSha256=Text(value,"expectedMetaSha256"),applyOwner=Text(value,"applyOwner"),
                    requiredCaseIds=Strings(value,"requiredCaseIds",2)
                };
            }
        }
        private static SourceCheckpoint ReadSourceCheckpoint(string path,string root,long limit)
        {
            using(var document=ReadStrictDocument(path,root,limit))
            {var value=document.RootElement;return new SourceCheckpoint{fileCount=Integer(value,"fileCount"),files=FilePins(value,"files",590)};}
        }
        private static Observer.SealRow ReadSealRow(string path,string root,long limit)
        {
            using(var document=ReadStrictDocument(path,root,limit))
            {
                var value=document.RootElement;
                var result=new Observer.SealRow
                {
                    schemaVersion=Integer(value,"schemaVersion"),kind=Text(value,"kind"),utc=Text(value,"utc"),sessionId=Text(value,"sessionId"),caseId=Text(value,"caseId"),reason=Text(value,"reason"),
                    runKey=Text(value,"runKey",true),completionKey=Text(value,"completionKey",true),firstFailureJson=Text(value,"firstFailureJson",true),lastSampleJson=Text(value,"lastSampleJson",true),
                    ioLimitation=Text(value,"ioLimitation"),sourceCheckpointSha256=Text(value,"sourceCheckpointSha256",true),observerHelperSha256=Text(value,"observerHelperSha256",true),
                    prefabSha256=Text(value,"prefabSha256",true),prefabMetaSha256=Text(value,"prefabMetaSha256",true),sealError=Text(value,"sealError",true),
                    generation=LongInteger(value,"generation"),samples=LongInteger(value,"samples"),duplicates=LongInteger(value,"duplicates"),unavailable=LongInteger(value,"unavailable"),
                    fixedCalls=LongInteger(value,"fixedCalls"),displayCalls=LongInteger(value,"displayCalls"),cleanupCalls=LongInteger(value,"cleanupCalls"),
                    firstTick=Integer(value,"firstTick"),lastTick=Integer(value,"lastTick"),exitCandidateTick=Integer(value,"exitCandidateTick"),gapCount=Integer(value,"gapCount"),
                    live=Boolean(value,"live"),sourcePinsUnchanged=Boolean(value,"sourcePinsUnchanged"),continuousFromZero=Boolean(value,"continuousFromZero"),
                    independentAcceptance=Boolean(value,"independentAcceptance"),movementAccepted=Boolean(value,"movementAccepted"),elapsedSeconds=Real(value,"elapsedSeconds"),
                    samplesFile=FilePin(value,"samplesFile",true),eventsFile=FilePin(value,"eventsFile",true),inputFile=FilePin(value,"inputFile",true),
                    inputFileClosed=Boolean(value,"inputFileClosed"),inputObservationComplete=Boolean(value,"inputObservationComplete"),inputIncomplete=Boolean(value,"inputIncomplete"),
                    inputRows=LongInteger(value,"inputRows"),inputBytes=LongInteger(value,"inputBytes"),receivedCount=LongInteger(value,"receivedCount"),
                    inputErrors=LongInteger(value,"inputErrors"),inputSubscriptions=LongInteger(value,"inputSubscriptions"),inputUnsubscriptions=LongInteger(value,"inputUnsubscriptions"),
                    inputPauseCount=LongInteger(value,"inputPauseCount"),inputObservationScope=Text(value,"inputObservationScope")
                };
                if(result.schemaVersion!=1 || result.kind!="seal")throw new IOException("명시된 봉인 schemaVersion·kind가 다릅니다.");
                return result;
            }
        }
        // 마지막 보존 행도 관대한 기본값 표본으로 표시하지 않는다. 게임 조회·과거 행 보충은 없다.
        private static Observer.SampleRow ReadSampleProjection(string json)
        {
            if(new UTF8Encoding(false,true).GetByteCount(json)>Observer.MaxRowBytes)throw new IOException("보존 표본 행 상한입니다.");
            using(var document=ParseStrictDocument(json))
            {
                var value=document.RootElement;
                RequireExactKeys(value,"schemaVersion,kind,utc,sessionId,caseId,roomId,socketId,runKey,mode,wallId,wallSide,actionState,hostHierarchy,graphHierarchy,transferHighlightedTarget,transferActiveTarget,transferPlayerState,transferLastFailure,transferLifecycleReason,transferLifecycleClear,transferLifecyclePreviousTarget,monotonicStamp,generation,frameOrdinal,modeEpoch,fixedCall,displayCall,distinctCount,duplicateCount,unavailableCount,streamBytes,frameCount,startCall,hostId,routerId,hazardsId,playerId,combatId,transferId,collectorId,sceneHandle,tick,nextExpectedTick,health,invulnerabilityEndTick,positionXQ4096,positionYQ4096,velocityXQ4096,velocityYQ4096,transferRevision,transferTransitionLockEndTick,transferRemovalCount,transferLifecycleTick,transferLifecycleRevision,isDead,isGrounded,airDashAvailable,hadAim,hadPress,sameSourcesBeforeAfter,firstTickZero,exitCandidate,independentAcceptance,bodyCenterX,bodyCenterY,bodyWidth,bodyHeight,bodyFootY,elapsedSeconds,readMilliseconds,previousAppendFlushMilliseconds,sourceMetadataMilliseconds,healthPublication,killPlanePublication,lethalCrushPublication");
                var result=new Observer.SampleRow
                {
                    schemaVersion=Integer(value,"schemaVersion"),kind=Text(value,"kind"),utc=Text(value,"utc"),sessionId=Text(value,"sessionId"),caseId=Text(value,"caseId"),roomId=Text(value,"roomId"),socketId=Text(value,"socketId"),
                    runKey=Text(value,"runKey"),mode=Text(value,"mode"),wallId=Text(value,"wallId"),wallSide=Text(value,"wallSide"),actionState=Text(value,"actionState"),
                    hostHierarchy=Text(value,"hostHierarchy"),graphHierarchy=Text(value,"graphHierarchy"),transferHighlightedTarget=Text(value,"transferHighlightedTarget"),
                    transferActiveTarget=Text(value,"transferActiveTarget"),transferPlayerState=Text(value,"transferPlayerState"),transferLastFailure=Text(value,"transferLastFailure"),
                    transferLifecycleReason=Text(value,"transferLifecycleReason",true),transferLifecycleClear=Text(value,"transferLifecycleClear",true),transferLifecyclePreviousTarget=Text(value,"transferLifecyclePreviousTarget",true),
                    monotonicStamp=LongInteger(value,"monotonicStamp"),generation=LongInteger(value,"generation"),frameOrdinal=LongInteger(value,"frameOrdinal"),modeEpoch=LongInteger(value,"modeEpoch"),
                    fixedCall=LongInteger(value,"fixedCall"),displayCall=LongInteger(value,"displayCall"),distinctCount=LongInteger(value,"distinctCount"),duplicateCount=LongInteger(value,"duplicateCount"),
                    unavailableCount=LongInteger(value,"unavailableCount"),streamBytes=LongInteger(value,"streamBytes"),frameCount=Integer(value,"frameCount"),startCall=Integer(value,"startCall"),
                    hostId=UInt64Integer(value,"hostId"),routerId=UInt64Integer(value,"routerId"),hazardsId=UInt64Integer(value,"hazardsId"),playerId=UInt64Integer(value,"playerId"),combatId=UInt64Integer(value,"combatId"),
                    transferId=UInt64Integer(value,"transferId"),collectorId=UInt64Integer(value,"collectorId"),sceneHandle=UInt64Integer(value,"sceneHandle"),
                    tick=Integer(value,"tick"),nextExpectedTick=Integer(value,"nextExpectedTick"),health=Integer(value,"health"),invulnerabilityEndTick=Integer(value,"invulnerabilityEndTick"),
                    positionXQ4096=Integer(value,"positionXQ4096"),positionYQ4096=Integer(value,"positionYQ4096"),velocityXQ4096=Integer(value,"velocityXQ4096"),velocityYQ4096=Integer(value,"velocityYQ4096"),
                    transferRevision=Integer(value,"transferRevision"),transferTransitionLockEndTick=Integer(value,"transferTransitionLockEndTick"),transferRemovalCount=Integer(value,"transferRemovalCount"),
                    transferLifecycleTick=Integer(value,"transferLifecycleTick"),transferLifecycleRevision=Integer(value,"transferLifecycleRevision"),
                    isDead=Boolean(value,"isDead"),isGrounded=Boolean(value,"isGrounded"),airDashAvailable=Boolean(value,"airDashAvailable"),hadAim=Boolean(value,"hadAim"),hadPress=Boolean(value,"hadPress"),
                    sameSourcesBeforeAfter=Boolean(value,"sameSourcesBeforeAfter"),firstTickZero=Boolean(value,"firstTickZero"),exitCandidate=Boolean(value,"exitCandidate"),independentAcceptance=Boolean(value,"independentAcceptance"),
                    bodyCenterX=FloatReal(value,"bodyCenterX"),bodyCenterY=FloatReal(value,"bodyCenterY"),bodyWidth=FloatReal(value,"bodyWidth"),bodyHeight=FloatReal(value,"bodyHeight"),bodyFootY=FloatReal(value,"bodyFootY"),
                    elapsedSeconds=Real(value,"elapsedSeconds"),readMilliseconds=Real(value,"readMilliseconds"),sourceMetadataMilliseconds=Real(value,"sourceMetadataMilliseconds"),previousAppendFlushMilliseconds=Real(value,"previousAppendFlushMilliseconds"),
                    healthPublication=ProjectPublication(RequiredObject(value,"healthPublication")),killPlanePublication=ProjectPublication(RequiredObject(value,"killPlanePublication")),
                    lethalCrushPublication=ProjectPublication(RequiredObject(value,"lethalCrushPublication"))
                };
                if(result.schemaVersion!=1 || result.kind!="sample")throw new IOException("명시 표본 schemaVersion·kind가 다릅니다.");
                if(!MetadataCostAllowed(result.sourceMetadataMilliseconds))throw new IOException("소스 메타데이터 비용이 유한 비음수가 아닙니다.");
                return result;
            }
        }
        private static Observer.PublicationRow ProjectPublication(JsonElement value)=>new Observer.PublicationRow
        {
            localPublicationNumber=LongInteger(value,"localPublicationNumber"),revision=LongInteger(value,"revision"),issuerNumber=Integer(value,"issuerNumber"),tokenNumber=Integer(value,"tokenNumber"),routerNumber=Integer(value,"routerNumber"),
            kind=Text(value,"kind"),triggered=Boolean(value,"triggered"),ownedByStoredSlot=Boolean(value,"ownedByStoredSlot"),issuerSame=Boolean(value,"issuerSame"),routerSame=Boolean(value,"routerSame")
        };
        private static SourceCheckpoint ReadCheckpoint(Observer.FilePin pin)
        {
            Observer.VerifyPin(pin,EvidenceRoot());var source=ReadSourceCheckpoint(pin.path,EvidenceRoot(),Observer.MaxEventBytes);
            if(source==null || source.fileCount!=590 || source.files==null || source.files.Length!=590)throw new IOException("관측 구현 뒤 실제590 기준이 없습니다.");
            for(var i=0;i<source.files.Length;i++)
            {
                var item=source.files[i];item.path=Path.IsPathRooted(item.path)?Path.GetFullPath(item.path):AbsoluteAsset(item.path);
                for(var j=0;j<i;j++)if(Observer.SamePath(item.path,source.files[j].path))throw new IOException("현재 소스 원장의 경로가 중복입니다.");
            }
            return source;
        }
        private static void VerifyAllSources(Observer.FilePin[] files)
        {foreach(var pin in files)Observer.VerifyPin(pin,ProjectRoot());}
        private static void VerifyExpectedAsset(Observer.FilePin pin,string asset)
        {if(pin==null || !Observer.SamePath(pin.path,AbsoluteAsset(asset)))throw new IOException("정확 제품 원문 경로가 다릅니다.");}
        private static void VerifyToolPins(Observer.FilePin observer,Observer.FilePin observerMeta,Observer.FilePin window,Observer.FilePin windowMeta)
        {
            VerifyExpectedAsset(observer,ObserverAsset);VerifyExpectedAsset(observerMeta,ObserverAsset+".meta");VerifyExpectedAsset(window,WindowAsset);VerifyExpectedAsset(windowMeta,WindowAsset+".meta");
            Observer.VerifyPin(observer,ProjectRoot());Observer.VerifyPin(observerMeta,ProjectRoot());Observer.VerifyPin(window,ProjectRoot());Observer.VerifyPin(windowMeta,ProjectRoot());
        }
        private static string[] ExpectedCases(string room)=>room=="R01" || room=="R06"?new[]{room+"-A"}:new[]{room+"-A",room+"-B"};
        private static void ExactStrings(string[] actual,string[] expected)
        {if(actual==null || actual.Length!=expected.Length)throw new InvalidOperationException("필수 사례 집합이 다릅니다.");for(var i=0;i<expected.Length;i++)if(actual[i]!=expected[i])throw new InvalidOperationException("필수 사례 집합·순서가 다릅니다.");}
        private static void RequireExactCases(CaseApproval[] cases)
        {
            var expected=new[]{"R01-A","R02-A","R02-B","R03-A","R03-B","R04-A","R04-B","R05-A","R05-B","R06-A"};
            if(cases==null || cases.Length!=expected.Length)throw new InvalidOperationException("승인된 정확10 구성 목록이 없습니다.");
            for(var i=0;i<expected.Length;i++)if(cases[i]==null || cases[i].caseId!=expected[i] || !Observer.ValidCase(cases[i].caseId,cases[i].roomId,cases[i].socketId) ||
                cases[i].reviewMenu!="AcadeGameMaker/Chapter1/Review Basic Movement/"+Observer.MenuSuffix(expected[i]) || cases[i].sceneName!="Chapter1BasicMovementReview"+cases[i].roomId)
                throw new InvalidOperationException("승인 구성 목록·메뉴·실제 A/B가 다릅니다.");
        }
    }
}
