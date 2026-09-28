# VD-09 M5D7L profile launch preparation coordinator — Luna post-review

- Contract: [Approved M5D7L](../specs/work-contracts/2026-09-13-vd09-m5d7l-profile-launch-preparation-coordinator.md)
- Reviewer: Luna (independent; Astra owns final integration)
- Review date: 2026-09-13
- Compared with: M5D7L R2 pre-gate, Verified M5D7D/E/G/H/I/J/K boundaries, final coordinator source/tests/allowlist, and implementation evidence

## Evidence identity

The recorded final results were independently parsed. Every XML has the claimed count and zero failed, skipped, or inconclusive cases; the recorded SHA-256 values match the files on disk.

| Run | Result | SHA-256 |
|---|---:|---|
| Focused M5D7L R8 | 33/33 | `77eb28aca4a1098eb77bcf582a6d1eab05cb93bc19ea2c6e789627ef1d73e199` |
| Direct M5D7I regression | 8/8 | `eec8e0ee69e3673222ee741b89513bf533ad1e415562b9e8da473a8afdc9f6d9` |
| Full EditMode R2 | 663/663 | `406fb7915105354ba5daad344c883bd0acba00a08c49aae041ab2cb28b326315` |
| Full PlayMode R2 | 617/617 | `8064d2a96a7756d2421e07668366da492b4ccb0e3251e15393c8e37ec6692f52` |

Current scoped file hashes also match the evidence: coordinator `4f29227aad122e89cd71ea2e0447c2b8c7dced59b2616a06d587c5ea1618efc5`, coordinator meta `3775f07a8bd98f84aa0f87f77e446e3079d3b2b4dc884684eaa34e0e9f6159a6`, fixture `28bba8ce778e18536d111fa5e336a567ef18d6dbd1937e82f57f78f06be273c9`, fixture meta `a6c6bd9f8dd041d65ff05c7f4f82647da46fa93e578df8db85744f3bc85d661c`, and Profile friend declaration `89652373f2bb5b965eb2353b9708821decd19941094825566af584859fbd96f4`.

## Findings

### P1-001 — AC006/AC007 exact argument and exception-precedence proof is incomplete

The implementation now passes the expected normalized root, UTC, observation/selection, H candidate, and final document directly, and the token routes disposal through the actions port. However, the focused fixture does not prove all of the contract's required call arguments: `CoordinatorPort` records `KObservation`, `KSelection`, and `DDocument` but never asserts them; the H assertion checks only `HPrimary.Role`, not exact candidate content; and `ActionsPort` does not record the input/candidate passed to `Apply`. The test therefore cannot detect a future wrong-candidate, wrong-document, or wrong-selection call while still returning a valid typed result.

The hostile cleanup test also uses `Assert.Catch<Exception>` and does not assert that the original apply/programmer exception remains the propagated cause when an injected cleanup `Dispose` exception follows. That is the specific exception-precedence rule in AC-M5D7L-007 and the actions-port contract. The green XML proves execution, not these missing assertions.

Minimal fix: record and assert exact K observation/selection, D root/document, H candidate, and every Apply candidate/input; assert the original exception type (and no save) for cleanup-failure combinations.

### P1-002 — AC010 does not assert concurrent calls actually drain

`AC010_LeasesSerializeAliasesAllowDifferentRootsAndReleaseAfterFault` calls `Task.WaitAll(..., 2000)` but ignores its Boolean completion result. A timeout can therefore leave a worker blocked while the test proceeds based only on `MaxActive`; the test does not prove both same-root workers and both different-root workers completed, nor that the lease was released on each fault path. The final single call is useful but does not close the missing completion assertion.

Minimal fix: require `Task.WaitAll` to return true (or await both tasks with a bounded assertion) before checking concurrency and subsequent progress.

### P2-001 — Source-dependent save-row validity is split across projection and token validation

`ProfileLaunchSaveProjectionV1.Validate()` validates the abstract projection row, while `PreparedProfileLaunchV1.ValidateSavePath()` supplies source/reason context and rejects a D/H kind mismatch. This is safe for returned tokens and is covered by the mismatch test, but standalone blocked/failed projection values cannot encode their source context. Keep the context check centralized and covered; do not expose these projections as a general persistence API.

### P2-002 — Coordinator-specific known-override forwarding is indirect

The direct M5D7I regression covers a real known override, and the coordinator covers real empty/default and stale-ID recovery paths. The coordinator fixture does not independently assert a real known-valid override being forwarded unchanged. Source inspection shows the direct selection input is passed, so this is residual coverage debt rather than a correctness blocker.

## Requirement and acceptance matrix

