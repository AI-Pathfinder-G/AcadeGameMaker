---
status: Verified
---

# VD-09 M5D7B profile v1 canonical decoder and validator

- Date: 2026-09-13
- Owning Approved specs: [VD-09](../vertical-demo/09-platform-and-quality.md), [SYSTEM-CONTRACTS](../vertical-demo/SYSTEM-CONTRACTS.md)
- Decision basis: ADR-0010, ADR-0018, ADR-0031
- Dependency: [M5D7A canonical encoder](./2026-09-13-vd09-m5d7a-profile-v1-canonical-encoder.md) Verified
- Assigned by / final authority: Astra
- Contract design: Astra; implementation: Terra; independent pre/post verification: Luna
- Requirements: `REQ-PLAT-008`, `REQ-PLAT-010`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D7B-001` through `AC-M5D7B-009`; parent `AC-PLAT-006` is completed at the engine-free codec boundary while file selection and IO remain later work.
- Astra approval: 2026-09-13 after Luna second pre-gate PASS (P0=0, P1=0). Approval includes the non-input recovery projection repair and the P2 adversarial test gates below; only the exact allowlist is authorized.

## 목적과 경계

M5D7B는 외부에서 받은 하나의 byte array가 exact profile v1 canonical final-file representation인지 engine-free 방식으로 판정한다. 성공 시 M5D7A `ProfileCanonicalDocumentV1`과 입력 호환성 분류를 반환한다. 지원하지 않는 outer profile schema와 손상된 v1을 구분하며, M5D6 input metadata mismatch는 전체 profile 손상으로 오인하지 않고 input-only recovery 필요 상태로 보존한다.

이 단위는 byte parser/validator일 뿐 파일 경로 선택, disk read/write, primary/previous/temp 우선순위, quarantine, revision 증가, input binding 실제 적용, input default 치환, atomic save 또는 알림을 수행하지 않는다. SHA-256은 손상 검출이며 authenticity/security signature가 아니다.

## 승인 API

기존 engine-free `AcadeGameMaker.Profile` assembly에 새 파일 하나로 아래 public API를 추가한다.

- `ProfileCanonicalDecodeClassification`: `ValidCurrentInput`, `ValidInputMetadataRecoveryRequired`, `ValidBindingRecoveryRequired`, `UnsupportedProfileSchema`, `Invalid`.
- `ProfileCanonicalDecodeFailure`: `None`, `MalformedJson`, `SchemaViolation`, `CanonicalOrIntegrityMismatch`.
- flags enum `ProfileInputRecoveryReason`: `None`, `AssetIdMismatch`, `BindingSchemaMismatch`, `BindingOverridesMalformedOrNonCanonical`.
- immutable `ProfileInputRecoveryProjectionV1(ProfileSettingsSnapshot settings, ProfileTutorialSnapshot tutorial, ProfileProgressionSnapshot progression)` with exact typed getters `long SourceProfileRevision`, `ProfileSettingsSnapshot Settings`, `ProfileTutorialSnapshot Tutorial`, `ProfileProgressionSnapshot Progression` and `Validate()`; it deliberately has no input field.
- immutable `ProfileCanonicalDecodeResultV1` with exact typed getters `ProfileCanonicalDecodeClassification Classification`, `ProfileCanonicalDocumentV1 Document`, `ProfileInputCompatibility InputCompatibility`, `ProfileInputRecoveryProjectionV1 RecoveryProjection`, `ProfileInputRecoveryReason InputRecoveryReasons`, `int UnsupportedSchemaVersion`, `ProfileCanonicalDecodeFailure Failure`, and `Validate()`.
- `ProfileCanonicalDecoderV1.Decode(byte[] canonicalFileBytes)` static method.

`null` argument는 `ArgumentNullException`이다. 그 밖의 untrusted byte/data 오류는 exception으로 유출하지 않고 `Invalid` 결과로 귀결한다. programmer-created default/reflection-bypass result와 classification/getter 불일치는 `Validate()` 및 모든 public getter에서 `InvalidOperationException`으로 거부한다.

Getter availability:

- `ValidCurrentInput`과 `ValidInputMetadataRecoveryRequired`만 `Document`와 `InputCompatibility`를 노출한다. 전자는 exact `Current`; 후자는 exact nonzero M5D6 flags이며 같은 flags가 `InputRecoveryReasons`의 asset/schema bit로 투영된다.
- 두 recovery classification은 `RecoveryProjection`과 non-`None` `InputRecoveryReasons`를 노출한다. `ValidBindingRecoveryRequired`에는 항상 `BindingOverridesMalformedOrNonCanonical` bit가 있고 metadata mismatch가 함께 있으면 그 bit도 합친다. 이 classification은 `Document`를 노출하지 않는다.
- `UnsupportedProfileSchema`만 `UnsupportedSchemaVersion`을 노출하며 `Failure == None`이다.
- `Invalid`만 non-`None` `Failure`를 노출한다.
- availability 밖 getter는 `InvalidOperationException`이다.

Decoder는 caller array를 즉시 clone해 이후 caller mutation과 분리한다. 결과에 source byte array나 mutable parse tree를 보관하지 않는다. valid document byte getters의 방어 복사는 M5D7A를 따른다.

## 판정 순서와 outer schema 분리

1. 빈 byte, UTF-8 BOM, malformed UTF-8, final newline/whitespace, JSON comment/trailing comma, non-object root는 `Invalid/MalformedJson`이다. UTF-8 decode는 replacement fallback 없이 수행한다. JSON parse max depth는 64다.
2. top-level에서 decoded name이 exact `schemaVersion`인 property를 ordinal 기준으로 센다. 없거나 둘 이상이면 `Invalid/SchemaViolation`; 값이 JSON integer이 아니거나 signed 32-bit 범위를 벗어나도 `Invalid/SchemaViolation`이다.
3. 유일한 outer `schemaVersion != 1`이면 나머지 field를 v1로 추측하거나 parse하지 않고 `UnsupportedProfileSchema`와 exact integer version을 반환한다. 이 classification은 canonical/valid future profile 주장이나 migration이 아니다.
4. version 1이면 exact v1 field set, property order, type와 nested constraint를 parse한다. 먼저 outer string으로서 `bindingOverridesJson`을 보존한 채 settings/tutorial/progression과 input ID/nonnegative schema를 검증한다. missing/unknown/duplicate/out-of-order field, number token의 fraction/exponent/leading-zero/overflow, unknown enum spelling, invalid nullability, invalid array/order/duplicate, invalid outer NFC/surrogate/control representation은 `Invalid/SchemaViolation`이다.
5. supplied `integrity.payloadSha256`는 exact lowercase `[0-9a-f]{64}`여야 한다. decoder의 private outer writer는 raw `bindingOverridesJson` string을 normalize/parse하지 않고 M5D7A outer string 규칙으로 재구성한다. reconstructed raw payload hash와 supplied integrity를 fixed-time 비교하고 reconstructed raw final bytes와 cloned source bytes 전체를 byte-exact 비교한다. 실패는 `Invalid/CanonicalOrIntegrityMismatch`다. 이 writer는 decoder 검증용이며 public encoder/document authority가 아니다.
6. outer canonical/hash가 정상인 뒤 non-empty raw binding string을 M5D5로 parse/canonicalize한다. malformed이거나 `CanonicalText`가 raw string과 ordinal 불일치하면 전체 profile을 invalid로 버리지 않는다. 검증된 settings/tutorial/progression으로 `ProfileInputRecoveryProjectionV1`를 만들고 `ValidBindingRecoveryRequired`와 해당 reason flags를 반환한다. 빈 문자열은 no-override sentinel이다. 이 결과는 canonical document나 persisted-valid profile 주장이 아니며 후속 owner가 current input defaults로 교체해야 한다.
7. binding string도 canonical이면 M5D6 input과 M5D7A snapshot/document를 구축하고 regenerated M5D7A final bytes를 source와 다시 exact 비교한다. `Input.Compatibility == Current`는 `ValidCurrentInput`, nonzero known flags는 `ValidInputMetadataRecoveryRequired`다. metadata recovery 결과도 동일 non-input projection과 reason flags를 제공하며 정상 save 가능 판정이 아니다.

Failure precedence는 위 순서로 고정한다: byte/UTF-8/JSON lexical failure → schemaVersion singleton/type → unsupported outer version → v1 structural constraint → integrity shape/outer canonical/hash → inner binding recovery → metadata compatibility. UTF-8 BOM 또는 leading/trailing whitespace/newline은 첫 단계 `MalformedJson`; duplicate schema는 `SchemaViolation`; uppercase/short/nonhex integrity는 `CanonicalOrIntegrityMismatch`; inner malformed/noncanonical은 outer/hash가 exact일 때만 recovery다. JSON `MaxDepth=64`는 System.Text.Json root depth semantics 그대로 사용하며 depth 64는 parser가 허용하고 65는 `MalformedJson`이다. Decoder entry는 예상 가능한 `DecoderFallbackException`, `JsonException`, overflow/format/argument/invalid-operation data rejection을 typed result로 닫되 `OutOfMemoryException`, `StackOverflowException` 같은 fatal runtime exception을 삼키지 않는다.

Version 1 exact top-level order는 M5D7A payload six fields 뒤 `integrity`다. 모든 nested order와 enum/string/integer/array 규칙도 M5D7A를 그대로 사용한다. parser가 임의 기본값을 채우거나 normalize, trim, sort, deduplicate, enum case-fold하지 않는다.

## 요구사항

- **REQ-M5D7B-001:** untrusted bytes를 strict UTF-8/JSON으로 parse하고 outer unsupported schema, invalid v1, valid current-input, valid metadata recovery, valid binding recovery 상태로 결정론적으로 분류한다.
- **REQ-M5D7B-002:** version 1의 exact field set/order/type/value constraints를 시행하고 unknown/missing/duplicate/out-of-order 또는 implicit coercion/default를 거부한다.
- **REQ-M5D7B-003:** raw outer binding string을 보존한 private reconstruction으로 payload-only SHA와 supplied integrity 및 full source bytes를 검증하고, binding도 canonical인 경우 M5D7A 재인코딩까지 exact 일치할 때만 document를 반환한다.
- **REQ-M5D7B-004:** M5D6 input metadata mismatch 및 malformed/noncanonical binding string을 canonical outer profile 손상과 구분해 reason flags와 non-input recovery projection으로 보존한다.
- **REQ-M5D7B-005:** null/default/reflection-bypass/result getter misuse와 mutable input alias를 방어하며 data 오류는 typed invalid result로 닫는다.
- **REQ-M5D7B-006:** no IO/path/file selection/quarantine/revision mutation/recovery write/input apply/clock/RNG/network/callback 경계를 지키고 existing M5D3~M5D7A API, asmdef, packages, settings와 assets를 바꾸지 않는다.

## 수용 기준

- **AC-M5D7B-001:** M5D7A가 만든 default·Extraction·Solidarity 대표 final bytes를 decode하면 original snapshot, payload/hash/final bytes가 exact 일치하고 current input은 `ValidCurrentInput`이다.
- **AC-M5D7B-002:** wrong asset ID, schema `0`, `2`, `int.MaxValue`, 두 mismatch 동시 조합의 M5D7A final bytes는 `ValidInputMetadataRecoveryRequired`, exact document/M5D6 flags/reason flags와 non-input projection을 반환한다.
- **AC-M5D7B-003:** unique integer outer schema `0`, `2`, `int.MaxValue`는 `UnsupportedProfileSchema`와 exact version을 반환하고 v1 나머지 field를 해석하지 않는다. missing/duplicate/non-integer/out-of-int outer schema는 `Invalid/SchemaViolation`이다.
- **AC-M5D7B-004:** null/empty/BOM/malformed UTF-8, comments, trailing comma, whitespace/newline, non-object, truncated JSON은 exception 없이 정해진 invalid 결과가 된다. null argument만 `ArgumentNullException`이다.
- **AC-M5D7B-005:** 각 level의 missing/unknown/duplicate/out-of-order property, wrong JSON type, negative revision/schema constraints, integer overflow/fraction/exponent/leading-zero, unknown/case-changed enum, invalid null/array order/duplicate/NFC/surrogate는 `Invalid/SchemaViolation`이다. Outer duplicate schema는 unsupported보다 invalid가 우선하며 negative outer version은 exact `UnsupportedProfileSchema(-1)`로 분류한다.
- **AC-M5D7B-006:** changed payload with old hash, changed hash, uppercase/short/nonhex hash, duplicate/relocated integrity, hash를 다시 맞춘 whitespace/property reorder/alternate outer escaping은 `Invalid/CanonicalOrIntegrityMismatch` 또는 더 이른 strict failure다. 반면 outer canonical/hash를 정확히 다시 만든 malformed inner JSON 및 semantically valid하지만 noncanonical inner JCS는 `ValidBindingRecoveryRequired`, exact non-input projection과 binding reason을 반환하며 document를 노출하지 않는다.
- **AC-M5D7B-007:** source array mutation, repeated Decode/Validate와 result/document/projection getters가 deterministic/idempotent하고 default/reflection-bypassed result/document/projection/classification payload 및 getter availability 위반이 거부된다.
- **AC-M5D7B-008:** static review가 only approved fields, strict UTF-8/JSON, max depth 64, no forbidden authority, engine-free boundary와 strict allowlist를 확인한다. 성공을 disk persistence/load recovery/input apply/authenticity PASS로 확대하지 않는다.
- **AC-M5D7B-009:** Luna independent review와 focused/full EditMode, full PlayMode가 failed/skipped/inconclusive `0`으로 통과한다.

## 정확한 구현 allowlist

Runtime:

- `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalDecoderV1.cs`
- matching `.meta`

Tests:

- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalDecoderV1Tests.cs`
- matching `.meta`

