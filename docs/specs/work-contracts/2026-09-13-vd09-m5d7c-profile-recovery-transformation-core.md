---
status: Verified
---

# VD-09 M5D7C profile recovery transformation core

- Date: 2026-09-13
- Owners: Astra approval; Terra implementation; Luna independent pre/post review
- Dependencies: M5D3-M5D7B Verified
- Requirements: `REQ-PLAT-008`, `REQ-PLAT-010`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D7C-001` through `AC-M5D7C-008`
- Astra approval: 2026-09-13 after Luna second pre-gate PASS (P0=0, P1=0); exact allowlist only.

## 목적과 승인 기본값

M5D7C는 file selection/IO 이전의 engine-free recovery state transformation만 소유한다. 사용자가 2026-09-13 승인한 신규 profile 기본값은 다음과 같다.

- settings: `BorderlessFullscreen`, master `1000`, music `700`, sfx `800`, aim invert X/Y `false`
- input: current asset ID, binding schema `1`, empty override sentinel
- tutorial confirmed IDs: empty
- progression: result revision `0`, null seed/choice/skill, `RefusesOwnershipTransfer`, empty completed branches
- default는 committed source가 아니며 source revision은 `-1`; first result만 revision `0`이다.

## 승인 API와 변환

새 engine-free runtime file은 다음 public immutable API를 제공한다.

- flags `ProfileRecoveryPlanReason`: `DefaultBootstrap=1`, `PreviousPromotion=2`, `InputRepair=4`.
- `ProfileRecoveryPlanV1` getters: `long SourceRevision`, `long ResultRevision`, `ProfileRecoveryPlanReason Reasons`, `ProfileCanonicalDocumentV1 ResultDocument`, `Validate()`.
- `ProfileRecoveryPlannerV1.PlanDefaultBootstrap()`.
- `ProfileRecoveryPlannerV1.PlanInputRepair(ProfileCanonicalDecodeResultV1 source)`.
- `ProfileRecoveryPlannerV1.PlanPreviousPromotion(ProfileCanonicalDecodeResultV1 source)`.

Default plan은 source `-1`, result `0`, reason exact `DefaultBootstrap`이다.

Input repair는 M5D7B metadata/binding recovery classification만 받는다. settings/tutorial/progression semantic values를 보존하고 input 전체를 승인 current defaults로 교체하며 result revision은 source `r+1`, reason exact `InputRepair`다. 이 engine-free method는 source가 primary file에서 왔다고 주장하거나 증명하지 않는다. 후속 file-selection owner가 실제 primary를 선택한 뒤 이 transform을 호출한다.

Previous promotion은 current, metadata-recovery 또는 binding-recovery decode result를 받는다. Current source는 모든 semantic state와 canonical binding override를 보존하고 reason `PreviousPromotion`; input recovery source는 비입력 state만 보존하고 current default input으로 교체해 reason `PreviousPromotion|InputRepair`다. 모두 result revision `r+1`이다. 이 method도 disk provenance를 검사하지 않으며, 후속 selector가 previous candidate를 선택했다는 typed orchestration precondition 아래 호출한다. M5D7C는 두 명시적 transform을 제공할 뿐 primary/previous 선택 PASS를 주장하지 않는다.

모든 result document는 M5D7A encoder로 생성되고 `Input.IsCurrentCompatible == true`여야 한다. Decoder result와 nested values는 entry 및 plan revalidation에서 다시 검증한다. Source revision `long.MaxValue`는 overflow/wrap/default fallback 없이 `InvalidOperationException`; well-formed이지만 method에 허용되지 않는 source classification은 `ArgumentException`이다. default/reflection-invalid source result는 그 `InvalidOperationException`을 inner exception으로 보존한 `ArgumentException`으로 entry에서 변환한다. 생성된 plan의 getter/`Validate()`에서 발견되는 default/reflection-invalid plan은 `InvalidOperationException`이다. 입력 source를 변경하지 않는다.

Plan `Validate()`의 허용 reason은 exact `{DefaultBootstrap, InputRepair, PreviousPromotion, PreviousPromotion|InputRepair}` 네 조합뿐이다. 다른 단일/복합/unknown/zero flags는 거부한다. source/result exact 관계, default exact fields, current-compatible document, document revision, recovery별 state preservation을 재검증한다. 이를 위해 plan은 source semantic values와 transform kind의 immutable proof state를 private하게 보존하되 source byte array/parse tree/IO handle을 보존하지 않는다. default/reflection-bypass/unknown flags/mismatched document/source/revision은 `InvalidOperationException`이다.

## 요구사항

- **REQ-M5D7C-001:** 승인 default를 source `-1`에서 exact revision `0` current-compatible canonical document로 만든다.
- **REQ-M5D7C-002:** metadata/binding recovery transform에서 비입력 state를 보존하고 input 전체만 current defaults로 바꾸며 revision을 정확히 1 증가시킨다. Disk primary provenance는 후속 selector 책임이다.
- **REQ-M5D7C-003:** previous current source는 전체 state를 보존하고, previous input-recovery source는 비입력 state만 보존해 정확한 reason과 revision으로 승격한다.
- **REQ-M5D7C-004:** result document와 private proof를 full revalidation하고 default/reflection/overflow/classification misuse를 fail closed한다.
- **REQ-M5D7C-005:** no file/path/IO/selection/quarantine/save notification/input apply/clock/RNG/network/callback authority와 existing API/asmdef/settings/assets 무변경을 지킨다.

## 수용 기준

- **AC-M5D7C-001:** default plan의 모든 승인 값, source `-1`, result `0`, reason, canonical bytes/hash가 exact이며 반복 호출이 동일하다.
- **AC-M5D7C-002:** asset/schema/both metadata mismatch decode sources와 malformed/noncanonical binding sources에 `PlanInputRepair`를 적용하면 settings/tutorial/progression을 값으로 보존하고 current empty input, source `r`, result `r+1`, exact input-repair reason을 만든다. 이 AC는 primary file 선택을 검증하지 않는다.
- **AC-M5D7C-003:** valid current previous source는 non-empty canonical override를 포함한 모든 state를 보존하고 `PreviousPromotion`; metadata/binding recovery previous는 current empty input과 combined reasons를 만든다.
- **AC-M5D7C-004:** source revision `0`, arbitrary positive, `long.MaxValue-1`은 exact 증가하며 `long.MaxValue`는 모든 applicable entry에서 mutation 없이 거부된다.
- **AC-M5D7C-005:** `PlanInputRepair`의 current/invalid/unsupported 및 `PlanPreviousPromotion`의 invalid/unsupported well-formed classification misuse는 `ArgumentException`; default/reflection-invalid source도 inner `InvalidOperationException`을 가진 `ArgumentException`; `long.MaxValue` valid source는 `InvalidOperationException`이다.
- **AC-M5D7C-006:** result input은 항상 exact current ID/schema/empty override이고 current-compatible이다. non-input arrays are value-equal and defensively independent.
- **AC-M5D7C-007:** 허용 reason 네 조합 외 zero/unknown/다른 복합 flags, default/reflection-bypassed plan/result document/revision/preserved-state mismatch는 plan getters/Validate에서 `InvalidOperationException`으로 거부된다. Invalid source entry behavior는 AC-005를 따른다.
- **AC-M5D7C-008:** focused/full EditMode와 full PlayMode가 fail/skip/inconclusive 0, Luna P0/P1 0이며 persistence/recovery execution PASS로 과장하지 않는다.

## 정확한 allowlist

- `Assets/AcadeGameMaker/Runtime/Profile/ProfileRecoveryPlannerV1.cs` + `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileRecoveryPlannerV1Tests.cs` + `.meta`
- this contract
- `docs/verification/2026-09-13-vd09-m5d7c-contract-pregate.md`
- `docs/verification/2026-09-13-vd09-m5d7c-implementation-evidence.md`
- `docs/verification/2026-09-13-vd09-m5d7c-luna-independent-review.md`
- minimal `docs/README.md` entry

Existing M5D3-M5D7B source/tests, asmdefs, Packages, ProjectSettings, scenes, prefabs, input/actions and other assets are read-only and forbidden changes. Luna pre-gate → Astra Approved → Terra → root tests → Luna post-review → Astra Verified 순서다.
