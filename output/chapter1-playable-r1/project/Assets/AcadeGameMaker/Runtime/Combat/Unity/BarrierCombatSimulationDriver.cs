using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    internal sealed class BarrierInputCandidate
    {
        internal BarrierInputCandidate(BarrierCombatSimulationDriver consumer, object owner, InputFrameCommitReceipt receipt,
            InputFrameCommitReceipt? priorReceipt, long revision, bool locked)
        {Consumer=consumer;Owner=owner;Receipt=receipt;PriorReceipt=priorReceipt;Revision=revision;Locked=locked;}
        internal BarrierCombatSimulationDriver Consumer {get;}
        internal object Owner {get;}
        internal InputFrameCommitReceipt Receipt {get;}
        internal InputFrameCommitReceipt? PriorReceipt {get;}
        internal long Revision {get;}
        internal bool Locked {get;}
    }
    internal sealed class BarrierCombatPublication
    {
        private readonly ReadOnlyCollection<CombatantSnapshot> _combatants;
        private readonly ReadOnlyCollection<DamageRequest> _requests;
        private readonly ReadOnlyCollection<DamageResult> _results;
        internal BarrierCombatPublication(BarrierCombatSimulationDriver owner, Chapter1ChoiceEnvironmentOwner environment,
            object runKey, long generation, InputFrameCommitReceipt receipt, object capture,
            DamageRequest[] requests, DamageResult[] results, CombatantSnapshot[] combatants)
        {Owner=owner;Environment=environment;RunKey=runKey;Generation=generation;Receipt=receipt;Capture=capture;
            _requests=Array.AsReadOnly((DamageRequest[])requests.Clone());_results=Array.AsReadOnly((DamageResult[])results.Clone());
            _combatants=Array.AsReadOnly((CombatantSnapshot[])combatants.Clone());}
        internal BarrierCombatSimulationDriver Owner {get;}
        internal Chapter1ChoiceEnvironmentOwner Environment {get;}
        internal object RunKey {get;}
        internal long Generation {get;}
        internal InputFrameCommitReceipt Receipt {get;}
        internal SimulationTick Tick => Receipt.Tick;
        internal object Capture {get;}
        internal IReadOnlyList<CombatantSnapshot> Combatants => _combatants;
        internal IReadOnlyList<DamageRequest> Requests => _requests;
        internal IReadOnlyList<DamageResult> Results => _results;
    }

    // REQ-C1C-002/004, AC-CHOICE-004. -190의 실제 CombatSession만 HP·죽음을 소유한다.
    [DefaultExecutionOrder(-190)]
    [DisallowMultipleComponent]
    internal sealed class BarrierCombatSimulationDriver : MonoBehaviour
    {
        private PlayerMovementController _player;
        private CombatTargetRegistry _registry;
        private TransferSimulationDriver _transfer;
        private Chapter1ChoiceEnvironmentOwner _environment;
        private IInputFrameCommitSource _source;
        private object _owner,_runKey;
        private long _generation,_inputRevision,_lastOrdinal=-1,_modeEpoch;
        private bool _configured,_faulted,_hasEpoch;
        private CombatSession _session;
        private IReadOnlyList<FrozenCombatTarget> _roster;
        private BarrierInputCandidate _pendingInput;
        private InputFrameCommitReceipt? _committedInput;
        private BarrierCombatPublication _publication;
        internal BarrierCombatPublication LatestPublication => _publication;

        internal void ConfigureForAuthoring(PlayerMovementController player, CombatTargetRegistry registry,
            TransferSimulationDriver transfer, Chapter1ChoiceEnvironmentOwner environment,
            object owner, IInputFrameCommitSource source, object runKey, long generation, int branch,
            SimulationTick firstTick, int carriedHealth, int carriedInvulnerabilityEndTick)
        {
            if(gameObject.activeInHierarchy || _configured || player==null || registry==null || transfer==null || environment==null ||
                owner==null || source==null || !ReferenceEquals(owner,source) || source.IsFaulted || runKey==null || generation<1 ||
                firstTick.Value<0 || firstTick.Value>int.MaxValue-600 || carriedHealth<1 || carriedHealth>5 || carriedInvulnerabilityEndTick< -1)
                throw new InvalidOperationException("봉쇄선 전투는 실제 승계값으로 비활성 새 그래프에 결속합니다.");
            var roster=registry.ReadValidatedAuthoringRoster();ValidateRoster(roster,branch);
            var session=new CombatSession(firstTick);
            foreach(var target in roster)
            {
                if(target.TargetId=="player")session.RegisterCarriedPlayer(target.CombatantRegistration,carriedHealth,carriedInvulnerabilityEndTick);
                else session.Register(target.CombatantRegistration);
            }
            _player=player;_registry=registry;_transfer=transfer;_environment=environment;_owner=owner;_source=source;
            _runKey=runKey;_generation=generation;_roster=roster;_session=session;_configured=true;
        }
        internal BarrierInputCandidate PrepareInputFrame(object owner,SimulationTick tick,bool locked,InputFrameCommitReceipt receipt)
        {
            Ensure(owner,tick);if(_pendingInput!=null || _committedInput.HasValue)throw new InvalidOperationException("전투 입력 후보가 중복되었습니다.");
            ValidateReceipt(receipt,tick,locked);var next=checked(_inputRevision+1);
            _pendingInput=new BarrierInputCandidate(this,owner,receipt,_source.CurrentReceipt,next,locked);return _pendingInput;
        }
        internal void ValidateInputFrame(object owner,BarrierInputCandidate candidate)
        {
            if(candidate==null || !ReferenceEquals(candidate,_pendingInput) || !ReferenceEquals(candidate.Consumer,this) ||
                !ReferenceEquals(candidate.Owner,owner) || candidate.Revision!=checked(_inputRevision+1) ||
                !Nullable.Equals(candidate.PriorReceipt,_source.CurrentReceipt))throw new InvalidOperationException("전투 입력 후보가 외부·오래된·소비 상태입니다.");
            Ensure(owner,candidate.Receipt.Tick);ValidateReceipt(candidate.Receipt,candidate.Receipt.Tick,candidate.Locked);
        }
        internal void CommitInputFrame(object owner,BarrierInputCandidate candidate)
        {
            ValidateInputFrame(owner,candidate);_committedInput=candidate.Receipt;_inputRevision=candidate.Revision;_pendingInput=null;
        }
        private void FixedUpdate()
        {
            if(!_configured || _faulted)return;
            try{AdvanceCombatPhase();}catch{_faulted=true;throw;}
        }
        internal void AdvanceCombatPhase()
        {
            if(!_configured || _faulted || _player==null || _source.IsFaulted || !_committedInput.HasValue ||
                !_source.CurrentReceipt.HasValue || _source.CurrentReceipt.Value!=_committedInput.Value ||
                _committedInput.Value.Tick!=_player.NextExpectedTick || _session.NextExpectedTick!=_player.NextExpectedTick)
                throw new InvalidOperationException("전역 receipt가 확정되기 전에 전투를 실행할 수 없습니다.");
            var receipt=_committedInput.Value;var tick=receipt.Tick;
            var capture=_transfer.ReadChapter1CommittedCapture(tick);
            if(!_transfer.ValidateChapter1CommittedCapture(tick,capture))throw new InvalidOperationException("같은 tick의 실제 전이 capture가 없습니다.");
            var prior=SnapshotRoster();
            var batch=receipt.Mode==InputMode.GameplayEnabled?_environment.PrepareDueDamage(tick,capture,_roster,prior):null;
            if(batch!=null && !_environment.ValidatePendingDamage(batch))throw new InvalidOperationException("피해 발급원이 실제 예약과 다릅니다.");
            var requests=batch==null?Array.Empty<DamageRequest>():Copy(batch.Requests);
            foreach(var request in requests)
                if(request.Kind!=DamageKind.ChoiceSkill || request.SourceId!="BarrierCounterweight" || request.Tick!=tick ||
                    (request.TargetId!="BarrierCounterweight" && request.TargetId!="BarrierGate-Fractured") ||
                    (request.TargetId=="BarrierCounterweight"?request.Amount!=4:request.Amount!=2))
                    throw new InvalidOperationException("봉쇄선 허용 피해 요청이 아닙니다.");
            // 모든 rejection·checked 연산은 실제 HP 소비 전에 완료한다.
            _=checked(tick.Value+600);ValidateReceipt(receipt,tick,receipt.Mode!=InputMode.GameplayEnabled);
            Array.Sort(requests,(a,b)=>string.CompareOrdinal(a.RequestId,b.RequestId));
            var applied=_session.Process(tick,requests);var results=new DamageResult[applied.Count];for(var i=0;i<results.Length;i++)results[i]=applied[i];
            if(batch!=null)_environment.ConsumePendingDamage(batch);
            _publication=new BarrierCombatPublication(this,_environment,_runKey,_generation,receipt,capture,requests,results,SnapshotRoster());
            _committedInput=null;_lastOrdinal=receipt.FrameOrdinal;_modeEpoch=receipt.ModeEpoch;_hasEpoch=true;
            _environment.ObserveCombatCompletion(_publication);
        }
        internal bool ValidatePublished(BarrierCombatPublication publication) => publication!=null && ReferenceEquals(publication,_publication) &&
            ReferenceEquals(publication.Owner,this) && !_faulted && !_source.IsFaulted && _source.CurrentReceipt.HasValue &&
            _source.CurrentReceipt.Value==publication.Receipt && _transfer.ValidateChapter1CommittedCapture(publication.Tick,publication.Capture);
        private void Ensure(object owner,SimulationTick tick)
        {
            if(!_configured || _faulted || !ReferenceEquals(owner,_owner) || _source.IsFaulted || _player.NextExpectedTick!=tick || _session.NextExpectedTick!=tick)
                throw new InvalidOperationException("현재 생산 입력 소유자·tick이 아닙니다.");
        }
        private void ValidateReceipt(InputFrameCommitReceipt receipt,SimulationTick tick,bool locked)
        {
            var canonical=new InputFrameCommitReceipt(receipt.Tick,receipt.FrameOrdinal,receipt.ModeEpoch,receipt.Mode);
            if(canonical!=receipt || receipt.Tick!=tick || receipt.FrameOrdinal!=checked(_lastOrdinal+1) || locked!=(receipt.Mode!=InputMode.GameplayEnabled) ||
                (!_hasEpoch && receipt.ModeEpoch!=0) || (_hasEpoch && receipt.ModeEpoch!=_modeEpoch && receipt.ModeEpoch!=checked(_modeEpoch+1)))
                throw new InvalidOperationException("봉쇄선 입력 receipt 순서가 다릅니다.");
        }
        private CombatantSnapshot[] SnapshotRoster()
        {var snapshots=new CombatantSnapshot[_roster.Count];for(var i=0;i<snapshots.Length;i++)snapshots[i]=_session.Snapshot(_roster[i].TargetId);return snapshots;}
        private static DamageRequest[] Copy(IReadOnlyList<DamageRequest> source)
        {var rows=new DamageRequest[source.Count];for(var i=0;i<rows.Length;i++)rows[i]=source[i];return rows;}
        private static void ValidateRoster(IReadOnlyList<FrozenCombatTarget> roster,int branch)
        {
            if((branch!=1 && branch!=2) || roster.Count!=(branch==1?3:2))throw new InvalidOperationException("봉쇄선 분기별 등록 개수가 다릅니다.");
            var player=0;var box=0;var gate=0;
            foreach(var target in roster)
            {
                if(target.CanReceivePlayerBasicAttack)throw new InvalidOperationException("일반 공격으로 봉쇄선을 우회할 수 없습니다.");
                switch(target.TargetId)
                {
                    case "player": if(target.MaxHealth!=5 || target.InvulnerabilityTicks!=45 || target.HasMatchingTransferTarget)throw new InvalidOperationException("플레이어 등록 수치가 다릅니다.");player++;break;
                    case "BarrierCounterweight": if(target.MaxHealth!=8 || target.InvulnerabilityTicks!=0 || !target.HasMatchingTransferTarget)throw new InvalidOperationException("상자 등록 수치가 다릅니다.");box++;break;
                    case "BarrierGate-Fractured": if(branch!=1 || target.MaxHealth!=2 || target.InvulnerabilityTicks!=0 || target.HasMatchingTransferTarget)throw new InvalidOperationException("수탈 문 등록 수치가 다릅니다.");gate++;break;
                    default:throw new InvalidOperationException("승인 roster 밖의 대상입니다.");
                }
            }
            if(player!=1 || box!=1 || gate!=(branch==1?1:0))throw new InvalidOperationException("실제 등록 대상이 승인 집합과 다릅니다.");
        }
        private void OnDisable(){_faulted=true;_pendingInput=null;_committedInput=null;}
    }
}
