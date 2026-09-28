# VD-09 M5D7G profile launch observation adapter — Luna independent implementation review

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [Approved M5D7G profile launch observation adapter](../specs/work-contracts/2026-09-13-vd09-m5d7g-profile-launch-observation-adapter.md)
- Pre-gate: [Luna second-pass review](2026-09-13-vd09-m5d7g-contract-pregate.md)
- Evidence: [Terra implementation evidence](2026-09-13-vd09-m5d7g-implementation-evidence.md)
- Scope: independent source, test, XML, hash, authority, and allowlist review. Runtime, tests, contract, evidence, and README were not modified; Unity was not rerun.

## Verdict

**PASS — P0=0, P1=0, residual P2=3. Recommend Astra accept the implementation and advance M5D7G toward Verified integration.**

The implementation stays within the approved observation-only boundary. It observes exactly the three fixed sibling leaves in primary/previous/temp order, preserves all five non-default M5D7B decoder classifications as role-specific M5D7E candidates, isolates recoverable faults, and fails closed on invalid port protocol or unexpected/fatal exceptions. It does not select, repair, quarantine, save, mutate, log, notify, use Unity/clock/network/RNG authority, or introduce an observer lease.

## Independent source and test review

- `ProfileLaunchObservationBatchV1` is immutable in normal use and every public role getter calls complete validation. Default, role-swapped, duplicated-role, and unknown candidate states fail with `InvalidOperationException`.
- The BCL port uses `File.GetAttributes` for typed presence, maps only file/directory-not-found to Missing, opens one `FileAccess.Read`/`FileShare.Read` handle, reads the captured length to completion, and returns only after `using` closes the handle. `EndOfStreamException` and other recoverable read faults map that already-present role to Unreadable.
- `ValidateRead` rejects default/unknown completion, not-closed, null bytes, negative or over-limit counts, count/array mismatches, and short/zero-progress claims before decoder invocation. A genuine empty file satisfies `0 == 0 == byte[0].Length` and is decoded as Invalid. Locals are not published until all three roles complete, so protocol/unexpected/fatal failures cannot expose a partial batch.
- The focused tests exercise all five decoder classifications for every role, mixed role order and observation-only behavior, all four recoverable fault forms for each role, exact close/return sequencing, the complete-read protocol matrix, injected-buffer mutation, invalid paths, batch/candidate reflection corruption, unexpected/fatal propagation, and static authority restrictions. The binding-recovery fixture recalculates the payload SHA-256 after injecting malformed binding JSON, preserving the intended BindingRecoveryRequired classification.
- The implementation has no write/delete/copy/move/retry/fallback, clock, Unity, persistent-data-path, selection, transformation, notification, network, RNG, or `lock`/lease authority. The two added runtime/test files and their metas are the scoped implementation files; unrelated dirty-worktree changes were not attributed to M5D7G.

## Residual P2 precision notes

### P2-001 — BCL race cases are represented by seam exceptions, not live filesystem races

AC-M5D7G-004's focused fixture injects `IOException`/`EndOfStreamException` for present-then-disappear and premature-EOF behavior rather than orchestrating an actual BCL deletion/truncation race. The production implementation's catch boundary and full-read loop nevertheless give the required Unreadable result. A future regression may add a deterministic real-file race fixture, but this is not a source acceptance blocker.

### P2-002 — Some protocol/ordering proofs rely on source inspection and a proxy abstraction

The injected call log proves `probe → open/read → close → return` before the next role probe, while the source proves `FileAccess.Read`/`FileShare.Read` and the `using` close boundary. The seam does not expose actual `FileStream` arguments or an independent handle-state object. This is sufficient for the bounded synchronous implementation, but a future test could make those properties explicit without changing the public API.

### P2-003 — Reflection getter and role cross-product assertions are representative rather than exhaustive

The source validates all three getters through the same `Validate()` path, and the focused test directly exercises default, swapped, duplicate, and unknown states plus representative getter access. It does not independently invoke every malformed state through each getter or enumerate every fault/classification/role cross-product. Full suites are green and the shared validation logic is direct; this is evidence-strengthening work only.

