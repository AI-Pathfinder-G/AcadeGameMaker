using System;
using System.Collections.Generic;
using AcadeGameMaker.Costumes;
using AcadeGameMaker.Costumes.IO;
using AcadeGameMaker.Presentation;

namespace AcadeGameMaker.Presentation.Unity
{
    // REQ-CUA-004..007/009: all authority is injected; no scene/global/renderer discovery occurs here.
    public enum CostumeUnityPersistenceStatusV1 { NoAcceptedDefault = 0, Ready = 1, FailedBeforeCommit = 2, CommitOutcomeUncertain = 3, ReloadRequired = 4 }
    public enum CostumeUnityAdapterRejectionV1 { None = 0, Pending = 1, HighlightOnly = 2, Package = 3, Selection = 4, Save = 5, Snapshot = 6, ReloadRequired = 7 }
    public readonly struct CostumeUnityRowV1 { public CostumeUnityRowV1(CostumeViewModelV1 value) { CostumeId = value.CostumeId; IsUnlocked = value.IsUnlocked; IsCurrent = value.IsCurrent; Disposition = value.Disposition; CanPrepareSelection = value.CanPrepareSelection; } public string CostumeId { get; } public bool IsUnlocked { get; } public bool IsCurrent { get; } public CostumeDisposition Disposition { get; } public bool CanPrepareSelection { get; } }
    public sealed class CostumeUnityViewV1 { private readonly CostumeUnityRowV1[] _rows; internal CostumeUnityViewV1(long catalogRevision, long stateRevision, string actorId, string currentId, string transientPreviewId, CostumeAvailability availability, CostumeUnityPersistenceStatusV1 status, CostumeUnityRowV1[] rows) { CatalogRevision = catalogRevision; StateRevision = stateRevision; ActorId = actorId; CurrentId = currentId ?? String.Empty; TransientPreviewId = transientPreviewId ?? String.Empty; Availability = availability; PersistenceStatus = status; _rows = rows; } public long CatalogRevision { get; } public long StateRevision { get; } public string ActorId { get; } public string CurrentId { get; } public string TransientPreviewId { get; } public CostumeAvailability Availability { get; } public CostumeUnityPersistenceStatusV1 PersistenceStatus { get; } public IReadOnlyList<CostumeUnityRowV1> Rows { get { return Array.AsReadOnly((CostumeUnityRowV1[])_rows.Clone()); } } }
    public sealed class PublishedCostumePresentationV1 { internal PublishedCostumePresentationV1(CostumeStateV1 state, CostumePresentationBindingV1 binding, CostumeUnityMediaPackageV1 media, CompletedActorSnapshotV1 snapshot, CostumeUnityFrameV1 frame) { State = state; Binding = binding; Media = media; Snapshot = snapshot; Frame = frame; } public CostumeStateV1 State { get; } public CostumePresentationBindingV1 Binding { get; } public CostumeUnityMediaPackageV1 Media { get; } public CompletedActorSnapshotV1 Snapshot { get; } public CostumeUnityFrameV1 Frame { get; } }
    public interface ICostumeUnityProjectionPortV1 { void Project(PublishedCostumePresentationV1 published); }
    // REQ-CUA-005/009: the sole public completed-snapshot ingress is an instance port.
    public interface ICostumeUnityCompletedSnapshotPortV1 { CostumeUnityAdapterRejectionV1 Observe(CompletedActorSnapshotV1 snapshot); }

    public sealed class CostumeUnityPresentationAdapterV1 : ICostumeUnityCompletedSnapshotPortV1
    {
        private readonly CostumeCatalogV1 _catalog; private readonly CostumeFileAdapterV1 _cio; private readonly ICostumeUnityProjectionPortV1 _projection; private readonly string _actorId; private readonly CostumeStateV1 _initialState;
        private bool _hasObservedSnapshot; private CompletedActorSnapshotV1 _observedSnapshot; private string _highlight; private PublishedCostumePresentationV1 _published; private CostumeUnityPersistenceStatusV1 _status;
        // REQ-CUA-007: private audit observation of the exact concrete CIO result; it grants no caller authority.
        private bool _hasLastSaveResult; private CostumeFileSaveResultV1 _lastSaveResult;
        public CostumeUnityPresentationAdapterV1(CostumeCatalogV1 catalog, CostumeStateV1 state, string actorId, CostumeFileAdapterV1 cio, ICostumeUnityProjectionPortV1 projection)
        { _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); _initialState = state ?? throw new ArgumentNullException(nameof(state)); _cio = cio ?? throw new ArgumentNullException(nameof(cio)); _projection = projection ?? throw new ArgumentNullException(nameof(projection)); if (String.IsNullOrEmpty(actorId)) throw new ArgumentException(nameof(actorId)); _actorId = actorId; _status = _catalog.Availability(actorId) == CostumeAvailability.NoAcceptedDefault ? CostumeUnityPersistenceStatusV1.NoAcceptedDefault : CostumeUnityPersistenceStatusV1.Ready; }
        public PublishedCostumePresentationV1 Published { get { return _published; } } public CostumeUnityPersistenceStatusV1 PersistenceStatus { get { return _status; } } public ICostumeUnityCompletedSnapshotPortV1 CompletedSnapshots { get { return this; } }
        // REQ-CUA-001/002: terminal states and unknown rows are strict no-ops.
        public CostumeUnityAdapterRejectionV1 Highlight(string costumeId) { if (Terminal) return CostumeUnityAdapterRejectionV1.ReloadRequired; if (_catalog.Find(costumeId) == null) return CostumeUnityAdapterRejectionV1.Pending; _highlight = costumeId; return CostumeUnityAdapterRejectionV1.HighlightOnly; }
        public CostumeUnityViewV1 SnapshotView() { var state = AuthoritativeState; var source = CostumePresentationServiceV1.BuildViews(_catalog, state, _actorId); var rows = new CostumeUnityRowV1[source.Count]; for (var i = 0; i < source.Count; i++) rows[i] = new CostumeUnityRowV1(source[i]); return new CostumeUnityViewV1(_catalog.Revision, state.Revision, _actorId, state.Current(_actorId), _highlight, _catalog.Availability(_actorId), _status, rows); }

