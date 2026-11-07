using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace AcadeGameMaker.Profile.Tests
{
    // REQ-M5D7QC2-001/005/006; AC-M5D7QC2-001/005/006/009.  These are
    // engine-free C1-owner tests; Unity session ownership is covered elsewhere.
    public sealed class ProfileResetMemoryAuthorityV1Tests
    {
        [Test]
        public void AC001_ExactPreparedRowReauthenticatesAndExposesOnlyDefensiveScalars()
        {
            WithPrepared((root, proof) =>
            {
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof);
                    Assert.That(authority.Root, Is.EqualTo(ProfileResetBarrierV1.NormalizeRoot(root)));
                    Assert.That(authority.TargetRevision, Is.EqualTo(0));
                    Assert.That(authority.TargetByteLength, Is.GreaterThan(0));
                    Assert.That(authority.TransactionId, Has.Length.EqualTo(32));
                    Assert.That(authority.MarkerSha256, Has.Length.EqualTo(64));
                    Assert.That(authority.TargetSha256, Has.Length.EqualTo(64));
                    Assert.That(authority.ArchiveProofDigest, Has.Length.EqualTo(64));
                    var detachedTarget = authority.CopyTargetBytes(); var first = detachedTarget[0]; detachedTarget[0] ^= 1;
                    Assert.That(authority.CopyTargetBytes()[0], Is.EqualTo(first), "authority target copy must not expose mutable proof bytes");
                }
            });
        }

        [Test]
        public void AC001_MutatedLiveMarkerFailsWithoutConsumingPreparedProof()
        {
            WithPrepared((root, proof) =>
            {
                var marker = Path.Combine(root, ProfileResetBarrierV1.BarrierName);
                var exact = File.ReadAllBytes(marker); File.WriteAllText(marker, "{}");
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                    Assert.Throws<ProfileResetMemoryDiskAuthorityExceptionV1>(() => ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof));
                File.WriteAllBytes(marker, exact);
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                    Assert.That(ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof), Is.Not.Null);
            });
        }

        [Test]
        public void AC001_ForeignRootRejectsBeforeProofConsumption()
        {
            WithPrepared((root, proof) =>
            {
                var other = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C2-foreign-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(other);
                try
                {
                    using (var lease = ProfileRootOperationLockV1.Acquire(other))
                        Assert.Throws<InvalidOperationException>(() => ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, other, proof));
                    using (var lease = ProfileRootOperationLockV1.Acquire(root))
                        Assert.That(ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof), Is.Not.Null);
                }
                finally { Directory.Delete(other, true); }
            });
        }

        [Test]
        public void AC005_AfterActualDeleteFaultRetainsAttemptAndForbidsRetry()
        {
            WithPrepared((root, proof) =>
            {
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof);
                    var control = new ThrowingControl(ProfileResetMemoryAuthorityCheckpointV1.AfterBarrierDelete);
                    Assert.Throws<ProfileResetMemoryDiskAuthorityCheckpointException>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority, control));
                    Assert.That(File.Exists(Path.Combine(root, ProfileResetBarrierV1.BarrierName)), Is.False);
                    Assert.That(authority.DeleteAttempted, Is.True);
                    Assert.Throws<InvalidOperationException>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority));
                }
            });
        }

        [Test]
        public void AC005_BeforeDeleteFaultAndRealExclusiveMarkerLockDoNotProduceSuccess()
        {
            WithPrepared((root, proof) =>
            {
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof);
                    Assert.Throws<ProfileResetMemoryDiskAuthorityCheckpointException>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority, new ThrowingControl(ProfileResetMemoryAuthorityCheckpointV1.BeforeBarrierDelete)));
                    Assert.That(File.Exists(Path.Combine(root, ProfileResetBarrierV1.BarrierName)), Is.True);
                    Assert.Throws<InvalidOperationException>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority));
                }
            });
            WithPrepared((root, proof) =>
            {
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof);
                    using (new FileStream(Path.Combine(root, ProfileResetBarrierV1.BarrierName), FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        var failure = Assert.Throws<ProfileResetMemoryDiskAuthorityExceptionV1>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority));
                        Assert.That(failure.ManualRepairRequired, Is.False);
                    }
                    Assert.That(File.Exists(Path.Combine(root, ProfileResetBarrierV1.BarrierName)), Is.True);
                }
            });
            // The control opens this real handle at BeforeBarrierDelete, after
            // marker/archive reauthentication and MarkDeleteAttempted. It thus
            // denies only the direct BCL delete, not C1's exclusive reads.
            WithPrepared((root, proof) =>
            {
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof);
                    var marker = Path.Combine(root, ProfileResetBarrierV1.BarrierName);
                    using (var control = new DeleteDenyingControl(marker))
                    {
                        var failure = Assert.Throws<ProfileResetMemoryDiskAuthorityExceptionV1>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority, control));
                        Assert.That(failure.ManualRepairRequired, Is.False);
                        Assert.That(failure.InnerException, Is.TypeOf<IOException>());
                        Assert.That(authority.DeleteAttempted, Is.True, "direct delete attempt must be retained after a sharing failure");
                        Assert.That(File.Exists(marker), Is.True);
                        Assert.Throws<InvalidOperationException>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority));
                    }
                }
            });
        }

        [Test]
        public void AC005_MissingAndDirectoryMarkerAreReloadRequiredNotSuccess()
        {
            WithPrepared((root, proof) =>
            {
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof);
                    File.Delete(Path.Combine(root, ProfileResetBarrierV1.BarrierName));
                    var failure = Assert.Throws<ProfileResetMemoryDiskAuthorityExceptionV1>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority));
                    Assert.That(failure.ManualRepairRequired, Is.False);
                }
            });
            WithPrepared((root, proof) =>
            {
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof);
                    File.Delete(Path.Combine(root, ProfileResetBarrierV1.BarrierName)); Directory.CreateDirectory(Path.Combine(root, ProfileResetBarrierV1.BarrierName));
                    var failure = Assert.Throws<ProfileResetMemoryDiskAuthorityExceptionV1>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority));
                    Assert.That(failure.ManualRepairRequired, Is.False);
                }
            });
        }

        [Test]
        public void AC006_ArchiveMutationAfterDeleteFailsFinalReprobeWithoutBarrierRecreation()
        {
            WithRoot(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                var proof = result.PreparedProof;
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof);
                    ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority);
                    var archive = Path.Combine(root, "recovery", "new-game", authority.TransactionId, "primary.bin");
                    Assert.That(File.Exists(archive), Is.True); File.WriteAllBytes(archive, new byte[] { 1, 2, 3 });
                    var failure = Assert.Throws<ProfileResetMemoryDiskAuthorityExceptionV1>(() => ProfileResetDiskTransactionV1.VerifyFinalDiskUnderHeldRoot(lease, authority));
                    Assert.That(failure.ManualRepairRequired, Is.False);
                    Assert.That(File.Exists(Path.Combine(root, ProfileResetBarrierV1.BarrierName)), Is.False);
                }
            });
        }

        [TestCase("primary")]
        [TestCase("previous")]
        [TestCase("temp")]
        [TestCase("barrier-temp")]
        [TestCase("barrier")]
        [TestCase("archive-present")]
        [TestCase("archive-missing")]
        public void AC006_EachFinalDiskRowMutationRejectsReceiptPrecondition(string mutation)
        {
            WithRoot(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, result.PreparedProof);
                    ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority);
                    if (mutation == "primary") File.WriteAllBytes(Path.Combine(root, "profile.json"), new byte[] { 7 });
                    else if (mutation == "previous") File.WriteAllBytes(Path.Combine(root, "profile.prev.json"), new byte[] { 7 });
                    else if (mutation == "temp") File.WriteAllBytes(Path.Combine(root, "profile.tmp.json"), new byte[] { 7 });
                    else if (mutation == "barrier-temp") File.WriteAllBytes(Path.Combine(root, ProfileResetBarrierV1.BarrierTempName), new byte[] { 7 });
                    else if (mutation == "barrier") File.WriteAllBytes(Path.Combine(root, ProfileResetBarrierV1.BarrierName), new byte[] { 7 });
                    else if (mutation == "archive-present") File.WriteAllBytes(Path.Combine(root, "recovery", "new-game", authority.TransactionId, "primary.bin"), new byte[] { 7 });
                    else File.WriteAllBytes(Path.Combine(root, "recovery", "new-game", authority.TransactionId, "previous.bin"), new byte[] { 7 });
                    var failure = Assert.Throws<ProfileResetMemoryDiskAuthorityExceptionV1>(() => ProfileResetDiskTransactionV1.VerifyFinalDiskUnderHeldRoot(lease, authority));
                    Assert.That(failure.ManualRepairRequired, Is.False);
                }
            });
        }

        [Test]
        public void AC006_ExactFinalRowVerifiesOnlyUnderTheOriginalLiveLease()
        {
            WithRoot(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                var leaseA = ProfileRootOperationLockV1.Acquire(root); ValidatedResetMemoryAuthorityV1 authority;
                try
                {
                    authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(leaseA, root, result.PreparedProof);
                    ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(leaseA, authority);
                    Assert.DoesNotThrow(() => ProfileResetDiskTransactionV1.VerifyFinalDiskUnderHeldRoot(leaseA, authority));
                }
                finally { leaseA.Dispose(); }
                using (var leaseB = ProfileRootOperationLockV1.Acquire(root)) Assert.Throws<InvalidOperationException>(() => ProfileResetDiskTransactionV1.VerifyFinalDiskUnderHeldRoot(leaseB, authority));
            });
        }

        [Test]
        public void AC006_FinalDiskProofIsDetachedExactAndDefensivelyCopied()
        {
            WithRoot(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, result.PreparedProof);
                    ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority);
                    var proof = ProfileResetDiskTransactionV1.CaptureFinalDiskProofUnderHeldRoot(lease, authority);
                    proof.Validate(); Assert.That(proof.Root, Is.EqualTo(ProfileResetBarrierV1.NormalizeRoot(root))); Assert.That(proof.TransactionId, Is.EqualTo(authority.TransactionId));
                    Assert.That(proof.TargetRevision, Is.EqualTo(0)); Assert.That(proof.BarrierAbsent && proof.BarrierTempAbsent, Is.True);
                    var copy = proof.CopyTargetBytes(); var first = copy[0]; copy[0] ^= 1; Assert.That(proof.CopyTargetBytes()[0], Is.EqualTo(first));
                    foreach (var field in typeof(ValidatedResetFinalDiskProofV1).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                        Assert.That(typeof(ProfileRootOperationLockV1).IsAssignableFrom(field.FieldType) || typeof(ValidatedResetMemoryAuthorityV1).IsAssignableFrom(field.FieldType), Is.False, "final proof must not retain live authority");
                }
            });
        }

        [Test]
        public void AC006_FinalDiskProofCannotMintBeforeDeletionOrAfterReflectionCorruption()
        {
            WithPrepared((root, proof) =>
            {
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof);
                    Assert.Throws<InvalidOperationException>(() => ProfileResetDiskTransactionV1.CaptureFinalDiskProofUnderHeldRoot(lease, authority));
                    Assert.Throws<InvalidOperationException>(() => ValidatedResetFinalDiskProofV1.MintAfterProfileFinalProbe(null, authority));
                    var forged = ValidatedResetFinalDiskProofV1.MintAfterProfileFinalProbe(new object(), authority);
                    Assert.Throws<InvalidOperationException>(() => forged.Validate());
                }
            });
            WithPrepared((root, proof) =>
            {
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof);
                    ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority);
                    var final = ProfileResetDiskTransactionV1.CaptureFinalDiskProofUnderHeldRoot(lease, authority);
                    typeof(ValidatedResetFinalDiskProofV1).GetField("_barrierAbsent", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(final, false);
                    Assert.Throws<InvalidOperationException>(() => final.Validate());
                    typeof(ValidatedResetFinalDiskProofV1).GetField("_barrierAbsent", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(final, true);
                    typeof(ValidatedResetFinalDiskProofV1).GetField("_root", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(final, Path.Combine(root, "foreign-final-proof-root"));
                    Assert.Throws<InvalidOperationException>(() => final.Validate(), "CWT registration must bind the actual captured root, not only the proof object");
                    typeof(ValidatedResetFinalDiskProofV1).GetField("_root", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(final, ProfileResetBarrierV1.NormalizeRoot(root));
                    typeof(ValidatedResetFinalDiskProofV1).GetField("_mintWitness", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(final, new object());
                    Assert.Throws<InvalidOperationException>(() => final.Validate());
                }
            });
        }

        [Test]
        public void AC001_OldPrimaryEqualToDefaultStillReauthenticatesThroughArchiveProof()
        {
            WithRoot(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                    Assert.That(ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, result.PreparedProof), Is.Not.Null);
            });
        }

        [Test]
        public void AC005_ActualBarrierJunctionFailsClosedWithoutMutatingJunctionTarget()
        {
            using (var fixture = new ProfileResetMemoryJunctionFixtureV1())
            {
                var result = ProfileResetDiskTransactionV1.Begin(fixture.Root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(fixture.Root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                using (var lease = ProfileRootOperationLockV1.Acquire(fixture.Root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, fixture.Root, result.PreparedProof);
                    var barrier = Path.Combine(fixture.Root, ProfileResetBarrierV1.BarrierName); File.Delete(barrier); fixture.CreateJunction(barrier, fixture.Target);
                    var failure = Assert.Throws<ProfileResetMemoryDiskAuthorityExceptionV1>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority));
                    Assert.That(failure.ManualRepairRequired, Is.False); fixture.AssertJunction(barrier); fixture.AssertSentinel();
                }
            }
        }

        [Test]
        public void AC001_ActualArchiveJunctionFailsReauthenticationPathWithoutForeignLockCreation()
        {
            using (var fixture = new ProfileResetMemoryJunctionFixtureV1())
            {
                var result = ProfileResetDiskTransactionV1.Begin(fixture.Root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(fixture.Root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                using (var lease = ProfileRootOperationLockV1.Acquire(fixture.Root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, fixture.Root, result.PreparedProof);
                    var transaction = Path.Combine(fixture.Root, "recovery", "new-game", authority.TransactionId); Directory.Delete(transaction, true); fixture.CreateJunction(transaction, fixture.Target);
                    var failure = Assert.Throws<ProfileResetMemoryDiskAuthorityExceptionV1>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority));
                    Assert.That(failure.ManualRepairRequired, Is.False); fixture.AssertJunction(transaction); fixture.AssertSentinel();
                }
            }
        }

        [Test]
        public void AC003_ActualRootAndAncestorJunctionsFailPreleaseValidationWithoutForeignLockCreation()
        {
            using (var fixture = new ProfileResetMemoryJunctionFixtureV1())
            {
                Assert.That(ProfileResetDiskTransactionV1.ValidateMemoryResetRootBeforeLease(fixture.Root), Is.EqualTo(ProfileResetBarrierV1.NormalizeRoot(fixture.Root)));
                var ownedBase = Path.GetDirectoryName(fixture.Root);
                var rootLink = Path.Combine(ownedBase, "root-link"); fixture.CreateJunction(rootLink, fixture.Target);
                Assert.Throws<IOException>(() => ProfileResetDiskTransactionV1.ValidateMemoryResetRootBeforeLease(rootLink)); fixture.AssertJunction(rootLink); fixture.AssertSentinel();
            }
            using (var fixture = new ProfileResetMemoryJunctionFixtureV1())
            {
                var ownedBase = Path.GetDirectoryName(fixture.Root); Directory.CreateDirectory(Path.Combine(fixture.Target, "child"));
                var ancestorLink = Path.Combine(ownedBase, "ancestor-link"); fixture.CreateJunction(ancestorLink, fixture.Target);
                Assert.Throws<IOException>(() => ProfileResetDiskTransactionV1.ValidateMemoryResetRootBeforeLease(Path.Combine(ancestorLink, "child")));
                fixture.AssertJunction(ancestorLink); fixture.AssertSentinel();
            }
        }

        [Test]
        public void AC005_ActualLockDirectoryAndJunctionFailPreleaseValidationWithoutForeignLockCreation()
        {
            using (var fixture = new ProfileResetMemoryJunctionFixtureV1())
            {
                Directory.CreateDirectory(Path.Combine(fixture.Root, "profile.operation.lock"));
                Assert.Throws<IOException>(() => ProfileResetDiskTransactionV1.ValidateMemoryResetRootBeforeLease(fixture.Root)); fixture.AssertSentinel();
            }
            using (var fixture = new ProfileResetMemoryJunctionFixtureV1())
            {
                var lockPath = Path.Combine(fixture.Root, "profile.operation.lock"); fixture.CreateJunction(lockPath, fixture.Target);
                Assert.Throws<IOException>(() => ProfileResetDiskTransactionV1.ValidateMemoryResetRootBeforeLease(fixture.Root)); fixture.AssertJunction(lockPath); fixture.AssertSentinel();
            }
        }

        [TestCase("primary")]
        [TestCase("previous")]
        [TestCase("temp")]
        [TestCase("barrier-temp")]
        [TestCase("archive")]
        public void AC001_EachLivePreparedRowMutationFailsBeforeProofCas(string mutation)
        {
            WithRoot(root =>
            {
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                if (mutation == "primary") File.WriteAllBytes(Path.Combine(root, "profile.json"), new byte[] { 1 });
                else if (mutation == "previous") File.WriteAllBytes(Path.Combine(root, "profile.prev.json"), new byte[] { 1 });
                else if (mutation == "temp") File.WriteAllBytes(Path.Combine(root, "profile.tmp.json"), new byte[] { 1 });
                else if (mutation == "barrier-temp") File.WriteAllBytes(Path.Combine(root, ProfileResetBarrierV1.BarrierTempName), new byte[] { 1 });
                else Directory.CreateDirectory(Path.Combine(root, "recovery", "new-game", result.TransactionId, "primary.bin"));
                using (var lease = ProfileRootOperationLockV1.Acquire(root)) Assert.Throws<ProfileResetMemoryDiskAuthorityExceptionV1>(() => ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, result.PreparedProof));
                Assert.That(result.PreparedProof.TryConsume(), Is.True, "live disk failure must precede proof CAS");
            });
        }

        [TestCase("primary.bin")]
        [TestCase("previous.bin")]
        [TestCase("temp.bin")]
        public void AC001_PresentDeclaredArchiveMutationFailsBeforeProofCas(string archiveLeaf)
        {
            WithRoot(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                File.WriteAllBytes(Path.Combine(root, "profile.prev.json"), new byte[] { 1, 2 }); File.WriteAllBytes(Path.Combine(root, "profile.tmp.json"), new byte[] { 3, 4 });
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                File.WriteAllBytes(Path.Combine(root, "recovery", "new-game", result.TransactionId, archiveLeaf), new byte[] { 9 });
                using (var lease = ProfileRootOperationLockV1.Acquire(root)) Assert.Throws<ProfileResetMemoryDiskAuthorityExceptionV1>(() => ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, result.PreparedProof));
                Assert.That(result.PreparedProof.TryConsume(), Is.True);
            });
        }

        [Test]
        public void AC001_DuplicateAndReflectionMutatedProofCannotMintAuthority()
        {
            WithPrepared((root, proof) =>
            {
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    Assert.That(ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof), Is.Not.Null);
                    Assert.Throws<InvalidOperationException>(() => ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof));
                }
            });
            WithPrepared((root, proof) =>
            {
                var bytes = (byte[])typeof(ProfileResetDiskPreparedProofV1).GetField("_targetBytes", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(proof); bytes[0] ^= 1;
                using (var lease = ProfileRootOperationLockV1.Acquire(root)) Assert.Throws<InvalidOperationException>(() => ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof));
            });
        }

        [Test]
        public void AC001_DisposedIssuerLeaseAndReflectedAuthorityPayloadAreClosed()
        {
            WithPrepared((root, proof) =>
            {
                ValidatedResetMemoryAuthorityV1 authority;
                var leaseA = ProfileRootOperationLockV1.Acquire(root);
                try { authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(leaseA, root, proof); }
                finally { leaseA.Dispose(); }
                using (var leaseB = ProfileRootOperationLockV1.Acquire(root)) Assert.Throws<InvalidOperationException>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(leaseB, authority));
            });
            WithPrepared((root, proof) =>
            {
                using (var lease = ProfileRootOperationLockV1.Acquire(root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, root, proof);
                    typeof(ValidatedResetMemoryAuthorityV1).GetField("_targetByteLength", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(authority, -1L);
                    Assert.Throws<InvalidOperationException>(() => { var ignored = authority.TargetSha256; });
                    Assert.Throws<InvalidOperationException>(() => ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority));
                }
            });
        }

        private sealed class ThrowingControl : IProfileResetMemoryAuthorityTestControlV1
        {
            private readonly ProfileResetMemoryAuthorityCheckpointV1 _checkpoint;
            internal ThrowingControl(ProfileResetMemoryAuthorityCheckpointV1 checkpoint) { _checkpoint = checkpoint; }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint) { if (checkpoint == _checkpoint) throw new ProfileResetMemoryDiskAuthorityCheckpointException(); }
        }
        private sealed class DeleteDenyingControl : IProfileResetMemoryAuthorityTestControlV1, IDisposable
        {
            private readonly string _marker; private FileStream _handle;
            internal DeleteDenyingControl(string marker) { _marker = marker; }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint)
            {
                if (checkpoint != ProfileResetMemoryAuthorityCheckpointV1.BeforeBarrierDelete) return;
                if (_handle != null) throw new InvalidOperationException("Delete-denying test handle was opened twice.");
                _handle = new FileStream(_marker, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            public void Dispose() { if (_handle != null) { _handle.Dispose(); _handle = null; } }
        }
        private sealed class ProfileResetMemoryDiskAuthorityCheckpointException : Exception { }
        private static void WithPrepared(Action<string, ProfileResetDiskPreparedProofV1> test)
        {
            WithRoot(root => { var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root)); Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared)); test(root, result.PreparedProof); });
        }
        private static void WithRoot(Action<string> test)
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C2-authority-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
            try { test(root); }
            finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
        }
    }
}
