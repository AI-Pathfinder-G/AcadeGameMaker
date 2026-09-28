# VD-09 M5D7H primary input-recovery atomic save — Luna independent contract pre-gate

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7H primary input-recovery atomic save extension](../specs/work-contracts/2026-09-13-vd09-m5d7h-primary-input-recovery-atomic-save.md)
- Compared against: VD-09 `REQ-PLAT-009`/`REQ-PLAT-011`, the approved atomic-write and load-recovery decisions, `SYSTEM-CONTRACTS`/traceability, AGENTS.md, and Verified M5D7B/M5D7C/M5D7D/M5D7E/M5D7G boundaries.
- Scope: contract-only independent pre-gate. No contract, runtime, test, README, or Unity files were modified; no Unity test was run.

## Verdict

**CONDITIONAL FAIL — P0=0, P1=3, P2=3. Do not approve until the three P1 findings below are resolved.**

The draft has a sound narrow objective: take an already selected primary input-recovery decode, derive the exact M5D7C input-repair document, write and validate `profile.tmp.json`, revalidate the fresh primary, and perform exactly one `File.Replace(temp, primary, previous, true)`. The draft correctly excludes selection, observation, quarantine, input application, logging, Unity paths, launch orchestration, retries, and fallback. The binding-recovery amendment is also correct: `InputCompatibility` is compared only for metadata recovery; binding recovery uses the available exact `InputRecoveryReasons` and plan output and must not call the unavailable getter.

Three details currently prevent a deterministic, fail-closed implementation gate: source role is not represented by the entry proof, the new result type/matrix is not defined precisely enough to implement, and the permitted modification of Verified M5D7D source has no explicit additive compatibility boundary.

## P1 findings

### P1-001 — Bare decode result cannot prove that the source is the selected primary

`SaveInputRecovery` accepts only `ProfileCanonicalDecodeResultV1`. That value intentionally carries no file role, path, candidate identity, or source-byte identity. A decode result from `profile.prev.json` or `profile.tmp.json`, or an independently constructed/reflection-forged value with the same valid recovery plan, is indistinguishable from the selected `profile.json` input-recovery source at this API boundary. The method then writes the derived result to the fixed primary path and preserves the currently read primary as previous. Its name and an eventual caller convention do not provide an enforceable source proof.

The draft explicitly says that the later coordinator owns the primary-role provenance, which is a reasonable layering boundary for M5D7C's pure planner but is too weak as the sole guard on a write-capable recovery adapter. This is especially relevant to REQ-M5D7H-001's “only an exact ... primary input-repair plan” wording and AC-M5D7H-003's fail-closed observed-value cases.

Minimal fix: change the internal entry to accept a typed primary candidate/wrapper, for example an internal `ProfileLoadCandidateV1` whose role must be `Primary`, kind `Decoded`, and classification one of the two input-recovery classes, or an internal `ProfilePrimaryInputRecoverySourceV1` created only from that primary candidate. Derive the plan from the wrapper's decode result, retain no path/bytes, and keep the fresh semantic revalidation. Add a wrong-role/previous/temp and forged-wrapper test proving no port call. If caller-owned provenance is intentional instead, narrow REQ/AC language to a documented caller precondition and explicitly state that the adapter does not claim to enforce primary identity; under the current “only ... primary” wording, a typed proof is the safer contract.

### P1-002 — `ProfileInputRecoverySaveResultV1` and its closed state matrix are underspecified

The draft names a new immutable result and describes three outcomes, but does not define its exact enum/type fields or public/internal getters. It is unclear whether the result reuses Verified M5D7D's `ProfileAtomicSaveOutcome`/`ProfileAtomicSaveStage`/`ProfileStoredFileState` and `ProfileAtomicSaveResultV1`, adds a recovery-specific outcome to those enums, or introduces a separate result family. Reusing M5D7D's result would not represent the required `CommittedRecoveryReplacement` row without changing its already Verified enum/matrix; adding a public enum value would alter that contract.

The failure rows also say “independently probed three-file states” without stating the exact allowed `NotProbed`/`Missing`/`Unreadable` combinations for directory, temp-write, temp-read, and existing-primary failures, or the required all-probed rule after commit uncertainty. Without this, default/reflection/illegal state validation and AC-M5D7H-005/006/009 are not deterministic, and two implementations can both claim conformance while exposing different state meanings.

