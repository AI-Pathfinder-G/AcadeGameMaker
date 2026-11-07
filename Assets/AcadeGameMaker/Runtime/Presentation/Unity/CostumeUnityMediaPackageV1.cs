using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Costumes;
using AcadeGameMaker.Presentation;
using UnityEngine;

namespace AcadeGameMaker.Presentation.Unity
{
    // REQ-CUA-003/004/008: synthetic-only, complete package validation before any publication.
    public enum CostumeUnityPackageRejectionV1 { None = 0, Identity = 1, Revision = 2, Hash = 3, Manifest = 4, Reference = 5, Geometry = 6, Filter = 7, Ppu = 8, Pivot = 9, Baseline = 10, Clips = 11, Synthetic = 12 }

    public readonly struct CostumeUnityFrameV1
    {
        public CostumeUnityFrameV1(int x, int y, int width, int height, int pivotXQ1000, int pivotYQ1000, int baselineY, int durationTicks)
        {
            if (x < 0 || y < 0 || width <= 0 || height <= 0 || durationTicks <= 0) throw new ArgumentOutOfRangeException();
            X = x; Y = y; Width = width; Height = height; PivotXQ1000 = pivotXQ1000; PivotYQ1000 = pivotYQ1000; BaselineY = baselineY; DurationTicks = durationTicks;
        }
        public int X { get; } public int Y { get; } public int Width { get; } public int Height { get; }
        public int PivotXQ1000 { get; } public int PivotYQ1000 { get; } public int BaselineY { get; } public int DurationTicks { get; }
    }

    public sealed class CostumeUnityClipV1
    {
        private readonly CostumeUnityFrameV1[] _frames;
        public CostumeUnityClipV1(string actionId, string facing, bool loop, IEnumerable<CostumeUnityFrameV1> frames)
        {
            CheckId(actionId); if (facing != "left" && facing != "right") throw new ArgumentException(nameof(facing));
            _frames = new List<CostumeUnityFrameV1>(frames ?? throw new ArgumentNullException(nameof(frames))).ToArray();
            if (_frames.Length == 0) throw new ArgumentException("Clip requires frames.");
            ActionId = actionId; Facing = facing; Loop = loop;
        }
        public string ActionId { get; } public string Facing { get; } public bool Loop { get; }
        public IReadOnlyList<CostumeUnityFrameV1> Frames { get { return Array.AsReadOnly((CostumeUnityFrameV1[])_frames.Clone()); } }
        private static void CheckId(string value) { if (String.IsNullOrEmpty(value)) throw new ArgumentException(); }
        // REQ-CUA-005: pure phase-to-frame mapping; it reads no clock or engine state.
        public CostumeUnityFrameV1 FrameFor(long actionAgeQ1000)
        {
            if (actionAgeQ1000 < 0 || actionAgeQ1000 > 1000) throw new ArgumentOutOfRangeException(nameof(actionAgeQ1000));
            var count = _frames.Length;
            var index = Loop && actionAgeQ1000 == 1000 ? 0 : (int)(actionAgeQ1000 * count / 1000);
            if (!Loop && index >= count) index = count - 1;
            return _frames[index];
        }
    }

    public sealed class CostumeUnityMediaPackageV1
    {
        // This bounded, synthetic-only child has one approved body profile.  Real-media
        // PPU choices remain deferred and cannot enter through this package surface.
        public const int SyntheticApprovedPpu = 1;
        private readonly byte[] _portraitBytes, _atlasBytes, _clipMapBytes, _sourceManifestBytes;
        private readonly CostumeUnityClipV1[] _clips;
        public CostumeUnityMediaPackageV1(string actorId, string costumeId, string portraitSetId, string gameplaySetId, long catalogRevision, long presentationRevision, bool synthetic, Texture2D portrait, Texture2D atlas, byte[] portraitBytes, byte[] atlasBytes, byte[] clipMapBytes, byte[] sourceManifestBytes, int ppu, IEnumerable<CostumeUnityClipV1> clips)
        {
            CheckId(actorId); CheckId(costumeId); CheckId(portraitSetId); CheckId(gameplaySetId);
            if (!synthetic || catalogRevision < 0 || presentationRevision < 0 || ppu <= 0) throw new ArgumentException("Only a complete synthetic package is admissible.");
            ActorId = actorId; CostumeId = costumeId; PortraitSetId = portraitSetId; GameplaySetId = gameplaySetId; CatalogRevision = catalogRevision; PresentationRevision = presentationRevision; Synthetic = synthetic; Portrait = portrait; Atlas = atlas; Ppu = ppu;
            _portraitBytes = Copy(portraitBytes); _atlasBytes = Copy(atlasBytes); _clipMapBytes = Copy(clipMapBytes); _sourceManifestBytes = Copy(sourceManifestBytes);
            _clips = new List<CostumeUnityClipV1>(clips ?? throw new ArgumentNullException(nameof(clips))).ToArray();
            if (_clips.Length == 0) throw new ArgumentException("Package requires clips.");
        }
        public string ActorId { get; } public string CostumeId { get; } public string PortraitSetId { get; } public string GameplaySetId { get; }
        public long CatalogRevision { get; } public long PresentationRevision { get; } public bool Synthetic { get; } public Texture2D Portrait { get; } public Texture2D Atlas { get; } public int Ppu { get; }
        public byte[] PortraitBytes { get { return Copy(_portraitBytes); } } public byte[] AtlasBytes { get { return Copy(_atlasBytes); } } public byte[] ClipMapBytes { get { return Copy(_clipMapBytes); } } public byte[] SourceManifestBytes { get { return Copy(_sourceManifestBytes); } }
        public IReadOnlyList<CostumeUnityClipV1> Clips { get { return Array.AsReadOnly((CostumeUnityClipV1[])_clips.Clone()); } }
        public CostumeUnityClipV1 FindClip(string actionId, string facing) { foreach (var clip in _clips) if (clip.ActionId == actionId && clip.Facing == facing) return clip; return null; }

