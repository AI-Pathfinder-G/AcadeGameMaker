---
status: Verified
---

# VD-09 M5D7A profile v1 canonical envelope encoder

- Date: 2026-09-13
- Owning Approved specs: [VD-05](../vertical-demo/05-failure-and-persistence.md), [VD-09](../vertical-demo/09-platform-and-quality.md), [SYSTEM-CONTRACTS](../vertical-demo/SYSTEM-CONTRACTS.md)
- Decision basis: ADR-0010, ADR-0018, ADR-0031
- Dependencies: [M5D3](./2026-09-12-vd09-m5d3-profile-progression-projection-core.md), [M5D4](./2026-09-12-vd09-m5d4-profile-settings-tutorial-core.md), [M5D5](./2026-09-12-vd09-m5d5-binding-overrides-jcs-core.md), [M5D6](./2026-09-12-vd09-m5d6-profile-input-compatibility-core.md) Verified
- Assigned by / final authority: Astra
- Implementation: Terra; independent pre/post verification: Luna
- Requirements: `REQ-RUN-003`, `REQ-PLAT-006`, `REQ-PLAT-008`
- Acceptance IDs: `AC-M5D7A-001` through `AC-M5D7A-008`; parent AC-PLAT-006 is partially exercised.
- Astra approval: 2026-09-13 after Luna second pre-gate PASS (P0=0, P1=0). Only the exact allowlist below is authorized; this is not implementation verification.

## 목적과 경계

M5D3~M5D6는 profile v1의 progression, settings/tutorial, binding override canonical text, input metadata를 각각 검증했다. M5D7A는 이 네 immutable value를 하나의 full profile snapshot으로 결합하고, VD-09의 고정 property order와 string/integer 규칙으로 canonical payload bytes와 lowercase SHA-256 integrity를 생성한다.

이 단위는 encoder만 소유한다. 임의 파일 bytes를 parse하거나 hash가 맞다고 인증하지 않으며, disk write/load/recovery 또는 Input System 적용도 하지 않는다. 후속 decoder가 exact file bytes를 다시 encode해 동일성을 확인하기 전에는 persisted source로 부를 수 없다.

M5D6가 구조적으로 보존한 asset ID/schema mismatch도 encoder 입력으로 허용한다. 이는 후속 decoder가 mismatch profile의 canonical byte/hash를 재현하고 input-only recovery 대상으로 분류하기 위해 필요하다. 이 결과는 **recovery 전 canonical candidate**일 뿐 current-compatible persisted-valid profile이나 새 정상 save로 주장할 수 없다. 후속 atomic writer는 recovery owner가 current input defaults로 교체하고 revision을 증가시킨 current-compatible snapshot만 정상 commit하도록 별도 계약해야 한다.

## 승인 API

기존 `AcadeGameMaker.Profile` engine-free assembly에 아래 public immutable API를 새 파일 하나로 추가한다.

- `ProfileContract.SchemaVersion = 1` public const.
- `ProfileSnapshotV1(ProfileSettingsSnapshot settings, ProfileInputSnapshot input, ProfileTutorialSnapshot tutorial, ProfileProgressionSnapshot progression)` readonly struct.
- snapshot getters `SchemaVersion`, `ProfileRevision`, `Settings`, `Input`, `Tutorial`, `Progression`과 `public void Validate()`.
- `ProfileCanonicalDocumentV1` readonly struct getters `Snapshot`, `CanonicalPayloadBytes`, `PayloadSha256`, `CanonicalFileBytes`와 `public void Validate()`.
- `ProfileCanonicalEncoder.Encode(ProfileSnapshotV1 snapshot)` static method returning `ProfileCanonicalDocumentV1`.

`ProfileRevision`은 `Progression.ProfileRevision`의 exact projection이며 별도 mutable/redundant field를 갖지 않는다. encoder는 revision을 증가시키지 않는다. 각 nested value는 constructor, snapshot `Validate`, encoder entry와 document revalidation에서 다시 `Validate()`한다.

