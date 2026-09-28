# C3 확인 소유자와 재무장 구현 순서

- 날짜: 2026-09-28
- 상태: 구현 준비 전용. C3L 집중 검증의 독립 수용과 아스트라의 시작 지시 전에는 아래 본체 소스와 시험을 변경하지 않는다.
- 근거: Approved C3 계약, Sol의 epoch-handshake 반대 검수, C3 기존 경계 지도·fixture 구성·시험 행렬, 아스트라의 adapter 후속 SHA 승인.

## 동결 및 경계

현재 C3L 관찰 단위가 실제 Unity 집중 검증 중이다. 그 수용 전에는
`ProfileResetDiskTransactionV1.cs`, `ProfileNewGameResetServiceV1.cs`,
`ProfileNewGameConfirmationV1.cs`, `DesktopProfileLaunchAdapterV1.cs` 및
모든 C3L 시험을 포함해 C3 본체 소스를 변경하지 않는다. 승인된 adapter
SHA `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB`는
그대로 보존한다.

구현은 `Hub.Presentation.Unity -> Input.Unity -> Profile` 방향만 따른다.
새 asmdef, friend assembly, 공용 ABI, Hub에서 Profile 내부 타입을 노출하는
반환값은 허용하지 않는다. Hub에는 불투명한 Input 결과만 전달한다.

다음 기존 기록은 불변 역사다.

- Q-A `IntentRetained`와 그 원래 intent/cursor/controller 기록
- Q-B `RequestTaken`, `_takenRequest`, 원래 request와 발급 영수증
- launch/current/handoff receipt, router/actions, 통지의 해제 상태
- C2/C2R 지문 역사와 Router 현재 엄격 지문

미래 C4의 실제 발급, C1 `Begin` 호출, C2 실행, 맵 활성화, 장면·프리팹·실제
확인 UI 작성은 이 작업 범위에서 금지한다.

## 실제 수정 허용 목록

