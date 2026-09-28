---
status: Approved
---

# C1 real process-crash fixture amendment

- Date / approval: 2026-09-28 / Astra
- Bounded design: Sol; implementation: Terra; independent verification: Luna
- Parent: `2026-09-28-vd09-m5d7q-c1-reset-disk-transaction.md`
- Companion: `2026-09-28-vd09-m5d7q-c1-test-and-result-clarification.md`
- Trace: REQ-M5D7QC1-002..007, AC-M5D7QC1-003/006/008/009

This adds only test plumbing needed to test actual process termination. The
existing PowerShell lock-holder remains contention evidence, not proof that a
transaction was killed. No production path, ABI, save algorithm or authority
changes. No user product choice is introduced.

## Exact allowlist addition

- `qa/fixtures/ProfileResetCrashWorker/ProfileResetCrashWorker.csproj`
- `qa/fixtures/ProfileResetCrashWorker/Program.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileResetDiskProcessV1Tests.cs`
  and its `.meta`
- internal-only test-control factory in the already allowlisted reset source,
  accepting a typed closed checkpoint callback and returning the existing test
  control. No public API, friend declaration or asmdef change.
- C1 verification evidence only.

Use the already installed .NET 10 SDK/runtime. The package-free engine-free
worker loads the actual compiled `AcadeGameMaker.Profile.dll` by reflection;
it copies no reset/marker/save algorithm. A smoke test must validate loading and
calling the actual planner/Begin/Resume. Failure stops this fixture approach,
not a guessed compatibility workaround or production algorithm copy.
Build output/intermediate files live only under a unique verified OS temporary
directory; no repository bin/obj or installed software/dependency change.

## Child ownership and crash protocol

Worker receives only a unique test-owned temporary profile root, a verified
existing compiled assembly path, Begin/Resume mode, one closed checkpoint,
and fixed sibling signal name. Reject filesystem roots, non-temporary profile
roots, signal escape, reparse root/ancestors and unknown checkpoint/mode.
Use hidden bounded child processes, no network or credentials. Ready signal
contains the actual worker PID and exact checkpoint and is exclusive-create,
fully written/flushed/closed outside the profile directory while the reset
root lease remains held. Callback waits at most 30 seconds; timing out is
failure, not successful simulated death. Parent validates signal/PID, proves
another process cannot acquire the root lease (five-second Busy with unchanged
bytes), forcibly terminates only its exact Process handle, waits for confirmed
exit, and starts a distinct worker for resume. Signal timeout, unexpected exit,
wrong PID, missing checkpoint or unconfirmed kill must fail, never skip/pass.
Cleanup affects only these verified child handles and exact temporary roots.

Every selected checkpoint gets an independent root. Cover pre-marker commit,
committed marker reopen, all three old-leaf moves, archive-complete before
default write, default temp write/flush/close, commit and postcheck. Verify old
bytes remain authoritative before barrier publication; after publication
ordinary load/save refuse the barrier, no old resurrection occurs, archive
bytes remain exact and restart converges only to DiskPrepared or a proven
fail-closed outcome. Include invalid/unsupported old bytes and corrupt reset
Primary at restart. Keep transaction-process crash, simulated checkpoint
interruption and ordinary lock-holder contention evidence distinct.

The parent contract remains Approved but unverified until actual execution,
AC-linked evidence and independent Luna acceptance. DiskPrepared retains the
barrier and grants no UI/memory/receipt/barrier-removal authority.
