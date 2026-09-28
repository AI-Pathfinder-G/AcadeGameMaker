# VD-09 M5D7E profile load selection core — Luna independent contract pre-gate

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7E profile load selection core](../specs/work-contracts/2026-09-13-vd09-m5d7e-profile-load-selection-core.md)
- Compared against: Approved VD-09 platform/load recovery, vertical-demo SYSTEM-CONTRACTS and TRACEABILITY, P1 profile load-recovery approval, AGENTS.md, and Verified M5D7B–M5D7D boundaries
- Scope: contract/design pre-gate only. No contract, runtime, test, README, or Unity change was made; no Unity test was run.
- First-pass verdict: **CONDITIONAL FAIL — P0=0, P1=1** (historical; P1-001 is resolved below).

## Executive finding

The proposed pure selector is well-bounded and consistent with VD-09. It consumes already-observed candidates, gives current primary precedence, applies M5D7C input repair or previous promotion with the required `r→r+1` semantics, falls back to the approved `-1→0` default only when neither primary nor previous is selectable, never selects temp, and emits non-executing preservation intents. It does not add IO, path, quarantine, save, notification, input-application, Unity, or source-provenance authority. The `long.MaxValue` no-fallback overflow rule is explicit.

The first pass found one public API ambiguity in `DecodeResult` availability. The amended contract now explicitly makes the getter unavailable for Missing/Unreadable candidates, requires full candidate validation before the exact exception, and restricts Decoded candidates to revalidated non-default results with the five exact M5D7B classifications. It also explicitly allows direct-current `long.MaxValue` while retaining no-fallback overflow for recovery paths. The second pass below finds no P0/P1 blocker.

## P1 finding

### P1-001 — RESOLVED: `DecodeResult` availability for non-decoded candidates was unspecified

`ProfileLoadCandidateV1` has one `DecodeResult` getter for all candidate kinds, but the contract only says that `Missing` and `Unreadable` “have no decode payload.” It does not define the public getter behavior or the exact invariant for the absent field. A conforming implementation could return `default(ProfileCanonicalDecodeResultV1)`, return an invalid typed result, or throw only from `Validate()`; each gives different behavior to selector code, default/reflection construction, and callers that inspect the getter.

This matters because AC-M5D7E-008 requires kind/decode mismatch and default/reflection-invalid candidate rejection without mutation. A missing candidate must not be able to expose a decoder classification, and a decoded candidate must not accept a default/malformed decoder result. The selector also needs a deterministic entry rule before it reads a decoded classification.

Required narrow fix: make the rule explicit and testable. Recommended contract/API behavior is: candidate `Validate()` requires `DecodeResult` to be an unavailable private/default representation for `Missing` and `Unreadable`, requires a non-default fully valid `ProfileCanonicalDecodeResultV1` for `Decoded`, and `DecodeResult` calls `Validate()` then throws `InvalidOperationException` when `Kind != Decoded`. Alternatively use an explicit nullable/optional payload, but specify its getter and reflection invariants. Add tests for getter-unavailable on both non-decoded kinds, default decoded result rejection, and all five M5D7B classifications (current, metadata recovery, binding recovery, unsupported, invalid) through the decoded path.

The amended contract applies the recommended rule directly: Missing/Unreadable have no decode payload and their getter validates the complete candidate then throws exact `InvalidOperationException`; only Decoded exposes a revalidated non-default result, restricted to the exact five M5D7B classifications. AC-M5D7E-008 now requires getter, private-proof, default/reflection, and all-kind tests. **P1-001 resolved.**

## P2 findings and precision recommendations

- State directly that `Decoded` accepts the five typed M5D7B classifications, including `Invalid` and `UnsupportedProfileSchema`, because those are required to produce role-specific preservation reasons. “Non-default decode result” currently implies this but does not say it.
- Define the exact getter behavior for preservation intents and plan fields after `Validate()` (all are present for a valid plan; unknown/default enum values and proof mismatches throw). This is implied by the full-getter validation sentence but should be covered by the reflection matrix.
- AC-M5D7E-007 should include a compact cross-product assertion for valid primary plus invalid/unsupported/unreadable previous, and for primary missing/unusable plus each previous state, to demonstrate that preservation intent is independent of which source wins.
- AC-M5D7E-009 should include direct-primary revision `long.MaxValue` as an allowed no-increment case, alongside recovery-source `long.MaxValue` rejection. The selection matrix already implies this distinction.
- The later preservation adapter must receive the original observed candidates separately if it is to produce `hash8`/`nohash`; M5D7E correctly does not retain bytes, paths, parse trees, or exceptions and must not expand its API to do so.

