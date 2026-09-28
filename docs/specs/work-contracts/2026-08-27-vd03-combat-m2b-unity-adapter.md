# Work Contract: VD-03 M2B Unity Combat Adapter

- Status: Verified — Luna final PASS and Sol integration acceptance, 2026-08-28
- Owning specs: `VD-03`, `VD-07`, Approved 2026-08-25
- Contract owner and final integration: Sol
- Unit design and implementation: Terra; Kimi K3 may receive only a redacted abstract work specification and return isolated drafts
- Independent verifier: Luna
- QA supplement: GLM 5.2 may propose non-authoritative scenario gaps after this contract freezes
- Requirement IDs: `REQ-COM-001`, `REQ-COM-004`, `REQ-COM-005`, `REQ-COM-006`, `REQ-UX-007`, `REQ-UX-008`, `REQ-UX-013`, affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-006`
- Partial acceptance evidence only: `AC-COM-003`, `AC-UX-007`, `AC-UX-012`, affected `AC-WT-003`, `AC-WT-005`, `AC-WT-006`
- Rollback point: annotated tag `vd03-combat-m2a-verified-v1`

## Sol decisions

M2B connects the verified engine-free M2A attack session and M1 damage session to authored Unity combatants. The vertical-demo basic attack remains an instantaneous acquired shot; Unity supplies only exact-tick observations. No projectile, render object, animation event, overlap callback or frame time decides a hit.

The fixed phase is frozen as follows:

1. `TransferSimulationDriver`, `DefaultExecutionOrder(-200)`, remains the vertical-demo's sole aim-physics synchronization owner. When an exact aim/camera pair exists it performs exactly one `Physics2D.SyncTransforms`, records an immutable internal capture receipt, completes transfer, and reflects the final movement modifier.
2. `CombatSimulationDriver`, `DefaultExecutionOrder(-190)`, runs after transfer and before the default-order movement controller. For an aim tick it must verify the transfer capture receipt has the same processing tick, aim sample ID, sample tick and camera pose tick. It never calls `Physics2D.SyncTransforms`.
3. `CombatSimulationDriver` captures combat observations, processes M2A exactly once, merges its optional request with the exact-tick external request batch without sorting, then calls M1 exactly once. M1 remains the sole canonical sorter and health/death owner.
4. Movement then processes the same global tick at default execution order.

This narrow one-sync gateway is vertical-demo infrastructure, not a claim that VD-02 owns attack state. Combat may reuse verified `TransferFixedMath` and `TransferLineOfSight` through a one-way `Combat.Unity → Transfer.Unity` assembly reference and friend declaration, but it may not read `TransferSession`, target modifier state, transfer availability or transfer selection.

## Milestones

- **M2B1 — authoring and capture:** new combat authoring/registry, shared fixed spatial calculation reuse and engine-to-M2A observation translation. EditMode and isolated PlayMode evidence.
- **M2B2 — fixed-phase integration:** one-sync receipt, exact input buffer, M2A→M1 merge, encounter reset, death availability and next-tick transfer-removal handoff. PlayMode and full regression evidence.

Both milestones are governed by this contract. M2B1 may be integrated independently after its required tests pass; M2B2 is the M2B Verified gate.

## Allowed and forbidden files

Allowed:

- `Assets/AcadeGameMaker/Runtime/Combat/Unity/**`
- `Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/**`
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/**`
- required `.asmdef`, `.meta`, internal-friend declarations
- narrowly required `TransferSimulationDriver`, `Transfer.Unity/AssemblyInfo.cs`, one internal immutable authored-geometry accessor in `TransferTarget.cs`, and their existing tests for cross-validation, the capture receipt and next-tick removal seam
- this contract, system-contract clarification, documentation index and verification evidence

Forbidden:

- existing public `AimSample`, `BasicAttackPressed`, `DamageRequest` or `DamageResult` changes
- M1/M2A semantic changes, projectile/ballistics, InputRouter/device reads, AI/patterns, animation, VFX/UI/audio, rewards, room completion, run/persistence, ChoiceSkill, scenes/prefabs, packages, project settings and external assets
- new health, death, transfer or input authority outside the verified sessions

The only new public type is the exact Unity authoring component `CombatTarget`. Driver, registry, observations, receipts, lifecycle input and outcomes remain internal. No new public gameplay payload is authorized.

## CombatTarget authoring

`CombatTarget : MonoBehaviour` freezes these serialized fields:

- ordinal `TargetId` using the existing 1..96 ASCII ID grammar
- `MaxHealth` in `1..999` and `InvulnerabilityTicks` in `0..600`
- `CanReceivePlayerBasicAttack`
- local aim point X/Y Q1000
- local axis-aligned elliptical aim-shape center X/Y and positive half extents Q1000
- exact non-trigger target `Collider2D` and zero-Z-rotation, unit-XY-scale target pose root
- optional co-authored `TransferTarget` reference

The explicit internal `CombatTargetRegistry` is serialized, ordinal-sorted by target ID, contains no null, duplicate target ID or duplicate stable collider key, and never uses discovery order, `Find*`, Unity instance ID or registration order.

Initialization validates the full registry and freezes immutable `CombatantRegistration` and targeting descriptors before creating either session. Encounter reset reuses only those frozen copies; it never rereads mutable Unity authoring during a reset transaction.

Pinned vertical-demo registrations and literal target IDs are player `player` with `5/45/false`, Collection Walker `walker` with `9/0/true`, Floating Surveyor `surveyor` with `6/0/true`, and Ordan body `ordan` with `60/0/true`. These four IDs are reserved role bindings for M2B. The player is registered with M1 but never enters the attack candidate list. Ordan's body must not carry `TransferTarget`; only his scripted payload may be transferable.

When the optional `TransferTarget` is present, combat and transfer authoring must have the exact same target ID, collider, pose root, local aim point and local aim-shape geometry. Its transfer kind/profile/sink never determines attackability. Missing or mismatched co-authoring is a startup contract error before either session is created.

Cross-validation uses the approved internal immutable authored-geometry accessor only. It validates the TransferTarget binding but does not call `IsStillAvailable`, build a runtime transfer descriptor, read modifier/session state or add any public surface.

## Exact observation capture

M2B1 consumes the completed player pose at `t-1`, approved camera pose at `t-1`, exact `AimSample` at `t`, ordinal combat registry and combat-owned availability snapshot taken before same-tick damage.

- Unity world poses are quantized Q1000 AwayFromZero and every conversion/addition is checked before publication.
- Projection, inverse pointer conversion, shape test, squared screen distance and gamepad angle use the verified `TransferFixedMath` implementation without float-based recomputation.
- LOS uses the verified layer 8 / mask 256 query, ignores triggers and player/target-root colliders, fails closed at 64-hit saturation and uses stable authored diagnostics.
- `PlayerDistanceKey` is the same rounded Q1000 squared-distance key used by VD-02: `checked((dx²+dy²+500)/1000)`. Therefore 5.999/6.000/6.001u map exactly to 35,988 / 36,000 / 36,012.
- `IsAttackable` is true only when `CanReceivePlayerBasicAttack=true` and the M1 snapshot before this tick says the combatant is alive. Transfer availability and active transfer never participate.
- The observation list is constructed fully before publication and is independent of registry enumeration order after validation.

## One-sync receipt

The internal transfer capture receipt contains only `SimulationTick`, `AimSampleId`, `AimSample.SampleTick` and `SimulationCameraPoseSnapshot.CameraPoseTick`. It contains no target selection or transfer state.

- A receipt is published only after the single sync succeeds and before either target builder reads poses.
- The combat driver requires an exact matching receipt whenever its input contains aim. Missing, stale or mismatched receipt is a contract error before M2A/M1 mutation.
- No aim means no attack press and requires no synchronization. A press without the exact aim/camera pair is rejected by input construction.
- The future VD-07 producer must submit the same admitted aim/camera pair to the transfer and combat phase inputs. Mouse presses from letterbox/pillarbox are not produced; direct malformed fixtures remain deterministically rejected by M2A.
- “One sync” counts the authoritative `-200` aim capture only. Movement's later default-order temporary query-pose synchronization cannot create or satisfy a receipt and is tested separately from the capture count.

## Combat phase input and processing

Internal `CombatSimulationInput` contains exact `Tick`, coupled nullable camera/aim, nullable `BasicAttackPressed`, non-null defensively copied external `DamageRequest` list and `EncounterReset` flag.

- Inputs are exact future ticks and unique per tick. Missing input becomes an empty non-reset tick so M1 and M2A clocks still advance.
- Every external request and optional press must use the input tick. Reset is mutually exclusive with aim, press and damage.
- Initialization freezes `MaxRegisteredInvulnerabilityTicks`, the maximum of every validated registration. Before consuming M2A, the driver validates registry/session alignment, the capture receipt, all input structure, and `checked(tick + max(21, MaxRegisteredInvulnerabilityTicks))`. This single conservative horizon covers next-tick arithmetic, every M1 invulnerability calculation allowed by the authored `0..600` range, the M2A 21-tick cooldown and a possible `t+1` removal before either session mutates.
- M2A is processed once. Its zero-or-one request is appended to a copied external batch. The driver does not sort, deduplicate, change IDs or apply health.
- M1 is processed once with that batch. Full `BasicAttackTickOutcome`, ordered M1 results and combat snapshots are published together as an internal driver outcome.
- A basic-attack miss, invalid aim, stale aim or cooldown still advances both sessions for the tick and yields no basic-attack request.

Malformed authoring/capture/input is fail-stop. Input already consumed from a driver buffer is not retryable. No downstream failure permitted by this contract may occur after M2A publication: the driver preflight plus existing M1 constructor/session validation must make the M1 call non-throwing for in-contract data.

## Encounter reset and death handoff

Encounter reset is explicit; it is never inferred from scene unload, object disable or absence.

- Reset may occur only after a completed prior tick. At exact next tick `t`, M1 resets and re-registers the authored roster, M2A is replaced with a new session starting at `t`, and both process one empty reset tick. Sequence IDs and cooldown are encounter-lifetime and may restart after this replacement; global `SimulationTick` never rewinds.
- A combatant killed at tick `t` remains a valid transfer-session participant through completion of tick `t`, because transfer precedes combat. It becomes non-attackable immediately in the final combat outcome.
- If the dead combat target has a matching `TransferTarget`, combat schedules one internal `TransferTargetRemoved(targetId, t+1)` through a dedicated mergeable removal seam. Transfer consumes it at `t+1` before capture and clears an active transfer exactly once with `TargetRemoved`.
- Death of a target without a matching transfer target schedules no transfer removal. Repeated `TargetDead`, duplicate or invulnerable results never schedule a second removal.

The removal seam merges with an already queued transfer input for the same tick using ordinal unique target IDs; it does not replace aim, press or lifecycle data. Conflicting duplicate removal payloads are contract errors before transfer-session mutation.

M2B exposes no reward, room-complete, run-failure or public death event. Those remain later contracts.

## Required evidence

### M2B1

- Public component is the only new public type; all fields and pinned registrations validate at exact boundaries.
- Registry rejects null, duplicate, unsorted and duplicate stable-collider authoring without discovery APIs.
- Optional transfer co-authoring matches all shared identity/geometry fields; Ordan body rejects a transfer binding.
- Q1000 pose/camera capture pins 5.999/6.000/6.001u and checked overflow atomicity.
- Mouse inside/proximity produces exact 23/24/25px keys; gamepad observations preserve 17.9/18.0/18.1°, 25.9/26.0/26.1° and 3.9/4.0/4.1° selector behavior when passed to M2A.
- LOS open/blocked/trigger/self/target-root/saturation fixtures use mask 256 and stable diagnostics.
- Dead, player and authored non-attackable combatants are excluded independently of transfer state.

### M2B2

- Reflection verifies execution orders `-200`, `-190`, default movement and proves one synchronization for a shared aim tick.
- Matching and stale/missing/mismatched capture receipts, exact input buffering, empty ticks and reset exclusivity are test-pinned.
- Horizon preflight is pinned with actual maximum invulnerability values 0, 45 and 600 at pass/overflow boundaries and proves no M2A/M1 publication on failure.
- Attack at `t` changes walker health `9→6`; `t+20` is cooldown/no request and `t+21` changes `6→3`.
- Fired miss shares cooldown; stale/invalid/cooldown emit no M1 request.
- External requests plus attack are passed unsorted once and M1 returns canonical request-ID order, duplicate/tombstone and same-tick death behavior unchanged.
- Death at `t` excludes attack selection and clears a matching active transfer exactly once at `t+1`; no same-tick retroactive clear occurs.
- Reset retains exact global `t+1`, restores authored health, clears tombstones/attack cooldown/sequence IDs and requires no scene discovery.
- Malformed input, receipt mismatch, checked horizon overflow and removal merge failure leave M1/M2A/transfer state at their contractually preceding publication boundary.
- The same 60Hz script grouped under 30/60/144 render frames exact-matches the complete attack outcome, M1 results, every combat snapshot, death handoff, transfer outcome and next expected ticks.
- Final regression includes M1/M2A Combat EditMode, Transfer Unity EditMode/PlayMode, Movement EditMode/PlayMode and full project EditMode.

## Stop conditions

Stop and report to Sol if the adapter requires a second physics sync, a public payload/type beyond `CombatTarget`, changes to existing ABI or M1/M2A semantics, Transfer-session state to decide attackability, same-tick retroactive transfer clearing, scene discovery, device input, animation-driven damage, a new layer/project setting, or any deferred AI/reward/room/run feature.

## Approval gate

Luna reported no remaining P0/P1 contradiction after the distance-key and horizon-preflight corrections, and Sol approved implementation on 2026-08-27. M2B completion remains partial substrate evidence and does not verify enemy behavior, boss patterns, rewards, room completion, final input/UI or the full `VD-03`/`VD-07` acceptance criteria.
