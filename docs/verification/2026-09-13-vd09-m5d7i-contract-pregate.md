# VD-09 M5D7I input-binding-override apply adapter — Luna independent contract pre-gate

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7I input-binding-override apply adapter](../specs/work-contracts/2026-09-13-vd09-m5d7i-input-binding-override-apply-adapter.md)
- Compared against: Verified M5D5 binding JSON canonicalization, M5D6 input compatibility, M5D7H recovery-save boundary, Verified M5B3 generated action asset and M5B5 `InputRouter`, VD-07/VD-09 authority rules, AGENTS.md, the checked-in generated `GameInputActions`, and pinned `com.unity.inputsystem` 1.20.0 source.
- Scope: contract-only independent pre-gate. No contract, runtime, test, asset, asmdef, README, or Unity files were modified; no Unity test was run.

## Verdict

**CONDITIONAL FAIL — P0=0, P1=3, P2=3. Do not advance to Approved until the three P1 findings are amended.**

The boundary is otherwise appropriately narrow. The pinned package provides both required extension methods for generated wrappers. `SaveBindingOverridesAsJson` is safe on a disabled generated collection, and `LoadBindingOverridesFromJson(..., removeExisting: true)` removes existing overrides and then applies the JSON. The package logs a warning and continues when an ID is unknown, so the required M5D5 canonical round-trip comparison is the correct detection gate. The adapter can remain in `AcadeGameMaker.Input.Unity`, add only a Profile reference, and avoid `InputRouter`/live-map authority entirely.

## P1 findings

### P1-001 — The result matrix contradicts its representation of unavailable verification proof

The two failure rows `RecoveryRequired/LoadFailed/Discard` and `RecoveryRequired/RoundTripFailed/Discard` require the verified proof to be “unavailable.” The next paragraph says “null proof” is illegal for every result. No other proof-availability value or field is defined. A load failure occurs before a verified JSON exists; a save/parse failure may likewise have no valid canonical verified text. Therefore an implementation must either return null (contradicting the validation rule), invent an empty sentinel (which collides with the valid empty round-trip and `RoundTripMismatch` semantics), or add an undocumented field.

Required amendment: define the exact internal result fields/getters and state that `RequestedCanonicalText` is always non-null, canonical, and non-empty for every recovery row; `VerifiedCanonicalText` is non-null and canonical only for `OverridesApplied` and `RoundTripMismatch`, and is null only for `LoadFailed`/`RoundTripFailed`. Scope the null-proof rejection accordingly, and require every getter plus `Validate()` to enforce this closed rule. Add reflection/default tests for each permitted unavailable-proof row and for null in every forbidden row.

### P1-002 — Required sequence/call counts conflict with AC-M5D7I-004, and the direct-removal prohibition conflicts with the mandated package call

The precondition requires one `SaveBindingOverridesAsJson()` call before mutation (line 47), while the non-empty sequence requires another save after load (lines 57–60). Thus a non-empty successful or failed apply necessarily has two Save invocations and one Load invocation. AC-M5D7I-004 instead says every path calls “load/save at most once,” which cannot satisfy the preflight plus post-load sequence. The deterministic port is also described as wrapping “only the two Input System calls,” without saying whether it counts call types or total invocations.

Separately, the pinned 1.20.0 implementation of the mandated `LoadBindingOverridesFromJson(..., removeExisting: true)` calls `RemoveAllBindingOverrides()` internally before parsing. The draft says no `RemoveAllBindingOverrides` is allowed. A literal reading makes the required package call impossible, even though the adapter itself need not call that method directly.

Required amendment: make the accounting explicit: empty input = one preflight Save, zero Load; non-empty input = one preflight Save, exactly one Load with `removeExisting:true`, and exactly one post-load Save (two Save invocations total), with no additional invocation or retry. Define the deterministic port as wrapping those two call types and all three permitted invocation points. Clarify that the adapter must not invoke `RemoveAllBindingOverrides` directly or perform extra cleanup; the package-internal removal caused by the mandated Load call is expected and allowed.

