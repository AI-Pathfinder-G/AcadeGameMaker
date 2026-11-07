using System;
using System.Collections;
using AcadeGameMaker.Core;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Combat.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // AC-C1B-ENTRY-001: actual Awake/Start/FixedUpdate and actual destroy/dispose.
    internal sealed class Chapter1EntryPlayModeTests : InputTestFixture
    {
        [UnityEngine.SerializeField] private AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope _runtimeFixture=new AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope();
        [UnityEngine.TestTools.UnitySetUp] public System.Collections.IEnumerator BeginRuntimeCase(){_runtimeFixture.PrepareEnter();yield return new UnityEngine.TestTools.EnterPlayMode();_runtimeFixture.ConfirmEntered();yield return _runtimeFixture.RunBody(_runtimeFixture.BeginCase(false));}
        public override void TearDown(){}
        [UnityEngine.TestTools.UnityTearDown] public System.Collections.IEnumerator CloseRuntimeCase(){try{if(_runtimeFixture.OwnsRuntimeFixture && UnityEngine.Application.isPlaying)yield return _runtimeFixture.RunBody(_runtimeFixture.CloseCase(),true);}finally{_runtimeFixture.CaptureCleanupFailure(()=>base.TearDown());}if(_runtimeFixture.OwnsRuntimeFixture){if(UnityEngine.Application.isPlaying)yield return new UnityEngine.TestTools.ExitPlayMode();_runtimeFixture.CaptureCleanupFailure(_runtimeFixture.CompleteExit);}_runtimeFixture.ThrowCleanupFailure();}
        

        private const string Path="Assets/Prefabs/Chapter1/Chapter1RegularEncounter.prefab";
        private GameObject _ownerObject,_inactive;
        private Chapter1EncounterLifetimeOwner _owner;
        private GameObject Prefab=>AssetDatabase.LoadAssetAtPath<GameObject>(Path);
        private void Begin(){InputSystem.AddDevice<Keyboard>();Assert.That(Prefab,Is.Not.Null);_ownerObject=new GameObject("Chapter1Owner");_owner=_ownerObject.AddComponent<Chapter1EncounterLifetimeOwner>();_owner.BeginAuthoredRegularEncounter(Prefab);}
        // The normal entry is the saved host, never direct test-owned Begin.
        private void BeginHost()
        {
            InputSystem.AddDevice<Keyboard>();var host=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/Chapter1RegularEncounterHost.prefab");Assert.That(host,Is.Not.Null);
            _ownerObject=UnityEngine.Object.Instantiate(host);_owner=_ownerObject.GetComponent<Chapter1EncounterLifetimeOwner>();
            Assert.That(_owner,Is.Not.Null);Assert.That(_owner.InitialPrefabForAuthoring,Is.EqualTo(Prefab));Assert.That(_ownerObject.transform.childCount,Is.Zero);Assert.That(_owner.CurrentRouter,Is.Null);
        }
        private IEnumerator Completed(){for(var i=0;i<30;i++){InputSystem.Update();yield return new WaitForFixedUpdate();if(_owner.TryGetCompletedEvidence(out _))yield break;}Assert.Fail("No actual completed regular tick.");}
        [TearDown] public void Cleanup(){if(_owner!=null && _owner.CurrentRouter!=null)UnityEngine.Object.DestroyImmediate(_owner.CurrentRouter.transform.root.gameObject);if(_ownerObject!=null)UnityEngine.Object.DestroyImmediate(_ownerObject);if(_inactive!=null)UnityEngine.Object.DestroyImmediate(_inactive);}
        [UnityTest] public IEnumerator SeedsAllProducersAtOneTick()
        {yield return _runtimeFixture.RunBody(SeedsAllProducersAtOneTickOriginalBody());}
        private IEnumerator SeedsAllProducersAtOneTickOriginalBody(){
            var stage="not-started";var rows=0;GameObject syntheticGraph=null;
            string ReadEntry(Func<string> read)
            {try{var value=read()??"<null>";return value.Length>256?value.Substring(0,240)+"[item-truncated]":value;}catch(Exception error){return "unobserved:"+error.GetType().Name;}}
            void ObserveEntry(string nextStage)
            {
                stage=nextStage;if(rows>=6)return;rows++;
                try
                {
                    var router=_owner==null?null:_owner.CurrentRouter;var graph=syntheticGraph!=null?syntheticGraph:router==null?null:router.transform.parent.gameObject;
                    var text="[CH1EntryStage] stage="+stage+" row="+rows+" frame="+Time.frameCount+" fixed="+Time.fixedTimeAsDouble.ToString("R")+" owner="+ReadEntry(()=>_owner.GetEntityId().ToString())+" router="+ReadEntry(()=>router.GetEntityId().ToString());
                    if(graph==null)text+=" graph=unpublished";
                    else
                    {
                        text+=" graph="+ReadEntry(()=>graph.GetEntityId()+"/name="+graph.name+"/scene="+graph.scene.name+"/handle="+graph.scene.handle.GetRawData()+"/active="+graph.activeInHierarchy);
                        var player=graph.GetComponentInChildren<PlayerMovementController>(true);
                        text+=" player="+ReadEntry(()=>player.GetEntityId()+"/active="+player.isActiveAndEnabled)+" motion="+ReadEntry(()=>{if(player==null || !player.TryReadExistingMotionForAuthoring(out var motion,out var next))return "unobserved:no-existing-active-motor";return "snapshotTickRaw="+motion.Tick.Value+"/nextTick="+next.Value;})+" configuredTick="+ReadEntry(()=>{var configured=player.Chapter1ConfiguredFirstTick;return configured.HasValue?configured.Value.Value.ToString():"unconfigured";});
                        var cameras=graph.GetComponentsInChildren<AcadeGameMaker.Camera.Unity.SimulationCameraDriver>(true);text+=" cameraCount="+cameras.Length;
                        for(var i=0;i<Math.Min(2,cameras.Length);i++)
                        {
                            var camera=cameras[i];text+=" camera["+i+"]="+ReadEntry(()=>camera.GetEntityId()+"/active="+camera.isActiveAndEnabled+"/scene="+camera.gameObject.scene.name+"/boundPlayer="+camera.BoundPlayer.GetEntityId()+"/samePlayer="+ReferenceEquals(camera.BoundPlayer,player)+"/fault="+camera.IsFaulted)+" cameraNext="+ReadEntry(()=>camera.NextExpectedTick.Value.ToString())+" diagnostic="+ReadEntry(()=>camera.LatestDiagnostic.Tick.Value+"/disposition="+camera.LatestDiagnostic.Disposition+"/historical="+camera.LatestDiagnostic.HasHistoricalPublication);
                        }
                    }
                    var truncated=false;while(System.Text.Encoding.UTF8.GetByteCount(text)>4000){text=text.Substring(0,text.Length/2);truncated=true;}Debug.Log(text+" recordTruncated="+truncated);
                }
                catch(Exception error){try{Debug.Log("[CH1EntryStage] stage="+stage+" observationFailed="+error.GetType().Name+" originalFailurePreserved=true");}catch{}}
            }
            try
            {BeginHost();ObserveEntry("host-before-first-yield");yield return null;ObserveEntry("host-after-first-yield");yield return Completed();ObserveEntry("host-completed");Assert.That(_owner.TryGetCompletedEvidence(out var view),Is.True);var router=_owner.CurrentRouter;Assert.That(router.BoundPlayerForAuthoring.Snapshot.Tick,Is.EqualTo(view.Tick));Assert.That(router.BoundTransferForAuthoring.LatestPhasePublication.Value.Tick,Is.EqualTo(view.Tick));Assert.That(router.BoundChapter1Combat.LatestOutcomeForEncounterHandoff.Tick,Is.EqualTo(view.Tick));Assert.That(view.Generation,Is.EqualTo(1));
            // AC-C1B-HP-001: initial Ordan registration only, not a regular-to-boss transition.
            UnityEngine.Object.DestroyImmediate(_owner.CurrentRouter.transform.root.gameObject);UnityEngine.Object.DestroyImmediate(_ownerObject);_owner=null;_ownerObject=null;
            _inactive=new GameObject("InactiveBossStage");_inactive.SetActive(false);
            var bossPrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab");Assert.That(bossPrefab,Is.Not.Null);
            var bossGraph=UnityEngine.Object.Instantiate(bossPrefab,_inactive.transform,false);syntheticGraph=bossGraph;
            var player=bossGraph.GetComponentInChildren<PlayerMovementController>(true);var transfer=bossGraph.GetComponentInChildren<TransferSimulationDriver>(true);var boss=bossGraph.GetComponentInChildren<OrdanBossCombatSimulationDriver>(true);
            var seed=new SimulationTick(20);player.ConfigureChapter1EntryForAuthoring(seed,new Vector2(player.transform.position.x,player.transform.position.y));transfer.ConfigureChapter1EntryForAuthoring(seed);boss.ConfigureChapter1CarriedPlayerForAuthoring(seed,3,100);
            ObserveEntry("synthetic-before-activation");_inactive.SetActive(true);Assert.That(player.Snapshot.Tick,Is.EqualTo(seed));Assert.That(player.NextExpectedTick,Is.EqualTo(seed));
            var initialCamera=bossGraph.GetComponentInChildren<AcadeGameMaker.Camera.Unity.SimulationCameraDriver>(true);Assert.That(initialCamera,Is.Not.Null);initialCamera.InitializeForTests();ObserveEntry("synthetic-after-explicit-initialization");
            InputSystem.Update();yield return new WaitForFixedUpdate();
            Assert.That(boss.HasLatestOutcome,Is.True);Assert.That(boss.LatestOutcome.Tick.Value,Is.EqualTo(20));
            var found=false;foreach(var value in boss.LatestOutcome.Combatants)if(value.TargetId=="player"){found=true;Assert.That(value.CurrentHealth,Is.EqualTo(3));Assert.That(value.InvulnerabilityEndTick,Is.EqualTo(100));}Assert.That(found,Is.True);InputSystem.Update();yield return new WaitForFixedUpdate();Assert.That(boss.LatestOutcome.Tick.Value,Is.EqualTo(21));
        
            }
            finally{ObserveEntry("finally-after-"+stage);}
        }
        private static readonly int[] CarriedEntryHealthCases={1,2,3,4,5};
        // REQ/AC-AUTOCH1-CARRYBASELINE-001: real first publications and separate pure contract negatives.
        [UnityTest] public IEnumerator CarriedPlayerHealthSeedsLifecycleAtExactEntry([ValueSource(nameof(CarriedEntryHealthCases))] int health)
        {yield return _runtimeFixture.RunBody(CarriedPlayerHealthSeedsLifecycleAtExactEntryOriginalBody(health));}
        private IEnumerator CarriedPlayerHealthSeedsLifecycleAtExactEntryOriginalBody(int health)
        {
            var first=new SimulationTick(20);var second=new SimulationTick(21);
            RegularEnemyEncounterRoleState[] Roles(int hp)=>new[]{new RegularEnemyEncounterRoleState("player",hp,hp==0),new RegularEnemyEncounterRoleState("surveyor",6,false),new RegularEnemyEncounterRoleState("walker",9,false)};
            var emptyRequests=Array.Empty<DamageRequest>();var emptyResults=Array.Empty<DamageResult>();
            RegularEnemyEncounterLifecycleInput Empty(SimulationTick tick,int hp)=>new RegularEnemyEncounterLifecycleInput(tick,RegularEnemyEncounterLifecycleInputKind.Gameplay,emptyRequests,emptyResults,Roles(hp));
            Assert.Throws<ArgumentOutOfRangeException>(()=>new RegularEnemyEncounterLifecycleSession(first,0));Assert.Throws<ArgumentOutOfRangeException>(()=>new RegularEnemyEncounterLifecycleSession(first,6));
            var lifecycle=new RegularEnemyEncounterLifecycleSession(first,health);
            Assert.Throws<InvalidOperationException>(()=>lifecycle.PreviewNext(Empty(first,health-1)));Assert.That(lifecycle.NextExpectedTick,Is.EqualTo(first));Assert.That(lifecycle.LatestSnapshot.HasValue,Is.False);
            var initial=Empty(first,health);lifecycle.CommitNext(initial,lifecycle.PreviewNext(initial));Assert.That(lifecycle.LatestSnapshot.Value.Roles[0].CurrentHealth,Is.EqualTo(health));
            Assert.Throws<InvalidOperationException>(()=>lifecycle.PreviewNext(Empty(second,health-1)));Assert.That(lifecycle.NextExpectedTick,Is.EqualTo(second));
            var m1=new CombatSession(first);m1.RegisterCarriedPlayer(new CombatantRegistration("player",5,45),health,-1);m1.Process(first,emptyRequests);
            var damage=new[]{new DamageRequest("carry-boundary-hit","surveyor","player",1,DamageKind.EnemyRanged,second)};var results=m1.Process(second,damage);
            Assert.That(results[0].Result,Is.EqualTo(DamageResultCode.Applied));Assert.That(results[0].AppliedAmount,Is.EqualTo(1));
            var wrong=new RegularEnemyEncounterLifecycleInput(second,RegularEnemyEncounterLifecycleInputKind.Gameplay,damage,results,Roles(health));Assert.Throws<InvalidOperationException>(()=>lifecycle.PreviewNext(wrong));Assert.That(lifecycle.NextExpectedTick,Is.EqualTo(second));
            var correct=new RegularEnemyEncounterLifecycleInput(second,RegularEnemyEncounterLifecycleInputKind.Gameplay,damage,results,Roles(m1.Snapshot("player").CurrentHealth));lifecycle.CommitNext(correct,lifecycle.PreviewNext(correct));Assert.That(lifecycle.LatestSnapshot.Value.Roles[0].CurrentHealth,Is.EqualTo(health-1));
            var resetOwner=new RegularEnemyEncounterLifecycleSession(first,health);var reset=new RegularEnemyEncounterLifecycleInput(first,RegularEnemyEncounterLifecycleInputKind.EncounterReset,emptyRequests,emptyResults,Roles(5));resetOwner.CommitNext(reset,resetOwner.PreviewNext(reset));Assert.That(resetOwner.LatestSnapshot.Value.Roles[0].CurrentHealth,Is.EqualTo(5));
            var defaultOwner=new RegularEnemyEncounterLifecycleSession(first);var full=Empty(first,5);defaultOwner.CommitNext(full,defaultOwner.PreviewNext(full));Assert.That(defaultOwner.LatestSnapshot.Value.Roles[0].CurrentHealth,Is.EqualTo(5));
            InputSystem.AddDevice<Keyboard>();_inactive=new GameObject("CarriedLifecycleBoundaryStage");_inactive.SetActive(false);
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Combat/CombatEncounterGraph.prefab");Assert.That(prefab,Is.Not.Null);var graph=UnityEngine.Object.Instantiate(prefab,_inactive.transform,false);
            var player=graph.GetComponentInChildren<PlayerMovementController>(true);var transfer=graph.GetComponentInChildren<TransferSimulationDriver>(true);var combat=graph.GetComponentInChildren<CombatSimulationDriver>(true);var handoff=graph.GetComponentInChildren<RegularEnemyEncounterHandoffAdapter>(true);
            Assert.That(combat.ReadEncounterInitialPlayerHealth(first),Is.EqualTo(5));Assert.That(handoff.HasOwnedLifecycleForEncounterHandoff,Is.False);
            player.ConfigureChapter1EntryForAuthoring(first,new Vector2(player.transform.position.x,player.transform.position.y));transfer.ConfigureChapter1EntryForAuthoring(first);combat.ConfigureChapter1CarriedPlayerForAuthoring(first,health,-1);
            Assert.That(combat.ReadEncounterInitialPlayerHealth(first),Is.EqualTo(health));Assert.That(combat.ReadEncounterInitialPlayerHealth(first),Is.EqualTo(health));
            Assert.Throws<InvalidOperationException>(()=>combat.ReadEncounterInitialPlayerHealth(new SimulationTick(19)));Assert.Throws<InvalidOperationException>(()=>combat.ReadEncounterInitialPlayerHealth(second));Assert.That(handoff.HasOwnedLifecycleForEncounterHandoff,Is.False);
            graph.SetActive(true);_inactive.SetActive(true);
            for(var tick=20;tick<=21;tick++)
            {
                InputSystem.Update();yield return new WaitForFixedUpdate();
                Assert.That(combat.HasLatestOutcomeForEncounterHandoff,Is.True);var actual=combat.LatestOutcomeForEncounterHandoff;Assert.That(actual.Tick.Value,Is.EqualTo(tick));Assert.That(actual.CanonicalRequestTrace,Is.Empty);Assert.That(actual.DamageResults,Is.Empty);
                var found=false;foreach(var role in actual.Combatants)if(role.TargetId=="player"){found=true;Assert.That(role.CurrentHealth,Is.EqualTo(health));Assert.That(role.IsDead,Is.False);}Assert.That(found,Is.True);
                Assert.That(handoff.HasOwnedLifecycleForEncounterHandoff,Is.True);Assert.That(handoff.LatestPresentationView.HasValue,Is.True);var view=handoff.LatestPresentationView.Value;Assert.That(view.Tick,Is.EqualTo(actual.Tick));Assert.That(view.NextExpectedTick.Value,Is.EqualTo(tick+1));Assert.That(view.PlayerAlive,Is.True);Assert.That(view.AppliedHitTargetIds,Is.Empty);
            }
        }
        [Test] public void RejectLateOrMismatchedSeed(){_inactive=new GameObject("InactivePlayer");_inactive.SetActive(false);_inactive.AddComponent<Rigidbody2D>();_inactive.AddComponent<CapsuleCollider2D>();var player=_inactive.AddComponent<PlayerMovementController>();player.ConfigureChapter1EntryForAuthoring(new SimulationTick(10),Vector2.zero);Assert.Throws<InvalidOperationException>(()=>player.ConfigureChapter1EntryForAuthoring(new SimulationTick(11),Vector2.zero));Assert.Throws<ArgumentException>(()=>new Chapter1EncounterEntry(null,"run",1,new SimulationTick(0),Vector2.zero,5,-1));}
        [Test] public void RejectOverlappingGeneration(){Begin();Assert.Throws<InvalidOperationException>(()=>_owner.BeginAuthoredRegularEncounter(Prefab));Assert.Throws<InvalidOperationException>(()=>_owner.RequestRegularReplacement(Prefab));}
        [UnityTest] public IEnumerator InactiveConstructionSeedsBeforeAwake(){yield return _runtimeFixture.RunBody(InactiveConstructionSeedsBeforeAwakeOriginalBody());}
        private IEnumerator InactiveConstructionSeedsBeforeAwakeOriginalBody(){BeginHost();yield return null;var router=_owner.CurrentRouter;Assert.That(router,Is.Not.Null);Assert.That(router.BoundPlayerForAuthoring.Chapter1ConfiguredFirstTick,Is.EqualTo(new SimulationTick(0)));Assert.That(router.BoundTransferForAuthoring.Chapter1ConfiguredFirstTick,Is.EqualTo(new SimulationTick(0)));Assert.That(router.BoundChapter1Combat.Chapter1ConfiguredCarryTick,Is.EqualTo(new SimulationTick(0)));yield return Completed();Assert.That(_owner.IsFailed,Is.False);}
        // AC-C1B-ENTRY-001: synthetic authoring rejection fixtures only.
        [Test] public void HostRejectsWrongPrefab()
        {
            _ownerObject=new GameObject("InactiveHost");_ownerObject.SetActive(false);_owner=_ownerObject.AddComponent<Chapter1EncounterLifetimeOwner>();
            _inactive=new GameObject("WrongGraph");_inactive.SetActive(false);
            Assert.Throws<InvalidOperationException>(()=>_owner.ConfigureInitialPrefabForAuthoring(_inactive));Assert.That(_owner.InitialPrefabForAuthoring,Is.Null);
        }
        [Test] public void HostRejectsDuplicateConfiguration()
        {
            _ownerObject=new GameObject("InactiveHost");_ownerObject.SetActive(false);_owner=_ownerObject.AddComponent<Chapter1EncounterLifetimeOwner>();
            Assert.That(Prefab,Is.Not.Null);_owner.ConfigureInitialPrefabForAuthoring(Prefab);
            Assert.Throws<InvalidOperationException>(()=>_owner.ConfigureInitialPrefabForAuthoring(Prefab));Assert.That(_owner.InitialPrefabForAuthoring,Is.EqualTo(Prefab));
        }
        [Test] public void HostRejectsNestedOwner()
        {
            _ownerObject=new GameObject("InactiveHost");_ownerObject.SetActive(false);_owner=_ownerObject.AddComponent<Chapter1EncounterLifetimeOwner>();
            Assert.That(Prefab,Is.Not.Null);_inactive=UnityEngine.Object.Instantiate(Prefab);_inactive.AddComponent<Chapter1EncounterLifetimeOwner>();
            Assert.Throws<InvalidOperationException>(()=>_owner.ConfigureInitialPrefabForAuthoring(_inactive));Assert.That(_owner.InitialPrefabForAuthoring,Is.Null);
        }
        [Test] public void RejectAlreadyInitializedEntry(){_inactive=new GameObject("InactivePlayer");_inactive.SetActive(false);_inactive.AddComponent<Rigidbody2D>();_inactive.AddComponent<CapsuleCollider2D>();var player=_inactive.AddComponent<PlayerMovementController>();var snapshot=player.Snapshot;Assert.Throws<InvalidOperationException>(()=>player.ConfigureChapter1EntryForAuthoring(new SimulationTick(10),Vector2.zero));}
        [UnityTest] public IEnumerator ReplacementWaitsForDestroyAndActionsDispose()
        {yield return _runtimeFixture.RunBody(ReplacementWaitsForDestroyAndActionsDisposeOriginalBody());}
        private IEnumerator ReplacementWaitsForDestroyAndActionsDisposeOriginalBody(){
            BeginHost();yield return null;yield return Completed();var old=_owner.CurrentRouter;var root=old.transform.root.gameObject;var lifetime=old.Chapter1Lifetime;
            _owner.TryGetCompletedEvidence(out var before);_owner.RequestRegularReplacement(Prefab);
            Assert.Throws<InvalidOperationException>(()=>_owner.RequestRegularReplacement(Prefab));
            for(var i=0;i<60 && _owner.Generation==1;i++){InputSystem.Update();yield return new WaitForFixedUpdate();yield return null;if(root!=null)Assert.That(_owner.Generation,Is.EqualTo(1));}
            Assert.That(root==null,Is.True);Assert.That(lifetime.IsClosed,Is.True);Assert.That(_owner.Generation,Is.EqualTo(2));Assert.That(_owner.IsFailed,Is.False);
            yield return Completed();_owner.TryGetCompletedEvidence(out var next);Assert.That(next.RunKey,Is.EqualTo(before.RunKey));Assert.That(next.Tick.Value,Is.GreaterThan(before.Tick.Value));Assert.That(next.Health,Is.EqualTo(before.Health));
        }
        [UnityTest] public IEnumerator RejectForgedOrUnclosedPredecessor(){yield return _runtimeFixture.RunBody(RejectForgedOrUnclosedPredecessorOriginalBody());}
        private IEnumerator RejectForgedOrUnclosedPredecessorOriginalBody(){BeginHost();yield return null;yield return Completed();var router=_owner.CurrentRouter;var lifetime=router.Chapter1Lifetime;Assert.That(lifetime.IsClosed,Is.False);Assert.Throws<InvalidOperationException>(()=>router.ConfigureChapter1GameplayForAuthoring(new Chapter1EncounterEntry(_owner,"foreign",2,new SimulationTick(0),Vector2.zero,5,-1),_owner));Assert.That(_owner.Generation,Is.EqualTo(1));}
        // AC-C1B-HP-001: 실제 기본 그래프 최초0/다음1. 직접 phase 호출이 아니다.
        [UnityTest] public IEnumerator AuthoredBossBootstrapZeroAndOne()
        {yield return _runtimeFixture.RunBody(AuthoredBossBootstrapZeroAndOneOriginalBody());}
        private IEnumerator AuthoredBossBootstrapZeroAndOneOriginalBody(){
            InputSystem.AddDevice<Keyboard>();_inactive=new GameObject("BossZeroStage");_inactive.SetActive(false);
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab");Assert.That(prefab,Is.Not.Null);
            var graph=UnityEngine.Object.Instantiate(prefab,_inactive.transform,false);var boss=graph.GetComponentInChildren<OrdanBossCombatSimulationDriver>(true);
            _inactive.SetActive(true);InputSystem.Update();yield return new WaitForFixedUpdate();Assert.That(boss.LatestOutcome.Tick.Value,Is.EqualTo(0));
            InputSystem.Update();yield return new WaitForFixedUpdate();Assert.That(boss.LatestOutcome.Tick.Value,Is.EqualTo(1));
        }
        // 합성 음성 phase 설정: 실제 완료 movement를 prior 슬롯에 다시 소비하지 못해야 한다.
        [UnityTest] public IEnumerator ProducersRejectCompletedMovementAsPriorSnapshot()
        {yield return _runtimeFixture.RunBody(ProducersRejectCompletedMovementAsPriorSnapshotOriginalBody());}
        private IEnumerator ProducersRejectCompletedMovementAsPriorSnapshotOriginalBody(){
            InputSystem.AddDevice<Keyboard>();_inactive=new GameObject("BossWrongPriorStage");_inactive.SetActive(false);
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab");Assert.That(prefab,Is.Not.Null);
            var graph=UnityEngine.Object.Instantiate(prefab,_inactive.transform,false);
            var player=graph.GetComponentInChildren<PlayerMovementController>(true);var transfer=graph.GetComponentInChildren<TransferSimulationDriver>(true);var boss=graph.GetComponentInChildren<OrdanBossCombatSimulationDriver>(true);
            var first=new SimulationTick(20);player.ConfigureChapter1EntryForAuthoring(first,new Vector2(player.transform.position.x,player.transform.position.y));transfer.ConfigureChapter1EntryForAuthoring(first);boss.ConfigureChapter1CarriedPlayerForAuthoring(first,3,100);
            // REQ/AC-AUTOCH1-G5PRIOR-001: this prior-publication negative does not consume handoff.
            var handoff=graph.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>(true);Assert.That(handoff,Is.Not.Null);handoff.enabled=false;
            _inactive.SetActive(true);InputSystem.Update();yield return new WaitForFixedUpdate();
            var hostile=graph.GetComponentInChildren<OrdanBossHostileDamageProducer>(true);var pull=graph.GetComponentInChildren<OrdanBossBalanceAuditPullProducer>(true);
            hostile.enabled=false;pull.enabled=false;Assert.That(hostile.LatestOutcome.Tick.Value,Is.EqualTo(20));Assert.That(pull.LatestOutcome.SourceTick.Value,Is.EqualTo(20));
            var hostileTick=hostile.LatestOutcome.Tick;var pullTick=pull.LatestOutcome.SourceTick;
            Assert.Throws<InvalidOperationException>(()=>hostile.AdvanceForTests());Assert.Throws<InvalidOperationException>(()=>pull.AdvanceForTests());
            Assert.That(hostile.LatestOutcome.Tick,Is.EqualTo(hostileTick));Assert.That(pull.LatestOutcome.SourceTick,Is.EqualTo(pullTick));
        }
        // 합성 저작 음성: 다른 실제 player 및 미발행 upstream을 거절한다. 정상 종단 대체0.
        [Test] public void ProducersRejectForeignPlayerOrMissingUpstreamWithoutPublication()
        {
            _inactive=new GameObject("BossForeignStage");_inactive.SetActive(false);
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab");Assert.That(prefab,Is.Not.Null);
            var graph=UnityEngine.Object.Instantiate(prefab,_inactive.transform,false);var foreign=UnityEngine.Object.Instantiate(prefab,_inactive.transform,false);var missing=UnityEngine.Object.Instantiate(prefab,_inactive.transform,false);
            var hostile=graph.GetComponentInChildren<OrdanBossHostileDamageProducer>(true);var pull=graph.GetComponentInChildren<OrdanBossBalanceAuditPullProducer>(true);
            var missingHostile=missing.GetComponentInChildren<OrdanBossHostileDamageProducer>(true);var missingPull=missing.GetComponentInChildren<OrdanBossBalanceAuditPullProducer>(true);
            Assert.Throws<InvalidOperationException>(()=>missingHostile.AdvanceForTests());Assert.Throws<InvalidOperationException>(()=>missingPull.AdvanceForTests());
            Assert.That(missingHostile.HasLatestOutcome,Is.False);Assert.That(missingPull.HasLatestOutcome,Is.False);
            var otherPlayer=foreign.GetComponentInChildren<PlayerMovementController>(true);
            hostile.ConfigureForAuthoring(otherPlayer,hostile.BoundTransfer,hostile.BoundCombat,hostile.BoundBridge,hostile.BoundScheduler);
            pull.ConfigureForAuthoring(otherPlayer,pull.BoundTransfer,pull.BoundCombat,pull.BoundBridge,pull.BoundScheduler);
            Assert.Throws<InvalidOperationException>(()=>hostile.AdvanceForTests());Assert.Throws<InvalidOperationException>(()=>pull.AdvanceForTests());
            Assert.That(hostile.HasLatestOutcome,Is.False);Assert.That(pull.HasLatestOutcome,Is.False);
        }
    }
}