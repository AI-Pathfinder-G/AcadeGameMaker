# Work Contract: VD-03 M3C1 Deterministic Regular-Enemy Locomotion Core

- Status: Approved — Sol, after Luna PASS, 2026-08-30
- Owning spec: `VD-03`, Approved 2026-08-25
- Contract owner, numeric decisions and final integration: Sol
- Unit design and implementation after approval: Terra
- Independent contract and implementation verification: Luna
- Requirement IDs: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`
- Rollback point: commit `9b82b69`

## Purpose and decomposition

M3C1 is an engine-free, deterministic 60 Hz pair motor for the literal `walker` and `surveyor`. It converts a Combat-internal immutable input containing the exact M3B2A pair snapshot and two exact M3A reaction snapshots for tick `t` into Q4096 motion predictions, accepts an externally resolved pair collision result, and commits both enemy motion snapshots or neither. A later M3C2 adapter validates and unwraps Unity carried views into that input; M3C1 never references a `Combat.Unity` type.

M3C1 does not query Unity physics, move a body, create contact damage or a denial line, create `DamageRequest`, author a scene, or change project settings. A later M3C2 contract binds this core to kinematic bodies at execution order `-160`. Contact damage and surveyor line creation/hit/lifetime are separate M3D1/M3D2 contracts after motion is verified.

## Sol numeric and semantic decisions

All rates are Q4096 world units per second and all simulation runs at exactly 60 ticks per second.

| Rule | Baseline |
|---|---:|
| walker `Horizontal` approach speed | `3.0 u/s` = `12288 Q4096/s` |
| walker `DashHorizontal` speed | `12.0 u/s` = `49152 Q4096/s` |
| surveyor `Horizontal` relocate speed | `2.5 u/s` = `10240 Q4096/s` |
| enemy downward acceleration | `20.0 u/s²` = `81920 Q4096/s²` |
| enemy maximum fall speed | `12.0 u/s` = `49152 Q4096/s` |
| Heavy move-speed multiplier | `750/1000` |
| Heavy gravity multiplier | `2200/1000` |
| Heavy airborne horizontal-control multiplier | `350/1000` |

Position and velocity fields are signed 32-bit Q4096 values. Integration remainders are signed 64-bit values. Each applicable Q1000 multiplier is applied in the fixed order below with `RoundDivAwayFromZero(checked((long)value * multiplier), 1000)`; multiplier division never carries a remainder. Walker horizontal rate uses base speed → Heavy move `750` →, only when the prior committed walker snapshot is airborne, Heavy air-control `350`. Downward acceleration uses base gravity → Heavy gravity `2200`. Exact derived rates are walker Heavy grounded approach `9216`, airborne approach `3226`, grounded dash `36864`, airborne dash `12902`, and Heavy gravity `180224` Q4096 units.

Only acceleration and position integration carry canonical signed `/60` remainders whose magnitude is less than `60` and whose sign matches the numerator or is zero. The step order is desired horizontal velocity → vertical velocity update → fall-speed clamp → position integration using the updated velocities. Reaching the fall-speed clamp clears the gravity remainder but preserves the Y-position remainder. Multiplication and addition use checked signed 64-bit intermediates before a checked signed 32-bit result. No floating point participates in the core.

The walker receives downward acceleration while alive and not grounded. Grounded downward velocity and its vertical remainder are zero. Heavy air control applies to the walker's horizontal speed only while the prior committed snapshot is airborne. Dash remains horizontally controllable only through its already latched direction; Heavy still applies the `750` speed multiplier and, while airborne, the `350` control multiplier.

The surveyor is suspension-stable when its behavior movement is `None` or `Horizontal`: vertical velocity and vertical remainder are zero, so it neither falls nor rises. `Downward` applies Heavy gravity until a floor resolution grounds it. After Heavy/fire lock ends, the surveyor does not invent an upward intent or return to its spawn height; it resumes horizontal relocation at its current height. This preserves the spatial advantage earned by transfer without changing M3B2A.

`None`, zero-direction Approach and `Downward` zero horizontal velocity and the X-position remainder. Death uses an identity prediction/resolution, freezes the current position and prior grounded value, and clears both velocities and all integration remainders. Encounter reset bypasses gameplay collision resolution: its separate preflight/commit path restores the immutable construction-time spawn position, clears velocities/remainders, then restores the construction-time walker-grounded and surveyor-grounded seeds. Reset never rewinds the global tick.

## Ownership and exact inputs

- M3B2A/M3B2B retain behavior phase, movement intent, direction, telegraph and action ordinal authority.
- M3A/M3B1 retain Heavy state, reaction windows and transfer-revision authority.
- M3C1 alone owns regular-enemy Q4096 position, velocity, integration remainder, grounded state, spawn seed and motion `NextExpectedTick`.
- M1 retains health, death, damage ordering and dedupe authority.
- VD-02 retains transfer state and the numeric Heavy modifier profile.

`RegularEnemyLocomotionPairInput` is an internal immutable discriminated Combat value. Gameplay carries the exact `RegularEnemyBehaviorPairSnapshot` plus literal walker and surveyor `RegularEnemyReactionSnapshot` values. Encounter reset carries the exact upstream reset snapshots and no gameplay collision payload. Every gameplay step consumes:

- exact behavior pair snapshot for `t` with walker then surveyor and `NextExpectedTick=t+1`;
- exact literal walker and surveyor reaction snapshots for `t`, each with `NextExpectedTick=t+1`;
- final alive values that agree between behavior and reaction;
- one later pair collision resolution bearing the exact prediction identity and tick.

The core validates an initial tick in `0..int.MaxValue-1`, later exactly consecutive ticks, literal IDs, enum ranges, direction domains, behavior/phase compatibility, Heavy structural validity and all reachable arithmetic before mutation. Its constructor accepts immutable Q4096 spawn positions and authored initial grounded seeds, starts velocities/remainders at zero, and exposes no latest snapshot before the first commit. It never reads a Transform, Rigidbody, Collider, frame delta, wall clock or runtime RNG.

## Two-stage pair transaction

`PreviewNext(input)` is mutation-free and returns one immutable pair prediction in walker-then-surveyor order. Each prediction contains tick, start position/velocity, desired velocity, desired position, carried position/gravity remainders, prior grounded state and a stable role ID. Both roles read only the prior committed pair; no staged or resolved result of one role becomes input to the other. Both roles are fully staged before the pair prediction is returned.

The later adapter returns one immutable pair collision resolution. For each role it must echo the exact tick, role, start position and predicted position; publish resolved Q4096 position; mark X/Y blocked, stationary-support-confirmed and grounded; and remain within the axis sweep from start to predicted position. Prediction identity is the complete equality of tick + role + start position + predicted position, never a generated nonce. `Blocked=false` requires resolved=predicted on that axis; when resolved travel magnitude from start is strictly less than predicted travel magnitude, `Blocked=true` is required. A resolution may not move opposite the prediction, exceed it, change an unmoved axis or use a default/duplicate role.

For an alive role, `Grounded == (downwardYBlocked || StationarySupportConfirmed)`. `StationarySupportConfirmed=true` requires prior grounded, stationary Y prediction and `YBlocked=false`; it is forbidden otherwise. A dead role is the sole exception: its identity resolution must echo start=predicted=resolved, set both blocked flags and stationary-support-confirmed false, and copy `Grounded=priorGrounded` without a support query. X block clears X velocity and X-position remainder. Y block clears Y velocity, Y-position remainder and gravity remainder. The pure core validates only this integer envelope, identity and pair atomicity; the geometry and truth of casts/support probes belong exclusively to M3C2.

`ValidateResolvedNext(input, resolution)` recomputes the exact preview and returns a complete immutable `ResolvedPairCandidate` without storing pending state. `CommitResolvedNext(input, resolution, candidate)` recomputes the preview and candidate, requires field-for-field equality, then replaces both role states and the carried pair snapshot in one owner-local commit. Any input, overflow or resolution failure preserves both roles, remainders, grounded flags, previous carried view and expected tick.

An unblocked axis retains the staged desired velocity and canonical remainder. The committed pair snapshot has exact `Tick=t`, walker then surveyor, and `NextExpectedTick=t+1`.

## Intent mapping

### Walker

| Phase + intent | Required state | Motion |
|---|---|---|
| `Approach + Horizontal` | alive; direction `-1|0|1` | direction × approach speed × applicable Heavy factors |
| `DashTelegraph + None` | alive; direction `0`; latched direction `-1|0|1` | horizontal zero |
| `DashActive + DashHorizontal` | alive; direction `-1|0|1` equal to latched direction | direction × dash speed × applicable Heavy factors; zero direction clears X-position remainder |
| `Recovery + None` | alive; direction `0` | horizontal zero |
| `Dead + None` | dead; direction `0` | identity position, zero velocities/remainders |
| `Reset + None` | reset discriminator; direction `0` | separate reset transaction |

Every other walker phase/intent/direction/alive combination is rejected. Alive airborne gameplay steps apply downward acceleration and clamp at maximum fall speed. A grounded stationary step may remain grounded only through the resolution rule above; otherwise it becomes airborne.

### Surveyor

| Phase + intent | Required reaction | Motion |
|---|---|---|
| `Relocate + Horizontal` | alive baseline, `ForcedDescent=false`, `CanFire=true`; direction `-1|1` | direction × relocate speed; vertical suspension-stable |
| `Suppressed + Downward` | alive Heavy, `ForcedDescent=true`, `CanFire=false`; direction `0` | horizontal zero; Heavy gravity to fall cap |
| `Suppressed + None` | alive baseline, `ForcedDescent=false`, residual lock `CanFire=false`; direction `0` | fully stationary/suspension-stable |
| `FireTelegraph|Fire|Cooldown + None` | alive baseline, `ForcedDescent=false`, `CanFire=true`; direction `0` | fully stationary/suspension-stable |
| `Dead + None` | dead; direction `0` | identity position, zero velocities/remainders |
| `Reset + None` | reset discriminator; direction `0` | separate reset transaction |

Every other surveyor combination is rejected, including Relocate while Heavy. Suspension-stable states clear vertical velocity, Y-position remainder and gravity remainder. No phase creates upward velocity.

Any mismatch between phase, intent, direction, alive state or reaction directive is rejected before prediction.

## Tick, overflow, reset and replay rules

- Construction accepts immutable Q4096 spawn positions and initial ground seeds at one exact first tick in `0..int.MaxValue-1`.
- First gameplay tick may be any tick in `0..int.MaxValue-1`. Every later step is exact and consecutive.
- Reset is valid only when both exact upstream publications are reset publications for the expected tick and after at least one committed gameplay step. `PreflightReset(input)` returns an immutable reset candidate; `CommitReset(input, candidate)` recomputes and compares it before atomically restoring both spawn positions and seeds. Reset accepts no collision resolution.
- A first dead transition commits an identity position and preserves the prior grounded value while clearing velocities/remainders. Repeated dead ticks are identical inert transitions. Any alive gameplay input after a dead latch is rejected until reset.
- `checked(t+1)`, rate multiplication, remainder carry, velocity clamp and position addition must all preflight. Unreachable axes do not perform unrelated overflow-prone arithmetic.
- A valid upstream publication can exist only through `t=int.MaxValue-1`; committing it may leave terminal `NextExpectedTick=int.MaxValue`, after which every gameplay/reset input is rejected without mutation because no valid upstream `t+1` publication can be formed.
- Replaying identical exact-tick inputs and collision resolutions under 30/60/144 render grouping yields byte-for-byte identical predictions and committed snapshots.

## Allowed files

- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyLocomotionSession.cs` and required `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/RegularEnemyLocomotionSessionTests.cs` and required `.meta`
- narrowly required internal friend declaration only if existing test access is insufficient
- this contract, its pre-gate and later implementation evidence

