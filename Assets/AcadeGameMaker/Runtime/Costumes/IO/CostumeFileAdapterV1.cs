using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using AcadeGameMaker.Costumes;

[assembly: InternalsVisibleTo("AcadeGameMaker.Costumes.IO.EditMode.Tests")]

namespace AcadeGameMaker.Costumes.IO
{
    // REQ-CIO-001..008. Windows local-NTFS costume-only persistence; never uses unrelated authority.
    public enum CostumeFileRoleV1 { Primary = 1, Previous = 2, Temp = 3 }
    public enum CostumeFileReadDispositionV1 { Missing = 1, Read = 2, ReadFailed = 3, NotObserved = 4 }
    public enum CostumeFileLoadOutcomeV1 { Recovered = 1, ReadFailure = 2, PathRejected = 3 }
    public enum CostumeFileSaveOutcomeV1 { CommittedFirst = 1, CommittedReplacement = 2, FailedBeforeCommit = 3, CommitOutcomeUncertain = 4 }
    public enum CostumeFileSaveStageV1 { None = 0, Validation = 1, TempWrite = 2, ManagedFlush = 3, DurableFlush = 4, TempClose = 5, AtomicMove = 6, AtomicReplace = 7, PrimaryReopen = 8, PrimaryEquality = 9, PrimaryDecode = 10 }
    public enum CostumeFilePathKindV1 { Missing = 1, File = 2, Directory = 3, ReadFailed = 4 }

    public readonly struct CostumeFilePathInfoV1
    {
        public CostumeFilePathInfoV1(CostumeFilePathKindV1 kind, FileAttributes attributes)
        {
            if (kind != CostumeFilePathKindV1.Missing && kind != CostumeFilePathKindV1.File && kind != CostumeFilePathKindV1.Directory && kind != CostumeFilePathKindV1.ReadFailed) throw new ArgumentOutOfRangeException(nameof(kind));
            Kind = kind; Attributes = attributes;
        }
        public CostumeFilePathKindV1 Kind { get; }
        public FileAttributes Attributes { get; }
    }

    public readonly struct CostumeFilePortReadResultV1
    {
        private readonly byte[] _bytes;
        public CostumeFilePortReadResultV1(CostumeFileReadDispositionV1 disposition, byte[] bytes, FileAttributes attributes)
        {
            if (disposition != CostumeFileReadDispositionV1.Missing && disposition != CostumeFileReadDispositionV1.Read && disposition != CostumeFileReadDispositionV1.ReadFailed) throw new ArgumentOutOfRangeException(nameof(disposition));
            if ((disposition == CostumeFileReadDispositionV1.Read) != (bytes != null)) throw new ArgumentException("Read bytes must match disposition.");
            Disposition = disposition; _bytes = Copy(bytes); Attributes = attributes;
        }
        public CostumeFileReadDispositionV1 Disposition { get; }
        public FileAttributes Attributes { get; }
        public byte[] Bytes { get { return Copy(_bytes); } }
        private static byte[] Copy(byte[] value) { return value == null ? null : (byte[])value.Clone(); }
    }

    public readonly struct CostumeFileObservationV1
    {
        private readonly byte[] _bytes;
        public CostumeFileObservationV1(CostumeFileRoleV1 role, CostumeFileReadDispositionV1 disposition, byte[] bytes)
        {
            if (role != CostumeFileRoleV1.Primary && role != CostumeFileRoleV1.Previous && role != CostumeFileRoleV1.Temp) throw new ArgumentOutOfRangeException(nameof(role));
            if (disposition != CostumeFileReadDispositionV1.Missing && disposition != CostumeFileReadDispositionV1.Read && disposition != CostumeFileReadDispositionV1.ReadFailed && disposition != CostumeFileReadDispositionV1.NotObserved) throw new ArgumentOutOfRangeException(nameof(disposition));
            if ((disposition == CostumeFileReadDispositionV1.Read) != (bytes != null)) throw new ArgumentException("Observed bytes must match disposition.");
            Role = role; Disposition = disposition; _bytes = Copy(bytes);
        }
        public CostumeFileRoleV1 Role { get; }
        public CostumeFileReadDispositionV1 Disposition { get; }
        public byte[] Bytes { get { return Copy(_bytes); } }
        private static byte[] Copy(byte[] value) { return value == null ? null : (byte[])value.Clone(); }
    }

