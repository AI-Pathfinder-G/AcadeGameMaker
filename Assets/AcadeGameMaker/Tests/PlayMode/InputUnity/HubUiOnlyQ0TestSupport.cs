using System;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Profile;
using UnityEngine;

namespace AcadeGameMaker.Input.Unity.PlayMode.Tests
{
    // REQ-M5D7Q0-002..011. Q0-only reflection support; it never changes a
    // verified M5D7M/N fixture or product source and keeps matrix rows terse.
    internal static class HubUiOnlyQ0TestSupport
    {
        internal static T Get<T>(object target, string field) => (T)target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
        internal static void Set(object target, string field, object value) => target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
        internal static void Invoke(object target, string method) => target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);
        // The generated wrapper only verifies map disablement in its finalizer.
        // Q0 reflection rows deliberately keep substituted wrappers caller-owned,
        // so their cleanup belongs to the fixture rather than router teardown.
        internal static void DisposeCallerOwnedActions(GameInputActions actions)
        {
            if (actions == null) return;
            actions.Gameplay.Disable();
            actions.UI.Disable();
            actions.Dispose();
        }
        internal static void AssertNoForbiddenComponent(GameObject root)
        {
            foreach (var component in root.GetComponents<Component>())
                if (component != null && component.GetType().Name.IndexOf("Canvas", StringComparison.OrdinalIgnoreCase) >= 0)
                    throw new InvalidOperationException("Q0 hub owns forbidden presentation.");
        }

        // AC-M5D7Q0-002. This is deliberately local to the Q0 suite: the
        // real coordinator owns creation, binding evidence, and token disposal
        // while the fixture retains exact candidate identity for the adapter
        // ownership rows. It is not a product action factory.
        internal sealed class RecordingActionsPortV1 : IProfileLaunchPreparationActionsPortV1
        {
            internal readonly List<GameInputActions> Created = new List<GameInputActions>();
            internal readonly List<GameInputActions> Applied = new List<GameInputActions>();
            internal readonly List<GameInputActions> Disposed = new List<GameInputActions>();

            public GameInputActions Create()
            {
                var candidate = new GameInputActions();
                Created.Add(candidate);
                return candidate;
            }

            public ProfileBindingOverrideApplyResultV1 Apply(ProfileInputSnapshot input, GameInputActions candidate)
            {
                if (input.BindingOverrides.CanonicalText != string.Empty)
                    throw new InvalidOperationException("Q0 recorder requires the default binding row.");
                Applied.Add(candidate);
                return new ProfileBindingOverrideApplyResultV1(
                    ProfileBindingOverrideApplyOutcomeV1.DefaultsReady,
                    ProfileBindingOverrideApplyFailureV1.None,
                    ProfileBindingOverrideCandidateDispositionV1.Retain,
                    string.Empty,
                    string.Empty);
            }

            public void Dispose(GameInputActions candidate)
            {
                if (candidate == null) throw new ArgumentNullException(nameof(candidate));
                Disposed.Add(candidate);
                DisposeCallerOwnedActions(candidate);
            }

            // The adapter's post-Take boundary intentionally owns direct
            // disposal, rather than calling the token's port. Record it only
            // after observing Unity's destroyed asset, so this never merely
            // predicts a disposal call.
            internal void ObserveAdapterDisposal(GameInputActions candidate)
            {
                if (candidate == null || candidate.asset != null)
                    throw new InvalidOperationException("Adapter did not dispose the rejected candidate.");
                Disposed.Add(candidate);
            }
        }

        internal sealed class RecordingCoordinatorPreparationV1 : IDesktopProfileLaunchPreparationV1
        {
            private readonly IProfileLaunchPreparationProfilePortV1 _profile = new DefaultProfilePortV1();
            internal RecordingActionsPortV1 Actions { get; } = new RecordingActionsPortV1();
            internal IReadOnlyList<GameInputActions> Created => Actions.Created;
            internal IReadOnlyList<GameInputActions> Applied => Actions.Applied;
            internal IReadOnlyList<GameInputActions> Disposed => Actions.Disposed;
            internal PreparedProfileLaunchV1 Prepared { get; private set; }
            internal InputRouter Router { get; set; }
            internal object Owner { get; set; }
            internal bool PreConsumeTake { get; set; }
            internal bool RejectAdopt { get; set; }

            public PreparedProfileLaunchV1 Prepare(string root, DateTimeOffset utc)
            {
                var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, utc, _profile, Actions);
                Prepared = prepared;
                if (PreConsumeTake)
                {
                    var candidate = prepared.TakeActions();
                    Actions.Dispose(candidate);
                }
                if (RejectAdopt)
                {
                    if (Router == null || Owner == null) throw new InvalidOperationException("Q0 adopt rejection was not armed.");
                    Router.FailPreparedHubLaunch(Owner);
                }
                return prepared;
            }
        }

        private sealed class DefaultProfilePortV1 : IProfileLaunchPreparationProfilePortV1
        {
            public ProfileLaunchObservationProjectionV1 Observe(string root)
            {
                return new ProfileLaunchObservationProjectionV1(
                    ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary),
                    ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),
                    ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp));
            }

            public ProfileLaunchPreservationProjectionV1 Preserve(string root, ProfileLaunchObservationProjectionV1 observation, ProfileLoadSelectionPlanV1 selection, DateTimeOffset utc)
            {
                return new ProfileLaunchPreservationProjectionV1(
                    ProfileLaunchPreservationStateV1.NotRequired,
                    ProfileLaunchPreservationStateV1.NotRequired,
                    ProfileLaunchPreservationStateV1.NotRequired,
                    true,
                    null,
                    selection.TempPreservation,
                    selection.PrimaryPreservation,
                    selection.PreviousPreservation,
                    0);
            }

            public ProfileLaunchSaveProjectionV1 Save(string root, ProfileCanonicalDocumentV1 document)
            {
                return new ProfileLaunchSaveProjectionV1(
                    ProfileLaunchSaveKindV1.Ordinary,
                    ProfileLaunchPersistenceOutcomeV1.CommittedFirst,
                    ProfileAtomicSaveStage.None,
                    ProfileStoredFileState.ValidCurrentInput,
                    ProfileStoredFileState.Missing,
                    ProfileStoredFileState.Missing,
                    document.Snapshot.ProfileRevision);
            }

            public ProfileLaunchSaveProjectionV1 SaveInputRecovery(string root, ProfileLoadCandidateV1 primary)
            {
                throw new InvalidOperationException("The Q0 default row does not repair input.");
            }
        }
    }
}
