using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Hub.Presentation.Unity;
using AcadeGameMaker.Input.Unity;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.LowLevel;

namespace AcadeGameMaker.Hub.Authoring.Editor
{
    // REQ-M5D7QA-007/009: bounded deterministic authoring entrypoint.
    public static class HubPresentationAuthoringBuilder
    {
        public enum LabelMigrationFailurePoint { BeforeSave,AfterSave,AfterRefresh,AfterReloadValidation,BeforeSceneValidation,AfterSceneValidation }
        public const string MenuPrefabPath = "Assets/Prefabs/Hub/HubMenuRoot.prefab";
        public const string ScenePath = "Assets/Scenes/Hub.unity";
        internal const string RegularFontPath = "Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular.otf";
        internal const string BoldFontPath = "Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold.otf";
        internal const string RegularSdfPath = "Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset";
        internal const string BoldSdfPath = "Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold-SDF.asset";
        private const string Glyphs = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~×게경계구기니다러로료를만못변보복불사상새설속손습완왔은을일임장저정존종지파프필하항했";
        private static Func<LabelMigrationFailurePoint,Exception> migrationFailureForTests;

        // Test-only deterministic seam; production callers leave this null.
        public static void SetLabelMigrationFailureInjectorForTests(Func<LabelMigrationFailurePoint,Exception> injector) { migrationFailureForTests=injector; }

