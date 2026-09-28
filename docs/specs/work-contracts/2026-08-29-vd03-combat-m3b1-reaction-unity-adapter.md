# Work Contract: VD-03 M3B1 Regular Enemy Reaction Unity Adapter

- Status: Approved — Sol, after Luna PASS, 2026-08-29
- Owning specs: `VD-03`, affected `VD-02`, Approved 2026-08-25
- Contract owner, cross-system decisions and final integration: Sol
- Unit design and implementation after approval: Terra
- Independent contract and implementation verification: Luna
- Requirement IDs: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`, affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005`
- Rollback point: commit `3ee8728`

## Purpose and milestone boundary

M3B1 connects the verified M3A regular-enemy reaction sessions to the verified Unity fixed-tick transfer and combat phases. It adds immutable internal result seams, exact-tick reaction publication and walker attackability gating. It does not yet create enemy locomotion, recovery-cycle production, surveyor firing, projectiles, collision sampling, animation, presentation, scenes or prefabs. Those belong to M3B2 or later approved units.

M3B1 must not change M1 damage semantics, M2A selection/cooldown semantics, VD-02 transfer semantics, public ABI or Unity project settings.

## Sol decisions

### Fixed-phase order and carried reaction view

The fixed order is:

1. `TransferSimulationDriver` at `DefaultExecutionOrder(-200)` publishes one immutable `TransferTickOutcome` for tick `t`.
2. `CombatSimulationDriver` at `DefaultExecutionOrder(-190)` validates the exact transfer outcome, samples the last successfully published reaction view, constructs M2A observations, completes M2A and M1, and publishes one immutable `CombatSimulationOutcome` for `t`.
3. `RegularEnemyReactionSimulationDriver` at `DefaultExecutionOrder(-180)` consumes the exact `t` transfer and combat outcomes, advances both M3A sessions once, and atomically replaces its carried reaction view.
4. Default-order movement runs after these phases.

Unity invokes these phases sequentially on the main thread. No phase reads a mutable snapshot being written by another phase.

The combat phase at tick `t` cannot observe reaction events derived from its own not-yet-produced M1 result. It therefore uses only the immutable carried reaction view published at the end of reaction tick `t-1`. A walker window created by reaction tick `t` gates the next `N` combat phases `t+1..t+N` inclusive: combat `t+N` still reads the active snapshot from reaction `t+N-1`, then reaction `t+N` publishes the expired raw window. This preserves exactly 30 or 90 usable attack phases without retroactive same-tick selection.

There is no carried snapshot before the first reaction publication. Walker attackability is closed on the first combat tick. Surveyor basic attackability does not depend on M3A's fire-lock directives.

### Effective M2A attackability

`CombatObservationBuilder` receives an immutable per-target attackability map prepared before capture. It does not discover or query reaction components.

- `walker`: `CombatTarget.CanReceivePlayerBasicAttack && AliveBeforeM1AtT && CarriedWalkerReactionAllowsBasicAttack`.
- `surveyor`, `ordan`, and other contract-valid non-player targets: `CombatTarget.CanReceivePlayerBasicAttack && AliveBeforeM1AtT`.
- `player` remains excluded from M2A observations.

Transfer availability, active-transfer identity, raw Heavy, surveyor `CanFire`, collider enablement and presentation state never enter this expression. A death produced later in M1 at `t` cannot retroactively alter the already-resolved M2A attempt at `t`; it prevents all later attacks through the combat-owned alive state.

### Immutable upstream result seams

`TransferSimulationDriver` stores its last successful internal `TransferPhasePublication`, containing the full `TransferTickOutcome` and exact-tick input summary, and exposes it internally to `AcadeGameMaker.Combat.Unity`. It replaces the value only after the VD-02 session and final player modifier reflection both succeed. A failed transfer phase retains the prior value. The engine-free VD-02 `TransferTickOutcome` type is not changed.

`CombatSimulationDriver` already stores its last successful `CombatSimulationOutcome`. M3B1 may add only narrowly named internal accessors and preflight hooks; neither outcome becomes public and neither retains mutable caller collections.

