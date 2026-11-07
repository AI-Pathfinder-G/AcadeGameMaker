using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D7F-001..005. Typed BCL quarantine execution only; no selection, transformation, save, clock, or Unity authority.
    public enum ProfileQuarantineOutcome { Preserved = 1, SourceMissing = 2, FailedBeforeMove = 3, MoveOutcomeUncertain = 4 }
    public enum ProfileQuarantineStage { None = 0, SourceRevalidation = 1, RecoveryDirectoryPreparation = 2, DestinationAllocation = 3, MoveCall = 4, PostMoveValidation = 5 }
    public enum ProfileQuarantineFileState { NotProbed = 0, Missing = 1, Present = 2, Unreadable = 3 }
    internal enum ProfileQuarantinePresence { Missing = 1, Present = 2 }

    public readonly struct ProfileQuarantineResultV1
    {
        private readonly ProfileQuarantineOutcome _outcome; private readonly ProfileQuarantineStage _stage; private readonly ProfileLoadFileRole _role; private readonly ProfileFilePreservationReason _reason;
        private readonly ProfileQuarantineFileState _sourceState; private readonly ProfileQuarantineFileState _destinationState; private readonly string _destinationPath; private readonly string _hashSuffix; private readonly int _collisionOrdinal;
        // Destination proof is deliberately private: getters re-derive the full identity from it.
        private readonly string _root; private readonly string _utcToken; private readonly int? _unsupportedVersion; private readonly bool _hashWasProven;
        internal ProfileQuarantineResultV1(ProfileQuarantineOutcome outcome, ProfileQuarantineStage stage, ProfileLoadFileRole role, ProfileFilePreservationReason reason, ProfileQuarantineFileState sourceState, ProfileQuarantineFileState destinationState, string destinationPath, string hashSuffix, int collisionOrdinal, string root, string utcToken, int? unsupportedVersion, bool hashWasProven)
        { _outcome = outcome; _stage = stage; _role = role; _reason = reason; _sourceState = sourceState; _destinationState = destinationState; _destinationPath = destinationPath; _hashSuffix = hashSuffix; _collisionOrdinal = collisionOrdinal; _root = root; _utcToken = utcToken; _unsupportedVersion = unsupportedVersion; _hashWasProven = hashWasProven; }
        public ProfileQuarantineOutcome Outcome { get { Validate(); return _outcome; } } public ProfileQuarantineStage FailureStage { get { Validate(); return _stage; } } public ProfileLoadFileRole SourceRole { get { Validate(); return _role; } } public ProfileFilePreservationReason Reason { get { Validate(); return _reason; } }
        public ProfileQuarantineFileState SourceState { get { Validate(); return _sourceState; } } public ProfileQuarantineFileState DestinationState { get { Validate(); return _destinationState; } } public string DestinationPath { get { Validate(); return _destinationPath; } } public string HashSuffix { get { Validate(); return _hashSuffix; } } public int CollisionOrdinal { get { Validate(); return _collisionOrdinal; } }
        public void Validate()
        {
            if (!KnownOutcome(_outcome) || !KnownStage(_stage) || !KnownState(_sourceState) || !KnownState(_destinationState) || !Compatible(_role, _reason)) throw new InvalidOperationException("Quarantine result is malformed.");
            var hasDestination = !string.IsNullOrEmpty(_destinationPath);
            if (_outcome == ProfileQuarantineOutcome.Preserved)
            { if (_stage != ProfileQuarantineStage.None || _sourceState != ProfileQuarantineFileState.Missing || _destinationState != ProfileQuarantineFileState.Present || !ValidDestinationProof() || _collisionOrdinal < 0) throw new InvalidOperationException("Preserved result is malformed."); return; }
            if (_outcome == ProfileQuarantineOutcome.SourceMissing)
            { if (_stage != ProfileQuarantineStage.SourceRevalidation || _sourceState != ProfileQuarantineFileState.Missing || _destinationState != ProfileQuarantineFileState.NotProbed || hasDestination || _hashSuffix != string.Empty || _collisionOrdinal != -1 || !NoDestinationProof()) throw new InvalidOperationException("Source-missing result is malformed."); return; }
            if (_outcome == ProfileQuarantineOutcome.FailedBeforeMove)
            { if ((_stage != ProfileQuarantineStage.SourceRevalidation && _stage != ProfileQuarantineStage.RecoveryDirectoryPreparation && _stage != ProfileQuarantineStage.DestinationAllocation) || (_sourceState != ProfileQuarantineFileState.Present && _sourceState != ProfileQuarantineFileState.Unreadable) || _destinationState != ProfileQuarantineFileState.NotProbed || hasDestination || _hashSuffix != string.Empty || _collisionOrdinal != -1 || !NoDestinationProof()) throw new InvalidOperationException("Pre-move failure result is malformed."); return; }
            if ((_stage != ProfileQuarantineStage.MoveCall && _stage != ProfileQuarantineStage.PostMoveValidation) || _sourceState == ProfileQuarantineFileState.NotProbed || _destinationState == ProfileQuarantineFileState.NotProbed || !ValidDestinationProof() || _collisionOrdinal < 0) throw new InvalidOperationException("Uncertain move result is malformed.");
        }
        private bool NoDestinationProof() => _root == null && _utcToken == null && !_unsupportedVersion.HasValue && !_hashWasProven;
        private bool ValidDestinationProof()
        {
            if (string.IsNullOrEmpty(_root) || string.IsNullOrEmpty(_destinationPath) || string.IsNullOrEmpty(_utcToken) || !Path.IsPathFullyQualified(_root) || !Path.IsPathFullyQualified(_destinationPath) || !ValidUtcToken(_utcToken) || !ValidSuffix(_hashSuffix)) return false;
            if ((_hashSuffix == "nohash") != !_hashWasProven || (_hashSuffix == "nohash" && !NoHashAllowed(_reason))) return false;
            var unsupported = _reason == ProfileFilePreservationReason.UnsupportedPrimary || _reason == ProfileFilePreservationReason.UnsupportedPrevious;
            if (unsupported != _unsupportedVersion.HasValue) return false;
            string normalized; try { normalized = Trim(Path.GetFullPath(_root)); } catch (Exception e) when (e is ArgumentException || e is NotSupportedException || e is PathTooLongException) { return false; }
            if (!string.Equals(normalized, _root, StringComparison.Ordinal) || string.Equals(_root, Path.GetPathRoot(_root), StringComparison.OrdinalIgnoreCase)) return false;
            var stem = ExactStem(_role, _reason, _utcToken, _hashSuffix, _unsupportedVersion); var expected = Path.Combine(_root, "recovery", stem + (_collisionOrdinal == 0 ? string.Empty : "-" + _collisionOrdinal.ToString(CultureInfo.InvariantCulture)) + ".json");
            return string.Equals(_destinationPath, expected, StringComparison.Ordinal);
        }
        private static bool KnownOutcome(ProfileQuarantineOutcome value) => value >= ProfileQuarantineOutcome.Preserved && value <= ProfileQuarantineOutcome.MoveOutcomeUncertain;
        private static bool KnownStage(ProfileQuarantineStage value) => value >= ProfileQuarantineStage.None && value <= ProfileQuarantineStage.PostMoveValidation;
        private static bool KnownState(ProfileQuarantineFileState value) => value >= ProfileQuarantineFileState.NotProbed && value <= ProfileQuarantineFileState.Unreadable;
        private static bool ValidSuffix(string value) { if (value == "nohash") return true; if (value == null || value.Length != 8) return false; for (var i = 0; i < value.Length; i++) if (!(value[i] >= '0' && value[i] <= '9' || value[i] >= 'a' && value[i] <= 'f')) return false; return true; }
        private static bool ValidUtcToken(string value) { DateTime parsed; return value != null && value.Length == 19 && DateTime.TryParseExact(value, "yyyyMMdd'T'HHmmssfff'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out parsed) && string.Equals(parsed.ToString("yyyyMMdd'T'HHmmssfff'Z'", CultureInfo.InvariantCulture), value, StringComparison.Ordinal); }
        private static string ExactStem(ProfileLoadFileRole role, ProfileFilePreservationReason reason, string utc, string suffix, int? unsupportedVersion)
        { if (reason == ProfileFilePreservationReason.StaleTemp) return "profile-stale-temp-" + utc + "-" + suffix; if (reason == ProfileFilePreservationReason.InvalidPrimary) return "profile-invalid-primary-" + utc + "-" + suffix; if (reason == ProfileFilePreservationReason.InvalidPrevious) return "profile-invalid-previous-" + utc + "-" + suffix; if (reason == ProfileFilePreservationReason.UnsupportedPrimary || reason == ProfileFilePreservationReason.UnsupportedPrevious) return "profile-unsupported-v" + unsupportedVersion.Value.ToString(CultureInfo.InvariantCulture) + "-" + utc + "-" + suffix; return "profile-unreadable-" + (role == ProfileLoadFileRole.Primary ? "primary" : "previous") + "-" + utc + "-" + suffix; }
        private static string Trim(string path) { var root = Path.GetPathRoot(path); var end = path.Length; while (end > root.Length && (path[end - 1] == Path.DirectorySeparatorChar || path[end - 1] == Path.AltDirectorySeparatorChar)) end--; return path.Substring(0, end); }
        private static bool NoHashAllowed(ProfileFilePreservationReason reason) => reason == ProfileFilePreservationReason.UnreadablePrimary || reason == ProfileFilePreservationReason.UnreadablePrevious || reason == ProfileFilePreservationReason.StaleTemp;
        private static bool Compatible(ProfileLoadFileRole role, ProfileFilePreservationReason reason) => (role == ProfileLoadFileRole.Primary && (reason == ProfileFilePreservationReason.InvalidPrimary || reason == ProfileFilePreservationReason.UnsupportedPrimary || reason == ProfileFilePreservationReason.UnreadablePrimary)) || (role == ProfileLoadFileRole.Previous && (reason == ProfileFilePreservationReason.InvalidPrevious || reason == ProfileFilePreservationReason.UnsupportedPrevious || reason == ProfileFilePreservationReason.UnreadablePrevious)) || (role == ProfileLoadFileRole.Temp && reason == ProfileFilePreservationReason.StaleTemp);
    }

    internal interface IProfileQuarantineFileOperations { ProfileQuarantinePresence ProbePresence(string path); byte[] ReadAllExclusive(string path); void EnsureDirectory(string path); void Move(string source, string destination); string Hash(byte[] bytes); }
    internal sealed class ProfileQuarantineBclOperations : IProfileQuarantineFileOperations
    {
        internal static readonly ProfileQuarantineBclOperations Instance = new ProfileQuarantineBclOperations(); private ProfileQuarantineBclOperations() { }
        public ProfileQuarantinePresence ProbePresence(string path) { try { File.GetAttributes(path); return ProfileQuarantinePresence.Present; } catch (FileNotFoundException) { return ProfileQuarantinePresence.Missing; } catch (DirectoryNotFoundException) { return ProfileQuarantinePresence.Missing; } }
        public byte[] ReadAllExclusive(string path) { using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None)) { if (stream.Length > int.MaxValue) throw new IOException(); var bytes = new byte[(int)stream.Length]; var offset = 0; while (offset < bytes.Length) { var read = stream.Read(bytes, offset, bytes.Length - offset); if (read == 0) throw new EndOfStreamException(); offset += read; } return bytes; } }
        public void EnsureDirectory(string path) { Directory.CreateDirectory(path); }
        public void Move(string source, string destination) { File.Move(source, destination); }
        public string Hash(byte[] bytes) { using (var sha = SHA256.Create()) { var hash = sha.ComputeHash(bytes); var builder = new StringBuilder(64); foreach (var item in hash) builder.Append(item.ToString("x2", CultureInfo.InvariantCulture)); return builder.ToString(); } }
    }

    public static class ProfileQuarantineServiceV1
    {
        private static readonly object RegistryGate = new object(); private static readonly Dictionary<string, Lease> Leases = new Dictionary<string, Lease>(StringComparer.OrdinalIgnoreCase);
        public static ProfileQuarantineResultV1 Preserve(string rootDirectoryPath, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, DateTimeOffset utcNow) => Preserve(rootDirectoryPath, candidate, reason, utcNow, ProfileQuarantineBclOperations.Instance);
        internal static ProfileQuarantineResultV1 Preserve(string rootDirectoryPath, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, DateTimeOffset utcNow, IProfileQuarantineFileOperations operations)
        {
            var root = ValidateArguments(rootDirectoryPath, candidate, reason, utcNow, operations); var lease = Acquire(root);
            try { lock (lease.Gate) { return PreserveHeld(root, candidate, reason, utcNow, operations); } } finally { Release(root, lease); }
        }
        private static ProfileQuarantineResultV1 PreserveHeld(string root, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, DateTimeOffset utc, IProfileQuarantineFileOperations operations)
        {
            var source = Path.Combine(root, Leaf(candidate.Role)); var recovery = Path.Combine(root, "recovery"); byte[] bytes = null; var readable = false; var sourceState = ProfileQuarantineFileState.Present;
            try { if (RequirePresence(operations.ProbePresence(source)) == ProfileQuarantinePresence.Missing) return NoDestinationResult(ProfileQuarantineOutcome.SourceMissing, ProfileQuarantineStage.SourceRevalidation, candidate, reason, ProfileQuarantineFileState.Missing); }
            catch (Exception e) when (Recoverable(e)) { sourceState = ProfileQuarantineFileState.Unreadable; if (candidate.Kind != ProfileLoadCandidateKind.Unreadable) return Failed(ProfileQuarantineStage.SourceRevalidation, candidate, reason, sourceState, root); }
            try { bytes = operations.ReadAllExclusive(source); readable = true; if (!Eligible(candidate, reason, ProfileCanonicalDecoderV1.Decode(bytes))) return Failed(ProfileQuarantineStage.SourceRevalidation, candidate, reason, ProfileQuarantineFileState.Present, root); }
            catch (Exception e) when (Recoverable(e)) { sourceState = ProfileQuarantineFileState.Unreadable; if (candidate.Kind != ProfileLoadCandidateKind.Unreadable) return Failed(ProfileQuarantineStage.SourceRevalidation, candidate, reason, sourceState, root); }
            try { operations.EnsureDirectory(recovery); } catch (Exception e) when (Recoverable(e)) { return Failed(ProfileQuarantineStage.RecoveryDirectoryPreparation, candidate, reason, sourceState, root); }
            string digest = null; string suffix;
            try { if (readable) { digest = RequireDigest(operations.Hash(bytes)); suffix = digest.Substring(0, 8); } else suffix = "nohash"; }
            catch (Exception e) when (Recoverable(e)) { return Failed(ProfileQuarantineStage.DestinationAllocation, candidate, reason, sourceState, root); }
            string destination; int ordinal;
            try { destination = Allocate(recovery, candidate, reason, utc, suffix, operations, out ordinal); }
            catch (Exception e) when (Recoverable(e)) { return Failed(ProfileQuarantineStage.DestinationAllocation, candidate, reason, sourceState, root); }
            try { operations.Move(source, destination); }
            catch (Exception e) when (Recoverable(e)) { return Uncertain(ProfileQuarantineStage.MoveCall, candidate, reason, source, destination, suffix, ordinal, root, utc, readable, operations); }
            try
            {
                var postSource = State(source, operations); var postDestination = State(destination, operations);
                if (postSource != ProfileQuarantineFileState.Missing || postDestination != ProfileQuarantineFileState.Present || (readable && !string.Equals(RequireDigest(operations.Hash(operations.ReadAllExclusive(destination))), digest, StringComparison.Ordinal))) return Uncertain(ProfileQuarantineStage.PostMoveValidation, candidate, reason, source, destination, suffix, ordinal, root, utc, readable, operations);
                return DestinationResult(ProfileQuarantineOutcome.Preserved, ProfileQuarantineStage.None, candidate, reason, ProfileQuarantineFileState.Missing, ProfileQuarantineFileState.Present, destination, suffix, ordinal, root, utc, readable);
            }
            catch (Exception e) when (Recoverable(e)) { return Uncertain(ProfileQuarantineStage.PostMoveValidation, candidate, reason, source, destination, suffix, ordinal, root, utc, readable, operations); }
        }
        private static string Allocate(string recovery, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, DateTimeOffset utc, string suffix, IProfileQuarantineFileOperations operations, out int ordinal)
        { var stem = Stem(candidate, reason, utc, suffix); ordinal = 0; while (true) { var path = Path.Combine(recovery, stem + (ordinal == 0 ? string.Empty : "-" + ordinal.ToString(CultureInfo.InvariantCulture)) + ".json"); if (RequirePresence(operations.ProbePresence(path)) == ProfileQuarantinePresence.Missing) return path; if (ordinal == int.MaxValue) throw new IOException("Collision ordinal exhausted."); ordinal = checked(ordinal + 1); } }
        private static bool Eligible(ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, ProfileCanonicalDecodeResultV1 result)
        { if (reason == ProfileFilePreservationReason.StaleTemp) return true; var classification = result.Classification; if (reason == ProfileFilePreservationReason.InvalidPrimary || reason == ProfileFilePreservationReason.InvalidPrevious) return classification == ProfileCanonicalDecodeClassification.Invalid; if (reason == ProfileFilePreservationReason.UnsupportedPrimary || reason == ProfileFilePreservationReason.UnsupportedPrevious) return classification == ProfileCanonicalDecodeClassification.UnsupportedProfileSchema && candidate.DecodeResult.UnsupportedSchemaVersion == result.UnsupportedSchemaVersion; if (reason == ProfileFilePreservationReason.UnreadablePrimary || reason == ProfileFilePreservationReason.UnreadablePrevious) return classification == ProfileCanonicalDecodeClassification.Invalid || classification == ProfileCanonicalDecodeClassification.UnsupportedProfileSchema; return false; }
        private static string Stem(ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, DateTimeOffset utc, string suffix)
        { var time = utc.ToUniversalTime().ToString("yyyyMMdd'T'HHmmssfff'Z'", CultureInfo.InvariantCulture); if (reason == ProfileFilePreservationReason.StaleTemp) return "profile-stale-temp-" + time + "-" + suffix; if (reason == ProfileFilePreservationReason.InvalidPrimary) return "profile-invalid-primary-" + time + "-" + suffix; if (reason == ProfileFilePreservationReason.InvalidPrevious) return "profile-invalid-previous-" + time + "-" + suffix; if (reason == ProfileFilePreservationReason.UnsupportedPrimary || reason == ProfileFilePreservationReason.UnsupportedPrevious) return "profile-unsupported-v" + candidate.DecodeResult.UnsupportedSchemaVersion.ToString(CultureInfo.InvariantCulture) + "-" + time + "-" + suffix; return "profile-unreadable-" + (candidate.Role == ProfileLoadFileRole.Primary ? "primary" : "previous") + "-" + time + "-" + suffix; }
        private static ProfileQuarantineResultV1 Failed(ProfileQuarantineStage stage, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, ProfileQuarantineFileState source, string root) => NoDestinationResult(ProfileQuarantineOutcome.FailedBeforeMove, stage, candidate, reason, source);
        private static ProfileQuarantineResultV1 Uncertain(ProfileQuarantineStage stage, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, string source, string destination, string suffix, int ordinal, string root, DateTimeOffset utc, bool hashWasProven, IProfileQuarantineFileOperations operations) => DestinationResult(ProfileQuarantineOutcome.MoveOutcomeUncertain, stage, candidate, reason, State(source, operations), State(destination, operations), destination, suffix, ordinal, root, utc, hashWasProven);
        private static ProfileQuarantineResultV1 NoDestinationResult(ProfileQuarantineOutcome outcome, ProfileQuarantineStage stage, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, ProfileQuarantineFileState source) => new ProfileQuarantineResultV1(outcome, stage, candidate.Role, reason, source, ProfileQuarantineFileState.NotProbed, string.Empty, string.Empty, -1, null, null, null, false);
        private static ProfileQuarantineResultV1 DestinationResult(ProfileQuarantineOutcome outcome, ProfileQuarantineStage stage, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, ProfileQuarantineFileState source, ProfileQuarantineFileState destination, string path, string suffix, int ordinal, string root, DateTimeOffset utc, bool hashWasProven) => new ProfileQuarantineResultV1(outcome, stage, candidate.Role, reason, source, destination, path, suffix, ordinal, root, utc.ToUniversalTime().ToString("yyyyMMdd'T'HHmmssfff'Z'", CultureInfo.InvariantCulture), (reason == ProfileFilePreservationReason.UnsupportedPrimary || reason == ProfileFilePreservationReason.UnsupportedPrevious) ? candidate.DecodeResult.UnsupportedSchemaVersion : (int?)null, hashWasProven);
        private static ProfileQuarantineFileState State(string path, IProfileQuarantineFileOperations operations) { try { return RequirePresence(operations.ProbePresence(path)) == ProfileQuarantinePresence.Missing ? ProfileQuarantineFileState.Missing : ProfileQuarantineFileState.Present; } catch (Exception e) when (Recoverable(e)) { return ProfileQuarantineFileState.Unreadable; } }
        private static string RequireDigest(string value) { if (value == null || value.Length != 64) throw new InvalidOperationException("Digest is malformed."); for (var i = 0; i < value.Length; i++) if (!((value[i] >= '0' && value[i] <= '9') || (value[i] >= 'a' && value[i] <= 'f'))) throw new InvalidOperationException("Digest is malformed."); return value; }
        private static string ValidateArguments(string root, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, DateTimeOffset utc, IProfileQuarantineFileOperations operations) { if (operations == null) throw new ArgumentNullException(nameof(operations)); if (string.IsNullOrWhiteSpace(root) || !Path.IsPathFullyQualified(root) || utc.Offset != TimeSpan.Zero) throw new ArgumentException("Quarantine arguments are malformed."); string normalized; try { normalized = Trim(Path.GetFullPath(root)); } catch (Exception e) when (e is ArgumentException || e is NotSupportedException || e is PathTooLongException) { throw new ArgumentException("Root path is malformed.", nameof(root), e); } if (string.Equals(normalized, Path.GetPathRoot(normalized), StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Root cannot be a filesystem root.", nameof(root)); try { candidate.Validate(); } catch (InvalidOperationException e) { throw new ArgumentException("Candidate is malformed.", nameof(candidate), e); } var expected = candidate.Role == ProfileLoadFileRole.Temp ? ProfileLoadSelectionPlanV1.TempPreservationFor(candidate) : ProfileLoadSelectionPlanV1.PreservationFor(candidate, candidate.Role == ProfileLoadFileRole.Primary); if (reason == ProfileFilePreservationReason.None || reason != expected) throw new ArgumentException("Preservation reason is incompatible.", nameof(reason)); return normalized; }
        private static string Leaf(ProfileLoadFileRole role) => role == ProfileLoadFileRole.Primary ? "profile.json" : role == ProfileLoadFileRole.Previous ? "profile.prev.json" : "profile.tmp.json";
        private static string Trim(string path) { var root = Path.GetPathRoot(path); var end = path.Length; while (end > root.Length && (path[end - 1] == Path.DirectorySeparatorChar || path[end - 1] == Path.AltDirectorySeparatorChar)) end--; return path.Substring(0, end); }
        private static bool Recoverable(Exception e) => e is IOException || e is UnauthorizedAccessException || e is SecurityException;
        private static ProfileQuarantinePresence RequirePresence(ProfileQuarantinePresence value) { if (value != ProfileQuarantinePresence.Missing && value != ProfileQuarantinePresence.Present) throw new InvalidOperationException("Presence is malformed."); return value; }
        private static Lease Acquire(string key) { lock (RegistryGate) { Lease lease; if (!Leases.TryGetValue(key, out lease)) { lease = new Lease(); Leases.Add(key, lease); } lease.Count++; return lease; } }
        private static void Release(string key, Lease lease) { lock (RegistryGate) { lease.Count--; if (lease.Count == 0) Leases.Remove(key); } }
        private sealed class Lease { internal readonly object Gate = new object(); internal int Count; }
    }
}
