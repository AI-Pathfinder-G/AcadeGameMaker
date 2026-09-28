# Work Contract: VD-03 M1 Deterministic Damage Core

- Status: M1 Verified — Sol acceptance and Luna final PASS, 2026-08-27
- Owning spec and revision: `VD-03`, Approved 2026-08-25
- Assigned and contract-frozen by: Sol
- Implementer: Terra; Kimi K3 may draft repetitive validation and unit-test proposals only after this contract becomes Approved
- Independent verifier: Luna
- Requirement IDs: `REQ-COM-001`, `REQ-COM-004`, `REQ-COM-005`, `REQ-COM-006`
- Partial acceptance-criterion IDs: `AC-COM-003`; no claim yet for complete combat, enemy distinction, boss completion, or transfer advantage
- Rollback point: annotated tag `vd02-transfer-m2b2b-verified-v1`

## Purpose and boundary

M1 creates the engine-free owner of health, damage ordering, request deduplication, invulnerability and one-time death state. It does not create attacks, hit detection, AI, rewards, room completion, run failure, boss patterns, transfer consumers, Unity components or scenes.

This split deliberately leaves the following Sol decisions to later contracts: basic-attack projectile/hitscan and aim acquisition, combat FixedUpdate phase and producer merge, actual enemy behaviors, box-impact sampling, Ordan phase thresholds and vulnerability effect, boss/room completion events, ChoiceSkill stagger, and combat-to-transfer removal production.

## Allowed and forbidden files

- Allowed: `Assets/AcadeGameMaker/Runtime/Combat/**`, `Assets/AcadeGameMaker/Tests/EditMode/Combat/**`, their `.meta` files and assembly definitions; this work-contract file and its verification evidence.
- Forbidden: `Core/**`, `Movement/**`, `Transfer/**`, every Unity scene/prefab/editor/PlayMode path, `GameInput.inputactions`, `InputRouter`, project/package settings, UI, persistence, rooms, rewards, choice implementation and external assets.
- Public API changes are limited to the exact engine-free payloads frozen below. Any other public type or member requires Sol re-contracting.

## Public damage ABI

Public types live in `AcadeGameMaker.Combat` and contain no Unity or object references.

### IDs and scalar validation

- `requestId`, `sourceId` and `targetId` are nonempty ordinal ASCII strings of at most 96 characters. Allowed characters are `A-Z`, `a-z`, `0-9`, `.`, `_`, `:`, `/`, `-`.
- `amount` is an integer in `1..999`.
- Every enum constructor rejects unknown numeric values. Malformed payload construction throws before entering a session and is not represented by `Invalid`.
- `DamageResultCode.Invalid` means a structurally valid request addressed an unknown or encounter-reset target.

### Exact public types

- `DamageKind`: `BasicAttack=0`, `EnemyContact=1`, `BossPattern=2`, `HeavyImpact=3`, `ChoiceSkill=4`, `BossPayloadTransfer=5`.
- `DamageResultCode`: `Applied=0`, `Invulnerable=1`, `Duplicate=2`, `TargetDead=3`, `Invalid=4`.
- `DamageRequest(string requestId, string sourceId, string targetId, int amount, DamageKind kind, SimulationTick tick)` with same PascalCase properties.
- `DamageResult(string requestId, string targetId, int appliedAmount, DamageResultCode result, SimulationTick tick)` with same PascalCase properties.
- `DamageResult` requires `appliedAmount>0` only for `Applied`; every other result requires zero. `appliedAmount` may not exceed 999.
- Every result echoes the `RequestId`, `TargetId` and `Tick` of its own originating input request. This remains true for a later `Duplicate` whose payload reuses an ID with a different target; canonical ordering chooses which occurrence consumes the ID but never rewrites another request's echoed target.

No public health snapshot, attack command, death event, reward event or lifecycle payload is added in M1.

## Internal combatant and session contract

### Registration

- The internal session registers explicitly authored combatants by ordinal ID. Discovery order and Unity instance IDs are forbidden.
- A registration freezes `targetId`, `maxHealth` in `1..999`, and `invulnerabilityTicks` in `0..600`.
- Vertical-demo values pinned in tests are player `maxHealth=5`, `invulnerabilityTicks=45`; Collection Walker `9,0`; Floating Surveyor `6,0`; Ordan `60,0`.
- Duplicate target registration is a contract error. Registration order may not affect resolution.

### Tick processing and ordering

