using System;
using AcadeGameMaker.Input.Unity;
using UnityEngine;

namespace AcadeGameMaker.Run.Unity
{
    // Package A visual-only right-facing candidate. Never controls physics,
    // combat damage, input, collision, camera, save, or encounter lifetime.
    // Left-facing movement atlas has not been authored: use existing approved
    // Left static artwork, never a false horizontal flip of asymmetric gear.
    internal static class Chapter1DoeonAnimation
    {
        private const int FrameSize = 64;
        private const float PixelsPerUnit = 18f;
        private const int ClipIdle = 1;
        private const int ClipWalk = 2;
        private const int ClipRun = 3;
        private const int ClipJump = 4;
        private const int ClipDash = 5;
        private const int ClipAttack = 6;
        private const int ClipHit = 7;

        [Serializable]
        private sealed class CandidateProfile
        {
            public string schema,disposition,facing,movementResource,combatResource;
            public bool cannotMirrorLeft;
            public int rows,columns,cellSize,pixelsPerUnit;
        }
        private static Sprite[,] _movement;
        private static Sprite[,] _combat;
        private static bool _loadAttempted;
        private static Collider2D _visualSource;
        private static int _lastTick = -1, _lastHealth = -1;
        private static int _attackStarted = -99999, _hitStarted = -99999;
        private static long _lastAttackOrdinal = -1;

        internal static bool CandidateAvailable
        {
            get { EnsureLoaded(); return _movement != null && _combat != null; }
        }

        internal static int SelectVisualClip(int vxQ4096, int vyQ4096,
            bool grounded, bool dashing, int attackAge, int hitAge)
        {
            if (hitAge >= 0 && hitAge < 12) return ClipHit;
            if (attackAge >= 0 && attackAge < 21) return ClipAttack;
            if (dashing) return ClipDash;
            if (!grounded || (vyQ4096 > 1024 || vyQ4096 < -1024))
                return ClipJump;
            var magnitude = Mathf.Abs(vxQ4096);
            if (magnitude >= 5 * 4096) return ClipRun;
            return magnitude >= 1024 ? ClipWalk : ClipIdle;
        }

        internal static Sprite ForPlayerPose(Collider2D source, bool facingLeft, InputRouter router)
        {
            var fallback = Chapter1DoeonArt.ForPlayer(source, facingLeft);
            if (fallback == null) return null;
            if (router == null ||
                !router.TryGetDoeonAnimationEvidence(source,
                    out var tick, out var vxQ, out var vyQ,
                    out var grounded, out var dashing, out var health,
                    out var attackOrdinal))
                return fallback;

            if (!ReferenceEquals(_visualSource, source) || tick < _lastTick ||
                attackOrdinal < _lastAttackOrdinal)
            {
                _visualSource = source;
                _lastHealth = -1;
                _lastAttackOrdinal = -1;
                _attackStarted = _hitStarted = -99999;
            }
            if (_lastHealth >= 0 && health < _lastHealth) _hitStarted = tick;
            if (_lastAttackOrdinal >= 0 && attackOrdinal > _lastAttackOrdinal)
                _attackStarted = tick;
            _lastTick = tick;
            _lastHealth = health;
            _lastAttackOrdinal = attackOrdinal;

            if (facingLeft || !CandidateAvailable) return fallback;
            var clip = SelectVisualClip(vxQ,vyQ,grounded,dashing,
                tick-_attackStarted,tick-_hitStarted);
            var frame = 0;
            var row = 0;
            var sheet = _movement;
            switch (clip)
            {
                case ClipHit:
                    row = 3;
                    frame = (tick-_hitStarted) < 6 ? 4 : 5;
                    break;
                case ClipAttack:
                    sheet = _combat;
                    row = 0;
                    frame = Mathf.Min(5,(tick-_attackStarted)*6/21);
                    break;
                case ClipDash:
                    row=3;
                    frame=(tick/3)%3;
                    break;
                case ClipJump:
                    row=2;
                    frame=vyQ>1024?Mathf.Clamp(2-(vyQ/(10*4096)),1,2):
                        vyQ< -1024?4:3;
                    break;
                case ClipRun:
                    row=1;
                    frame=(tick/4)%6;
                    break;
                case ClipWalk:
                    row=0;
                    frame=2+(tick/7)%4;
                    break;
                default:
                    row=0;
                    frame=(tick/22)%2;
                    break;
            }
            return sheet[row,frame] ?? fallback;
        }

        private static void EnsureLoaded()
        {
            if (_loadAttempted) return;
            _loadAttempted = true;
            var profile = Resources.Load<TextAsset>(
                "Chapter1Doeon/candidate-animation-profile");
            if (profile == null) return;
            var settings = JsonUtility.FromJson<CandidateProfile>(profile.text);
            if (settings == null ||
                settings.schema != "chapter1-doeon-right-candidate-anim-v1" ||
                settings.disposition != "candidate-not-independent-accepted" ||
                settings.facing != "right-only" || !settings.cannotMirrorLeft ||
                settings.rows != 4 || settings.columns != 6 ||
                settings.cellSize != FrameSize || settings.pixelsPerUnit != 18 ||
                settings.movementResource != "Chapter1Doeon/Candidate-movement" ||
                settings.combatResource != "Chapter1Doeon/Candidate-combat") return;
            var movement = Resources.Load<Texture2D>(settings.movementResource);
            var combat = Resources.Load<Texture2D>(settings.combatResource);
            if (!Compatible(movement) || !Compatible(combat)) return;
            _movement = Slice(movement);
            _combat = Slice(combat);
        }

        private static bool Compatible(Texture2D image) =>
            image != null && image.width == 384 && image.height == 256 &&
            image.mipmapCount == 1 && image.filterMode == FilterMode.Point;

        private static Sprite[,] Slice(Texture2D atlas)
        {
            var sprites = new Sprite[4,6];
            for (var row = 0; row < 4; ++row)
            for (var column = 0; column < 6; ++column)
            {
                // Unity texture rectangles use lower-left origin.
                sprites[row,column] = Sprite.Create(atlas,
                    new Rect(column*FrameSize,(3-row)*FrameSize,
                        FrameSize,FrameSize),
                    new Vector2(.5f,.1875f),PixelsPerUnit,0,
                    SpriteMeshType.FullRect);
                sprites[row,column].name = atlas.name+"-"+row+"-"+column;
            }
            return sprites;
        }

        internal static Rect TextureUv(Sprite sprite)
        {
            var region = sprite.textureRect;
            return new Rect(
                region.x/sprite.texture.width,
                region.y/sprite.texture.height,
                region.width/sprite.texture.width,
                region.height/sprite.texture.height);
        }
    }
}