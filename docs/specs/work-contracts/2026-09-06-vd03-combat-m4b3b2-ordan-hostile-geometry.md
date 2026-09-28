# Work Contract: VD-03 M4B3B2 Ordan Hostile Geometry and Next-tick Player Damage

- Status: Verified
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Date: 2026-09-06
- Parent: [VD-03 Combat and Enemies](../vertical-demo/03-combat-and-enemies.md)
- Predecessor: [M4B3B1 Audit Forecast and Exposure](./2026-09-06-vd03-combat-m4b3b1-ordan-audit-exposure.md)

## Purpose and split

M4B3B2 turns the three existing Ordan attack families into deterministic hostile contact observations and exact `t+1` player `DamageRequest` values. Combat remains the only health, dedupe, ordering and death owner. This unit does not implement BalanceAudit pull movement, presentation/VFX/audio, reward or room consumption, encounter teardown, scene transition, run persistence or input authority.

## Sol decisions frozen for review

### Phase order and ownership

- Add exactly one `OrdanBossHostileDamageProducer` at execution order `-170`. The boss order becomes `-200 Transfer -> -190 Combat -> -185 bridge -> -180 exposure scheduler -> -170 hostile producer -> default player movement -> +110 handoff`.
- `-170` never calls M1, creates a `DamageResult`, mutates health, or rolls back an already completed phase. A successful source tick `t` may only append immutable player-damage requests for Combat tick `t+1`.
- The producer consumes the exact current bridge, Combat, Transfer and exposure publications. Missing, stale, skipped, cross-wired or terminal input rejects before queue or latest-output mutation.
- A boss or player reported dead by current `-190` suppresses every new hostile request. A current Transfer lifecycle publication also suppresses new `-170` requests. A request queued at `t-1` and already processed by Combat at `t` is never rolled back; M4B3C owns later graph teardown and lifecycle consumption.

### Pure deterministic geometry boundary

- M4B3B2 performs no `Physics2D.SyncTransforms`, Cast, Raycast, Overlap, collision/trigger callback, temporary Transform movement or Rigidbody movement. For source ticks after bootstrap it uses the completed player movement snapshot whose tick is exactly `t-1`; at first source tick `0` it uses only the movement owner's seeded pre-simulation snapshot whose compatibility tick is `0`. Immutable authored Q4096 geometry is captured before the current movement tick.
- All contact and occlusion calculations are engine-free checked integer math. Inclusive/exclusive edge rules, stable authored occluder order, overflow and saturation are fail-closed and will be frozen in the Approved revision.
- This pre-movement boundary is intentional: render cadence and current tick movement cannot change the hostile result. The later `t` player movement cannot retroactively alter a request produced from the prior completed pose. Any other snapshot horizon rejects before producer mutation.
- The authored graph owns explicit immutable attack geometry descriptors. Unity phase, phase age, duration, renderer state, collider enabled state and visual pose are never used to reconstruct an attack.

### Exact authored Q4096 geometry

- Authoring converts decimal world values with `round(value * 4096, MidpointRounding.AwayFromZero)` and stores integers. Runtime geometry never rereads a `Transform`, `Collider2D`, layer or renderer.
- The player hostile hurtbox is a closed AABB centered on the completed `PlayerMotionSnapshot` position, with offset `(0,0)` and half extents `(1639,3277)`. The X half extent deliberately uses the enclosing ceiling of the authored `0.8`-wide capsule; hostile geometry must not under-approximate the physical player.
- The frozen arena frame is `x=[-47104,47104]`, `y=[0,40960]`. Ordan's authored center is `(28672,4915)`, its box half extents are `(3277,4915)`, and its left emission point is `(25395,4915)`.
- `DebtLineA` is the closed horizontal strip `x=[-45056,25395]`, `y=[3891,5939]`. `DebtLineB` is the closed elevated strip `x=[-45056,25395]`, `y=[11264,13312]`. Only the matching M4A `DebtLineIntent` at Execute age zero activates either strip.
- A Seizure projectile is a closed AABB with half extents `(1024,1024)`. Its start point is selected solely by payload ID: `BossWeight-0=(12288,24576)`, `BossWeight-1=(20480,24576)`, `BossWeight-2=(28672,24576)`. The normal path has three points `{start,(8192,12288),(-28672,4915)}`; the accelerated path has two points `{start,(-28672,4915)}`.
- Valid frozen Seizure durations are normal `{72,66,60}` and accelerated `{24,22,20}` for stages 1/2/3. Normal uses pivot `(duration-1)/2`, so segment lengths are `pivot` and `duration-1-pivot`; accelerated uses one segment of `duration-1`. Each coordinate is `from + DivRoundNearestAway((to-from)*localElapsed, segmentLength)` with checked `Int64` intermediates and checked `Int32` results. Contact uses the closed swept AABB between the preceding and current projected point; elapsed zero uses the start AABB only.
- The uninterrupted Audit shockwave is the closed annulus centered at `(28672,4915)`, with inner radius `49152` and outer radius `65536`. A player AABB contacts it iff its squared minimum distance to the center is `<= outer^2` and its squared maximum distance is `>= inner^2`.
- Stable authored occluders are the three Environment AABBs in literal order: `Ground center=(0,-2048), half=(49152,2048)`; `LeftBound center=(-49152,18432), half=(2048,22528)`; `RightBound center=(49152,18432), half=(2048,22528)`. Payload, audit and Ordan bodies are never hostile-geometry occluders.
- Primitive contact is inclusive. Debt and Seizure occlusion uses a rational slab interval from the attack's source point to first player contact; Audit uses the deterministic nearest point of the player AABB. An occluder blocks when its entry parameter is less than or equal to the first player-contact entry, so an equal-edge tie favors the occluder; occluder ties use the literal descriptor order above. Denominators are normalized positive and checked integer cross-products are used. Invalid bounds, unsupported duration, division failure or arithmetic overflow reject atomically before queue or latest-output mutation.

