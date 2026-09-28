# C2 R24 focused execution digest — Luna review

Date: 2026-09-28. Read-only review; no source edits or Unity execution.

R24 ran 45 cases: 41 passed, 4 failed, 0 skipped/inconclusive; duration
333.3807241 s; Editor PID 49464 exited 2. XML SHA:
`A16F92F95F9C973ACC9B875378F2697C2447A846EAF5814AA97AE70979C4EF5C`.

## Failure classification

All four failures are test expectation mismatches in the two root-drift cases
(matrix root-A/root-B and data rows). The runtime correction correctly rejects
the live `CurrentResetSession`/retained-cell identity after root mirror drift,
keeps terminal/no-new-receipt/no-notification/maps-disabled behavior, and
preserves the detached receipt.

`DesktopProfileLaunchAdapterV1.CurrentReceipt` (`:77–85`) is the approved
historical getter: it validates the immutable historical receipt and its CWT
cohort binding, not the mutable root-A/root-B diagnostic mirrors. Root drift
therefore must not erase or reject the historical receipt; it should remain
readable and equal to the pre-cutover history. The matrix helper at
`ProfileResetMemoryCutoverFaultMatrixV1Tests.cs:296–301` currently requests a
throw for root/root-b, and the data rows at `ProfileResetMemoryCutoverDataV1Tests.cs:139`
also request a throw. Those expectations should be changed to exact historical
receipt equality while retaining all live-cell/root/document/generation
rejections and terminal/no-receipt publication checks.

This is a test-only P1 gate, not a runtime defect and not grounds to weaken the
root-drift guard. After correcting the four expectations, rerun the full 74-row
matrix plus the focused 45-case set; no 41/45 or 73/74 result is acceptance.
