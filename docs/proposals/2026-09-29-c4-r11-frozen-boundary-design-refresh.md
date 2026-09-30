# C4의 R11 동결 경계와 제한 설계 갱신

2026-09-29. 솔, 실제 `gpt-6-sol`. 읽기 전용 설계 제안이다. 원본 계약·소스·설정·QA·진행 중 캡처를 변경하지 않았으며 Git·네트워크·Unity·컴파일 실행도 없다. C4는 **Review**다. 이 문서는 Approved 전환·구현·정상 결과 발급의 권한이 아니다.

## 현재 선행 상태와 정확 근거

[ADR-0036](../adr/0036-staged-confirmation-and-execution-acceptance.md) 6~12행은 C3 선행 단계의 독립 수용과 실제 C4 결과를 사용한 공동 최종 검증을 구분한다. SHA `627D7CE2F948F5A7BBECE2907C772321CBDB450212780D0D281227776D9BB8A3`다. [C4 원본](../specs/work-contracts/2026-09-28-vd09-m5d7q-c4-reset-execution-bridge.md) SHA `3F29B8BC7D3F0172717C0FC94101BF4086FEE1FE475B65A3B2E67E0AE4FA5DC0`의 326~327행에 있는 C2 “구현 중” 문구는 현재 사실과 다르지만 이번에 원문을 수정하지 않는다.

- [C1 계약](../specs/work-contracts/2026-09-28-vd09-m5d7q-c1-reset-disk-transaction.md) 2/8행은 아스트라의 디스크 범위 Verified다. SHA `D03C77478D2EC4214A225E4EA46B993E59C753C6EB883BB0C913491D3068E455`. [C1 루나 검수](../verification/2026-09-28-vd09-m5d7q-c1-final-luna-review.md) 30~32/84~96행은 R12 212, 실제 작업자 R13 51, Profile R14 175와 독립 판정을 기록한다. SHA `A16DF015F805F73507B65378497F13DF02B637F711D53F4793FE5212B8EA4B00`.
- [C2 계약](../specs/work-contracts/2026-09-28-vd09-m5d7q-c2-memory-cutover.md) 2/8/503~523행은 아스트라의 합성 범위 Verified와 기존 613/610 독립 수용을 명시한다. SHA `F76E9AF2B7A9A2E936A8E2253A3E1FDA7E94E164DC0FBCBC08BBF2BAA4D1F5DF`. [C2/C2R 최종 루나](../verification/2026-09-28-vd09-m5d7q-c2-c2r-luna-final-independent-acceptance.md) 17~27/43~60행은 정확 원장·실행 분할과 P0/P1=0을 기록한다. SHA `3EE81482D43D5039E3D412EF3C38400F6A7E4903541DCDE29D77E7BD3A733A8E`.
- 현재 C2 `ProfileResetMemoryCutoverV1.cs` 지문은 수용 근거 `artifacts/c2-r36-source-before.json`의 808~809행과 같다. 그 원장 SHA `2CAB80CC20A01165778905F67161866906F439C6E5AAFC5E316D1813628F5EA4`는 최종 루나 17~19행에 직접 연결된다. C1의 현재 지문은 과거 C1 검수 18행의 지문과 다르며, [C3L 통합 수용](../approvals/2026-09-29-c3l-integration-acceptance.md) 17행의 정확 후속 지문으로 연결한다. 과거 수용 바이트를 현재 파일로 바꾸어 기록하지 않는다.
- C3는 R11 동결 14파일 원장 `0A36C9E7CCF3EBBF477B96C1B742344021D642586AD906042E48C755258571EB`에 대해 Play 15개와 Edit 240개(서로소 행렬 91개+나머지 149개)의 실제 통과가 각각 부분 수용됐다. [Edit 집중 제한 수용](../approvals/2026-09-29-c3-r11-edit-focused-partial-acceptance.md)과 [루나 독립 결과 검수](../verification/2026-09-29-c3-r11-edit-focused-240-luna-result-review.md) SHA `DB6B08D6CBC3FF8CF5C4381D6EBA24C6471C16E6C2FEE276983AEE6F383D3013`를 따른다. 현재 필수 562개는 진행 중이고 51/610은 미시작이며 그 결과를 추정하지 않는다. ADR의 C3 선행 전체 수용은 아직 수행되지 않았고 C3 전체 Verified도 아니다. AC-M5D7QC3-007/008은 미완료이며 C4는 Review로 유지한다.

