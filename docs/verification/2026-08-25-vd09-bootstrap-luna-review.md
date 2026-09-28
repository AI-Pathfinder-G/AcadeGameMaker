# VD-09 Unity Bootstrap — Luna Independent Review

- Date: 2026-08-25
- Reviewer: Luna
- Work contract: `docs/specs/work-contracts/2026-08-25-vd09-unity-bootstrap.md`
- Verdict: PASS
- Scope note: bootstrap unit only; not a claim that the full `AC-PLAT-001` or `AC-PLAT-005` is complete

## Static contract review

- `REQ-PLAT-001`, `REQ-PLAT-002`, `REQ-PLAT-005`, `REQ-PLAT-007`: PASS.
- Unity `6000.3.21f1`, URP 2D, Windows x64 Mono, Input System exactly `1.20.0`, and New Input System only are configured.
- 2560×1440 default output, resizable window, 60 Hz fixed timestep, and Physics2D gravity `(0,-9.81)` are present.
- Bootstrap is the sole build scene.
- No gameplay runtime code, `InputRouter`, `GameInput.inputactions`, legacy input dependency, or gameplay-facing public runtime API was found.
- `BootstrapBuild` is confined to an Editor-only test assembly.

## Acceptance evidence review

- `AC-PLAT-001` bootstrap evidence: PASS. Sol's EditMode XML reports 1/1 passed, and the Windows x64 Mono development build reports success with batch exit code 0.
- `AC-PLAT-005` bootstrap configuration evidence: PASS. The manifest and lock resolve Input System `1.20.0`, `activeInputHandler: 1` is present, and no legacy input asset or API dependency was found.
- The 640×360 minimum-window behavior and real input-device support remain later runtime/presentation work and are not claimed complete here.

## Independent execution

- Luna independently started `Builds/Windows/AcadeGameMaker.exe` hidden with `-batchmode -nographics`.
- After three seconds the player had initialized Mono, Input System, and PhysX without an exception, missing reference, or error. The harness then stopped it intentionally.
- Luna's independent EditMode rerun was inconclusive because Unity Licensing Client initialization timed out after 60 seconds before tests began. This is an environment-only pre-test failure, not a product or test failure, and it does not contradict the preserved 1/1 PASS result.

## Recommendation

Sol may accept the VD-09 bootstrap and release the dependent VD-01 movement work contract.
