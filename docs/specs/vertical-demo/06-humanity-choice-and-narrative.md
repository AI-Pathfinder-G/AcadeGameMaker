# [VD-06] Humanity Choice, Skills, and Final Scene

- Status: Review
- Owner: Terra
- Canon and contract approval: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Scope

유담의 동의 상태, 수탈·연대 선택, 선택별 기술, 봉쇄선 검증과 성공 경로의 히로인 첫 등장을 포함한다.

## Non-scope

히로인의 전체 관계 아크, 최종장 분기, `각자의 아래` 실제 사용과 선택 장면 밖의 전투 판정은 포함하지 않는다.

## Contract

주체성을 가진 인물과 관련된 인간성 선택 1회, 상호 배타적인 수탈·연대 결과, 결과별 기술 1개, 선택 기록, 성공 경로 마지막의 히로인 첫 등장 장면을 정의한다. 선택 대상은 공동장부 보관자 유담이다.

### Inputs

유담의 `ConsentRefused`/`ConsentOffered`, 선택 확정 또는 취소, 활성 전이, `ChoiceSkill`, 봉쇄선 앵커 상태와 장면 생명주기 입력 모드.

### Outputs

`ChoiceCommitted`, 선택별 해금 기술, `ResonanceLease` 또는 `ImprintSevered`, 봉쇄선 해제 요청과 선택·기술 저장 요청을 낸다.

### Owned state

선택 확정 여부, 유담의 동의 상태, 해금 기술, 기술 쿨다운과 봉쇄선 선택 결과를 소유한다. 전이·피해·런 phase의 권위 상태는 소유하지 않는다.

### Invariants

- 인간성을 숫자 게이지나 선악 점수로 표현하지 않는다.
- 어느 선택도 데모 진행을 막거나 기술 보상을 제거하지 않는다.
- 히로인은 선택의 보상물이나 무조건적 동료로 표현하지 않는다.
- 수직 데모는 `각자의 아래`를 해금하거나 사용하지 않는다.
- 유담과 `ConsentAnchor-Barrier`는 `TransferTarget`이 아니다. 실제 전이는 봉쇄선의 안정 ID `BarrierCounterweight` 상자에만 성립한다.
- `ChoiceSkill`은 `GameplayEnabled`에서만 동작하며 UI·컷신·방 전환·실패·종료에서는 잠긴다. 쿨다운은 60Hz 고정 틱으로 계산한다.
- 유담의 동의 상태는 `RefusesOwnershipTransfer → OffersScopedResonance → ResonanceLender` 또는 `RefusesOwnershipTransfer → ImprintSevered`만 허용하며 확정 전 취소는 거부 상태로 돌아간다.
- `압착 판결`은 성공 시 전이를 회수하고, `공동 기준면`은 전이를 유지한다. 공동 기준면의 활성 대상에는 대상 소유 시스템이 60틱 `ResonanceHold`를 적용하며 제거·사망·장면 전환 시 속도 복원 없이 상태만 정리한다.

## Requirements

- **REQ-CHOICE-001:** 플레이어는 타인의 각인을 강제로 힘으로 바꾸는 수탈과 주체성을 보존하는 연대 중 하나를 명시적으로 선택한다.
- **REQ-CHOICE-002:** 수탈 결과는 직접적인 파괴력·효율 중심 기술 1개를 제공한다.
- **REQ-CHOICE-003:** 연대 결과는 이동·제어·환경 조합 중심 기술 1개를 제공한다.
- **REQ-CHOICE-004:** 선택은 한 번만 확정되고 원정 생명주기에서 정의한 보존 규칙을 따른다.
- **REQ-CHOICE-005:** 성공 경로 마지막에는 히로인이 주인공의 추적자로 처음 등장한다.
- **REQ-CHOICE-006:** 수탈 `압착 판결`은 활성 전이 대상에 36틱 뒤 중심 4 피해·주변 2 피해와 60틱 경직을 적용하고 전이를 회수하며 300틱 재사용을 가져야 한다.
- **REQ-CHOICE-007:** 연대 `공동 기준면`은 활성 전이 대상과 플레이어의 발동 틱 X 사이에 150틱 지속하는 길이 1.5~6.0·두께 0.25의 `PlayerOnlyPlatform` 발판을 만들고 420틱 재사용을 가져야 한다.

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

### AC-CHOICE-004 — 봉쇄선의 즉시 사용

- **Given** `BarrierCounterweight`가 6.0 유닛 안이고 LOS가 보장된 비치명 봉쇄선과 양쪽 선택의 기준 상태가 있고
- **When** 각 경로에서 전이를 성립시킨 뒤 ChoiceSkill을 한 번 사용하면
- **Then** 수탈은 `BarrierGate-Fractured`를 파괴하고 전이를 회수하며, 연대는 기존 발판과 `BarrierGate-ConsentLock` 해제로 통과하고, 두 통과 시간 차이는 10초 이내다.

## Verification

두 분기 상태·기술 해금·발판 수명·쿨다운 테스트, 봉쇄선 PlayMode 테스트, Sol의 캐논 리뷰, Luna의 두 경로 수동 완주를 사용한다. 기기별 바인딩은 VD-07·VD-09의 해결된 입력 계약을 따르며 UI 자산은 `OD-ART-001`에서 고정한다.

## Traceability

[서사 캐논](../../canon/narrative-canon.md), [ADR-0003](../../adr/0003-humanity-is-preserving-others-agency.md), [ADR-0004](../../adr/0004-heroine-is-a-conditional-human-anchor.md), [ADR-0005](../../adr/0005-choices-grant-different-skill-families.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md)
