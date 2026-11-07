using System;

using System.Collections.Generic;

using System.Globalization;

using System.Security.Cryptography;

using System.Text;


namespace AcadeGameMaker.Costumes
{
    
    // REQ-COST-001..006,012,014. Engine-free values; this lane never opens a file.
    public enum CostumeDisposition {
         Accepted = 1, ProductionPending = 2, Rejected = 3 
        }
    
    public enum CostumeAvailability {
         Available = 1, NoAcceptedDefault = 2 
        }
    
    public enum CostumeSelectionRejection {
         None = 0, UnknownActor = 1, UnknownCostume = 2, NotAccepted = 3, Locked = 4, StaleState = 5, StaleCatalog = 6, ForeignCandidate = 7, ConsumedCandidate = 8 
        }
    
    public enum CostumeRecoveryReason {
         None = 0, DefaultBootstrap = 1, InvalidCurrentFallback = 2, NoAcceptedDefault = 3 
        }
    
    public enum CostumeLoadRole {
         Primary = 1, Previous = 2, Temp = 3 
        }
    
    public enum CostumeIssuerKind {
         AuthoredDefault = 1, Progression = 2, Entitlement = 3 
        }
    

    public sealed class CostumeDefinitionV1
    {
        
        public CostumeDefinitionV1(string actorId, string costumeId, string sourceOption, string sourceHash, string referenceHair, CostumeDisposition disposition, bool defaultGrant, string portraitSetId, string gameplaySetId, string portraitHash, string atlasHash, string clipMapHash, long presentationRevision)
        {
            
            CheckId(actorId);
             CheckId(costumeId);
             CheckText(sourceOption);
             CheckHash(sourceHash);
             CheckText(referenceHair);
            
            if (!costumeId.StartsWith(actorId + ".costume.", StringComparison.Ordinal) || presentationRevision < 0 || !Known(disposition)) throw new ArgumentException("Malformed costume definition.");
            
            if (disposition == CostumeDisposition.Accepted)
            {
                
                CheckId(portraitSetId);
                 CheckId(gameplaySetId);
                 CheckHash(portraitHash);
                 CheckHash(atlasHash);
                 CheckHash(clipMapHash);
                
            
                }
            
            ActorId = actorId;
             CostumeId = costumeId;
             SourceOption = sourceOption;
             SourceHash = sourceHash;
             ReferenceHair = referenceHair;
             Disposition = disposition;
             DefaultGrant = defaultGrant;
            
            PortraitSetId = portraitSetId ?? string.Empty;
             GameplaySetId = gameplaySetId ?? string.Empty;
             PortraitHash = portraitHash ?? string.Empty;
             AtlasHash = atlasHash ?? string.Empty;
             ClipMapHash = clipMapHash ?? string.Empty;
             PresentationRevision = presentationRevision;
            
        
            }
        
        public string ActorId {
             get;
             
            }
        
        public string CostumeId {
             get;
             
            }
        
        public string SourceOption {
             get;
             
            }
        
        public string SourceHash {
             get;
             
            }
        
        public string ReferenceHair {
             get;
             
            }
        
        public CostumeDisposition Disposition {
             get;
             
            }
        
        public bool DefaultGrant {
             get;
             
            }
        
        public string PortraitSetId {
             get;
             
            }
        
        public string GameplaySetId {
             get;
             
            }
        
        public string PortraitHash {
             get;
             
            }
        
        public string AtlasHash {
             get;
             
            }
        
        public string ClipMapHash {
             get;
             
            }
        
        public long PresentationRevision {
             get;
             
            }
        
        internal static void CheckId(string value) {
             if (String.IsNullOrEmpty(value) || value != value.Normalize(NormalizationForm.FormC)) throw new ArgumentException("Invalid canonical ID.");
             for (var i = 0; i < value.Length; i++) {
                 var c = value[i];
                 if (!(c >= 'a' && c <= 'z' || c >= '0' && c <= '9' || c == '.' || c == '_' || c == '-') || (i == 0 && !(c >= 'a' && c <= 'z' || c >= '0' && c <= '9'))) throw new ArgumentException("Invalid canonical ID.");
                 
                }
             
            }
        
