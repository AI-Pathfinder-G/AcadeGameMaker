# VD-09 M5D7C recovery transformation core — Luna independent contract pre-gate

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7C profile recovery transformation core](../specs/work-contracts/2026-09-13-vd09-m5d7c-profile-recovery-transformation-core.md)
- Compared against: Approved VD-09, VD-05, SYSTEM-CONTRACTS, ADR-0031, and Verified M5D3–M5D7B APIs/contracts
- Verdict: **Second pre-gate PASS — P0=0, P1=0; recommend Astra approval before implementation**
- Scope: second contract/design pre-gate re-review. No implementation was changed and no Unity test was run.

## Second pre-gate re-review

The revised contract resolves all three first-pass P1 blockers.

- **P1-001 resolved:** the operation is now named `PlanInputRepair`, explicitly disclaims primary-file provenance, and states that the later file-selection owner selects the actual primary before invoking it. `PlanPreviousPromotion` likewise states that disk provenance is not inspected and requires the later selector's typed previous-candidate orchestration precondition. AC-M5D7C-002 explicitly does not overclaim primary selection. This preserves the no-IO/no-selection boundary while making the caller-owned primary/previous semantics explicit.
- **P1-002 resolved:** well-formed but inapplicable `Invalid`/`Unsupported` or other wrong classifications are `ArgumentException`; default/reflection-invalid source results are converted at entry to `ArgumentException` while preserving the inner `InvalidOperationException`; invalid plans/getters remain `InvalidOperationException`; a validated `long.MaxValue` source remains an unwrapped overflow `InvalidOperationException`. AC-M5D7C-005 and AC-M5D7C-007 now state the separation and precedence.
- **P1-003 resolved:** plan validation now closes the reason domain to exactly `{DefaultBootstrap, InputRepair, PreviousPromotion, PreviousPromotion|InputRepair}`, with all zero/unknown/other combinations rejected. The source/result and transformation-kind mapping is explicit and testable.

The remaining implementation-gate risk is only evidence-level: tests must prove the selector preconditions are caller-owned, source/result entry validation happens before revision increment, and private preservation proof stores semantic values/projections rather than source bytes or parse trees. No new P0/P1 contradiction was found.

## Executive finding

The proposed transformation boundary is otherwise well scoped: it is engine-free, does not select files or perform IO, treats default as source revision `-1` and first result revision `0`, increments committed source revisions without wrap, and uses the M5D7B metadata/binding recovery projection to avoid retaining untrusted source bytes. The approved defaults and current-compatible replacement input are compatible with VD-09 `REQ-PLAT-010/011` and the Verified M5D3–M5D7B value APIs.

The first pass identified three contract ambiguities: primary/previous caller provenance, default/source exception precedence, and closure of the reason-flags domain. The revised contract now resolves each one through caller-owned selection preconditions, explicit entry-versus-plan exception rules, and an exact four-combination reason set. These remain listed below as historical findings for traceability only.

## First-pass P1 findings — all resolved by the revision

The following sections preserve the original adversarial analysis; their required fixes are superseded by the second-review resolution above.

### P1-001 — RESOLVED: primary/previous source provenance is not represented or normatively caller-certified

`ProfileCanonicalDecodeResultV1` is intentionally provenance-free in Verified M5D7B: it describes the bytes and their semantic classification, not whether the caller obtained them from primary or previous storage. Both `PlanPrimaryInputRepair(ProfileCanonicalDecodeResultV1 source)` and `PlanPreviousPromotion(ProfileCanonicalDecodeResultV1 source)` accept the same value type. Therefore the identical valid current or recovery result can be passed to the wrong method and still produce a plan with the wrong semantic reason (`PreviousPromotion` versus `InputRepair`). The planner cannot inspect a path without violating the no-IO/file-selection boundary, and it cannot prove the caller's slot from the result itself.

This matters even though the state transformation is otherwise identical in some cases: VD-09 requires valid primary to win and only a selected valid previous to be promoted, while the emitted reason/diagnostic contract identifies the operation. The current AC-005 matrix rejects invalid classifications but does not reject a valid primary result supplied to the previous operation.

Required narrow fix (choose one and test it):

1. Keep the pure methods but state a mandatory caller precondition that the argument to `PlanPreviousPromotion` is a caller-certified previous candidate and the argument to `PlanPrimaryInputRepair` is a caller-certified primary candidate; explicitly state that this provenance is external, never inferred by M5D7B or the planner, and is not persisted/gameplay authority. Add an AC case showing that the planner does not claim to select a file and that the caller owns the method choice; or
2. Add a small non-IO source-slot enum/context parameter (for example `Primary`/`Previous`) and reject a mismatched operation before transformation. Do not add paths, file reads, or recovery selection.

The contract must also say whether a valid current result is allowed as a primary input to a separate future “normal load” path; M5D7C itself must not silently turn that into a previous-promotion reason.

### P1-002 — RESOLVED: exception classification/precedence conflicts for default and bypassed sources

