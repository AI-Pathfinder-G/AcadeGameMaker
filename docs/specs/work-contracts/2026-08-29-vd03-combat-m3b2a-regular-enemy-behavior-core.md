# Work Contract: VD-03 M3B2A Regular Enemy Behavior Core

- Status: Approved — Sol, after Luna PASS, 2026-08-29
- Owning spec: `VD-03`, Approved 2026-08-25
- Contract owner, cross-system decisions and final integration: Sol
- Unit design and implementation after approval: Terra
- Independent contract and implementation verification: Luna
- Requirement IDs: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`
- Rollback point: commit `e544d73`

## Purpose and milestone boundary

M3B2A adds an engine-free, deterministic 60 Hz behavior planner for the literal `walker` and `surveyor`. It owns behavior phases, stable action ordinals and immutable action intents. It produces the walker's exact future recovery candidate needed by verified M3B1, but it does not submit that candidate to a Unity component.

M3B2A does not move a Rigidbody, query geometry, sample a Transform, spawn a denial line, resolve a hit, create `DamageRequest`, animate, render, author scenes or alter project settings. M3B2B will bind these pure outputs to Unity after a separate Approved contract.

## Ownership and authority

- M1 remains the sole health, death, damage-order and request-dedupe authority.
- M3A remains the sole raw Heavy, walker shield-window, surveyor fire-lock, forced-descent and recovery-multiplier authority.
- M3B1 remains the sole transfer-edge projection, combat/reaction arbitration and recovery-candidate queue/tombstone authority.
- M3B2A owns only behavior phase, phase end, encounter-local attack/shot ordinals and deterministic action intents.
- VD-02 remains the sole active-transfer and transfer-revision authority.

No M3B2A input or output may mutate another owner's snapshot or infer a same-tick M1 result.

## Common input, output and atomicity

Every step consumes one immutable pair input for exact `SimulationTick t`:

- walker and surveyor final-alive values copied from the already completed M1 snapshot for `t`;
- exact M3A walker and surveyor snapshots published for `t`;
- player, walker and surveyor positions quantized to signed Q1000 world units by a later adapter;
- one authored line-of-sight boolean per enemy, already sampled by that adapter;
- an `EncounterReset` discriminator mutually exclusive with gameplay payload.

The pure core validates nonnegative `t`, rejects default snapshots, validates literal snapshot IDs, each M3A snapshot `Tick=t` and `NextExpectedTick=t+1`, enum ranges, Q1000 subtraction safety and all reachable horizons before mutation. It also revalidates the M3A structural derivations: walker multiplier is exactly `1000|1500`, dead walker has attack permission false and multiplier 1000, live walker permission agrees with its exclusive shield end; surveyor has walker permission false and multiplier 1000, dead surveyor has forced descent/fire false, live forced descent equals raw Heavy, and live `CanFire` is true exactly when baseline and outside its exclusive fire-lock end. End ticks are either sentinel `-1` or valid exclusive horizons. Transfer revision is exactly sentinel `-1` or `1..int.MaxValue`; `0` is invalid, and raw Heavy requires a positive revision. It stages both enemy results, then commits both or neither. A failure preserves both snapshots, ordinals and next expected tick.

For each role, the copied M1 final-alive value must equal its M3A snapshot `IsAlive`. Any mismatch is a contract failure before either behavior session mutates.

Every successful step publishes one immutable pair snapshot containing the exact input tick, each enemy phase and exclusive phase-end tick, effective alive state, movement intent, guard state, optional walker recovery candidate, optional surveyor shot intent, action ordinals and `NextExpectedTick=t+1`. Caller collections are never retained.

All time arithmetic uses `SimulationTick` and `checked` signed arithmetic. Horizontal distance uses `long dx = (long)playerXQ1000 - enemyXQ1000` and a non-overflowing absolute `long` magnitude compared directly with `4000L`; it is never narrowed back to `int`, and `Math.Abs(int)` is forbidden. Direction is `-1`, `0` or `1`, with zero selected only for exact equality.

## Walker state machine

The walker begins each encounter alive in `Approach` with attack-cycle ordinal `0`.

All duration phases use post-transition snapshots. A phase entered at `entryTick=e` has `exclusiveEndTick=checked(e+D)`, owns outputs on ticks `e..e+D-1`, and transitions before producing the snapshot at tick `e+D`. `Approach`, `Dead` and `Reset` use end sentinel `-1`. Thus a qualifying Approach input at `t` publishes the first `DashTelegraph` snapshot at `t`, telegraph ticks are `t..t+17`, dash ticks are `t+18..t+29`, and recovery begins at `t+30` after the candidate was emitted on `t+29`.

| Phase | Duration / transition | Pure output |
|---|---|---|
| `Approach` | Remains until LOS is true and horizontal distance is within 4,000 Q1000. Entry to `DashTelegraph` occurs on that qualifying tick. | Horizontal direction toward the player; zero if aligned. |
| `DashTelegraph` | 18 ticks, exclusive end. Player direction is latched on entry; later player motion does not retarget this cycle. | No movement; telegraph intent and latched direction. |
| `DashActive` | 12 ticks, exclusive end. | Latched dash-direction intent. No hit or damage is inferred. |
| `Recovery` | Base 40 ticks multiplied once on entry by M3A `RecoveryMultiplierQ1000`: `ceil(40 × multiplier / 1000)`, therefore exactly 40 or 60 ticks. | No movement. Existing recovery duration never changes when Heavy changes later. |
| `Dead` | Latched until exact encounter reset. | No movement, attack or recovery output. |

On the final `DashActive` tick `t`, the core allocates the current nonnegative `long` attack-cycle ordinal and emits exactly one engine-free recovery intent `(targetId="walker", attackCycleOrdinal, scheduledTick=t+1)`. It checked-increments the ordinal only in the same successful commit; `long.MaxValue` fails before snapshot, ordinal or intent mutation. This pure shape does not reference the `Combat.Unity` assembly's `AttackRecoveryStarted` type. The next gameplay step at `t+1` enters `Recovery` and samples that tick's exact M3A multiplier once.

The intent is an immutable output; only a later Approved M3B2B contract may choose an execution order, convert it to M3B1's internal candidate and submit it after M3B2A tick `t`, while the global clock remains strictly before `t+1`. A delayed or duplicate submission is forbidden and never retried. M3B1 may discard/tombstone the converted candidate when same-tick applied HeavyImpact wins or death suppresses it. M3B2A still enters its scheduled recovery and never defers or re-emits the intent.

`GuardRaised` is derived, not independently owned: alive walker and `ReactionAllowsBasicAttack=false`. A 30- or 90-tick M3A window lowers the guard without changing the behavior phase or its deadlines.

## Surveyor state machine

The surveyor begins each encounter alive in `Relocate` with shot ordinal `0`.

| Phase | Duration / transition | Pure output |
|---|---|---|
| `Relocate` | 48 ticks, exclusive end. Direction alternates deterministically by shot ordinal: even `-1`, odd `1`. | Horizontal relocation direction. |
| `FireTelegraph` | 30 ticks, exclusive end. Player Q1000 position is latched on entry. | No movement; telegraph intent and immutable latched target point. |
| `Fire` | Exactly 1 tick. | One shot intent identified by `(surveyor, shotOrdinal)` and the latched point; ordinal increments in the same commit. |
| `Cooldown` | 60 ticks, exclusive end, then `Relocate`. | No movement or shot intent. |
| `Suppressed` | While `ForcedDescent=true` or `CanFire=false`. | Downward intent only while forced descent is true; never fires. |
| `Dead` | Latched until exact encounter reset. | No movement or shot output. |

The same post-transition boundary rule applies: Relocate owns 48 ticks `e..e+47`, FireTelegraph owns 30 ticks, Fire owns its single entry tick and emits exactly one intent there, and Cooldown owns 60 ticks. The following tick transitions before output. Fire checked-increments its nonnegative `long` shot ordinal in the same commit; `long.MaxValue` fails atomically without a shot, snapshot or ordinal change.

`ForcedDescent=true` or `CanFire=false` has priority over every live phase, cancels an unfinished relocation or telegraph without a shot, and enters `Suppressed` on that exact step. Baseline recall may clear forced descent while the residual 75-tick fire lock keeps `CanFire=false`; the surveyor remains suppressed without downward intent. When both `ForcedDescent=false` and `CanFire=true`, it restarts a full 48-tick `Relocate` phase. An already emitted shot intent is immutable and its later line lifetime belongs to M3B2B; fire-lock never retroactively erases it.

## Death and encounter reset

Final-alive false overrides every gameplay transition on that exact step. It publishes `Dead`, clears pending telegraph/latched target and emits no new movement, recovery or shot intent. Repeated dead ticks remain inert. Once the behavior dead latch is set, a later M1/M3A alive input before reset is rejected without mutation; resurrection is forbidden. A recovery intent already emitted on a prior tick is external immutable history: M3B2A neither removes nor regenerates it, and M3B1 alone consumes and tombstones any submitted candidate. M3B2A never schedules VD-02 target removal.

Reset is accepted only after at least one successful gameplay step, at the exact expected global tick with no gameplay payload, after both exact M3A reset snapshots for that tick are present. A reset at the first expected tick is rejected before mutation even if forged snapshots otherwise resemble reset output. Each valid reset snapshot must retain its literal role, publish `IsAlive=true`, raw baseline Heavy, revision/window sentinel `-1`, and `NextExpectedTick=t+1`; walker must publish guard closed and surveyor must publish `CanFire=false`, matching the verified M3A reset contract. Reset clears both behavior states, phase deadlines, latched values and encounter-local ordinals. It publishes for each role: phase `Reset`, end sentinel `-1`, `IsAlive=true`, direction `0`, guard false, no telegraph, no recovery/shot intent, no latched point, ordinal `0`, and `NextExpectedTick=t+1`. On the next ordinary gameplay tick `t+1`, walker publishes `Approach` unless that input immediately qualifies for post-transition `DashTelegraph`; surveyor publishes the first `Relocate` tick, both with ordinal `0`. Validation failure clears nothing and the global clock never rewinds.

## IDs, ordering and replay

- Walker recovery cycle IDs are the encounter-local `long` ordinals `0,1,2...`; checked increment rejects overflow before commit. M3B1 reset clears its encounter-local ID history before reused post-reset ordinal `0` can be submitted.
- Surveyor shot identity is the ordinal pair `(surveyor, shotOrdinal)`; it is not a `DamageRequestId`. Its checked increment has the same atomic `long.MaxValue` failure rule.
- Snapshot ordering is always walker then surveyor. At most one recovery candidate and one shot intent exist per pair step.
- No Unity instance ID, object creation order, hash iteration, wall clock, frame time or runtime RNG may affect output.
- Replaying the same 60 Hz input script under 30/60/144 render grouping must produce byte-for-byte equal logical snapshots and intent sequences.

## Allowed files

- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyBehaviorSession.cs` and required `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/RegularEnemyBehaviorSessionTests.cs` and required `.meta`
- narrowly required internal friend declaration only if tests cannot access the existing Combat assembly
- this contract, its pre-gate and later implementation evidence

