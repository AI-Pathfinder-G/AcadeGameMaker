using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Run;
using AcadeGameMaker.Profile;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Combat.Unity;
using UnityEngine;

namespace AcadeGameMaker.Input.Unity
{
    internal interface IChapter1AuthoredViewPort
    {void BindChapter1AuthoredView(object flow,Chapter1ChoicePhaseAdapter source);}
    internal interface IChapter1ChoiceUiPort
    {
        void HandleChoiceUiIntent(Chapter1ChoicePhaseAdapter source, Chapter1ChoiceUiToken token);
        bool ValidatePendingChapter1SuccessfulClose(object passage);
        bool ValidatePendingChapter1SuccessfulRun(object demo, object branchSave);
        bool ValidatePendingClosingHostEntry(Chapter1ClosingHostEntry entry);
        bool ValidateCommittedChapter1Completion(Chapter1ChoicePhaseAdapter source,
            Chapter1ClosingHostEntry entry, object completion);
    }
    // REQ-C1C-UIR-003. 발급자는 원래 lifetime이며 소비자는 양측 pending/issued 동일 객체를 검증한다.
    internal interface IChapter1ClosingViewPort
    {
        void BeginFromCommittedRun(object demo, object branchSave);
        bool AdvanceFromPendingUi(Chapter1ChoicePhaseAdapter source, Chapter1ChoiceUiToken token);
        bool OwnsPendingClosingReceipt(object receipt);
        void ConsumePendingClosingReceipt(object receipt);
    }
    internal sealed class Chapter1ClosingHostEntry
    {
        internal Chapter1ClosingHostEntry(Chapter1EncounterLifetimeOwner owner, IChapter1ChoiceUiPort endpoint,
            object runKey, object demo, object branchSave, long oldDemoGen, InputFrameCommitReceipt endedReceipt,
            SimulationTick lastCompletedTick, object closureEvidence)
        {
            if(owner==null || endpoint==null || runKey==null || demo==null || branchSave==null || closureEvidence==null)
                throw new ArgumentNullException();
            if(oldDemoGen<1 || endedReceipt.Mode!=InputMode.Ended || lastCompletedTick.Value<endedReceipt.Tick.Value)
                throw new ArgumentException("실제 종료 입력과 마지막 완료 tick이 다릅니다.");
            Owner=owner;Endpoint=endpoint;RunKey=runKey;Demo=demo;BranchSave=branchSave;
            OldDemoGeneration=oldDemoGen;EndedReceipt=endedReceipt;LastCompletedTick=lastCompletedTick;
            ClosureEvidence=closureEvidence;NewUiGeneration=checked(oldDemoGen+1);
            FirstUiTick=new SimulationTick(checked(lastCompletedTick.Value+1));
        }
        internal Chapter1EncounterLifetimeOwner Owner { get; }
        internal IChapter1ChoiceUiPort Endpoint { get; }
        internal object RunKey { get; }
        internal object Demo { get; }
        internal object BranchSave { get; }
        internal long OldDemoGeneration { get; }
        internal long NewUiGeneration { get; }
        internal InputFrameCommitReceipt EndedReceipt { get; }
        internal SimulationTick LastCompletedTick { get; }
        internal SimulationTick FirstUiTick { get; }
        internal object ClosureEvidence { get; }
    }
    internal enum Chapter1ChoiceUiPhase { Required, ExtractionProposed, SolidarityProposed, Saving, SaveFailed, SaveUncertain, Barrier, Closing, Ended }
    internal sealed class Chapter1ChoiceUiToken
    {
        internal Chapter1ChoiceUiToken(Chapter1ChoicePhaseAdapter owner, InputRouter router, IChapter1ChoiceUiPort endpoint,
            object runKey, long generation, long ordinal, UiSemanticFrameV1 frame, string intent, Chapter1ChoiceUiPhase phase)
        { Owner=owner; Router=router; Endpoint=endpoint; RunKey=runKey; Generation=generation; RequestOrdinal=ordinal; Frame=frame; Intent=intent; Phase=phase; }
        internal Chapter1ChoicePhaseAdapter Owner { get; }
        internal InputRouter Router { get; }
        internal IChapter1ChoiceUiPort Endpoint { get; }
        internal object RunKey { get; }
        internal long Generation { get; }
        internal long RequestOrdinal { get; }
        internal UiSemanticFrameV1 Frame { get; }
        internal string Intent { get; }
        internal Chapter1ChoiceUiPhase Phase { get; }
    }

