# C4 새 요청·생명주기의 내부 연결 한정 개정

- 상태: **Approved — 한정 내부 연결 구현 승인, 실제 실행·수용 미완료**.
- 승인: 아스트라, 실제 `gpt-6-astra`, 2026-09-29. 독립 검수 `docs/verification/2026-09-29-c4-fresh-checkpoint-accessibility-luna-review.md` SHA `F16A23DAE0B43F6FB4D3274B3C6202F031B58ADDBBB5FA601734F5CAFE0ABE63`의 정적 P0/P1=0/0을 확인했다. 검수 원문 SHA `D8E0B90432CE0CE9900AABC281F3DEE05BC12475C11194631D66E7677318C988`는 `2026-09-29-c4-fresh-checkpoint-accessibility-reviewed-draft.md`에 정확 보존한다. 본문 Draft·대기 문장은 초안 작성 시점의 이력이며 현재 권한은 이 승인 상태를 따른다. 검수된 규범 본문은 그대로다. 새 소스·도구·입력·선택·계획 동결과 독립 사전 검수 후 별도 실제 실행 배분을 유지한다.
- 작성: 아스트라, 실제 `gpt-6-astra`, 2026-09-29.
- 추적: `REQ-M5D7QC4-004/006/007`, `AC-M5D7QC4-004/008/009/010`.
- 선행: Approved [r4 실행 계약](2026-09-29-c4-r4-exact-implementation-amendment.md)과 [QA r2 규약](2026-09-29-c4-qa-evidence-protocol.md). 두 원문·27개 지점·Hub 준비/의도 전달 서명은 변경하지 않는다.

실제 상위 Prepare/ack/Commit/consume는 Owner가 수행하지만 승인된 제어는 lower의 실제 실행 원장에만 보관된다. lower가 Hub를 역조회하지 않고 기존 다섯 지점을 정확 위치에서 호출할 수 있도록 아래 하나의 내부 전달 서명만 추가한다. 결과의 권한·제어 getter나 다른 제어 경로를 만들지 않는다.

```csharp
// ProfileNewGameConfirmationV1 internal partial에 한 개만 추가한다.
internal static void CheckpointFreshExecution(
    ProfileResetExecutionResultV1 result,
    ProfileResetFreshExecutionReservationV1 reservation,
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router,
    object ownerToken, ProfileResetExecutionCheckpointV1 checkpoint);
```

모든 인자는 by-value다. result/reservation은 실제 C1 Busy/NoBarrier 또는 Stale/NoBarrier에서 발급된 같은 미완료 원본 예약과 결과여야 한다. 원래 thread·owner·pair·root·epoch·C4 소비·guard·원본 제어를 관리 원장과 전체 무결성으로 인증한다. null/foreign/unregistered/unsupported/default/허용하지 않은 checkpoint는 해당 실제 단계나 제어를 실행하기 전에 거절한다. 단일 projection 손상과 실제 winning 작업의 단계 오류는 fault 선기록 후 throw하며 원래 Owner catch가 보관한 pair로 terminal 정리를 한다. 정상 closed/completed/중복 늦은 호출은 winning 성공을 다시 닫지 않는 비변경 거절이다.

허용 checkpoint는 정확 다음 다섯 개이며 순서도 고정한다.

| checkpoint | Owner에서 호출을 지배하는 실제 단계 |
| --- | --- |
| AfterFreshPrepare | 같은 reservation의 Q-A Prepare 정상 반환 |
| AfterFreshAcknowledge | 같은 reservation의 actual MatchesPreparedExecutionFresh 성공 |
| AfterFreshCommit | 같은 Q-B CommitExecutionFresh 정상 반환 |
| BeforeFreshConsume | 위 실제 ack·commit 성공 후, same Owner Hub capability를 소비하기 직전 |
| AfterFreshConsume | same Owner Hub capability 단일 소비 성공 후, lower Complete 전에 |

