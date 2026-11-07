using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;

namespace AcadeGameMaker.Run.Unity
{
    // REQ-C1B-SRETRY-005/006: 실패·파기·새 준비를 성공 출정과 구별한다.
    internal enum Chapter1RunPhase { PreparingHub, Hub, Saving, RetiringHub, StartingRoom, InRoom, LeavingRoom, AtBoss, BossCleanup, LockedPostBoss, Failing, CommittedFailure, SavingReturn, ReturnedToHub, Diagnostic,
        ExpeditionStorageFailed, RetiringHubForStorageRetry, PreparingHubAfterStorageRetry,
        PreparingChoice, ChoiceRequired, PreparingBarrier, AtBarrier, EndingSuccessfulRun, PreparingClosing, Closing, ChapterCompleted, RetiringCompleted }
    internal enum Chapter1ExpeditionStorageFailureKind { ReadUnavailableNoSaveIssued, FailedBeforeCommit }
    // REQ-C1B-SRETRY-001/002: 실제 시도 사실의 읽기 전용 투영이며 요청·저장 권위를 발급하지 않는다.
    internal sealed class Chapter1ExpeditionStorageAttempt
    {
        internal Chapter1ExpeditionStorageAttempt(long ordinal,string authenticatedRoot,Chapter1HubRequestToken token,long startedRevision,
            ProfileLaunchObservationBatchV1? observation,Chapter1RoomPlan? plan,long? expectedRevision,
            ProfileAtomicSaveResultV1? save,Chapter1ExpeditionStorageFailureKind? failure,string error)
        {Ordinal=ordinal;AuthenticatedRoot=authenticatedRoot;Token=token;StartedRevision=startedRevision;Observation=observation;Plan=plan;ExpectedRevision=expectedRevision;Save=save;Failure=failure;Error=error;}
        internal long Ordinal {get;}
        internal string AuthenticatedRoot {get;}
        internal Chapter1HubRequestToken Token {get;}
        internal long StartedRevision {get;}
        internal ProfileLaunchObservationBatchV1? Observation {get;}
        internal Chapter1RoomPlan? Plan {get;}
        internal long? ExpectedRevision {get;}
        internal ProfileAtomicSaveResultV1? Save {get;}
        internal Chapter1ExpeditionStorageFailureKind? Failure {get;}
        internal string Error {get;}
    }
    // REQ-C1B-HUB-001/RETURN-001: 표시값은 저장·장면 종료 권위가 아니다.
    internal readonly struct Chapter1RunView
    {
        internal Chapter1RunView(Chapter1RunPhase phase,string runKey,string roomId,int roomIndex,int seed,int health,
            long generation,SimulationTick? tick,long revision,string error,bool hubNeutral,
            Chapter1ExpeditionStorageAttempt currentStorageAttempt,Chapter1ExpeditionStorageAttempt lastStorageFailure)
        {Phase=phase;RunKey=runKey;RoomId=roomId;RoomIndex=roomIndex;Seed=seed;Health=health;Generation=generation;Tick=tick;Revision=revision;Error=error;HubNeutral=hubNeutral;
            CurrentStorageAttempt=currentStorageAttempt;LastStorageFailure=lastStorageFailure;}
        internal Chapter1RunPhase Phase {get;}
        internal string RunKey {get;}
        internal string RoomId {get;}
        internal int RoomIndex {get;}
        internal int Seed {get;}
        internal int Health {get;}
        internal long Generation {get;}
        internal SimulationTick? Tick {get;}
        internal long Revision {get;}
        internal string Error {get;}
        internal bool HubNeutral {get;}
        internal Chapter1ExpeditionStorageAttempt CurrentStorageAttempt {get;}
        internal Chapter1ExpeditionStorageAttempt LastStorageFailure {get;}
    }
}