### M4A projections and attack mapping

- Existing `DebtLineIntent` and uninterrupted `BalanceAuditShockwaveIntent` remain the sole one-shot timing authorities. Each carries or maps to an exact authored geometry key without Unity inferring phase data.
- Add an immutable M4A `SeizureTrajectoryProjection` during every active Seizure Execute tick. It carries source tick, stable payload ID, attack ordinal, exact elapsed tick, exact frozen duration and accelerated bit. Empty outside that Execute and after defeat. Output equality and preview/commit validation include it.
- `OrdanBossTickOutput` exposes it as nullable `SeizureTrajectoryProjection`; both compatibility constructors supply `null`, while the new full constructor validates that presence exactly matches an active Seizure Execute snapshot. `Equals` includes nullable value equality. `OrdanBossBridgeOutcome` copies the same nullable value into an explicit read-only field and rejects disagreement with `Output`.
- The producer reads only `OrdanBossSimulationDriver.LatestOutcome.SeizureTrajectoryProjection` and the same outcome's intents/snapshot. Source tick, attack ordinal, payload ID, elapsed, duration, acceleration and pattern must cross-check before geometry. No Unity-side phase reconstruction or alternate projection seam is permitted.
- Normal and accelerated Seizure paths use separate authored trajectory tables. The producer indexes only by M4A's exact elapsed/duration projection and never recalculates stage timing.
- One attack instance may queue at most one player request. Contact after its first accepted request remains observable if needed but cannot enqueue again.

### Damage delivery

- Reuse `DamageKind.BossPattern`. Every produced request has `sourceId=ordan`, `targetId=player`, `amount=1`, `tick=t+1`, and a checked stable ID derived from the existing M4A attack/trajectory identity plus delivery tick.
- Extend the existing boss Combat delivery value additively: the verified optional Ordan payload self-damage request remains unchanged, while a defensively copied hostile-player request list is appended through one producer-private owner token and mutation-free preview/commit seam.
- `OrdanBossDeliveryBatch` contains exactly `SourceTick`, `DeliveryTick`, optional `PayloadRequest`, and a defensively copied hostile request list whose count is `0..1`. The `-185` bridge creates the unique `t+1` batch with its unchanged optional payload and an empty hostile list. The `-170` producer may replace that exact pending batch once with the same payload and either zero or one validated hostile request; it cannot create a second delivery tick or alter/remove the payload.
- Combat authoring binds exactly one producer identity. Combat creates a separate private hostile-append owner token during pristine boss-lane preparation; only the configured producer can obtain a preview candidate through `PreflightHostileDeliveryAppend` and commit it through `CommitPreflightedHostileDeliveryAppend`. A second producer, second append, forged token, changed payload/list, stale source tick or missing exact pending batch rejects without mutation.
- A read-only test seam returns a defensive pending-batch value for `AC-M4B3B2-003/005`; production consumers do not enumerate or mutate the queue. Combat's existing canonical request trace and aligned `DamageResult` list remain the only post-consumption evidence.
- Combat tick `t+1` merges payload, hostile-player, existing external and basic-attack requests, then M1 alone performs canonical ordinal sorting, dedupe, invulnerability, health and death.
- Duplicate attack instance, request ID, stale/wrong horizon, wrong source/target/kind/amount, foreign owner, second producer, partial list or overflow rejects the entire append while preserving the prior queue and publications.

