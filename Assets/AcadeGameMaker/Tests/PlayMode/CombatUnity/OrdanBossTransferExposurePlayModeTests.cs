// AC-COM-002, AC-COM-003, AC-COM-004; affected AC-WT-003, AC-WT-005.
using System;
using System.Collections.Generic;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    internal sealed class OrdanBossTransferExposurePlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        [TearDown] public void TearDown(){for(var i=_objects.Count-1;i>=0;i--)UnityEngine.Object.DestroyImmediate(_objects[i]);_objects.Clear();}

        // AC-COM-002, AC-WT-005. A real four-owner trace proves that M4A's
        // forecast makes age zero selectable, then clears only temporarily.
        [Test] public void ForecastExposesTelegraphZeroThroughFiftyNineThenClearsAndCycles()
        {
            var r=CreateRig();var chunks=new List<string>();var current=string.Empty;var count=0;var targetZeroWindows=0;var reuseNext=false;var reused=false;
            for(var i=0;i<2100;i++){
                var tick=r.Player.NextExpectedTick;if(tick.Value==108||reuseNext)r.Transfer.SubmitForTests(Press(tick));
                var transfer=r.Transfer.AdvanceTransferPhaseForTests();var combat=r.Combat.AdvanceCombatPhaseForTests();var bridge=r.Bridge.AdvanceBridgePhaseForTests();var exposure=r.Scheduler.AdvanceSchedulerPhaseForTests();
                if(reuseNext){Assert.That(transfer.PressResult.Value.Success,Is.True);Assert.That(transfer.PressResult.Value.SelectedTargetId,Is.EqualTo("BossWeight-0"));reused=true;reuseNext=false;}
                Assert.That(transfer.Snapshot.Tick,Is.EqualTo(tick));Assert.That(combat.Tick,Is.EqualTo(tick));Assert.That(bridge.SourceTick,Is.EqualTo(tick));Assert.That(exposure.SourceTick,Is.EqualTo(tick));Assert.That(Enabled(r.Colliders),Is.EqualTo(exposure.ExposedTargetId.Length==0?0:1));
                if(exposure.ExposedTargetId=="BossWeight-0"&&current!="BossWeight-0"){targetZeroWindows++;if(targetZeroWindows==2)reuseNext=true;}
                if(exposure.ExposedTargetId==current)count++;else{if(current.Length>0)chunks.Add(current+":"+count);current=exposure.ExposedTargetId;count=1;}
                if(tick.Value==107)Assert.That(exposure.ExposedTargetId,Is.EqualTo("BossWeight-0"));
                if(tick.Value==167)Assert.That(exposure.ExposedTargetId,Is.Empty);
                if(tick.Value==168){Assert.That(transfer.Snapshot.PlayerState,Is.EqualTo(TransferPlayerState.Baseline));Assert.That(r.Sinks[0].IsApplied,Is.False);}
                r.Player.AdvanceOneFixedTickForTests();}
            if(current.Length>0)chunks.Add(current+":"+count);Assert.That(chunks.GetRange(0,4),Is.EqualTo(new[]{"BossWeight-0:60","BossWeight-1:60","BossWeight-2:60","BossWeight-0:60"}));Assert.That(reused,Is.True);
        }

        // AC-COM-003. Exact M4A defeat handoffs choose permanent removal and
        // lock the scheduler; they never emit a mixed temporary result.
        [Test] public void DefeatPermanentlyRemovesAllPayloadsAndTerminates()
        {
            var r=CreateRig();
            for(var i=0;i<108;i++){r.Transfer.AdvanceTransferPhaseForTests();r.Combat.AdvanceCombatPhaseForTests();r.Bridge.AdvanceBridgePhaseForTests();r.Scheduler.AdvanceSchedulerPhaseForTests();r.Player.AdvanceOneFixedTickForTests();}
            var tick=r.Player.NextExpectedTick;r.Transfer.SubmitForTests(Press(tick));var sameTickTransfer=r.Transfer.AdvanceTransferPhaseForTests();
            Assert.That(sameTickTransfer.Snapshot.PlayerState,Is.EqualTo(TransferPlayerState.Lightweight));
            r.Combat.Submit(new OrdanBossCombatInput(tick,null,null,null,new[]{new DamageRequest("ordan.lethal","test","ordan",60,DamageKind.BossPattern,tick)}));
            r.Combat.AdvanceCombatPhaseForTests();r.Bridge.AdvanceBridgePhaseForTests();var result=r.Scheduler.AdvanceSchedulerPhaseForTests();
            Assert.That(result.IsTerminal,Is.True);Assert.That(result.TemporarilyEndedTargetIds,Is.Empty);Assert.That(result.PermanentlyRemovedTargetIds,Is.EqualTo(new[]{"BossWeight-0","BossWeight-1","BossWeight-2"}));
            Assert.That(sameTickTransfer.Snapshot.PlayerState,Is.EqualTo(TransferPlayerState.Lightweight));
            r.Player.AdvanceOneFixedTickForTests();var cleanup=r.Transfer.AdvanceTransferPhaseForTests();Assert.That(cleanup.Snapshot.PlayerState,Is.EqualTo(TransferPlayerState.Baseline));
            Assert.Throws<InvalidOperationException>(()=>r.Scheduler.AdvanceSchedulerPhaseForTests());
            while(r.Player.NextExpectedTick.Value<130){r.Player.AdvanceOneFixedTickForTests();if(r.Player.NextExpectedTick.Value<130)r.Transfer.AdvanceTransferPhaseForTests();}
            r.Sinks[0].CommitExposure(true);r.Colliders[0].enabled=true;r.Transfer.SubmitForTests(Press(r.Player.NextExpectedTick));var rejected=r.Transfer.AdvanceTransferPhaseForTests();
            Assert.That(rejected.PressResult.Value.Success,Is.False);Assert.That(rejected.PressResult.Value.FailureReason,Is.EqualTo(TransferFailureReason.InvalidTarget));Assert.That(r.Sinks[0].IsApplied,Is.False);
        }

        // AC-COM-003. Outcome shape makes terminal removal and ordinary
        // temporary withdrawal unambiguous and defensively copied.
        [Test] public void ExposureOutcomeRejectsMixedTerminalAndTemporaryShapes()
        {
            var tick=new SimulationTick(7);
            Assert.Throws<ArgumentException>(()=>new OrdanBossTransferExposureOutcome(tick,new SimulationTick(8),"BossWeight-0",string.Empty,
                new[]{new TransferTargetExposureEnded("BossWeight-0",new SimulationTick(8))},new TransferTargetRemoved[0],true));
            var ends=new[]{new TransferTargetExposureEnded("BossWeight-0",new SimulationTick(8))};
            var valid=new OrdanBossTransferExposureOutcome(tick,new SimulationTick(8),"BossWeight-0",string.Empty,ends,new TransferTargetRemoved[0],false);
            ends[0]=new TransferTargetExposureEnded("BossWeight-1",new SimulationTick(8));
            Assert.That(valid.TemporarilyEndedTargetIds,Is.EqualTo(new[]{"BossWeight-0"}));
        }

        // AC-COM-003, AC-COM-004. A repeated source tick is rejected before
        // collider, sink, queue, or latest-outcome state can change.
        [Test] public void RepeatedSchedulerAdvanceRejectsWithoutMutation()
        {
            var r=CreateRig();r.Transfer.AdvanceTransferPhaseForTests();r.Combat.AdvanceCombatPhaseForTests();r.Bridge.AdvanceBridgePhaseForTests();
            var first=r.Scheduler.AdvanceSchedulerPhaseForTests();var enabled=Enabled(r.Colliders);
            Assert.Throws<InvalidOperationException>(()=>r.Scheduler.AdvanceSchedulerPhaseForTests());
            Assert.That(r.Scheduler.LatestOutcome.SourceTick,Is.EqualTo(first.SourceTick));Assert.That(Enabled(r.Colliders),Is.EqualTo(enabled));
        }

        // AC-COM-003. The verified three-payload test seam is explicitly
        // legacy and its typed mode cannot be switched after registration.
        [Test] public void LegacyConfigurationIsTypedAndCannotSwitchAfterInitialization()
        {
            var r=CreateRig();
            Assert.That(r.Scheduler.ConfigurationModeForAuthoring,Is.EqualTo(OrdanBossTransferExposureConfigurationMode.LegacyPayloadOnly));
            Assert.Throws<InvalidOperationException>(()=>r.Scheduler.ConfigureForTests(r.Bridge,r.Transfer,null,null));
            Assert.That(r.Scheduler.ConfigurationModeForAuthoring,Is.EqualTo(OrdanBossTransferExposureConfigurationMode.LegacyPayloadOnly));
        }

        // AC-WT-003, AC-WT-005. Pre-forecast hidden payloads remain in the
        // frozen registry but cannot become selected or accept a modifier.
        [Test] public void InitiallyHiddenPayloadCannotBeSelected()
        {
            var r=CreateRig();r.Transfer.AdvanceTransferPhaseForTests();r.Player.AdvanceOneFixedTickForTests();var tick=r.Player.NextExpectedTick;r.Transfer.SubmitForTests(Press(tick));var result=r.Transfer.AdvanceTransferPhaseForTests();
            Assert.That(result.PressResult.Value.Success,Is.False);Assert.That(result.PressResult.Value.FailureReason,Is.EqualTo(TransferFailureReason.TargetUnavailable));
            Assert.That(result.Snapshot.ActiveTargetId,Is.Empty);Assert.That(Enabled(r.Colliders),Is.EqualTo(0));
            for(var index=0;index<3;index++)Assert.That(r.Sinks[index].IsApplied,Is.False);
        }

        // AC-COM-002. Render grouping does not participate in the fixed trace.
        [Test] public void RenderGroupsHaveIdenticalExposureTrace(){var a=Replay(30);TearDown();var b=Replay(60);TearDown();var c=Replay(144);Assert.That(a,Is.EqualTo(b));Assert.That(a,Is.EqualTo(c));}
        private string Replay(int hz){var r=CreateRig();var values=new List<string>();var bank=0;while(values.Count<220){bank+=60;while(bank>=hz&&values.Count<220){bank-=hz;var tick=r.Player.NextExpectedTick;r.Transfer.AdvanceTransferPhaseForTests();r.Combat.AdvanceCombatPhaseForTests();r.Bridge.AdvanceBridgePhaseForTests();values.Add(tick.Value+":"+r.Scheduler.AdvanceSchedulerPhaseForTests().ExposedTargetId);r.Player.AdvanceOneFixedTickForTests();}}return string.Join("/",values);}

        private Rig CreateRig(){var playerRoot=Track("player");var body=playerRoot.AddComponent<Rigidbody2D>();body.bodyType=RigidbodyType2D.Kinematic;var playerCollider=playerRoot.AddComponent<BoxCollider2D>();var player=playerRoot.AddComponent<PlayerMovementController>();var targets=new TransferTarget[3];var sinks=new OrdanBossPayloadTransferModifierSink[3];var colliders=new Collider2D[3];for(var i=0;i<3;i++){var value=Track("BossWeight-"+i);value.transform.position=Vector3.right;colliders[i]=value.AddComponent<BoxCollider2D>();sinks[i]=value.AddComponent<OrdanBossPayloadTransferModifierSink>();sinks[i].ConfigureForTests("BossWeight-"+i);targets[i]=value.AddComponent<TransferTarget>();targets[i].ConfigureForTests("BossWeight-"+i,TransferTargetKind.BossPayload,0,0,0,0,100,100,OrdanBossPayloadTransferModifierSink.RequiredBaseProfileId,colliders[i],value.transform,sinks[i]);}var transferRegistry=Track("transfer-registry").AddComponent<TransferTargetRegistry>();transferRegistry.ConfigureForTests(targets);var transfer=Track("transfer").AddComponent<TransferSimulationDriver>();transfer.ConfigureForTests(player,transferRegistry);var ordanRoot=Track("ordan");var ordanCollider=ordanRoot.AddComponent<BoxCollider2D>();var ordan=ordanRoot.AddComponent<CombatTarget>();ordan.ConfigureForTests("ordan",60,0,true,0,0,0,0,100,100,ordanCollider,ordanRoot.transform);var playerTarget=playerRoot.AddComponent<CombatTarget>();playerTarget.ConfigureForTests("player",5,45,false,0,0,0,0,100,100,playerCollider,playerRoot.transform);var combatRegistry=Track("combat-registry").AddComponent<CombatTargetRegistry>();combatRegistry.ConfigureForTests(ordan,playerTarget);var combatRoot=Track("combat");combatRoot.SetActive(false);var combat=combatRoot.AddComponent<OrdanBossCombatSimulationDriver>();combat.ConfigureForTests(player,combatRegistry,transfer,combat);combatRoot.SetActive(true);var bridgeRoot=Track("bridge");bridgeRoot.SetActive(false);var bridge=bridgeRoot.AddComponent<OrdanBossSimulationDriver>();bridge.ConfigureForTests(player,transfer,combat);bridge.InitializeForTests();bridgeRoot.SetActive(true);var schedulerRoot=Track("scheduler");schedulerRoot.SetActive(false);var scheduler=schedulerRoot.AddComponent<OrdanBossTransferExposureScheduler>();scheduler.ConfigureForTests(bridge,transfer,targets,sinks);scheduler.InitializeForTests();schedulerRoot.SetActive(true);Assert.That(Enabled(colliders),Is.EqualTo(0));return new Rig(player,transfer,combat,bridge,scheduler,sinks,colliders);}
        private static TransferSimulationInput Press(SimulationTick tick){var camera=new SimulationCameraPoseSnapshot(new SimulationTick(tick.Value-1),0,0,10000,0,1920,1080,0,0,1920,1080,3);var aim=new AimSample(99,AimSource.GamepadStick,null,null,null,4096,0,camera.CameraPoseTick,tick);return new TransferSimulationInput(tick,camera,aim,new TransferPressed(99,tick),Array.Empty<TransferTargetRemoved>(),null);}
        private static int Enabled(IReadOnlyList<Collider2D> values){var count=0;for(var i=0;i<values.Count;i++)if(values[i].enabled)count++;return count;}
        private GameObject Track(string name){var value=new GameObject(name);_objects.Add(value);return value;}
        private readonly struct Rig{internal Rig(PlayerMovementController player,TransferSimulationDriver transfer,OrdanBossCombatSimulationDriver combat,OrdanBossSimulationDriver bridge,OrdanBossTransferExposureScheduler scheduler,OrdanBossPayloadTransferModifierSink[] sinks,Collider2D[] colliders){Player=player;Transfer=transfer;Combat=combat;Bridge=bridge;Scheduler=scheduler;Sinks=sinks;Colliders=colliders;}internal PlayerMovementController Player{get;}internal TransferSimulationDriver Transfer{get;}internal OrdanBossCombatSimulationDriver Combat{get;}internal OrdanBossSimulationDriver Bridge{get;}internal OrdanBossTransferExposureScheduler Scheduler{get;}internal OrdanBossPayloadTransferModifierSink[] Sinks{get;}internal Collider2D[] Colliders{get;}}
    }
}
