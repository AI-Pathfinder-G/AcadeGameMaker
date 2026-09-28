# Work Contract: VD-03 M3D2 Threat Delivery and Unity Bridge

- Status: Approved — implementation-correction addendum
- Initially approved by Sol: 2026-08-31 after Luna pre-gate PASS (`P0=0`, `P1=0`)
- Reopened by Sol: 2026-08-31 after Luna implementation review found that M2A mutation-free preflight and bounded collider validation needed explicit contract authority
- Reapproved by Sol: 2026-08-31 after Luna addendum pre-gate PASS (`P0=0`, `P1=0`)
- Owning spec: `VD-03`, Approved 2026-08-25
- Contract owner, phase order and integration decisions: Sol
- Unit design and implementation after approval: Terra
- Independent contract and implementation verification: Luna
- Requirement IDs: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-MOV-001`, `REQ-WT-005`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`; affected `AC-MOV-001`, `AC-WT-005`
- Rollback point: commit `c6d480c`

## Purpose and indivisible boundary

M3D2 integrates verified M3D1 without same-tick retrocausality. M3D2A adds one Combat-owned future threat-delivery lane. M3D2B runs after default-order player movement at execution order `+100`, consumes exact completed tick-`t` logical publications, advances M3D1, and commits its immutable requests for Combat tick `t+1`.

M3D2A and M3D2B are one Approved unit because enabling exact threat-batch presence without its producer would leave an invalid intermediate pipeline. They may be reviewed in separate implementation passes but are integrated only together.

The exact fixed order becomes:

`-200 Transfer → -190 Combat → -180 Reaction → -170 Behavior → -160 Enemy Locomotion → default Player Movement → +100 Enemy Threat`

Combat at `-190` never reads a threat generated later in the same tick. Threat source tick `t` is delivered only at `-190` tick `t+1`.

## Ownership

- M1 remains the sole request sorter, dedupe, health, invulnerability and death authority.
- M3D1 remains the sole walker swept-contact, denial-line, shot ordinal, death-latch and threat-candidate authority.
- M3D2A owns only threat-lane registration, one immutable batch per delivery tick, exact consumption and merge staging.
- M3D2B owns only Unity binding, frozen hurtbox extraction, exact post-movement input construction, pair preflight/commit and carried threat publication.
- M2A input, external damage, reactions, behavior, locomotion, Movement, Transfer and presentation retain their existing ownership.

## M3D2A internal delivery lane

`CombatSimulationDriver` exposes internal, nonserialized seams only:

- `PrepareThreatDeliveryRegistration()` to initialize and return the Combat-owned frozen roster while leaving the lane inactive;
- `RegisterThreatDeliveryLane(firstSourceTick)`;
- `PreflightThreatDelivery(sourceTick, requests)` returning an immutable delivery candidate;
- `CommitPreflightedThreatDelivery(candidate)`.

Registration:

- occurs exactly once from `RegularEnemyThreatSimulationDriver.Awake`, whose `+100` script order is still before every `FixedUpdate`, or from the exact test initialization seam; no lazy registration from `FixedUpdate` is permitted;
- `Awake` first validates the exact driver graph, calls `PrepareThreatDeliveryRegistration`, freezes all threat geometry while the lane is still inactive, and only then calls the registration seam;
- preparation may perform the existing atomic Combat initialization but never activates or queues the threat lane. Geometry failure therefore leaves an initialized legacy-compatible Combat driver with no active threat lane, and a corrected exact test configuration may retry;
- the registration seam itself never initializes Combat and requires `firstSourceTick == CombatSession.NextExpectedTick == BasicAttackSession.NextExpectedTick == PlayerMovementController.NextExpectedTick`, no latest outcome and no prior Combat-phase attempt;
- requires a nonnegative `firstSourceTick` that leaves room for both source publication and next-tick delivery under Combat's frozen production horizon: with `H=max(21, maxRegisteredInvulnerabilityTicks)`, `firstSourceTick <= int.MaxValue-H-1`; Combat alone may still publish through `int.MaxValue-H`, and this lane does not weaken or redefine that existing horizon;
- freezes `firstDeliveryTick=firstSourceTick+1` and marks the lane active;
- duplicate, late, unprepared or mismatched registration rejects without changing Combat or lane state;
- isolated legacy tests that do not register the lane retain the pre-M3D empty behavior. The production M3D2B component must complete registration in `Awake` before the first `-190 FixedUpdate`; later composition validation will require that component in authored gameplay scenes.

