using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D7QC-006/007. Engine-free root ownership and barrier interception.
    // Reset mutation/resume, memory cutover, UI, and scene effects are excluded.
    public enum ProfileRootOperationOutcomeV1 { Busy = 1, ResetBarrierPresent = 2 }

    public sealed class ProfileRootOperationBlockedExceptionV1 : IOException
    {
        public ProfileRootOperationOutcomeV1 Outcome { get; private set; }
        internal ProfileRootOperationBlockedExceptionV1(ProfileRootOperationOutcomeV1 outcome, string message) : base(message) { Outcome = outcome; }
    }

    // REQ-M5D7QC3L-002: these internal exceptions preserve authenticated
    // acquisition provenance for the observation-only C3 lane.  Legacy C1
    // callers continue to receive ProfileRootOperationBlockedExceptionV1.
    internal enum ProfileResetObservationAcquireFailureV1 { RootCreation = 1, LockOpen = 2, ContentionTimeout = 3 }
    internal sealed class ProfileResetObservationUnavailableExceptionV1 : IOException
    {
        internal readonly ProfileResetObservationAcquireFailureV1 Failure;
        internal ProfileResetObservationUnavailableExceptionV1(ProfileResetObservationAcquireFailureV1 failure, Exception cause)
            : base("Profile observation acquisition is unavailable.", cause)
        {
            if (failure != ProfileResetObservationAcquireFailureV1.RootCreation && failure != ProfileResetObservationAcquireFailureV1.LockOpen)
                throw new ArgumentOutOfRangeException(nameof(failure));
            Failure = failure;
        }
    }
    internal sealed class ProfileRootOperationLockV1 : IDisposable
    {
        private const int TimeoutMilliseconds = 5000;
        private readonly string _root;
        private FileStream _stream;
        private sealed class ObservationContentionTimeoutException : IOException
        {
            internal ObservationContentionTimeoutException(IOException cause) : base("Profile observation acquisition timed out.", cause) { }
        }
        private sealed class ObservationTimeoutWitness
        {
            internal readonly IOException Cause; internal readonly int CauseHResult, TimeoutHResult;
            internal ObservationTimeoutWitness(IOException cause, IOException timeout) { Cause = cause; CauseHResult = cause.HResult; TimeoutHResult = timeout.HResult; }
        }
        private static readonly ConditionalWeakTable<IOException, ObservationTimeoutWitness> ObservationTimeouts = new ConditionalWeakTable<IOException, ObservationTimeoutWitness>();
        private ProfileRootOperationLockV1(string root, FileStream stream) { _root = root; _stream = stream; }
        internal static ProfileRootOperationLockV1 Acquire(string root)
        {
            var normalized = ProfileResetBarrierV1.NormalizeRoot(root);
            try { Directory.CreateDirectory(normalized); }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is SecurityException)
            { throw new ProfileRootOperationBlockedExceptionV1(ProfileRootOperationOutcomeV1.Busy, "Profile root is unavailable."); }
            var path = Path.Combine(normalized, "profile.operation.lock");
            var until = Environment.TickCount + TimeoutMilliseconds;
            while (true)
            {
                try { return new ProfileRootOperationLockV1(normalized, new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)); }
                catch (IOException)
                {
                    if (unchecked(Environment.TickCount - until) >= 0)
                        throw new ProfileRootOperationBlockedExceptionV1(ProfileRootOperationOutcomeV1.Busy, "Profile root is busy; reload is required.");
                    System.Threading.Thread.Sleep(25);
                }
                catch (UnauthorizedAccessException)
                { throw new ProfileRootOperationBlockedExceptionV1(ProfileRootOperationOutcomeV1.Busy, "Profile root is unavailable."); }
                catch (SecurityException)
                { throw new ProfileRootOperationBlockedExceptionV1(ProfileRootOperationOutcomeV1.Busy, "Profile root is unavailable."); }
            }
        }
        // REQ-M5D7QC3L-001..003: separate observation-only lane.  It returns
        // the same actual lease object, while preserving unavailable versus
        // genuine Windows sharing contention without changing legacy Acquire.
        internal static ProfileRootOperationLockV1 AcquireForObservation(string root)
        {
            var normalized = ProfileResetBarrierV1.NormalizeRoot(root);
            try { Directory.CreateDirectory(normalized); }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is SecurityException)
            { throw new ProfileResetObservationUnavailableExceptionV1(ProfileResetObservationAcquireFailureV1.RootCreation, e); }
            var path = Path.Combine(normalized, "profile.operation.lock");
            var until = Environment.TickCount + TimeoutMilliseconds;
            while (true)
            {
                try { return new ProfileRootOperationLockV1(normalized, new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)); }
                catch (IOException e)
                {
                    if (!IsWindowsSharingViolation(e)) throw new ProfileResetObservationUnavailableExceptionV1(ProfileResetObservationAcquireFailureV1.LockOpen, e);
                    if (unchecked(Environment.TickCount - until) >= 0) throw MintAuthenticatedObservationTimeout(e);
                    System.Threading.Thread.Sleep(25);
                }
                catch (UnauthorizedAccessException e) { throw new ProfileResetObservationUnavailableExceptionV1(ProfileResetObservationAcquireFailureV1.LockOpen, e); }
                catch (SecurityException e) { throw new ProfileResetObservationUnavailableExceptionV1(ProfileResetObservationAcquireFailureV1.LockOpen, e); }
            }
        }
        internal static bool IsWindowsSharingViolation(IOException error)
        {
            return error != null && (error.HResult == unchecked((int)0x80070020) || error.HResult == unchecked((int)0x80070021));
        }
        // This is the only C3-visible timeout discriminator. It cannot create
        // evidence and accepts only the exact IOException emitted by this loop.
        internal static bool IsAuthenticatedObservationContentionTimeout(IOException timeout)
        {
            return timeout != null && ObservationTimeouts.TryGetValue(timeout, out var witness) && witness != null &&
                ReferenceEquals(timeout.InnerException, witness.Cause) && timeout.HResult == witness.TimeoutHResult &&
                witness.Cause.HResult == witness.CauseHResult && IsWindowsSharingViolation(witness.Cause);
        }
        private static IOException MintAuthenticatedObservationTimeout(IOException cause)
        {
            if (cause == null || !IsWindowsSharingViolation(cause)) throw new InvalidOperationException("Observation timeout cause is invalid.");
            var timeout = new ObservationContentionTimeoutException(cause);
            ObservationTimeouts.Add(timeout, new ObservationTimeoutWitness(cause, timeout));
            return timeout;
        }
        // A held-root operation receives the actual unforgeable lease, not a
        // boolean or thread-local claim. A disposed or wrong-root lease fails
        // before any ordinary profile work can occur.
        internal void ValidateOwnership(string root)
        {
            if (_stream == null || !string.Equals(_root, ProfileResetBarrierV1.NormalizeRoot(root), StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Profile root operation ownership is invalid.");
        }
        public void Dispose() { if (_stream != null) { _stream.Dispose(); _stream = null; } }
    }

    internal static class ProfileResetBarrierV1
    {
        internal const string BarrierName = "profile.reset.json";
        internal const string BarrierTempName = "profile.reset.tmp.json";
        internal static string NormalizeRoot(string root)
        {
            if (string.IsNullOrWhiteSpace(root) || !Path.IsPathFullyQualified(root)) throw new ArgumentException("Profile root must be fully qualified.", nameof(root));
            var full = Path.GetFullPath(root); var drive = Path.GetPathRoot(full);
            full = full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (string.IsNullOrEmpty(full) || string.Equals(full, drive.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Profile root cannot be a filesystem root.", nameof(root));
            return full;
        }
        internal static void ThrowIfPresent(string root)
        {
            var path = Path.Combine(NormalizeRoot(root), BarrierName);
            // File.Exists collapses access failures and directories to false. A
            // barrier name that cannot be positively proved absent is terminal:
            // ordinary recovery must never select Previous through that state.
            try
            {
                var attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.Directory) != 0)
                    throw new ProfileRootOperationBlockedExceptionV1(ProfileRootOperationOutcomeV1.ResetBarrierPresent, "Reset barrier path is a directory; manual repair is required.");
                throw new ProfileRootOperationBlockedExceptionV1(ProfileRootOperationOutcomeV1.ResetBarrierPresent, "A reset transaction requires validated resume or manual repair.");
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }
            catch (UnauthorizedAccessException)
            { throw new ProfileRootOperationBlockedExceptionV1(ProfileRootOperationOutcomeV1.ResetBarrierPresent, "Reset barrier cannot be inspected; manual repair is required."); }
            catch (SecurityException)
            { throw new ProfileRootOperationBlockedExceptionV1(ProfileRootOperationOutcomeV1.ResetBarrierPresent, "Reset barrier cannot be inspected; manual repair is required."); }
            catch (IOException)
            { throw new ProfileRootOperationBlockedExceptionV1(ProfileRootOperationOutcomeV1.ResetBarrierPresent, "Reset barrier cannot be inspected; manual repair is required."); }
        }
    }

}
