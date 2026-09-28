# VD-09 M5D7J binding-apply-failure recovery transform

- Status: Verified
- Owner: Astra
- Implementer: Terra after Approved
- Independent reviewer: Luna
- Dependencies: M5D7B, M5D7C, M5D7E, and M5D7I Verified
- Parent requirements: `REQ-PLAT-008`, `REQ-PLAT-009`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D7J-001` through `AC-M5D7J-008`
- Astra approval: 2026-09-13 after Luna pre-gate PASS (P0=0, P1=0; residual P2=3 retained as implementation and later-coordinator gates). This authorizes only the exact allowlist below and is not implementation verification.
- Astra verification: 2026-09-13 after focused M5D7J 5/5, unchanged M5D7C 10/10, full EditMode 652/652, full PlayMode 584/584, and Luna post-review PASS (P0=0, P1=0; residual P2=3). This verifies transformation only, not authentication of the apply failure, persistence, or launch sequencing.

## Purpose and boundary

M5D7J adds the engine-free transformation needed after M5D7I reports that a metadata-current, non-empty binding override could not be applied or verified. It preserves settings, tutorial, and progression semantics; replaces the entire input block with the approved current empty default; and produces the exact recovery revision/reason for either a selected primary or selected previous source.

This planner cannot observe or authenticate an Input System failure. Calling one of its explicit methods is the later launch coordinator's assertion that M5D7I returned `RecoveryRequired` for the matching selected source. It does not reference Unity, M5D7I types, candidate objects, file roles, or paths and does not apply bindings, select files, quarantine, save, log, notify, or enter gameplay.

## Approved additive public API

Add two methods to existing `ProfileRecoveryPlannerV1`:

```csharp
public static ProfileRecoveryPlanV1 PlanPrimaryBindingApplyFailure(
    ProfileCanonicalDecodeResultV1 source);

public static ProfileRecoveryPlanV1 PlanPreviousBindingApplyFailure(
    ProfileCanonicalDecodeResultV1 source);
