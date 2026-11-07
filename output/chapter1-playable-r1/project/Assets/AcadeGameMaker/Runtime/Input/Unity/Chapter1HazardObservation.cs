using System;
using AcadeGameMaker.Core;
using UnityEngine;

namespace AcadeGameMaker.Input.Unity
{
    // 기존 requester가 실제 combat/handoff/cleanup을 대조하고 원문 객체를 retain한다.
    internal sealed class Chapter1BossPublication
    {
        internal Chapter1BossPublication(object issuer,object token,string runKey,long generation,SimulationTick deathTick,
            int bossHealth,bool bossDead,int playerHealth,bool playerDead,int[] orderedHandoffs,SimulationTick? cleanupTick,
            bool lifecycleSuperseded,Chapter1LifecycleKind? lifecycle,bool registrationsAbsent,bool durableRemovalCommitted,
            bool playerBaseline,bool activeTargetEmpty,bool bossTeardownCommitted)
        {Issuer=issuer;Token=token;RunKey=runKey;Generation=generation;DeathTick=deathTick;BossHealth=bossHealth;BossDead=bossDead;
            PlayerHealth=playerHealth;PlayerDead=playerDead;_handoffs=orderedHandoffs==null?null:(int[])orderedHandoffs.Clone();CleanupTick=cleanupTick;
            LifecycleSuperseded=lifecycleSuperseded;Lifecycle=lifecycle;RegistrationsAbsent=registrationsAbsent;DurableRemovalCommitted=durableRemovalCommitted;
            PlayerBaseline=playerBaseline;ActiveTargetEmpty=activeTargetEmpty;BossTeardownCommitted=bossTeardownCommitted;}
        private readonly int[] _handoffs;
        internal object Issuer {get;} internal object Token {get;} internal string RunKey {get;} internal long Generation {get;}
        internal SimulationTick DeathTick {get;} internal int BossHealth {get;} internal bool BossDead {get;}
        internal int PlayerHealth {get;} internal bool PlayerDead {get;} internal int[] OrderedHandoffs=>_handoffs==null?null:(int[])_handoffs.Clone();
        internal SimulationTick? CleanupTick {get;} internal bool LifecycleSuperseded {get;} internal Chapter1LifecycleKind? Lifecycle {get;}
        internal bool RegistrationsAbsent {get;} internal bool DurableRemovalCommitted {get;} internal bool PlayerBaseline {get;}
        internal bool ActiveTargetEmpty {get;} internal bool BossTeardownCommitted {get;}
    }
    internal enum Chapter1LifecycleKind { RoomLeaving, RunFailed }
    internal sealed class Chapter1LifecyclePublication
    {
        internal Chapter1LifecyclePublication(Chapter1EncounterLifetimeOwner owner,InputRouter router,string runKey,long generation,
            InputFrameCommitReceipt lockedReceipt,Chapter1LifecycleKind kind,long revision)
        {if(owner==null || router==null || string.IsNullOrEmpty(runKey) || generation<=0 || lockedReceipt.Mode!=InputMode.Transition || revision<=0)throw new ArgumentException();
            Owner=owner;Router=router;RunKey=runKey;Generation=generation;LockedReceipt=lockedReceipt;Kind=kind;Revision=revision;}
        internal Chapter1EncounterLifetimeOwner Owner {get;}
        internal InputRouter Router {get;}
        internal string RunKey {get;}
        internal long Generation {get;}
        internal InputFrameCommitReceipt LockedReceipt {get;}
        internal Chapter1LifecycleKind Kind {get;}
        internal long Revision {get;}
    }
    // REQ-C1B-FAIL-001: 생성자 접근은 권한이 아니며 양쪽 private pending과 원문 발행원을 대조한다.
    internal sealed class Chapter1AcceptedFailureToken
    {
        internal Chapter1AcceptedFailureToken(IChapter1BossProgressionPort issuer,Chapter1EncounterLifetimeOwner consumer,
            InputRouter router,string runKey,long generation,InputFrameCommitReceipt sourceReceipt,long arbitrationRevision,
            Chapter1FailureSourceKind cause,Chapter1FailurePublication health,Chapter1FailurePublication kill,Chapter1FailurePublication crush)
        {
            if(issuer==null || consumer==null || router==null || string.IsNullOrEmpty(runKey) || generation<=0 || arbitrationRevision<=0 ||
                health==null || kill==null || crush==null)throw new ArgumentException("실패 승인 원문이 완전하지 않습니다.");
            Issuer=issuer;Consumer=consumer;Router=router;RunKey=runKey;Generation=generation;SourceReceipt=sourceReceipt;
            ArbitrationRevision=arbitrationRevision;Cause=cause;Health=health;Kill=kill;Crush=crush;
        }
        internal IChapter1BossProgressionPort Issuer {get;} internal Chapter1EncounterLifetimeOwner Consumer {get;}
        internal InputRouter Router {get;} internal string RunKey {get;} internal long Generation {get;}
        internal InputFrameCommitReceipt SourceReceipt {get;} internal long ArbitrationRevision {get;}
        internal Chapter1FailureSourceKind Cause {get;} internal Chapter1FailurePublication Health {get;}
        internal Chapter1FailurePublication Kill {get;} internal Chapter1FailurePublication Crush {get;}
    }
    internal enum Chapter1FailureSourceKind { Health, KillPlane, LethalCrush }

