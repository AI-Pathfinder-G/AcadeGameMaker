using System;

using System.Collections.Generic;

using AcadeGameMaker.Costumes;


namespace AcadeGameMaker.Presentation
{
    
    // REQ-COST-007..010,012. Immutable renderer/UI seams; no Unity or simulation calls.
    public enum CostumeBindingRejection {
         None = 0, UnknownCostume = 1, NotAccepted = 2, WrongActor = 3, MissingClip = 4, HashMismatch = 5, StaleCatalog = 6, StalePresentation = 7 
        }
    
    public readonly struct CostumePresentationBindingV1
    {
        private readonly string[] _clipIds;
        
        public CostumePresentationBindingV1(string actorId, string costumeId, string portraitSetId, string portraitHash, string atlasSetId, string atlasHash, string clipMapHash, long presentationRevision, long catalogRevision, IEnumerable<string> clipIds)
        {
             CostumeDefinitionV1.CheckId(actorId);
             CostumeDefinitionV1.CheckId(costumeId);
             CostumeDefinitionV1.CheckId(portraitSetId);
             CostumeDefinitionV1.CheckId(atlasSetId);
             CostumeDefinitionV1.CheckHash(portraitHash);
             CostumeDefinitionV1.CheckHash(atlasHash);
             CostumeDefinitionV1.CheckHash(clipMapHash);
             if (presentationRevision < 0 || catalogRevision < 0 || !costumeId.StartsWith(actorId + ".costume.", StringComparison.Ordinal)) throw new ArgumentException();
             ActorId = actorId;
             CostumeId = costumeId;
             PortraitSetId = portraitSetId;
             PortraitHash = portraitHash;
             GameplayAtlasSetId = atlasSetId;
             GameplayAtlasHash = atlasHash;
             ClipMapHash = clipMapHash;
             PresentationRevision = presentationRevision;
             CatalogRevision = catalogRevision;
             var list = new List<string>(clipIds ?? throw new ArgumentNullException());
             list.Sort(StringComparer.Ordinal);
             for (var i = 0; i < list.Count; i++) {
                 CostumeDefinitionV1.CheckId(list[i]);
                 if (i > 0 && list[i - 1] == list[i]) throw new ArgumentException();
                 
                }
             _clipIds = list.ToArray();
             
            }
        
        public string ActorId {
             get;
             
            }
        
        public string CostumeId {
             get;
             
            }
        
        public string PortraitSetId {
             get;
             
            }
        
        public string PortraitHash {
             get;
             
            }
        
        public string GameplayAtlasSetId {
             get;
             
            }
        
        public string GameplayAtlasHash {
             get;
             
            }
        
        public string ClipMapHash {
             get;
             
            }
        
        public long PresentationRevision {
             get;
             
            }
        
        public long CatalogRevision {
             get;
             
            }
        
        public IReadOnlyList<string> ClipIds { get { return _clipIds == null ? Array.AsReadOnly(new string[0]) : Array.AsReadOnly((string[])_clipIds.Clone()); } }
        
        public bool HasClip(string id) { return _clipIds != null && id != null && Array.BinarySearch(_clipIds, id, StringComparer.Ordinal) >= 0; }
        internal bool IsComplete { get { return _clipIds != null && _clipIds.Length > 0 && !String.IsNullOrEmpty(ActorId) && !String.IsNullOrEmpty(CostumeId) && !String.IsNullOrEmpty(PortraitSetId) && !String.IsNullOrEmpty(GameplayAtlasSetId) && !String.IsNullOrEmpty(PortraitHash) && !String.IsNullOrEmpty(GameplayAtlasHash) && !String.IsNullOrEmpty(ClipMapHash) && PresentationRevision >= 0 && CatalogRevision >= 0; } }
        
    
        }
    
    public readonly struct CompletedActorSnapshotV1
    {
         public CompletedActorSnapshotV1(long tick, string actor, string action, long actionAgeQ1000, string facing, int anchorX, int anchorY) {
             if (tick < 0 || actionAgeQ1000 < 0 || actionAgeQ1000 > 1000) throw new ArgumentOutOfRangeException();
             CostumeDefinitionV1.CheckId(actor);
             CostumeDefinitionV1.CheckId(action);
             CostumeDefinitionV1.CheckText(facing);
             Tick = tick;
             ActorId = actor;
             ActionId = action;
             ActionAgeQ1000 = actionAgeQ1000;
             Facing = facing;
             AnchorX = anchorX;
             AnchorY = anchorY;
             
            }
         public long Tick {
             get;
             
            }
         public string ActorId {
             get;
             
            }
         public string ActionId {
             get;
             
            }
         public long ActionAgeQ1000 {
             get;
             
            }
         public string Facing {
             get;
             
            }
         public int AnchorX {
             get;
             
            }
         public int AnchorY {
             get;
             
            }
         
        }
    
