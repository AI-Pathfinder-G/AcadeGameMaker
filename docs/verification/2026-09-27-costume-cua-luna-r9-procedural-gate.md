# Costume CUA — Luna R9 retry procedural gate

Date: 2026-09-27 (Asia/Seoul)

## Scope

Static-only review of Astra's R9 retry authorization. Unity was not run and implementation files were not modified.

## Contract and immutable prior failures

- Current contract SHA-256: `251A4C41B711933D06BC7C4A2174B2B77CE9437655D4E9EFEED9204B5471E865`
- R7 focused-log SHA-256: `2E48CCACE390FE16B3F327497D6BA1DD6133071E3A04583CD9CEE8AC3E1AE14A`
- R8 focused-log SHA-256: `A759B4E6D0C49321F79C5AB40BEE023BC5F753E2E50E9AC4223A38806743C579`
- Corrected adapter-test SHA-256: `8E93A8293ABD0E79913628E74A9FDC54E57D37C20022D053308037136358CF99`
- Corrected media SHA-256: `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`

Both prior logs exist at their recorded paths with exact hashes. No R7/R8 result XML or full-suite result is present.

## Procedural checks

The R9 clause is complete:

1. It withdraws unused R8 full-suite stems and preserves both R7 and R8 failure logs as immutable evidence.
2. It authorizes the same exact three commands, order, Unity version (`6000.6.0f1`), assembly/filter boundaries, and no-`-quit` behavior as the prior authorization. No new command or scope is introduced.
3. It names only fresh outputs below `artifacts/unity-results/costume-cua-20260927/`:
   - `costume-cua-r9-focused-playmode` (`.xml` and `.log`)
   - `costume-cua-r9-full-editmode` (`.xml` and `.log`)
   - `costume-cua-r9-full-playmode` (`.xml` and `.log`)
4. An independent recursive artifact check found zero matches for all six R9 filenames. The existing directory contains only the two preserved focused failure logs, so no R9 output can be mistaken for fresh evidence.
5. The existing hard-stop rules remain independently applicable to each R9 run.

## Verdict

**PASS — P0=0, P1=0, P2=0 for the R9 procedural gate.**

R9 execution is procedurally authorized subject to the exact command/order and hard-stop rules. This is not a Unity result PASS; the executor must produce and verify all six R9 artifacts.

