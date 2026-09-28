# Work Contract: VD-03 M3D1 Deterministic Regular-Enemy Threat Core

- Status: Approved — equal-X upstream compatibility addendum
- Compatibility addendum approved by Sol: 2026-08-31 after Luna pre-gate PASS (`P0=0`, `P1=0`, `P2=0`)
- Owning spec: `VD-03`, Approved 2026-08-25
- Contract owner, public ABI and integration decisions: Sol
- Unit design and implementation after approval: Terra
- Independent contract and implementation verification: Luna
- Requirement IDs: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`
- Rollback point: commit `006e71d`

## Purpose and milestone split

M3D1 adds an engine-free deterministic 60 Hz owner for regular-enemy threat generation. It consumes exact value snapshots after a logical source tick `t`, detects walker swept contact, owns the surveyor denial-line lifecycle, and previews immutable `DamageRequest` values addressed to Combat tick `t+1`.

M3D1 does not enqueue into Combat, sample Unity, move bodies, query physics, render a line or mutate health. Later contracts are fixed as:

1. M3D2A adds a Combat-internal future threat-batch lane and exact `t+1` merge.
2. M3D2B runs after default-order player movement at `+100`, stages M3D1, preflights M3D2A, commits both and publishes a carried threat view.

This split forbids same-tick retrocausality. Combat `-190` at tick `t` is already complete before threats from completed motion `t` are produced; their exact delivery tick is `t+1`.

## Sol-owned threat decisions

- Walker contact deals `1` in every alive gameplay phase. Guard, Heavy and behavior phase do not disable body contact.
- Surveyor fire creates a vertical denial strip centered on its latched target X. Its half-width is `1024 Q4096` (`0.25u`), so total width is `0.5u`; vertical extent is logically unbounded.
- A line is active on its spawn source tick and for exactly 30 source ticks: `[spawnTick, spawnTick+30)`. It may issue at most one request per active source tick when touched.
- A line emitted before Surveyor suppression, Heavy or death remains immutable and active until expiry. Only encounter reset removes it early.
- Player and walker contact uses continuous swept axis-aligned boxes in both axes. Boundary touch counts as contact. The surveyor strip uses the player's swept horizontal box interval; boundary touch counts.
- Player M1 invulnerability remains the sole repeat-damage gate. M3D1 emits a distinct request on every touched source tick and never predicts `Applied` versus `Invulnerable`.
- `DamageKind.EnemyRanged=6` is an approved public ABI extension. Walker uses `EnemyContact`; the Surveyor line uses `EnemyRanged`.

## Public ABI extension

`DamageContracts.cs` adds only `EnemyRanged=6` to the existing `DamageKind` enum. Constructor validation, request sorting, result semantics and all existing numeric values remain unchanged. No other public type or member is added.

## Frozen construction

`RegularEnemyThreatSession` is internal and constructed with:

- exact nonnegative first expected `SimulationTick` no later than `int.MaxValue-2`;
- player hurtbox half-width and half-height in positive signed Q4096;
- walker hurtbox half-width and half-height in positive signed Q4096;
- denial-line half-width exactly `1024 Q4096`.

Half extents must be positive and their pairwise sums must fit positive signed 64-bit values. The session stores values only; no Transform, collider, mutable authoring object or caller collection is retained.

## Exact input and validation

Every input is either `Gameplay` or `EncounterReset` at the exact next expected global tick `t`.

Gameplay contains:

- exact `SimulationTick t`;
- final player alive state and current player center Q4096;
- the exact M3B2A pair snapshot for `t`;
- the exact M3C1 motion pair snapshot for `t`.

The behavior and motion snapshots must both have `Tick=t`, `NextExpectedTick=t+1`, literal walker/surveyor ordering and equal alive/dead state per role. Motion dead flags must be the inverse of behavior `IsAlive`. The optional `SurveyorShotIntent` is accepted only as the exact value already carried inside the behavior pair.

On every Gameplay input, the behavior snapshot's `Surveyor.ShotOrdinal` must equal the session's expected shot ordinal when no intent exists. An intent exists if and only if the Surveyor phase is alive `Fire`; then the intent target is literal `surveyor`, its ordinal equals the pre-step expected ordinal, the Fire snapshot has `HasLatchedTarget=true`, its `LatchedTargetXQ1000` equals the intent target X, and its published `ShotOrdinal` equals `checked(expectedOrdinal+1)`. No other phase may carry an intent. On accepted spawn, M3D1 adopts that already-published `expectedOrdinal+1`; stale, duplicate, skipped, overflowed or structurally inconsistent publications reject atomically.

Before mutation, the core validates enum ranges, all literal IDs, phase-derived Reset/death shapes, Q1000→Q4096 conversion, request strings, every reachable line deadline and both `t+1` and delivery processing horizons. A source tick must be at most `int.MaxValue-2`; this lets Combat process delivery `t+1` at most `int.MaxValue-1`. A shot additionally requires `checked(t+30)`.

The first successful gameplay tick seeds both prior centers from the same current centers, so it performs a static overlap rather than inventing a pre-encounter sweep. The first successful gameplay after reset uses the same rule.

## Swept contact math

Walker contact treats the player and walker as axis-aligned boxes with frozen half extents. On later gameplay ticks it intersects the relative center segment

`(playerPrevious-walkerPrevious) → (playerCurrent-walkerCurrent)`

against the closed expanded box `[-sumHalfX,+sumHalfX] × [-sumHalfY,+sumHalfY]`.

All subtraction is widened to signed 64-bit before evaluation. Slab-entry/exit fraction comparison uses exact signed rational arithmetic with `System.Numerics.BigInteger` cross-products; float, double, decimal, division rounding and narrowed intermediate multiplication are forbidden. Parallel-axis inside/outside handling is explicit. Intersection over closed normalized time `[0,1]` is a hit.

The line test constructs the player's closed swept horizontal interval from prior/current center plus frozen player half-width. It intersects the closed strip `[lineCenterX-1024,lineCenterX+1024]` with widened signed arithmetic. Y never participates.

## Surveyor line lifecycle and ordinals

- The expected shot ordinal begins at `0` and is encounter-local.
- A shot intent must have target `surveyor`, exact expected nonnegative ordinal and a target X convertible from signed Q1000 to signed Q4096 by mathematical AwayFromZero rounding of `value×4096/1000`.
- The source behavior must satisfy the exact alive `Fire`/latched-target/intent/snapshot-ordinal derivation above. Missing, duplicate, stale, skipped or `long.MaxValue` ordinals reject atomically.
- A new shot while a prior line is still active rejects. The verified upstream cycle normally prevents overlap, but M3D1 does not rely on that assumption.
- On accepted spawn, the line becomes active before this tick's line-touch test, freezes center/ordinal/spawn/end, and checked-increments the expected ordinal in the same candidate.
- At the start of each later gameplay tick, a line with `t >= exclusiveEndTick` expires before touch evaluation.
- Shooter Heavy, suppression or death prevents only new intents; it never removes an already active line.

## Damage requests and ordering

Each successful preview contains zero to two immutable requests, ordered walker then Surveyor line:

- walker: `requestId="enemy.contact/walker/{sourceTick:D10}"`, `sourceId="walker"`, `targetId="player"`, amount `1`, kind `EnemyContact`, tick `t+1`;
- line: `requestId="enemy.line/surveyor/{shotOrdinal:D19}/{sourceTick:D10}"`, `sourceId="surveyor"`, `targetId="player"`, amount `1`, kind `EnemyRanged`, tick `t+1`.

The fixed order is publication order only. M1 remains the sole canonical sorter, dedupe, health, invulnerability and death authority. Request IDs remain within the existing ASCII/length contract. Player-dead gameplay emits no request.

## Death, reset and atomicity

- Final player-dead, walker-dead and surveyor-dead inputs latch independently. Alive gameplay for a latched identity rejects until reset.
- Walker death suppresses contact on that exact source tick. Surveyor death suppresses a new shot but retains an older active line. Player death suppresses both request types while the line clock may still expire normally.
- Repeated player-dead ticks advance exactly and emit no requests. Repeated walker-dead ticks suppress walker contact only. Repeated surveyor-dead ticks suppress new shot intents only; an already active line still performs its normal touch test and may emit one line request per active source tick until expiry. Every dead latch remains set until reset.
- Reset is accepted only after at least one successful gameplay tick, at the exact expected tick, with exact paired M3B2A/M3C1 Reset snapshots, alive player and no shot intent. It clears all dead latches, active line, expected ordinal and prior sweep history and stores no center seed. The next successful Gameplay input sets the previous player and walker centers equal to that input's current centers, so it performs only a static overlap and cannot create a cross-encounter sweep. Reset emits no request.
- `PreviewNext` is mutation-free and returns an immutable complete candidate. `CommitNext` recomputes and compares every field before atomically replacing session state. Any malformed second role, line, request or horizon preserves prior centers, latches, ordinal, active line, latest snapshot and next expected tick.
- Caller arrays and lists are copied; no mutable request collection escapes.

## Allowed files

- `Assets/AcadeGameMaker/Runtime/Combat/DamageContracts.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/RegularEnemyThreatSession.cs` and `.meta`
- narrowly required additions to `Assets/AcadeGameMaker/Tests/EditMode/Combat/DamageContractsTests.cs` or the existing exact damage-contract test file
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/RegularEnemyThreatSessionTests.cs` and `.meta`
- this contract, pre-gate, implementation evidence and documentation index

