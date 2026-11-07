using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using AcadeGameMaker.Profile;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Run.Unity;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Hub.Presentation.Unity;
using TMPro;
using NUnit.Framework;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.RunUnity
{
    // AC-C1B-HUB-001/E2E-001..003: 합성 장치는 실제 callback을 사용하지만 실제 창 플레이를 대체하지 않는다.
    internal sealed class Chapter1ExpeditionPlayModeTests : InputTestFixture
    {
        // AC-VERIFEXIT-002: isolated selector/hold states, not a claim of natural input or physics completion.
        [Test] public void VerificationExitApproachBrakesAndRecovers()
        {
            var exit=Rect.MinMaxRect(30,0,32,3);
            foreach(var id in new[]{"R01","R02","R03","R04","R05","R06"})
            {
                var supported=false;
                foreach(var floor in Chapter1RoomInstance.ExpectedFloors(id))if(floor.xMin<=exit.xMin-4 && floor.xMax>=exit.xMax && floor.yMax==0)supported=true;
                Assert.That(supported,Is.True,id+" exit approach floor");
                foreach(var terrain in Chapter1RoomInstance.ExpectedTerrain(id))Assert.That(terrain.xMax,Is.LessThan(exit.xMin-4),id+" exit approach obstacle");
            }
            Chapter1VerificationRoomPolicy.Command command;
            Assert.That(Chapter1VerificationRoomPolicy.TryReadExitApproach(exit,34.804f,8,false,false,out command),Is.True);
            CollectionAssert.AreEqual(new[]{Key.A},command.Keys);Assert.That(command.Attack,Is.False);Assert.That(command.Target,Is.Empty);Assert.That(command.AimValid,Is.False);
            Assert.That(Chapter1VerificationRoomPolicy.TryReadExitApproach(exit,30.2f,8,false,false,out command),Is.True);
            CollectionAssert.AreEqual(new[]{Key.A},command.Keys); // retained velocity must brake before the center.
            Assert.That(Chapter1VerificationRoomPolicy.TryReadExitApproach(exit,26,0,false,false,out command),Is.True);
            CollectionAssert.AreEqual(new[]{Key.D},command.Keys);
            Assert.That(Chapter1VerificationRoomPolicy.TryReadExitApproach(exit,31,0,false,false,out command),Is.True);
            Assert.That(command.Keys,Is.Empty);Assert.That(command.Attack,Is.False);Assert.That(command.Target,Is.Empty);
            // Height is intentionally not an input to the selector: high/wall cases emit no jump/dash/attack/transfer.
            var jump=new Chapter1VerificationRoomPolicy.JumpHold();Assert.That(jump.Read(10,true,false),Is.True);
            for(var tick=11;tick<80;tick++)Assert.That(jump.Read(tick,true,true),Is.False);
            Assert.That(Chapter1VerificationRoomPolicy.TryReadExitApproach(exit,34.804f,8,true,false,out command),Is.False);
            Assert.That(Chapter1VerificationRoomPolicy.TryReadExitApproach(exit,34.804f,8,false,true,out command),Is.False);
            Assert.That(Chapter1VerificationRoomPolicy.TryReadExitApproach(exit,25.99f,8,false,false,out command),Is.False);
            Assert.DoesNotThrow(()=>Chapter1VerificationRoomPolicy.ValidateCompletedTick(3099,100));
            Assert.Throws<InvalidOperationException>(()=>Chapter1VerificationRoomPolicy.ValidateCompletedTick(3100,100));
            // The existing no-progress stopwatch remains in Read; this fixture does not advance or replace it.
        }
        private Chapter1AuthoredInputTrace _trace;
        private string _caseRunId;
        [SetUp] public void IsolateChapter1Profile(){Chapter1AuthoredInputTrace.ResetStages();_caseRunId="expedition-"+Guid.NewGuid().ToString("N");DesktopProfileLaunchAdapterV1.ConfigureChapter1TestRun(_caseRunId);}
        [UnityTearDown] public IEnumerator Close(){try{if(_trace!=null)yield return _trace.Close();}finally{try{if(_trace!=null)_trace.FinallyCloseEvidence();}finally{_trace=null;Chapter1AuthoredInputTrace.LogStage("hub.fixture-restore-before");base.TearDown();Chapter1AuthoredInputTrace.LogStage("hub.fixture-restore-after");}}}
        private IEnumerator Open(){_trace=new Chapter1AuthoredInputTrace();yield return _trace.Open();}
        public override void TearDown()
        {Chapter1AuthoredInputTrace.LogStage("hub.fixture-restore-deferred",_trace==null?null:_trace.Flow);}
        [UnityTest] public IEnumerator AuthoredHubPreparedWithoutCatalogAdmission()
        {
            yield return Open();yield return _trace.WaitReady();var hub=_trace.Flow.CurrentHub;
            Assert.That(hub.Ready,Is.True);Assert.That(hub.Adapter.Chapter1PreparedForUi,Is.True);
            Assert.That(hub.Router.CurrentUiFrame.HasValue,Is.True);Assert.That(hub.Router.CurrentUiFrame.Value.Receipt.Mode,Is.EqualTo(AcadeGameMaker.Core.InputMode.UIOnly));
            Assert.That(hub.Router.CurrentUiFrame.Value.Receipt,Is.EqualTo(hub.Router.CurrentReceipt.Value));
            var root=Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot);Assert.That(File.Exists(Path.Combine(root,"profile.json")),Is.True);
            Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);Assert.That(_trace.Flow.View.RunKey,Is.Null);Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Hub));
            yield return ObserveHubIdle(hub);
        }
        private IEnumerator ObserveHubIdle(Chapter1HubLifetimeOwner hub)
        {
            Assert.That(hub!=null && hub.isActiveAndEnabled && ReferenceEquals(_trace.Flow.CurrentHub,hub),Is.True);
            var before=new long[120];var after=new long[120];var frameBefore=new int[120];var frameAfter=new int[120];var completed=0;
            var frequency=System.Diagnostics.Stopwatch.Frequency;var utcStart=DateTime.UtcNow;var batch=Application.isBatchMode;var focused=Application.isFocused;var background=Application.runInBackground;var scale=Time.timeScale;var vsync=QualitySettings.vSyncCount;var target=Application.targetFrameRate;var graphics=SystemInfo.graphicsDeviceType;
            var aliveAfter=false;var totalStart=System.Diagnostics.Stopwatch.GetTimestamp();
            try
            {
                for(var i=0;i<120;i++)
                {
                    before[i]=System.Diagnostics.Stopwatch.GetTimestamp();frameBefore[i]=Time.frameCount;
                    yield return null;
                    after[i]=System.Diagnostics.Stopwatch.GetTimestamp();frameAfter[i]=Time.frameCount;completed=i+1;
                }
                aliveAfter=hub!=null && hub.isActiveAndEnabled && ReferenceEquals(_trace.Flow.CurrentHub,hub);
                Assert.That(aliveAfter,Is.True);Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Hub));Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);
            }
            finally
            {
                var end=completed>0?after[completed-1]:System.Diagnostics.Stopwatch.GetTimestamp();var text=new System.Text.StringBuilder();var intervals=new double[completed];var nonIncreasing=0;
                text.AppendLine("ordinal,qpcBefore,qpcAfter,frequency,frameBefore,frameAfter,milliseconds");
                for(var i=0;i<completed;i++){var duration=(after[i]-before[i])*1000d/frequency;intervals[i]=duration;if(frameAfter[i]<=frameBefore[i])nonIncreasing++;text.Append(i).Append(',').Append(before[i]).Append(',').Append(after[i]).Append(',').Append(frequency).Append(',').Append(frameBefore[i]).Append(',').Append(frameAfter[i]).Append(',').Append(duration.ToString("R",System.Globalization.CultureInfo.InvariantCulture)).AppendLine();}
                Array.Sort(intervals);var middle=completed==0?double.NaN:completed%2==0?(intervals[completed/2-1]+intervals[completed/2])/2:intervals[completed/2];var p95=completed==0?double.NaN:intervals[(int)Math.Ceiling(completed*.95)-1];
                text.Append("summary evidenceClass=batch-editor-idle completed=").Append(completed).Append(" aliveAfter=").Append(aliveAfter).Append(" totalMilliseconds=").Append(((end-totalStart)*1000d/frequency).ToString("R",System.Globalization.CultureInfo.InvariantCulture)).Append(" minimum=").Append(completed==0?double.NaN:intervals[0]).Append(" median=").Append(middle).Append(" p95=").Append(p95).Append(" maximum=").Append(completed==0?double.NaN:intervals[completed-1]).Append(" nonIncreasingFrameIntervals=").Append(nonIncreasing).Append(" batch=").Append(batch).Append(" focusedBefore=").Append(focused).Append(" background=").Append(background).Append(" timeScale=").Append(scale).Append(" vsync=").Append(vsync).Append(" targetFrameRate=").Append(target).Append(" graphics=").Append(graphics).Append(" utcStart=").Append(utcStart.ToString("O")).Append(" utcEnd=").Append(DateTime.UtcNow.ToString("O")).Append(" displayedFramesOrGPUTimeClaimed=false");
                var bytes=new System.Text.UTF8Encoding(false).GetBytes(text.ToString());Assert.That(bytes.Length,Is.LessThanOrEqualTo(65536));
                var directory=Path.GetFullPath(Path.Combine(Application.dataPath,"../../../chapter1-autonomous-20261021/evidence/hub-idle",_caseRunId));Directory.CreateDirectory(directory);
                using(var stream=new FileStream(Path.Combine(directory,"idle-120.raw.csv"),FileMode.CreateNew,FileAccess.Write,FileShare.Read)){stream.Write(bytes,0,bytes.Length);stream.Flush(true);}
            }
        }
        [UnityTest] public IEnumerator AuthoredHubSubmitStartsExpedition(){yield return Open();yield return _trace.Start(101);Assert.That(_trace.Flow.RunStartedRevision,Is.EqualTo(1));}
        [UnityTest] public IEnumerator AuthoredHubClickStartsExpedition(){yield return Open();yield return _trace.Start(null,true);Assert.That(_trace.Flow.RunStartedRevision,Is.EqualTo(1));}
        [UnityTest] public IEnumerator HeldSubmitDoesNotStartOrRestart()
        {
            _trace=new Chapter1AuthoredInputTrace();_trace.Keyboard(Key.Enter);yield return _trace.Open();
            for(var i=0;i<12;i++)yield return new WaitForFixedUpdate();Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);
            _trace.Keyboard();yield return _trace.WaitReady();Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);
        }
        [UnityTest] public IEnumerator ClickAndSubmitProduceOneRequest(){yield return Open();yield return _trace.Start(null,true,true);Assert.That(_trace.Flow.RunStartedRevision,Is.EqualTo(1));}
        [UnityTest] public IEnumerator CannotRebindActiveHubPort()
        {
            yield return Open();yield return _trace.WaitReady();var hub=_trace.Flow.CurrentHub;
            Assert.Throws<InvalidOperationException>(()=>hub.BindRequestPort(_trace.Flow));Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);
        }
        [UnityTest] public IEnumerator RejectForeignHubEndpointOrRouter()
        {
            yield return Open();yield return _trace.WaitReady();var hub=_trace.Flow.CurrentHub;
            Assert.That(hub.ValidatePendingRequest(new ForeignEndpoint(),null),Is.False);
            var retry=new Chapter1HubRequestToken(hub,hub.Router,new ForeignEndpoint(),hub.Generation,1,hub.Adapter.CurrentReceipt.Value,hub.Router.CurrentUiFrame.Value,"chapter1.retry-expedition-storage",null);
            Assert.That(hub.ValidatePendingRequest(_trace.Flow,retry),Is.False);_trace.Flow.HandleHubRequest(hub,retry);
            Assert.That(hub.SubmitUiIntent(hub.Router.CurrentUiFrame.Value,"chapter1.retry-expedition-storage"),Is.False);
            _trace.Flow.HandleHubRequest(null,null);Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);
        }
        [UnityTest] public IEnumerator RejectCopiedStaleOrConsumedHubToken()
        {
            yield return Open();yield return _trace.WaitReady();var hub=_trace.Flow.CurrentHub;var frame=hub.Router.CurrentUiFrame.Value;
            // 미발급 값 복사 음성일 뿐 정상 UI 입력을 대체하지 않는다.
            var copied=new Chapter1HubRequestToken(hub,hub.Router,_trace.Flow,hub.Generation,1,hub.Adapter.CurrentReceipt.Value,frame,"chapter1.start-expedition",101);
            Assert.That(hub.ValidatePendingRequest(_trace.Flow,copied),Is.False);_trace.Flow.HandleHubRequest(hub,copied);
            Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);Assert.Throws<InvalidOperationException>(()=>hub.ConsumePendingRequest(_trace.Flow,copied));
            var retry=new Chapter1HubRequestToken(hub,hub.Router,_trace.Flow,hub.Generation,1,hub.Adapter.CurrentReceipt.Value,frame,"chapter1.retry-expedition-storage",null);
            _trace.Flow.HandleHubRequest(hub,retry);Assert.That(hub.ValidatePendingRequest(_trace.Flow,retry),Is.False);
            Assert.Throws<InvalidOperationException>(()=>hub.ArmExpeditionRetryAfterFailure(_trace.Flow,retry));
            Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);
        }
        [UnityTest] public IEnumerator RejectHubRequestInWrongPhase()
        {yield return Open();yield return _trace.WaitReady();var hub=_trace.Flow.CurrentHub;var frame=hub.Router.CurrentUiFrame.Value;var prepared=hub.Adapter.CurrentReceipt.Value;
            yield return _trace.Start(101);var started=_trace.Flow.RunStartedRevision;_trace.Flow.HandleHubRequest(null,null);
            var retry=new Chapter1HubRequestToken(hub,hub.Router,_trace.Flow,hub.Generation,1,prepared,frame,"chapter1.retry-expedition-storage",null);
            _trace.Flow.HandleHubRequest(hub,retry);Assert.That(_trace.Flow.RunStartedRevision,Is.EqualTo(started));}
        [UnityTest] public IEnumerator AuthoredHubThroughFourRoomsToOrdan(){yield return RunNormalCase(101,0);}
        private IEnumerator RunNormalCase(int seed,int repetition)
        {_trace=new Chapter1AuthoredInputTrace();_trace.BeginEvidence(_caseRunId,true);yield return _trace.Open();yield return _trace.Start(seed);Assert.That(_trace.Flow.View.Seed,Is.EqualTo(seed));yield return ObserveNormalPath(seed,repetition);_trace.ValidateSocketEvidence();Assert.That(_trace.VisitedCount,Is.EqualTo(4));Assert.That(_trace.Flow.View.RoomId,Is.EqualTo("Ordan"));Assert.That(_trace.Flow.View.Health,Is.GreaterThan(0));
            Assert.That(_trace.HasBossEntry,Is.True);Assert.That(_trace.BossEntry.Generation,Is.EqualTo(_trace.LastRoom.Generation+1));
            Assert.That(_trace.BossEntry.Tick.Value,Is.EqualTo(_trace.LastRoom.Tick.Value+1));Assert.That(_trace.BossEntry.Health,Is.EqualTo(_trace.LastRoom.Health));
            var expectedInvulnerability=_trace.LastRoom.InvulnerabilityEndTick<_trace.BossEntry.Tick.Value?-1:_trace.LastRoom.InvulnerabilityEndTick;
            Assert.That(_trace.BossEntry.InvulnerabilityEndTick,Is.EqualTo(expectedInvulnerability));
            var router=_trace.Flow.Lifetime.CurrentRouter;var graph=router.transform.parent;
            var expected=graph.TransformPoint(new Vector3(-7,.81f,0));
            Assert.That(_trace.BossEntry.Position.x,Is.EqualTo(expected.x).Within(.02f));
            Assert.That(_trace.BossEntry.Position.y,Is.EqualTo(expected.y).Within(.02f));
            Assert.That(graph.GetComponentsInChildren<OrdanBossTerminalTransitionRequester>(true),Has.Length.EqualTo(1));
            Assert.Throws<InvalidOperationException>(()=>router.RegisterModeRequester(_trace.Flow.Lifetime));}
        // REQ/AC-AUTOCH1 G6/G7: 각각 별도 fixture와 격리 저장 runId로 발견된다.
        [UnityTest] public IEnumerator G6_Seed101_Repeat1(){yield return RunNormalCase(101,1);}
        [UnityTest] public IEnumerator G6_Seed101_Repeat2(){yield return RunNormalCase(101,2);}
        [UnityTest] public IEnumerator G6_Seed101_Repeat3(){yield return RunNormalCase(101,3);}
        [UnityTest] public IEnumerator G6_Seed202_Repeat1(){yield return RunNormalCase(202,1);}
        [UnityTest] public IEnumerator G6_Seed202_Repeat2(){yield return RunNormalCase(202,2);}
        [UnityTest] public IEnumerator G6_Seed202_Repeat3(){yield return RunNormalCase(202,3);}
        [UnityTest] public IEnumerator G6_Seed303_Repeat1(){yield return RunNormalCase(303,1);}
        [UnityTest] public IEnumerator G6_Seed303_Repeat2(){yield return RunNormalCase(303,2);}
        [UnityTest] public IEnumerator G6_Seed303_Repeat3(){yield return RunNormalCase(303,3);}
        [UnityTest] public IEnumerator G6_Seed404_Repeat1(){yield return RunNormalCase(404,1);}
        [UnityTest] public IEnumerator G6_Seed404_Repeat2(){yield return RunNormalCase(404,2);}
        [UnityTest] public IEnumerator G6_Seed404_Repeat3(){yield return RunNormalCase(404,3);}
        [UnityTest] public IEnumerator G7_Normal101(){yield return RunNormalCase(101,0);}
        [UnityTest] public IEnumerator G7_HealthZero(){yield return RunFailureCase(0);}
        [UnityTest] public IEnumerator G7_Fall(){yield return RunFailureCase(1);}
        [UnityTest] public IEnumerator G7_Crush(){yield return RunFailureCase(2);}
        private IEnumerator RunFailureCase(int cause)
        {
            _trace=new Chapter1AuthoredInputTrace();_trace.BeginEvidence(_caseRunId,false);yield return _trace.Open();yield return _trace.Start(cause==2?202:101);var oldKey=_trace.Flow.View.RunKey;
            var before=Chapter1AuthoredInputTrace.ReadCurrentTestProfile();var expected=checked(before.ProfileRevision+1);
            _trace.RecordEvidence("failure-before","run="+oldKey+" revision="+before.ProfileRevision+" expected="+expected);
            yield return _trace.PlayToFailure(cause);
            Assert.That(_trace.SawCommitBeforeRetirement,Is.True);
            Assert.That(_trace.Flow.RunEndCommittedRevision,Is.EqualTo(1));Assert.That(_trace.Flow.ReturnedToHubRevision,Is.EqualTo(1));
            var save=_trace.Flow.LastSaveResult;Assert.That(save.HasValue,Is.True);Assert.That(save.Value.Outcome,Is.EqualTo(ProfileAtomicSaveOutcome.CommittedReplacement));Assert.That(save.Value.CommittedRevision,Is.EqualTo(expected));
            var after=Chapter1AuthoredInputTrace.ReadCurrentTestProfile();Assert.That(after.ProfileRevision,Is.EqualTo(expected));
            Assert.That(after.Settings.Equals(before.Settings),Is.True);Assert.That(after.Input.BindingOverrides.CanonicalText,Is.EqualTo(before.Input.BindingOverrides.CanonicalText));Assert.That(after.Progression.CommittedChoice,Is.EqualTo(before.Progression.CommittedChoice));
            _trace.RecordEvidence("failure-saved","oldRun="+oldKey+" cause="+_trace.Flow.AcceptedFailureKind+" outcome="+save.Value.Outcome+" committed="+save.Value.CommittedRevision+" actual="+after.ProfileRevision+" commitBeforeRetirement="+_trace.SawCommitBeforeRetirement);
            yield return _trace.ConfirmReturn();_trace.RecordEvidence("returned-neutral","oldRun="+oldKey);yield return _trace.Start(101);Assert.That(_trace.Flow.View.RunKey,Is.Not.EqualTo(oldKey));_trace.RecordEvidence("new-run","oldRun="+oldKey+" newRun="+_trace.Flow.View.RunKey);
        }
        private IEnumerator ObserveNormalPath(int seed,int repetition)
        {
            var plan=_trace.Flow.GetComponent<Chapter1RoomCatalog>().ReadPlan(seed);
            var rows=new System.Text.StringBuilder("seed,repeat,runId,runKey,phase,room,index,generation,tick,health,planSha,socketExpectedOnly,frame\n");
            _trace.AccountAuxiliaryEvidence(rows.ToString());var iterator=_trace.PlayToBoss();var last="";
            try
            {
                while(iterator.MoveNext())
                {
                    var view=_trace.Flow.View;
                    if(view.Phase==Chapter1RunPhase.InRoom)
                    {
                        Assert.That(view.RoomId,Is.EqualTo(plan.RoomAt(view.RoomIndex)));
                        Assert.That(view.Seed,Is.EqualTo(seed));Assert.That(_trace.Flow.PlanSha,Is.EqualTo(plan.CanonicalSha));
                    }
                    var numericTick=view.Tick.HasValue?view.Tick.Value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture):"unobserved";
                    var key=view.Generation+":"+numericTick;
                    if(key!=last)
                    {
                        if(rows.Length>2*1024*1024)throw new InvalidOperationException("원정 원문 상한입니다.");
                        var rowStart=rows.Length;rows.Append(seed).Append(',').Append(repetition).Append(',').Append(_caseRunId).Append(',').Append(view.RunKey).Append(',').Append(view.Phase).Append(',').Append(view.RoomId).Append(',').Append(view.RoomIndex).Append(',').Append(view.Generation).Append(',').Append(numericTick).Append(',').Append(view.Health).Append(',').Append(_trace.Flow.PlanSha).Append(',').Append(view.Phase==Chapter1RunPhase.InRoom?plan.SocketAt(view.RoomIndex):"unobserved").Append(',').Append(Time.frameCount).AppendLine();_trace.AccountAuxiliaryEvidence(rows.ToString(rowStart,rows.Length-rowStart));last=key;
                    }
                    yield return iterator.Current;
                }
            }
            finally
            {
                var directory=Path.Combine(Directory.GetParent(Application.dataPath).FullName,"../../evidence/expedition-cases",_caseRunId);Directory.CreateDirectory(directory);
                var path=Path.Combine(directory,"seed-"+seed+"-repeat-"+repetition+".synthetic.csv");
                using(var stream=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.Read))
                {var bytes=new System.Text.UTF8Encoding(false).GetBytes(rows.ToString());stream.Write(bytes,0,bytes.Length);stream.Flush(true);}
            }
        }
        [UnityTest] public IEnumerator FourRoomsThenBossWithCarriedHealth(){yield return AuthoredHubThroughFourRoomsToOrdan();}
        [UnityTest] public IEnumerator AuthoredFailureReturnsAndAllowsFreshStart()
        {
            // 원인3의 실제 장치 trace는 각각 별도 저작 실행으로 닫는다. 직접 피해/HP 설정은 없다.
            for(var cause=0;cause<3;cause++)
            {
                if(_trace!=null){yield return _trace.CloseForReopen();_trace=null;}
                yield return Open();yield return _trace.Start(cause==2?202:101);var oldKey=_trace.Flow.View.RunKey;
                yield return _trace.PlayToFailure(cause);Assert.That(_trace.Flow.RunEndCommittedRevision,Is.EqualTo(1));Assert.That(_trace.Flow.ReturnedToHubRevision,Is.EqualTo(1));
                yield return _trace.ConfirmReturn();yield return _trace.Start(101);Assert.That(_trace.Flow.View.RunKey,Is.Not.EqualTo(oldKey));
            }
        }
        [UnityTest] public IEnumerator FailureCommitBeforeDestroy(){yield return Open();yield return _trace.Start(101);yield return _trace.PlayToFailure(0);Assert.That(_trace.SawCommitBeforeRetirement,Is.True);}
        [UnityTest] public IEnumerator RestartDoesNotResumeRun()
        {yield return Open();yield return _trace.Start(101);yield return _trace.CloseForReopen();_trace=null;yield return Open();yield return _trace.WaitReady();Assert.That(_trace.Flow.View.RunKey,Is.Null);Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);}
        [UnityTest] public IEnumerator AuthoredReturnPreservesDevelopmentProfile()
        {
            yield return Open();yield return _trace.WaitReady();var before=new Chapter1DevelopmentProfile(_trace.Flow.CurrentHub).Snapshot;
            yield return _trace.Start(101);yield return _trace.PlayToFailure(0);yield return _trace.WaitReady();var after=new Chapter1DevelopmentProfile(_trace.Flow.CurrentHub).Snapshot;
            Assert.That(after.Settings.Equals(before.Settings),Is.True);
            Assert.That(after.Input.InputActionsAssetId,Is.EqualTo(before.Input.InputActionsAssetId));Assert.That(after.Input.BindingSchemaVersion,Is.EqualTo(before.Input.BindingSchemaVersion));
            Assert.That(after.Input.BindingOverrides.CanonicalText,Is.EqualTo(before.Input.BindingOverrides.CanonicalText));Assert.That(after.Tutorial.ConfirmedIds,Is.EqualTo(before.Tutorial.ConfirmedIds));
            Assert.That(after.Progression.CommittedChoice,Is.EqualTo(before.Progression.CommittedChoice));Assert.That(after.Progression.GrantedSkill,Is.EqualTo(before.Progression.GrantedSkill));
            Assert.That(after.Progression.ConsentState,Is.EqualTo(before.Progression.ConsentState));Assert.That(after.Progression.CompletedBranches,Is.EqualTo(before.Progression.CompletedBranches));
            Assert.That(after.ProfileRevision,Is.GreaterThan(before.ProfileRevision));
        }
        // 실제 파일 공유 잠금은 공개 저장 소유자의 정상 실패를 만든다. 내부 fault/저장 우회는 없다.
        [UnityTest] public IEnumerator SaveFailureShowsErrorWithoutRunStarted()
        {
            yield return Open();yield return _trace.WaitReady();
            var hub=_trace.Flow.CurrentHub;var before=new Chapter1DevelopmentProfile(hub).Snapshot;
            var primary=Path.Combine(Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot),"profile.json");
            LogAssert.Expect(LogType.Error,new Regex("^출정 저장 실패: "));
            using(var held=new FileStream(primary,FileMode.Open,FileAccess.Read,FileShare.Read))
            {yield return _trace.SendStartEdge(101,keepHeld:true);yield return _trace.WaitExpeditionStorageFailed();}
            Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);Assert.That(_trace.Flow.View.RunKey,Is.Null);
            AssertPreCommitFailure(before.ProfileRevision+1);
            yield return RecoverStorageAndStart(false);
            // 실제 Read/Read 잠금은 재관측을 허용하고 공개 저장의 배타 primary 검증에서 실패해야 한다.
        }
        // AC-C1B-SRETRY-001/002/004/005: 읽기 불가 결과를 저장 실패 결과로 만들어 채우지 않는다.
        [UnityTest] public IEnumerator StorageReadUnavailableRequiresFreshHubAndNewStart()
        {
            yield return Open();yield return _trace.WaitReady();
            var primary=Path.Combine(Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot),"profile.json");
            LogAssert.Expect(LogType.Error,new Regex("^출정 저장 실패: "));
            using(var held=new FileStream(primary,FileMode.Open,FileAccess.Read,FileShare.None))
            {yield return _trace.SendStartEdge(null,keepHeld:true);yield return _trace.WaitExpeditionStorageFailed();}
            var failure=_trace.Flow.View.LastStorageFailure;Assert.That(failure,Is.Not.Null);
            Assert.That(failure.Failure,Is.EqualTo(Chapter1ExpeditionStorageFailureKind.ReadUnavailableNoSaveIssued));
            Assert.That(failure.Observation.HasValue,Is.True);Assert.That(failure.Observation.Value.Primary.Kind,Is.EqualTo(ProfileLoadCandidateKind.Unreadable));
            Assert.That(failure.Save.HasValue,Is.False);Assert.That(failure.ExpectedRevision.HasValue,Is.False);Assert.That(failure.Plan.HasValue,Is.False);
            Assert.That(_trace.Flow.LastSaveResult.HasValue,Is.False);AssertUnstartedStorageFailure();
            RecordStorageFailureTrace(failure);
            yield return RecoverStorageAndStart(false);
        }
        // AC-C1B-SRETRY-001/002/004/005: held Click/Navigate, release frame edge, click+submit 중복과 새 Hub held를 연속 관측한다.
        [UnityTest] public IEnumerator HeldClickSaveFailureRequiresReleaseAndFreshStart()
        {
            yield return Open();yield return _trace.WaitReady();var before=new Chapter1DevelopmentProfile(_trace.Flow.CurrentHub).Snapshot;
            var primary=Path.Combine(Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot),"profile.json");
            LogAssert.Expect(LogType.Error,new Regex("^출정 저장 실패: "));
            using(var held=new FileStream(primary,FileMode.Open,FileAccess.Read,FileShare.Read))
            {yield return _trace.SendStartEdge(null,true,keepHeld:true);yield return _trace.WaitExpeditionStorageFailed();}
            AssertPreCommitFailure(before.ProfileRevision+1);_trace.Keyboard(Key.DownArrow);
            yield return RecoverStorageAndStart(true,true);
        }
        // AC-C1B-SRETRY-006: 실제 불확실 commit은 재확인 가능한 상태가 아니다.
        [UnityTest] public IEnumerator ExpeditionSaveUncertainRemainsDiagnostic()
        {
            // 실제 첫 출정의 replacement로 이전 저장 파일을 마련하고, 별도 새 scene의 현재 시도를 관측한다.
            yield return Open();yield return _trace.Start(101);Assert.That(_trace.Flow.RunStartedRevision,Is.EqualTo(1));
            yield return _trace.CloseForReopen();_trace=null;yield return Open();yield return _trace.WaitReady();
            var hub=_trace.Flow.CurrentHub;var generation=hub.Generation;
            var previous=Path.Combine(Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot),"profile.prev.json");
            LogAssert.Expect(LogType.Error,new Regex("^원정 연결 중단: "));
            using(var held=new FileStream(previous,FileMode.Open,FileAccess.Read,FileShare.Read))
            {yield return _trace.SendStartEdge(101,keepHeld:true);yield return _trace.WaitDiagnostic();}
            Assert.That(_trace.Flow.LastSaveResult.HasValue,Is.True);
            Assert.That(_trace.Flow.LastSaveResult.Value.Outcome,Is.EqualTo(ProfileAtomicSaveOutcome.CommitOutcomeUncertain));
            Assert.That(_trace.Flow.LastSaveResult.Value.CommittedRevision,Is.EqualTo(-1));Assert.That(_trace.Flow.View.LastStorageFailure,Is.Null);
            Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Diagnostic));Assert.That(hub.StorageRetryArmed,Is.False);
            _trace.ReleaseInputsWithSubmitEdge();yield return new WaitForFixedUpdate();_trace.Keyboard(Key.Enter);yield return new WaitForFixedUpdate();
            Assert.That(hub.SubmitUiIntent(hub.Router.CurrentUiFrame.Value,"chapter1.retry-expedition-storage"),Is.False);
            Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Diagnostic));Assert.That(_trace.Flow.CurrentHub.Generation,Is.EqualTo(generation));
            Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);Assert.That(_trace.Flow.View.RunKey,Is.Null);Assert.That(_trace.Flow.Lifetime.CurrentRouter,Is.Null);
            Assert.That(_trace.Flow.StorageRetryToken,Is.Null);
        }
        private void AssertUnstartedStorageFailure()
        {
            Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.ExpeditionStorageFailed));Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);
            Assert.That(_trace.Flow.View.RunKey,Is.Null);Assert.That(_trace.Flow.Lifetime.CurrentRouter,Is.Null);
            Assert.That(_trace.Flow.View.Error,Does.StartWith("저장에 실패하여 출정하지 못했습니다\n"));
            Assert.That(_trace.Flow.View.Error,Is.EqualTo(_trace.Flow.View.LastStorageFailure.Error));
            Assert.That(_trace.Flow.View.Error.Length,Is.GreaterThan("저장에 실패하여 출정하지 못했습니다\n".Length));
            Assert.That(_trace.Flow.View.LastStorageFailure.Token.NeutralReceipt.HasValue,Is.True);
        }
        private void AssertPreCommitFailure(long expected)
        {
            AssertUnstartedStorageFailure();Assert.That(_trace.Flow.LastSaveResult.HasValue,Is.True);
            var result=_trace.Flow.LastSaveResult.Value;result.Validate();
            Assert.That(result.Outcome,Is.EqualTo(ProfileAtomicSaveOutcome.FailedBeforeCommit));
            Assert.That(result.FailureStage,Is.EqualTo(ProfileAtomicSaveStage.ExistingPrimaryValidation));Assert.That(result.CommittedRevision,Is.EqualTo(-1));
            var failure=_trace.Flow.View.LastStorageFailure;
            Assert.That(failure.Failure,Is.EqualTo(Chapter1ExpeditionStorageFailureKind.FailedBeforeCommit));Assert.That(failure.ExpectedRevision,Is.EqualTo(expected));
            Assert.That(failure.Save.HasValue,Is.True);Assert.That(failure.Save.Value.Equals(result),Is.True);
            RecordStorageFailureTrace(failure);
        }
        private static void RecordStorageFailureTrace(Chapter1ExpeditionStorageAttempt value)
        {
            Debug.Log("실제 출정 저장 실패 사실: 시도="+value.Ordinal+" 요청="+value.Token.Ordinal+" 거점세대="+value.Token.Generation+
                " 인증경로="+value.AuthenticatedRoot+" 입력갱신="+value.Token.InputUpdate+" 요청틱="+value.Token.Frame.Receipt.Tick.Value+
                " 요청프레임="+value.Token.Frame.Receipt.FrameOrdinal+" 이전중립틱="+value.Token.NeutralReceipt.Value.Tick.Value+
                " prepared출처개정="+value.Token.Prepared.SourceRevision+" prepared최종개정="+value.Token.Prepared.FinalRevision+
                " 분류="+value.Failure+" 관측주="+value.Observation.Value.Primary.Kind+" 관측이전="+value.Observation.Value.Previous.Kind+" 관측임시="+value.Observation.Value.Temp.Kind+
                " 기대개정="+(value.ExpectedRevision.HasValue?value.ExpectedRevision.Value.ToString():"미발급")+
                " 결과="+(value.Save.HasValue?value.Save.Value.Outcome+"/"+value.Save.Value.FailureStage+"/"+value.Save.Value.PrimaryState+"/"+value.Save.Value.PreviousState+"/"+value.Save.Value.TempState+"/"+value.Save.Value.CommittedRevision:"미발급")+
                " 시도전시작개정="+value.StartedRevision+" 오류="+value.Error);
        }
        private IEnumerator RecoverStorageAndStart(bool click,bool together=false)
        {
            var flow=_trace.Flow;var oldHub=flow.CurrentHub;var oldRouter=oldHub.Router;var oldGeneration=oldHub.Generation;
            var failure=flow.View.LastStorageFailure;var close=oldRouter.GetChapter1HubClosureReader();var save=flow.LastSaveResult;
            for(var i=0;i<6;i++){yield return new WaitForFixedUpdate();AssertUnstartedStorageFailure();Assert.That(flow.CurrentHub,Is.SameAs(oldHub));
                Assert.That(oldHub.Ready,Is.False);Assert.That(flow.StorageRetryToken,Is.Null);Assert.That(flow.LastSaveResult.Equals(save),Is.True);}
            // 잠금 해제 자체·held·Navigate는 복구 요청이나 새 저장을 만들지 않는다.
            _trace.ReleaseInputsWithSubmitEdge();yield return _trace.WaitReady();
            var neutral=oldRouter.Chapter1HubNeutralReceipt;Assert.That(neutral.HasValue,Is.True);
            Assert.That(neutral.Value.Tick.Value,Is.GreaterThan(failure.Token.Frame.Receipt.Tick.Value));
            Assert.That(neutral.Value.FrameOrdinal,Is.GreaterThan(failure.Token.Frame.Receipt.FrameOrdinal));
            Assert.That(oldRouter.Chapter1HubNeutralInputUpdate,Is.GreaterThan(failure.Token.InputUpdate));
            Assert.That(oldRouter.CurrentUiFrame.Value.SubmitPressed || oldRouter.CurrentUiFrame.Value.ClickPressed,Is.False);
            var neutralUpdate=oldRouter.Chapter1HubNeutralInputUpdate;
            yield return _trace.SendStorageRetryEdge(click,together);
            for(var i=0;i<180;i++)
            {
                yield return new WaitForFixedUpdate();Assert.That(flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),flow.View.Error);
                if(flow.CurrentHub!=null && flow.CurrentHub.Generation==oldGeneration+1 && flow.StorageRetryPrepared.HasValue)break;
                if(i==179)Assert.Fail("실제 old closure 뒤 새 prepared 거점을 얻지 못했습니다.");
            }
            var fresh=flow.CurrentHub;Assert.That(close(),Is.True);Assert.That(fresh,Is.Not.SameAs(oldHub));Assert.That(fresh.Router,Is.Not.SameAs(oldRouter));
            Assert.That(flow.StorageRetryToken.Intent,Is.EqualTo("chapter1.retry-expedition-storage"));Assert.That(flow.StorageRetryToken.Seed,Is.Null);
            Assert.That(flow.StorageRetryToken.Ordinal,Is.EqualTo(failure.Token.Ordinal+1));
            Assert.That(flow.StorageRetryToken.Frame.Receipt.Tick.Value,Is.GreaterThan(neutral.Value.Tick.Value));
            Assert.That(flow.StorageRetryToken.Frame.Receipt.FrameOrdinal,Is.GreaterThan(neutral.Value.FrameOrdinal));
            Assert.That(InputState.updateCount,Is.GreaterThan(neutralUpdate));
            Assert.That(flow.StorageRetryToken.InputUpdate,Is.GreaterThan(neutralUpdate));
            Assert.That(flow.StorageRetryPrepared.Value.PersistencePending,Is.False);
            Assert.That(flow.View.LastStorageFailure,Is.SameAs(failure));Assert.That(flow.LastSaveResult.Equals(save),Is.True);
            for(var i=0;i<6;i++){yield return new WaitForFixedUpdate();Assert.That(fresh.Ready,Is.False);Assert.That(flow.View.Phase,Is.EqualTo(Chapter1RunPhase.PreparingHubAfterStorageRetry));
                Assert.That(flow.RunStartedRevision,Is.Zero);Assert.That(flow.View.RunKey,Is.Null);Assert.That(flow.Lifetime.CurrentRouter,Is.Null);}
            _trace.ReleaseInputsWithSubmitEdge();yield return _trace.WaitReady();
            Assert.That(flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Hub));Assert.That(fresh.Router.Chapter1HubNeutralReceipt.HasValue,Is.True);
            Assert.That(fresh.Router.CurrentUiFrame.Value.SubmitPressed || fresh.Router.CurrentUiFrame.Value.ClickPressed,Is.False);
            Assert.That(flow.RunStartedRevision,Is.Zero);Assert.That(flow.ReturnedToHubRevision,Is.Zero);Assert.That(flow.RunEndCommittedRevision,Is.Zero);
            var before=new Chapter1DevelopmentProfile(fresh).Snapshot;var freshNeutral=fresh.Router.Chapter1HubNeutralReceipt.Value;
            yield return _trace.Start(null,click);
            Assert.That(flow.RunStartedRevision,Is.EqualTo(1));Assert.That(flow.LastSaveResult.HasValue,Is.True);
            Assert.That(Chapter1DevelopmentProfile.IsExactCommit(flow.LastSaveResult.Value,before.ProfileRevision+1),Is.True);
            Assert.That(flow.View.CurrentStorageAttempt.ExpectedRevision,Is.EqualTo(before.ProfileRevision+1));
            Assert.That(flow.View.CurrentStorageAttempt.Ordinal,Is.EqualTo(failure.Ordinal+1));Assert.That(flow.View.LastStorageFailure,Is.SameAs(failure));
            Assert.That(flow.View.CurrentStorageAttempt.Token,Is.Not.SameAs(failure.Token));
            Assert.That(flow.View.CurrentStorageAttempt.Token.Frame.Receipt.Tick.Value,Is.GreaterThan(freshNeutral.Tick.Value));
            Assert.That(flow.View.CurrentStorageAttempt.Token.Frame.Receipt.FrameOrdinal,Is.GreaterThan(freshNeutral.FrameOrdinal));
            Assert.That(flow.View.CurrentStorageAttempt.Token.NeutralReceipt.Value,Is.EqualTo(freshNeutral));
            Assert.That(flow.Lifetime.TryGetCompletedEvidence(out var completed),Is.True);Assert.That(completed.RunKey,Is.EqualTo(flow.View.RunKey));
            Debug.Log("실제 저장 재확인 trace: 실패시도="+failure.Ordinal+" 요청="+failure.Token.Ordinal+" 구세대="+oldGeneration+" 새세대="+fresh.Generation+
                " 중립갱신="+neutralUpdate+" 준비출처개정="+flow.StorageRetryPrepared.Value.SourceRevision+" 준비최종개정="+flow.StorageRetryPrepared.Value.FinalRevision+
                " 준비확정개정="+flow.StorageRetryPrepared.Value.CommittedRevision+" 준비보존="+flow.StorageRetryPrepared.Value.Preservation.TempState+"/"+flow.StorageRetryPrepared.Value.Preservation.PrimaryState+"/"+flow.StorageRetryPrepared.Value.Preservation.PreviousState+
                " 출정기대개정="+flow.View.CurrentStorageAttempt.ExpectedRevision+" 출정확정개정="+flow.LastSaveResult.Value.CommittedRevision+
                " 첫완료틱="+completed.Tick.Value+" 시작개정="+flow.RunStartedRevision);
        }
        [UnityTest] public IEnumerator FailedSaveBlocksReturn()
        {
            yield return Open();yield return _trace.Start(101);yield return _trace.PlayUntilFailureAccepted();
            var temp=Path.Combine(Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot),"profile.tmp.json");
            Assert.That(File.Exists(temp),Is.False);LogAssert.Expect(LogType.Error,new Regex("^원정 연결 중단: "));
            try
            {
                using(var held=new FileStream(temp,FileMode.CreateNew,FileAccess.ReadWrite,FileShare.None))yield return _trace.WaitDiagnostic();
                Assert.That(_trace.Flow.LastSaveResult.HasValue,Is.True);
                Assert.That(_trace.Flow.LastSaveResult.Value.Outcome,Is.EqualTo(ProfileAtomicSaveOutcome.FailedBeforeCommit));
                Assert.That(_trace.Flow.RunEndCommittedRevision,Is.EqualTo(1));Assert.That(_trace.Flow.ReturnedToHubRevision,Is.Zero);
            }
            finally{if(File.Exists(temp))File.Delete(temp);} // 이 시험이 만든 지정 leaf만 제거한다.
        }
        [UnityTest] public IEnumerator AuthoredSaveUncertainDoesNotPublishReturned()
        {
            yield return Open();yield return _trace.Start(101);yield return _trace.PlayUntilFailureAccepted();
            var previous=Path.Combine(Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot),"profile.prev.json");
            LogAssert.Expect(LogType.Error,new Regex("^원정 연결 중단: "));
            using(var held=new FileStream(previous,FileMode.Open,FileAccess.Read,FileShare.Read))yield return _trace.WaitDiagnostic();
            Assert.That(_trace.Flow.LastSaveResult.HasValue,Is.True);
            Assert.That(_trace.Flow.LastSaveResult.Value.Outcome,Is.EqualTo(ProfileAtomicSaveOutcome.CommitOutcomeUncertain));
            Assert.That(_trace.Flow.LastSaveResult.Value.CommittedRevision,Is.EqualTo(-1));
            Assert.That(_trace.Flow.ReturnedToHubRevision,Is.Zero);Assert.That(_trace.Flow.CurrentHub,Is.Null);
            // Windows의 실제 File.Replace backup 공유 잠금 경계를 검사한다. 실패 결과를 성공으로 채우지 않는다.
        }
        [UnityTest] public IEnumerator SingleReturnForConcurrentCauses()
        {
            yield return Open();yield return _trace.Start(202);yield return _trace.PlayUntilConcurrentFailure();
            Assert.That(_trace.Flow.AcceptedFailureCauseCount,Is.GreaterThan(1));
            yield return _trace.WaitReturned();Assert.That(_trace.Flow.RunEndCommittedRevision,Is.EqualTo(1));Assert.That(_trace.Flow.ReturnedToHubRevision,Is.EqualTo(1));
        }