        internal static void CheckText(string value) {
             if (String.IsNullOrEmpty(value)) throw new ArgumentException("Required value missing.");
             
            }
        
        internal static void CheckHash(string value) {
             if (String.IsNullOrEmpty(value) || value.Length != 64) throw new ArgumentException("Expected SHA-256 hex hash.");
             for (var i = 0; i < value.Length; i++) if (!((value[i] >= '0' && value[i] <= '9') || (value[i] >= 'a' && value[i] <= 'f'))) throw new ArgumentException("Expected SHA-256 hex hash.");
             
            }
        
        internal static bool Known(CostumeDisposition value) => value == CostumeDisposition.Accepted || value == CostumeDisposition.ProductionPending || value == CostumeDisposition.Rejected;
        
    
        }
    

    public sealed class CostumeCatalogV1
    {
        
        private readonly CostumeDefinitionV1[] _definitions;
        
        public CostumeCatalogV1(long revision, IEnumerable<CostumeDefinitionV1> definitions)
        {
            
            if (revision < 0 || definitions == null) throw new ArgumentException();
             Revision = revision;
             var list = new List<CostumeDefinitionV1>(definitions);
             if (list.Count == 0) throw new ArgumentException();
            
            for (var i = 0; i < list.Count; i++) {
                 if (list[i] == null || (i > 0 && StringComparer.Ordinal.Compare(list[i - 1].CostumeId, list[i].CostumeId) >= 0)) throw new ArgumentException("Catalog IDs must be strictly ordinal-sorted and duplicate-free.");
                 
                }
            
            _definitions = list.ToArray();
             ValidateDefaults();
            
        
            }
        
        public long Revision {
             get;
             
            }
        
        public IReadOnlyList<CostumeDefinitionV1> Definitions => Array.AsReadOnly(_definitions);
        
        public CostumeDefinitionV1 Find(string costumeId) {
             if (costumeId == null) return null;
             for (var i = 0; i < _definitions.Length; i++) if (StringComparer.Ordinal.Equals(_definitions[i].CostumeId, costumeId)) return _definitions[i];
             return null;
             
            }
        
        public CostumeDefinitionV1 AcceptedDefault(string actorId) {
             CostumeDefinitionV1 found = null;
             for (var i = 0; i < _definitions.Length; i++) {
                 var d = _definitions[i];
                 if (d.ActorId == actorId && d.Disposition == CostumeDisposition.Accepted && d.DefaultGrant) found = d;
                 
                }
             return found;
             
            }
        
        public CostumeAvailability Availability(string actorId) => AcceptedDefault(actorId) == null ? CostumeAvailability.NoAcceptedDefault : CostumeAvailability.Available;
        
        private void ValidateDefaults() {
             var seen = new Dictionary<string, int>(StringComparer.Ordinal);
             foreach (var d in _definitions) if (d.Disposition == CostumeDisposition.Accepted && d.DefaultGrant) {
                 int count;
                 seen.TryGetValue(d.ActorId, out count);
                 seen[d.ActorId] = count + 1;
                 if (count > 0) throw new ArgumentException("Multiple accepted defaults.");
                 
                }
             
            }
        
        public static CostumeCatalogV1 SeryeongInventoryPending() {
             var rows = new List<CostumeDefinitionV1>(SeryeongRows());
             rows.Sort((left, right) => StringComparer.Ordinal.Compare(left.CostumeId, right.CostumeId));
             return new CostumeCatalogV1(1, rows);
             
            }
        