        public CostumeUnityPackageRejectionV1 Validate(CostumeDefinitionV1 definition, CostumePresentationBindingV1 binding)
        {
            if (!Synthetic) return CostumeUnityPackageRejectionV1.Synthetic;
            if (definition == null || definition.Disposition != CostumeDisposition.Accepted || definition.ActorId != ActorId || definition.CostumeId != CostumeId || definition.PortraitSetId != PortraitSetId || definition.GameplaySetId != GameplaySetId) return CostumeUnityPackageRejectionV1.Identity;
            if (binding.ActorId != ActorId || binding.CostumeId != CostumeId || binding.PortraitSetId != PortraitSetId || binding.GameplayAtlasSetId != GameplaySetId) return CostumeUnityPackageRejectionV1.Identity;
            if (definition.PresentationRevision != PresentationRevision || binding.CatalogRevision != CatalogRevision || binding.PresentationRevision != PresentationRevision) return CostumeUnityPackageRejectionV1.Revision;
            if (binding.PortraitHash != definition.PortraitHash || binding.GameplayAtlasHash != definition.AtlasHash || binding.ClipMapHash != definition.ClipMapHash) return CostumeUnityPackageRejectionV1.Hash;
            if (Portrait == null || Atlas == null || !Portrait || !Atlas) return CostumeUnityPackageRejectionV1.Reference;
            if (Portrait.filterMode != FilterMode.Point || Atlas.filterMode != FilterMode.Point) return CostumeUnityPackageRejectionV1.Filter;
            if (Ppu != SyntheticApprovedPpu) return CostumeUnityPackageRejectionV1.Ppu;
            if (Hash(_portraitBytes) != definition.PortraitHash || Hash(_atlasBytes) != definition.AtlasHash || Hash(_clipMapBytes) != definition.ClipMapHash) return CostumeUnityPackageRejectionV1.Hash;
            if (!BytesEqual(_clipMapBytes, CanonicalClipMapBytes())) return CostumeUnityPackageRejectionV1.Manifest;
            if (!BytesEqual(_sourceManifestBytes, CanonicalSourceManifestBytes(definition))) return CostumeUnityPackageRejectionV1.Manifest;
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var actions = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var clip in _clips)
            {
                if (!seen.Add(clip.ActionId + "\n" + clip.Facing)) return CostumeUnityPackageRejectionV1.Clips;
                actions.Add(clip.ActionId);
                var rectangles = new HashSet<string>(StringComparer.Ordinal);
                foreach (var frame in clip.Frames)
                {
                    long right;
                    long top;
                    try { right = checked((long)frame.X + (long)frame.Width); top = checked((long)frame.Y + (long)frame.Height); }
                    catch (OverflowException) { return CostumeUnityPackageRejectionV1.Geometry; }
                    if (frame.X < 0 || frame.Y < 0 || frame.Width <= 0 || frame.Height <= 0 || right > Atlas.width || top > Atlas.height) return CostumeUnityPackageRejectionV1.Geometry;
                    if (frame.PivotXQ1000 != 500 || frame.PivotYQ1000 != 0) return CostumeUnityPackageRejectionV1.Pivot;
                    if (frame.BaselineY != 0) return CostumeUnityPackageRejectionV1.Baseline;
                    if (!rectangles.Add(frame.X.ToString(CultureInfo.InvariantCulture) + "," + frame.Y.ToString(CultureInfo.InvariantCulture) + "," + frame.Width.ToString(CultureInfo.InvariantCulture) + "," + frame.Height.ToString(CultureInfo.InvariantCulture))) return CostumeUnityPackageRejectionV1.Clips;
                }
            }
            if (binding.ClipIds.Count != actions.Count) return CostumeUnityPackageRejectionV1.Clips;
            var index = 0;
            foreach (var action in actions)
            {
                if (binding.ClipIds[index++] != action || FindClip(action, "left") == null || FindClip(action, "right") == null) return CostumeUnityPackageRejectionV1.Clips;
            }
            return CostumeUnityPackageRejectionV1.None;
        }