Minimal fix: define the result type and all getter names/types; define separate internal outcome/stage/state enums or explicitly define a recovery-only result that cannot change M5D7D's public ABI. Specify the exact success row, `FailedBeforeCommit` stage rows and allowable `NotProbed` states, `CommitOutcomeUncertain` all-file-probed rule, exact `-1` revision sentinel, and full validation from every getter. Add an AC matrix covering each stage, default/unknown enum, success-state mismatch, sentinel mismatch, and reflection-forged combinations.

### P1-003 — Modification of the Verified M5D7D source is not bounded enough

The allowlist permits changes to `ProfileAtomicSaveServiceV1.cs` so the new entry can share the private M5D7D lease and file port, while explicitly making the existing M5D7D test file read-only. However, the requirements do not state that ordinary `Save`, its public enums/result ABI, existing BCL operations, and its result-validation matrix must remain behaviorally unchanged. A Terra change to the shared `SaveHeld`, registry, catch filters, or result types could satisfy the new method while regressing the already Verified ordinary-save contract or changing its public surface.

Minimal fix: add an explicit additive-only rule: ordinary `Save` behavior, public types, existing port semantics, and M5D7D result invariants remain unchanged; only private shared helpers may be refactored when equivalence is proven. Forbid adding a new public save API or changing existing enum numeric values/ranges. Require the M5D7D focused regression (or an equivalent full regression) and a static diff/allowlist check as part of AC-M5D7H-010. The new recovery result should be internal and separate from the Verified public result unless M5D7D is explicitly re-contracted and re-approved.

## P2 findings and precision notes

### P2-001 — Temp and previous-preservation preconditions are caller-only and not typed

The draft correctly says the later launch owner must resolve stale temp and previous-file preservation intent before calling, and that M5D7H must not claim this precondition. The method nevertheless truncates `profile.tmp.json` and overwrites `profile.prev.json` through `File.Replace`; it cannot detect an unresolved preservation intent because no such input is accepted. Keep this as an explicit caller precondition in the future orchestration contract and add a no-selection/no-quarantine test. It is not a M5D7H authority defect.

### P2-002 — Shared lease testability needs an explicit cross-entry barrier

Reusing M5D7D's normalized, case-insensitive ref-counted lease is the correct design, but AC-M5D7H-008 should state the exact injected barrier: an ordinary `Save` and one or more `SaveInputRecovery` calls using case/trailing-separator aliases must register before the active transaction releases, serialize all file operations, permit different roots to proceed, and leave the single registry empty. This should be tested without adding a second registry or public injection API.

### P2-003 — Recoverable-probe and result-state precedence should be tabulated

The text distinguishes recoverable faults from unexpected/fatal exceptions, but a future implementation still needs a compact precedence table for primary `Missing`, access failure, source read failure, decoder classification mismatch, temp validation failure, replace exception, and postcheck failure. In particular, probe access after a present requirement should be an `ExistingPrimaryValidation` precommit result, while a missing primary must not fall back to first-save move. This is mostly covered by the prose and should be made executable in AC-004/005/006 tests after the P1 result shape is fixed.

## Boundary and dependency review

- M5D7C is used only as an engine-free transformation. The amendment correctly avoids calling `InputCompatibility` for binding recovery, where M5D7B makes that getter unavailable; exact reasons plus plan output are the available proof.
- M5D7D's exact write-through sequence, `Flush(true)`, close-before-read, and Windows `File.Replace(temp, primary, previous, true)` are compatible with this recovery-only replacement. The recovery path correctly forbids first-save move, copy/delete/manual rename fallback, retry, rollback, or cleanup.
- Fresh source validation is semantically strong: classification, available compatibility, exact reasons, source/result revisions, and M5D7C canonical result bytes must match before replace. Allowing semantically equivalent source bytes is explicitly distinguished from original-byte identity; the captured old primary bytes are separately required to match post-replace previous byte-for-byte.
- The result must not claim that stale temp or prior preservation intent was handled, select a source, read Unity `Application.persistentDataPath`, apply bindings, log/notify, or enter gameplay. The allowlist is otherwise appropriately narrow, subject to P1-003's additive D-source rule.

