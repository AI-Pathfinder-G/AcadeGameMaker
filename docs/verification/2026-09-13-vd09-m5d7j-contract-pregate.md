# VD-09 M5D7J binding-apply-failure recovery transform — Luna independent contract pre-gate

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7J binding-apply-failure recovery transform](../specs/work-contracts/2026-09-13-vd09-m5d7j-binding-apply-failure-recovery-transform.md)
- Compared against: Verified M5D7C recovery planner, M5D7E load selector, M5D7I Input System apply adapter, M5D7B decoder, M5D7D ordinary atomic save boundary, VD-09 rule 7, SYSTEM-CONTRACTS/traceability, AGENTS.md, and current Profile source/tests.
- Scope: engine-free contract-only independent pre-gate. No contract, runtime, test, asmdef, asset, README, or Unity file was modified; no Unity test was run.

## Verdict

**PASS — P0=0, P1=0, residual P2=3. Recommend Astra advance M5D7J to Approved.**

The draft is a narrow and implementable additive transform. It handles only the case that M5D7I cannot apply a non-empty override to a metadata-current candidate, resets only the input block to the approved current empty default, preserves non-input semantics, and increments the source revision exactly once. The explicit primary and previous methods remove the otherwise easy previous-promotion double-increment error. The contract correctly disclaims authentication of the external M5D7I assertion and keeps Unity, Input System, IO, selection, quarantine, save, notification, and launch authority outside the engine-free Profile assembly.

## Boundary and dependency review

- M5D7I's `RecoveryRequired` rows necessarily originate from a current-compatible non-empty input snapshot. M5D7J accepts the corresponding `ValidCurrentInput` decode result only when its canonical binding override is non-empty; empty current input is rejected because M5D7I would have returned `DefaultsReady`. Metadata/binding decoder-recovery classifications remain owned by the existing M5D7C transforms.
- The primary method yields `r → r+1` with exact `InputRepair`. The previous method yields `r → r+1` with exact `PreviousPromotion|InputRepair`; it does not compose `PlanPreviousPromotion` and then add another revision. This is consistent with M5D7E's recovery reason model and M5D7C's four closed reason combinations.
- The source input remains a private proof while output input is current ID/current schema/empty sentinel. Settings, tutorial IDs, and progression values are copied as immutable snapshots; only progression revision changes. Existing snapshot constructors and getters defensively copy arrays, so the requested non-input independence is feasible without retaining bytes, parse trees, or IO handles.
- Later ordinary M5D7D `Save` can persist both rows: a primary-current source is an accepted existing-current primary for replacement, while a previous-current source is saved after the selector-owned primary invalid/missing/unreadable/quarantine precondition has resolved. M5D7J does not need or add a second save path.
- The Profile assembly is engine-free. The proposed implementation can modify only `ProfileRecoveryPlannerV1.cs`, add private proof branches and two public methods, and require no Unity/Input dependency or asmdef edge. Existing M5D7C methods, enum numeric values, outputs, and tests remain protected by the additive-only rule.

## P0/P1 findings

No P0 or P1 findings. The following potentially dangerous boundaries are explicitly closed by the contract:

1. **Actual-failure proof:** the transform accepts a caller assertion that M5D7I failed for the matching selected source but does not pretend to authenticate that assertion. This matches M5D7C/M5D7E's caller-owned source/provenance boundary and avoids a forbidden Input.Unity dependency. The later launch owner must compare the assertion to the selected source before calling.
2. **Revision arithmetic:** source `long.MaxValue` is rejected before output; `long.MaxValue-1` increments to `long.MaxValue`; previous failure uses one increment, not two. No fallback/default result is allowed.
3. **Result proof:** new private proof kinds must be distinct and every public getter/`Validate()` must revalidate proof kind, exact reason, source/result revision relation, original non-empty current input, preserved state, and current-empty output. Existing four reason combinations and public enum values are not broadened.
4. **Failure boundary:** well-formed inapplicable sources use `ArgumentException`; default/reflection-invalid decode values use an `ArgumentException` with the original `InvalidOperationException` inner exception; applicable overflow remains an unwrapped `InvalidOperationException`. No mutable state is retained or poisoned.

