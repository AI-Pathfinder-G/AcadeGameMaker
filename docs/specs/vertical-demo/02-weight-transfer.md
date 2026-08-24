# [VD-02] Weight Transfer

- Status: Review
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Scope

단일 대상의 후보 판정, 전이·회수, 가역 modifier와 생명주기 정리를 포함한다.

## Non-scope

대상 체력·AI·기본 물리값 소유, 복수 동시 전이, 중력 방향 변경과 유담의 공동 서명 앵커는 포함하지 않는다.

## Contract

플레이어가 자신의 무게를 허용된 대상에 넘기고 되찾는 핵심 행위를 소유한다. 수직 데모의 허용 대상은 상자, 일반 적, 보스 패턴 중 노출된 scripted `압류추` handle이며, 이동·전투·환경은 전이 상태를 읽어 각자의 결과를 계산한다. 유담과 공동 서명 앵커는 전이 대상이 아니다.

### Inputs

`Transfer` 시도, 마우스 포인터 또는 게임패드 오른쪽 스틱의 조준 의도, 후보 대상의 저작 `TransferTargetId`와 기본 보정값, 대상 유효성, 방/원정/컷신 생명주기 사건.

### Outputs

주인공의 기본/가벼움 상태, 대상의 기본/무거움 modifier, 성공·실패 이유, 후보·활성·정리 피드백과 `TransferStateChanged`/`TransferCleared` 사건.

### Owned state

활성 대상 ID 하나, 전이 쿨다운 종료 틱, 피드백 제한 종료 틱, 주인공과 대상에 합성한 전이 modifier, 후보 판정 결과를 소유한다. 대상의 체력, 방 순서, 기본 mass/gravityScale/AI 상태는 소유하지 않는다.

### Invariants

- 전이가 성공한 경우 주인공과 대상의 상태 변화는 같은 갱신에서 확정된다.
- 유효하지 않은 대상에는 부분 상태가 남지 않는다.
- 회수, 대상 제거, 방 종료, 원정 실패 후에는 고아 전이 상태가 남지 않는다.
- 후반 기능인 중력 방향 변경은 수직 데모에서 제외한다.
- 입력은 프레임에서 수집하고 다음 FixedUpdate에서 한 번만 원자 적용한다. 성공한 전이·회수 뒤 21 고정 틱 동안 상태를 바꾸지 않는다.
- 후보는 플레이어에서 6.0 월드 유닛 안이고 대상 중심점 기준 `TransferLineOfSight` 지형 레이어에 가려지지 않으며 trigger가 아닌 대상이다.
- 마우스·오른쪽 스틱 조준이 같은 의미 후보를 만들고 바라보는 방향 fallback과 50도 원뿔은 사용하지 않는다. 정확한 보정 반경·후보 정렬·스틱 조준 유지 계약은 `OD-PLAT-001`에서 고정한다.
- 기본 물리값은 대상 소유 시스템이 제공하고, 이 시스템은 가역 modifier만 합성·제거한다. 제거된 대상에는 복원을 시도하지 않고 참조와 효과만 정리한다.

## Requirements

- **REQ-WT-001:** 플레이어는 유효한 상자, 일반 적, 보스 패턴 중 노출된 `BossPayload`에 무게를 전이하고 회수할 수 있다.
- **REQ-WT-002:** 전이 중 주인공은 기본 상태와 구분되는 가벼운 공중 기동을 얻는다.
- **REQ-WT-003:** 무거워진 상자와 적은 기본 상태와 구분되는 낙하·충돌 또는 환경 반응을 보인다.
- **REQ-WT-004:** 전이 가능 여부와 현재 보유 상태를 조작 전에 또는 즉시 이해할 수 있는 피드백을 제공한다.
- **REQ-WT-005:** 생명주기 경계에서 전이 상태를 결정적으로 정리한다.
- **REQ-WT-006:** 전이 대상 선택, 적용, 회수와 실패는 고정 틱·저작 ID·명시된 시야 규칙에 따라 재현 가능해야 한다.
- **REQ-WT-007:** 전이는 주인공 `gravityScale ×0.65`, 공중 가속 ×1.25, 최대 낙하 속도 ×0.70과 상자 `mass ×3.0`·`gravityScale ×2.0`·충돌 피해 배율 ×1.5, 일반 적 `gravityScale ×2.2`·이동 속도 ×0.75·공중 제어 ×0.35·넉백 저항 ×2.0의 modifier를 적용한다.

## Acceptance criteria

### AC-WT-001 — 상자 전이와 회수

- **Given** 유효 범위의 상자와 기본 상태의 주인공이 있고
- **When** 전이하고 21틱 전환 잠금이 끝난 뒤 같은 `Transfer` 상호작용으로 회수하면
- **Then** 주인공과 상자가 각각 가벼움/무거움 상태를 거쳐 기본 상태로 돌아오며 상자의 물리 반응이 달라진다.

### AC-WT-002 — 적 전이와 전투 결과

- **Given** 전이 가능한 일반 적이 있고
- **When** 적에게 무게를 전이하면
- **Then** 적의 낙하·충돌 또는 행동 제한 중 적 유형에 정의된 반응이 발생하고 전투가 진행 불능이 되지 않는다.

### AC-WT-003 — 실패 원자성

- **Given** 범위 밖, 차단됨, 제거 중인 대상 중 하나가 있고
- **When** 전이를 시도하면
- **Then** 주인공과 대상의 상태는 바뀌지 않고 실패 이유에 맞는 피드백만 발생한다.

### AC-WT-004 — 생명주기 정리

- **Given** 활성 전이가 있고
- **When** 대상 제거, 방 종료, 또는 원정 실패가 발생하면
- **Then** 다음 방이나 원정에 활성 참조·보정값·시각 효과가 남지 않는다.

### AC-WT-005 — 결정적 후보와 물리 modifier

- **Given** 포인터·스틱 조준의 중첩 후보, 차단된 후보, scripted `압류추` handle을 포함한 고정 틱 검수 장면이 있고
- **When** 같은 조준·`Transfer` 입력 기록을 재생하면
- **Then** 같은 `TransferTargetId`가 선택되고 21틱 쿨다운·가역 modifier·`TransferStateChanged`가 일치하며, 상자의 피해 계산은 mass가 아니라 `ImpactDamageMultiplier`를 사용한다.

## Verification

상태 전이 단위 테스트, 후보 정렬·LOS·고정 틱 테스트, 상자·적·scripted handle PlayMode 테스트, 방 전환/실패 회귀 테스트로 검증한다. 이동의 최종 속도·점프·대시 허용오차는 [VD-01](./01-player-movement.md)을 따른다.

## Traceability

[CONTEXT 무게 전이](../../../CONTEXT.md), [ADR-0007](../../adr/0007-weight-transfer-is-the-core-player-verb.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md), [ADR-0019](../../adr/0019-pointer-aimed-sidescroller-controls.md), [VD-01](./01-player-movement.md), [VD-03](./03-combat-and-enemies.md)