AC-M5D7C-005 says primary current/invalid/unsupported and previous invalid/unsupported/**default result misuse** are `ArgumentException`. The purpose/validation text and AC-M5D7C-007 simultaneously require default/reflection-bypassed source/result/document/revision misuse to be rejected with `InvalidOperationException`. A default `ProfileCanonicalDecodeResultV1` has no usable classification; accessing its validating getter can therefore fail before the planner can classify it as a normal `Invalid` or `Unsupported` source. Reflection-bypassed malformed results have the same distinction.

Required narrow fix: define an explicit precedence and update AC-005/AC-007. Recommended rule:

- a well-formed M5D7B result with classification `Invalid` or `UnsupportedProfileSchema`, when passed to either transformation, is caller-data misuse and yields `ArgumentException`;
- a default, reflection-bypassed, internally inconsistent, or otherwise non-validatable decode result is representation/programmer misuse and yields `InvalidOperationException` (not wrapped as `ArgumentException`);
- a well-formed but wrong valid classification (`ValidCurrentInput` for primary repair, for example) is `ArgumentException`;
- overflow is checked after source validation and remains `InvalidOperationException`, with no source/result mutation.

If another policy is intended, the same distinction and test precedence must be stated unambiguously. Current wording cannot produce a deterministic AC result.

### P1-003 — RESOLVED: the plan-reason flags domain is not closed to the four legal transformations

The contract declares `DefaultBootstrap=1`, `PreviousPromotion=2`, and `InputRepair=4`, and each entry point specifies an exact reason. However, `Validate()` is only said to check “known nonzero reason combinations”; that could be read as accepting every known-bit combination (`3`, `5`, `6`, `7`) rather than only the combinations produced by this API. In particular, `DefaultBootstrap|InputRepair` and `DefaultBootstrap|PreviousPromotion` have no defined source/result semantics, and bare `InputRepair` is only legal for primary repair while `PreviousPromotion|InputRepair` is only legal for previous recovery promotion.

Required narrow fix: state the closed legal set and context mapping explicitly: `DefaultBootstrap` only for `source=-1,result=0`; `InputRepair` only for primary recovery repair; `PreviousPromotion` only for previous current promotion; `PreviousPromotion|InputRepair` only for previous input recovery; `None`, unknown bits, and all other known-bit combinations reject with `InvalidOperationException` on plan validation/getters. Add reflection-bypass tests for at least `None`, `3`, `5`, and `7` and assert no fallback transformation.

## P2 findings / implementation-gate requirements

- The private preservation proof is feasible without unsafe source bytes: for current sources retain validated semantic snapshots (including canonical binding text), and for metadata/binding recovery retain the M5D7B projection plus source revision. Do not retain the decoder's raw byte array, mutable parse tree, or IO handle. Revalidate the retained value snapshots and compare output non-input fields by value, including tutorial/completed-branch collections; output getters must continue to be independent clones where the dependency API exposes collections.
- For metadata recovery, the planner must not accidentally use the mismatched input from `Document`; preservation proof should come from the projection/non-input values and the output must be rebuilt with the exact M5D6 current asset ID, schema `1`, and empty binding sentinel. For binding recovery, there is deliberately no document; only the projection can supply settings/tutorial/progression and revision.
- The `long.MaxValue-1` boundary is representable (`result=long.MaxValue`), while `long.MaxValue` must fail before constructing or mutating a result. `-1` is valid only for `PlanDefaultBootstrap`; every decode-derived source revision must be nonnegative and come from validated progression.
- AC-005 should include invalid/unsupported and default/reflection-bypass cases separately after P1-002 is resolved. There is no null case because all public transformation arguments are value types; do not invent a nullable source or null authority.
- The no-IO boundary is honest and must remain so: M5D7C does not choose primary/previous, quarantine files, save, notify, apply Input System bindings, consult clocks/RNG/network, or assert persistence/recovery execution success. A later adapter must supply source-slot selection and consume the plan.

## AC design status

| AC | Pre-gate status | Independent basis |
|---|---|---|
| AC-M5D7C-001 | Conditional pass | Defaults and `-1 → 0` are explicit and compatible with M5D3/M5D4/M5D6/M5D7A; implementation must prove exact bytes/hash and repeatability. |
| AC-M5D7C-002 | Pass by design | M5D7B metadata/binding projections carry exactly the non-input values needed; the revised contract assigns primary selection to the later selector and does not overclaim it here. |
| AC-M5D7C-003 | Pass by design | The revised contract makes the caller-owned previous-candidate precondition explicit while keeping disk provenance outside this transform. |
| AC-M5D7C-004 | Pass by design | Signed-64 revision and overflow policy are representable; implementation must check before increment/mutation. |
| AC-M5D7C-005 | Pass by design | Source-entry data misuse, default/reflection-invalid source conversion, and overflow now have distinct stated outcomes. |
| AC-M5D7C-006 | Conditional pass | Dependency value objects support defensive collection semantics; planner proof/output must compare and clone by value. |
| AC-M5D7C-007 | Pass by design | Plan invalidity remains `InvalidOperationException`, and the four legal reason combinations are explicitly closed. |
| AC-M5D7C-008 | Not executable at pre-gate | Runtime/test and full-suite evidence do not exist yet; no persistence/recovery execution claim is permitted. |

## P0/P1/P2 summary — second review

- P0: none found.
- P1: none remaining. P1-001, P1-002, and P1-003 are resolved by the revised contract text and AC matrix.
- P2: proof-state representation and projection-only preservation are implementation gates; keep AC adversarial cases, caller-owned source selection, and no-IO/no-overclaim boundary explicit.

## Recommendation

Recommend Astra advance this Review contract to approval. The revised wording has P0/P1=0 and is implementable within the stated one-runtime-file/one-test-file boundary. Terra's implementation gate must preserve the exact four reason mappings, `-1` default versus nonnegative decode revisions, pre-increment overflow rejection, caller-owned primary/previous selection, semantic/projection-only preservation proof, and the no-IO/no-persistence boundary. This report does not mark the contract Approved or Verified.
