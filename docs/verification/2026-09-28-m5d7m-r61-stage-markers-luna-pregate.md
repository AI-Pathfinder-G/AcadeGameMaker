# M5D7M R61 stage-marker Luna pre-gate

- Date: 2026-09-28
- Reviewer: Luna (independent QA)
- Scope: implementation/procedural pre-gate only; Unity was not executed.

## Evidence reviewed

- Approved contract R61 section in `docs/specs/work-contracts/2026-09-27-m5d7m-duplicate-liveness-recovery.md` (current SHA-256 `9A08DBA36FBA9BC8288DCAC7DA5D200A39572376FD900B4CF70400F7C8E52796`).
- The only named implementation file, `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/DesktopProfileLaunchAdapterV1Tests.cs`, current SHA-256 `6480CF5D84B3D20A32AC51D4E6C39FBC7CD0A30110FA2B5F1EFF428F3DE16154`; contract pre-marker SHA-256 `3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3`.
- Fresh stems `m5d7m-r61-d4-stage-markers.xml/.log` are absent. No Unity, UnityCrashHandler64, or UnityPackageManager process is present.
- Workspace porcelain remains the approved pre-amendment 511-entry baseline with joined SHA-256 `FA25B260BDCED6919281C2D51759F3752C882A2A6D3E41BA94BFAC3B95339F15`; no unexpected pre-run mutation was attributed to R61.
- The four CUA source/test baselines remain unchanged: Media `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`, adapter `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`, adapter tests `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`, view-presenter tests `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`.

## Marker-only review

Fourteen `Debug.Log` calls prefixed `M5D7M-R61` are present only around the authorized D4 phases at lines 493–514 and in `CreateDuplicateAdapter` around `parent.SetActive(true)` and its existing catch at lines 920–930. The existing `LogAssert.Expect`, context construction, `try/finally`, winner start, assertions, scenario data, and catch behavior are unchanged on inspection; no assertion, scenario, runtime, or control-flow change was observed. However, removing those 14 marker lines in-memory did not reproduce the contract's pre-marker SHA (`449C2011372B173F4D2756A3F17D013FB7E8A2A15C9F0C635DB8A5B44E91899D`, versus `3C02A838...`), including common line-ending/final-newline variants. Exact marker-only provenance therefore remains unproven and must be resolved before execution.

## Procedural verdict

- **AC-M5D7M-R61-001: FAIL (pre-execution provenance gate).** Marker placement and semantics are acceptable by inspection, but the exact pre-marker source hash cannot be reconstructed from the current file, so marker-only diff and source-baseline preservation are not independently established.
- **P0: 0; P1: 1; P2: 0.** Do not execute R61 until Terra/Astra resolves the hash/provenance discrepancy. If resolved, the bounded diagnostic remains subject to 1900s global and 180s exact-D4 stale-log/increasing-CPU watchdogs, no `-quit`, no retry/version probe, and exact task-PID-tree stop only.
- This pre-gate does not satisfy `AC-M5D7M-R61-002..004`, does not authorize a behavioral fix, and does not close CUA `AC-CUA-009`.

## Second independent audit after marker reinsertion

Terra reinserted the authorized 14 marker statements without a behavioral
change. The current source SHA-256 is
`30CC6D547F56B7C5F4596535B00DD481FBC8EB41170717BE9C7446AADA5647E`.
Removing the exact marker statements plus their insertion whitespace
mechanically in memory reproduces the original source SHA-256
`3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3`.
The current source contains exactly 14 `M5D7M-R61` occurrences. The
mechanical restoration result, together with the unchanged assertions,
scenario data, existing catch, and control-flow inspection, resolves the
prior provenance discrepancy; no runtime or production file is in scope.

Fresh `m5d7m-r61-d4-stage-markers.xml/.log` stems remain absent and no Unity
process is present. The exact R60 filter plus only
`HubUiOnlyQ0HandoffMatrixPlayModeTests`, the 1900-second global watchdog, and
the separate 180-second exact-D4 stale-marker/increasing-CPU watchdog remain
the approved procedure; no `-quit`, retry, or version probe is allowed. The
four CUA source/test hashes and the 511-entry pre-amendment drift baseline
remain unchanged (the current count includes this verification record).

**Second-audit verdict:** **AC-M5D7M-R61-001 PASS (pre-execution)**; the
initial FAIL remains preserved as history. **P0: 0; P1: 0; P2: 0.** This
clears the implementation/provenance pre-gate only. It does not authorize
execution by itself, satisfy `AC-M5D7M-R61-002..004`, or close
`AC-CUA-009`.