Evidence/index:

- this contract
- `docs/verification/2026-09-13-vd09-m5d7b-contract-pregate.md`
- `docs/verification/2026-09-13-vd09-m5d7b-implementation-evidence.md`
- `docs/verification/2026-09-13-vd09-m5d7b-luna-independent-review.md`
- minimal M5D7B entry in `docs/README.md`

Existing M5D3~M5D7A runtime/test files, Profile asmdefs, `Packages/**`, `ProjectSettings/**`, scenes, prefabs, input/actions and other assets are read-only dependencies and forbidden changes.

## 실행·중단 게이트

Luna pre-gate → Astra Approved → Terra allowlist implementation → root Unity focused/full execution → Luna post-review → Astra integration 순서다. File IO/selection/quarantine, migration, recovery mutation/atomic save, actual input binding apply, existing API change 또는 allowlist 밖 변경이 필요하면 중단하고 별도 계약으로 넘긴다.

Focused tests는 failure precedence별 representative, JSON depth 64/65, strict UTF-8 no-replacement, source-array post-call mutation, result getter availability, and exception containment를 직접 포함해야 한다.
It must also cover decomposed/unpaired outer text precedence, malformed suffix on unsupported schema, escaped duplicate `schemaVersion`, integrity-shape precedence, unknown reflection-injected recovery reason flags, reflection-bypassed projection/result invariants, and unavailable getter rejection.

## Astra draft rationale

Decoder가 semantic JSON만 수용하면 hash까지 다시 계산한 noncanonical outer byte를 정상 profile로 통과시킬 수 있으므로 strict outer reconstruction과 full bytes exact 비교를 유지한다. 그러나 VD-09는 malformed/noncanonical inner binding도 input-only recovery 대상으로 규정한다. 따라서 outer canonical/hash 검증을 inner binding validation보다 먼저 수행하고, inner만 실패하면 input이 없는 immutable projection으로 settings/tutorial/progression/source revision을 보존한다. M5D6 metadata mismatch는 document를 만들 수 있으므로 별도 recovery classification으로 둔다.
