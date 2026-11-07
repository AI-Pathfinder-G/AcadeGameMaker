using System;
using System.IO;
using System.Threading;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using NUnit.Framework;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // AC-M5D7QC4-007/008: 정상 가상 입력·실제 action.canceled와 정상 C2 완료를 관측한다.
    public sealed class ProfileResetExecutionBridgePlayModeTests : InputTestFixture
    {
        private readonly System.Collections.Generic.List<C4ActualExecutionPlayFixtureV1.EvidenceRow> _evidenceRows=new System.Collections.Generic.List<C4ActualExecutionPlayFixtureV1.EvidenceRow>();
        [TearDown]
        public void FinishActualEvidenceRows()
        { foreach (var row in _evidenceRows) row.Finish(); _evidenceRows.Clear(); }
        private C4ActualExecutionPlayFixtureV1.EvidenceRow BeginEvidence(string id,string kind,string family,string checkpoint,int? value,string[] requirements,string[] acceptance)
        {
            var row=new C4ActualExecutionPlayFixtureV1.EvidenceRow(id,requirements,acceptance,kind,family,checkpoint,value);
            _evidenceRows.Add(row); return row;
        }
        private sealed class ActualControlCounts
        {
            internal int C1Returns,C2Returns,ProofObservers,MemoryObservers,FreshReserves,NativeCallbacks;
            internal void Checkpoint(ProfileResetExecutionCheckpointV1 checkpoint)
            {
                if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) C1Returns++;
                if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) C2Returns++;
                if (checkpoint == ProfileResetExecutionCheckpointV1.AfterFreshReserve) FreshReserves++;
            }
        }
        private static ProfileResetExecutionResultV1 ExecuteWithActualNativeObservation(C4ActualExecutionPlayFixtureV1 f,IProfileResetExecutionTestControlV1 control,ActualControlCounts counts)
        {
            var actions=(GameInputActions)C4ActualExecutionPlayFixtureV1.Field(f.Router,typeof(InputRouter),"_actions",typeof(GameInputActions));
            Action<InputAction.CallbackContext> callback=context=>counts.NativeCallbacks++;
            actions.UI.Submit.canceled+=callback;
            try { return f.Execute(control,C4ActualExecutionPlayFixtureV1.DiskControl((name,value)=>{})); }
            finally { actions.UI.Submit.canceled-=callback; }
        }
        private static void ObserveControlledResult(C4ActualExecutionPlayFixtureV1 f,C4ActualExecutionPlayFixtureV1.EvidenceRow row,ProfileResetExecutionResultV1 result,ActualControlCounts counts,C4ActualExecutionPlayFixtureV1.RootLeaseWorker actualWorker=null)
        {
            Assert.That(counts.NativeCallbacks,Is.EqualTo(1));
            f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result,actualWorker); f.ObserveAbsentFreshCompletion(row,result);
            row.Observe("callCounts.c1Begin",(long)counts.C1Returns); row.Observe("callCounts.c2Finalize",(long)counts.C2Returns);
            row.Observe("callCounts.preparedObserver",(long)counts.ProofObservers); row.Observe("callCounts.c2Observer",(long)counts.MemoryObservers);
            row.Observe("callCounts.freshReserve",(long)counts.FreshReserves); row.Observe("callCounts.nativeCallback",(long)counts.NativeCallbacks);
            f.Dispose(); f.ObserveActualCleanup(row);
            if (actualWorker != null)
            {
                row.Observe("cleanup.releaseSignaled",actualWorker.ReleaseSignaled ? "Yes" : "No");
                row.Observe("cleanup.workerLeaseReleased",actualWorker.LeaseReleased ? "Yes" : "No");
                row.Observe("cleanup.workerJoined",actualWorker.Joined ? "Yes" : "No");
            }
        }
        [Test]
        [Timeout(180000)]
        public void AC002_RealEnableAndAfterUpdateCannotReleaseExecutionGuard()
        {
            var evidence=BeginEvidence("play_AC002_RealEnableAndAfterUpdateCannotReleaseExecutionGuard","ActualCallback","None",null,null,new[] {"REQ-M5D7QC4-002"},new[] {"AC-M5D7QC4-002"});
            var counts=new ActualControlCounts();
            using (var f=C4ActualExecutionPlayFixtureV1.Create())
            {
                var actions=(GameInputActions)C4ActualExecutionPlayFixtureV1.Field(f.Router,typeof(InputRouter),"_actions",typeof(GameInputActions));
                var reached=0; var receipt=f.Router.CurrentReceipt.Value;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                {
                    counts.Checkpoint(checkpoint);
                    if (checkpoint != ProfileResetExecutionCheckpointV1.AfterRouterGuard) return;
                    reached++; evidence.Reached(); actions.UI.Disable();
                    f.ReleaseDuringGuard(); actions.UI.Enable(); InputSystem.Update();
                    Assert.That(ProfileNewGameConfirmationV1.IsExecutionGuardedPair(f.Adapter,f.Router),Is.True);
                    Assert.That(f.Router.CurrentReceipt.Value,Is.EqualTo(receipt));
                    Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                },()=>{},proof=>counts.ProofObservers++,memory=>counts.MemoryObservers++);
                var result=ExecuteWithActualNativeObservation(f,control,counts);
                Assert.That(reached,Is.EqualTo(1)); Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                foreach(var map in ((GameInputActions)C4ActualExecutionPlayFixtureV1.Field(f.Router,typeof(InputRouter),"_actions",typeof(GameInputActions))).asset.actionMaps) Assert.That(map.enabled,Is.False);
                ObserveControlledResult(f,evidence,result,counts);
            }
        }
        [Test]
        [Timeout(180000)]
        public void AC007_ActualC2AcquireBusyAfterDiskPreparedIsTerminal()
        {
            var evidence=BeginEvidence("play_AC007_ActualC2AcquireBusyAfterDiskPreparedIsTerminal","ActualCallback","None",null,null,new[] {"REQ-M5D7QC4-005"},new[] {"AC-M5D7QC4-007"});
            var counts=new ActualControlCounts();
            using (var f=C4ActualExecutionPlayFixtureV1.Create())
            {
                C4ActualExecutionPlayFixtureV1.RootLeaseWorker lease=null;
                var c1=0; var c2=0;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                { counts.Checkpoint(checkpoint); if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) c1++; if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) c2++; },
                    ()=>{ lease=new C4ActualExecutionPlayFixtureV1.RootLeaseWorker(f.Root); },proof=>counts.ProofObservers++,memory=>counts.MemoryObservers++);
                ProfileResetExecutionResultV1 result;
                try { result=ExecuteWithActualNativeObservation(f,control,counts); }
                finally { if (lease != null) lease.Dispose(); }
                Assert.That(lease,Is.Not.Null); Assert.That(lease.LeaseReleased,Is.True); Assert.That(lease.Joined,Is.True);
                Assert.That(c1,Is.EqualTo(1)); Assert.That(c2,Is.EqualTo(1));
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.ReloadRequired));
                Assert.That(result.Phase,Is.EqualTo(ProfileResetExecutionPhaseV1.C2Invoked));
                Assert.That(File.Exists(Path.Combine(f.Root,"profile.reset.json")),Is.True);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True); Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                Assert.That(() => f.AcceptFresh(result),Throws.InvalidOperationException); result.Validate();
                evidence.Reached(); ObserveControlledResult(f,evidence,result,counts,lease);
            }
        }
        [Test]
        [Timeout(180000)]
        public void AC007_ActualC2UnsafeLockPathManualRepairIsTerminal()
        {
            var evidence=BeginEvidence("play_AC007_ActualC2UnsafeLockPathManualRepairIsTerminal","ActualCallback","None",null,null,new[] {"REQ-M5D7QC4-005"},new[] {"AC-M5D7QC4-007"});
            var counts=new ActualControlCounts();
            using (var f=C4ActualExecutionPlayFixtureV1.Create())
            {
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>counts.Checkpoint(checkpoint),()=>
                {
                    var path=Path.Combine(f.Root,"profile.operation.lock"); File.Delete(path); Directory.CreateDirectory(path);
                },proof=>counts.ProofObservers++,memory=>counts.MemoryObservers++);
                var result=ExecuteWithActualNativeObservation(f,control,counts);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.ManualRepairRequired));
                Assert.That(result.Phase,Is.EqualTo(ProfileResetExecutionPhaseV1.C2Invoked));
                Assert.That(File.Exists(Path.Combine(f.Root,"profile.reset.json")),Is.True);
                Assert.That(Directory.Exists(Path.Combine(f.Root,"profile.operation.lock")),Is.True);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True); Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                Assert.That(() => f.AcceptFresh(result),Throws.InvalidOperationException); result.Validate();
                evidence.Reached(); ObserveControlledResult(f,evidence,result,counts);
            }
        }
        [TestCase(false)]
        [TestCase(true)]
        [Timeout(180000)]
        public void AC008_ActualDisableDuringPendingOrFreshClosesOriginalCohort(bool fresh)
        {
            var evidence=BeginEvidence("play_AC008_ActualDisableDuringPendingOrFreshClosesOriginalCohort_"+(fresh ? "True" : "False"),"ActualCallback",fresh ? "None" : "C4",fresh ? null : "AfterRouterGuard",fresh ? (int?)null : 6,
                new[] {"REQ-M5D7QC4-006"},new[] {"AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionPlayFixtureV1.Create())
            {
                var reached=0; var callbackReturned=false; var c1=0; var c2=0; var prepared=0; var c2Observed=0; var freshReserve=0;
                Exception actualFailure=null; ProfileResetExecutionResultV1 historical=null;
                if (fresh)
                {
                    ProfileResetExecutionResultV1 result;
                    using(var lease=new C4ActualExecutionPlayFixtureV1.RootLeaseWorker(f.Root)) result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router); historical=result;
                    Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Busy));
                    f.Router.enabled=false; reached++; evidence.Reached();
                    Assert.That(() => f.AcceptFresh(result),Throws.InvalidOperationException); result.Validate();
                }
                else
                {
                    var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                    {
                        if(checkpoint == ProfileResetExecutionCheckpointV1.AfterRouterGuard) { reached++; f.Router.enabled=false; callbackReturned=true; }
                        if(checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) c2++;
                        if(checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) freshReserve++;
                    },()=>{},proof=>prepared++,memory=>c2Observed++);
                    actualFailure=Assert.Throws<InvalidOperationException>(() => f.Execute(control,C4ActualExecutionPlayFixtureV1.DiskControl((name,value)=>{ if(value == 1) c1++; })));
                }
                Assert.That(reached,Is.EqualTo(1)); Assert.That(f.Router.enabled,Is.False);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router),Throws.InvalidOperationException);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                if (fresh) { Assert.That(historical,Is.Not.Null); f.ObserveActualResult(evidence,historical); f.ObserveActualBarrier(evidence,historical); f.ObserveNoOpCallCounts(evidence,historical); }
                else
                {
                    f.ObserveUnreturnedLifecycleFailure(evidence,actualFailure,reached,callbackReturned,c1,c2,prepared,c2Observed,freshReserve);
                    evidence.Reached();
                }
                f.Dispose(); f.ObserveActualCleanup(evidence);
            }
        }
        [Test]
        [Timeout(180000)]
        public void AC008_ActualDestroyAfterExecutionPreservesForensicHistory()
        {
            var evidence=BeginEvidence("play_AC008_ActualDestroyAfterExecutionPreservesForensicHistory","FullBridge","None",null,null,new[] {"REQ-M5D7QC4-006"},new[] {"AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionPlayFixtureV1.Create())
            {
                var result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router);
                var generation=result.ExecutionGeneration;
                UnityEngine.Object.DestroyImmediate(f.Adapter);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router),Throws.InvalidOperationException);
                result.Validate(); Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(result.ExecutionGeneration,Is.EqualTo(generation));
                evidence.Reached(); f.ObserveActualResult(evidence,result); f.ObserveActualBarrier(evidence,result); f.ObserveNoOpCallCounts(evidence,result);
                f.Dispose(); f.ObserveActualCleanup(evidence);
            }
        }
        [Test]
        [Timeout(180000)]
        public void AC008_ActualFaultRecordedBeforeNativeDisableMakesCancelReentryInert()
        {
            var evidence=BeginEvidence("play_AC008_ActualFaultRecordedBeforeNativeDisableMakesCancelReentryInert","ActualCallback","C4","AfterRouterGuard",(int)ProfileResetExecutionCheckpointV1.AfterRouterGuard,
                new[] {"REQ-M5D7QC4-001","REQ-M5D7QC4-006"},new[] {"AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionPlayFixtureV1.Create())
            {
                var actions=(GameInputActions)C4ActualExecutionPlayFixtureV1.Field(f.Router,typeof(InputRouter),"_actions",typeof(GameInputActions));
                var canceled=0; var rejected=0; var faultBeforeNative=true; Exception callbackFailure=null;
                var reached=0; var c1=0; var c2=0; var prepared=0; var memory=0; var reserve=0;
                Action<InputAction.CallbackContext> callback=context=>
                {
                    canceled++;
                    try
                    {
                        faultBeforeNative &= ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner);
                        try { ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router); }
                        catch(InvalidOperationException) { rejected++; }
                    }
                    catch(Exception error) { callbackFailure=error; }
                };
                actions.UI.Submit.canceled+=callback;
                var injected=new InvalidOperationException("실제 native 종료 전 fault 시험"); Exception actualFailure=null;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                {
                    if(checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin)c1++;
                    if(checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize)c2++;
                    if(checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve)reserve++;
                    if(checkpoint == ProfileResetExecutionCheckpointV1.AfterRouterGuard) { reached++; evidence.Reached(); throw injected; }
                },()=>{},proof=>prepared++,actualMemory=>memory++);
                try { actualFailure=Assert.Throws<InvalidOperationException>(() => f.Execute(control,C4ActualExecutionPlayFixtureV1.DiskControl((name,value)=>{}))); }
                finally { actions.UI.Submit.canceled-=callback; }
                Assert.That(callbackFailure,Is.Null); Assert.That(canceled,Is.EqualTo(1));
                Assert.That(faultBeforeNative,Is.True); Assert.That(rejected,Is.EqualTo(canceled));
                Assert.That(actualFailure,Is.SameAs(injected));
                f.ObservePreC1Failure(evidence,actualFailure,injected,ProfileResetExecutionCheckpointV1.AfterRouterGuard,reached,c1,c2,prepared,memory,reserve);
                evidence.Observe("callCounts.nativeCallback",(long)canceled);
                f.Dispose(); f.ObserveActualCleanup(evidence);
            }
        }
        [Test]
        [Timeout(180000)]
        public void AC004_ActualBusyFreshCursorDiscardsFirstSubmitThenTakesNewRequest()
        { ActualFreshCursor(false); }
        [Test]
        [Timeout(180000)]
        public void AC004_ActualStaleFreshCursorDiscardsFirstSubmitThenTakesNewRequest()
        { ActualFreshCursor(true); }
        [Test]
        [Timeout(180000)]
        public void AC004_ActualFreshThenRepeatedCancelUsesLatestCursorAndPreservesHistory()
        {
            var evidence=BeginEvidence("play_AC004_ActualFreshThenRepeatedCancelUsesLatestCursorAndPreservesHistory","FullBridge","None",null,null,
                new[] {"REQ-M5D7QC4-004"},new[] {"AC-M5D7QC4-004"});
            using (var f=C4ActualExecutionPlayFixtureV1.Create(true,true))
            {
                var ownerType=C4ActualExecutionPlayFixtureV1.Hub("NewGameConfirmationOwnerV1");
                var presenterType=C4ActualExecutionPlayFixtureV1.Hub("HubMenuPresenterV1");
                var handoffType=C4ActualExecutionPlayFixtureV1.Hub("HubMenuIntentHandoffOwnerV1");
                var historyType=ownerType.GetNestedType("CommitHistory",System.Reflection.BindingFlags.NonPublic);
                Assert.That(historyType,Is.Not.Null); Assert.That(historyType.DeclaringType,Is.EqualTo(ownerType));
                var originalCommit=C4ActualExecutionPlayFixtureV1.Field(f.Owner,ownerType,"_commitHistory",historyType);
                var originalCursor=C4ActualExecutionPlayFixtureV1.Field(f.Presenter,presenterType,"_cursor",typeof(UiSemanticFrameCursorV1));
                var takeType=typeof(Nullable<>).MakeGenericType(C4ActualExecutionPlayFixtureV1.Hub("HubMenuIntentRequestV1"));
                var originalTake=C4ActualExecutionPlayFixtureV1.Field(f.HandoffOwner,handoffType,"_takenRequest",takeType);
                ProfileResetExecutionResultV1 busy; C4ActualExecutionPlayFixtureV1.RootLeaseWorker actualWorker=null;
                using (actualWorker=new C4ActualExecutionPlayFixtureV1.RootLeaseWorker(f.Root))
                    busy=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router);
                Assert.That(busy.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Busy));
                f.ReleaseDuringGuard(); f.AcceptFresh(busy);
                object start=null;
                for (var cancellation=0;cancellation<3;cancellation++)
                {
                    f.Publish(Key.Enter); Assert.That(f.Router.CurrentUiFrame.Value.SubmitPressed,Is.True);
                    f.PresenterFixed(); f.LateTakePoll();
                    Assert.That(f.TryTake(out var discarded),Is.False); Assert.That(discarded,Is.Null);
                    f.Publish(); f.PresenterFixed(); f.LateTakePoll();
                    f.Publish(Key.Enter); Assert.That(f.Router.CurrentUiFrame.Value.SubmitPressed,Is.True);
                    f.PresenterFixed(); f.LateTakePoll(); Assert.That(f.TryTake(out var actualHandle),Is.True);
                    start=f.Intake(actualHandle);
                    Assert.That(C4ActualExecutionPlayFixtureV1.Read(start,C4ActualExecutionPlayFixtureV1.Hub("NewGameConfirmationStartResultV1"),"Outcome",C4ActualExecutionPlayFixtureV1.Hub("NewGameConfirmationOutcomeV1")).ToString(),Is.EqualTo("DecisionRequired"));
                    Assert.That(C4ActualExecutionPlayFixtureV1.Field(f.Owner,ownerType,"_commitHistory",historyType),Is.SameAs(originalCommit));
                    Assert.That(C4ActualExecutionPlayFixtureV1.Field(f.Presenter,presenterType,"_cursor",typeof(UiSemanticFrameCursorV1)),Is.SameAs(originalCursor));
                    Assert.That(C4ActualExecutionPlayFixtureV1.Field(f.HandoffOwner,handoffType,"_takenRequest",takeType),Is.EqualTo(originalTake));
                    if (cancellation==0) { evidence.Reached(); f.ObserveActualResult(evidence,busy); f.ObserveActualBarrier(evidence,busy); f.ObserveNoOpCallCounts(evidence,busy); }
                    if (cancellation==2) break;
                    var decision=C4ActualExecutionPlayFixtureV1.Read(start,C4ActualExecutionPlayFixtureV1.Hub("NewGameConfirmationStartResultV1"),"DecisionCapability",C4ActualExecutionPlayFixtureV1.Hub("NewGameDecisionCapabilityV1"));
                    var canceled=C4ActualExecutionPlayFixtureV1.Invoke(f.Owner,ownerType,"Cancel",C4ActualExecutionPlayFixtureV1.Hub("NewGameConfirmationStartResultV1"),new[] {C4ActualExecutionPlayFixtureV1.Hub("NewGameDecisionCapabilityV1")},decision);
                    var rearm=C4ActualExecutionPlayFixtureV1.Read(canceled,C4ActualExecutionPlayFixtureV1.Hub("NewGameConfirmationStartResultV1"),"RearmCapability",C4ActualExecutionPlayFixtureV1.Hub("HubNewGameRearmCapabilityV1"));
                    f.Publish();
                    C4ActualExecutionPlayFixtureV1.Invoke(f.Owner,ownerType,"Rearm",typeof(void),new[] {C4ActualExecutionPlayFixtureV1.Hub("HubNewGameRearmCapabilityV1")},rearm);
                    Assert.That(() => f.Intake(actualHandle),Throws.InvalidOperationException);
                }
                var confirmed=f.ConfirmActual(start);
                Assert.That(C4ActualExecutionPlayFixtureV1.Invoke(f.Owner,ownerType,"CommitForExecution",typeof(ConfirmedProfileResetRequestV1),new[] {typeof(ConfirmedProfileResetRequestV1)},confirmed),Is.SameAs(confirmed));
                Assert.That(ProfileNewGameConfirmationV1.ExecuteConfirmedReset(confirmed,f.Adapter,f.Router).Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                busy.Validate(); Assert.That(busy.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Busy));
                f.Dispose(); f.ObserveActualCleanup(evidence);
                evidence.Observe("cleanup.releaseSignaled",actualWorker.ReleaseSignaled ? "Yes" : "No");
                evidence.Observe("cleanup.workerLeaseReleased",actualWorker.LeaseReleased ? "Yes" : "No");
                evidence.Observe("cleanup.workerJoined",actualWorker.Joined ? "Yes" : "No");
            }
        }
        [Test]
        [Timeout(180000)]
        public void AC002_GuardedHistoryReadDoesNotPublishAndFreshCursorUsesActualPair()
        { ActualFreshCursor(false,true); }
        private void ActualFreshCursor(bool stale,bool checkGuardedHistory=false)
        {
            var method=TestContext.CurrentContext.Test.MethodName;
            var evidence=BeginEvidence("play_"+method,"FullBridge","None",null,null,new[] {"REQ-M5D7QC4-004"},new[] {checkGuardedHistory ? "AC-M5D7QC4-002" : "AC-M5D7QC4-004"});
            var successorEvidence=BeginEvidence("play_"+method+"_successor","FullBridge","None",null,null,new[] {"REQ-M5D7QC4-004"},new[] {checkGuardedHistory ? "AC-M5D7QC4-002" : "AC-M5D7QC4-004"});
            using (var f=C4ActualExecutionPlayFixtureV1.Create())
            {
                var oldTaken=C4ActualExecutionPlayFixtureV1.Field(f.HandoffOwner,C4ActualExecutionPlayFixtureV1.Hub("HubMenuIntentHandoffOwnerV1"),"_takenRequest",typeof(Nullable<>).MakeGenericType(C4ActualExecutionPlayFixtureV1.Hub("HubMenuIntentRequestV1")));
                var oldCursor=C4ActualExecutionPlayFixtureV1.Field(f.Presenter,C4ActualExecutionPlayFixtureV1.Hub("HubMenuPresenterV1"),"_cursor",typeof(UiSemanticFrameCursorV1));
                C4ActualExecutionPlayFixtureV1.RootLeaseWorker actualWorker=null;
                ProfileResetExecutionResultV1 result;
                if (stale)
                { File.AppendAllText(Path.Combine(f.Root,"profile.json"),"\n"); result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router); }
                else
                { using (actualWorker=new C4ActualExecutionPlayFixtureV1.RootLeaseWorker(f.Root)) result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router); }
                Assert.That(result.Outcome,Is.EqualTo(stale ? ProfileResetExecutionOutcomeV1.ConfirmationStale : ProfileResetExecutionOutcomeV1.Busy));
                if (checkGuardedHistory)
                {
                    var oldReceipt=f.Router.CurrentReceipt.Value;
                    var oldFrame=f.Router.CurrentUiFrame.Value;
                    var oldTick=C4ActualExecutionPlayFixtureV1.Field(f.Router,typeof(InputRouter),"_hubNextTick",typeof(AcadeGameMaker.Core.SimulationTick));
                    Assert.That(f.Router.CurrentReceipt.Value,Is.EqualTo(oldReceipt));
                    Assert.That(f.Router.CurrentUiFrame.Value,Is.EqualTo(oldFrame));
                    Assert.That(C4ActualExecutionPlayFixtureV1.Field(f.Router,typeof(InputRouter),"_hubNextTick",typeof(AcadeGameMaker.Core.SimulationTick)),Is.EqualTo(oldTick));
                    Assert.That(C4ActualExecutionPlayFixtureV1.Field(f.Presenter,C4ActualExecutionPlayFixtureV1.Hub("HubMenuPresenterV1"),"_cursor",typeof(UiSemanticFrameCursorV1)),Is.SameAs(oldCursor));
                    Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                    Assert.That(f.Router.CurrentUiFrame.Value,Is.EqualTo(oldFrame));
                    Assert.That(C4ActualExecutionPlayFixtureV1.Field(f.Router,typeof(InputRouter),"_hubNextTick",typeof(AcadeGameMaker.Core.SimulationTick)),Is.EqualTo(oldTick));
                }
                f.ReleaseDuringGuard(); f.AcceptFresh(result);
                f.Publish(Key.Enter); Assert.That(f.Router.CurrentUiFrame.Value.SubmitPressed,Is.True);
                f.PresenterFixed(); f.LateTakePoll();
                Assert.That(f.TryTake(out var first),Is.False); Assert.That(first,Is.Null);
                Assert.That(C4ActualExecutionPlayFixtureV1.Field(f.HandoffOwner,C4ActualExecutionPlayFixtureV1.Hub("HubMenuIntentHandoffOwnerV1"),"_takenRequest",typeof(Nullable<>).MakeGenericType(C4ActualExecutionPlayFixtureV1.Hub("HubMenuIntentRequestV1"))),Is.EqualTo(oldTaken));
                Assert.That(C4ActualExecutionPlayFixtureV1.Field(f.Presenter,C4ActualExecutionPlayFixtureV1.Hub("HubMenuPresenterV1"),"_cursor",typeof(UiSemanticFrameCursorV1)),Is.SameAs(oldCursor));
                evidence.Reached();
                f.ObserveActualResult(evidence,result); f.ObserveActualBarrier(evidence,result,actualWorker); f.ObserveNoOpCallCounts(evidence,result);
                f.Publish(); f.PresenterFixed(); f.LateTakePoll();
                Assert.That(f.TryTake(out _),Is.False);
                f.Publish(Key.Enter); Assert.That(f.Router.CurrentUiFrame.Value.SubmitPressed,Is.True); f.PresenterFixed(); f.LateTakePoll();
                Assert.That(f.TryTake(out var next),Is.True); Assert.That(next,Is.Not.Null);
                var confirmed=f.ConfirmActual(f.Intake(next)); Assert.That(confirmed,Is.Not.SameAs(f.Confirmed));
                Assert.That(C4ActualExecutionPlayFixtureV1.Invoke(f.Owner,C4ActualExecutionPlayFixtureV1.Hub("NewGameConfirmationOwnerV1"),"CommitForExecution",typeof(ConfirmedProfileResetRequestV1),new[] {typeof(ConfirmedProfileResetRequestV1)},confirmed),Is.SameAs(confirmed));
                var completed=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(confirmed,f.Adapter,f.Router);
                Assert.That(completed.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                result.Validate(); Assert.That(result.Outcome,Is.EqualTo(stale ? ProfileResetExecutionOutcomeV1.ConfirmationStale : ProfileResetExecutionOutcomeV1.Busy));
                successorEvidence.Reached();
                f.ObserveActualResult(successorEvidence,completed,confirmed);
                f.ObserveActualSuccessorLink(successorEvidence,"play_"+method,result,confirmed,completed);
                f.ObserveActualBarrier(successorEvidence,completed,null,confirmed);
                f.ObserveNoOpCallCounts(successorEvidence,completed,false,confirmed);
                f.ObserveAbsentFreshCompletion(successorEvidence,completed,confirmed);
                f.Dispose(); f.ObserveActualCleanup(evidence); f.ObserveActualCleanup(successorEvidence);
                if (actualWorker != null)
                {
                    evidence.Observe("cleanup.releaseSignaled",actualWorker.ReleaseSignaled ? "Yes" : "No");
                    evidence.Observe("cleanup.workerLeaseReleased",actualWorker.LeaseReleased ? "Yes" : "No");
                    evidence.Observe("cleanup.workerJoined",actualWorker.Joined ? "Yes" : "No");
                }
            }
        }
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        [TestCase(18)]
        [TestCase(19)]
        [Timeout(180000)]
        public void AC007_ActualC2CheckpointFailureIsTerminal(int memoryCheckpoint)
        {
            Assert.That(Enum.IsDefined(typeof(ProfileResetMemoryCutoverCheckpointV1),memoryCheckpoint),Is.True);
            var evidence=BeginEvidence("play_AC007_ActualC2CheckpointFailureIsTerminal_"+memoryCheckpoint,"FullBridge","C2",Enum.GetName(typeof(ProfileResetMemoryCutoverCheckpointV1),memoryCheckpoint),memoryCheckpoint,
                new[] {"REQ-M5D7QC4-003","REQ-M5D7QC4-006"},new[] {"AC-M5D7QC4-007"});
            using (var f=C4ActualExecutionPlayFixtureV1.Create())
            {
                var control=new MemoryCheckpointFailure((ProfileResetMemoryCutoverCheckpointV1)memoryCheckpoint,()=>evidence.Reached());
                var c1Calls=0; var c2Calls=0; var preparedObservers=0; var c2Observers=0; var reserveCalls=0; var nativeCallbacks=0;
                var actions=(GameInputActions)C4ActualExecutionPlayFixtureV1.Field(f.Router,typeof(InputRouter),"_actions",typeof(GameInputActions));
                Action<InputAction.CallbackContext> callback=context=>nativeCallbacks++;
                actions.UI.Submit.canceled+=callback;
                var execution=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint =>
                { if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) c1Calls++; if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) c2Calls++; if (checkpoint == ProfileResetExecutionCheckpointV1.AfterFreshReserve) reserveCalls++; },()=>{},proof=>preparedObservers++,memory=>c2Observers++);
                ProfileResetExecutionResultV1 result;
                try { result=f.Execute(execution,C4ActualExecutionPlayFixtureV1.DiskControl((name,value)=>{}),control); }
                finally { actions.UI.Submit.canceled-=callback; }
                Assert.That(control.Reached,Is.EqualTo(1),"계획한 실제 C2 지점에 도달하지 않았습니다.");
                Assert.That(control.ActualInvocations,Is.EqualTo(1),"실제 C2 첫 내부 경계를 관측합니다.");
                Assert.That(c2Calls,Is.EqualTo(1)); result.Validate();
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.ReloadRequired).Or.EqualTo(ProfileResetExecutionOutcomeV1.ManualRepairRequired));
                Assert.That(result.Phase,Is.EqualTo(ProfileResetExecutionPhaseV1.C2Invoked));
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                Assert.That(ProfileNewGameConfirmationV1.IsFreshExecutionAvailable(result,f.Adapter,f.Router,f.Owner,new object(),1,1),Is.False);
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router),Throws.InvalidOperationException);
                Assert.That(control.Reached,Is.EqualTo(1));
                Assert.That(nativeCallbacks,Is.EqualTo(1));
                f.ObserveActualResult(evidence,result); f.ObserveActualBarrier(evidence,result); f.ObserveAbsentFreshCompletion(evidence,result);
                evidence.Observe("callCounts.c1Begin",(long)c1Calls);
                evidence.Observe("callCounts.c2Finalize",(long)control.ActualInvocations);
                evidence.Observe("callCounts.preparedObserver",(long)preparedObservers);
                evidence.Observe("callCounts.c2Observer",(long)c2Observers);
                evidence.Observe("callCounts.freshReserve",(long)reserveCalls);
                evidence.Observe("callCounts.nativeCallback",(long)nativeCallbacks);
                evidence.Observe("diagnostic.c2NormalReturnCount",(long)c2Calls);
                f.Dispose(); f.ObserveActualCleanup(evidence);
            }
        }
        private sealed class MemoryCheckpointFailure : IProfileResetMemoryCutoverControlV1
        {
            private readonly ProfileResetMemoryCutoverCheckpointV1 _target;
            internal int Reached, ActualInvocations;
            private readonly Action _reached;
            internal MemoryCheckpointFailure(ProfileResetMemoryCutoverCheckpointV1 target,Action reached) { _target=target; _reached=reached; }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 checkpoint)
            { if (checkpoint == ProfileResetMemoryCutoverCheckpointV1.BeforeStaging) ActualInvocations++; if (checkpoint == _target) { Reached++; _reached(); throw new InvalidOperationException("실제 C2 지점 종료 시험: "+checkpoint); } }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint) { }
            public void InvokeOperation(ResetCutoverOperationV1 operation,Action exactOwnedOperation)
            { if (exactOwnedOperation == null) throw new ArgumentNullException(nameof(exactOwnedOperation)); exactOwnedOperation(); }
        }
        [Test]
        [Timeout(180000)]
        public void AC007_ActualCompletedReceiptLeavesAllMapsDisabledAndUIOnlyBlocked()
        {
            var evidence=BeginEvidence("play_AC007_ActualCompletedReceiptLeavesAllMapsDisabledAndUIOnlyBlocked","FullBridge","None",null,null,new[] {"REQ-M5D7QC4-005"},new[] {"AC-M5D7QC4-007"});
            using (var f=C4ActualExecutionPlayFixtureV1.Create())
            {
                var result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router);
                result.Validate(); Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(result.Phase,Is.EqualTo(ProfileResetExecutionPhaseV1.Completed));
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                var actions=(GameInputActions)C4ActualExecutionPlayFixtureV1.Field(f.Router,typeof(InputRouter),"_actions",typeof(GameInputActions));
                Assert.That(actions,Is.Not.Null); Assert.That(actions.asset,Is.Not.Null);
                foreach (var map in actions.asset.actionMaps) Assert.That(map.enabled,Is.False);
                Assert.That(f.Adapter.CurrentResetSession.Generation,Is.GreaterThan(0));
                ProfileResetMemoryReceiptV1 receipt;
                Assert.That(f.Adapter.TryTakeResetReceipt(out receipt),Is.True); receipt.Validate();
                Assert.That(receipt.UiOnlyBlocked,Is.True);
                Assert.That(f.Adapter.TryTakeResetReceipt(out _),Is.False);
                Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router),Throws.InvalidOperationException);
                result.Validate(); receipt.Validate();
                evidence.Reached(); f.ObserveActualResult(evidence,result); f.ObserveActualBarrier(evidence,result);
                f.ObserveNoOpCallCounts(evidence,result,false);
                f.Dispose(); f.ObserveActualCleanup(evidence);
            }
        }
        [Test]
        [Timeout(180000)]
        public void AC008_ActualOldSubmitCancelReentryRejectsConsumedRequest()
        {
            var evidence=BeginEvidence("play_AC008_ActualOldSubmitCancelReentryRejectsConsumedRequest","ActualCallback","None",null,null,new[] {"REQ-M5D7QC4-001"},new[] {"AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionPlayFixtureV1.Create())
            {
                var actions=(GameInputActions)C4ActualExecutionPlayFixtureV1.Field(f.Router,typeof(InputRouter),"_actions",typeof(GameInputActions));
                var canceled=0; var rejected=0; Exception callbackFailure=null;
                Action<InputAction.CallbackContext> callback=context =>
                {
                    canceled++;
                    try { ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router); }
                    catch (InvalidOperationException) { rejected++; }
                    catch (Exception error) { callbackFailure=error; }
                };
                actions.UI.Submit.canceled+=callback;
                ProfileResetExecutionResultV1 result;
                try { result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router); }
                finally { actions.UI.Submit.canceled-=callback; }
                Assert.That(callbackFailure,Is.Null); Assert.That(canceled,Is.EqualTo(1)); Assert.That(rejected,Is.EqualTo(canceled));
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.False);
                result.Validate();
                evidence.Reached(); f.ObserveActualResult(evidence,result); f.ObserveActualBarrier(evidence,result);
                f.ObserveNoOpCallCounts(evidence,result,true);
                evidence.Observe("callCounts.nativeCallback",(long)canceled);
                f.Dispose(); f.ObserveActualCleanup(evidence);
            }
        }
    }
}
