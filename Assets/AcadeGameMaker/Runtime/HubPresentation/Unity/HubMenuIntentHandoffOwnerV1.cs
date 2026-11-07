using System;
using System.Runtime.CompilerServices;
using System.Threading;
using AcadeGameMaker.Input.Unity;
using UnityEngine;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    // REQ-M5D7QB-003: immutable, effect-free typed request.
    internal readonly struct HubMenuIntentRequestV1
    {
        private readonly HubMenuItemV1 _item, _itemProof;
        private readonly HubEntryHandoffReceiptV1 _receipt, _receiptProof;

        internal HubMenuIntentRequestV1(HubMenuItemV1 item, HubEntryHandoffReceiptV1 receipt)
        {
            _item = _itemProof = item;
            _receipt = _receiptProof = receipt;
            Validate();
        }

        internal HubMenuItemV1 Item { get { Validate(); return _item; } }
        internal HubEntryHandoffReceiptV1 Receipt { get { Validate(); return _receipt; } }
        internal void Validate()
        {
            if (_item != _itemProof || !_receipt.Equals(_receiptProof) || !HubMenuViewV1.IsKnownMenuItem(_item))
                throw new InvalidOperationException("Hub menu intent request is malformed.");
            _receipt.Validate(); _receiptProof.Validate();
        }
    }

    internal enum HubMenuIntentHandoffOwnerStateV1 { AwaitingIntent = 1, RequestReady = 2, RequestTaken = 3, Failed = 4, Closed = 5 }
    internal sealed class IssuedNewGameRequestV1 { internal IssuedNewGameRequestV1() { } }
    internal sealed class HubNewGameSuccessorReservationV1 { internal HubNewGameSuccessorReservationV1() { } }

    // REQ-M5D7QB-001..005: one late-frame, non-effectful ownership boundary.
    [DefaultExecutionOrder(-170)]
    [DisallowMultipleComponent]
    internal sealed class HubMenuIntentHandoffOwnerV1 : MonoBehaviour
    {
        [SerializeField] private HubMenuPresenterV1 _presenter;
        private HubMenuIntentRequestV1? _request, _requestProof, _takenRequest, _takenRequestProof;
        private HubMenuIntentHandoffOwnerStateV1 _state;
        // REQ-M5D7QC3-001/005: separate current slots; original taken fields remain history.
        private NewGameConfirmationOwnerV1 _confirmationOwner, _confirmationOwnerProof;
        private object _epochToken, _epochTokenProof;
        private long _epoch = 1, _epochProof = 1;
        private IssuedNewGameRequestV1 _pendingIssued;
        private TakenEpochRecord _epochs;
        private SuccessorRequestSlot _successor;
        private HubNewGameSuccessorReservationV1 _pendingSuccessor;
        private int _c3Operation;
        private FreshWitness _executionFresh;
        private bool _executionCancelCurrent;
        private HubNewGameExecutionFreshReservationV1 _pendingExecutionFresh;
        private FreshIssuanceHistory _executionFreshHistory;
        private sealed class FreshIssuanceHistory
        {
            internal readonly FreshIssuanceHistory Previous;
            internal readonly FreshWitness Witness;
            internal readonly HubMenuIntentHandoffOwnerV1 Issuer;
            internal readonly NewGameConfirmationOwnerV1 Owner;
            internal readonly HubMenuPresenterV1 Presenter;
            internal readonly InputRouter Router;
            internal readonly HubNewGameExecutionFreshReservationV1 Reservation;
            internal readonly Thread Thread;
            internal FreshIssuanceHistory(FreshIssuanceHistory previous,FreshWitness witness)
            { Previous=previous; Witness=witness; Issuer=witness.Issuer; Owner=witness.Owner; Presenter=witness.Presenter;
              Router=witness.Router; Reservation=witness.Reservation; Thread=witness.Thread; }
        }
        private sealed class FreshWitness
        {
            internal readonly FreshWitness Previous;
            internal readonly HubMenuIntentHandoffOwnerV1 Issuer;
            internal readonly NewGameConfirmationOwnerV1 Owner;
            internal readonly HubMenuPresenterV1 Presenter;
            internal readonly InputRouter Router;
            internal readonly HubNewGameExecutionFreshCapabilityV1 Capability;
            internal readonly HubNewGameExecutionFreshReservationV1 Reservation;
            internal readonly object OldToken, Token;
            internal readonly long OldEpoch, Epoch;
            internal readonly SuccessorRequestSlot OriginalSuccessor;
            internal readonly Thread Thread=Thread.CurrentThread;
            internal HubMenuIntentRequestV1? Live, Proof, Taken, TakenProof;
            internal HubMenuIntentHandoffOwnerStateV1 Phase=HubMenuIntentHandoffOwnerStateV1.AwaitingIntent;
            internal bool Committed, Closed;
            internal FreshWitness(HubMenuIntentHandoffOwnerV1 issuer,NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router,
                HubNewGameExecutionFreshCapabilityV1 capability,HubNewGameExecutionFreshReservationV1 reservation,object token,long epoch)
            { Issuer=issuer; Owner=owner; Presenter=presenter; Router=router; Capability=capability; Reservation=reservation;
              Previous=issuer._executionFresh; OldToken=issuer._epochToken; OldEpoch=issuer._epoch; Token=token; Epoch=epoch; OriginalSuccessor=issuer._successor; }
        }
        private static readonly ConditionalWeakTable<HubNewGameExecutionFreshReservationV1,FreshWitness> ExecutionFreshReservations=new ConditionalWeakTable<HubNewGameExecutionFreshReservationV1,FreshWitness>();
        private sealed class TakenEpochRecord
        {
            internal readonly TakenEpochRecord Previous;
            internal readonly long Epoch;
            internal readonly object Token;
            internal readonly IssuedNewGameRequestV1 Handle;
            internal readonly HubMenuIntentRequestV1 Request;
            internal readonly HubNewGameRearmCapabilityV1 Rearm;
            internal TakenEpochRecord(TakenEpochRecord previous,long epoch,object token,IssuedNewGameRequestV1 handle,HubMenuIntentRequestV1 request,HubNewGameRearmCapabilityV1 rearm)
            { Previous=previous; Epoch=epoch; Token=token; Handle=handle; Request=request; Rearm=rearm; }
        }
        private sealed class SuccessorRequestSlot
        {
            internal readonly long Epoch; internal readonly object Token;
            internal readonly HubNewGameRearmCapabilityV1 Rearm;
            internal readonly HubNewGameSuccessorReservationV1 Reservation;
            internal HubMenuIntentRequestV1? Live, Proof, Taken, TakenProof;
            internal HubMenuIntentHandoffOwnerStateV1 Phase = HubMenuIntentHandoffOwnerStateV1.AwaitingIntent;
            internal SuccessorRequestSlot(long epoch,object token,HubNewGameRearmCapabilityV1 rearm,HubNewGameSuccessorReservationV1 reservation)
            { Epoch=epoch; Token=token; Rearm=rearm; Reservation=reservation; }
        }
        private sealed class IssuanceWitness
        {
            internal readonly HubMenuIntentHandoffOwnerV1 Issuer;
            internal readonly NewGameConfirmationOwnerV1 Owner;
            internal readonly HubMenuPresenterV1 Presenter;
            internal readonly InputRouter Router;
            internal readonly TakenEpochRecord Record;
            internal int Consumed;
            internal IssuanceWitness(HubMenuIntentHandoffOwnerV1 issuer,NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router,TakenEpochRecord record)
            { Issuer=issuer; Owner=owner; Presenter=presenter; Router=router; Record=record; }
        }
        private sealed class SuccessorWitness
        {
            internal readonly HubMenuIntentHandoffOwnerV1 Issuer;
            internal readonly NewGameConfirmationOwnerV1 Owner;
            internal readonly HubMenuPresenterV1 Presenter;
            internal readonly InputRouter Router;
            internal readonly HubNewGameRearmCapabilityV1 Capability;
            internal readonly object OldToken, NewToken;
            internal readonly long OldEpoch, NewEpoch;
            internal bool Committed;
            internal SuccessorWitness(HubMenuIntentHandoffOwnerV1 issuer,NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router,HubNewGameRearmCapabilityV1 capability,object oldToken,object newToken,long oldEpoch,long newEpoch)
            { Issuer=issuer; Owner=owner; Presenter=presenter; Router=router; Capability=capability; OldToken=oldToken; NewToken=newToken; OldEpoch=oldEpoch; NewEpoch=newEpoch; }
        }
        private static readonly ConditionalWeakTable<IssuedNewGameRequestV1,IssuanceWitness> Issuances = new ConditionalWeakTable<IssuedNewGameRequestV1,IssuanceWitness>();
        private static readonly ConditionalWeakTable<HubNewGameSuccessorReservationV1,SuccessorWitness> Successors = new ConditionalWeakTable<HubNewGameSuccessorReservationV1,SuccessorWitness>();

        internal HubMenuPresenterV1 Presenter { get { return _presenter; } }
        internal HubMenuIntentHandoffOwnerStateV1 State { get { return _state; } }

        // REQ-M5D7QB-004: builder-only reciprocal authoring seam.
        internal void ConfigurePresenterForAuthoring(HubMenuPresenterV1 presenter)
        {
            if (presenter == null || _presenter != null)
                throw new InvalidOperationException("Hub handoff owner authoring seam requires a pristine inactive component.");
            _presenter = presenter;
        }

        private void Awake()
        {
            _state = HubMenuIntentHandoffOwnerStateV1.AwaitingIntent;
            try { ValidateTopology(); ValidateState(); }
            catch { Fail(); }
        }

        // REQ-M5D7QB-001: exactly one poll in the following render-frame LateUpdate.
        private void LateUpdate()
        {
            if (_executionFresh != null && !_executionCancelCurrent) { PollExecutionFresh(); return; }
            if (_executionCancelCurrent) { PollSuccessor(); return; }
            if (_successor != null)
            {
                PollSuccessor();
                return;
            }
            if (_state != HubMenuIntentHandoffOwnerStateV1.AwaitingIntent) return;
            try
            {
                ValidateTopology();
                if (_presenter.State == HubPresenterStateV1.Failed || _presenter.State == HubPresenterStateV1.Closed) { Close(); return; }
                HubMenuIntentRequestV1 request;
                if (!_presenter.TryTakeRetainedIntent(this, out request)) return;
                request.Validate();
                _requestProof = request;
                _request = request;
                _state = HubMenuIntentHandoffOwnerStateV1.RequestReady;
                ValidateState();
            }
            catch { Fail(); }
        }

        // REQ-M5D7QB-002/003: one immutable consumer take, no callback/effect.
        internal bool TryTakeRequest(out HubMenuIntentRequestV1 request)
        {
            request = default(HubMenuIntentRequestV1);
            try
            {
                ValidateState();
                if (_state != HubMenuIntentHandoffOwnerStateV1.RequestReady) return false;
                var value = _request.Value;
                value.Validate();
                if (!_requestProof.HasValue || !_requestProof.Value.Equals(value) || _takenRequest.HasValue || _takenRequestProof.HasValue)
                    throw new InvalidOperationException("Hub handoff request proof is malformed.");
                _takenRequestProof = value;
                _takenRequest = value;
                _request = null; _requestProof = null;
                _state = HubMenuIntentHandoffOwnerStateV1.RequestTaken;
                request = value;
                ValidateState();
                return true;
            }
            catch { Fail(); throw; }
        }

        internal void BindConfirmationOwner(NewGameConfirmationOwnerV1 owner,object token)
        {
            if (owner == null || token == null || _confirmationOwner != null || _epochs != null || _successor != null) throw new InvalidOperationException("Confirmation authoring requires a pristine recipient.");
            _confirmationOwner=_confirmationOwnerProof=owner; _epochToken=_epochTokenProof=token;
        }

        internal bool TryTakeNewGameForConfirmation(NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router,out IssuedNewGameRequestV1 issued)
        {
            if (_executionFresh != null && !_executionCancelCurrent) return TryTakeExecutionFresh(owner,presenter,router,out issued);
            issued=null;
            if (Interlocked.CompareExchange(ref _c3Operation,1,0) != 0) throw new InvalidOperationException("Confirmation take is already inspecting.");
            var taken=false;
            try
            {
                ValidateC3State();
                if (!ReferenceEquals(owner,_confirmationOwner) || !ReferenceEquals(presenter,_presenter) || !owner.MatchesCohort(this,presenter,router) || !owner.CanBindIssuance(_epochToken,_epoch,_epochs != null) || !presenter.MatchesConfirmationCohort(owner,this,router)) return false;
                HubMenuIntentRequestV1? live=_successor == null ? _request : _successor.Live;
                var phase=_successor == null ? _state : _successor.Phase;
                if (phase != HubMenuIntentHandoffOwnerStateV1.RequestReady || !live.HasValue) return false;
                live.Value.Validate();
                if (live.Value.Item != HubMenuItemV1.NewGame || !presenter.MatchesImmutableHandoff(live.Value.Receipt)) return false;
                HubMenuIntentRequestV1 row;
                if (_successor == null)
                {
                    if (_epochs != null || _takenRequest.HasValue || _takenRequestProof.HasValue) throw new InvalidOperationException("Initial take has unexpected history.");
                    if (!TryTakeRequest(out row)) return false;
                }
                else
                {
                    if (_epochs == null || (_state != HubMenuIntentHandoffOwnerStateV1.RequestTaken && !(_executionCancelCurrent && _state == HubMenuIntentHandoffOwnerStateV1.Closed)) || !_takenRequest.HasValue || _successor.Taken.HasValue || _successor.TakenProof.HasValue) throw new InvalidOperationException("Successor take evidence is malformed.");
                    row=live.Value;
                    _successor.Taken=_successor.TakenProof=row;
                    _successor.Live=_successor.Proof=null;
                    _successor.Phase=HubMenuIntentHandoffOwnerStateV1.RequestTaken;
                }
                taken=true;
                var handle=new IssuedNewGameRequestV1();
                var record=new TakenEpochRecord(_epochs,_epoch,_epochToken,handle,row,_successor == null ? null : _successor.Rearm);
                _epochs=record;
                Issuances.Add(handle,new IssuanceWitness(this,owner,presenter,router,record));
                _pendingIssued=handle;
                owner.BindIssued(handle,_epochToken,_epoch);
                if (!owner.HasPendingIssued(handle,_epochToken,_epoch)) throw new InvalidOperationException("Confirmation reciprocal bind failed.");
                issued=handle;
                return true;
            }
            catch
            {
                Fail();
                if (_confirmationOwner != null) _confirmationOwner.CloseFromPeer();
                if (taken && _presenter != null) _presenter.CloseConfirmationInteraction();
                throw;
            }
            finally { Volatile.Write(ref _c3Operation,0); }
        }

        internal HubMenuIntentRequestV1 ConsumeNewGameIssuance(IssuedNewGameRequestV1 issued,NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router)
        {
            // Authentication rejects foreign candidates before any consumption.
            if (issued == null || !Issuances.TryGetValue(issued,out var witness) || witness == null || !ReferenceEquals(witness.Issuer,this) || !ReferenceEquals(witness.Owner,owner) || !ReferenceEquals(witness.Presenter,presenter) || !ReferenceEquals(witness.Router,router) || !ReferenceEquals(witness.Record,_epochs) || !ReferenceEquals(witness.Record.Handle,issued) || !ReferenceEquals(_pendingIssued,issued) || !ReferenceEquals(witness.Record.Token,_epochToken) || witness.Record.Epoch != _epoch || !owner.HasPendingIssued(issued,_epochToken,_epoch)) throw new InvalidOperationException("New Game issuance is not the exact pending handle.");
            ValidateC3State();
            if (Interlocked.CompareExchange(ref witness.Consumed,1,0) != 0) throw new InvalidOperationException("New Game issuance was already consumed.");
            try
            {
                owner.MarkIssuedConsumed(issued,_epochToken,_epoch);
                _pendingIssued=null;
                witness.Record.Request.Validate();
                return witness.Record.Request;
            }
            catch { Fail(); owner.CloseFromPeer(); _presenter.CloseConfirmationInteraction(); throw; }
        }

        // REQ-M5D7QC3-001: read-only explicit issuance classification, no authority returned.
        internal int ClassifyNewGameIssuance(IssuedNewGameRequestV1 issued,NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router)
        {
            if (issued == null || !Issuances.TryGetValue(issued,out var w) || w == null || !ReferenceEquals(w.Issuer,this) || !ReferenceEquals(w.Owner,owner) || !ReferenceEquals(w.Presenter,presenter) || !ReferenceEquals(w.Router,router) || w.Record == null || !ReferenceEquals(w.Record.Handle,issued)) return 0;
            for (var record=_epochs;record != null;record=record.Previous)
                if (ReferenceEquals(record,w.Record)) return Volatile.Read(ref w.Consumed) == 1 ? 2 : 1;
            return 0;
        }

        internal HubNewGameSuccessorReservationV1 ReserveNewGameSuccessor(HubNewGameRearmCapabilityV1 capability,NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router,object newToken,long newEpoch)
        {
            ValidateC3State();
            if (!ReferenceEquals(owner,_confirmationOwner) || !owner.IsRearmInProgress(capability,this,presenter,router,_epochToken,_epoch) || _pendingSuccessor != null || _pendingIssued != null || _epochs == null || newToken == null || ReferenceEquals(newToken,_epochToken) || newEpoch != checked(_epoch+1)) throw new InvalidOperationException("New Game successor reservation is invalid.");
            var reservation=new HubNewGameSuccessorReservationV1();
            Successors.Add(reservation,new SuccessorWitness(this,owner,presenter,router,capability,_epochToken,newToken,_epoch,newEpoch));
            _pendingSuccessor=reservation;
            return reservation;
        }

        internal bool MatchesSuccessorReservation(HubNewGameSuccessorReservationV1 reservation,NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router,object token,long epoch)
        {
            return reservation != null && Successors.TryGetValue(reservation,out var w) && w != null && ReferenceEquals(w.Issuer,this) && ReferenceEquals(w.Owner,owner) && ReferenceEquals(w.Presenter,presenter) && ReferenceEquals(w.Router,router) && ReferenceEquals(w.NewToken,token) && w.NewEpoch == epoch && (ReferenceEquals(_pendingSuccessor,reservation) || (w.Committed && _successor != null && ReferenceEquals(_successor.Reservation,reservation))) && w.Owner.IsActualRearm(w.Capability,this,presenter,router,w.OldToken,w.OldEpoch);
        }

        internal void CommitNewGameSuccessor(HubNewGameSuccessorReservationV1 reservation,NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router,object token,long epoch)
        {
            if (!HasLiveSuccessorPermission(reservation,owner,presenter,router,token,epoch) || !presenter.HasPreparedSuccessor(reservation,token,epoch) || !Successors.TryGetValue(reservation,out var w)) throw new InvalidOperationException("New Game successor acknowledgement is invalid.");
            _successor=new SuccessorRequestSlot(epoch,token,w.Capability,reservation);
            if (_executionFresh != null) { RequireOwnedExecutionFreshWitness(_executionFresh).Closed=true; _executionCancelCurrent=true; }
            _epoch=_epochProof=epoch; _epochToken=_epochTokenProof=token;
            w.Committed=true; _pendingSuccessor=null;
        }

        // REQ-M5D7QC3-005/006: historical binding cannot authorize a new cursor.
        internal bool HasLiveSuccessorPermission(HubNewGameSuccessorReservationV1 reservation,NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router,object token,long epoch)
        {
            return MatchesSuccessorReservation(reservation,owner,presenter,router,token,epoch) && ReferenceEquals(_pendingSuccessor,reservation) && Successors.TryGetValue(reservation,out var w) && !w.Committed && ReferenceEquals(owner,_confirmationOwner) && ReferenceEquals(_epochToken,w.OldToken) && ReferenceEquals(_epochToken,_epochTokenProof) && _epoch == w.OldEpoch && _epoch == _epochProof && owner.IsRearmInProgress(w.Capability,this,presenter,router,w.OldToken,w.OldEpoch);
        }

        private void PollSuccessor()
        {
            if (_state == HubMenuIntentHandoffOwnerStateV1.Failed || (!_executionCancelCurrent && _state == HubMenuIntentHandoffOwnerStateV1.Closed) || _successor.Phase != HubMenuIntentHandoffOwnerStateV1.AwaitingIntent) return;
            try
            {
                ValidateC3State();
                HubMenuIntentRequestV1 row;
                if (!_presenter.TryTakeSuccessorIntent(this,_successor.Reservation,_successor.Token,_successor.Epoch,out row)) return;
                row.Validate(); _successor.Live=_successor.Proof=row;
                _successor.Phase=HubMenuIntentHandoffOwnerStateV1.RequestReady;
            }
            catch { Fail(); if (_confirmationOwner != null) _confirmationOwner.CloseFromPeer(); _presenter.CloseConfirmationInteraction(); }
        }

        internal void CloseConfirmationInteraction() { Close(); }
        private void ValidateC3State()
        {
            ValidateTopology(); ValidateState();
            if (_confirmationOwner == null || !ReferenceEquals(_confirmationOwner,_confirmationOwnerProof) || _epoch != _epochProof || _epoch < 1 || _epochToken == null || !ReferenceEquals(_epochToken,_epochTokenProof)) throw new InvalidOperationException("Confirmation cohort evidence is malformed.");
            if ((_executionFresh == null || _executionCancelCurrent) && _successor != null && (_epochs == null || _successor.Epoch != _epoch || !ReferenceEquals(_successor.Token,_epochToken) || _successor.Live.HasValue != _successor.Proof.HasValue || (_successor.Live.HasValue && !_successor.Live.Value.Equals(_successor.Proof.Value)) || _successor.Taken.HasValue != _successor.TakenProof.HasValue || (_successor.Taken.HasValue && !_successor.Taken.Value.Equals(_successor.TakenProof.Value)) || (_successor.Phase == HubMenuIntentHandoffOwnerStateV1.AwaitingIntent && (_successor.Live.HasValue || _successor.Taken.HasValue)) || (_successor.Phase == HubMenuIntentHandoffOwnerStateV1.RequestReady && (!_successor.Live.HasValue || _successor.Taken.HasValue)) || (_successor.Phase == HubMenuIntentHandoffOwnerStateV1.RequestTaken && (_successor.Live.HasValue || !_successor.Taken.HasValue)))) throw new InvalidOperationException("Confirmation successor evidence is malformed.");
        }

        private void OnDisable() { CloseFromLifecycle(); }
        private void OnDestroy() { CloseFromLifecycle(); }
        private void CloseFromLifecycle()
        {
            var witness=SelectOriginalExecutionFreshWitness();
            if (witness != null)
            {
                var original=RequireOwnedExecutionFreshWitness(witness);
                try { Close(); } finally { original.Owner.CloseFromPeer(); }
            }
            else { Close(); if (_confirmationOwner != null) _confirmationOwner.CloseFromPeer(); }
        }
        private FreshWitness RequireOwnedExecutionFreshWitness(FreshWitness witness)
        {
            var issued=_executionFreshHistory;
            for (var remaining=0;issued != null && !ReferenceEquals(issued.Witness,witness) && remaining<4;remaining++) issued=issued.Previous;
            if (witness == null || !ReferenceEquals(witness.Issuer,this) || witness.Reservation == null ||
                ReferenceEquals(witness.Owner,null) || ReferenceEquals(witness.Presenter,null) || ReferenceEquals(witness.Router,null) ||
                !ExecutionFreshReservations.TryGetValue(witness.Reservation,out var original) || !ReferenceEquals(original,witness))
                throw new InvalidOperationException("C4 fresh closure has no original issuer association.");
            if (issued != null && ReferenceEquals(issued.Witness,witness) &&
                (!ReferenceEquals(issued.Issuer,this) || !ReferenceEquals(issued.Reservation,witness.Reservation) ||
                 !ReferenceEquals(issued.Owner,witness.Owner) || !ReferenceEquals(issued.Presenter,witness.Presenter) ||
                 !ReferenceEquals(issued.Router,witness.Router) || !ReferenceEquals(issued.Thread,witness.Thread)))
                throw new InvalidOperationException("C4 fresh closure disagrees with its original issuance history.");
            if (!ReferenceEquals(witness.Thread,Thread.CurrentThread)) throw new InvalidOperationException("C4 fresh closure requires its original thread.");
            return witness;
        }
        private FreshWitness SelectOriginalExecutionFreshWitness()
        {
            FreshWitness latest=null;
            var history=_executionFreshHistory;
            ConsiderOriginalExecutionFreshWitness(_executionFresh,ref latest);
            if (_pendingExecutionFresh != null && ExecutionFreshReservations.TryGetValue(_pendingExecutionFresh,out var pending)) ConsiderOriginalExecutionFreshWitness(pending,ref latest);
            if (history != null && ReferenceEquals(history.Issuer,this)) ConsiderOriginalExecutionFreshWitness(history.Witness,ref latest);
            return latest;
        }
        private void ConsiderOriginalExecutionFreshWitness(FreshWitness witness,ref FreshWitness latest)
        {
            if (witness == null || !ReferenceEquals(witness.Issuer,this) || witness.Reservation == null ||
                !ExecutionFreshReservations.TryGetValue(witness.Reservation,out var original) || !ReferenceEquals(original,witness)) return;
            if (!ReferenceEquals(witness.Thread,Thread.CurrentThread)) throw new InvalidOperationException("C4 fresh closure requires its original thread.");
            if (latest == null || witness.Epoch>latest.Epoch) latest=witness;
        }
        private void Close()
        {
            var latest=SelectOriginalExecutionFreshWitness();
            try
            {
                if (latest != null)
                {
                    var witness=RequireOwnedExecutionFreshWitness(latest);
                    var wasOpen=!witness.Closed;
                    witness.Closed=true;
                    if (wasOpen)
                    {
                        bool notify;
                        try { notify=witness.Owner.State == NewGameConfirmationOwnerStateV1.ExecutionFreshPreparing; }
                        catch { witness.Owner.CloseFromPeer(); throw; }
                        if (notify) witness.Owner.CloseFromPeer();
                    }
                }
            }
            finally
            {
                _request = null; _requestProof = null;
                if (_state != HubMenuIntentHandoffOwnerStateV1.Failed) _state = HubMenuIntentHandoffOwnerStateV1.Closed;
            }
        }
        private void Fail()
        {
            SelectOriginalExecutionFreshWitness();
            try { Close(); } finally { _request = null; _requestProof = null; _state = HubMenuIntentHandoffOwnerStateV1.Failed; }
        }
        internal HubNewGameExecutionFreshReservationV1 ReserveExecutionFresh(HubNewGameExecutionFreshCapabilityV1 capability,
            NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router,object nextEpochToken,long nextEpoch)
        {
            if (ReferenceEquals(owner,null) || !owner.IsExecutionFreshInProgress(capability,this,presenter,router,nextEpochToken,nextEpoch))
                throw new InvalidOperationException("C4 fresh reservation is foreign or inactive.");
            ValidateC3State();
            if (!ReferenceEquals(owner,_confirmationOwner) || !ReferenceEquals(presenter,_presenter) || _pendingExecutionFresh != null ||
                _pendingIssued != null || _epochs == null || _state != HubMenuIntentHandoffOwnerStateV1.Closed ||
                nextEpochToken == null || ReferenceEquals(nextEpochToken,_epochToken) || nextEpoch != checked(_epoch+1))
                throw new InvalidOperationException("C4 old handoff closure is inconsistent.");
            var reservation=new HubNewGameExecutionFreshReservationV1();
            var witness=new FreshWitness(this,owner,presenter,router,capability,reservation,nextEpochToken,nextEpoch);
            var history=new FreshIssuanceHistory(_executionFreshHistory,witness);
            ExecutionFreshReservations.Add(reservation,witness); _executionFreshHistory=history; _pendingExecutionFresh=reservation; return reservation;
        }
        internal bool MatchesExecutionFreshReservation(HubNewGameExecutionFreshReservationV1 reservation,NewGameConfirmationOwnerV1 owner,
            HubMenuPresenterV1 presenter,InputRouter router,object nextEpochToken,long nextEpoch)
        {
            return reservation != null && ExecutionFreshReservations.TryGetValue(reservation,out var w) &&
                ReferenceEquals(w.Issuer,this) && ReferenceEquals(w.Owner,owner) && ReferenceEquals(w.Presenter,presenter) && ReferenceEquals(w.Router,router) &&
                ReferenceEquals(w.Token,nextEpochToken) && w.Epoch == nextEpoch && !w.Committed && !w.Closed &&
                ReferenceEquals(_pendingExecutionFresh,reservation) && ReferenceEquals(w.OldToken,_epochToken) && w.OldEpoch == _epoch &&
                _epoch == _epochProof && ReferenceEquals(_epochToken,_epochTokenProof) &&
                owner.IsExecutionFreshInProgress(w.Capability,this,presenter,router,nextEpochToken,nextEpoch);
        }
        internal void CommitExecutionFresh(HubNewGameExecutionFreshReservationV1 reservation,NewGameConfirmationOwnerV1 owner,
            HubMenuPresenterV1 presenter,InputRouter router,object nextEpochToken,long nextEpoch)
        {
            if (!MatchesExecutionFreshReservation(reservation,owner,presenter,router,nextEpochToken,nextEpoch) ||
                !presenter.MatchesPreparedExecutionFresh(owner,reservation,nextEpochToken,nextEpoch))
                throw new InvalidOperationException("C4 fresh handoff acknowledgement is unavailable.");
            if (!ExecutionFreshReservations.TryGetValue(reservation,out var witness)) throw new InvalidOperationException("C4 missing reservation.");
            RequireOwnedExecutionFreshWitness(witness);
            witness.Committed=true; _executionFresh=witness; _pendingExecutionFresh=null; _executionCancelCurrent=false;
            _epoch=_epochProof=nextEpoch; _epochToken=_epochTokenProof=nextEpochToken;
        }
        internal bool HasLiveExecutionFreshPermission(HubNewGameExecutionFreshReservationV1 reservation,NewGameConfirmationOwnerV1 owner,
            HubMenuPresenterV1 presenter,InputRouter router,object nextEpochToken,long nextEpoch)
        {
            return reservation != null && ExecutionFreshReservations.TryGetValue(reservation,out var w) &&
                ReferenceEquals(w.Issuer,this) && ReferenceEquals(w.Owner,owner) && ReferenceEquals(w.Presenter,presenter) && ReferenceEquals(w.Router,router) &&
                ReferenceEquals(_executionFresh,w) && w.Committed && !w.Closed && ReferenceEquals(w.Token,nextEpochToken) && w.Epoch == nextEpoch &&
                owner.MatchesExecutionFreshCohort(this,presenter,router,reservation,nextEpochToken,nextEpoch) &&
                _epoch == _epochProof && _epoch == nextEpoch && ReferenceEquals(_epochToken,_epochTokenProof) && ReferenceEquals(_epochToken,nextEpochToken);
        }
        private void ValidateExecutionFresh()
        {
            var w=_executionFresh;
            if (w == null || !HasLiveExecutionFreshPermission(w.Reservation,w.Owner,w.Presenter,w.Router,w.Token,w.Epoch) ||
                w.Live.HasValue != w.Proof.HasValue || (w.Live.HasValue && !w.Live.Value.Equals(w.Proof.Value)) ||
                w.Taken.HasValue != w.TakenProof.HasValue || (w.Taken.HasValue && !w.Taken.Value.Equals(w.TakenProof.Value)) ||
                (w.Phase == HubMenuIntentHandoffOwnerStateV1.AwaitingIntent && (w.Live.HasValue || w.Taken.HasValue)) ||
                (w.Phase == HubMenuIntentHandoffOwnerStateV1.RequestReady && (!w.Live.HasValue || w.Taken.HasValue)) ||
                (w.Phase == HubMenuIntentHandoffOwnerStateV1.RequestTaken && (w.Live.HasValue || !w.Taken.HasValue)))
                throw new InvalidOperationException("C4 fresh handoff evidence is inconsistent.");
        }
        private void PollExecutionFresh()
        {
            var w=_executionFresh;
            if (w.Closed || w.Phase != HubMenuIntentHandoffOwnerStateV1.AwaitingIntent ||
                !HasLiveExecutionFreshPermission(w.Reservation,w.Owner,w.Presenter,w.Router,w.Token,w.Epoch)) return;
            try
            {
                ValidateExecutionFresh(); ValidateTopology();
                if (!_presenter.TryTakeExecutionFreshIntent(this,w.Reservation,w.Token,w.Epoch,out var row)) return;
                row.Validate(); w.Live=w.Proof=row; w.Phase=HubMenuIntentHandoffOwnerStateV1.RequestReady;
            }
            catch { Fail(); _confirmationOwner.CloseFromPeer(); _presenter.CloseConfirmationInteraction(); }
        }
        private bool TryTakeExecutionFresh(NewGameConfirmationOwnerV1 owner,HubMenuPresenterV1 presenter,InputRouter router,out IssuedNewGameRequestV1 issued)
        {
            issued=null; var w=_executionFresh;
            if (w == null || !HasLiveExecutionFreshPermission(w.Reservation,owner,presenter,router,w.Token,w.Epoch)) return false;
            if (Interlocked.CompareExchange(ref _c3Operation,1,0) != 0) throw new InvalidOperationException("C4 fresh take is already active.");
            try
            {
                if (!HasLiveExecutionFreshPermission(w.Reservation,owner,presenter,router,w.Token,w.Epoch)) return false;
                try
                {
                    ValidateExecutionFresh(); ValidateC3State();
                    if (!owner.CanBindIssuance(w.Token,w.Epoch,true) || w.Phase != HubMenuIntentHandoffOwnerStateV1.RequestReady) return false;
                    var row=w.Live.Value; row.Validate(); if (row.Item != HubMenuItemV1.NewGame || !presenter.MatchesImmutableHandoff(row.Receipt)) return false;
                    w.Taken=w.TakenProof=row; w.Live=w.Proof=null; w.Phase=HubMenuIntentHandoffOwnerStateV1.RequestTaken;
                    var handle=new IssuedNewGameRequestV1(); var record=new TakenEpochRecord(_epochs,w.Epoch,w.Token,handle,row,null);
                    _epochs=record; Issuances.Add(handle,new IssuanceWitness(this,owner,presenter,router,record)); _pendingIssued=handle;
                    owner.BindIssued(handle,w.Token,w.Epoch);
                    if (!owner.HasPendingIssued(handle,w.Token,w.Epoch)) throw new InvalidOperationException("C4 fresh reciprocal bind failed.");
                    issued=handle; return true;
                }
                catch { Fail(); owner.CloseFromPeer(); presenter.CloseConfirmationInteraction(); throw; }
            }
            finally { Volatile.Write(ref _c3Operation,0); }
        }
        private void ValidateTopology()
        {
            if (_presenter == null || !ReferenceEquals(_presenter.gameObject, gameObject) || !ReferenceEquals(_presenter.HandoffOwner, this) || gameObject.GetComponents<HubMenuIntentHandoffOwnerV1>().Length != 1)
                throw new InvalidOperationException("Hub handoff owner topology is malformed.");
        }
        private void ValidateState()
        {
            if (_state < HubMenuIntentHandoffOwnerStateV1.AwaitingIntent || _state > HubMenuIntentHandoffOwnerStateV1.Closed)
                throw new InvalidOperationException("Hub handoff owner state is malformed.");
            if (_request.HasValue != _requestProof.HasValue || (_request.HasValue && !_request.Value.Equals(_requestProof.Value)) || _takenRequest.HasValue != _takenRequestProof.HasValue || (_takenRequest.HasValue && !_takenRequest.Value.Equals(_takenRequestProof.Value)))
                throw new InvalidOperationException("Hub handoff owner evidence is malformed.");
            if (_state == HubMenuIntentHandoffOwnerStateV1.AwaitingIntent && (_request.HasValue || _takenRequest.HasValue)) throw new InvalidOperationException("Hub handoff owner awaiting state is malformed.");
            if (_state == HubMenuIntentHandoffOwnerStateV1.RequestReady && (!_request.HasValue || _takenRequest.HasValue)) throw new InvalidOperationException("Hub handoff owner ready state is malformed.");
            if (_state == HubMenuIntentHandoffOwnerStateV1.RequestTaken && (_request.HasValue || !_takenRequest.HasValue)) throw new InvalidOperationException("Hub handoff owner taken state is malformed.");
            if ((_state == HubMenuIntentHandoffOwnerStateV1.Failed || _state == HubMenuIntentHandoffOwnerStateV1.Closed) && _request.HasValue) throw new InvalidOperationException("Hub handoff owner terminal state is malformed.");
            if (_request.HasValue) _request.Value.Validate();
            if (_takenRequest.HasValue) _takenRequest.Value.Validate();
        }
    }
}
