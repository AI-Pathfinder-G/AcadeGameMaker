using AcadeGameMaker.Hub.Presentation.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.HubPresentation
{
    // CH1 boss visual-only candidate, not acceptance of natural boss inputs.
    internal sealed class Chapter1OrdanPhaseArtPlayModeTests
    {
        [Test]
        public void ApprovedPatternSlotsUseOnlyTheAuthoredRightCombatRows()
        {
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(1,0,0),
                Is.EqualTo(7));  // DebtLineA telegraph
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(1,0,8),
                Is.EqualTo(8));
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(1,1,3),
                Is.EqualTo(9));
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(1,1,6),
                Is.EqualTo(10));
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(3,2,4),
                Is.EqualTo(11)); // DebtLineB shares the authored row
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(2,0,0),
                Is.EqualTo(13)); // SeizureWeight telegraph
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(2,1,8),
                Is.EqualTo(16)); // SeizureWeight execute
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(4,1,1),
                Is.EqualTo(21)); // BalanceAudit execute
        }

        [Test]
        public void MissingAndUnpublishedBossStatesCannotInventAnAttack()
        {
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(0,0,0),
                Is.EqualTo(-1));
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(5,0,0),
                Is.EqualTo(-1));
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(1,0,-1),
                Is.EqualTo(-1));
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(1,3,0),
                Is.EqualTo(-1));  // vulnerability: static source fallback
            Assert.That(Chapter1OrdanArt.SelectRightPatternCell(1,4,0),
                Is.EqualTo(-1));  // defeat: no false death clip
        }

        [Test]
        public void RightCandidateImportsWithoutReplacingOriginalLeft()
        {
            var atlas=Resources.Load<Texture2D>(
                "Chapter1Ordan/Candidate-OrdanAttackRight");
            var profile=Resources.Load<TextAsset>(
                "Chapter1Ordan/candidate-right-pattern-profile");
            var originalLeft=Resources.Load<Texture2D>("Chapter1Ordan/Left");
            var originalRight=Resources.Load<Texture2D>("Chapter1Ordan/Right");
            Assert.That(atlas,Is.Not.Null);
            Assert.That(profile,Is.Not.Null);
            Assert.That(atlas.width,Is.EqualTo(768));
            Assert.That(atlas.height,Is.EqualTo(512));
            Assert.That(atlas.filterMode,Is.EqualTo(FilterMode.Point));
            Assert.That(atlas.mipmapCount,Is.EqualTo(1));
            Assert.That(profile.text,Does.Contain(
                "ch1-ordan-right-combat-phase-atlas-v1"));
            Assert.That(profile.text,Does.Contain(
                "provisional-not-manual-approved"));
            Assert.That(profile.text,Does.Contain(
                "leftOriginalStaticUnchanged"));
            Assert.That(originalLeft,Is.Not.Null);
            Assert.That(originalRight,Is.Not.Null);
            Assert.That(originalLeft.width,Is.EqualTo(1215));
            Assert.That(originalLeft.height,Is.EqualTo(1295));
        }

        [Test]
        public void LeftArticulatedCandidateIsNativeLeftAndRetainsStaticSource()
        {
            var atlas=Resources.Load<Texture2D>(
                "Chapter1Ordan/Candidate-OrdanAttackLeft");
            var profile=Resources.Load<TextAsset>(
                "Chapter1Ordan/candidate-left-pattern-profile");
            var rightAtlas=Resources.Load<Texture2D>(
                "Chapter1Ordan/Candidate-OrdanAttackRight");
            var originalLeft=Resources.Load<Texture2D>("Chapter1Ordan/Left");
            Assert.That(atlas,Is.Not.Null);
            Assert.That(rightAtlas,Is.Not.Null);
            Assert.That(profile,Is.Not.Null);
            Assert.That(atlas,Is.Not.SameAs(rightAtlas));
            Assert.That(atlas,Is.Not.SameAs(originalLeft));
            Assert.That(atlas.width,Is.EqualTo(768));
            Assert.That(atlas.height,Is.EqualTo(512));
            Assert.That(atlas.filterMode,Is.EqualTo(FilterMode.Point));
            Assert.That(atlas.mipmapCount,Is.EqualTo(1));
            Assert.That(profile.text,Does.Contain(
                "ch1-ordan-left-rig-from-owned-left-static-v1"));
            Assert.That(profile.text,Does.Contain(
                "provisional-cutout-rig-not-final-hand-authored"));
            Assert.That(profile.text,Does.Contain(
                "rightFramesMirrored"));
            for(var row=0;row<4;++row)
            for(var key=0;key<6;++key)
            {
                var sprite=Chapter1OrdanArt.LeftPhaseSprite(row,key);
                Assert.That(sprite,Is.Not.Null,"row="+row+" key="+key);
                Assert.That(sprite.texture,Is.SameAs(atlas));
                Assert.That(sprite.rect.width,Is.EqualTo(128));
                Assert.That(sprite.rect.height,Is.EqualTo(128));
                Assert.That(sprite.pixelsPerUnit,Is.EqualTo(47.4f).Within(.01f));
                Assert.That(sprite.pivot.x,Is.EqualTo(128*.446502f).Within(.01f));
                Assert.That(sprite.rect.x,Is.EqualTo(key*128));
                Assert.That(sprite.rect.y,Is.EqualTo((3-row)*128));
            }
            Assert.That(Chapter1OrdanArt.LeftPhaseSprite(-1,0),Is.Null);
            Assert.That(Chapter1OrdanArt.LeftPhaseSprite(4,0),Is.Null);
            Assert.That(Chapter1OrdanArt.LeftPhaseSprite(0,6),Is.Null);
            Assert.That(originalLeft.width,Is.EqualTo(1215));
            Assert.That(originalLeft.height,Is.EqualTo(1295));
        }
    }
}
