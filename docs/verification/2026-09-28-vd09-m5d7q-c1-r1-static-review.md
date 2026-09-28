# M5D7Q-C1 R1 — NOT ACCEPTED

Date: 2026-09-28. Static inspection only. No Unity test pass is claimed.
Contract remains Approved with implementation/verification pending.

## Participation

Terra implemented the first disk-only draft. Astra inspected it and Luna
independently reviewed source against `AC-M5D7QC1-001..009`. Astra rejected
integration and assigned Terra R2 corrections. No production hookup occurred.

## Blocking findings

- `AC-009`: undefined IsHash in result validation prevents compilation (Luna P0).
- `AC-003/004/007`: archive-complete check still required old active source
  absence; new Primary at the same path would invalidate normal prepared resume.
- `AC-006/007`: exact new Primary plus interrupted Temp erroneously yielded
  repair after preserving Temp instead of revalidating prepared state.
- `AC-005`: held reset capability did not revalidate complete old archives.
- `AC-009`: proof omitted complete archive/active fingerprints and root equality;
  result validation admitted unsupported outcome/state combinations.

Earlier Astra draft findings also covered malformed canonical marker generation,
ancestor/reparse checks, collision handling and fixture signal-path restriction.
Those changes are not independently accepted merely because edits were made.

## Open coverage

R1 had three basic tests only. Full marker malformed/reflection matrix,
stage-specific fault/crash injection and real child-process contention execution
are open. Fixture presence alone is not `AC-008` PASS. The disk result snapshots
and prepared-proof contract also remain implementation gates.

R2 must close code correctness and expand deterministic tests before execution.
The eventual report must distinguish any bounded partial pass from complete
C1 verification and from parent Q-C memory/UI/transition completion.
