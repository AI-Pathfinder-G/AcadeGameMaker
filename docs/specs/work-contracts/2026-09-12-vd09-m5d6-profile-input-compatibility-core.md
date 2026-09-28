---
status: Verified
---

# VD-09 M5D6 프로필 입력 블록·호환성 코어

- Date: 2026-09-12
- Owning Approved specs: [VD-07](../vertical-demo/07-input-ui-and-feedback.md), [VD-09](../vertical-demo/09-platform-and-quality.md), [SYSTEM-CONTRACTS](../vertical-demo/SYSTEM-CONTRACTS.md)
- Decision basis: ADR-0010, ADR-0018, ADR-0031; [M5B3 device action asset](./2026-09-08-vd07-m5b3-device-action-asset.md)
- Dependency: [M5D5 Verified](./2026-09-12-vd09-m5d5-binding-overrides-jcs-core.md)
- Assigned by / final authority: Astra
- Implementation: Terra; independent pre/post verification: Luna
- Requirements: `REQ-PLAT-006`, `REQ-PLAT-007`, `REQ-PLAT-008`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D6-001` through `AC-M5D6-006`; AC-PLAT-009 is only partially exercised.
- Astra approval: 2026-09-12 after Luna pre-gate PASS (P0=0, P1=0). Only the exact allowlist below is authorized; this is not implementation verification.
- Verified: 2026-09-12 — Luna independent implementation review PASS (P0=0, P1=0; two non-blocking coverage P2) and Astra accepts this bounded integration. See [implementation evidence](../../verification/2026-09-12-vd09-m5d6-implementation-evidence.md) and [Luna review](../../verification/2026-09-12-vd09-m5d6-luna-independent-review.md). This verifies metadata compatibility only, not actual binding apply or persisted recovery.

## 목적과 경계

VD-09는 input asset ID 또는 binding schema가 현재 값과 다르거나 override 적용이 실패해도 settings/tutorial/progression을 보존하고 input block만 복구하도록 요구한다. 따라서 outer profile parser는 mismatch를 파일 전체 손상으로 오분류해서는 안 된다. M5D6는 input block의 구조 유효성과 현재 authoring asset에 대한 metadata compatibility를 분리한 engine-free immutable 코어를 만든다.

M5D6는 override JSON의 문법/canonicality를 M5D5 value로 위임한다. 실제 Input System 적용 성공/실패, input 기본값 교체, revision 증가와 atomic save는 후속 Unity/load owner 책임이다.

## 현재 입력 identity

- `CurrentInputActionsAssetId = "7c8d9e0f1a2b4c3d8e9f0a1b2c3d4e5f"`: 승인된 `Assets/GameInput.inputactions.meta` GUID.
- `CurrentBindingSchemaVersion = 1`.

두 값은 새 `ProfileInputContract` public static class의 `public const`로 고정한다. M5D6는 asset/meta 파일을 읽지 않으며 authoring evidence와 상수를 test에서 대조한다. 향후 asset identity나 schema를 바꾸는 작업은 새 Approved 계약과 recovery test 없이 상수만 바꿀 수 없다.

## 승인 API

- `ProfileInputSnapshot(string inputActionsAssetId, int bindingSchemaVersion, ProfileBindingOverridesJson bindingOverrides)` readonly struct.
- public getters `InputActionsAssetId`, `BindingSchemaVersion`, `BindingOverrides`, `Compatibility`, `IsCurrentCompatible`와 `public void Validate()`.
- `[Flags] ProfileInputCompatibility`: `Current=0`, `AssetIdMismatch=1`, `BindingSchemaMismatch=2`; both는 bitwise `3`.

`inputActionsAssetId`는 null/empty가 아니며 unpaired surrogate가 없고 already-NFC여야 한다. parent가 GUID 문법을 schema에 강제하지 않았으므로 M5D6는 arbitrary non-empty authored stable ID를 구조적으로 허용한다. `bindingSchemaVersion`은 JSON integer를 C# `int`로 표현하며 `>=0`을 구조적으로 허용한다. 현재 `1`이 아닌 값은 whole-profile corruption이 아니라 `BindingSchemaMismatch`다. int 범위 밖/negative는 후속 parser에서 structural invalid다. 이 부분 복구 해석은 **input block의 `bindingSchemaVersion`에만** 적용된다. outer profile `schemaVersion != 1`은 VD-09대로 전체 unsupported profile이며 M5D6가 보존·migration하지 않는다.

`bindingOverrides`는 public getter/constructor에서 `Validate()`를 통과해야 한다. empty sentinel과 valid non-empty canonical JSON은 모두 구조적으로 합법이다. default M5D5 value를 empty로 보정하지 않는다.

`Compatibility`는 exact ordinal asset ID 비교와 schema integer 비교를 독립 bit로 계산한다. 둘 다 다르면 `3`; 둘 다 같으면 `Current`. 이 값은 override 적용 가능성을 뜻하지 않는다. `IsCurrentCompatible`은 metadata bits가 `Current`임만 뜻하며, load owner는 이후 실제 override 적용 실패를 별도 원인으로 처리해야 한다.

## 불변성과 재검증

- constructor는 모든 인자를 publication 전에 검증하고 보정/trim/normalize하지 않는다.
- 모든 public getter와 compatibility 계산은 전체 snapshot `Validate()`를 먼저 수행한다.
- default struct는 null asset ID와 default override 때문에 invalid며 getter가 default mismatch 값으로 숨기지 않는다.
- reflection-bypass null/empty/decomposed/unpaired ID, negative version, default/malformed override는 public consumption에서 `InvalidOperationException`이다.
- clock/RNG/IO/Unity/Input System callback/mutable static state가 없다.

## 요구사항

- **REQ-M5D6-001:** input block은 non-empty surrogate-safe NFC stable ID, nonnegative int schema version과 validated M5D5 override value를 immutable하게 보존한다.
- **REQ-M5D6-002:** 현재 asset GUID와 schema 1을 승인 상수로 고정하고 ordinal ID/schema mismatch를 독립 flags로 분류한다.
- **REQ-M5D6-003:** metadata mismatch는 structural corruption이나 override-apply failure로 재분류하지 않으며 input-only recovery를 후속 owner에 맡긴다.
- **REQ-M5D6-004:** default/bypass/nested-invalid 값을 public full revalidation으로 거부하고 empty override sentinel을 default와 구분한다.
- **REQ-M5D6-005:** actual binding apply/default replacement/revision/save/load/JSON outer codec/hash/source authentication을 수행하거나 주장하지 않는다.
- **REQ-M5D6-006:** 기존 Profile API/asmdef/Packages/ProjectSettings/assets를 변경하지 않고 engine-free/no IO·crypto·clock·RNG·network·callback 경계를 유지한다.

## 수용 기준

- **AC-M5D6-001:** current ID/version과 empty/non-empty M5D5 overrides가 exact 보존되고 null/empty/decomposed/unpaired ID, negative version, default override는 constructor에서 거부된다.
- **AC-M5D6-002:** current/current=`0`, wrong-ID/current=`1`, current/wrong-schema=`2`, both-wrong=`3`이 exact이고 comparison은 ordinal/culture-independent다.
- **AC-M5D6-003:** schema `0`, `2`, `int.MaxValue`는 구조적으로 보존되면서 mismatch이고, current version `1`만 metadata-compatible이다. 이는 override 적용 성공을 주장하지 않는다.
- **AC-M5D6-004:** default snapshot과 reflection-bypass ID/version/override의 `Validate` 및 모든 getter가 throw하고 실패 뒤 별도 valid 생성은 정상이다.
- **AC-M5D6-005:** static/authoring test가 두 current 상수를 approved M5B3 asset meta GUID와 schema version에 대조하고 no-engine/no authority 및 allowlist를 확인한다.
- **AC-M5D6-006:** Luna independent review와 focused/full EditMode, full PlayMode가 failed/skipped/inconclusive `0`으로 통과한다. 결과를 actual binding application이나 persisted profile recovery PASS로 확대하지 않는다.

## 정확한 구현 allowlist

Runtime:

- new `Assets/AcadeGameMaker/Runtime/Profile/ProfileInputSnapshot.cs` and `.meta`

Tests:

- new `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileInputSnapshotTests.cs` and `.meta`

Documents:

- this contract
- later `docs/verification/2026-09-12-vd09-m5d6-contract-pregate.md`
- later `docs/verification/2026-09-12-vd09-m5d6-implementation-evidence.md`
- later `docs/verification/2026-09-12-vd09-m5d6-luna-independent-review.md`
- `docs/README.md` minimum index/status link

기존 M5D3/M5D4/M5D5 source/tests/asmdef, `Assets/GameInput.inputactions(.meta)`, Packages, ProjectSettings와 다른 runtime/test/assets 변경은 금지한다.

## 순서와 중단 조건

Luna pre-gate → Astra Approved → Terra allowlist implementation → root Unity execution → Luna post-review → Astra integration 순서다. stable ID 형식 제한, schema migration, 실제 override apply/default replacement, outer codec/IO가 필요하면 이 단위에 넣지 않고 후속 계약으로 반환한다.

## 롤백과 참여 기록

Rollback point는 M5D5 Verified다. 되돌리기는 승인 시 M5D6 신규 파일과 문서/index hunk만 대상으로 하며 reset/clean/checkout/다른 dirty 파일 삭제를 사용하지 않는다.

Astra가 VD-09 partial input recovery와 M5B3 approved asset identity를 대조해 초안을 작성했다. Luna pre-gate는 P0=0/P1=0으로 통과했고 outer schema와 binding schema의 구분, exact GUID/culture/schema/default/getter matrix를 P2로 남겼다. Astra가 구분 문구와 검증 지시를 반영해 Approved로 전환했다.
