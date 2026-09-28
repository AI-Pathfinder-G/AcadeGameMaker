# C2/C2R R22 PlayMode pause digest — Luna

Date: 2026-09-28  
Scope: read-only diagnostic of the completed R22 XML; no rerun, filter/source/test change, or Unity operation.

## Actual evidence

`artifacts/c2-r22-final-playmode.xml` SHA-256:
`43D4FE3ABDC861E7CE70F0B2E6355D3AD801C47A7453F5901EFC905C9EA08FFB`.

The XML contains 614 cases: 610 passed, 4 failed, 0 skipped and 0
inconclusive. All four failures are the externally orchestrated process
fixture:

- `ProfileResetRestartProcessV1Tests.AC_M5D7QC2R_005_006_ProcessResumeCompletesFreshAuthorGraph`
- `ProfileResetRestartProcessV1Tests.AC_M5D7QC2R_005_ProcessPrepareWritesDiskPreparedEvidence`
- `ProfileResetRestartProcessV1Tests.AC_M5D7QC2R_006_ProcessDeleteGateSignalsAfterActualDeleteAndWaitsForMain`
- `ProfileResetRestartProcessV1Tests.AC_M5D7QC2R_006_ProcessOrdinaryAfterDeleteHasNoResetReceipt`

Their missing externally supplied process environment is an execution/setup
failure, not evidence of a product-runtime regression. They remain failures;
they are not waived or relabeled as a pass.

## Partition defect and evidence boundary

The preserved R22 runner partition did not exclude the external process class
and may also have included the R25-owned 74-row memory-cutover matrix. Thus
R22 is not the required non-process partition. The 610 passing rows are useful
diagnostic evidence, including the historical 446 qualified baseline cases,
but cannot close the zero-failure aggregate until a correctly partitioned run
or equivalent explicit reconciliation exists. Matrix overlap must be removed
when reconciling against R25; the only intended combined exclusion is the
external process class, with R25 owning the matrix.

R25’s independent 74/74 matrix and the separately evidenced R26–R30 process
phases remain valid records under their frozen source hashes. They do not
convert this R22 XML into a passing run. C2/C2R acceptance remains open under
the required zero-failure gate.

R22 is now paused as requested; no further execution was performed.

## Reconciliation correction

The independent full-name reconciliation confirms all 446 historical scoped
baseline names are present. It also confirms that R22 included all 74 R25
matrix names: the 614 XML cases are 446 baseline names plus current additions,
with the 74 matrix names overlapping that set (not 688 unique cases). The
intended exclusion regex therefore failed for both the matrix and process
classes, not only the process class. The frozen 202-source dependency digest
remains `7D712A7326C121AE2DA439E16368968A7A3A716E7CAF1E0E444E18FC4C9CBD1E`.
This correction does not alter the failed-run classification or waive the
zero-failure gate.