        public byte[] CanonicalClipMapBytes()
        {
            var ordered = new List<CostumeUnityClipV1>(_clips); ordered.Sort((a, b) => { var x = StringComparer.Ordinal.Compare(a.ActionId, b.ActionId); return x != 0 ? x : (a.Facing == b.Facing ? 0 : a.Facing == "left" ? -1 : 1); });
            var sb = new StringBuilder("{\"schemaVersion\":1,\"actorId\":\"").Append(ActorId).Append("\",\"costumeId\":\"").Append(CostumeId).Append("\",\"gameplaySetId\":\"").Append(GameplaySetId).Append("\",\"ppu\":").Append(Ppu.ToString(CultureInfo.InvariantCulture)).Append(",\"cellWidth\":").Append(Atlas.width.ToString(CultureInfo.InvariantCulture)).Append(",\"cellHeight\":").Append(Atlas.height.ToString(CultureInfo.InvariantCulture)).Append(",\"pivotXQ1000\":500,\"pivotYQ1000\":0,\"baselineY\":0,\"clips\":[");
            for (var i = 0; i < ordered.Count; i++) { var c = ordered[i]; if (i != 0) sb.Append(','); sb.Append("{\"actionId\":\"").Append(c.ActionId).Append("\",\"facing\":\"").Append(c.Facing).Append("\",\"loop\":").Append(c.Loop ? "true" : "false").Append(",\"frames\":["); for (var j = 0; j < c.Frames.Count; j++) { var f = c.Frames[j]; if (j != 0) sb.Append(','); sb.Append("{\"x\":").Append(f.X).Append(",\"y\":").Append(f.Y).Append(",\"width\":").Append(f.Width).Append(",\"height\":").Append(f.Height).Append(",\"pivotXQ1000\":").Append(f.PivotXQ1000).Append(",\"pivotYQ1000\":").Append(f.PivotYQ1000).Append(",\"baselineY\":").Append(f.BaselineY).Append(",\"durationTicks\":").Append(f.DurationTicks).Append('}'); } sb.Append("]}"); }
            return new UTF8Encoding(false, true).GetBytes(sb.Append("]}").ToString());
        }
        // REQ-CUA-003: this evidence document binds all three catalog payload hashes
        // and the closed action/facing surface; it never substitutes for those hashes.
        public byte[] CanonicalSourceManifestBytes(CostumeDefinitionV1 d)
        {
            var actions = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var clip in _clips) actions.Add(clip.ActionId);
            var sb = new StringBuilder("{\"actorId\":\"").Append(ActorId).Append("\",\"costumeId\":\"").Append(CostumeId).Append("\",\"portraitSetId\":\"").Append(PortraitSetId).Append("\",\"gameplaySetId\":\"").Append(GameplaySetId).Append("\",\"catalogRevision\":").Append(CatalogRevision.ToString(CultureInfo.InvariantCulture)).Append(",\"presentationRevision\":").Append(PresentationRevision.ToString(CultureInfo.InvariantCulture)).Append(",\"portraitHash\":\"").Append(d.PortraitHash).Append("\",\"atlasHash\":\"").Append(d.AtlasHash).Append("\",\"clipMapHash\":\"").Append(d.ClipMapHash).Append("\",\"requiredActionIds\":[");
            var index = 0;
            foreach (var action in actions) { if (index++ != 0) sb.Append(','); sb.Append('\"').Append(action).Append('\"'); }
            sb.Append("],\"requiredFacingIds\":[\"left\",\"right\"],\"clipMapSchemaVersion\":1}");
            return new UTF8Encoding(false, true).GetBytes(sb.ToString());
        }
        public static string Hash(byte[] bytes) { if (bytes == null) return String.Empty; using (var sha = SHA256.Create()) { var x = sha.ComputeHash(bytes); var b = new StringBuilder(64); foreach (var v in x) b.Append(v.ToString("x2", CultureInfo.InvariantCulture)); return b.ToString(); } }
        private static void CheckId(string value) { if (String.IsNullOrEmpty(value)) throw new ArgumentException(); }
        private static byte[] Copy(byte[] value) { return value == null ? null : (byte[])value.Clone(); }
        private static bool BytesEqual(byte[] a, byte[] b) { if (a == null || b == null || a.Length != b.Length) return false; for (var i = 0; i < a.Length; i++) if (a[i] != b[i]) return false; return true; }
    }
}
