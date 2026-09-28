# ADR-0035: 새 게임 전 진행은 수동 복구 전용으로 보관한다

- Status: accepted product direction; implementation pending
- Date: 2026-09-28
- Decision owner: user; integrated by Astra

## Context

VD-09의 단일 프로필은 `profile.json`, `profile.prev.json`, `profile.tmp.json`을 사용한다. 정상 저장 뒤 새 primary가 손상되면 loader가 previous를 자동 선택할 수 있다. 따라서 이전 진행을 둔 채 새 기본 primary만 덮어쓰면, 새 게임 뒤 과거 진행이 다시 나타날 수 있다.

## Decision

- 기존 진행이 의미 있게 있는 상태의 `새 게임`은 손실을 알리는 확인을 받은 뒤에만 현재 단일 프로필을 초기화한다. 취소는 파일과 메모리를 변경하지 않는다.
- 초기화 전 이전 진행은 별도 수동 복구용 자료로 보존한다. 새 게임 뒤 이를 정상 시작이나 자동 복구의 후보로 삼지 않는다.
- 초기화 직후뿐 아니라 새 primary가 손상되거나 초기화 중 앱이 종료된 뒤에도 이전 진행을 자동 선택하지 않는다. 불확실한 경우에는 성공을 표시하거나 옛 진행으로 조용히 되돌리지 않고 안전하게 중단한다.
- 이 결정은 이전 자료의 영구 보관을 기본으로 하며 자동 정리·삭제·복원 UI를 승인하지 않는다. 사용자가 요청한 보안 삭제를 뜻하지 않는다.

## Rationale

수동 복구 가능성과 새 게임의 예측 가능한 시작을 함께 보장한다. 기존 `previous`를 자동 후보로 남기는 방식은 새 게임의 의미를 깨뜨리고, 이전 자료를 즉시 삭제하는 방식은 사고 복구 가능성을 잃는다.

## Consequences

- VD-09의 일반 loader/저장 계약에 새 게임 전환 중 적용할 명시적 예외가 필요하다. 초기화 표식 또는 그와 동등하게 검증 가능한 전환 경계를 둬야 한다.
- 전환 중 종료, 손상 파일, 동시 저장, 메모리·입력 상태 전환을 독립적으로 검증하기 전에는 실제 reset effect를 연결하지 않는다.
- 기존 M5D7Q-B의 synthetic-only 요청 전달은 그대로 유지된다. 본 ADR만으로 초기화 구현이나 장면 전환을 승인하지 않는다.

## Evidence

- `docs/approvals/2026-09-28-new-game-single-profile-confirmation-approval.md`
- `docs/specs/vertical-demo/09-platform-and-quality.md`
