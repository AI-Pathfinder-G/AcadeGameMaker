# VD-09 M5D6 Luna 독립 구현 검증

- Date: 2026-09-12
- Reviewer: Luna
- Contract: [M5D6 프로필 입력 블록·호환성 코어](../specs/work-contracts/2026-09-12-vd09-m5d6-profile-input-compatibility-core.md)
- Implementation evidence: [Terra 구현 증적](./2026-09-12-vd09-m5d6-implementation-evidence.md)
- Result: **PASS — P0=0, P1=0, P2=2**

## AC 판정

- `AC-M5D6-001` PASS: current ID/version, empty/non-empty M5D5 override 보존과 structural invalid 거부가 정확하다.
- `AC-M5D6-002` PASS: compatibility `0/1/2/3`, ordinal/culture-independent 비교가 확인됐다.
- `AC-M5D6-003` PASS: binding schema `0/2/int.MaxValue`는 구조적으로 보존되며 exact bit 2 mismatch이고 `1`만 current다.
- `AC-M5D6-004` PASS: default, reflection ID/version/nested override bypass와 모든 getter/compatibility가 full validation을 수행한다.
- `AC-M5D6-005` PASS: current GUID가 실제 GameInput meta와 일치하며 outer schema/Unity/IO/persistence/apply authority가 없다.
- `AC-M5D6-006` PASS: focused `5/5`, full EditMode `521/521`, full PlayMode `576/576`; failed/skipped/inconclusive `0`, XML hash가 증적과 일치한다.

## P2

- unpaired low-surrogate ID와 noncanonical nested M5D5 text를 reflection으로 각각 주입하는 표본은 없으나 공통 `IsValidId`와 nested `Validate()` source가 거부한다.
- AC005 정적 test의 forbidden-token 목록은 전체 authority 검사를 대신하지 않지만 Luna가 source/asmdef/allowlist를 직접 검토해 no-engine/no-authority를 확인했다.

## 결론

M5D6는 input metadata compatibility core로 수용 가능하다. `IsCurrentCompatible`는 실제 override 적용 성공, input-only recovery 저장 또는 outer profile 유효성을 의미하지 않는다.
