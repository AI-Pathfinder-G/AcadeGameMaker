# C2/C2R R35 EditMode actual execution — Luna independent digest

Date: 2026-09-28  
Scope: read-only independent audit of the completed R35 XML/log, exact-name
partition, and 667-file source manifest. No Unity/CIM operation, source/spec/
test edit, or acceptance action was performed by Luna.

## Actual evidence

- Tool session `72993`: exit `0`, `Completed: passed=613 total=613`; launcher
  ancillary exit `0`.
- XML `artifacts/c2-r35-final-editmode.xml`: SHA-256
  `7F1B78659A0F6E13567D4ACFD07BFA6B6A24E4FE8DF913169CFE6DB557D5840E`.
- Log `artifacts/c2-r35-final-editmode.log`: SHA-256
  `9B45373FF0020629AF0E364BF4297D7D2010D7827D42DF709EEB4877DF5BC57C`.
- XML counters: total `613`, passed `613`, failed/skipped/inconclusive `0`,
  duration `2435.2359838` seconds.
- XML start/end: `2026-09-28 11:55:59Z` to `2026-09-28 12:36:34Z`.
- Recorded owned PID `40032` was absent at the recorded post-run observation;
  Luna does not infer additional process identity.

## Independent name and manifest checks

The XML has `613` distinct qualified names and matches the R35 preflight set
exactly: expected `613`, missing `0`, extra `0`. The split is exactly `51`
`ProfileResetDiskProcessV1Tests` worker names and `562` other current names.

Luna independently rehashed the complete R35 before manifest
`artifacts/c2-r35-source-before.json` (SHA-256
`FC779816AF74773EBF31F309384655F0B1091FA230F745D31491326C39E2B146`):
`667/667` entries checked, missing paths `0`, hash differences `0`.

## Verdict and evidence boundary

**Luna actual-run verdict: PASS, P0=0, P1=1 temporary evidence blocker.**
The `613/613` execution is factual and current; the non-worker `562` names are
eligible for the approved evidence union. The 51 worker names remain excluded
from final provenance until the separately approved R36 fresh worker run with
the complete 669-input manifest (667 shared files plus the two worker inputs)
and exact before/after equality.

The source-before hash is corrected here to the independently recomputed value
above; any prior `...EBB31...` transcription is superseded. R35 does not
rewrite R22, reuse old 592 passes, or establish equality to the
historical 202-file digest. Final C2/C2R acceptance remains open pending R36
worker provenance and Astra's independent integration decision under
`AC-M5D7QC2-010` / `AC-M5D7QC2R-007/008`.
