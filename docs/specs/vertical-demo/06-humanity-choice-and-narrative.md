# [VD-06] Humanity Choice, Skills, and Final Scene

- Status: Draft
- Owner: Terra
- Canon and contract approval: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Contract

주체성을 가진 인물과 관련된 인간성 선택 1회, 상호 배타적인 수탈·연대 결과, 결과별 기술 1개, 선택 기록, 성공 경로 마지막의 히로인 첫 등장 장면을 정의한다.

### Invariants

- 인간성을 숫자 게이지나 선악 점수로 표현하지 않는다.
- 어느 선택도 데모 진행을 막거나 기술 보상을 제거하지 않는다.
- 히로인은 선택의 보상물이나 무조건적 동료로 표현하지 않는다.
- 수직 데모는 `각자의 아래`를 해금하거나 사용하지 않는다.

## Requirements

- **REQ-CHOICE-001:** 플레이어는 타인의 각인을 강제로 힘으로 바꾸는 수탈과 주체성을 보존하는 연대 중 하나를 명시적으로 선택한다.
- **REQ-CHOICE-002:** 수탈 결과는 직접적인 파괴력·효율 중심 기술 1개를 제공한다.
- **REQ-CHOICE-003:** 연대 결과는 이동·제어·환경 조합 중심 기술 1개를 제공한다.
- **REQ-CHOICE-004:** 선택은 한 번만 확정되고 원정 생명주기에서 정의한 보존 규칙을 따른다.
- **REQ-CHOICE-005:** 성공 경로 마지막에는 히로인이 주인공의 추적자로 처음 등장한다.

## Acceptance criteria

### AC-CHOICE-001 — 두 유효 경로

- **Given** 선택 직전의 동일한 기준 상태가 있고
- **When** 수탈과 연대를 각각 별도 실행에서 선택하면
- **Then** 대응 기술 하나가 해금되고 다른 기술은 해금되지 않으며 두 경로 모두 마지막 장면까지 진행된다.

### AC-CHOICE-002 — 선택의 의미

- **Given** 선택 장면이 있고
- **When** 플레이어가 선택지와 결과 연출을 확인하면
- **Then** 대상 인물의 소유권·선택권을 보존하거나 침해한다는 차이가 대사·행동·기술 효과에서 일치한다.

### AC-CHOICE-003 — 히로인 첫 등장

- **Given** 중간보스와 선택을 완료한 성공 경로가 있고
- **When** 데모 종결 조건이 발생하면
- **Then** 히로인은 별도 추적자로 처음 등장하고 중간보스나 선택 보상으로 오인되지 않는다.

## Verification

두 분기 상태 테스트, 기술 해금 테스트, Sol의 캐논 리뷰, Luna의 두 경로 수동 완주를 사용한다. 선택 대상·시점·기술 효과·장면 대사는 `OD-CHOICE-001`과 `OD-CHOICE-002`의 P0 결정이다.

## Traceability

[서사 캐논](../../canon/narrative-canon.md), [ADR-0003](../../adr/0003-humanity-is-preserving-others-agency.md), [ADR-0004](../../adr/0004-heroine-is-a-conditional-human-anchor.md), [ADR-0005](../../adr/0005-choices-grant-different-skill-families.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md)
