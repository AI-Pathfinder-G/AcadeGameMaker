# B1 공유 API와 실제 실패 출처 한정 인계

- 상태: Approved. 아스트라의 기존 B1 승인 범위 구체화이며 구현·실행 수용은 별도다.
- 구현: 별도 테라 `gpt-6.1-sol`; 독립 소스·실행 검수: 루나 `gpt-6-luna`; 통합·실행 소유: Root.
- 기준: CH1 주 계약 §11~12·§15~18, `b1-shared-api-handoff-terra61.v1.json`의 읽기 원장. 정확 현재 입력은 `b1-shared-api-closure-astra.v1.json`에 결속한다.
- REQ-C1B-ENTRY-001/ROOM-001/HP-001/FAIL-001, AC-C1B-ENTRY-001/HP-001/E2E-001/E2E-002를 추적한다.

## 1. 수정 경계

기존 승인된 `Input/Unity/OrdanBossTerminalTransitionRequester.cs`, `Chapter1EncounterLifetimeOwner.cs`, `InputRouter.cs`, `Chapter1EncounterEvidenceView.cs`와 B1 신규 후보 `Chapter1HazardObservation.cs`, `Chapter1HubLifetimeOwner.cs`, `Run/Unity/Chapter1FailureSources.cs`, `Chapter1PlayableFlowOwner.cs` 안에서만 아래 서명을 닫는다. 신규 제품 파일·assembly·Profile 변경0이며 **기존 §11.6/§12 승인 집합을 넘는 friend 추가0**이다. Input.Unity는 Run.Unity나 순수 Run을 새로 참조하지 않는다. 실제 구현은 현재 B0 최종 기준을 인계받고 같은 project Unity 종료 뒤에만 통합한다. 작성 중 B1 후보를 구현 완료로 취급하지 않는다.

접근성 권위 보정: §11.6은 “기존 Run/AssemblyInfo와 Input/Unity/AssemblyInfo에 production Run.Unity 각1을 추가하고 Input/Unity에 새 시험 friend1을 추가한다”고 명시하고 §12가 해당 범위를 Approved로 전환했다. 따라서 아직 B0 기준에 없는 `Input/Unity/AssemblyInfo.cs`의 `InternalsVisibleTo("AcadeGameMaker.Run.Unity")` 및 `InternalsVisibleTo("AcadeGameMaker.Run.Unity.PlayMode.Tests")` 정확2개는 B1 통합 시 추가해야 할 **기승인 변경**이다. `Run/AssemblyInfo.cs`의 Run.Unity friend1, Run.Unity asmdef의 Input.Unity 참조도 같은 기존 승인 범위다. 이 문서의 이전 “신규 friend0”은 추가 확장0을 뜻했으나 기존 미통합 friend까지 금지하는 것으로 읽힐 수 있어 이 문단으로 명확히 한다. 현재 소스에 friend가 없다는 루나의 관측은 맞으며, 이번 판정은 존재하는 것처럼 소급하지 않는다.

그 정확 friend2와 기존 단방향 참조가 함께 통합되면 Run.Unity의 FlowOwner는 Input.Unity의 internal `IChapter1BossProgressionPort`, `Chapter1AcceptedFailureToken` 및 internal 생성자를 접근할 수 있다. 토큰을 public으로 바꾸거나 다른 factory·권한 우회를 만들 필요가 없다. 생성자 접근 가능성은 성공 권한이 아니며 §3의 양쪽 private pending·실제 세 발행원·실제 중재 결과 검증은 그대로다. 테라는 이 두 AssemblyInfo의 기승인 델타를 구현 원장에 명시하고, 루나는 정확 이름·중복0·참조 방향·컴파일 결과를 검수한다. 공유 API 독립 검수의 접근성 P0는 이 권위 보정 원문을 대상으로 재대조하며 기존 검수 원문은 보존한다.

## 2. 보스 requester 구성과 단일 입력 권위

`OrdanBossTerminalTransitionRequester`에 다음 내부 메서드를 추가한다.