Preflight accepts one source tick and an immutable zero-to-two request collection:

- source tick must equal Combat's already-completed latest outcome tick and `CombatSession.NextExpectedTick-1`, and must itself have passed Combat's unchanged frozen production horizon when published;
- delivery is exactly `sourceTick+1 == CombatSession.NextExpectedTick`, must be at most `int.MaxValue-1`, and must independently pass Combat preflight horizon `deliveryTick+H <= int.MaxValue`; thus the latest M3D-deliverable source is `int.MaxValue-H-1` and its delivery is `int.MaxValue-H`;
- no batch may already exist for that delivery tick;
- caller order is exactly walker request then Surveyor-line request, with either independently absent;
- walker request has the exact M3D1 ID for source tick, source `walker`, target `player`, amount `1`, kind `EnemyContact`, tick delivery;
- line request has the exact M3D1 prefix, 19-digit nonnegative ordinal, exact 10-digit source-tick suffix, source `surveyor`, target `player`, amount `1`, kind `EnemyRanged`, tick delivery;
- IDs are parsed using invariant ordinal ASCII rules without regex culture or integer narrowing;
- input is defensively copied and every string/field is validated before returning a candidate.

Commit recomputes candidate structure and rechecks only state that cannot change between preflight and commit on the same main-thread call stack: registered lane, latest source tick, next delivery tick and empty slot. With the exact candidate returned by preflight and no intervening call, it has no ordinary rejection path. A forged/stale/duplicate candidate rejects before queue mutation. M3D2B treats an unexpected post-M3D1 commit exception as fail-stop and publishes no carried threat view; it never fabricates rollback of the already committed pure core.

An empty batch is a real publication. It proves the producer completed source tick `t` and is required at delivery `t+1` once the lane is active.

## Combat consumption and merge

- The registered first Combat tick is the only bootstrap tick and requires no prior threat batch.
- Every later Combat tick requires exactly one batch for itself, including an empty batch. Missing, stale, future or duplicate delivery fails before M2A or M1 mutation.
- The exact batch is consumed once. Existing `CombatSimulationInput.ExternalDamageRequests`, threat requests and the optional M2A basic-attack request are copied into one batch. Producer insertion order carries no authority; M1 performs its unchanged canonical sort/dedupe.
- M2A retains sole sequence, selection, cooldown and request authority. `BasicAttackSession` gains only a mutation-free exact-input preview candidate and exact commit seam; the Combat driver holds no shadow sequence set. Existing `Process` delegates through those seams without semantic change. Combat obtains the M2A candidate during preflight, consumes the threat batch after every rejection-capable validation, then commits that exact M2A candidate before M1. A forged/stale candidate rejects before either owner mutates.
- On `EncounterReset`, Combat stages a new unowned `BasicAttackSession(t)`, previews the neutral tick-`t` input on that staged owner, completes every remaining Combat/threat validation, consumes and discards the exact pending threat batch, exact-commits the neutral candidate to that same staged owner, then performs M1 reset and exact frozen-combatant re-registration and adopts the staged M2A session. Candidate construction and all checked reset/register prerequisites are validated before the first commit; the exact staged M2A commit, M1 reset/register and adoption have no ordinary rejection path. Until that commit boundary, the current M2A/M1 sessions and lane queue remain unchanged except for the separately documented legacy Combat-input consumption.
- The threat lane never overwrites or constructs `CombatSimulationInput`, so aim/attack submission and threat production cannot collide.
- On `EncounterReset`, the exact pending threat batch is still required and consumed but every threat request is discarded before M1 reset processing. Reset remains exclusive with external/aim/attack input and cannot take old-encounter damage.
- After reset, source `t` M3D1 reset produces an exact empty delivery for `t+1`; the lane stays registered and global ticks do not restart.
- Queue order is explicit: Combat first selects and removes the exact `CombatSimulationInput` under its legacy consume-on-attempt rule; it then peeks and validates the required threat batch without removing it; it runs all remaining Combat preflight; only after every preflight succeeds does it remove the exact threat batch immediately before deterministic reset/attack/damage processing.
- A delivery validation or later Combat preflight failure is fail-stop. It does not mutate M1/M2A, latest Combat outcome, threat-delivery queue or dedupe state. The already selected Combat-input queue entry remains consumed under the legacy rule; "queue preservation" in this contract otherwise means the distinct threat-delivery queue.