| 순서 | 파일 | 허용되는 좁은 변경 |
| --- | --- | --- |
| 1 | `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs` | 실제 `TryTakeRequest` 전이에서만 private Q-B 발급 증거를 등록하고, append-only successor epoch/rearm을 추가한다. 기존 take 본문·역사값은 재작성하지 않는다. |
| 2 | `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | 실제 C3L 발급 capture만 소비하는 불투명 confirmed-request/결과/재검증 경계를 완성한다. Hub 타입 참조와 외부 발급 factory는 두지 않는다. |
| 3 | `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` 및 `.meta` | 새 synthetic-only 소유자를 추가한다. intake, retry, 결정 CAS, 재검증, cancel, rearm 조정, 실행 직전 폐쇄 permit만 둔다. |
| 4 | `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs` | owner-authenticated successor만 수용한다. 새 controller/cursor, 통지 해제 보존, AwaitingBaseline, 첫 실제 연속 frame 폐기를 구현한다. |
| 5 | `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileNewGameConfirmationV1Tests.cs` 및 `.meta` | Input lower capture·opaque request·closed proof 집중 검사를 추가한다. |
| 6 | `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/NewGameConfirmationOwnerV1Tests.cs` 및 `.meta` | intake, copy, capability CAS, recapture, cancel, pre-barrier 상태를 실제 cohort fixture로 검사한다. |
| 7 | `Assets/AcadeGameMaker/Tests/PlayMode/HubPresentation/NewGameConfirmationRearmPlayModeTests.cs` 및 `.meta` | 실제 successor epoch, baseline 폐기, lifecycle/fault closure를 검사한다. |

`DesktopProfileLaunchAdapterV1.cs`의 승인 조회 함수와 C3L Profile 관찰 소스는
새 C3 본체 단계에서 수정 대상이 아니다. 추가 수정이 필요해지면 새 계약과
아스트라의 정확 SHA 승인을 먼저 받는다.

## 최소 일관 구현 단위와 순서

### 1. Q-B private issuance

`HubMenuIntentHandoffOwnerV1.TryTakeRequest`의 실제 성공 전이 끝에서만 private
발급 증거를 등록한다. 증거는 실제 request, handoff owner, presenter, router,
adapter/cohort 및 strictly increasing epoch를 참조 동일성으로 묶는다. 값이 같은
복사 request, 외부 receipt, 기본값, 반사 변조, 이전 epoch는 모두 거절한다.

successor rearm은 기존 `_takenRequest`를 지우거나 `_state`를 RequestTaken 이전으로
되돌리지 않는다. 별도 append-only epoch와 신규 한 번의 request slot만 만든다.

### 2. lower opaque confirmed request

`ProfileNewGameConfirmationV1`에서 adapter의 승인된 root 조회와 C3L의 실제
capture registry만 사용한다. 실제 세 leaf identity/projection, root, opaque owner,
epoch, 분류와 capture 발급 증거가 모두 일치할 때만 불투명 confirmed request를
발행한다. 실제 capture 재확인에서 Busy는 동일 결정의 명시 retry만 허용하며,
Unreadable 및 예상 밖 원인은 terminal로 닫는다.

이 단위는 Hub request/presenter를 알지 못한다. 결과 행은 private registry와
readonly verifier로 닫고, 복사·reflection·owner/epoch/classification 변경은
property와 Validate 모두에서 거절한다.

### 3. Hub confirmation owner

새 `NewGameConfirmationOwnerV1`는 Q-B 실제 발급 증거와 request topology를 먼저
검증한 뒤 lower 경계를 호출한다. DecisionRequired에는 계약의 정확한 한국어 문구와
두 label만 노출하고 identity/document는 노출하지 않는다.

Confirm/Cancel은 owner, presenter, Q-B owner, adapter, router, root, epoch, request,
capture에 참조 동일성으로 묶인 private capability의 세대별 CAS를 사용한다.
Confirm은 완전한 세 leaf identity를 재포착해 비교한다. 같으면 ConfirmedReady,
변경이면 새 decision generation, Busy면 같은 pending capability, unreadable/예상 밖
실패면 terminal이다. Cancel은 profile bytes, memory, maps/actions, receipt, scene/run
state를 바꾸지 않고 `Cancelled` 기록만 남긴다.

### 4. reciprocal rearm과 baseline

Cancel 후 owner는 취소 epoch에만 유효한 rearm capability를 발행한다. Q-B가 새
epoch와 request slot을 append-only로 준비한 뒤, Q-A presenter가 동일 successor를
검증해 새 controller와 cursor를 만든다. 기존 통지가 이미 해제되었다면 기존 해제
방법으로 같은 상태를 유지한다.

successor는 `AwaitingBaseline` witness가 남아 있는 동안 ordinary `Update`로 Ready가
될 수 없다. `FixedUpdate`는 정확한 새 cursor의 `TryAdvance`만 호출한다. false는
계속 quarantine, 첫 true의 완전한 frame은 validate 후 폐기하며 `Interpret`와
`Activate`를 호출하지 않는다. 그 다음 frame부터만 Ready/입력이 가능하다. skipped,
malformed, foreign cursor, lifecycle 및 witness fault는 모든 successor를 닫는다.

### 5. 실행 직전 폐쇄 permit

C3는 Confirmed request를 단 한 번 terminal consume하고 confirm/cancel/retry/rearm
capability를 먼저 무효화한 private executor permit까지만 제공한다. 이 단계는 C1
`Begin`을 호출하지 않으며 C4 Review 또는 실제 실행 권한을 만들지 않는다. 이후의
Busy/ConfirmationStale와 Reload/ManualRepair 경로는 Approved C3의 typed report
계약으로만 모델링하고, C4 구현은 보류한다.

## 시험 행렬과 증거

모든 정상 fixture는 실제 adapter/router/latch/presenter/Q-B lifecycle로 cohort와
request를 발급한다. identity, projection, receipt, request, cursor, semantic frame을
반사로 정상화하거나 fabricated 성공값으로 대체하지 않는다. 반사는 negative
provenance 검사에만 사용한다. fixture마다 profile bytes, actions/maps, launch/current/
handoff receipts, 통지, scene/run 상태와 Q-A/Q-B 역사를 전후 비교한다.

| 수용 기준 | 집중 증거 |
| --- | --- |
| AC-M5D7QC3-001 | 정확한 NewGame 단 한 번의 intake, private Q-B issuance, foreign/duplicate/value-copy/reflection/epoch/topology 거절 |
| AC-M5D7QC3-002 | Primary/Previous/Temp missing/default/meaningful/invalid/unsupported/recovery/unreadable 조합과 우선순위, 각 제품 필드 변화 |
| AC-M5D7QC3-003 | 정확한 한국어 문구와 label, default/foreign/copied/late/concurrent/reentrant Confirm·Cancel의 한 승자 CAS |
| AC-M5D7QC3-004 | display 이후 모든 leaf 추가·삭제·bytes/identity/projection 변화, default 전환, stale identity 미전달 |
| AC-M5D7QC3-005 | Cancel의 bytes/memory/actions/maps/receipts/scene 중립성, old Q-A/Q-B 불변과 새 epoch 단일 selection |
| AC-M5D7QC3-006 | notice 보존, AwaitingBaseline의 false 유지·첫 true 폐기·다음 frame만 전달, fault closure |
| AC-M5D7QC3-007 | initial/confirm Busy의 동일 ownership, typed stale과 terminal unreadable/reload/manual 경계 |
| AC-M5D7QC3-008 | execution-commit 전 capability 전체 무효화와 permit 발급 fault containment, C1 Begin 미호출 |
| AC-M5D7QC3-009 | 허용 목록·assembly 방향·public ABI·C2/scene/map/UI/C4 발급 금지 정적 점검 |
| AC-M5D7QC3-010 | focused EditMode/PlayMode와 지정 영향 회귀의 고정 입력·결과 ledger, Luna 독립 P0/P1 검수 |

## 시작 및 중단 조건

시작 전에는 C3L R3 실제 Unity 결과, frozen input 지문, Luna 독립 검수와 아스트라
수용을 확인한다. 이 중 하나라도 없으면 이 문서만 유지하고 C3 본체 구현을 시작하지
않는다.

구현 중 다음 사실이 나오면 즉시 아스트라에 중단·상승 보고한다: 원래
IntentRetained/RequestTaken을 보존할 수 없음, 권한에 값 동등 비교가 필요함,
successor pending을 ordinary Update보다 먼저 둘 수 없음, 새 public ABI/asmdef/friend가
필요함, C1 Begin 또는 C2/맵/장면/UI를 호출해야만 검증 가능함.
