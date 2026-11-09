using System;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using UnityEngine;

namespace AcadeGameMaker.Run.Unity
{
    // Original industrial enemy pixel-art candidate. PURE PRESENTATION:
    // this bridge only reads the existing -170 completed behavior publication.
    // It never drives animations back into combat, enemy AI, movement, hitboxes,
    // target discovery, saves, input, damage or boss/room progression.
    internal static class Chapter1RegularEnemyArt
    {
        internal const int Cell=64;
        internal const int Columns=8;
        internal const int Rows=4;
        private const int PixelsPerUnit=18;
        private static Texture2D _atlas;
        private static Sprite[,] _sprites;
        private static bool _attempted;

        internal static bool IsEnemy(Collider2D source)
        {
            return source!=null && !source.isTrigger &&
                (source.gameObject.name=="Walker" ||
                 source.gameObject.name=="Surveyor");
        }

        internal static int SelectWalkerKey(WalkerBehaviorPhase phase,
            RegularEnemyMovementIntent movement, int publishedTick)
        {
            switch(phase)
            {
                case WalkerBehaviorPhase.Approach:
                    return movement==RegularEnemyMovementIntent.None?0:
                        1+((Math.Max(0,publishedTick)/6)&1);
                case WalkerBehaviorPhase.DashTelegraph: return 3;
                case WalkerBehaviorPhase.DashActive: return 4;
                case WalkerBehaviorPhase.Recovery: return 7;
                case WalkerBehaviorPhase.Dead: return 6;
                case WalkerBehaviorPhase.Reset: return 0;
                default:return 0;
            }
        }

        internal static int SelectSurveyorKey(SurveyorBehaviorPhase phase,
            RegularEnemyMovementIntent movement, int publishedTick)
        {
            switch(phase)
            {
                case SurveyorBehaviorPhase.Relocate:
                    return movement==RegularEnemyMovementIntent.None?0:
                        1+((Math.Max(0,publishedTick)/7)&1);
                case SurveyorBehaviorPhase.FireTelegraph: return 3;
                case SurveyorBehaviorPhase.Fire: return 4;
                case SurveyorBehaviorPhase.Cooldown: return 7;
                case SurveyorBehaviorPhase.Suppressed: return 5;
                case SurveyorBehaviorPhase.Dead: return 6;
                case SurveyorBehaviorPhase.Reset: return 0;
                default: return 0;
            }
        }

        internal static bool CandidateAvailable
        {
            get { EnsureLoaded(); return _sprites!=null; }
        }

        internal static Sprite SpriteAt(int row, int key)
        {
            EnsureLoaded();
            if(_sprites==null || row<0 || row>=Rows || key<0 || key>=Columns)
                return null;
            return _sprites[row,key];
        }

        internal static Sprite ForEnemy(Collider2D source,
            CarriedBehaviorView? published)
        {
            if(!IsEnemy(source)) return null;
            EnsureLoaded();
            if(_sprites==null) return null;
            var isWalker=source.gameObject.name=="Walker";
            // No published evidence means a neutral authored idle, NEVER
            // guessed telegraph or fake fired shot. Each left row is separately
            // drawn, not horizontal SpriteRenderer.flipX of another costume.
            var row=isWalker?0:2;
            var key=0;
            if(published.HasValue)
            {
                var snap=published.Value.Snapshot;
                var tick=published.Value.Tick.Value;
                if(isWalker)
                {
                    row=snap.Walker.Direction<0?1:0;
                    key=SelectWalkerKey(snap.Walker.Phase,
                        snap.Walker.Movement,tick);
                }
                else
                {
                    row=snap.Surveyor.Direction<0?3:2;
                    key=SelectSurveyorKey(snap.Surveyor.Phase,
                        snap.Surveyor.Movement,tick);
                }
            }
            return _sprites[row,key];
        }

        private static void EnsureLoaded()
        {
            if(_attempted) return;
            _attempted=true;
            var texture=Resources.Load<Texture2D>(
                "Chapter1Enemies/Candidate-RegularEnemies");
            if(texture==null || texture.width!=Columns*Cell ||
               texture.height!=Rows*Cell ||
               texture.filterMode!=FilterMode.Point ||
               texture.mipmapCount!=1)
                return;
            _atlas=texture;
            var sprites=new Sprite[Rows,Columns];
            for(var row=0;row<Rows;row++)
            for(var key=0;key<Columns;key++)
            {
                var rect=new Rect(key*Cell,
                    texture.height-(row+1)*Cell,Cell,Cell);
                var sprite=Sprite.Create(texture,rect,
                    new Vector2(.5f,.1875f),PixelsPerUnit,0,
                    SpriteMeshType.FullRect);
                sprite.name="CH1Enemy"+row+"_"+key;
                sprites[row,key]=sprite;
            }
            _sprites=sprites;
        }
    }
}