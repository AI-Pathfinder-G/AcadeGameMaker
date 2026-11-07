using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.CompilerServices;
using System.Threading;
using AcadeGameMaker.Core;
using AcadeGameMaker.Profile;
using UnityEngine;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-M5D7M-001..009. Desktop-only profile preparation and the narrow
    // M5D7L-to-M5B5 handoff; this component has no scene or presentation role.
    [DefaultExecutionOrder(-220)]
    [DisallowMultipleComponent]
    internal sealed class DesktopProfileLaunchAdapterV1 : MonoBehaviour
    {
        // REQ-AUTOCH1-RETRY-001/002: 호출 간 캐시 없이 같은 lease 안에서 원문 재검증과 저장을 수행한다.
        internal enum Chapter1ChoiceRetryDisposition { Saved=1, RejectedBeforeSave=2, AlreadyCommittedObserved=3 }
        internal readonly struct Chapter1ChoiceRetryAttempt
        {
            internal Chapter1ChoiceRetryAttempt(Chapter1ChoiceRetryDisposition disposition,ProfileAtomicSaveResultV1? save,string reason){Disposition=disposition;Save=save;Reason=reason;}
            internal Chapter1ChoiceRetryDisposition Disposition{get;}
            internal ProfileAtomicSaveResultV1? Save{get;}
            internal string Reason{get;}
        }
        internal static Chapter1ChoiceRetryAttempt RetryChapter1ChoiceSave(string root,ProfileCanonicalDocumentV1 before,ProfileCanonicalDocumentV1 candidate,int choice,int consent,int skill,ProfileAtomicSaveResultV1 failed)
        {
            before.Validate();candidate.Validate();failed.Validate();
            if(choice!=1 && choice!=2 || consent!=(choice==1?4:3) || skill!=choice || failed.Outcome!=ProfileAtomicSaveOutcome.FailedBeforeCommit)
                throw new ArgumentException("동일 선택의 실제 사전 실패만 재시도할 수 있습니다.");
            var source=before.Snapshot;var prior=source.Progression;
            if(prior.TryGetCommittedPair(out var committedPair) && ((int)committedPair.Choice!=choice || (int)committedPair.Skill!=skill || (int)prior.ConsentState!=consent))throw new ArgumentException("이미 확정된 선택 조합은 재시도로 바꾸지 않습니다.");
            var expected=checked(prior.ProfileRevision+1);
            var exact=ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(source.Settings,source.Input,source.Tutorial,new ProfileProgressionSnapshot(expected,prior.LastOfferedSeed,(ProfileChoice)choice,(ProfileConsentState)consent,(ProfileSkill)skill,prior.CompletedBranches)));exact.Validate();
            if(!ChoiceRetryBytesEqual(exact.CanonicalFileBytes,candidate.CanonicalFileBytes))throw new ArgumentException("보관된 선택 후보가 원래 요청과 다릅니다.");
            var saveStarted=false;
            try
            {
                root=Normalize(root);if(!string.Equals(root,Normalize(Chapter1ProfileRoot),StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("인증된 챕터 저장 root가 아닙니다.");
                root=ProfileResetDiskTransactionV1.ValidateMemoryResetRootBeforeLease(root);
                using(var lease=ProfileRootOperationLockV1.Acquire(root))
                {
                    var observed=ProfileLaunchObservationAdapterV1.ObserveUnderHeldRoot(lease,root);observed.Validate();
                    var selection=ProfileLoadSelectorV1.Select(observed.Primary,observed.Previous,observed.Temp);selection.Validate();
                    if(ChoiceRetryCandidateMatches(observed.Primary,candidate))return new Chapter1ChoiceRetryAttempt(Chapter1ChoiceRetryDisposition.AlreadyCommittedObserved,null,"이미 같은 후보가 관측되어 추가 저장과 완료를 거절했습니다.");
                    if(!ChoiceRetryCandidateMatches(observed.Primary,before) || selection.Source!=ProfileLaunchSource.Primary || selection.RequiresAtomicSave || selection.PrimaryPreservation!=ProfileFilePreservationReason.None || selection.PreviousPreservation!=ProfileFilePreservationReason.None || !ChoiceRetryBytesEqual(selection.InMemoryDocument.CanonicalFileBytes,before.CanonicalFileBytes))
                        throw new InvalidOperationException("선택 재시도의 원래 primary 또는 보존 조건이 다릅니다.");
                    if(observed.Temp.Kind==ProfileLoadCandidateKind.Missing)
                    {if(selection.TempPreservation!=ProfileFilePreservationReason.None || failed.TempState!=ProfileStoredFileState.Missing || (failed.FailureStage!=ProfileAtomicSaveStage.DirectoryPreparation && failed.FailureStage!=ProfileAtomicSaveStage.TempWriteFlushClose))throw new InvalidOperationException("원래 부재한 temp의 실패 경로가 아닙니다.");}
                    else
                    {
                        if(failed.TempState!=ProfileStoredFileState.ValidCurrentInput || !ChoiceRetryCandidateMatches(observed.Temp,candidate) || selection.TempPreservation!=ProfileFilePreservationReason.StaleTemp)throw new InvalidOperationException("현재 temp가 같은 요청 후보가 아닙니다.");
                        var preserved=ProfileLaunchPreservationExecutorV1.Execute(root,observed,selection,DateTimeOffset.UtcNow);preserved.Validate();
                        if(!preserved.MayPersist || preserved.TempState!=ProfileLaunchPreservationStateV1.Preserved || preserved.PrimaryState!=ProfileLaunchPreservationStateV1.NotRequired || preserved.PreviousState!=ProfileLaunchPreservationStateV1.NotRequired)throw new InvalidOperationException("선택 재시도 원문 보존이 완료되지 않았습니다.");
                    }
                    var fresh=ProfileLaunchObservationAdapterV1.ObserveUnderHeldRoot(lease,root);fresh.Validate();var freshPlan=ProfileLoadSelectorV1.Select(fresh.Primary,fresh.Previous,fresh.Temp);freshPlan.Validate();
                    if(!ChoiceRetryCandidateMatches(fresh.Primary,before) || !ChoiceRetryCandidatesEqual(fresh.Previous,observed.Previous) || fresh.Temp.Kind!=ProfileLoadCandidateKind.Missing || freshPlan.Source!=ProfileLaunchSource.Primary || freshPlan.RequiresAtomicSave || freshPlan.PrimaryPreservation!=ProfileFilePreservationReason.None || freshPlan.PreviousPreservation!=ProfileFilePreservationReason.None || freshPlan.TempPreservation!=ProfileFilePreservationReason.None || !ChoiceRetryBytesEqual(freshPlan.InMemoryDocument.CanonicalFileBytes,before.CanonicalFileBytes))
                        throw new InvalidOperationException("원문 보존 뒤 저장 후보가 바뀌었습니다.");
                    saveStarted=true;var saved=ProfileAtomicSaveServiceV1.SaveUnderHeldRoot(lease,root,candidate);saved.Validate();return new Chapter1ChoiceRetryAttempt(Chapter1ChoiceRetryDisposition.Saved,saved,"이번 실제 저장 결과");
                }
            }
            catch(Exception error) when(error is IOException || error is UnauthorizedAccessException || error is System.Security.SecurityException || error is InvalidOperationException)
            {if(saveStarted)throw new InvalidOperationException("이번 선택 저장의 결과가 확인되지 않았습니다.",error);return new Chapter1ChoiceRetryAttempt(Chapter1ChoiceRetryDisposition.RejectedBeforeSave,null,error.GetType().Name+":"+error.Message);}
        }
        private static bool ChoiceRetryCandidateMatches(ProfileLoadCandidateV1 value,ProfileCanonicalDocumentV1 expected)
        {value.Validate();return value.Kind==ProfileLoadCandidateKind.Decoded && value.DecodeResult.Classification==ProfileCanonicalDecodeClassification.ValidCurrentInput && ChoiceRetryBytesEqual(value.DecodeResult.Document.CanonicalFileBytes,expected.CanonicalFileBytes);}
        private static bool ChoiceRetryCandidatesEqual(ProfileLoadCandidateV1 first,ProfileLoadCandidateV1 second)
        {first.Validate();second.Validate();return first.Kind==second.Kind && (first.Kind!=ProfileLoadCandidateKind.Decoded || first.DecodeResult.Classification==second.DecodeResult.Classification && ChoiceRetryBytesEqual(first.DecodeResult.Document.CanonicalFileBytes,second.DecodeResult.Document.CanonicalFileBytes));}
        private static bool ChoiceRetryBytesEqual(byte[] first,byte[] second)
        {if(first==null||second==null||first.Length!=second.Length)return false;for(var i=0;i<first.Length;i++)if(first[i]!=second[i])return false;return true;}
        [SerializeField] private InputRouter _router;
        [SerializeField] private string _chapter1DevelopmentRoot;
        // REQ-AUTOCH1-011: the authored marker is not a caller supplied save path.
        internal static string Chapter1ProfileRoot
        {
            get
            {
#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
                if(_chapter1TestRoot!=null)return _chapter1TestRoot;
#endif
#if UNITY_EDITOR
                return @"H:\GPT-workspace\AcadeGameMaker\output\local-playable-slice-r1\profile\chapter1-development";
#else
                return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.dataPath),"SaveData","Chapter1"));
#endif
            }
        }
#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        private static string _chapter1TestRoot;
        internal static void ConfigureChapter1TestRun(string runId)
        {
            if(string.IsNullOrEmpty(runId) || runId.Length>80)throw new ArgumentException("시험 식별자가 유효하지 않습니다.");
            foreach(var c in runId)if(!((c>='A'&&c<='Z') || (c>='a'&&c<='z') || (c>='0'&&c<='9') || c=='-'))throw new ArgumentException("시험 식별자가 유효하지 않습니다.");
            var workspace=@"H:\GPT-workspace\AcadeGameMaker\output\chapter1-autonomous-20261021\profiles";
            _chapter1TestRoot=Path.GetFullPath(Path.Combine(workspace,runId));
        }
