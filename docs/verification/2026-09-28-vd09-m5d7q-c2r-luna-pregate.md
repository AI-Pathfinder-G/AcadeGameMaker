# M5D7Q-C2R Luna pre-gate review

Date: 2026-09-28  
Reviewer: Luna (`gpt-5.6-luna`)  
Contract: `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c2r-restart-bootstrap.md`  
Contract SHA-256 (amended): `995816524904EA8A4ADC804E4688E7163740C198EF06A5E7CDA062F237E29A4E`

Scope is independent read-only architectural/source pre-review. No runtime
edits, Unity calls, implementation, or acceptance were performed. C2R remains
blocked until this pre-gate closes and current C2 R10 execution exits.

## Finding and disposition

No P0/P1 contract blocker found. The contract correctly identifies the parent
AC007 defect: ordinary `ProfileLaunchPreparation` rejects a committed barrier,
while current C2 requires an ordinary published cohort, so a true restart cannot
be claimed from existing launch behavior.

The proposed private `RecoveryReservedNoActions` lane is the appropriate narrow
correction. Adapter execution order `-220` precedes router `-210`; recovery
selection must happen in adapter `Awake`, before ordinary UTC/preparation reads.
Router `Awake` must validate only the private reservation and remain actionless.
Adapter `Start` must branch exclusively to recovery, call the real same-process
`ProfileResetDiskTransactionV1.Resume(root)`, and mint authority only from that
call's exact `DiskPrepared` result/proof plus the exact authored adapter/router/
normalized-root tuple. Existing ordinary `Awake`/`Start` currently performs
`ReservePreparedHubLaunch`, UTC/preparation, action adoption, and ordinary
publication; implementation must keep those calls unreachable on the recovery
branch.

The amended BCL barrier/root probe is sound as a safety classification only:
the existing Profile-owned root/ancestor/lock containment check runs first,
then both barrier paths receive real BCL attribute and regular-file reads.
`File.Exists` alone cannot select ordinary launch. Missing-file/directory
exceptions count as absence only after safe ancestor validation; unreadable,
directory, reparse, or reset-temp-only rows are repair-blocked. Selection does
no directory/lock I/O writes. `Resume` remains the sole C1 authority and must
revalidate the live root/marker under its lease. Busy before or during recovery
becomes `BusyBlocked` with no allocation/retry/fallback. Disable/destroy before
Resume or private mint closes the reservation and cannot enter legacy cleanup or
restore input authority.

## AC review map

- `AC-M5D7QC2R-001`: sound, but requires implementation proof of adapter-before-router ordering and zero ordinary Prepare/UTC/Confirm calls on barrier-present startup.
- `AC-M5D7QC2R-002`: sound; private mint must use same-call `Resume` identity and CWT/private witness checks, rejecting stale/foreign/reflected substitutions.
- `AC-M5D7QC2R-003`: sound; pristine recovery has no old actions, so old disposal is explicitly `NotApplicable`; candidate disposal remains exactly once after allocation; router lifecycle must stay quiescent.
- `AC-M5D7QC2R-004`: sound reuse of C2 semantics, but requires a distinct recovery Busy/staging/transfer/delete/final-reprobe matrix and no automatic retry/fallback rows.
- `AC-M5D7QC2R-005`: viable with a main-owned two-process fixture: process A creates real C1 `DiskPrepared`, exits; fresh process B runs real `Resume` then C2. Runtime/test code must not spawn nested workers.
- `AC-M5D7QC2R-006`: viable but requires a controlled process-death point after authenticated delete and before receipt assignment; next process must prove ordinary exact-r0 launch with no receipt replay.
- `AC-M5D7QC2R-007`: contract preserves the C2 exclusions and allowlist; static review must reject Profile/C1 edits, public ABI, fake history, map enable, load/save, cleanup, and configuration changes.
- `AC-M5D7QC2R-008`: amended gate is non-circular and sound: Luna reports contract P0/P1=0 before Astra approval; after Approved implementation, runtime regressions must have zero failed/skipped/inconclusive and Luna must independently report implementation P0/P1=0 before integration.

## Review gate

Astra alone may move C2R from Review to Approved. Terra implementation must
remain within the stated adapter/router/cutover/test allowlist. Parent
`AC-M5D7QC2-007` remains open pending actual distinct-process A/B evidence.