    public interface ICostumeFileWriteSessionV1 : IDisposable
    {
        void WriteExact(byte[] bytes);
        void FlushManaged();
        void FlushDurable();
    }

    // Injection seam is limited to the approved CIO fault-boundary tests; callers never provide owned filenames.
    public interface ICostumeFilePortV1
    {
        bool IsExistingLocalNtfsDirectoryWithoutReparseAncestor(string normalizedDirectory);
        CostumeFilePortReadResultV1 ReadOnce(string fullPath);
        CostumeFilePathInfoV1 Inspect(string fullPath);
        ICostumeFileWriteSessionV1 OpenTempForWrite(string fullPath);
        void MoveNoOverwrite(string sourcePath, string destinationPath);
        void Replace(string sourcePath, string destinationPath, string backupPath);
    }

    // An optional deterministic test seam. It grants no path or storage authority.
    internal interface ICostumeFileTransactionObserverV1
    {
        void RecordStage(CostumeFileSaveStageV1 stage);
    }

    public sealed class CostumeFileLoadResultV1
    {
        internal CostumeFileLoadResultV1(CostumeFileLoadOutcomeV1 outcome, CostumeFileObservationV1 primary, CostumeFileObservationV1 previous, CostumeFileObservationV1 temp, CostumeRecoveryPlanV1 plan, bool hasPlan)
        {
            Outcome = outcome; Primary = primary; Previous = previous; Temp = temp; _plan = plan; HasRecoveryPlan = hasPlan;
        }
        private readonly CostumeRecoveryPlanV1 _plan;
        public CostumeFileLoadOutcomeV1 Outcome { get; }
        public CostumeFileObservationV1 Primary { get; }
        public CostumeFileObservationV1 Previous { get; }
        public CostumeFileObservationV1 Temp { get; }
        public bool HasRecoveryPlan { get; }
        public CostumeRecoveryPlanV1 RecoveryPlan { get { if (!HasRecoveryPlan) throw new InvalidOperationException("No recovery plan was published."); return _plan; } }
    }

