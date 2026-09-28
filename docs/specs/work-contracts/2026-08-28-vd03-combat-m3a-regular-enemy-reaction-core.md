# Work Contract: VD-03 M3A Regular Enemy Reaction Core

- Status: Verified — implemented by Terra, independently verified by Luna, integrated by Sol, 2026-08-28
- Owning specs: `VD-03`, affected `VD-02`, Approved 2026-08-25
- Contract owner and final integration: Sol
- Unit design and implementation: Terra; Kimi K3 may receive only a redacted abstract work specification and return isolated drafts
- Independent verifier: Luna
- Requirement IDs: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`, affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005`
- Rollback point: commit `8e6860c`

## Purpose and milestone boundary

M3A creates the engine-free deterministic reaction authority for the two regular enemy types. It owns only transfer-response and vulnerability/fire-lock windows. It does not implement Unity movement, collision, attacks, animation or target modifier sinks. A later M3B Unity adapter and behavior contract consumes these immutable outputs.

M3A must keep the verified M1 health/dedupe semantics and M2A targeting semantics unchanged. It may not alter any existing public type. All new types are internal to `AcadeGameMaker.Combat`.

## Sol decisions

### Shared clock and lifecycle

- Each enemy reaction session is constructed with a literal target ID (`walker` or `surveyor`) and exact first expected nonnegative `SimulationTick`.
- One immutable input is processed per exact tick. Skipped, duplicate, reversed or mismatched-target input fails before state mutation.
- Input carries the combat-owned final alive snapshot after M1 for the tick, zero or one transfer modifier transition with its exact transfer revision, and the enemy-specific event described below. A future Unity realization runs this reaction publication after the verified `-190` combat phase; M3A itself contains no execution-order attribute.
- Repeating the same or an older transfer revision is a contract error. `Heavy` is edge-triggered: remaining heavy across later ticks does not restart a window.
- Transfer revisions are exactly `1..int.MaxValue`. Gaps are allowed, but each received revision must be greater than the last received revision. A higher-revision `Heavy→Heavy` or `Baseline→Baseline` no-op is a contract error rather than a new edge.
- Death suppresses every effective gameplay directive on the processing tick and later ticks but preserves the raw mirrored heavy state until the expected `t+1` clear arrives. On every `finalAliveAfterM1=false` tick, the walker event must be `None`; any event payload is rejected before mutation, window arithmetic or ID consumption. Transfer transitions still validate and commit their raw state and revision, but no 30/75/90 reaction window starts or extends while dead. Thus a dead surveyor may temporarily mirror `IsHeavy=true` without forced descent or a new fire lock. The expected higher-revision baseline clear retires the mirrored state deterministically. M3A does not invent a transfer clear or combat event. A dead session cannot accept `alive=true` again without encounter reset. Transfer removal remains M2B's `t+1` responsibility.
- Encounter reset is allowed only after at least one completed gameplay tick and only when raw `IsHeavy=false`; the VD-02 lifecycle clear must arrive first if a transfer remains mirrored. It is an explicit input at exactly the prior snapshot's `NextExpectedTick=r`, mutually exclusive with final-alive gameplay, transfer transitions and enemy events. Preflight first computes `checked(r+1)`, then reset clears the dead latch, raw Heavy, both window ends, last transfer revision, and all impact/attack-cycle tombstones. It publishes one empty new-encounter snapshot at `Tick=r` with `IsAlive=true`, `IsHeavy=false`, revision sentinel `-1`, end-tick sentinel `-1`, `ReactionAllowsBasicAttack=false`, `RecoveryMultiplierQ1000=1000`, `ForcedDescent=false`, `CanFire=false`, and commits `NextExpectedTick=r+1`. Empty reset publication is the sole exception to the live-baseline surveyor `CanFire` derivation; the next ordinary alive baseline tick publishes true. The global clock never restarts at zero.
- On the alive-to-dead transition tick, an optional valid transfer edge may be mirrored but the walker event must be `None`. After the dead latch is set, ordinary event-free dead ticks may advance; a Heavy edge is forbidden, and the only permitted transition is a higher-revision `Baseline` clear while raw Heavy is true. A redundant Baseline clear while raw Heavy is false is a mutation-safe error.
- All end ticks are exclusive and checked before publication. A window started at tick `t` for `N` ticks is active on `t..t+N-1` and inactive at `t+N`.

### Collection Walker (`walker`)

The walker reaction state owns `ShieldWindow`, raw `IsHeavy`, `ReactionAllowsBasicAttack` and `RecoveryMultiplierQ1000` only. VD-02's target owner remains the sole authority for the regular-enemy Heavy movement, gravity, air-control and knockback-resistance modifiers.

