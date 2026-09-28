# VD-09 M5D4 Luna 독립 구현 검증

- Date: 2026-09-12
- Reviewer: Luna
- Contract: [M5D4 프로필 설정·튜토리얼 값 코어](../specs/work-contracts/2026-09-12-vd09-m5d4-profile-settings-tutorial-core.md)
- Implementation evidence: [Terra 구현 증적](./2026-09-12-vd09-m5d4-implementation-evidence.md)
- Result: **PASS — P0=0, P1=0, P2=1**

## AC 판정

- `AC-M5D4-001` PASS: mode `1/2`, Q1000 `0/1000`, boolean 조합과 invalid constructor/bypass 경계가 닫혀 있다.
- `AC-M5D4-002` PASS: empty list와 `['']`, ordinal sorted unique, valid surrogate pair, unpaired high/low, decomposed NFC와 normalization collision/no-repair를 검증한다.
- `AC-M5D4-003` PASS: 입력 배열, generic/non-generic 반환 mutation과 반복 getter가 내부 상태를 바꾸지 않는다.
- `AC-M5D4-004` PASS: default 두 값의 `Validate()`와 모든 getter, settings bypass와 tutorial text bypass가 거부된다. 공통 source 경로는 null/order/duplicate bypass도 거부한다.
- `AC-M5D4-005` PASS: no-engine, no IO/JSON/crypto/clock/RNG/network/callback/progression authority와 allowlist가 확인됐다.
- `AC-M5D4-006` PASS: focused `5/5`, full EditMode `509/509`, full PlayMode `576/576`; failed/skipped/inconclusive `0`. XML SHA-256은 Terra 증적과 일치한다.

## P2

AC004가 reflection으로 tutorial backing list의 null/order/duplicate를 각각 주입하는 테스트까지는 포함하지 않는다. 생성자 반례와 동일한 `AreCanonicalConfirmedIds` 재검증 source가 이 셋을 모두 거부하므로 수용 차단은 아니며, 후속 Profile test matrix 보강 시 포함할 수 있다.

## 결론

M5D4는 settings/tutorial value core로 수용 가능하다. 이 PASS는 실제 settings 적용, tutorial confirmation event, profile JSON/hash/IO 또는 저장된 source 인증으로 확장할 수 없다.