lower는 같은 예약별 순서·지점 도달 사건을 각각 한 번 기록하고, 저장한 실제 `IProfileResetExecutionTestControlV1.Checkpoint`에 동일 enum을 한 번 전달한다. 짧은 관리 기록 lock을 외부 callback 구간에 걸쳐 보유하지 않는다. callback throw는 원래 Owner 작업으로 그대로 전파하고 partial terminal 규칙을 적용한다. 이 API는 Prepare/ack/Commit/consume/lower Complete·guard 해제·정상 결과·새 발급을 대신하지 않는다. 다섯 지점의 정상 전달 완료가 lower Complete의 필수 선행이며 순서 누락·중복·건너뛰기를 허용하지 않는다.

lower가 Hub 실제 성공을 조회했다는 주장은 금지한다. 실제 Hub 단계→이 전달 호출의 source 지배와 실제 행의 call/권한/순서/부분 실패 증거가 필수다. 같은 예약 bearer 및 지점 enum만으로 Hub 성공이나 live permission을 제조하지 않는다. 호출자는 기존 Owner fresh orchestration의 위 실제 위치로 한정하고 다른 runtime 경로에서 호출하지 않는다.

정상 3인자 실행의 저장 제어는 fixed NoOp다. 명시 6인자 시험 실행에서만 승인된 control을 사용한다. C1/C2 제어·A/B observer·나머지22개 C4 checkpoint·Hub 일곱 서명·closed result matrix·append-only history는 그대로다. lower에는 Hub 타입·presenter/Q-B 참조·delegate authority·public ABI·정상 issuer·새 registry getter를 추가하지 않는다. private 자료구조와 source 내부 helper만 허용한다.

위 checkpoint 전달의 구현 위치는 이미 승인된 Bridge와 Owner 두 파일뿐이며 새 파일·meta·friend·asmdef·시험 경로를 추가하지 않는다. 아래 다른 한정 개정은 각 절에 명시한 기존 승인 파일만 사용한다. 기존 fresh 실패 named 시험의 실제 다섯 지점·원본 제어1회·lower Complete 전 차단·fault/native 순서·양측 슬롯 종료를 같은 schema의 행으로 검증한다. 실제 도달하지 않은 지점을 통과로 기록하지 않는다. 독립 루나 정적 검수 후 아스트라가 Approved로 전환하며, 이 문서 작성으로 추가 호출 구현·Unity 실행·전체 수용을 발급하지 않는다. 나머지 Approved 구현은 계속한다.

Approved 전환 뒤에는 이 문서가 r4의 해당 내부 callable 허용 목록, guarded 두 역사 getter의 읽기 제한, 해당 C2 시험 배치와 아래 원래 세대 사본 보존에 대해서만 우선한다. 그 밖의 결과 행렬·공개 API·파일 범위·실제 동작 요구는 r4와 QA r2를 유지한다. 선행 원문은 역사 보존을 위해 바꾸지 않는다.

## 새 Hub 예약의 준비·live 인증 조회

opaque Hub 예약의 실제 CWT는 Q-B private이며 Q-A는 그 원장을 직접 읽을 수 없다. 기존 Cancel 예약 조회를 재사용하지 않고 다음 세 bool callable만 정확 추가한다. 모두 by-value이며 정상 root/identity/proof/result·권한 객체를 반환하지 않는다.

```csharp
// HubMenuIntentHandoffOwnerV1에 두 개.
internal bool MatchesExecutionFreshReservation(
    HubNewGameExecutionFreshReservationV1 reservation,
    NewGameConfirmationOwnerV1 owner, HubMenuPresenterV1 presenter,
    InputRouter router, object nextEpochToken, long nextEpoch);
internal bool HasLiveExecutionFreshPermission(
    HubNewGameExecutionFreshReservationV1 reservation,
    NewGameConfirmationOwnerV1 owner, HubMenuPresenterV1 presenter,
    InputRouter router, object nextEpochToken, long nextEpoch);
// NewGameConfirmationOwnerV1에 한 개.
internal bool MatchesExecutionFreshCohort(
    HubMenuIntentHandoffOwnerV1 handoffOwner, HubMenuPresenterV1 presenter,
    InputRouter router, HubNewGameExecutionFreshReservationV1 reservation,
    object epochToken, long epoch);
```

