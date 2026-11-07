using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AcadeGameMaker.Hub.Authoring.Editor;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.HubPresentation
{
    // AC-M5D7QA-001/002/008: canonical TMP precondition and authoring output.
    internal sealed class HubPresentationAuthoringTests
    {
        [Test]
        public void AC_M5D7QA_002_And_008_CanonicalTmpSettingsPassesTypedPrecondition()
        {
            Assert.DoesNotThrow(() => HubPresentationAuthoringValidator.ValidateTmpSettingsAsset());
        }

        [Test]
        public void AC_M5D7QA_001_And_002_BuilderCreatesAndThenValidatesCanonicalAssets()
        {
            HubPresentationAuthoringBuilder.Build();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(HubPresentationAuthoringBuilder.MenuPrefabPath);
            Assert.That(prefab, Is.Not.Null);
            Assert.DoesNotThrow(() => HubPresentationAuthoringValidator.ValidateMenuAsset(prefab));
            Assert.DoesNotThrow(() => HubPresentationAuthoringValidator.ValidateSceneAsset(HubPresentationAuthoringBuilder.ScenePath));
        }

        // REQ-M5D7QA-006/009; AC-M5D7QA-002/007/010. All four labels share
        // the centered 12,5,168,22 profile; no per-label exception exists.
        [Test]
        public void AC_M5D7QA_002_And_007_MenuLabelsUseExactCenteredNoOverflowProfile()
        {
            HubPresentationAuthoringBuilder.Build();
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(HubPresentationAuthoringBuilder.MenuPrefabPath);
            var names=new[]{"Continue","NewGame","Settings","Quit"};
            foreach(var name in names)
            {
                var rect=prefab.transform.Find("SafeFrame/MenuPanel/"+name+"/Label") as RectTransform;
                var text=rect.GetComponent<TMP_Text>();
                Assert.That(rect.anchorMin,Is.EqualTo(Vector2.zero)); Assert.That(rect.anchorMax,Is.EqualTo(Vector2.zero)); Assert.That(rect.pivot,Is.EqualTo(Vector2.zero));
                Assert.That(rect.anchoredPosition,Is.EqualTo(new Vector2(12,5))); Assert.That(rect.sizeDelta,Is.EqualTo(new Vector2(168,22))); Assert.That(rect.anchoredPosition+rect.sizeDelta*.5f,Is.EqualTo(new Vector2(96,16)));
                Assert.That(rect.localScale,Is.EqualTo(Vector3.one)); Assert.That(rect.localRotation,Is.EqualTo(Quaternion.identity)); Assert.That(text.margin,Is.EqualTo(Vector4.zero)); Assert.That(text.alignment,Is.EqualTo(TextAlignmentOptions.TopLeft));
            }
        }

        [TestCase("legacy")]
        [TestCase("mixed20")]
        [TestCase("mixed22")]
        [TestCase("y4")]
        [TestCase("height")]
        [TestCase("height23")]
        [TestCase("width")]
        [TestCase("position")]
        [TestCase("anchor")]
        [TestCase("pivot")]
        [TestCase("scale")]
        [TestCase("rotation")]
        [TestCase("margin")]
        [TestCase("alignment")]
        [TestCase("copy")]
        [TestCase("fontSize")]
        [TestCase("wrapping")]
        [TestCase("material")]
        [TestCase("button")]
        public void AC_M5D7QA_002_And_010_LabelProfileRejectsIndependentMutation(string mutation)
        {
            HubPresentationAuthoringBuilder.Build();
            var clone=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(HubPresentationAuthoringBuilder.MenuPrefabPath));
            try
            {
                var label=clone.transform.Find("SafeFrame/MenuPanel/Continue/Label") as RectTransform; var text=label.GetComponent<TMP_Text>();
                switch(mutation){case "legacy":label.anchoredPosition=new Vector2(12,6);label.sizeDelta=new Vector2(168,20);break;case "mixed20":label.sizeDelta=new Vector2(168,20);break;case "mixed22":label.anchoredPosition=new Vector2(12,6);break;case "y4":label.anchoredPosition=new Vector2(12,4);break;case "height":label.sizeDelta=new Vector2(168,21);break;case "height23":label.sizeDelta=new Vector2(168,23);break;case "width":label.sizeDelta=new Vector2(167,22);break;case "position":label.anchoredPosition=new Vector2(11,5);break;case "anchor":label.anchorMin=new Vector2(.5f,0);break;case "pivot":label.pivot=new Vector2(.5f,0);break;case "scale":label.localScale=new Vector3(1,2,1);break;case "rotation":label.localRotation=Quaternion.Euler(0,0,1);break;case "margin":text.margin=new Vector4(1,0,0,0);break;case "alignment":text.alignment=TextAlignmentOptions.Center;break;case "copy":text.text="x";break;case "fontSize":text.fontSize=13;break;case "wrapping":text.textWrappingMode=TextWrappingModes.NoWrap;break;case "material":text.fontSharedMaterial=null;break;case "button":((RectTransform)label.parent).anchoredPosition=new Vector2(15,146);break;}
                Assert.Throws<System.InvalidOperationException>(()=>HubPresentationAuthoringValidator.ValidateMenuLabelProfile(clone));
            }
            finally { UnityEngine.Object.DestroyImmediate(clone); }
        }

        [Test]
        public void AC_M5D7QA_002_And_010_LegacyAllFourIsAdmittedOnlyAsACompleteMigrationProfile()
        {
            HubPresentationAuthoringBuilder.Build();
            var clone=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(HubPresentationAuthoringBuilder.MenuPrefabPath));
            try
            {
                foreach(var name in new[]{"Continue","NewGame","Settings","Quit"}){var label=clone.transform.Find("SafeFrame/MenuPanel/"+name+"/Label") as RectTransform;label.anchoredPosition=new Vector2(12,6);label.sizeDelta=new Vector2(168,20);}
                Assert.DoesNotThrow(()=>HubPresentationAuthoringValidator.ValidateLegacyMenuLabelProfile(clone)); Assert.Throws<System.InvalidOperationException>(()=>HubPresentationAuthoringValidator.ValidateMenuLabelProfile(clone));
                ((RectTransform)clone.transform.Find("SafeFrame/MenuPanel/Continue/Label")).anchoredPosition=new Vector2(12,5); ((RectTransform)clone.transform.Find("SafeFrame/MenuPanel/Continue/Label")).sizeDelta=new Vector2(168,22);
                Assert.Throws<System.InvalidOperationException>(()=>HubPresentationAuthoringValidator.ValidateLegacyMenuLabelProfile(clone));
            }
            finally { UnityEngine.Object.DestroyImmediate(clone); }
        }

        // REQ-M5D7QA-009; AC-M5D7QA-002/010. Each deterministic post-candidate
        // failure restores the exact prefab/scene/meta bytes and their GUIDs.
        [TestCase(HubPresentationAuthoringBuilder.LabelMigrationFailurePoint.BeforeSave)]
        [TestCase(HubPresentationAuthoringBuilder.LabelMigrationFailurePoint.AfterSave)]
        [TestCase(HubPresentationAuthoringBuilder.LabelMigrationFailurePoint.AfterRefresh)]
        [TestCase(HubPresentationAuthoringBuilder.LabelMigrationFailurePoint.AfterReloadValidation)]
        [TestCase(HubPresentationAuthoringBuilder.LabelMigrationFailurePoint.BeforeSceneValidation)]
        [TestCase(HubPresentationAuthoringBuilder.LabelMigrationFailurePoint.AfterSceneValidation)]
        public void AC_M5D7QA_002_And_010_MigrationFailureRestoresExactAuthoringBytesAndGuids(HubPresentationAuthoringBuilder.LabelMigrationFailurePoint point)
        {
            var files=new[]{HubPresentationAuthoringBuilder.MenuPrefabPath,HubPresentationAuthoringBuilder.ScenePath,HubPresentationAuthoringBuilder.MenuPrefabPath+".meta",HubPresentationAuthoringBuilder.ScenePath+".meta"}; var original=files.Select(File.ReadAllBytes).ToArray();
            try
            {
                SaveExactLegacyProfile(); var before=files.Select(File.ReadAllBytes).ToArray(); var prefabGuid=AssetDatabase.AssetPathToGUID(HubPresentationAuthoringBuilder.MenuPrefabPath); var sceneGuid=AssetDatabase.AssetPathToGUID(HubPresentationAuthoringBuilder.ScenePath);
                HubPresentationAuthoringBuilder.SetLabelMigrationFailureInjectorForTests(candidate=>candidate==point?new InvalidOperationException(point.ToString()):null);
                Assert.Throws<InvalidOperationException>(()=>HubPresentationAuthoringBuilder.Build());
                for(var i=0;i<files.Length;i++) CollectionAssert.AreEqual(before[i],File.ReadAllBytes(files[i]),files[i]); Assert.That(AssetDatabase.AssetPathToGUID(HubPresentationAuthoringBuilder.MenuPrefabPath),Is.EqualTo(prefabGuid)); Assert.That(AssetDatabase.AssetPathToGUID(HubPresentationAuthoringBuilder.ScenePath),Is.EqualTo(sceneGuid));
                Assert.DoesNotThrow(()=>HubPresentationAuthoringValidator.ValidateLegacyMenuLabelProfile(AssetDatabase.LoadAssetAtPath<GameObject>(HubPresentationAuthoringBuilder.MenuPrefabPath)));
            }
            finally
            {
                HubPresentationAuthoringBuilder.SetLabelMigrationFailureInjectorForTests(null); for(var i=0;i<files.Length;i++) File.WriteAllBytes(files[i],original[i]); AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }

        // REQ-M5D7QA-009; AC-M5D7QA-002/010. The scene admits only the
        // canonical root-default matrix plus its two exact Q0 presenter links.
        [TestCase("missingRouter")]
        [TestCase("wrongRouter")]
        [TestCase("duplicateRouter")]
        [TestCase("missingLatch")]
        [TestCase("wrongLatch")]
        [TestCase("duplicateLatch")]
        [TestCase("wrongTarget")]
        [TestCase("wrongPath")]
        [TestCase("wrongPresenterValue")]
        [TestCase("extraRootTransform")]
        [TestCase("wrongRootTransformValue")]
        [TestCase("wrongRootName")]
        [TestCase("wrongRootReference")]
        public void AC_M5D7QA_002_And_010_SceneOverrideMatrixRejectsIndependentMutation(string mutation)
        {
            HubPresentationAuthoringBuilder.Build();
            var scene=EditorSceneManager.OpenScene(HubPresentationAuthoringBuilder.ScenePath,OpenSceneMode.Additive); GameObject wrong=null;
            try
            {
                var roots=scene.GetRootGameObjects(); var runtime=roots.Single(x=>x.name=="HubRuntimeRoot"); var menu=roots.Single(x=>x.name=="HubMenuRoot");
                Assert.DoesNotThrow(()=>HubPresentationAuthoringValidator.ValidateSceneInstanceOverrideMatrix(runtime,menu));
                var original=(PrefabUtility.GetPropertyModifications(menu)??Array.Empty<PropertyModification>()).Select(Clone).ToArray(); var modified=original.Select(Clone).ToList();
                switch(mutation)
                {
                    case "missingRouter": modified.RemoveAll(x=>x.propertyPath=="_router"); break;
                    case "wrongRouter": wrong=new GameObject("WrongRouter"); Find(modified,"_router").objectReference=wrong.AddComponent(Find(original,"_router").objectReference.GetType()); break;
                    case "duplicateRouter": modified.Add(Clone(Find(original,"_router"))); break;
                    case "missingLatch": modified.RemoveAll(x=>x.propertyPath=="_latch"); break;
                    case "wrongLatch": wrong=new GameObject("WrongLatch"); Find(modified,"_latch").objectReference=wrong.AddComponent(Find(original,"_latch").objectReference.GetType()); break;
                    case "duplicateLatch": modified.Add(Clone(Find(original,"_latch"))); break;
                    case "wrongTarget": Find(modified,"_router").target=menu.transform; break;
                    case "wrongPath": Find(modified,"_router").propertyPath="_unexpected"; break;
                    case "wrongPresenterValue": Find(modified,"_router").value="1"; break;
                    case "extraRootTransform": modified.Add(new PropertyModification{target=menu.transform,propertyPath="m_LocalScale.x",value="1",objectReference=null}); break;
                    case "wrongRootTransformValue": Find(modified,"m_LocalPosition.x").value="1"; break;
                    case "wrongRootName": Find(modified,"m_Name").value="Unexpected"; break;
                    case "wrongRootReference": Find(modified,"m_LocalPosition.x").objectReference=menu; break;
                    default: throw new InvalidOperationException("Unknown mutation: "+mutation);
                }
                PrefabUtility.SetPropertyModifications(menu,modified.ToArray());
                Assert.Throws<InvalidOperationException>(()=>HubPresentationAuthoringValidator.ValidateSceneInstanceOverrideMatrix(runtime,menu));
                PrefabUtility.SetPropertyModifications(menu,original);
            }
            finally { if(wrong!=null) UnityEngine.Object.DestroyImmediate(wrong); EditorSceneManager.CloseScene(scene,true); }
        }

        // REQ-M5D7QA-009; AC-M5D7QA-002/010. Q0 has no semantic override:
        // its only admitted entries are Unity's exact serialized root defaults.
        [TestCase("missing")]
        [TestCase("duplicate")]
        [TestCase("wrongTarget")]
        [TestCase("wrongPath")]
        [TestCase("wrongValue")]
        [TestCase("wrongReference")]
        [TestCase("extra")]
        public void AC_M5D7QA_002_And_010_Q0OverrideMatrixRejectsIndependentMutation(string mutation)
        {
            HubPresentationAuthoringBuilder.Build();
            var scene=EditorSceneManager.OpenScene(HubPresentationAuthoringBuilder.ScenePath,OpenSceneMode.Additive);
            try
            {
                var roots=scene.GetRootGameObjects(); var runtime=roots.Single(x=>x.name=="HubRuntimeRoot"); var menu=roots.Single(x=>x.name=="HubMenuRoot");
                Assert.DoesNotThrow(()=>HubPresentationAuthoringValidator.ValidateSceneInstanceOverrideMatrix(runtime,menu));
                var original=(PrefabUtility.GetPropertyModifications(runtime)??Array.Empty<PropertyModification>()).Select(Clone).ToArray(); var modified=original.Select(Clone).ToList();
                switch(mutation)
                {
                    case "missing": modified.RemoveAll(x=>x.propertyPath=="m_Name"); break;
                    case "duplicate": modified.Add(Clone(Find(original,"m_Name"))); break;
                    case "wrongTarget": Find(modified,"m_Name").target=runtime.transform; break;
                    case "wrongPath": Find(modified,"m_Name").propertyPath="m_TagString"; break;
                    case "wrongValue": Find(modified,"m_Name").value="Unexpected"; break;
                    case "wrongReference": Find(modified,"m_LocalPosition.x").objectReference=runtime; break;
                    case "extra": modified.Add(new PropertyModification{target=runtime.transform,propertyPath="m_LocalScale.x",value="1",objectReference=null}); break;
                    default: throw new InvalidOperationException("Unknown mutation: "+mutation);
                }
                PrefabUtility.SetPropertyModifications(runtime,modified.ToArray());
                Assert.Throws<InvalidOperationException>(()=>HubPresentationAuthoringValidator.ValidateSceneInstanceOverrideMatrix(runtime,menu));
                PrefabUtility.SetPropertyModifications(runtime,original);
            }
            finally { EditorSceneManager.CloseScene(scene,true); }
        }

        static PropertyModification Find(System.Collections.Generic.IEnumerable<PropertyModification> modifications,string path)
        {
            return modifications.Single(x=>x.propertyPath==path);
        }
        static PropertyModification Clone(PropertyModification value)
        {
            return new PropertyModification{target=value.target,propertyPath=value.propertyPath,value=value.value,objectReference=value.objectReference};
        }

        static void SaveExactLegacyProfile()
        {
            var contents=PrefabUtility.LoadPrefabContents(HubPresentationAuthoringBuilder.MenuPrefabPath);
            try { foreach(var name in new[]{"Continue","NewGame","Settings","Quit"}){var label=contents.transform.Find("SafeFrame/MenuPanel/"+name+"/Label") as RectTransform; label.anchoredPosition=new Vector2(12,6); label.sizeDelta=new Vector2(168,20);} PrefabUtility.SaveAsPrefabAsset(contents,HubPresentationAuthoringBuilder.MenuPrefabPath); }
            finally { PrefabUtility.UnloadPrefabContents(contents); AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport); }
        }

        // REQ-M5D7QA-006/008/009; AC-M5D7QA-001/002/007/008/009/010.
        // Uses the real project prefab TMP objects and uGUI package path, never a mock mask setter.
        [Test]
        public void AC_M5D7QA_007_And_009_CanvasChannelsAreIsolatedAcrossBothCapturePassLabels()
        {
            WithCaptureCanvas((canvas,camera,texts)=>
            {
                InvokeProbe(canvas,camera,texts,"A","640x360.png",0);
                Assert.AreEqual(AdditionalCanvasShaderChannels.None,canvas.additionalShaderChannels);
                InvokeProbe(canvas,camera,texts,"A","1280x720.png",0);
                Assert.AreEqual(AdditionalCanvasShaderChannels.None,canvas.additionalShaderChannels);
                InvokeProbe(canvas,camera,texts,"B","640x360.png",0);
                Assert.AreEqual(AdditionalCanvasShaderChannels.None,canvas.additionalShaderChannels);
                InvokeProbe(canvas,camera,texts,"B","1280x720.png",0);
                Assert.AreEqual(AdditionalCanvasShaderChannels.None,canvas.additionalShaderChannels);
            });
        }

        [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)]
        public void AC_M5D7QA_009_FaultsAfterTmpThroughReadbackStillRestoreExactNone(int injection)
        {
            WithCaptureCanvas((canvas,camera,texts)=>
            {
                var error=InvokeProbeExpectingFailure(canvas,camera,texts,"A","fault.png",injection);
                StringAssert.Contains("Injected",error.Message);Assert.AreEqual(AdditionalCanvasShaderChannels.None,canvas.additionalShaderChannels);
            });
        }

        [Test]
        public void AC_M5D7QA_009_CleanupFailureRetainsThePrimaryFault()
        {
            WithCaptureCanvas((canvas,camera,texts)=>
            {
                var error=InvokeProbeExpectingFailure(canvas,camera,texts,"B","cleanup.png",6);
                Assert.IsInstanceOf<AggregateException>(error.InnerException);StringAssert.Contains("Canvas probe failed and cleanup failed",error.Message);
                InvokeProbe(canvas,camera,texts,"B","recovered.png",0);Assert.AreEqual(AdditionalCanvasShaderChannels.None,canvas.additionalShaderChannels);
            });
        }

        [TestCase((int)AdditionalCanvasShaderChannels.TexCoord1,"AFTER_TMP_MISSING_BITS")]
        [TestCase((int)AdditionalCanvasShaderChannels.Normal,"AFTER_TMP_MISSING_BITS")]
        [TestCase((int)AdditionalCanvasShaderChannels.Tangent,"AFTER_TMP_MISSING_BITS")]
        [TestCase((int)(AdditionalCanvasShaderChannels.TexCoord1|AdditionalCanvasShaderChannels.Normal|AdditionalCanvasShaderChannels.Tangent|AdditionalCanvasShaderChannels.TexCoord2),"AFTER_TMP_EXTRA_BITS")]
        [TestCase((int)(AdditionalCanvasShaderChannels.TexCoord1|AdditionalCanvasShaderChannels.Normal|AdditionalCanvasShaderChannels.Tangent|AdditionalCanvasShaderChannels.TexCoord3),"AFTER_TMP_EXTRA_BITS")]
        public void AC_M5D7QA_009_ChannelPredicateRejectsIndependentMissingAndExtraBits(int mask,string code)
        {
            WithCaptureCanvas((canvas,camera,texts)=>
            {
                InvokeProbe(canvas,camera,texts,"A","seed.png",0);canvas.additionalShaderChannels=(AdditionalCanvasShaderChannels)mask;
                var error=InvokeAssertExpectingFailure(canvas,camera,"A","mutation.png","after-tmp",25);StringAssert.Contains(code,error.Message);
            });
        }

        [Test]
        public void AC_M5D7QA_009_ChannelPredicateRejectsNonChannelCanvasProfileDrift()
        {
            WithCaptureCanvas((canvas,camera,texts)=>
            {
                InvokeProbe(canvas,camera,texts,"A","seed.png",0);canvas.pixelPerfect=false;
                var error=InvokeAssertExpectingFailure(canvas,camera,"A","profile.png","baseline",0);StringAssert.Contains("CANVAS_PROFILE_DRIFT",error.Message);
            });
        }

        static void WithCaptureCanvas(Action<Canvas,Camera,TMP_Text[]> action)
        {
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(HubPresentationAuthoringBuilder.MenuPrefabPath);var clone=UnityEngine.Object.Instantiate(prefab);var cameraGo=new GameObject("M5D7QA Canvas Channel Test Camera");var camera=cameraGo.AddComponent<Camera>();
            try{clone.SetActive(true);var canvas=clone.GetComponent<Canvas>();var texts=clone.GetComponentsInChildren<TMP_Text>(true);Assert.NotNull(canvas);Assert.AreEqual(6,texts.Length);action(canvas,camera,texts);}finally{UnityEngine.Object.DestroyImmediate(cameraGo);UnityEngine.Object.DestroyImmediate(clone);}
        }
        static Exception InvokeProbeExpectingFailure(Canvas c,Camera camera,TMP_Text[] texts,string pass,string file,int injection){var ex=Assert.Throws<TargetInvocationException>(()=>Probe().Invoke(null,new object[]{c,camera,texts,pass,file,injection}));return ex.InnerException;}
        static void InvokeProbe(Canvas c,Camera camera,TMP_Text[] texts,string pass,string file,int injection){Probe().Invoke(null,new object[]{c,camera,texts,pass,file,injection});}
        static Exception InvokeAssertExpectingFailure(Canvas c,Camera camera,string pass,string file,string phase,int required){var ex=Assert.Throws<TargetInvocationException>(()=>AssertChannels().Invoke(null,new object[]{c,camera,pass,file,phase,required}));return ex.InnerException;}
        static MethodInfo Probe(){return typeof(HubPresentationCaptureGenerator).GetMethod("CanvasChannelProbeForTests",BindingFlags.Static|BindingFlags.NonPublic);}
        static MethodInfo AssertChannels(){return typeof(HubPresentationCaptureGenerator).GetMethod("AssertCanvasChannelsForTests",BindingFlags.Static|BindingFlags.NonPublic);}
    }
}
