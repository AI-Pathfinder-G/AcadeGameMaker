using System;
using AcadeGameMaker.Profile;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-M5D7O-003..005: closed, engine-free hub-menu presentation vocabulary.
    internal enum HubMenuItemV1 { Continue = 1, NewGame = 2, Settings = 3, Quit = 4 }
    internal enum HubNoticeDismissalV1 { Click = 1, Submit = 2 }
    internal enum HubMenuControllerStateV1 { Ready = 1, IntentPending = 2, IntentConsumed = 3, Failed = 4 }
    internal enum HubNoticeStateV1 { Absent = 1, Visible = 2, Dismissed = 3 }
    internal enum HubNotificationAnchorV1 { TopRight = 1 }

    // REQ-M5D7O-001..005/007: immutable, defensive presentation snapshot.
    internal readonly struct HubMenuViewV1
    {
        private readonly bool _hasPersistedValidProfile, _continueInteractable;
        private readonly HubMenuItemV1 _initialFocus;
        private readonly ProfileLaunchNotificationKindV1? _notificationKind;
        private readonly HubNoticeStateV1 _notificationState;
        private readonly HubNotificationAnchorV1 _notificationAnchor;
        private readonly int _logicalWidth, _logicalHeight, _minimumBodyPx, _standardBodyPx, _headingPx, _minimumHitPx, _minimumEdgeMarginPx;
        private readonly bool _hasPersistedValidProfileProof, _continueInteractableProof;
        private readonly HubMenuItemV1 _initialFocusProof;
        private readonly ProfileLaunchNotificationKindV1? _notificationKindProof;
        private readonly HubNoticeStateV1 _notificationStateProof;
        private readonly HubNotificationAnchorV1 _notificationAnchorProof;
        private readonly int _logicalWidthProof, _logicalHeightProof, _minimumBodyPxProof, _standardBodyPxProof, _headingPxProof, _minimumHitPxProof, _minimumEdgeMarginPxProof;

        internal HubMenuViewV1(bool hasPersistedValidProfile, ProfileLaunchNotificationKindV1? notificationKind, HubNoticeStateV1 notificationState)
        {
            _hasPersistedValidProfile = hasPersistedValidProfile;
            _continueInteractable = hasPersistedValidProfile;
            _initialFocus = hasPersistedValidProfile ? HubMenuItemV1.Continue : HubMenuItemV1.NewGame;
            _notificationKind = notificationKind;
            _notificationState = notificationState;
            _notificationAnchor = HubNotificationAnchorV1.TopRight;
            _logicalWidth = 640; _logicalHeight = 360;
            _minimumBodyPx = 12; _standardBodyPx = 14; _headingPx = 18;
            _minimumHitPx = 24; _minimumEdgeMarginPx = 12;
            _hasPersistedValidProfileProof = _hasPersistedValidProfile;
            _continueInteractableProof = _continueInteractable;
            _initialFocusProof = _initialFocus;
            _notificationKindProof = _notificationKind;
            _notificationStateProof = _notificationState;
            _notificationAnchorProof = _notificationAnchor;
            _logicalWidthProof = _logicalWidth; _logicalHeightProof = _logicalHeight;
            _minimumBodyPxProof = _minimumBodyPx; _standardBodyPxProof = _standardBodyPx; _headingPxProof = _headingPx;
            _minimumHitPxProof = _minimumHitPx; _minimumEdgeMarginPxProof = _minimumEdgeMarginPx;
            Validate();
        }

        internal bool HasPersistedValidProfile { get { Validate(); return _hasPersistedValidProfile; } }
        internal bool ContinueInteractable { get { Validate(); return _continueInteractable; } }
        internal HubMenuItemV1 InitialFocus { get { Validate(); return _initialFocus; } }
        internal ProfileLaunchNotificationKindV1? NotificationKind { get { Validate(); return _notificationKind; } }
        internal HubNoticeStateV1 NotificationState { get { Validate(); return _notificationState; } }
        internal HubNotificationAnchorV1 NotificationAnchor { get { Validate(); return _notificationAnchor; } }
        internal int LogicalWidth { get { Validate(); return _logicalWidth; } }
        internal int LogicalHeight { get { Validate(); return _logicalHeight; } }
        internal int MinimumBodyPx { get { Validate(); return _minimumBodyPx; } }
        internal int StandardBodyPx { get { Validate(); return _standardBodyPx; } }
        internal int HeadingPx { get { Validate(); return _headingPx; } }
        internal int MinimumHitPx { get { Validate(); return _minimumHitPx; } }
        internal int MinimumEdgeMarginPx { get { Validate(); return _minimumEdgeMarginPx; } }

        internal HubMenuItemV1 GetMenuItem(int index)
        {
            Validate();
            switch (index)
            {
                case 0: return HubMenuItemV1.Continue;
                case 1: return HubMenuItemV1.NewGame;
                case 2: return HubMenuItemV1.Settings;
                case 3: return HubMenuItemV1.Quit;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        internal bool IsInteractable(HubMenuItemV1 item)
        {
            Validate();
            if (!IsKnownMenuItem(item)) throw new ArgumentOutOfRangeException(nameof(item));
            return item != HubMenuItemV1.Continue || _continueInteractable;
        }

        internal HubMenuViewV1 WithDismissedNotice()
        {
            Validate();
            if (!_notificationKind.HasValue || _notificationState != HubNoticeStateV1.Visible)
                throw new InvalidOperationException("Hub notice cannot be dismissed from its current state.");
            return new HubMenuViewV1(_hasPersistedValidProfile, _notificationKind, HubNoticeStateV1.Dismissed);
        }

        internal void Validate()
        {
            if (_hasPersistedValidProfile != _hasPersistedValidProfileProof || _continueInteractable != _continueInteractableProof || _initialFocus != _initialFocusProof || _notificationKind != _notificationKindProof || _notificationState != _notificationStateProof || _notificationAnchor != _notificationAnchorProof || _logicalWidth != _logicalWidthProof || _logicalHeight != _logicalHeightProof || _minimumBodyPx != _minimumBodyPxProof || _standardBodyPx != _standardBodyPxProof || _headingPx != _headingPxProof || _minimumHitPx != _minimumHitPxProof || _minimumEdgeMarginPx != _minimumEdgeMarginPxProof)
                throw new InvalidOperationException("Hub menu view proof is malformed.");
            if (_continueInteractable != _hasPersistedValidProfile || _initialFocus != (_continueInteractable ? HubMenuItemV1.Continue : HubMenuItemV1.NewGame))
                throw new InvalidOperationException("Hub menu availability is malformed.");
            if (_notificationAnchor != HubNotificationAnchorV1.TopRight || _logicalWidth != 640 || _logicalHeight != 360 || _minimumBodyPx != 12 || _standardBodyPx != 14 || _headingPx != 18 || _minimumHitPx != 24 || _minimumEdgeMarginPx != 12)
                throw new InvalidOperationException("Hub menu presentation constants are malformed.");
            if (_notificationKind.HasValue)
            {
                if (!IsKnownNotificationKind(_notificationKind.Value) || (_notificationState != HubNoticeStateV1.Visible && _notificationState != HubNoticeStateV1.Dismissed))
                    throw new InvalidOperationException("Hub notification state is malformed.");
            }
            else if (_notificationState != HubNoticeStateV1.Absent)
                throw new InvalidOperationException("Hub notification absence is malformed.");
        }

        internal static bool IsKnownMenuItem(HubMenuItemV1 value)
        {
            return value == HubMenuItemV1.Continue || value == HubMenuItemV1.NewGame || value == HubMenuItemV1.Settings || value == HubMenuItemV1.Quit;
        }

        internal static bool IsKnownNotificationKind(ProfileLaunchNotificationKindV1 value)
        {
            return value == ProfileLaunchNotificationKindV1.RecoveryCompleted || value == ProfileLaunchNotificationKindV1.RecoveryArtifactPreservationFailed || value == ProfileLaunchNotificationKindV1.PersistenceDeferred;
        }
    }

    // REQ-M5D7O-006/007: one exact command, with an independent proof field.
    internal readonly struct HubMenuIntentV1
    {
        private readonly HubMenuItemV1 _item, _itemProof;
        internal HubMenuIntentV1(HubMenuItemV1 item)
        {
            _item = item; _itemProof = item; Validate();
        }
        internal HubMenuItemV1 Item { get { Validate(); return _item; } }
        internal void Validate()
        {
            if (_item != _itemProof || !HubMenuViewV1.IsKnownMenuItem(_item)) throw new InvalidOperationException("Hub menu intent is malformed.");
        }
    }

    // REQ-M5D7O-001..008: engine-free, side-effect-free presentation/session core.
    internal sealed class HubMenuPresentationControllerV1
    {
        private HubEntryHandoffReceiptV1 _handoff, _handoffProof;
        private ProfileLaunchNotificationV1? _notification, _notificationProof;
        // REQ-M5D7O-001/002/007: Create closes the deep M5D7N boundary once.
        // These scalar snapshots are the only source-derived facts used after
        // ownership has transferred; each has an exact local proof.
        private bool _persistedValidProfile, _persistedValidProfileProof;
        private ProfileLaunchNotificationKindV1? _notificationKind, _notificationKindProof;
        private HubMenuViewV1 _view, _viewProof;
        private HubMenuIntentV1? _intent, _intentProof, _consumedIntent, _consumedIntentProof;
        private HubMenuControllerStateV1 _state, _stateProof;

        private HubMenuPresentationControllerV1(HubEntryHandoffReceiptV1 handoff, ProfileLaunchNotificationV1? notification, bool persistedValidProfile, ProfileLaunchNotificationKindV1? notificationKind)
        {
            // Create has already closed the incoming boundary. Construct only
            // copied snapshots; later controller boundaries intentionally do
            // not re-enter the deep M5D7N receipt/notification validation graph.
            _handoff = handoff; _handoffProof = handoff;
            _notification = notification; _notificationProof = notification;
            _persistedValidProfile = persistedValidProfile; _persistedValidProfileProof = persistedValidProfile;
            _notificationKind = notificationKind; _notificationKindProof = notificationKind;
            _view = new HubMenuViewV1(persistedValidProfile, notificationKind, notificationKind.HasValue ? HubNoticeStateV1.Visible : HubNoticeStateV1.Absent);
            _viewProof = _view;
            _intent = null; _intentProof = null; _consumedIntent = null; _consumedIntentProof = null;
            _state = HubMenuControllerStateV1.Ready; _stateProof = _state;
            ValidateOperational();
        }

        internal static HubMenuPresentationControllerV1 Create(HubEntryHandoffReceiptV1 handoff, ProfileLaunchNotificationV1? notification)
        {
            // REQ-M5D7O-001/007: the incoming M5D7N value and optional transfer
            // are validated once at the ownership boundary before any projection.
            handoff.Validate();
            var receipt = handoff.LaunchReceipt;
            ProfileLaunchNotificationV1? expected = ProfileLaunchNotificationV1.FromReceipt(receipt);
            var source = receipt.Source;
            var persisted = source == ProfileLaunchSource.Primary || source == ProfileLaunchSource.Previous;
            var notificationKind = expected.HasValue ? expected.Value.Kind : (ProfileLaunchNotificationKindV1?)null;
            if (notification.HasValue)
            {
                notification.Value.Validate();
                if (!expected.HasValue || !notification.Value.Equals(expected.Value))
                    throw new InvalidOperationException("Hub notification does not exactly correlate to its handoff.");
            }
            else if (expected.HasValue)
                throw new InvalidOperationException("Hub notification transfer is missing.");
            return new HubMenuPresentationControllerV1(handoff, notification, persisted, notificationKind);
        }

        internal HubMenuControllerStateV1 State
        {
            get
            {
                try { ValidateState(); return _state; }
                catch { LatchFailed(); throw; }
            }
        }

        internal HubMenuViewV1 CurrentView
        {
            get { ValidateOperational(); return _view; }
        }

        internal bool TryDismissNotification(HubNoticeDismissalV1 source)
        {
            ValidateOperational();
            try
            {
                if (source != HubNoticeDismissalV1.Click && source != HubNoticeDismissalV1.Submit)
                    throw new ArgumentOutOfRangeException(nameof(source));
                if (_view.NotificationState != HubNoticeStateV1.Visible) return false;
                _view = _view.WithDismissedNotice(); _viewProof = _view;
                ValidateOperational();
                return true;
            }
            catch { LatchFailed(); throw; }
        }

        internal bool TryActivate(HubMenuItemV1 item)
        {
            ValidateOperational();
            try
            {
                if (!HubMenuViewV1.IsKnownMenuItem(item)) throw new ArgumentOutOfRangeException(nameof(item));
                if (_state != HubMenuControllerStateV1.Ready) return false;
                if (!_view.IsInteractable(item)) return false;
                var intent = new HubMenuIntentV1(item);
                _intent = intent; _intentProof = intent;
                _state = HubMenuControllerStateV1.IntentPending; _stateProof = _state;
                ValidateOperational();
                return true;
            }
            catch { LatchFailed(); throw; }
        }

        internal bool TryTakeIntent(out HubMenuIntentV1 intent)
        {
            ValidateOperational();
            try
            {
                if (_state != HubMenuControllerStateV1.IntentPending)
                {
                    intent = default(HubMenuIntentV1);
                    return false;
                }
                if (!_intent.HasValue || !_intentProof.HasValue || !_intent.Value.Equals(_intentProof.Value))
                    throw new InvalidOperationException("Hub menu pending intent is malformed.");
                intent = _intent.Value;
                intent.Validate();
                _consumedIntent = intent; _consumedIntentProof = intent;
                _intent = null; _intentProof = null;
                _state = HubMenuControllerStateV1.IntentConsumed; _stateProof = _state;
                ValidateOperational();
                return true;
            }
            catch
            {
                LatchFailed();
                intent = default(HubMenuIntentV1);
                throw;
            }
        }

        private void ValidateOperational()
        {
            try
            {
                ValidateState();
                if (_state == HubMenuControllerStateV1.Failed) throw new InvalidOperationException("Hub menu controller is failed.");
            }
            catch { LatchFailed(); throw; }
        }

        private void ValidateState()
        {
            if (_state < HubMenuControllerStateV1.Ready || _state > HubMenuControllerStateV1.Failed || _state != _stateProof)
                throw new InvalidOperationException("Hub menu controller state is malformed.");
            if (_state == HubMenuControllerStateV1.Failed) return;
            _handoff.Validate(); _handoffProof.Validate();
            if (!_handoff.Equals(_handoffProof)) throw new InvalidOperationException("Hub menu handoff proof is malformed.");
            if (_notification.HasValue != _notificationProof.HasValue || (_notification.HasValue && !_notification.Value.Equals(_notificationProof.Value)))
                throw new InvalidOperationException("Hub menu notification proof is malformed.");
            if (_persistedValidProfile != _persistedValidProfileProof)
                throw new InvalidOperationException("Hub menu persisted-profile projection is malformed.");
            if (_notificationKind != _notificationKindProof || _notification.HasValue != _notificationKind.HasValue)
                throw new InvalidOperationException("Hub menu notification projection proof is malformed.");
            if (_notificationKind.HasValue && !HubMenuViewV1.IsKnownNotificationKind(_notificationKind.Value))
                throw new InvalidOperationException("Hub menu notification kind is malformed.");
            _view.Validate(); _viewProof.Validate();
            if (!_view.Equals(_viewProof)) throw new InvalidOperationException("Hub menu view proof is malformed.");
            if (_view.HasPersistedValidProfile != _persistedValidProfile || _view.ContinueInteractable != _persistedValidProfile || _view.InitialFocus != (_persistedValidProfile ? HubMenuItemV1.Continue : HubMenuItemV1.NewGame))
                throw new InvalidOperationException("Hub menu profile projection is malformed.");
            if (_view.NotificationKind != _notificationKind)
                throw new InvalidOperationException("Hub menu notification projection is malformed.");
            ValidateOptionalIntent(_intent, _intentProof, "pending");
            ValidateOptionalIntent(_consumedIntent, _consumedIntentProof, "consumed");
            if (_state == HubMenuControllerStateV1.Ready)
            {
                if (_intent.HasValue || _consumedIntent.HasValue) throw new InvalidOperationException("Hub menu ready state is malformed.");
            }
            else if (_state == HubMenuControllerStateV1.IntentPending)
            {
                if (!_intent.HasValue || _consumedIntent.HasValue) throw new InvalidOperationException("Hub menu pending state is malformed.");
            }
            else if (_state == HubMenuControllerStateV1.IntentConsumed)
            {
                if (_intent.HasValue || !_consumedIntent.HasValue) throw new InvalidOperationException("Hub menu consumed state is malformed.");
            }
        }

        private static void ValidateOptionalIntent(HubMenuIntentV1? value, HubMenuIntentV1? proof, string label)
        {
            if (value.HasValue != proof.HasValue || (value.HasValue && !value.Value.Equals(proof.Value)))
                throw new InvalidOperationException("Hub menu " + label + " intent proof is malformed.");
            if (value.HasValue) value.Value.Validate();
        }

        private void LatchFailed()
        {
            _state = HubMenuControllerStateV1.Failed;
            _stateProof = HubMenuControllerStateV1.Failed;
        }
    }
}
