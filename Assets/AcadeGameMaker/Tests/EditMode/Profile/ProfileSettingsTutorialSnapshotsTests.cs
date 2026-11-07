using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D4-001..006; AC-M5D4-001..005. Includes Luna P2 surrogate/NFC/no-repair counterexamples.
    public sealed class ProfileSettingsTutorialSnapshotsTests
    {
        [Test]
        public void AC001_SettingsPreserveExactModesVolumesAndInvertFlagsAndRejectInvalidValues()
        {
            var windowed = new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 0, 1000, 0, false, true);
            var borderless = new ProfileSettingsSnapshot(ProfileWindowMode.BorderlessFullscreen, 1000, 0, 1000, true, false);
            AssertSettings(windowed, ProfileWindowMode.Windowed, 0, 1000, 0, false, true);
            AssertSettings(borderless, ProfileWindowMode.BorderlessFullscreen, 1000, 0, 1000, true, false);

            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileSettingsSnapshot((ProfileWindowMode)0, 0, 0, 0, false, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileSettingsSnapshot((ProfileWindowMode)99, 0, 0, 0, false, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, -1, 0, 0, false, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 1001, 0, 0, false, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 0, -1, 0, false, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 0, 1001, 0, false, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 0, 0, -1, false, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 0, 0, 1001, false, false));

            Assert.Throws<InvalidOperationException>(() => CorruptSettings(windowed, "_windowMode", (ProfileWindowMode)0).Validate());
            Assert.Throws<InvalidOperationException>(() => CorruptSettings(windowed, "_masterVolumeQ1000", -1).Validate());
            Assert.Throws<InvalidOperationException>(() => CorruptSettings(windowed, "_musicVolumeQ1000", 1001).Validate());
            Assert.Throws<InvalidOperationException>(() => CorruptSettings(windowed, "_sfxVolumeQ1000", -1).Validate());
        }

        [Test]
        public void AC002_TutorialIdsRequireSurrogateSafeNfcOrdinalCanonicalValuesWithoutRepair()
        {
            AssertIds(new string[0]);
            AssertIds(new[] { "" }, "");
            AssertIds(new[] { "a", "\u00e9", "\ud83d\ude00" }, "a", "\u00e9", "\ud83d\ude00");
            AssertIds(new[] { "\ud83d\ude00" }, "\ud83d\ude00"); // valid surrogate pair

            Assert.Throws<ArgumentNullException>(() => new ProfileTutorialSnapshot(null));
            Assert.Throws<ArgumentException>(() => new ProfileTutorialSnapshot(new string[] { null }));
            Assert.Throws<ArgumentException>(() => new ProfileTutorialSnapshot(new[] { "same", "same" }));
            Assert.Throws<ArgumentException>(() => new ProfileTutorialSnapshot(new[] { "z", "a" }));
            Assert.Throws<ArgumentException>(() => new ProfileTutorialSnapshot(new[] { "\ud83d" })); // unpaired high
            Assert.Throws<ArgumentException>(() => new ProfileTutorialSnapshot(new[] { "\ude00" })); // unpaired low
            Assert.Throws<ArgumentException>(() => new ProfileTutorialSnapshot(new[] { "e\u0301" })); // decomposed, non-NFC
            Assert.Throws<ArgumentException>(() => new ProfileTutorialSnapshot(new[] { "\u00e9", "e\u0301" })); // collision is rejected, never normalized/deduped
        }

        [Test]
        public void AC003_TutorialCollectionsAreDefensivelyCopiedFromSourceAndOnEveryGetter()
        {
            var source = new[] { "a", "b" };
            var snapshot = new ProfileTutorialSnapshot(source);
            source[0] = "z";
            AssertIds(snapshot.ConfirmedIds, "a", "b");

            var returned = snapshot.ConfirmedIds;
            Assert.Throws<NotSupportedException>(() => ((IList)returned)[0] = "z");
            Assert.Throws<NotSupportedException>(() => ((IList<string>)returned)[0] = "z");
            AssertIds(snapshot.ConfirmedIds, "a", "b");
            AssertIds(snapshot.ConfirmedIds, "a", "b");
        }

        [Test]
        public void AC004_DefaultAndConstructorBypassValuesAreRejectedByEveryPublicGetter()
        {
            var defaultSettings = default(ProfileSettingsSnapshot);
            Assert.Throws<InvalidOperationException>(() => defaultSettings.Validate());
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSettings.WindowMode; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSettings.MasterVolumeQ1000; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSettings.MusicVolumeQ1000; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSettings.SfxVolumeQ1000; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSettings.GamepadAimInvertX; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultSettings.GamepadAimInvertY; });

            var defaultTutorial = default(ProfileTutorialSnapshot);
            Assert.Throws<InvalidOperationException>(() => defaultTutorial.Validate());
            Assert.Throws<InvalidOperationException>(() => { var ignored = defaultTutorial.ConfirmedIds; });

            var bypassedSettings = CorruptSettings(ValidSettings(), "_masterVolumeQ1000", 1001);
            Assert.Throws<InvalidOperationException>(() => { var ignored = bypassedSettings.GamepadAimInvertX; });
            var bypassedTutorial = CorruptTutorial(new ProfileTutorialSnapshot(new[] { "a" }), "_confirmedIds", new[] { "e\u0301" });
            Assert.Throws<InvalidOperationException>(() => { var ignored = bypassedTutorial.ConfirmedIds; });

            ValidSettings().Validate();
            new ProfileTutorialSnapshot(new[] { "" }).Validate();
        }

        [Test]
        public void AC005_RuntimeSourceStaysEngineFreeAndSurrogateScanPrecedesNormalization()
        {
            var root = FindProjectRoot();
            var source = File.ReadAllText(Path.Combine(root, "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileSettingsTutorialSnapshots.cs"));
            StringAssert.Contains("REQ-M5D4-001..006", source);
            Assert.That(source.IndexOf("HasNoUnpairedSurrogates(value)", StringComparison.Ordinal), Is.LessThan(source.IndexOf("value.Normalize", StringComparison.Ordinal)));
            foreach (var forbidden in new[] { "UnityEngine", "System.IO", "Cryptography", "Json", "DateTime", "Random", "System.Net", "callback", "ProfileProgressionSnapshot" })
                StringAssert.DoesNotContain(forbidden, source);
        }

        private static ProfileSettingsSnapshot ValidSettings() => new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false);

        private static void AssertSettings(ProfileSettingsSnapshot snapshot, ProfileWindowMode mode, int master, int music, int sfx, bool invertX, bool invertY)
        {
            snapshot.Validate();
            Assert.That(snapshot.WindowMode, Is.EqualTo(mode));
            Assert.That(snapshot.MasterVolumeQ1000, Is.EqualTo(master));
            Assert.That(snapshot.MusicVolumeQ1000, Is.EqualTo(music));
            Assert.That(snapshot.SfxVolumeQ1000, Is.EqualTo(sfx));
            Assert.That(snapshot.GamepadAimInvertX, Is.EqualTo(invertX));
            Assert.That(snapshot.GamepadAimInvertY, Is.EqualTo(invertY));
        }

        private static void AssertIds(IReadOnlyList<string> values, params string[] expected)
        {
            Assert.That(values, Is.EqualTo(expected));
        }

        private static ProfileSettingsSnapshot CorruptSettings(ProfileSettingsSnapshot source, string fieldName, object value)
        {
            object boxed = source;
            typeof(ProfileSettingsSnapshot).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value);
            return (ProfileSettingsSnapshot)boxed;
        }

        private static ProfileTutorialSnapshot CorruptTutorial(ProfileTutorialSnapshot source, string fieldName, object value)
        {
            object boxed = source;
            typeof(ProfileTutorialSnapshot).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value);
            return (ProfileTutorialSnapshot)boxed;
        }

        private static string FindProjectRoot()
        {
            var current = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName;
                current = current.Parent;
            }
            throw new DirectoryNotFoundException("Project root was not found.");
        }
    }
}
