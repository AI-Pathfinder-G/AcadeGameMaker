using System;
using System.Reflection;
using System.Text.RegularExpressions;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using NUnit.Framework;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-M5D7I-001..007; AC-M5D7I-001..009. Fresh generated candidates only; never touches InputRouter.
    internal sealed class ProfileBindingOverrideApplyAdapterV1Tests : InputTestFixture
    {
        [Test]
        public void AC001_EmptySentinelRetainsDisabledGeneratedDefaults()
        {
            using (var actions = new GameInputActions())
            {
                var result = ProfileBindingOverrideApplyAdapterV1.Apply(Input(string.Empty), actions);
                Assert.That(result.Outcome, Is.EqualTo(ProfileBindingOverrideApplyOutcomeV1.DefaultsReady)); Assert.That(result.Failure, Is.EqualTo(ProfileBindingOverrideApplyFailureV1.None)); Assert.That(result.CandidateDisposition, Is.EqualTo(ProfileBindingOverrideCandidateDispositionV1.Retain)); Assert.That(result.RequestedCanonicalText, Is.EqualTo(string.Empty)); Assert.That(result.VerifiedCanonicalText, Is.EqualTo(string.Empty)); Assert.That(actions.asset.SaveBindingOverridesAsJson(), Is.EqualTo(string.Empty)); Assert.That(actions.asset.enabled, Is.False);
            }
        }
        [Test]
        public void AC002_RealGeneratedOverrideRoundTripsCanonicalMeaning()
        {
            string canonical;
            using (var source = new GameInputActions()) { source.Gameplay.Jump.ApplyBindingOverride("<Keyboard>/k"); canonical = ProfileBindingOverridesJson.Parse(source.asset.SaveBindingOverridesAsJson()).CanonicalText; }
            using (var candidate = new GameInputActions())
            {
                var result = ProfileBindingOverrideApplyAdapterV1.Apply(Input(canonical), candidate);
                Assert.That(result.Outcome, Is.EqualTo(ProfileBindingOverrideApplyOutcomeV1.OverridesApplied)); Assert.That(result.VerifiedCanonicalText, Is.EqualTo(canonical)); Assert.That(result.CandidateDisposition, Is.EqualTo(ProfileBindingOverrideCandidateDispositionV1.Retain));
            }
        }
        [Test]
        public void AC005_PreflightMisuseThrowsBeforeLoad()
        {
            var current = Input(string.Empty);
            Assert.Throws<ArgumentNullException>(() => ProfileBindingOverrideApplyAdapterV1.Apply(current, null, null));
            var nullCandidatePort = new Port();
            Assert.Throws<ArgumentException>(() => ProfileBindingOverrideApplyAdapterV1.Apply(current, null, nullCandidatePort));
            Assert.That(nullCandidatePort.Calls, Is.Empty);
            var metadataPort = new Port();
            using (var metadata = new GameInputActions())
            {
                Assert.Throws<ArgumentException>(() => ProfileBindingOverrideApplyAdapterV1.Apply(new ProfileInputSnapshot("obsolete", 1, ProfileBindingOverridesJson.Parse(string.Empty)), metadata, metadataPort));
                Assert.That(metadataPort.Calls, Is.Empty);
            }
            var defaultPort = new Port();
            using (var candidate = new GameInputActions())
            {
                Assert.Throws<InvalidOperationException>(() => ProfileBindingOverrideApplyAdapterV1.Apply(default(ProfileInputSnapshot), candidate, defaultPort));
                Assert.That(defaultPort.Calls, Is.Empty);
            }
            var reflectionPort = new Port();
            using (var candidate = new GameInputActions())
            {
                object boxed = current;
                typeof(ProfileInputSnapshot).GetField("_inputActionsAssetId", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, null);
                Assert.Throws<InvalidOperationException>(() => ProfileBindingOverrideApplyAdapterV1.Apply((ProfileInputSnapshot)boxed, candidate, reflectionPort));
                Assert.That(reflectionPort.Calls, Is.Empty);
            }
            var enabledPort = new Port();
            using (var enabled = new GameInputActions())
            {
                try
                {
                    enabled.Gameplay.Enable();
                    Assert.Throws<ArgumentException>(() => ProfileBindingOverrideApplyAdapterV1.Apply(current, enabled, enabledPort));
                    Assert.That(enabledPort.Calls, Is.Empty);
                }
                finally { enabled.Gameplay.Disable(); }
            }
            var overriddenPort = new Port();
            using (var overridden = new GameInputActions())
            {
                overridden.Gameplay.Jump.ApplyBindingOverride("<Keyboard>/k");
                Assert.Throws<ArgumentException>(() => ProfileBindingOverrideApplyAdapterV1.Apply(current, overridden));
            }
            var preflightPort = new Port { FirstSave = "[]" };
            using (var candidate = new GameInputActions())
            {
                Assert.Throws<ArgumentException>(() => ProfileBindingOverrideApplyAdapterV1.Apply(current, candidate, preflightPort));
                CollectionAssert.AreEqual(new[] { "Save" }, preflightPort.Calls);
            }
            var disposedPort = new Port();
            var disposed = new GameInputActions();
            try
            {
                UnityEngine.Object.DestroyImmediate(disposed.asset);
                Assert.Throws<ArgumentException>(() => ProfileBindingOverrideApplyAdapterV1.Apply(current, disposed, disposedPort));
                Assert.That(disposedPort.Calls, Is.Empty);
            }
            finally { if (disposed.asset != null) disposed.Dispose(); }
        }
        [Test]
        public void AC008_StaticAuthorityIsCandidateOnly()
        {
            var source = System.IO.File.ReadAllText(System.IO.Path.Combine(FindRoot(), "Assets", "AcadeGameMaker", "Runtime", "Input", "Unity", "ProfileBindingOverrideApplyAdapterV1.cs"));
            foreach (var required in new[] { "LoadBindingOverridesFromJson(json, true)", "SaveBindingOverridesAsJson", "removeExisting" }) StringAssert.Contains(required, source);
            foreach (var forbidden in new[] { "typeof(InputRouter)", "InputRouter.", "RemoveAllBindingOverrides", "File.", "DateTime", "Random", "persistentDataPath" }) StringAssert.DoesNotContain(forbidden, source);
        }
        [Test]
        public void AC004_AC006_DeterministicPortSequencesAndClosedFailures()
        {
            using (var actions = new GameInputActions()) { var port = new Port(); var result = ProfileBindingOverrideApplyAdapterV1.Apply(Input(string.Empty), actions, port); Assert.That(result.Outcome, Is.EqualTo(ProfileBindingOverrideApplyOutcomeV1.DefaultsReady)); CollectionAssert.AreEqual(new[] { "Save" }, port.Calls); }
            foreach (var kind in new[] { "load-ArgumentException", "load-InvalidOperationException", "load-FormatException", "load-NullReferenceException", "load-IndexOutOfRangeException", "load-UnityException" })
            using (var actions = new GameInputActions()) { var port = new Port { Fault = kind }; var result = ProfileBindingOverrideApplyAdapterV1.Apply(Input("[]"), actions, port); Assert.That(result.Failure, Is.EqualTo(ProfileBindingOverrideApplyFailureV1.LoadFailed)); CollectionAssert.AreEqual(new[] { "Save", "Load" }, port.Calls); Assert.That(result.CandidateDisposition, Is.EqualTo(ProfileBindingOverrideCandidateDispositionV1.Discard)); }
            foreach (var kind in new[] { "save-ArgumentException", "save-InvalidOperationException", "save-FormatException", "save-NullReferenceException", "save-IndexOutOfRangeException", "save-UnityException" })
            using (var actions = new GameInputActions()) { var port = new Port { Fault = kind }; var result = ProfileBindingOverrideApplyAdapterV1.Apply(Input("[]"), actions, port); Assert.That(result.Failure, Is.EqualTo(ProfileBindingOverrideApplyFailureV1.RoundTripFailed)); CollectionAssert.AreEqual(new[] { "Save", "Load", "Save" }, port.Calls); }
            using (var actions = new GameInputActions()) { var port = new Port { RoundTrip = "[]" }; var result = ProfileBindingOverrideApplyAdapterV1.Apply(Input(KnownCanonical()), actions, port); Assert.That(result.Failure, Is.EqualTo(ProfileBindingOverrideApplyFailureV1.RoundTripMismatch)); Assert.That(result.HasVerifiedCanonicalText, Is.True); CollectionAssert.AreEqual(new[] { "Save", "Load", "Save" }, port.Calls); }
            using (var actions = new GameInputActions()) { var port = new Port { RoundTrip = "{" }; var result = ProfileBindingOverrideApplyAdapterV1.Apply(Input("[]"), actions, port); Assert.That(result.Failure, Is.EqualTo(ProfileBindingOverrideApplyFailureV1.RoundTripFailed)); CollectionAssert.AreEqual(new[] { "Save", "Load", "Save" }, port.Calls); }
            using (var actions = new GameInputActions()) { var port = new Port { Fault = "load-unexpected" }; Assert.Throws<NotSupportedException>(() => ProfileBindingOverrideApplyAdapterV1.Apply(Input("[]"), actions, port)); }
            using (var actions = new GameInputActions()) { var port = new Port { Fault = "load-fatal" }; Assert.Throws<OutOfMemoryException>(() => ProfileBindingOverrideApplyAdapterV1.Apply(Input("[]"), actions, port)); }
        }
        [Test]
        public void AC003_UnknownBindingIdIsWarningOnlyMismatch()
        {
            using (var actions = new GameInputActions())
            {
                var input = Input(StaleBindingIdCanonical());
                LogAssert.Expect(LogType.Warning, new Regex(".*"));
                var result = ProfileBindingOverrideApplyAdapterV1.Apply(input, actions);
                Assert.That(result.Outcome, Is.EqualTo(ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired));
                Assert.That(result.Failure, Is.EqualTo(ProfileBindingOverrideApplyFailureV1.RoundTripMismatch));
                Assert.That(result.CandidateDisposition, Is.EqualTo(ProfileBindingOverrideCandidateDispositionV1.Discard));
            }
        }

        [Test]
        public void AC006_UnexpectedAndFatalPortExceptionsPropagateAndFreshCallRemainsIndependent()
        {
            foreach (var fault in new[] { "load-unexpected", "load-fatal", "save-unexpected", "save-fatal" })
            using (var actions = new GameInputActions())
            {
                var port = new Port { Fault = fault };
                Assert.Throws(fault.EndsWith("fatal", StringComparison.Ordinal) ? typeof(OutOfMemoryException) : typeof(NotSupportedException), () => ProfileBindingOverrideApplyAdapterV1.Apply(Input("[]"), actions, port), fault);
                Assert.That(port.Calls.Count, Is.EqualTo(fault.StartsWith("load-", StringComparison.Ordinal) ? 2 : 3), fault);
            }
            using (var actions = new GameInputActions())
            {
                var port = new Port();
                var result = ProfileBindingOverrideApplyAdapterV1.Apply(Input("[]"), actions, port);
                Assert.That(result.Outcome, Is.EqualTo(ProfileBindingOverrideApplyOutcomeV1.OverridesApplied));
                CollectionAssert.AreEqual(new[] { "Save", "Load", "Save" }, port.Calls);
            }
        }

        [Test]
        public void AC007_LegalResultRowsAndIllegalReflectionCombinationsFailClosed()
        {
            var requested = KnownCanonical();
            var rows = new[]
            {
                new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.DefaultsReady, ProfileBindingOverrideApplyFailureV1.None, ProfileBindingOverrideCandidateDispositionV1.Retain, string.Empty, string.Empty),
                new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.OverridesApplied, ProfileBindingOverrideApplyFailureV1.None, ProfileBindingOverrideCandidateDispositionV1.Retain, requested, requested),
                new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired, ProfileBindingOverrideApplyFailureV1.LoadFailed, ProfileBindingOverrideCandidateDispositionV1.Discard, requested, null),
                new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired, ProfileBindingOverrideApplyFailureV1.RoundTripFailed, ProfileBindingOverrideCandidateDispositionV1.Discard, requested, null),
                new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired, ProfileBindingOverrideApplyFailureV1.RoundTripMismatch, ProfileBindingOverrideCandidateDispositionV1.Discard, requested, "[]")
            };
            foreach (var row in rows)
            {
                row.Validate();
                var outcome = row.Outcome; var failure = row.Failure; var disposition = row.CandidateDisposition; var requestedText = row.RequestedCanonicalText;
                if (failure == ProfileBindingOverrideApplyFailureV1.LoadFailed || failure == ProfileBindingOverrideApplyFailureV1.RoundTripFailed)
                {
                    Assert.That(row.HasVerifiedCanonicalText, Is.False);
                    Assert.Throws<InvalidOperationException>(() => { var unavailable = row.VerifiedCanonicalText; });
                }
                else
                {
                    Assert.That(row.HasVerifiedCanonicalText, Is.True);
                    var verifiedText = row.VerifiedCanonicalText;
                }
            }

            AssertResultGettersFail(rows[0], "_outcome", (ProfileBindingOverrideApplyOutcomeV1)99);
            AssertResultGettersFail(rows[0], "_failure", ProfileBindingOverrideApplyFailureV1.LoadFailed);
            AssertResultGettersFail(rows[0], "_disposition", (ProfileBindingOverrideCandidateDispositionV1)99);
            AssertResultGettersFail(rows[0], "_requested", null);
            AssertResultGettersFail(rows[0], "_verified", null);
            AssertResultGettersFail(rows[0], "_requested", "[]");
            AssertResultGettersFail(rows[1], "_verified", "[]");
            AssertResultGettersFail(rows[1], "_requested", string.Empty);
            AssertResultGettersFail(rows[2], "_failure", ProfileBindingOverrideApplyFailureV1.None);
            AssertResultGettersFail(rows[2], "_disposition", ProfileBindingOverrideCandidateDispositionV1.Retain);
            AssertResultGettersFail(rows[2], "_requested", string.Empty);
            AssertResultGettersFail(rows[2], "_verified", "[]");
            AssertResultGettersFail(rows[4], "_verified", null);
            AssertResultGettersFail(rows[4], "_verified", requested);
            AssertResultGettersFail(rows[4], "_verified", "{");
        }
        private static ProfileInputSnapshot Input(string overrides) => new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, ProfileInputContract.CurrentBindingSchemaVersion, ProfileBindingOverridesJson.Parse(overrides));
        private static string KnownCanonical()
        {
            using (var source = new GameInputActions())
            {
                source.Gameplay.Jump.ApplyBindingOverride("<Keyboard>/k");
                return ProfileBindingOverridesJson.Parse(source.asset.SaveBindingOverridesAsJson()).CanonicalText;
            }
        }
        private static string StaleBindingIdCanonical()
        {
            using (var source = new GameInputActions())
            {
                source.Gameplay.Jump.ApplyBindingOverride("<Keyboard>/k");
                var saved = source.asset.SaveBindingOverridesAsJson();
                StringAssert.Contains("\"bindings\"", saved);
                var match = Regex.Match(saved, "\\\"id\\\":\\\"[^\\\"]+\\\"", RegexOptions.CultureInvariant);
                if (!match.Success) throw new InvalidOperationException("Generated Input System override did not contain a binding ID.");
                var stale = saved.Substring(0, match.Index) + "\"id\":\"11111111-2222-4333-8444-555555555555\"" + saved.Substring(match.Index + match.Length);
                return ProfileBindingOverridesJson.Parse(stale).CanonicalText;
            }
        }
        private static void AssertResultGettersFail(ProfileBindingOverrideApplyResultV1 source, string field, object value)
        {
            object boxed = source;
            typeof(ProfileBindingOverrideApplyResultV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value);
            var result = (ProfileBindingOverrideApplyResultV1)boxed;
            Assert.Throws<InvalidOperationException>(() => result.Validate());
            Assert.Throws<InvalidOperationException>(() => { var unused = result.Outcome; });
            Assert.Throws<InvalidOperationException>(() => { var unused = result.Failure; });
            Assert.Throws<InvalidOperationException>(() => { var unused = result.CandidateDisposition; });
            Assert.Throws<InvalidOperationException>(() => { var unused = result.RequestedCanonicalText; });
            Assert.Throws<InvalidOperationException>(() => { var unused = result.HasVerifiedCanonicalText; });
            Assert.Throws<InvalidOperationException>(() => { var unused = result.VerifiedCanonicalText; });
        }
        private static string FindRoot() { var current = new System.IO.DirectoryInfo(TestContext.CurrentContext.TestDirectory); while (current != null) { if (System.IO.File.Exists(System.IO.Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; } throw new System.IO.DirectoryNotFoundException(); }
        private sealed class Port : IProfileBindingOverrideApplyPort
        {
            internal string Fault; internal string FirstSave; internal string RoundTrip; internal readonly System.Collections.Generic.List<string> Calls = new System.Collections.Generic.List<string>();
            public string Save(GameInputActions candidate) { Calls.Add("Save"); if (Calls.Count == 3 && Fault != null && Fault.StartsWith("save-", StringComparison.Ordinal)) Throw(Fault.Substring(5)); return Calls.Count == 1 ? (FirstSave ?? string.Empty) : (RoundTrip ?? "[]"); }
            public void Load(GameInputActions candidate, string json) { Calls.Add("Load"); if (Fault != null && Fault.StartsWith("load-", StringComparison.Ordinal)) Throw(Fault.Substring(5)); }
            private static void Throw(string kind) { if (kind == "ArgumentException") throw new ArgumentException(); if (kind == "InvalidOperationException") throw new InvalidOperationException(); if (kind == "FormatException") throw new FormatException(); if (kind == "NullReferenceException") throw new NullReferenceException(); if (kind == "IndexOutOfRangeException") throw new IndexOutOfRangeException(); if (kind == "UnityException") throw new UnityException(); if (kind == "fatal") throw new OutOfMemoryException(); throw new NotSupportedException(); }
        }
    }
}