The reaction driver accepts only explicit serialized references to the player clock, transfer driver and combat driver. `CombatSimulationDriver` gains one explicit private serialized reaction-driver reference, with an internal test configurator, so it can invoke the mutation-free preflight before M1. This cyclic component binding must not trigger recursive initialization: reaction initialization validates its own clock/roster/session state without initializing combat, and reaction publication reads only combat's already-published outcome. There is no `Find*`, tag lookup, singleton, service locator or scene enumeration.

`CombatSimulationOutcome` carries the exact `EncounterReset` discriminator that produced it. Only the current exact-tick marker may reset reaction; an earlier retained outcome can never do so.

`TransferPhasePublication` carries an immutable exact-tick input summary with `HadAim`, `HadPress`, `RemovalCount` and nullable lifecycle reason. It describes consumed intent even when the attempt failed or produced no state transition. It contains no camera, pointer, scene or device data.

### Transfer-edge projection

For each successful tick, the reaction driver derives at most one edge for each literal reaction target from the transfer outcome:

- a successful `TransferStateChanged` for `walker|surveyor` becomes `Heavy` with the event's exact revision and tick;
- a successful `TransferCleared` whose previous target is `walker|surveyor` becomes `Baseline` with the event's exact revision and tick;
- removal clear is read from `RemovalResult`; manual/apply/recall is read from `PressResult`; lifecycle clear may also be carried by the successful result slot;
- each edge is validated against the exact `TransferAttemptResult.Session` that contains it; its target, tick, revision and resulting local active/baseline state must agree with that result;
- removal and press result slots may legally describe different target edges in the same tick, such as walker removal at revision 2 followed by surveyor apply at revision 3; edges are projected independently to their literal sessions in ascending revision order;
- if both result slots claim an edge for the same reaction target, revisions do not increase across the two result slots, or the final transfer snapshot does not agree with the last successful edge, preflight fails before combat mutation;
- unrelated box or boss-payload transitions do not enter either reaction session.

The adapter does not synthesize a missing baseline edge. VD-02 remains the sole transfer revision and active-target authority.

### Walker event arbitration

M3B1 produces `HeavyImpactApplied` only from the current canonical M1 `DamageResult` batch. It chooses the ordinal-first result whose target is `walker`, result is `Applied`, corresponding request kind is `HeavyImpact`, and exact request identity can be proven from the frozen current combat input. Results with `Invulnerable`, `Duplicate`, `TargetDead` or `Invalid` never open the window.

`CombatSimulationDriver` defensively copies the combined external-plus-basic-attack request batch, sorts that private trace by M1's existing full deterministic tuple (`RequestId`, `SourceId`, `TargetId`, `DamageKind`, `Amount`), and stores it beside the results in `CombatSimulationOutcome`. Every trace/result pair must have equal count, request ID, target ID and tick. This trace is diagnostic provenance only: it is never sent back into M1 and cannot change acceptance, ordering, health or dedupe. Same-ID requests remain legal and their exact sorted index, rather than a dictionary lookup by ID, identifies the request that produced each result.

The driver may accept at most one internal exact-tick `AttackRecoveryStarted(attackCycleId)` candidate from a later M3B2 producer. In M3B1 production no such producer exists, so the candidate is normally absent; tests may submit it through the same internal seam.

A recovery candidate for tick `t` must be submitted while the player clock still expects a tick strictly less than `t`; current-tick or past submission is rejected without queue mutation. M3B2 therefore predicts and queues recovery entry no later than reaction/AI tick `t-1`. The queue is unique by tick and cycle ID. At reaction `t`, the candidate is consumed exactly once. It is tombstoned even when a same-tick applied HeavyImpact wins arbitration or final M1 state is dead and M3A must receive `None`; it is never deferred. Encounter reset clears queued candidates and adapter-owned cycle tombstones after reset preflight succeeds.

Arbitration is fixed: ordinal-first applied heavy impact, otherwise the submitted recovery start, otherwise `None`. When an applied heavy impact wins, the same-tick recovery candidate is intentionally discarded, not queued to a later tick. Invalid, duplicate, stale or mismatched recovery submissions fail before upstream combat mutation through the combat preflight hook.

### Two-stage validation, publication and failure scope

Global rollback across already completed fixed phases is not promised. Each owner remains atomic only within its own phase.