```csharp
void ConfigureChapter1ProgressionForAuthoring(
    Chapter1EncounterLifetimeOwner owner,
    Chapter1EncounterEntry entry,
    IChapter1BossProgressionPort port,
    OrdanBossTerminalTeardown teardown);
void ObserveChapter1Completed(Chapter1EncounterEvidenceView completed);
bool TryGetChapter1Publication(out Chapter1BossPublication publication);
bool OwnsChapter1Publication(Chapter1BossPublication publication);
void RequestChapter1AcceptedFailure(
    Chapter1EncounterLifetimeOwner owner,
    Chapter1AcceptedFailureToken token);
```

기존 `ConfigureForAuthoring(router,player,combat,handoff)`의 실제 직렬화 결속을 먼저 완료하고, lifetime이 비활성 보스 graph에 Movement/Transfer/Combat seed와 router의 CH1 entry를 설치한 다음 위 구성 메서드를 한 번 호출한다. 아직 motor/session을 만드는 getter를 읽지 않는다. requester는 미초기화·미요청·미폐쇄, `_localPlayable=false`, 동일 graph의 router/player/combat/handoff/teardown, 실제 `owner.IsIssuedEntry(router,entry)`와 양수 generation·현재 run·같은 port를 검사한다. 값만 같은 다른 entry/owner나 활성 graph에서 재설정은 거절한다. 검증 뒤 기존 Start가 실제 router requester를 등록한다. LocalPlayable 및 legacy 경로는 기존대로이며 CH1과 동시 opt-in은 거절한다.

보스 graph의 `RegisterModeRequester` 소유자는 이 requester 하나다. lifetime은 보스 router에 자신을 별도 등록하거나 두 번째 RequestMode를 보내지 않는다. regular graph는 기존 lifetime requester를 유지한다. 실패 전환은 lifetime에서 위 `RequestChapter1AcceptedFailure`로 위임한다. 정상 보스 사망은 기존 -211 requester가 다음 tick의 Transition을 요청한다. 기존 -210 입력 확정·-200 전이·-195 teardown·-190 Combat·+110 handoff 순서와 terminal triplet 검증을 완화하지 않는다.

`ObserveChapter1Completed`는 lifetime의 +200에서 현재 실제 완료 view가 확보된 뒤 호출하는 읽기/보관 입구다. 해당 메서드는 RequestMode/피해/정리/Run 상태를 바꾸지 않는다. 실제 router receipt·Movement/Combat 완료·handoff의 같은 tick·정확 roster·boss terminal triplet·`HasCompletedBossDeath`를 원래 소스로 대조한 경우에만 requester private token과 함께 `Chapter1BossPublication` 사망 객체를 한 번 발급·보관한다. 같은 tick 재조회는 같은 객체를 반환한다. FlowOwner +210은 세 실패 출처를 먼저 중재하고 그 다음 이 사망 객체를 읽는다. 따라서 같은 tick 실패가 보스 죽음보다 우선하며 -211의 다음 tick 결정 전에 중재가 끝난다. 아직 +210 중재가 끝나지 않았거나 다음 tick 계산이 overflow이면 성공 진행을 만들지 않는다.

정상 사망 때 `BlocksNewFailureRequests=true`는 FlowOwner의 실제 PostBossProgressionSession 결과에서만 설정된다. 실패가 수용되면 실패 token의 실제 전환 요청이 이미 requester에 결속되어야 하며 정상 성공 요청을 추가하지 않는다. bool 하나만으로 실패 수용·정리·성공을 발급하지 않는다.

정리 publication은 실제 `TransferSimulationDriver.LatestBossTerminalCleanupReceipt`와 결속 `OrdanBossTerminalTeardown.LatestReceipt`, requester의 실제 Transition receipt를 대조한 뒤 별도 새 불변 객체로 보관한다. death tick·정확 cleanup tick(t+1)·run/generation·실제 제거등록/영구제거/플레이어 기준값/활성대상 비움/teardown commit은 기존 receipt의 값에서만 읽는다. true 상수나 사망 객체의 값 복사로 정리 성공을 만들지 않는다. lifecycle 우선 정리이면 실제 RoomLeaving/RunFailed publication에 결속하여 suppressed로 전달한다. `OwnsChapter1Publication`은 private retained death 또는 cleanup 객체의 동일성과 requester private token·현재 entry를 검사하며 생성자 호출만으로 만든 객체를 거절한다.

