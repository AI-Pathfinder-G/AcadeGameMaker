using System;
using System.Collections;
using System.Linq;
using AcadeGameMaker.Hub.Presentation.Unity;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.HubPresentation
{
    // REQ-M5D7QA-006; AC-M5D7QA-006/010: safe frame remains authored for the fixed baseline.
    internal sealed class HubMenuResolutionPlayModeTests
    {
        [Test] public void AC_M5D7QA_006_MenuSafeFrameIs640By360()
        {
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hub/HubMenuRoot.prefab");
            var safe=prefab.transform.Find("SafeFrame") as RectTransform;
            Assert.That(safe, Is.Not.Null); Assert.That(safe.sizeDelta, Is.EqualTo(new Vector2(640,360)));
        }

        // REQ-M5D7QA-006/009; AC-M5D7QA-007/010. Exercise the actual prefab
        // and its Hub scene instance, including TMP's local mesh predicate.
        [UnityTest] public IEnumerator AC_M5D7QA_007_And_010_MenuLabelsAreCenteredAndOverflowFreeInPrefabAndScene()
        {
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hub/HubMenuRoot.prefab"); var clone=UnityEngine.Object.Instantiate(prefab);
            try { clone.SetActive(true); Canvas.ForceUpdateCanvases(); AssertLabels(clone); }
            finally { UnityEngine.Object.DestroyImmediate(clone); }
            var load=EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/Scenes/Hub.unity",new LoadSceneParameters(LoadSceneMode.Additive)); Assert.That(load,Is.Not.Null,"Hub scene must remain PlayMode-loadable without editing build settings."); yield return load;
            var scene=SceneManager.GetSceneByPath("Assets/Scenes/Hub.unity"); if(!scene.IsValid() || !scene.isLoaded) Assert.Fail("Hub scene did not load through SceneManager.");
            Exception failure=null;
            try { var roots=scene.GetRootGameObjects(); Assert.That(scene.path,Is.EqualTo("Assets/Scenes/Hub.unity")); Assert.That(roots.Length,Is.EqualTo(2)); Assert.That(roots.Count(x=>x.name=="HubRuntimeRoot"),Is.EqualTo(1)); Assert.That(roots.Count(x=>x.name=="HubMenuRoot"),Is.EqualTo(1)); Assert.That(roots.All(x=>x.activeSelf),Is.True); var root=roots.Single(x=>x.name=="HubMenuRoot"); Assert.That(root.GetComponent<Canvas>(),Is.Not.Null); Assert.That(root.GetComponent<HubSafeFrameScalerV1>(),Is.Not.Null); var presenter=root.GetComponent<HubMenuPresenterV1>(); var owners=root.GetComponents<HubMenuIntentHandoffOwnerV1>(); Assert.That(presenter,Is.Not.Null); Assert.That(owners,Has.Length.EqualTo(1)); Assert.That(object.ReferenceEquals(presenter.HandoffOwner,owners[0]),Is.True); Assert.That(object.ReferenceEquals(owners[0].Presenter,presenter),Is.True); Assert.That(root.GetComponentsInChildren<EventSystem>(true).Length,Is.EqualTo(1)); Assert.That(roots.SelectMany(x=>x.GetComponentsInChildren<Camera>(true)).Any(),Is.False); Assert.That(roots.SelectMany(x=>x.GetComponentsInChildren<MonoBehaviour>(true)).Any(x=>x==null),Is.False); Canvas.ForceUpdateCanvases(); AssertLabels(root); }
            catch(Exception error) { failure=error; }
            var unload=SceneManager.UnloadSceneAsync(scene); Assert.That(unload,Is.Not.Null,"Hub scene must unload after the actual-scene assertion."); yield return unload;
            if(failure!=null) throw failure;
        }

        static void AssertLabels(GameObject root)
        {
            var names=new[]{"Continue","NewGame","Settings","Quit"}; var copy=new[]{"계속하기","새 게임","설정","종료"};
            for(var i=0;i<names.Length;i++)
            {
                var rect=root.transform.Find("SafeFrame/MenuPanel/"+names[i]+"/Label") as RectTransform; var text=rect.GetComponent<TMP_Text>();
                Assert.That(rect.anchoredPosition,Is.EqualTo(new Vector2(12,5))); Assert.That(rect.sizeDelta,Is.EqualTo(new Vector2(168,22))); Assert.That(rect.anchoredPosition+rect.sizeDelta*.5f,Is.EqualTo(new Vector2(96,16))); Assert.That(text.text,Is.EqualTo(copy[i])); Assert.That(text.margin,Is.EqualTo(Vector4.zero));
                text.ForceMeshUpdate(true,true); Assert.That(text.isTextOverflowing,Is.False); Assert.That(text.firstOverflowCharacterIndex,Is.EqualTo(-1)); Assert.That(text.textInfo.characterCount,Is.EqualTo(text.text.Length));
                var local=rect.rect; foreach(var character in text.textInfo.characterInfo){if(!character.isVisible) continue; var vertices=text.textInfo.meshInfo[character.materialReferenceIndex].vertices; for(var v=0;v<4;v++){var point=vertices[character.vertexIndex+v]; Assert.That(point.x,Is.InRange(local.xMin,local.xMax)); Assert.That(point.y,Is.InRange(local.yMin,local.yMax));}}
            }
        }
    }
}
