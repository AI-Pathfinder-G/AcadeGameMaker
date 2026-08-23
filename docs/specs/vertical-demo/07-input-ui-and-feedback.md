# [VD-07] Input, UI, and Feedback

- Status: Draft
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Contract

이동·전투·무게 전이·상호작용의 의미 기반 입력과 플레이어가 현재 상태·실패 이유·선택 결과를 이해하는 최소 피드백을 정의한다. 구체적인 기기 바인딩은 플랫폼 계약에 둔다.

## Requirements

- **REQ-UX-001:** 모든 필수 행동은 하나의 명명된 입력 액션에 연결되고 중복 충돌이 없어야 한다.
- **REQ-UX-002:** 전이 후보, 성공, 실패, 활성 상태, 회수를 서로 구분할 수 있어야 한다.
- **REQ-UX-003:** 체력/실패 위험, 원정 결과, 선택 결과, 새 기술을 플레이 진행을 멈추지 않고 확인할 수 있어야 한다.
- **REQ-UX-004:** 컷신·메뉴·장면 전환의 입력 잠금과 복구가 일관되어야 한다.

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

## Verification

입력 맵 정적 검사, 상태별 스크린 캡처, 잠금/복구 PlayMode 테스트, 짧은 이해도 관찰로 검증한다.

## Traceability

[ADR-0007](../../adr/0007-weight-transfer-is-the-core-player-verb.md), [VD-01](./01-player-movement.md), [VD-02](./02-weight-transfer.md), [VD-06](./06-humanity-choice-and-narrative.md)