MatchesReservation은 실제 Q-B 예약 CWT·같은 owner/presenter/router·token/epoch·원본 capability·Owner의 실제 Preparing operation을 인증하고 **미commit 준비 예약**일 때만 true다. Q-A Prepare가 이 성공에 의해 지배된다. HasLivePermission은 같은 CWT의 실제 commit·Q-A ack·Owner consume·lower complete·Owner 최종 Awaiting 공개 이후의 미closed 신규 슬롯에서만 true다. Preparing/부분 완료는 live=false다. 원장 history만 남았거나 다른 token/epoch·closed·foreign·unregistered·늦은 단계는 false다.

Owner의 MatchesCohort는 그 same completed fresh 예약을 보존한 실제 immutable history·현재 token/epoch/proof·원본 reciprocal 객체·state/proof를 검사한다. 실제 원래 thread를 사적 완료 operation의 증거로 먼저 확인하고 unsupported thread는 관리 false이며 Unity/payload 조회를 하지 않는다. 준비 중이나 원래 닫힌/terminal/ExecutionCommitted 상태, 이후 다른 epoch로 넘어간 history에는 false다. 현재 같은 완료 슬롯에 결속된 정상 C3 intake/capture/decision 상태의 cohort 인증만 허용하며 현재 빈 pristine 상태를 과거 commit 권한으로 해석하지 않는다. 이후 Cancel 새 세대는 기존의 실제 Cancel 예약 경로로 인증하며 이 완료 이력으로 새 live 권한을 만들지 않는다.

세 조회는 발급·consume·guard 해제·publication·native 작업을 하지 않는다. 관리 증거로 원래 thread를 확증한 뒤 등록된 원본의 단일 projection 손상은 오류를 전달하고 기존 Owner orchestration/peer fault 경계가 원본과 신규 슬롯을 닫는다. 늦은 정상 loser나 foreign false를 winner의 terminal 오류로 확대하지 않는다. lower는 이 Hub 조회를 호출하거나 Hub 타입을 참조하지 않는다. Q-A/Q-B 최초 transfer 본문·Cancel 역사·원래 taken 값을 보존하고 신규 strict 감사에서 이 세 제한 조회를 대조한다.

## 생명주기 fault의 관리 선기록

이미 승인된 Bridge의 lower internal partial에 다음 하나를 추가한다.

```csharp
internal static void RecordExecutionLifecycleFault(object initiatingHalf);
```

lower는 actual C4 consume 때 실제 두 Adapter/Router 인스턴스에 결속한 private 관리 원장을 만든다. initiatingHalf는 그 원본 한쪽의 같은 참조만 인정하며 caller 값에서 다른 pair를 만들지 않는다. C4 active 기록이 없는 기존 C2/정상 세션에는 비변경 반환한다. 등록된 실제 C4의 원래 anchor thread 확인이 먼저이며 미지원 thread는 native/payload/fault 기록 전에 거절한다. 실제 pending guard/fresh/C2 실행 단계의 원본 half만 영구 fault를 **native Disable/Dispose보다 먼저** append한다. 이미 같은 종료가 기록됐으면 native 정리를 추가 발급하지 않고 동일 관리 이력만 유지한다. 완료된 결과의 outcome·원래 typed 구성·receipt 이력은 소급 변경하지 않는다.

