using System;
using System.Globalization;
using System.IO;
using System.Security;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D7G-001..006. Engine-free, read-only, non-atomic observation of the three fixed launch leaves.
    internal enum ProfileObservationPresence { Missing = 1, Present = 2 }
    internal enum ProfileObservationReadCompletion { CompletedAndClosed = 1 }

    internal readonly struct ProfileObservationReadResult
    {
        internal readonly ProfileObservationReadCompletion Completion; internal readonly long ExpectedLength; internal readonly long BytesRead; internal readonly byte[] Bytes;
        internal ProfileObservationReadResult(ProfileObservationReadCompletion completion, long expectedLength, long bytesRead, byte[] bytes) { Completion = completion; ExpectedLength = expectedLength; BytesRead = bytesRead; Bytes = bytes; }
    }

    public readonly struct ProfileLaunchObservationBatchV1
    {
        private readonly ProfileLoadCandidateV1 _primary; private readonly ProfileLoadCandidateV1 _previous; private readonly ProfileLoadCandidateV1 _temp;
        internal ProfileLaunchObservationBatchV1(ProfileLoadCandidateV1 primary, ProfileLoadCandidateV1 previous, ProfileLoadCandidateV1 temp) { _primary = primary; _previous = previous; _temp = temp; }
        public ProfileLoadCandidateV1 Primary { get { Validate(); return _primary; } } public ProfileLoadCandidateV1 Previous { get { Validate(); return _previous; } } public ProfileLoadCandidateV1 Temp { get { Validate(); return _temp; } }
        public void Validate() { ValidateRole(_primary, ProfileLoadFileRole.Primary); ValidateRole(_previous, ProfileLoadFileRole.Previous); ValidateRole(_temp, ProfileLoadFileRole.Temp); }
        private static void ValidateRole(ProfileLoadCandidateV1 candidate, ProfileLoadFileRole role) { candidate.Validate(); if (candidate.Role != role) throw new InvalidOperationException("Observation batch role is malformed."); }
    }

    internal interface IProfileLaunchObservationPort { ProfileObservationPresence ProbePresence(string path); ProfileObservationReadResult ReadAllClosed(string path); }
    internal sealed class ProfileLaunchObservationBclPort : IProfileLaunchObservationPort
    {
        internal static readonly ProfileLaunchObservationBclPort Instance = new ProfileLaunchObservationBclPort(); private ProfileLaunchObservationBclPort() { }
        public ProfileObservationPresence ProbePresence(string path) { try { File.GetAttributes(path); return ProfileObservationPresence.Present; } catch (FileNotFoundException) { return ProfileObservationPresence.Missing; } catch (DirectoryNotFoundException) { return ProfileObservationPresence.Missing; } }
        public ProfileObservationReadResult ReadAllClosed(string path)
        {
            byte[] bytes; long expected; long readTotal = 0;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                expected = stream.Length; if (expected > int.MaxValue) throw new IOException("Profile file is too large.");
                bytes = new byte[(int)expected];
                while (readTotal < expected) { var read = stream.Read(bytes, (int)readTotal, (int)(expected - readTotal)); if (read == 0) throw new EndOfStreamException("Profile read ended early."); readTotal += read; }
            }
            return new ProfileObservationReadResult(ProfileObservationReadCompletion.CompletedAndClosed, expected, readTotal, bytes);
        }
    }

    public static class ProfileLaunchObservationAdapterV1
    {
        public static ProfileLaunchObservationBatchV1 Observe(string rootDirectoryPath)
        {
            // REQ-M5D7QC-007. A reset transaction is never allowed to fall
            // through into the ordinary Primary/Previous selector.
            using (ProfileRootOperationLockV1.Acquire(rootDirectoryPath))
            {
                ProfileResetBarrierV1.ThrowIfPresent(rootDirectoryPath);
                return Observe(rootDirectoryPath, ProfileLaunchObservationBclPort.Instance);
            }
        }

        // REQ-M5D7QC-007. The launch coordinator owns the same root lock
        // across observation, preservation and save. This path deliberately
        // does not reacquire that non-reentrant OS lock.
        internal static ProfileLaunchObservationBatchV1 ObserveUnderHeldRoot(ProfileRootOperationLockV1 ownership, string rootDirectoryPath)
        {
            if (ownership == null) throw new ArgumentNullException(nameof(ownership));
            ownership.ValidateOwnership(rootDirectoryPath);
            var root = ProfileResetBarrierV1.NormalizeRoot(rootDirectoryPath);
            ProfileResetBarrierV1.ThrowIfPresent(root);
            return Observe(root, ProfileLaunchObservationBclPort.Instance);
        }

        // Test-only injected observation seam. Runtime entrypoints use the
        // guarded one-argument overload above.
        internal static ProfileLaunchObservationBatchV1 Observe(string rootDirectoryPath, IProfileLaunchObservationPort port)
        {
            var root = ValidateRoot(rootDirectoryPath, port);
            var primary = ObserveRole(root, ProfileLoadFileRole.Primary, "profile.json", port);
            var previous = ObserveRole(root, ProfileLoadFileRole.Previous, "profile.prev.json", port);
            var temp = ObserveRole(root, ProfileLoadFileRole.Temp, "profile.tmp.json", port);
            return new ProfileLaunchObservationBatchV1(primary, previous, temp);
        }
        private static ProfileLoadCandidateV1 ObserveRole(string root, ProfileLoadFileRole role, string leaf, IProfileLaunchObservationPort port)
        {
            var path = Path.Combine(root, leaf);
            try { if (RequirePresence(port.ProbePresence(path)) == ProfileObservationPresence.Missing) return ProfileLoadCandidateV1.Missing(role); }
            catch (Exception exception) when (Recoverable(exception)) { return ProfileLoadCandidateV1.Unreadable(role); }
            try
            {
                var read = port.ReadAllClosed(path); ValidateRead(read);
                return ProfileLoadCandidateV1.Decoded(role, ProfileCanonicalDecoderV1.Decode(read.Bytes));
            }
            catch (Exception exception) when (Recoverable(exception)) { return ProfileLoadCandidateV1.Unreadable(role); }
        }
        private static void ValidateRead(ProfileObservationReadResult read)
        {
            if (read.Completion != ProfileObservationReadCompletion.CompletedAndClosed || read.Bytes == null || read.ExpectedLength < 0 || read.BytesRead < 0 || read.ExpectedLength > int.MaxValue || read.BytesRead > int.MaxValue || read.ExpectedLength != read.BytesRead || read.ExpectedLength != read.Bytes.Length) throw new InvalidOperationException("Read port violated the complete-and-closed protocol.");
        }
        private static string ValidateRoot(string root, IProfileLaunchObservationPort port)
        {
            if (port == null) throw new ArgumentNullException(nameof(port)); if (string.IsNullOrWhiteSpace(root) || !Path.IsPathFullyQualified(root)) throw new ArgumentException("Root must be fully qualified.", nameof(root));
            string normalized; try { normalized = Trim(Path.GetFullPath(root)); } catch (Exception exception) when (exception is ArgumentException || exception is NotSupportedException || exception is PathTooLongException) { throw new ArgumentException("Root path is malformed.", nameof(root), exception); }
            if (string.Equals(normalized, Path.GetPathRoot(normalized), StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Root cannot be a filesystem root.", nameof(root)); return normalized;
        }
        private static ProfileObservationPresence RequirePresence(ProfileObservationPresence value) { if (value != ProfileObservationPresence.Missing && value != ProfileObservationPresence.Present) throw new InvalidOperationException("Presence is malformed."); return value; }
        private static bool Recoverable(Exception exception) => exception is IOException || exception is UnauthorizedAccessException || exception is SecurityException;
        private static string Trim(string path) { var pathRoot = Path.GetPathRoot(path); var end = path.Length; while (end > pathRoot.Length && (path[end - 1] == Path.DirectorySeparatorChar || path[end - 1] == Path.AltDirectorySeparatorChar)) end--; return path.Substring(0, end); }
    }
}