## Evidence integrity

I independently parsed each supplied NUnit XML and recomputed SHA-256. Root attributes and test-case results agree: every case is Passed, with zero failed, skipped, or inconclusive cases.

| Suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused EditMode R3 | 10 | 10 | 0 | 0 | 0 | `29B2F11F934066668B0FFEEC8616CE92AB2A27489DFDAB097A11BF23970EC9DE` |
| Full EditMode | 622 | 622 | 0 | 0 | 0 | `5D9D25AC839F54FB4089F6E6E957CC13B97D19EE5F04BADA74CE1C207B89B3D2` |
| Full PlayMode | 576 | 576 | 0 | 0 | 0 | `6791C7DB50652569040CB33336823C25DDEA5F5042EDAC575A439DFBF0B91B99` |

The evidence file hash independently matches the supplied current record: `8B8904E4FB2EAE76C8CF02E7FBE6E5BDD82CACCD6759E5ABDD10F11C89FC8E1D`.

| Scoped file | SHA-256 |
|---|---|
| `ProfileLaunchObservationAdapterV1.cs` | `36663BA4B61F7A31B56BE07689132E04B94B70F3572F5CE3F70AA756FB16582E` |
| `ProfileLaunchObservationAdapterV1.cs.meta` | `8CEA63C429C974B654579EE713C066BB5736FA3454F59DE22845FC65170431F0` |
| `ProfileLaunchObservationAdapterV1Tests.cs` | `22DFFFCD03ADF1AA3E390540A2C7649AD9E3E321CF1310BDCDF7504A04F0B0C1` |
| `ProfileLaunchObservationAdapterV1Tests.cs.meta` | `BBE68988BB8B3735EC9560FF115B6C336EAC290AF7C0090E2449EF644E7CA25B` |

## AC status

| AC | Independent status | Basis |
|---|---|---|
| AC-M5D7G-001 | **PASS** | Exact three leaves, fixed roles/order, Missing without read, and isolated fixtures. |
| AC-M5D7G-002 | **PASS** | All five decoder classifications are preserved for all three roles; binding recovery is correctly rehashed. |
| AC-M5D7G-003 | **PASS with P2 precision note** | Mixed fixture proves order and temp remains a candidate; source proves one-handle read/close. |
| AC-M5D7G-004 | **PASS with P2 race note** | Every role has probe/read/disappear/EOF recoverable coverage; live BCL race is not separately orchestrated. |
| AC-M5D7G-005 | **PASS** | Exact protocol, count, empty-file, close/return, and no-partial-batch behavior are tested and source-confirmed. |
| AC-M5D7G-006 | **PASS** | Injected source-buffer mutation cannot alter returned decoded data or repeated getter result. |
| AC-M5D7G-007 | **PASS** | Invalid root/path forms fail before port calls; normalization and fixed leaves are bounded. |
| AC-M5D7G-008 | **PASS with P2 getter note** | Default/swapped/duplicate/unknown/reflection and unexpected/fatal paths fail closed; shared getter validation is direct. |
| AC-M5D7G-009 | **PASS** | Source and static tests prove exact names, BCL presence/read-only reads, no mutation/retry/clock/Unity/atomicity/lease claim. |
| AC-M5D7G-010 | **PASS** | Focused 10/10, full EditMode 622/622, and full PlayMode 576/576; all failure-state counts are zero. |

## Severity summary and recommendation

- P0: none.
- P1: none. The pre-gate full-read ambiguity is resolved by `CompletedAndClosed` plus `ExpectedLength`, `BytesRead`, and `Bytes.Length` equality checks, including explicit empty-file semantics.
- P2: three evidence-precision notes above; none requires runtime or contract changes for acceptance.

**Recommendation: PASS; Astra may proceed with M5D7G acceptance/Verified integration, while retaining the three P2 notes as optional follow-up evidence improvements.**