#if UNITY_EDITOR
        [UnityTest] public IEnumerator RejectReentrantRequestBeforePersistence()
        {
            var staging=new GameObject("ActualReentryHubStage");staging.SetActive(false);
            try
            {
                var prefab=UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/Hub.prefab");Assert.That(prefab,Is.Not.Null);
                var graph=UnityEngine.Object.Instantiate(prefab,staging.transform,false);var hub=graph.GetComponent<Chapter1HubLifetimeOwner>();
                var endpoint=new ReentryEndpoint();hub.ConfigureInstanceGeneration(1);hub.BindRequestPort(endpoint);graph.SetActive(true);
                var keyboard=InputSystem.AddDevice<Keyboard>();InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();staging.SetActive(true);
                for(var i=0;i<120 && !hub.Ready;i++)yield return new WaitForFixedUpdate();Assert.That(hub.Ready,Is.True);
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Enter));InputSystem.Update();yield return new WaitForFixedUpdate();
                var frame=hub.Router.CurrentUiFrame.Value;Assert.That(frame.SubmitPressed,Is.True);
                Assert.That(hub.SubmitUiIntent(frame,"chapter1.validation-seed-101"),Is.True);
                Assert.That(endpoint.Requests,Is.EqualTo(1));Assert.That(endpoint.NestedAccepted,Is.False);
                Assert.That(endpoint.Save.HasValue,Is.True);Assert.That(Chapter1DevelopmentProfile.IsExactCommit(endpoint.Save.Value,endpoint.Before+1),Is.True);
            }
            finally{staging.SetActive(false);UnityEngine.Object.DestroyImmediate(staging);}
            // source-bound UI 포트의 동기 재진입 집중 fixture. 정상 원정 종단을 대체하지 않는다.
        }
        // AC-C1B-SRETRY-003: 정상 저장 성공을 대체하지 않는 실제 장치·동기 endpoint 보안 경계다.
        [UnityTest] public IEnumerator RejectCopiedStaleOrConsumedStorageRetryToken()
        {
            var staging=new GameObject("ActualStorageRetryBoundaryStage");staging.SetActive(false);
            try
            {
                var prefab=UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/Hub.prefab");Assert.That(prefab,Is.Not.Null);
                var graph=UnityEngine.Object.Instantiate(prefab,staging.transform,false);var hub=graph.GetComponent<Chapter1HubLifetimeOwner>();
                var inactiveForeign=UnityEngine.Object.Instantiate(prefab,staging.transform,false);inactiveForeign.SetActive(false);
                var endpoint=new StorageRetryBoundaryEndpoint{ForeignHub=inactiveForeign.GetComponent<Chapter1HubLifetimeOwner>()};
                hub.ConfigureInstanceGeneration(2);hub.BindRequestPort(endpoint);graph.SetActive(true);
                var keyboard=InputSystem.AddDevice<Keyboard>();InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();staging.SetActive(true);
                for(var i=0;i<120 && !hub.Ready;i++)yield return new WaitForFixedUpdate();Assert.That(hub.Ready,Is.True);
                Assert.That(hub.SubmitUiIntent(hub.Router.CurrentUiFrame.Value,"chapter1.retry-expedition-storage"),Is.False);
                endpoint.NeutralFrame=hub.Router.CurrentUiFrame.Value;
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Enter));InputSystem.Update();yield return new WaitForFixedUpdate();
                var frame=hub.Router.CurrentUiFrame.Value;Assert.That(frame.SubmitPressed,Is.True);
                Assert.That(hub.SubmitUiIntent(frame,"chapter1.validation-seed-101"),Is.True);Assert.That(endpoint.Requests,Is.EqualTo(1));
                Assert.That(endpoint.Rearms,Is.EqualTo(1));Assert.That(hub.Ready,Is.False);
                Assert.Throws<InvalidOperationException>(()=>hub.ArmExpeditionRetryAfterFailure(endpoint,endpoint.StartToken));
                Assert.Throws<InvalidOperationException>(()=>hub.Router.RearmChapter1HubNeutralAfterStorageFailure(hub,endpoint.StartToken));
                for(var i=0;i<6;i++){yield return new WaitForFixedUpdate();Assert.That(hub.Ready,Is.False);Assert.That(endpoint.Requests,Is.EqualTo(1));}
                // 실제 동일 update의 누름→release edge를 중립 frame에서 폐기한다.
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Enter));
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();
                for(var i=0;i<120 && !hub.Ready;i++)yield return new WaitForFixedUpdate();Assert.That(hub.Ready,Is.True);
                Assert.That(hub.Router.CurrentUiFrame.Value.SubmitPressed,Is.False);
                Assert.That(hub.SubmitUiIntent(hub.Router.CurrentUiFrame.Value,"chapter1.start-expedition"),Is.False);
                var neutral=hub.Router.Chapter1HubNeutralReceipt.Value;
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Enter));InputSystem.Update();yield return new WaitForFixedUpdate();
                frame=hub.Router.CurrentUiFrame.Value;Assert.That(frame.SubmitPressed,Is.True);
                Assert.That(frame.Receipt.Tick.Value,Is.GreaterThan(neutral.Tick.Value));Assert.That(frame.Receipt.FrameOrdinal,Is.GreaterThan(neutral.FrameOrdinal));
                Assert.That(hub.SubmitUiIntent(frame,"chapter1.retry-expedition-storage"),Is.True);
                Assert.That(endpoint.Requests,Is.EqualTo(2));Assert.That(endpoint.Rearms,Is.EqualTo(1));Assert.That(endpoint.RetryToken.Seed,Is.Null);
                Assert.That(hub.ValidatePendingRequest(endpoint,endpoint.RetryToken),Is.False);
                Assert.That(hub.SubmitUiIntent(frame,"chapter1.retry-expedition-storage"),Is.False);
                Assert.Throws<InvalidOperationException>(()=>hub.ConsumePendingRequest(endpoint,endpoint.RetryToken));
                Assert.Throws<InvalidOperationException>(()=>hub.ArmExpeditionRetryAfterFailure(endpoint,endpoint.RetryToken));
            }
            finally{staging.SetActive(false);UnityEngine.Object.DestroyImmediate(staging);}
        }
