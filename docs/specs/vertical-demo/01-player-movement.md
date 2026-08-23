# [VD-01] Player Movement

- Status: Review
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Scope

좌우 이동, 점프, 대시, 벽차기, 접지·공중 상태와 무게 전이 이동 modifier의 적용·복구를 포함한다.

## Non-scope

전이 대상 선택, 피해 판정, 방 조립, 후반 중력 방향 변경과 최종 애니메이션 튜닝은 포함하지 않는다.

## Contract

입력 의미를 받아 주인공의 위치·속도·접지 상태를 갱신하고 충돌 가능한 방 지형에 결과를 제공한다. 무게 전이는 이동 파라미터를 바꿀 수 있지만 이동 기능의 상태를 직접 소유하지 않는다.

### Inputs

이동·점프·대시 의미 명령, 현재 `InputMode`, 충돌 질의 결과, 전이 player modifier, 피격·방 전환 생명주기 사건.

### Outputs

주인공 위치·속도·접지·행동 상태와 이동 완료·거부 진단을 제공한다.

### Owned state

현재 속도, 접지·벽 접촉, 점프·대시·벽차기 상태, 입력 버퍼와 코요테 타이머를 소유한다. 전이 세션, 체력, 런 결과는 소유하지 않는다.

### Invariants

- 이동, 점프, 대시, 벽차기는 동일한 입력 조건에서 재현 가능하다.
- 방 경계 전환 후 입력 잠김이나 잔류 속도가 플레이어를 제어 불능 상태로 만들지 않는다.
- 피격, 사망, 컷신이 입력을 잠그는 경우 해제 조건이 명시되어야 한다.
- 활성 전이 중 이동 파라미터는 기준값에 `gravityScale ×0.65`, 공중 가속 ×1.25, 최대 낙하 속도 ×0.70을 곱하고 회수 시 같은 기준값으로 복구한다.

## Requirements

- **REQ-MOV-001:** 기준 중력에서 좌우 이동과 점프를 제공한다.
- **REQ-MOV-002:** 플레이어가 방향을 예측할 수 있는 대시를 제공한다.
- **REQ-MOV-003:** 유효한 벽 접촉에서 벽차기를 제공한다.
- **REQ-MOV-004:** 무게 전이로 가벼워진 상태는 기준값 대비 `gravityScale ×0.65`, 공중 가속 ×1.25, 최대 낙하 속도 ×0.70을 적용한다.
- **REQ-MOV-005:** 모든 수작업 방의 필수 동선은 선택 기술 없이 기본 이동 세트로 통과 가능하다.

## Acceptance criteria

### AC-MOV-001 — 기본 이동 세트

- **Given** 평지와 벽이 있는 검수 방이 있고
- **When** 이동, 점프, 대시, 벽차기 입력을 각각 수행하면
- **Then** 각 행동이 한 번만 발동하고 애니메이션·이동 결과·재사용 가능 상태가 일치한다.

### AC-MOV-002 — 무게 전이와 공중 기동

- **Given** 같은 시작점과 입력 기록이 있고
- **When** 기본 상태와 가벼워진 상태에서 각각 점프 경로를 재생하면
- **Then** 세 파라미터 배율이 정확히 적용·복구되고 가벼워진 경로 차이가 UI 피드백과 일치한다.

### AC-MOV-003 — 방 완주성

- **Given** 방 풀 6개의 필수 경로가 있고
- **When** 검수자가 선택 기술 없이 기본 이동 세트로 각 경로를 수행하면
- **Then** 모든 입구에서 대응 출구까지 막힘 없이 도달한다.

## Verification

PlayMode 입력 재생, 이동 상태 로그, 방별 수동 완주 영상으로 검증한다. 속도·점프 높이·대시 거리·버퍼·코요테 타임은 `OD-MOV-001` 해결 후 테스트 허용오차와 함께 고정한다.

## Traceability

[ADR-0007](../../adr/0007-weight-transfer-is-the-core-player-verb.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md), [VD-02](./02-weight-transfer.md), [VD-04](./04-authored-rooms-and-expedition.md)
