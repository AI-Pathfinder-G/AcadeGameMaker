using System;
using System.Text;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D6-001..006. Engine-free input metadata compatibility; no Input System application authority.
    public static class ProfileInputContract
    {
        public const string CurrentInputActionsAssetId = "7c8d9e0f1a2b4c3d8e9f0a1b2c3d4e5f";
        public const int CurrentBindingSchemaVersion = 1;
    }

    [Flags]
    public enum ProfileInputCompatibility { Current = 0, AssetIdMismatch = 1, BindingSchemaMismatch = 2 }

    public readonly struct ProfileInputSnapshot
    {
        private readonly string _inputActionsAssetId;
        private readonly int _bindingSchemaVersion;
        private readonly ProfileBindingOverridesJson _bindingOverrides;

        public ProfileInputSnapshot(string inputActionsAssetId, int bindingSchemaVersion, ProfileBindingOverridesJson bindingOverrides)
        {
            ValidateIdArgument(inputActionsAssetId);
            if (bindingSchemaVersion < 0) throw new ArgumentOutOfRangeException(nameof(bindingSchemaVersion));
            try { bindingOverrides.Validate(); }
            catch (InvalidOperationException exception) { throw new ArgumentException("Binding overrides are malformed.", nameof(bindingOverrides), exception); }
            _inputActionsAssetId = inputActionsAssetId;
            _bindingSchemaVersion = bindingSchemaVersion;
            _bindingOverrides = bindingOverrides;
        }

        public string InputActionsAssetId { get { Validate(); return _inputActionsAssetId; } }
        public int BindingSchemaVersion { get { Validate(); return _bindingSchemaVersion; } }
        public ProfileBindingOverridesJson BindingOverrides { get { Validate(); return _bindingOverrides; } }
        public ProfileInputCompatibility Compatibility { get { Validate(); return CompatibilityUnchecked(); } }
        public bool IsCurrentCompatible { get { Validate(); return CompatibilityUnchecked() == ProfileInputCompatibility.Current; } }

        // REQ-M5D6-001..004. Every public consumption validates nested M5D5 data first.
        public void Validate()
        {
            if (!IsValidId(_inputActionsAssetId) || _bindingSchemaVersion < 0)
                throw new InvalidOperationException("Profile input snapshot is malformed.");
            try { _bindingOverrides.Validate(); }
            catch (InvalidOperationException exception) { throw new InvalidOperationException("Profile input override is malformed.", exception); }
        }

        private ProfileInputCompatibility CompatibilityUnchecked()
        {
            var result = ProfileInputCompatibility.Current;
            if (!string.Equals(_inputActionsAssetId, ProfileInputContract.CurrentInputActionsAssetId, StringComparison.Ordinal)) result |= ProfileInputCompatibility.AssetIdMismatch;
            if (_bindingSchemaVersion != ProfileInputContract.CurrentBindingSchemaVersion) result |= ProfileInputCompatibility.BindingSchemaMismatch;
            return result;
        }

        private static void ValidateIdArgument(string value)
        {
            if (!IsValidId(value)) throw new ArgumentException("Input asset ID must be non-empty surrogate-safe NFC.", nameof(value));
        }

        private static bool IsValidId(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            for (var index = 0; index < value.Length; index++)
            {
                if (char.IsHighSurrogate(value[index])) { if (index + 1 >= value.Length || !char.IsLowSurrogate(value[index + 1])) return false; index++; }
                else if (char.IsLowSurrogate(value[index])) return false;
            }
            return string.Equals(value, value.Normalize(NormalizationForm.FormC), StringComparison.Ordinal);
        }
    }
}