    // REQ-C1C-004. 콜백·FixedUpdate를 추가하지 않는 기존 입력 발행자의 한정 소비자다.
    [DisallowMultipleComponent]
    internal sealed class Chapter1ChoicePhaseAdapter : MonoBehaviour, IChapter1ChoicePhasePort
    {
        private InputRouter _router;
        private TransferSimulationDriver _transfer;
        private Chapter1ChoiceEnvironmentOwner _environment;
        private IChapter1ChoiceUiPort _endpoint;
        private object _runKey;
        private long _generation, _requestOrdinal, _lastUiOrdinal=-1;
        private int _selectedChoice;
        private bool _configured, _dispatching, _retired, _neutral;
        private Chapter1ChoiceUiToken _pendingUi, _savingUi;
        private Chapter1ChoiceUiPhase _phase;
        private ChoiceSkillSession _session;
        private ChoicePhaseCandidate _pendingChoice;
        private Chapter1ClosingHostEntry _closingEntry;
        private bool _technicalBlocked;
        private object _closingCompletion;
        private long _lastClosingObservedUiOrdinal=-1, _closingNeutralUiOrdinal=-1;
        private long _pendingClosingNeutralUiOrdinal=-1, _completionUiOrdinal=-1;
        private bool _completedReturnConsumed;
        internal Chapter1ClosingHostEntry ClosingHostEntry => _closingEntry;
        internal bool OwnsIssuedClosingHost => _configured && _closingEntry!=null && !_retired && !_technicalBlocked &&
            _router!=null && !_router.IsFaulted && _closingEntry.Owner!=null &&
            ReferenceEquals(_closingEntry.Endpoint,_endpoint) && ReferenceEquals(_closingEntry.RunKey,_runKey) &&
            _generation==_closingEntry.NewUiGeneration &&
            _closingEntry.Owner.OwnsChapter1ClosingHost(_closingEntry,_router);
        // REQ-C1C-UIR-002/003. 표시 준비만 읽는다. 요청 권한은 아래 현재 frame/원본 검증에서 다시 대조한다.
        internal bool ClosingControlsNeutralReady
        {
            get
            {
                if(!OwnsIssuedClosingHost || !_neutral || _closingNeutralUiOrdinal<0 || _completedReturnConsumed ||
                    (_phase!=Chapter1ChoiceUiPhase.Closing && _phase!=Chapter1ChoiceUiPhase.Ended) ||
                    (_phase==Chapter1ChoiceUiPhase.Ended && !OwnsCurrentClosingCompletion()))return false;
                var current=_router.CurrentUiFrame;
                if(!current.HasValue)return false;
                var frame=current.Value;frame.Validate();
                return frame.Receipt.FrameOrdinal==_lastClosingObservedUiOrdinal &&
                    frame.Receipt.FrameOrdinal==_closingNeutralUiOrdinal && frame.Receipt.FrameOrdinal>_completionUiOrdinal &&
                    !frame.SubmitPressed && !frame.ClickPressed && !frame.CancelPressed &&
                    _router.TryReadChapter1ClosingControlsNeutral(this,_closingEntry,frame,out var neutral) && neutral;
            }
        }
        internal InputRouter Router => _router;
        internal Chapter1ChoiceUiPhase UiPhase => _phase;
        internal object RunSkillSessionForHandoff => _session;
        internal void ConfigureChoiceHostForAuthoring(InputRouter router,IChapter1ChoiceUiPort endpoint,object runKey,long generation)
        {
            if(gameObject.activeInHierarchy || _configured || router==null || endpoint==null || runKey==null || generation<1)throw new InvalidOperationException("선택 호스트 결속이 유효하지 않습니다.");
            _router=router;_endpoint=endpoint;_runKey=runKey;_generation=generation;_phase=Chapter1ChoiceUiPhase.Required;_configured=true;
        }