    public readonly struct CostumeFileSaveResultV1
    {
        internal CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1 outcome, CostumeFileSaveStageV1 stage, long expectedRevision)
        {
            Outcome = outcome; Stage = stage; ExpectedRevision = expectedRevision;
        }
        public CostumeFileSaveOutcomeV1 Outcome { get; }
        public CostumeFileSaveStageV1 Stage { get; }
        public long ExpectedRevision { get; }
    }

    public sealed class CostumeFileAdapterV1
    {
        public const string PrimaryFileName = "costume-state-v1.json";
        public const string PreviousFileName = "costume-state-v1.previous.json";
        public const string TempFileName = "costume-state-v1.temp.json";

        private readonly string _directory;
        private readonly string _primary;
        private readonly string _previous;
        private readonly string _temp;
        private readonly ICostumeFilePortV1 _port;

        public CostumeFileAdapterV1(string directory) : this(directory, new WindowsNtfsCostumeFilePortV1()) { }

        public CostumeFileAdapterV1(string directory, ICostumeFilePortV1 port)
        {
            _directory = NormalizeDirectory(directory);
            _primary = DeriveOwnedPath(_directory, PrimaryFileName);
            _previous = DeriveOwnedPath(_directory, PreviousFileName);
            _temp = DeriveOwnedPath(_directory, TempFileName);
            if (StringComparer.OrdinalIgnoreCase.Equals(_primary, _previous) || StringComparer.OrdinalIgnoreCase.Equals(_primary, _temp) || StringComparer.OrdinalIgnoreCase.Equals(_previous, _temp)) throw new ArgumentException("Owned role paths alias.");
            _port = port ?? throw new ArgumentNullException(nameof(port));
        }

        public string DirectoryPath { get { return _directory; } }

        // REQ-CIO-002/003: each role is opened once; selector still owns byte validity and recovery.
        public CostumeFileLoadResultV1 Load(CostumeCatalogV1 catalog)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (!PathsAreSafe()) return RejectedLoad();
            var primary = Observe(CostumeFileRoleV1.Primary, _primary);
            var previous = Observe(CostumeFileRoleV1.Previous, _previous);
            var temp = Observe(CostumeFileRoleV1.Temp, _temp);
            if (UnreadAuthorityCouldChangeSelection(primary, previous, temp)) return new CostumeFileLoadResultV1(CostumeFileLoadOutcomeV1.ReadFailure, primary, previous, temp, default(CostumeRecoveryPlanV1), false);
            var input = new CostumeRecoveryInputV1(BytesOrNull(primary), BytesOrNull(previous), BytesOrNull(temp));
            var plan = CostumeRecoverySelectorV1.Recover(catalog, input);
            // A valid canonical empty state has no actor from which the pure selector can infer an unavailable default.
            // CIO retains the pure state but truthfully publishes the catalog-wide unavailable result without writing it.
            if (plan.Reason != CostumeRecoveryReason.NoAcceptedDefault && plan.State.CurrentByActor.Count == 0 && !HasAnyAcceptedDefault(catalog)) plan = new CostumeRecoveryPlanV1(plan.State, CostumeRecoveryReason.NoAcceptedDefault, false);
            return new CostumeFileLoadResultV1(CostumeFileLoadOutcomeV1.Recovered, primary, previous, temp, plan, true);
        }

        // REQ-CIO-004/005: this method never changes the state revision and never retries after atomic commit begins.
        public CostumeFileSaveResultV1 Save(CostumeStateV1 expectedState)
        {
            var revision = expectedState == null ? -1 : expectedState.Revision;
            if (expectedState == null || !PathsAreSafe()) return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.Validation, revision);
            byte[] exact;
            try
            {
                exact = CostumeCanonicalCodecV1.Encode(expectedState);
                CostumeStateV1 decoded;
                if (!CostumeCanonicalCodecV1.TryDecode(exact, out decoded) || decoded == null || decoded.Revision != expectedState.Revision) return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.Validation, revision);
            }
            catch (ArgumentException) { return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.Validation, revision); }
            catch (OverflowException) { return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.Validation, revision); }

            CostumeFilePathInfoV1 primaryInfo;
            CostumeFilePathInfoV1 previousInfo;
            CostumeFilePathInfoV1 tempInfo;
            try
            {
                Trace(CostumeFileSaveStageV1.Validation);
                primaryInfo = _port.Inspect(_primary);
                previousInfo = _port.Inspect(_previous);
                tempInfo = _port.Inspect(_temp);
            }
            catch (Exception exception) when (IsExpectedInspectionFailure(exception)) { return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.Validation, revision); }
            if (!SafeOwnedRole(primaryInfo) || !SafeOwnedRole(previousInfo) || !SafeOwnedRole(tempInfo) || primaryInfo.Kind == CostumeFilePathKindV1.Directory || previousInfo.Kind == CostumeFilePathKindV1.Directory || tempInfo.Kind == CostumeFilePathKindV1.Directory)
                return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.Validation, revision);

            ICostumeFileWriteSessionV1 session = null;
            try
            {
                Trace(CostumeFileSaveStageV1.TempWrite);
                session = _port.OpenTempForWrite(_temp);
                session.WriteExact(exact);
            }
            catch (Exception exception) when (IsExpectedFileFailure(exception)) { DisposeNoThrow(session); return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.TempWrite, revision); }
            try { Trace(CostumeFileSaveStageV1.ManagedFlush); session.FlushManaged(); }
            catch (Exception exception) when (IsExpectedFileFailure(exception)) { DisposeNoThrow(session); return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.ManagedFlush, revision); }
            try { Trace(CostumeFileSaveStageV1.DurableFlush); session.FlushDurable(); }
            catch (Exception exception) when (IsExpectedFileFailure(exception)) { DisposeNoThrow(session); return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.DurableFlush, revision); }
            try { Trace(CostumeFileSaveStageV1.TempClose); session.Dispose(); session = null; }
            catch (Exception exception) when (IsExpectedFileFailure(exception)) { return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.TempClose, revision); }

            var replacement = primaryInfo.Kind == CostumeFilePathKindV1.File;
            var commitStage = replacement ? CostumeFileSaveStageV1.AtomicReplace : CostumeFileSaveStageV1.AtomicMove;
            try
            {
                // From this call onward the commit result is deliberately never retried, cleaned, or reclassified as success.
                Trace(commitStage);
                if (replacement) _port.Replace(_temp, _primary, _previous); else _port.MoveNoOverwrite(_temp, _primary);
            }
            catch (Exception exception) when (IsExpectedFileFailure(exception)) { return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, commitStage, revision); }

            CostumeFilePortReadResultV1 reopened;
            try { Trace(CostumeFileSaveStageV1.PrimaryReopen); reopened = _port.ReadOnce(_primary); }
            catch (Exception exception) when (IsExpectedFileFailure(exception)) { return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.PrimaryReopen, revision); }
            if (reopened.Disposition != CostumeFileReadDispositionV1.Read) return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.PrimaryReopen, revision);
            var reopenedBytes = reopened.Bytes;
            try
            {
                Trace(CostumeFileSaveStageV1.PrimaryEquality);
                if (!ByteEquals(exact, reopenedBytes)) return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.PrimaryEquality, revision);
            }
            catch (Exception exception) when (IsExpectedFileFailure(exception)) { return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.PrimaryEquality, revision); }
            CostumeStateV1 confirmed;
            try
            {
                Trace(CostumeFileSaveStageV1.PrimaryDecode);
                if (!CostumeCanonicalCodecV1.TryDecode(reopenedBytes, out confirmed) || confirmed == null || confirmed.Revision != expectedState.Revision) return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.PrimaryDecode, revision);
            }
            catch (Exception exception) when (IsExpectedFileFailure(exception)) { return new CostumeFileSaveResultV1(CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.PrimaryDecode, revision); }
            return new CostumeFileSaveResultV1(replacement ? CostumeFileSaveOutcomeV1.CommittedReplacement : CostumeFileSaveOutcomeV1.CommittedFirst, CostumeFileSaveStageV1.PrimaryDecode, revision);
        }

        private CostumeFileLoadResultV1 RejectedLoad()
        {
            var missingPrimary = new CostumeFileObservationV1(CostumeFileRoleV1.Primary, CostumeFileReadDispositionV1.NotObserved, null);
            var missingPrevious = new CostumeFileObservationV1(CostumeFileRoleV1.Previous, CostumeFileReadDispositionV1.NotObserved, null);
            var missingTemp = new CostumeFileObservationV1(CostumeFileRoleV1.Temp, CostumeFileReadDispositionV1.NotObserved, null);
            return new CostumeFileLoadResultV1(CostumeFileLoadOutcomeV1.PathRejected, missingPrimary, missingPrevious, missingTemp, default(CostumeRecoveryPlanV1), false);
        }

        private CostumeFileObservationV1 Observe(CostumeFileRoleV1 role, string path)
        {
            CostumeFilePortReadResultV1 result;
            try { result = _port.ReadOnce(path); }
            catch (Exception exception) when (IsExpectedFileFailure(exception)) { return new CostumeFileObservationV1(role, CostumeFileReadDispositionV1.ReadFailed, null); }
            if ((result.Attributes & FileAttributes.ReparsePoint) != 0 || result.Disposition == CostumeFileReadDispositionV1.ReadFailed) return new CostumeFileObservationV1(role, CostumeFileReadDispositionV1.ReadFailed, null);
            if (result.Disposition != CostumeFileReadDispositionV1.Missing && result.Disposition != CostumeFileReadDispositionV1.Read) return new CostumeFileObservationV1(role, CostumeFileReadDispositionV1.ReadFailed, null);
            var bytes = result.Bytes;
            if ((result.Disposition == CostumeFileReadDispositionV1.Read) != (bytes != null)) return new CostumeFileObservationV1(role, CostumeFileReadDispositionV1.ReadFailed, null);
            return new CostumeFileObservationV1(role, result.Disposition, bytes);
        }

        private bool PathsAreSafe()
        {
            try { return _port.IsExistingLocalNtfsDirectoryWithoutReparseAncestor(_directory) && ParentIsExact(_primary) && ParentIsExact(_previous) && ParentIsExact(_temp); }
            catch (Exception exception) when (IsExpectedFileFailure(exception)) { return false; }
        }

        private bool ParentIsExact(string path) { return StringComparer.OrdinalIgnoreCase.Equals(Path.GetDirectoryName(path), _directory); }
        private static bool SafeOwnedRole(CostumeFilePathInfoV1 info) { return info.Kind != CostumeFilePathKindV1.ReadFailed && (info.Attributes & FileAttributes.ReparsePoint) == 0; }
        private static byte[] BytesOrNull(CostumeFileObservationV1 observation) { return observation.Disposition == CostumeFileReadDispositionV1.Read ? observation.Bytes : null; }

        private static bool UnreadAuthorityCouldChangeSelection(CostumeFileObservationV1 primary, CostumeFileObservationV1 previous, CostumeFileObservationV1 temp)
        {
            if (primary.Disposition == CostumeFileReadDispositionV1.ReadFailed) return true;
            if (Decodes(primary)) return false;
            if (previous.Disposition == CostumeFileReadDispositionV1.ReadFailed) return true;
            if (Decodes(previous)) return false;
            return temp.Disposition == CostumeFileReadDispositionV1.ReadFailed;
        }

        private static bool Decodes(CostumeFileObservationV1 observation)
        {
            if (observation.Disposition != CostumeFileReadDispositionV1.Read) return false;
            CostumeStateV1 ignored;
            return CostumeCanonicalCodecV1.TryDecode(observation.Bytes, out ignored);
        }

        private static bool HasAnyAcceptedDefault(CostumeCatalogV1 catalog)
        {
            for (var i = 0; i < catalog.Definitions.Count; i++) if (catalog.Definitions[i].Disposition == CostumeDisposition.Accepted && catalog.Definitions[i].DefaultGrant) return true;
            return false;
        }

        private static string NormalizeDirectory(string directory)
        {
            if (String.IsNullOrWhiteSpace(directory) || !Path.IsPathRooted(directory) || IsDriveRelative(directory) || HasTraversalOrAlternatePrefix(directory)) throw new ArgumentException("Directory must be an absolute, ordinary non-root path.", nameof(directory));
            var full = Path.GetFullPath(directory);
            if (full.StartsWith("\\\\", StringComparison.Ordinal) || full.StartsWith("//", StringComparison.Ordinal)) throw new ArgumentException("Network paths are not supported.", nameof(directory));
            var root = Path.GetPathRoot(full);
            while (full.Length > root.Length && (full[full.Length - 1] == Path.DirectorySeparatorChar || full[full.Length - 1] == Path.AltDirectorySeparatorChar)) full = full.Substring(0, full.Length - 1);
            if (StringComparer.OrdinalIgnoreCase.Equals(full, root)) throw new ArgumentException("Filesystem roots are not supported.", nameof(directory));
            return full;
        }

        private static bool HasTraversalOrAlternatePrefix(string value)
        {
            if (value.StartsWith("\\\\?\\", StringComparison.Ordinal) || value.StartsWith("\\\\.\\", StringComparison.Ordinal)) return true;
            var segments = value.Replace('/', '\\').Split('\\');
            for (var i = 0; i < segments.Length; i++) if (segments[i] == "." || segments[i] == "..") return true;
            return false;
        }

        // On Windows Path.IsPathRooted("C:relative") is true, but it is not fully qualified.
        private static bool IsDriveRelative(string value)
        {
            return value.Length >= 2 && value[1] == ':' && (value.Length == 2 || (value[2] != Path.DirectorySeparatorChar && value[2] != Path.AltDirectorySeparatorChar));
        }

        private void Trace(CostumeFileSaveStageV1 stage)
        {
            var observer = _port as ICostumeFileTransactionObserverV1;
            if (observer != null) observer.RecordStage(stage);
        }

        private static string DeriveOwnedPath(string directory, string name)
        {
            var path = Path.GetFullPath(Path.Combine(directory, name));
            if (!StringComparer.OrdinalIgnoreCase.Equals(Path.GetDirectoryName(path), directory) || !StringComparer.Ordinal.Equals(Path.GetFileName(path), name)) throw new ArgumentException("Derived owned path escaped its directory.");
            return path;
        }

        private static bool ByteEquals(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            for (var i = 0; i < left.Length; i++) if (left[i] != right[i]) return false;
            return true;
        }

        private static void DisposeNoThrow(IDisposable value) { try { if (value != null) value.Dispose(); } catch (Exception exception) when (IsExpectedFileFailure(exception)) { } }
        private static bool IsExpectedInspectionFailure(Exception exception) { return exception is IOException || exception is UnauthorizedAccessException || exception is SecurityException || exception is NotSupportedException || (exception is ArgumentException && !(exception is ArgumentNullException)); }
        private static bool IsExpectedFileFailure(Exception exception) { return IsExpectedInspectionFailure(exception); }
    }

    public sealed class WindowsNtfsCostumeFilePortV1 : ICostumeFilePortV1
    {
        public bool IsExistingLocalNtfsDirectoryWithoutReparseAncestor(string normalizedDirectory)
        {
            try
            {
                var info = new DirectoryInfo(normalizedDirectory);
                if (!info.Exists || (File.GetAttributes(info.FullName) & FileAttributes.ReparsePoint) != 0) return false;
                var root = Path.GetPathRoot(info.FullName);
                var drive = new DriveInfo(root);
                if (drive.DriveType != DriveType.Fixed || !StringComparer.OrdinalIgnoreCase.Equals(drive.DriveFormat, "NTFS")) return false;
                for (var cursor = info.Parent; cursor != null; cursor = cursor.Parent) if ((File.GetAttributes(cursor.FullName) & FileAttributes.ReparsePoint) != 0) return false;
                return true;
            }
            catch (IOException) { return false; }
            catch (UnauthorizedAccessException) { return false; }
            catch (SecurityException) { return false; }
            catch (NotSupportedException) { return false; }
            catch (ArgumentException) { return false; }
        }

        public CostumeFilePortReadResultV1 ReadOnce(string fullPath)
        {
            try
            {
                var attributes = File.GetAttributes(fullPath);
                if ((attributes & FileAttributes.ReparsePoint) != 0 || (attributes & FileAttributes.Directory) != 0) return new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.ReadFailed, null, attributes);
                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var copy = new MemoryStream()) { stream.CopyTo(copy); return new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.Read, copy.ToArray(), attributes); }
            }
            catch (FileNotFoundException) { return new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.Missing, null, 0); }
            catch (DirectoryNotFoundException) { return new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.Missing, null, 0); }
            catch (IOException) { return new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.ReadFailed, null, 0); }
            catch (UnauthorizedAccessException) { return new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.ReadFailed, null, 0); }
            catch (SecurityException) { return new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.ReadFailed, null, 0); }
        }

        public CostumeFilePathInfoV1 Inspect(string fullPath)
        {
            try
            {
                var attributes = File.GetAttributes(fullPath);
                return new CostumeFilePathInfoV1((attributes & FileAttributes.Directory) != 0 ? CostumeFilePathKindV1.Directory : CostumeFilePathKindV1.File, attributes);
            }
            catch (FileNotFoundException) { return new CostumeFilePathInfoV1(CostumeFilePathKindV1.Missing, 0); }
            catch (DirectoryNotFoundException) { return new CostumeFilePathInfoV1(CostumeFilePathKindV1.Missing, 0); }
            catch (IOException) { return new CostumeFilePathInfoV1(CostumeFilePathKindV1.ReadFailed, 0); }
            catch (UnauthorizedAccessException) { return new CostumeFilePathInfoV1(CostumeFilePathKindV1.ReadFailed, 0); }
            catch (SecurityException) { return new CostumeFilePathInfoV1(CostumeFilePathKindV1.ReadFailed, 0); }
            catch (NotSupportedException) { return new CostumeFilePathInfoV1(CostumeFilePathKindV1.ReadFailed, 0); }
            catch (ArgumentException) { return new CostumeFilePathInfoV1(CostumeFilePathKindV1.ReadFailed, 0); }
        }

        public ICostumeFileWriteSessionV1 OpenTempForWrite(string fullPath) { return new WindowsWriteSession(new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None)); }
        public void MoveNoOverwrite(string sourcePath, string destinationPath) { File.Move(sourcePath, destinationPath); }
        public void Replace(string sourcePath, string destinationPath, string backupPath) { File.Replace(sourcePath, destinationPath, backupPath); }

        private sealed class WindowsWriteSession : ICostumeFileWriteSessionV1
        {
            private readonly FileStream _stream;
            public WindowsWriteSession(FileStream stream) { _stream = stream ?? throw new ArgumentNullException(nameof(stream)); }
            public void WriteExact(byte[] bytes) { if (bytes == null) throw new ArgumentNullException(nameof(bytes)); _stream.Write(bytes, 0, bytes.Length); }
            public void FlushManaged() { _stream.Flush(); }
            public void FlushDurable() { _stream.Flush(true); }
            public void Dispose() { _stream.Dispose(); }
        }
    }
}
