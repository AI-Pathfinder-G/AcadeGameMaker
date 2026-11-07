using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D6-001..006; AC-M5D6-001..005. Metadata-only input compatibility matrix.
    public sealed class ProfileInputSnapshotTests
    {
        [Test]
        public void AC001_CurrentMetadataAndOverridesAreExactAndStructuralInvalidsReject()
        {
            var snapshot = Current(ProfileBindingOverridesJson.Parse("{\"x\":1}"));
            Assert.That(snapshot.InputActionsAssetId, Is.EqualTo(ProfileInputContract.CurrentInputActionsAssetId));
            Assert.That(snapshot.BindingSchemaVersion, Is.EqualTo(1));
            Assert.That(snapshot.BindingOverrides.CanonicalText, Is.EqualTo("{\"x\":1}"));
            Assert.Throws<ArgumentException>(() => new ProfileInputSnapshot(null, 1, Empty()));
            Assert.Throws<ArgumentException>(() => new ProfileInputSnapshot("", 1, Empty()));
            Assert.Throws<ArgumentException>(() => new ProfileInputSnapshot("e\u0301", 1, Empty()));
            Assert.Throws<ArgumentException>(() => new ProfileInputSnapshot("\ud800", 1, Empty()));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileInputSnapshot("id", -1, Empty()));
            Assert.Throws<ArgumentException>(() => new ProfileInputSnapshot("id", 1, default(ProfileBindingOverridesJson)));
        }

        [Test]
        public void AC002_CompatibilityFlagsUseExactOrdinalCultureIndependentComparison()
        {
            Assert.That(Current(Empty()).Compatibility, Is.EqualTo(ProfileInputCompatibility.Current));
            Assert.That(new ProfileInputSnapshot("other", 1, Empty()).Compatibility, Is.EqualTo(ProfileInputCompatibility.AssetIdMismatch));
            Assert.That(new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, 2, Empty()).Compatibility, Is.EqualTo(ProfileInputCompatibility.BindingSchemaMismatch));
            Assert.That(new ProfileInputSnapshot("other", 2, Empty()).Compatibility, Is.EqualTo((ProfileInputCompatibility)3));
            var prior = CultureInfo.CurrentCulture; try { CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR"); Assert.That(new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId.ToUpperInvariant(), 1, Empty()).Compatibility, Is.EqualTo(ProfileInputCompatibility.AssetIdMismatch)); } finally { CultureInfo.CurrentCulture = prior; }
        }

        [Test]
        public void AC003_BindingSchemaValuesRemainStructuralButMismatched()
        {
            foreach (var version in new[] { 0, 2, int.MaxValue })
                Assert.That(new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, version, Empty()).Compatibility,
                    Is.EqualTo(ProfileInputCompatibility.BindingSchemaMismatch));
        }

        [Test]
        public void AC004_DefaultBypassNestedInvalidAndEveryGetterThrow()
        {
            var value = default(ProfileInputSnapshot);
            Assert.Throws<InvalidOperationException>(() => value.Validate());
            Assert.Throws<InvalidOperationException>(() => { var x = value.InputActionsAssetId; });
            Assert.Throws<InvalidOperationException>(() => { var x = value.BindingSchemaVersion; });
            Assert.Throws<InvalidOperationException>(() => { var x = value.BindingOverrides; });
            Assert.Throws<InvalidOperationException>(() => { var x = value.Compatibility; });
            Assert.Throws<InvalidOperationException>(() => { var x = value.IsCurrentCompatible; });
            var badOverride = Corrupt(Current(Empty()), "_bindingOverrides", default(ProfileBindingOverridesJson));
            Assert.Throws<InvalidOperationException>(() => badOverride.Validate());
            Assert.Throws<InvalidOperationException>(() => { var x = badOverride.Compatibility; });
            foreach (var badId in new string[] { null, "", "e\u0301" })
            {
                var bypassed = Corrupt(Current(Empty()), "_inputActionsAssetId", badId);
                Assert.Throws<InvalidOperationException>(() => bypassed.Validate());
                Assert.Throws<InvalidOperationException>(() => { var x = bypassed.InputActionsAssetId; });
                Assert.Throws<InvalidOperationException>(() => { var x = bypassed.Compatibility; });
            }
            var badVersion = Corrupt(Current(Empty()), "_bindingSchemaVersion", -1);
            Assert.Throws<InvalidOperationException>(() => badVersion.Validate());
            Assert.Throws<InvalidOperationException>(() => { var x = badVersion.BindingSchemaVersion; });
            Assert.Throws<InvalidOperationException>(() => { var x = badVersion.Compatibility; });
            Current(Empty()).Validate();
        }

        [Test]
        public void AC005_ConstantsMatchMetaAndDoNotConfuseOuterSchema()
        {
            var root = FindRoot();
            StringAssert.Contains("guid: " + ProfileInputContract.CurrentInputActionsAssetId, File.ReadAllText(Path.Combine(root, "Assets", "GameInput.inputactions.meta")));
            var source = File.ReadAllText(Path.Combine(root, "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileInputSnapshot.cs"));
            StringAssert.Contains("CurrentBindingSchemaVersion = 1", source);
            StringAssert.DoesNotContain("OuterSchema", source);
            StringAssert.DoesNotContain("UnityEngine", source);
        }

        private static ProfileBindingOverridesJson Empty() => ProfileBindingOverridesJson.Parse(string.Empty);
        private static ProfileInputSnapshot Current(ProfileBindingOverridesJson overrides) => new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, overrides);
        private static ProfileInputSnapshot Corrupt(ProfileInputSnapshot source, string field, object value) { object boxed = source; typeof(ProfileInputSnapshot).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value); return (ProfileInputSnapshot)boxed; }
        private static string FindRoot() { var current = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); while (current != null) { if (File.Exists(Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; } throw new DirectoryNotFoundException(); }
    }
}