#endif
        private sealed class StorageRetryBoundaryEndpoint:IChapter1HubRequestPort
        {
            internal int Requests,Rearms;internal Chapter1HubRequestToken StartToken,RetryToken;
            internal Chapter1HubLifetimeOwner ForeignHub;internal UiSemanticFrameV1 NeutralFrame;
            public void HandleHubRequest(Chapter1HubLifetimeOwner source,Chapter1HubRequestToken token)
            {
                Requests++;Assert.That(source.ValidatePendingRequest(this,token),Is.True);
                Assert.That(source.SubmitUiIntent(token.Frame,"chapter1.retry-expedition-storage"),Is.False);
                if(Requests==1)
                {
                    StartToken=token;Assert.Throws<InvalidOperationException>(()=>source.ArmExpeditionRetryAfterFailure(this,token));
                    var copied=new Chapter1HubRequestToken(source,token.Router,this,token.Generation,token.Ordinal,token.Prepared,token.Frame,token.Intent,token.Seed);
                    var otherEndpoint=new ForeignEndpoint();
                    var foreignEndpoint=new Chapter1HubRequestToken(source,token.Router,otherEndpoint,token.Generation,token.Ordinal,token.Prepared,token.Frame,token.Intent,token.Seed);
                    var foreignRouter=new Chapter1HubRequestToken(source,ForeignHub.Router,this,token.Generation,token.Ordinal,token.Prepared,token.Frame,token.Intent,token.Seed);
                    var foreignSource=new Chapter1HubRequestToken(ForeignHub,token.Router,this,token.Generation,token.Ordinal,token.Prepared,token.Frame,token.Intent,token.Seed);
                    var staleGeneration=new Chapter1HubRequestToken(source,token.Router,this,token.Generation-1,token.Ordinal,token.Prepared,token.Frame,token.Intent,token.Seed);
                    var staleFrame=new Chapter1HubRequestToken(source,token.Router,this,token.Generation,token.Ordinal,token.Prepared,NeutralFrame,token.Intent,token.Seed);
                    foreach(var invalid in new[]{copied,foreignEndpoint,foreignRouter,foreignSource,staleGeneration,staleFrame})
                    {Assert.That(source.ValidatePendingRequest(this,invalid),Is.False);Assert.Throws<InvalidOperationException>(()=>source.ArmExpeditionRetryAfterFailure(this,invalid));}
                    source.ConsumePendingRequest(this,token);
                    Assert.Throws<InvalidOperationException>(()=>source.ArmExpeditionRetryAfterFailure(otherEndpoint,token));
                    foreach(var invalid in new[]{copied,foreignEndpoint,foreignRouter,foreignSource,staleGeneration,staleFrame})
                        Assert.Throws<InvalidOperationException>(()=>source.ArmExpeditionRetryAfterFailure(this,invalid));
                    source.ArmExpeditionRetryAfterFailure(this,token);Rearms++;
                    Assert.Throws<InvalidOperationException>(()=>source.ArmExpeditionRetryAfterFailure(this,token));
                }
                else
                {
                    RetryToken=token;Assert.That(token.Intent,Is.EqualTo("chapter1.retry-expedition-storage"));
                    Assert.That(source.ValidatePendingRequest(this,StartToken),Is.False);
                    Assert.Throws<InvalidOperationException>(()=>source.ArmExpeditionRetryAfterFailure(this,StartToken));
                    source.ConsumePendingRequest(this,token);
                    Assert.Throws<InvalidOperationException>(()=>source.ArmExpeditionRetryAfterFailure(this,token));
                }
                Assert.Throws<InvalidOperationException>(()=>source.ConsumePendingRequest(this,token));
            }
        }
        private sealed class ReentryEndpoint:IChapter1HubRequestPort
        {
            internal int Requests;internal bool NestedAccepted;internal long Before;internal ProfileAtomicSaveResultV1? Save;
            public void HandleHubRequest(Chapter1HubLifetimeOwner source,Chapter1HubRequestToken token)
            {
                Requests++;Assert.That(source.ValidatePendingRequest(this,token),Is.True);
                NestedAccepted=source.SubmitUiIntent(token.Frame,token.Intent);
                Assert.That(source.SubmitUiIntent(token.Frame,"chapter1.retry-expedition-storage"),Is.False);
                source.ConsumePendingRequest(this,token);
                Assert.Throws<InvalidOperationException>(()=>source.ConsumePendingRequest(this,token));
                var profile=new Chapter1DevelopmentProfile(source);Before=profile.Snapshot.ProfileRevision;Save=profile.SaveForExpedition(101,true);
            }
        }
        private sealed class ForeignEndpoint:IChapter1HubRequestPort{public void HandleHubRequest(Chapter1HubLifetimeOwner source,Chapter1HubRequestToken token){}}
    }

    internal sealed class Chapter1AuthoredInputTrace
    {
        private Chapter1InitialSocketObserver _socketObserver;
        private GameObject _evidenceObject;
        private string _evidenceRunId;
        private bool _forbidTech;
        private readonly System.Collections.Generic.List<string> _evidenceRows=new System.Collections.Generic.List<string>();
        private int _evidenceBytes,_totalEvidenceRows;
        internal void AccountAuxiliaryEvidence(string payload){if(_evidenceRunId==null)return;var bytes=System.Text.Encoding.UTF8.GetByteCount(payload);if(_totalEvidenceRows>=20000 || _evidenceBytes+bytes>16*1024*1024)throw new InvalidOperationException("사례 전체 원문 행/바이트 상한입니다.");_totalEvidenceRows++;_evidenceBytes+=bytes;}
        private long _issuedSequence,_completedIssued;
        private string _lastCompletedCorrelation;
        internal void RecordQueueSuccess(){if(_evidenceRunId==null)return;_issuedSequence=checked(_issuedSequence+1);RecordEvidence("queue-succeeded","sequence="+_issuedSequence+" consumption=unobserved");}
        internal void RecordCompletedCorrelation()
        {if(_evidenceRunId==null || Flow==null || Flow.Lifetime==null || !Flow.Lifetime.TryGetCompletedEvidence(out var value))return;var key=value.Generation+":"+value.Tick.Value;if(key==_lastCompletedCorrelation)return;if(_issuedSequence<_completedIssued)throw new InvalidOperationException("입력 발행 서수가 역행했습니다.");RecordEvidence("completed-result","run="+value.RunKey+" generation="+value.Generation+" tick="+value.Tick.Value+" health="+value.Health+" issuedAfter="+_completedIssued+" issuedThrough="+_issuedSequence+" correlation=interval-not-one-event-per-tick");_completedIssued=_issuedSequence;_lastCompletedCorrelation=key;}
        internal void BeginEvidence(string runId,bool forbidTech)
        {_evidenceRunId=runId;_forbidTech=forbidTech;_evidenceObject=new GameObject("CH1-test-only-socket-observer");_socketObserver=_evidenceObject.AddComponent<Chapter1InitialSocketObserver>();_socketObserver.Trace=this;}
        internal void RecordEvidence(string stage,string value)
        {
            if(_evidenceRunId==null)return;
            var context="run=unobserved generation=unobserved tick=unobserved";if(Flow!=null && Flow.Lifetime!=null && Flow.Lifetime.CurrentRouter!=null){var view=Flow.View;context="run="+view.RunKey+" generation="+Flow.Lifetime.Generation+" tick="+(view.Tick.HasValue?view.Tick.Value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture):"unobserved");}
            var line="synthetic|"+context+"|"+stage+"|runId="+_evidenceRunId+"|frame="+Time.frameCount+"|update="+InputState.currentUpdateType+"|count="+InputState.updateCount+"|"+value;
            var bytes=System.Text.Encoding.UTF8.GetByteCount(line)+1;
            if(_totalEvidenceRows>=20000 || _evidenceBytes+bytes>16*1024*1024)throw new InvalidOperationException("시험 원문 행/바이트 상한입니다.");
            _evidenceRows.Add(line);_totalEvidenceRows++;_evidenceBytes+=bytes;
        }
        internal void ValidateSocketEvidence(){if(_socketObserver==null)throw new InvalidOperationException("소켓 관찰자가 없습니다.");_socketObserver.ValidateComplete();}
        private void RecordKeyboard(Key[] keys)
        {RecordEvidence("queue-keyboard","nextSequence="+(_issuedSequence+1)+" device="+_keyboard.deviceId+" keys="+string.Join(";",keys));if(_forbidTech && Array.IndexOf(keys,Key.Q)>=0)throw new InvalidOperationException("G6 선택기술 발행은 금지됩니다.");}
        private void RecordPointer(Vector2 point,bool attack,bool transfer)
        {RecordEvidence("queue-pointer","nextSequence="+(_issuedSequence+1)+" device="+_mouse.deviceId+" x="+point.x.ToString("R")+" y="+point.y.ToString("R")+" attack="+attack+" transfer="+transfer);if(_forbidTech && transfer)throw new InvalidOperationException("G6 전이 발행은 금지됩니다.");}
        internal void FinallyCloseEvidence(){var alreadyFailed=TestContext.CurrentContext.Result.Outcome.Status==NUnit.Framework.Interfaces.TestStatus.Failed;try{CloseEvidence();}catch(Exception error){if(alreadyFailed)Debug.LogError("시험 원문 폐쇄 실패(기존 실패와 별도): "+error);else throw new InvalidOperationException("성공 후보의 시험 원문 폐쇄 실패입니다.",error);}}
        private void CloseEvidence()
        {
            if(_evidenceRunId==null)return;
            try
            {
                var directory=Path.GetFullPath(Path.Combine(Application.dataPath,"../../../evidence/expedition-cases",_evidenceRunId));Directory.CreateDirectory(directory);
                using(var stream=new FileStream(Path.Combine(directory,"issued-input-and-lifecycle.synthetic.txt"),FileMode.CreateNew,FileAccess.Write,FileShare.Read))
                {var bytes=new System.Text.UTF8Encoding(false).GetBytes(string.Join("\n",_evidenceRows));stream.Write(bytes,0,bytes.Length);stream.Flush(true);}
            }
            finally{if(_evidenceObject!=null)UnityEngine.Object.Destroy(_evidenceObject);_socketObserver=null;_evidenceObject=null;_evidenceRunId=null;}
        }
        private Keyboard _keyboard;
        private Mouse _mouse;
        private Scene _scene;
        private readonly string[] _visited=new string[4];
        private int _visitedCount;
        internal Chapter1PlayableFlowOwner Flow {get;private set;}
        internal int VisitedCount=>_visitedCount;
        internal Chapter1EncounterEvidenceView BossEntry {get;private set;}
        internal Chapter1EncounterEvidenceView LastRoom {get;private set;}
        internal bool HasBossEntry {get;private set;}
        internal bool SawCommitBeforeRetirement {get;private set;}
        internal void Keyboard(params Key[] keys)
        {if(_keyboard==null)_keyboard=InputSystem.AddDevice<Keyboard>();RecordKeyboard(keys);InputSystem.QueueStateEvent(_keyboard,new KeyboardState(keys));RecordQueueSuccess();InputSystem.Update();}
        internal void Pointer(Vector2 point,bool attack,bool transfer=false)
        {if(_mouse==null)_mouse=InputSystem.AddDevice<Mouse>();var state=new MouseState{position=point};if(attack)state=state.WithButton(MouseButton.Left);if(transfer)state=state.WithButton(MouseButton.Right);RecordPointer(point,attack,transfer);InputSystem.QueueStateEvent(_mouse,state);RecordQueueSuccess();InputSystem.Update();}
        internal IEnumerator Open()
        {
            LogStage("trace.open-before",Flow);LogBoundary("open-before");
            if(_keyboard==null)_keyboard=InputSystem.AddDevice<Keyboard>();if(_mouse==null)_mouse=InputSystem.AddDevice<Mouse>();
#if UNITY_EDITOR
            _scene=EditorSceneManager.LoadSceneInPlayMode("Assets/Scenes/Chapter1Playable.unity",new LoadSceneParameters(LoadSceneMode.Additive));
            LogStage("trace.scene-loaded",Flow);
#else
            yield return SceneManager.LoadSceneAsync("Chapter1Playable",LoadSceneMode.Additive);
            _scene=SceneManager.GetSceneAt(SceneManager.sceneCount-1);
#endif
            for(var i=0;i<60;i++){yield return new WaitForFixedUpdate();foreach(var root in _scene.GetRootGameObjects()){var owner=root.GetComponentInChildren<Chapter1PlayableFlowOwner>(true);if(owner!=null)Flow=owner;}if(i==0||i==59){LogStage("trace.open-fixed-"+i,Flow);LogBoundary("open-fixed-"+i);}if(Flow!=null)yield break;}
            Assert.Fail("정확 저작 scene의 실제 flow owner가 없습니다.");
        }
        internal IEnumerator WaitReady()
        {LogStage("trace.wait-ready-before");LogBoundary("wait-ready-before");for(var i=0;i<120;i++){yield return new WaitForFixedUpdate();if(i==0||i==119)LogBoundary("wait-ready-"+i);Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);
            var hub=Flow.CurrentHub;
            if(hub!=null && ReadReadyMeasured(hub) && (Flow.View.Phase==Chapter1RunPhase.Hub || Flow.View.Phase==Chapter1RunPhase.ReturnedToHub || Flow.View.Phase==Chapter1RunPhase.ExpeditionStorageFailed))
            {MeasurePreparedReadsOnce(hub);LogBoundary("wait-ready-success");yield break;}}
            LogStage("trace.wait-ready-failed");Assert.Fail("실제 prepared·UI 중립 관측에 도달하지 못했습니다.");}
        private int _readyTimingCount;private bool _extraPreparedTiming;
        private bool ReadReadyMeasured(Chapter1HubLifetimeOwner hub)
        {
            if(_readyTimingCount>=8)return hub.Ready;
            var ordinal=++_readyTimingCount;var start=System.Diagnostics.Stopwatch.GetTimestamp();var result=false;string error=null;
            try{result=hub.Ready;return result;}
            catch(Exception exception){error=exception.GetType().Name+":"+exception.Message;throw;}
            finally{var end=System.Diagnostics.Stopwatch.GetTimestamp();LogTiming("ready-normal",ordinal,start,end,"returned="+result+" error="+error);}
        }
        private void MeasurePreparedReadsOnce(Chapter1HubLifetimeOwner hub)
        {
            if(_extraPreparedTiming)return;_extraPreparedTiming=true;
            var start=System.Diagnostics.Stopwatch.GetTimestamp();var receiptEnd=start;var pendingEnd=start;var pending="unobserved";string error=null;
            try{var receipt=hub.Adapter.CurrentReceipt;receiptEnd=System.Diagnostics.Stopwatch.GetTimestamp();if(receipt.HasValue)pending=receipt.Value.PersistencePending.ToString();pendingEnd=System.Diagnostics.Stopwatch.GetTimestamp();}
            catch(Exception exception){error=exception.GetType().Name+":"+exception.Message;throw;}
            finally{LogTiming("prepared-extra-call",1,start,System.Diagnostics.Stopwatch.GetTimestamp(),"receiptEnd="+receiptEnd+" pendingEnd="+pendingEnd+" pending="+pending+" error="+error);}
        }
        private static void LogTiming(string scope,int ordinal,long start,long end,string result)
        {
            if(_stageRows++>=16)return;
            var line="[CH1HubCost] scope="+scope+" ordinal="+ordinal+" frame="+Time.frameCount+" completedTick=unobserved start="+start+" end="+end+" frequency="+System.Diagnostics.Stopwatch.Frequency+" milliseconds="+((end-start)*1000d/System.Diagnostics.Stopwatch.Frequency).ToString("R")+" utc="+DateTime.UtcNow.ToString("O")+" "+result;
            while(System.Text.Encoding.UTF8.GetByteCount(line)>4096)line=line.Substring(0,line.Length/2);Debug.Log(line);
        }
        internal IEnumerator Start(int? seed,bool click=false,bool together=false)
        {
            yield return SendStartEdge(seed,click,together);
            for(var i=0;i<120;i++){yield return new WaitForFixedUpdate();Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);if(Flow.View.Phase==Chapter1RunPhase.InRoom)yield break;}Assert.Fail("실제 UI→공개 저장→첫 완료 view 출정에 도달하지 못했습니다.");
        }
        internal IEnumerator SendStartEdge(int? seed,bool click=false,bool together=false,bool keepHeld=false)
        {
            Keyboard();Pointer(Vector2.zero,false);yield return WaitReady();
            var index=!seed.HasValue?0:seed==101?1:seed==202?2:seed==303?3:4;
            for(var i=0;i<index;i++){Keyboard(Key.DownArrow);yield return new WaitForFixedUpdate();Keyboard();yield return new WaitForFixedUpdate();}
            var scale=Mathf.FloorToInt(Mathf.Min(Screen.width/640f,Screen.height/360f));Assert.That(scale,Is.GreaterThan(0));
            var point=new Vector2(Screen.width/2f,Screen.height/2f+(45-index*30)*scale);
            if(click)Pointer(point,true);if(!click || together)Keyboard(Key.Enter);yield return new WaitForFixedUpdate();
            if(!keepHeld){Keyboard();Pointer(point,false);}
        }
        internal void ReleaseInputsWithSubmitEdge()
        {
            if(_keyboard==null)_keyboard=InputSystem.AddDevice<Keyboard>();if(_mouse==null)_mouse=InputSystem.AddDevice<Mouse>();
            RecordKeyboard(Array.Empty<Key>());InputSystem.QueueStateEvent(_keyboard,new KeyboardState());RecordQueueSuccess();RecordKeyboard(new[]{Key.Enter});InputSystem.QueueStateEvent(_keyboard,new KeyboardState(Key.Enter));RecordQueueSuccess();
            RecordKeyboard(Array.Empty<Key>());InputSystem.QueueStateEvent(_keyboard,new KeyboardState());RecordQueueSuccess();RecordPointer(Vector2.zero,false,false);InputSystem.QueueStateEvent(_mouse,new MouseState{position=Vector2.zero});RecordQueueSuccess();InputSystem.Update();
        }
        internal IEnumerator SendStorageRetryEdge(bool click,bool together)
        {
            Assert.That(Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.ExpeditionStorageFailed));Assert.That(Flow.CurrentHub.Ready,Is.True);
            var scale=Mathf.FloorToInt(Mathf.Min(Screen.width/640f,Screen.height/360f));Assert.That(scale,Is.GreaterThan(0));
            var point=new Vector2(Screen.width/2f,Screen.height/2f+45*scale);
            if(click)Pointer(point,true);if(!click || together)Keyboard(Key.Enter);yield return new WaitForFixedUpdate();
            // 새 Hub가 실제 held control을 관측할 때까지 release하지 않는다.
        }
        internal IEnumerator WaitExpeditionStorageFailed()
        {for(var i=0;i<180;i++){yield return new WaitForFixedUpdate();Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);
            if(Flow.View.Phase==Chapter1RunPhase.ExpeditionStorageFailed)yield break;}Assert.Fail("전용 출정 저장 실패 상태가 180 완료 프레임 안에 관측되지 않았습니다.");}
        internal IEnumerator WaitDiagnostic()
        {for(var i=0;i<180;i++){yield return new WaitForFixedUpdate();if(Flow.View.Phase==Chapter1RunPhase.Diagnostic)yield break;}Assert.Fail("실제 저장 실패 진단이 관측되지 않았습니다.");}
        internal IEnumerator PlayUntilFailureAccepted()
        {
            var deadline=Time.realtimeSinceStartupAsDouble+1800;
            while(Time.realtimeSinceStartupAsDouble<deadline)
            {
                Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);
                if(Flow.AcceptedFailureKind.HasValue){Keyboard();Pointer(Vector2.zero,false);yield break;}
                if(Flow.Lifetime.TryGetCompletedEvidence(out var view) && view.Position.x<6)Keyboard(Key.D);else Keyboard();
                Pointer(Vector2.zero,false);yield return new WaitForFixedUpdate();
            }
            Assert.Fail("실제 health 실패 입력 상한에 도달했습니다.");
        }
        internal IEnumerator WaitReturned()
        {for(var i=0;i<180;i++){yield return new WaitForFixedUpdate();Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);if(Flow.ReturnedToHubRevision==1)yield break;}Assert.Fail("실제 보존 저장 복귀가 완료되지 않았습니다.");}
        internal IEnumerator PlayUntilConcurrentFailure()
        {
            // REQ/AC-AUTOCH1-R05COINCIDENT-001: 준비 후보. 실제 적용·실행·독립 수용 전이다.
            var deadline=Time.realtimeSinceStartupAsDouble+1800;var ordinal=0;var first=-1;var last=-1;var transitionWaits=0;var transitionReleased=false;
            var carry=-1;var previousHP=-1;var preparationApplied=0;var ready=-1;var finalTick=-1;
            var selectedLatch=-1;var selectedFire=-1;var selectedClose=-1;long selectedOrdinal=-1;
            var rows=0;var bytes=0;var dropped=0;var recordFailures=0;var requests=new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            InputRouter bound=null;Transform graph=null;CombatSimulationDriver combat=null;
            RegularEnemyBehaviorSimulationDriver behavior=null;RegularEnemyThreatSimulationDriver threat=null;Chapter1HazardObservation hazard=null;
            var run="";long generation=0;
            var initialWalker=-1;var previousWalker=-1;var walkerApplied=0;var walkerHits=0;var walkerKilledByNormalAttack=false;
            var walkerRequests=new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            var pointerKnown=false;var lastPointerAttack=false;var lastPointerTransfer=false;var pointerTick=-1;long pointerReceipt=-1;long pointerOrdinal=0;
            var keyboardReleaseTick=-1;long keyboardReleaseReceipt=-1;var dashIssued=false;var dashConsumed=false;
            void IssuePointer(Vector2 point,bool attack,bool transfer,int completedTick,long completedReceipt)
            {
                Pointer(point,attack,transfer);
                pointerKnown=true;lastPointerAttack=attack;lastPointerTransfer=transfer;pointerTick=completedTick;pointerReceipt=completedReceipt;pointerOrdinal++;
            }
            void Record(string reason,string raw,bool summary=false)
            {
                try{var line="[CH1R05Coincident] "+reason+" "+raw;
                var truncated=false;while(System.Text.Encoding.UTF8.GetByteCount(line)>1800){line=line.Substring(0,line.Length-1);truncated=true;}if(truncated)dropped++;line+=" truncated="+truncated;
                var size=System.Text.Encoding.UTF8.GetByteCount(line)+1;
                if((!summary && (rows>=31 || bytes+size>65536-1900)) || rows>=32 || size>1900 || bytes+size>65536){dropped++;return;}
                rows++;bytes+=size;Debug.Log(line);}catch(Exception){if(recordFailures<int.MaxValue)recordFailures++;}
            }
            try
            {
                while(Time.realtimeSinceStartupAsDouble<deadline)
                {
                    Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);
                    if(first<0)
                    {
                        Assert.That(Flow.AcceptedFailureKind.HasValue,Is.False,"R05 진입 전에 실제 실패가 발생했습니다.");
                        var phase=Flow.View.Phase;
                        if(phase==Chapter1RunPhase.StartingRoom || phase==Chapter1RunPhase.LeavingRoom)
                        {
                            if(!transitionReleased)
                            {Assert.That(_keyboard,Is.Not.Null);Assert.That(_mouse,Is.Not.Null);Keyboard();Pointer(Vector2.zero,false,false);transitionReleased=true;}
                            transitionWaits++;Assert.That(transitionWaits,Is.LessThan(180),"R05 이전 전이의 누적 fixed 대기 상한입니다.");
                            yield return new WaitForFixedUpdate();continue;
                        }
                        Assert.That(phase,Is.EqualTo(Chapter1RunPhase.InRoom),"예상 밖 단계는 방 진입 대기로 숨기지 않습니다.");transitionReleased=false;
                        if(Flow.View.RoomId!="R05"){PlayInput(ordinal++,true);yield return new WaitForFixedUpdate();continue;}
                    }
                    Assert.That(Flow.Lifetime.TryGetCompletedEvidence(out var view),Is.True,"R05 완료 관측이 없습니다.");
                    if(first<0)
                    {
                        Assert.That(Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.InRoom));Assert.That(Flow.View.RoomId,Is.EqualTo("R05"));
                        bound=Flow.Lifetime.CurrentRouter;Assert.That(bound,Is.Not.Null);graph=bound.transform.parent;Assert.That(graph,Is.Not.Null);
                        var enteredRoom=graph.GetComponent<Chapter1RoomInstance>();Assert.That(enteredRoom,Is.Not.Null);Assert.That(enteredRoom.RoomId,Is.EqualTo("R05"));
                        Assert.That(view.HasCompleted && !view.Lightweight,Is.True);Assert.That(view.RunKey,Is.EqualTo(Flow.View.RunKey));
                        Assert.That(view.Generation,Is.EqualTo(Flow.Lifetime.Generation));Assert.That(view.Generation,Is.EqualTo(Flow.View.Generation));
                        Assert.That(Flow.View.Tick.HasValue,Is.True);Assert.That(view.Tick,Is.EqualTo(Flow.View.Tick.Value));
                        Assert.That(bound.TryGetChapter1CompletedEvidence(out var entrySource),Is.True);
                        Assert.That(entrySource.RunKey,Is.EqualTo(view.RunKey));Assert.That(entrySource.Generation,Is.EqualTo(view.Generation));Assert.That(entrySource.Tick,Is.EqualTo(view.Tick));Assert.That(entrySource.Receipt.Equals(view.Receipt),Is.True);
                        carry=view.Health;previousHP=carry;Assert.That(carry,Is.InRange(1,5));run=view.RunKey;generation=view.Generation;
                        combat=graph.GetComponentInChildren<CombatSimulationDriver>(true);behavior=graph.GetComponentInChildren<RegularEnemyBehaviorSimulationDriver>(true);
                        threat=graph.GetComponentInChildren<RegularEnemyThreatSimulationDriver>(true);hazard=graph.GetComponentInChildren<Chapter1HazardObservation>(true);
                        Assert.That(combat,Is.Not.Null);Assert.That(behavior,Is.Not.Null);Assert.That(threat,Is.Not.Null);Assert.That(hazard,Is.Not.Null);
                        Assert.That(combat.Chapter1ConfiguredCarryTick.HasValue,Is.True);Assert.That(view.Tick,Is.EqualTo(combat.Chapter1ConfiguredCarryTick.Value),"늦은 완료로 R05 첫tick 기준을 재설정하지 않습니다.");
                        first=view.Tick.Value;
                        Record("시작","tick="+first+" carryHP="+carry+" run="+run+" generation="+generation+" actualRoom="+enteredRoom.RoomId+" phase="+Flow.View.Phase+" transitionFixed="+transitionWaits);
                    }
                    Assert.That(view.Tick.Value-first,Is.LessThan(8000));Assert.That(view.RunKey,Is.EqualTo(run));Assert.That(view.Generation,Is.EqualTo(generation));
                    Assert.That(Flow.Lifetime.CurrentRouter,Is.SameAs(bound));Assert.That(view.HasCompleted && !view.Lightweight,Is.True);
                    Assert.That(combat.HasLatestOutcomeForEncounterHandoff,Is.True);var outcome=combat.LatestOutcomeForEncounterHandoff;
                    Assert.That(outcome.Tick,Is.EqualTo(view.Tick));Assert.That(outcome.CanonicalRequestTrace.Count,Is.EqualTo(outcome.DamageResults.Count));
                    var hp=-1;var invulnerability=-1;var walkerHP=-1;var surveyorHP=-1;
                    foreach(var state in outcome.Combatants)
                    {if(state.TargetId=="player"){hp=state.CurrentHealth;invulnerability=state.InvulnerabilityEndTick;}else if(state.TargetId=="walker")walkerHP=state.CurrentHealth;else if(state.TargetId=="surveyor")surveyorHP=state.CurrentHealth;}
                    Assert.That(hp,Is.EqualTo(view.Health));Assert.That(surveyorHP,Is.GreaterThan(0),"측량병을 죽인 경로는 후보가 아닙니다.");
                    if(initialWalker<0){Assert.That(walkerHP,Is.GreaterThan(0));initialWalker=walkerHP;previousWalker=walkerHP;}var damage=0;var finalLine=0;var walkerDamage=0;
                    if(last!=view.Tick.Value)
                    {
                        for(var i=0;i<outcome.CanonicalRequestTrace.Count;i++)
                        {
                            var request=outcome.CanonicalRequestTrace[i];var result=outcome.DamageResults[i];
                            Assert.That(request.RequestId,Is.EqualTo(result.RequestId));Assert.That(request.Tick,Is.EqualTo(view.Tick));Assert.That(result.Tick,Is.EqualTo(view.Tick));Assert.That(result.TargetId,Is.EqualTo(request.TargetId));
                            if(result.Result!=AcadeGameMaker.Combat.DamageResultCode.Applied)continue;
                            Assert.That(request.TargetId,Is.Not.EqualTo("surveyor"));
                            if(request.TargetId=="walker")
                            {
                                Assert.That(request.SourceId,Is.EqualTo("player"));Assert.That(request.Kind,Is.EqualTo(AcadeGameMaker.Combat.DamageKind.BasicAttack));
                                Assert.That(request.Amount,Is.EqualTo(3));Assert.That(result.AppliedAmount,Is.EqualTo(3));Assert.That(result.TargetId,Is.EqualTo("walker"));
                                Assert.That(walkerRequests.Add(request.RequestId),Is.True);walkerDamage+=result.AppliedAmount;walkerHits++;
                                Assert.That(outcome.Attack.Attempt.HasValue,Is.True);Assert.That(outcome.Attack.Attempt.Value.Code,Is.EqualTo(AcadeGameMaker.Combat.BasicAttackAttemptCode.FiredHit));
                                continue;
                            }
                            Assert.That(request.TargetId,Is.EqualTo("player"),"알 수 없는 대상의 피해는 정상 처치 근거가 아닙니다.");
                            Assert.That(request.Amount,Is.EqualTo(1));Assert.That(result.AppliedAmount,Is.EqualTo(1));Assert.That(requests.Add(request.RequestId),Is.True);
                            damage++;
                            if(request.SourceId=="surveyor" && request.Kind==AcadeGameMaker.Combat.DamageKind.EnemyRanged && request.RequestId=="enemy.line/surveyor/"+selectedOrdinal.ToString("D19",System.Globalization.CultureInfo.InvariantCulture)+"/"+(finalTick-1).ToString("D10",System.Globalization.CultureInfo.InvariantCulture))finalLine++;
                        }
                        Assert.That(previousWalker-walkerHP,Is.EqualTo(walkerDamage),"보행병 체력 감소는 같은 완료tick의 실제 정상 공격 피해와 같아야 합니다.");
                        walkerApplied+=walkerDamage;
                        if(previousWalker>0 && walkerHP==0)
                        {Assert.That(walkerDamage,Is.GreaterThan(0));Assert.That(walkerApplied,Is.EqualTo(initialWalker));Assert.That(walkerHits,Is.GreaterThan(0));walkerKilledByNormalAttack=true;Record("실제정상처치","tick="+view.Tick.Value+" baseline="+initialWalker+" applied="+walkerApplied+" uniqueHits="+walkerHits);}
                        previousWalker=walkerHP;
                        Assert.That(previousHP-hp,Is.EqualTo(damage));
                        if(finalTick<0){preparationApplied+=damage;Assert.That(preparationApplied,Is.LessThanOrEqualTo(carry-1));}
                        previousHP=hp;last=view.Tick.Value;
                        if(damage>0)Record("실제피격","tick="+last+" hp="+hp+" applied="+damage+" preparation="+preparationApplied);
                    }
                    if(Flow.AcceptedFailureKind.HasValue)
                    {
                        Assert.That(dashIssued && dashConsumed,Is.True);Assert.That(finalTick,Is.EqualTo(view.Tick.Value));Assert.That(hp,Is.Zero);Assert.That(finalLine,Is.EqualTo(1));Assert.That(damage,Is.EqualTo(1));
                        Assert.That(view.HealthPublication.Triggered,Is.True);Assert.That(view.HealthPublication.Receipt.Equals(view.Receipt),Is.True);
                        Assert.That(hazard.TryGetCompleted(view,out var kill,out var crush),Is.True);Assert.That(crush.Triggered,Is.True);Assert.That(kill.Triggered,Is.False);
                        Assert.That(crush.Receipt.Equals(view.Receipt),Is.True);Assert.That(Flow.AcceptedFailureCauseCount,Is.GreaterThan(1));
                        Assert.That((int)Flow.AcceptedFailureKind.Value,Is.Zero);Assert.That(Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Failing));
                        Record("실제동시원인","tick="+last+" health=true crush=true lineApplied=1");Keyboard();Pointer(Vector2.zero,false);yield break;
                    }
                    Assert.That(finalTick<0 || view.Tick.Value<finalTick,Is.True,"마지막 입력 후 예정 완료tick에서 동시 원인이 없습니다.");
                    Assert.That(Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.InRoom));Assert.That(hp,Is.GreaterThan(0));
                    var b=behavior.LatestCarriedViewForEncounterHandoff;var t=threat.LatestCarriedViewForEncounterHandoff;
                    Assert.That(b.HasValue && t.HasValue,Is.True);Assert.That(b.Value.Tick,Is.EqualTo(view.Tick));Assert.That(t.Value.Tick,Is.EqualTo(view.Tick));
                    var surveyor=b.Value.Snapshot.Surveyor;var line=t.Value.Snapshot;Assert.That(surveyor.IsAlive,Is.True);
                    Assert.That(bound.TryGetChapter1CompletedEvidence(out var sourceCompleted),Is.True);Assert.That(sourceCompleted.Receipt.Equals(view.Receipt),Is.True);
                    var releasedPointerThenNoAttempt=pointerKnown && !lastPointerAttack && !lastPointerTransfer && view.Tick.Value>pointerTick && view.Receipt.FrameOrdinal>pointerReceipt && !outcome.Attack.Attempt.HasValue;
                    if(walkerHP==0)Assert.That(walkerKilledByNormalAttack,Is.True,"다른 원인의 보행병 죽음으로 진행하지 않습니다.");
                    var local=graph.InverseTransformPoint(view.Position);var localMin=graph.InverseTransformPoint(new Vector3(view.PlayerBody.xMin,view.PlayerBody.yMin,0));
                    var localMax=graph.InverseTransformPoint(new Vector3(view.PlayerBody.xMax,view.PlayerBody.yMax,0));var half=(localMax.x-localMin.x)/2f;
                    if(finalTick>=0)
                    {
                        Assert.That(view.Tick.Value,Is.EqualTo(finalTick-1));Assert.That(line.HasActiveLine,Is.True);
                        Assert.That(dashIssued,Is.True);Assert.That(bound.BoundPlayerForAuthoring.TryReadExistingMotionForAuthoring(out var dashMotion,out var dashNext),Is.True);Assert.That(dashNext.Value,Is.EqualTo(view.Tick.Value+1));
                        Assert.That(dashMotion.Tick,Is.EqualTo(view.Tick));Assert.That(dashMotion.ActionState,Is.EqualTo(AcadeGameMaker.Movement.MovementActionState.Dashing));dashConsumed=true;
                        var actualCenter=line.LineCenterXQ4096/4096f;
                        Assert.That(localMax.x,Is.GreaterThan(actualCenter-.25f));Assert.That(localMin.x,Is.LessThan(actualCenter+.25f));
                        Assert.That(localMax.x,Is.GreaterThan(12f));Assert.That(localMin.x,Is.LessThan(14f));Assert.That(localMin.y,Is.LessThan(2f));Assert.That(localMax.y,Is.GreaterThan(0f));
                        Assert.That(hazard.IsClosedAt(view.Tick),Is.False);Assert.That(hp,Is.EqualTo(1));
                        Record("실제접촉후보","tick="+last+" dashActionObserved="+dashConsumed+" sourceReceipt="+view.Receipt.FrameOrdinal+" lineCenter="+actualCenter+" bodyMin="+localMin+" bodyMax="+localMax);
                        Keyboard();keyboardReleaseTick=view.Tick.Value;keyboardReleaseReceipt=view.Receipt.FrameOrdinal;IssuePointer(Vector2.zero,false,false,view.Tick.Value,view.Receipt.FrameOrdinal);yield return new WaitForFixedUpdate();continue;
                    }
                    Assert.That(bound.BoundPlayerForAuthoring.TryReadExistingMotionForAuthoring(out var motion,out var motionNext),Is.True);Assert.That(motion.Tick,Is.EqualTo(view.Tick));Assert.That(motionNext.Value,Is.EqualTo(view.Tick.Value+1));var velocity=motion.VelocityXQ4096/4096f;
                    int Steer(float target)
                    {var delta=target-local.x;var stop=velocity*velocity/(2f*(motion.IsGrounded?90f:20f))+Mathf.Abs(velocity)/60f+.02f;if(Mathf.Abs(delta)<.06f)return velocity>.3f?-1:velocity<-.3f?1:0;return velocity*delta>0 && stop>=Mathf.Abs(delta)?(velocity>0?-1:1):(delta>0?1:-1);}
                    bool Closed(int tick)=>hazard.IsClosedAt(new AcadeGameMaker.Core.SimulationTick(tick));
                    var next=view.Tick.Value+1;var direction=0;var shoot=false;var dash=false;var point=Vector2.zero;
                    if(walkerHP>0)
                    {
                        Assert.That(view.Tick.Value-first,Is.LessThan(2500));
                        var walker=graph.Find("Walker");var camera=graph.GetComponentInChildren<UnityEngine.Camera>(true);Assert.That(walker,Is.Not.Null);Assert.That(camera,Is.Not.Null);
                        var wx=graph.InverseTransformPoint(walker.position).x;direction=Steer(Mathf.Min(9f,wx-2.2f));point=camera.WorldToScreenPoint(walker.position);
                        shoot=releasedPointerThenNoAttempt && outcome.Attack.Selection.TargetId=="walker" && outcome.Attack.Snapshot.CooldownEndTick<=next && camera.pixelRect.Contains(point);
                    }
                    else if(hp>1)
                    {
                        Assert.That(view.Tick.Value-first,Is.LessThan(2500));direction=Steer(8.8f);
                    }
                    else
                    {
                        Assert.That(preparationApplied,Is.EqualTo(carry-1));
                        if(ready<0){ready=view.Tick.Value;Record("체력준비완료","tick="+ready+" hp=1 actualApplied="+preparationApplied);}
                        Assert.That(view.Tick.Value-ready,Is.LessThan(4448));
                        // 다음 예고시점과 순수 압착 조회로 넓은 창을 정한다. 좌표는 입력 목표일 뿐 성공 증거가 아니다.
                        var nextLatch=surveyor.Phase==AcadeGameMaker.Combat.SurveyorBehaviorPhase.Relocate?surveyor.ExclusiveEndTick:
                            surveyor.Phase==AcadeGameMaker.Combat.SurveyorBehaviorPhase.Cooldown?surveyor.ExclusiveEndTick+48:-1;
                        if(selectedLatch<0 && !line.HasActiveLine && nextLatch>view.Tick.Value && nextLatch-view.Tick.Value<=90)
                        {
                            var close=nextLatch;while(close<nextLatch+180 && !Closed(close))close++;
                            var latchPhase=60-(close-nextLatch);
                            if(latchPhase>=15 && latchPhase<=29){selectedLatch=nextLatch;selectedFire=nextLatch+30;selectedClose=close;selectedOrdinal=surveyor.ShotOrdinal;Record("창선택","latch="+selectedLatch+" fire="+selectedFire+" close="+selectedClose+" ordinal="+selectedOrdinal);}
                        }
                        var target=8.8f;
                        if(selectedLatch>=0)
                        {
                            Assert.That(view.Tick.Value-selectedLatch,Is.LessThan(180));
                            if(view.Tick.Value<selectedLatch)
                            {
                                // 이전 닫힘이 끝나기 전에는 판 안으로 접근하지 않는다.
                                target=Closed(next)?11.3f:12.2f;
                            }
                            else
                            {
                                if(view.Tick.Value==selectedLatch)
                                {Assert.That(surveyor.HasLatchedTarget,Is.True);Assert.That(surveyor.ShotOrdinal,Is.EqualTo(selectedOrdinal));Assert.That(Mathf.Abs(surveyor.LatchedTargetXQ1000/1000f-local.x),Is.LessThan(.05f));Assert.That(surveyor.LatchedTargetXQ1000/1000f,Is.InRange(12.05f,12.35f));Record("실제예고","tick="+last+" latchedX="+surveyor.LatchedTargetXQ1000);}
                                target=11.35f;
                                if(next==selectedClose-1)
                                {
                                    Assert.That(hp,Is.EqualTo(1));Assert.That(invulnerability,Is.LessThanOrEqualTo(selectedClose));Assert.That(!Closed(next) && Closed(selectedClose),Is.True);
                                    Assert.That(line.HasActiveLine,Is.True);Assert.That(line.LineShotOrdinal,Is.EqualTo(selectedOrdinal));Assert.That(line.LineSpawnTick,Is.EqualTo(selectedFire));Assert.That(line.LineExclusiveEndTick,Is.GreaterThan(next));
                                    var center=line.LineCenterXQ4096/4096f;Assert.That(center,Is.InRange(12.05f,12.35f));
                                    Assert.That(localMax.x,Is.LessThan(center-.25f));Assert.That(localMax.x,Is.LessThanOrEqualTo(12f));Assert.That(local.x,Is.InRange(11.25f,11.45f));
                                    Assert.That(releasedPointerThenNoAttempt,Is.True);
                                    Assert.That(keyboardReleaseTick>=first && view.Tick.Value>keyboardReleaseTick && view.Receipt.FrameOrdinal>keyboardReleaseReceipt,Is.True);
                                    Assert.That(motion.Tick,Is.EqualTo(view.Tick));Assert.That(motion.ActionState,Is.Not.EqualTo(AcadeGameMaker.Movement.MovementActionState.Dashing));Assert.That(dashIssued,Is.False);
                                    dash=true;direction=1;finalTick=selectedClose;Record("마지막입력발행계획","source="+last+" contactCandidate="+next+" expected="+finalTick+" center="+center+" half="+half+" queuedCountAux="+combat.ThreatDeliveryCountForTests);
                                }
                            }
                        }
                        else if(surveyor.HasLatchedTarget){var cx=surveyor.LatchedTargetXQ1000/1000f;target=cx<9.5f?Mathf.Min(10.9f,cx+half+.65f):Mathf.Max(7.5f,cx-half-.65f);Assert.That(Mathf.Abs(target-cx),Is.GreaterThan(half+.25f));}
                        else if(line.HasActiveLine){var cx=line.LineCenterXQ4096/4096f;target=cx<9.5f?Mathf.Min(10.9f,cx+half+.65f):Mathf.Max(7.5f,cx-half-.65f);Assert.That(Mathf.Abs(target-cx),Is.GreaterThan(half+.25f));}
                        if(!dash)direction=Steer(target);
                    }
                    if(dash){Keyboard(Key.D,Key.LeftShift);dashIssued=true;Record("대시발행반환","source="+view.Tick.Value+" expected="+finalTick+" consumed=false");}
                    else{if(direction>0)Keyboard(Key.D);else if(direction<0)Keyboard(Key.A);else Keyboard();keyboardReleaseTick=view.Tick.Value;keyboardReleaseReceipt=view.Receipt.FrameOrdinal;}
                    IssuePointer(point,shoot,false,view.Tick.Value,view.Receipt.FrameOrdinal);yield return new WaitForFixedUpdate();
                }
                Assert.Fail("체력·압착 동시원인 후보의 실제시간 상한입니다.");
            }
            finally{try{Record("최종요약","first="+first+" last="+last+" carry="+carry+" actualPreparation="+preparationApplied+" expected="+finalTick+" walkerApplied="+walkerApplied+" walkerHits="+walkerHits+" walkerNormalKill="+walkerKilledByNormalAttack+" pointerIssued="+pointerOrdinal+" dashIssued="+dashIssued+" dashActionObserved="+dashConsumed+" rowsBefore="+rows+" bytesBefore="+bytes+" dropped="+dropped+" recordFailuresBefore="+recordFailures,true);}catch(Exception){if(recordFailures<int.MaxValue)recordFailures++;}}
        }
        internal IEnumerator PlayUntilBossAndPlayerDeath()
        {
            // REQ/AC-AUTOCH1-BOSSCOINCIDENT-001. Normal input only; no production state or damage injection.
            var deadline=Time.realtimeSinceStartupAsDouble+1800;
            Assert.That(HasBossEntry,Is.True);var firstTick=BossEntry.Tick.Value;
            var owner=Flow.Lifetime;var router=owner.CurrentRouter;var graph=router.transform.parent;var generation=owner.Generation;var run=Flow.View.RunKey;
            var player=router.BoundPlayerForAuthoring;var transfer=router.BoundTransferForAuthoring;
            var combat=graph.GetComponentInChildren<OrdanBossCombatSimulationDriver>(true);var hostile=graph.GetComponentInChildren<OrdanBossHostileDamageProducer>(true);
            var bridge=graph.GetComponentInChildren<OrdanBossSimulationDriver>(true);var boss=graph.Find("Ordan");var camera=graph.GetComponentInChildren<UnityEngine.Camera>(true);var bossBody=boss.GetComponent<Collider2D>();
            Assert.That(BossEntry.Health,Is.InRange(1,5));Assert.That(combat,Is.Not.Null);Assert.That(hostile,Is.Not.Null);Assert.That(bridge,Is.Not.Null);Assert.That(camera,Is.Not.Null);
            Assert.That(combat.BoundPlayer,Is.SameAs(player));Assert.That(ReferenceEquals(combat.BoundTransferDriver,transfer),Is.True);Assert.That(hostile.BoundCombat,Is.SameAs(combat));Assert.That(hostile.BoundBridge,Is.SameAs(bridge));Assert.That(hostile.BoundPlayer,Is.SameAs(player));Assert.That(ReferenceEquals(bridge.BoundTransferDriver,transfer),Is.True);
            var requests=new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            var stage=0;var previousBoss=60;var previousPlayer=BossEntry.Health;var attacks=0;var lastTick=-1;var jumpStarted=false;var rows=0;var bytes=0;var dropped=0;var finalTick=-1;string finalHostile=null;var lastHealth=BossEntry.Health;
            void Record(string marker,string raw,bool summary=false)
            {
                try{var line="[CH1BossCoincident] stage="+marker+" "+raw;var truncated=false;while(System.Text.Encoding.UTF8.GetByteCount(line)>1800){line=line.Substring(0,line.Length-1);truncated=true;}line+=" truncated="+truncated;var length=System.Text.Encoding.UTF8.GetByteCount(line)+1;
                    if(!summary && (rows>=31||bytes+length>65536-1900)){dropped++;return;}if(rows>=32||length>1900||bytes+length>65536){dropped++;return;}rows++;bytes+=length;Debug.Log(line);
                }catch{dropped++;}
            }
            // REQ/AC-AUTOCH1-BOSSCOINCIDENT-001: issued values are not proof of physical neutrality or consumption.
            var pointerOrdinal=0;var lastPointerTick=-1;var lastPointerAttack=false;var lastPointerTransfer=false;var lastReleaseOrdinal=0;
            var gateRecorded=false;var firstEdgeRecorded=false;var firstResultRecorded=false;var firstEdgeExpectedTick=-1;var healthChanges=0;
            Record("entry","firstTick="+firstTick+" carryHP="+BossEntry.Health+" carryInvulnerabilityEnd="+BossEntry.InvulnerabilityEndTick+" run="+run+" generation="+generation);
            try
            {
                while(Time.realtimeSinceStartupAsDouble<deadline)
                {
                    Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);
                    Assert.That(owner.CurrentRouter,Is.SameAs(router));Assert.That(owner.Generation,Is.EqualTo(generation));Assert.That(Flow.View.RunKey,Is.EqualTo(run));Assert.That(router.transform.parent,Is.SameAs(graph));
                    Assert.That(owner.TryGetCompletedEvidence(out var view),Is.True);Assert.That(view.RunKey,Is.EqualTo(run));Assert.That(view.Generation,Is.EqualTo(generation));Assert.That(view.Tick.Value-firstTick,Is.InRange(0,5999));
                    Assert.That(combat.HasLatestOutcome && hostile.HasLatestOutcome && bridge.HasLatestOutcome,Is.True);
                    var outcome=combat.LatestOutcome;var danger=hostile.LatestOutcome;var bridgeView=bridge.LatestOutcome;
                    Assert.That(outcome.Tick,Is.EqualTo(view.Tick));Assert.That(danger.Tick,Is.EqualTo(view.Tick));Assert.That(bridgeView.SourceTick,Is.EqualTo(view.Tick));
                    Assert.That(router.CurrentReceipt.HasValue && combat.CurrentCoordinatedReceipt.HasValue,Is.True);Assert.That(router.CurrentReceipt.Value,Is.EqualTo(view.Receipt));Assert.That(combat.CurrentCoordinatedReceipt.Value,Is.EqualTo(view.Receipt));
                    Assert.That(player.TryReadExistingMotionForAuthoring(out var motion,out var next),Is.True);Assert.That(motion.Tick,Is.EqualTo(view.Tick));Assert.That(next.Value,Is.EqualTo(view.Tick.Value+1));
                    var bossHP=-1;var playerHP=-1;var invulnerability=-1;
                    foreach(var state in outcome.Combatants){if(state.TargetId=="ordan")bossHP=state.CurrentHealth;else if(state.TargetId=="player"){playerHP=state.CurrentHealth;invulnerability=state.InvulnerabilityEndTick;}}
                    Assert.That(playerHP,Is.EqualTo(view.Health));Assert.That(playerHP,Is.InRange(0,BossEntry.Health));
                    var raw="tick="+view.Tick.Value+" next="+next.Value+" receiptOrdinal="+view.Receipt.FrameOrdinal+" epoch="+view.Receipt.ModeEpoch+" mode="+view.Receipt.Mode+" bossHP="+bossHP+" playerHP="+playerHP+" invulnerability="+invulnerability+" cooldown="+outcome.Attack.Snapshot.CooldownEndTick+" selection="+outcome.Attack.Selection.TargetId+" position="+view.Position.ToString("R")+" priorIssuedThrough="+_issuedSequence;
                    if(firstEdgeRecorded&&!firstResultRecorded&&view.Tick.Value>=firstEdgeExpectedTick)
                    {
                        firstResultRecorded=true;var detail=raw+" expectedAfterIssuedEdge="+firstEdgeExpectedTick+" attempt="+(outcome.Attack.Attempt.HasValue?outcome.Attack.Attempt.Value.Code.ToString():"none");
                        foreach(var request in outcome.CanonicalRequestTrace)detail+=" request="+request.RequestId+":"+request.TargetId+":"+request.Amount;
                        foreach(var result in outcome.DamageResults)detail+=" result="+result.RequestId+":"+result.TargetId+":"+result.Result+":"+result.AppliedAmount;
                        Record("first-preparation-edge-next-actual-result",detail);
                    }
                    if(Flow.AcceptedFailureKind.HasValue)
                    {
                        Assert.That(finalTick,Is.EqualTo(view.Tick.Value),"마지막 입력이 없는 다른 실제 실패는 동시 사망 성공이 아닙니다.");
                        Assert.That(outcome.CanonicalRequestTrace.Count,Is.EqualTo(2));Assert.That(outcome.DamageResults.Count,Is.EqualTo(2));var appliedBoss=0;var appliedPlayer=0;
                        foreach(var request in outcome.CanonicalRequestTrace)
                        {
                            Assert.That(request.Tick,Is.EqualTo(view.Tick));
                            if(request.TargetId=="player"){Assert.That(request.RequestId,Is.EqualTo(finalHostile));Assert.That(request.Amount,Is.EqualTo(1));Assert.That(request.SourceId,Is.EqualTo("ordan"));}
                            else{Assert.That(request.TargetId,Is.EqualTo("ordan"));Assert.That(request.Amount,Is.EqualTo(3));Assert.That(request.SourceId,Is.EqualTo("player"));Assert.That(request.Kind,Is.EqualTo(AcadeGameMaker.Combat.DamageKind.BasicAttack));}
                        }
                        foreach(var result in outcome.DamageResults){Assert.That(result.Result,Is.EqualTo(AcadeGameMaker.Combat.DamageResultCode.Applied));if(result.TargetId=="ordan"){Assert.That(result.AppliedAmount,Is.EqualTo(3));appliedBoss++;}else if(result.TargetId=="player"){Assert.That(result.AppliedAmount,Is.EqualTo(1));appliedPlayer++;}}
                        Assert.That(appliedBoss,Is.EqualTo(1));Assert.That(appliedPlayer,Is.EqualTo(1));Assert.That(bossHP,Is.Zero);Assert.That(playerHP,Is.Zero);
                        Assert.That(outcome.Attack.Attempt.HasValue,Is.True);Assert.That(outcome.Attack.Attempt.Value.Code,Is.EqualTo(AcadeGameMaker.Combat.BasicAttackAttemptCode.FiredHit));
                        Assert.That(owner.TryGetChapter1BossPublication(out var death),Is.True,"동일 tick 보스 사망이 없는 player 실패는 이 AC 성공이 아닙니다.");
                        Assert.That(death.PlayerDead && death.BossDead,Is.True);Assert.That(Flow.View.Tick.Value,Is.EqualTo(death.DeathTick));
                        Assert.That(death.DeathTick,Is.EqualTo(view.Tick));Assert.That(view.HealthPublication.Triggered,Is.True);Assert.That(Flow.BlocksNewFailureRequests,Is.False);Assert.That(Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Failing));
                        Record("actual-both-applied",raw+" hostile="+finalHostile+" canonicalRequests=2 actualApplied=2");Keyboard();Pointer(Vector2.zero,false);yield break;
                    }
                    Assert.That(Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.AtBoss),"동시 사망이 없는 보스 성공은 이 AC 성공이 아닙니다.");
                    Assert.That(finalTick<0,Is.True,"마지막 입력 다음 완료 tick에 실제 양쪽 사망이 없습니다.");Assert.That(playerHP,Is.GreaterThan(0));
                    if(view.Tick.Value!=lastTick)
                    {
                        var bossDamage=0;var uniqueRequest="";
                        for(var i=0;i<outcome.CanonicalRequestTrace.Count;i++)
                        {
                            var request=outcome.CanonicalRequestTrace[i];var result=outcome.DamageResults[i];
                            if(request.TargetId=="ordan" && result.Result==AcadeGameMaker.Combat.DamageResultCode.Applied)
                            {Assert.That(stage,Is.EqualTo(1));Assert.That(request.Kind,Is.EqualTo(AcadeGameMaker.Combat.DamageKind.BasicAttack));Assert.That(request.SourceId,Is.EqualTo("player"));Assert.That(result.AppliedAmount,Is.EqualTo(3));Assert.That(requests.Add(request.RequestId),Is.True);bossDamage+=result.AppliedAmount;uniqueRequest=request.RequestId;}
                        }
                        Assert.That(previousBoss-bossHP,Is.EqualTo(bossDamage));
                        if(bossDamage>0){Assert.That(bossDamage,Is.EqualTo(3));Assert.That(outcome.Attack.Attempt.HasValue,Is.True);Assert.That(outcome.Attack.Attempt.Value.Code,Is.EqualTo(AcadeGameMaker.Combat.BasicAttackAttemptCode.FiredHit));attacks++;Assert.That(attacks,Is.LessThanOrEqualTo(19));Assert.That(bossHP,Is.EqualTo(60-3*attacks));Record("actual-preparation-hit",raw+" uniqueRequest="+uniqueRequest+" appliedAmount=3 count="+attacks);}
                        if(playerHP!=lastHealth){healthChanges++;if(healthChanges==1)Record("actual-player-health-change",raw+" previousHP="+lastHealth+" laterChangesAggregatedInSummary=true");lastHealth=playerHP;}
                        previousBoss=bossHP;previousPlayer=playerHP;lastTick=view.Tick.Value;
                    }
                    var point=(Vector2)camera.WorldToScreenPoint(boss.position);var x=view.Position.x;var velocity=motion.VelocityXQ4096/4096f;var shoot=false;var jump=false;var direction=0;
                    var releaseIssuedBeforeNewCompletion=lastReleaseOrdinal>0 && !lastPointerAttack && !lastPointerTransfer && lastPointerTick>=0 && view.Tick.Value>lastPointerTick && !outcome.Attack.Attempt.HasValue;
                    if(stage==1&&!gateRecorded)
                    {
                        gateRecorded=true;Record("first-preparation-gate",raw+" point="+point.ToString("R")+" cameraRect="+camera.pixelRect+" inside="+camera.pixelRect.Contains(point)+" referenceMouseHeld="+(_mouse!=null&&_mouse.leftButton.isPressed)+" noAttempt="+(!outcome.Attack.Attempt.HasValue)+" lastIssuedAttack="+lastPointerAttack+" lastIssuedTransfer="+lastPointerTransfer+" lastIssuedTick="+lastPointerTick+" lastReleaseOrdinal="+lastReleaseOrdinal+" eligibleIssuedHistory="+releaseIssuedBeforeNewCompletion+" physicalNeutral=unobserved consumption=unobserved");
                    }
                    int Steer(float target)
                    {var delta=target-x;var deceleration=motion.IsGrounded?90f:20f;var stop=velocity*velocity/(2f*deceleration)+Mathf.Abs(velocity)/60f+.02f;if(Mathf.Abs(delta)<.12f)return velocity>.5f?-1:velocity<-.5f?1:0;return velocity*delta>0&&stop>=Mathf.Abs(delta)?(velocity>0?-1:1):(delta>0?1:-1);}
                    if(stage==0)
                    {
                        if(!jumpStarted && motion.IsGrounded && view.PlayerBody.xMax>=bossBody.bounds.min.x-.4f)jumpStarted=true;
                        jump=jumpStarted&&view.PlayerBody.xMin<=bossBody.bounds.max.x+.02f;direction=view.PlayerBody.xMin<=bossBody.bounds.max.x+.02f?1:Steer(8.75f);
                        if(x>=8.5f&&x<=9&&motion.IsGrounded&&Mathf.Abs(velocity)<=.5f){stage=1;Record("safe-preparation-observed",raw);}
                    }
                    else if(stage==1)
                    {
                        direction=Steer(8.75f);
                        if(attacks==19)
                        {
                            Assert.That(bossHP,Is.EqualTo(3));var pending=combat.ReadPendingDeliveryForTests(next);Assert.That(pending.SourceTick,Is.EqualTo(view.Tick));Assert.That(pending.DeliveryTick,Is.EqualTo(next));Assert.That(pending.Request.HasValue,Is.False,"추가 payload 피해가 준비돼 있습니다.");
                            stage=2;jumpStarted=false;Record("boss-three-release",raw+" payloadPending=false");
                        }
                        else shoot=releaseIssuedBeforeNewCompletion && outcome.Attack.Selection.TargetId=="ordan" && outcome.Attack.Snapshot.CooldownEndTick<=next.Value && camera.pixelRect.Contains(point);
                    }
                    else if(stage==2)
                    {
                        if(!jumpStarted&&motion.IsGrounded&&view.PlayerBody.xMin<=bossBody.bounds.max.x+.4f)jumpStarted=true;
                        jump=jumpStarted&&view.PlayerBody.xMax>=bossBody.bounds.min.x-.02f;direction=view.PlayerBody.xMax>=bossBody.bounds.min.x-.02f?-1:Steer(5.2f);
                        if(x>=5&&x<=5.4f&&motion.IsGrounded&&Mathf.Abs(velocity)<=.5f){stage=3;Record("normal-hostile-health-preparation",raw);}
                    }
                    else
                    {
                        direction=Steer(5.2f);
                        if(playerHP==1 && danger.Digest.Disposition==OrdanBossHostileDisposition.Queued && danger.Digest.DeliveryTick.HasValue && danger.Digest.DeliveryTick.Value==next &&
                            invulnerability<=next.Value && outcome.Attack.Snapshot.CooldownEndTick<=next.Value && outcome.Attack.Selection.TargetId=="ordan" && releaseIssuedBeforeNewCompletion && camera.pixelRect.Contains(point))
                        {
                            var batch=combat.ReadPendingDeliveryForTests(next);Assert.That(batch.SourceTick,Is.EqualTo(view.Tick));Assert.That(batch.DeliveryTick,Is.EqualTo(next));Assert.That(batch.Request.HasValue,Is.False);Assert.That(batch.HostileRequests.Count,Is.EqualTo(1));var request=batch.HostileRequests[0];
                            Assert.That(request.RequestId,Is.EqualTo(danger.Digest.RequestId));Assert.That(request.Tick,Is.EqualTo(next));Assert.That(request.SourceId,Is.EqualTo("ordan"));Assert.That(request.TargetId,Is.EqualTo("player"));Assert.That(request.Amount,Is.EqualTo(1));Assert.That(request.Kind,Is.EqualTo(AcadeGameMaker.Combat.DamageKind.BossPattern));
                            Assert.That(bossHP,Is.EqualTo(3));Assert.That(attacks,Is.EqualTo(19));finalTick=next.Value;finalHostile=request.RequestId;shoot=true;direction=0;Record("actual-queued-before-final-edge",raw+" pendingRequest="+finalHostile+" pendingDelivery="+finalTick+" payloadPending=false");
                        }
                    }
                    if(direction>0)Keyboard(jump?new[]{Key.D,Key.Space}:new[]{Key.D});else if(direction<0)Keyboard(jump?new[]{Key.A,Key.Space}:new[]{Key.A});else Keyboard(jump?new[]{Key.Space}:Array.Empty<Key>());
                    Pointer(point,shoot,false);
                    pointerOrdinal++;lastPointerTick=view.Tick.Value;lastPointerAttack=shoot;lastPointerTransfer=false;if(!shoot)lastReleaseOrdinal=pointerOrdinal;
                    if(stage==1&&shoot&&!firstEdgeRecorded){firstEdgeRecorded=true;firstEdgeExpectedTick=next.Value;Record("first-preparation-edge-issued",raw+" pointerOrdinal="+pointerOrdinal+" expectedNext="+next.Value+" attack=true transfer=false consumption=unobserved");}
                    if(finalTick>=0)Record("final-normal-edge-issued-consumption-unobserved","next="+finalTick+" actualIssuedThrough="+_issuedSequence+" pointer="+point.ToString("R")+" attack=true transfer=false");
                    yield return new WaitForFixedUpdate();
                }
                Assert.Fail("실제 보스/player 동시 사망 입력 상한에 도달했습니다.");
            }
            finally{Record("finally-summary","rowsBefore="+rows+" bytesBefore="+bytes+" dropped="+dropped+" limitExceeded="+(dropped>0)+" actualPreparationApplied="+attacks+" finalExpectedTick="+finalTick+" firstTick="+firstTick+" lastCompleted="+lastTick+" healthChanges="+healthChanges+" lastObservedHP="+lastHealth+" lastPointerOrdinal="+pointerOrdinal+" firstGateObserved="+gateRecorded+" firstEdgeIssued="+firstEdgeRecorded+" firstResultObserved="+firstResultRecorded,true);}
        }

        internal IEnumerator WalkToUnclearedExit()
        {
            for(var i=0;i<900;i++)
            {
                Assert.That(Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.InRoom),"공격 없는 실제 출구 진입 fixture가 실패했습니다.");
                if(Flow.Lifetime.TryGetCompletedEvidence(out var view) && view.Position.x>=30 && view.Position.y>=0 && view.Position.y<3)
                {Keyboard();Pointer(Vector2.zero,false);yield break;}
                Keyboard(i%60<3?new[]{Key.D,Key.LeftShift}:i%30<3?new[]{Key.D,Key.Space}:new[]{Key.D});
                Pointer(Vector2.zero,false);yield return new WaitForFixedUpdate();
            }
            Assert.Fail("실제 미정리 출구 관측 900틱 상한에 도달했습니다.");
        }
        internal IEnumerator PlayToBoss()
        {
            var deadline=Time.realtimeSinceStartupAsDouble+1800;var ordinal=0;
            while(Time.realtimeSinceStartupAsDouble<deadline)
            {
                Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);
                if(Flow.View.Phase==Chapter1RunPhase.AtBoss)
                {Assert.That(Flow.Lifetime.TryGetCompletedEvidence(out var firstBoss),Is.True);BossEntry=firstBoss;LastRoom=Flow.Lifetime.PreviousCompletedEvidence;HasBossEntry=true;Keyboard();Pointer(Vector2.zero,false);yield break;}
                if(Flow.View.RoomId=="R06" && Flow.Lifetime.TryGetCompletedEvidence(out var lastRoom))LastRoom=lastRoom;
                Assert.That(Flow.ReturnedToHubRevision,Is.Zero,"실제 입력 정상 원정이 실패했습니다. 자동 건너뛰기는 하지 않습니다.");
                RecordRoom();PlayInput(ordinal++,true);yield return new WaitForFixedUpdate();
            }
            Assert.Fail("정상 원정 30분 상한에 도달했습니다. 미완료입니다.");
        }
        internal IEnumerator PlayToFailure(int cause)
        {
            var r02=R02ProgressionDiagnostic.Open(this,cause);
            try
            {
            var deadline=Time.realtimeSinceStartupAsDouble+1800;var ordinal=0;var sawAccepted=false;var lastDiagnosticRoom="";var negativeRooms=new System.Collections.Generic.HashSet<string>();var approachRooms=new System.Collections.Generic.HashSet<string>();_diagnosticCause=cause;LogBoundary("failure-begin expected="+cause);
            while(Time.realtimeSinceStartupAsDouble<deadline)
            {
                r02?.Observe(ordinal);
                Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);
                if(Flow.RunEndCommittedRevision>0 && !Flow.Lifetime.Chapter1GraphRetired)SawCommitBeforeRetirement=true;
                if(Flow.AcceptedFailureKind.HasValue){LogBoundary("failure-accepted expected="+cause+" actual="+Flow.AcceptedFailureKind.Value,true);Assert.That((int)Flow.AcceptedFailureKind.Value,Is.EqualTo(cause),"다른 실제 원인으로 실패했습니다.");sawAccepted=true;}
                if(Flow.View.Phase==Chapter1RunPhase.ReturnedToHub){Assert.That(sawAccepted,Is.True,"실제 accepted failure 관측이 없습니다.");Keyboard();Pointer(Vector2.zero,false);yield break;}
                if(r02!=null && r02.StopAfterBoundary && !Flow.AcceptedFailureKind.HasValue)Assert.Inconclusive("R02 x32 이후 두 완료 표본을 확보한 진단 미완료입니다. G7 통과가 아닙니다.");
                var owner=Flow.Lifetime;
                if(cause==0){if(owner.TryGetCompletedEvidence(out var healthView) && healthView.Position.x<6)Keyboard(Key.D);else Keyboard();Pointer(Vector2.zero,false);} // 실제 적에 접근하고 공격을 받아 HP0을 발생시킨다.
                else if(owner.TryGetCompletedEvidence(out var view))
                {
                    var targetRoom=cause==1?"R04":"R05";if(lastDiagnosticRoom!=Flow.View.RoomId){lastDiagnosticRoom=Flow.View.RoomId;LogBoundary("failure-room-entry expected="+cause);}
                    if(view.Position.y<0 && negativeRooms.Add(Flow.View.RoomId))LogBoundary("failure-first-below-floor expected="+cause);
                    if((Flow.View.RoomId=="R04" || Flow.View.RoomId=="R05") && view.Position.x>=10 && approachRooms.Add(Flow.View.RoomId))LogBoundary("failure-hazard-approach expected="+cause);
                    if(Flow.View.RoomId==targetRoom && view.Position.x>=12.8f && view.Position.x<=13.5f){Keyboard();Pointer(Vector2.zero,false);}
                    else PlayInput(ordinal++,Flow.View.RoomId!=targetRoom); // 낙하/압착 방에서는 점프 없이 실제 바닥 경로로 이동한다.
                }
                yield return new WaitForFixedUpdate();
            }
            Assert.Fail("실제 실패→저장→복귀 30분 상한에 도달했습니다. 미완료입니다.");
            }
            finally{r02?.Close();}
        }
        // REQ-AUTOCH1-R02DIAG-001: explicit diagnostic only; no input or producer calls.
        private sealed class R02ProgressionDiagnostic
        {
            private readonly Chapter1AuthoredInputTrace _trace;
            private readonly string _path;
            private readonly System.Collections.Generic.List<string> _rows=new System.Collections.Generic.List<string>();
            private readonly System.Collections.Generic.Queue<string> _before=new System.Collections.Generic.Queue<string>();
            private readonly bool[] _crossed=new bool[3];private readonly int[] _after=new int[3];
            private string _lastKey,_transition;private int _transitionRows,_dropped,_bytes,_duplicateReasons,_after32,_r02EntryRemaining=4;
            private bool _zero,_below,_failed,_past32,_closed,_readError;
            internal bool StopAfterBoundary=>_past32 && _after32>=2;
            private R02ProgressionDiagnostic(Chapter1AuthoredInputTrace trace,string path){_trace=trace;_path=path;Add("begin","runId="+trace._evidenceRunId+" source=synthetic explicit=true");}
            internal static R02ProgressionDiagnostic Open(Chapter1AuthoredInputTrace trace,int cause)
            {
                var args=Environment.GetCommandLineArgs();string root=null;
                for(var i=0;i<args.Length;i++)if(args[i]=="-autonomousR02DiagnosticRoot")
                {if(root!=null || i+1>=args.Length)throw new InvalidOperationException("R02 진단 인자가 중복되거나 비었습니다.");root=args[++i];}
                if(root==null)return null;
                if(cause!=2 || TestContext.CurrentContext.Test.Name!="G7_Crush" || string.IsNullOrEmpty(trace._evidenceRunId))throw new InvalidOperationException("R02 진단은 명시 G7_Crush 한 사례 전용입니다.");
                root=Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);
                var parent=Path.GetFullPath(Path.Combine(Application.dataPath,"../../../chapter1-autonomous-20261021/evidence")).TrimEnd(Path.DirectorySeparatorChar);
                if(!string.Equals(Path.GetDirectoryName(root),parent,StringComparison.OrdinalIgnoreCase) || !Directory.Exists(root))throw new InvalidOperationException("R02 진단 실행 디렉터리 경계가 다릅니다.");
                for(var d=new DirectoryInfo(root);d!=null;d=d.Parent)if((d.Attributes&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("R02 진단 경로에 링크가 있습니다.");
                var path=Path.Combine(root,"r02-progression-diagnostic.v1.jsonl");if(File.Exists(path)||Directory.Exists(path))throw new InvalidOperationException("R02 진단 원문이 이미 있습니다.");
                return new R02ProgressionDiagnostic(trace,path);
            }
            private static string Q(string text)=>"\""+(text??"").Replace("\\","\\\\").Replace("\"","\\\"").Replace("\r","\\r").Replace("\n","\\n").Replace("\t","\\t")+"\"";
            private void Add(string reason,string raw)
            {
                var line="{\"reason\":"+Q(reason)+",\"raw\":"+Q(raw)+",\"truncated\":false}";
                var truncated=false;while(System.Text.Encoding.UTF8.GetByteCount(line)+1>1900 && raw.Length>0){raw=raw.Substring(0,raw.Length-1);truncated=true;line="{\"reason\":"+Q(reason)+",\"raw\":"+Q(raw)+",\"truncated\":true}";}
                if(truncated)_dropped++;
                var bytes=System.Text.Encoding.UTF8.GetByteCount(line)+1;
                if(_rows.Count>=31 || _bytes+bytes>62000){_dropped++;return;}
                _rows.Add(line);_bytes+=bytes;
            }
            internal void Observe(int ordinal)
            {
                try
                {
                    var flow=_trace.Flow;var phase=flow.View.Phase;var owner=flow.Lifetime;var router=owner.CurrentRouter;
                    var room=router==null?null:router.transform.parent.GetComponent<Chapter1RoomInstance>();
                    var actualRoom=room==null?"unobserved":room.RoomId;
                    var pilotRouter=_trace._roomPilot==null?null:_trace._roomPilot.Router;
                    var identity="phase="+phase+" declaredRoom="+flow.View.RoomId+" actualRoom="+actualRoom+" generation="+owner.Generation+" router="+(router==null?"none":router.GetEntityId().ToString())+" pilot="+(_trace._roomPilot==null?"none":System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(_trace._roomPilot).ToString())+" pilotRouter="+(pilotRouter==null?"none":pilotRouter.GetEntityId().ToString())+" pilotRouterSame="+ReferenceEquals(pilotRouter,router);
                    if(identity!=_transition){_transition=identity;if(_transitionRows++<4)Add("transition",identity);else _dropped++;}
                    if(flow.AcceptedFailureKind.HasValue && !_failed){_failed=true;Add("first-failure",identity+" actual="+flow.AcceptedFailureKind.Value);}
                    if(!owner.TryGetCompletedEvidence(out var view))return;
                    var key=view.Generation+":"+view.Tick.Value;if(key==_lastKey)return;_lastKey=key;
                    var raw=identity+" run="+view.RunKey+" completedTick="+view.Tick.Value+" wrapperOrdinalBefore="+ordinal+" issuedThroughBefore="+_trace._issuedSequence+" priorCorrelatedThrough="+_trace._completedIssued+" worldPosition="+view.Position.ToString("R")+" health="+view.Health+" alive="+view.AliveEnemyCount+" transferTarget="+view.TransferTarget;
                    if(router!=null && router.BoundPlayerForAuthoring.TryReadExistingMotionForAuthoring(out var motion,out var next))raw+=" velocityQ="+motion.VelocityXQ4096+","+motion.VelocityYQ4096+" grounded="+motion.IsGrounded+" next="+next.Value;
                    else raw+=" motion=unobserved";
                    var combat=router==null?null:router.transform.parent.GetComponentInChildren<CombatSimulationDriver>(true);
                    if(combat!=null && combat.HasLatestOutcomeForEncounterHandoff){foreach(var state in combat.LatestOutcomeForEncounterHandoff.Combatants)raw+=" target="+state.TargetId+":"+state.CurrentHealth;var attack=combat.LatestOutcomeForEncounterHandoff.Attack;raw+=" combatTick="+combat.LatestOutcomeForEncounterHandoff.Tick.Value+" selectedTarget="+attack.Selection.TargetId+" selectionTick="+attack.Selection.Tick.Value+" attackResult="+(attack.Attempt.HasValue?attack.Attempt.Value.Code.ToString():"none");}else raw+=" combat=unobserved";
                    string keyboardIssued=null,pointerIssued=null;
                    for(var i=_trace._evidenceRows.Count-1;i>=Math.Max(0,_trace._evidenceRows.Count-16) && (keyboardIssued==null || pointerIssued==null);i--)
                    {var issued=_trace._evidenceRows[i];var payload=issued.LastIndexOf("|nextSequence=",StringComparison.Ordinal);if(payload<0)continue;if(keyboardIssued==null && issued.Contains("|queue-keyboard|"))keyboardIssued=issued.Substring(payload);if(pointerIssued==null && issued.Contains("|queue-pointer|"))pointerIssued=issued.Substring(payload);}
                    raw+=" issuedKeyboard="+(keyboardIssued??"unobserved-within-last16")+" issuedPointer="+(pointerIssued??"unobserved-within-last16");
                    if(System.Text.Encoding.UTF8.GetByteCount(raw)>1700){while(System.Text.Encoding.UTF8.GetByteCount(raw)>1680)raw=raw.Substring(0,raw.Length-1);raw+="[truncated]";_dropped++;}
                    var isR02=phase==Chapter1RunPhase.InRoom && actualRoom=="R02" && view.Generation==owner.Generation && view.RunKey==flow.View.RunKey && router!=null;
                    if(!isR02)return;
                    var local=room.transform.InverseTransformPoint(new Vector3(view.Position.x,view.Position.y,0));raw+=" local="+local.ToString("R");
                    if(_r02EntryRemaining>0){Add("r02-first-completed-and-three-following",raw);_r02EntryRemaining--;}
                    for(var i=0;i<3;i++)if(_after[i]>0){Add("boundary-"+new[]{27,30,32}[i]+"-after",raw);_after[i]--;_duplicateReasons++;}
                    if(_past32)_after32++;
                    var boundaries=new[]{27,30,32};
                    for(var i=0;i<3;i++)if(!_crossed[i] && local.x>=boundaries[i])
                    {foreach(var prior in _before)Add("boundary-"+boundaries[i]+"-before",prior);Add("boundary-"+boundaries[i]+"-at",raw);_crossed[i]=true;_after[i]=2;_duplicateReasons++;if(i==2)_past32=true;}
                    if(!_zero && view.AliveEnemyCount==0){_zero=true;Add("first-enemies-zero",raw);}
                    if(!_below && local.y<0){_below=true;Add("first-below-floor",raw);}
                    _before.Enqueue(raw);while(_before.Count>2)_before.Dequeue();
                }
                catch(Exception error){if(!_readError){_readError=true;Add("read-unobserved",error.GetType().Name+":"+error.Message);}else _dropped++;}
            }
            internal void Close()
            {
                if(_closed)return;_closed=true;
                try
                {
                    var summary="{\"reason\":\"end\",\"rowsBeforeEnd\":"+_rows.Count+",\"droppedOrTruncated\":"+_dropped+",\"boundaryReasonCount\":"+_duplicateReasons+",\"diagnosticIncomplete\":"+(StopAfterBoundary?"true":"false")+"}";
                    _rows.Add(summary);var bytes=new System.Text.UTF8Encoding(false).GetBytes(string.Join("\n",_rows)+"\n");
                    if(_rows.Count>32 || bytes.Length>65536)throw new InvalidOperationException("R02 진단 원문 상한입니다.");
                    using(var stream=new FileStream(_path,FileMode.CreateNew,FileAccess.Write,FileShare.Read)){stream.Write(bytes,0,bytes.Length);stream.Flush(true);}
                }
                catch(Exception error){Debug.LogError("R02 진단 폐쇄 실패(원래 시험 결과 별도): "+error);}
            }
        }
        internal IEnumerator ConfirmReturn()
        {Keyboard();Pointer(Vector2.zero,false);yield return WaitReady();Keyboard(Key.Enter);yield return new WaitForFixedUpdate();Keyboard();yield return new WaitForFixedUpdate();Assert.That(Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Hub));}
        private void RecordRoom(){var id=Flow.View.RoomId;if(Flow.View.Phase!=Chapter1RunPhase.InRoom || id==null || id=="Ordan" || !Flow.Lifetime.TryGetCompletedEvidence(out var actual))return;
            for(var i=0;i<_visitedCount;i++)if(_visited[i]==id)return;Assert.That(_visitedCount,Is.LessThan(4));_visited[_visitedCount++]=id;
            Debug.Log("실제 방 진입 관측: "+actual.RunKey+" "+id+" 세대 "+actual.Generation+" 틱 "+actual.Tick.Value+" 계획 "+Flow.PlanSha);}
        private void PlayInput(int ordinal,bool jump)
        {
            var router=Flow.Lifetime.CurrentRouter;if(router==null){Keyboard();Pointer(Vector2.zero,false);return;}
            if(router.IsChapter1Regular)
            {
                var observedTick=router.BoundPlayerForAuthoring.Snapshot.Tick.Value;
                if(!_closedLoopFirstTick.HasValue)_closedLoopFirstTick=observedTick;
                if(observedTick-_closedLoopFirstTick.Value>=3000)throw new InvalidOperationException("시험 입력 사례 전체의 3000 완료 틱 상한입니다.");
                if(_roomPilot==null || !ReferenceEquals(_roomPilot.Router,router))_roomPilot=new Chapter1ProductionRoomPilot(router);
                var command=_roomPilot.Read(ordinal,jump);var issuedKeys=command.Keys;
                // REQ-AUTOCH1-R02INPUT-001: filter this issuance only, after matching actual completed combat evidence.
                var room=router.transform.parent.GetComponent<Chapter1RoomInstance>();
                var combat=router.transform.parent.GetComponentInChildren<CombatSimulationDriver>(true);
                if(Flow.View.Phase==Chapter1RunPhase.InRoom && Flow.View.RoomId=="R02" && room!=null && room.RoomId=="R02" &&
                    ReferenceEquals(_roomPilot.Router,router) && ReferenceEquals(Flow.Lifetime.CurrentRouter,router) &&
                    Flow.Lifetime.TryGetCompletedEvidence(out var completed) && completed.HasCompleted && !completed.Lightweight &&
                    completed.Generation==Flow.Lifetime.Generation && completed.RunKey==Flow.View.RunKey &&
                    completed.Tick.Value==observedTick && completed.Tick.Value==command.ObservedTick && completed.AliveEnemyCount==0 &&
                    combat!=null && combat.HasLatestOutcomeForEncounterHandoff && combat.LatestOutcomeForEncounterHandoff.Tick==completed.Tick)
                {
                    var alive=false;foreach(var state in combat.LatestOutcomeForEncounterHandoff.Combatants)
                        if(state.TargetId!="player" && !state.IsDead && state.CurrentHealth>0){alive=true;break;}
                    if(!alive){var filtered=new System.Collections.Generic.List<Key>();foreach(var key in command.Keys)if(key!=Key.LeftShift)filtered.Add(key);issuedKeys=filtered.ToArray();}
                }
                Keyboard(issuedKeys);Pointer(command.Pointer,command.Attack,false);return;
            }
            var graph=router.transform.parent;var camera=graph.GetComponentInChildren<UnityEngine.Camera>(true);Transform target=null;
            foreach(var id in new[]{"Walker","Surveyor"}){var value=graph.Find(id);if(value!=null && value.gameObject.activeInHierarchy && value.GetComponent<Collider2D>()!=null && value.GetComponent<Collider2D>().enabled){target=value;break;}}
            var point=camera==null?Vector2.zero:(Vector2)camera.WorldToScreenPoint(target==null?new Vector3(31,1,0):target.position);
            Keyboard(jump && ordinal%30<3?new[]{Key.D,Key.Space}:new[]{Key.D});
            Pointer(point,ordinal%8==0,ordinal%80==0);
        }
        private Chapter1ProductionRoomPilot _roomPilot;
        private int? _closedLoopFirstTick;
        internal static ProfileSnapshotV1 ReadCurrentTestProfile()
        {
            var root=Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot);
            var observed=ProfileLaunchObservationAdapterV1.Observe(root);observed.Validate();
            var plan=ProfileLoadSelectorV1.Select(observed.Primary,observed.Previous,observed.Temp);plan.Validate();
            Assert.That(plan.RequiresAtomicSave,Is.False);return plan.InMemoryDocument.Snapshot;
        }
        internal GameObject BarrierGraph=>Flow.Lifetime.CurrentRouter.transform.parent.gameObject;
        internal IEnumerator WaitPhase(Chapter1RunPhase phase,int limit=300)
        {
            for(var i=0;i<limit;i++)
            {
                Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);
                if(Flow.View.Phase==phase)yield break;
                yield return new WaitForFixedUpdate();
            }
            Assert.Fail("생산 단계 도달 상한: "+phase+" · 현재 "+Flow.View.Phase);
        }
        internal IEnumerator PlayToChoice()
        {
            yield return Start(null);yield return PlayToBoss();
            for(var ordinal=0;ordinal<18000;ordinal++)
            {
                Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);
                Assert.That(Flow.AcceptedFailureKind.HasValue,Is.False,"정상 보스 입력 중 실제 플레이어가 사망했습니다.");
                if(Flow.View.Phase==Chapter1RunPhase.ChoiceRequired){Keyboard();Pointer(Vector2.zero,false);yield return new WaitForFixedUpdate();yield break;}
                if(Flow.View.Phase==Chapter1RunPhase.AtBarrier){Keyboard();Pointer(Vector2.zero,false);yield break;} // 재시작의 저장된 선택.
                if(Flow.View.Phase==Chapter1RunPhase.AtBoss)
                {
                    var graph=Flow.Lifetime.CurrentRouter.transform.parent;var boss=graph.Find("Ordan");var camera=graph.GetComponentInChildren<UnityEngine.Camera>(true);
                    Assert.That(boss,Is.Not.Null);Assert.That(camera,Is.Not.Null);Assert.That(Flow.Lifetime.TryGetCompletedEvidence(out var actual),Is.True);
                    var keys=new System.Collections.Generic.List<Key>();if(actual.Position.x<6)keys.Add(Key.D);
                    if(ordinal%90<3)keys.Add(Key.LeftShift);else if(ordinal%60<3)keys.Add(Key.Space);
                    Keyboard(keys.ToArray());Pointer(camera.WorldToScreenPoint(boss.position),ordinal%8==0);
                }
                else{Keyboard();Pointer(Vector2.zero,false);}
                yield return new WaitForFixedUpdate();
            }
            Assert.Fail("생산 보스 입력 18000틱 안에 선택 단계에 도달하지 못했습니다.");
        }
        internal IEnumerator ChoiceClick(int index,bool together=false,bool keepHeld=false)
        {
            Assert.That(index,Is.InRange(0,4));Keyboard();Pointer(Vector2.zero,false);yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
            var scale=Mathf.FloorToInt(Mathf.Min(Screen.width/640f,Screen.height/360f));Assert.That(scale,Is.GreaterThan(0));
            var point=new Vector2(Screen.width/2f,Screen.height/2f+(-35-index*29)*scale);
            Pointer(point,true);if(together)Keyboard(Key.Enter);yield return new WaitForFixedUpdate();
            if(!keepHeld){Keyboard();Pointer(point,false);yield return new WaitForFixedUpdate();}
        }
        internal IEnumerator SelectBranch(int branch)
        {
            if(Flow.View.Phase==Chapter1RunPhase.AtBarrier)yield break;
            Assert.That(branch,Is.InRange(1,2));Assert.That(Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.ChoiceRequired));
            yield return ChoiceClick(branch-1);
            Assert.That(Flow.Lifetime.CurrentChoiceAdapter.UiPhase,Is.EqualTo(branch==1?Chapter1ChoiceUiPhase.ExtractionProposed:Chapter1ChoiceUiPhase.SolidarityProposed));
            yield return ChoiceClick(2);yield return WaitPhase(Chapter1RunPhase.AtBarrier);
            Assert.That(Flow.CurrentChapter1CompletionReceipt,Is.Null);
        }
        private int _c1cDiagnosticRows,_c1cDiagnosticBytes;
        internal static string C1cRead(Func<string> read)
        {try{var value=read()??"<null>";return value.Length>1024?value.Substring(0,1024-"[item-truncated]".Length)+"[item-truncated]":value;}catch(Exception error){return "unobserved-read-rejected:"+error.GetType().FullName;}}
        internal void RecordC1cDiagnostic(string stage,Func<string> read)
        {
            if(_c1cDiagnosticRows>=6)return;
            try
            {
                var line="[CH1C1CDiagnostic] case="+TestContext.CurrentContext.Test.ID+" stage="+stage+" frame="+Time.frameCount+" update="+InputState.currentUpdateType+" count="+InputState.updateCount+" "+read();
                var truncated=false;while(System.Text.Encoding.UTF8.GetByteCount(line)>10000){line=line.Substring(0,line.Length/2);truncated=true;}
                line+=" recordTruncated="+truncated;var bytes=System.Text.Encoding.UTF8.GetByteCount(line)+1;
                if(_c1cDiagnosticBytes+bytes>65536)return;_c1cDiagnosticRows++;_c1cDiagnosticBytes+=bytes;Debug.Log(line);
            }
            catch(Exception error){try{var line="[CH1C1CDiagnostic] stage="+stage+" diagnosticFailed="+error.GetType().FullName+" originalFailurePreserved=true";var bytes=System.Text.Encoding.UTF8.GetByteCount(line)+1;if(_c1cDiagnosticRows<6 && _c1cDiagnosticBytes+bytes<=65536){_c1cDiagnosticRows++;_c1cDiagnosticBytes+=bytes;Debug.Log(line);}}catch{}}
        }
        private void RecordCounterweightDiagnostic(string stage,UnityEngine.Camera camera,Transform target)
        {
            RecordC1cDiagnostic(stage,()=>
            {
                var router=Flow.Lifetime.CurrentRouter;var driver=router==null?null:router.BoundTransferForAuthoring;
                var text="flow="+C1cRead(()=>Flow.View.Phase+"/generation="+Flow.View.Generation+"/tick="+(Flow.View.Tick.HasValue?Flow.View.Tick.Value.Value.ToString():"unobserved"))+" router="+C1cRead(()=>router.GetEntityId().ToString())+" driver="+C1cRead(()=>driver.GetEntityId().ToString());
                text+=" publication=unobserved-internal-not-accessible-to-RunUnity-test";
                text+=" completed="+C1cRead(()=>{if(!Flow.Lifetime.TryGetCompletedEvidence(out var value))return "unpublished";return "tick="+value.Tick.Value+"/generation="+value.Generation+"/activeTarget="+value.TransferTarget+"/position="+value.Position+"/health="+value.Health;});
                text+=" player="+C1cRead(()=>router.BoundPlayerForAuthoring.GetEntityId()+"/position="+router.BoundPlayerForAuthoring.transform.position+"/local="+router.BoundPlayerForAuthoring.transform.localPosition+"/snapshotTick="+router.BoundPlayerForAuthoring.Snapshot.Tick.Value);
                text+=" target="+C1cRead(()=>target.GetEntityId()+"/position="+target.position+"/local="+target.localPosition+"/scale="+target.lossyScale);
                text+=" camera="+C1cRead(()=>camera.GetEntityId()+"/position="+camera.transform.position+"/pixel="+camera.pixelRect+"/targetScreen="+camera.WorldToScreenPoint(target.position))+" viewMatrix="+C1cRead(()=>camera.worldToCameraMatrix.ToString("R"))+" projection="+C1cRead(()=>camera.projectionMatrix.ToString("R"));
                return text+" ownedMouse="+C1cRead(()=>_mouse.deviceId+"/position="+_mouse.position.ReadValue()+"/right="+_mouse.rightButton.isPressed);
            });
        }
        internal IEnumerator TransferToCounterweight()
        {
            Assert.That(Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.AtBarrier));var graph=BarrierGraph;
            var camera=graph.GetComponentInChildren<UnityEngine.Camera>(true);var target=graph.transform.Find("Counterweight");Assert.That(target,Is.Not.Null);
            RecordCounterweightDiagnostic("transfer-ready",camera,target);
            Keyboard();Pointer(camera.WorldToScreenPoint(target.position),false);yield return new WaitForFixedUpdate();
            Pointer(camera.WorldToScreenPoint(target.position),false,true);RecordCounterweightDiagnostic("transfer-edge-issued-consumption-unobserved",camera,target);yield return new WaitForFixedUpdate();RecordCounterweightDiagnostic("transfer-first-fixed-result",camera,target);Pointer(camera.WorldToScreenPoint(target.position),false);
            for(var i=0;i<60;i++){yield return new WaitForFixedUpdate();if(Flow.Lifetime.TryGetCompletedEvidence(out var view) && view.TransferTarget=="BarrierCounterweight")yield break;}
            Assert.Fail("실제 오른쪽 버튼 입력으로 상자 전이가 발생하지 않았습니다.");
        }
        // REQ-AUTOCH1-BARRIERDIAG-001: bounded observational reads, separate from the six C1C rows.
        private BarrierDiagnostic _barrierDiagnostic;
        private sealed class BarrierDiagnostic
        {
            private readonly Chapter1AuthoredInputTrace _trace;
            private readonly System.Collections.Generic.HashSet<string> _seen=new System.Collections.Generic.HashSet<string>();
            private int _rows,_bytes,_dropped,_readFailures,_graphChanges;private bool _closed;
            private string _lastGraph,_lastState;
            internal BarrierDiagnostic(Chapter1AuthoredInputTrace trace){_trace=trace;}
            private string Read(Func<string> read){try{return read()??"unobserved-null";}catch(Exception e){_readFailures++;return "unobserved:"+e.GetType().Name;}}
            internal void Observe(string stage,int ordinal=-1,bool forced=false)
            {
                if(_closed)return;
                try
                {
                    var flow=_trace.Flow;var owner=flow.Lifetime;var router=owner.CurrentRouter;
                    var graph=router==null?null:router.transform.parent;
                    var id=graph==null?"none":graph.GetEntityId().ToString();
                    var reasons=new System.Collections.Generic.List<string>();
                    if(_lastGraph==null){_lastGraph=id;reasons.Add("initial");}
                    else if(_lastGraph!=id){_lastGraph=id;_graphChanges++;if(_seen.Add("graph-change"))reasons.Add("first-graph-change");}
                    var env=graph==null?null:graph.GetComponentInChildren<Chapter1ChoiceEnvironmentOwner>(true);
                    var player=router==null?null:router.BoundPlayerForAuthoring;
                    var body=player==null?null:player.GetComponent<Rigidbody2D>();var shape=player==null?null:player.GetComponent<Collider2D>();
                    var platform=graph==null?null:graph.Find("Platform").GetComponent<BoxCollider2D>();
                    var gate=graph==null?null:graph.Find("Gate").GetComponent<BoxCollider2D>();
                    var bridge=graph==null?null:graph.Find("Bridge").GetComponent<BoxCollider2D>();
                    var touch=Read(()=>shape==null||platform==null?"unobserved":shape.IsTouching(platform).ToString());
                    var state=Read(()=>"skill="+(env==null?"unobserved":env.HasActualSkill.ToString())+"/gateOpened="+(env==null?"unobserved":env.GateOpened.ToString())+"/gate="+(gate==null?"unobserved":gate.enabled.ToString())+"/bridge="+(bridge==null?"unobserved":bridge.enabled.ToString())+"/platform="+(platform==null?"unobserved":platform.enabled.ToString())+"/contact="+touch);
                    if(_lastState==null)_lastState=state;
                    else if(_lastState!=state){var previous=_lastState.Split('/');var current=state.Split('/');for(var i=0;i<Math.Min(previous.Length,current.Length);i++)if(previous[i]!=current[i] && _seen.Add("state-"+i+":"+current[i]))reasons.Add("first-"+current[i]);_lastState=state;}
                    var completed=default(Chapter1EncounterEvidenceView);var hasCompleted=owner.TryGetCompletedEvidence(out completed);
                    if(hasCompleted){if(completed.Position.x>1 && _seen.Add("x1"))reasons.Add("first-x1-exit");if(completed.Position.y<=0 && _seen.Add("y0"))reasons.Add("first-y0");if(completed.Position.y<=-2 && _seen.Add("y-2"))reasons.Add("first-y-2");}
                    if(forced)reasons.Add(stage);if(reasons.Count==0)return;
                    var raw="stage="+stage+" reasons="+string.Join(",",reasons)+" wrapperOrdinalBefore="+ordinal+" priorIssuedThrough="+_trace._issuedSequence+" observation=completed-before-next-issuance";
                    raw+=" phase="+flow.View.Phase+" room="+flow.View.RoomId+" run="+flow.View.RunKey+" generation="+owner.Generation+" router="+(router==null?"none":router.GetEntityId().ToString())+" graph="+id+" "+state+" passagePresent="+(owner.CurrentChapter1Passage!=null);
                    raw+=" completed="+(hasCompleted?"tick="+completed.Tick.Value+"/gen="+completed.Generation+"/run="+completed.RunKey+"/position="+completed.Position.ToString("R")+"/health="+completed.Health+"/transfer="+completed.TransferTarget:"unobserved");
                    raw+=" motion="+Read(()=>{if(player==null||!player.TryReadExistingMotionForAuthoring(out var motion,out var next))return "unobserved";return "tick="+motion.Tick.Value+"/next="+next.Value+"/positionQ="+motion.PositionXQ4096+","+motion.PositionYQ4096+"/velocityQ="+motion.VelocityXQ4096+","+motion.VelocityYQ4096+"/grounded="+motion.IsGrounded;});
                    raw+=" playerBody="+Read(()=>body==null?"unobserved":body.bodyType+"/fullKinematicContacts="+body.useFullKinematicContacts+"/bounds="+shape.bounds);
                    foreach(var name in new[]{"Gate","Bridge","Platform","Exit","Counterweight"})raw+=" "+name+"="+Read(()=>{var child=graph==null?null:graph.Find(name);var collider=child==null?null:child.GetComponent<Collider2D>();if(collider==null)return "unobserved";var attached=collider.attachedRigidbody;return "enabled="+collider.enabled+"/active="+collider.gameObject.activeInHierarchy+"/bounds="+collider.bounds+"/body="+(attached==null?"none-static":attached.bodyType.ToString());});
                    raw+=" footPlatformGap="+Read(()=>shape==null||platform==null?"unobserved":(shape.bounds.min.y-platform.bounds.max.y).ToString("R",System.Globalization.CultureInfo.InvariantCulture));
                    raw+=" combat="+Read(()=>{var combat=graph==null?null:graph.GetComponentInChildren<BarrierCombatSimulationDriver>(true);var pub=combat==null?null:combat.LatestPublication;if(pub==null)return "unobserved";var value="tick="+pub.Tick.Value+"/gen="+pub.Generation+"/sameOwner="+ReferenceEquals(pub.Owner,combat)+"/sameEnvironment="+ReferenceEquals(pub.Environment,env)+"/sameRun="+Equals(pub.RunKey,flow.View.RunKey)+"/sameGeneration="+(pub.Generation==owner.Generation)+"/sameCompletedTick="+(hasCompleted&&pub.Tick==completed.Tick)+"/valid="+combat.ValidatePublished(pub);foreach(var request in pub.Requests)value+="/request="+request.RequestId+":"+request.TargetId+":"+request.Amount;foreach(var result in pub.Results)value+="/result="+result.RequestId+":"+result.TargetId+":"+result.Result;foreach(var target in pub.Combatants)value+="/target="+target.TargetId+":"+target.CurrentHealth+":"+target.IsDead;return value;});
                    var issued=0;for(var i=_trace._evidenceRows.Count-1;i>=0 && issued<2;i--){var row=_trace._evidenceRows[i];if(row.Contains("|queue-keyboard|")||row.Contains("|queue-pointer|")){raw+=" priorActualIssuance="+row;issued++;}}
                    Emit(raw,false);
                }
                catch(Exception e){_readFailures++;Emit("stage="+stage+" unobserved="+e.GetType().Name+" originalFailurePreserved=true",false);}
            }
            private void Emit(string raw,bool summary)
            {
                try{var text="[CH1BarrierDiagnostic] case="+TestContext.CurrentContext.Test.ID+" "+raw;var truncated=false;while(System.Text.Encoding.UTF8.GetByteCount(text)>8000){text=text.Substring(0,text.Length-1);truncated=true;}text+=" truncated="+truncated;var bytes=System.Text.Encoding.UTF8.GetByteCount(text)+1;
                    if(!summary && (_rows>=23 || _bytes+bytes>65536-8192)){_dropped++;return;}
                    if(_rows>=24||_bytes+bytes>65536){_dropped++;return;}_rows++;_bytes+=bytes;Debug.Log(text);
                }catch{_dropped++;}
            }
            internal void Close(){if(_closed)return;_closed=true;Emit("stage=finally-summary rowsBefore="+_rows+" bytesBefore="+_bytes+" dropped="+_dropped+" limitExceeded="+(_dropped>0)+" readFailures="+_readFailures+" graphChanges="+_graphChanges+" repeatedGraphDetailsOmitted="+Math.Max(0,_graphChanges-1)+" originalFailurePreserved=true",true);}
        }
        internal IEnumerator UseBarrierSkill()
        {
            _barrierDiagnostic=new BarrierDiagnostic(this);var skillObserved=false;
            try
            {
            yield return TransferToCounterweight();
            for(var i=0;i<24;i++)yield return new WaitForFixedUpdate();
            _barrierDiagnostic.Observe("before-Q",-1,true);
            Keyboard(Key.Q);yield return new WaitForFixedUpdate();Keyboard();
            var environment=BarrierGraph.GetComponentInChildren<Chapter1ChoiceEnvironmentOwner>(true);
            for(var i=0;i<60;i++){yield return new WaitForFixedUpdate();if(environment.HasActualSkill){skillObserved=true;_barrierDiagnostic.Observe("first-actual-skill",-1,true);yield break;}}
            _barrierDiagnostic.Observe("skill-failure-before-assert",-1,true);
            Assert.Fail("실제 Q 입력으로 확정 기술이 사용되지 않았습니다.");
            }finally{if(!skillObserved)_barrierDiagnostic.Close();}
        }
        internal IEnumerator PassBarrier()
        {
            // REQ-AUTOCH1-BARRIERINPUT-001: all preparation, braking and landing consume the original 1200 ticks.
            var originalRouter=Flow.Lifetime.CurrentRouter;var originalGeneration=Flow.Lifetime.Generation;
            var graph=BarrierGraph;var environment=graph.GetComponentInChildren<Chapter1ChoiceEnvironmentOwner>(true);
            var bridge=graph.transform.Find("Bridge").GetComponent<BoxCollider2D>();
            var box=graph.transform.Find("Counterweight").GetComponent<BoxCollider2D>();var exit=graph.transform.Find("Exit").GetComponent<BoxCollider2D>();
            var boxJumpStarted=false;var boxCleared=false;var preparationTicks=0;
            try
            {
            _barrierDiagnostic?.Observe("pass-start",-1,true);
            Assert.That(Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.AtBarrier));
            for(var ordinal=0;ordinal<1200;ordinal++)
            {
                _barrierDiagnostic?.Observe("pass-before-issuance",ordinal);
                Assert.That(Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),Flow.View.Error);
                if(Flow.View.Phase==Chapter1RunPhase.Closing){Keyboard();Pointer(Vector2.zero,false);yield return new WaitForFixedUpdate();yield return null;yield break;}
                // REQ-AUTOCH1-BARRIERINPUT-001: only the two normal retirement phases release the old graph requirement.
                if(Flow.View.Phase==Chapter1RunPhase.EndingSuccessfulRun || Flow.View.Phase==Chapter1RunPhase.PreparingClosing)
                {Keyboard();Pointer(Vector2.zero,false);}
                else
                {
                Assert.That(Flow.Lifetime.CurrentRouter,Is.SameAs(originalRouter),"봉쇄선 입력 중 원본 graph가 교체됐습니다.");
                Assert.That(Flow.Lifetime.Generation,Is.EqualTo(originalGeneration),"봉쇄선 입력 중 원본 세대가 교체됐습니다.");
                if(Flow.View.Phase==Chapter1RunPhase.AtBarrier && Flow.Lifetime.TryGetCompletedEvidence(out var view) &&
                    view.Generation==originalGeneration && originalRouter.BoundPlayerForAuthoring.TryReadExistingMotionForAuthoring(out var motion,out var next) && motion.Tick==view.Tick && next.Value==view.Tick.Value+1)
                {
                    var publication=graph.GetComponentInChildren<BarrierCombatSimulationDriver>(true).LatestPublication;
                    Assert.That(publication,Is.Not.Null);Assert.That(publication.Tick,Is.EqualTo(view.Tick));Assert.That(publication.Generation,Is.EqualTo(originalGeneration));
                    var extraction=false;foreach(var target in publication.Combatants)if(target.TargetId=="BarrierGate-Fractured")extraction=true;
                    if(extraction && (!environment.GateOpened || !bridge.enabled))
                    {Assert.That(preparationTicks++,Is.LessThan(60),"실제 문/다리 준비가 60틱 안에 완료되지 않았습니다.");Keyboard();Pointer(Vector2.zero,false);}
                    else
                    {
                        var body=view.PlayerBody;var obstacle=box.bounds;var destination=exit.bounds;const float skin=82f/4096f;
                        var x=view.Position.x;var velocity=motion.VelocityXQ4096/4096f;var half=body.width/2f;
                        var min=destination.min.x+half+skin;var max=destination.max.x-half-skin;var center=(min+max)/2f;
                        Assert.That(float.IsNaN(x)||float.IsInfinity(x)||float.IsNaN(velocity)||float.IsInfinity(velocity)||float.IsNaN(min)||float.IsInfinity(min)||float.IsNaN(max)||float.IsInfinity(max)||min>=max,Is.False,"봉쇄선 실제 출구 기하/속도가 유효하지 않습니다.");
                        foreach(var coordinate in new[]{body.xMin,body.xMax,body.yMin,body.yMax,obstacle.min.x,obstacle.max.x,obstacle.min.y,obstacle.max.y,destination.min.x,destination.max.x,destination.min.y,destination.max.y})
                            Assert.That(float.IsNaN(coordinate)||float.IsInfinity(coordinate),Is.False,"봉쇄선 실제 몸체/상자/출구 기하가 비유한입니다.");
                        if(body.xMin>obstacle.max.x+skin)boxCleared=true;
                        if(!boxCleared && !boxJumpStarted && body.xMax>=obstacle.min.x-.3f && motion.IsGrounded)boxJumpStarted=true;
                        var jump=boxJumpStarted&&!boxCleared;
                        var direction=1;
                        if(boxCleared)
                        {
                            var deceleration=motion.IsGrounded?90f:20f;var stopping=velocity*velocity/(2f*deceleration)+Mathf.Abs(velocity)/60f+skin;
                            var delta=center-body.center.x;
                            if(body.center.x>=min && body.center.x<=max)direction=velocity>.5f?-1:velocity<-.5f?1:0;
                            else if(velocity*delta>0 && stopping>=Mathf.Abs(delta))direction=velocity>0?-1:1;
                            else direction=delta>0?1:delta<0?-1:0;
                        }
                        if(direction>0)Keyboard(jump?new[]{Key.D,Key.Space}:new[]{Key.D});else if(direction<0)Keyboard(jump?new[]{Key.A,Key.Space}:new[]{Key.A});else Keyboard(jump?new[]{Key.Space}:Array.Empty<Key>());
                        Pointer(Vector2.zero,false);
                    }
                }
                else{Keyboard();Pointer(Vector2.zero,false);}
                }
                yield return new WaitForFixedUpdate();
            }
            _barrierDiagnostic?.Observe("pass-failure-before-assert",1200,true);
            Assert.Fail("실제 이동·점프로 봉쇄선 출구와 확정 종결에 도달하지 못했습니다.");
            }finally{_barrierDiagnostic?.Close();}
        }
        internal IEnumerator OpenBarrier(int branch)
        {yield return Open();yield return PlayToChoice();yield return SelectBranch(branch);}
        internal IEnumerator CompleteBranch(int branch)
        {yield return OpenBarrier(branch);yield return UseBarrierSkill();yield return PassBarrier();yield return FinishClosing(branch);}
        internal string ClosingStatus()
        {
            var source=Flow.Lifetime.CurrentChoiceAdapter;Assert.That(source,Is.Not.Null);
            foreach(var text in source.GetComponentsInChildren<TMP_Text>(true))if(text.name=="Status")return text.text;
            Assert.Fail("현재 종결 화면의 실제 상태 문구가 없습니다.");return null;
        }
        // REQ/AC-AUTOCH1-CLOSINGDIAG-001: first closing line only; no input or consumer/index inference.
        private ClosingInputDiagnostic _closingInputDiagnostic;
        private bool _closingFirstLineEdge;
        private sealed class ClosingInputDiagnostic
        {
            private readonly Chapter1AuthoredInputTrace _trace;private Chapter1ChoicePhaseAdapter _source;private InputRouter _router;private Chapter1ClosingHostEntry _entry;
            private int _rows,_bytes,_dropped,_readFailures,_recordFailures;private bool _closed;
            internal ClosingInputDiagnostic(Chapter1AuthoredInputTrace trace){_trace=trace;}
            private string Read(Func<string> read){try{return read()??"unobserved-null";}catch(Exception e){_readFailures++;return "unobserved:"+e.GetType().Name;}}
            internal void Observe(string stage)
            {
                if(_closed)return;
                try
                {
                    var flow=_trace.Flow;var source=flow.Lifetime.CurrentChoiceAdapter;var router=source==null?null:source.Router;var entry=source==null?null:source.ClosingHostEntry;
                    if(_source==null && source!=null){_source=source;_router=router;_entry=entry;}
                    var raw="stage="+stage+" flow="+flow.View.Phase+" frame="+Time.frameCount+" inputUpdate="+InputState.updateCount+" updateType="+InputState.currentUpdateType+
                        " sourceSame="+ReferenceEquals(source,_source)+" routerSame="+ReferenceEquals(router,_router)+" entrySame="+ReferenceEquals(entry,_entry)+
                        " source="+(source==null?"none":source.GetEntityId().ToString())+" router="+(router==null?"none":router.GetEntityId().ToString());
                    raw+=" entry="+Read(()=>entry==null?"unobserved":"id="+System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(entry)+"/ownerSame="+ReferenceEquals(entry.Owner,flow.Lifetime)+"/endpointSame="+ReferenceEquals(entry.Endpoint,flow)+"/gen="+entry.NewUiGeneration+"/firstUiTick="+entry.FirstUiTick.Value);
                    var owned=source!=null&&source.OwnsIssuedClosingHost;
                    raw+=" ownsIssued="+owned+" uiPhase="+Read(()=>source==null?"unobserved":source.UiPhase.ToString());
                    raw+=" receipt="+Read(()=>{var receipt=router==null?null:router.CurrentReceipt;return !receipt.HasValue?"unobserved":"tick="+receipt.Value.Tick.Value+"/ordinal="+receipt.Value.FrameOrdinal+"/mode="+receipt.Value.Mode+"/epoch="+receipt.Value.ModeEpoch;});
                    UiSemanticFrameV1? frame=null;
                    raw+=" uiFrame="+Read(()=>{if(!owned||router==null)return "unobserved-not-owned";frame=router.CurrentUiFrame;if(!frame.HasValue)return "unpublished";var value=frame.Value;return "tick="+value.Receipt.Tick.Value+"/ordinal="+value.Receipt.FrameOrdinal+"/mode="+value.Receipt.Mode+"/Submit="+value.SubmitPressed+"/Click="+value.ClickPressed+"/Cancel="+value.CancelPressed;});
                    raw+=" closingNeutralReady="+Read(()=>source==null?"unobserved":source.ClosingControlsNeutralReady.ToString());
                    raw+=" closingPhysicalNeutral="+Read(()=>{if(!owned||router==null||entry==null||!frame.HasValue)return "unobserved";var available=router.TryReadChapter1ClosingControlsNeutral(source,entry,frame.Value,out var neutral);return "available="+available+"/value="+(available?neutral.ToString():"unobserved");});
                    raw+=" held="+Read(()=>"enter="+(_trace._keyboard==null?"unobserved":_trace._keyboard.enterKey.isPressed.ToString())+"/left="+(_trace._mouse==null?"unobserved":_trace._mouse.leftButton.isPressed.ToString())+"/right="+(_trace._mouse==null?"unobserved":_trace._mouse.rightButton.isPressed.ToString()));
                    raw+=" scalerSupported=unobserved-internal-access scalerIntegerScale=unobserved-internal-access screen="+Screen.width+"x"+Screen.height;
                    raw+=" status="+Read(()=>{if(source==null)return "unobserved";foreach(var text in source.GetComponentsInChildren<TMP_Text>(true))if(text.name=="Status")return text.text;return "unobserved-no-status";});
                    raw+=" adapterConsumed=unobserved-private closingLineIndex=unobserved-private priorIssuedThrough="+_trace._issuedSequence;
                    Emit(raw,false);
                }
                catch(Exception e){_readFailures++;Emit("stage="+stage+" unobserved="+e.GetType().Name+" originalFailurePreserved=true",false);}
            }
            private void Emit(string raw,bool summary)
            {
                try{var text="[CH1ClosingNormal] case="+TestContext.CurrentContext.Test.ID+" "+raw;var truncated=false;while(System.Text.Encoding.UTF8.GetByteCount(text)>1740){text=text.Substring(0,text.Length-1);truncated=true;}text+=" truncated="+truncated;var bytes=System.Text.Encoding.UTF8.GetByteCount(text)+1;
                    if(!summary&&(_rows>=23||_bytes+bytes>65536-1800)){_dropped++;return;}if(_rows>=24||bytes>1800||_bytes+bytes>65536){_dropped++;return;}_rows++;_bytes+=bytes;Debug.Log(text);
                }catch{_recordFailures++;}
            }
            internal void Close(){if(_closed)return;_closed=true;Emit("stage=finally-summary rowsBefore="+_rows+" bytesBefore="+_bytes+" dropped="+_dropped+" limitExceeded="+(_dropped>0)+" readFailures="+_readFailures+" recordingFailures="+_recordFailures+" adapterConsumed=unobserved-private closingLineIndex=unobserved-private originalFailurePreserved=true",true);}
        }
        internal IEnumerator ClosingEdge(bool click=false,bool keepHeld=false)
        {
            Keyboard();Pointer(Vector2.zero,false);yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
            if(_closingFirstLineEdge)_closingInputDiagnostic?.Observe("after-two-neutral-fixed");
            if(click){var scale=Mathf.FloorToInt(Mathf.Min(Screen.width/640f,Screen.height/360f));Assert.That(scale,Is.GreaterThan(0));Pointer(new Vector2(Screen.width/2f,Screen.height/2f-130*scale),true);}else Keyboard(Key.Enter);
            if(_closingFirstLineEdge)_closingInputDiagnostic?.Observe("first-enter-issued-consumption-unobserved");
            yield return new WaitForFixedUpdate();if(_closingFirstLineEdge)_closingInputDiagnostic?.Observe("after-first-edge-fixed");if(!keepHeld){Keyboard();Pointer(Vector2.zero,false);yield return new WaitForFixedUpdate();if(_closingFirstLineEdge)_closingInputDiagnostic?.Observe("after-release-fixed");}
        }
        // REQ/AC-AUTOCH1-CLOSINGINPUT-001: normal playback only; hostile edge probes stay unchanged.
        // REQ/AC-AUTOCH1-CLOSINGQUEUE-001: normal UI input is processed by the existing player loop.
        private void QueueClosingKeyboard(params Key[] keys)
        {if(_keyboard==null)_keyboard=InputSystem.AddDevice<Keyboard>();RecordKeyboard(keys);InputSystem.QueueStateEvent(_keyboard,new KeyboardState(keys));RecordQueueSuccess();}
        private void QueueClosingPointer(Vector2 point,bool attack,bool transfer=false)
        {if(_mouse==null)_mouse=InputSystem.AddDevice<Mouse>();var state=new MouseState{position=point};if(attack)state=state.WithButton(MouseButton.Left);if(transfer)state=state.WithButton(MouseButton.Right);RecordPointer(point,attack,transfer);InputSystem.QueueStateEvent(_mouse,state);RecordQueueSuccess();}
        private IEnumerator FinishClosingLine(string[] lines,int line)
        {
            var source=Flow.Lifetime.CurrentChoiceAdapter;Assert.That(source,Is.Not.Null);
            var router=source.Router;var entry=source.ClosingHostEntry;
            Assert.That(router.CurrentReceipt.HasValue,Is.True);
            var previous=router.CurrentReceipt.Value;var baseline=previous.Tick.Value;var epoch=previous.ModeEpoch;
            var issued=false;var released=false;var advanced=false;var rounds=0;
            _closingInputDiagnostic.Observe("line="+line+" baseline");
            try
            {
                QueueClosingKeyboard();QueueClosingPointer(Vector2.zero,false);
                while(rounds<180)
                {
                    yield return null;yield return new WaitForFixedUpdate();rounds++;
                    if(issued&&!released){QueueClosingKeyboard();QueueClosingPointer(Vector2.zero,false);released=true;}
                    if(line==lines.Length-1 && Flow.View.Phase==Chapter1RunPhase.ChapterCompleted && Flow.CurrentChapter1CompletionReceipt!=null)
                    {
                        Assert.That(previous.Tick.Value-baseline,Is.InRange(0,120));
                        Assert.That(issued,Is.True,"completion requires this line's single issued edge");
                        var completedStatus=ClosingStatus();
                        if(completedStatus.Contains("1챕터 완료") && completedStatus.Contains("아직 열리지 않은 길")){advanced=true;yield break;}
                        Assert.That(completedStatus,Does.Contain(lines[line]),"unexpected pending completed render text");
                        continue;
                    }
                    Assert.That(router.CurrentReceipt.HasValue,Is.True);
                    var observed=router.CurrentReceipt.Value;
                    var observedDelta=observed.Tick.Value-previous.Tick.Value;var observedOrdinalDelta=observed.FrameOrdinal-previous.FrameOrdinal;
                    Assert.That(observed.ModeEpoch,Is.EqualTo(epoch));Assert.That(observedDelta,Is.GreaterThanOrEqualTo(0));Assert.That(observedOrdinalDelta,Is.EqualTo(observedDelta));
                    if(observedDelta==0)Assert.That(observed,Is.EqualTo(previous));
                    Assert.That(observed.Tick.Value-baseline,Is.LessThanOrEqualTo(120),"normal closing UI tick budget");
                    previous=observed;
                    Assert.That(Flow.Lifetime.CurrentChoiceAdapter,Is.SameAs(source));
                    Assert.That(source.Router,Is.SameAs(router));Assert.That(source.ClosingHostEntry,Is.SameAs(entry));
                    Assert.That(source.OwnsIssuedClosingHost,Is.True);Assert.That(source.UiPhase,Is.EqualTo(Chapter1ChoiceUiPhase.Closing));
                    Assert.That(router.CurrentReceipt.HasValue,Is.True);Assert.That(router.CurrentUiFrame.HasValue,Is.True);
                    var receipt=router.CurrentReceipt.Value;var frame=router.CurrentUiFrame.Value;
                    Assert.That(frame.Receipt,Is.EqualTo(receipt));Assert.That(receipt.Mode,Is.EqualTo(AcadeGameMaker.Core.InputMode.UIOnly));
                    Assert.That(receipt,Is.EqualTo(observed));
                    var status=ClosingStatus();
                    if(issued && line<lines.Length-1 && status.Contains(lines[line+1])){advanced=true;yield break;}
                    Assert.That(status,Does.Contain(lines[line]),"unexpected normal closing text");
                    if(!issued && receipt.Tick.Value>=baseline+60 && !_keyboard.enterKey.isPressed && !_mouse.leftButton.isPressed && !_mouse.rightButton.isPressed &&
                        source.ClosingControlsNeutralReady && router.TryReadChapter1ClosingControlsNeutral(source,entry,frame,out var neutral) && neutral)
                    {
                        _closingInputDiagnostic.Observe("line="+line+" armed rounds="+rounds);
                        if(line%2==1){var scale=Mathf.FloorToInt(Mathf.Min(Screen.width/640f,Screen.height/360f));Assert.That(scale,Is.GreaterThan(0));QueueClosingPointer(new Vector2(Screen.width/2f,Screen.height/2f-130*scale),true);}else QueueClosingKeyboard(Key.Enter);
                        issued=true;_closingInputDiagnostic.Observe("line="+line+" issued edge=1");
                    }
                }
                Assert.Fail("normal closing observation budget exhausted line="+line);
            }
            finally{_closingInputDiagnostic.Observe("line="+line+" "+(advanced?"advanced":"failure")+" issued="+issued+" rounds="+rounds);}
        }
        internal IEnumerator FinishClosing(int branch)
        {
            _closingInputDiagnostic=new ClosingInputDiagnostic(this);_closingFirstLineEdge=false;
            try
            {
            yield return WaitPhase(Chapter1RunPhase.Closing);yield return new WaitForFixedUpdate();
            var lines=ClosingLines(branch);
            for(var line=0;line<lines.Length;line++)
            {
                Assert.That(ClosingStatus(),Does.Contain(lines[line]));Assert.That(Flow.CurrentChapter1CompletionReceipt,Is.Null);
                yield return FinishClosingLine(lines,line);
            }
            yield return WaitPhase(Chapter1RunPhase.ChapterCompleted);
            var receipt=Flow.CurrentChapter1CompletionReceipt;Assert.That(receipt,Is.Not.Null);Assert.That(receipt.Demo.Branch,Is.EqualTo((ProfileBranch)branch));
            Assert.That(receipt.Save.CommittedRevision,Is.EqualTo(receipt.Save.ExpectedRevision));Assert.That(receipt.Closing.UiGeneration,Is.EqualTo(receipt.Demo.Generation+1));
            Assert.That(receipt.Closing.FinalAdvanceOrdinal,Is.GreaterThanOrEqualTo(5));
            Assert.That(ClosingStatus(),Does.Contain("1챕터 완료"));Assert.That(ClosingStatus(),Does.Contain("아직 열리지 않은 길"));
            }finally{_closingFirstLineEdge=false;_closingInputDiagnostic?.Close();}
        }
        internal static string[] ClosingLines(int branch)=>branch==1?new[]{"도언. 나는 아인을 찾고 있어. 언니에게 네가 무엇을 맡겼는지도 알아야겠어.","유담의 거부를 무시했군. 네 사정만 듣고 넘어가지는 않겠어.","내가 설명하지 못한 위험이 있었어. 아인을 찾고, 내가 한 일에 답하겠다.","네 말을 믿을지는 내가 판단해. 지금은 네가 남긴 흔적부터 확인하겠어.","봉쇄선 너머로 길이 이어집니다. 도언은 아인을 찾고 자신의 책임을 확인하기 위해 걸음을 옮깁니다."}:new[]{"도언. 나는 아인을 찾고 있어. 언니에게 네가 무엇을 맡겼는지도 알아야겠어.","유담에게 선택을 남겼군. 하지만 그것만으로 네 책임이 사라지지는 않아.","내가 설명하지 못한 위험이 있었어. 아인을 찾고, 내가 한 일에 답하겠다.","네 말을 믿을지는 내가 판단해. 지금은 네가 남긴 흔적부터 확인하겠어.","봉쇄선 너머로 길이 이어집니다. 도언은 아인을 찾고 자신의 책임을 확인하기 위해 걸음을 옮깁니다."};
        internal IEnumerator Close()
        {LogStage("trace.close-before",Flow);Keyboard();Pointer(Vector2.zero,false);if(_scene.IsValid() && _scene.isLoaded){foreach(var root in _scene.GetRootGameObjects())root.SetActive(false);yield return SceneManager.UnloadSceneAsync(_scene);if(_scene.isLoaded)throw new InvalidOperationException("시험 장면 정리가 끝나지 않았습니다.");}Flow=null;LogStage("trace.close-after",Flow);}
        internal IEnumerator CloseForReopen()
        {
            LogBoundary("reopen-close-before");
            var closures=new System.Collections.Generic.List<Func<bool>>();
            if(Flow!=null)
            {
                var hub=Flow.CurrentHub;if(hub!=null && hub.Router!=null)closures.Add(hub.Router.GetChapter1HubClosureReader());
                if(Flow.Lifetime!=null && Flow.Lifetime.CurrentRouter!=null && (hub==null || !ReferenceEquals(Flow.Lifetime.CurrentRouter,hub.Router)))closures.Add(Flow.Lifetime.CurrentRouter.GetChapter1ClosureReader());
            }
            yield return Close();yield return null;
            foreach(var closed in closures)Assert.That(closed(),Is.True,"이전 정확 router가 닫히기 전에 장치를 제거하지 않습니다.");
            if(_keyboard!=null && _keyboard.added)InputSystem.RemoveDevice(_keyboard);
            if(_mouse!=null && _mouse.added)InputSystem.RemoveDevice(_mouse);
            _keyboard=null;_mouse=null;LogBoundary("reopen-owned-devices-removed");
        }
        private static int _boundaryRows,_boundaryBytes;
        private static int[] _boundaryGroups=new int[4];
        private int _diagnosticCause=-1;
        private void LogBoundary(string stage,bool terminal=false)
        {
            var group=_diagnosticCause>=0 && _diagnosticCause<=2?_diagnosticCause+1:0;
            var limit=group==0||terminal?8:7;if(_boundaryRows>=32 || _boundaryGroups[group]>=limit)return;
            try
            {
                var state="diagnosticGroup="+group+" runId="+TestContext.CurrentContext.Test.ID+" evidenceRunId="+(_evidenceRunId??"unobserved");
                if(Flow!=null)
                {
                    var view=Flow.View;state+=" seed="+view.Seed+" room="+view.RoomId+" run="+view.RunKey+" phase="+view.Phase+" generation="+view.Generation+" tick="+(view.Tick.HasValue?view.Tick.Value.Value.ToString():"unobserved")+" health="+view.Health+" acceptedFailure="+Flow.AcceptedFailureKind;
                    if(Flow.Lifetime!=null && Flow.Lifetime.TryGetCompletedEvidence(out var actual))state+=" completedTick="+actual.Tick.Value+" position="+actual.Position+" completedHealth="+actual.Health;
                    var hub=Flow.CurrentHub;if(hub!=null && hub.Router!=null){var router=hub.Router;state+=" prepared="+hub.PreparedForUi+" neutral="+router.Chapter1HubNeutralReady+" neutralUpdate="+router.Chapter1HubNeutralInputUpdate+" frameUpdate="+router.Chapter1HubFrameInputUpdate+" blocked="+router.Chapter1HubPreparationBlocked;}
                }
                state+=" keyboard="+(_keyboard==null?"none":_keyboard.deviceId+"/added="+_keyboard.added+"/enabled="+_keyboard.enabled+"/enter="+_keyboard.enterKey.isPressed+"/wasd="+_keyboard.wKey.isPressed+","+_keyboard.aKey.isPressed+","+_keyboard.sKey.isPressed+","+_keyboard.dKey.isPressed+"/arrows="+_keyboard.upArrowKey.isPressed+","+_keyboard.downArrowKey.isPressed+","+_keyboard.leftArrowKey.isPressed+","+_keyboard.rightArrowKey.isPressed);
                state+=" mouse="+(_mouse==null?"none":_mouse.deviceId+"/added="+_mouse.added+"/enabled="+_mouse.enabled+"/left="+_mouse.leftButton.isPressed+"/right="+_mouse.rightButton.isPressed+"/position="+_mouse.position.ReadValue());
                var line="[CH1Boundary] stage="+stage+" frame="+Time.frameCount+" updateType="+InputState.currentUpdateType+" updateCount="+InputState.updateCount+" "+state;
                while(System.Text.Encoding.UTF8.GetByteCount(line)>2047)line=line.Substring(0,line.Length/2);
                var bytes=System.Text.Encoding.UTF8.GetByteCount(line)+1;if(_boundaryBytes+bytes>65536)return;
                _boundaryRows++;_boundaryGroups[group]++;_boundaryBytes+=bytes;Debug.Log(line);
            }
            catch(Exception error){try{if(_boundaryRows<32){var line="[CH1Boundary] stage="+stage+" observationRejected="+error.GetType().FullName;var bytes=System.Text.Encoding.UTF8.GetByteCount(line)+1;if(_boundaryBytes+bytes<=65536){_boundaryRows++;_boundaryGroups[group]++;_boundaryBytes+=bytes;Debug.Log(line);}}}catch{}}
        }
        private static int _stageRows;
        internal static void ResetStages(){_stageRows=0;_boundaryRows=0;_boundaryBytes=0;_boundaryGroups=new int[4];}
        internal static void LogStage(string stage,Chapter1PlayableFlowOwner flow=null)
        {
            if(_stageRows++>=16)return;
            var state="none";
            try{if(flow!=null){var hub=flow.CurrentHub;state="phase="+flow.View.Phase+" error="+flow.View.Error+" hub="+(hub!=null)+" ready="+(hub!=null&&hub.Ready)+" prepared="+(hub!=null&&hub.PreparedForUi);}}
            catch(Exception error){state="state-read-rejected="+error.GetType().Name+":"+error.Message;}
            var line=("[CH1TestStage] stage="+stage+" utc="+DateTime.UtcNow.ToString("O")+" wall="+System.Diagnostics.Stopwatch.GetTimestamp()+" frame="+Time.frameCount+" fixed="+Time.fixedTimeAsDouble.ToString("R")+" inputUpdate="+InputState.updateCount+" background="+Application.runInBackground+" focus="+Application.isFocused+" batch="+Application.isBatchMode+" timeScale="+Time.timeScale+" "+state);
            while(System.Text.Encoding.UTF8.GetByteCount(line)>4096)line=line.Substring(0,line.Length/2);Debug.Log(line);
        }
    }
    // REQ/AC-AUTOCH1-012: 시험 장치의 명령만 발행하며 생산 상태를 변경하지 않는다.
    [DefaultExecutionOrder(-10000)]
    internal sealed class Chapter1InitialSocketObserver:MonoBehaviour
    {
        internal Chapter1AuthoredInputTrace Trace;
        private readonly System.Collections.Generic.HashSet<long> _seen=new System.Collections.Generic.HashSet<long>();
        private string _error;
        private void FixedUpdate()
        {
            if(Trace==null || Trace.Flow==null || _error!=null)return;
            try
            {
                Trace.RecordCompletedCorrelation();var flow=Trace.Flow;var owner=flow.Lifetime;if(owner==null)return;var router=owner.CurrentRouter;if(router==null)return;
                var generation=owner.Generation;if(_seen.Contains(generation))return;
                var view=flow.View;if(view.Phase!=Chapter1RunPhase.StartingRoom && view.Phase!=Chapter1RunPhase.LeavingRoom && view.Phase!=Chapter1RunPhase.InRoom && view.Phase!=Chapter1RunPhase.AtBoss)return;
                var graph=router.transform.parent;if(graph==null || !ReferenceEquals(owner.CurrentRouter,router))throw new InvalidOperationException("관찰 graph 신원이 다릅니다.");
                var player=router.BoundPlayerForAuthoring;
                if(owner.TryGetCompletedEvidence(out var completed) || !player.TryReadExistingMotionForAuthoring(out var motion,out var next) || motion.Tick!=next || next!=player.Chapter1ConfiguredFirstTick)
                    throw new InvalidOperationException("첫 생산 전 실제 모터/완료 부재를 관측하지 못했습니다.");
                if(view.RunKey==null || generation!=view.RoomIndex+1)throw new InvalidOperationException("소켓 원정/세대 신원이 다릅니다.");
                if(view.RoomId=="Ordan"){Trace.RecordEvidence("boss-pristine","run="+view.RunKey+" generation="+generation+" graph="+graph.GetEntityId()+" next="+next.Value);_seen.Add(generation);return;}
                if(_seen.Count>=4)throw new InvalidOperationException("방 소켓 관측 상한입니다.");
                var plan=flow.GetComponent<Chapter1RoomCatalog>().ReadPlan(view.Seed);var socket=plan.SocketAt(view.RoomIndex);
                var room=graph.GetComponent<Chapter1RoomInstance>();if(room==null || room.RoomId!=view.RoomId)throw new InvalidOperationException("실제 방 원본이 다릅니다.");
                var walker=graph.Find("Walker");var surveyor=graph.Find("Surveyor");if(walker==null || surveyor==null)throw new InvalidOperationException("실제 소켓 대상이 없습니다.");
                var w=walker.localPosition;var v=surveyor.localPosition;var ew=room.WalkerSocket(socket);var ev=room.SurveyorSocket(socket);
                Trace.RecordEvidence("socket-pristine","run="+view.RunKey+" generation="+generation+" graph="+graph.GetEntityId()+" router="+router.GetEntityId()+" tick="+next.Value+" room="+room.RoomId+" socket="+socket+" walker="+w.ToString("R")+" surveyor="+v.ToString("R")+" expectedWalker="+ew+" expectedSurveyor="+ev);
                if((Vector2)w!=ew || (Vector2)v!=ev || w.z!=0 || v.z!=0)throw new InvalidOperationException("실제 초기 소켓 배치가 다릅니다.");
                _seen.Add(generation);
            }
            catch(Exception error){_error=error.Message;Trace.RecordEvidence("socket-failed",_error);}
        }
        internal void ValidateComplete(){if(_error!=null || _seen.Count!=5)throw new InvalidOperationException("초기 소켓 미관측/불일치: "+_error+" count="+_seen.Count);}
    }
    internal sealed class Chapter1ProductionRoomPilot
    {
        internal readonly struct Command
        {
            internal Command(Key[] keys,Vector2 pointer,bool attack,string target,int targetHealth,string attackCode,int observedTick=-1)
            {Keys=keys;Pointer=pointer;Attack=attack;Target=target;TargetHealth=targetHealth;AttackCode=attackCode;ObservedTick=observedTick;}
            internal Key[] Keys {get;}internal Vector2 Pointer {get;}internal bool Attack {get;}
            internal string Target {get;}internal int TargetHealth {get;}internal string AttackCode {get;}
            internal int ObservedTick {get;}
        }
        internal InputRouter Router {get;}
        private readonly System.Diagnostics.Stopwatch _watch=System.Diagnostics.Stopwatch.StartNew();
        private double _lastProgress;private float _bestX;private int _lowestEnemyHealth=int.MaxValue,_firstTick=-1;
        private readonly JumpHold _jumpHold=new JumpHold();private bool _traversalJumpIssued,_crushCrossing;
        internal Chapter1ProductionRoomPilot(InputRouter router){Router=router;}
        internal Command Read(int ordinal,bool allowJump)
        {
            var graph=Router.transform.parent;var combat=graph.GetComponentInChildren<CombatSimulationDriver>(true);
            if(combat==null)throw new InvalidOperationException("시험 입력의 실제 일반 전투 원본이 없습니다.");
            if(!combat.HasLatestOutcomeForEncounterHandoff)return new Command(Array.Empty<Key>(),Vector2.zero,false,"",-1,"unpublished");
            var outcome=combat.LatestOutcomeForEncounterHandoff;var player=Router.BoundPlayerForAuthoring;var pose=player.Snapshot;
            if(_firstTick<0)_firstTick=pose.Tick.Value;
            if(pose.Tick.Value-_firstTick>=3000)throw new InvalidOperationException("시험 입력 사례의 3000 완료 틱 상한입니다.");
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
            var attacking=false;
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
                }
            }
            else
            {
                keys.Add(Key.D);if((pose.Tick.Value-_firstTick)%90==45)keys.Add(Key.LeftShift);
            }
            var room=graph.GetComponent<Chapter1RoomInstance>();
            if(room==null)throw new InvalidOperationException("시험 이동 정책의 실제 방 메타가 없습니다.");
            var local=room.transform.InverseTransformPoint(new Vector3(x,y,0));
            var ceiling=IsLowCeiling(room.RoomId,local.x);
            if(allowJump && room.RoomId=="R05" && local.x>=10.5f && local.x<15.2f)
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
                foreach(var terrain in room.Terrain)
                {
                    var ahead=direction>0?terrain.xMin-local.x:local.x-terrain.xMax;
                    if(NeedsStepJump(ahead,foot,terrain.yMin,terrain.yMax))obstacle=true;
                }
                var floors=room.Floors;
                for(var n=0;n+1<floors.Length;n++)
                {
                    var takeoff=direction>0?floors[n].xMax:floors[n+1].xMin;
                    var ahead=direction>0?takeoff-local.x:local.x-takeoff;
                    if(NeedsGapJump(ahead,floors[n+1].xMin-floors[n].xMax))obstacle=true;
                }
            }
            var wantsJump=pose.WallSide!=AcadeGameMaker.Movement.WallSide.None ||
                (pose.IsGrounded && (obstacle || (targetId.Length!=0&&Math.Abs(aim.y-y)>1.8f) || (targetId.Length==0&&!_traversalJumpIssued)));
            if(_jumpHold.Read(pose.Tick.Value,wantsJump,!allowJump||ceiling)){keys.Add(Key.Space);_traversalJumpIssued=true;}
            var code=outcome.Attack.Attempt.HasValue?outcome.Attack.Attempt.Value.Code.ToString():"none";
            return new Command(keys.ToArray(),pointer,attacking,targetId,targetHealth,code,outcome.Tick.Value);
        }
        internal static int ApproachDirection(float dx)=>Math.Abs(dx)>3.5f?(dx>0?1:-1):Math.Abs(dx)<2f?(dx>0?-1:1):0;
        internal static bool ShouldAttack(float squaredDistance,double u,double v,int ordinal)=>squaredDistance<=30.25f&&u>=0&&u<=1&&v>=0&&v<=1&&ordinal%2==0;
        internal static bool NeedsStepJump(float ahead,float foot,float bottom,float top)=>ahead>=-.5f&&ahead<=1.5f&&bottom<=foot+.05f&&top>foot+.15f;
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

    // REQ/AC-AUTOCH1-NEUTRALPROBE-001: device publication only, no game/UI consumption claim.
    public sealed class Chapter1InputNeutralPublicationPlayModeTests : InputTestFixture
    {
        [UnityTest] public IEnumerator NeutralOnceWithSeparateUpdates(){yield return ProbeNeutral(0);}
        [UnityTest] public IEnumerator NeutralRepeatedWithSeparateUpdates(){yield return ProbeNeutral(1);}
        [UnityTest] public IEnumerator NeutralOnceWithCombinedUpdate(){yield return ProbeNeutral(2);}
        private IEnumerator ProbeNeutral(int mode)
        {
            Keyboard keyboard=null;Mouse mouse=null;
            var rows=0;var bytes=0;var callbackRows=0;var eventObserved=0;var afterUpdates=0;var dropped=0;var recordFailures=0;
            var subscribedEvent=false;var subscribedAfter=false;var bodySucceeded=false;var neutralObserved=false;var rounds=0;
            Exception cleanupError=null;
            Action<string,bool> emit=(message,summary)=>
            {
                try
                {
                    var text="[CH1NeutralProbe] case="+TestContext.CurrentContext.Test.ID+" mode="+mode+" "+message;
                    while(System.Text.Encoding.UTF8.GetByteCount(text)>1780)text=text.Substring(0,text.Length-1);
                    var length=System.Text.Encoding.UTF8.GetByteCount(text)+1;
                    if((!summary&&(rows>=31||bytes+length>65536-1800))||rows>=32||length>1800||bytes+length>65536){dropped++;return;}
                    rows++;bytes+=length;Debug.Log(text);
                }catch{recordFailures++;}
            };
            Action<string> snapshot=stage=>emit("stage="+stage+" frame="+Time.frameCount+" updateCount="+InputState.updateCount+" updateType="+InputState.currentUpdateType+
                " keyboardId="+(keyboard==null?-1:keyboard.deviceId)+" mouseId="+(mouse==null?-1:mouse.deviceId)+
                " enter="+(keyboard!=null&&keyboard.enterKey.isPressed)+" right="+(mouse!=null&&mouse.rightButton.isPressed)+" productionUiConsumption=unobserved",false);
            Action<InputEventPtr,InputDevice> onEvent=(value,device)=>
            {
                if(device==null || (device!=keyboard&&device!=mouse))return;
                eventObserved++;
                if(callbackRows>=12){dropped++;return;}callbackRows++;
                emit("stage=onEventObserved deviceId="+value.deviceId+" type="+value.type+" time="+value.time.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+" handled="+value.handled+" updateCount="+InputState.updateCount+" updateType="+InputState.currentUpdateType,false);
            };
            Action afterUpdate=()=>
            {
                afterUpdates++;
                if(callbackRows>=12){dropped++;return;}callbackRows++;snapshot("afterUpdateHeld");
            };
            Action neutral=()=>
            {
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());
                if(mode!=2)InputSystem.Update();
                InputSystem.QueueStateEvent(mouse,new MouseState{position=Vector2.zero});InputSystem.Update();
            };
            try
            {
                keyboard=InputSystem.AddDevice<Keyboard>();mouse=InputSystem.AddDevice<Mouse>();
                InputSystem.onEvent+=onEvent;subscribedEvent=true;InputSystem.onAfterUpdate+=afterUpdate;subscribedAfter=true;
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Enter));InputSystem.Update();
                InputSystem.QueueStateEvent(mouse,new MouseState{position=Vector2.zero}.WithButton(MouseButton.Right));InputSystem.Update();
                snapshot("held-setup");Assert.That(keyboard.enterKey.isPressed,Is.True);Assert.That(mouse.rightButton.isPressed,Is.True);
                neutral();snapshot("neutral-issued");
                for(var i=0;i<8;i++)
                {
                    if(mode==1)neutral();
                    yield return null;yield return new WaitForFixedUpdate();rounds++;snapshot("round="+rounds);
                    if(!keyboard.enterKey.isPressed&&!mouse.rightButton.isPressed){neutralObserved=true;break;}
                }
                Assert.That(neutralObserved,Is.True,"device neutral was not observed within eight render/fixed rounds");
                Assert.That(recordFailures,Is.Zero,"neutral probe recording failed");bodySucceeded=true;
            }
            finally
            {
                try{if(subscribedEvent)InputSystem.onEvent-=onEvent;}catch(Exception e){cleanupError=e;}
                try{if(subscribedAfter)InputSystem.onAfterUpdate-=afterUpdate;}catch(Exception e){if(cleanupError==null)cleanupError=e;}
                try{if(mouse!=null&&mouse.added)InputSystem.RemoveDevice(mouse);}catch(Exception e){if(cleanupError==null)cleanupError=e;}
                try{if(keyboard!=null&&keyboard.added)InputSystem.RemoveDevice(keyboard);}catch(Exception e){if(cleanupError==null)cleanupError=e;}
                emit("stage=finally-summary rounds="+rounds+" neutralObserved="+neutralObserved+" onEventObserved="+eventObserved+" afterUpdates="+afterUpdates+" rowsBefore="+rows+" bytesBefore="+bytes+" dropped="+dropped+" recordingFailures="+recordFailures+" cleanupError="+(cleanupError==null?"none":cleanupError.GetType().Name)+" originalFailurePreserved="+(!bodySucceeded)+" productionUiConsumption=unobserved",true);
                if(cleanupError!=null&&bodySucceeded)throw cleanupError;
            }
        }
    }
    // AC-SOP-001..005: negative value boundaries only; no forged runtime authorization.
    internal sealed class Chapter1StandaloneOwnerPolicyTests
    {
        [Test] public void RejectsDefaultConstructorOwner()
        {Assert.That(Chapter1StandaloneVerificationSession.MatchesRequest(null,new object(),false,false,false),Is.False);}
        [Test] public void RejectsUninitializedOrDuplicateOwner()
        {var owner=new object();var session=new Chapter1StandaloneVerificationSession(owner,"branch1");Assert.That(Chapter1StandaloneVerificationSession.MatchesRequest(owner,owner,false,false,false),Is.False);Assert.That(session.Bind(null),Is.False);session.Revoke();Assert.That(session.Active,Is.False);}
        [Test] public void RejectsWrongPilotOrReplayedSession()
        {Assert.That(Chapter1StandaloneVerificationSession.MatchesRequest(new object(),new object(),true,false,false),Is.False);var session=new Chapter1StandaloneVerificationSession(new object(),"branch1");session.Revoke();Assert.That(session.Bind(null),Is.False);}
        [Test] public void RejectsUnsupportedOrFailureRetryMode()
        {foreach(var mode in new[]{"failure-retry","","branch3"})Assert.That(Chapter1StandaloneVerificationSession.Admitted(mode,false,false,true,true,true),Is.False);}
        [Test] public void RejectsEditorDevelopmentOrUnpreparedSave()
        {Assert.That(Chapter1StandaloneVerificationSession.Admitted("branch1",true,false,true,true,true),Is.False);Assert.That(Chapter1StandaloneVerificationSession.Admitted("branch2",false,true,true,true,true),Is.False);Assert.That(Chapter1StandaloneVerificationSession.Admitted("branch1",false,false,false,true,true),Is.False);Assert.That(Chapter1StandaloneVerificationSession.Admitted("branch1",false,false,true,false,true),Is.False);Assert.That(Chapter1StandaloneVerificationSession.Admitted("branch1",false,false,true,true,false),Is.False);}
        [Test] public void RejectsMissingOwnedEventOrStateMismatch()
        {Assert.That(Chapter1StandaloneVerificationSession.ObservationValid(true,true,false,true,true,true,false,true),Is.False);Assert.That(Chapter1StandaloneVerificationSession.ObservationValid(true,true,true,false,true,true,false,true),Is.False);Assert.That(Chapter1StandaloneVerificationSession.ObservationValid(true,true,true,true,false,true,false,true),Is.False);Assert.That(Chapter1StandaloneVerificationSession.ObservationValid(true,true,true,true,true,false,false,true),Is.False);}
        [Test] public void RejectsForeignInputOrFocusEpochChange()
        {
            Assert.That(Chapter1StandaloneVerificationSession.ObservationValid(true,true,true,true,true,true,true,true),Is.False);
            Assert.That(Chapter1StandaloneVerificationSession.ObservationValid(true,true,true,true,true,true,false,false),Is.False);
            // REQ/AC-SOP-002: pure lifetime objects never attach a Driver or grant Editor authority.
            foreach(var mode in new[]{"branch1","branch2"})
            {
                var session=new Chapter1StandaloneVerificationSession(new object(),mode);
                Assert.That(session.RevokeForFocusChange(true,true),Is.False);
                Assert.That(session.Active,Is.True);
                Assert.That(session.RevokeForFocusChange(true,false),Is.True);
                Assert.That(session.Active,Is.False);
                Assert.That(session.RevokeForFocusChange(false,true),Is.True);
                Assert.That(session.Active,Is.False);
                Assert.That(session.RevokeForFocusChange(true,true),Is.False);
                Assert.That(session.Active,Is.False);
                Assert.That(Chapter1StandaloneVerificationSession.ObservationValid(session.Active,true,true,true,true,true,false,true),Is.False);
            }
        }
        [Test] public void RevokeOnCloseDisposeOrDeviceReplacement()
        {var session=new Chapter1StandaloneVerificationSession(new object(),"branch1");session.Revoke();session.Revoke();Assert.That(session.Active,Is.False);Assert.That(Chapter1StandaloneVerificationSession.MatchesRequest(session,session,true,true,false),Is.False);Assert.That(Chapter1StandaloneVerificationSession.MatchesRequest(session,session,true,false,true),Is.False);Assert.That(Chapter1StandaloneVerificationSession.ObservationValid(true,false,true,true,true,true,false,true),Is.False);}
    }

}
