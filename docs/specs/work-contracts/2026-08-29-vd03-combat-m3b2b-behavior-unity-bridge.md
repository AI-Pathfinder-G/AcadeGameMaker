# Work Contract: VD-03 M3B2B Regular Enemy Behavior Unity Bridge

- Status: Approved — Sol, after Luna PASS, 2026-08-29
- Owning specs: `VD-03`, affected `VD-02`, Approved 2026-08-25
- Contract owner, cross-system decisions and final integration: Sol
- Unit design and implementation after approval: Terra
- Independent contract and implementation verification: Luna
- Requirement IDs: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`
- Rollback point: commit `9102bce`

## Purpose and milestone boundary

M3B2B connects the verified M3B2A behavior planner to the verified `-200 -> -190 -> -180` Unity pipeline. It samples exact Q1000 positions and authored LOS from the already current physics world, advances both behavior roles once at execution order `-170`, atomically carries the resulting pair view, and converts a walker recovery intent for tick `t+1` into M3B1's unique future queue before the global movement clock advances.

This unit publishes movement, telegraph and surveyor-shot intents as immutable data only. It does not move an enemy body, select speed, resolve collision or contact damage, create a `DamageRequest`, spawn or expire a denial line, animate, render, author scenes or alter project settings. Those choices require a later Approved contract because the owning specs do not yet fix their runtime values.

## Ownership and fixed phase

The fixed sequence is:

1. `TransferSimulationDriver` at `-200` performs the sole conditional `Physics2D.SyncTransforms` and publishes tick `t`.
2. `CombatSimulationDriver` at `-190` completes M2A/M1 for `t`.
3. `RegularEnemyReactionSimulationDriver` at `-180` publishes exact M3A walker/surveyor views for `t`.
4. New `RegularEnemyBehaviorSimulationDriver` at `-170` consumes those exact publications and publishes one M3B2A pair view for `t`.
5. Default-order player movement advances the shared clock afterward.

Unity phases are sequential on the main thread. M3B2B performs no `Physics2D.SyncTransforms`, does not advance the player clock, and rejects a missing, stale or future upstream publication. The behavior driver owns only its M3B2A session and carried behavior view. M1 retains health/death authority, M3A reaction authority, M3B1 recovery arbitration/queue authority, and VD-02 transfer authority.

## Frozen bindings and observation

All production references are explicit serialized bindings to the player controller, combat driver and reaction driver. The bridge obtains the already validated frozen combat roster only through a new read-only internal seam on the combat driver; it must not bind, rediscover or re-freeze a second roster. Dynamic discovery, tags, Unity instance IDs and object-creation order are forbidden. The roster must contain exactly one literal `walker` and one literal `surveyor` with the M3B1-approved combat identities; the bridge reads their frozen pose roots and local aim points without retaining mutable caller collections.

The combat driver must expose publication presence separately from its value so the default `CombatSimulationOutcome` at initial tick `0` can never masquerade as a successful M1 publication. The behavior bridge accepts an outcome only when that presence flag is true and its tick is exactly `t`. The reaction driver likewise exposes a production-named nullable/read-only carried-view seam rather than relying on a test-named accessor.

For tick `t`, before any behavior mutation, the bridge stages:

- player X/Y from the current immutable player snapshot, converted Q4096 to Q1000 with checked AwayFromZero rounding. At the first expected tick `t0`, the motor's authored initial snapshot has `Snapshot.Tick=t0` and is the explicit bootstrap spatial state; after one or more movement commits, tick `t` requires `Snapshot.Tick=t-1`. Any other snapshot horizon is rejected before mutation;
- walker and surveyor pose-root X/Y quantized to Q1000 with checked AwayFromZero rounding;
- each role's LOS from player position to its checked world aim point, using the existing authored layer-8 non-trigger `TransferLineOfSight` evaluator and explicit player/target roots;
- exact final-alive values from M1's already published `CombatSimulationOutcome(t)`;
- exact walker/surveyor `CarriedReactionView(t)` from M3B1;
- the encounter-reset discriminator from M1's exact outcome.

The LOS queries read the physics world current after the `-200` phase and never request another transform synchronization. They run in stable walker-then-surveyor order. Saturation, duplicate stable collider identity, invalid float, quantization/addition overflow, malformed roster or upstream disagreement fails before the M3B2A session, recovery queue or carried behavior view mutates.

## Two-owner atomic handoff

M3B2B must not first mutate M3B2A and then discover that M3B1 rejects the emitted recovery candidate. Narrow internal non-public seams are therefore permitted:

- M3B2A may expose a mutation-free preview that returns the exact candidate pair snapshot produced by its existing validate/process path.
- M3B1 may expose a mutation-free recovery preflight and a commit method whose rejection conditions are completely discharged by that preflight under the fixed sequential phase.

The bridge stages the complete observation, previews the pair result, validates the preview against the exact input, and preflights any recovery conversion. Only then does it process M3B2A once. The committed result must equal the preview field-for-field. If it carries a recovery intent, the bridge converts `(walker, attackCycleOrdinal, t+1)` to `AttackRecoveryStarted(attackCycleOrdinal, t+1)`, commits it exactly once, and only then replaces the carried behavior view. With no recovery intent, it directly replaces the carried view after the successful core step.

A duplicate, late, stale, tombstoned or non-`t+1` candidate fails in preflight. There is no retry, silent drop, rollback of an upstream publication or partial behavior publication. Since no external mutation occurs between successful preflight and commit on the main thread, the commit seam must not add a new ordinary rejection branch; invariant violation is fail-stop and no behavior view is published.

Surveyor shot intent remains part of the immutable carried behavior view identified by `(surveyor, shotOrdinal)`. M3B2B neither turns it into a `DamageRequest` nor erases it when later Heavy/fire-lock state changes.

## Lifecycle, death and ordering

- Final death from M1 at `t` reaches M3B2A in the same `-170` phase and suppresses new behavior intents as defined by M3B2A.
- A recovery candidate emitted at `t` for `t+1` remains M3B1-owned history; death or HeavyImpact at `t+1` may tombstone it under the verified M3B1 rules.
- Encounter reset uses the exact reset publications from M1/M3A at `t`; M3B2A reset clears its encounter-local ordinals. M3B1 reset has already cleared its queue/tombstones during the same upstream pipeline, so post-reset ordinal `0` is reusable.
- The bridge's first expected tick is captured from the player clock during initialization. It accepts exactly one step per global tick and carries only the latest immutable pair view.
- Replay of the same fixed-tick observation script must produce byte-equal pair views and recovery/shot intent sequences under 30/60/144 render grouping.

## Allowed files

- new `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyBehaviorSimulationDriver.cs` and `.meta`
- narrow internal-only seams in `RegularEnemyBehaviorSession.cs`, `RegularEnemyReactionSimulationDriver.cs` and `CombatSimulationDriver.cs` only as required by this contract
- Combat Unity EditMode/PlayMode tests and required `.meta` files
- `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md` only to append the approved `-170` phase
- this contract, its pre-gate and later implementation evidence

## Forbidden scope

- any public ABI, public serialized field or changed existing public-component schema
- enemy `Rigidbody2D` movement, speed/acceleration/gravity values, collision/contact resolution or knockback
- projectile/denial-line creation, lifetime, hit detection, damage amount or `DamageRequest`
- animation, VFX, UI, audio, scene/prefab/package/project-setting/asset changes
- a second health, reaction, transfer, recovery-queue or global-clock owner
- new physics synchronization, frame/wall time, runtime RNG, mutable shared collections or dynamic discovery
- modification of established M1, M2A, M3A, M3B1 arbitration or VD-02 semantics

## Required evidence

- execution order is exactly `-200 -> -190 -> -180 -> -170 -> default`, with no additional physics synchronization;
- explicit component bindings, combat-owned frozen read-only roster and exact literal roles reject missing, duplicate, reordered or malformed values before mutation; no second registry freeze occurs;
- the first-tick initial player snapshot convention and every later `t-1` completed snapshot horizon are accepted exactly; all other horizons fail before mutation;
- combat publication presence distinguishes missing initial-tick output from a valid tick-0 outcome, and the production reaction carried-view seam rejects missing/stale/future values;
- Q4096/Q1000 AwayFromZero boundaries, invalid float, checked pose/local-point overflow and long coordinate separation are atomic;
- LOS open/blocked, player/target self-exclusion, layer-8 filtering, stable-collider duplicate and saturation paths use the existing evaluator without a second sync;
- missing/stale/future M1 or M3A publications and final-alive disagreement preserve behavior session, recovery queue and carried behavior view;
- preview is mutation-free and committed pair output equals its preview on every phase boundary;
- final dash tick emits exactly one `t+1` recovery conversion, preflights before core commit, queues before movement clock advance, and duplicate/late/tombstoned candidates never partially publish;
- no-recovery and surveyor-shot steps publish exactly once; shot identity and latched point are immutable and create no damage;
- lethal tick, repeated dead tick, reset, post-reset ordinal reuse and `int.MaxValue`/`long.MaxValue` boundaries preserve owner invariants;
- deterministic traces match across 30/60/144 render grouping;
- source scan proves no public surface, dynamic discovery, body movement, damage/projectile production, time/RNG source or project-setting change;
- focused Combat Unity EditMode/PlayMode and full project EditMode regressions pass.

## Ollama utilization record for contract formation

| Lane | Outcome | Sol screening |
|---|---|---|
| Kimi K3 | used and accepted in part | Accepted explicit references, stage/validate/commit structure, stale/duplicate guards and boundary tests. Rejected its `-170`-before-combat order, mutable Transform use inside the pure planner, global tick reset, death-stops-clock behavior and publish-before-queue ordering. |
| GLM 5.2 | used and accepted in part | Accepted a single spatial baseline, LOS staleness tests, two-owner handoff atomicity, value-only publications, lifecycle contamination and Q1000 overflow risks. Rejected cross-owner rollback, lock-free/concurrency concerns and alleged movement-intent priority conflict because phases are sequential and `-180` publishes reaction rather than locomotion. |
| MiniMax M3 | failed and replaced | Two bounded non-sensitive fixture requests returned empty output without HTTP/quota error. Sol classifies this as a non-quota model-output failure and replaces the fixture proposal with GPT-authored preview/queue mutation and render-grouping cases; Luna must independently review them. |

No cloud model received repository text, paths, credentials, personal data or secrets. Ollama output has no approval or integration authority.

## Stop conditions

Stop and report to Sol if implementation requires actual enemy motion, speed/damage/line-lifetime values, a public API or schema, new physics synchronization, dynamic discovery, upstream arbitration changes, scene/prefab work or project settings. A failed preflight-to-commit equivalence proof also stops integration.

## Approval gate

Implementation may begin only after Luna independently reports no P0/P1 contradiction and Sol changes this contract to `Approved`. M3B2B is integration substrate and cannot by itself close full `AC-COM-001`, `AC-COM-003`, `AC-WT-002` or `AC-WT-005`.
