# Work Contract: VD-01 Deterministic Movement Sandbox

- Status: Approved
- Milestone status: M1 pure movement core and M2 Unity adapter/PlayMode Verified by Sol and independently PASS-reviewed by Luna, 2026-08-25; full VD-01 remains open only for deferred `AC-MOV-003` after VD-04 rooms exist
- Owning spec and revision: `VD-01`, Approved 2026-08-25
- Assigned by: Sol
- Implementer: Terra; Kimi K3 actively drafts the pure movement motor, adapter, and bounded tests inside the frozen interface, and Terra screens, corrects, and integrates them
- Independent verifier: Luna
- Requirement IDs: `REQ-MOV-001~004`, `REQ-MOV-006~010`
- Acceptance-criterion IDs: `AC-MOV-001`, `AC-MOV-002`, `AC-MOV-004`, `AC-MOV-005`, `AC-MOV-006`; `AC-MOV-003` remains deferred until VD-04's six authored traversal rooms exist
- Allowed files/directories: `Assets/AcadeGameMaker/Runtime/Core/**`, `Assets/AcadeGameMaker/Runtime/Movement/**`, `Assets/AcadeGameMaker/Editor/Movement/**`, `Assets/AcadeGameMaker/Tests/EditMode/Movement/**`, `Assets/AcadeGameMaker/Tests/PlayMode/Movement/**`, `Assets/Scenes/MovementSandbox.unity`, `Assets/Prefabs/Player/MovementSandboxPlayer.prefab`, their `.meta` and assembly-definition files, and `ProjectSettings/Physics2DSettings.asset` only for the already approved gravity value
- Forbidden files/directories: `GameInput.inputactions`; `InputRouter`; all weight-transfer target selection, combat, enemies, room assembly, persistence, narrative, UI, camera, final art, external assets, non-built-in package changes, and unrelated project settings. Sol approved the required Unity built-in `com.unity.modules.physics2d=1.0.0` activation on 2026-08-25 after the first M2 compile proved the bootstrap omitted it.
- Public interface or data contract: the types and semantics in the frozen interface section below; no new public type, member, enum value, event, serialized schema, or namespace may be added without Sol approval
- Cross-part invariants: one authoritative integer `SimulationTick` at 60 Hz; commands are consumed once for their stated tick; Q4096 semantic movement values; stable authored wall IDs rather than Unity instance IDs; Unity object references never enter public payloads; movement solely owns motion state; Lightweight modifies only approved movement parameters and never changes dash or wall-jump values
- Rollback point: annotated Git tag `pre-unity-approved-v1`; the integrated VD-09 bootstrap commit is the unit's direct parent
- Required implementation evidence: EditMode state-machine tests and PlayMode Rigidbody2D/collision tests; explicit 6/7-tick coyote and jump-buffer boundaries, 35/36-tick dash-cooldown boundary, 5/6-tick same-wall lock boundary; baseline/Lightweight enter-exit restoration; three identical 60 Hz input replays with tick-by-tick snapshot equality; measured evidence for every claimed acceptance criterion
- Required independent verification evidence: Luna reviews Kimi-originated and Terra-integrated code, reruns EditMode and PlayMode suites, repeats the three-replay comparison, and records evidence IDs against `AC-MOV-001`, `AC-MOV-002`, `AC-MOV-004`, `AC-MOV-005`, and `AC-MOV-006`
- Integration order and dependencies: begins only after VD-09 bootstrap evidence is accepted by Sol; VD-02 and VD-07 may consume the published immutable payloads later but may not own or mutate movement state
- Sol approval/date: Approved by Sol, 2026-08-25

## Frozen interface

Namespaces are `AcadeGameMaker.Core` and `AcadeGameMaker.Movement`.