### P1-003 — “Nonfatal” exception classification is not closed

The draft maps a “nonfatal load exception” to `LoadFailed` and a “nonfatal save/parse/canonicalization exception” to `RoundTripFailed`, while naming only `OutOfMemoryException`, `StackOverflowException`, and `AccessViolationException` as fatal. This does not define whether `ArgumentException`, `InvalidOperationException`, `JsonException`/`FormatException`, `NullReferenceException`, `MissingReferenceException`, or other runtime/programmer exceptions are recoverable at each seam. A broad `catch (Exception)` with three exclusions would swallow malformed-candidate/programmer defects and violate the fail-closed boundary; a narrow filter cannot be implemented deterministically without an allowlist.

Required amendment: define the exact recoverable exception set per seam (or a typed port failure discriminant with an explicit production exception mapping), and state that all unexpected/programmer exceptions escape. Keep the three fatal exceptions unconditionally propagating. Preflight misuse exceptions must escape before Load; only exceptions from the mandated Load or post-load Save/parse path may produce typed recovery rows. Add injected tests for every allowlisted recoverable type and at least one unexpected exception proving it escapes with the candidate disposition/lease state documented.

## P2 findings and precision notes

### P2-001 — Fresh/caller-owned candidate origin is a precondition, not an enforceable identity proof

`GameInputActions` has a public constructor and a get-only `asset`; its generated constructor embeds the authored JSON and initially disables both maps. The draft can fail closed for null-like/disposed, enabled, or already-overridden candidates by checking the asset/maps and preflight Save. It cannot authenticate historical origin or distinguish a caller-passed live router collection if a caller deliberately bypasses encapsulation/reflection. That is acceptable for this internal candidate-owned boundary, but the contract should say explicitly that “fresh generated” is a caller proof and that the adapter does not claim asset provenance. Do not duplicate binding IDs or add a second binding authority merely to make this unit prove origin.

### P2-002 — The deterministic test port needs a lifecycle/reset rule

The API has no port parameter, yet AC-M5D7I-004/005 require injected load/save failures and invalid-port-presence checks. An internal seam in the new adapter file is feasible, but the draft should specify that it is test-only, cannot be set on the production path, is reset in `finally`, cannot retain a candidate/delegate after a call, and is not a second Input System or router authority. The focused tests should prove a failed/fatal call does not leave the seam installed.

### P2-003 — “Poisoned for discard” is a typed disposition, not an enforced candidate lifecycle

The adapter cannot mark a `GameInputActions` instance unusable through the approved API without adding state to the generated wrapper. After a load has begun, a failure may leave partial overrides in the candidate; the result can report `Discard`, but only the caller can dispose it. Clarify that `Discard` is the adapter's proof/ownership handoff, that it performs no cleanup or disposal, and that the caller must dispose even when a fatal exception escapes. This preserves the no-second-collection and no-live-router boundary.

## Dependency, API, and authority review

- `GameInputActions` implements `IInputActionCollection2`; the pinned signatures are `SaveBindingOverridesAsJson(this IInputActionCollection2)` and `LoadBindingOverridesFromJson(this IInputActionCollection2, string, bool)`. The generated wrapper is explicitly supported by package documentation. Both maps and the asset expose enabled state, so disabled-candidate preflight is implementable.
- M5D5 is the correct comparison authority: retain `input.BindingOverrides.CanonicalText`, parse/canonicalize the post-save text through M5D5, and compare with ordinal equality. Empty or unequal output must become `RoundTripMismatch`; malformed/noncanonical post-save output must become `RoundTripFailed`. Unknown IDs are warning-only in the package and are therefore caught by this comparison.
- M5D6 supplies only metadata/current-compatibility validation. M5D7H owns persistence recovery and must not be called. The adapter adds no save/revision/IO/path/quarantine/recovery-plan authority.
- M5B5 remains the sole live `InputRouter` map/callback owner. The adapter receives a caller-owned disabled candidate, never references the router, never enables a map, and does not touch the router's action collection. The proposed Unity asmdef edge `Input.Unity → Profile` does not create a cycle because `Profile` is engine-free and has no Input reference. The test asmdef edge is similarly feasible.
- The package's internal `RemoveAllBindingOverrides` call is unavoidable under `removeExisting:true`; only an additional direct call/cleanup is out of scope.

