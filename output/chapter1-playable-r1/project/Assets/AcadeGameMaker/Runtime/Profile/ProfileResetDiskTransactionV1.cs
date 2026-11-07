using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D7QC1-001..007. Disk-only New Game transaction.  This type has no
    // Unity, memory, receipt, UI, or barrier-removal authority.
    public enum ProfileResetDiskOutcomeV1 { DiskPrepared = 1, ConfirmationStale = 2, Busy = 3, ReloadRequired = 4, ManualRepairRequired = 5 }
    public enum ProfileResetDurableStateV1 { NoBarrier = 1, BarrierPublished = 2, ArchivingOldLeaves = 3, OldLeavesArchived = 4, DefaultCommitUncertain = 5, DiskPrepared = 6, ManualRepairRequired = 7 }
    public enum ProfileResetLeafPresenceV1 { Missing = 1, Present = 2 }
    public enum ProfileResetLeafClassificationV1 { ValidCurrentInput = 1, ValidInputRecoveryRequired = 2, UnsupportedSchema = 3, Invalid = 4 }
    // REQ-M5D7QC1-006/007: result observations are evidence only.  They do
    // not identify a path or authorize recovery when marker authentication is absent.
    internal enum ProfileResetDiskObservationV1 { Missing = 1, Readable = 2, Unreadable = 3, NotObserved = 4 }
    internal enum ProfileResetDiskArchiveProofV1 { NotApplicable = 1, Pending = 2, Exact = 3, Collision = 4, Mismatch = 5, Unreadable = 6, NotObserved = 7 }
    internal enum ProfileResetDiskFailureSourceV1 { None = 1, Lease = 2, FreshObservation = 3, Directory = 4, MarkerPublication = 5, MarkerValidation = 6, ArchiveTemp = 7, ArchivePrimary = 8, ArchivePrevious = 9, PreserveTemp = 10, PreservePrimary = 11, DefaultPrecommit = 12, DefaultCommit = 13, DefaultPostcheck = 14, Snapshot = 15 }

    internal readonly struct ProfileResetDiskActiveSnapshotV1
    {
        private readonly string _role, _name, _hash;
        private readonly ProfileResetDiskObservationV1 _observation;
        private readonly ProfileResetLeafClassificationV1? _classification;
        private readonly long? _length, _revision;
        internal ProfileResetDiskActiveSnapshotV1(string role, string name, ProfileResetDiskObservationV1 observation, Leaf leaf)
        {
            _role = role; _name = name; _observation = observation;
            _classification = leaf == null ? null : leaf.Classification;
            _length = leaf == null ? null : leaf.Length;
            _hash = leaf == null ? null : leaf.Hash;
            _revision = leaf == null ? null : leaf.Revision;
        }
        public ProfileResetDiskObservationV1 Observation { get { Validate(); return _observation; } }
        public ProfileResetLeafClassificationV1? Classification { get { Validate(); return _classification; } }
        public long? ByteLength { get { Validate(); return _length; } }
        public string Sha256 { get { Validate(); return _hash; } }
        public long? DecodedRevision { get { Validate(); return _revision; } }
        internal bool HasRoleName(string role, string name) { return _role == role && _name == name; }
        internal void Validate()
        {
            if (string.IsNullOrEmpty(_role) || string.IsNullOrEmpty(_name) || _observation < ProfileResetDiskObservationV1.Missing || _observation > ProfileResetDiskObservationV1.NotObserved) throw new InvalidOperationException("Active snapshot is malformed.");
            if (_observation != ProfileResetDiskObservationV1.Readable && (_classification.HasValue || _length.HasValue || _hash != null || _revision.HasValue)) throw new InvalidOperationException("Non-readable snapshot has a fingerprint.");
            if (_observation == ProfileResetDiskObservationV1.Readable && (!_classification.HasValue || _classification.Value < ProfileResetLeafClassificationV1.ValidCurrentInput || _classification.Value > ProfileResetLeafClassificationV1.Invalid || !_length.HasValue || _length.Value < 0 || !HashOk(_hash) || ((_classification == ProfileResetLeafClassificationV1.ValidCurrentInput || _classification == ProfileResetLeafClassificationV1.ValidInputRecoveryRequired) != _revision.HasValue) || (_revision.HasValue && _revision.Value < 0))) throw new InvalidOperationException("Readable snapshot is malformed.");
        }
        private static bool HashOk(string value) { if (value == null || value.Length != 64) return false; for (var i = 0; i < value.Length; i++) if (!((value[i] >= '0' && value[i] <= '9') || (value[i] >= 'a' && value[i] <= 'f'))) return false; return true; }
    }

    internal readonly struct ProfileResetDiskArchiveSnapshotV1
    {
        private readonly string _role, _name, _hash;
        private readonly ProfileResetDiskArchiveProofV1 _proof;
        private readonly ProfileResetDiskObservationV1 _observation;
        private readonly ProfileResetLeafClassificationV1? _classification;
        private readonly long? _length, _revision;
        internal ProfileResetDiskArchiveSnapshotV1(string role, string name, ProfileResetDiskArchiveProofV1 proof, ProfileResetDiskObservationV1 observation, Leaf leaf)
        {
            _role = role; _name = name; _proof = proof; _observation = observation;
            _classification = leaf == null ? null : leaf.Classification;
            _length = leaf == null ? null : leaf.Length;
            _hash = leaf == null ? null : leaf.Hash;
            _revision = leaf == null ? null : leaf.Revision;
        }
        public ProfileResetDiskArchiveProofV1 Proof { get { Validate(); return _proof; } }
        public ProfileResetDiskObservationV1 Observation { get { Validate(); return _observation; } }
        public ProfileResetLeafClassificationV1? Classification { get { Validate(); return _classification; } }
        public long? ByteLength { get { Validate(); return _length; } }
        public string Sha256 { get { Validate(); return _hash; } }
        public long? DecodedRevision { get { Validate(); return _revision; } }
        internal bool HasRoleName(string role, string name) { return _role == role && _name == name; }
        internal void Validate()
        {
            if (string.IsNullOrEmpty(_role) || string.IsNullOrEmpty(_name) || _proof < ProfileResetDiskArchiveProofV1.NotApplicable || _proof > ProfileResetDiskArchiveProofV1.NotObserved || _observation < ProfileResetDiskObservationV1.Missing || _observation > ProfileResetDiskObservationV1.NotObserved) throw new InvalidOperationException("Archive snapshot is malformed.");
            if (_observation != ProfileResetDiskObservationV1.Readable && (_classification.HasValue || _length.HasValue || _hash != null || _revision.HasValue)) throw new InvalidOperationException("Non-readable archive has a fingerprint.");
            if (_observation == ProfileResetDiskObservationV1.Readable && (!_classification.HasValue || _classification.Value < ProfileResetLeafClassificationV1.ValidCurrentInput || _classification.Value > ProfileResetLeafClassificationV1.Invalid || !_length.HasValue || _length.Value < 0 || !HashOk(_hash) || ((_classification == ProfileResetLeafClassificationV1.ValidCurrentInput || _classification == ProfileResetLeafClassificationV1.ValidInputRecoveryRequired) != _revision.HasValue) || (_revision.HasValue && _revision.Value < 0))) throw new InvalidOperationException("Readable archive is malformed.");
            if ((_proof == ProfileResetDiskArchiveProofV1.NotApplicable || _proof == ProfileResetDiskArchiveProofV1.Pending) && _observation != ProfileResetDiskObservationV1.Missing) throw new InvalidOperationException("Pending archive observation is malformed.");
            if ((_proof == ProfileResetDiskArchiveProofV1.Exact || _proof == ProfileResetDiskArchiveProofV1.Mismatch || _proof == ProfileResetDiskArchiveProofV1.Collision) && _observation != ProfileResetDiskObservationV1.Readable) throw new InvalidOperationException("Readable archive proof is malformed.");
            if (_proof == ProfileResetDiskArchiveProofV1.Unreadable && _observation != ProfileResetDiskObservationV1.Unreadable) throw new InvalidOperationException("Unreadable archive proof is malformed.");
            if (_proof == ProfileResetDiskArchiveProofV1.NotObserved && _observation != ProfileResetDiskObservationV1.NotObserved) throw new InvalidOperationException("Unobserved archive proof is malformed.");
        }
        private static bool HashOk(string value) { if (value == null || value.Length != 64) return false; for (var i = 0; i < value.Length; i++) if (!((value[i] >= '0' && value[i] <= '9') || (value[i] >= 'a' && value[i] <= 'f'))) return false; return true; }
    }
    public readonly struct ProfileResetDiskLeafProofV1
    {
        private readonly string _role, _name; private readonly ProfileResetLeafPresenceV1 _presence; private readonly long _length; private readonly string _hash;
        internal ProfileResetDiskLeafProofV1(Leaf leaf) { _role = leaf.Role; _name = leaf.Name; _presence = leaf.Presence; _length = leaf.Length ?? -1; _hash = leaf.Hash; }
        public ProfileResetLeafPresenceV1 Presence { get { Validate(); return _presence; } } public long ByteLength { get { Validate(); return _length; } } public string Sha256 { get { Validate(); return _hash; } }
        internal void Validate() { if (string.IsNullOrEmpty(_role) || string.IsNullOrEmpty(_name) || (_presence != ProfileResetLeafPresenceV1.Missing && _presence != ProfileResetLeafPresenceV1.Present) || (_presence == ProfileResetLeafPresenceV1.Missing && (_length != -1 || _hash != null)) || (_presence == ProfileResetLeafPresenceV1.Present && (_length < 0 || !HashOk(_hash)))) throw new InvalidOperationException("Leaf proof is malformed."); }
        internal bool Matches(Leaf leaf) { return leaf != null && _role == leaf.Role && _name == leaf.Name && _presence == leaf.Presence && _length == (leaf.Length ?? -1) && _hash == leaf.Hash; }
        internal bool HasRoleName(string role, string name) { return _role == role && _name == name; }
        private static bool HashOk(string value) { if (value == null || value.Length != 64) return false; for (var i = 0; i < value.Length; i++) if (!((value[i] >= '0' && value[i] <= '9') || (value[i] >= 'a' && value[i] <= 'f'))) return false; return true; }
    }

    internal sealed class ProfileResetConfirmationIdentityV1
    {
        internal readonly string Root; internal readonly Leaf[] Leaves; private int _used;
        private ProfileResetConfirmationIdentityV1(string root, Leaf[] leaves) { Root = root; Leaves = (Leaf[])leaves.Clone(); }
        internal static ProfileResetConfirmationIdentityV1 Create(string root, Leaf[] leaves) { return new ProfileResetConfirmationIdentityV1(root, leaves); }
        internal bool TryConsume() { return System.Threading.Interlocked.CompareExchange(ref _used, 1, 0) == 0; }
    }
    // REQ-M5D7QC3-002/004 and REQ-M5D7QC3L-003: immutable C1-owned
    // observation result.  It is evidence only and grants no Begin authority.
    internal sealed class ProfileResetConfirmationCaptureV1
    {
        internal readonly ProfileResetConfirmationIdentityV1 Identity;
        internal readonly ProfileCanonicalDecodeResultV1? Primary, Previous, Temp;
        internal ProfileResetConfirmationCaptureV1(ProfileResetConfirmationIdentityV1 identity,
            ProfileCanonicalDecodeResultV1? primary, ProfileCanonicalDecodeResultV1? previous,
            ProfileCanonicalDecodeResultV1? temp)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));
            Identity = identity; Primary = primary; Previous = previous; Temp = temp;
        }
        internal void Validate()
        {
            if (!ProfileResetDiskTransactionV1.ValidateIssuedConfirmationCapture(this)) throw new InvalidOperationException("Confirmation capture was not issued by the observation lease.");
            if (Identity == null || Identity.Leaves == null || Identity.Leaves.Length != 3) throw new InvalidOperationException("Confirmation capture identity is malformed.");
            var values = new ProfileCanonicalDecodeResultV1?[] { Primary, Previous, Temp };
            for (var i = 0; i < 3; i++)
            {
                var leaf = Identity.Leaves[i];
                if (leaf.Presence == ProfileResetLeafPresenceV1.Missing) { if (values[i].HasValue) throw new InvalidOperationException("Missing observation leaf has a projection."); continue; }
                values[i].Value.Validate(); var classification = values[i].Value.Classification;
                var mapped = classification == ProfileCanonicalDecodeClassification.ValidCurrentInput ? ProfileResetLeafClassificationV1.ValidCurrentInput : (classification == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired || classification == ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired) ? ProfileResetLeafClassificationV1.ValidInputRecoveryRequired : classification == ProfileCanonicalDecodeClassification.UnsupportedProfileSchema ? ProfileResetLeafClassificationV1.UnsupportedSchema : ProfileResetLeafClassificationV1.Invalid;
                if (leaf.Classification != mapped || (mapped == ProfileResetLeafClassificationV1.ValidCurrentInput && leaf.Revision != values[i].Value.Document.Snapshot.ProfileRevision) || (mapped == ProfileResetLeafClassificationV1.ValidInputRecoveryRequired && leaf.Revision != values[i].Value.RecoveryProjection.SourceProfileRevision)) throw new InvalidOperationException("Confirmation capture projection is malformed.");
            }
        }
    }
    internal sealed class Leaf
    {
        internal readonly string Role, Name, Hash; internal readonly ProfileResetLeafPresenceV1 Presence; internal readonly ProfileResetLeafClassificationV1? Classification; internal readonly long? Length, Revision;
        internal Leaf(string role, string name, ProfileResetLeafPresenceV1 presence, ProfileResetLeafClassificationV1? classification, long? length, string hash, long? revision) { Role = role; Name = name; Presence = presence; Classification = classification; Length = length; Hash = hash; Revision = revision; }
        internal bool Same(Leaf other) { return other != null && Role == other.Role && Name == other.Name && Presence == other.Presence && Classification == other.Classification && Length == other.Length && Hash == other.Hash && Revision == other.Revision; }
    }
    internal sealed class ValidatedProfileResetBarrierV1
    {
        internal readonly string Root, Id, Text; internal readonly byte[] Target; internal readonly Leaf[] Leaves; internal readonly ProfileRootOperationLockV1 IssuingLease;
        private ValidatedProfileResetBarrierV1(string root, string id, string text, byte[] target, Leaf[] leaves, ProfileRootOperationLockV1 issuingLease) { Root = root; Id = id; Text = text; Target = (byte[])target.Clone(); Leaves = (Leaf[])leaves.Clone(); IssuingLease = issuingLease; }
        internal static ValidatedProfileResetBarrierV1 BindLease(ValidatedProfileResetBarrierV1 marker, ProfileRootOperationLockV1 lease) { if (marker == null || lease == null) throw new ArgumentNullException(marker == null ? nameof(marker) : nameof(lease)); lease.ValidateOwnership(marker.Root); return new ValidatedProfileResetBarrierV1(marker.Root, marker.Id, marker.Text, marker.Target, marker.Leaves, lease); }
        internal static ValidatedProfileResetBarrierV1 Create(string root, string id, string text, byte[] target, Leaf[] leaves) { return new ValidatedProfileResetBarrierV1(root, id, text, (byte[])target.Clone(), leaves, null); }
    }
    // REQ-M5D7QC1-002/003/005. Internal deterministic checkpoint seam only;
    // production passes the no-op implementation and exposes no public route.
    internal enum ProfileResetDiskCheckpointV1
    {
        BeforeMarkerWrite = 1, AfterMarkerWrite = 2, BeforeMarkerFlush = 3,
        AfterMarkerFlush = 4, BeforeMarkerClose = 5, AfterMarkerClose = 6, BeforeMarkerTempReopen = 7,
        AfterMarkerTempReopen = 8, BeforeMarkerCommitMove = 9,
        AfterMarkerCommitMove = 10, BeforeMarkerCommittedReopen = 11,
        AfterMarkerCommittedReopen = 12,
        BeforeArchiveTempMove = 13, AfterArchiveTempMove = 14, BeforeArchiveTempReopen = 15, AfterArchiveTempReopen = 16,
        BeforeArchivePrimaryMove = 17, AfterArchivePrimaryMove = 18, BeforeArchivePrimaryReopen = 19, AfterArchivePrimaryReopen = 20,
        BeforeArchivePreviousMove = 21, AfterArchivePreviousMove = 22, BeforeArchivePreviousReopen = 23, AfterArchivePreviousReopen = 24,
        BeforeInterruptedTempMove = 25, AfterInterruptedTempMove = 26, BeforeInterruptedTempReopen = 27, AfterInterruptedTempReopen = 28,
        BeforeInterruptedPrimaryMove = 29, AfterInterruptedPrimaryMove = 30, BeforeInterruptedPrimaryReopen = 31, AfterInterruptedPrimaryReopen = 32,
        BeforeDefaultSave = 33, BeforeDefaultWrite = 34, AfterDefaultWrite = 35, BeforeDefaultFlush = 36, AfterDefaultFlush = 37,
        BeforeDefaultClose = 38, AfterDefaultClose = 39, BeforeDefaultTempReopen = 40, AfterDefaultTempReopen = 41,
        BeforeDefaultCommitMove = 42, AfterDefaultCommitMove = 43, BeforeDefaultPostPrimaryReopen = 44, AfterDefaultPostPrimaryReopen = 45,
        BeforeDefaultPreviousProbe = 46, AfterDefaultPreviousProbe = 47, BeforeDefaultTempProbe = 48, AfterDefaultTempProbe = 49,
        BeforeDefaultTempOpen = 50, AfterDefaultTempOpen = 51
    }
    internal interface IProfileResetDiskTestControlV1 { void Checkpoint(ProfileResetDiskCheckpointV1 checkpoint); }
    internal sealed class ProfileResetDiskNoOpTestControlV1 : IProfileResetDiskTestControlV1
    {
        internal static readonly ProfileResetDiskNoOpTestControlV1 Instance = new ProfileResetDiskNoOpTestControlV1();
        private ProfileResetDiskNoOpTestControlV1() { }
        public void Checkpoint(ProfileResetDiskCheckpointV1 checkpoint) { }
    }
    // Reflection-based engine-free workers can construct an Action<closed enum>
    // without widening the production ABI or exposing a mutable global hook.
    internal static class ProfileResetDiskTestControlFactoryV1
    {
        internal static IProfileResetDiskTestControlV1 Create(Action<ProfileResetDiskCheckpointV1> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return new CallbackControl(action);
        }
        private sealed class CallbackControl : IProfileResetDiskTestControlV1
        {
            private readonly Action<ProfileResetDiskCheckpointV1> _action;
            internal CallbackControl(Action<ProfileResetDiskCheckpointV1> action) { _action = action; }
            public void Checkpoint(ProfileResetDiskCheckpointV1 checkpoint) { _action(checkpoint); }
        }
    }
    // REQ-M5D7QC1-004/006. This is an internal BCL delegate used only by the
    // reset coordinator's controlled first save; it does not duplicate SaveHeld.
    internal sealed class ProfileResetCheckpointAtomicOperationsV1 : IProfileAtomicSaveFileOperations
    {
        private readonly IProfileResetDiskTestControlV1 _control; private readonly string _root; private readonly IProfileAtomicSaveFileOperations _inner;
        internal ProfileResetCheckpointAtomicOperationsV1(IProfileResetDiskTestControlV1 control, string root)
        {
            if (control == null) throw new ArgumentNullException(nameof(control)); if (root == null) throw new ArgumentNullException(nameof(root));
            _control = control; _root = root; _inner = ProfileAtomicSaveBclFileOperations.Instance;
        }
        public void EnsureDirectory(string path) { _inner.EnsureDirectory(path); }
        public IProfileAtomicSaveTempWriter OpenTempExclusive(string path)
        {
            _control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeDefaultTempOpen);
            var writer = _inner.OpenTempExclusive(path);
            try { _control.Checkpoint(ProfileResetDiskCheckpointV1.AfterDefaultTempOpen); return new Writer(writer, _control); }
            catch { writer.Close(); throw; }
        }
        public ProfileAtomicSaveFilePresence ProbePresence(string path)
        {
            var before = ProbeCheckpoint(path, true); if (before.HasValue) _control.Checkpoint(before.Value);
            var value = _inner.ProbePresence(path);
            var after = ProbeCheckpoint(path, false); if (after.HasValue) _control.Checkpoint(after.Value);
            return value;
        }
        public byte[] ReadAllExclusive(string path)
        {
            var before = ReadCheckpoint(path, true); if (before.HasValue) _control.Checkpoint(before.Value);
            var value = _inner.ReadAllExclusive(path);
            var after = ReadCheckpoint(path, false); if (after.HasValue) _control.Checkpoint(after.Value);
            return value;
        }
        public void Replace(string tempPath, string primaryPath, string previousPath) { _inner.Replace(tempPath, primaryPath, previousPath); }
        public void Move(string tempPath, string primaryPath)
        {
            _control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeDefaultCommitMove);
            _inner.Move(tempPath, primaryPath);
            _control.Checkpoint(ProfileResetDiskCheckpointV1.AfterDefaultCommitMove);
        }
        private ProfileResetDiskCheckpointV1? ProbeCheckpoint(string path, bool before)
        {
            if (string.Equals(path, Path.Combine(_root, "profile.prev.json"), StringComparison.OrdinalIgnoreCase)) return before ? ProfileResetDiskCheckpointV1.BeforeDefaultPreviousProbe : ProfileResetDiskCheckpointV1.AfterDefaultPreviousProbe;
            if (string.Equals(path, Path.Combine(_root, "profile.tmp.json"), StringComparison.OrdinalIgnoreCase)) return before ? ProfileResetDiskCheckpointV1.BeforeDefaultTempProbe : ProfileResetDiskCheckpointV1.AfterDefaultTempProbe;
            return null;
        }
        private ProfileResetDiskCheckpointV1? ReadCheckpoint(string path, bool before)
        {
            if (string.Equals(path, Path.Combine(_root, "profile.tmp.json"), StringComparison.OrdinalIgnoreCase)) return before ? ProfileResetDiskCheckpointV1.BeforeDefaultTempReopen : ProfileResetDiskCheckpointV1.AfterDefaultTempReopen;
            if (string.Equals(path, Path.Combine(_root, "profile.json"), StringComparison.OrdinalIgnoreCase)) return before ? ProfileResetDiskCheckpointV1.BeforeDefaultPostPrimaryReopen : ProfileResetDiskCheckpointV1.AfterDefaultPostPrimaryReopen;
            return null;
        }
        private sealed class Writer : IProfileAtomicSaveTempWriter
        {
            private readonly IProfileAtomicSaveTempWriter _inner; private readonly IProfileResetDiskTestControlV1 _control;
            internal Writer(IProfileAtomicSaveTempWriter inner, IProfileResetDiskTestControlV1 control) { _inner = inner; _control = control; }
            public void WriteAll(byte[] bytes) { _control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeDefaultWrite); _inner.WriteAll(bytes); _control.Checkpoint(ProfileResetDiskCheckpointV1.AfterDefaultWrite); }
            public void FlushToStorage() { _control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeDefaultFlush); _inner.FlushToStorage(); _control.Checkpoint(ProfileResetDiskCheckpointV1.AfterDefaultFlush); }
            public void Close()
            {
                try { _control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeDefaultClose); }
                catch { _inner.Close(); throw; }
                _inner.Close(); _control.Checkpoint(ProfileResetDiskCheckpointV1.AfterDefaultClose);
            }
        }
    }
    // REQ-M5D7QC1-006/AC-009. This proof is process-local and one-consumer;
    // it deliberately grants no ordinary save, memory, UI, receipt or removal authority.
    internal sealed class ProfileResetDiskPreparedProofV1
    {
        private readonly string _root, _id, _markerHash, _targetHash; private readonly long _targetLength; private readonly byte[] _markerBytes, _targetBytes; private readonly Leaf[] _leaves; private readonly ValidatedProfileResetBarrierV1 _expectedMarker; private int _consumed;
        private ProfileResetDiskPreparedProofV1(string root, string id, string marker, byte[] target, Leaf[] leaves)
        {
            _root = root; _id = id; _markerBytes = Encoding.UTF8.GetBytes(marker); _targetBytes = (byte[])target.Clone(); _leaves = CloneLeaves(leaves); _markerHash = Hash(marker); _targetHash = Hash(target); _targetLength = target.LongLength;
            // Retain an unbound, independently deep-copied observation.  A
            // prepared proof is not a live lease or reset-save capability.
            _expectedMarker = ValidatedProfileResetBarrierV1.Create(root, id, marker, target, CloneLeaves(leaves));
        }
        internal static ProfileResetDiskPreparedProofV1 Create(ValidatedProfileResetBarrierV1 marker) { return new ProfileResetDiskPreparedProofV1(marker.Root, marker.Id, marker.Text, marker.Target, marker.Leaves); }
        // REQ-M5D7QC1-006/AC-M5D7QC1-009: observation never becomes authority
        // until its immutable payload has passed its closed invariant check.
        internal bool TryConsume()
        {
            ValidateInvariant();
            Validate(_expectedMarker);
            var observed = System.Threading.Volatile.Read(ref _consumed);
            if (observed != 0 && observed != 1) throw new InvalidOperationException("Prepared proof consumption state is malformed.");
            return System.Threading.Interlocked.CompareExchange(ref _consumed, 1, 0) == 0;
        }
        internal void ValidateInvariant()
        {
            if (string.IsNullOrEmpty(_root) || !string.Equals(_root, ProfileResetBarrierV1.NormalizeRoot(_root), StringComparison.OrdinalIgnoreCase)
                || _targetBytes == null || _markerBytes == null || _targetLength != _targetBytes.LongLength
                || _markerHash != Hash(_markerBytes) || _targetHash != Hash(_targetBytes)
                || _expectedMarker == null || _expectedMarker.IssuingLease != null)
                throw new InvalidOperationException("Prepared proof is malformed.");
        }
        internal void ValidateForRoot(string root)
        {
            ValidateInvariant();
            if (!string.Equals(ProfileResetBarrierV1.NormalizeRoot(root), _root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Prepared proof root is invalid.");
        }
        internal void ValidateArchive(ProfileResetDiskLeafProofV1[] archive) { if (_leaves == null || archive == null || _leaves.Length != 3 || archive.Length != 3) throw new InvalidOperationException("Prepared archive proof is malformed."); for (var i = 0; i < 3; i++) if (!archive[i].Matches(_leaves[i])) throw new InvalidOperationException("Prepared archive proof is malformed."); }
        internal void Validate(ValidatedProfileResetBarrierV1 marker)
        {
            if (marker == null || _root == null || !string.Equals(_root, marker.Root, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(_root, ProfileResetBarrierV1.NormalizeRoot(_root), StringComparison.OrdinalIgnoreCase)
                || _id != marker.Id || _targetBytes == null || _markerBytes == null
                || _targetLength != marker.Target.LongLength || _markerHash != Hash(marker.Text)
                || _targetHash != Hash(marker.Target) || !Bytes(_targetBytes, marker.Target)
                || !Bytes(_markerBytes, Encoding.UTF8.GetBytes(marker.Text))
                || !ProfileResetDiskTransactionV1.MarkerSnapshotIsCanonical(marker)
                || _leaves == null || marker.Leaves == null || _leaves.Length != 3 || marker.Leaves.Length != 3)
                throw new InvalidOperationException("Prepared proof is malformed.");
            for (var i = 0; i < 3; i++) if (!_leaves[i].Same(marker.Leaves[i])) throw new InvalidOperationException("Prepared proof is malformed.");
        }
        internal bool MatchesResult(string id, long length, string hash) { return _id == id && _targetLength == length && _targetHash == hash; }
        private static string Hash(string value) { return ProfileResetDiskTransactionV1.HashForProof(Encoding.UTF8.GetBytes(value)); }
        private static string Hash(byte[] value) { return ProfileResetDiskTransactionV1.HashForProof(value); }
        private static Leaf[] CloneLeaves(Leaf[] leaves)
        {
            if (leaves == null) return null;
            var copy = new Leaf[leaves.Length];
            for (var i = 0; i < leaves.Length; i++)
            {
                var leaf = leaves[i];
                if (leaf == null) return null;
                copy[i] = new Leaf(leaf.Role, leaf.Name, leaf.Presence, leaf.Classification, leaf.Length, leaf.Hash, leaf.Revision);
            }
            return copy;
        }
        private static bool Bytes(byte[] a, byte[] b) { if (a == null || b == null || a.Length != b.Length) return false; for (var i = 0; i < a.Length; i++) if (a[i] != b[i]) return false; return true; }
    }
    // REQ-M5D7QC2-001/005/006.  This is deliberately separate from the C1
    // prepared proof: it exists only while the *actual* reacquired lease is
    // held, and it is not a receipt or a general barrier-delete capability.
    internal sealed class ValidatedResetMemoryAuthorityV1
    {
        private readonly string _root, _transactionId, _markerSha256, _targetSha256, _archiveProofDigest;
        private readonly long _targetByteLength;
        private readonly byte[] _markerBytes, _targetBytes;
        private readonly Leaf[] _leaves;
        private readonly ProfileRootOperationLockV1 _issuingLease;
        private int _deleteAttempted;

        private ValidatedResetMemoryAuthorityV1(ProfileRootOperationLockV1 lease, ValidatedProfileResetBarrierV1 marker, string archiveProofDigest)
        {
            _root = marker.Root; _transactionId = marker.Id; _markerBytes = Encoding.UTF8.GetBytes(marker.Text);
            _targetBytes = (byte[])marker.Target.Clone(); _leaves = CloneLeaves(marker.Leaves); _issuingLease = lease;
            _markerSha256 = ProfileResetDiskTransactionV1.HashForProof(_markerBytes);
            _targetSha256 = ProfileResetDiskTransactionV1.HashForProof(_targetBytes);
            _targetByteLength = _targetBytes.LongLength; _archiveProofDigest = archiveProofDigest;
        }
        internal static ValidatedResetMemoryAuthorityV1 Create(ProfileRootOperationLockV1 lease, ValidatedProfileResetBarrierV1 marker, string archiveProofDigest)
        {
            if (lease == null || marker == null || archiveProofDigest == null) throw new ArgumentNullException(lease == null ? nameof(lease) : marker == null ? nameof(marker) : nameof(archiveProofDigest));
            return new ValidatedResetMemoryAuthorityV1(lease, marker, archiveProofDigest);
        }
        // These accessors validate only defensive in-memory state: detached
        // receipt getters must never perform disk I/O or retain this authority.
        internal string Root { get { ValidateInvariant(); return _root; } }
        internal string TransactionId { get { ValidateInvariant(); return _transactionId; } }
        internal string MarkerSha256 { get { ValidateInvariant(); return _markerSha256; } }
        internal string TargetSha256 { get { ValidateInvariant(); return _targetSha256; } }
        internal long TargetByteLength { get { ValidateInvariant(); return _targetByteLength; } }
        internal long TargetRevision { get { ValidateInvariant(); return 0; } }
        internal string ArchiveProofDigest { get { ValidateInvariant(); return _archiveProofDigest; } }
        internal byte[] CopyTargetBytes() { ValidateInvariant(); return (byte[])_targetBytes.Clone(); }
        internal byte[] CopyMarkerBytes() { ValidateInvariant(); return (byte[])_markerBytes.Clone(); }
        internal void ValidateForHeldLease(ProfileRootOperationLockV1 lease, string root)
        {
            ValidateInvariant();
            if (!ReferenceEquals(lease, _issuingLease) || !string.Equals(root, _root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Reset memory authority lease is invalid.");
            lease.ValidateOwnership(root);
        }
        internal void MarkDeleteAttempted()
        {
            ValidateInvariant();
            var observed = System.Threading.Volatile.Read(ref _deleteAttempted);
            if (observed != 0 && observed != 1) throw new InvalidOperationException("Reset memory authority delete state is malformed.");
            if (System.Threading.Interlocked.CompareExchange(ref _deleteAttempted, 1, 0) != 0) throw new InvalidOperationException("Authenticated barrier deletion cannot be retried.");
        }
        internal bool DeleteAttempted { get { ValidateInvariant(); return System.Threading.Volatile.Read(ref _deleteAttempted) == 1; } }
        internal void ValidateInvariant()
        {
            if (string.IsNullOrEmpty(_root) || !string.Equals(_root, ProfileResetBarrierV1.NormalizeRoot(_root), StringComparison.OrdinalIgnoreCase)
                || !IsId(_transactionId) || _markerBytes == null || _targetBytes == null || _leaves == null || _leaves.Length != 3 || _issuingLease == null
                || _targetByteLength != _targetBytes.LongLength || _markerSha256 != ProfileResetDiskTransactionV1.HashForProof(_markerBytes)
                || _targetSha256 != ProfileResetDiskTransactionV1.HashForProof(_targetBytes) || !IsHash(_markerSha256) || !IsHash(_targetSha256) || !IsHash(_archiveProofDigest)
                || _archiveProofDigest != ProfileResetDiskTransactionV1.ArchiveDigestForAuthority(_markerBytes, _leaves))
                throw new InvalidOperationException("Reset memory authority is malformed.");
            for (var i = 0; i < 3; i++) if (_leaves[i] == null || _leaves[i].Role != ProfileResetDiskTransactionV1.RoleAt(i) || _leaves[i].Name != ProfileResetDiskTransactionV1.NameAt(i)) throw new InvalidOperationException("Reset memory authority leaves are malformed.");
            var observed = System.Threading.Volatile.Read(ref _deleteAttempted); if (observed != 0 && observed != 1) throw new InvalidOperationException("Reset memory authority delete state is malformed.");
        }
        internal void ValidateMarker(ValidatedProfileResetBarrierV1 marker)
        {
            ValidateInvariant();
            if (marker == null || marker.Root != _root || marker.Id != _transactionId || !Bytes(_markerBytes, Encoding.UTF8.GetBytes(marker.Text)) || !Bytes(_targetBytes, marker.Target) || marker.Leaves == null || marker.Leaves.Length != 3) throw new InvalidOperationException("Reset memory authority marker is invalid.");
            for (var i = 0; i < 3; i++) if (!_leaves[i].Same(marker.Leaves[i])) throw new InvalidOperationException("Reset memory authority marker is invalid.");
        }
        internal Leaf[] CopyLeaves() { ValidateInvariant(); return CloneLeaves(_leaves); }
        private static Leaf[] CloneLeaves(Leaf[] leaves) { var copy = new Leaf[leaves.Length]; for (var i = 0; i < leaves.Length; i++) { var l = leaves[i]; if (l == null) throw new InvalidOperationException("Reset memory authority leaves are malformed."); copy[i] = new Leaf(l.Role, l.Name, l.Presence, l.Classification, l.Length, l.Hash, l.Revision); } return copy; }
        private static bool Bytes(byte[] a, byte[] b) { if (a == null || b == null || a.Length != b.Length) return false; for (var i = 0; i < a.Length; i++) if (a[i] != b[i]) return false; return true; }
        private static bool IsId(string id) { if (id == null || id.Length != 32) return false; for (var i = 0; i < id.Length; i++) if (!((id[i] >= '0' && id[i] <= '9') || (id[i] >= 'a' && id[i] <= 'f'))) return false; return true; }
        private static bool IsHash(string value) { if (value == null || value.Length != 64) return false; for (var i = 0; i < value.Length; i++) if (!((value[i] >= '0' && value[i] <= '9') || (value[i] >= 'a' && value[i] <= 'f'))) return false; return true; }
    }
    internal sealed class ProfileResetMemoryDiskAuthorityExceptionV1 : IOException
    {
        internal bool ManualRepairRequired { get; private set; }
        internal ProfileResetMemoryDiskAuthorityExceptionV1(bool manualRepairRequired, string message, Exception inner = null) : base(message, inner) { ManualRepairRequired = manualRepairRequired; }
    }
    // REQ-M5D7QC2-006. Detached, immutable final disk evidence.  It has no
    // lease, authority, delete, save, restore, or further-probe capability.
    internal sealed class ValidatedResetFinalDiskProofV1
    {
        private readonly string _root, _transactionId, _markerSha256, _targetSha256, _archiveProofDigest;
        private readonly byte[] _markerBytes, _targetBytes; private readonly Leaf[] _leaves;
        private readonly long _targetByteLength; private readonly bool _barrierAbsent, _barrierTempAbsent; private readonly object _mintWitness;
        private ValidatedResetFinalDiskProofV1(object witness, ValidatedResetMemoryAuthorityV1 authority)
        {
            _mintWitness = witness;
            _root = authority.Root; _transactionId = authority.TransactionId; _markerBytes = authority.CopyMarkerBytes(); _targetBytes = authority.CopyTargetBytes(); _leaves = authority.CopyLeaves();
            _markerSha256 = authority.MarkerSha256; _targetSha256 = authority.TargetSha256; _archiveProofDigest = authority.ArchiveProofDigest; _targetByteLength = authority.TargetByteLength;
            _barrierAbsent = true; _barrierTempAbsent = true;
        }
        // Candidate creation alone is not minting. Validate requires the
        // Profile transaction's private CWT registration after a real probe.
        internal static ValidatedResetFinalDiskProofV1 MintAfterProfileFinalProbe(object witness, ValidatedResetMemoryAuthorityV1 authority)
        {
            if (witness == null || authority == null) throw new InvalidOperationException("Final disk proof mint is not authorized.");
            return new ValidatedResetFinalDiskProofV1(witness, authority);
        }
        internal string Root { get { Validate(); return _root; } } internal string TransactionId { get { Validate(); return _transactionId; } }
        internal string MarkerSha256 { get { Validate(); return _markerSha256; } } internal string TargetSha256 { get { Validate(); return _targetSha256; } }
        internal string ArchiveProofDigest { get { Validate(); return _archiveProofDigest; } } internal long TargetByteLength { get { Validate(); return _targetByteLength; } }
        internal long TargetRevision { get { Validate(); return 0; } } internal bool BarrierAbsent { get { Validate(); return _barrierAbsent; } } internal bool BarrierTempAbsent { get { Validate(); return _barrierTempAbsent; } }
        internal byte[] CopyTargetBytes() { Validate(); return (byte[])_targetBytes.Clone(); }
        internal byte[] CopyMarkerBytes() { Validate(); return (byte[])_markerBytes.Clone(); }
        internal void Validate()
        {
            if (string.IsNullOrEmpty(_root) || !string.Equals(_root, ProfileResetBarrierV1.NormalizeRoot(_root), StringComparison.OrdinalIgnoreCase)
                || !IsId(_transactionId) || _markerBytes == null || _targetBytes == null || _leaves == null || _leaves.Length != 3 || _targetByteLength != _targetBytes.LongLength
                || !_barrierAbsent || !_barrierTempAbsent || _markerSha256 != ProfileResetDiskTransactionV1.HashForProof(_markerBytes) || _targetSha256 != ProfileResetDiskTransactionV1.HashForProof(_targetBytes)
                || _archiveProofDigest != ProfileResetDiskTransactionV1.ArchiveDigestForAuthority(_markerBytes, _leaves) || !IsHash(_markerSha256) || !IsHash(_targetSha256) || !IsHash(_archiveProofDigest)
                || !ProfileResetDiskTransactionV1.IsRegisteredFinalProof(this, _mintWitness, _root)) throw new InvalidOperationException("Final disk proof is malformed.");
            for (var i = 0; i < 3; i++) if (_leaves[i] == null || _leaves[i].Role != ProfileResetDiskTransactionV1.RoleAt(i) || _leaves[i].Name != ProfileResetDiskTransactionV1.NameAt(i)) throw new InvalidOperationException("Final disk proof leaves are malformed.");
            ProfileResetDiskTransactionV1.ValidateDetachedFinalProof(_root, _transactionId, _markerBytes, _targetBytes, _leaves);
        }
        private static bool IsId(string id) { if (id == null || id.Length != 32) return false; for (var i = 0; i < id.Length; i++) if (!((id[i] >= '0' && id[i] <= '9') || (id[i] >= 'a' && id[i] <= 'f'))) return false; return true; }
        private static bool IsHash(string value) { if (value == null || value.Length != 64) return false; for (var i = 0; i < value.Length; i++) if (!((value[i] >= '0' && value[i] <= '9') || (value[i] >= 'a' && value[i] <= 'f'))) return false; return true; }
    }
    // Closed observation/throw seam: it never supplies a disk value or alters
    // the direct BCL deletion algorithm.
    internal enum ProfileResetMemoryAuthorityCheckpointV1 { BeforeProofCas = 1, AfterProofCas = 2, BeforeBarrierDelete = 3, AfterBarrierDelete = 4, BeforeBarrierAbsenceProbe = 5, AfterBarrierAbsenceProbe = 6, BeforeFinalDiskProbe = 7, AfterFinalDiskProbe = 8 }
    internal interface IProfileResetMemoryAuthorityTestControlV1 { void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint); }
    internal sealed class ProfileResetMemoryAuthorityNoOpTestControlV1 : IProfileResetMemoryAuthorityTestControlV1 { internal static readonly ProfileResetMemoryAuthorityNoOpTestControlV1 Instance = new ProfileResetMemoryAuthorityNoOpTestControlV1(); private ProfileResetMemoryAuthorityNoOpTestControlV1() { } public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint) { } }
    internal readonly struct ProfileResetDiskResultV1
    {
        private readonly ProfileResetDiskOutcomeV1 _outcome; private readonly ProfileResetDurableStateV1 _state; private readonly string _transactionId; private readonly long _targetRevision, _targetLength; private readonly string _targetHash; private readonly ProfileResetDiskPreparedProofV1 _proof; private readonly ValidatedProfileResetBarrierV1 _marker; private readonly ProfileResetDiskLeafProofV1[] _archive, _active; private readonly ProfileResetDiskArchiveSnapshotV1[] _archiveSnapshots; private readonly ProfileResetDiskActiveSnapshotV1[] _activeSnapshots; private readonly ProfileResetDiskFailureSourceV1 _failureSource;
        internal ProfileResetDiskResultV1(ProfileResetDiskOutcomeV1 outcome, ProfileResetDurableStateV1 state, string id, long targetRevision, long targetLength, string targetHash, ProfileResetDiskPreparedProofV1 proof, ValidatedProfileResetBarrierV1 marker, ProfileResetDiskLeafProofV1[] archive, ProfileResetDiskLeafProofV1[] active, ProfileResetDiskArchiveSnapshotV1[] archiveSnapshots, ProfileResetDiskActiveSnapshotV1[] activeSnapshots, ProfileResetDiskFailureSourceV1 failureSource) { _outcome = outcome; _state = state; _transactionId = id; _targetRevision = targetRevision; _targetLength = targetLength; _targetHash = targetHash; _proof = proof; _marker = marker; _archive = archive == null ? null : (ProfileResetDiskLeafProofV1[])archive.Clone(); _active = active == null ? null : (ProfileResetDiskLeafProofV1[])active.Clone(); _archiveSnapshots = archiveSnapshots == null ? null : (ProfileResetDiskArchiveSnapshotV1[])archiveSnapshots.Clone(); _activeSnapshots = activeSnapshots == null ? null : (ProfileResetDiskActiveSnapshotV1[])activeSnapshots.Clone(); _failureSource = failureSource; }
        public ProfileResetDiskOutcomeV1 Outcome { get { Validate(); return _outcome; } } public ProfileResetDurableStateV1 DurableState { get { Validate(); return _state; } } public string TransactionId { get { Validate(); return _transactionId; } }
        public long? TargetRevision { get { Validate(); return _transactionId == null ? (long?)null : _targetRevision; } } public long? TargetByteLength { get { Validate(); return _transactionId == null ? (long?)null : _targetLength; } } public string TargetSha256 { get { Validate(); return _targetHash; } }
        public ProfileResetDiskArchiveSnapshotV1[] ArchiveSnapshots { get { Validate(); return (ProfileResetDiskArchiveSnapshotV1[])_archiveSnapshots.Clone(); } } public ProfileResetDiskActiveSnapshotV1[] ActiveSnapshots { get { Validate(); return (ProfileResetDiskActiveSnapshotV1[])_activeSnapshots.Clone(); } }
        internal ProfileResetDiskFailureSourceV1 FailureSource { get { Validate(); return _failureSource; } }
        internal ProfileResetDiskPreparedProofV1 PreparedProof { get { Validate(); if (_proof == null) throw new InvalidOperationException("Prepared proof is unavailable."); return _proof; } }
        public void Validate() { if (_outcome < ProfileResetDiskOutcomeV1.DiskPrepared || _outcome > ProfileResetDiskOutcomeV1.ManualRepairRequired || _state < ProfileResetDurableStateV1.NoBarrier || _state > ProfileResetDurableStateV1.ManualRepairRequired || !ClosedPair(_outcome, _state) || _archiveSnapshots == null || _activeSnapshots == null || _archiveSnapshots.Length != 3 || _activeSnapshots.Length != 3 || _failureSource < ProfileResetDiskFailureSourceV1.None || _failureSource > ProfileResetDiskFailureSourceV1.Snapshot || (_failureSource == ProfileResetDiskFailureSourceV1.None && _outcome != ProfileResetDiskOutcomeV1.DiskPrepared) || (_transactionId == null && (_targetRevision != -1 || _targetLength != -1 || _targetHash != null || _marker != null)) || (_transactionId != null && (!IsId(_transactionId) || _targetRevision != 0 || _targetLength < 0 || !IsHash(_targetHash))) || (_outcome == ProfileResetDiskOutcomeV1.DiskPrepared && (_proof == null || _marker == null || _archive == null || _active == null || _archive.Length != 3 || _active.Length != 3 || _failureSource != ProfileResetDiskFailureSourceV1.None)) || (_outcome != ProfileResetDiskOutcomeV1.DiskPrepared && _proof != null)) throw new InvalidOperationException("Reset disk result is malformed."); if (_transactionId != null && (_marker == null || _marker.Id != _transactionId || _marker.Target.LongLength != _targetLength || ProfileResetDiskTransactionV1.HashForProof(_marker.Target) != _targetHash || !ProfileResetDiskTransactionV1.MarkerSnapshotIsCanonical(_marker))) throw new InvalidOperationException("Authenticated result scalars are malformed."); for (var i = 0; i < 3; i++) { _archiveSnapshots[i].Validate(); _activeSnapshots[i].Validate(); if (!_archiveSnapshots[i].HasRoleName(ProfileResetDiskTransactionV1.RoleAt(i), ProfileResetDiskTransactionV1.NameAt(i)) || !_activeSnapshots[i].HasRoleName(ProfileResetDiskTransactionV1.RoleAt(i), ProfileResetDiskTransactionV1.NameAt(i))) throw new InvalidOperationException("Snapshot role order is malformed."); if (_marker != null) ValidateAuthenticatedArchiveSnapshot(_marker.Leaves[i], _archiveSnapshots[i]); if (_outcome == ProfileResetDiskOutcomeV1.DiskPrepared && _marker.Leaves[i].Presence == ProfileResetLeafPresenceV1.Present && (_archiveSnapshots[i].ByteLength != _marker.Leaves[i].Length || _archiveSnapshots[i].Sha256 != _marker.Leaves[i].Hash)) throw new InvalidOperationException("Prepared archive snapshot is malformed."); } if (_transactionId == null) for (var i = 0; i < 3; i++) if (_archiveSnapshots[i].Proof != ProfileResetDiskArchiveProofV1.NotObserved) throw new InvalidOperationException("Unauthenticated archive path was observed."); if (_proof != null) { _proof.ValidateInvariant(); _proof.Validate(_marker); if (!_proof.MatchesResult(_transactionId, _targetLength, _targetHash)) throw new InvalidOperationException("Prepared result scalars are malformed."); _proof.ValidateArchive(_archive); } if (_archive != null) foreach (var x in _archive) x.Validate(); if (_active != null) foreach (var x in _active) x.Validate(); if (_outcome == ProfileResetDiskOutcomeV1.DiskPrepared && (!_active[0].HasRoleName("Primary", "profile.json") || !_active[1].HasRoleName("Previous", "profile.prev.json") || !_active[2].HasRoleName("Temp", "profile.tmp.json") || _active[0].Presence != ProfileResetLeafPresenceV1.Present || _active[0].ByteLength != _targetLength || _active[0].Sha256 != _targetHash || _active[1].Presence != ProfileResetLeafPresenceV1.Missing || _active[2].Presence != ProfileResetLeafPresenceV1.Missing || (_marker.Leaves[0].Presence == ProfileResetLeafPresenceV1.Present ? _archiveSnapshots[0].Proof != ProfileResetDiskArchiveProofV1.Exact : _archiveSnapshots[0].Proof != ProfileResetDiskArchiveProofV1.NotApplicable) || (_marker.Leaves[1].Presence == ProfileResetLeafPresenceV1.Present ? _archiveSnapshots[1].Proof != ProfileResetDiskArchiveProofV1.Exact : _archiveSnapshots[1].Proof != ProfileResetDiskArchiveProofV1.NotApplicable) || (_marker.Leaves[2].Presence == ProfileResetLeafPresenceV1.Present ? _archiveSnapshots[2].Proof != ProfileResetDiskArchiveProofV1.Exact : _archiveSnapshots[2].Proof != ProfileResetDiskArchiveProofV1.NotApplicable) || _activeSnapshots[0].Observation != ProfileResetDiskObservationV1.Readable || _activeSnapshots[0].Classification != ProfileResetLeafClassificationV1.ValidCurrentInput || _activeSnapshots[0].DecodedRevision != 0 || _activeSnapshots[0].ByteLength != _targetLength || _activeSnapshots[0].Sha256 != _targetHash || _activeSnapshots[1].Observation != ProfileResetDiskObservationV1.Missing || _activeSnapshots[2].Observation != ProfileResetDiskObservationV1.Missing)) throw new InvalidOperationException("Prepared active state is malformed."); }
        private static void ValidateAuthenticatedArchiveSnapshot(Leaf declared, ProfileResetDiskArchiveSnapshotV1 snapshot) { if (declared.Presence == ProfileResetLeafPresenceV1.Missing) { if (snapshot.Proof == ProfileResetDiskArchiveProofV1.NotApplicable || snapshot.Proof == ProfileResetDiskArchiveProofV1.Mismatch || snapshot.Proof == ProfileResetDiskArchiveProofV1.Collision || snapshot.Proof == ProfileResetDiskArchiveProofV1.Unreadable) return; throw new InvalidOperationException("Missing declared archive is malformed."); } if (snapshot.Proof == ProfileResetDiskArchiveProofV1.NotApplicable) throw new InvalidOperationException("Present declared archive is malformed."); if (snapshot.Proof == ProfileResetDiskArchiveProofV1.Exact && (snapshot.Classification != declared.Classification || snapshot.ByteLength != declared.Length || snapshot.Sha256 != declared.Hash || snapshot.DecodedRevision != declared.Revision)) throw new InvalidOperationException("Exact archive fingerprint is malformed."); }
        private static bool ClosedPair(ProfileResetDiskOutcomeV1 outcome, ProfileResetDurableStateV1 state) { return (outcome == ProfileResetDiskOutcomeV1.DiskPrepared && state == ProfileResetDurableStateV1.DiskPrepared) || (outcome == ProfileResetDiskOutcomeV1.ConfirmationStale && state == ProfileResetDurableStateV1.NoBarrier) || (outcome == ProfileResetDiskOutcomeV1.Busy && state == ProfileResetDurableStateV1.NoBarrier) || (outcome == ProfileResetDiskOutcomeV1.ReloadRequired && state == ProfileResetDurableStateV1.DefaultCommitUncertain) || (outcome == ProfileResetDiskOutcomeV1.ManualRepairRequired && state == ProfileResetDurableStateV1.ManualRepairRequired); }
        private static bool IsId(string id) { if (id == null || id.Length != 32) return false; for (var i = 0; i < id.Length; i++) if (!((id[i] >= '0' && id[i] <= '9') || (id[i] >= 'a' && id[i] <= 'f'))) return false; return true; }
        private static bool IsHash(string value) { if (value == null || value.Length != 64) return false; for (var i = 0; i < value.Length; i++) if (!((value[i] >= '0' && value[i] <= '9') || (value[i] >= 'a' && value[i] <= 'f'))) return false; return true; }
    }

    internal static class ProfileResetDiskTransactionV1
    {
        private const string Recovery = "recovery", NewGame = "new-game";
        private static readonly string[] Roles = { "Primary", "Previous", "Temp" };
        private static readonly string[] Names = { "profile.json", "profile.prev.json", "profile.tmp.json" };
        private sealed class ConfirmationCaptureWitness
        {
            internal readonly ProfileResetConfirmationIdentityV1 Identity; internal readonly Leaf[] Leaves; internal readonly ProfileCanonicalDecodeResultV1? Primary, Previous, Temp;
            internal ConfirmationCaptureWitness(ProfileResetConfirmationIdentityV1 identity, Leaf[] leaves, ProfileCanonicalDecodeResultV1? primary, ProfileCanonicalDecodeResultV1? previous, ProfileCanonicalDecodeResultV1? temp) { Identity=identity; Leaves=Copy(leaves); Primary=primary; Previous=previous; Temp=temp; }
        }
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<ProfileResetConfirmationCaptureV1, ConfirmationCaptureWitness> IssuedConfirmationCaptures = new System.Runtime.CompilerServices.ConditionalWeakTable<ProfileResetConfirmationCaptureV1, ConfirmationCaptureWitness>();
        internal static bool ValidateIssuedConfirmationCapture(ProfileResetConfirmationCaptureV1 capture)
        {
            if (capture == null || !IssuedConfirmationCaptures.TryGetValue(capture, out var witness) || witness == null || !ReferenceEquals(capture.Identity, witness.Identity) || capture.Identity.Leaves == null || capture.Identity.Leaves.Length != 3) return false;
            var values = new ProfileCanonicalDecodeResultV1?[] { capture.Primary, capture.Previous, capture.Temp }; var expected = new ProfileCanonicalDecodeResultV1?[] { witness.Primary, witness.Previous, witness.Temp };
            for (var i=0;i<3;i++) if (!SameLeaf(capture.Identity.Leaves[i], witness.Leaves[i]) || values[i].HasValue != expected[i].HasValue || (values[i].HasValue && !values[i].Value.Equals(expected[i].Value))) return false;
            return true;
        }
        private static void RegisterIssuedConfirmationCapture(ProfileResetConfirmationCaptureV1 capture, ProfileResetConfirmationIdentityV1 identity, Leaf[] leaves, ProfileCanonicalDecodeResultV1? primary, ProfileCanonicalDecodeResultV1? previous, ProfileCanonicalDecodeResultV1? temp) { IssuedConfirmationCaptures.Add(capture, new ConfirmationCaptureWitness(identity, leaves, primary, previous, temp)); }
        private static Leaf[] Copy(Leaf[] source) { if (source == null || source.Length != 3) throw new ArgumentException("Observation leaves are malformed.", nameof(source)); var copy = new Leaf[3]; for (var i=0;i<3;i++) { var x=source[i]; if(x==null) throw new ArgumentException("Observation leaf is absent.",nameof(source)); copy[i]=new Leaf(x.Role,x.Name,x.Presence,x.Classification,x.Length,x.Hash,x.Revision); } return copy; }
        private static readonly string[] Archives = { "primary.bin", "previous.bin", "temp.bin" };
        private static readonly UTF8Encoding Utf8 = new UTF8Encoding(false, true);
        private static readonly object FinalProofMintWitness = new object();
        private static readonly ConditionalWeakTable<ValidatedResetFinalDiskProofV1, FinalProofMintRecord> FinalProofMints = new ConditionalWeakTable<ValidatedResetFinalDiskProofV1, FinalProofMintRecord>();
        private sealed class FinalProofMintRecord { internal readonly object Witness; internal readonly string Root; internal FinalProofMintRecord(object witness, string root) { Witness = witness; Root = root; } }
        internal static string RoleAt(int index) { return Roles[index]; }
        internal static string NameAt(int index) { return Names[index]; }
        private static void RegisterFinalProof(ValidatedResetFinalDiskProofV1 proof, object witness, string root)
        {
            if (proof == null || !ReferenceEquals(witness, FinalProofMintWitness) || string.IsNullOrEmpty(root) || !string.Equals(root, ProfileResetBarrierV1.NormalizeRoot(root), StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Final disk proof mint is not authorized.");
            FinalProofMints.Add(proof, new FinalProofMintRecord(witness, root));
        }
        internal static bool IsRegisteredFinalProof(ValidatedResetFinalDiskProofV1 proof, object witness, string root)
        {
            FinalProofMintRecord record;
            return proof != null && ReferenceEquals(witness, FinalProofMintWitness) && FinalProofMints.TryGetValue(proof, out record) && record != null && ReferenceEquals(record.Witness, FinalProofMintWitness) && string.Equals(root, record.Root, StringComparison.OrdinalIgnoreCase);
        }
        internal static void ValidateDetachedFinalProof(string root, string id, byte[] markerBytes, byte[] target, Leaf[] leaves)
        {
            if (markerBytes == null || target == null || leaves == null || leaves.Length != 3 || !Bytes(target, DefaultBytes())) throw new InvalidOperationException("Final disk proof target is malformed.");
            var text = Utf8.GetString(markerBytes);
            if (!string.Equals(text, MarkerText(id, target, leaves), StringComparison.Ordinal)) throw new InvalidOperationException("Final disk proof marker is malformed.");
        }
        internal static string ArchiveDigestForAuthority(byte[] markerBytes, Leaf[] leaves)
        {
            if (markerBytes == null || leaves == null || leaves.Length != 3) throw new InvalidOperationException("Reset archive proof is malformed.");
            var text = Utf8.GetString(markerBytes);
            for (var i = 0; i < 3; i++)
            {
                var leaf = leaves[i];
                if (leaf == null) throw new InvalidOperationException("Reset archive proof is malformed.");
                text += "|" + leaf.Role + ":" + leaf.Name + ":" + leaf.Presence + ":" + leaf.Classification + ":" + leaf.Length + ":" + leaf.Hash + ":" + leaf.Revision;
            }
            return Hash(Utf8.GetBytes(text));
        }

        // REQ-M5D7QC2-001: C2 invokes this before Acquire so a foreign or
        // reparse root cannot cause the lock service to create a lock file.
        internal static string ValidateMemoryResetRootBeforeLease(string root)
        {
            var normalized = CheckRoot(root); VerifyBeforeLease(normalized);
            var lockPath = Path.Combine(normalized, "profile.operation.lock");
            try
            {
                var attributes = File.GetAttributes(lockPath);
                if ((attributes & (FileAttributes.Directory | FileAttributes.ReparsePoint)) != 0) throw new IOException("Profile operation lock path is unsafe.");
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }
            return normalized;
        }

        // REQ-M5D7QC2-001.  There is intentionally no result object here:
        // callers either receive a lease-bound capability or a closed failure.
        internal static ValidatedResetMemoryAuthorityV1 ReauthenticatePreparedUnderHeldRoot(ProfileRootOperationLockV1 lease, string root, ProfileResetDiskPreparedProofV1 proof)
        {
            return ReauthenticatePreparedUnderHeldRoot(lease, root, proof, ProfileResetMemoryAuthorityNoOpTestControlV1.Instance);
        }
        internal static ValidatedResetMemoryAuthorityV1 ReauthenticatePreparedUnderHeldRoot(ProfileRootOperationLockV1 lease, string root, ProfileResetDiskPreparedProofV1 proof, IProfileResetMemoryAuthorityTestControlV1 control)
        {
            if (lease == null || proof == null || control == null) throw new ArgumentNullException(lease == null ? nameof(lease) : proof == null ? nameof(proof) : nameof(control));
            try
            {
                var normalized = ProfileResetBarrierV1.NormalizeRoot(root);
                proof.ValidateForRoot(normalized); lease.ValidateOwnership(normalized);
                var live = ReadMarker(normalized); proof.Validate(live);
                VerifyPreparedDiskRow(live, true);
                control.Checkpoint(ProfileResetMemoryAuthorityCheckpointV1.BeforeProofCas);
                if (!proof.TryConsume()) throw new InvalidOperationException("Prepared proof was already consumed.");
                control.Checkpoint(ProfileResetMemoryAuthorityCheckpointV1.AfterProofCas);
                return ValidatedResetMemoryAuthorityV1.Create(lease, live, ArchiveProofDigest(live));
            }
            catch (ProfileResetMemoryDiskAuthorityExceptionV1) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception e) when (Recoverable(e)) { throw new ProfileResetMemoryDiskAuthorityExceptionV1(true, "Prepared reset disk state requires manual repair.", e); }
        }

        // REQ-M5D7QC2-005: direct BCL deletion is one-shot.  The attempt is
        // recorded before File.Delete so a post-delete checkpoint/fault cannot
        // make the same authority retry or claim that deletion was not tried.
        internal static void RemoveAuthenticatedBarrierUnderHeldRoot(ProfileRootOperationLockV1 lease, ValidatedResetMemoryAuthorityV1 authority)
        {
            RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority, ProfileResetMemoryAuthorityNoOpTestControlV1.Instance);
        }
        internal static void RemoveAuthenticatedBarrierUnderHeldRoot(ProfileRootOperationLockV1 lease, ValidatedResetMemoryAuthorityV1 authority, IProfileResetMemoryAuthorityTestControlV1 control)
        {
            if (lease == null || authority == null || control == null) throw new ArgumentNullException(lease == null ? nameof(lease) : authority == null ? nameof(authority) : nameof(control));
            try
            {
                authority.ValidateForHeldLease(lease, authority.Root);
                // A successful direct delete may be followed by a throwing
                // checkpoint or uncertain probe.  Reject the same authority
                // before any marker I/O so that state can never be retried.
                if (authority.DeleteAttempted) throw new InvalidOperationException("Authenticated barrier deletion cannot be retried.");
                var live = ReadMarker(authority.Root); authority.ValidateMarker(live);
                VerifyPreparedDiskRow(live, true);
                authority.MarkDeleteAttempted(); control.Checkpoint(ProfileResetMemoryAuthorityCheckpointV1.BeforeBarrierDelete);
                File.Delete(Path.Combine(authority.Root, ProfileResetBarrierV1.BarrierName));
                control.Checkpoint(ProfileResetMemoryAuthorityCheckpointV1.AfterBarrierDelete);
                control.Checkpoint(ProfileResetMemoryAuthorityCheckpointV1.BeforeBarrierAbsenceProbe);
                if (Exists(Path.Combine(authority.Root, ProfileResetBarrierV1.BarrierName)) || Exists(Path.Combine(authority.Root, ProfileResetBarrierV1.BarrierTempName))) throw new IOException("Reset barrier absence is unproven.");
                control.Checkpoint(ProfileResetMemoryAuthorityCheckpointV1.AfterBarrierAbsenceProbe);
            }
            catch (ProfileResetMemoryDiskAuthorityExceptionV1) { throw; }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is SecurityException) { throw new ProfileResetMemoryDiskAuthorityExceptionV1(false, "Authenticated reset barrier removal is uncertain.", e); }
        }

        // REQ-M5D7QC2-006: after deletion, the same immutable authority is
        // checked against the exact marker-free row.  It never recreates data.
        internal static void VerifyFinalDiskUnderHeldRoot(ProfileRootOperationLockV1 lease, ValidatedResetMemoryAuthorityV1 authority)
        {
            CaptureFinalDiskProofUnderHeldRoot(lease, authority, ProfileResetMemoryAuthorityNoOpTestControlV1.Instance);
        }
        internal static void VerifyFinalDiskUnderHeldRoot(ProfileRootOperationLockV1 lease, ValidatedResetMemoryAuthorityV1 authority, IProfileResetMemoryAuthorityTestControlV1 control)
        {
            CaptureFinalDiskProofUnderHeldRoot(lease, authority, control);
        }
        internal static ValidatedResetFinalDiskProofV1 CaptureFinalDiskProofUnderHeldRoot(ProfileRootOperationLockV1 lease, ValidatedResetMemoryAuthorityV1 authority)
        {
            return CaptureFinalDiskProofUnderHeldRoot(lease, authority, ProfileResetMemoryAuthorityNoOpTestControlV1.Instance);
        }
        internal static ValidatedResetFinalDiskProofV1 CaptureFinalDiskProofUnderHeldRoot(ProfileRootOperationLockV1 lease, ValidatedResetMemoryAuthorityV1 authority, IProfileResetMemoryAuthorityTestControlV1 control)
        {
            if (lease == null || authority == null || control == null) throw new ArgumentNullException(lease == null ? nameof(lease) : authority == null ? nameof(authority) : nameof(control));
            try
            {
                authority.ValidateForHeldLease(lease, authority.Root);
                if (!authority.DeleteAttempted) throw new InvalidOperationException("Final reset disk verification requires a deletion attempt.");
                control.Checkpoint(ProfileResetMemoryAuthorityCheckpointV1.BeforeFinalDiskProbe);
                VerifyFinalDiskRow(authority);
                control.Checkpoint(ProfileResetMemoryAuthorityCheckpointV1.AfterFinalDiskProbe);
                var proof = ValidatedResetFinalDiskProofV1.MintAfterProfileFinalProbe(FinalProofMintWitness, authority);
                RegisterFinalProof(proof, FinalProofMintWitness, authority.Root);
                proof.Validate();
                return proof;
            }
            catch (ProfileResetMemoryDiskAuthorityExceptionV1) { throw; }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is SecurityException) { throw new ProfileResetMemoryDiskAuthorityExceptionV1(false, "Final reset disk state is uncertain.", e); }
        }

        // REQ-M5D7QC1-001. Capture is observation only; it creates no marker.
        internal static ProfileResetConfirmationIdentityV1 CaptureConfirmationIdentity(string root)
        {
            var normalized = CheckRoot(root); VerifyBeforeLease(normalized);
            try { using (ProfileRootOperationLockV1.Acquire(normalized)) { ProfileResetBarrierV1.ThrowIfPresent(normalized); return ProfileResetConfirmationIdentityV1.Create(normalized, Observe(normalized)); } }
            catch (ProfileRootOperationBlockedExceptionV1 e) { throw new InvalidOperationException("Confirmation capture is blocked: " + e.Outcome, e); }
        }
        // C3L observation-only counterpart.  Existing CaptureConfirmationIdentity
        // remains byte-for-byte independent so legacy Busy behavior is unchanged.
        internal static ProfileResetConfirmationCaptureV1 CaptureConfirmationObservation(string root)
        {
            var normalized = CheckRoot(root); VerifyBeforeLease(normalized);
            using (var lease = ProfileRootOperationLockV1.AcquireForObservation(normalized))
            {
                // Revalidate containment while owning the real C1 lease before
                // the barrier probe and the exactly-three leaf reads.
                normalized = CheckRoot(normalized); lease.ValidateOwnership(normalized);
                ProfileResetBarrierV1.ThrowIfPresent(normalized);
                var leaves = new Leaf[3];
                var decoded = new ProfileCanonicalDecodeResultV1?[3];
                for (var i = 0; i < 3; i++)
                {
                    var path = Path.Combine(normalized, Names[i]);
                    if (!Exists(path)) { leaves[i] = new Leaf(Roles[i], Names[i], ProfileResetLeafPresenceV1.Missing, null, null, null, null); continue; }
                    var bytes = Read(path); var value = ProfileCanonicalDecoderV1.Decode(bytes); decoded[i] = value;
                    ProfileResetLeafClassificationV1 classification; long? revision = null;
                    if (value.Classification == ProfileCanonicalDecodeClassification.ValidCurrentInput) { classification = ProfileResetLeafClassificationV1.ValidCurrentInput; revision = value.Document.Snapshot.ProfileRevision; }
                    else if (value.Classification == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired || value.Classification == ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired) { classification = ProfileResetLeafClassificationV1.ValidInputRecoveryRequired; revision = value.RecoveryProjection.SourceProfileRevision; }
                    else if (value.Classification == ProfileCanonicalDecodeClassification.UnsupportedProfileSchema) classification = ProfileResetLeafClassificationV1.UnsupportedSchema;
                    else classification = ProfileResetLeafClassificationV1.Invalid;
                    leaves[i] = new Leaf(Roles[i], Names[i], ProfileResetLeafPresenceV1.Present, classification, bytes.LongLength, Hash(bytes), revision);
                }
                var identity = ProfileResetConfirmationIdentityV1.Create(normalized, leaves);
                var capture = new ProfileResetConfirmationCaptureV1(identity, decoded[0], decoded[1], decoded[2]);
                RegisterIssuedConfirmationCapture(capture, identity, leaves, decoded[0], decoded[1], decoded[2]);
                return capture;
            }
        }
        // REQ-M5D7QC1-001..006. A confirmed caller owns invocation; no UI consent is represented here.
        internal static ProfileResetDiskResultV1 Begin(string root, ProfileResetConfirmationIdentityV1 identity)
        {
            return Begin(root, identity, ProfileResetDiskNoOpTestControlV1.Instance);
        }
        internal static ProfileResetDiskResultV1 Begin(string root, ProfileResetConfirmationIdentityV1 identity, IProfileResetDiskTestControlV1 control)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity)); if (control == null) throw new ArgumentNullException(nameof(control));
            var normalized = ProfileResetBarrierV1.NormalizeRoot(root);
            if (!string.Equals(normalized, identity.Root, StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Confirmation root does not match.", nameof(identity));
            try
            {
                CheckRoot(normalized); VerifyBeforeLease(normalized);
                using (var lease = ProfileRootOperationLockV1.Acquire(normalized))
                {
                    ValidatedProfileResetBarrierV1 marker = null;
                    ProfileResetDiskFailureSourceV1 phase = ProfileResetDiskFailureSourceV1.FreshObservation;
                    try
                    {
                        if (!identity.TryConsume()) throw new InvalidOperationException("Confirmation identity is single-use.");
                        ProfileResetBarrierV1.ThrowIfPresent(normalized);
                        var fresh = Observe(normalized); if (!Same(identity.Leaves, fresh)) return NoAuthorityResult(ProfileResetDiskOutcomeV1.ConfirmationStale, ProfileResetDurableStateV1.NoBarrier, normalized, true, phase);
                        var target = DefaultBytes(); var id = NewId(); marker = Marker(normalized, id, target, fresh); var tx = TransactionDirectory(normalized, id);
                        phase = ProfileResetDiskFailureSourceV1.Directory; CheckCreatePath(normalized, tx); if (Directory.Exists(tx) || File.Exists(tx)) throw new IOException("Transaction directory collision."); Directory.CreateDirectory(tx);
                        phase = ProfileResetDiskFailureSourceV1.MarkerPublication; Publish(normalized, marker, control); marker = ValidatedProfileResetBarrierV1.BindLease(marker, lease);
                        return ResumeHeld(lease, marker, control);
                    }
                    catch (Exception e) when (Recoverable(e))
                    {
                        var authenticated = AuthenticateFailureMarker(lease, normalized, marker);
                        return authenticated == null ? NoAuthorityResult(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, normalized, true, phase) : Result(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, authenticated, phase);
                    }
                }
            }
            catch (ProfileRootOperationBlockedExceptionV1 e) { return e.Outcome == ProfileRootOperationOutcomeV1.Busy ? NoAuthorityResult(ProfileResetDiskOutcomeV1.Busy, ProfileResetDurableStateV1.NoBarrier, null, false, ProfileResetDiskFailureSourceV1.Lease) : NoAuthorityResult(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, null, false, ProfileResetDiskFailureSourceV1.Lease); }
            // No lease was acquired or validated here: rejected-root and
            // pre-lease failures must not read the untrusted root for a result.
            catch (Exception e) when (Recoverable(e)) { return NoAuthorityResult(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, null, false, ProfileResetDiskFailureSourceV1.Lease); }
        }
        internal static ProfileResetDiskResultV1 Resume(string root)
        {
            return Resume(root, ProfileResetDiskNoOpTestControlV1.Instance);
        }
        internal static ProfileResetDiskResultV1 Resume(string root, IProfileResetDiskTestControlV1 control)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            var normalized = ProfileResetBarrierV1.NormalizeRoot(root);
            try { CheckRoot(normalized); VerifyBeforeLease(normalized); using (var lease = ProfileRootOperationLockV1.Acquire(normalized)) { ValidatedProfileResetBarrierV1 marker = null; try { marker = ValidatedProfileResetBarrierV1.BindLease(ReadMarker(normalized), lease); return ResumeHeld(lease, marker, control); } catch (Exception e) when (Recoverable(e)) { return marker == null ? NoAuthorityResult(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, normalized, true, ProfileResetDiskFailureSourceV1.MarkerValidation) : Result(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, marker, ProfileResetDiskFailureSourceV1.MarkerValidation); } } }
            catch (ProfileRootOperationBlockedExceptionV1 e) { return e.Outcome == ProfileRootOperationOutcomeV1.Busy ? NoAuthorityResult(ProfileResetDiskOutcomeV1.Busy, ProfileResetDurableStateV1.NoBarrier, null, false, ProfileResetDiskFailureSourceV1.Lease) : NoAuthorityResult(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, null, false, ProfileResetDiskFailureSourceV1.Lease); }
            // Resume has no authority to inspect a root when pre-lease root
            // validation itself failed.
            catch (Exception e) when (Recoverable(e)) { return NoAuthorityResult(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, null, false, ProfileResetDiskFailureSourceV1.MarkerValidation); }
        }
        private static ProfileResetDiskResultV1 ResumeHeld(ProfileRootOperationLockV1 lease, ValidatedProfileResetBarrierV1 marker, IProfileResetDiskTestControlV1 control)
        {
            var phase = ProfileResetDiskFailureSourceV1.Directory;
            try
            {
            // AC-M5D7QC1-003/004/007: inspect all old destinations before any move.
            // An absent transaction directory is never inferred as an empty
            // archive, even when all marker leaves were originally Missing.
            var transactionDirectory = TransactionDirectory(marker.Root, marker.Id);
            CheckCreatePath(marker.Root, transactionDirectory);
            if (!Directory.Exists(transactionDirectory) || (File.GetAttributes(transactionDirectory) & FileAttributes.ReparsePoint) != 0)
                return Result(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, marker, ProfileResetDiskFailureSourceV1.Directory);
            var complete = true; for (var i = 0; i < 3; i++) { phase = ArchiveFailureSource(i); if (!ArchiveComplete(marker, i)) complete = false; }
            if (!complete)
            {
                for (var i = 0; i < 3; i++) { phase = ArchiveFailureSource(i); if (!OldMatrix(marker, i)) return Result(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, marker, phase); }
                // Required order for newly pending work: Temp, Primary, Previous.
                foreach (var i in new[] { 2, 0, 1 }) { phase = ArchiveFailureSource(i); if (!MoveOld(marker, i, control)) return Result(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, marker, phase); }
            }
            for (var i = 0; i < 3; i++) { phase = ArchiveFailureSource(i); if (!ArchiveComplete(marker, i)) return Result(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, marker, phase); }
            phase = ProfileResetDiskFailureSourceV1.ArchivePrevious; if (Exists(Path.Combine(marker.Root, Names[1]))) return Result(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, marker, phase);
            var primary = Path.Combine(marker.Root, Names[0]); var temp = Path.Combine(marker.Root, Names[2]);
            phase = ProfileResetDiskFailureSourceV1.DefaultPostcheck; if (Exact(primary, marker.Target) && !Exists(temp)) { phase = ProfileResetDiskFailureSourceV1.Snapshot; return Prepared(marker); }
            phase = ProfileResetDiskFailureSourceV1.PreserveTemp; if (Exists(temp) && !PreserveInterrupted(marker, temp, "interrupted-default-temp", control)) return Result(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, marker, phase);
            // A committed exact primary survives an interrupted temp first-save.
            phase = ProfileResetDiskFailureSourceV1.DefaultPostcheck; if (Exact(primary, marker.Target) && !Exists(temp)) { phase = ProfileResetDiskFailureSourceV1.Snapshot; return Prepared(marker); }
            phase = ProfileResetDiskFailureSourceV1.PreservePrimary; if (Exists(primary) && !Exact(primary, marker.Target) && !PreserveInterrupted(marker, primary, "interrupted-default-primary", control)) return Result(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, marker, phase);
            phase = ProfileResetDiskFailureSourceV1.DefaultPrecommit; if (Exists(primary)) return Result(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, marker, phase);
            var document = ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument;
            phase = ProfileResetDiskFailureSourceV1.DefaultCommit;
            try { control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeDefaultSave); }
            catch (Exception e) when (Recoverable(e)) { return Result(ProfileResetDiskOutcomeV1.ReloadRequired, ProfileResetDurableStateV1.DefaultCommitUncertain, marker, ProfileResetDiskFailureSourceV1.DefaultPrecommit); }
            var save = ProfileAtomicSaveServiceV1.SaveResetFirstUnderHeldRoot(lease, marker, marker.Root, document, new ProfileResetCheckpointAtomicOperationsV1(control, marker.Root));
            phase = ProfileResetDiskFailureSourceV1.DefaultPostcheck;
            if (save.Outcome == ProfileAtomicSaveOutcome.CommittedFirst && Exact(primary, marker.Target) && !Exists(Path.Combine(marker.Root, Names[1])) && !Exists(temp)) { phase = ProfileResetDiskFailureSourceV1.Snapshot; return Prepared(marker); }
            return Result(ProfileResetDiskOutcomeV1.ReloadRequired, ProfileResetDurableStateV1.DefaultCommitUncertain, marker, ProfileResetDiskFailureSourceV1.DefaultCommit);
            }
            catch (Exception e) when (Recoverable(e)) { return Result(ProfileResetDiskOutcomeV1.ManualRepairRequired, ProfileResetDurableStateV1.ManualRepairRequired, marker, phase); }
        }
        private static bool OldMatrix(ValidatedProfileResetBarrierV1 marker, int i)
        {
            var source = Path.Combine(marker.Root, Names[i]);
            var destination = ArchivePath(marker, i);
            var leaf = marker.Leaves[i];
            if (leaf.Presence == ProfileResetLeafPresenceV1.Missing) return !Exists(source) && !Exists(destination);
            var sourceMatches = Exact(source, leaf);
            var destinationMatches = Exact(destination, leaf);
            return (sourceMatches && !Exists(destination)) || (destinationMatches && !Exists(source));
        }
        private static ProfileResetDiskFailureSourceV1 ArchiveFailureSource(int i) { return i == 2 ? ProfileResetDiskFailureSourceV1.ArchiveTemp : i == 0 ? ProfileResetDiskFailureSourceV1.ArchivePrimary : ProfileResetDiskFailureSourceV1.ArchivePrevious; }
        private static bool ArchiveComplete(ValidatedProfileResetBarrierV1 marker, int i)
        {
            var source = Path.Combine(marker.Root, Names[i]);
            var destination = ArchivePath(marker, i);
            var leaf = marker.Leaves[i];
            // Pass one establishes archive completion from fixed destination
            // identity alone. Active Primary may subsequently be the reset
            // product (including byte-identical old/default), so source absence
            // cannot be used here without reintroducing the ambiguity.
            return leaf.Presence == ProfileResetLeafPresenceV1.Missing ? !Exists(destination) : Exact(destination, leaf);
        }
        // C2 must preserve C1's two-pass interpretation: an old Primary that
        // happens byte-equal to r0 is acceptable only after its archive is
        // exact and the *active* row is the target-only row below.
        private static void VerifyPreparedDiskRow(ValidatedProfileResetBarrierV1 marker, bool requireBarrier)
        {
            if (marker == null) throw new IOException("Reset marker is unavailable.");
            CheckRoot(marker.Root); CheckArchiveDirectory(marker);
            for (var i = 0; i < 3; i++) if (!ArchiveComplete(marker, i)) throw new IOException("Reset archive is not exact.");
            var primary = Path.Combine(marker.Root, Names[0]);
            if (!Exact(primary, marker.Target) || Exists(Path.Combine(marker.Root, Names[1])) || Exists(Path.Combine(marker.Root, Names[2])) || Exists(Path.Combine(marker.Root, ProfileResetBarrierV1.BarrierTempName))) throw new IOException("Reset active row is not prepared.");
            if (requireBarrier)
            {
                var live = ReadMarker(marker.Root);
                if (live.Id != marker.Id || !string.Equals(live.Text, marker.Text, StringComparison.Ordinal) || !Bytes(live.Target, marker.Target)) throw new IOException("Reset marker changed.");
            }
        }
        private static void VerifyFinalDiskRow(ValidatedResetMemoryAuthorityV1 authority)
        {
            if (Exists(Path.Combine(authority.Root, ProfileResetBarrierV1.BarrierName)) || Exists(Path.Combine(authority.Root, ProfileResetBarrierV1.BarrierTempName))) throw new IOException("Reset barriers remain present.");
            var marker = ValidatedProfileResetBarrierV1.Create(authority.Root, authority.TransactionId, Encoding.UTF8.GetString(authorityMarkerBytes(authority)), authority.CopyTargetBytes(), authority.CopyLeaves());
            authority.ValidateMarker(marker);
            VerifyPreparedDiskRowWithoutBarrier(marker);
        }
        private static byte[] authorityMarkerBytes(ValidatedResetMemoryAuthorityV1 authority)
        {
            // The original canonical marker is kept only as immutable proof
            // material; this helper reconstructs no disk state and verifies it
            // through ValidateMarker immediately above.
            var text = MarkerText(authority.TransactionId, authority.CopyTargetBytes(), authority.CopyLeaves());
            return Utf8.GetBytes(text);
        }
        private static void VerifyPreparedDiskRowWithoutBarrier(ValidatedProfileResetBarrierV1 marker)
        {
            CheckRoot(marker.Root); CheckArchiveDirectory(marker);
            for (var i = 0; i < 3; i++) if (!ArchiveComplete(marker, i)) throw new IOException("Reset archive is not exact.");
            if (!Exact(Path.Combine(marker.Root, Names[0]), marker.Target) || Exists(Path.Combine(marker.Root, Names[1])) || Exists(Path.Combine(marker.Root, Names[2]))) throw new IOException("Reset active row is not final.");
        }
        private static string ArchiveProofDigest(ValidatedProfileResetBarrierV1 marker)
        {
            return ArchiveDigestForAuthority(Utf8.GetBytes(marker.Text), marker.Leaves);
        }
        private static bool MoveOld(ValidatedProfileResetBarrierV1 marker, int i, IProfileResetDiskTestControlV1 control)
        {
            var leaf = marker.Leaves[i]; if (leaf.Presence == ProfileResetLeafPresenceV1.Missing) return true;
            var source = Path.Combine(marker.Root, Names[i]);
            var destination = ArchivePath(marker, i);
            CheckArchiveDirectory(marker);
            if (Exact(destination, leaf) && !Exists(source)) return true;
            if (!Exact(source, leaf) || Exists(destination)) return false;
            try
            {
                CheckArchiveDirectory(marker); control.Checkpoint(ArchiveCheckpoint(i, true, false)); File.Move(source, destination); control.Checkpoint(ArchiveCheckpoint(i, false, false)); CheckArchiveDirectory(marker);
                control.Checkpoint(ArchiveCheckpoint(i, true, true)); var exact = Exact(destination, leaf) && !Exists(source); control.Checkpoint(ArchiveCheckpoint(i, false, true)); return exact;
            }
            catch (Exception e) when (Recoverable(e)) { return false; }
        }
        private static bool PreserveInterrupted(ValidatedProfileResetBarrierV1 marker, string source, string folder, IProfileResetDiskTestControlV1 control)
        {
            byte[] bytes; try { bytes = Read(source); } catch (Exception e) when (Recoverable(e)) { return false; }
            var destinationParent = Path.Combine(TransactionDirectory(marker.Root, marker.Id), folder); CheckArchiveDirectory(marker); CheckCreatePath(marker.Root, destinationParent); Directory.CreateDirectory(destinationParent); CheckCreatePath(marker.Root, destinationParent);
            var destination = Path.Combine(destinationParent, Hash(bytes) + "-" + bytes.LongLength.ToString(CultureInfo.InvariantCulture) + ".bin"); if (Exists(destination)) return false;
            var primary = folder == "interrupted-default-primary";
            try
            {
                CheckCreatePath(marker.Root, destinationParent); control.Checkpoint(primary ? ProfileResetDiskCheckpointV1.BeforeInterruptedPrimaryMove : ProfileResetDiskCheckpointV1.BeforeInterruptedTempMove); File.Move(source, destination); control.Checkpoint(primary ? ProfileResetDiskCheckpointV1.AfterInterruptedPrimaryMove : ProfileResetDiskCheckpointV1.AfterInterruptedTempMove); CheckCreatePath(marker.Root, destinationParent);
                control.Checkpoint(primary ? ProfileResetDiskCheckpointV1.BeforeInterruptedPrimaryReopen : ProfileResetDiskCheckpointV1.BeforeInterruptedTempReopen); var exact = Bytes(Read(destination), bytes) && !Exists(source); control.Checkpoint(primary ? ProfileResetDiskCheckpointV1.AfterInterruptedPrimaryReopen : ProfileResetDiskCheckpointV1.AfterInterruptedTempReopen); return exact;
            }
            catch (Exception e) when (Recoverable(e)) { return false; }
        }
        private static ProfileResetDiskCheckpointV1 ArchiveCheckpoint(int index, bool before, bool reopen)
        {
            if (index == 2) return reopen ? (before ? ProfileResetDiskCheckpointV1.BeforeArchiveTempReopen : ProfileResetDiskCheckpointV1.AfterArchiveTempReopen) : (before ? ProfileResetDiskCheckpointV1.BeforeArchiveTempMove : ProfileResetDiskCheckpointV1.AfterArchiveTempMove);
            if (index == 0) return reopen ? (before ? ProfileResetDiskCheckpointV1.BeforeArchivePrimaryReopen : ProfileResetDiskCheckpointV1.AfterArchivePrimaryReopen) : (before ? ProfileResetDiskCheckpointV1.BeforeArchivePrimaryMove : ProfileResetDiskCheckpointV1.AfterArchivePrimaryMove);
            return reopen ? (before ? ProfileResetDiskCheckpointV1.BeforeArchivePreviousReopen : ProfileResetDiskCheckpointV1.AfterArchivePreviousReopen) : (before ? ProfileResetDiskCheckpointV1.BeforeArchivePreviousMove : ProfileResetDiskCheckpointV1.AfterArchivePreviousMove);
        }
        private static void Publish(string root, ValidatedProfileResetBarrierV1 marker, IProfileResetDiskTestControlV1 control)
        {
            var temp = Path.Combine(root, ProfileResetBarrierV1.BarrierTempName);
            var committed = Path.Combine(root, ProfileResetBarrierV1.BarrierName);
            if (Exists(temp) || Exists(committed)) throw new IOException("Barrier collision.");
            var bytes = Utf8.GetBytes(marker.Text); control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeMarkerWrite); using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough)) { stream.Write(bytes, 0, bytes.Length); control.Checkpoint(ProfileResetDiskCheckpointV1.AfterMarkerWrite); control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeMarkerFlush); stream.Flush(true); control.Checkpoint(ProfileResetDiskCheckpointV1.AfterMarkerFlush); control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeMarkerClose); } control.Checkpoint(ProfileResetDiskCheckpointV1.AfterMarkerClose);
            control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeMarkerTempReopen); if (!Bytes(Read(temp), bytes)) throw new IOException("Barrier temp did not validate."); control.Checkpoint(ProfileResetDiskCheckpointV1.AfterMarkerTempReopen); control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeMarkerCommitMove); File.Move(temp, committed); control.Checkpoint(ProfileResetDiskCheckpointV1.AfterMarkerCommitMove); control.Checkpoint(ProfileResetDiskCheckpointV1.BeforeMarkerCommittedReopen); var reread = ReadMarker(root); control.Checkpoint(ProfileResetDiskCheckpointV1.AfterMarkerCommittedReopen); if (reread.Id != marker.Id || !Bytes(Utf8.GetBytes(reread.Text), bytes)) throw new IOException("Barrier did not validate.");
        }
        private static ValidatedProfileResetBarrierV1 ReadMarker(string root)
        {
            try { return ReadMarkerCore(root); }
            catch (IOException) { throw; }
            catch (UnauthorizedAccessException) { throw; }
            catch (SecurityException) { throw; }
            catch (Exception e) when (e is JsonException || e is DecoderFallbackException || e is InvalidOperationException || e is ArgumentException || e is KeyNotFoundException || e is FormatException || e is OverflowException) { throw new IOException("Barrier is malformed.", e); }
        }
        private static ValidatedProfileResetBarrierV1 ReadMarkerCore(string root)
        {
            var path = Path.Combine(root, ProfileResetBarrierV1.BarrierName);
            // FileStream alone can follow an unsafe leaf; prove it is a regular
            // present file before opening the canonical marker bytes.
            if (!Exists(path)) throw new IOException("Barrier is absent.");
            var bytes = ReadLimited(path, 8192); var text = Utf8.GetString(bytes); using (var json = JsonDocument.Parse(text))
            {
                var o = json.RootElement; var id = o.GetProperty("transactionId").GetString(); if (o.ValueKind != JsonValueKind.Object || o.EnumerateObject().CountSafe() != 6 || o.GetProperty("format").GetString() != "AcadeGameMaker.ProfileResetBarrier" || o.GetProperty("version").GetInt32() != 1 || !IsId(id)) throw new IOException("Barrier is malformed.");
                var target = DefaultBytes(); var targetElement = o.GetProperty("target"); if (targetElement.GetProperty("profileRevision").GetInt64() != 0 || targetElement.GetProperty("byteLength").GetInt64() != target.LongLength || targetElement.GetProperty("sha256").GetString() != Hash(target)) throw new IOException("Barrier target is malformed.");
                var leaves = ParseLeaves(o.GetProperty("oldLeaves")); var canonical = MarkerText(id, target, leaves); var body = canonical.Substring(0, canonical.LastIndexOf(",\"bodySha256\"", StringComparison.Ordinal)) + "}"; if (!string.Equals(text, canonical, StringComparison.Ordinal) || o.GetProperty("bodySha256").GetString() != Hash(Utf8.GetBytes(body))) throw new IOException("Barrier is noncanonical."); return ValidatedProfileResetBarrierV1.Create(root, id, text, target, leaves);
            }
        }
        private static ValidatedProfileResetBarrierV1 Marker(string root, string id, byte[] target, Leaf[] leaves) { return ValidatedProfileResetBarrierV1.Create(root, id, MarkerText(id, target, leaves), target, leaves); }
        internal static bool MarkerSnapshotIsCanonical(ValidatedProfileResetBarrierV1 marker)
        {
            return marker != null && marker.Leaves != null && marker.Leaves.Length == 3
                && string.Equals(marker.Text, MarkerText(marker.Id, marker.Target, marker.Leaves), StringComparison.Ordinal);
        }
        private static string MarkerText(string id, byte[] target, Leaf[] leaves)
        {
            var body = "{\"format\":\"AcadeGameMaker.ProfileResetBarrier\",\"version\":1,\"transactionId\":\"" + id + "\",\"target\":{\"profileRevision\":0,\"byteLength\":" + target.LongLength.ToString(CultureInfo.InvariantCulture) + ",\"sha256\":\"" + Hash(target) + "\"},\"oldLeaves\":[" + LeafText(leaves[0]) + "," + LeafText(leaves[1]) + "," + LeafText(leaves[2]) + "]}";
            return body.Substring(0, body.Length - 1) + ",\"bodySha256\":\"" + Hash(Utf8.GetBytes(body)) + "\"}";
        }
        private static string LeafText(Leaf l) { var p = l.Presence == ProfileResetLeafPresenceV1.Present; return "{\"role\":\"" + l.Role + "\",\"fileName\":\"" + l.Name + "\",\"presence\":\"" + (p ? "Present" : "Missing") + "\",\"classification\":" + (p ? "\"" + l.Classification.Value + "\"" : "null") + ",\"byteLength\":" + (p ? l.Length.Value.ToString(CultureInfo.InvariantCulture) : "null") + ",\"sha256\":" + (p ? "\"" + l.Hash + "\"" : "null") + ",\"decodedRevision\":" + (l.Revision.HasValue ? l.Revision.Value.ToString(CultureInfo.InvariantCulture) : "null") + "}"; }
        private static Leaf[] ParseLeaves(JsonElement a)
        {
            if (a.ValueKind != JsonValueKind.Array || a.GetArrayLength() != 3) throw new IOException("Barrier leaves malformed."); var leaves = new Leaf[3]; for (var i = 0; i < 3; i++) { var x = a[i]; var presence = x.GetProperty("presence").GetString(); if (x.GetProperty("role").GetString() != Roles[i] || x.GetProperty("fileName").GetString() != Names[i] || (presence != "Present" && presence != "Missing")) throw new IOException("Barrier leaves malformed."); if (presence == "Missing") leaves[i] = new Leaf(Roles[i], Names[i], ProfileResetLeafPresenceV1.Missing, null, null, null, null); else { var c = (ProfileResetLeafClassificationV1)Enum.Parse(typeof(ProfileResetLeafClassificationV1), x.GetProperty("classification").GetString()); var length = x.GetProperty("byteLength").GetInt64(); var hash = x.GetProperty("sha256").GetString(); long? rev = x.GetProperty("decodedRevision").ValueKind == JsonValueKind.Null ? (long?)null : x.GetProperty("decodedRevision").GetInt64(); if (c < ProfileResetLeafClassificationV1.ValidCurrentInput || c > ProfileResetLeafClassificationV1.Invalid || length < 0 || !IsHash(hash) || ((c == ProfileResetLeafClassificationV1.ValidCurrentInput || c == ProfileResetLeafClassificationV1.ValidInputRecoveryRequired) != rev.HasValue) || (rev.HasValue && rev.Value < 0)) throw new IOException("Barrier leaf invalid."); leaves[i] = new Leaf(Roles[i], Names[i], ProfileResetLeafPresenceV1.Present, c, length, hash, rev); } } return leaves;
        }
        private static Leaf[] Observe(string root) { var result = new Leaf[3]; for (var i = 0; i < 3; i++) result[i] = ObserveLeaf(Roles[i], Names[i], Path.Combine(root, Names[i])); return result; }
        private static Leaf ObserveLeaf(string role, string name, string path) { if (!Exists(path)) return new Leaf(role, name, ProfileResetLeafPresenceV1.Missing, null, null, null, null); var bytes = Read(path); var decoded = ProfileCanonicalDecoderV1.Decode(bytes); ProfileResetLeafClassificationV1 c; long? rev = null; if (decoded.Classification == ProfileCanonicalDecodeClassification.ValidCurrentInput) { c = ProfileResetLeafClassificationV1.ValidCurrentInput; rev = decoded.Document.Snapshot.ProfileRevision; } else if (decoded.Classification == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired || decoded.Classification == ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired) { c = ProfileResetLeafClassificationV1.ValidInputRecoveryRequired; rev = decoded.RecoveryProjection.SourceProfileRevision; } else if (decoded.Classification == ProfileCanonicalDecodeClassification.UnsupportedProfileSchema) c = ProfileResetLeafClassificationV1.UnsupportedSchema; else c = ProfileResetLeafClassificationV1.Invalid; return new Leaf(role, name, ProfileResetLeafPresenceV1.Present, c, bytes.LongLength, Hash(bytes), rev); }
        private static bool Exact(string path, Leaf leaf) { try { return leaf.Presence == ProfileResetLeafPresenceV1.Present && Exists(path) && SameLeaf(leaf, ObserveLeaf(leaf.Role, leaf.Name, path)); } catch (Exception e) when (Recoverable(e)) { return false; } }
        private static bool Exact(string path, byte[] target) { try { return Exists(path) && Bytes(Read(path), target); } catch (Exception e) when (Recoverable(e)) { return false; } }
        private static bool SameLeaf(Leaf a, Leaf b) { return a.Same(b); } private static bool Same(Leaf[] a, Leaf[] b) { for (var i = 0; i < 3; i++) if (!a[i].Same(b[i])) return false; return true; }
        private static string ArchivePath(ValidatedProfileResetBarrierV1 marker, int i) { CheckArchiveDirectory(marker); return Path.Combine(TransactionDirectory(marker.Root, marker.Id), Archives[i]); }
        private static string TransactionDirectory(string root, string id) { return Path.Combine(root, Recovery, NewGame, id); }
        private static byte[] DefaultBytes() { return ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes; }
        private static byte[] Read(string path) { using (var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None)) { if (s.Length > int.MaxValue) throw new IOException("File too large."); var b = new byte[(int)s.Length]; var n = 0; while (n < b.Length) { var r = s.Read(b, n, b.Length - n); if (r == 0) throw new EndOfStreamException(); n += r; } return b; } }
        private static byte[] ReadLimited(string path, int maximumBytes) { using (var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None)) { if (s.Length > maximumBytes) throw new IOException("Barrier is oversized."); var b = new byte[(int)s.Length]; var n = 0; while (n < b.Length) { var r = s.Read(b, n, b.Length - n); if (r == 0) throw new EndOfStreamException(); n += r; } return b; } }
        private static bool Exists(string path) { try { var a = File.GetAttributes(path); if ((a & FileAttributes.Directory) != 0 || (a & FileAttributes.ReparsePoint) != 0) throw new IOException("Unsafe path."); return true; } catch (FileNotFoundException) { return false; } catch (DirectoryNotFoundException) { return false; } }
        private static string CheckRoot(string root) { var n = ProfileResetBarrierV1.NormalizeRoot(root); if (Directory.Exists(n) && (File.GetAttributes(n) & FileAttributes.ReparsePoint) != 0) throw new IOException("Root cannot be a reparse point."); return n; }
        // The atomic service cannot manufacture this authority: all inputs are
        // reauthenticated against live disk, exact target bytes and the lease.
        internal static void ValidateResetFirstCapability(ProfileRootOperationLockV1 lease, ValidatedProfileResetBarrierV1 marker, string root, ProfileCanonicalDocumentV1 document)
        {
            if (lease == null || marker == null || !ReferenceEquals(lease, marker.IssuingLease) || !string.Equals(root, marker.Root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Reset first-save capability is invalid.");
            lease.ValidateOwnership(root); var live = ReadMarker(root);
            if (live.Id != marker.Id || !string.Equals(live.Text, marker.Text, StringComparison.Ordinal) || !Bytes(live.Target, marker.Target) || !Bytes(document.CanonicalFileBytes, marker.Target) || document.Snapshot.ProfileRevision != 0) throw new InvalidOperationException("Reset first-save marker no longer matches.");
            var transactionDirectory = TransactionDirectory(root, marker.Id);
            CheckCreatePath(root, transactionDirectory);
            if (!Directory.Exists(transactionDirectory) || (File.GetAttributes(transactionDirectory) & FileAttributes.ReparsePoint) != 0) throw new InvalidOperationException("Reset first-save archive is unavailable.");
            for (var i = 0; i < 3; i++) if (!ArchiveComplete(live, i)) throw new InvalidOperationException("Reset first-save archive proof is incomplete.");
            foreach (var name in Names) if (Exists(Path.Combine(root, name))) throw new InvalidOperationException("Reset first-save requires no active leaves.");
        }
        // REQ-M5D7QC1-003/005: do not let Directory.Exists hide access, file,
        // or reparse failures while proving a destination remains contained.
        private static void CheckCreatePath(string root, string candidate)
        {
            root = CheckRoot(root);
            CheckRootAncestors(root);
            var full = Path.GetFullPath(candidate);
            var prefix = root.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) ? root : root + Path.DirectorySeparatorChar;
            if (!full.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) throw new IOException("Path escapes root.");
            var current = root;
            var relative = full.Substring(prefix.Length);
            foreach (var part in relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            {
                if (part.Length == 0) continue;
                current = Path.Combine(current, part);
                FileAttributes attributes;
                try { attributes = File.GetAttributes(current); }
                catch (FileNotFoundException) { continue; }
                catch (DirectoryNotFoundException) { continue; }
                if ((attributes & FileAttributes.ReparsePoint) != 0 || (attributes & FileAttributes.Directory) == 0) throw new IOException("Transaction path is unsafe.");
            }
        }
        private static void CheckArchiveDirectory(ValidatedProfileResetBarrierV1 marker)
        {
            var transactionDirectory = TransactionDirectory(marker.Root, marker.Id);
            CheckCreatePath(marker.Root, transactionDirectory);
            FileAttributes attributes;
            try { attributes = File.GetAttributes(transactionDirectory); }
            catch (FileNotFoundException) { throw new IOException("Transaction directory is absent."); }
            catch (DirectoryNotFoundException) { throw new IOException("Transaction directory is absent."); }
            if ((attributes & (FileAttributes.Directory | FileAttributes.ReparsePoint)) != FileAttributes.Directory) throw new IOException("Transaction directory is unsafe.");
        }
        private static void CheckRootAncestors(string root)
        {
            var parent = Directory.GetParent(root);
            while (parent != null)
            {
                var attributes = File.GetAttributes(parent.FullName);
                if ((attributes & FileAttributes.ReparsePoint) != 0 || (attributes & FileAttributes.Directory) == 0) throw new IOException("Root ancestor is unsafe.");
                parent = parent.Parent;
            }
        }
        private static void VerifyBeforeLease(string root) { CheckRoot(root); var lockPath = Path.Combine(root, "profile.operation.lock"); if (File.Exists(lockPath)) { var attributes = File.GetAttributes(lockPath); if ((attributes & (FileAttributes.Directory | FileAttributes.ReparsePoint)) != 0) throw new IOException("Lock path is unsafe."); } var parent = Directory.GetParent(root); while (parent != null) { if ((File.GetAttributes(parent.FullName) & FileAttributes.ReparsePoint) != 0) throw new IOException("Root ancestor is a reparse point."); parent = parent.Parent; } }
        private static string NewId() { var b = new byte[16]; using (var r = RandomNumberGenerator.Create()) r.GetBytes(b); var sb = new StringBuilder(32); for (var i = 0; i < b.Length; i++) sb.Append(b[i].ToString("x2")); return sb.ToString(); }
        private static string Hash(byte[] b) { using (var s = SHA256.Create()) { var h = s.ComputeHash(b); var x = new StringBuilder(64); for (var i = 0; i < h.Length; i++) x.Append(h[i].ToString("x2")); return x.ToString(); } }
        internal static string HashForProof(byte[] bytes) { return Hash(bytes); }
        private static bool Bytes(byte[] a, byte[] b) { if (a == null || b == null || a.Length != b.Length) return false; for (var i = 0; i < a.Length; i++) if (a[i] != b[i]) return false; return true; }
        private static bool IsId(string v) { return v != null && v.Length == 32 && IsHash(v + v); } private static bool IsHash(string v) { if (v == null || v.Length != 64) return false; for (var i = 0; i < v.Length; i++) if (!((v[i] >= '0' && v[i] <= '9') || (v[i] >= 'a' && v[i] <= 'f'))) return false; return true; }
        private static bool Recoverable(Exception e) { return e is IOException || e is UnauthorizedAccessException || e is SecurityException; }
        // REQ-M5D7QC1-006/007: BuildResult is invoked while the held lease still
        // owns the root.  A snapshot failure is Unreadable evidence, never Missing.
        private static ProfileResetDiskResultV1 Prepared(ValidatedProfileResetBarrierV1 marker)
        {
            var archive = new ProfileResetDiskLeafProofV1[3];
            for (var i = 0; i < 3; i++) archive[i] = new ProfileResetDiskLeafProofV1(marker.Leaves[i]);
            var activeLeaves = Observe(marker.Root);
            var active = new ProfileResetDiskLeafProofV1[3];
            for (var i = 0; i < 3; i++) active[i] = new ProfileResetDiskLeafProofV1(activeLeaves[i]);
            var archiveSnapshots = ArchiveSnapshots(marker);
            var activeSnapshots = ActiveSnapshots(marker.Root, true);
            return new ProfileResetDiskResultV1(ProfileResetDiskOutcomeV1.DiskPrepared, ProfileResetDurableStateV1.DiskPrepared, marker.Id, 0, marker.Target.LongLength, Hash(marker.Target), ProfileResetDiskPreparedProofV1.Create(marker), marker, archive, active, archiveSnapshots, activeSnapshots, ProfileResetDiskFailureSourceV1.None);
        }
        private static ProfileResetDiskResultV1 NoAuthorityResult(ProfileResetDiskOutcomeV1 outcome, ProfileResetDurableStateV1 state, string root, bool mayProbe, ProfileResetDiskFailureSourceV1 source)
        {
            return new ProfileResetDiskResultV1(outcome, state, null, -1, -1, null, null, null, null, null, NotObservedArchives(), ActiveSnapshots(root, mayProbe), source);
        }
        private static ProfileResetDiskResultV1 Result(ProfileResetDiskOutcomeV1 outcome, ProfileResetDurableStateV1 state, ValidatedProfileResetBarrierV1 marker)
        {
            return Result(outcome, state, marker, ProfileResetDiskFailureSourceV1.DefaultPostcheck);
        }
        private static ProfileResetDiskResultV1 Result(ProfileResetDiskOutcomeV1 outcome, ProfileResetDurableStateV1 state, ValidatedProfileResetBarrierV1 marker, ProfileResetDiskFailureSourceV1 source)
        {
            return new ProfileResetDiskResultV1(outcome, state, marker.Id, 0, marker.Target.LongLength, Hash(marker.Target), null, marker, null, null, ArchiveSnapshots(marker), ActiveSnapshots(marker.Root, true), source);
        }
        // The locally created marker is only a candidate.  Retain its identity
        // after a fault only when the committed on-disk marker is canonical and
        // exactly matches it while the issuing lease is still held.
        private static ValidatedProfileResetBarrierV1 AuthenticateFailureMarker(ProfileRootOperationLockV1 lease, string root, ValidatedProfileResetBarrierV1 expected)
        {
            if (expected == null) return null;
            try
            {
                var live = ReadMarker(root);
                if (live.Id != expected.Id || !string.Equals(live.Text, expected.Text, StringComparison.Ordinal) || !Bytes(live.Target, expected.Target)) return null;
                return ValidatedProfileResetBarrierV1.BindLease(live, lease);
            }
            catch (Exception e) when (Recoverable(e)) { return null; }
        }
        private static ProfileResetDiskActiveSnapshotV1[] ActiveSnapshots(string root, bool mayProbe)
        {
            var result = new ProfileResetDiskActiveSnapshotV1[3];
            for (var i = 0; i < 3; i++)
            {
                if (!mayProbe) { result[i] = new ProfileResetDiskActiveSnapshotV1(Roles[i], Names[i], ProfileResetDiskObservationV1.NotObserved, null); continue; }
                try
                {
                    var leaf = ObserveLeaf(Roles[i], Names[i], Path.Combine(root, Names[i]));
                    result[i] = leaf.Presence == ProfileResetLeafPresenceV1.Missing ? new ProfileResetDiskActiveSnapshotV1(Roles[i], Names[i], ProfileResetDiskObservationV1.Missing, null) : new ProfileResetDiskActiveSnapshotV1(Roles[i], Names[i], ProfileResetDiskObservationV1.Readable, leaf);
                }
                catch (Exception e) when (Recoverable(e)) { result[i] = new ProfileResetDiskActiveSnapshotV1(Roles[i], Names[i], ProfileResetDiskObservationV1.Unreadable, null); }
            }
            return result;
        }
        private static ProfileResetDiskArchiveSnapshotV1[] NotObservedArchives()
        {
            var result = new ProfileResetDiskArchiveSnapshotV1[3];
            for (var i = 0; i < 3; i++) result[i] = new ProfileResetDiskArchiveSnapshotV1(Roles[i], Names[i], ProfileResetDiskArchiveProofV1.NotObserved, ProfileResetDiskObservationV1.NotObserved, null);
            return result;
        }
        private static ProfileResetDiskArchiveSnapshotV1[] ArchiveSnapshots(ValidatedProfileResetBarrierV1 marker)
        {
            var result = new ProfileResetDiskArchiveSnapshotV1[3];
            for (var i = 0; i < 3; i++)
            {
                var declared = marker.Leaves[i];
                try
                {
                    // Every archive observation is preceded by an owned-root
                    // containment/reparse proof; a failed proof is unreadable.
                    CheckArchiveDirectory(marker);
                    var path = ArchivePath(marker, i);
                    if (!Exists(path)) { result[i] = new ProfileResetDiskArchiveSnapshotV1(Roles[i], Names[i], declared.Presence == ProfileResetLeafPresenceV1.Missing ? ProfileResetDiskArchiveProofV1.NotApplicable : ProfileResetDiskArchiveProofV1.Pending, ProfileResetDiskObservationV1.Missing, null); continue; }
                    CheckArchiveDirectory(marker);
                    var found = ObserveLeaf(Roles[i], Names[i], path);
                    var proof = declared.Presence == ProfileResetLeafPresenceV1.Present && SameLeaf(declared, found) ? ProfileResetDiskArchiveProofV1.Exact : ProfileResetDiskArchiveProofV1.Mismatch;
                    result[i] = new ProfileResetDiskArchiveSnapshotV1(Roles[i], Names[i], proof, ProfileResetDiskObservationV1.Readable, found);
                }
                catch (Exception e) when (Recoverable(e)) { result[i] = new ProfileResetDiskArchiveSnapshotV1(Roles[i], Names[i], ProfileResetDiskArchiveProofV1.Unreadable, ProfileResetDiskObservationV1.Unreadable, null); }
            }
            return result;
        }
    }
    internal static class JsonElementC1Extensions { internal static int CountSafe(this JsonElement.ObjectEnumerator e) { var n = 0; while (e.MoveNext()) n++; return n; } }
}
