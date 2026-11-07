using System;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D7E-001..005. Engine-free selection only: observed candidates in, immutable load plan out.
    public enum ProfileLoadFileRole { Primary = 1, Previous = 2, Temp = 3 }
    public enum ProfileLoadCandidateKind { Missing = 1, Unreadable = 2, Decoded = 3 }
    public enum ProfileLaunchSource { Primary = 1, Previous = 2, Default = 3 }
    public enum ProfileFilePreservationReason { None = 0, StaleTemp = 1, InvalidPrimary = 2, UnsupportedPrimary = 3, UnreadablePrimary = 4, InvalidPrevious = 5, UnsupportedPrevious = 6, UnreadablePrevious = 7 }

    public readonly struct ProfileLoadCandidateV1
    {
        private readonly ProfileLoadFileRole _role;
        private readonly ProfileLoadCandidateKind _kind;
        private readonly ProfileCanonicalDecodeResultV1 _decodeResult;

        private ProfileLoadCandidateV1(ProfileLoadFileRole role, ProfileLoadCandidateKind kind, ProfileCanonicalDecodeResultV1 decodeResult)
        { _role = role; _kind = kind; _decodeResult = decodeResult; }

        public static ProfileLoadCandidateV1 Missing(ProfileLoadFileRole role) { ValidateRoleArgument(role); return new ProfileLoadCandidateV1(role, ProfileLoadCandidateKind.Missing, default(ProfileCanonicalDecodeResultV1)); }
        public static ProfileLoadCandidateV1 Unreadable(ProfileLoadFileRole role) { ValidateRoleArgument(role); return new ProfileLoadCandidateV1(role, ProfileLoadCandidateKind.Unreadable, default(ProfileCanonicalDecodeResultV1)); }
        public static ProfileLoadCandidateV1 Decoded(ProfileLoadFileRole role, ProfileCanonicalDecodeResultV1 decodeResult)
        {
            ValidateRoleArgument(role);
            try { decodeResult.Validate(); } catch (InvalidOperationException exception) { throw new ArgumentException("Decode result is malformed.", nameof(decodeResult), exception); }
            return new ProfileLoadCandidateV1(role, ProfileLoadCandidateKind.Decoded, decodeResult);
        }

        public ProfileLoadFileRole Role { get { Validate(); return _role; } }
        public ProfileLoadCandidateKind Kind { get { Validate(); return _kind; } }
        public ProfileCanonicalDecodeResultV1 DecodeResult { get { Validate(); if (_kind != ProfileLoadCandidateKind.Decoded) throw new InvalidOperationException("Decode result is unavailable."); return _decodeResult; } }

        public void Validate()
        {
            if (!KnownRole(_role) || !KnownKind(_kind)) throw new InvalidOperationException("Load candidate is malformed.");
            if (_kind == ProfileLoadCandidateKind.Missing || _kind == ProfileLoadCandidateKind.Unreadable)
            {
                if (!IsDefault(_decodeResult)) throw new InvalidOperationException("Non-decoded candidate has a decode payload.");
                return;
            }
            _decodeResult.Validate();
            if (!KnownDecodeClassification(_decodeResult.Classification)) throw new InvalidOperationException("Decoded candidate classification is malformed.");
        }

        private static void ValidateRoleArgument(ProfileLoadFileRole role) { if (!KnownRole(role)) throw new ArgumentOutOfRangeException(nameof(role)); }
        internal static bool KnownRole(ProfileLoadFileRole value) => value == ProfileLoadFileRole.Primary || value == ProfileLoadFileRole.Previous || value == ProfileLoadFileRole.Temp;
        internal static bool KnownKind(ProfileLoadCandidateKind value) => value == ProfileLoadCandidateKind.Missing || value == ProfileLoadCandidateKind.Unreadable || value == ProfileLoadCandidateKind.Decoded;
        internal static bool KnownDecodeClassification(ProfileCanonicalDecodeClassification value) => value == ProfileCanonicalDecodeClassification.ValidCurrentInput || value == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired || value == ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired || value == ProfileCanonicalDecodeClassification.UnsupportedProfileSchema || value == ProfileCanonicalDecodeClassification.Invalid;
        internal static bool IsDefault<T>(T value) where T : struct => value.Equals(default(T));
    }

    public readonly struct ProfileLoadSelectionPlanV1
    {
        private readonly ProfileLaunchSource _source;
        private readonly long _sourceRevision;
        private readonly long _resultRevision;
        private readonly ProfileCanonicalDocumentV1 _inMemoryDocument;
        private readonly bool _requiresAtomicSave;
        private readonly ProfileRecoveryPlanReason _recoveryReasons;
        private readonly ProfileFilePreservationReason _primaryPreservation;
        private readonly ProfileFilePreservationReason _previousPreservation;
        private readonly ProfileFilePreservationReason _tempPreservation;
        private readonly int _proofKind;
        private readonly ProfileLoadCandidateV1 _primaryProof;
        private readonly ProfileLoadCandidateV1 _previousProof;
        private readonly ProfileLoadCandidateV1 _tempProof;
        private readonly ProfileRecoveryPlanV1 _recoveryProof;
        private readonly ProfileCanonicalDocumentV1 _directPrimaryProof;

        internal ProfileLoadSelectionPlanV1(ProfileLaunchSource source, long sourceRevision, long resultRevision, ProfileCanonicalDocumentV1 document,
            bool requiresAtomicSave, ProfileRecoveryPlanReason recoveryReasons, ProfileFilePreservationReason primaryPreservation,
            ProfileFilePreservationReason previousPreservation, ProfileFilePreservationReason tempPreservation, int proofKind,
            ProfileLoadCandidateV1 primary, ProfileLoadCandidateV1 previous, ProfileLoadCandidateV1 temp,
            ProfileRecoveryPlanV1 recoveryProof, ProfileCanonicalDocumentV1 directPrimaryProof)
        {
            _source = source; _sourceRevision = sourceRevision; _resultRevision = resultRevision; _inMemoryDocument = document;
            _requiresAtomicSave = requiresAtomicSave; _recoveryReasons = recoveryReasons; _primaryPreservation = primaryPreservation;
            _previousPreservation = previousPreservation; _tempPreservation = tempPreservation; _proofKind = proofKind;
            _primaryProof = primary; _previousProof = previous; _tempProof = temp; _recoveryProof = recoveryProof; _directPrimaryProof = directPrimaryProof;
        }

        public ProfileLaunchSource Source { get { Validate(); return _source; } }
        public long SourceRevision { get { Validate(); return _sourceRevision; } }
        public long ResultRevision { get { Validate(); return _resultRevision; } }
        public ProfileCanonicalDocumentV1 InMemoryDocument { get { Validate(); return _inMemoryDocument; } }
        public bool RequiresAtomicSave { get { Validate(); return _requiresAtomicSave; } }
        public ProfileRecoveryPlanReason RecoveryReasons { get { Validate(); return _recoveryReasons; } }
        public ProfileFilePreservationReason PrimaryPreservation { get { Validate(); return _primaryPreservation; } }
        public ProfileFilePreservationReason PreviousPreservation { get { Validate(); return _previousPreservation; } }
        public ProfileFilePreservationReason TempPreservation { get { Validate(); return _tempPreservation; } }

        public void Validate()
        {
            if (!KnownSource(_source) || !KnownPreservation(_primaryPreservation) || !KnownPreservation(_previousPreservation) || !KnownPreservation(_tempPreservation)) throw new InvalidOperationException("Load plan is malformed.");
            ValidateCandidate(_primaryProof, ProfileLoadFileRole.Primary); ValidateCandidate(_previousProof, ProfileLoadFileRole.Previous); ValidateCandidate(_tempProof, ProfileLoadFileRole.Temp);
            _inMemoryDocument.Validate(); if (!_inMemoryDocument.Snapshot.Input.IsCurrentCompatible || _inMemoryDocument.Snapshot.ProfileRevision != _resultRevision) throw new InvalidOperationException("Load plan document is malformed.");
            if (_primaryPreservation != PreservationFor(_primaryProof, true) || _previousPreservation != PreservationFor(_previousProof, false) || _tempPreservation != TempPreservationFor(_tempProof)) throw new InvalidOperationException("Load plan preservation proof is malformed.");

            if (_proofKind == 1)
            {
                var direct = RequireCurrent(_primaryProof); _directPrimaryProof.Validate();
                if (_source != ProfileLaunchSource.Primary || _requiresAtomicSave || _recoveryReasons != 0 || _sourceRevision < 0 || _resultRevision != _sourceRevision
                    || !DocumentsEqual(direct, _directPrimaryProof) || !DocumentsEqual(direct, _inMemoryDocument) || !ProfileLoadCandidateV1.IsDefault(_recoveryProof)) throw new InvalidOperationException("Direct primary load proof is malformed.");
                return;
            }
            if (_proofKind == 2)
            {
                var expected = ProfileRecoveryPlannerV1.PlanInputRepair(RequireRecoverable(_primaryProof));
                ValidateRecovery(expected, ProfileLaunchSource.Primary); return;
            }
            if (_proofKind == 3)
            {
                if (IsSelectable(_primaryProof)) throw new InvalidOperationException("Previous promotion ignored a selectable primary.");
                var expected = ProfileRecoveryPlannerV1.PlanPreviousPromotion(RequireSelectable(_previousProof));
                ValidateRecovery(expected, ProfileLaunchSource.Previous); return;
            }
            if (_proofKind == 4)
            {
                if (IsSelectable(_primaryProof) || IsSelectable(_previousProof)) throw new InvalidOperationException("Default load ignored a selectable candidate.");
                var expected = ProfileRecoveryPlannerV1.PlanDefaultBootstrap();
                ValidateRecovery(expected, ProfileLaunchSource.Default); return;
            }
            throw new InvalidOperationException("Load plan proof kind is malformed.");
        }

        private void ValidateRecovery(ProfileRecoveryPlanV1 expected, ProfileLaunchSource expectedSource)
        {
            expected.Validate(); _recoveryProof.Validate();
            if (_source != expectedSource || !_requiresAtomicSave || _sourceRevision != expected.SourceRevision || _resultRevision != expected.ResultRevision
                || _recoveryReasons != expected.Reasons || !RecoveryEqual(_recoveryProof, expected) || !DocumentsEqual(_inMemoryDocument, expected.ResultDocument)
                || !ProfileLoadCandidateV1.IsDefault(_directPrimaryProof)) throw new InvalidOperationException("Recovery load proof is malformed.");
        }

        private static void ValidateCandidate(ProfileLoadCandidateV1 candidate, ProfileLoadFileRole expectedRole)
        {
            candidate.Validate(); if (candidate.Role != expectedRole) throw new InvalidOperationException("Load proof role is malformed.");
        }
        private static ProfileCanonicalDocumentV1 RequireCurrent(ProfileLoadCandidateV1 candidate)
        {
            if (candidate.Kind != ProfileLoadCandidateKind.Decoded || candidate.DecodeResult.Classification != ProfileCanonicalDecodeClassification.ValidCurrentInput) throw new InvalidOperationException("Current source proof is malformed.");
            return candidate.DecodeResult.Document;
        }
        private static ProfileCanonicalDecodeResultV1 RequireRecoverable(ProfileLoadCandidateV1 candidate)
        {
            if (candidate.Kind != ProfileLoadCandidateKind.Decoded) throw new InvalidOperationException("Recovery source proof is malformed.");
            var result = candidate.DecodeResult; if (result.Classification != ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired && result.Classification != ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired) throw new InvalidOperationException("Recovery source classification is malformed.");
            return result;
        }
        private static ProfileCanonicalDecodeResultV1 RequireSelectable(ProfileLoadCandidateV1 candidate)
        {
            if (candidate.Kind != ProfileLoadCandidateKind.Decoded) throw new InvalidOperationException("Previous source proof is malformed.");
            var result = candidate.DecodeResult; if (result.Classification != ProfileCanonicalDecodeClassification.ValidCurrentInput && result.Classification != ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired && result.Classification != ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired) throw new InvalidOperationException("Previous source classification is malformed.");
            return result;
        }
        internal static bool IsSelectable(ProfileLoadCandidateV1 candidate)
        {
            if (candidate.Kind != ProfileLoadCandidateKind.Decoded) return false;
            var value = candidate.DecodeResult.Classification; return value == ProfileCanonicalDecodeClassification.ValidCurrentInput || value == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired || value == ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired;
        }
        internal static ProfileFilePreservationReason PreservationFor(ProfileLoadCandidateV1 candidate, bool primary)
        {
            if (candidate.Kind == ProfileLoadCandidateKind.Unreadable) return primary ? ProfileFilePreservationReason.UnreadablePrimary : ProfileFilePreservationReason.UnreadablePrevious;
            if (candidate.Kind != ProfileLoadCandidateKind.Decoded) return ProfileFilePreservationReason.None;
            var classification = candidate.DecodeResult.Classification;
            if (classification == ProfileCanonicalDecodeClassification.Invalid) return primary ? ProfileFilePreservationReason.InvalidPrimary : ProfileFilePreservationReason.InvalidPrevious;
            if (classification == ProfileCanonicalDecodeClassification.UnsupportedProfileSchema) return primary ? ProfileFilePreservationReason.UnsupportedPrimary : ProfileFilePreservationReason.UnsupportedPrevious;
            return ProfileFilePreservationReason.None;
        }
        internal static ProfileFilePreservationReason TempPreservationFor(ProfileLoadCandidateV1 candidate) => candidate.Kind == ProfileLoadCandidateKind.Missing ? ProfileFilePreservationReason.None : ProfileFilePreservationReason.StaleTemp;
        private static bool KnownSource(ProfileLaunchSource value) => value == ProfileLaunchSource.Primary || value == ProfileLaunchSource.Previous || value == ProfileLaunchSource.Default;
        private static bool KnownPreservation(ProfileFilePreservationReason value) => value >= ProfileFilePreservationReason.None && value <= ProfileFilePreservationReason.UnreadablePrevious;
        private static bool DocumentsEqual(ProfileCanonicalDocumentV1 left, ProfileCanonicalDocumentV1 right) => BytesEqual(left.CanonicalFileBytes, right.CanonicalFileBytes);
        private static bool RecoveryEqual(ProfileRecoveryPlanV1 left, ProfileRecoveryPlanV1 right) => left.SourceRevision == right.SourceRevision && left.ResultRevision == right.ResultRevision && left.Reasons == right.Reasons && DocumentsEqual(left.ResultDocument, right.ResultDocument);
        private static bool BytesEqual(byte[] left, byte[] right) { if (left == null || right == null || left.Length != right.Length) return false; for (var i = 0; i < left.Length; i++) if (left[i] != right[i]) return false; return true; }
    }

    public static class ProfileLoadSelectorV1
    {
        public static ProfileLoadSelectionPlanV1 Select(ProfileLoadCandidateV1 primary, ProfileLoadCandidateV1 previous, ProfileLoadCandidateV1 temp)
        {
            ValidateArgument(primary, ProfileLoadFileRole.Primary, nameof(primary)); ValidateArgument(previous, ProfileLoadFileRole.Previous, nameof(previous)); ValidateArgument(temp, ProfileLoadFileRole.Temp, nameof(temp));
            var primaryPreservation = ProfileLoadSelectionPlanV1.PreservationFor(primary, true); var previousPreservation = ProfileLoadSelectionPlanV1.PreservationFor(previous, false); var tempPreservation = ProfileLoadSelectionPlanV1.TempPreservationFor(temp);
            if (primary.Kind == ProfileLoadCandidateKind.Decoded && primary.DecodeResult.Classification == ProfileCanonicalDecodeClassification.ValidCurrentInput)
            {
                var document = primary.DecodeResult.Document; var revision = document.Snapshot.ProfileRevision;
                return new ProfileLoadSelectionPlanV1(ProfileLaunchSource.Primary, revision, revision, document, false, 0, primaryPreservation, previousPreservation, tempPreservation, 1, primary, previous, temp, default(ProfileRecoveryPlanV1), document);
            }
            if (primary.Kind == ProfileLoadCandidateKind.Decoded && (primary.DecodeResult.Classification == ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired || primary.DecodeResult.Classification == ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired))
            {
                var recovery = ProfileRecoveryPlannerV1.PlanInputRepair(primary.DecodeResult);
                return RecoveryPlan(ProfileLaunchSource.Primary, recovery, primaryPreservation, previousPreservation, tempPreservation, 2, primary, previous, temp);
            }
            if (ProfileLoadSelectionPlanV1.IsSelectable(previous))
            {
                var recovery = ProfileRecoveryPlannerV1.PlanPreviousPromotion(previous.DecodeResult);
                return RecoveryPlan(ProfileLaunchSource.Previous, recovery, primaryPreservation, previousPreservation, tempPreservation, 3, primary, previous, temp);
            }
            return RecoveryPlan(ProfileLaunchSource.Default, ProfileRecoveryPlannerV1.PlanDefaultBootstrap(), primaryPreservation, previousPreservation, tempPreservation, 4, primary, previous, temp);
        }

        private static ProfileLoadSelectionPlanV1 RecoveryPlan(ProfileLaunchSource source, ProfileRecoveryPlanV1 recovery, ProfileFilePreservationReason primaryPreservation, ProfileFilePreservationReason previousPreservation, ProfileFilePreservationReason tempPreservation, int proofKind, ProfileLoadCandidateV1 primary, ProfileLoadCandidateV1 previous, ProfileLoadCandidateV1 temp)
        {
            recovery.Validate(); return new ProfileLoadSelectionPlanV1(source, recovery.SourceRevision, recovery.ResultRevision, recovery.ResultDocument, true, recovery.Reasons, primaryPreservation, previousPreservation, tempPreservation, proofKind, primary, previous, temp, recovery, default(ProfileCanonicalDocumentV1));
        }
        private static void ValidateArgument(ProfileLoadCandidateV1 candidate, ProfileLoadFileRole role, string name)
        {
            try { candidate.Validate(); } catch (InvalidOperationException exception) { throw new ArgumentException("Candidate is malformed.", name, exception); }
            if (candidate.Role != role) throw new ArgumentException("Candidate role does not match its argument position.", name);
        }
    }
}
