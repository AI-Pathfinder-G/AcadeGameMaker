using AcadeGameMaker.Combat.Authoring;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    // AC-LPS-001/007/009: bounded builder and validator use the exact new authoring products.
    internal sealed class LocalPlayableSliceAuthoringTests
    {
        [Test] public void AcLps001_ExactNewSceneUsesOriginalStructuralValidatorAndLocalBindings()
        {
            var originalScenes=EditorBuildSettings.scenes;
            System.Runtime.ExceptionServices.ExceptionDispatchInfo failure=null;
            try
            {
                EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(LocalPlayableSliceAuthoringBuilder.ScenePath,true)};

            if(!System.IO.File.Exists(LocalPlayableSliceAuthoringBuilder.ScenePath))LocalPlayableSliceAuthoringBuilder.Build();
            var scene=EditorSceneManager.OpenScene(LocalPlayableSliceAuthoringBuilder.ScenePath);
            Assert.DoesNotThrow(()=>LocalPlayableSliceAuthoringValidator.ValidateScene(scene));
            Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>(LocalPlayableSliceAuthoringBuilder.GraphPath),Is.Not.Null);
        
            }
            catch(System.Exception error){failure=System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error);}
            finally
            {
                try
                {
                EditorBuildSettings.scenes=originalScenes;
                var restored=EditorBuildSettings.scenes;
                Assert.That(restored.Length,Is.EqualTo(originalScenes.Length));
                for(var i=0;i<originalScenes.Length;i++)
                {Assert.That(restored[i].path,Is.EqualTo(originalScenes[i].path));Assert.That(restored[i].enabled,Is.EqualTo(originalScenes[i].enabled));}
                }
                catch(System.Exception error)
                {if(failure==null)failure=System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error);else try{failure.SourceException.Data["CH1BuildSettingsRestoreFailure"]=error;}catch{}}
            }
            if(failure!=null)failure.Throw();
        }
        [Test] public void AcLps009_MissingPresentationRootFailsWithoutGraphRepair()
        {
            var originalScenes=EditorBuildSettings.scenes;
            System.Runtime.ExceptionServices.ExceptionDispatchInfo failure=null;
            try
            {
                EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(LocalPlayableSliceAuthoringBuilder.ScenePath,true)};

            if(!System.IO.File.Exists(LocalPlayableSliceAuthoringBuilder.ScenePath))LocalPlayableSliceAuthoringBuilder.Build();
            var scene=EditorSceneManager.OpenScene(LocalPlayableSliceAuthoringBuilder.ScenePath);
            foreach(var root in scene.GetRootGameObjects())if(root.name=="LocalPlayablePresentation")Object.DestroyImmediate(root);
            Assert.Throws<System.InvalidOperationException>(()=>LocalPlayableSliceAuthoringValidator.ValidateScene(scene));
            EditorSceneManager.OpenScene(LocalPlayableSliceAuthoringBuilder.ScenePath);
        
            }
            catch(System.Exception error){failure=System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error);}
            finally
            {
                try
                {
                EditorBuildSettings.scenes=originalScenes;
                var restored=EditorBuildSettings.scenes;
                Assert.That(restored.Length,Is.EqualTo(originalScenes.Length));
                for(var i=0;i<originalScenes.Length;i++)
                {Assert.That(restored[i].path,Is.EqualTo(originalScenes[i].path));Assert.That(restored[i].enabled,Is.EqualTo(originalScenes[i].enabled));}
                }
                catch(System.Exception error)
                {if(failure==null)failure=System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error);else try{failure.SourceException.Data["CH1BuildSettingsRestoreFailure"]=error;}catch{}}
            }
            if(failure!=null)failure.Throw();
        }
    }
}
