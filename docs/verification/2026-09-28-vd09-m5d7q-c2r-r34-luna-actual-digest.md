# C2/C2R R34 PlayMode actual execution — Luna independent digest

Date: 2026-09-28  
Scope: read-only independent audit of R34 actual XML, log, selector manifest,
and fresh source inventory. No Unity/CIM operation, source/spec/test edit, or
acceptance action was performed by Luna.

## Actual evidence

- Tool session `33682`: exit `0`, `Completed: passed=536 total=536`; launcher
  ancillary exit `0`.
- XML `artifacts/c2-r34-final-playmode.xml`: SHA-256
  `3E7FCA0E0F11C103125934407AE9515522B1F7B71DA1AB894A2740337C39DECD`.
- Log `artifacts/c2-r34-final-playmode.log`: SHA-256
  `E6A85D66FEFADCC16E17DA3944275F4EAA4172D240449B61F66B94D34202A9EE`.
- XML counters: total `536`, passed `536`, failed/skipped/inconclusive `0`,
  result `Passed`, duration `5990.6334909` seconds.
- XML start/end: `2026-09-28 10:08:24Z` to `2026-09-28 11:48:15Z`.
- Recorded actual PID `13200` and secondary `37628` were both absent at the
  recorded 20:48:47+09 observation. No PID is independently inferred beyond
  the execution record.

## Independent name and partition checks

The XML contains exactly `536` qualified test-case names. Comparing sorted
names to `artifacts/c2-r34-selection-preflight.json` gives `536/536`, missing
`0`, extra `0`. The XML contains zero
`ProfileResetMemoryCutoverFaultMatrixV1Tests` cases and zero
`ProfileResetRestartProcessV1Tests` cases. The preflight records all `446`
historical baseline names with missing baseline `0` and forbidden `0`.

R25’s preserved matrix XML contributes `74` names; the R34/R25 intersection
is `0`, yielding `610` distinct names. This reconciliation does not rewrite
R22’s historical `614/610/4` result and does not claim any external-process
phase pass.

The fresh R34 source inventory declares `204` paths. The execution record
reports identical before/after path sets and hashes (`added/removed 0`, SHA
differences `0`). The four frozen runtime anchors independently match:
Adapter `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`,
Router `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`,
Cutover `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`,
and C1 `168D1539EAF359C3242ECABADAC0776956370E52105007D9224566F7F9C3FDC4`.

Luna independently re-enumerated the eight declared roots and rehashed every
manifest path: `204/204` checked, missing `0`, hash differences `0`, and
normalized extras `0`. Seven parent-directory `.meta` siblings appeared only
when walking above the declared roots; they are outside the manifest scope and
were excluded from the normalized comparison.

## Verdict and boundary

**Luna actual-run verdict: PASS, P0=0, P1=0** for the R34 execution evidence
under `AC-M5D7QC2-010` and `AC-M5D7QC2R-007/008`.

This closes only the R34 partition evidence. It does not close final C2/C2R:
R35’s fresh `613`-case EditMode run remains required, the old `592` rows are
not reused as final evidence, and the unreconstructed historical 202-file
digest is not claimed equal to the fresh R34 inventory. Astra alone decides
integration.
