# Work Contract: VD-03 M4B2 Ordan Scripted Transfer Exposure

- Status: Verified
- Approved by: Sol, 2026-09-06 after Luna pre-gate PASS (`P0=0`, `P1=0`, `P2=0` after cleanup)
- Verified by: Sol, 2026-09-06 after Luna implementation review (`P0=0`, `P1=0`, non-blocking evidence/gate-hygiene `P2=2`, both closed in the implementation evidence)
- Drafted by: Sol, 2026-09-06
- Unit design and implementation: Terra
- Independent verification: Luna
- Owning specs: VD-03, with affected VD-02 and VD-07 boundaries
- Requirements: `REQ-COM-001`, `REQ-COM-003`, `REQ-COM-004`, `REQ-COM-006`; affected `REQ-WT-001`, `REQ-WT-002`, `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-006`, `REQ-WT-007`
- Partial acceptance targets: `AC-COM-002`, `AC-COM-003`, `AC-COM-004`; affected `AC-WT-003`, `AC-WT-005`

## Purpose and staged boundary

M4B2 consumes M4B1's immutable `OrdanTransferExposureForecast(t,t+1,targetId)` and makes the exact scripted payload `BossWeight-0`, `BossWeight-1`, or `BossWeight-2` targetable before Transfer phase `-200` of tick `t+1`. It also clears an active transfer when that temporary payload exposure ends without permanently unregistering the cyclic handle.

M4B2 adds the runtime seam, reusable payload modifier sink, temporary-withdrawal input and focused test graph only. It does not author the production boss scene, positions, sprites, animation, VFX, attack geometry, `BossAuditBox-0` behavior, room/reward consumer, choice transition, audio, or run lifecycle. M4B3 owns the fresh authored boss graph and must include all payload and Audit targets before the one-time Transfer initialization.

## Sol decisions frozen for M4B2

### Fixed phase order and forecast ownership

The exact boss sequence is:

1. `-200` Transfer completes tick `t`;
2. `-190` boss Combat completes tick `t`;
3. `-185` M4A bridge publishes the exact forecast for Transfer tick `t+1`;
4. new `OrdanBossTransferExposureScheduler` at `-180` consumes that exact bridge output, changes the three payload sinks for the inter-tick interval, and merges any required temporary withdrawal into Transfer input `t+1`;
5. the next `-200` Transfer observes the prepared sink/collider state and consumes the preserved input plus temporary withdrawal.

The scheduler must not reconstruct phase age, duration, slot, ordinal, target rotation, or target position. `Forecast.TargetId` is its sole exposure authority. Initial tick has no prior exposure; all three sinks must already be hidden before the first `-200`. Every scheduler advance after bootstrap requires `bridge.LatestOutcome.SourceTick == t` and `Forecast.TransferTick == t+1`, where `t` is the scheduler's exact next source tick.

In `Awake`, before the first `-200`, the scheduler validates every binding and payload pair, registers its private exposure-end lane once, then commits a prevalidated initial state with all three sinks unexposed and all three colliders disabled. This does not initialize, register, rewrite or freeze the Transfer session/registry. Failure is fail-stop and never exposes a fallback target.

### Persistent registration, temporary exposure

- `BossWeight-0`, `BossWeight-1`, and `BossWeight-2` are registered once with the fresh boss `TransferSession`. No runtime add, re-register, registry rewrite or second freeze is allowed.
- Each exact target binds one `OrdanBossPayloadTransferModifierSink` and one collider. The scheduler accepts exactly three ordinal target/sink pairs and rejects null, duplicate, reordered, cross-wired or additional payload owners before first advance.
- The exact base profile is `Boss.Payload.Scripted.v1`; kind is `BossPayload`; accepted modifier is `Target.BossPayload.ScriptedHeavy.v1`.
- Exactly the forecasted payload sink is exposed and has its target collider enabled. The other two are hidden and disabled. An empty forecast hides all three.
- A hidden payload sink's `IsStillAvailable()` returns false even though its descriptor remains registered; its `Clear()` remains valid while hidden so Transfer can finish an already-active relation on the following tick. The observation builder may still visit a registered disabled collider, but its descriptor is unavailable and cannot become a selected Ready target.
- The sink owns only its `isExposed`, applied scripted-heavy bit and last accepted Transfer revision. It never changes player state, M4A, health, damage, phase, room state or reward. `TryApply` accepts the exact bound ID/kind/base/modifier, nonnegative tick and strictly increasing revision only while exposed. `Clear` validates an allowed reason and increasing revision, then removes only the applied bit.
- Scheduler exposure commits are idempotent for the same desired ID but source ticks are not replayable. An unexpected exception is fail-stop; no guessed rollback or alternate handle is allowed.

