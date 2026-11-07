using System;
using System.IO;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Profile.Tests
{
    // REQ-M5D7QC1-002..007; AC-M5D7QC1-003/006/008. These use the real BCL
    // operations through the coordinator; no alternate transaction exists.
    // Injection is one-shot because SaveHeld may legitimately reprobe a leaf
    // while building its typed failure diagnostics.
    public sealed class ProfileResetDiskCheckpointV1Tests
    {
        [TestCase(13)] [TestCase(14)] [TestCase(15)] [TestCase(16)] [TestCase(17)] [TestCase(18)]
        [TestCase(19)] [TestCase(20)] [TestCase(21)] [TestCase(22)] [TestCase(23)] [TestCase(24)]
        [TestCase(25)] [TestCase(26)] [TestCase(27)] [TestCase(28)] [TestCase(29)] [TestCase(30)]
        [TestCase(31)] [TestCase(32)] [TestCase(33)] [TestCase(34)] [TestCase(35)] [TestCase(36)]
        [TestCase(37)] [TestCase(38)] [TestCase(39)] [TestCase(40)] [TestCase(41)] [TestCase(42)]
        [TestCase(43)] [TestCase(44)] [TestCase(45)] [TestCase(46)] [TestCase(47)] [TestCase(48)]
        [TestCase(49)] [TestCase(50)] [TestCase(51)]
        public void AC003_AC006_RealBclRecoverableCheckpointRetainsBarrierAndRestarts(int checkpointValue)
        {
            var checkpoint = (ProfileResetDiskCheckpointV1)checkpointValue;
            WithPublishedMarker(checkpoint, (root, old) =>
            {
                var control = new ThrowIoAt(checkpoint);
                var failed = ProfileResetDiskTransactionV1.Resume(root, control);
                failed.Validate();
                Assert.That(failed.Outcome, Is.Not.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                Assert.That(control.Hits, Is.EqualTo(1));
                Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.True);

                var resumed = ProfileResetDiskTransactionV1.Resume(root);
                resumed.Validate();
                Assert.That(resumed.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                AssertNoActiveOldBytes(root, old);
                foreach (var bytes in old) AssertBytesExist(root, bytes);
            });
        }

        [TestCase(13)] [TestCase(14)] [TestCase(15)] [TestCase(16)] [TestCase(17)] [TestCase(18)]
        [TestCase(19)] [TestCase(20)] [TestCase(21)] [TestCase(22)] [TestCase(23)] [TestCase(24)]
        [TestCase(25)] [TestCase(26)] [TestCase(27)] [TestCase(28)] [TestCase(29)] [TestCase(30)]
        [TestCase(31)] [TestCase(32)] [TestCase(33)] [TestCase(34)] [TestCase(35)] [TestCase(36)]
        [TestCase(37)] [TestCase(38)] [TestCase(39)] [TestCase(40)] [TestCase(41)] [TestCase(42)]
        [TestCase(43)] [TestCase(44)] [TestCase(45)] [TestCase(46)] [TestCase(47)] [TestCase(48)]
        [TestCase(49)] [TestCase(50)] [TestCase(51)]
        public void AC003_AC006_TestOnlyInterruptionEscapesThenRestartsWithoutResurrection(int checkpointValue)
        {
            var checkpoint = (ProfileResetDiskCheckpointV1)checkpointValue;
            WithPublishedMarker(checkpoint, (root, old) =>
            {
                var control = new ThrowInterruptionAt(checkpoint);
                Assert.Throws<InjectedInterruptionException>(() => ProfileResetDiskTransactionV1.Resume(root, control));
                Assert.That(control.Hits, Is.EqualTo(1));
                Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.True);

                var resumed = ProfileResetDiskTransactionV1.Resume(root);
                resumed.Validate();
                Assert.That(resumed.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                AssertNoActiveOldBytes(root, old);
                foreach (var bytes in old) AssertBytesExist(root, bytes);
            });
        }

        private static void WithPublishedMarker(ProfileResetDiskCheckpointV1 checkpoint, Action<string, byte[][]> test)
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C1-checkpoint-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
            var old = new[] { new byte[] { 31, 1 }, new byte[] { 32, 2 }, new byte[] { 33, 3 } };
            try
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), old[0]);
                File.WriteAllBytes(Path.Combine(root, "profile.prev.json"), old[1]);
                File.WriteAllBytes(Path.Combine(root, "profile.tmp.json"), old[2]);
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                var published = ProfileResetDiskTransactionV1.Begin(root, identity, new ThrowIoAt(ProfileResetDiskCheckpointV1.AfterMarkerCommittedReopen));
                published.Validate(); Assert.That(published.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ManualRepairRequired));
                if (checkpoint >= ProfileResetDiskCheckpointV1.BeforeInterruptedTempMove && checkpoint <= ProfileResetDiskCheckpointV1.AfterInterruptedPrimaryReopen)
                {
                    var prepared = ProfileResetDiskTransactionV1.Resume(root);
                    prepared.Validate(); Assert.That(prepared.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                    var interrupted = checkpoint <= ProfileResetDiskCheckpointV1.AfterInterruptedTempReopen ? new byte[] { 71, 7 } : new byte[] { 72, 8 };
                    File.WriteAllBytes(Path.Combine(root, checkpoint <= ProfileResetDiskCheckpointV1.AfterInterruptedTempReopen ? "profile.tmp.json" : "profile.json"), interrupted);
                    Array.Resize(ref old, old.Length + 1); old[old.Length - 1] = interrupted;
                }
                test(root, old);
            }
            finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
        }

        private static void AssertNoActiveOldBytes(string root, byte[][] old)
        {
            foreach (var name in new[] { "profile.json", "profile.prev.json", "profile.tmp.json" })
            {
                var path = Path.Combine(root, name); if (!File.Exists(path)) continue;
                var active = File.ReadAllBytes(path);
                foreach (var bytes in old) CollectionAssert.AreNotEqual(bytes, active, "Old bytes were resurrected into an active leaf.");
            }
        }
        private static void AssertBytesExist(string root, byte[] expected)
        {
            foreach (var path in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
            {
                var actual = File.ReadAllBytes(path);
                if (actual.Length != expected.Length) continue;
                var same = true; for (var index = 0; index < actual.Length; index++) if (actual[index] != expected[index]) { same = false; break; }
                if (same) return;
            }
            Assert.Fail("Archived bytes were not preserved.");
        }
        private sealed class ThrowIoAt : IProfileResetDiskTestControlV1
        {
            private readonly ProfileResetDiskCheckpointV1 _checkpoint; internal int Hits { get; private set; }
            internal ThrowIoAt(ProfileResetDiskCheckpointV1 checkpoint) { _checkpoint = checkpoint; }
            public void Checkpoint(ProfileResetDiskCheckpointV1 checkpoint) { if (checkpoint == _checkpoint && Hits == 0) { Hits++; throw new IOException("Injected checkpoint IO fault."); } }
        }
        private sealed class ThrowInterruptionAt : IProfileResetDiskTestControlV1
        {
            private readonly ProfileResetDiskCheckpointV1 _checkpoint; internal int Hits { get; private set; }
            internal ThrowInterruptionAt(ProfileResetDiskCheckpointV1 checkpoint) { _checkpoint = checkpoint; }
            public void Checkpoint(ProfileResetDiskCheckpointV1 checkpoint) { if (checkpoint == _checkpoint && Hits == 0) { Hits++; throw new InjectedInterruptionException(); } }
        }
        private sealed class InjectedInterruptionException : Exception { }
    }
}