## 실제 조회 소스 지문

경로 접두사는 `Assets/AcadeGameMaker/Runtime/`다. 아래 줄 번호는 이번 현재 바이트 기준이다.

| 파일 | SHA-256 | 확인 경계 |
| --- | --- | --- |
| `Profile/ProfileResetDiskTransactionV1.cs` | `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2` | Begin 660~680행, 실제 identity·root와 새 관찰 비교 |
| `Input/Unity/ProfileResetMemoryCutoverV1.cs` | `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275` | FinalizeReset 97/133~175행 |
| `Input/Unity/DesktopProfileLaunchAdapterV1.cs` | `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB` | 210~240행의 실제 launch cohort/root, 530~556행의 terminal 진입 |
| `Input/Unity/InputRouter.cs` | `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB` | 40행 억제 필드, 484행 실패 폐쇄, 1014~1045행 UI 격리/억제 해제 |
| `Input/Unity/ProfileNewGameConfirmationV1.cs` | `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1` | 17행 불투명 요청, 69~76행 private 원본 증거, 214~264행 commit |
| `HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` | `CAEBD7B5DA99AF94A25C5C300789AA48CA87908CA6D8894B99AE49C2391D7043` | 294~320행 Cancel 전용 Rearm, 322~347행 실행 직전 폐쇄 |
| `HubPresentation/Unity/HubMenuPresenterV1.cs` | `393B325BA5B8F6D79CA983F1A91098215E7A6993DE37E9F5D98BA775760E4900` | 220~239행 최초 결속·살아 있는 결속·Cancel successor |
| `HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs` | `F47C61A043DAF4695CC53E41C6BE9BB67CA147C722F7B830459AA6304B45B3B5` | 201~207행 실제 발급, 221~226행 실제 소비, 262행 Cancel successor 권한 |

## C1/C2와 C3의 접점

C2의 실제 경계는 `internal static ProfileResetMemoryResultV1 FinalizeReset(string root, ProfileResetDiskPreparedProofV1 proof, DesktopProfileLaunchAdapterV1 owner, InputRouter router)`다. 97행은 실제 no-op control을 사용한 133행의 다섯 인자 구현으로 들어간다. 136행은 정확 pair/root를 먼저 검사하고, 145~155행에서 실제 root 정규화·lease 재획득·prepared proof 재인증을 한다. 160~163행은 기본값 staging→terminal→actions 전환→메모리→barrier 삭제→최종 재관찰→정확 receipt를 수행한다. 최초 lease Busy만 150~151행에서 비terminal 결과이며, 165~175행의 나머지 오류는 fail-stop이다. C4는 이 Busy도 **C1 DiskPrepared 이후**이므로 terminal로 처리해야 한다.

현재 C3의 실제 메서드 이름은 초안의 `CommitExecution` 대신 `CommitForExecution`이다. Owner 333~339행은 lower reserve→모든 확인/rearm 권한 무효화→Q-A/Q-B interaction 폐쇄→ExecutionCommitted→lower complete이며 동일 불투명 요청을 반환한다. lower 259~261행은 CAS 1→2와 append-only Completed 이력을 보존한다. 이는 C3 commit의 완료이며 아직 C4의 별도 실행 소비는 아니다. 현재 `ConfirmedProfileResetRequestV1` 생성자가 internal이어도 미등록 후보는 정상 권한이 아니다. 실제 권한은 private CWT와 원본 capture/source/owner/epoch 이력에 있다.

