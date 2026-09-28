# Work Contract: VD-03 M4B1 Ordan Unity Combat Bridge

- Status: Verified
- Verified by: Luna, 2026-09-05 (`P0=0`, `P1=0`, non-blocking `P2=1`); Sol integration approved
- Approved by: Sol, 2026-09-05 after Luna pre-gate PASS (`P0=0`, `P1=0`, `P2=0`)
- Drafted by: Sol, 2026-09-05
- Unit design and implementation: Terra
- Independent verification: Luna
- Owning specs: VD-03, with affected VD-02, VD-04, VD-05 and VD-07 boundaries
- Requirements: `REQ-COM-001`, `REQ-COM-003`, `REQ-COM-004`, `REQ-COM-006`; affected `REQ-WT-001`, `REQ-WT-005`, `REQ-WT-006`
- Partial acceptance targets: `AC-COM-002`, `AC-COM-003`, `AC-COM-004`; affected `AC-WT-005`

## Purpose and staged boundary

M4B1 connects the Verified engine-free `OrdanBossSession` to one boss-scene Unity Combat owner. It reads the already completed `-200` Transfer publication and the `-190` Combat result (private provisional result on bootstrap, completed publication thereafter), advances M4A once at `-185`, and submits an immutable, possibly empty batch for exact `t+1` Combat consumption. It does not create the scripted payload objects, authored boss scene, pattern hit geometry, room/reward consumer, UI, VFX, audio, or actual run transition. Those remain M4B2/M4B3 work.

This stage is deliberately not added to any production scene. It establishes the runtime seam and fixed-order proof without changing the already Verified regular-enemy graph.

## Sol decisions frozen for M4B1

### Single Combat owner

- A boss scene uses one new internal `OrdanBossCombatSimulationDriver` at exact execution order `-190` instead of the regular-enemy `CombatSimulationDriver`. They must never coexist under one encounter root.
- The explicit boss graph binding contains a one-element Combat-owner list whose only value is the bound `OrdanBossCombatSimulationDriver`. A regular `CombatSimulationDriver`, a duplicate boss driver, null, reordered or additional owner rejects before session creation. M4B3's authoring validator must later prove that the explicit list equals the authored root; M4B1 is not activated in a production scene before that proof.
- The boss driver owns exactly one `CombatSession` and one `BasicAttackSession`. M1 remains the only health, invulnerability, request ordering, dedupe, clamp and death authority.
- Its exact frozen Combat roster is ordinal `ordan`, then `player`. `ordan` is health `60`, invulnerability `0`, basic-attack eligible and has no matching `TransferTarget`; `player` is health `5`, invulnerability `45`, and is not basic-attack eligible.
- The boss Combat driver and bridge bind the same exact player, `TransferSimulationDriver`, Combat registry and boss Combat owner identities. Cross-wired instances reject before initialization.
- The existing `CombatTarget`, `CombatTargetRegistry`, `CombatObservationBuilder`, M1 and M2A are reused without semantic changes. No existing regular-enemy driver, builder, validator or scene is modified.

### Fixed phase order and publication ownership

The boss encounter order is:

1. `-200` existing `TransferSimulationDriver` completes and publishes one exact `TransferPhasePublication` for tick `t`;
2. `-190` `OrdanBossCombatSimulationDriver` consumes the exact optional player input plus the mandatory boss-delivery batch for `t` after bootstrap. On the first tick it retains a private provisional outcome and staged M1/M2A sessions; on later ticks it publishes one completed `OrdanBossCombatOutcome` before returning;
3. `-185` `OrdanBossSimulationDriver` validates both upstream results for `t`, projects at most one relevant rising edge, advances M4A once, publishes one immutable bridge outcome, and commits one empty-or-singleton batch for Combat tick `t+1`. On bootstrap the same preflight transaction also promotes the private provisional Combat sessions/outcome;
4. later stages may consume the immutable bridge output but cannot change it.

Default execution order attributes, reflection tests and a PlayMode trace must prove `-200 < -190 < -185`. M4B1 adds no `Physics2D.SyncTransforms`, physics query, physics callback, `Update`, coroutine, wall clock, delta time or RNG.

### Mandatory next-tick delivery lane