        private static IEnumerable<CostumeDefinitionV1> SeryeongRows()
        {
            
            const string story = "a6b985dbfc907bf6d24d10b97301497622fed09f421b6ba9aad300b58161eef1", a = "951310f8cb41f126e192ea6de74ab8b297a5f95d18be158d8d98503775da5cc1", c = "62324f25e8463cc38635bb94ccc9fb942e60f599a7a64067606a278b85c673bf", d = "3b024bf0708630f478679cd1023ccff9746a8a5cf27ab3863a9c0f5fb1dbeb3e", v = "9eb4ab28ecd2ab99e1f68a7a121e55dfbd1fe3e73a981fb58b2d27f01a281681", t = "5e381b1b971c885aad27be424767a9f0d0ea000a3a98f35764faa651d40a21e2", l = "daa5d18012b8f090a3a6cfefa40ecb7054e707158fee51c9aadf3d6c16a7d5e8", b = "a6e943e48da3e8d6e4d76ec0a61c40001e6cc09d3f0099979dff3474beb64f3d", x = "7dd1b89abcff3aafb55099c44cf27b2c25b0d529a3361a214d82ed4f9541ceea", s = "f7fea2e45ce25edeced2d2ba1b25499d225be6c6b317f8a6bdc5b66b0c5987b2", r = "dd97686dfa0a0ccd6a12cbebd13ccdb9d08479a5e27e913ceecae683056b73f6";
            
            foreach (var row in new[] {
                 "story|story|" + story, "a1|a1|" + a, "a2|a2|" + a, "a3|a3|" + a, "c1|c1|" + c, "c2|c2|" + c, "c3|c3|" + c, "d1|d1|" + d, "d2|d2|" + d, "d3|d3|" + d, "v1|v1|" + v, "v2|v2|" + v, "v3|v3|" + v, "v4|v4|" + v, "t1|t1|" + t, "t2|t2|" + t, "t3|t3|" + t, "t4|t4|" + t, "l1|l1|" + l, "l2|l2|" + l, "l3|l3|" + l, "l4|l4|" + l, "b1|b1|" + b, "b2|b2|" + b, "b3|b3|" + b, "b4|b4|" + b, "x1|x1|" + x, "x2|x2|" + x, "x3|x3|" + x, "x4|x4|" + x, "s1|s1|" + s, "s2|s2|" + s, "s3|s3|" + s, "r1|r1|" + r, "r2|r2|" + r, "r3|r3|" + r 
                }
            ) {
                 var p = row.Split('|');
                 yield return new CostumeDefinitionV1("seryeong", "seryeong.costume." + p[0] + ".v1", p[1], p[2], "reference-hair-retained", CostumeDisposition.ProductionPending, false, null, null, null, null, null, 0);
                 
                }
            
        
            }
        
    
        }
    

    public sealed class CostumeStateV1
    {
        
        private readonly string[] _unlocked;
         private readonly KeyValuePair<string, string>[] _current;
         private readonly KeyValuePair<string, long>[] _issuerRevisions;
         private readonly KeyValuePair<string, string>[] _appliedGrants;
        
        public CostumeStateV1(long revision, IEnumerable<string> unlocked, IEnumerable<KeyValuePair<string, string>> current)
        {
             if (revision < 0 || unlocked == null || current == null) throw new ArgumentException();
             Revision = revision;
             _unlocked = SortedDistinct(unlocked);
             _current = SortedPairs(current);
             _issuerRevisions = new KeyValuePair<string, long>[0];
             _appliedGrants = new KeyValuePair<string, string>[0];
             
            }
        
        internal CostumeStateV1(long revision, IEnumerable<string> unlocked, IEnumerable<KeyValuePair<string, string>> current, IEnumerable<KeyValuePair<string, long>> issuerRevisions, IEnumerable<KeyValuePair<string, string>> appliedGrants)
        {
             if (revision < 0 || unlocked == null || current == null || issuerRevisions == null || appliedGrants == null) throw new ArgumentException();
             Revision = revision;
             _unlocked = SortedDistinct(unlocked);
             _current = SortedPairs(current);
             var lanes = new List<KeyValuePair<string, long>>(issuerRevisions);
             lanes.Sort((a, b) => StringComparer.Ordinal.Compare(a.Key, b.Key));
             for (var i = 0; i < lanes.Count; i++) {
                 CostumeDefinitionV1.CheckId(lanes[i].Key);
                 if (lanes[i].Value < 0 || (i > 0 && lanes[i - 1].Key == lanes[i].Key)) throw new ArgumentException();
                 
                }
             _issuerRevisions = lanes.ToArray();
             _appliedGrants = SortedKeyValues(appliedGrants);
             
            }
        
