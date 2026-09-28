# C2/C2R R35 fresh complete EditMode execution — Astra

Trace: `AC-M5D7QC2-010` / `AC-M5D7QC2R-007/008`.
This record starts with pre-execution evidence, not a passing result.

## Approved pre-execution gate

Terra planned the exact complete current partition; Luna independently reviewed
its selector and expected names, P0=0/P1=0. Astra approved one invocation in
the required-regression plan. R34 finished 536/536 with exact names, unchanged
204-file paths/hashes, and both recorded processes absent. Luna independently
accepted that actual partition; final C2/C2R acceptance still requires R35.

- Selection manifest: `artifacts/c2-r35-selection-preflight.json`, 613 expected
  qualified names, selector length 2364, SHA-256
  `389C515C521ACEBA78C710AB2B20845803232EDD083F09C788A2A000A6650161`.
- Fresh dependency manifest: `artifacts/c2-r35-source-before.json`, SHA-256
  `FC779816AF74773EBF31F309384655F0B1091FA230F745D31491326C39E2B146`.
  It captures 667 explicit files: 570 code/code-meta/asmdef files, 52 directory
  metas, and 45 additional authored/configuration/provenance files, including
  the top-level AcadeGameMaker folder meta. All six fixed runtime/replacement
  test anchors match their approved frozen fingerprints.
- The 51 internal EditMode process-worker cases run normally in this partition.
  The four separately orchestrated external PlayMode phases are not invoked.
- Historical 592 passes and the unreconstructed 202-file digest are not reused
  as final evidence. Every current case must execute freshly.

## Planned invocation and acceptance boundary

Main alone invokes the Verified UQW runner once in EditMode using the exact
manifest selector, fresh paths `artifacts/c2-r35-final-editmode.xml` and `.log`,
and AwaitSeconds 10800 (observation bound only). No source, assertion, Skip,
NUnit timeout, assembly, package or setting changes are made. No competing
Unity execution or Assets edit is permitted during this frozen regression.

Require actual owned-Editor exit, exactly all 613 expected qualified names,
zero failures/skips/inconclusive, complete before/after path/hash equality,
and independent Luna review before Astra considers C2/C2R integration. No
current source can be accepted based only on historical identical test names.

### Pre-launch command resolution correction

One Main shell attempt used a nonexistent conventional PowerShell installation
path. It failed before loading the UQW script; neither R35 XML nor log existed.
The outer shell's exit 0 is not a runner or Unity pass. Main resolved the actual
bundled `pwsh` command and independently observed version 7.6.5. The next
invocation uses that resolved command with a terminating command-resolution
check. This is a pre-launch correction, not a duplicate test run or a quota,
authentication, licensing or product-test failure.

## Actual invocation and newly identified input-provenance gap

The resolved PowerShell 7.6.5 invocation passed every UQW environment check
and started one actual Editor. Tool session is 72993; UQW authenticated owned
PID 40032, observed StartTime `2026-09-28T20:55:35.6062163+09:00`.
The XML is pending. No duplicate Unity run or Assets change is authorized.

After launch, Main inspected `ProfileResetDiskProcessV1Tests.BuildWorker` and
found two additional build inputs outside Assets:
`qa/fixtures/ProfileResetCrashWorker/Program.cs` and
`qa/fixtures/ProfileResetCrashWorker/ProfileResetCrashWorker.csproj`.
They are not present in the preserved 667-file before manifest. Its stated
complete-closure intent is therefore not proven for the 51 internal worker
rows; do not relabel a later hash capture as before R35. Preserve the original
manifest/hash and finish the actual R35 invocation unchanged.

Terra and Luna independently review a bounded correction: capture a complete
fresh worker/shared/configuration closure and execute affected worker rows
again only after R35's Editor exits. If exact dependency/name-set review
supports it, retain only R35's other 562 current rows and the fresh 51 worker
rows, with unchanged shared source evidence. Astra must approve that execution
partition before it starts; this note does not authorize a run, alter criteria,
reuse old 592 passes or claim current C2/C2R acceptance.

## Actual completion

The Verified runner completed session 72993 with exit 0,
`Completed: passed=613 total=613`, launcher ancillary exit 0. XML contains
exactly all 613 expected names (613 distinct), missing/extra 0 and
failed/skipped/inconclusive 0. Duration is 2435.2359838 seconds; start
`2026-09-28 11:55:59Z`, end `2026-09-28 12:36:34Z`.

- XML SHA-256:
  `7F1B78659A0F6E13567D4ACFD07BFA6B6A24E4FE8DF913169CFE6DB557D5840E`.
- Log SHA-256:
  `9B45373FF0020629AF0E364BF4297D7D2010D7827D42DF709EEB4877DF5BC57C`.
- Main observed owned PID 40032 absent at
  `2026-09-28T21:37:31.9010459+09:00`.
- Re-enumerated shared manifest: before/after 667 paths, path differences 0,
  missing files 0 and SHA-256 differences 0.

This is a factual 613/613 execution, not resolution of the missing worker-input
provenance. Exactly 51 names belong to the worker fixture; only the other 562
are eligible for the approved current evidence union until fresh R36 succeeds.
No original XML or before-manifest is rewritten. Final C2/C2R acceptance and
the temporary worker-evidence P1 remain open pending R36 and Luna actual review.
