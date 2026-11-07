using System;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    // REQ-M5D7QA-002..005/008/010: sole presentation-side semantic-frame owner.
    [DefaultExecutionOrder(-180)]
    [DisallowMultipleComponent]
    internal sealed class HubMenuPresenterV1 : MonoBehaviour
    {
        [SerializeField] private InputRouter _router;
        [SerializeField] private HubEntryHandoffLatchV1 _latch;
        [SerializeField] private HubSafeFrameScalerV1 _safeFrame;
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private HubMenuButtonViewV1[] _buttons = new HubMenuButtonViewV1[4];
        [SerializeField] private RectTransform _dismissRect;
        [SerializeField] private GameObject _notice;
        [SerializeField] private TMP_Text _noticeText;
        [SerializeField] private TMP_Text _dismissText;
        // REQ-M5D7QB-001/004: exact one-way recipient; no intent getter exists.
        [SerializeField] private HubMenuIntentHandoffOwnerV1 _handoffOwner;

        private UiSemanticFrameCursorV1 _cursor;
        private HubMenuPresentationControllerV1 _controller;
        private HubEntryHandoffReceiptV1? _handoff, _handoffProof;
        private ProfileLaunchNotificationV1? _notification, _notificationProof;
        private HubMenuIntentV1? _retainedIntent, _retainedIntentProof;
        private HubMenuIntentV1? _consumedTransferIntent, _consumedTransferIntentProof;
        private HubMenuItemV1? _focus, _hover;
        private HubPresenterStateV1 _state;
        private int _stableViewportFrames;
        private NewGameConfirmationOwnerV1 _confirmationOwner, _confirmationOwnerProof;
        private SuccessorEpochSlot _successor;
        private bool _confirmationInteractionClosed;
        private ExecutionFreshSlot _executionFresh;
        private ExecutionFreshSlot _executionFreshOriginal;
        private bool _executionCancelCurrent;
        private sealed class ExecutionFreshSlot
        {
            internal readonly HubMenuPresenterV1 Issuer;
            internal readonly NewGameConfirmationOwnerV1 Owner;
            internal readonly HubMenuIntentHandoffOwnerV1 HandoffOwner;
            internal readonly InputRouter Router;
            internal readonly DesktopProfileLaunchAdapterV1 Adapter;
            internal readonly ExecutionFreshSlot Previous;
            internal readonly System.Threading.Thread Thread=System.Threading.Thread.CurrentThread;
            internal readonly HubNewGameExecutionFreshReservationV1 Reservation;
            internal readonly object Token;
            internal readonly long Epoch;
            internal readonly HubMenuPresentationControllerV1 Controller;
            internal readonly UiSemanticFrameCursorV1 Cursor, CursorProof;
            internal HubPresenterStateV1 Phase=HubPresenterStateV1.AwaitingBaseline, PhaseProof=HubPresenterStateV1.AwaitingBaseline;
            internal bool BaselinePending=true, BaselineProof=true, Transferred, TransferProof, Closed;
            internal HubMenuItemV1? Focus=HubMenuItemV1.NewGame, Hover;
            internal HubMenuIntentV1? Retained, RetainedProof;
            internal ExecutionFreshSlot(HubMenuPresenterV1 issuer,ExecutionFreshSlot previous,HubNewGameExecutionFreshReservationV1 reservation,object token,long epoch,HubMenuPresentationControllerV1 controller,UiSemanticFrameCursorV1 cursor)
            { Issuer=issuer; Owner=issuer._confirmationOwnerProof; HandoffOwner=issuer._handoffOwner; Router=issuer._router; Adapter=issuer._latch.BoundLaunchAdapterForAuthoring;
              Previous=previous; Reservation=reservation; Token=token; Epoch=epoch; Controller=controller; Cursor=CursorProof=cursor; }
        }
        private sealed class SuccessorEpochSlot
        {
            internal readonly SuccessorEpochSlot Previous;
            internal readonly HubNewGameSuccessorReservationV1 Reservation;
            internal readonly object Token;
            internal readonly long Epoch;
            internal readonly HubMenuPresentationControllerV1 Controller;
            internal readonly UiSemanticFrameCursorV1 Cursor, CursorProof;
            internal HubPresenterStateV1 Phase, PhaseProof;
            internal bool BaselinePending=true, BaselineProof=true, Transferred, TransferProof;
            internal HubMenuItemV1? Focus=HubMenuItemV1.NewGame, Hover;
            internal HubMenuIntentV1? Retained, RetainedProof;
            internal SuccessorEpochSlot(SuccessorEpochSlot previous,HubNewGameSuccessorReservationV1 reservation,object token,long epoch,HubMenuPresentationControllerV1 controller,UiSemanticFrameCursorV1 cursor)
            { Previous=previous; Reservation=reservation; Token=token; Epoch=epoch; Controller=controller; Cursor=CursorProof=cursor; Phase=PhaseProof=HubPresenterStateV1.AwaitingBaseline; }
        }

        internal HubPresenterStateV1 State { get { return _state; } }
        internal HubMenuIntentHandoffOwnerV1 HandoffOwner { get { return _handoffOwner; } }
        internal void ConfigureForAuthoring(InputRouter router, HubEntryHandoffLatchV1 latch, HubSafeFrameScalerV1 safeFrame, EventSystem eventSystem, HubMenuButtonViewV1[] buttons, RectTransform dismissRect, GameObject notice, TMP_Text noticeText, TMP_Text dismissText)
        { _router = router; _latch = latch; _safeFrame = safeFrame; _eventSystem = eventSystem; _buttons = buttons; _dismissRect = dismissRect; _notice = notice; _noticeText = noticeText; _dismissText = dismissText; }

        // REQ-M5D7QB-001/004: builder-only reciprocal authoring seam.
        internal void ConfigureHandoffOwnerForAuthoring(HubMenuIntentHandoffOwnerV1 owner)
        {
            if (owner == null || _handoffOwner != null)
                throw new InvalidOperationException("Hub presenter handoff owner authoring seam requires a pristine inactive component.");
            _handoffOwner = owner;
        }

        // REQ-M5D7QB-001..004: the only retained-intent transfer boundary.
        internal bool TryTakeRetainedIntent(HubMenuIntentHandoffOwnerV1 owner, out HubMenuIntentRequestV1 request)
        {
            request = default(HubMenuIntentRequestV1);
            try
            {
                ValidateTransferState();
                if (owner == null || !ReferenceEquals(owner, _handoffOwner))
                    throw new InvalidOperationException("Hub retained intent recipient is foreign.");
                if (_state != HubPresenterStateV1.IntentRetained) return false;
                var intent = _retainedIntent.Value;
                intent.Validate();
                if (!_retainedIntentProof.HasValue || !_retainedIntentProof.Value.Equals(intent))
                    throw new InvalidOperationException("Hub retained intent transfer proof is malformed.");
                if (_consumedTransferIntent.HasValue || _consumedTransferIntentProof.HasValue) return false;
                if (!_handoff.HasValue || !_handoffProof.HasValue || !_handoff.Value.Equals(_handoffProof.Value))
                    throw new InvalidOperationException("Hub retained intent handoff is malformed.");
                request = new HubMenuIntentRequestV1(intent.Item, _handoff.Value);
                _consumedTransferIntentProof = intent;
                _consumedTransferIntent = intent; // last non-throwing publication marker
                ValidateTransferState();
                return true;
            }
            catch
            {
                Fail();
                throw;
            }
        }

        private void Awake()
        {
            _state = HubPresenterStateV1.AwaitingHandoff;
            try { ValidateTopology(); Cursor.visible = true; Cursor.lockState = CursorLockMode.None; Render(); }
            catch { Fail(); }
        }

        private void Update()
        {
            if (_executionCancelCurrent) { UpdateExecutionCancel(); return; }
            if (_executionFresh != null) { UpdateExecutionFreshSlot(); return; }
            if (_confirmationInteractionClosed) return;
            if (_successor != null) { UpdateSuccessor(); return; }
            if (_state == HubPresenterStateV1.Failed || _state == HubPresenterStateV1.Closed) return;
            try
            {
                var changed = _safeFrame.Refresh();
                if (changed) _stableViewportFrames = 0; else _stableViewportFrames++;
                if (_state == HubPresenterStateV1.AwaitingHandoff)
                {
                    var published = _latch.CurrentHandoff;
                    if (!published.HasValue) return;
                    _handoff = _handoffProof = published.Value;
                    ProfileLaunchNotificationV1 taken;
                    if (_latch.TryTakeLaunchNotification(out taken)) _notification = _notificationProof = taken;
                    _controller = HubMenuPresentationControllerV1.Create(_handoff.Value, _notification);
                    _cursor = UiSemanticFrameCursorV1.Create(_router);
                    _state = _cursor.State == UiSemanticFrameCursorStateV1.Ready ? HubPresenterStateV1.Ready : HubPresenterStateV1.AwaitingBaseline;
                    _focus = _controller.CurrentView.InitialFocus;
                }
                if (_state == HubPresenterStateV1.AwaitingBaseline && _cursor.State == UiSemanticFrameCursorStateV1.Ready) _state = HubPresenterStateV1.Ready;
                Render();
            }
            catch { Fail(); }
        }

        private void FixedUpdate()
        {
            if (_executionCancelCurrent) { AdvanceExecutionCancel(); return; }
            if (_executionFresh != null) { AdvanceExecutionFreshSlot(); return; }
            if (_confirmationInteractionClosed) return;
            if (_successor != null) { AdvanceSuccessor(); return; }
            if (_state != HubPresenterStateV1.Ready && _state != HubPresenterStateV1.IntentRetained) return;
            try
            {
                UiSemanticFrameV1 frame;
                if (!_cursor.TryAdvance(_router, out frame)) return;
                if (!_safeFrame.IsSupported) { _hover = null; Render(); return; }
                // A changed frame and one subsequent stable frame quarantine Point/Click.
                var quarantine = _stableViewportFrames < 2;
                Interpret(frame, quarantine);
                Render();
            }
            catch { Fail(); }
        }

        private void Interpret(UiSemanticFrameV1 frame, bool quarantine)
        {
            var view = _controller.CurrentView;
            if (frame.NavigateChanged && Math.Abs(frame.NavigateYQ4096) >= 2048 && _state == HubPresenterStateV1.Ready)
                MoveFocus(frame.NavigateYQ4096 > 0 ? -1 : 1, view);
            if (!quarantine && frame.PointChanged) _hover = Hit(frame.PointXActualPx, frame.PointYActualPx, false);
            if (frame.ClickPressed && !quarantine)
            {
                if (view.NotificationState == HubNoticeStateV1.Visible && InRect(frame.ClickPointXActualPx, frame.ClickPointYActualPx, _dismissRect)) { _controller.TryDismissNotification(HubNoticeDismissalV1.Click); return; }
                var clicked = Hit(frame.ClickPointXActualPx, frame.ClickPointYActualPx, false);
                if (clicked.HasValue && _state == HubPresenterStateV1.Ready && view.IsInteractable(clicked.Value)) { _focus = clicked; Activate(clicked.Value); return; }
            }
            if (frame.SubmitPressed)
            {
                if (view.NotificationState == HubNoticeStateV1.Visible) { _controller.TryDismissNotification(HubNoticeDismissalV1.Submit); return; }
                if (_state == HubPresenterStateV1.Ready && _focus.HasValue) Activate(_focus.Value);
            }
        }

        private void Activate(HubMenuItemV1 item)
        {
            if (_executionCancelCurrent) { ActivateSuccessor(item); return; }
            if (_executionFresh != null) { ActivateExecutionFreshSlot(item); return; }
            if (_confirmationInteractionClosed) return;
            if (!_controller.TryActivate(item)) return;
            HubMenuIntentV1 intent;
            if (!_controller.TryTakeIntent(out intent)) throw new InvalidOperationException("Controller did not yield its accepted intent.");
            _retainedIntentProof = intent;
            _retainedIntent = intent; _state = HubPresenterStateV1.IntentRetained;
        }

        private void MoveFocus(int delta, HubMenuViewV1 view)
        {
            var current = _focus.HasValue ? (int)_focus.Value - 1 : 0;
            for (var next = current + delta; next >= 0 && next < 4; next += delta)
            { var item = (HubMenuItemV1)(next + 1); if (view.IsInteractable(item)) { _focus = item; return; } }
        }
        private HubMenuItemV1? Hit(int actualX, int actualY, bool dismiss)
        {
            var scale = _safeFrame.IntegerScale; var rect = _safeFrame.SafeRectActual;
            if (scale <= 0 || actualX < rect.xMin || actualX >= rect.xMax || actualY < rect.yMin || actualY >= rect.yMax) return null;
            for (var i = 0; i < 4; i++) if (InRect(actualX, actualY, _buttons[i].HitRect)) return (HubMenuItemV1)(i + 1);
            return null;
        }
        private bool InRect(int actualX, int actualY, RectTransform rect)
        {
            if (rect == null || _safeFrame.IntegerScale <= 0) return false;
            var p = _safeFrame.SafeRectActual; var s = _safeFrame.IntegerScale;
            var localX = (actualX - p.xMin) / s; var localY = (actualY - p.yMin) / s;
            var r = rect.rect; var a = rect.anchoredPosition;
            var left = a.x - rect.pivot.x * r.width; var bottom = a.y - rect.pivot.y * r.height;
            return localX >= left && localX < left + r.width && localY >= bottom && localY < bottom + r.height;
        }
        private void Render()
        {
            if (_successor != null) { RenderSuccessor(); return; }
            if (_controller == null) return;
            var view = _controller.CurrentView;
            var labels = new[] { HubMenuCopyV1.Continue, HubMenuCopyV1.NewGame, HubMenuCopyV1.Settings, HubMenuCopyV1.Quit };
            for (var i = 0; i < 4; i++) { var item = (HubMenuItemV1)(i + 1); _buttons[i].Render(labels[i], view.IsInteractable(item) && _state == HubPresenterStateV1.Ready, _focus == item, _hover == item); }
            var visible = view.NotificationState == HubNoticeStateV1.Visible;
            if (_notice != null) _notice.SetActive(visible);
            if (visible && _notification.HasValue) _noticeText.text = HubMenuCopyV1.Notification(_notification.Value.Kind);
            if (_dismissText != null) _dismissText.text = HubMenuCopyV1.Dismiss;
        }
        private void ValidateTopology()
        {
            if (_router == null || _latch == null || _safeFrame == null || _eventSystem == null || _buttons == null || _buttons.Length != 4 || _dismissRect == null || _notice == null || _noticeText == null || _dismissText == null || _handoffOwner == null || !ReferenceEquals(_handoffOwner.Presenter, this) || !ReferenceEquals(_handoffOwner.gameObject, gameObject) || gameObject.GetComponents<HubMenuIntentHandoffOwnerV1>().Length != 1)
                throw new InvalidOperationException("Hub presenter authored topology is malformed.");
        }
        // REQ-M5D7QC3-001/005/006: authored cohort and successor slots are
        // additive. Initial controller/cursor/intent/handoff remain forensic history.
        internal void BindConfirmationOwner(NewGameConfirmationOwnerV1 owner)
        {
            if (owner == null || _confirmationOwner != null) throw new InvalidOperationException("Confirmation presenter authoring is not pristine.");
            _confirmationOwner=_confirmationOwnerProof=owner;
        }
        internal bool MatchesConfirmationCohort(NewGameConfirmationOwnerV1 owner,HubMenuIntentHandoffOwnerV1 handoffOwner,InputRouter router)
        {
            if (_executionFresh != null && !ReferenceEquals(_executionFresh.Thread,System.Threading.Thread.CurrentThread)) return false;
            if (_executionCancelCurrent) return _successor != null && _successor.Phase != HubPresenterStateV1.Closed &&
                ReferenceEquals(_confirmationOwner,owner) && ReferenceEquals(_confirmationOwnerProof,owner) &&
                ReferenceEquals(_handoffOwner,handoffOwner) && ReferenceEquals(_router,router) &&
                handoffOwner.MatchesSuccessorReservation(_successor.Reservation,owner,this,router,_successor.Token,_successor.Epoch);
            if (_executionFresh != null) return !_executionFresh.Closed && ReferenceEquals(_confirmationOwner,owner) &&
                ReferenceEquals(_confirmationOwnerProof,owner) && ReferenceEquals(_handoffOwner,handoffOwner) && ReferenceEquals(_router,router) &&
                owner.MatchesExecutionFreshCohort(handoffOwner,this,router,_executionFresh.Reservation,_executionFresh.Token,_executionFresh.Epoch);
            return !_confirmationInteractionClosed && ReferenceEquals(_confirmationOwner,owner) && ReferenceEquals(_confirmationOwnerProof,owner) && ReferenceEquals(_handoffOwner,handoffOwner) && ReferenceEquals(_router,router) && _latch != null && ReferenceEquals(_latch.BoundRouterForAuthoring,router) && owner.MatchesLaunchAdapter(_latch.BoundLaunchAdapterForAuthoring);
        }
        internal bool MatchesImmutableHandoff(HubEntryHandoffReceiptV1 receipt)
        {
            receipt.Validate();
            if (_handoff.HasValue != _handoffProof.HasValue || (_handoff.HasValue && !_handoff.Value.Equals(_handoffProof.Value))) throw new InvalidOperationException("Confirmation handoff proof is malformed.");
            if (!_handoff.HasValue) return false;
            _handoff.Value.Validate(); _handoffProof.Value.Validate();
            return _handoff.Value.Equals(receipt);
        }
        internal void PrepareNewGameSuccessor(NewGameConfirmationOwnerV1 owner,HubNewGameSuccessorReservationV1 reservation,object token,long epoch)
        {
            if (!MatchesConfirmationCohort(owner,_handoffOwner,_router) || !_handoffOwner.HasLiveSuccessorPermission(reservation,owner,this,_router,token,epoch) || !_handoff.HasValue || !MatchesImmutableHandoff(_handoff.Value) || !_retainedIntent.HasValue || !_consumedTransferIntent.HasValue || (_successor != null && !(_executionFresh != null && !_executionCancelCurrent && _executionFresh.Transferred) && (_successor.Phase != HubPresenterStateV1.IntentRetained || !_successor.Transferred))) throw new InvalidOperationException("New Game presenter successor is not authenticated.");
            ValidateTransferState();
            var previousController=_executionFresh != null && !_executionCancelCurrent ? _executionFresh.Controller : _successor == null ? _controller : _successor.Controller;
            var controller=HubMenuPresentationControllerV1.Create(_handoff.Value,_notification);
            if (previousController.CurrentView.NotificationState == HubNoticeStateV1.Dismissed && !controller.TryDismissNotification(HubNoticeDismissalV1.Submit)) throw new InvalidOperationException("New Game notice dismissal could not be preserved.");
            var cursor=UiSemanticFrameCursorV1.Create(_router);
            if (cursor.State == UiSemanticFrameCursorStateV1.Closed || cursor.State == UiSemanticFrameCursorStateV1.Failed) throw new InvalidOperationException("New Game successor cursor is unavailable.");
            _successor=new SuccessorEpochSlot(_successor,reservation,token,epoch,controller,cursor);
            if (_executionFresh != null) { RequireOwnedExecutionFreshSlot(_executionFresh).Closed=true; _executionCancelCurrent=true; }
            _stableViewportFrames=0;
            RenderSuccessor();
        }
        internal bool HasPreparedSuccessor(HubNewGameSuccessorReservationV1 reservation,object token,long epoch)
        {
            return (!_confirmationInteractionClosed || _executionCancelCurrent) && _successor != null && ReferenceEquals(_successor.Reservation,reservation) && ReferenceEquals(_successor.Token,token) && _successor.Epoch == epoch && _successor.BaselinePending && _successor.BaselineProof && _successor.Phase == HubPresenterStateV1.AwaitingBaseline && _successor.PhaseProof == _successor.Phase;
        }
        internal bool TryTakeSuccessorIntent(HubMenuIntentHandoffOwnerV1 owner,HubNewGameSuccessorReservationV1 reservation,object token,long epoch,out HubMenuIntentRequestV1 request)
        {
            request=default(HubMenuIntentRequestV1);
            ValidateSuccessor();
            if (!ReferenceEquals(owner,_handoffOwner) || !ReferenceEquals(reservation,_successor.Reservation) || !ReferenceEquals(token,_successor.Token) || epoch != _successor.Epoch) throw new InvalidOperationException("New Game successor recipient is foreign.");
            if (_successor.Phase != HubPresenterStateV1.IntentRetained || _successor.Transferred) return false;
            var intent=_successor.Retained.Value; intent.Validate();
            request=new HubMenuIntentRequestV1(intent.Item,_handoff.Value);
            _successor.Transferred=_successor.TransferProof=true;
            return true;
        }
        private void ValidateSuccessor()
        {
            if ((!_executionCancelCurrent && _confirmationInteractionClosed) || _successor == null || !MatchesConfirmationCohort(_confirmationOwner,_handoffOwner,_router) || _successor.Phase != _successor.PhaseProof || _successor.BaselinePending != _successor.BaselineProof || _successor.Transferred != _successor.TransferProof || !ReferenceEquals(_successor.Cursor,_successor.CursorProof) || _successor.Retained.HasValue != _successor.RetainedProof.HasValue || (_successor.Retained.HasValue && !_successor.Retained.Value.Equals(_successor.RetainedProof.Value)) || (_successor.BaselinePending && (_successor.Phase != HubPresenterStateV1.AwaitingBaseline || _successor.Retained.HasValue || _successor.Transferred)) || (_successor.Phase == HubPresenterStateV1.IntentRetained && !_successor.Retained.HasValue) || (_successor.Phase != HubPresenterStateV1.IntentRetained && (_successor.Retained.HasValue || _successor.Transferred))) throw new InvalidOperationException("New Game successor evidence is malformed.");
            if (_successor.Cursor.State == UiSemanticFrameCursorStateV1.Failed || _successor.Cursor.State == UiSemanticFrameCursorStateV1.Closed) throw new InvalidOperationException("New Game successor cursor is terminal.");
        }
        private void UpdateSuccessor()
        {
            if (_confirmationInteractionClosed) return;
            try
            {
                ValidateSuccessor();
                var changed=_safeFrame.Refresh();
                if (changed) _stableViewportFrames=0; else _stableViewportFrames++;
                // Factory Ready never overrides the independent pending witness.
                RenderSuccessor();
            }
            catch { CloseSuccessorFailure(); }
        }
        private void AdvanceSuccessor()
        {
            if (_confirmationInteractionClosed) return;
            try
            {
                ValidateSuccessor();
                UiSemanticFrameV1 frame;
                if (!_successor.Cursor.TryAdvance(_router,out frame)) return;
                frame.Validate();
                if (_successor.BaselinePending)
                {
                    _successor.BaselinePending=_successor.BaselineProof=false;
                    _successor.Phase=_successor.PhaseProof=HubPresenterStateV1.Ready;
                    RenderSuccessor();
                    return; // first true frame is discarded without interpretation
                }
                if (!_safeFrame.IsSupported) { _successor.Hover=null; RenderSuccessor(); return; }
                InterpretSuccessor(frame,_stableViewportFrames < 2);
                RenderSuccessor();
            }
            catch { CloseSuccessorFailure(); }
        }
        private void UpdateExecutionCancel()
        {
            if (!_executionCancelCurrent || _successor == null || _successor.Phase == HubPresenterStateV1.Closed) return;
            try
            {
                ValidateSuccessor();
                var changed=_safeFrame.Refresh();
                if (changed) _stableViewportFrames=0; else _stableViewportFrames++;
                // Factory Ready never overrides the independent pending witness.
                RenderSuccessor();
            }
            catch { CloseSuccessorFailure(); }
        }
        private void AdvanceExecutionCancel()
        {
            if (!_executionCancelCurrent || _successor == null || _successor.Phase == HubPresenterStateV1.Closed) return;
            try
            {
                ValidateSuccessor();
                UiSemanticFrameV1 frame;
                if (!_successor.Cursor.TryAdvance(_router,out frame)) return;
                frame.Validate();
                if (_successor.BaselinePending)
                {
                    _successor.BaselinePending=_successor.BaselineProof=false;
                    _successor.Phase=_successor.PhaseProof=HubPresenterStateV1.Ready;
                    RenderSuccessor();
                    return; // first true frame is discarded without interpretation
                }
                if (!_safeFrame.IsSupported) { _successor.Hover=null; RenderSuccessor(); return; }
                InterpretSuccessor(frame,_stableViewportFrames < 2);
                RenderSuccessor();
            }
            catch { CloseSuccessorFailure(); }
        }
        private void InterpretSuccessor(UiSemanticFrameV1 frame,bool quarantine)
        {
            var s=_successor; var view=s.Controller.CurrentView;
            if (frame.NavigateChanged && Math.Abs(frame.NavigateYQ4096) >= 2048 && s.Phase == HubPresenterStateV1.Ready)
            {
                var delta=frame.NavigateYQ4096 > 0 ? -1 : 1;
                var current=s.Focus.HasValue ? (int)s.Focus.Value-1 : 0;
                for (var next=current+delta;next>=0 && next<4;next+=delta) if (view.IsInteractable((HubMenuItemV1)(next+1))) { s.Focus=(HubMenuItemV1)(next+1); break; }
            }
            if (!quarantine && frame.PointChanged) s.Hover=Hit(frame.PointXActualPx,frame.PointYActualPx,false);
            if (frame.ClickPressed && !quarantine)
            {
                if (view.NotificationState == HubNoticeStateV1.Visible && InRect(frame.ClickPointXActualPx,frame.ClickPointYActualPx,_dismissRect)) { s.Controller.TryDismissNotification(HubNoticeDismissalV1.Click); return; }
                var clicked=Hit(frame.ClickPointXActualPx,frame.ClickPointYActualPx,false);
                if (clicked.HasValue && s.Phase == HubPresenterStateV1.Ready && view.IsInteractable(clicked.Value)) { s.Focus=clicked; ActivateSuccessor(clicked.Value); return; }
            }
            if (frame.SubmitPressed)
            {
                if (view.NotificationState == HubNoticeStateV1.Visible) { s.Controller.TryDismissNotification(HubNoticeDismissalV1.Submit); return; }
                if (s.Phase == HubPresenterStateV1.Ready && s.Focus.HasValue) ActivateSuccessor(s.Focus.Value);
            }
        }
        private void ActivateSuccessor(HubMenuItemV1 item)
        {
            ValidateSuccessor();
            if (_successor.BaselinePending || _successor.Phase != HubPresenterStateV1.Ready || !_successor.Controller.TryActivate(item)) return;
            HubMenuIntentV1 intent;
            if (!_successor.Controller.TryTakeIntent(out intent)) throw new InvalidOperationException("Successor controller did not yield its intent.");
            _successor.Retained=_successor.RetainedProof=intent;
            _successor.Phase=_successor.PhaseProof=HubPresenterStateV1.IntentRetained;
        }
        private void RenderSuccessor()
        {
            var s=_successor; if (s == null) return;
            var view=s.Controller.CurrentView;
            var labels=new[] { HubMenuCopyV1.Continue,HubMenuCopyV1.NewGame,HubMenuCopyV1.Settings,HubMenuCopyV1.Quit };
            for (var i=0;i<4;i++) if (_buttons[i] != null) { var item=(HubMenuItemV1)(i+1); _buttons[i].Render(labels[i],(!_confirmationInteractionClosed || _executionCancelCurrent) && !s.BaselinePending && s.Phase == HubPresenterStateV1.Ready && view.IsInteractable(item),s.Focus == item,s.Hover == item); }
            var visible=view.NotificationState == HubNoticeStateV1.Visible;
            if (_notice != null) _notice.SetActive(visible);
            if (visible && _notification.HasValue) _noticeText.text=HubMenuCopyV1.Notification(_notification.Value.Kind);
            if (_dismissText != null) _dismissText.text=HubMenuCopyV1.Dismiss;
        }
        private void ValidateExecutionFreshSlot()
        {
            if (_executionFresh == null || _executionFresh.Closed || !_handoffOwner.HasLiveExecutionFreshPermission(_executionFresh.Reservation,_confirmationOwner,this,_router,_executionFresh.Token,_executionFresh.Epoch) || _executionFresh.Phase != _executionFresh.PhaseProof || _executionFresh.BaselinePending != _executionFresh.BaselineProof || _executionFresh.Transferred != _executionFresh.TransferProof || !ReferenceEquals(_executionFresh.Cursor,_executionFresh.CursorProof) || _executionFresh.Retained.HasValue != _executionFresh.RetainedProof.HasValue || (_executionFresh.Retained.HasValue && !_executionFresh.Retained.Value.Equals(_executionFresh.RetainedProof.Value)) || (_executionFresh.BaselinePending && (_executionFresh.Phase != HubPresenterStateV1.AwaitingBaseline || _executionFresh.Retained.HasValue || _executionFresh.Transferred)) || (_executionFresh.Phase == HubPresenterStateV1.IntentRetained && !_executionFresh.Retained.HasValue) || (_executionFresh.Phase != HubPresenterStateV1.IntentRetained && (_executionFresh.Retained.HasValue || _executionFresh.Transferred))) throw new InvalidOperationException("New Game successor evidence is malformed.");
            if (_executionFresh.Cursor.State == UiSemanticFrameCursorStateV1.Failed || _executionFresh.Cursor.State == UiSemanticFrameCursorStateV1.Closed) throw new InvalidOperationException("New Game successor cursor is terminal.");
        }
        private void UpdateExecutionFreshSlot()
        {
            if (_executionFresh.Closed) return;
            if (!_handoffOwner.HasLiveExecutionFreshPermission(_executionFresh.Reservation,_confirmationOwner,this,_router,_executionFresh.Token,_executionFresh.Epoch)) return;
            try
            {
                ValidateExecutionFreshSlot();
                var changed=_safeFrame.Refresh();
                if (changed) _stableViewportFrames=0; else _stableViewportFrames++;
                // Factory Ready never overrides the independent pending witness.
                RenderExecutionFreshSlot();
            }
            catch { CloseExecutionFreshFailure(); }
        }
        private void AdvanceExecutionFreshSlot()
        {
            if (_executionFresh.Closed) return;
            if (!_handoffOwner.HasLiveExecutionFreshPermission(_executionFresh.Reservation,_confirmationOwner,this,_router,_executionFresh.Token,_executionFresh.Epoch)) return;
            try
            {
                ValidateExecutionFreshSlot();
                UiSemanticFrameV1 frame;
                if (!_executionFresh.Cursor.TryAdvance(_router,out frame)) return;
                frame.Validate();
                if (_executionFresh.BaselinePending)
                {
                    _executionFresh.BaselinePending=_executionFresh.BaselineProof=false;
                    _executionFresh.Phase=_executionFresh.PhaseProof=HubPresenterStateV1.Ready;
                    RenderExecutionFreshSlot();
                    return; // first true frame is discarded without interpretation
                }
                if (!_safeFrame.IsSupported) { _executionFresh.Hover=null; RenderExecutionFreshSlot(); return; }
                InterpretExecutionFreshSlot(frame,_stableViewportFrames < 2);
                RenderExecutionFreshSlot();
            }
            catch { CloseExecutionFreshFailure(); }
        }
        private void InterpretExecutionFreshSlot(UiSemanticFrameV1 frame,bool quarantine)
        {
            var s=_executionFresh; var view=s.Controller.CurrentView;
            if (frame.NavigateChanged && Math.Abs(frame.NavigateYQ4096) >= 2048 && s.Phase == HubPresenterStateV1.Ready)
            {
                var delta=frame.NavigateYQ4096 > 0 ? -1 : 1;
                var current=s.Focus.HasValue ? (int)s.Focus.Value-1 : 0;
                for (var next=current+delta;next>=0 && next<4;next+=delta) if (view.IsInteractable((HubMenuItemV1)(next+1))) { s.Focus=(HubMenuItemV1)(next+1); break; }
            }
            if (!quarantine && frame.PointChanged) s.Hover=Hit(frame.PointXActualPx,frame.PointYActualPx,false);
            if (frame.ClickPressed && !quarantine)
            {
                if (view.NotificationState == HubNoticeStateV1.Visible && InRect(frame.ClickPointXActualPx,frame.ClickPointYActualPx,_dismissRect)) { s.Controller.TryDismissNotification(HubNoticeDismissalV1.Click); return; }
                var clicked=Hit(frame.ClickPointXActualPx,frame.ClickPointYActualPx,false);
                if (clicked.HasValue && s.Phase == HubPresenterStateV1.Ready && view.IsInteractable(clicked.Value)) { s.Focus=clicked; ActivateExecutionFreshSlot(clicked.Value); return; }
            }
            if (frame.SubmitPressed)
            {
                if (view.NotificationState == HubNoticeStateV1.Visible) { s.Controller.TryDismissNotification(HubNoticeDismissalV1.Submit); return; }
                if (s.Phase == HubPresenterStateV1.Ready && s.Focus.HasValue) ActivateExecutionFreshSlot(s.Focus.Value);
            }
        }
        private void ActivateExecutionFreshSlot(HubMenuItemV1 item)
        {
            ValidateExecutionFreshSlot();
            if (_executionFresh.BaselinePending || _executionFresh.Phase != HubPresenterStateV1.Ready || !_executionFresh.Controller.TryActivate(item)) return;
            HubMenuIntentV1 intent;
            if (!_executionFresh.Controller.TryTakeIntent(out intent)) throw new InvalidOperationException("Successor controller did not yield its intent.");
            _executionFresh.Retained=_executionFresh.RetainedProof=intent;
            _executionFresh.Phase=_executionFresh.PhaseProof=HubPresenterStateV1.IntentRetained;
        }
        private void RenderExecutionFreshSlot()
        {
            var s=_executionFresh; if (s == null) return;
            var view=s.Controller.CurrentView;
            var labels=new[] { HubMenuCopyV1.Continue,HubMenuCopyV1.NewGame,HubMenuCopyV1.Settings,HubMenuCopyV1.Quit };
            for (var i=0;i<4;i++) if (_buttons[i] != null) { var item=(HubMenuItemV1)(i+1); _buttons[i].Render(labels[i],!_executionFresh.Closed && !s.BaselinePending && s.Phase == HubPresenterStateV1.Ready && view.IsInteractable(item),s.Focus == item,s.Hover == item); }
            var visible=view.NotificationState == HubNoticeStateV1.Visible;
            if (_notice != null) _notice.SetActive(visible);
            if (visible && _notification.HasValue) _noticeText.text=HubMenuCopyV1.Notification(_notification.Value.Kind);
            if (_dismissText != null) _dismissText.text=HubMenuCopyV1.Dismiss;
        }
        internal void CloseConfirmationInteraction()
        {
            var original=SelectOriginalExecutionFreshSlot();
            try { if (original != null) RequireOwnedExecutionFreshSlot(original).Closed=true; }
            finally
            {
                _confirmationInteractionClosed=true;
                if (_successor != null) { _successor.Phase=_successor.PhaseProof=HubPresenterStateV1.Closed; _successor.Hover=null; }
                if (_eventSystem != null) _eventSystem.SetSelectedGameObject(null);
                if (_buttons != null) foreach (var button in _buttons) if (button != null) button.interactable=false;
            }
        }
        internal void PrepareExecutionFresh(NewGameConfirmationOwnerV1 owner,HubNewGameExecutionFreshReservationV1 reservation,object token,long epoch)
        {
            if (!_handoffOwner.MatchesExecutionFreshReservation(reservation,owner,this,_router,token,epoch)) throw new InvalidOperationException("C4 fresh presenter reservation is not live preparation.");
            if (!ReferenceEquals(owner,_confirmationOwner) || !ReferenceEquals(_confirmationOwner,_confirmationOwnerProof) ||
                !_handoff.HasValue || !MatchesImmutableHandoff(_handoff.Value) || !_confirmationInteractionClosed ||
                (_executionFresh != null && !_executionFresh.Closed)) throw new InvalidOperationException("C4 presenter original closure is inconsistent.");
            ValidateTransferState();
            var previousController=_executionFresh != null && !_executionCancelCurrent ? _executionFresh.Controller : _successor == null ? _controller : _successor.Controller;
            var controller=HubMenuPresentationControllerV1.Create(_handoff.Value,_notification);
            if (previousController.CurrentView.NotificationState == HubNoticeStateV1.Dismissed && !controller.TryDismissNotification(HubNoticeDismissalV1.Submit))
                throw new InvalidOperationException("C4 notice dismissal history is inconsistent.");
            var cursor=UiSemanticFrameCursorV1.Create(_router);
            if (cursor.State == UiSemanticFrameCursorStateV1.Closed || cursor.State == UiSemanticFrameCursorStateV1.Failed) throw new InvalidOperationException("C4 original cursor source is unavailable.");
            _executionFresh=new ExecutionFreshSlot(this,_executionFresh,reservation,token,epoch,controller,cursor);
            _executionFreshOriginal=_executionFresh;
            _executionCancelCurrent=false;
            _stableViewportFrames=0; RenderExecutionFreshSlot();
        }
        internal bool MatchesPreparedExecutionFresh(NewGameConfirmationOwnerV1 owner,HubNewGameExecutionFreshReservationV1 reservation,object token,long epoch)
        {
            return _executionFresh != null && !_executionFresh.Closed && ReferenceEquals(owner,_confirmationOwner) &&
                ReferenceEquals(_executionFresh.Reservation,reservation) && ReferenceEquals(_executionFresh.Token,token) && _executionFresh.Epoch == epoch &&
                _executionFresh.BaselinePending && _executionFresh.BaselineProof && !_executionFresh.Transferred && !_executionFresh.TransferProof &&
                _executionFresh.Phase == HubPresenterStateV1.AwaitingBaseline && _executionFresh.Phase == _executionFresh.PhaseProof &&
                ReferenceEquals(_executionFresh.Cursor,_executionFresh.CursorProof) && !_executionFresh.Retained.HasValue && !_executionFresh.RetainedProof.HasValue;
        }
        internal bool TryTakeExecutionFreshIntent(HubMenuIntentHandoffOwnerV1 owner,HubNewGameExecutionFreshReservationV1 reservation,object token,long epoch,out HubMenuIntentRequestV1 request)
        {
            request=default(HubMenuIntentRequestV1);
            if (_executionFresh == null || _executionFresh.Closed || !ReferenceEquals(owner,_handoffOwner) ||
                !ReferenceEquals(reservation,_executionFresh.Reservation) || !ReferenceEquals(token,_executionFresh.Token) || epoch != _executionFresh.Epoch) return false;
            ValidateExecutionFreshSlot();
            if (_executionFresh.Phase != HubPresenterStateV1.IntentRetained || _executionFresh.Transferred) return false;
            var intent=_executionFresh.Retained.Value; intent.Validate(); request=new HubMenuIntentRequestV1(intent.Item,_handoff.Value);
            _executionFresh.Transferred=_executionFresh.TransferProof=true; return true;
        }
        private void CloseExecutionFreshFailure()
        {
            var original=RequireOwnedExecutionFreshSlot(SelectOriginalExecutionFreshSlot());
            try { CloseConfirmationInteraction(); }
            finally { try { original.HandoffOwner.CloseConfirmationInteraction(); } finally { try { original.Owner.CloseFromPeer(); } finally { original.Adapter.LatchResetCutoverFailure(); } } }
        }
        private ExecutionFreshSlot RequireOwnedExecutionFreshSlot(ExecutionFreshSlot slot)
        {
            if (slot == null || !ReferenceEquals(slot.Issuer,this) || !ReferenceEquals(slot.Thread,System.Threading.Thread.CurrentThread) ||
                slot.Reservation == null || ReferenceEquals(slot.Owner,null) ||
                ReferenceEquals(slot.HandoffOwner,null) || ReferenceEquals(slot.Router,null) || ReferenceEquals(slot.Adapter,null))
                throw new InvalidOperationException("C4 presenter closure has no original slot association.");
            return slot;
        }
        private ExecutionFreshSlot SelectOriginalExecutionFreshSlot()
        {
            ExecutionFreshSlot latest=null;
            ConsiderOriginalExecutionFreshSlot(_executionFreshOriginal,ref latest);
            ConsiderOriginalExecutionFreshSlot(_executionFresh,ref latest);
            return latest;
        }
        private void ConsiderOriginalExecutionFreshSlot(ExecutionFreshSlot slot,ref ExecutionFreshSlot latest)
        {
            if (slot == null || !ReferenceEquals(slot.Issuer,this)) return;
            RequireOwnedExecutionFreshSlot(slot);
            if (latest == null || slot.Epoch>latest.Epoch) latest=slot;
        }
        private void CloseSuccessorFailure()
        {
            var freshOriginal=SelectOriginalExecutionFreshSlot();
            if (freshOriginal != null)
            {
                var original=RequireOwnedExecutionFreshSlot(freshOriginal);
                try { CloseConfirmationInteraction(); }
                finally { try { original.HandoffOwner.CloseConfirmationInteraction(); } finally { original.Owner.CloseFromPeer(); } }
                return;
            }
            CloseConfirmationInteraction();
            if (_handoffOwner != null) _handoffOwner.CloseConfirmationInteraction();
            if (_confirmationOwner != null) _confirmationOwner.CloseFromPeer();
        }
        private void ValidateTransferState()
        {
            if (_state == HubPresenterStateV1.Failed || _state == HubPresenterStateV1.Closed)
                throw new InvalidOperationException("Hub presenter transfer is closed.");
            if (_retainedIntent.HasValue != _retainedIntentProof.HasValue || (_retainedIntent.HasValue && !_retainedIntent.Value.Equals(_retainedIntentProof.Value)) || _consumedTransferIntent.HasValue != _consumedTransferIntentProof.HasValue || (_consumedTransferIntent.HasValue && !_consumedTransferIntent.Value.Equals(_consumedTransferIntentProof.Value)))
                throw new InvalidOperationException("Hub presenter transfer evidence is malformed.");
            if (_state == HubPresenterStateV1.IntentRetained)
            {
                if (!_retainedIntent.HasValue) throw new InvalidOperationException("Hub presenter retained intent is absent.");
                _retainedIntent.Value.Validate();
            }
            else if (_retainedIntent.HasValue || _consumedTransferIntent.HasValue)
                throw new InvalidOperationException("Hub presenter transfer state is malformed.");
        }
        private void Fail()
        {
            _state = HubPresenterStateV1.Failed; _hover = null;
            if (_eventSystem != null) _eventSystem.SetSelectedGameObject(null);
            if (_buttons != null) foreach (var button in _buttons) if (button != null) button.interactable = false;
        }
        private void OnDisable() { Close(); if (_confirmationOwner != null) CloseSuccessorFailure(); }
        private void OnDestroy() { Close(); if (_confirmationOwner != null) CloseSuccessorFailure(); }
        private void Close() { if (_state != HubPresenterStateV1.Failed) _state = HubPresenterStateV1.Closed; _hover = null; if (_eventSystem != null) _eventSystem.SetSelectedGameObject(null); }
    }
    internal enum HubPresenterStateV1 { AwaitingHandoff = 1, AwaitingBaseline = 2, Ready = 3, IntentRetained = 4, Failed = 5, Closed = 6 }
}