- Baseline closed shield publishes internal `ReactionAllowsBasicAttack=false`; a shield window publishes `true` only while the post-M1 alive snapshot is true. A later M3B contract must compute `EffectiveIsAttackable = CombatTarget.CanReceivePlayerBasicAttack && PostM1FinalAlive && ReactionSnapshot.ReactionAllowsBasicAttack` before constructing M2A observations. Transfer availability and active-transfer state do not enter that expression. M3A does not construct observations or change M1, M2A or the verified M2B adapter in this milestone.
- A later authored attack-recovery producer may submit one `AttackRecoveryStarted(attackCycleId)` event with a nonnegative encounter-unique signed-64 ID. It opens a natural basic-only punish window for exactly 30 ticks. Reusing a cycle ID is rejected before mutation. Sol pins 30 ticks as a local vertical-demo balance value: VD-03 does not prescribe its duration, but `REQ-COM-001` requires a basic-attack-only completion path, so a deterministic natural opening is mandatory and remains tunable only through a later approved contract revision.
- An applied heavy-box collision may submit one `HeavyImpactApplied` event using the exact accepted `DamageRequest.RequestId`. It opens or extends the shield to `checked(tick+90)`.
- Reusing a consumed heavy-impact request ID is rejected before mutation. The input is a structurally singular discriminated event, so M3A never accepts an event collection. If recovery start and applied heavy impact coexist in one tick, the future producer must choose the ordinal-first `Applied` heavy-impact result and omit recovery start; otherwise it emits recovery start, otherwise `None`. M1 still owns and reports the full damage-result batch according to its verified rules.
- A window never shortens: the exclusive end becomes `max(currentEnd, proposedEnd)`.
- Entering `Heavy` sets the walker-local recovery multiplier to `1500`; returning to baseline restores it to `1000`. The separate VD-02 target owner continues to apply its exact `750/2200/350/2000` Q1000 movement-speed/gravity/air-control/knockback-resistance modifiers once; M3A neither republishes nor reapplies them. A future AI phase samples the recovery multiplier only when entering recovery and computes `ceil(baseTicks×multiplier/1000)` once; applying or clearing Heavy during an already-started recovery does not retroactively resize it. Heavy transfer alone does not open the shield.
- The reaction core emits no movement delta, collision query, damage request, animation or reward.

### Floating Surveyor (`surveyor`)

The surveyor reaction state owns `IsHeavy`, `ForcedDescent`, `FireLockEndTick` and `CanFire` only.

- Entering `Heavy` on an alive gameplay tick immediately sets `ForcedDescent=true` and starts or extends the fire lock to `checked(tick+75)`. A Heavy edge on a dead tick is mirrored but starts no lock.
- Remaining heavy does not refresh the lock. Returning to baseline clears `ForcedDescent` but does not erase the remaining overheat lock.
- `ForcedDescent` and `CanFire` are effective directives. Both are false while dead. While alive, `ForcedDescent` mirrors Heavy and `CanFire=false` while forced descent is active or `tick<FireLockEndTick`. At the first alive baseline tick equal to the exclusive end, `CanFire=true`.
- A new later heavy edge may extend but never shorten the current lock.
- The reaction core emits no altitude, velocity, ground collision, projectile, damage request, animation or audio.

## Input and output shape

The internal input contracts contain only ordinal IDs, integers, booleans, enums and existing `SimulationTick` values. They contain no Unity references.

- Shared transfer transition: target ID, `Baseline|Heavy`, strictly increasing transfer revision in `1..int.MaxValue`, exact processing tick.
- Walker event: `None|AttackRecoveryStarted|HeavyImpactApplied`, plus attack-cycle ID only for `AttackRecoveryStarted` and request ID only for `HeavyImpactApplied`.
- Surveyor has no extra event in M3A.
- Output is a full immutable snapshot containing `Tick`, literal `TargetId`, `IsAlive`, raw `IsHeavy`, `LastTransferRevision`, raw exclusive window/lock end tick, effective directives and `NextExpectedTick`. Dead walker snapshots publish `ReactionAllowsBasicAttack=false` and `RecoveryMultiplierQ1000=1000`; dead surveyor snapshots publish `ForcedDescent=false` and `CanFire=false`. Raw Heavy and end ticks follow the lifecycle rules above. Reset publishes the live baseline described under lifecycle.

### Internal ABI sentinels and discriminants

The top-level input is a discriminated union with exactly `Gameplay` and `EncounterReset` kinds; it is not a bag of optional flags. All structs reject unknown enum values and invalid default combinations before session mutation.

| Input form | Required fields | Forbidden companion fields |
|---|---|---|
| `Gameplay` | exact `Tick`, literal target, `finalAliveAfterM1`, optional transfer edge, walker event discriminant | reset payload |
| `EncounterReset` | exact `Tick=r`, no additional payload | final-alive gameplay, transfer edge, walker event |
| Transfer edge | `Baseline\|Heavy`, strictly increasing revision in `1..int.MaxValue`, exact target/tick | revision `0`, `-1`, or any negative value |
| Walker `None` | no cycle/request ID | any companion ID |
| Walker `AttackRecoveryStarted` | nonnegative encounter-unique cycle ID | request ID |
| Walker `HeavyImpactApplied` | exact nonempty ordinal `DamageRequest.RequestId` string from an accepted `Applied` result | cycle ID |

