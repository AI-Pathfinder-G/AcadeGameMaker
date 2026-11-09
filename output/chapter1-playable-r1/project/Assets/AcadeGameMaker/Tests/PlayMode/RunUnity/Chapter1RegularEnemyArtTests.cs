#if UNITY_EDITOR
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Run.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.RunUnity
{
    // Visual-only source bindings. No owner input, simulation advancement,
    // combat damage, enemy transforms or saves are modified by these tests.
    internal sealed class Chapter1RegularEnemyArtTests
    {
        [Test] public void BothEnemyCostumesAndEveryPoseAreDistinctBoundedSprites()
        {
            var atlas=Resources.Load<Texture2D>(
                "Chapter1Enemies/Candidate-RegularEnemies");
            var profile=Resources.Load<TextAsset>(
                "Chapter1Enemies/candidate-enemy-profile");
            Assert.That(atlas,Is.Not.Null);
            Assert.That(atlas.width,Is.EqualTo(512));
            Assert.That(atlas.height,Is.EqualTo(256));
            Assert.That(atlas.filterMode,Is.EqualTo(FilterMode.Point));
            Assert.That(atlas.mipmapCount,Is.EqualTo(1));
            Assert.That(profile,Is.Not.Null);
            Assert.That(profile.text,Does.Contain(
                "provisional-visual-only-not-final-approved"));
            Assert.That(profile.text,Does.Contain(
                "ch1-regular-enemies-visual-candidate-profile-v1"));
            Assert.That(Chapter1RegularEnemyArt.CandidateAvailable,Is.True);
            for(var row=0;row<4;++row)
            for(var key=0;key<8;++key)
            {
                var sprite=Chapter1RegularEnemyArt.SpriteAt(row,key);
                Assert.That(sprite,Is.Not.Null,"row="+row+" key="+key);
                Assert.That(sprite.texture,Is.SameAs(atlas));
                Assert.That(sprite.pixelsPerUnit,Is.EqualTo(18f));
                Assert.That(sprite.rect.width,Is.EqualTo(64f));
                Assert.That(sprite.rect.height,Is.EqualTo(64f));
                Assert.That(sprite.pivot.x,Is.EqualTo(32f).Within(.001f));
                Assert.That(sprite.pivot.y,Is.EqualTo(12f).Within(.001f));
                Assert.That(sprite.rect.x/512f,
                    Is.EqualTo(key/8f).Within(.0001f));
                Assert.That(sprite.rect.y/256f,
                    Is.EqualTo((3-row)/4f).Within(.0001f));
            }
            Assert.That(Chapter1RegularEnemyArt.SpriteAt(-1,0),Is.Null);
            Assert.That(Chapter1RegularEnemyArt.SpriteAt(4,0),Is.Null);
            Assert.That(Chapter1RegularEnemyArt.SpriteAt(0,8),Is.Null);
        }

        [Test] public void CompletedBehaviorPhasesSelectOnlyTheirVisualKeys()
        {
            Assert.That(Chapter1RegularEnemyArt.SelectWalkerKey(
                WalkerBehaviorPhase.Approach,RegularEnemyMovementIntent.None,10),
                Is.EqualTo(0));
            Assert.That(Chapter1RegularEnemyArt.SelectWalkerKey(
                WalkerBehaviorPhase.Approach,RegularEnemyMovementIntent.Horizontal,0),
                Is.EqualTo(1));
            Assert.That(Chapter1RegularEnemyArt.SelectWalkerKey(
                WalkerBehaviorPhase.Approach,RegularEnemyMovementIntent.Horizontal,6),
                Is.EqualTo(2));
            Assert.That(Chapter1RegularEnemyArt.SelectWalkerKey(
                WalkerBehaviorPhase.DashTelegraph,RegularEnemyMovementIntent.None,12),
                Is.EqualTo(3));
            Assert.That(Chapter1RegularEnemyArt.SelectWalkerKey(
                WalkerBehaviorPhase.DashActive,RegularEnemyMovementIntent.DashHorizontal,12),
                Is.EqualTo(4));
            Assert.That(Chapter1RegularEnemyArt.SelectWalkerKey(
                WalkerBehaviorPhase.Recovery,RegularEnemyMovementIntent.None,12),
                Is.EqualTo(7));
            Assert.That(Chapter1RegularEnemyArt.SelectWalkerKey(
                WalkerBehaviorPhase.Dead,RegularEnemyMovementIntent.None,12),
                Is.EqualTo(6));
            Assert.That(Chapter1RegularEnemyArt.SelectWalkerKey(
                WalkerBehaviorPhase.Reset,RegularEnemyMovementIntent.None,12),
                Is.EqualTo(0));
            Assert.That(Chapter1RegularEnemyArt.SelectSurveyorKey(
                SurveyorBehaviorPhase.Relocate,RegularEnemyMovementIntent.Horizontal,7),
                Is.EqualTo(2));
            Assert.That(Chapter1RegularEnemyArt.SelectSurveyorKey(
                SurveyorBehaviorPhase.Relocate,RegularEnemyMovementIntent.None,7),
                Is.EqualTo(0));
            Assert.That(Chapter1RegularEnemyArt.SelectSurveyorKey(
                SurveyorBehaviorPhase.FireTelegraph,RegularEnemyMovementIntent.None,7),
                Is.EqualTo(3));
            Assert.That(Chapter1RegularEnemyArt.SelectSurveyorKey(
                SurveyorBehaviorPhase.Fire,RegularEnemyMovementIntent.None,7),
                Is.EqualTo(4));
            Assert.That(Chapter1RegularEnemyArt.SelectSurveyorKey(
                SurveyorBehaviorPhase.Cooldown,RegularEnemyMovementIntent.None,7),
                Is.EqualTo(7));
            Assert.That(Chapter1RegularEnemyArt.SelectSurveyorKey(
                SurveyorBehaviorPhase.Suppressed,RegularEnemyMovementIntent.None,7),
                Is.EqualTo(5));
            Assert.That(Chapter1RegularEnemyArt.SelectSurveyorKey(
                SurveyorBehaviorPhase.Dead,RegularEnemyMovementIntent.None,7),
                Is.EqualTo(6));
        }

        [Test] public void PublishedEnemySnapshotSelectsFacingAndAttackWithoutMutation()
        {
            var walkerObject=new GameObject("Walker");
            var surveyorObject=new GameObject("Surveyor");
            try
            {
                var w=walkerObject.AddComponent<BoxCollider2D>();
                var s=surveyorObject.AddComponent<BoxCollider2D>();
                var tick=new SimulationTick(55);
                var walker=new WalkerBehaviorSnapshot(
                    WalkerBehaviorPhase.DashActive,58,true,
                    RegularEnemyMovementIntent.DashHorizontal,-1,
                    false,false,-1,1);
                var surveyor=new SurveyorBehaviorSnapshot(
                    SurveyorBehaviorPhase.Fire,58,true,
                    RegularEnemyMovementIntent.None,1,false,true,12,2);
                var pair=new RegularEnemyBehaviorPairSnapshot(
                    tick,walker,surveyor,null,null,new SimulationTick(56));
                var exact=new CarriedBehaviorView(tick,pair);
                var leftMelee=Chapter1RegularEnemyArt.ForEnemy(w,exact);
                var rightRanged=Chapter1RegularEnemyArt.ForEnemy(s,exact);
                Assert.That(leftMelee,Is.SameAs(
                    Chapter1RegularEnemyArt.SpriteAt(1,4)));
                Assert.That(rightRanged,Is.SameAs(
                    Chapter1RegularEnemyArt.SpriteAt(2,4)));
                Assert.That(Chapter1RegularEnemyArt.ForEnemy(w,null),
                    Is.SameAs(Chapter1RegularEnemyArt.SpriteAt(0,0)),
                    "no published evidence means quiet, neutral actor");
                Assert.That(Chapter1RegularEnemyArt.ForEnemy(s,null),
                    Is.SameAs(Chapter1RegularEnemyArt.SpriteAt(2,0)));
                // Visual selector may not modify underlying behavior phase.
                Assert.That(exact.Snapshot.Walker.Phase,
                    Is.EqualTo(WalkerBehaviorPhase.DashActive));
                Assert.That(exact.Snapshot.Surveyor.Phase,
                    Is.EqualTo(SurveyorBehaviorPhase.Fire));
            }
            finally
            {
                Object.DestroyImmediate(walkerObject);
                Object.DestroyImmediate(surveyorObject);
            }
        }
        [Test] public void OnlyExactRegularEnemyCollidersUseNewArt()
        {
            var walker=new GameObject("Walker");
            var surveyor=new GameObject("Surveyor");
            var other=new GameObject("Ordan");
            try
            {
                var w=walker.AddComponent<BoxCollider2D>();
                var s=surveyor.AddComponent<BoxCollider2D>();
                var o=other.AddComponent<BoxCollider2D>();
                Assert.That(Chapter1RegularEnemyArt.IsEnemy(w),Is.True);
                Assert.That(Chapter1RegularEnemyArt.IsEnemy(s),Is.True);
                Assert.That(Chapter1RegularEnemyArt.IsEnemy(o),Is.False);
                Assert.That(Chapter1RegularEnemyArt.ForEnemy(o,null),Is.Null);
                var wIdle=Chapter1RegularEnemyArt.ForEnemy(w,null);
                var sIdle=Chapter1RegularEnemyArt.ForEnemy(s,null);
                Assert.That(wIdle,Is.SameAs(Chapter1RegularEnemyArt.SpriteAt(0,0)));
                Assert.That(sIdle,Is.SameAs(Chapter1RegularEnemyArt.SpriteAt(2,0)));
                Assert.That(wIdle,Is.Not.SameAs(sIdle));
                w.isTrigger=true;
                Assert.That(Chapter1RegularEnemyArt.ForEnemy(w,null),Is.Null);
            }
            finally
            {
                Object.DestroyImmediate(walker);
                Object.DestroyImmediate(surveyor);
                Object.DestroyImmediate(other);
            }
        }
    }
}
#endif