## Boundary and consistency review

- **Selection precedence:** Current primary is direct source at revision `r`, regardless of previous/temp revisions. Input-recoverable primary is repaired before considering previous. Only missing/unreadable/unusable primary permits previous current or input-recoverable promotion; otherwise default bootstrap is selected. This matches REQ-PLAT-010/011 and the P1 load-recovery approval.
- **Temp:** Every existing temp is `StaleTemp`, including valid higher-revision current data and all decoded classifications; temp cannot supply source, result revision, or recovery reason. Missing temp alone yields `None`.
- **Recovery and overflow:** M5D7C owns exact `InputRepair`, `PreviousPromotion`, combined promotion-plus-input repair, and `DefaultBootstrap` outputs. `long.MaxValue` recovery inputs propagate `InvalidOperationException` and cannot silently default; direct current primary needs no increment and remains valid.
- **Preservation:** Non-selected invalid/unsupported/unreadable primary and previous map to their exact role-specific reasons. Current, input-recoverable, or missing files map to `None`; selected primary does not cause previous to change the result. This preserves the approved “preserve, do not delete/guess” boundary without executing quarantine.
- **Ownership and authority:** The plan is an immutable in-memory proof of selection/recovery intent, not a file provenance claim. No IO, path, hash computation, timestamp, quarantine, save execution, notification, input apply, Unity, network, RNG, or callback authority is introduced. Existing M5D7B–D APIs remain read-only dependencies.
- **Allowlist:** The contract's one runtime selector file, one EditMode test file, their metadata, this report/evidence, and a minimal README entry are sufficiently narrow; no asmdef/package/project/scene/prefab/input asset change is needed.

## AC design status

| AC | Pre-gate status | Independent basis |
|---|---|---|
| AC-M5D7E-001 | **Pass by contract** | Direct current primary always wins and preserves revision `r`; temp/previous cannot alter it. |
| AC-M5D7E-002 | **Pass by contract** | Primary metadata/binding recovery maps to M5D7C `InputRepair`, preserves non-input state, and increments exactly once. |
| AC-M5D7E-003 | **Pass by contract** | Missing/invalid/unsupported/unreadable primary permits only current or input-recoverable previous promotion with exact reasons/revision. |
| AC-M5D7E-004 | **Pass by contract** | Input-recoverable previous uses exact combined `PreviousPromotion|InputRepair` and `r→r+1`. |
| AC-M5D7E-005 | **Pass by contract** | No selectable primary/previous yields approved default `-1→0`; current temp remains irrelevant. |
| AC-M5D7E-006 | **Pass by contract** | All existing temp classifications become `StaleTemp`; only missing temp is `None`. |
| AC-M5D7E-007 | **Pass by contract; P2 matrix extension** | Role-specific invalid/unsupported/unreadable intents are explicit; the full source-winner cross-product should be tested. |
| AC-M5D7E-008 | **Blocked by P1-001** | Role/kind/default/reflection rejection is stated, but decode getter availability for non-decoded candidates is not closed. |
| AC-M5D7E-009 | **Pass by contract; P2 boundary extension** | M5D7C overflow/no-fallback and deterministic repeated selection are explicit; direct-current `long.MaxValue` should be a test case. |
| AC-M5D7E-010 | **Not executable at pre-gate** | Focused/full execution belongs after implementation; no launch-recovery or quarantine PASS is claimed here. |

## P0/P1/P2 summary

- P0: none.
- P1: P1-001 — specify `DecodeResult` unavailable behavior and candidate kind/decode invariants.
- P2: explicitly list accepted typed decoded classifications; expand role/source-winner preservation cross-product; test direct-current `long.MaxValue`; reflection getter/proof matrix; preserve later adapter's separate candidate input requirement.

