# Work Contract: VD-03 M3C2 Regular-Enemy Locomotion Unity Bridge

- Status: Approved
- Owning spec: `VD-03`, Approved 2026-08-25
- Contract owner, cross-system decisions and final integration: Sol
- Unit design and implementation after approval: Terra
- Independent contract and implementation verification: Luna
- Requirement IDs: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`
- Rollback point: commit `38c3b3a`

## Purpose and phase ownership

M3C2 binds verified M3C1 to Unity at execution order `-160`. It consumes exact current-tick M3B2B and M3B1 carried views, derives explicit logical BoxCast collision resolutions for the frozen walker/surveyor pair, commits M3C1 once, schedules both kinematic bodies to the committed Q4096 positions, and publishes one immutable carried motion pair.

The fixed pipeline becomes exactly:

`-200 Transfer → -190 Combat → -180 Reaction → -170 Behavior → -160 Enemy Locomotion → default Player Movement`

M3C2 owns only the Unity binding, collision/support truth, M3C1 session and carried motion view. It does not create contact damage, surveyor denial lines, `DamageRequest`, animation, VFX, audio, scene authoring or project settings. Actor-to-actor threat overlap belongs to later M3D; locomotion collides with authored environment only.

## Explicit sources and frozen authoring

The new driver has private serialized references to player movement, combat, reaction and behavior drivers. It obtains walker/surveyor geometry only through Combat's already frozen ordinal roster; it does not bind a second registry or dynamically discover objects. Narrow internal read-only seams may expose the exact behavior carried view and an alias of the same Combat-owned frozen roster.

Each literal role must retain its verified combat identity and satisfy:

- frozen target collider is one enabled, non-trigger `BoxCollider2D`;
- that box is the only enabled non-trigger collider on or below the role root;
- `attachedRigidbody` exists on the exact frozen pose root, whose GameObject is active in hierarchy;
- the body has `simulated=true`, `bodyType=Kinematic`, constraints exactly `RigidbodyConstraints2D.FreezeRotation` with no position constraint, `interpolation=None`, `useFullKinematicContacts=false`, rotation `0`, angular velocity `0` and linear velocity `(0,0)`;
- body, box and frozen pose root use the same Transform;
- world XY scale is exactly `(1,1)`, Z rotation is `0`, box offset is `(0,0)`, edge radius is `0`;
- box size is finite and positive, quantizes AwayFromZero to positive signed Q4096 on both axes, and the quantized size is used for every cast;
- body position is finite and quantizes AwayFromZero to signed Q4096; solver linear velocity is zero and is never locomotion authority.

No tag, Unity instance ID, creation order, hash iteration, scene search or second mutable caller collection may select a role or collider.

## Tick and initialization transaction

At `-160`, the player's `NextExpectedTick` is the exact global tick `t` because default-order movement has not advanced it. Before creating or advancing an M3C1 session, the driver validates:

- all bindings and frozen authoring above;
- exact behavior carried view `Tick=t`, pair snapshot `Tick=t`, `NextExpectedTick=t+1`;
- exact reaction carried view `Tick=t`, both reaction snapshots `Tick=t`, `NextExpectedTick=t+1`;
- literal walker/surveyor identity and ordinal roster order;
- existing-session expected tick, prior carried motion view and both body Q4096 positions agree field-for-field with the last committed M3C1 snapshot.

On first use, both finite body positions are frozen as M3C1 spawn seeds. Initialization probes are unconditional seed queries, distinct from gameplay stationary-support queries. After the full authoring and upstream preflight, probe walker then surveyor from each exact spawn center with the frozen box size, angle `0`, downward direction and exact distance `205/4096f`, using the same filter, saturation, exclusion, validity and stable-order rules below. A retained hit confirms the initial grounded seed only when `floor(normal.y * 4096) >= 2867`. All seed queries must succeed before the new session becomes driver-owned. The first gameplay prediction, pair resolution and candidate validation also complete before adoption; a failed first tick leaves no session or carried view.

No bootstrap or later step uses frame delta, wall time or a Transform pose as simulation authority after quantization.

## Collision geometry and query policy

Constants are fixed at:

- skin `82 Q4096`;
- stationary support probe `205 Q4096` downward;
- blocking-normal component threshold `2867 Q4096`;
- nonalloc hit buffer capacity `64` per query.

Every query uses `Physics2D.DefaultRaycastLayers`, ignores triggers, and then excludes every collider whose Transform is the player root or its descendant, walker root or descendant, or surveyor root or descendant. Thus player and both enemies never block enemy locomotion; later M3D samples logical swept actor overlap. Every remaining enabled solid may block.

Queries use explicit Q4096-derived origin and size through the static nonalloc `Physics2D.BoxCast(Vector2, Vector2, float, Vector2, ContactFilter2D, RaycastHit2D[], float)` overload; they never temporarily move a body/collider and never call `Physics2D.SyncTransforms`. The walker is fully resolved before the surveyor, but both read the same pre-commit physics world and neither body moves until the complete pair candidate validates.

For each alive gameplay prediction:

1. For each moving axis, compute requested travel as the checked nonnegative 64-bit magnitude `R=abs((long)predicted-start)`. Overflow or a nonfinite converted value rejects before querying.
2. Issue exactly one BoxCast from the explicit Q4096-dequantized start center, with Q4096-dequantized box size, angle `0`, unit axis direction, a `ContactFilter2D` whose layer mask is exactly `Physics2D.DefaultRaycastLayers` and `useTriggers=false`, the 64-entry result array, and maximum distance `(R+82)/4096f`.
3. Resolve X from the Q4096 start center, then resolve Y from the X-resolved center. After null/trigger/actor-root exclusion, every environment hit must have finite distance, fraction and both normal components and a unique `ColliderIdentity` key.
4. The signed opposing normal component is `-normal.x` for right, `normal.x` for left, `-normal.y` for up and `normal.y` for down. A hit qualifies only when `floor(component * 4096) >= 2867`.
5. Quantize each qualifying distance as `max(0, floor(distance * 4096))`. Allowed travel is `max(0, distanceQ4096 - 82)`.
6. The axis is blocked only when allowed travel is strictly shorter than requested travel; otherwise it reaches the exact prediction and is unblocked.
7. A blocked downward Y movement is grounded; a blocked upward movement is not.
8. During gameplay only when `PriorGrounded=true`, predicted Y equals start Y and Y is unblocked, run one downward support probe from the final X/Y position using the same exact filter and validity rules. A qualifying upward-facing blocker within `205` yields `StationarySupportConfirmed=true`.

Zero-delta axes perform no cast. Dead identity predictions perform no cast or support probe. Encounter reset performs no cast or support probe and uses the M3C1 reset candidate's exact spawn positions and frozen initial grounded seeds. Select `EncounterReset` only when both behavior roles carry their exact Reset phases; a mixed Reset/non-Reset pair rejects before any query. Otherwise select Gameplay. Exact reset reactions and prior completed-gameplay eligibility remain M3C1 validation responsibilities.

If a nonalloc query returns `64`, saturation is indistinguishable from truncation and fails. Every retained hit must have finite distance/fraction and a unique canonical collider key. Candidate comparison order is floor Q4096 distance, then existing `ColliderIdentity` canonical order: scene name, ordinal hierarchy segments, collider type full name and component ordinal. Stable key duplication, invalid geometry, overflow or an invalid normal fails before core/body/view mutation.

## Pair transaction and body handoff

For gameplay, the exact order is:

1. validate all upstream/binding/body alignment and construct the Combat-only M3C1 input;
2. obtain the mutation-free M3C1 pair prediction;
3. resolve walker then surveyor without moving either body;
4. obtain immutable `ResolvedPairCandidate` from M3C1;
5. revalidate body/collider bindings, the complete frozen Rigidbody conditions, body current Q4096 positions and all known `MovePosition` preconditions;
6. commit M3C1 once with exact input, pair resolution and candidate;
7. call `MovePosition` for alive walker then alive surveyor using exact committed Q4096 positions; dead identity roles omit the call;
8. after both calls return, replace the carried motion view.

Reset uses the analogous exact reset preflight/commit, then schedules walker and surveyor spawn positions and publishes the reset motion view. Body/snapshot equality is checked at the next `-160`, after the prior physics step has consumed `MovePosition`, never immediately after scheduling.

All ordinary validation and query failures preserve the M3C1 pair, prior carried view, both body positions/move goals and expected tick. For valid, simulated kinematic bodies, `MovePosition` is the non-rejecting final Unity handoff after every known rejection condition is exhausted. An unexpected Unity engine exception during either final call is a fail-stop condition: do not invent rollback, do not publish a carried view, and report the exact role/tick. This exceptional engine-fault boundary is not represented as a recoverable gameplay branch.

`CarriedEnemyMotionView` is an internal immutable value containing exact tick and the committed M3C1 pair snapshot. Its only future production consumer is M3D. No Transform, Rigidbody, Collider, mutable list or request queue escapes the view.

## Allowed files

- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyLocomotionSimulationDriver.cs` and `.meta`
- narrow internal read-only seams in `RegularEnemyBehaviorSimulationDriver.cs` and `CombatSimulationDriver.cs`
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/RegularEnemyLocomotionSimulationDriverPlayModeTests.cs` and `.meta`
- narrowly required existing Combat Unity PlayMode test helpers only if a new fixture cannot reuse them without duplication
- `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md`
- this contract, its pre-gate, implementation evidence and documentation index

## Forbidden scope

- M3C1, M1, M2A, M3A, M3B1, M3B2A or VD-02 behavior changes
- new public ABI, public serialized field, registry, schema, assembly reference, layer or project setting
- `Physics2D.SyncTransforms`, temporary collider movement, solver force/velocity locomotion or collision callbacks
- player/enemy actor blocking, contact damage, projectile/denial-line lifecycle, `DamageRequest` or knockback
- dynamic discovery, instance-ID ordering, frame time, wall clock, runtime RNG or mutable publication
- scene, prefab, package, asset, boss, reward, room, run or persistence changes

## Required evidence

- exact `-200/-190/-180/-170/-160/default` order and active-aim pipeline sync count remains one;
- first tick zero and nonzero bootstrap; first-failure session absence; missing/stale/future behavior or reaction view preserves existing pair/body/view;
- wrong/missing bindings, inactive root, roster order/identity, collider/body/root mismatch, extra solid collider, trigger, `simulated=false`, dynamic body, position constraint, interpolation, full-kinematic-contact, nonzero linear/angular velocity, rotation/scale/offset/edge-radius, nonfinite geometry and body drift reject before mutation;
- exact 60-tick Approach, 12-tick DashActive, 48-tick Relocate and Heavy grounded/airborne/forced-descent/fall-clamp snapshot-to-body traces;
- left/right wall, upward ceiling, floor landing, grounded support, ledge departure and X-resolved Y corner origin;
- trigger and all three actor-root descendants ignored while environment blocks; opposing-normal filtering prevents floor from blocking X;
- equal-distance canonical ordering, duplicate stable key, NaN/Infinity and 64-hit saturation mutations preserve the entire pair and bodies;
- walker success followed by surveyor failure preserves both roles; no query or body move occurs after an earlier role/phase failure;
- after successful initialization, first death transition and repeated-dead identity ticks issue no query or MovePosition; resurrection rejects, and reset restores exact spawn/ground seeds without query;
- `t=int.MaxValue-1` terminal commit and later exact terminal input rejection without mutation;
- next-physics-step body Q4096 equals carried snapshot; carried view is immutable and exact;
- identical logical traces under 30/60/144 render grouping;
- source scan proves no added sync, temporary move, public surface, discovery, force/velocity locomotion, callbacks, time/RNG, damage or line dependency;
- full Combat PlayMode, full project PlayMode and full project EditMode regression pass.

## Ollama utilization record for contract formation

| Lane | Outcome | Sol screening |
|---|---|---|
| Kimi K3 | used and accepted in part | Accepted explicit-position X→Y nonalloc cast structure, full preflight before core commit and focused boundary fixtures. Rejected invented public interfaces, instance-ID-derived keys, support probing after core commit, and failure-time identity publication because failures preserve the prior view. |
| GLM 5.2 | used and accepted in part | Accepted cast saturation, X-resolved Y origin, half-commit, transform drift, stable ordering and render-group mutation classes. Rejected frame-delta injection, implicit transform rollback and its assumption that sibling collision must be sampled; Sol explicitly excludes all actor roots. |
| MiniMax M3 | failed and replaced | The bounded non-thinking fixture request returned an empty response without quota/rate error. Terra and Sol supplied the repeatable fixture matrix above; no retry loop or empty output was adopted. |

No cloud model received repository text, paths, credentials, personal data or secrets. Ollama output has no approval or integration authority.

## Stop conditions and approval gate

Stop and report to Sol if implementation needs a new sync, layer/project setting, public surface, non-box geometry, actor blocking, collision callback, upstream behavior change, recoverable rollback after a Unity engine exception, scene/prefab work, damage or denial-line production.

Implementation may begin only after Luna reports no P0/P1 contradiction and Sol changes this contract to `Approved`. M3C2 remains partial integration evidence and cannot alone close full `AC-COM-001`, `AC-COM-003`, `AC-WT-002` or `AC-WT-005`.
