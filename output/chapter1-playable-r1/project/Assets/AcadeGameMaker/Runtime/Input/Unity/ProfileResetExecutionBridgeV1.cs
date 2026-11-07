using System;
using System.Runtime.CompilerServices;
using System.Threading;
using AcadeGameMaker.Profile;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-M5D7QC4-001..007 / AC-M5D7QC4-001..009.
    internal enum ProfileResetExecutionOutcomeV1 { Completed=1, Busy=2, ConfirmationStale=3, ReloadRequired=4, ManualRepairRequired=5 }
    internal enum ProfileResetExecutionPhaseV1 { PreC1=1, C1Returned=2, C2Invoked=3, Completed=4 }
    internal enum ProfileResetExecutionCheckpointV1
    {
        BeforeConfirmedConsume=1,
        AfterConfirmedConsume=2,
        BeforeAdapterGuard=3,
        AfterAdapterGuard=4,
        BeforeRouterGuard=5,
        AfterRouterGuard=6,
        BeforeC1Begin=7,
        AfterC1Begin=8,
        BeforeC1ResultValidate=9,
        AfterC1ResultValidate=10,
        BeforePreparedProofTake=11,
        AfterPreparedProofTake=12,
        BeforeC2Finalize=13,
        AfterC2Finalize=14,
        BeforeC2ResultMap=15,
        AfterC2ResultMap=16,
        BeforeFreshHandbackPublish=17,
        AfterFreshHandbackPublish=18,
        BeforeFreshReserve=19,
        AfterFreshReserve=20,
        AfterFreshPrepare=21,
        AfterFreshAcknowledge=22,
        AfterFreshCommit=23,
        BeforeFreshConsume=24,
        AfterFreshConsume=25,
        BeforeTerminalPublish=26,
        AfterTerminalPublish=27
    }
    internal interface IProfileResetExecutionTestControlV1
    {
        void Checkpoint(ProfileResetExecutionCheckpointV1 checkpoint);
        void PostDiskPreparedValidatedPreC2();
        void ObserveActualPreparedProofBeforeRowValidation(ProfileResetDiskPreparedProofV1 actual);
        void ObserveActualC2ResultBeforeComposition(ProfileResetMemoryResultV1 actual);
    }
    internal sealed class ProfileResetExecutionNoOpControlV1 : IProfileResetExecutionTestControlV1
    {
        internal static readonly ProfileResetExecutionNoOpControlV1 Instance=new ProfileResetExecutionNoOpControlV1();
        private ProfileResetExecutionNoOpControlV1() { }
        public void Checkpoint(ProfileResetExecutionCheckpointV1 checkpoint) { }
        public void PostDiskPreparedValidatedPreC2() { }
        public void ObserveActualPreparedProofBeforeRowValidation(ProfileResetDiskPreparedProofV1 actual) { }
        public void ObserveActualC2ResultBeforeComposition(ProfileResetMemoryResultV1 actual) { }
    }
    // AC-M5D7QC4-003/005: 기존 관찰 지점의 동일 객체만 전달하는 시험 제어 연결이다.
    internal static class ProfileResetExecutionTestControlFactoryV1
    {
        internal static IProfileResetExecutionTestControlV1 Create(Action<ProfileResetExecutionCheckpointV1> checkpoint,
            Action postDiskPreparedValidatedPreC2,Action<object> actualPreparedProof,Action<object> actualC2Result)
        {
            if (checkpoint == null || postDiskPreparedValidatedPreC2 == null || actualPreparedProof == null || actualC2Result == null) throw new ArgumentNullException();
            return new Control(checkpoint,postDiskPreparedValidatedPreC2,actualPreparedProof,actualC2Result);
        }
        private sealed class Control : IProfileResetExecutionTestControlV1
        {
            private readonly Action<ProfileResetExecutionCheckpointV1> _checkpoint;
            private readonly Action _prepared;
            private readonly Action<object> _proof, _memory;
            internal Control(Action<ProfileResetExecutionCheckpointV1> checkpoint,Action prepared,Action<object> proof,Action<object> memory)
            { _checkpoint=checkpoint; _prepared=prepared; _proof=proof; _memory=memory; }
            public void Checkpoint(ProfileResetExecutionCheckpointV1 checkpoint) { _checkpoint(checkpoint); }
            public void PostDiskPreparedValidatedPreC2() { _prepared(); }
            public void ObserveActualPreparedProofBeforeRowValidation(ProfileResetDiskPreparedProofV1 actual) { _proof(actual); }
            public void ObserveActualC2ResultBeforeComposition(ProfileResetMemoryResultV1 actual) { _memory(actual); }
        }
    }
    internal sealed class ProfileResetFreshC3HandbackV1 { }
    internal sealed class ProfileResetFreshExecutionReservationV1 { }
    internal sealed class ProfileResetExecutionGuardContextV1 { }
    internal enum ProfileResetExecutionGuardOperationV1 { Enter=1, CompleteFresh=2, Close=3 }
    internal sealed class ProfileResetExecutionResultV1
    {
        private readonly ProfileResetExecutionOutcomeV1 _outcome;
        private readonly ProfileResetExecutionPhaseV1 _phase;
        private readonly long _executionGeneration;
        internal ProfileResetExecutionResultV1(ProfileResetExecutionOutcomeV1 outcome,ProfileResetExecutionPhaseV1 phase,long executionGeneration)
        { _outcome=outcome; _phase=phase; _executionGeneration=executionGeneration; }
        internal ProfileResetExecutionOutcomeV1 Outcome { get { Validate(); return _outcome; } }
        internal ProfileResetExecutionPhaseV1 Phase { get { Validate(); return _phase; } }
        internal long ExecutionGeneration { get { Validate(); return _executionGeneration; } }
        internal void Validate() { ProfileNewGameConfirmationV1.ValidateExecutionResult(this,_outcome,_phase,_executionGeneration); }
    }

    internal static partial class ProfileNewGameConfirmationV1
    {
        private enum ExecutionEventKind { Consumed, GuardEntered, Returned, Faulted, FreshReserving, FreshReserved, FreshCompleted, FreshClosed }
        private sealed class ExecutionEvent
        {
            internal readonly ExecutionEventKind Kind;
            internal readonly ExecutionEvent Previous;
            internal ExecutionEvent(ExecutionEventKind kind,ExecutionEvent previous=null) { Kind=kind; Previous=previous; }
        }
        private sealed class ExecutionRecord
        {
            internal readonly object Gate=new object();
            internal readonly ConfirmedProfileResetRequestV1 Confirmed;
            internal readonly ConfirmedRequestWitness Witness;
            internal readonly RequestLifecycle Lifecycle;
            internal readonly DesktopProfileLaunchAdapterV1 Adapter;
            internal readonly InputRouter Router;
            internal readonly object Owner, Epoch;
            internal readonly long InteractionGeneration, DecisionGeneration, Generation;
            internal readonly string Root;
            internal readonly Thread Thread;
            internal readonly ProfileResetExecutionGuardContextV1 Guard=new ProfileResetExecutionGuardContextV1();
            internal readonly ExecutionEvent Consumed=new ExecutionEvent(ExecutionEventKind.Consumed);
            internal readonly ExecutionEvent Failure=new ExecutionEvent(ExecutionEventKind.Faulted);
            internal readonly IProfileResetExecutionTestControlV1 Control;
            internal ExecutionEvent Head, ConsumedHistory, FaultHistory;
            internal ProfileResetExecutionGuardOperationV1 GuardOperation;
            internal bool AdapterEntered, RouterEntered, AdapterFreshCompleted, RouterFreshCompleted, NativeClosed, NativeFaultClosed;
            internal ProfileResetExecutionResultV1 Result;
            internal ResultRecord ResultRecord;
            internal ExecutionRecord(ConfirmedProfileResetRequestV1 confirmed,ConfirmedRequestWitness witness,RequestLifecycle lifecycle,IProfileResetExecutionTestControlV1 control,long generation)
            { Confirmed=confirmed; Witness=witness; Lifecycle=lifecycle;
              var issued=FindOriginalRequestIssue(lifecycle); var source=issued.ResultWitness;
              Adapter=source.Adapter; Router=source.Router; Owner=source.Owner; Epoch=source.Epoch;
              InteractionGeneration=lifecycle.IssuerInteractionGeneration;
              DecisionGeneration=lifecycle.IssuerDecisionGeneration; Root=source.Root; Thread=lifecycle.IssuerThreadAnchor;
              Control=control; Generation=generation; }
        }
        private sealed class ResultRecord
        {
            internal readonly ExecutionRecord Execution;
            internal readonly ProfileResetDiskResultV1 Disk;
            internal readonly ProfileResetMemoryResultV1 Memory;
            internal readonly ProfileResetMemoryReceiptV1 Receipt;
            internal readonly ProfileResetExecutionOutcomeV1 Outcome;
            internal readonly ProfileResetExecutionPhaseV1 Phase;
            internal readonly ProfileResetFreshC3HandbackV1 Handback;
            internal readonly ExecutionEvent Returned;
            internal FreshRecord Reservation;
            internal ExecutionEvent FreshHead, FreshClosedHistory;
            internal readonly ExecutionEvent FreshFailure=new ExecutionEvent(ExecutionEventKind.FreshClosed);
            internal ResultRecord(ExecutionRecord execution,ProfileResetDiskResultV1 disk,ProfileResetMemoryResultV1 memory,
                ProfileResetExecutionOutcomeV1 outcome,ProfileResetExecutionPhaseV1 phase,ProfileResetFreshC3HandbackV1 handback)
            { Execution=execution; Disk=disk; Memory=memory; Receipt=memory == null ? null : memory.Receipt;
              Outcome=outcome; Phase=phase; Handback=handback; Returned=new ExecutionEvent(ExecutionEventKind.Returned,execution.Head); }
        }
        private sealed class FreshRecord
        {
            internal readonly ProfileResetFreshExecutionReservationV1 Reservation;
            internal readonly ResultRecord Result;
            internal readonly object NextToken;
            internal readonly long NextEpoch;
            internal readonly ExecutionEvent Reserved, Completed;
            internal FreshCheckpointEvent CheckpointAttempt, CheckpointCompleted;
            internal FreshRecord(ProfileResetFreshExecutionReservationV1 reservation,ResultRecord result,object token,long epoch)
            { Reservation=reservation; Result=result; NextToken=token; NextEpoch=epoch;
              Reserved=new ExecutionEvent(ExecutionEventKind.FreshReserved,result.Returned);
              Completed=new ExecutionEvent(ExecutionEventKind.FreshCompleted,Reserved); }
        }
        private sealed class FreshCheckpointEvent
        {
            internal readonly FreshCheckpointEvent Previous;
            internal readonly ProfileResetExecutionCheckpointV1 Checkpoint;
            internal FreshCheckpointEvent(FreshCheckpointEvent previous,ProfileResetExecutionCheckpointV1 checkpoint)
            { Previous=previous; Checkpoint=checkpoint; }
        }
        private static long _executionGeneration;
        private static readonly ConditionalWeakTable<ConfirmedProfileResetRequestV1,ExecutionRecord> Executions=new ConditionalWeakTable<ConfirmedProfileResetRequestV1,ExecutionRecord>();
        private static readonly ConditionalWeakTable<ProfileResetExecutionGuardContextV1,ExecutionRecord> ExecutionGuards=new ConditionalWeakTable<ProfileResetExecutionGuardContextV1,ExecutionRecord>();
        private static readonly ConditionalWeakTable<DesktopProfileLaunchAdapterV1,ExecutionRecord> GuardedAdapters=new ConditionalWeakTable<DesktopProfileLaunchAdapterV1,ExecutionRecord>();
        private static readonly ConditionalWeakTable<InputRouter,ExecutionRecord> GuardedRouters=new ConditionalWeakTable<InputRouter,ExecutionRecord>();
        private static readonly ConditionalWeakTable<ProfileResetExecutionResultV1,ResultRecord> ExecutionResults=new ConditionalWeakTable<ProfileResetExecutionResultV1,ResultRecord>();
        private static readonly ConditionalWeakTable<ProfileResetFreshC3HandbackV1,ResultRecord> FreshHandbacks=new ConditionalWeakTable<ProfileResetFreshC3HandbackV1,ResultRecord>();
        private static readonly ConditionalWeakTable<ProfileResetFreshExecutionReservationV1,FreshRecord> ExecutionReservations=new ConditionalWeakTable<ProfileResetFreshExecutionReservationV1,FreshRecord>();

        // 최초에는 관리 참조만 검사하며 Unity의 null 연산자와 속성을 읽지 않는다.
        private static bool TryTrustedExecutionSource(ConfirmedProfileResetRequestV1 confirmed,DesktopProfileLaunchAdapterV1 adapter,
            InputRouter router,object owner,out ConfirmedRequestWitness witness,out RequestLifecycle lifecycle)
        {
            witness=null; lifecycle=null;
            if (ReferenceEquals(confirmed,null) || ReferenceEquals(adapter,null) || ReferenceEquals(router,null) ||
                !IssuedConfirmedRequests.TryGetValue(confirmed,out witness) || ReferenceEquals(witness,null) ||
                !RequestLifecycles.TryGetValue(witness,out lifecycle) || ReferenceEquals(lifecycle,null) ||
                !ReferenceEquals(lifecycle.Witness,witness)) return false;
            var issued=FindOriginalRequestIssue(lifecycle);
            return issued != null && ReferenceEquals(issued.Request,confirmed) && ReferenceEquals(issued.RequestWitness,witness) &&
                !ReferenceEquals(lifecycle.IssuerThreadAnchor,null) && ReferenceEquals(lifecycle.IssuerThreadAnchor,issued.IssuerThreadAtIssue) &&
                ReferenceEquals(lifecycle.IssuerThreadAnchor,Thread.CurrentThread) && issued.ResultWitness != null &&
                ReferenceEquals(issued.ResultWitness.Adapter,adapter) && ReferenceEquals(issued.ResultWitness.Router,router) &&
                (owner == null || ReferenceEquals(issued.ResultWitness.Owner,owner));
        }
        private static LifecycleEvent FindOriginalRequestIssue(RequestLifecycle lifecycle)
        {
            var issued=lifecycle.Head;
            for (var i=0;i<4 && issued != null;i++)
            { if (issued.Kind == EventKind.RequestIssued) return issued; issued=issued.Previous; }
            return null;
        }
        internal static bool IsExecutionAvailable(ConfirmedProfileResetRequestV1 confirmed,DesktopProfileLaunchAdapterV1 adapter,InputRouter router,object ownerToken)
        {
            return !ReferenceEquals(ownerToken,null) && TryTrustedExecutionSource(confirmed,adapter,router,ownerToken,out var witness,out var lifecycle) &&
                lifecycle.Head != null && lifecycle.Head.Kind == EventKind.Completed && !Executions.TryGetValue(confirmed,out _);
        }
        internal static bool IsExecutionFaulted(ConfirmedProfileResetRequestV1 confirmed,DesktopProfileLaunchAdapterV1 adapter,InputRouter router,object ownerToken)
        {
            return TryTrustedExecutionSource(confirmed,adapter,router,ownerToken,out _,out _) && Executions.TryGetValue(confirmed,out var e) &&
                ReferenceEquals(e.Adapter,adapter) && ReferenceEquals(e.Router,router) && ReferenceEquals(e.Owner,ownerToken) && HasExecutionFault(e);
        }
        internal static ProfileResetExecutionResultV1 ExecuteConfirmedReset(ConfirmedProfileResetRequestV1 confirmed,DesktopProfileLaunchAdapterV1 adapter,InputRouter router)
        { return ExecuteConfirmedReset(confirmed,adapter,router,ProfileResetExecutionNoOpControlV1.Instance,ProfileResetDiskNoOpTestControlV1.Instance,ProfileResetMemoryCutoverNoOpControlV1.Instance); }
        internal static ProfileResetExecutionResultV1 ExecuteConfirmedReset(ConfirmedProfileResetRequestV1 confirmed,DesktopProfileLaunchAdapterV1 adapter,InputRouter router,
            IProfileResetExecutionTestControlV1 control,IProfileResetDiskTestControlV1 diskControl,IProfileResetMemoryCutoverControlV1 memoryControl)
        {
            // REQ001/AC001/008: 지원하지 않는 문맥과 정상 소비 패자는 검증·정리 경계 밖이다.
            if (!TryTrustedExecutionSource(confirmed,adapter,router,null,out var witness,out var lifecycle) ||
                lifecycle.Head.Kind != EventKind.Completed || Executions.TryGetValue(confirmed,out _)) throw new InvalidOperationException("C4 execution is unavailable in this context.");
            if (control == null || diskControl == null || memoryControl == null) throw new ArgumentNullException();
            ExecutionRecord execution;
            lock (lifecycle.Gate)
            {
                if (!TryTrustedExecutionSource(confirmed,adapter,router,null,out var sameWitness,out var sameLifecycle) ||
                    !ReferenceEquals(sameWitness,witness) || !ReferenceEquals(sameLifecycle,lifecycle) ||
                    lifecycle.Head.Kind != EventKind.Completed || Executions.TryGetValue(confirmed,out _)) throw new InvalidOperationException("C4 execution completed before entry.");
                var generation=Interlocked.Increment(ref _executionGeneration);
                if (generation <= 0) throw new InvalidOperationException("C4 generation overflow.");
                execution=new ExecutionRecord(confirmed,witness,lifecycle,control,generation);
                Executions.Add(confirmed,execution);
            }
            try
            {
                ExecutionGuards.Add(execution.Guard,execution);
                if (!ReferenceEquals(witness.BirthThreadProjection,execution.Thread)) throw new InvalidOperationException("C4 birth thread projection is malformed.");
                if (!ReferenceEquals(witness.Adapter,execution.Adapter) || !ReferenceEquals(witness.Router,execution.Router) ||
                    !ReferenceEquals(witness.Owner,execution.Owner) || !ReferenceEquals(witness.Epoch,execution.Epoch) ||
                    !string.Equals(witness.Root,execution.Root,StringComparison.Ordinal) || witness.InteractionGeneration <= 0 || witness.DecisionGeneration <= 0 ||
                    witness.InteractionGeneration != execution.InteractionGeneration || witness.DecisionGeneration != execution.DecisionGeneration)
                    throw new InvalidOperationException("C4 request projection disagrees with its original issuance.");
                ValidateRequestHistory(confirmed,witness,lifecycle);
                ValidateRequestAtActiveRoot(witness,adapter,router);
                control.Checkpoint(ProfileResetExecutionCheckpointV1.BeforeConfirmedConsume);
                execution.ConsumedHistory=execution.Consumed;
                execution.Head=execution.Consumed;
                control.Checkpoint(ProfileResetExecutionCheckpointV1.AfterConfirmedConsume);
                // 같은 pair의 정상 fresh 뒤에는 이전 원장의 이력을 남기고 현재 연결만 교체한다.
                if (GuardedAdapters.TryGetValue(adapter,out var previousAdapter) && !IsFreshCompleted(previousAdapter)) throw new InvalidOperationException("C4 adapter is already guarded.");
                if (GuardedRouters.TryGetValue(router,out var previousRouter) && !IsFreshCompleted(previousRouter)) throw new InvalidOperationException("C4 router is already guarded.");
                GuardedAdapters.Remove(adapter); GuardedRouters.Remove(router);
                GuardedAdapters.Add(adapter,execution); GuardedRouters.Add(router,execution);
                execution.GuardOperation=ProfileResetExecutionGuardOperationV1.Enter;
                control.Checkpoint(ProfileResetExecutionCheckpointV1.BeforeAdapterGuard);
                adapter.ApplyExecutionGuard(execution.Guard,ProfileResetExecutionGuardOperationV1.Enter); execution.AdapterEntered=true;
                control.Checkpoint(ProfileResetExecutionCheckpointV1.AfterAdapterGuard);
                control.Checkpoint(ProfileResetExecutionCheckpointV1.BeforeRouterGuard);
                router.ApplyExecutionGuard(execution.Guard,ProfileResetExecutionGuardOperationV1.Enter); execution.RouterEntered=true;
                execution.Head=new ExecutionEvent(ExecutionEventKind.GuardEntered,execution.Head);
                control.Checkpoint(ProfileResetExecutionCheckpointV1.AfterRouterGuard);
                control.Checkpoint(ProfileResetExecutionCheckpointV1.BeforeC1Begin);
                ValidateRequestAtActiveRoot(witness,adapter,router);
                var disk=ProfileResetDiskTransactionV1.Begin(execution.Root,witness.Capture.Identity,diskControl);
                control.Checkpoint(ProfileResetExecutionCheckpointV1.AfterC1Begin);
                control.Checkpoint(ProfileResetExecutionCheckpointV1.BeforeC1ResultValidate);
                ValidateExecutionDiskRow(disk);
                control.Checkpoint(ProfileResetExecutionCheckpointV1.AfterC1ResultValidate);
                ProfileResetMemoryResultV1 memory=null;
                ProfileResetExecutionOutcomeV1 outcome;
                ProfileResetExecutionPhaseV1 phase;
                if (disk.Outcome == ProfileResetDiskOutcomeV1.DiskPrepared)
                {
                    control.Checkpoint(ProfileResetExecutionCheckpointV1.BeforePreparedProofTake);
                    var proof=disk.PreparedProof;
                    control.ObserveActualPreparedProofBeforeRowValidation(proof);
                    ValidateExecutionDiskRow(disk);
                    control.Checkpoint(ProfileResetExecutionCheckpointV1.AfterPreparedProofTake);
                    control.PostDiskPreparedValidatedPreC2();
                    control.Checkpoint(ProfileResetExecutionCheckpointV1.BeforeC2Finalize);
                    if (!string.Equals(adapter.GetAuthenticatedLaunchRootForNewGame(router),execution.Root,StringComparison.OrdinalIgnoreCase) ||
                        !IsExecutionGuardedPair(adapter,router)) throw new InvalidOperationException("C4 original pair changed before cutover.");
                    memory=ProfileResetMemoryCutoverV1.FinalizeReset(execution.Root,proof,adapter,router,memoryControl);
                    control.Checkpoint(ProfileResetExecutionCheckpointV1.AfterC2Finalize);
                    control.ObserveActualC2ResultBeforeComposition(memory);
                    control.Checkpoint(ProfileResetExecutionCheckpointV1.BeforeC2ResultMap);
                    if (memory == null) throw new InvalidOperationException("C4 memory result is absent.");
                    memory.Validate();
                    outcome=memory.Outcome == ProfileResetMemoryOutcomeV1.Completed ? ProfileResetExecutionOutcomeV1.Completed :
                        memory.Outcome == ProfileResetMemoryOutcomeV1.ManualRepairRequired ? ProfileResetExecutionOutcomeV1.ManualRepairRequired : ProfileResetExecutionOutcomeV1.ReloadRequired;
                    phase=outcome == ProfileResetExecutionOutcomeV1.Completed ? ProfileResetExecutionPhaseV1.Completed : ProfileResetExecutionPhaseV1.C2Invoked;
                    control.Checkpoint(ProfileResetExecutionCheckpointV1.AfterC2ResultMap);
                }
                else
                {
                    phase=ProfileResetExecutionPhaseV1.C1Returned;
                    outcome=disk.Outcome == ProfileResetDiskOutcomeV1.Busy ? ProfileResetExecutionOutcomeV1.Busy :
                        disk.Outcome == ProfileResetDiskOutcomeV1.ConfirmationStale ? ProfileResetExecutionOutcomeV1.ConfirmationStale :
                        disk.Outcome == ProfileResetDiskOutcomeV1.ManualRepairRequired ? ProfileResetExecutionOutcomeV1.ManualRepairRequired : ProfileResetExecutionOutcomeV1.ReloadRequired;
                }
                var fresh=outcome == ProfileResetExecutionOutcomeV1.Busy || outcome == ProfileResetExecutionOutcomeV1.ConfirmationStale;
                control.Checkpoint(fresh ? ProfileResetExecutionCheckpointV1.BeforeFreshHandbackPublish : ProfileResetExecutionCheckpointV1.BeforeTerminalPublish);
                var handback=fresh ? new ProfileResetFreshC3HandbackV1() : null;
                var row=new ResultRecord(execution,disk,memory,outcome,phase,handback);
                var result=new ProfileResetExecutionResultV1(outcome,phase,execution.Generation);
                execution.ResultRecord=row; execution.Result=result;
                if (handback != null) FreshHandbacks.Add(handback,row);
                ExecutionResults.Add(result,row);
                execution.Head=row.Returned;
                result.Validate();
                if (!fresh) CloseOriginalExecution(execution);
                control.Checkpoint(fresh ? ProfileResetExecutionCheckpointV1.AfterFreshHandbackPublish : ProfileResetExecutionCheckpointV1.AfterTerminalPublish);
                return result;
            }
            catch { FaultOriginalExecution(execution); throw; }
        }
        internal static void ValidateExecutionDiskRow(ProfileResetDiskResultV1 actual) { actual.Validate(); }
        private static bool IsFreshCompleted(ExecutionRecord e)
        { return e != null && !HasExecutionFault(e) && e.ResultRecord != null && e.ResultRecord.Reservation != null && ReferenceEquals(e.ResultRecord.FreshHead,e.ResultRecord.Reservation.Completed) && e.AdapterFreshCompleted && e.RouterFreshCompleted; }
        internal static void ValidateExecutionResult(ProfileResetExecutionResultV1 result,ProfileResetExecutionOutcomeV1 outcome,ProfileResetExecutionPhaseV1 phase,long generation)
        {
            if (result == null || !ExecutionResults.TryGetValue(result,out var row) || row == null || !ReferenceEquals(row.Execution.Result,result) ||
                !ReferenceEquals(row.Execution.ResultRecord,row) || !ReferenceEquals(row.Execution.ConsumedHistory,row.Execution.Consumed) ||
                row.Outcome != outcome || row.Phase != phase || row.Execution.Generation != generation || generation <= 0)
                throw new InvalidOperationException("C4 result has no original execution association.");
            ValidateExecutionDiskRow(row.Disk);
            if (row.Memory != null) row.Memory.Validate();
            var d=row.Disk.Outcome; var s=row.Disk.DurableState;
            var m=row.Memory == null ? (ProfileResetMemoryOutcomeV1?)null : row.Memory.Outcome;
            bool valid;
            if (outcome == ProfileResetExecutionOutcomeV1.Busy || outcome == ProfileResetExecutionOutcomeV1.ConfirmationStale)
                valid=phase == ProfileResetExecutionPhaseV1.C1Returned && s == ProfileResetDurableStateV1.NoBarrier &&
                    d == (outcome == ProfileResetExecutionOutcomeV1.Busy ? ProfileResetDiskOutcomeV1.Busy : ProfileResetDiskOutcomeV1.ConfirmationStale) &&
                    m == null && row.Receipt == null && row.Handback != null && FreshHandbacks.TryGetValue(row.Handback,out var bound) && ReferenceEquals(bound,row);
            else if (outcome == ProfileResetExecutionOutcomeV1.Completed)
                valid=phase == ProfileResetExecutionPhaseV1.Completed && d == ProfileResetDiskOutcomeV1.DiskPrepared && s == ProfileResetDurableStateV1.DiskPrepared &&
                    m == ProfileResetMemoryOutcomeV1.Completed && row.Receipt != null && ReferenceEquals(row.Memory.Receipt,row.Receipt) && row.Handback == null;
            else if (phase == ProfileResetExecutionPhaseV1.C2Invoked)
                valid=d == ProfileResetDiskOutcomeV1.DiskPrepared && s == ProfileResetDurableStateV1.DiskPrepared && row.Handback == null && row.Receipt == null &&
                    (outcome == ProfileResetExecutionOutcomeV1.ReloadRequired && (m == ProfileResetMemoryOutcomeV1.Busy || m == ProfileResetMemoryOutcomeV1.ReloadRequired) ||
                     outcome == ProfileResetExecutionOutcomeV1.ManualRepairRequired && m == ProfileResetMemoryOutcomeV1.ManualRepairRequired);
            else
                valid=phase == ProfileResetExecutionPhaseV1.C1Returned && m == null && row.Receipt == null && row.Handback == null &&
                    (outcome == ProfileResetExecutionOutcomeV1.ReloadRequired && d == ProfileResetDiskOutcomeV1.ReloadRequired && s == ProfileResetDurableStateV1.DefaultCommitUncertain ||
                     outcome == ProfileResetExecutionOutcomeV1.ManualRepairRequired && d == ProfileResetDiskOutcomeV1.ManualRepairRequired && s == ProfileResetDurableStateV1.ManualRepairRequired);
            if (!valid) throw new InvalidOperationException("C4 result does not match its whole typed rows.");
        }
        private static bool TryFreshResult(ProfileResetExecutionResultV1 result,DesktopProfileLaunchAdapterV1 adapter,InputRouter router,object owner,out ResultRecord row)
        {
            row=null;
            if (ReferenceEquals(result,null) || !ExecutionResults.TryGetValue(result,out row) || row == null || row.Handback == null ||
                !FreshHandbacks.TryGetValue(row.Handback,out var handback) || !ReferenceEquals(handback,row)) return false;
            var e=row.Execution;
            return TryTrustedExecutionSource(e.Confirmed,adapter,router,owner,out _,out _) &&
                ReferenceEquals(e.Adapter,adapter) && ReferenceEquals(e.Router,router) && ReferenceEquals(e.Owner,owner) &&
                ReferenceEquals(e.Thread,Thread.CurrentThread) && !HasExecutionFault(e) && !e.NativeClosed &&
                ReferenceEquals(e.Result,result) && ReferenceEquals(e.ConsumedHistory,e.Consumed);
        }
        internal static bool IsFreshExecutionAvailable(ProfileResetExecutionResultV1 result,DesktopProfileLaunchAdapterV1 adapter,InputRouter router,
            object ownerToken,object committedEpochToken,long committedInteractionGeneration,long committedDecisionGeneration)
        {
            return !ReferenceEquals(ownerToken,null) && TryFreshResult(result,adapter,router,ownerToken,out var row) &&
                row.FreshHead == null && row.Reservation == null && row.FreshClosedHistory == null &&
                ReferenceEquals(row.Execution.Epoch,committedEpochToken) &&
                row.Execution.Lifecycle.IssuerInteractionGeneration == committedInteractionGeneration &&
                row.Execution.Lifecycle.IssuerDecisionGeneration == committedDecisionGeneration &&
                row.Execution.Lifecycle.Head != null && row.Execution.Lifecycle.Head.Kind == EventKind.Completed &&
                (row.Outcome == ProfileResetExecutionOutcomeV1.Busy || row.Outcome == ProfileResetExecutionOutcomeV1.ConfirmationStale) &&
                row.Phase == ProfileResetExecutionPhaseV1.C1Returned;
        }
        internal static void RecordExecutionLifecycleFault(object initiatingHalf)
        {
            ExecutionRecord e;
            if (initiatingHalf is DesktopProfileLaunchAdapterV1 adapter)
            { if (!GuardedAdapters.TryGetValue(adapter,out e)) return; }
            else if (initiatingHalf is InputRouter router)
            { if (!GuardedRouters.TryGetValue(router,out e)) return; }
            else return;
            if (!ReferenceEquals(e.Thread,Thread.CurrentThread)) throw new InvalidOperationException("C4 lifecycle requires its original thread.");
            lock (e.Gate) { RecordExecutionFault(e); }
            if (!e.NativeClosed) CloseOriginalExecution(e);
        }
        internal static ProfileResetFreshExecutionReservationV1 ReserveFreshExecution(ProfileResetExecutionResultV1 result,DesktopProfileLaunchAdapterV1 adapter,InputRouter router,
            object ownerToken,object committedEpochToken,long committedInteractionGeneration,long committedDecisionGeneration,object nextEpochToken,long nextInteractionGeneration)
        {
            if (!TryFreshResult(result,adapter,router,ownerToken,out var row) || row.FreshHead != null || row.Reservation != null || row.FreshClosedHistory != null ||
                !ReferenceEquals(row.Execution.Epoch,committedEpochToken) || row.Execution.InteractionGeneration != committedInteractionGeneration ||
                row.Execution.DecisionGeneration != committedDecisionGeneration) throw new InvalidOperationException("C4 fresh handback is unavailable.");
            var e=row.Execution;
            // 관리 예약 경쟁은 짧게 봉인하고 외부 제어·native 구간에는 lock을 유지하지 않는다.
            lock (e.Gate)
            {
                if (!TryFreshResult(result,adapter,router,ownerToken,out var same) || !ReferenceEquals(same,row) || row.FreshHead != null)
                    throw new InvalidOperationException("C4 fresh handback completed before reservation.");
                row.FreshHead=new ExecutionEvent(ExecutionEventKind.FreshReserving,row.Returned);
            }
            try
            {
                result.Validate();
                if (nextEpochToken == null || ReferenceEquals(nextEpochToken,committedEpochToken) || nextInteractionGeneration != checked(committedInteractionGeneration+1) ||
                    !e.AdapterEntered || !e.RouterEntered || !IsExecutionGuardedPair(adapter,router)) throw new InvalidOperationException("C4 next epoch is inconsistent.");
                e.Control.Checkpoint(ProfileResetExecutionCheckpointV1.BeforeFreshReserve);
                var reservation=new ProfileResetFreshExecutionReservationV1();
                var fresh=new FreshRecord(reservation,row,nextEpochToken,nextInteractionGeneration);
                lock (e.Gate)
                {
                    if (HasExecutionFault(e) || row.FreshHead.Kind != ExecutionEventKind.FreshReserving) throw new InvalidOperationException("C4 reservation operation ended.");
                    row.Reservation=fresh; row.FreshHead=fresh.Reserved;
                    ExecutionReservations.Add(reservation,fresh);
                }
                e.Control.Checkpoint(ProfileResetExecutionCheckpointV1.AfterFreshReserve);
                return reservation;
            }
            catch { lock (e.Gate) { row.FreshClosedHistory=row.FreshHead; row.FreshHead=row.FreshFailure; RecordExecutionFault(e); } CloseOriginalExecution(e); throw; }
        }
        private static bool TryFreshReservation(ProfileResetFreshExecutionReservationV1 reservation,DesktopProfileLaunchAdapterV1 adapter,InputRouter router,
            object owner,object token,long epoch,out FreshRecord fresh)
        {
            fresh=null;
            return reservation != null && ExecutionReservations.TryGetValue(reservation,out fresh) && fresh != null &&
                TryTrustedExecutionSource(fresh.Result.Execution.Confirmed,adapter,router,owner,out _,out _) &&
                ReferenceEquals(fresh.Reservation,reservation) && ReferenceEquals(fresh.Result.Reservation,fresh) &&
                ReferenceEquals(fresh.Result.Execution.Adapter,adapter) && ReferenceEquals(fresh.Result.Execution.Router,router) &&
                ReferenceEquals(fresh.Result.Execution.Owner,owner) && ReferenceEquals(fresh.Result.Execution.Thread,Thread.CurrentThread) &&
                ReferenceEquals(fresh.NextToken,token) && fresh.NextEpoch == epoch;
        }
        internal static bool ValidateFreshExecutionReservation(ProfileResetFreshExecutionReservationV1 reservation,DesktopProfileLaunchAdapterV1 adapter,InputRouter router,
            object ownerToken,object nextEpochToken,long nextInteractionGeneration)
        {
            if (!TryFreshReservation(reservation,adapter,router,ownerToken,nextEpochToken,nextInteractionGeneration,out var fresh) ||
                HasExecutionFault(fresh.Result.Execution) || !ReferenceEquals(fresh.Result.FreshHead,fresh.Reserved)) return false;
            try { fresh.Result.Execution.Result.Validate(); return true; }
            catch { RecordExecutionFault(fresh.Result.Execution); throw; }
        }
        internal static void CompleteFreshExecutionReservation(ProfileResetFreshExecutionReservationV1 reservation,DesktopProfileLaunchAdapterV1 adapter,InputRouter router,
            object ownerToken,object nextEpochToken,long nextInteractionGeneration)
        {
            if (!TryFreshReservation(reservation,adapter,router,ownerToken,nextEpochToken,nextInteractionGeneration,out var fresh) ||
                HasExecutionFault(fresh.Result.Execution) || !ReferenceEquals(fresh.Result.FreshHead,fresh.Reserved)) throw new InvalidOperationException("C4 fresh reservation is unavailable.");
            var e=fresh.Result.Execution;
            lock (e.Gate)
            {
                if (!ReferenceEquals(fresh.Result.FreshHead,fresh.Reserved) || HasExecutionFault(e) ||
                    !ReferenceEquals(fresh.CheckpointAttempt,fresh.CheckpointCompleted)) throw new InvalidOperationException("C4 fresh reservation already ended or is inside a checkpoint.");
            }
            try
            {
                if (!ValidateFreshExecutionReservation(reservation,adapter,router,ownerToken,nextEpochToken,nextInteractionGeneration) ||
                    fresh.CheckpointCompleted == null || !ReferenceEquals(fresh.CheckpointCompleted,fresh.CheckpointAttempt) ||
                    fresh.CheckpointCompleted.Checkpoint != ProfileResetExecutionCheckpointV1.AfterFreshConsume)
                    throw new InvalidOperationException("C4 fresh reservation lost its source or actual steps.");
                lock (e.Gate)
                {
                    // 실제 완료 사건을 먼저 남긴다. 양측 결속 실패는 이를 권한으로 공개하지 않는다.
                    fresh.Result.FreshHead=fresh.Completed;
                    e.GuardOperation=ProfileResetExecutionGuardOperationV1.CompleteFresh;
                }
                adapter.ApplyExecutionGuard(e.Guard,e.GuardOperation); e.AdapterFreshCompleted=true;
                router.ApplyExecutionGuard(e.Guard,e.GuardOperation); e.RouterFreshCompleted=true;
            }
            catch { FaultOriginalExecution(e); throw; }
        }
        internal static void CheckpointFreshExecution(ProfileResetExecutionResultV1 result,ProfileResetFreshExecutionReservationV1 reservation,
            DesktopProfileLaunchAdapterV1 adapter,InputRouter router,object ownerToken,ProfileResetExecutionCheckpointV1 checkpoint)
        {
            if (checkpoint < ProfileResetExecutionCheckpointV1.AfterFreshPrepare || checkpoint > ProfileResetExecutionCheckpointV1.AfterFreshConsume ||
                !TryFreshResult(result,adapter,router,ownerToken,out var row) || reservation == null ||
                !ExecutionReservations.TryGetValue(reservation,out var fresh) || !ReferenceEquals(fresh.Result,row) ||
                !ReferenceEquals(row.FreshHead,fresh.Reserved)) throw new InvalidOperationException("C4 fresh checkpoint is unavailable.");
            var e=row.Execution; FreshCheckpointEvent step;
            lock (e.Gate)
            {
                if (!ReferenceEquals(row.FreshHead,fresh.Reserved) || HasExecutionFault(e) ||
                    !ReferenceEquals(fresh.CheckpointAttempt,fresh.CheckpointCompleted)) throw new InvalidOperationException("C4 fresh checkpoint is already active or ended.");
                try
                {
                    result.Validate();
                    if (!ValidateFreshExecutionReservation(reservation,adapter,router,ownerToken,fresh.NextToken,fresh.NextEpoch))
                        throw new InvalidOperationException("C4 checkpoint reservation source is inconsistent.");
                    var expected=fresh.CheckpointCompleted == null ? ProfileResetExecutionCheckpointV1.AfterFreshPrepare :
                        fresh.CheckpointCompleted.Checkpoint+1;
                    if (checkpoint != expected) throw new InvalidOperationException("C4 fresh checkpoint order is inconsistent.");
                    step=new FreshCheckpointEvent(fresh.CheckpointCompleted,checkpoint); fresh.CheckpointAttempt=step;
                }
                catch { RecordExecutionFault(e); throw; }
            }
            try
            {
                e.Control.Checkpoint(checkpoint);
                lock (e.Gate)
                {
                    if (HasExecutionFault(e) || !ReferenceEquals(fresh.CheckpointAttempt,step)) throw new InvalidOperationException("C4 fresh checkpoint ended during its callback.");
                    fresh.CheckpointCompleted=step;
                }
            }
            catch { RecordExecutionFault(e); throw; }
        }
        internal static void CloseFreshExecution(ProfileResetExecutionResultV1 result,ProfileResetFreshExecutionReservationV1 reservation,
            DesktopProfileLaunchAdapterV1 adapter,InputRouter router,object ownerToken)
        {
            if (result == null || !ExecutionResults.TryGetValue(result,out var row) || row == null || row.Handback == null ||
                !ReferenceEquals(row.Execution.Result,result) || !ReferenceEquals(row.Execution.Adapter,adapter) ||
                !ReferenceEquals(row.Execution.Router,router) || !ReferenceEquals(row.Execution.Owner,ownerToken) ||
                !TryTrustedExecutionSource(row.Execution.Confirmed,adapter,router,ownerToken,out _,out _) ||
                !ReferenceEquals(row.Execution.Thread,Thread.CurrentThread)) throw new InvalidOperationException("C4 close is foreign.");
            if (reservation != null && (!ExecutionReservations.TryGetValue(reservation,out var fresh) || !ReferenceEquals(fresh.Result,row))) throw new InvalidOperationException("C4 close reservation is foreign.");
            if (row.Execution.NativeClosed) return;
            row.FreshClosedHistory=row.FreshHead; row.FreshHead=row.FreshFailure; FaultOriginalExecution(row.Execution);
        }
        internal static void ValidateExecutionGuardContext(ProfileResetExecutionGuardContextV1 context,DesktopProfileLaunchAdapterV1 adapter,InputRouter router,ProfileResetExecutionGuardOperationV1 operation)
        {
            if (context == null || !ExecutionGuards.TryGetValue(context,out var e) || !ReferenceEquals(e.Adapter,adapter) || !ReferenceEquals(e.Router,router) ||
                !TryTrustedExecutionSource(e.Confirmed,adapter,router,e.Owner,out _,out _) ||
                !ReferenceEquals(e.Thread,Thread.CurrentThread) || !ReferenceEquals(e.Guard,context) || e.GuardOperation != operation ||
                operation < ProfileResetExecutionGuardOperationV1.Enter || operation > ProfileResetExecutionGuardOperationV1.Close ||
                operation == ProfileResetExecutionGuardOperationV1.Enter && (!ReferenceEquals(e.ConsumedHistory,e.Consumed) || HasExecutionFault(e)) ||
                operation == ProfileResetExecutionGuardOperationV1.CompleteFresh && (HasExecutionFault(e) || e.ResultRecord == null || e.ResultRecord.Reservation == null || !ReferenceEquals(e.ResultRecord.FreshHead,e.ResultRecord.Reservation.Completed)) ||
                operation == ProfileResetExecutionGuardOperationV1.Close && !HasExecutionFault(e) && (e.ResultRecord == null || e.ResultRecord.Handback != null))
                throw new InvalidOperationException("C4 guard context is not an original operation.");
        }
        internal static bool IsExecutionGuardedPair(DesktopProfileLaunchAdapterV1 adapter,InputRouter router)
        {
            return !ReferenceEquals(adapter,null) && !ReferenceEquals(router,null) && GuardedAdapters.TryGetValue(adapter,out var e) &&
                GuardedRouters.TryGetValue(router,out var r) && ReferenceEquals(e,r) && ReferenceEquals(e.Thread,Thread.CurrentThread) &&
                TryTrustedExecutionSource(e.Confirmed,adapter,router,e.Owner,out _,out _) &&
                ReferenceEquals(e.ConsumedHistory,e.Consumed) && !HasExecutionFault(e) && !e.NativeClosed && e.AdapterEntered && e.RouterEntered && !IsFreshCompleted(e);
        }
        private static bool HasExecutionFault(ExecutionRecord e)
        { return e.FaultHistory != null || ReferenceEquals(e.Head,e.Failure); }
        private static void RecordExecutionFault(ExecutionRecord e)
        { if (!HasExecutionFault(e)) { e.FaultHistory=e.Head ?? e.Failure; e.Head=e.Failure; } }
        private static void FaultOriginalExecution(ExecutionRecord e) { RecordExecutionFault(e); CloseOriginalExecution(e); }
        private static void CloseOriginalExecution(ExecutionRecord e)
        {
            if (!ReferenceEquals(e.Thread,Thread.CurrentThread) || e.NativeClosed && (!HasExecutionFault(e) || e.NativeFaultClosed)) return;
            if (HasExecutionFault(e)) e.NativeFaultClosed=true;
            e.NativeClosed=true; e.GuardOperation=ProfileResetExecutionGuardOperationV1.Close;
            try { e.Adapter.ApplyExecutionGuard(e.Guard,ProfileResetExecutionGuardOperationV1.Close); }
            catch { e.Adapter.LatchResetCutoverFailure(); }
            finally
            {
                try { e.Router.ApplyExecutionGuard(e.Guard,ProfileResetExecutionGuardOperationV1.Close); }
                catch { e.Router.LatchResetCutoverFailure(); }
            }
        }
    }
}