No assembly-definition change is authorized unless Luna first confirms `System.Numerics.BigInteger` cannot compile under the existing Combat assembly; that condition returns to Sol rather than broadening scope.

## Equal-X upstream compatibility addendum

Executable M3D2 integration exposed one over-narrow consumer check. Approved M3B2A owns walker direction as exactly `-1`, `0` or `1`, chooses `0` when player and walker X are equal, and may enter `DashTelegraph` whenever LOS is open and absolute X distance is at most 4u. Approved M3C1 carries that same `-1/0/1` domain through `DashTelegraph` and `DashActive`. M3D1 must accept the complete verified upstream domain rather than redefine it.

- `DashTelegraph` retains every existing phase/end/movement/direction/telegraph check but validates `LatchedDashDirection` with `IsDirection`, allowing exactly `-1/0/1`.
- `DashActive` retains every existing phase/end/movement/telegraph check and exact `Direction == LatchedDashDirection`, but validates `Direction` with `IsDirection`, allowing exactly `-1/0/1`.
- `Approach`, `Recovery`, death, reset, recovery intent, Surveyor, line and request semantics are unchanged.
- Direction `-2`, `2` and every other out-of-domain value still reject atomically.
- A zero-direction telegraph/active snapshot changes no threat geometry or request rule: contact remains solely the existing static/swept player–walker AABB result.