        public long Revision {
             get;
             
            }
        
        public IReadOnlyList<string> UnlockedCostumeIds => Array.AsReadOnly(_unlocked);
         public IReadOnlyList<KeyValuePair<string, string>> CurrentByActor => Array.AsReadOnly(_current);
         internal IReadOnlyList<KeyValuePair<string, long>> IssuerRevisions => Array.AsReadOnly(_issuerRevisions);
         internal IReadOnlyList<KeyValuePair<string, string>> AppliedGrants => Array.AsReadOnly(_appliedGrants);
        
        public bool IsUnlocked(string id) {
             return Array.BinarySearch(_unlocked, id, StringComparer.Ordinal) >= 0;
             
            }
        
        public string Current(string actor) {
             foreach (var p in _current) if (p.Key == actor) return p.Value;
             return null;
             
            }
        
        internal bool TryGrant(string grantId, out string fingerprint) {
             foreach (var p in _appliedGrants) if (p.Key == grantId) {
                 fingerprint = p.Value;
                 return true;
                 
                }
             fingerprint = null;
             return false;
             
            }
        
        internal long IssuerRevision(string lane) {
             foreach (var p in _issuerRevisions) if (p.Key == lane) return p.Value;
             return -1;
             
            }
        
        internal CostumeStateV1 WithGrant(string id, string lane, long sourceRevision, string grantId, string fingerprint) {
             var u = new List<string>(_unlocked);
             if (!IsUnlocked(id)) u.Add(id);
             var l = new List<KeyValuePair<string, long>>(_issuerRevisions);
             for (var i = 0; i < l.Count; i++) if (l[i].Key == lane) {
                 l[i] = new KeyValuePair<string, long>(lane, sourceRevision);
                 goto done;
                 
                }
             l.Add(new KeyValuePair<string, long>(lane, sourceRevision));
             done: var a = new List<KeyValuePair<string, string>>(_appliedGrants) {
                 new KeyValuePair<string, string>(grantId, fingerprint) 
                }
            ;
             return new CostumeStateV1(checked(Revision + 1), u, _current, l, a);
             
            }
        
        internal CostumeStateV1 WithCurrent(string actor, string id) {
             var x = new List<KeyValuePair<string, string>>(_current);
             for (var i = 0; i < x.Count; i++) if (x[i].Key == actor) {
                 x[i] = new KeyValuePair<string, string>(actor, id);
                 return new CostumeStateV1(checked(Revision + 1), _unlocked, x, _issuerRevisions, _appliedGrants);
                 
                }
             x.Add(new KeyValuePair<string, string>(actor, id));
             return new CostumeStateV1(checked(Revision + 1), _unlocked, x, _issuerRevisions, _appliedGrants);
             
            }

        internal CostumeStateV1 WithoutCurrent(string actor) {
             var x = new List<KeyValuePair<string, string>>(_current);
             x.RemoveAll(pair => StringComparer.Ordinal.Equals(pair.Key, actor));
             return new CostumeStateV1(Revision, _unlocked, x, _issuerRevisions, _appliedGrants);
            }
        
        private static string[] SortedDistinct(IEnumerable<string> values) {
             var x = new List<string>(values);
             x.Sort(StringComparer.Ordinal);
             for (var i = 0; i < x.Count; i++) {
                 CostumeDefinitionV1.CheckId(x[i]);
                 if (i > 0 && x[i - 1] == x[i]) throw new ArgumentException();
                 
                }
             return x.ToArray();
             
            }
        
