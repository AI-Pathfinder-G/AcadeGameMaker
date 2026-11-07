using System;
using AcadeGameMaker.Input.Unity;
using TMPro;
using UnityEngine;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    // REQ-C1C-004/005. 실제 의미 frame으로 의도를 제출하며 저장·기술 부여 권한은 없다.
    [DefaultExecutionOrder(220)]
    internal sealed class Chapter1ChoicePresenter : MonoBehaviour,IChapter1AuthoredViewPort
    {
        private Chapter1ChoicePhaseAdapter _source;
        [SerializeField] private TMP_Text _status;
        [SerializeField] private TMP_Text[] _labels;
        [SerializeField] private RectTransform[] _buttons;
        [SerializeField] private HubSafeFrameScalerV1 _scaler;
        private UiSemanticFrameCursorV1 _cursor;
        private int _focus;
        private bool _configured;
        private static readonly string[] Intents={"chapter1.choice-extraction","chapter1.choice-solidarity","chapter1.choice-confirm","chapter1.choice-cancel","chapter1.choice-save-retry"};
        private static readonly string[] Labels={"수탈","연대","선택 확정","취소","저장 다시 시도"};
        internal void ConfigureForAuthoring(Chapter1ChoicePhaseAdapter source,TMP_Text status,TMP_Text[] labels,RectTransform[] buttons,HubSafeFrameScalerV1 scaler)
        {
            if(gameObject.activeInHierarchy || _configured || source==null || status==null || labels==null || buttons==null || labels.Length!=5 || buttons.Length!=5 || scaler==null)
                throw new InvalidOperationException("선택 표시에는 정확 다섯 버튼이 필요합니다.");
            for(var i=0;i<5;i++)if(labels[i]==null || buttons[i]==null || buttons[i].sizeDelta.x<24 || buttons[i].sizeDelta.y<24)throw new InvalidOperationException("선택 hit 영역이 작거나 없습니다.");
            _source=source;_status=status;_labels=(TMP_Text[])labels.Clone();_buttons=(RectTransform[])buttons.Clone();_scaler=scaler;_configured=true;
        }
        internal void ConfigureStaticForAuthoring(TMP_Text status,TMP_Text[] labels,RectTransform[] buttons,HubSafeFrameScalerV1 scaler)
        {if(gameObject.activeInHierarchy||_configured||status==null||labels==null||labels.Length!=5||buttons==null||buttons.Length!=5||scaler==null)throw new InvalidOperationException("선택 정적 표시가 유효하지 않습니다.");_status=status;_labels=labels;_buttons=buttons;_scaler=scaler;}
        internal void ConfigureFromAuthored(Chapter1ChoicePhaseAdapter source)=>ConfigureForAuthoring(source,_status,_labels,_buttons,_scaler);
        void IChapter1AuthoredViewPort.BindChapter1AuthoredView(object flow,Chapter1ChoicePhaseAdapter source)
        {if(!(flow is IChapter1ChoiceUiPort))throw new InvalidOperationException("현재 선택 소유자가 아닙니다.");ConfigureFromAuthored(source);}
        private void FixedUpdate()
        {
            if(!_configured)return;
            var preparation=_source.Router.GetComponent<DesktopProfileLaunchAdapterV1>();
            if(preparation!=null && !preparation.Chapter1PreparedForUi)return;
            try
            {
                if(_cursor==null)_cursor=UiSemanticFrameCursorV1.Create(_source.Router);
                if(!_cursor.TryAdvance(_source.Router,out var frame) || !_scaler.IsSupported)return;
                var phase=_source.UiPhase;var available=Available(phase);
                if(frame.NavigateChanged && frame.NavigateYQ4096!=0)
                {
                    var direction=frame.NavigateYQ4096<0?1:-1;
                    for(var i=0;i<5;i++){_focus=(_focus+direction+5)%5;if(available[_focus])break;}
                }
                var selected=-1;
                if(frame.CancelPressed && available[3])selected=3;
                else if(frame.ClickPressed && frame.HasClickPoint)
                {var actual=new Vector2(frame.ClickPointXActualPx,frame.ClickPointYActualPx);for(var i=0;i<5;i++)if(available[i] && Hit(i,actual)){selected=i;break;}}
                if(selected<0 && frame.SubmitPressed && available[_focus])selected=_focus;
                // 빈 실제 frame도 전달하여 neutral을 관측한다. 의도 없는 frame은 token을 발급하지 않는다.
                _source.SubmitUiIntent(frame,selected<0?string.Empty:Intents[selected]);
            }
            catch(Exception e){_status.text="선택 입력을 진행할 수 없습니다.\n"+e.Message;enabled=false;}
        }
        private bool Hit(int index,Vector2 actual)
        {
            var scale=_scaler.IntegerScale;if(scale<=0)return false;
            var safe=_scaler.SafeRectActual;var point=(actual-safe.position)/scale-new Vector2(320,180);
            var rect=_buttons[index];return new Rect(rect.anchoredPosition-rect.sizeDelta/2,rect.sizeDelta).Contains(point);
        }
        private void LateUpdate()
        {
            if(!_configured)return;_scaler.Refresh();var phase=_source.UiPhase;var available=Available(phase);
            for(var i=0;i<5;i++){_buttons[i].gameObject.SetActive(available[i]);_labels[i].text=(i==_focus?"▶ ":"")+Labels[i];}
            const string opening="유담: 이 장부에는 마흔일곱 사람의 허락이 담겨 있어. 네게 소유권을 넘기지는 않겠어.\n도언: 네 선택을 남길지, 네 각인을 끊어 힘을 가져갈지 결정해야 하는군.";
            switch(phase)
            {
                case Chapter1ChoiceUiPhase.Required:_status.text=opening;break;
                case Chapter1ChoiceUiPhase.ExtractionProposed:_status.text=opening+"\n수탈 — 유담의 거부를 무시하고 각인을 절단합니다.";break;
                case Chapter1ChoiceUiPhase.SolidarityProposed:_status.text=opening+"\n유담: 봉쇄선을 지나는 데 필요한 공명만 빌려주겠어. 장부와 선택은 내게 남겨 둬.\n연대 — 유담이 허락한 범위의 공명만 빌립니다.";break;
                case Chapter1ChoiceUiPhase.SaveFailed:_status.text="저장에 실패했습니다. 다시 시도해 주세요";break;
                case Chapter1ChoiceUiPhase.SaveUncertain:_status.text="저장 결과를 확인하고 있습니다";break;
                default:_status.text=string.Empty;break;
            }
        }
        private static bool[] Available(Chapter1ChoiceUiPhase phase)
        {
            var selecting=phase==Chapter1ChoiceUiPhase.Required || phase==Chapter1ChoiceUiPhase.ExtractionProposed || phase==Chapter1ChoiceUiPhase.SolidarityProposed;
            var proposed=phase==Chapter1ChoiceUiPhase.ExtractionProposed || phase==Chapter1ChoiceUiPhase.SolidarityProposed;
            return new[]{selecting,selecting,proposed,proposed,phase==Chapter1ChoiceUiPhase.SaveFailed};
        }
        private void OnDisable(){_cursor=null;}
    }
}