## M3D2B binding and initialization

`RegularEnemyThreatSimulationDriver` is internal, nonpublic, `[DefaultExecutionOrder(100)]` and explicitly binds player movement, Combat, Behavior and Enemy Locomotion drivers. Its `Awake` validates that Behavior is bound to the same player/Combat/reaction graph and Locomotion to that same player/Combat/reaction/Behavior graph, prepares Combat, freezes geometry, then registers the lane before any `FixedUpdate`; tests use one exact initialization seam with identical validation and registration. No Combat→Threat serialized reference or role/component selection discovery is added.

At `+100`, the exact source tick is `t = player.Snapshot.Tick` and `player.NextExpectedTick=t+1`. Before creating or advancing M3D1, the driver requires:

- latest Combat outcome `Tick=t`, with final `player`, `walker`, `surveyor` combat snapshots present exactly once;
- exact `CarriedBehaviorView.Tick=t` and pair tick/next tick `t/t+1`;
- exact `CarriedEnemyMotionView.Tick=t` and pair tick/next tick `t/t+1`;
- alive/dead equality across Combat, Behavior and Motion for walker/surveyor;
- player final-alive copied only from Combat outcome;
- literal Combat-owned roster identities and frozen geometry below.

The driver reads no Rigidbody or Transform position. Player center comes only from `PlayerMotionSnapshot` and enemies only from M3C1 carried motion.

On first use, all bindings, publications and geometry validate before a staged M3D1 session becomes owned. The staged session's preview, Combat delivery preflight, M3D1 commit, delivery commit and carried publication must all succeed in that order. A failure before M3D1 commit leaves no owned session/view/batch. Later ordinary validation or preflight failure preserves M3D1, prior view and queue.

Reset selects M3D1 `EncounterReset` only when Combat outcome, both Behavior roles and both Motion roles all carry their exact Reset forms. Mixed reset/gameplay sources reject before preview or queue work.

## Frozen hurtbox authoring

The driver aliases Combat's already frozen roster; it creates no second registry. Literal `player`, `walker` and `surveyor` must retain their verified Combat identity and ordinal roster order. The pinned scalar identities are exact: player `(health 5, invulnerability 45, attackable false)`, walker `(health 9, invulnerability 0, attackable true)`, and surveyor `(health 6, invulnerability 0, attackable true)`, together with their already frozen role-specific transfer bindings.

Player threat geometry is frozen from the exact Combat-owned target collider:

- one enabled non-trigger `CapsuleCollider2D`, the only enabled non-trigger collider on or below the player root;
- the explicitly bound `PlayerMovementController.transform`, frozen player `TargetPoseRoot`, collider Transform and attached simulated kinematic Rigidbody2D Transform are the same active Transform; cross-wired controller/root/body/collider objects reject;
- capsule direction is vertical, offset zero, finite positive size;
- world XY scale exactly `(1,1)`, Z rotation `0`;
- half-width and half-height are independently quantized AwayFromZero from `size/2` to positive signed Q4096.

Walker geometry is frozen from the exact Combat-owned target collider:

- the same active, unit-scale, zero-rotation, zero-offset, finite positive `BoxCollider2D`/kinematic-body/root form required by M3C2;
- it is the only enabled non-trigger collider on or below the walker root;
- half extents are independently quantized AwayFromZero from `size/2` to positive signed Q4096.

Surveyor geometry is not used by M3D1 but its literal roster binding must exist. Every retained binding and frozen half extent is revalidated before each preview and immediately before M3D1 commit. The driver never repairs malformed authoring.