    // REQ-C1B-FAIL-001: 발행 객체 자체를 보존하며 값 복사로 출처를 만들지 않는다.
    internal sealed class Chapter1FailurePublication
    {
        internal Chapter1FailurePublication(object issuer, object token, InputRouter router, string runKey,
            long generation, InputFrameCommitReceipt receipt, long revision, Chapter1FailureSourceKind kind, bool triggered)
        {
            if (issuer == null || token == null || router == null || string.IsNullOrEmpty(runKey) || generation <= 0 || revision <= 0)
                throw new ArgumentException("실패 관측 출처가 완전하지 않습니다.");
            Issuer=issuer; Token=token; Router=router; RunKey=runKey; Generation=generation;
            Receipt=receipt; Revision=revision; Kind=kind; Triggered=triggered;
        }
        internal object Issuer { get; }
        internal object Token { get; }
        internal InputRouter Router { get; }
        internal string RunKey { get; }
        internal long Generation { get; }
        internal InputFrameCommitReceipt Receipt { get; }
        internal long Revision { get; }
        internal Chapter1FailureSourceKind Kind { get; }
        internal bool Triggered { get; }
    }

    // Input.Unity가 Run.Unity를 참조하지 않는 닫힌 저작 값이다.
    internal readonly struct Chapter1HazardDefinition
    {
        internal Chapter1HazardDefinition(string roomId, bool catalogValidated)
        {
            if (!catalogValidated || roomId==null || (roomId != "Ordan" && (roomId.Length != 3 || roomId[0]!='R' || roomId[1]!='0' || roomId[2]<'1' || roomId[2]>'6')))
                throw new ArgumentException("검증된 지정 방만 위험 관측을 구성할 수 있습니다.");
            RoomId=roomId;
        }
        internal string RoomId { get; }
        internal bool HasLethalCrush => RoomId=="R05";
        internal bool IsClosed(int tick, int firstTick)
        {
            if(tick<firstTick)throw new ArgumentOutOfRangeException(nameof(tick));
            var phase=(tick-firstTick)%180; return HasLethalCrush && phase>=60 && phase<120;
        }
        internal bool KillPlane(float completedCenterY) => completedCenterY < -8f;
        internal bool Crush(int tick, int firstTick, Rect completedBody) => IsClosed(tick,firstTick) &&
            completedBody.xMin<14f && completedBody.xMax>12f && completedBody.yMin<2f && completedBody.yMax>0f;
    }