        private static KeyValuePair<string, string>[] SortedPairs(IEnumerable<KeyValuePair<string, string>> values) {
             var x = new List<KeyValuePair<string, string>>(values);
             x.Sort((a, b) => StringComparer.Ordinal.Compare(a.Key, b.Key));
             for (var i = 0; i < x.Count; i++) {
                 CostumeDefinitionV1.CheckId(x[i].Key);
                 CostumeDefinitionV1.CheckId(x[i].Value);
                 if (i > 0 && x[i - 1].Key == x[i].Key) throw new ArgumentException();
                 
                }
             return x.ToArray();
             
            }
        
        private static KeyValuePair<string, string>[] SortedKeyValues(IEnumerable<KeyValuePair<string, string>> values) {
             var x = new List<KeyValuePair<string, string>>(values);
             x.Sort((a, b) => StringComparer.Ordinal.Compare(a.Key, b.Key));
             for (var i = 0; i < x.Count; i++) {
                 CostumeDefinitionV1.CheckId(x[i].Key);
                 CostumeDefinitionV1.CheckText(x[i].Value);
                 if (i > 0 && x[i - 1].Key == x[i].Key) throw new ArgumentException();
                 
                }
             return x.ToArray();
             
            }
        
    
        }
    
    internal sealed class SelectionToken {
         internal bool Consumed;
         
        }
    
    public readonly struct CostumeSelectionPreparationV1 {
         internal CostumeSelectionPreparationV1(CostumeSelectionRejection rejection, CostumeCatalogV1 catalog, CostumeStateV1 state, string actor, string costume, long catalogRevision, SelectionToken owner) {
             Rejection = rejection;
             Catalog = catalog;
             State = state;
             ActorId = actor;
             CostumeId = costume;
             CatalogRevision = catalogRevision;
             Owner = owner;
             
            }
         public CostumeSelectionRejection Rejection {
             get;
             
            }
         internal CostumeStateV1 State {
             get;
             
            }
         internal CostumeCatalogV1 Catalog { get; }
         internal string ActorId {
             get;
             
            }
         internal string CostumeId {
             get;
             
            }
         internal long CatalogRevision {
             get;
             
            }
         internal SelectionToken Owner {
             get;
             
            }
         public bool IsPrepared => Rejection == CostumeSelectionRejection.None;
         
        }
    
    public static class CostumeSelectionServiceV1
    {
        
        public static CostumeSelectionPreparationV1 PrepareSelection(CostumeCatalogV1 c, CostumeStateV1 s, string actor, string costume, long expectedState, long expectedCatalog) {
             if (c == null || s == null) throw new ArgumentNullException();
             var reject = s.Revision != expectedState ? CostumeSelectionRejection.StaleState : c.Revision != expectedCatalog ? CostumeSelectionRejection.StaleCatalog : Classify(c, s, actor, costume);
             return new CostumeSelectionPreparationV1(reject, c, s, actor, costume, c.Revision, new SelectionToken());
             
            }
        
        public static CostumeStateV1 Commit(CostumeSelectionPreparationV1 p, CostumeCatalogV1 c, CostumeStateV1 s) {
             if (!p.IsPrepared || p.Owner == null || p.Owner.Consumed || c == null || s == null || p.Catalog != c || p.State != s || p.CatalogRevision != c.Revision || p.State.Revision != s.Revision) throw new InvalidOperationException("Selection candidate rejected.");
             p.Owner.Consumed = true;
             return s.WithCurrent(p.ActorId, p.CostumeId);
             
            }
        
        private static CostumeSelectionRejection Classify(CostumeCatalogV1 c, CostumeStateV1 s, string actor, string id) {
             var d = c.Find(id);
             if (d == null) return CostumeSelectionRejection.UnknownCostume;
             if (d.ActorId != actor) return CostumeSelectionRejection.UnknownActor;
             if (d.Disposition != CostumeDisposition.Accepted) return CostumeSelectionRejection.NotAccepted;
             if (!s.IsUnlocked(id)) return CostumeSelectionRejection.Locked;
             return CostumeSelectionRejection.None;
             
            }
        
    
        }
    
    public enum CostumeGrantResult {
         Applied = 1, IdempotentReplay = 2, UnknownIssuer = 3, ForeignCapability = 4, UnknownCostume = 5, NotAccepted = 6, StaleOrDecreasingRevision = 7, ConflictingReplay = 8, Overflow = 9 
        }
    
