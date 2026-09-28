# VD-09 M5D4 계약 독립 사전 게이트

- Date: 2026-09-12
- Reviewer: Luna
- Contract: [M5D4 프로필 설정·튜토리얼 값 코어](../specs/work-contracts/2026-09-12-vd09-m5d4-profile-settings-tutorial-core.md)
- Result: **PASS — P0=0, P1=0, P2=1**
- Execution: 문서 정적 검토만 수행; 구현/Unity 테스트 미실행

## 판정

- empty tutorial ID 허용은 VD-09 string array 계약과 일치한다. empty collection과 `['']`는 서로 다른 합법 상태다.
- UTF-16 surrogate validity를 NFC normalization 전에 검사하고, `StringComparer.Ordinal` strict ascending/unique를 써야 한다.
- decomposed input이나 normalization collision을 normalize/dedupe로 보정하지 않고 거부하는 경계가 안전하다.
- default settings의 zero mode와 default tutorial의 null backing collection은 유효 상태와 구분되며 `Validate()`와 모든 getter에서 거부할 수 있다.
- Q1000 `0..1000`, defensive copy, no-engine/no-persistence/source-authority와 allowlist가 parent와 충돌하지 않는다.

## P2 검증 지시

Terra focused tests는 valid surrogate pair, composed NFC, unpaired high/low surrogate, decomposed non-NFC, normalization collision/no-repair, empty collection과 single empty ID를 각각 포함해야 한다. 모든 public getter가 full `Validate()`를 선행하는지도 확인한다.

## 결론

필수 계약 수정 없이 Astra 승인 후 구현할 수 있다. 이 PASS는 settings/tutorial value contract만 다루며 persisted profile이나 실제 설정 적용을 검증하지 않는다.