To prove the “only enabled solid collider” authoring invariant without adding serialized lists or selecting gameplay roles dynamically, at most one bounded `GetComponentsInChildren<Collider2D>(true)` enumeration per explicit root per validation/revalidation pass is authorized: one for the already frozen player root and one for the already frozen walker root. Each call may only count enabled non-trigger colliders and compare each to the already frozen exact collider; it may not choose a collider, identity, body, root or role, and no enumerated reference escapes the call. All other runtime discovery remains forbidden.

## Pair transaction and publication

For every source tick `t`, gameplay or encounter reset:

1. validate exact post-movement and upstream publications, retained geometry and session alignment;
2. build immutable M3D1 gameplay or reset input from values only; reset requires the exact paired Reset publications and therefore yields an empty candidate;
3. obtain mutation-free M3D1 candidate;
4. obtain mutation-free Combat delivery candidate from the candidate's copied requests;
5. revalidate bindings, publications and geometry;
6. commit M3D1 once;
7. commit the exact preflighted Combat delivery batch;
8. publish `CarriedEnemyThreatView(t, M3D1 snapshot)`.

The same preservation boundary applies to reset: any failure through delivery preflight or revalidation preserves the prior owned M3D1 session/view and threat queue; an unexpected delivery-commit exception after M3D1 reset commit is fail-stop, publishes no view and never fabricates rollback.

`CarriedEnemyThreatView` is an internal immutable value with exact source tick and complete M3D1 snapshot only. No request array, Transform, collider, queue or mutable object escapes. Presentation may later consume line state from this view under a separate contract.

## Allowed files

- new `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyThreatDelivery.cs` and `.meta`
- new `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyThreatSimulationDriver.cs` and `.meta`
- narrow internal seams and merge logic in `CombatSimulationDriver.cs`
- narrow mutation-free preview/exact-commit seams in `Assets/AcadeGameMaker/Runtime/Combat/BasicAttackSession.cs`, with no M2A semantic change
- narrow internal carried-view and exact graph-identity validation seams in `CombatSimulationDriver.cs`, `RegularEnemyBehaviorSimulationDriver.cs` and `RegularEnemyLocomotionSimulationDriver.cs`; these are nonserialized and may only compare already explicit references or return immutable values
- new `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/RegularEnemyThreatSimulationDriverPlayModeTests.cs` and `.meta`
- narrowly required existing Combat Unity and BasicAttack EditMode test helpers only
- `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md`
- this contract, pre-gate, implementation evidence and documentation index

No assembly definition, public ABI, scene, prefab or project setting change is authorized.

## Forbidden scope

- M1, M2A, M3A, M3B, M3C, M3D1 or VD-02 semantic changes
- same-tick threat damage, Combat input overwrite, producer-order damage authority or global rollback
- physics query, `Physics2D.SyncTransforms`, collision/trigger callback, Transform/Rigidbody position sampling or body movement
- public/serialized surface beyond the private driver bindings, second registry, role/component selection discovery, or discovery outside the authorized bounded per-root collider-invariant enumerations
- line rendering/blocking, projectile travel, knockback, animation, VFX, audio or UI
- time, wall clock, frame delta, runtime RNG, instance ID or mutable publication
- boss, reward, room completion, run, persistence, scene, prefab, package, asset or project-setting work

## Required evidence

