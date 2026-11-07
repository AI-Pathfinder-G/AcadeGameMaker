using System;
using System.Collections.Generic;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D7C-001..005; REQ-M5D7J-001..007. Engine-free profile recovery transformation only; caller owns primary/previous selection and all IO.
    [Flags]
    public enum ProfileRecoveryPlanReason { DefaultBootstrap = 1, PreviousPromotion = 2, InputRepair = 4 }

    public readonly struct ProfileRecoveryPlanV1
    {
        private readonly long _sourceRevision;
        private readonly long _resultRevision;
        private readonly ProfileRecoveryPlanReason _reasons;
        private readonly ProfileCanonicalDocumentV1 _resultDocument;
        private readonly int _proofKind;
        private readonly ProfileSettingsSnapshot _proofSettings;
        private readonly ProfileTutorialSnapshot _proofTutorial;
        private readonly ProfileProgressionSnapshot _proofProgression;
        private readonly ProfileInputSnapshot _proofInput;
        private readonly string _proofBindingOverride;

        internal ProfileRecoveryPlanV1(long sourceRevision, long resultRevision, ProfileRecoveryPlanReason reasons, ProfileCanonicalDocumentV1 resultDocument,
            int proofKind, ProfileSettingsSnapshot settings, ProfileTutorialSnapshot tutorial, ProfileProgressionSnapshot progression, ProfileInputSnapshot input, string proofBindingOverride)
        { _sourceRevision = sourceRevision; _resultRevision = resultRevision; _reasons = reasons; _resultDocument = resultDocument; _proofKind = proofKind; _proofSettings = settings; _proofTutorial = tutorial; _proofProgression = progression; _proofInput = input; _proofBindingOverride = proofBindingOverride; }

        public long SourceRevision { get { Validate(); return _sourceRevision; } }
        public long ResultRevision { get { Validate(); return _resultRevision; } }
        public ProfileRecoveryPlanReason Reasons { get { Validate(); return _reasons; } }
        public ProfileCanonicalDocumentV1 ResultDocument { get { Validate(); return _resultDocument; } }

        public void Validate()
        {
            if (!IsAllowedReason(_reasons) || _resultRevision < 0) throw new InvalidOperationException("Recovery plan is malformed.");
            _resultDocument.Validate(); var output = _resultDocument.Snapshot;
            if (!output.Input.IsCurrentCompatible || output.ProfileRevision != _resultRevision) throw new InvalidOperationException("Recovery plan result is malformed.");
            if (_reasons == ProfileRecoveryPlanReason.DefaultBootstrap)
            {
                if (_sourceRevision != -1 || _resultRevision != 0 || _proofKind != 1 || _proofBindingOverride != null || !IsDefault(_proofSettings) || !IsDefault(_proofTutorial) || !IsDefault(_proofProgression) || !IsDefault(_proofInput) || !IsApprovedDefault(output)) throw new InvalidOperationException("Default recovery plan is malformed.");
                return;
            }
            if (_sourceRevision < 0 || _sourceRevision == long.MaxValue || _resultRevision != _sourceRevision + 1) throw new InvalidOperationException("Recovery revision relation is malformed.");
            if ((_reasons == ProfileRecoveryPlanReason.InputRepair && _proofKind != 2 && _proofKind != 5) || (_reasons == ProfileRecoveryPlanReason.PreviousPromotion && _proofKind != 3) || (_reasons == (ProfileRecoveryPlanReason.PreviousPromotion | ProfileRecoveryPlanReason.InputRepair) && _proofKind != 4 && _proofKind != 6)) throw new InvalidOperationException("Recovery plan kind is malformed.");
            _proofSettings.Validate(); _proofTutorial.Validate(); _proofProgression.Validate();
            if (_proofProgression.ProfileRevision != _sourceRevision || !SettingsEqual(_proofSettings, output.Settings) || !StringsEqual(_proofTutorial.ConfirmedIds, output.Tutorial.ConfirmedIds) || !ProgressionSemanticEqual(_proofProgression, output.Progression)) throw new InvalidOperationException("Recovery semantic preservation is malformed.");
            if (_proofKind == 3)
            {
                _proofInput.Validate(); if (_proofBindingOverride != null || !InputEqual(_proofInput, output.Input)) throw new InvalidOperationException("Previous current input preservation is malformed.");
            }
            else if (_proofKind == 5 || _proofKind == 6)
            {
                _proofInput.Validate();
                if (_proofBindingOverride == null || !IsCurrentNonEmptyInput(_proofInput) || !string.Equals(_proofInput.BindingOverrides.CanonicalText, _proofBindingOverride, StringComparison.Ordinal) || !IsCurrentDefaultInput(output.Input)) throw new InvalidOperationException("Binding apply-failure recovery input is malformed.");
            }
            else if (_proofBindingOverride != null || !IsDefault(_proofInput) || !IsCurrentDefaultInput(output.Input)) throw new InvalidOperationException("Recovery input repair is malformed.");
        }

        private static bool IsAllowedReason(ProfileRecoveryPlanReason value) => value == ProfileRecoveryPlanReason.DefaultBootstrap || value == ProfileRecoveryPlanReason.InputRepair || value == ProfileRecoveryPlanReason.PreviousPromotion || value == (ProfileRecoveryPlanReason.PreviousPromotion | ProfileRecoveryPlanReason.InputRepair);
        private static bool IsDefault<T>(T value) where T : struct => value.Equals(default(T));
        private static bool IsApprovedDefault(ProfileSnapshotV1 value)
        {
            return value.Settings.WindowMode == ProfileWindowMode.BorderlessFullscreen && value.Settings.MasterVolumeQ1000 == 1000 && value.Settings.MusicVolumeQ1000 == 700 && value.Settings.SfxVolumeQ1000 == 800 && !value.Settings.GamepadAimInvertX && !value.Settings.GamepadAimInvertY && IsCurrentDefaultInput(value.Input) && value.Tutorial.ConfirmedIds.Count == 0 && value.Progression.ProfileRevision == 0 && !value.Progression.LastOfferedSeed.HasValue && !value.Progression.CommittedChoice.HasValue && value.Progression.ConsentState == ProfileConsentState.RefusesOwnershipTransfer && !value.Progression.GrantedSkill.HasValue && value.Progression.CompletedBranches.Count == 0;
        }
        private static bool IsCurrentDefaultInput(ProfileInputSnapshot input) => input.InputActionsAssetId == ProfileInputContract.CurrentInputActionsAssetId && input.BindingSchemaVersion == ProfileInputContract.CurrentBindingSchemaVersion && input.BindingOverrides.IsEmpty;
        private static bool IsCurrentNonEmptyInput(ProfileInputSnapshot input) => input.InputActionsAssetId == ProfileInputContract.CurrentInputActionsAssetId && input.BindingSchemaVersion == ProfileInputContract.CurrentBindingSchemaVersion && !input.BindingOverrides.IsEmpty;
        private static bool SettingsEqual(ProfileSettingsSnapshot a, ProfileSettingsSnapshot b) => a.WindowMode == b.WindowMode && a.MasterVolumeQ1000 == b.MasterVolumeQ1000 && a.MusicVolumeQ1000 == b.MusicVolumeQ1000 && a.SfxVolumeQ1000 == b.SfxVolumeQ1000 && a.GamepadAimInvertX == b.GamepadAimInvertX && a.GamepadAimInvertY == b.GamepadAimInvertY;
        private static bool InputEqual(ProfileInputSnapshot a, ProfileInputSnapshot b) => a.InputActionsAssetId == b.InputActionsAssetId && a.BindingSchemaVersion == b.BindingSchemaVersion && a.BindingOverrides.CanonicalText == b.BindingOverrides.CanonicalText;
        private static bool StringsEqual(IReadOnlyList<string> a, IReadOnlyList<string> b) { if (a.Count != b.Count) return false; for (var i = 0; i < a.Count; i++) if (!string.Equals(a[i], b[i], StringComparison.Ordinal)) return false; return true; }
        private static bool ProgressionSemanticEqual(ProfileProgressionSnapshot a, ProfileProgressionSnapshot b) { return a.LastOfferedSeed == b.LastOfferedSeed && a.CommittedChoice == b.CommittedChoice && a.ConsentState == b.ConsentState && a.GrantedSkill == b.GrantedSkill && BranchesEqual(a.CompletedBranches, b.CompletedBranches); }
        private static bool BranchesEqual(IReadOnlyList<ProfileBranch> a, IReadOnlyList<ProfileBranch> b) { if (a.Count != b.Count) return false; for (var i = 0; i < a.Count; i++) if (a[i] != b[i]) return false; return true; }
    }

    public static class ProfileRecoveryPlannerV1
    {
        public static ProfileRecoveryPlanV1 PlanDefaultBootstrap()
        {
            var settings = new ProfileSettingsSnapshot(ProfileWindowMode.BorderlessFullscreen, 1000, 700, 800, false, false);
            var tutorial = new ProfileTutorialSnapshot(Array.Empty<string>());
            var progression = new ProfileProgressionSnapshot(0, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>());
            var input = CurrentDefaultInput(); var document = ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(settings, input, tutorial, progression));
            return new ProfileRecoveryPlanV1(-1, 0, ProfileRecoveryPlanReason.DefaultBootstrap, document, 1, default(ProfileSettingsSnapshot), default(ProfileTutorialSnapshot), default(ProfileProgressionSnapshot), default(ProfileInputSnapshot), null);
        }

        public static ProfileRecoveryPlanV1 PlanInputRepair(ProfileCanonicalDecodeResultV1 source) => Plan(source, false);
        public static ProfileRecoveryPlanV1 PlanPreviousPromotion(ProfileCanonicalDecodeResultV1 source) => Plan(source, true);
        public static ProfileRecoveryPlanV1 PlanPrimaryBindingApplyFailure(ProfileCanonicalDecodeResultV1 source) => PlanBindingApplyFailure(source, false);
        public static ProfileRecoveryPlanV1 PlanPreviousBindingApplyFailure(ProfileCanonicalDecodeResultV1 source) => PlanBindingApplyFailure(source, true);

        private static ProfileRecoveryPlanV1 Plan(ProfileCanonicalDecodeResultV1 source, bool previous)
        {
            try { source.Validate(); }
            catch (InvalidOperationException e) { throw new ArgumentException("Source decode result is malformed.", nameof(source), e); }
            var classification = source.Classification;
            var current = classification == ProfileCanonicalDecodeClassification.ValidCurrentInput;
            var recovery = classification == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired || classification == ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired;
            if ((!previous && !recovery) || (previous && !current && !recovery)) throw new ArgumentException("Source classification is not applicable.", nameof(source));
            ProfileSettingsSnapshot settings; ProfileTutorialSnapshot tutorial; ProfileProgressionSnapshot progression; ProfileInputSnapshot input; var preserveInput = previous && current;
            if (current)
            {
                var snapshot = source.Document.Snapshot; settings = snapshot.Settings; tutorial = snapshot.Tutorial; progression = snapshot.Progression; input = snapshot.Input;
            }
            else
            {
                var projection = source.RecoveryProjection; settings = projection.Settings; tutorial = projection.Tutorial; progression = projection.Progression; input = CurrentDefaultInput();
            }
            var revision = progression.ProfileRevision;
            if (revision == long.MaxValue) throw new InvalidOperationException("Source revision cannot be incremented.");
            var resultProgression = WithRevision(progression, revision + 1);
            var document = ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(settings, input, tutorial, resultProgression));
            var reason = previous ? (preserveInput ? ProfileRecoveryPlanReason.PreviousPromotion : ProfileRecoveryPlanReason.PreviousPromotion | ProfileRecoveryPlanReason.InputRepair) : ProfileRecoveryPlanReason.InputRepair;
            return new ProfileRecoveryPlanV1(revision, revision + 1, reason, document, preserveInput ? 3 : previous ? 4 : 2, settings, tutorial, progression, preserveInput ? input : default(ProfileInputSnapshot), null);
        }

        private static ProfileRecoveryPlanV1 PlanBindingApplyFailure(ProfileCanonicalDecodeResultV1 source, bool previous)
        {
            try { source.Validate(); }
            catch (InvalidOperationException e) { throw new ArgumentException("Source decode result is malformed.", nameof(source), e); }
            if (source.Classification != ProfileCanonicalDecodeClassification.ValidCurrentInput) throw new ArgumentException("Source classification is not applicable.", nameof(source));
            var snapshot = source.Document.Snapshot;
            var input = snapshot.Input;
            if (input.BindingOverrides.IsEmpty) throw new ArgumentException("Source input override is not applicable.", nameof(source));
            var revision = snapshot.Progression.ProfileRevision;
            if (revision == long.MaxValue) throw new InvalidOperationException("Source revision cannot be incremented.");
            var progression = WithRevision(snapshot.Progression, revision + 1);
            var document = ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(snapshot.Settings, CurrentDefaultInput(), snapshot.Tutorial, progression));
            var reason = previous ? ProfileRecoveryPlanReason.PreviousPromotion | ProfileRecoveryPlanReason.InputRepair : ProfileRecoveryPlanReason.InputRepair;
            return new ProfileRecoveryPlanV1(revision, revision + 1, reason, document, previous ? 6 : 5, snapshot.Settings, snapshot.Tutorial, snapshot.Progression, input, input.BindingOverrides.CanonicalText);
        }

        private static ProfileInputSnapshot CurrentDefaultInput() => new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, ProfileInputContract.CurrentBindingSchemaVersion, ProfileBindingOverridesJson.Parse(string.Empty));
        private static ProfileProgressionSnapshot WithRevision(ProfileProgressionSnapshot value, long revision) => new ProfileProgressionSnapshot(revision, value.LastOfferedSeed, value.CommittedChoice, value.ConsentState, value.GrantedSkill, value.CompletedBranches);
    }
}
