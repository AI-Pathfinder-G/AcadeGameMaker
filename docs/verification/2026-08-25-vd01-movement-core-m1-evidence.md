# VD-01 Deterministic Movement Core M1 Evidence

- Date: 2026-08-25
- Implementer: Terra, with screened Kimi K3 draft input
- Integration and evidence owner: Sol
- Independent verification: Luna PASS, 2026-08-25
- Work contract: `docs/specs/work-contracts/2026-08-25-vd01-movement-sandbox.md`
- Requirements: `REQ-MOV-001~004`, `REQ-MOV-006~010`
- Acceptance criteria: pure-core evidence toward `AC-MOV-001`, `AC-MOV-002`, `AC-MOV-004`, `AC-MOV-005`, and `AC-MOV-006`; PlayMode adapter evidence remains deferred to M2

## Integrated unit

- Pure C# movement state machine with no Unity engine references.
- One authoritative signed `SimulationTick`, strict sequential command consumption, Q4096 public payloads, and signed 64-bit integration intermediates.
- Baseline movement, jump and release, coyote and jump buffer, horizontal dash, validated ground air-dash restoration, wall slide and wall jump, Lightweight modifier, input lock, lifecycle reset, and stable authored wall identifiers.
- `PlayerMotionSnapshot` always reports the authoritative end-of-tick motor state. The configured jump impulse is exactly `58941` Q4096 (`14.39u/s`); the first semi-implicit baseline tick reports `56844` after gravity, preserving the approved apex calibration.

## Evidence

### EV-2026-08-25-AC-MOV-CORE-EDITMODE

- Command: Unity `6000.3.21f1` batch EditMode test filtered to `AcadeGameMaker.Tests.EditMode.Movement`.
- Result: PASS, 20/20, failed 0, Unity exit code 0.
- Result XML SHA-256: `47EDB9731789153337C5811DD6FF3573555E6B28EC908AB8094B2B64146F550D`.
- Covered boundaries include coyote/buffer ages 6/7, dash duration 15 and cooldown ages 35/36, same-wall lock ages 5/6, radial deadzone 0.20, one-shot jump release, input lock, lifecycle-reset stale-coyote prevention, duplicate/skipped/reversed tick rejection, air-dash restoration, modifier restoration, numeric apex/fall caps, obstruction behavior, and three tick-identical replays.

### EV-2026-08-25-AC-MOV-CORE-STATIC

- `git diff --check`: PASS.
- Runtime source search for `UnityEngine`, `UnityEditor`, and `Rigidbody2D`: no matches.
- Frozen public API audit: no public runtime types beyond `SimulationTick`, `MovementCommand`, `WallSide`, `MovementContacts`, `PlayerMovementModifierKind`, `MovementActionState`, and `PlayerMotionSnapshot`.
- Source SHA-256:
  - `SimulationTick.cs`: `779514FEEB11BA8A3E3272AFA814ABDCAA4290581BED81C23E8CFA8D219DBABB`
  - `FixedQ4096.cs`: `BD06E3C202B2030EBCEE4755FEC48091627C2DEDE69D38BA2DC711D14EC73D75`
  - `MovementContracts.cs`: `191318E4049E0205B2BEF8B49B3F4EB3D8B7430439989064B3D48330A1B1B3C2`
  - `PlayerMovementMotor.cs`: `9029674D8C1376420206F55EA2B9F8A470624AB5CF6DFAC64A5F92203B0C185E`
  - `PlayerMovementMotorTests.cs`: `1472DCE9F0D8C6DB94DF40612BA724BFA812E7EAA82F4597C9E75FD79FF64460`

## Corrected implementation and verification issues

1. The first expanded run exposed two defects: the jump-boundary assertion conflated configured impulse with authoritative end-of-tick velocity, and the input-lock test assumed displacement despite approved neutral ground deceleration. Terra corrected the motor/test semantics.
2. Sol rejected a proposed snapshot-only jump-velocity override because it made the public snapshot disagree with internal state. The override was removed and the numeric apex calibration retained.
3. A fresh post-correction run reached 18/19; one buffer age-6 assertion still used the impulse constant. Sol corrected that residual expectation to the authoritative first end-of-tick value. The next fresh run passed 19/19.
4. Unity LicensingClient IPC delayed two attempts. Sol restarted the signed installed licensing client on its expected named pipe; the subsequent real Unity runs completed normally. No acceptance claim is based on the delayed attempts.
5. Luna's first independent review found that lifecycle reset cleared `_coyoteAge` but not `_wasGrounded`, allowing stale coyote regeneration on the next airborne tick. Terra cleared the stale ground bit and added the exact regression, plus explicit skipped/reversed tick coverage. The final fresh run passed 20/20.

## Deferred criterion portions

- `PlayerMovementController`, `Rigidbody2D` collision/contact translation, `MovementSandbox.unity`, the player prefab, and PlayMode collision/replay tests belong to M2.
- `AC-MOV-003` remains deferred until VD-04 supplies all six authored traversal rooms.
- This document does not mark VD-01 Verified; it records the pure-core M1 gate only.

## Sol integration decision

Sol accepts the pure-core M1 implementation evidence. Luna independently reported PASS with no remaining M1 P0/P1 blocker. M2 Unity adapter and PlayMode work may begin; VD-01 itself remains Approved and incomplete.
