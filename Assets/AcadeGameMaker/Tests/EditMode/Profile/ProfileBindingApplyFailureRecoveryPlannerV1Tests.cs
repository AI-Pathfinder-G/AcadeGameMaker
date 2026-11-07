using System;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D7J-001..007; AC-M5D7J-001..008. Pure current/non-empty binding-apply-failure transform only.
    public sealed class ProfileBindingApplyFailureRecoveryPlannerV1Tests
    {
        [Test]
        public void AC001_AC002_PrimaryAndPreviousResetOnlyInputWithExactSingleIncrement()
        {
            var ids = new[] { "tutorial.a", "tutorial.b" }; var branches = new[] { ProfileBranch.Extraction, ProfileBranch.Solidarity };
            var source = Decode(Snapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse("{\"b\":2,\"a\":1}"), 17, ids, branches));
            ids[0] = "mutated"; branches[0] = ProfileBranch.Solidarity;
            var primary = ProfileRecoveryPlannerV1.PlanPrimaryBindingApplyFailure(source);
            var previous = ProfileRecoveryPlannerV1.PlanPreviousBindingApplyFailure(source);
            foreach (var plan in new[] { primary, previous })
            {
                plan.Validate(); var output = plan.ResultDocument.Snapshot;
                Assert.That(plan.SourceRevision, Is.EqualTo(17)); Assert.That(plan.ResultRevision, Is.EqualTo(18));
                Assert.That(output.Input.IsCurrentCompatible, Is.True); Assert.That(output.Input.BindingOverrides.IsEmpty, Is.True);
                Assert.That(output.Settings.MasterVolumeQ1000, Is.EqualTo(501)); Assert.That(output.Progression.LastOfferedSeed, Is.EqualTo(101)); Assert.That(output.Progression.CommittedChoice, Is.EqualTo(ProfileChoice.Extraction));
                CollectionAssert.AreEqual(new[] { "tutorial.a", "tutorial.b" }, output.Tutorial.ConfirmedIds); CollectionAssert.AreEqual(new[] { ProfileBranch.Extraction, ProfileBranch.Solidarity }, output.Progression.CompletedBranches);
            }
            Assert.That(primary.Reasons, Is.EqualTo(ProfileRecoveryPlanReason.InputRepair));
            Assert.That(previous.Reasons, Is.EqualTo(ProfileRecoveryPlanReason.PreviousPromotion | ProfileRecoveryPlanReason.InputRepair));
            Assert.That(ProfileRecoveryPlannerV1.PlanPreviousPromotion(source).ResultRevision, Is.EqualTo(18));
            Assert.That(ProfileRecoveryPlannerV1.PlanPreviousPromotion(source).ResultDocument.Snapshot.Input.BindingOverrides.IsEmpty, Is.False);
        }

        [Test]
        public void AC003_RevisionBoundariesAreExactAndOverflowDoesNotPoisonFollowingCall()
        {
            foreach (var revision in new[] { 0L, 37L, long.MaxValue - 1 })
            {
                var source = CurrentNonEmpty(revision);
                Assert.That(ProfileRecoveryPlannerV1.PlanPrimaryBindingApplyFailure(source).ResultRevision, Is.EqualTo(revision + 1));
                Assert.That(ProfileRecoveryPlannerV1.PlanPreviousBindingApplyFailure(source).ResultRevision, Is.EqualTo(revision + 1));
            }
            var overflow = CurrentNonEmpty(long.MaxValue);
            Assert.Throws<InvalidOperationException>(() => ProfileRecoveryPlannerV1.PlanPrimaryBindingApplyFailure(overflow));
            Assert.Throws<InvalidOperationException>(() => ProfileRecoveryPlannerV1.PlanPreviousBindingApplyFailure(overflow));
            Assert.That(ProfileRecoveryPlannerV1.PlanPrimaryBindingApplyFailure(CurrentNonEmpty(2)).ResultRevision, Is.EqualTo(3));
        }

        [Test]
        public void AC004_InapplicableAndMalformedSourcesHaveClosedBoundary()
        {
            var bindingRecovery = ProfileCanonicalDecoderV1.Decode(RebuildRawBinding(ProfileCanonicalEncoder.Encode(Snapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty), 1, Array.Empty<string>(), Array.Empty<ProfileBranch>())), "{bad}"));
            Assert.That(bindingRecovery.Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired));
            var sources = new[]
            {
                Decode(Snapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty), 1, Array.Empty<string>(), Array.Empty<ProfileBranch>())),
                Decode(Snapshot("obsolete", 1, ProfileBindingOverridesJson.Parse("{\"x\":1}"), 1, Array.Empty<string>(), Array.Empty<ProfileBranch>())),
                bindingRecovery,
                ProfileCanonicalDecoderV1.Decode(Encoding.UTF8.GetBytes("{}")),
                ProfileCanonicalDecoderV1.Decode(Encoding.UTF8.GetBytes("{\"schemaVersion\":0}"))
            };
            foreach (var source in sources) { Assert.Throws<ArgumentException>(() => ProfileRecoveryPlannerV1.PlanPrimaryBindingApplyFailure(source)); Assert.Throws<ArgumentException>(() => ProfileRecoveryPlannerV1.PlanPreviousBindingApplyFailure(source)); }
            var defaultPrimary = Assert.Throws<ArgumentException>(() => ProfileRecoveryPlannerV1.PlanPrimaryBindingApplyFailure(default(ProfileCanonicalDecodeResultV1))); Assert.That(defaultPrimary.InnerException, Is.TypeOf<InvalidOperationException>());
            var defaultPrevious = Assert.Throws<ArgumentException>(() => ProfileRecoveryPlannerV1.PlanPreviousBindingApplyFailure(default(ProfileCanonicalDecodeResultV1))); Assert.That(defaultPrevious.InnerException, Is.TypeOf<InvalidOperationException>());
            var reflected = Corrupt(CurrentNonEmpty(1), "_classification", (ProfileCanonicalDecodeClassification)99);
            var reflectedPrimary = Assert.Throws<ArgumentException>(() => ProfileRecoveryPlannerV1.PlanPrimaryBindingApplyFailure(reflected)); Assert.That(reflectedPrimary.InnerException, Is.TypeOf<InvalidOperationException>());
            var reflectedPrevious = Assert.Throws<ArgumentException>(() => ProfileRecoveryPlannerV1.PlanPreviousBindingApplyFailure(reflected)); Assert.That(reflectedPrevious.InnerException, Is.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void AC005_NewProofKindsAndAllGettersFailClosedUnderReflectionMutation()
        {
            var primary = ProfileRecoveryPlannerV1.PlanPrimaryBindingApplyFailure(CurrentNonEmpty(4));
            var previous = ProfileRecoveryPlannerV1.PlanPreviousBindingApplyFailure(CurrentNonEmpty(4));
            primary.Validate(); previous.Validate(); var a = primary.SourceRevision; var b = primary.ResultRevision; var c = primary.Reasons; var d = primary.ResultDocument;
            foreach (var mutation in new[]
            {
                new Mutation("_proofKind", 3), new Mutation("_proofKind", 99), new Mutation("_reasons", ProfileRecoveryPlanReason.PreviousPromotion), new Mutation("_sourceRevision", 5L), new Mutation("_resultRevision", 99L),
                new Mutation("_proofInput", default(ProfileInputSnapshot)), new Mutation("_proofInput", new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty))), new Mutation("_proofInput", new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse("{\"different\":1}"))), new Mutation("_proofBindingOverride", "{\"different\":1}"),
                new Mutation("_proofSettings", new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 1, 1, 1, false, false)), new Mutation("_proofTutorial", new ProfileTutorialSnapshot(new[] { "different" })), new Mutation("_proofProgression", new ProfileProgressionSnapshot(4, 202, ProfileChoice.Extraction, ProfileConsentState.RefusesOwnershipTransfer, ProfileSkill.CompressionVerdict, Array.Empty<ProfileBranch>())),
                new Mutation("_resultDocument", ResultDocumentWithNonEmptyInput(primary)), new Mutation("_resultDocument", ResultDocumentWithWrongSettings(primary))
            }) AssertPlanGettersFail(primary, mutation);
            AssertPlanGettersFail(previous, new Mutation("_proofKind", 5));
            AssertPlanGettersFail(previous, new Mutation("_reasons", ProfileRecoveryPlanReason.InputRepair));
        }

        [Test]
        public void AC006_AC007_ExistingReasonsRemainNumericAndPlannerHasNoApplyOrIoAuthority()
        {
            Assert.That((int)ProfileRecoveryPlanReason.DefaultBootstrap, Is.EqualTo(1)); Assert.That((int)ProfileRecoveryPlanReason.PreviousPromotion, Is.EqualTo(2)); Assert.That((int)ProfileRecoveryPlanReason.InputRepair, Is.EqualTo(4));
            var root = FindRoot(); var text = System.IO.File.ReadAllText(System.IO.Path.Combine(root, "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileRecoveryPlannerV1.cs"));
            StringAssert.Contains("REQ-M5D7J-001..007", text); StringAssert.Contains("PlanPrimaryBindingApplyFailure", text); StringAssert.Contains("PlanPreviousBindingApplyFailure", text);
            foreach (var forbidden in new[] { "UnityEngine", "M5D7I", "ApplyBinding", "System.IO", "File.", "persistentDataPath", "Save(", "DateTime", "Random", "callback" }) StringAssert.DoesNotContain(forbidden, text);
        }

        private readonly struct Mutation { internal readonly string Field; internal readonly object Value; internal Mutation(string field, object value) { Field = field; Value = value; } }
        private static ProfileCanonicalDecodeResultV1 CurrentNonEmpty(long revision) => Decode(Snapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse("{\"x\":1}"), revision, Array.Empty<string>(), Array.Empty<ProfileBranch>()));
        private static ProfileSnapshotV1 Snapshot(string id, int schema, ProfileBindingOverridesJson binding, long revision, string[] ids, ProfileBranch[] branches) => new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 501, 502, 503, true, false), new ProfileInputSnapshot(id, schema, binding), new ProfileTutorialSnapshot(ids), new ProfileProgressionSnapshot(revision, 101, ProfileChoice.Extraction, ProfileConsentState.RefusesOwnershipTransfer, ProfileSkill.CompressionVerdict, branches));
        private static ProfileCanonicalDecodeResultV1 Decode(ProfileSnapshotV1 snapshot) => ProfileCanonicalDecoderV1.Decode(ProfileCanonicalEncoder.Encode(snapshot).CanonicalFileBytes);
        private static ProfileCanonicalDocumentV1 ResultDocumentWithNonEmptyInput(ProfileRecoveryPlanV1 plan)
        {
            var snapshot = plan.ResultDocument.Snapshot;
            var input = new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, ProfileInputContract.CurrentBindingSchemaVersion, ProfileBindingOverridesJson.Parse("{\"still\":1}"));
            return ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(snapshot.Settings, input, snapshot.Tutorial, snapshot.Progression));
        }
        private static ProfileCanonicalDocumentV1 ResultDocumentWithWrongSettings(ProfileRecoveryPlanV1 plan)
        {
            var snapshot = plan.ResultDocument.Snapshot;
            var settings = new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 777, snapshot.Settings.MusicVolumeQ1000, snapshot.Settings.SfxVolumeQ1000, snapshot.Settings.GamepadAimInvertX, snapshot.Settings.GamepadAimInvertY);
            return ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(settings, snapshot.Input, snapshot.Tutorial, snapshot.Progression));
        }
        private static ProfileCanonicalDecodeResultV1 Corrupt(ProfileCanonicalDecodeResultV1 source, string field, object value) { object boxed = source; typeof(ProfileCanonicalDecodeResultV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value); return (ProfileCanonicalDecodeResultV1)boxed; }
        private static byte[] RebuildRawBinding(ProfileCanonicalDocumentV1 source, string raw) { var payload = Encoding.UTF8.GetString(source.CanonicalPayloadBytes).Replace("\"bindingOverridesJson\":\"\"", "\"bindingOverridesJson\":\"" + raw + "\""); var hash = Hash(Encoding.UTF8.GetBytes(payload)); return Encoding.UTF8.GetBytes(payload.Substring(0, payload.Length - 1) + ",\"integrity\":{\"payloadSha256\":\"" + hash + "\"}}"); }
        private static string Hash(byte[] bytes) { using (var sha = SHA256.Create()) { var value = sha.ComputeHash(bytes); var text = new StringBuilder(64); foreach (var item in value) text.Append(item.ToString("x2", CultureInfo.InvariantCulture)); return text.ToString(); } }
        private static void AssertPlanGettersFail(ProfileRecoveryPlanV1 source, Mutation mutation) { object boxed = source; typeof(ProfileRecoveryPlanV1).GetField(mutation.Field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, mutation.Value); var plan = (ProfileRecoveryPlanV1)boxed; Assert.Throws<InvalidOperationException>(() => plan.Validate()); Assert.Throws<InvalidOperationException>(() => { var x = plan.SourceRevision; }); Assert.Throws<InvalidOperationException>(() => { var x = plan.ResultRevision; }); Assert.Throws<InvalidOperationException>(() => { var x = plan.Reasons; }); Assert.Throws<InvalidOperationException>(() => { var x = plan.ResultDocument; }); }
        private static string FindRoot() { var current = new System.IO.DirectoryInfo(TestContext.CurrentContext.TestDirectory); while (current != null) { if (System.IO.File.Exists(System.IO.Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; } throw new System.IO.DirectoryNotFoundException(); }
    }
}