### Internal temporary-withdrawal contract

Existing `TransferTargetRemoved` remains permanent and must never be used for normal payload Telegraph/Execute exposure changes. M4B2 additively introduces internal `TransferTargetExposureEnded(targetId,tick)` for a scripted target that remains registered but is unavailable on that tick.

- `TransferSimulationInput` gains a defensively copied internal exposure-end collection through an additive constructor; the existing constructor remains source-compatible and supplies an empty collection.
- `TransferSimulationDriver.MergeExposureEnds` accepts only one exact future tick strictly after the current player tick, preserves any queued camera, aim, press, permanent removals and lifecycle value, merges by exact target ID, sorts ordinally and writes only after complete validation. It neither changes `_removedTargetIds` nor initializes a replacement session.
- `MergeExposureEnds` is restricted to one private owner token registered by the exact scheduler before the first Transfer publication. Missing, duplicate, late, forged or changed-owner registration and calls reject without queue mutation. The token never appears in an output value. The merge itself requires an initialized Transfer driver and exact current publication and never calls `EnsureInitialized`; existing `MergeRemovals` retains its already verified initialization behavior.
- `MergeRemovals` and `MergeExposureEnds` share one staged input-replacement rule: both preserve camera, aim, press, permanent removals, temporary ends and lifecycle regardless of merge call order. Calling the two merges in either order produces the same exact queued value. A queued lifecycle value retains the temporary-end data, but lifecycle-first processing gives it no gameplay effect.
- `TransferSession` processes lifecycle first. Without lifecycle it processes temporary exposure ends before permanent removals, aim evaluation and press. A temporary end clears highlight for the ID. If it is active, Transfer alone emits one ordinary successful `TransferAttemptResult` with `TransferCleared(TargetRemoved, previousTargetId, tick, revision)`, restores player Baseline through the existing final reflection, calls the registered sink's `Clear`, and retains the registration. A repeated inactive end is an idempotent no-op and consumes no revision.
- A same-ID temporary end plus permanent removal produces exactly one clear, one revision increment and one sink `Clear` when active, and none when inactive; it then permanently unregisters the descriptor and adds the ID to `_removedTargetIds`. The optional press result remains the distinct existing outcome. A lifecycle input supersedes both collections and retains existing lifecycle semantics. Temporary ends do not appear in `_removedTargetIds` and the same payload ID can be selected in a later `0,1,2,0` cycle.
- `TransferPhaseInputSummary` and existing public Transfer value types do not change. The temporary-end type and merge seam are internal; no public ABI or asmdef change is permitted.

M4B2 explicitly approves using existing `TransferClearReason.TargetRemoved` for the active transfer's clear result when a scripted payload object leaves the targetable world interval. This means “the selected target was removed from current targetability,” not that its pre-registered descriptor is permanently destroyed. Permanent registry removal remains uniquely represented by `TransferTargetRemoved`.

### Scheduler transaction and cleanup