    internal sealed class CostumeIssuerCapability {
         internal readonly string Lane;
         internal CostumeIssuerCapability(string lane) {
             Lane = lane;
             
            }
         
        }
    
    internal sealed class CostumeTrustedIssuerV1
    {
         internal readonly CostumeIssuerKind Kind;
         internal readonly CostumeIssuerCapability Capability;
         internal CostumeTrustedIssuerV1(CostumeIssuerKind kind, string lane) {
             Kind = kind;
             Capability = new CostumeIssuerCapability(lane);
             
            }
         internal CostumeUnlockGrantV1 Grant(string grantId, string costumeId, long sourceRevision) => new CostumeUnlockGrantV1(grantId, costumeId, Kind, sourceRevision, Capability);
         
        }
    
    internal readonly struct CostumeUnlockGrantV1
    {
         internal CostumeUnlockGrantV1(string grantId, string costumeId, CostumeIssuerKind kind, long revision, CostumeIssuerCapability capability) {
             CostumeDefinitionV1.CheckId(grantId);
             CostumeDefinitionV1.CheckId(costumeId);
             if (revision < 0 || capability == null) throw new ArgumentException();
             GrantId = grantId;
             CostumeId = costumeId;
             Kind = kind;
             SourceRevision = revision;
             Capability = capability;
             
            }
         internal string GrantId {
             get;
             
            }
         internal string CostumeId {
             get;
             
            }
         internal CostumeIssuerKind Kind {
             get;
             
            }
         internal long SourceRevision {
             get;
             
            }
         internal CostumeIssuerCapability Capability {
             get;
             
            }
         internal string Fingerprint => ((int)Kind).ToString(CultureInfo.InvariantCulture) + "|" + CostumeId + "|" + SourceRevision.ToString(CultureInfo.InvariantCulture) + "|" + Capability.Lane;
         
        }
    
    internal static class CostumeTrustedIssuersV1
    {
        private static readonly List<CostumeIssuerCapability> RegisteredCapabilities = new List<CostumeIssuerCapability>();
        // Closed production allowlist: callers cannot supply lane names or capabilities.
        internal static CostumeTrustedIssuerV1 CreateProduction(CostumeIssuerKind kind) {
             if (kind == CostumeIssuerKind.AuthoredDefault) return Register(kind, "costume.authored-default.v1");
             if (kind == CostumeIssuerKind.Progression) return Register(kind, "costume.progression.v1");
             if (kind == CostumeIssuerKind.Entitlement) return Register(kind, "costume.entitlement.v1");
             throw new ArgumentOutOfRangeException(nameof(kind));
            }
        private static CostumeTrustedIssuerV1 Register(CostumeIssuerKind kind, string lane) { var issuer = new CostumeTrustedIssuerV1(kind, lane); RegisteredCapabilities.Add(issuer.Capability); return issuer; }
        internal static bool IsRegistered(CostumeIssuerCapability capability) { return capability != null && RegisteredCapabilities.Contains(capability); }
        internal static CostumeTrustedIssuerV1 CreateForTests(CostumeIssuerKind kind, string lane) {
             if (kind != CostumeIssuerKind.Progression && kind != CostumeIssuerKind.Entitlement && kind != CostumeIssuerKind.AuthoredDefault) throw new ArgumentOutOfRangeException();
             CostumeDefinitionV1.CheckId(lane);
             return new CostumeTrustedIssuerV1(kind, lane);
             
            }
         
        }
    
    internal static class CostumeUnlockServiceV1
    {
        
