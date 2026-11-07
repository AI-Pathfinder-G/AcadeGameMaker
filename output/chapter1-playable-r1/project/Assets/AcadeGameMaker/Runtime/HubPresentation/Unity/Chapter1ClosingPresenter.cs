using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using AcadeGameMaker.Run.Unity;
using TMPro;
using UnityEngine;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    // REQ-C1C-005, AC-CH1-007. 실제 저장·통과 뒤 다섯 줄을 새 의미 edge로 재생한다.
    [DefaultExecutionOrder(220)]
    internal sealed class Chapter1ClosingPresenter : MonoBehaviour, IChapter1ClosingViewPort,IChapter1AuthoredViewPort
    {
        [Serializable] private sealed class DialogueLine { public string id; public string route; public string speaker; public string text; }
        [Serializable] private sealed class DialogueDocument { public int version; public int minimumDisplaySimulationTicks; public DialogueLine[] lines; public string[] closingExtraction; public string[] closingSolidarity; public string[] ended; }
        private Chapter1PlayableFlowOwner _flow;
        void IChapter1AuthoredViewPort.BindChapter1AuthoredView(object flow,Chapter1ChoicePhaseAdapter source)
        {if(!(flow is Chapter1PlayableFlowOwner owner))throw new InvalidOperationException("현재 종결 소유자가 아닙니다.");ConfigureFromAuthored(owner,source);}
        private Chapter1ChoicePhaseAdapter _source;
        [SerializeField] private TMP_Text _status,_buttonLabel;
        [SerializeField] private RectTransform _button;
        [SerializeField] private HubSafeFrameScalerV1 _scaler;
        // REQ/AC-C1C-CLOSEAUTH-001: 정적 표시 참조만 보존하며 실행 권위는 보존하지 않는다.
        [SerializeField] private TextAsset _authoredNarrative;
        [SerializeField] private bool _staticConfigured;
        private UiSemanticFrameCursorV1 _cursor;
        private DialogueDocument _document;
        private string _assetSha256;
        private readonly Dictionary<string,DialogueLine> _lines=new Dictionary<string,DialogueLine>(StringComparer.Ordinal);
        private string[] _sequence;
        private int _index,_displayStartTick;
        private long _playbackOrdinal;
        private bool _configured,_playing,_retired,_receiptConsumed,_completionLinked,_diagnostic;
        private string _diagnosticMessage;
        private Chapter1ClosingHostEntry _entry;
        private long _finalAdvanceOrdinal;
        private Chapter1DemoEndReceipt _demo;
        private Chapter1BranchSaveReceipt _save;
        private Chapter1ClosingReceipt _pending;
        private Chapter1CompletionReceipt _completion;
        internal string ClosingAssetSha256 => _assetSha256;

        // REQ/AC-C1C-CLOSEAUTH-001: 실제 host 없이 정적 자산만 저작한다.
        internal void ConfigureStaticForAuthoring(TextAsset narrative, TMP_Text status, TMP_Text buttonLabel,
            RectTransform button, HubSafeFrameScalerV1 scaler)
        {
            if(gameObject.activeInHierarchy || _configured || _staticConfigured)
                throw new InvalidOperationException("정적 마지막 장면은 비활성 상태에서 한 번만 저작합니다.");
            ValidateStaticReferences(narrative,status,buttonLabel,button,scaler);
            _authoredNarrative=narrative;_status=status;_buttonLabel=buttonLabel;_button=button;_scaler=scaler;_staticConfigured=true;
        }
        internal void ConfigureFromAuthored(Chapter1PlayableFlowOwner flow, Chapter1ChoicePhaseAdapter source)
        {
            if(!_staticConfigured)throw new InvalidOperationException("정적 마지막 장면의 저작 참조가 없습니다.");
            ValidateStaticReferences(_authoredNarrative,_status,_buttonLabel,_button,_scaler);
            ConfigureForAuthoring(flow,source,_authoredNarrative,_status,_buttonLabel,_button,_scaler);
        }
        private void ValidateStaticReferences(TextAsset narrative,TMP_Text status,TMP_Text buttonLabel,
            RectTransform button,HubSafeFrameScalerV1 scaler)
        {
            if(narrative==null || status==null || buttonLabel==null || button==null || scaler==null ||
                status.font==null || buttonLabel.font==null || button.sizeDelta.x<24 || button.sizeDelta.y<24 ||
                !status.transform.IsChildOf(transform) || !buttonLabel.transform.IsChildOf(transform) ||
                !button.IsChildOf(transform) || (scaler.transform!=transform && !scaler.transform.IsChildOf(transform)))
                throw new InvalidOperationException("자기 프리팹의 유효한 서사·표시·버튼·스케일러가 필요합니다.");
            ValidateDocument(JsonUtility.FromJson<DialogueDocument>(narrative.text));
        }

        internal void ConfigureForAuthoring(Chapter1PlayableFlowOwner flow, Chapter1ChoicePhaseAdapter source,
            TextAsset narrative,TMP_Text status,TMP_Text buttonLabel,RectTransform button,HubSafeFrameScalerV1 scaler)
        {
            if(gameObject.activeInHierarchy || _configured || flow==null || source==null || narrative==null || status==null ||
                buttonLabel==null || button==null || scaler==null || source.ClosingHostEntry==null || button.sizeDelta.x<24 || button.sizeDelta.y<24)
                throw new InvalidOperationException("마지막 장면의 실제 서사와 표시 영역이 없습니다.");
            var document=JsonUtility.FromJson<DialogueDocument>(narrative.text);ValidateDocument(document);
            using(var hash=SHA256.Create())_assetSha256=BitConverter.ToString(hash.ComputeHash(narrative.bytes)).Replace("-",string.Empty);
            _document=document;foreach(var line in document.lines)_lines.Add(line.id,line);
            _flow=flow;_source=source;_entry=source.ClosingHostEntry;_status=status;_buttonLabel=buttonLabel;_button=button;_scaler=scaler;_configured=true;
        }
        void IChapter1ClosingViewPort.BeginFromCommittedRun(object demo,object branchSave)
        {
            if(!(demo is Chapter1DemoEndReceipt actualDemo) || !(branchSave is Chapter1BranchSaveReceipt actualSave))
                throw new InvalidOperationException("현재 원정의 실제 종료·저장 객체가 아닙니다.");
            BeginFromCommittedRun(actualDemo,actualSave);
        }
        bool IChapter1ClosingViewPort.AdvanceFromPendingUi(Chapter1ChoicePhaseAdapter source,Chapter1ChoiceUiToken token)
            => AdvanceFromPendingUi(source,token);
        bool IChapter1ClosingViewPort.OwnsPendingClosingReceipt(object receipt)
            => receipt is Chapter1ClosingReceipt closing && ValidatePendingReceipt(closing);
        void IChapter1ClosingViewPort.ConsumePendingClosingReceipt(object receipt)
        {
            if(!(receipt is Chapter1ClosingReceipt closing) || !ValidatePendingReceipt(closing))
                throw new InvalidOperationException("현재 마지막 대사의 미소비 원본이 아닙니다.");
            _receiptConsumed=true;
        }
        // REQ-C1C-UIR-003. 실제 FlowOwner retained 원본 및 Issued entry 양측을 검증한다.
        internal void BeginFromCommittedRun(Chapter1DemoEndReceipt demo,Chapter1BranchSaveReceipt save)
        {
            if(!_configured || _retired || _diagnostic || _playing || _receiptConsumed || _pending!=null || _completion!=null || demo==null || save==null ||
                _source.UiPhase!=Chapter1ChoiceUiPhase.Closing || !ReferenceEquals(_source.ClosingHostEntry,_entry) ||
                !ReferenceEquals(_entry.Endpoint,_flow) || !_source.OwnsIssuedClosingHost || !ReferenceEquals(_entry.Demo,demo) || !ReferenceEquals(_entry.BranchSave,save) ||
                !ReferenceEquals(_entry.RunKey,demo.RunKey) || _entry.OldDemoGeneration!=demo.Generation ||
                _entry.NewUiGeneration!=checked(demo.Generation+1) || save.ExpectedRevision!=save.CommittedRevision ||
                (save.Outcome!=ProfileAtomicSaveOutcome.CommittedFirst && save.Outcome!=ProfileAtomicSaveOutcome.CommittedReplacement) ||
                !ReferenceEquals(demo.Owner,_flow) || !ReferenceEquals(save.Owner,_flow) || !ReferenceEquals(save.Demo,demo) ||
                !_flow.ValidatePendingChapter1ClosingStart(this,demo,save) || !_source.Router.CurrentUiFrame.HasValue)
                throw new InvalidOperationException("현재 실행의 실제 분기 저장과 마지막 장면 입력이 준비되지 않았습니다.");
            var frame=_source.Router.CurrentUiFrame.Value;frame.Validate();
            if(frame.Receipt.Tick.Value<_entry.FirstUiTick.Value || !_source.Router.CurrentReceipt.HasValue ||
                !_source.Router.CurrentReceipt.Value.Equals(frame.Receipt))throw new InvalidOperationException("새 UI 시계의 실제 입력이 아닙니다.");
            var nextPlayback=checked(_playbackOrdinal+1);
            var cursor=UiSemanticFrameCursorV1.Create(_source.Router);
            _demo=demo;_save=save;_sequence=demo.Branch==ProfileBranch.Extraction?_document.closingExtraction:_document.closingSolidarity;
            _playbackOrdinal=nextPlayback;_index=0;_displayStartTick=frame.Receipt.Tick.Value;_playing=true;
            _cursor=cursor;
        }
        internal bool AdvanceFromPendingUi(Chapter1ChoicePhaseAdapter source,Chapter1ChoiceUiToken token)
        {
            if(_retired || _diagnostic || !_playing || _receiptConsumed || _pending!=null || token==null || !ReferenceEquals(source,_source) || token.Intent!="chapter1.closing-next" ||
                !source.ValidatePendingUi(_flow,token) || !ReferenceEquals(token.RunKey,_demo.RunKey) || token.Generation!=_entry.NewUiGeneration || !_source.OwnsIssuedClosingHost ||
                !ReferenceEquals(source.ClosingHostEntry,_entry) || !ReferenceEquals(_entry.Endpoint,_flow) ||
                !ReferenceEquals(_entry.Demo,_demo) || !ReferenceEquals(_entry.BranchSave,_save) ||
                _entry.OldDemoGeneration!=_demo.Generation || _entry.NewUiGeneration!=checked(_demo.Generation+1) ||
                !_flow.ValidatePendingChapter1ClosingStart(this,_demo,_save))return false;
            if(checked(token.Frame.Receipt.Tick.Value-_displayStartTick)<60)return false;
            Chapter1ClosingReceipt prepared=null;
            if(_index==_sequence.Length-1)
                prepared=new Chapter1ClosingReceipt(this,_demo,_entry,_assetSha256,_playbackOrdinal,token.RequestOrdinal);
            source.ConsumePendingUi(_flow,token);
            if(_index<_sequence.Length-1){_index++;_displayStartTick=token.Frame.Receipt.Tick.Value;return true;}
            _playing=false;
            _finalAdvanceOrdinal=token.RequestOrdinal;_pending=prepared;
            try
            {
                _flow.TryCommitChapterCompletion(_demo,_pending,_save);
                // REQ-C1C-UIR-001/003, REQ-C1C-005/006. 발급 원자 구간이 끝난 뒤 원본을 검증하고 따로 통지한다.
                var actual=_flow.CurrentChapter1CompletionReceipt;
                if(!MatchesCommittedCompletion(actual))throw new InvalidOperationException("실제 마지막 대사와 확정 저장의 원본 완료가 발급되지 않았습니다.");
                _completion=actual;
                _source.CompleteClosingFromCommittedReceipt(_flow,_entry,actual);
                _completionLinked=true;
            }
            catch(Exception e)
            {
                // 이미 발행한 FlowOwner 완료와 소비 원본을 보존한다. 자동 재발급·재통지·복귀는 없다.
                StopDiagnostic(e);throw;
            }
            return true;
        }
        private bool MatchesCommittedCompletion(Chapter1CompletionReceipt receipt) => !_retired && !_diagnostic && _receiptConsumed &&
            receipt!=null && ReferenceEquals(receipt,_flow.CurrentChapter1CompletionReceipt) &&
            ReferenceEquals(receipt.Owner,_flow) && ReferenceEquals(receipt.Demo,_demo) && ReferenceEquals(receipt.Save,_save) &&
            ReferenceEquals(receipt.Closing,_pending) && ReferenceEquals(_pending.Presenter,this) &&
            ReferenceEquals(_pending.Demo,_demo) && ReferenceEquals(_pending.HostEntry,_entry) &&
            ReferenceEquals(_entry.Demo,_demo) && ReferenceEquals(_entry.BranchSave,_save) &&
            ReferenceEquals(_entry.RunKey,_demo.RunKey) && ReferenceEquals(_entry.Endpoint,_flow) &&
            ReferenceEquals(_source.ClosingHostEntry,_entry) && _source.OwnsIssuedClosingHost &&
            ReferenceEquals(_demo.Owner,_flow) && ReferenceEquals(_save.Owner,_flow) && ReferenceEquals(_save.Demo,_demo) &&
            _save.ExpectedRevision==_save.CommittedRevision &&
            (_save.Outcome==ProfileAtomicSaveOutcome.CommittedFirst || _save.Outcome==ProfileAtomicSaveOutcome.CommittedReplacement) &&
            _pending.AssetSha256==_assetSha256 && _pending.PlaybackOrdinal==_playbackOrdinal &&
            _pending.FinalAdvanceOrdinal==_finalAdvanceOrdinal && _finalAdvanceOrdinal>0 && !_playing && _index==_sequence.Length-1 &&
            _pending.DemoGeneration==_demo.Generation && _entry.OldDemoGeneration==_demo.Generation &&
            _pending.UiGeneration==_entry.NewUiGeneration && _entry.NewUiGeneration==checked(_demo.Generation+1) &&
            ((IChapter1ChoiceUiPort)_flow).ValidateCommittedChapter1Completion(_source,_entry,receipt);
        internal bool ValidatePendingReceipt(Chapter1ClosingReceipt receipt) => !_retired && !_diagnostic && !_receiptConsumed && _source.OwnsIssuedClosingHost && receipt!=null && ReferenceEquals(receipt,_pending) &&
            ReferenceEquals(_source.ClosingHostEntry,_entry) && ReferenceEquals(_entry.Endpoint,_flow) &&
            ReferenceEquals(_entry.Demo,_demo) && ReferenceEquals(_entry.BranchSave,_save) && ReferenceEquals(_entry.RunKey,_demo.RunKey) &&
            _entry.OldDemoGeneration==_demo.Generation && _entry.NewUiGeneration==checked(_demo.Generation+1) &&
            _save.ExpectedRevision==_save.CommittedRevision &&
            (_save.Outcome==ProfileAtomicSaveOutcome.CommittedFirst || _save.Outcome==ProfileAtomicSaveOutcome.CommittedReplacement) &&
            ReferenceEquals(receipt.Presenter,this) && ReferenceEquals(receipt.Demo,_demo) && receipt.AssetSha256==_assetSha256 &&
            receipt.PlaybackOrdinal==_playbackOrdinal && receipt.FinalAdvanceOrdinal==_finalAdvanceOrdinal &&
            ReferenceEquals(receipt.HostEntry,_entry) && receipt.DemoGeneration==_demo.Generation && receipt.UiGeneration==_entry.NewUiGeneration && !_playing && _index==_sequence.Length-1;
        private void FixedUpdate()
        {
            if(!_configured || _retired || _diagnostic || _cursor==null || (!_playing && !_completionLinked))return;
            try
            {
                if(_completionLinked && (_source.UiPhase!=Chapter1ChoiceUiPhase.Ended || !MatchesCommittedCompletion(_completion)))
                    throw new InvalidOperationException("복귀 입력의 현재 원본 완료를 확인할 수 없습니다.");
                if(!_cursor.TryAdvance(_source.Router,out var frame) || !_scaler.IsSupported)return;
                var selected=frame.SubmitPressed || (frame.ClickPressed && frame.HasClickPoint && Hit(new Vector2(frame.ClickPointXActualPx,frame.ClickPointYActualPx)));
                var intent=_completionLinked?"chapter1.completed-return":"chapter1.closing-next";
                _source.SubmitUiIntent(frame,selected?intent:string.Empty);
            }
            catch(Exception e){StopDiagnostic(e);}
        }
        private void StopDiagnostic(Exception error)
        {
            if(_diagnostic)return;
            _diagnostic=true;_diagnosticMessage="마지막 장면을 진행할 수 없습니다.\n"+error.Message;
            _playing=false;_cursor=null;_button.gameObject.SetActive(false);_status.text=_diagnosticMessage;_buttonLabel.text=string.Empty;
            enabled=false;
        }
        private bool Hit(Vector2 actual)
        {
            var scale=_scaler.IntegerScale;if(scale<=0)return false;
            var point=(actual-_scaler.SafeRectActual.position)/scale-new Vector2(320,180);
            return new Rect(_button.anchoredPosition-_button.sizeDelta/2,_button.sizeDelta).Contains(point);
        }
        private void LateUpdate()
        {
            if(!_configured)return;
            if(_diagnostic){_button.gameObject.SetActive(false);_status.text=_diagnosticMessage;return;}
            if(_retired)return;
            try
            {
                _scaler.Refresh();_button.gameObject.SetActive((_playing || _completionLinked) && _source.ClosingControlsNeutralReady);
                if(_completionLinked)
                {
                    if(_source.UiPhase!=Chapter1ChoiceUiPhase.Ended || !MatchesCommittedCompletion(_completion))
                        throw new InvalidOperationException("완료 결과의 원본과 종료 입력 연결을 확인할 수 없습니다.");
                    _status.text=_lines["C02"].text+"\n"+_lines["C03"].text;_buttonLabel.text="거점으로";
                }
                else if(_playing)
                {
                    var line=_lines[_sequence[_index]];_status.text=(line.speaker=="표시"?string.Empty:line.speaker+": ")+line.text;
                    _buttonLabel.text="다음";
                }
                else {_status.text=string.Empty;_buttonLabel.text=string.Empty;}
            }
            catch(Exception e){StopDiagnostic(e);}
        }
        private static void ValidateDocument(DialogueDocument document)
        {
            if(document==null || document.version!=1 || document.minimumDisplaySimulationTicks!=60 || document.lines==null || document.lines.Length!=17 ||
                !Same(document.closingExtraction,new[]{"S01","SE01","S02","S03","C01"}) ||
                !Same(document.closingSolidarity,new[]{"S01","SS01","S02","S03","C01"}) || !Same(document.ended,new[]{"C02","C03"}))
                throw new InvalidOperationException("승인 서사 순서가 다릅니다.");
            var expected=new[]{
                "Y01|유담|이 장부에는 마흔일곱 사람의 허락이 담겨 있어. 네게 소유권을 넘기지는 않겠어.",
                "Y02|도언|네 선택을 남길지, 네 각인을 끊어 힘을 가져갈지 결정해야 하는군.",
                "YE01|표시|수탈 — 유담의 거부를 무시하고 각인을 절단합니다.",
                "YS01|유담|봉쇄선을 지나는 데 필요한 공명만 빌려주겠어. 장부와 선택은 내게 남겨 둬.",
                "YS02|표시|연대 — 유담이 허락한 범위의 공명만 빌립니다.",
                "YE02|유담|내 허락 없이 끊었구나. 이제 나는 공동장부를 다룰 수 없어.",
                "YE03|도언|내가 네 거부를 꺾었다. 그 책임을 남에게 돌리지는 않겠어.",
                "YS03|도언|네 각인은 네 것이야. 허락한 범위 안에서만 쓰겠다.",
                "YS04|유담|그 약속이 지켜지는지 보겠어.",
                "S01|세령|도언. 나는 아인을 찾고 있어. 언니에게 네가 무엇을 맡겼는지도 알아야겠어.",
                "SE01|세령|유담의 거부를 무시했군. 네 사정만 듣고 넘어가지는 않겠어.",
                "SS01|세령|유담에게 선택을 남겼군. 하지만 그것만으로 네 책임이 사라지지는 않아.",
                "S02|도언|내가 설명하지 못한 위험이 있었어. 아인을 찾고, 내가 한 일에 답하겠다.",
                "S03|세령|네 말을 믿을지는 내가 판단해. 지금은 네가 남긴 흔적부터 확인하겠어.",
                "C01|표시|봉쇄선 너머로 길이 이어집니다. 도언은 아인을 찾고 자신의 책임을 확인하기 위해 걸음을 옮깁니다.",
                "C02|표시|1챕터 완료","C03|표시|2챕터 — 아직 열리지 않은 길"};
            for(var i=0;i<expected.Length;i++)
            {var line=document.lines[i];if(line==null || line.id+"|"+line.speaker+"|"+line.text!=expected[i])throw new InvalidOperationException("승인 대사 원문이 다릅니다.");}
        }
        private static bool Same(string[] actual,string[] expected)
        {if(actual==null || actual.Length!=expected.Length)return false;for(var i=0;i<actual.Length;i++)if(actual[i]!=expected[i])return false;return true;}
        private void OnDisable(){_retired=true;_cursor=null;_playing=false;}
    }
}
