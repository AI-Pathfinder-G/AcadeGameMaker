using System;
using AcadeGameMaker.Input.Unity;
using UnityEngine;

namespace AcadeGameMaker.Run.Unity
{
    // Package A visual-only candidate; left is separately derived from the left-authored static master. No horizontal mirroring. Never controls physics,
    // combat damage, input, collision, camera, save, or encounter lifetime.
    // Optional left attack/hit frames are procedural candidates, NOT final authored poses;
    // Original Left.png stays the fallback; right gear is never mirrored.
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
        // Combat hits are resolved in the same completed simulation tick as
        // the fired press. The first rendered key pose therefore depicts the
        // strike / recoil, never an invented delayed wind-up.
        private const int AttackVisualTicks = 12;
        private const int HitVisualTicks = 12;

        [Serializable]
        private sealed class CandidateProfile
        {
            public string schema,disposition,facing,movementResource,combatResource;
            public string leftMotionResource,leftMotionStatus;
            public string leftCombatCandidateResource,leftCombatCandidateStatus;
            public bool cannotMirrorLeft;
            public int rows,columns,cellSize,pixelsPerUnit;
        }
        private static Sprite[,] _movement;
        private static Sprite[,] _combat;
        private static Sprite[,] _leftMovement;
        private static Sprite[,] _leftCombat;
        private static bool _loadAttempted;
        private static Collider2D _visualSource;
        private static int _lastTick = -1, _lastHealth = -1;
        private static int _attackStarted = -99999, _hitStarted = -99999;
        private static long _lastAttackOrdinal = -1;
        private static Color _currentTint = Color.white;
        // Presentation cadence derives from completed movement displacement,
        // never wall-clock frames or issued input. Neither gameplay speed nor
        // collision/attack outcomes are changed by these fields.
        private static int _lastGaitTick=-1, _lastGaitXQ4096;
        private static float _gaitPhase;
        internal static Color CurrentVisualTint => _currentTint;

        internal static bool CandidateAvailable
        {
            get { EnsureLoaded(); return _movement != null && _combat != null; }
        }

        // Provisional articulated left motion. Never used for attack/hit
        // or as an authoritative art acceptance claim. The old independently
        // authored Left.png remains the exact fallback for those states.
        internal static bool LeftMotionCandidateAvailable
        {
            get { EnsureLoaded(); return _leftMovement != null; }
        }

        internal static Sprite LeftMotionFrame(int clip, int tick, int vyQ4096,
            int displacementFrame=-1)
        {
            EnsureLoaded();
            if (_leftMovement == null || tick < 0 || clip == ClipAttack ||
                clip == ClipHit || clip < ClipIdle || clip > ClipHit) return null;
            var row=0;
            var frame=0;
            switch (clip)
            {
                case ClipIdle:
                    row=0;
                    frame=(tick/22)%6;
                    break;
                case ClipWalk:
                    row=1;
                    frame=displacementFrame>=0?displacementFrame:(tick/7)%6;
                    break;
                case ClipRun:
                    row=2;
                    frame=displacementFrame>=0?displacementFrame:(tick/4)%6;
                    break;
                case ClipDash:
                    row=2;
                    frame=displacementFrame>=0?displacementFrame:(tick/3)%6;
                    break;
                case ClipJump:
                    row=3;
                    frame=vyQ4096>1024?1:vyQ4096< -1024?4:3;
                    break;
            }
            return _leftMovement[row,frame];
        }
        // Provisional combat-only left frames from the independent left master.
        // Right-facing artwork is never flipped to reconstruct the left.
        // Source attack/hit boxes, damage timing and simulation stay unchanged.
        internal static bool LeftCombatCandidateAvailable
        {
            get { EnsureLoaded(); return _leftCombat != null; }
        }

