using System;
using System.Collections.Generic;
using System.Text;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D4-001..006. Engine-free settings/tutorial value projection only.
    public enum ProfileWindowMode
    {
        Windowed = 1,
        BorderlessFullscreen = 2
    }

    public readonly struct ProfileSettingsSnapshot
    {
        private readonly ProfileWindowMode _windowMode;
        private readonly int _masterVolumeQ1000;
        private readonly int _musicVolumeQ1000;
        private readonly int _sfxVolumeQ1000;
        private readonly bool _gamepadAimInvertX;
        private readonly bool _gamepadAimInvertY;

        public ProfileSettingsSnapshot(ProfileWindowMode windowMode, int masterVolumeQ1000, int musicVolumeQ1000,
            int sfxVolumeQ1000, bool gamepadAimInvertX, bool gamepadAimInvertY)
        {
            ProfileSettingsTutorialValidation.ValidateWindowModeArgument(windowMode);
            ProfileSettingsTutorialValidation.ValidateQ1000Argument(masterVolumeQ1000);
            ProfileSettingsTutorialValidation.ValidateQ1000Argument(musicVolumeQ1000);
            ProfileSettingsTutorialValidation.ValidateQ1000Argument(sfxVolumeQ1000);
            _windowMode = windowMode;
            _masterVolumeQ1000 = masterVolumeQ1000;
            _musicVolumeQ1000 = musicVolumeQ1000;
            _sfxVolumeQ1000 = sfxVolumeQ1000;
            _gamepadAimInvertX = gamepadAimInvertX;
            _gamepadAimInvertY = gamepadAimInvertY;
        }

        public ProfileWindowMode WindowMode { get { Validate(); return _windowMode; } }
        public int MasterVolumeQ1000 { get { Validate(); return _masterVolumeQ1000; } }
        public int MusicVolumeQ1000 { get { Validate(); return _musicVolumeQ1000; } }
        public int SfxVolumeQ1000 { get { Validate(); return _sfxVolumeQ1000; } }
        public bool GamepadAimInvertX { get { Validate(); return _gamepadAimInvertX; } }
        public bool GamepadAimInvertY { get { Validate(); return _gamepadAimInvertY; } }

        // REQ-M5D4-001, REQ-M5D4-003. Public consumption rejects default and bypassed values.
        public void Validate()
        {
            if (!ProfileSettingsTutorialValidation.IsKnownWindowMode(_windowMode)
                || !ProfileSettingsTutorialValidation.IsQ1000(_masterVolumeQ1000)
                || !ProfileSettingsTutorialValidation.IsQ1000(_musicVolumeQ1000)
                || !ProfileSettingsTutorialValidation.IsQ1000(_sfxVolumeQ1000))
                throw new InvalidOperationException("Profile settings snapshot is malformed.");
        }
    }

    public readonly struct ProfileTutorialSnapshot
    {
        private readonly string[] _confirmedIds;

        public ProfileTutorialSnapshot(IReadOnlyList<string> confirmedIds)
        {
            _confirmedIds = ProfileSettingsTutorialValidation.CopyAndValidateConfirmedIdsArgument(confirmedIds);
        }

        public IReadOnlyList<string> ConfirmedIds
        {
            get
            {
                Validate();
                return Array.AsReadOnly((string[])_confirmedIds.Clone());
            }
        }

        // REQ-M5D4-002..004. Validation preserves already-canonical authored values; it never repairs IDs.
        public void Validate()
        {
            if (!ProfileSettingsTutorialValidation.AreCanonicalConfirmedIds(_confirmedIds))
                throw new InvalidOperationException("Profile tutorial snapshot is malformed.");
        }
    }

    internal static class ProfileSettingsTutorialValidation
    {
        internal static bool IsKnownWindowMode(ProfileWindowMode value)
        {
            return value == ProfileWindowMode.Windowed || value == ProfileWindowMode.BorderlessFullscreen;
        }

        internal static bool IsQ1000(int value) => value >= 0 && value <= 1000;

        internal static void ValidateWindowModeArgument(ProfileWindowMode value)
        {
            if (!IsKnownWindowMode(value)) throw new ArgumentOutOfRangeException(nameof(value));
        }

        internal static void ValidateQ1000Argument(int value)
        {
            if (!IsQ1000(value)) throw new ArgumentOutOfRangeException(nameof(value));
        }

        internal static string[] CopyAndValidateConfirmedIdsArgument(IReadOnlyList<string> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var copy = new string[values.Count];
            for (var index = 0; index < copy.Length; index++) copy[index] = values[index];
            if (!AreCanonicalConfirmedIds(copy)) throw new ArgumentException("Tutorial IDs must be canonical ordinal values.", nameof(values));
            return copy;
        }

        internal static bool AreCanonicalConfirmedIds(string[] values)
        {
            if (values == null) return false;
            string previous = null;
            for (var index = 0; index < values.Length; index++)
            {
                var current = values[index];
                if (!IsCanonicalTutorialId(current) || (previous != null && StringComparer.Ordinal.Compare(previous, current) >= 0))
                    return false;
                previous = current;
            }
            return true;
        }

        internal static bool IsCanonicalTutorialId(string value)
        {
            // Surrogate validity must be decided before Normalize can inspect the UTF-16 payload.
            if (!HasNoUnpairedSurrogates(value)) return false;
            return string.Equals(value, value.Normalize(NormalizationForm.FormC), StringComparison.Ordinal);
        }

        internal static bool HasNoUnpairedSurrogates(string value)
        {
            if (value == null) return false;
            for (var index = 0; index < value.Length; index++)
            {
                var current = value[index];
                if (char.IsHighSurrogate(current))
                {
                    if (index + 1 >= value.Length || !char.IsLowSurrogate(value[index + 1])) return false;
                    index++;
                }
                else if (char.IsLowSurrogate(current)) return false;
            }
            return true;
        }
    }
}
