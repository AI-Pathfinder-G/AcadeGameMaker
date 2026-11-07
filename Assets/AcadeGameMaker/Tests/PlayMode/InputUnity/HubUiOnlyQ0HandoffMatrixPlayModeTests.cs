using System;
using System.Collections.Generic;
using System.IO;
using AcadeGameMaker.Core;
using AcadeGameMaker.Profile;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Input.Unity.PlayMode.Tests
{
    // REQ-M5D7Q0-007 / AC-M5D7Q0-006.  Each row uses the real coordinator,
    // adapter, and latch; only the filesystem ports are deterministic fixtures.
    internal sealed class HubUiOnlyQ0HandoffMatrixPlayModeTests
    {
        [TestCaseSource(nameof(Scenarios))]
        public void AC006_PrimaryPreviousDefaultAndEveryTypedNotificationKindHaveOneExactHandoff(HubLaunchScenario scenario)
        {
            using (var context = HubLaunchMatrixContext.Create(scenario))
            {
                HubRuntimeContext.Invoke(context.Adapter, "Start");
                HubRuntimeContext.Invoke(context.Latch, "Start");
                HubRuntimeContext.Invoke(context.Latch, "Update");

                var handoff = context.Latch.CurrentHandoff;
                Assert.That(handoff.HasValue, Is.True, scenario.Name);
                handoff.Value.Validate();
                Assert.That(handoff.Value.LaunchReceipt.Source, Is.EqualTo(scenario.Source), scenario.Name + " source");
                Assert.That(handoff.Value.InputMode, Is.EqualTo(InputMode.UIOnly), scenario.Name + " mode");
                Assert.That(handoff.Value.GameplayEnabled, Is.False, scenario.Name + " gameplay");
                Assert.That(handoff.Value.UiEnabled, Is.True, scenario.Name + " UI");
                Assert.That(context.Adapter.BoundRouter, Is.SameAs(context.Router), scenario.Name + " adapter identity");
                Assert.That(context.Latch.BoundRouterForAuthoring, Is.SameAs(context.Router), scenario.Name + " latch/router identity");

                ProfileLaunchNotificationV1 notification;
                var taken = context.Latch.TryTakeLaunchNotification(out notification);
                Assert.That(taken, Is.EqualTo(scenario.NotificationKind.HasValue), scenario.Name + " notification presence");
                if (taken)
                {
                    notification.Validate();
                    Assert.That(notification.Kind, Is.EqualTo(scenario.NotificationKind.Value), scenario.Name + " notification kind");
                    Assert.That(HubEntryHandoffReceiptV1.SameReceipt(notification.Receipt, handoff.Value.LaunchReceipt), Is.True, scenario.Name + " exact correlation");
                }
                Assert.That(context.Latch.TryTakeLaunchNotification(out notification), Is.False, scenario.Name + " exactly one local handoff notification");
                Assert.That(context.Latch.CurrentHandoff, Is.EqualTo(handoff), scenario.Name + " immutable handoff");
            }
        }

        private static IEnumerable<TestCaseData> Scenarios
        {
            get
            {
                yield return new TestCaseData(HubLaunchScenario.CleanPrimary()).SetName("Primary_NoNotification");
                yield return new TestCaseData(HubLaunchScenario.Previous()).SetName("Previous_RecoveryCompleted");
                yield return new TestCaseData(HubLaunchScenario.Default()).SetName("Default_RecoveryCompleted");
                yield return new TestCaseData(HubLaunchScenario.PersistenceDeferred()).SetName("Default_PersistenceDeferred");
                yield return new TestCaseData(HubLaunchScenario.RecoveryArtifactPreservationFailed()).SetName("Primary_RecoveryArtifactPreservationFailed");
            }
        }
    }

    internal sealed class HubLaunchMatrixContext : IDisposable
    {
        private readonly string _root;
        private readonly GameObject _host;
        internal InputRouter Router { get; }
        internal DesktopProfileLaunchAdapterV1 Adapter { get; }
        internal HubEntryHandoffLatchV1 Latch { get; }

        private HubLaunchMatrixContext(GameObject host, string root, HubLaunchScenario scenario, InputRouter router, DesktopProfileLaunchAdapterV1 adapter, HubEntryHandoffLatchV1 latch)
        { _host = host; _root = root; Router = router; Adapter = adapter; Latch = latch; }

        internal static HubLaunchMatrixContext Create(HubLaunchScenario scenario)
        {
            var root = Path.Combine(Path.GetTempPath(), "m5d7q0-handoff-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            var host = new GameObject("m5d7q0-handoff-hub");
            host.SetActive(false);
            var router = host.AddComponent<InputRouter>();
            var adapter = host.AddComponent<DesktopProfileLaunchAdapterV1>();
            var latch = host.AddComponent<HubEntryHandoffLatchV1>();
            router.ConfigureHubUiOnlyForAuthoring();
            adapter.ConfigureHubUiOnlyForAuthoring(router);
            adapter.ConfigureForTests(router, new Environment(root), new ScenarioPreparation(scenario));
            latch.ConfigureHubUiOnlyForAuthoring(adapter, router);
            host.SetActive(true);
            return new HubLaunchMatrixContext(host, root, scenario, router, adapter, latch);
        }

        public void Dispose()
        {
            UnityEngine.Object.DestroyImmediate(_host);
            if (Directory.Exists(_root)) Directory.Delete(_root, true);
        }

        private sealed class Environment : IDesktopProfileLaunchEnvironmentV1
        {
            private readonly string _root;
            internal Environment(string root) { _root = root; }
            public string GetPersistentDataPath() => _root;
            public DateTimeOffset GetUtcNow() => new DateTimeOffset(2026, 9, 20, 0, 0, 0, TimeSpan.Zero);
        }

        private sealed class ScenarioPreparation : IDesktopProfileLaunchPreparationV1
        {
            private readonly HubLaunchScenario _scenario;
            internal ScenarioPreparation(HubLaunchScenario scenario) { _scenario = scenario; }
            public PreparedProfileLaunchV1 Prepare(string root, DateTimeOffset utc) =>
                ProfileLaunchPreparationCoordinatorV1.Prepare(root, utc, new ScenarioProfilePort(_scenario), _scenario.Actions);
        }

        private sealed class ScenarioProfilePort : IProfileLaunchPreparationProfilePortV1
        {
            private readonly HubLaunchScenario _scenario;
            internal ScenarioProfilePort(HubLaunchScenario scenario) { _scenario = scenario; }
            public ProfileLaunchObservationProjectionV1 Observe(string root) => _scenario.Observation;
            public ProfileLaunchPreservationProjectionV1 Preserve(string root, ProfileLaunchObservationProjectionV1 observation, ProfileLoadSelectionPlanV1 selection, DateTimeOffset utc) =>
                _scenario.Preservation ?? new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, true, null, selection.TempPreservation, selection.PrimaryPreservation, selection.PreviousPreservation, 0);
            public ProfileLaunchSaveProjectionV1 Save(string root, ProfileCanonicalDocumentV1 document) => _scenario.Save;
            public ProfileLaunchSaveProjectionV1 SaveInputRecovery(string root, ProfileLoadCandidateV1 primary) => _scenario.Save;
        }
    }

    internal sealed class HubLaunchScenario
    {
        internal string Name { get; }
        internal ProfileLaunchObservationProjectionV1 Observation { get; }
        internal ProfileLaunchPreservationProjectionV1? Preservation { get; }
        internal ProfileLaunchSaveProjectionV1 Save { get; }
        internal ProfileLaunchSource Source { get; }
        internal ProfileLaunchNotificationKindV1? NotificationKind { get; }
        internal HubLaunchTrackingActions Actions { get; }

        private HubLaunchScenario(string name, ProfileLaunchObservationProjectionV1 observation, ProfileLaunchPreservationProjectionV1? preservation, ProfileLaunchSaveProjectionV1 save, ProfileLaunchSource source, ProfileLaunchNotificationKindV1? notificationKind)
        { Name = name; Observation = observation; Preservation = preservation; Save = save; Source = source; NotificationKind = notificationKind; Actions = new HubLaunchTrackingActions(); }

        internal static HubLaunchScenario CleanPrimary()
        {
            var document = Document(3);
            return new HubLaunchScenario("clean-primary", new ProfileLaunchObservationProjectionV1(Candidate(ProfileLoadFileRole.Primary, document), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)), null, NoSave(), ProfileLaunchSource.Primary, null);
        }

        internal static HubLaunchScenario Previous()
        {
            var document = Document(4);
            return new HubLaunchScenario("previous", new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary), Candidate(ProfileLoadFileRole.Previous, document), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)), null, Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst, 5), ProfileLaunchSource.Previous, ProfileLaunchNotificationKindV1.RecoveryCompleted);
        }

        internal static HubLaunchScenario Default() => new HubLaunchScenario("default", MissingObservation(), null, Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst, 0), ProfileLaunchSource.Default, ProfileLaunchNotificationKindV1.RecoveryCompleted);

        internal static HubLaunchScenario PersistenceDeferred()
        {
            var preservation = new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.FailedBeforeMove, ProfileLaunchPreservationStateV1.NotRequired, false, ProfileLoadFileRole.Primary, ProfileFilePreservationReason.None, ProfileFilePreservationReason.UnreadablePrimary, ProfileFilePreservationReason.None, 1);
            var observation = new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Unreadable(ProfileLoadFileRole.Primary), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp));
            return new HubLaunchScenario("persistence-deferred", observation, preservation, new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation, ProfileAtomicSaveStage.None, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, null), ProfileLaunchSource.Default, ProfileLaunchNotificationKindV1.PersistenceDeferred);
        }

        internal static HubLaunchScenario RecoveryArtifactPreservationFailed()
        {
            var document = Document(7);
            var badTemp = ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Temp, ProfileCanonicalDecoderV1.Decode(System.Text.Encoding.UTF8.GetBytes("{}")));
            var preservation = new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.MoveOutcomeUncertain, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, false, ProfileLoadFileRole.Temp, ProfileFilePreservationReason.StaleTemp, ProfileFilePreservationReason.None, ProfileFilePreservationReason.None, 1);
            return new HubLaunchScenario("preservation-failed", new ProfileLaunchObservationProjectionV1(Candidate(ProfileLoadFileRole.Primary, document), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), badTemp), preservation, NoSave(), ProfileLaunchSource.Primary, ProfileLaunchNotificationKindV1.RecoveryArtifactPreservationFailed);
        }

        private static ProfileLaunchObservationProjectionV1 MissingObservation() => new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp));
        private static ProfileLaunchSaveProjectionV1 NoSave() => new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.None, ProfileLaunchPersistenceOutcomeV1.NotRequired, ProfileAtomicSaveStage.None, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, null);
        private static ProfileLaunchSaveProjectionV1 Ordinary(ProfileLaunchPersistenceOutcomeV1 outcome, long revision) => new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, outcome, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, ProfileStoredFileState.Missing, revision);
        private static ProfileLoadCandidateV1 Candidate(ProfileLoadFileRole role, ProfileCanonicalDocumentV1 document) => ProfileLoadCandidateV1.Decoded(role, ProfileCanonicalDecoderV1.Decode(document.CanonicalFileBytes));
        private static ProfileCanonicalDocumentV1 Document(long revision)
        {
            var input = new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, ProfileInputContract.CurrentBindingSchemaVersion, ProfileBindingOverridesJson.Parse(string.Empty));
            var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), input, new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
            return ProfileCanonicalEncoder.Encode(snapshot);
        }
    }

    internal sealed class HubLaunchTrackingActions : IProfileLaunchPreparationActionsPortV1
    {
        internal readonly List<GameInputActions> Created = new List<GameInputActions>();
        public GameInputActions Create() { var actions = new GameInputActions(); Created.Add(actions); return actions; }
        public ProfileBindingOverrideApplyResultV1 Apply(ProfileInputSnapshot input, GameInputActions candidate) => new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.DefaultsReady, ProfileBindingOverrideApplyFailureV1.None, ProfileBindingOverrideCandidateDispositionV1.Retain, string.Empty, string.Empty);
        public void Dispose(GameInputActions candidate) { candidate.Dispose(); }
    }
}
