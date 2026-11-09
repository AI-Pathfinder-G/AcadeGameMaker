using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using UnityEngine;

namespace AcadeGameMaker.Run.Unity
{
    // This effect bridge only READS completed regular-enemy behavior.
    // It cannot affect AI, attack damage, hitboxes, cooldown, collision,
    // targeting, saves, health or native input. Do not infer a hit from
    // a firing/attack animation. A null or stale view creates no VFX.
    internal static class Chapter1RegularEnemyVfx
    {
        internal const int Cell=64;
        internal const int Columns=8;
        internal const int Rows=4;
        private const int Ppu=18;
        private static bool _attempted;
        private static Sprite[,] _frames;

        internal static int WalkerEffectKey(WalkerBehaviorPhase phase)
        {
            switch(phase)
            {
                case WalkerBehaviorPhase.DashTelegraph: return 3;
                case WalkerBehaviorPhase.DashActive: return 4;
                case WalkerBehaviorPhase.Recovery: return 7;
                default: return -1;
            }
        }

        internal static int SurveyorEffectKey(SurveyorBehaviorPhase phase)
        {
            switch(phase)
            {
                case SurveyorBehaviorPhase.FireTelegraph: return 3;
                case SurveyorBehaviorPhase.Fire: return 4;
                case SurveyorBehaviorPhase.Suppressed: return 5;
                case SurveyorBehaviorPhase.Cooldown: return 7;
                default: return -1;
            }
        }

        internal static bool CandidateAvailable
        {
            get { EnsureLoaded(); return _frames!=null; }
        }

        internal static Sprite SpriteAt(int row,int key)
        {
            EnsureLoaded();
            if(_frames==null || row<0 || row>=Rows || key<0 || key>=Columns)
                return null;
            // No blank/idle/stride/death keys can draw effects.
            if((row<2 && key!=3 && key!=4 && key!=7) ||
               (row>=2 && key!=3 && key!=4 && key!=5 && key!=7))
                return null;
            return _frames[row,key];
        }

        internal static Sprite ForEnemy(Collider2D source,
            CarriedBehaviorView? published)
        {
            // Require actual completed evidence for every cue. Do not show
            // warnings or impacts in paused/editor/unknown encounters.
            if(!Chapter1RegularEnemyArt.IsEnemy(source) || !published.HasValue)
                return null;
            var pair=published.Value.Snapshot;
            var walker=source.gameObject.name=="Walker";
            var row=walker
                ?(pair.Walker.Direction<0?1:0)
                :(pair.Surveyor.Direction<0?3:2);
            var key=walker
                ?WalkerEffectKey(pair.Walker.Phase)
                :SurveyorEffectKey(pair.Surveyor.Phase);
            return SpriteAt(row,key);
        }

        private static void EnsureLoaded()
        {
            if(_attempted) return;
            _attempted=true;
            var texture=Resources.Load<Texture2D>(
                "Chapter1Enemies/Candidate-RegularEnemyVfx");
            var profile=Resources.Load<TextAsset>(
                "Chapter1Enemies/candidate-enemy-vfx-profile");
            if(texture==null || profile==null ||
               !profile.text.Contains(
                   "ch1-regular-enemy-v3-published-phase-vfx-v1") ||
               texture.width!=Columns*Cell || texture.height!=Rows*Cell ||
               texture.filterMode!=FilterMode.Point || texture.mipmapCount!=1)
                return;
            var frames=new Sprite[Rows,Columns];
            for(var row=0;row<Rows;row++)
            for(var key=0;key<Columns;key++)
            {
                var rect=new Rect(key*Cell,
                    texture.height-(row+1)*Cell,Cell,Cell);
                frames[row,key]=Sprite.Create(texture,rect,
                    new Vector2(.5f,.1875f),Ppu,0,
                    SpriteMeshType.FullRect);
                frames[row,key].name="CH1EnemyFX"+row+"_"+key;
            }
            _frames=frames;
        }
    }
}