# Costume CUA — Luna R8 retry procedural gate

Date: 2026-09-27 (Asia/Seoul)

## Scope

Static-only review of Astra's R8 retry authorization after the R7 compile stop. Unity was not run and implementation files were not modified.

## Contract and preserved failure

- Current contract SHA-256: `A9CEE479C8AB7A3150B60FE13FC88B82C6339E824777318A4FC013C5B3FC2388`
- R7 failure log: `artifacts/unity-results/costume-cua-20260927/costume-cua-r7-focused-playmode.log`
- R7 failure log SHA-256: `2E48CCACE390FE16B3F327497D6BA1DD6133071E3A04583CD9CEE8AC3E1AE14A`
- Corrected media SHA-256: `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`
- Corrected adapter-test SHA-256: `9C1C28CBC650E955BE64FFC0778F7BDB6129F56BA762D48049F0D6D47435CDF9`

The contract explicitly preserves the R7 failure log, states that no R7 result XML or full-suite result exists, withdraws unused R7 full-suite stems, and leaves the prior aggregate hashes unchanged.

## Procedural checks

The R8 clause is sufficient and exact:

1. It authorizes the same three commands and order as the preceding approved procedure, on Unity `6000.6.0f1`, without `-quit`.
2. It retains the focused PlayMode assembly/filter and the unfiltered full EditMode/full PlayMode runs by reference to the exact prior authorization.
3. It authorizes only fresh stems below `artifacts/unity-results/costume-cua-20260927/`:
   - `costume-cua-r8-focused-playmode` (`.xml` and `.log`)
   - `costume-cua-r8-full-editmode` (`.xml` and `.log`)
   - `costume-cua-r8-full-playmode` (`.xml` and `.log`)
4. The R7 failure log is explicitly immutable and cannot be overwritten or removed.
5. The prior target directory currently contains only the preserved R7 focused log. All six R8 filenames were independently searched under `artifacts/` and each has zero matches. Thus no R8 artifact can be mistaken for fresh output.
6. The same hard-stop rules apply independently to every R8 run; no result acceptance or production-scope expansion is implied by this gate.

## Verdict

**PASS — P0=0, P1=0, P2=0 for the R8 procedural gate.**

R8 execution is procedurally authorized subject to the contract hard stops and exact command/order rules. This is not a Unity test-result PASS; the authorized executor must produce and independently verify all six R8 files.

