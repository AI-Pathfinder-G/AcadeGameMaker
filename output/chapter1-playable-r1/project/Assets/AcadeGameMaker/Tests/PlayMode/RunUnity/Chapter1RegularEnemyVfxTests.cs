#if UNITY_EDITOR
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Run.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.RunUnity
{
    internal sealed class Chapter1RegularEnemyVfxTests
    {
        [Test] public void OnlyActualPublishedPhasesCanDisplayCues()
        {
            Assert.That(Chapter1RegularEnemyVfx.WalkerEffectKey(
                WalkerBehaviorPhase.Approach),Is.EqualTo(-1));
            Assert.That(Chapter1RegularEnemyVfx.WalkerEffectKey(
                WalkerBehaviorPhase.Reset),Is.EqualTo(-1));
            Assert.That(Chapter1RegularEnemyVfx.WalkerEffectKey(
                WalkerBehaviorPhase.Dead),Is.EqualTo(-1));
            Assert.That(Chapter1RegularEnemyVfx.WalkerEffectKey(
                WalkerBehaviorPhase.DashTelegraph),Is.EqualTo(3));
            Assert.That(Chapter1RegularEnemyVfx.WalkerEffectKey(
                WalkerBehaviorPhase.DashActive),Is.EqualTo(4));
            Assert.That(Chapter1RegularEnemyVfx.WalkerEffectKey(
                WalkerBehaviorPhase.Recovery),Is.EqualTo(7));
            Assert.That(Chapter1RegularEnemyVfx.SurveyorEffectKey(
                SurveyorBehaviorPhase.Relocate),Is.EqualTo(-1));
            Assert.That(Chapter1RegularEnemyVfx.SurveyorEffectKey(
                SurveyorBehaviorPhase.Reset),Is.EqualTo(-1));
            Assert.That(Chapter1RegularEnemyVfx.SurveyorEffectKey(
                SurveyorBehaviorPhase.Dead),Is.EqualTo(-1));
            Assert.That(Chapter1RegularEnemyVfx.SurveyorEffectKey(
                SurveyorBehaviorPhase.FireTelegraph),Is.EqualTo(3));
            Assert.That(Chapter1RegularEnemyVfx.SurveyorEffectKey(
                SurveyorBehaviorPhase.Fire),Is.EqualTo(4));
            Assert.That(Chapter1RegularEnemyVfx.SurveyorEffectKey(
                SurveyorBehaviorPhase.Suppressed),Is.EqualTo(5));
            Assert.That(Chapter1RegularEnemyVfx.SurveyorEffectKey(
                SurveyorBehaviorPhase.Cooldown),Is.EqualTo(7));
        }

        [Test] public void VfxImportHasSameFootPivotAndNoBlankIdleMoveDeathEffects()
        {
            var atlas=Resources.Load<Texture2D>(
                "Chapter1Enemies/Candidate-RegularEnemyVfx");
            var profile=Resources.Load<TextAsset>(
                "Chapter1Enemies/candidate-enemy-vfx-profile");
            Assert.That(atlas,Is.Not.Null);
            Assert.That(profile,Is.Not.Null);
            Assert.That(atlas.width,Is.EqualTo(512));
            Assert.That(atlas.height,Is.EqualTo(256));
            Assert.That(atlas.filterMode,Is.EqualTo(FilterMode.Point));
            Assert.That(atlas.mipmapCount,Is.EqualTo(1));
            Assert.That(profile.text,Does.Contain(
                "ch1-regular-enemy-v3-published-phase-vfx-v1"));
            Assert.That(profile.text,Does.Contain(
                "visual-only-candidate-not-artist-approved"));
            Assert.That(profile.text,Does.Contain(
                "build_ch1_regular_enemy_v3_effects.py"));
            Assert.That(Chapter1RegularEnemyVfx.CandidateAvailable,Is.True);
            var activeCount=0;
            for(var row=0;row<4;++row)
            for(var key=0;key<8;++key)
            {
                var sprite=Chapter1RegularEnemyVfx.SpriteAt(row,key);
                var expected=row<2?(key==3 || key==4 || key==7):
                    (key==3 || key==4 || key==5 || key==7);
                if(!expected)
                {
                    Assert.That(sprite,Is.Null,"no unsolicited visual key");
                    continue;
                }
                ++activeCount;
                Assert.That(sprite,Is.Not.Null);
                Assert.That(sprite.texture,Is.SameAs(atlas));
                Assert.That(sprite.pixelsPerUnit,Is.EqualTo(18f));
                Assert.That(sprite.pivot.x,Is.EqualTo(32f));
                Assert.That(sprite.pivot.y,Is.EqualTo(12f));
                Assert.That(sprite.rect.x/512f,
                    Is.EqualTo(key/8f).Within(.0001f));
                Assert.That(sprite.rect.y/256f,
                    Is.EqualTo((3-row)/4f).Within(.0001f));
            }
            Assert.That(activeCount,Is.EqualTo(14));
            Assert.That(Chapter1RegularEnemyVfx.SpriteAt(-1,3),Is.Null);
            Assert.That(Chapter1RegularEnemyVfx.SpriteAt(4,4),Is.Null);
            Assert.That(Chapter1RegularEnemyVfx.SpriteAt(2,8),Is.Null);
        }

        [Test] public void EnemyVfxNeedsCompletedBehaviorAndKeepsFacingWithoutMutation()
        {
            var walkerGo=new GameObject("Walker");
            var surveyorGo=new GameObject("Surveyor");
            var ordanGo=new GameObject("Ordan");
            try
            {
                var walkerCol=walkerGo.AddComponent<BoxCollider2D>();
                var surveyorCol=surveyorGo.AddComponent<BoxCollider2D>();
                var ordanCol=ordanGo.AddComponent<BoxCollider2D>();
                Assert.That(Chapter1RegularEnemyVfx.ForEnemy(walkerCol,null),
                    Is.Null,"no published Walker evidence");
                Assert.That(Chapter1RegularEnemyVfx.ForEnemy(surveyorCol,null),
                    Is.Null,"no published Surveyor evidence");
                var tick=new SimulationTick(55);
                var walker=new WalkerBehaviorSnapshot(
                    WalkerBehaviorPhase.DashTelegraph,58,true,
                    RegularEnemyMovementIntent.None,-1,
                    false,false,-1,1);
                var surveyor=new SurveyorBehaviorSnapshot(
                    SurveyorBehaviorPhase.Fire,58,true,
                    RegularEnemyMovementIntent.None,1,
                    false,true,12,2);
                var pair=new RegularEnemyBehaviorPairSnapshot(
                    tick,walker,surveyor,null,null,new SimulationTick(56));
                var publication=new CarriedBehaviorView(tick,pair);
                Assert.That(Chapter1RegularEnemyVfx.ForEnemy(walkerCol,publication),
                    Is.SameAs(Chapter1RegularEnemyVfx.SpriteAt(1,3)));
                Assert.That(Chapter1RegularEnemyVfx.ForEnemy(surveyorCol,publication),
                    Is.SameAs(Chapter1RegularEnemyVfx.SpriteAt(2,4)));
                Assert.That(Chapter1RegularEnemyVfx.ForEnemy(ordanCol,publication),
                    Is.Null,"boss must not be affected by regular-enemy FX");
                walkerCol.isTrigger=true;
                Assert.That(Chapter1RegularEnemyVfx.ForEnemy(walkerCol,publication),
                    Is.Null,"only authoritative solid enemy collider can render FX");
                Assert.That(publication.Snapshot.Walker.Phase,
                    Is.EqualTo(WalkerBehaviorPhase.DashTelegraph));
                Assert.That(publication.Snapshot.Surveyor.Phase,
                    Is.EqualTo(SurveyorBehaviorPhase.Fire));
            }
            finally
            {
                Object.DestroyImmediate(walkerGo);
                Object.DestroyImmediate(surveyorGo);
                Object.DestroyImmediate(ordanGo);
            }
        }
    }
}
#endif