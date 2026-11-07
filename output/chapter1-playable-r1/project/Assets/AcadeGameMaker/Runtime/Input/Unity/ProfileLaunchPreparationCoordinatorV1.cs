using System;
using System.Collections.Generic;
using System.IO;
using AcadeGameMaker.Input;
using AcadeGameMaker.Profile;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-M5D7L-001..008. This is deliberately preparation-only: it owns neither a router nor hub entry.
    internal enum ProfileLaunchSaveKindV1 { None = 1, Ordinary = 2, PrimaryInputRecovery = 3 }
    internal enum ProfileLaunchPersistenceOutcomeV1 { NotRequired = 1, BlockedByPreservation = 2, CommittedFirst = 3, CommittedReplacement = 4, CommittedPrimaryInputRecovery = 5, FailedBeforeCommit = 6, CommitOutcomeUncertain = 7 }

    internal readonly struct ProfileLaunchObservationProjectionV1
    {
        private readonly ProfileLoadCandidateV1 _primary, _previous, _temp;
        internal ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1 primary, ProfileLoadCandidateV1 previous, ProfileLoadCandidateV1 temp) { _primary = primary; _previous = previous; _temp = temp; }
        internal ProfileLoadCandidateV1 Primary { get { Validate(); return _primary; } }
        internal ProfileLoadCandidateV1 Previous { get { Validate(); return _previous; } }
        internal ProfileLoadCandidateV1 Temp { get { Validate(); return _temp; } }
        internal void Validate() { Role(_primary, ProfileLoadFileRole.Primary); Role(_previous, ProfileLoadFileRole.Previous); Role(_temp, ProfileLoadFileRole.Temp); }
        internal ProfileLaunchObservationBatchV1 ToBatch() { Validate(); return new ProfileLaunchObservationBatchV1(_primary, _previous, _temp); }
        private static void Role(ProfileLoadCandidateV1 value, ProfileLoadFileRole expected) { value.Validate(); if (value.Role != expected) throw new InvalidOperationException("Launch observation projection is malformed."); }
    }

    internal readonly struct ProfileLaunchPreservationProjectionV1
    {
        private readonly ProfileLaunchPreservationStateV1 _temp, _primary, _previous;
        private readonly bool _mayPersist;
        private readonly ProfileLoadFileRole? _stopped;
        private readonly ProfileFilePreservationReason _tempIntent, _primaryIntent, _previousIntent;
        private readonly int _attempted;
        internal ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1 temp, ProfileLaunchPreservationStateV1 primary, ProfileLaunchPreservationStateV1 previous, bool mayPersist, ProfileLoadFileRole? stopped, ProfileFilePreservationReason tempIntent, ProfileFilePreservationReason primaryIntent, ProfileFilePreservationReason previousIntent, int attempted)
        { _temp = temp; _primary = primary; _previous = previous; _mayPersist = mayPersist; _stopped = stopped; _tempIntent = tempIntent; _primaryIntent = primaryIntent; _previousIntent = previousIntent; _attempted = attempted; }
        internal ProfileLaunchPreservationStateV1 TempState { get { Validate(); return _temp; } }
        internal ProfileLaunchPreservationStateV1 PrimaryState { get { Validate(); return _primary; } }
        internal ProfileLaunchPreservationStateV1 PreviousState { get { Validate(); return _previous; } }
        internal bool MayPersist { get { Validate(); return _mayPersist; } }
        internal ProfileLoadFileRole? StoppedRole { get { Validate(); return _stopped; } }
        internal void ValidateAgainst(ProfileLoadSelectionPlanV1 selection)
        {
            Validate(); selection.Validate();
            if (_tempIntent != selection.TempPreservation || _primaryIntent != selection.PrimaryPreservation || _previousIntent != selection.PreviousPreservation) throw new InvalidOperationException("Preservation projection does not match selection.");
        }
        internal void Validate()
        {
            if (_attempted < 0 || _attempted > 3 || !Intent(_tempIntent, ProfileLoadFileRole.Temp) || !Intent(_primaryIntent, ProfileLoadFileRole.Primary) || !Intent(_previousIntent, ProfileLoadFileRole.Previous)) throw new InvalidOperationException("Launch preservation projection is malformed.");
            var states = new[] { _temp, _primary, _previous }; var intents = new[] { _tempIntent, _primaryIntent, _previousIntent }; var roles = new[] { ProfileLoadFileRole.Temp, ProfileLoadFileRole.Primary, ProfileLoadFileRole.Previous };
            var stopped = false; var calls = 0; ProfileLoadFileRole? expected = null;
            for (var i = 0; i < states.Length; i++)
            {
                if (intents[i] == ProfileFilePreservationReason.None) { if (states[i] != ProfileLaunchPreservationStateV1.NotRequired) throw new InvalidOperationException("Non-required preservation proof is malformed."); continue; }
                if (stopped) { if (states[i] != ProfileLaunchPreservationStateV1.NotAttempted) throw new InvalidOperationException("Stopped preservation proof is malformed."); continue; }
                calls++;
                if (states[i] == ProfileLaunchPreservationStateV1.Preserved || states[i] == ProfileLaunchPreservationStateV1.SourceMissing) continue;
                if (states[i] != ProfileLaunchPreservationStateV1.FailedBeforeMove && states[i] != ProfileLaunchPreservationStateV1.MoveOutcomeUncertain) throw new InvalidOperationException("Preservation state is malformed.");
                stopped = true; expected = roles[i];
            }
            if (calls != _attempted || _stopped != expected || _mayPersist != !stopped) throw new InvalidOperationException("Preservation prefix proof is malformed.");
        }
        private static bool Intent(ProfileFilePreservationReason value, ProfileLoadFileRole role) => value == ProfileFilePreservationReason.None || (role == ProfileLoadFileRole.Temp && value == ProfileFilePreservationReason.StaleTemp) || (role == ProfileLoadFileRole.Primary && (value == ProfileFilePreservationReason.InvalidPrimary || value == ProfileFilePreservationReason.UnsupportedPrimary || value == ProfileFilePreservationReason.UnreadablePrimary)) || (role == ProfileLoadFileRole.Previous && (value == ProfileFilePreservationReason.InvalidPrevious || value == ProfileFilePreservationReason.UnsupportedPrevious || value == ProfileFilePreservationReason.UnreadablePrevious));
    }

    internal readonly struct ProfileLaunchSaveProjectionV1
    {
        private readonly ProfileLaunchSaveKindV1 _kind; private readonly ProfileLaunchPersistenceOutcomeV1 _outcome; private readonly ProfileAtomicSaveStage _stage; private readonly ProfileStoredFileState _primary, _previous, _temp; private readonly long? _revision;
        internal ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1 kind, ProfileLaunchPersistenceOutcomeV1 outcome, ProfileAtomicSaveStage stage, ProfileStoredFileState primary, ProfileStoredFileState previous, ProfileStoredFileState temp, long? revision)
        { _kind = kind; _outcome = outcome; _stage = stage; _primary = primary; _previous = previous; _temp = temp; _revision = revision; }
        internal ProfileLaunchSaveKindV1 Kind { get { Validate(); return _kind; } } internal ProfileLaunchPersistenceOutcomeV1 Outcome { get { Validate(); return _outcome; } } internal ProfileAtomicSaveStage Stage { get { Validate(); return _stage; } } internal ProfileStoredFileState PrimaryState { get { Validate(); return _primary; } } internal ProfileStoredFileState PreviousState { get { Validate(); return _previous; } } internal ProfileStoredFileState TempState { get { Validate(); return _temp; } } internal long? CommittedRevision { get { Validate(); return _revision; } }
        internal void Validate()
        {
            if (_kind < ProfileLaunchSaveKindV1.None || _kind > ProfileLaunchSaveKindV1.PrimaryInputRecovery || _outcome < ProfileLaunchPersistenceOutcomeV1.NotRequired || _outcome > ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain || _stage < ProfileAtomicSaveStage.None || _stage > ProfileAtomicSaveStage.PostCommitValidation || !State(_primary) || !State(_previous) || !State(_temp)) throw new InvalidOperationException("Launch save projection is malformed.");
            if (_outcome == ProfileLaunchPersistenceOutcomeV1.NotRequired)
            { if (_kind != ProfileLaunchSaveKindV1.None || _stage != ProfileAtomicSaveStage.None || _primary != ProfileStoredFileState.NotProbed || _previous != ProfileStoredFileState.NotProbed || _temp != ProfileStoredFileState.NotProbed || _revision.HasValue) throw new InvalidOperationException("Not-required persistence projection is malformed."); return; }
            if (_outcome == ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation)
            { if (_kind == ProfileLaunchSaveKindV1.None || _stage != ProfileAtomicSaveStage.None || _primary != ProfileStoredFileState.NotProbed || _previous != ProfileStoredFileState.NotProbed || _temp != ProfileStoredFileState.NotProbed || _revision.HasValue) throw new InvalidOperationException("Blocked persistence projection is malformed."); return; }
            if (_outcome == ProfileLaunchPersistenceOutcomeV1.CommittedFirst || _outcome == ProfileLaunchPersistenceOutcomeV1.CommittedReplacement || _outcome == ProfileLaunchPersistenceOutcomeV1.CommittedPrimaryInputRecovery)
            {
                if (_stage != ProfileAtomicSaveStage.None || !_revision.HasValue || _revision.Value < 0) throw new InvalidOperationException("Committed persistence projection is malformed.");
                if (_outcome == ProfileLaunchPersistenceOutcomeV1.CommittedFirst && (_kind != ProfileLaunchSaveKindV1.Ordinary || _primary != ProfileStoredFileState.ValidCurrentInput || _temp != ProfileStoredFileState.Missing || _previous == ProfileStoredFileState.NotProbed || _previous == ProfileStoredFileState.Unreadable)) throw new InvalidOperationException("First-save projection is malformed.");
                if (_outcome == ProfileLaunchPersistenceOutcomeV1.CommittedReplacement && (_kind != ProfileLaunchSaveKindV1.Ordinary || _primary != ProfileStoredFileState.ValidCurrentInput || _previous != ProfileStoredFileState.ValidCurrentInput || _temp != ProfileStoredFileState.Missing)) throw new InvalidOperationException("Replacement projection is malformed.");
                if (_outcome == ProfileLaunchPersistenceOutcomeV1.CommittedPrimaryInputRecovery && (_kind != ProfileLaunchSaveKindV1.PrimaryInputRecovery || _primary != ProfileStoredFileState.ValidCurrentInput || _previous != ProfileStoredFileState.ValidInputRecoveryRequired || _temp != ProfileStoredFileState.Missing)) throw new InvalidOperationException("Input recovery projection is malformed.");
                return;
            }
            if (_kind == ProfileLaunchSaveKindV1.None || _revision.HasValue) throw new InvalidOperationException("Failed persistence projection is malformed.");
            if (_outcome == ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit)
            { if (!Pre(_stage) || (_kind == ProfileLaunchSaveKindV1.PrimaryInputRecovery && (_primary == ProfileStoredFileState.NotProbed || _previous == ProfileStoredFileState.NotProbed || _temp == ProfileStoredFileState.NotProbed))) throw new InvalidOperationException("Pre-commit stage is malformed."); return; }
            if (!Post(_stage) || _primary == ProfileStoredFileState.NotProbed || _previous == ProfileStoredFileState.NotProbed || _temp == ProfileStoredFileState.NotProbed) throw new InvalidOperationException("Uncertain persistence projection is malformed.");
        }
        private static bool State(ProfileStoredFileState value) => value >= ProfileStoredFileState.NotProbed && value <= ProfileStoredFileState.Unreadable;
        private static bool Pre(ProfileAtomicSaveStage value) => value == ProfileAtomicSaveStage.DirectoryPreparation || value == ProfileAtomicSaveStage.TempWriteFlushClose || value == ProfileAtomicSaveStage.TempReadValidation || value == ProfileAtomicSaveStage.ExistingPrimaryValidation;
        private static bool Post(ProfileAtomicSaveStage value) => value == ProfileAtomicSaveStage.CommitCall || value == ProfileAtomicSaveStage.PostCommitValidation;
    }

    internal interface IProfileLaunchPreparationProfilePortV1
    {
        ProfileLaunchObservationProjectionV1 Observe(string root);
        ProfileLaunchPreservationProjectionV1 Preserve(string root, ProfileLaunchObservationProjectionV1 observation, ProfileLoadSelectionPlanV1 selection, DateTimeOffset utc);
        ProfileLaunchSaveProjectionV1 Save(string root, ProfileCanonicalDocumentV1 document);
        ProfileLaunchSaveProjectionV1 SaveInputRecovery(string root, ProfileLoadCandidateV1 observedPrimary);
    }
    internal interface IProfileLaunchPreparationActionsPortV1 { GameInputActions Create(); ProfileBindingOverrideApplyResultV1 Apply(ProfileInputSnapshot input, GameInputActions candidate); void Dispose(GameInputActions candidate); }

    internal sealed class ProfileLaunchPreparationProfilePortV1 : IProfileLaunchPreparationProfilePortV1
    {
        private readonly ProfileRootOperationLockV1 _ownership;
        internal ProfileLaunchPreparationProfilePortV1(ProfileRootOperationLockV1 ownership) { _ownership = ownership ?? throw new ArgumentNullException(nameof(ownership)); }
        // REQ-M5D7QC-007. This production port is entered only while
        // ProfileLaunchPreparationCoordinatorV1 holds profile.operation.lock.
        public ProfileLaunchObservationProjectionV1 Observe(string root) { var value = ProfileLaunchObservationAdapterV1.ObserveUnderHeldRoot(_ownership, root); value.Validate(); return new ProfileLaunchObservationProjectionV1(value.Primary, value.Previous, value.Temp); }
        public ProfileLaunchPreservationProjectionV1 Preserve(string root, ProfileLaunchObservationProjectionV1 observation, ProfileLoadSelectionPlanV1 selection, DateTimeOffset utc)
        { var value = ProfileLaunchPreservationExecutorV1.Execute(root, observation.ToBatch(), selection, utc); value.Validate(); return new ProfileLaunchPreservationProjectionV1(value.TempState, value.PrimaryState, value.PreviousState, value.MayPersist, value.StoppedRole, selection.TempPreservation, selection.PrimaryPreservation, selection.PreviousPreservation, Count(value, selection)); }
        public ProfileLaunchSaveProjectionV1 Save(string root, ProfileCanonicalDocumentV1 document) { var value = ProfileAtomicSaveServiceV1.SaveUnderHeldRoot(_ownership, root, document); value.Validate(); return Ordinary(value); }
        public ProfileLaunchSaveProjectionV1 SaveInputRecovery(string root, ProfileLoadCandidateV1 primary) { var value = ProfileAtomicSaveServiceV1.SaveInputRecoveryUnderHeldRoot(_ownership, root, primary); value.Validate(); return Recovery(value); }
        private static int Count(ProfileLaunchPreservationResultV1 value, ProfileLoadSelectionPlanV1 selection) { var count = 0; if (selection.TempPreservation != ProfileFilePreservationReason.None && value.TempState != ProfileLaunchPreservationStateV1.NotAttempted) count++; if (selection.PrimaryPreservation != ProfileFilePreservationReason.None && value.PrimaryState != ProfileLaunchPreservationStateV1.NotAttempted) count++; if (selection.PreviousPreservation != ProfileFilePreservationReason.None && value.PreviousState != ProfileLaunchPreservationStateV1.NotAttempted) count++; return count; }
        private static ProfileLaunchSaveProjectionV1 Ordinary(ProfileAtomicSaveResultV1 value)
        {
            if (value.Outcome == ProfileAtomicSaveOutcome.CommittedFirst) return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedFirst, value.FailureStage, value.PrimaryState, value.PreviousState, value.TempState, value.CommittedRevision);
            if (value.Outcome == ProfileAtomicSaveOutcome.CommittedReplacement) return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedReplacement, value.FailureStage, value.PrimaryState, value.PreviousState, value.TempState, value.CommittedRevision);
            return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, value.Outcome == ProfileAtomicSaveOutcome.FailedBeforeCommit ? ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit : ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain, value.FailureStage, value.PrimaryState, value.PreviousState, value.TempState, null);
        }
        private static ProfileLaunchSaveProjectionV1 Recovery(ProfileInputRecoverySaveResultV1 value)
        {
            if (value.Outcome == ProfileInputRecoverySaveOutcomeV1.CommittedRecoveryReplacement) return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.PrimaryInputRecovery, ProfileLaunchPersistenceOutcomeV1.CommittedPrimaryInputRecovery, value.FailureStage, value.PrimaryState, value.PreviousState, value.TempState, value.CommittedRevision);
            return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.PrimaryInputRecovery, value.Outcome == ProfileInputRecoverySaveOutcomeV1.FailedBeforeCommit ? ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit : ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain, value.FailureStage, value.PrimaryState, value.PreviousState, value.TempState, null);
        }
    }
    internal sealed class ProfileLaunchPreparationActionsPortV1 : IProfileLaunchPreparationActionsPortV1
    {
        public GameInputActions Create() { return new GameInputActions(); }
        public ProfileBindingOverrideApplyResultV1 Apply(ProfileInputSnapshot input, GameInputActions candidate) { return ProfileBindingOverrideApplyAdapterV1.Apply(input, candidate); }
        public void Dispose(GameInputActions candidate) { candidate.Dispose(); }
    }

    internal sealed class PreparedProfileLaunchV1 : IDisposable
    {
        private readonly ProfileLaunchSource _source; private readonly long _sourceRevision, _finalRevision; private readonly ProfileCanonicalDocumentV1 _document; private readonly ProfileRecoveryPlanReason _reasons; private readonly ProfileLaunchObservationProjectionV1 _observation; private readonly ProfileBindingOverrideApplyResultV1 _initialBinding, _finalBinding; private readonly ProfileLaunchPreservationProjectionV1 _preservation; private readonly ProfileLaunchSaveProjectionV1 _save; private readonly bool _requiresSave; private readonly IProfileLaunchPreparationActionsPortV1 _disposer;
        private GameInputActions _actions; private int _ownership; // 0 owned, 1 transferred, 2 disposed
        internal PreparedProfileLaunchV1(ProfileLaunchSource source, long sourceRevision, ProfileCanonicalDocumentV1 document, ProfileRecoveryPlanReason reasons, ProfileLaunchObservationProjectionV1 observation, ProfileBindingOverrideApplyResultV1 initialBinding, ProfileBindingOverrideApplyResultV1 finalBinding, ProfileLaunchPreservationProjectionV1 preservation, ProfileLaunchSaveProjectionV1 save, bool requiresSave, GameInputActions actions, IProfileLaunchPreparationActionsPortV1 disposer)
        { _source = source; _sourceRevision = sourceRevision; _document = document; _finalRevision = document.Snapshot.ProfileRevision; _reasons = reasons; _observation = observation; _initialBinding = initialBinding; _finalBinding = finalBinding; _preservation = preservation; _save = save; _requiresSave = requiresSave; _actions = actions; _disposer = disposer; _ownership = 0; Validate(); }
        internal ProfileLaunchSource Source { get { Validate(); return _source; } } internal long SourceRevision { get { Validate(); return _sourceRevision; } } internal long FinalRevision { get { Validate(); return _finalRevision; } } internal ProfileCanonicalDocumentV1 FinalDocument { get { Validate(); return _document; } } internal ProfileRecoveryPlanReason FinalRecoveryReasons { get { Validate(); return _reasons; } } internal ProfileBindingOverrideApplyOutcomeV1 InitialBindingOutcome { get { Validate(); return _initialBinding.Outcome; } } internal ProfileBindingOverrideApplyFailureV1 InitialBindingFailure { get { Validate(); return _initialBinding.Failure; } } internal ProfileBindingOverrideCandidateDispositionV1 InitialBindingDisposition { get { Validate(); return _initialBinding.CandidateDisposition; } } internal ProfileBindingOverrideApplyOutcomeV1 FinalBindingOutcome { get { Validate(); return _finalBinding.Outcome; } } internal ProfileBindingOverrideApplyFailureV1 FinalBindingFailure { get { Validate(); return _finalBinding.Failure; } } internal ProfileBindingOverrideApplyOutcomeV1 BindingOutcome { get { Validate(); return _finalBinding.Outcome; } } internal ProfileBindingOverrideApplyFailureV1 BindingFailure { get { Validate(); return _finalBinding.Failure; } } internal ProfileLaunchPreservationProjectionV1 Preservation { get { Validate(); return _preservation; } } internal ProfileLaunchSaveKindV1 SaveKind { get { Validate(); return _save.Kind; } } internal ProfileLaunchPersistenceOutcomeV1 PersistenceOutcome { get { Validate(); return _save.Outcome; } } internal ProfileAtomicSaveStage PersistenceStage { get { Validate(); return _save.Stage; } } internal ProfileStoredFileState PrimaryStoredState { get { Validate(); return _save.PrimaryState; } } internal ProfileStoredFileState PreviousStoredState { get { Validate(); return _save.PreviousState; } } internal ProfileStoredFileState TempStoredState { get { Validate(); return _save.TempState; } } internal long? CommittedRevision { get { Validate(); return _save.CommittedRevision; } } internal bool PersistencePending { get { Validate(); return _save.Outcome == ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation || _save.Outcome == ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit || _save.Outcome == ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain; } } internal bool HubEntryAllowed { get { Validate(); return true; } }
        // REQ-AUTOCH1: 한 공개 검증 경계의 동기 비교 안에서만 사용하는 지역 투영.
        internal ValidatedEvidence ValidateAndReadEvidence()
        {Validate();return new ValidatedEvidence(this);}
        internal readonly struct ValidatedEvidence
        {
            internal readonly ProfileLaunchSource Source;
            internal readonly long SourceRevision;
            internal readonly long FinalRevision;
            internal readonly ProfileRecoveryPlanReason FinalRecoveryReasons;
            internal readonly ProfileBindingOverrideApplyOutcomeV1 InitialBindingOutcome;
            internal readonly ProfileBindingOverrideApplyFailureV1 InitialBindingFailure;
            internal readonly ProfileBindingOverrideCandidateDispositionV1 InitialBindingDisposition;
            internal readonly ProfileBindingOverrideApplyOutcomeV1 FinalBindingOutcome;
            internal readonly ProfileBindingOverrideApplyFailureV1 FinalBindingFailure;
            internal readonly ProfileCanonicalDocumentV1 FinalDocument;
            internal readonly ProfileLaunchPreservationProjectionV1 Preservation;
            internal readonly ProfileLaunchSaveKindV1 SaveKind;
            internal readonly ProfileLaunchPersistenceOutcomeV1 PersistenceOutcome;
            internal readonly ProfileAtomicSaveStage PersistenceStage;
            internal readonly ProfileStoredFileState PrimaryStoredState;
            internal readonly ProfileStoredFileState PreviousStoredState;
            internal readonly ProfileStoredFileState TempStoredState;
            internal readonly long? CommittedRevision;
            internal ValidatedEvidence(PreparedProfileLaunchV1 source)
            {
                Source=source._source;
                SourceRevision=source._sourceRevision;
                FinalRevision=source._finalRevision;
                FinalRecoveryReasons=source._reasons;
                InitialBindingOutcome=source._initialBinding.Outcome;
                InitialBindingFailure=source._initialBinding.Failure;
                InitialBindingDisposition=source._initialBinding.CandidateDisposition;
                FinalBindingOutcome=source._finalBinding.Outcome;
                FinalBindingFailure=source._finalBinding.Failure;
                FinalDocument=source._document;
                Preservation=source._preservation;
                SaveKind=source._save.Kind;
                PersistenceOutcome=source._save.Outcome;
                PersistenceStage=source._save.Stage;
                PrimaryStoredState=source._save.PrimaryState;
                PreviousStoredState=source._save.PreviousState;
                TempStoredState=source._save.TempState;
                CommittedRevision=source._save.CommittedRevision;
            }
        }
        internal GameInputActions TakeActions() { Validate(); if (_ownership != 0) throw new InvalidOperationException("Prepared actions are no longer owned."); var result = _actions; _actions = null; _ownership = 1; return result; }
        public void Dispose() { if (_ownership != 0) return; var actions = _actions; _actions = null; _ownership = 2; if (actions == null || _disposer == null) throw new InvalidOperationException("Prepared actions ownership is malformed."); _disposer.Dispose(actions); }
        internal void Validate()
        {
            if (_source < ProfileLaunchSource.Primary || _source > ProfileLaunchSource.Default || _finalRevision < 0 || _disposer == null) throw new InvalidOperationException("Prepared launch evidence is malformed.");
            if (_source == ProfileLaunchSource.Default)
            { if (_sourceRevision != -1 || _finalRevision != 0 || _reasons != ProfileRecoveryPlanReason.DefaultBootstrap) throw new InvalidOperationException("Default launch evidence is malformed."); }
            else
            {
                if (_sourceRevision < 0) throw new InvalidOperationException("Selected launch source is malformed.");
                if (_source == ProfileLaunchSource.Primary)
                { if (!(_reasons == 0 || _reasons == ProfileRecoveryPlanReason.InputRepair) || (_reasons == 0 ? _finalRevision != _sourceRevision : _sourceRevision == long.MaxValue || _finalRevision != _sourceRevision + 1)) throw new InvalidOperationException("Primary launch revision proof is malformed."); }
                else if (!(_reasons == ProfileRecoveryPlanReason.PreviousPromotion || _reasons == (ProfileRecoveryPlanReason.PreviousPromotion | ProfileRecoveryPlanReason.InputRepair)) || _sourceRevision == long.MaxValue || _finalRevision != _sourceRevision + 1) throw new InvalidOperationException("Previous launch revision proof is malformed.");
            }
            _document.Validate(); if (!_document.Snapshot.Input.IsCurrentCompatible || _document.Snapshot.ProfileRevision != _finalRevision) throw new InvalidOperationException("Prepared launch document is malformed."); _observation.Validate(); var selection = ProfileLoadSelectorV1.Select(_observation.Primary, _observation.Previous, _observation.Temp); selection.Validate(); if (selection.Source != _source || selection.SourceRevision != _sourceRevision) throw new InvalidOperationException("Prepared launch source is not bound to observation."); _preservation.ValidateAgainst(selection); _save.Validate(); ValidateBindingProof(selection);
            if (_requiresSave)
            { if (_save.Outcome == ProfileLaunchPersistenceOutcomeV1.NotRequired || (!_preservation.MayPersist && _save.Outcome != ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation) || (_save.Outcome == ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation && _preservation.MayPersist)) throw new InvalidOperationException("Prepared launch persistence proof is malformed."); }
            else if (_save.Outcome != ProfileLaunchPersistenceOutcomeV1.NotRequired || _save.Kind != ProfileLaunchSaveKindV1.None) throw new InvalidOperationException("Unnecessary persistence proof is malformed.");
            if (_save.CommittedRevision.HasValue && _save.CommittedRevision.Value != _finalRevision) throw new InvalidOperationException("Committed revision is malformed."); ValidateSavePath(selection);
            if (_ownership == 0) { ValidateDisabled(_actions); return; } if ((_ownership == 1 || _ownership == 2) && _actions == null) return; throw new InvalidOperationException("Prepared actions ownership is malformed.");
        }
        private void ValidateBindingProof(ProfileLoadSelectionPlanV1 selection)
        {
            _initialBinding.Validate(); _finalBinding.Validate(); var expectedInput = selection.InMemoryDocument.Snapshot.Input.BindingOverrides.CanonicalText;
            var initialSuccess = (_initialBinding.Outcome == ProfileBindingOverrideApplyOutcomeV1.DefaultsReady || _initialBinding.Outcome == ProfileBindingOverrideApplyOutcomeV1.OverridesApplied) && _initialBinding.Failure == ProfileBindingOverrideApplyFailureV1.None && _initialBinding.CandidateDisposition == ProfileBindingOverrideCandidateDispositionV1.Retain;
            var initialRecovery = _initialBinding.Outcome == ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired && _initialBinding.CandidateDisposition == ProfileBindingOverrideCandidateDispositionV1.Discard;
            if ((!initialSuccess && !initialRecovery) || !string.Equals(_initialBinding.RequestedCanonicalText, expectedInput, StringComparison.Ordinal)) throw new InvalidOperationException("Initial binding evidence is malformed.");
            if (initialRecovery)
            {
                if (expectedInput.Length == 0 || _finalBinding.Outcome != ProfileBindingOverrideApplyOutcomeV1.DefaultsReady || _finalBinding.Failure != ProfileBindingOverrideApplyFailureV1.None || _finalBinding.CandidateDisposition != ProfileBindingOverrideCandidateDispositionV1.Retain || _finalBinding.RequestedCanonicalText != string.Empty || _finalBinding.VerifiedCanonicalText != string.Empty) throw new InvalidOperationException("Recovered binding evidence is malformed.");
                if (_source == ProfileLaunchSource.Default) throw new InvalidOperationException("Default binding recovery is malformed.");
                var source = _source == ProfileLaunchSource.Primary ? _observation.Primary : _observation.Previous; if (source.Kind != ProfileLoadCandidateKind.Decoded || source.DecodeResult.Classification != ProfileCanonicalDecodeClassification.ValidCurrentInput || source.DecodeResult.Document.Snapshot.Input.BindingOverrides.CanonicalText != expectedInput) throw new InvalidOperationException("Binding recovery source proof is malformed.");
                var repair = _source == ProfileLaunchSource.Primary ? ProfileRecoveryPlannerV1.PlanPrimaryBindingApplyFailure(source.DecodeResult) : ProfileRecoveryPlannerV1.PlanPreviousBindingApplyFailure(source.DecodeResult);
                if (_reasons != repair.Reasons || _finalRevision != repair.ResultRevision || !_requiresSave || !BytesEqual(_document.CanonicalFileBytes, repair.ResultDocument.CanonicalFileBytes)) throw new InvalidOperationException("Binding recovery final proof is malformed."); return;
            }
            if (!SameBinding(_initialBinding, _finalBinding) || _reasons != selection.RecoveryReasons || _requiresSave != selection.RequiresAtomicSave || !BytesEqual(_document.CanonicalFileBytes, selection.InMemoryDocument.CanonicalFileBytes)) throw new InvalidOperationException("Direct binding final proof is malformed.");
        }
        private void ValidateSavePath(ProfileLoadSelectionPlanV1 selection)
        {
            var primaryRecovery = _observation.Primary.Kind == ProfileLoadCandidateKind.Decoded && (_observation.Primary.DecodeResult.Classification == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired || _observation.Primary.DecodeResult.Classification == ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired);
            var initialRecovery = _initialBinding.Outcome == ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired;
            var expected = primaryRecovery && _source == ProfileLaunchSource.Primary && _reasons == ProfileRecoveryPlanReason.InputRepair && !initialRecovery ? ProfileLaunchSaveKindV1.PrimaryInputRecovery : ProfileLaunchSaveKindV1.Ordinary;
            if (!_requiresSave) { if (_save.Kind != ProfileLaunchSaveKindV1.None || _save.Outcome != ProfileLaunchPersistenceOutcomeV1.NotRequired) throw new InvalidOperationException("No-save launch proof is malformed."); return; }
            if (_save.Kind != expected) throw new InvalidOperationException("Save dispatch proof is malformed.");
            if (_source == ProfileLaunchSource.Default && (_reasons != ProfileRecoveryPlanReason.DefaultBootstrap || expected != ProfileLaunchSaveKindV1.Ordinary)) throw new InvalidOperationException("Default save proof is malformed.");
            if (_source == ProfileLaunchSource.Previous && expected != ProfileLaunchSaveKindV1.Ordinary) throw new InvalidOperationException("Previous save proof is malformed.");
            if (initialRecovery && expected != ProfileLaunchSaveKindV1.Ordinary) throw new InvalidOperationException("Apply recovery save proof is malformed.");
        }
        private static bool SameBinding(ProfileBindingOverrideApplyResultV1 left, ProfileBindingOverrideApplyResultV1 right) { if (left.Outcome != right.Outcome || left.Failure != right.Failure || left.CandidateDisposition != right.CandidateDisposition || !string.Equals(left.RequestedCanonicalText, right.RequestedCanonicalText, StringComparison.Ordinal) || left.HasVerifiedCanonicalText != right.HasVerifiedCanonicalText) return false; return !left.HasVerifiedCanonicalText || string.Equals(left.VerifiedCanonicalText, right.VerifiedCanonicalText, StringComparison.Ordinal); }
        private static bool BytesEqual(byte[] left, byte[] right) { if (left == null || right == null || left.Length != right.Length) return false; for (var i = 0; i < left.Length; i++) if (left[i] != right[i]) return false; return true; }
        private static void ValidateDisabled(GameInputActions actions) { if (actions == null || actions.asset == null || actions.asset.enabled) throw new InvalidOperationException("Prepared actions are malformed."); foreach (var map in actions.asset.actionMaps) if (map.enabled) throw new InvalidOperationException("Prepared action map is enabled."); }
    }

    internal static class ProfileLaunchPreparationCoordinatorV1
    {
        private static readonly object RegistryGate = new object();
        private static readonly Dictionary<string, LeaseEntry> Leases = new Dictionary<string, LeaseEntry>(StringComparer.OrdinalIgnoreCase);
        // REQ-M5D7QC-006/007. Production launch preparation has one durable
        // ownership span from barrier interception through token construction.
        internal static PreparedProfileLaunchV1 Prepare(string rootDirectoryPath, DateTimeOffset utcNow)
        {
            var actions = new ProfileLaunchPreparationActionsPortV1();
            var root = NormalizeRootAndUtc(rootDirectoryPath, utcNow);
            using (var ownership = ProfileRootOperationLockV1.Acquire(root))
            {
                var profile = new ProfileLaunchPreparationProfilePortV1(ownership);
                ProfileResetBarrierV1.ThrowIfPresent(root);
                return PrepareCore(root, utcNow, profile, actions);
            }
        }

        // Test-only injected seam. Production callers use the two-argument
        // overload above, which owns the cross-process root lock.
        internal static PreparedProfileLaunchV1 Prepare(string rootDirectoryPath, DateTimeOffset utcNow, IProfileLaunchPreparationProfilePortV1 profile, IProfileLaunchPreparationActionsPortV1 actions)
        {
            var root = Normalize(rootDirectoryPath, utcNow, profile, actions);
            return PrepareCore(root, utcNow, profile, actions);
        }

        // Test-only lock-span seam: injected ports let PlayMode assert the
        // production ownership boundary without becoming runtime entrypoints.
        internal static PreparedProfileLaunchV1 PrepareUnderRootLockForTest(string rootDirectoryPath, DateTimeOffset utcNow, IProfileLaunchPreparationProfilePortV1 profile, IProfileLaunchPreparationActionsPortV1 actions)
        {
            var root = Normalize(rootDirectoryPath, utcNow, profile, actions);
            using (ProfileRootOperationLockV1.Acquire(root))
            {
                ProfileResetBarrierV1.ThrowIfPresent(root);
                return PrepareCore(root, utcNow, profile, actions);
            }
        }

        private static PreparedProfileLaunchV1 PrepareCore(string root, DateTimeOffset utcNow, IProfileLaunchPreparationProfilePortV1 profile, IProfileLaunchPreparationActionsPortV1 actions)
        {
            var lease = Acquire(root); GameInputActions owned = null;
            try
            {
                lock (lease.Gate)
                {
                    var observation = profile.Observe(root); observation.Validate(); ProfileResetBarrierV1.ThrowIfPresent(root);
                    var selection = ProfileLoadSelectorV1.Select(observation.Primary, observation.Previous, observation.Temp); selection.Validate(); ProfileResetBarrierV1.ThrowIfPresent(root);
                    var preservation = profile.Preserve(root, observation, selection, utcNow); preservation.ValidateAgainst(selection); ProfileResetBarrierV1.ThrowIfPresent(root);
                    owned = actions.Create(); owned = RequireCandidate(owned); var initialBinding = Apply(actions, selection.InMemoryDocument.Snapshot.Input, owned); ValidateInitialBindingAgainstSelection(initialBinding, selection); var binding = initialBinding;
                    ProfileCanonicalDocumentV1 document = selection.InMemoryDocument; var reasons = selection.RecoveryReasons; var finalRequiresSave = selection.RequiresAtomicSave;
                    if (binding.Outcome == ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired)
                    {
                        RequireRecovery(binding, selection, observation); var poisoned = owned; owned = null; actions.Dispose(poisoned);
                        var source = selection.Source == ProfileLaunchSource.Primary ? observation.Primary : observation.Previous;
                        var repair = selection.Source == ProfileLaunchSource.Primary ? ProfileRecoveryPlannerV1.PlanPrimaryBindingApplyFailure(source.DecodeResult) : ProfileRecoveryPlannerV1.PlanPreviousBindingApplyFailure(source.DecodeResult);
                        repair.Validate(); document = repair.ResultDocument; reasons = repair.Reasons; finalRequiresSave = true;
                        owned = actions.Create(); owned = RequireCandidate(owned); if (ReferenceEquals(poisoned, owned)) { owned = null; throw new InvalidOperationException("Actions port reused a discarded candidate."); } var second = Apply(actions, document.Snapshot.Input, owned); if (second.Outcome != ProfileBindingOverrideApplyOutcomeV1.DefaultsReady || second.Failure != ProfileBindingOverrideApplyFailureV1.None || second.CandidateDisposition != ProfileBindingOverrideCandidateDispositionV1.Retain) throw new InvalidOperationException("Repaired actions candidate did not prove defaults."); binding = second;
                    }
                    else if (binding.Outcome != ProfileBindingOverrideApplyOutcomeV1.DefaultsReady && binding.Outcome != ProfileBindingOverrideApplyOutcomeV1.OverridesApplied) throw new InvalidOperationException("Binding result is malformed.");
                    ProfileLaunchSaveProjectionV1 save;
                    if (!finalRequiresSave) save = new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.None, ProfileLaunchPersistenceOutcomeV1.NotRequired, ProfileAtomicSaveStage.None, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, null);
                    else if (!preservation.MayPersist) save = new ProfileLaunchSaveProjectionV1(IsPrimaryDecodeRecovery(selection, observation) ? ProfileLaunchSaveKindV1.PrimaryInputRecovery : ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation, ProfileAtomicSaveStage.None, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, null);
                    else save = IsPrimaryDecodeRecovery(selection, observation) ? profile.SaveInputRecovery(root, observation.Primary) : profile.Save(root, document);
                    save.Validate(); ProfileResetBarrierV1.ThrowIfPresent(root); var token = new PreparedProfileLaunchV1(selection.Source, selection.SourceRevision, document, reasons, observation, initialBinding, binding, preservation, save, finalRequiresSave, owned, actions); owned = null; return token;
                }
            }
            catch (Exception original) { DisposeDuringFailure(actions, ref owned, original); throw; }
            finally { Release(root, lease); }
        }
        private static ProfileBindingOverrideApplyResultV1 Apply(IProfileLaunchPreparationActionsPortV1 port, ProfileInputSnapshot input, GameInputActions candidate) { var result = port.Apply(input, candidate); result.Validate(); return result; }
        private static void ValidateInitialBindingAgainstSelection(ProfileBindingOverrideApplyResultV1 result, ProfileLoadSelectionPlanV1 selection) { result.Validate(); if (!string.Equals(result.RequestedCanonicalText, selection.InMemoryDocument.Snapshot.Input.BindingOverrides.CanonicalText, StringComparison.Ordinal)) throw new InvalidOperationException("Binding result does not match selected input."); }
        private static void RequireRecovery(ProfileBindingOverrideApplyResultV1 result, ProfileLoadSelectionPlanV1 selection, ProfileLaunchObservationProjectionV1 observation)
        {
            if (result.CandidateDisposition != ProfileBindingOverrideCandidateDispositionV1.Discard || selection.Source == ProfileLaunchSource.Default || selection.RecoveryReasons == ProfileRecoveryPlanReason.InputRepair || selection.RecoveryReasons == (ProfileRecoveryPlanReason.PreviousPromotion | ProfileRecoveryPlanReason.InputRepair)) throw new InvalidOperationException("Binding recovery is inapplicable.");
            var source = selection.Source == ProfileLaunchSource.Primary ? observation.Primary : observation.Previous; if (source.Kind != ProfileLoadCandidateKind.Decoded || source.DecodeResult.Classification != ProfileCanonicalDecodeClassification.ValidCurrentInput || source.DecodeResult.Document.Snapshot.Input.BindingOverrides.IsEmpty || source.DecodeResult.Document.Snapshot.Progression.ProfileRevision != selection.SourceRevision) throw new InvalidOperationException("Binding recovery source is malformed.");
        }
        private static bool IsPrimaryDecodeRecovery(ProfileLoadSelectionPlanV1 selection, ProfileLaunchObservationProjectionV1 observation) { if (selection.Source != ProfileLaunchSource.Primary || !selection.RequiresAtomicSave) return false; var candidate = observation.Primary; if (candidate.Kind != ProfileLoadCandidateKind.Decoded) return false; var value = candidate.DecodeResult.Classification; return value == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired || value == ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired; }
        private static GameInputActions RequireCandidate(GameInputActions value) { if (value == null || value.asset == null || value.asset.enabled) throw new InvalidOperationException("Actions port returned an invalid candidate."); foreach (var map in value.asset.actionMaps) if (map.enabled) throw new InvalidOperationException("Actions port returned an enabled candidate."); return value; }
        // A recovery/token Dispose failure is the sole failure and propagates normally. Here an earlier failure is already propagating, so cleanup is attempted exactly once and its failure cannot replace that cause; an injected port can record both attempts deterministically.
        private static void DisposeDuringFailure(IProfileLaunchPreparationActionsPortV1 port, ref GameInputActions value, Exception original) { if (value == null) return; var candidate = value; value = null; try { port.Dispose(candidate); } catch (Exception) { if (original == null) throw; } }
        private static string Normalize(string root, DateTimeOffset utc, IProfileLaunchPreparationProfilePortV1 profile, IProfileLaunchPreparationActionsPortV1 actions) { if (profile == null) throw new ArgumentNullException(nameof(profile)); if (actions == null) throw new ArgumentNullException(nameof(actions)); return NormalizeRootAndUtc(root, utc); }
        private static string NormalizeRootAndUtc(string root, DateTimeOffset utc) { if (utc.Offset != TimeSpan.Zero) throw new ArgumentException("UTC must have zero offset.", nameof(utc)); if (string.IsNullOrWhiteSpace(root) || !Path.IsPathFullyQualified(root)) throw new ArgumentException("Root must be fully qualified.", nameof(root)); string normalized; try { normalized = Path.GetFullPath(root); } catch (Exception e) when (e is ArgumentException || e is NotSupportedException || e is PathTooLongException) { throw new ArgumentException("Root cannot be normalized.", nameof(root), e); } var pathRoot = Path.GetPathRoot(normalized); var end = normalized.Length; while (end > pathRoot.Length && (normalized[end - 1] == Path.DirectorySeparatorChar || normalized[end - 1] == Path.AltDirectorySeparatorChar)) end--; normalized = normalized.Substring(0, end); if (string.Equals(normalized, pathRoot, StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Root cannot be a filesystem root.", nameof(root)); return normalized; }
        private static LeaseEntry Acquire(string key) { lock (RegistryGate) { LeaseEntry entry; if (!Leases.TryGetValue(key, out entry)) { entry = new LeaseEntry(); Leases.Add(key, entry); } entry.RefCount++; return entry; } }
        private static void Release(string key, LeaseEntry entry) { lock (RegistryGate) { entry.RefCount--; if (entry.RefCount == 0) Leases.Remove(key); } }
        private sealed class LeaseEntry { internal readonly object Gate = new object(); internal int RefCount; }
    }
}