- exact execution order `-200/-190/-180/-170/-160/default/+100`;
- preparation initializes Combat and returns its frozen roster while the lane and queue remain inactive/empty; geometry failure after preparation leaves them inactive/empty, and corrected in-place retry registers exactly once; unprepared registration rejects atomically;
- `Awake` registration at zero/nonzero first ticks, largest M3D-registration-admissible first source tick, duplicate/late/mismatch/prior-attempt rejection, BasicAttack/Combat/player alignment and isolated unregistered Combat compatibility;
- first Combat bootstrap without batch, then exact nonempty and empty presence; missing/stale/future/duplicate/forged batch preserves M1/M2A/outcome/threat-delivery queue while the selected Combat input follows its documented legacy consumption rule;
- exact walker/line request parsing, order/copy isolation, zero/two capacity and malformed ID/source/target/amount/kind/tick/ordinal cases;
- external + walker + line + basic attack merge reaches M1 once and follows unchanged canonical ordering/dedupe;
- same delivery tick threat damage and player attack neither cancels the other; player invulnerability age 44/45 remains M1-owned;
- BasicAttack preview is mutation-free and exact commit is behaviorally identical to legacy `Process` for neutral, aim-only, press hit/miss, cooldown and duplicate-sequence paths; forged, stale and double candidates reject atomically; reset previews/commits on one staged owner before adoption; source scan proves Combat owns no shadow M2A sequence store;
- reset consumes/discards a nonempty prior batch, resets health/dedupe without damage, runs the same pair transaction, and requires the next empty publication;
- the largest M3D-deliverable source `int.MaxValue-H-1` under Combat's frozen horizon delivers at exact tick `int.MaxValue-H`; Combat-only publication at its separate `int.MaxValue-H` boundary remains unchanged, isolated M3D1 retains its already verified `int.MaxValue-2` boundary, and exhausted source/delivery or candidate mutation rejects atomically;
- +100 zero/nonzero bootstrap and exact Combat/Behavior/Motion/Movement horizons;
- exact graph identity accepts the intended player/Combat/reaction/Behavior/Locomotion chain and rejects each cross-wired driver reference before lane activation or session/view publication;
- missing/stale/future/mixed-reset publications and malformed second role preserve session/view/queue;
- complete player capsule and walker box authoring mutation matrix, pinned scalar-role mutations, cross-wired player controller/root/body/collider, extra solid collider and retained-geometry drift rejection;
- stationary/crossing/dash walker contact is absent at source `t` Combat and appears exactly at `t+1` Combat;
- line spawn/touch/expiry and post-Surveyor-death persistence reach exact next-tick Combat requests;
- source-tick player death suppresses all new M3D1 requests; walker death suppresses new walker contact; Surveyor death suppresses new line spawn but an already active denial line may keep generating its M3D1 request until expiry; in every case an already delivered prior batch still resolves normally;
- actual `MovePosition`/physics-step timing cannot change logical threat traces because no body/Transform position is read;
- identical logical and delivery traces under 30/60/144 render grouping;
- source scan proves no added sync/query/callback/body sampling, public surface, role-selection discovery, discovery outside the authorized bounded collider-invariant enumeration, time/RNG, same-tick damage or presentation dependency;
- focused Threat PlayMode, full Combat PlayMode, full project PlayMode and full project EditMode regressions pass.

## Ollama utilization record for contract formation

| Lane | Outcome | Sol screening |
|---|---|---|
| GLM 5.2 | used and accepted in part | Accepted ghost-hit latency as a contract/UI risk, exact line activation ambiguity, simultaneous-threat ordering, invulnerability boundary and reset-leak mutations. Sol resolves latency with completed logical tick `t` and explicit delivery `t+1`; M1 retains invulnerability authority. |
| Kimi K3 | used and accepted in part | Accepted immutable bounded batch, preflight/commit separation, explicit active interval and stable per-source identities. Rejected its numeric IDs, unsigned clock, two-slot global queue and producer-owned damage order; M1 string ABI and Combat-owned exact lane remain authoritative. |
| MiniMax M3 | failed and replaced | The bounded non-thinking fixture request returned empty text with no quota/rate error. Terra's impact matrix and Sol's required evidence replace it; no empty output is adopted. |

No cloud model received repository text, paths, credentials, personal data or secrets. Ollama output has no approval or integration authority.

## Stop conditions and approval gate

Stop and report to Sol if implementation needs a Combat→Threat serialized dependency, public type/member, assembly change, same-tick delivery, query/callback/body pose, different M3D1 behavior, line presentation, scene/prefab/project work or a recoverable rollback after the post-core delivery boundary.

Correction implementation may resume only after Luna reports no P0/P1 contradiction on this addendum and Sol changes this contract back to `Approved`. M3D2 remains partial integration evidence and does not alone close `AC-COM-001` or `AC-COM-003`.