`ProfileSnapshotV1.Validate()`와 `Encode()`는 `Input.Compatibility`가 mismatch여도 구조적으로 허용하며 그대로 schema fields만 encode한다. `Compatibility`/`IsCurrentCompatible` 같은 derived metadata는 JSON property가 아니며 payload/final bytes에 절대 넣지 않는다. caller는 `snapshot.Input.Compatibility`로 recovery 필요를 별도 판단한다.

default snapshot/document와 reflection-bypass nested/default/bytes/hash는 public getter/Validate에서 `InvalidOperationException`으로 거부한다. document byte getters는 clone을 반환한다. snapshot nested structs는 immutable value copy다.

## exact canonical payload

Payload top-level property order는 아래와 같고 `integrity`는 없다.

1. `schemaVersion`
2. `profileRevision`
3. `settings`
4. `input`
5. `tutorial`
6. `progression`

Nested order:

- settings: `windowMode`, `masterVolumeQ1000`, `musicVolumeQ1000`, `sfxVolumeQ1000`, `gamepadAimInvertX`, `gamepadAimInvertY`
- input: `inputActionsAssetId`, `bindingSchemaVersion`, `bindingOverridesJson`
- tutorial: `confirmedIds`
- progression: `lastOfferedSeed`, `committedChoice`, `consentState`, `grantedSkill`, `completedBranches`

enum string은 schema table의 exact spelling만 쓴다. nullable seed/choice/skill은 lowercase `null`; integers는 invariant base-10 leading zero/plus/exponent 없는 ASCII; booleans은 lowercase다. arrays는 M5D3/M5D4에서 이미 검증한 order를 그대로 보존한다.

string property name과 value는 이미 surrogate-safe NFC여야 하며 outer encoder가 normalize/trim하지 않는다. quote/backslash와 U+0000..001F만 escape하고 short escape를 우선하며 나머지 control은 lowercase `\u00xx`; solidus와 다른 Unicode scalar는 escape하지 않는다. `bindingOverridesJson`에는 M5D5 `CanonicalText`를 하나의 outer JSON string value로 위 규칙에 따라 escape한다.

Payload는 UTF-8 without BOM, insignificant whitespace와 final newline 없이 정확히 한 top-level object다. clock, save time, path, active run/scene/runtime reference와 unknown field를 넣는 API가 없다.

## integrity와 final file bytes

`PayloadSha256`은 exact canonical payload bytes 전체의 SHA-256 lowercase 64 hex다. 보안 서명이나 authenticity claim이 아니다.

Final file은 payload의 마지막 `}` 직전에 exact `,"integrity":{"payloadSha256":"{hash}"}`를 추가한 뒤 닫는다. 따라서 top-level order는 payload six properties 뒤 `integrity`; integrity nested order는 `payloadSha256` 하나다. final bytes도 UTF-8 without BOM/newline/whitespace다.

`ProfileCanonicalDocumentV1.Validate()`는 nested snapshot을 검증하고 같은 encoder primitive로 payload/hash/final bytes를 다시 만들어 stored arrays/string과 byte-exact 비교한다. private marker나 hash length만 믿지 않는다. revalidation은 mutation-free이며 clock/culture에 독립적이다.

## 요구사항

- **REQ-M5D7A-001:** 네 Verified value core를 immutable `ProfileSnapshotV1`로 결합하고 schemaVersion 1과 progression source revision을 exact projection하며 M5D6 metadata mismatch를 recovery 전 candidate로 보존한다.
- **REQ-M5D7A-002:** 승인된 top-level/nested order, exact enum/null/integer/boolean/string/array 규칙으로 canonical payload UTF-8 bytes를 결정론적으로 생성한다.
- **REQ-M5D7A-003:** integrity를 제외한 payload bytes의 SHA-256 lowercase hex를 만들고 이를 마지막 integrity object에 넣은 canonical file bytes를 생성한다.
- **REQ-M5D7A-004:** default/bypass/mutated nested or document representation을 full deterministic revalidation으로 거부하고 byte arrays를 방어 복사한다.
- **REQ-M5D7A-005:** active run/runtime/wall-clock/path/unknown/derived compatibility field를 표현하지 않으며 revision 증가, parse/authentication, IO/save/load/recovery/Input apply 또는 mismatch candidate의 정상 commit 가능성을 수행하거나 주장하지 않는다.
- **REQ-M5D7A-006:** existing Profile API/asmdef/Packages/ProjectSettings/assets를 변경하지 않고 engine-free BCL `SHA256`/UTF8만 사용하며 no IO/clock/RNG/network/callback 경계를 지킨다.