### Stable request and latch identity

- Request IDs are invariant-culture ASCII and at most 96 characters:
  - `DebtLineA`: `ordan.hostile/debt-a/{attackOrdinal:D19}/{deliveryTick:D10}`
  - `DebtLineB`: `ordan.hostile/debt-b/{attackOrdinal:D19}/{deliveryTick:D10}`
  - Seizure: `ordan.hostile/seizure/{attackOrdinal:D19}/{payloadId}/{deliveryTick:D10}`
  - Audit: `ordan.hostile/audit/{attackOrdinal:D19}/{deliveryTick:D10}`
- The producer parses and cross-checks every ordinal, payload ID, source intent/projection and exact `sourceTick + 1` horizon before append. The request always uses `sourceId=ordan`, `targetId=player`, `amount=1` and `DamageKind.BossPattern`.
- One-hit latch keys are separate immutable values: `Debt(patternSlot,attackOrdinal)`, `Seizure(attackOrdinal,payloadId,frozenDuration,accelerated)`, and `Audit(attackOrdinal)`. A latch is marked only after the producer-private delivery append commits. It changes only when the exact source identity changes; later contact may remain observable but cannot enqueue again.

### Presentation handoff boundary

- The `+110` handoff receives only an immutable `OrdanBossHostileDamageDigest`, never full geometry or occlusion internals. It contains source tick, terminal flag, disposition, geometry key, attack ordinal, optional payload ID, optional queued request ID and optional delivery tick.
- Disposition is exactly one of `NoAttack`, `NoContact`, `Occluded`, `AlreadyLatched`, `Queued`, `SuppressedBossDead`, `SuppressedPlayerDead`, or `SuppressedLifecycle`. Optional strings are empty and optional delivery is absent unless applicable.
- The producer keeps the richer immutable per-tick geometry observation in its own latest outcome for focused verification. The digest is sufficient for presentation and 30/60/144 trace comparison without making presentation an attack-geometry consumer.
- Digest field combinations are exact:

| Disposition | terminal | geometry key / ordinal | payload ID | request ID / delivery |
|---|---:|---|---|---|
| `NoAttack` | false | empty / `-1` | empty | empty / absent |
| `NoContact`, `Occluded`, `AlreadyLatched` | false | non-empty / non-negative | exact payload only for Seizure, otherwise empty | empty / absent |
| `Queued` | false | non-empty / non-negative | exact payload only for Seizure, otherwise empty | non-empty / exact `sourceTick+1` |
| `SuppressedBossDead` | true | empty / `-1` | empty | empty / absent |
| `SuppressedPlayerDead`, `SuppressedLifecycle` | false | empty / `-1` | empty | empty / absent |

- `OrdanBossEncounterHandoffAdapter` has one serialized exact producer binding on the same Systems object. At `+110` it requires producer, bridge, Combat, Transfer and scheduler publications to share the current tick, copies the digest by value into `OrdanBossEncounterPresentationView`, and exposes no producer geometry-observation collection. Stale/cross-wired/partial or invalid digest input preserves the prior view and rejects.

### Authoring and validation ownership

- `OrdanBossEncounterAuthoringBuilder` is the sole writer of the exact descriptor values above, the one `OrdanBossHostileDamageProducer`, its player/Transfer/Combat/bridge/scheduler bindings, Combat's configured producer identity, and the handoff's producer binding. The producer lives on the existing `Systems` object; no hierarchy child, layer, scene setting or project setting is added.
- `OrdanBossEncounterAuthoringValidator` read-only validates exactly one producer, `[DefaultExecutionOrder(-170)]`, every binding identity, the complete integer descriptor, exactly three occluders in `Ground,LeftBound,RightBound` order, exact Seizure point counts `3/2`, and the expanded Systems component set. Any additional/missing descriptor, occluder, producer or owner rejects.
- Audit shockwave uses only its annulus contact rule and is not occlusion-tested. Debt and Seizure alone use source-to-first-contact occlusion. The production boundary occluders cannot block an in-bounds authored attack; their inclusive behavior is verified through isolated engine-free synthetic cases.
- Focused test seams may construct immutable descriptors and call producer preview/commit/advance methods directly, submit existing tick-indexed movement/Transfer/Combat inputs, and read defensive producer outcomes, pending delivery values and handoff views. They do not bypass M1, mutate production publications or add a second runtime clock. The fresh-scene cadence harness uses only those existing input seams and completed read models.

### Terminal and lifecycle boundary

