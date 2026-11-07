using System;
using System.Collections.Generic;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D3-001..006. Closed schema-v1 progression values; no profile ownership or persistence authority.
    public enum ProfileChoice
    {
        Extraction = 1,
        Solidarity = 2
    }

    public enum ProfileSkill
    {
        CompressionVerdict = 1,
        CommonReferencePlane = 2
    }

    public enum ProfileConsentState
    {
        RefusesOwnershipTransfer = 1,
        OffersScopedResonance = 2,
        ResonanceLender = 3,
        ImprintSevered = 4
    }

    public enum ProfileBranch
    {
        Extraction = 1,
        Solidarity = 2
    }

    public readonly struct ProfileChoiceSkillPair
    {
        private readonly ProfileChoice _choice;
        private readonly ProfileSkill _skill;

        public ProfileChoiceSkillPair(ProfileChoice choice, ProfileSkill skill)
        {
            if (!ProfileProgressionValidation.IsExactPair(choice, skill))
                throw new ArgumentException("Profile choice and skill must be one approved exact pair.");
            _choice = choice;
            _skill = skill;
        }

        public ProfileChoice Choice
        {
            get
            {
                Validate();
                return _choice;
            }
        }

        public ProfileSkill Skill
        {
            get
            {
                Validate();
                return _skill;
            }
        }

        // REQ-M5D3-001, REQ-M5D3-003. Revalidates default and constructor-bypass values at public consumption.
        public void Validate()
        {
            if (!ProfileProgressionValidation.IsExactPair(_choice, _skill))
                throw new InvalidOperationException("Profile choice-skill pair is not an approved exact pair.");
        }
    }

    public readonly struct ProfileProgressionSnapshot
    {
        private readonly long _profileRevision;
        private readonly int? _lastOfferedSeed;
        private readonly ProfileChoice? _committedChoice;
        private readonly ProfileConsentState _consentState;
        private readonly ProfileSkill? _grantedSkill;
        private readonly ProfileBranch[] _completedBranches;

        public ProfileProgressionSnapshot(long profileRevision, int? lastOfferedSeed, ProfileChoice? committedChoice,
            ProfileConsentState consentState, ProfileSkill? grantedSkill, IReadOnlyList<ProfileBranch> completedBranches)
        {
            ProfileProgressionValidation.ValidateRevisionArgument(profileRevision);
            ProfileProgressionValidation.ValidateSeedArgument(lastOfferedSeed);
            ProfileProgressionValidation.ValidateChoiceSkillArgument(committedChoice, grantedSkill);
            ProfileProgressionValidation.ValidateConsentArgument(consentState);
            var copiedBranches = ProfileProgressionValidation.CopyAndValidateBranchesArgument(completedBranches);

            _profileRevision = profileRevision;
            _lastOfferedSeed = lastOfferedSeed;
            _committedChoice = committedChoice;
            _consentState = consentState;
            _grantedSkill = grantedSkill;
            _completedBranches = copiedBranches;
        }

        public long ProfileRevision
        {
            get
            {
                Validate();
                return _profileRevision;
            }
        }

        public int? LastOfferedSeed
        {
            get
            {
                Validate();
                return _lastOfferedSeed;
            }
        }

        public ProfileChoice? CommittedChoice
        {
            get
            {
                Validate();
                return _committedChoice;
            }
        }

        public ProfileConsentState ConsentState
        {
            get
            {
                Validate();
                return _consentState;
            }
        }

        public ProfileSkill? GrantedSkill
        {
            get
            {
                Validate();
                return _grantedSkill;
            }
        }

        public IReadOnlyList<ProfileBranch> CompletedBranches
        {
            get
            {
                Validate();
                return Array.AsReadOnly((ProfileBranch[])_completedBranches.Clone());
            }
        }

        public bool HasCommittedPair
        {
            get
            {
                Validate();
                return _committedChoice.HasValue;
            }
        }

        public bool TryGetCommittedPair(out ProfileChoiceSkillPair pair)
        {
            Validate();
            if (!_committedChoice.HasValue)
            {
                pair = default(ProfileChoiceSkillPair);
                return false;
            }

            pair = new ProfileChoiceSkillPair(_committedChoice.Value, _grantedSkill.Value);
            return true;
        }

        // REQ-M5D3-001..004. Does not normalize invalid state or infer progression transitions.
        public void Validate()
        {
            if (!ProfileProgressionValidation.IsValidRevision(_profileRevision)
                || !ProfileProgressionValidation.IsValidSeed(_lastOfferedSeed)
                || !ProfileProgressionValidation.IsValidChoiceSkill(_committedChoice, _grantedSkill)
                || !ProfileProgressionValidation.IsKnownConsent(_consentState)
                || !ProfileProgressionValidation.AreCanonicalBranches(_completedBranches))
                throw new InvalidOperationException("Profile progression snapshot is malformed.");
        }
    }

    internal static class ProfileProgressionValidation
    {
        internal static bool IsExactPair(ProfileChoice choice, ProfileSkill skill)
        {
            return (choice == ProfileChoice.Extraction && skill == ProfileSkill.CompressionVerdict)
                || (choice == ProfileChoice.Solidarity && skill == ProfileSkill.CommonReferencePlane);
        }

        internal static bool IsKnownChoice(ProfileChoice value) => value == ProfileChoice.Extraction || value == ProfileChoice.Solidarity;
        internal static bool IsKnownSkill(ProfileSkill value) => value == ProfileSkill.CompressionVerdict || value == ProfileSkill.CommonReferencePlane;
        internal static bool IsKnownConsent(ProfileConsentState value) => value >= ProfileConsentState.RefusesOwnershipTransfer && value <= ProfileConsentState.ImprintSevered;
        internal static bool IsKnownBranch(ProfileBranch value) => value == ProfileBranch.Extraction || value == ProfileBranch.Solidarity;
        internal static bool IsValidRevision(long value) => value >= 0;
        internal static bool IsValidSeed(int? value) => !value.HasValue || value.Value == 101 || value.Value == 202 || value.Value == 303 || value.Value == 404;

        internal static bool IsValidChoiceSkill(ProfileChoice? choice, ProfileSkill? skill)
        {
            if (!choice.HasValue && !skill.HasValue) return true;
            return choice.HasValue && skill.HasValue && IsExactPair(choice.Value, skill.Value);
        }

        internal static bool AreCanonicalBranches(ProfileBranch[] values)
        {
            if (values == null || values.Length > 2) return false;
            var previous = 0;
            for (var index = 0; index < values.Length; index++)
            {
                var current = (int)values[index];
                if (!IsKnownBranch(values[index]) || current <= previous) return false;
                previous = current;
            }
            return true;
        }

        internal static void ValidateRevisionArgument(long value)
        {
            if (!IsValidRevision(value)) throw new ArgumentOutOfRangeException(nameof(value));
        }

        internal static void ValidateSeedArgument(int? value)
        {
            if (!IsValidSeed(value)) throw new ArgumentOutOfRangeException(nameof(value));
        }

        internal static void ValidateConsentArgument(ProfileConsentState value)
        {
            if (!IsKnownConsent(value)) throw new ArgumentOutOfRangeException(nameof(value));
        }

        internal static void ValidateChoiceSkillArgument(ProfileChoice? choice, ProfileSkill? skill)
        {
            if (!IsValidChoiceSkill(choice, skill)) throw new ArgumentException("Profile choice-skill fields are not a closed pair.");
        }

        internal static ProfileBranch[] CopyAndValidateBranchesArgument(IReadOnlyList<ProfileBranch> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var copy = new ProfileBranch[values.Count];
            for (var index = 0; index < copy.Length; index++) copy[index] = values[index];
            if (!AreCanonicalBranches(copy)) throw new ArgumentException("Completed branches must be sorted, unique, and known.", nameof(values));
            return copy;
        }
    }
}