## Forbidden scope

- any public ABI or public serialized component field
- `MonoBehaviour`, `Transform`, `Rigidbody2D`, collider, physics query or `Physics2D.SyncTransforms`
- modification of M1, M2A, M3A, M3B1 or VD-02 behavior
- actual locomotion integration, collision sampling, damage requests, projectile/denial-line lifecycle, animation, VFX, UI or audio
- dynamic discovery, mutable shared collections, floating-point simulation, frame time, wall clock or runtime RNG
- scenes, prefabs, packages, project settings, assets, boss, rewards, room completion, run state or persistence

## Required evidence

- exact first, last and following-tick post-transition boundaries for 18/12/40-or-60 and 48/30/1/60 ticks;
- recovery candidate emitted once at `t` for `t+1`, unique ordinal, no re-emission, and exact recovery multiplier sampled only at entry;
- walker guard follows M3A attackability without changing behavior phase;
- surveyor forced descent/fire lock priority, baseline residual-lock behavior, telegraph cancellation and full relocation restart;
- lethal step, repeated dead step, dead-then-alive rejection, prior external recovery intent ownership and reset clean every pending action without producing an intent;
- invalid IDs, default/invalid enum, revision `0`, Heavy-without-positive-revision, tick mismatch, stale M3A view, first-tick reset and malformed reset fail before either enemy commits;
- reachable deadline, Q1000 long delta, `t+1` candidate and both walker/surveyor ordinal overflow boundaries are checked and atomic; inactive/dead paths avoid unrelated horizon arithmetic;
- two-session validate-before-commit mutation cases preserve both snapshots and ordinals;
- exact snapshots and intent sequences match across 30/60/144 render grouping;
- source scan proves no Unity reference, public surface, floating point, time source, RNG or discovery API;
- full Combat EditMode and full project EditMode regression pass.

