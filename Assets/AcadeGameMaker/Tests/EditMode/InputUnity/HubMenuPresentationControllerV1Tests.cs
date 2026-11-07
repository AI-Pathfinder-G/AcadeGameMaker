using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using NUnit.Framework;
using UnityEngine.InputSystem;

namespace AcadeGameMaker.Tests.EditMode.InputUnity
{
    // REQ-M5D7O-001..008; focused AC-M5D7O-001..007 coverage via injected M5D7L ports.
    internal sealed class HubMenuPresentationControllerV1Tests
    {
        private static readonly DateTimeOffset Utc = new DateTimeOffset(2026, 9, 14, 4, 5, 6, TimeSpan.Zero);

        [Test]
        public void AC001_SourceAloneDeterminesProfileContinuationAcrossPersistenceOutcomes()
        {
            using (var primary = Build(PrimaryObservation(4), Ordinary(ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit, ProfileAtomicSaveStage.DirectoryPreparation, null), null))
            {
                var controller = Create(primary.Receipt, ProfileLaunchNotificationV1.FromReceipt(primary.Receipt.LaunchReceipt));
                AssertView(controller.CurrentView, true, HubMenuItemV1.Continue);
            }
            using (var previous = Build(PreviousObservation(5), Ordinary(ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain, ProfileAtomicSaveStage.CommitCall, null), null))
            {
                var controller = Create(previous.Receipt, ProfileLaunchNotificationV1.FromReceipt(previous.Receipt.LaunchReceipt));
                AssertView(controller.CurrentView, true, HubMenuItemV1.Continue);
            }
            using (var defaults = Build(DefaultObservation(), Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, 0), null))
            {
                var controller = Create(defaults.Receipt, ProfileLaunchNotificationV1.FromReceipt(defaults.Receipt.LaunchReceipt));
                AssertView(controller.CurrentView, false, HubMenuItemV1.NewGame);
            }
            using (var denied = Build(DefaultDeniedObservation(), Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, 0), PreservationDenied()))
            {
                var controller = Create(denied.Receipt, ProfileLaunchNotificationV1.FromReceipt(denied.Receipt.LaunchReceipt));
                AssertView(controller.CurrentView, false, HubMenuItemV1.NewGame);
            }
        }

        [Test]
        public void AC002_ExactOrderAndDisabledContinueRemainStable()
        {
            using (var fixture = Build(DefaultObservation(), Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, 0), null))
            {
                var controller = Create(fixture.Receipt, ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt));
                var view = controller.CurrentView;
                Assert.That(view.GetMenuItem(0), Is.EqualTo(HubMenuItemV1.Continue));
                Assert.That(view.GetMenuItem(1), Is.EqualTo(HubMenuItemV1.NewGame));
                Assert.That(view.GetMenuItem(2), Is.EqualTo(HubMenuItemV1.Settings));
                Assert.That(view.GetMenuItem(3), Is.EqualTo(HubMenuItemV1.Quit));
                Assert.That(view.IsInteractable(HubMenuItemV1.Continue), Is.False);
                Assert.That(view.IsInteractable(HubMenuItemV1.NewGame), Is.True);
                Assert.That(view.IsInteractable(HubMenuItemV1.Settings), Is.True);
                Assert.That(view.IsInteractable(HubMenuItemV1.Quit), Is.True);
                Assert.Throws<ArgumentOutOfRangeException>(() => view.GetMenuItem(-1));
                Assert.Throws<ArgumentOutOfRangeException>(() => view.GetMenuItem(4));
                Assert.That(controller.TryActivate(HubMenuItemV1.Continue), Is.False);
                Assert.That(controller.State, Is.EqualTo(HubMenuControllerStateV1.Ready));
                AssertView(controller.CurrentView, false, HubMenuItemV1.NewGame);
            }
        }

        [TestCase(HubMenuItemV1.Continue)]
        [TestCase(HubMenuItemV1.NewGame)]
        [TestCase(HubMenuItemV1.Settings)]
        [TestCase(HubMenuItemV1.Quit)]
        public void AC003_OneInteractableActivationPublishesOneTakeOnly(HubMenuItemV1 item)
        {
            using (var fixture = Build(PrimaryObservation(8), Ordinary(ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit, ProfileAtomicSaveStage.DirectoryPreparation, null), null))
            {
                var controller = Create(fixture.Receipt, ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt));
                Assert.That(controller.TryActivate(item), Is.True);
                Assert.That(controller.TryActivate(HubMenuItemV1.Quit), Is.False);
                HubMenuIntentV1 taken;
                Assert.That(controller.TryTakeIntent(out taken), Is.True);
                Assert.That(taken.Item, Is.EqualTo(item));
                Assert.That(controller.TryTakeIntent(out taken), Is.False);
                Assert.That(taken.Equals(default(HubMenuIntentV1)), Is.True);
                Assert.That(controller.State, Is.EqualTo(HubMenuControllerStateV1.IntentConsumed));
            }
        }

        [Test]
        public void AC004_AC005_NotificationIsTopRightAndManualOnly()
        {
            using (var fixture = Build(PreviousObservation(3), Ordinary(ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain, ProfileAtomicSaveStage.PostCommitValidation, null), null))
            {
                var notice = ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt);
                Assert.That(notice.HasValue, Is.True);
                var controller = Create(fixture.Receipt, notice);
                Assert.That(controller.CurrentView.NotificationAnchor, Is.EqualTo(HubNotificationAnchorV1.TopRight));
                Assert.That(controller.CurrentView.NotificationState, Is.EqualTo(HubNoticeStateV1.Visible));
                Assert.That(controller.TryActivate(HubMenuItemV1.Settings), Is.True);
                Assert.That(controller.CurrentView.NotificationState, Is.EqualTo(HubNoticeStateV1.Visible));
                HubMenuIntentV1 intent;
                Assert.That(controller.TryTakeIntent(out intent), Is.True);
                Assert.That(controller.CurrentView.NotificationState, Is.EqualTo(HubNoticeStateV1.Visible));
                Assert.That(controller.TryDismissNotification(HubNoticeDismissalV1.Click), Is.True);
                Assert.That(controller.CurrentView.NotificationState, Is.EqualTo(HubNoticeStateV1.Dismissed));
                Assert.That(controller.TryDismissNotification(HubNoticeDismissalV1.Submit), Is.False);
            }
        }

        [Test]
        public void AC004_AbsentNotificationUsesCleanPrimaryReceipt()
        {
            AssertNotification(PrimaryObservation(4), Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, 4), null, false, null);
        }

        [Test]
        public void AC004_RecoveryCompletedNotificationUsesPreviousReceipt()
        {
            AssertNotification(PreviousObservation(4), Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, 5), null, true, ProfileLaunchNotificationKindV1.RecoveryCompleted);
        }

        [Test]
        public void AC004_PersistenceDeferredNotificationUsesFailedSaveReceipt()
        {
            AssertNotification(DefaultObservation(), Ordinary(ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit, ProfileAtomicSaveStage.DirectoryPreparation, null), null, true, ProfileLaunchNotificationKindV1.PersistenceDeferred);
        }

        [Test]
        public void AC004_PreservationFailureNotificationUsesPreservationOnlyReceipt()
        {
            AssertNotification(PreservationOnlyObservation(7), Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, 7), PreservationOnly(), true, ProfileLaunchNotificationKindV1.RecoveryArtifactPreservationFailed);
        }

        [Test]
        public void AC004_ForeignNotificationIsRejected()
        {
            using (var left = Build(DefaultObservation(), Ordinary(ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit, ProfileAtomicSaveStage.DirectoryPreparation, null), null))
            using (var right = Build(DefaultObservation(), Ordinary(ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit, ProfileAtomicSaveStage.DirectoryPreparation, null), null))
            {
                var foreign = ProfileLaunchNotificationV1.FromReceipt(right.Receipt.LaunchReceipt);
                Assert.Throws<InvalidOperationException>(() => Create(left.Receipt, foreign));
            }
        }

        [Test]
        public void AC004_MissingRequiredNotificationIsRejected()
        {
            using (var fixture = Build(DefaultObservation(), Ordinary(ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit, ProfileAtomicSaveStage.DirectoryPreparation, null), null))
            {
                Assert.Throws<InvalidOperationException>(() => Create(fixture.Receipt, null));
            }
        }

        [TestCase(HubNoticeDismissalV1.Click)]
        [TestCase(HubNoticeDismissalV1.Submit)]
        public void AC005_EachApprovedDismissalSourceClosesVisibleNoticeOnce(HubNoticeDismissalV1 source)
        {
            using (var fixture = Build(DefaultObservation(), Ordinary(ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit, ProfileAtomicSaveStage.DirectoryPreparation, null), null))
            {
                var controller = Create(fixture.Receipt, ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt));
                Assert.That(controller.TryDismissNotification(source), Is.True);
                Assert.That(controller.CurrentView.NotificationState, Is.EqualTo(HubNoticeStateV1.Dismissed));
                Assert.That(controller.TryDismissNotification(source), Is.False);
            }
        }

        [Test]
        public void AC005_UnknownCommandEnumsLatchFailedAndThrow()
        {
            using (var fixture = Build(DefaultObservation(), Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, 0), null))
            {
                var controller = Create(fixture.Receipt, ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt));
                Assert.Throws<ArgumentOutOfRangeException>(() => controller.TryActivate((HubMenuItemV1)99));
                Assert.That(controller.State, Is.EqualTo(HubMenuControllerStateV1.Failed));
            }
            using (var fixture = Build(DefaultObservation(), Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, 0), null))
            {
                var controller = Create(fixture.Receipt, ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt));
                Assert.Throws<ArgumentOutOfRangeException>(() => controller.TryDismissNotification((HubNoticeDismissalV1)99));
                Assert.That(controller.State, Is.EqualTo(HubMenuControllerStateV1.Failed));
            }
        }

        [Test]
        public void AC006_ReflectedViewAndControllerProofsFailClosedAtBoundary()
        {
            using (var fixture = Build(PrimaryObservation(9), Ordinary(ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit, ProfileAtomicSaveStage.DirectoryPreparation, null), null))
            {
                var controller = Create(fixture.Receipt, ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt));
                var field = typeof(HubMenuPresentationControllerV1).GetField("_view", BindingFlags.Instance | BindingFlags.NonPublic);
                var corrupt = controller.CurrentView;
                var backing = typeof(HubMenuViewV1).GetField("_logicalWidth", BindingFlags.Instance | BindingFlags.NonPublic);
                object boxed = corrupt; backing.SetValue(boxed, 641); field.SetValue(controller, boxed);
                Assert.Throws<InvalidOperationException>(() => { var ignored = controller.CurrentView; });
                Assert.That(controller.State, Is.EqualTo(HubMenuControllerStateV1.Failed));
            }
        }

        // REQ-M5D7O-006 / AC-M5D7O-006: each row has a fresh, real handoff and
        // controller.  The nested value loop remains one row because those
        // values have no mutable controller lifetime between their probes.
        [TestCaseSource(nameof(Ac006CorruptionCases))]
        public void AC006_AllControllerAndNestedProofBackingsFailClosed(string lane, string fieldName)
        {
            using (var fixture = Build(DefaultObservation(), Ordinary(ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit, ProfileAtomicSaveStage.DirectoryPreparation, null), null))
            {
                // AC-M5D7O-006: each immutable value validates its own backing
                // fields at its own next boundary.  Do not route every value
                // mutation through the complete M5D7N receipt graph: that
                // repeats an unrelated deep validation path for every field.
                if (lane == "value")
                {
                    var controller = Create(fixture.Receipt, ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt));
                    var view = controller.CurrentView;
                    foreach (var backing in typeof(HubMenuViewV1).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                        AssertCorruptViewBacking(view, backing);

                    var optionalNotification = ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt);
                    Assert.That(optionalNotification.HasValue, Is.True);
                    var notification = optionalNotification.Value;
                    foreach (var backing in typeof(ProfileLaunchNotificationV1).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                        AssertCorruptNotificationBacking(notification, backing);

                    var intent = new HubMenuIntentV1(HubMenuItemV1.NewGame);
                    foreach (var backing in typeof(HubMenuIntentV1).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                        AssertCorruptIntentBacking(intent, backing);
                    return;
                }

                if (lane == "ready")
                {
                    // Controller-owned receipt, notification, view, and state
                    // correlation must still fail via a controller boundary and
                    // latch Failed. The baseline is safe to shallow-clone: this
                    // controller holds only immutable value snapshots.
                    AssertCorruptControllerBacking(Create(fixture.Receipt, ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt)), fieldName);
                    return;
                }

                if (lane == "pending")
                {
                    var pending = Create(fixture.Receipt, ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt));
                    Assert.That(pending.TryActivate(HubMenuItemV1.NewGame), Is.True);
                    AssertCorruptControllerBacking(pending, fieldName);
                    return;
                }

                if (lane == "consumed")
                {
                    var consumed = Create(fixture.Receipt, ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt));
                    Assert.That(consumed.TryActivate(HubMenuItemV1.NewGame), Is.True);
                    HubMenuIntentV1 ignored;
                    Assert.That(consumed.TryTakeIntent(out ignored), Is.True);
                    AssertCorruptControllerBacking(consumed, fieldName);
                    return;
                }

                Assert.Fail("Unknown AC006 corruption lane: " + lane);
            }
        }

        private static IEnumerable<TestCaseData> Ac006CorruptionCases()
        {
            yield return new TestCaseData("value", null).SetName("AC006_NestedValueBackings");
            foreach (var fieldName in new[] { "_handoff", "_handoffProof", "_notification", "_notificationProof", "_persistedValidProfile", "_persistedValidProfileProof", "_notificationKind", "_notificationKindProof", "_view", "_viewProof", "_state", "_stateProof" })
                yield return new TestCaseData("ready", fieldName).SetName("AC006_Ready" + fieldName);
            foreach (var fieldName in new[] { "_intent", "_intentProof" })
                yield return new TestCaseData("pending", fieldName).SetName("AC006_Pending" + fieldName);
            foreach (var fieldName in new[] { "_consumedIntent", "_consumedIntentProof" })
                yield return new TestCaseData("consumed", fieldName).SetName("AC006_Consumed" + fieldName);
        }

        [Test]
        public void AC007_SourceHasNoForbiddenPresentationOrSideEffectAuthority()
        {
            var sourcePath = Path.Combine(Directory.GetParent(UnityEngine.Application.dataPath).FullName, "Assets", "AcadeGameMaker", "Runtime", "Input", "Unity", "HubMenuPresentationControllerV1.cs");
            var source = File.ReadAllText(sourcePath);
            foreach (var forbidden in new[] { "UnityEngine", "TMPro", "EventSystem", "SceneManager", "GameObject", "LoadScene", "RequestMode", "ProfileLaunchPreparationCoordinatorV1", "System.IO", "File.", "Directory.", "Debug.Log", "System.Net", "UnityEngine.Random", "ProfileAtomicSave", ".Save(", ".Preserve(", "UnityWebRequest", "HttpClient", "GameInputActions", "SetMode", "ConfigureFor", "Application.Quit", "Timer", "DateTime" })
                Assert.That(source, Does.Not.Contain(forbidden), forbidden);
        }

        private static HubMenuPresentationControllerV1 Create(HubEntryHandoffReceiptV1 handoff, ProfileLaunchNotificationV1? notification)
        {
            return HubMenuPresentationControllerV1.Create(handoff, notification);
        }

        private static void AssertNotification(ProfileLaunchObservationProjectionV1 observation, ProfileLaunchSaveProjectionV1 save, ProfileLaunchPreservationProjectionV1? preservation, bool expectedPresent, ProfileLaunchNotificationKindV1? expectedKind)
        {
            using (var fixture = Build(observation, save, preservation))
            {
                var notification = ProfileLaunchNotificationV1.FromReceipt(fixture.Receipt.LaunchReceipt);
                Assert.That(notification.HasValue, Is.EqualTo(expectedPresent));
                var controller = Create(fixture.Receipt, notification);
                Assert.That(controller.CurrentView.NotificationAnchor, Is.EqualTo(HubNotificationAnchorV1.TopRight));
                Assert.That(controller.CurrentView.NotificationKind, Is.EqualTo(expectedKind));
                Assert.That(controller.CurrentView.NotificationState, Is.EqualTo(expectedPresent ? HubNoticeStateV1.Visible : HubNoticeStateV1.Absent));
                if (notification.HasValue)
                {
                    Assert.That(notification.Value.Kind, Is.EqualTo(expectedKind.Value));
                    Assert.That(HubEntryHandoffReceiptV1.SameReceipt(notification.Value.Receipt, fixture.Receipt.LaunchReceipt), Is.True);
                }
            }
        }

        private static void AssertCorruptViewBacking(HubMenuViewV1 value, FieldInfo backing)
        {
            object boxed = value;
            backing.SetValue(boxed, Corrupt(backing, backing.GetValue(boxed)));
            var corrupt = (HubMenuViewV1)boxed;
            Assert.Throws<InvalidOperationException>(() => corrupt.Validate(), "view." + backing.Name);
        }

        private static void AssertCorruptNotificationBacking(ProfileLaunchNotificationV1 value, FieldInfo backing)
        {
            object boxed = value;
            backing.SetValue(boxed, Corrupt(backing, backing.GetValue(boxed)));
            var corrupt = (ProfileLaunchNotificationV1)boxed;
            Assert.Throws<InvalidOperationException>(() => corrupt.Validate(), "notification." + backing.Name);
        }

        private static void AssertCorruptIntentBacking(HubMenuIntentV1 value, FieldInfo backing)
        {
            object boxed = value;
            backing.SetValue(boxed, Corrupt(backing, backing.GetValue(boxed)));
            var corrupt = (HubMenuIntentV1)boxed;
            Assert.Throws<InvalidOperationException>(() => corrupt.Validate(), "intent." + backing.Name);
        }

        private static void AssertCorruptControllerBacking(HubMenuPresentationControllerV1 baseline, string name)
        {
            var controller = CloneController(baseline);
            var field = typeof(HubMenuPresentationControllerV1).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(controller, Corrupt(field, field.GetValue(controller)));
            Assert.Throws<InvalidOperationException>(() => { var view = controller.CurrentView; }, name);
            Assert.That(controller.State, Is.EqualTo(HubMenuControllerStateV1.Failed));
        }

        private static HubMenuPresentationControllerV1 CloneController(HubMenuPresentationControllerV1 baseline)
        {
            var clone = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            return (HubMenuPresentationControllerV1)clone.Invoke(baseline, null);
        }

        private static object Corrupt(FieldInfo field, object current)
        {
            if (field.FieldType == typeof(bool)) return !(bool)current;
            if (field.FieldType == typeof(int)) return ((int)current) + 1;
            if (field.FieldType.IsEnum) return Enum.ToObject(field.FieldType, 999);
            if (field.FieldType == typeof(HubEntryHandoffReceiptV1)) return default(HubEntryHandoffReceiptV1);
            if (field.FieldType == typeof(HubMenuViewV1)) return default(HubMenuViewV1);
            if (field.FieldType == typeof(ProfileLaunchNotificationV1?)) return null;
            if (field.FieldType == typeof(HubMenuIntentV1?)) return null;
            if (field.FieldType == typeof(ProfileLaunchNotificationKindV1?)) return null;
            return field.FieldType.IsValueType ? Activator.CreateInstance(field.FieldType) : null;
        }

        private static void AssertView(HubMenuViewV1 view, bool persisted, HubMenuItemV1 focus)
        {
            Assert.That(view.HasPersistedValidProfile, Is.EqualTo(persisted));
            Assert.That(view.ContinueInteractable, Is.EqualTo(persisted));
            Assert.That(view.InitialFocus, Is.EqualTo(focus));
            Assert.That(view.LogicalWidth, Is.EqualTo(640)); Assert.That(view.LogicalHeight, Is.EqualTo(360));
            Assert.That(view.MinimumBodyPx, Is.EqualTo(12)); Assert.That(view.StandardBodyPx, Is.EqualTo(14)); Assert.That(view.HeadingPx, Is.EqualTo(18));
            Assert.That(view.MinimumHitPx, Is.EqualTo(24)); Assert.That(view.MinimumEdgeMarginPx, Is.EqualTo(12));
        }

        private static Fixture Build(ProfileLaunchObservationProjectionV1 observation, ProfileLaunchSaveProjectionV1 save, ProfileLaunchPreservationProjectionV1? preservation)
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-M5D7O-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            var actions = new ActionsPort();
            var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, new CoordinatorPort(observation, preservation, save), actions);
            return new Fixture(root, prepared, new HubEntryHandoffReceiptV1(DesktopProfileLaunchReceiptV1.FromPrepared(prepared).WithHubProof()));
        }

        private static ProfileLaunchObservationProjectionV1 PrimaryObservation(long revision)
        {
            return new ProfileLaunchObservationProjectionV1(Candidate(ProfileLoadFileRole.Primary, Document(revision)), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp));
        }

        private static ProfileLaunchObservationProjectionV1 PreviousObservation(long revision)
        {
            return new ProfileLaunchObservationProjectionV1(Missing(ProfileLoadFileRole.Primary), Candidate(ProfileLoadFileRole.Previous, Document(revision)), Missing(ProfileLoadFileRole.Temp));
        }

        private static ProfileLaunchObservationProjectionV1 DefaultObservation()
        {
            return new ProfileLaunchObservationProjectionV1(Missing(ProfileLoadFileRole.Primary), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp));
        }

        private static ProfileLaunchObservationProjectionV1 DefaultDeniedObservation()
        {
            return new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Unreadable(ProfileLoadFileRole.Primary), Missing(ProfileLoadFileRole.Previous), Missing(ProfileLoadFileRole.Temp));
        }

        private static ProfileLaunchObservationProjectionV1 PreservationOnlyObservation(long revision)
        {
            var invalidTemp = ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Temp, ProfileCanonicalDecoderV1.Decode(Encoding.UTF8.GetBytes("{}")));
            return new ProfileLaunchObservationProjectionV1(Candidate(ProfileLoadFileRole.Primary, Document(revision)), Missing(ProfileLoadFileRole.Previous), invalidTemp);
        }

        private static ProfileLaunchPreservationProjectionV1 PreservationDenied()
        {
            return new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.FailedBeforeMove, ProfileLaunchPreservationStateV1.NotRequired, false, ProfileLoadFileRole.Primary, ProfileFilePreservationReason.None, ProfileFilePreservationReason.UnreadablePrimary, ProfileFilePreservationReason.None, 1);
        }

        private static ProfileLaunchPreservationProjectionV1 PreservationOnly()
        {
            return new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.MoveOutcomeUncertain, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, false, ProfileLoadFileRole.Temp, ProfileFilePreservationReason.StaleTemp, ProfileFilePreservationReason.None, ProfileFilePreservationReason.None, 1);
        }

        private static ProfileLaunchSaveProjectionV1 Ordinary(ProfileLaunchPersistenceOutcomeV1 outcome, ProfileAtomicSaveStage stage, long? revision)
        {
            if (outcome == ProfileLaunchPersistenceOutcomeV1.CommittedFirst)
                return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, outcome, stage, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, ProfileStoredFileState.Missing, revision);
            if (outcome == ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit)
                return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, outcome, stage, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, null);
            return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, outcome, stage, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, null);
        }

        private static ProfileCanonicalDocumentV1 Document(long revision)
        {
            var input = new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, ProfileInputContract.CurrentBindingSchemaVersion, ProfileBindingOverridesJson.Parse(string.Empty));
            var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), input, new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
            return ProfileCanonicalEncoder.Encode(snapshot);
        }

        private static ProfileLoadCandidateV1 Candidate(ProfileLoadFileRole role, ProfileCanonicalDocumentV1 document)
        {
            return ProfileLoadCandidateV1.Decoded(role, ProfileCanonicalDecoderV1.Decode(document.CanonicalFileBytes));
        }
        private static ProfileLoadCandidateV1 Missing(ProfileLoadFileRole role) { return ProfileLoadCandidateV1.Missing(role); }

        private sealed class Fixture : IDisposable
        {
            private readonly string _root; private readonly PreparedProfileLaunchV1 _prepared;
            internal Fixture(string root, PreparedProfileLaunchV1 prepared, HubEntryHandoffReceiptV1 receipt) { _root = root; _prepared = prepared; Receipt = receipt; }
            internal HubEntryHandoffReceiptV1 Receipt { get; private set; }
            public void Dispose() { _prepared.Dispose(); if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        }

        private sealed class CoordinatorPort : IProfileLaunchPreparationProfilePortV1
        {
            private readonly ProfileLaunchObservationProjectionV1 _observation; private readonly ProfileLaunchPreservationProjectionV1? _preservation; private readonly ProfileLaunchSaveProjectionV1 _save;
            internal CoordinatorPort(ProfileLaunchObservationProjectionV1 observation, ProfileLaunchPreservationProjectionV1? preservation, ProfileLaunchSaveProjectionV1 save) { _observation = observation; _preservation = preservation; _save = save; }
            public ProfileLaunchObservationProjectionV1 Observe(string root) { return _observation; }
            public ProfileLaunchPreservationProjectionV1 Preserve(string root, ProfileLaunchObservationProjectionV1 observation, ProfileLoadSelectionPlanV1 selection, DateTimeOffset utc)
            {
                return _preservation ?? new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, true, null, selection.TempPreservation, selection.PrimaryPreservation, selection.PreviousPreservation, 0);
            }
            public ProfileLaunchSaveProjectionV1 Save(string root, ProfileCanonicalDocumentV1 document) { return _save; }
            public ProfileLaunchSaveProjectionV1 SaveInputRecovery(string root, ProfileLoadCandidateV1 primary) { return _save; }
        }

        private sealed class ActionsPort : IProfileLaunchPreparationActionsPortV1
        {
            public GameInputActions Create() { return new GameInputActions(); }
            public ProfileBindingOverrideApplyResultV1 Apply(ProfileInputSnapshot input, GameInputActions candidate)
            {
                return new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.DefaultsReady, ProfileBindingOverrideApplyFailureV1.None, ProfileBindingOverrideCandidateDispositionV1.Retain, string.Empty, string.Empty);
            }
            public void Dispose(GameInputActions candidate)
            {
                // AC-M5D7O-001: the injected coordinator seam still owns a
                // generated actions asset. EditMode cleanup must be immediate;
                // GameInputActions.Dispose uses delayed Destroy by design.
                candidate.asset.Disable();
                UnityEngine.Object.DestroyImmediate(candidate.asset);
            }
        }
    }
}
