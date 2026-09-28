# VD-09 M5D7L profile launch preparation coordinator — Luna contract pre-gate

- Contract: [Draft M5D7L](../specs/work-contracts/2026-09-13-vd09-m5d7l-profile-launch-preparation-coordinator.md)
- Reviewer: Luna (independent; Astra owns approval)
- Review date: 2026-09-13
- Compared with: VD-09 platform/load-recovery requirements, SYSTEM-CONTRACTS, P1 load-recovery approval, and Verified M5D7D/E/G/H/I/J/K boundaries and source APIs

## Verdict

**FAIL — P0=0, P1=1, residual P2=2.** Do not advance this draft to Approved until the profile-port/test-allowlist seam is made explicit.

The orchestration sequence is directionally sound and does not grant router, scene, path-source, notification, or active-run authority. The blocker is implementability of the required deterministic profile-port tests under the exact allowlist: several dependency result constructors/types are internal to `AcadeGameMaker.Profile`, while the new PlayMode test assembly is not a Profile friend.

## Boundary comparison

- VD-09 requires valid primary → valid previous → default selection, stale-temp exclusion, preservation before save, input-only recovery at `r+1`, atomic persistence, and hub entry with verified in-memory state after typed save failure. M5D7L delegates each responsibility to the Verified G/E/K/I/J/D/H units in the correct conceptual order.
- M5D7D ordinary `Save` accepts the final current-compatible document. M5D7H's internal `SaveInputRecovery` is the only correct path for an originally selected primary with metadata/binding decode recovery. The draft dispatch table correctly forbids both simultaneously and makes K denial suppress all D/H calls.
- M5D7I's result contract supports `DefaultsReady`/`OverridesApplied` + `Retain` and `RecoveryRequired` + `Discard`. M5D7J's source-specific methods accept the selected current decode result and produce the already-incremented repair plan, so the draft's no-double-increment rule is feasible.
- `GameInputActions` is a public generated wrapper whose constructor creates disabled maps and whose `Dispose` destroys the asset. The proposed sealed token can own one reference, transfer it once, and dispose it on all pre-return failure paths without touching `InputRouter`.
- A private L registry can serialize same-root preparations before observation while permitting different roots. The contract explicitly leaves cross-process/direct-writer races outside the startup-owner invariant; this is an honest boundary, not a P1 contradiction.

## P0/P1 findings

### P1-001 — Profile port result types are not constructible by the allowlisted PlayMode tests

The draft permits one injected profile port for M5D7G, M5D7K, M5D7D, and M5D7H and then requires deterministic tests for K denial at every stop, every D/H committed/failure/uncertain row, invalid typed results, revision mismatch, and following-call behavior (REQ-M5D7L-001/004/005/007; AC-M5D7L-004..007/009). However:

- `ProfileLaunchObservationBatchV1` and `ProfileLaunchPreservationResultV1` have internal constructors in `AcadeGameMaker.Profile`.
- `ProfileAtomicSaveResultV1` has an internal constructor.
- `ProfileInputRecoverySaveResultV1` and its outcome enum/getters/validation are internal.
- The new test assembly named by the allowlist, `AcadeGameMaker.Input.Unity.PlayMode.Tests`, is a friend of `AcadeGameMaker.Input.Unity`, but is not a friend of `AcadeGameMaker.Profile`. The allowlist permits adding only `InternalsVisibleTo("AcadeGameMaker.Input.Unity")` to Profile's existing `AssemblyInfo.cs`; adding the PlayMode test assembly as a Profile friend would exceed the stated allowlist.

Therefore a naïve profile-port interface returning the dependency result types cannot compile deterministic fake rows in the required PlayMode tests. Real filesystem calls cannot replace this seam for all stage/protocol/fatal/K-denial cross-products without making the tests nondeterministic or violating the contract's “instance ports” intent.

Required minimal amendment: require the new coordinator file to define coordinator-owned, closed profile-port projections for G/K/D/H (including the internal H result), with explicit fields/invariants for every mapped outcome/stage/state/revision. The production adapter may call Profile internals through the single permitted Profile→Input.Unity friend and project validated values; the PlayMode tests use the existing Input.Unity→tests friend to construct the projections. Alternatively the contract must explicitly expand the Profile friend allowlist, but that is a materially broader change and is not recommended.

## P2 findings

### P2-001 — Actions-port ownership-on-throw protocol is underspecified

The token rules require exact-once disposal for every created candidate, including first/second creation or apply failures, but the contract gives no method signatures or ownership rule for an actions-port `Create` that throws, `Apply` that throws after candidate creation, or `Dispose` that itself fails. Define a minimal seam such as `Create` returning an owned candidate or guaranteeing no candidate escaped on throw, and state whether disposal exceptions propagate after the best-effort exact cleanup. This is a testability precision issue; the ordinary generated constructor path is usable.

### P2-002 — Scope is a large cross-product for one coordinator file

The 12 ACs combine filesystem observation/quarantine/save, Unity candidate lifecycle, recovery transformation, lease concurrency, result reflection, and full regressions. The responsibilities are bounded and the sequence is coherent, but implementation review should keep coordinator-local projections/helpers private and avoid silently turning this into a launch/hub/router adapter. If the test matrix becomes unreviewable, split future work after this contract rather than widening its authority.

## Acceptance readiness

