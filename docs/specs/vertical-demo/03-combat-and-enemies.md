# [VD-03] Combat and Enemies

- Status: Review
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Scope

기본 공격·피해 판정, 일반 적 두 유형, 환수관 오르단과 전투 생명주기를 포함한다.

## Non-scope

진보스, 장기 성장 밸런스, 선택 장면의 소유와 무게 전이 세션 자체는 포함하지 않는다.

## Contract

기본 공격, 피격, 사망, 일반 적 2종, 중간보스 1종과 각자의 무게 전이 반응을 정의한다. 인간성 선택 기술은 이 계약을 소비하지만 전투 생명주기나 적 상태를 소유하지 않는다.

### Inputs

기본 공격, 전이 modifier, `DamageRequest`, 보스 패턴 틱, scripted `압류추` 전이 성공, ChoiceSkill의 피해·경직 요청, 방/런 종료 사건.

### Outputs

체력·피격·무적·사망·취약 상태, `DamageResult`, 보상·방 완료 요청과 적별 전이 반응을 낸다.

### Owned state

체력, 무적, 적 AI 전투 상태, 보스 패턴 상태, 피해 요청 중복 제거를 소유한다. 활성 전이와 선택·런 상태는 소유하지 않는다.

### Invariants

- 피해는 안정 `DamageRequestId`를 한 번만 수락하며, 같은 틱의 복수 요청은 ID 오름차순으로 처리한다. 사망 확정 뒤 나머지 요청은 무시한다.
- 플레이어 체력 5, 기본 공격 피해 3, 최소 공격 간격 21틱, 피격 무적 45틱을 검수 기준으로 한다.
- 보스 본체는 전이 대상이 아니며 `압류추`는 Dynamic joint가 아닌 보스 소유 kinematic scripted payload다.
- 오르단 패턴은 런타임 RNG 없이 `체납선(48/21/39틱) → 압류추(60/최대72/48틱) → 체납선 → 균형 감사(60/90/48틱)`를 반복한다. 2·3단계는 Execute/Recovery의 유효 틱을 각각 `ceil(baseTicks / 1.10)`, `ceil(baseTicks / 1.20)`으로 줄이고 Telegraph는 최소 48틱을 유지한다.
- `압류추`는 Telegraph 동안 handle 하나만 켜고 전이 성공 시 가속 낙하·보스 피해·120틱 취약을 패턴당 한 번만 낸다. 균형 감사의 상자 전이는 Execute를 중단하고 90틱 취약을 낸다.

## Requirements

- **REQ-COM-001:** 기본 공격만으로도 모든 전투를 끝낼 수 있으나 무게 전이는 분명한 전술 이점을 제공한다.
- **REQ-COM-002:** 일반 적 2종은 이동, 위협, 전이 반응 중 적어도 두 항목에서 구별된다.
- **REQ-COM-003:** 중간보스는 무게 전이 사용을 학습·활용하게 하는 읽을 수 있는 패턴과 안전한 대응 창을 가진다.
- **REQ-COM-004:** 피격·무적·사망·방 초기화 상태는 중복 보상이나 진행 막힘 없이 한 번씩 전이한다.
- **REQ-COM-005:** 징수보행관은 체력 9·피해 1·방패 열림 90틱, 부유측량사는 체력 6·피해 1·전이 과부하 사격 잠금 75틱을 가져야 한다.
- **REQ-COM-006:** 오르단은 체력 60, 피해 1, `체납선 → 압류추 → 체납선 → 균형 감사`의 결정적 패턴 순서와 명시된 Telegraph/Execute/Recovery 틱을 가져야 한다.

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

### AC-COM-004 — 보스 전이 이점과 결정성

- **Given** 시드 101의 신규 프로필과 동일 검수자의 전이 미사용/사용 보스 3쌍이 있고
- **When** 각 패턴을 재생해 격파하면
- **Then** `미사용→사용`, `사용→미사용`, `미사용→사용` 순서의 여섯 성공 표본은 각각 180초 이내이며 사용 평균 완료 시간은 미사용 평균의 85% 이하이고, `압류추` 전이는 패턴당 한 번의 120틱 취약만 만든다. 사망·진행 중단은 조건별 최대 2회 재시도하고 성공 표본을 채우지 못하면 실패다.

## Verification

적 상태·DamageRequest 중복 제거 단위 테스트, 복수 충돌 PlayMode 테스트, scripted payload와 패턴 틱 스냅샷, 무게 전이 사용/미사용 보스 완주 영상을 남긴다. 선택 기술의 구체 바인딩은 VD-07의 해결된 입력 계약을 따른다.

## Traceability

[ADR-0007](../../adr/0007-weight-transfer-is-the-core-player-verb.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md), [VD-02](./02-weight-transfer.md), [VD-06](./06-humanity-choice-and-narrative.md)
