# C2 R25 final matrix digest — Luna

Date: 2026-09-28. Read-only evidence review; no Unity execution or source edits.

R25 executed the complete corrected fault matrix: **74/74 passed**, zero
failed/skipped/inconclusive; duration 1966.6145073 s; Editor PID 52284 exited
0 and is gone. XML SHA-256:
`FD9B0E7B1FE60ABDBADAB2FF4C0AAD309B5635D4D4188264DA4BB8E43292D396`.
The XML contains the full qualified case inventory for the matrix, including
the original root-drift assertion, symmetric root-B drift, all
checkpoint/operation rows, junction cleanup, and typed corruption cases. The
separate two-row post-completion live-cell/receipt data fixture is not
attributed to this XML; it remains covered by its own frozen source and focused
execution evidence.

R25 closes the matrix evidence for the applicable C2 AC-002–006/008 rows and
supports C2/C2R terminal, root-binding, receipt, cleanup, and no-publication
claims. Main also confirmed the frozen six-source baseline hashes: Adapter
`0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`, Router
`66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`, Cutover
`60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`, and the
final matrix/data fixture hashes recorded in the preceding pre-gate.

## Remaining closure checklist

Final acceptance remains open until the scheduled R23 EditMode and R22
non-process PlayMode partitions each produce actual XML with exhaustive
fully-qualified case-name/count reconciliation, zero failed/skipped/
inconclusive rows, and the process class excluded only from the non-process
partition. R14/R31 bootstrap/process evidence and R25 matrix evidence must be
aggregated without claiming an unfiltered full-suite pass. No new runtime or
contract issue was found in R25.
