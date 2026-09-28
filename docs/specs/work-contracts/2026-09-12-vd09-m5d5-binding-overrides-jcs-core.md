---
status: Verified
---

# VD-09 M5D5 binding override RFC 8785 canonicalization 코어

- Date: 2026-09-12
- Owning Approved specs: [VD-09](../vertical-demo/09-platform-and-quality.md), [SYSTEM-CONTRACTS](../vertical-demo/SYSTEM-CONTRACTS.md)
- Decision basis: ADR-0010, ADR-0018, ADR-0031; RFC 8785
- Dependency: [M5D3 Verified](./2026-09-12-vd09-m5d3-profile-progression-projection-core.md), [M5D4 Verified](./2026-09-12-vd09-m5d4-profile-settings-tutorial-core.md)
- Assigned by / final authority: Astra
- Implementation: Terra; independent pre/post verification: Luna
- Requirements: `REQ-PLAT-006`, `REQ-PLAT-008`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D5-001` through `AC-M5D5-008`; parent AC-PLAT-006/009 are partially exercised only.
- Astra approval: 2026-09-12 after Luna pre-gate PASS (P0=0, P1=0). Luna P2 implementation gates are mandatory; only the exact allowlist below is authorized and this is not implementation verification.
- Verified: 2026-09-12 — Luna final independent review PASS (P0=0, P1=0; one non-blocking existing M5D1 finalizer P2) and Astra accepts this bounded integration. See [implementation evidence](../../verification/2026-09-12-vd09-m5d5-implementation-evidence.md) and [Luna review](../../verification/2026-09-12-vd09-m5d5-luna-independent-review.md). This verifies only inner binding JSON canonicalization, not binding application, outer profile/hash or persistence.

## 목적과 경계

VD-09는 비어 있지 않은 `bindingOverridesJson`을 parse하고 duplicate key를 거부한 뒤 RFC 8785 canonical text로 만들어 outer profile string에 저장하도록 승인했다. M5D5는 이 inner JSON 변환만 담당하는 engine-free pure value/core다. empty string은 승인된 no-override sentinel이며 parse하지 않는다.

이 단위는 Input System override를 적용하거나 의미를 검사하지 않고, profile outer JSON/hash/file도 만들지 않는다. canonical text는 인증 capability가 아니며 후속 input compatibility 및 profile codec/loader 검증을 대신하지 않는다.

## 승인 API

기존 `AcadeGameMaker.Profile` assembly에 새 public immutable `ProfileBindingOverridesJson` readonly struct를 추가한다.

- `public static ProfileBindingOverridesJson Parse(string source)`
- `public bool IsEmpty`
- `public string CanonicalText`
- `public void Validate()`

`source=null`은 `ArgumentNullException`; `source==string.Empty`은 backing text가 명시적 empty인 합법 sentinel이다. 비어 있지 않은 whitespace-only text는 JSON이 아니므로 invalid다. non-empty source는 strict parse/canonicalization 후 canonical text만 보존하고 원문은 보유하지 않는다.

default struct는 null backing text로 invalid다. `Validate`, `IsEmpty`, `CanonicalText` getter가 모두 재검증하고 default/constructor-bypass malformed text를 `InvalidOperationException`으로 거부한다. public getter가 재parse할 필요는 없지만 internal validation marker만 믿지 말고 canonical text를 deterministic revalidation하거나 생성 경계를 위조할 수 없는 representation으로 닫아야 한다.

## JSON input과 duplicate 규칙

- `System.Text.Json`은 Unity 6000.6 netstandard 2.1 BCL에서 제공되는 strict parser로만 사용한다. Packages/asmdef/precompiled reference를 추가하지 않는다. 실제 Unity focused compile 실패 시 범위를 넓히지 않고 Astra로 반환한다.
- comments, trailing comma, 여러 root value, invalid escape/UTF-8 surrogate 표현, non-JSON number는 invalid다. root는 RFC 8785가 허용하는 object/array/string/number/boolean/null 모두 가능하다. insignificant leading/trailing whitespace는 parse할 수 있고 출력에서 제거한다.
- parser resource bound는 root container/value를 depth 1로 세는 `System.Text.Json` 의미의 최대 depth `64`다. depth 64는 합법이고 65 이상은 invalid다. 이 bound는 Input System override의 gameplay 의미를 제한하지 않고 손상·악성 profile의 비제한 재귀를 막는 VD-09 validator 경계다.
- 모든 object level에서 decoded property name의 ordinal duplicate를 거부한다. escape spelling이 달라도 decoded name이 같으면 duplicate다. array 안 object도 같은 규칙이다.
- decoded property name과 string value는 unpaired surrogate가 없어야 하고 이미 NFC여야 한다. M5D5는 RFC 8785 입력을 normalization하지 않는다. decomposed text와 normalization-collision 후보를 보정하지 않고 invalid로 거부해 inner JCS 의미와 outer profile NFC 규칙을 동시에 보존한다.

## Canonical serialization

- object property는 decoded name의 UTF-16 code unit lexicographic order(`StringComparer.Ordinal`)로 재귀 정렬한다. array order는 보존한다.
- null/true/false는 lowercase literal이다.
- string은 quote/backslash와 U+0000..001F만 escape한다. `\b`, `\t`, `\n`, `\f`, `\r` short escape를 사용하고 나머지 control은 lowercase `\u00xx`; solidus와 다른 Unicode scalar는 escape하지 않는다.
- number는 JSON parser의 finite IEEE-754 binary64 값으로 해석하고 ECMAScript-compatible shortest round-trip decimal을 쓴다. negative zero는 `0`; `1e-6 <= abs(n) < 1e21`은 decimal form, 그 밖의 nonzero는 lowercase `e` scientific form이며 positive exponent에는 `+`, exponent leading zero는 없다. NaN/Infinity와 binary64로 표현 불가한 입력은 invalid다.
- 출력에는 whitespace/BOM/final newline이 없고, 같은 parsed value는 같은 .NET string과 UTF-8 byte를 만든다.
- `double.ToString()`의 culture/default scientific threshold를 그대로 canonical 결과로 쓰지 않는다. 구현은 invariant shortest digits를 얻은 뒤 RFC/ECMAScript threshold와 exponent spelling을 명시적으로 정규화하고 RFC 8785 Appendix B 및 경계 golden vector로 검증한다.

## 요구사항

- **REQ-M5D5-001:** empty override sentinel과 non-empty strict JSON을 구분하고 non-empty만 recursive RFC 8785 canonical text로 변환한다.
- **REQ-M5D5-002:** 최대 depth 64 안의 모든 object depth에서 decoded duplicate key를 검사하고, depth 65+, invalid syntax/escape/surrogate, non-NFC decoded name/value와 non-finite/unrepresentable number를 보정 없이 거부한다.
- **REQ-M5D5-003:** property ordinal order, array order, exact string escaping과 ECMAScript-compatible binary64 number spelling으로 whitespace/BOM/newline 없는 deterministic text를 만든다.
- **REQ-M5D5-004:** immutable value와 public revalidation이 default/bypass malformed representation을 empty sentinel로 오인하지 않는다.
- **REQ-M5D5-005:** Input System 의미·적용, asset/schema match, outer profile/schema/hash/IO/save/load/recovery/source authentication을 수행하거나 주장하지 않는다.
- **REQ-M5D5-006:** 기존 public API/asmdef/Packages/ProjectSettings/assets를 변경하지 않고 Profile engine-free 경계와 no IO/crypto/clock/RNG/network/callback을 유지한다.

## 수용 기준

- **AC-M5D5-001:** exact empty string은 valid empty sentinel이고 whitespace-only/null/default는 각각 계약대로 거부되며 public getter가 default를 false/empty로 숨기지 않는다.
- **AC-M5D5-002:** nested objects/arrays의 key 재정렬과 array order 보존이 exact canonical text로 확인되고 raw/escaped 동명 key, nested duplicate, trailing content/comment/comma가 거부된다. root 기준 depth 64 표본은 통과하고 동일 구조 depth 65는 거부된다.
- **AC-M5D5-003:** quote/backslash/control short escape/lowercase `\u00xx`, solidus 미escape, BMP/valid surrogate-pair scalar 미escape가 golden text와 일치한다. unpaired surrogate와 decomposed non-NFC name/value는 거부된다.
- **AC-M5D5-004:** RFC 8785 Appendix B의 zero/negative-zero, min subnormal, max finite, safe-integer 주변과 `1e-6`, `1e21` 경계 대표 binary64 vector가 exact expected spelling과 일치하고 culture 변경에도 byte가 같다.
- **AC-M5D5-005:** already canonical input은 idempotent하고, 서로 다른 whitespace/property order/escape spelling의 같은 parsed value는 exact same text/UTF-8 SHA-256을 만든다. hash는 테스트 증적용이며 runtime API가 소유하지 않는다.
- **AC-M5D5-006:** reflection으로 null/noncanonical backing text를 삽입한 값의 `Validate`/`IsEmpty`/`CanonicalText`가 throw하고 실패 뒤 valid Parse는 정상이다.
- **AC-M5D5-007:** static review가 `System.Text.Json` 이외 신규 dependency 없음, no-engine/no IO/crypto/clock/RNG/network/callback, existing ABI/asmdef/Packages/ProjectSettings/assets 무변경과 exact REQ trace를 확인한다.
- **AC-M5D5-008:** Luna 독립 review와 focused/full EditMode, full PlayMode가 failed/skipped/inconclusive `0`으로 통과한다. canonicalizer PASS를 binding 적용이나 persisted profile PASS로 확대하지 않는다.

## 정확한 구현 allowlist

Runtime:

- new `Assets/AcadeGameMaker/Runtime/Profile/ProfileBindingOverridesJson.cs` and `.meta`

Tests:

- new `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileBindingOverridesJsonTests.cs` and `.meta`

Documents:

- this contract
- later `docs/verification/2026-09-12-vd09-m5d5-contract-pregate.md`
- later `docs/verification/2026-09-12-vd09-m5d5-implementation-evidence.md`
- later `docs/verification/2026-09-12-vd09-m5d5-luna-independent-review.md`
- `docs/README.md` minimum index/status link

M5D3/M5D4 source/tests, Profile asmdef, Packages, ProjectSettings, 그 밖의 runtime/test, prefab/scene/assets/media 변경은 금지한다.

## 검증 순서와 중단 조건

Luna는 parser strictness, duplicate decoding, UTF-16 sort, string escape, numeric shortest/threshold, default representation과 BCL availability를 pre-gate한다. Astra Approved 뒤 Terra가 구현하고 root가 Unity를 실행하며 Luna가 독립 post-review한다.

System.Text.Json이 현재 asmdef에서 compile되지 않거나 RFC Appendix B를 exact 통과시키려면 asmdef/package/native dependency 변경이 필요하면 구현을 중단하고 Astra로 반환한다. number vector mismatch를 허용오차나 string 비교 완화로 숨기지 않는다.

## 롤백과 참여 기록

Rollback point는 M5D4 Verified다. 되돌리기는 승인 시 M5D5 신규 파일과 문서/index hunk만 대상으로 하고 reset/clean/checkout/다른 dirty 파일 삭제를 사용하지 않는다. commit과 remote publication은 암시되지 않는다.

Astra가 VD-09 inner binding JSON 규칙과 Unity 6000.6의 `System.Text.Json` BCL 존재를 대조해 초안을 작성했다. 공식 RFC open은 root 도구 응답이 없었지만 Luna가 RFC 원문 및 현재 Profile compiler response의 자동 reference를 독립 대조했다. pre-gate는 P0=0/P1=0으로 통과했고 strict options/decoded duplicate/custom string+number serialization/full canonical revalidation을 P2 구현 게이트로 남겼다. 구현 후 Luna가 계약에 없던 `MaxDepth=64`를 P1로 발견했고, Astra는 parser resource bound를 root 기준 exact 64로 명시하고 64/65 경계 테스트를 요구해 이를 해소했다.