| ID | Status | Independent assessment |
|---|---|---|
| REQ-M5D7L-001 | PASS | Source performs G→E→K→I→optional J→D/H under the same-root lease. |
| REQ-M5D7L-002 | PASS | Fresh disabled candidate and selected input are validated before retention. |
| REQ-M5D7L-003 | PASS | Poisoned candidate is disposed, source-specific J runs once, and a distinct second candidate is proven. |
| REQ-M5D7L-004 | PASS | K denial suppresses D/H; save kind is revalidated against source/reason context. |
| REQ-M5D7L-005 | PASS | Typed preservation/save failures return pending, hub-ready in-memory evidence; protocol/fatal paths throw. |
| REQ-M5D7L-006 | CONDITIONAL | Ownership implementation is correct, but exact Apply identity and cleanup-cause assertions are incomplete. |
| REQ-M5D7L-007 | CONDITIONAL | Projection/reflection and hostile rows execute, but exact argument/exception precedence proof is incomplete. |
| REQ-M5D7L-008 | PASS | No router/map-enable/live-scene/hub/notification/active-run authority or forbidden dependency was found. |
| AC-M5D7L-001 | PASS | Default, current-empty, and current stale-input paths are covered and full suites are green. |
| AC-M5D7L-002 | PASS | Both primary decode-recovery classifications dispatch H only; ordinary routes are separately covered. |
| AC-M5D7L-003 | PASS | Primary/previous preservation and input-repair rows, including distinct candidates and no double increment, are covered. |
| AC-M5D7L-004 | PASS | Temp, Primary, and Previous preservation stops suppress persistence and retain typed pending evidence. |
| AC-M5D7L-005 | PASS | All pre-commit and post-commit typed ordinary/recovery failure rows execute with pending hub-ready results. |
| AC-M5D7L-006 | CONDITIONAL | Call order/counts are covered, but exact argument identity is not fully asserted. |
| AC-M5D7L-007 | CONDITIONAL | Hostile cleanup/reuse/reflection rows run, but original-exception precedence and all Apply identity are not asserted. |
| AC-M5D7L-008 | PASS | Take/dispose exact-once behavior and disabled maps are covered. |
| AC-M5D7L-009 | PASS | Projection/token field reflection mutations and representative legal rows execute. |
| AC-M5D7L-010 | CONDITIONAL | Alias serialization and different-root overlap execute, but worker completion is not asserted. |
| AC-M5D7L-011 | PASS | Real generated actions/filesystem paths and direct M5D7I regression are green; known override forwarding remains P2. |
| AC-M5D7L-012 | FAIL | XML outcomes are green, but Luna cannot certify AC012 while P1-001/P1-002 remain. |

## Verdict

**FAIL — P0=0, P1=2, P2=2.** The runtime implementation is directionally and authoritatively bounded, and all recorded focused/dependency/full XMLs are clean with matching hashes. Acceptance is blocked by meaningful-test gaps in exact call-argument/Apply identity, cleanup exception precedence, and lease-worker completion. Do not mark M5D7L Verified until those assertions are added and the focused/full evidence is refreshed.

## R2 remediation review

The fixture-only remediation was independently re-read. It now asserts normalized root/UTC, full observation and selection equality at K, exact D document/root, exact H candidate/root, every Apply input and candidate identity, exact disposal identities, original exception precedence when cleanup also throws, and successful completion/non-faulted state for all bounded lease tasks. The runtime source remains unchanged from the prior review; its token disposal, source-bound save-kind proof, observation/selection revalidation, and authority boundaries remain sound.

### R2 evidence identity

All refreshed XMLs were independently parsed and hashed. Counts match the evidence and every run reports zero failed, skipped, and inconclusive cases.

| Run | Result | SHA-256 |
|---|---:|---|
| Focused M5D7L R9 | 33/33 | `2bc15b1b74656070d3359c21dfaac219d019ff54a86bbd9ff1a8f327ad9b0d51` |
| Direct M5D7I regression | 8/8 | `eec8e0ee69e3673222ee741b89513bf533ad1e415562b9e8da473a8afdc9f6d9` |
| Full EditMode R3 | 663/663 | `108a73001ec3ea9fbc15c587cd93b439e8f35173f9466d89db6c805c76f6d248` |
| Full PlayMode R3 | 617/617 | `40bfa9aaf4de9cbb820b49df2854dcf266fff1b6e5482aa5e94ae19ed7b2f8dc` |

The refreshed fixture hash is `81d9d630d026ae0d62f23745ab2b40987c7e671e056f040b82b6bb527c9c1ff1`; runtime, metas, and the single Profile friend declaration remain the allowlisted identities recorded in R1.

### P1 closure

- **P1-001 — CLOSED.** The focused fixture now proves exact K observation/selection and normalized root/UTC, D document/root, H candidate/root, and all actions-port Apply input/candidate identity. It also proves exact disposal identity and that cleanup failure does not replace the original apply failure.
- **P1-002 — CLOSED.** AC010 now asserts `Task.WaitAll(...) == true` and both same-root/different-root workers are completed and non-faulted before subsequent progress checks. The lease fault paths therefore have an observed drain/release proof.

### R2 verdict

**PASS — P0=0, P1=0, residual P2=2.** The prior P1 findings are closed by the bounded fixture changes and clean focused/full reruns. Residual P2 items are the intentionally split source-context validation of standalone save projections and the indirect coordinator-specific known-override forwarding coverage; neither grants new authority or blocks M5D7L acceptance. Recommend Astra accept the Luna post-review and proceed with integration; Luna does not change the contract's Verified status.
