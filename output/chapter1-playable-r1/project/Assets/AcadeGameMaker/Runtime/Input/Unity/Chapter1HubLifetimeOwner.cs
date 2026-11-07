using System;
using AcadeGameMaker.Core;
using UnityEngine;

namespace AcadeGameMaker.Input.Unity
{
    internal interface IChapter1BossProgressionPort
    {
        bool BlocksNewFailureRequests {get;}
        bool ValidatePendingFailure(Chapter1EncounterLifetimeOwner consumer,Chapter1AcceptedFailureToken token);
        void ConsumePendingFailure(Chapter1EncounterLifetimeOwner consumer,Chapter1AcceptedFailureToken token);
    }
    internal interface IChapter1HubRequestPort
    {
        void HandleHubRequest(Chapter1HubLifetimeOwner source, Chapter1HubRequestToken token);
    }
    internal sealed class Chapter1HubRequestToken
    {
        internal Chapter1HubRequestToken(Chapter1HubLifetimeOwner owner,InputRouter router,IChapter1HubRequestPort endpoint,
            long generation,long ordinal,DesktopProfileLaunchReceiptV1 prepared,UiSemanticFrameV1 frame,string intent,int? seed)
        {Owner=owner;Router=router;Endpoint=endpoint;Generation=generation;Ordinal=ordinal;Prepared=prepared;Frame=frame;Intent=intent;Seed=seed;
            InputUpdate=router==null?0:router.Chapter1HubFrameInputUpdate;NeutralReceipt=router==null?null:router.Chapter1HubNeutralReceipt;}
        internal Chapter1HubLifetimeOwner Owner {get;}
        internal InputRouter Router {get;}
        internal IChapter1HubRequestPort Endpoint {get;}
        internal long Generation {get;}
        internal long Ordinal {get;}
        internal DesktopProfileLaunchReceiptV1 Prepared {get;}
        internal UiSemanticFrameV1 Frame {get;}
        internal string Intent {get;}
        internal int? Seed {get;}
        internal uint InputUpdate {get;}
        internal InputFrameCommitReceipt? NeutralReceipt {get;}
    }

