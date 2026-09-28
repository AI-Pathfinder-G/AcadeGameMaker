# C2 R21 root-drift correction — Luna pre-gate

Date: 2026-09-28. Independent source review only; no Unity execution or source
edits.

## Frozen hashes

- Adapter: `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`
- Router: `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`
- Cutover: `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`

## Review result

The correction is narrow and contract-compatible. `HasTerminalCohortMirrorMismatch`
(`DesktopProfileLaunchAdapterV1.cs:128–133`) compares only the published
immutable original-cohort CWT owner/router/root/receipt against the duplicated
live adapter mirrors. It intentionally excludes prepared-action checks after
terminal transfer. `ValidateResetPair` (`:566–571`) invokes this check before
receipt staging and calls the existing terminal failure latch on mismatch;
`ValidateCurrentResetCellIdentity` (`:559–564`) also rejects a current-cell
identity mismatch. The restart-recovery branch remains unchanged.

This addresses R21’s `AfterFinalProbePairMetadataMismatch("root")` failure:
foreign `_launchRootWitnessA` must now return the existing ReloadRequired path,
without receipt/live-cell publication or acceptance of foreign actions. No P0
or P1 remains in this source correction by static review.

The 73-row R21 matrix must be rerun after the frozen correction; the prior
72/73 result is not acceptance. The additional root-b/data post-completion rows
must remain additive evidence and must not weaken the original mismatch row.