        // 원정 FlowOwner가 보관한 같은 객체만 새 graph에 결속한다. cooldown을 재생성하거나 낮추지 않는다.
        internal void BindRunSkillSessionForAuthoring(object runSession)
        {
            if(gameObject.activeInHierarchy || !_configured || _session!=null || _retired ||
                !(runSession is ChoiceSkillSession session) || !session.IsOwnedByRun(_runKey))
                throw new InvalidOperationException("현재 원정의 같은 기술 세션이 아닙니다.");
            _session=session;_phase=Chapter1ChoiceUiPhase.Barrier;
        }

        internal void ConfigureForAuthoring(InputRouter router, TransferSimulationDriver transfer,
            Chapter1ChoiceEnvironmentOwner environment, object runKey, long generation)
        {
            if(gameObject.activeInHierarchy || _configured || router==null || transfer==null || environment==null || runKey==null || generation<1)
                throw new InvalidOperationException("선택 포트는 비활성 새 그래프에 한 번 결속합니다.");
            _router=router; _transfer=transfer; _environment=environment; _runKey=runKey; _generation=generation;
            _phase=Chapter1ChoiceUiPhase.Required; _configured=true;
        }
        // REQ-C1C-UIR-001/002. 새 비활성 router의 구성 및 Issued 전환은 lifetime 소유다.
        internal void ConfigureClosingHostForAuthoring(InputRouter router, Chapter1ClosingHostEntry entry)
        {
            if(gameObject.activeInHierarchy || _configured || _retired || router==null || entry==null ||
                !entry.Owner.ValidatePendingChapter1ClosingHostEntry(entry) ||
                !entry.Endpoint.ValidatePendingClosingHostEntry(entry))
                throw new InvalidOperationException("현재 종료 호스트의 원본 pending 입구가 아닙니다.");
            _router=router;_closingEntry=entry;_endpoint=entry.Endpoint;_runKey=entry.RunKey;
            _generation=entry.NewUiGeneration;_phase=Chapter1ChoiceUiPhase.Closing;_configured=true;
        }
        internal void BindUiPort(IChapter1ChoiceUiPort endpoint)
        {
            if(gameObject.activeInHierarchy || !_configured || _endpoint!=null || endpoint==null || _retired)
                throw new InvalidOperationException("선택 UI 결속을 바꿀 수 없습니다.");
            _endpoint=endpoint;
        }
        internal bool SubmitUiIntent(UiSemanticFrameV1 frame, string intent)
        {
            if(!_configured || _retired || _technicalBlocked || _dispatching || _pendingUi!=null || _endpoint==null || _router.IsFaulted) return false;
            frame.Validate();
            if(!_router.CurrentUiFrame.HasValue || !_router.CurrentUiFrame.Value.Equals(frame) || frame.Receipt.FrameOrdinal<=_lastUiOrdinal) return false;
            if(_closingEntry!=null)return SubmitClosingUiIntent(frame,intent);
            var edge=frame.SubmitPressed || frame.ClickPressed || frame.CancelPressed;
            if(!edge) { _neutral=true; return false; }
            if(!_neutral || !Allows(intent, _phase) || (frame.CancelPressed && intent!="chapter1.choice-cancel")) return false;
            // 한 frame Click+Submit·재진입·저장 재호출을 consume보다 먼저 닫는다.
            var next=checked(_requestOrdinal+1);
            _neutral=false; _lastUiOrdinal=frame.Receipt.FrameOrdinal; _requestOrdinal=next;
            _pendingUi=new Chapter1ChoiceUiToken(this,_router,_endpoint,_runKey,_generation,next,frame,intent,_phase);
            _dispatching=true;
            try { _endpoint.HandleChoiceUiIntent(this,_pendingUi); }
            finally { _pendingUi=null; _dispatching=false; }
            return true;
        }
        // REQ-C1C-UIR-001/002/003, REQ-C1C-004/005/006. 물리 샘플은 router가 발행한 현재 transaction의 읽기만 사용한다.
        private bool SubmitClosingUiIntent(UiSemanticFrameV1 frame,string intent)
        {
            var ordinal=frame.Receipt.FrameOrdinal;
            if(!OwnsIssuedClosingHost || _completedReturnConsumed ||
                (_phase!=Chapter1ChoiceUiPhase.Closing && _phase!=Chapter1ChoiceUiPhase.Ended) ||
                ordinal<=_lastClosingObservedUiOrdinal || ordinal<=_completionUiOrdinal ||
                frame.Receipt.Tick.Value<_closingEntry.FirstUiTick.Value ||
                (_phase==Chapter1ChoiceUiPhase.Ended && !OwnsCurrentClosingCompletion()) ||
                !_router.TryReadChapter1ClosingControlsNeutral(this,_closingEntry,frame,out var neutral))return false;
            var edge=frame.SubmitPressed || frame.ClickPressed || frame.CancelPressed;
            var armed=_neutral && _closingNeutralUiOrdinal>=0 && ordinal>_closingNeutralUiOrdinal;
            var neutralOrdinal=_closingNeutralUiOrdinal;
            var accepted=edge && armed && !frame.CancelPressed && Allows(intent,_phase);
            var next=accepted?checked(_requestOrdinal+1):0;
            // 이미 관측한 중립과 거절된 59틱 edge도 다시 쓰지 않는다. edge frame은 중립으로 arm하지 않는다.
            _lastClosingObservedUiOrdinal=ordinal;
            if(!edge)
            {
                _neutral=neutral;_closingNeutralUiOrdinal=neutral?ordinal:-1;
                return false;
            }
            _neutral=false;_closingNeutralUiOrdinal=-1;_lastUiOrdinal=ordinal;
            if(!accepted)return false;
            _requestOrdinal=next;_pendingClosingNeutralUiOrdinal=neutralOrdinal;
            _pendingUi=new Chapter1ChoiceUiToken(this,_router,_endpoint,_runKey,_generation,next,frame,intent,_phase);
            _dispatching=true;
            try{_endpoint.HandleChoiceUiIntent(this,_pendingUi);}
            finally{_pendingUi=null;_pendingClosingNeutralUiOrdinal=-1;_dispatching=false;}
            return true;
        }
        private bool OwnsCurrentClosingCompletion() => _closingCompletion!=null && _completionUiOrdinal>=0 &&
            OwnsIssuedClosingHost && _endpoint.ValidateCommittedChapter1Completion(this,_closingEntry,_closingCompletion);
        // REQ-C1C-UIR-001/003, REQ-C1C-005/006. 완료 발급 뒤 별도 후속 통지이며 불투명 값은 endpoint가 원본 검증한다.
        internal void CompleteClosingFromCommittedReceipt(IChapter1ChoiceUiPort endpoint,
            Chapter1ClosingHostEntry entry,object completion)
        {
            if(!OwnsIssuedClosingHost || !ReferenceEquals(endpoint,_endpoint) || !ReferenceEquals(entry,_closingEntry) ||
                _phase!=Chapter1ChoiceUiPhase.Closing || _closingCompletion!=null || completion==null || _completedReturnConsumed ||
                _pendingUi!=null || !endpoint.ValidateCommittedChapter1Completion(this,entry,completion))
                throw new InvalidOperationException("현재 종료 호스트의 원본 완료 통지가 아닙니다.");
            var current=_router.CurrentUiFrame;
            if(!current.HasValue)throw new InvalidOperationException("완료 이후 입력 하한의 실제 frame이 없습니다.");
            var frame=current.Value;frame.Validate();
            if(frame.Receipt.FrameOrdinal<_lastClosingObservedUiOrdinal || frame.Receipt.FrameOrdinal<_lastUiOrdinal ||
                frame.Receipt.Tick.Value<entry.FirstUiTick.Value ||
                !_router.TryReadChapter1ClosingControlsNeutral(this,entry,frame,out _))
                throw new InvalidOperationException("완료 통지의 현재 발행 입력 증거가 다릅니다.");
            // 모든 검증 뒤에만 한 번 보관한다. 실패와 중복 통지는 기존 상태를 바꾸지 않는다.
            _closingCompletion=completion;_phase=Chapter1ChoiceUiPhase.Ended;_pendingUi=null;
            _pendingClosingNeutralUiOrdinal=-1;_neutral=false;_closingNeutralUiOrdinal=-1;
            _completionUiOrdinal=frame.Receipt.FrameOrdinal;_lastClosingObservedUiOrdinal=frame.Receipt.FrameOrdinal;
        }
        internal bool ValidatePendingUi(IChapter1ChoiceUiPort endpoint, Chapter1ChoiceUiToken token)
        {
            if(!_configured || _retired || _technicalBlocked || (_closingEntry!=null && !OwnsIssuedClosingHost) || !_dispatching || token==null || _router.IsFaulted ||
                !ReferenceEquals(endpoint,_endpoint) || !ReferenceEquals(token,_pendingUi) || !ReferenceEquals(token.Owner,this) ||
                !ReferenceEquals(token.Router,_router) || !ReferenceEquals(token.Endpoint,endpoint) || !ReferenceEquals(token.RunKey,_runKey) ||
                token.Generation!=_generation || token.RequestOrdinal!=_requestOrdinal || token.Phase!=_phase ||
                !_router.CurrentUiFrame.HasValue || !_router.CurrentUiFrame.Value.Equals(token.Frame) || !Allows(token.Intent,_phase)) return false;
            token.Frame.Validate();
            if(_closingEntry!=null && (_completedReturnConsumed || token.Frame.CancelPressed ||
                (!token.Frame.SubmitPressed && !token.Frame.ClickPressed) || _pendingClosingNeutralUiOrdinal<0 ||
                token.Frame.Receipt.FrameOrdinal<=_pendingClosingNeutralUiOrdinal ||
                token.Frame.Receipt.FrameOrdinal!=_lastClosingObservedUiOrdinal || token.Frame.Receipt.FrameOrdinal!=_lastUiOrdinal ||
                token.Frame.Receipt.Tick.Value<_closingEntry.FirstUiTick.Value ||
                !_router.TryReadChapter1ClosingControlsNeutral(this,_closingEntry,token.Frame,out _) ||
                (token.Intent=="chapter1.completed-return" && (_phase!=Chapter1ChoiceUiPhase.Ended ||
                    _pendingClosingNeutralUiOrdinal<=_completionUiOrdinal || !OwnsCurrentClosingCompletion()))))return false;
            return true;
        }
        internal void ConsumePendingUi(IChapter1ChoiceUiPort endpoint, Chapter1ChoiceUiToken token)
        {
            if(!ValidatePendingUi(endpoint,token)) throw new InvalidOperationException("현재 선택 UI 요청이 아닙니다.");
            _pendingUi=null;
            switch(token.Intent)
            {
                case "chapter1.choice-extraction": _selectedChoice=1; _phase=Chapter1ChoiceUiPhase.ExtractionProposed; break;
                case "chapter1.choice-solidarity": _selectedChoice=2; _phase=Chapter1ChoiceUiPhase.SolidarityProposed; break;
                case "chapter1.choice-cancel": _selectedChoice=0; _phase=Chapter1ChoiceUiPhase.Required; break;
                case "chapter1.choice-confirm": case "chapter1.choice-save-retry": _savingUi=token; _phase=Chapter1ChoiceUiPhase.Saving; break;
                case "chapter1.completed-return": _completedReturnConsumed=true;_neutral=false;_closingNeutralUiOrdinal=-1;break;
            }
        }
        // 실제 FlowOwner의 공개 V1 저장 호출 뒤에만 연결할 한정 종료점. 저장 자체를 소유하지 않는다.
        internal void CompleteChoiceSave(IChapter1ChoiceUiPort endpoint, Chapter1ChoiceUiToken token,
            ProfileProgressionSnapshot saved, ProfileAtomicSaveResultV1 result, long expectedRevision)
        {
            if(_retired || !ReferenceEquals(endpoint,_endpoint) || token==null || !ReferenceEquals(token,_savingUi) || _phase!=Chapter1ChoiceUiPhase.Saving)
                throw new InvalidOperationException("현재 저장 요청이 아닙니다.");
            result.Validate(); saved.Validate();
            if(result.Outcome==ProfileAtomicSaveOutcome.FailedBeforeCommit){_phase=Chapter1ChoiceUiPhase.SaveFailed; return;}
            if(result.Outcome==ProfileAtomicSaveOutcome.CommitOutcomeUncertain){_phase=Chapter1ChoiceUiPhase.SaveUncertain; return;}
            if(result.CommittedRevision!=expectedRevision || saved.ProfileRevision!=expectedRevision || !saved.TryGetCommittedPair(out var pair))
                throw new InvalidOperationException("선택 저장의 실제 개정이 다릅니다.");
            pair.Validate(); var choice=(int)pair.Choice; var skill=(int)pair.Skill; var consent=(int)saved.ConsentState;
            if(choice!=_selectedChoice) throw new InvalidOperationException("저장 분기가 제안과 다릅니다.");
            _session=new ChoiceSkillSession(_runKey,choice,consent,skill); _savingUi=null; _phase=Chapter1ChoiceUiPhase.Barrier;
        }
        // REQ-C1C-UIR-004/005. 실제 driver의 opaque proof만 운반하며 Session을 직접 회수하지 않는다.
        public ChoicePhaseCandidate PrepareChoice(ChoicePhaseContext context)
        {
            if(_retired || _technicalBlocked || _pendingChoice!=null || _session==null || !Current(context)) return null;
            ChoiceSkillCandidate session=null;ChoiceEnvironmentCandidate environment=null;object recall=null;
            try
            {
                session=_session.Prepare(context);if(session==null)return null;
                environment=context.Skill==1?_environment.PrepareCompressionReservation(context):_environment.PrepareReferencePlane(context);
                if(environment==null){_session.Cancel(session);return null;}
                if(context.Skill==1)
                {
                    recall=_transfer.PrepareChapter1RecallForChoiceSkill(context);
                    if(recall==null){_session.Cancel(session);_environment.Cancel(environment);return null;}
                }
                _pendingChoice=new ChoicePhaseCandidate(this,context,session,recall,environment);
                if(!ValidateChoice(_pendingChoice)){CancelChoice(_pendingChoice);return null;}
                return _pendingChoice;
            }
            catch(OverflowException)
            {
                CancelPrepared(context,session,environment,recall);return null;
            }
            catch
            {
                CancelPrepared(context,session,environment,recall);throw;
            }
        }
        private void CancelPrepared(ChoicePhaseContext context, ChoiceSkillCandidate session,
            ChoiceEnvironmentCandidate environment, object recall)
        {
            if(recall!=null)_transfer.CancelChapter1RecallForChoiceSkill(context,recall);
            if(environment!=null)_environment.Cancel(environment);
            if(session!=null)_session.Cancel(session);
            _pendingChoice=null;
        }
        public bool ValidateChoice(ChoicePhaseCandidate candidate) => !_technicalBlocked && candidate!=null && ReferenceEquals(candidate,_pendingChoice) &&
            ReferenceEquals(candidate.Owner,this) && Current(candidate.Context) &&
            candidate.SessionProof is ChoiceSkillCandidate session && _session.Validate(session) &&
            candidate.EnvironmentProof is ChoiceEnvironmentCandidate environment && _environment.Validate(environment) &&
            (candidate.Context.Skill==1?_transfer.ValidateChapter1RecallForChoiceSkill(candidate.Context,candidate.RecallProof):candidate.RecallProof==null);
        public ChoicePhaseResult CommitChoice(ChoicePhaseCandidate candidate)
        {
            if(!ValidateChoice(candidate)) throw new InvalidOperationException("현재 생산 기술 후보가 아닙니다.");
            try
            {
                if(candidate.Context.Skill==1)_transfer.CommitChapter1RecallForChoiceSkill(candidate.Context,candidate.RecallProof);
                _environment.Commit((ChoiceEnvironmentCandidate)candidate.EnvironmentProof);
                var result=_session.Commit((ChoiceSkillCandidate)candidate.SessionProof);_pendingChoice=null;return result;
            }
            catch
            {
                // 이미 발행한 input receipt와 부분 변경을 보존한다. driver/sink의 후속 phase 봉쇄는 공유 통합 관문이다.
                _technicalBlocked=true;throw;
            }
        }
        public void CancelChoice(ChoicePhaseCandidate candidate)
        {
            if(_technicalBlocked || candidate==null || !ReferenceEquals(candidate,_pendingChoice) || !ReferenceEquals(candidate.Owner,this)) return;
            CancelPrepared(candidate.Context,(ChoiceSkillCandidate)candidate.SessionProof,(ChoiceEnvironmentCandidate)candidate.EnvironmentProof,candidate.RecallProof);
        }
        private bool Current(ChoicePhaseContext context) => context!=null && _configured && !_retired && !_technicalBlocked && _phase==Chapter1ChoiceUiPhase.Barrier &&
            ReferenceEquals(context.Adapter,this) && ReferenceEquals(context.Driver,_transfer) && ReferenceEquals(context.RunKey,_runKey) &&
            context.Generation==_generation && _transfer.ValidatePendingChoiceContext(context);
        private static bool Allows(string intent, Chapter1ChoiceUiPhase phase)
        {
            switch(intent)
            {
                case "chapter1.choice-extraction": case "chapter1.choice-solidarity":
                    return phase==Chapter1ChoiceUiPhase.Required || phase==Chapter1ChoiceUiPhase.ExtractionProposed || phase==Chapter1ChoiceUiPhase.SolidarityProposed;
                case "chapter1.choice-confirm": return phase==Chapter1ChoiceUiPhase.ExtractionProposed || phase==Chapter1ChoiceUiPhase.SolidarityProposed;
                case "chapter1.choice-cancel": return phase==Chapter1ChoiceUiPhase.ExtractionProposed || phase==Chapter1ChoiceUiPhase.SolidarityProposed;
                case "chapter1.choice-save-retry": return phase==Chapter1ChoiceUiPhase.SaveFailed;
                case "chapter1.closing-next": return phase==Chapter1ChoiceUiPhase.Closing;
                case "chapter1.completed-return": return phase==Chapter1ChoiceUiPhase.Ended;
                default: return false;
            }
        }
        private void OnDisable(){if(_pendingChoice!=null)CancelChoice(_pendingChoice);_retired=true;_pendingUi=null;_savingUi=null;_neutral=false;_closingNeutralUiOrdinal=-1;_pendingClosingNeutralUiOrdinal=-1;}
    }
}