    [DisallowMultipleComponent]
    internal sealed class Chapter1HazardObservation : MonoBehaviour
    {
        private InputRouter _router;
        private Chapter1HazardDefinition _definition;
        private int _firstTick;
        private bool _configured;
        private readonly object _killToken=new object(), _crushToken=new object();
        private Chapter1FailurePublication _kill, _crush;
        private long _revision;
        internal bool IsClosedAt(SimulationTick tick)=>_configured && _definition.IsClosed(tick.Value,_firstTick);
        internal void ConfigureForAuthoring(InputRouter router, Chapter1HazardDefinition definition, SimulationTick firstTick)
        {
            if(gameObject.activeInHierarchy || _configured || router==null || definition.RoomId==null || firstTick.Value<0)
                throw new InvalidOperationException("위험 발행자는 비활성 상태에서 한 번 결속합니다.");
            _router=router;_definition=definition;_firstTick=firstTick.Value;_configured=true;
        }
        // 기존 입력 소유자가 Movement/Combat/Transfer 완료를 확인한 뒤 호출한다.
        internal void PublishCompleted(Chapter1EncounterEvidenceView completed)
        {
            if(!_configured || !completed.HasCompleted || !_router.TryGetChapter1CompletedEvidence(out var actual) ||
                !actual.Receipt.Equals(completed.Receipt) || actual.RunKey!=completed.RunKey || actual.Generation!=completed.Generation ||
                !actual.Position.Equals(completed.Position) || !actual.PlayerBody.Equals(completed.PlayerBody))
                throw new InvalidOperationException("위험 입력은 같은 라우터의 실제 완료 관측이어야 합니다.");
            if(_kill!=null && _kill.Receipt.Equals(completed.Receipt))return;
            if(_kill!=null && completed.Tick.Value!=checked(_kill.Receipt.Tick.Value+1))
                throw new InvalidOperationException("위험 관측 틱이 연속하지 않습니다.");
            var revision=checked(_revision+1);
            // 승인 지형은 graph 로컬 좌표다. 완료 world snapshot을 동일 graph의 실제 변환으로 읽는다.
            var localPosition=transform.InverseTransformPoint(new Vector3(completed.Position.x,completed.Position.y,0));
            var localMin=transform.InverseTransformPoint(new Vector3(completed.PlayerBody.xMin,completed.PlayerBody.yMin,0));
            var localMax=transform.InverseTransformPoint(new Vector3(completed.PlayerBody.xMax,completed.PlayerBody.yMax,0));
            var localBody=Rect.MinMaxRect(localMin.x,localMin.y,localMax.x,localMax.y);
            // 빈 위험 목록 역시 정확 방 구성과 실제 완료 관측을 거쳐 닫힌 false를 발행한다.
            _kill=new Chapter1FailurePublication(this,_killToken,_router,completed.RunKey,completed.Generation,completed.Receipt,revision,
                Chapter1FailureSourceKind.KillPlane,_definition.KillPlane(localPosition.y));
            _crush=new Chapter1FailurePublication(this,_crushToken,_router,completed.RunKey,completed.Generation,completed.Receipt,revision,
                Chapter1FailureSourceKind.LethalCrush,_definition.Crush(completed.Tick.Value,_firstTick,localBody));
            _revision=revision;
        }
        internal bool TryGetCompleted(Chapter1EncounterEvidenceView completed, out Chapter1FailurePublication kill, out Chapter1FailurePublication crush)
        {
            kill=_kill;crush=_crush;
            return _configured && kill!=null && crush!=null && kill.Receipt.Equals(completed.Receipt) &&
                kill.RunKey==completed.RunKey && kill.Generation==completed.Generation && crush.Receipt.Equals(completed.Receipt);
        }
        internal bool Owns(Chapter1FailurePublication publication) =>
            ReferenceEquals(publication,_kill) && ReferenceEquals(publication.Token,_killToken) ||
            ReferenceEquals(publication,_crush) && ReferenceEquals(publication.Token,_crushToken);
        // REQ-C1B-G3OBS-011: 위험 true도 기존 객체 그대로 반환하며 무기록으로 읽는다.
        internal bool TryReadPublishedHazardsForAuthoring(out Chapter1FailurePublication kill, out Chapter1FailurePublication crush)
        {
            kill=null;crush=null;
            if(this==null || !isActiveAndEnabled || !_configured || _definition.RoomId==null || _router==null ||
                !_router.isActiveAndEnabled || !_router.IsChapter1Regular || _router.IsFaulted || _kill==null || _crush==null ||
                !Owns(_kill) || !Owns(_crush) || !ReferenceEquals(_kill.Issuer,this) || !ReferenceEquals(_crush.Issuer,this) ||
                !ReferenceEquals(_kill.Token,_killToken) || !ReferenceEquals(_crush.Token,_crushToken) ||
                !ReferenceEquals(_kill.Router,_router) || !ReferenceEquals(_crush.Router,_router) ||
                _kill.Kind!=Chapter1FailureSourceKind.KillPlane || _crush.Kind!=Chapter1FailureSourceKind.LethalCrush ||
                _revision<=0 || _kill.Revision!=_revision || _crush.Revision!=_revision ||
                string.IsNullOrEmpty(_kill.RunKey) || _kill.Generation<=0 || _kill.RunKey!=_crush.RunKey ||
                _kill.Generation!=_crush.Generation || !_kill.Receipt.Equals(_crush.Receipt) ||
                _kill.Receipt.Tick.Value<_firstTick || _kill.Receipt.Mode!=InputMode.GameplayEnabled)return false;
            kill=_kill;crush=_crush;return true;
        }
    }
}