- At `-180(t)`, the scheduler first validates the exact bridge/Transfer bindings, completed Transfer publication `t`, `transferDriver.BoundPlayer.NextExpectedTick == t`, source/forecast horizon, three payload authoring values, previous scheduler state and required end set. Every merge target is exactly `t+1`. It then calls the Transfer future-merge seam and only afterward performs prevalidated, nonthrowing sink/collider exposure commits and publishes an immutable scheduler outcome.
- If the desired ID differs from the previously exposed ID, the previous ID is hidden immediately and one exposure end for tick `t+1` is merged. The desired ID becomes exposed before `-200(t+1)`. If unchanged, no end is queued and no sink revision changes.
- On an empty forecast the prior payload is hidden and, if one was exposed, its end is queued for `t+1`.
- If the exact bridge output contains the one-shot `Defeated`, `RewardRequest`, and `RoomCompletionRequest` handoff set, the scheduler treats it as terminal rather than as an ordinary empty forecast: it hides all payloads and merges permanent `TransferTargetRemoved` values for all three payload IDs at `t+1`. It schedules no temporary end for the same IDs, publishes one terminal outcome and accepts no later advance. If Combat death is learned at `-185(t)` after Transfer `t` already completed, this permanent cleanup occurs at `-200(t+1)`; completed Transfer is never rolled back.
- An ordinary empty forecast during Execute, Vulnerable or Recovery schedules only `TransferTargetExposureEnded` for the previously exposed ID and preserves all registrations.
- Missing, stale, skipped, repeated, wrong-source, wrong-horizon or invalid-target forecasts reject before queue, sink, collider or latest scheduler mutation. A fail-stop does not invent a safety mutation. M4B3 owns encounter-root disable/teardown after a surfaced contract failure.
- Existing `RoomLeaving`, `RunFailed`, `Cutscene`, and `DemoCompleted` inputs remain the lifecycle authority. If one is already queued for `t+1`, merge preserves it; at `-200(t+1)` lifecycle clear supersedes temporary ends. M4B2 does not emit lifecycle commands. M4B3 must hide/disable the encounter objects during teardown.

### Authoring and geometry boundary

- M4B2 validation proves the three exact payload target/sink/collider bindings and that each payload belongs to the bound Transfer registry. It may coexist in that registry with future exact boss targets such as `BossAuditBox-0`, but no duplicate or additional `BossWeight-*` ID is allowed.
- `TransferSimulationDriver` adds internal read-only `BoundPlayer` and `BoundRegistry` seams. The scheduler proves `bridge.BoundTransferDriver == transferDriver`, exact registry identity, and exact payload object membership without revalidating or freezing the registry. Each `-180` advance additionally requires the already completed `TransferPhasePublication(t)`, which proves the fresh registry was initialized by `-200(t)`.
- Ordan's body remains absent from the Transfer registry.
- Target geometry scalars, hierarchy placement, renderers and production collider shapes are M4B3 decisions. Focused tests use explicit temporary-scene fixtures and do not establish production geometry.
- The scheduler adds no `Physics2D.SyncTransforms`, physics query/callback, `Update`, coroutine, wall clock, delta time or RNG. The existing `-200` Transfer phase remains the sole capture/synchronization authority.

## Owned state and immutable output

`OrdanBossTransferExposureScheduler` owns its first expected source tick, previous exposed target ID, terminal latch, successful-publication latch and latest `OrdanBossTransferExposureOutcome(sourceTick,transferTick,previousTargetId,exposedTargetId,temporarilyEndedTargetIds,permanentlyRemovedTargetIds,isTerminal)`.

The outcome defensively copies ordinal end/removal lists. It is presentation-neutral and does not expose Unity objects. `sourceTick+1 == transferTick`; IDs are empty or exact payload IDs; a nonterminal temporary-end list is empty or contains only the prior payload ID when it differs from the new ID; a terminal removal list is exactly all three payload IDs and its temporary list is empty.

## Required verification

Focused engine-free/Transfer tests must cover:

1. temporary end clears active/highlight and player state once while retaining registration;
2. inactive/repeated end is revision-neutral and later same-ID selection succeeds;
3. temporary end plus permanent removal, lifecycle precedence, malformed tick/ID, duplicate and caller-mutation cases;
   an active same-ID pair emits one clear/revision/sink-clear and then unregisters exactly once;
4. existing permanent removal and lifecycle behavior remains unchanged.

Focused Unity tests must cover:

1. exact `-200 < -190 < -185 < -180 < next -200` attributes and trace;
2. initial all-hidden state and age-0 targetability from the prior Recovery tick forecast;
3. exactly one exposed collider for Telegraph ages `0..59`, empty at Execute, and cycle `0,1,2,0` without registry reinitialization;
4. active payload transfer followed by exposure end produces one `TargetRemoved` clear, player Baseline, sink clear and later same-ID reuse;
   hidden sinks are explicitly unavailable to selection while their `Clear` remains valid;
