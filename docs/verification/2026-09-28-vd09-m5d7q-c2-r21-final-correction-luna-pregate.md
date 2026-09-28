# C2 R21 final root-drift correction — Luna pre-gate

Date: 2026-09-28. Static/test-source review only; no Unity execution or source
edits by Luna.

## Frozen test inputs

- Fault matrix, now 74 rows: `7FCBF75770677402BCE018CF38CA9437DB68DD613BCEC474E173E019B3AD071A`.
- Data fixture, two post-completion rows: `8D220F9D86B587884327BED55ED6953DDA89D210B14AD27A68335DE856BF294D`.
- Runtime correction remains Adapter `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`, Router `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`, Cutover `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`.

## Review result

P0: 0. P1: 0 by static review of the correction and additive tests.

The matrix retains the original root-drift case and adds the symmetric
root-B mutation, preserving the original root-A expectation and foreign-action
isolation. The two data rows mutate completed live-cell/current-receipt mirrors
and verify that live getters reject while the detached historical receipt still
validates, maps remain disabled, and no notification is emitted. These are
additive checks; they do not weaken the original failure expectation.

The runtime mirror guard compares only immutable published original-cohort
authority against live root/router/receipt mirrors, and the existing pair
validation latches failure before receipt staging. Restart recovery remains on
its separate branch. The 74-row matrix must still execute fresh with zero
failed/skipped/inconclusive rows before acceptance.

The helper is invoked by the original `ValidateResetPair`/live-cell path only;
the recovery branch, recovery CWT/root/cutover flow, C1 Resume, and process
fixture are unchanged by this correction. Because the Adapter hash changed,
fresh four-phase process execution is recommended for final evidence even
though the prior R16–R20 evidence was independently valid for its earlier
adapter snapshot.

## R24 historical-getter test correction

The authorized test-only correction is now frozen:

- matrix test SHA: `1AD60243FB3902E52CC5BD9D05C5A4C4DBEC7ADE1060979CAA79659EA32A85E4`;
- data test SHA: `9F61A4EB2278C8069742BDA56BCF13140E1B717826908D5D6BC64B60B9EEB8A0`.

The matrix keeps historical `CurrentReceipt` readable for all mirror-drift
variants while retaining terminal, null-new-receipt, live-cell rejection, and
foreign-action checks. The data rows assert exact historical receipt equality
and add live-cell `Document` rejection; detached receipt validation, maps, and
notification checks remain. P0=0/P1=0 for this test-only correction. R24's
41/45 result remains preserved; fresh 74-row and focused reruns are required.