| Criterion | Pre-gate status | Reason |
|---|---|---|
| AC-M5D7L-001 | CONDITIONAL | Sequence and generated-actions feasibility are sound; profile-port projection seam is needed for deterministic tests. |
| AC-M5D7L-002 | CONDITIONAL | H/D dispatch rules are compatible with Verified D/H; typed result projection is unspecified. |
| AC-M5D7L-003 | PASS by design | M5D7I/J current/non-empty and source-specific no-double-increment path is implementable. |
| AC-M5D7L-004 | CONDITIONAL | K denial semantics are clear, but deterministic K rows require the seam fix. |
| AC-M5D7L-005 | CONDITIONAL | D/H typed mapping is clear, but H result is internal and unavailable to direct test fakes. |
| AC-M5D7L-006 | CONDITIONAL | Port call logs are feasible after projection methods are specified. |
| AC-M5D7L-007 | CONDITIONAL | Cleanup/invalid result coverage needs explicit projection and ownership protocol. |
| AC-M5D7L-008 | PASS by design | One-shot transfer and disabled-map ownership are feasible with generated `GameInputActions`. |
| AC-M5D7L-009 | CONDITIONAL | Closed token matrix is described, but concrete field/projection types are not. |
| AC-M5D7L-010 | PASS with test seam note | Private ref-counted L lease is feasible; cross-process exclusion is expressly out of scope. |
| AC-M5D7L-011 | PASS by boundary | Real isolated paths can be caller-supplied; no persistent path/router/scene authority is granted. |
| AC-M5D7L-012 | BLOCKED | Execution is appropriately downstream of Approved implementation and independent review. |

## Recommendation

Add the coordinator-local typed profile-port projections and their closed validation/ownership rules, without adding a second Profile friend or changing Verified dependency APIs. Re-run the pre-gate after that bounded amendment. Until then, **P0=0/P1=1: FAIL**; no implementation approval is recommended.

## R2 amended-contract review

The amended Draft M5D7L was re-read against the original P1/P2 findings and the current Verified M5D7D/E/G/H/I/J/K API boundaries. The amendment is bounded to the already allowlisted coordinator file, the single Profile-to-Input.Unity friend, and the existing Input.Unity-to-PlayMode-tests friend.

### Resolution of prior findings

- **P1-001 — RESOLVED.** The coordinator now owns internal, immutable, closed projections for observation (G), preservation (K), and unified save outcomes (D/H). Production validates the Profile result first, maps it through the sole permitted `InternalsVisibleTo("AcadeGameMaker.Input.Unity")` edge, and reconstructs only the required Profile observation batch. The PlayMode friend constructs only coordinator projections, so it does not require a new Profile friend and can deterministically cover K denial, every D/H outcome, protocol corruption, revision mismatch, and fatal/cleanup paths. The projections retain no Profile result, bytes, path, exception, callback, or mutable collection and revalidate from every getter; defaults/unknown/reflection-corrupted values fail closed.
- **P2-001 — RESOLVED.** The actions port now has explicit `Create`/`Apply`/`Dispose` ownership semantics: creation either throws before ownership exists or returns one fresh disabled candidate; apply must return the exact validated result for that object; dispose is one attempt for that exact object, propagates immediately, is never retried, and cannot be hidden by cleanup. The token-transfer and pre-return cleanup rules are consequently testable by identity without changing the generated-actions API.
- **P2-002 — MITIGATED, not a gate.** AC009 now requires representative pairwise behavioral coverage plus exhaustive legal projection/result rows rather than an unbounded Cartesian product. The remaining breadth is appropriate for this coordinator's safety boundary, provided implementation helpers remain private and no launch/router/path authority is added.

### Remaining assessment

No new P0 or P1 contradiction is present. The projection fields and closed-row validation are sufficiently specified to implement without widening the allowlist or inventing Profile authority. The “one internal overload” wording leaves ordinary public entry behavior to implementation, but the production/test seam, ownership, and exception rules are concrete enough for a post-implementation review rather than a contract blocker. Cross-process writers remain explicitly outside the startup-owner invariant, consistently with the Verified save boundary.

| Criterion | R2 pre-gate status | Reason |
|---|---|---|
| AC-M5D7L-001 | PASS by design | Exact observe/select/preserve/apply sequence remains bounded and mapped to Verified units. |
| AC-M5D7L-002 | PASS by design | Typed local save projection preserves D/H kind, stage, state, revision, and failure distinctions. |
| AC-M5D7L-003 | PASS by design | Current non-empty input repair uses J once with no double increment; default/empty input stays ordinary. |
| AC-M5D7L-004 | PASS by design | K denial is a closed projection row and suppresses all D/H calls. |
| AC-M5D7L-005 | PASS by design | D/H outcomes are validated before mapping and are deterministically constructible through the coordinator seam. |
| AC-M5D7L-006 | PASS by design | Fresh instance ports and identity-based action rules make call order/counts testable. |
| AC-M5D7L-007 | PASS by design | Projection, ownership, fatal-exception, and cleanup rules fail closed. |
| AC-M5D7L-008 | PASS by design | One-owner disabled-actions token and exact transfer/disposal semantics remain feasible. |
| AC-M5D7L-009 | PASS by design | Representative pairwise rows plus exhaustive legal projection/result and reflection-corruption rows are implementable. |
| AC-M5D7L-010 | PASS by design | Same-root ref-counted launch lease and different-root progress remain explicit; no FIFO/cross-process overclaim. |
| AC-M5D7L-011 | PASS by boundary | Caller-supplied paths and internal ports add no router, scene, notification, or persistence authority beyond delegated units. |
| AC-M5D7L-012 | DOWNSTREAM | Requires Terra implementation and Luna execution review; this pre-gate does not claim runtime evidence. |

## R2 verdict

**PASS — P0=0, P1=0, residual P2=1 (scope breadth only).** The amended contract closes the exact Profile-friend/constructibility blocker and the action ownership ambiguity without changing Verified dependency APIs or expanding authority. Recommend Astra advance the bounded draft to Approved; do not change contract status in this independent pre-gate.