5. future merge preserves queued aim, press, permanent removals and lifecycle, with ordinal dedupe and defensive copies; both `MergeRemovals → MergeExposureEnds` and the reverse order yield the same value;
6. empty/unchanged/change exposure outcomes and exact immutable IDs;
7. missing/stale/skipped/repeated/wrong-horizon/invalid-target forecast, cross-wired driver, duplicate/reordered/mismatched target/sink/modifier and altered queue rejection without mutation;
8. same-tick Transfer success followed by Combat death schedules next-Transfer cleanup without rollback;
   ordinary empty forecast preserves registration, while the exact defeated handoff permanently removes all three IDs and records them in `_removedTargetIds`;
   terminal outcome has an empty temporary-end list and every later scheduler advance rejects without mutation;
9. 30/60/144 render grouping produces identical fixed-tick exposure/Transfer traces;
10. no production scene/prefab/settings/public ABI and no additional physics/time/RNG authority.

M4B1 focused regression, Transfer focused regression, Combat Unity regression, full EditMode and full PlayMode must remain green. Every implementation change cites the listed REQ IDs and every result maps to the listed AC IDs. XML totals, failures, skips and SHA-256 are recorded.

## Allowlist

- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossSimulationDriver.cs` — additive bound-Transfer read seam only;
- new `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTransferExposureScheduler.cs` and `.meta`;
- new `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossPayloadTransferModifierSink.cs` and `.meta`;
- `Assets/AcadeGameMaker/Runtime/Transfer/TransferContracts.cs`;
- `Assets/AcadeGameMaker/Runtime/Transfer/TransferSession.cs`;
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferSimulationDriver.cs`;
- `Assets/AcadeGameMaker/Tests/EditMode/Transfer/TransferM1Tests.cs`;
- `Assets/AcadeGameMaker/Tests/PlayMode/TransferUnity/TransferSimulationDriverPlayModeTests.cs`;
- new `Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/OrdanBossTransferExposureTests.cs` and `.meta`;
- new `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossTransferExposurePlayModeTests.cs` and `.meta`;
- `docs/specs/work-contracts/2026-08-25-vd02-weight-transfer.md` — narrow event-versus-result permanence clarification only;
- this contract, its pre-gate/evidence documents, `docs/README.md`, and the minimal boss-order/temporary-exposure amendment to `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md`.

No public member/type, asmdef, AssemblyInfo, existing regular-enemy runtime, M1/M2A, M4A phase logic, scene, prefab, art/audio asset, package, input action or project setting may change.

## Stop conditions

Stop and return to Sol if implementation requires permanent unregister for normal exposure expiry, runtime registry rewrite/re-freeze, M4A forecast reconstruction, same-tick post-Transfer mutation, completed Transfer rollback, a second Transfer session, public ABI, production geometry, room/reward/run mutation, or new physics/time/RNG authority.

## Ollama utilization record

| Lane | Outcome | Sol screening disposition |
|---|---|---|
| Kimi K3 | used and accepted in part | Accepted persistent registration, explicit preflight/commit ordering and append-without-replacement emphasis. Rejected invented exposure duration, mutable bool arrays, sink-index identity, forecast generation outside M4A and public data models. |
| GLM 5.2 | used and accepted in part | Accepted input overwrite, accidental unregister, duplicate application, lifecycle suppression, premature removal and cyclic reuse risks. Rejected dynamic re-registration and ambiguous hidden-target deferral premises. |
| MiniMax M3 | used and accepted in part | Accepted baseline, first exposure, same-ID idempotence, change, cleanup, lifecycle-preservation and cyclic-reuse fixture rows. Rejected generic expiry/magnitude fields and any authority not present in the frozen forecast. |

No cloud model received repository content, local paths, credentials, personal data, secrets, or approval authority.

## Approval gate

Luna independently reviewed the exact phase order, temporary-clear semantics, death permanence, future-input merge, registry reuse, lifecycle precedence, allowlist and required tests with `P0=0`, `P1=0`. Sol resolved the three P2 wording/test/metadata items and approves implementation within this exact allowlist and stop boundary. Approval is not Unity execution evidence.
