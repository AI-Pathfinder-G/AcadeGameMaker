using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using UnityEngine;

namespace AcadeGameMaker.Run.Unity
{
    // REQ-C1B-HUB-001/ROOM-001/FAIL-001/RETURN-001: 원정 상태와 단일 실패 commit의 소유자.
    [DefaultExecutionOrder(210)]
    [DisallowMultipleComponent]
    internal sealed class Chapter1PlayableFlowOwner : MonoBehaviour,IChapter1HubRequestPort,IChapter1BossProgressionPort,IChapter1ChoiceUiPort
    {
        [SerializeField] private GameObject _hubPrefab,_ordanPrefab;
        [SerializeField] private GameObject _choicePrefab,_barrierPrefab,_closingPrefab;
        private Chapter1ChoicePhaseAdapter _choiceAdapter;
        private IChapter1ClosingViewPort _closingView;
        private Chapter1DemoEndReceipt _demoReceipt;
        private Chapter1BranchSaveReceipt _branchSaveReceipt;
        private Chapter1CompletionReceipt _chapterCompletion;
        private object _passage;
        private int _proposedChoice;
        private bool _choiceHostStarted,_closingStarted;
        private double _postBossWaitTime;
        private int _postBossWaitFrame;
        // REQ-AUTOCH1-CLOSINGDIAG-001: bounded last original evaluation; never query a producer for logging.
        private string _postBossLastEvaluation="unobserved";
        private double _postBossLastEvaluationTime;
        private int _postBossLastEvaluationFrame;
        private Func<bool> _completedUiClosed;
        internal Chapter1CompletionReceipt CurrentChapter1CompletionReceipt=>_chapterCompletion;
        internal void ConfigureChapter1PostBossForAuthoring(GameObject choice,GameObject barrier,GameObject closing)
        {
            if(gameObject.activeInHierarchy||_choicePrefab!=null||choice==null||barrier==null||closing==null||choice.activeSelf||barrier.activeSelf||closing.activeSelf)
                throw new InvalidOperationException("보스 이후 정적 프리팹이 유효하지 않습니다.");
            _choicePrefab=choice;_barrierPrefab=barrier;_closingPrefab=closing;
        }
        public bool ValidatePendingChapter1SuccessfulClose(object passage)=>ReferenceEquals(passage,_passage)&&_phase==Chapter1RunPhase.AtBarrier&&_lifetime.ValidatePendingChapter1Passage(passage);
        public bool ValidatePendingChapter1SuccessfulRun(object demo,object save)=>demo!=null&&save!=null&&ReferenceEquals(demo,_demoReceipt)&&ReferenceEquals(save,_branchSaveReceipt)&&
            ReferenceEquals(_demoReceipt.Owner,this)&&ReferenceEquals(_branchSaveReceipt.Demo,_demoReceipt)&&ReferenceEquals(_demoReceipt.RunKey,_runKey)&&
            _branchSaveReceipt.CommittedRevision==_profile.Snapshot.ProfileRevision&&_demoReceipt.CommittedRunEndRevision==_runEndCommittedRevision;
        public bool ValidatePendingClosingHostEntry(Chapter1ClosingHostEntry entry)=>entry!=null&&ReferenceEquals(entry.Endpoint,this)&&ReferenceEquals(entry.Owner,_lifetime)&&
            ReferenceEquals(entry.RunKey,_runKey)&&ReferenceEquals(entry.Demo,_demoReceipt)&&ReferenceEquals(entry.BranchSave,_branchSaveReceipt)&&
            entry.OldDemoGeneration==_demoReceipt.Generation&&entry.NewUiGeneration==checked(entry.OldDemoGeneration+1)&&ValidatePendingChapter1SuccessfulRun(entry.Demo,entry.BranchSave);
        public bool ValidateCommittedChapter1Completion(Chapter1ChoicePhaseAdapter source,Chapter1ClosingHostEntry entry,object completion)=>completion!=null&&ReferenceEquals(completion,_chapterCompletion)&&
            source!=null&&ReferenceEquals(source,_choiceAdapter)&&ReferenceEquals(entry,source.ClosingHostEntry)&&ValidatePendingClosingHostEntry(entry)&&
            ReferenceEquals(_chapterCompletion.Owner,this)&&ReferenceEquals(_chapterCompletion.Demo,_demoReceipt)&&ReferenceEquals(_chapterCompletion.Save,_branchSaveReceipt)&&
            ReferenceEquals(_chapterCompletion.Closing.HostEntry,entry)&&(_phase==Chapter1RunPhase.ChapterCompleted||_phase==Chapter1RunPhase.RetiringCompleted);
        internal bool ValidatePendingChapter1ClosingStart(object presenter,Chapter1DemoEndReceipt demo,Chapter1BranchSaveReceipt save)=>presenter!=null&&ReferenceEquals(presenter,_closingView)&&
            ValidatePendingChapter1SuccessfulRun(demo,save)&&_choiceAdapter!=null&&_choiceAdapter.OwnsIssuedClosingHost&&(_phase==Chapter1RunPhase.PreparingClosing||_phase==Chapter1RunPhase.Closing);
        internal void TryCommitChapterCompletion(Chapter1DemoEndReceipt demo,Chapter1ClosingReceipt closing,Chapter1BranchSaveReceipt save)
        {
            if(_chapterCompletion!=null||_phase!=Chapter1RunPhase.Closing||!ValidatePendingChapter1SuccessfulRun(demo,save)||closing==null||
                !ReferenceEquals(closing.Presenter,_closingView)||!ReferenceEquals(closing.HostEntry,_choiceAdapter.ClosingHostEntry)||!_closingView.OwnsPendingClosingReceipt(closing))
                throw new InvalidOperationException("원본 마지막 대사와 현재 완료 권위가 다릅니다.");
            var prepared=new Chapter1CompletionReceipt(this,demo,closing,save);
            _closingView.ConsumePendingClosingReceipt(closing);_chapterCompletion=prepared;_phase=Chapter1RunPhase.ChapterCompleted;
        }
        public void HandleChoiceUiIntent(Chapter1ChoicePhaseAdapter source,Chapter1ChoiceUiToken token)
        {
            if(source==null||!ReferenceEquals(source,_choiceAdapter)||token==null||!source.ValidatePendingUi(this,token))return;
            try
            {
                if(token.Intent=="chapter1.closing-next"){if(_phase!=Chapter1RunPhase.Closing||!_closingView.AdvanceFromPendingUi(source,token))return;return;}
                if(token.Intent=="chapter1.completed-return")
                {
                    if(!ValidateCommittedChapter1Completion(source,source.ClosingHostEntry,_chapterCompletion))throw new InvalidOperationException("완료 복귀의 원본이 다릅니다.");
                    source.ConsumePendingUi(this,token);_completedUiClosed=_lifetime.RetireChapter1ClosingHost(this,_chapterCompletion);_phase=Chapter1RunPhase.RetiringCompleted;BeginPostBossWait();return;
                }
                if(_phase!=Chapter1RunPhase.ChoiceRequired)return;
                source.ConsumePendingUi(this,token);
                if(token.Intent=="chapter1.choice-extraction"){_proposedChoice=1;return;}
                if(token.Intent=="chapter1.choice-solidarity"){_proposedChoice=2;return;}
                if(token.Intent=="chapter1.choice-cancel"){_proposedChoice=0;return;}
                if(token.Intent!="chapter1.choice-confirm"&&token.Intent!="chapter1.choice-save-retry")return;
                if(_proposedChoice!=1&&_proposedChoice!=2)throw new InvalidOperationException("실제 선택 제안이 없습니다.");
                ProfileSnapshotV1 before;ProfileAtomicSaveResultV1 result;
                if(token.Intent=="chapter1.choice-save-retry")result=_profile.RetryChapter1Choice(_proposedChoice,_proposedChoice==1?4:3,_proposedChoice,out before,out _lastChoiceRetryStatus);
                else{before=_profile.Snapshot;result=_profile.SaveChapter1Choice(_proposedChoice,_proposedChoice==1?4:3,_proposedChoice);_lastChoiceRetryStatus="not-retry";}
                var expected=checked(before.ProfileRevision+1);_lastSaveResult=result;
                source.CompleteChoiceSave(this,token,Chapter1DevelopmentProfile.IsExactCommit(result,expected)?_profile.Snapshot.Progression:before.Progression,result,expected);
                if(Chapter1DevelopmentProfile.IsExactCommit(result,expected))StartBarrier();
            }
            catch(Exception error){Stop(error);}
        }
        private void BeginPostBossWait(){_postBossWaitTime=Time.realtimeSinceStartupAsDouble;_postBossWaitFrame=Time.frameCount;_postBossLastEvaluation="unobserved";_postBossLastEvaluationTime=0;_postBossLastEvaluationFrame=-1;}
        private void RecordPostBossEvaluation(string value)
        {_postBossLastEvaluation=value;_postBossLastEvaluationTime=Time.realtimeSinceStartupAsDouble;_postBossLastEvaluationFrame=Time.frameCount;}
        private void CheckPostBossWait()
        {
            var now=Time.realtimeSinceStartupAsDouble;var frame=Time.frameCount;
            var elapsed=now-_postBossWaitTime;var frames=frame-_postBossWaitFrame;
            var secondsReached=elapsed>=5;var framesReached=frames>=300;
            // REQ-AUTOCH1-WAIT-001: render counts are diagnostic; only finite monotonic real time grants the wait.
            var invalidTime=double.IsNaN(now)||double.IsInfinity(now)||double.IsNaN(_postBossWaitTime)||double.IsInfinity(_postBossWaitTime)||double.IsNaN(elapsed)||double.IsInfinity(elapsed)||elapsed<0;
            if(!invalidTime && !secondsReached)return;
            var message="장면 연결 준비 기한이 지났습니다. [CH1ClosingWait] phase="+_phase+
                " beginTime="+_postBossWaitTime.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+
                " now="+now.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+
                " elapsed="+elapsed.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+
                " beginFrame="+_postBossWaitFrame+" frame="+frame+" deltaFrame="+frames+
                " secondsReached="+secondsReached+" framesReached="+framesReached+" invalidTime="+invalidTime+
                " fixedTime="+Time.fixedTimeAsDouble.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+
                " lastTick="+(_lastTick.HasValue?_lastTick.Value.Value.ToString():"unobserved")+
                " observedGeneration="+_observedGeneration+" runKey="+(_runKey??"none")+
                " choiceAdapterPresent="+(_choiceAdapter!=null)+" lastActualEvaluation="+_postBossLastEvaluation+
                " lastEvaluationTime="+_postBossLastEvaluationTime.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+
                " lastEvaluationFrame="+_postBossLastEvaluationFrame;
            var truncated=false;while(System.Text.Encoding.UTF8.GetByteCount(message)>4000){message=message.Substring(0,message.Length-1);truncated=true;}
            throw new InvalidOperationException(message+" recordTruncated="+truncated);
        }
        private void BindAuthoredPostBossView(GameObject prefab,Chapter1ChoicePhaseAdapter source,bool closing)
        {
            var view=Instantiate(prefab,source.transform,false);IChapter1AuthoredViewPort port=null;
            foreach(var component in view.GetComponentsInChildren<MonoBehaviour>(true))if(component is IChapter1AuthoredViewPort candidate){if(port!=null)throw new InvalidOperationException("표시 결속 포트가 중복입니다.");port=candidate;}
            if(port==null)throw new InvalidOperationException("저작 표시 결속 포트가 없습니다.");
            port.BindChapter1AuthoredView(this,source);if(closing)_closingView=port as IChapter1ClosingViewPort;
            if(closing&&_closingView==null)throw new InvalidOperationException("종결 표시 포트가 없습니다.");view.SetActive(true);
        }
        private void StartBarrier()
        {
            if(_choiceAdapter.RunSkillSessionForHandoff==null)throw new InvalidOperationException("확정 선택의 원정 기술 세션이 없습니다.");
            _lifetime.BeginChapter1Barrier(_barrierPrefab,_choiceAdapter.RunSkillSessionForHandoff,_proposedChoice,this);_phase=Chapter1RunPhase.PreparingBarrier;BeginPostBossWait();
        }
        private void AdvancePostBoss()
        {
            if(_lifetime.IsFailed)throw new InvalidOperationException("보스 이후 장면 수명이 중단됐습니다.");
            if(_phase==Chapter1RunPhase.PreparingChoice)
            {
                CheckPostBossWait();var source=_lifetime.TryCompleteChapter1ChoiceHost(this);RecordPostBossEvaluation("choice-host source="+(source==null?"null":"issued"));if(source==null)return;
                if(_choiceAdapter==null)
                {
                    _choiceAdapter=source;
                    if(_profile.Snapshot.Progression.TryGetCommittedPair(out var pair))
                    {_proposedChoice=(int)pair.Choice;source.BindRunSkillSessionForAuthoring(new ChoiceSkillSession(_runKey,(int)pair.Choice,(int)_profile.Snapshot.Progression.ConsentState,(int)pair.Skill));}
                    else BindAuthoredPostBossView(_choicePrefab,source,false);
                    _lifetime.ActivateChapter1UiHost(source);
                }
                var preparation=source.Router.GetComponent<DesktopProfileLaunchAdapterV1>();var prepared=preparation.Chapter1PreparedForUi;RecordPostBossEvaluation("choice-prepared="+prepared);if(!prepared)return;
                if(source.RunSkillSessionForHandoff!=null)StartBarrier();else _phase=Chapter1RunPhase.ChoiceRequired;return;
            }
            if(_phase==Chapter1RunPhase.PreparingBarrier){CheckPostBossWait();var completedBarrier=_lifetime.TryCompleteChapter1Barrier(this);RecordPostBossEvaluation("barrier-complete="+completedBarrier);if(completedBarrier){_phase=Chapter1RunPhase.AtBarrier;_observedGeneration=_lifetime.Generation;_lastTick=null;}return;}
            if(_phase==Chapter1RunPhase.AtBarrier)
            {
                if(!_lifetime.TryGetCompletedEvidence(out var completed))return;_health=completed.Health;_lastTick=completed.Tick;
                if(completed.Health<=0)throw new InvalidOperationException("봉쇄선에서 실제 플레이어 사망이 관측됐습니다.");
                if(completed.Position.y<=-2){_lifetime.BeginChapter1Barrier(_barrierPrefab,_lifetime.CurrentBarrierSkillSession,_proposedChoice,this);_phase=Chapter1RunPhase.PreparingBarrier;BeginPostBossWait();return;}
                var passage=_lifetime.CurrentChapter1Passage;if(passage==null||!_lifetime.ValidatePendingChapter1Passage(passage))return;
                _passage=passage;_lifetime.RequestChapter1SuccessfulClose(this,passage);_phase=Chapter1RunPhase.EndingSuccessfulRun;BeginPostBossWait();return;
            }
            if(_phase==Chapter1RunPhase.EndingSuccessfulRun)
            {
                CheckPostBossWait();var successfulClose=_lifetime.TryGetChapter1SuccessfulClose(out var actual);RecordPostBossEvaluation("successful-close="+successfulClose);if(!successfulClose)return;
                _runEndCommittedRevision=checked(_runEndCommittedRevision+1);_demoReceipt=new Chapter1DemoEndReceipt(this,_runKey,actual.View.Generation,(ProfileBranch)_proposedChoice,_runEndCommittedRevision);
                var expected=checked(_profile.Snapshot.ProfileRevision+1);var saved=_profile.SaveChapter1CompletedBranch((ProfileBranch)_proposedChoice);_lastSaveResult=saved;
                if(!Chapter1DevelopmentProfile.IsExactCommit(saved,expected))throw new InvalidOperationException("완료 분기 저장이 확정되지 않았습니다.");
                _branchSaveReceipt=new Chapter1BranchSaveReceipt(this,_demoReceipt,saved,expected);_lifetime.BeginChapter1ClosingHost(this,_demoReceipt,_branchSaveReceipt);
                _choiceAdapter=null;_phase=Chapter1RunPhase.PreparingClosing;BeginPostBossWait();return;
            }
            if(_phase==Chapter1RunPhase.PreparingClosing)
            {
                CheckPostBossWait();var source=_lifetime.TryCompleteChapter1ClosingHost(this);RecordPostBossEvaluation("closing-host source="+(source==null?"null":"issued"));if(source==null)return;
                if(_choiceAdapter==null){_choiceAdapter=source;BindAuthoredPostBossView(_closingPrefab,source,true);_lifetime.ActivateChapter1UiHost(source);}
                var preparation=source.Router.GetComponent<DesktopProfileLaunchAdapterV1>();var prepared=preparation.Chapter1PreparedForUi;
                RecordPostBossEvaluation("closing-prepared="+prepared+" uiFrame=unobserved-short-circuit");if(!prepared)return;
                var uiFrame=source.Router.CurrentUiFrame;
                RecordPostBossEvaluation("closing-prepared=true uiFramePresent="+uiFrame.HasValue+(uiFrame.HasValue?" receiptTick="+uiFrame.Value.Receipt.Tick.Value+" receiptOrdinal="+uiFrame.Value.Receipt.FrameOrdinal:""));if(!uiFrame.HasValue)return;
                if(!_closingStarted){_closingView.BeginFromCommittedRun(_demoReceipt,_branchSaveReceipt);_closingStarted=true;_phase=Chapter1RunPhase.Closing;}return;
            }
            if(_phase==Chapter1RunPhase.RetiringCompleted)
            {
                CheckPostBossWait();var completedUiClosed=_completedUiClosed();RecordPostBossEvaluation("completed-ui-closed="+completedUiClosed);if(!completedUiClosed)return;
                _choiceAdapter=null;_closingView=null;_runKey=null;_sources=null;_lastTick=null;_observedGeneration=0;_health=0;_plan=default;_roomIndex=0;
                CreateFreshHub();_phase=Chapter1RunPhase.PreparingHub;
            }
        }
        [SerializeField] private Chapter1RoomCatalog _catalog;
        [SerializeField] private Chapter1EncounterLifetimeOwner _lifetime;
        private Chapter1HubLifetimeOwner _hub;
        private Func<bool> _hubClosed;
        private Chapter1DevelopmentProfile _profile;
        private Chapter1RoomPlan _plan;
        private Chapter1FailureSources _sources;
        private RunFailureArbitrationSession _failure;
        private PostBossProgressionSession _postBoss;
        private bool _blockFailure;
        private RunFailureIntent? _acceptedIntent;
        private Chapter1LifecyclePublication _clear;
        private Chapter1RunPhase _phase=Chapter1RunPhase.PreparingHub;
        private string _runKey,_error;
        private int _roomIndex,_health;
        private long _hubGeneration,_lastHubOrdinal,_observedGeneration,_runStartedRevision,_runEndCommittedRevision,_returnedRevision;
        private SimulationTick? _lastTick;
        [SerializeField] private bool _configured;
        private bool _processingRequest,_failureCommitted;
        private Chapter1AcceptedFailureToken _pendingFailureToken;
        private bool _failureTokenConsumed;
        internal Chapter1HubLifetimeOwner CurrentHub => _hub;
        internal Chapter1EncounterLifetimeOwner Lifetime => _lifetime;
        private ProfileAtomicSaveResultV1? _lastSaveResult;
        // REQ-C1B-SRETRY-001/002/007: 결과 미발급과 실제 실패 결과·원래 오류를 구별하여 보존한다.
        private long _expeditionAttemptOrdinal;
        private string _expeditionAuthenticatedRoot;
        private Chapter1ExpeditionStorageAttempt _currentStorageAttempt,_lastStorageFailure;
        private ProfileLaunchObservationBatchV1? _storageRetryObservation;
        private DesktopProfileLaunchReceiptV1? _storageRetryPrepared;
        private Chapter1HubRequestToken _storageRetryToken;
        internal ProfileAtomicSaveResultV1? LastSaveResult=>_lastSaveResult;
        private string _lastChoiceRetryStatus;
        internal string LastChoiceRetryStatus=>_lastChoiceRetryStatus;
        internal ProfileLaunchObservationBatchV1? StorageRetryObservation=>_storageRetryObservation;
        internal DesktopProfileLaunchReceiptV1? StorageRetryPrepared=>_storageRetryPrepared;
        internal Chapter1HubRequestToken StorageRetryToken=>_storageRetryToken;
        internal long RunStartedRevision => _runStartedRevision;
        internal long RunEndCommittedRevision => _runEndCommittedRevision;
        internal long ReturnedToHubRevision => _returnedRevision;
        internal string PlanSha => _runKey==null?null:_plan.CanonicalSha;
        internal Chapter1FailureSourceKind? AcceptedFailureKind => _acceptedIntent.HasValue?(Chapter1FailureSourceKind?)(int)_acceptedIntent.Value.Request.Cause:null;
        internal long FailureArbitrationRevision=>_failure==null?0:_failure.Snapshot.Revision;
        internal int AcceptedFailureCauseCount=>_failure!=null && _failure.Snapshot.AcceptedFailure.HasValue?_failure.Snapshot.AcceptedFailure.Value.ObservedCauses.Count:0;
        internal RunFailureSnapshot FailureSnapshot => _failure==null?default:_failure.Snapshot;
        public bool BlocksNewFailureRequests => _blockFailure;
        public bool ValidatePendingFailure(Chapter1EncounterLifetimeOwner consumer,Chapter1AcceptedFailureToken token)
        {
            if(consumer==null || !ReferenceEquals(consumer,_lifetime) || token==null || !ReferenceEquals(token,_pendingFailureToken) || _failureTokenConsumed ||
                !ReferenceEquals(token.Issuer,this) || !ReferenceEquals(token.Consumer,_lifetime) || _phase!=Chapter1RunPhase.Failing || _blockFailure ||
                !_acceptedIntent.HasValue || _failure==null || _sources==null || !ReferenceEquals(token.Router,_lifetime.CurrentRouter) ||
                token.RunKey!=_runKey || token.Generation!=_observedGeneration || !ReferenceEquals(token.Health,_sources.LastHealthPublication) ||
                !ReferenceEquals(token.Kill,_sources.LastKillPublication) || !ReferenceEquals(token.Crush,_sources.LastCrushPublication))return false;
            var intent=_acceptedIntent.Value;var snapshot=_failure.Snapshot;
            return snapshot.Phase==RunFailurePhase.Failed && snapshot.AcceptedFailure.HasValue && snapshot.Revision==token.ArbitrationRevision &&
                snapshot.RunInstanceKey==_runKey && snapshot.AcceptedFailure.Value.Tick==token.SourceReceipt.Tick &&
                (int)snapshot.AcceptedFailure.Value.Cause==(int)token.Cause && intent.SourceRevision==token.ArbitrationRevision &&
                intent.RunInstanceKey==_runKey && intent.Request.Tick==token.SourceReceipt.Tick && (int)intent.Request.Cause==(int)token.Cause &&
                token.Health.Receipt.Equals(token.SourceReceipt) && token.Kill.Receipt.Equals(token.SourceReceipt) && token.Crush.Receipt.Equals(token.SourceReceipt);
        }
        public void ConsumePendingFailure(Chapter1EncounterLifetimeOwner consumer,Chapter1AcceptedFailureToken token)
        {
            if(!ValidatePendingFailure(consumer,token))throw new InvalidOperationException("현재 실패 pending 원문만 한 번 소비합니다.");
            _failureTokenConsumed=true;_pendingFailureToken=null;
        }
        internal PostBossSnapshot PostBossSnapshot => _postBoss==null?default:_postBoss.Snapshot;
        internal Chapter1RunView View => new Chapter1RunView(_phase,_runKey,RoomId,_roomIndex,_plan.Seed,_health,
            _observedGeneration,_lastTick,_runEndCommittedRevision,_error,_hub!=null && _hub.Ready,_currentStorageAttempt,_lastStorageFailure);
        private string RoomId => _runKey==null?null:_roomIndex<4?_plan.RoomAt(_roomIndex):"Ordan";
        internal void ConfigureForAuthoring(GameObject hubPrefab,GameObject ordanPrefab,Chapter1RoomCatalog catalog,Chapter1EncounterLifetimeOwner lifetime)
        {
            if(gameObject.activeInHierarchy || _configured || hubPrefab==null || hubPrefab.activeSelf || ordanPrefab==null || ordanPrefab.activeSelf || catalog==null || lifetime==null)
                throw new InvalidOperationException("원정 소유자는 비활성 저작 경계에서 한 번 구성합니다.");
            _hubPrefab=hubPrefab;_ordanPrefab=ordanPrefab;_catalog=catalog;_lifetime=lifetime;_configured=true;
        }
        private void Start(){try{if(!_configured)throw new InvalidOperationException("원정 저작 구성이 없습니다.");_catalog.Validate(false);CreateFreshHub();}catch(Exception e){Stop(e);}}
        public void HandleHubRequest(Chapter1HubLifetimeOwner source,Chapter1HubRequestToken token)
        {
            if(_processingRequest || source==null || !ReferenceEquals(source,_hub) || token==null ||
                token.Generation!=_hubGeneration || token.Ordinal<=_lastHubOrdinal ||
                (_phase!=Chapter1RunPhase.Hub && _phase!=Chapter1RunPhase.ReturnedToHub && _phase!=Chapter1RunPhase.ExpeditionStorageFailed))return;
            try{if(!source.ValidatePendingRequest(this,token))return;}catch(Exception e){Stop(e);return;}
            var retry=token.Intent=="chapter1.retry-expedition-storage";
            if((_phase==Chapter1RunPhase.ExpeditionStorageFailed)!=retry || (retry && token.Seed.HasValue))return;
            if(token.Intent=="chapter1.return-to-hub")
            {
                if(_phase!=Chapter1RunPhase.ReturnedToHub)return;
                source.ConsumePendingRequest(this,token);_lastHubOrdinal=token.Ordinal;_phase=Chapter1RunPhase.Hub;return;
            }
            // 재진입 차단과 일회 객체 소비가 실제 저장보다 먼저다.
            _processingRequest=true;_lastHubOrdinal=token.Ordinal;
            try{source.ConsumePendingRequest(this,token);if(retry)RequestStorageRetry(token);else RequestExpedition(token);}catch(Exception e){Stop(e);}finally{_processingRequest=false;}
        }
        private void RequestExpedition(Chapter1HubRequestToken token)
        {
            if(!_processingRequest || _hub==null || (_phase!=Chapter1RunPhase.Hub && _phase!=Chapter1RunPhase.ReturnedToHub))
                throw new InvalidOperationException("출정은 실제 거점 요청 포트에서만 진행합니다.");
            _phase=Chapter1RunPhase.Saving;
            _lastSaveResult=null;_profile=null;_expeditionAuthenticatedRoot=null;
            _expeditionAttemptOrdinal=checked(_expeditionAttemptOrdinal+1);
            _currentStorageAttempt=new Chapter1ExpeditionStorageAttempt(_expeditionAttemptOrdinal,null,token,_runStartedRevision,null,null,null,null,null,null);
            // 인증·catalog·공개 Observe 예외는 복구 가능한 파일 읽기 결과로 변환하지 않는다.
            var root=Chapter1DevelopmentProfile.ValidateExactRoot(_hub.Adapter.GetAuthenticatedLaunchRootForNewGame(_hub.Router));
            _expeditionAuthenticatedRoot=root;
            _catalog.Validate(true);
            var observed=ProfileLaunchObservationAdapterV1.Observe(root);observed.Validate();
            RecordStorageAttempt(observed,null,null,null,null,null);
            if(HasUnreadable(observed))
            {
                FailExpeditionStorage(Chapter1ExpeditionStorageFailureKind.ReadUnavailableNoSaveIssued,
                    "저장 파일을 읽을 수 없습니다. 주 저장: "+ReadStateCopy(observed.Primary.Kind)+", 이전 저장: "+ReadStateCopy(observed.Previous.Kind)+", 임시 저장: "+ReadStateCopy(observed.Temp.Kind));
                return;
            }
            _profile=new Chapter1DevelopmentProfile(_hub);
            var explicitSeed=token.Seed;
            var before=_profile.Snapshot;_plan=_catalog.Select(explicitSeed,before.Progression.LastOfferedSeed);
            var expected=checked(before.ProfileRevision+1);RecordStorageAttempt(observed,_plan,expected,null,null,null);
            var saved=_profile.SaveForExpedition(_plan.Seed,explicitSeed.HasValue);_lastSaveResult=saved;
            saved.Validate();RecordStorageAttempt(observed,_plan,expected,saved,null,null);
            if(saved.Outcome==ProfileAtomicSaveOutcome.FailedBeforeCommit)
            {
                FailExpeditionStorage(Chapter1ExpeditionStorageFailureKind.FailedBeforeCommit,
                    "저장이 확정되기 전에 중단되었습니다. "+SaveStageCopy(saved.FailureStage));
                return;
            }
            if(!Chapter1DevelopmentProfile.IsExactCommit(saved,expected))throw new InvalidOperationException("출정 저장이 확정되지 않았습니다.");
            _runKey=Guid.NewGuid().ToString("N");_roomIndex=0;_health=5;_lastTick=null;_observedGeneration=0;
            _failure=new RunFailureArbitrationSession();_postBoss=new PostBossProgressionSession();_blockFailure=false;
            _demoReceipt=null;_branchSaveReceipt=null;_chapterCompletion=null;_choiceAdapter=null;_closingView=null;_closingStarted=false;_passage=null;_proposedChoice=0;
            _acceptedIntent=null;_failureCommitted=false;_clear=null;_sources=null;_pendingFailureToken=null;_failureTokenConsumed=false;
            _hubClosed=_hub.BeginRetirement();_hub=null;_phase=Chapter1RunPhase.RetiringHub;
        }
        private static bool HasUnreadable(ProfileLaunchObservationBatchV1 value)=>value.Primary.Kind==ProfileLoadCandidateKind.Unreadable ||
            value.Previous.Kind==ProfileLoadCandidateKind.Unreadable || value.Temp.Kind==ProfileLoadCandidateKind.Unreadable;
        private static string ReadStateCopy(ProfileLoadCandidateKind value)=>value==ProfileLoadCandidateKind.Unreadable?"읽기 불가":value==ProfileLoadCandidateKind.Missing?"없음":"판독 완료";
        private static string SaveStageCopy(ProfileAtomicSaveStage value)
        {
            switch(value){case ProfileAtomicSaveStage.DirectoryPreparation:return "저장 폴더를 준비하지 못했습니다.";
                case ProfileAtomicSaveStage.TempWriteFlushClose:return "임시 저장 파일을 기록하지 못했습니다.";
                case ProfileAtomicSaveStage.TempReadValidation:return "임시 저장 파일을 검증하지 못했습니다.";
                case ProfileAtomicSaveStage.ExistingPrimaryValidation:return "기존 저장 파일을 검증하지 못했습니다.";
                default:throw new InvalidOperationException("저장 확정 전 실패 단계가 아닙니다.");}
        }
        private void RecordStorageAttempt(ProfileLaunchObservationBatchV1? observed,Chapter1RoomPlan? plan,long? expected,
            ProfileAtomicSaveResultV1? saved,Chapter1ExpeditionStorageFailureKind? failure,string error)
        {
            var before=_currentStorageAttempt;
            _currentStorageAttempt=new Chapter1ExpeditionStorageAttempt(before.Ordinal,_expeditionAuthenticatedRoot,before.Token,before.StartedRevision,observed,plan,expected,saved,failure,error);
        }
        private void FailExpeditionStorage(Chapter1ExpeditionStorageFailureKind kind,string cause)
        {
            var attempt=_currentStorageAttempt;
            if(_phase!=Chapter1RunPhase.Saving || attempt==null || _hub==null || _runKey!=null || _lifetime.CurrentRouter!=null ||
                _runStartedRevision!=attempt.StartedRevision || !ReferenceEquals(attempt.Token.Owner,_hub) || !ReferenceEquals(attempt.Token.Router,_hub.Router))
                throw new InvalidOperationException("시작 전 실제 출정 실패 경계가 아닙니다.");
            var error="저장에 실패하여 출정하지 못했습니다\n"+cause;
            RecordStorageAttempt(attempt.Observation,attempt.Plan,attempt.ExpectedRevision,attempt.Save,kind,error);
            _lastStorageFailure=_currentStorageAttempt;_error=error;
            // 실제 dispatch 안에서 이미 소비한 원문만 한 번 중립 재무장한다.
            _hub.ArmExpeditionRetryAfterFailure(this,attempt.Token);
            _phase=Chapter1RunPhase.ExpeditionStorageFailed;Debug.LogError("출정 저장 실패: "+cause,this);
        }
        private void RequestStorageRetry(Chapter1HubRequestToken token)
        {
            if(!_processingRequest || _phase!=Chapter1RunPhase.ExpeditionStorageFailed || _hub==null || _lastStorageFailure==null ||
                _runKey!=null || _lifetime.CurrentRouter!=null || _runStartedRevision!=_lastStorageFailure.StartedRevision)
                throw new InvalidOperationException("저장 재확인은 시작 전 실패 상태의 새 실제 요청만 받습니다.");
            _storageRetryToken=token;_storageRetryObservation=null;_storageRetryPrepared=null;
            var root=Chapter1DevelopmentProfile.ValidateExactRoot(_hub.Adapter.GetAuthenticatedLaunchRootForNewGame(_hub.Router));
            var observed=ProfileLaunchObservationAdapterV1.Observe(root);observed.Validate();_storageRetryObservation=observed;
            if(HasUnreadable(observed))throw new InvalidOperationException("재확인에서도 실제 저장 파일을 읽을 수 없습니다.");
            _hubClosed=_hub.BeginRetirement();_hub=null;_phase=Chapter1RunPhase.RetiringHubForStorageRetry;
        }
        private void ObserveStorageRetryPreparation()
        {
            if(_hub==null || _hub.Router==null || _hub.Router.Chapter1HubPreparationBlocked)
                throw new InvalidOperationException("새 거점의 실제 저장 준비가 중단되었습니다.");
            var prepared=_hub.Adapter.CurrentReceipt;
            if(!prepared.HasValue)return;
            prepared.Value.Validate();_storageRetryPrepared=prepared;
            if(prepared.Value.PersistencePending)throw new InvalidOperationException("새 거점의 실제 준비 저장이 미확정입니다.");
            Chapter1DevelopmentProfile.ValidateExactRoot(_hub.Adapter.GetAuthenticatedLaunchRootForNewGame(_hub.Router));
            if(_hub.Ready)_phase=Chapter1RunPhase.Hub;
        }
        private void Update()
        {
            if(_phase==Chapter1RunPhase.Diagnostic)return;
            try
            {
                if(_phase==Chapter1RunPhase.PreparingHub && _hub!=null && _hub.Ready)_phase=Chapter1RunPhase.Hub;
                if(_phase==Chapter1RunPhase.RetiringHubForStorageRetry && _hubClosed!=null && _hubClosed())
                {
                    CreateFreshHub();_phase=Chapter1RunPhase.PreparingHubAfterStorageRetry;
                    ObserveStorageRetryPreparation();
                }
                if(_phase==Chapter1RunPhase.PreparingHubAfterStorageRetry)ObserveStorageRetryPreparation();
                if(_phase==Chapter1RunPhase.RetiringHub && _hubClosed!=null && _hubClosed())
                {
                    var room=_catalog.Prefab(RoomId).GetComponent<Chapter1RoomInstance>();
                    _lifetime.BeginChapter1Expedition(_catalog.Prefab(RoomId),_runKey,room.Entry,room.Hazard,_plan.SocketAt(0),this);
                    _phase=Chapter1RunPhase.StartingRoom;
                }
                if(_phase==Chapter1RunPhase.CommittedFailure && _lifetime.Chapter1GraphRetired)
                {
                    _phase=Chapter1RunPhase.SavingReturn;
                    var expected=checked(_profile.Snapshot.ProfileRevision+1);var saved=_profile.SaveAfterFailure();_lastSaveResult=saved;
                    if(!Chapter1DevelopmentProfile.IsExactCommit(saved,expected))throw new InvalidOperationException("실패 복귀 저장이 확정되지 않았습니다.");
                    _returnedRevision=checked(_returnedRevision+1);
                    _sources=null;_lastTick=null;_observedGeneration=0;_health=0;_clear=null;
                    _runKey=null;_plan=default;_roomIndex=0;_acceptedIntent=null;
                    CreateFreshHub();_phase=Chapter1RunPhase.ReturnedToHub;
                }
            }
            catch(Exception e){Stop(e);}
        }
        private void FixedUpdate()
        {
            if(_phase>=Chapter1RunPhase.PreparingChoice)
            {try{AdvancePostBoss();}catch(Exception error){Stop(error);}return;}
            if(_phase==Chapter1RunPhase.Diagnostic || _phase==Chapter1RunPhase.Hub || _phase==Chapter1RunPhase.PreparingHub ||
                _phase==Chapter1RunPhase.ReturnedToHub || _phase==Chapter1RunPhase.RetiringHub || _phase==Chapter1RunPhase.CommittedFailure || _phase==Chapter1RunPhase.LockedPostBoss ||
                _phase==Chapter1RunPhase.ExpeditionStorageFailed || _phase==Chapter1RunPhase.RetiringHubForStorageRetry || _phase==Chapter1RunPhase.PreparingHubAfterStorageRetry)return;
            try
            {
                if(_lifetime.IsFailed)throw new InvalidOperationException("장면 수명 소유자가 중단되었습니다.");
                if(_phase==Chapter1RunPhase.BossCleanup){ObserveBossCleanup();return;}
                if(_phase==Chapter1RunPhase.Failing)
                {
                    if(!_lifetime.TryGetChapter1LifecyclePublication(Chapter1LifecycleKind.RunFailed,out var clear))return;
                    ValidateClear(clear,Chapter1LifecycleKind.RunFailed);
                    if(!_acceptedIntent.HasValue || _failureCommitted)throw new InvalidOperationException("실패 종료 의도가 없거나 이미 소비되었습니다.");
                    // 실제 Transition와 RunFailed clear 이후 commit, 그 이후에만 파기를 허용한다.
                    var committedRevision=checked(_runEndCommittedRevision+1);
                    _lifetime.CommitChapter1FailedRun(clear,_acceptedIntent.Value.SourceRevision);
                    _failureCommitted=true;_runEndCommittedRevision=committedRevision;_clear=clear;
                    _phase=Chapter1RunPhase.CommittedFailure;return;
                }
                if(!_lifetime.TryGetCompletedEvidence(out var completed))return;
                if(_lastTick.HasValue && _observedGeneration==completed.Generation && completed.Tick==_lastTick.Value)return;
                if(completed.RunKey!=_runKey)throw new InvalidOperationException("다른 원정 완료 관측입니다.");
                var first=_observedGeneration!=completed.Generation;
                if(first)
                {
                    if(_observedGeneration!=0 && completed.Generation!=checked(_observedGeneration+1))throw new InvalidOperationException("방 세대가 연속하지 않습니다.");
                    _sources=new Chapter1FailureSources(_lifetime.CurrentRouter,_lifetime.CurrentHazards,_runKey,completed.Generation);
                    if(_failure.Snapshot.Phase==RunFailurePhase.NotStarted)_failure.Start(new RunStartInput(_runKey,completed.Tick));
                    _observedGeneration=completed.Generation;
                }
                var result=_failure.ObserveTick(_sources.CloseCompleted(completed));
                _lastTick=completed.Tick;_health=completed.Health;
                if(result.NewlyEmittedIntents.Count!=0)
                {
                    _acceptedIntent=result.NewlyEmittedIntents[0];_phase=Chapter1RunPhase.Failing;
                    if(_roomIndex==4)ObserveBossDeath(completed); // 동일 tick 실패 중재가 반드시 먼저다.
                    var accepted=_acceptedIntent.Value;
                    if(accepted.Request.Result!=RunResult.Failed || accepted.RunInstanceKey!=_runKey || accepted.Request.Tick!=completed.Tick || _blockFailure)
                        throw new InvalidOperationException("실제 실패 중재 의도가 아닙니다.");
                    _pendingFailureToken=new Chapter1AcceptedFailureToken(this,_lifetime,_lifetime.CurrentRouter,_runKey,completed.Generation,
                        completed.Receipt,accepted.SourceRevision,(Chapter1FailureSourceKind)(int)accepted.Request.Cause,
                        _sources.LastHealthPublication,_sources.LastKillPublication,_sources.LastCrushPublication);
                    _lifetime.RequestChapter1FailureTransition(this,_pendingFailureToken);return;
                }
                if(first)
                {
                    if(_roomIndex==0 && _phase==Chapter1RunPhase.StartingRoom)_runStartedRevision=checked(_runStartedRevision+1);
                    _phase=_roomIndex<4?Chapter1RunPhase.InRoom:Chapter1RunPhase.AtBoss;
                }
                if(_phase==Chapter1RunPhase.InRoom)ObserveRoomExit(RoomId,completed.Generation,completed.Tick);
                if(_phase==Chapter1RunPhase.AtBoss)ObserveBossDeath(completed);
                // B1은 M5A 잠긴 인계까지만 보존하고 선택·봉쇄선·완료를 게시하지 않는다.
            }
            catch(Exception e){Stop(e);}
        }
        private void ObserveRoomExit(string roomId,long generation,SimulationTick tick)
        {
            if(_phase!=Chapter1RunPhase.InRoom || roomId!=RoomId || generation!=_observedGeneration ||
                !_lifetime.TryGetCompletedEvidence(out var completed) || completed.Tick!=tick || completed.Generation!=generation ||
                completed.RunKey!=_runKey)return;
            var room=_catalog.Prefab(roomId).GetComponent<Chapter1RoomInstance>();
            if(completed.AliveEnemyCount!=0 || !room.Exit.Contains(completed.Position))return;
            _phase=Chapter1RunPhase.LeavingRoom;_roomIndex++;
            if(_roomIndex<4)
            {
                var next=_catalog.Prefab(RoomId).GetComponent<Chapter1RoomInstance>();
                _lifetime.RequestChapter1RoomReplacement(_catalog.Prefab(RoomId),next.Entry,next.Hazard,_plan.SocketAt(_roomIndex));
            }
            else _lifetime.RequestChapter1BossReplacement(_ordanPrefab,new Vector2(-7,0.81f),new Chapter1HazardDefinition("Ordan",true),this);
        }
        private void ObserveBossDeath(Chapter1EncounterEvidenceView completed)
        {
            if(!_lifetime.TryGetChapter1BossPublication(out var actual) || actual.CleanupTick.HasValue)return;
            ValidateBossSource(actual);
            if(actual.DeathTick!=completed.Tick || actual.PlayerHealth!=completed.Health)return;
            var ordered=actual.OrderedHandoffs;
            if(ordered==null || ordered.Length!=3)throw new InvalidOperationException("실제 보스 handoff3가 없습니다.");
            var handoffs=new PostBossHandoff[3];for(var i=0;i<3;i++)handoffs[i]=new PostBossHandoff((PostBossHandoffKind)ordered[i],actual.DeathTick);
            var failure=_failure.Snapshot;PostBossFailure? accepted=null;
            if(failure.AcceptedFailure.HasValue)accepted=new PostBossFailure((PostBossFailureCause)failure.AcceptedFailure.Value.Cause,failure.AcceptedFailure.Value.Tick);
            var progression=_profile.Snapshot.Progression;progression.Validate();PostBossPair pair;
            if(progression.TryGetCommittedPair(out var persisted))
                pair=new PostBossPair(persisted.Choice==ProfileChoice.Extraction?PostBossChoice.Extraction:PostBossChoice.Solidarity,
                    persisted.Skill==ProfileSkill.CompressionVerdict?PostBossSkill.CompressionVerdict:PostBossSkill.CommonReferencePlane);
            else pair=new PostBossPair(PostBossChoice.None,PostBossSkill.None); // 저장 검증 결과의 실제 미확정 pair다.
            var result=_postBoss.ObserveDeath(new PostBossDeathInput(PostBossProgressionSession.OrdanEncounterKey,actual.DeathTick,handoffs,
                actual.BossHealth,actual.BossDead,actual.PlayerHealth,actual.PlayerDead,failure.RunActive,accepted,pair));
            foreach(var intent in result.Intents)if(intent.BlockNewFailureRequests)_blockFailure=true;
            if(_blockFailure)_phase=Chapter1RunPhase.BossCleanup;
        }
        private void ObserveBossCleanup()
        {
            if(!_lifetime.TryGetChapter1BossPublication(out var actual) || !actual.CleanupTick.HasValue)return;
            ValidateBossSource(actual);
            var result=_postBoss.CompleteCleanup(new PostBossCleanupInput(PostBossProgressionSession.OrdanEncounterKey,actual.DeathTick,actual.CleanupTick.Value,
                actual.LifecycleSuperseded?PostBossCleanupDisposition.LifecycleSuperseded:PostBossCleanupDisposition.Removed,
                actual.Lifecycle.HasValue?(PostBossLifecycleReason?)(actual.Lifecycle.Value==Chapter1LifecycleKind.RunFailed?PostBossLifecycleReason.RunFailed:PostBossLifecycleReason.RoomLeaving):null,
                actual.RegistrationsAbsent,actual.DurableRemovalCommitted,actual.PlayerBaseline,actual.ActiveTargetEmpty,actual.BossTeardownCommitted));
            if(result.Snapshot.Phase==PostBossPhase.Suppressed){_phase=Chapter1RunPhase.LockedPostBoss;return;}
            if(result.Snapshot.Phase==PostBossPhase.ChoiceRequired || result.Snapshot.Phase==PostBossPhase.BarrierPreparationRequired)
            {
                if(_choicePrefab==null||_barrierPrefab==null||_closingPrefab==null)throw new InvalidOperationException("보스 이후 생산 저작 참조가 없습니다.");
                _lifetime.BeginChapter1ChoiceHost(this);_phase=Chapter1RunPhase.PreparingChoice;BeginPostBossWait();
            }
        }
        private void ValidateBossSource(Chapter1BossPublication value)
        {
            if(value==null || !_lifetime.OwnsChapter1BossPublication(value) || value.RunKey!=_runKey || value.Generation!=_observedGeneration)
                throw new InvalidOperationException("보스 publication이 실제 현재 requester 소유가 아닙니다.");
        }
        private void ValidateClear(Chapter1LifecyclePublication value,Chapter1LifecycleKind kind)
        {
            if(value==null || !_lifetime.OwnsChapter1LifecyclePublication(value) || !ReferenceEquals(value.Owner,_lifetime) ||
                value.RunKey!=_runKey || value.Generation!=_observedGeneration || value.Kind!=kind || value.LockedReceipt.Mode!=InputMode.Transition ||
                !_lastTick.HasValue || value.LockedReceipt.Tick.Value<=_lastTick.Value.Value)
                throw new InvalidOperationException("실제 잠금·전이 정리 영수증이 아닙니다.");
        }
        private void CreateFreshHub()
        {
            var staging=new GameObject("Chapter1HubStage");staging.SetActive(false);
            try
            {
                var instance=Instantiate(_hubPrefab,staging.transform,false);instance.name="Chapter1Hub";
                _hub=instance.GetComponent<Chapter1HubLifetimeOwner>();if(_hub==null)throw new InvalidOperationException("거점 소유자가 없습니다.");
                _hubGeneration=checked(_hubGeneration+1);_lastHubOrdinal=0;
                _hub.ConfigureInstanceGeneration(_hubGeneration);_hub.BindRequestPort(this);
                instance.SetActive(true);instance.transform.SetParent(transform,false);Destroy(staging);
            }
            catch{Destroy(staging);throw;}
        }
        private void Stop(Exception e)
        {
            if(_lifetime!=null)_lifetime.CloseChapter1PostBossAfterFailure();
            // REQ-C1B-SRETRY-002/005: 복구 중 새 오류도 원래 실패와 실제 준비 사실을 지우지 않는다.
            var storageRecovery=_phase==Chapter1RunPhase.ExpeditionStorageFailed || _phase==Chapter1RunPhase.RetiringHubForStorageRetry || _phase==Chapter1RunPhase.PreparingHubAfterStorageRetry;
            if(_phase==Chapter1RunPhase.Saving && _currentStorageAttempt!=null)
            {
                var attempt=_currentStorageAttempt;
                RecordStorageAttempt(attempt.Observation,attempt.Plan,attempt.ExpectedRevision,attempt.Save,attempt.Failure,e.Message);
            }
            _phase=Chapter1RunPhase.Diagnostic;_error=(_lastStorageFailure==null?"":_lastStorageFailure.Error+"\n")+e.Message;
            if(storageRecovery && _hub!=null){try{_hubClosed=_hub.BeginRetirement();_hub=null;}catch(Exception close){_error+="\n"+close.Message;}}
            Debug.LogError("원정 연결 중단: "+e.Message,this);
        }
    }
}
