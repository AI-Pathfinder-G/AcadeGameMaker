using System;
using System.IO;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D7QC-006/007; AC-M5D7QC-004/007. This bounded foundation has no
    // reset mutation authority; it proves ordinary entrypoints fail closed.
    public sealed class ProfileResetBarrierInterceptionV1Tests
    {
        [Test]
        public void AC007_AnyCommittedBarrierBlocksOrdinaryLoadAndSaveBeforePreviousRecovery()
        {
            WithDirectory(root =>
            {
                var previous = Document(7);
                File.WriteAllBytes(Path.Combine(root, "profile.prev.json"), previous.CanonicalFileBytes);
                File.WriteAllText(Path.Combine(root, "profile.reset.json"), "not-a-valid-barrier");

                var load = Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileLaunchObservationAdapterV1.Observe(root));
                var save = Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileAtomicSaveServiceV1.Save(root, Document(0)));

                Assert.That(load.Outcome, Is.EqualTo(ProfileRootOperationOutcomeV1.ResetBarrierPresent));
                Assert.That(save.Outcome, Is.EqualTo(ProfileRootOperationOutcomeV1.ResetBarrierPresent));
                CollectionAssert.AreEqual(previous.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(root, "profile.prev.json")));
                Assert.That(File.Exists(Path.Combine(root, "profile.json")), Is.False);
            });
        }

        [Test]
        public void AC004_HeldRootOperationLockReturnsBusyAndDoesNotMutateFiles()
        {
            WithDirectory(root =>
            {
                var primary = Document(3); File.WriteAllBytes(Path.Combine(root, "profile.json"), primary.CanonicalFileBytes);
                using (ProfileRootOperationLockV1.Acquire(root))
                {
                    var blocked = Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileAtomicSaveServiceV1.Save(root, Document(4)));
                    Assert.That(blocked.Outcome, Is.EqualTo(ProfileRootOperationOutcomeV1.Busy));
                }
                CollectionAssert.AreEqual(primary.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(root, "profile.json")));
            });
        }

        [Test]
        public void AC007_BarrierDirectoryIsTerminalAndCannotFallThroughToPrevious()
        {
            WithDirectory(root =>
            {
                var previous = Document(9);
                File.WriteAllBytes(Path.Combine(root, "profile.prev.json"), previous.CanonicalFileBytes);
                Directory.CreateDirectory(Path.Combine(root, "profile.reset.json"));

                var blocked = Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileLaunchObservationAdapterV1.Observe(root));

                Assert.That(blocked.Outcome, Is.EqualTo(ProfileRootOperationOutcomeV1.ResetBarrierPresent));
                CollectionAssert.AreEqual(previous.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(root, "profile.prev.json")));
            });
        }

        [Test]
        public void AC007_GuardedTwoArgumentInputRecoveryIsBlockedByBarrier()
        {
            WithDirectory(root =>
            {
                var source = RecoveryCandidate(5);
                File.WriteAllBytes(Path.Combine(root, "profile.json"), source.DecodeResult.Document.CanonicalFileBytes);
                File.WriteAllText(Path.Combine(root, "profile.reset.json"), "barrier");

                var blocked = Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileAtomicSaveServiceV1.SaveInputRecovery(root, source));

                Assert.That(blocked.Outcome, Is.EqualTo(ProfileRootOperationOutcomeV1.ResetBarrierPresent));
                Assert.That(File.Exists(Path.Combine(root, "profile.prev.json")), Is.False);
            });
        }

        [Test]
        // REQ-M5D7QC-006/007; partial AC-M5D7QC-004/007. Held-root entry
        // points accept only the live lease for their exact normalized root.
        public void AC004_AC007_HeldRootEntryPointsRejectWrongRootBeforeProfileIo()
        {
            var ownerRoot = CreateDirectory(); var wrongRoot = CreateDirectory();
            try
            {
                var recovery = RecoveryCandidate(5);
                using (var ownership = ProfileRootOperationLockV1.Acquire(ownerRoot))
                {
                    Assert.Throws<InvalidOperationException>(() => ProfileLaunchObservationAdapterV1.ObserveUnderHeldRoot(ownership, wrongRoot));
                    Assert.Throws<InvalidOperationException>(() => ProfileAtomicSaveServiceV1.SaveUnderHeldRoot(ownership, wrongRoot, Document(0)));
                    Assert.Throws<InvalidOperationException>(() => ProfileAtomicSaveServiceV1.SaveInputRecoveryUnderHeldRoot(ownership, wrongRoot, recovery));
                }
                Assert.That(File.Exists(Path.Combine(wrongRoot, "profile.json")), Is.False);
                Assert.That(File.Exists(Path.Combine(wrongRoot, "profile.prev.json")), Is.False);
                Assert.That(File.Exists(Path.Combine(wrongRoot, "profile.tmp.json")), Is.False);
                Assert.That(File.Exists(Path.Combine(wrongRoot, "profile.operation.lock")), Is.False);
            }
            finally { DeleteDirectory(ownerRoot); DeleteDirectory(wrongRoot); }
        }

        [Test]
        // REQ-M5D7QC-006/007; partial AC-M5D7QC-004/007. Disposing a lease
        // invalidates every held-root route before it can observe or mutate.
        public void AC004_AC007_HeldRootEntryPointsRejectDisposedLeaseBeforeProfileIo()
        {
            WithDirectory(root =>
            {
                var recovery = RecoveryCandidate(5); var ownership = ProfileRootOperationLockV1.Acquire(root); ownership.Dispose();
                Assert.Throws<InvalidOperationException>(() => ProfileLaunchObservationAdapterV1.ObserveUnderHeldRoot(ownership, root));
                Assert.Throws<InvalidOperationException>(() => ProfileAtomicSaveServiceV1.SaveUnderHeldRoot(ownership, root, Document(0)));
                Assert.Throws<InvalidOperationException>(() => ProfileAtomicSaveServiceV1.SaveInputRecoveryUnderHeldRoot(ownership, root, recovery));
                Assert.That(File.Exists(Path.Combine(root, "profile.json")), Is.False);
                Assert.That(File.Exists(Path.Combine(root, "profile.prev.json")), Is.False);
                Assert.That(File.Exists(Path.Combine(root, "profile.tmp.json")), Is.False);
            });
        }

        private static ProfileCanonicalDocumentV1 Document(long revision)
        {
            var settings = new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false);
            var input = new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, ProfileInputContract.CurrentBindingSchemaVersion, ProfileBindingOverridesJson.Parse(string.Empty));
            var tutorial = new ProfileTutorialSnapshot(new[] { "tutorial.complete" });
            var progression = new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>());
            return ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(settings, input, tutorial, progression));
        }
        private static ProfileLoadCandidateV1 RecoveryCandidate(long revision)
        {
            var settings = new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false);
            var input = new ProfileInputSnapshot("obsolete-input", ProfileInputContract.CurrentBindingSchemaVersion, ProfileBindingOverridesJson.Parse(string.Empty));
            var snapshot = new ProfileSnapshotV1(settings, input, new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
            var decoded = ProfileCanonicalDecoderV1.Decode(ProfileCanonicalEncoder.Encode(snapshot).CanonicalFileBytes);
            Assert.That(decoded.Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired));
            return ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Primary, decoded);
        }
        private static void WithDirectory(Action<string> test)
        {
            var root = CreateDirectory(); try { test(root); } finally { DeleteDirectory(root); }
        }
        private static string CreateDirectory() { var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-M5D7QC-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root); return root; }
        private static void DeleteDirectory(string root) { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }
}
