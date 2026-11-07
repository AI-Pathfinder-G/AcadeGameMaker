using System;
using System.Runtime.CompilerServices;
using System.Threading;
using AcadeGameMaker.Input.Unity;
using UnityEngine;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    internal enum NewGameConfirmationOwnerStateV1 { AwaitingRequest=1, Inspecting, AwaitingCaptureRetry, AwaitingDecision, FreshDecisionRequired, ConfirmedReady, Cancelled, Rearming, ExecutionCommitted, TerminalFailure, Closed, ExecutionFreshPreparing=12 }
    internal enum NewGameConfirmationOutcomeV1 { DecisionRequired=1, Confirmed, Cancelled, FreshDecisionRequired, Busy, CaptureUnreadable }
    internal sealed class NewGameDecisionCapabilityV1 { internal NewGameDecisionCapabilityV1() { } }
    internal sealed class NewGameCaptureRetryCapabilityV1 { internal NewGameCaptureRetryCapabilityV1() { } }
    internal sealed class HubNewGameRearmCapabilityV1 { internal HubNewGameRearmCapabilityV1() { } }
    internal sealed class HubNewGameExecutionFreshCapabilityV1 { internal HubNewGameExecutionFreshCapabilityV1() { } }
    internal sealed class HubNewGameExecutionFreshReservationV1 { internal HubNewGameExecutionFreshReservationV1() { } }
    internal sealed class NewGameConfirmationStartResultV1
    {
        private readonly NewGameConfirmationOutcomeV1 _outcome;
        private readonly NewGameObservationClassificationV1? _classification;
        private readonly NewGameDecisionCapabilityV1 _decision;
        private readonly NewGameCaptureRetryCapabilityV1 _retry;
        private readonly ConfirmedProfileResetRequestV1 _confirmed;
        private readonly HubNewGameRearmCapabilityV1 _rearm;
        internal NewGameConfirmationStartResultV1(NewGameConfirmationOutcomeV1 outcome,NewGameObservationClassificationV1? classification,NewGameDecisionCapabilityV1 decision,NewGameCaptureRetryCapabilityV1 retry,ConfirmedProfileResetRequestV1 confirmed,HubNewGameRearmCapabilityV1 rearm)
        { _outcome=outcome; _classification=classification; _decision=decision; _retry=retry; _confirmed=confirmed; _rearm=rearm; }
        internal NewGameConfirmationOutcomeV1 Outcome { get { Validate(); return _outcome; } }
        internal NewGameObservationClassificationV1 Classification { get { Validate(); if (!_classification.HasValue) throw new InvalidOperationException("No decision classification."); return _classification.Value; } }
        internal NewGameDecisionCapabilityV1 DecisionCapability { get { Validate(); if (_decision == null) throw new InvalidOperationException("No decision capability."); return _decision; } }
        internal NewGameCaptureRetryCapabilityV1 RetryCapability { get { Validate(); if (_retry == null) throw new InvalidOperationException("No intake retry capability."); return _retry; } }
        internal ConfirmedProfileResetRequestV1 ConfirmedRequest { get { Validate(); if (_confirmed == null) throw new InvalidOperationException("No confirmed request."); return _confirmed; } }
        internal HubNewGameRearmCapabilityV1 RearmCapability { get { Validate(); if (_rearm == null) throw new InvalidOperationException("No rearm capability."); return _rearm; } }
        internal string Prompt { get { Validate(); if (_outcome != NewGameConfirmationOutcomeV1.DecisionRequired) throw new InvalidOperationException("No confirmation prompt."); return NewGameConfirmationOwnerV1.ConfirmationPrompt; } }
        internal string ConfirmLabel { get { var ignored=Prompt; return "초기화하고 새 게임"; } }
        internal string CancelLabel { get { var ignored=Prompt; return "취소"; } }
        internal bool Matches(NewGameConfirmationStartResultV1 other)
        { return other != null && _outcome == other._outcome && _classification == other._classification && ReferenceEquals(_decision,other._decision) && ReferenceEquals(_retry,other._retry) && ReferenceEquals(_confirmed,other._confirmed) && ReferenceEquals(_rearm,other._rearm); }
        private void Validate() { if (!NewGameConfirmationOwnerV1.IsIssuedResult(this)) throw new InvalidOperationException("Confirmation result is not an issued row."); }
    }

    // REQ-M5D7QC3-001..007: synthetic ownership only. This component never
    // extracts an identity, invokes disk reset, cuts over memory or enables a map.
    [DisallowMultipleComponent]
    internal sealed class NewGameConfirmationOwnerV1 : MonoBehaviour
    {
        internal const string ConfirmationPrompt="새 게임을 시작하면 현재 단일 프로필의 진행 상황, 설정, 입력 설정, 튜토리얼 확인 상태가 초기화됩니다. 기존 프로필 파일은 자동 복구에 사용되지 않는 수동 복구용 자료로 계속 보관됩니다. 의상 및 외형 자료는 별도 저장 영역이므로 변경되지 않습니다. 계속하시겠습니까?";
        private HubMenuPresenterV1 _presenter;
        private HubMenuIntentHandoffOwnerV1 _handoffOwner;
        private DesktopProfileLaunchAdapterV1 _adapter;
        private InputRouter _router;
        private object _epochToken=new object(), _epochTokenProof;
        private long _epoch=1, _epochProof=1, _generation;
        private NewGameConfirmationOwnerStateV1 _state=NewGameConfirmationOwnerStateV1.AwaitingRequest, _stateProof=NewGameConfirmationOwnerStateV1.AwaitingRequest;
        private IssuedNewGameRequestV1 _pendingIssued, _acceptedIssued;
        private HubMenuIntentRequestV1? _acceptedRequest;
        private ProfileNewGameCaptureResultV1 _display, _fresh;
        private NewGameDecisionCapabilityV1 _decision;
        private NewGameCaptureRetryCapabilityV1 _retry;
        private ConfirmedProfileResetRequestV1 _confirmed;
        private HubNewGameRearmCapabilityV1 _rearm;
        private EpochHistory _history;
        private int _operation;
        private bool _configured, _peerClosing;
        private string _faultBoundary;
        private ProfileResetExecutionResultV1 _executionResult;
        private CommitHistory _commitHistory;
        private FreshOperation _executionFresh;
        private ExecutionFreshHistory _executionFreshHistory;
        private sealed class FreshOperation
        {
            internal readonly NewGameConfirmationOwnerV1 Owner;
            internal readonly CohortWitness Cohort;
            internal readonly CommitHistory Original;
            internal readonly ProfileResetExecutionResultV1 Result;
            internal readonly Thread Thread=Thread.CurrentThread;
            internal readonly HubNewGameExecutionFreshCapabilityV1 Capability=new HubNewGameExecutionFreshCapabilityV1();
            internal readonly object Token=new object();
            internal readonly long Epoch;
            internal ProfileResetFreshExecutionReservationV1 LowerReservation;
            internal HubNewGameExecutionFreshReservationV1 UpperReservation;
            internal int Consumed;
            internal FreshOperation(NewGameConfirmationOwnerV1 owner,CohortWitness cohort,CommitHistory original,ProfileResetExecutionResultV1 result)
            { Owner=owner; Cohort=cohort; Original=original; Result=result; Epoch=checked(original.Epoch+1); }
        }
        private sealed class ExecutionFreshHistory
        {
            internal readonly ExecutionFreshHistory Previous;
            internal readonly FreshOperation Operation;
            internal readonly ProfileResetFreshExecutionReservationV1 LowerReservation;
            internal readonly HubNewGameExecutionFreshReservationV1 UpperReservation;
            internal ExecutionFreshHistory(ExecutionFreshHistory previous,FreshOperation operation)
            { Previous=previous; Operation=operation; LowerReservation=operation.LowerReservation; UpperReservation=operation.UpperReservation; }
        }
        private static readonly ConditionalWeakTable<HubNewGameExecutionFreshCapabilityV1,FreshOperation> ExecutionFreshCapabilities=new ConditionalWeakTable<HubNewGameExecutionFreshCapabilityV1,FreshOperation>();
        // REQ-M5D7QC4-004: 원래 commit 참조는 콜백 권한 폐쇄 전에 보관한다.
        private sealed class CommitHistory
        {
            internal readonly CommitHistory Previous;
            internal readonly object Token;
            internal readonly long Epoch, Generation;
            internal readonly IssuedNewGameRequestV1 Pending, Issued;
            internal readonly HubMenuIntentRequestV1? Request;
            internal readonly ProfileNewGameCaptureResultV1 Display, Fresh;
            internal readonly NewGameDecisionCapabilityV1 Decision;
            internal readonly NewGameCaptureRetryCapabilityV1 Retry;
            internal readonly HubNewGameRearmCapabilityV1 Rearm;
            internal readonly ConfirmedProfileResetRequestV1 Confirmed;
            internal readonly ProfileNewGameExecutionCommitReservationV1 Reservation;
            internal CommitHistory(NewGameConfirmationOwnerV1 owner,ProfileNewGameExecutionCommitReservationV1 reservation)
            {
                Previous=owner._commitHistory; Token=owner._epochToken; Epoch=owner._epoch;
                Generation=owner._generation; Pending=owner._pendingIssued; Issued=owner._acceptedIssued;
                Request=owner._acceptedRequest; Display=owner._display; Fresh=owner._fresh;
                Decision=owner._decision; Retry=owner._retry; Rearm=owner._rearm;
                Confirmed=owner._confirmed; Reservation=reservation;
            }
        }
        private sealed class CohortWitness
        {
            internal readonly HubMenuPresenterV1 Presenter;
            internal readonly HubMenuIntentHandoffOwnerV1 HandoffOwner;
            internal readonly DesktopProfileLaunchAdapterV1 Adapter;
            internal readonly InputRouter Router;
            internal CohortWitness(HubMenuPresenterV1 presenter,HubMenuIntentHandoffOwnerV1 handoffOwner,DesktopProfileLaunchAdapterV1 adapter,InputRouter router)
            { Presenter=presenter; HandoffOwner=handoffOwner; Adapter=adapter; Router=router; }
        }
        private sealed class EpochHistory
        {
            internal readonly ExecutionFreshHistory ExecutionFresh;
            internal readonly EpochHistory Previous;
            internal readonly object Token;
            internal readonly long Epoch;
            internal readonly IssuedNewGameRequestV1 Issued;
            internal readonly HubMenuIntentRequestV1 Request;
            internal readonly HubNewGameRearmCapabilityV1 Rearm;
            internal EpochHistory(EpochHistory previous,object token,long epoch,IssuedNewGameRequestV1 issued,HubMenuIntentRequestV1 request,HubNewGameRearmCapabilityV1 rearm)
            { Previous=previous; Token=token; Epoch=epoch; Issued=issued; Request=request; Rearm=rearm; }
            internal EpochHistory(EpochHistory previous,ExecutionFreshHistory executionFresh)
            { Previous=previous; ExecutionFresh=executionFresh; var original=executionFresh.Operation.Original;
              Token=original.Token; Epoch=original.Epoch; Issued=original.Issued; Request=original.Request.Value; Rearm=original.Rearm; }
        }
        private sealed class DecisionWitness
        {
            internal readonly NewGameConfirmationOwnerV1 Owner;
            internal readonly object Token;
            internal readonly long Epoch, Generation;
            internal readonly ProfileNewGameCaptureResultV1 Display;
            internal readonly IssuedNewGameRequestV1 Issued;
            internal int State;
            internal bool Used;
            internal DecisionWitness(NewGameConfirmationOwnerV1 owner,object token,long epoch,long generation,ProfileNewGameCaptureResultV1 display,IssuedNewGameRequestV1 issued)
            { Owner=owner; Token=token; Epoch=epoch; Generation=generation; Display=display; Issued=issued; }
        }
        private sealed class RetryWitness
        {
            internal readonly NewGameConfirmationOwnerV1 Owner;
            internal readonly object Token;
            internal readonly long Epoch;
            internal readonly IssuedNewGameRequestV1 Issued;
            internal int Inspecting;
            internal bool Closed;
            internal RetryWitness(NewGameConfirmationOwnerV1 owner,object token,long epoch,IssuedNewGameRequestV1 issued)
            { Owner=owner; Token=token; Epoch=epoch; Issued=issued; }
        }
        private sealed class RearmWitness
        {
            internal readonly NewGameConfirmationOwnerV1 Owner;
            internal readonly HubMenuIntentHandoffOwnerV1 HandoffOwner;
            internal readonly HubMenuPresenterV1 Presenter;
            internal readonly InputRouter Router;
            internal readonly object Token;
            internal readonly long Epoch;
            internal int Consumed;
            internal bool Used;
            internal RearmWitness(NewGameConfirmationOwnerV1 owner,HubMenuIntentHandoffOwnerV1 handoffOwner,HubMenuPresenterV1 presenter,InputRouter router,object token,long epoch)
            { Owner=owner; HandoffOwner=handoffOwner; Presenter=presenter; Router=router; Token=token; Epoch=epoch; }
        }
        private static readonly ConditionalWeakTable<NewGameDecisionCapabilityV1,DecisionWitness> Decisions=new ConditionalWeakTable<NewGameDecisionCapabilityV1,DecisionWitness>();
        private static readonly ConditionalWeakTable<NewGameCaptureRetryCapabilityV1,RetryWitness> Retries=new ConditionalWeakTable<NewGameCaptureRetryCapabilityV1,RetryWitness>();
        private static readonly ConditionalWeakTable<HubNewGameRearmCapabilityV1,RearmWitness> Rearms=new ConditionalWeakTable<HubNewGameRearmCapabilityV1,RearmWitness>();
        private static readonly ConditionalWeakTable<NewGameConfirmationStartResultV1,NewGameConfirmationStartResultV1> Results=new ConditionalWeakTable<NewGameConfirmationStartResultV1,NewGameConfirmationStartResultV1>();
        private static readonly ConditionalWeakTable<NewGameConfirmationOwnerV1,CohortWitness> Cohorts=new ConditionalWeakTable<NewGameConfirmationOwnerV1,CohortWitness>();
        internal static bool IsIssuedResult(NewGameConfirmationStartResultV1 result) { return result != null && Results.TryGetValue(result,out var proof) && result.Matches(proof); }
        internal NewGameConfirmationOwnerStateV1 State { get { Validate(); return _state; } }
        internal void ConfigureForAuthoring(HubMenuPresenterV1 presenter,HubMenuIntentHandoffOwnerV1 handoffOwner,DesktopProfileLaunchAdapterV1 adapter,InputRouter router)
        {
            if (_configured || presenter == null || handoffOwner == null || adapter == null || router == null || gameObject.activeInHierarchy || !ReferenceEquals(presenter.gameObject,gameObject) || !ReferenceEquals(handoffOwner.gameObject,gameObject) || !ReferenceEquals(presenter.HandoffOwner,handoffOwner) || !ReferenceEquals(handoffOwner.Presenter,presenter)) throw new InvalidOperationException("Confirmation authoring requires the exact inactive cohort.");
            _presenter=presenter; _handoffOwner=handoffOwner; _adapter=adapter; _router=router; _epochTokenProof=_epochToken;
            Cohorts.Add(this,new CohortWitness(presenter,handoffOwner,adapter,router));
            presenter.BindConfirmationOwner(this); handoffOwner.BindConfirmationOwner(this,_epochToken); _configured=true;
        }
        internal bool MatchesLaunchAdapter(DesktopProfileLaunchAdapterV1 adapter) { return _configured && ReferenceEquals(_adapter,adapter); }
        internal bool MatchesCohort(HubMenuIntentHandoffOwnerV1 handoffOwner,HubMenuPresenterV1 presenter,InputRouter router)
        { return _configured && ReferenceEquals(_handoffOwner,handoffOwner) && ReferenceEquals(_presenter,presenter) && ReferenceEquals(_router,router); }
        internal bool CanBindIssuance(object token,long epoch,bool hasHistory)
        {
            Validate();
            return _state == NewGameConfirmationOwnerStateV1.AwaitingRequest && ReferenceEquals(token,_epochToken) && epoch == _epoch && _pendingIssued == null && _acceptedIssued == null && !_acceptedRequest.HasValue && _display == null && _fresh == null && _decision == null && _retry == null && _confirmed == null && _rearm == null && (epoch == 1 ? !hasHistory && _history == null : hasHistory && _history != null && _history.Epoch == epoch-1 && !ReferenceEquals(_history.Token,token));
        }
        internal void BindIssued(IssuedNewGameRequestV1 issued,object token,long epoch)
        { if (issued == null || !CanBindIssuance(token,epoch,_history != null)) throw new InvalidOperationException("Confirmation pending slot is not pristine."); _pendingIssued=issued; }
        internal bool HasPendingIssued(IssuedNewGameRequestV1 issued,object token,long epoch)
        { return issued != null && _configured && _state == NewGameConfirmationOwnerStateV1.AwaitingRequest && _state == _stateProof && ReferenceEquals(_pendingIssued,issued) && ReferenceEquals(token,_epochToken) && epoch == _epoch; }
        internal void MarkIssuedConsumed(IssuedNewGameRequestV1 issued,object token,long epoch)
        { if (!HasPendingIssued(issued,token,epoch)) throw new InvalidOperationException("Confirmation issued binding is foreign."); _acceptedIssued=issued; _pendingIssued=null; }
        internal NewGameConfirmationStartResultV1 AcceptNewGame(IssuedNewGameRequestV1 issued,DesktopProfileLaunchAdapterV1 adapter,InputRouter router)
        {
            if (!ReferenceEquals(adapter,_adapter) || !ReferenceEquals(router,_router) || _handoffOwner == null || IsNormallyEnded() || _handoffOwner.ClassifyNewGameIssuance(issued,this,_presenter,router) != 1) throw new InvalidOperationException("Confirmation intake is foreign or completed.");
            Enter();
            try
            {
                if (IsNormallyEnded() || _handoffOwner.ClassifyNewGameIssuance(issued,this,_presenter,router) != 1) throw new InvalidOperationException("Confirmation intake completed before entry.");
                try
                {
                Validate();
                if (!HasPendingIssued(issued,_epochToken,_epoch)) throw new InvalidOperationException("Actual intake pending evidence is inconsistent.");
                var row=_handoffOwner.ConsumeNewGameIssuance(issued,this,_presenter,router);
                if (row.Item != HubMenuItemV1.NewGame || !_presenter.MatchesImmutableHandoff(row.Receipt)) throw new InvalidOperationException("Confirmation issuance data is inconsistent.");
                _acceptedRequest=row; SetState(NewGameConfirmationOwnerStateV1.Inspecting);
                return InspectInitial();
                }
                catch { Terminal(); throw; }
            }
            finally { Exit(); }
        }
        internal NewGameConfirmationStartResultV1 RetryIntake(NewGameCaptureRetryCapabilityV1 capability)
        {
            if (capability == null || !Retries.TryGetValue(capability,out var witness) || !ReferenceEquals(witness.Owner,this) || witness.Closed) throw new InvalidOperationException("Confirmation retry is foreign or completed.");
            Enter();
            var inspecting=false;
            try
            {
                if (!Retries.TryGetValue(capability,out var current) || !ReferenceEquals(current,witness) || witness.Closed) throw new InvalidOperationException("Confirmation retry completed before entry.");
                try
                {
                Validate();
                if (!ReferenceEquals(_retry,capability) || !ReferenceEquals(witness.Token,_epochToken) || witness.Epoch != _epoch || !ReferenceEquals(witness.Issued,_acceptedIssued) || _state != NewGameConfirmationOwnerStateV1.AwaitingCaptureRetry) throw new InvalidOperationException("Actual retry evidence is inconsistent.");
                if (Interlocked.CompareExchange(ref witness.Inspecting,1,0) != 0) throw new InvalidOperationException("Confirmation retry is already inspecting.");
                inspecting=true; SetState(NewGameConfirmationOwnerStateV1.Inspecting);
                var result=InspectInitial();
                if (result.Outcome != NewGameConfirmationOutcomeV1.Busy) { witness.Closed=true; _retry=null; }
                return result;
                }
                catch { Terminal(); throw; }
            }
            finally { if (inspecting) Volatile.Write(ref witness.Inspecting,0); Exit(); }
        }
        private NewGameConfirmationStartResultV1 InspectInitial()
        {
            CheckFault("BeforeCapture");
            var capture=ProfileNewGameConfirmationV1.Capture(_adapter,_router,this,_epochToken);
            if (capture.Outcome == ProfileNewGameCaptureOutcomeV1.Busy)
            {
                if (_retry == null) { _retry=new NewGameCaptureRetryCapabilityV1(); Retries.Add(_retry,new RetryWitness(this,_epochToken,_epoch,_acceptedIssued)); }
                SetState(NewGameConfirmationOwnerStateV1.AwaitingCaptureRetry);
                return Result(NewGameConfirmationOutcomeV1.Busy,retry:_retry);
            }
            if (capture.Outcome == ProfileNewGameCaptureOutcomeV1.Unreadable) { Terminal(); return Result(NewGameConfirmationOutcomeV1.CaptureUnreadable); }
            _generation=checked(_generation+1); _display=capture;
            if (capture.Classification == NewGameObservationClassificationV1.NoConfirmationRequired)
            {
                _confirmed=ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,_adapter,_router,this,_epochToken,_epoch,_generation).ConfirmedRequest;
                SetState(NewGameConfirmationOwnerStateV1.ConfirmedReady);
                return Result(NewGameConfirmationOutcomeV1.Confirmed,capture.Classification,confirmed:_confirmed);
            }
            return PublishDecision(capture);
        }
        private NewGameConfirmationStartResultV1 PublishDecision(ProfileNewGameCaptureResultV1 capture)
        {
            _decision=new NewGameDecisionCapabilityV1();
            Decisions.Add(_decision,new DecisionWitness(this,_epochToken,_epoch,_generation,capture,_acceptedIssued));
            SetState(NewGameConfirmationOwnerStateV1.AwaitingDecision);
            return Result(NewGameConfirmationOutcomeV1.DecisionRequired,capture.Classification,decision:_decision);
        }
        private DecisionWitness AuthenticateDecision(NewGameDecisionCapabilityV1 capability)
        {
            if (capability == null || !Decisions.TryGetValue(capability,out var w) || w == null || !ReferenceEquals(w.Owner,this) || w.Used) throw new InvalidOperationException("Confirmation decision is foreign or completed.");
            return w;
        }
        private void ValidateLiveDecision(NewGameDecisionCapabilityV1 capability,DecisionWitness w)
        { if (!ReferenceEquals(_decision,capability) || !ReferenceEquals(w.Token,_epochToken) || w.Epoch != _epoch || w.Generation != _generation || !ReferenceEquals(w.Display,_display) || !ReferenceEquals(w.Issued,_acceptedIssued) || _state != NewGameConfirmationOwnerStateV1.AwaitingDecision) throw new InvalidOperationException("Actual decision evidence is inconsistent."); }
        internal NewGameConfirmationStartResultV1 Confirm(NewGameDecisionCapabilityV1 capability)
        {
            var witness=AuthenticateDecision(capability); Enter();
            try
            {
                if (!ReferenceEquals(AuthenticateDecision(capability),witness)) throw new InvalidOperationException("Confirmation decision changed before entry.");
                try
                {
                Validate();
                ValidateLiveDecision(capability,witness);
                if (Interlocked.CompareExchange(ref witness.State,1,0) != 0) throw new InvalidOperationException("Confirmation decision is already inspecting.");
                CheckFault("AfterDecisionReserve");
                var capture=ProfileNewGameConfirmationV1.Capture(_adapter,_router,this,_epochToken);
                if (capture.Outcome == ProfileNewGameCaptureOutcomeV1.Busy)
                { Volatile.Write(ref witness.State,0); return Result(NewGameConfirmationOutcomeV1.Busy,decision:capability); }
                witness.Used=true; Volatile.Write(ref witness.State,2); _decision=null;
                if (capture.Outcome == ProfileNewGameCaptureOutcomeV1.Unreadable) { Terminal(); return Result(NewGameConfirmationOutcomeV1.CaptureUnreadable); }
                var resolution=ProfileNewGameConfirmationV1.ResolveConfirmedRecapture(witness.Display,capture,_adapter,_router,this,_epochToken,_epoch,_generation);
                if (resolution.Disposition == ProfileNewGameConfirmedDispositionV1.FreshDecisionRequired)
                { _fresh=capture; SetState(NewGameConfirmationOwnerStateV1.FreshDecisionRequired); return Result(NewGameConfirmationOutcomeV1.FreshDecisionRequired,capture.Classification); }
                _confirmed=resolution.ConfirmedRequest;
                SetState(NewGameConfirmationOwnerStateV1.ConfirmedReady);
                return Result(NewGameConfirmationOutcomeV1.Confirmed,capture.Classification,confirmed:_confirmed);
                }
                catch { Terminal(); throw; }
            }
            finally { Exit(); }
        }
        internal NewGameConfirmationStartResultV1 OpenFreshDecision()
        {
            if (_state != NewGameConfirmationOwnerStateV1.FreshDecisionRequired || _fresh == null) throw new InvalidOperationException("No fresh decision is pending.");
            var fresh=_fresh; var generation=_generation; var epoch=_epoch;
            Enter();
            try
            {
                if (!ReferenceEquals(_fresh,fresh) || _generation != generation || _epoch != epoch || _state == NewGameConfirmationOwnerStateV1.ExecutionCommitted || _state == NewGameConfirmationOwnerStateV1.TerminalFailure || _state == NewGameConfirmationOwnerStateV1.Closed) throw new InvalidOperationException("Fresh decision completed or changed before entry.");
                try
                {
                Validate();
                if (_state != NewGameConfirmationOwnerStateV1.FreshDecisionRequired || !ReferenceEquals(_fresh,fresh)) throw new InvalidOperationException("Actual fresh evidence is inconsistent.");
                _generation=checked(_generation+1); _display=_fresh; _fresh=null;
                return PublishDecision(_display);
                }
                catch { Terminal(); throw; }
            }
            finally { Exit(); }
        }
        internal NewGameConfirmationStartResultV1 Cancel(NewGameDecisionCapabilityV1 capability)
        {
            var witness=AuthenticateDecision(capability); Enter();
            try
            {
                if (!ReferenceEquals(AuthenticateDecision(capability),witness)) throw new InvalidOperationException("Confirmation decision changed before entry.");
                try
                {
                Validate();
                ValidateLiveDecision(capability,witness);
                if (Interlocked.CompareExchange(ref witness.State,2,0) != 0) throw new InvalidOperationException("Confirmation cannot cancel an inspecting decision.");
                witness.Used=true; _decision=null;
                _rearm=new HubNewGameRearmCapabilityV1();
                Rearms.Add(_rearm,new RearmWitness(this,_handoffOwner,_presenter,_router,_epochToken,_epoch));
                SetState(NewGameConfirmationOwnerStateV1.Cancelled);
                return Result(NewGameConfirmationOutcomeV1.Cancelled,rearm:_rearm);
                }
                catch { Terminal(); throw; }
            }
            finally { Exit(); }
        }
        internal bool IsActualRearm(HubNewGameRearmCapabilityV1 capability,HubMenuIntentHandoffOwnerV1 handoffOwner,HubMenuPresenterV1 presenter,InputRouter router,object token,long epoch)
        { return capability != null && Rearms.TryGetValue(capability,out var w) && w != null && ReferenceEquals(w.Owner,this) && ReferenceEquals(w.HandoffOwner,handoffOwner) && ReferenceEquals(w.Presenter,presenter) && ReferenceEquals(w.Router,router) && ReferenceEquals(w.Token,token) && w.Epoch == epoch; }
        internal bool IsRearmInProgress(HubNewGameRearmCapabilityV1 capability,HubMenuIntentHandoffOwnerV1 handoffOwner,HubMenuPresenterV1 presenter,InputRouter router,object token,long epoch)
        { return _state == NewGameConfirmationOwnerStateV1.Rearming && _state == _stateProof && Volatile.Read(ref _operation) == 1 && ReferenceEquals(_rearm,capability) && IsActualRearm(capability,handoffOwner,presenter,router,token,epoch) && Rearms.TryGetValue(capability,out var w) && !w.Used && Volatile.Read(ref w.Consumed) == 1; }
        internal void Rearm(HubNewGameRearmCapabilityV1 capability)
        {
            if (capability == null || !Rearms.TryGetValue(capability,out var witness) || !ReferenceEquals(witness.Owner,this) || !ReferenceEquals(witness.HandoffOwner,_handoffOwner) || !ReferenceEquals(witness.Presenter,_presenter) || !ReferenceEquals(witness.Router,_router) || witness.Used || Volatile.Read(ref witness.Consumed) != 0) throw new InvalidOperationException("Rearm capability is foreign or completed.");
            Enter();
            try
            {
                if (!Rearms.TryGetValue(capability,out var current) || !ReferenceEquals(current,witness) || witness.Used || Volatile.Read(ref witness.Consumed) != 0) throw new InvalidOperationException("Rearm completed before entry.");
                try
                {
                Validate();
                if (!ReferenceEquals(capability,_rearm) || !ReferenceEquals(witness.Token,_epochToken) || witness.Epoch != _epoch || _state != NewGameConfirmationOwnerStateV1.Cancelled) throw new InvalidOperationException("Actual rearm evidence is inconsistent.");
                if (Interlocked.CompareExchange(ref witness.Consumed,1,0) != 0) throw new InvalidOperationException("Rearm capability was already consumed.");
                SetState(NewGameConfirmationOwnerStateV1.Rearming);
                var token=new object(); var epoch=checked(_epoch+1);
                var reservation=_handoffOwner.ReserveNewGameSuccessor(capability,this,_presenter,_router,token,epoch);
                _presenter.PrepareNewGameSuccessor(this,reservation,token,epoch);
                CheckFault("AfterSuccessorPrepare");
                _handoffOwner.CommitNewGameSuccessor(reservation,this,_presenter,_router,token,epoch);
                _history=new EpochHistory(_history,_epochToken,_epoch,_acceptedIssued,_acceptedRequest.Value,capability);
                witness.Used=true;
                _epoch=_epochProof=epoch; _epochToken=_epochTokenProof=token;
                _acceptedIssued=null; _acceptedRequest=null; _display=null; _fresh=null; _decision=null; _retry=null; _confirmed=null; _rearm=null;
                SetState(NewGameConfirmationOwnerStateV1.AwaitingRequest);
                }
                catch { Terminal(); throw; }
            }
            finally { Exit(); }
        }
        internal ConfirmedProfileResetRequestV1 CommitForExecution(ConfirmedProfileResetRequestV1 request)
        {
            if (request == null || !ReferenceEquals(request,_confirmed) || _state != NewGameConfirmationOwnerStateV1.ConfirmedReady) throw new InvalidOperationException("Confirmed request is not ready.");
            Enter(); ProfileNewGameExecutionCommitReservationV1 reservation=null;
            try
            {
                if (!ReferenceEquals(request,_confirmed) || _state == NewGameConfirmationOwnerStateV1.ExecutionCommitted || _state == NewGameConfirmationOwnerStateV1.TerminalFailure || _state == NewGameConfirmationOwnerStateV1.Closed) throw new InvalidOperationException("Execution commit completed or changed before entry.");
                try
                {
                Validate();
                if (!ReferenceEquals(request,_confirmed) || _state != NewGameConfirmationOwnerStateV1.ConfirmedReady) throw new InvalidOperationException("Actual confirmed pending evidence is inconsistent.");
                reservation=ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,_adapter,_router,this,_epochToken,_epoch,_generation);
                var history=new CommitHistory(this,reservation);
                CheckFault("AfterLowerReserve");
                InvalidateCallbacks();
                _handoffOwner.CloseConfirmationInteraction(); _presenter.CloseConfirmationInteraction();
                SetState(NewGameConfirmationOwnerStateV1.ExecutionCommitted);
                CheckFault("AfterUpperClosure");
                var completed=ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,_adapter,_router,this,_epochToken,_epoch,_generation);
                _commitHistory=history;
                return completed;
                }
                catch
                {
                if (reservation != null) { try { ProfileNewGameConfirmationV1.CloseExecutionCommit(request,_adapter,_router,this,_epochToken,_epoch,_generation); } catch (InvalidOperationException) { } }
                Terminal(); throw;
                }
            }
            finally { Exit(); }
        }
        // REQ-M5D7QC4-001/004/006: 기존 commit과 명시 실행 진입을 분리한다.
        internal ProfileResetExecutionResultV1 ExecuteCommittedReset(ConfirmedProfileResetRequestV1 confirmed)
        {
            if (!Cohorts.TryGetValue(this,out var cohort) ||
                !ProfileNewGameConfirmationV1.IsExecutionAvailable(confirmed,cohort.Adapter,cohort.Router,this))
                throw new InvalidOperationException("Committed execution is unavailable.");
            Enter();
            ProfileResetExecutionResultV1 result;
            var lowerInvoked=false;
            try
            {
                // 정상 소비·교체 경쟁자는 현재 승자의 오류 처리 구간 밖에서 거절한다.
                if (!ProfileNewGameConfirmationV1.IsExecutionAvailable(confirmed,cohort.Adapter,cohort.Router,this) ||
                    !ReferenceEquals(_confirmed,confirmed) || _state == NewGameConfirmationOwnerStateV1.TerminalFailure ||
                    _state == NewGameConfirmationOwnerStateV1.Closed) throw new InvalidOperationException("Committed execution changed before entry.");
                try
                {
                    Validate();
                    if (_state != NewGameConfirmationOwnerStateV1.ExecutionCommitted) throw new InvalidOperationException("Original commit state is inconsistent.");
                    ValidateOriginalCommit(confirmed);
                    lowerInvoked=true;
                    result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(confirmed,cohort.Adapter,cohort.Router);
                    result.Validate(); _executionResult=result;
                }
                catch
                {
                    if (!lowerInvoked || ProfileNewGameConfirmationV1.IsExecutionFaulted(confirmed,cohort.Adapter,cohort.Router,this))
                    { Terminal(); if (!lowerInvoked) cohort.Adapter.LatchResetCutoverFailure(); }
                    throw;
                }
            }
            finally { Exit(); }
            if (result.Outcome == ProfileResetExecutionOutcomeV1.Busy || result.Outcome == ProfileResetExecutionOutcomeV1.ConfirmationStale)
                AcceptFreshExecutionHandback(result);
            else if (result.Outcome != ProfileResetExecutionOutcomeV1.Completed) Terminal();
            return result;
        }
        private void ValidateOriginalCommit(ConfirmedProfileResetRequestV1 confirmed)
        {
            var original=_commitHistory;
            if (original == null || !ReferenceEquals(original.Confirmed,confirmed) ||
                !ReferenceEquals(original.Token,_epochToken) || original.Epoch != _epoch ||
                original.Generation != _generation || !ReferenceEquals(original.Issued,_acceptedIssued) ||
                !ReferenceEquals(original.Display,_display) || !ReferenceEquals(original.Fresh,_fresh) ||
                original.Request.HasValue != _acceptedRequest.HasValue ||
                (original.Request.HasValue && !original.Request.Value.Equals(_acceptedRequest.Value)))
                throw new InvalidOperationException("Original commit projection is inconsistent.");
        }
        // REQ-M5D7QC4-004/006: 같은 실제 하위 결과에만 다음 빈 슬롯을 결속한다.
        internal void AcceptFreshExecutionHandback(ProfileResetExecutionResultV1 result)
        {
            if (!Cohorts.TryGetValue(this,out var cohort) || _commitHistory == null ||
                !ProfileNewGameConfirmationV1.IsFreshExecutionAvailable(result,cohort.Adapter,cohort.Router,this,
                    _commitHistory.Token,_commitHistory.Epoch,_commitHistory.Generation) ||
                !ReferenceEquals(_confirmed,_commitHistory.Confirmed) || _state != NewGameConfirmationOwnerStateV1.ExecutionCommitted)
                throw new InvalidOperationException("C4 fresh handback is unavailable to this owner.");
            var original=_commitHistory; Enter(); FreshOperation operation=null;
            ProfileResetFreshExecutionReservationV1 originalLowerReservation=null;
            try
            {
                if (!ReferenceEquals(_commitHistory,original) || !ReferenceEquals(_confirmed,original.Confirmed) ||
                    _state != NewGameConfirmationOwnerStateV1.ExecutionCommitted ||
                    !ProfileNewGameConfirmationV1.IsFreshExecutionAvailable(result,cohort.Adapter,cohort.Router,this,original.Token,original.Epoch,original.Generation))
                    throw new InvalidOperationException("C4 fresh handback changed before entry.");
                try
                {
                    Validate(); ValidateOriginalCommit(original.Confirmed); result.Validate();
                    operation=new FreshOperation(this,cohort,original,result);
                    for (var history=_history;history != null;history=history.Previous)
                        if (ReferenceEquals(history.Token,operation.Token)) throw new InvalidOperationException("C4 next token repeats history.");
                    _executionFresh=operation; SetState(NewGameConfirmationOwnerStateV1.ExecutionFreshPreparing);
                    operation.LowerReservation=ProfileNewGameConfirmationV1.ReserveFreshExecution(result,cohort.Adapter,cohort.Router,this,
                        original.Token,original.Epoch,original.Generation,operation.Token,operation.Epoch);
                    originalLowerReservation=operation.LowerReservation;
                    ExecutionFreshCapabilities.Add(operation.Capability,operation);
                    operation.UpperReservation=cohort.HandoffOwner.ReserveExecutionFresh(operation.Capability,this,cohort.Presenter,cohort.Router,operation.Token,operation.Epoch);
                    cohort.Presenter.PrepareExecutionFresh(this,operation.UpperReservation,operation.Token,operation.Epoch);
                    FreshCheckpoint(operation,ProfileResetExecutionCheckpointV1.AfterFreshPrepare);
                    if (!cohort.Presenter.MatchesPreparedExecutionFresh(this,operation.UpperReservation,operation.Token,operation.Epoch))
                        throw new InvalidOperationException("C4 reciprocal fresh acknowledgement failed.");
                    FreshCheckpoint(operation,ProfileResetExecutionCheckpointV1.AfterFreshAcknowledge);
                    cohort.HandoffOwner.CommitExecutionFresh(operation.UpperReservation,this,cohort.Presenter,cohort.Router,operation.Token,operation.Epoch);
                    FreshCheckpoint(operation,ProfileResetExecutionCheckpointV1.AfterFreshCommit);
                    FreshCheckpoint(operation,ProfileResetExecutionCheckpointV1.BeforeFreshConsume);
                    if (Interlocked.CompareExchange(ref operation.Consumed,1,0) != 0) throw new InvalidOperationException("C4 fresh owner capability was consumed.");
                    FreshCheckpoint(operation,ProfileResetExecutionCheckpointV1.AfterFreshConsume);
                    // 마지막 외부 제어가 Owner나 준비 슬롯을 닫은 경우 공개하지 않는다.
                    Validate(); ValidateOriginalCommit(original.Confirmed);
                    if (!ReferenceEquals(_executionFresh,operation) || _state != NewGameConfirmationOwnerStateV1.ExecutionFreshPreparing ||
                        Volatile.Read(ref operation.Consumed) != 1 || !ReferenceEquals(operation.Cohort,cohort) ||
                        !cohort.Presenter.MatchesPreparedExecutionFresh(this,operation.UpperReservation,operation.Token,operation.Epoch) ||
                        !ProfileNewGameConfirmationV1.ValidateFreshExecutionReservation(operation.LowerReservation,cohort.Adapter,cohort.Router,this,operation.Token,operation.Epoch))
                        throw new InvalidOperationException("C4 prepared owner ended before final publication.");
                    ProfileNewGameConfirmationV1.CompleteFreshExecutionReservation(operation.LowerReservation,cohort.Adapter,cohort.Router,this,operation.Token,operation.Epoch);
                    var completed=new ExecutionFreshHistory(_executionFreshHistory,operation);
                    var historyNode=new EpochHistory(_history,completed);
                    _executionFreshHistory=completed; _history=historyNode;
                    _epoch=_epochProof=operation.Epoch; _epochToken=_epochTokenProof=operation.Token;
                    _pendingIssued=null; _acceptedIssued=null; _acceptedRequest=null; _display=null; _fresh=null;
                    _decision=null; _retry=null; _confirmed=null; _rearm=null; _executionResult=result;
                    SetState(NewGameConfirmationOwnerStateV1.AwaitingRequest);
                }
                catch
                {
                    try { ProfileNewGameConfirmationV1.CloseFreshExecution(result,originalLowerReservation,cohort.Adapter,cohort.Router,this); }
                    finally { Terminal(); }
                    throw;
                }
            }
            finally { Exit(); }
        }
        private static void FreshCheckpoint(FreshOperation operation,ProfileResetExecutionCheckpointV1 checkpoint)
        { ProfileNewGameConfirmationV1.CheckpointFreshExecution(operation.Result,operation.LowerReservation,operation.Cohort.Adapter,operation.Cohort.Router,operation.Owner,checkpoint); }
        internal bool IsExecutionFreshInProgress(HubNewGameExecutionFreshCapabilityV1 capability,HubMenuIntentHandoffOwnerV1 handoffOwner,
            HubMenuPresenterV1 presenter,InputRouter router,object token,long epoch)
        {
            return capability != null && ExecutionFreshCapabilities.TryGetValue(capability,out var operation) &&
                ReferenceEquals(operation.Thread,Thread.CurrentThread) && ReferenceEquals(operation.Owner,this) && ReferenceEquals(_executionFresh,operation) &&
                ReferenceEquals(operation.Cohort.HandoffOwner,handoffOwner) && ReferenceEquals(operation.Cohort.Presenter,presenter) && ReferenceEquals(operation.Cohort.Router,router) &&
                ReferenceEquals(operation.Token,token) && operation.Epoch == epoch && Volatile.Read(ref operation.Consumed) == 0 &&
                _state == NewGameConfirmationOwnerStateV1.ExecutionFreshPreparing && _state == _stateProof && Volatile.Read(ref _operation) == 1 &&
                ReferenceEquals(operation.Original.Token,_epochToken) && operation.Original.Epoch == _epoch && _epoch == _epochProof &&
                ReferenceEquals(_epochToken,_epochTokenProof);
        }
        internal bool MatchesExecutionFreshCohort(HubMenuIntentHandoffOwnerV1 handoffOwner,HubMenuPresenterV1 presenter,InputRouter router,
            HubNewGameExecutionFreshReservationV1 reservation,object epochToken,long epoch)
        {
            var history=_executionFreshHistory;
            if (history == null || !ReferenceEquals(history.Operation.Thread,Thread.CurrentThread)) return false;
            var operation=history.Operation;
            return ReferenceEquals(operation.Owner,this) && ReferenceEquals(operation.Cohort.HandoffOwner,handoffOwner) &&
                ReferenceEquals(operation.Cohort.Presenter,presenter) && ReferenceEquals(operation.Cohort.Router,router) &&
                ReferenceEquals(history.UpperReservation,reservation) && ReferenceEquals(operation.Token,epochToken) && operation.Epoch == epoch &&
                ReferenceEquals(_epochToken,epochToken) && ReferenceEquals(_epochToken,_epochTokenProof) && _epoch == epoch && _epoch == _epochProof &&
                Volatile.Read(ref operation.Consumed) == 1 && _state == _stateProof && _state >= NewGameConfirmationOwnerStateV1.AwaitingRequest &&
                (_state <= NewGameConfirmationOwnerStateV1.ConfirmedReady ||
                 (_state <= NewGameConfirmationOwnerStateV1.Rearming && _rearm != null && Rearms.TryGetValue(_rearm,out var rearm) &&
                  ReferenceEquals(rearm.Owner,this) && ReferenceEquals(rearm.Token,epochToken) && rearm.Epoch == epoch));
        }
        private NewGameConfirmationStartResultV1 Result(NewGameConfirmationOutcomeV1 outcome,NewGameObservationClassificationV1? classification=null,NewGameDecisionCapabilityV1 decision=null,NewGameCaptureRetryCapabilityV1 retry=null,ConfirmedProfileResetRequestV1 confirmed=null,HubNewGameRearmCapabilityV1 rearm=null)
        {
            var row=new NewGameConfirmationStartResultV1(outcome,classification,decision,retry,confirmed,rearm);
            Results.Add(row,new NewGameConfirmationStartResultV1(outcome,classification,decision,retry,confirmed,rearm)); return row;
        }
        private void Validate()
        {
            if (!_configured || !Cohorts.TryGetValue(this,out var cohort) || !ReferenceEquals(cohort.Presenter,_presenter) || !ReferenceEquals(cohort.HandoffOwner,_handoffOwner) || !ReferenceEquals(cohort.Adapter,_adapter) || !ReferenceEquals(cohort.Router,_router) || !ReferenceEquals(_presenter.gameObject,gameObject) || !ReferenceEquals(_handoffOwner.gameObject,gameObject) || !ReferenceEquals(_presenter.HandoffOwner,_handoffOwner) || !ReferenceEquals(_handoffOwner.Presenter,_presenter) || _state != _stateProof || _state < NewGameConfirmationOwnerStateV1.AwaitingRequest || _state > NewGameConfirmationOwnerStateV1.ExecutionFreshPreparing || _epoch != _epochProof || _epoch < 1 || !ReferenceEquals(_epochToken,_epochTokenProof)) throw new InvalidOperationException("Confirmation owner evidence is malformed.");
            if (_state <= NewGameConfirmationOwnerStateV1.Rearming && !_presenter.MatchesConfirmationCohort(this,_handoffOwner,_router)) throw new InvalidOperationException("Live confirmation input connection is inconsistent.");
        }
        private void Enter() { if (Interlocked.CompareExchange(ref _operation,1,0) != 0) throw new InvalidOperationException("Confirmation owner is already inspecting."); }
        private bool IsNormallyEnded() { return _state == _stateProof && _state >= NewGameConfirmationOwnerStateV1.ExecutionCommitted && _state <= NewGameConfirmationOwnerStateV1.Closed; }
        private void Exit() { Volatile.Write(ref _operation,0); }
        private void SetState(NewGameConfirmationOwnerStateV1 state) { _state=_stateProof=state; }
        private void InvalidateCallbacks()
        {
            if (_decision != null && Decisions.TryGetValue(_decision,out var decision)) { decision.Used=true; Volatile.Write(ref decision.State,3); }
            if (_retry != null && Retries.TryGetValue(_retry,out var retry)) retry.Closed=true;
            if (_rearm != null && Rearms.TryGetValue(_rearm,out var rearm)) { rearm.Used=true; Volatile.Write(ref rearm.Consumed,1); }
            _decision=null; _retry=null; _rearm=null; _pendingIssued=null;
        }
        private void Terminal()
        {
            Cohorts.TryGetValue(this,out var originalCohort);
            if (_confirmed != null && _configured)
            { try { ProfileNewGameConfirmationV1.CloseExecutionCommit(_confirmed,originalCohort == null ? _adapter : originalCohort.Adapter,originalCohort == null ? _router : originalCohort.Router,this,_epochToken,_epoch,_generation); } catch (InvalidOperationException) { } }
            InvalidateCallbacks(); SetState(NewGameConfirmationOwnerStateV1.TerminalFailure);
            if (_peerClosing) return;
            _peerClosing=true;
            try
            {
                if (originalCohort != null) { try { originalCohort.HandoffOwner.CloseConfirmationInteraction(); } finally { originalCohort.Presenter.CloseConfirmationInteraction(); } }
                else { if (_handoffOwner != null) _handoffOwner.CloseConfirmationInteraction(); if (_presenter != null) _presenter.CloseConfirmationInteraction(); }
            }
            finally { _peerClosing=false; }
        }
        internal void CloseFromPeer() { if (_state == NewGameConfirmationOwnerStateV1.ExecutionCommitted || _state == NewGameConfirmationOwnerStateV1.Closed || _state == NewGameConfirmationOwnerStateV1.TerminalFailure) return; Terminal(); }
        internal void SetFaultForTests(string boundary)
        {
            if (boundary != null && boundary != "BeforeCapture" && boundary != "AfterDecisionReserve" && boundary != "AfterSuccessorPrepare" && boundary != "AfterLowerReserve" && boundary != "AfterUpperClosure") throw new ArgumentOutOfRangeException(nameof(boundary));
            _faultBoundary=boundary;
        }
        private void CheckFault(string boundary) { if (_faultBoundary == boundary) throw new InvalidOperationException("Confirmation boundary fault: "+boundary); }
        private void Awake() { try { Validate(); if (gameObject.GetComponents<NewGameConfirmationOwnerV1>().Length != 1) throw new InvalidOperationException("Duplicate confirmation owners."); } catch { Terminal(); } }
        private void OnDisable() { if (_configured) { Terminal(); SetState(NewGameConfirmationOwnerStateV1.Closed); } }
        private void OnDestroy() { if (_configured) { Terminal(); SetState(NewGameConfirmationOwnerStateV1.Closed); } }
    }
}