- `SimulationTick`: immutable signed 32-bit integer tick wrapper with value semantics and no wall-clock conversion.
- `MovementCommand`: immutable payload containing `Tick`, signed 32-bit `MoveXQ4096`, signed 32-bit `MoveYQ4096`, `JumpPressed`, `JumpReleased`, and `DashPressed`. Semantic axes are clamped to `[-4096,4096]`; this unit receives commands and does not read devices.
- `WallSide`: `None=0`, `Left=-1`, or `Right=1`, describing the wall relative to the player.
- `MovementContacts`: immutable payload containing `IsGrounded`, stable authored `WallId`, `WallSide`, and `IsDashPathBlocked`. `WallId` is empty exactly when `WallSide=None`.
- `PlayerMovementModifierKind`: `Baseline` or `Lightweight`. VD-01 owns the exact approved multipliers.
- `MovementActionState`: `Normal`, `Dashing`, `WallSliding`, `WallJumping`, or `InputLocked`.
- `PlayerMotionSnapshot`: immutable `Tick`, signed 32-bit `PositionXQ4096`, `PositionYQ4096`, `VelocityXQ4096`, `VelocityYQ4096`, grounded state, `WallId`, `WallSide`, action state, and air-dash availability. Position uses 1/4096 world-unit increments and velocity uses 1/4096 u/s increments. Calculations use signed 64-bit intermediates with checked conversion; internal counters, rational remainders, and Unity references are not public state.
- `PlayerMovementMotor`: internal deterministic state machine that advances exactly one `SimulationTick` from a command, contacts, and modifier.
- `PlayerMovementController`: the Unity `Rigidbody2D` and collision adapter. It may translate scene contacts into `MovementContacts` but may not read input devices or own a second movement state.

## Lifecycle and action rules

- Q4096 constants use nearest-integer rounding away from zero at an exact half. Acceleration and position integration carry signed rational remainders across the `/60` step so an unobstructed 20 u/s dash is exactly 5.0u after 15 ticks.
- A command tick must equal the motor's next expected tick. Duplicate, skipped, or reversed ticks fail fast in tests and never advance state.
- Room entry, respawn, teleport, input lock, and run end clear buffered edge commands and stale wall attachment. Input lock cancels an active dash, ignores movement/action commands, and continues gravity/collision physics; unlock resumes from current motion. Reset semantics must be covered by tests.
- Coyote and jump-buffer age are measured in complete ticks after the source event. Age 6 is accepted and age 7 is rejected. Jump release can reduce upward velocity only once for the current jump.
- Dash is horizontal. On `DashPressed`, it snapshots the sign of non-deadzone horizontal movement or otherwise the last nonzero horizontal facing. Dash velocity is 20 u/s for 15 consecutive active ticks, producing 5.0u in an unobstructed path. Vertical input and later direction changes are ignored while dashing. Obstruction stops displacement but does not shorten the scheduled 15-tick action. The 36-tick cooldown begins after the scheduled final active tick; cooldown age 35 rejects and age 36 accepts a new dash.
- `MovementContacts.IsGrounded` is already a validated stable-ground signal from the adapter. The first tick on which it is true restores the single air-dash charge; transient unvalidated contacts must never be encoded as grounded.
- Falling while airborne with a valid wall contact enters wall slide without requiring directional input. A wall jump sets `WallJumping` for its impulse tick and returns to the derived state on the next tick. After a wall jump, the same authored `WallId` cannot be adopted for wall slide or another wall jump at lock ages 1 through 5 and becomes eligible at age 6.
- Lightweight applies `gravityScale ×0.65`, air acceleration `×1.25`, and maximum fall speed `×0.70`, and restores the exact baseline on exit.

## M2 frozen Unity adapter rules

