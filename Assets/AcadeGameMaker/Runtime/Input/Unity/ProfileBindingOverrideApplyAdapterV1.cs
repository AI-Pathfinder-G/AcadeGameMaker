using System;
using AcadeGameMaker.Input;
using AcadeGameMaker.Profile;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-M5D7I-001..007. Candidate-only binding override apply detection; no InputRouter, persistence, or launch authority.
    internal enum ProfileBindingOverrideApplyOutcomeV1 { DefaultsReady = 1, OverridesApplied = 2, RecoveryRequired = 3 }
    internal enum ProfileBindingOverrideApplyFailureV1 { None = 0, LoadFailed = 1, RoundTripFailed = 2, RoundTripMismatch = 3 }
    internal enum ProfileBindingOverrideCandidateDispositionV1 { Retain = 1, Discard = 2 }

    internal readonly struct ProfileBindingOverrideApplyResultV1
    {
        private readonly ProfileBindingOverrideApplyOutcomeV1 _outcome; private readonly ProfileBindingOverrideApplyFailureV1 _failure; private readonly ProfileBindingOverrideCandidateDispositionV1 _disposition; private readonly string _requested; private readonly string _verified;
        internal ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1 outcome, ProfileBindingOverrideApplyFailureV1 failure, ProfileBindingOverrideCandidateDispositionV1 disposition, string requested, string verified) { _outcome = outcome; _failure = failure; _disposition = disposition; _requested = requested; _verified = verified; }
        internal ProfileBindingOverrideApplyOutcomeV1 Outcome { get { Validate(); return _outcome; } } internal ProfileBindingOverrideApplyFailureV1 Failure { get { Validate(); return _failure; } } internal ProfileBindingOverrideCandidateDispositionV1 CandidateDisposition { get { Validate(); return _disposition; } } internal string RequestedCanonicalText { get { Validate(); return _requested; } } internal bool HasVerifiedCanonicalText { get { Validate(); return _verified != null; } }
        internal string VerifiedCanonicalText { get { Validate(); if (_verified == null) throw new InvalidOperationException("Verified canonical text is unavailable."); return _verified; } }
        internal void Validate()
        {
            if (_requested == null || _outcome < ProfileBindingOverrideApplyOutcomeV1.DefaultsReady || _outcome > ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired || _failure < ProfileBindingOverrideApplyFailureV1.None || _failure > ProfileBindingOverrideApplyFailureV1.RoundTripMismatch || _disposition < ProfileBindingOverrideCandidateDispositionV1.Retain || _disposition > ProfileBindingOverrideCandidateDispositionV1.Discard) throw new InvalidOperationException("Binding override result is malformed.");
            ValidateCanonical(_requested);
            if (_outcome == ProfileBindingOverrideApplyOutcomeV1.DefaultsReady) { if (_failure != ProfileBindingOverrideApplyFailureV1.None || _disposition != ProfileBindingOverrideCandidateDispositionV1.Retain || _requested != string.Empty || _verified != string.Empty) throw new InvalidOperationException("Defaults result is malformed."); return; }
            if (_outcome == ProfileBindingOverrideApplyOutcomeV1.OverridesApplied) { if (_failure != ProfileBindingOverrideApplyFailureV1.None || _disposition != ProfileBindingOverrideCandidateDispositionV1.Retain || _requested.Length == 0 || _verified == null || _verified.Length == 0 || !string.Equals(_requested, _verified, StringComparison.Ordinal)) throw new InvalidOperationException("Applied result is malformed."); ValidateCanonical(_verified); return; }
            if (_disposition != ProfileBindingOverrideCandidateDispositionV1.Discard || _requested.Length == 0) throw new InvalidOperationException("Recovery result is malformed.");
            if (_failure == ProfileBindingOverrideApplyFailureV1.LoadFailed || _failure == ProfileBindingOverrideApplyFailureV1.RoundTripFailed) { if (_verified != null) throw new InvalidOperationException("Unavailable proof is malformed."); return; }
            if (_failure != ProfileBindingOverrideApplyFailureV1.RoundTripMismatch || _verified == null || string.Equals(_requested, _verified, StringComparison.Ordinal)) throw new InvalidOperationException("Mismatch result is malformed."); ValidateCanonical(_verified);
        }
        private static void ValidateCanonical(string value) { try { if (ProfileBindingOverridesJson.Parse(value).CanonicalText != value) throw new InvalidOperationException("Proof is noncanonical."); } catch (ArgumentException e) { throw new InvalidOperationException("Proof is malformed.", e); } }
    }

    internal interface IProfileBindingOverrideApplyPort { string Save(GameInputActions candidate); void Load(GameInputActions candidate, string json); }
    internal sealed class ProfileBindingOverrideApplyBclPort : IProfileBindingOverrideApplyPort
    {
        internal static readonly ProfileBindingOverrideApplyBclPort Instance = new ProfileBindingOverrideApplyBclPort(); private ProfileBindingOverrideApplyBclPort() { }
        public string Save(GameInputActions candidate) { return candidate.asset.SaveBindingOverridesAsJson(); }
        public void Load(GameInputActions candidate, string json) { candidate.asset.LoadBindingOverridesFromJson(json, true); } // removeExisting: true
    }

    internal static class ProfileBindingOverrideApplyAdapterV1
    {
        internal static ProfileBindingOverrideApplyResultV1 Apply(ProfileInputSnapshot input, GameInputActions candidate) => Apply(input, candidate, ProfileBindingOverrideApplyBclPort.Instance);
        internal static ProfileBindingOverrideApplyResultV1 Apply(ProfileInputSnapshot input, GameInputActions candidate, IProfileBindingOverrideApplyPort port)
        {
            if (port == null) throw new ArgumentNullException(nameof(port)); input.Validate(); if (input.Compatibility != ProfileInputCompatibility.Current) throw new ArgumentException("Input metadata is not current.", nameof(input)); ValidateCandidate(candidate);
            var requested = input.BindingOverrides.CanonicalText; var pristine = port.Save(candidate); if (pristine != string.Empty) throw new ArgumentException("Candidate has overrides.", nameof(candidate));
            if (requested == string.Empty) return new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.DefaultsReady, ProfileBindingOverrideApplyFailureV1.None, ProfileBindingOverrideCandidateDispositionV1.Retain, string.Empty, string.Empty);
            try { port.Load(candidate, requested); } catch (Exception e) when (Recoverable(e)) { return Recovery(ProfileBindingOverrideApplyFailureV1.LoadFailed, requested, null); }
            string raw; try { raw = port.Save(candidate); } catch (Exception e) when (Recoverable(e)) { return Recovery(ProfileBindingOverrideApplyFailureV1.RoundTripFailed, requested, null); }
            string verified; try { verified = ProfileBindingOverridesJson.Parse(raw).CanonicalText; } catch (Exception e) when (e is ArgumentException || e is InvalidOperationException) { return Recovery(ProfileBindingOverrideApplyFailureV1.RoundTripFailed, requested, null); }
            if (!string.Equals(requested, verified, StringComparison.Ordinal)) return Recovery(ProfileBindingOverrideApplyFailureV1.RoundTripMismatch, requested, verified);
            return new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.OverridesApplied, ProfileBindingOverrideApplyFailureV1.None, ProfileBindingOverrideCandidateDispositionV1.Retain, requested, verified);
        }
        private static ProfileBindingOverrideApplyResultV1 Recovery(ProfileBindingOverrideApplyFailureV1 failure, string requested, string verified) => new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired, failure, ProfileBindingOverrideCandidateDispositionV1.Discard, requested, verified);
        private static void ValidateCandidate(GameInputActions candidate) { if (candidate == null || candidate.asset == null || candidate.asset.enabled) throw new ArgumentException("Candidate is unavailable or enabled.", nameof(candidate)); foreach (var map in candidate.asset.actionMaps) if (map.enabled) throw new ArgumentException("Candidate map is enabled.", nameof(candidate)); }
        private static bool Recoverable(Exception e) => e is ArgumentException || e is InvalidOperationException || e is FormatException || e is NullReferenceException || e is IndexOutOfRangeException || e is UnityException;
    }
}