## Pre-gate AC status

| AC | Status | Independent basis |
|---|---|---|
| AC-M5D7H-001 | **CONDITIONAL — P1-001/P1-002** | Transaction and byte-preservation sequence is clear, but primary-role proof and success-result shape are not closed. |
| AC-M5D7H-002 | **PASS by contract amendment, implementation pending** | Binding recovery uses exact reasons/plan output and does not call unavailable `InputCompatibility`; canonical outer binding fixture is specified. |
| AC-M5D7H-003 | **CONDITIONAL — P1-001/P1-002** | Pre-I/O misuse/overflow/path rejection is specified, but wrong-role source and exact result failure rows need typed proof. |
| AC-M5D7H-004 | **PASS by algorithm, test matrix pending** | Fresh semantic identity rejects missing/current/invalid/unsupported/changed source and forbids move fallback. |
| AC-M5D7H-005 | **CONDITIONAL — P1-002/P1-003** | Precommit stage behavior is aligned with M5D7D, but state combinations and ordinary-save non-regression are not explicit. |
| AC-M5D7H-006 | **PASS by algorithm, state matrix pending** | Replace/postcheck faults are uncertain with no retry/rollback; exact getter-state proof remains underspecified. |
| AC-M5D7H-007 | **PASS by contract, barrier evidence pending** | Exact write/flush/close/read/replace/postcheck order and old-primary byte comparison are stated. |
| AC-M5D7H-008 | **CONDITIONAL — P2-002** | One shared lease is required, but the cross-entry waiter/barrier seam should be made explicit. |
| AC-M5D7H-009 | **CONDITIONAL — P1-001/P1-002** | Mutation/default/reflection/fault boundaries are required but cannot be fully implemented without typed source/result definitions. |
| AC-M5D7H-010 | **BLOCKED — P1-003 and pre-implementation gate** | No execution evidence exists yet, and the contract must require D regression/non-regression proof. |

## Required amendments before approval

1. Add an enforceable primary-source identity wrapper or primary-role candidate parameter, with wrong-role/forged-source fail-closed tests.
2. Define `ProfileInputRecoverySaveResultV1` (or a separate internal recovery result family) completely: getters, enums, exact stage/state matrix, sentinels, success row, uncertain all-probed rule, and reflection/default validation.
3. State additive-only compatibility for Verified M5D7D source/public ABI/ordinary `Save`, and require focused/full regression evidence plus a scoped diff check.
4. Make the shared-lease cross-entry registration barrier and precommit/probe result precedence executable in the focused AC matrix.

**Recommendation: remain `Review`. No P0 was found, but P1=3 means Astra should not advance the contract to Approved until the amendments are incorporated and Luna performs a second pre-gate.**

## Second-pass review after the three P1 amendments

The revised draft closes all three first-pass P1 findings. This remains a contract-only review; no implementation or Unity tests were run.

- **P1-001 resolved:** `SaveInputRecovery` now accepts a `ProfileLoadCandidateV1`, requires exact `Primary` role and `Decoded` kind, fully validates the candidate, and accepts only the two input-recovery classifications before acquiring the lease or touching I/O. Previous/temp/missing/unreadable/default/reflection-invalid/wrong-role candidates are explicit pre-I/O argument failures. The method derives the repair plan from `observedPrimary.DecodeResult`, while retaining no path or bytes.
- **P1-002 resolved:** `ProfileInputRecoverySaveOutcomeV1` and `ProfileInputRecoverySaveResultV1` are now explicitly internal, with exact getters and separate recovery outcome values. Existing M5D7D stage/state enums are reused without changing their values or ranges. The result matrix is closed: success is the exact recovery replacement row; precommit stages are directory, temp write/flush/close, temp validation, or existing-primary validation; uncertainty is commit call or postcheck; all three states must be independently probed and non-`NotProbed`; failures use committed revision `-1`; every getter validates.
- **P1-003 resolved:** the additive compatibility rule explicitly preserves M5D7D public ABI, enum values, result matrix, port semantics, ordinary `Save` behavior, and focused/full regressions. Private lease/helper refactoring requires equivalence proof; no public recovery API, second registry, or second port is allowed.
- The binding-recovery getter restriction remains correct. Fresh metadata-recovery sources compare available `InputCompatibility`; binding-recovery sources never call that unavailable getter and instead compare exact `InputRecoveryReasons` plus the M5D7C plan output.
- The failure-precedence table now fixes missing/access/read/classification/semantic mismatches as `ExistingPrimaryValidation` with replace/move zero, temp failures as `TempReadValidation`, replace faults as `CommitCall` uncertainty, postcheck faults as `PostCommitValidation` uncertainty, and invalid typed port/unexpected/fatal faults as escaping exceptions with lease release.
- AC-M5D7H-008 now requires only eventual waiter drain after all active+waiter registrations are observed; it explicitly makes no Monitor FIFO/wake-order assumption. Same-root case/trailing aliases share one ref-counted lease, different roots can progress, and the registry must empty after final release.

