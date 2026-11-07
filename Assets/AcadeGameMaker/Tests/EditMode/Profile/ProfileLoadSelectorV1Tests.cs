using System;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D7E-001..005; AC-M5D7E-001..010. Pure observed-candidate selection matrix.
    public sealed class ProfileLoadSelectorV1Tests
    {
        [Test]
        public void AC001_CurrentPrimaryAlwaysWinsOverEveryPreviousAndHigherTemp()
        {
            foreach (var previous in new[] { Missing(ProfileLoadFileRole.Previous), Unreadable(ProfileLoadFileRole.Previous), Decoded(ProfileLoadFileRole.Previous, Invalid()), Decoded(ProfileLoadFileRole.Previous, Unsupported()), Decoded(ProfileLoadFileRole.Previous, Metadata(99)) })
            foreach (var temp in new[] { Missing(ProfileLoadFileRole.Temp), Unreadable(ProfileLoadFileRole.Temp), Decoded(ProfileLoadFileRole.Temp, Current(999)) })
            {
                var plan = ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, Current(7)), previous, temp);
                Assert.That(plan.Source, Is.EqualTo(ProfileLaunchSource.Primary)); Assert.That(plan.SourceRevision, Is.EqualTo(7)); Assert.That(plan.ResultRevision, Is.EqualTo(7));
                Assert.That(plan.RequiresAtomicSave, Is.False); Assert.That(plan.RecoveryReasons, Is.EqualTo((ProfileRecoveryPlanReason)0));
                Assert.That(plan.InMemoryDocument.Snapshot.ProfileRevision, Is.EqualTo(7));
            }
        }

        [Test]
        public void AC002_PrimaryMetadataAndBindingRecoveryUseExactInputRepair()
        {
            foreach (var source in new[] { Metadata(5), Binding(6) })
            {
                var plan = ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, source), Decoded(ProfileLoadFileRole.Previous, Current(99)), Decoded(ProfileLoadFileRole.Temp, Current(100)));
                Assert.That(plan.Source, Is.EqualTo(ProfileLaunchSource.Primary)); Assert.That(plan.SourceRevision, Is.EqualTo(source.RecoveryProjection.SourceProfileRevision)); Assert.That(plan.ResultRevision, Is.EqualTo(plan.SourceRevision + 1));
                Assert.That(plan.RequiresAtomicSave, Is.True); Assert.That(plan.RecoveryReasons, Is.EqualTo(ProfileRecoveryPlanReason.InputRepair)); Assert.That(plan.InMemoryDocument.Snapshot.Input.IsCurrentCompatible, Is.True);
            }
        }

        [Test]
        public void AC003_UnusablePrimaryPromotesCurrentPrevious()
        {
            foreach (var primary in new[] { Missing(ProfileLoadFileRole.Primary), Unreadable(ProfileLoadFileRole.Primary), Decoded(ProfileLoadFileRole.Primary, Invalid()), Decoded(ProfileLoadFileRole.Primary, Unsupported()) })
            {
                var plan = ProfileLoadSelectorV1.Select(primary, Decoded(ProfileLoadFileRole.Previous, Current(8)), Missing(ProfileLoadFileRole.Temp));
                Assert.That(plan.Source, Is.EqualTo(ProfileLaunchSource.Previous)); Assert.That(plan.SourceRevision, Is.EqualTo(8)); Assert.That(plan.ResultRevision, Is.EqualTo(9));
                Assert.That(plan.RecoveryReasons, Is.EqualTo(ProfileRecoveryPlanReason.PreviousPromotion)); Assert.That(plan.RequiresAtomicSave, Is.True);
            }
        }

        [Test]
        public void AC004_UnusablePrimaryPromotesInputRecoverablePrevious()
        {
            foreach (var previous in new[] { Metadata(8), Binding(9) })
            {
                var plan = ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, Invalid()), Decoded(ProfileLoadFileRole.Previous, previous), Missing(ProfileLoadFileRole.Temp));
                Assert.That(plan.Source, Is.EqualTo(ProfileLaunchSource.Previous)); Assert.That(plan.ResultRevision, Is.EqualTo(plan.SourceRevision + 1));
                Assert.That(plan.RecoveryReasons, Is.EqualTo(ProfileRecoveryPlanReason.PreviousPromotion | ProfileRecoveryPlanReason.InputRepair)); Assert.That(plan.InMemoryDocument.Snapshot.Input.IsCurrentCompatible, Is.True);
            }
        }

        [Test]
        public void AC005_EveryUnselectablePrimaryPreviousCrossProductDefaultsAndIgnoresTemp()
        {
            var primaries = new[] { Missing(ProfileLoadFileRole.Primary), Unreadable(ProfileLoadFileRole.Primary), Decoded(ProfileLoadFileRole.Primary, Invalid()), Decoded(ProfileLoadFileRole.Primary, Unsupported()) };
            var previouses = new[] { Missing(ProfileLoadFileRole.Previous), Unreadable(ProfileLoadFileRole.Previous), Decoded(ProfileLoadFileRole.Previous, Invalid()), Decoded(ProfileLoadFileRole.Previous, Unsupported()) };
            foreach (var primary in primaries) foreach (var previous in previouses)
            {
                var plan = ProfileLoadSelectorV1.Select(primary, previous, Decoded(ProfileLoadFileRole.Temp, Current(999)));
                Assert.That(plan.Source, Is.EqualTo(ProfileLaunchSource.Default)); Assert.That(plan.SourceRevision, Is.EqualTo(-1)); Assert.That(plan.ResultRevision, Is.EqualTo(0));
                Assert.That(plan.RequiresAtomicSave, Is.True); Assert.That(plan.RecoveryReasons, Is.EqualTo(ProfileRecoveryPlanReason.DefaultBootstrap)); Assert.That(plan.TempPreservation, Is.EqualTo(ProfileFilePreservationReason.StaleTemp));
            }
        }

        [Test]
        public void AC006_TempExistenceAloneControlsStaleTempIntent()
        {
            var temps = new[] { Unreadable(ProfileLoadFileRole.Temp), Decoded(ProfileLoadFileRole.Temp, Current(91)), Decoded(ProfileLoadFileRole.Temp, Metadata(92)), Decoded(ProfileLoadFileRole.Temp, Binding(93)), Decoded(ProfileLoadFileRole.Temp, Unsupported()), Decoded(ProfileLoadFileRole.Temp, Invalid()) };
            foreach (var temp in temps)
            {
                var plan = ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, Current(1)), Missing(ProfileLoadFileRole.Previous), temp);
                Assert.That(plan.TempPreservation, Is.EqualTo(ProfileFilePreservationReason.StaleTemp)); Assert.That(plan.SourceRevision, Is.EqualTo(1));
            }
            Assert.That(ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, Current(1)), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp)).TempPreservation, Is.EqualTo(ProfileFilePreservationReason.None));
        }

        [Test]
        public void AC007_PreservationReasonsAreRoleExactAndIndependentOfWinner()
        {
            var abnormalPrevious = new[] { Decoded(ProfileLoadFileRole.Previous, Invalid()), Decoded(ProfileLoadFileRole.Previous, Unsupported()), Unreadable(ProfileLoadFileRole.Previous) };
            var expectedPrevious = new[] { ProfileFilePreservationReason.InvalidPrevious, ProfileFilePreservationReason.UnsupportedPrevious, ProfileFilePreservationReason.UnreadablePrevious };
            for (var index = 0; index < abnormalPrevious.Length; index++)
            {
                var plan = ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, Current(1)), abnormalPrevious[index], Missing(ProfileLoadFileRole.Temp));
                Assert.That(plan.PreviousPreservation, Is.EqualTo(expectedPrevious[index]));
            }
            var primary = new[] { Decoded(ProfileLoadFileRole.Primary, Invalid()), Decoded(ProfileLoadFileRole.Primary, Unsupported()), Unreadable(ProfileLoadFileRole.Primary) };
            var expectedPrimary = new[] { ProfileFilePreservationReason.InvalidPrimary, ProfileFilePreservationReason.UnsupportedPrimary, ProfileFilePreservationReason.UnreadablePrimary };
            for (var index = 0; index < primary.Length; index++) Assert.That(ProfileLoadSelectorV1.Select(primary[index], Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp)).PrimaryPreservation, Is.EqualTo(expectedPrimary[index]));
        }

        [Test]
        public void AC008_CandidatePlanGetterRoleAndReflectionMisuseFailClosed()
        {
            var missing = Missing(ProfileLoadFileRole.Primary); var unreadable = Unreadable(ProfileLoadFileRole.Primary);
            Assert.Throws<InvalidOperationException>(() => { var ignored = missing.DecodeResult; }); Assert.Throws<InvalidOperationException>(() => { var ignored = unreadable.DecodeResult; });
            foreach (var result in new[] { Current(1), Metadata(1), Binding(1), Unsupported(), Invalid() }) Assert.That(Decoded(ProfileLoadFileRole.Primary, result).DecodeResult.Classification, Is.EqualTo(result.Classification));
            Assert.Throws<ArgumentException>(() => ProfileLoadSelectorV1.Select(Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp)));
            Assert.Throws<ArgumentException>(() => ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Primary, default(ProfileCanonicalDecodeResultV1)));
            Assert.Throws<InvalidOperationException>(() => default(ProfileLoadCandidateV1).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(missing, "_kind", (ProfileLoadCandidateKind)99).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(missing, "_decodeResult", Current(1)).Validate());
            var plan = ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, Current(1)), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp));
            Assert.Throws<InvalidOperationException>(() => Corrupt(plan, "_source", (ProfileLaunchSource)99).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(plan, "_resultRevision", 2L).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(plan, "_recoveryReasons", ProfileRecoveryPlanReason.InputRepair).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(plan, "_proofKind", 4).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(plan, "_primaryProof", Missing(ProfileLoadFileRole.Primary)).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(plan, "_tempProof", Missing(ProfileLoadFileRole.Previous)).Validate());
            var bytes = plan.InMemoryDocument.CanonicalFileBytes; bytes[0] = (byte)'!'; Assert.That(plan.InMemoryDocument.CanonicalFileBytes[0], Is.Not.EqualTo((byte)'!'));
            var ids = new[] { "a" };
            var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty)), new ProfileTutorialSnapshot(ids), new ProfileProgressionSnapshot(2, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
            ids[0] = "z";
            var arrayPlan = ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, ProfileCanonicalDecoderV1.Decode(ProfileCanonicalEncoder.Encode(snapshot).CanonicalFileBytes)), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp));
            Assert.That(arrayPlan.InMemoryDocument.Snapshot.Tutorial.ConfirmedIds[0], Is.EqualTo("a"));
        }

        [Test]
        public void AC009_RevisionBoundariesAndDeterminismAreExact()
        {
            var direct = ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, Current(long.MaxValue)), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp));
            Assert.That(direct.ResultRevision, Is.EqualTo(long.MaxValue)); Assert.That(direct.RequiresAtomicSave, Is.False);
            foreach (var revision in new[] { 0L, 37L, long.MaxValue - 1 })
            {
                var plan = ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, Metadata(revision)), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp));
                Assert.That(plan.ResultRevision, Is.EqualTo(revision + 1));
            }
            Assert.Throws<InvalidOperationException>(() => ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, Metadata(long.MaxValue)), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp)));
            Assert.Throws<InvalidOperationException>(() => ProfileLoadSelectorV1.Select(Missing(ProfileLoadFileRole.Primary), Decoded(ProfileLoadFileRole.Previous, Current(long.MaxValue)), Missing(ProfileLoadFileRole.Temp)));
            var one = ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, Current(4)), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp));
            var two = ProfileLoadSelectorV1.Select(Decoded(ProfileLoadFileRole.Primary, Current(4)), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp));
            CollectionAssert.AreEqual(one.InMemoryDocument.CanonicalFileBytes, two.InMemoryDocument.CanonicalFileBytes);
        }

        [Test]
        public void AC010_RuntimeIsEngineFreeAndHasNoLoadExecutionAuthority()
        {
            var source = System.IO.File.ReadAllText(System.IO.Path.Combine(FindRoot(), "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileLoadSelectorV1.cs"));
            StringAssert.Contains("REQ-M5D7E-001..005", source);
            foreach (var forbidden in new[] { "System.IO", "File.", "Path.", "persistentDataPath", "UnityEngine", "DateTime", "Random", "System.Net", "Save(", "quarantine", "ApplyBinding", "callback" }) StringAssert.DoesNotContain(forbidden, source);
        }

        private static ProfileLoadCandidateV1 Missing(ProfileLoadFileRole role) => ProfileLoadCandidateV1.Missing(role);
        private static ProfileLoadCandidateV1 Unreadable(ProfileLoadFileRole role) => ProfileLoadCandidateV1.Unreadable(role);
        private static ProfileLoadCandidateV1 Decoded(ProfileLoadFileRole role, ProfileCanonicalDecodeResultV1 value) => ProfileLoadCandidateV1.Decoded(role, value);
        private static ProfileCanonicalDecodeResultV1 Current(long revision) => ProfileCanonicalDecoderV1.Decode(Document(revision, ProfileInputContract.CurrentInputActionsAssetId).CanonicalFileBytes);
        private static ProfileCanonicalDecodeResultV1 Metadata(long revision) => ProfileCanonicalDecoderV1.Decode(Document(revision, "obsolete-input").CanonicalFileBytes);
        private static ProfileCanonicalDecodeResultV1 Binding(long revision) => ProfileCanonicalDecoderV1.Decode(RebuildRawBinding(Document(revision, ProfileInputContract.CurrentInputActionsAssetId), "{bad}"));
        private static ProfileCanonicalDecodeResultV1 Unsupported() => ProfileCanonicalDecoderV1.Decode(Utf8("{\"schemaVersion\":0}"));
        private static ProfileCanonicalDecodeResultV1 Invalid() => ProfileCanonicalDecoderV1.Decode(Utf8("{}"));
        private static ProfileCanonicalDocumentV1 Document(long revision, string assetId)
        {
            var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), new ProfileInputSnapshot(assetId, 1, ProfileBindingOverridesJson.Parse(string.Empty)), new ProfileTutorialSnapshot(new[] { "tutorial.a" }), new ProfileProgressionSnapshot(revision, 101, ProfileChoice.Extraction, ProfileConsentState.RefusesOwnershipTransfer, ProfileSkill.CompressionVerdict, new[] { ProfileBranch.Extraction }));
            return ProfileCanonicalEncoder.Encode(snapshot);
        }
        private static byte[] RebuildRawBinding(ProfileCanonicalDocumentV1 source, string rawEscapedBinding)
        {
            var payload = Encoding.UTF8.GetString(source.CanonicalPayloadBytes).Replace("\"bindingOverridesJson\":\"\"", "\"bindingOverridesJson\":\"" + rawEscapedBinding + "\"");
            var bytes = Utf8(payload); var hash = Hash(bytes); return Utf8(payload.Substring(0, payload.Length - 1) + ",\"integrity\":{\"payloadSha256\":\"" + hash + "\"}}");
        }
        private static byte[] Utf8(string value) => new UTF8Encoding(false).GetBytes(value);
        private static string Hash(byte[] value) { using (var sha = SHA256.Create()) { var bytes = sha.ComputeHash(value); var builder = new StringBuilder(bytes.Length * 2); foreach (var item in bytes) builder.Append(item.ToString("x2", CultureInfo.InvariantCulture)); return builder.ToString(); } }
        private static ProfileLoadCandidateV1 Corrupt(ProfileLoadCandidateV1 source, string field, object value) { object boxed = source; typeof(ProfileLoadCandidateV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value); return (ProfileLoadCandidateV1)boxed; }
        private static ProfileLoadSelectionPlanV1 Corrupt(ProfileLoadSelectionPlanV1 source, string field, object value) { object boxed = source; typeof(ProfileLoadSelectionPlanV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value); return (ProfileLoadSelectionPlanV1)boxed; }
        private static string FindRoot() { var current = new System.IO.DirectoryInfo(TestContext.CurrentContext.TestDirectory); while (current != null) { if (System.IO.File.Exists(System.IO.Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; } throw new System.IO.DirectoryNotFoundException(); }
    }
}