        public static void Build()
        {
            HubPresentationAuthoringValidator.ValidateTmpSettingsAsset();
            HubPresentationAuthoringValidator.ValidateShaderPrerequisites();
            EnsureFonts();
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(MenuPrefabPath);
            if (existing == null) CreateMenuPrefab(); else MigrateExactLegacyMenuLabelsOrValidate(existing);
            if (!File.Exists(ScenePath)) CreateScene(); else HubPresentationAuthoringValidator.ValidateSceneAsset(ScenePath);
            AssetDatabase.Refresh();
        }
        private static void EnsureFonts()
        {
            if (!File.Exists(RegularFontPath) || !File.Exists(BoldFontPath) || !File.Exists("Assets/UI/Fonts/Hub/OFL-1.1.txt")) throw new InvalidOperationException("Verified font bytes are absent.");
            if (SystemInfo.maxTextureSize < 2048) throw new InvalidOperationException("Static SDF profile requires SystemInfo.maxTextureSize >= 2048.");
            var regularExisting = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(RegularSdfPath);
            var boldExisting = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(BoldSdfPath);
            if (regularExisting != null || boldExisting != null)
            {
                if (regularExisting == null || boldExisting == null) throw new InvalidOperationException("Static SDF pair is partially persisted.");
                ValidatePersistedCandidate(RegularSdfPath, RegularFontPath); ValidatePersistedCandidate(BoldSdfPath, BoldFontPath);
                return;
            }
            AssetDatabase.ImportAsset(RegularFontPath, ImportAssetOptions.ForceSynchronousImport); AssetDatabase.ImportAsset(BoldFontPath, ImportAssetOptions.ForceSynchronousImport);
            TMP_FontAsset regular = null, bold = null;
            var regularSaved = false; var boldSaved = false;
            try
            {
                regular = CreateCandidate(RegularFontPath);
                bold = CreateCandidate(BoldFontPath);
                ValidateCandidate(regular, RegularFontPath, false); ValidateCandidate(bold, BoldFontPath, false);
                regular.atlasPopulationMode = AtlasPopulationMode.Static;
                bold.atlasPopulationMode = AtlasPopulationMode.Static;
                ValidateCandidate(regular, RegularFontPath, true); ValidateCandidate(bold, BoldFontPath, true);
                PersistCandidate(regular, RegularSdfPath); regularSaved = true;
                PersistCandidate(bold, BoldSdfPath); boldSaved = true;
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                ValidatePersistedCandidate(RegularSdfPath, RegularFontPath);
                ValidatePersistedCandidate(BoldSdfPath, BoldFontPath);
            }
            catch
            {
                if (regularSaved) AssetDatabase.DeleteAsset(RegularSdfPath);
                if (boldSaved) AssetDatabase.DeleteAsset(BoldSdfPath);
                throw;
            }
            finally
            {
                if (!regularSaved && regular != null) UnityEngine.Object.DestroyImmediate(regular);
                if (!boldSaved && bold != null) UnityEngine.Object.DestroyImmediate(bold);
            }
        }
        private static TMP_FontAsset CreateCandidate(string fontPath)
        {
            var source = AssetDatabase.LoadAssetAtPath<Font>(fontPath); if (source == null) throw new InvalidOperationException("Font import failed.");
            var asset = TMP_FontAsset.CreateFontAsset(source, 90, 9, GlyphRenderMode.SDFAA, 2048, 2048, AtlasPopulationMode.Dynamic, false);
            if (asset == null) throw new InvalidOperationException("Static SDF asset creation failed.");
            var populated = asset.TryAddCharacters(Glyphs, out string missing);
            if (!populated || !string.IsNullOrEmpty(missing)) throw new InvalidOperationException("Static glyph population failed: " + (missing ?? string.Empty));
            return asset;
        }
        private static void PersistCandidate(TMP_FontAsset asset, string targetPath)
        {
            AssetDatabase.CreateAsset(asset, targetPath);
            AssetDatabase.AddObjectToAsset(asset.atlasTextures[0], asset);
            AssetDatabase.AddObjectToAsset(asset.material, asset);
        }
        private static void ValidatePersistedCandidate(string targetPath, string sourcePath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(targetPath);
            ValidateCandidate(asset, sourcePath, true);
            if (AssetDatabase.LoadAllAssetsAtPath(targetPath).Length != 3)
                throw new InvalidOperationException("Static SDF persistence is malformed.");
        }
        private static void ValidateCandidate(TMP_FontAsset asset, string fontPath, bool expectStatic)
        {
            var texture = asset != null && asset.atlasTextures != null && asset.atlasTextures.Length == 1 ? asset.atlasTextures[0] : null;
            var expectedSource = expectStatic ? HasExpectedSourceGuid(asset, fontPath) : asset != null && asset.sourceFontFile != null && AssetDatabase.GetAssetPath(asset.sourceFontFile) == fontPath;
            if (asset == null || !expectedSource ||
                asset.atlasWidth != 2048 || asset.atlasHeight != 2048 || asset.atlasPadding != 9 ||
                asset.atlasRenderMode != GlyphRenderMode.SDFAA || asset.atlasPopulationMode != (expectStatic ? AtlasPopulationMode.Static : AtlasPopulationMode.Dynamic) || asset.isMultiAtlasTexturesEnabled ||
                texture == null || texture.width != 2048 || texture.height != 2048 ||
                texture.format != TextureFormat.Alpha8 || texture.mipmapCount != 1 ||
                texture.GetRawTextureData().Length != 4194304)
                throw new InvalidOperationException($"Static SDF candidate profile is malformed: sourceGuid={SourceGuid(asset)}; atlas={asset?.atlasWidth}x{asset?.atlasHeight}; padding={asset?.atlasPadding}; render={asset?.atlasRenderMode}; population={asset?.atlasPopulationMode}; multi={asset?.isMultiAtlasTexturesEnabled}; textures={asset?.atlasTextures?.Length}; texture={texture?.width}x{texture?.height}:{texture?.format}:mips={texture?.mipmapCount}:raw={(texture == null ? 0 : texture.GetRawTextureData().Length)}.");
            if (asset.characterTable == null || asset.characterTable.Count != 138 || asset.glyphTable == null || asset.glyphTable.Count != 138)
                throw new InvalidOperationException("Static SDF glyph coverage is malformed.");
            var shader = HubPresentationAuthoringValidator.CanonicalMobileSdfShader();
            if (asset.material == null || !ReferenceEquals(asset.material.shader, shader) || asset.material.GetFloat("_TextureWidth") != 2048 || asset.material.GetFloat("_TextureHeight") != 2048 || asset.material.GetFloat("_GradientScale") != 10)
                throw new InvalidOperationException("Static SDF material profile is malformed.");
        }
        private static bool HasExpectedSourceGuid(TMP_FontAsset asset, string fontPath) => SourceGuid(asset) == AssetDatabase.AssetPathToGUID(fontPath);
        private static string SourceGuid(TMP_FontAsset asset)
        {
            if (asset == null) return string.Empty;
            var property = new SerializedObject(asset).FindProperty("m_SourceFontFileGUID");
            return property == null ? string.Empty : property.stringValue;
        }
        private static void CreateMenuPrefab()
        {
            var regular = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(RegularSdfPath);
            var root = new GameObject("HubMenuRoot", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(HubSafeFrameScalerV1), typeof(HubMenuPresenterV1), typeof(HubMenuIntentHandoffOwnerV1));
            var canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize; scaler.scaleFactor = 1;
            var eventRoot = new GameObject("EventSystem", typeof(EventSystem)); eventRoot.transform.SetParent(root.transform, false);
            var backdrop = ImageChild(root.transform, "OutputBackdrop", Vector2.zero, Vector2.zero, Vector2.one, Vector2.one, new Color32(9,13,18,255)); backdrop.raycastTarget = false;
            var safe = new GameObject("SafeFrame", typeof(RectTransform)); safe.transform.SetParent(root.transform, false); var safeRect=safe.GetComponent<RectTransform>(); safeRect.anchorMin=safeRect.anchorMax=new Vector2(.5f,.5f); safeRect.sizeDelta=new Vector2(640,360);
            var surface = ImageChild(safe.transform, "Surface", new Vector2(.5f,.5f), new Vector2(640,360), new Vector2(.5f,.5f), new Vector2(.5f,.5f), new Color32(20,27,37,255));
            var panel = new GameObject("MenuPanel", typeof(RectTransform)); panel.transform.SetParent(safe.transform,false); Place(panel.GetComponent<RectTransform>(),48,86,224,188);
            var buttons = new HubMenuButtonViewV1[4]; var names=new[]{"Continue","NewGame","Settings","Quit"}; var ys=new[]{146,106,66,26}; var labels=new[]{HubMenuCopyV1.Continue,HubMenuCopyV1.NewGame,HubMenuCopyV1.Settings,HubMenuCopyV1.Quit};
            for(int i=0;i<4;i++) buttons[i]=CreateButton(panel.transform,names[i],16,ys[i],labels[i],regular);
            var notice = new GameObject("Notification", typeof(RectTransform), typeof(Image)); notice.transform.SetParent(safe.transform,false); Place(notice.GetComponent<RectTransform>(),372,260,256,88); notice.GetComponent<Image>().color=new Color32(34,45,58,255);
            var message=TextChild(notice.transform,"Message",12,12,196,64,regular,12,""); var dismiss=TextChild(notice.transform,"Dismiss",220,52,24,24,regular,14,HubMenuCopyV1.Dismiss);
            root.GetComponent<HubSafeFrameScalerV1>().ConfigureForAuthoring(safeRect);
            var presenter=root.GetComponent<HubMenuPresenterV1>(); var owner=root.GetComponent<HubMenuIntentHandoffOwnerV1>();
            presenter.ConfigureForAuthoring(null,null,root.GetComponent<HubSafeFrameScalerV1>(),eventRoot.GetComponent<EventSystem>(),buttons,dismiss.rectTransform,notice,message,dismiss);
            presenter.ConfigureHandoffOwnerForAuthoring(owner); owner.ConfigurePresenterForAuthoring(presenter);
            PrefabUtility.SaveAsPrefabAsset(root,MenuPrefabPath); UnityEngine.Object.DestroyImmediate(root);
        }
        private static HubMenuButtonViewV1 CreateButton(Transform parent,string name,int x,int y,string label,TMP_FontAsset font)
        { var go=new GameObject(name,typeof(RectTransform),typeof(Image),typeof(HubMenuButtonViewV1)); go.transform.SetParent(parent,false); Place(go.GetComponent<RectTransform>(),x,y,192,32); var image=go.GetComponent<Image>(); image.color=new Color32(48,60,74,255); var text=TextChild(go.transform,"Label",12,5,168,22,font,14,label); var rail=ImageChild(go.transform,"FocusRail",new Vector2(0,0),new Vector2(2,32),new Vector2(0,0),new Vector2(0,0),Color.white); var border=ImageChild(go.transform,"Border",new Vector2(.5f,.5f),new Vector2(192,32),new Vector2(.5f,.5f),new Vector2(.5f,.5f),Color.clear); var strike=ImageChild(go.transform,"DisabledStrike",new Vector2(.5f,.5f),new Vector2(184,1),new Vector2(.5f,.5f),new Vector2(.5f,.5f),Color.white); var v=go.GetComponent<HubMenuButtonViewV1>(); v.ConfigureForAuthoring(text,rail,border,strike); return v; }
        private static TMP_Text TextChild(Transform parent,string name,int x,int y,int w,int h,TMP_FontAsset font,float size,string text)
        { var go=new GameObject(name,typeof(RectTransform),typeof(TextMeshProUGUI)); go.transform.SetParent(parent,false); Place(go.GetComponent<RectTransform>(),x,y,w,h); var t=go.GetComponent<TextMeshProUGUI>(); t.font=font;t.fontSize=size;t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Overflow;t.alignment=TextAlignmentOptions.TopLeft;t.margin=Vector4.zero;t.text=text;t.raycastTarget=false;return t; }
        private static void MigrateExactLegacyMenuLabelsOrValidate(GameObject root)
        {
            try { HubPresentationAuthoringValidator.ValidateMenuAsset(root); return; }
            catch (InvalidOperationException) { HubPresentationAuthoringValidator.ValidateLegacyMenuAssetForMigration(root); }
            var snapshot=LabelMigrationSnapshot.Take(); GameObject contents=null; var saveAttempted=false;
            try
            {
                contents=PrefabUtility.LoadPrefabContents(MenuPrefabPath); HubPresentationAuthoringValidator.ValidateLegacyMenuContentsForMigration(contents);
                var names=new[]{"Continue","NewGame","Settings","Quit"}; foreach(var name in names){var label=contents.transform.Find("SafeFrame/MenuPanel/"+name+"/Label") as RectTransform;if(label==null) throw new InvalidOperationException("Legacy label path is malformed: "+name);label.anchoredPosition=new Vector2(12,5);label.sizeDelta=new Vector2(168,22);}
                HubPresentationAuthoringValidator.ValidateMenuContents(contents); Inject(LabelMigrationFailurePoint.BeforeSave); saveAttempted=true;
                PrefabUtility.SaveAsPrefabAsset(contents,MenuPrefabPath); PrefabUtility.UnloadPrefabContents(contents); contents=null;
                Inject(LabelMigrationFailurePoint.AfterSave); AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport); Inject(LabelMigrationFailurePoint.AfterRefresh);
                HubPresentationAuthoringValidator.ValidateMenuAsset(AssetDatabase.LoadAssetAtPath<GameObject>(MenuPrefabPath)); Inject(LabelMigrationFailurePoint.AfterReloadValidation);
                Inject(LabelMigrationFailurePoint.BeforeSceneValidation); HubPresentationAuthoringValidator.ValidateSceneAsset(ScenePath); Inject(LabelMigrationFailurePoint.AfterSceneValidation); snapshot.AssertMetaGuidsUnchanged();
            }
            catch(Exception failure)
            {
                Exception rollbackFailure=null;
                if(contents!=null){try{PrefabUtility.UnloadPrefabContents(contents);}catch(Exception e){rollbackFailure=e;}finally{contents=null;}}
                try{if(saveAttempted)snapshot.RestoreAndAssert();else snapshot.AssertExact();}catch(Exception e){rollbackFailure=rollbackFailure==null?e:new AggregateException(rollbackFailure,e);}
                if(rollbackFailure!=null) throw new InvalidOperationException("Label migration failed and rollback verification failed.",new AggregateException(failure,rollbackFailure));
                throw;
            }
            finally { if(contents!=null) PrefabUtility.UnloadPrefabContents(contents); }
        }
        private static void Inject(LabelMigrationFailurePoint point){var failure=migrationFailureForTests==null?null:migrationFailureForTests(point);if(failure!=null) throw failure;}
        private sealed class LabelMigrationSnapshot
        {
            readonly byte[] prefab,scene,prefabMeta,sceneMeta;readonly string prefabHash,sceneHash,prefabMetaHash,sceneMetaHash,prefabGuid,sceneGuid;
            LabelMigrationSnapshot(byte[] p,byte[] s,byte[] pm,byte[] sm,string pg,string sg){prefab=p;scene=s;prefabMeta=pm;sceneMeta=sm;prefabHash=Hash(p);sceneHash=Hash(s);prefabMetaHash=Hash(pm);sceneMetaHash=Hash(sm);prefabGuid=pg;sceneGuid=sg;}
            internal static LabelMigrationSnapshot Take(){return new LabelMigrationSnapshot(Read(MenuPrefabPath),Read(ScenePath),Read(MenuPrefabPath+".meta"),Read(ScenePath+".meta"),AssetDatabase.AssetPathToGUID(MenuPrefabPath),AssetDatabase.AssetPathToGUID(ScenePath));}
            internal void RestoreAndAssert(){File.WriteAllBytes(MenuPrefabPath,prefab);File.WriteAllBytes(ScenePath,scene);File.WriteAllBytes(MenuPrefabPath+".meta",prefabMeta);File.WriteAllBytes(ScenePath+".meta",sceneMeta);AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);AssertExact();}
            internal void AssertExact(){if(Hash(Read(MenuPrefabPath))!=prefabHash||Hash(Read(ScenePath))!=sceneHash||Hash(Read(MenuPrefabPath+".meta"))!=prefabMetaHash||Hash(Read(ScenePath+".meta"))!=sceneMetaHash||AssetDatabase.AssetPathToGUID(MenuPrefabPath)!=prefabGuid||AssetDatabase.AssetPathToGUID(ScenePath)!=sceneGuid)throw new InvalidOperationException("Label migration rollback did not restore exact bytes/GUIDs.");}
            internal void AssertMetaGuidsUnchanged(){if(Hash(Read(MenuPrefabPath+".meta"))!=prefabMetaHash||Hash(Read(ScenePath+".meta"))!=sceneMetaHash||AssetDatabase.AssetPathToGUID(MenuPrefabPath)!=prefabGuid||AssetDatabase.AssetPathToGUID(ScenePath)!=sceneGuid)throw new InvalidOperationException("Label migration altered meta bytes/GUIDs.");}
            static byte[] Read(string path){if(!File.Exists(path))throw new InvalidOperationException("Label migration snapshot file is missing: "+path);return File.ReadAllBytes(path);} static string Hash(byte[] bytes){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-",string.Empty);}
        }
        private static Image ImageChild(Transform parent,string name,Vector2 anchor,Vector2 size,Vector2 min,Vector2 max,Color color)
        { var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);var r=go.GetComponent<RectTransform>();r.anchorMin=min;r.anchorMax=max;r.pivot=anchor;r.sizeDelta=size;r.anchoredPosition=Vector2.zero;var i=go.GetComponent<Image>();i.color=color;i.raycastTarget=false;return i; }
        private static void Place(RectTransform r,int x,int y,int w,int h){r.anchorMin=r.anchorMax=r.pivot=Vector2.zero;r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(x,y);}
        private static void CreateScene()
        { HubRuntimeAuthoringBuilder.BuildCanonicalPrefab(); var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single); var runtime=PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(HubRuntimeAuthoringBuilder.PrefabPath)) as GameObject; var menu=PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(MenuPrefabPath)) as GameObject; var p=menu.GetComponent<HubMenuPresenterV1>(); p.ConfigureForAuthoring(runtime.GetComponent<InputRouter>(),runtime.GetComponent<HubEntryHandoffLatchV1>(),menu.GetComponent<HubSafeFrameScalerV1>(),menu.GetComponentInChildren<EventSystem>(),menu.GetComponentsInChildren<HubMenuButtonViewV1>(true),p.transform.Find("SafeFrame/Notification/Dismiss").GetComponent<RectTransform>(),p.transform.Find("SafeFrame/Notification").gameObject,p.transform.Find("SafeFrame/Notification/Message").GetComponent<TMP_Text>(),p.transform.Find("SafeFrame/Notification/Dismiss").GetComponent<TMP_Text>()); EditorSceneManager.SaveScene(scene,ScenePath); }
    }
}
