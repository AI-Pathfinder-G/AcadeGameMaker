using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace AcadeGameMaker.Profile.Tests
{
    // REQ-M5D7QC1-001..007; AC-M5D7QC1-001/002/005/006/007/009.
    public sealed class ProfileResetDiskResultV1Tests
    {
        [Test]
        public void AC009_PreparedCarriesFixedExactAndTargetOnlySnapshots()
        {
            WithRoot(root =>
            {
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                Assert.That(result.TargetRevision, Is.EqualTo(0));
                Assert.That(result.ArchiveSnapshots, Has.Length.EqualTo(3));
                Assert.That(result.ActiveSnapshots, Has.Length.EqualTo(3));
                Assert.That(result.ArchiveSnapshots[0].Proof, Is.EqualTo(ProfileResetDiskArchiveProofV1.NotApplicable));
                Assert.That(result.ActiveSnapshots[0].Observation, Is.EqualTo(ProfileResetDiskObservationV1.Readable));
                Assert.That(result.ActiveSnapshots[1].Observation, Is.EqualTo(ProfileResetDiskObservationV1.Missing));
                Assert.That(result.ActiveSnapshots[2].Observation, Is.EqualTo(ProfileResetDiskObservationV1.Missing));
                Assert.That(result.ActiveSnapshots[0].Classification, Is.EqualTo(ProfileResetLeafClassificationV1.ValidCurrentInput));
                Assert.That(result.ActiveSnapshots[0].DecodedRevision, Is.EqualTo(0));
            });
        }

        [Test]
        public void AC009_ArchiveExactCarriesDeclaredClassificationAndRevision()
        {
            WithRoot(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                Assert.That(result.ArchiveSnapshots[0].Proof, Is.EqualTo(ProfileResetDiskArchiveProofV1.Exact));
                Assert.That(result.ArchiveSnapshots[0].Classification, Is.EqualTo(ProfileResetLeafClassificationV1.ValidCurrentInput));
                Assert.That(result.ArchiveSnapshots[0].DecodedRevision, Is.EqualTo(0));
            });
        }

        [Test]
        public void AC009_AuthenticatedMissingArchiveDirectoryReportsUnreadableInsteadOfThrowing()
        {
            WithRoot(root =>
            {
                var prepared = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                var transaction = Path.Combine(root, "recovery", "new-game", prepared.TransactionId);
                Directory.Delete(transaction, true);
                var result = ProfileResetDiskTransactionV1.Resume(root);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ManualRepairRequired));
                Assert.That(result.TransactionId, Is.EqualTo(prepared.TransactionId));
                Assert.That(result.TargetRevision, Is.EqualTo(0));
                Assert.That(result.ArchiveSnapshots[0].Observation, Is.EqualTo(ProfileResetDiskObservationV1.Unreadable));
                Assert.That(result.ArchiveSnapshots[1].Observation, Is.EqualTo(ProfileResetDiskObservationV1.Unreadable));
                Assert.That(result.ArchiveSnapshots[2].Observation, Is.EqualTo(ProfileResetDiskObservationV1.Unreadable));
            });
        }

        [Test]
        public void AC009_AuthenticatedUnexpectedArchiveForMissingLeafIsReadableMismatch()
        {
            WithRoot(root =>
            {
                File.WriteAllText(Path.Combine(root, "profile.json"), "old primary");
                var prepared = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                var unexpected = Path.Combine(root, "recovery", "new-game", prepared.TransactionId, "previous.bin");
                File.WriteAllText(unexpected, "unexpected previous archive");
                var result = ProfileResetDiskTransactionV1.Resume(root);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ManualRepairRequired));
                Assert.That(result.TransactionId, Is.EqualTo(prepared.TransactionId));
                Assert.That(result.ArchiveSnapshots[1].Proof, Is.EqualTo(ProfileResetDiskArchiveProofV1.Mismatch));
                Assert.That(result.ArchiveSnapshots[1].Observation, Is.EqualTo(ProfileResetDiskObservationV1.Readable));
            });
        }

        [Test]
        public void AC001_StaleResultHasNoAuthenticatedArchivePath()
        {
            WithRoot(root =>
            {
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                File.WriteAllText(Path.Combine(root, "profile.json"), "changed");
                var result = ProfileResetDiskTransactionV1.Begin(root, identity);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ConfirmationStale));
                Assert.That(result.TransactionId, Is.Null);
                Assert.That(result.ArchiveSnapshots[0].Proof, Is.EqualTo(ProfileResetDiskArchiveProofV1.NotObserved));
            });
        }

        [Test]
        public void AC009_BusyDoesNotProbeOrAuthenticateResult()
        {
            WithRoot(root =>
            {
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                using (ProfileRootOperationLockV1.Acquire(root))
                {
                    var result = ProfileResetDiskTransactionV1.Begin(root, identity);
                    Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.Busy));
                    Assert.That(result.ActiveSnapshots[0].Observation, Is.EqualTo(ProfileResetDiskObservationV1.NotObserved));
                }
            });
        }

        [Test]
        public void AC002_MalformedMarkerDoesNotAuthenticateArchiveSnapshots()
        {
            WithRoot(root =>
            {
                File.WriteAllText(Path.Combine(root, "profile.reset.json"), "{}");
                var result = ProfileResetDiskTransactionV1.Resume(root);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ManualRepairRequired));
                Assert.That(result.TransactionId, Is.Null);
                Assert.That(result.ArchiveSnapshots[1].Proof, Is.EqualTo(ProfileResetDiskArchiveProofV1.NotObserved));
            });
        }

        [Test]
        public void AC009_ReflectionTargetMutationFailsClosed()
        {
            WithRoot(root =>
            {
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                object boxed = result;
                typeof(ProfileResetDiskResultV1).GetField("_targetRevision", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, 1L);
                Assert.Throws<InvalidOperationException>(() => ((ProfileResetDiskResultV1)boxed).Validate());
            });
        }

        [Test]
        public void AC009_ReflectionUnknownArchiveClassificationFailsClosed()
        {
            WithRoot(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                object boxed = result;
                var snapshots = (ProfileResetDiskArchiveSnapshotV1[])typeof(ProfileResetDiskResultV1).GetField("_archiveSnapshots", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(boxed);
                object snapshot = snapshots[0];
                typeof(ProfileResetDiskArchiveSnapshotV1).GetField("_classification", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(snapshot, (ProfileResetLeafClassificationV1)99);
                snapshots[0] = (ProfileResetDiskArchiveSnapshotV1)snapshot;
                Assert.Throws<InvalidOperationException>(() => ((ProfileResetDiskResultV1)boxed).Validate());
            });
        }

        [Test]
        public void AC009_DefensiveSnapshotArrayCopyCannotMutateResult()
        {
            WithRoot(root =>
            {
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                var copy = result.ActiveSnapshots;
                copy[0] = copy[1];
                result.Validate();
                Assert.That(result.ActiveSnapshots[0].Observation, Is.EqualTo(ProfileResetDiskObservationV1.Readable));
            });
        }

        [Test]
        public void AC003_PostPublicationFaultRetainsOnlyFreshlyAuthenticatedTargetEvidence()
        {
            WithRoot(root =>
            {
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                var control = ProfileResetDiskTestControlFactoryV1.Create(checkpoint =>
                {
                    if (checkpoint == ProfileResetDiskCheckpointV1.AfterMarkerCommittedReopen) throw new IOException("injected post-publication fault");
                });
                var result = ProfileResetDiskTransactionV1.Begin(root, identity, control);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ManualRepairRequired));
                Assert.That(result.TransactionId, Is.Not.Null);
                Assert.That(result.TargetRevision, Is.EqualTo(0));
                Assert.That(result.TargetByteLength, Is.GreaterThan(0));
                Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.True);
                result.Validate();
            });
        }

        private static void WithRoot(Action<string> action)
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C1-result-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            try { action(root); }
            finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
        }
    }
}
