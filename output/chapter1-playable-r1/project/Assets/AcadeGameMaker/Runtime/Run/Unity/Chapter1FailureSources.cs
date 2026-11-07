using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;

namespace AcadeGameMaker.Run.Unity
{
    // REQ-C1B-FAIL-001: 누락을 false로 채우지 않고 실제 세 발행 객체로 mask를 만든다.
    internal sealed class Chapter1FailureSources
    {
        private readonly InputRouter _router;
        private readonly Chapter1HazardObservation _hazards;
        private readonly string _runKey;
        private readonly long _generation;
        private long _lastHealthRevision,_lastKillRevision,_lastCrushRevision;
        internal Chapter1FailureSources(InputRouter router,Chapter1HazardObservation hazards,string runKey,long generation)
        {if(router==null || hazards==null || string.IsNullOrEmpty(runKey) || generation<=0)throw new ArgumentException();_router=router;_hazards=hazards;_runKey=runKey;_generation=generation;}
        internal Chapter1FailurePublication LastHealthPublication {get;private set;}
        internal Chapter1FailurePublication LastKillPublication {get;private set;}
        internal Chapter1FailurePublication LastCrushPublication {get;private set;}
        internal RunFailureTickInput CloseCompleted(Chapter1EncounterEvidenceView completed)
        {
            if(!completed.HasCompleted || completed.RunKey!=_runKey || completed.Generation!=_generation ||
                !_router.TryGetChapter1CompletedEvidence(out var actual) || !actual.Receipt.Equals(completed.Receipt) ||
                !ReferenceEquals(actual.HealthPublication,completed.HealthPublication) || completed.HealthPublication==null ||
                !_hazards.TryGetCompleted(completed,out var kill,out var crush))throw new InvalidOperationException("세 원인의 실제 완료 publication이 닫히지 않았습니다.");
            var health=completed.HealthPublication;
            Validate(health,completed,Chapter1FailureSourceKind.Health,_lastHealthRevision);
            Validate(kill,completed,Chapter1FailureSourceKind.KillPlane,_lastKillRevision);
            Validate(crush,completed,Chapter1FailureSourceKind.LethalCrush,_lastCrushRevision);
            if(!ReferenceEquals(health.Issuer,completed.HealthProducer) || !ReferenceEquals(health.Token,completed.HealthProducerToken) ||
                !_hazards.Owns(kill) || !_hazards.Owns(crush) || health.Triggered!=(completed.Health==0))
                throw new InvalidOperationException("실패 publication의 발행 소유자가 다릅니다.");
            var presence=RunFailureCauseMask.None;var triggered=RunFailureCauseMask.None;
            Add(health,RunFailureCauseMask.HealthDepleted,ref presence,ref triggered);
            Add(kill,RunFailureCauseMask.KillPlane,ref presence,ref triggered);
            Add(crush,RunFailureCauseMask.LethalCrush,ref presence,ref triggered);
            var closed=new RunFailureTickInput(_runKey,completed.Tick,presence,triggered);
            LastHealthPublication=health;LastKillPublication=kill;LastCrushPublication=crush;
            _lastHealthRevision=health.Revision;_lastKillRevision=kill.Revision;_lastCrushRevision=crush.Revision;return closed;
        }
        private void Validate(Chapter1FailurePublication publication,Chapter1EncounterEvidenceView completed,Chapter1FailureSourceKind kind,long previous)
        {
            if(publication==null || publication.Kind!=kind || !ReferenceEquals(publication.Router,_router) ||
                publication.RunKey!=_runKey || publication.Generation!=_generation || !publication.Receipt.Equals(completed.Receipt) || publication.Revision<=previous)
                throw new InvalidOperationException("다른 출처·세대·틱 또는 중복 publication입니다.");
        }
        private static void Add(Chapter1FailurePublication value,RunFailureCauseMask bit,ref RunFailureCauseMask presence,ref RunFailureCauseMask triggered)
        {presence|=bit;if(value.Triggered)triggered|=bit;}
    }
}
