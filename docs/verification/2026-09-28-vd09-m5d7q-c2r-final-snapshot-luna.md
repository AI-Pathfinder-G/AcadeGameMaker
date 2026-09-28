# C2R final snapshot — Luna independent digest

Date: 2026-09-28. Read-only verification; no Assets edits or Unity execution.

## Focused correction run

R31 executed 45/45 with zero failed/skipped/inconclusive; Editor PID 49260
exited 0 and is gone. XML SHA-256:
`510821792AB9FF66FA7FE65D01F85AF7711F0475AB0CFC64EC803986FE3558CE`.
The wrapper’s early missing-XML condition remains historical runner evidence,
not a test failure; the actual XML is complete and passing.

## Fresh process evidence

- R26 Prepare: PID 50812, `DiskPrepared`, exact old hash and transaction.
- R27 Resume: PID 49072, `Completed`, exact r0 hash, maps disabled, no
  history, one-shot receipt, old progress absent.
- R28 Prepare (separate base): PID 1296, `DiskPrepared`, exact old hash and
  transaction.
- R29 DeleteGate: signal PID 47288 matched Main’s child; Main killed the exact
  PID after deletion and confirmed termination. No NUnit/XML pass is claimed.
- R30 OrdinaryAfterDelete: PID 28508, preparation=1, UTC=1, reset receipt
  none.

The six copied phase artifacts are present and match these outcomes. This is
fresh evidence for AC-M5D7QC2R-005/006 on the corrected adapter snapshot.

## Remaining gate status

R25’s final 74-row matrix and the R22/R23 required non-process regression
partitions remain pending. Therefore C2/C2R final AC-007/008 and parent
integration are not marked closed or accepted by this snapshot.