        // REQ-CUA-003..007: snapshot-free selection; every value is prepared before concrete Save.
        public CostumeUnityAdapterRejectionV1 TrySelect(CostumeUnityMediaPackageV1 media)
        {
            if (Terminal) return CostumeUnityAdapterRejectionV1.ReloadRequired;
            if (!_hasObservedSnapshot) return CostumeUnityAdapterRejectionV1.Snapshot;
            var snapshot = _observedSnapshot;
            if (media == null || media.FindClip(snapshot.ActionId, snapshot.Facing) == null) return CostumeUnityAdapterRejectionV1.Snapshot;
            var currentState = AuthoritativeState;
            var prep = CostumeSelectionServiceV1.PrepareSelection(_catalog, currentState, _actorId, media.CostumeId, currentState.Revision, _catalog.Revision);
            if (!prep.IsPrepared) return CostumeUnityAdapterRejectionV1.Selection;
            CostumeStateV1 staged; try { staged = CostumeSelectionServiceV1.Commit(prep, _catalog, currentState); } catch (InvalidOperationException) { return CostumeUnityAdapterRejectionV1.Selection; }
            CostumePresentationBindingV1 proposed;
            if (CostumePresentationServiceV1.TryCreateBinding(_catalog, _actorId, media.CostumeId, _catalog.Revision, media.PresentationRevision, ActionIds(media), out proposed) != CostumeBindingRejection.None || media.Validate(_catalog.Find(media.CostumeId), proposed) != CostumeUnityPackageRejectionV1.None) return CostumeUnityAdapterRejectionV1.Package;
            var clip = media.FindClip(snapshot.ActionId, snapshot.Facing); CostumePresentationBindingV1 committed = default(CostumePresentationBindingV1);
            if (_published == null)
            {
                if (!String.IsNullOrEmpty(_initialState.Current(_actorId)) || CostumePresentationServiceV1.TrySwap(_catalog, proposed, proposed, snapshot, out committed) != CostumeBindingRejection.None || !BindingEqual(committed, proposed)) return CostumeUnityAdapterRejectionV1.Selection;
            }
            else
            {
                if (!CurrentTupleValid()) return CloseReloadRequired();
                if (CostumePresentationServiceV1.TrySwap(_catalog, _published.Binding, proposed, snapshot, out committed) != CostumeBindingRejection.None || !BindingEqual(committed, proposed)) return CostumeUnityAdapterRejectionV1.Selection;
            }
            var candidate = new PublishedCostumePresentationV1(staged, committed, media, snapshot, clip.FrameFor(snapshot.ActionAgeQ1000));
            var result = _cio.Save(staged);
            _lastSaveResult = result;
            _hasLastSaveResult = true;
            if (result.Outcome == CostumeFileSaveOutcomeV1.FailedBeforeCommit) { _status = CostumeUnityPersistenceStatusV1.FailedBeforeCommit; return CostumeUnityAdapterRejectionV1.Save; }
            if (result.Outcome == CostumeFileSaveOutcomeV1.CommitOutcomeUncertain) { _status = CostumeUnityPersistenceStatusV1.CommitOutcomeUncertain; return CostumeUnityAdapterRejectionV1.ReloadRequired; }
            if ((result.Outcome != CostumeFileSaveOutcomeV1.CommittedFirst && result.Outcome != CostumeFileSaveOutcomeV1.CommittedReplacement) || result.ExpectedRevision != staged.Revision) return CloseReloadRequired();
            _published = candidate; // The single authoritative publication assignment after exact durable success.
            _highlight = String.Empty;
            try { _projection.Project(candidate); } catch { _status = CostumeUnityPersistenceStatusV1.ReloadRequired; return CostumeUnityAdapterRejectionV1.ReloadRequired; }
            _status = CostumeUnityPersistenceStatusV1.Ready; return CostumeUnityAdapterRejectionV1.None;
        }