## Pre-gate AC status

| AC | Status | Independent basis |
|---|---|---|
| AC-M5D7I-001 | **CONDITIONAL — P1-001/P1-002** | Empty sentinel and disabled real wrapper are feasible, but result proof and exact preflight call count need closure. |
| AC-M5D7I-002 | **CONDITIONAL — P1-001/P1-002** | Real known-binding round-trip is package-feasible and M5D5 comparison is correct; sequence accounting and retained proof are ambiguous. |
| AC-M5D7I-003 | **PASS by algorithm, implementation pending** | Package warning-only unknown IDs become empty/unequal and cannot pass exact canonical equality. |
| AC-M5D7I-004 | **CONDITIONAL — P1-002/P1-003** | Exactly-once behavior is intended, but AC wording conflicts with the required preflight Save and recoverable exception set is open. |
| AC-M5D7I-005 | **CONDITIONAL — P1-003/P2-001/P2-002** | Preflight misuse is bounded, but exception mapping, candidate-origin convention, and test-port lifecycle need executable precision. |
| AC-M5D7I-006 | **CONDITIONAL — P1-003/P2-003** | Fatal propagation is named, but the unexpected-exception boundary and fatal candidate disposition need explicit rules. |
| AC-M5D7I-007 | **CONDITIONAL — P1-001/P1-003** | Closed result/reflection coverage cannot be implemented unambiguously until unavailable proof and exception rows are defined. |
| AC-M5D7I-008 | **PASS by boundary, implementation pending** | Static scope can prove one load/post-load save, M5D5 comparison, no router reference, and no forbidden file changes once call accounting is corrected. |
| AC-M5D7I-009 | **BLOCKED — pre-implementation gate** | No execution evidence exists; this AC additionally requires the three P1 amendments and later focused/full regression evidence. |

## Required amendments before approval

1. Close the result representation: explicitly permit/limit unavailable verified proof and define all internal fields/getters and validation rules.
2. Correct call accounting and clarify the package-internal `RemoveAllBindingOverrides` behavior: one preflight Save plus one post-load Save for non-empty input, one Load, no direct removal/extra cleanup.
3. Define an exact recoverable exception allowlist or typed-failure mapping; unexpected and fatal exceptions must escape.
4. Add the P2 lifecycle wording for caller-owned candidate origin/disposal and deterministic-port reset so implementation cannot silently introduce identity or static-state authority.

**Recommendation: remain `Draft`/`Review` and do not approve. No P0 was found, but P1=3 blocks Astra approval. After the three amendments, Luna should perform a second pre-gate before Terra implementation.**

## R2 second pre-gate after amendments

The amended contract was re-read in full against the same Verified dependencies and pinned Input System 1.20.0 source. No runtime, test, contract, asset, asmdef, README, or Unity file was modified and no Unity test was run.

### First-pass P1 resolution