- The internal session constructor takes its first expected global `SimulationTick`. It must be nonnegative and at most `int.MaxValue-1`; vertical-demo M1 tests begin at tick 0. This permits a later room encounter to start on the unchanged global simulation clock without renumbering ticks.
- The session processes each tick exactly once in increasing order. A skipped, repeated, stale or negative tick is a contract error and causes no mutation.
- The caller submits a non-null batch whose every request has the exact processing tick. The session makes a defensive copy and performs every validation, deterministic sort-key construction and checked invulnerability-end calculation before mutating health, dedupe state or death state.
- Before any mutation it also computes `checked(processingTick.Value + 1)` as the next expected tick. Processing `int.MaxValue` therefore fails atomically; successful processing of `int.MaxValue-1` leaves `int.MaxValue` as an exhausted sentinel at which no further tick can commit.
- Requests sort by ordinal `RequestId`; ties sort by ordinal `SourceId`, ordinal `TargetId`, numeric `Kind`, then numeric `Amount`. No submission order participates.
- Results are returned in that canonical processing order and preserve one result for every submitted request. Each result echoes that sorted input occurrence's own request ID, target ID and tick.

### Deduplication and result precedence

- A structurally valid `RequestId` is consumed for the whole encounter by its first canonical occurrence, including `Invalid`, `Invulnerable` and `TargetDead` outcomes.
- A previously consumed ID, or every later same-batch occurrence of the same ID, resolves `Duplicate` with zero applied amount before target lookup, dead-state or invulnerability checks.
- For a first-seen ID, precedence is unknown target → `Invalid`; already dead target → `TargetDead`; active invulnerability → `Invulnerable`; otherwise `Applied`.
- Dedupe tombstones remain until explicit encounter reset. Wall clock, render frames and bounded eviction may not affect them.

### Health, invulnerability and death

- Applied damage is `min(request.Amount, currentHealth)` and cannot make health negative.
- If health remains above zero and `invulnerabilityTicks>0`, the target is invulnerable while `tick < checked(appliedTick + invulnerabilityTicks)`. Damage at age exactly 45 is therefore allowed for the player.
- A hit that reduces health to zero establishes dead state once. The killing request is `Applied`; later first-seen requests are `TargetDead`. M1 emits no reward or completion event.
- Zero-invulnerability enemies and boss may accept multiple distinct requests in one tick until the canonical request that kills them; later requests in that same sorted batch resolve `TargetDead`.
- Internal inspection may expose immutable health, dead, invulnerability-end and accepted-request-count snapshots to tests through `internal` members only.

### Encounter reset

- An explicit parameterless internal reset is allowed only after at least one completed processing tick. It clears registrations, health/death/invulnerability state and all dedupe tombstones while retaining the session's already-computed next expected global tick. Reset immediately after tick `t` therefore requires the first new-encounter batch to use exactly `checked(t+1)`; it cannot rewind, skip or renumber time.
- Reset itself performs no tick arithmetic. If the retained next expected value is the exhausted `int.MaxValue` sentinel, re-registration may occur but processing still fails atomically until a new session is constructed for a valid clock—which the vertical demo must not attempt because the authoritative clock cannot wrap.
- Reset performs no reward, room, run, transfer or persistence work. Re-registration is required before the next processed tick.

## Required evidence

- Constructor and enum validation for every public payload field.
- Registration-order independence and canonical same-tick ordering, including duplicate IDs with different payloads.
- Cross-tick duplicate results and reset clearing the dedupe lifetime.
- Unknown target `Invalid`, player invulnerability at ages 44/45, overkill clamp, and zero-invulnerability multi-hit.
- Killing request `Applied`, remaining same-tick requests `TargetDead`, and dead transition observed once internally.
- Wrong/skipped/repeated/negative tick, processing-tick increment overflow, and checked invulnerability-end overflow fail before any session mutation. Tests pin constructor start tick, reset retaining exact `t+1`, successful `int.MaxValue-1`, and atomic rejection at `int.MaxValue`.
- The same 60 Hz request script grouped as 30/60/144 render frames produces exact `DamageResult` and internal snapshot traces.
- All implementation source comments cite applicable `REQ-COM-*`; verification records cite only the partial `AC-COM-003` evidence actually established.

## Stop conditions

Stop and return to Sol if M1 needs a Unity reference, attack/hit contract, new public payload, existing assembly change, transfer mutation, reward/death event, target resurrection/healing, damage multiplier, stagger, vulnerability multiplier, package/project setting or any decision listed as deferred above.

## Approval gate

Luna independently reports no P0/P1 contradiction and Sol approves M1 implementation on 2026-08-27. M1 completion does not verify VD-03; it only establishes the damage/health substrate for later approved units.
