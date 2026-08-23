# [VD-04] Authored Rooms and Expedition Assembly

- Status: Review
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Scope

저작 방 6개의 등록, 런당 4개 방 계획, 네 지원 시드, 소켓 계획과 방 전환을 포함한다.

## Non-scope

절차적 지형 생성, 보스·선택·봉쇄선 고정 장면의 내부 구성과 플레이어 이동 수치 소유는 포함하지 않는다.

## Contract

검증된 수작업 방 6개의 입출구, 필수 동선, 태그와 허용 변주를 정의하고 유효한 원정 순서로 조립한다. 방의 핵심 지형과 점프 거리는 생성하지 않는다.

### Inputs

자동 또는 검증 메뉴의 시드 요청, 저작 `RoomPlanSnapshot`, 방 완료·이탈 사건, 기본 이동 검증 결과.

### Outputs

유효한 4방 원정 계획, 현재 방·소켓 계획, `RoomPlanSnapshot` 증적 또는 계약 오류를 낸다.

### Owned state

방 풀, 지원 snapshot, 현재 방 계획과 socket plan을 소유한다. 플레이어 물리·적 체력·전이 상태는 소유하지 않는다.

### Invariants

- 같은 시드는 같은 방 순서와 변주를 재현한다.
- 모든 연결은 호환되는 출구와 입구를 사용한다.
- 필수 경로는 기본 이동 세트로 도달 가능하다.
- 원정 생성 실패는 불완전한 런을 시작하지 않고 진단 가능한 오류를 남긴다.
- 지원 계획은 런타임 RNG가 아닌 불변 저작 snapshot 네 개다: 101=`R01→R02→R04→R06`, 202=`R01→R02→R05→R06`, 303=`R01→R03→R04→R06`, 404=`R01→R03→R05→R06`.
- 각 snapshot은 정수 격자·안정 문자열 ID만 포함하며 RFC 8785 canonical JSON payload 뒤 단일 LF를 붙인 UTF-8 without BOM 파일 전체의 SHA-256 lowercase hex로 검증한다.
- 신규 프로필의 `lastOfferedSeed`는 null이며 다음 자동 시드는 101이다. 자동 원정은 다음 값을 원자 저장한 뒤에만 시작하고, 저장 실패 시 `RunStarted`를 발행하지 않는다. 검증 메뉴의 명시 시드는 이 값을 바꾸지 않는다.

## Requirements

- **REQ-ROOM-001:** 방 풀은 서로 식별되는 수작업 방 정확히 6개를 포함한다.
- **REQ-ROOM-002:** 각 방은 ID, 입출구, 필수 경로, 역할, 적/위험/보상 소켓, 허용 변주를 선언한다.
- **REQ-ROOM-003:** 조립기는 시드를 입력받아 유효한 원정 구성을 만든다.
- **REQ-ROOM-004:** 서로 다른 두 시드는 방 순서 또는 적·위험·보상 변주 중 적어도 하나가 달라야 한다.
- **REQ-ROOM-005:** 자동 구조 검증과 수동 이동 검증을 모두 통과하지 않은 방은 풀에 들어갈 수 없다.
- **REQ-ROOM-006:** 자동 시드 선택은 `selectionRuleVersion=1`의 `101 → 303 → 202 → 404` 순환을 사용하며, 신규 프로필의 첫 시드는 101이어야 한다.

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

### AC-ROOM-005 — 저작 snapshot과 순환

- **Given** 신규 프로필과 네 지원 snapshot이 있고
- **When** 자동 원정을 여덟 번 시작하고 검증 메뉴에서 명시 시드 하나를 시작하면
- **Then** 자동 순서는 `101, 303, 202, 404`를 두 번 반복하고, 각 JSON 해시가 증적과 일치하며 명시 시드는 자동 순서를 바꾸지 않는다.

## Verification

방 메타데이터 구조 테스트, canonical JSON·SHA-256 snapshot 테스트, 자동 순환 저장 실패 테스트, 방별 수동 완주 증적을 사용한다. 입출구 좌표와 이동 수치 허용오차는 `OD-MOV-001`에서 고정한다.

## Traceability

[CONTEXT 조립형 원정](../../../CONTEXT.md), [ADR-0008](../../adr/0008-assemble-authored-expedition-rooms.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md), [VD-01](./01-player-movement.md), [VD-05](./05-failure-and-persistence.md)