    public readonly struct CostumeMotionDescriptorV1
    {
         public CostumeMotionDescriptorV1(string action, int onset, int peak, int settle, int nativePixels, string garment) {
             CostumeDefinitionV1.CheckId(action);
             CostumeDefinitionV1.CheckText(garment);
             if (onset < 0 || peak < onset || settle < peak || nativePixels < 0 || nativePixels > 1) throw new ArgumentOutOfRangeException();
             ActionId = action;
             OnsetTick = onset;
             PeakTick = peak;
             SettleTick = settle;
             NativePixelMaximum = nativePixels;
             GarmentRegion = garment;
             
            }
         public string ActionId {
             get;
             
            }
         public int OnsetTick {
             get;
             
            }
         public int PeakTick {
             get;
             
            }
         public int SettleTick {
             get;
             
            }
         public int NativePixelMaximum {
             get;
             
            }
         public string GarmentRegion {
             get;
             
            }
         
        }
    
    public readonly struct CostumeViewModelV1 {
         public CostumeViewModelV1(string id, bool unlocked, bool current, CostumeDisposition disposition, string portrait) {
             CostumeId = id;
             IsUnlocked = unlocked;
             IsCurrent = current;
             Disposition = disposition;
             ConceptPortraitSetId = portrait ?? String.Empty;
             
            }
         public string CostumeId {
             get;
             
            }
         public bool IsUnlocked {
             get;
             
            }
         public bool IsCurrent {
             get;
             
            }
         public CostumeDisposition Disposition {
             get;
             
            }
         public string ConceptPortraitSetId {
             get;
             
            }
         public bool CanPrepareSelection => IsUnlocked && Disposition == CostumeDisposition.Accepted;
         
        }
    
    public static class CostumePresentationServiceV1
    {
        
        public static CostumeBindingRejection TryCreateBinding(CostumeCatalogV1 catalog, string actor, string costume, long expectedCatalog, long expectedPresentation, IEnumerable<string> clips, out CostumePresentationBindingV1 binding) {
             binding = default(CostumePresentationBindingV1);
             if (catalog == null) throw new ArgumentNullException();
             if (catalog.Revision != expectedCatalog) return CostumeBindingRejection.StaleCatalog;
             var d = catalog.Find(costume);
             if (d == null) return CostumeBindingRejection.UnknownCostume;
             if (d.ActorId != actor) return CostumeBindingRejection.WrongActor;
             if (d.Disposition != CostumeDisposition.Accepted) return CostumeBindingRejection.NotAccepted;
             if (d.PresentationRevision != expectedPresentation) return CostumeBindingRejection.StalePresentation;
             try {
                 binding = new CostumePresentationBindingV1(actor, costume, d.PortraitSetId, d.PortraitHash, d.GameplaySetId, d.AtlasHash, d.ClipMapHash, d.PresentationRevision, catalog.Revision, clips);
                 return CostumeBindingRejection.None;
                 
                }
             catch (ArgumentException) {
                 return CostumeBindingRejection.HashMismatch;
                 
                }
             
            }
        
        public static CostumeBindingRejection TrySwap(CostumeCatalogV1 catalog, CostumePresentationBindingV1 current, CostumePresentationBindingV1 proposed, CompletedActorSnapshotV1 snapshot, out CostumePresentationBindingV1 committed) {
             committed = current;
             if (catalog == null) return CostumeBindingRejection.StaleCatalog;
             if (!current.IsComplete || !proposed.IsComplete) return CostumeBindingRejection.HashMismatch;
             if (current.ActorId != snapshot.ActorId || current.ActorId != proposed.ActorId) return CostumeBindingRejection.WrongActor;
             if (current.CatalogRevision != catalog.Revision || proposed.CatalogRevision != catalog.Revision) return CostumeBindingRejection.StaleCatalog;
             if (!Matches(catalog.Find(current.CostumeId), current) || !Matches(catalog.Find(proposed.CostumeId), proposed)) return CostumeBindingRejection.HashMismatch;
             if (proposed.ActorId != snapshot.ActorId) return CostumeBindingRejection.WrongActor;
             if (!proposed.HasClip(snapshot.ActionId)) return CostumeBindingRejection.MissingClip;
             committed = proposed;
             return CostumeBindingRejection.None;
             
            }
        private static bool Matches(CostumeDefinitionV1 definition, CostumePresentationBindingV1 binding) { return definition != null && definition.ActorId == binding.ActorId && definition.Disposition == CostumeDisposition.Accepted && definition.PortraitSetId == binding.PortraitSetId && definition.GameplaySetId == binding.GameplayAtlasSetId && definition.PortraitHash == binding.PortraitHash && definition.AtlasHash == binding.GameplayAtlasHash && definition.ClipMapHash == binding.ClipMapHash && definition.PresentationRevision == binding.PresentationRevision; }
        
        public static IReadOnlyList<CostumeViewModelV1> BuildViews(CostumeCatalogV1 catalog, CostumeStateV1 state, string actor) {
             var list = new List<CostumeViewModelV1>();
             foreach (var d in catalog.Definitions) if (d.ActorId == actor) list.Add(new CostumeViewModelV1(d.CostumeId, state.IsUnlocked(d.CostumeId), state.Current(actor) == d.CostumeId, d.Disposition, d.PortraitSetId));
             return list.AsReadOnly();
             
            }
        
    
        }
    
}


