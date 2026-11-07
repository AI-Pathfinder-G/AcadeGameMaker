using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace AcadeGameMaker.Profile.Tests
{
    // AC-M5D7QC1-001/002/004/007 focused deterministic disk ownership tests.
    public sealed class ProfileResetDiskTransactionV1Tests
    {
        [Test]
        public void AC001_StaleIdentityDoesNotPublishBarrierOrMoveLeaves()
        {
            WithRoot(root =>
            {
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                File.WriteAllText(Path.Combine(root, "profile.json"), "not-a-profile");
                var result = ProfileResetDiskTransactionV1.Begin(root, identity);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ConfirmationStale));
                Assert.That(File.ReadAllText(Path.Combine(root, "profile.json")), Is.EqualTo("not-a-profile"));
                Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.False);
            });
        }

        [Test]
        public void AC002_AC004_ArchivesInvalidBytesAndWritesOnlyExactR0()
        {
            WithRoot(root =>
            {
                var old = new byte[] { 0, 255, 7, 9 }; File.WriteAllBytes(Path.Combine(root, "profile.json"), old);
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                var archive = Path.Combine(root, "recovery", "new-game", result.TransactionId, "primary.bin");
                CollectionAssert.AreEqual(old, File.ReadAllBytes(archive));
                CollectionAssert.AreEqual(ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(root, "profile.json")));
                Assert.That(File.Exists(Path.Combine(root, "profile.prev.json")), Is.False);
                Assert.That(File.Exists(Path.Combine(root, "profile.tmp.json")), Is.False);
            });
        }

        [Test]
        public void AC007_ResumeWithoutBarrierFailsClosed()
        {
            WithRoot(root => Assert.That(ProfileResetDiskTransactionV1.Resume(root).Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ManualRepairRequired)));
        }

        [Test]
        public void AC004_FullyPreparedBarrierResumesWithoutCreatingAnotherArchive()
        {
            WithRoot(root =>
            {
                var first = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                var second = ProfileResetDiskTransactionV1.Resume(root);
                Assert.That(first.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                Assert.That(second.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                Assert.That(second.TransactionId, Is.EqualTo(first.TransactionId));
            });
        }

        [Test]
        public void AC006_ExactPrimaryWithInterruptedTempPreservesTempThenPrepares()
        {
            WithRoot(root =>
            {
                var first = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                var bytes = new byte[] { 8, 7, 6 }; File.WriteAllBytes(Path.Combine(root, "profile.tmp.json"), bytes);
                var resumed = ProfileResetDiskTransactionV1.Resume(root);
                Assert.That(resumed.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                var saved = Path.Combine(root, "recovery", "new-game", first.TransactionId, "interrupted-default-temp");
                Assert.That(Directory.GetFiles(saved).Length, Is.EqualTo(1));
                CollectionAssert.AreEqual(bytes, File.ReadAllBytes(Directory.GetFiles(saved)[0]));
            });
        }

        [TestCase("{}")]
        [TestCase("[]")]
        [TestCase("{\"format\":\"wrong\"}")]
        public void AC002_MalformedBarrierVariantsFailClosed(string marker)
        {
            WithRoot(root =>
            {
                File.WriteAllText(Path.Combine(root, "profile.reset.json"), marker);
                Assert.That(ProfileResetDiskTransactionV1.Resume(root).Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ManualRepairRequired));
            });
        }

        [Test]
        public void AC009_DefaultResultFailsClosed()
        {
            Assert.Throws<InvalidOperationException>(() => default(ProfileResetDiskResultV1).Validate());
        }

        [Test]
        public void AC009_ReflectionCorruptedPreparedResultFailsClosed()
        {
            WithRoot(root =>
            {
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                object boxed = result;
                typeof(ProfileResetDiskResultV1).GetField("_targetLength", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, -1L);
                var corrupt = (ProfileResetDiskResultV1)boxed;
                Assert.Throws<InvalidOperationException>(() => corrupt.Validate());
            });
        }

        [TestCase("_transactionId", "00000000000000000000000000000000")]
        [TestCase("_targetHash", "0000000000000000000000000000000000000000000000000000000000000000")]
        public void AC009_ReflectionCorruptedPreparedScalarFailsClosed(string field, string value)
        {
            WithRoot(root =>
            {
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                object boxed = result;
                typeof(ProfileResetDiskResultV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value);
                Assert.Throws<InvalidOperationException>(() => ((ProfileResetDiskResultV1)boxed).Validate());
            });
        }

        [Test]
        public void AC009_ReflectionCorruptedProofRootFailsClosed()
        {
            WithRoot(root =>
            {
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                var proof = typeof(ProfileResetDiskResultV1).GetField("_proof", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(result);
                typeof(ProfileResetDiskPreparedProofV1).GetField("_root", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(proof, Path.GetTempPath());
                Assert.Throws<InvalidOperationException>(() => result.Validate());
            });
        }

        [Test]
        public void AC009_CoupledProofLeafAndArchiveMutationStillFailsAgainstMarker()
        {
            WithRoot(root =>
            {
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                var proof = typeof(ProfileResetDiskResultV1).GetField("_proof", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(result);
                var leaves = (Leaf[])typeof(ProfileResetDiskPreparedProofV1).GetField("_leaves", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(proof);
                typeof(Leaf).GetField("Role", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(leaves[0], "Temp");
                var archive = (ProfileResetDiskLeafProofV1[])typeof(ProfileResetDiskResultV1).GetField("_archive", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(result);
                object proofLeaf = archive[0];
                typeof(ProfileResetDiskLeafProofV1).GetField("_role", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(proofLeaf, "Temp");
                archive[0] = (ProfileResetDiskLeafProofV1)proofLeaf;
                Assert.Throws<InvalidOperationException>(() => result.Validate());
            });
        }

        [TestCase(0, "_role", "Temp")]
        [TestCase(1, "_name", "profile.json")]
        [TestCase(2, "_role", "Previous")]
        public void AC009_ReflectionCorruptedActiveRoleMappingFailsClosed(int index, string field, string value)
        {
            WithRoot(root =>
            {
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                var active = (ProfileResetDiskLeafProofV1[])typeof(ProfileResetDiskResultV1).GetField("_active", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(result);
                object leaf = active[index];
                typeof(ProfileResetDiskLeafProofV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(leaf, value);
                active[index] = (ProfileResetDiskLeafProofV1)leaf;
                Assert.Throws<InvalidOperationException>(() => result.Validate());
            });
        }

        [Test]
        public void AC008_BusyDoesNotConsumeIdentity()
        {
            WithRoot(root =>
            {
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                using (ProfileRootOperationLockV1.Acquire(root))
                {
                    Assert.That(ProfileResetDiskTransactionV1.Begin(root, identity).Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.Busy));
                }
                Assert.That(ProfileResetDiskTransactionV1.Begin(root, identity).Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
            });
        }

        [Test]
        public void AC001_IdentityCannotBeUsedTwice()
        {
            WithRoot(root =>
            {
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                Assert.That(ProfileResetDiskTransactionV1.Begin(root, identity).Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                Assert.Throws<InvalidOperationException>(() => ProfileResetDiskTransactionV1.Begin(root, identity));
            });
        }

        private static void WithRoot(Action<string> action)
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C1-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
            try { action(root); } finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
        }
    }
}
