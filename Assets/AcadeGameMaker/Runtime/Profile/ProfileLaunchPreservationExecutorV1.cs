using System;
using System.IO;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D7K-001..007. Preservation sequencing only; no observation, selection, save, or launch authority.
    public enum ProfileLaunchPreservationStateV1 { NotRequired = 1, Preserved = 2, SourceMissing = 3, FailedBeforeMove = 4, MoveOutcomeUncertain = 5, NotAttempted = 6 }

    public readonly struct ProfileLaunchPreservationResultV1
    {
        private readonly ProfileLaunchPreservationStateV1 _temp; private readonly ProfileLaunchPreservationStateV1 _primary; private readonly ProfileLaunchPreservationStateV1 _previous;
        private readonly bool _mayPersist; private readonly ProfileLoadFileRole? _stoppedRole; private readonly ProfileFilePreservationReason _tempIntent; private readonly ProfileFilePreservationReason _primaryIntent; private readonly ProfileFilePreservationReason _previousIntent; private readonly int _attemptedCount;
        internal ProfileLaunchPreservationResultV1(ProfileLaunchPreservationStateV1 temp, ProfileLaunchPreservationStateV1 primary, ProfileLaunchPreservationStateV1 previous, bool mayPersist, ProfileLoadFileRole? stoppedRole, ProfileFilePreservationReason tempIntent, ProfileFilePreservationReason primaryIntent, ProfileFilePreservationReason previousIntent, int attemptedCount)
        { _temp = temp; _primary = primary; _previous = previous; _mayPersist = mayPersist; _stoppedRole = stoppedRole; _tempIntent = tempIntent; _primaryIntent = primaryIntent; _previousIntent = previousIntent; _attemptedCount = attemptedCount; }
        public ProfileLaunchPreservationStateV1 TempState { get { Validate(); return _temp; } } public ProfileLaunchPreservationStateV1 PrimaryState { get { Validate(); return _primary; } } public ProfileLaunchPreservationStateV1 PreviousState { get { Validate(); return _previous; } } public bool MayPersist { get { Validate(); return _mayPersist; } } public ProfileLoadFileRole? StoppedRole { get { Validate(); return _stoppedRole; } }
        public void Validate()
        {
            if (!KnownIntent(_tempIntent, ProfileLoadFileRole.Temp) || !KnownIntent(_primaryIntent, ProfileLoadFileRole.Primary) || !KnownIntent(_previousIntent, ProfileLoadFileRole.Previous) || _attemptedCount < 0 || _attemptedCount > 3) throw new InvalidOperationException("Preservation result is malformed.");
            var states = new[] { _temp, _primary, _previous }; var intents = new[] { _tempIntent, _primaryIntent, _previousIntent }; var roles = new[] { ProfileLoadFileRole.Temp, ProfileLoadFileRole.Primary, ProfileLoadFileRole.Previous };
            var stopped = false; var attempted = 0; ProfileLoadFileRole? expectedStop = null;
            for (var index = 0; index < 3; index++)
            {
                var required = intents[index] != ProfileFilePreservationReason.None; var state = states[index];
                if (!required) { if (state != ProfileLaunchPreservationStateV1.NotRequired) throw new InvalidOperationException("Non-required preservation state is malformed."); continue; }
                if (stopped) { if (state != ProfileLaunchPreservationStateV1.NotAttempted) throw new InvalidOperationException("Unattempted preservation state is malformed."); continue; }
                attempted++;
                if (state == ProfileLaunchPreservationStateV1.Preserved || state == ProfileLaunchPreservationStateV1.SourceMissing) continue;
                if (state != ProfileLaunchPreservationStateV1.FailedBeforeMove && state != ProfileLaunchPreservationStateV1.MoveOutcomeUncertain) throw new InvalidOperationException("Required preservation state is malformed.");
                stopped = true; expectedStop = roles[index];
            }
            if (_attemptedCount != attempted || _stoppedRole != expectedStop || _mayPersist != (!stopped && attempted == RequiredCount(intents))) throw new InvalidOperationException("Preservation result proof is malformed.");
        }
        private static int RequiredCount(ProfileFilePreservationReason[] values) { var count = 0; for (var i = 0; i < values.Length; i++) if (values[i] != ProfileFilePreservationReason.None) count++; return count; }
        private static bool KnownIntent(ProfileFilePreservationReason reason, ProfileLoadFileRole role) => reason == ProfileFilePreservationReason.None || (role == ProfileLoadFileRole.Temp && reason == ProfileFilePreservationReason.StaleTemp) || (role == ProfileLoadFileRole.Primary && (reason == ProfileFilePreservationReason.InvalidPrimary || reason == ProfileFilePreservationReason.UnsupportedPrimary || reason == ProfileFilePreservationReason.UnreadablePrimary)) || (role == ProfileLoadFileRole.Previous && (reason == ProfileFilePreservationReason.InvalidPrevious || reason == ProfileFilePreservationReason.UnsupportedPrevious || reason == ProfileFilePreservationReason.UnreadablePrevious));
    }

    internal interface IProfileLaunchPreservationPort { ProfileQuarantineResultV1 Preserve(string root, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, DateTimeOffset utc); }
    internal sealed class ProfileLaunchPreservationBclPort : IProfileLaunchPreservationPort
    {
        internal static readonly ProfileLaunchPreservationBclPort Instance = new ProfileLaunchPreservationBclPort(); private ProfileLaunchPreservationBclPort() { }
        public ProfileQuarantineResultV1 Preserve(string root, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, DateTimeOffset utc) { return ProfileQuarantineServiceV1.Preserve(root, candidate, reason, utc); }
    }

    public static class ProfileLaunchPreservationExecutorV1
    {
        public static ProfileLaunchPreservationResultV1 Execute(string rootDirectoryPath, ProfileLaunchObservationBatchV1 observation, ProfileLoadSelectionPlanV1 selection, DateTimeOffset utcNow) => Execute(rootDirectoryPath, observation, selection, utcNow, ProfileLaunchPreservationBclPort.Instance);
        internal static ProfileLaunchPreservationResultV1 Execute(string rootDirectoryPath, ProfileLaunchObservationBatchV1 observation, ProfileLoadSelectionPlanV1 selection, DateTimeOffset utcNow, IProfileLaunchPreservationPort port)
        {
            var root = Validate(rootDirectoryPath, observation, selection, utcNow, port);
            var intents = new[] { selection.TempPreservation, selection.PrimaryPreservation, selection.PreviousPreservation }; var candidates = new[] { observation.Temp, observation.Primary, observation.Previous }; var states = new[] { ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired };
            var attempted = 0; ProfileLoadFileRole? stopped = null;
            for (var i = 0; i < 3; i++)
            {
                if (intents[i] == ProfileFilePreservationReason.None) continue;
                if (stopped.HasValue) { states[i] = ProfileLaunchPreservationStateV1.NotAttempted; continue; }
                attempted++; var result = port.Preserve(root, candidates[i], intents[i], utcNow); result.Validate();
                if (result.SourceRole != candidates[i].Role || result.Reason != intents[i]) throw new InvalidOperationException("Quarantine port result does not match its request.");
                if (result.Outcome == ProfileQuarantineOutcome.Preserved) states[i] = ProfileLaunchPreservationStateV1.Preserved;
                else if (result.Outcome == ProfileQuarantineOutcome.SourceMissing) states[i] = ProfileLaunchPreservationStateV1.SourceMissing;
                else if (result.Outcome == ProfileQuarantineOutcome.FailedBeforeMove) { states[i] = ProfileLaunchPreservationStateV1.FailedBeforeMove; stopped = candidates[i].Role; }
                else if (result.Outcome == ProfileQuarantineOutcome.MoveOutcomeUncertain) { states[i] = ProfileLaunchPreservationStateV1.MoveOutcomeUncertain; stopped = candidates[i].Role; }
                else throw new InvalidOperationException("Quarantine port outcome is malformed.");
            }
            var mayPersist = !stopped.HasValue;
            return new ProfileLaunchPreservationResultV1(states[0], states[1], states[2], mayPersist, stopped, intents[0], intents[1], intents[2], attempted);
        }
        private static string Validate(string root, ProfileLaunchObservationBatchV1 observation, ProfileLoadSelectionPlanV1 selection, DateTimeOffset utc, IProfileLaunchPreservationPort port)
        {
            if (port == null) throw new ArgumentNullException(nameof(port)); if (string.IsNullOrWhiteSpace(root) || !Path.IsPathFullyQualified(root) || utc.Offset != TimeSpan.Zero) throw new ArgumentException("Preservation arguments are malformed.");
            string normalized; try { normalized = Trim(Path.GetFullPath(root)); } catch (Exception e) when (e is ArgumentException || e is NotSupportedException || e is PathTooLongException) { throw new ArgumentException("Root path is malformed.", nameof(root), e); }
            if (string.Equals(normalized, Path.GetPathRoot(normalized), StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Root cannot be a filesystem root.", nameof(root));
            try { observation.Validate(); selection.Validate(); } catch (InvalidOperationException e) { throw new ArgumentException("Observation or selection is malformed.", e); }
            var expected = ProfileLoadSelectorV1.Select(observation.Primary, observation.Previous, observation.Temp);
            if (!Same(expected, selection)) throw new ArgumentException("Selection does not match observation.", nameof(selection)); return normalized;
        }
        private static bool Same(ProfileLoadSelectionPlanV1 a, ProfileLoadSelectionPlanV1 b) => a.Source == b.Source && a.SourceRevision == b.SourceRevision && a.ResultRevision == b.ResultRevision && a.RequiresAtomicSave == b.RequiresAtomicSave && a.RecoveryReasons == b.RecoveryReasons && a.PrimaryPreservation == b.PrimaryPreservation && a.PreviousPreservation == b.PreviousPreservation && a.TempPreservation == b.TempPreservation && Bytes(a.InMemoryDocument.CanonicalFileBytes, b.InMemoryDocument.CanonicalFileBytes);
        private static bool Bytes(byte[] a, byte[] b) { if (a == null || b == null || a.Length != b.Length) return false; for (var i = 0; i < a.Length; i++) if (a[i] != b[i]) return false; return true; }
        private static string Trim(string path) { var root = Path.GetPathRoot(path); var end = path.Length; while (end > root.Length && (path[end - 1] == Path.DirectorySeparatorChar || path[end - 1] == Path.AltDirectorySeparatorChar)) end--; return path.Substring(0, end); }
    }
}