- Before the first Combat attempt, the bridge prepares and registers exactly one owner-token lane with the boss Combat driver. First source tick `s` has no incoming batch; every later Combat tick `t>s` requires exactly one batch whose source is `t-1` and delivery is `t`, including an empty batch.
- `OrdanBossSimulationDriver.Awake`, ordered after the boss Combat driver's noninitializing `Awake` and before the first FixedUpdate, performs this one-time prepare/register transaction and freezes the shared first source tick. Reconfiguration or registration after any Combat attempt rejects.
- Bootstrap requires `s == player.NextExpectedTick == CombatSession.NextExpectedTick == BasicAttackSession.NextExpectedTick`. The `Awake` registration finishes before any `-190` FixedUpdate; explicit test initialization uses the same transaction.
- The bridge can submit only M4A's exact `PayloadDamageRequest`. A singleton must preserve request ID, source handle, target `ordan`, amount `8`, kind `BossPayloadTransfer`, and delivery tick `source+1`. All other M4A intents remain immutable presentation/future-geometry values and do not become damage in M4B1.
- The boss Combat driver merges the validated boss batch, caller external requests and optional M2A request into one value batch. M1 alone performs canonical ordinal ordering and emits aligned results.
- Within `-190`, all rejection-capable graph, tick, input, capture-receipt, observation, queue and M2A validation occurs before consuming the pending boss batch or committing M1/M2A. A malformed batch is fail-stop and not silently discarded. On bootstrap the sessions and result remain private provisional values until `-185` adoption.
- For any aim-bearing boss Combat input, `-190` compares the bound Transfer driver's current `TransferCaptureReceipt` field-for-field: processing tick, aim sample ID, aim sample tick and camera pose tick. Missing, stale or mismatched receipt rejects before M2A/M1 mutation and before batch consumption. No additional synchronization is permitted.
- A preflighted batch carries a private owner token and defensive copy. Forged, stale, duplicate, wrong-tick, non-payload, empty-ID or altered candidates reject without queue mutation.

### M4A input and result alignment

- The bridge finds exact `ordan` and `player` snapshots in the current Combat outcome; duplicates, omissions, wrong order, wrong health contract or mismatched tick reject before M4A mutation.
- If the previous bridge output contained a pending payload request, the current canonical request/result arrays must contain that exact request and aligned result once. Missing, duplicate or altered request/result evidence rejects. If no request is pending, no payload result is passed to M4A.
- Current Combat death is passed unchanged and retains M4A's priority over phase, edge, vulnerability and intents. The bridge never invents or repairs an M1 result.
- The bridge calls `PreviewNext`, preflights the future batch and, on bootstrap, preflights promotion of the exact provisional Combat values. It then performs only prevalidated deterministic commits: bootstrap Combat promotion if applicable, exact M4A candidate, future batch, and latest bridge publication. These commit methods contain no scene/physics callback. An unexpected engine exception is fail-stop; no rollback is guessed.

### Transfer edge projection

- The bridge reads only the current `TransferPhasePublication` from its exact configured Transfer driver. A relevant edge exists only when its exact successful `PressResult.StateChanged` has tick `t`, player modifier `Player.Lightweight.v1`, the exact target modifier below, a positive revision, and target ID `BossWeight-0`, `BossWeight-1`, `BossWeight-2`, or `BossAuditBox-0`.
- `BossWeight-*` maps to local kind `BossPayload`; `BossAuditBox-0` maps to local kind `Box`. The projection is exact `Baseline -> Heavy` with the published positive revision. Clear/removal results, failed presses, recalls, highlights and unrelated target IDs produce no rising edge.
- `BossWeight-*` requires `Target.BossPayload.ScriptedHeavy.v1`; `BossAuditBox-0` requires `Target.Box.Heavy.v1`. A relevant ID with a mismatched modifier is a contract error, not an irrelevant edge.
- Tests cover the exact modifier mapping for each of the four relevant IDs. Any mismatch preserves M4A session, queue and latest bridge publication.
- M4A still validates expected active handle, phase, global revision and per-target revision. The bridge does not filter a relevant-but-invalid edge into silence.

### Telegraph age-0 availability seam

M4B1 resolves the previously identified phase-order conflict with an M4A-owned immutable next-tick exposure forecast; it does not mutate a Transfer target yet. Bridge-side reconstruction from snapshot age, duplicated phase tables, Unity object state or guessed payload ordinal is forbidden.

