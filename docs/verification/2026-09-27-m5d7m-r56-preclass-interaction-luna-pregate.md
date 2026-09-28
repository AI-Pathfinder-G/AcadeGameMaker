# M5D7M R56 pre-class interaction diagnostic — Luna pre-gate

Date: 2026-09-27 (Asia/Seoul)

## Scope

Independent static pre-gate of Astra's R56 diagnostic amendment. Unity was not run and no source, test, scene, package, setting, or evidence file was modified.

## Inputs

- Contract SHA-256: `74E24FAB05971A02C0018AB16280B0412C5C82559B169C36B4E36545A66EAE90`
- R55 XML SHA-256: `9B664706F9CC0814C34169673EF6751747F300FC4DD0BC6FD1283944F84E2EB8`
- R55 log SHA-256: `0D1551BE80E49CCB2FC5AD2AF57F482CFF3106BFEF03331AE1DC2611623FAB2B`
- R55 result: 117/117 passed, duration 982.0166498 s.

## Checks

1. R56 is one PlayMode run on Unity `6000.6.0f1`, without `-quit`, with no source changes or retry. The exact semicolon-separated regex set is:
   `^AcadeGameMaker\.Tests\.PlayMode\.CameraUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.CombatUnity\.;^AcadeGameMaker\.Tests\.PlayMode\.Input\.;^AcadeGameMaker\.Input\.Unity\.PlayMode\.Tests\.DesktopProfileLaunchAdapterV1Tests`.
2. The pure Input arm ends in the escaped terminal dot (`Input\.`). Independent .NET regex checks match `AcadeGameMaker.Tests.PlayMode.Input.*` but do not match `AcadeGameMaker.Tests.PlayMode.InputUnity.*`; therefore the filter does not accidentally include the `InputUnity` namespace.
3. Fresh outputs are exactly `m5d7m-r56-preclass-interaction.xml` and `.log` under the existing diagnostic directory. Both stems are currently absent; the directory contains the preserved R54/R55 evidence only.
4. The 1,500-second external watchdog is justified by the contract's component-duration total below 1,000 seconds while remaining below the prior 45-minute livelock. Exact PID-only stop, preservation, drift accounting, and no-retry rules remain mandatory.
5. A timeout classifies cross-assembly state contamination; a pass rejects this prefix and selects a later broader-order diagnostic. Neither result authorizes a runtime/test fix.

## Verdict

**PASS — P0=0, P1=0, P2=0 for the R56 diagnostic pre-gate.**

