# VD-09 M5D7G profile launch observation adapter

- Status: Verified
- Owner: Astra
- Implementer: Terra after Approved
- Independent reviewer: Luna
- Dependencies: M5D7B and M5D7E Verified
- Parent requirements: `REQ-PLAT-008`, `REQ-PLAT-010`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D7G-001` through `AC-M5D7G-010`
- Astra approval: 2026-09-13 after Luna second pre-gate PASS (P0=0, P1=0); residual P2=2 retained as implementation-evidence precision requirements.
- Astra verification: 2026-09-13 after Luna implementation PASS (P0=0, P1=0); residual P2=3 accepted as non-blocking evidence-precision notes.

## Purpose and boundary

M5D7G is the engine-free, read-only filesystem adapter that observes the exact three launch candidates directly below a caller-supplied persistent root: `profile.json`, `profile.prev.json`, and `profile.tmp.json`. It maps each leaf independently to one M5D7E `ProfileLoadCandidateV1`: missing, unreadable, or an M5D7B decode result. The returned batch is then suitable for `ProfileLoadSelectorV1.Select(primary, previous, temp)`.

This unit does not choose a source, transform a profile, quarantine or rename a file, save a result, apply binding overrides, record diagnostics, notify the player, read Unity `Application.persistentDataPath`, enter the hub, retry, delete, copy, write, or obtain wall-clock time. It makes a bounded observation, not a stable byte-identity or cross-file atomic-snapshot claim.

## Public contract

Production exposes one synchronous entry point:

```text
ProfileLaunchObservationBatchV1 Observe(string rootDirectoryPath)
```

The immutable batch exposes exact `Primary`, `Previous`, and `Temp` candidates. Every getter first validates the complete batch, including exact role order and each candidate's existing M5D7E invariants. Default, reflection-bypassed, role-swapped, duplicated-role, or malformed candidates throw `InvalidOperationException` from getters and `Validate()`.

`rootDirectoryPath` must be nonempty, fully qualified, and not a filesystem root. It is normalized with `Path.GetFullPath` and trailing non-root separators removed before any file operation. Programmer/path-shape errors throw `ArgumentException` before the injected port is called. The normalized root and paths are not retained in the public result.

## Exact observation algorithm

The adapter observes the three fixed leaves in exact order: primary, previous, temp. For each leaf it performs one logical observation:

1. Probe the exact path with typed presence semantics backed by `File.GetAttributes`.
2. `FileNotFoundException` and `DirectoryNotFoundException` mean `Missing`; no read follows.
3. Present files are opened read-only with `FileShare.Read` and read completely from one handle. The production port owns open/read/exact-completion/close as one synchronous operation, returns only after the handle is closed, and never holds multiple profile-file handles simultaneously.
4. The internal port returns a typed read result with exact `CompletedAndClosed` protocol state, nonnegative `ExpectedLength`, nonnegative `BytesRead`, and the complete byte array. No other/default/unknown state is accepted. The adapter requires `ExpectedLength == BytesRead == Bytes.Length` and each count to be at most `int.MaxValue` before decode. A successful complete read, including the exact `0 == 0 == byte[0].Length` case, is passed unchanged to `ProfileCanonicalDecoderV1.Decode`; its exact five non-default classifications become `ProfileLoadCandidateV1.Decoded(role, result)`. A complete zero-byte file therefore becomes `Decoded(Invalid)`, never `Unreadable`.
5. A recoverable access/read/probe exception (`IOException`, `UnauthorizedAccessException`, `SecurityException`) becomes `ProfileLoadCandidateV1.Unreadable(role)` and observation continues with the next role.
6. An actual BCL disappearance/access/read error after a present probe, including an `EndOfStreamException` caused by incomplete production reading, is a recoverable read fault and maps that role to `Unreadable`. In contrast, a returned default/unknown/not-complete/not-closed typed state, null bytes, or any injected result claiming completion after a short/zero-progress protocol violation is a port-contract violation: the adapter throws `InvalidOperationException` before decode and returns no batch. Unexpected/programmer/fatal exceptions likewise escape.

If a file disappears after a present probe but before/during open/read, the recoverable read exception maps that role to `Unreadable`, not `Missing`, because the exact observation did not establish a missing state. If it appears after a missing probe, the candidate remains `Missing` for this observation. M5D7F independently revalidates files selected for preservation; a later launch orchestration contract owns sequencing and any broader root lease.

The adapter makes no assertion that the three files were observed at one instant, that bytes remain unchanged after return, or that another process cannot mutate them. The decoder and candidate own defensive copies as already verified; M5D7G retains no caller-visible byte array, stream, exception, path, file timestamp, or handle.

## Deterministic test seam

An internal file-observation port mirrors only typed presence and a typed single-handle full-read-and-close result. `CompletedAndClosed` is the sole accepted success state and may carry an empty, non-null byte array; `ExpectedLength` and `BytesRead` make incomplete data distinguishable at the adapter boundary. Tests may inject exact `probe/open/read/close/return` call logs, barriers, exact bytes/counts, incomplete/not-closed/default states, null bytes, count mismatch/overflow, recoverable exceptions, malformed presence values, and unexpected/fatal exceptions. Production uses BCL operations only; its synchronous read operation opens with `FileAccess.Read` and `FileShare.Read`, rejects initial lengths above `int.MaxValue`, loops to the captured expected length, treats premature zero progress as `EndOfStreamException`, closes in `finally`/`using`, and returns equal counts with `CompletedAndClosed` only after close. No public dependency injection, callback, async task, Unity type, or alternate storage provider is added.

The adapter does not add a private per-root lease. Read-only observation is intentionally serialized by exact role order within one call only. Concurrent observations may see different generations and a writer may interleave between roles. A later launch-orchestration consumer must treat the batch as non-atomic, pass M5D7E preservation intents with their original candidates to M5D7F for its mandatory source revalidation, and must not infer coordination from M5D7D/M5D7F service-local leases. Cross-call/cross-process sequencing belongs to that later owner.

## Requirements

- **REQ-M5D7G-001:** exact primary, previous, and temp sibling leaves are observed independently in fixed order and mapped to exact M5D7E candidate roles.
- **REQ-M5D7G-002:** typed missing, unreadable, and decoded states preserve M5D7B classification without selection or recovery transformation.
- **REQ-M5D7G-003:** full reads use one read-only handle per file, close before the next role, and defensively transfer bytes to the verified decoder/candidate boundary.
- **REQ-M5D7G-004:** recoverable file faults are isolated per role while unexpected/fatal/invalid-port faults fail closed without a partial public batch.
- **REQ-M5D7G-005:** root/path and immutable batch invariants reject invalid/default/reflection-forged states before authority can be inferred.
- **REQ-M5D7G-006:** no selection, transformation, quarantine, save, write/delete/copy, retry, logging, notification, Unity, clock, gameplay, network, RNG, or cross-service lease authority is introduced.

## Acceptance criteria

- **AC-M5D7G-001:** real isolated-directory fixtures for three missing files return exact Missing roles and perform no read.
- **AC-M5D7G-002:** valid-current, metadata-recovery, binding-recovery, unsupported, and invalid bytes each preserve the exact M5D7B classification in the correct Decoded role.
- **AC-M5D7G-003:** a mixed primary/previous/temp fixture proves exact probe/read/close role order and that selection has not occurred; a higher-revision valid temp remains merely a Temp candidate.
- **AC-M5D7G-004:** probe/read access failures for every role yield only that role Unreadable and continue observing the other two roles; an actual present-then-disappear or incomplete BCL read is explicitly Unreadable, while missing-then-appear remains Missing for that observation.
- **AC-M5D7G-005:** exact seam events prove `probe → open/read → close → return` for each successful role before the next role probe, with `FileAccess.Read`/`FileShare.Read`. A genuine `ExpectedLength=BytesRead=Bytes.Length=0` file becomes `Decoded(Invalid)`. Injected incomplete/not-closed/default/null, negative/over-limit count, expected/read/array-length mismatch, or short/zero-progress completion claims throw `InvalidOperationException` before decoder invocation and return no partial batch.
- **AC-M5D7G-006:** caller-owned or injected byte mutation after decode cannot change the returned candidate, decode result, document, projection, or later repeated getter result.
- **AC-M5D7G-007:** null/empty/relative/filesystem-root/normalization-failure paths are rejected before any port call; case and trailing separators normalize only the root used to construct the three exact leaves and cannot introduce traversal.
- **AC-M5D7G-008:** default, role-swapped, duplicated-role, malformed-candidate, unknown enum, invalid presence/read-completion value, null bytes, unexpected exception, and fatal exception paths return no apparently valid batch and do not observe later roles after an escaping fault.
- **AC-M5D7G-009:** source review proves exactly the three fixed filenames, `File.GetAttributes`, read-only single-handle full read, no filesystem mutation/retry/fallback/clock/Unity, and no claim of cross-file atomicity or cross-service serialization.
- **AC-M5D7G-010:** focused and full EditMode plus full PlayMode have failed/skipped/inconclusive zero and Luna reports P0=0/P1=0. PASS is observation-only and must not be described as launch recovery, quarantine, save, notification, or binding-apply completion.

## Allowlist and delivery gate

Implementation may add only:

- `Assets/AcadeGameMaker/Runtime/Profile/ProfileLaunchObservationAdapterV1.cs` and `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileLaunchObservationAdapterV1Tests.cs` and `.meta`
- this contract, its M5D7G pre-gate/implementation/review evidence, and one minimal `docs/README.md` entry

Existing M5D3-M5D7F source/tests, asmdefs, Packages, ProjectSettings, scenes, prefabs, input assets, and other project files remain read-only. Required sequence is Luna contract pre-gate, Astra status change to Approved, Terra implementation, Astra test execution, Luna independent post-review, then Astra Verified integration.
