# C3L observation-lease provenance — Luna pre-gate

Date: 2026-09-28. Reviewed the complete C3L draft in `Review` status,
SHA-256 `9FE666C476D9B3650721DD52FA0A5D61240A6F3B54192BDC56CEAD654A17B4ED`.
Read-only design review; no implementation, source/test/spec/Unity/settings, or
permission changes.

## Independent assessment

The exact C3 capture path is `ProfileResetDiskTransactionV1.CaptureConfirmationIdentity`
(`ProfileResetDiskTransactionV1.cs` SHA-256
`168D1539EAF359C3242ECABADAC0776956370E52105007D9224566F7F9C3FDC4`), which
calls `ProfileRootOperationLockV1.Acquire` and wraps the collapsed blocked
exception as an `InvalidOperationException.InnerException`. The wrapper
preserves the collapsed outcome but cannot recover whether the cause was
contention or unavailable access. The draft closes that exact counter-review
boundary. It adds only one internal
observation acquisition lane on the existing lock type, returns the real
private same-root lease, preserves legacy `Acquire`, capture, Begin/Resume and
C1/C2 durable bodies, and forbids public ABI, friends, caller booleans,
message inference, forged timeout provenance, fallback, and new authority.
Only exact Windows sharing/lock HRESULTs `0x80070020`/`0x80070021` may retry to
the existing five-second timeout. Root creation, access/security, unknown and
non-contention failures are terminal `Unavailable`; only authenticated actual
timeout maps to C3 `CaptureBusy`.

## AC review

- **AC-M5D7QC3L-001:** Pass by design: legacy bodies and ABI are explicitly frozen;
  the single source-file allowlist and no-friend/no-public rules are clear.
- **AC-M5D7QC3L-002:** Pass by design: a retained real exclusive FileStream is the
  only timeout evidence, and release-before-deadline must permit one real
  acquisition. No throw-only control can mint timeout provenance.
- **AC-M5D7QC3L-003:** Pass by design, with testability constraint below: root-file,
  unsafe lock path, unreadable/non-contention failures, unknown and corrupt
  provenance all fail closed as Unavailable; original exception identity is
  retained.
- **AC-M5D7QC3L-004:** Pass by design: same lease, exactly three reads, projection /
  identity agreement, barrier/root safety, and no mutation or success-on-fail
  path are explicit.
- **AC-M5D7QC3L-005:** Correct gate: no implementation or regression result is
  claimed; Luna must independently verify zero failed/skipped/inconclusive
  before Astra acceptance.

## Testability clarification

No real permission changes are needed or authorized. AC-M5D7QC3L-003 should use
safe real filesystem fixtures for root-as-file and lock-path-as-directory, plus
the actual retained-share contention fixture for AC-M5D7QC3L-002. Access/security
and other non-contention branches may use a narrowly scoped throw-only boundary
fixture that can return only typed `Unavailable` while preserving the original
exception; it must be structurally unable to mint `TimedOutBusy`, a lease, or
an observation identity. If the implementation cannot make that distinction
without a caller-controlled classification, stop and return to Astra.

## Verdict

**C3L design pre-gate: PASS, P0=0/P1=0.** The prior P1 is design-closed,
conditional on the exact allowlist and testability constraint above. C3L remains
unimplemented; Terra must not claim runtime acceptance, and Astra must record
Approved before implementation.
