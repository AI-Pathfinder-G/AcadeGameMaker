using System;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Run.Unity;
using TMPro;
using UnityEngine;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    // REQ-C1B-HUB-001: 표시와 입력 의도만 소유하며 화면 버튼 자체로 방 완료를 만들지 않는다.
    [DefaultExecutionOrder(220)]
    internal sealed class Chapter1ExpeditionPresenter : MonoBehaviour
    {
        [SerializeField] private Chapter1PlayableFlowOwner _flow;
        [SerializeField] private TMP_Text _status;
        [SerializeField] private TMP_Text[] _labels;
        [SerializeField] private RectTransform[] _buttons;
        [SerializeField] private HubSafeFrameScalerV1 _scaler;
        private Chapter1HubLifetimeOwner _source;
        private UiSemanticFrameCursorV1 _cursor;
        private int _focus;
        private UnityEngine.Camera _presentationClearCamera;
#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        private const bool ValidationMenuEnabled=true;
#else
        private const bool ValidationMenuEnabled=false;
#endif
        private static readonly string[] Intents={"chapter1.start-expedition","chapter1.validation-seed-101","chapter1.validation-seed-202","chapter1.validation-seed-303","chapter1.validation-seed-404","chapter1.return-to-hub"};
        private static readonly string[] Copy={"출정","검증 원정 101","검증 원정 202","검증 원정 303","검증 원정 404","거점으로"};
        internal void ConfigureForAuthoring(Chapter1PlayableFlowOwner flow,TMP_Text status,TMP_Text[] labels,RectTransform[] buttons,HubSafeFrameScalerV1 scaler)
        {if(_flow!=null || flow==null || status==null || labels==null || buttons==null || labels.Length!=6 || buttons.Length!=6 || scaler==null)throw new ArgumentException("원정 화면 구성은 정확 여섯 버튼이 필요합니다.");
            _flow=flow;_status=status;_labels=(TMP_Text[])labels.Clone();_buttons=(RectTransform[])buttons.Clone();_scaler=scaler;}
        // REQ-AUTOCH1-PRESENTATION-CLEAR-001: UI-only phases still need a
        // color+depth clear after the room-owned world camera has retired.
        // This untagged camera draws no world layers and renders before them.
        private void OnEnable()
        {
            if(_presentationClearCamera==null)
            {
                var clearRoot=new GameObject("Chapter1PresentationClear");
                clearRoot.transform.SetParent(transform,false);
                _presentationClearCamera=clearRoot.AddComponent<UnityEngine.Camera>();
                _presentationClearCamera.enabled=false;
                _presentationClearCamera.clearFlags=CameraClearFlags.SolidColor;
                _presentationClearCamera.backgroundColor=Color.black;
                _presentationClearCamera.cullingMask=0;
                _presentationClearCamera.depth=-1000;
                _presentationClearCamera.orthographic=true;
                _presentationClearCamera.nearClipPlane=.01f;
                _presentationClearCamera.farClipPlane=1;
                _presentationClearCamera.allowHDR=false;
                _presentationClearCamera.allowMSAA=false;
            }
            _presentationClearCamera.enabled=true;
        }
        private void OnDisable()
        {
            if(_presentationClearCamera!=null)_presentationClearCamera.enabled=false;
        }
        private void FixedUpdate()
        {
            if(_flow==null)return;
            var hub=_flow.CurrentHub;
            if(!ReferenceEquals(hub,_source))
            {_source=hub;_cursor=null;_focus=0;}
            if(hub==null || !hub.PreparedForUi){_cursor=null;return;}
            try
            {
                if(_cursor==null)_cursor=UiSemanticFrameCursorV1.Create(hub.Router);
                // 중립 대기에서도 cursor는 실제 모든 의미 frame을 소비하여 오래된 edge를 보류하지 않는다.
                if(!_cursor.TryAdvance(hub.Router,out var frame))return;
                if(!((frame.NavigateChanged && frame.NavigateYQ4096!=0)||frame.ClickPressed||frame.SubmitPressed))return;
                if(!hub.Ready || !_scaler.IsSupported)return;
                var phase=_flow.View.Phase;
                var failed=phase==Chapter1RunPhase.ExpeditionStorageFailed;
                if(!failed && phase!=Chapter1RunPhase.Hub && phase!=Chapter1RunPhase.ReturnedToHub)return;
                var returned=phase==Chapter1RunPhase.ReturnedToHub;
                if(failed)_focus=0;
                if(ValidationMenuEnabled && !failed && frame.NavigateChanged && frame.NavigateYQ4096!=0)
                    _focus=returned?5:(_focus+(frame.NavigateYQ4096<0?1:4))%5;
                if(returned)_focus=5;
                var chosen=-1;
                if(frame.ClickPressed && frame.HasClickPoint)
                {
                    var actual=new Vector2(frame.ClickPointXActualPx,frame.ClickPointYActualPx);
                    for(var i=0;i<6;i++)if((ValidationMenuEnabled || i==0 || i==5) && (failed?i==0:(i==5)==returned) && Hit(i,actual)){chosen=i;break;}
                }
                if(chosen<0 && frame.SubmitPressed)chosen=_focus;
                if(chosen>=0)hub.SubmitUiIntent(frame,failed?"chapter1.retry-expedition-storage":Intents[chosen]);
            }
            catch(Exception e){_status.text="입력 표시 중단: "+e.Message;enabled=false;}
        }
        private bool Hit(int index,Vector2 actual)
        {
            var scale=_scaler.IntegerScale;if(scale<=0)return false;
            var safe=_scaler.SafeRectActual;var point=(actual-safe.position)/scale-new Vector2(320,180);
            var rect=_buttons[index];var bounds=new Rect(rect.anchoredPosition-rect.sizeDelta/2,rect.sizeDelta);
            return rect.sizeDelta.x>=24 && rect.sizeDelta.y>=24 && bounds.Contains(point);
        }
        private void LateUpdate()
        {
            if(_flow==null)return;_scaler.Refresh();var view=_flow.View;
            var ownStatus=view.Phase<Chapter1RunPhase.PreparingChoice || view.Phase==Chapter1RunPhase.AtBarrier || view.Phase==Chapter1RunPhase.EndingSuccessfulRun;
            if(_status.gameObject.activeSelf!=ownStatus)_status.gameObject.SetActive(ownStatus);
            var hub=view.Phase==Chapter1RunPhase.Hub || view.Phase==Chapter1RunPhase.ReturnedToHub || view.Phase==Chapter1RunPhase.PreparingHub;
            var returned=view.Phase==Chapter1RunPhase.ReturnedToHub;
            var failed=view.Phase==Chapter1RunPhase.ExpeditionStorageFailed;
            var recovering=view.Phase==Chapter1RunPhase.RetiringHubForStorageRetry || view.Phase==Chapter1RunPhase.PreparingHubAfterStorageRetry;
            if(failed || recovering)_focus=0;
            if(returned)_focus=5;
            // REQ-C1B-SRETRY-006: 기존 버튼0의 동일 hit rect만 사용한다. 복구 중 버튼은 잠근다.
            for(var i=0;i<6;i++){_buttons[i].gameObject.SetActive((ValidationMenuEnabled || i==0 || i==5) && ((failed || recovering)?i==0:hub && (i==5)==returned));
                _labels[i].text=(i==_focus?"▶ ":"")+((failed || recovering) && i==0?"저장 다시 확인":Copy[i]);
                _labels[i].color=view.HubNeutral && !recovering?Color.white:Color.gray;}
            switch(view.Phase)
            {
                case Chapter1RunPhase.Hub:case Chapter1RunPhase.PreparingHub:_status.text=view.HubNeutral?"거점 · 출정 준비\n이동 선택 · 확인 또는 클릭으로 출정":"입력을 모두 놓으면 출정할 수 있습니다.";break;
                case Chapter1RunPhase.ReturnedToHub:_status.text="원정 실패 · 보존 저장 완료\n입력을 놓은 뒤 거점으로 돌아가 새로 출정합니다.";break;
                case Chapter1RunPhase.ExpeditionStorageFailed:_status.text=view.Error+"\n"+(view.HubNeutral?"확인 또는 클릭으로 저장을 다시 확인합니다.":"입력을 모두 놓으면 저장을 다시 확인할 수 있습니다.");break;
                case Chapter1RunPhase.RetiringHubForStorageRetry:_status.text=view.Error+"\n이전 거점의 입력 종료를 기다립니다.";break;
                case Chapter1RunPhase.PreparingHubAfterStorageRetry:_status.text=view.Error+"\n새 거점의 저장 준비와 새 중립 입력을 기다립니다.";break;
                case Chapter1RunPhase.Diagnostic:_status.text="원정을 진행할 수 없습니다.\n"+view.Error;break;
                case Chapter1RunPhase.LockedPostBoss:_status.text="보스 구간 정리 완료 · 후속 연결 대기\n현재 단계는 1챕터 완료가 아닙니다.";break;
                case Chapter1RunPhase.AtBarrier:_status.text="체력 "+view.Health+"/5 · 봉쇄선\n상자에 오른쪽 클릭으로 전이한 뒤 Q로 선택 기술을 사용해 길을 열고 출구로 이동합니다.";break;
                default:_status.text="체력 "+view.Health+"/5\n이동 WASD · 점프 Space · 대시 Shift · 공격 왼쪽 클릭 · 전이 오른쪽 클릭 · 선택 기술 Q";break;
            }
            if(hub && view.LastStorageFailure!=null)_status.text+="\n이전 출정 오류: "+view.LastStorageFailure.Error;
        }
        // REQ-AUTOCH1-009/011: temporary presentation reads the actual live
        // collider geometry; it does not create gameplay or a second lifetime.
        private void OnGUI()
        {
            if(_flow==null)return;var phase=_flow.View.Phase;
            if(phase!=Chapter1RunPhase.AtBoss && phase!=Chapter1RunPhase.BossCleanup && phase!=Chapter1RunPhase.AtBarrier && phase!=Chapter1RunPhase.EndingSuccessfulRun)return;
            var router=_flow.Lifetime.CurrentRouter;if(router==null)return;
            var graph=router.transform.parent;if(graph==null)return;var camera=graph.GetComponentInChildren<UnityEngine.Camera>(true);if(camera==null)return;
            var old=GUI.color;
            foreach(var collider in graph.GetComponentsInChildren<Collider2D>())
            {
                if(!collider.enabled||!collider.gameObject.activeInHierarchy)continue;
                var name=collider.gameObject.name;var lower=name.ToLowerInvariant();
                if(collider.isTrigger && !lower.Contains("exit") && !lower.Contains("safety"))continue;
                var bounds=collider.bounds;var a=camera.WorldToScreenPoint(bounds.min);var b=camera.WorldToScreenPoint(bounds.max);
                if(a.z<0||b.z<0)continue;
                var rect=new Rect(Mathf.Min(a.x,b.x),Screen.height-Mathf.Max(a.y,b.y),Mathf.Abs(b.x-a.x),Mathf.Abs(b.y-a.y));
                if(rect.width<=0||rect.height<=0)continue;
                var player=lower.Contains("player");var boss=Chapter1OrdanArt.IsOwned(graph,collider);
                var box=lower.Contains("counterweight")||lower.Contains("box");var exit=lower.Contains("exit");var gate=lower.Contains("gate");
                GUI.color=player?new Color(.25f,.7f,1f):boss?new Color(.95f,.3f,.35f):box?new Color(.75f,.55f,.95f):exit?new Color(.3f,1f,.6f,.6f):gate?new Color(1f,.65f,.2f):new Color(.6f,.65f,.7f,.8f);
                if(!Chapter1OrdanArt.Draw(rect,graph,collider) && !Chapter1DoeonArt.DrawPlayer(rect,collider) && !Chapter1IndustrialArt.DrawEnvironment(rect,collider))GUI.DrawTexture(rect,Texture2D.whiteTexture);GUI.color=Color.white;
                if(player||boss||box||exit||gate)GUI.Label(new Rect(rect.x,rect.y-22,Mathf.Max(90,rect.width),24),player?"도언":boss?"오르단":box?"전이 가능한 상자":exit?"출구":"봉쇄선");
            }
            GUI.color=old;
        }
    }
}