## 3. 실패 승인 token과 세 실제 발행원

후보의 `RequestChapter1FailureTransition(string runKey,long generation,SimulationTick acceptedTick)`는 채택하지 않는다. 새 정확 입구는 다음이다.

```csharp
// Chapter1EncounterLifetimeOwner
void BeginChapter1Expedition(GameObject prefab, string runKey, Vector2 entry,
    Chapter1HazardDefinition hazards, string socketPlan, IChapter1BossProgressionPort port);
void RequestChapter1FailureTransition(IChapter1BossProgressionPort source,
    Chapter1AcceptedFailureToken token);
bool OwnsPendingChapter1Failure(Chapter1AcceptedFailureToken token);

// Chapter1HubLifetimeOwner.cs 안의 기존 interface
interface IChapter1BossProgressionPort
{
    bool BlocksNewFailureRequests { get; }
    bool ValidatePendingFailure(Chapter1EncounterLifetimeOwner consumer,
        Chapter1AcceptedFailureToken token);
    void ConsumePendingFailure(Chapter1EncounterLifetimeOwner consumer,
        Chapter1AcceptedFailureToken token);
}
```

port는 FlowOwner 하나이며 최초 원정 시작에 실제 lifetime과 한 번 결속되고 네 방과 보스까지 동일 객체다. 기존 `RequestChapter1BossReplacement(...,port)`의 port는 이 최초 객체와 같아야 한다. 새 run 시작의 새 flow 상태 이외 활성 원정 중 port 교체는 거절한다. interface 이름에 Boss가 있더라도 실패 검증 port는 해당 B1 원정 전체에 쓰며 Run.Unity 역참조를 만들지 않는다.

`Chapter1AcceptedFailureToken`은 기존 후보 `Chapter1HazardObservation.cs` 안의 internal sealed 불변 타입으로 둔다. 내부 생성 인자는 `(IChapter1BossProgressionPort issuer, Chapter1EncounterLifetimeOwner consumer, InputRouter router, string runKey, long generation, InputFrameCommitReceipt sourceReceipt, long arbitrationRevision, Chapter1FailureSourceKind cause, Chapter1FailurePublication health, Chapter1FailurePublication kill, Chapter1FailurePublication crush)`로 고정한다. 외부 공개 생성자나 bool 성공 생성자는 없다. 이 토큰은 기존 입력 receipt를 대체하지 않으며 전환/RunFailed/RunEndCommitted의 증거도 아니다.

발급자는 FlowOwner 하나다. `Chapter1FailureSources.CloseCompleted`가 읽은 health/kill/crush 세 원문 객체를 같은 tick의 retained 읽기 속성 `LastHealthPublication/LastKillPublication/LastCrushPublication`으로 제공한다. 반환된 실제 closed batch를 `_failure.ObserveTick`에 제출하고 `NewlyEmittedIntents`의 유효 첫 Failed 요청을 받은 경우에만 FlowOwner가 위 토큰을 만들고 private pending 슬롯에 보관한다. 토큰 revision은 실제 intent.SourceRevision이고 cause/receipt/tick/run은 실제 intent와 세 출처에 모두 일치해야 한다. 중재 실패·출처 누락·이미 소비·Block 상태에서는 토큰 발급0이다. 세 source의 revision을 먼저 반영하고 중재 실패 후 토큰을 임의 재발급하지 않는다.

`ValidatePendingFailure`는 호출 consumer가 결속 lifetime인지, 토큰이 private pending 객체와 같은지, 실제 저장 중인 `_acceptedIntent`, `_failure.Snapshot`의 Failed/AcceptedFailure/Revision, phase Failing, 현재 run/generation/router와 source receipt, 세 retained publication 객체를 전부 대조한다. 단순 값 전달이나 interface가 true를 돌려주는 것만으로 lifetime이 수용하지 않는다.

