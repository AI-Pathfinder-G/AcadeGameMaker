# VD-09 M5D7F Profile Quarantine Adapter — Terra implementation evidence

Status: **Verified — Astra accepted after Luna R2 PASS (P0=0, P1=0); residual P2=3 are non-blocking test-precision notes.**

## Scope and ownership

- Implementer: Terra.
- Contract: [M5D7F profile quarantine adapter](../specs/work-contracts/2026-09-13-vd09-m5d7f-profile-quarantine-adapter.md), Verified.
- Independent review: [Luna R2 implementation review](2026-09-13-vd09-m5d7f-luna-independent-review.md), PASS.
- Pre-gate: [Luna second pre-gate](2026-09-13-vd09-m5d7f-contract-pregate.md), PASS.
- Scoped files: `ProfileQuarantineServiceV1.cs`, its `.meta`, `ProfileQuarantineServiceV1Tests.cs`, its `.meta`, and this evidence record.  Verified M5D3–M5D7E files were not modified.

## Requirement and acceptance evidence

| Contract coverage | Implementation / test evidence |
| --- | --- |
| REQ-M5D7F-001 | Engine-free typed BCL operation port; `File.GetAttributes` classifies only missing-file/directory as missing, exclusive read, SHA-256 and one `File.Move`. |
| REQ-M5D7F-002 | Candidate/reason validation precedes lease and I/O; source is re-decoded classification-only; the root lease is normalized and `OrdinalIgnoreCase` ref-counted. |
| REQ-M5D7F-003 | Exact fixed leaves, UTC/hash8/nohash recovery names, smallest checked collision ordinal, and no fallback/delete/copy/retry paths. |
| REQ-M5D7F-004 | Move/post state probe and readable full-digest proof produce the closed result matrix; recoverable I/O/access/security exceptions map to explicit stages while unexpected/fatal faults escape. |
| REQ-M5D7F-005 | Immutable result validation constrains role/reason, state, destination, suffix, ordinal and outcome/stage combinations. |
| AC-M5D7F-001..003 | Real isolated invalid/unsupported/stale moves; exact bytes, UTC/hash8/nohash name and collision ordinal assertions. |
| AC-M5D7F-004..006 | Revalidation mismatch and injected missing/directory/allocation/move/post recoverable faults assert exact result rows and at-most-one move. |
| AC-M5D7F-007..008 | Invalid arguments are rejected before port calls; reflection-corrupted results fail closed; post-move byte tampering yields post-validation uncertainty. |
| AC-M5D7F-009..010 | Unexpected `InvalidOperationException` and `OutOfMemoryException` escape with lease cleanup; alias/different-root barrier coverage and source authority scan are included. |

## Post-review remediation (completed; fresh regression recorded below)

- Luna P1-001 is addressed by retaining private destination proof (normalized root, UTC token, optional unsupported version, and source-read digest proof). `Validate()` now reconstructs the reason-specific recovery leaf and rejects forged path, UTC, suffix, ordinal, root, unsupported-version, and readable-`nohash` mutations.
- Recoverable pre-move hash-port failures now return `FailedBeforeMove` at `DestinationAllocation`; malformed/non-lowercase/non-64-character port digests escape as unexpected faults and still release the lease.
- The focused tests now include those reflection/fault cases, typed source presence access, and a case-changing same-root waiter whose registration is observed before the active transaction is released.

## Scoped hashes

| File | SHA-256 |
|---|---|
| `ProfileQuarantineServiceV1.cs` | `CF73B2452B1F7839A346F13E65E4076F2B688F067DD2B66ED931F7D3810FF162` |
| `ProfileQuarantineServiceV1.cs.meta` | `07A30262C8FDEB170D25B8546556E17E9E6F7BDB349555BB00DCBD029A311ED5` |
| `ProfileQuarantineServiceV1Tests.cs` | `8FCDB5CE41671C4CC991C525F25043690674DB7BE74CAD1B35762D455CAEFDAA` |
| `ProfileQuarantineServiceV1Tests.cs.meta` | `F01112A5674D6217FF5FA888566898F2BDE751F5ACE904B32F5CA1FC2016FE16` |

## Root execution

All final runs used Unity `6000.6.0f1` after editor/licensing/entitlement and competing-process preflights passed. The first focused attempt stopped at a missing test helper compile error and is diagnostic history only.

| Run | Result | NUnit XML | SHA-256 |
|---|---:|---|---|
| Focused EditMode `ProfileQuarantineServiceV1Tests` R5 | 18 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7f-20260913/focused-editmode-r5.xml` | `264127DB457F2321FC94B36E76E6C7E20B7276EF0A547B5939D65D1363EB45DB` |
| Full EditMode R2 | 612 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7f-20260913/full-editmode-r2.xml` | `F28E332B12EE4D209F55740A13BCBE8E21D1D059856E3C613ACC0DE329DDAF08` |
| Full PlayMode R2 | 576 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7f-20260913/full-playmode-r2.xml` | `F2CDAB5FA842B6A06927A8F3573A331C0EE4A85AA3BA071D774A56AB84CDEB8B` |

## R4 focused execution attempt — licensing block

After the UTC-token correction, Terra's sandbox-session attempt stalled at `[Licensing::Module] Licensing is not yet initialized`, generated no NUnit XML, and was not retried. Astra subsequently stopped that orphaned sandbox process and reran the focused and full suites in the approved general Windows user session. The R5/R2 artifacts above are the final fresh regression results; the licensing attempt remains diagnostic history only.
