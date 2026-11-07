using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AcadeGameMaker.Profile.Tests
{
    // REQ-M5D7QC1-004/006; AC-M5D7QC1-005/009.  This covers process-local
    // proof integrity only. C2 must explicitly invoke TryConsume before it
    // treats a prepared proof as a one-shot cutover authority.
    public sealed class ProfileResetDiskProofV1Tests
    {
        [TestCase("_id", "ffffffffffffffffffffffffffffffff")]
        [TestCase("_root", "C:\\Temp\\AcadeGameMaker-C1-proof-other-root")]
        public void AC009_MutatedProofScalarRejectsWithoutBurningItsConsumer(string fieldName, string replacement)
        {
            WithPreparedProof((proof, result) =>
            {
                var field = ProofField(fieldName); var original = field.GetValue(proof);
                if (fieldName == "_id" && string.Equals((string)original, replacement, StringComparison.Ordinal)) replacement = "00000000000000000000000000000000";
                field.SetValue(proof, replacement);
                Assert.Throws<InvalidOperationException>(() => proof.TryConsume());
                field.SetValue(proof, original);
                Assert.That(proof.TryConsume(), Is.True);
            });
        }

        [TestCase("Classification", ProfileResetLeafClassificationV1.Invalid)]
        [TestCase("Role", "Temp")]
        [TestCase("Hash", "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")]
        public void AC009_MutatedProofLeafRejectsWithoutBurningItsConsumer(string fieldName, object replacement)
        {
            WithPreparedProof((proof, result) =>
            {
                var leaves = (Leaf[])ProofField("_leaves").GetValue(proof);
                var field = typeof(Leaf).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Assert.That(field, Is.Not.Null);
                var original = field.GetValue(leaves[0]);
                field.SetValue(leaves[0], replacement);
                Assert.Throws<InvalidOperationException>(() => proof.TryConsume());
                field.SetValue(leaves[0], original);
                Assert.That(proof.TryConsume(), Is.True);
            });
        }

        [Test]
        public void AC009_MutatedProofMarkerBytesRejectWithoutBurningItsConsumer()
        {
            WithPreparedProof((proof, result) =>
            {
                var bytes = (byte[])ProofField("_markerBytes").GetValue(proof); var original = bytes[0];
                bytes[0] ^= 1;
                Assert.Throws<InvalidOperationException>(() => proof.TryConsume());
                bytes[0] = original;
                Assert.That(proof.TryConsume(), Is.True);
            });
        }

        [Test]
        public void AC009_InvalidConsumptionStateFailsAndTheNormalDuplicateIsFalse()
        {
            WithPreparedProof((proof, result) =>
            {
                var field = ProofField("_consumed"); field.SetValue(proof, 2);
                Assert.Throws<InvalidOperationException>(() => proof.TryConsume());
                field.SetValue(proof, 0);
                Assert.That(proof.TryConsume(), Is.True);
                Assert.That(proof.TryConsume(), Is.False);
            });
        }

        [Test]
        public void AC009_ConcurrentProofConsumersHaveExactlyOneWinnerWithoutDiskIo()
        {
            WithPreparedProof((proof, result) =>
            {
                var successes = 0;
                Parallel.For(0, 64, ignored => { if (proof.TryConsume()) Interlocked.Increment(ref successes); });
                Assert.That(successes, Is.EqualTo(1));
            });
        }

        [Test]
        public void AC009_PreparedProofGetterObservesButDoesNotConsume()
        {
            WithPreparedProof((proof, result) =>
            {
                Assert.That(result.PreparedProof, Is.SameAs(proof));
                Assert.That(proof.TryConsume(), Is.True);
            });
        }

        [Test]
        public void AC005_ActualIssuingLeaseInstanceIsRequiredBeforeAnyResetSaveMutation()
        {
            WithRoot(root =>
            {
                var exactDefault = ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument;
                File.WriteAllBytes(Path.Combine(root, "profile.json"), exactDefault.CanonicalFileBytes);
                var prepared = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(prepared.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));

                ValidatedProfileResetBarrierV1 markerFromA;
                using (var leaseA = ProfileRootOperationLockV1.Acquire(root)) markerFromA = ReadMarkerBindLease(root, leaseA);

                // Begin made the immutable old archive through C1. Remove only
                // this test's reset product to construct the active-absent row.
                File.Delete(Path.Combine(root, "profile.json"));
                using (var leaseB = ProfileRootOperationLockV1.Acquire(root))
                {
                    Assert.Throws<InvalidOperationException>(() => ProfileAtomicSaveServiceV1.SaveResetFirstUnderHeldRoot(leaseB, markerFromA, root, exactDefault));
                    Assert.That(File.Exists(Path.Combine(root, "profile.json")), Is.False);
                    Assert.That(File.Exists(Path.Combine(root, "profile.tmp.json")), Is.False);

                    var markerFromB = ReadMarkerBindLease(root, leaseB);
                    var saved = ProfileAtomicSaveServiceV1.SaveResetFirstUnderHeldRoot(leaseB, markerFromB, root, exactDefault);
                    Assert.That(saved.Outcome, Is.EqualTo(ProfileAtomicSaveOutcome.CommittedFirst));
                    CollectionAssert.AreEqual(exactDefault.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(root, "profile.json")));
                    Assert.That(File.Exists(Path.Combine(root, "profile.prev.json")), Is.False);
                    Assert.That(File.Exists(Path.Combine(root, "profile.tmp.json")), Is.False);
                }
            });
        }

        private static FieldInfo ProofField(string name)
        {
            var field = typeof(ProfileResetDiskPreparedProofV1).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null); return field;
        }

        private static void WithPreparedProof(Action<ProfileResetDiskPreparedProofV1, ProfileResetDiskResultV1> test)
        {
            WithRoot(root =>
            {
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                var proof = (ProfileResetDiskPreparedProofV1)typeof(ProfileResetDiskResultV1).GetField("_proof", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(result);
                test(proof, result);
            });
        }

        private static ValidatedProfileResetBarrierV1 ReadMarkerBindLease(string root, ProfileRootOperationLockV1 lease)
        {
            var method = typeof(ProfileResetDiskTransactionV1).GetMethod("ReadMarker", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            return ValidatedProfileResetBarrierV1.BindLease((ValidatedProfileResetBarrierV1)method.Invoke(null, new object[] { root }), lease);
        }

        private static void WithRoot(Action<string> test)
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C1-proof-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
            try { test(root); }
            finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
        }
    }
}