#endif
        internal bool Chapter1PreparedForUi => _currentReceipt.HasValue && _startAttempted &&
            !_resetCutoverTerminal && _router!=null && _router.Chapter1PreparedForUi(this);
        internal void ConfigureChapter1DevelopmentRootForAuthoring(InputRouter router,string exactRoot)
        {
            const string allowed=@"H:\GPT-workspace\AcadeGameMaker\output\local-playable-slice-r1\profile\chapter1-development";
            if(!string.Equals(exactRoot,allowed,StringComparison.OrdinalIgnoreCase) || !string.IsNullOrEmpty(_chapter1DevelopmentRoot))
                throw new InvalidOperationException("지정 개발 root만 저작할 수 있습니다.");
            ConfigureHubUiOnlyForAuthoring(router);_chapter1DevelopmentRoot=exactRoot;
        }
        private IDesktopProfileLaunchEnvironmentV1 ResolveLaunchEnvironment()
        {
            // CH1 거점이 존재하면 비어 있거나 바뀐 설정에서 일반 사용자 저장으로 fallback하지 않는다.
            if(GetComponent<Chapter1HubLifetimeOwner>()!=null || !string.IsNullOrEmpty(_chapter1DevelopmentRoot))
            {
                const string allowed=@"H:\GPT-workspace\AcadeGameMaker\output\local-playable-slice-r1\profile\chapter1-development";
                if(_environment!=null || !string.Equals(_chapter1DevelopmentRoot,allowed,StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("CH1 거점의 개발 root 결속이 유효하지 않습니다.");
                return new Chapter1DevelopmentLaunchEnvironment(Chapter1ProfileRoot);
            }
            return _environment??new DesktopProfileLaunchEnvironmentV1();
        }
        private sealed class Chapter1DevelopmentLaunchEnvironment : IDesktopProfileLaunchEnvironmentV1
        {
            private readonly string _root;
            internal Chapter1DevelopmentLaunchEnvironment(string root){_root=root;}
            public string GetPersistentDataPath()
            {
                var allowed=Chapter1ProfileRoot;
                if(!string.Equals(_root,allowed,StringComparison.OrdinalIgnoreCase) || !Path.IsPathFullyQualified(_root))throw new InvalidOperationException("개발 root가 변경되었습니다.");
                for(var current=_root;!string.IsNullOrEmpty(current);current=Path.GetDirectoryName(current))
                    if(Directory.Exists(current) && (File.GetAttributes(current)&FileAttributes.ReparsePoint)!=0)throw new IOException("개발 root 링크 경유가 거절되었습니다.");
                // The reset pre-lease probe requires existing parents. Create only this
                // authenticated product namespace, never a fallback or an existing profile reset.
                var parent=Path.GetDirectoryName(_root);
                Directory.CreateDirectory(parent);
                for(var current=parent;!string.IsNullOrEmpty(current);current=Path.GetDirectoryName(current))
                    if((File.GetAttributes(current)&FileAttributes.ReparsePoint)!=0)throw new IOException("저장 부모의 링크 경유가 거절되었습니다.");
                return _root;
            }
            public DateTimeOffset GetUtcNow()=>DateTimeOffset.UtcNow;
        }
        private IDesktopProfileLaunchEnvironmentV1 _environment;
        private IDesktopProfileLaunchPreparationV1 _preparation;
        private bool _awakeAttempted, _reservationAcquired, _adopted, _startAttempted;
        private ProfileLaunchNotificationV1? _notification;
        private DesktopProfileLaunchAdapterFaultStageV1? _testFaultStage;

        private DesktopProfileLaunchReceiptV1? _currentReceipt;
        // REQ-M5D7QC2-003/007. Awake's candidate is not authority.  Start
        // promotion produces two independent immutable witnesses for C2.
        private string _launchRootCandidate;
        private string _launchRootWitnessA;
        private string _launchRootWitnessB;
        private InputRouter _launchRouterWitness;
        private InputRouter _launchRouterWitnessB;
        private DesktopProfileLaunchReceiptV1? _launchReceiptWitness;
        private bool _resetCutoverTerminal;
        private ProfileResetExecutionGuardContextV1 _executionGuard;
        private bool _executionGuardBlocked, _executionGuardClosed;
        private sealed class ExecutionGuardHalf
        {
            internal readonly ProfileResetExecutionGuardContextV1 Context;
            internal readonly bool Blocked, Closed;
            internal readonly ExecutionGuardHalf Previous;
            internal ExecutionGuardHalf(ProfileResetExecutionGuardContextV1 context,bool blocked,bool closed,ExecutionGuardHalf previous)
            { Context=context; Blocked=blocked; Closed=closed; Previous=previous; }
        }
        private static readonly ConditionalWeakTable<DesktopProfileLaunchAdapterV1,ExecutionGuardHalf> ExecutionGuardHalves=new ConditionalWeakTable<DesktopProfileLaunchAdapterV1,ExecutionGuardHalf>();
        private bool ExecutionSessionBlocked()
        {
            if (!ExecutionGuardHalves.TryGetValue(this,out var half)) return _executionGuardBlocked || _executionGuardClosed;
            if (!ReferenceEquals(half.Context,_executionGuard) || half.Blocked != _executionGuardBlocked || half.Closed != _executionGuardClosed)
                throw new InvalidOperationException("C4 adapter guard projection is malformed.");
            return half.Blocked || half.Closed;
        }
        private ResetAdapterLifecycleV1 _resetLifecycle;
        private bool _resetReceiptTaken;
        private long _resetGeneration;
        private CurrentSessionProfileV1 _currentSession;
        private ProfileResetMemoryReceiptV1 _resetReceipt;
        private GameInputActions _untransferredCandidate;
        private bool _untransferredCandidateDisposeAttempted;
        private bool _resetCandidateClaimedByRouter;
        private IProfileResetMemoryCutoverControlV1 _resetCandidateControl;
        private RestartBootstrapStateV1 _restartBootstrap;
        private string _restartRoot;
        private IProfileResetMemoryCutoverControlV1 _restartControl;
        private RestartBootstrapResultV1 _restartResult;
        private int _restartReceiptTaken;
        private sealed class RestartRecoveryWitness { internal readonly DesktopProfileLaunchAdapterV1 Owner; internal readonly InputRouter Router; internal readonly string Root; internal ProfileResetDiskPreparedProofV1 Proof; internal RestartBootstrapResultV1 Result; internal string TransactionId,TargetHash; internal long TargetLength,Generation; internal int ResumeInvoked,Minted,ExecutionStarted, TerminalState, CandidateClaimed, CandidateDisposeAttempted, ReceiptTaken; internal GameInputActions Candidate; internal IProfileResetMemoryCutoverControlV1 Control; internal RestartRecoveryWitness(DesktopProfileLaunchAdapterV1 owner,InputRouter router,string root){Owner=owner;Router=router;Root=root;} }
        private static readonly ConditionalWeakTable<DesktopProfileLaunchAdapterV1,RestartRecoveryWitness> RestartRecoveriesByAdapter=new ConditionalWeakTable<DesktopProfileLaunchAdapterV1,RestartRecoveryWitness>();
        private static readonly ConditionalWeakTable<InputRouter,RestartRecoveryWitness> RestartRecoveriesByRouter=new ConditionalWeakTable<InputRouter,RestartRecoveryWitness>();
        // REQ-M5D7QC2-003/004/007.  The mutable serialized router and the
        // diagnostic launch fields are deliberately not reset authority.  A
        // successful Start mints this private, immutable cohort before
        // Confirm; both original objects index the same witness so reflection
        // cannot substitute a sibling adapter/router into a live reset row.
        private sealed class ResetLaunchCohortWitness
        {
            internal readonly DesktopProfileLaunchAdapterV1 Owner;
            internal readonly InputRouter Router;
            internal readonly string Root;
            internal readonly DesktopProfileLaunchReceiptV1 Receipt;
            internal readonly GameInputActions Actions;
            internal bool Published;
            internal ResetCohortTerminalPhaseV1 TerminalPhase;
            internal GameInputActions OriginalOldActions, OriginalNewActions;
            internal long Generation;
            internal int OldDisposeAttempted, NewDisposeAttempted, ResetReceiptTaken, CandidateClaimedByRouter;
            internal IProfileResetMemoryCutoverControlV1 ResetControl;
            internal ResetLaunchCohortWitness(DesktopProfileLaunchAdapterV1 owner, InputRouter router, string root, DesktopProfileLaunchReceiptV1 receipt, GameInputActions actions)
            { Owner=owner; Router=router; Root=root; Receipt=receipt; Actions=actions; OriginalOldActions=actions; TerminalPhase=ResetCohortTerminalPhaseV1.Launch; }
        }
        private static readonly ConditionalWeakTable<DesktopProfileLaunchAdapterV1, ResetLaunchCohortWitness> ResetLaunchCohortsByAdapter = new ConditionalWeakTable<DesktopProfileLaunchAdapterV1, ResetLaunchCohortWitness>();
        private static readonly ConditionalWeakTable<InputRouter, ResetLaunchCohortWitness> ResetLaunchCohortsByRouter = new ConditionalWeakTable<InputRouter, ResetLaunchCohortWitness>();
        private sealed class ResetCandidateWitness { internal readonly GameInputActions Candidate; internal readonly IProfileResetMemoryCutoverControlV1 Control; internal int Claimed, DisposeAttempts; internal ResetCandidateWitness(GameInputActions candidate, IProfileResetMemoryCutoverControlV1 control) { Candidate = candidate; Control = control; } }
        private static readonly ConditionalWeakTable<DesktopProfileLaunchAdapterV1, ResetCandidateWitness> ResetCandidateWitnesses = new ConditionalWeakTable<DesktopProfileLaunchAdapterV1, ResetCandidateWitness>();
        internal DesktopProfileLaunchReceiptV1? CurrentReceipt
        {
            get
            {
                if (_currentReceipt.HasValue) _currentReceipt.Value.Validate();
                if (TryGetOriginalCohort(out var cohort) && cohort.Published && (!_currentReceipt.HasValue || !_launchReceiptWitness.HasValue || !_currentReceipt.Value.Equals(cohort.Receipt) || !_launchReceiptWitness.Value.Equals(cohort.Receipt)))
                    throw new InvalidOperationException("Historical launch receipt no longer matches its immutable cohort.");
                return _currentReceipt;
            }
        }
        // Compatibility-only diagnostic read. C2 uses IsExactResetRouter and
        // never acts on this mutable serialized reference.
        internal InputRouter BoundRouter => _router;
        internal bool IsResetCutoverTerminal => _resetCutoverTerminal;
        // REQ-M5D7QC4-002/004/006: 전환 사건은 lower 원장만 발급한다.
        internal void ApplyExecutionGuard(ProfileResetExecutionGuardContextV1 context,ProfileResetExecutionGuardOperationV1 operation)
        {
            if (!ResetLaunchCohortsByAdapter.TryGetValue(this,out var cohort) || ReferenceEquals(cohort,null) ||
                !ReferenceEquals(cohort.Owner,this)) throw new InvalidOperationException("C4 adapter has no original cohort.");
            ProfileNewGameConfirmationV1.ValidateExecutionGuardContext(context,this,cohort.Router,operation);
            if (!TryGetOriginalCohort(out var original) || !ReferenceEquals(original,cohort)) throw new InvalidOperationException("C4 adapter original cohort is inconsistent.");
            ExecutionGuardHalves.TryGetValue(this,out var previous);
            if (operation == ProfileResetExecutionGuardOperationV1.Enter)
            {
                if (_executionGuardBlocked || _executionGuardClosed) throw new InvalidOperationException("C4 adapter guard already entered.");
                _executionGuard=context; _executionGuardBlocked=true; _notification=null;
            }
            else if (operation == ProfileResetExecutionGuardOperationV1.CompleteFresh)
            {
                if (!ReferenceEquals(_executionGuard,context) || !_executionGuardBlocked || _executionGuardClosed) throw new InvalidOperationException("C4 adapter guard cannot complete fresh.");
                _executionGuardBlocked=false;
            }
            else
            {
                _executionGuard=context; _executionGuardBlocked=true; _executionGuardClosed=true;
            }
            var half=new ExecutionGuardHalf(context,_executionGuardBlocked,_executionGuardClosed,previous);
            ExecutionGuardHalves.Remove(this); ExecutionGuardHalves.Add(this,half);
            if (operation == ProfileResetExecutionGuardOperationV1.Close &&
                (previous != null && previous.Closed || cohort.TerminalPhase != ResetCohortTerminalPhaseV1.Completed || _resetLifecycle != ResetAdapterLifecycleV1.Completed))
                LatchResetCutoverFailure();
        }
        private bool TryGetOriginalCohort(out ResetLaunchCohortWitness witness)
        {
            if (!ResetLaunchCohortsByAdapter.TryGetValue(this, out witness) || witness == null || !ReferenceEquals(witness.Owner, this) || witness.Router == null || witness.Actions == null || string.IsNullOrEmpty(witness.Root)) return false;
            return ResetLaunchCohortsByRouter.TryGetValue(witness.Router, out var byRouter) && ReferenceEquals(byRouter, witness);
        }
        private bool TryGetRestartRecovery(out RestartRecoveryWitness witness)
        { return RestartRecoveriesByAdapter.TryGetValue(this,out witness) && witness!=null && ReferenceEquals(witness.Owner,this) && witness.Router!=null && RestartRecoveriesByRouter.TryGetValue(witness.Router,out var byRouter) && ReferenceEquals(byRouter,witness); }
        internal static bool IsPrivateRepairRecoveryReservation(InputRouter router,object owner)
        {
            return !ReferenceEquals(router,null) && owner!=null && RestartRecoveriesByRouter.TryGetValue(router,out var witness) && witness!=null && ReferenceEquals(witness.Router,router) && ReferenceEquals(witness.Owner,owner) && string.IsNullOrEmpty(witness.Root) && witness.Result!=null && witness.Result.Outcome==RestartBootstrapOutcomeV1.ManualRepairRequired;
        }
        internal bool IsExactRestartRecovery(InputRouter router,string root,ProfileResetDiskResultV1 resumed)
        {
            try { resumed.Validate(); return router!=null && TryGetRestartRecovery(out var w) && Volatile.Read(ref w.Minted)==1 && ReferenceEquals(w.Router,router) && ReferenceEquals(_router,w.Router) && string.Equals(w.Root,root,StringComparison.OrdinalIgnoreCase) && ReferenceEquals(w.Proof,resumed.PreparedProof) && w.TransactionId==resumed.TransactionId && w.TargetHash==resumed.TargetSha256 && w.TargetLength==resumed.TargetByteLength; } catch { return false; }
        }
        internal bool TryBeginRestartRecoveryExecution(InputRouter router,string root,ProfileResetDiskResultV1 resumed)
        {
            return IsExactRestartRecovery(router,root,resumed) && TryGetRestartRecovery(out var witness) && Interlocked.CompareExchange(ref witness.ExecutionStarted,1,0)==0;
        }
        internal static bool TryBeginRestartRecoveryCandidateDispose(InputRouter router,out GameInputActions candidate,out IProfileResetMemoryCutoverControlV1 control)
        {
            candidate=null; control=null;
            if(ReferenceEquals(router,null) || !RestartRecoveriesByRouter.TryGetValue(router,out var witness) || witness==null || Volatile.Read(ref witness.Minted)!=1 || Volatile.Read(ref witness.TerminalState)<1 || Volatile.Read(ref witness.CandidateClaimed)==0 || witness.Candidate==null || Interlocked.CompareExchange(ref witness.CandidateDisposeAttempted,1,0)!=0) return false;
            candidate=witness.Candidate; control=witness.Control; return control!=null;
        }
        private bool HasCohortDiagnosticMismatch(ResetLaunchCohortWitness witness)
        {
            return witness == null || !witness.Published || !ReferenceEquals(_router, witness.Router) ||
                !ReferenceEquals(_launchRouterWitness, witness.Router) || !ReferenceEquals(_launchRouterWitnessB, witness.Router) ||
                !string.Equals(_launchRootWitnessA, witness.Root, StringComparison.OrdinalIgnoreCase) || !string.Equals(_launchRootWitnessB, witness.Root, StringComparison.OrdinalIgnoreCase) ||
                !_currentReceipt.HasValue || !_launchReceiptWitness.HasValue || !_currentReceipt.Value.Equals(witness.Receipt) || !_launchReceiptWitness.Value.Equals(witness.Receipt) ||
                !witness.Router.MatchesPreparedLaunchCohortActions(this, witness.Actions);
        }
        // REQ-M5D7QC2-003/004/005/006.  After transfer, prepared-launch
        // action state is deliberately no longer a valid diagnostic.  These
        // independent adapter mirrors nevertheless remain a live integrity
        // boundary for the immutable, published original cohort.
        private bool HasTerminalCohortMirrorMismatch(ResetLaunchCohortWitness witness)
        {
            return witness == null || !witness.Published || !ReferenceEquals(witness.Owner, this) ||
                !ReferenceEquals(_router, witness.Router) || !ReferenceEquals(_launchRouterWitness, witness.Router) || !ReferenceEquals(_launchRouterWitnessB, witness.Router) ||
                !string.Equals(_launchRootWitnessA, witness.Root, StringComparison.OrdinalIgnoreCase) || !string.Equals(_launchRootWitnessB, witness.Root, StringComparison.OrdinalIgnoreCase) ||
                !_currentReceipt.HasValue || !_launchReceiptWitness.HasValue || !_currentReceipt.Value.Equals(witness.Receipt) || !_launchReceiptWitness.Value.Equals(witness.Receipt);
        }
        // These are intentionally narrow internal CWT queries.  No caller may
        // provide a mutable prepared-owner field as proof of a reset cohort.
        internal static bool IsRouterCohortTerminal(InputRouter router)
        {
            return router != null && ResetLaunchCohortsByRouter.TryGetValue(router, out var witness) &&
                (witness == null || witness.TerminalPhase >= ResetCohortTerminalPhaseV1.Faulted);
        }
        internal static bool IsRouterCohortInputClosed(InputRouter router)
        {
            return router != null && ResetLaunchCohortsByRouter.TryGetValue(router, out var witness) &&
                (witness == null || witness.TerminalPhase >= ResetCohortTerminalPhaseV1.Entered);
        }
        internal static void RecordRouterTerminal(InputRouter router, GameInputActions oldActions, GameInputActions newActions, long generation, bool oldDisposeAttempted, bool newDisposeAttempted, bool faulted)
        {
            if (router == null || !ResetLaunchCohortsByRouter.TryGetValue(router, out var witness) || witness == null) return;
            // The originals were captured by Start/RegisterStagedCandidate,
            // never learned from router mutable fields during containment.
            if ((oldActions != null && !ReferenceEquals(witness.OriginalOldActions, oldActions)) ||
                (newActions != null && !ReferenceEquals(witness.OriginalNewActions, newActions)) ||
                (generation != 0 && witness.Generation != 0 && witness.Generation != generation)) faulted = true;
            // Local flags are diagnostics only; reflected values must never
            // manufacture a private close attempt or suppress the real one.
            if (faulted) witness.TerminalPhase = ResetCohortTerminalPhaseV1.Faulted;
        }
        internal static bool TryBeginOriginalDispose(InputRouter router, bool isNew)
        {
            if (ReferenceEquals(router, null) || !ResetLaunchCohortsByRouter.TryGetValue(router, out var witness) || witness == null ||
                (isNew ? (witness.TerminalPhase < ResetCohortTerminalPhaseV1.Faulted || Volatile.Read(ref witness.CandidateClaimedByRouter)==0) :
                (witness.TerminalPhase != ResetCohortTerminalPhaseV1.Transferred && witness.TerminalPhase != ResetCohortTerminalPhaseV1.Completed && witness.TerminalPhase != ResetCohortTerminalPhaseV1.Faulted))) return false;
            return isNew ? Interlocked.CompareExchange(ref witness.NewDisposeAttempted, 1, 0) == 0 : Interlocked.CompareExchange(ref witness.OldDisposeAttempted, 1, 0) == 0;
        }
        internal static void RegisterOriginalResetCandidate(DesktopProfileLaunchAdapterV1 owner, GameInputActions candidate, IProfileResetMemoryCutoverControlV1 control)
        {
            if (owner == null || candidate == null || control == null || !ResetLaunchCohortsByAdapter.TryGetValue(owner, out var witness) || witness == null || witness.OriginalNewActions != null)
                throw new InvalidOperationException("Reset candidate does not belong to the original launch cohort.");
            witness.OriginalNewActions = candidate; witness.ResetControl=control;
        }
        internal static void MarkOriginalCandidateClaimed(DesktopProfileLaunchAdapterV1 owner, InputRouter router, GameInputActions candidate)
        {
            if (owner == null || router == null || candidate == null || !ResetLaunchCohortsByAdapter.TryGetValue(owner, out var witness) || witness == null || !ReferenceEquals(witness.Router, router) || !ReferenceEquals(witness.OriginalNewActions, candidate) || Volatile.Read(ref witness.CandidateClaimedByRouter)!=0)
                throw new InvalidOperationException("Reset candidate claim does not match the original cohort.");
            if (Interlocked.CompareExchange(ref witness.CandidateClaimedByRouter, 1, 0) != 0) throw new InvalidOperationException("Reset candidate claim was repeated.");
        }
        internal static void SetOriginalResetGeneration(InputRouter router, long generation)
        {
            if (router == null || generation <= 0 || !ResetLaunchCohortsByRouter.TryGetValue(router, out var witness) || witness == null || witness.Generation != 0)
                throw new InvalidOperationException("Reset generation cannot be minted for this cohort.");
            witness.Generation = generation;
        }
        internal static bool TryGetRouterTerminalOwnership(InputRouter router, out GameInputActions oldActions, out GameInputActions newActions, out bool oldDisposeAttempted, out bool newDisposeAttempted, out bool newClaimedByRouter, out IProfileResetMemoryCutoverControlV1 control)
        {
            oldActions=null; newActions=null; oldDisposeAttempted=false; newDisposeAttempted=false; newClaimedByRouter=false; control=null;
            if (router == null || !ResetLaunchCohortsByRouter.TryGetValue(router, out var witness) || witness == null) return false;
            oldActions=witness.OriginalOldActions; newActions=witness.OriginalNewActions; oldDisposeAttempted=Volatile.Read(ref witness.OldDisposeAttempted)!=0; newDisposeAttempted=Volatile.Read(ref witness.NewDisposeAttempted)!=0; newClaimedByRouter=Volatile.Read(ref witness.CandidateClaimedByRouter)!=0; control=witness.ResetControl; return true;
        }
        internal static void MarkRouterTransferCompleted(InputRouter router, long generation)
        {
            if (router != null && ResetLaunchCohortsByRouter.TryGetValue(router, out var witness) && witness != null && witness.TerminalPhase < ResetCohortTerminalPhaseV1.Faulted && witness.Generation == generation)
                witness.TerminalPhase = ResetCohortTerminalPhaseV1.Completed;
        }
        internal static void MarkRouterTransferred(InputRouter router, long generation)
        {
            if (router != null && ResetLaunchCohortsByRouter.TryGetValue(router, out var witness) && witness != null && witness.TerminalPhase == ResetCohortTerminalPhaseV1.Entered && witness.Generation == generation)
                witness.TerminalPhase = ResetCohortTerminalPhaseV1.Transferred;
        }
        internal static bool ValidateRouterCurrentResetPair(DesktopProfileLaunchAdapterV1 owner, InputRouter router, GameInputActions actions, long generation, string root)
        {
            if (owner == null || router == null || actions == null || string.IsNullOrEmpty(root) ||
                !ResetLaunchCohortsByAdapter.TryGetValue(owner, out var witness) || witness == null ||
                !ResetLaunchCohortsByRouter.TryGetValue(router, out var byRouter) || !ReferenceEquals(witness, byRouter) ||
                !ReferenceEquals(witness.Owner, owner) || !ReferenceEquals(witness.Router, router) || !witness.Published ||
                witness.TerminalPhase != ResetCohortTerminalPhaseV1.Completed || witness.Generation != generation ||
                !string.Equals(witness.Root, root, StringComparison.OrdinalIgnoreCase) || !ReferenceEquals(witness.OriginalNewActions, actions)) return false;
            return router.ValidateResetCutoverPair(owner, actions, generation);
        }
        internal bool HasExactResetLaunchCohort()
        {
            try { if (!TryGetOriginalCohort(out var witness) || witness.TerminalPhase != ResetCohortTerminalPhaseV1.Launch || !witness.Published || _resetCutoverTerminal || _resetLifecycle != ResetAdapterLifecycleV1.None || !_currentReceipt.HasValue || !_launchReceiptWitness.HasValue || !ReferenceEquals(_router, witness.Router) || !ReferenceEquals(_launchRouterWitness, witness.Router) || !ReferenceEquals(_launchRouterWitnessB, witness.Router) || witness.Router.IsFaulted || !witness.Router.isActiveAndEnabled || !isActiveAndEnabled) return false; _currentReceipt.Value.Validate(); _launchReceiptWitness.Value.Validate(); witness.Receipt.Validate(); return string.Equals(witness.Root, _launchRootWitnessA, StringComparison.OrdinalIgnoreCase) && string.Equals(witness.Root, _launchRootWitnessB, StringComparison.OrdinalIgnoreCase); }
            catch { return false; }
        }
        internal bool IsExactResetRouter(InputRouter router)
        {
            try { if (TryGetOriginalCohort(out var witness) && HasCohortDiagnosticMismatch(witness)) { LatchResetCutoverFailure(); return false; } return router != null && HasExactResetLaunchCohort() && ReferenceEquals(router,_router) && ReferenceEquals(router,_launchRouterWitness) && ReferenceEquals(router,_launchRouterWitnessB); }
            catch { return false; }
        }
        // REQ-M5D7QC3-001/002: C3 may observe only the immutable original
        // launch root. This boundary is deliberately read-only: no environment
        // lookup, lease, profile observation, receipt, or lifecycle mutation.
        internal string GetAuthenticatedLaunchRootForNewGame(InputRouter router)
        {
            if (router == null || !TryGetOriginalCohort(out var witness) || HasCohortDiagnosticMismatch(witness) || !HasExactResetLaunchCohort() ||
                !ReferenceEquals(router, witness.Router) || !ReferenceEquals(router, _router) ||
                !ReferenceEquals(router, _launchRouterWitness) || !ReferenceEquals(router, _launchRouterWitnessB))
                throw new InvalidOperationException("New Game launch cohort is not exact.");
            var root = Normalize(witness.Root);
            if (!string.Equals(root, witness.Root, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(root, _launchRootWitnessA, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(root, _launchRootWitnessB, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("New Game launch root witnesses are not exact.");
            return root;
        }
        internal bool IsEligibleResetRoot(string root)
        {
            if (TryGetOriginalCohort(out var witness) && HasCohortDiagnosticMismatch(witness)) { LatchResetCutoverFailure(); return false; }
            if (root == null || !HasExactResetLaunchCohort()) return false;
            try { return string.Equals(Normalize(root), _launchRootWitnessA, StringComparison.OrdinalIgnoreCase); }
            catch { return false; }
        }
        // REQ-M5D7Q0-009. Builder-only typed assignment seam.  It cannot
        // reserve, prepare, allocate, or read profile state.
        internal void ConfigureHubUiOnlyForAuthoring(InputRouter router)
        {
            if (router == null || gameObject.activeInHierarchy || _awakeAttempted || _reservationAcquired || _adopted || _startAttempted ||
                _currentReceipt.HasValue || _pending.HasValue || _notification.HasValue || _router != null)
                throw new InvalidOperationException("Hub launch adapter authoring seam requires a pristine inactive component.");
            _router = router;
        }
        internal void SetFaultStageForTests(DesktopProfileLaunchAdapterFaultStageV1? stage) { _testFaultStage = stage; }

        internal void ConfigureForTests(InputRouter router, IDesktopProfileLaunchEnvironmentV1 environment, IDesktopProfileLaunchPreparationV1 preparation)
        {
            if (_awakeAttempted || router == null || environment == null || preparation == null) throw new InvalidOperationException("Desktop launch adapter cannot be configured.");
            _router = router; _environment = environment; _preparation = preparation;
        }

        internal bool TryTakeNotification(out ProfileLaunchNotificationV1 notification)
        {
            if (ExecutionSessionBlocked()) { notification=default(ProfileLaunchNotificationV1); return false; }
            if (_resetCutoverTerminal || (TryGetOriginalCohort(out var cohort) && cohort.TerminalPhase >= ResetCohortTerminalPhaseV1.Entered)) { notification = default(ProfileLaunchNotificationV1); return false; }
            if (!_notification.HasValue) { notification = default(ProfileLaunchNotificationV1); return false; }
            notification = _notification.Value; notification.Validate(); _notification = null; return true;
        }

        private void Awake()
        {
            if (_awakeAttempted) throw new InvalidOperationException("Desktop launch adapter cannot retry.");
            _awakeAttempted = true; PreparedProfileLaunchV1 prepared = null; GameInputActions candidate = null;
            try
            {
                ValidateCohort();
                // Startup compatibility requires the exact ordinary owner to
                // reserve before its single environment-root read.  It is not
                // an OriginalLaunch cohort and carries no actions or history.
                _router.ReservePreparedHubLaunch(this); _reservationAcquired=true;
                // C2R selection is read-only and precedes UTC/preparation.
                var selectionEnvironment = ResolveLaunchEnvironment();
                var recoveryEligible=_router.IsAuthoredHubUiOnlyForRecovery;
                // Getter exceptions are historical launch failures.  Capture
                // its sole returned value before the classification-only catch.
                var returnedRoot=selectionEnvironment.GetPersistentDataPath();
                string selectionRoot; bool committed, temporary;
                if(!recoveryEligible)
                {
                    selectionRoot=Normalize(returnedRoot); committed=false; temporary=false;
                }
                else try
                {
                    selectionRoot = ProfileResetDiskTransactionV1.ValidateMemoryResetRootBeforeLease(returnedRoot);
                    committed = ProbeRegularBarrier(selectionRoot, ProfileResetBarrierV1.BarrierName);
                    temporary = ProbeRegularBarrier(selectionRoot, ProfileResetBarrierV1.BarrierTempName);
                }
                catch (IOException)
                {
                    _restartBootstrap=RestartBootstrapStateV1.RepairBlocked; _restartResult=RestartBootstrapResultV1.Create(RestartBootstrapOutcomeV1.ManualRepairRequired); RegisterRepairBlockedWitness();
                    _router.PromotePreparedReservationToRecovery(this,null,true); return;
                }
                catch (UnauthorizedAccessException)
                {
                    _restartBootstrap=RestartBootstrapStateV1.RepairBlocked; _restartResult=RestartBootstrapResultV1.Create(RestartBootstrapOutcomeV1.ManualRepairRequired); RegisterRepairBlockedWitness();
                    _router.PromotePreparedReservationToRecovery(this,null,true); return;
                }
                catch (ArgumentException)
                {
                    _restartBootstrap=RestartBootstrapStateV1.RepairBlocked; _restartResult=RestartBootstrapResultV1.Create(RestartBootstrapOutcomeV1.ManualRepairRequired); RegisterRepairBlockedWitness();
                    _router.PromotePreparedReservationToRecovery(this,null,true); return;
                }
                if (recoveryEligible && (committed || temporary))
                {
                    _restartRoot=selectionRoot; _restartBootstrap=committed && !temporary ? RestartBootstrapStateV1.Reserved : RestartBootstrapStateV1.RepairBlocked;
                    if(_restartBootstrap==RestartBootstrapStateV1.RepairBlocked)_restartResult=RestartBootstrapResultV1.Create(RestartBootstrapOutcomeV1.ManualRepairRequired);
                    var recovery=new RestartRecoveryWitness(this,_router,selectionRoot){Result=_restartResult}; RestartRecoveriesByAdapter.Add(this,recovery); try { RestartRecoveriesByRouter.Add(_router,recovery); } catch { RestartRecoveriesByAdapter.Remove(this); throw; }
                    _router.PromotePreparedReservationToRecovery(this,selectionRoot,_restartBootstrap==RestartBootstrapStateV1.RepairBlocked);
                    return;
                }
                if(recoveryEligible)_restartResult=RestartBootstrapResultV1.Create(RestartBootstrapOutcomeV1.OrdinaryLaunchRequired);
                var environment = ResolveLaunchEnvironment();
                var preparation = _preparation ?? new DesktopProfileLaunchPreparationPortV1();
                var root = selectionRoot; _launchRootCandidate = root;
                var utc = environment.GetUtcNow(); if (utc.Offset != TimeSpan.Zero) throw new InvalidOperationException("Desktop launch UTC must have zero offset.");
                prepared = preparation.Prepare(root, utc); if (prepared == null) throw new InvalidOperationException("Preparation returned no token.");
                prepared.Validate(); if (!prepared.HubEntryAllowed) throw new InvalidOperationException("Preparation denied hub entry.");
                var pending = DesktopProfileLaunchReceiptV1.FromPrepared(prepared);
                candidate = prepared.TakeActions();
                _router.AdoptPreparedHubActions(this, candidate); _adopted = true; candidate = null;
                prepared.Dispose(); prepared = null; // transferred token owns no actions after Take.
                _pending = pending;
            }
            catch (Exception original)
            {
                if (prepared != null) { try { prepared.Dispose(); } catch { } }
                if (candidate != null) { try { candidate.Dispose(); } catch { } }
                if (_reservationAcquired && !CurrentReceipt.HasValue)
                {
                    try { _router.FailPreparedHubLaunch(this); } catch { }
                }
                ExceptionDispatchInfo.Capture(original).Throw(); throw;
            }
        }

        private DesktopProfileLaunchReceiptV1? _pending;
        private void Start()
        {
            if (_startAttempted || CurrentReceipt.HasValue) return;
            _startAttempted = true;
            var hasRestartWitness=TryGetRestartRecovery(out var restartWitness);
            if (hasRestartWitness || _restartBootstrap != RestartBootstrapStateV1.None)
            {
                if (!hasRestartWitness || _restartBootstrap != RestartBootstrapStateV1.Reserved) { if(hasRestartWitness && restartWitness.Result==null)restartWitness.Result=_restartResult ?? RestartBootstrapResultV1.Create(RestartBootstrapOutcomeV1.ManualRepairRequired); _restartBootstrap=RestartBootstrapStateV1.Closed; _router.CloseRecoveryReservation(this); return; }
                try
                {
                    if(!TryGetRestartRecovery(out var recoveryBeforeResume) || !ReferenceEquals(recoveryBeforeResume.Router,_router) || Interlocked.CompareExchange(ref recoveryBeforeResume.ResumeInvoked,1,0)!=0) throw new InvalidOperationException("Restart recovery Resume was already invoked.");
                    var resumed=ProfileResetDiskTransactionV1.Resume(_restartRoot);
                    resumed.Validate();
                    if (resumed.Outcome != ProfileResetDiskOutcomeV1.DiskPrepared) { _restartResult=RestartBootstrapResultV1.Create(TranslateRestartDiskOutcome(resumed.Outcome)); recoveryBeforeResume.Result=_restartResult; _restartBootstrap=RestartBootstrapStateV1.Closed; _router.CloseRecoveryReservation(this); return; }
                    var targetLength=resumed.TargetByteLength;
                    if(!targetLength.HasValue || targetLength.Value<=0)throw new InvalidOperationException("Restart recovery prepared result has no target length.");
                    recoveryBeforeResume.Proof=resumed.PreparedProof; recoveryBeforeResume.TransactionId=resumed.TransactionId; recoveryBeforeResume.TargetHash=resumed.TargetSha256; recoveryBeforeResume.TargetLength=targetLength.Value;
                    if(Interlocked.CompareExchange(ref recoveryBeforeResume.Minted,1,0)!=0) throw new InvalidOperationException("Restart recovery mint repeated.");
                    var memoryResult=ProfileResetMemoryCutoverV1.FinalizeRestartRecovery(_restartRoot,resumed,this,_router,_restartControl ?? ProfileResetMemoryCutoverNoOpControlV1.Instance);
                    _restartResult=memoryResult.IsCompletedTrusted ? memoryResult.StagedRestartBootstrap : RestartBootstrapResultV1.Create(TranslateRestartMemoryOutcome(memoryResult.RawOutcome)); recoveryBeforeResume.Result=_restartResult;
                    _restartBootstrap=memoryResult.IsCompletedTrusted ? RestartBootstrapStateV1.Completed : RestartBootstrapStateV1.Closed;
                    return;
                }
                catch { _restartResult=RestartBootstrapResultV1.Create(RestartBootstrapOutcomeV1.ReloadRequired); if(TryGetRestartRecovery(out var failedRecovery))failedRecovery.Result=_restartResult; _restartBootstrap=RestartBootstrapStateV1.Closed; _router.CloseRecoveryReservation(this); return; }
            }
            try
            {
                if (!_adopted || !_pending.HasValue) throw new InvalidOperationException("Desktop launch has no adopted preparation.");
                var proof = _router.InitializePreparedHub(this); proof.Validate();
                CheckInjectedFault(DesktopProfileLaunchAdapterFaultStageV1.AfterInitializeProof);
                if (!proof.IsOwnedBy(this) || proof.Mode != InputMode.UIOnly || proof.GameplayEnabled || !proof.UiEnabled || !proof.CallbacksRegistered || !proof.Initialized || !proof.PendingPublication)
                    throw new InvalidOperationException("Prepared hub initialization proof is malformed.");
                var receipt = _pending.Value.WithHubProof(); receipt.Validate();
                CheckInjectedFault(DesktopProfileLaunchAdapterFaultStageV1.AfterReceiptStaging);
                ProfileLaunchNotificationV1? notification = ProfileLaunchNotificationV1.FromReceipt(receipt);
                if (notification.HasValue) notification.Value.Validate();
                CheckInjectedFault(DesktopProfileLaunchAdapterFaultStageV1.AfterNotificationStaging);
                // All allocations are staged before the router's one-shot
                // publication confirmation.  Mint/register the immutable
                // original launch cohort only after the real Initialize proof
                // and receipt validation; a registration failure is still a
                // pre-Confirm legacy failure and is contained by the catch.
                ResetLaunchCohortWitness cohort=null;
                if(_router.IsAuthoredHubUiOnlyForRecovery)
                {
                    var rootWitnessA = _launchRootCandidate; var rootWitnessB = string.Copy(_launchRootCandidate); var routerWitness = _router;
                    var actionsWitness = _router.GetPreparedActionsForLaunchCohort(this);
                    cohort = new ResetLaunchCohortWitness(this, routerWitness, rootWitnessA, receipt, actionsWitness);
                    ResetLaunchCohortsByAdapter.Add(this, cohort);
                    try { ResetLaunchCohortsByRouter.Add(routerWitness, cohort); }
                    catch { ResetLaunchCohortsByAdapter.Remove(this); throw; }
                    _launchRootWitnessA = rootWitnessA; _launchRootWitnessB = rootWitnessB; _launchRouterWitness = routerWitness; _launchRouterWitnessB = routerWitness; _launchReceiptWitness = receipt;
                }
                CheckInjectedFault(DesktopProfileLaunchAdapterFaultStageV1.BeforeConfirm);
                _router.ConfirmPreparedHubPublication(this);
                // No fallible operation may follow Confirm.  CWT value writes
                // and existing historical assignments are non-throwing.
                if(cohort!=null)cohort.Published = true;
                _notification = notification;
                _currentReceipt = receipt; // REQ-M5D7M-005: publication is deliberately last.
            }
            catch
            {
                if (_reservationAcquired && !CurrentReceipt.HasValue) { try { _router.FailPreparedHubLaunch(this); } catch { } }
                throw;
            }
        }

        private void OnDisable() { ProfileNewGameConfirmationV1.RecordExecutionLifecycleFault(this); if (_resetCutoverTerminal || (TryGetOriginalCohort(out var witness) && witness.TerminalPhase >= ResetCohortTerminalPhaseV1.Entered)) LatchResetCutoverFailure(); else FailBeforeReceipt(); }
        private void OnDestroy() { ProfileNewGameConfirmationV1.RecordExecutionLifecycleFault(this); if (_resetCutoverTerminal) { _resetLifecycle=ResetAdapterLifecycleV1.Disposed; LatchResetCutoverFailure(); } else FailBeforeReceipt(); }
        private void FailBeforeReceipt()
        {
            ProfileNewGameConfirmationV1.RecordExecutionLifecycleFault(this);
            if (_restartBootstrap==RestartBootstrapStateV1.Reserved || _restartBootstrap==RestartBootstrapStateV1.RepairBlocked) { _restartBootstrap=RestartBootstrapStateV1.Closed; _router.CloseRecoveryReservation(this); return; }
            if (_reservationAcquired && !CurrentReceipt.HasValue) { try { _router.FailPreparedHubLaunch(this); } catch { } }
        }
        private void ValidateCohort()
        {
            // During a same-cohort Awake pass a sibling's isActiveAndEnabled is
            // not a stable ordering proof. Reservation remains the authoritative
            // pre-Start/allocation gate; these are the literal cohort facts.
            if (_router == null || !_router.enabled || !_router.gameObject.activeInHierarchy || !enabled || !gameObject.activeInHierarchy || _router.AuthoredInitialMode != InputMode.UIOnly)
                throw new InvalidOperationException("Desktop launch adapter/router cohort is not exact.");
        }
        private static string Normalize(string root)
        {
            if (string.IsNullOrWhiteSpace(root) || !Path.IsPathFullyQualified(root)) throw new ArgumentException("Desktop profile root is malformed.", nameof(root));
            var full = Path.GetFullPath(root); var pathRoot = Path.GetPathRoot(full);
            if (string.Equals(full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), pathRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Desktop profile root cannot be a filesystem root.", nameof(root));
            full = full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (string.IsNullOrEmpty(full)) throw new ArgumentException("Desktop profile root cannot be a filesystem root.", nameof(root));
            return full;
        }
        private static bool ProbeRegularBarrier(string root, string name)
        {
            var path=Path.Combine(root,name);
            try
            {
                var attributes=File.GetAttributes(path);
                if ((attributes&FileAttributes.Directory)!=0 || (attributes&FileAttributes.ReparsePoint)!=0) throw new IOException("Reset barrier is not a regular file.");
                using(var stream=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.ReadWrite)) { if(stream.Length<0) throw new IOException("Reset barrier is unreadable."); }
                return true;
            }
            catch(FileNotFoundException) { return false; }
            catch(DirectoryNotFoundException) { return false; }
        }
        private void RegisterRepairBlockedWitness()
        {
            if(TryGetRestartRecovery(out var existing)) { existing.Result=_restartResult; return; }
            var witness=new RestartRecoveryWitness(this,_router,string.Empty){Result=_restartResult}; RestartRecoveriesByAdapter.Add(this,witness); try { RestartRecoveriesByRouter.Add(_router,witness); } catch { RestartRecoveriesByAdapter.Remove(this); throw; }
        }
        internal void ConfigureRecoveryControlForTests(IProfileResetMemoryCutoverControlV1 control)
        {
            if(control==null || _awakeAttempted || _restartControl!=null) throw new InvalidOperationException("Recovery control requires a pristine adapter.");
            _restartControl=control;
        }
        internal RestartBootstrapResultV1 RestartBootstrapResult
        {
            get
            {
                if(_restartResult==null) throw new InvalidOperationException("Restart bootstrap has no closed result.");
                _restartResult.Validate();
                if(_restartResult.Outcome==RestartBootstrapOutcomeV1.OrdinaryLaunchRequired)
                {
                    if(TryGetRestartRecovery(out var unexpected)) throw new InvalidOperationException("Ordinary bootstrap acquired recovery authority.");
                }
                else if(!TryGetRestartRecovery(out var recovery) || !ReferenceEquals(recovery.Result,_restartResult))
                    throw new InvalidOperationException("Restart bootstrap result lost its private witness.");
                return _restartResult;
            }
        }
        private static RestartBootstrapOutcomeV1 TranslateRestartDiskOutcome(ProfileResetDiskOutcomeV1 outcome)
        {
            if(outcome==ProfileResetDiskOutcomeV1.Busy)return RestartBootstrapOutcomeV1.BusyBlocked;
            if(outcome==ProfileResetDiskOutcomeV1.ManualRepairRequired)return RestartBootstrapOutcomeV1.ManualRepairRequired;
            return RestartBootstrapOutcomeV1.ReloadRequired;
        }
        private static RestartBootstrapOutcomeV1 TranslateRestartMemoryOutcome(ProfileResetMemoryOutcomeV1 outcome)
        {
            if(outcome==ProfileResetMemoryOutcomeV1.Completed)return RestartBootstrapOutcomeV1.Completed;
            if(outcome==ProfileResetMemoryOutcomeV1.Busy)return RestartBootstrapOutcomeV1.BusyBlocked;
            if(outcome==ProfileResetMemoryOutcomeV1.ManualRepairRequired)return RestartBootstrapOutcomeV1.ManualRepairRequired;
            return RestartBootstrapOutcomeV1.ReloadRequired;
        }
        private void CheckInjectedFault(DesktopProfileLaunchAdapterFaultStageV1 stage)
        { if (_testFaultStage.HasValue && _testFaultStage.Value == stage) throw new InvalidOperationException("Injected desktop launch adapter fault."); }

        // REQ-M5D7QC2-003/004. The coordinator is the only caller and supplies
        // the Profile authority; this method validates the original launch
        // cohort before any root lock/proof operation is attempted.
        // Coordinator transfers candidate forensic ownership before terminal
        // entry, so a gate exception cannot make its finally path double-close.
        internal void AdoptResetCandidateForTerminal(GameInputActions staged)
        {
            AdoptResetCandidateForTerminal(staged, ProfileResetMemoryCutoverNoOpControlV1.Instance);
        }
        internal void AdoptResetCandidateForTerminal(GameInputActions staged, IProfileResetMemoryCutoverControlV1 control)
        {
            ResetCandidateWitness witness; if (staged == null || control == null || !ResetCandidateWitnesses.TryGetValue(this, out witness) || witness == null || !ReferenceEquals(witness.Candidate, staged) || !ReferenceEquals(witness.Control, control) || _untransferredCandidateDisposeAttempted || _resetLifecycle != ResetAdapterLifecycleV1.None) throw new InvalidOperationException("Reset candidate adoption is not authorized.");
            _untransferredCandidate = staged; _resetCandidateControl = control;
        }
        // C2 coordinator records exact ownership immediately after allocation,
        // before any map validation, binding or checkpoint can throw.
        internal void RegisterStagedCandidate(GameInputActions staged, IProfileResetMemoryCutoverControlV1 control)
        {
            if (staged == null || control == null || _untransferredCandidate != null || _untransferredCandidateDisposeAttempted || _resetLifecycle != ResetAdapterLifecycleV1.None) throw new InvalidOperationException("Reset candidate registration is not authorized.");
            RegisterOriginalResetCandidate(this, staged, control);
            ResetCandidateWitnesses.Add(this, new ResetCandidateWitness(staged, control)); _untransferredCandidate = staged; _resetCandidateControl = control;
        }
        internal void RegisterRestartRecoveryCandidate(GameInputActions staged, IProfileResetMemoryCutoverControlV1 control)
        {
            if (_restartBootstrap!=RestartBootstrapStateV1.Reserved || staged==null || control==null || _untransferredCandidate!=null || _resetLifecycle!=ResetAdapterLifecycleV1.None || !TryGetRestartRecovery(out var recovery) || Volatile.Read(ref recovery.Minted)!=1 || Volatile.Read(ref recovery.TerminalState)!=0 || recovery.Proof==null || string.IsNullOrEmpty(recovery.TransactionId) || recovery.TargetLength<=0 || string.IsNullOrEmpty(recovery.TargetHash) || recovery.Candidate!=null) throw new InvalidOperationException("Restart candidate registration is not authorized.");
            recovery.Candidate=staged; recovery.Control=control;
            ResetCandidateWitnesses.Add(this,new ResetCandidateWitness(staged,control)); _untransferredCandidate=staged; _resetCandidateControl=control;
        }
        internal ResetCutoverCapabilityV1 EnterRestartRecoveryTerminal(string root, ValidatedResetMemoryAuthorityV1 authority, IProfileResetMemoryCutoverControlV1 control)
        {
            ResetCandidateWitness witness;
            if (_restartBootstrap!=RestartBootstrapStateV1.Reserved || authority==null || control==null || !TryGetRestartRecovery(out var recovery) || Volatile.Read(ref recovery.Minted)!=1 || !ReferenceEquals(recovery.Router,_router) || !string.Equals(root,recovery.Root,StringComparison.OrdinalIgnoreCase) || !ResetCandidateWitnesses.TryGetValue(this,out witness) || witness==null || !ReferenceEquals(witness.Candidate,_untransferredCandidate) || !ReferenceEquals(witness.Control,control)) throw new InvalidOperationException("Restart recovery cohort is not exact.");
            _resetCutoverTerminal=true; _resetLifecycle=ResetAdapterLifecycleV1.Entered; _notification=null;
            try { if(Interlocked.CompareExchange(ref recovery.TerminalState,1,0)!=0 || Interlocked.CompareExchange(ref recovery.CandidateClaimed,1,0)!=0) throw new InvalidOperationException("Restart candidate claim repeated."); _resetCandidateClaimedByRouter=true; witness.Claimed=1; var cap=_router.EnterRestartRecoveryTerminal(this,witness.Candidate,authority,control); recovery.Generation=cap.Generation; return cap; }
            catch { LatchResetCutoverFailure(); throw; }
        }
        internal ResetCutoverCapabilityV1 EnterResetCutoverTerminal(string root, ValidatedResetMemoryAuthorityV1 authority)
        {
            return EnterResetCutoverTerminal(root, authority, ProfileResetMemoryCutoverNoOpControlV1.Instance);
        }
        internal ResetCutoverCapabilityV1 EnterResetCutoverTerminal(string root, ValidatedResetMemoryAuthorityV1 authority, IProfileResetMemoryCutoverControlV1 control)
        {
            // A reflected candidate field is mutable evidence only.  Close the
            // original registered candidate; never touch a foreign field.
            ResetCandidateWitness witness; if (ResetCandidateWitnesses.TryGetValue(this, out witness) && witness != null && _untransferredCandidate != null && !ReferenceEquals(_untransferredCandidate, witness.Candidate)) { DisposeUntransferredResetCandidate(witness.Candidate, witness.Control); LatchResetCutoverFailure(); throw new InvalidOperationException("Reset candidate field is corrupt."); }
            if (!TryGetOriginalCohort(out var cohort) || !cohort.Published || !ReferenceEquals(cohort.Router, _router))
            {
                // A cohort/mutable-router mismatch is not a clean foreign
                // argument rejection: it is a local corruption and must close
                // the original cohort before any lock/proof is consumed.
                if (TryGetOriginalCohort(out cohort)) LatchResetCutoverFailure();
                throw new InvalidOperationException("Reset launch cohort is corrupt.");
            }
            if (!ResetCandidateWitnesses.TryGetValue(this, out witness) || witness == null || _untransferredCandidate == null || !ReferenceEquals(_untransferredCandidate, witness.Candidate) || !ReferenceEquals(control, witness.Control) || authority == null || control == null || !IsEligibleResetRoot(root) || _resetLifecycle != ResetAdapterLifecycleV1.None)
                throw new InvalidOperationException("Reset launch cohort is not exact.");
            _currentReceipt.Value.Validate(); _resetCutoverTerminal = true; _resetLifecycle=ResetAdapterLifecycleV1.Entered; cohort.TerminalPhase=ResetCohortTerminalPhaseV1.Entered; _notification = null;
            try
            {
                _resetCandidateControl = control;
                _router.ClaimResetCandidateForTerminal(this, witness.Candidate);
                _resetCandidateClaimedByRouter = true; Interlocked.CompareExchange(ref witness.Claimed, 1, 0);
                return _router.EnterResetCutoverTerminal(this, witness.Candidate, authority, control);
            }
            catch
            {
                if (!_resetCandidateClaimedByRouter) DisposeUntransferredResetCandidate(witness.Candidate, control);
                LatchResetCutoverFailure(); throw;
            }
        }
        internal void PublishCurrentSessionProfile(ResetCutoverCapabilityV1 capability, ProfileCanonicalDocumentV1 document, ProfileBindingOverrideApplyResultV1 binding)
        {
            if (capability == null || !_resetCutoverTerminal || _currentSession != null || _resetReceipt != null || !capability.IsOwnedBy(this)) throw new InvalidOperationException("Reset session publication is not authorized.");
            document.Validate(); binding.Validate(); var snapshot = document.Snapshot;
            if (snapshot.Progression.ProfileRevision != 0 || binding.Outcome != ProfileBindingOverrideApplyOutcomeV1.DefaultsReady || binding.Failure != ProfileBindingOverrideApplyFailureV1.None || binding.CandidateDisposition != ProfileBindingOverrideCandidateDispositionV1.Retain)
                throw new InvalidOperationException("Reset session profile is not default exact.");
            _resetGeneration = capability.Generation;
            var sessionRoot=capability.IsRestartRecovery ? _restartRoot : Normalize(_launchRootWitnessA);
            _currentSession = new CurrentSessionProfileV1(this, sessionRoot, document, binding, capability.Actions, _resetGeneration);
            _resetLifecycle=ResetAdapterLifecycleV1.Transferred;
        }
        internal CurrentSessionProfileV1 CurrentResetSession { get { if (_currentSession == null) throw new InvalidOperationException("Current reset session is unavailable."); _currentSession.Validate(); return _currentSession; } }
        // Called by CurrentSessionProfileV1 after its static immutable data
        // checks.  Do not call Cell.Validate/IsFor here: that would recurse.
        internal bool ValidateCurrentResetCellIdentity(CurrentSessionProfileV1 cell, GameInputActions actions, long generation, string root)
        {
            if (_restartBootstrap==RestartBootstrapStateV1.Reserved || _restartBootstrap==RestartBootstrapStateV1.Completed) return TryGetRestartRecovery(out var recovery) && Volatile.Read(ref recovery.Minted)==1 && (Volatile.Read(ref recovery.TerminalState)==1||Volatile.Read(ref recovery.TerminalState)==2) && recovery.Generation==generation && ReferenceEquals(recovery.Router,_router) && cell!=null && ReferenceEquals(cell,_currentSession) && _router.ValidateRestartRecoveryPair(this,actions,generation) && string.Equals(root,recovery.Root,StringComparison.OrdinalIgnoreCase);
            return cell != null && ReferenceEquals(cell, _currentSession) && _resetCutoverTerminal &&
                (_resetLifecycle == ResetAdapterLifecycleV1.Transferred || _resetLifecycle == ResetAdapterLifecycleV1.Completed) &&
                TryGetOriginalCohort(out var cohort) && ReferenceEquals(cohort.Owner, this) &&
                !HasTerminalCohortMirrorMismatch(cohort) && ReferenceEquals(cohort.Router, _launchRouterWitness) && string.Equals(cohort.Root, root, StringComparison.OrdinalIgnoreCase) &&
                ValidateRouterCurrentResetPair(this, cohort.Router, actions, generation, root);
        }
        internal bool ValidateResetPair(ResetCutoverCapabilityV1 capability)
        {
            if(capability!=null && capability.IsRestartRecovery) return _resetCutoverTerminal && _resetLifecycle==ResetAdapterLifecycleV1.Transferred && _currentSession!=null && TryGetRestartRecovery(out var restart) && Volatile.Read(ref restart.Minted)==1 && Volatile.Read(ref restart.TerminalState)==1 && restart.Generation==capability.Generation && _router.ValidateRestartRecoveryPair(this,capability.Actions,capability.Generation);
            if (capability == null || !_resetCutoverTerminal || _resetLifecycle != ResetAdapterLifecycleV1.Transferred || _currentSession == null || !TryGetOriginalCohort(out var cohort)) return false;
            if (HasTerminalCohortMirrorMismatch(cohort)) { LatchResetCutoverFailure(); return false; }
            return ReferenceEquals(cohort.Router, _router) && ValidateRouterCurrentResetPair(this, cohort.Router, capability.Actions, capability.Generation, cohort.Root);
        }
        internal void LatchResetCutoverFailure() { ProfileNewGameConfirmationV1.RecordExecutionLifecycleFault(this); _resetCutoverTerminal = true; if (_resetLifecycle != ResetAdapterLifecycleV1.Disposed) _resetLifecycle=ResetAdapterLifecycleV1.Faulted; _notification = null; if(TryGetRestartRecovery(out var restart) && restart.Router!=null) { Interlocked.Exchange(ref restart.TerminalState,3); restart.Router.LatchResetCutoverFailure(); return; } if(TryGetOriginalCohort(out var cohort)) { cohort.TerminalPhase=ResetCohortTerminalPhaseV1.Faulted; var trusted=cohort.Router; if(trusted!=null) trusted.LatchResetCutoverFailure(); } }
        internal void DisposeUntransferredResetCandidate(GameInputActions candidate)
        {
            DisposeUntransferredResetCandidate(candidate, ProfileResetMemoryCutoverNoOpControlV1.Instance);
        }
        internal void DisposeUntransferredResetCandidate(GameInputActions candidate, IProfileResetMemoryCutoverControlV1 control)
        {
            if(candidate==null) return; if(control==null)throw new ArgumentNullException(nameof(control)); ResetCandidateWitness witness; if(!ResetCandidateWitnesses.TryGetValue(this,out witness)||witness==null||!ReferenceEquals(candidate,witness.Candidate)||!ReferenceEquals(control,witness.Control)) throw new InvalidOperationException("Reset candidate disposal is not authorized.");
            if(TryGetRestartRecovery(out var restart) && ReferenceEquals(candidate,restart.Candidate))
            {
                if(Volatile.Read(ref restart.CandidateClaimed)!=0 || Interlocked.CompareExchange(ref restart.CandidateDisposeAttempted,1,0)!=0) throw new InvalidOperationException("Restart candidate disposal repeated.");
            }
            else if(Interlocked.CompareExchange(ref witness.DisposeAttempts, 1, 0) != 0) throw new InvalidOperationException("Reset candidate disposal repeated.");
            _untransferredCandidateDisposeAttempted=true; control.InvokeOperation(ResetCutoverOperationV1.DisposeNew, () => witness.Candidate.Dispose());
        }
        internal bool TryTakeResetReceipt(out ProfileResetMemoryReceiptV1 receipt)
        {
            if (_restartBootstrap==RestartBootstrapStateV1.Completed && _resetReceipt!=null && TryGetRestartRecovery(out var restart) && Volatile.Read(ref restart.TerminalState)==2 && Interlocked.CompareExchange(ref restart.ReceiptTaken,1,0)==0) { _resetReceipt.Validate(); _resetReceiptTaken=true; receipt=_resetReceipt; return true; }
            if (TryGetOriginalCohort(out var cohort) && cohort.TerminalPhase == ResetCohortTerminalPhaseV1.Completed && _resetReceipt != null && Interlocked.CompareExchange(ref cohort.ResetReceiptTaken, 1, 0) == 0) { _resetReceipt.Validate(); _resetReceiptTaken = true; receipt = _resetReceipt; return true; }
            receipt = null; return false;
        }
        internal void PublishResetReceipt(ProfileResetMemoryReceiptV1 receipt, ResetCutoverCapabilityV1 capability)
        {
            if (receipt == null || capability == null || !_resetCutoverTerminal || _resetReceipt != null || !ValidateResetPair(capability) || receipt.Generation != capability.Generation) throw new InvalidOperationException("Reset receipt publication is not exact.");
            receipt.Validate(); _resetLifecycle=ResetAdapterLifecycleV1.Completed; if(capability.IsRestartRecovery) { if(!TryGetRestartRecovery(out var restart)||restart.Generation!=capability.Generation||Interlocked.CompareExchange(ref restart.TerminalState,2,1)!=1)throw new InvalidOperationException("Restart receipt cohort is unavailable."); _restartBootstrap=RestartBootstrapStateV1.Completed; _resetReceipt=receipt; return; } if(!TryGetOriginalCohort(out var cohort)) throw new InvalidOperationException("Reset receipt cohort is unavailable."); cohort.TerminalPhase=ResetCohortTerminalPhaseV1.Completed; _resetReceipt = receipt; // REQ-M5D7QC2-006: final authoritative assignment.
        }
        private enum ResetAdapterLifecycleV1 { None = 0, Entered = 1, Transferred = 2, Completed = 3, Faulted = 4, Disposed = 5 }
        private enum RestartBootstrapStateV1 { None = 0, Reserved = 1, RepairBlocked = 2, Completed = 3, Closed = 4 }
        private enum ResetCohortTerminalPhaseV1 { Launch = 0, Entered = 1, Transferred = 2, Completed = 3, Faulted = 4 }
    }

    internal enum DesktopProfileLaunchAdapterFaultStageV1 { AfterInitializeProof = 1, AfterReceiptStaging = 2, AfterNotificationStaging = 3, BeforeConfirm = 4 }

    internal interface IDesktopProfileLaunchEnvironmentV1 { string GetPersistentDataPath(); DateTimeOffset GetUtcNow(); }
    internal interface IDesktopProfileLaunchPreparationV1 { PreparedProfileLaunchV1 Prepare(string root, DateTimeOffset utc); }
    internal sealed class DesktopProfileLaunchEnvironmentV1 : IDesktopProfileLaunchEnvironmentV1
    { public string GetPersistentDataPath() { return Application.persistentDataPath; } public DateTimeOffset GetUtcNow() { return DateTimeOffset.UtcNow; } }
    internal sealed class DesktopProfileLaunchPreparationPortV1 : IDesktopProfileLaunchPreparationV1
    { public PreparedProfileLaunchV1 Prepare(string root, DateTimeOffset utc) { return ProfileLaunchPreparationCoordinatorV1.Prepare(root, utc); } }

    internal readonly struct PreparedHubInitializationProofV1
    {
        private readonly object _owner; private readonly InputMode _mode; private readonly bool _gameplay, _ui, _callbacks, _initialized, _pendingPublication;
        internal PreparedHubInitializationProofV1(object owner, InputMode mode, bool gameplay, bool ui, bool callbacks, bool initialized)
        { _owner = owner; _mode = mode; _gameplay = gameplay; _ui = ui; _callbacks = callbacks; _initialized = initialized; _pendingPublication = true; Validate(); }
        internal InputMode Mode { get { Validate(); return _mode; } } internal bool GameplayEnabled { get { Validate(); return _gameplay; } } internal bool UiEnabled { get { Validate(); return _ui; } } internal bool CallbacksRegistered { get { Validate(); return _callbacks; } } internal bool Initialized { get { Validate(); return _initialized; } } internal bool PendingPublication { get { Validate(); return _pendingPublication; } }
        internal bool IsOwnedBy(object owner) { Validate(); return owner != null && ReferenceEquals(owner, _owner); }
        internal void Validate() { if (_owner == null || _mode != InputMode.UIOnly || _gameplay || !_ui || !_callbacks || !_initialized || !_pendingPublication) throw new InvalidOperationException("Prepared hub proof is malformed."); }
    }

    internal enum ProfileLaunchNotificationKindV1 { RecoveryCompleted = 1, RecoveryArtifactPreservationFailed = 2, PersistenceDeferred = 3 }

    internal readonly struct DesktopProfileLaunchReceiptV1
    {
        private readonly ProfileLaunchSource _source; private readonly long _sourceRevision, _finalRevision; private readonly ProfileCanonicalDocumentV1 _document; private readonly ProfileRecoveryPlanReason _reasons;
        private readonly ProfileBindingOverrideApplyOutcomeV1 _initialOutcome, _finalOutcome; private readonly ProfileBindingOverrideApplyFailureV1 _initialFailure, _finalFailure; private readonly ProfileBindingOverrideCandidateDispositionV1 _initialDisposition, _finalDisposition;
        private readonly ProfileLaunchPreservationProjectionV1 _preservation; private readonly ProfileLaunchSaveProjectionV1 _save; private readonly PreparedProfileLaunchV1 _proof; private readonly bool _hubProof;
        private DesktopProfileLaunchReceiptV1(PreparedProfileLaunchV1 p, bool hub)
        { _proof=p; _source=p.Source; _sourceRevision=p.SourceRevision; _finalRevision=p.FinalRevision; _document=p.FinalDocument; _reasons=p.FinalRecoveryReasons; _initialOutcome=p.InitialBindingOutcome; _initialFailure=p.InitialBindingFailure; _initialDisposition=p.InitialBindingDisposition; _finalOutcome=p.FinalBindingOutcome; _finalFailure=p.FinalBindingFailure; _finalDisposition=ProfileBindingOverrideCandidateDispositionV1.Retain; _preservation=p.Preservation; _save=new ProfileLaunchSaveProjectionV1(p.SaveKind,p.PersistenceOutcome,p.PersistenceStage,p.PrimaryStoredState,p.PreviousStoredState,p.TempStoredState,p.CommittedRevision); _hubProof=hub; ValidateEvidence(false); }
        internal static DesktopProfileLaunchReceiptV1 FromPrepared(PreparedProfileLaunchV1 p) { if (p == null) throw new ArgumentNullException(nameof(p)); return new DesktopProfileLaunchReceiptV1(p, false); }
        internal DesktopProfileLaunchReceiptV1 WithHubProof() { ValidateEvidence(false); return new DesktopProfileLaunchReceiptV1(this, true); }
        private DesktopProfileLaunchReceiptV1(DesktopProfileLaunchReceiptV1 other, bool hub)
        { _proof=other._proof;_source=other._source;_sourceRevision=other._sourceRevision;_finalRevision=other._finalRevision;_document=other._document;_reasons=other._reasons;_initialOutcome=other._initialOutcome;_finalOutcome=other._finalOutcome;_initialFailure=other._initialFailure;_finalFailure=other._finalFailure;_initialDisposition=other._initialDisposition;_finalDisposition=other._finalDisposition;_preservation=other._preservation;_save=other._save;_hubProof=hub; ValidateEvidence(false); }
        internal ProfileLaunchSource Source { get { Validate(); return _source; } } internal long SourceRevision { get { Validate(); return _sourceRevision; } } internal long FinalRevision { get { Validate(); return _finalRevision; } } internal ProfileCanonicalDocumentV1 FinalDocument { get { Validate(); return _document; } } internal ProfileRecoveryPlanReason FinalRecoveryReasons { get { Validate(); return _reasons; } } internal ProfileBindingOverrideApplyOutcomeV1 InitialBindingOutcome { get { Validate(); return _initialOutcome; } } internal ProfileBindingOverrideApplyFailureV1 InitialBindingFailure { get { Validate(); return _initialFailure; } } internal ProfileBindingOverrideCandidateDispositionV1 InitialBindingDisposition { get { Validate(); return _initialDisposition; } } internal ProfileBindingOverrideApplyOutcomeV1 FinalBindingOutcome { get { Validate(); return _finalOutcome; } } internal ProfileBindingOverrideApplyFailureV1 FinalBindingFailure { get { Validate(); return _finalFailure; } } internal ProfileBindingOverrideCandidateDispositionV1 FinalBindingDisposition { get { Validate(); return _finalDisposition; } }
        internal ProfileLaunchPreservationProjectionV1 Preservation { get { Validate(); return _preservation; } } internal ProfileLaunchSaveKindV1 SaveKind { get { Validate(); return _save.Kind; } } internal ProfileLaunchPersistenceOutcomeV1 PersistenceOutcome { get { Validate(); return _save.Outcome; } } internal ProfileAtomicSaveStage PersistenceStage { get { Validate(); return _save.Stage; } } internal ProfileStoredFileState PrimaryStoredState { get { Validate(); return _save.PrimaryState; } } internal ProfileStoredFileState PreviousStoredState { get { Validate(); return _save.PreviousState; } } internal ProfileStoredFileState TempStoredState { get { Validate(); return _save.TempState; } } internal long? CommittedRevision { get { Validate(); return _save.CommittedRevision; } }
        internal bool MayPersist { get { Validate(); return _preservation.MayPersist; } } internal ProfileLoadFileRole? PreservationStoppedRole { get { Validate(); return _preservation.StoppedRole; } }
        internal bool PersistencePending { get { Validate(); var x=_save.Outcome; return x==ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation||x==ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit||x==ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain; } }
        internal bool AnyPreservationAttempted { get { Validate(); return Attempt(_preservation.TempState)||Attempt(_preservation.PrimaryState)||Attempt(_preservation.PreviousState); } }
        internal bool GameplayEnabled { get { Validate(); return false; } } internal bool UiEnabled { get { Validate(); return true; } } internal global::AcadeGameMaker.Core.InputMode InputMode { get { Validate(); return global::AcadeGameMaker.Core.InputMode.UIOnly; } }
        internal void Validate() { ValidateEvidence(true); }
        private void ValidateEvidence(bool requirePublication)
        {
            _document.Validate(); _preservation.Validate(); _save.Validate();
            if (_proof == null) throw new InvalidOperationException("Desktop launch receipt proof is absent.");
            var proof=_proof.ValidateAndReadEvidence();
            if (_source != proof.Source || _sourceRevision != proof.SourceRevision || _finalRevision != proof.FinalRevision || _reasons != proof.FinalRecoveryReasons || _initialOutcome != proof.InitialBindingOutcome || _initialFailure != proof.InitialBindingFailure || _initialDisposition != proof.InitialBindingDisposition || _finalOutcome != proof.FinalBindingOutcome || _finalFailure != proof.FinalBindingFailure || _finalDisposition != ProfileBindingOverrideCandidateDispositionV1.Retain || !Bytes(_document.CanonicalFileBytes, proof.FinalDocument.CanonicalFileBytes) || !Same(_preservation, proof.Preservation) || !Same(_save, proof)) throw new InvalidOperationException("Desktop launch receipt proof does not match copied evidence.");
            if (requirePublication && !_hubProof) throw new InvalidOperationException("Desktop launch receipt was not published.");
            if (_source < ProfileLaunchSource.Primary || _source > ProfileLaunchSource.Default || _finalRevision != _document.Snapshot.ProfileRevision) throw new InvalidOperationException("Desktop launch receipt source is malformed.");
            if (_source == ProfileLaunchSource.Default) { if (_sourceRevision != -1 || _finalRevision != 0 || _reasons != ProfileRecoveryPlanReason.DefaultBootstrap) throw new InvalidOperationException("Default launch receipt is malformed."); }
            else if (_sourceRevision < 0 || (_source == ProfileLaunchSource.Primary && (!(_reasons == 0 || _reasons == ProfileRecoveryPlanReason.InputRepair) || (_reasons == 0 ? _finalRevision != _sourceRevision : _sourceRevision == long.MaxValue || _finalRevision != _sourceRevision + 1))) || (_source == ProfileLaunchSource.Previous && (!(_reasons == ProfileRecoveryPlanReason.PreviousPromotion || _reasons == (ProfileRecoveryPlanReason.PreviousPromotion | ProfileRecoveryPlanReason.InputRepair)) || _sourceRevision == long.MaxValue || _finalRevision != _sourceRevision + 1))) throw new InvalidOperationException("Selected launch receipt is malformed.");
            if (_initialOutcome < ProfileBindingOverrideApplyOutcomeV1.DefaultsReady || _initialOutcome > ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired || _finalOutcome < ProfileBindingOverrideApplyOutcomeV1.DefaultsReady || _finalOutcome > ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired || _initialFailure < ProfileBindingOverrideApplyFailureV1.None || _initialFailure > ProfileBindingOverrideApplyFailureV1.RoundTripMismatch || _finalFailure < ProfileBindingOverrideApplyFailureV1.None || _finalFailure > ProfileBindingOverrideApplyFailureV1.RoundTripMismatch || _initialDisposition < ProfileBindingOverrideCandidateDispositionV1.Retain || _initialDisposition > ProfileBindingOverrideCandidateDispositionV1.Discard || _finalDisposition != ProfileBindingOverrideCandidateDispositionV1.Retain || _finalFailure != ProfileBindingOverrideApplyFailureV1.None || _finalOutcome == ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired) throw new InvalidOperationException("Desktop launch receipt binding is malformed.");
            if (_initialOutcome == ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired && _initialDisposition != ProfileBindingOverrideCandidateDispositionV1.Discard) throw new InvalidOperationException("Desktop launch recovery binding is malformed.");
            if (_initialOutcome != ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired && (_initialFailure != ProfileBindingOverrideApplyFailureV1.None || _initialDisposition != ProfileBindingOverrideCandidateDispositionV1.Retain)) throw new InvalidOperationException("Desktop launch direct binding is malformed.");
        }
        private static bool Attempt(ProfileLaunchPreservationStateV1 s) { return s==ProfileLaunchPreservationStateV1.Preserved||s==ProfileLaunchPreservationStateV1.SourceMissing||s==ProfileLaunchPreservationStateV1.FailedBeforeMove||s==ProfileLaunchPreservationStateV1.MoveOutcomeUncertain; }
        private static bool Bytes(byte[] a, byte[] b) { if(a==null||b==null||a.Length!=b.Length)return false; for(var i=0;i<a.Length;i++)if(a[i]!=b[i])return false; return true; }
        private static bool Same(ProfileLaunchPreservationProjectionV1 a, ProfileLaunchPreservationProjectionV1 b) { return a.TempState==b.TempState&&a.PrimaryState==b.PrimaryState&&a.PreviousState==b.PreviousState&&a.MayPersist==b.MayPersist&&a.StoppedRole==b.StoppedRole; }
        private static bool Same(ProfileLaunchSaveProjectionV1 a, PreparedProfileLaunchV1.ValidatedEvidence b) { return a.Kind==b.SaveKind&&a.Outcome==b.PersistenceOutcome&&a.Stage==b.PersistenceStage&&a.PrimaryState==b.PrimaryStoredState&&a.PreviousState==b.PreviousStoredState&&a.TempState==b.TempStoredState&&a.CommittedRevision==b.CommittedRevision; }
    }

    internal readonly struct ProfileLaunchNotificationV1
    {
        private readonly DesktopProfileLaunchReceiptV1 _receipt; private readonly DesktopProfileLaunchReceiptV1 _correlation; private readonly ProfileLaunchNotificationKindV1 _kind;
        private ProfileLaunchNotificationV1(DesktopProfileLaunchReceiptV1 receipt, ProfileLaunchNotificationKindV1 kind) { _receipt=receipt; _correlation=receipt; _kind=kind; Validate(); }
        internal ProfileLaunchNotificationKindV1 Kind { get { Validate(); return _kind; } } internal DesktopProfileLaunchReceiptV1 Receipt { get { Validate(); return _receipt; } }
        internal ProfileLaunchSource Source { get { Validate(); return _receipt.Source; } } internal long SourceRevision { get { Validate(); return _receipt.SourceRevision; } } internal long FinalRevision { get { Validate(); return _receipt.FinalRevision; } } internal ProfileRecoveryPlanReason FinalRecoveryReasons { get { Validate(); return _receipt.FinalRecoveryReasons; } }
        internal ProfileBindingOverrideApplyOutcomeV1 InitialBindingOutcome { get { Validate(); return _receipt.InitialBindingOutcome; } } internal ProfileBindingOverrideApplyFailureV1 InitialBindingFailure { get { Validate(); return _receipt.InitialBindingFailure; } } internal ProfileBindingOverrideCandidateDispositionV1 InitialBindingDisposition { get { Validate(); return _receipt.InitialBindingDisposition; } }
        internal ProfileBindingOverrideApplyOutcomeV1 FinalBindingOutcome { get { Validate(); return _receipt.FinalBindingOutcome; } } internal ProfileBindingOverrideApplyFailureV1 FinalBindingFailure { get { Validate(); return _receipt.FinalBindingFailure; } } internal ProfileBindingOverrideCandidateDispositionV1 FinalBindingDisposition { get { Validate(); return _receipt.FinalBindingDisposition; } }
        internal ProfileLaunchPreservationProjectionV1 Preservation { get { Validate(); return _receipt.Preservation; } } internal ProfileLaunchSaveKindV1 SaveKind { get { Validate(); return _receipt.SaveKind; } } internal ProfileLaunchPersistenceOutcomeV1 PersistenceOutcome { get { Validate(); return _receipt.PersistenceOutcome; } } internal ProfileAtomicSaveStage PersistenceStage { get { Validate(); return _receipt.PersistenceStage; } } internal bool PersistencePending { get { Validate(); return _receipt.PersistencePending; } }
        internal static ProfileLaunchNotificationV1? FromReceipt(DesktopProfileLaunchReceiptV1 receipt) { receipt.Validate(); ProfileLaunchNotificationKindV1? k=null; if(receipt.PersistencePending) k=ProfileLaunchNotificationKindV1.PersistenceDeferred; else if(!receipt.Preservation.MayPersist) k=ProfileLaunchNotificationKindV1.RecoveryArtifactPreservationFailed; else if(receipt.Source!=ProfileLaunchSource.Primary||receipt.FinalRecoveryReasons!=0||receipt.InitialBindingOutcome==ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired||receipt.AnyPreservationAttempted) k=ProfileLaunchNotificationKindV1.RecoveryCompleted; return k.HasValue ? new ProfileLaunchNotificationV1(receipt,k.Value) : (ProfileLaunchNotificationV1?)null; }
        internal void Validate() { _receipt.Validate(); _correlation.Validate(); if (!Same(_receipt,_correlation)) throw new InvalidOperationException("Profile launch notification correlation is malformed."); var expected=FromReceiptWithoutConstruction(_receipt); if (!expected.HasValue || expected.Value!=_kind) throw new InvalidOperationException("Profile launch notification is malformed."); }
        private static ProfileLaunchNotificationKindV1? FromReceiptWithoutConstruction(DesktopProfileLaunchReceiptV1 r) { if(r.PersistencePending)return ProfileLaunchNotificationKindV1.PersistenceDeferred; if(!r.Preservation.MayPersist)return ProfileLaunchNotificationKindV1.RecoveryArtifactPreservationFailed; if(r.Source!=ProfileLaunchSource.Primary||r.FinalRecoveryReasons!=0||r.InitialBindingOutcome==ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired||r.AnyPreservationAttempted)return ProfileLaunchNotificationKindV1.RecoveryCompleted;return null; }
        private static bool Same(DesktopProfileLaunchReceiptV1 a, DesktopProfileLaunchReceiptV1 b) { var ap=a.Preservation;var bp=b.Preservation; return Bytes(a.FinalDocument.CanonicalFileBytes,b.FinalDocument.CanonicalFileBytes)&&a.Source==b.Source&&a.SourceRevision==b.SourceRevision&&a.FinalRevision==b.FinalRevision&&a.FinalRecoveryReasons==b.FinalRecoveryReasons&&a.InitialBindingOutcome==b.InitialBindingOutcome&&a.InitialBindingFailure==b.InitialBindingFailure&&a.InitialBindingDisposition==b.InitialBindingDisposition&&a.FinalBindingOutcome==b.FinalBindingOutcome&&a.FinalBindingFailure==b.FinalBindingFailure&&a.FinalBindingDisposition==b.FinalBindingDisposition&&ap.TempState==bp.TempState&&ap.PrimaryState==bp.PrimaryState&&ap.PreviousState==bp.PreviousState&&ap.MayPersist==bp.MayPersist&&ap.StoppedRole==bp.StoppedRole&&a.SaveKind==b.SaveKind&&a.PersistenceOutcome==b.PersistenceOutcome&&a.PersistenceStage==b.PersistenceStage&&a.PrimaryStoredState==b.PrimaryStoredState&&a.PreviousStoredState==b.PreviousStoredState&&a.TempStoredState==b.TempStoredState&&a.CommittedRevision==b.CommittedRevision&&a.PersistencePending==b.PersistencePending; }
        private static bool Bytes(byte[] a, byte[] b) { if(a==null||b==null||a.Length!=b.Length)return false;for(var i=0;i<a.Length;i++)if(a[i]!=b[i])return false;return true; }
    }
}
