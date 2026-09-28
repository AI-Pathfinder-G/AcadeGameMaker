# C3 capture-lock counter-review — Luna

Date: 2026-09-28. Read-only bounded design review; no Assets, source, test,
specification, Unity, settings, or permission changes.

## Finding

The C3 path is `ProfileResetDiskTransactionV1.CaptureConfirmationIdentity`
(current source SHA-256
`168D1539EAF359C3242ECABADAC0776956370E52105007D9224566F7F9C3FDC4`), which
calls `ProfileRootOperationLockV1.Acquire` before the barrier and three-leaf
read. Its blocked exception is wrapped as an `InvalidOperationException` with
the collapsed blocked exception as `InnerException`; that preserves the
collapsed result, not its original acquisition cause. The related
`ProfileLaunchObservationAdapterV1` path has the same underlying lock issue.
The current `Acquire` implementation maps all `Directory.CreateDirectory`
`IOException`/`UnauthorizedAccessException`/`SecurityException` failures to
`ProfileRootOperationOutcomeV1.Busy`. It also maps `FileStream` unauthorized or
security failures to Busy, and retries every `IOException` until timeout before
returning Busy. The exception carries no inner exception or typed cause.

Therefore the current observation API cannot implement the C3 distinction
without guessing from messages or exception text: only a genuine lock
contention timeout may become `CaptureBusy`; root creation, permission,
security, or other unavailable safe-path failures must become terminal
`CaptureUnreadable`. The existing API is consequently insufficient for the
amended C3 capture contract, even though its ordinary durable behavior must
remain unchanged.

## Narrow recommendation

Add an Astra-approved, observer-only typed acquisition seam in the same lock
source/type, for example an internal `AcquireForObservation` returning a closed
outcome `{ Acquired(lease), TimedOutBusy, Unavailable }`. It must:

- preserve `Acquire` and every durable operation unchanged;
- classify only the Windows sharing/lock-contention HRESULTs
  `0x80070020` (`ERROR_SHARING_VIOLATION`) and `0x80070021`
  (`ERROR_LOCK_VIOLATION`) as retry, with timeout yielding `TimedOutBusy`;
- classify directory creation, unauthorized/security, malformed/unavailable
  root, and non-contention I/O as `Unavailable`, without message inference;
- return the same unforgeable `ProfileRootOperationLockV1` lease on success;
- let the C1 capture wrapper map only `TimedOutBusy` to `CaptureBusy` and every
  `Unavailable` result to terminal `CaptureUnreadable`, before barrier or leaf
  observation; and
- remain internal and read-only with no new authority, fallback, or C2/C1
  durable mutation.

The seam must preserve the actual `ProfileRootOperationLockV1` lease through
the existing private constructor; a caller-supplied Boolean or mutable “busy”
flag is not evidence. Required focused tests are a real held-share timeout,
root-file/directory-creation failure, and access/security failure. On a
portable or unknown error code, fail closed as `Unavailable` rather than
inventing Busy.

If the current target framework cannot expose a reliable typed sharing/lock
classification, the correct result is terminal `CaptureUnreadable` rather
than treating every `IOException` as Busy; do not use message matching.

## Verdict

**Counter-review: P1 design blocker, no P0.** The existing allowlist cannot
truthfully satisfy the Busy/unreadable boundary. Terra must stop at this seam
and Astra must approve the minimal observer-only amendment before any source or
test edit. C3 remains unimplemented and unaccepted.
