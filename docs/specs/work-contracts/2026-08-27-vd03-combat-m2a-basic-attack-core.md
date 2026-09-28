# Work Contract: VD-03 M2A Deterministic Basic Attack Core

- Status: Verified — Luna final PASS and Sol integration acceptance, 2026-08-27
- Owning specs: `VD-03`, `VD-07`, Approved 2026-08-25
- Assigned and contract-frozen by: Sol
- Implementer: Terra; Kimi K3 may propose only abstract repetitive test cases from the redacted work specification
- Independent verifier: Luna
- Requirement IDs: `REQ-COM-001`, `REQ-COM-004`, `REQ-UX-007`, `REQ-UX-008`, `REQ-UX-013`
- Partial acceptance evidence only: `AC-COM-003`, `AC-UX-007`, `AC-UX-012`
- Rollback point: annotated tag `vd03-combat-m1-verified-v1`

## Sol decision and scope

The vertical-demo basic attack is a **target-acquired instantaneous shot**, not a simulated projectile. A valid shot uses the same immutable Q4096 aim intent as weight transfer, independently acquires an attackable combatant, and emits at most one M1 `DamageRequest` for 3 damage. Presentation may later draw a non-authoritative tracer, but no render object or travel time may affect the result. The post-demo charged bow remains a separate real ballistic system under ADR-0021.

M2A is engine-free. It owns attack press validation, independent target selection, fired/miss/cooldown semantics and deterministic damage-request construction. It does not own device input, camera projection, live target observation, Unity physics/LOS, health mutation, enemy behavior, death/removal, reward, room/run lifecycle, ChoiceSkill, boss behavior or visuals. M2B will translate completed Unity poses into the exact internal observations frozen here.

## Allowed and forbidden files

- Allowed: one exact shared payload file under `Assets/AcadeGameMaker/Runtime/Core/**`; `Assets/AcadeGameMaker/Runtime/Combat/**`; `Assets/AcadeGameMaker/Tests/EditMode/Combat/**`; required `.meta`, assembly definition and internal-friend changes; this work contract and later evidence.
- Forbidden: Movement, Transfer, InputRouter/inputactions, Unity adapter, PlayMode, scene/prefab, project/package settings, UI, AI, boss, choice, room/run/persistence and external assets.
- The only new public type is the exact shared command below. Candidate observations, selection snapshots, attack outcomes, sessions and cooldown state remain internal.

## Shared public command

`BasicAttackPressed(long attackSequenceId, long aimSampleId, SimulationTick tick)` lives in `AcadeGameMaker.Core` with properties `AttackSequenceId`, `AimSampleId`, `Tick`.

- Both IDs are nonnegative signed-64 integers.
- `AttackSequenceId` is an encounter-lifetime unique semantic press ID assigned by the future VD-07 producer; it is not a render-frame count or Unity identity.
- `AimSampleId` names the exact `AimSample` consumed at the same tick. There is no nullable or fallback aim attack command.
- Constructor validation rejects negative IDs before any attack state exists.

## Internal candidate observation

Each observation freezes: ordinal `TargetId`; exact `ObservationTick`; `IsAttackable`; `IsLineOfSightOpen`; nonnegative `PlayerDistanceKey`; `MouseInsideShape`; nonnegative `ScreenDistanceSquaredKey`; and `GamepadAngleKey` in `0..1800` tenths of a degree.

- IDs use the existing 1..96 ASCII contract (`A-Z`, `a-z`, `0-9`, `.`, `_`, `:`, `/`, `-`).
- Observations are unique by target ID and exact-tick. Duplicate ID, wrong tick, invalid key or null list is a contract error before attack-state mutation.
- `IsAttackable=false`, blocked LOS or `PlayerDistanceKey>36000` excludes a target. `36000` is the user-approved inclusive 6.000u common targeting boundary; M2B must pin 5.999/6.000/6.001u.
- Attackability is independent from transfer availability and contains no transfer target kind or modifier state.

## Independent target selection

The selector consumes an exact-tick `AimSample` and the prior attack-highlight ID. It never reads Transfer state.

### Mouse

- `IsPointerInsideGameplayRect` must be true. False cannot select a target or fire an attack.
- A target is eligible when `MouseInsideShape=true` or `ScreenDistanceSquaredKey<=576`, the same 24 normalized-pixel comfort boundary used by the shared spatial grammar.
- Eligible mouse candidates sort by inside rank (`inside=0`, proximity-only `=1`) → screen-distance key → player-distance key → ordinal target ID.

### Gamepad

- A new target requires `GamepadAngleKey<=180`.
- The previous attack-highlight may be retained while `GamepadAngleKey<=260` and remains otherwise valid.
- A new best candidate replaces the retained target only when `newAngleKey+40<=retainedAngleKey`.
- Candidates sort by angle key → player-distance key → ordinal target ID.

