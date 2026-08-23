# [VD-05] Failure and Persistence

- Status: Review
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Scope

런 상태, 승인된 실패 원인, 단일 종료 커밋, 거점 복귀와 P0 보존·초기화 경계를 포함한다.

## Non-scope

구체 JSON 스키마·파일 경로·손상 복구 구현, 체크포인트와 활성 런 재개는 포함하지 않는다.

## Contract

원정 시작, 진행, 성공, 실패, 거점 복귀의 상태 전이와 원정 자산·지속 성장·서사 상태의 소유 경계를 정의한다. 저장 필드 형식은 Sol의 플랫폼 계약에 둔다.

### Inputs

체력 0, 낙하 한계, `Lethal` 압착 위험, 원정 시작 시드, 선택 확정, `DemoCompleted`, 앱 재시작과 저장 성공·실패 결과.

### Outputs

`RunEndRequested`/`RunEndCommitted`, 거점 복귀, 보존 snapshot 요청, 활성 런 폐기 또는 진단 가능한 저장 오류를 낸다.

### Owned state

런 phase, 종료 원인, 원정 중 임시 상태의 초기화와 성공/실패 종료 중복 제거를 소유한다. 버전 저장 필드와 입력 바인딩은 소유하지 않는다.

### Invariants

- 현재 체력·위치·속도, 활성 전이·대상 참조, 방 순서·현재 방·적·위험·보상, 원정 중 임시 자원은 실패와 앱 재시작 뒤 제거한다.
- 설정·입력 바인딩, 튜토리얼 확인, 확정된 선택·해금 기술, 완료한 데모 분기 기록은 실패와 앱 재시작 뒤 보존한다.
- 활성 런은 저장하거나 이어 하지 않으며, 비정상 종료 뒤에는 새 원정 상태의 거점으로 시작한다.

## Requirements

- **REQ-RUN-001:** 원정은 `NotStarted → Active → Failed → Returned` 또는 `NotStarted → Active → Succeeded`의 단방향 종료 상태를 갖는다. `Succeeded`는 데모 세션의 terminal이며 명시적인 재시작만 새 `NotStarted`를 만든다.
- **REQ-RUN-002:** 실패 시 모든 원정 자산과 방별 일시 상태를 초기화한다.
- **REQ-RUN-003:** 확정된 서사 선택과 데모에서 정의한 지속 상태는 실패 후에도 보존한다.
- **REQ-RUN-004:** 실패 처리와 거점 복귀는 중복 호출되어도 보상·초기화·장면 전환을 한 번만 수행한다.
- **REQ-RUN-005:** 실패는 체력 0, 낙하 한계 통과, `Lethal` 압착 위험에서만 확정하며 시간 초과·메뉴·길 찾기 지연은 실패가 아니어야 한다.
- **REQ-RUN-006:** 선택은 확인 직후, 완료 분기 기록은 성공 `RunEndCommitted` 뒤에 각각 원자 저장하며 활성 런은 저장·재개하지 않아야 한다.

## Acceptance criteria

### AC-RUN-001 — 실패 초기화

- **Given** 원정 자산, 활성 무게 전이, 적·방 상태가 있는 Active 원정이 있고
- **When** 실패 조건이 발생하면
- **Then** 원정 자산과 일시 상태가 제거되고 거점으로 복귀하며 즉시 새 원정을 시작할 수 있다.

### AC-RUN-002 — 보존 상태

- **Given** 확정된 보존 상태가 있고
- **When** 원정 실패와 새 원정 시작을 거치면
- **Then** 보존 상태는 동일하고 이전 원정의 일시 상태는 존재하지 않는다.

### AC-RUN-003 — 단일 종료

- **Given** 동일 프레임에 둘 이상의 실패 원인이 발생하고
- **When** 종료 처리를 수행하면
- **Then** 하나의 Failed 결과와 하나의 Returned 전환만 기록된다.

### AC-RUN-004 — 성공 커밋과 재시작 경계

- **Given** 선택을 확정한 Active 원정과 완료 직전의 활성 전이가 있고
- **When** `DemoCompleted`가 발생하거나 앱을 비정상 종료한 뒤 다시 시작하면
- **Then** 완료 시에는 `TransferCleared → RunEndRequested(Succeeded) → RunEndCommitted → completedBranch` 원자 저장 순서가 한 번 기록되고, 비정상 종료 시에는 새 원정 상태의 거점으로 돌아오며 활성 런은 복구되지 않는다.

## Verification

상태 머신 단위 테스트, 중복 실패 PlayMode 테스트, 실패·성공·비정상 종료 전후 상태 스냅샷과 원자 저장 실패 테스트를 사용한다. 저장 JSON 필드와 플랫폼별 경로는 `OD-PLAT-001`에서 고정한다.

## Traceability

[CONTEXT 원정 실패/부분 초기화](../../../CONTEXT.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md), [VD-04](./04-authored-rooms-and-expedition.md), [VD-09](./09-platform-and-quality.md)
