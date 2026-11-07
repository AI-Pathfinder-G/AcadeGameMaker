using System;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D7C-001..005; AC-M5D7C-001..007. Caller-owned pure recovery transformation matrix.
    public sealed class ProfileRecoveryPlannerV1Tests
    {
        [Test]
        public void AC001_DefaultBootstrapIsExactCurrentCompatibleAndIdempotent()
        {
            var first = ProfileRecoveryPlannerV1.PlanDefaultBootstrap(); var second = ProfileRecoveryPlannerV1.PlanDefaultBootstrap();
            Assert.That(first.SourceRevision, Is.EqualTo(-1)); Assert.That(first.ResultRevision, Is.Zero); Assert.That(first.Reasons, Is.EqualTo(ProfileRecoveryPlanReason.DefaultBootstrap));
            var snapshot = first.ResultDocument.Snapshot; Assert.That(snapshot.Settings.WindowMode, Is.EqualTo(ProfileWindowMode.BorderlessFullscreen)); Assert.That(snapshot.Settings.MasterVolumeQ1000, Is.EqualTo(1000)); Assert.That(snapshot.Settings.MusicVolumeQ1000, Is.EqualTo(700)); Assert.That(snapshot.Settings.SfxVolumeQ1000, Is.EqualTo(800)); Assert.That(snapshot.Input.IsCurrentCompatible, Is.True); Assert.That(snapshot.Input.BindingOverrides.IsEmpty, Is.True);
            CollectionAssert.AreEqual(first.ResultDocument.CanonicalFileBytes, second.ResultDocument.CanonicalFileBytes);
            var payload = Encoding.UTF8.GetString(first.ResultDocument.CanonicalPayloadBytes);
            Assert.That(payload, Is.EqualTo("{\"schemaVersion\":1,\"profileRevision\":0,\"settings\":{\"windowMode\":\"BorderlessFullscreen\",\"masterVolumeQ1000\":1000,\"musicVolumeQ1000\":700,\"sfxVolumeQ1000\":800,\"gamepadAimInvertX\":false,\"gamepadAimInvertY\":false},\"input\":{\"inputActionsAssetId\":\"7c8d9e0f1a2b4c3d8e9f0a1b2c3d4e5f\",\"bindingSchemaVersion\":1,\"bindingOverridesJson\":\"\"},\"tutorial\":{\"confirmedIds\":[]},\"progression\":{\"lastOfferedSeed\":null,\"committedChoice\":null,\"consentState\":\"RefusesOwnershipTransfer\",\"grantedSkill\":null,\"completedBranches\":[]}}"));
            Assert.That(first.ResultDocument.PayloadSha256, Is.EqualTo(Hash(Encoding.UTF8.GetBytes(payload))));
        }

        [TestCase("other", 1)] [TestCase("7c8d9e0f1a2b4c3d8e9f0a1b2c3d4e5f", 0)] [TestCase("other", 2)]
        public void AC002_AC006_InputRepairPreservesNonInputAndReplacesEntireInput(string id, int schema)
        {
            var source = Decode(Snapshot(id, schema, ProfileBindingOverridesJson.Parse(string.Empty), 7)); var plan = ProfileRecoveryPlannerV1.PlanInputRepair(source); var output = plan.ResultDocument.Snapshot;
            Assert.That(plan.SourceRevision, Is.EqualTo(7)); Assert.That(plan.ResultRevision, Is.EqualTo(8)); Assert.That(plan.Reasons, Is.EqualTo(ProfileRecoveryPlanReason.InputRepair)); Assert.That(output.Input.IsCurrentCompatible, Is.True); Assert.That(output.Input.BindingOverrides.IsEmpty, Is.True); Assert.That(output.Settings.MasterVolumeQ1000, Is.EqualTo(500)); Assert.That(output.Progression.ProfileRevision, Is.EqualTo(8));
        }

        [Test]
        public void AC003_PreviousPromotionPreservesCurrentOverrideAndRepairsRecoverySources()
        {
            var current = Snapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse("{\"b\":2,\"a\":1}"), 3); var currentPlan = ProfileRecoveryPlannerV1.PlanPreviousPromotion(Decode(current));
            Assert.That(currentPlan.Reasons, Is.EqualTo(ProfileRecoveryPlanReason.PreviousPromotion)); Assert.That(currentPlan.ResultDocument.Snapshot.Input.BindingOverrides.CanonicalText, Is.EqualTo("{\"a\":1,\"b\":2}"));
            var recovered = ProfileRecoveryPlannerV1.PlanPreviousPromotion(Decode(Snapshot("other", 2, ProfileBindingOverridesJson.Parse(string.Empty), 3)));
            Assert.That(recovered.Reasons, Is.EqualTo(ProfileRecoveryPlanReason.PreviousPromotion | ProfileRecoveryPlanReason.InputRepair)); Assert.That(recovered.ResultDocument.Snapshot.Input.IsCurrentCompatible, Is.True);
        }

        [Test]
        public void AC004_AC005_OverflowAndClassificationMisuseHaveExactEntryFailures()
        {
            var overflow = Decode(Snapshot("other", 2, ProfileBindingOverridesJson.Parse(string.Empty), long.MaxValue)); Assert.Throws<InvalidOperationException>(() => ProfileRecoveryPlannerV1.PlanInputRepair(overflow)); Assert.Throws<InvalidOperationException>(() => ProfileRecoveryPlannerV1.PlanPreviousPromotion(overflow));
            var current = Decode(Snapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty), 1)); Assert.Throws<ArgumentException>(() => ProfileRecoveryPlannerV1.PlanInputRepair(current));
            var invalid = ProfileCanonicalDecoderV1.Decode(Encoding.UTF8.GetBytes("{}")); var unsupported = ProfileCanonicalDecoderV1.Decode(Encoding.UTF8.GetBytes("{\"schemaVersion\":0}")); Assert.Throws<ArgumentException>(() => ProfileRecoveryPlannerV1.PlanInputRepair(invalid)); Assert.Throws<ArgumentException>(() => ProfileRecoveryPlannerV1.PlanPreviousPromotion(unsupported));
            var defaultSource = default(ProfileCanonicalDecodeResultV1); var exception = Assert.Throws<ArgumentException>(() => ProfileRecoveryPlannerV1.PlanPreviousPromotion(defaultSource)); Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void AC002_AC003_BindingRecoveryFormsAndFullProgressionCollectionsArePreserved()
        {
            var ids = new[] { "a", "b" }; var branches = new[] { ProfileBranch.Extraction, ProfileBranch.Solidarity };
            var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), new ProfileInputSnapshot("other", 2, ProfileBindingOverridesJson.Parse(string.Empty)), new ProfileTutorialSnapshot(ids), new ProfileProgressionSnapshot(11, 101, ProfileChoice.Extraction, ProfileConsentState.RefusesOwnershipTransfer, ProfileSkill.CompressionVerdict, branches));
            ids[0] = "z"; branches[0] = ProfileBranch.Solidarity;
            foreach (var raw in new[] { "{bad}", "{\\\"b\\\":2,\\\"a\\\":1}" })
            {
                var decoded = ProfileCanonicalDecoderV1.Decode(RebuildRawBinding(ProfileCanonicalEncoder.Encode(snapshot), raw)); Assert.That(decoded.Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired));
                var repair = ProfileRecoveryPlannerV1.PlanInputRepair(decoded); var promotion = ProfileRecoveryPlannerV1.PlanPreviousPromotion(decoded);
                Assert.That(repair.Reasons, Is.EqualTo(ProfileRecoveryPlanReason.InputRepair)); Assert.That(promotion.Reasons, Is.EqualTo(ProfileRecoveryPlanReason.PreviousPromotion | ProfileRecoveryPlanReason.InputRepair));
                foreach (var plan in new[] { repair, promotion }) { var output = plan.ResultDocument.Snapshot; Assert.That(plan.ResultRevision, Is.EqualTo(12)); Assert.That(output.Input.IsCurrentCompatible, Is.True); Assert.That(output.Input.BindingOverrides.IsEmpty, Is.True); Assert.That(output.Progression.LastOfferedSeed, Is.EqualTo(101)); Assert.That(output.Progression.CommittedChoice, Is.EqualTo(ProfileChoice.Extraction)); CollectionAssert.AreEqual(new[] { "a", "b" }, output.Tutorial.ConfirmedIds); CollectionAssert.AreEqual(new[] { ProfileBranch.Extraction, ProfileBranch.Solidarity }, output.Progression.CompletedBranches); }
            }
        }

        [Test]
        public void AC004_RevisionBoundariesApplyToEveryApplicableClassification()
        {
            foreach (var revision in new[] { 0L, 37L, long.MaxValue - 1 }) { var recovery = Decode(Snapshot("other", 2, ProfileBindingOverridesJson.Parse(string.Empty), revision)); Assert.That(ProfileRecoveryPlannerV1.PlanInputRepair(recovery).ResultRevision, Is.EqualTo(revision + 1)); Assert.That(ProfileRecoveryPlannerV1.PlanPreviousPromotion(recovery).ResultRevision, Is.EqualTo(revision + 1)); var current = Decode(Snapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty), revision)); Assert.That(ProfileRecoveryPlannerV1.PlanPreviousPromotion(current).ResultRevision, Is.EqualTo(revision + 1)); }
        }

        [Test]
        public void AC007_ReasonClosureAndReflectionBypassPlansFailClosed()
        {
            var valid = ProfileRecoveryPlannerV1.PlanDefaultBootstrap();
            foreach (var reason in new[] { (ProfileRecoveryPlanReason)0, (ProfileRecoveryPlanReason)3, (ProfileRecoveryPlanReason)5, (ProfileRecoveryPlanReason)7, (ProfileRecoveryPlanReason)8 }) Assert.Throws<InvalidOperationException>(() => Corrupt(valid, "_reasons", reason).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(valid, "_resultRevision", 1L).Validate()); Assert.Throws<InvalidOperationException>(() => Corrupt(valid, "_resultDocument", default(ProfileCanonicalDocumentV1)).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(valid, "_proofSettings", new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 1, 1, 1, false, false)).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(valid, "_proofTutorial", new ProfileTutorialSnapshot(Array.Empty<string>())).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(valid, "_proofProgression", new ProfileProgressionSnapshot(0, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>())).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(valid, "_proofInput", new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty))).Validate());
            var repair = ProfileRecoveryPlannerV1.PlanInputRepair(Decode(Snapshot("other", 2, ProfileBindingOverridesJson.Parse(string.Empty), 2)));
            Assert.Throws<InvalidOperationException>(() => Corrupt(repair, "_proofInput", new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty))).Validate());
            var promotedRepair = ProfileRecoveryPlannerV1.PlanPreviousPromotion(Decode(Snapshot("other", 2, ProfileBindingOverridesJson.Parse(string.Empty), 2)));
            Assert.Throws<InvalidOperationException>(() => Corrupt(promotedRepair, "_proofInput", new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty))).Validate());
        }

        [Test]
        public void AC008_StaticBoundaryStoresNoSourceBytesOrIoAuthority()
        {
            var root = FindRoot(); var source = System.IO.File.ReadAllText(System.IO.Path.Combine(root, "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileRecoveryPlannerV1.cs")); StringAssert.Contains("REQ-M5D7C-001..005", source);
            foreach (var forbidden in new[] { "System.IO", "persistentDataPath", "File.", "DateTime", "Random", "System.Net", "UnityEngine", "ApplyBinding", "callback", "byte[]" }) StringAssert.DoesNotContain(forbidden, source);
        }

        private static ProfileSnapshotV1 Snapshot(string id, int schema, ProfileBindingOverridesJson binding, long revision) => new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), new ProfileInputSnapshot(id, schema, binding), new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
        private static ProfileCanonicalDecodeResultV1 Decode(ProfileSnapshotV1 snapshot) => ProfileCanonicalDecoderV1.Decode(ProfileCanonicalEncoder.Encode(snapshot).CanonicalFileBytes);
        private static ProfileRecoveryPlanV1 Corrupt(ProfileRecoveryPlanV1 source, string field, object value) { object boxed = source; typeof(ProfileRecoveryPlanV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value); return (ProfileRecoveryPlanV1)boxed; }
        private static byte[] RebuildRawBinding(ProfileCanonicalDocumentV1 source, string raw) { var payload = Encoding.UTF8.GetString(source.CanonicalPayloadBytes).Replace("\"bindingOverridesJson\":\"\"", "\"bindingOverridesJson\":\"" + raw + "\""); var hash = Hash(Encoding.UTF8.GetBytes(payload)); return Encoding.UTF8.GetBytes(payload.Substring(0, payload.Length - 1) + ",\"integrity\":{\"payloadSha256\":\"" + hash + "\"}}"); }
        private static string Hash(byte[] bytes) { using (var sha = SHA256.Create()) { var hash = sha.ComputeHash(bytes); var b = new StringBuilder(64); foreach (var x in hash) b.Append(x.ToString("x2", CultureInfo.InvariantCulture)); return b.ToString(); } }
        private static string FindRoot() { var current = new System.IO.DirectoryInfo(TestContext.CurrentContext.TestDirectory); while (current != null) { if (System.IO.File.Exists(System.IO.Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; } throw new System.IO.DirectoryNotFoundException(); }
    }
}
