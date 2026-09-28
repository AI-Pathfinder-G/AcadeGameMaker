# VD-09 M5D7D atomic profile save adapter — Luna independent implementation review

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7D atomic profile save adapter](../specs/work-contracts/2026-09-13-vd09-m5d7d-atomic-profile-save-adapter.md)
- Evidence: [Terra implementation evidence](2026-09-13-vd09-m5d7d-implementation-evidence.md)
- Compared against: Approved M5D7D, VD-09, SYSTEM-CONTRACTS, Verified M5D7A–M5D7C APIs, and the M5D7D second pre-gate
- Scope: independent source, test, evidence, XML, dependency-boundary, and allowlist review. Runtime/test/contract/evidence/README were not modified and Unity was not rerun.
- First-pass verdict: **CONDITIONAL FAIL — P0=0, P1=2** (historical; both findings are resolved below).
- Final R2 verdict: **PASS — P0=0, P1=0; recommend Astra mark Verified.**

## Executive finding

The implementation follows the approved transaction shape: argument/document validation precedes the lease, the exact three sibling leaves are used, temp bytes are written through an exclusive `FileStream` with `WriteThrough` and `Flush(true)`, temp is reread before commit, valid existing primary uses `File.Replace`, first save uses same-directory `File.Move`, and commit/postcheck failures return an uncertain result without rollback or retry. The public result matrix, reference-counted lease, normalized path key, explicit existing-primary stage, and source-byte cloning are present.

The first review found two production safety gaps: ambiguous `File.Exists` presence and unfiltered catches. R2 replaces the former with typed `File.GetAttributes` presence and maps only not-found exceptions to `Missing`; access/other filesystem failures now reach `ExistingPrimaryValidation`/`Unreadable`. R2 also filters transaction/probe catches to recoverable IO/security exceptions, so unexpected and fatal exceptions escape while the lease still releases. No P0/P1 remains in the revised source.

## P1 findings

### P1-001 — RESOLVED: BCL `File.Exists` collapsed inaccessible existing primary into “missing”

At `ProfileAtomicSaveServiceV1.cs:102`, `ProfileAtomicSaveBclFileOperations.Exists` returns `File.Exists(path)`. The service uses this boolean at lines 186–198 to choose replacement versus first-save move. .NET documents that `File.Exists` returns `false` when the path cannot be inspected (for example access denied or another filesystem error), not only when the file is absent ([Microsoft `File.Exists`](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.exists?view=net-8.0)). Consequently, an existing but unreadable `profile.json` can set `replacement=false`; the service then calls `File.Move(temp, primary)` instead of returning `FailedBeforeCommit/ExistingPrimaryValidation`. Depending on the filesystem, this either attempts an unauthorized overwrite/fails at the commit call (wrong stage/outcome) or risks violating the “no commit call” rule. `Probe` has the same misclassification because it first trusts `Exists` and returns `Missing`.

The injected test named `unreadable` does not expose this production gap: its proxy throws from `ReadAllExclusive` after `File.Exists` returns true. The real BCL adapter is not tested with an access-denied primary.

Minimal fix: replace the boolean presence operation with a presence probe that distinguishes only `FileNotFoundException`/`DirectoryNotFoundException` as `Missing` and propagates access/other filesystem errors to the service's unreadable path. Apply the same distinction in `Probe`; an inaccessible existing primary must be `Unreadable` and stop before `Replace`/`Move` at `ExistingPrimaryValidation`. Add a production-oriented regression or an equivalent injected presence-error test that proves the commit call count remains zero.

R2 implements this bounded fix. `ProbePresence` uses `File.GetAttributes`, returns `Missing` only for `FileNotFoundException`/`DirectoryNotFoundException`, and lets access/other errors propagate. `Probe` maps recoverable presence/read errors to `Unreadable`; existing-primary presence errors are returned at `ExistingPrimaryValidation` before any commit call. The new `AC005_PresenceAccessErrorIsUnreadableAndNeverCommits` test covers the injected access-error path. **P1-001 resolved.**

### P1-002 — RESOLVED: broad catches swallowed fatal and unexpected exceptions

The service has unfiltered `catch {}` blocks at lines 164, 173, 180, 193, 200, 221, and 243. These catch more than expected filesystem/decoder faults, including `OutOfMemoryException` and arbitrary programming/runtime exceptions. A fatal allocation/decoder error can therefore be converted to a typed `FailedBeforeCommit`, `CommitOutcomeUncertain`, or `Unreadable` result and may trigger additional probing/allocation, instead of escaping. This makes result classification and process safety non-deterministic under resource failure; the focused fault seam only injects `IOException`, so the 25/25 run does not cover it.

Minimal fix: use explicit recoverable exception filters for the file/decoder faults authorized by the contract and rethrow fatal/unexpected exceptions. At minimum, fatal runtime exceptions must not be swallowed; avoid turning programmer errors into persisted-state classifications. Preserve the existing stage/result mapping for expected `IOException`/access/read/decode failures and keep lease release in `finally`.

R2 implements `IsRecoverableFileException`, limited to `IOException`, `UnauthorizedAccessException`, and `SecurityException`, on every transaction/probe catch. Injected `InvalidOperationException` and `OutOfMemoryException` now escape, and tests assert `RegistryCount()==0` afterward, proving the `finally` lease release. **P1-002 resolved.**

## P2 findings and test-precision notes