- First Ordan defeat terminal-latches the producer after publishing an empty terminal result; later advances reject and preserve M4B3B1's last terminal handoff view.
- Player death publishes empty hostile output and accepts no new player request. It does not consume boss reward/room handoffs.
- Current Transfer lifecycle suppresses new geometry/request production. It does not rewrite a request already consumed by the earlier same-tick Combat phase, does not touch M4B3B1's four-ID removal input, and does not claim teardown.

## Requirements and acceptance evidence

- `REQ-COM-002`, `REQ-COM-003`, `REQ-COM-004`; affected `REQ-WT-003`, `REQ-WT-005`.
- `AC-M4B3B2-001`: focused EditMode proves every exact descriptor value, enclosing player hurtbox, inclusive contact, equal-edge occluder precedence, literal occluder order, the exact tick-0 seeded bootstrap/prior-tick snapshot rule and atomic fail-closed overflow/invalid-input behavior.
- `AC-M4B3B2-002`: focused EditMode proves immutable M4A Seizure projection presence only during Execute, exact ordinal/payload/elapsed/duration/acceleration identity, all six valid durations, table endpoints and forged candidate rejection.
- `AC-M4B3B2-003`: focused EditMode proves all four exact request formats, one-tick horizon, request-field validation, per-attack latch behavior, defensive copies, second-owner/foreign-owner rejection and all-or-nothing queue append.
- `AC-M4B3B2-004`: focused PlayMode proves DebtLine A/B, normal and accelerated Seizure contact, uninterrupted shockwave, final-tick audit interrupt suppression, and zero hostile request when the authored player path does not contact. Synthetic closed-boundary occlusion cases remain in `AC-M4B3B2-001` because the three production room-boundary occluders do not lie between an in-bounds player and the authored attacks.
- `AC-M4B3B2-005`: focused PlayMode proves source tick `t` only queues, Combat tick `t+1` alone applies/dedupes/invulnerability-gates damage, and an already processed request is never rolled back.
- `AC-M4B3B2-006`: focused PlayMode proves boss/player death, Transfer lifecycle suppression, producer terminal latch, and complete independence from M4B3B1's exact four-ID removal batch.
- `AC-M4B3B2-007`: focused handoff tests prove the digest enum/optional fields, defensive immutability, and absence of presentation access to full geometry observations.
- `AC-M4B3B2-008`: fresh authored 30/60/144 traces compare geometry observations, queued requests, Combat results, boss/player health, both M4A forecasts, both exposure outcomes, lifecycle/terminal state and stable IDs at matching simulation ticks.

## Resolved decisions

- `OD-M4B3B2-001`: resolved by the exact authored Q4096 descriptor, inclusive boundary, stable occluder and fail-closed rules above.
- `OD-M4B3B2-002`: resolved by the four request formats and independent per-attack latch keys above.
- `OD-M4B3B2-003`: resolved in favor of the minimal immutable handoff digest; full geometry remains producer-private verification data.

## Implementation allowlist

- `Assets/AcadeGameMaker/Runtime/Combat/OrdanBossSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossCombatSimulationDriver.cs`
- new `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossHostileDamageProducer.cs` and `.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossEncounterHandoffAdapter.cs`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringBuilder.cs`
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringValidator.cs`
- generated `Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab` and `Assets/Scenes/OrdanBossEncounterSandbox.unity`
- focused M4A, Combat bridge, hostile geometry/producer, authored graph and handoff EditMode/PlayMode tests under the existing test assemblies
- this contract, its pre-gate/implementation evidence, `docs/README.md`, and `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md`

## Ollama utilization record

| Lane | Outcome | Sol screening |
|---|---|---|
| GLM 5.2 | used and accepted in part | Accepted owner-order, quantization boundary, occlusion drift, duplicate identity and terminal/lifecycle failure modes. Host-engine ID recycling language was rejected as outside the authored-ID contract. |
| Kimi K3 | used and rejected | It proposed generic integer geometry but invented a 1/16 grid, four-substep sweep and fractional interpolation. No code, constants or schema were adopted. |
| MiniMax M3 | used and accepted in part | Retained table-driven fixtures, first-divergent-tick logging and explicit oracle outputs. Rejected PRNG seed, wall-clock mapping, visual/audio fields and invented request origins. |

All cloud prompts were abstract and non-sensitive. Terra or Luna must screen any later code-bearing output before Sol integration.

## Stop conditions

Stop and return to Sol if implementation needs a second Combat owner, same-tick health mutation or rollback, Unity phase/duration reconstruction, additional physics synchronization/query/callback authority, direct movement, a new public `DamageKind`, changes to M1 ordering/dedupe, modification of M4B3B1's four-ID removal semantics, reward/room/run/input/scene consumption, teardown, project settings or final art/audio.