다음 기술 선택을 제안한다. 모두 아스트라 승인과 루나 설계 검수 전 구현하지 않는다.

1. **C4 소비와 private identity 접근:** lower의 기존 `RequestLifecycle.Gate` 안에서 실제 Completed 이력·원본 capture·pair/root·세대를 재인증하고 별도 private 실행 이력을 한 번 등록한다. 앞선 인증 뒤 늦게 도착한 경쟁자는 gate 획득 후 재인증으로 비변경 거절한다. C3 Completed 이력은 덮어쓰지 않는다. 단일 가변 CAS 값만으로 권한을 복구할 수 없도록 실제 소비 사건과 정확 원본 참조를 함께 검증한다. 별도 C4 소비 이후의 예외는 양측 가드의 전체 폐쇄를 보장한다.
2. **컴파일 가능한 private 접근:** 기존 lower 선언 58행을 `internal static partial class ProfileNewGameConfirmationV1`로 좁게 바꾸고, 신규 `ProfileResetExecutionBridgeV1.cs`에 같은 partial의 executor 본문을 두는 안이 작다. 그 본문만 private witness에서 `Capture.Identity`를 꺼내 실제 Begin에 전달한다. Hub에 identity/root/document/proof getter나 registry 접근을 공개하지 않는다. `ExecuteConfirmedReset(confirmed, adapter, router)`는 내부 불투명 intake이며 Hub 타입 인자는 없다. 정상 요청·결과를 반사 또는 시험용 발급기로 제조하지 않는다.
3. **상호 가드와 C2 진입:** ResetExecutionPending은 C2 terminal과 다른 private 상태다. 처음부터 기존 terminal/fault 플래그를 사용하면 Adapter 212행과 C2 136/543행의 정상 eligibility를 깨뜨릴 수 있다. 정확 원본 cohort와 private 실행 등록에 결속된 별도 가드 두 절반을 두고, 일반 관찰/저장/notification/메뉴·semantic 발행은 차단한다. UI 격리 해제나 OnEnable도 pending 억제를 풀 수 없어야 한다. C2에는 실제 준비 proof와 같은 guarded pair/root가 일치하는 경우만 허용하는 좁은 내부 분기를 제안한다. 일반 `HasExactResetLaunchCohort`를 느슨하게 만들어 모든 C3 호출이 pending 상태에 들어오도록 하지 않는다. C2 proof 재인증·staging·terminal·receipt 알고리즘은 유지한다.
4. **결과의 실제 출처:** C4 결과·fresh handback은 실제 executor의 private 발급 이력과 전체 C1/C2 typed 객체에 연결한다. enum/tx/hash를 조합한 값은 권한이 아니다. Completed에는 같은 실제 C2 receipt만, Busy/Stale에는 완전히 검증된 C1 NoBarrier 행과 일회용 fresh 권한만 둔다. C3가 이 실제 결과를 검증하는 출처 경계를 함께 설계하며, 정상 결과 constructor를 시험용 통로로 개방하지 않는다.

## Busy/Stale의 fresh cohort는 별도 최소 보정이 필요하다

실행 반례가 아니라 **현재 정적 계약 접점의 공백**이다. Owner Rearm 304행은 Cancelled만 허용한다. 실행 commit은 이미 Q-A/Q-B를 닫았으므로 Presenter 227/239행의 살아 있는 결속 및 Q-B 262행의 `IsRearmInProgress`도 충족하지 않는다. 최초 결속 222행은 재결속을 허용하지 않는다. 따라서 C4 Busy/Stale를 Cancel/Rearm으로 가장하거나 기존 closed 플래그를 되돌려 처리할 수 없다. C4 원본 301~303행의 Q-A/Q-B 수정 금지와 AC004의 실제 fresh cursor 요구를 함께 만족할 구체적 후속이 아직 없다.

제안하는 최소 API는 다음 세 단계다. 명칭은 설계 후보이며 현재 API가 아니다.

