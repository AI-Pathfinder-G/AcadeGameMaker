# 2026-09-08 역할 전환 검토 및 M4B3C 터미널 teardown 사전 게이트

- 검토자: Luna (`gpt-5.6-luna`)
- 검토일: 2026-09-08
- 범위: ADR-0027 역할 전환 문서 검토, `M4B3B3 Verified` 이후 다음 단위의 런타임 사전 실패 모드 분석
- 결과: 역할 문서는 **정합(잔여 위키 문구를 보정함)**, M4B3C 구현 게이트는 **보류(계약 부재)**
- 실행 범위: 문서와 실제 런타임 정적 읽기만 수행했다. Unity 실행, PlayMode/EditMode 실행, 런타임·씬·프리팹 수정은 하지 않았다.

## 확인한 권위 문서

- `AGENTS.md`의 2026-09-08 current authority
- `docs/README.md`, `CONTEXT.md`, `docs/agent-operating-model.md`
- `docs/adr/0027-astra-orchestration-and-gpt-only-delivery.md`
- `docs/specs/README.md`, `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md`
- `docs/specs/work-contracts/2026-09-07-vd03-combat-m4b3b3-ordan-audit-pull.md`
- `docs/verification/2026-09-07-vd03-combat-m4b3b3-implementation-evidence.md`
- `wiki/Development-Model.md`, `wiki/Traceability.md`

## 1. 역할 전환 독립 검토

### 정합한 부분

ADR-0027과 `docs/agent-operating-model.md`의 current section은 다음을 일관되게 선언한다.

| 역할 | 현재 책임 |
|---|---|
| Astra | 오케스트레이션, 전역 설계·계약·캐논, 승인, 최종 통합 |
| Sol | 어려운 범위의 설계, 계약 초안, 아키텍처 반대 검토 |
| Terra | 서브시스템·콘텐츠 구현, 구현 테스트, fixture/validator/replay/Editor 도구 |
| Luna | 사전·사후 적대 QA, 독립 코드·서사 검토, 회귀·빌드 증적 |

ADR-0027은 기존 Sol 승인과 산출물을 보존하면서, 이후 승인·에스컬레이션을 Astra로 승계하고 Ollama 호출·probe·retry·schedule을 금지한다. 이는 이미 `M4B3B3`에 남아 있는 과거 Kimi/GLM/MiniMax 기록을 삭제하지 않고 역사적 증적으로 보존하는 방향과도 맞는다.

### 보정 확인 및 적용

초기 읽기에서 다음 문구가 ADR-0027의 current authority와 충돌할 수 있음을 확인했다. 검토 중 문서 소유자가 대부분을 Astra 기준으로 고쳤고, 남은 위키 추적 문구는 Luna가 보정했다.

1. `docs/specs/README.md`의 상태표·승인 조건·완료 조건은 Astra 승인·통합으로 정리됐다. 과거 승인일은 유지된다.
2. `docs/README.md`의 상태 게이트도 신규 `Approved` 전환·통합을 Astra로 정리했다.
3. `wiki/Traceability.md`의 상단 추적 흐름을 `Astra 통합 승인`으로 보정했다.
4. `AGENTS.md`는 current authority를 선두에 두고, Ollama 호출 금지와 historical approval 보존을 명시한다. 기존 하단의 오인 가능한 활성 Ollama 절차는 제거됐다.
5. `docs/agent-operating-model.md`와 `wiki/Development-Model.md`는 current workflow를 Astra→Terra→Luna→Astra로 두고, historical Ollama 배정은 실행하지 않는 기록으로 분리했다.

이 보정은 기존 `M4A`~`M4B3B3`의 Sol 승인·Ollama screening 증적을 무효화하지 않는다. 새 작업 계약 템플릿은 Astra 승인과 Terra/Luna GPT 참여를 요구한다. 역할 문서 검토 결과는 **PASS**다.

## 2. M4B3C 구현 전제

현재 저장소에는 M4B3C 작업 계약이나 구현 파일이 없다. `M4B3B3` 구현 증적의 `Unclaimed work`도 teardown, room completion, reward, persistence, scene transition을 명시적으로 다음 단위로 남긴다. 따라서 아래 결과는 구현 허가가 아닌 사전 실패 모드이며, `Approved` 계약과 Astra의 범위 승인이 먼저 필요하다.

현재 boss graph의 고정 순서는 `-200 Transfer → -190 Combat → -185 bridge → -180 exposure scheduler → -170 hostile → -165 pull → default Movement → +110 handoff`이다 (`docs/specs/vertical-demo/SYSTEM-CONTRACTS.md:55,61`, M4B3B3 contract의 exact phase horizon). M4B3C가 이 순서를 바꾸거나 기존 owner의 상태를 재구성해서는 안 된다.

## 3. 실제 런타임 읽기 증거

### 3.1 Boss terminal publication