## Residual P2 notes

- **P2-001 — M5D7I assertion is not origin-authenticated:** a bare decode result cannot prove which file or candidate produced the apply failure. This is intentional and honestly stated; the launch coordinator owns source selection and matching. Do not add path/candidate/Unity provenance to this transform.
- **P2-002 — Private proof-kind numeric values are not fixed:** the contract requires distinct private kinds and reflection fail-closed behavior but does not prescribe their integer values. Because they are private and not persisted/public ABI, this is harmless; tests should inspect behavior rather than assume a number.
- **P2-003 — Save sequencing belongs to the later launch owner:** M5D7J returns an in-memory plan but does not prove that stale temp/previous preservation, quarantine, or selected-source persistence preconditions have been resolved. The future coordinator must pass the exact plan to ordinary M5D7D Save once, without inventing a special save path.

## Pre-gate AC status

| AC | Status | Independent basis |
|---|---|---|
| AC-M5D7J-001 | **PASS by contract; implementation pending** | Primary current/non-empty source resets only input, preserves all non-input semantics defensively, and produces exact `InputRepair` at `r+1`. |
| AC-M5D7J-002 | **PASS by contract; implementation pending** | Previous current/non-empty source produces one combined `PreviousPromotion|InputRepair` transform at `r+1`, explicitly avoiding `r+2` and existing input-preserving promotion. |
| AC-M5D7J-003 | **PASS by contract** | `0`, positive, and `long.MaxValue-1` boundaries are specified for both entries; `long.MaxValue` rejects before plan output. |
| AC-M5D7J-004 | **PASS by contract** | Both entries reject current-empty, metadata/binding recovery, invalid, unsupported, default, and reflection-invalid sources with the documented exception boundary. |
| AC-M5D7J-005 | **PASS by contract; implementation pending** | New proof kinds, original non-empty input, preserved snapshots, output default input, exact reasons, revisions, and all getter validation are required. |
| AC-M5D7J-006 | **PASS by contract; implementation pending** | Existing M5D7C methods/reason values/rows/tests are explicitly unchanged and must run as regression coverage. |
| AC-M5D7J-007 | **PASS by contract** | Allowlist is one existing Profile source plus one new EditMode fixture; no Unity/Input/file/save authority is permitted. |
| AC-M5D7J-008 | **BLOCKED — implementation gate** | No M5D7J execution evidence exists yet; focused new tests, unchanged M5D7C tests, full suites, and Luna post-review remain required. |

## Required implementation checks

Before recommending Verified, inspect that Terra:

- adds only the two public methods and private proof branches to `ProfileRecoveryPlannerV1.cs`;
- does not route the new methods through existing input-preserving `PlanPreviousPromotion` in a way that increments twice or retains the override;
- validates `source.Document`, current/non-empty input, source revision, and all nested snapshots before arithmetic/output;
- records the original non-empty `ProfileInputSnapshot` in the new plan proof and validates it against the source/output relationship;
- uses checked/guarded `r+1`, preserves long.Max overflow behavior, and does not fall back to default;
- proves every plan getter and reflection-mutated private field fails closed while existing M5D3/M5D7C behavior remains byte/exception-equivalent;
- keeps the Profile source free of M5D7I/Unity/Input System/IO/path/save/log/clock/RNG references and complies with the exact allowlist.

## Final recommendation

**PASS — P0=0, P1=0, residual P2=3.** Astra may advance the draft to Approved. This pre-gate approves only the pure binding-apply-failure transformation design; it does not approve implementation or claim actual Input System failure evidence, source selection, persistence, quarantine, or launch recovery.
