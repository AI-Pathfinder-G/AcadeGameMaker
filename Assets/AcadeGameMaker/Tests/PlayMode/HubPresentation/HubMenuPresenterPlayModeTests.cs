using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.HubPresentation
{
    // REQ-M5D7QA-002/003/008; AC-M5D7QA-002/008/010: authored presenter assets stay concrete at runtime.
    internal sealed class HubMenuPresenterPlayModeTests
    {
        [Test] public void AC_M5D7QA_008_AllAuthoredTmpTextUsesStaticCanonicalFont()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hub/HubMenuRoot.prefab");
            Assert.That(prefab, Is.Not.Null);
            foreach (var text in prefab.GetComponentsInChildren<TMP_Text>(true))
            {
                Assert.That(text.font, Is.Not.Null); Assert.That(text.font.atlasPopulationMode, Is.EqualTo(AtlasPopulationMode.Static));
                Assert.That(text.font.atlasTextures, Has.Length.EqualTo(1)); Assert.That(text.font.atlasTextures[0], Is.Not.Null);
            }
        }
    }
}
