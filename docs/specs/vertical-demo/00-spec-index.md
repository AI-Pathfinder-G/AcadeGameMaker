# [VD-00] Fifteen-minute Vertical Demo

- Status: Review
- Owner: Sol
- Unit design/implementation: Terra
- Verification: Luna
- Last updated: 2026-08-24

## Purpose

약 15분 안에 무게 전이의 조작 재미, 수작업 방 재조립의 반복성, 에셋 통합 방향, 인간성 선택의 플레이 차이를 한 세션으로 검증한다.

## Scope contract

### In scope

- 작은 거점 1개와 원정 1개
- 수작업 방 풀 6개 중 한 원정에 사용하는 방 4개
- 이동, 점프, 대시, 벽차기
- 상자와 적에 적용되는 무게 전이
- 일반 적 2종과 중간보스 1종
- 원정 실패와 부분 초기화
- 인간성 선택 1회와 수탈·연대 기술 각 1개
- 성공 경로 마지막의 히로인 추적자 첫 등장

### Out of scope

- 전체 챕터, 진보스, 완성형 지속 성장
- `각자의 아래` 실제 사용
- 후반 중력 방향 조작
- 출시 준비, 콘솔, 온라인 기능

## Requirements

- **REQ-SCOPE-001:** 깨끗한 시작에서 거점, 원정, 중간보스, 인간성 선택, 마지막 장면까지 하나의 연속된 성공 경로가 있어야 한다.
- **REQ-SCOPE-002:** 실패 경로는 원정을 종료하고 부분 초기화 후 거점에서 다시 시작할 수 있어야 한다.
- **REQ-SCOPE-003:** 콘텐츠 수량은 거점 1, 방 풀 6, 런당 방 4, 일반 적 유형 2, 중간보스 1, 선택 사건 1, 선택별 기술 1을 충족해야 한다.
- **REQ-SCOPE-004:** 기준 경로의 목표 플레이 시간은 로딩과 설정 시간을 제외하고 12~18분이어야 한다.

## Acceptance criteria

### AC-SCOPE-001 — 성공 경로 완주

- **Given** 신규 데모 상태와 검수용 기준 입력 프로필이 있고
- **When** 검수자가 거점에서 출정해 중간보스를 격파하고 인간성 선택을 완료하면
- **Then** 선택에 맞는 기술을 확인한 뒤 히로인의 첫 등장 장면에 도달한다.

### AC-SCOPE-002 — 실패 후 재시작

- **Given** 진행 중인 원정이 있고
- **When** 원정 실패 조건이 발생하면
- **Then** 원정 자산은 초기화 규칙을 따르고 보존 대상은 유지되며 거점에서 새 원정을 시작할 수 있다.

### AC-SCOPE-003 — 분량 측정

- **Given** 튜토리얼을 읽을 수 있는 첫 플레이 검수자 3명이 있고
- **When** 각자가 성공 경로를 완주하면
- **Then** 중앙값이 12~18분이며 필수 장면이나 기능을 건너뛸 필요가 없다.

### AC-SCOPE-004 — 콘텐츠 수량

- **Given** 검수 대상 콘텐츠 등록부와 빌드가 있고
- **When** 거점, 방, 적 유형, 보스, 선택 사건, 선택 기술을 고유 ID로 집계하면
- **Then** 거점 1개, 방 풀 6개, 런당 방 4개, 일반 적 유형 2개, 중간보스 1개, 선택 사건 1개, 선택 결과별 기술 1개씩이며 중복 ID가 없다.

## Unit specifications

1. [VD-01 Player movement](./01-player-movement.md)
2. [VD-02 Weight transfer](./02-weight-transfer.md)
3. [VD-03 Combat and enemies](./03-combat-and-enemies.md)
4. [VD-04 Authored rooms and expedition](./04-authored-rooms-and-expedition.md)
5. [VD-05 Failure and persistence](./05-failure-and-persistence.md)
6. [VD-06 Humanity choice and narrative](./06-humanity-choice-and-narrative.md)
7. [VD-07 Input, UI, and feedback](./07-input-ui-and-feedback.md)
8. [VD-08 Art and asset integration](./08-art-and-asset-integration.md)
9. [VD-09 Platform and quality](./09-platform-and-quality.md)
10. [VD-10 Verification script](./10-verification-script.md)

교차 시스템의 상태·사건·생명주기 소유권은 [System Contracts](./SYSTEM-CONTRACTS.md)가 정의한다.

## Approval blocker

[ADR-0018](../../adr/0018-vertical-demo-p0-integration.md)로 P0 여섯 항목은 모두 해결됐다. P1 계약, 소비 스펙 간 일치, Luna 독립 검토가 끝나고 Sol이 승인 기록을 남기기 전에는 이 패키지나 하위 스펙을 Approved로 전환하지 않는다.

## Traceability

[게임 디자인 캐논](../../canon/game-design.md), [ADR-0007](../../adr/0007-weight-transfer-is-the-core-player-verb.md), [ADR-0008](../../adr/0008-assemble-authored-expedition-rooms.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md)