`CombatSimulationDriver` invokes a mutation-free structural reaction preflight before observation capture and before M2A/M1 processing. That preflight verifies everything knowable without predicting M1:

- reaction driver initialization and exact expected tick for both sessions;
- the transfer driver's latest successful outcome is exactly tick `t`;
- transfer-event shape, final snapshot agreement and reaction-revision monotonicity;
- reset exclusivity and raw-Heavy reset eligibility;
- any queued recovery candidate's exact tick, uniqueness and ID rules.

Applied/ignored damage results and final alive states are not predicted before M1. At `-180`, the adapter first constructs both exact M3A inputs from the already immutable transfer and combat publications, then invokes a new mutation-free internal `RegularEnemyReactionSession.ValidateNext(input)` on both sessions. `ValidateNext` executes the same validation and checked arithmetic used by `Process` but commits no tick, snapshot, revision, window or tombstone. It checks `t+30` only for a live recovery candidate that actually wins arbitration, `t+75` only for a live surveyor Heavy edge, and `t+90` only for a live walker applied-HeavyImpact winner. Empty, dead, baseline, invulnerable, duplicate, target-dead and discarded-event paths perform no reaction-window arithmetic.

Only after both validations succeed does the driver call `Process` for walker and surveyor and atomically replace the carried pair. The two calls must be total after their identical successful validations; any mismatch is an invariant failure. A result-dependent overflow or other exact post-M1 validation failure preserves both reaction sessions, the carried pair and adapter queues, but does not roll back the already successful transfer or combat publication. The whole fixed pipeline becomes fail-stop at that tick.

An upstream `-200` failure prevents `-190` and `-180` from publishing the same tick. An upstream `-190` failure prevents `-180` publication. Inputs consumed by a failing owner remain subject to that owner's existing fail-stop rule.

### Death and encounter reset

The reaction driver derives final alive independently for `walker` and `surveyor` from the exact combat snapshots after M1. On a death tick it submits `WalkerReactionEvent.None` as required by M3A, even if an applied heavy-impact result exists. Transfer edges may still be mirrored under M3A's death rules.

Encounter reset is initiated only by the existing exclusive combat reset input at exact tick `r`. The transfer input summary for `r` must be entirely idle: no aim, press, removal or lifecycle intent, including an aim-only capture, a failed press or a no-op lifecycle submission. The same tick also contains no combat aim, attack press, external damage or queued recovery candidate. Raw Heavy for both sessions must already be false due to an earlier VD-02 clear and reaction publication. Combat resets M1/M2A at `-190`; reaction resets both M3A sessions at `-180`; the carried view becomes the two specified empty reset snapshots. Global tick continues to `r+1`.

M3B1 does not recreate transfer registrations cleared by a room lifecycle. Room re-authoring and run orchestration remain later milestones.

## Internal shape and public surface

- One new internal `RegularEnemyReactionSimulationDriver` MonoBehaviour is allowed.
- One internal immutable carried-view shape may contain exact walker and surveyor snapshots and publication tick.
- One internal recovery-candidate input shape and exact-future-tick queue are allowed.
- Narrow internal accessors and a defensively copied canonical request trace may be added to transfer/combat outcomes only when required to prove exact request kind/identity.
- No new public type, public member or serialized public field is allowed.

The new driver has `DefaultExecutionOrder(-180)` and `DisallowMultipleComponent`. Its serialized references are private and explicit. Test configuration methods remain internal.

Initialization freezes and validates a roster containing exactly one `walker` registration (`9/0/true`) and exactly one `surveyor` registration (`6/0/true`), both with matching transfer co-authoring. Missing, duplicated or mismatched literal roles fail before either combat or reaction session advances. Other already-approved combat roles may coexist and are ignored by the reaction driver.

## Allowed files