## Ollama utilization record for contract formation

| Lane | Outcome | Sol screening |
|---|---|---|
| Kimi K3 | used and accepted in part | Accepted separate pure FSMs, deadline-based phases, external reaction gating and future recovery tests. Rejected replacement-on-conflict recovery queues and silent stale-candidate discard because verified M3B1 rejects both. |
| GLM 5.2 | used and accepted in part | Accepted death/reset, `t-1` scheduling, overflow, stale observation, residual movement and fire-lock/lifetime risk classes. Rejected claims of concurrent fixed phases and global rollback; Unity phases are sequential and owner-local fail-stop remains authoritative. |
| MiniMax M3 | used and accepted in part | Accepted an engine-free fixture with exact transitions, stable IDs, dedupe and 30/60/144 comparison. Rejected `uint64` time, death-to-recovery and billion-tick compression because the approved clock is signed `SimulationTick`, death is inert, and exact boundaries need direct fixtures. |

No cloud model received repository text, paths, credentials, personal data or secrets. Ollama output has no approval or integration authority.

## Stop conditions

Stop and report to Sol if implementation needs Unity types, real movement/collision/projectiles, M1/M3A mutation, M3B1 arbitration changes, a public surface, new physics synchronization, scene/prefab changes, or any package/project-setting change.

## Approval gate

Implementation may begin only after Luna reports no P0/P1 contradiction and Sol changes this contract to `Approved`. M3B2A is partial substrate and cannot by itself close `AC-COM-001` or `AC-COM-003`.
