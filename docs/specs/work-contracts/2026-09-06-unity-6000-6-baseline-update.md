# Work Contract: Unity 6000.6.0f1 baseline update

- Status: Approved by explicit user direction, 2026-09-06
- Purpose: move the active Unity Editor baseline to the installed Hub-managed `6000.6.0f1`
- Supersedes for future execution: the `6000.3.21f1` active-environment assumption in the VD-09 bootstrap work contract
- Historical evidence rule: verification records produced under `6000.3.21f1` or `6000.5.9f1` remain historical and are not relabeled as `6000.6.0f1` evidence
- Requirement IDs: `REQ-PLAT-001`, `REQ-PLAT-002`, `REQ-PLAT-005`, `REQ-PLAT-007`
- Acceptance-criterion IDs: `AC-PLAT-001`, `AC-PLAT-005`
- Allowed changes: `ProjectSettings/ProjectVersion.txt`, the bootstrap baseline assertion and baseline note, current environment/readiness documentation, and new versioned verification evidence
- Forbidden changes: gameplay runtime semantics, public APIs, package version invention, license-file deletion, manual Licensing Client binary replacement, and retroactive edits to historical verification results

## Frozen baseline

- Editor: `6000.6.0f1`
- Editor revision: `f7f8ed4d1e24`
- Editor executable: `C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe`
- Embedded Licensing Client: `1.18.3`, Authenticode `Valid`
- Hub Licensing Client: `1.18.3`, Authenticode `Valid`
- Scripting/backend, URP 2D, Windows x64 Mono, fixed 60 Hz, output, gravity and Input System constraints remain unchanged
- Existing manifest pins (`com.unity.inputsystem 1.20.0`, URP `17.3.0`, Test Framework `1.6.0`) remain unchanged until the new Editor import verifies compatibility

## Required verification

1. `ProjectVersion.txt` contains the exact baseline and revision.
2. The installed Editor and both Licensing Client binaries exist and are Authenticode-valid.
3. Package manifest and lockfile are inspected; no package is upgraded solely because the Editor changed.
4. The bootstrap EditMode test is run through the licensing preflight wrapper, with `AC-PLAT-001` and `AC-PLAT-005` cited.
5. If the run is blocked by IPC or entitlement diagnostics, it is recorded as an environment blocker and not as a code/test failure.

## Ollama utilization record

- Kimi K3: not applicable — this is a project baseline migration, not an isolated runtime implementation experiment.
- GLM 5.2: not applicable — no new gameplay failure-mode analysis is needed; the version/revision and local installation are authoritative.
- MiniMax M3: not applicable — no repeatable fixture or validation tool is introduced by this baseline-only change.
- Luna: required independent verification of the new baseline, package inspection and bootstrap result before promotion of new evidence.
