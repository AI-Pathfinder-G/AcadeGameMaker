using System.Collections;
using AcadeGameMaker.Hub.Presentation.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using TMPro;
using UnityEditor;

namespace AcadeGameMaker.Tests.PlayMode.HubPresentation
{
    // AC-LPS-001/009: the production scene entry and scene-exit lifecycle.
    internal sealed class LocalPlayableSliceLifecyclePlayModeTests
    {
        [UnityEngine.SerializeField] private AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope _runtimeFixture=new AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope();
        [UnityEngine.TestTools.UnitySetUp] public System.Collections.IEnumerator BeginRuntimeCase(){_runtimeFixture.PrepareEnter();yield return new UnityEngine.TestTools.EnterPlayMode();_runtimeFixture.ConfirmEntered();yield return _runtimeFixture.RunBody(_runtimeFixture.BeginCase(true));}
        [UnityEngine.TestTools.UnityTearDown] public System.Collections.IEnumerator CloseRuntimeCase(){try{if(_runtimeFixture.OwnsRuntimeFixture && UnityEngine.Application.isPlaying)yield return _runtimeFixture.RunBody(_runtimeFixture.CloseCase(),true);}finally{}if(_runtimeFixture.OwnsRuntimeFixture){if(UnityEngine.Application.isPlaying)yield return new UnityEngine.TestTools.ExitPlayMode();_runtimeFixture.CaptureCleanupFailure(_runtimeFixture.CompleteExit);}_runtimeFixture.ThrowCleanupFailure();}
        

        [UnityTest] public IEnumerator AcLps001_ProductionSceneFirstTickReachesPresenterWithoutRepair()
        {yield return _runtimeFixture.RunBody(AcLps001_ProductionSceneFirstTickReachesPresenterWithoutRepairOriginalBody());}
        private IEnumerator AcLps001_ProductionSceneFirstTickReachesPresenterWithoutRepairOriginalBody(){
            yield return SceneManager.LoadSceneAsync("LocalPlayableSlice",LoadSceneMode.Single);
            yield return null;yield return new WaitForFixedUpdate();yield return new WaitForFixedUpdate();
            var presenter=Object.FindFirstObjectByType<LocalPlayableSlicePresenter>();
            Assert.That(presenter,Is.Not.Null);Assert.That(presenter.IsFailed,Is.False);
            Assert.That(presenter.HasCompletedView,Is.True);Assert.That(presenter.Generation,Is.EqualTo(1));
            Assert.That(presenter.RestartPending,Is.False);
            var regular=presenter.RuntimeRegularFont;var bold=presenter.RuntimeBoldFont;
            var original=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset");
            Assert.That(regular,Is.Not.Null);Assert.That(bold,Is.Not.Null);Assert.That(regular,Is.Not.SameAs(original));
            Assert.That(regular.HasCharacters(LocalPlayableSlicePresenter.GlyphCopy),Is.True);
            Assert.That(bold.HasCharacters(LocalPlayableSlicePresenter.GlyphCopy),Is.True);
            var regularMaterial=regular.material;var boldMaterial=bold.material;
            var regularAtlas=regular.atlasTextures[0];var boldAtlas=bold.atlasTextures[0];
            var scene=SceneManager.GetActiveScene();
            var empty=SceneManager.CreateScene("LocalSliceLifecycleExit");SceneManager.SetActiveScene(empty);
            yield return SceneManager.UnloadSceneAsync(scene);
            yield return null;
            Assert.That(presenter==null,Is.True);Assert.That(regular==null && bold==null,Is.True);
            Assert.That(regularMaterial==null && boldMaterial==null,Is.True);
            Assert.That(regularAtlas==null && boldAtlas==null,Is.True);Assert.That(original!=null,Is.True);
            Assert.That(original.material!=null && original.atlasTextures[0]!=null,Is.True);
        }
    }
}