- `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyReactionSimulationDriver.cs`
- narrowly required mutation-free `ValidateNext` refactor in `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyReactionSession.cs` with focused regression tests
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatObservationBuilder.cs`
- narrowly required `TransferSimulationDriver.cs` and internal friend declarations
- Combat Unity EditMode/PlayMode tests and required `.meta` files
- `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md`
- this contract, pre-gate and verification evidence

## Forbidden scope

- public ABI or existing public-component schema changes
- M1 health, ordering, invulnerability or dedupe changes; M2A cooldown, target ordering or damage changes
- a second transfer, health, attackability or reaction authority
- `Physics2D.SyncTransforms`, new physics queries, frame time, wall clock, runtime RNG or Unity instance IDs
- enemy locomotion, attack-cycle state machine, surveyor projectile/damage production, collision sampling, animation, VFX, UI or audio
- scenes, prefabs, packages, project settings, assets, boss, rewards, room completion or persistence

## Required evidence

- execution order is exactly `-200 → -190 → -180 → default`, with no additional physics synchronization;
- first-tick walker closure and carried-view `t-1` semantics;
- natural 30-tick and impact 90-tick windows each create exactly N subsequent combat attack opportunities and close on the following combat phase;
- surveyor remains basic-attackable while its separate fire lock is active, subject only to authored attackability and alive state;
- only ordinal-first canonical `Applied HeavyImpact` to walker wins; all non-applied results are ignored; same-tick recovery is discarded when impact wins;
- same request IDs with different source, target, kind or amount remain aligned by the complete M1 ordering tuple and never use ambiguous ID-only lookup;
- transfer apply, manual clear, removal clear and lifecycle clear project exact target/tick/revision once; unrelated transitions are ignored;
- death suppresses events/directives, observes and verifies M2B's existing one-time `t+1` transfer-removal handoff, never schedules a second removal and never permits a later attack;
- reset is rejected while raw Heavy is mirrored and succeeds only as the exclusive aligned tick after clear;
- reset rejects aim-only transfer capture, failed transfer press, lifecycle input, queued removal and recovery candidate; a stale prior reset marker cannot reset a later tick;
- empty and dead ticks at `int.MaxValue-1` do not perform unnecessary 30/75/90 horizon arithmetic, while each actually proposed live window fails atomically at its exact overflow boundary;
- walker removal plus surveyor apply in one tick projects two independently validated increasing revisions and agrees with the final transfer snapshot;
- every structural preflight mutation test preserves combat, M2A, both M3A sessions, carried view and queued candidate;
- post-M1 validation distinguishes `Applied`, `Invulnerable`, `Duplicate`, `TargetDead` and lethal walker/surveyor cases at the exact 30/75/90 overflow boundaries; a required window overflow preserves both M3A sessions/carried view/queues while retaining the already published M1 result and stopping the pipeline;
- `ValidateNext` and `Process` accept and reject the same M3A inputs, and successful validation alone changes no observable state;
- no dynamic discovery, new public surface or changed public component schema;
- deterministic same-script traces match across 30/60/144 render grouping;
- final regression includes Combat EditMode, Combat Unity PlayMode and full project EditMode.

## Ollama utilization requirements

- Kimi K3: one bounded adapter/arbitration implementation or boundary-test experiment from this frozen abstract contract; Terra screens and rewrites it.
- GLM 5.2: pre-implementation failure-mode pass and post-implementation scenario-gap sweep.
- MiniMax M3: deterministic transition/reaction fixture or mutation-matrix proposal; Terra may integrate only contract-conformant parts.
- Evidence records each lane as `used and accepted`, `used and rejected`, `failed and replaced` or concrete `not applicable`.

### Pre-draft GLM record

GLM was used and accepted in part on a redacted abstract summary. Sol accepted the warning that cross-phase rollback cannot be promised after upstream publication and added mutation-free reaction preflight plus phase-local failure language. Sol rejected its alleged main-thread snapshot concurrency race and its claim that the carried window necessarily loses one opportunity; fixed sequential phases and the explicit expiry carry rule resolve both. No repository content, local path, credential, personal data or secret was sent.

## Stop conditions

Stop and report to Sol if the adapter requires public ABI, dynamic discovery, a second physics sync, mutable shared collections, reaction decisions inside M1/M2A, transfer-state reconstruction, actual enemy movement/attack logic, scene/prefab changes, or any package/project-setting change.

## Approval gate

Implementation may begin only after Luna independently reports no P0/P1 contradiction and Sol changes this contract to `Approved`. M3B1 remains adapter substrate; it does not complete `AC-COM-001` enemy behavior distinction or `AC-WT-002` playable heavy-enemy reaction by itself.