lifetime은 같은 현재 completed view와 실제 health publication 객체, `CurrentHazards.TryGetCompleted`의 kill/crush 객체 및 `Owns` 결과를 다시 독립 대조한다. 모든 issuer/token/router/run/generation/receipt와 세 kind가 정확하며 세 출처가 모두 존재해야 한다. health.Triggered는 실제 HP0과 같아야 하고 kill/crush는 현재 hazard 소유 원문이어야 한다. priority는 기존 중재의 Health→KillPlane→LethalCrush로, 토큰 cause가 해당 tick의 첫 실제 triggered 원인과 일치해야 한다. 임의 mask·복제 publication·오래된 revision·다른 generation·모두 false는 거절한다. 이 대조와 port pending 검증을 통과해야만 전환 후보가 된다.

검증과 t+1 checked 연산·현재 requester 가용성 확인 뒤 lifetime이 토큰을 private pending 슬롯에 보관하고 재진입 잠금을 건 다음 port.ConsumePendingFailure를 한 번 호출한다. 실패한 소비는 전환0·진단 중단이다. 성공 소비 뒤 regular은 기존 lifetime이, boss는 실제 결속 requester가 t+1 Transition을 한 번 요청한다. requester는 전달 owner 동일성·`OwnsPendingChapter1Failure(token)`·자기 router/entry와 retained 세 출처를 대조하고 임의 공개 token을 받지 않는다. 요청 실패 후 자동 재요청0, 이미 소비된 토큰으로 성공을 꾸미지 않는다. lifetime의 pending은 동일 객체를 clear/commit까지 retained하며 요청 소비와 정리 완료를 구별한다.

실제 Transition receipt와 RunFailed clear가 나온 뒤에만 `Chapter1LifecyclePublication`을 발급한다. `CommitChapter1FailedRun(clear,failureRevision)`은 retained token의 실제 arbitrationRevision과 clear의 owner/router/run/generation/receipt 및 정리 source를 대조하여1회 허용한다. 이 다음 실제 graph Destroy/actions dispose→실제 공개 저장 성공→ReturnedToHub 순서를 유지한다. 문자열3개·bool·새 object token만으로 어느 단계도 앞당기지 않는다.

## 4. 보스 입구와 검사

FlowOwner의 Ordan 입구는 기존 승인 §18대로 graph 로컬 `(-7,0.81)`, 속도0이며 기존 player/보스 prefab 좌표를 보존한다. `RequestChapter1BossReplacement`의 Vector2 인자는 graph 로컬 입구이고, 비활성 graph 생성 뒤 lifetime이 실제 graph root.TransformPoint로 world 위치를 계산하여 entry seed에 넣는다. regular room Entry도 동일 로컬 의미다. 잘못된 `(0,0.81)` 후보 호출을 그대로 통합하지 않는다. 실제 HP·무적 종료·연속 tick은 직전 완료 view에서만 승계한다.

기존 `Chapter1FailureSourceTests`에 정확 `RejectCopiedOrForeignAcceptedFailureToken`, `RejectStaleOrAllFalseFailureBatch`, `AcceptedFailureConsumedOnceBeforeTransition`, `SameTickFailurePrecedesBossTransition`을 추가한다. 첫 둘은 pending/producer 원문 객체 대조의 음성, 뒤 둘은 실제 장치/완료 publications와 단일 중재를 사용한다. 기존 `Chapter1ExpeditionPlayModeTests.AuthoredHubThroughFourRoomsToOrdan`에서 실제 보스 최초 완료 view의 위치·HP·tick과 requester1개를 검사하고 기존 종단 실패3원인 시험을 유지한다. 같은 receipt 재조회에서 객체 동일성, 다른 router/owner/port·복사·중복·옛 generation 거절, 실제 cleanup 없는 완료0, runtime legacy/LocalPlayable 경로 불변을 루나가 독립 검수한다. 기대값 setter·생산 ForTests·반사 호출로 정상 수용을 만들지 않는다.

테라는 현재 입력 SHA와 변경 파일을 원장에 묶고 이 정확 API의 구현 소유를 단일화한다. 루나 소스 대조 후 기존 §11.4/§11.7의 compile·집중·회귀·종단 상한으로 실행하며 이번 인계가 추가 반복 실행을 허용하지 않는다. B0 회귀와 충돌 없는 읽기/설계는 병행할 수 있으나 Unity 중 공유 프로젝트 쓰기는0이다. 본 문서만으로 실제 실패·보스 정리·B1 완료나 C1C 성공을 수용하지 않는다.