    // REQ-C1B-HUB-001: 실제 prepared UI frame만 받고 콜백을 추가하지 않는다.
    [DisallowMultipleComponent]
    internal sealed class Chapter1HubLifetimeOwner : MonoBehaviour
    {
        [SerializeField] private InputRouter _router;
        [SerializeField] private DesktopProfileLaunchAdapterV1 _adapter;
        private IChapter1HubRequestPort _endpoint;
        private Chapter1HubRequestToken _pending;
        // REQ-C1B-SRETRY-003: 실제 소비 객체와 일회 재무장은 값 복사로 대체하지 않는다.
        private Chapter1HubRequestToken _consumed,_armingToken;
        private bool _consumedRearmed,_armingRetry,_storageRetryArmed;
        private long _generation, _ordinal, _lastFrameOrdinal=-1;
        [SerializeField] private bool _configured;
        private bool _retiring, _dispatching;
        internal InputRouter Router => _router;
        internal long Generation => _generation;
        internal DesktopProfileLaunchAdapterV1 Adapter => _adapter;
        internal bool StorageRetryArmed=>_storageRetryArmed;
        internal bool PreparedForUi=>_configured && !_retiring && _endpoint!=null && _router!=null &&
            _adapter!=null && _adapter.Chapter1PreparedForUi;
        internal void ConfigureForAuthoring(InputRouter router,DesktopProfileLaunchAdapterV1 adapter,long generation)
        {
            if(gameObject.activeInHierarchy || _configured || router==null || adapter==null || generation<=0 || !ReferenceEquals(adapter.BoundRouter,router))
                throw new InvalidOperationException("거점 소유자는 실제 어댑터·라우터와 비활성 상태에서 결속합니다.");
            _router=router;_adapter=adapter;_generation=generation;_configured=true;
        }
        internal void BindRequestPort(IChapter1HubRequestPort endpoint)
        {
            if(gameObject.activeInHierarchy || _endpoint!=null || endpoint==null || _retiring || _pending!=null)
                throw new InvalidOperationException("활성 거점 요청 포트는 다시 결속할 수 없습니다.");
            _endpoint=endpoint;
        }
        internal void ConfigureInstanceGeneration(long generation)
        {
            if(gameObject.activeInHierarchy || !_configured || _endpoint!=null || generation<=0 || _retiring)
                throw new InvalidOperationException("거점 instance 세대는 실제 활성화 전에 발급합니다.");
            _generation=generation;
        }
        internal bool Ready => _configured && !_retiring && !_dispatching && _endpoint!=null && _router!=null &&
            !_router.IsFaulted && _router.Chapter1HubNeutralReady && _adapter.CurrentReceipt.HasValue &&
            !_adapter.CurrentReceipt.Value.PersistencePending && _router.CurrentUiFrame.HasValue;
        internal bool SubmitUiIntent(UiSemanticFrameV1 frame,string intent)
        {
            if(!Ready || _pending!=null)return false;
            frame.Validate();
            if(!_router.CurrentUiFrame.HasValue || !_router.CurrentUiFrame.Value.Equals(frame) ||
                frame.Receipt.FrameOrdinal<=_lastFrameOrdinal || (!frame.SubmitPressed && !frame.ClickPressed) ||
                !_router.IsChapter1HubIntentFrameAfterNeutral(this,frame))return false;
            int? seed=null;
            // REQ-C1B-SRETRY-005: 실패 거점은 seed 없는 재확인만 발급한다.
            if(_storageRetryArmed && intent!="chapter1.retry-expedition-storage")return false;
            if(!_storageRetryArmed && intent=="chapter1.retry-expedition-storage")return false;
            if(intent!="chapter1.start-expedition" && intent!="chapter1.return-to-hub" && intent!="chapter1.retry-expedition-storage")
            {
                switch(intent){case "chapter1.validation-seed-101":seed=101;break;case "chapter1.validation-seed-202":seed=202;break;
                    case "chapter1.validation-seed-303":seed=303;break;case "chapter1.validation-seed-404":seed=404;break;default:return false;}
            }
            var prepared=_adapter.CurrentReceipt.Value;prepared.Validate();
            _lastFrameOrdinal=frame.Receipt.FrameOrdinal;
            _pending=new Chapter1HubRequestToken(this,_router,_endpoint,_generation,checked(++_ordinal),prepared,frame,intent,seed);
            _dispatching=true;
            try{_endpoint.HandleHubRequest(this,_pending);}
            finally{_pending=null;_dispatching=false;}
            return true;
        }
        internal bool ValidatePendingRequest(IChapter1HubRequestPort endpoint,Chapter1HubRequestToken token)
        {
            if(!_configured || _retiring || !_dispatching || token==null || !ReferenceEquals(endpoint,_endpoint) ||
                !ReferenceEquals(token,_pending) || !ReferenceEquals(token.Owner,this) || !ReferenceEquals(token.Router,_router) ||
                !ReferenceEquals(token.Endpoint,_endpoint) || token.Generation!=_generation || token.Ordinal!=_ordinal ||
                !_adapter.CurrentReceipt.HasValue || !_adapter.CurrentReceipt.Value.Equals(token.Prepared) ||
                !_router.CurrentUiFrame.HasValue || !_router.CurrentUiFrame.Value.Equals(token.Frame) || !_router.Chapter1HubNeutralReady ||
                token.InputUpdate!=_router.Chapter1HubFrameInputUpdate || !token.NeutralReceipt.HasValue ||
                !_router.Chapter1HubNeutralReceipt.HasValue || !token.NeutralReceipt.Value.Equals(_router.Chapter1HubNeutralReceipt.Value) ||
                !_router.IsChapter1HubIntentFrameAfterNeutral(this,token.Frame))return false;
            token.Prepared.Validate();token.Frame.Validate();return !token.Prepared.PersistencePending;
        }
        internal void ConsumePendingRequest(IChapter1HubRequestPort endpoint,Chapter1HubRequestToken token)
        {
            if(!ValidatePendingRequest(endpoint,token))throw new InvalidOperationException("미발급·소비된 거점 요청입니다.");
            _consumed=token;_consumedRearmed=false;_pending=null;
        }
        internal void ArmExpeditionRetryAfterFailure(IChapter1HubRequestPort endpoint,Chapter1HubRequestToken consumedToken)
        {
            if(!_configured || _retiring || !_dispatching || _pending!=null || consumedToken==null || _consumedRearmed || _storageRetryArmed || _armingRetry ||
                !ReferenceEquals(endpoint,_endpoint) || !ReferenceEquals(consumedToken,_consumed) || !ReferenceEquals(consumedToken.Owner,this) ||
                !ReferenceEquals(consumedToken.Router,_router) || !ReferenceEquals(consumedToken.Endpoint,_endpoint) || consumedToken.Generation!=_generation || consumedToken.Ordinal!=_ordinal ||
                consumedToken.Intent=="chapter1.retry-expedition-storage" || consumedToken.Intent=="chapter1.return-to-hub" ||
                !_adapter.CurrentReceipt.HasValue || !_adapter.CurrentReceipt.Value.Equals(consumedToken.Prepared) || consumedToken.Prepared.PersistencePending ||
                !_router.CurrentUiFrame.HasValue || !_router.CurrentUiFrame.Value.Equals(consumedToken.Frame) || !_router.Chapter1HubNeutralReady)
                throw new InvalidOperationException("현재 동기 요청에서 실제 소비한 출정 원문만 한 번 재무장합니다.");
            consumedToken.Prepared.Validate();consumedToken.Frame.Validate();
            _consumedRearmed=true;_storageRetryArmed=true;_armingToken=consumedToken;_armingRetry=true;
            try{_router.RearmChapter1HubNeutralAfterStorageFailure(this,consumedToken);}
            finally{_armingRetry=false;_armingToken=null;}
        }
        internal bool IsArmingStorageRetry(InputRouter router,Chapter1HubRequestToken token)=>_armingRetry && _dispatching && _pending==null &&
            _consumedRearmed && _storageRetryArmed && !_retiring && ReferenceEquals(router,_router) && ReferenceEquals(token,_consumed) && ReferenceEquals(token,_armingToken);
        internal Func<bool> BeginRetirement()
        {
            if(_retiring || _pending!=null || _router==null)throw new InvalidOperationException("거점 입력 정리 경계가 올바르지 않습니다.");
            var closed=_router.GetChapter1HubClosureReader();_retiring=true;
            gameObject.SetActive(false);Destroy(gameObject);return closed;
        }
    }
}