- M4B1 additively extends the internal M4A output with `OrdanTransferExposureForecast(sourceTick,transferTick,targetId)`. `transferTick` is checked exact `sourceTick+1`; `targetId` is empty or one exact `BossWeight-*` ID. The value participates in output equality and caller-mutation tests.
- M4A computes the forecast from its private candidate state before commit. It is exposed exactly when the deterministic next M4A tick, assuming the boss remains alive, will be a SeizureWeight Telegraph tick. This includes the last Recovery tick before Seizure entry and excludes the last Telegraph tick before Execute.
- The bridge forwards and validates the exact candidate forecast without recalculation. No payload ordinal or phase duration is duplicated in Unity code.
- M4B2 will apply the forecast before `-200` on `t+1`, so Telegraph age `0` is targetable. Initial tick has no payload exposure because the first slot is DebtLineA.
- The forecast describes the complete next-Transfer-tick exposure, not a clear command. It holds the same ID for forecasted Telegraph ages `0..59`; it is empty for forecasted Execute, Vulnerable, Recovery or Defeated. In particular, a current Telegraph age `58` forecasts exposed age `59`, while current age `59` forecasts empty Execute age `0`. M4B2 alone interprets exposure changes and cleanup.
- If Combat kills Ordan at `t+1` after Transfer already accepted an upstream action, M4A death still wins at `-185`; M4B2 must schedule deterministic `TargetRemoved` cleanup for the following `-200` tick. No cross-phase rollback of the completed Transfer phase is promised.

### Handoffs and presentation

- `OrdanBossBridgeOutcome` defensively contains the M4A output, exact source tick, next exposure forecast, and the three one-shot death handoff values when present.
- M4B1 does not grant reward, complete a room, change run state, lock input, or mutate presentation. Future consumers may read values only after a later contract.
- No public type/member, asmdef reference, project setting, layer, tag, input asset, scene or prefab is added or changed.

## Inputs, owned state and outputs

### Boss Combat driver input

`OrdanBossCombatInput(tick,camera|null,aim|null,basicAttackPressed|null,externalDamageRequests)` follows the existing exact aim/camera/press coupling and defensively copies current-tick requests. There is no encounter reset; a fresh component creates a fresh boss session.

### Boss Combat driver state/output

It owns M1/M2A, a unique next-tick boss-delivery queue, registration latch and latest immutable `OrdanBossCombatOutcome(tick,attack,canonicalRequestTrace,damageResults,combatants)`. `combatants` is exact ordinal `ordan`,`player`; `canonicalRequestTrace` uses M1's complete ordinal key `(RequestId, SourceId, TargetId, Kind, Amount)`; `damageResults` has the same count and fully index-aligned request ID, target and tick. All arrays are defensive immutable copies. Before M1, attack observation uses exact frozen pre-tick life: `ordan` is attackable while alive in every M4A phase and `player` is never a candidate. Transfer availability, exposure and vulnerability never gate or multiply M2A damage.

### Bridge state/output

It owns one `OrdanBossSession`, last exact pending payload request correlation, latest immutable `OrdanBossBridgeOutcome`, and successful-publication latch. It does not own Combat, Transfer, Unity body state or consumer state.

### First-use atomic adoption

Awake/bootstrap setup first validates every serialized binding, the explicit one-owner list, exact roster and frozen values, shared player/registry/Transfer/Combat identities, first tick and pristine lane state. It may register the empty owner-token lane but does not publish an outcome. At first `-190`, staged `CombatSession` and `BasicAttackSession` produce only a private provisional outcome. First `-185` validates that provisional value together with the exact current Transfer publication and a staged `OrdanBossSession`; only after all M4A/future-batch preflights succeed are the three sessions, provisional Combat outcome, lane state and bridge outcome adopted. Failure discards provisional values and leaves no public latest outcome.

After bootstrap, fixed phases are sequentially atomic, not one global rollback transaction. A successful `-190` Combat publication is never rolled back by a later `-185` rejection. Such rejection preserves the prior M4A state, future queue and latest bridge publication while retaining the already completed current Combat outcome, then fail-stops downstream boss phases for that tick. This matches the system-wide upper-phase no-rollback rule.

