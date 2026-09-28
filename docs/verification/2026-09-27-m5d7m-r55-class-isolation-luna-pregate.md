# M5D7M R55 class-isolation diagnostic — Luna pre-gate

Date: 2026-09-27 (Asia/Seoul)

## Scope

Independent static pre-gate of Astra's R55 diagnostic amendment. Unity was not run and no source, test, scene, package, setting, or existing evidence was modified.

## Inputs

- Contract SHA-256: `E42CE555E4FAE3DF75DEA8C068D75F1DD241DB8D69BA4394DB0C571834A0B2AA`
- R54 XML SHA-256: `D76894EADADE98329843DC093A1D5C5E74F165FB56E4EF283D3A160F0435A284`
- R54 log SHA-256: `34AE43B68B5363589DD6288DCE3FA224352EF7A6AC4D22421E382CF146DAF2C1`
- Verified R54 result: 1/1 passed, failed/skipped/inconclusive 0, duration 22.2733782 s.

## Checks

1. R55 is explicitly diagnostic-only: one PlayMode run, no source changes, no retry, and no `-quit`, using the whole class filter `AcadeGameMaker.Input.Unity.PlayMode.Tests.DesktopProfileLaunchAdapterV1Tests` on Unity `6000.6.0f1`.
2. Fresh outputs are exactly `m5d7m-r55-adapter-class.xml` and `.log` in the existing diagnostic directory. The directory currently contains only the preserved R54 pair; recursive search found zero R55 stem matches.
3. The 1,200-second external monotonic watchdog is justified by the contract's recorded accepted whole-class runs of approximately 946–985 seconds. It retains the exact PID-only stop, artifact preservation, drift accounting, and no-retry rules from R54.
4. R54's natural isolated-test pass correctly selects the suite-interaction branch; R55's pass/timeout branches are diagnostic classification only. A pass selects contamination outside the class; a timeout selects a test-only stage-marker amendment. Neither branch authorizes runtime, adapter, coordinator, InputRouter, or assertion implementation changes.

## Verdict

**PASS — P0=0, P1=0, P2=0 for the R55 diagnostic pre-gate.**

The single bounded class-isolation diagnostic may proceed under the exact amendment. This PASS is not a production or implementation approval.