## Forbidden scope

- Unity types, physics query, collision geometry, `MonoBehaviour`, body movement or execution-order changes
- mutation of M1, M3A, M3B1, M3B2A, M3B2B or VD-02 behavior
- contact damage, projectile/denial-line lifecycle, `DamageRequest`, knockback, animation, VFX, UI or audio
- public ABI, public serialized field, dynamic discovery, mutable caller collection, float simulation, time source or RNG
- scene, prefab, package, project setting, asset, boss, reward, room completion, run or persistence changes

## Required evidence

- exact first and consecutive tick behavior, stale/future/default publication rejection, valid `t=int.MaxValue-1` terminal commit and malformed/default `t=int.MaxValue` no-mutation rejection;
- exact reachable displacement and retained `/60` remainder: 60-tick Approach, 12-tick DashActive and 48-tick Relocate compared with rate × ticks / 60;
- Heavy `750/2200/350` boundaries, apply/clear without stale remainder contamination, grounded/airborne distinction and fall-speed clamp;
- every row of the complete walker/surveyor phase-intent-reaction tables, including residual-lock `Suppressed+None`, forbidden Relocate+Heavy and invalid direction/alive mutations;
- pair preview and resolved validation equality; malformed walker or surveyor resolution preserves both roles and prior carried view;
- blocked/resolved equivalence, truncated-axis flag requirement, stationary support revalidation, downward block/grounded equivalence, and axis-specific velocity/remainder clearing;
- death identity freeze with grounded preservation, repeated dead inert, forbidden resurrection and collision-free exact reset spawn/seed cleanup;
- checked spawn/rate/remainder/position boundaries and inactive-axis avoidance;
- exact pair prediction/snapshot traces across 30/60/144 render grouping;
- source scan proving no Unity, public surface, floating point, time/RNG, damage or discovery dependency;
- full Combat and full project EditMode regression pass.