        internal static Sprite LeftCombatFrame(int clip, int actionAge)
        {
            EnsureLoaded();
            if (_leftCombat == null) return null;
            int row, frame;
            if (clip == ClipAttack)
            {
                row=0;
                frame=SelectAttackFrame(actionAge);
            }
            else if (clip == ClipHit)
            {
                row=1;
                frame=SelectHitFrame(actionAge);
            }
            else return null;
            return frame < 0 ? null : _leftCombat[row,frame];
        }
        // Warm strike/readable recoil at exact completed gameplay events.
        // This is a renderer color only; it cannot affect combat, collision,
        // timing, save, or input. Hit always wins over attack.
        internal static Color VisualTintForAges(int attackAge, int hitAge)
        {
            if (hitAge >= 0 && hitAge < 5)
                return new Color(1f,.63f,.60f,1f);
            if (attackAge >= 0 && attackAge < 4)
                return new Color(1f,.97f,.70f,1f);
            return Color.white;
        }

        internal static bool ShouldStartFiredAttackVisual(
            bool firedAttackThisTick, int tick, int previousStartTick)
        {
            return firedAttackThisTick && tick >= 0 && tick != previousStartTick;
        }

        internal static int SelectAttackFrame(int age)
        {
            if (age < 0 || age >= AttackVisualTicks) return -1;
            if (age < 2) return 3; // strike on the actual fired tick
            if (age < 5) return 4; // authored follow-through
            if (age < 9) return 5; // recover
            return 0; // settle into source stance
        }

        internal static int SelectHitFrame(int age)
        {
            if (age < 0 || age >= HitVisualTicks) return -1;
            if (age < 2) return 3; // immediate damage recoil
            if (age < 5) return 4;
            if (age < 9) return 5;
            return 0;
        }

        internal static int SelectRightHitFrame(int age)
        {
            if (age < 0 || age >= HitVisualTicks) return -1;
            return age < 6 ? 4 : 5;
        }

        // One cycle has six visual keys; run uses a longer physical step
        // than walk. Movement distance is the *completed position delta*,
        // so a blocked actor cannot keep jogging in place or speed up/slow
        // down the gait independently from actual physical travel.
        internal static float AdvanceGaitPhaseFromPosition(float previousPhase,
            int oldXQ4096, int newXQ4096, int elapsedTicks,
            bool grounded, int velocityXQ4096, bool running)
        {
            if(elapsedTicks<=0 || elapsedTicks>8 || !grounded ||
                Math.Abs((long)velocityXQ4096)<1024)
                return previousPhase;
            var deltaQ=Math.Abs((long)newXQ4096-oldXQ4096);
            // Ignore a portal/teleport or a stale sample. Even fast dash
            // cannot travel 0.75 world units within one fixed physics tick.
            if(deltaQ>(long)elapsedTicks*3072L)
                return previousPhase;
            var pixelDistance=deltaQ*PixelsPerUnit/4096f;
            var pixelsPerKey=running?9f:5.5f;
            return Mathf.Repeat(previousPhase+pixelDistance/pixelsPerKey,6f);
        }

        internal static int GaitFrame(float phase)
        {
            if(float.IsNaN(phase) || float.IsInfinity(phase)) return 0;
            return Mathf.Clamp(Mathf.FloorToInt(Mathf.Repeat(phase,6f)),0,5);
        }

