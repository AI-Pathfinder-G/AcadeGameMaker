using System.Collections;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // AC-LPS-001/004/005: production Start/FixedUpdate, not lifecycle reflection.
    internal sealed class LocalPlayableSliceInputPlayModeTests : InputTestFixture
    {
        [UnityEngine.SerializeField] private AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope _runtimeFixture=new AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope();
        [UnityEngine.TestTools.UnitySetUp] public System.Collections.IEnumerator BeginRuntimeCase(){_runtimeFixture.PrepareEnter();yield return new UnityEngine.TestTools.EnterPlayMode();_runtimeFixture.ConfirmEntered();yield return _runtimeFixture.RunBody(_runtimeFixture.BeginCase(true));}
        public override void TearDown(){}
        [UnityEngine.TestTools.UnityTearDown] public System.Collections.IEnumerator CloseRuntimeCase(){try{if(_runtimeFixture.OwnsRuntimeFixture && UnityEngine.Application.isPlaying)yield return _runtimeFixture.RunBody(_runtimeFixture.CloseCase(),true);}finally{_runtimeFixture.CaptureCleanupFailure(()=>base.TearDown());}if(_runtimeFixture.OwnsRuntimeFixture){if(UnityEngine.Application.isPlaying)yield return new UnityEngine.TestTools.ExitPlayMode();_runtimeFixture.CaptureCleanupFailure(_runtimeFixture.CompleteExit);}_runtimeFixture.ThrowCleanupFailure();}
        

        private const string LocalPath="Assets/Prefabs/LocalPlayableSlice/LocalPlayableSliceGraph.prefab";
        private GameObject _root;
        // REQ-LPS-012: observation only; no scheduler, input mode, or assertion changes.
        private void ObserveInputBoundary(string label,Keyboard keyboard,Gamepad pad,InputRouter router)
        {
            var raw="";
            try
            {
                raw=" space="+keyboard.spaceKey.ReadValue()+" d="+keyboard.dKey.ReadValue()+" enter="+keyboard.enterKey.ReadValue();
                if(pad!=null)raw+=" stick="+pad.leftStick.ReadValue();
            }
            catch(System.Exception error)
            {
                // Preserve unavailable native-state evidence; the test assertions remain authoritative.
                raw=" 원시 상태 읽기 오류="+error.GetType().Name+":"+error.Message;
            }
            var player=router==null?null:router.transform.root.GetComponentInChildren<PlayerMovementController>();
            var requester=router==null?null:router.transform.root.GetComponentInChildren<OrdanBossTerminalTransitionRequester>();
            var state=router==null?" 라우터 없음":" fault="+router.IsFaulted+" ready="+router.LocalPlayableStartReady+
                " receipt="+(router.CurrentReceipt.HasValue?router.CurrentReceipt.Value.Mode+"/"+router.CurrentReceipt.Value.Tick.Value:"none");
            if(player!=null)state+=" x="+player.transform.position.x+" y="+player.transform.position.y+" nextTick="+player.NextExpectedTick.Value;
            if(requester!=null && requester.LatestLocalPlayableView.HasValue)
                state+=" health="+requester.LatestLocalPlayableView.Value.PlayerHealth+" result="+requester.LatestLocalPlayableView.Value.Result;
            Debug.Log("입력관측 "+label+" updateMode="+InputSystem.settings.updateMode+" updateType="+InputState.currentUpdateType+
                " updateCount="+InputState.updateCount+" keyboardAdded="+keyboard.added+" padAdded="+(pad!=null&&pad.added)+
                " gameFixedTime="+Time.fixedTime+raw+state);
        }
        [TearDown] public void DestroyGraph() { if(_root!=null)Object.DestroyImmediate(_root); }
        [UnityTest] public IEnumerator AcLps001_NormalAuthoredStartPublishesCompletedHealth()
        {yield return _runtimeFixture.RunBody(AcLps001_NormalAuthoredStartPublishesCompletedHealthOriginalBody());}
        private IEnumerator AcLps001_NormalAuthoredStartPublishesCompletedHealthOriginalBody(){
            var keyboard=InputSystem.AddDevice<Keyboard>();
            _root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(LocalPath));
            var router=_root.GetComponentInChildren<InputRouter>();
            var requester=_root.GetComponentInChildren<OrdanBossTerminalTransitionRequester>();
            yield return null; InputSystem.Update(); yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            Assert.That(router.IsFaulted,Is.False);Assert.That(router.CurrentReceipt.HasValue,Is.True);
            Assert.That(requester.LatestLocalPlayableView.HasValue,Is.True);
            var view=requester.LatestLocalPlayableView.Value;
            Assert.That(view.PlayerHealth,Is.EqualTo(5));Assert.That(view.BossHealth,Is.EqualTo(60));
            Assert.That(view.Result,Is.EqualTo(LocalPlayableSliceResult.Playing));
        }
        [UnityTest] public IEnumerator AcLps004_HeldKeyboardAndGamepadBlockUntilProcessedNeutralUpdate()
        {yield return _runtimeFixture.RunBody(AcLps004_HeldKeyboardAndGamepadBlockUntilProcessedNeutralUpdateOriginalBody());}
        private IEnumerator AcLps004_HeldKeyboardAndGamepadBlockUntilProcessedNeutralUpdateOriginalBody(){
            var keyboard=InputSystem.AddDevice<Keyboard>();var pad=InputSystem.AddDevice<Gamepad>();
            ObserveInputBoundary("누른 상태 큐 이전",keyboard,pad,null);InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Space));InputSystem.QueueStateEvent(pad,new GamepadState { leftStick=new Vector2(.8f,0) });InputSystem.Update();ObserveInputBoundary("누른 상태 수동 갱신 이후",keyboard,pad,null);
            _root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(LocalPath));
            var router=_root.GetComponentInChildren<InputRouter>();
            yield return null; InputSystem.Update();yield return new WaitForFixedUpdate();
            ObserveInputBoundary("중립 전 고정 단계",keyboard,pad,router);Assert.That(router.LocalPlayableStartReady,Is.False);
            var releaseUpdate=InputState.updateCount;var releaseTick=router.CurrentReceipt.Value.Tick;
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.QueueStateEvent(pad,new GamepadState());
            ObserveInputBoundary("중립 큐 이후 갱신 전",keyboard,pad,router);InputSystem.Update();
            ObserveInputBoundary("중립 수동 갱신 이후",keyboard,pad,router);
            var processed=false;
            for(var boundary=0;boundary<4;boundary++)
            {
                yield return new WaitForFixedUpdate();
                if(InputState.updateCount>releaseUpdate && router.CurrentReceipt.HasValue && router.CurrentReceipt.Value.Tick.Value>releaseTick.Value)
                { processed=true;break; }
            }
            ObserveInputBoundary("중립 처리와 고정 단계 이후",keyboard,pad,router);
            Assert.That(processed,Is.True,"중립 상태를 처리한 입력 갱신 이후 고정 단계가 관측되지 않았습니다.");
            Assert.That(keyboard.spaceKey.ReadValue(),Is.Zero);Assert.That(pad.leftStick.ReadValue(),Is.EqualTo(Vector2.zero));
            Assert.That(router.LocalPlayableStartReady,Is.True);Assert.That(router.IsFaulted,Is.False);
        }
        [UnityTest] public IEnumerator AcLps004_DisableDoesNotCloseLifetimeButActualDestroyDoes()
        {yield return _runtimeFixture.RunBody(AcLps004_DisableDoesNotCloseLifetimeButActualDestroyDoesOriginalBody());}
        private IEnumerator AcLps004_DisableDoesNotCloseLifetimeButActualDestroyDoesOriginalBody(){
            InputSystem.AddDevice<Keyboard>();
            _root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(LocalPath));
            var router=_root.GetComponentInChildren<InputRouter>();var lifetime=router.LocalPlayableLifetime;
            yield return null;InputSystem.Update();yield return new WaitForFixedUpdate();
            _root.SetActive(false);Assert.That(lifetime.IsClosed,Is.False);
            Object.Destroy(_root);yield return null;Assert.That(_root==null,Is.True);Assert.That(lifetime.IsClosed,Is.True);
        }
        [UnityTest] public IEnumerator AcLps003_SyntheticBossDeathKeepsExactLegacyTransitionProof()
        {yield return _runtimeFixture.RunBody(AcLps003_SyntheticBossDeathKeepsExactLegacyTransitionProofOriginalBody());}
        private IEnumerator AcLps003_SyntheticBossDeathKeepsExactLegacyTransitionProofOriginalBody(){ yield return SyntheticTerminalBoundary(true,false,true); }
        [UnityTest] public IEnumerator AcLps005_SyntheticSimultaneousDeathLatchesFailureAndPreservesBossTriplet()
        {yield return _runtimeFixture.RunBody(AcLps005_SyntheticSimultaneousDeathLatchesFailureAndPreservesBossTripletOriginalBody());}
        private IEnumerator AcLps005_SyntheticSimultaneousDeathLatchesFailureAndPreservesBossTripletOriginalBody(){ yield return SyntheticTerminalBoundary(true,true,true); }
        [UnityTest] public IEnumerator AcLps005_SyntheticPlayerDeathNeverCreatesBossSuccessReceipt()
        {yield return _runtimeFixture.RunBody(AcLps005_SyntheticPlayerDeathNeverCreatesBossSuccessReceiptOriginalBody());}
        private IEnumerator AcLps005_SyntheticPlayerDeathNeverCreatesBossSuccessReceiptOriginalBody(){ yield return SyntheticTerminalBoundary(false,true,true); }
        [UnityTest] public IEnumerator AcLps009_DefaultFalseKeepsLegacySuccessWithoutLocalDisplay()
        {yield return _runtimeFixture.RunBody(AcLps009_DefaultFalseKeepsLegacySuccessWithoutLocalDisplayOriginalBody());}
        private IEnumerator AcLps009_DefaultFalseKeepsLegacySuccessWithoutLocalDisplayOriginalBody(){ yield return SyntheticTerminalBoundary(true,false,false); }
        private IEnumerator SyntheticTerminalBoundary(bool killBoss,bool killPlayer,bool local)
        {
            // REQ-LPS-005/006; AC-LPS-003/005/009. Synthetic damage is boundary evidence only.
            // Every phase retains normal Start/FixedUpdate ownership through terminal cleanup.
            InputSystem.AddDevice<Keyboard>();
            _root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab"));
            var router=_root.GetComponentInChildren<InputRouter>();
            var requester=_root.GetComponentInChildren<OrdanBossTerminalTransitionRequester>();
            var player=_root.GetComponentInChildren<PlayerMovementController>();
            var transfer=_root.GetComponentInChildren<TransferSimulationDriver>();
            var combat=_root.GetComponentInChildren<OrdanBossCombatSimulationDriver>();
            if(local)requester.ConfigureLocalPlayableForAuthoring(true);
            yield return null; InputSystem.Update(); yield return new WaitForFixedUpdate();
            Assert.That(router.IsFaulted,Is.False);
            var tick=player.NextExpectedTick;
            var requests=new System.Collections.Generic.List<DamageRequest>();
            if(killBoss)requests.Add(new DamageRequest("lps/synthetic-boss","test","ordan",60,DamageKind.BossPattern,tick));
            if(killPlayer)requests.Add(new DamageRequest("lps/synthetic-player","test","player",5,DamageKind.BossPattern,tick));
            combat.Submit(new OrdanBossCombatInput(tick,null,null,null,requests.ToArray()));
            yield return new WaitForFixedUpdate();
            Assert.That(combat.LatestOutcome.Tick,Is.EqualTo(tick));
            yield return new WaitForFixedUpdate();
            Assert.That(router.CurrentReceipt.Value.Mode,Is.EqualTo(InputMode.Transition));
            Assert.That(router.CurrentReceipt.Value.Tick.Value,Is.EqualTo(tick.Value+1));
            Assert.That(player.IsInputGateLocked && transfer.IsInputGateLocked && combat.IsInputGateLocked,Is.True);
            yield return new WaitForFixedUpdate();
            Assert.That(requester.LatestCompletionReceipt.HasValue,Is.EqualTo(killBoss));
            if(!local)Assert.That(requester.LatestLocalPlayableView.HasValue,Is.False);
            else
            {
                Assert.That(requester.LatestLocalPlayableView.HasValue,Is.True);
                Assert.That(requester.LatestLocalPlayableView.Value.Result,
                    Is.EqualTo(killPlayer?LocalPlayableSliceResult.Failed:LocalPlayableSliceResult.Succeeded));
                Assert.That(requester.LatestLocalPlayableView.Value.Tick,Is.EqualTo(tick));
                Assert.That(router.CurrentReceipt.Value.Mode,Is.EqualTo(InputMode.UIOnly));
                yield return new WaitForFixedUpdate();
                Assert.That(requester.LatestLocalPlayableView.Value.UiOnlyTick.HasValue,Is.True);
                var fixedResult=requester.LatestLocalPlayableView.Value;
                yield return new WaitForFixedUpdate();
                Assert.That(requester.LatestLocalPlayableView.Value.Tick,Is.EqualTo(fixedResult.Tick));
                Assert.That(requester.LatestLocalPlayableView.Value.PlayerHealth,Is.EqualTo(killPlayer?0:5));
            }
            Assert.That(router.IsFaulted,Is.False);
        }
        [UnityTest] public IEnumerator AcLps004_ProductionFailureUiRetryTwiceDestroysBeforeFreshGraph()
        {yield return _runtimeFixture.RunBody(AcLps004_ProductionFailureUiRetryTwiceDestroysBeforeFreshGraphOriginalBody());}
        private IEnumerator AcLps004_ProductionFailureUiRetryTwiceDestroysBeforeFreshGraphOriginalBody(){
            var keyboard=InputSystem.AddDevice<Keyboard>();InputSystem.AddDevice<Mouse>();
            yield return SceneManager.LoadSceneAsync("LocalPlayableSlice",LoadSceneMode.Single);
            yield return null;InputSystem.Update();yield return new WaitForFixedUpdate();
            for(var attempt=0;attempt<2;attempt++)
            {
                var router=Object.FindFirstObjectByType<InputRouter>();
                Assert.That(router,Is.Not.Null);var old=router.transform.root.gameObject;
                var requester=old.GetComponentInChildren<OrdanBossTerminalTransitionRequester>();
                var player=old.GetComponentInChildren<PlayerMovementController>();
                InputSystem.Update();yield return new WaitForFixedUpdate();InputSystem.Update();yield return new WaitForFixedUpdate();
                ObserveInputBoundary("이동 큐 이전 "+attempt,keyboard,null,router);InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.D));InputSystem.Update();ObserveInputBoundary("이동 수동 갱신 이후 "+attempt,keyboard,null,router);
                for(var step=0;step<180 && player.transform.position.x<5f;step++)
                { InputSystem.Update();yield return new WaitForFixedUpdate();if(step==0||step==1||step==179)ObserveInputBoundary("이동 고정 단계 "+step,keyboard,null,router); }
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();
                ObserveInputBoundary("이동 종료",keyboard,null,router);var reached=false;var jumpEdges=0;var releaseEdges=0;
                for(var step=0;step<1200;step++)
                {
                    // Stage1 cycle 594: DebtLineB at 336/930, contact damage at 337/931.
                    // Real jump callbacks put the hurtbox across y=3; no health/damage injection.
                    var nextTick=player.NextExpectedTick.Value;
                    if(nextTick==326||nextTick==920)
                    { InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Space));jumpEdges++;ObserveInputBoundary("부채선 높이 진입 점프 "+nextTick,keyboard,null,router); }
                    if(nextTick==338||nextTick==932)
                    { InputSystem.QueueStateEvent(keyboard,new KeyboardState());releaseEdges++;ObserveInputBoundary("부채선 이후 점프 해제 "+nextTick,keyboard,null,router); }
                    InputSystem.Update();yield return new WaitForFixedUpdate();
                    if(step==0||step==1||step==1199||nextTick==336||nextTick==337||nextTick==930||nextTick==931)ObserveInputBoundary("사망 대기 고정 단계 "+step,keyboard,null,router);
                    if(requester.LatestLocalPlayableView.HasValue && requester.LatestLocalPlayableView.Value.UiOnlyTick.HasValue)
                    { reached=true;break; }
                }
                Assert.That(jumpEdges,Is.EqualTo(2),"정확한 두 점프 시작 tick이 관측되지 않았습니다.");Assert.That(releaseEdges,Is.EqualTo(2),"정확한 두 점프 해제 tick이 관측되지 않았습니다.");
                Assert.That(reached,Is.True,"Production boss damage did not produce a bounded failure result.");
                Assert.That(requester.LatestLocalPlayableView.Value.Result,Is.EqualTo(LocalPlayableSliceResult.Failed));
                Assert.That(requester.LatestCompletionReceipt.HasValue,Is.False);
                var lifetime=router.LocalPlayableLifetime;
                InputSystem.Update();yield return new WaitForFixedUpdate();
                InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.Enter));InputSystem.Update();yield return new WaitForFixedUpdate();
                // Destroy is deferred: no second graph is allowed beside the old graph.
                Assert.That(Object.FindObjectsByType<InputRouter>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length,Is.LessThanOrEqualTo(1));
                yield return null;InputSystem.Update();yield return new WaitForFixedUpdate();
                Assert.That(old==null,Is.True);Assert.That(lifetime.IsClosed,Is.True);
                var fresh=Object.FindFirstObjectByType<InputRouter>();Assert.That(fresh,Is.Not.Null);Assert.That(fresh,Is.Not.SameAs(router));
                Assert.That(fresh.LocalPlayableStartReady,Is.False,"Held Submit must quarantine the new graph.");
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();yield return new WaitForFixedUpdate();
                InputSystem.Update();yield return new WaitForFixedUpdate();
                Assert.That(fresh.IsFaulted,Is.False);
            }
            var active=SceneManager.GetActiveScene();var exit=SceneManager.CreateScene("LocalInputTestExit");SceneManager.SetActiveScene(exit);
            yield return SceneManager.UnloadSceneAsync(active);
        }
        [Test] public void AcLps003_ResultSnapshotIsImmutableAndKeepsSourceTickWhenUiReceiptArrives()
        {
            var source=new LocalPlayableSliceView(1,new SimulationTick(8),0,0,"",false,LocalPlayableSliceResult.Failed,null);
            var result=source.WithUiOnlyTick(new SimulationTick(10)).WithGeneration(2);
            Assert.That(source.UiOnlyTick.HasValue,Is.False);Assert.That(result.Tick.Value,Is.EqualTo(8));
            Assert.That(result.PlayerHealth,Is.Zero);Assert.That(result.BossHealth,Is.Zero);Assert.That(result.Generation,Is.EqualTo(2));
            Assert.That(result.Result,Is.EqualTo(LocalPlayableSliceResult.Failed));
        }
    }
}