Required compatibility evidence adds exact aligned-X M3B telegraph→active publications accepted through M3D1, zero-direction static overlap/non-overlap outcomes, out-of-domain `±2` rejection with full state preservation, the existing focused M3D1 suite, and full EditMode regression.

## Forbidden scope

- Combat queue/driver, Unity adapter, component, physics query, callback, sync or body movement
- M1 sorting/dedupe/health/invulnerability/result changes beyond recognizing the new valid enum member
- M3A/M3B/M3C, Movement or Transfer behavior changes
- line rendering, collision blocking, knockback, animation, VFX, audio or UI
- public schema/type/member other than `DamageKind.EnemyRanged=6`
- runtime discovery, object/instance identity, frame time, wall clock, runtime RNG or mutable publication
- scene, prefab, asset, package, project setting, boss, reward, room, run or persistence change

## Required evidence

- public enum values remain stable; `EnemyRanged=6` is preserved by `DamageRequest.Kind`, accepted by validation, participates in unchanged canonical sorting, and produces normal echoed `DamageResult` behavior;
- first-tick static overlap, exact boundary touch, one-Q4096 miss, player-only/walker-only/opposed crossing, diagonal miss, parallel slab and extreme-coordinate rational cases;
- every alive walker phase threatens while Dead/Reset does not, and one contact request is emitted per touched source tick;
- line spawn-tick hit, tick 29 hit, tick 30 expiry, one-Q4096 strip miss, swept crossing and boundary touch;
- exact Fire snapshot ordinal `1` with intent ordinal `0`, non-Fire intent, Fire-without-intent, target-X mismatch, duplicate/stale/skipped/max ordinal, active-line overlap and Q1000 conversion/overflow mutations preserve all state;
- shooter suppression/Heavy/death retains an existing line but emits no new shot; an existing line touched after Surveyor death still emits through tick 29 and expires before touch at tick 30; walker/player death, repeated death and resurrection rejection follow the latches;
- reset clears line/latches/ordinal/history, emits nothing and permits post-reset ordinal `0` with no cross-encounter sweep;
- simultaneous contact/line requests have exact IDs, kinds, delivery tick and fixed publication order;
- source `int.MaxValue-2` may produce delivery `int.MaxValue-1`; source `int.MaxValue-1`, line deadline overflow and candidate/request mutations reject atomically;
- mutation-free preview, forged candidate and malformed second-role cases preserve every field;
- identical logical traces under 30/60/144 render grouping;
- source scan proves no Unity, public surface beyond the enum member, floating-point/decimal simulation, time/RNG, discovery or mutable collection escape;
- focused Threat EditMode, full Combat EditMode and full project EditMode regressions pass.

## Ollama utilization record for contract formation

| Lane | Outcome | Sol screening |
|---|---|---|
| GLM 5.2 | used and accepted in part | Accepted shot spawn/death ambiguity, one-tick delivery latency, invulnerability boundary, simultaneous-threat and reset-leak mutation classes. Rejected its implication that M3D owns invulnerability decrement or player-death movement; M1 and Movement retain those authorities. |
| Kimi K3 | used and accepted in part | Accepted bounded immutable outputs, preflight/commit separation, stable threat identity and explicit line lifetime. Rejected unsigned ticks, numeric request IDs, X-only walker overlap, one-tick line lifetime and queue ownership because existing contracts require signed ticks, string IDs, 2D contact, a readable denial window and later Combat integration. |
| MiniMax M3 | failed and replaced | The non-thinking bounded fixture request returned empty text with `done=true`, `length` and 900 output tokens, without quota/rate error. Terra and Sol supplied the fixture matrix; no empty result was adopted. |

No cloud model received repository text, local paths, credentials, personal data or secrets. Ollama output has no approval or integration authority.

## Stop conditions and approval gate

Stop and report to Sol if implementation needs Unity, an assembly change, a different denial width/lifetime/orientation, projectile travel, more than one active line, a new public type, M1 semantic change, queue integration, physics/Transform truth, scene authoring or any forbidden system.

Compatibility implementation is authorized under the approved addendum. M3D1 is partial substrate and cannot alone close `AC-COM-001` or `AC-COM-003`.
