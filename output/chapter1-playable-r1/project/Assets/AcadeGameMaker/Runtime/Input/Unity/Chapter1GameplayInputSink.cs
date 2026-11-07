using System;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
namespace AcadeGameMaker.Input.Unity
{
    // REQ-C1B-INPUT-001: regular only; no callback or independent receipt publisher.
    internal sealed class Chapter1GameplayInputSink
    {
        private readonly InputRouter _router;
        private readonly CombatSimulationDriver _combat;
        private readonly object _token;
        private readonly Chapter1EncounterEntry _entry;
        internal Chapter1GameplayInputSink(InputRouter router,CombatSimulationDriver combat,object token,Chapter1EncounterEntry entry)
        {if(router==null || combat==null || token==null)throw new ArgumentException();_router=router;_combat=combat;_token=token;_entry=entry;}
        internal bool IsInputGateLocked => _combat.IsInputGateLocked;
        internal void ValidateRegistration(SimulationTick tick) => _combat.ValidateCoordinatedInputRegistration(_router,_router,_token,_entry.RunKey,_entry.Generation,tick);
        internal void Register(SimulationTick tick) => _combat.RegisterCoordinatedInputOwner(_router,_router,_token,_entry.RunKey,_entry.Generation,tick);
        internal CombatSimulationDriver.InputFrameCandidate Prepare(SimulationTick tick,bool locked,OrdanBossCombatInput? input,InputFrameCommitReceipt receipt)
        {
            CombatSimulationInput? value=input.HasValue?new CombatSimulationInput(tick,input.Value.Camera,input.Value.Aim,input.Value.Press,Array.Empty<AcadeGameMaker.Combat.DamageRequest>(),false):(CombatSimulationInput?)null;
            return _combat.PrepareInputFrame(_router,_token,tick,locked,value,receipt);
        }
        internal void Validate(CombatSimulationDriver.InputFrameCandidate candidate)=>_combat.ValidateInputFrameCandidate(_router,_token,candidate);
        internal void Commit(CombatSimulationDriver.InputFrameCandidate candidate)=>_combat.CommitInputFrame(_router,_token,candidate);
        internal bool TryReadCompleted(out Chapter1EncounterEvidenceView view)
        {
            view=default;
            if(_router.IsFaulted || !_router.CurrentReceipt.HasValue || !_combat.HasLatestOutcomeForEncounterHandoff)return false;
            var player=_combat.BoundPlayer;var transfer=_combat.BoundTransferDriver;
            var receipt=_router.CurrentReceipt.Value;var movement=player.Snapshot;var combat=_combat.LatestOutcomeForEncounterHandoff;var phase=transfer.LatestPhasePublication;
            if(!phase.HasValue || movement.Tick!=receipt.Tick || combat.Tick!=receipt.Tick || phase.Value.Tick!=receipt.Tick || player.NextExpectedTick.Value!=checked(receipt.Tick.Value+1))return false;
            AcadeGameMaker.Combat.CombatantSnapshot? health=null;
            foreach(var item in combat.Combatants)if(item.TargetId=="player") {if(health.HasValue)throw new InvalidOperationException("Duplicate player evidence.");health=item;}
            if(!health.HasValue)throw new InvalidOperationException("Missing completed player health.");
            var state=phase.Value.Snapshot;
            if(state.Tick!=receipt.Tick)return false;
            view=new Chapter1EncounterEvidenceView(_entry,receipt,new UnityEngine.Vector2(movement.PositionXQ4096/4096f,movement.PositionYQ4096/4096f),
                health.Value.CurrentHealth,health.Value.InvulnerabilityEndTick,state.ActiveTargetId,state.PlayerState==TransferPlayerState.Lightweight);
            return true;
        }
    }
}
