# C2R R12 execution review — Luna independent scope finding

Date: 2026-09-28. R12 ran the frozen required filter; no source edits or Unity
execution were performed by Luna.

## Observed execution

R12 result: 6 passed, 4 failed, 0 skipped/inconclusive; duration 72.586 s;
Unity PID `45232`, actual exit `2`; XML SHA-256
`EA897686F1D0EF856F1E3B870A3D0009029E82E44C23B89902FC080FDD07152B`.

Passed evidence includes the original C2 happy path, production Adapter Awake
C2R happy path, exact r0/current generation, single-root A/B behavior, Busy,
ordinary launch, and junction cleanup. Three ManualRepair rows failed during
test cleanup with a second cleanup attempt against the root registry; that
cleanup issue is separately delegated and is not reclassified here.

## P1 — C2R selection/promotion is currently applied outside the authored
HubUIOnly scope

The remaining legacy failure is the strict `AC001` trace at Start:
`GetPreparedActionsForLaunchCohort` receives null owner-proof fields for the
existing non-HubUIOnly desktop variant. The same unconditional Adapter Awake
selection also applies C2R returned-root ManualRepair promotion to non-Hub
variants; their `ReservePreparedHubLaunch` intentionally has no HubUIOnly owner
proof, so promotion is not an authorized private C2R cohort and throws rather
than preserving the historical non-Hub launch behavior.

This is a P1 scope/regression finding, not evidence that HubUIOnly recovery is
unsafe. The approved C2R requirements already constrain the lane to the actual
authored pristine `HubUIOnly` router (`IsHubUiOnlyVariant`, no actions/maps/
callbacks), and the private recovery reservation is explicitly a HubUIOnly
boundary. The implementation should therefore make C2R selection/promotion
explicitly HubUIOnly-only. Non-Hub adapters must retain the original
Reserve → single getter → Normalize/root failure → UTC → Prepare → ordinary
receipt semantics, with no reset witness/CWT or C2R ManualRepair promotion.
Restore the four legacy root-unsafe rows' expected historical exception/log
behavior; keep the new ManualRepair expectations on actual HubUIOnly bootstrap
rows. Do not weaken `GetPreparedActionsForLaunchCohort` or accept null proof as
a generalized cohort proof.

## Gate

R12 is not a final pass. The HubUIOnly production happy path and C2R-specific
assertions are encouraging but cannot close ACs while the legacy non-Hub scope
regression and cleanup failures remain. After a narrowly scoped correction,
rerun the legacy trace/root rows and the C2R focused rows; retain the separate
four-process Main evidence requirement for AC-M5D7QC2R-005/006.