Adapter/Router의 실제 OnDisable/OnDestroy 및 guard 중 OnEnable 격리 실패, 해당 실제 C4 상태에서 기존 LatchResetCutoverFailure/FailBeforeReceipt로 native 정리를 시작하는 경계는 이 선기록의 성공 또는 이미 기록된 동일 fault에 의해 지배돼야 한다. 등록됐던 half의 mutable pair projection이 손상됐어도 private original association을 사용하고 foreign action을 정리하지 않는다. 관리 원장은 짧게 기록하고 native 구간에서는 lock을 보유하지 않는다. 원래 thread만 기존 native containment를 수행하며 worker 거절을 catch로 무시하고 native로 진행하지 않는다.

이 callable은 가드 Close 사건이나 정상 권한을 발급하는 대체 경로가 아니다. 기존 lower Close/Owner partial catch도 원래 영구 사건과 양측 종료 규칙을 유지한다. actual callback 재진입은 그 선기록된 fault/consume 사건에서 관리 거절되어 C1/C2를 재실행하지 못한다. 새 public API·Hub 역참조·전역 Profile writer latch·출력 getter는 없다. 이 개정의 추가 구현 위치는 기존 승인 범위 안의 Bridge, Owner, Q-A, Q-B, Adapter, Router뿐이다.

## guarded 실제 역사 pair 읽기와 커서 생성

`UiSemanticFrameCursorV1.Create`는 현재124~129행에서 원래 source의 IsFaulted 및 ReadPair를 검사하고, ReadPair182~185행은 CurrentReceipt와 CurrentUiFrame을 실제 읽는다. cursor를 수정하거나 가짜 source·receipt·frame을 만들지 않는다. C4 pending guard는 **생산·callback·모드/새 의도 발행 차단**이다. 정상 guarded 역사 pair의 읽기까지 새 authority로 취급하지 않는다.

Router는 실제 guard 진입 직전 정상 validator로 확인한 동일 원본 현재 receipt/frame pair를 사적 원본 기록으로 보존한다. guard 중 기존 CurrentReceipt/CurrentUiFrame 두 getter만 그 같은 실제 pair를 검사해 읽기 전용으로 반환할 수 있다. 현재 private value/proof와 immutable 실제 pair 및 receipt/frame 자기 검증·원래 thread·같은 미fault pending context가 모두 일치해야 한다. 다른 frame·default 제조·tick 증가·추가 빈 발행·Mode 전환·C1/C2 권한을 만들지 않는다. getter 읽기는 callback/semantic 생산/guard 해제/새 권한/Native 정리를 수행하지 않는다. terminal/fault/Closed 및 손상된 pair에는 계속 거절한다. worker는 관리 thread 확인 전에 Unity/payload를 읽지 않는다.

Q-A Prepare는 위 미commit 예약 인증 이후에만 원래 Router를 source로 원래 Cursor.Create를 호출한다. 완료 전 그 커서는 live intent를 advance/transfer하지 않는다. lower Complete와 Owner 최종 공개 이후 원래 actual production이 재개되고 첫 **새 실제 true frame**을 폐기하는 규범을 유지한다. 읽은 old pair를 새 actual true frame으로 집계하거나 UI publication0 증거를 바꾸지 않는다. 새 getter·권한 추출·cursor source 변경·map enable은 없다. `AC002_GuardedHistoryReadDoesNotPublishAndFreshCursorUsesActualPair`를 허용된 신규 focused fixture에 실제 행으로 추가하고 현재 guard 생산0·same actual pair·old cursor/tick 불변·진짜 첫 후속 발행 폐기를 검증한다.

## 실제 C2 제어 행의 시험 배치

기존 `IProfileResetMemoryCutoverControlV1`은 Profile internal enum을 받는 memory authority control을 상속한다. 편집 fixture가 friend 확대 없이 이를 직접 구현할 수 없으므로, 원래 AC007의 `AC007_ActualC2CheckpointFailureIsTerminal(memoryCheckpoint)` 19개 actual enum 행을 이미 승인된 `AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetExecutionBridgePlayModeTests`와 대응 Play fixture에 배치한다. 이는 r4 표의 해당 배치에 대한 한정 개정이며 메서드 이름·19 enum·actual C2·terminal·180초·전체 행 의미는 보존한다. 신규 class/file/friend/제어 factory를 추가하지 않는다. 최종 qualified names와 선택은 실제 Play source에서 생성해 동결하며 기존 선행610 및 새 focused 행을 흡수·삭제하지 않는다. 두 조립에서 같은 행을 중복 통과 수로 집계하지 않는다.

