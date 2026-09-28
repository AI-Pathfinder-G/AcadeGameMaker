# VD-09 M5D7H primary input-recovery atomic save extension

- Status: Verified
- Owner: Astra
- Implementer: Terra after Approved
- Independent reviewer: Luna
- Dependencies: M5D7B, M5D7C, M5D7D, M5D7E, and M5D7G Verified
- Parent requirements: `REQ-PLAT-009`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D7H-001` through `AC-M5D7H-010`
- Astra approval: 2026-09-13 after Luna second pre-gate PASS (P0=0, P1=0); residual P2=3 retained as implementation-evidence requirements.
- Astra verification: 2026-09-13 after the revision-proof remediation, focused M5D7H 25/25, unchanged M5D7D 29/29, full EditMode 647/647, full PlayMode 576/576, and Luna R2 PASS (P0=0, P1=0; residual P2=3). This verifies only primary input-recovery atomic replacement, not launch coordination or actual Input System override application.

## Purpose and boundary

M5D7H closes one intentionally excluded M5D7D case: the selected primary is valid profile v1 but its input block requires metadata or binding recovery. Ordinary `ProfileAtomicSaveServiceV1.Save` correctly refuses to replace such a primary because that API accepts only an already-current existing primary. M5D7H adds a separate internal recovery entry that revalidates the observed input-recovery source, derives the exact M5D7C input-repair document, and atomically replaces primary while preserving the exact pre-repair primary bytes as previous.

This is not the launch coordinator. It does not observe three candidates, select a source, quarantine stale/invalid files, decide whether recovery is needed, apply Input System overrides, log, notify, retry, read Unity paths, or enter gameplay. The later launch owner may call this internal entry only after stale temp and any previous-file preservation intent have safely resolved. M5D7H itself must not claim that precondition was met.

## Internal API and shared transaction authority

`ProfileAtomicSaveServiceV1` gains an internal entry only:

```text
ProfileInputRecoverySaveResultV1 SaveInputRecovery(
    string directoryPath,
    ProfileLoadCandidateV1 observedPrimary)
