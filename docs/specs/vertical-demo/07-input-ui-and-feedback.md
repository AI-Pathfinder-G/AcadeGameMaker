# [VD-07] Input, UI, and Feedback

- Status: Review
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Scope

의미 입력, 입력 모드 gate, 키보드·XInput 지원 범위와 전이·런·선택의 최소 판독 피드백을 포함한다.

## Non-scope

실제 action map·키 배치, 최종 UI 아트, 접근성 전체 기능과 게임플레이 권위 상태의 직접 변경은 포함하지 않는다.

## Contract

이동·전투·무게 전이·상호작용의 의미 기반 입력과 플레이어가 현재 상태·실패 이유·선택 결과를 이해하는 최소 피드백을 정의한다. 키보드와 XInput 게임패드를 모두 지원하고 마우스는 UI·조준에 사용할 수 있으며, 구체 action map과 실제 바인딩은 플랫폼 계약에 둔다.

### Inputs

의미 액션 `Aim`, `Transfer`, `ChoiceSkill`, 이동·공격·상호작용, 전이·런·선택·입력 모드 사건, 실패 이유와 피드백 제한 틱.

### Outputs

후보 외곽선·연결선, 활성·회수·실패·쿨다운·선택 결과 피드백과 `InputModeChanged` 사건을 낸다.

### Owned state

권위 입력 모드 gate, 피드백 표시와 중복 억제를 소유한다. 전이·체력·선택·런의 게임플레이 권위 상태, 기기 바인딩, 저장 필드는 소유하지 않는다.

### Invariants

- `GameplayEnabled`, `UIOnly`, `Cutscene`, `Transition`, `Ended`의 모드에 따라 입력 표현과 허용 상태를 일관되게 갱신한다.
- 같은 틱의 유효 `ChoiceSkill`과 `Transfer`는 ChoiceSkill을 먼저 처리한다. 수탈 성공은 Transfer를 소비하고, 연대 성공은 Transfer를 무시하며, 실패한 ChoiceSkill은 Transfer를 막지 않는다.

## Requirements

- **REQ-UX-001:** 모든 필수 행동은 하나의 명명된 입력 액션에 연결되고 중복 충돌이 없어야 한다.
- **REQ-UX-002:** 전이 후보, 성공, 실패, 활성 상태, 회수를 서로 구분할 수 있어야 한다.
- **REQ-UX-003:** 체력/실패 위험, 원정 결과, 선택 결과, 새 기술을 플레이 진행을 멈추지 않고 확인할 수 있어야 한다.
- **REQ-UX-004:** 컷신·메뉴·장면 전환의 입력 잠금과 복구가 일관되어야 한다.
- **REQ-UX-005:** 전이 후보·활성·회수·차단·쿨다운과 `NoActiveTransfer`, `Cooldown`, `InputLocked`, `InvalidTarget` 실패 이유를 서로 구분해 표시해야 한다.

## Acceptance criteria

### AC-UX-001 — 전이 상태 판독

- **Given** 전이 불가·가능·활성 상태가 각각 있고
- **When** 같은 대상에 접근하고 조작하면
- **Then** 검수자는 화면 피드백만으로 세 상태와 실패 이유를 구분한다.

### AC-UX-002 — 입력 복구

- **Given** 컷신, 메뉴, 방 전환 중 하나가 입력을 잠그고
- **When** 해당 상태가 정상 또는 취소로 끝나면
- **Then** 이전 입력이 고착되지 않고 플레이 입력이 한 번만 복구된다.

### AC-UX-003 — 필수 상태와 결과 판독

- **Given** 정상 플레이, 실패 직전, 원정 종료, 선택 완료, 기술 획득 상태가 각각 있고
- **When** 검수자가 화면과 오디오 피드백을 확인하면
- **Then** 체력/실패 위험, 원정 결과, 선택 결과, 새 기술을 서로 혼동하지 않고 식별한다.

### AC-UX-004 — ChoiceSkill과 전이 입력 순서

- **Given** 활성 전이와 양쪽 ChoiceSkill, 그리고 UIOnly·Cutscene·Ended 상태가 있고
- **When** 같은 틱의 ChoiceSkill·Transfer 또는 잠긴 상태의 ChoiceSkill을 입력하면
- **Then** 성공 수탈은 전이를 한 번 회수하고 성공 연대는 유지하며, 실패한 ChoiceSkill은 Transfer를 허용하고 잠긴 상태는 `InputLocked` 피드백만 보인다.

## Verification

의미 입력 맵 정적 검사, 키보드·XInput 동등 동작 재생, 상태별 스크린 캡처, 고정 틱 입력 순서·잠금/복구 PlayMode 테스트, 짧은 이해도 관찰로 검증한다. 실제 바인딩은 `OD-PLAT-001`에서 고정한다.

## Traceability

[ADR-0007](../../adr/0007-weight-transfer-is-the-core-player-verb.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md), [VD-01](./01-player-movement.md), [VD-02](./02-weight-transfer.md), [VD-06](./06-humanity-choice-and-narrative.md)
