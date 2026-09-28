# C2R R23 regression pre-gate — Luna review

Date: 2026-09-28. Read-only review; no Assets edits or Unity execution.

R23 XML: `c2-r23-final-editmode.xml`, SHA-256
`9A9309CF1B5208EE478782D58F0DF18663E38637792B55A4A04E6FF501E5CCC6`.
Result: 595 passed, 2 failed, 0 skipped/inconclusive; duration 1474.5806043
s; Editor PID 17868 exited 2 and is gone.

## Failures and bounded remedies

1. `HubMenuPresentationControllerV1Tests.AC006_AllControllerAndNestedProofBackingsFailClosed`
   timed out at 180 seconds. Main’s inspection shows one fixture loops the
   three value-field groups plus 12 Ready, 2 Pending, and 2 Consumed controller
   backing fields. Splitting this into 17 fresh, real Build/Create rows—one
   retained value-row helper plus 16 exact controller-state cases—preserves the
   original setup, corrupt-backing assertions, and AC-006 coverage. No timeout
   increase, Skip, shared-state reuse, or helper weakening is authorized.

2. Q0 `AC011_EvidenceBackedCurrentSourceHashesMatchTheAuthoritativeManifest`
   fails because it requires historical Router/Adapter hashes `6C1B...` and
   `3A31...`, while the approved current snapshot is `66CD...` and `0DC2...`.
   This is a stale audit-manifest gate, not a runtime failure. A bounded
   test-only correction may retain all nine historical manifest rows/checks and
   add exactly two pinned current successor hashes with provenance to the R25
   Luna and process-execution evidence. The remaining seven strict checks stay
   unchanged; no old-or-new fallback is acceptable.

R23’s other 592 passing rows remain reusable only after an unchanged-runtime
and shared-helper comparison. The two failed rows and old four-row Q0 audit
must be excluded from any accepted aggregate until R32 reruns all 17 controller
rows plus all four Q0 audit rows with zero failures/skips/inconclusive.

This is a test/evidence P1 gate, not a product-runtime P1. R25 matrix, R31
focused/process evidence, and frozen runtime hashes remain valid; final C2/C2R
acceptance remains open.
