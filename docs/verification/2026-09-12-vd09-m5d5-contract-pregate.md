# VD-09 M5D5 계약 독립 사전 게이트

- Date: 2026-09-12
- Reviewer: Luna
- Contract: [M5D5 binding override JCS core](../specs/work-contracts/2026-09-12-vd09-m5d5-binding-overrides-jcs-core.md)
- Result: **PASS — P0=0, P1=0, P2 implementation gates**
- Execution: 문서·환경 정적 검토만 수행; 구현/Unity 테스트 미실행

## 판정

- Unity 6000.6 netstandard 2.1 targeting pack과 현 Profile compiler response에 `System.Text.Json` 자동 reference가 있어 asmdef/Packages 변경 없이 구현 가능하다.
- RFC 8785의 decoded duplicate 금지, Unicode normalization 미수행, UTF-16 property sort, ECMAScript-compatible number/string serialization과 계약이 일치한다.
- empty sentinel과 default null backing을 구분하고 public consumption에서 재검증할 수 있다.
- Input System 적용, outer profile/hash/IO/persistence/source authentication을 주장하지 않으며 allowlist가 닫혀 있다.

## 의무 P2 구현 게이트

1. strict `JsonDocumentOptions`, single root, recursive decoded-name duplicate 검사를 사용한다.
2. 기본 writer의 HTML/non-ASCII escaping에 의존하지 않고 exact string escaping과 UTF-16 ordinal sort를 구현·검증한다.
3. binary64 불가의 의미는 lexical exactness가 아닌 finite conversion overflow/failure다. RFC rounding, negative zero, min subnormal, `1e-6`, `1e21`, safe-integer와 Appendix B vector를 golden test로 고정한다.
4. public revalidation은 null/empty만 보지 않고 non-empty canonical text를 deterministic하게 다시 검증한다.

## 결론

Astra 승인 뒤 exact allowlist로 구현할 수 있다. golden mismatch를 허용오차나 완화된 string 비교로 통과시킬 수 없다.

## 구현 후 P1 계약 보완

Luna의 구현 검토는 runtime `MaxDepth=64`가 초안에 없다는 P1을 발견했다. Astra는 무제한 재귀 대신 `System.Text.Json`의 root 기준 depth 64를 validator resource bound로 승인하고 계약의 REQ-M5D5-002/AC-M5D5-002에 depth 64 통과·65 거부를 명시했다. Runtime 값은 바뀌지 않으며 focused 경계 테스트가 수용에 필요하다.
