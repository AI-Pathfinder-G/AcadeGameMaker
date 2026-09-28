# VD-09 Unity Bootstrap Implementation Evidence

- Date: 2026-08-25
- Implementer: Terra
- Integration and evidence owner: Sol
- Independent verification: Luna PASS, 2026-08-25
- Work contract: `docs/specs/work-contracts/2026-08-25-vd09-unity-bootstrap.md`
- Requirements: `REQ-PLAT-001`, `REQ-PLAT-002`, `REQ-PLAT-005`, `REQ-PLAT-007`
- Acceptance criteria: partial bootstrap evidence for `AC-PLAT-001` and `AC-PLAT-005`; no full vertical-demo completion claim

## Integrated baseline

- Unity `6000.3.21f1`, URP `17.3.0`, Input System `1.20.0`, Test Framework `1.6.0`.
- Repository root is the Unity project root.
- Windows x64 Mono development target, New Input System only, no legacy action asset.
- Default output 2560×1440, resizable window, fixed timestep `0.016666668`, Physics2D gravity `(0,-9.81)`.
- `Assets/Scenes/Bootstrap.unity` is the only enabled build scene.
- No gameplay-facing runtime C# type was introduced.

## Evidence

### EV-2026-08-25-AC-PLAT-001-BOOTSTRAP-EDITMODE

- Command: Unity batch EditMode test on the repository root.
- Result: PASS, 1/1 (`ApprovedPlatformBaselineIsConfigured`).
- Result file SHA-256: `3D606035182FCAD08912951F6EA6B2F8374206A8CD3BDB8D597B60F733523E4F`.
- Verified engine, direct package pins, URP assets, product and application identifiers, output baseline, resizable window, Mono serialization, New Input System, 60 Hz fixed step, Physics2D gravity, build scene, and absence of forbidden default input-action assets.

### EV-2026-08-25-AC-PLAT-001-WINDOWS-BUILD

- Command: `BootstrapBuild.BuildWindows64Development` using `BuildTarget.StandaloneWindows64`, Mono2x, `Development | StrictMode`.
- Result: `Build Finished, Result: Success`, Unity batch exit code 0.
- Executable SHA-256: `C8B0D73DC40E4F2CDDBF656CFB7257FCB8273DA22E44E12A8694CD8E275C6FB2`.
- Build log SHA-256: `77E389E82B87F5362E7F57F509F32D1491867B9BFF682555FFA0EF966DE6C934`.

### EV-2026-08-25-AC-PLAT-005-PLAYER-SMOKE

- The generated Windows player started hidden with `-batchmode -nographics`, remained alive after five seconds, and was then stopped by the harness.
- Player log reported Unity `6000.3.21f1`, project `AcadeGameMaker`, initialized Input System and Mono, and contained no exception, missing-reference, null-reference, or error entry.
- Player log SHA-256: `22734C9295ABC4A8D8C450E8D43696E72A7B5608A8F1ECF6C9BE3D1C2BEDB99F`.

## Corrected bootstrap issues

1. The initial test assembly declared Test Runner references twice. Terra removed the explicit references and retained the standard `TestAssemblies` optional reference; the next real test run passed.
2. Unity 6.3 rejected legacy `-build` use without an active Build Profile. Terra supplied a bounded `BuildPipeline.BuildPlayer` entrypoint rather than adding a profile or changing runtime contracts.

## Deferred criterion portions

- The 640×360 minimum desktop window behavior requires a later presentation/runtime consumer because Player Settings does not expose a Windows minimum-window-size field.
- Full editor/build gameplay equivalence under `AC-PLAT-005` remains open until gameplay exists; this evidence proves only the bootstrap scene and startup baseline.

## Sol integration decision

Sol accepts the VD-09 bootstrap unit. The approved VD-01 movement sandbox may now begin. VD-09 itself remains Approved because the later runtime portions of `AC-PLAT-001` and `AC-PLAT-005` are not yet complete.
