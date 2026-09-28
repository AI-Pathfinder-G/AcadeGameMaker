# C2 R21 fault-matrix digest — Luna independent review

Date: 2026-09-28. No Unity execution or source edits by Luna.

R21 executed the 73-row matrix: 72 passed, 1 failed, 0 skipped/inconclusive;
duration 1806.1421165 s; Unity Editor PID 26536 logged and exited 2. XML SHA:
`6C361431CBD48E12F9BAB1AAAFA506C436BFDB471DA4A395A7A107D97CB1FDE8`.

## P1 — final-pair validation misses a mutated diagnostic root witness

The failing row is
`AC_M5D7QC2_004_006_AfterFinalProbePairMetadataMismatchCannotStageReceipt("root")`.
Its control mutates `_launchRootWitnessA` to a foreign path after the final
probe. The required result is `ReloadRequired`, no receipt/live-cell
publication, and untouched foreign actions. Instead the result is Completed.

The source path at `ProfileResetMemoryCutoverV1.cs:124–125` calls
`owner.ValidateResetPair(cap)` after `AfterFinalProbe`, then stages the receipt.
`DesktopProfileLaunchAdapterV1.cs:555–559` validates the immutable cohort root
and router/action pair but does not first reject a diagnostic witness mismatch.
The existing `HasCohortDiagnosticMismatch` path (`:114–122`) is the narrow
authoritative check intended to detect A/B root witness corruption. This is a
real P1 against AC-M5D7QC2-004/006, not a test expectation defect.

Required correction is narrow: before final receipt staging, validate the
immutable cohort/authority binding together with the diagnostic root witness
consistency; on mismatch contain the terminal state and return the existing
ReloadRequired outcome. Do not weaken the row, replace immutable authority with
mutable fields, or permit receipt publication. Re-run all 73 rows after the
correction; 72/73 is not acceptance.

R14 bootstrap and R16–R20 process evidence remain valid but cannot close the
overall gate while R21 and the required non-process regression union are open.
