---
status: Partial review — focused scope only
---

# M5D7Q-C1 Luna R7 partial independent review

- Date: 2026-09-28
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: current C1 source, bounded result/proof reflection invariants, happy resume, and focused EditMode evidence
- Source SHA-256 (`ProfileResetDiskTransactionV1.cs`): `4485DF37480B6032FF64BC7B47707F1CB08F9621F324F06B1DB86AAFAE29BC5D`
- Focused evidence: `artifacts/c1-r7-focused.xml`

## Focused result

The supplied Unity XML reports `19/19` passed, `0` failed, `0` inconclusive,
and `0` skipped for `ProfileResetDiskTransactionV1Tests` (EditMode; run
2026-09-28 00:38:52Z–00:38:57Z). The focused set covers stale identity,
invalid-byte archival, malformed barriers, prepared resume, interrupted Temp,
Busy identity retry, closed result pairs, scalar/root/role reflection
mutations, and coupled proof/archive mutation.

Static review found no remaining P0/P1 in this bounded scope. The R7 source
binds proof root/id/marker bytes/target and all three marker leaves, validates
archive role/name fingerprints, enforces active Primary/Previous/Temp role
mapping and target-only state, and validates the closed outcome/state pairs.
The destination-first archive pass and exact-primary resume path remain
consistent with the parent C1 contract.

## Deliberately open gates

This is not full C1 acceptance or integration. The following remain open:

- AC-M5D7QC1-003/006: complete fault and crash injection across every marker,
  move, flush, commit, post-validation, and interrupted-primary window.
- AC-M5D7QC1-008: full real separate-process contention/restart matrix using
  the fixture, beyond the focused Busy case.
- AC-M5D7QC1-009: broader output/proof mutation coverage beyond the current
  bounded reflection cases.
- Parent cutover work: UI, memory/settings/input application, receipt,
  barrier removal, scene/gameplay behavior, and their verification gates.

The review does not authorize implementation integration or change the C1
document's Approved status (implementation/verification pending); Astra remains
the sole approver. Approval preceded implementation, not this partial review.

## Correction record

An earlier R4 accessibility P0 claim was withdrawn after rereading the current
source showed the affected façade and methods are `internal`; the focused host
later passed. An earlier Unity invocation using the wrong executable/options
and an incorrect `qa/results` XML path are invalid evidence and are not used
here. This record uses only the provided `artifacts/c1-r7-focused.xml` path.

Traceability: `REQ-M5D7QC1-001..007`; focused evidence primarily covers
`AC-M5D7QC1-001/002/004/005/007/009`.
