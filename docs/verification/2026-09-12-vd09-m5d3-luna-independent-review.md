# VD-09 M5D3 Luna 독립 구현 검증

- Date: 2026-09-12
- Reviewer: Luna
- Contract: [M5D3 프로필 진행 상태 투영 코어](../specs/work-contracts/2026-09-12-vd09-m5d3-profile-progression-projection-core.md)
- Implementation evidence: [Terra 구현 증적](./2026-09-12-vd09-m5d3-implementation-evidence.md)
- Result: **PASS — P0=0, P1=0, P2=1**

## 독립 판정

Luna는 Approved 계약, Profile runtime source/asmdef, focused tests와 root가 생성한 XML/log를 직접 대조했다. source/asmdef/test SHA-256은 Terra 증적과 일치했다.

| Acceptance criterion | Result | Independent finding |
|---|---|---|
| AC-M5D3-001 | PASS | revision `0`/`long.MaxValue`, null/네 seed, invalid revision/seed와 bypass 재검증이 닫혀 있다. |
| AC-M5D3-002 | PASS | 명시적 no-choice, 두 exact pair, one-sided/crossed/zero/unknown nullable enum 거부가 정확하다. |
| AC-M5D3-003 | PASS | 네 consent 값을 보존하고 pair-consent 추론이나 transition을 만들지 않는다. |
| AC-M5D3-004 | PASS | empty/single/both branch, canonical order, duplicate/reverse/unknown/null 거부와 입력·반환 defensive copy가 확인됐다. |
| AC-M5D3-005 | PASS | default snapshot/pair의 `Validate`, projection, getter가 throw하며 no-choice로 숨기지 않는다. |
| AC-M5D3-006 | PASS | Profile asmdef는 `noEngineReferences=true`이고 persistence/JSON/crypto/Unity authority가 없으며 allowlist를 지킨다. |
| AC-M5D3-007 | PASS | focused `10/10`, full EditMode `504/504`, 분류 후 full PlayMode `576/576`; 최종 failed/skipped/inconclusive `0`. |

## P2 — 기존 M5D1 finalizer 경고

최초 full PlayMode는 `575/576`이었고 유일한 실패는 기존 M5D1 `AcM5D1004_SourceTickOverflowClosesWithoutRequestOrCompletion` 실행 중 비동기로 도착한 `GameInputActions.Gameplay.Disable()` 미호출 finalizer warning이었다. Profile 코드는 stack에 없었다. 해당 테스트 단독 실행은 `1/1`, 분류 후 전체 재실행은 `576/576` PASS였다.

이는 M5D3 수용 차단은 아니지만 기존 M5D1 teardown 순서 위험으로 추적할 가치가 있다. M5D3 범위에서 기존 입력 코드를 수정하지 않은 것은 올바르다.

## 결론

M5D3는 계약 범위의 engine-free immutable progression projection으로 수용 가능하다. 이 PASS는 profile file/canonical codec/hash/atomic persistence/source owner 또는 M5A playable 연결의 검증으로 확장할 수 없다.
