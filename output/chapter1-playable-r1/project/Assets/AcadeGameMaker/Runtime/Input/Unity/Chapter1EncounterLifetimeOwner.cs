using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Combat.Unity;
using UnityEngine;
namespace AcadeGameMaker.Input.Unity
{
    // REQ/AC-R01PLAY-006: dependency points from Camera.Unity to Input.Unity only.
    internal interface IChapter1CameraBootstrap
    {
        void PrepareChapter1Bootstrap(Chapter1EncounterLifetimeOwner owner,GameObject graph,InputRouter router,PlayerMovementController player,SimulationTick firstTick);
        void CompleteChapter1Bootstrap(Chapter1EncounterLifetimeOwner owner,GameObject graph,InputRouter router,PlayerMovementController player,SimulationTick firstTick);
    }
    // REQ-C1B-ENTRY-001: single graph creation/replacement owner; no Run or failure authority.
    [DefaultExecutionOrder(200)]
    [DisallowMultipleComponent]
    internal sealed class Chapter1EncounterLifetimeOwner : MonoBehaviour
    {
        private GameObject _postBossUi;
        private Chapter1ChoicePhaseAdapter _postBossAdapter;
        private Func<bool> _postBossClosure;
        private double _postBossCloseTime;
        private int _postBossCloseFrame;
        private Chapter1ClosingHostEntry _pendingClosingHost,_issuedClosingHost;
        private InputRouter _closingRouter;
        private object _successfulDemo,_successfulSave;
        private Chapter1SuccessfulClose _successfulClose;
        private object _successfulPassage;
        private SimulationTick _successfulEndTick;
        internal sealed class Chapter1SuccessfulClose
        {
            internal Chapter1SuccessfulClose(Chapter1EncounterLifetimeOwner owner,object passage,Chapter1EncounterEvidenceView view)
            {Owner=owner;Passage=passage;View=view;}
            internal readonly Chapter1EncounterLifetimeOwner Owner;internal readonly object Passage;internal readonly Chapter1EncounterEvidenceView View;
        }
        internal void RequestChapter1SuccessfulClose(IChapter1ChoiceUiPort endpoint,object passage)
        {
            if(!ReferenceEquals(endpoint,_port)||!endpoint.ValidatePendingChapter1SuccessfulClose(passage)||!ValidatePendingChapter1Passage(passage)||_successfulPassage!=null)
                throw new InvalidOperationException("현재 실제 통과 원문이 아닙니다.");
            var tick=new SimulationTick(checked(_latest.Tick.Value+1));
            QueueLifecycle(tick,AcadeGameMaker.Transfer.TransferClearReason.DemoCompleted);
            _router.RequestMode(this,InputMode.Ended,"chapter1-barrier-completed",tick);_successfulPassage=passage;_successfulEndTick=tick;
        }
        internal bool TryGetChapter1SuccessfulClose(out Chapter1SuccessfulClose value)
        {value=_successfulClose;return value!=null && ReferenceEquals(value.Owner,this)&&ReferenceEquals(value.Passage,_successfulPassage)&&_hasCompleted&&value.View.Generation==_generation;}
        internal void BeginChapter1ClosingHost(IChapter1ChoiceUiPort endpoint,object demo,object save)
        {
            if(!ReferenceEquals(endpoint,_port)||_successfulClose==null||_pendingClosingHost!=null||_issuedClosingHost!=null||_router==null||
                !endpoint.ValidatePendingChapter1SuccessfulRun(demo,save))throw new InvalidOperationException("실제 원정 종료와 확정 저장이 없습니다.");
            _successfulDemo=demo;_successfulSave=save;
            _pendingClosingHost=new Chapter1ClosingHostEntry(this,endpoint,_runKey,demo,save,_generation,_successfulClose.View.Receipt,_successfulClose.View.Tick,_successfulClose);
            if(!endpoint.ValidatePendingClosingHostEntry(_pendingClosingHost))throw new InvalidOperationException("종결 pending 입구의 양측 신원이 다릅니다.");
            _postBossClosure=_router.GetChapter1ClosureReader();_postBossCloseTime=Time.realtimeSinceStartupAsDouble;_postBossCloseFrame=Time.frameCount;
            var old=_graph;_graph=null;_router=null;old.SetActive(false);Destroy(old);
        }
        internal Chapter1ChoicePhaseAdapter TryCompleteChapter1ClosingHost(IChapter1ChoiceUiPort endpoint)
        {
            if(_issuedClosingHost!=null)return _postBossAdapter;
            if(_pendingClosingHost==null||!ReferenceEquals(endpoint,_port))throw new InvalidOperationException("종결 pending 입구가 없습니다.");
            if(!_postBossClosure())
            {
                // REQ-AUTOCH1-WAIT-001: preserve the original start and closure evaluation.
                var now=Time.realtimeSinceStartupAsDouble;var elapsed=now-_postBossCloseTime;var frames=Time.frameCount-_postBossCloseFrame;
                if(double.IsNaN(now)||double.IsInfinity(now)||double.IsNaN(_postBossCloseTime)||double.IsInfinity(_postBossCloseTime)||double.IsNaN(elapsed)||double.IsInfinity(elapsed)||elapsed<0||elapsed>=5)
                    throw new InvalidOperationException("종결 전 입력 정리 기한이 지났습니다. elapsed="+elapsed.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+" deltaFrame="+frames);
                return null;
            }
            return CreateChapter1UiHost(endpoint,_pendingClosingHost);
        }
        internal Func<bool> RetireChapter1ClosingHost(IChapter1ChoiceUiPort endpoint,object completion)
        {
            if(_postBossUi==null||_issuedClosingHost==null||!ReferenceEquals(endpoint,_port)||!endpoint.ValidateCommittedChapter1Completion(_postBossAdapter,_issuedClosingHost,completion))
                throw new InvalidOperationException("원본 완료의 현재 종료 호스트가 아닙니다.");
            var closed=_closingRouter.GetChapter1ClosureReader();var host=_postBossUi;_postBossUi=null;_postBossAdapter=null;_closingRouter=null;host.SetActive(false);Destroy(host);_retired=true;return closed;
        }
        private GameObject _barrierPrefab;
        private object _barrierSkillSession;
        internal object CurrentBarrierSkillSession=>_barrierSkillSession;
        internal void CloseChapter1PostBossAfterFailure()
        {
            if(_postBossUi!=null){_postBossUi.SetActive(false);Destroy(_postBossUi);_postBossUi=null;_postBossAdapter=null;}
            if(_barrierEnvironment!=null && _graph!=null){_graph.SetActive(false);Destroy(_graph);_graph=null;_router=null;}
        }
        private int _barrierBranch;
        private Chapter1ChoiceEnvironmentOwner _barrierEnvironment;
        private Collider2D _barrierExit,_barrierPlayerBody;
        private Func<bool> _barrierOldClosure;
        private GameObject _barrierOldGraph;
        private double _barrierCloseTime;
        private int _barrierCloseFrame;
        private bool _barrierRetirementRequested;
        private SimulationTick _barrierRetirementTick;
        private Chapter1BarrierPassage _barrierPassage;
        internal sealed class Chapter1BarrierPassage
        {
            internal Chapter1BarrierPassage(Chapter1EncounterLifetimeOwner owner,Chapter1EncounterEvidenceView view,object capture)
            {Owner=owner;View=view;Capture=capture;}
            internal readonly Chapter1EncounterLifetimeOwner Owner;internal readonly Chapter1EncounterEvidenceView View;internal readonly object Capture;
        }
        internal bool ValidatePendingChapter1Passage(object value)=>value!=null && ReferenceEquals(value,_barrierPassage) && _barrierEnvironment!=null &&
            _barrierEnvironment.HasActualSkill && _barrierEnvironment.GateOpened && _router!=null && _barrierPassage.View.Generation==_generation &&
            _barrierPassage.View.RunKey==_runKey && _router.BoundTransferForAuthoring.ValidateChapter1CommittedCapture(_barrierPassage.View.Tick,_barrierPassage.Capture);
        internal object CurrentChapter1Passage=>_barrierPassage;
        internal void BeginChapter1Barrier(GameObject prefab,object session,int branch,IChapter1ChoiceUiPort endpoint)
        {
            if(prefab==null||prefab.activeSelf||session==null||endpoint==null||!ReferenceEquals(endpoint,_port)||(branch!=1&&branch!=2)||_barrierOldClosure!=null||_barrierRetirementRequested||!_hasCompleted||_latest.Health<=0)
                throw new InvalidOperationException("봉쇄선 승계 준비가 유효하지 않습니다.");
            _barrierPrefab=prefab;_barrierSkillSession=session;_barrierBranch=branch;
            if(_postBossUi==null)
            {
                if(_router==null||_barrierEnvironment==null)throw new InvalidOperationException("실제 봉쇄선 재진입 graph가 없습니다.");
                _barrierRetirementTick=new SimulationTick(checked(_latest.Tick.Value+1));
                QueueLifecycle(_barrierRetirementTick,AcadeGameMaker.Transfer.TransferClearReason.RoomLeaving);
                _router.RequestMode(this,InputMode.Transition,"chapter1-barrier-safe-return",_barrierRetirementTick);_barrierRetirementRequested=true;return;
            }
            CloseBarrierGraphForReplacement();
        }
        private void CloseBarrierGraphForReplacement()
        {
            var old=_postBossUi!=null?_postBossUi:_graph;
            var oldRouter=_postBossUi!=null?_postBossAdapter.Router:_router;
            if(old==null||oldRouter==null)throw new InvalidOperationException("실제 이전 graph가 없습니다.");
            _barrierOldClosure=oldRouter.GetChapter1ClosureReader();_barrierOldGraph=old;_barrierCloseTime=Time.realtimeSinceStartupAsDouble;_barrierCloseFrame=Time.frameCount;
            old.SetActive(false);Destroy(old);_postBossUi=null;_postBossAdapter=null;_graph=null;_router=null;
            _barrierRetirementRequested=false;
        }
        internal bool TryCompleteChapter1Barrier(IChapter1ChoiceUiPort endpoint)
        {
            if(_barrierRetirementRequested)return false;
            if(_barrierOldClosure==null)return _barrierEnvironment!=null && _router!=null;
            if(!_barrierOldClosure() || _barrierOldGraph!=null)
            {
                // REQ-AUTOCH1-WAIT-001: preserve the original start and closure evaluation.
                var now=Time.realtimeSinceStartupAsDouble;var elapsed=now-_barrierCloseTime;var frames=Time.frameCount-_barrierCloseFrame;
                if(double.IsNaN(now)||double.IsInfinity(now)||double.IsNaN(_barrierCloseTime)||double.IsInfinity(_barrierCloseTime)||double.IsNaN(elapsed)||double.IsInfinity(elapsed)||elapsed<0||elapsed>=5)
                    throw new InvalidOperationException("이전 입력 graph 정리 기한이 지났습니다. elapsed="+elapsed.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+" deltaFrame="+frames);
                return false;
            }
            if(!ReferenceEquals(endpoint,_port))throw new InvalidOperationException("봉쇄선 발급 소유자가 다릅니다.");
            var staging=new GameObject("Chapter1BarrierStage");staging.SetActive(false);staging.transform.SetPositionAndRotation(transform.position,transform.rotation);
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(staging,gameObject.scene);
            try
            {
                var graph=Instantiate(_barrierPrefab,staging.transform,false);var first=new SimulationTick(checked(_latest.Tick.Value+1));
                var player=graph.GetComponentInChildren<PlayerMovementController>(true);var transfer=graph.GetComponentInChildren<TransferSimulationDriver>(true);
                var router=graph.GetComponentInChildren<InputRouter>(true);var combat=graph.GetComponentInChildren<BarrierCombatSimulationDriver>(true);
                var registry=graph.GetComponentInChildren<CombatTargetRegistry>(true);var environment=graph.GetComponentInChildren<Chapter1ChoiceEnvironmentOwner>(true);
                var camera=router.BoundCameraProviderForAuthoring;
                if(camera==null)foreach(var candidate in graph.GetComponentsInChildren<MonoBehaviour>(true))
                    if(candidate is IChapter1CameraBootstrap && candidate is IInputCameraSnapshotProvider){if(camera!=null)throw new InvalidOperationException("카메라 bootstrap이 중복입니다.");camera=candidate;}
                if(player==null||transfer==null||router==null||combat==null||registry==null||environment==null||camera==null)throw new InvalidOperationException("봉쇄선 역할이 누락됐습니다.");
                if(graph.GetComponentsInChildren<PlayerMovementController>(true).Length!=1 || player.transform==graph.transform || !player.transform.IsChildOf(graph.transform) || player.gameObject.scene!=graph.scene || player.gameObject.activeInHierarchy)throw new InvalidOperationException("봉쇄선 저작 플레이어 원본이 다릅니다.");
                var world=player.transform.position;if(float.IsNaN(world.x)||float.IsNaN(world.y)||float.IsNaN(world.z)||float.IsInfinity(world.x)||float.IsInfinity(world.y)||float.IsInfinity(world.z))throw new InvalidOperationException("봉쇄선 저작 입구가 유한하지 않습니다.");
                _generation=checked(_generation+1);
                var entry=new Chapter1EncounterEntry(this,_runKey,_generation,first,new Vector2(world.x,world.y),_latest.Health,_latest.InvulnerabilityEndTick);
                _router=router;_issuedEntry=entry;_hasIssued=true;
                player.ConfigureChapter1EntryForAuthoring(first,entry.Position);transfer.ConfigureChapter1EntryForAuthoring(first);
                router.ConfigureChapter1BarrierForAuthoring(player,transfer,combat,camera,first);
                var adapter=graph.AddComponent<Chapter1ChoicePhaseAdapter>();adapter.ConfigureForAuthoring(router,transfer,environment,_runKey,_generation);adapter.BindRunSkillSessionForAuthoring(_barrierSkillSession);
                var weight=graph.transform.Find("Counterweight");var gate=graph.transform.Find("Gate");var bridge=graph.transform.Find("Bridge");var platform=graph.transform.Find("Platform");var exit=graph.transform.Find("Exit");
                if(weight==null||gate==null||bridge==null||platform==null||exit==null)throw new InvalidOperationException("정확 봉쇄선 저작 참조가 없습니다.");
                _barrierPlayerBody=player.GetComponent<CapsuleCollider2D>();_barrierExit=exit.GetComponent<Collider2D>();
                environment.ConfigureForAuthoring(transfer,player,_barrierPlayerBody,graph.transform,weight.GetComponent<Rigidbody2D>(),gate.GetComponent<BoxCollider2D>(),bridge.GetComponent<BoxCollider2D>(),platform.GetComponent<BoxCollider2D>(),platform.GetComponent<PlatformEffector2D>(),adapter,_runKey,_generation,_barrierBranch);
                var targets=new System.Collections.Generic.List<CombatTarget>();foreach(var target in graph.GetComponentsInChildren<CombatTarget>(true))if(_barrierBranch==1||target.TargetId!="BarrierGate-Fractured")targets.Add(target);
                targets.Sort((a,b)=>string.CompareOrdinal(a.TargetId,b.TargetId));registry.ConfigureForAuthoring(targets.ToArray());
                combat.ConfigureForAuthoring(player,registry,transfer,environment,router,router,_runKey,_generation,_barrierBranch,first,_latest.Health,_latest.InvulnerabilityEndTick);
                transfer.BindChapter1ChoicePortForAuthoring(adapter,_runKey,_generation,_barrierBranch,_barrierBranch==1?4:3,_barrierBranch,environment.AdvanceEnvironmentFromCapture);
                router.ConfigureChapter1GameplayForAuthoring(entry,this);_barrierEnvironment=environment;_barrierPassage=null;
                _bossRequester=null;_hazards=null;_barrierOldClosure=null;_barrierOldGraph=null;
                ActivateExpeditionGraph(staging,graph,router,player,first);return true;
            }
            catch{staging.SetActive(false);Destroy(staging);throw;}
        }
        internal Chapter1ChoicePhaseAdapter CurrentChoiceAdapter=>_postBossAdapter;
        internal Chapter1ChoicePhaseAdapter BeginChapter1ChoiceHost(IChapter1ChoiceUiPort endpoint)
        {
            if(endpoint==null || !ReferenceEquals(endpoint,_port) || _postBossUi!=null || _pendingClosingHost!=null ||
                !TryGetChapter1BossPublication(out var actual) || !actual.CleanupTick.HasValue || !actual.RegistrationsAbsent || !actual.BossTeardownCommitted)
                throw new InvalidOperationException("실제 보스 정리 이후 선택 호스트가 아닙니다.");
            if(_postBossClosure==null)
            {
                _postBossClosure=_router.GetChapter1ClosureReader();_postBossCloseTime=Time.realtimeSinceStartupAsDouble;_postBossCloseFrame=Time.frameCount;
                var old=_graph;_graph=null;_router=null;old.SetActive(false);Destroy(old);return null;
            }
            return null;
        }
        internal Chapter1ChoicePhaseAdapter TryCompleteChapter1ChoiceHost(IChapter1ChoiceUiPort endpoint)
        {
            if(_postBossAdapter!=null)return _postBossAdapter;
            if(endpoint==null || !ReferenceEquals(endpoint,_port) || _postBossClosure==null)throw new InvalidOperationException("선택 정리 권위가 없습니다.");
            if(!_postBossClosure())
            {
                // REQ-AUTOCH1-WAIT-001: preserve the original start and closure evaluation.
                var now=Time.realtimeSinceStartupAsDouble;var elapsed=now-_postBossCloseTime;var frames=Time.frameCount-_postBossCloseFrame;
                if(double.IsNaN(now)||double.IsInfinity(now)||double.IsNaN(_postBossCloseTime)||double.IsInfinity(_postBossCloseTime)||double.IsNaN(elapsed)||double.IsInfinity(elapsed)||elapsed<0||elapsed>=5)
                    throw new InvalidOperationException("보스 입력 정리 기한이 지났습니다. elapsed="+elapsed.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+" deltaFrame="+frames);
                return null;
            }
            _generation=checked(_generation+1);return CreateChapter1UiHost(endpoint,null);
        }
        private Chapter1ChoicePhaseAdapter CreateChapter1UiHost(IChapter1ChoiceUiPort endpoint,Chapter1ClosingHostEntry closing)
        {
            var host=new GameObject(closing==null?"Chapter1ChoiceHost":"Chapter1ClosingHost");host.SetActive(false);host.transform.SetParent(transform,false);
            try
            {
                var router=host.AddComponent<InputRouter>();
                if(closing==null)router.ConfigureHubUiOnlyForAuthoring();else router.ConfigureChapter1ClosingHostForAuthoring(closing);
                var launch=host.AddComponent<DesktopProfileLaunchAdapterV1>();
                launch.ConfigureChapter1DevelopmentRootForAuthoring(router,@"H:\GPT-workspace\AcadeGameMaker\output\local-playable-slice-r1\profile\chapter1-development");
                var adapter=host.AddComponent<Chapter1ChoicePhaseAdapter>();
                if(closing==null)adapter.ConfigureChoiceHostForAuthoring(router,endpoint,_runKey,_generation);else adapter.ConfigureClosingHostForAuthoring(router,closing);
                _postBossUi=host;_postBossAdapter=adapter;if(closing!=null){_closingRouter=router;_issuedClosingHost=closing;_pendingClosingHost=null;}
                return adapter; // The caller binds the authored view before activation.
            }
            catch{Destroy(host);throw;}
        }
        internal void ActivateChapter1UiHost(Chapter1ChoicePhaseAdapter adapter)
        {if(adapter==null || !ReferenceEquals(adapter,_postBossAdapter)||_postBossUi==null||_postBossUi.activeSelf)throw new InvalidOperationException("현재 새 UI 호스트가 아닙니다.");_postBossUi.SetActive(true);}
        internal bool ValidatePendingChapter1ClosingHostEntry(Chapter1ClosingHostEntry entry)=>entry!=null && ReferenceEquals(entry,_pendingClosingHost) &&
            ReferenceEquals(entry.Owner,this) && ReferenceEquals(entry.Endpoint,_port) && ReferenceEquals(entry.RunKey,_runKey) &&
            ReferenceEquals(entry.Demo,_successfulDemo) && ReferenceEquals(entry.BranchSave,_successfulSave);
        internal bool OwnsChapter1ClosingHost(Chapter1ClosingHostEntry entry,InputRouter router)=>entry!=null && ReferenceEquals(entry,_issuedClosingHost) && ReferenceEquals(router,_closingRouter) &&
            ReferenceEquals(entry.Owner,this) && ReferenceEquals(entry.RunKey,_runKey) && ReferenceEquals(entry.Endpoint,_port) && _postBossUi!=null;
        // REQ-C1B-ENTRY-001/FAIL-001: B0 경로와 분리된 승인 원정 인계.
        [SerializeField] private GameObject _reviewPrefab;
        [SerializeField] private Vector2 _reviewEntry;
        [SerializeField] private string _reviewRoom,_reviewSocket;
#if UNITY_EDITOR
        [SerializeField] private string _reviewSceneName;
        [SerializeField] private ulong _reviewSceneHandle,_reviewHostId;
        [SerializeField] private int _reviewNameRestores;
        [SerializeField] private Chapter1BasicMovementTickObserver _reviewObserver;
        private bool _reviewSceneValidated;
        private GameObject _reviewStaging,_reviewInstance;
        private bool _reviewStartRequested,_reviewBeforeSeen,_reviewAfterSeen,_reviewProductionStarted;
        private int _reviewActivationCount,_reviewGateRows,_reviewWaitFrame;
        private long _reviewWaitStamp;
        private ulong _reviewBeforeOrdinal,_reviewAfterOrdinal;
        private static bool ReviewGateReady(bool requested,bool before,bool after,int activations,bool startDone,bool pristine)
        {return requested&&before&&after&&activations==1&&startDone&&pristine;}
        private static bool ReviewActivationAllowed(bool requested,bool before,int activations)=>requested&&before&&activations==0;
        private static bool ReviewAfterAllowed(bool before,bool after,ulong expected,ulong actual)=>before&&!after&&expected==actual;
        private static bool ReviewWaitWithin(long elapsed,long frequency,int frames)
        {return elapsed>=0&&frequency>0&&elapsed/frequency<5&&frames>=0&&frames<300;}
        internal bool ReviewGraphWaitingForAuthoring=>_reviewStaging!=null&&_reviewActivationCount==0&&!_failed;
        private void ReviewGateTrace(string stage)
        {
            if(_reviewGateRows>=8)throw new InvalidOperationException("생산 게이트 전이8 상한입니다.");
            _reviewGateRows++;Debug.Log("R01GATE stage="+stage+" requested="+_reviewStartRequested+" before="+_reviewBeforeSeen+" after="+_reviewAfterSeen+" beforeOrdinal="+_reviewBeforeOrdinal+" afterOrdinal="+_reviewAfterOrdinal+" activations="+_reviewActivationCount+" wrapperMapSame="+(_router!=null)+" startDone="+(_router!=null&&_router.ReviewStartCompletedForAuthoring)+" firstTick="+(_router!=null&&_router.CurrentReceipt.HasValue?_router.CurrentReceipt.Value.Tick.Value:-1)+" qpc="+System.Diagnostics.Stopwatch.GetTimestamp());
        }
        internal void PrepareReviewGraphForAuthoring(Chapter1BasicMovementTickObserver observer,long stamp,int frame)
        {
            if(observer==null||observer!=_reviewObserver||!_reviewSceneValidated||_reviewPrefab==null||_graph!=null||_router!=null||_runKey!=null||_reviewWaitStamp!=0||stamp<=0||_failed)throw new InvalidOperationException("사전 graph 준비 권위가 다릅니다.");
            _reviewWaitStamp=stamp;_reviewWaitFrame=frame;_expedition=true;_review=true;_runKey=Guid.NewGuid().ToString("N");_generation=1;_localEntry=_reviewEntry;_hazardDefinition=new Chapter1HazardDefinition(_reviewRoom,true);_socket=_reviewSocket;
            CreateExpeditionGraph(_reviewPrefab,new SimulationTick(0),5,-1);ReviewGateTrace("prepared");
        }
        internal void VerifyReviewWaitForAuthoring()
        {
            if(_reviewWaitStamp==0||_reviewProductionStarted||_failed)return;
            try{if(!ReviewWaitWithin(System.Diagnostics.Stopwatch.GetTimestamp()-_reviewWaitStamp,System.Diagnostics.Stopwatch.Frequency,Time.frameCount-_reviewWaitFrame)||_reviewObserver==null||!_reviewObserver.VerifyReviewGateAuthorityForAuthoring())throw new InvalidOperationException("첫 Dynamic 대기5초/300프레임 또는 원래 권위·기한 실패입니다.");
                if(_reviewActivationCount==0&&(_reviewStaging==null||_reviewStaging.activeInHierarchy||_router==null||_router.CurrentReceipt.HasValue))throw new InvalidOperationException("생산 대기 graph가 pristine이 아닙니다.");
            }catch(Exception error){Fail(error);throw;}
        }
        internal void ObserveReviewDynamicBeforeForAuthoring(ulong ordinal)
        {
            VerifyReviewWaitForAuthoring();if(_reviewBeforeSeen||_router==null)throw new InvalidOperationException("첫 Dynamic before 중복 또는 graph 누락입니다.");
            _router.VerifyReviewActionsForAuthoring(this);_reviewBeforeSeen=true;_reviewBeforeOrdinal=ordinal;ReviewGateTrace("dynamic-before");TryActivateReviewGraph();
        }
        internal void ObserveReviewDynamicAfterForAuthoring(ulong ordinal)
        {
            VerifyReviewWaitForAuthoring();if(!ReviewAfterAllowed(_reviewBeforeSeen,_reviewAfterSeen,_reviewBeforeOrdinal,ordinal))throw new InvalidOperationException("첫 Dynamic after의 실제 서수가 다릅니다.");
            _router.VerifyReviewActionsForAuthoring(this);_reviewAfterSeen=true;_reviewAfterOrdinal=ordinal;ReviewGateTrace("dynamic-after");TryActivateReviewGraph();
        }
        private void TryActivateReviewGraph()
        {
            if(!ReviewActivationAllowed(_reviewStartRequested,_reviewBeforeSeen,_reviewActivationCount))return;
            VerifyReviewWaitForAuthoring();_router.VerifyReviewActionsForAuthoring(this);_reviewActivationCount++;
            ActivateExpeditionGraph(_reviewStaging,_reviewInstance,_router,_router.BoundPlayerForAuthoring,new SimulationTick(0));ReviewGateTrace("activated");
        }
        internal void RequireReviewProductionForAuthoring(InputRouter router)
        {
            if(_reviewProductionStarted)return;
            try{VerifyReviewWaitForAuthoring();if(router!=_router||!ReviewGateReady(_reviewStartRequested,_reviewBeforeSeen,_reviewAfterSeen,_reviewActivationCount,router.ReviewStartCompletedForAuthoring,!router.CurrentReceipt.HasValue&&_reviewBeforeOrdinal==_reviewAfterOrdinal))throw new InvalidOperationException("첫 생산 전 Start·동일 Dynamic 종료·pristine 권위가 없습니다.");_reviewProductionStarted=true;ReviewGateTrace("first-production");}
            catch(Exception error){Fail(error);throw;}
        }
        internal void CloseReviewGateForAuthoring()
        {if(_reviewStaging!=null&&_router!=null)_router.CloseReviewActionsForAuthoring(this);}
        private static int ReviewSceneDecision(bool valid,bool loaded,bool unsaved,bool sameHost,bool sameScene,string expected,string actual,int restores)
        {if(!valid||!loaded||!unsaved||!sameHost||!sameScene||string.IsNullOrEmpty(expected)||restores<0||restores>1)return -1;if(actual==expected)return 0;return string.IsNullOrEmpty(actual)&&restores==0?1:-1;}
        private static bool ReviewTargetReady(bool sameScene,string sceneName,string hierarchy)
        {return sameScene&&!string.IsNullOrEmpty(sceneName)&&!string.IsNullOrEmpty(hierarchy);}
        internal void BindReviewPlayObserverForAuthoring(Chapter1BasicMovementTickObserver observer)
        {
            if(_reviewPrefab==null||_reviewObserver!=null||observer==null||observer.gameObject.scene!=gameObject.scene)throw new InvalidOperationException("검수 scene의 정확 관측기 단회 결속이 아닙니다.");
            _reviewObserver=observer;
        }
        internal void EnsureReviewSceneForPlayForAuthoring()
        {
            if(_reviewPrefab==null)return;
            var scene=gameObject.scene;
            var decision=ReviewSceneDecision(scene.IsValid(),scene.isLoaded,string.IsNullOrEmpty(scene.path),EntityId.ToULong(GetEntityId())==_reviewHostId,scene.handle.GetRawData()==_reviewSceneHandle,_reviewSceneName,scene.name,_reviewNameRestores);
            if(decision==1)
            {
                if(_reviewObserver==null||!_reviewObserver.ReviewSceneRestoreAuthorizedForAuthoring)throw new InvalidOperationException("승인된 최초 Play 이외 scene 이름 복원은 금지됩니다.");
                scene.name=_reviewSceneName;_reviewNameRestores++;
            }
            if(!_reviewSceneValidated&&_reviewObserver!=null)_reviewObserver.RecordPlayTransitionForAuthoring("review-scene","decision="+decision+" valid="+scene.IsValid()+" loaded="+scene.isLoaded+" name="+scene.name+" path="+scene.path+" handle="+scene.handle.GetRawData()+" expectedHandle="+_reviewSceneHandle+" hostId="+EntityId.ToULong(GetEntityId())+" nameRestores="+_reviewNameRestores);
            if(decision<0||scene.name!=_reviewSceneName||scene.handle.GetRawData()!=_reviewSceneHandle||!string.IsNullOrEmpty(scene.path))throw new InvalidOperationException("동일 미저장 review scene 신원·이름을 확인할 수 없습니다.");
            _reviewSceneValidated=true;
        }
        internal void CompleteReviewSceneLoadedForAuthoring(UnityEngine.SceneManagement.Scene scene)
        {
            if(_reviewSceneValidated||scene!=gameObject.scene||!scene.isLoaded)throw new InvalidOperationException("중복 또는 다른 review sceneLoaded입니다.");
            EnsureReviewSceneForPlayForAuthoring();
        }
        internal void RecordCameraBootstrapForAuthoring(string details)
        {if(_reviewObserver!=null)_reviewObserver.RecordPlayTransitionForAuthoring("camera-bootstrap",details);}
        private void ValidateReviewGraphBeforeActivation(GameObject staging,GameObject instance)
        {
            if(_reviewPrefab==null)return;
            if(!_reviewSceneValidated)throw new InvalidOperationException("graph 활성 전 sceneLoaded 선행 완료가 없습니다.");
            EnsureReviewSceneForPlayForAuthoring();var scene=gameObject.scene;
            var details=new System.Text.StringBuilder("hostScene="+scene.name+" hostHandle="+scene.handle.GetRawData()+" hostPath="+scene.path+" stagingScene="+staging.scene.name+" stagingHandle="+staging.scene.handle.GetRawData()+" stagingPath="+staging.scene.path);
            var ready=staging.scene==scene&&instance.scene==scene;
            foreach(var target in instance.GetComponentsInChildren<CombatTarget>(true))
            {var hierarchy=Chapter1BasicMovementTickObserver.Hierarchy(target.transform);ready&=ReviewTargetReady(target.gameObject.scene==scene,target.gameObject.scene.name,hierarchy);details.Append(" combatScene=").Append(target.gameObject.scene.name).Append(" handle=").Append(target.gameObject.scene.handle.GetRawData()).Append(" path=").Append(target.gameObject.scene.path).Append(" hierarchy=").Append(hierarchy);}
            foreach(var target in instance.GetComponentsInChildren<TransferTarget>(true))
            {var hierarchy=Chapter1BasicMovementTickObserver.Hierarchy(target.transform);ready&=ReviewTargetReady(target.gameObject.scene==scene,target.gameObject.scene.name,hierarchy);details.Append(" transferScene=").Append(target.gameObject.scene.name).Append(" handle=").Append(target.gameObject.scene.handle.GetRawData()).Append(" path=").Append(target.gameObject.scene.path).Append(" hierarchy=").Append(hierarchy);}
            if(_reviewObserver!=null)_reviewObserver.RecordPlayTransitionForAuthoring("graph-before-active",details+" ready="+ready);
            if(!ready)throw new InvalidOperationException("활성화 전 graph/target의 장면·계층 귀속이 다릅니다.");
        }
#endif
        private bool _expedition,_review,_bossReplacement,_failureConsumed,_runCommit,_retired,_failureRequestBusy;
        private IChapter1BossProgressionPort _port;
        private Chapter1AcceptedFailureToken _pendingFailure;
        private Chapter1LifecyclePublication _lifecycle;
        private string _lifecyclePreviousTarget;
        private Chapter1HazardDefinition _hazardDefinition,_replacementHazards;
        private Vector2 _localEntry,_replacementEntry;
        private string _socket,_replacementSocket;
        private Chapter1HazardObservation _hazards;
        private OrdanBossTerminalTransitionRequester _bossRequester;
        private Func<bool> _oldClosure;
        internal Chapter1EncounterEvidenceView PreviousCompletedEvidence=>_predecessor;
        internal Chapter1HazardObservation CurrentHazards=>_hazards;
        internal bool Chapter1GraphRetired=>_expedition && _retired && _graph==null && _router==null;
        internal void ConfigureChapter1RoomReviewForAuthoring(GameObject prefab,Vector2 entry,Chapter1HazardDefinition hazards,string socketPlan)
        {
            if(gameObject.activeInHierarchy || _reviewPrefab!=null || _runKey!=null || _hasIssued || _initialPrefab!=null || prefab==null)
                throw new InvalidOperationException("검수 host는 비활성 단회 구성입니다.");
            _reviewPrefab=prefab;_reviewEntry=entry;_reviewRoom=hazards.RoomId;_reviewSocket=socketPlan;
#if UNITY_EDITOR
            var scene=gameObject.scene;
            if(!scene.IsValid()||!scene.isLoaded||!string.IsNullOrEmpty(scene.path)||scene.name!="Chapter1BasicMovementReview"+hazards.RoomId)throw new InvalidOperationException("정확 미저장 review scene만 보존합니다.");
            _reviewSceneName=scene.name;_reviewSceneHandle=scene.handle.GetRawData();_reviewHostId=EntityId.ToULong(GetEntityId());
#endif
        }
        internal void BeginChapter1Expedition(GameObject prefab,string runKey,Vector2 entry,Chapter1HazardDefinition hazards,string socketPlan,IChapter1BossProgressionPort port)
        {
            if(string.IsNullOrEmpty(runKey) || port==null || _failed || _graph!=null || _router!=null || _waiting || (_expedition && !_retired) || (!_expedition && _runKey!=null))
                throw new InvalidOperationException("출정은 기존 graph가 없는 실제 새 원정에서만 시작합니다.");
            _expedition=true;_review=false;_runKey=runKey;_generation=1;_port=port;_retired=false;_runCommit=false;_failureConsumed=false;
            _barrierEnvironment=null;_barrierPassage=null;_barrierOldClosure=null;_barrierSkillSession=null;_postBossClosure=null;
            _pendingClosingHost=_issuedClosingHost=null;_successfulClose=null;_successfulPassage=null;_successfulDemo=_successfulSave=null;
            _pendingFailure=null;_lifecycle=null;_hasIssued=false;_pending=false;_readyToDestroy=false;
            _localEntry=entry;_hazardDefinition=hazards;_socket=socketPlan;
            CreateExpeditionGraph(prefab,new SimulationTick(0),5,-1);
        }
        internal void RequestChapter1RoomReplacement(GameObject prefab,Vector2 entry,Chapter1HazardDefinition hazards,string socketPlan)
        {RequestExpeditionReplacement(prefab,entry,hazards,socketPlan,false);}
        internal void RequestChapter1BossReplacement(GameObject prefab,Vector2 entry,Chapter1HazardDefinition hazards,IChapter1BossProgressionPort port)
        {
            if(!ReferenceEquals(port,_port) || hazards.RoomId!="Ordan" || entry!=new Vector2(-7,0.81f))throw new InvalidOperationException("보스 port 또는 로컬 입구가 다릅니다.");
            RequestExpeditionReplacement(prefab,entry,hazards,null,true);
        }
        private void RequestExpeditionReplacement(GameObject prefab,Vector2 entry,Chapter1HazardDefinition hazards,string socketPlan,bool boss)
        {
            if(!_expedition || _review || _bossRequester!=null || _pendingFailure!=null || _failed || _pending || _waiting || prefab==null || _router==null ||
                !TryGetCompletedEvidence(out var completed) || completed.Health<=0)throw new InvalidOperationException("방 교체 원문이 없습니다.");
            var next=new SimulationTick(checked(completed.Tick.Value+1));
            QueueLifecycle(next,AcadeGameMaker.Transfer.TransferClearReason.RoomLeaving);
            _router.RequestMode(this,InputMode.Transition,"chapter1-room-leaving",next);
            _transitionTick=next;_replacementPrefab=prefab;_replacementEntry=entry;_replacementHazards=hazards;_replacementSocket=socketPlan;
            _bossReplacement=boss;_pending=true;_lifecycle=null;
        }
        internal bool ValidateChapter1FailureSources(Chapter1AcceptedFailureToken token)
        {
            if(token==null || !_expedition || _failed || _waiting || _router==null || !ReferenceEquals(token.Consumer,this) || !ReferenceEquals(token.Issuer,_port) ||
                !ReferenceEquals(token.Router,_router) || token.RunKey!=_runKey || token.Generation!=_generation || token.ArbitrationRevision<=0 ||
                !TryGetCompletedEvidence(out var completed) || !token.SourceReceipt.Equals(completed.Receipt) || completed.Receipt.Mode!=InputMode.GameplayEnabled ||
                !ReferenceEquals(token.Health,completed.HealthPublication) || !_router.OwnsChapter1HealthPublication(token.Health) ||
                !_hazards.TryGetCompleted(completed,out var kill,out var crush) || !ReferenceEquals(token.Kill,kill) || !ReferenceEquals(token.Crush,crush) ||
                !_hazards.Owns(kill) || !_hazards.Owns(crush))return false;
            var sources=new[]{token.Health,kill,crush};
            for(var i=0;i<sources.Length;i++)if(sources[i]==null || sources[i].Kind!=(Chapter1FailureSourceKind)i || !ReferenceEquals(sources[i].Router,_router) ||
                sources[i].RunKey!=_runKey || sources[i].Generation!=_generation || !sources[i].Receipt.Equals(completed.Receipt))return false;
            if(token.Health.Triggered!=(completed.Health==0))return false;
            for(var i=0;i<sources.Length;i++)if(sources[i].Triggered)return token.Cause==(Chapter1FailureSourceKind)i;
            return false;
        }
        internal void RequestChapter1FailureTransition(IChapter1BossProgressionPort source,Chapter1AcceptedFailureToken token)
        {
            if(_failureRequestBusy || _pendingFailure!=null || _runCommit || _pending || _waiting || !ReferenceEquals(source,_port) || source.BlocksNewFailureRequests ||
                !ValidateChapter1FailureSources(token) || !source.ValidatePendingFailure(this,token))throw new InvalidOperationException("실제 실패 pending 권위가 없습니다.");
            var next=new SimulationTick(checked(token.SourceReceipt.Tick.Value+1));
            if(_bossRequester!=null && (_bossRequester.IsClosed || _bossRequester.LatestCompletionReceipt.HasValue))throw new InvalidOperationException("보스 요청자가 폐쇄되었습니다.");
            _lifecyclePreviousTarget=_latest.TransferTarget;
            _failureRequestBusy=true;_pendingFailure=token;
            try
            {
                source.ConsumePendingFailure(this,token);_failureConsumed=true;
                if(_bossRequester!=null)_bossRequester.RequestChapter1AcceptedFailure(this,token);
                else {QueueLifecycle(next,AcadeGameMaker.Transfer.TransferClearReason.RunFailed);_router.RequestMode(this,InputMode.Transition,"chapter1-run-failed",next);}
                _transitionTick=next;
            }
            catch(Exception error){Fail(error);throw;}
            finally{_failureRequestBusy=false;}
        }
        internal bool OwnsPendingChapter1Failure(Chapter1AcceptedFailureToken token)=>token!=null && _failureConsumed && ReferenceEquals(_pendingFailure,token) &&
            ReferenceEquals(token.Consumer,this) && ReferenceEquals(token.Router,_router) && token.RunKey==_runKey && token.Generation==_generation;
        internal bool TryGetChapter1LifecyclePublication(Chapter1LifecycleKind kind,out Chapter1LifecyclePublication publication)
        {publication=_lifecycle;return publication!=null && publication.Kind==kind && OwnsChapter1LifecyclePublication(publication);}
        internal bool OwnsChapter1LifecyclePublication(Chapter1LifecyclePublication publication)=>publication!=null && ReferenceEquals(publication,_lifecycle) &&
            ReferenceEquals(publication.Owner,this) && ReferenceEquals(publication.Router,_router) && publication.RunKey==_runKey && publication.Generation==_generation;
        internal void CommitChapter1FailedRun(Chapter1LifecyclePublication clear,long failureRevision)
        {
            if(_runCommit || !_failureConsumed || _pendingFailure==null || clear==null || clear.Kind!=Chapter1LifecycleKind.RunFailed ||
                !OwnsChapter1LifecyclePublication(clear) || failureRevision!=_pendingFailure.ArbitrationRevision ||
                clear.LockedReceipt.Tick.Value!=checked(_pendingFailure.SourceReceipt.Tick.Value+1))throw new InvalidOperationException("실패 commit 원문이 없습니다.");
            ValidateLifecycle(clear.Kind,clear.LockedReceipt);
            _runCommit=true;_readyToDestroy=true;
        }
        internal bool TryGetChapter1BossPublication(out Chapter1BossPublication publication)
        {publication=null;return _bossRequester!=null && _bossRequester.TryGetChapter1Publication(out publication);}
        internal bool OwnsChapter1BossPublication(Chapter1BossPublication publication)=>_bossRequester!=null && _bossRequester.OwnsChapter1Publication(publication);
        private void QueueLifecycle(SimulationTick tick,AcadeGameMaker.Transfer.TransferClearReason reason)
        {_lifecyclePreviousTarget=_latest.TransferTarget;_router.BoundTransferForAuthoring.Submit(new TransferSimulationInput(tick,null,null,null,Array.Empty<AcadeGameMaker.Transfer.TransferTargetRemoved>(),reason));}
        private void ValidateLifecycle(Chapter1LifecycleKind kind,InputFrameCommitReceipt receipt)
        {
            var phase=_router.BoundTransferForAuthoring.LatestPhasePublication;
            var reason=kind==Chapter1LifecycleKind.RunFailed?AcadeGameMaker.Transfer.TransferClearReason.RunFailed:AcadeGameMaker.Transfer.TransferClearReason.RoomLeaving;
            if(!phase.HasValue || phase.Value.Tick!=receipt.Tick || phase.Value.Input.LifecycleReason!=reason || receipt.Mode!=InputMode.Transition ||
                !_router.CurrentReceipt.HasValue || !_router.CurrentReceipt.Value.Equals(receipt) || phase.Value.Snapshot.PlayerState!=AcadeGameMaker.Transfer.TransferPlayerState.Baseline ||
                !string.IsNullOrEmpty(phase.Value.Snapshot.ActiveTargetId))throw new InvalidOperationException("실제 lifecycle 완료가 없습니다.");
            var clear=phase.Value.LifecycleClear;
            if(!string.IsNullOrEmpty(_lifecyclePreviousTarget) && (!clear.HasValue ||
                clear.Value.PreviousTargetId!=_lifecyclePreviousTarget))
                throw new InvalidOperationException("실제 점유 대상 lifecycle clear가 없습니다.");
            if(clear.HasValue && (clear.Value.Reason!=reason || clear.Value.Tick!=receipt.Tick))
                throw new InvalidOperationException("lifecycle clear 원문이 다릅니다.");
        }
        private void FixedExpedition()
        {
            if(_failed || _waiting || _router==null || _runCommit)return;
            try
            {
                if(_router.IsFaulted)throw new InvalidOperationException("원정 입력이 중단되었습니다.");
                var receipt=_router.CurrentReceipt;
                if((_pending || _pendingFailure!=null) && receipt.HasValue && receipt.Value.Tick==_transitionTick && receipt.Value.Mode==InputMode.Transition)
                {
                    var kind=_pendingFailure!=null?Chapter1LifecycleKind.RunFailed:Chapter1LifecycleKind.RoomLeaving;
                    ValidateLifecycle(kind,receipt.Value);
                    if(_lifecycle==null)_lifecycle=new Chapter1LifecyclePublication(this,_router,_runKey,_generation,receipt.Value,kind,
                        _pendingFailure!=null?_pendingFailure.ArbitrationRevision:checked(_generation+1));
                    if(kind==Chapter1LifecycleKind.RoomLeaving)
                    {
                        if(!_router.TryGetChapter1CompletedEvidence(out var final))throw new InvalidOperationException("방 이탈 완료 원문이 없습니다.");
                        _latest=final;_hasCompleted=true;_readyToDestroy=true;
                    }
                }
                if(_router.TryGetChapter1CompletedEvidence(out var completed))
                {
                    if(completed.RunKey!=_runKey || completed.Generation!=_generation || (!_hasCompleted && completed.Tick!=_issuedEntry.FirstTick) ||
                        (_hasCompleted && completed.Tick!=_latest.Tick && completed.Tick.Value!=checked(_latest.Tick.Value+1)))throw new InvalidOperationException("원정 완료 tick이 연속하지 않습니다.");
                    _latest=completed;_hasCompleted=true;
                    if(_barrierRetirementRequested && completed.Tick==_barrierRetirementTick)
                    {
                        var clear=_router.BoundTransferForAuthoring.LatestPhasePublication;
                        if(completed.Receipt.Mode!=InputMode.Transition||!clear.HasValue||clear.Value.Tick!=completed.Tick||
                            clear.Value.Input.LifecycleReason!=AcadeGameMaker.Transfer.TransferClearReason.RoomLeaving||clear.Value.Snapshot.PlayerState!=AcadeGameMaker.Transfer.TransferPlayerState.Baseline||!string.IsNullOrEmpty(clear.Value.Snapshot.ActiveTargetId))
                            throw new InvalidOperationException("비치명 복귀의 실제 잠금·전이 정리가 다릅니다.");
                        CloseBarrierGraphForReplacement();return;
                    }
                    if(_successfulPassage!=null && _successfulClose==null && completed.Tick==_successfulEndTick)
                    {
                        var phase=_router.BoundTransferForAuthoring.LatestPhasePublication;
                        if(completed.Receipt.Mode!=InputMode.Ended || !phase.HasValue || phase.Value.Tick!=completed.Tick || phase.Value.Input.LifecycleReason!=AcadeGameMaker.Transfer.TransferClearReason.DemoCompleted ||
                            phase.Value.Snapshot.PlayerState!=AcadeGameMaker.Transfer.TransferPlayerState.Baseline || !string.IsNullOrEmpty(phase.Value.Snapshot.ActiveTargetId))throw new InvalidOperationException("정상 종료의 실제 입력·전이 정리가 다릅니다.");
                        _successfulClose=new Chapter1SuccessfulClose(this,_successfulPassage,completed);
                    }
                    if(_barrierEnvironment!=null)
                    {
                        var transfer=_router.BoundTransferForAuthoring;object completedView=completed;transfer.PublishChapter1CompletedBarrierView(completed.Tick,completedView);
                        _barrierEnvironment.ObserveCompletedPlayerContact(completed.Tick,completedView,completed.Receipt);
                        if(_barrierEnvironment.HasActualSkill && _barrierEnvironment.GateOpened && _barrierExit!=null && _barrierPlayerBody!=null && _barrierExit.bounds.Intersects(_barrierPlayerBody.bounds))
                            _barrierPassage=new Chapter1BarrierPassage(this,completed,transfer.ReadChapter1CommittedCapture(completed.Tick));
                    }
                    else _hazards.PublishCompleted(completed);
                }
                if(_bossRequester!=null && _hasCompleted)_bossRequester.ObserveChapter1Completed(_latest);
            }
            catch(Exception error){Fail(error);}
        }
        private void CreateExpeditionGraph(GameObject prefab,SimulationTick firstTick,int health,int invulnerability)
        {
            GameObject staging=null;
            try
            {
                if(prefab==null || prefab.activeSelf || prefab.GetComponentsInChildren<InputRouter>(true).Length!=1 ||
                    prefab.GetComponentsInChildren<Chapter1EncounterLifetimeOwner>(true).Length!=0)throw new InvalidOperationException("비활성 sole graph가 필요합니다.");
                staging=new GameObject("Chapter1EncounterStage");staging.transform.SetPositionAndRotation(transform.position,transform.rotation);staging.SetActive(false);
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(staging,gameObject.scene);
                var instance=Instantiate(prefab,staging.transform,false);instance.name=prefab.name;
                var player=instance.GetComponentInChildren<PlayerMovementController>(true);var transfer=instance.GetComponentInChildren<TransferSimulationDriver>(true);
                var router=instance.GetComponentInChildren<InputRouter>(true);var regular=instance.GetComponentInChildren<CombatSimulationDriver>(true);
                var boss=instance.GetComponentInChildren<OrdanBossCombatSimulationDriver>(true);
                if(player==null || transfer==null || router==null || (regular==null)==(boss==null) || router.IsChapter1Regular!=(regular!=null))throw new InvalidOperationException("원정 graph 역할이 다릅니다.");
                if(regular!=null)ApplySocket(instance.transform,_hazardDefinition.RoomId,_socket);
                var world=instance.transform.TransformPoint(new Vector3(_localEntry.x,_localEntry.y,0));
                var entry=new Chapter1EncounterEntry(this,_runKey,_generation,firstTick,new Vector2(world.x,world.y),health,invulnerability);
                _router=router;_issuedEntry=entry;_hasIssued=true;
                player.ConfigureChapter1EntryForAuthoring(firstTick,entry.Position);transfer.ConfigureChapter1EntryForAuthoring(firstTick);
                if(regular!=null)regular.ConfigureChapter1CarriedPlayerForAuthoring(firstTick,health,invulnerability);
                else boss.ConfigureChapter1CarriedPlayerForAuthoring(firstTick,health,invulnerability);
                router.ConfigureChapter1GameplayForAuthoring(entry,this);
                _hazards=instance.AddComponent<Chapter1HazardObservation>();_hazards.ConfigureForAuthoring(router,_hazardDefinition,firstTick);
                var requesters=instance.GetComponentsInChildren<OrdanBossTerminalTransitionRequester>(true);
                _bossRequester=null;
                if(boss!=null)
                {
                    if(requesters.Length!=1 || _port==null)throw new InvalidOperationException("보스 sole requester 또는 port가 없습니다.");
                    _bossRequester=requesters[0];var handoff=instance.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>(true);
                    _bossRequester.ConfigureForAuthoring(router,player,boss,handoff);
                    _bossRequester.ConfigureChapter1ProgressionForAuthoring(this,entry,_port,instance.GetComponentInChildren<OrdanBossTerminalTeardown>(true));
                }
                else if(requesters.Length!=0)throw new InvalidOperationException("일반방에 보스 requester가 있습니다.");
#if UNITY_EDITOR
                ValidateReviewGraphBeforeActivation(staging,instance);
#endif
#if UNITY_EDITOR
                if(_review&&_reviewWaitStamp>0&&_reviewActivationCount==0)
                {router.PrepareReviewActionsForAuthoring(this,staging);_reviewStaging=staging;_reviewInstance=instance;_graph=staging;_hasCompleted=false;_latest=default;return;}
#endif
                ActivateExpeditionGraph(staging,instance,router,player,firstTick);
            }
            catch(Exception error){if(staging!=null){staging.SetActive(false);Destroy(staging);}Fail(error);throw;}
        }
        private void ActivateExpeditionGraph(GameObject staging,GameObject instance,InputRouter router,PlayerMovementController player,SimulationTick firstTick)
        {
                var cameraProvider=router.BoundCameraProviderForAuthoring;
                var bootstrap=cameraProvider as IChapter1CameraBootstrap;
                if(cameraProvider==null||bootstrap==null)throw new InvalidOperationException("정확 bound camera 생산 bootstrap이 없습니다.");
                bootstrap.PrepareChapter1Bootstrap(this,staging,router,player,firstTick);
                instance.SetActive(true);_graph=staging;_hasCompleted=false;_latest=default;staging.SetActive(true);
                if(router.BoundCameraProviderForAuthoring!=cameraProvider||CurrentRouter!=router||player.transform.root.gameObject!=staging)throw new InvalidOperationException("활성화 직후 camera graph 결속이 다릅니다.");
                bootstrap.CompleteChapter1Bootstrap(this,staging,router,player,firstTick);
        }
        private static void ApplySocket(Transform graph,string room,string socket)
        {
            if(room==null || room.Length!=3 || room[0]!='R' || room[1]!='0' || room[2]<'1' || room[2]>'6' ||
                (socket!="S"+room.Substring(1)+"-A" && (room=="R01" || room=="R06" || socket!="S"+room.Substring(1)+"-B")))throw new InvalidOperationException("소켓 계획이 승인 방과 다릅니다.");
            var offset=socket.EndsWith("-B",StringComparison.Ordinal)?1:0;
            var walker=room=="R01"?8:room=="R02"||room=="R05"?6:room=="R04"?7:5;
            var surveyor=room=="R01"?20:room=="R03"?25:room=="R05"?26:27;
            var w=graph.Find("Walker");var v=graph.Find("Surveyor");if(w==null || v==null)throw new InvalidOperationException("기존 적 root가 없습니다.");
            w.localPosition=new Vector3(walker+offset,0.81f,0);v.localPosition=new Vector3(surveyor-offset,0.81f,0);
        }
        private void RetireExpeditionGraph()
        {
            _oldGraph=_graph;_oldClosure=_router.GetChapter1ClosureReader();_predecessor=_latest;
            _waiting=true;_pending=false;_readyToDestroy=false;_destroyFrame=Time.frameCount;_destroyTime=Time.realtimeSinceStartupAsDouble;
            _oldGraph.SetActive(false);Destroy(_oldGraph);_graph=null;_router=null;_hazards=null;
        }
        private void UpdateExpedition()
        {
            if(!_waiting || _failed)return;
            try
            {
                if(_oldGraph!=null || _oldClosure==null || !_oldClosure())
                {if(Time.frameCount-_destroyFrame>=300 || Time.realtimeSinceStartupAsDouble-_destroyTime>=5d)throw new InvalidOperationException("실제 파기/입력 dispose가 완료되지 않았습니다.");return;}
                _waiting=false;_oldGraph=null;_oldClosure=null;
                if(_runCommit){_retired=true;_hasIssued=false;_bossRequester=null;return;}
                if(!_predecessor.HasCompleted || _predecessor.Health<=0 || _predecessor.Generation!=_generation || _predecessor.RunKey!=_runKey)throw new InvalidOperationException("승계 원문이 다릅니다.");
                _generation=checked(_generation+1);_localEntry=_replacementEntry;_hazardDefinition=_replacementHazards;_socket=_replacementSocket;
                var prefab=_replacementPrefab;_replacementPrefab=null;_lifecycle=null;
                CreateExpeditionGraph(prefab,new SimulationTick(checked(_predecessor.Tick.Value+1)),_predecessor.Health,_predecessor.InvulnerabilityEndTick);
            }
            catch(Exception error){Fail(error);}
        }
        [SerializeField] private GameObject _initialPrefab;
        // REQ-C1B-ENTRY-001: one pristine inactive authored host reference.
        internal GameObject InitialPrefabForAuthoring => _initialPrefab;
        internal void ConfigureInitialPrefabForAuthoring(GameObject inactiveRegularPrefab)
        {
            if(gameObject.activeInHierarchy || _initialPrefab!=null || _runKey!=null || _hasIssued || _graph!=null || _failed ||
                inactiveRegularPrefab==null || inactiveRegularPrefab.activeSelf ||
                inactiveRegularPrefab.GetComponentsInChildren<InputRouter>(true).Length!=1 ||
                !inactiveRegularPrefab.GetComponentInChildren<InputRouter>(true).IsChapter1Regular ||
                inactiveRegularPrefab.GetComponentsInChildren<Chapter1EncounterLifetimeOwner>(true).Length!=0)
                throw new InvalidOperationException("Initial prefab requires a pristine inactive host and an owner-free regular graph.");
            _initialPrefab=inactiveRegularPrefab;
        }
        private string _runKey;
        private long _generation;
        private GameObject _graph,_oldGraph,_replacementPrefab;
        private InputRouter _router;
        private Chapter1InputLifetime _oldLifetime;
        private Chapter1EncounterEntry _issuedEntry;
        private Chapter1EncounterEvidenceView _latest,_predecessor;
        private bool _hasIssued,_hasCompleted,_pending,_readyToDestroy,_waiting,_failed;
        private SimulationTick _transitionTick;
        private int _destroyFrame;
        private double _destroyTime;
        internal bool IsFailed => _failed;
        internal bool IsReplacementPending => _pending || _waiting;
        internal long Generation => _generation;
        internal InputRouter CurrentRouter => _router;
        internal bool IsIssuedEntry(InputRouter router,Chapter1EncounterEntry entry) => _hasIssued && !_failed && ReferenceEquals(router,_router) && _issuedEntry.SameAs(entry);
        private void Start()
        {
            if(_reviewPrefab!=null)
            {
#if UNITY_EDITOR
                if(!_reviewSceneValidated)throw new InvalidOperationException("Start 이전 정확 sceneLoaded 준비가 완료되지 않았습니다.");
                if(_reviewStartRequested||_reviewStaging==null)throw new InvalidOperationException("검수 Start 요청 중복 또는 사전 graph가 없습니다.");
                _reviewStartRequested=true;ReviewGateTrace("start-requested");TryActivateReviewGraph();return;
#endif
                _expedition=true;_review=true;_runKey=Guid.NewGuid().ToString("N");_generation=1;_localEntry=_reviewEntry;
                _hazardDefinition=new Chapter1HazardDefinition(_reviewRoom,true);_socket=_reviewSocket;CreateExpeditionGraph(_reviewPrefab,new SimulationTick(0),5,-1);
            }
            else if(_initialPrefab!=null && !_hasIssued)BeginAuthoredRegularEncounter(_initialPrefab);
        }
        internal void BeginAuthoredRegularEncounter(GameObject prefab)
        {
            if(_runKey!=null || _hasIssued || _graph!=null || _waiting || _failed)throw new InvalidOperationException("Encounter owner can begin only once.");
            _runKey=Guid.NewGuid().ToString("N");_generation=1;
            CreateSeededGraph(prefab,new SimulationTick(0),5,-1,null);
        }
        internal void RequestRegularReplacement(GameObject prefab)
        {
            if(_failed || _pending || _waiting || prefab==null || _router==null || !TryGetCompletedEvidence(out var completed) || completed.Health<=0)
                throw new InvalidOperationException("Replacement requires the sole live completed graph.");
            _transitionTick=new SimulationTick(checked(completed.Tick.Value+1));
            _router.RequestMode(this,InputMode.Transition,"chapter1-regular-replacement",_transitionTick);
            _replacementPrefab=prefab;_pending=true;
        }
        internal bool TryGetCompletedEvidence(out Chapter1EncounterEvidenceView view)
        {
            view=default;
            return !_failed && !_waiting && _router!=null && _router.TryGetChapter1CompletedEvidence(out view);
        }
        private void CreateSeededGraph(GameObject prefab,SimulationTick firstTick,int health,int invulnerability,Vector2? carriedPosition)
        {
            GameObject staging=null;
            try
            {
                if(prefab==null || prefab.activeSelf || prefab.GetComponentsInChildren<InputRouter>(true).Length!=1)
                    throw new InvalidOperationException("Regular prefab must be authored inactive with one router.");
                staging=new GameObject("Chapter1RegularStage");staging.SetActive(false);
                var instance=Instantiate(prefab,staging.transform,false);instance.name="CombatEncounterGraph";
                if(instance.activeInHierarchy)throw new InvalidOperationException("Entry construction activated before seed.");
                var player=instance.GetComponentInChildren<PlayerMovementController>(true);
                var transfer=instance.GetComponentInChildren<TransferSimulationDriver>(true);
                var combat=instance.GetComponentInChildren<CombatSimulationDriver>(true);
                var router=instance.GetComponentInChildren<InputRouter>(true);
                if(player==null || transfer==null || combat==null || router==null || !router.IsChapter1Regular ||
                    instance.GetComponentsInChildren<OrdanBossTerminalTransitionRequester>(true).Length!=0 ||
                    instance.GetComponentsInChildren<Chapter1EncounterLifetimeOwner>(true).Length!=0)
                    throw new InvalidOperationException("Regular graph roles or lifetime ownership are not exact.");
                var position=carriedPosition ?? new Vector2(player.transform.position.x,player.transform.position.y);
                var entry=new Chapter1EncounterEntry(this,_runKey,_generation,firstTick,position,health,invulnerability);
                _router=router;_issuedEntry=entry;_hasIssued=true;
                player.ConfigureChapter1EntryForAuthoring(firstTick,position);
                transfer.ConfigureChapter1EntryForAuthoring(firstTick);
                combat.ConfigureChapter1CarriedPlayerForAuthoring(firstTick,health,invulnerability);
                router.ConfigureChapter1GameplayForAuthoring(entry,this);
                if(player.Chapter1ConfiguredFirstTick!=firstTick || transfer.Chapter1ConfiguredFirstTick!=firstTick || combat.Chapter1ConfiguredCarryTick!=firstTick)
                    throw new InvalidOperationException("Inactive seed configuration is inconsistent.");
                ActivateExpeditionGraph(staging,instance,router,player,firstTick);
            }
            catch(Exception error)
            {
                if(staging!=null){staging.SetActive(false);Destroy(staging);}
                Fail(error);throw;
            }
        }
        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if(ReviewGraphWaitingForAuthoring){VerifyReviewWaitForAuthoring();return;}
#endif
            if(_expedition){FixedExpedition();return;}
            if(_failed || _waiting || _router==null)return;
            try
            {
                if(_router.IsFaulted)throw new InvalidOperationException("Regular input source faulted.");
                if(!_router.TryGetChapter1CompletedEvidence(out var completed))return;
                if(completed.RunKey!=_runKey || completed.Generation!=_generation ||
                    (!_hasCompleted && completed.Tick!=_issuedEntry.FirstTick) ||
                    (_hasCompleted && completed.Tick.Value!=checked(_latest.Tick.Value+1)))
                    throw new InvalidOperationException("Regular completed evidence skipped, repeated, or changed graph.");
                _latest=completed;_hasCompleted=true;
                if(_pending && completed.Receipt.Mode==InputMode.Transition && completed.Tick.Value>=_transitionTick.Value)
                    _readyToDestroy=true;
            }
            catch(Exception error){Fail(error);}
        }
        private void LateUpdate()
        {
            if(_expedition){if(!_failed && _readyToDestroy && (_pending || _runCommit) && !_waiting)RetireExpeditionGraph();return;}
            if(_failed || !_readyToDestroy || !_pending || _waiting)return;
            try
            {
                if(!_router.TryGetChapter1CompletedEvidence(out var last) || !_hasCompleted || !last.Receipt.Equals(_latest.Receipt) || last.Health<=0)
                    throw new InvalidOperationException("Replacement has no final live completed receipt.");
                _predecessor=last;_oldLifetime=_router.Chapter1Lifetime;_oldGraph=_graph;
                _waiting=true;_pending=false;_readyToDestroy=false;_destroyFrame=Time.frameCount;_destroyTime=Time.realtimeSinceStartupAsDouble;
                _oldGraph.SetActive(false);Destroy(_oldGraph);_graph=null;_router=null;
            }
            catch(Exception error){Fail(error);}
        }
        private void Update()
        {
            if(_expedition){UpdateExpedition();return;}
            if(!_waiting || _failed)return;
            try
            {
                if(_oldGraph!=null || _oldLifetime==null || !_oldLifetime.IsClosed)
                {
                    if(Time.frameCount-_destroyFrame>=300 || Time.realtimeSinceStartupAsDouble-_destroyTime>=5d)
                        throw new InvalidOperationException("Predecessor destruction/disposal timed out.");
                    return;
                }
                if(!_predecessor.HasCompleted || _predecessor.RunKey!=_runKey || _predecessor.Generation!=_generation || _predecessor.Health<=0)
                    throw new InvalidOperationException("Predecessor carry evidence is foreign or dead.");
                _generation=checked(_generation+1);var next=new SimulationTick(checked(_predecessor.Tick.Value+1));
                _waiting=false;CreateSeededGraph(_replacementPrefab,next,_predecessor.Health,_predecessor.InvulnerabilityEndTick,_predecessor.Position);
                _oldGraph=null;_oldLifetime=null;_replacementPrefab=null;
            }
            catch(Exception error){Fail(error);}
        }
        private void Fail(Exception error)
        {
#if UNITY_EDITOR
          if(!_failed&&_reviewWaitStamp>0){try{ReviewGateTrace("failed");}catch(Exception traceError){Debug.LogError(traceError.Message,this);}}
#endif
          _failed=true;
#if UNITY_EDITOR
          try{CloseReviewGateForAuthoring();}catch(Exception closeError){Debug.LogError("사전 입력 정리 실패: "+closeError.Message,this);}
#endif
          CloseChapter1PostBossAfterFailure();if(_graph!=null){_graph.SetActive(false);Destroy(_graph);}_router=null;Debug.LogError("Chapter1 regular owner failed: "+error.Message,this); }
        private void OnDestroy(){
#if UNITY_EDITOR
            try{CloseReviewGateForAuthoring();}catch(Exception error){Debug.LogError(error.Message,this);}
#endif
            if(_graph!=null){_graph.SetActive(false);Destroy(_graph);}if(_oldGraph!=null)Destroy(_oldGraph);}
    }
}
