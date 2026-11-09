using AcadeGameMaker.Run.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.RunUnity
{
    internal sealed class Chapter1DoeonAnimationTests
    {
        [Test] public void ReadOnlyVisualStateSelectionDoesNotModifyGameplay()
        {
            Assert.That(Chapter1DoeonAnimation.SelectVisualClip(0,0,true,false,40,40),Is.EqualTo(1),"idle");
            Assert.That(Chapter1DoeonAnimation.SelectVisualClip(2*4096,0,true,false,40,40),Is.EqualTo(2),"walk");
            Assert.That(Chapter1DoeonAnimation.SelectVisualClip(8*4096,0,true,false,40,40),Is.EqualTo(3),"run");
            Assert.That(Chapter1DoeonAnimation.SelectVisualClip(4*4096,12*4096,false,false,40,40),Is.EqualTo(4),"jump");
            Assert.That(Chapter1DoeonAnimation.SelectVisualClip(0,0,true,true,40,40),Is.EqualTo(5),"dash");
            Assert.That(Chapter1DoeonAnimation.SelectVisualClip(8*4096,0,true,true,4,40),Is.EqualTo(6),"attack over dash");
            Assert.That(Chapter1DoeonAnimation.SelectVisualClip(0,0,false,true,0,3),Is.EqualTo(7),"hurt over attack");
            Assert.That(Chapter1DoeonAnimation.SelectVisualClip(0,0,true,false,21,12),Is.EqualTo(1),"expired effects release");
        }

        [Test] public void LeftFacingStillUsesIndependentlyAuthoredStaticMaster()
        {
            Assert.That(Resources.Load<Texture2D>("Chapter1Doeon/Candidate-left"),
                Is.Null, "Old rejected left asset must not enter runtime or override the actual left master");
            Assert.That(Resources.Load<Texture2D>("Chapter1Doeon/Left"), Is.Not.Null);
            var go=new GameObject("Player");
            try
            {
                var source=go.AddComponent<CapsuleCollider2D>();
                var authored=Chapter1DoeonArt.ForPlayer(source,true);
                var selected=Chapter1DoeonAnimation.ForPlayerPose(source,true,null);
                Assert.That(selected,Is.SameAs(authored));
                Assert.That(selected.texture,Is.SameAs(
                    Resources.Load<Texture2D>("Chapter1Doeon/Left")));
            }
            finally { Object.DestroyImmediate(go); }
        }
        [Test] public void ProceduralLeftMotionIsDistinctAndNeverClaimsLeftCombat()
        {
            var authored=Resources.Load<Texture2D>("Chapter1Doeon/Left");
            var provisional=Resources.Load<Texture2D>(
                "Chapter1Doeon/Candidate-left-movement");
            var profile=Resources.Load<TextAsset>(
                "Chapter1Doeon/candidate-animation-profile");
            Assert.That(authored,Is.Not.Null);
            Assert.That(provisional,Is.Not.Null);
            Assert.That(provisional,Is.Not.SameAs(authored));
            Assert.That(profile,Is.Not.Null);
            Assert.That(profile.text,Does.Contain(
                "procedural-experiment-not-hand-drawn"));
            Assert.That(System.Text.RegularExpressions.Regex.IsMatch(
                profile.text, "\"leftCombatAvailable\"\\s*:\\s*false"),Is.True);
            Assert.That(System.Text.RegularExpressions.Regex.IsMatch(
                profile.text, "\"noMirroredRightSprites\"\\s*:\\s*true"),Is.True);
            Assert.That(provisional.width,Is.EqualTo(384));
            Assert.That(provisional.height,Is.EqualTo(256));
            Assert.That(provisional.filterMode,Is.EqualTo(FilterMode.Point));
            Assert.That(provisional.mipmapCount,Is.EqualTo(1));
            Assert.That(Chapter1DoeonAnimation.LeftMotionCandidateAvailable,Is.True);
            // 4 action rows: idle, walk, run/dash, jump. Every row is visible
            // from the same left-authored source, with no transform.flipX.
            foreach(var clip in new[]{1,2,3,4,5})
            {
                var pose=Chapter1DoeonAnimation.LeftMotionFrame(clip,35,4096);
                Assert.That(pose,Is.Not.Null,"clip "+clip);
                Assert.That(pose.texture,Is.SameAs(provisional));
                var uv=Chapter1DoeonAnimation.TextureUv(pose);
                Assert.That(uv.width,Is.EqualTo(1f/6f).Within(.0001f));
                Assert.That(uv.height,Is.EqualTo(.25f).Within(.0001f));
                Assert.That(pose.pivot.y,Is.EqualTo(12f).Within(.001f));
            }
            Assert.That(Chapter1DoeonAnimation.LeftMotionFrame(6,50,0),Is.Null,
                "left attack still uses independently authored static fallback");
            Assert.That(Chapter1DoeonAnimation.LeftMotionFrame(7,50,0),Is.Null,
                "left hit still uses independently authored static fallback");
            Assert.That(Chapter1DoeonAnimation.LeftMotionFrame(2,-1,0),Is.Null);
            Assert.That(Chapter1DoeonAnimation.LeftMotionFrame(123,50,0),Is.Null);
        }
        [Test] public void LeftCombatCandidateSeparatesAttackAndHitFromNoncombat()
        {
            var left=Resources.Load<Texture2D>("Chapter1Doeon/Candidate-left-combat");
            var right=Resources.Load<Texture2D>("Chapter1Doeon/Candidate-combat");
            var profile=Resources.Load<TextAsset>("Chapter1Doeon/candidate-animation-profile");
            Assert.That(left,Is.Not.Null);
            Assert.That(right,Is.Not.Null);
            Assert.That(left,Is.Not.SameAs(right),"never flip source-right equipment");
            Assert.That(left.width,Is.EqualTo(384));
            Assert.That(left.height,Is.EqualTo(256));
            Assert.That(left.mipmapCount,Is.EqualTo(1));
            Assert.That(left.filterMode,Is.EqualTo(FilterMode.Point));
            Assert.That(profile.text,Does.Contain("procedural-experiment-not-hand-drawn"));
            Assert.That(profile.text,Does.Contain("Candidate-left-combat"));
            Assert.That(System.Text.RegularExpressions.Regex.IsMatch(
                profile.text, "\"leftCombatAvailable\"\\s*:\\s*false"),Is.True,
                "final left combat art is not authored or accepted");
            Assert.That(Chapter1DoeonAnimation.LeftCombatCandidateAvailable,Is.True);
            foreach(var age in new[]{0,1,4,8,11})
            {
                var pose=Chapter1DoeonAnimation.LeftCombatFrame(6,age);
                Assert.That(pose,Is.Not.Null,"attack age="+age);
                Assert.That(pose.texture,Is.SameAs(left));
                Assert.That(pose.pivot.y,Is.EqualTo(12f).Within(.001f));
                var uv=Chapter1DoeonAnimation.TextureUv(pose);
                Assert.That(uv.height,Is.EqualTo(.25f).Within(.0001f));
                Assert.That(uv.width,Is.EqualTo(1f/6f).Within(.0001f));
                Assert.That(uv.y,Is.EqualTo(.75f).Within(.0001f),"row0 attack");
            }
            foreach(var age in new[]{0,2,6,9,11})
            {
                var hurt=Chapter1DoeonAnimation.LeftCombatFrame(7,age);
                Assert.That(hurt,Is.Not.Null,"hit age="+age);
                Assert.That(hurt.texture,Is.SameAs(left));
                Assert.That(Chapter1DoeonAnimation.TextureUv(hurt).y,
                    Is.EqualTo(.50f).Within(.0001f),"row1 hit recoil");
            }
            foreach(var clip in new[]{1,2,3,4,5,0,8})
                Assert.That(Chapter1DoeonAnimation.LeftCombatFrame(clip,2),Is.Null);
            Assert.That(Chapter1DoeonAnimation.LeftCombatFrame(6,-1),Is.Null);
            Assert.That(Chapter1DoeonAnimation.LeftCombatFrame(7,-1),Is.Null);
            Assert.That(Chapter1DoeonAnimation.LeftCombatFrame(6,12),Is.Null);
            Assert.That(Chapter1DoeonAnimation.LeftCombatFrame(6,21),Is.Null);
            Assert.That(Chapter1DoeonAnimation.LeftCombatFrame(7,12),Is.Null);
            Assert.That(Chapter1DoeonAnimation.LeftMotionFrame(6,3,0),Is.Null,
                "left combat state never borrows a fake movement sprite");
            Assert.That(Chapter1DoeonAnimation.LeftMotionFrame(7,3,0),Is.Null);
            Assert.That(Resources.Load<Texture2D>("Chapter1Doeon/Left"),Is.Not.Null,
                "the independently authored static fallback must remain");
        }
        [Test] public void AttackAndHitVisualsUseCompletedImpactTick()
        {
            // A queued press can be rejected for cooldown or invalid aim.
            // Only the completed Combat FiredHit/FiredMiss receipt animates.
            Assert.That(Chapter1DoeonAnimation.ShouldStartFiredAttackVisual(
                false,100,99),Is.False,"issued but rejected attack");
            Assert.That(Chapter1DoeonAnimation.ShouldStartFiredAttackVisual(
                true,100,99),Is.True,"accepted fired attack");
            Assert.That(Chapter1DoeonAnimation.ShouldStartFiredAttackVisual(
                true,100,100),Is.False,"duplicate renderer sample same tick");
            Assert.That(Chapter1DoeonAnimation.ShouldStartFiredAttackVisual(
                true,-1,99),Is.False,"invalid incomplete tick");

            // The simulation commits hit/miss immediately, not 4-8 ticks
            // after a new visual wind-up. Key pose 3 is strike/recoil at tick0.
            var strike=new[]{3,3,4,4,4,5,5,5,5,0,0,0};
            var hurt=new[]{3,3,4,4,4,5,5,5,5,0,0,0};
            for(var age=0;age<12;++age)
            {
                Assert.That(Chapter1DoeonAnimation.SelectAttackFrame(age),
                    Is.EqualTo(strike[age]),"strike age="+age);
                Assert.That(Chapter1DoeonAnimation.SelectHitFrame(age),
                    Is.EqualTo(hurt[age]),"hurt age="+age);
            }
            foreach(var age in new[]{-1,12,13,21})
            {
                Assert.That(Chapter1DoeonAnimation.SelectAttackFrame(age),Is.EqualTo(-1));
                Assert.That(Chapter1DoeonAnimation.SelectHitFrame(age),Is.EqualTo(-1));
            }
            Assert.That(Chapter1DoeonAnimation.SelectVisualClip(
                0,0,true,false,0,30),Is.EqualTo(6),"fired attack has priority");
            Assert.That(Chapter1DoeonAnimation.SelectVisualClip(
                0,0,true,false,12,30),Is.EqualTo(1),"attack expires after 12 ticks");
            Assert.That(Chapter1DoeonAnimation.SelectVisualClip(
                0,0,true,false,0,0),Is.EqualTo(7),"damage overrides attack");
        }

        [Test] public void SourceSpecificHitFramesAndImpactTintRemainPresentationOnly()
        {
            // Right-source row3 has independent hurt key poses 4/5,
            // rather than left rig row1 poses 3/4/5/0.
            for (var age=0;age<12;++age)
                Assert.That(Chapter1DoeonAnimation.SelectRightHitFrame(age),
                    Is.EqualTo(age<6?4:5),"right hit age="+age);
            Assert.That(Chapter1DoeonAnimation.SelectRightHitFrame(-1),Is.EqualTo(-1));
            Assert.That(Chapter1DoeonAnimation.SelectRightHitFrame(12),Is.EqualTo(-1));

            var fire=Chapter1DoeonAnimation.VisualTintForAges(0,40);
            Assert.That(fire.r,Is.EqualTo(1f));
            Assert.That(fire.g,Is.EqualTo(.97f).Within(.001f));
            Assert.That(fire.b,Is.EqualTo(.70f).Within(.001f));
            Assert.That(fire.a,Is.EqualTo(1f));
            var damage=Chapter1DoeonAnimation.VisualTintForAges(0,0);
            Assert.That(damage.r,Is.EqualTo(1f));
            Assert.That(damage.g,Is.EqualTo(.63f).Within(.001f));
            Assert.That(damage.b,Is.EqualTo(.60f).Within(.001f));
            Assert.That(damage.a,Is.EqualTo(1f));
            Assert.That(Chapter1DoeonAnimation.VisualTintForAges(4,5),
                Is.EqualTo(Color.white),"flash expires without persistent tint");
        }

        [Test] public void CandidateAtlasesAreExactAndPointSampled()
        {
            var movement=Resources.Load<Texture2D>("Chapter1Doeon/Candidate-movement");
            var combat=Resources.Load<Texture2D>("Chapter1Doeon/Candidate-combat");
            var profile=Resources.Load<TextAsset>("Chapter1Doeon/candidate-animation-profile");
            Assert.That(movement,Is.Not.Null);
            Assert.That(combat,Is.Not.Null);
            Assert.That(profile,Is.Not.Null);
            Assert.That(profile.text,Does.Contain("right-only"));
            Assert.That(profile.text,Does.Contain("cannotMirrorLeft"));
            foreach(var atlas in new[]{movement,combat})
            {
                Assert.That(atlas.width,Is.EqualTo(384));
                Assert.That(atlas.height,Is.EqualTo(256));
                Assert.That(atlas.mipmapCount,Is.EqualTo(1));
                Assert.That(atlas.filterMode,Is.EqualTo(FilterMode.Point));
            }
            Assert.That(Chapter1DoeonAnimation.CandidateAvailable,Is.True);
        }
    }
}