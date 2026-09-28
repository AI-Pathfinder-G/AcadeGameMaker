# VD-09 M5D7H primary input-recovery atomic save — Luna independent post-review

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7H primary input-recovery atomic save extension](../specs/work-contracts/2026-09-13-vd09-m5d7h-primary-input-recovery-atomic-save.md)
- Pre-gate: [Luna second-pass PASS](2026-09-13-vd09-m5d7h-contract-pregate.md)
- Evidence: [Terra implementation evidence](2026-09-13-vd09-m5d7h-implementation-evidence.md)
- Scope: independent source, test, evidence, XML, dependency, authority, and allowlist review. Runtime, tests, contract, evidence, and README were not modified; Unity was not rerun.

## Verdict

**CONDITIONAL FAIL — P0=0, P1=1, P2=3. Do not mark M5D7H Verified until P1-001 is repaired and rerun evidence is reviewed.**

The implementation is otherwise a narrow additive extension of the Verified M5D7D adapter. It requires a Primary/Decoded M5D7E candidate, derives the M5D7C input-repair plan before lease/I/O, writes and revalidates temp, performs fresh semantic source-plan validation, and uses exactly one `File.Replace(temp, primary, previous, true)`. Ordinary `Save`, the public M5D7D ABI, existing stage/state enums, and the one normalized-root lease/port remain structurally unchanged. The binding-recovery path correctly avoids the unavailable `InputCompatibility` getter and compares exact recovery reasons plus plan output.

## P1 finding

### P1-001 — Successful recovery result does not prove the derived committed revision

`ProfileInputRecoverySaveResultV1.Validate()` accepts a successful `CommittedRecoveryReplacement` row whenever `_revision >= 0` (runtime lines 21–24). The Approved contract requires the committed revision to equal the derived M5D7C repair revision. The result stores no expected/source/plan revision proof and therefore cannot reject a structurally valid but wrong positive revision. For example, after a valid success result for source revision `5`, reflection can change private `_revision` to `999`; `Validate()` and every internal getter still accept the result because the success branch checks only nonnegativity and exact outcome/stage/file-state values.

This violates the result invariant and AC-M5D7H-009's reflection fail-closed requirement even though the normal constructor passes `plan.ResultRevision`. The focused test mutates `_revision` to `-1`, but does not test a wrong nonnegative revision, so the green focused result does not close this gap.

Minimal fix: retain sufficient private structural proof to validate the expected repair revision (for example source/result revision proof with the exact `result == source + 1` relation, or a private expected result revision carried from the validated plan) and reject any mismatch from every getter/`Validate()`. Add a positive wrong-revision reflection test and applicable `0`, arbitrary positive, `long.MaxValue-1`, and overflow cases. Do not retain source bytes, paths, decoder trees, or mutable data. The normal transaction’s plan derivation and file postcheck should remain unchanged.

## P2 findings and test-precision notes

### P2-001 — Observed long.MaxValue overflow is not directly focused-tested

The implementation delegates plan derivation before lease/I/O, and M5D7C rejects a valid recovery source at `long.MaxValue`; the focused suite tests a fresh changed primary at `long.MaxValue`, not an initially observed Primary/Decoded recovery candidate at that revision. Add a direct pre-I/O overflow assertion with zero port calls. This is required evidence precision, not a second source defect.

### P2-002 — Result/reflection and mutation coverage is representative

The source result validator correctly rejects unknown outcome/stage, `NotProbed`, and the `-1` success mutation, while tests cover representative rows and getter access through reflection. The suite does not enumerate every successful/failed row field combination, every getter after each corruption, or caller/source byte mutation after candidate construction. Add these cases in the repair rerun, especially the wrong-positive revision from P1-001 and source-candidate ownership.

### P2-003 — Cross-entry lease evidence is bounded to one recovery waiter and static ABI checks

AC008's barrier correctly holds an ordinary save, waits for an alias recovery caller to register (`RefCount == 2`), verifies a different root can proceed, then drains and checks an empty registry without assuming Monitor FIFO/wake order. The focused test does not use multiple recovery waiters or independently compare ordinary public ABI signatures; the unchanged M5D7D focused regression and full suites provide strong regression evidence. Keep this as optional precision follow-up after P1 repair.

## Source and authority review

