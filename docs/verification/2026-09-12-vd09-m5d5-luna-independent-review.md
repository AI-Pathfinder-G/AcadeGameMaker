# VD-09 M5D5 Luna 독립 구현 검증

- Date: 2026-09-12
- Reviewer: Luna
- Contract: [M5D5 binding override JCS core](../specs/work-contracts/2026-09-12-vd09-m5d5-binding-overrides-jcs-core.md)
- Implementation evidence: [Terra 구현 증적](./2026-09-12-vd09-m5d5-implementation-evidence.md)
- Result: **PASS — P0=0, P1=0, P2=1**

## AC 판정

- `AC-M5D5-001` PASS: empty sentinel, null/whitespace/default와 public revalidation 경계가 정확하다.
- `AC-M5D5-002` PASS: recursive decoded duplicate, nested ordering/array order, depth 64 허용·65 거부가 확인됐다.
- `AC-M5D5-003` PASS: exact escaping, solidus/non-ASCII 보존, NFC/unpaired surrogate rejection이 닫혀 있다.
- `AC-M5D5-004` PASS: negative zero, min subnormal, max finite, safe-integer rounding, `1e-6`/`1e21`, culture-independent 출력이 golden과 일치한다. compiled assembly의 추가 finite binary64 999개 spot-check도 Node `Number.toString()`과 불일치가 없었다.
- `AC-M5D5-005` PASS: idempotence, equivalent input의 canonical text/UTF-8 hash 동일성과 public recanonicalization이 확인됐다.
- `AC-M5D5-006` PASS: default/reflection-bypass representation을 empty/canonical로 숨기지 않는다.
- `AC-M5D5-007` PASS: System.Text.Json 외 신규 dependency 없이 engine-free/no IO·crypto·input·persistence authority와 allowlist를 지킨다.
- `AC-M5D5-008` PASS: focused `7/7`, full EditMode `516/516`, 분류 후 full PlayMode `576/576`; 최종 failed/skipped/inconclusive `0`.

## 해소된 P1과 잔여 P2

초기 구현의 `MaxDepth=64` 미명시 P1은 Astra가 root 기준 exact 64를 Approved 계약에 추가하고 depth 64/65 focused test가 통과해 해소됐다.

첫 full PlayMode는 기존 M5D1 `GameInputActions.Gameplay.Disable()` finalizer warning 때문에 `575/576`이었다. M5D5 source/stack과 무관하고 분류된 전체 재검증이 `576/576`이므로 M5D5 차단은 아니다. 기존 teardown-order P2로 유지한다.

## 결론

M5D5는 RFC 8785 inner binding override canonicalization core로 수용 가능하다. 이 PASS는 Input System 적용, outer profile JSON/integrity, atomic save/load/recovery 또는 source authentication으로 확장할 수 없다.