## First-pass recommendation (superseded by second pass)

Do not advance M5D7E to Astra approval until P1-001 is repaired in the contract. After the getter/absence rule is explicit, this is otherwise a bounded, implementable selector design aligned with VD-09 and the Verified M5D7B–D ownership boundaries. A second pre-gate should confirm the repair; no implementation or Unity testing is warranted before then.

## Second independent pre-gate — amended contract

- Review date: 2026-09-13
- Scope: amended contract text only; no contract, runtime, test, README, or Unity change was made and no Unity test was run.
- Verdict: **PASS — P0=0, P1=0; recommend Astra approval review (do not change contract status here).**

### P1 resolution

P1-001 is closed. The amended contract now unambiguously requires:

- Missing/Unreadable candidates to carry no decode payload; their `DecodeResult` getter must validate the whole candidate and throw exact `InvalidOperationException`.
- Decoded candidates alone to expose `DecodeResult`, after full revalidation, with a non-default result restricted to the five exact M5D7B classifications: current, metadata recovery, binding recovery, unsupported schema, or invalid.
- AC-M5D7E-008 to test unavailable getters, private-proof/default/reflection-invalid candidates, kind/decode mismatch, all five classifications, and source mutation independence.

The revision also closes the noted boundary ambiguity: direct current-primary `long.MaxValue` is allowed because no increment occurs; `long.MaxValue` input-recovery primary or selectable previous still propagates M5D7C overflow and cannot fall back to default.

### Second-pass AC assessment

| AC | Second-pass status | Independent basis |
|---|---|---|
| AC-M5D7E-001 | **Pass by contract** | Direct current primary always wins, including `long.MaxValue`, and temp/previous cannot alter the selected revision. |
| AC-M5D7E-002 | **Pass by contract** | Primary metadata/binding recovery uses exact M5D7C input repair with non-input preservation and one revision increment. |
| AC-M5D7E-003 | **Pass by contract** | Missing/unusable primary selects only current or input-recoverable previous promotion. |
| AC-M5D7E-004 | **Pass by contract** | Input-recoverable previous produces exact combined promotion/input-repair reasons and `r→r+1`. |
| AC-M5D7E-005 | **Pass by contract** | No selectable primary/previous yields approved `-1→0` default; current temp remains ignored. |
| AC-M5D7E-006 | **Pass by contract** | Every existing temp is `StaleTemp`, regardless of classification/revision; only missing temp is `None`. |
| AC-M5D7E-007 | **Pass by contract; implementation matrix remains** | Role-specific preservation reasons and valid-primary/abnormal-previous behavior are exact. |
| AC-M5D7E-008 | **Pass by contract** | Getter availability, five-classification restriction, full revalidation, private proof, reflection rejection, and mutation independence are now explicit. |
| AC-M5D7E-009 | **Pass by contract** | Recovery revisions `0`, positive, max-1 and overflow are closed; direct current max is explicitly no-save/no-increment. |
| AC-M5D7E-010 | **Not executable at pre-gate** | Focused/full execution and Luna P0/P1 evidence belong after implementation; no load/quarantine/save overclaim is made. |

## Second-pass remaining P2

- Keep the recommended cross-product tests for valid-primary plus abnormal previous, primary unusable plus each previous classification, and all temp classifications. These are coverage-strengthening, not contract blockers.
- Preserve the later adapter boundary: it must receive observed candidates separately if it needs readable-content `hash8`/unreadable `nohash`; M5D7E must not retain bytes, paths, parse trees, or exceptions.
- Reflection tests should cover preservation-intent and plan getter revalidation in addition to candidate getter availability, as now required by the amended AC-008.

## Second-pass conclusion

No P0 or P1 remains in the amended M5D7E contract. Selection precedence, temp exclusion, recovery/default revision semantics, preservation intent, and no-IO authority are sufficiently explicit for Astra's approval decision. This is a contract pre-gate PASS only; the contract remains `Review` until Astra changes its status.
