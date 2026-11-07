using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using AcadeGameMaker.Profile;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-M5D7QC3-001/002/004/007: engine-side C3 observation boundary only.
    // It has no Hub request, UI, mutation, C1 Begin, C2, map, or scene authority.
    internal enum ProfileNewGameCaptureOutcomeV1 { Captured = 1, Busy = 2, Unreadable = 3 }
    internal enum NewGameObservationClassificationV1 { NoConfirmationRequired = 1, ConfirmationRequiredMeaningful = 2, ConfirmationRequiredAmbiguous = 3 }
    internal enum ProfileNewGameConfirmedDispositionV1 { Confirmed = 1, FreshDecisionRequired = 2 }

    // REQ-M5D7QC3-001/004/006: opaque Input.Unity values.  They expose no
    // Profile identity/root/document and grant no C1 Begin or C4 authority.
    internal sealed class ConfirmedProfileResetRequestV1 { internal ConfirmedProfileResetRequestV1() { } }
    internal sealed class ProfileNewGameExecutionCommitReservationV1 { internal ProfileNewGameExecutionCommitReservationV1() { } }
    internal sealed class ProfileNewGameConfirmedResolutionV1
    {
        private readonly ProfileNewGameConfirmedDispositionV1 _disposition;
        private readonly NewGameObservationClassificationV1 _classification;
        private readonly ConfirmedProfileResetRequestV1 _request;
        internal ProfileNewGameConfirmedResolutionV1(ProfileNewGameConfirmedDispositionV1 disposition, NewGameObservationClassificationV1 classification, ConfirmedProfileResetRequestV1 request)
        { _disposition=disposition; _classification=classification; _request=request; }
        internal ProfileNewGameConfirmedDispositionV1 Disposition { get { Validate(); return _disposition; } }
        internal NewGameObservationClassificationV1 FreshClassification { get { Validate(); return _classification; } }
        internal ConfirmedProfileResetRequestV1 ConfirmedRequest { get { Validate(); if (_disposition != ProfileNewGameConfirmedDispositionV1.Confirmed || _request == null) throw new InvalidOperationException("New Game resolution has no confirmed request."); return _request; } }
        internal bool MatchesIssued(ProfileNewGameConfirmedDispositionV1 disposition, NewGameObservationClassificationV1 classification, ConfirmedProfileResetRequestV1 request) { return _disposition == disposition && _classification == classification && ReferenceEquals(_request, request); }
        private void Validate()
        {
            if (!ProfileNewGameConfirmationV1.ValidateIssuedResolution(this) || _disposition < ProfileNewGameConfirmedDispositionV1.Confirmed || _disposition > ProfileNewGameConfirmedDispositionV1.FreshDecisionRequired || _classification < NewGameObservationClassificationV1.NoConfirmationRequired || _classification > NewGameObservationClassificationV1.ConfirmationRequiredAmbiguous || (_disposition == ProfileNewGameConfirmedDispositionV1.Confirmed) != (_request != null)) throw new InvalidOperationException("New Game confirmed resolution is malformed.");
        }
    }

    internal sealed class ProfileNewGameCaptureResultV1
    {
        private readonly object _owner, _epoch;
        private readonly string _root;
        private readonly ProfileResetConfirmationCaptureV1 _capture;
        private readonly ProfileNewGameCaptureOutcomeV1 _outcome;
        private readonly NewGameObservationClassificationV1? _classification;
        internal ProfileNewGameCaptureResultV1(object owner, object epoch, string root, ProfileResetConfirmationCaptureV1 capture, ProfileNewGameCaptureOutcomeV1 outcome, NewGameObservationClassificationV1? classification)
        { _owner = owner; _epoch = epoch; _root = root; _capture = capture; _outcome = outcome; _classification = classification; }
        internal ProfileNewGameCaptureOutcomeV1 Outcome { get { Validate(); return _outcome; } }
        internal bool MatchesIssued(object owner, object epoch, string root, ProfileResetConfirmationCaptureV1 capture, ProfileNewGameCaptureOutcomeV1 outcome, NewGameObservationClassificationV1? classification) { return ReferenceEquals(_owner, owner) && ReferenceEquals(_epoch, epoch) && string.Equals(_root, root, StringComparison.Ordinal) && ReferenceEquals(_capture, capture) && _outcome == outcome && _classification == classification; }
        internal NewGameObservationClassificationV1 Classification { get { Validate(); if (!_classification.HasValue) throw new InvalidOperationException("New Game observation has no classification."); return _classification.Value; } }
        internal bool IsOwnedBy(object owner, object epoch) { Validate(); return owner != null && epoch != null && ReferenceEquals(_owner, owner) && ReferenceEquals(_epoch, epoch); }
        private void Validate()
        {
            if (!ProfileNewGameConfirmationV1.ValidateIssuedResult(this)) throw new InvalidOperationException("New Game capture result was not issued by the lower observation boundary.");
            if (_owner == null || _epoch == null || _outcome < ProfileNewGameCaptureOutcomeV1.Captured || _outcome > ProfileNewGameCaptureOutcomeV1.Unreadable || (_classification.HasValue && (_classification.Value < NewGameObservationClassificationV1.NoConfirmationRequired || _classification.Value > NewGameObservationClassificationV1.ConfirmationRequiredAmbiguous))) throw new InvalidOperationException("New Game capture result is malformed.");
            if (_outcome == ProfileNewGameCaptureOutcomeV1.Captured) { if (string.IsNullOrEmpty(_root) || _capture == null || !_classification.HasValue) throw new InvalidOperationException("New Game captured result is malformed."); _capture.Validate(); if (!string.Equals(_root, _capture.Identity.Root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("New Game capture root disagrees with its identity."); }
            else if (_root != null || _capture != null || _classification.HasValue) throw new InvalidOperationException("New Game failed capture leaked observation evidence.");
        }
    }

    internal static partial class ProfileNewGameConfirmationV1
    {
        private sealed class ResultWitness
        {
            internal readonly DesktopProfileLaunchAdapterV1 Adapter; internal readonly InputRouter Router;
            internal readonly object Owner, Epoch; internal readonly string Root; internal readonly ProfileResetConfirmationCaptureV1 Capture;
            internal readonly ProfileNewGameCaptureOutcomeV1 Outcome; internal readonly NewGameObservationClassificationV1? Classification;
            internal int RequestMintState;
            internal ResultWitness(DesktopProfileLaunchAdapterV1 adapter, InputRouter router, object owner, object epoch, string root, ProfileResetConfirmationCaptureV1 capture, ProfileNewGameCaptureOutcomeV1 outcome, NewGameObservationClassificationV1? classification)
            { Adapter=adapter; Router=router; Owner=owner; Epoch=epoch; Root=root; Capture=capture; Outcome=outcome; Classification=classification; }
        }
        private sealed class ConfirmedRequestWitness
        {
            internal readonly DesktopProfileLaunchAdapterV1 Adapter; internal readonly InputRouter Router;
            internal readonly object Owner, Epoch; internal readonly long InteractionGeneration, DecisionGeneration;
            internal readonly string Root; internal readonly ProfileResetConfirmationCaptureV1 Capture;
            internal readonly ProfileNewGameCaptureResultV1 SourceResult;
            internal readonly ResultWitness SourceWitness;
            // REQ-M5D7QC4-001: 이 값은 C4 입력의 투영이며 독립 발급 이력과 대조한다.
            internal readonly Thread BirthThreadProjection;
            internal int State;
            internal ConfirmedRequestWitness(DesktopProfileLaunchAdapterV1 adapter, InputRouter router, object owner, object epoch, long interactionGeneration, long decisionGeneration, string root, ProfileResetConfirmationCaptureV1 capture, ProfileNewGameCaptureResultV1 sourceResult, ResultWitness sourceWitness)
            { Adapter=adapter; Router=router; Owner=owner; Epoch=epoch; InteractionGeneration=interactionGeneration; DecisionGeneration=decisionGeneration; Root=root; Capture=capture; SourceResult=sourceResult; SourceWitness=sourceWitness; BirthThreadProjection=Thread.CurrentThread; }
        }
        private sealed class ReservationWitness
        {
            internal readonly ConfirmedProfileResetRequestV1 Request; internal readonly ConfirmedRequestWitness RequestWitness;
            internal ReservationWitness(ConfirmedProfileResetRequestV1 request, ConfirmedRequestWitness requestWitness) { Request=request; RequestWitness=requestWitness; }
        }
        private sealed class ResolutionWitness
        {
            internal readonly ProfileNewGameConfirmedDispositionV1 Disposition; internal readonly NewGameObservationClassificationV1 Classification; internal readonly ConfirmedProfileResetRequestV1 Request;
            internal ResolutionWitness(ProfileNewGameConfirmedDispositionV1 disposition, NewGameObservationClassificationV1 classification, ConfirmedProfileResetRequestV1 request) { Disposition=disposition; Classification=classification; Request=request; }
        }
        private enum EventKind { Fresh, MintAttempt, MintIssued, RequestIssued, ReserveAttempt, Reserved, Completed, Closed }
        private sealed class LifecycleEvent
        {
            internal readonly EventKind Kind;
            internal readonly ProfileNewGameCaptureResultV1 Result;
            internal readonly ResultWitness ResultWitness;
            internal readonly ConfirmedProfileResetRequestV1 Request;
            internal readonly ConfirmedRequestWitness RequestWitness;
            internal readonly ProfileNewGameExecutionCommitReservationV1 Reservation;
            internal readonly ReservationWitness ReservationWitness;
            internal readonly ProfileNewGameConfirmedResolutionV1 Resolution;
            internal readonly LifecycleEvent Previous;
            internal readonly Thread IssuerThreadAtIssue;
            internal LifecycleEvent(EventKind kind, ProfileNewGameCaptureResultV1 result, ResultWitness resultWitness,
                ConfirmedProfileResetRequestV1 request = null, ConfirmedRequestWitness requestWitness = null,
                ProfileNewGameExecutionCommitReservationV1 reservation = null, ReservationWitness reservationWitness = null,
                ProfileNewGameConfirmedResolutionV1 resolution = null, LifecycleEvent previous = null)
            { Kind=kind; Result=result; ResultWitness=resultWitness; Request=request; RequestWitness=requestWitness;
              Reservation=reservation; ReservationWitness=reservationWitness; Resolution=resolution; Previous=previous;
              IssuerThreadAtIssue=kind == EventKind.RequestIssued ? Thread.CurrentThread : null; }
        }
        private sealed class MintLifecycle
        {
            internal readonly object Gate = new object();
            internal readonly ResultWitness Witness;
            internal readonly LifecycleEvent FailureClosed;
            internal LifecycleEvent Head, ClosedHistory;
            internal MintLifecycle(ProfileNewGameCaptureResultV1 result, ResultWitness witness)
            { Witness=witness; Head=new LifecycleEvent(EventKind.Fresh,result,witness); FailureClosed=new LifecycleEvent(EventKind.Closed,result,witness); }
        }
        private sealed class RequestLifecycle
        {
            internal readonly object Gate = new object();
            internal readonly Thread IssuerThreadAnchor = Thread.CurrentThread;
            internal readonly long IssuerInteractionGeneration, IssuerDecisionGeneration;
            internal readonly ConfirmedRequestWitness Witness;
            internal readonly LifecycleEvent FailureClosed;
            internal LifecycleEvent Head, ClosedHistory;
            internal RequestLifecycle(ConfirmedProfileResetRequestV1 request, ConfirmedRequestWitness witness)
            { Witness=witness; IssuerInteractionGeneration=witness.InteractionGeneration; IssuerDecisionGeneration=witness.DecisionGeneration;
              Head=new LifecycleEvent(EventKind.RequestIssued,witness.SourceResult,witness.SourceWitness,request,witness);
              FailureClosed=new LifecycleEvent(EventKind.Closed,witness.SourceResult,witness.SourceWitness,request,witness); }
        }
        private static readonly ConditionalWeakTable<ResultWitness, MintLifecycle> MintLifecycles = new ConditionalWeakTable<ResultWitness, MintLifecycle>();
        private static readonly ConditionalWeakTable<ConfirmedRequestWitness, RequestLifecycle> RequestLifecycles = new ConditionalWeakTable<ConfirmedRequestWitness, RequestLifecycle>();
        private static readonly ConditionalWeakTable<ReservationWitness, RequestLifecycle> ReservationLifecycles = new ConditionalWeakTable<ReservationWitness, RequestLifecycle>();
        private static readonly ConditionalWeakTable<ProfileNewGameCaptureResultV1, ResultWitness> IssuedResults = new ConditionalWeakTable<ProfileNewGameCaptureResultV1, ResultWitness>();
        private static readonly ConditionalWeakTable<ConfirmedProfileResetRequestV1, ConfirmedRequestWitness> IssuedConfirmedRequests = new ConditionalWeakTable<ConfirmedProfileResetRequestV1, ConfirmedRequestWitness>();
        private static readonly ConditionalWeakTable<ProfileNewGameExecutionCommitReservationV1, ReservationWitness> IssuedReservations = new ConditionalWeakTable<ProfileNewGameExecutionCommitReservationV1, ReservationWitness>();
        private static readonly ConditionalWeakTable<ProfileNewGameConfirmedResolutionV1, ResolutionWitness> IssuedResolutions = new ConditionalWeakTable<ProfileNewGameConfirmedResolutionV1, ResolutionWitness>();
        internal static bool ValidateIssuedResult(ProfileNewGameCaptureResultV1 r) { return r != null && IssuedResults.TryGetValue(r,out var w) && w != null && r.MatchesIssued(w.Owner,w.Epoch,w.Root,w.Capture,w.Outcome,w.Classification); }
        internal static bool ValidateIssuedResolution(ProfileNewGameConfirmedResolutionV1 r) { return r != null && IssuedResolutions.TryGetValue(r,out var w) && w != null && r.MatchesIssued(w.Disposition,w.Classification,w.Request); }
        private static ProfileNewGameCaptureResultV1 Issue(DesktopProfileLaunchAdapterV1 adapter, InputRouter router, object owner, object epoch, string root, ProfileResetConfirmationCaptureV1 capture, ProfileNewGameCaptureOutcomeV1 outcome, NewGameObservationClassificationV1? classification)
        {
            var r=new ProfileNewGameCaptureResultV1(owner,epoch,root,capture,outcome,classification);
            var witness=new ResultWitness(adapter,router,owner,epoch,root,capture,outcome,classification);
            var lifecycle=new MintLifecycle(r,witness);
            MintLifecycles.Add(witness,lifecycle);
            IssuedResults.Add(r,witness);
            return r;
        }
        private static ProfileNewGameConfirmedResolutionV1 IssueResolution(ProfileNewGameConfirmedDispositionV1 disposition, NewGameObservationClassificationV1 classification, ConfirmedProfileResetRequestV1 request) { var r=new ProfileNewGameConfirmedResolutionV1(disposition,classification,request); IssuedResolutions.Add(r,new ResolutionWitness(disposition,classification,request)); return r; }
        internal static ProfileNewGameCaptureResultV1 Capture(DesktopProfileLaunchAdapterV1 adapter, InputRouter router, object owner, object epoch)
        {
            if (adapter == null || router == null || owner == null || epoch == null) throw new ArgumentNullException("C3 capture cohort is absent.");
            try
            {
                var root = adapter.GetAuthenticatedLaunchRootForNewGame(router);
                var capture = ProfileResetDiskTransactionV1.CaptureConfirmationObservation(root);
                capture.Validate();
                return Issue(adapter, router, owner, epoch, root, capture, ProfileNewGameCaptureOutcomeV1.Captured, Classify(capture));
            }
            catch (IOException error) when (ProfileRootOperationLockV1.IsAuthenticatedObservationContentionTimeout(error))
            { return Issue(adapter, router, owner, epoch, null, null, ProfileNewGameCaptureOutcomeV1.Busy, null); }
            catch (IOException) { return Issue(adapter, router, owner, epoch, null, null, ProfileNewGameCaptureOutcomeV1.Unreadable, null); }
            catch (UnauthorizedAccessException) { return Issue(adapter, router, owner, epoch, null, null, ProfileNewGameCaptureOutcomeV1.Unreadable, null); }
            catch (System.Security.SecurityException) { return Issue(adapter, router, owner, epoch, null, null, ProfileNewGameCaptureOutcomeV1.Unreadable, null); }
        }

        // REQ-M5D7QC3-002/004: the two captures must be genuine lower rows
        // for the same active cohort and opaque owner/epoch. This is comparison
        // only: it cannot issue confirmation or execution authority.
        internal static bool MatchesRecapturedIdentity(
            DesktopProfileLaunchAdapterV1 adapter, InputRouter router, object owner, object epoch,
            ProfileNewGameCaptureResultV1 displayed, ProfileNewGameCaptureResultV1 recaptured)
        {
            var left = GetCapturedWitness(adapter, router, displayed, owner, epoch);
            var right = GetCapturedWitness(adapter, router, recaptured, owner, epoch);
            var root = GetAuthenticatedRoot(adapter, router);
            if (!string.Equals(left.Root, root, StringComparison.OrdinalIgnoreCase) || !string.Equals(right.Root, root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("New Game capture root does not match the active cohort.");
            return SameIdentity(left.Capture, right.Capture);
        }

        // REQ-M5D7QC3-001/002: no-prompt confirmation is minted only from one
        // actual NoConfirmationRequired capture. The opaque tokens are lower
        // binding evidence, not a claim that Hub/Q-B authorization is proved.
        internal static ProfileNewGameConfirmedResolutionV1 IssueNoConfirmationRequired(
            ProfileNewGameCaptureResultV1 capture, DesktopProfileLaunchAdapterV1 adapter, InputRouter router,
            object ownerToken, object epochToken, long interactionGeneration, long decisionGeneration)
        {
            var witness = GetCapturedWitness(adapter, router, capture, ownerToken, epochToken);
            RequireGenerations(interactionGeneration, decisionGeneration);
            if (witness.Classification != NewGameObservationClassificationV1.NoConfirmationRequired) throw new InvalidOperationException("New Game capture requires a confirmation decision.");
            return IssueConfirmedFromFreshCapture(capture, witness, adapter, router, ownerToken, epochToken, interactionGeneration, decisionGeneration);
        }

        // REQ-M5D7QC3-002/004: both rows must be independently issued captures
        // from the same exact lower cohort. The returned resolution exposes no
        // identity, root, decoded document, or execution authority.
        internal static ProfileNewGameConfirmedResolutionV1 ResolveConfirmedRecapture(
            ProfileNewGameCaptureResultV1 displayedCapture, ProfileNewGameCaptureResultV1 freshCapture,
            DesktopProfileLaunchAdapterV1 adapter, InputRouter router, object ownerToken, object epochToken,
            long interactionGeneration, long decisionGeneration)
        {
            if (ReferenceEquals(displayedCapture, freshCapture)) throw new InvalidOperationException("New Game confirmation requires a distinct fresh capture.");
            var displayed = GetCapturedWitness(adapter, router, displayedCapture, ownerToken, epochToken);
            var fresh = GetCapturedWitness(adapter, router, freshCapture, ownerToken, epochToken);
            RequireGenerations(interactionGeneration, decisionGeneration);
            var root = GetAuthenticatedRoot(adapter, router);
            if (!string.Equals(displayed.Root, root, StringComparison.OrdinalIgnoreCase) || !string.Equals(fresh.Root, root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("New Game recapture root does not match the active cohort.");
            if (SameIdentity(displayed.Capture, fresh.Capture)) return IssueConfirmedFromFreshCapture(freshCapture, fresh, adapter, router, ownerToken, epochToken, interactionGeneration, decisionGeneration);
            if (fresh.Classification == NewGameObservationClassificationV1.NoConfirmationRequired) return IssueConfirmedFromFreshCapture(freshCapture, fresh, adapter, router, ownerToken, epochToken, interactionGeneration, decisionGeneration);
            return IssueResolution(ProfileNewGameConfirmedDispositionV1.FreshDecisionRequired, fresh.Classification.Value, null);
        }

        // REQ-M5D7QC3-006: this is only the lower half of reciprocal commit.
        // It cannot prove Hub closure, expose an identity, consume for C4, or
        // invoke C1. Future owner code must close its own authority between
        // reserve and complete and present the exact same opaque bindings.
        internal static ProfileNewGameExecutionCommitReservationV1 ReserveExecutionCommit(
            ConfirmedProfileResetRequestV1 request, DesktopProfileLaunchAdapterV1 adapter, InputRouter router,
            object ownerToken, object epochToken, long interactionGeneration, long decisionGeneration)
        {
            var witness = GetConfirmedRequestWitness(request, adapter, router, ownerToken, epochToken, interactionGeneration, decisionGeneration);
            var lifecycle = GetRequestLifecycle(witness);
            lock (lifecycle.Gate)
            {
                ValidateRequestHistory(request,witness,lifecycle);
                // A normal duplicate/concurrent loser leaves the winner untouched.
                if (lifecycle.Head.Kind != EventKind.RequestIssued) throw new InvalidOperationException("New Game confirmed request cannot reserve execution commit.");
                try
                {
                    ValidateRequestAtActiveRoot(witness, adapter, router);
                    var reservation = new ProfileNewGameExecutionCommitReservationV1();
                    var reservationWitness = new ReservationWitness(request,witness);
                    var attempt = new LifecycleEvent(EventKind.ReserveAttempt,witness.SourceResult,witness.SourceWitness,request,witness,reservation,reservationWitness,previous:lifecycle.Head);
                    var reserved = new LifecycleEvent(EventKind.Reserved,witness.SourceResult,witness.SourceWitness,request,witness,reservation,reservationWitness,previous:attempt);
                    if (Interlocked.CompareExchange(ref witness.State,1,0) != 0) throw new InvalidOperationException("New Game reserve state disagrees with its history.");
                    lifecycle.Head=attempt;
                    IssuedReservations.Add(reservation,reservationWitness);
                    ReservationLifecycles.Add(reservationWitness,lifecycle);
                    lifecycle.Head=reserved;
                    return reservation;
                }
                catch { CloseAuthenticatedFailure(witness,lifecycle); throw; }
            }
        }

        internal static ConfirmedProfileResetRequestV1 CompleteExecutionCommit(
            ConfirmedProfileResetRequestV1 request, ProfileNewGameExecutionCommitReservationV1 reservation,
            DesktopProfileLaunchAdapterV1 adapter, InputRouter router, object ownerToken, object epochToken,
            long interactionGeneration, long decisionGeneration)
        {
            var witness = GetConfirmedRequestWitness(request, adapter, router, ownerToken, epochToken, interactionGeneration, decisionGeneration);
            var lifecycle = GetRequestLifecycle(witness);
            lock (lifecycle.Gate)
            {
                ValidateRequestHistory(request,witness,lifecycle);
                if (lifecycle.Head.Kind == EventKind.Completed || lifecycle.Head.Kind == EventKind.Closed) throw new InvalidOperationException("New Game execution commit is already terminal.");
                try
                {
                    if (reservation == null || !IssuedReservations.TryGetValue(reservation, out var reservationWitness) || reservationWitness == null || !ReferenceEquals(reservationWitness.Request, request) || !ReferenceEquals(reservationWitness.RequestWitness, witness) || lifecycle.Head.Kind != EventKind.Reserved || !ReferenceEquals(lifecycle.Head.Reservation,reservation) || !ReferenceEquals(lifecycle.Head.ReservationWitness,reservationWitness) || !ReservationLifecycles.TryGetValue(reservationWitness,out var boundLifecycle) || !ReferenceEquals(boundLifecycle,lifecycle)) throw new InvalidOperationException("New Game execution reservation is not issued for this request.");
                    ValidateRequestAtActiveRoot(witness, adapter, router);
                    var completed=new LifecycleEvent(EventKind.Completed,witness.SourceResult,witness.SourceWitness,request,witness,reservation,reservationWitness,previous:lifecycle.Head);
                    if (Interlocked.CompareExchange(ref witness.State,2,1) != 1) throw new InvalidOperationException("New Game execution commit is not reserved.");
                    lifecycle.Head=completed;
                    return request;
                }
                catch { CloseAuthenticatedFailure(witness,lifecycle); throw; }
            }
        }

        // A failed upper reciprocal step must close the lower request rather
        // than restore Issued. This terminal operation creates no authority.
        internal static void CloseExecutionCommit(
            ConfirmedProfileResetRequestV1 request, DesktopProfileLaunchAdapterV1 adapter, InputRouter router,
            object ownerToken, object epochToken, long interactionGeneration, long decisionGeneration)
        {
            var witness = GetConfirmedRequestWitness(request, adapter, router, ownerToken, epochToken, interactionGeneration, decisionGeneration);
            var lifecycle=GetRequestLifecycle(witness);
            lock (lifecycle.Gate)
            {
                ValidateRequestHistory(request,witness,lifecycle);
                if (lifecycle.Head.Kind == EventKind.Completed || lifecycle.Head.Kind == EventKind.Closed) throw new InvalidOperationException("New Game execution request is already terminal.");
                CloseAuthenticatedFailure(witness,lifecycle);
            }
        }

        private static ProfileNewGameConfirmedResolutionV1 IssueConfirmedFromFreshCapture(ProfileNewGameCaptureResultV1 sourceResult, ResultWitness fresh, DesktopProfileLaunchAdapterV1 adapter, InputRouter router, object ownerToken, object epochToken, long interactionGeneration, long decisionGeneration)
        {
            if (!MintLifecycles.TryGetValue(fresh,out var lifecycle) || lifecycle == null || !ReferenceEquals(lifecycle.Witness,fresh)) throw new InvalidOperationException("New Game mint history is absent.");
            lock (lifecycle.Gate)
            {
                var head=lifecycle.Head;
                var expected=head.Kind == EventKind.Fresh ? 0 : head.Kind == EventKind.Closed ? 2 : 1;
                if (!ReferenceEquals(head.Result,sourceResult) || !ReferenceEquals(head.ResultWitness,fresh) || fresh.RequestMintState != expected)
                { CloseMintFailure(fresh,lifecycle); throw new InvalidOperationException("New Game mint state disagrees with its history."); }
                if (head.Kind != EventKind.Fresh) throw new InvalidOperationException("New Game capture already attempted to mint a confirmed request.");
                ConfirmedRequestWitness requestWitness=null; RequestLifecycle requestLifecycle=null;
                try
                {
                    RequireActiveCaptureRoot(fresh, adapter, router);
                    var attempt=new LifecycleEvent(EventKind.MintAttempt,sourceResult,fresh,previous:head);
                    if (Interlocked.CompareExchange(ref fresh.RequestMintState,1,0) != 0) throw new InvalidOperationException("New Game mint state changed without history.");
                    lifecycle.Head=attempt;
                    var request=new ConfirmedProfileResetRequestV1();
                    requestWitness=new ConfirmedRequestWitness(adapter,router,ownerToken,epochToken,interactionGeneration,decisionGeneration,fresh.Root,fresh.Capture,sourceResult,fresh);
                    requestLifecycle=new RequestLifecycle(request,requestWitness);
                    // Preserve actual constructed pairs BEFORE registration can fail.
                    lifecycle.Head=new LifecycleEvent(EventKind.MintAttempt,sourceResult,fresh,request,requestWitness,previous:attempt);
                    RequestLifecycles.Add(requestWitness,requestLifecycle);
                    IssuedConfirmedRequests.Add(request,requestWitness);
                    var resolution=IssueResolution(ProfileNewGameConfirmedDispositionV1.Confirmed,fresh.Classification.Value,request);
                    var issued=new LifecycleEvent(EventKind.MintIssued,sourceResult,fresh,request,requestWitness,resolution:resolution,previous:lifecycle.Head);
                    requestLifecycle.Head=new LifecycleEvent(EventKind.RequestIssued,sourceResult,fresh,request,requestWitness,previous:issued);
                    lifecycle.Head=issued;
                    return resolution;
                }
                catch
                {
                    // This request is unpublished: no second lifecycle lock is needed.
                    if (requestLifecycle != null) CloseAuthenticatedFailure(requestWitness,requestLifecycle);
                    CloseMintFailure(fresh,lifecycle);
                    throw;
                }
            }
        }

        private static ResultWitness GetCapturedWitness(DesktopProfileLaunchAdapterV1 adapter, InputRouter router, ProfileNewGameCaptureResultV1 result, object owner, object epoch)
        {
            if (result == null || adapter == null || router == null || owner == null || epoch == null || !IssuedResults.TryGetValue(result, out var witness) || witness == null ||
                witness.Outcome != ProfileNewGameCaptureOutcomeV1.Captured || witness.Classification == null || !
                ReferenceEquals(witness.Adapter, adapter) || !ReferenceEquals(witness.Router, router) || !ReferenceEquals(witness.Owner, owner) || !ReferenceEquals(witness.Epoch, epoch) || !result.MatchesIssued(witness.Owner, witness.Epoch, witness.Root, witness.Capture, witness.Outcome, witness.Classification)) throw new InvalidOperationException("New Game capture is not an issued captured row for this owner.");
            result.Outcome.ToString();
            witness.Capture.Validate();
            if (string.IsNullOrEmpty(witness.Root) || !string.Equals(witness.Root, witness.Capture.Identity.Root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("New Game capture witness is malformed.");
            if (witness.Classification != Classify(witness.Capture)) throw new InvalidOperationException("New Game capture classification disagrees with its evidence.");
            return witness;
        }

        private static void RequireActiveCaptureRoot(ResultWitness witness, DesktopProfileLaunchAdapterV1 adapter, InputRouter router)
        {
            var root = GetAuthenticatedRoot(adapter, router);
            if (!string.Equals(witness.Root, root, StringComparison.OrdinalIgnoreCase) || witness.Capture == null || !string.Equals(witness.Capture.Identity.Root, root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("New Game capture root does not match the active cohort.");
        }

        private static void ValidateRequestAtActiveRoot(ConfirmedRequestWitness witness, DesktopProfileLaunchAdapterV1 adapter, InputRouter router)
        {
            var source = GetCapturedWitness(adapter, router, witness.SourceResult, witness.Owner, witness.Epoch);
            if (!ReferenceEquals(source, witness.SourceWitness) || !ReferenceEquals(source.Capture, witness.Capture) || !string.Equals(source.Root, witness.Root, StringComparison.Ordinal)) throw new InvalidOperationException("New Game request lost its original issuance evidence.");
            witness.Capture.Validate();
            var root = GetAuthenticatedRoot(adapter, router);
            if (!string.Equals(witness.Root, root, StringComparison.OrdinalIgnoreCase) || !string.Equals(witness.Capture.Identity.Root, root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("New Game confirmed request root is no longer authenticated.");
        }

        private static RequestLifecycle GetRequestLifecycle(ConfirmedRequestWitness witness)
        {
            if (!RequestLifecycles.TryGetValue(witness,out var lifecycle) || lifecycle == null || !ReferenceEquals(lifecycle.Witness,witness)) throw new InvalidOperationException("New Game request history is absent.");
            return lifecycle;
        }

        private static void ValidateRequestHistory(ConfirmedProfileResetRequestV1 request, ConfirmedRequestWitness witness, RequestLifecycle lifecycle)
        {
            var head=lifecycle.Head;
            var expected=head.Kind == EventKind.RequestIssued ? 0 : head.Kind == EventKind.Reserved ? 1 : head.Kind == EventKind.Completed ? 2 : head.Kind == EventKind.Closed ? 3 : -1;
            if (expected < 0 || witness.State != expected || !ReferenceEquals(head.Request,request) || !ReferenceEquals(head.RequestWitness,witness) || !ReferenceEquals(head.Result,witness.SourceResult) || !ReferenceEquals(head.ResultWitness,witness.SourceWitness))
            { CloseAuthenticatedFailure(witness,lifecycle); throw new InvalidOperationException("New Game request state disagrees with its immutable history."); }
            if (head.Kind == EventKind.Closed) return;
            var issued=head;
            if (issued.Kind == EventKind.Completed) issued=issued.Previous;
            if (issued != null && issued.Kind == EventKind.Reserved)
            {
                if (issued.Reservation == null || issued.ReservationWitness == null || !IssuedReservations.TryGetValue(issued.Reservation,out var registered) || !ReferenceEquals(registered,issued.ReservationWitness) || !ReservationLifecycles.TryGetValue(registered,out var bound) || !ReferenceEquals(bound,lifecycle))
                { CloseAuthenticatedFailure(witness,lifecycle); throw new InvalidOperationException("New Game reserved history has no actual registered pair."); }
                issued=issued.Previous;
                if (issued != null && issued.Kind == EventKind.ReserveAttempt) issued=issued.Previous;
            }
            var mint=issued == null ? null : issued.Previous;
            if (issued == null || issued.Kind != EventKind.RequestIssued || mint == null || mint.Kind != EventKind.MintIssued || !ReferenceEquals(mint.Result,witness.SourceResult) || !ReferenceEquals(mint.ResultWitness,witness.SourceWitness) || !ReferenceEquals(mint.Request,request) || !ReferenceEquals(mint.RequestWitness,witness) || mint.Resolution == null || !IssuedResolutions.TryGetValue(mint.Resolution,out var resolution) || resolution == null || !ReferenceEquals(resolution.Request,request) || !ValidateIssuedResolution(mint.Resolution))
            { CloseAuthenticatedFailure(witness,lifecycle); throw new InvalidOperationException("New Game request has no original issued event."); }
        }

        private static void CloseAuthenticatedFailure(ConfirmedRequestWitness witness, RequestLifecycle lifecycle)
        {
            // REQ-M5D7QC3-006: no allocation in exception containment. The
            // prebuilt closed event has no future reservation; ClosedHistory
            // preserves only events for actual pairs constructed along the way.
            if (lifecycle.Head.Kind == EventKind.Completed || lifecycle.Head.Kind == EventKind.Closed) return;
            lifecycle.ClosedHistory=lifecycle.Head;
            lifecycle.Head=lifecycle.FailureClosed;
            Volatile.Write(ref witness.State,3);
        }

        private static void CloseMintFailure(ResultWitness witness, MintLifecycle lifecycle)
        {
            if (lifecycle.Head.Kind == EventKind.Closed) return;
            lifecycle.ClosedHistory=lifecycle.Head;
            lifecycle.Head=lifecycle.FailureClosed;
            Volatile.Write(ref witness.RequestMintState,2);
        }

        private static ConfirmedRequestWitness GetConfirmedRequestWitness(ConfirmedProfileResetRequestV1 request, DesktopProfileLaunchAdapterV1 adapter, InputRouter router, object ownerToken, object epochToken, long interactionGeneration, long decisionGeneration)
        {
            RequireGenerations(interactionGeneration, decisionGeneration);
            if (request == null || adapter == null || router == null || ownerToken == null || epochToken == null || !IssuedConfirmedRequests.TryGetValue(request, out var witness) || witness == null || !ReferenceEquals(witness.Adapter, adapter) || !ReferenceEquals(witness.Router, router) || !ReferenceEquals(witness.Owner, ownerToken) || !ReferenceEquals(witness.Epoch, epochToken) || witness.InteractionGeneration != interactionGeneration || witness.DecisionGeneration != decisionGeneration) throw new InvalidOperationException("New Game confirmed request is not issued for this lower cohort.");
            return witness;
        }

        private static void RequireGenerations(long interactionGeneration, long decisionGeneration)
        {
            if (interactionGeneration <= 0 || decisionGeneration <= 0) throw new ArgumentOutOfRangeException(interactionGeneration <= 0 ? nameof(interactionGeneration) : nameof(decisionGeneration));
        }

        private static string GetAuthenticatedRoot(DesktopProfileLaunchAdapterV1 adapter, InputRouter router)
        {
            if (adapter == null || router == null) throw new ArgumentNullException(adapter == null ? nameof(adapter) : nameof(router));
            var root = adapter.GetAuthenticatedLaunchRootForNewGame(router);
            if (string.IsNullOrEmpty(root)) throw new InvalidOperationException("New Game launch root is absent.");
            return root;
        }

        private static bool SameIdentity(ProfileResetConfirmationCaptureV1 left, ProfileResetConfirmationCaptureV1 right)
        {
            if (left == null || right == null) throw new InvalidOperationException("New Game capture identity is absent.");
            left.Validate(); right.Validate();
            var a = left.Identity; var b = right.Identity;
            if (a == null || b == null || !string.Equals(a.Root, b.Root, StringComparison.OrdinalIgnoreCase) || a.Leaves == null || b.Leaves == null || a.Leaves.Length != 3 || b.Leaves.Length != 3) throw new InvalidOperationException("New Game capture identity is malformed.");
            for (var i = 0; i < 3; i++) if (a.Leaves[i] == null || b.Leaves[i] == null || !a.Leaves[i].Same(b.Leaves[i])) return false;
            return true;
        }

        private static NewGameObservationClassificationV1 Classify(ProfileResetConfirmationCaptureV1 capture)
        {
            if (capture == null || capture.Identity == null || capture.Identity.Leaves == null || capture.Identity.Leaves.Length != 3) throw new InvalidOperationException("C3 observation evidence is malformed.");
            var decoded = new ProfileCanonicalDecodeResultV1?[] { capture.Primary, capture.Previous, capture.Temp };
            var meaningful = false; var ambiguous = false;
            for (var i = 0; i < 3; i++)
            {
                var leaf = capture.Identity.Leaves[i]; if (leaf == null) throw new InvalidOperationException("C3 observation leaf is absent.");
                if (leaf.Presence == ProfileResetLeafPresenceV1.Missing) { if (decoded[i].HasValue) throw new InvalidOperationException("C3 missing leaf has decoded evidence."); continue; }
                if (leaf.Presence != ProfileResetLeafPresenceV1.Present || !leaf.Classification.HasValue || !decoded[i].HasValue) throw new InvalidOperationException("C3 leaf evidence is malformed.");
                var value = decoded[i].Value; value.Validate();
                if (leaf.Classification.Value == ProfileResetLeafClassificationV1.ValidCurrentInput)
                {
                    if (value.Classification != ProfileCanonicalDecodeClassification.ValidCurrentInput || leaf.Revision != value.Document.Snapshot.ProfileRevision) throw new InvalidOperationException("C3 current leaf evidence disagrees.");
                    meaningful |= !IsDefaultProduct(value.Document.Snapshot);
                }
                else ambiguous = true;
                if (i != 0) ambiguous = true;
            }
            return meaningful ? NewGameObservationClassificationV1.ConfirmationRequiredMeaningful : ambiguous ? NewGameObservationClassificationV1.ConfirmationRequiredAmbiguous : NewGameObservationClassificationV1.NoConfirmationRequired;
        }

        private static bool IsDefaultProduct(ProfileSnapshotV1 value)
        {
            var d = ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.Snapshot;
            return value.Settings.WindowMode == d.Settings.WindowMode && value.Settings.MasterVolumeQ1000 == d.Settings.MasterVolumeQ1000 && value.Settings.MusicVolumeQ1000 == d.Settings.MusicVolumeQ1000 && value.Settings.SfxVolumeQ1000 == d.Settings.SfxVolumeQ1000 && value.Settings.GamepadAimInvertX == d.Settings.GamepadAimInvertX && value.Settings.GamepadAimInvertY == d.Settings.GamepadAimInvertY && value.Input.InputActionsAssetId == d.Input.InputActionsAssetId && value.Input.BindingSchemaVersion == d.Input.BindingSchemaVersion && value.Input.BindingOverrides.CanonicalText == d.Input.BindingOverrides.CanonicalText && SameStrings(value.Tutorial.ConfirmedIds, d.Tutorial.ConfirmedIds) && value.Progression.LastOfferedSeed == d.Progression.LastOfferedSeed && value.Progression.CommittedChoice == d.Progression.CommittedChoice && value.Progression.ConsentState == d.Progression.ConsentState && value.Progression.GrantedSkill == d.Progression.GrantedSkill && SameBranches(value.Progression.CompletedBranches, d.Progression.CompletedBranches);
        }
        private static bool SameStrings(System.Collections.Generic.IReadOnlyList<string> a, System.Collections.Generic.IReadOnlyList<string> b) { if (a.Count != b.Count) return false; for (var i = 0; i < a.Count; i++) if (!string.Equals(a[i], b[i], StringComparison.Ordinal)) return false; return true; }
        private static bool SameBranches(System.Collections.Generic.IReadOnlyList<ProfileBranch> a, System.Collections.Generic.IReadOnlyList<ProfileBranch> b) { if (a.Count != b.Count) return false; for (var i = 0; i < a.Count; i++) if (a[i] != b[i]) return false; return true; }
    }
}
