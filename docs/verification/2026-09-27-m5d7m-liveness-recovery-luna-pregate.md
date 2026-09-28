# M5D7M duplicate-liveness recovery — Luna diagnostic pre-gate

Date: 2026-09-27 (Asia/Seoul)

## Scope

Independent static pre-gate of the diagnostic-only contract. No Unity process was started, no source/test/scene/package/setting was edited, and no implementation authorization is inferred.

## Inputs

- Diagnostic contract: `docs/specs/work-contracts/2026-09-27-m5d7m-duplicate-liveness-recovery.md`
- Contract SHA-256: `2440F199B69CA15AEDBEC8852F80488D927EFB1E26BE1778ECAE1AC53810723D`
- Verified parent M5D7M contract: `docs/specs/work-contracts/2026-09-13-vd09-m5d7m-desktop-profile-launch-adapter.md`
- Parent contract SHA-256: `09DAB4C3734DA01533944C78F786AE92D2706A30A7863C818C4C00140235C923`
- Preserved partial R11 full-PlayMode log SHA-256: `397B445907A90CB78F32F3DC66DDC6F1B9A3EAF7C4FD7DC26690BDFDDBC4F235`

## Checks

1. The diagnostic is explicitly `Approved for diagnostic execution only — implementation remains blocked`, with Astra authority, Terra execution, and Luna independent review. It authorizes exactly one Unity `6000.6.0f1` PlayMode run without `-quit`, filtered to `AcadeGameMaker.Input.Unity.PlayMode.Tests.DesktopProfileLaunchAdapterV1Tests.PhaseD4_DuplicateAdapters_LoserDoesNoPreparationAndCannotCloseWinner`.
2. The mandatory monotonic watchdog is exact: after 180 seconds without complete XML, record the exact PID, CPU, log length/last-write/tail, then stop only the exact task-created Unity processes; do not retry; preserve the partial log and generated test scene. If XML appears, wait for natural exit and record counts/duration/hashes.
3. The current diagnostic directory `artifacts/unity-results/m5d7m-liveness-20260927/` is absent, and no `m5d7m-r54-duplicate-liveness.*` artifact exists. The prior partial R11 log is present at its recorded path/hash, and the generated `InitTestScene7441...unity` plus meta remain present as drift evidence.
4. The allowlist forbids source, test, scene, package, setting, and existing-evidence modification. It permits only the contract, one pre-gate record, fresh diagnostic artifacts, and one diagnostic evidence record. It explicitly forbids changing the duplicate scenario, runtime adapter/coordinator, `InputRouter`, or assertions without a separately approved amendment.
5. The contract requires before/after source-hash and worktree-drift comparison, named generated artifacts, no overwrite, no indefinite process, and a single-run stop boundary. Its result branches are diagnostic only: isolated pass leads to suite-interaction investigation; isolated timeout leads to a stage-marker amendment. Neither branch authorizes a behavioral fix or implementation change.

## Verdict

**PASS — P0=0, P1=0, P2=0 for the diagnostic pre-gate.**

The one bounded diagnostic may proceed under the exact contract and watchdog rules. This PASS authorizes no implementation, retry, or behavioral amendment.