## Second-pass residual P2 notes

### P2-004 — Role provenance remains structural, not origin-authenticated

The typed candidate closes the practical wrong-role path and matches the existing M5D7E value-proof boundary. As with other internal immutable value types, reflection can fabricate a structurally valid primary candidate from copied values; no runtime value type can prove historical filesystem origin without retaining forbidden path/byte authority. The contract correctly treats this as caller-owned observation provenance and still requires fresh semantic revalidation. Keep the later launch-owner call-site precondition explicit and test structural reflection failures.

### P2-005 — Caller-owned temp/previous preservation precondition remains intentionally unverified here

M5D7H may truncate stale temp and replace previous, but the later launch owner is explicitly responsible for resolving stale-temp and previous-preservation intents first. This is the correct no-orchestration boundary; the future launch contract should carry the sequencing proof rather than expanding this adapter.

### P2-006 — Additive M5D7D equivalence awaits implementation evidence

The revised rule is precise, but the eventual implementation review must compare the M5D7D focused results and full-suite evidence, inspect public ABI/enum diffs, and prove no second lease/port/registry was introduced. This is an implementation gate, not a remaining contract contradiction.

## Second-pass AC status

| AC | Second-pass status | Basis |
|---|---|---|
| AC-M5D7H-001 | **PASS by contract; implementation pending** | Exact metadata-recovery plan, `r→r+1`, one replace, old primary byte preservation, and closed success row are defined. |
| AC-M5D7H-002 | **PASS by contract; implementation pending** | Binding recovery uses exact reasons/plan output, preserves non-input state, and never calls unavailable compatibility getter. |
| AC-M5D7H-003 | **PASS by contract** | Primary candidate role/kind/classification, misuse, overflow, and path errors are pre-I/O and fail closed. |
| AC-M5D7H-004 | **PASS by contract** | Fresh source semantic identity and no-move/no-fallback precedence are explicit. |
| AC-M5D7H-005 | **PASS by contract** | Exact precommit stages, all-probed typed states, unchanged primary/previous, and replace/move zero are defined. |
| AC-M5D7H-006 | **PASS by contract** | Commit/postcheck faults produce uncertainty with all states probed and no retry/rollback/fallback. |
| AC-M5D7H-007 | **PASS by contract** | Exact operation order, one replace, zero move, and old-primary byte comparison are explicit. |
| AC-M5D7H-008 | **PASS by contract** | One shared lease, active+waiter barrier, alias normalization, eventual drain without FIFO assumptions, different-root progress, and empty registry are explicit. |
| AC-M5D7H-009 | **PASS by contract** | Candidate/result reflection, every closed row, `NotProbed`, sentinel, typed-port, exception, and lease-cleanup boundaries are defined. |
| AC-M5D7H-010 | **PASS as future implementation gate** | Static additive-only M5D7D rule and required focused/full regression evidence are now explicit; no execution evidence exists at pre-gate. |

## Second-pass verdict and recommendation

**PASS — P0=0, P1=0, residual P2=3. Recommend Astra advance M5D7H to Approved.** The amended contract now has enforceable primary candidate provenance, a complete recovery-only result/matrix without changing M5D7D's public ABI, explicit failure precedence, and a realistic shared-lease barrier that does not overclaim FIFO ordering. Preserve the caller-owned temp/previous sequencing boundary and require the stated M5D7D equivalence evidence during implementation review.
