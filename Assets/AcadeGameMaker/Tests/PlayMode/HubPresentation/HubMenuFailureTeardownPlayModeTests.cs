using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.HubPresentation
{
    // REQ-M5D7QA-007/009; AC-M5D7QA-007/009/010: authored scene has the bounded runtime roots.
    internal sealed class HubMenuFailureTeardownPlayModeTests
    {
        [Test] public void AC_M5D7QA_009_HubSceneAndBoundedRuntimePrefabsExist()
        {
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Hub.unity"), Is.Not.Null);
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hub/HubMenuRoot.prefab"), Is.Not.Null);
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hub/HubRuntimeRoot.prefab"), Is.Not.Null);
        }
    }
}