`LastTransferRevision=-1` means no transfer transition has been seen in this encounter. `ShieldWindowEndTick=-1` and `FireLockEndTick=-1` mean inactive/unset. An event at `t` for `N` ticks proposes `checked(t+N)`; a window is active exactly when alive and `currentTick<EndTick`, covering `t..t+N-1` and not `t+N`. Overlap commits `max(currentEnd, proposedEnd)`.

Expired raw end ticks remain at their historical exclusive value for replay diagnosis until encounter reset; only the active derivation changes. The constructor publishes no synthetic snapshot. `LatestSnapshot` is absent until the first successful gameplay tick; a first-input failure preserves that absence, and every later failure preserves the complete last successful snapshot.

Constructors defensively validate every payload before a session sees it. Inputs do not retain mutable collections.

## Processing and atomicity

The session performs, in order:

1. exact tick, target, alive-state and payload validation;
2. checked next-tick calculation and checked horizon calculation only for windows actually proposed by a valid live input;
3. duplicate/revision preflight without mutating dedupe sets;
4. local staged state calculation;
5. one snapshot publication and next-tick commit.

Any exception leaves the previous snapshot, transfer revision, consumed heavy-impact IDs, window ends and next expected tick unchanged. A malformed input is caller-owned and may not be partially consumed by the session.

## Allowed files

- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyReactionSession.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/RegularEnemyReactionSessionTests.cs`
- required `.meta` files and narrowly required internal friend declarations
- this contract, pre-gate and verification evidence

## Forbidden scope

- changes to public `DamageRequest`, `DamageResult`, `AimSample`, `BasicAttackPressed` or transfer ABI
- changes to M1 damage acceptance, health, invulnerability, dedupe or M2A cooldown/selection semantics
- Unity components, Rigidbody2D, collider queries, navigation, movement, projectiles, animation, VFX, UI or audio
- Ordan, boss payloads, box impact sampling, room/reward/run completion, ChoiceSkill, persistence
- scenes, prefabs, packages, project settings and external assets

## Required evidence

- Reflection or source-surface test proves no new public type.
- Exact-tick and target mismatch, transfer revisions `0/-1`, duplicate/older/no-op revisions, accepted gaps and `int.MaxValue`, malformed event/request ID and checked overflow preserve complete prior state.
- Walker natural recovery is active for exactly 30 ticks; heavy impact is active for exactly 90 ticks; overlapping windows never shorten.
- Walker recovery projection is exactly `1500` while alive and Heavy and otherwise `1000`; VD-02 target modifiers are not duplicated, and Heavy alone does not open the shield.
- Duplicate heavy-impact request IDs and attack-cycle IDs and malformed discriminated-event payloads are mutation-safe. M3A proves only that its discriminated event is singular; the actual ordinal-first heavy-impact producer arbitration is deferred to and must be verified by M3B without changing M1 semantics.
- Surveyor heavy edge forces descent and locks fire for exactly 75 ticks; baseline recall clears descent but not residual lock; steady heavy does not refresh.
- Death requires walker event `None`, suppresses directives while accepting the revisioned transfer mirror/clear, and performs no reaction-window arithmetic; reset consumes one global tick and publishes the specified live baseline.
- The same tick script grouped under 30/60/144 render frames exact-matches every snapshot scalar and next expected tick.
- Final regression includes Combat EditMode and full project EditMode.

## Ollama utilization requirements

- Kimi K3: one bounded engine-free state-machine or boundary-test proposal; Terra must screen and rewrite it before any integration.
- GLM 5.2: pre-implementation failure-mode pass and post-implementation scenario-gap sweep.
- MiniMax M3: deterministic fixture/mutation-matrix proposal; Terra may integrate only contract-conformant parts.
- Each lane records `used and accepted`, `used and rejected`, `failed and replaced` or concrete `not applicable` in the verification evidence.

## Stop conditions

Stop and report to Sol if M3A appears to require a new public type, M1/M2A semantic change, Unity state, a second health or transfer authority, dynamic discovery, wall-clock/frame time, runtime RNG, box collision sampling, actual AI movement/attack production, or any project-setting/package change. A later `-180` reaction/AI order must be added to `SYSTEM-CONTRACTS` by its own approved Unity adapter contract; M3A does not silently reserve that phase.

## Approval gate

Implementation may begin only after Luna independently reports no P0/P1 contradiction and Sol changes this contract to `Approved`. M3A verification is partial substrate evidence; it does not by itself satisfy complete enemy distinction, combat-room completion or boss acceptance criteria.
