# VD-09 M5D7D atomic profile save adapter — Luna independent contract pre-gate

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7D atomic profile save adapter](../specs/work-contracts/2026-09-13-vd09-m5d7d-atomic-profile-save-adapter.md)
- Compared against: Approved VD-09, SYSTEM-CONTRACTS, VD-05, ADR-0031, and Verified M5D7A–M5D7C APIs/contracts
- First-pass verdict: **CONDITIONAL FAIL — P0=0, P1=4** (historical; all four findings are rechecked below)
- Scope: contract/design pre-gate only. No implementation was changed and no Unity test was run.

## Executive finding

The transaction outline is directionally consistent with VD-09: validate a current-compatible M5D7A document, write all final bytes to a sibling temp using exclusive `FileStream`/`WriteThrough`/`Flush(true)`, close and decode the temp before commit, use Windows `File.Replace(temp, primary, previous, true)` for replacement or same-directory `File.Move` for first save, and return an uncertain result after commit/postcheck failures without rollback or retry. `System.IO.FileStream.Flush(Boolean)` and `File.Replace(String,String,String,Boolean)` are available in the .NET Standard API surface, and Unity 6 supports .NET Standard 2.1 compatibility ([Microsoft `FileStream.Flush`](https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.flush?view=net-7.0), [Microsoft `File.Replace`](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.replace?view=net-8.0), [Unity .NET profile support](https://docs.unity3d.com/6000.0/Documentation/Manual/dotnet-profile-support.html)).

The first pass identified four P1 gaps. The revised contract now closes them with a closed result matrix and `-1` failure/uncertain sentinel, a reference-counted active-plus-waiter lease, explicit fully-qualified non-root Windows path normalization with an `OrdinalIgnoreCase` key, and an `ExistingPrimaryValidation` stage plus exact injected call order. The second pass below finds no remaining P0/P1 contract blocker.

## P1 findings

### P1-001 — `ProfileAtomicSaveResultV1` outcome/stage/state/revision invariants are under-specified

The result exposes outcome, failure stage, three file states, and committed revision, while AC-M5D7D-008 requires default/reflection-bypassed combinations to fail closed. However, the contract does not define the value relation for the four outcomes:

- Is `CommittedRevision` the supplied document revision only for committed outcomes? What exact sentinel represents `FailedBeforeCommit` and `CommitOutcomeUncertain`?
- May `CommittedRevision` be the supplied revision after an uncertain `File.Replace` that might have succeeded, or must it remain an unavailable sentinel to avoid claiming commit?
- Which `ProfileStoredFileState` values are legal for each stage, and when is `NotProbed` permitted versus `Missing`/`Unreadable`?
- Is `failureStage=None` legal only for committed outcomes? Must uncertain postchecks always carry three non-`NotProbed` states, even when a probe itself fails?

Without these rules, a forged result such as `FailedBeforeCommit + None + ValidCurrentInput primary + supplied revision`, or `CommittedReplacement + CommitCall + Missing primary`, has no normative disposition. This is not merely a getter detail: callers need to know whether a revision was definitely persisted.

Required narrow fix: define a closed result matrix. Recommended policy is `CommittedRevision=document.ProfileRevision` only for `CommittedFirst`/`CommittedReplacement`, and a documented unavailable sentinel (prefer `-1`) for both failure outcomes; uncertain never claims a committed revision. Require `None` only on committed outcomes; allow only `DirectoryPreparation`, `TempWriteFlushClose`, or `TempReadValidation` for `FailedBeforeCommit`; allow only `CommitCall` or `PostCommitValidation` for uncertain; and specify legal state combinations for successful first save, replacement, precommit partial probes, and uncertain independent probes. Add reflection tests for every illegal cross-product and for getter revalidation.

### P1-002 — lock-registry removal can break same-directory serialization

“Per-directory ordinal lock” plus “remove the registry entry after save completes” is insufficient unless lookup/waiter references are counted atomically. A concrete race is:

1. Save A and waiting Save B obtain the same lock object.
2. A releases the monitor and removes the dictionary entry while B is still waiting on that object.
3. Save C starts, obtains a newly created lock object for the same directory, and enters while B acquires the old object.

B and C then interleave writes to the same `profile.tmp.json`/primary set, violating AC-M5D7D-007 even though each save used a nominal per-directory lock.

Required narrow fix: specify a registry gate plus reference-counted lease. Increment the entry count before a caller waits, decrement only after that caller exits the directory lock, and remove an entry only while holding the registry gate when its count reaches zero. The registry gate must not be held during file IO. Also require that exceptions at every stage release the lease exactly once.

### P1-003 — absolute-path validation and Windows directory-key identity are not exact

The contract says `directoryPath` must be nonempty absolute and then says to call `Path.GetFullPath`. `GetFullPath` normalizes a relative path into an absolute path; it does not itself enforce that the caller supplied an absolute path. An implementation that calls it first would accept `profiles` despite AC-M5D7D-006. `Path.IsPathRooted` is also not the same as fully qualified on Windows for a rooted path such as `\profiles`.

Further, an ordinal dictionary key over the returned string does not serialize `C:\Save` and `c:\save` on Windows, even though they address the same case-insensitive directory. `Path.GetFullPath` does not canonicalize case, and no symlink/junction identity is promised.

Required narrow fix: require an explicit fully-qualified-path check before normalization (for the target Unity/.NET profile, use the available `Path.IsPathFullyQualified` semantics or an equivalent Windows drive/UNC check), then normalize separators and `.`/`..` segments with `Path.GetFullPath`. Define the lock key as an ordinal, culture-independent Windows case-insensitive key (`OrdinalIgnoreCase`) for this Windows-only contract, or explicitly narrow AC-007 to identical normalized spelling. State that symlink/junction aliases are outside this engine-free path identity if they are not resolved by IO.

### P1-004 — existing-primary validation has no precise stage or required call order

The contract requires that an existing primary be independently read/decoded before `File.Replace`, that invalid/unsupported/input-recovery/unreadable primary causes no replace, and that its exact bytes/revision be proven for the previous backup. Yet the stage enum has no `ExistingPrimaryValidation` value, step 4 only says to “judge primary existence once,” and AC-M5D7D-002's call order omits the existing-primary read/decode. The prose labels a primary-validation failure `FailedBeforeCommit/TempReadValidation`, which is semantically a temp stage and makes injected call-order assertions ambiguous.

Required narrow fix: either add `ExistingPrimaryValidation` to `ProfileAtomicSaveStage`, or explicitly define existing-primary read/decode as a substage of `TempReadValidation` and include it in AC-002/AC-005 order. Require the production and injected paths to perform: temp read/decode, primary existence/read/decode (when present), then the single replace/move call. Test primary valid, invalid, unsupported, recovery-required, unreadable, and race/throw cases with commit count zero and unchanged primary/previous bytes.

## P2 findings and implementation constraints

- The .NET API choice is feasible for this Windows Unity target: `FileStream.Flush(true)` is the documented flush-to-disk overload and `File.Replace(source,destination,backup,true)` is the documented backup replacement overload. The implementation must still verify the project’s actual Unity 6000.6 Windows runtime behavior, because `File.Replace` is platform/filesystem dependent; a non-Windows fallback is not authorized by this contract.
- The internal injectable port is implementable without changing the existing asmdef only if the new allowlisted runtime file contains the required friend declaration (or the contract explicitly permits a matching allowlisted `AssemblyInfo` addition). Do not make the port public gameplay API, change the existing Profile asmdef, or add a second runtime file. Production `Save` must use actual `System.IO`; the internal overload must be the only deterministic fault seam.
- Temp write faults must close/dispose the handle in a finally path while preserving the recorded `TempWriteFlushClose` stage. A flush exception, close exception, partial write, and stale-temp truncation are distinct injected points; none may delete or overwrite primary/previous before commit.
- Directory preparation, document/path argument validation, and mismatch/default document rejection must occur before any file IO. `FileMode.Create` is correct for replacing a stale temp, but the contract should state that an existing stale temp may be overwritten while primary/previous remain untouched until commit.
- A successful first save with a pre-existing previous file must leave that previous file unchanged; a replacement must make previous byte-exactly the old valid primary. Postcommit probes must check primary exact supplied bytes/revision, temp missing, and replacement previous exact old-primary bytes; any failure is uncertain with no rollback/delete/retry.
- Probe state is correctly limited to M5D7B classifications (`ValidCurrentInput`, combined `ValidInputRecoveryRequired`, `UnsupportedSchema`, `Invalid`, `Unreadable`, or `Missing`) and does not claim load selection, quarantine, default/recovery transformation, input application, notification, or gameplay entry.

## AC design status

| AC | Pre-gate status | Independent basis |
|---|---|---|
| AC-M5D7D-001 | Conditional pass | First-save order and exact document bytes are coherent, but success result state/revision invariants require P1-001 closure. |
| AC-M5D7D-002 | Blocked by P1-004 | Replacement semantics are correct in principle; existing-primary proof and exact call order need a stage/rule. |
| AC-M5D7D-003 | Blocked by P1-001/P1-004 | Failure staging is mostly listed, but result state/revision and primary-validation stage are not closed. |
| AC-M5D7D-004 | Blocked by P1-001 | Uncertain result semantics and probe-state availability are under-specified. |
| AC-M5D7D-005 | Conditional pass | Existing-primary rejection and stale-temp truncation are coherent once primary validation is made an explicit stage/order. |
| AC-M5D7D-006 | Blocked by P1-003 | Absolute-path enforcement and normalized Windows lock identity need exact rules. |
| AC-M5D7D-007 | Blocked by P1-002/P1-003 | Reference-counted locking and case-insensitive same-directory key semantics are required. |
| AC-M5D7D-008 | Blocked by P1-001 | Reflection-bypass result combinations cannot be tested deterministically without a closed result matrix. |
| AC-M5D7D-009 | Conditional pass | `Flush(true)`, `File.Replace`, exclusive `FileStream`, and same-directory move are feasible; static implementation proof is later. |
| AC-M5D7D-010 | Not executable at pre-gate | No implementation or suite evidence exists; no persistence/load-recovery overclaim is allowed. |

## P0/P1/P2 summary

- P0: none found.
- P1: P1-001 result invariant/sentinel matrix; P1-002 lock lease lifecycle race; P1-003 absolute path and Windows key identity; P1-004 existing-primary stage/order ambiguity.
- P2: friend-access placement for the internal test port; exact dispose/fault-point mechanics; stale-temp/first-save previous preservation tests; platform-specific `File.Replace` execution proof; no-IO/recovery overclaim boundary.

## First-pass recommendation (superseded by second pass)

Do not advance M5D7D to Astra approval yet. Add the closed result invariant matrix, reference-counted per-directory lock lease, explicit absolute/case-aware path identity, and an explicit existing-primary validation stage/order. After those contract repairs, Luna should perform a second pre-gate. The core transaction design and .NET API choices are otherwise viable within the one-runtime-file/one-test-file allowlist, with no P0 finding.

## Second independent pre-gate — revised contract

- Review date: 2026-09-13
- Scope: revised contract text only; no runtime/test/contract implementation was changed and no Unity test was run.
- Verdict: **PASS — P0=0, P1=0; recommend Astra approval review (do not change contract status here).**

### Resolution of first-pass P1 findings

| Finding | Second-pass result | Evidence in revised contract |
|---|---|---|
| P1-001 result invariants | **Resolved** | `CommittedRevision` is supplied and nonnegative only for success; both failure outcomes require exact `-1`. `None`, precommit stages, commit/postcheck stages, probe availability, success file states, and reflection-invalid combinations are now explicitly constrained. `Validate()` precedes every getter. |
| P1-002 lock lease race | **Resolved** | A registry mutex protects an active-plus-waiting ref-count. The count increments before waiting and decrements only after lock release; removal occurs only at exact zero. The registry mutex is not held during IO, preventing the A/B/C split-lock race. |
| P1-003 path/key identity | **Resolved** | Entry validation requires `Path.IsPathFullyQualified` before `Path.GetFullPath`; normalized non-root paths use an ordinal-ignore-case per-directory key. Exact `Path.Combine` leaves and no caller leaf/path API close traversal ambiguity. Symlink/junction identity is not overclaimed. |
| P1-004 existing-primary stage/order | **Resolved** | `ExistingPrimaryValidation` is an explicit stage. Temp is read/decoded first; an existing primary is then exclusive read/closed and exact current-input-valid before the single `File.Replace`; first save uses same-directory move. The injected call-order and later-call suppression are stated exactly. |

### Second-pass AC assessment

| AC | Status | Independent finding |
|---|---|---|
| AC-M5D7D-001 | Pass by contract | First commit has exact supplied bytes, primary valid, temp missing, revision and previous post-probe invariants. |
| AC-M5D7D-002 | Pass by contract | Replacement preserves the old primary through exact `File.Replace`; validation/read/close now appears in the required order. |
| AC-M5D7D-003 | Pass by contract | Directory/temp-stage failures are precommit, commit count zero, with typed stage and observed-vs-unobserved states. |
| AC-M5D7D-004 | Pass by contract | Commit throw/postcheck failure is fail-closed as uncertain, revision `-1`, with independent probes and no rollback/retry/fallback. |
| AC-M5D7D-005 | Pass by contract | Invalid, unsupported, recovery-required, or unreadable existing primary stops at `ExistingPrimaryValidation`; stale temp may truncate without touching primary/previous. |
| AC-M5D7D-006 | Pass by contract | Relative, empty/null, root, normalization, default, malformed, and mismatch inputs are rejected before IO/lock; aliases share the normalized case-insensitive key. |
| AC-M5D7D-007 | Pass by contract | Ref-counted leases serialize active and waiting same-directory saves, including case/separator aliases, while different keys are not globally serialized. |
| AC-M5D7D-008 | Pass by contract | Result getter revalidation, exact sentinel, enum/state/stage cross-product rejection, and source/result byte ownership are normative. |
| AC-M5D7D-009 | Pass by contract; implementation gate remains | Required .NET file operations and forbidden fallbacks are exact. Actual production source/static review belongs to post-implementation QA. |
| AC-M5D7D-010 | Not executable at pre-gate | The contract correctly reserves focused/full test and Luna P0/P1 evidence for implementation review; it does not overclaim persistence or recovery selection. |

### Remaining P2 / implementation checks

1. Unity/Windows execution must confirm the production adapter's `Flush(true)`, `File.Replace(..., true)`, and same-directory `File.Move` behavior; this is an implementation/evidence gate, not a contract blocker. No non-Windows fallback is authorized.
2. Terra's tests must exercise ref-count release on every injected exception, multiple waiters, case/trailing-separator aliases, and distinct-directory overlap. They must verify no lock entry remains after the final lease.
3. Fault tests should cover partial write, flush, close, temp reread/decode, existing-primary reread/decode, replace/move throw, and every post-probe mismatch. These are explicit in the ACs; the pre-gate cannot verify code-level disposal or call-log behavior.
4. The contract deliberately leaves symlink/junction aliases and cross-process locking out of scope. A later implementation must not silently claim those identities or add cross-process coordination.

## Second-pass conclusion

No P0 or P1 remains in the revised M5D7D contract. The transaction boundary is fail-closed, existing-primary proof is staged before replacement, and result/lock/path invariants are now sufficiently closed for Astra's approval decision. This is a pre-implementation PASS only; it is not an implementation verification or contract status change.
