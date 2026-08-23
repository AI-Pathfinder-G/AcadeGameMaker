# [VD-03] Combat and Enemies

- Status: Draft
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Contract

기본 공격, 피격, 사망, 일반 적 2종, 중간보스 1종과 각자의 무게 전이 반응을 정의한다. 인간성 선택 기술은 이 계약을 소비하지만 전투 생명주기나 적 상태를 소유하지 않는다.

## Requirements

- **REQ-COM-001:** 기본 공격만으로도 모든 전투를 끝낼 수 있으나 무게 전이는 분명한 전술 이점을 제공한다.
- **REQ-COM-002:** 일반 적 2종은 이동, 위협, 전이 반응 중 적어도 두 항목에서 구별된다.
- **REQ-COM-003:** 중간보스는 무게 전이 사용을 학습·활용하게 하는 읽을 수 있는 패턴과 안전한 대응 창을 가진다.
- **REQ-COM-004:** 피격·무적·사망·방 초기화 상태는 중복 보상이나 진행 막힘 없이 한 번씩 전이한다.

## Acceptance criteria

### AC-COM-001 — 적 유형 구분

- **Given** 같은 검수 방에 일반 적 두 유형이 각각 배치되고
- **When** 기본 공격과 무게 전이로 상대하면
- **Then** 두 유형의 위협과 권장 대응이 관찰 가능하게 다르며 모두 격파 가능하다.

### AC-COM-002 — 중간보스 완료

- **Given** 기본 이동·공격·무게 전이를 보유한 플레이어가 보스 방에 진입하고
- **When** 패턴의 신호와 대응 창을 이용하면
- **Then** 선택 기술 없이도 격파할 수 있고 무게 전이를 사용하면 의도된 이점이 발생한다.

### AC-COM-003 — 생명주기 안정성

- **Given** 적 또는 보스가 피격과 사망 임계에 도달하고
- **When** 같은 프레임에 복수 충돌이 발생해도
- **Then** 사망·보상·방 완료는 각각 한 번만 처리된다.

## Verification

적 상태 단위 테스트, 복수 충돌 PlayMode 테스트, 무게 전이 사용/미사용 보스 완주 영상을 남긴다. 적 콘셉트와 수치, 중간보스 패턴은 P0 결정이다.

## Traceability

[ADR-0007](../../adr/0007-weight-transfer-is-the-core-player-verb.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [VD-02](./02-weight-transfer.md), [VD-06](./06-humanity-choice-and-narrative.md)
