using System;
using System.IO;
using System.Reflection;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Profile.Tests
{
    // REQ-M5D7QC1-004/005/006; AC-M5D7QC1-005/006.  The decorator calls the
    // actual BCL file port and injects only at its concrete save boundaries.
    public sealed class ProfileResetDiskSaveFaultV1Tests
    {
        [TestCase(1, 0)] [TestCase(1, 1)] [TestCase(2, 0)] [TestCase(2, 1)]
        [TestCase(3, 0)] [TestCase(3, 1)] [TestCase(4, 0)] [TestCase(4, 1)]
        public void AC005_AC006_DefaultPrecommitAfterIoFaultRetainsBarrierAndDoesNotCommit(int faultValue, int afterValue)
        {
            var fault = (SaveFault)faultValue;
            WithPreparedFirstSave((root, lease, marker, document) =>
            {
                var result = ProfileAtomicSaveServiceV1.SaveResetFirstUnderHeldRoot(lease, marker, root, document, new BclDecorator(root, fault, afterValue == 0 ? FaultMode.BeforeRecoverable : FaultMode.AfterRecoverable));
                Assert.That(result.Outcome, Is.EqualTo(ProfileAtomicSaveOutcome.FailedBeforeCommit));
                Assert.That(result.FailureStage, Is.EqualTo(fault == SaveFault.TempRead ? ProfileAtomicSaveStage.TempReadValidation : ProfileAtomicSaveStage.TempWriteFlushClose));
                Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.True);
                Assert.That(File.Exists(Path.Combine(root, "profile.json")), Is.False);
            });
        }

        [TestCase(5, 0)] [TestCase(5, 1)] [TestCase(6, 0)] [TestCase(6, 1)]
        [TestCase(7, 0)] [TestCase(7, 1)] [TestCase(8, 0)] [TestCase(8, 1)]
        public void AC005_AC006_DefaultCommitOrPostcheckAfterIoFaultIsUncertainAndRetainsBarrier(int faultValue, int afterValue)
        {
            WithPreparedFirstSave((root, lease, marker, document) =>
            {
                var result = ProfileAtomicSaveServiceV1.SaveResetFirstUnderHeldRoot(lease, marker, root, document, new BclDecorator(root, (SaveFault)faultValue, afterValue == 0 ? FaultMode.BeforeRecoverable : FaultMode.AfterRecoverable));
                Assert.That(result.Outcome, Is.EqualTo(ProfileAtomicSaveOutcome.CommitOutcomeUncertain));
                Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.True);
            });
        }

        [TestCase(1, 0)] [TestCase(1, 1)] [TestCase(2, 0)] [TestCase(2, 1)]
        [TestCase(3, 0)] [TestCase(3, 1)] [TestCase(4, 0)] [TestCase(4, 1)]
        [TestCase(5, 0)] [TestCase(5, 1)] [TestCase(6, 0)] [TestCase(6, 1)]
        [TestCase(7, 0)] [TestCase(7, 1)] [TestCase(8, 0)] [TestCase(8, 1)]
        public void AC005_AC006_SimulatedInterruptionEscapesAndRestartPreservesBytes(int faultValue, int afterValue)
        {
            var fault = (SaveFault)faultValue;
            byte[] primaryBeforeRestart = null;
            byte[] tempBeforeRestart = null;
            byte[] archivedOld = null;
            WithPreparedFirstSave((root, lease, marker, document) =>
            {
                archivedOld = File.ReadAllBytes(Path.Combine(root, "recovery", "new-game", marker.Id, "primary.bin"));
                var port = new BclDecorator(root, fault, afterValue == 0 ? FaultMode.BeforeInterruption : FaultMode.AfterInterruption);
                Assert.Throws<InjectedInterruptionException>(() => ProfileAtomicSaveServiceV1.SaveResetFirstUnderHeldRoot(lease, marker, root, document, port));
                Assert.That(port.OpenWriterCalls, Is.EqualTo(1));
                Assert.That(port.MoveCalls, Is.LessThanOrEqualTo(1));
                primaryBeforeRestart = ReadIfPresent(Path.Combine(root, "profile.json"));
                tempBeforeRestart = ReadIfPresent(Path.Combine(root, "profile.tmp.json"));
                CollectionAssert.AreEqual(archivedOld, File.ReadAllBytes(Path.Combine(root, "recovery", "new-game", marker.Id, "primary.bin")));
            }, root =>
            {
                var document = ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument;
                Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileAtomicSaveServiceV1.Save(root, document));
                Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileLaunchObservationAdapterV1.Observe(root));
                var resumed = ProfileResetDiskTransactionV1.Resume(root);
                resumed.Validate();
                Assert.That(resumed.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                AssertBytesRemain(root, archivedOld);
                if (primaryBeforeRestart != null) AssertBytesRemain(root, primaryBeforeRestart);
                if (tempBeforeRestart != null) AssertBytesRemain(root, tempBeforeRestart);
            });
        }

        private enum SaveFault { Write = 1, Flush = 2, Close = 3, TempRead = 4, Move = 5, PostPrimaryRead = 6, PreviousProbe = 7, TempProbe = 8 }
        private enum FaultMode { BeforeRecoverable = 1, AfterRecoverable = 2, BeforeInterruption = 3, AfterInterruption = 4 }
        private sealed class InjectedAfterIOException : IOException { internal InjectedAfterIOException(string message) : base(message) { } }
        // This intentionally does not inherit IOException: it escapes the
        // recoverable core catch and models a test-only interruption, not death.
        private sealed class InjectedInterruptionException : Exception { internal InjectedInterruptionException(string message) : base(message) { } }

        private sealed class BclDecorator : IProfileAtomicSaveFileOperations
        {
            private readonly string _root; private readonly SaveFault _fault; private readonly FaultMode _mode; private readonly IProfileAtomicSaveFileOperations _inner = ProfileAtomicSaveBclFileOperations.Instance;
            internal int OpenWriterCalls { get; private set; } internal int MoveCalls { get; private set; }
            internal BclDecorator(string root, SaveFault fault, FaultMode mode) { _root = root; _fault = fault; _mode = mode; }
            public void EnsureDirectory(string path) { _inner.EnsureDirectory(path); }
            public IProfileAtomicSaveTempWriter OpenTempExclusive(string path) { OpenWriterCalls++; return new Writer(_inner.OpenTempExclusive(path), this); }
            public ProfileAtomicSaveFilePresence ProbePresence(string path)
            {
                var point = LeafFault(path); Hit(point); var value = _inner.ProbePresence(path); HitAfter(point); return value;
            }
            public byte[] ReadAllExclusive(string path)
            {
                var point = string.Equals(path, Path.Combine(_root, "profile.tmp.json"), StringComparison.OrdinalIgnoreCase) ? SaveFault.TempRead : string.Equals(path, Path.Combine(_root, "profile.json"), StringComparison.OrdinalIgnoreCase) ? SaveFault.PostPrimaryRead : (SaveFault?)null;
                Hit(point); var value = _inner.ReadAllExclusive(path); HitAfter(point); return value;
            }
            public void Replace(string tempPath, string primaryPath, string previousPath) { _inner.Replace(tempPath, primaryPath, previousPath); }
            public void Move(string tempPath, string primaryPath) { Hit(SaveFault.Move); MoveCalls++; _inner.Move(tempPath, primaryPath); HitAfter(SaveFault.Move); }
            private SaveFault? LeafFault(string path) { if (string.Equals(path, Path.Combine(_root, "profile.prev.json"), StringComparison.OrdinalIgnoreCase)) return SaveFault.PreviousProbe; if (string.Equals(path, Path.Combine(_root, "profile.tmp.json"), StringComparison.OrdinalIgnoreCase)) return SaveFault.TempProbe; return null; }
            private void Hit(SaveFault? point) { if (point.HasValue && point.Value == _fault && (_mode == FaultMode.BeforeRecoverable || _mode == FaultMode.BeforeInterruption)) Throw(); }
            private void HitAfter(SaveFault? point) { if (point.HasValue && point.Value == _fault && (_mode == FaultMode.AfterRecoverable || _mode == FaultMode.AfterInterruption)) Throw(); }
            private void Throw() { if (_mode == FaultMode.BeforeInterruption || _mode == FaultMode.AfterInterruption) throw new InjectedInterruptionException("Injected simulated interruption."); throw new InjectedAfterIOException("Injected BCL IO fault."); }
            private sealed class Writer : IProfileAtomicSaveTempWriter
            {
                private readonly IProfileAtomicSaveTempWriter _inner; private readonly BclDecorator _owner;
                internal Writer(IProfileAtomicSaveTempWriter inner, BclDecorator owner) { _inner = inner; _owner = owner; }
                public void WriteAll(byte[] bytes) { _owner.Hit(SaveFault.Write); _inner.WriteAll(bytes); _owner.HitAfter(SaveFault.Write); }
                public void FlushToStorage() { _owner.Hit(SaveFault.Flush); _inner.FlushToStorage(); _owner.HitAfter(SaveFault.Flush); }
                public void Close()
                {
                    // The core treats this as a close-boundary failure; close
                    // the real BCL handle first so the injected test never
                    // leaks a lock into its cleanup path.
                    if (_owner._fault == SaveFault.Close && (_owner._mode == FaultMode.BeforeRecoverable || _owner._mode == FaultMode.BeforeInterruption)) { _inner.Close(); _owner.Throw(); }
                    _inner.Close(); _owner.HitAfter(SaveFault.Close);
                }
            }
        }

        private static void WithPreparedFirstSave(Action<string, ProfileRootOperationLockV1, ValidatedProfileResetBarrierV1, ProfileCanonicalDocumentV1> test, Action<string> afterLease = null)
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C1-save-fault-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
            try
            {
                var old = new byte[] { 9, 4, 2 }; File.WriteAllBytes(Path.Combine(root, "profile.json"), old);
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                var publication = ProfileResetDiskTransactionV1.Begin(root, identity, new ThrowAt(ProfileResetDiskCheckpointV1.AfterMarkerCommittedReopen));
                Assert.That(publication.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ManualRepairRequired));
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var marker = ReadAndBind(root, lease);
                    var archive = Path.Combine(root, "recovery", "new-game", marker.Id, "primary.bin");
                    File.Move(Path.Combine(root, "profile.json"), archive);
                    test(root, lease, marker, ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument);
                }
                if (afterLease != null) afterLease(root);
            }
            finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
        }
        private static byte[] ReadIfPresent(string path) { return File.Exists(path) ? File.ReadAllBytes(path) : null; }
        private static void AssertBytesRemain(string root, byte[] expected)
        {
            Assert.That(expected, Is.Not.Null);
            foreach (var path in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
            {
                var actual = File.ReadAllBytes(path);
                if (actual.Length != expected.Length) continue;
                var equal = true; for (var i = 0; i < actual.Length; i++) if (actual[i] != expected[i]) { equal = false; break; }
                if (equal) return;
            }
            Assert.Fail("Restart did not preserve the interrupted bytes.");
        }
        private static ValidatedProfileResetBarrierV1 ReadAndBind(string root, ProfileRootOperationLockV1 lease)
        {
            var method = typeof(ProfileResetDiskTransactionV1).GetMethod("ReadMarker", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            var marker = (ValidatedProfileResetBarrierV1)method.Invoke(null, new object[] { root });
            return ValidatedProfileResetBarrierV1.BindLease(marker, lease);
        }
        private sealed class ThrowAt : IProfileResetDiskTestControlV1
        {
            private readonly ProfileResetDiskCheckpointV1 _point; internal ThrowAt(ProfileResetDiskCheckpointV1 point) { _point = point; }
            public void Checkpoint(ProfileResetDiskCheckpointV1 point) { if (point == _point) throw new IOException("Stop after marker publication."); }
        }
    }
}