## Ollama utilization record for contract formation

| Lane | Outcome | Sol screening |
|---|---|---|
| Kimi K3 | used and accepted in part | Accepted a two-stage preview/validation/commit shape, checked fixed-point carries and atomic pair boundary tests. Rejected physics cast inside the engine-free step, invented public shapes, and failure-time zero publication because failures must preserve prior state. |
| GLM 5.2 | used and accepted in part | Accepted retained-remainder drift, half-committed pair, cast-before-move, multiplier and render-group mutation classes. Rejected inter-enemy current-state dependency and its Q4096 bit-shift multiplier assumption; the contract uses explicit Q1000/60 denominators and both roles read the prior committed pair. |
| MiniMax M3 | failed and replaced | The bounded non-thinking validation-fixture request returned an empty response without quota/rate error. Sol/Terra supplied the repeatable fixture matrix in Required evidence; no retry loop or model output was adopted. |

No cloud model received repository text, paths, credentials, personal data or secrets. Ollama output has no approval or integration authority.

## Stop conditions and approval gate

Stop and report to Sol if implementation needs Unity, a public surface, physics/collision policy, actual body movement, damage/line creation, upstream mutation, scene/prefab work or project settings. Stop if pair-atomic validation cannot be proved without rollback.

Implementation may begin only after Luna independently reports no P0/P1 contradiction and Sol changes this contract to `Approved`. M3C1 remains partial substrate and cannot alone close `AC-COM-001`, `AC-COM-003`, `AC-WT-002` or `AC-WT-005`.
