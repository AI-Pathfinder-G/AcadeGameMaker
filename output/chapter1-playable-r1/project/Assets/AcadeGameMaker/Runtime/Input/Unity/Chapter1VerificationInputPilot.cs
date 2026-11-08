using System;
using System.Collections.Generic;

using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-SOP-001/003: runtime authority is distinct from fixture evidence.
    internal interface IChapter1StandaloneVerificationOwner
    {
        bool TryObserveStandaloneOwner(Chapter1VerificationInputPilot requester,out Chapter1StandaloneVerificationSession session);
    }
    internal sealed class Chapter1StandaloneVerificationSession
    {
        private static long _next;
        internal readonly object Owner;
        internal readonly string Mode;
        internal readonly long Id;
        internal Chapter1VerificationInputPilot Pilot {get;private set;}
        internal bool Active {get;private set;}=true;
        internal Chapter1StandaloneVerificationSession(object owner,string mode)
        {Owner=owner;Mode=mode;Id=System.Threading.Interlocked.Increment(ref _next);}
        internal static bool Admitted(string mode,bool editor,bool development,bool optionValidated,bool evidenceCreated,bool saveEmpty)
        {return (mode=="branch1" || mode=="branch2") && !editor && !development && optionValidated && evidenceCreated && saveEmpty;}
        internal static bool MatchesRequest(object pilot,object requester,bool ready,bool finishing,bool closed)
        {return pilot!=null && ReferenceEquals(pilot,requester) && ready && !finishing && !closed;}
        internal static bool ObservationValid(bool active,bool devices,bool keyboardSeen,bool mouseSeen,bool payload,bool states,bool foreignChanged,bool focused)
        {return active && devices && keyboardSeen && mouseSeen && payload && states && !foreignChanged && focused;}
        internal bool Bind(Chapter1VerificationInputPilot pilot)
        {if(!Active || Pilot!=null || pilot==null)return false;Pilot=pilot;return true;}
        // REQ/AC-SOP-002: focus transitions permanently revoke this session.
        internal bool RevokeForFocusChange(bool before,bool after)
        {if(before==after)return false;Revoke();return true;}
        internal void Revoke(){Active=false;}
    }
    // REQ-AUTOCH1-SAMEBUILD-002/003/004/005/006. Own devices only; production access is observational.
    internal sealed class Chapter1VerificationInputPilot : IDisposable
    {
        private long _beforeCalls,_afterCalls,_dynamicBeforeCalls,_nonDynamicBeforeCalls,_disposedReturns,_focusReturns;
        private bool _subscriptionsActive;
        private string _lastBoundarySignature;
        private string _lastBoundaryUpdateType="NotObserved";
        private int _boundaryRows,_generalBoundaryRows,_mandatoryBoundaryRows;
        private long _armingCycle,_firstEdgeSequence=-1;private bool _firstEdgeConsumed;
        internal Chapter1VerificationInputBoundarySnapshot CaptureInputBoundarySnapshot()
        {
            var settings=InputSystem.settings;
            return new Chapter1VerificationInputBoundarySnapshot{BeforeCalls=_beforeCalls,AfterCalls=_afterCalls,DynamicBeforeCalls=_dynamicBeforeCalls,NonDynamicBeforeCalls=_nonDynamicBeforeCalls,DisposedReturns=_disposedReturns,FocusReturns=_focusReturns,ActualFocus=Application.isFocused,LastUpdateType=_lastBoundaryUpdateType,PublishedSequence=_sequence,ConsumedSequence=_consumed,OwnedEvents=_ownedEvents,NaturalUpdates=NaturalUpdates,KeyboardId=_keyboard.deviceId,MouseId=_mouse.deviceId,Error=Error,Disposed=_disposed,SubscriptionsActive=_subscriptionsActive,KeyboardAdded=_keyboard.added,MouseAdded=_mouse.added,KeyboardEnabled=_keyboard.enabled,MouseEnabled=_mouse.enabled,UpdateMode=settings.updateMode.ToString(),Background=settings.backgroundBehavior.ToString(),EditorInput=settings.editorInputBehaviorInPlayMode.ToString(),Batch=Application.isBatchMode,AuthorizedBatchContext=_contextValid,PublicationPermitted=_publicationPermitted,PermissionSource=_permissionSource,ContextEpoch=_contextEpoch,ActualFocusFalseBeforeCalls=_actualFocusFalseBeforeCalls,ContextPermitCalls=_contextPermitCalls,ContextDenials=_contextDenials,ResumeNeutralConsumed=_contextNeutralConsumed,ArmingCycle=_armingCycle,WitnessKind="sample",WitnessSequence=0,WitnessUpdate=UnityEngine.InputSystem.LowLevel.InputState.updateCount,KeyboardEventSequence=_keyboardEventSequence,MouseEventSequence=_mouseEventSequence,IsolatedApi=_contextEvidence.IsolatedApi,ExpectedKeyboardEventId=_expectedKeyboardEventId,ExpectedMouseEventId=_expectedMouseEventId,ObservedKeyboardEventId=_observedKeyboardEventId,ObservedMouseEventId=_observedMouseEventId,PendingPair=_pendingPair!=null,ContextCaseName=_contextEvidence.CaseName,ContextRunId=_contextEvidence.RunId,ContextPhase=_contextEvidence.Phase,ContextSourceSha=_contextEvidence.SourceSha,ContextOwnerPid=_contextEvidence.OwnerPid,ContextOwnerBirth=_contextEvidence.OwnerBirth};
        }
        internal void RecordInputBoundarySnapshot(string context,bool final=false)
        {
            var value=CaptureInputBoundarySnapshot();
            var signature=context+"|"+value.ActualFocus+"|"+value.LastUpdateType+"|"+value.Error+"|"+value.SubscriptionsActive+"|"+value.UpdateMode+"|"+value.Background+"|"+value.EditorInput;
            if(!final && (signature==_lastBoundarySignature || _generalBoundaryRows>=23))return;
            if(_boundaryRows>=32)throw new InvalidOperationException("최종 입력 관측 한도입니다.");
            var json=JsonUtility.ToJson(new Chapter1VerificationInputBoundaryEnvelope{context=context,final=final,snapshot=value});
            _bossBudget.Add("boundary "+json);_record("boundary "+json);_lastBoundarySignature=signature;_boundaryRows++;if(!final)_generalBoundaryRows++;
        }
        private void RecordMandatoryInputWitness(string kind,long sequence)
        {
            if(_mandatoryBoundaryRows>=8 || _boundaryRows>=31)throw new InvalidOperationException("필수 입력 시점 관측 한도입니다.");
            var value=CaptureInputBoundarySnapshot();value.WitnessKind=kind;value.WitnessSequence=sequence;value.WitnessUpdate=InputState.updateCount;
            var json=JsonUtility.ToJson(new Chapter1VerificationInputBoundaryEnvelope{context="mandatory-input-witness",final=false,snapshot=value});
            _bossBudget.Add("boundary "+json);_record("boundary "+json);_mandatoryBoundaryRows++;_boundaryRows++;
        }
        private sealed class OwnedPair
        {internal Key[] Keys;internal Vector2 Point;internal bool Attack,Transfer,KeyboardSeen,MouseSeen;internal long Sequence,Cycle,Dynamic;internal double Started,KeyboardTime,MouseTime;internal int KeyboardEventId,MouseEventId,KeyboardDeviceId,MouseDeviceId;internal double KeyboardClockBefore,KeyboardClockAfter,MouseClockBefore,MouseClockAfter,KeyboardQueuedTime,MouseQueuedTime;internal double KeyboardBeforeTime,KeyboardBeforeNow,KeyboardAfterNow,MouseBeforeTime,MouseBeforeNow,MouseAfterNow;}
        private OwnedPair _pendingPair;private int _expectedKeyboardEventId,_expectedMouseEventId,_observedKeyboardEventId,_observedMouseEventId;
        private Chapter1StandaloneVerificationSession _standaloneSession;
        private OwnedPair _standalonePair;
        private bool _standaloneAttachAttempted,_standaloneFocused;
        private long _standaloneEpoch=1,_standaloneConsumed=-1;
        private int _standaloneNeutral;
        private bool _standaloneCarryLogged;
        private bool StandaloneReady => _standaloneSession!=null && _standaloneSession.Active && ReferenceEquals(_standaloneSession.Pilot,this) && _standaloneNeutral>=2 && _standaloneFocused;
        internal bool AttachStandaloneOwner(IChapter1StandaloneVerificationOwner owner)
        {
            if(_standaloneAttachAttempted || _ownedContext!=null || _disposed || owner==null)return false;
            _standaloneAttachAttempted=true;
            var type=owner.GetType();
            if(type.FullName!="AcadeGameMaker.Run.Unity.Chapter1SameBuildVerificationDriver" || type.Assembly.GetName().Name!="AcadeGameMaker.Run.Unity" || !type.IsSealed)return false;
            Chapter1StandaloneVerificationSession session;
            if(!owner.TryObserveStandaloneOwner(this,out session) || session==null || !session.Active || !ReferenceEquals(session.Owner,owner) || !session.Bind(this))return false;
            _standaloneSession=session;_standaloneFocused=Application.isFocused;
            _record("standalone-owner attached=true session="+session.Id+" mode="+session.Mode+" fixtureEvidence=false productionDeviceId=unavailable exclusiveInputAttribution=false");
            return true;
        }
        internal void RevokeStandaloneOwner()
        {if(_standaloneSession==null)return;if(_standaloneSession!=null)_standaloneSession.Revoke();_standalonePair=null;_standaloneNeutral=0;if(_bossPolicy!=null)_bossPolicy.Focus(false);}
        private void ObserveStandalonePair(InputEventPtr value,InputDevice device)
        {
            var pair=_standalonePair;
            if(pair==null || !_standaloneSession.Active || InputState.currentUpdateType!=InputUpdateType.Dynamic || value.type!=StateEvent.Type || pair.Sequence!=_sequence || pair.Cycle!=_standaloneEpoch || value.deviceId!=device.deviceId)
                throw new InvalidOperationException("단독 소유 사건의 세션/갱신/발행 신원 불일치입니다.");
            if(ReferenceEquals(device,_keyboard))
            {
                if(pair.KeyboardSeen)throw new InvalidOperationException("단독 키보드 중복 사건입니다.");
                foreach(var key in _keyboard.allKeys){float actual;if(!key.ReadValueFromEvent(value,out actual) || (actual>=.5f)!=(Array.IndexOf(pair.Keys,key.keyCode)>=0))throw new InvalidOperationException("단독 키보드 사건 payload 불일치입니다.");}
                pair.KeyboardSeen=true;pair.KeyboardEventId=value.id;pair.KeyboardTime=value.time;pair.KeyboardDeviceId=value.deviceId;
            }
            else
            {
                if(pair.MouseSeen)throw new InvalidOperationException("단독 마우스 중복 사건입니다.");
                Vector2 point;float left,right;
                if(!_mouse.position.ReadValueFromEvent(value,out point) || !_mouse.leftButton.ReadValueFromEvent(value,out left) || !_mouse.rightButton.ReadValueFromEvent(value,out right) || !point.Equals(pair.Point) || (left>=.5f)!=pair.Attack || (right>=.5f)!=pair.Transfer)throw new InvalidOperationException("단독 마우스 사건 payload 불일치입니다.");
                pair.MouseSeen=true;pair.MouseEventId=value.id;pair.MouseTime=value.time;pair.MouseDeviceId=value.deviceId;
            }
        }
        private readonly IChapter1OwnedBatchInputContext _ownedContext;
        private void PublishOwnedPair()
        {
            if(!_contextEvidence.IsolatedApi || _pendingPair!=null)throw new InvalidOperationException("격리 발행 수명/중복입니다.");
            var p=new OwnedPair{Keys=(Key[])_keys.Clone(),Point=_point,Attack=_attack,Transfer=_transfer,Sequence=_sequence+1,Cycle=_armingCycle,Dynamic=_dynamicBeforeCalls,Started=Time.realtimeSinceStartupAsDouble};_pendingPair=p;
            var keyboardExpected=ReadOwnedClock();p.KeyboardClockBefore=keyboardExpected;using(var memory=StateEvent.FromDefaultStateFor(_keyboard,out var value))
            {foreach(var key in _keyboard.allKeys)key.WriteValueIntoEvent(Array.IndexOf(p.Keys,key.keyCode)>=0?1f:0f,value);p.KeyboardBeforeTime=value.time;p.KeyboardBeforeNow=InputState.currentTime;InputSystem.QueueEvent(value);p.KeyboardEventId=value.id;p.KeyboardQueuedTime=value.time;p.KeyboardTime=ConfirmOwnedClock(keyboardExpected);p.KeyboardClockAfter=p.KeyboardTime;p.KeyboardDeviceId=value.deviceId;p.KeyboardAfterNow=InputState.currentTime;RecordReceiptObservation(p,true,"queued-keyboard",p.KeyboardEventId,p.KeyboardTime,p.KeyboardDeviceId,p.KeyboardDeviceId,false,false,false,0,p.KeyboardQueuedTime,value.type.ToString());}
            var mouseExpected=ReadOwnedClock();p.MouseClockBefore=mouseExpected;using(var memory=StateEvent.FromDefaultStateFor(_mouse,out var value))
            {_mouse.position.WriteValueIntoEvent(p.Point,value);_mouse.leftButton.WriteValueIntoEvent(p.Attack?1f:0f,value);_mouse.rightButton.WriteValueIntoEvent(p.Transfer?1f:0f,value);p.MouseBeforeTime=value.time;p.MouseBeforeNow=InputState.currentTime;InputSystem.QueueEvent(value);p.MouseEventId=value.id;p.MouseQueuedTime=value.time;p.MouseTime=ConfirmOwnedClock(mouseExpected);p.MouseClockAfter=p.MouseTime;p.MouseDeviceId=value.deviceId;p.MouseAfterNow=InputState.currentTime;RecordReceiptObservation(p,false,"queued-mouse",p.MouseEventId,p.MouseTime,p.MouseDeviceId,p.MouseDeviceId,false,false,false,0,p.MouseQueuedTime,value.type.ToString());}
            if(p.KeyboardEventId<=0 || p.MouseEventId<=0 || p.KeyboardEventId==p.MouseEventId)throw new InvalidOperationException("격리 큐의 실제 양수 ID가 없습니다.");
            _expectedKeyboardEventId=p.KeyboardEventId;_expectedMouseEventId=p.MouseEventId;_observedKeyboardEventId=0;_observedMouseEventId=0;_sequence=p.Sequence;_publishedThisUpdate=true;
        }
        // REQ/AC-RTIME-001: same active fixture getter; no clock writes or tolerance.
        private double ReadOwnedClock()
        {double value;if(_ownedContext==null || !_ownedContext.TryReadIsolatedTime(out value) || double.IsNaN(value) || double.IsInfinity(value))throw new InvalidOperationException("격리 시계 읽기 거절입니다.");return value;}
        private double ConfirmOwnedClock(double before)
        {var after=ReadOwnedClock();if(before!=after)throw new InvalidOperationException("사건 생성/큐 사이 격리 시계가 바뀌었습니다.");return before;}
        private void ObserveOwnedPair(InputEventPtr value,InputDevice device)
        {
            var p=_pendingPair;if(p==null || p.Cycle!=_armingCycle || value.type!=StateEvent.Type)throw new InvalidOperationException("미해결 영수증 없는 소유 사건입니다.");
            var keyboard=ReferenceEquals(device,_keyboard);var expectedId=keyboard?p.KeyboardEventId:p.MouseEventId;var expectedTime=keyboard?p.KeyboardTime:p.MouseTime;
            var comparedId=0;var comparedTime=0d;var comparedEventDevice=0;var comparedDevice=0;var timeEvaluated=false;var deviceEvaluated=false;
            if((comparedId=value.id)!=expectedId || ((timeEvaluated=true) && (comparedTime=value.time)!=expectedTime) || ((deviceEvaluated=true) && (comparedEventDevice=value.deviceId)!=(comparedDevice=device.deviceId)))
            {var mismatch=comparedId!=expectedId?1:timeEvaluated&&comparedTime!=expectedTime?2:4;try{RecordReceiptObservation(p,keyboard,"mismatch",comparedId,comparedTime,comparedEventDevice,comparedDevice,true,timeEvaluated,deviceEvaluated,mismatch,value.time,value.type.ToString());}catch(Exception trace){_receiptObservationError=trace.GetType().Name;}throw new InvalidOperationException("큐 영수증 ID/시간/장치 불일치입니다.");}
            RecordReceiptObservation(p,keyboard,keyboard?"observed-keyboard":"observed-mouse",comparedId,comparedTime,comparedEventDevice,comparedDevice,true,timeEvaluated,deviceEvaluated,0,value.time,value.type.ToString());
            if(InputState.currentUpdateType!=InputUpdateType.Dynamic)throw new InvalidOperationException("Dynamic 이외의 격리 사건입니다.");
            if(keyboard){if(p.KeyboardSeen)throw new InvalidOperationException("중복 키보드 사건입니다.");foreach(var key in _keyboard.allKeys){float actual;if(!key.ReadValueFromEvent(value,out actual) || (actual>=.5f)!=(Array.IndexOf(p.Keys,key.keyCode)>=0))throw new InvalidOperationException("키보드 사건 payload/형식 불일치입니다.");}p.KeyboardSeen=true;_observedKeyboardEventId=value.id;_keyboardEventSequence=p.Sequence;}
            else{if(p.MouseSeen)throw new InvalidOperationException("중복 마우스 사건입니다.");Vector2 point;float left,right;if(!_mouse.position.ReadValueFromEvent(value,out point)||!_mouse.leftButton.ReadValueFromEvent(value,out left)||!_mouse.rightButton.ReadValueFromEvent(value,out right)||!point.Equals(p.Point)||(left>=.5f)!=p.Attack||(right>=.5f)!=p.Transfer)throw new InvalidOperationException("마우스 사건 payload/형식 불일치입니다.");p.MouseSeen=true;_observedMouseEventId=value.id;_mouseEventSequence=p.Sequence;}
        }
        private int _receiptRows;private long _receiptOrdinal;private bool _receiptMismatchRecorded;private string _receiptObservationError;
        private void RecordReceiptObservation(OwnedPair p,bool keyboard,string stage,int id,double time,int eventDevice,int device,bool idEvaluated,bool timeEvaluated,bool deviceEvaluated,int mismatch,double diagnosticTime,string eventType)
        {
            if(p.Sequence>2 && mismatch==0)return;if(mismatch!=0 && _receiptMismatchRecorded)return;if(_receiptRows>=9)throw new InvalidOperationException("영수증9행 상한입니다.");
            var queued=keyboard?p.KeyboardQueuedTime:p.MouseQueuedTime;var now=InputState.currentTime;var expected=keyboard?p.KeyboardTime:p.MouseTime;var beforeTime=keyboard?p.KeyboardBeforeTime:p.MouseBeforeTime;var beforeNow=keyboard?p.KeyboardBeforeNow:p.MouseBeforeNow;var afterNow=keyboard?p.KeyboardAfterNow:p.MouseAfterNow;
            var row=new Chapter1ReceiptObservation{Stage=stage,EventType=eventType,QueueBeforeTimeRoundTrip=beforeTime.ToString("R",System.Globalization.CultureInfo.InvariantCulture),QueueBeforeNowRoundTrip=beforeNow.ToString("R",System.Globalization.CultureInfo.InvariantCulture),ClockBeforeBits=BitConverter.DoubleToInt64Bits(keyboard?p.KeyboardClockBefore:p.MouseClockBefore),ClockAfterBits=BitConverter.DoubleToInt64Bits(keyboard?p.KeyboardClockAfter:p.MouseClockAfter),ClockCoordinate="same-fixture-public-currentTime",QueueAfterTimeRoundTrip=queued.ToString("R",System.Globalization.CultureInfo.InvariantCulture),QueueAfterNowRoundTrip=afterNow.ToString("R",System.Globalization.CultureInfo.InvariantCulture),QueueBeforeTimeBits=BitConverter.DoubleToInt64Bits(beforeTime),QueueBeforeNowBits=BitConverter.DoubleToInt64Bits(beforeNow),QueueAfterTimeBits=BitConverter.DoubleToInt64Bits(queued),QueueAfterNowBits=BitConverter.DoubleToInt64Bits(afterNow),Ordinal=++_receiptOrdinal,Sequence=p.Sequence,Cycle=p.Cycle,UpdateType=InputState.currentUpdateType.ToString(),UpdateCount=InputState.updateCount,ExpectedId=keyboard?p.KeyboardEventId:p.MouseEventId,ComparedId=id,ExpectedTimeBits=BitConverter.DoubleToInt64Bits(expected),ComparedTimeBits=BitConverter.DoubleToInt64Bits(time),DiagnosticTimeBits=BitConverter.DoubleToInt64Bits(diagnosticTime),DiagnosticNowBits=BitConverter.DoubleToInt64Bits(now),ExpectedTimeRoundTrip=expected.ToString("R",System.Globalization.CultureInfo.InvariantCulture),ComparedTimeRoundTrip=timeEvaluated?time.ToString("R",System.Globalization.CultureInfo.InvariantCulture):"not-evaluated",DiagnosticTimeRoundTrip=diagnosticTime.ToString("R",System.Globalization.CultureInfo.InvariantCulture),DiagnosticNowRoundTrip=now.ToString("R",System.Globalization.CultureInfo.InvariantCulture),ExpectedDeviceId=keyboard?p.KeyboardDeviceId:p.MouseDeviceId,ComparedEventDeviceId=eventDevice,ComparedDeviceId=device,EvaluatedMask=(idEvaluated?1:0)|(timeEvaluated?2:0)|(deviceEvaluated?4:0),MismatchMask=mismatch,KeyboardSeen=p.KeyboardSeen,MouseSeen=p.MouseSeen,IdEvaluated=idEvaluated,TimeEvaluated=timeEvaluated,DeviceEvaluated=deviceEvaluated,CaseName=_contextEvidence.CaseName,RunId=_contextEvidence.RunId,Phase=_contextEvidence.Phase,SourceSha=_contextEvidence.SourceSha,OwnerPid=_contextEvidence.OwnerPid,OwnerBirth=_contextEvidence.OwnerBirth};
            var json=JsonUtility.ToJson(row);_bossBudget.Add("receipt "+json);_record("receipt "+json);_receiptRows++;if(mismatch!=0)_receiptMismatchRecorded=true;
        }
        private bool _contextValid,_publicationPermitted,_lastActualFocus,_contextObserved;
        private long _contextEpoch,_actualFocusFalseBeforeCalls,_contextPermitCalls,_contextDenials;
        private int _contextNeutralConsumed;
        private string _permissionSource="ActualFocus";private Chapter1OwnedBatchInputEvidence _contextEvidence;
        internal bool ContextReady => _ownedContext==null || (_contextValid && _contextNeutralConsumed>=2);
        private bool ControlAvailable => _ownedContext==null ? Application.isFocused : (_publicationPermitted && ContextReady);
        private bool ReadPublicationPermission()
        {
            var actual=Application.isFocused;
            if(_ownedContext==null){_publicationPermitted=actual;return actual;}
            Chapter1OwnedBatchInputEvidence evidence=default(Chapter1OwnedBatchInputEvidence);var valid=false;
            try{valid=_ownedContext.TryObserve(out evidence);}catch(Exception error){Error="맥락 관측: "+error;Neutral();_contextValid=false;_publicationPermitted=false;_contextDenials++;return false;}
            valid=valid && Application.isEditor && Application.isBatchMode && Application.isPlaying;
            if(!_contextObserved || actual!=_lastActualFocus || valid!=_contextValid || (valid && evidence.Epoch!=_contextEpoch))
            {Neutral();_contextNeutralConsumed=0;_armingCycle++;_firstEdgeSequence=-1;_firstEdgeConsumed=false;}
            _contextEvidence=evidence;_contextObserved=true;_lastActualFocus=actual;_contextValid=valid;_contextEpoch=valid?evidence.Epoch:0;
            _publicationPermitted=actual||valid;_permissionSource=valid?"OwnedBatchInputContext":"ActualFocus";
            if(!valid)_contextDenials++;
            return _publicationPermitted;
        }
        private readonly Keyboard _keyboard;
        private readonly Mouse _mouse;
        private readonly Action<string> _record;
        private Key[] _keys=Array.Empty<Key>();
        private Vector2 _point;
        private bool _attack,_transfer,_tap,_releaseTap,_disposed,_publishedThisUpdate;
        private long _sequence,_consumed,_keyboardEventSequence=-1,_mouseEventSequence=-1,_foreignEvents,_ownedEvents;
        private string _lastIssued="",_lastConsumed="";
        private double _lastInputLog;
        private Chapter1VerificationRoomPolicy _policy;
        private int _ordinal,_lastCombatTick=-1,_priorHealth=-1,_skillBaselineTick=-1;
        private bool _roomTransferPending,_roomTransferAwaitRelease,_ownedRightObserved,_ownedRightWasPressed;
        private long _completedCombatOrdinal,_lastRoomTransferOrdinal=-1;
        private string _roomTransferTarget="";
        private Vector2 _roomTransferPoint;
        private long _skillRequestSequence=-1,_qConsumedSequence=-1;
        private bool _skillEffectObserved;
        private InputRouter _observedRouter;
        private Chapter1VerificationBossPolicy _bossPolicy;
        private InputRouter _bossRouter;
        private int _bossApproachLogged;
        private string _bossRouterChange="unbound",_bossPreviousRouter="none";
        private int _bossRouterReplacements;
        private int _bossObservationTick=-1,_bossPriorPlayer=-1,_bossPriorHealth=-1;
        private string _bossSignature="",_bossCommandSignature="";
        private readonly Chapter1VerificationBossBudget _bossBudget=new Chapter1VerificationBossBudget();
        private readonly Dictionary<string,int> _enemyHealth=new Dictionary<string,int>();
        internal string Error {get;private set;}
        internal string AttributionLimit => "exclusiveInputAttribution=false productionDeviceId=unavailable nonOwnedChangedEvents="+_foreignEvents+" ownedStateEvents="+_ownedEvents;
        internal bool InitialPositiveHealth {get;private set;}
        internal bool EnemyDamageObserved {get;private set;}
        internal bool NaturalZeroObserved {get;private set;}
        internal int LastDamageTick {get;private set;}=-1;
        internal int LastDamagePriorHealth {get;private set;}=-1;
        internal int LastDamageCurrentHealth {get;private set;}=-1;
        internal int LastDamageAmount {get;private set;}
        internal string LastDamageSource {get;private set;}
        internal long NaturalUpdates {get;private set;}
        internal bool NeutralConsumed => !_keyboard.anyKey.isPressed && !_mouse.leftButton.isPressed && !_mouse.rightButton.isPressed && _consumed==_sequence;
        internal Chapter1VerificationInputPilot(Action<string> record,IChapter1OwnedBatchInputContext ownedContext=null)
        {
            _record=record;_ownedContext=ownedContext;if(ownedContext!=null)_bossBudget=ownedContext.Budget;
            if(ownedContext!=null){Chapter1OwnedBatchInputEvidence initial;if(!ownedContext.TryObserve(out initial) || !initial.IsolatedApi || !Application.isEditor || !Application.isBatchMode || !Application.isPlaying)throw new InvalidOperationException("소유 배치 입력 맥락 선행이 없습니다.");_contextEvidence=initial;_contextValid=true;_contextEpoch=initial.Epoch;_publicationPermitted=true;_permissionSource="OwnedBatchInputContext";}
            try
            {
                _keyboard=InputSystem.AddDevice<Keyboard>("CH1VerificationKeyboard");
                _mouse=InputSystem.AddDevice<Mouse>("CH1VerificationMouse");
                InputSystem.onEvent+=ObserveEvent;
                InputSystem.onBeforeUpdate+=Before;
                InputSystem.onAfterUpdate+=After;_subscriptionsActive=true;
                _record("devices-created keyboard="+_keyboard.deviceId+" mouse="+_mouse.deviceId);
            }
            catch(Exception original)
            {
                var errors=new List<Exception>{original};
                try{InputSystem.onEvent-=ObserveEvent;}catch(Exception error){errors.Add(error);}
                try{InputSystem.onBeforeUpdate-=Before;}catch(Exception error){errors.Add(error);}
                try{InputSystem.onAfterUpdate-=After;}catch(Exception error){errors.Add(error);}
                try{if(_mouse!=null)InputSystem.RemoveDevice(_mouse);}catch(Exception error){errors.Add(error);}
                try{if(_keyboard!=null)InputSystem.RemoveDevice(_keyboard);}catch(Exception error){errors.Add(error);}
                if(errors.Count>1)throw new AggregateException("생성 실패와 소유 정리 실패입니다.",errors);
                throw;
            }
        }
        internal void Neutral(){_roomTransferAwaitRelease=_roomTransferAwaitRelease||_roomTransferPending||_transfer;_keys=Array.Empty<Key>();_attack=false;_transfer=false;_tap=false;_point=Vector2.zero;_roomTransferPending=false;}
        internal void Hold(Key[] keys,Vector2 point,bool attack=false,bool transfer=false)
        {_keys=keys;_point=point;_attack=attack;_transfer=transfer;_tap=false;}
        internal void TapSubmit(){Neutral();_keys=new[]{Key.Enter};_tap=true;_record("intent tap=Enter nextSequence="+(_sequence+1));}
        internal void TapClick(Vector2 point,bool right=false){Neutral();_point=point;_attack=!right;_transfer=right;_tap=true;_record("intent tap="+(right?"right":"left")+" point="+point+" nextSequence="+(_sequence+1));}
        private void Before()
        {
            _beforeCalls++;_lastBoundaryUpdateType=InputState.currentUpdateType.ToString();if(InputState.currentUpdateType==InputUpdateType.Dynamic)_dynamicBeforeCalls++;else _nonDynamicBeforeCalls++;
            if(_disposed)_disposedReturns++;
            if(InputState.currentUpdateType!=InputUpdateType.Dynamic || _disposed)return;
            _publishedThisUpdate=false;
            if(_standaloneSession!=null && _standaloneSession.RevokeForFocusChange(_standaloneFocused,Application.isFocused))
            {_standaloneFocused=Application.isFocused;_standaloneEpoch++;RevokeStandaloneOwner();Neutral();Error="단독 소유 초점 세대 변경입니다.";return;}
            if(_standaloneSession!=null && (!_keyboard.added || !_mouse.added || !_keyboard.enabled || !_mouse.enabled)){RevokeStandaloneOwner();Error="단독 소유 장치 수명 변경입니다.";return;}
            if(_standaloneSession!=null && _standaloneSession.Active && _standaloneNeutral<2)Neutral();
            if(!Application.isFocused)_actualFocusFalseBeforeCalls++;
            var permitted=ReadPublicationPermission();if(_bossPolicy!=null)_bossPolicy.Focus(ControlAvailable);
            if(!permitted){_focusReturns++;if(_bossPolicy!=null)Neutral();return;}
            if(_ownedContext!=null && _contextValid && !Application.isFocused)_contextPermitCalls++;
            if(_ownedContext!=null && !ContextReady)Neutral();
            if(_ownedContext!=null && _pendingPair!=null)
            {if(_pendingPair.Cycle!=_armingCycle || _dynamicBeforeCalls-_pendingPair.Dynamic>=4 || Time.realtimeSinceStartupAsDouble-_pendingPair.Started>=1){Error="격리 입력 한쌍 대기 상한입니다.";return;}_publishedThisUpdate=true;return;}
            try
            {
                if(_releaseTap){Neutral();_releaseTap=false;}
                if(_ownedContext!=null && ContextReady && _firstEdgeSequence<0 && (_keys.Length>0 || _attack || _transfer))
                {_firstEdgeSequence=_sequence+1;RecordMandatoryInputWitness("before-first-edge",_firstEdgeSequence);}
                var state=new MouseState{position=_point};if(_attack)state=state.WithButton(MouseButton.Left);if(_transfer)state=state.WithButton(MouseButton.Right);
                if(_ownedContext!=null)PublishOwnedPair();else{
                if(_standaloneSession!=null && _standaloneSession.Active)_standalonePair=new OwnedPair{Keys=(Key[])_keys.Clone(),Point=_point,Attack=_attack,Transfer=_transfer,Sequence=_sequence+1,Cycle=_standaloneEpoch,Dynamic=_dynamicBeforeCalls};
                InputSystem.QueueStateEvent(_keyboard,new KeyboardState(_keys));
                InputSystem.QueueStateEvent(_mouse,state);_sequence++;_publishedThisUpdate=true;}
                var signature=string.Join(",",_keys)+"/"+_attack+"/"+_transfer;
                if(signature!=_lastIssued && Time.realtimeSinceStartupAsDouble-_lastInputLog>=1)
                {_record("issued sequence="+_sequence+" keys="+signature+" point="+_point+" consumption=unobserved");_lastIssued=signature;_lastInputLog=Time.realtimeSinceStartupAsDouble;}
                if(_tap){_releaseTap=true;_tap=false;}
            }
            catch(Exception error){if(_standaloneSession!=null)RevokeStandaloneOwner();Error="입력 발행: "+error;}
        }
        private void ObserveEvent(InputEventPtr value,InputDevice device)
        {
            if(_disposed || device==null || (value.type!=StateEvent.Type && value.type!=DeltaStateEvent.Type))return;
            try
            {
                if(ReferenceEquals(device,_keyboard) || ReferenceEquals(device,_mouse))
                {
                    _ownedEvents++;
                    if(_ownedContext!=null)ObserveOwnedPair(value,device);else{if(_standaloneSession!=null && _standaloneSession.Active)ObserveStandalonePair(value,device);if(ReferenceEquals(device,_keyboard))_keyboardEventSequence=_sequence;else _mouseEventSequence=_sequence;}
                    if(_ownedEvents<=4)_record("owned-event device="+device.deviceId+" sequence="+_sequence+" id="+value.id+" time="+value.time+" stateConsumption=unobserved");
                }
                else
                {
                    var changed=0;foreach(var control in value.EnumerateChangedControls(device))changed++;
                    if(changed>0)
                    {
                        _foreignEvents++;if(_standaloneSession!=null && _standaloneSession.Active)RevokeStandaloneOwner();if(_ownedContext!=null)throw new InvalidOperationException("격리 시험계의 비소유 사건입니다.");
                        if(_foreignEvents<=16)_record("nonowned-input device="+device.deviceId+" layout="+device.layout+" changedControls="+changed+" exclusiveInputAttribution=false");
                    }
                }
            }
            catch(Exception error){if(_standaloneSession!=null)RevokeStandaloneOwner();Error="입력 장치 사건 관측: "+error;}
        }
        private void After()
        {
            _afterCalls++;_lastBoundaryUpdateType=InputState.currentUpdateType.ToString();
            if(InputState.currentUpdateType!=InputUpdateType.Dynamic || _disposed)return;
            if(!_publishedThisUpdate || !ReadPublicationPermission())return;
            try
            {
                NaturalUpdates++;
                var receipt=_pendingPair;
                if(_ownedContext!=null && (receipt==null || !receipt.KeyboardSeen || !receipt.MouseSeen))return;
                var standalone=_standaloneSession!=null && _standaloneSession.Active;
                var own=standalone?_standalonePair:null;
                var standaloneStates=own!=null && _mouse.position.ReadValue().Equals(own.Point) && _mouse.leftButton.isPressed==own.Attack && _mouse.rightButton.isPressed==own.Transfer;
                if(standaloneStates)foreach(var key in _keyboard.allKeys){if(key.isPressed!=(Array.IndexOf(own.Keys,key.keyCode)>=0)){standaloneStates=false;break;}}
                if(standalone && (own==null || !Chapter1StandaloneVerificationSession.ObservationValid(_standaloneSession.Active,_keyboard.added && _mouse.added && _keyboard.enabled && _mouse.enabled,own.KeyboardSeen,own.MouseSeen,own.Sequence==_sequence && own.Cycle==_standaloneEpoch && own.Dynamic==_dynamicBeforeCalls,standaloneStates,_foreignEvents>0,_standaloneFocused)))
                {RevokeStandaloneOwner();Error="단독 사건/상태 소비 선행 누락입니다.";return;}
                var expectedKeys=_ownedContext==null?(standalone?own.Keys:_keys):receipt.Keys;var expectedAttack=_ownedContext==null?(standalone?own.Attack:_attack):receipt.Attack;var expectedTransfer=_ownedContext==null?(standalone?own.Transfer:_transfer):receipt.Transfer;
                if(standalone && !_mouse.position.ReadValue().Equals(own.Point)){RevokeStandaloneOwner();Error="단독 포인터 상태 불일치입니다.";return;}
                if(_ownedContext!=null && (InputSystem.devices.Count!=2 || !_keyboard.added || !_mouse.added || !_mouse.position.ReadValue().Equals(receipt.Point)))throw new InvalidOperationException("격리 장치 집합/포인터 상태 불일치입니다.");
                if(_keyboardEventSequence!=_sequence || _mouseEventSequence!=_sequence)throw new InvalidOperationException("자체 장치의 현재 발행 사건을 관측하지 못했습니다.");
                foreach(var key in _keyboard.allKeys){var expected=Array.IndexOf(expectedKeys,key.keyCode)>=0;if(key.isPressed!=expected)throw new InvalidOperationException("소유 키보드 요청과 실제 상태가 다릅니다.");}
                if(_mouse.leftButton.isPressed!=expectedAttack || _mouse.rightButton.isPressed!=expectedTransfer)throw new InvalidOperationException("소유 마우스 요청과 실제 상태가 다릅니다.");
                _consumed=_sequence;if(_ownedContext!=null)_pendingPair=null;
                if(_ownedContext!=null && _contextValid && _contextNeutralConsumed<2 && !_keyboard.anyKey.isPressed && !expectedAttack && !expectedTransfer)
                {_contextNeutralConsumed++;RecordMandatoryInputWitness("neutral-consumed-"+_contextNeutralConsumed,_consumed);}
                if(_ownedContext!=null && _firstEdgeSequence==_consumed && !_firstEdgeConsumed)
                {RecordMandatoryInputWitness("first-edge-consumed",_consumed);_firstEdgeConsumed=true;}
                if(_bossPolicy!=null)_bossPolicy.Consumed(expectedAttack,!_keyboard.anyKey.isPressed && !expectedAttack && !expectedTransfer,_sequence);
                if(_bossPolicy!=null && _ownedContext!=null)_bossPolicy.ObserveOwnedConsumption(this,_ownedContext,_contextEvidence.SourceSha,_contextEvidence.CaseName,_contextEvidence.OwnerBirth,_contextEvidence.OwnerPid,_contextEpoch,_armingCycle,expectedKeys,_sequence,_contextValid && _contextNeutralConsumed>=2);
                if(standalone)
                {
                    _standaloneConsumed=_sequence;
                    if(_standaloneNeutral<2 && !_keyboard.anyKey.isPressed && !expectedAttack && !expectedTransfer)_standaloneNeutral++;
                    if(_bossPolicy!=null)_bossPolicy.ObserveStandaloneConsumption(this,_standaloneSession,_standaloneEpoch,expectedKeys,_sequence,StandaloneReady);
                }
                if(_roomTransferPending && _mouse.rightButton.isPressed)
                {
                    if(!_ownedRightObserved || _ownedRightWasPressed)throw new InvalidOperationException("방 전이 입력의 실제 해제 뒤 새 오른쪽 edge가 아닙니다.");
                    _roomTransferPending=false;_roomTransferAwaitRelease=true;_lastRoomTransferOrdinal=checked(_completedCombatOrdinal+1);
                    _record("room-transfer right-state-consumed sequence="+_sequence+" completedBeforeOrdinal="+_completedCombatOrdinal+" localCompletedBefore="+_lastCombatTick+" requestedTarget="+_roomTransferTarget+" actualTransferEffect=unobserved");
                }
                else if(_roomTransferAwaitRelease && !_mouse.rightButton.isPressed)_roomTransferAwaitRelease=false;
                _ownedRightWasPressed=_mouse.rightButton.isPressed;_ownedRightObserved=true;
                if(Array.IndexOf(expectedKeys,Key.Q)>=0 && _keyboard.qKey.isPressed)_qConsumedSequence=_sequence;
                var signature=_keyboard.anyKey.isPressed+"/"+_mouse.leftButton.isPressed+"/"+_mouse.rightButton.isPressed;
                if(signature!=_lastConsumed && NaturalUpdates%30==0)
                {_record("device-state sequence="+_consumed+" update="+InputState.updateCount+" keyboard="+_keyboard.deviceId+" mouse="+_mouse.deviceId+" actual="+signature);_lastConsumed=signature;}
            }
            catch(Exception error){if(_standaloneSession!=null)RevokeStandaloneOwner();Error="입력 관측: "+error;}
        }
        // Called after completed producers and before Flow's 210 failure/retirement processing.
        internal bool ObserveCombat(InputRouter router,Action<string> progress)
        {
            if(router==null)return false;
            if(ObserveBossCombat(router,progress))return true;
            if(!ReferenceEquals(_observedRouter,router))
            {_observedRouter=router;_lastCombatTick=-1;_priorHealth=-1;_enemyHealth.Clear();_policy=null;}
            var combat=router.transform.parent.GetComponentInChildren<CombatSimulationDriver>(true);
            if(combat==null || !combat.HasLatestOutcomeForEncounterHandoff)return false;
            var outcome=combat.LatestOutcomeForEncounterHandoff;
            if(outcome.Tick.Value==_lastCombatTick)return false;
            _completedCombatOrdinal=checked(_completedCombatOrdinal+1);
            var contiguous=_lastCombatTick>=0 && outcome.Tick.Value==_lastCombatTick+1;
            var hp=-1;foreach(var state in outcome.Combatants)
            {
                if(state.TargetId=="player")hp=state.CurrentHealth;
                else
                {
                    if(_enemyHealth.TryGetValue(state.TargetId,out var previous) && state.CurrentHealth<previous)progress("enemy-hp target="+state.TargetId+" before="+previous+" after="+state.CurrentHealth+" tick="+outcome.Tick.Value);
                    _enemyHealth[state.TargetId]=state.CurrentHealth;
                }
            }
            if(hp>0)InitialPositiveHealth=true;
            var damage=0;string sources="";
            for(var i=0;i<outcome.DamageResults.Count;i++)
            {
                var result=outcome.DamageResults[i];var request=outcome.CanonicalRequestTrace[i];
                if(result.TargetId=="player" && result.AppliedAmount>0 && result.Result==DamageResultCode.Applied && request.SourceId!="player" &&
                    (request.Kind==DamageKind.EnemyContact || request.Kind==DamageKind.EnemyRanged))
                {
                    damage+=result.AppliedAmount;sources+=request.SourceId+":"+request.Kind+":"+request.RequestId+";";
                }
            }
            if(damage>0)
            {
                _record("enemy-damage tick="+outcome.Tick.Value+" contiguous="+contiguous+" priorHp="+_priorHealth+" hp="+hp+" applied="+damage+" source="+sources);
                if(contiguous && _priorHealth>0 && hp==Math.Max(0,_priorHealth-damage))
                {
                    EnemyDamageObserved=true;LastDamageTick=outcome.Tick.Value;LastDamagePriorHealth=_priorHealth;LastDamageCurrentHealth=hp;LastDamageAmount=damage;LastDamageSource=sources;
                    if(hp==0)NaturalZeroObserved=true;
                }
            }
            _lastCombatTick=outcome.Tick.Value;_priorHealth=hp;return true;
        }
        internal void PlayRoom(InputRouter router,string roomId,Rect[] floors,Rect[] terrain,Rect exit,string activeTransferTarget)
        {
            if(router==null){Neutral();return;}
            if(_policy==null || !ReferenceEquals(_policy.Router,router))
            {_policy=new Chapter1VerificationRoomPolicy(router,roomId,floors,terrain,exit,_record);Neutral();}
            var command=_policy.Read(_ordinal++,true,!_keyboard.spaceKey.isPressed && !_mouse.rightButton.isPressed && !_mouse.leftButton.isPressed,NaturalUpdates);var keys=new List<Key>(command.Keys);
            var combat=router.transform.parent.GetComponentInChildren<CombatSimulationDriver>(true);
            if(roomId=="R02" && combat!=null && combat.HasLatestOutcomeForEncounterHandoff)
            {
                var alive=false;foreach(var state in combat.LatestOutcomeForEncounterHandoff.Combatants)if(state.TargetId!="player" && !state.IsDead && state.CurrentHealth>0)alive=true;
                if(!alive)keys.Remove(Key.LeftShift);
            }
            if(_roomTransferPending && (!command.AimValid || command.Target!=_roomTransferTarget || command.TargetHealth<=0))
            {_roomTransferPending=false;_roomTransferAwaitRelease=true;}
            if(!_roomTransferPending && !_roomTransferAwaitRelease && _ownedRightObserved && !_ownedRightWasPressed && string.IsNullOrEmpty(activeTransferTarget) && command.Target.Length!=0 && command.TargetHealth>0 && command.AimValid && command.ObservedTick>=0 && (_lastRoomTransferOrdinal<0 || _completedCombatOrdinal-_lastRoomTransferOrdinal>=24))
            {
                _roomTransferPending=true;_roomTransferTarget=command.Target;_roomTransferPoint=command.Pointer;
                _record("room-transfer intent target="+command.Target+" completedOrdinal="+_completedCombatOrdinal+" localCompletedTick="+command.ObservedTick+" actualTransferEffect=unobserved");
            }
            if(_roomTransferPending && command.Target==_roomTransferTarget && command.AimValid)_roomTransferPoint=command.Pointer;
            Hold(keys.ToArray(),_roomTransferPending?_roomTransferPoint:command.Pointer,command.Attack && !_roomTransferPending && !_roomTransferAwaitRelease,_roomTransferPending);
        }
        // REQ-BVP-001~005: completed publications only, owned normal input only.
        private static string BossRouterIdentity(InputRouter router)
        {
            if(ReferenceEquals(router,null))return "unavailable routerReason=not-bound";
            if(router==null)return "unavailable routerReason=retired-Unity-object";
            try{var id=router.GetEntityId();return id.Equals(default(EntityId))?"unavailable routerReason=public-entity-id-zero":id.ToString()+" routerReason=public-UnityObject.GetEntityId";}
            catch(Exception error){return "unavailable routerReason=public-entity-id-read-"+error.GetType().Name;}
        }
        private void BossRecord(string detail)
        {
            detail+=" router="+BossRouterIdentity(_bossRouter)+" routerIdentityScope=process-object-lifetime routerChange="+_bossRouterChange+" routerReplacementOrdinal="+_bossRouterReplacements+" previousRouter="+_bossPreviousRouter;
            _bossBudget.Add(detail);_record("boss "+detail);
        }
        private bool ObserveBossCombat(InputRouter router,Action<string> progress)
        {
            var graph=router.transform.parent;
            var combat=graph.GetComponentInChildren<OrdanBossCombatSimulationDriver>(true);
            if(combat==null){if(!ReferenceEquals(_bossRouter,null)){_bossPreviousRouter=BossRouterIdentity(_bossRouter).Replace(" routerReason="," previousRouterReason=");_bossRouterChange="retired";}_bossPolicy=null;_bossRouter=null;return false;}
            if(!combat.HasLatestOutcome)return false;
            if(!ReferenceEquals(_bossRouter,router))
            {
                var hadPrevious=!ReferenceEquals(_bossRouter,null) || _bossRouterChange=="retired";if(!ReferenceEquals(_bossRouter,null))_bossPreviousRouter=BossRouterIdentity(_bossRouter).Replace(" routerReason="," previousRouterReason=");
                _bossRouterChange=hadPrevious?"replacement":"initial-binding";if(hadPrevious)_bossRouterReplacements++;
                _bossRouter=router;_bossPolicy=new Chapter1VerificationBossPolicy();_bossApproachLogged=0;_bossObservationTick=-1;_bossPriorPlayer=-1;_bossPriorHealth=-1;_bossSignature="";}
            var value=combat.LatestOutcome;
            if(value.Tick.Value==_bossObservationTick)return true;
            var bridge=graph.GetComponentInChildren<OrdanBossSimulationDriver>(true);
            var hostile=graph.GetComponentInChildren<OrdanBossHostileDamageProducer>(true);
            var pull=graph.GetComponentInChildren<OrdanBossBalanceAuditPullProducer>(true);
            if(bridge==null || hostile==null || pull==null || !bridge.HasLatestOutcome || !hostile.HasLatestOutcome || !pull.HasLatestOutcome)
                throw new InvalidOperationException("보스 완료 관측 원본이 없습니다.");
            if(bridge.LatestOutcome.SourceTick!=value.Tick || hostile.LatestOutcome.Tick!=value.Tick)
                throw new InvalidOperationException("보스 완료 생산 tick이 다릅니다.");
            var hp=-1;var boss=-1;
            foreach(var state in value.Combatants){if(state.TargetId=="player")hp=state.CurrentHealth;else if(state.TargetId=="ordan")boss=state.CurrentHealth;}
            var snapshot=bridge.LatestOutcome.Output.Snapshot;var h=hostile.LatestOutcome;
            var signature=snapshot.PatternSlot+"/"+snapshot.Phase+"/"+h.Digest.Disposition+"/"+h.Digest.AttackOrdinal;
            var trace="";var damage=0;
            for(var i=0;i<value.DamageResults.Count;i++)
            {
                var q=value.CanonicalRequestTrace[i];var d=value.DamageResults[i];
                damage+=d.AppliedAmount;trace+=Chapter1VerificationBossPolicy.FormatTrace(q,d,value.Tick.Value);
            }
            if(value.DamageResults.Count>0 || damage>0 || hp!=_bossPriorPlayer || boss!=_bossPriorHealth || signature!=_bossSignature || value.Tick.Value%60==0)
            {
                var pose=router.BoundPlayerForAuthoring.Snapshot;
                var receipt=router.CurrentReceipt;var exact=router.TryGetChapter1CompletedEvidence(out var evidence);
                BossRecord("completed tick="+value.Tick.Value+" movementTick="+pose.Tick.Value+" contiguous="+(_bossObservationTick>=0 && value.Tick.Value==_bossObservationTick+1)+
                    " run="+(exact?evidence.RunKey:"unavailable")+" generation="+(exact?evidence.Generation:-1)+" receipt="+(receipt.HasValue?receipt.Value.Tick.Value:-1)+
                    " playerBefore="+_bossPriorPlayer+" playerHp="+hp+" bossBefore="+_bossPriorHealth+" bossHp="+boss+
                    " xQ="+pose.PositionXQ4096+" yQ="+pose.PositionYQ4096+" vxQ="+pose.VelocityXQ4096+" vyQ="+pose.VelocityYQ4096+
                    " slot="+snapshot.PatternSlot+" phase="+snapshot.Phase+" age="+snapshot.PhaseAge+" attackOrdinal="+h.Digest.AttackOrdinal+
                    " hostile="+h.Digest.Disposition+" geometry="+h.Observation.GeometryKey+" contact="+h.Observation.Contact+" occluded="+h.Observation.Occluded+
                    " hostileSourceTick="+h.Tick.Value+" hostileDelivery="+(h.Digest.DeliveryTick.HasValue?h.Digest.DeliveryTick.Value.Value:-1)+" hostileRequest="+h.Digest.RequestId+
                    " pull="+pull.LatestOutcome.Disposition.ToString()+" pullSource="+pull.LatestOutcome.SourceTick.Value+" pullOrdinal="+pull.LatestOutcome.AttackOrdinal+" pullQueued="+pull.LatestOutcome.QueuedNextDirectiveId+" attackAttempt="+(value.Attack.Attempt.HasValue?value.Attack.Attempt.Value.Code.ToString():"none")+
                    " cooldownEnd="+value.Attack.Snapshot.CooldownEndTick+" issued="+_sequence+" consumed="+_consumed+" attackState="+_bossPolicy.AttackState+trace);
                if(boss>=0 && _bossPriorHealth>=0 && boss<_bossPriorHealth)progress("boss-hp tick="+value.Tick.Value+" before="+_bossPriorHealth+" after="+boss);
            }
            _bossObservationTick=value.Tick.Value;_bossPriorPlayer=hp;_bossPriorHealth=boss;_bossSignature=signature;return true;
        }
        internal void PlayBoss(InputRouter router,Vector2 position,int ordinal)
        {
            // position/ordinal are retained for the existing caller; neither controls policy time.
            if(router==null || !router.TryGetChapter1CompletedEvidence(out var evidence)){Neutral();return;}
            if(!ReferenceEquals(_bossRouter,router))ObserveBossCombat(router,_=>{});
            var graph=router.transform.parent;var boss=graph.Find("Ordan");
            var combat=graph.GetComponentInChildren<OrdanBossCombatSimulationDriver>(true);
            var bridge=graph.GetComponentInChildren<OrdanBossSimulationDriver>(true);
            var player=router.BoundPlayerForAuthoring;var pose=player.Snapshot;
            if(_bossPolicy==null || boss==null || combat==null || !combat.HasLatestOutcome || bridge==null || !bridge.HasLatestOutcome ||
                combat.LatestOutcome.Tick!=evidence.Tick || bridge.LatestOutcome.SourceTick!=evidence.Tick || pose.Tick!=evidence.Tick)
                throw new InvalidOperationException("정확 보스 완료 관측 결속이 아닙니다.");
            if(evidence.AliveEnemyCount==0){Neutral();return;}
            var provider=router.BoundCameraProviderForAuthoring as IInputCameraSnapshotProvider;
            var point=Vector2.zero;var aimValid=false;
            if(provider!=null && ReferenceEquals(provider.BoundPlayer,player) && provider.TryGetCompletedCamera(evidence.Tick,out var camera))
            {
                var aim=boss.position;var u=.5+(aim.x-camera.PositionXQ100/100d)/(2*camera.OrthoSizeQ1000/1000d*16/9);
                var v=.5+(aim.y-camera.PositionYQ100/100d)/(2*camera.OrthoSizeQ1000/1000d);
                point=new Vector2((float)(camera.GameplayRectX+u*(camera.GameplayRectWidth-1)),(float)(camera.GameplayRectY+v*(camera.GameplayRectHeight-1)));
                aimValid=u>=0 && u<=1 && v>=0 && v<=1 && ((Vector2)aim-evidence.Position).sqrMagnitude<=30.25f;
            }
            var state=bridge.LatestOutcome.Output.Snapshot;
            var hostile=graph.GetComponentInChildren<OrdanBossHostileDamageProducer>(true);var pull=graph.GetComponentInChildren<OrdanBossBalanceAuditPullProducer>(true);
            if(hostile==null || pull==null || !hostile.HasLatestOutcome || !pull.HasLatestOutcome || hostile.LatestOutcome.Tick!=evidence.Tick || pull.LatestOutcome.SourceTick!=evidence.Tick)
                throw new InvalidOperationException("회피의 정확 hostile/pull 출판이 없습니다.");
            var threat=new Chapter1VerificationBossPolicy.Threat(state.ActiveStage,state.IsAccelerated,hostile.LatestOutcome.Digest.Disposition==OrdanBossHostileDisposition.Queued,
                hostile.LatestOutcome.Observation.Contact,hostile.LatestOutcome.Observation.Occluded,pull.LatestOutcome.Disposition==OrdanBossBalanceAuditPullDisposition.Queued,
                pull.AuthoredAnchorXQ4096,pull.AuthoredAnchorYQ4096,pull.AuthoredMaximumStepQ4096,
                bridge.LatestOutcome.SeizureTrajectoryProjection.HasValue?bridge.LatestOutcome.SeizureTrajectoryProjection.Value.FrozenDuration:0,
                pose.WallSide!=AcadeGameMaker.Movement.WallSide.None || !string.IsNullOrEmpty(pose.WallId));
            var observedRun=evidence.RunKey;var observedGeneration=evidence.Generation;var observedTick=evidence.Tick.Value;
            var observedX=pose.PositionXQ4096/4096f;var observedY=pose.PositionYQ4096/4096f;var observedVx=pose.VelocityXQ4096/4096f;
            var observedGrounded=pose.IsGrounded;var observedLocked=player.IsInputGateLocked;var observedFocused=ControlAvailable;var observedAim=aimValid && evidence.AliveEnemyCount>0;
            var observedCooldown=combat.LatestOutcome.Attack.Snapshot.CooldownEndTick;var observedSlot=state.PatternSlot;var observedPhase=state.Phase;var observedAge=state.PhaseAge;
            var observedLatency=Time.maximumDeltaTime+Time.fixedDeltaTime;var observedVy=pose.VelocityYQ4096/4096f;
            if(_standaloneSession!=null)
                _bossPolicy.BindOwnedJump(new Chapter1VerificationBossPolicy.OwnedJumpIdentity(this,_standaloneSession,router,player,_standaloneEpoch,router.CurrentReceipt.Value.ModeEpoch,StandaloneReady));
            else
            _bossPolicy.BindOwnedJump(new Chapter1VerificationBossPolicy.OwnedJumpIdentity(this,_ownedContext,router,player,_contextEvidence.SourceSha,_contextEvidence.CaseName,_contextEvidence.OwnerBirth,_contextEvidence.OwnerPid,_contextEpoch,_armingCycle,router.CurrentReceipt.Value.ModeEpoch,_ownedContext!=null && ContextReady));
            if(_standaloneSession!=null && _bossPolicy.OwnedContinuationActive!=_standaloneCarryLogged)
            {
                _standaloneCarryLogged=_bossPolicy.OwnedContinuationActive;
                var pair=_standalonePair;
                BossRecord("standalone-continuation active="+_standaloneCarryLogged+" session="+_standaloneSession.Id+" authorityActive="+_standaloneSession.Active+" epoch="+_standaloneEpoch+" tick="+observedTick+" deadline="+_bossPolicy.OwnedContinuationDeadline+" published="+_sequence+" consumed="+_standaloneConsumed+" keyboard="+_keyboard.deviceId+" mouse="+_mouse.deviceId+" keyboardEvent="+(pair==null?0:pair.KeyboardEventId)+" mouseEvent="+(pair==null?0:pair.MouseEventId)+" kind=OwnDeviceStateDown fixtureEvidence=false productionDeviceId=unavailable exclusiveInputAttribution=false");
            }
            var command=_bossPolicy.Read(observedRun,observedGeneration,observedTick,observedX,observedY,observedVx,observedGrounded,observedLocked,observedFocused,observedAim,observedCooldown,observedSlot,observedPhase,observedAge,observedLatency,observedVy,threat);
            if(_bossApproachLogged!=_bossPolicy.RecoveryApproachState){_bossApproachLogged=_bossPolicy.RecoveryApproachState;BossRecord("navigation state="+_bossApproachLogged+" tick="+observedTick+" x="+observedX+" y="+observedY+" actualSpacePress="+_bossPolicy.RecoveryApproachPressObserved+" actualRising="+_bossPolicy.RecoveryApproachAirborneObserved+" futureSafetyGuarantee=false");}
            if(_bossPolicy.Unresolved!=null){Neutral();BossRecord("unresolved tick="+evidence.Tick.Value+" reason="+_bossPolicy.Unresolved+
                " observedRun="+observedRun+" observedGeneration="+observedGeneration+" observedTick="+observedTick+" x="+observedX.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+" y="+observedY.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+
                " vx="+observedVx.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+" vy="+observedVy.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+" grounded="+observedGrounded+" locked="+observedLocked+" focused="+observedFocused+
                " slot="+observedSlot+" phase="+observedPhase+" age="+observedAge+" latency="+observedLatency.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+" jumpStart=unavailable-not-captured jumpUntil=unavailable-not-captured candidateReasons=unavailable-not-captured");throw new InvalidOperationException(_bossPolicy.Unresolved);}
            var commandSignature=string.Join(",",command.Keys)+"/"+command.Attack;
            if(commandSignature!=_bossCommandSignature){BossRecord("intent tick="+evidence.Tick.Value+" run="+evidence.RunKey+" generation="+evidence.Generation+" keys="+commandSignature+" point="+point+" nextSequence="+(_sequence+1)+" consumed="+_consumed+" state="+_bossPolicy.AttackState);_bossCommandSignature=commandSignature;}
            Hold(command.Keys,point,command.Attack);
        }
        internal void ApproachForFailure(Vector2 position){Hold(position.x<6?new[]{Key.D}:Array.Empty<Key>(),Vector2.zero);}
        internal void BarrierTransfer(InputRouter router)
        {
            var graph=router.transform.parent;var target=graph.Find("Counterweight");var camera=graph.GetComponentInChildren<Camera>(true);
            if(target==null || camera==null)throw new InvalidOperationException("실제 전이 조준 원본이 없습니다.");
            TapClick(camera.WorldToScreenPoint(target.position),true);
        }
        internal void CaptureSkillBaseline(InputRouter router,Chapter1EncounterEvidenceView view,int branch)
        {
            var graph=router.transform.parent;var environment=graph.GetComponentInChildren<Chapter1ChoiceEnvironmentOwner>(true);
            var platform=graph.Find("Platform").GetComponent<BoxCollider2D>();
            if(environment==null || environment.HasActualSkill || platform==null || platform.enabled)
                throw new InvalidOperationException("Q 이전 실제 기술/발판 초기 상태가 아닙니다.");
            _skillBaselineTick=view.Tick.Value;_skillEffectObserved=false;
            _record("skill-baseline branch="+branch+" tick="+_skillBaselineTick+" hasActualSkill=false platformEnabled=false generation="+view.Generation+" run="+view.RunKey);
        }
        internal void TapSkill(){Neutral();_keys=new[]{Key.Q};_tap=true;_skillRequestSequence=_sequence+1;_record("intent tap=Q nextSequence="+_skillRequestSequence);}
        internal bool SkillEffectObserved(InputRouter router,Chapter1EncounterEvidenceView view,int branch)
        {
            var graph=router.transform.parent;var environment=graph.GetComponentInChildren<Chapter1ChoiceEnvironmentOwner>(true);
            var publication=graph.GetComponentInChildren<BarrierCombatSimulationDriver>(true).LatestPublication;
            if(_skillBaselineTick<0 || _qConsumedSequence<_skillRequestSequence || _skillRequestSequence<0 || view.Tick.Value<=_skillBaselineTick ||
                environment==null || !environment.HasActualSkill || publication==null || !publication.Owner.ValidatePublished(publication) ||
                !ReferenceEquals(publication.Environment,environment) || publication.Tick!=view.Tick || publication.Generation!=view.Generation || !Equals(publication.RunKey,view.RunKey))return false;
            string effect;
            if(branch==2)
            {
                var platform=graph.Find("Platform").GetComponent<BoxCollider2D>();var weight=graph.Find("Counterweight").GetComponent<Rigidbody2D>();
                if(!platform.enabled || platform.size.x<=0 || platform.size.y<=0 || (weight.constraints&RigidbodyConstraints2D.FreezePosition)!=RigidbodyConstraints2D.FreezePosition)return false;
                effect="platformEnabled=true platformSize="+platform.size+" platformLocal="+platform.transform.localPosition+" counterweightFreezePosition=true";
            }
            else
            {
                var actualDamage=0;
                for(var i=0;i<publication.Requests.Count;i++)
                {
                    var request=publication.Requests[i];var result=publication.Results[i];
                    if(request.Kind==DamageKind.ChoiceSkill && request.SourceId=="BarrierCounterweight" && request.TargetId==result.TargetId && request.RequestId==result.RequestId &&
                        request.Tick==view.Tick && result.Tick==view.Tick && result.Result==DamageResultCode.Applied)actualDamage+=result.AppliedAmount;
                }
                if(actualDamage<=0 || environment.StunEndTick<=view.Tick.Value)return false;
                effect="actualChoiceDamage="+actualDamage+" stunEndTick="+environment.StunEndTick;
            }
            if(!_skillEffectObserved)_record("skill-effect branch="+branch+" baselineTick="+_skillBaselineTick+" actualTick="+view.Tick.Value+" qConsumedSequence="+_qConsumedSequence+" generation="+view.Generation+" run="+view.RunKey+" "+effect);
            _skillEffectObserved=true;return true;
        }
        internal bool GateReady(InputRouter router,int branch)
        {
            if(!_skillEffectObserved)return false;
            var graph=router.transform.parent;var environment=graph.GetComponentInChildren<Chapter1ChoiceEnvironmentOwner>(true);
            if(branch==2)return graph.Find("Platform").GetComponent<BoxCollider2D>().enabled;
            return environment.GateOpened && graph.Find("Bridge").GetComponent<BoxCollider2D>().enabled;
        }
        private bool _boxJumpStarted,_boxCleared;
        internal void TraverseBarrier(InputRouter router,Chapter1EncounterEvidenceView view)
        {
            var graph=router.transform.parent;var player=router.BoundPlayerForAuthoring;
            if(!player.TryReadExistingMotionForAuthoring(out var motion,out var next) || motion.Tick!=view.Tick || next.Value!=view.Tick.Value+1){Neutral();return;}
            var obstacle=graph.Find("Counterweight").GetComponent<BoxCollider2D>().bounds;
            var destination=graph.Find("Exit").GetComponent<BoxCollider2D>().bounds;
            var body=view.PlayerBody;const float skin=82f/4096f;var velocity=motion.VelocityXQ4096/4096f;
            var min=destination.min.x+body.width/2+skin;var max=destination.max.x-body.width/2-skin;var center=(min+max)/2;
            if(float.IsNaN(min)||float.IsInfinity(min)||min>=max)throw new InvalidOperationException("출구 기하가 유효하지 않습니다.");
            if(body.xMin>obstacle.max.x+skin)_boxCleared=true;
            if(!_boxCleared && !_boxJumpStarted && body.xMax>=obstacle.min.x-.3f && motion.IsGrounded)_boxJumpStarted=true;
            var keys=new List<Key>();var direction=1;
            if(_boxCleared)
            {
                var stopping=velocity*velocity/(2*(motion.IsGrounded?90f:20f))+Mathf.Abs(velocity)/60+skin;var delta=center-body.center.x;
                if(body.center.x>=min && body.center.x<=max)direction=velocity>.5f?-1:velocity<-.5f?1:0;
                else if(velocity*delta>0 && stopping>=Mathf.Abs(delta))direction=velocity>0?-1:1;
                else direction=delta>0?1:delta<0?-1:0;
            }
            if(direction!=0)keys.Add(direction>0?Key.D:Key.A);if(_boxJumpStarted&&!_boxCleared)keys.Add(Key.Space);
            Hold(keys.ToArray(),Vector2.zero);
        }
        // TMP_Text.m_text is [SerializeField]; observe via Unity's public serializer, without a reflected getter.
        [Serializable] private sealed class SerializedStatus {public string m_text;}
        internal static string ReadStatus(Chapter1ChoicePhaseAdapter source)
        {
            foreach(var component in source.GetComponentsInChildren<MonoBehaviour>(true))
                if(component!=null && component.name=="Status" && component.GetType().FullName=="TMPro.TextMeshProUGUI")
                {
                    var snapshot=JsonUtility.FromJson<SerializedStatus>(JsonUtility.ToJson(component));
                    if(snapshot==null || snapshot.m_text==null)throw new InvalidOperationException("직렬화된 실제 상태 문구를 관측하지 못했습니다.");
                    return snapshot.m_text;
                }
            throw new InvalidOperationException("실제 상태 문구를 관측하지 못했습니다.");
        }
        public void Dispose()
        {
            RevokeStandaloneOwner();
            if(_disposed)return;_disposed=true;
            var errors=new List<Exception>();
            try{InputSystem.onEvent-=ObserveEvent;}catch(Exception error){errors.Add(error);}
            try{InputSystem.onBeforeUpdate-=Before;}catch(Exception error){errors.Add(error);}
            try{InputSystem.onAfterUpdate-=After;_subscriptionsActive=false;}catch(Exception error){errors.Add(error);}
            try{InputSystem.RemoveDevice(_mouse);}catch(Exception error){errors.Add(error);}
            try{InputSystem.RemoveDevice(_keyboard);}catch(Exception error){errors.Add(error);}
            if(errors.Count>0)throw new AggregateException("소유 입력 장치/구독 정리 실패입니다.",errors);
        }
    }
    // REQ-OBI-001: only the approved Editor fixture implements this lifetime lease.
    internal interface IChapter1OwnedBatchInputContext:IDisposable
    {bool TryReadIsolatedTime(out double value);bool TryObserve(out Chapter1OwnedBatchInputEvidence evidence);Chapter1VerificationBossBudget Budget{get;}}
    internal struct Chapter1OwnedBatchInputEvidence
    {public long Epoch;public string CaseName,RunId,Phase,SourceSha,OwnerBirth;public int OwnerPid;public bool IsolatedApi;}
    [Serializable] internal sealed class Chapter1VerificationInputBoundarySnapshot
    {
        public long BeforeCalls,AfterCalls,DynamicBeforeCalls,NonDynamicBeforeCalls,DisposedReturns,FocusReturns,PublishedSequence,ConsumedSequence,OwnedEvents,NaturalUpdates;
        public bool ActualFocus,Disposed,SubscriptionsActive,KeyboardAdded,MouseAdded,KeyboardEnabled,MouseEnabled,Batch,IsolatedApi,PendingPair;
        public int ExpectedKeyboardEventId,ExpectedMouseEventId,ObservedKeyboardEventId,ObservedMouseEventId;
        public bool AuthorizedBatchContext,PublicationPermitted;public string PermissionSource;public long ContextEpoch,ActualFocusFalseBeforeCalls,ContextPermitCalls,ContextDenials;public long ArmingCycle,WitnessSequence,WitnessUpdate,KeyboardEventSequence,MouseEventSequence;public string WitnessKind;
        public int ResumeNeutralConsumed,ContextOwnerPid;public string ContextCaseName,ContextRunId,ContextPhase,ContextSourceSha,ContextOwnerBirth;
        public int KeyboardId,MouseId;
        public string LastUpdateType,Error,UpdateMode,Background,EditorInput;
    }
    [Serializable] internal sealed class Chapter1VerificationInputBoundaryEnvelope
    {public string context;public bool final;public Chapter1VerificationInputBoundarySnapshot snapshot;}
    internal sealed class Chapter1VerificationRoomPolicy
    {
        internal readonly struct Command
        {
            internal Command(Key[] keys,Vector2 pointer,bool attack,string target,int targetHealth,string attackCode,int observedTick=-1,bool aimValid=false)
            {Keys=keys;Pointer=pointer;Attack=attack;Target=target;TargetHealth=targetHealth;AttackCode=attackCode;ObservedTick=observedTick;AimValid=aimValid;}
            internal Key[] Keys {get;}internal Vector2 Pointer {get;}internal bool Attack {get;}
            internal string Target {get;}internal int TargetHealth {get;}internal string AttackCode {get;}
            internal int ObservedTick {get;}
            internal bool AimValid {get;}
        }
        internal InputRouter Router {get;}
        private readonly System.Diagnostics.Stopwatch _watch=System.Diagnostics.Stopwatch.StartNew();
        private double _lastProgress;private float _bestX;private int _lowestEnemyHealth=int.MaxValue,_firstTick=-1;
        private readonly JumpHold _jumpHold=new JumpHold();private bool _traversalJumpIssued,_crushCrossing;
        private int _stepLogged,_lastStepTick=-1;
        private readonly string _roomId; private readonly Rect[] _floors,_terrain; private readonly Rect _exit; private bool _exitApproachLogged;
        private readonly Action<string> _record; private long _dodgeOrdinal=-1; private int _dodgeSide; private bool _missingThreatReported;
        private int _gapPhase,_gapDirection,_gapStartedTick,_gapLaunchTick; private float _gapTakeoff,_gapFar,_gapLanding; private double _gapStartedSeconds; private long _gapReadyUpdate=-1; private bool _gapLifted; private int _gapWaitLogged,_gapLastWaitTick;
        internal Chapter1VerificationRoomPolicy(InputRouter router,string roomId,Rect[] floors,Rect[] terrain,Rect exit,Action<string> record){Router=router;_roomId=roomId;_floors=(Rect[])floors.Clone();_terrain=(Rect[])terrain.Clone();_exit=exit;_record=record;}
        internal Command Read(int ordinal,bool allowJump,bool ownedJumpAndButtonsReleased,long naturalUpdates)
        {
            var graph=Router.transform.parent;var combat=graph.GetComponentInChildren<CombatSimulationDriver>(true);
            if(combat==null)throw new InvalidOperationException("시험 입력의 실제 일반 전투 원본이 없습니다.");
            if(!combat.HasLatestOutcomeForEncounterHandoff)return new Command(Array.Empty<Key>(),Vector2.zero,false,"",-1,"unpublished");
            var outcome=combat.LatestOutcomeForEncounterHandoff;var player=Router.BoundPlayerForAuthoring;var pose=player.Snapshot;
            if(_firstTick<0)_firstTick=pose.Tick.Value;
            ValidateCompletedTick(pose.Tick.Value,_firstTick);
            var x=pose.PositionXQ4096/4096f;var y=pose.PositionYQ4096/4096f;var healthSum=0;var playerHealth=-1;
            foreach(var state in outcome.Combatants){if(state.TargetId=="player")playerHealth=state.CurrentHealth;else if(!state.IsDead)healthSum+=state.CurrentHealth;}
            if(playerHealth<=0)throw new InvalidOperationException("시험 입력 중 실제 플레이어 사망이 관측됐습니다.");
            if(x>_bestX+.25f || healthSum<_lowestEnemyHealth){_bestX=Math.Max(_bestX,x);_lowestEnemyHealth=healthSum;_lastProgress=_watch.Elapsed.TotalSeconds;}
            if(_watch.Elapsed.TotalSeconds-_lastProgress>=60)throw new InvalidOperationException("시험 입력 사례가 60초 동안 실제 진전이 없습니다.");
            var keys=new System.Collections.Generic.List<Key>();var pointer=Vector2.zero;var targetId="";var targetHealth=-1;var nearest=float.PositiveInfinity;var aim=Vector2.zero;
            foreach(var target in combat.FrozenTargetsForEncounterHandoff)
            {
                if(target.TargetId=="player")continue;var alive=false;var hp=0;
                foreach(var state in outcome.Combatants)if(state.TargetId==target.TargetId){alive=!state.IsDead&&state.CurrentHealth>0;hp=state.CurrentHealth;}
                if(!alive)continue;
                var point=(Vector2)target.TargetPoseRoot.position+new Vector2(target.LocalAimPointXQ1000/1000f,target.LocalAimPointYQ1000/1000f);
                var distance=(point-new Vector2(x,y)).sqrMagnitude;if(distance>=nearest)continue;
                nearest=distance;aim=point;targetId=target.TargetId;targetHealth=hp;
            }
            var attacking=false;var aimValid=false;
            if(targetId.Length!=0)
            {
                var dx=aim.x-x;var dy=aim.y-y;
                var approachDirection=ApproachDirection(dx);if(approachDirection!=0)keys.Add(approachDirection>0?Key.D:Key.A);
                var provider=Router.BoundCameraProviderForAuthoring as IInputCameraSnapshotProvider;
                if(provider!=null && ReferenceEquals(provider.BoundPlayer,player) && provider.TryGetCompletedCamera(pose.Tick,out var camera))
                {
                    var u=.5+(aim.x-camera.PositionXQ100/100d)/(2*camera.OrthoSizeQ1000/1000d*16/9);
                    var v=.5+(aim.y-camera.PositionYQ100/100d)/(2*camera.OrthoSizeQ1000/1000d);
                    pointer=new Vector2((float)(camera.GameplayRectX+u*(camera.GameplayRectWidth-1)),(float)(camera.GameplayRectY+v*(camera.GameplayRectHeight-1)));
                    attacking=ShouldAttack(nearest,u,v,ordinal);
                    aimValid=ShouldAttack(nearest,u,v,0);
                }
            }
            else
            {
                keys.Add(Key.D);if((pose.Tick.Value-_firstTick)%90==45)keys.Add(Key.LeftShift);
            }


            var local=graph.InverseTransformPoint(new Vector3(x,y,0));
            if(targetId.Length!=0){keys.Remove(Key.LeftShift);AvoidDenialLine(graph,pose.Tick.Value,x,pose.VelocityXQ4096/4096f,player.GetComponent<CapsuleCollider2D>(),keys);}
            if(targetId.Length!=0)
            {
                var left=float.PositiveInfinity;var right=float.NegativeInfinity;
                foreach(var floor in _floors){left=Mathf.Min(left,floor.xMin);right=Mathf.Max(right,floor.xMax);}
                var body=player.GetComponent<CapsuleCollider2D>();
                if(body==null)throw new InvalidOperationException("전투 외곽 제동의 실제 몸체가 없습니다.");
                // Combat omits Dash; preserve R05 crush crossing and include retained actual velocity.
                // before the next Dynamic update, including grounded-to-air transitions.
                var velocity=pose.VelocityXQ4096/4096f;var speed=Mathf.Max(_roomId=="R05" && local.x>=10.5f && local.x<15.2f?20f:9.5f,Mathf.Abs(velocity));
                var burst=Time.maximumDeltaTime+Time.fixedDeltaTime;
                var distance=body.bounds.extents.x+.15f+speed*burst+speed*speed/(2f*20f);
                var margin=Mathf.Abs(graph.InverseTransformVector(new Vector3(distance,0,0)).x);
                if(float.IsNaN(margin)||float.IsInfinity(margin)||margin<=0 || float.IsInfinity(left)||float.IsInfinity(right)||burst<=0 || margin*2>=right-left)
                    throw new InvalidOperationException("전투 외곽/다음 입력 전 최대 진행거리의 안전 여유가 유효하지 않습니다.");
                if(local.x<=left+margin && aim.x>=x){keys.Remove(Key.A);keys.Remove(Key.LeftShift);if(velocity<0 && !keys.Contains(Key.D))keys.Add(Key.D);}
                else if(local.x>=right-margin && aim.x<=x){keys.Remove(Key.D);keys.Remove(Key.LeftShift);if(velocity>0 && !keys.Contains(Key.A))keys.Add(Key.A);}
            }
            if(TryTraverseGap(graph,pose,player.IsInputGateLocked,local.x,player.GetComponent<CapsuleCollider2D>(),keys,targetId,graph.InverseTransformPoint(new Vector3(aim.x,aim.y,0)).x,ownedJumpAndButtonsReleased,naturalUpdates,out var gapCommand))return gapCommand;
            // REQ-VERIFEXIT-001/002: completed observations select normal keys; never move the player directly.
            if(TryReadExitApproach(_exit,local.x,graph.InverseTransformVector(new Vector3(pose.VelocityXQ4096/4096f,0,0)).x,healthSum>0 || targetId.Length!=0,_gapPhase!=0,out var exitCommand))
            {
                _jumpHold.Read(pose.Tick.Value,false,true);
                if(!_exitApproachLogged)
                {
                    _exitApproachLogged=true;
                    _record("exit-approach tick="+pose.Tick.Value+" x="+local.x+" y="+local.y+" vx="+(pose.VelocityXQ4096/4096f)+" vy="+(pose.VelocityYQ4096/4096f)+" grounded="+pose.IsGrounded+" wall="+pose.WallSide+" enemyHealth="+healthSum+" exit="+_exit+" keys="+string.Join(",",exitCommand.Keys)+" physicalRecovery=unobserved");
                }
                return exitCommand;
            }
            var ceiling=IsLowCeiling(_roomId,local.x);
            if(allowJump && _roomId=="R05" && local.x>=10.5f && local.x<15.2f)
            {
                var hazards=graph.GetComponentInChildren<Chapter1HazardObservation>(true);
                if(hazards==null)throw new InvalidOperationException("압착 통과 정책의 실제 위험 원본이 없습니다.");
                var open=CrushWindowOpen(pose.Tick.Value,tick=>hazards.IsClosedAt(new AcadeGameMaker.Core.SimulationTick(tick)));
                keys.Remove(Key.A);keys.Remove(Key.D);keys.Remove(Key.LeftShift);
                if(_crushCrossing || open){keys.Add(Key.D);if(!_crushCrossing)keys.Add(Key.LeftShift);_crushCrossing=true;}
            }
            var direction=keys.Contains(Key.D)?1:keys.Contains(Key.A)?-1:0;
            var capsule=player.GetComponent<CapsuleCollider2D>();
            if(capsule==null)throw new InvalidOperationException("시험 이동 정책의 원본 capsule이 없습니다.");
            var foot=local.y-capsule.size.y/2+capsule.offset.y;
            var obstacle=false;
            if(direction!=0)
            {
                foreach(var terrain in _terrain)
                {
                    var ahead=direction>0?terrain.xMin-local.x:local.x-terrain.xMax;
                    if(NeedsStepJump(ahead,foot,terrain.yMin,terrain.yMax))
                    {
                        obstacle=true;
                        if(_stepLogged<12 && pose.Tick.Value-_lastStepTick>=30)
                        {
                            _stepLogged++;_lastStepTick=pose.Tick.Value;
                            _record("step-observation tick="+pose.Tick.Value+" x="+x+" y="+y+" foot="+foot+" top="+terrain.yMax+" vx="+(pose.VelocityXQ4096/4096f)+" vy="+(pose.VelocityYQ4096/4096f)+" ground="+pose.IsGrounded+" wall="+pose.WallSide);
                        }
                    }
                }
                var floors=_floors;
                for(var n=0;n+1<floors.Length;n++)
                {
                    var takeoff=direction>0?floors[n].xMax:floors[n+1].xMin;
                    var ahead=direction>0?takeoff-local.x:local.x-takeoff;
                    if(NeedsGapJump(ahead,floors[n+1].xMin-floors[n].xMax))obstacle=true;
                }
            }
            var wantsJump=pose.WallSide!=AcadeGameMaker.Movement.WallSide.None ||
                (pose.IsGrounded && (obstacle || (targetId.Length!=0&&(Math.Abs(aim.y-y)>1.8f || nearest<=30.25f)) || (targetId.Length==0&&!_traversalJumpIssued)));
            if(_jumpHold.Read(pose.Tick.Value,wantsJump,!allowJump||ceiling)){keys.Add(Key.Space);_traversalJumpIssued=true;}
            var code=outcome.Attack.Attempt.HasValue?outcome.Attack.Attempt.Value.Code.ToString():"none";
            return new Command(keys.ToArray(),pointer,attacking,targetId,targetHealth,code,outcome.Tick.Value,aimValid);
        }
        // REQ-AUTOCH1-SAMEBUILD-003/004: observe immutable completed publications; emit keys only.
        private void AvoidDenialLine(Transform graph,int tick,float x,float velocity,CapsuleCollider2D body,List<Key> keys)
        {
            if(body==null)throw new InvalidOperationException("회피 관측의 실제 capsule이 없습니다.");
            var behavior=graph.GetComponentInChildren<RegularEnemyBehaviorSimulationDriver>(true)?.LatestCarriedViewForEncounterHandoff;
            var threat=graph.GetComponentInChildren<RegularEnemyThreatSimulationDriver>(true)?.LatestCarriedViewForEncounterHandoff;
            if(!behavior.HasValue || !threat.HasValue || behavior.Value.Tick.Value!=tick || threat.Value.Tick.Value!=tick)
            {
                if(!_missingThreatReported){_record("denial-observation unavailable exactCompletedTick="+tick+" avoidanceEffect=unobserved");_missingThreatReported=true;}
                return;
            }
            _missingThreatReported=false;
            var state=behavior.Value.Snapshot.Surveyor;var line=threat.Value.Snapshot;
            var telegraph=state.Phase==SurveyorBehaviorPhase.FireTelegraph && state.HasLatchedTarget;
            if(!line.HasActiveLine && !telegraph){_dodgeOrdinal=-1;return;}
            var center=line.HasActiveLine?line.LineCenterXQ4096/4096f:state.LatchedTargetXQ1000/1000f;
            var ordinal=line.HasActiveLine?line.LineShotOrdinal:state.ShotOrdinal;
            if(_dodgeOrdinal!=ordinal)
            {
                var left=float.PositiveInfinity;var right=float.NegativeInfinity;
                foreach(var floor in _floors){left=Mathf.Min(left,floor.xMin);right=Mathf.Max(right,floor.xMax);}
                var worldLeft=graph.TransformPoint(new Vector3(left,0,0)).x;var worldRight=graph.TransformPoint(new Vector3(right,0,0)).x;
                var clear=body.bounds.extents.x+.25f+.2f;
                foreach(var floor in _floors){var a=graph.TransformPoint(new Vector3(floor.xMin,0,0)).x+body.bounds.extents.x+.2f;var b=graph.TransformPoint(new Vector3(floor.xMax,0,0)).x-body.bounds.extents.x-.2f;if(x>=a && x<=b){worldLeft=a;worldRight=b;break;}}
                _dodgeSide=x<center-clear?-1:x>center+clear?1:center<(worldLeft+worldRight)/2?1:-1;
                if(_dodgeSide>0 && center+clear>worldRight)_dodgeSide=-1;else if(_dodgeSide<0 && center-clear<worldLeft)_dodgeSide=1;
                _dodgeOrdinal=ordinal;
                _record("denial-observation ordinal="+ordinal+" tick="+tick+" phase="+state.Phase+" active="+line.HasActiveLine+" centerX="+center+" side="+_dodgeSide+" avoidanceEffect=unobserved");
            }
            var distance=_dodgeSide*(x-center);var towardSpeed=Mathf.Max(0,-_dodgeSide*velocity);
            var margin=body.bounds.extents.x+.25f+.3f+towardSpeed*(Time.maximumDeltaTime+Time.fixedDeltaTime)+towardSpeed*towardSpeed/40f;
            var direction=keys.Contains(Key.D)?1:keys.Contains(Key.A)?-1:0;
            if(distance<margin || direction==-_dodgeSide)
            {
                keys.Remove(Key.A);keys.Remove(Key.D);
                if(distance<margin || towardSpeed>.1f)keys.Add(_dodgeSide>0?Key.D:Key.A);
            }
        }
        private bool TryTraverseGap(Transform graph,AcadeGameMaker.Movement.PlayerMotionSnapshot pose,bool gateLocked,float x,CapsuleCollider2D body,List<Key> requested,string targetId,float targetX,bool ownedReleased,long updates,out Command command)
        {
            command=default;
            if(body==null)throw new InvalidOperationException("틈 횡단의 실제 몸체가 없습니다.");
            var velocity=pose.VelocityXQ4096/4096f;var speed=Mathf.Abs(velocity);
            var half=Mathf.Abs(graph.InverseTransformVector(new Vector3(body.bounds.extents.x,0,0)).x);
            var stop=half+.2f+speed*(Time.maximumDeltaTime+Time.fixedDeltaTime)+speed*speed/40f;
            if(_gapPhase==0)
            {
                var direction=requested.Contains(Key.D)?1:requested.Contains(Key.A)?-1:0;
                if(direction==0)return false;
                for(var i=0;i+1<_floors.Length;i++)
                {
                    var from=direction>0?_floors[i]:_floors[i+1];var to=direction>0?_floors[i+1]:_floors[i];
                    var takeoff=direction>0?from.xMax:from.xMin;var far=direction>0?to.xMin:to.xMax;
                    var width=direction*(far-takeoff);var remaining=direction*(takeoff-x);
                    if(width<=0 || remaining<0 || remaining>Mathf.Max(6f,stop+1f))continue;
                    if(targetId.Length!=0 && direction*(targetX-far)<=half+.3f)
                    {
                        // Same-side combat retreat is not a traversal; preserve aim/attack/transfer.
                        if(remaining<=stop+1f)
                        {
                            requested.Remove(direction>0?Key.D:Key.A);
                            if(direction*velocity>.1f && !requested.Contains(direction>0?Key.A:Key.D))requested.Add(direction>0?Key.A:Key.D);
                        }
                        return false;
                    }
                    if(remaining<=stop || Mathf.Abs(from.yMax-to.yMax)>.01f || to.width<2*half+2f || width+2*half+3.6f>8f*(2f*14.39f/30.72f))
                        throw new InvalidOperationException("실제 제동/착지 여유가 없는 틈 진입입니다.");
                    _gapPhase=1;_gapDirection=direction;_gapTakeoff=takeoff;_gapFar=far;_gapLanding=far+direction*2f;
                    _gapStartedTick=pose.Tick.Value;_gapStartedSeconds=_watch.Elapsed.TotalSeconds;_gapReadyUpdate=-1;_gapLifted=false;_gapLastWaitTick=pose.Tick.Value;
                    _record("gap prepare tick="+pose.Tick.Value+" x="+x+" vx="+velocity+" takeoff="+takeoff+" far="+far+" stop="+stop+" updates="+updates);
                    break;
                }
                if(_gapPhase==0)return false;
            }
            if(pose.Tick.Value-_gapStartedTick>=300 || _watch.Elapsed.TotalSeconds-_gapStartedSeconds>=5)
                throw new InvalidOperationException("틈 횡단의 5초/300완료틱 상한입니다.");
            _jumpHold.Read(pose.Tick.Value,false,true);
            var keys=new List<Key>();
            if(_gapPhase==1)
            {
                var target=_gapTakeoff-_gapDirection*3.2f;
                var steering=SteerTo(target,x,velocity);
                if(steering!=0)keys.Add(steering>0?Key.D:Key.A);
                var ready=pose.IsGrounded && !gateLocked && pose.ActionState!=AcadeGameMaker.Movement.MovementActionState.InputLocked && speed<.5f && Mathf.Abs(x-target)<.5f && ownedReleased;
                if(_gapWaitLogged<10 && pose.Tick.Value-_gapLastWaitTick>=30){_gapWaitLogged++;_gapLastWaitTick=pose.Tick.Value;_record("gap waiting tick="+pose.Tick.Value+" x="+x+" y="+(pose.PositionYQ4096/4096f)+" vx="+velocity+" grounded="+pose.IsGrounded+" gateLocked="+gateLocked+" action="+pose.ActionState+" ownedReleased="+ownedReleased+" updates="+updates);}
                if(!ready)_gapReadyUpdate=-1;
                else if(_gapReadyUpdate<0)_gapReadyUpdate=updates;
                else if(updates>=_gapReadyUpdate+2)
                {
                    _gapPhase=2;_gapLaunchTick=pose.Tick.Value;keys.Clear();keys.Add(_gapDirection>0?Key.D:Key.A);keys.Add(Key.Space);
                    _record("gap jump-request tick="+pose.Tick.Value+" x="+x+" grounded="+pose.IsGrounded+" ownedReleased="+ownedReleased+" updates="+updates+" effect=unobserved");
                }
            }
            else
            {
                if(!_gapLifted && pose.VelocityYQ4096>4096)
                {_gapLifted=true;_record("gap rising tick="+pose.Tick.Value+" x="+x+" y="+(pose.PositionYQ4096/4096f)+" vy="+(pose.VelocityYQ4096/4096f));}
                if(!_gapLifted && pose.Tick.Value-_gapLaunchTick>=30)throw new InvalidOperationException("틈 점프 요청 뒤 실제 상승을 확인하지 못했습니다.");
                if(_gapPhase==2 && _gapDirection*(x-_gapFar)>half+.3f)
                {_gapPhase=3;_record("gap body-crossed tick="+pose.Tick.Value+" x="+x+" y="+(pose.PositionYQ4096/4096f));}
                var steering=_gapPhase==2?_gapDirection:SteerTo(_gapLanding,x,velocity);
                if(steering!=0)keys.Add(steering>0?Key.D:Key.A);
                if(_gapPhase==2 && pose.Tick.Value-_gapLaunchTick<24)keys.Add(Key.Space);
                if(_gapPhase==3 && _gapLifted && pose.IsGrounded && _gapDirection*(x-_gapFar)>half+.3f && speed<.5f)
                {_record("gap landed tick="+pose.Tick.Value+" x="+x+" grounded=True updates="+updates);_gapPhase=0;keys.Clear();}
            }
            command=new Command(keys.ToArray(),Vector2.zero,false,"",-1,"gap-owned-input-only");
            return true;
        }
        // AC-VERIFEXIT-002: the isolated regression calls the same selector as Read above.
        internal static bool TryReadExitApproach(Rect exit,float localX,float velocity,bool enemiesAlive,bool gapActive,out Command command)
        {
            command=default;
            if(enemiesAlive || gapActive || localX<exit.xMin-4f)return false;
            var direction=SteerTo(exit.center.x,localX,velocity);
            command=new Command(direction==0?Array.Empty<Key>():new[]{direction>0?Key.D:Key.A},Vector2.zero,false,"",-1,"exit-approach");
            return true;
        }
        internal static void ValidateCompletedTick(int tick,int firstTick)
        {if(tick-firstTick>=3000)throw new InvalidOperationException("시험 입력 사례의 3000 완료 틱 상한입니다.");}
        private static int SteerTo(float target,float x,float velocity)
        {
            var delta=target-x;
            if(Mathf.Abs(delta)<.2f && Mathf.Abs(velocity)<1.5f)return 0;
            var stopping=velocity*velocity/40f+Mathf.Abs(velocity)/60f+.08f;
            if(velocity*delta>0 && stopping>=Mathf.Abs(delta))return velocity>0?-1:1;
            if(Mathf.Abs(delta)<.2f)return velocity>0?-1:velocity<0?1:0;
            return delta>0?1:-1;
        }
        internal static int ApproachDirection(float dx)=>Math.Abs(dx)>4.8f?(dx>0?1:-1):Math.Abs(dx)<4f?(dx>0?-1:1):0;
        internal static bool ShouldAttack(float squaredDistance,double u,double v,int ordinal)=>squaredDistance<=30.25f&&u>=0&&u<=1&&v>=0&&v<=1&&ordinal%2==0;
        internal static bool NeedsStepJump(float ahead,float foot,float bottom,float top)=>ahead>=-.5f&&ahead<=1.5f&&bottom<=foot+.05f&&top>foot+2f/4096f;
        internal static bool NeedsGapJump(float ahead,float width)=>width>0&&ahead>=-.5f&&ahead<=1.5f;
        internal static bool IsLowCeiling(string roomId,float x)=>roomId=="R05"&&x>=9.5f&&x<=16.5f;
        internal static bool CrushWindowOpen(int tick,Func<int,bool> isClosed)
        {for(var future=0;future<=45;future++)if(isClosed(checked(tick+future)))return false;return true;}
        internal sealed class JumpHold
        {
            private int _start=-1,_releaseUntil=-1,_last=-1;
            internal bool Read(int tick,bool wantsJump,bool suppress)
            {
                if(tick<_last)throw new InvalidOperationException("점프 정책 완료 틱이 역행했습니다.");_last=tick;
                if(suppress){if(_start>=0)_releaseUntil=checked(tick+2);_start=-1;return false;}
                if(_start>=0){if(tick-_start<24)return true;_start=-1;_releaseUntil=checked(tick+2);return false;}
                if(tick<_releaseUntil)return false;
                if(wantsJump){_start=tick;return true;}return false;
            }
        }
    }

    // REQ-BVP-003/004. Pure selector: observations in, normal key intent out.
    internal sealed class Chapter1VerificationBossPolicy
    {
        internal readonly struct Command
        { internal Command(Key[] keys,bool attack){Keys=keys;Attack=attack;} internal Key[] Keys {get;} internal bool Attack {get;} }

        // REQ/AC-BAP-OWN-001: value-only identity; no producer state is changed.
        internal readonly struct OwnedJumpIdentity
        {
            internal OwnedJumpIdentity(object issuer,object lease,object router,object player,string source,string caseName,string ownerBirth,int ownerPid,long epoch,long cycle,long modeEpoch,bool ready)
            {Issuer=issuer;Lease=lease;Router=router;Player=player;Source=source;CaseName=caseName;OwnerBirth=ownerBirth;OwnerPid=ownerPid;Epoch=epoch;Cycle=cycle;ModeEpoch=modeEpoch;Ready=ready;}
            internal OwnedJumpIdentity(object issuer,Chapter1StandaloneVerificationSession session,object router,object player,long epoch,long modeEpoch,bool ready)
            {Issuer=issuer;Lease=session;Router=router;Player=player;Source=null;CaseName=null;OwnerBirth=null;OwnerPid=0;Epoch=epoch;Cycle=session==null?0:session.Id;ModeEpoch=modeEpoch;Ready=ready;}
            internal readonly object Issuer,Lease,Router,Player;internal readonly string Source,CaseName,OwnerBirth;internal readonly int OwnerPid;internal readonly long Epoch,Cycle,ModeEpoch;internal readonly bool Ready;
            internal bool Valid=>Ready && Issuer!=null && Lease!=null && Router!=null && Player!=null && Epoch>0 && Cycle>0 && ModeEpoch>=0 &&
                (Lease is Chapter1StandaloneVerificationSession standalone ? standalone.Active && ReferenceEquals(standalone.Pilot,Issuer) && (standalone.Mode=="branch1" || standalone.Mode=="branch2") && Source==null && CaseName==null && OwnerBirth==null && OwnerPid==0 : !string.IsNullOrEmpty(Source) && !string.IsNullOrEmpty(CaseName) && !string.IsNullOrEmpty(OwnerBirth) && OwnerPid>0);
            internal bool Same(OwnedJumpIdentity b)=>Valid && b.Valid && ReferenceEquals(Issuer,b.Issuer) && ReferenceEquals(Lease,b.Lease) && ReferenceEquals(Router,b.Router) && ReferenceEquals(Player,b.Player) && Source==b.Source && CaseName==b.CaseName && OwnerBirth==b.OwnerBirth && OwnerPid==b.OwnerPid && Epoch==b.Epoch && Cycle==b.Cycle && ModeEpoch==b.ModeEpoch;
        }
        private OwnedJumpIdentity _ownedIdentity,_tokenIdentity;private bool _ownedIdentityBound,_spaceObserved,_spaceHeld,_launchPending,_carryActive;private int _launchDirection,_launchTick,_launchPhaseStart,_carryUntil,_carryTick;private OrdanPatternSlot _launchSlot;private long _lastOwnedSequence=-1,_neutralSpaceSequence=-1,_launchSequence=-1,_carryReadSequence=-1;
        // REQ-BVP-003/004, AC-BVP-001/003: one bounded owned normal-input navigation attempt.
        // This is an observed gameplay experiment, not a future collision-safety certificate.
        private bool _approachAttempted,_approachActive,_approachClosed,_approachPress,_approachAirborne;
        private int _approachStart,_approachTick,_approachRecoveryStart;
        private long _approachReleaseSequence,_approachReadSequence;
        private OwnedJumpIdentity _approachIdentity;
        private void CloseApproach(string reason)
        {if(!_approachActive)return;_approachActive=false;_approachClosed=true;Unresolved="소유 Recovery 탐색 폐쇄: "+reason;}
        internal int RecoveryApproachState=>_approachClosed?3:_approachActive?1:_approachAttempted?2:0;
        internal bool RecoveryApproachPressObserved=>_approachPress;
        internal bool RecoveryApproachAirborneObserved=>_approachAirborne;
        internal bool OwnedContinuationActive=>_carryActive;
        internal int OwnedContinuationDeadline=>_carryUntil;
        internal void BindOwnedJump(OwnedJumpIdentity identity)
        {if(!_ownedIdentityBound || !_ownedIdentity.Same(identity)){CloseApproach("owner identity changed");InvalidateOwnedJump();_spaceObserved=false;_lastOwnedSequence=-1;_neutralSpaceSequence=-1;}_ownedIdentity=identity;_ownedIdentityBound=identity.Valid;}
        private void InvalidateOwnedJump(){_launchPending=false;_carryActive=false;_launchSequence=-1;}
        internal void ObserveStandaloneConsumption(object issuer,Chapter1StandaloneVerificationSession session,long epoch,Key[] keys,long sequence,bool ready)
        {ObserveOwnedConsumption(issuer,session,null,null,null,0,epoch,session==null?0:session.Id,keys,sequence,ready && session!=null && session.Active && ReferenceEquals(session.Pilot,issuer));}
        // Called only after the existing event ID/time/device/payload and state checks. No allocation, clock read or exception.
        internal void ObserveOwnedConsumption(object issuer,object lease,string source,string caseName,string ownerBirth,int ownerPid,long epoch,long cycle,Key[] keys,long sequence,bool ready)
        {
            if(!_ownedIdentityBound || !_ownedIdentity.Valid || !ReferenceEquals(issuer,_ownedIdentity.Issuer) || !ReferenceEquals(lease,_ownedIdentity.Lease) || source!=_ownedIdentity.Source || caseName!=_ownedIdentity.CaseName || ownerBirth!=_ownedIdentity.OwnerBirth || ownerPid!=_ownedIdentity.OwnerPid || epoch!=_ownedIdentity.Epoch || cycle!=_ownedIdentity.Cycle || !ready || keys==null || sequence<0){CloseApproach("owned consumption invalid");InvalidateOwnedJump();return;}
            if(sequence==_lastOwnedSequence)return;
            if(_lastOwnedSequence>=0 && sequence!=_lastOwnedSequence+1){CloseApproach("consumption sequence gap");InvalidateOwnedJump();_spaceObserved=false;}
            var space=false;var direction=0;var permitted=true;
            for(var i=0;i<keys.Length;i++){if(keys[i]==Key.Space)space=true;else if(keys[i]==Key.D){if(direction!=0)permitted=false;direction=1;}else if(keys[i]==Key.A){if(direction!=0)permitted=false;direction=-1;}else permitted=false;}
            if(!permitted){CloseApproach("unexpected owned keys");InvalidateOwnedJump();_spaceObserved=false;_lastOwnedSequence=sequence;return;}
            if(_approachActive)
            {
                if(!_approachPress && sequence==_approachReleaseSequence+1 && space && direction==1 && _spaceObserved && !_spaceHeld)_approachPress=true;
                else if(!_approachPress || !space || direction!=1)CloseApproach("missing single released-to-pressed D+Space consumption");
            }
            if(!space){InvalidateOwnedJump();_neutralSpaceSequence=sequence;}
            else if(_launchPending && _spaceObserved && !_spaceHeld && _neutralSpaceSequence>=0 && sequence==_neutralSpaceSequence+1 && direction==_launchDirection)
            {_carryActive=true;_launchPending=false;_launchSequence=sequence;_tokenIdentity=_ownedIdentity;_carryTick=_launchTick;}
            else if(!_spaceObserved || !_spaceHeld || !_carryActive || direction!=_launchDirection)InvalidateOwnedJump();
            _spaceHeld=space;_spaceObserved=true;_lastOwnedSequence=sequence;
        }
        private bool TryContinueOwnedJump(int tick,float x,float y,float vx,float vy,bool grounded,bool locked,OrdanPatternSlot slot,OrdanBossPhase phase,int age,float latency,Threat threat,out int direction)
        {
            direction=0;
            if(!_carryActive || !_ownedIdentityBound || !_ownedIdentity.Same(_tokenIdentity) || !_spaceObserved || !_spaceHeld || _launchSequence<0 || tick!=_carryTick+1 || (_carryTick!=_launchTick && _lastOwnedSequence<=_carryReadSequence) || tick>=_carryUntil || grounded || locked || !_focused || _resume || !float.IsFinite(x)||!float.IsFinite(y)||!float.IsFinite(vx)||!float.IsFinite(vy)||!float.IsFinite(latency)||latency<=0 || slot!=_launchSlot || phase!=OrdanBossPhase.Telegraph || age<1 || age>46 || tick-age!=_launchPhaseStart || threat.Stage<OrdanHealthStage.Stage1 || threat.Stage>OrdanHealthStage.Stage3 || threat.Queued || threat.Contact || threat.Pulling || threat.WallContact)
            {InvalidateOwnedJump();return false;}
            var low=x+Math.Min(vx,_launchDirection*8f)*latency-.02f;var high=x+Math.Max(vx,_launchDirection*8f)*latency+.02f;var half=OrdanBossHostileDamageProducer.ApprovedPlayerHalfWidth/4096f;
            if(low-half<=OrdanBossHostileDamageProducer.ApprovedArena.MinX/4096f || high+half>=OrdanBossHostileDamageProducer.ApprovedArena.MaxX/4096f){InvalidateOwnedJump();return false;}
            if(!OwnedDebtLineEnvelopeClear(x,y,vx,vy,_launchDirection,slot,age,latency)){InvalidateOwnedJump();return false;}
            _carryTick=tick;_carryReadSequence=_lastOwnedSequence;direction=_launchDirection;return true;
        }
        private string _run;private long _generation;private int _tick=-1,_pressTick=-1,_jumpStart=-1,_jumpUntil=-1;
        private bool _focused=true,_resume=true,_pressed,_released;private int _neutralUpdates;
        private Command _last=new Command(Array.Empty<Key>(),false);
        internal string Unresolved {get;private set;}
        internal string AttackState=>_pressed?"PressConsumed":_released?"Ready":"WaitRelease";
        internal void Focus(bool focused)
        {if(!focused){CloseApproach("focus lost");InvalidateOwnedJump();_spaceObserved=false;_resume=true;_neutralUpdates=0;_pressed=false;_released=false;_jumpStart=-1;}_focused=focused;}
        internal void Consumed(bool attack,bool neutral,long sequence)
        {
            if(!_focused)return;
            if(_resume){if(neutral)_neutralUpdates++;else _neutralUpdates=0;if(_neutralUpdates>=2)_resume=false;}
            if(attack)_pressed=true;else{_released=true;_pressed=false;}
        }
        internal Command Read(string run,long generation,int tick,float x,float y,float vx,bool grounded,bool locked,bool focused,bool aim,int cooldown,
            OrdanPatternSlot slot,OrdanBossPhase phase,int age,float latency,float vy=0,Threat? observedThreat=null)
        {
            if(string.IsNullOrEmpty(run)||generation<1||tick<0||float.IsNaN(x)||float.IsInfinity(x)||float.IsNaN(y)||float.IsInfinity(y)||float.IsNaN(vx)||float.IsInfinity(vx)||latency<=0||float.IsNaN(latency)||float.IsInfinity(latency))throw new InvalidOperationException("보스 정책 관측이 유한하지 않습니다.");
            if(_run==null){_run=run;_generation=generation;}
            if(run!=_run || generation!=_generation || tick<_tick)throw new InvalidOperationException("보스 정책 신원/tick 역행입니다.");
            Focus(focused);
            if(!focused || _resume){_last=new Command(Array.Empty<Key>(),false);return _last;}
            if(tick==_tick)return _last;
            if(_carryActive && tick!=_tick+1)InvalidateOwnedJump();
            _tick=tick;
            var keys=new List<Key>();
            var threat=observedThreat??new Threat(OrdanHealthStage.Stage1,false,false,false,false,false,28672,4915,410,0);
            if(_approachClosed){_last=new Command(Array.Empty<Key>(),false);return _last;}
            if(TryRecoveryApproach(tick,x,y,vx,vy,grounded,locked,slot,phase,age,threat,out var approachKeys))
            {_last=new Command(approachKeys,false);return _last;}

            if(_carryActive && (locked || grounded || threat.Queued || threat.Contact || threat.Pulling || threat.WallContact || !_ownedIdentityBound || !_ownedIdentity.Same(_tokenIdentity)))InvalidateOwnedJump();
            if(!TrySafeIntent(x,y,vx,vy,grounded,locked,slot,phase,age,latency,threat,out var direction,out var jump,out var reason))
            {if(reason!="모든 정상 이동/점프 후보의 예고 도달·벽·실제 패턴 안전을 입증할 수 없음" || !TryContinueOwnedJump(tick,x,y,vx,vy,grounded,locked,slot,phase,age,latency,threat,out direction)){InvalidateOwnedJump();Unresolved=reason;_last=new Command(Array.Empty<Key>(),false);return _last;}}
            if(_carryActive && tick!=_carryTick){if(tick==_carryTick+1 && tick<_carryUntil && phase==OrdanBossPhase.Telegraph && slot==_launchSlot && tick-age==_launchPhaseStart)_carryTick=tick;else InvalidateOwnedJump();}
            if(direction!=0)keys.Add(direction>0?Key.D:Key.A);
            if(jump && _jumpStart<0){_jumpStart=tick;_jumpUntil=checked(tick+Math.Max(24,48-age+2));
                if(_ownedIdentityBound && grounded && !locked && phase==OrdanBossPhase.Telegraph && age>=0 && age<=46 && (slot==OrdanPatternSlot.DebtLineA || slot==OrdanPatternSlot.DebtLineB) && _spaceObserved && !_spaceHeld && _neutralSpaceSequence==_lastOwnedSequence && !threat.Queued && !threat.Contact && !threat.Pulling)
                {_launchPending=true;_launchDirection=direction;_launchTick=tick;_launchPhaseStart=tick-age;_launchSlot=slot;_carryUntil=Math.Min(_jumpUntil,Math.Min(checked(_launchPhaseStart+48+2),checked(tick+50)));}}
            if(_jumpStart>=0 && tick<_jumpUntil)keys.Add(Key.Space);
            else if(_jumpStart>=0 && slot!=OrdanPatternSlot.DebtLineA && slot!=OrdanPatternSlot.DebtLineB)_jumpStart=-1;
            var attack=RightRegionClear((int)(x*4096),(int)(y*4096)) && aim && !locked && tick>=cooldown && _released && !_pressed && tick>_pressTick && x>=8.75f && x<=9.25f;
            if(attack){_released=false;_pressTick=tick;}
            _last=new Command(keys.ToArray(),attack);return _last;
        }
        // Start earlier than the blocking body face: observed Recovery age15 x3.06, vx8.
        // Source jump model suggests crossing the body at ~.35-.65s; actual pose/HP/contact decides the result.
        private bool TryRecoveryApproach(int tick,float x,float y,float vx,float vy,bool grounded,bool locked,OrdanPatternSlot slot,OrdanBossPhase phase,int age,Threat threat,out Key[] keys)
        {
            keys=Array.Empty<Key>();
            var finite=float.IsFinite(x)&&float.IsFinite(y)&&float.IsFinite(vx)&&float.IsFinite(vy);
            var clear=finite && !locked && !threat.Queued && !threat.Contact && !threat.Pulling && !threat.WallContact && !threat.Accelerated && threat.Stage==OrdanHealthStage.Stage1;
            if(!_approachActive)
            {
                if(_approachAttempted || !clear || !_ownedIdentityBound || !_ownedIdentity.Valid || !_spaceObserved || _spaceHeld || _neutralSpaceSequence!=_lastOwnedSequence ||
                    slot!=OrdanPatternSlot.DebtLineA || phase!=OrdanBossPhase.Recovery || age<0 || age>18 || !grounded || vy!=0 || x<3f || x>3.25f || vx<7.5f || vx>8.001f)return false;
                _approachAttempted=true;_approachActive=true;_approachStart=tick;_approachTick=tick;_approachRecoveryStart=tick-age;
                _approachReleaseSequence=_lastOwnedSequence;_approachReadSequence=_lastOwnedSequence;_approachIdentity=_ownedIdentity;
                InvalidateOwnedJump();_jumpStart=-1;_jumpUntil=-1;keys=new[]{Key.D,Key.Space};return true;
            }
            var samePhase=slot==OrdanPatternSlot.DebtLineA && phase==OrdanBossPhase.Recovery && tick-age==_approachRecoveryStart ||
                slot==OrdanPatternSlot.SeizureWeight && phase==OrdanBossPhase.Telegraph && tick-age==_approachRecoveryStart+39 && age>=0 && age<48;
            if(!clear || !_approachIdentity.Same(_ownedIdentity) || tick!=_approachTick+1 || tick>=_approachStart+60 || !samePhase || _lastOwnedSequence<=_approachReadSequence ||
                tick>_approachStart+2 && !_approachPress)
            {CloseApproach("identity/focus/threat/tick/phase/deadline or actual Space press missing");return true;}
            _approachTick=tick;_approachReadSequence=_lastOwnedSequence;
            if(!grounded && vy>0)_approachAirborne=true;
            if(tick>_approachStart+3 && !_approachAirborne){CloseApproach("actual rising pose not observed");return true;}
            var body=OrdanBossHostileDamageProducer.ApprovedOrdanBody;var half=OrdanBossHostileDamageProducer.ApprovedPlayerHalfWidth/4096f;var height=OrdanBossHostileDamageProducer.ApprovedPlayerHalfHeight/4096f;
            var arena=OrdanBossHostileDamageProducer.ApprovedArena;
            if(x-half<=arena.MinX/4096f || x+half>=arena.MaxX/4096f || y-height<arena.MinY/4096f || y+height>=arena.MaxY/4096f ||
                x+half>=body.MinX/4096f && x-half<=body.MaxX/4096f && y-height<=body.MaxY/4096f+.02f)
            {CloseApproach("actual arena/body clearance lost");return true;}
            if(_approachPress && _approachAirborne && x>body.MaxX/4096f+half+.05f)
            {_approachActive=false;_jumpStart=-1;_jumpUntil=-1;return true;} // one neutral release; original selector resumes next tick.
            keys=new[]{Key.D,Key.Space};return true;
        }
        internal readonly struct Threat
        {
            internal Threat(OrdanHealthStage stage,bool accelerated,bool queued,bool contact,bool occluded,bool pulling,int anchorX,int anchorY,int maximumStep,int seizureDuration,bool wallContact=false)
            {Stage=stage;Accelerated=accelerated;Queued=queued;Contact=contact;Occluded=occluded;Pulling=pulling;AnchorX=anchorX;AnchorY=anchorY;MaximumStep=maximumStep;SeizureDuration=seizureDuration;WallContact=wallContact;}
            internal OrdanHealthStage Stage {get;} internal bool Accelerated {get;} internal bool Queued {get;} internal bool Contact {get;} internal bool Occluded {get;} internal bool Pulling {get;} internal bool WallContact {get;}
            internal int AnchorX {get;} internal int AnchorY {get;} internal int MaximumStep {get;} internal int SeizureDuration {get;}
        }
        // Conservative certificates, not a replacement simulation: unproved alternatives fail neutral.
        internal static bool TrySafeIntent(float x,float y,float vx,float vy,bool grounded,bool locked,OrdanPatternSlot slot,OrdanBossPhase phase,int age,float latency,Threat threat,out int direction,out bool jump,out string reason)
        {
            direction=0;jump=false;reason=null;
            if(threat.Stage<OrdanHealthStage.Stage1 || threat.Stage>OrdanHealthStage.Stage3 || age<0 || !float.IsFinite(x)||!float.IsFinite(y)||!float.IsFinite(vx)||!float.IsFinite(vy) || latency<=0 || !float.IsFinite(latency))
            {reason="위험 관측 누락/비유한 또는 체력단계 부재";return false;}
            if(threat.Queued && threat.Contact && !threat.Occluded){reason="이미 배송 예약된 실제 접촉을 정상 입력으로 취소할 수 없음";return false;}
            if(threat.Pulling && (threat.AnchorX!=OrdanBossBalanceAuditPullProducer.ApprovedAnchorXQ4096 || threat.AnchorY!=OrdanBossBalanceAuditPullProducer.ApprovedAnchorYQ4096 || threat.MaximumStep!=OrdanBossBalanceAuditPullProducer.ApprovedMaximumStepQ4096))
            {reason="실제 당김 저작값이 승인 기하와 다름";return false;}
            var stop=vx*vx/40f+Math.Abs(vx)*latency+.08f;var delta=9-x;
            var preferred=Math.Abs(delta)<.2f && Math.Abs(vx)<.5f?0:delta>=0?1:-1;
            if(vx*delta>0 && stop>=Math.Abs(delta))preferred=vx>0?-1:1;
            foreach(var candidate in new[]{preferred,0,-preferred})
            {
                if(locked && candidate!=0)continue;
                var low=x+Math.Min(vx,candidate*8f)*latency-.02f;var high=x+Math.Max(vx,candidate*8f)*latency+.02f;
                var half=OrdanBossHostileDamageProducer.ApprovedPlayerHalfWidth/4096f;
                if(low-half<=OrdanBossHostileDamageProducer.ApprovedArena.MinX/4096f || high+half>=OrdanBossHostileDamageProducer.ApprovedArena.MaxX/4096f)continue;
                var debt=slot==OrdanPatternSlot.DebtLineA || slot==OrdanPatternSlot.DebtLineB;
                if(debt && (phase==OrdanBossPhase.Telegraph || phase==OrdanBossPhase.Execute && age==0))
                {
                    var seconds=phase==OrdanBossPhase.Telegraph?Math.Max(0,48-age)/60f:0;var safeX=OrdanBossHostileDamageProducer.ApprovedSourceX/4096f+half+.02f;
                    var reach=GuaranteedRightReach(x,vx,Math.Max(0,seconds-latency));
                    if(low>safeX || candidate==1 && reach>safeX){direction=candidate;return true;}
                    // Full jump interval at the fire tick must clear the closed horizontal line.
                    var line=slot==OrdanPatternSlot.DebtLineA?OrdanBossHostileDamageProducer.ApprovedSourceY/4096f:3f;
                    var upper=line+OrdanBossHostileDamageProducer.ApprovedPlayerHalfHeight/4096f+.25f+.02f;
                    var a=seconds-latency;var b=seconds;var minHeight=Math.Min(LowerBallisticHeight(y,14.39f,a),LowerBallisticHeight(y,14.39f,b));
                    if(!grounded && Math.Min(y,LowerBallisticHeight(y,vy,seconds))>upper){direction=candidate;return true;}
                    if(grounded && !locked && a>0 && b<=.8f && minHeight>upper){direction=candidate;jump=true;return true;}
                    continue;
                }
                if(slot==OrdanPatternSlot.SeizureWeight && (phase==OrdanBossPhase.Telegraph || phase==OrdanBossPhase.Execute))
                {
                    var index=(int)threat.Stage-1;
                    var duration=(threat.Accelerated?OrdanBossHostileDamageProducer.ApprovedAcceleratedDurations:OrdanBossHostileDamageProducer.ApprovedNormalDurations)[index];
                    if(phase==OrdanBossPhase.Execute && threat.SeizureDuration!=duration)continue;
                    var safeX=7f+half+OrdanBossHostileDamageProducer.ApprovedProjectileHalfExtent/4096f+.02f;
                    var remaining=Math.Max(0,60-age)/60f-latency;
                    if(low>safeX || phase==OrdanBossPhase.Telegraph && candidate==1 && GuaranteedRightReach(x,vx,Math.Max(0,remaining))>safeX){direction=candidate;return true;}
                    continue;
                }
                if(slot==OrdanPatternSlot.BalanceAudit && (phase==OrdanBossPhase.Telegraph || phase==OrdanBossPhase.Execute))
                {
                    // Approved pull is toward the exact shockwave centre and cannot increase radial distance.
                    var dx=Math.Max(Math.Abs(low-7),Math.Abs(high-7))+half;
                    var dy=Math.Abs(y-1.2f)+OrdanBossHostileDamageProducer.ApprovedPlayerHalfHeight/4096f+Math.Abs(vy)*latency+15.36f*latency*latency;
                    if(dx*dx+dy*dy>=144f)continue;
                    direction=candidate;return true;
                }
                direction=candidate;return true;
            }
            reason="모든 정상 이동/점프 후보의 예고 도달·벽·실제 패턴 안전을 입증할 수 없음";return false;
        }
        // REQ-BAP-CARRY-001 / AC-SOP-004. A certificate for this already-owned jump only, never a physics replay.
        // COM emits once at Execute age 0 (tick t); contact uses player pose t-1 and queues damage for t+1.
        // Check the exact prior-pose horizon plus two conservative samples; delivery is not another attack frame.
        internal static bool OwnedDebtLineEnvelopeClear(float x,float y,float vx,float vy,int direction,OrdanPatternSlot slot,int age,float latency)
        {
            if((slot!=OrdanPatternSlot.DebtLineA && slot!=OrdanPatternSlot.DebtLineB) || age<1 || age>46 || direction<-1 || direction>1 ||
                !float.IsFinite(x) || !float.IsFinite(y) || !float.IsFinite(vx) || !float.IsFinite(vy) || !float.IsFinite(latency) || latency<=0 ||
                Math.Abs(vx)>8.001f || vy>14.391f || vy< -22.001f)return false;
            var halfWidth=OrdanBossHostileDamageProducer.ApprovedPlayerHalfWidth/4096f;
            var halfHeight=OrdanBossHostileDamageProducer.ApprovedPlayerHalfHeight/4096f;
            var arena=OrdanBossHostileDamageProducer.ApprovedArena;
            var floor=arena.MinY/4096f+halfHeight;
            if(y<floor-.03f)return false;
            var fireInTicks=checked(48-age);var seconds=fireInTicks/60f;
            var first=checked(fireInTicks-1)/60f;var last=checked(fireInTicks+1)/60f;
            var line=slot==OrdanPatternSlot.DebtLineA?OrdanBossHostileDamageProducer.ApprovedDebtLineA:OrdanBossHostileDamageProducer.ApprovedDebtLineB;
            var clearance=line.MaxY/4096f+halfHeight+.02f;
            if(Math.Min(LowerBallisticHeight(y,vy,first),LowerBallisticHeight(y,vy,last))<=clearance)return false;
            // Round below the slowest authored gravity (Q4096 * .65) and fall cap for an upper flight bound.
            // Holding the original Space avoids jump-cut; any release, pull, lock or contact revokes the owner before here.
            const float slowGravity=19.96f;const float slowFall=15.39f;
            var upperVelocity=Math.Max(vy,-slowFall);
            var apex=y+Math.Max(0,upperVelocity)*Math.Max(0,upperVelocity)/(2*slowGravity)+.03f;
            if(apex+halfHeight+.02f>=arena.MaxY/4096f)return false;
            var height=Math.Max(0,y-floor+.03f);
            var terminalTime=Math.Max(0,(upperVelocity+slowFall)/slowGravity);
            var landing=(upperVelocity+(float)Math.Sqrt(upperVelocity*upperVelocity+2*slowGravity*height))/slowGravity;
            if(landing>terminalTime)
            {var terminalHeight=height+upperVelocity*terminalTime-.5f*slowGravity*terminalTime*terminalTime;landing=terminalTime+Math.Max(0,terminalHeight)/slowFall;}
            // Fastest Stage3 Debt Execute18 + Recovery33 precede the unscaled next Telegraph60.
            // Land before its start; earliest next hostile sample is Execute tick minus one, not damage delivery.
            if(!float.IsFinite(landing) || landing>=seconds+51f/60f || landing+latency>=seconds+110f/60f)return false;
            var low=x+Math.Min(vx,direction*8f)*landing-.02f;
            var high=x+Math.Max(vx,direction*8f)*landing+.02f;
            if(low-halfWidth<=arena.MinX/4096f || high+halfWidth>=arena.MaxX/4096f)return false;
            // This corridor remains wholly left of the authored boss body through landing.
            // General right-region candidates are handled first by the unchanged ordinary selector.
            if(high+halfWidth>=OrdanBossHostileDamageProducer.ApprovedOrdanBody.MinX/4096f)return false;
            return true;
        }
        internal static float LowerBallisticHeight(float y,float velocity,float seconds)=>y+velocity*seconds-15.36f*seconds*seconds-.256f*seconds-.03f;
        internal static float GuaranteedRightReach(float x,float vx,float seconds)
        {
            // Lower 20 units/s² ordinary acceleration/deceleration; retain adverse actual momentum.
            var v=Math.Min(8,vx);var ramp=Math.Min(seconds,Math.Max(0,(8-v)/20f));
            return x+v*ramp+10*ramp*ramp+8*Math.Max(0,seconds-ramp)-.02f;
        }
        internal static bool RightRegionClear(int x,int y)
        {
            var half=OrdanBossHostileDamageProducer.ApprovedPlayerHalfWidth;
            var maxX=0;foreach(var item in OrdanBossHostileDamageProducer.ApprovedSeizureStartXs)maxX=Math.Max(maxX,item);
            var dx=Math.Abs((long)x-OrdanBossHostileDamageProducer.ApprovedAuditCenterX)+half;
            var dy=Math.Abs((long)y-OrdanBossHostileDamageProducer.ApprovedAuditCenterY)+OrdanBossHostileDamageProducer.ApprovedPlayerHalfHeight;
            return x-half>OrdanBossHostileDamageProducer.ApprovedSourceX && x-half-OrdanBossHostileDamageProducer.ApprovedProjectileHalfExtent>maxX &&
                x+half<OrdanBossHostileDamageProducer.ApprovedArena.MaxX && dx*dx+dy*dy<(long)OrdanBossHostileDamageProducer.ApprovedAuditInnerRadius*OrdanBossHostileDamageProducer.ApprovedAuditInnerRadius;
        }
        internal static string FormatTrace(DamageRequest request,DamageResult result,int completedTick)
        {
            ValidateTrace(request.RequestId,result.RequestId,request.Tick.Value,result.Tick.Value,completedTick,request.TargetId,result.TargetId);
            return " request="+request.RequestId+" source="+request.SourceId+" kind="+request.Kind+" target="+request.TargetId+" requested="+request.Amount+" deliveryTick="+request.Tick.Value+" resultTick="+result.Tick.Value+" result="+result.Result+" applied="+result.AppliedAmount;
        }
        internal static void ValidateTrace(string request,string result,int sourceTick,int resultTick,int completedTick,string target,string resultTarget)
        {if(request!=result || sourceTick!=completedTick || resultTick!=completedTick || target!=resultTarget)throw new InvalidOperationException("보스 피해 요청/결과 결속 불일치입니다.");}
    }
    [Serializable] internal sealed class Chapter1ReceiptObservation
    {
        public long ClockBeforeBits,ClockAfterBits;public string ClockCoordinate;public string EventType,QueueBeforeTimeRoundTrip,QueueBeforeNowRoundTrip,QueueAfterTimeRoundTrip,QueueAfterNowRoundTrip;public long QueueBeforeTimeBits,QueueBeforeNowBits,QueueAfterTimeBits,QueueAfterNowBits;public string Stage,UpdateType,ExpectedTimeRoundTrip,ComparedTimeRoundTrip,DiagnosticTimeRoundTrip,DiagnosticNowRoundTrip,CaseName,RunId,Phase,SourceSha,OwnerBirth;
        public long Ordinal,Sequence,Cycle,UpdateCount,ExpectedTimeBits,ComparedTimeBits,DiagnosticTimeBits,DiagnosticNowBits;
        public int ExpectedId,ComparedId,ExpectedDeviceId,ComparedEventDeviceId,ComparedDeviceId,EvaluatedMask,MismatchMask,OwnerPid;
        public bool KeyboardSeen,MouseSeen,IdEvaluated,TimeEvaluated,DeviceEvaluated;
    }
    internal sealed class Chapter1VerificationBossBudget
    {
        [Serializable] private sealed class DetailEnvelope {public string detail;}
        // REQ/AC-RTIME-003: shared fixture evidence reserves three terminal rows inside the unchanged total cap.
        private const int CleanupRows=3,CleanupBytes=CleanupRows*8192;
        private int _rows,_bytes,_cleanupRows,_cleanupBytes;private readonly int _maximumBytes;private readonly bool _reserveCleanup;
        internal Chapter1VerificationBossBudget(int maximumBytes=128*1024,bool reserveCleanup=false)
        {if(maximumBytes<1||maximumBytes>128*1024||reserveCleanup&&maximumBytes<CleanupBytes)throw new ArgumentOutOfRangeException(nameof(maximumBytes));_maximumBytes=maximumBytes;_reserveCleanup=reserveCleanup;}
        internal static int SerializedUpperBound(string row)
        {
            // Same JsonUtility escaping as Driver.Record; 512 covers its fixed UTC/kind/double/frame wrapper.
            var escaped=JsonUtility.ToJson(new DetailEnvelope{detail="boss "+row});
            return checked(System.Text.Encoding.UTF8.GetByteCount(escaped)+1+512);
        }
        internal void Add(string row)=>Add(row,false);
        internal void AddCleanup(string row)=>Add(row,true);
        private void Add(string row,bool cleanup)
        {
            var bytes=SerializedUpperBound(row);
            var quotaExceeded=cleanup?(!_reserveCleanup || _cleanupRows+1>CleanupRows || _cleanupBytes+bytes>CleanupBytes):
                _rows-_cleanupRows+1>256-(_reserveCleanup?CleanupRows:0) || _bytes-_cleanupBytes+bytes>_maximumBytes-(_reserveCleanup?CleanupBytes:0);
            if(bytes>8192 || _rows+1>256 || _bytes+bytes>_maximumBytes || quotaExceeded)
                throw new InvalidOperationException("보스 실제 JSON 상계 원시 한도입니다. cleanup="+cleanup+" rows="+_rows+" bytes="+_bytes+" next="+bytes+" maximum="+_maximumBytes);
            _rows++;_bytes+=bytes;if(cleanup){_cleanupRows++;_cleanupBytes+=bytes;}
        }

    }

}