        internal static CostumeGrantResult Apply(CostumeCatalogV1 catalog, CostumeStateV1 state, CostumeUnlockGrantV1 grant, out CostumeStateV1 result) {
             result = state;
             if (catalog == null || state == null) return CostumeGrantResult.UnknownIssuer;
             var known = CostumeTrustedIssuersV1.IsRegistered(grant.Capability) && (grant.Kind == CostumeIssuerKind.Progression || grant.Kind == CostumeIssuerKind.Entitlement || grant.Kind == CostumeIssuerKind.AuthoredDefault);
             if (!known) return CostumeGrantResult.UnknownIssuer;
             string old;
             if (state.TryGrant(grant.GrantId, out old)) return old == grant.Fingerprint ? CostumeGrantResult.IdempotentReplay : CostumeGrantResult.ConflictingReplay;
             var d = catalog.Find(grant.CostumeId);
             if (d == null) return CostumeGrantResult.UnknownCostume;
             if (d.Disposition != CostumeDisposition.Accepted) return CostumeGrantResult.NotAccepted;
             var prior = state.IssuerRevision(grant.Capability.Lane);
             if (grant.SourceRevision <= prior) return CostumeGrantResult.StaleOrDecreasingRevision;
             if (state.Revision == long.MaxValue) return CostumeGrantResult.Overflow;
             try {
                 result = state.WithGrant(grant.CostumeId, grant.Capability.Lane, grant.SourceRevision, grant.GrantId, grant.Fingerprint);
                 return CostumeGrantResult.Applied;
                 
                }
             catch (OverflowException) {
                 return CostumeGrantResult.Overflow;
                 
                }
             
            }
        
    
        }
    
    public readonly struct CostumeRecoveryInputV1 {
         private readonly byte[] _primary;
         private readonly byte[] _previous;
         private readonly byte[] _temp;
         public CostumeRecoveryInputV1(byte[] primary, byte[] previous, byte[] temp) {
             _primary = Copy(primary);
             _previous = Copy(previous);
             _temp = Copy(temp);
             
            }
         public byte[] Primary { get { return Copy(_primary); } }
         public byte[] Previous { get { return Copy(_previous); } }
         public byte[] Temp { get { return Copy(_temp); } }
         private static byte[] Copy(byte[] value) { return value == null ? null : (byte[])value.Clone(); }
         
        }
    
    public readonly struct CostumeRecoveryPlanV1 {
         public CostumeRecoveryPlanV1(CostumeStateV1 state, CostumeRecoveryReason reason, bool hasBinding) {
             State = state;
             Reason = reason;
             HasBinding = hasBinding;
             
            }
         public CostumeStateV1 State {
             get;
             
            }
         public CostumeRecoveryReason Reason {
             get;
             
            }
         public bool HasBinding {
             get;
             
            }
         
        }
    
    public static class CostumeRecoverySelectorV1
    {
         public static CostumeRecoveryPlanV1 Recover(CostumeCatalogV1 c, CostumeRecoveryInputV1 input) {
             if (c == null) throw new ArgumentNullException();
             CostumeStateV1 s;
             if (!Decode(input.Primary, out s) && !Decode(input.Previous, out s) && !Decode(input.Temp, out s)) return Bootstrap(c);
             var current = new List<KeyValuePair<string, string>>(s.CurrentByActor);
             var reason = CostumeRecoveryReason.None;
             for (var i = 0; i < current.Count; i++) {
                 var d = c.Find(current[i].Value);
                 if (d == null || d.Disposition != CostumeDisposition.Accepted || !s.IsUnlocked(current[i].Value)) {
                     var def = c.AcceptedDefault(current[i].Key);
                 if (def == null) return new CostumeRecoveryPlanV1(s.WithoutCurrent(current[i].Key), CostumeRecoveryReason.NoAcceptedDefault, false);
                     current[i] = new KeyValuePair<string, string>(current[i].Key, def.CostumeId);
                     reason = CostumeRecoveryReason.InvalidCurrentFallback;
                     
                    }
                 
                }
             return new CostumeRecoveryPlanV1(new CostumeStateV1(s.Revision, s.UnlockedCostumeIds, current), reason, true);
             
            }
         private static bool Decode(byte[] b, out CostumeStateV1 s) {
             s = null;
             return b != null && CostumeCanonicalCodecV1.TryDecode(b, out s);
             
            }
         private static CostumeRecoveryPlanV1 Bootstrap(CostumeCatalogV1 c) {
             var ids = new List<string>();
             var cur = new List<KeyValuePair<string, string>>();
             foreach (var d in c.Definitions) if (d.Disposition == CostumeDisposition.Accepted && d.DefaultGrant) {
                 ids.Add(d.CostumeId);
                 cur.Add(new KeyValuePair<string, string>(d.ActorId, d.CostumeId));
                 
                }
             if (ids.Count == 0) return new CostumeRecoveryPlanV1(new CostumeStateV1(0, ids, cur), CostumeRecoveryReason.NoAcceptedDefault, false);
             return new CostumeRecoveryPlanV1(new CostumeStateV1(0, ids, cur), CostumeRecoveryReason.DefaultBootstrap, true);
             
            }
         
        }
    
