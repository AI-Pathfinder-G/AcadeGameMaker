using System;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Hub.Presentation.Unity;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AcadeGameMaker.Combat.Authoring
{
    // REQ-LPS-001/010, AC-LPS-001/007: original structural authority plus local-only bindings.
    internal static class LocalPlayableSliceAuthoringValidator
    {
        internal static void ValidateScene(Scene scene)
        {
            if(!scene.IsValid() || !scene.isLoaded || scene.path!=LocalPlayableSliceAuthoringBuilder.ScenePath)
                throw new InvalidOperationException("Local scene path is not exact.");
            var roots=scene.GetRootGameObjects();
            if(roots.Length!=2)throw new InvalidOperationException("Local scene needs exactly graph and presentation roots.");
            GameObject graph=null,presentation=null;
            foreach(var root in roots)
                if(root.name==OrdanBossTerminalTransitionRequester.EncounterKey)graph=root;
                else if(root.name=="LocalPlayablePresentation")presentation=root;
            if(graph==null || presentation==null || graph.transform.childCount!=9)
                throw new InvalidOperationException("Local root topology mismatch.");
            OrdanBossEncounterAuthoringValidator.Validate(graph,false);
            if(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(graph)!=LocalPlayableSliceAuthoringBuilder.GraphPath)
                throw new InvalidOperationException("Local graph prefab is not exact.");
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(LocalPlayableSliceAuthoringBuilder.GraphPath);
            var regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(LocalPlayableSliceAuthoringBuilder.RegularPath);
            var bold=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(LocalPlayableSliceAuthoringBuilder.BoldPath);
            var presenter=presentation.GetComponent<LocalPlayableSlicePresenter>();
            if(presenter==null || graph.GetComponentsInChildren<UnityEngine.Camera>(true).Length!=1 ||
                presentation.GetComponentsInChildren<Canvas>(true).Length!=1 || presentation.GetComponentsInChildren<TMP_Text>(true).Length!=4)
                throw new InvalidOperationException("Local camera or UI topology mismatch.");
            var regularSource=AssetDatabase.LoadAssetAtPath<Font>(LocalPlayableSliceAuthoringBuilder.RegularSourcePath);
            var boldSource=AssetDatabase.LoadAssetAtPath<Font>(LocalPlayableSliceAuthoringBuilder.BoldSourcePath);
            var shader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader");
            if(regularSource==null || boldSource==null || shader==null || shader.name!="TextMeshPro/Mobile/Distance Field" ||
                regular.material==null || bold.material==null || regular.material.shader!=shader || bold.material.shader!=shader)
                throw new InvalidOperationException("Explicit approved Font/shader inputs are not exact.");
            foreach(var path in new[]{LocalPlayableSliceAuthoringBuilder.RegularSourcePath,LocalPlayableSliceAuthoringBuilder.BoldSourcePath})
            {
                var importer=AssetImporter.GetAtPath(path) as TrueTypeFontImporter;
                if(importer==null || !importer.includeFontData)throw new InvalidOperationException("Included Font data is required.");
            }
            presenter.ValidateForAuthoring(prefab,graph,regular,bold,regularSource,boldSource);
            var scenes=EditorBuildSettings.scenes;
            if(scenes.Length!=1 || !scenes[0].enabled || scenes[0].path!=LocalPlayableSliceAuthoringBuilder.ScenePath)
                throw new InvalidOperationException("Only the exact local start scene may be enabled.");
        }
    }
}
