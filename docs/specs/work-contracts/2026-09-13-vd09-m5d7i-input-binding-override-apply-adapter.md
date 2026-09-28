# VD-09 M5D7I Input System binding-override apply adapter

- Status: Verified
- Owner: Astra
- Implementer: Terra after Approved
- Independent reviewer: Luna
- Dependencies: M5D5, M5D6, and M5D7H Verified; M5B3/M5B5 input asset/router Verified
- Parent requirements: `REQ-PLAT-007`, `REQ-PLAT-009`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D7I-001` through `AC-M5D7I-009`
- Astra approval: 2026-09-13 after Luna R2 pre-gate PASS (P0=0, P1=0; residual P2=3 retained as implementation gates). This authorizes only the exact allowlist below and is not implementation verification.
- Astra verification: 2026-09-13 after focused PlayMode 8/8, full EditMode 647/647, full PlayMode 584/584, and Luna post-review PASS (P0=0, P1=0; residual P2=3). This verifies isolated candidate override-application detection only, not repair transformation, persistence, launch coordination, or gameplay entry.

## Purpose and boundary

M5D7I closes the Unity-facing detection half of VD-09 load-recovery rule 7. A structurally valid, metadata-current profile may still contain binding override JSON that Unity Input System cannot apply to the current authored action collection. This adapter applies a current `ProfileInputSnapshot` only to a caller-owned, fresh `GameInputActions` candidate and verifies the applied state by saving the candidate overrides back to JSON and comparing their M5D5 canonical meaning with the requested canonical text.

The adapter never mutates the live `InputRouter` action collection. On any ordinary apply or verification failure, the candidate is explicitly poisoned for discard and the caller receives a typed `RecoveryRequired` result. A later launch owner must dispose that candidate, create current defaults, plan input-only profile repair, persist it, notify once, and decide hub entry. M5D7I does none of those things and does not claim launch recovery completion.

## Approved internal API and closed result model

Inside `AcadeGameMaker.Input.Unity`:

```csharp
internal static ProfileBindingOverrideApplyResultV1 Apply(
    ProfileInputSnapshot input,
    GameInputActions candidate);
