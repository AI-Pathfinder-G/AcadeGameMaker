using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D7A-001..006. Engine-free profile-v1 canonical encoding only; no parse, IO, recovery, or input application authority.
    public static class ProfileContract
    {
        public const int SchemaVersion = 1;
    }

    public readonly struct ProfileSnapshotV1
    {
        private readonly ProfileSettingsSnapshot _settings;
        private readonly ProfileInputSnapshot _input;
        private readonly ProfileTutorialSnapshot _tutorial;
        private readonly ProfileProgressionSnapshot _progression;

        public ProfileSnapshotV1(ProfileSettingsSnapshot settings, ProfileInputSnapshot input,
            ProfileTutorialSnapshot tutorial, ProfileProgressionSnapshot progression)
        {
            ValidateNestedArgument(settings, nameof(settings));
            ValidateNestedArgument(input, nameof(input));
            ValidateNestedArgument(tutorial, nameof(tutorial));
            ValidateNestedArgument(progression, nameof(progression));
            _settings = settings;
            _input = input;
            _tutorial = tutorial;
            _progression = progression;
        }

        public int SchemaVersion { get { Validate(); return ProfileContract.SchemaVersion; } }
        public long ProfileRevision { get { Validate(); return _progression.ProfileRevision; } }
        public ProfileSettingsSnapshot Settings { get { Validate(); return _settings; } }
        public ProfileInputSnapshot Input { get { Validate(); return _input; } }
        public ProfileTutorialSnapshot Tutorial { get { Validate(); return _tutorial; } }
        public ProfileProgressionSnapshot Progression { get { Validate(); return _progression; } }

        // REQ-M5D7A-001, REQ-M5D7A-004. Nested values are revalidated at every public consumption boundary.
        public void Validate()
        {
            _settings.Validate();
            _input.Validate();
            _tutorial.Validate();
            _progression.Validate();
        }

        private static void ValidateNestedArgument(ProfileSettingsSnapshot value, string name) { try { value.Validate(); } catch (InvalidOperationException exception) { throw new ArgumentException("Nested profile value is malformed.", name, exception); } }
        private static void ValidateNestedArgument(ProfileInputSnapshot value, string name) { try { value.Validate(); } catch (InvalidOperationException exception) { throw new ArgumentException("Nested profile value is malformed.", name, exception); } }
        private static void ValidateNestedArgument(ProfileTutorialSnapshot value, string name) { try { value.Validate(); } catch (InvalidOperationException exception) { throw new ArgumentException("Nested profile value is malformed.", name, exception); } }
        private static void ValidateNestedArgument(ProfileProgressionSnapshot value, string name) { try { value.Validate(); } catch (InvalidOperationException exception) { throw new ArgumentException("Nested profile value is malformed.", name, exception); } }
    }

    public readonly struct ProfileCanonicalDocumentV1
    {
        private readonly ProfileSnapshotV1 _snapshot;
        private readonly byte[] _canonicalPayloadBytes;
        private readonly string _payloadSha256;
        private readonly byte[] _canonicalFileBytes;

        internal ProfileCanonicalDocumentV1(ProfileSnapshotV1 snapshot, byte[] canonicalPayloadBytes,
            string payloadSha256, byte[] canonicalFileBytes)
        {
            _snapshot = snapshot;
            _canonicalPayloadBytes = (byte[])canonicalPayloadBytes.Clone();
            _payloadSha256 = payloadSha256;
            _canonicalFileBytes = (byte[])canonicalFileBytes.Clone();
        }

        public ProfileSnapshotV1 Snapshot { get { Validate(); return _snapshot; } }
        public byte[] CanonicalPayloadBytes { get { Validate(); return (byte[])_canonicalPayloadBytes.Clone(); } }
        public string PayloadSha256 { get { Validate(); return _payloadSha256; } }
        public byte[] CanonicalFileBytes { get { Validate(); return (byte[])_canonicalFileBytes.Clone(); } }

        // REQ-M5D7A-003, REQ-M5D7A-004. Stored representation is never trusted by marker, length, or hash shape alone.
        public void Validate()
        {
            _snapshot.Validate();
            if (_canonicalPayloadBytes == null || _payloadSha256 == null || _canonicalFileBytes == null)
                throw new InvalidOperationException("Profile canonical document representation is absent.");

            var regenerated = ProfileCanonicalEncoder.BuildValidated(_snapshot);
            if (!ByteArraysEqual(_canonicalPayloadBytes, regenerated.Payload)
                || !string.Equals(_payloadSha256, regenerated.Hash, StringComparison.Ordinal)
                || !ByteArraysEqual(_canonicalFileBytes, regenerated.File))
                throw new InvalidOperationException("Profile canonical document representation is malformed.");
        }

        private static bool ByteArraysEqual(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            for (var index = 0; index < left.Length; index++) if (left[index] != right[index]) return false;
            return true;
        }
    }

    public static class ProfileCanonicalEncoder
    {
        public static ProfileCanonicalDocumentV1 Encode(ProfileSnapshotV1 snapshot)
        {
            snapshot.Validate();
            var built = BuildValidated(snapshot);
            return new ProfileCanonicalDocumentV1(snapshot, built.Payload, built.Hash, built.File);
        }

        internal static CanonicalEncoding BuildValidated(ProfileSnapshotV1 snapshot)
        {
            snapshot.Validate();
            var payloadText = BuildPayloadText(snapshot);
            var payload = new UTF8Encoding(false).GetBytes(payloadText);
            var hash = ComputeLowercaseSha256(payload);
            var file = new UTF8Encoding(false).GetBytes(payloadText.Substring(0, payloadText.Length - 1)
                + ",\"integrity\":{\"payloadSha256\":\"" + hash + "\"}}");
            return new CanonicalEncoding(payload, hash, file);
        }

        internal readonly struct CanonicalEncoding
        {
            internal readonly byte[] Payload;
            internal readonly string Hash;
            internal readonly byte[] File;

            internal CanonicalEncoding(byte[] payload, string hash, byte[] file)
            {
                Payload = payload;
                Hash = hash;
                File = file;
            }
        }

        private static string BuildPayloadText(ProfileSnapshotV1 snapshot)
        {
            var settings = snapshot.Settings;
            var input = snapshot.Input;
            var tutorial = snapshot.Tutorial;
            var progression = snapshot.Progression;
            var builder = new StringBuilder();
            builder.Append("{\"schemaVersion\":1,\"profileRevision\":");
            AppendLong(builder, progression.ProfileRevision);
            builder.Append(",\"settings\":{\"windowMode\":");
            AppendQuoted(builder, WindowModeText(settings.WindowMode));
            builder.Append(",\"masterVolumeQ1000\":"); AppendInt(builder, settings.MasterVolumeQ1000);
            builder.Append(",\"musicVolumeQ1000\":"); AppendInt(builder, settings.MusicVolumeQ1000);
            builder.Append(",\"sfxVolumeQ1000\":"); AppendInt(builder, settings.SfxVolumeQ1000);
            builder.Append(",\"gamepadAimInvertX\":"); builder.Append(settings.GamepadAimInvertX ? "true" : "false");
            builder.Append(",\"gamepadAimInvertY\":"); builder.Append(settings.GamepadAimInvertY ? "true" : "false");
            builder.Append("},\"input\":{\"inputActionsAssetId\":");
            AppendQuoted(builder, input.InputActionsAssetId);
            builder.Append(",\"bindingSchemaVersion\":"); AppendInt(builder, input.BindingSchemaVersion);
            builder.Append(",\"bindingOverridesJson\":"); AppendQuoted(builder, input.BindingOverrides.CanonicalText);
            builder.Append("},\"tutorial\":{\"confirmedIds\":");
            AppendStringArray(builder, tutorial.ConfirmedIds);
            builder.Append("},\"progression\":{\"lastOfferedSeed\":");
            AppendNullableInt(builder, progression.LastOfferedSeed);
            builder.Append(",\"committedChoice\":");
            AppendNullableChoice(builder, progression.CommittedChoice);
            builder.Append(",\"consentState\":"); AppendQuoted(builder, ConsentText(progression.ConsentState));
            builder.Append(",\"grantedSkill\":");
            AppendNullableSkill(builder, progression.GrantedSkill);
            builder.Append(",\"completedBranches\":");
            AppendBranchArray(builder, progression.CompletedBranches);
            builder.Append("}}");
            return builder.ToString();
        }

        private static string ComputeLowercaseSha256(byte[] bytes)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(bytes);
                var builder = new StringBuilder(hash.Length * 2);
                for (var index = 0; index < hash.Length; index++) builder.Append(hash[index].ToString("x2", CultureInfo.InvariantCulture));
                return builder.ToString();
            }
        }

        private static void AppendInt(StringBuilder builder, int value) { builder.Append(value.ToString(CultureInfo.InvariantCulture)); }
        private static void AppendLong(StringBuilder builder, long value) { builder.Append(value.ToString(CultureInfo.InvariantCulture)); }
        private static void AppendNullableInt(StringBuilder builder, int? value) { if (value.HasValue) AppendInt(builder, value.Value); else builder.Append("null"); }
        private static void AppendNullableChoice(StringBuilder builder, ProfileChoice? value) { if (value.HasValue) AppendQuoted(builder, ChoiceText(value.Value)); else builder.Append("null"); }
        private static void AppendNullableSkill(StringBuilder builder, ProfileSkill? value) { if (value.HasValue) AppendQuoted(builder, SkillText(value.Value)); else builder.Append("null"); }

        private static void AppendStringArray(StringBuilder builder, System.Collections.Generic.IReadOnlyList<string> values)
        {
            builder.Append('[');
            for (var index = 0; index < values.Count; index++) { if (index != 0) builder.Append(','); AppendQuoted(builder, values[index]); }
            builder.Append(']');
        }

        private static void AppendBranchArray(StringBuilder builder, System.Collections.Generic.IReadOnlyList<ProfileBranch> values)
        {
            builder.Append('[');
            for (var index = 0; index < values.Count; index++) { if (index != 0) builder.Append(','); AppendQuoted(builder, BranchText(values[index])); }
            builder.Append(']');
        }

        // REQ-M5D7A-002. Existing cores guarantee surrogate-safe NFC; this writer preserves, never normalizes, the approved scalars.
        private static void AppendQuoted(StringBuilder builder, string value)
        {
            builder.Append('"');
            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                switch (character)
                {
                    case '"': builder.Append("\\\""); break;
                    case '\\': builder.Append("\\\\"); break;
                    case '\b': builder.Append("\\b"); break;
                    case '\t': builder.Append("\\t"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\f': builder.Append("\\f"); break;
                    case '\r': builder.Append("\\r"); break;
                    default:
                        if (character <= 0x1f)
                        {
                            builder.Append("\\u00");
                            builder.Append(((int)character).ToString("x2", CultureInfo.InvariantCulture));
                        }
                        else builder.Append(character);
                        break;
                }
            }
            builder.Append('"');
        }

        private static string WindowModeText(ProfileWindowMode value)
        {
            if (value == ProfileWindowMode.Windowed) return "Windowed";
            if (value == ProfileWindowMode.BorderlessFullscreen) return "BorderlessFullscreen";
            throw new InvalidOperationException("Profile window mode is malformed.");
        }

        private static string ChoiceText(ProfileChoice value)
        {
            if (value == ProfileChoice.Extraction) return "Extraction";
            if (value == ProfileChoice.Solidarity) return "Solidarity";
            throw new InvalidOperationException("Profile choice is malformed.");
        }

        private static string ConsentText(ProfileConsentState value)
        {
            if (value == ProfileConsentState.RefusesOwnershipTransfer) return "RefusesOwnershipTransfer";
            if (value == ProfileConsentState.OffersScopedResonance) return "OffersScopedResonance";
            if (value == ProfileConsentState.ResonanceLender) return "ResonanceLender";
            if (value == ProfileConsentState.ImprintSevered) return "ImprintSevered";
            throw new InvalidOperationException("Profile consent state is malformed.");
        }

        private static string SkillText(ProfileSkill value)
        {
            if (value == ProfileSkill.CompressionVerdict) return "CompressionVerdict";
            if (value == ProfileSkill.CommonReferencePlane) return "CommonReferencePlane";
            throw new InvalidOperationException("Profile skill is malformed.");
        }

        private static string BranchText(ProfileBranch value)
        {
            if (value == ProfileBranch.Extraction) return "Extraction";
            if (value == ProfileBranch.Solidarity) return "Solidarity";
            throw new InvalidOperationException("Profile branch is malformed.");
        }
    }
}
