# C2/C2R R36 — fresh worker provenance execution

Date: 2026-09-28. Astra execution authority: approved required-regression plan;
independent pre-gate: Luna worker-provenance review. References:
AC-M5D7QC2-010 and AC-M5D7QC2R-007/008.

## Frozen inputs and bounded execution

- Selection: `artifacts/c2-r36-selection-preflight.json`, SHA-256
  `B2CF18E155D98BA7ECAC1E8CABE5EAF90CF6C48CC0419D1ECC800758220120E3`.
- Fresh before: `artifacts/c2-r36-source-before.json`, SHA-256
  `2CAB80CC20A01165778905F67161866906F439C6E5AAFC5E316D1813628F5EA4`.
- Complete 669 inputs: 667 shared R35 inputs with zero differences, plus
  Program.cs and ProfileResetCrashWorker.csproj. Repository build-config scan
  is explicitly empty. Observed installed SDK: 10.0.401.
- Execute exactly 51 worker cases with the verified owned-Editor QA runner;
  leaf-prefix selector excludes fixture-parent selection. No manual external
  restart phases, source edits, criterion changes, skips, timeout changes or
  historical pass reuse. AwaitSeconds 10800 is an observation bound only.
- R35 owned Editor has exited. R35 contributes only the 562 non-worker names
  until this fresh run and independent verification close the evidence gap.

## Pre-launch status (historical)

Pending launch. No execution pass or final C2/C2R acceptance is claimed.

The first shell precheck exited 1 before invoking the runner: the shell used
`Hash` instead of the manifest's `SHA256` property. No Unity process or output
was started. Correcting this comparison does not modify frozen inputs or tests.

The corrected invocation passed all environment checks and started one actual
Editor. Tool session: 11454; runner-authenticated owned PID: 39752; observed
StartTime: `2026-09-28T21:47:51.9652523+09:00`. XML is pending; no duplicate run
or source modification is authorized during this execution.

## Actual completion and source closure

Session 11454 completed with exit 0, `Completed: passed=51 total=51`, launcher
ancillary exit 0. Exact qualified names: 51 distinct, missing/extra 0;
failed/skipped/inconclusive 0. Duration 269.7467068 seconds, start
`2026-09-28 12:48:11Z`, end `2026-09-28 12:52:41Z`.

- XML SHA-256: `6ADC451890CADDE6729E0D654D122042CFC12FB7B3ED2D1309ED2643C12A76B8`.
- Log SHA-256: `641D6DA917936736F35FBEA110AEC3C414DB2779DA75347DAB082C2AAD330EE2`.
- Owned PID 39752 observed gone at `2026-09-28T21:54:10.4841260+09:00`.
- Full before/after: 669 paths, path differences 0, SHA-256 differences 0.
  All shared 667 R35 inputs remain identical. Repository build configs remain
  explicitly empty; SDK remains 10.0.401.
- After evidence: `artifacts/c2-r36-source-after.json`. Its full rows retain
  the before values only after every current path was freshly rehashed and
  exactly matched; this is not a retroactive R35 before capture.
  SHA-256: `B0D8D917D1A9D65B0D4B2B66EDEDAEB4C097BE7F2A36FA01FDFE603E8303E218`.

The eligible current EditMode union is R35's 562 non-worker rows plus R36's
51 fresh worker rows, without historical-592 reuse. Independent Luna final
review and Astra integration remain pending; execution alone is not acceptance.

Main additionally compared the assembled current result set against all R35
expected names: 562 retained + 51 fresh = 613 distinct; missing/extra 0. No
qualification-name or shared-source drift is hidden by this partition.