- lower `ProfileResetFreshC3HandbackV1`: 실제 C1 Busy/ConfirmationStale **NoBarrier**만 발급하는 getter 없는 내부 불투명 값. 같은 소비된 request·실행 generation·adapter/router/root·원래 owner/epoch를 private CWT에 고정한다. 결과에서 정상 소유자가 반환받은 바로 그 객체만 한 번 reserve할 수 있다. 외부 값·복사·기본·종료된 실행·post-barrier 결과는 거절한다.
- Owner `AcceptFreshExecutionHandback(result)`가 실행 gate 안에서 실제 issuer/pair/owner/기존 commit과 위 객체를 재인증하고 lower reserve를 한다. 기존 ExecutionCommitted epoch는 immutable history로 남긴다. 신규 `HubNewGameExecutionFreshReservationV1`에는 다음 epoch token, 실제 owner/Q-A/Q-B/router 참조와 **현재 미완료 예약**을 private 원장에 결속한다. Q-A/Q-B에는 lower Profile 형식·identity·C1/C2 결과를 전달하지 않는다.
- Q-B `ReserveExecutionFresh(...)` → Q-A `PrepareExecutionFresh(...)`/실제 acknowledgment → Q-B `CommitExecutionFresh(...)` → Owner consume 및 lower handback complete 순서다. 기존 Cancel successor API와 guard는 그대로 유지한다. 각 단계는 Owner의 별도 `IsExecutionFreshInProgress(...)`와 정확 actual reservation을 확인한다. 새 살아 있는 interaction 슬롯만 pristine하게 만들고 기존 Q-A retained/cursor/transfer 및 Q-B legacy/taken/epoch 이력과 완료·closed 상태는 보존한다. 새 슬롯의 controller/cursor를 정상 factory로 생성하며 ready 여부와 무관하게 첫 실제 true 프레임을 baseline으로 폐기한 뒤에만 새 선택을 허용한다. 입력/action/map 교체나 추가 빈 발행으로 baseline을 우회하지 않는다. partial reserve/prepare/ack/commit/consume 오류는 원본 pair와 신규 슬롯 모두 terminal이며 같은 handback을 되살리지 않는다. 양측 fresh 슬롯이 결속되기 전에는 execution 가드가 새 UI 발행을 허용하지 않는다.

이 안에는 **별도 좁은 Q-A/Q-B 허용 목록 보정**이 필요하다. 아스트라가 C4 Review를 갱신할 때 허용 여부를 기술적으로 결정하고 루나가 AC004/008과 기존 Approved Q-A/Q-B의 상호 결속·단일 take·역사 보존을 재검수해야 한다. 제품 결정은 추가하지 않는다. Q-A/Q-B에 Profile/C1/C2/실행 동작을 옮기거나 일반 재개 통로를 만드는 대안은 배제한다.

승인 검토에 사용할 정확 내부 서명 후보는 아래와 같다. 기존 공개 API는 아니며 구현된 것으로 간주하지 않는다. `HubNewGameExecutionFreshCapabilityV1`은 Owner가 실제 lower handback reserve 뒤에만 등록하는 getter 없는 Hub 값이고, 예약은 Q-B의 private CWT에 실제 capability와 같은 pair/token/epoch로 등록한다. 후보 객체를 만들 수 있는 생성자가 있더라도 미등록 객체는 거절한다.