```

Both accept only a fully revalidated `ValidCurrentInput` decode result whose input override is non-empty. Empty current input is inapplicable because M5D7I returns `DefaultsReady`; metadata/binding recovery classifications use existing M5D7C paths; invalid/unsupported/default/reflection-bypassed values are rejected.

Primary failure produces source revision `r`, result `r+1`, exact reason `InputRepair`, and current empty input. Previous failure produces source `r`, result `r+1`, exact reasons `PreviousPromotion|InputRepair`, and current empty input. The previous path does not first create a promotion at `r+1` and then increment to `r+2`; it is one combined transformation from the original previous revision.

## Structural proof and additive compatibility

Existing `ProfileRecoveryPlanV1` remains the result type and no enum numeric value changes. Add distinct private proof kinds for primary-current apply failure and previous-current apply failure. Each new plan retains the original current non-empty `ProfileInputSnapshot` together with the existing immutable settings/tutorial/progression proof. `Validate()` must prove:

- source `r >= 0`, `r != long.MaxValue`, result exact `r+1`;
- proof input is fully valid, exact metadata-current, and non-empty;
- result input is exact current ID/current schema/empty sentinel;
- non-input semantics are equal to the source proof and result progression differs only by revision;
- primary proof kind pairs only with exact `InputRepair`; previous proof kind pairs only with exact `PreviousPromotion|InputRepair`.

Default/unknown/mutated proof kind, reason, revision, proof input, preserved state, or result document fails closed from `Validate()` and every public getter. This is structural consistency, not authentication that M5D7I actually ran.

The change is additive to Verified M5D7C. Existing public methods, enum values, existing four plan kinds/result rows, exception types, outputs, and source/test behavior remain unchanged. The unchanged M5D7C focused suite must pass alongside a new focused M5D7J fixture.

## Error boundary

- A well-formed but inapplicable source—current empty input, metadata recovery, binding recovery, invalid, or unsupported—throws `ArgumentException` before output.
- Default/reflection-invalid decode result is wrapped as `ArgumentException` with the original `InvalidOperationException` as inner exception, matching M5D7C.
- Applicable source revision `long.MaxValue` throws `InvalidOperationException` without wrap, overflow, mutation, or fallback.
- Failure does not retain mutable state or poison later calls.

## Requirements

- **REQ-M5D7J-001:** transform only fully valid current/non-empty decoded input after caller-asserted M5D7I recovery need.
- **REQ-M5D7J-002:** primary apply failure preserves all non-input semantics, resets input, and produces exact `r→r+1` plus `InputRepair`.
- **REQ-M5D7J-003:** previous apply failure performs one combined transform with exact `r→r+1` plus `PreviousPromotion|InputRepair`, never `r+2`.
- **REQ-M5D7J-004:** new proof kinds retain and revalidate original current non-empty input and all preserved semantics while output input is exact current default.
- **REQ-M5D7J-005:** invalid/inapplicable/overflow/reflection misuse fails with the closed M5D7C-compatible exception boundary.
- **REQ-M5D7J-006:** no Unity/Input System/apply-result/file/path/IO/selection/quarantine/save/log/notification/clock/RNG/network/callback authority is added.
- **REQ-M5D7J-007:** Verified M5D7C API, enum values, four existing plan kinds, behavior, tests, and dependent profile ABI remain unchanged.

## Acceptance criteria

- **AC-M5D7J-001:** current non-empty primary source with representative settings/tutorial/progression becomes exact current-empty input at `r+1`, reason `InputRepair`, while all non-input semantics are preserved and defensively independent.
- **AC-M5D7J-002:** the same previous source becomes exact current-empty input at `r+1`, reasons `PreviousPromotion|InputRepair`; a comparison test proves it is not `r+2` and not the existing input-preserving previous promotion.
- **AC-M5D7J-003:** source revisions `0`, arbitrary positive, and `long.MaxValue-1` increment exactly in both entries; `long.MaxValue` throws before a plan is returned and a following valid call succeeds.
- **AC-M5D7J-004:** current empty, metadata recovery, binding recovery, invalid, unsupported, default, and reflection-invalid sources follow the exact error boundary in both entries.
- **AC-M5D7J-005:** every new legal plan row passes all getters/`Validate`; mutation of reason, revision, proof kind, original input including empty/mismatched/default, preserved state, and result input/document fails closed.
- **AC-M5D7J-006:** unchanged M5D7C focused tests prove existing default, decode-time input repair, current previous promotion, and previous+decode-repair outputs/exceptions remain exact; public enum numeric values are unchanged.
- **AC-M5D7J-007:** static review proves only two additive public methods and private proof branches, no M5D7I/Unity/file/path/save authority, no existing API/enum alteration, and exact REQ traceability.
- **AC-M5D7J-008:** M5D7J focused EditMode, unchanged M5D7C focused EditMode, full EditMode, and full PlayMode have failed/skipped/inconclusive zero, and Luna reports P0=0/P1=0. PASS is transformation only, not proof of apply failure, persistence, or launch recovery.

## Exact allowlist

Runtime:

- modify only `Assets/AcadeGameMaker/Runtime/Profile/ProfileRecoveryPlannerV1.cs`

Tests:

- new `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileBindingApplyFailureRecoveryPlannerV1Tests.cs` and `.meta`

Documents:

- this contract, M5D7J pre-gate/implementation/review evidence, and one minimal `docs/README.md` entry

Existing `ProfileRecoveryPlannerV1Tests.cs`, all other Profile/Input runtime and tests, asmdefs, Packages, ProjectSettings, scenes, prefabs, and assets remain read-only. Luna pre-gate → Astra Approved → Terra implementation → Astra Unity runs → Luna post-review → Astra Verified integration.

## Stop and rollback conditions

Stop if a correct combined previous transformation requires changing the existing public reason enum values or M5D7E selection semantics, or if Unity/M5D7I references are required in the Profile assembly. Rollback is the two additive runtime entry/proof branches plus the new fixture/docs only; never reset/clean/checkout unrelated dirty work.

## Participation record

Astra derived the two transforms from VD-09 rule 7 and the Verified M5D7E selection matrix. The explicit dual entry prevents the previous-source path from accidentally double-incrementing after its promotion plan and keeps actual Input System evidence outside the engine-free Profile assembly.
