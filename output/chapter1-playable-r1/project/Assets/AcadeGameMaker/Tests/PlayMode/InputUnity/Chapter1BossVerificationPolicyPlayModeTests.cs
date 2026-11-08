// AC-BVP-001~007: pure counterexamples and naturally scheduled authored flow.
using System;
using System.Collections;
using System.Collections.Generic;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Input.Unity;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    internal sealed class Chapter1BossVerificationPolicyPlayModeTests
    {
        private Scene _loaded;private OwnedBatchInputIsolation _isolation;[SerializeField] private bool _receiptBodyEntered,_receiptBodyCompleted,_receiptExpectedFailureReached;
        [SerializeField] private OwnedBatchRuntimeFixtureScope _fixture=new OwnedBatchRuntimeFixtureScope();
        [SerializeField] private string _entryInstance;
        private void EntryWitness(string stage)
        {Debug.Log("[RTIME_ENTRY] "+JsonUtility.ToJson(new EntryRecord{Stage=stage,CaseName=TestContext.CurrentContext.Test.FullName,CaseId=TestContext.CurrentContext.Test.ID,Instance=_entryInstance}));}
        [Serializable] private sealed class EntryRecord{public string Stage,CaseName,CaseId,Instance;}
        [UnitySetUp] public IEnumerator Enter()
        {
            _entryInstance=Guid.NewGuid().ToString("N");EntryWitness("enter-start");
            try{_fixture.PrepareEnter(TestContext.CurrentContext.Test.FullName);yield return new EnterPlayMode();EntryWitness("enter-play-returned");_fixture.ConfirmEntered();yield return _fixture.RunBody(_fixture.BeginCase(false));_receiptBodyEntered=false;_receiptBodyCompleted=false;_receiptExpectedFailureReached=false;_fixture.RecordCaseInputWitness("case-ready",false,false,false);EntryWitness("enter-complete");Debug.Log("[BVP] entered");}
            finally{EntryWitness("enter-finally");}
        }
        // REQ/AC-RTIME-003: synchronous rollback precedes every asynchronous teardown.
        private sealed class CleanupStep
        {internal readonly string Name;internal bool Attempted,Completed,Failed,Disposed;internal IEnumerator Routine;internal string ErrorType;internal CleanupStep(string name){Name=name;}}
        private void CleanupFailure(CleanupStep step,Exception error)
        {step.Failed=true;step.ErrorType=error.GetType().FullName;_fixture.CaptureCleanupFailure(()=>System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw());}
        private void CleanupSync(CleanupStep step,Action action)
        {if(step.Attempted)return;step.Attempted=true;try{action();step.Completed=true;}catch(Exception error){CleanupFailure(step,error);}}
        private void DisposeCleanup(CleanupStep step)
        {if(step.Disposed || step.Routine==null)return;step.Disposed=true;try{(step.Routine as IDisposable)?.Dispose();}catch(Exception error){step.Completed=false;CleanupFailure(step,error);}}
        private IEnumerator CleanupRoutine(CleanupStep step,Func<IEnumerator> create)
        {
            if(step.Attempted)yield break;step.Attempted=true;var exhausted=false;
            try
            {
                try{step.Routine=create();if(step.Routine==null)throw new InvalidOperationException("정리 열거기 부재입니다.");}catch(Exception error){CleanupFailure(step,error);}
                while(!step.Failed)
                {
                    object current=null;var next=false;try{next=step.Routine.MoveNext();if(next)current=step.Routine.Current;}catch(Exception error){CleanupFailure(step,error);}
                    if(step.Failed)break;if(!next){exhausted=true;break;}yield return current;
                }
            }
            finally{DisposeCleanup(step);step.Completed=exhausted && !step.Failed;}
        }
        private IEnumerator UnloadProductionScene()
        {var operation=SceneManager.UnloadSceneAsync(_loaded);if(operation==null)throw new InvalidOperationException("장면 해제 연산 부재입니다.");yield return operation;if(_loaded.IsValid() && _loaded.isLoaded)throw new InvalidOperationException("장면 해제 미완료입니다.");}
        private void CloseCaseFirstStep(CleanupStep step)
        {
            if(step.Attempted){DisposeCleanup(step);return;}step.Attempted=true;
            try{step.Routine=_fixture.CloseCase();if(step.Routine==null)throw new InvalidOperationException("사례 정리 열거기 부재입니다.");step.Routine.MoveNext();}
            catch(Exception error){CleanupFailure(step,error);}finally{DisposeCleanup(step);} // Revocation attempt only; never completed.
        }
        private void CleanupWitness(CleanupStep step)
        {try{Debug.Log("[BVP] cleanup stage="+step.Name+" attempted="+step.Attempted+" completed="+step.Completed+" failed="+step.Failed+" disposed="+step.Disposed+" errorType="+(step.ErrorType??"none"));}catch{}}
        [UnityTearDown] public IEnumerator Exit()
        {
            var restore=new CleanupStep("restore");var confirm=new CleanupStep("confirm");var scene=new CleanupStep("scene");var close=new CleanupStep("close-case");var complete=new CleanupStep("complete-exit");var witness=new CleanupStep("case-witness");var checks=new CleanupStep("body-checks");var residual=new CleanupStep("exit-residual");
            var exitAttempted=false;var exitReturned=false;
            try
            {
                if(_isolation!=null){CleanupSync(restore,_isolation.Restore);yield return CleanupRoutine(confirm,_isolation.ConfirmRestored);}else{restore.Completed=true;confirm.Completed=true;}
                if(Application.isPlaying && _loaded.IsValid() && _loaded.isLoaded)yield return CleanupRoutine(scene,UnloadProductionScene);else scene.Completed=!_loaded.IsValid() || !_loaded.isLoaded;
                CleanupSync(witness,()=>_fixture.RecordCaseInputWitness("teardown",_receiptBodyEntered,_receiptBodyCompleted,_receiptExpectedFailureReached));
                if(_fixture.OwnsRuntimeFixture && Application.isPlaying)yield return CleanupRoutine(close,_fixture.CloseCase);
                if(_fixture.OwnsRuntimeFixture && Application.isPlaying){exitAttempted=true;yield return new ExitPlayMode();exitReturned=true;}
                if(_fixture.OwnsRuntimeFixture && !Application.isPlaying && scene.Completed && close.Completed)CleanupSync(complete,_fixture.CompleteExit);
                CleanupSync(checks,()=>{if(TestContext.CurrentContext.Test.FullName.Contains(".OwnedInputIsolationRestores")){Assert.That(_receiptBodyEntered,Is.True,"본문 진입 미관측");Assert.That(_receiptBodyCompleted,Is.True,"본문 필수 조건 완료 미관측");}});
            }
            finally
            {
                // No yield, stop fallback or exception escape: preserve the original body/Exit failure.
                try
                {
                    if(_isolation!=null && !restore.Attempted)CleanupSync(restore,_isolation.Restore);
                    DisposeCleanup(confirm);DisposeCleanup(scene);
                    if(_fixture.OwnsRuntimeFixture)CloseCaseFirstStep(close);
                    if(!complete.Attempted && _fixture.OwnsRuntimeFixture && !Application.isPlaying && scene.Completed && close.Completed)CleanupSync(complete,_fixture.CompleteExit);
                    if(_fixture.OwnsRuntimeFixture || Application.isPlaying)CleanupSync(residual,()=>{throw new InvalidOperationException("소유 시험 종료 미완료입니다.");});
                    if(confirm.Completed)_isolation=null;
                }catch(Exception error){try{CleanupFailure(residual,error);}catch{}}
                foreach(var step in new[]{restore,confirm,scene,close,complete,witness,checks,residual})CleanupWitness(step);
                try{Debug.Log("[BVP] cleanup exitAttempted="+exitAttempted+" exitReturned="+exitReturned+" isPlaying="+Application.isPlaying+" finallyObserved=true");}catch{}
            }
            if(complete.Completed && !residual.Failed)Debug.Log("[BVP] exited");_fixture.ThrowCleanupFailure();
        }
        private static Chapter1VerificationBossPolicy Ready()
        {var p=new Chapter1VerificationBossPolicy();p.Consumed(false,true,1);p.Consumed(false,true,2);return p;}
        private static Chapter1VerificationBossPolicy.Command Read(Chapter1VerificationBossPolicy p,int tick,float x=9,float vx=0,bool aim=true,bool focused=true,int cooldown=0)
        {return p.Read("run",1,tick,x,.81f,vx,true,false,focused,aim,cooldown,OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,0,.35f);}
        [Test] public void CompletedTickIdentityAndDuplicateCalls()
        {var p=Ready();var a=Read(p,10);for(var n=0;n<3;n++){var b=Read(p,10);CollectionAssert.AreEqual(a.Keys,b.Keys);Assert.That(b.Attack,Is.EqualTo(a.Attack));}Assert.Throws<InvalidOperationException>(()=>Read(p,9));Assert.Throws<InvalidOperationException>(()=>p.Read("foreign",2,11,9,1,0,true,false,true,true,0,OrdanPatternSlot.None,OrdanBossPhase.Recovery,0,.35f));}
        [Test] public void AttackRequiresConsumedRelease()
        {var p=Ready();Assert.That(Read(p,10).Attack,Is.True);p.Consumed(true,false,3);Assert.That(Read(p,11).Attack,Is.False);p.Consumed(false,true,4);Assert.That(Read(p,12,cooldown:20).Attack,Is.False);Assert.That(Read(p,20,aim:false).Attack,Is.False);Assert.That(Read(p,21).Attack,Is.True);}
        [Test] public void FocusResumeRequiresTwoNeutralUpdates()
        {var p=Ready();Read(p,1);p.Focus(false);Assert.That(Read(p,2,focused:false).Attack,Is.False);p.Focus(true);p.Consumed(false,true,3);Assert.That(Read(p,3).Attack,Is.False);p.Consumed(false,true,4);Assert.That(Read(p,4).Attack,Is.True);}
        [Test] public void ApprovedGeometryBoundariesAndTimingOffsets()
        {
            foreach(var x in new[]{8.75f,9f,9.25f})Assert.That(Chapter1VerificationBossPolicy.RightRegionClear((int)(x*4096),3318),Is.True);
            Assert.That(Chapter1VerificationBossPolicy.RightRegionClear(31335,3318),Is.False);Assert.That(Chapter1VerificationBossPolicy.RightRegionClear(31336,3318),Is.True);
            Assert.That(Chapter1VerificationBossPolicy.RightRegionClear(47104-1639,3318),Is.False);
            // Every stage and normal/accelerated authored trajectory duration, using realistic x/y.
            foreach(var stage in new[]{OrdanHealthStage.Stage1,OrdanHealthStage.Stage2,OrdanHealthStage.Stage3})foreach(var accelerated in new[]{false,true})
            {
                var duration=(accelerated?OrdanBossHostileDamageProducer.ApprovedAcceleratedDurations:OrdanBossHostileDamageProducer.ApprovedNormalDurations)[(int)stage-1];
                var threat=new Chapter1VerificationBossPolicy.Threat(stage,accelerated,false,false,false,false,28672,4915,410,duration);
                foreach(var age in new[]{0,1,duration-2,duration-1})foreach(var offset in new[]{-1,0,1})
                {
                    var p=Ready();var c=p.Read("run",1,100+offset,9,.81f,0,true,false,true,true,101,OrdanPatternSlot.SeizureWeight,OrdanBossPhase.Execute,age,.35f,0,threat);
                    Assert.That(p.Unresolved,Is.Null);Assert.That(c.Keys,Is.Empty);Assert.That(c.Attack,Is.EqualTo(offset>=1),"cooldown ±1");
                }
                var bad=new Chapter1VerificationBossPolicy.Threat(stage,accelerated,false,false,false,false,28672,4915,410,duration+1);
                int direction;bool jump;string reason;Assert.That(Chapter1VerificationBossPolicy.TrySafeIntent(9,.81f,0,0,true,false,OrdanPatternSlot.SeizureWeight,OrdanBossPhase.Execute,0,.35f,bad,out direction,out jump,out reason),Is.False);
                Assert.That(Chapter1VerificationBossPolicy.TrySafeIntent(0,.81f,0,0,true,false,OrdanPatternSlot.SeizureWeight,OrdanBossPhase.Telegraph,59,.35f,threat,out direction,out jump,out reason),Is.False);
            }
            foreach(var slot in new[]{OrdanPatternSlot.DebtLineA,OrdanPatternSlot.DebtLineB})foreach(var age in new[]{0,1,17,18,19,46,47,48})
            {
                var p=Ready();var c=p.Read("run",1,100,9,.81f,0,true,false,true,true,0,slot,OrdanBossPhase.Telegraph,age,.35f);
                Assert.That(p.Unresolved,Is.Null);Assert.That(c.Keys,Is.Empty);
            }
            foreach(var offset in new[]{-1,0,1})
            {
                var p=Ready();var threat=new Chapter1VerificationBossPolicy.Threat(OrdanHealthStage.Stage2,false,false,false,false,false,28672,4915,410,66);
                var c=p.Read("run",1,60+offset,0,.81f,0,true,false,true,true,0,OrdanPatternSlot.SeizureWeight,offset<0?OrdanBossPhase.Telegraph:OrdanBossPhase.Execute,offset<0?59:offset,.35f,0,threat);
                Assert.That(p.Unresolved,Is.Not.Null);Assert.That(c.Keys,Is.Empty);Assert.That(c.Attack,Is.False);
                var audit=Ready();var auditCommand=audit.Read("run",1,90+offset,9,.81f,0,true,false,true,true,0,OrdanPatternSlot.BalanceAudit,offset<0?OrdanBossPhase.Telegraph:OrdanBossPhase.Execute,offset<0?59:offset,.35f,0,threat);
                Assert.That(audit.Unresolved,Is.Null);Assert.That(auditCommand.Keys,Is.Empty);
            }
            // Actual command attack zone boundaries, one Q4096 unit on either side.
            var q=1f/4096f;
            var left=(OrdanBossHostileDamageProducer.ApprovedOrdanBody.MaxX+OrdanBossHostileDamageProducer.ApprovedPlayerHalfWidth)/4096f+.05f;
            foreach(var x in new[]{left-q,left,left+q,9.25f-q,9.25f,9.25f+q})
            {var policy=Ready();var c=policy.Read("run",1,100,x,.81f,0,true,false,true,true,0,OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,2,.35f);Assert.That(policy.Unresolved,Is.Null);Assert.That(c.Attack,Is.EqualTo(x>=left && x<=9.25f),"actual attack boundary x="+x);}
            // Debt B: an already airborne normal jump can clear the line; do not invent a ground jump proof.
            // The conservative fire-height threshold is quantized to the same production Q4096 unit.
            var bSeconds=6f/60;var bThreshold=3f+3277f/4096+.25f+.02f+15.36f*bSeconds*bSeconds+.256f*bSeconds+.03f;
            var bBoundary=(float)Math.Ceiling(bThreshold*4096)/4096;
            foreach(var y in new[]{bBoundary-q,bBoundary,bBoundary+q})
            {var policy=Ready();var c=policy.Read("run",1,42,-8,y,8,false,false,true,true,0,OrdanPatternSlot.DebtLineB,OrdanBossPhase.Telegraph,42,.02f,0);Assert.That(policy.Unresolved==null,Is.EqualTo(y>=bBoundary),"airborne Debt B Q4096 height="+y);Assert.That(c.Attack,Is.False);if(y<bBoundary)Assert.That(c.Keys,Is.Empty);}
            foreach(var age in new[]{19,20,21})
            {var policy=Ready();var c=policy.Read("run",1,age,0,3318f/4096,0,true,false,true,true,0,OrdanPatternSlot.DebtLineB,OrdanBossPhase.Telegraph,age,1f/60);Assert.That(policy.Unresolved,Is.Not.Null);Assert.That(c.Keys,Is.Empty);}
            // Audit radial command boundary at the actual production ground-centre Q4096 height.
            var radialY=3318f/4096;var radialDy=Math.Abs(radialY-1.2f)+3277f/4096+15.36f*.35f*.35f;
            var radialX=7f-((float)Math.Sqrt(144-radialDy*radialDy)-1639f/4096-.02f);
            var radialBoundary=(float)Math.Floor(radialX*4096)/4096;
            foreach(var x in new[]{radialBoundary-q,radialBoundary,radialBoundary+q})
            {var policy=Ready();var c=policy.Read("run",1,90,x,radialY,0,true,true,true,false,0,OrdanPatternSlot.BalanceAudit,OrdanBossPhase.Execute,0,.35f);Assert.That(policy.Unresolved==null,Is.EqualTo(x>radialBoundary),"actual Audit Q4096 radius boundary="+x);Assert.That(c.Attack,Is.False);if(x<=radialBoundary)Assert.That(c.Keys,Is.Empty);}
            foreach(var bad in new[]{new Chapter1VerificationBossPolicy.Threat(OrdanHealthStage.Stage2,false,false,false,false,true,28672,4916,410,0),new Chapter1VerificationBossPolicy.Threat(OrdanHealthStage.Stage2,false,false,false,false,true,28672,4915,411,0)})
            {var policy=Ready();var c=policy.Read("run",1,90,9,.81f,0,true,false,true,true,0,OrdanPatternSlot.BalanceAudit,OrdanBossPhase.Execute,0,.35f,0,bad);Assert.That(policy.Unresolved,Is.Not.Null);Assert.That(c.Keys,Is.Empty);Assert.That(c.Attack,Is.False);}
            {var policy=Ready();var c=policy.Read("run",1,90,-11,.81f,-8,true,false,true,true,0,OrdanPatternSlot.BalanceAudit,OrdanBossPhase.Execute,0,.35f);Assert.That(policy.Unresolved,Is.Not.Null);Assert.That(c.Keys,Is.Empty);Assert.That(c.Attack,Is.False);}
            var debtThreat=new Chapter1VerificationBossPolicy.Threat(OrdanHealthStage.Stage1,false,false,false,false,false,28672,4915,410,0);
            int d;bool j;string why;
            Assert.That(Chapter1VerificationBossPolicy.TrySafeIntent(0,.81f,0,0,true,false,OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,2,.35f,debtThreat,out d,out j,out why),Is.True);Assert.That(j,Is.True);
            foreach(var age in new[]{0,1,2})
            {var possible=Chapter1VerificationBossPolicy.TrySafeIntent(0,.81f,0,0,true,false,OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,age,.35f,debtThreat,out d,out j,out why);Assert.That(possible,Is.EqualTo(age>=1),"entry jump discrete lower envelope ±1");}
            foreach(var slot in new[]{OrdanPatternSlot.DebtLineA,OrdanPatternSlot.DebtLineB})
            {var p=Ready();var c=p.Read("run",1,1,0,.81f,0,true,false,true,true,0,slot,OrdanBossPhase.Telegraph,47,.35f);Assert.That(p.Unresolved,Is.Not.Null);Assert.That(c.Keys,Is.Empty);Assert.That(c.Attack,Is.False);}
            foreach(var vx in new[]{-20f,-9.5f,8f,20f})
            {var p=Ready();var c=p.Read("run",1,1,vx>0?11f:-11f,.81f,vx,true,false,true,true,0,OrdanPatternSlot.BalanceAudit,OrdanBossPhase.Execute,1,.35f);Assert.That(p.Unresolved,Is.Not.Null);Assert.That(c.Keys,Is.Empty);}
            var pull=new Chapter1VerificationBossPolicy.Threat(OrdanHealthStage.Stage3,false,false,false,false,true,28672,4915,410,0);
            Assert.That(Chapter1VerificationBossPolicy.TrySafeIntent(9,.81f,0,0,true,false,OrdanPatternSlot.BalanceAudit,OrdanBossPhase.Execute,89,.35f,pull,out d,out j,out why),Is.True);
            var badPull=new Chapter1VerificationBossPolicy.Threat(OrdanHealthStage.Stage3,false,false,false,false,true,28673,4915,410,0);
            Assert.That(Chapter1VerificationBossPolicy.TrySafeIntent(9,.81f,0,0,true,false,OrdanPatternSlot.BalanceAudit,OrdanBossPhase.Execute,89,.35f,badPull,out d,out j,out why),Is.False);
            var queued=new Chapter1VerificationBossPolicy.Threat(OrdanHealthStage.Stage1,false,true,true,false,false,28672,4915,410,0);
            Assert.That(Chapter1VerificationBossPolicy.TrySafeIntent(9,.81f,0,0,true,false,OrdanPatternSlot.SeizureWeight,OrdanBossPhase.Execute,1,.35f,queued,out d,out j,out why),Is.False);
        }
        [Test] public void BossDamageTraceCausalityAndLimits()
        {
            Chapter1VerificationBossPolicy.ValidateTrace("q","q",12,12,12,"player","player");
            Assert.Throws<InvalidOperationException>(()=>Chapter1VerificationBossPolicy.ValidateTrace("q","other",12,12,12,"player","player"));
            Assert.Throws<InvalidOperationException>(()=>Chapter1VerificationBossPolicy.ValidateTrace("q","q",11,12,12,"player","player"));
            foreach(DamageResultCode code in Enum.GetValues(typeof(DamageResultCode)))
            {
                var tick=new AcadeGameMaker.Core.SimulationTick(12);var request=new DamageRequest("request","ordan","player",1,DamageKind.BossPattern,tick);var result=new DamageResult("request","player",code==DamageResultCode.Applied?1:0,code,tick);
                var row=Chapter1VerificationBossPolicy.FormatTrace(request,result,12);Assert.That(row,Does.Contain("request=request"));Assert.That(row,Does.Contain("result="+code));Assert.That(row,Does.Contain("applied="+(code==DamageResultCode.Applied?1:0)));
            }
            var b=new Chapter1VerificationBossBudget();var count=0;try{for(;count<257;count++)b.Add("x");}catch(InvalidOperationException){}Assert.That(count,Is.InRange(1,256));
            foreach(var text in new[]{new string('x',8192),new string('"',4000),new string('\\',4000),new string('\n',4000),new string('한',3000)})
                Assert.Throws<InvalidOperationException>(()=>new Chapter1VerificationBossBudget().Add(text));
            var actual=JsonUtility.ToJson(new Wrapper{utc=DateTime.UtcNow.ToString("O"),kind="input",detail="boss "+"\"\\\n한",seconds=179.99999999999997,frame=int.MaxValue});
            Assert.That(Chapter1VerificationBossBudget.SerializedUpperBound("\"\\\n한"),Is.GreaterThanOrEqualTo(System.Text.Encoding.UTF8.GetByteCount(actual)+1));
        }
        [Serializable] private sealed class Wrapper{public string utc,kind,detail;public double seconds;public int frame;}
        [UnityTest] public IEnumerator IsolatedApiBossPolicyFromAuthoredFlow()
        {var context=_fixture.AcquireOwnedInputContext();_isolation=_fixture.BeginOwnedInputIsolation(context);yield return _fixture.RunBody(NaturalBody(context));}
        [UnityTest] public IEnumerator OwnedInputIsolationRestoresAfterNormalBody()
        {EntryWitness("body-enter");_receiptBodyEntered=true;_fixture.RecordCaseInputWitness("body-enter",true,false,false);var context=_fixture.AcquireOwnedInputContext();_isolation=_fixture.BeginOwnedInputIsolation(context);yield return _fixture.RunBody(SmokeBody(context,false));}
        [UnityTest] public IEnumerator OwnedInputIsolationRestoresAfterBodyFailure()
        {EntryWitness("body-enter");_receiptBodyEntered=true;_fixture.RecordCaseInputWitness("body-enter",true,false,false);var context=_fixture.AcquireOwnedInputContext();_isolation=_fixture.BeginOwnedInputIsolation(context);yield return _fixture.RunBody(CaptureExpectedSmoke(SmokeBody(context,true)));}
        // REQ/AC-RTIME-001~003: naturally driven public API cases, no direct Update.
        [UnityTest,Timeout(20000)] public IEnumerator OwnedInputIsolationRestoresWithAdvancingFixtureClock(){yield return ReceiptTimeCase("clock");}
        [UnityTest,Timeout(20000)] public IEnumerator OwnedInputIsolationRestoresWithTimestampMutationRejected(){yield return ReceiptTimeCase("time");}
        [UnityTest,Timeout(20000)] public IEnumerator OwnedInputIsolationRestoresWithUnexpectedQueuedIdRejected(){yield return ReceiptTimeCase("id");}
        [UnityTest,Timeout(20000)] public IEnumerator OwnedInputIsolationRestoresWithDeviceMutationRejected(){yield return ReceiptTimeCase("device");}
        [UnityTest,Timeout(20000)] public IEnumerator OwnedInputIsolationRestoresWithReplayedPairRejected(){yield return ReceiptTimeCase("replay");}
        private IEnumerator ReceiptTimeCase(string kind)
        {EntryWitness("body-enter");_receiptBodyEntered=true;_fixture.RecordCaseInputWitness("body-enter",true,false,false);var context=_fixture.AcquireOwnedInputContext();_isolation=_fixture.BeginOwnedInputIsolation(context);yield return _fixture.RunBody(ReceiptTimeBody(context,kind));}
        [Serializable] private sealed class TimeRecord
        {
            public string Stage,Kind,CaseName,CaseId,Instance,RunId,Phase,SourceSha,OwnerBirth,UpdateType,EventType,Error;
            public long Ordinal,Sequence,Cycle,UpdateCount,TimeBits,AfterTimeBits,OriginalTimeBits,ExpectedTimeBits;
            public int OwnerPid,Id,ExpectedId,DeviceId,CallbackDeviceId,OriginalId,OriginalDeviceId,TraceCount,Size,AlignedSize,TotalSize,ReplayPosition;
            public bool Applied,Pending,ControllerDisposed,TraceDisposed,ListenersRemoved;
        }
        private IEnumerator ReceiptTimeBody(IChapter1OwnedBatchInputContext context,string kind)
        {
            _fixture.RecordCaseInputWitness("owned-body-enter",true,false,false);
            Chapter1VerificationInputPilot pilot=null;Keyboard keyboard=null;Mouse mouse=null;InputEventTrace trace=null;InputEventTrace.ReplayController replay=null;
            var receipts=new List<Chapter1ReceiptObservation>();var originalIds=new List<int>();var originalTimes=new List<long>();var traceSizes=new List<int>();var actualOrder=new List<int>();var expectedOrder=new List<int>();
            bool injecting=true,armed=false,applied=false;string callbackError=null;long ordinal=0;int unexpectedId=0,traceAccepted=0,totalSize=0;Chapter1OwnedBatchInputEvidence identity;
            Assert.That(context.TryObserve(out identity),Is.True);
            void Witness(string stage,InputEventPtr value=default(InputEventPtr),InputDevice device=null,int originalId=0,long originalTime=0,int originalDevice=0)
            {
                var state=pilot==null?default(Chapter1VerificationInputBoundarySnapshot):pilot.CaptureInputBoundarySnapshot();
                var row=new TimeRecord{Stage=stage,Kind=kind,CaseName=identity.CaseName,CaseId=TestContext.CurrentContext.Test.ID,Instance=_entryInstance,RunId=identity.RunId,Phase=identity.Phase,SourceSha=identity.SourceSha,OwnerPid=identity.OwnerPid,OwnerBirth=identity.OwnerBirth,Ordinal=++ordinal,Sequence=state.PublishedSequence,Cycle=state.ArmingCycle,Pending=state.PendingPair,ExpectedId=state.ExpectedKeyboardEventId,UpdateType=InputState.currentUpdateType.ToString(),UpdateCount=InputState.updateCount,Applied=applied,TraceCount=traceAccepted,TotalSize=totalSize,ReplayPosition=replay==null?-1:replay.position,Error=callbackError};
                if(value.valid){row.Id=value.id;row.TimeBits=BitConverter.DoubleToInt64Bits(value.time);row.DeviceId=value.deviceId;row.CallbackDeviceId=device==null?0:device.deviceId;row.EventType=value.type.ToString();row.OriginalId=originalId;row.OriginalTimeBits=originalTime;row.OriginalDeviceId=originalDevice;row.Size=(int)value.sizeInBytes;row.AlignedSize=(row.Size+3)&~3;}
                var json=JsonUtility.ToJson(row);context.Budget.Add("time-contract "+json);Debug.Log("[RTIME] "+json);
            }
            void Before()
            {
                if(!injecting || !armed || applied || pilot==null || InputState.currentUpdateType!=InputUpdateType.Dynamic || (kind!="id" && kind!="replay"))return;
                try
                {
                    Witness("before-injection");
                    if(kind=="id")using(var memory=StateEvent.FromDefaultStateFor(keyboard,out var value)){InputSystem.QueueEvent(value);unexpectedId=value.id;Assert.That(unexpectedId,Is.GreaterThan(0));expectedOrder.Add(keyboard.deviceId);Witness("unexpected-queued",value,keyboard);}
                    else
                    {
                        Assert.That(trace.eventCount,Is.EqualTo(2));Assert.That(traceAccepted,Is.EqualTo(2));Assert.That(trace.totalEventSizeInBytes,Is.EqualTo(totalSize));Assert.That(totalSize,Is.LessThanOrEqualTo(8192));
                        replay=trace.Replay();Assert.That(replay.position,Is.Zero);replay.PlayOneEvent();Assert.That(replay.position,Is.EqualTo(1));Witness("replay-one");replay.PlayOneEvent();Assert.That(replay.position,Is.EqualTo(2));Witness("replay-two");expectedOrder.Add(keyboard.deviceId);expectedOrder.Add(mouse.deviceId);
                    }
                    expectedOrder.Add(keyboard.deviceId);expectedOrder.Add(mouse.deviceId);applied=true;
                }
                catch(Exception error){callbackError=error.ToString();injecting=false;}
            }
            void OnEvent(InputEventPtr value,InputDevice device)
            {
                if(!injecting || pilot==null || value.type!=StateEvent.Type || (!ReferenceEquals(device,keyboard)&&!ReferenceEquals(device,mouse)))return;
                try
                {
                    if(armed && InputState.currentUpdateType==InputUpdateType.Dynamic)
                    {
                        if(kind=="id" || kind=="replay"){actualOrder.Add(device.deviceId);if(actualOrder.Count>4)throw new InvalidOperationException("예상 사건 순서 상한입니다.");Witness("incoming",value,device);}
                        if(!applied && ReferenceEquals(device,keyboard) && (kind=="time" || kind=="device"))
                        {
                            var state=pilot.CaptureInputBoundarySnapshot();Assert.That(state.PendingPair,Is.True);Assert.That(value.id,Is.EqualTo(state.ExpectedKeyboardEventId));Assert.That(state.PublishedSequence,Is.GreaterThan(2));
                            var id=value.id;var time=BitConverter.DoubleToInt64Bits(value.time);var deviceId=value.deviceId;Witness("before-mutation",value,device,id,time,deviceId);
                            if(kind=="time"){Assert.That(value.time,Is.GreaterThan(0));value.time=BitConverter.Int64BitsToDouble(time+1);Assert.That(BitConverter.DoubleToInt64Bits(value.time),Is.EqualTo(time+1));}
                            else{value.deviceId=mouse.deviceId;Assert.That(value.deviceId,Is.EqualTo(mouse.deviceId));Assert.That(device.deviceId,Is.EqualTo(keyboard.deviceId));}
                            applied=true;Witness("mutation-readback",value,device,id,time,deviceId);
                        }
                    }
                }
                catch(Exception error){callbackError=error.ToString();injecting=false;}
            }
            InputSystem.onBeforeUpdate+=Before;InputSystem.onEvent+=OnEvent;
            try
            {
                if(kind=="replay")
                {
                    trace=new InputEventTrace(bufferSizeInBytes:8192,growBuffer:false);trace.recordFrameMarkers=false;
                    trace.onFilterEvent=(value,device)=>
                    {
                        if(!injecting || traceAccepted>=2 || value.type!=StateEvent.Type || (!ReferenceEquals(device,keyboard)&&!ReferenceEquals(device,mouse)))return false;
                        try{Assert.That(device,Is.SameAs(traceAccepted==0?(InputDevice)keyboard:mouse));var size=(int)value.sizeInBytes;var aligned=(size+3)&~3;Assert.That(size,Is.InRange(1,4096));Assert.That(totalSize+aligned,Is.LessThanOrEqualTo(8192));originalIds.Add(value.id);originalTimes.Add(BitConverter.DoubleToInt64Bits(value.time));traceSizes.Add(aligned);totalSize+=aligned;traceAccepted++;Witness("trace-accepted",value,device);return true;}
                        catch(Exception error){callbackError=error.ToString();injecting=false;return false;}
                    };trace.Enable();
                }
                pilot=new Chapter1VerificationInputPilot(row=>{Debug.Log("[BVP] "+row);if(row.StartsWith("receipt ",StringComparison.Ordinal))receipts.Add(JsonUtility.FromJson<Chapter1ReceiptObservation>(row.Substring(8)));},context);
                var initial=pilot.CaptureInputBoundarySnapshot();keyboard=InputSystem.GetDeviceById(initial.KeyboardId) as Keyboard;mouse=InputSystem.GetDeviceById(initial.MouseId) as Mouse;_isolation.BindOwnedDevices(initial);_isolation.EmergencyCleanup=()=>{injecting=false;InputSystem.onBeforeUpdate-=Before;InputSystem.onEvent-=OnEvent;replay?.Dispose();trace?.Disable();trace?.Dispose();pilot.Dispose();};pilot.Neutral();var end=_isolation.StartedReal+20;
                while(!pilot.ContextReady && pilot.Error==null && callbackError==null && Time.realtimeSinceStartupAsDouble<end)yield return null;
                Assert.That(callbackError,Is.Null);Assert.That(pilot.Error,Is.Null);Assert.That(pilot.ContextReady&&pilot.NeutralConsumed,Is.True);Assert.That(receipts.Count,Is.EqualTo(8));Assert.That(receipts[0].ExpectedTimeBits,Is.EqualTo(BitConverter.DoubleToInt64Bits(0d)));Assert.That(BitConverter.Int64BitsToDouble(receipts[4].ExpectedTimeBits),Is.GreaterThan(0));
                if(trace!=null){trace.Disable();Assert.That(trace.eventCount,Is.EqualTo(2));Assert.That(trace.totalEventSizeInBytes,Is.EqualTo(totalSize));Assert.That(traceAccepted,Is.EqualTo(2));Assert.That(originalIds[0],Is.EqualTo(receipts[0].ExpectedId));Assert.That(originalIds[1],Is.EqualTo(receipts[1].ExpectedId));Witness("trace-frozen");}
                var prior=pilot.CaptureInputBoundarySnapshot().ConsumedSequence;
                if(kind=="clock")
                {
                    pilot.TapSubmit();while(pilot.Error==null && Time.realtimeSinceStartupAsDouble<end && pilot.CaptureInputBoundarySnapshot().ConsumedSequence<=prior)yield return null;
                    Assert.That(pilot.Error,Is.Null);Assert.That(pilot.CaptureInputBoundarySnapshot().ConsumedSequence,Is.GreaterThan(prior));Witness("clock-edge-consumed");
                }
                else
                {
                    armed=true;Witness("armed");while(pilot.Error==null && callbackError==null && Time.realtimeSinceStartupAsDouble<end)yield return null;injecting=false;Assert.That(callbackError,Is.Null);Assert.That(applied,Is.True);Assert.That(pilot.Error,Does.Contain("큐 영수증 ID/시간/장치 불일치"));
                    Assert.That(receipts.Count,Is.EqualTo(9));var mismatch=receipts[8];var mask=kind=="time"?2:kind=="device"?4:1;Assert.That(mismatch.Stage,Is.EqualTo("mismatch"));Assert.That(mismatch.MismatchMask,Is.EqualTo(mask));Assert.That(mismatch.EvaluatedMask,Is.EqualTo(mask==1?1:mask==2?3:7));Assert.That(mismatch.Sequence,Is.GreaterThan(2));
                    if(kind=="id"){Assert.That(mismatch.ComparedId,Is.EqualTo(unexpectedId));Assert.That(unexpectedId,Is.Not.EqualTo(mismatch.ExpectedId));}
                    if(kind=="id" || kind=="replay"){CollectionAssert.AreEqual(expectedOrder,actualOrder);Assert.That(mismatch.TimeEvaluated,Is.False);Assert.That(mismatch.DeviceEvaluated,Is.False);}
                    if(kind=="replay"){Assert.That(mismatch.ComparedId,Is.Not.EqualTo(originalIds[0]));Assert.That(mismatch.ComparedId,Is.Not.EqualTo(mismatch.ExpectedId));Assert.That(replay.position,Is.EqualTo(2));}
                    Witness("rejection-confirmed");
                }
                pilot.RecordInputBoundarySnapshot("receipt-time-contract",true);_receiptBodyCompleted=true;_fixture.RecordCaseInputWitness("normal-complete",true,true,false);Witness("contract-complete");
            }
            finally
            {
                injecting=false;armed=false;InputSystem.onBeforeUpdate-=Before;InputSystem.onEvent-=OnEvent;
                try{replay?.Dispose();}finally{try{trace?.Disable();trace?.Dispose();}finally{pilot?.Dispose();_isolation.EmergencyCleanup=null;}}
                Debug.Log("[RTIME] "+JsonUtility.ToJson(new TimeRecord{Stage="cleanup",Kind=kind,CaseName=identity.CaseName,CaseId=TestContext.CurrentContext.Test.ID,Instance=_entryInstance,RunId=identity.RunId,Phase=identity.Phase,SourceSha=identity.SourceSha,OwnerPid=identity.OwnerPid,OwnerBirth=identity.OwnerBirth,Ordinal=++ordinal,Applied=applied,ListenersRemoved=true,ControllerDisposed=true,TraceDisposed=true}));
            }
        }
        private sealed class ExpectedSmokeBodyFailure:Exception{}
        private IEnumerator SmokeBody(IChapter1OwnedBatchInputContext context,bool fail)
        {
            _fixture.RecordCaseInputWitness("owned-body-enter",true,false,false);using(var pilot=new Chapter1VerificationInputPilot(row=>Debug.Log("[BVP] "+row),context))
            {
                _isolation.BindOwnedDevices(pilot.CaptureInputBoundarySnapshot());_isolation.EmergencyCleanup=pilot.Dispose;pilot.Neutral();var end=_isolation.StartedReal+20;
                while(!pilot.ContextReady && pilot.Error==null && Time.realtimeSinceStartupAsDouble<end)yield return null;
                Assert.That(pilot.Error,Is.Null);Assert.That(pilot.ContextReady && pilot.NeutralConsumed,Is.True);var prior=pilot.CaptureInputBoundarySnapshot().ConsumedSequence;pilot.TapSubmit();
                while(pilot.Error==null && Time.realtimeSinceStartupAsDouble<end){var s=pilot.CaptureInputBoundarySnapshot();if(s.ConsumedSequence>prior && s.PublishedSequence==s.ConsumedSequence)break;yield return null;}
                pilot.RecordInputBoundarySnapshot("isolation-smoke",true);Assert.That(pilot.Error,Is.Null);var final=pilot.CaptureInputBoundarySnapshot();Assert.That(final.ConsumedSequence,Is.GreaterThan(prior));Assert.That(final.ExpectedKeyboardEventId,Is.EqualTo(final.ObservedKeyboardEventId));Assert.That(final.ExpectedMouseEventId,Is.EqualTo(final.ObservedMouseEventId));
                _receiptBodyCompleted=true;if(!fail)_fixture.RecordCaseInputWitness("normal-complete",true,true,false);if(fail)throw new ExpectedSmokeBodyFailure();
            }
        }
        private IEnumerator CaptureExpectedSmoke(IEnumerator body)
        {
            var expected=false;try{while(true){var more=false;try{more=body.MoveNext();}catch(ExpectedSmokeBodyFailure){expected=true;break;}if(!more)break;yield return body.Current;}}
            finally{(body as IDisposable)?.Dispose();}
            Assert.That(expected,Is.True);_receiptExpectedFailureReached=true;_fixture.RecordCaseInputWitness("expected-body-failure",true,true,true);_isolation.MarkExpectedBodyFailure();
        }
        private IEnumerator NaturalBody(IChapter1OwnedBatchInputContext context)
        {
            DesktopProfileLaunchAdapterV1.ConfigureChapter1TestRun("bvp-"+Guid.NewGuid().ToString("N"));
            _loaded=EditorSceneManager.LoadSceneInPlayMode("Assets/Scenes/Chapter1Playable.unity",new LoadSceneParameters(LoadSceneMode.Additive));
            var go=new GameObject("BVP-Natural-Observer");var observer=go.AddComponent<Chapter1BossNaturalPolicyObserver>();
            try
            {
                observer.Initialize(context);_isolation.BindOwnedDevices(observer.CaptureBoundary());_isolation.EmergencyCleanup=observer.Close;
                var end=Time.realtimeSinceStartupAsDouble+150;
                while(!observer.BossCompleted && observer.Error==null && Time.realtimeSinceStartupAsDouble<end)yield return null;
                observer.RecordInputBoundarySnapshot(true);Assert.That(observer.Error,Is.Null);Assert.That(observer.BossEntryHealth,Is.EqualTo(1));Assert.That(observer.NaturalHealthOne,Is.True,"체력1은 정상 원정 실제 피해로 도달해야 하며 주입하지 않습니다.");
                Assert.That(observer.PlayerDamage,Is.True);Assert.That(observer.BossDamage,Is.True);Assert.That(observer.BossCompleted,Is.True,"정상 생산 보스 완료 미관측");
                Debug.Log("[BVP] natural hp1=true bossDamage=true completed=true productionWrites=0");
            }
            finally{_fixture.CaptureCleanupFailure(observer.Close);_fixture.CaptureCleanupFailure(()=>_isolation.Restore());_fixture.CaptureCleanupFailure(context.Dispose);_fixture.CaptureCleanupFailure(()=>UnityEngine.Object.Destroy(go));}
            if(_loaded.IsValid() && _loaded.isLoaded)yield return SceneManager.UnloadSceneAsync(_loaded);
        }
    }
    [DefaultExecutionOrder(205)]
    internal sealed class Chapter1BossNaturalPolicyObserver:MonoBehaviour
    {
        private Chapter1VerificationInputPilot _pilot;private InputRouter _router;private bool _submitted;private int _last=-1;
        internal bool NaturalHealthOne,PlayerDamage,BossDamage,BossCompleted;internal string Error;
        internal int BossEntryHealth=-1;private int _lastRegularHealth=-1,_lastRegularTick=-1,_hpOneDamageTick=-1;private bool _extraDamage;private double _waitingSince=-1;
        internal Chapter1VerificationInputBoundarySnapshot CaptureBoundary()=>_pilot.CaptureInputBoundarySnapshot();
        internal void Initialize(IChapter1OwnedBatchInputContext context){if(_pilot!=null)throw new InvalidOperationException("관측기 중복 초기화");_pilot=new Chapter1VerificationInputPilot(row=>{Debug.Log("[BVP] "+row);},context);_pilot.Neutral();}
        private void FixedUpdate()
        {
            try
            {
                if(_pilot.Error!=null)throw new InvalidOperationException(_pilot.Error);
                RecordInputBoundarySnapshot();
                foreach(var candidate in UnityEngine.Object.FindObjectsByType<InputRouter>(FindObjectsSortMode.None))
                {
                    if(candidate.gameObject.scene.path!="Assets/Scenes/Chapter1Playable.unity")continue;
                    if(!candidate.TryGetChapter1CompletedEvidence(out var view))continue;
                    _router=candidate;_pilot.ObserveCombat(candidate,_=>{});PlayerDamage|=_pilot.EnemyDamageObserved;
                    var boss=candidate.transform.parent.GetComponentInChildren<OrdanBossCombatSimulationDriver>(true);
                    if(boss!=null && boss.HasLatestOutcome)
                    {
                        if(BossEntryHealth<0)
                        {
                            BossEntryHealth=view.Health;var first=candidate.BoundPlayerForAuthoring.Chapter1ConfiguredFirstTick;
                            NaturalHealthOne=first.HasValue && view.Tick==first.Value && BossEntryHealth==1 && _lastRegularHealth==1 && _hpOneDamageTick>=0 && _hpOneDamageTick<=_lastRegularTick && _lastRegularTick<view.Tick.Value;
                            Debug.Log("[BVP] firstBossTick="+view.Tick.Value+" configuredFirst="+(first.HasValue?first.Value.Value:-1)+" entryHp="+BossEntryHealth+" lastRegularTick="+_lastRegularTick+" lastRegularHp="+_lastRegularHealth+" actualDamageToOneTick="+_hpOneDamageTick);
                            if(!NaturalHealthOne)throw new InvalidOperationException("실제 일반방 피해로 체력1 보스 최초입장을 관측하지 못했습니다.");
                        }
                        foreach(var d in boss.LatestOutcome.DamageResults)if(d.TargetId=="ordan" && d.AppliedAmount>0)BossDamage=true;
                        foreach(var state in boss.LatestOutcome.Combatants)if(state.TargetId=="ordan" && state.IsDead)BossCompleted=true;
                    }
                    else
                    {
                        _lastRegularHealth=view.Health;_lastRegularTick=view.Tick.Value;
                        if(_pilot.LastDamageCurrentHealth==1 && _pilot.LastDamagePriorHealth>1 && _pilot.LastDamageAmount>0)_hpOneDamageTick=_pilot.LastDamageTick;
                    }
                }
            }catch(Exception e){Error=e.ToString();_pilot.Neutral();}
        }
        private void LateUpdate()
        {
            if(Error!=null || BossCompleted){_pilot.Neutral();return;}
            try
            {
                if(_pilot.Error!=null)throw new InvalidOperationException(_pilot.Error);
                RecordInputBoundarySnapshot();
                if(!_submitted)
                {if(_pilot.NaturalUpdates<2 || !_pilot.NeutralConsumed || !_pilot.ContextReady)return;foreach(var hub in UnityEngine.Object.FindObjectsByType<Chapter1HubLifetimeOwner>(FindObjectsSortMode.None))if(hub.gameObject.scene.path=="Assets/Scenes/Chapter1Playable.unity" && hub.Ready){_pilot.TapSubmit();_submitted=true;return;}return;}
                if(_router==null || !_router.TryGetChapter1CompletedEvidence(out var view))return;
                if(view.Tick.Value==_last)return;_last=view.Tick.Value;
                var graph=_router.transform.parent;
                if(graph.GetComponentInChildren<OrdanBossCombatSimulationDriver>(true)!=null){_pilot.PlayBoss(_router,view.Position,0);return;}
                if(graph.name=="R06" && !_extraDamage)
                {
                    if(view.Health==1){_extraDamage=true;_pilot.Neutral();Debug.Log("[BVP] R06 normal hostile damage reached hp1 tick="+view.Tick.Value);return;}
                    if(view.Health==2 && view.AliveEnemyCount>0)
                    {if(_waitingSince<0)_waitingSince=Time.realtimeSinceStartupAsDouble;if(Time.realtimeSinceStartupAsDouble-_waitingSince>=10)throw new InvalidOperationException("정상 입력 추가피해 대기10초 안에 체력1 미도달");_pilot.ApproachForFailure(view.Position);return;}
                }
                var environment=graph.Find("Environment");if(environment==null)throw new InvalidOperationException("원정 지형 원본 부재");
                var floors=new List<Rect>();var terrain=new List<Rect>();
                foreach(var body in environment.GetComponentsInChildren<BoxCollider2D>(true))
                {var pos=graph.InverseTransformPoint(body.transform.position);var rect=new Rect((Vector2)pos+body.offset-body.size/2,body.size);if(body.name.StartsWith("Floor",StringComparison.Ordinal))floors.Add(rect);else if(body.name.StartsWith("Terrain",StringComparison.Ordinal))terrain.Add(rect);}
                floors.Sort((a,b)=>a.xMin.CompareTo(b.xMin));_pilot.PlayRoom(_router,graph.name,floors.ToArray(),terrain.ToArray(),Rect.MinMaxRect(30,0,32,3),view.TransferTarget);
            }catch(Exception e){Error=e.ToString();_pilot.Neutral();}
        }
        internal void RecordInputBoundarySnapshot(bool final=false)
        {
            if(_pilot==null)return;
            if(_pilot.Error!=null)Error=_pilot.Error;
            var routers=UnityEngine.Object.FindObjectsByType<InputRouter>(FindObjectsSortMode.None);
            var hubs=UnityEngine.Object.FindObjectsByType<Chapter1HubLifetimeOwner>(FindObjectsSortMode.None);
            var context="flowPhase=unavailable-no-existing-assembly-reference observerError="+Error+" submitted="+_submitted+" routerCount="+routers.Length+" hubCount="+hubs.Length+" observerScene="+gameObject.scene.path;
            foreach(var hub in hubs)context+=" hubScene="+hub.gameObject.scene.path+" ready="+hub.Ready;
            foreach(var router in routers)context+=" routerScene="+router.gameObject.scene.path+" completed="+router.TryGetChapter1CompletedEvidence(out _);
            _pilot.RecordInputBoundarySnapshot(context,final);
        }
        internal void Close(){_pilot?.Neutral();_pilot?.Dispose();_pilot=null;Debug.Log("[BVP] owned devices removed");}
        private void OnDestroy(){if(_pilot!=null)Close();}
    }
    [Serializable] internal sealed class Chapter1CaseInputWitness
    {public string Stage,CaseName,RunId,Phase,SourceSha,OwnerBirth;public int OwnerPid;public long Ordinal;public bool BodyEntered,BodyCompleted,ExpectedFailureReached;}
    // REQ/AC-BAP-EMP/OWN/CARRY/OUTCOME-001: real pure policy entry and consumed sidecar, no fixture/API.
    internal sealed class BossOwnedJumpContinuationPolicyTests
    {
        private readonly object _issuer=new object(),_lease=new object(),_router=new object(),_player=new object();
        private Chapter1VerificationBossPolicy.OwnedJumpIdentity Identity(long epoch=1,long cycle=1,long mode=1)=>new Chapter1VerificationBossPolicy.OwnedJumpIdentity(_issuer,_lease,_router,_player,"source","case","birth",1,epoch,cycle,mode,true);
        private static Chapter1VerificationBossPolicy.Threat Threat(bool queued=false,bool contact=false,bool pulling=false,bool wall=false)=>new Chapter1VerificationBossPolicy.Threat(OrdanHealthStage.Stage1,false,queued,contact,false,pulling,28672,4915,410,0,wall);
        private void Consume(Chapter1VerificationBossPolicy p,Key[] keys,long sequence,bool ready=true,long epoch=1,long cycle=1)
        {p.ObserveOwnedConsumption(_issuer,_lease,"source","case","birth",1,epoch,cycle,keys,sequence,ready);}
        private Chapter1VerificationBossPolicy Ready()
        {var p=new Chapter1VerificationBossPolicy();p.BindOwnedJump(Identity());p.Consumed(false,true,1);p.Consumed(false,true,2);Consume(p,Array.Empty<Key>(),1);Consume(p,Array.Empty<Key>(),2);return p;}
        private static Chapter1VerificationBossPolicy.Command Read(Chapter1VerificationBossPolicy p,int tick=101,int age=3,bool grounded=false,bool locked=false,bool focus=true,float x=-7f,bool queued=false,bool contact=false,bool pulling=false,OrdanPatternSlot slot=OrdanPatternSlot.DebtLineA,OrdanBossPhase phase=OrdanBossPhase.Telegraph,bool aim=false,int cooldown=0,float y=1.04f,float vy=13.88f,bool wall=false)
        {return p.Read("run",1,tick,x,y,.333f,grounded,locked,focus,aim,cooldown,slot,phase,age,.35f,vy,Threat(queued,contact,pulling,wall));}
        private Chapter1VerificationBossPolicy Launched(bool consume=true)
        {var p=Ready();var c=Read(p,100,2,true);Assert.That(c.Keys,Does.Contain(Key.Space));if(consume)Consume(p,c.Keys,3);return p;}
        [Test] public void ContinuationRequiresOwnedConsumedJump()
        {var missing=Launched(false);Read(missing);Assert.That(missing.Unresolved,Is.Not.Null);var p=Launched();var c=Read(p);Assert.That(p.Unresolved,Is.Null);Assert.That(c.Keys,Does.Contain(Key.Space));Assert.That(p.OwnedContinuationActive,Is.True);}
        [Test] public void ContinuationAcceptsOneDelayedNeutralReceiptBeforeJump()
        {
            var p=Ready();
            var jump=Read(p,100,2,true);
            Assert.That(jump.Keys,Does.Contain(Key.Space));
            Consume(p,Array.Empty<Key>(),3); // queued neutral was issued before jump.
            Consume(p,jump.Keys,4); // exact next owned D+Space consumption.
            var next=Read(p,101,3);
            Assert.That(p.Unresolved,Is.Null);
            Assert.That(p.OwnedContinuationActive,Is.True);
            Assert.That(next.Keys,Does.Contain(Key.Space));
        }
        [Test] public void ContinuationRejectsTwoDelayedNeutralReceiptsAndMissingSequence()
        {
            var p=Ready();
            var jump=Read(p,100,2,true);
            Consume(p,Array.Empty<Key>(),3);
            Consume(p,Array.Empty<Key>(),4); // more than one: stale jump must fail closed.
            Consume(p,jump.Keys,5);
            Read(p,101,3);
            Assert.That(p.Unresolved,Is.Not.Null);
            var q=Ready();
            var other=Read(q,100,2,true);
            Consume(q,Array.Empty<Key>(),3);
            Consume(q,other.Keys,5); // missing sequence 4.
            Read(q,101,3);
            Assert.That(q.Unresolved,Is.Not.Null);
        }
        [Test] public void StandaloneBatchedPhysicsGapRemainsFinite()
        {
            Assert.That(Chapter1VerificationBossPolicy.IsBoundedStandaloneTickGap(2072,2080),Is.True);
            foreach(var tick in new[]{2072,2073,2081,2090})
                Assert.That(Chapter1VerificationBossPolicy.IsBoundedStandaloneTickGap(2072,tick),Is.False);
        }
        private Chapter1VerificationBossPolicy StandalonePrepared(Chapter1VerificationInputPilot pilot,Chapter1StandaloneVerificationSession session)
        {
            var p=new Chapter1VerificationBossPolicy();
            p.BindOwnedJump(new Chapter1VerificationBossPolicy.OwnedJumpIdentity(pilot,session,_router,_player,1,1,true));
            p.Consumed(false,true,1);p.Consumed(false,true,2);
            p.ObserveStandaloneConsumption(pilot,session,1,Array.Empty<Key>(),1,true);
            p.ObserveStandaloneConsumption(pilot,session,1,Array.Empty<Key>(),2,true);
            return p;
        }
        [Test] public void StandaloneStartupQuietRequiresForegroundAndNoRecentForeignInput()
        {
            Assert.That(Chapter1VerificationInputPilot.StartupQuiet(2,10,true),Is.False);
            Assert.That(Chapter1VerificationInputPilot.StartupQuiet(3.2,.99,true),Is.False);
            Assert.That(Chapter1VerificationInputPilot.StartupQuiet(3.2,1.01,true),Is.True);
            Assert.That(Chapter1VerificationInputPilot.StartupQuiet(5,3,false),Is.False);
            Assert.That(Chapter1VerificationInputPilot.StartupQuiet(double.NaN,2,true),Is.False);
            Assert.That(Chapter1VerificationInputPilot.StartupQuiet(4,double.PositiveInfinity,true),Is.False);
            // Nonowner changes after the session is armed still revoke input.
            Assert.That(Chapter1StandaloneVerificationSession.ObservationValid(
                true,true,true,true,true,true,false,true),Is.True);
            Assert.That(Chapter1StandaloneVerificationSession.ObservationValid(
                true,true,true,true,true,true,true,true),Is.False);
        }
        [Test] public void StandalonePendingJumpRequiresReceiptAfterOneFrame()
        {
            // Replays the actual Windows observation boundary: owned D+Space
            // issued at Telegraph age2; next FixedUpdate sees age10 before
            // the next Dynamic input receipt. No forged owner state is allowed.
            using(var pilot=new Chapter1VerificationInputPilot(_=>{}))
            {
                var session=new Chapter1StandaloneVerificationSession(new object(),"branch1");
                Assert.That(session.Bind(pilot),Is.True);
                var p=StandalonePrepared(pilot,session);
                var start=Read(p,100,2,true);
                Assert.That(start.Keys,Does.Contain(Key.Space));
                var pending=p.Read("run",1,108,-6.980713f,1.04125977f,1.16650391f,false,false,true,false,0,
                    OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,10,.35f,13.87793f,Threat());
                Assert.That(p.Unresolved,Is.Null,p.OwnedJumpDebugState);
                CollectionAssert.AreEqual(new[]{Key.D,Key.Space},pending.Keys);
                Assert.That(p.OwnedContinuationActive,Is.False,"first Dynamic D+Space receipt still pending");
                p.ObserveStandaloneConsumption(pilot,session,1,new[]{Key.D,Key.Space},3,true);
                Assert.That(p.OwnedContinuationActive,Is.True,"real owned receipt now activates carry");
                var next=p.Read("run",1,116,-5.8f,2.60f,8f,false,false,true,false,0,
                    OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,18,.35f,9.78f,Threat());
                Assert.That(p.Unresolved,Is.Null,p.OwnedJumpDebugState);
                Assert.That(next.Keys,Does.Contain(Key.Space));
            }
        }
        [Test] public void StandalonePendingJumpRejectsUnconfirmedSecondFrameAndThreat()
        {
            using(var pilot=new Chapter1VerificationInputPilot(_=>{}))
            {
                var session=new Chapter1StandaloneVerificationSession(new object(),"branch1");
                Assert.That(session.Bind(pilot),Is.True);
                var p=StandalonePrepared(pilot,session);
                Assert.That(Read(p,100,2,true).Keys,Does.Contain(Key.Space));
                var pending=p.Read("run",1,108,-6.980713f,1.04125977f,1.16650391f,false,false,true,false,0,
                    OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,10,.35f,13.87793f,Threat());
                Assert.That(p.Unresolved,Is.Null);
                Assert.That(p.OwnedContinuationActive,Is.False);
                // Use a state where the ordinary selector is unable to prove safety.
                p.Read("run",1,116,-6.980713f,1.04125977f,1.16650391f,false,false,true,false,0,
                    OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,18,.35f,13.87793f,Threat());
                Assert.That(p.Unresolved,Is.Not.Null,"missing actual receipt must close after one frame");
                var q=StandalonePrepared(pilot,session);
                Assert.That(Read(q,100,2,true).Keys,Does.Contain(Key.Space));
                q.Read("run",1,108,-6.980713f,1.04125977f,1.16650391f,false,false,true,false,0,
                    OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,10,.35f,13.87793f,Threat(queued:true,contact:true));
                Assert.That(q.Unresolved,Is.Not.Null,"already queued contact must not become pending carry");
                var gap=StandalonePrepared(pilot,session);
                Assert.That(Read(gap,100,2,true).Keys,Does.Contain(Key.Space));
                gap.Read("run",1,109,-6.980713f,1.04125977f,1.16650391f,false,false,true,false,0,
                    OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,11,.35f,13.87793f,Threat());
                Assert.That(gap.Unresolved,Is.Not.Null,"physics gap above eight ticks must fail");
            }
        }
        [Test] public void ContinuationRequiresSameCompletedIdentity()
        {var p=Launched();p.BindOwnedJump(Identity(mode:2));Read(p);Assert.That(p.Unresolved,Is.Not.Null);var q=Launched();Assert.Throws<InvalidOperationException>(()=>q.Read("other",1,101,-7,1,.3f,false,false,true,false,0,OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,3,.35f,13.8f,Threat()));}
        [Test] public void ContinuationRequiresContinuousTicksAndPhase()
        {var p=Launched();Read(p,102,4);Assert.That(p.Unresolved,Is.Not.Null);var q=Launched();Read(q,101,4);Assert.That(q.Unresolved,Is.Not.Null);var r=Launched();Read(r,slot:OrdanPatternSlot.DebtLineB);Assert.That(r.Unresolved,Is.Not.Null);}
        [Test] public void ContinuationDoesNotReissueJumpEdge()
        {var p=Launched();var c=Read(p);var same=Read(p);Assert.That(same.Keys,Is.SameAs(c.Keys));Consume(p,c.Keys,4);Assert.That(p.OwnedContinuationDeadline,Is.EqualTo(148));var next=Read(p,102,4);Assert.That(next.Keys,Does.Contain(Key.Space));Assert.That(p.OwnedContinuationDeadline,Is.EqualTo(148));}
        [Test] public void ContinuationDoesNotInventFutureReceipt()
        {var p=Launched();var c=Read(p);Consume(p,c.Keys,5);Read(p,102,4);Assert.That(p.Unresolved,Is.Not.Null);var q=Launched();Consume(q,new[]{Key.D,Key.Space},3);Assert.That(q.OwnedContinuationActive,Is.True);}
        [Test] public void ContinuationStopsAtFiniteDeadline()
        {var p=Launched();for(var tick=101;tick<=144;tick++){var t=(tick-101)/60f;var y=1.04f+13.88f*t-15.36f*t*t-.256f*t;var vy=13.88f-30.72f*t;var c=Read(p,tick,tick-98,y:y,vy:vy);Assert.That(p.Unresolved,Is.Null);Consume(p,c.Keys,tick-97);}Assert.That(p.OwnedContinuationDeadline,Is.EqualTo(148));Read(p,145,47);Assert.That(p.Unresolved,Is.Not.Null);Assert.That(p.OwnedContinuationActive,Is.False);}
        [Test] public void ContinuationRejectsReleaseOrContextLoss()
        {var p=Launched();Consume(p,new[]{Key.D},4);Read(p);Assert.That(p.Unresolved,Is.Not.Null);var q=Launched();Consume(q,new[]{Key.D,Key.Space},4,false);Read(q);Assert.That(q.Unresolved,Is.Not.Null);var r=Launched();Read(r,focus:false);Assert.That(r.OwnedContinuationActive,Is.False);}
        [Test] public void ContinuationRejectsLockOrQueuedThreat()
        {var p=Launched();Read(p,locked:true);Assert.That(p.OwnedContinuationActive,Is.False);var q=Launched();Read(q,queued:true,contact:true);Assert.That(q.Unresolved,Is.EqualTo("이미 배송 예약된 실제 접촉을 정상 입력으로 취소할 수 없음"));var r=Launched();Read(r,queued:true);Assert.That(r.Unresolved,Is.Not.Null);}
        [Test] public void ContinuationPreservesWallAndPullGuards()
        {var p=Launched();Read(p,x:-11.8f);Assert.That(p.Unresolved,Is.Not.Null);var q=Launched();Read(q,pulling:true);Assert.That(q.Unresolved,Is.Not.Null);}
        [Test] public void ContinuationPreservesAttackReleaseAndCooldown()
        {var p=Launched();var c=Read(p,aim:true,cooldown:1000);Assert.That(c.Attack,Is.False);var q=Ready();q.Consumed(true,false,3);var a=q.Read("run",1,100,9,1,0,true,false,true,true,1000,OrdanPatternSlot.DebtLineA,OrdanBossPhase.Telegraph,2,.35f,0,Threat());Assert.That(a.Attack,Is.False);}
        [Test] public void SafeRightSideNearBodyPermitsActualAttackWithoutChangingHostileGuards()
        {
            // Observed Windows stage: x=8.309, y=0.82 after an authored
            // BalanceAudit pull; previous fixed x>=8.75 left it unable to hit.
            var p=Ready();
            var command=p.Read("run",1,3000,8.309326f,.815185547f,0,true,false,true,true,0,
                OrdanPatternSlot.BalanceAudit,OrdanBossPhase.Recovery,35,.35f,0,Threat());
            Assert.That(p.Unresolved,Is.Null);
            Assert.That(command.Attack,Is.True,
                "exact right-side geometry, real distance and available cooldown");
            var tooClose=Ready();
            var denied=tooClose.Read("run",1,3000,8.20f,.815185547f,0,true,false,true,true,0,
                OrdanPatternSlot.BalanceAudit,OrdanBossPhase.Recovery,35,.35f,0,Threat());
            Assert.That(denied.Attack,Is.False,"player must clear body before attacking");
            var hostile=Ready();
            var hazard=hostile.Read("run",1,3000,8.309326f,.815185547f,0,true,false,true,true,0,
                OrdanPatternSlot.BalanceAudit,OrdanBossPhase.Execute,0,.35f,0,Threat(queued:true,contact:true));
            Assert.That(hazard.Attack,Is.False);
            Assert.That(hostile.Unresolved,Is.Not.Null,"already queued contact is still denied");
        }
        [Test] public void ContinuationPreservesLegacyCallersAndFailures()
        {var p=new Chapter1VerificationBossPolicy();p.Consumed(false,true,1);p.Consumed(false,true,2);Read(p);Assert.That(p.Unresolved,Is.Not.Null);Assert.Throws<InvalidOperationException>(()=>Read(Ready(),x:float.NaN));}
        [Test] public void ContinuationProducesOnlyAuthorizedKeys()
        {var p=Launched();var c=Read(p);foreach(var key in c.Keys)Assert.That(key==Key.A || key==Key.D || key==Key.Space,Is.True);Assert.That(c.Keys,Has.Length.EqualTo(2));Assert.That(c.Attack,Is.False);}

        // Exhaustion remains failure; terminal evidence is reserved inside each caller's existing 64/128 KiB cap.
        [Test] public void CleanupBudgetKeepsTerminalEvidenceAfterOrdinaryExhaustion()
        {
            var overhead=Chapter1VerificationBossBudget.SerializedUpperBound("");
            var fullRow=new string('x',8192-overhead);
            Assert.That(Chapter1VerificationBossBudget.SerializedUpperBound(fullRow),Is.EqualTo(8192));
            foreach(var maximum in new[]{65536,131072})
            {
                var budget=new Chapter1VerificationBossBudget(maximum,true);
                Assert.Throws<InvalidOperationException>(()=>budget.AddCleanup(fullRow+"x"));
                for(var used=0;used<maximum-3*8192;used+=8192)budget.Add(fullRow);
                Assert.Throws<InvalidOperationException>(()=>budget.Add("ordinary-exhausted"));
                budget.AddCleanup(fullRow);budget.AddCleanup(fullRow);budget.AddCleanup(fullRow);
                Assert.Throws<InvalidOperationException>(()=>budget.AddCleanup("fourth-terminal-row"));
                Assert.Throws<InvalidOperationException>(()=>budget.Add("ordinary-still-exhausted"));
            }
            Assert.Throws<InvalidOperationException>(()=>new Chapter1VerificationBossBudget().AddCleanup("unreserved"));
        }
        // State-machine evidence only; analytic fixture poses below do not prove actual Unity collision safety.
        [Test] public void RecoveryApproachStartsAtObservedAge20AndClearsBody()
        {
            // Exact observed Windows case: DebtLineA Recovery age18 x=2.7583,
            // first x=3.0249 is age20, grounded, vy=0, rightward speed8.
            // This pure kinematic fixture tests the bounded state machine,
            // not real Physics2D collisions or chapter completion.
            var p=Ready();
            var first=p.Read("run",1,1982,3.024902f,.815185547f,8,true,false,true,false,0,
                OrdanPatternSlot.DebtLineA,OrdanBossPhase.Recovery,20,.35f,0,Threat());
            CollectionAssert.AreEqual(new[]{Key.D,Key.Space},first.Keys);
            var last=first.Keys;
            var braking=false;
            var released=false;
            for(var n=1;n<=51;n++)
            {
                Consume(p,last,n+2);
                var t=n/60f;
                var absoluteAge=20+n;
                var recovery=absoluteAge<39;
                // Fully authored air acceleration is 40 u/s². At n=40 the
                // right side is cleared, the owned input releases Space and
                // presses A; after that this analytic pose decelerates.
                var brakeTime=Math.Max(0,(n-40)/60f);
                var x=3.024902f+8f*Math.Min(t,40f/60f)
                    +8*brakeTime-20*brakeTime*brakeTime;
                var vx=n<=40?8f:Math.Max(0,8-40*brakeTime);
                var keys=p.Read("run",1,1982+n,x,
                    .815185547f+14.39f*t-15.36f*t*t,vx,false,false,true,false,0,
                    recovery?OrdanPatternSlot.DebtLineA:OrdanPatternSlot.SeizureWeight,
                    recovery?OrdanBossPhase.Recovery:OrdanBossPhase.Telegraph,
                    recovery?absoluteAge:absoluteAge-39,.35f,14.39f-30.72f*t,Threat());
                Assert.That(p.Unresolved,Is.Null,"step="+n+" position="+x);
                if(n==40){CollectionAssert.AreEqual(new[]{Key.A},keys.Keys);braking=true;}
                if(n>40 && n<=48){CollectionAssert.AreEqual(new[]{Key.A},keys.Keys);}
                if(n>=49 && keys.Keys.Length==0)released=true;
                last=keys.Keys;
            }
            Assert.That(braking && released,Is.True,
                "right-side A braking followed by neutral Space release");
            foreach(var invalid in new[]{19,21})
            {
                var q=Ready();
                var noJump=q.Read("run",1,2000,3.024902f,.815185547f,8,true,false,true,false,0,
                    OrdanPatternSlot.DebtLineA,OrdanBossPhase.Recovery,invalid,.35f,0,Threat());
                if(invalid==21)CollectionAssert.DoesNotContain(noJump.Keys,Key.Space);
            }
        }
        [Test] public void OwnedRecoveryApproachConsumesOneEdgeThenReleasesAtObservedRightSide()
        {
            var p=Ready();var first=p.Read("run",1,2000,3.06f,.815185547f,8,true,false,true,false,0,OrdanPatternSlot.DebtLineA,OrdanBossPhase.Recovery,15,.35f,0,Threat());
            CollectionAssert.AreEqual(new[]{Key.D,Key.Space},first.Keys);Assert.That(first.Attack,Is.False);
            for(var n=1;n<=39;n++)
            {
                Consume(p,new[]{Key.D,Key.Space},n+2);var t=n/60f;var age=15+n;
                var c=p.Read("run",1,2000+n,3.06f+8*t,.815185547f+14.39f*t-15.36f*t*t,8,false,false,true,false,0,
                    age<39?OrdanPatternSlot.DebtLineA:OrdanPatternSlot.SeizureWeight,age<39?OrdanBossPhase.Recovery:OrdanBossPhase.Telegraph,age<39?age:age-39,.35f,14.39f-30.72f*t,Threat());
                Assert.That(p.Unresolved,Is.Null);Assert.That(c.Attack,Is.False);
                if(n<39)CollectionAssert.AreEqual(new[]{Key.D,Key.Space},c.Keys);else CollectionAssert.AreEqual(new[]{Key.A},c.Keys,"observed right-side pose starts bounded A braking, with Space released");
            }
        }
        [Test] public void OwnedRecoveryApproachRejectsUnownedLostProofAndDiscontinuity()
        {
            var unowned=new Chapter1VerificationBossPolicy();unowned.Consumed(false,true,1);unowned.Consumed(false,true,2);
            var ordinary=unowned.Read("run",1,2000,3.06f,.815185547f,8,true,false,true,false,0,OrdanPatternSlot.DebtLineA,OrdanBossPhase.Recovery,15,.35f,0,Threat());
            CollectionAssert.DoesNotContain(ordinary.Keys,Key.Space,"unowned caller gets no navigation edge");
            foreach(var failure in new[]{"no-press","queued","gap","owner","focus","wall","negative-age"})
            {
                var p=Ready();p.Read("run",1,2000,3.06f,.815185547f,8,true,false,true,false,0,OrdanPatternSlot.DebtLineA,OrdanBossPhase.Recovery,15,.35f,0,Threat());
                if(failure=="owner")p.BindOwnedJump(Identity(epoch:2));if(failure=="focus")p.Focus(false);
                Consume(p,failure=="no-press"?new[]{Key.D}:new[]{Key.D,Key.Space},3);
                var c=p.Read("run",1,failure=="gap"?2002:2001,3.19f,1.046f,8,false,false,failure!="focus",false,0,failure=="negative-age"?OrdanPatternSlot.SeizureWeight:OrdanPatternSlot.DebtLineA,failure=="negative-age"?OrdanBossPhase.Telegraph:OrdanBossPhase.Recovery,failure=="negative-age"?-23:failure=="gap"?17:16,.35f,13.87793f,Threat(queued:failure=="queued",wall:failure=="wall"));
                Assert.That(c.Keys,Is.Empty,failure);Assert.That(c.Attack,Is.False);Assert.That(p.Unresolved,Is.Not.Null,failure);
            }
        }
        [Test] public void OwnedDebtEnvelopeSeparatesInactiveRiseFromFireWindow()
        {
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(-7,1.04126f,.333f,13.87793f,1,OrdanPatternSlot.DebtLineA,3,.35f),Is.True);
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(-7,1.04126f,.333f,13.87793f,1,OrdanPatternSlot.DebtLineB,3,.35f),Is.False);
            var owned=Launched();var c=Read(owned,y:1.04126f,vy:13.87793f);Assert.That(owned.Unresolved,Is.Null);Assert.That(c.Keys,Does.Contain(Key.Space));
            var unowned=Ready();Read(unowned,y:1.04126f,vy:13.87793f);Assert.That(unowned.Unresolved,Is.Not.Null);
        }
        [Test] public void OwnedDebtEnvelopeRejectsDescendingAndInsufficientApex()
        {
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(-7,3,.333f,-2,1,OrdanPatternSlot.DebtLineA,20,.35f),Is.False);
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(-7,1.04f,.333f,2,1,OrdanPatternSlot.DebtLineA,3,.35f),Is.False);
            var p=Launched();var c=Read(p,vy:-5);Assert.That(p.Unresolved,Is.Not.Null);Assert.That(c.Keys,Is.Empty);Assert.That(p.OwnedContinuationActive,Is.False);
        }
        [Test] public void OwnedDebtEnvelopeKeepsClosedLineMarginAndLandingBounds()
        {
            var first=44f/60;var last=46f/60;var upper=(OrdanBossHostileDamageProducer.ApprovedDebtLineA.MaxY+OrdanBossHostileDamageProducer.ApprovedPlayerHalfHeight)/4096f+.02f;
            var boundary=(float)Math.Ceiling((upper-Math.Min(Chapter1VerificationBossPolicy.LowerBallisticHeight(0,13.87793f,first),Chapter1VerificationBossPolicy.LowerBallisticHeight(0,13.87793f,last)))*4096)/4096;
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(-7,boundary-1f/4096,.333f,13.87793f,1,OrdanPatternSlot.DebtLineA,3,.35f),Is.False);
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(-7,boundary+1f/4096,.333f,13.87793f,1,OrdanPatternSlot.DebtLineA,3,.35f),Is.True);
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(4,1.04f,.333f,13.88f,1,OrdanPatternSlot.DebtLineA,3,.35f),Is.False);
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(-10,1.04f,-8,13.88f,-1,OrdanPatternSlot.DebtLineA,3,.35f),Is.False);
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(-7,8,.333f,13.88f,1,OrdanPatternSlot.DebtLineA,3,.35f),Is.False);
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(-7,1.04f,.333f,13.88f,1,OrdanPatternSlot.DebtLineA,3,3f),Is.False);
        }

        [Test] public void OwnedDebtEnvelopeUsesFastestStageAndObservedWallContact()
        {
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(-7,1.04126f,.333f,13.87793f,1,OrdanPatternSlot.DebtLineA,3,1.16f),Is.True);
            Assert.That(Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(-7,1.04126f,.333f,13.87793f,1,OrdanPatternSlot.DebtLineA,3,1.18f),Is.False);
            var p=Launched();var c=Read(p,wall:true);Assert.That(p.OwnedContinuationActive,Is.False);Assert.That(p.Unresolved,Is.Not.Null);Assert.That(c.Keys,Is.Empty);
            var ordinary=Launched();Read(ordinary,wall:true,x:9);Assert.That(ordinary.OwnedContinuationActive,Is.False);
        }

        // AC-SOP-001/004: denied standalone authority, never an Editor positive handoff.
        private Chapter1VerificationBossPolicy StandaloneUnowned()
        {var p=new Chapter1VerificationBossPolicy();p.Consumed(false,true,1);p.Consumed(false,true,2);p.BindOwnedJump(new Chapter1VerificationBossPolicy.OwnedJumpIdentity(_issuer,(Chapter1StandaloneVerificationSession)null,_router,_player,1,1,false));return p;}
        [Test] public void ContinuationRequiresOwnedConsumedJumpStandaloneUnowned()
        {var p=StandaloneUnowned();var command=Read(p,100,2,true);Assert.That(command.Keys,Does.Contain(Key.Space));Read(p);Assert.That(p.OwnedContinuationActive,Is.False);Assert.That(p.Unresolved,Is.Not.Null);}
        [Test] public void ContinuationRequiresSameCompletedIdentityStandaloneUnowned()
        {var p=StandaloneUnowned();p.BindOwnedJump(new Chapter1VerificationBossPolicy.OwnedJumpIdentity(_issuer,(Chapter1StandaloneVerificationSession)null,_router,_player,2,2,false));Read(p);Assert.That(p.OwnedContinuationActive,Is.False);}
        [Test] public void ContinuationRequiresContinuousTicksAndPhaseStandaloneUnowned()
        {var p=StandaloneUnowned();Read(p,100,2,true);Read(p,102,4);Assert.That(p.OwnedContinuationActive,Is.False);Assert.That(p.Unresolved,Is.Not.Null);}
        [Test] public void ContinuationDoesNotReissueJumpEdgeStandaloneUnowned()
        {var p=StandaloneUnowned();var a=Read(p,100,2,true);var b=Read(p,100,2,true);Assert.That(b.Keys,Is.SameAs(a.Keys));Assert.That(p.OwnedContinuationActive,Is.False);}
        [Test] public void ContinuationDoesNotInventFutureReceiptStandaloneUnowned()
        {var p=StandaloneUnowned();Read(p,100,2,true);p.ObserveStandaloneConsumption(_issuer,null,1,new[]{Key.D,Key.Space},3,true);Read(p);Assert.That(p.OwnedContinuationActive,Is.False);}
        [Test] public void ContinuationStopsAtFiniteDeadlineStandaloneUnowned()
        {var p=StandaloneUnowned();Read(p,100,2,true);Read(p,148,50);Assert.That(p.OwnedContinuationActive,Is.False);}
        [Test] public void ContinuationRejectsReleaseOrContextLossStandaloneUnowned()
        {var p=StandaloneUnowned();Read(p,100,2,true);Read(p,focus:false);Assert.That(p.OwnedContinuationActive,Is.False);}
        [Test] public void ContinuationRejectsLockOrQueuedThreatStandaloneUnowned()
        {var p=StandaloneUnowned();Read(p,locked:true,queued:true);Assert.That(p.OwnedContinuationActive,Is.False);}
        [Test] public void ContinuationPreservesWallAndPullGuardsStandaloneUnowned()
        {var p=StandaloneUnowned();Read(p,x:-11.8f,pulling:true);Assert.That(p.OwnedContinuationActive,Is.False);Assert.That(p.Unresolved,Is.Not.Null);}
        [Test] public void ContinuationPreservesAttackReleaseAndCooldownStandaloneUnowned()
        {var p=StandaloneUnowned();var command=Read(p,aim:true,cooldown:1000);Assert.That(command.Attack,Is.False);Assert.That(p.OwnedContinuationActive,Is.False);}
        [Test] public void ContinuationPreservesLegacyCallersAndFailuresStandaloneUnowned()
        {var p=StandaloneUnowned();Assert.Throws<InvalidOperationException>(()=>Read(p,x:float.NaN));Assert.That(p.OwnedContinuationActive,Is.False);}
        [Test] public void ContinuationProducesOnlyAuthorizedKeysStandaloneUnowned()
        {var p=StandaloneUnowned();var command=Read(p,100,2,true);foreach(var key in command.Keys)Assert.That(key==Key.A || key==Key.D || key==Key.Space,Is.True);Assert.That(p.OwnedContinuationActive,Is.False);}
    }

}
