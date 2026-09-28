# Work Contract: VD-09 Unity Bootstrap

- Status: Verified (bootstrap unit only)
- Owning spec and revision: `VD-09`, Approved 2026-08-25
- Assigned by: Sol
- Implementer: Terra; bounded project-layout and verification drafts may be supplied by Kimi K3 and must be screened by Terra
- Independent verifier: Luna
- Requirement IDs: `REQ-PLAT-001`, `REQ-PLAT-002`, `REQ-PLAT-005`, `REQ-PLAT-007`
- Acceptance-criterion IDs: `AC-PLAT-001`, `AC-PLAT-005`; this unit provides bootstrap evidence only and does not claim full runtime completion of either criterion
- Allowed files/directories: `Assets/Scenes/Bootstrap.unity`, `Assets/Settings/**`, `Assets/AcadeGameMaker/Tests/Bootstrap/**`, assembly definitions required by those tests, `Packages/manifest.json`, `Packages/packages-lock.json`, and the minimum necessary files under `ProjectSettings/**`
- Forbidden files/directories: gameplay runtime code; `GameInput.inputactions`; `InputRouter`; movement, weight-transfer, combat, room, run, persistence, narrative, UI, and camera implementation; external asset import; Unity Cloud service binding; any file outside the repository
- Public interface or data contract: none; the bootstrap must not introduce a gameplay-facing public C# type
- Cross-part invariants: Unity `6000.3.21f1`; repository root is the Unity project root; URP 2D; Windows x64; Mono scripting backend for this non-release demo; `com.unity.inputsystem` exactly `1.20.0`; Active Input Handling is Input System Package (New) only; 60 Hz fixed step; 2560×1440 output baseline; 640×360 minimum window; `Physics2D.gravity=(0,-9.81)`; no legacy input dependency
- Rollback point: annotated Git tag `pre-unity-approved-v1`, which must exist before project generation starts
- Required implementation evidence: exact Unity and package versions; project-setting snapshot; package-lock inspection; batch-mode EditMode bootstrap test result; empty Windows x64 development-build smoke result; warning/error log classification with requirement IDs
- Required independent verification evidence: Luna reproduces the package/settings inspection and batch test, checks the empty Windows build starts without an unhandled exception or missing reference, and records evidence against `AC-PLAT-001` and `AC-PLAT-005` without promoting the full criteria prematurely
- Integration order and dependencies: first runtime unit; depends on the approved vertical-demo package and `SYSTEM-CONTRACTS`; movement work may start only after Sol accepts this unit's evidence
- Sol approval/date: Approved by Sol, 2026-08-25
- Sol integration acceptance: Accepted 2026-08-25 after Terra implementation evidence and Luna PASS; this does not promote the full VD-09 spec beyond Approved

## Sol decisions frozen for this unit

1. The repository root, not a nested folder, is the Unity project root.
2. The installed Windows support contains Mono player variations and no IL2CPP variation, so Mono is the approved development backend. Installing IL2CPP is outside this work contract.
3. The baseline scene is intentionally empty except for objects needed to prove URP 2D and project startup. Gameplay objects are prohibited.
4. Generated or locally cached Unity template content must still be reviewed against the allowed-file boundary before integration.

## Stop conditions

Terra must stop and report to Sol if project generation changes a public contract, resolves packages to a different Input System version, requires a cloud connection, writes outside the repository, or requires any forbidden subsystem implementation.