- `OrdanBossSession.Calculate`는 current Combat state가 dead이면 최초 한 번만 `Defeated`, `RewardRequest`, `RoomCompletionRequest` 세 handoff를 같은 source tick에 정확한 순서로 발행하고, `NormalizeDefeated`로 phase를 `Defeated`로 만든다 (`Assets/AcadeGameMaker/Runtime/Combat/OrdanBossSession.cs:347-352,525-527`).
- 이후 입력은 `ValidateInput`의 normalized-dead 조건을 만족해야 하며, pending payload나 relevant Transfer edge가 있으면 거부된다 (`OrdanBossSession.cs:400-405`). 즉, teardown은 health==0을 다시 추론하지 말고 이 immutable handoff tuple을 한 번 소비해야 한다.

### 3.2 Terminal Transfer batch

- `OrdanBossTransferExposureScheduler`는 handoff가 세 항목·순서·tick까지 정확히 맞을 때만 terminal로 판정하고, production graph에서 `BossAuditBox-0, BossWeight-0, BossWeight-1, BossWeight-2` 네 removal request를 다음 Transfer tick에 병합한다 (`Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTransferExposureScheduler.cs:114-147,213-216`).
- scheduler는 그 source tick 후 `_terminal = true`가 되고 다음 `AdvanceSchedulerPhase`를 예외로 거부한다 (`...OrdanBossTransferExposureScheduler.cs:99-103,147`). 반면 hostile/pull producer는 boss-dead source tick에 suppressed terminal outcome을 발행한 뒤 FixedUpdate에서 quiescent해지도록 되어 있고, bridge/session은 normalized-dead publication을 계속 만들 수 있다. 이 비대칭을 명시적으로 정리하지 않으면 다음 fixed tick의 scheduler만 예외를 내거나, 일부 owner만 살아 있는 graph가 된다.
- `TransferSimulationDriver.MergeExactBossTerminalRemovals`는 기존 queued removal이 있으면 batch를 거부하고, exact four-ID 순서와 strict future tick을 요구한다 (`Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferSimulationDriver.cs:147-205`). M4B3C는 이를 우회하거나 unrelated queue를 silently merge해서는 안 된다.
- `TransferSession.Process`는 lifecycle이 있으면 removals를 처리하지 않고 lifecycle clear를 우선한다 (`Assets/AcadeGameMaker/Runtime/Transfer/TransferSession.cs:42-49`). non-lifecycle removal은 registration을 지우지만 `RemovalResult`는 active target이 실제로 clear된 경우에만 생긴다 (`TransferSession.cs:46-52`). 따라서 `LatestCompletedInputRemovalIds`는 완료된 **요청 목록**이지 네 target의 실제 제거 효과 증명이 아니다 (`TransferSimulationDriver.cs:29-33,46-53,280-288`). M4B3C의 cleanup receipt는 request IDs와 effect/registration state를 분리해야 한다.

### 3.3 Player continuity and handoff boundary

- `OrdanBossEncounterHandoffAdapter.AdvanceHandoff`는 +110에서 current completed player snapshot, current Transfer/Combat/bridge/scheduler/hostile/pull publications, exact movement receipt와 graph identity를 모두 요구하고 하나라도 stale/missing이면 이전 view를 보존한 채 fail-stop한다 (`Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossEncounterHandoffAdapter.cs:114-150`). Terminal view를 받은 뒤 같은 tick +110보다 먼저 component를 disable/destroy하면 마지막 presentation 증적이 사라진다.
- SYSTEM-CONTRACTS는 `BossDefeated` 뒤 입력과 failure request를 잠그고 Transfer를 정리한 뒤 `ChoicePresented(UIOnly)`로 넘기도록 정의한다 (`docs/specs/vertical-demo/SYSTEM-CONTRACTS.md:202-210`). 현재 boss runtime에는 이 input-mode/choice consumer가 없다. M4B3C가 boss teardown과 choice transition을 한 계약에 섞으면 player movement snapshot/Transfer baseline/choice lock의 ownership이 모호해진다.
- 현재 scheduler의 terminal batch는 다음 `-200`에서만 TransferSession으로 소비된다. graph를 terminal source tick의 +110에서 즉시 파괴하면 네 removal request가 실제 session에 도달하지 못한다. 반대로 다음 tick을 그대로 진행하면 scheduler가 throw한다. 이 두 조건을 함께 만족하는 explicit cleanup boundary가 필요하다.

## 4. M4B3C 사전 실패 모드

