# VD-01 Unity Movement Adapter M2 Evidence

- Date: 2026-08-25
- Implementer: Terra, with screened Kimi K3 draft input
- Integration and evidence owner: Sol
- Independent verification: Luna PASS, 2026-08-25
- Work contract: `docs/specs/work-contracts/2026-08-25-vd01-movement-sandbox.md`
- Decision: `ADR-0024`
- Requirements: `REQ-MOV-001~004`, `REQ-MOV-006~010`
- Acceptance criteria: M2 evidence for `AC-MOV-001`, `AC-MOV-002`, `AC-MOV-004`, `AC-MOV-005`, and `AC-MOV-006`; `AC-MOV-003` remains deferred to VD-04

## Integrated unit

- Public `PlayerMovementController` kinematic Unity adapter with no public gameplay members and no device input reads.
- Pure motor `BeginTick`→`CommitTick` internal transaction; the existing M1 `Advance` is a no-collision compatibility wrapper.
- Q4096 axis-separated X→Y collider casts, same-tick collision commit, body/snapshot equality, stable scene plus authored hierarchy wall IDs, tick-indexed command buffering, neutral fallback, and malformed command rejection.
- Motor-owned gravity authority under ADR-0024. Unity keeps compatibility `Physics2D.gravity=(-9.81)` and kinematic body `gravityScale=3.1315`, with Lightweight `×0.65`, without solver gravity duplication.
- Authored `MovementSandbox.unity` and `MovementSandboxPlayer.prefab` plus deterministic authoring validation.
- Unity built-in `com.unity.modules.physics2d=1.0.0` activated after the first M2 compile proved the minimal bootstrap omitted the module.

## Final evidence

### EV-2026-08-25-AC-MOV-M2-EDITMODE

- Unity `6000.3.21f1` batch EditMode movement suite.
- Result: PASS, 22/22, failed 0, exit code 0.
- Coverage: all 20 M1 deterministic-core regressions plus generated scene/prefab validation and duplicate/empty stable-ID rejection.
- Result XML SHA-256: `94430DAF3C3C081974520594879DA12255E2029531A4B0CFAF85B6DCA87EDD27`.

### EV-2026-08-25-AC-MOV-M2-PLAYMODE

- Unity `6000.3.21f1` batch PlayMode movement suite.
- Result: PASS, 11/11, failed 0, exit code 0.
- Coverage: real ground landing, ceiling block, independent left/right wall block, no overlap, exact snapshot/body Q equality, blocked dash position/velocity/15-tick schedule/cooldown boundary, stable ID, neutral/late/duplicate command behavior, controller/cast/MovePosition/Physics2D simulation replay equality across 30/60/144 render grouping, and Baseline→Lightweight→Baseline compatibility gravity-scale reflection.
- Result XML SHA-256: `A45BB538BA44C95A793C63C2B8774D9B0F7617663B97B318104AABDDBF1A7423`.

### EV-2026-08-25-AC-MOV-M2-STATIC-AND-AUTHORING

- `git diff --check`: PASS.
- Pure movement assembly search for Unity engine/editor/physics types outside the dedicated `Unity` adapter folder: no leak.
- No new public runtime type beyond the frozen `PlayerMovementController`; adapter input and inspection seams remain internal.
- Key SHA-256:
  - `PlayerMovementMotor.cs`: `3699AFDEFAE0813E06843EA037FC3D3D511C504008B593D624B3424503AA2900`
  - `MovementCollisionResolution.cs`: `03164D9B903CE6386CC57C62F740A34B2F5714392C922B54D98E8D62E76648B5`
  - `PlayerMovementController.cs`: `A678748F96086D5BC0746C2ACB0F5FA82607C97A3AD062D926882DB55E1BE045`
  - `MovementSandboxAuthoringBuilder.cs`: `55B0E3ADEA8B5FEBC1AEF84848661D52F0846AE20D45323AEF61E53234666E10`
  - `MovementSandboxAuthoringTests.cs`: `D1BB9CF06E8ADAFF8A921F1D2899F380C4111828298F81B59BE33734EF6A5347`
  - `PlayerMovementControllerPlayModeTests.cs`: `F2102F05988C3C381A58ADF1E68E29AEE7E6ABADE441AAC6198F501369603D94`
  - `MovementSandbox.unity`: `9087D4A74137D0770A08E9E057AE2E0D4A77B88A40EFABFA6CB9F4BD188197C9`
  - `MovementSandboxPlayer.prefab`: `B31D45569AFBC852AF6A92EBA33885CD4182776B98DDA345940F9D60862A2C68`
  - `Packages/manifest.json`: `7EE76CE89F845F718D861C1015E07C02F4C9A469E5ECCA71E396E5363D97DD25`
  - `Packages/packages-lock.json`: `500E2A58434F271E97AC9CF0F0AD896FED1653CFEA095C64BE99427145F4400D`

### EV-2026-08-25-AC-PLAT-PHYSICS2D-REGRESSION

- Existing VD-09 bootstrap EditMode suite rerun after activating the built-in Physics2D module.
- Result: PASS, 1/1, failed 0, exit code 0.
- Result XML SHA-256: `918C4B68D596A5E345C9DAE9AAB91E1DAAB2B9E824994CD09ADDFAEE6D520295`.

## Corrected issues and screened drafts

1. Kimi's architecture draft intentionally allowed motor/body divergence, Unity instance IDs, and multi-advance substeps. Sol rejected those items and retained only the kinematic-cast direction and test decomposition.
2. Kimi's code skeleton invented incompatible types and placed Unity queries inside the pure motor. Terra did not copy it; Sol recorded the rejection in the implementation proposal review.
3. Luna's pre-gate review exposed the missing two-phase transaction and a contradiction over gravity authority. Sol froze `BeginTick`→`CommitTick` and accepted ADR-0024 before implementation evidence was claimed.
4. The first compile exposed the missing Unity built-in Physics2D module and two Unity 6.3 API/type errors. Sol activated only the built-in module; Terra corrected the bounded code errors.
5. The first 6/6 PlayMode suite passed but did not prove the required collision behavior. Luna and Sol rejected it as insufficient and expanded the suite.
6. The expanded suite initially passed 7/10. Measured ceiling and wall `Collider2D.Distance` overlap was about 0.0048u at the pre-implementation 41-Q skin. Sol raised the skin to 82 Q4096 (about 0.0200u), amended the contract with the empirical reason, and the next suite passed 10/10.
7. Luna's final P2 suggestion added direct adapter gravity-scale transition/restoration evidence; the final suite passed 11/11.

## Deferred scope and Sol decision

Sol accepts M2. Luna independently reports M2 PASS with no P0/P1 blocker. M1 and M2 are Verified milestones, but VD-01 remains Approved rather than fully Verified because `AC-MOV-003` requires the six VD-04 authored traversal rooms.