```

No new public save method is added. The entry reuses M5D7D's exact normalized-root `OrdinalIgnoreCase` ref-counted lease and exact `IProfileAtomicSaveFileOperations` port. Ordinary save and input-recovery save on the same normalized/case-alias root therefore cannot interleave; a separate registry or lock is forbidden.

`observedPrimary` is an enforceable M5D7E candidate proof: role must be exact `Primary`, kind exact `Decoded`, and its fully revalidated decode classification one of the two input-recovery classifications. Previous/temp/wrong-role/missing/unreadable/default/reflection-invalid candidates are argument errors before lease/I/O.

The recovery-only types are internal and separate from M5D7D's public ABI:

- `ProfileInputRecoverySaveOutcomeV1`: exact values `CommittedRecoveryReplacement=1`, `FailedBeforeCommit=2`, `CommitOutcomeUncertain=3`.
- `ProfileInputRecoverySaveResultV1`: internal readonly struct with internal getters `Outcome`, `FailureStage`, `PrimaryState`, `PreviousState`, `TempState`, `CommittedRevision`, plus internal `Validate()`.
- `FailureStage` reuses the existing `ProfileAtomicSaveStage`; three file-state getters reuse `ProfileStoredFileState`. Neither existing enum gains a value and no existing numeric value/range changes.

Every returned row has all three file states independently probed. Therefore `NotProbed` is illegal in every valid M5D7H result; a recoverable individual probe becomes `Unreadable`, while invalid typed presence or unexpected/fatal probe errors escape and no result is returned. The closed matrix is:

- `CommittedRecoveryReplacement`: stage `None`; primary `ValidCurrentInput`, previous `ValidInputRecoveryRequired`, temp `Missing`; committed revision is `>=0` and equals the derived repair revision.
- `FailedBeforeCommit`: stage is exactly `DirectoryPreparation`, `TempWriteFlushClose`, `TempReadValidation`, or `ExistingPrimaryValidation`; each file state is one known non-`NotProbed` value; committed revision exact `-1`.
- `CommitOutcomeUncertain`: stage is exactly `CommitCall` or `PostCommitValidation`; each file state is one known non-`NotProbed` value; committed revision exact `-1`.

Default, unknown enum, illegal state/stage/revision combination, or reflection-forged result fails from every getter and `Validate()`. Results retain no bytes, paths, exceptions, decoder trees, or mutable data.

## Entry proof and exact source revalidation

Before lease or I/O, `observedPrimary` must fully validate with exact Primary/Decoded provenance and have classification exactly `ValidInputMetadataRecoveryRequired` or `ValidBindingRecoveryRequired`. M5D7C `PlanInputRepair(observedPrimary.DecodeResult)` must succeed; its result document/revision is the sole write candidate. Invalid, unsupported, current, wrong-role, missing/unreadable, default/reflection-bypassed candidates and revision overflow escape as argument/programmer errors before I/O.

After temp write/flush/close and exact current-document revalidation, the service requires `profile.json` to be present and reads its exact bytes exclusively. It decodes the fresh bytes and accepts them only when all are true:

- fresh classification equals the observed classification;
- fresh `InputRecoveryReasons` equal the observed value. For `ValidInputMetadataRecoveryRequired` only, fresh `InputCompatibility` also equals the observed value. `ValidBindingRecoveryRequired` does not expose `InputCompatibility`; the method must not call that unavailable getter, and any simultaneous metadata mismatch is compared through the exact reason flags;
- `PlanInputRepair(fresh)` succeeds and its source revision, result revision, reasons, and canonical result bytes equal the originally derived plan;
- the fresh source revision is therefore exactly one less than the repair result.

This is semantic recovery-plan identity, not original byte identity. Two different source byte sequences may be accepted only when they produce the same exact classification, recovery flags, source/result revisions, reasons, and canonical repair document. Any mismatch is `FailedBeforeCommit/ExistingPrimaryValidation`, with replace call zero.

## Exact atomic sequence

1. Validate arguments and derive the M5D7C input-repair plan before lease/I/O.
2. Acquire the existing M5D7D normalized-root lease.
3. Ensure the supplied directory, then create/truncate exact sibling `profile.tmp.json` through the existing write-through exclusive writer, write all repair bytes, flush to storage, and close.
4. Re-read temp exclusively and require exact `ValidCurrentInput`, canonical bytes, and repair revision.
5. Require exact sibling `profile.json` present; exclusive-read/close its bytes and pass the fresh semantic recovery-plan identity checks above.
6. Invoke exactly one `File.Replace(profile.tmp.json, profile.json, profile.prev.json, true)`. There is no first-save move in this recovery entry and no copy/delete/rename fallback.
7. Post-read primary and require exact repair canonical bytes/revision; post-read previous and require byte-for-byte equality with the captured pre-repair primary; independently probe primary/previous/temp and require current/input-recovery/missing states before success.

Any recoverable directory/write/flush/close/temp-read/source-read fault before replace yields typed `FailedBeforeCommit` and replace zero. A recoverable replace exception or any postcheck mismatch/fault yields `CommitOutcomeUncertain`; no retry, rollback, cleanup, delete, or fallback follows. Unexpected/programmer/fatal exceptions escape, while the shared lease still releases in `finally`.

### Failure precedence

| Observation | Result |
|---|---|
| directory/create or temp open/write/flush/close recoverable fault | `FailedBeforeCommit` at its exact precommit stage; all three files independently probed |
| temp read fault or temp not exact current repair document | `FailedBeforeCommit/TempReadValidation`; replace/move zero |
| primary missing, presence/read fault, current/invalid/unsupported classification, or semantic-plan mismatch | `FailedBeforeCommit/ExistingPrimaryValidation`; replace/move zero; missing never falls back to first-save move |
| replace recoverable exception | `CommitOutcomeUncertain/CommitCall`; all three files independently probed |
| any post primary/previous/temp state, bytes, classification, or revision mismatch/fault | `CommitOutcomeUncertain/PostCommitValidation`; all three files independently probed |
| invalid typed port value or unexpected/programmer/fatal exception | exception escapes; no apparently valid result; shared lease releases |

## Additive M5D7D compatibility rule

The change to Verified `ProfileAtomicSaveServiceV1.cs` is additive only. Existing public `Save`, `ProfileAtomicSaveOutcome`, `ProfileAtomicSaveStage`, `ProfileStoredFileState`, `ProfileAtomicSaveResultV1`, their numeric values/getters/validation, and the production/injected file-port semantics remain behaviorally and binary-source compatible. The ordinary `SaveHeld` algorithm and accepted/failure result rows may not be broadened. Private helpers/lease access may be refactored only with equivalence proven by the unchanged M5D7D focused suite and full regressions. No public recovery method or second lease/port/registry is permitted.

## Requirements

- **REQ-M5D7H-001:** only an exact Primary/Decoded M5D7E candidate and its M5D7C input-repair plan may enter the recovery replacement path.
- **REQ-M5D7H-002:** fresh primary classification, available metadata compatibility, recovery flags, revisions, reasons, and repair document are revalidated before commit so a stale or changed plan cannot overwrite primary; unavailable M5D7B getters are never called.
- **REQ-M5D7H-003:** exact write-through temp sequence and one Windows `File.Replace` preserve the pre-repair primary bytes as previous.
- **REQ-M5D7H-004:** precommit failure and commit uncertainty are typed without retry/fallback or false committed revision.
- **REQ-M5D7H-005:** ordinary and recovery save entries share the one M5D7D normalized-root lease and deterministic file-operation port.
- **REQ-M5D7H-006:** no public API, source selection, quarantine, launch ordering, input apply, logging, notification, Unity, clock, gameplay, network, or RNG authority is added.
- **REQ-M5D7H-007:** Verified M5D7D public ABI, ordinary-save behavior, result matrix, port semantics, enum values, and focused regressions remain unchanged under an additive-only source modification.

## Acceptance criteria

- **AC-M5D7H-001:** real metadata-recovery primary `r` becomes exact current primary `r+1`; previous is exact original primary bytes and temp is missing, with `CommittedRecoveryReplacement`.
- **AC-M5D7H-002:** real binding-recovery primary, including canonical outer profile with malformed/noncanonical inner override, produces the exact M5D7C default-input repair while settings/tutorial/progression remain byte-equivalent in meaning.
- **AC-M5D7H-003:** previous/temp/wrong-role, missing/unreadable, current/invalid/unsupported/default/reflection-invalid candidates and `long.MaxValue` recovery overflow fail before any port call; null/empty/relative/root paths do likewise.
- **AC-M5D7H-004:** fresh primary missing/current/invalid/unsupported, classification change, recovery-flag change, revision change, or repair-document change yields `ExistingPrimaryValidation`, replace zero, and does not fall back to first-save move.
- **AC-M5D7H-005:** directory/open/write/flush/close/temp-read/temp-corruption failures are typed precommit failures; existing primary/previous remain exact and replace/move calls are zero.
- **AC-M5D7H-006:** replace throw and postcheck primary/previous/temp state, byte, classification, or revision mismatch yield uncertainty with independently probed states and no retry/rollback/delete/fallback.
- **AC-M5D7H-007:** injected call logs prove exact write/flush/close/re-read/source-read/replace/postcheck order, replace at most once, move always zero, and old primary capture is compared byte-for-byte to previous.
- **AC-M5D7H-008:** a barrier holds one ordinary `Save` transaction while case/trailing-alias `SaveInputRecovery` callers have registered on the same lease; no file operation interleaves, every waiter eventually drains without assuming monitor wake/FIFO order, different roots can proceed, refcount includes active+waiters, and the single registry is empty after final release.
- **AC-M5D7H-009:** caller/source/result mutations, every closed result-matrix stage row, default/unknown/illegal reflection combinations, `NotProbed`, sentinel mismatch, invalid typed presence, recoverable versus unexpected/fatal faults, and lease cleanup all fail closed with the documented boundary.
- **AC-M5D7H-010:** static review proves one shared registry/port, unchanged public ABI/enums/ordinary `Save`, exact `File.Replace(..., true)`, no recovery move or forbidden authority. The unchanged M5D7D focused suite, M5D7H focused suite, full EditMode, and full PlayMode are clean and Luna reports P0=0/P1=0. PASS is primary input-recovery atomic replacement only, not launch recovery completion.

## Allowlist and gate

Implementation may modify only:

- `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs`
- add `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileInputRecoveryAtomicSaveServiceV1Tests.cs` and `.meta`
- this contract, its M5D7H pre-gate/implementation/review evidence, and one minimal `docs/README.md` entry

The existing M5D7D test file and all M5D3-M5D7G runtime/tests/metas, asmdefs, Packages, ProjectSettings, scenes, prefabs, input assets, and other files remain read-only. Required sequence is Luna contract pre-gate, Astra Approved, Terra implementation, Astra tests, Luna post-review, then Astra Verified integration.
