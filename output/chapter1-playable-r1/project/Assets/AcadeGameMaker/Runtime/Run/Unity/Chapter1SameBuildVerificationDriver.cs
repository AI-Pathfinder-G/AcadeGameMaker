using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using UnityEngine;

namespace AcadeGameMaker.Run.Unity
{
    // REQ-AUTOCH1-SAMEBUILD-001~007; AC-AUTOCH1-SAMEBUILD-001/002/004.
    // The only writes outside this driver are owned device events and dedicated verification evidence.
    [DefaultExecutionOrder(205)]
    internal sealed class Chapter1SameBuildVerificationDriver : MonoBehaviour,IChapter1StandaloneVerificationOwner
    {
        private Chapter1VerificationInputPilot _pilot;
        private Chapter1StandaloneVerificationSession _standaloneSession;
        private bool _optionValidated,_standaloneReady,_standaloneAttached,_standaloneNeutralWaitLogged;
        bool IChapter1StandaloneVerificationOwner.TryObserveStandaloneOwner(Chapter1VerificationInputPilot requester,out Chapter1StandaloneVerificationSession session)
        {
            session=null;
            if(!Chapter1StandaloneVerificationSession.MatchesRequest(_pilot,requester,_standaloneReady,_finish,_closed) || _standaloneSession==null || !_standaloneSession.Active)return false;
            session=_standaloneSession;return true;
        }
        private void RevokeStandaloneSession(){_standaloneReady=false;if(_standaloneSession!=null)_standaloneSession.Revoke();_pilot?.RevokeStandaloneOwner();}
        private Chapter1PlayableFlowOwner _flow;
        private StreamWriter _writer;
        private InputRouter _firstRunRouter;
        private Func<bool> _firstRunRouterClosed;
        private string _mode,_failure,_phaseIdentity,_graphIdentity,_oldRunKey,_line,_transfer;
        private long _oldGeneration,_profileBeforeFailure,_lastProfile=-1,_lastRunStarted,_lastRunEnd,_retryNeutralUpdate,_finishNeutralUpdate;
        private int _bytes,_rows,_ordinal,_closingLines,_roomCount,_stage,_actionTick=-1,_lineTick=-1,_lineIssuedTick=-1,_lastTick=-1;
        private double _start,_lastProgress,_lastSample,_holdUntil,_finishAt;
        private float _bestX=float.NegativeInfinity;
        private bool _finish,_success,_closed,_sawAccepted,_sawReturn,_returnedToHub,_focusLogged,_wasFocused,_saveAtFailure;
        private readonly List<string> _rooms=new List<string>();
        private readonly string[] _expectedRooms=new string[4];
        private Chapter1CompletionReceipt _completion;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Begin()
        {
            string selected=null;var count=0;var invalid=false;
            foreach(var argument in Environment.GetCommandLineArgs())
            {
                if(!argument.StartsWith("--ch1-verify",StringComparison.Ordinal))continue;
                count++;
                if(argument=="--ch1-verify=branch1")selected="branch1";
                else if(argument=="--ch1-verify=branch2")selected="branch2";
                else if(argument=="--ch1-verify=failure-retry")selected="failure-retry";
                else invalid=true;
            }
            if(count==0)return; // Default launch creates no object, device, subscription or evidence.
            if(count!=1 || invalid || selected==null)
            {Debug.LogError("[CH1SameBuild] 미지원 또는 중복 검증 옵션입니다.");Application.Quit(2);return;}
            var owned=new GameObject("Chapter1SameBuildVerification");
            UnityEngine.Object.DontDestroyOnLoad(owned);
            var driver=owned.AddComponent<Chapter1SameBuildVerificationDriver>();
            driver._optionValidated=true;
            try{driver.Initialize(selected);}catch(Exception error){driver.EmergencyClose(error);}
        }
        private void Initialize(string mode)
        {
            _mode=mode;_start=Now;_lastProgress=_start;
            var parent=Path.GetFullPath(Path.GetDirectoryName(Application.dataPath));
            var directory=Path.Combine(parent,"VerificationEvidence");
            Directory.CreateDirectory(directory);
            var path=Path.Combine(directory,mode+".jsonl");
            _writer=new StreamWriter(new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.Read),new UTF8Encoding(false));
            Record("begin","mode="+mode+" evidence=synthetic-owned-input productionWrites=0 developmentBuild="+Debug.isDebugBuild+" focused="+Application.isFocused+" exclusiveInputAttribution=false productionDeviceId=unavailable");
            if(Debug.isDebugBuild)throw new InvalidOperationException("비개발 릴리스만 검증할 수 있습니다.");
            var saveRoot=Path.Combine(parent,"SaveData","Chapter1");
            var saveEmpty=ObserveEmptySaveRoot(saveRoot);
            if(!saveEmpty)throw new InvalidOperationException("신규 사례 위치에 기존 저장이 있습니다.");
            _pilot=new Chapter1VerificationInputPilot(text=>Record("input",text));
            if(mode=="branch1" || mode=="branch2")
            {
                if(!Chapter1StandaloneVerificationSession.Admitted(mode,Application.isEditor,Debug.isDebugBuild,_optionValidated,_writer!=null,saveEmpty))throw new InvalidOperationException("단독 검증 소유 인계 선행이 없습니다.");
                _standaloneSession=new Chapter1StandaloneVerificationSession(this,mode);
                // Attach the owned input only after focus and startup OS events settle.
            }
            gameObject.AddComponent<Chapter1VerificationPresentationObserver>().Bind(this);
            _pilot.Neutral();Record("wait","자연 장면 부팅과 입력 중립을 기다립니다. 기한은 늘리지 않습니다.");
        }
        // REQ-SOP-001 / AC-SOP-001: access and I/O failures propagate to EmergencyClose before admission.
        private static bool ObserveEmptySaveRoot(string saveRoot)
        {
            var requested=Path.GetFullPath(saveRoot);
            for(var current=requested;!string.IsNullOrEmpty(current);current=Path.GetDirectoryName(current))
            {
                FileAttributes attributes;
                try{attributes=File.GetAttributes(current);}
                catch(FileNotFoundException){continue;}
                catch(DirectoryNotFoundException){continue;}
                if((attributes&FileAttributes.Directory)==0 || (attributes&FileAttributes.ReparsePoint)!=0)
                    throw new IOException("검증 저장 경로가 일반 디렉토리가 아닙니다.");
                if(string.Equals(current,requested,StringComparison.OrdinalIgnoreCase))
                    return Directory.GetFileSystemEntries(current).Length==0;
                // Absence is accepted only below an actually readable existing directory.
                using(var entries=Directory.EnumerateFileSystemEntries(current).GetEnumerator()){entries.MoveNext();}
                return true;
            }
            throw new IOException("검증 저장 경로의 존재 여부를 확인하지 못했습니다.");
        }
        private static double Now=>Time.realtimeSinceStartupAsDouble;
        private bool PresentationPhase => _flow!=null && (_flow.View.Phase==Chapter1RunPhase.Closing || _flow.View.Phase==Chapter1RunPhase.ChapterCompleted);
        private void FixedUpdate(){if(_finish || !PresentationPhase)StepGuarded();}
        private void LateUpdate(){if(_finish)StepGuarded();}
        internal void ObserveAfterPresentation(){if(!_closed && !_finish && PresentationPhase)StepGuarded();}
        private void StepGuarded()
        {
            if(_closed)return;
            try
            {
                if(_finish)
                {
                    _pilot?.Neutral();
                    if((_success && _mode=="failure-retry" && _pilot!=null && _pilot.NaturalUpdates>_finishNeutralUpdate && _pilot.NeutralConsumed) || Now>=_finishAt || Now-_start>=1800 || Now-_lastProgress>=60)CloseAndQuit();
                    return;
                }
                if(_pilot!=null && _pilot.Error!=null)throw new InvalidOperationException(_pilot.Error);
                if(Now-_start>=1800)throw new TimeoutException("사례 30분 상한입니다.");
                if(Now-_lastProgress>=60)throw new TimeoutException("권위 있는 진전이 60초 동안 없습니다.");
                if(!_focusLogged || _wasFocused!=Application.isFocused)
                {_focusLogged=true;_wasFocused=Application.isFocused;Record("focus","actual="+Application.isFocused+" settingsChanged=false");}
                if(!Application.isFocused){_pilot.Neutral();return;}
                if(_mode=="branch1" || _mode=="branch2")
                {
                    if(!_standaloneAttached)
                    {
                        _pilot.Neutral();
                        if(!Chapter1VerificationInputPilot.StartupQuiet(
                            Now-_start,Now-_pilot.LastForeignInputAt,Application.isFocused))return;
                        _standaloneReady=true;
                        if(!_pilot.AttachStandaloneOwner(this))
                            throw new InvalidOperationException("안정된 시작 이후 자기 Pilot의 소유 인계가 거절됐습니다.");
                        _standaloneAttached=true;
                        Progress("standalone-owner armed after startup, nonownedBefore="+_pilot.ForeignInputCount+" stillRequiresOwnedDynamicNeutral=true");
                        return;
                    }
                    if(_standaloneSession==null || !_standaloneSession.Active)
                        throw new InvalidOperationException("단독 입력 소유권이 철회됐으므로 자동 플레이를 중단합니다.");
                    if(!_pilot.StandaloneInputReady)
                    {
                        _pilot.Neutral();
                        if(!_standaloneNeutralWaitLogged)
                        {
                            _standaloneNeutralWaitLogged=true;
                            Progress("waiting for two consumed owned neutral updates before first expedition intent");
                        }
                        return;
                    }
                }
                if(_flow==null)
                {
                    var owners=UnityEngine.Object.FindObjectsByType<Chapter1PlayableFlowOwner>();
                    if(owners.Length>1)throw new InvalidOperationException("생산 소유자가 여러 개입니다.");
                    if(owners.Length==0)return;
                    _flow=owners[0];Record("owner","scene="+_flow.gameObject.scene.path+" object="+_flow.GetEntityId());
                }
                Step();
            }
            catch(Exception error){BeginFinish(false,error.ToString());}
        }
        private void Progress(string reason){_lastProgress=Now;Record("progress",reason);}
        private void Step()
        {
            var view=_flow.View;var owner=_flow.Lifetime;var router=owner==null?null:owner.CurrentRouter;
            var phaseIdentity=view.Phase+"/"+view.RoomId+"/"+view.Generation;
            if(_phaseIdentity!=phaseIdentity)
            {_phaseIdentity=phaseIdentity;Progress("phase="+phaseIdentity+" run="+view.RunKey+" seed="+view.Seed+" error="+view.Error);}
            if(view.Phase==Chapter1RunPhase.Diagnostic || view.Phase==Chapter1RunPhase.ExpeditionStorageFailed)
                throw new InvalidOperationException("생산 진단/저장 실패: "+view.Error);
            if(owner!=null)_pilot.ObserveCombat(router,Progress);
            if(_flow.LastSaveResult.HasValue)
            {
                var result=_flow.LastSaveResult.Value;
                if(result.CommittedRevision>_lastProfile)
                {_lastProfile=result.CommittedRevision;Progress("save outcome="+result.Outcome+" revision="+result.CommittedRevision);}
            }
            if(_flow.RunStartedRevision!=_lastRunStarted)
            {
                _lastRunStarted=_flow.RunStartedRevision;
                if(_lastRunStarted>0)
                {
                    var attempt=view.CurrentStorageAttempt;
                    if(attempt==null || !attempt.Plan.HasValue)throw new InvalidOperationException("실제 제공 계획이 없습니다.");
                    var plan=attempt.Plan.Value;
                    for(var i=0;i<4;i++)_expectedRooms[i]=plan.RoomAt(i);
                    Record("plan","seed="+plan.Seed+" sha="+plan.CanonicalSha+" rooms="+string.Join(",",_expectedRooms)+" run="+view.RunKey+" generation="+view.Generation);
                    if(_oldRunKey==null)
                    {
                        _oldRunKey=view.RunKey;_oldGeneration=view.Generation;_profileBeforeFailure=ReadProfile().ProfileRevision;
                        if(_mode=="failure-retry")
                        {if(router==null)throw new InvalidOperationException("첫 원정의 실제 입력 소유자가 없습니다.");_firstRunRouter=router;_firstRunRouterClosed=router.GetChapter1ClosureReader();}
                    }
                }
            }
            if(_flow.RunEndCommittedRevision!=_lastRunEnd)
            {_lastRunEnd=_flow.RunEndCommittedRevision;Record("run-end","revision="+_lastRunEnd+" graphRetired="+(owner!=null&&owner.Chapter1GraphRetired));}
            Chapter1EncounterEvidenceView completed=default;
            var hasCompleted=owner!=null && owner.TryGetCompletedEvidence(out completed);
            if(hasCompleted)
            {
                if(completed.RunKey!=view.RunKey || completed.Generation!=owner.Generation)throw new InvalidOperationException("완료 관측의 원정/세대 불일치입니다.");
                var graph=view.RunKey+"/"+completed.Generation;
                if(graph!=_graphIdentity){_graphIdentity=graph;_bestX=float.NegativeInfinity;_lastTick=-1;_actionTick=-1;}
                // Purpose-directed progress uses the best ever rightward position per production graph.
                if((view.Phase==Chapter1RunPhase.InRoom || view.Phase==Chapter1RunPhase.AtBarrier || (view.Phase==Chapter1RunPhase.AtBoss && completed.Position.x<=6.25f)) && completed.Position.x>_bestX+.25f)
                {_bestX=completed.Position.x;Progress("position room="+view.RoomId+" x="+_bestX+" tick="+completed.Tick.Value);}
                if(Now-_lastSample>=5)
                {_lastSample=Now;Record("completed","room="+view.RoomId+" run="+completed.RunKey+" generation="+completed.Generation+" tick="+completed.Tick.Value+" hp="+completed.Health+" enemies="+completed.AliveEnemyCount+" x="+completed.Position.x+" y="+completed.Position.y+" focused="+Application.isFocused+" exclusiveInputAttribution=false productionDeviceId=unavailable");}
                if(completed.TransferTarget!=_transfer)
                {_transfer=completed.TransferTarget;if(!string.IsNullOrEmpty(_transfer))Progress("transfer="+_transfer+" tick="+completed.Tick.Value);}
            }
            if(_mode=="failure-retry"){FailureStep(view,completed,hasCompleted);return;}
            if(_flow.AcceptedFailureKind.HasValue)throw new InvalidOperationException("정상 양분기 운전 중 실제 실패: "+_flow.AcceptedFailureKind.Value);
            switch(view.Phase)
            {
                case Chapter1RunPhase.Hub:
                    if(_stage==0 && _flow.CurrentHub!=null && _flow.CurrentHub.Ready && _pilot.NeutralConsumed)
                    {CheckFreshProfile();_pilot.TapSubmit();_stage=1;Record("action","기본 출정 새 입력; 검증 메뉴/명시 seed 없음");}
                    break;
                case Chapter1RunPhase.InRoom:
                    if(!hasCompleted)return;
                    if(_rooms.Count==0 || _rooms[_rooms.Count-1]!=view.RoomId)
                    {
                        if(view.RoomIndex!=_rooms.Count || _rooms.Count>=4 || view.RoomId!=_expectedRooms[_rooms.Count])throw new InvalidOperationException("실제 계획과 방 순서가 다릅니다.");
                        _rooms.Add(view.RoomId);_roomCount=_rooms.Count;Record("room","index="+view.RoomIndex+" id="+view.RoomId+" hp="+completed.Health);
                    }
                    if(completed.Health<=0)throw new InvalidOperationException("정상 운전 HP가 양수가 아닙니다.");
                    if(_lastTick!=completed.Tick.Value){_lastTick=completed.Tick.Value;var room=router.transform.parent.GetComponent<Chapter1RoomInstance>();if(room==null || room.RoomId!=view.RoomId)throw new InvalidOperationException("실제 방 메타 불일치입니다.");_pilot.PlayRoom(router,room.RoomId,room.Floors,room.Terrain,room.Exit,completed.TransferTarget);}
                    break;
                case Chapter1RunPhase.AtBoss:
                    if(_roomCount!=4)throw new InvalidOperationException("네 방을 거치지 않았습니다.");
                    if(hasCompleted && _lastTick!=completed.Tick.Value){_lastTick=completed.Tick.Value;_pilot.PlayBoss(router,completed.Position,_ordinal++);}
                    break;
                case Chapter1RunPhase.ChoiceRequired:ChoiceStep();break;
                case Chapter1RunPhase.AtBarrier:if(hasCompleted)BarrierStep(router,completed);break;
                case Chapter1RunPhase.Closing:ClosingStep();break;
                case Chapter1RunPhase.ChapterCompleted:ValidateCompletion();BeginFinish(true,"양분기 해당 사례의 생산 완료 영수증·저장 확인");break;
                default:_pilot.Neutral();break;
            }
        }
        private void ChoiceStep()
        {
            var source=_flow.Lifetime.CurrentChoiceAdapter;
            if(source==null || !source.Router.CurrentReceipt.HasValue){_pilot.Neutral();return;}
            var tick=source.Router.CurrentReceipt.Value.Tick.Value;
            if(_actionTick<0){_actionTick=tick;_pilot.Neutral();return;}
            if(tick-_actionTick<3 || !_pilot.NeutralConsumed)return;
            var branch=_mode=="branch1"?1:2;
            if(source.UiPhase==Chapter1ChoiceUiPhase.Required)
            {_pilot.TapClick(ChoicePoint(branch-1));_actionTick=tick;Record("action","choice-propose branch="+branch+" tick="+tick);}
            else if(source.UiPhase==(branch==1?Chapter1ChoiceUiPhase.ExtractionProposed:Chapter1ChoiceUiPhase.SolidarityProposed))
            {_pilot.TapClick(ChoicePoint(2));_actionTick=tick;Record("action","choice-confirm tick="+tick);}
            else if(source.UiPhase==Chapter1ChoiceUiPhase.SaveFailed || source.UiPhase==Chapter1ChoiceUiPhase.SaveUncertain)
                throw new InvalidOperationException("선택 저장이 확정되지 않았습니다.");
        }
        private static Vector2 ChoicePoint(int index)
        {
            var scale=Mathf.FloorToInt(Mathf.Min(Screen.width/640f,Screen.height/360f));
            if(scale<1)throw new InvalidOperationException("UI 배율을 관측하지 못했습니다.");
            return new Vector2(Screen.width/2f,Screen.height/2f+(-35-index*29)*scale);
        }
        private void BarrierStep(InputRouter router,Chapter1EncounterEvidenceView view)
        {
            if(_stage<10)
            {
                var saved=ReadProfile();var choice=_mode=="branch1"?ProfileChoice.Extraction:ProfileChoice.Solidarity;
                if(saved.Progression.CommittedChoice!=choice)throw new InvalidOperationException("실제 선택 저장 불일치입니다.");
                _stage=10;_actionTick=view.Tick.Value;_pilot.Neutral();Progress("choice-saved revision="+saved.ProfileRevision+" choice="+choice);
            }
            if(_stage==10 && view.Tick.Value-_actionTick>=3 && _pilot.NeutralConsumed)
            {_pilot.BarrierTransfer(router);_stage=11;_actionTick=view.Tick.Value;Record("action","counterweight transfer request tick="+_actionTick);}
            else if(_stage==11 && view.TransferTarget=="BarrierCounterweight")
            {_stage=12;_actionTick=view.Tick.Value;_pilot.Neutral();Progress("actual-counterweight-transfer tick="+_actionTick);}
            else if(_stage==12 && view.Tick.Value-_actionTick>=24 && _pilot.NeutralConsumed)
            {_pilot.CaptureSkillBaseline(router,view,_mode=="branch1"?1:2);_pilot.TapSkill();_stage=13;_actionTick=view.Tick.Value;Record("action","choice-skill input tick="+_actionTick);}
            else if(_stage==13 && _pilot.SkillEffectObserved(router,view,_mode=="branch1"?1:2))
            {_stage=14;_actionTick=view.Tick.Value;_pilot.Neutral();Progress("actual-choice-skill tick="+_actionTick);}
            else if(_stage==14 && _pilot.GateReady(router,_mode=="branch1"?1:2))
            {_stage=15;_actionTick=view.Tick.Value;Progress("barrier-traversal-prepared tick="+_actionTick);}
            else if(_stage==15)
            {
                if(view.Tick.Value-_actionTick>=1200)throw new TimeoutException("봉쇄선 입력 1200 완료 틱 상한입니다.");
                _pilot.TraverseBarrier(router,view);
            }
            if(_stage>=10 && _stage<15 && view.Tick.Value-_actionTick>120)throw new TimeoutException("전이/기술/봉쇄선 준비 관측 상한입니다.");
        }
        private void ClosingStep()
        {
            var source=_flow.Lifetime.CurrentChoiceAdapter;
            if(source==null || !source.Router.CurrentReceipt.HasValue)return;
            var text=Chapter1VerificationInputPilot.ReadStatus(source);var tick=source.Router.CurrentReceipt.Value.Tick.Value;
            if(text!=_line)
            {
                if(_line!=null && _lineIssuedTick<0)throw new InvalidOperationException("입력 없는 종결줄 변경입니다.");
                _line=text;_lineTick=tick;_lineIssuedTick=-1;_closingLines++;
                var lines=ExpectedClosingLines(_mode=="branch1");if(_closingLines>lines.Length || !text.Contains(lines[_closingLines-1]))throw new InvalidOperationException("실제 종결줄의 순서/문구 불일치입니다.");
                _pilot.Neutral();Progress("closing-line ordinal="+_closingLines+" tick="+tick+" text="+text);
            }
            if(tick-_lineTick>120)throw new TimeoutException("종결줄의 120 UI틱 상한입니다.");
            if(_lineIssuedTick<0 && tick-_lineTick>=60 && _pilot.NeutralConsumed && source.ClosingControlsNeutralReady)
            {_pilot.TapSubmit();_lineIssuedTick=tick;Record("action","closing-next tick="+tick+" line="+_closingLines);}
        }
        private static string[] ExpectedClosingLines(bool extraction)=>new[]{"도언. 나는 아인을 찾고 있어. 언니에게 네가 무엇을 맡겼는지도 알아야겠어.",extraction?"유담의 거부를 무시했군. 네 사정만 듣고 넘어가지는 않겠어.":"유담에게 선택을 남겼군. 하지만 그것만으로 네 책임이 사라지지는 않아.","내가 설명하지 못한 위험이 있었어. 아인을 찾고, 내가 한 일에 답하겠다.","네 말을 믿을지는 내가 판단해. 지금은 네가 남긴 흔적부터 확인하겠어.","봉쇄선 너머로 길이 이어집니다. 도언은 아인을 찾고 자신의 책임을 확인하기 위해 걸음을 옮깁니다."};
        private void ValidateCompletion()
        {
            var receipt=_flow.CurrentChapter1CompletionReceipt;
            if(receipt==null || _closingLines!=5 || _roomCount!=4 || _stage!=15 || receipt.Demo.Branch!=(ProfileBranch)(_mode=="branch1"?1:2) ||
                receipt.Save.CommittedRevision!=receipt.Save.ExpectedRevision || receipt.Closing.UiGeneration!=receipt.Demo.Generation+1 ||
                receipt.Closing.FinalAdvanceOrdinal<5 || _flow.RunEndCommittedRevision!=1)
                throw new InvalidOperationException("완료 영수증·순서·저장이 완전하지 않습니다.");
            var saved=ReadProfile();var found=false;foreach(var branch in saved.Progression.CompletedBranches)if(branch==receipt.Demo.Branch)found=true;
            if(!found || saved.ProfileRevision!=receipt.Save.CommittedRevision)throw new InvalidOperationException("완료 분기가 저장에 없습니다.");
            var status=Chapter1VerificationInputPilot.ReadStatus(_flow.Lifetime.CurrentChoiceAdapter);
            if(!status.Contains("1챕터 완료") || !status.Contains("아직 열리지 않은 길"))throw new InvalidOperationException("완료 표시가 없습니다.");
            _completion=receipt;Progress("completion branch="+receipt.Demo.Branch+" revision="+saved.ProfileRevision+" advance="+receipt.Closing.FinalAdvanceOrdinal+" status="+status);
        }
        private void FailureStep(Chapter1RunView view,Chapter1EncounterEvidenceView completed,bool hasCompleted)
        {
            if(_flow.AcceptedFailureKind.HasValue && !_sawAccepted)
            {
                var failure=_flow.FailureSnapshot.AcceptedFailure;
                if((int)_flow.AcceptedFailureKind.Value!=0 || !failure.HasValue || !_pilot.InitialPositiveHealth || !_pilot.EnemyDamageObserved || !_pilot.NaturalZeroObserved || failure.Value.Tick.Value!=_pilot.LastDamageTick)
                    throw new InvalidOperationException("양수 HP→실제 적 피해→HP0→기대한 실패 원인의 인과가 불완전합니다.");
                _sawAccepted=true;Record("failure-accepted","kind="+_flow.AcceptedFailureKind.Value+" tick="+failure.Value.Tick.Value+" source="+_pilot.LastDamageSource+" priorHp="+_pilot.LastDamagePriorHealth+" hp="+_pilot.LastDamageCurrentHealth+" applied="+_pilot.LastDamageAmount+" run="+view.RunKey+" generation="+view.Generation);
            }
            if(view.Phase==Chapter1RunPhase.ReturnedToHub)
            {
                if(!_sawAccepted || !_flow.LastSaveResult.HasValue || _flow.RunEndCommittedRevision!=1)throw new InvalidOperationException("실패 수용/저장/종료 revision이 없습니다.");
                var saved=_flow.LastSaveResult.Value;
                if(saved.CommittedRevision!=_profileBeforeFailure+1 || (saved.Outcome!=ProfileAtomicSaveOutcome.CommittedReplacement && saved.Outcome!=ProfileAtomicSaveOutcome.CommittedFirst))
                    throw new InvalidOperationException("실패 저장 revision/결과 불일치입니다.");
                if(!_sawReturn)
                {_sawReturn=true;_saveAtFailure=true;_holdUntil=Now+15;_pilot.Neutral();Record("wait","failure UI neutral observation <=15 seconds; deadlines unchanged; revision="+saved.CommittedRevision);}
                if(Now>=_holdUntil && _flow.CurrentHub!=null && _flow.CurrentHub.Ready && _pilot.NeutralConsumed && _stage<20)
                {_pilot.TapSubmit();_stage=20;Record("action","실패 UI에서 거점 복귀 새 입력");}
                return;
            }
            if(view.Phase==Chapter1RunPhase.Hub)
            {
                if(_stage==0 && _pilot.NeutralConsumed && _flow.CurrentHub!=null && _flow.CurrentHub.Ready)
                {CheckFreshProfile();_pilot.TapSubmit();_stage=1;}
                else if(_stage==20 && _sawReturn && _pilot.NeutralConsumed)
                {_returnedToHub=true;_stage=21;_retryNeutralUpdate=_pilot.NaturalUpdates;_pilot.Neutral();Record("wait","거점 복귀 뒤 두 자연 입력 갱신의 중립 관측; baseline="+_retryNeutralUpdate);}
                else if(_stage==21 && _pilot.NaturalUpdates>=_retryNeutralUpdate+2 && _pilot.NeutralConsumed && _flow.CurrentHub!=null && _flow.CurrentHub.Ready)
                {
                    var scale=Mathf.FloorToInt(Mathf.Min(Screen.width/640f,Screen.height/360f));
                    if(scale<1)throw new InvalidOperationException("거점 기본 버튼의 실제 화면 배율이 유효하지 않습니다.");
                    _pilot.TapClick(new Vector2(Screen.width/2f,Screen.height/2f+45*scale));_stage=22;
                    Record("action","실패 저장 후 새 원정 마우스 입력; neutralUpdates="+_pilot.NaturalUpdates);
                }
                return;
            }
            if(view.Phase==Chapter1RunPhase.InRoom && hasCompleted)
            {
                if(_stage>=22)
                {
                    var currentRouter=_flow.Lifetime.CurrentRouter;var profile=ReadProfile();
                    if(!_returnedToHub || !_saveAtFailure || view.RunKey==_oldRunKey || view.Generation<1 || view.Generation!=completed.Generation || currentRouter==null || ReferenceEquals(currentRouter,_firstRunRouter) || _firstRunRouterClosed==null || !_firstRunRouterClosed() || profile.ProfileRevision!=_profileBeforeFailure+2 || completed.Tick.Value<1 || completed.Health<=0 || _flow.RunStartedRevision!=2 || view.RoomId!=_expectedRooms[0])
                        throw new InvalidOperationException("실패 후 새 원정/세대/첫 완료가 확인되지 않았습니다.");
                    Progress("retry-completed oldRun="+_oldRunKey+" newRun="+view.RunKey+" oldGeneration="+_oldGeneration+" newGeneration="+view.Generation+" oldRouterClosed=true routerChanged=true generationScope=per-run profileRevision="+profile.ProfileRevision+" tick="+completed.Tick.Value+" room="+view.RoomId+" hp="+completed.Health);
                    BeginFinish(true,"자연 적 피해 실패→정상 저장→거점→새 원정 완료");
                }
                else _pilot.ApproachForFailure(completed.Position);
            }
            else _pilot.Neutral();
        }
        private static ProfileSnapshotV1 ReadProfile()
        {
            var root=DesktopProfileLaunchAdapterV1.Chapter1ProfileRoot;
            var observed=ProfileLaunchObservationAdapterV1.Observe(root);observed.Validate();
            var selected=ProfileLoadSelectorV1.Select(observed.Primary,observed.Previous,observed.Temp);selected.Validate();
            if(selected.RequiresAtomicSave)throw new InvalidOperationException("읽기 관측 중 복구 저장이 필요합니다.");
            return selected.InMemoryDocument.Snapshot;
        }
        private static void CheckFreshProfile()
        {
            var profile=ReadProfile();
            if(profile.Progression.CommittedChoice.HasValue || profile.Progression.CompletedBranches.Count!=0)
                throw new InvalidOperationException("신규 사례의 자연 프로필이 아닙니다.");
        }
        private void BeginFinish(bool success,string reason)
        {
            RevokeStandaloneSession();
            if(_finish || _closed)return;
            _finish=true;_success=success;_failure=success?null:reason;_finishNeutralUpdate=_pilot==null?0:_pilot.NaturalUpdates;_pilot?.Neutral();
            _finishAt=Math.Min(Now+15,Math.Min(_start+1800,_lastProgress+60));
            try{Record(success?"candidate-success":"failure",reason);Record("wait","terminal neutral observation <=15 seconds; deadlines unchanged");}
            catch(Exception error){_success=false;_failure=(reason??"")+"; 원시 기록 실패: "+error;_finishAt=Now;Debug.LogError(_failure);}
        }
        private void CloseAndQuit()
        {
            RevokeStandaloneSession();
            if(_closed)return;
            var neutral=false;
            try
            {
            if(_pilot!=null && _pilot.Error!=null){_success=false;_failure=(_failure??"")+"; 종료 입력 오류: "+_pilot.Error;}
            if(_success && _mode=="failure-retry" && (_pilot==null || _pilot.NaturalUpdates<=_finishNeutralUpdate || _flow.View.Phase!=Chapter1RunPhase.InRoom || !_flow.Lifetime.TryGetCompletedEvidence(out var fresh) || fresh.RunKey!=_flow.View.RunKey || fresh.Generation!=_flow.View.Generation || fresh.Health<=0 || _flow.AcceptedFailureKind.HasValue))
            {_success=false;_failure="재출정 종료 시 현재 첫 방의 생존 완료 상태를 확인하지 못했습니다.";}
            neutral=_pilot!=null && _pilot.NeutralConsumed;
            if(!neutral){_success=false;_failure=(_failure??"")+"; 자연 중립 소비 미확인";}
            if(_success && _completion!=null && !ReferenceEquals(_completion,_flow.CurrentChapter1CompletionReceipt))
            {_success=false;_failure="완료 영수증 신원이 변경됐습니다.";}
            }
            catch(Exception error){_success=false;_failure=(_failure??"")+"; 종료 상태 관측 오류: "+error;}
            try{_pilot?.Dispose();Record("cleanup","ownedDevicesAndSubscriptionsRemoved=true neutralConsumed="+neutral);}
            catch(Exception error){_success=false;_failure=(_failure??"")+"; 정리 실패: "+error;}
            try{Record("result","success="+_success+" failure="+_failure+" rows="+(_rows+1)+" bytesBeforeFinal="+_bytes+" elapsed="+(Now-_start)+" "+(_pilot==null?"exclusiveInputAttribution=false pilotUnavailable":_pilot.AttributionLimit));_writer?.Flush();_writer?.Dispose();_writer=null;}
            catch(Exception error){_success=false;Debug.LogError("[CH1SameBuild] 원시 폐쇄 실패: "+error);}
            _closed=true;Application.Quit(_success?0:1);
        }
        [Serializable] private sealed class EvidenceRow {public string utc,kind,detail;public double seconds;public int frame;}
        private void Record(string kind,string detail)
        {
            if(_writer==null)throw new InvalidOperationException("원시 출력이 열려 있지 않습니다.");
            var line=JsonUtility.ToJson(new EvidenceRow{utc=DateTime.UtcNow.ToString("O"),kind=kind,detail=detail,seconds=Now-_start,frame=Time.frameCount});
            var bytes=Encoding.UTF8.GetByteCount(line)+1;
            var terminal=kind=="result" || kind=="cleanup" || kind=="failure" || kind=="wait";
            if(_rows+1>(terminal?1024:1016) || _bytes+bytes>(terminal?512*1024:504*1024))throw new InvalidOperationException("원시 1024행/512KiB 상한입니다.");
            _writer.Write(line);_writer.Write('\n');_writer.Flush();_bytes+=bytes;_rows++;
        }
        private void EmergencyClose(Exception error)
        {
            RevokeStandaloneSession();
            _success=false;_failure=error.ToString();
            try{_pilot?.Dispose();}catch(Exception cleanup){Debug.LogError("[CH1SameBuild] 초기화 정리 실패: "+cleanup);}
            try{if(_writer!=null){Record("failure",_failure);_writer.Dispose();_writer=null;}}catch(Exception close){Debug.LogError("[CH1SameBuild] 초기화 원시 폐쇄 실패: "+close);}
            _closed=true;Debug.LogError("[CH1SameBuild] "+_failure);Application.Quit(1);
        }
        private void OnDestroy()
        {
            RevokeStandaloneSession();
            if(_closed)return;
            _success=false;Debug.LogError("[CH1SameBuild] 검증 종료 전 운전 객체가 제거됐습니다.");
            try{_pilot?.Dispose();}catch(Exception error){Debug.LogError(error);}
            try{_writer?.Dispose();}catch(Exception error){Debug.LogError(error);}
            _closed=true;
        }
    }
    // Owned only by the explicit verification launch; the authored presenter updates at 220.
    [DefaultExecutionOrder(225)]
    internal sealed class Chapter1VerificationPresentationObserver : MonoBehaviour
    {
        private Chapter1SameBuildVerificationDriver _owner;
        internal void Bind(Chapter1SameBuildVerificationDriver owner)
        {
            if(_owner!=null || owner==null || owner.gameObject!=gameObject)throw new InvalidOperationException("검증 표시 관측기의 소유자가 유효하지 않습니다.");
            _owner=owner;
        }
        private void LateUpdate(){if(_owner!=null)_owner.ObserveAfterPresentation();}
    }
}





