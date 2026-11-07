using System;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Hub.Presentation.Unity;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AcadeGameMaker.Combat.Authoring
{
    // REQ-LPS-001/009/010: bounded authoring; original prefab is read-only.
    internal static class LocalPlayableSliceAuthoringBuilder
    {
        internal const string ScenePath = "Assets/Scenes/LocalPlayableSlice.unity";
        internal const string GraphPath = "Assets/Prefabs/LocalPlayableSlice/LocalPlayableSliceGraph.prefab";
        internal const string RegularPath = "Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset";
        internal const string RegularSourcePath = "Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular.otf";
        internal const string BoldSourcePath = "Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold.otf";
        internal const string BoldPath = "Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold-SDF.asset";
        [MenuItem("AcadeGameMaker/Build Local Playable Slice")]
        public static void Build()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath) != null || System.IO.File.Exists(ScenePath))
                throw new InvalidOperationException("Local slice authoring outputs already exist; inspect before rebuilding.");
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(OrdanBossEncounterAuthoringBuilder.GraphPrefabPath);
            var regular = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(RegularPath);
            var bold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(BoldPath);
            var regularSource = AssetDatabase.LoadAssetAtPath<Font>(RegularSourcePath);
            var boldSource = AssetDatabase.LoadAssetAtPath<Font>(BoldSourcePath);
            if (source == null || regular == null || bold == null || regularSource == null || boldSource == null) throw new InvalidOperationException("Approved graph/font inputs are missing.");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "LocalPlayableSlice";
            var copy = (GameObject)PrefabUtility.InstantiatePrefab(source, scene);
            PrefabUtility.UnpackPrefabInstance(copy, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            copy.name = OrdanBossTerminalTransitionRequester.EncounterKey;
            copy.GetComponentInChildren<InputRouter>(true).ConfigureLocalPlayableForAuthoring(true);
            copy.GetComponentInChildren<OrdanBossTerminalTransitionRequester>(true).ConfigureLocalPlayableForAuthoring(true);
            OrdanBossEncounterAuthoringValidator.Validate(copy, false);
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/LocalPlayableSlice")) AssetDatabase.CreateFolder("Assets/Prefabs", "LocalPlayableSlice");
            var prefab = PrefabUtility.SaveAsPrefabAsset(copy, GraphPath);
            UnityEngine.Object.DestroyImmediate(copy);
            var graph = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            graph.name = OrdanBossTerminalTransitionRequester.EncounterKey;
            var root = new GameObject("LocalPlayablePresentation");
            var presenter = root.AddComponent<LocalPlayableSlicePresenter>();
            var markers = new Transform[9];
            Color[] colors = { new Color(.9f,.9f,.9f), new Color(.7f,.35f,.3f), new Color(.15f,.7f,.9f),
                new Color(.2f,.8f,.9f),new Color(.2f,.8f,.9f),new Color(.2f,.8f,.9f),Color.clear,Color.clear,new Color(.3f,.3f,.3f) };
            for (var i=0; i<9; i++)
            {
                var marker = new GameObject("RoleMarker-"+i); marker.transform.SetParent(root.transform, false);
                marker.AddComponent<SpriteRenderer>().color = colors[i]; markers[i] = marker.transform;
            }
            var canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas));
            canvasObject.transform.SetParent(root.transform, false);
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var safeObject = new GameObject("SafeFrame", typeof(RectTransform)); safeObject.transform.SetParent(canvasObject.transform, false);
            var safe = safeObject.GetComponent<RectTransform>(); safe.anchorMin=safe.anchorMax=new Vector2(.5f,.5f);safe.pivot=new Vector2(.5f,.5f);safe.sizeDelta=new Vector2(640,360);
            root.AddComponent<HubSafeFrameScalerV1>().ConfigureForAuthoring(safe);
            var title = Text(safe,"Title",new Vector2(0,160),new Vector2(620,25),bold,16);
            var hud = Text(safe,"HealthAndTransfer",new Vector2(0,120),new Vector2(620,48),regular,14);
            var help = Text(safe,"Controls",new Vector2(0,-148),new Vector2(620,55),regular,11);
            var result = Text(safe,"Result",new Vector2(0,5),new Vector2(300,65),bold,20);
            var retryObject = new GameObject("RetryHitRect",typeof(RectTransform));retryObject.transform.SetParent(safe,false);
            var retry=retryObject.GetComponent<RectTransform>();retry.anchorMin=retry.anchorMax=new Vector2(.5f,.5f);retry.sizeDelta=new Vector2(280,50);retry.anchoredPosition=new Vector2(0,-15);
            presenter.ConfigureForAuthoring(prefab,graph.GetComponentInChildren<OrdanBossTerminalTransitionRequester>(true),
                graph.GetComponentInChildren<InputRouter>(true),regular,bold,regularSource,boldSource,title,hud,help,result,retry,markers);
            if(!EditorSceneManager.SaveScene(scene,ScenePath))throw new InvalidOperationException("Local scene save failed.");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath,true) };
            LocalPlayableSliceAuthoringValidator.ValidateScene(scene);
            AssetDatabase.SaveAssets();
        }
        private static TMP_Text Text(Transform parent,string name,Vector2 position,Vector2 size,TMP_FontAsset font,float points)
        {
            var value=new GameObject(name,typeof(RectTransform));value.transform.SetParent(parent,false);
            var text=value.AddComponent<TextMeshProUGUI>();text.font=font;text.fontSize=points;text.color=Color.white;
            text.alignment=TextAlignmentOptions.Center;text.raycastTarget=false;text.textWrappingMode=TextWrappingModes.Normal;
            var rect=value.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.sizeDelta=size;rect.anchoredPosition=position;
            return text;
        }
    }
}