- **P1-001 resolved.** The result now names all internal getters and explicitly separates proof availability. Requested proof is never null. `LoadFailed` and `RoundTripFailed` alone require a null verified proof and make its getter throw after full validation; `DefaultsReady`, `OverridesApplied`, and `RoundTripMismatch` require a valid non-null verified proof with the documented empty/non-empty and ordinal-equality rules. The default/reflection/illegal-row tests are now actionable.
- **P1-002 resolved.** The sequence and call accounting are closed: empty sentinel is `Save` (preflight) and no `Load`; load failure is `Save,Load`; successful, mismatch, and post-load verification failure are `Save,Load,Save`; no retries or calls follow an applicable failure. The instance port wraps the two dependency call types and the contract explicitly permits the package-internal `RemoveAllBindingOverrides` induced by the mandated `Load(..., true)`, while banning any direct adapter call or extra cleanup.
- **P1-003 resolved.** Recoverable exceptions are now a closed set scoped only to the named Input System port operations (`ArgumentException`, `InvalidOperationException`, `FormatException`, `NullReferenceException`, `IndexOutOfRangeException`, and `UnityException`), with the specified M5D5 parse/validation mappings. Adapter validation/programmer errors and every other exception, including the named fatal exceptions, propagate. The internal test port is instance-scoped and supplied only through an internal overload, so it cannot leave static poison.

The amendments preserve the correct authority boundary: M5D5 remains the canonical semantic comparison, M5D6 remains metadata compatibility validation, M5D7H is not called, and M5B5/InputRouter remains the only live map/callback owner. The generated wrapper, input asset, package pin, and project settings remain read-only. The proposed Profile reference does not create an assembly cycle because `AcadeGameMaker.Profile` has no engine or Input dependency.

### Residual P2 notes

- **P2-004 (retained):** “Fresh generated” and caller ownership remain a structural/caller precondition rather than origin-authenticated identity. The adapter cannot prove that a deliberately reflection-bypassed caller did not pass a live wrapper without adding a second binding authority or changing the generated wrapper. The contract now correctly keeps this outside the adapter's authority.
- **P2-005 (retained):** `Discard` is an enforceable result handoff, not physical destruction. The caller must dispose a candidate after any post-load ordinary failure; the adapter must not mutate or clean up the caller-owned object. The fatal-exception path should follow the same caller disposal rule in implementation evidence.
- **P2-006 (retained):** The instance port is feasible, but implementation evidence must prove per-call construction/reset and no candidate/delegate retention, alongside the exact three invocation patterns.

### R2 AC status

| AC | R2 status | Independent basis |
|---|---|---|
| AC-M5D7I-001 | **PASS by contract; implementation pending** | Empty path has exact one preflight Save, zero Load/post-save, exact empty proofs, and disabled-candidate requirements. |
| AC-M5D7I-002 | **PASS by contract; implementation pending** | Real generated-wrapper APIs and known-binding M5D5 canonical round-trip are feasible with exact `Save,Load,Save` sequencing. |
| AC-M5D7I-003 | **PASS by contract; implementation pending** | Pinned package warning-only stale IDs are rejected by valid-but-unequal/empty canonical proof. |
| AC-M5D7I-004 | **PASS by contract; implementation pending** | Call counts, no-retry behavior, closed recoverable injection set, and discard handoff are explicit. |
| AC-M5D7I-005 | **PASS by contract; implementation pending** | Metadata/default/reflection/candidate/port preflight failures are outside the recovery catches and occur before Load. |
| AC-M5D7I-006 | **PASS by contract; implementation pending** | Unexpected and fatal exceptions escape; the instance-only seam cannot statically poison a later fresh call. |
| AC-M5D7I-007 | **PASS by contract; implementation pending** | Every legal row and proof-availability combination now has deterministic validation/getter rules. |
| AC-M5D7I-008 | **PASS by contract; implementation pending** | Exact production sequence, package-internal removal distinction, M5D5 comparison, no router reference, and allowlist are closed. |
| AC-M5D7I-009 | **BLOCKED — implementation gate** | No execution evidence exists at pre-gate; focused/full regressions and Luna post-review remain required after implementation. |

## R2 verdict and recommendation

**PASS — P0=0, P1=0, residual P2=3. Recommend Astra advance M5D7I to Approved.** The amendments close all first-pass gate blockers without expanding InputRouter, persistence, recovery, or generated-wrapper authority. Terra implementation must retain the caller-owned candidate/disposal boundary, instance-scoped test seam, exact call sequences, and exception filters documented above.