- **Entry proof:** `ValidateRecoveryArguments` validates the candidate before `Acquire`, requires exact Primary role/Decoded kind and one of the two input-recovery classifications, and rejects wrong-role/kind/missing/unreadable/current/invalid/unsupported/default/reflection-invalid candidates before I/O.
- **Fresh identity:** after temp write/flush/close/re-read, the fresh primary must retain classification and reasons; metadata recovery additionally compares available `InputCompatibility`; binding recovery does not call that unavailable getter. M5D7C source/result revisions, reasons, and canonical repair bytes must match, including fresh overflow rejection.
- **Transaction:** temp is write-through and closed before read validation; source primary is read/validated before commit; recovery never calls `Move`; exactly one replace is attempted; old primary bytes are compared byte-for-byte with post-replace previous; replace/postcheck faults produce uncertainty without retry/rollback/delete/fallback.
- **Failure boundary:** recoverable precommit faults map to the exact typed precommit stage; replace/postcheck faults map to uncertainty; invalid typed presence and unexpected/programmer/fatal faults escape while `finally` releases the shared lease. All returned recovery results are required to have all three states probed and non-`NotProbed`.
- **Lease/ABI:** recovery calls the same `Acquire`/`Release` and `IProfileAtomicSaveFileOperations` as ordinary `Save`; the alias/waiter barrier and different-root progress are exercised. The additive rule leaves existing public enums/result/ordinary Save/port semantics intact, supported by the unchanged M5D7D focused regression and full EditMode/PlayMode runs.
- **Authority/allowlist:** no Unity, persistent-data-path, clock, selection, quarantine, input apply, logging, notification, network, RNG, recovery move, or forbidden file mutation is introduced. Only the permitted M5D7D runtime extension, new focused test/meta, contract/report/evidence changes are attributed to M5D7H; unrelated dirty-worktree files were not considered.

## Evidence integrity

I independently parsed each supplied NUnit XML and recomputed SHA-256. Every root result is Passed with zero failed, skipped, or inconclusive cases, matching Terra's evidence. The earlier focused compile diagnostic is not an acceptance result.

| Suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused M5D7H EditMode | 25 | 25 | 0 | 0 | 0 | `A20EE0660EA87B688CC27E4E68655B901953B16CA4304452D51B7B36CCB5A981` |
| Unchanged M5D7D focused regression | 29 | 29 | 0 | 0 | 0 | `62C511B2298D71296421884DEE01E41F7E535AD807E95F840D9B17970C68643D` |
| Full EditMode | 647 | 647 | 0 | 0 | 0 | `569B36ED08B82944B87A1A4D2F6CC62088B1034F6524894C449ACF21B468900F` |
| Full PlayMode | 576 | 576 | 0 | 0 | 0 | `ACCC789C7B234640D847FF7CB773B7BEC6F434083B200B28E95806A3F358B960` |

The implementation-evidence file hash independently matches the supplied anchor: `E86A3D02655D6208A275CA8F562CE09BE68428F6F62A3ED8687EC39EC0D2B58D`.

