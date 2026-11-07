using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using AcadeGameMaker.Hub.Presentation.Unity;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AcadeGameMaker.Hub.Authoring.Editor
{
    // REQ-M5D7QA-001/007/009: strictly observational authoring audit.
    public static class HubPresentationAuthoringValidator
    {
        private const string ShaderPath = "Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader";
        private const string IncludePath = "Assets/UI/Shaders/Hub/TMPro_Properties.cginc";
        private const string LicensePath = "third_party/unity-ugui-2.6.0-LICENSE.md";
        private const string ShaderName = "TextMeshPro/Mobile/Distance Field";

        // REQ-M5D7QA-007/009 and AC-M5D7QA-002/008: source-controlled package
        // precondition. This editor schema read is observational; neither the
        // builder nor validator creates or mutates TMP Settings.
        public static void ValidateTmpSettingsAsset()
        {
            const string path = "Assets/UI/Resources/TMP Settings.asset";
            var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(path);
            if (settings == null || AssetDatabase.LoadAllAssetsAtPath(path).Length != 1 || Resources.Load<TMP_Settings>("TMP Settings") != settings)
                throw new InvalidOperationException("Canonical TMP Settings resource is missing or duplicated.");
            var serialized = new SerializedObject(settings);
            RequireString(serialized, "assetVersion", "2");
            RequireBool(serialized, "m_ClearDynamicDataOnBuild", true);
            RequireBool(serialized, "m_GetFontFeaturesAtRuntime", false);
            RequireBool(serialized, "m_UseModernHangulLineBreakingRules", true);
            RequireEmptyArray(serialized, "m_ActiveFontFeatures");
            RequireEmptyArray(serialized, "m_fallbackFontAssets");
            RequireEmptyArray(serialized, "m_EmojiFallbackTextAssets");
            RequireNull(serialized, "m_defaultFontAsset"); RequireNull(serialized, "m_defaultSpriteAsset"); RequireNull(serialized, "m_defaultStyleSheet");
            RequireNull(serialized, "m_leadingCharacters"); RequireNull(serialized, "m_followingCharacters");
            RequireEmptyString(serialized, "m_defaultFontAssetPath"); RequireEmptyString(serialized, "m_defaultSpriteAssetPath");
            RequireEmptyString(serialized, "m_defaultColorGradientPresetsPath"); RequireEmptyString(serialized, "m_StyleSheetsResourcePath");
            if (TMP_Settings.clearDynamicDataOnBuild != true || TMP_Settings.getFontFeaturesAtRuntime || !TMP_Settings.useModernHangulLineBreakingRules || TMP_Settings.fontFeatures == null || TMP_Settings.fontFeatures.Count != 0)
                throw new InvalidOperationException("Canonical TMP Settings runtime projection is malformed.");
        }

        // REQ-M5D7QA-007/009 and AC-M5D7QA-002/008: source-controlled,
        // byte-locked Mobile SDF prerequisite. This observes only the admitted
        // project files and never imports the Essential Resources package.
        public static void ValidateShaderPrerequisites()
        {
            RequireHash("Assets/UI/Shaders.meta", "142F6FE56DA2CF181D4E55B0C86F0A10ECFD0A1CE266BFAAF1FCFC0ABB5C9BC8");
            RequireHash("Assets/UI/Shaders/Hub.meta", "0441D34AA04ED1622960D9B115A249E452A1E75B97DB0E1DCF8D8A861EAAE5A9");
            RequireHash(ShaderPath, "44C39AABC7E88E7E1FEFFC1880DF754F4ADF86B62E9512AA89E9D2D65171AFF4");
            RequireHash(ShaderPath + ".meta", "94D3127E88D565DE1A4D71F6FB8F4E08A2CAEAC3D2FD6D8BE0277C033E9BF91C");
            RequireHash(IncludePath, "66DB1F03E8D7A413EBA79BCB6602FDB2DE710B586F34A6F2584ED6F68F028E90");
            RequireHash(IncludePath + ".meta", "E8B9FD72E432CE711D125188E548A461263E1A74FE673A01D736984BCFCCDAF8");
            RequireHash(LicensePath, "3F8833F9736C0B5DB5076663BA5ABB0A33606712FA9128456AC834FC9D43FDD6");
            if (AssetDatabase.AssetPathToGUID(ShaderPath) != "5d56ff4c414a9d872126e38007c1d57d" || AssetDatabase.AssetPathToGUID(IncludePath) != "f0cdd71d926cc071f345d0b51adef566")
                throw new InvalidOperationException("Canonical Mobile SDF asset identity is malformed.");
            var shader = CanonicalMobileSdfShader();
            if (shader == null || shader.name != ShaderName || !ReferenceEquals(Shader.Find(ShaderName), shader))
                throw new InvalidOperationException("Canonical Mobile SDF shader resolution is malformed.");
            var allNamed = AssetDatabase.FindAssets("t:Shader").Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Shader>).Count(candidate => candidate != null && candidate.name == ShaderName);
            if (allNamed != 1 || AssetDatabase.IsValidFolder("Assets/TextMesh Pro"))
                throw new InvalidOperationException("TMP Essential Resources boundary is malformed.");
            var source = File.ReadAllText(ShaderPath);
            if (!source.Contains("Shader \"TextMeshPro/Mobile/Distance Field\"") || !source.Contains("#include \"TMPro_Properties.cginc\"") || source.Contains("#include \"TMPro_Properties.cginc\"\n\t\t#include \"TMPro_Properties.cginc\""))
                throw new InvalidOperationException("Canonical Mobile SDF shader surface is malformed.");
        }

        public static Shader CanonicalMobileSdfShader()
        {
            return AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);
        }

        private static void RequireHash(string path, string expected)
        {
            if (!File.Exists(path)) throw new InvalidOperationException("Required Mobile SDF prerequisite is missing: " + path);
            using (var algorithm = SHA256.Create())
            using (var stream = File.OpenRead(path))
            {
                var actual = BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", string.Empty);
                if (actual != expected) throw new InvalidOperationException("Required Mobile SDF prerequisite hash is malformed: " + path);
            }
        }
        private static void RequireString(SerializedObject value, string field, string expected) { var p=value.FindProperty(field); if(p==null || p.stringValue!=expected) throw new InvalidOperationException("TMP Settings " + field + " is malformed."); }
        private static void RequireEmptyString(SerializedObject value, string field) { RequireString(value,field,string.Empty); }
        private static void RequireBool(SerializedObject value, string field, bool expected) { var p=value.FindProperty(field); if(p==null || p.boolValue!=expected) throw new InvalidOperationException("TMP Settings " + field + " is malformed."); }
        private static void RequireNull(SerializedObject value, string field) { var p=value.FindProperty(field); if(p==null || p.objectReferenceValue!=null) throw new InvalidOperationException("TMP Settings " + field + " is malformed."); }
        private static void RequireEmptyArray(SerializedObject value, string field) { var p=value.FindProperty(field); if(p==null || !p.isArray || p.arraySize!=0) throw new InvalidOperationException("TMP Settings " + field + " is malformed."); }
        public static void ValidateMenuAsset(GameObject root)
        {
            ValidateMenuSurface(root); ValidateMenuLabels(root,false);
        }
        internal static void ValidateLegacyMenuAssetForMigration(GameObject root)
        {
            ValidateMenuSurface(root); ValidateMenuLabels(root,true);
        }
        internal static void ValidateLegacyMenuContentsForMigration(GameObject root)
        {
            ValidateMenuSurface(root,false); ValidateMenuLabels(root,true);
        }
        internal static void ValidateMenuContents(GameObject root)
        {
            ValidateMenuSurface(root,false); ValidateMenuLabels(root,false);
        }
        public static void ValidateMenuLabelProfile(GameObject root)
        {
            ValidateMenuLabels(root,false);
        }
        public static void ValidateLegacyMenuLabelProfile(GameObject root)
        {
            ValidateMenuLabels(root,true);
        }
        private static void ValidateMenuSurface(GameObject root,bool requireAssetPath=true)
        {
            if (root == null || (requireAssetPath && AssetDatabase.GetAssetPath(root) != HubPresentationAuthoringBuilder.MenuPrefabPath) || root.name != "HubMenuRoot") throw new InvalidOperationException("Hub menu prefab path is malformed.");
            var presenter=root.GetComponent<HubMenuPresenterV1>(); var owner=root.GetComponent<HubMenuIntentHandoffOwnerV1>();
            if (root.GetComponent<Canvas>() == null || root.GetComponent<Canvas>().renderMode != RenderMode.ScreenSpaceOverlay || root.GetComponent<CanvasScaler>() == null || root.GetComponent<HubSafeFrameScalerV1>() == null || presenter == null || owner == null || root.GetComponents<HubMenuIntentHandoffOwnerV1>().Length != 1 || !ReferenceEquals(presenter.HandoffOwner,owner) || !ReferenceEquals(owner.Presenter,presenter)) throw new InvalidOperationException("Hub menu root matrix is malformed.");
            var systems=root.GetComponentsInChildren<EventSystem>(true); if(systems.Length!=1 || systems[0].GetComponent<BaseInputModule>()!=null) throw new InvalidOperationException("Hub EventSystem is malformed.");
            var buttons=root.GetComponentsInChildren<HubMenuButtonViewV1>(true); if(buttons.Length!=4) throw new InvalidOperationException("Hub menu button matrix is malformed.");
            foreach(var b in buttons) if(b.navigation.mode!=Navigation.Mode.None || b.GetComponent<Button>()!=null) throw new InvalidOperationException("Hub menu view must remain inert.");
            var texts=root.GetComponentsInChildren<TMP_Text>(true); if(texts.Length<6) throw new InvalidOperationException("Hub TMP matrix is malformed.");
            foreach(var t in texts) if(t.font==null || t.font.atlasPopulationMode!=AtlasPopulationMode.Static) throw new InvalidOperationException("Hub static TMP font matrix is malformed.");
            var backdrop=root.transform.Find("OutputBackdrop"); if(backdrop==null || backdrop.GetComponent<Image>()==null || backdrop.GetComponent<Image>().raycastTarget || backdrop.GetComponent<Selectable>()!=null || backdrop.GetComponent<CanvasGroup>()!=null) throw new InvalidOperationException("Hub backdrop is malformed.");
        }
        private static void ValidateMenuLabels(GameObject root,bool legacy)
        {
            var regular=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(HubPresentationAuthoringBuilder.RegularSdfPath); var names=new[]{"Continue","NewGame","Settings","Quit"}; var ys=new[]{146,106,66,26}; var copies=new[]{HubMenuCopyV1.Continue,HubMenuCopyV1.NewGame,HubMenuCopyV1.Settings,HubMenuCopyV1.Quit};
            if(root==null || root.GetComponentsInChildren<TMP_Text>(true).Count(x=>x.name=="Label")!=4) throw new InvalidOperationException("Hub menu label matrix is malformed.");
            var expectedPosition=new Vector2(12,legacy?6:5); var expectedSize=new Vector2(168,legacy?20:22);
            for(var i=0;i<names.Length;i++)
            {
                var path="SafeFrame/MenuPanel/"+names[i]+"/Label"; var node=root.transform.Find(path); var rect=node as RectTransform; var text=node==null?null:node.GetComponent<TMP_Text>(); var button=rect==null?null:rect.parent as RectTransform;
                if(rect==null || text==null || button==null || rect.anchorMin!=Vector2.zero || rect.anchorMax!=Vector2.zero || rect.pivot!=Vector2.zero || rect.anchoredPosition!=expectedPosition || rect.sizeDelta!=expectedSize || rect.localScale!=Vector3.one || rect.localRotation!=Quaternion.identity || text.margin!=Vector4.zero || text.alignment!=TextAlignmentOptions.TopLeft || text.text!=copies[i] || text.font!=regular || text.fontSize!=14 || text.textWrappingMode!=TextWrappingModes.Normal || text.overflowMode!=TextOverflowModes.Overflow || text.fontSharedMaterial!=regular.material || button.anchorMin!=Vector2.zero || button.anchorMax!=Vector2.zero || button.pivot!=Vector2.zero || button.anchoredPosition!=new Vector2(16,ys[i]) || button.sizeDelta!=new Vector2(192,32) || button.rect.size!=new Vector2(192,32) || rect.anchoredPosition+rect.sizeDelta*.5f!=new Vector2(96,16) || rect.anchoredPosition.x<0 || rect.anchoredPosition.y<0 || rect.anchoredPosition.x+rect.sizeDelta.x>button.rect.width || rect.anchoredPosition.y+rect.sizeDelta.y>button.rect.height) throw new InvalidOperationException((legacy?"Legacy":"Canonical")+" menu label profile is malformed: "+path);
            }
        }
        // REQ-M5D7QA-009; AC-M5D7QA-002/010. The Hub scene has exactly one
        // admitted cross-prefab seam: the menu presenter points at the Q0
        // scene-instance router/latch. Unity also serializes the exact root
        // transform/name defaults below. Nothing else is an allowed override.
        public static void ValidateSceneInstanceOverrideMatrix(GameObject runtime,GameObject menu)
        {
            if(runtime==null || menu==null || PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(runtime)!=HubRuntimeAuthoringBuilder.PrefabPath || PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(menu)!=HubPresentationAuthoringBuilder.MenuPrefabPath) throw new InvalidOperationException("Hub scene prefab instance paths are malformed.");
            var router=runtime.GetComponent<AcadeGameMaker.Input.Unity.InputRouter>(); var latch=runtime.GetComponent<AcadeGameMaker.Input.Unity.HubEntryHandoffLatchV1>(); var presenter=menu.GetComponent<HubMenuPresenterV1>(); var rect=menu.GetComponent<RectTransform>();
            var runtimeTransform=runtime.GetComponent<Transform>(); var presenterTarget=presenter==null?null:PrefabUtility.GetCorrespondingObjectFromSource(presenter); var rectTarget=rect==null?null:PrefabUtility.GetCorrespondingObjectFromSource(rect); var menuTarget=PrefabUtility.GetCorrespondingObjectFromSource(menu); var runtimeTarget=PrefabUtility.GetCorrespondingObjectFromSource(runtime); var runtimeTransformTarget=runtimeTransform==null?null:PrefabUtility.GetCorrespondingObjectFromSource(runtimeTransform);
            if(router==null || latch==null || presenter==null || rect==null || runtimeTransform==null || presenterTarget==null || rectTarget==null || menuTarget==null || runtimeTarget==null || runtimeTransformTarget==null) throw new InvalidOperationException("Hub scene override targets are malformed.");
            var q0Expected=new List<ExpectedSceneModification>{new ExpectedSceneModification(runtimeTarget,"m_Name","HubRuntimeRoot",null),new ExpectedSceneModification(runtimeTransformTarget,"m_LocalRotation.w","1",null)};
            foreach(var path in Q0TransformZeroPaths) q0Expected.Add(new ExpectedSceneModification(runtimeTransformTarget,path,"0",null));
            ValidateExactModificationMatrix(PrefabUtility.GetPropertyModifications(runtime)??Array.Empty<PropertyModification>(),q0Expected,"Hub Q0 scene override matrix is malformed.");
            var expected=new List<ExpectedSceneModification>
            {
                new ExpectedSceneModification(presenterTarget,"_latch",string.Empty,latch),new ExpectedSceneModification(presenterTarget,"_router",string.Empty,router),
                new ExpectedSceneModification(menuTarget,"m_Name","HubMenuRoot",null),new ExpectedSceneModification(rectTarget,"m_LocalRotation.w","1",null)
            };
            foreach(var path in RootRectZeroPaths) expected.Add(new ExpectedSceneModification(rectTarget,path,"0",null));
            ValidateExactModificationMatrix(PrefabUtility.GetPropertyModifications(menu)??Array.Empty<PropertyModification>(),expected,"Hub menu scene override matrix is malformed.");
        }
        private static readonly string[] Q0TransformZeroPaths={"m_LocalPosition.x","m_LocalPosition.y","m_LocalPosition.z","m_LocalRotation.x","m_LocalRotation.y","m_LocalRotation.z","m_LocalEulerAnglesHint.x","m_LocalEulerAnglesHint.y","m_LocalEulerAnglesHint.z"};
        private static readonly string[] RootRectZeroPaths={"m_Pivot.x","m_Pivot.y","m_AnchorMax.x","m_AnchorMax.y","m_AnchorMin.x","m_AnchorMin.y","m_SizeDelta.x","m_SizeDelta.y","m_LocalPosition.x","m_LocalPosition.y","m_LocalPosition.z","m_LocalRotation.x","m_LocalRotation.y","m_LocalRotation.z","m_AnchoredPosition.x","m_AnchoredPosition.y","m_LocalEulerAnglesHint.x","m_LocalEulerAnglesHint.y","m_LocalEulerAnglesHint.z"};
        private static void ValidateExactModificationMatrix(PropertyModification[] actual,List<ExpectedSceneModification> expected,string failure)
        {
            if(actual.Length!=expected.Count) throw new InvalidOperationException(failure);
            foreach(var required in expected) if(actual.Count(candidate=>required.Matches(candidate))!=1) throw new InvalidOperationException(failure);
        }
        private sealed class ExpectedSceneModification
        {
            readonly UnityEngine.Object target,reference; readonly string path,value;
            internal ExpectedSceneModification(UnityEngine.Object target,string path,string value,UnityEngine.Object reference){this.target=target;this.path=path;this.value=value;this.reference=reference;}
            internal bool Matches(PropertyModification candidate){return candidate!=null && object.ReferenceEquals(candidate.target,target) && candidate.propertyPath==path && candidate.value==value && object.ReferenceEquals(candidate.objectReference,reference);}
        }
        public static void ValidateSceneAsset(string path)
        {
            var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive); try { var roots=scene.GetRootGameObjects(); var runtime=Array.Find(roots,x=>x.name=="HubRuntimeRoot"); var menu=Array.Find(roots,x=>x.name=="HubMenuRoot"); if(roots.Length!=2 || runtime==null || menu==null) throw new InvalidOperationException("Hub scene must contain exactly two roots."); ValidateSceneInstanceOverrideMatrix(runtime,menu); ValidateMenuLabelProfile(menu); if(Array.Exists(roots,x=>x.GetComponentInChildren<Camera>(true)!=null)) throw new InvalidOperationException("Hub scene must not contain a camera."); } finally { EditorSceneManager.CloseScene(scene,true); }
        }
    }
}