        CostumeUnityAdapterRejectionV1 ICostumeUnityCompletedSnapshotPortV1.Observe(CompletedActorSnapshotV1 snapshot)
        {
            if (Terminal) return CostumeUnityAdapterRejectionV1.ReloadRequired;
            if (snapshot.ActorId != _actorId || (snapshot.Facing != "left" && snapshot.Facing != "right")) return CostumeUnityAdapterRejectionV1.Snapshot;
            if (!_hasObservedSnapshot) { _observedSnapshot = snapshot; _hasObservedSnapshot = true; return CostumeUnityAdapterRejectionV1.None; }
            if (snapshot.Tick < _observedSnapshot.Tick) return CostumeUnityAdapterRejectionV1.Snapshot;
            if (snapshot.Tick == _observedSnapshot.Tick) return SnapshotEqual(snapshot, _observedSnapshot) ? CostumeUnityAdapterRejectionV1.None : CostumeUnityAdapterRejectionV1.Snapshot;
            if (_published == null) { _observedSnapshot = snapshot; return CostumeUnityAdapterRejectionV1.None; }
            if (!CurrentTupleValid()) return CloseReloadRequired();
            var clip = _published.Media.FindClip(snapshot.ActionId, snapshot.Facing); if (clip == null) return CostumeUnityAdapterRejectionV1.Snapshot;
            var next = new PublishedCostumePresentationV1(_published.State, _published.Binding, _published.Media, snapshot, clip.FrameFor(snapshot.ActionAgeQ1000));
            _observedSnapshot = snapshot; _published = next;
            try { _projection.Project(next); } catch { _status = CostumeUnityPersistenceStatusV1.ReloadRequired; return CostumeUnityAdapterRejectionV1.ReloadRequired; }
            return CostumeUnityAdapterRejectionV1.None;
        }

        private bool Terminal { get { return _status == CostumeUnityPersistenceStatusV1.CommitOutcomeUncertain || _status == CostumeUnityPersistenceStatusV1.ReloadRequired; } }
        private CostumeStateV1 AuthoritativeState { get { return _published == null ? _initialState : _published.State; } }
        private CostumeUnityAdapterRejectionV1 CloseReloadRequired() { _status = CostumeUnityPersistenceStatusV1.ReloadRequired; return CostumeUnityAdapterRejectionV1.ReloadRequired; }
        private bool CurrentTupleValid()
        {
            if (_published == null || !_hasObservedSnapshot || _published.State == null || _published.Media == null || _published.Binding.ActorId != _actorId || _published.Media.ActorId != _actorId || _published.Snapshot.ActorId != _actorId || !SnapshotEqual(_published.Snapshot, _observedSnapshot) || _published.State.Current(_actorId) != _published.Binding.CostumeId) return false;
            var clip = _published.Media.FindClip(_published.Snapshot.ActionId, _published.Snapshot.Facing);
            if (clip == null || !FrameEqual(_published.Frame, clip.FrameFor(_published.Snapshot.ActionAgeQ1000))) return false;
            return _published.Media.Validate(_catalog.Find(_published.Binding.CostumeId), _published.Binding) == CostumeUnityPackageRejectionV1.None;
        }
        private static bool SnapshotEqual(CompletedActorSnapshotV1 a, CompletedActorSnapshotV1 b) { return a.Tick == b.Tick && a.ActorId == b.ActorId && a.ActionId == b.ActionId && a.ActionAgeQ1000 == b.ActionAgeQ1000 && a.Facing == b.Facing && a.AnchorX == b.AnchorX && a.AnchorY == b.AnchorY; }
        private static bool FrameEqual(CostumeUnityFrameV1 a, CostumeUnityFrameV1 b) { return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height && a.PivotXQ1000 == b.PivotXQ1000 && a.PivotYQ1000 == b.PivotYQ1000 && a.BaselineY == b.BaselineY && a.DurationTicks == b.DurationTicks; }
        private static IEnumerable<string> ActionIds(CostumeUnityMediaPackageV1 media) { var actions = new SortedSet<string>(StringComparer.Ordinal); foreach (var clip in media.Clips) actions.Add(clip.ActionId); return actions; }
        private static bool BindingEqual(CostumePresentationBindingV1 a, CostumePresentationBindingV1 b) { if (a.ActorId != b.ActorId || a.CostumeId != b.CostumeId || a.PortraitSetId != b.PortraitSetId || a.PortraitHash != b.PortraitHash || a.GameplayAtlasSetId != b.GameplayAtlasSetId || a.GameplayAtlasHash != b.GameplayAtlasHash || a.ClipMapHash != b.ClipMapHash || a.PresentationRevision != b.PresentationRevision || a.CatalogRevision != b.CatalogRevision || a.ClipIds.Count != b.ClipIds.Count) return false; for (var i = 0; i < a.ClipIds.Count; i++) if (a.ClipIds[i] != b.ClipIds[i]) return false; return true; }
    }
}