| Scoped file | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs` | `E7A0203C76E1E4A9AA41DDA06ACE9DBE63F8A50E51781E166E60AEE0B23D6592` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileInputRecoveryAtomicSaveServiceV1Tests.cs` | `082FB000EBE436D4DCE4353FB8E0D1F30901682D7D6099161F6DFC9DED6A4B51` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileInputRecoveryAtomicSaveServiceV1.cs.meta` | `CB0431CE7E282125DBE4E78D8F30285212BF9FE85C0522798B708D4ED675D5E3` |

## AC status

| AC | Independent status | Basis |
|---|---|---|
| AC-M5D7H-001 | **CONDITIONAL — P1-001** | Normal metadata replacement and exact previous bytes pass, but success revision proof is forgeable. |
| AC-M5D7H-002 | **PASS with P2 coverage note** | Binding recovery succeeds without unavailable getter; semantic non-input preservation is implemented, with focused assertions narrower than the full prose. |
| AC-M5D7H-003 | **CONDITIONAL — P2 overflow evidence** | Candidate/path misuse fails pre-I/O; direct observed `long.MaxValue` is source-correct but not focused-tested. |
| AC-M5D7H-004 | **PASS** | Fresh missing/current/invalid/unsupported/classification/reason/revision/plan mismatches produce ExistingPrimaryValidation and no move/replace. |
| AC-M5D7H-005 | **PASS with P2 matrix note** | Precommit fault stages and zero replace/move are tested; all result row fields are not exhaustively asserted. |
| AC-M5D7H-006 | **PASS** | Replace and postcheck failures produce uncertainty, re-probe states, and no retry/rollback/fallback. |
| AC-M5D7H-007 | **PASS** | Port order, one replace, no move, and exact old-primary previous bytes are asserted. |
| AC-M5D7H-008 | **PASS with P2 breadth note** | Shared alias lease/refcount barrier and different-root progress pass without FIFO assumption; one waiter is exercised. |
| AC-M5D7H-009 | **FAIL — P1-001** | Result validation does not reject a wrong nonnegative committed revision; focused test covers only negative mutation. |
| AC-M5D7H-010 | **CONDITIONAL — P1-001** | All supplied suites are green and static scope is bounded, but Luna P0/P1 zero is not met. |

## Severity summary and recommendation

- P0: none.
- P1: one — successful recovery result lacks exact derived-revision proof.
- P2: three — direct observed overflow evidence, broader reflection/mutation matrix, and wider cross-entry lease/ABI evidence.

**Recommendation: FAIL pending the minimal P1-001 repair, a positive wrong-revision reflection regression, and focused/full reruns. After those are supplied, the current transaction, source identity, binding getter, shared lease, and authority boundaries are otherwise suitable for a second independent review.**

## R2 independent re-review after revision-proof remediation

The remediation closes P1-001. No runtime/test/contract/evidence files were modified by Luna and Unity was not rerun.

- `ProfileInputRecoverySaveResultV1` now carries private `_expectedRevision` proof. A successful row requires `_revision >= 0`, `_expectedRevision >= 0`, and exact equality; precommit/uncertain rows require both revision fields to be exact `-1`.
- The focused reflection test now mutates `_revision` to both `-1` and positive `999`; it asserts `Validate()` and the `CommittedRevision` getter fail closed for both. The normal constructor supplies `plan.ResultRevision` to both fields, and the transaction still post-validates the derived repair bytes/revision.
- Source identity, fresh semantic-plan validation, metadata-only `InputCompatibility` access, binding-recovery reason comparison, exact one `Replace`, zero recovery `Move`, old-primary byte equality, and shared ordinary/recovery lease behavior remain unchanged.
- The additive M5D7D rule remains satisfied: public ordinary-save enums/result/API and `Save` logic are unchanged in the reviewed source shape; the separate recovery result remains internal.

## R2 AC and residual status

All P1 acceptance concerns are closed. The direct observed `long.MaxValue` pre-I/O case remains a non-blocking evidence-precision P2: the source delegates to M5D7C before `Acquire`, while the focused matrix exercises a fresh-primary max revision and the full regressions are clean. Other residual P2 notes remain the representative breadth of result/mutation/reflection rows and one-waiter lease breadth; no new source blocker was found.

| AC | R2 independent status | Basis |
|---|---|---|
| AC-M5D7H-001 | **PASS** | Metadata recovery commits exact `r+1`, preserves exact previous bytes, and now carries checked expected revision proof. |
| AC-M5D7H-002 | **PASS** | Binding recovery commits the M5D7C repair without calling unavailable compatibility getter; prior success remains green. |
| AC-M5D7H-003 | **PASS with P2 direct-overflow note** | Typed primary misuse/path failures are pre-I/O; source overflow order is correct, though direct observed max is not a separate focused case. |
| AC-M5D7H-004 | **PASS** | Fresh source semantic mismatch table remains ExistingPrimaryValidation with replace/move zero. |
| AC-M5D7H-005 | **PASS** | Precommit fault stages remain typed and preserve no-commit behavior. |
| AC-M5D7H-006 | **PASS** | Replace/postcheck faults remain uncertain with no retry/rollback/fallback. |
| AC-M5D7H-007 | **PASS** | Exact order, one replace, zero move, and byte-for-byte old-primary preservation remain asserted. |
| AC-M5D7H-008 | **PASS with P2 breadth note** | Shared alias/refcount barrier and different-root progress remain green without FIFO assumptions. |
| AC-M5D7H-009 | **PASS** | Positive `999` and `-1` revision tampering, illegal outcome/stage/state, getter validation, fault boundaries, and lease cleanup are covered. |
| AC-M5D7H-010 | **PASS** | Static boundary and unchanged D regression plus all refreshed suites are clean; no public recovery API or forbidden authority was added. |

## R2 evidence integrity

I independently parsed all four refreshed NUnit XML files and recomputed SHA-256. Each root result is `Passed`; failed, skipped, and inconclusive counts are all zero. Hashes match the refreshed evidence record and supplied anchors.

| Suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused M5D7H EditMode R3 | 25 | 25 | 0 | 0 | 0 | `1A75D03A576E9A3052DBF93C7B1F5C5BBC0DFCE9CDCA9F79538AD46588F49543` |
| M5D7D focused regression R2 | 29 | 29 | 0 | 0 | 0 | `786E9396434D3C864E9E143E83AC40B00BB0BFBA46938F2F8CFDF157B7D118E8` |
| Full EditMode R2 | 647 | 647 | 0 | 0 | 0 | `7AA5DA79F66123749DBA37DDE04779CE31E09BC98DED243808CFE6EF65B0FAFB` |
| Full PlayMode R2 | 576 | 576 | 0 | 0 | 0 | `0D1605CB59078F43543CDB76290B6B8CCC6D8A961B6069536CA99955A0B9D0D0` |

Current evidence hash: `4054088C6F84A73D3DA7358A630C37337C7DFDFA1FB1C5174D565A7E9812B1B2`.

| Scoped file | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs` | `0A6926E65BD5AEA177FBDAF4C0768232ED209EA0FE713CC0EE22220A5AA3B894` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileInputRecoveryAtomicSaveServiceV1Tests.cs` | `C601560CE285B9774F64800ECECBFABED3140AB6D9795A424B43BEF78BA76F06` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileInputRecoveryAtomicSaveServiceV1Tests.cs.meta` | `CB0431CE7E282125DBE4E78D8F30285212BF9FE85C0522798B708D4ED675D5E3` |

## R2 verdict and recommendation

**PASS — P0=0, P1=0, residual P2=3. Recommend Astra advance M5D7H to Verified integration.** The expected-revision invariant and positive wrong-revision reflection regression close the sole R1 blocker. All refreshed focused/regression/full XMLs and evidence/source hashes independently match; remaining notes are evidence breadth improvements only.