## 원래 요청의 숫자 세대 보존과 fresh 사전 인증

기존 lower의 RequestLifecycle에 원래 숫자 세대의 readonly 사본 `IssuerInteractionGeneration`, `IssuerDecisionGeneration` 두 개만 추가한다. 기존 실제 정상 mint의 생성자에서 실제 witness의 두 숫자를 한 번 보관한다. 새 발급 경로·CWT·정상 권한·setter는 만들지 않으며 기존 RequestIssued/Completed 사건과 소비 규칙을 유지한다. C4가 trusted 원래 thread를 확증한 뒤 전체 검증에서 witness의 두 숫자를 이 독립 사본과 각각 대조한다. 양수인 다른 숫자로 교체한 단일 projection 손상도 실제 원래 숫자의 사본으로 검출한다. 미지원 thread는 이 숫자 payload 검증을 했다고 주장하지 않는다.

Bridge의 lower partial에 다음 비소비 조회 하나만 추가한다.

```csharp
internal static bool IsFreshExecutionAvailable(
    ProfileResetExecutionResultV1 result,
    DesktopProfileLaunchAdapterV1 adapter, InputRouter router,
    object ownerToken, object committedEpochToken,
    long committedInteractionGeneration, long committedDecisionGeneration);
```

actual result/handback CWT 및 원래 anchor thread가 먼저이며, 같은 원래 pair/owner/old epoch/독립 두 세대 사본·실제 C3 Completed와 C4 소비·실제 Busy/Stale NoBarrier 행·미reserve/미fault/미closed의 관리 기록에 모두 결속할 때만 true다. caller 숫자나 boxed 결과를 authority로 해석하지 않는다. 이 조회는 payload getter·Unity·native·consume·reserve·새 발급을 수행하지 않는다. null/foreign/unregistered/worker/정상 late loser는 false로 거절하며 기존 winner의 terminal 정리를 발급하지 않는다.

Owner.AcceptFreshExecutionHandback은 최초와 작업 진입 직후에 원래 current confirmed/ExecutionCommitted 문맥과 함께 이 조회를 terminal catch 밖에서 확인한다. true 뒤 원래 thread의 전체 result/Owner/pair 무결성 검증과 실제 Reserve는 terminal catch 안에서 수행한다. 관리 인증을 통과한 원본의 projection 손상은 그 전체 검증에서 fault 선기록 후 원본·신규 양측 종료를 수행한다. Owner의 저장 `_executionResult`만을 최초 발급 근거로 삼지 않으며, 명시 lower 6인자 실행에서 발급된 동일 실제 원본 result도 같은 Owner의 원래 committed 문맥에서 받는다. 시험에서 참조를 제조하거나 저장 필드를 교체하지 않는다. 정상 3인자 NoOp 실행과 시험 제어 분리는 유지한다.

정상 actual C2 Completed의 guard 종료는 이미 완성된 실제 C2 세션·receipt·새 메모리 소유권 및 UIOnlyBlocked/양측 map 비활성을 보존하고 C4 guard만 닫는다. 이를 무조건 기존 실패 latch로 바꾸지 않는다. 실제 fault·비완료 종료 및 완료 뒤 실제 throw 지점은 영구 fault를 native 이전에 기록하고 필요한 원래 containment를 수행하며, 과거 typed 결과·receipt의 원래 발급 사실을 소급 변경하지 않는다. 이 구분은 기존 r4의 실제 결과·역사 보존 요구를 구현하는 것이며 완료 세션을 새 UI 생산 권한으로 해석하지 않는다.