## 수용 기준

- **AC-M5D7A-001:** explicit default profile와 두 committed progression branch 대표 snapshot이 exact schema/revision/nested values를 보존하고 revision `0`/`long.MaxValue`가 invariant base-10으로 encode되며 default/nested-invalid snapshot은 constructor/getter/Validate/Encode에서 거부된다.
- **AC-M5D7A-002:** empty/non-empty override, empty/multi tutorial IDs와 branches, null/non-null seed/choice/skill, 두 window mode/volume/boolean 대표를 포함한 golden payload가 exact property order와 byte로 일치한다. wrong asset ID와 schema `0`/`2`/`int.MaxValue` candidate도 exact mismatch fields를 encode하되 `Compatibility`/`IsCurrentCompatible` derived field는 bytes에 없다.
- **AC-M5D7A-003:** empty override와 non-empty inner JCS JSON, quote/backslash/control/solidus/BMP/supplementary NFC Unicode가 outer string golden과 일치하고 binding canonical JSON은 outer string으로 exact escape된다. culture 변경에도 bytes가 같다.
- **AC-M5D7A-004:** payload SHA-256은 independent test calculation과 일치하며 lowercase 64 hex이고 final file의 integrity 값과 field order가 exact다. payload에는 integrity가 없고 final에는 정확히 한 번만 있다. payload/file은 UTF-8 without BOM/final newline이다.
- **AC-M5D7A-005:** 같은 semantic snapshot을 다른 source collection instances로 만들면 payload/hash/file bytes가 exact 같고 repeated Encode/Validate가 idempotent하다.
- **AC-M5D7A-006:** returned payload/file arrays, source arrays와 reflection-bypass stored snapshot/hash/bytes를 변경해도 원본 document가 바뀌지 않거나 revalidation에서 거부된다.
- **AC-M5D7A-007:** static review가 only approved fields, no wall-clock/active-run/runtime/path/IO/parse/input-apply authority, engine-free boundary, fixed order와 existing ABI/asmdef/Packages/ProjectSettings/assets 무변경을 확인한다.
- **AC-M5D7A-008:** Luna independent review와 focused/full EditMode, full PlayMode가 failed/skipped/inconclusive `0`으로 통과한다. encoder PASS를 persisted file validity나 atomic save/load PASS로 확대하지 않는다.

## 정확한 구현 allowlist

Runtime:

- new `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalEncoder.cs` and `.meta`

Tests:

- new `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalEncoderTests.cs` and `.meta`

Documents:

- this contract
- later `docs/verification/2026-09-13-vd09-m5d7a-contract-pregate.md`
- later `docs/verification/2026-09-13-vd09-m5d7a-implementation-evidence.md`
- later `docs/verification/2026-09-13-vd09-m5d7a-luna-independent-review.md`
- `docs/README.md` minimum index/status link

M5D3~M5D6 source/tests, Profile asmdef, Packages, ProjectSettings, input asset/meta와 다른 runtime/test/assets는 변경 금지다.

## 순서와 중단 조건

Luna pre-gate → Astra Approved → Terra allowlist implementation → root Unity execution → Luna post-review → Astra integration 순서다. Decode/file IO/recovery, schema migration, actual Input apply, existing value API 변경 또는 allowlist 밖 변경이 필요하면 이 단위에서 중단한다.

## 롤백과 참여 기록

Rollback point는 M5D6 Verified다. 되돌리기는 승인 시 M5D7A 신규 파일과 문서/index hunk만 대상으로 하며 reset/clean/checkout/다른 dirty 파일 삭제를 사용하지 않는다.

Astra가 VD-09 canonical profile schema와 M5D3~M5D6 Verified API를 대조해 초안을 작성했다. Luna 1차 pre-gate의 mismatch-candidate P1을 recovery 전 canonical candidate 보존/정상 commit 비주장으로 보완했고 second pre-gate가 P0=0/P1=0으로 통과했다. Astra가 P2 golden/no-overclaim 지시를 포함해 Approved로 전환했다.
