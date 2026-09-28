---
status: Review
---

# M5D7Q-C1 final Luna evidence review

- Date: 2026-09-28
- Role: Luna independent post-implementation evidence review
- Scope: C1 disk transaction runtime, checkpoint/proof/result tests, actual
  child-process crash matrix, and Profile regression evidence
- This is an independent review record, not Astra acceptance or integration.

## Frozen source identity

The reviewed runtime source hashes are:

- `ProfileResetDiskTransactionV1.cs` SHA-256
  `90EFBE7A71C20241BD9CF792DE42BFD1B2142BCCB7AD2A146215D62ECC194743`
- `ProfileAtomicSaveServiceV1.cs` SHA-256
  `80DDBF925C74A8A81DD3B3950F25B3843D76B81F62ED942F3F3585CFB52B3F87`
- `ProfileNewGameResetServiceV1.cs` SHA-256
  `2E1DCE184495D4AABAC135AFC6910121A94D69498A85669A7D7EA1BD5E554098`

## Evidence runs

The runs are intentionally kept separate:

| Run | Scope | XML SHA-256 | Result |
|---|---|---|---|
| R12 | Non-process C1 adversarial/checkpoint/proof/result/save-fault suites | `D48BCB48B5C880D42301AA0E3F9EF480AC087AF7303845ED204E95D45A15CA1E` | 212/212 passed; failed/skipped/inconclusive 0 |
| R13 | Actual child-process crash/restart at all 51 checkpoints | `01F6061BB55971120626BE731A6C6BD8E2CB039AFEFD7371CD7AC59B2DCFB4FE` | 51/51 passed; failed/skipped/inconclusive 0 |
| R14 | Profile regression suite | `2F09B383BAF2A3289A44A56B454449F9E3692B85095309AE6F4E1B230F905320` | 175/175 passed; failed/skipped/inconclusive 0 |

R13's outer wrapper reported a premature 120-second timeout, but the actual
Unity Editor run continued to completion: the retained log reports 51/51,
exit code 0, and no live worker remained. The XML is therefore the authoritative
test result for R13; the wrapper timeout is retained as operational history,
not converted into a failure or omitted.

R12 fixture counts were independently checked as 50 adversarial, 78 checkpoint,
12 marker-fault, 10 proof, 11 result, 32 save-fault, and 19 original focused
tests. R13 contains all 51 closed checkpoints. R14 contains the 175 Profile
regression cases. XML inspection found no failed, skipped, or inconclusive
test-case nodes in any run.

## AC trace and independent assessment

- **AC-M5D7QC1-001:** R12 adversarial and focused suites cover confirmation
  identity freshness, stale mutation, and no-mutation rejection.
- **AC-M5D7QC1-002:** R12 adversarial/marker suites and R13 pre-publication
  process windows cover canonical marker publication, collision, malformed and
  unsupported marker handling.
- **AC-M5D7QC1-003:** R12 marker-fault/checkpoint suites plus R13 checkpoints
  1–24 cover marker durability, old-leaf archive ordering, preservation, and
  restart inference.
- **AC-M5D7QC1-004:** R12's 50-case adversarial suite and the original focused
  tests exercise all-three-leaf Missing/valid/invalid/unsupported/unreadable
  classifications, exact old/default byte rows, archive completion, ordinary
  writer blocking, and fail-closed result behavior. The exact-default old-row
  assertions are source-visible in the focused/adversarial fixtures. (The
  interrupted-default and first-save checkpoint matrix is AC-006 below.)
- **AC-M5D7QC1-005:** R12 proof/save-fault tests cover exact default first-save,
  actual issuing-lease identity, ordinary-writer rejection, and no duplicate
  persistence core.
- **AC-M5D7QC1-006:** R12's 78 checkpoint and 32 save-fault cases cover
  recoverable IO/interruption semantics, including interrupted-default
  preservation and first-save commit boundaries; R13 supplies actual process
  termination and restart at every closed checkpoint.
- **AC-M5D7QC1-007:** R12 adversarial/result suites cover collision,
  byte-identity, missing/both/neither copies, unreadable archive, and closed
  observation/proof classification cases. The runtime source also contains
  explicit root/ancestor/reparse containment guards, independently reviewed
  here; the executed XML does not establish a dedicated Windows reparse-point
  fixture case. No actual reparse-point test is claimed by this record.
- **AC-M5D7QC1-008:** R13 independently proves real separate-process Busy
  blocking, exact child termination, lock release, and distinct-process resume
  for all 51 checkpoints.
- **AC-M5D7QC1-009:** R12 proof/result/reflection/static-boundary tests and R14
  regressions cover defensive rows, one-consumer proof integrity, no authority
  widening, and preservation of existing Profile behavior.

## Findings

No P0 or P1 finding remains in the reviewed C1 source/evidence set. The
previously open concerns—checkpoint forwarding, actual process coverage,
archive classification/row invariants, failure-stage fidelity, proof
single-consumption, and issuing-lease binding—are represented in the frozen
source and the executed R12/R13 evidence above. Dedicated Windows reparse-point
execution is not present in these XMLs; that is an evidence-scope limitation,
not a fabricated pass claim. The static containment audit found no source
defect, while any requirement for a separately constructed reparse fixture
would remain an explicit additional gate for Astra.

This review does not itself change the Approved/Verified state or grant C2,
memory, UI, scene, or product-destination authority. Astra must perform the
final integration decision under the C1 delivery gate.
