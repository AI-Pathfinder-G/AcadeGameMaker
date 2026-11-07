using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Profile;
using UnityEngine;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-M5D7N-001..008: authored-cohort, first-Update-only hub entry handoff.
    [DefaultExecutionOrder(-190)]
    [DisallowMultipleComponent]
    internal sealed class HubEntryHandoffLatchV1 : MonoBehaviour
    {
        [SerializeField] private DesktopProfileLaunchAdapterV1 _launchAdapter;
        [SerializeField] private InputRouter _router;
        private HubEntryHandoffStateV1 _state;
        private HubEntryHandoffReceiptV1? _handoff;
        private HubEntryHandoffReceiptV1? _handoffProof;
        private ProfileLaunchNotificationV1? _notification;
        private ProfileLaunchNotificationV1? _ownedNotificationProof;
        // REQ-M5D7N-004: immutable local publication history.  A taken source
        // payload is retained separately until it is either published or terminally
        // rejected; it is never silently discarded after ownership transfers.
        private bool _expectedNotification;
        private bool _sourceNotificationProbed;
        private bool _sourceNotificationTaken;
        private bool _localNotificationDelivered;
        private ProfileLaunchNotificationV1? _retainedFailureNotification;

        // REQ-M5D7Q0-009. Builder-only typed assignment/read seams.  They
        // perform no lifecycle, source read, notification take, or handoff.
        internal DesktopProfileLaunchAdapterV1 BoundLaunchAdapterForAuthoring => _launchAdapter;
        internal InputRouter BoundRouterForAuthoring => _router;
        internal void ConfigureHubUiOnlyForAuthoring(DesktopProfileLaunchAdapterV1 launchAdapter, InputRouter router)
        {
            if (launchAdapter == null || router == null || gameObject.activeInHierarchy || _state != HubEntryHandoffStateV1.Pristine ||
                _launchAdapter != null || _router != null || _handoff.HasValue || _handoffProof.HasValue || _notification.HasValue)
                throw new InvalidOperationException("Hub handoff authoring seam requires a pristine inactive component.");
            _launchAdapter = launchAdapter;
            _router = router;
        }

        internal HubEntryHandoffReceiptV1? CurrentHandoff
        {
            get { ValidateStateOrFail(); return _handoff; }
        }

        internal bool TryTakeLaunchNotification(out ProfileLaunchNotificationV1 notification)
        {
            ValidateStateOrFail();
            try
            {
                if (_state == HubEntryHandoffStateV1.PublishedNoNotice || _state == HubEntryHandoffStateV1.PublishedNoticeConsumed)
                {
                    notification = default(ProfileLaunchNotificationV1);
                    ValidateStateOrFail();
                    return false;
                }
                if (_state != HubEntryHandoffStateV1.PublishedNoticePending)
                    throw new InvalidOperationException("Hub launch notification cannot be taken in this state.");

                if (!_notification.HasValue || !_ownedNotificationProof.HasValue || !_notification.Value.Equals(_ownedNotificationProof.Value))
                    throw new InvalidOperationException("Hub launch notification publication is malformed.");
                notification = _notification.Value;

                _notification = null;
                _ownedNotificationProof = null;
                _localNotificationDelivered = true;
                _state = HubEntryHandoffStateV1.PublishedNoticeConsumed;
                ValidateStateOrFail();
                return true;
            }
            catch
            {
                _state = HubEntryHandoffStateV1.Failed;
                throw;
            }
        }

        private void Awake()
        {
            ValidateStateOrFail();
            try
            {
                if (_state != HubEntryHandoffStateV1.Pristine)
                    throw new InvalidOperationException("Hub handoff Awake cannot repeat.");
                ValidateTopology();
                _state = HubEntryHandoffStateV1.AwaitingStart;
                ValidateStateOrFail();
            }
            catch
            {
                _state = HubEntryHandoffStateV1.Failed;
                throw;
            }
        }

        private void Start()
        {
            ValidateStateOrFail();
            try
            {
                if (_state != HubEntryHandoffStateV1.AwaitingStart)
                    throw new InvalidOperationException("Hub handoff Start is malformed.");
                // REQ-M5D7N-001/006: authored topology remains authoritative until
                // publication; a reflected or disabled replacement cannot defer its
                // rejection until after a source notification is consumed.
                ValidateTopology();
                _state = HubEntryHandoffStateV1.AwaitingFirstUpdate;
                ValidateStateOrFail();
            }
            catch
            {
                _state = HubEntryHandoffStateV1.Failed;
                throw;
            }
        }

        private void Update()
        {
            ValidateStateOrFail();
            if (_state == HubEntryHandoffStateV1.Failed || _state == HubEntryHandoffStateV1.ClosedBeforePublication || IsPublished(_state))
                return; // REQ-M5D7N-006: a terminal latch never rereads its source.
            try
            {
                if (_state != HubEntryHandoffStateV1.AwaitingFirstUpdate)
                    throw new InvalidOperationException("Hub handoff Update occurred before Start.");
                ValidateTopology();
                var receipt = _launchAdapter.CurrentReceipt;
                if (!receipt.HasValue) throw new InvalidOperationException("Hub handoff source receipt is absent.");
                // CurrentReceipt validates an available immutable receipt; FromReceipt
                // below validates it again at the typed notification boundary.
                ValidateLiveRouter();
                var expected = ProfileLaunchNotificationV1.FromReceipt(receipt.Value);
                _expectedNotification = expected.HasValue;
                ProfileLaunchNotificationV1 actual;
                var took = _launchAdapter.TryTakeNotification(out actual);
                _sourceNotificationProbed = true;
                _sourceNotificationTaken = took;
                if (took)
                {
                    // The source take has already validated actual. Keep an exact
                    // owned copy before any local comparison, including failure.
                    _retainedFailureNotification = actual;
                    _ownedNotificationProof = actual;
                }
                if (!expected.HasValue)
                {
                    if (took || !actual.Equals(default(ProfileLaunchNotificationV1)))
                        throw new InvalidOperationException("Hub handoff received an unexpected notification.");
                }
                else
                {
                    if (!took) throw new InvalidOperationException("Hub handoff notification is missing.");
                    // expected was fully validated by FromReceipt and actual by
                    // M5D7M's one authorized take. Exact ValueType equality keeps
                    // kind and both receipt-correlation fields closed without a
                    // second deep validation traversal.
                    if (!actual.Equals(expected.Value)) throw new InvalidOperationException("Hub handoff notification does not match its receipt.");
                }
                var handoff = new HubEntryHandoffReceiptV1(receipt.Value);
                handoff.Validate();
                _notification = expected.HasValue ? _retainedFailureNotification : null;
                _retainedFailureNotification = null;
                _localNotificationDelivered = false;
                _state = expected.HasValue ? HubEntryHandoffStateV1.PublishedNoticePending : HubEntryHandoffStateV1.PublishedNoNotice;
                _handoffProof = handoff;
                _handoff = handoff; // REQ-M5D7N-004: non-throwing publication is last.
            }
            catch { _state = HubEntryHandoffStateV1.Failed; throw; }
        }

        private void OnDisable() { CloseBeforePublication(); }
        private void OnDestroy() { CloseBeforePublication(); }
        private void CloseBeforePublication()
        {
            ValidateStateOrFail();
            try
            {
                if (_state == HubEntryHandoffStateV1.Pristine || _state == HubEntryHandoffStateV1.AwaitingStart || _state == HubEntryHandoffStateV1.AwaitingFirstUpdate)
                    _state = HubEntryHandoffStateV1.ClosedBeforePublication;
                ValidateStateOrFail();
            }
            catch
            {
                _state = HubEntryHandoffStateV1.Failed;
                throw;
            }
        }

        private void ValidateTopology()
        {
            if (!enabled || !gameObject.activeInHierarchy || _launchAdapter == null || _router == null || !_launchAdapter.enabled || !_router.enabled || !_launchAdapter.gameObject.activeInHierarchy || !_router.gameObject.activeInHierarchy || !ReferenceEquals(_launchAdapter.gameObject, gameObject) || !ReferenceEquals(_launchAdapter.BoundRouter, _router))
                throw new InvalidOperationException("Hub handoff authored cohort is not exact.");
        }
        private void ValidateLiveRouter()
        {
            if (!ReferenceEquals(_launchAdapter.BoundRouter, _router) || !_router.enabled || !_router.gameObject.activeInHierarchy || _router.IsFaulted || _router.EffectiveMode != InputMode.UIOnly)
                throw new InvalidOperationException("Hub handoff router is not ready.");
        }
        // REQ-M5D7N-006..008: all public/lifecycle boundaries use one fail-closed,
        // immutable-state validator.  It deliberately preserves corrupt evidence
        // while latching Failed, rather than normalizing forensic state away.
        private void ValidateStateOrFail()
        {
            try { ValidateState(); }
            catch
            {
                _state = HubEntryHandoffStateV1.Failed;
                throw;
            }
        }

        private void ValidateState()
        {
            if (_state < HubEntryHandoffStateV1.Pristine || _state > HubEntryHandoffStateV1.ClosedBeforePublication)
                throw new InvalidOperationException("Hub handoff state is malformed.");

            var published = IsPublished(_state);
            if (published != _handoff.HasValue || _handoff.HasValue != _handoffProof.HasValue || (_handoff.HasValue && !_handoff.Value.Equals(_handoffProof.Value)))
                throw new InvalidOperationException("Hub handoff publication state is malformed.");
            if (_state != HubEntryHandoffStateV1.Failed && (_notification.HasValue != _ownedNotificationProof.HasValue || (_notification.HasValue && !_notification.Value.Equals(_ownedNotificationProof.Value))))
                throw new InvalidOperationException("Hub handoff notification proof is malformed.");

            if (_state == HubEntryHandoffStateV1.Failed)
            {
                if (_handoff.HasValue || _handoffProof.HasValue || _notification.HasValue || _localNotificationDelivered || (!_sourceNotificationTaken && (_retainedFailureNotification.HasValue || _ownedNotificationProof.HasValue)) || (_sourceNotificationTaken && (!_retainedFailureNotification.HasValue || !_ownedNotificationProof.HasValue || !_retainedFailureNotification.Value.Equals(_ownedNotificationProof.Value))))
                    throw new InvalidOperationException("Hub handoff failed publication history is malformed.");
                return;
            }

            if (!published)
            {
                if (_expectedNotification || _sourceNotificationProbed || _sourceNotificationTaken || _localNotificationDelivered || _notification.HasValue || _ownedNotificationProof.HasValue || _retainedFailureNotification.HasValue)
                    throw new InvalidOperationException("Hub handoff pre-publication history is malformed.");
                return;
            }

            if (!_sourceNotificationProbed || _retainedFailureNotification.HasValue)
                throw new InvalidOperationException("Hub handoff publication history is malformed.");

            if (_state == HubEntryHandoffStateV1.PublishedNoNotice)
            {
                if (_expectedNotification || _sourceNotificationTaken || _notification.HasValue || _ownedNotificationProof.HasValue || _localNotificationDelivered)
                    throw new InvalidOperationException("Hub handoff no-notice publication is malformed.");
                return;
            }
            if (_state == HubEntryHandoffStateV1.PublishedNoticePending)
            {
                if (!_expectedNotification || !_sourceNotificationTaken || !_notification.HasValue || !_ownedNotificationProof.HasValue || _localNotificationDelivered)
                    throw new InvalidOperationException("Hub handoff pending notification state is malformed.");
                return;
            }
            if (_state == HubEntryHandoffStateV1.PublishedNoticeConsumed)
            {
                if (!_expectedNotification || !_sourceNotificationTaken || _notification.HasValue || _ownedNotificationProof.HasValue || !_localNotificationDelivered)
                    throw new InvalidOperationException("Hub handoff consumed notification state is malformed.");
                return;
            }
            throw new InvalidOperationException("Hub handoff publication state is malformed.");
        }

        private static bool IsPublished(HubEntryHandoffStateV1 state)
        {
            return state == HubEntryHandoffStateV1.PublishedNoNotice || state == HubEntryHandoffStateV1.PublishedNoticePending || state == HubEntryHandoffStateV1.PublishedNoticeConsumed;
        }
    }

    internal enum HubEntryHandoffStateV1 { Pristine = 0, AwaitingStart = 1, AwaitingFirstUpdate = 2, PublishedNoNotice = 3, PublishedNoticePending = 4, PublishedNoticeConsumed = 5, Failed = 6, ClosedBeforePublication = 7 }

    // REQ-M5D7N-003..005: immutable publication snapshot; no live Unity state.
    internal readonly struct HubEntryHandoffReceiptV1
    {
        private readonly DesktopProfileLaunchReceiptV1 _receipt, _correlation;
        private readonly InputMode _mode;
        private readonly bool _gameplay, _ui, _accepted;
        internal HubEntryHandoffReceiptV1(DesktopProfileLaunchReceiptV1 receipt)
        { _receipt = receipt; _correlation = receipt; _mode = global::AcadeGameMaker.Core.InputMode.UIOnly; _gameplay = false; _ui = true; _accepted = true; }
        internal DesktopProfileLaunchReceiptV1 LaunchReceipt { get { Validate(); return _receipt; } }
        internal InputMode InputMode { get { Validate(); return _mode; } }
        internal bool GameplayEnabled { get { Validate(); return _gameplay; } }
        internal bool UiEnabled { get { Validate(); return _ui; } }
        internal bool HubEntryAccepted { get { Validate(); return _accepted; } }
        internal void Validate()
        {
            // REQ-M5D7N-003/008: M5D7N validates the source once before this
            // receipt is built. Later public boundaries compare exact copied
            // backing values, including shared proof identity, without rereading
            // the verified M5D7M receipt graph.
            if (_mode != global::AcadeGameMaker.Core.InputMode.UIOnly || _gameplay || !_ui || !_accepted || !_receipt.Equals(_correlation))
                throw new InvalidOperationException("Hub handoff receipt is malformed.");
        }
        internal static bool SameReceipt(DesktopProfileLaunchReceiptV1 left, DesktopProfileLaunchReceiptV1 right)
        {
            left.Validate();
            return left.Equals(right);
        }
    }
}
