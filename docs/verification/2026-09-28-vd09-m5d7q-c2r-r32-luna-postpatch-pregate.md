# C2R R32 post-patch independent pre-gate — Luna

Date: 2026-09-28  
Scope: frozen EditMode test-only patch review; no Unity execution, source/runtime edits, or acceptance.

## Frozen evidence

- Controller test final SHA-256: `40AA974BF8407A6FD2AAE57830256F04599AF1A835F708999BDC10F332061699`.
- Controller pre-patch snapshot SHA-256: `218EB71BDD168C59A9DBF03082A37F31741A94AE1A360D62BA4D15C89099FA5F`.
- Q0 audit test final SHA-256: `12B98E06919CDC2AFFBD9E380BF2B4546C00D3BA86A8F86E5D9EDF59FFE331FD`.
- Q0 pre-patch snapshot SHA-256: `23BE8E126687609321C7E26968ECC2F552C2AD8AE96E6F1FDDA2A04A5E30A8F8`.
- Runtime remains frozen at Adapter `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`, Router `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`, and Cutover `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`.

## Review result

No P0/P1 defect found in the frozen patch. The controller patch retains the original nested value-backings loops and helpers, while adding a `TestCaseSource` with one `AC006_NestedValueBackings`, twelve fresh Ready controller-field cases, two Pending cases, and two Consumed cases: 17 generated cases total. The old aggregate method remains available for historical provenance but is not part of the replacement filter.

The Q0 patch retains the nine historical evidence rows (seven current historical rows plus the two non-current evidence rows), verifies the seven historical current hashes exactly, and adds exactly two current successors (Router and Adapter) with R25/process-execution provenance. There is no old-or-new hash fallback.

Recommended R32 focused filter (actual generated names; 17 controller + 4 Q0 = 21 cases):

`HubMenuPresentationControllerV1Tests\.AC006_(NestedValueBackings|Ready_|Pending_|Consumed_)|HubUiOnlyQ0ScopeAuditEditModeTests`

The 592 unchanged passing R23 rows may be reused only as an explicitly evidenced dependency-preserving partition: the frozen runtime/shared helpers and aggregate 202-source/meta/asmdef inventory are unchanged. Exclude the timed-out original AC006 row and all four old Q0 rows; execute the 21 replacement cases fresh and require zero fail/skip/inconclusive before aggregating 592 + 21. R25 74/74 matrix, R31 focused 45/45, and the fresh process phase records remain valid because their covered source hashes are unchanged.

This remains a pre-gate: R32 execution and the other required regression partitions are pending. No acceptance or implementation claim is made.