Mouse and gamepad may select the same target from equivalent aim intent, but their approved input-specific comfort rules remain test-pinned. No angle, square root, float, discovery order or instance ID is recomputed in M2A.

## Attack session and tick semantics

- The internal session constructor accepts a first expected nonnegative global tick in `0..int.MaxValue-1` and processes exact increasing ticks. It calculates `checked(tick+1)` before mutation; `int.MaxValue` is an exhausted atomic failure.
- Each tick consumes at most one optional exact-tick aim sample, one immutable observation list and one optional exact-tick press. At most one press exists per tick; producer buffering of multiple presses is a later VD-07 concern.
- Before mutation, validate in this exact order: processing tick and next-tick arithmetic → observation list/ticks/keys/unique IDs → sample structural tick → press structural tick → repeated `AttackSequenceId`. Any failure is a contract error, advances no tick and consumes no new sequence ID.
- After structural preflight, a new press ID is staged for encounter-lifetime consumption regardless of semantic result. Semantic result precedence is exactly: missing or mismatched sample ID=`StaleAim` → mouse `IsPointerInsideGameplayRect=false`=`InvalidAim` → `tick<CooldownEndTick`=`Cooldown` → no selected target=fired miss → selected target=fired hit. Commit publishes the staged sequence ID together with the tick outcome.
- A first `StaleAim`, `InvalidAim` or `Cooldown` press therefore consumes its sequence ID but does not start a new cooldown. Reusing that ID on any later tick is a contract error before mutation.
- A structurally valid gamepad sample already has a nonzero Q4096 vector by the shared AimSample contract.
- A cooldown press is never buffered and emits no damage request.
- A valid non-cooldown press fires even when no attack target is selected. Both hit and miss set `CooldownEndTick=checked(tick+21)`; a press at `t+20` is blocked and `t+21` is allowed.
- A fired miss emits no `DamageRequest`. A fired hit emits exactly one request and may never cleave or penetrate to another target.
- No press produces no attempt result; target highlight may still update from the exact aim sample.

## Damage request construction

For a fired hit, construct exactly:

- `RequestId = "Player.BasicAttack/" + AttackSequenceId` using invariant unsigned decimal with no leading zero except `0`.
- `SourceId = "player"`.
- `TargetId = selected attack target ID`.
- `Amount = 3`.
- `Kind = DamageKind.BasicAttack`.
- `Tick = attack tick`.

All string and checked cooldown construction succeeds before session publication. M2A does not call `CombatSession.Process`; the future fixed-phase owner merges this request with other same-tick producers and passes the canonical batch to M1.

## Required evidence

- Public command validation at ID 0, `long.MaxValue`, and negative values.
- Observation validation, exact tick, duplicate IDs and list defensive copy.
- Mouse inside/proximity boundary at 23/24/25px and pointer-outside no-fire behavior.
- Gamepad acquire 17.9/18.0/18.1°, retain 25.9/26.0/26.1° and switch 3.9/4.0/4.1°.
- 5.999/6.000/6.001u inclusion boundary and blocked/unattackable exclusion.
- Canonical target ID tie-break with reversed observation order.
- Full precedence combinations for stale/invalid/cooldown, first semantic-failure ID consumption, repeated sequence ID and tick-overflow atomicity.
- Fire at `t`, cooldown at `t+20`, allowed at `t+21`; fired miss has the identical cooldown boundary as hit.
- Hit request exact ID/source/target/amount/kind/tick and no request on miss/cooldown/invalid/stale.
- Equivalent mouse/gamepad fixtures select the same intended target where both comfort rules admit it.
- The same 60Hz script grouped under 30/60/144 render frames produces exact attack snapshot, selection, attempt and request traces.

Kimi's abstract response contributed only the cooldown edge, fired-miss cooldown, reversed registration order, equivalent-device intent and duplicate-press test prompts. Suggestions that assumed an unstated stale-age threshold, angular wrap or public API were rejected. Terra and Sol remain responsible for every assertion and implementation decision.

## Stop conditions and deferred M2B

Stop if implementation requires Unity, a second public type, changes to existing AimSample/Damage ABI, Transfer state, a projectile, attack buffering, lifecycle reset, health processing, or any deferred system. M2B must separately freeze target authoring, completed `t-1` pose capture, camera projection, LOS mask/query, `DefaultExecutionOrder`, one-sync policy, M1 batch merge and Unity PlayMode evidence before those features begin.

## Approval gate

Luna independently reports no P0/P1 contradiction and Sol approves M2A implementation on 2026-08-27. M2A completion remains partial substrate evidence and does not verify actual combat playability or any full VD-03/VD-07 acceptance criterion.
