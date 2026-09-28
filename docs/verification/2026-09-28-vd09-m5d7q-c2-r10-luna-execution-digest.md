# M5D7Q-C2 R10 Luna execution digest

Date: 2026-09-28  
Reviewer: Luna (`gpt-5.6-luna`)  
Scope: independent digest of the supplied R10 focused PlayMode execution. No
Unity call was made by this review and no implementation acceptance is granted.

## Supplied execution evidence

- Focused XML: `41/41` tests passed; failed `0`, skipped `0`, inconclusive `0`.
- XML SHA-256: `579C8451AC800CF5BE315B171940FBC264B2EB822F3D67D71F2E791D624AB369`.
- Actual Unity Editor process: PID `21688`, test-run exit `0`.
- Wrapper/tool exit: early `1` before XML was available; the main Editor was
  unchanged and completed normally. This wrapper result is not treated as a
  test failure or as independent evidence of success.

The result is bounded C2 execution evidence only. It does not close C2R,
does not prove a true process restart, and does not by itself close parent
`AC-M5D7QC2-007`.

## Traceability

The supplied 41-row run selected the existing
`ProfileResetMemoryCutoverV1Tests` and `ProfileResetMemoryCutoverDataV1Tests`.
It did not execute the newly authored 73-row fault-matrix fixture. Its partial
trace covers AC-M5D7QC2-001 (actual Busy), AC-002 (staged cleanup), AC-003/004
(cohort/operation/retained-cell), AC-006 (receipt reflection), AC-008 (history),
and AC-009 (closed-shape checks), using the full M5D7QC2 prefix for each AC.
R10 still requires the separate independent source review and final Astra
integration decision. C2R process-A/process-B evidence is a distinct gate.

Astra corrected the execution-scope attribution above against the actual R10
XML after receiving the digest. No new execution or whole-AC closure is added.