        internal static int SelectVisualClip(int vxQ4096, int vyQ4096,
            bool grounded, bool dashing, int attackAge, int hitAge)
        {
            if (hitAge >= 0 && hitAge < HitVisualTicks) return ClipHit;
            if (attackAge >= 0 && attackAge < AttackVisualTicks) return ClipAttack;
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
            if (fallback == null)
            {
                _currentTint = Color.white;
                return null;
            }
            if (router == null ||
                !router.TryGetDoeonAnimationEvidence(source,
                    out var tick, out var vxQ, out var vyQ,
                    out var grounded, out var dashing, out var health,
                    out var attackOrdinal, out var firedAttackThisTick,
                    out var positionXQ4096))
            {
                _currentTint = Color.white;
                return fallback;
            }

            if (!ReferenceEquals(_visualSource, source) || tick < _lastTick ||
                attackOrdinal < _lastAttackOrdinal)
            {
                _visualSource = source;
                _lastHealth = -1;
                _lastAttackOrdinal = -1;
                _attackStarted = _hitStarted = -99999;
                _lastGaitTick=-1;
                _gaitPhase=0f;
            }
            if (_lastHealth >= 0 && health < _lastHealth) _hitStarted = tick;
            // Do not animate cooldown / invalid-aim presses. Attack visuals
            // are triggered by immutable completed Combat FiredHit/FiredMiss,
            // not by the router's issued input sequence counter.
            if (ShouldStartFiredAttackVisual(firedAttackThisTick,tick,_attackStarted))
                _attackStarted = tick;
            if(_lastGaitTick>=0 && tick>_lastGaitTick)
                _gaitPhase=AdvanceGaitPhaseFromPosition(_gaitPhase,
                    _lastGaitXQ4096,positionXQ4096,tick-_lastGaitTick,
                    grounded,vxQ,
                    dashing || Math.Abs((long)vxQ)>=5L*4096L);
            if(tick!=_lastGaitTick)
            {
                _lastGaitTick=tick;
                _lastGaitXQ4096=positionXQ4096;
            }
            _lastTick = tick;
            _lastHealth = health;
            _lastAttackOrdinal = attackOrdinal;

            _currentTint = VisualTintForAges(tick-_attackStarted,
                tick-_hitStarted);
            var clip = SelectVisualClip(vxQ,vyQ,grounded,dashing,
                tick-_attackStarted,tick-_hitStarted);
            var physicalGaitFrame=GaitFrame(_gaitPhase);
            if (facingLeft)
            {
                var age = clip == ClipHit ? tick-_hitStarted : tick-_attackStarted;
                return LeftCombatFrame(clip,age) ??
                    LeftMotionFrame(clip,tick,vyQ,physicalGaitFrame) ?? fallback;
            }
            if (!CandidateAvailable) return fallback;
            var frame = 0;

            var row = 0;
            var sheet = _movement;
            switch (clip)
            {
                case ClipHit:
                    row = 3;
                    // The authored right movement hit source uses frame4
                    // for contact and frame5 for release. The left source
                    // has an independent row1 (SelectHitFrame), so do not
                    // apply left joint-pose indices to right sprite art.
                    frame = SelectRightHitFrame(tick-_hitStarted);
                    if (frame < 0) return fallback;
                    break;
                case ClipAttack:
                    sheet = _combat;
                    row = 0;
                    frame = SelectAttackFrame(tick-_attackStarted);
                    if (frame < 0) return fallback;
                    break;
                case ClipDash:
                    row=3;
                    frame=physicalGaitFrame/2;
                    break;
                case ClipJump:
                    row=2;
                    frame=vyQ>1024?Mathf.Clamp(2-(vyQ/(10*4096)),1,2):
                        vyQ< -1024?4:3;
                    break;
                case ClipRun:
                    row=1;
                    frame=physicalGaitFrame;
                    break;
                case ClipWalk:
                    row=0;
                    frame=2+(physicalGaitFrame*4)/6;
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
            // Optional provisional motion, never accepted or flipped as final
            // left combat art. A corrupt/missing atlas retains the true left
            // static source rather than showing mirrored right equipment.
            if (settings.leftMotionResource == "Chapter1Doeon/Candidate-left-movement" &&
                settings.leftMotionStatus == "procedural-experiment-not-hand-drawn")
            {
                var left=Resources.Load<Texture2D>(settings.leftMotionResource);
                if (Compatible(left)) _leftMovement=Slice(left);
            }
            if (settings.leftCombatCandidateResource ==
                    "Chapter1Doeon/Candidate-left-combat" &&
                settings.leftCombatCandidateStatus ==
                    "procedural-experiment-not-hand-drawn")
            {
                var leftCombat = Resources.Load<Texture2D>(
                    settings.leftCombatCandidateResource);
                if (Compatible(leftCombat)) _leftCombat = Slice(leftCombat);
            }
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