- `AC007_AliasesAndWaitersSerializeAndFinalLeaseIsRemoved` starts waiter tasks and releases the first blocked writer immediately; it has no barrier proving those tasks incremented the registry ref-count before release. The static lease lifecycle is correct, but the test does not strongly demonstrate the active-plus-waiter split-lock race is closed. Add a port/barrier observation after all waiters are registered.
- `AC002_InjectedPortPreservesRequiredCommitPrefixOrder` asserts only the first nine calls, not the complete post-probe sequence or release ordering. Add full call-log assertions for `post-probe primary/previous/temp → release`.
- The runtime's post-commit `read(primary)` before `Probe(primary)`, and its extra `read(previous)` for byte-exact old-primary proof, are consistent with step 5 of the approved contract; they are not an additional commit or fallback. The issue is only that the test does not assert this complete suffix, so the exact observable order remains a P2 evidence gap rather than a source P1.
- AC-001 does not assert all successful result states or that a pre-existing `profile.prev.json` remains byte unchanged on first save; AC-003/005 likewise do not assert every probed state and byte-preservation combination. These are coverage improvements, not additional source defects.
- The focused source test confirms `FileMode.Create`, exclusive sharing, `WriteThrough`, `Flush(true)`, `File.Replace(..., true)`, `File.Move`, exact leaves, and forbidden-authority absence. The contract's Windows-only and single-process scope is respected; symlink/junction and cross-process identity are not claimed.

## Evidence integrity

I parsed all three supplied NUnit XML files and recomputed their SHA-256 values. Each has the evidence-reported counts and `Passed` result, with zero failed, skipped, or inconclusive tests.

| Suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused EditMode R5 | 29 | 29 | 0 | 0 | 0 | `0043AB07363460360E2464D9250F06A3F50AA8A95EC04AD589DBC216CA417845` |
| Full EditMode R2 | 584 | 584 | 0 | 0 | 0 | `0AA3D77392361507B3ABAA49F6741E89A8A6DD0FAB00D2C63B9BAA75E75E4903` |
| Full PlayMode R2 | 576 | 576 | 0 | 0 | 0 | `8B8098E0E4B183A177DB2DA7CE305F5E77776D65BB54BF46E803C5B10EFDFE84` |

The R2 workspace hashes match Terra's updated evidence:

| Scoped file | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs` | `32114257682BB19D8A48F15D9460B7E2437239ED23EDC8EA651A95026415A0FC` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs.meta` | `DE020BF9FBB5059399796460931AD619A782561EF8828AECE5F8743A495A1C80` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileAtomicSaveServiceV1Tests.cs` | `B7D562800E0ADA31869EB731A85FE2EE7FDA851E66B0C3613484AC1B54F821DF` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileAtomicSaveServiceV1Tests.cs.meta` | `B35C122B3514FFFC6F2BA62B9E85329ED2147A68255CC4B42652DEAE2D2DE3E0` |

Earlier failed/stalled and pre-P1 runs are diagnostic history only. The final R2 XMLs report Unity `6000.6.0f1` with licensing preflight passed.

## AC status

| AC | Independent status | Basis |
|---|---|---|
| AC-M5D7D-001 | **PASS** | Real first save writes exact document bytes, removes temp, and reports first outcome/revision. |
| AC-M5D7D-002 | **PASS with P2 test gap** | Real replacement preserves exact old primary via `File.Replace`; required prefix is present, but the test does not assert the complete post-probe suffix. |
| AC-M5D7D-003 | **PASS** | Focused injections cover directory, write, flush, close, temp read, and temp corruption with no commit call and precommit stages; unexpected/fatal propagation is separately covered. |
| AC-M5D7D-004 | **PASS** | Replace/move throws and postcommit corrupt/missing/wrong-primary/temp/previous cases return uncertain, revision `-1`, and three probed states without fallback. |
| AC-M5D7D-005 | **PASS** | Typed presence-error injection now yields `Unreadable` at `ExistingPrimaryValidation` with no replace/move; production `GetAttributes` no longer collapses access errors into missing. |
| AC-M5D7D-006 | **PASS** | Fully-qualified non-root path and pre-IO argument validation are implemented; exact leaves and aliases are covered. |
| AC-M5D7D-007 | **PASS with P2 test gap** | Ref-counted case-insensitive per-directory leases are statically correct and alias/concurrency tests pass, but waiter registration is not synchronized in the test. |
| AC-M5D7D-008 | **PASS with P2 coverage gap** | Result getters revalidate, source bytes are cloned, and principal reflection-invalid combinations fail; the test does not exhaustively enumerate the cross-product. |
| AC-M5D7D-009 | **PASS** | Static source contains the required BCL transaction and no fallback, IO authority expansion, Unity dependency, or forbidden delete/copy path. |
| AC-M5D7D-010 | **PASS** | Focused 29/29, full EditMode 584/584, and full PlayMode 576/576 all have zero failed/skipped/inconclusive; final evidence includes the bounded P1 regression tests. |

## P0/P1/P2 summary

- P0: none.
- P1: none remaining. P1-001 and P1-002 are resolved by the R2 source changes and focused regressions.
- P2: waiter test registration barrier; incomplete full call-log and result-state assertions; missing first-save pre-existing-previous preservation assertion.

## Recommendation

Recommend Astra mark M5D7D **Verified**. R2 independently confirms the bounded presence/error and exception-filter repairs, all final XML counts/hashes, exact BCL transaction operations, result/lease/path invariants, and the engine-free/allowlist boundary. The remaining P2 test-precision notes do not block acceptance; preserve the single-process and no-recovery-selection scope.
