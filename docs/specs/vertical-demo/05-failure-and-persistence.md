# [VD-05] Failure and Persistence

- Status: Draft
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Contract

원정 시작, 진행, 성공, 실패, 거점 복귀의 상태 전이와 원정 자산·지속 성장·서사 상태의 소유 경계를 정의한다. 저장 포맷은 Sol의 별도 플랫폼 계약을 따른다.

## Requirements

- **REQ-RUN-001:** 원정은 `NotStarted → Active → Succeeded | Failed → Returned`의 단방향 종료 상태를 갖는다.
- **REQ-RUN-002:** 실패 시 모든 원정 자산과 방별 일시 상태를 초기화한다.
- **REQ-RUN-003:** 확정된 서사 선택과 데모에서 정의한 지속 상태는 실패 후에도 보존한다.
- **REQ-RUN-004:** 실패 처리와 거점 복귀는 중복 호출되어도 보상·초기화·장면 전환을 한 번만 수행한다.

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

## Verification

상태 머신 단위 테스트, 중복 실패 PlayMode 테스트, 실패 전후 상태 스냅샷을 사용한다. 정확한 상태 보존표는 `OD-RUN-001` 해결 전까지 확정하지 않는다.

## Traceability

[CONTEXT 원정 실패/부분 초기화](../../../CONTEXT.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [VD-04](./04-authored-rooms-and-expedition.md), [VD-09](./09-platform-and-quality.md)