```csharp
// NewGameConfirmationOwnerV1
internal void AcceptFreshExecutionHandback(ProfileResetExecutionResultV1 result);
internal bool IsExecutionFreshInProgress(
    HubNewGameExecutionFreshCapabilityV1 capability,
    HubMenuIntentHandoffOwnerV1 handoffOwner, HubMenuPresenterV1 presenter,
    InputRouter router, object nextEpochToken, long nextEpoch);
// HubMenuIntentHandoffOwnerV1
internal HubNewGameExecutionFreshReservationV1 ReserveExecutionFresh(
    HubNewGameExecutionFreshCapabilityV1 capability,
    NewGameConfirmationOwnerV1 owner, HubMenuPresenterV1 presenter,
    InputRouter router, object nextEpochToken, long nextEpoch);
internal void CommitExecutionFresh(
    HubNewGameExecutionFreshReservationV1 reservation,
    NewGameConfirmationOwnerV1 owner, HubMenuPresenterV1 presenter,
    InputRouter router, object nextEpochToken, long nextEpoch);
// HubMenuPresenterV1
internal void PrepareExecutionFresh(
    NewGameConfirmationOwnerV1 owner,
    HubNewGameExecutionFreshReservationV1 reservation,
    object nextEpochToken, long nextEpoch);
internal bool MatchesPreparedExecutionFresh(
    NewGameConfirmationOwnerV1 owner,
    HubNewGameExecutionFreshReservationV1 reservation,
    object nextEpochToken, long nextEpoch);
```

Q-B의 commit은 Q-A의 정확 acknowledgment를 먼저 요구하고, 완료/폐쇄 이력 조회는 미완료 예약의 live permission으로 사용하지 않는다. Owner의 최종 consume 이후에만 새 세대와 살아 있는 결속을 공개한다. 가드 해제도 그 완료에 결속된 lower 실제 proof만 받는다. 가변 참조 여러 개를 순차 대입한 사실만으로 상호 결속을 인증하지 않는다.

## 정확 허용 경로 제안과 감사 영향

C4 원본 264~279행의 예정 경로를 실제 이름으로 확정할 후보는 다음과 같다. 지금은 편집 허용 목록이 아니다.

- 신규 `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs` 및 `.meta`.
- 같은 디렉터리의 기존 `ProfileNewGameConfirmationV1.cs`, `DesktopProfileLaunchAdapterV1.cs`, `InputRouter.cs`; `ProfileResetMemoryCutoverV1.cs`는 정확 guarded pair 진입 분기가 필요한 경우에 한정.
- 기존 `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs`: 실제 executor 호출·결과 인증·fresh reciprocal orchestration.
- 별도 amendment 후보 두 파일: 같은 디렉터리의 `HubMenuPresenterV1.cs`, `HubMenuIntentHandoffOwnerV1.cs`. 위 fresh reserve/prepare/ack/commit 및 새로운 살아 있는 슬롯의 읽기/단계 처리만. 기존 Cancel/Rearm/legacy take와 immutable history는 유지.
- 신규 focused 시험은 기존 `InputUnity`/`HubPresentation` EditMode·PlayMode 조립 안의 독립 파일로 정확 경로·class·선택을 승인 시 동결한다. 이미 선택된 기존 회귀 class에 행을 추가해 562/610 이름을 바꾸지 않는다. 신규 메타와 정확 선택·원장·증거·README 최소 링크는 별도 목록으로 명시한다.

Q0 Adapter/Router pin은 원본 C4 281~299행에 따라 파일이 실제 바뀐 경우만 별도 감사 amendment와 엄격한 현재 successor를 요구한다. 원래 9/7/2행, adapter 현재 `1798...`와 router `66CD...` 및 수용된 후속을 보존한다. Q-A/Q-B 후속 역시 원래 audit 본문·소스 지문·허용 예외와 successor 증거를 보존하고 새 파일의 exact SHA를 검수한다. Q-B source의 금지 token 검사를 넓게 풀지 않는다. 새 Hub 중립 reservation과 owner 검증만으로 금지된 Profile/gameplay/실행 호출을 도입하지 않는지 strict focused audit를 분리한다. 기존 감사 경로를 고쳐야 한다면 runtime 허용 목록만으로 추정하지 않고 그 정확 시험 경로의 별도 승인을 먼저 받는다.

