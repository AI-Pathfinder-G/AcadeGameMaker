using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D7D-001..006. Engine-free, single-process atomic persistence adapter only.
    public enum ProfileAtomicSaveOutcome { CommittedFirst = 1, CommittedReplacement = 2, FailedBeforeCommit = 3, CommitOutcomeUncertain = 4 }
    public enum ProfileAtomicSaveStage { None = 0, DirectoryPreparation = 1, TempWriteFlushClose = 2, TempReadValidation = 3, ExistingPrimaryValidation = 4, CommitCall = 5, PostCommitValidation = 6 }
    public enum ProfileStoredFileState { NotProbed = 0, Missing = 1, ValidCurrentInput = 2, ValidInputRecoveryRequired = 3, UnsupportedSchema = 4, Invalid = 5, Unreadable = 6 }
    internal enum ProfileInputRecoverySaveOutcomeV1 { CommittedRecoveryReplacement = 1, FailedBeforeCommit = 2, CommitOutcomeUncertain = 3 }

    internal readonly struct ProfileInputRecoverySaveResultV1
    {
        private readonly ProfileInputRecoverySaveOutcomeV1 _outcome; private readonly ProfileAtomicSaveStage _stage; private readonly ProfileStoredFileState _primary; private readonly ProfileStoredFileState _previous; private readonly ProfileStoredFileState _temp; private readonly long _revision; private readonly long _expectedRevision;
        internal ProfileInputRecoverySaveResultV1(ProfileInputRecoverySaveOutcomeV1 outcome, ProfileAtomicSaveStage stage, ProfileStoredFileState primary, ProfileStoredFileState previous, ProfileStoredFileState temp, long revision, long expectedRevision) { _outcome = outcome; _stage = stage; _primary = primary; _previous = previous; _temp = temp; _revision = revision; _expectedRevision = expectedRevision; }
        internal ProfileInputRecoverySaveOutcomeV1 Outcome { get { Validate(); return _outcome; } } internal ProfileAtomicSaveStage FailureStage { get { Validate(); return _stage; } } internal ProfileStoredFileState PrimaryState { get { Validate(); return _primary; } } internal ProfileStoredFileState PreviousState { get { Validate(); return _previous; } } internal ProfileStoredFileState TempState { get { Validate(); return _temp; } } internal long CommittedRevision { get { Validate(); return _revision; } }
        internal void Validate()
        {
            if (_outcome < ProfileInputRecoverySaveOutcomeV1.CommittedRecoveryReplacement || _outcome > ProfileInputRecoverySaveOutcomeV1.CommitOutcomeUncertain || _stage < ProfileAtomicSaveStage.None || _stage > ProfileAtomicSaveStage.PostCommitValidation || !Known(_primary) || !Known(_previous) || !Known(_temp) || _primary == ProfileStoredFileState.NotProbed || _previous == ProfileStoredFileState.NotProbed || _temp == ProfileStoredFileState.NotProbed) throw new InvalidOperationException("Input recovery save result is malformed.");
            if (_outcome == ProfileInputRecoverySaveOutcomeV1.CommittedRecoveryReplacement) { if (_stage != ProfileAtomicSaveStage.None || _primary != ProfileStoredFileState.ValidCurrentInput || _previous != ProfileStoredFileState.ValidInputRecoveryRequired || _temp != ProfileStoredFileState.Missing || _revision < 0 || _expectedRevision < 0 || _revision != _expectedRevision) throw new InvalidOperationException("Input recovery success result is malformed."); return; }
            if (_outcome == ProfileInputRecoverySaveOutcomeV1.FailedBeforeCommit) { if (!(_stage == ProfileAtomicSaveStage.DirectoryPreparation || _stage == ProfileAtomicSaveStage.TempWriteFlushClose || _stage == ProfileAtomicSaveStage.TempReadValidation || _stage == ProfileAtomicSaveStage.ExistingPrimaryValidation) || _revision != -1 || _expectedRevision != -1) throw new InvalidOperationException("Input recovery pre-commit result is malformed."); return; }
            if (!(_stage == ProfileAtomicSaveStage.CommitCall || _stage == ProfileAtomicSaveStage.PostCommitValidation) || _revision != -1 || _expectedRevision != -1) throw new InvalidOperationException("Input recovery uncertain result is malformed.");
        }
        private static bool Known(ProfileStoredFileState value) => value >= ProfileStoredFileState.NotProbed && value <= ProfileStoredFileState.Unreadable;
    }

    public readonly struct ProfileAtomicSaveResultV1
    {
        private readonly ProfileAtomicSaveOutcome _outcome;
        private readonly ProfileAtomicSaveStage _failureStage;
        private readonly ProfileStoredFileState _primaryState;
        private readonly ProfileStoredFileState _previousState;
        private readonly ProfileStoredFileState _tempState;
        private readonly long _committedRevision;

        internal ProfileAtomicSaveResultV1(ProfileAtomicSaveOutcome outcome, ProfileAtomicSaveStage failureStage,
            ProfileStoredFileState primaryState, ProfileStoredFileState previousState, ProfileStoredFileState tempState,
            long committedRevision)
        {
            _outcome = outcome; _failureStage = failureStage; _primaryState = primaryState;
            _previousState = previousState; _tempState = tempState; _committedRevision = committedRevision;
        }

        public ProfileAtomicSaveOutcome Outcome { get { Validate(); return _outcome; } }
        public ProfileAtomicSaveStage FailureStage { get { Validate(); return _failureStage; } }
        public ProfileStoredFileState PrimaryState { get { Validate(); return _primaryState; } }
        public ProfileStoredFileState PreviousState { get { Validate(); return _previousState; } }
        public ProfileStoredFileState TempState { get { Validate(); return _tempState; } }
        public long CommittedRevision { get { Validate(); return _committedRevision; } }

        // REQ-M5D7D-005. This closed matrix makes default and reflection-bypassed results fail closed.
        public void Validate()
        {
            if (!KnownOutcome(_outcome) || !KnownStage(_failureStage) || !KnownState(_primaryState)
                || !KnownState(_previousState) || !KnownState(_tempState))
                throw new InvalidOperationException("Atomic save result is malformed.");

            if (_outcome == ProfileAtomicSaveOutcome.CommittedFirst)
            {
                if (_failureStage != ProfileAtomicSaveStage.None || _primaryState != ProfileStoredFileState.ValidCurrentInput
                    || _tempState != ProfileStoredFileState.Missing || _committedRevision < 0
                    || _previousState == ProfileStoredFileState.NotProbed || _previousState == ProfileStoredFileState.Unreadable)
                    throw new InvalidOperationException("Atomic first-save result is malformed.");
                return;
            }
            if (_outcome == ProfileAtomicSaveOutcome.CommittedReplacement)
            {
                if (_failureStage != ProfileAtomicSaveStage.None || _primaryState != ProfileStoredFileState.ValidCurrentInput
                    || _previousState != ProfileStoredFileState.ValidCurrentInput || _tempState != ProfileStoredFileState.Missing
                    || _committedRevision < 0)
                    throw new InvalidOperationException("Atomic replacement result is malformed.");
                return;
            }
            if (_outcome == ProfileAtomicSaveOutcome.FailedBeforeCommit)
            {
                if (!PreCommitStage(_failureStage) || _committedRevision != -1)
                    throw new InvalidOperationException("Atomic pre-commit failure result is malformed.");
                return;
            }
            if (!PostCommitStage(_failureStage) || _committedRevision != -1
                || _primaryState == ProfileStoredFileState.NotProbed || _previousState == ProfileStoredFileState.NotProbed
                || _tempState == ProfileStoredFileState.NotProbed)
                throw new InvalidOperationException("Atomic uncertain-commit result is malformed.");
        }

        private static bool KnownOutcome(ProfileAtomicSaveOutcome value) => value >= ProfileAtomicSaveOutcome.CommittedFirst && value <= ProfileAtomicSaveOutcome.CommitOutcomeUncertain;
        private static bool KnownStage(ProfileAtomicSaveStage value) => value >= ProfileAtomicSaveStage.None && value <= ProfileAtomicSaveStage.PostCommitValidation;
        private static bool KnownState(ProfileStoredFileState value) => value >= ProfileStoredFileState.NotProbed && value <= ProfileStoredFileState.Unreadable;
        private static bool PreCommitStage(ProfileAtomicSaveStage value) => value == ProfileAtomicSaveStage.DirectoryPreparation || value == ProfileAtomicSaveStage.TempWriteFlushClose || value == ProfileAtomicSaveStage.TempReadValidation || value == ProfileAtomicSaveStage.ExistingPrimaryValidation;
        private static bool PostCommitStage(ProfileAtomicSaveStage value) => value == ProfileAtomicSaveStage.CommitCall || value == ProfileAtomicSaveStage.PostCommitValidation;
    }

    // Internal deterministic fault seam. It is intentionally absent from the gameplay API.
    internal enum ProfileAtomicSaveFilePresence { Missing = 1, Present = 2 }
    internal interface IProfileAtomicSaveFileOperations
    {
        void EnsureDirectory(string path);
        IProfileAtomicSaveTempWriter OpenTempExclusive(string path);
        ProfileAtomicSaveFilePresence ProbePresence(string path);
        byte[] ReadAllExclusive(string path);
        void Replace(string tempPath, string primaryPath, string previousPath);
        void Move(string tempPath, string primaryPath);
    }

    internal interface IProfileAtomicSaveTempWriter
    {
        void WriteAll(byte[] bytes);
        void FlushToStorage();
        void Close();
    }

    internal sealed class ProfileAtomicSaveBclFileOperations : IProfileAtomicSaveFileOperations
    {
        internal static readonly ProfileAtomicSaveBclFileOperations Instance = new ProfileAtomicSaveBclFileOperations();
        private ProfileAtomicSaveBclFileOperations() { }
        public void EnsureDirectory(string path) { Directory.CreateDirectory(path); }
        public IProfileAtomicSaveTempWriter OpenTempExclusive(string path) => new BclTempWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough));
        // Boolean presence APIs can collapse access failures into false; this adapter calls a file Missing only when the filesystem says not found.
        public ProfileAtomicSaveFilePresence ProbePresence(string path)
        {
            try { File.GetAttributes(path); return ProfileAtomicSaveFilePresence.Present; }
            catch (FileNotFoundException) { return ProfileAtomicSaveFilePresence.Missing; }
            catch (DirectoryNotFoundException) { return ProfileAtomicSaveFilePresence.Missing; }
        }
        public byte[] ReadAllExclusive(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                if (stream.Length > int.MaxValue) throw new IOException("Profile file is too large.");
                var result = new byte[(int)stream.Length];
                var offset = 0;
                while (offset < result.Length)
                {
                    var read = stream.Read(result, offset, result.Length - offset);
                    if (read == 0) throw new EndOfStreamException("Profile file changed during exclusive read.");
                    offset += read;
                }
                return result;
            }
        }
        public void Replace(string tempPath, string primaryPath, string previousPath) { File.Replace(tempPath, primaryPath, previousPath, true); }
        public void Move(string tempPath, string primaryPath) { File.Move(tempPath, primaryPath); }
        private sealed class BclTempWriter : IProfileAtomicSaveTempWriter
        {
            private FileStream _stream;
            internal BclTempWriter(FileStream stream) { _stream = stream; }
            public void WriteAll(byte[] bytes) { _stream.Write(bytes, 0, bytes.Length); }
            public void FlushToStorage() { _stream.Flush(true); }
            public void Close() { if (_stream != null) { _stream.Dispose(); _stream = null; } }
        }
    }

    public static class ProfileAtomicSaveServiceV1
    {
        private const string PrimaryName = "profile.json";
        private const string PreviousName = "profile.prev.json";
        private const string TempName = "profile.tmp.json";
        private static readonly object RegistryGate = new object();
        private static readonly Dictionary<string, LeaseEntry> Leases = new Dictionary<string, LeaseEntry>(StringComparer.OrdinalIgnoreCase);

        public static ProfileAtomicSaveResultV1 Save(string directoryPath, ProfileCanonicalDocumentV1 document)
        {
            // REQ-M5D7QC-007. The public writer is always behind the durable
            // reset barrier and the cross-process root owner.
            var normalized = ValidateArguments(directoryPath, document, ProfileAtomicSaveBclFileOperations.Instance);
            using (ProfileRootOperationLockV1.Acquire(normalized))
            {
                ProfileResetBarrierV1.ThrowIfPresent(normalized);
                return Save(normalized, document, ProfileAtomicSaveBclFileOperations.Instance);
            }
        }

        // REQ-M5D7QC-007. Called only by a coordinator that already owns the
        // root operation lock; reacquiring FileShare.None would deadlock.
        internal static ProfileAtomicSaveResultV1 SaveUnderHeldRoot(ProfileRootOperationLockV1 ownership, string directoryPath, ProfileCanonicalDocumentV1 document)
        {
            if (ownership == null) throw new ArgumentNullException(nameof(ownership));
            ownership.ValidateOwnership(directoryPath);
            var normalized = ValidateArguments(directoryPath, document, ProfileAtomicSaveBclFileOperations.Instance);
            ProfileResetBarrierV1.ThrowIfPresent(normalized);
            return SaveHeld(normalized, document, ProfileAtomicSaveBclFileOperations.Instance);
        }

        // REQ-M5D7QC1-004/005. This is deliberately the sole barrier-present
        // writer. The marker object has no public constructor and is minted by
        // C1 only after canonical marker validation under this exact lease.
        internal static ProfileAtomicSaveResultV1 SaveResetFirstUnderHeldRoot(ProfileRootOperationLockV1 ownership, ValidatedProfileResetBarrierV1 barrier, string directoryPath, ProfileCanonicalDocumentV1 document)
        {
            return SaveResetFirstUnderHeldRoot(ownership, barrier, directoryPath, document, ProfileAtomicSaveBclFileOperations.Instance);
        }

        // REQ-M5D7QC1-004/005. Internal deterministic fault seam for the
        // reset-only capability. It deliberately retains the same capability
        // validation and M5D7D SaveHeld core as the production overload.
        internal static ProfileAtomicSaveResultV1 SaveResetFirstUnderHeldRoot(ProfileRootOperationLockV1 ownership, ValidatedProfileResetBarrierV1 barrier, string directoryPath, ProfileCanonicalDocumentV1 document, IProfileAtomicSaveFileOperations operations)
        {
            if (ownership == null || barrier == null) throw new ArgumentNullException(ownership == null ? nameof(ownership) : nameof(barrier));
            ownership.ValidateOwnership(directoryPath);
            var normalized = ValidateArguments(directoryPath, document, operations);
            ProfileResetDiskTransactionV1.ValidateResetFirstCapability(ownership, barrier, normalized, document);
            // SaveHeld's FileMode.Create is safe only after C1 has proved all
            // three active leaves absent; it remains the single M5D7D core.
            return SaveHeld(normalized, document, operations);
        }

        internal static ProfileAtomicSaveResultV1 Save(string directoryPath, ProfileCanonicalDocumentV1 document, IProfileAtomicSaveFileOperations operations)
        {
            var normalized = ValidateArguments(directoryPath, document, operations);
            var lease = Acquire(normalized);
            try
            {
                lock (lease.Gate)
                {
                    return SaveHeld(normalized, document, operations);
                }
            }
            finally { Release(normalized, lease); }
        }

        // REQ-M5D7H-001..007. Internal-only recovery replacement shares this exact M5D7D lease and port.
        internal static ProfileInputRecoverySaveResultV1 SaveInputRecovery(string directoryPath, ProfileLoadCandidateV1 observedPrimary)
        {
            var normalized = ValidateRecoveryArguments(directoryPath, observedPrimary, ProfileAtomicSaveBclFileOperations.Instance);
            using (ProfileRootOperationLockV1.Acquire(normalized))
            {
                ProfileResetBarrierV1.ThrowIfPresent(normalized);
                return SaveInputRecovery(normalized, observedPrimary, ProfileAtomicSaveBclFileOperations.Instance);
            }
        }

        // REQ-M5D7QC-007. See SaveUnderHeldRoot: this is the matching
        // held-root recovery entry point for ordinary launch preparation.
        internal static ProfileInputRecoverySaveResultV1 SaveInputRecoveryUnderHeldRoot(ProfileRootOperationLockV1 ownership, string directoryPath, ProfileLoadCandidateV1 observedPrimary)
        {
            if (ownership == null) throw new ArgumentNullException(nameof(ownership));
            ownership.ValidateOwnership(directoryPath);
            var normalized = ValidateRecoveryArguments(directoryPath, observedPrimary, ProfileAtomicSaveBclFileOperations.Instance);
            ProfileResetBarrierV1.ThrowIfPresent(normalized);
            var plan = ProfileRecoveryPlannerV1.PlanInputRepair(observedPrimary.DecodeResult);
            return SaveRecoveryHeld(normalized, observedPrimary.DecodeResult, plan, ProfileAtomicSaveBclFileOperations.Instance);
        }
        // Test-only injected filesystem seam. Runtime entrypoints use the guarded
        // two-argument overload above; do not route production callers here.
        internal static ProfileInputRecoverySaveResultV1 SaveInputRecovery(string directoryPath, ProfileLoadCandidateV1 observedPrimary, IProfileAtomicSaveFileOperations operations)
        {
            var normalized = ValidateRecoveryArguments(directoryPath, observedPrimary, operations); var originalPlan = ProfileRecoveryPlannerV1.PlanInputRepair(observedPrimary.DecodeResult); var lease = Acquire(normalized);
            try { lock (lease.Gate) { return SaveRecoveryHeld(normalized, observedPrimary.DecodeResult, originalPlan, operations); } }
            finally { Release(normalized, lease); }
        }
        private static ProfileInputRecoverySaveResultV1 SaveRecoveryHeld(string directory, ProfileCanonicalDecodeResultV1 observed, ProfileRecoveryPlanV1 plan, IProfileAtomicSaveFileOperations operations)
        {
            var primary = Path.Combine(directory, PrimaryName); var previous = Path.Combine(directory, PreviousName); var temp = Path.Combine(directory, TempName); var repair = plan.ResultDocument;
            try { operations.EnsureDirectory(directory); } catch (Exception e) when (IsRecoverableFileException(e)) { return RecoveryFailed(ProfileAtomicSaveStage.DirectoryPreparation, primary, previous, temp, operations); }
            try { var writer = operations.OpenTempExclusive(temp); try { writer.WriteAll(repair.CanonicalFileBytes); writer.FlushToStorage(); } finally { writer.Close(); } }
            catch (Exception e) when (IsRecoverableFileException(e)) { return RecoveryFailed(ProfileAtomicSaveStage.TempWriteFlushClose, primary, previous, temp, operations); }
            try { if (!MatchesCurrent(operations.ReadAllExclusive(temp), repair)) return RecoveryFailed(ProfileAtomicSaveStage.TempReadValidation, primary, previous, temp, operations); }
            catch (Exception e) when (IsRecoverableFileException(e)) { return RecoveryFailed(ProfileAtomicSaveStage.TempReadValidation, primary, previous, temp, operations); }
            byte[] oldPrimary;
            try
            {
                if (RequirePresence(operations.ProbePresence(primary)) != ProfileAtomicSaveFilePresence.Present) return RecoveryFailed(ProfileAtomicSaveStage.ExistingPrimaryValidation, primary, previous, temp, operations);
                oldPrimary = operations.ReadAllExclusive(primary); var fresh = ProfileCanonicalDecoderV1.Decode(oldPrimary);
                if (!RecoveryIdentity(observed, plan, fresh)) return RecoveryFailed(ProfileAtomicSaveStage.ExistingPrimaryValidation, primary, previous, temp, operations);
            }
            catch (Exception e) when (IsRecoverableFileException(e)) { return RecoveryFailed(ProfileAtomicSaveStage.ExistingPrimaryValidation, primary, previous, temp, operations); }
            try { operations.Replace(temp, primary, previous); }
            catch (Exception e) when (IsRecoverableFileException(e)) { return RecoveryUncertain(ProfileAtomicSaveStage.CommitCall, primary, previous, temp, operations); }
            try
            {
                if (!MatchesCurrent(operations.ReadAllExclusive(primary), repair) || !BytesEqual(operations.ReadAllExclusive(previous), oldPrimary)) return RecoveryUncertain(ProfileAtomicSaveStage.PostCommitValidation, primary, previous, temp, operations);
                var primaryState = Probe(primary, operations); var previousState = Probe(previous, operations); var tempState = Probe(temp, operations);
                if (primaryState != ProfileStoredFileState.ValidCurrentInput || previousState != ProfileStoredFileState.ValidInputRecoveryRequired || tempState != ProfileStoredFileState.Missing) return RecoveryUncertain(ProfileAtomicSaveStage.PostCommitValidation, primary, previous, temp, operations);
                return new ProfileInputRecoverySaveResultV1(ProfileInputRecoverySaveOutcomeV1.CommittedRecoveryReplacement, ProfileAtomicSaveStage.None, primaryState, previousState, tempState, plan.ResultRevision, plan.ResultRevision);
            }
            catch (Exception e) when (IsRecoverableFileException(e)) { return RecoveryUncertain(ProfileAtomicSaveStage.PostCommitValidation, primary, previous, temp, operations); }
        }
        private static bool RecoveryIdentity(ProfileCanonicalDecodeResultV1 observed, ProfileRecoveryPlanV1 original, ProfileCanonicalDecodeResultV1 fresh)
        {
            var classification = fresh.Classification; if (classification != observed.Classification || (classification != ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired && classification != ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired)) return false;
            if (fresh.InputRecoveryReasons != observed.InputRecoveryReasons) return false;
            if (classification == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired && fresh.InputCompatibility != observed.InputCompatibility) return false;
            ProfileRecoveryPlanV1 current; try { current = ProfileRecoveryPlannerV1.PlanInputRepair(fresh); } catch (ArgumentException) { return false; } catch (InvalidOperationException) { return false; }
            return current.SourceRevision == original.SourceRevision && current.ResultRevision == original.ResultRevision && current.Reasons == original.Reasons && BytesEqual(current.ResultDocument.CanonicalFileBytes, original.ResultDocument.CanonicalFileBytes);
        }
        private static ProfileInputRecoverySaveResultV1 RecoveryFailed(ProfileAtomicSaveStage stage, string primary, string previous, string temp, IProfileAtomicSaveFileOperations operations) => new ProfileInputRecoverySaveResultV1(ProfileInputRecoverySaveOutcomeV1.FailedBeforeCommit, stage, Probe(primary, operations), Probe(previous, operations), Probe(temp, operations), -1, -1);
        private static ProfileInputRecoverySaveResultV1 RecoveryUncertain(ProfileAtomicSaveStage stage, string primary, string previous, string temp, IProfileAtomicSaveFileOperations operations) => new ProfileInputRecoverySaveResultV1(ProfileInputRecoverySaveOutcomeV1.CommitOutcomeUncertain, stage, Probe(primary, operations), Probe(previous, operations), Probe(temp, operations), -1, -1);

        private static ProfileAtomicSaveResultV1 SaveHeld(string directory, ProfileCanonicalDocumentV1 document, IProfileAtomicSaveFileOperations operations)
        {
            var primary = Path.Combine(directory, PrimaryName);
            var previous = Path.Combine(directory, PreviousName);
            var temp = Path.Combine(directory, TempName);
            try { operations.EnsureDirectory(directory); }
            catch (Exception exception) when (IsRecoverableFileException(exception)) { return Failed(ProfileAtomicSaveStage.DirectoryPreparation, primary, previous, temp, operations); }

            var expectedBytes = document.CanonicalFileBytes;
            try
            {
                var writer = operations.OpenTempExclusive(temp);
                try { writer.WriteAll(expectedBytes); writer.FlushToStorage(); }
                finally { writer.Close(); }
            }
            catch (Exception exception) when (IsRecoverableFileException(exception)) { return Failed(ProfileAtomicSaveStage.TempWriteFlushClose, primary, previous, temp, operations); }

            try
            {
                if (!MatchesCurrent(operations.ReadAllExclusive(temp), document))
                    return Failed(ProfileAtomicSaveStage.TempReadValidation, primary, previous, temp, operations);
            }
            catch (Exception exception) when (IsRecoverableFileException(exception)) { return Failed(ProfileAtomicSaveStage.TempReadValidation, primary, previous, temp, operations); }

            bool replacement;
            byte[] oldPrimary = null;
            try
            {
                replacement = RequirePresence(operations.ProbePresence(primary)) == ProfileAtomicSaveFilePresence.Present;
                if (replacement)
                {
                    oldPrimary = operations.ReadAllExclusive(primary);
                    if (!IsCurrent(oldPrimary)) return Failed(ProfileAtomicSaveStage.ExistingPrimaryValidation, primary, previous, temp, operations);
                }
            }
            catch (Exception exception) when (IsRecoverableFileException(exception)) { return Failed(ProfileAtomicSaveStage.ExistingPrimaryValidation, primary, previous, temp, operations); }

            try
            {
                if (replacement) operations.Replace(temp, primary, previous);
                else operations.Move(temp, primary);
            }
            catch (Exception exception) when (IsRecoverableFileException(exception)) { return Uncertain(ProfileAtomicSaveStage.CommitCall, primary, previous, temp, operations); }

            try
            {
                if (!MatchesCurrent(operations.ReadAllExclusive(primary), document))
                    return Uncertain(ProfileAtomicSaveStage.PostCommitValidation, primary, previous, temp, operations);
                var primaryState = Probe(primary, operations);
                var previousState = Probe(previous, operations);
                var tempState = Probe(temp, operations);
                if (primaryState != ProfileStoredFileState.ValidCurrentInput || tempState != ProfileStoredFileState.Missing)
                    return Uncertain(ProfileAtomicSaveStage.PostCommitValidation, primary, previous, temp, operations);
                if (replacement)
                {
                    if (previousState != ProfileStoredFileState.ValidCurrentInput || !BytesEqual(operations.ReadAllExclusive(previous), oldPrimary))
                        return Uncertain(ProfileAtomicSaveStage.PostCommitValidation, primary, previous, temp, operations);
                    return new ProfileAtomicSaveResultV1(ProfileAtomicSaveOutcome.CommittedReplacement, ProfileAtomicSaveStage.None, primaryState, previousState, tempState, document.Snapshot.ProfileRevision);
                }
                if (previousState == ProfileStoredFileState.NotProbed || previousState == ProfileStoredFileState.Unreadable)
                    return Uncertain(ProfileAtomicSaveStage.PostCommitValidation, primary, previous, temp, operations);
                return new ProfileAtomicSaveResultV1(ProfileAtomicSaveOutcome.CommittedFirst, ProfileAtomicSaveStage.None, primaryState, previousState, tempState, document.Snapshot.ProfileRevision);
            }
            catch (Exception exception) when (IsRecoverableFileException(exception)) { return Uncertain(ProfileAtomicSaveStage.PostCommitValidation, primary, previous, temp, operations); }
        }

        private static ProfileAtomicSaveResultV1 Failed(ProfileAtomicSaveStage stage, string primary, string previous, string temp, IProfileAtomicSaveFileOperations operations)
        {
            return new ProfileAtomicSaveResultV1(ProfileAtomicSaveOutcome.FailedBeforeCommit, stage, Probe(primary, operations), Probe(previous, operations), Probe(temp, operations), -1);
        }
        private static ProfileAtomicSaveResultV1 Uncertain(ProfileAtomicSaveStage stage, string primary, string previous, string temp, IProfileAtomicSaveFileOperations operations)
        {
            return new ProfileAtomicSaveResultV1(ProfileAtomicSaveOutcome.CommitOutcomeUncertain, stage, Probe(primary, operations), Probe(previous, operations), Probe(temp, operations), -1);
        }
        private static ProfileStoredFileState Probe(string path, IProfileAtomicSaveFileOperations operations)
        {
            try
            {
                if (RequirePresence(operations.ProbePresence(path)) == ProfileAtomicSaveFilePresence.Missing) return ProfileStoredFileState.Missing;
                var result = ProfileCanonicalDecoderV1.Decode(operations.ReadAllExclusive(path));
                if (result.Classification == ProfileCanonicalDecodeClassification.ValidCurrentInput) return ProfileStoredFileState.ValidCurrentInput;
                if (result.Classification == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired || result.Classification == ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired) return ProfileStoredFileState.ValidInputRecoveryRequired;
                if (result.Classification == ProfileCanonicalDecodeClassification.UnsupportedProfileSchema) return ProfileStoredFileState.UnsupportedSchema;
                return ProfileStoredFileState.Invalid;
            }
            catch (Exception exception) when (IsRecoverableFileException(exception)) { return ProfileStoredFileState.Unreadable; }
        }
        private static bool MatchesCurrent(byte[] bytes, ProfileCanonicalDocumentV1 expected)
        {
            var decoded = ProfileCanonicalDecoderV1.Decode(bytes);
            if (decoded.Classification != ProfileCanonicalDecodeClassification.ValidCurrentInput) return false;
            var actual = decoded.Document;
            return actual.Snapshot.ProfileRevision == expected.Snapshot.ProfileRevision && BytesEqual(actual.CanonicalFileBytes, expected.CanonicalFileBytes);
        }
        private static bool IsCurrent(byte[] bytes) => ProfileCanonicalDecoderV1.Decode(bytes).Classification == ProfileCanonicalDecodeClassification.ValidCurrentInput;
        private static bool IsRecoverableFileException(Exception exception) => exception is IOException || exception is UnauthorizedAccessException || exception is SecurityException;
        private static ProfileAtomicSaveFilePresence RequirePresence(ProfileAtomicSaveFilePresence value)
        {
            if (value != ProfileAtomicSaveFilePresence.Missing && value != ProfileAtomicSaveFilePresence.Present)
                throw new InvalidOperationException("Profile file presence result is malformed.");
            return value;
        }
        private static bool BytesEqual(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            for (var index = 0; index < left.Length; index++) if (left[index] != right[index]) return false;
            return true;
        }
        private static string ValidateRecoveryArguments(string directoryPath, ProfileLoadCandidateV1 observedPrimary, IProfileAtomicSaveFileOperations operations)
        {
            if (operations == null) throw new ArgumentNullException(nameof(operations));
            if (string.IsNullOrWhiteSpace(directoryPath) || !Path.IsPathFullyQualified(directoryPath)) throw new ArgumentException("Directory path must be fully qualified.", nameof(directoryPath));
            string normalized; try { normalized = TrimTrailingSeparators(Path.GetFullPath(directoryPath)); } catch (Exception exception) when (exception is ArgumentException || exception is NotSupportedException || exception is PathTooLongException) { throw new ArgumentException("Directory path cannot be normalized.", nameof(directoryPath), exception); }
            if (string.Equals(normalized, Path.GetPathRoot(normalized), StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Directory path cannot be a filesystem root.", nameof(directoryPath));
            try { observedPrimary.Validate(); } catch (InvalidOperationException exception) { throw new ArgumentException("Observed primary is malformed.", nameof(observedPrimary), exception); }
            if (observedPrimary.Role != ProfileLoadFileRole.Primary || observedPrimary.Kind != ProfileLoadCandidateKind.Decoded) throw new ArgumentException("Observed source is not a decoded primary.", nameof(observedPrimary));
            var classification = observedPrimary.DecodeResult.Classification;
            if (classification != ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired && classification != ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired) throw new ArgumentException("Observed primary does not require input recovery.", nameof(observedPrimary));
            return normalized;
        }
        private static string ValidateArguments(string directoryPath, ProfileCanonicalDocumentV1 document, IProfileAtomicSaveFileOperations operations)
        {
            if (operations == null) throw new ArgumentNullException(nameof(operations));
            if (string.IsNullOrWhiteSpace(directoryPath) || !Path.IsPathFullyQualified(directoryPath)) throw new ArgumentException("Directory path must be fully qualified.", nameof(directoryPath));
            string normalized;
            try { normalized = TrimTrailingSeparators(Path.GetFullPath(directoryPath)); }
            catch (Exception exception) when (exception is ArgumentException || exception is NotSupportedException || exception is PathTooLongException) { throw new ArgumentException("Directory path cannot be normalized.", nameof(directoryPath), exception); }
            if (string.Equals(normalized, Path.GetPathRoot(normalized), StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Directory path cannot be a filesystem root.", nameof(directoryPath));
            try { document.Validate(); }
            catch (InvalidOperationException exception) { throw new ArgumentException("Profile document is malformed.", nameof(document), exception); }
            if (!document.Snapshot.Input.IsCurrentCompatible) throw new ArgumentException("Profile document input is not current-compatible.", nameof(document));
            return normalized;
        }
        private static string TrimTrailingSeparators(string path)
        {
            var root = Path.GetPathRoot(path);
            var end = path.Length;
            while (end > root.Length && (path[end - 1] == Path.DirectorySeparatorChar || path[end - 1] == Path.AltDirectorySeparatorChar)) end--;
            return path.Substring(0, end);
        }
        private static LeaseEntry Acquire(string key)
        {
            lock (RegistryGate)
            {
                LeaseEntry entry;
                if (!Leases.TryGetValue(key, out entry)) { entry = new LeaseEntry(); Leases.Add(key, entry); }
                entry.RefCount++;
                return entry;
            }
        }
        private static void Release(string key, LeaseEntry entry)
        {
            lock (RegistryGate)
            {
                entry.RefCount--;
                if (entry.RefCount == 0) Leases.Remove(key);
            }
        }
        private sealed class LeaseEntry { internal readonly object Gate = new object(); internal int RefCount; }
    }
}
