using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D7B-001..006. Strict engine-free profile-v1 envelope validation; no IO, mutation, recovery write, or input application.
    public enum ProfileCanonicalDecodeClassification { ValidCurrentInput = 1, ValidInputMetadataRecoveryRequired = 2, ValidBindingRecoveryRequired = 3, UnsupportedProfileSchema = 4, Invalid = 5 }
    public enum ProfileCanonicalDecodeFailure { None = 0, MalformedJson = 1, SchemaViolation = 2, CanonicalOrIntegrityMismatch = 3 }
    [Flags] public enum ProfileInputRecoveryReason { None = 0, AssetIdMismatch = 1, BindingSchemaMismatch = 2, BindingOverridesMalformedOrNonCanonical = 4 }

    public readonly struct ProfileInputRecoveryProjectionV1
    {
        private readonly ProfileSettingsSnapshot _settings;
        private readonly ProfileTutorialSnapshot _tutorial;
        private readonly ProfileProgressionSnapshot _progression;
        public ProfileInputRecoveryProjectionV1(ProfileSettingsSnapshot settings, ProfileTutorialSnapshot tutorial, ProfileProgressionSnapshot progression)
        {
            try { settings.Validate(); tutorial.Validate(); progression.Validate(); }
            catch (InvalidOperationException e) { throw new ArgumentException("Recovery projection is malformed.", e); }
            _settings = settings; _tutorial = tutorial; _progression = progression;
        }
        public long SourceProfileRevision { get { Validate(); return _progression.ProfileRevision; } }
        public ProfileSettingsSnapshot Settings { get { Validate(); return _settings; } }
        public ProfileTutorialSnapshot Tutorial { get { Validate(); return _tutorial; } }
        public ProfileProgressionSnapshot Progression { get { Validate(); return _progression; } }
        public void Validate() { _settings.Validate(); _tutorial.Validate(); _progression.Validate(); }
    }

    public readonly struct ProfileCanonicalDecodeResultV1
    {
        private readonly ProfileCanonicalDecodeClassification _classification;
        private readonly ProfileCanonicalDocumentV1 _document;
        private readonly ProfileInputCompatibility _inputCompatibility;
        private readonly ProfileInputRecoveryProjectionV1 _recoveryProjection;
        private readonly ProfileInputRecoveryReason _inputRecoveryReasons;
        private readonly int _unsupportedSchemaVersion;
        private readonly ProfileCanonicalDecodeFailure _failure;

        internal ProfileCanonicalDecodeResultV1(ProfileCanonicalDecodeClassification classification, ProfileCanonicalDocumentV1 document,
            ProfileInputCompatibility compatibility, ProfileInputRecoveryProjectionV1 projection, ProfileInputRecoveryReason reasons,
            int unsupportedSchemaVersion, ProfileCanonicalDecodeFailure failure)
        { _classification = classification; _document = document; _inputCompatibility = compatibility; _recoveryProjection = projection; _inputRecoveryReasons = reasons; _unsupportedSchemaVersion = unsupportedSchemaVersion; _failure = failure; }

        public ProfileCanonicalDecodeClassification Classification { get { Validate(); return _classification; } }
        public ProfileCanonicalDocumentV1 Document { get { Validate(); if (_classification != ProfileCanonicalDecodeClassification.ValidCurrentInput && _classification != ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired) throw new InvalidOperationException("Document is unavailable."); return _document; } }
        public ProfileInputCompatibility InputCompatibility { get { Validate(); if (_classification != ProfileCanonicalDecodeClassification.ValidCurrentInput && _classification != ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired) throw new InvalidOperationException("Input compatibility is unavailable."); return _inputCompatibility; } }
        public ProfileInputRecoveryProjectionV1 RecoveryProjection { get { Validate(); if (_classification != ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired && _classification != ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired) throw new InvalidOperationException("Recovery projection is unavailable."); return _recoveryProjection; } }
        public ProfileInputRecoveryReason InputRecoveryReasons { get { Validate(); if (_classification != ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired && _classification != ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired) throw new InvalidOperationException("Input recovery reasons are unavailable."); return _inputRecoveryReasons; } }
        public int UnsupportedSchemaVersion { get { Validate(); if (_classification != ProfileCanonicalDecodeClassification.UnsupportedProfileSchema) throw new InvalidOperationException("Unsupported schema version is unavailable."); return _unsupportedSchemaVersion; } }
        public ProfileCanonicalDecodeFailure Failure { get { Validate(); if (_classification != ProfileCanonicalDecodeClassification.Invalid) throw new InvalidOperationException("Failure is unavailable."); return _failure; } }

        public void Validate()
        {
            if (!IsKnownClassification(_classification) || !IsKnownFailure(_failure) || !IsKnownReasons(_inputRecoveryReasons)) throw new InvalidOperationException("Decode result is malformed.");
            if (_classification == ProfileCanonicalDecodeClassification.ValidCurrentInput)
            {
                _document.Validate(); if (_unsupportedSchemaVersion != 0 || _inputCompatibility != ProfileInputCompatibility.Current || _document.Snapshot.Input.Compatibility != ProfileInputCompatibility.Current || _inputRecoveryReasons != ProfileInputRecoveryReason.None || _failure != ProfileCanonicalDecodeFailure.None || !IsDefault(_recoveryProjection)) throw new InvalidOperationException("Current result is malformed."); return;
            }
            if (_classification == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired)
            {
                _document.Validate(); _recoveryProjection.Validate(); if (_unsupportedSchemaVersion != 0 || _inputCompatibility == ProfileInputCompatibility.Current || _document.Snapshot.Input.Compatibility != _inputCompatibility || _failure != ProfileCanonicalDecodeFailure.None || _inputRecoveryReasons != ReasonsFromCompatibility(_inputCompatibility) || !ProjectionMatches(_recoveryProjection, _document.Snapshot)) throw new InvalidOperationException("Metadata recovery result is malformed."); return;
            }
            if (_classification == ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired)
            {
                _recoveryProjection.Validate(); if (_unsupportedSchemaVersion != 0 || _inputCompatibility != ProfileInputCompatibility.Current || _failure != ProfileCanonicalDecodeFailure.None || _inputRecoveryReasons == ProfileInputRecoveryReason.None || (_inputRecoveryReasons & ProfileInputRecoveryReason.BindingOverridesMalformedOrNonCanonical) == 0 || !IsDefault(_document)) throw new InvalidOperationException("Binding recovery result is malformed."); return;
            }
            if (_classification == ProfileCanonicalDecodeClassification.UnsupportedProfileSchema)
            {
                if (_unsupportedSchemaVersion == ProfileContract.SchemaVersion || _inputCompatibility != ProfileInputCompatibility.Current || _failure != ProfileCanonicalDecodeFailure.None || _inputRecoveryReasons != ProfileInputRecoveryReason.None || !IsDefault(_document) || !IsDefault(_recoveryProjection)) throw new InvalidOperationException("Unsupported result is malformed."); return;
            }
            if (_unsupportedSchemaVersion != 0 || _inputCompatibility != ProfileInputCompatibility.Current || _failure == ProfileCanonicalDecodeFailure.None || _inputRecoveryReasons != ProfileInputRecoveryReason.None || !IsDefault(_document) || !IsDefault(_recoveryProjection)) throw new InvalidOperationException("Invalid result is malformed.");
        }

        private static bool IsKnownClassification(ProfileCanonicalDecodeClassification value) => value >= ProfileCanonicalDecodeClassification.ValidCurrentInput && value <= ProfileCanonicalDecodeClassification.Invalid;
        private static bool IsKnownFailure(ProfileCanonicalDecodeFailure value) => value >= ProfileCanonicalDecodeFailure.None && value <= ProfileCanonicalDecodeFailure.CanonicalOrIntegrityMismatch;
        private static bool IsKnownReasons(ProfileInputRecoveryReason value) => (value & ~(ProfileInputRecoveryReason.AssetIdMismatch | ProfileInputRecoveryReason.BindingSchemaMismatch | ProfileInputRecoveryReason.BindingOverridesMalformedOrNonCanonical)) == 0;
        private static ProfileInputRecoveryReason ReasonsFromCompatibility(ProfileInputCompatibility value)
        {
            if (value == ProfileInputCompatibility.Current || (value & ~((ProfileInputCompatibility)3)) != 0) throw new InvalidOperationException("Input compatibility is malformed.");
            var result = ProfileInputRecoveryReason.None;
            if ((value & ProfileInputCompatibility.AssetIdMismatch) != 0) result |= ProfileInputRecoveryReason.AssetIdMismatch;
            if ((value & ProfileInputCompatibility.BindingSchemaMismatch) != 0) result |= ProfileInputRecoveryReason.BindingSchemaMismatch;
            return result;
        }
        private static bool IsDefault<T>(T value) where T : struct => value.Equals(default(T));
        private static bool ProjectionMatches(ProfileInputRecoveryProjectionV1 projection, ProfileSnapshotV1 snapshot)
        {
            if (projection.SourceProfileRevision != snapshot.ProfileRevision) return false;
            var leftSettings = projection.Settings; var rightSettings = snapshot.Settings;
            if (leftSettings.WindowMode != rightSettings.WindowMode || leftSettings.MasterVolumeQ1000 != rightSettings.MasterVolumeQ1000 || leftSettings.MusicVolumeQ1000 != rightSettings.MusicVolumeQ1000 || leftSettings.SfxVolumeQ1000 != rightSettings.SfxVolumeQ1000 || leftSettings.GamepadAimInvertX != rightSettings.GamepadAimInvertX || leftSettings.GamepadAimInvertY != rightSettings.GamepadAimInvertY) return false;
            var leftTutorial = projection.Tutorial.ConfirmedIds; var rightTutorial = snapshot.Tutorial.ConfirmedIds;
            if (!StringListsEqual(leftTutorial, rightTutorial)) return false;
            var leftProgression = projection.Progression; var rightProgression = snapshot.Progression;
            if (leftProgression.ProfileRevision != rightProgression.ProfileRevision || leftProgression.LastOfferedSeed != rightProgression.LastOfferedSeed || leftProgression.CommittedChoice != rightProgression.CommittedChoice || leftProgression.ConsentState != rightProgression.ConsentState || leftProgression.GrantedSkill != rightProgression.GrantedSkill) return false;
            return BranchListsEqual(leftProgression.CompletedBranches, rightProgression.CompletedBranches);
        }
        private static bool StringListsEqual(IReadOnlyList<string> left, IReadOnlyList<string> right) { if (left.Count != right.Count) return false; for (var i = 0; i < left.Count; i++) if (!string.Equals(left[i], right[i], StringComparison.Ordinal)) return false; return true; }
        private static bool BranchListsEqual(IReadOnlyList<ProfileBranch> left, IReadOnlyList<ProfileBranch> right) { if (left.Count != right.Count) return false; for (var i = 0; i < left.Count; i++) if (left[i] != right[i]) return false; return true; }
    }

    public static class ProfileCanonicalDecoderV1
    {
        private static readonly JsonDocumentOptions JsonOptions = new JsonDocumentOptions { AllowTrailingCommas = false, CommentHandling = JsonCommentHandling.Disallow, MaxDepth = 64 };
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

        public static ProfileCanonicalDecodeResultV1 Decode(byte[] canonicalFileBytes)
        {
            if (canonicalFileBytes == null) throw new ArgumentNullException(nameof(canonicalFileBytes));
            var source = (byte[])canonicalFileBytes.Clone();
            try { return DecodeCore(source); }
            catch (DecoderFallbackException) { return Invalid(ProfileCanonicalDecodeFailure.MalformedJson); }
            catch (JsonException) { return Invalid(ProfileCanonicalDecodeFailure.MalformedJson); }
            catch (OverflowException) { return Invalid(ProfileCanonicalDecodeFailure.SchemaViolation); }
            catch (FormatException) { return Invalid(ProfileCanonicalDecodeFailure.SchemaViolation); }
            catch (ArgumentException) { return Invalid(ProfileCanonicalDecodeFailure.SchemaViolation); }
            catch (InvalidOperationException) { return Invalid(ProfileCanonicalDecodeFailure.SchemaViolation); }
        }

        private static ProfileCanonicalDecodeResultV1 DecodeCore(byte[] source)
        {
            if (source.Length == 0 || HasBom(source)) return Invalid(ProfileCanonicalDecodeFailure.MalformedJson);
            var text = StrictUtf8.GetString(source);
            if (text.Length == 0 || char.IsWhiteSpace(text[0]) || char.IsWhiteSpace(text[text.Length - 1])) return Invalid(ProfileCanonicalDecodeFailure.MalformedJson);
            using (var json = JsonDocument.Parse(text, JsonOptions))
            {
                if (json.RootElement.ValueKind != JsonValueKind.Object) return Invalid(ProfileCanonicalDecodeFailure.MalformedJson);
                var schemaCount = 0; JsonElement schemaElement = default(JsonElement);
                foreach (var property in json.RootElement.EnumerateObject()) if (string.Equals(property.Name, "schemaVersion", StringComparison.Ordinal)) { schemaCount++; schemaElement = property.Value; }
                if (schemaCount != 1 || !TryInt32(schemaElement, out var schema)) return Invalid(ProfileCanonicalDecodeFailure.SchemaViolation);
                if (schema != ProfileContract.SchemaVersion) return Unsupported(schema);
                if (!TryParseV1(json.RootElement, out var parsed)) return Invalid(ProfileCanonicalDecodeFailure.SchemaViolation);
                if (!IsIntegrityShape(parsed.Integrity)) return Invalid(ProfileCanonicalDecodeFailure.CanonicalOrIntegrityMismatch);
                var rawPayload = WritePayload(parsed);
                var rawHash = Hash(rawPayload);
                var rawFile = StrictUtf8.GetBytes(rawPayload.Substring(0, rawPayload.Length - 1) + ",\"integrity\":{\"payloadSha256\":\"" + rawHash + "\"}}");
                if (!FixedTimeEquals(parsed.Integrity, rawHash) || !BytesEqual(rawFile, source)) return Invalid(ProfileCanonicalDecodeFailure.CanonicalOrIntegrityMismatch);
                ProfileBindingOverridesJson overrides;
                try { overrides = ProfileBindingOverridesJson.Parse(parsed.BindingRaw); if (!string.Equals(overrides.CanonicalText, parsed.BindingRaw, StringComparison.Ordinal)) throw new ArgumentException(); }
                catch (ArgumentException) { return BindingRecovery(parsed); }
                var input = new ProfileInputSnapshot(parsed.AssetId, parsed.BindingSchema, overrides);
                var snapshot = new ProfileSnapshotV1(parsed.Settings, input, parsed.Tutorial, parsed.Progression);
                var document = ProfileCanonicalEncoder.Encode(snapshot);
                if (!BytesEqual(document.CanonicalFileBytes, source)) return Invalid(ProfileCanonicalDecodeFailure.CanonicalOrIntegrityMismatch);
                var compatibility = input.Compatibility;
                if (compatibility == ProfileInputCompatibility.Current) return new ProfileCanonicalDecodeResultV1(ProfileCanonicalDecodeClassification.ValidCurrentInput, document, compatibility, default(ProfileInputRecoveryProjectionV1), ProfileInputRecoveryReason.None, 0, ProfileCanonicalDecodeFailure.None);
                var projection = new ProfileInputRecoveryProjectionV1(parsed.Settings, parsed.Tutorial, parsed.Progression);
                return new ProfileCanonicalDecodeResultV1(ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired, document, compatibility, projection, Reasons(compatibility), 0, ProfileCanonicalDecodeFailure.None);
            }
        }

        private static ProfileCanonicalDecodeResultV1 BindingRecovery(Parsed parsed)
        {
            var reasons = ReasonsForBinding(parsed.AssetId, parsed.BindingSchema);
            return new ProfileCanonicalDecodeResultV1(ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired, default(ProfileCanonicalDocumentV1), ProfileInputCompatibility.Current,
                new ProfileInputRecoveryProjectionV1(parsed.Settings, parsed.Tutorial, parsed.Progression), reasons, 0, ProfileCanonicalDecodeFailure.None);
        }
        private static ProfileInputRecoveryReason ReasonsForBinding(string id, int schema)
        {
            var result = ProfileInputRecoveryReason.BindingOverridesMalformedOrNonCanonical;
            if (!string.Equals(id, ProfileInputContract.CurrentInputActionsAssetId, StringComparison.Ordinal)) result |= ProfileInputRecoveryReason.AssetIdMismatch;
            if (schema != ProfileInputContract.CurrentBindingSchemaVersion) result |= ProfileInputRecoveryReason.BindingSchemaMismatch;
            return result;
        }
        private static ProfileInputRecoveryReason Reasons(ProfileInputCompatibility value)
        {
            var result = ProfileInputRecoveryReason.None;
            if ((value & ProfileInputCompatibility.AssetIdMismatch) != 0) result |= ProfileInputRecoveryReason.AssetIdMismatch;
            if ((value & ProfileInputCompatibility.BindingSchemaMismatch) != 0) result |= ProfileInputRecoveryReason.BindingSchemaMismatch;
            return result;
        }
        private static ProfileCanonicalDecodeResultV1 Invalid(ProfileCanonicalDecodeFailure failure) => new ProfileCanonicalDecodeResultV1(ProfileCanonicalDecodeClassification.Invalid, default(ProfileCanonicalDocumentV1), ProfileInputCompatibility.Current, default(ProfileInputRecoveryProjectionV1), ProfileInputRecoveryReason.None, 0, failure);
        private static ProfileCanonicalDecodeResultV1 Unsupported(int schema) => new ProfileCanonicalDecodeResultV1(ProfileCanonicalDecodeClassification.UnsupportedProfileSchema, default(ProfileCanonicalDocumentV1), ProfileInputCompatibility.Current, default(ProfileInputRecoveryProjectionV1), ProfileInputRecoveryReason.None, schema, ProfileCanonicalDecodeFailure.None);

        private static bool TryParseV1(JsonElement root, out Parsed parsed)
        {
            parsed = default(Parsed);
            var outer = new[] { "schemaVersion", "profileRevision", "settings", "input", "tutorial", "progression", "integrity" };
            if (!ExactProperties(root, outer, out var values)) return false;
            if (!TryPositiveInt(values[0], out var schema) || schema != 1 || !TryNonnegativeLong(values[1], out var revision)) return false;
            if (!TrySettings(values[2], out var settings) || !TryInputRaw(values[3], out var assetId, out var bindingSchema, out var bindingRaw) || !TryTutorial(values[4], out var tutorial) || !TryProgression(values[5], revision, out var progression) || !TryIntegrity(values[6], out var integrity)) return false;
            parsed = new Parsed(settings, tutorial, progression, assetId, bindingSchema, bindingRaw, integrity); return true;
        }
        private static bool TrySettings(JsonElement element, out ProfileSettingsSnapshot settings)
        {
            settings = default(ProfileSettingsSnapshot); var names = new[] { "windowMode", "masterVolumeQ1000", "musicVolumeQ1000", "sfxVolumeQ1000", "gamepadAimInvertX", "gamepadAimInvertY" };
            if (!ExactProperties(element, names, out var v) || !TryString(v[0], out var modeText) || !TryInt32(v[1], out var master) || !TryInt32(v[2], out var music) || !TryInt32(v[3], out var sfx) || v[4].ValueKind != JsonValueKind.True && v[4].ValueKind != JsonValueKind.False || v[5].ValueKind != JsonValueKind.True && v[5].ValueKind != JsonValueKind.False) return false;
            var mode = modeText == "Windowed" ? ProfileWindowMode.Windowed : modeText == "BorderlessFullscreen" ? ProfileWindowMode.BorderlessFullscreen : (ProfileWindowMode)0;
            try { settings = new ProfileSettingsSnapshot(mode, master, music, sfx, v[4].GetBoolean(), v[5].GetBoolean()); return true; } catch (ArgumentException) { return false; }
        }
        private static bool TryInputRaw(JsonElement element, out string id, out int schema, out string binding)
        {
            id = null; schema = 0; binding = null; var names = new[] { "inputActionsAssetId", "bindingSchemaVersion", "bindingOverridesJson" };
            if (!ExactProperties(element, names, out var v) || !TryString(v[0], out id) || !TryNonnegativeInt(v[1], out schema) || !TryString(v[2], out binding) || !SafeNfc(binding)) return false;
            try { new ProfileInputSnapshot(id, schema, ProfileBindingOverridesJson.Parse(string.Empty)); return true; } catch (ArgumentException) { return false; }
        }
        private static bool TryTutorial(JsonElement element, out ProfileTutorialSnapshot tutorial)
        {
            tutorial = default(ProfileTutorialSnapshot); if (!ExactProperties(element, new[] { "confirmedIds" }, out var v) || v[0].ValueKind != JsonValueKind.Array) return false;
            var ids = new List<string>(); foreach (var item in v[0].EnumerateArray()) { if (!TryString(item, out var id)) return false; ids.Add(id); }
            try { tutorial = new ProfileTutorialSnapshot(ids); return true; } catch (ArgumentException) { return false; }
        }
        private static bool TryProgression(JsonElement element, long revision, out ProfileProgressionSnapshot progression)
        {
            progression = default(ProfileProgressionSnapshot); var names = new[] { "lastOfferedSeed", "committedChoice", "consentState", "grantedSkill", "completedBranches" };
            if (!ExactProperties(element, names, out var v) || !TryNullableInt(v[0], out var seed) || !TryNullableChoice(v[1], out var choice) || !TryString(v[2], out var consentText) || !TryNullableSkill(v[3], out var skill) || v[4].ValueKind != JsonValueKind.Array) return false;
            var consent = Consent(consentText); if ((int)consent == 0) return false; var branches = new List<ProfileBranch>(); foreach (var item in v[4].EnumerateArray()) { if (!TryString(item, out var text)) return false; var branch = text == "Extraction" ? ProfileBranch.Extraction : text == "Solidarity" ? ProfileBranch.Solidarity : (ProfileBranch)0; if ((int)branch == 0) return false; branches.Add(branch); }
            try { progression = new ProfileProgressionSnapshot(revision, seed, choice, consent, skill, branches); return true; } catch (ArgumentException) { return false; }
        }
        private static bool TryIntegrity(JsonElement element, out string integrity) { integrity = null; return ExactProperties(element, new[] { "payloadSha256" }, out var v) && TryString(v[0], out integrity); }
        private static bool ExactProperties(JsonElement element, string[] names, out JsonElement[] values)
        {
            values = null; if (element.ValueKind != JsonValueKind.Object) return false; var collected = new List<JsonProperty>(); foreach (var p in element.EnumerateObject()) collected.Add(p); if (collected.Count != names.Length) return false;
            values = new JsonElement[names.Length]; for (var i = 0; i < names.Length; i++) { if (!string.Equals(collected[i].Name, names[i], StringComparison.Ordinal)) return false; values[i] = collected[i].Value; } return true;
        }
        private static bool TryString(JsonElement element, out string value) { value = null; if (element.ValueKind != JsonValueKind.String || HasUnpairedSurrogateEscape(element.GetRawText())) return false; value = element.GetString(); return SafeNfc(value); }
        private static bool HasUnpairedSurrogateEscape(string raw)
        {
            // Advance by complete JSON escapes. In particular, raw `\\uD800` is an escaped backslash plus literal text, not a Unicode escape.
            var index = 1;
            while (index < raw.Length - 1)
            {
                if (raw[index] != '\\') { index++; continue; }
                if (index + 1 >= raw.Length - 1) return true;
                if (raw[index + 1] != 'u') { index += 2; continue; }
                if (index + 5 >= raw.Length - 1) return true;
                var code = Hex(raw[index + 2]) * 4096 + Hex(raw[index + 3]) * 256 + Hex(raw[index + 4]) * 16 + Hex(raw[index + 5]);
                if (code < 0) return true;
                if (code >= 0xd800 && code <= 0xdbff)
                {
                    var next = index + 6;
                    if (next + 5 >= raw.Length - 1 || raw[next] != '\\' || raw[next + 1] != 'u') return true;
                    var low = Hex(raw[next + 2]) * 4096 + Hex(raw[next + 3]) * 256 + Hex(raw[next + 4]) * 16 + Hex(raw[next + 5]);
                    if (low < 0xdc00 || low > 0xdfff) return true;
                    index = next + 6;
                }
                else
                {
                    if (code >= 0xdc00 && code <= 0xdfff) return true;
                    index += 6;
                }
            }
            return false;
        }
        private static int Hex(char value) { if (value >= '0' && value <= '9') return value - '0'; if (value >= 'a' && value <= 'f') return value - 'a' + 10; if (value >= 'A' && value <= 'F') return value - 'A' + 10; return -100000; }
        private static bool SafeNfc(string value) { if (value == null) return false; for (var i = 0; i < value.Length; i++) { if (char.IsHighSurrogate(value[i])) { if (i + 1 >= value.Length || !char.IsLowSurrogate(value[i + 1])) return false; i++; } else if (char.IsLowSurrogate(value[i])) return false; } return string.Equals(value, value.Normalize(NormalizationForm.FormC), StringComparison.Ordinal); }
        private static bool TryInt32(JsonElement element, out int value) { value = 0; return IsIntegerToken(element) && int.TryParse(element.GetRawText(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value); }
        private static bool TryNonnegativeInt(JsonElement element, out int value) { return TryInt32(element, out value) && value >= 0; }
        private static bool TryNonnegativeLong(JsonElement element, out long value) { value = 0; return IsIntegerToken(element) && long.TryParse(element.GetRawText(), NumberStyles.None, CultureInfo.InvariantCulture, out value); }
        private static bool TryPositiveInt(JsonElement element, out int value) { return TryInt32(element, out value) && value >= 0; }
        private static bool IsIntegerToken(JsonElement element) { if (element.ValueKind != JsonValueKind.Number) return false; var raw = element.GetRawText(); if (raw.Length == 0) return false; var start = raw[0] == '-' ? 1 : 0; if (start == raw.Length || (raw.Length - start > 1 && raw[start] == '0')) return false; for (var i = start; i < raw.Length; i++) if (raw[i] < '0' || raw[i] > '9') return false; return true; }
        private static bool TryNullableInt(JsonElement e, out int? value) { value = null; if (e.ValueKind == JsonValueKind.Null) return true; if (!TryInt32(e, out var x)) return false; value = x; return true; }
        private static bool TryNullableChoice(JsonElement e, out ProfileChoice? value) { value = null; if (e.ValueKind == JsonValueKind.Null) return true; if (!TryString(e, out var x)) return false; if (x == "Extraction") value = ProfileChoice.Extraction; else if (x == "Solidarity") value = ProfileChoice.Solidarity; else return false; return true; }
        private static bool TryNullableSkill(JsonElement e, out ProfileSkill? value) { value = null; if (e.ValueKind == JsonValueKind.Null) return true; if (!TryString(e, out var x)) return false; if (x == "CompressionVerdict") value = ProfileSkill.CompressionVerdict; else if (x == "CommonReferencePlane") value = ProfileSkill.CommonReferencePlane; else return false; return true; }
        private static ProfileConsentState Consent(string value) { if (value == "RefusesOwnershipTransfer") return ProfileConsentState.RefusesOwnershipTransfer; if (value == "OffersScopedResonance") return ProfileConsentState.OffersScopedResonance; if (value == "ResonanceLender") return ProfileConsentState.ResonanceLender; if (value == "ImprintSevered") return ProfileConsentState.ImprintSevered; return (ProfileConsentState)0; }
        private static bool IsIntegrityShape(string value) { if (value == null || value.Length != 64) return false; for (var i = 0; i < value.Length; i++) if (!((value[i] >= '0' && value[i] <= '9') || (value[i] >= 'a' && value[i] <= 'f'))) return false; return true; }
        private static bool HasBom(byte[] bytes) => bytes.Length >= 3 && bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf;
        private static bool BytesEqual(byte[] a, byte[] b) { if (a.Length != b.Length) return false; for (var i = 0; i < a.Length; i++) if (a[i] != b[i]) return false; return true; }
        private static bool FixedTimeEquals(string a, string b) { var x = StrictUtf8.GetBytes(a); var y = StrictUtf8.GetBytes(b); return CryptographicOperations.FixedTimeEquals(x, y); }
        private static string Hash(string payload) { using (var sha = SHA256.Create()) { var hash = sha.ComputeHash(StrictUtf8.GetBytes(payload)); var b = new StringBuilder(64); for (var i = 0; i < hash.Length; i++) b.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture)); return b.ToString(); } }

        private static string WritePayload(Parsed p)
        {
            var s = p.Settings; var pr = p.Progression; var b = new StringBuilder(); b.Append("{\"schemaVersion\":1,\"profileRevision\":").Append(pr.ProfileRevision.ToString(CultureInfo.InvariantCulture)); b.Append(",\"settings\":{\"windowMode\":"); Q(b, s.WindowMode == ProfileWindowMode.Windowed ? "Windowed" : "BorderlessFullscreen"); b.Append(",\"masterVolumeQ1000\":").Append(s.MasterVolumeQ1000.ToString(CultureInfo.InvariantCulture)).Append(",\"musicVolumeQ1000\":").Append(s.MusicVolumeQ1000.ToString(CultureInfo.InvariantCulture)).Append(",\"sfxVolumeQ1000\":").Append(s.SfxVolumeQ1000.ToString(CultureInfo.InvariantCulture)).Append(",\"gamepadAimInvertX\":").Append(s.GamepadAimInvertX ? "true" : "false").Append(",\"gamepadAimInvertY\":").Append(s.GamepadAimInvertY ? "true" : "false"); b.Append("},\"input\":{\"inputActionsAssetId\":"); Q(b, p.AssetId); b.Append(",\"bindingSchemaVersion\":").Append(p.BindingSchema.ToString(CultureInfo.InvariantCulture)).Append(",\"bindingOverridesJson\":"); Q(b, p.BindingRaw); b.Append("},\"tutorial\":{\"confirmedIds\":["); var ids = p.Tutorial.ConfirmedIds; for (var i = 0; i < ids.Count; i++) { if (i != 0) b.Append(','); Q(b, ids[i]); } b.Append("]},\"progression\":{\"lastOfferedSeed\":"); if (pr.LastOfferedSeed.HasValue) b.Append(pr.LastOfferedSeed.Value.ToString(CultureInfo.InvariantCulture)); else b.Append("null"); b.Append(",\"committedChoice\":"); if (pr.CommittedChoice.HasValue) Q(b, pr.CommittedChoice.Value == ProfileChoice.Extraction ? "Extraction" : "Solidarity"); else b.Append("null"); b.Append(",\"consentState\":"); Q(b, ConsentText(pr.ConsentState)); b.Append(",\"grantedSkill\":"); if (pr.GrantedSkill.HasValue) Q(b, pr.GrantedSkill.Value == ProfileSkill.CompressionVerdict ? "CompressionVerdict" : "CommonReferencePlane"); else b.Append("null"); b.Append(",\"completedBranches\":["); var branches = pr.CompletedBranches; for (var i = 0; i < branches.Count; i++) { if (i != 0) b.Append(','); Q(b, branches[i] == ProfileBranch.Extraction ? "Extraction" : "Solidarity"); } b.Append("]}}"); return b.ToString();
        }
        private static string ConsentText(ProfileConsentState c) { if (c == ProfileConsentState.RefusesOwnershipTransfer) return "RefusesOwnershipTransfer"; if (c == ProfileConsentState.OffersScopedResonance) return "OffersScopedResonance"; if (c == ProfileConsentState.ResonanceLender) return "ResonanceLender"; return "ImprintSevered"; }
        private static void Q(StringBuilder b, string value) { b.Append('"'); for (var i = 0; i < value.Length; i++) { var c = value[i]; if (c == '"') b.Append("\\\""); else if (c == '\\') b.Append("\\\\"); else if (c == '\b') b.Append("\\b"); else if (c == '\t') b.Append("\\t"); else if (c == '\n') b.Append("\\n"); else if (c == '\f') b.Append("\\f"); else if (c == '\r') b.Append("\\r"); else if (c <= 0x1f) b.Append("\\u00").Append(((int)c).ToString("x2", CultureInfo.InvariantCulture)); else b.Append(c); } b.Append('"'); }
        private readonly struct Parsed { internal readonly ProfileSettingsSnapshot Settings; internal readonly ProfileTutorialSnapshot Tutorial; internal readonly ProfileProgressionSnapshot Progression; internal readonly string AssetId; internal readonly int BindingSchema; internal readonly string BindingRaw; internal readonly string Integrity; internal Parsed(ProfileSettingsSnapshot s, ProfileTutorialSnapshot t, ProfileProgressionSnapshot p, string id, int schema, string binding, string integrity) { Settings=s; Tutorial=t; Progression=p; AssetId=id; BindingSchema=schema; BindingRaw=binding; Integrity=integrity; } }
    }
}
