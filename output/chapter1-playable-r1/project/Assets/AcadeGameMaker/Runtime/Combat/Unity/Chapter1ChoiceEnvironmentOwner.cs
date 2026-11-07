using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Movement;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    internal sealed class ChoiceEnvironmentCandidate
    {
        internal ChoiceEnvironmentCandidate(Chapter1ChoiceEnvironmentOwner owner, ChoicePhaseContext context,
            int due, int holdEnd, int platformEnd, Vector2 platformCenter, Vector2 platformSize)
        {Owner=owner;Context=context;DueTick=due;HoldEndTick=holdEnd;PlatformEndTick=platformEnd;PlatformCenter=platformCenter;PlatformSize=platformSize;}
        internal Chapter1ChoiceEnvironmentOwner Owner {get;}
        internal ChoicePhaseContext Context {get;}
        internal int DueTick {get;}
        internal int HoldEndTick {get;}
        internal int PlatformEndTick {get;}
        internal Vector2 PlatformCenter {get;}
        internal Vector2 PlatformSize {get;}
    }
    internal sealed class ChoiceDamageBatch
    {
        private readonly ReadOnlyCollection<DamageRequest> _requests;
        internal ChoiceDamageBatch(Chapter1ChoiceEnvironmentOwner owner, ChoicePhaseContext reservation,
            SimulationTick tick, object capture, DamageRequest[] requests)
        {Owner=owner;Reservation=reservation;Tick=tick;Capture=capture;_requests=Array.AsReadOnly((DamageRequest[])requests.Clone());}
        internal Chapter1ChoiceEnvironmentOwner Owner {get;}
        internal ChoicePhaseContext Reservation {get;}
        internal SimulationTick Tick {get;}
        internal object Capture {get;}
        internal IReadOnlyList<DamageRequest> Requests => _requests;
    }

    // REQ-C1C-002/004, REQ-CHOICE-006/007. HP 대신 실제 CombatSession으로 가는 요청만 만든다.
    [DisallowMultipleComponent]
    internal sealed class Chapter1ChoiceEnvironmentOwner : MonoBehaviour
    {
        private TransferSimulationDriver _transfer;
        private PlayerMovementController _player;
        private Collider2D _playerCollider;
        private Transform _barrierRoot;
        private Rigidbody2D _counterweight;
        private BoxCollider2D _gate, _bridge, _platform;
        private PlatformEffector2D _platformEffector;
        private object _adapter, _runKey;
        private long _generation;
        private int _branch;
        private bool _configured, _retired, _hasActualSkill, _gateOpened;
        private ChoiceEnvironmentCandidate _pending, _reservation;
        private ChoiceDamageBatch _pendingDamage;
        private int _stunEndTick, _bridgeActivationTick=-1;
        private RigidbodyConstraints2D _beforeHold;
        private bool _holding;
        internal int StunEndTick => _stunEndTick;
        internal bool GateOpened => _gateOpened;
        internal bool HasActualSkill => _hasActualSkill;

        internal void ConfigureForAuthoring(TransferSimulationDriver transfer, PlayerMovementController player,
            Collider2D playerCollider, Transform barrierRoot, Rigidbody2D counterweight,
            BoxCollider2D gate, BoxCollider2D bridge, BoxCollider2D platform, PlatformEffector2D platformEffector,
            object adapter, object runKey, long generation, int branch)
        {
            if(gameObject.activeInHierarchy || _configured || transfer==null || player==null || playerCollider==null || barrierRoot==null ||
                counterweight==null || gate==null || bridge==null || platform==null || platformEffector==null || adapter==null || runKey==null || generation<1 || (branch!=1 && branch!=2))
                throw new InvalidOperationException("봉쇄선 환경은 비활성 새 그래프에 실제 물체를 결속합니다.");
            if(bridge.enabled || platform.enabled || gate.isTrigger || !platform.usedByEffector || !platformEffector.useOneWay ||
                platformEffector.colliderMask!=(1<<player.gameObject.layer) ||
                counterweight.gameObject.layer==player.gameObject.layer || gate.gameObject.layer==player.gameObject.layer)
                throw new InvalidOperationException("발판 초기 충돌과 플레이어 전용 규칙이 다릅니다.");
            ValidateGeometry(barrierRoot,counterweight,gate,bridge);
            _transfer=transfer;_player=player;_playerCollider=playerCollider;_barrierRoot=barrierRoot;_counterweight=counterweight;
            _gate=gate;_bridge=bridge;_platform=platform;_platformEffector=platformEffector;
            _adapter=adapter;_runKey=runKey;_generation=generation;_branch=branch;_configured=true;
        }
        internal ChoiceEnvironmentCandidate PrepareCompressionReservation(ChoicePhaseContext context)
        {
            if(!CanPrepare(context,1))return null;
            _pending=new ChoiceEnvironmentCandidate(this,context,checked(context.Tick.Value+36),0,0,default(Vector2),default(Vector2)); return _pending;
        }
        internal ChoiceEnvironmentCandidate PrepareReferencePlane(ChoicePhaseContext context)
        {
            if(!CanPrepare(context,2) || _platform.enabled || _holding)return null;
            // 현재 단일 capture의 실제 전이 관측을 사용하며 최초 상자 좌표를 재사용하지 않는다.
            if(!_transfer.TryReadPendingChoiceObservations(context,out var observations))return null;
            TransferCandidateObservation? target=null;
            for(var i=0;i<observations.Count;i++)if(observations[i].Descriptor.TargetId==context.TargetId && observations[i].ObservationTick==context.Tick)target=observations[i];
            if(!target.HasValue)return null;
            var foot=_playerCollider.bounds;var targetX=target.Value.WorldAimPointXQ1000/1000f;
            var length=Mathf.Clamp(Mathf.Abs(targetX-foot.center.x),1.5f,6f);
            var centerWorld=new Vector3((targetX+foot.center.x)/2f,foot.min.y-0.125f,_barrierRoot.position.z);
            var centerLocal=_barrierRoot.InverseTransformPoint(centerWorld);
            _pending=new ChoiceEnvironmentCandidate(this,context,context.Tick.Value,checked(context.Tick.Value+60),
                checked(context.Tick.Value+150),new Vector2(centerLocal.x,centerLocal.y),new Vector2(length,0.25f));return _pending;
        }
        internal bool Validate(ChoiceEnvironmentCandidate candidate) => candidate!=null && ReferenceEquals(candidate,_pending) &&
            ReferenceEquals(candidate.Owner,this) && Current(candidate.Context) && _counterweight!=null && !_retired &&
            (candidate.Context.Skill==1 || (!_platform.enabled && !_holding));
        internal void Commit(ChoiceEnvironmentCandidate candidate)
        {
            if(!Validate(candidate))throw new InvalidOperationException("현재 환경 후보가 아닙니다.");
            _pending=null;_reservation=candidate;_hasActualSkill=true;
            if(candidate.Context.Skill==2)
            {
                _platform.transform.localPosition=new Vector3(candidate.PlatformCenter.x,candidate.PlatformCenter.y,0);
                _platform.size=candidate.PlatformSize;_platform.enabled=true;
                _beforeHold=_counterweight.constraints;_counterweight.constraints|=RigidbodyConstraints2D.FreezePosition;
                _holding=true;
            }
        }
        internal void Cancel(ChoiceEnvironmentCandidate candidate)
        {if(candidate!=null && ReferenceEquals(candidate,_pending) && ReferenceEquals(candidate.Owner,this))_pending=null;}
        private bool CanPrepare(ChoicePhaseContext context,int skill) => _pending==null && Current(context) && _branch==skill &&
            context.Skill==skill && context.TargetId=="BarrierCounterweight" && _counterweight!=null &&
            (_reservation==null || context.Tick.Value>=_reservation.PlatformEndTick && context.Tick.Value>=_reservation.DueTick);
        private bool Current(ChoicePhaseContext context) => _configured && !_retired && context!=null &&
            ReferenceEquals(context.Adapter,_adapter) && ReferenceEquals(context.Driver,_transfer) && ReferenceEquals(context.RunKey,_runKey) &&
            context.Generation==_generation && _transfer.ValidatePendingChoiceContext(context);

        // 이 정확 읽기 서명은 공유 -200 capture 인계에서 아직 미구현이다. 새 SyncTransforms는 호출하지 않는다.
        internal ChoiceDamageBatch PrepareDueDamage(SimulationTick tick, object capture, IReadOnlyList<FrozenCombatTarget> roster,
            IReadOnlyList<CombatantSnapshot> priorCombatants)
        {
            if(!_configured || _retired || _pendingDamage!=null || _reservation==null || _reservation.Context.Skill!=1 ||
                tick.Value!=_reservation.DueTick || !_transfer.ValidateChapter1CommittedCapture(tick,capture))return null;
            CombatantSnapshot? centerState=null;
            for(var i=0;i<priorCombatants.Count;i++)if(priorCombatants[i].TargetId=="BarrierCounterweight")centerState=priorCombatants[i];
            if(_counterweight==null || !centerState.HasValue || centerState.Value.IsDead ||
                !_transfer.IsChapter1ChoiceTargetAvailable("BarrierCounterweight",_generation,capture)){_reservation=null;return null;}
            var requests=new List<DamageRequest>();var center=_counterweight.position;
            var centerX=TransferObservationBuilder.QuantizePoseQ1000(center.x);var centerY=TransferObservationBuilder.QuantizePoseQ1000(center.y);
            for(var i=0;i<roster.Count;i++)
            {
                var target=roster[i];if(target.TargetId=="player" || target.TargetPoseRoot==null)continue;
                var alive=false;for(var j=0;j<priorCombatants.Count;j++)if(priorCombatants[j].TargetId==target.TargetId && !priorCombatants[j].IsDead)alive=true;
                if(!alive)continue;
                var amount=0;
                if(target.TargetId=="BarrierCounterweight")amount=4;
                else if(target.TargetId=="BarrierGate-Fractured")
                {
                    var pose=target.TargetPoseRoot.position;
                    if(IsWithinCompressionRadiusQ1000(centerX,centerY,TransferObservationBuilder.QuantizePoseQ1000(pose.x),
                        TransferObservationBuilder.QuantizePoseQ1000(pose.y)))amount=2;
                }
                if(amount>0)requests.Add(new DamageRequest("choice/"+_generation+"/"+_reservation.Context.ChoiceEdgeOrdinal+"/"+target.TargetId,
                    "BarrierCounterweight",target.TargetId,amount,DamageKind.ChoiceSkill,tick));
            }
            _pendingDamage=new ChoiceDamageBatch(this,_reservation.Context,tick,capture,requests.ToArray());return _pendingDamage;
        }
        internal bool ValidatePendingDamage(ChoiceDamageBatch batch) => batch!=null && ReferenceEquals(batch,_pendingDamage) &&
            ReferenceEquals(batch.Owner,this) && _reservation!=null && ReferenceEquals(batch.Reservation,_reservation.Context) &&
            !_retired && _transfer.ValidateChapter1CommittedCapture(batch.Tick,batch.Capture);
        internal void ConsumePendingDamage(ChoiceDamageBatch batch)
        {
            if(!ValidatePendingDamage(batch))throw new InvalidOperationException("미발급·소비된 피해 예약입니다.");
            _pendingDamage=null;_stunEndTick=checked(batch.Tick.Value+60);_reservation=null;
        }
        internal void ObserveCombatCompletion(BarrierCombatPublication publication)
        {
            if(publication==null || !ReferenceEquals(publication.Environment,this) || publication.Generation!=_generation ||
                !ReferenceEquals(publication.RunKey,_runKey) || !publication.Owner.ValidatePublished(publication))throw new InvalidOperationException("현재 전투 결과가 아닙니다.");
            if(_branch!=1 || !_hasActualSkill)return;
            foreach(var combatant in publication.Combatants)if(combatant.TargetId=="BarrierGate-Fractured" && combatant.IsDead && combatant.CurrentHealth==0)
                _bridgeActivationTick=checked(publication.Tick.Value+1);
        }
        internal void ObserveCompletedPlayerContact(SimulationTick tick, object completedView,InputFrameCommitReceipt receipt)
        {
            if(!_configured || _retired || _branch!=2 || !_hasActualSkill || !_platform.enabled ||
                !_transfer.ValidateChapter1CompletedBarrierView(tick,completedView))return;
            // REQ-AUTOCH1-SUPPORT-001: the same completed movement owns exact top support, including kinematic/static pairs.
            if(_gateOpened || _reservation==null || _reservation.Context.Skill!=2 || tick.Value<_reservation.DueTick || tick.Value>=_reservation.PlatformEndTick ||
                _reservation.Context.Generation!=_generation || !ReferenceEquals(_reservation.Context.RunKey,_runKey) ||
                _player==null || _playerCollider==null || _platform==null || _player.transform.parent!=_barrierRoot || _platform.transform.parent!=_barrierRoot ||
                _player.GetComponent<Collider2D>()!=_playerCollider || receipt.Tick!=tick)return;
            if(_player.TryReadCompletedSupport(tick,_platform,receipt)){_gate.enabled=false;_gateOpened=true;}
        }
        internal void AdvanceEnvironmentFromCapture(SimulationTick tick,object capture)
        {
            if(!_configured || _retired || !_transfer.ValidatePendingChapter1Capture(tick,capture))throw new InvalidOperationException("현재 환경 capture가 아닙니다.");
            if(_bridgeActivationTick>=0 && tick.Value>=_bridgeActivationTick){_bridge.enabled=true;_gate.enabled=false;_gateOpened=true;_bridgeActivationTick=-1;}
            if(_reservation!=null && _reservation.Context.Skill==2)
            {
                if(_holding && tick.Value>=_reservation.HoldEndTick){_counterweight.constraints=_beforeHold;_holding=false;}
                if(tick.Value>=_reservation.PlatformEndTick){_platform.enabled=false;_reservation=null;}
            }
        }
        private void OnDisable()
        {
            _retired=true;_pending=null;_pendingDamage=null;_reservation=null;
            if(_holding && _counterweight!=null){_counterweight.constraints=_beforeHold;_holding=false;}
            if(_platform!=null)_platform.enabled=false;
        }
        internal static bool IsWithinCompressionRadiusQ1000(int centerX,int centerY,int targetX,int targetY)
        {
            var dx=checked((long)targetX-centerX);var dy=checked((long)targetY-centerY);
            return checked(dx*dx+dy*dy)<=2250000L;
        }
        private static void ValidateGeometry(Transform root,Rigidbody2D weight,BoxCollider2D gate,BoxCollider2D bridge)
        {
            var box=weight.GetComponent<BoxCollider2D>();var target=weight.GetComponent<TransferTarget>();
            if(root.localScale!=Vector3.one || root.eulerAngles.z!=0 || box==null || target==null ||
                root.InverseTransformPoint(weight.transform.position)!=new Vector3(4.5f,0.5f,0) ||
                root.InverseTransformPoint(gate.transform.position)!=new Vector3(5.1f,1.5f,0) ||
                root.InverseTransformPoint(bridge.transform.position)!=new Vector3(2.75f,-0.125f,0) ||
                box.offset!=Vector2.zero || box.size!=Vector2.one || box.isTrigger ||
                gate.offset!=Vector2.zero || gate.size!=new Vector2(0.2f,3f) || bridge.offset!=Vector2.zero || bridge.size!=new Vector2(3.5f,0.25f) ||
                weight.bodyType!=RigidbodyType2D.Dynamic || weight.mass!=1f || weight.gravityScale!=1f ||
                (weight.constraints&RigidbodyConstraints2D.FreezeRotation)==0)
                throw new InvalidOperationException("승인된 봉쇄선 초기 물체 기하가 다릅니다.");
            var geometry=target.FreezeAuthoredGeometryForCrossValidation();
            if(geometry.TargetId!="BarrierCounterweight" || geometry.TargetCollider!=box || geometry.TargetPoseRoot!=weight.transform ||
                geometry.LocalAimPointXQ1000!=0 || geometry.LocalAimPointYQ1000!=0 ||
                geometry.LocalAimShapeCenterXQ1000!=0 || geometry.LocalAimShapeCenterYQ1000!=0 ||
                geometry.LocalAimShapeHalfExtentXQ1000!=500 || geometry.LocalAimShapeHalfExtentYQ1000!=500)
                throw new InvalidOperationException("실제 상자 pose root의 여섯 조준값이 다릅니다.");
        }
    }
}
