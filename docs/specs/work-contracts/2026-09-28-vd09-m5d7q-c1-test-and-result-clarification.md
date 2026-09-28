---
status: Approved
---

# C1 test seam and result clarification

- Date: 2026-09-28
- Approval: Astra; bounded seam design: Sol; implementation: Terra; independent verification: Luna
- Parent: `2026-09-28-vd09-m5d7q-c1-reset-disk-transaction.md` (Approved)
- Trace: REQ-M5D7QC1-001..007; AC-M5D7QC1-001..009

This is an implementation clarification of the existing approved disk-only
contract, not approval for memory cutover, barrier removal, UI, cleanup, or new
production paths. The parent remains normative. No public API is widened.

## Internal deterministic test seam

Existing Begin/Resume delegate to internal overloads accepting an internal
`IProfileResetDiskTestControlV1`. Its closed checkpoint enum has before/after
points for marker write, flush, close, temp reopen, commit move and committed
reopen; each old-role move/reopen; interrupted-default Temp/Primary preservation
move/reopen; and default temp write/flush/close/reopen, commit move and postchecks.
Production uses a no-op control and the existing BCL atomic operations.
Tests inject recoverable IO faults or a distinct simulated interruption that
escapes after handle cleanup. A thin test decorator of the existing BCL atomic
operations supplies default-save points. The reset-only atomic overload still
validates its capability and delegates to existing SaveHeld; no duplicate save
algorithm, fake production filesystem, persisted phase, or global mutable hook.
Each operation is instrumented directly around its actual IO boundary.

## Capability and failures

The reset first-save capability binds the actual issuing live root-lease
instance, not merely its root. Canonical marker decoding creates observation
only; the coordinator binds the capability after committed marker validation
under ownership. Every save checks reference identity, live ownership, current
canonical marker, exact default r0, complete old archive and active absence.
Returned disk-prepared proof is not a live-lease/save capability.

All result outcomes contain three fixed-role active observations and three
fixed-role old-archive observations. Observation is closed to Missing,
Readable, Unreadable, or NotObserved (only when no ownership/authority permits
a safe probe); readable rows carry length/hash and the existing decoder class,
with revision only for valid classes. Non-readable rows carry no fingerprint.
Archive proof is closed to NotApplicable, Pending, Exact, Collision, Mismatch,
Unreadable, or NotObserved. No unauthenticated marker determines an archive
path. Before marker authentication, target and transaction ID are absent;
after authentication the exact target revision 0/length/hash and ID survive
recoverable failures. Best-effort snapshot failures record Unreadable rather
than granting success. Arrays are defensive copies with invariant role order.
Failure source is a closed internal enum covering lease, fresh observation,
directory, marker publication/validation, each archive/preservation operation,
default precommit/commit/postcheck, and snapshot probing.

Keep the existing closed outcome/state pairs: DiskPrepared/DiskPrepared,
ConfirmationStale/NoBarrier, Busy/NoBarrier,
ReloadRequired/DefaultCommitUncertain, and
ManualRepairRequired/ManualRepairRequired. A failure snapshot is evidence,
never authority. Prepared alone has its canonical marker/proof and requires
exact old archives plus target-only active rows. Default/unknown/mutated
combinations throw. Validate proof invariants before atomic one-consumer use.
Recoverable root/path probes are inside typed-failure handling; programmer
argument errors and fatal exceptions still escape safely.

## Verification honesty and scope

Fault/interruption tests must prove the actual before/after disk bytes, retained
barrier, strict collision matrix, no resurrection and no same-call retry.
Simulated interruption is not actual process death. The existing allowlisted
PowerShell fixture proves separate-process contention and exact child-kill
handle release at constructed durable states, not execution of the transaction
inside that process. Do not claim AC-008 crash-at-transaction-checkpoint coverage
from it; any additional fixture path or real transaction worker plumbing needs
a separate bounded Astra approval before implementation.

The parent allowlist is unchanged. Acceptance requires independent Luna review
and actual Unity XML evidence; approval of this clarification is not completion
or acceptance of C1.
