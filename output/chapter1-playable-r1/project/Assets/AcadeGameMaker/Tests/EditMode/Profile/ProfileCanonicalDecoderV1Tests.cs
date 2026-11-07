using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D7B-001..006; AC-M5D7B-001..008. Strict canonical-envelope and typed-recovery precedence matrix.
    public sealed class ProfileCanonicalDecoderV1Tests
    {
        [Test]
        public void AC001_CurrentDefaultExtractionAndSolidarityDocumentsRoundTripExactly()
        {
            foreach (var snapshot in new[] { Current(0, null, null, null), Current(3, 101, ProfileChoice.Extraction, ProfileSkill.CompressionVerdict), Current(4, 202, ProfileChoice.Solidarity, ProfileSkill.CommonReferencePlane) })
            {
                var source = ProfileCanonicalEncoder.Encode(snapshot); var result = ProfileCanonicalDecoderV1.Decode(source.CanonicalFileBytes);
                Assert.That(result.Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.ValidCurrentInput));
                Assert.That(result.InputCompatibility, Is.EqualTo(ProfileInputCompatibility.Current));
                CollectionAssert.AreEqual(source.CanonicalPayloadBytes, result.Document.CanonicalPayloadBytes);
                CollectionAssert.AreEqual(source.CanonicalFileBytes, result.Document.CanonicalFileBytes);
                Assert.That(result.Document.PayloadSha256, Is.EqualTo(source.PayloadSha256));
            }
        }

        [TestCase("different-id", 1, ProfileInputCompatibility.AssetIdMismatch)]
        [TestCase("7c8d9e0f1a2b4c3d8e9f0a1b2c3d4e5f", 0, ProfileInputCompatibility.BindingSchemaMismatch)]
        [TestCase("7c8d9e0f1a2b4c3d8e9f0a1b2c3d4e5f", 2, ProfileInputCompatibility.BindingSchemaMismatch)]
        [TestCase("7c8d9e0f1a2b4c3d8e9f0a1b2c3d4e5f", int.MaxValue, ProfileInputCompatibility.BindingSchemaMismatch)]
        [TestCase("different-id", 2, (ProfileInputCompatibility)3)]
        public void AC002_MetadataMismatchRetainsDocumentAndNonInputProjection(string id, int schema, ProfileInputCompatibility expected)
        {
            var snapshot = Snapshot(id, schema, ProfileBindingOverridesJson.Parse(string.Empty), 33);
            var source = ProfileCanonicalEncoder.Encode(snapshot); var result = ProfileCanonicalDecoderV1.Decode(source.CanonicalFileBytes);
            Assert.That(result.Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired));
            Assert.That(result.InputCompatibility, Is.EqualTo(expected));
            Assert.That(result.InputRecoveryReasons, Is.EqualTo(Reasons(expected)));
            Assert.That(result.RecoveryProjection.SourceProfileRevision, Is.EqualTo(33));
            Assert.That(result.RecoveryProjection.Settings.WindowMode, Is.EqualTo(snapshot.Settings.WindowMode));
            CollectionAssert.AreEqual(source.CanonicalFileBytes, result.Document.CanonicalFileBytes);
        }

        [TestCase(0)] [TestCase(2)] [TestCase(int.MaxValue)] [TestCase(-1)]
        public void AC003_ParseableUniqueUnsupportedSchemasReturnExactVersion(int schema)
        {
            var result = ProfileCanonicalDecoderV1.Decode(Utf8("{\"schemaVersion\":" + schema.ToString(CultureInfo.InvariantCulture) + "}"));
            Assert.That(result.Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.UnsupportedProfileSchema));
            Assert.That(result.UnsupportedSchemaVersion, Is.EqualTo(schema));
            Assert.Throws<InvalidOperationException>(() => { var ignored = result.Document; });
            AssertInvalid(Utf8("{\"schemaVersion\":" + schema.ToString(CultureInfo.InvariantCulture) + ",\"malformed\":[}"), ProfileCanonicalDecodeFailure.MalformedJson);
        }

        [Test]
        public void AC003_AC004_LexicalSchemaAndDepthFailuresAreTypedWithoutDataExceptions()
        {
            Assert.Throws<ArgumentNullException>(() => ProfileCanonicalDecoderV1.Decode(null));
            foreach (var bytes in new[] { Array.Empty<byte>(), new byte[] { 0xef, 0xbb, 0xbf, (byte)'{' }, new byte[] { 0xc3, 0x28 }, Utf8(" {\"schemaVersion\":1}"), Utf8("[]"), Utf8("{\"schemaVersion\":1,}") })
                AssertInvalid(bytes, ProfileCanonicalDecodeFailure.MalformedJson, ProfileCanonicalDecodeFailure.SchemaViolation);
            AssertInvalid(Utf8("{\"schemaVersion\":0,\"\\u0073chemaVersion\":0}"), ProfileCanonicalDecodeFailure.SchemaViolation);
            AssertInvalid(Utf8("{}"), ProfileCanonicalDecodeFailure.SchemaViolation);
            AssertInvalid(Utf8("{\"schemaVersion\":1.0}"), ProfileCanonicalDecodeFailure.SchemaViolation);
            AssertInvalid(Utf8("{\"schemaVersion\":2147483648}"), ProfileCanonicalDecodeFailure.SchemaViolation);
            AssertInvalid(Utf8(NestedObject(64)), ProfileCanonicalDecodeFailure.SchemaViolation);
            AssertInvalid(Utf8(NestedObject(65)), ProfileCanonicalDecodeFailure.MalformedJson);
        }

        [Test]
        public void AC005_AC006_OuterCanonicalPrecedesInnerBindingRecoveryAndIntegrityShape()
        {
            var source = ProfileCanonicalEncoder.Encode(Current(7, null, null, null));
            var noncanonical = RebuildRawBinding(source, "{\\\"b\\\":2,\\\"a\\\":1}");
            var recovered = ProfileCanonicalDecoderV1.Decode(noncanonical);
            Assert.That(recovered.Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired));
            Assert.That(recovered.InputRecoveryReasons, Is.EqualTo(ProfileInputRecoveryReason.BindingOverridesMalformedOrNonCanonical));
            Assert.That(recovered.RecoveryProjection.SourceProfileRevision, Is.EqualTo(7));
            Assert.Throws<InvalidOperationException>(() => { var ignored = recovered.Document; });
            var malformed = RebuildRawBinding(source, "{bad}");
            Assert.That(ProfileCanonicalDecoderV1.Decode(malformed).Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired));
            AssertInvalid(RebuildRawBinding(source, "\\uD800"), ProfileCanonicalDecodeFailure.SchemaViolation);
            Assert.That(ProfileCanonicalDecoderV1.Decode(RebuildRawBinding(source, "\\\\uD800")).Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired));
            var upperHash = ReplaceAscii(source.CanonicalFileBytes, "\"payloadSha256\":\"", "\"payloadSha256\":\"A");
            AssertInvalid(upperHash, ProfileCanonicalDecodeFailure.CanonicalOrIntegrityMismatch);
            AssertInvalid(Utf8(Encoding.UTF8.GetString(noncanonical).Replace("{\\\"b\\\":2,\\\"a\\\":1}", "é")), ProfileCanonicalDecodeFailure.SchemaViolation);
        }

        [Test]
        public void AC006_AC007_DefensiveInputDefaultBypassAndGetterAvailabilityReject()
        {
            var source = ProfileCanonicalEncoder.Encode(Current(1, null, null, null)); var bytes = source.CanonicalFileBytes; var result = ProfileCanonicalDecoderV1.Decode(bytes); bytes[0] = (byte)'!';
            Assert.That(result.Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.ValidCurrentInput)); result.Validate();
            Assert.Throws<InvalidOperationException>(() => { var ignored = result.RecoveryProjection; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = result.Failure; });
            var defaultResult = default(ProfileCanonicalDecodeResultV1); Assert.Throws<InvalidOperationException>(() => defaultResult.Validate()); Assert.Throws<InvalidOperationException>(() => { var ignored = defaultResult.Classification; });
            var projection = default(ProfileInputRecoveryProjectionV1); Assert.Throws<InvalidOperationException>(() => projection.Validate());
            var badReasons = Corrupt(result, "_inputRecoveryReasons", (ProfileInputRecoveryReason)8); Assert.Throws<InvalidOperationException>(() => badReasons.Validate());
            var badClass = Corrupt(result, "_classification", (ProfileCanonicalDecodeClassification)99); Assert.Throws<InvalidOperationException>(() => badClass.Validate());
        }

        [Test]
        public void AC007_LunaResultNeutralFieldsProjectionMatchAndUnavailableGetterMatrix()
        {
            var current = ProfileCanonicalDecoderV1.Decode(ProfileCanonicalEncoder.Encode(Current(1, null, null, null)).CanonicalFileBytes);
            var metadata = ProfileCanonicalDecoderV1.Decode(ProfileCanonicalEncoder.Encode(Snapshot("other-id", 2, ProfileBindingOverridesJson.Parse(string.Empty), 4)).CanonicalFileBytes);
            var bindingSource = ProfileCanonicalEncoder.Encode(Snapshot("other-id", 2, ProfileBindingOverridesJson.Parse(string.Empty), 5));
            var binding = ProfileCanonicalDecoderV1.Decode(RebuildRawBinding(bindingSource, "{bad}"));
            var unsupported = ProfileCanonicalDecoderV1.Decode(Utf8("{\"schemaVersion\":0}"));
            var invalid = ProfileCanonicalDecoderV1.Decode(Utf8("{}"));

            Assert.Throws<InvalidOperationException>(() => Corrupt(current, "_unsupportedSchemaVersion", 1).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(metadata, "_unsupportedSchemaVersion", 1).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(binding, "_unsupportedSchemaVersion", 1).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(binding, "_inputCompatibility", ProfileInputCompatibility.AssetIdMismatch).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(unsupported, "_inputCompatibility", ProfileInputCompatibility.AssetIdMismatch).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(invalid, "_inputCompatibility", ProfileInputCompatibility.AssetIdMismatch).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(unsupported, "_unsupportedSchemaVersion", 1).Validate());

            var differentProjection = new ProfileInputRecoveryProjectionV1(metadata.Document.Snapshot.Settings, metadata.Document.Snapshot.Tutorial,
                new ProfileProgressionSnapshot(9, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
            Assert.Throws<InvalidOperationException>(() => Corrupt(metadata, "_recoveryProjection", differentProjection).Validate());

            AssertUnavailableCurrent(current); AssertUnavailableMetadata(metadata); AssertUnavailableBinding(binding); AssertUnavailableUnsupported(unsupported); AssertUnavailableInvalid(invalid);
        }

        [Test]
        public void AC007_BindingCombinedReasonsAndProjectionCollectionsAreDefensive()
        {
            var ids = new[] { "a" };
            var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false),
                new ProfileInputSnapshot("other-id", 2, ProfileBindingOverridesJson.Parse(string.Empty)), new ProfileTutorialSnapshot(ids),
                new ProfileProgressionSnapshot(6, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
            ids[0] = "z";
            var binding = ProfileCanonicalDecoderV1.Decode(RebuildRawBinding(ProfileCanonicalEncoder.Encode(snapshot), "{bad}"));
            Assert.That(binding.InputRecoveryReasons, Is.EqualTo(ProfileInputRecoveryReason.AssetIdMismatch | ProfileInputRecoveryReason.BindingSchemaMismatch | ProfileInputRecoveryReason.BindingOverridesMalformedOrNonCanonical));
            Assert.That(binding.RecoveryProjection.Tutorial.ConfirmedIds[0], Is.EqualTo("a"));
            Assert.Throws<NotSupportedException>(() => ((System.Collections.IList)binding.RecoveryProjection.Tutorial.ConfirmedIds)[0] = "z");
        }

        [Test]
        public void AC008_RuntimeIsEngineFreeAndHasNoPersistenceOrInputApplyAuthority()
        {
            var root = FindRoot(); var source = System.IO.File.ReadAllText(System.IO.Path.Combine(root, "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileCanonicalDecoderV1.cs"));
            StringAssert.Contains("REQ-M5D7B-001..006", source);
            foreach (var forbidden in new[] { "System.IO", "persistentDataPath", "File.", "DateTime", "Random", "System.Net", "UnityEngine", "ApplyBinding", "callback" }) StringAssert.DoesNotContain(forbidden, source);
        }

        private static ProfileSnapshotV1 Current(long revision, int? seed, ProfileChoice? choice, ProfileSkill? skill) => Snapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty), revision, seed, choice, skill);
        private static ProfileSnapshotV1 Snapshot(string id, int schema, ProfileBindingOverridesJson overrides, long revision, int? seed = null, ProfileChoice? choice = null, ProfileSkill? skill = null) => new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), new ProfileInputSnapshot(id, schema, overrides), new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, seed, choice, ProfileConsentState.RefusesOwnershipTransfer, skill, Array.Empty<ProfileBranch>()));
        private static ProfileInputRecoveryReason Reasons(ProfileInputCompatibility value) { var result = ProfileInputRecoveryReason.None; if ((value & ProfileInputCompatibility.AssetIdMismatch) != 0) result |= ProfileInputRecoveryReason.AssetIdMismatch; if ((value & ProfileInputCompatibility.BindingSchemaMismatch) != 0) result |= ProfileInputRecoveryReason.BindingSchemaMismatch; return result; }
        private static void AssertInvalid(byte[] bytes, params ProfileCanonicalDecodeFailure[] allowed) { var result = ProfileCanonicalDecoderV1.Decode(bytes); Assert.That(result.Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.Invalid)); CollectionAssert.Contains(allowed, result.Failure); }
        private static byte[] Utf8(string value) => new UTF8Encoding(false).GetBytes(value);
        private static string NestedObject(int depth) { var b = new StringBuilder("{\"x\":"); for (var i = 1; i < depth; i++) b.Append('['); b.Append('0'); for (var i = 1; i < depth; i++) b.Append(']'); return b.Append('}').ToString(); }
        private static byte[] RebuildRawBinding(ProfileCanonicalDocumentV1 source, string rawEscapedBinding) { var payload = Encoding.UTF8.GetString(source.CanonicalPayloadBytes); payload = payload.Replace("\"bindingOverridesJson\":\"\"", "\"bindingOverridesJson\":\"" + rawEscapedBinding + "\""); return Final(payload); }
        private static byte[] Final(string payload) { var hash = Hash(Utf8(payload)); return Utf8(payload.Substring(0, payload.Length - 1) + ",\"integrity\":{\"payloadSha256\":\"" + hash + "\"}}"); }
        private static string Hash(byte[] bytes) { using (var sha = SHA256.Create()) { var h = sha.ComputeHash(bytes); var b = new StringBuilder(64); foreach (var x in h) b.Append(x.ToString("x2", CultureInfo.InvariantCulture)); return b.ToString(); } }
        private static byte[] ReplaceAscii(byte[] source, string find, string replacement) => Utf8(Encoding.UTF8.GetString(source).Replace(find, replacement));
        private static ProfileCanonicalDecodeResultV1 Corrupt(ProfileCanonicalDecodeResultV1 source, string field, object value) { object boxed = source; typeof(ProfileCanonicalDecodeResultV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value); return (ProfileCanonicalDecodeResultV1)boxed; }
        private static void AssertUnavailableCurrent(ProfileCanonicalDecodeResultV1 value) { Assert.Throws<InvalidOperationException>(() => { var x = value.RecoveryProjection; }); Assert.Throws<InvalidOperationException>(() => { var x = value.InputRecoveryReasons; }); Assert.Throws<InvalidOperationException>(() => { var x = value.UnsupportedSchemaVersion; }); Assert.Throws<InvalidOperationException>(() => { var x = value.Failure; }); }
        private static void AssertUnavailableMetadata(ProfileCanonicalDecodeResultV1 value) { Assert.Throws<InvalidOperationException>(() => { var x = value.UnsupportedSchemaVersion; }); Assert.Throws<InvalidOperationException>(() => { var x = value.Failure; }); }
        private static void AssertUnavailableBinding(ProfileCanonicalDecodeResultV1 value) { Assert.Throws<InvalidOperationException>(() => { var x = value.Document; }); Assert.Throws<InvalidOperationException>(() => { var x = value.InputCompatibility; }); Assert.Throws<InvalidOperationException>(() => { var x = value.UnsupportedSchemaVersion; }); Assert.Throws<InvalidOperationException>(() => { var x = value.Failure; }); }
        private static void AssertUnavailableUnsupported(ProfileCanonicalDecodeResultV1 value) { Assert.Throws<InvalidOperationException>(() => { var x = value.Document; }); Assert.Throws<InvalidOperationException>(() => { var x = value.InputCompatibility; }); Assert.Throws<InvalidOperationException>(() => { var x = value.RecoveryProjection; }); Assert.Throws<InvalidOperationException>(() => { var x = value.InputRecoveryReasons; }); Assert.Throws<InvalidOperationException>(() => { var x = value.Failure; }); }
        private static void AssertUnavailableInvalid(ProfileCanonicalDecodeResultV1 value) { Assert.Throws<InvalidOperationException>(() => { var x = value.Document; }); Assert.Throws<InvalidOperationException>(() => { var x = value.InputCompatibility; }); Assert.Throws<InvalidOperationException>(() => { var x = value.RecoveryProjection; }); Assert.Throws<InvalidOperationException>(() => { var x = value.InputRecoveryReasons; }); Assert.Throws<InvalidOperationException>(() => { var x = value.UnsupportedSchemaVersion; }); }
        private static string FindRoot() { var current = new System.IO.DirectoryInfo(TestContext.CurrentContext.TestDirectory); while (current != null) { if (System.IO.File.Exists(System.IO.Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; } throw new System.IO.DirectoryNotFoundException(); }
    }
}