- `PlayerMovementController` is the sole new public MonoBehaviour type and adds no public gameplay member beyond its inherited Unity surface. Serialized references and tuning fields are private; M2 command injection and inspection helpers are internal test seams only.
- The adapter uses a kinematic `Rigidbody2D`, a non-trigger `Collider2D`, fixed 60 Hz execution, and cast/overlap queries. It never reads input devices, integrates a second position or velocity, uses forces, or delegates authoritative displacement to the dynamic solver.
- The motor's Q4096 gravity is the player's actual acceleration authority. The project keeps `Physics2D.gravity=(0,-9.81)` and the kinematic player mirrors compatibility `gravityScale=3.1315` in Baseline and `3.1315×0.65` in Lightweight, but solver gravity is never added to player motion.
- Each simulation tick is a two-phase transaction: the motor predicts exactly one tick, then the adapter supplies one internal, quantized collision resolution before any snapshot is published or `Rigidbody2D.MovePosition` is issued. The motor commits the resolved Q4096 position and blocked-axis velocity, so its final snapshot and the body target are exactly equal in the same tick.
- The internal transaction is `BeginTick` then `CommitTick` or an exactly equivalent naming. `BeginTick` consumes one command tick and stages an immutable internal prediction but does not change the public `Snapshot`. `CommitTick` may occur exactly once for that staged tick and is the only operation that publishes its snapshot. The existing internal `Advance` remains as the M1-compatible wrapper that performs `BeginTick` plus a no-collision `CommitTick`.
- Beginning another tick while one is staged, committing without a staged tick, committing a different tick, or committing twice fails fast. The prior published snapshot stays unchanged until a valid commit, and no failure advances a second tick.
- The internal collision-resolution seam is not a public contract change. It may correct position and zero a blocked normal velocity only. During a scheduled dash it may cancel that tick's displacement while preserving dash velocity, duration, and cooldown timing.
- The adapter may ask the motor through an internal, non-mutating helper for the dash direction implied by the current command and owned facing. This exists only to build `IsDashPathBlocked` before `BeginTick`; facing is not duplicated in the adapter.
- Collision casts are axis separated and use the attached collider shape with a fixed Q4096 skin. Hits are ordered by nearest quantized distance and then by stable authored hierarchy path; Unity instance IDs, discovery order, render-frame time, and wall-clock time are forbidden as tie-breakers or wall identifiers.
- M2 uses skin `82` Q4096 (about 0.0200u), contact probe `205` Q4096 (about 0.0500u), and ground/wall normal threshold `2867` Q4096 (about 0.70). The skin was raised from the pre-implementation `41` after Unity PlayMode measurement showed about 0.0048u of `Collider2D.Distance` overlap at the smaller margin. A nonnegative Unity hit distance converts with floor toward zero; allowed travel is `max(0, floor(distance×4096)-82)`. Signed travel applies the cast direction after quantization. Equal allowed distance is ordered by ordinal authored path and then ordinal scene name; duplicate full authored IDs are an authoring error.
- A stable wall ID is the authored scene name plus authored transform hierarchy path. M2 authoring validation must reject empty or duplicate candidate paths in the sandbox. Renaming an authored wall intentionally changes its ID.
- Grounded is encoded only from a downward validation cast whose normal and skin-distance thresholds pass in the current fixed tick. Wall side is encoded only from a horizontal validation cast. No render-frame hysteresis or transient solver callback may enter `MovementContacts`.
- Cast origin is the attached collider at the last committed body position. Resolution order is horizontal X then vertical Y; the Y cast starts from the X-resolved position. `useFullKinematicContacts` remains false because callbacks are not authoritative.
- Missing M2 command input produces one neutral command for the exact expected tick. More than one submitted command for the same tick, a mismatched submitted tick, a second collision-resolution commit, or a resolution for any tick other than the just-advanced tick fails fast without advancing another tick.
- The internal M2 command buffer may accept exact tick-indexed future commands and consumes at most one command for each fixed tick. A command older than the next expected tick is late and rejected; a duplicate queued tick is rejected. If no command exists when a fixed tick begins, the neutral command is irrevocably consumed for that tick and any later submission for it is rejected. Render-rate replay tests preload the same tick-indexed sequence and never derive commands from render-frame count.
- Kinematic solver callbacks do not produce movement state or contacts. The adapter is cast-first; callbacks may be diagnostic only. `MovePosition` receives the exact float conversion of the committed Q4096 snapshot. Equality is observed after the physics step associated with that `FixedUpdate` and before another movement tick; the body's quantized position must equal the last published snapshot.
- Required M2 PlayMode evidence includes snapshot/body Q4096 equality every tick, ground landing without penetration, ceiling and left/right wall blocking, dash obstruction with its full 15-tick schedule, stable wall ID, neutral fallback, malformed command rejection, 30/60/144 render-frame replays producing identical 60 Hz snapshots, and scene/prefab authoring validation.

## Stop conditions

Terra must stop and report to Sol if the Rigidbody2D adapter cannot preserve deterministic tick semantics, a requested behavior needs a public-contract change, package/project-setting changes become necessary, or any claimed tolerance conflicts with VD-01.