| ID | 위험 | 판정 | 계약에서 고정할 것 |
|---|---|---|---|
| M4B3C-FM-001 | scheduler는 terminal source tick 뒤 다음 advance를 거부하지만 bridge/session은 계속 publication할 수 있음 | P1 | terminal source tick, final Transfer cleanup tick, 각 owner의 quiesce/disable 시점과 publication 의무를 고정 |
| M4B3C-FM-002 | +110 전에 handoff graph를 disable/destroy하면 terminal presentation과 movement receipt가 유실됨 | P1 | terminal view를 먼저 immutable하게 소비하고, 그 뒤에만 teardown mutation 허용 |
| M4B3C-FM-003 | +110에서 즉시 graph를 파괴하면 t+1 `-200` exact four-ID batch가 TransferSession에 도달하지 않음 | P1 | cleanup을 t+1 Transfer phase까지 보장하거나, 동등한 승인된 lifecycle consumer를 정의 |
| M4B3C-FM-004 | `LatestCompletedInputRemovalIds`를 네 target의 실제 제거 증거로 오인할 수 있음 | P1 | request list, `TransferCleared`, registration/availability effect를 별도 immutable fields로 검증 |
| M4B3C-FM-005 | death와 `RoomLeaving/RunFailed/Cutscene/DemoCompleted`가 같은 tick이면 lifecycle이 removal보다 우선함 | P1 | lifecycle path는 boss success/choice를 발행하지 않고, terminal request가 처리되지 않았음을 명시 |
| M4B3C-FM-006 | 기존 queued removal이 있는 terminal batch는 preflight에서 거부되어야 하나, teardown이 이를 덮어쓸 수 있음 | P1 | foreign queue 보존·fail-stop 및 terminal latch의 atomicity를 고정 |
| M4B3C-FM-007 | 같은 terminal handoff가 반복되면 reward/room completion/scene transition이 중복될 수 있음 | P1 | exact handoff tuple과 terminal-consumption key `(encounter, sourceTick)`를 한 번만 수락 |
| M4B3C-FM-008 | player를 boss graph와 함께 destroy하면 choice scene이 사용할 authoritative pose/health/Transfer baseline이 사라짐 | P1 | player ownership과 boss-owned object teardown을 분리하고, 연속성 snapshot의 수명·소유자를 고정 |
| M4B3C-FM-009 | target collider/sink/registry 제거 순서가 physics query 또는 next Transfer capture와 교차할 수 있음 | P2 | target availability, collider disable, registration removal의 phase boundary를 명시하고 same-tick query를 금지 |
| M4B3C-FM-010 | scheduler/pull/hostile의 서로 다른 terminal latch가 부분 teardown 상태를 만들 수 있음 | P2 | 단일 teardown coordinator 또는 명시적 owner별 quiesce protocol과 idempotence를 검증 |

## 5. 가장 작은 구현 가능 첫 슬라이스 제안

전체 shutdown·choice·scene 전환을 한 번에 구현하지 말고 다음을 별도 `M4B3C-a` 계약으로 먼저 고정하는 것이 안전하다.

1. **Terminal handoff consume only:** +110의 `IsTerminal=true` view와 세 handoff를 값으로 한 번 소비하고, duplicate/stale/cross-wired 입력은 mutation 없이 거부한다. 이 단계에서는 GameObject 파괴나 scene transition을 하지 않는다.
2. **One exact Transfer cleanup tick:** terminal source `t` 뒤 `t+1`의 `-200`에서 네 request IDs가 lifecycle 없이 처리되는지 검증한다. active clear 결과 하나와 네 registration/availability 효과를 별도 receipt로 기록한다. lifecycle이 있으면 lifecycle-first 결과만 기록하고 성공 경로를 진행하지 않는다.
3. **Post-cleanup quiesce boundary:** cleanup receipt가 +110에서 확인된 뒤에만 boss scheduler/hostile/pull/bridge/combat/handoff의 추가 advance를 막는다. player controller와 choice/run owner의 수명은 이 슬라이스의 teardown 대상에서 제외한다.

이 첫 슬라이스는 room reward 소비·choice UI·scene unload를 완료했다고 주장하지 않는다. 후속 계약이 `BossDefeated → InputLocked → ChoicePresented`를 소유해야 하며, 그 계약이 player pose/Transfer baseline의 연속성을 명시한 뒤에만 실제 GameObject teardown 또는 scene transition을 허용한다.

## 6. 필요한 독립 검증 증거(실행 전 목록)

아래는 실행 결과가 아니라 M4B3C 계약이 요구해야 할 증거 목록이다.

- terminal source tick의 non-audit와 active-audit lethal 경로에서 세 handoff의 단일성·순서·tick 일치
- t+1 exact four-ID batch의 strict order, foreign queue rejection, lifecycle-first suppression
- request IDs와 actual `TransferSession` registration/sink/collider effect의 분리된 receipt
- +110 terminal view가 movement snapshot·Transfer/Combat/bridge/scheduler/hostile/pull publications와 exact tick으로 일치
- t+1 cleanup 이후 duplicate handoff, repeated scheduler advance, repeated reward/completion consume의 idempotence
- player pose/health/velocity/Transfer baseline이 boss-owned object 정리와 무관하게 choice handoff까지 보존되는지에 대한 독립 trace
- 30/60/144 render cadence에서 terminal source, cleanup tick, quiesce tick의 simulation-tick 동일성

현재는 위 목록을 실행하지 않았으므로 PASS/Verified를 부여하지 않는다.

## 판정

역할 전환은 ADR-0027과 current operating model 기준으로 정합하며, Astra 승인·Terra 구현·Luna 독립 검증·Astra 통합으로 운영할 수 있다. 기존 Sol 승인과 Ollama screening 기록은 역사적 증적으로 보존된다. M4B3C는 위 P1 위험을 계약에 반영하고 Astra가 `Approved`로 전환하기 전에는 구현을 시작할 수 없다. Luna의 현재 판정은 **역할 문서 PASS; M4B3C pre-gate 보류, P0=0, P1=8, P2=2**이다.