## Required verification

Focused EditMode tests must cover at least:

1. exact execution-order attributes and absence of additional physics/time/RNG APIs;
2. exact two-target roster, no Ordan body Transfer target, missing/duplicate/unsorted/cross-wired graph rejection before initialization;
   the explicit Combat-owner list rejects regular/boss coexistence, duplicate boss owner, null and extra owners;
3. boss lane bootstrap, mandatory empty batches, singleton payload batch, owner token, defensive copy, stale/forged/duplicate/wrong-tick/altered request rejection and non-consumption;
4. optional basic attack plus boss payload plus external request canonical M1 ordering with a single Combat owner;
5. exact current Combat snapshot extraction and exact pending request/result correlation, including lethal clamp and `TargetDead` pass-through;
6. Transfer projection for all four relevant IDs, irrelevant/failed/clear/recall silence, and relevant invalid edge preservation for M4A rejection;
7. bootstrap provisional adoption, plus phase-local preview/preflight/commit atomicity: pre-adoption failure publishes neither Combat nor bridge latest; later `-185` failure preserves its M4A/queue/latest state without rolling back the completed `-190` Combat publication;
8. M4A-owned next-exposure forecast at the tick before Seizure Telegraph, Telegraph ages `0`, `1`, `58`, `59`, Execute entry, Vulnerable, Recovery and death, including `BossWeight-0,1,2,0` without bridge table reconstruction;
9. one-shot death values without room/reward/run mutation;
10. checked tick/queue/forecast horizon overflow and repeated/skipped tick rejection.

Focused PlayMode tests must cover exact `-200/-190/-185` publication order, one current-tick M4A advance, exact `t+1` payload delivery/result consumption, empty-batch continuity, same-tick death priority and identical tick traces under 30/60/144 render grouping. Full EditMode and PlayMode regressions must remain green.

Each implementation change cites the listed REQ IDs. Each verification maps to partial `AC-COM-002`, `AC-COM-003`, `AC-COM-004` and affected `AC-WT-005` as applicable. XML totals, failures, skips and SHA-256 are recorded.

## Allowlist

- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossCombatSimulationDriver.cs` and `.meta`;
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossSimulationDriver.cs` and `.meta`;
- additive forecast changes and focused regression tests in `Assets/AcadeGameMaker/Runtime/Combat/OrdanBossSession.cs` and `Assets/AcadeGameMaker/Tests/EditMode/Combat/OrdanBossSessionTests.cs`;
- focused new tests under existing Combat Unity EditMode/PlayMode test assemblies and their `.meta` files;
- this contract, its pre-gate/evidence documents, `docs/README.md`, and a minimal fixed-order amendment to `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md`.

No asmdef, `AssemblyInfo`, existing runtime source, existing test, scene, prefab, asset, package, input action or project-setting change is allowed.

## Stop conditions

Stop and return to Sol if implementation requires changing M1/M2A/Transfer semantics, adding a second Combat owner to one graph, changing an existing verified driver, allowing same-tick boss damage delivery, hiding a relevant invalid edge, registering Ordan's body with Transfer, adding physics callback authority, exposing public ABI, or mutating room/reward/run/input/presentation state.

## Ollama utilization record

| Lane | Outcome | Sol screening disposition |
|---|---|---|
| Kimi K3 | used and accepted in part | Accepted immutable DTO separation, preflight/commit staging and exact tick queue emphasis. Rejected invented health/direction/magnitude fields, floats for authority and external command ownership. |
| GLM 5.2 | used and accepted in part | Accepted identity-retention leak, incomplete Combat publication, tick misalignment, atomic first-use, callback authority and death-handoff mutation risks. |
| MiniMax M3 | failed and replaced | Returned an empty final response without quota/auth/network/model error. Terra's impact matrix and Luna's checklist replace the fixture proposal. |

No cloud model received repository content, local paths, credentials, personal data, secrets, or approval authority.

## Approval gate

Luna reported `P0=0`, `P1=0`, `P2=0` after the forecast, owner, receipt, bootstrap-provisional, phase-local atomicity and canonical-order corrections. Sol approves implementation within this exact allowlist and denylist. Approval is not Unity execution evidence and does not close a full parent acceptance criterion.
