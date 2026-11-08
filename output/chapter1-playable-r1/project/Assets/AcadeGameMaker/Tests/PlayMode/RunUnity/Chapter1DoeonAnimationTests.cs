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