Profile/C1 알고리즘·proof/result, asmdef/friend/public ABI, 자산·action wrapper·Packages·ProjectSettings·장면·prefab·실제 UI wiring은 금지 경계를 유지한다. 새 Q-A/Q-B 두 파일 amendment를 승인하지 않는다면 fresh handback AC004를 구현 가능하다고 단정하지 않으며 C4는 Review를 유지한다.

## AC별 남은 설계·증거

모든 행은 `AC-M5D7QC4-*`다. 아래는 시험 설계이며 실행 결과가 아니다.

| AC | 연결 REQ | 최소 결정과 실제 증거 공백 |
| --- | --- | --- |
| 001 | 001 | 실제 C3 발급→commit→C4 소비의 같은 opaque 참조. foreign/default/미등록/미commit/다른 root·pair·generation/이미 소비·반사 손상과 늦은 경쟁자에서 C1 0회. 무결성 오류와 정상 완료 loser를 구별 |
| 002 | 002 | 소비 전/후와 adapter·router 각 가드 사이의 throw-only 지점. 부분 진입은 양쪽 terminal; semantic/메뉴/notification/save 차단. 기존 UI quarantine 해제가 guard 억제를 풀지 않음 |
| 003 | 003 | 실제 Begin 1회와 private 원본 identity 참조; C1 전체 결과 검증 및 unknown/default/중첩 손상 음성 행. 시험 hook는 throw만, 실제 결과 대체 금지 |
| 004 | 004 | 실제 root contention의 Busy 및 실제 bytes 변경의 ConfirmationStale NoBarrier로만 fresh 발급. old request/decision/rearm/handback의 거절, 신규 epoch·cursor 첫 true 폐기·다음 새 take/결정. prepare/ack/commit/consume 각 partial 실패의 양측 폐쇄. ManualRepair/Reload/불확실 결과에서는 fresh 발급 0회 |
| 005 | 005 | 실제 DiskPrepared 전체 결과와 동일 PreparedProof/root/pair를 실제 FinalizeReset에 1회. foreign/replaced/consumed proof 및 pair/root drift에서 C2 memory 변경 전 terminal |
| 006 | 002/006 | 실제 C1 durable checkpoint→반환→C2 lease 획득 창마다 disk barrier와 runtime guard가 함께 차단. lease를 C1에서 C2로 전용하거나 guard를 durable barrier로 부르지 않음 |
| 007 | 004/005 | 실제 C2 Completed receipt의 같은 참조만 성공. C2 최초 Acquire Busy도 post-DiskPrepared terminal. Reload/ManualRepair/throw에서는 Cancel/fresh/재시도 불가 |
| 008 | 001/006 | 동시 executor·늦은 gate 획득·동시 fresh reserve/commit에서 단일 승자. 실제 Disable/Destroy와 부분 fresh 실패 뒤 원본 이력·영수증 보존. 외부 callback 도달이 없는 구간의 재진입은 구조 근거로 구분하며 실행했다고 꾸미지 않음 |
| 009 | 007 | private registry/identity/proof가 Hub로 노출되지 않음. lower에 Hub 참조 없음, public ABI/asmdef/friend 불변. 새 Q-A/Q-B strict audit와 허용 목록 검수. 제품·네트워크·시계·난수·ordinary save 권한 없음 |
| 010 | 007 | 승인 후 최종 파일 지문·정확 focused 선택·현재 필수 회귀 이름/원장·377행 및 실제 종료/건너뜀/판정 불가/중복/전후 지문 검증. C3 선행 수용 뒤 구현하고 C3 잔여 AC007/008과 C4 전체 공동 최종 검수 |

현재 필수 회귀가 진행되는 동안 위 파일은 동결한다. 우선 필요한 결정은 private 소비/가드 설계, C2의 guard-aware 정확 진입, Busy/Stale 전용 fresh 슬롯과 그 두 파일 amendment, 엄격 감사·시험 경로의 동결이다. 아스트라의 별도 계약 승인과 루나 설계 검수 후에만 테라 구현을 진행한다. 기존 사용자 제품 결정을 다시 요구할 이유는 없다.
