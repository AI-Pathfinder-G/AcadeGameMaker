using System;
using System.IO;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Profile.Tests
{
    // REQ-M5D7QC1-002; AC-M5D7QC1-003. Real BCL-root marker boundary faults.
    public sealed class ProfileResetDiskFaultV1Tests
    {
        [TestCase((int)ProfileResetDiskCheckpointV1.BeforeMarkerWrite)]
        [TestCase((int)ProfileResetDiskCheckpointV1.AfterMarkerWrite)]
        [TestCase((int)ProfileResetDiskCheckpointV1.BeforeMarkerFlush)]
        [TestCase((int)ProfileResetDiskCheckpointV1.AfterMarkerFlush)]
        [TestCase((int)ProfileResetDiskCheckpointV1.BeforeMarkerClose)]
        [TestCase((int)ProfileResetDiskCheckpointV1.AfterMarkerClose)]
        [TestCase((int)ProfileResetDiskCheckpointV1.BeforeMarkerTempReopen)]
        public void AC003_PreCommitMarkerFaultLeavesActiveAuthoritative(int checkpointValue)
        {
            var checkpoint = (ProfileResetDiskCheckpointV1)checkpointValue;
            WithRoot(root =>
            {
                var old = new byte[] { 6, 2, 9 }; File.WriteAllBytes(Path.Combine(root, "profile.json"), old);
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                var result = ProfileResetDiskTransactionV1.Begin(root, identity, new ThrowAt(checkpoint));
                result.Validate(); Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ManualRepairRequired));
                CollectionAssert.AreEqual(old, File.ReadAllBytes(Path.Combine(root, "profile.json")));
                Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.False);
            });
        }

        [TestCase((int)ProfileResetDiskCheckpointV1.AfterMarkerTempReopen)]
        [TestCase((int)ProfileResetDiskCheckpointV1.BeforeMarkerCommitMove)]
        [TestCase((int)ProfileResetDiskCheckpointV1.AfterMarkerCommitMove)]
        [TestCase((int)ProfileResetDiskCheckpointV1.BeforeMarkerCommittedReopen)]
        [TestCase((int)ProfileResetDiskCheckpointV1.AfterMarkerCommittedReopen)]
        public void AC003_PostPublicationFaultNeverMovesOldLeafBeforeValidatedResume(int checkpointValue)
        {
            var checkpoint = (ProfileResetDiskCheckpointV1)checkpointValue;
            WithRoot(root =>
            {
                var old = new byte[] { 8, 8, 1 }; File.WriteAllBytes(Path.Combine(root, "profile.json"), old);
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                var result = ProfileResetDiskTransactionV1.Begin(root, identity, new ThrowAt(checkpoint));
                result.Validate(); Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ManualRepairRequired));
                CollectionAssert.AreEqual(old, File.ReadAllBytes(Path.Combine(root, "profile.json")));
            });
        }

        private sealed class ThrowAt : IProfileResetDiskTestControlV1
        {
            private readonly ProfileResetDiskCheckpointV1 _checkpoint;
            internal ThrowAt(ProfileResetDiskCheckpointV1 checkpoint) { _checkpoint = checkpoint; }
            public void Checkpoint(ProfileResetDiskCheckpointV1 checkpoint) { if (checkpoint == _checkpoint) throw new IOException("injected checkpoint fault"); }
        }
        private static void WithRoot(Action<string> test)
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C1-fault-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
            try { test(root); } finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
        }
    }
}
