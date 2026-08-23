# [VD-04] Authored Rooms and Expedition Assembly

- Status: Draft
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Contract

검증된 수작업 방 6개의 입출구, 필수 동선, 태그와 허용 변주를 정의하고 유효한 원정 순서로 조립한다. 방의 핵심 지형과 점프 거리는 생성하지 않는다.

### Invariants

- 같은 시드는 같은 방 순서와 변주를 재현한다.
- 모든 연결은 호환되는 출구와 입구를 사용한다.
- 필수 경로는 기본 이동 세트로 도달 가능하다.
- 원정 생성 실패는 불완전한 런을 시작하지 않고 진단 가능한 오류를 남긴다.

## Requirements

- **REQ-ROOM-001:** 방 풀은 서로 식별되는 수작업 방 정확히 6개를 포함한다.
- **REQ-ROOM-002:** 각 방은 ID, 입출구, 필수 경로, 역할, 적/위험/보상 소켓, 허용 변주를 선언한다.
- **REQ-ROOM-003:** 조립기는 시드를 입력받아 유효한 원정 구성을 만든다.
- **REQ-ROOM-004:** 서로 다른 두 시드는 방 순서 또는 적·위험·보상 변주 중 적어도 하나가 달라야 한다.
- **REQ-ROOM-005:** 자동 구조 검증과 수동 이동 검증을 모두 통과하지 않은 방은 풀에 들어갈 수 없다.

## Acceptance criteria

### AC-ROOM-001 — 구조 유효성

- **Given** 방 풀 6개와 지원되는 시드 목록이 있고
- **When** 각 시드로 원정을 조립하면
- **Then** 모든 연결 포트가 호환되고 시작에서 종점까지 경로가 존재한다.

### AC-ROOM-002 — 재현성과 변주

- **Given** 시드 A와 B가 있고
- **When** A를 두 번, B를 한 번 조립하면
- **Then** 두 A 결과는 같고 B 결과는 A와 적어도 한 변주 축에서 다르다.

### AC-ROOM-003 — 플레이 도달성

- **Given** 생성된 모든 지원 구성과 선택 기술이 없는 플레이어가 있고
- **When** 방별 필수 경로를 수행하면
- **Then** 미검증 점프, 막힌 출구, 진행 불능 배치 없이 종점에 도달한다.

### AC-ROOM-004 — 방 풀 입장 게이트

- **Given** 신규 또는 수정된 방 후보가 있고
- **When** 자동 구조 검사나 기본 이동 수동 완주 중 하나라도 통과하지 못하면
- **Then** 해당 방 ID는 승인된 6개 방 풀과 지원 시드 결과에 포함되지 않는다.

## Verification

방 메타데이터 구조 테스트, 시드 스냅샷 테스트, 방별 수동 완주 증적을 사용한다. 한 런에서 사용하는 방 수와 지원 시드 표본 수는 `OD-ROOM-001`에서 확정한다.

## Traceability

[CONTEXT 조립형 원정](../../../CONTEXT.md), [ADR-0008](../../adr/0008-assemble-authored-expedition-rooms.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [VD-01](./01-player-movement.md), [VD-05](./05-failure-and-persistence.md)