    public static class CostumeCanonicalCodecV1
    {
        
        // Canonical UTF-8 value codec only; no filesystem or profile/run dependency.
        public static byte[] Encode(CostumeStateV1 state) {
             if (state == null) throw new ArgumentNullException();
             var b = new StringBuilder("v1\n").Append(state.Revision.ToString(CultureInfo.InvariantCulture)).Append('\n');
             foreach (var id in state.UnlockedCostumeIds) b.Append("u:").Append(id).Append('\n');
             foreach (var p in state.CurrentByActor) b.Append("c:").Append(p.Key).Append('=').Append(p.Value).Append('\n');
             foreach (var p in state.IssuerRevisions) b.Append("l:").Append(p.Key).Append('=').Append(p.Value.ToString(CultureInfo.InvariantCulture)).Append('\n');
             foreach (var p in state.AppliedGrants) b.Append("g:").Append(p.Key).Append('=').Append(p.Value).Append('\n');
             var payload = b.ToString();
             return new UTF8Encoding(false, true).GetBytes(payload + "i:" + Hash(payload));
             
            }
        
        public static bool TryDecode(byte[] bytes, out CostumeStateV1 state) {
             state = null;
             try {
                 var text = new UTF8Encoding(false, true).GetString(bytes);
                 var lines = text.Split(new[] {
                     '\n' 
                    }
                , StringSplitOptions.None);
                 if (lines.Length < 3 || lines[0] != "v1" || !lines[lines.Length - 1].StartsWith("i:", StringComparison.Ordinal)) return false;
                 var payload = text.Substring(0, text.LastIndexOf("i:", StringComparison.Ordinal));
                 if (Hash(payload) != lines[lines.Length - 1].Substring(2)) return false;
                 long rev = long.Parse(lines[1], CultureInfo.InvariantCulture);
                 var u = new List<string>();
                 var c = new List<KeyValuePair<string, string>>();
                 var l = new List<KeyValuePair<string, long>>();
                 var g = new List<KeyValuePair<string, string>>();
                 for (var i = 2; i < lines.Length - 1; i++) {
                     var x = lines[i].Substring(2).Split('=');
                     if (lines[i].StartsWith("u:")) u.Add(lines[i].Substring(2));
                     else if (lines[i].StartsWith("c:") && x.Length == 2) c.Add(new KeyValuePair<string, string>(x[0], x[1]));
                     else if (lines[i].StartsWith("l:") && x.Length == 2) l.Add(new KeyValuePair<string, long>(x[0], long.Parse(x[1], CultureInfo.InvariantCulture)));
                     else if (lines[i].StartsWith("g:") && x.Length == 2) g.Add(new KeyValuePair<string, string>(x[0], x[1]));
                     else return false;
                     
                    }
                 state = new CostumeStateV1(rev, u, c, l, g);
                 return true;
                 
                }
             catch {
                 return false;
                 
                }
             
            }
        
        private static string Hash(string value) {
             using (var sha = SHA256.Create()) {
                 var h = sha.ComputeHash(new UTF8Encoding(false, true).GetBytes(value));
                 var b = new StringBuilder(64);
                 foreach (var x in h) b.Append(x.ToString("x2", CultureInfo.InvariantCulture));
                 return b.ToString();
                 
                }
             
            }
        
    
        }
    
}


