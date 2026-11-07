using System;
using System.Collections;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // AC-C1B-INPUT-001/EVIDENCE-001: normal runtime and explicitly synthetic boundary stepping.
    internal sealed class Chapter1RegularInputPlayModeTests : InputTestFixture
    {
        [UnityEngine.SerializeField] private AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope _runtimeFixture=new AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope();
        [UnityEngine.TestTools.UnitySetUp] public System.Collections.IEnumerator BeginRuntimeCase(){_runtimeFixture.PrepareEnter();yield return new UnityEngine.TestTools.EnterPlayMode();_runtimeFixture.ConfirmEntered();yield return _runtimeFixture.RunBody(_runtimeFixture.BeginCase(false));}
        public override void TearDown(){}
        [UnityEngine.TestTools.UnityTearDown] public System.Collections.IEnumerator CloseRuntimeCase(){try{if(_runtimeFixture.OwnsRuntimeFixture && UnityEngine.Application.isPlaying)yield return _runtimeFixture.RunBody(_runtimeFixture.CloseCase(),true);}finally{_runtimeFixture.CaptureCleanupFailure(()=>base.TearDown());}if(_runtimeFixture.OwnsRuntimeFixture){if(UnityEngine.Application.isPlaying)yield return new UnityEngine.TestTools.ExitPlayMode();_runtimeFixture.CaptureCleanupFailure(_runtimeFixture.CompleteExit);}_runtimeFixture.ThrowCleanupFailure();}
        

        private GameObject _ownerObject;
        private Chapter1EncounterLifetimeOwner _owner;
        private InputRouter Router=>_owner.CurrentRouter;
        private void Begin(){InputSystem.AddDevice<Keyboard>();var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/Chapter1RegularEncounter.prefab");Assert.That(prefab,Is.Not.Null);_ownerObject=new GameObject("Chapter1Owner");_owner=_ownerObject.AddComponent<Chapter1EncounterLifetimeOwner>();_owner.BeginAuthoredRegularEncounter(prefab);}
        // Normal UnityTests instantiate the authored production host; direct Begin above is synthetic setup.
        private void BeginHost()
        {
            InputSystem.AddDevice<Keyboard>();var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/Chapter1RegularEncounterHost.prefab");Assert.That(prefab,Is.Not.Null);
            _ownerObject=UnityEngine.Object.Instantiate(prefab);_owner=_ownerObject.GetComponent<Chapter1EncounterLifetimeOwner>();Assert.That(_owner,Is.Not.Null);Assert.That(_owner.CurrentRouter,Is.Null);
        }
        [TearDown] public void Cleanup(){if(_owner!=null && _owner.CurrentRouter!=null)UnityEngine.Object.DestroyImmediate(_owner.CurrentRouter.transform.root.gameObject);if(_ownerObject!=null)UnityEngine.Object.DestroyImmediate(_ownerObject);}
        private IEnumerator Completed(){for(var i=0;i<30;i++){InputSystem.Update();yield return new WaitForFixedUpdate();if(_owner.TryGetCompletedEvidence(out _))yield break;}Assert.Fail("No completed producer evidence.");}
        [UnityTest] public IEnumerator RegularCommitUsesGlobalReceipt(){yield return _runtimeFixture.RunBody(RegularCommitUsesGlobalReceiptOriginalBody());}
        private IEnumerator RegularCommitUsesGlobalReceiptOriginalBody(){BeginHost();yield return null;yield return Completed();Assert.That(Router.IsFaulted,Is.False);Assert.That(_owner.TryGetCompletedEvidence(out var view),Is.True);Assert.That(view.Receipt,Is.EqualTo(Router.CurrentReceipt.Value));Assert.That(Router.BoundChapter1Combat.LatestOutcomeForEncounterHandoff.Tick,Is.EqualTo(view.Tick));}
        [Test] public void RejectForeignRegistration(){Begin();Router.InitializeForTests();var combat=Router.BoundChapter1Combat;Assert.Throws<InvalidOperationException>(()=>combat.ValidateCoordinatedInputRegistration(Router,Router,new object(),"foreign",1,Router.BoundPlayerForAuthoring.NextExpectedTick));Assert.That(Router.CurrentReceipt.HasValue,Is.False);}
        [UnityTest] public IEnumerator RejectStaleGraph()
        {yield return _runtimeFixture.RunBody(RejectStaleGraphOriginalBody());}
        private IEnumerator RejectStaleGraphOriginalBody(){
            BeginHost();yield return null;yield return Completed();var old=Router;var lifetime=old.Chapter1Lifetime;
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/Chapter1RegularEncounter.prefab");_owner.RequestRegularReplacement(prefab);
            for(var i=0;i<60 && _owner.Generation==1;i++){InputSystem.Update();yield return new WaitForFixedUpdate();yield return null;}
            Assert.That(_owner.Generation,Is.EqualTo(2));Assert.That(lifetime.IsClosed,Is.True);
            Assert.Throws<InvalidOperationException>(()=>old.StepForTests());Assert.That(ReferenceEquals(old,Router),Is.False);
        }
        [Test] public void PrepareFailureKeepsBatch()
        {
            Begin();Router.InitializeForTests();var first=Router.BoundPlayerForAuthoring.NextExpectedTick;Press(Keyboard.current.spaceKey);
            Router.SetFaultStageForTests(InputRouterFaultStage.PrepareCombat);Assert.Throws<InvalidOperationException>(()=>Router.StepForTests());
            Assert.That(Router.IsFaulted,Is.False);Assert.That(Router.CurrentReceipt.HasValue,Is.False);Assert.That(Router.BoundPlayerForAuthoring.NextExpectedTick,Is.EqualTo(first));
            Router.SetFaultStageForTests(null);Router.StepForTests();Assert.That(Router.CurrentReceipt.Value.Tick,Is.EqualTo(first));
            Physics2D.SyncTransforms();var movement=Router.BoundPlayerForAuthoring.AdvanceOneFixedTickForTests();Assert.That(movement.VelocityYQ4096,Is.GreaterThan(0));
            Assert.That(_owner.TryGetCompletedEvidence(out _),Is.False);
        }
        [Test] public void PartialCommitBlocksAllPhases()
        {
            Begin();Router.InitializeForTests();var router=Router;var first=router.BoundPlayerForAuthoring.NextExpectedTick;
            router.SetFaultStageForTests(InputRouterFaultStage.AfterTransferCommit);Assert.Throws<InvalidOperationException>(()=>router.StepForTests());
            Assert.That(router.IsFaulted,Is.True);Assert.That(router.CurrentReceipt.HasValue,Is.False);
            Assert.Throws<InvalidOperationException>(()=>router.BoundPlayerForAuthoring.AdvanceOneFixedTickForTests());
            Assert.Throws<InvalidOperationException>(()=>router.BoundTransferForAuthoring.AdvanceCoordinatedInputPhaseForTests());
            Assert.Throws<InvalidOperationException>(()=>router.BoundChapter1Combat.AdvanceCombatOnlyForTests());
            Assert.That(router.BoundPlayerForAuthoring.NextExpectedTick,Is.EqualTo(first));Assert.That(router.BoundChapter1Combat.NextCombatTickForTests,Is.EqualTo(first));
            Assert.That(router.TryGetChapter1CompletedEvidence(out _),Is.False);
        }
        [UnityTest] public IEnumerator PublishesOnlyCompletedProducerEvidence(){yield return _runtimeFixture.RunBody(PublishesOnlyCompletedProducerEvidenceOriginalBody());}
        private IEnumerator PublishesOnlyCompletedProducerEvidenceOriginalBody(){BeginHost();Assert.That(_owner.TryGetCompletedEvidence(out _),Is.False);yield return null;yield return Completed();Assert.That(_owner.TryGetCompletedEvidence(out var view),Is.True);Assert.That(view.HasCompleted,Is.True);Assert.That(view.Receipt,Is.EqualTo(Router.CurrentReceipt.Value));}
    }
}