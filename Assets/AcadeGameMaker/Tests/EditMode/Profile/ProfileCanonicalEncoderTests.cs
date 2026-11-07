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
    // REQ-M5D7A-001..006; AC-M5D7A-001..007. Exact profile-v1 encoder payload, integrity, and defensive-revalidation matrix.
    public sealed class ProfileCanonicalEncoderTests
    {
        [Test]
        public void AC001_AC002_GoldenPayloadUsesExactFixedOrderEnumsNullsAndInvariantIntegers()
        {
            var snapshot = new ProfileSnapshotV1(
                new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 0, 1000, 7, false, true),
                new ProfileInputSnapshot("wrong/id", int.MaxValue, ProfileBindingOverridesJson.Parse(string.Empty)),
                new ProfileTutorialSnapshot(new[] { "", "a", "é", "😀" }),
                new ProfileProgressionSnapshot(long.MaxValue, null, null, ProfileConsentState.RefusesOwnershipTransfer, null,
                    new[] { ProfileBranch.Extraction, ProfileBranch.Solidarity }));
            var document = ProfileCanonicalEncoder.Encode(snapshot);
            var expected = "{\"schemaVersion\":1,\"profileRevision\":9223372036854775807,\"settings\":{\"windowMode\":\"Windowed\",\"masterVolumeQ1000\":0,\"musicVolumeQ1000\":1000,\"sfxVolumeQ1000\":7,\"gamepadAimInvertX\":false,\"gamepadAimInvertY\":true},\"input\":{\"inputActionsAssetId\":\"wrong/id\",\"bindingSchemaVersion\":2147483647,\"bindingOverridesJson\":\"\"},\"tutorial\":{\"confirmedIds\":[\"\",\"a\",\"é\",\"😀\"]},\"progression\":{\"lastOfferedSeed\":null,\"committedChoice\":null,\"consentState\":\"RefusesOwnershipTransfer\",\"grantedSkill\":null,\"completedBranches\":[\"Extraction\",\"Solidarity\"]}}";
            Assert.That(Encoding.UTF8.GetString(document.CanonicalPayloadBytes), Is.EqualTo(expected));
            Assert.That(document.Snapshot.SchemaVersion, Is.EqualTo(1));
            StringAssert.DoesNotContain("Compatibility", expected);
            StringAssert.DoesNotContain("IsCurrentCompatible", expected);
        }

        [Test]
        public void AC002_AC003_OverridesOuterEscapeAndCultureDoNotChangeExactBytes()
        {
            var snapshot = Snapshot(ProfileBindingOverridesJson.Parse("{\"z\":\"\\\"\\\\\",\"a\":1}"),
                new[] { "a\u0001\\\"/é😀" }, 0, 101, ProfileChoice.Extraction, ProfileSkill.CompressionVerdict,
                ProfileConsentState.OffersScopedResonance, new[] { ProfileBranch.Extraction });
            var expected = "{\"schemaVersion\":1,\"profileRevision\":0,\"settings\":{\"windowMode\":\"BorderlessFullscreen\",\"masterVolumeQ1000\":1000,\"musicVolumeQ1000\":0,\"sfxVolumeQ1000\":500,\"gamepadAimInvertX\":true,\"gamepadAimInvertY\":false},\"input\":{\"inputActionsAssetId\":\"7c8d9e0f1a2b4c3d8e9f0a1b2c3d4e5f\",\"bindingSchemaVersion\":1,\"bindingOverridesJson\":\"{\\\"a\\\":1,\\\"z\\\":\\\"\\\\\\\"\\\\\\\\\\\"}\"},\"tutorial\":{\"confirmedIds\":[\"a\\u0001\\\\\\\"/é😀\"]},\"progression\":{\"lastOfferedSeed\":101,\"committedChoice\":\"Extraction\",\"consentState\":\"OffersScopedResonance\",\"grantedSkill\":\"CompressionVerdict\",\"completedBranches\":[\"Extraction\"]}}";
            var prior = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
                Assert.That(Encoding.UTF8.GetString(ProfileCanonicalEncoder.Encode(snapshot).CanonicalPayloadBytes), Is.EqualTo(expected));
            }
            finally { CultureInfo.CurrentCulture = prior; }
        }

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(int.MaxValue)]
        public void AC002_MismatchedInputCandidatesEncodeExactFieldsWithoutFallbackOrDerivedMetadata(int schemaVersion)
        {
            const string mismatchedAssetId = "different-authored-input-id";
            var input = new ProfileInputSnapshot(mismatchedAssetId, schemaVersion, ProfileBindingOverridesJson.Parse(string.Empty));
            Assert.That(input.Compatibility, Is.EqualTo(ProfileInputCompatibility.AssetIdMismatch | ProfileInputCompatibility.BindingSchemaMismatch));
            Assert.That(input.IsCurrentCompatible, Is.False);
            var snapshot = new ProfileSnapshotV1(
                new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false),
                input,
                new ProfileTutorialSnapshot(Array.Empty<string>()),
                new ProfileProgressionSnapshot(0, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));

            var payload = Encoding.UTF8.GetString(ProfileCanonicalEncoder.Encode(snapshot).CanonicalPayloadBytes);
            StringAssert.Contains("\"inputActionsAssetId\":\"different-authored-input-id\",\"bindingSchemaVersion\":"
                + schemaVersion.ToString(CultureInfo.InvariantCulture), payload);
            StringAssert.DoesNotContain(ProfileInputContract.CurrentInputActionsAssetId, payload);
            StringAssert.DoesNotContain("\"Compatibility\"", payload);
            StringAssert.DoesNotContain("\"IsCurrentCompatible\"", payload);
        }

        [Test]
        public void AC004_HashAndFinalFileAreExactLowercaseUtf8WithoutBomOrNewline()
        {
            var document = ProfileCanonicalEncoder.Encode(Snapshot(ProfileBindingOverridesJson.Parse(string.Empty), Array.Empty<string>(), 0, null, null, null,
                ProfileConsentState.ImprintSevered, Array.Empty<ProfileBranch>()));
            var payload = document.CanonicalPayloadBytes;
            var independent = IndependentHash(payload);
            Assert.That(document.PayloadSha256, Is.EqualTo(independent));
            Assert.That(document.PayloadSha256, Does.Match("^[0-9a-f]{64}$"));
            var text = Encoding.UTF8.GetString(document.CanonicalFileBytes);
            var payloadText = Encoding.UTF8.GetString(payload);
            Assert.That(text, Is.EqualTo(payloadText.Substring(0, payloadText.Length - 1) + ",\"integrity\":{\"payloadSha256\":\"" + independent + "\"}}"));
            Assert.That(Count(text, "\"integrity\""), Is.EqualTo(1));
            StringAssert.DoesNotContain("\"integrity\"", Encoding.UTF8.GetString(payload));
            Assert.That(document.CanonicalPayloadBytes[0], Is.Not.EqualTo(0xef));
            Assert.That(document.CanonicalFileBytes[0], Is.Not.EqualTo(0xef));
            Assert.That(text.EndsWith("\n", StringComparison.Ordinal), Is.False);
            Assert.That(text.EndsWith("\r", StringComparison.Ordinal), Is.False);
        }

        [Test]
        public void AC005_AC006_SourceCollectionsAndReturnedArraysAreDefensiveAndRepeatedEncodingIsIdempotent()
        {
            var ids = new[] { "a" };
            var branches = new[] { ProfileBranch.Extraction };
            var snapshot = Snapshot(ProfileBindingOverridesJson.Parse("{\"b\":2,\"a\":1}"), ids, 42, 202, ProfileChoice.Solidarity,
                ProfileSkill.CommonReferencePlane, ProfileConsentState.ResonanceLender, branches);
            ids[0] = "z";
            branches[0] = ProfileBranch.Solidarity;
            var first = ProfileCanonicalEncoder.Encode(snapshot);
            var second = ProfileCanonicalEncoder.Encode(snapshot);
            CollectionAssert.AreEqual(first.CanonicalPayloadBytes, second.CanonicalPayloadBytes);
            Assert.That(first.PayloadSha256, Is.EqualTo(second.PayloadSha256));
            CollectionAssert.AreEqual(first.CanonicalFileBytes, second.CanonicalFileBytes);
            var leakedPayload = first.CanonicalPayloadBytes; leakedPayload[0] = (byte)'!';
            var leakedFile = first.CanonicalFileBytes; leakedFile[0] = (byte)'!';
            Assert.That(Encoding.UTF8.GetString(first.CanonicalPayloadBytes).StartsWith("{", StringComparison.Ordinal), Is.True);
            Assert.That(Encoding.UTF8.GetString(first.CanonicalFileBytes).StartsWith("{", StringComparison.Ordinal), Is.True);
            first.Validate();
        }

        [Test]
        public void AC001_AC006_DefaultAndReflectionBypassNestedSnapshotDocumentHashAndBytesReject()
        {
            var valid = Snapshot(ProfileBindingOverridesJson.Parse(string.Empty), Array.Empty<string>(), 0, null, null, null,
                ProfileConsentState.RefusesOwnershipTransfer, Array.Empty<ProfileBranch>());
            var defaultSnapshot = default(ProfileSnapshotV1);
            Assert.Throws<InvalidOperationException>(() => defaultSnapshot.Validate());
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSnapshot.SchemaVersion; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSnapshot.ProfileRevision; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSnapshot.Settings; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSnapshot.Input; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSnapshot.Tutorial; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSnapshot.Progression; });
            Assert.Throws<InvalidOperationException>(() => ProfileCanonicalEncoder.Encode(defaultSnapshot));
            Assert.Throws<ArgumentException>(() => new ProfileSnapshotV1(default(ProfileSettingsSnapshot), valid.Input, valid.Tutorial, valid.Progression));
            var bypassedSnapshot = Corrupt(valid, "_settings", default(ProfileSettingsSnapshot));
            Assert.Throws<InvalidOperationException>(() => bypassedSnapshot.Validate());
            Assert.Throws<InvalidOperationException>(() => ProfileCanonicalEncoder.Encode(bypassedSnapshot));

            var document = ProfileCanonicalEncoder.Encode(valid);
            var defaultDocument = default(ProfileCanonicalDocumentV1);
            Assert.Throws<InvalidOperationException>(() => defaultDocument.Validate());
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultDocument.Snapshot; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultDocument.CanonicalPayloadBytes; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultDocument.PayloadSha256; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultDocument.CanonicalFileBytes; });
            Assert.Throws<InvalidOperationException>(() => Corrupt(document, "_snapshot", default(ProfileSnapshotV1)).Validate());
            Assert.Throws<InvalidOperationException>(() => Corrupt(document, "_payloadSha256", new string('0', 64)).Validate());
            var changedPayload = document.CanonicalPayloadBytes; changedPayload[0] = (byte)'!';
            Assert.Throws<InvalidOperationException>(() => Corrupt(document, "_canonicalPayloadBytes", changedPayload).Validate());
            var changedFile = document.CanonicalFileBytes; changedFile[0] = (byte)'!';
            Assert.Throws<InvalidOperationException>(() => Corrupt(document, "_canonicalFileBytes", changedFile).Validate());
        }

        private static int Count(string value, string needle)
        {
            var count = 0; var offset = 0;
            while ((offset = value.IndexOf(needle, offset, StringComparison.Ordinal)) >= 0) { count++; offset += needle.Length; }
            return count;
        }

        private static string IndependentHash(byte[] value)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(value);
                var builder = new StringBuilder(bytes.Length * 2);
                foreach (var valueByte in bytes) builder.Append(valueByte.ToString("x2", CultureInfo.InvariantCulture));
                return builder.ToString();
            }
        }

        private static ProfileSnapshotV1 Snapshot(ProfileBindingOverridesJson overrides, IReadOnlyList<string> ids, long revision, int? seed,
            ProfileChoice? choice, ProfileSkill? skill, ProfileConsentState consent, IReadOnlyList<ProfileBranch> branches)
        {
            return new ProfileSnapshotV1(
                new ProfileSettingsSnapshot(ProfileWindowMode.BorderlessFullscreen, 1000, 0, 500, true, false),
                new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, overrides),
                new ProfileTutorialSnapshot(ids),
                new ProfileProgressionSnapshot(revision, seed, choice, consent, skill, branches));
        }

        private static ProfileSnapshotV1 Corrupt(ProfileSnapshotV1 source, string field, object value)
        {
            object boxed = source;
            typeof(ProfileSnapshotV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value);
            return (ProfileSnapshotV1)boxed;
        }

        private static ProfileCanonicalDocumentV1 Corrupt(ProfileCanonicalDocumentV1 source, string field, object value)
        {
            object boxed = source;
            typeof(ProfileCanonicalDocumentV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value);
            return (ProfileCanonicalDocumentV1)boxed;
        }
    }

}