```

Exact internal enums:

- `ProfileBindingOverrideApplyOutcomeV1`: `DefaultsReady=1`, `OverridesApplied=2`, `RecoveryRequired=3`.
- `ProfileBindingOverrideApplyFailureV1`: `None=0`, `LoadFailed=1`, `RoundTripFailed=2`, `RoundTripMismatch=3`.
- `ProfileBindingOverrideCandidateDispositionV1`: `Retain=1`, `Discard=2`.

The immutable result exposes internal getters `Outcome`, `Failure`, `CandidateDisposition`, `RequestedCanonicalText`, `HasVerifiedCanonicalText`, `VerifiedCanonicalText`, and `Validate()`. The only legal rows are:

| Outcome | Failure | Disposition | Requested/verified canonical proof |
| --- | --- | --- | --- |
| `DefaultsReady` | `None` | `Retain` | both exact empty |
| `OverridesApplied` | `None` | `Retain` | both exact same non-empty M5D5 canonical text |
| `RecoveryRequired` | `LoadFailed` | `Discard` | requested non-empty; verified unavailable |
| `RecoveryRequired` | `RoundTripFailed` | `Discard` | requested non-empty; verified unavailable |
| `RecoveryRequired` | `RoundTripMismatch` | `Discard` | requested non-empty; verified valid but not equal |

`RequestedCanonicalText` is never null. `DefaultsReady` requires requested and verified exact empty; `OverridesApplied` requires equal non-empty requested and verified text; `RoundTripMismatch` requires non-empty requested plus a non-null valid M5D5 verified value (empty is allowed) that is ordinal-unequal. Only `LoadFailed` and `RoundTripFailed` require `_verifiedCanonicalText == null`, `HasVerifiedCanonicalText == false`, and make `VerifiedCanonicalText` throw `InvalidOperationException` after full result validation. All other legal rows have `HasVerifiedCanonicalText == true`. Default/unknown enums, null requested proof, null verified proof in any other row, noncanonical present proof, illegal empty/non-empty combinations, equality mismatches, or any row outside this table throw `InvalidOperationException` from `Validate()` and every getter. The result stores strings only, never the candidate, exception, callback, profile document, path, or mutable collection.

## Candidate preconditions and exact sequence

`input.Validate()` must pass and `input.Compatibility` must be exact `Current`; metadata mismatch is an `ArgumentException`, not an apply failure. `candidate` must be non-null, its asset and every map must be disabled, and `SaveBindingOverridesAsJson()` must report exact empty before any mutation. A null/disposed/malformed/already-overridden/enabled candidate is caller misuse and throws before `LoadBindingOverridesFromJson`; it is not converted to profile recovery.

For exact empty override sentinel:

1. Validate input and pristine disabled candidate.
2. Do not call `LoadBindingOverridesFromJson`.
3. Return `DefaultsReady/None/Retain` with exact empty proofs.

For non-empty override:

1. Retain the already-validated M5D5 canonical requested text.
2. Call Input System `LoadBindingOverridesFromJson(candidate, requested, removeExisting: true)` exactly once.
3. Call `SaveBindingOverridesAsJson(candidate)` exactly once after load. Together with the pristine-candidate preflight this makes exactly two save calls and one load call on a non-empty success/mismatch path.
4. Parse/canonicalize the non-empty round-trip through M5D5 and require exact ordinal equality with requested text. This detects ignored/stale binding IDs as well as semantic loss even when Input System only logs a warning.
5. Exact equality returns `OverridesApplied/None/Retain`. A nonfatal load exception returns `RecoveryRequired/LoadFailed/Discard`; a nonfatal save/parse/canonicalization exception returns `RoundTripFailed`; a valid unequal or empty round-trip returns `RoundTripMismatch`.

No retry, fallback apply, direct adapter call to `RemoveAllBindingOverrides`, second load, extra save, logging, or mutation of another action collection is allowed. The mandated pinned `LoadBindingOverridesFromJson(..., removeExisting:true)` internally calls `RemoveAllBindingOverrides`; that package-internal behavior is expected and is not a forbidden second cleanup. Exact production-port counts are: empty sentinel = one preflight save, zero load, zero post-load save; non-empty load failure = one preflight save and one load; non-empty post-load verification paths = one preflight save, one load, and one post-load save. There are no calls after an applicable failure.

After the load call begins, every recovery result records `Discard`; this is an enforceable handoff invariant on the result, not physical destruction of the caller object. The caller must dispose and never reuse that candidate. The adapter itself does not dispose caller-owned objects.

Only exceptions arising inside the two pinned Input System port operations are recoverable as profile-data application failures, and the set is closed: `ArgumentException`, `InvalidOperationException`, `FormatException`, `NullReferenceException`, `IndexOutOfRangeException`, and `UnityException`. The same closed set from the post-load save maps to `RoundTripFailed`. M5D5 round-trip parse/validation `ArgumentException` or `InvalidOperationException` also maps to `RoundTripFailed`. These catches surround only the named dependency calls; adapter validation/programmer errors outside them propagate. Every other exception, including `OutOfMemoryException`, `StackOverflowException`, and `AccessViolationException`, propagates and returns no result. A following fresh call must remain independent.

An instance-scoped internal deterministic port may wrap only `SaveBindingOverridesAsJson` and `LoadBindingOverridesFromJson`; it holds no static state and is supplied only through an internal overload. Tests use a fresh port per call to prove exact call order/count, exception classification, and no retry. Production must call the pinned Input System 1.20.0 APIs. The real generated `GameInputActions` collection and real Input System serialization round-trip are required in PlayMode coverage; injected tests alone are insufficient.

## Requirements

- **REQ-M5D7I-001:** accept only a validated metadata-current input snapshot and a fresh disabled default `GameInputActions` candidate.
- **REQ-M5D7I-002:** apply a non-empty override exactly once with `removeExisting:true`, then serialize and compare exact M5D5 canonical meaning.
- **REQ-M5D7I-003:** distinguish defaults-ready, verified applied, load failure, round-trip failure, and semantic mismatch with a closed immutable result matrix.
- **REQ-M5D7I-004:** any ordinary failure after load begins poisons only the caller-owned candidate for discard; no live router or second collection is touched.
- **REQ-M5D7I-005:** empty sentinel performs no load and retains pristine defaults; stale/unknown binding IDs cannot be accepted merely because Input System did not throw.
- **REQ-M5D7I-006:** no save/revision/recovery transform/file IO/path/quarantine/log/notification/router initialization/map enable/gameplay/clock/RNG/network authority is added.
- **REQ-M5D7I-007:** existing InputRouter behavior, generated action wrapper/asset, Profile ABI, package pins, and prior tests remain unchanged except the two exact asmdef reference additions required by this adapter and its tests.

## Acceptance criteria

- **AC-M5D7I-001:** real fresh generated actions plus empty sentinel return exact `DefaultsReady/None/Retain`; load count is zero, candidate remains disabled and saved overrides remain empty.
- **AC-M5D7I-002:** a real override produced from a known current binding, canonicalized through M5D5, applies to a second fresh generated collection and round-trips to exact canonical meaning with `OverridesApplied/None/Retain`.
- **AC-M5D7I-003:** valid Input System JSON containing unknown/stale binding ID does not throw but round-trips empty/unequal and returns `RecoveryRequired/RoundTripMismatch/Discard`.
- **AC-M5D7I-004:** every exception in the closed recoverable set is injected independently at load and post-load save; load failures return `LoadFailed`, post-load save and M5D5 parse/validation failures return `RoundTripFailed`. Empty has exact calls `Save`; load failure has `Save,Load`; post-load success/mismatch/failure has `Save,Load,Save`; no path retries or makes any additional call, and every recovery after load begins records discard.
- **AC-M5D7I-005:** metadata mismatch, default/reflection-invalid input, null/disposed/already-overridden/enabled candidate, invalid port presence, and preflight failure throw before load and do not masquerade as recovery.
- **AC-M5D7I-006:** every unexpected/fatal injected exception outside the closed set propagates; a following fresh valid call succeeds, proving no static poison or retained candidate state.
- **AC-M5D7I-007:** default/reflection-corrupted result and every illegal outcome/failure/disposition/requested/verified/availability combination fail from `Validate()` and all getters; every legal row is exercised, and unavailable `VerifiedCanonicalText` throws only in the two documented rows.
- **AC-M5D7I-008:** static review proves exact production call sequences, `removeExisting:true`, no direct adapter cleanup call, M5D5 ordinal semantic comparison, instance-only test port, no live-router reference/mutation, no forbidden authority, and unchanged generated input files/packages/project settings.
- **AC-M5D7I-009:** focused PlayMode, full EditMode, and full PlayMode have failed/skipped/inconclusive zero and Luna reports P0=0/P1=0. PASS means only isolated candidate apply detection, not repair planning, persistence, launch coordination, or gameplay entry.

## Exact allowlist

Runtime:

- new `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileBindingOverrideApplyAdapterV1.cs` and `.meta`
- modify only the `references` array of `Assets/AcadeGameMaker/Runtime/Input/Unity/AcadeGameMaker.Input.Unity.asmdef` to add `AcadeGameMaker.Profile`

Tests:

- new `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileBindingOverrideApplyAdapterV1Tests.cs` and `.meta`
- modify only the `references` array of `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/AcadeGameMaker.Input.Unity.PlayMode.Tests.asmdef` to add `AcadeGameMaker.Profile`

Documents:

- this contract, M5D7I pre-gate/implementation/review evidence, and one minimal `docs/README.md` entry

`InputRouter.cs`, `GameInputActions.cs`, `GameInput.inputactions(.meta)`, Profile runtime/tests, other asmdefs, Packages, ProjectSettings, scenes, prefabs, and all other assets are read-only. Luna pre-gate → Astra Approved → Terra implementation → Astra Unity runs → Luna post-review → Astra Verified integration.

## Stop and rollback conditions

Stop before implementation if the pinned Input System cannot provide `LoadBindingOverridesFromJson(..., true)` and `SaveBindingOverridesAsJson`, if correct detection requires live `InputRouter` mutation, or if a dependency/package/asset/generated-wrapper change is required. Rollback is removal of the new adapter/tests/docs and the two exact asmdef reference additions only; never reset/clean/checkout unrelated dirty work.

## Participation record

Astra derived this contract from VD-09 rule 7, the Verified M5D5/M5D6 boundaries, pinned Input System 1.20.0 source, and the current M5B3/M5B5 action/router lifecycle. The pinned implementation returns empty for no overrides and may only warn for unknown binding IDs, so post-apply canonical round-trip comparison is a required correctness gate rather than optional diagnostics.
