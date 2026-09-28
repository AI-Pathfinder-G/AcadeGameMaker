# Work Contract: VD-03 M4B3B1 Ordan Audit Forecast and Exposure

- Status: Verified
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Date: 2026-09-06
- Approved by: Sol, 2026-09-06 after Luna pre-gate PASS (`P0=0`, `P1=0`, `P2=0`)
- Verified by: Luna, 2026-09-06 after final implementation re-review (`P0=0`, `P1=0`; documentation-only `P2` closed by Sol)
- Parent: [VD-03 Combat and Enemies](../vertical-demo/03-combat-and-enemies.md)

## Purpose and split

M4B3B1 activates the already registered `BossAuditBox-0` only from a new immutable next-tick forecast owned by M4A. It extends the one existing boss Transfer exposure lane so the audit box can be selected during `BalanceAudit.Execute`, can interrupt the pattern through the already Verified rising `Baseline -> Heavy` edge, and is removed exactly once on terminal cleanup.

This unit does not add hostile hit geometry, player damage, pull movement, renderers, VFX/audio, encounter teardown, reward/room consumption, scene transition, or run/input authority. Those are separately gated as M4B3B2, M4B3B3, and M4B3C.

## Sol decisions frozen for M4B3B1

### M4A-owned audit forecast

- Add internal immutable `OrdanAuditExposureForecast(sourceTick, transferTick, targetId)` where `transferTick == sourceTick + 1` and `targetId` is either empty or literal `BossAuditBox-0`.
- `OrdanBossTickOutput` owns both the existing payload forecast and the new audit forecast. Its compatibility constructor supplies an exact empty audit forecast at the output horizon; M4A production calculation always supplies the explicit calculated forecast. Equality and candidate validation include both forecasts.
- The forecast is calculated only from the staged M4A candidate state. Unity consumers may forward it but may not infer it from `Snapshot.Phase`, phase age, durations, intent kind, object availability, or render state.
- The next Transfer tick exposes the audit box exactly when the next committed M4A input can legally accept its rising edge:
  - current committed state is `BalanceAudit.Telegraph` and `transferTick` equals the exclusive Telegraph end, so the next core step enters `Execute` age `0`; or
  - current committed state is uninterrupted `BalanceAudit.Execute` and `transferTick` is strictly before its exclusive Execute end.
- Every other state with a representable `t+1` horizon forecasts empty, including DebtLine, SeizureWeight, Recovery, Vulnerable, interrupted Audit, Defeated, and the tick whose next core step exits Execute.
- Payload and audit forecasts are mutually exclusive. Both may be empty. A valid staged candidate with a representable `t+1` horizon supplies an empty audit forecast when there is no exact legal next audit-exposure state.
- M4A's existing checked-horizon and checked phase-transition failures are never softened into an empty forecast. If calculating `sourceTick+1`, the output horizon, or a transition required by the staged candidate overflows, M4A rejects the complete candidate before mutation. It never synthesizes an empty forecast for a failed or non-representable candidate.

### One Transfer exposure owner

- `OrdanBossTransferExposureScheduler` remains the only `-180` owner and uses the same private owner token for payload and audit temporary ends.
- Production authoring binds the existing audit `TransferTarget`, `OrdanBossAuditTransferModifierSink`, and authored `BoxCollider2D` explicitly. All must belong to the same authored graph and exact Transfer registry as the three payload pairs.
- `TransferSimulationDriver` receives one additive internal registration path for the exact ordered boss exposure-end IDs `BossAuditBox-0`, `BossWeight-0`, `BossWeight-1`, `BossWeight-2`. It shares the existing one-owner/one-registration state. The existing exact three-payload test compatibility path remains source-compatible but cannot be used by the authored production graph.
- The scheduler has two explicit configurations selected only by their typed registration seams, never by runtime discovery, missing-reference inference, registry count, or forecast content. Legacy test configuration binds only the exact three payload target/sink pairs, ignores a non-empty audit forecast without exposing, removing, or mutating an audit target, publishes only the unchanged existing payload outcome shape, and retains exact three-ID terminal removal. Production configuration binds the audit target/sink/collider in addition to the payload pairs, requires exact four-ID registration, forwards both outcomes, and uses exact four-ID terminal removal. The existing `ConfigureForTests(bridge, transfer, payloads, sinks)` seam remains source-compatible; only the additive authored-production seam accepts audit references. The authored production graph rejects the legacy three-ID seam. A configuration cannot switch after its one-time registration/initialization latch. Each mode independently rejects duplicate, reordered, missing, or foreign IDs and any owner-token replacement before mutation.
- At most one boss Transfer target is exposed after each successful scheduler commit. Audit exposure is `sink.IsExposed=true` plus the exact authored collider enabled; all payloads are hidden. Payload exposure keeps audit hidden. Empty forecasts hide all four.
- A normal audit exposure end queues one `TransferTargetExposureEnded(BossAuditBox-0, t+1)` through the same owner lane. Registration is preserved. If the audit target is active, the next `-200` produces exactly one `TargetRemoved` clear, one revision increment, player Baseline restoration, sink clear, and no permanent removed-ID entry.
- Stale, repeated, wrong-horizon, simultaneous payload+audit, foreign target, wrong kind/profile/owner/collider, duplicate registration, and cross-wired bridge/Transfer inputs reject before any sink, collider, queue, or latest-outcome mutation.

### Terminal cleanup without changing M4B2 evidence

- Exact M4A death handoffs at source tick `t` force both forecasts empty and hide audit/payload targets at `-180(t)`.
- In production configuration the scheduler performs one atomic `MergeRemovals` for transfer tick `t+1` containing exact ordinal IDs `BossAuditBox-0`, `BossWeight-0`, `BossWeight-1`, `BossWeight-2`. The actual queued `TransferSimulationInput.Removals` contains all four removals and the queue is replaced exactly once. Because the existing publication/outcome contracts expose only removal count and resulting effects, not removal IDs, add only a driver-owned internal defensive read seam for the exact removal IDs of the latest completed phase input; do not change `TransferPhasePublication`, `TransferTickOutcome`, or `TransferPhaseInputSummary` shape.
- Existing `OrdanBossTransferExposureOutcome.PermanentlyRemovedTargetIds`, its constructor/ABI, and all existing M4B2 three-payload tests remain unchanged and continue to expose exactly `BossWeight-0`, `BossWeight-1`, `BossWeight-2`. That legacy value is a payload-only scheduler projection, not the complete Transfer removal batch.
- Add a separate immutable `OrdanBossAuditExposureOutcome(sourceTick, transferTick, previousTargetId, exposedTargetId, temporarilyEndedTargetIds, permanentlyRemovedTargetIds, isTerminal)`. Nonterminal lists are empty or the one exact audit ID. Terminal temporary ends are empty and permanent removals contain only `BossAuditBox-0`.
- The combined four-ID removal is fully validated before the single queue replacement. If any ID, tick, binding, registry membership, or outcome shape fails validation, the Transfer queue, audit/payload sinks and colliders, both scheduler latest outcomes, and latest handoff view remain unchanged. A failure cannot leave only payload or only audit removal queued.
- At `-200(t+1)`, when no lifecycle transition is queued, all four production boss Transfer descriptors become permanently removed. An active audit Transfer clears exactly once as `TargetRemoved` with one revision increment; an inactive/hidden audit target is removed without a sink clear or revision increment. If a verified lifecycle transition is queued for that same Transfer tick, existing lifecycle-first authority wins: the active transfer clears exactly once with the lifecycle reason, removal processing is skipped, the four descriptors are not claimed as permanently removed by this phase, and no second `TargetRemoved` clear or revision increment occurs. The phase publication's input summary/read seam may still show the queued four-ID removal batch while the tick outcome shows only the lifecycle clear; evidence must distinguish queued input from processed effect. Legacy configuration retains its verified exact three-payload behavior. The M4B2 terminal latch still rejects every later scheduler advance. M4B3A's last immutable death view remains preserved.
- This unit does not consume or clear `Defeated`, `RewardRequest`, or `RoomCompletionRequest` and does not disable the encounter root.

### Presentation and authoring

- `OrdanBossEncounterPresentationView` additively carries exact value-only audit forecast/exposed/temporary-end/permanent-removal IDs. No Unity reference escapes.
- The M4B3A builder wires the audit target/sink/collider into the existing scheduler through a typed assignment-only authoring seam. The validator proves exact shared registry membership, component identity, initial hidden state, and scheduler binding without initialization or repair.
- The prefab hierarchy, component order, transforms, geometry, player prefab, scene linkage, execution orders, and stable ID order remain unchanged. Only the explicitly allowlisted scheduler audit references and additive immutable value fields may change; no YAML byte-identity claim is made.

## Requirements covered

- `REQ-COM-003`: `BalanceAudit` gains its authored Transfer interrupt window without giving Unity phase authority.
- `REQ-COM-004`: one forecast owner, one exposure owner, exact tick continuity, and atomic queue replacement prevent duplicated lifecycle effects.
- `REQ-WT-003`, `REQ-WT-005`: exact audit availability, modifier ownership, temporary clear, revision, and permanent removal semantics are preserved.

## Acceptance evidence required

### EditMode

1. `AC-COM-002`, `AC-COM-003`: forecast construction/equality/defensive value semantics; every pattern and phase boundary; all three health-stage Audit durations; interrupt, death, overflow, forged candidate, stale/skipped tick, and simultaneous payload/audit rejection.
2. `AC-WT-003`, `AC-WT-005`: audit outcome shape/equality, hidden/apply/clear/reapply, legal increasing revision gaps, stale/duplicate rejection, wrong ID/kind/profile/owner/collider, and state-preserving rejection.
3. `AC-COM-003`, `AC-WT-005`: exact four-ID production lane registration, explicit legacy three-payload compatibility, authored-production rejection of the legacy seam, no mode inference or post-latch switch, per-mode duplicate owner/registration, missing/reordered/foreign ID, and combined-removal atomicity.
4. `AC-COM-003`: builder/validator exact binding mutation matrix and assignment-only inactive authoring checks. Static checks prove no new discovery, physics callback/query, RNG, delta-time, coroutine, public ABI, damage, movement, teardown, scene, reward, room, run, or input authority.

### PlayMode

1. `AC-COM-002`, `AC-WT-005`: fresh authored graph proves audit hidden through prior patterns, exposed for `BalanceAudit.Execute` age `0` through the last legal Execute tick, and hidden before Recovery.
2. `AC-COM-002`, `AC-COM-003`: a legal audit transfer at Execute age `0` produces one rising edge, exact `BalanceAuditPull` then `BalanceAuditInterrupt` intent order, immediate Vulnerable state, next-tick temporary clear, player Baseline, and later same-ID reuse.
3. `AC-COM-003`: absent/stale/duplicate/cross-wired forecasts, simultaneous payload/audit exposure, and forged audit bindings preserve all four availability bits, queued input, revision, M4A output, scheduler latest outcomes, and handoff latest view.
4. `AC-COM-003`, `AC-WT-005`: death with audit inactive and death with audit active both queue one exact four-ID removal batch. Without a same-tick lifecycle transition, the next Transfer phase removes all four, clears an active audit once, preserves the death triplet and last terminal view, and rejects later scheduler advance. The inactive case proves no audit sink clear or revision increase. Both cases prove the real queued input and driver-owned completed-input read seam have four IDs while the unchanged M4B2 payload outcome retains exactly three payload IDs and the new audit outcome retains exactly one audit ID.
5. `AC-COM-003`, `AC-WT-005`: a terminal four-ID batch combined with each supported same-tick lifecycle transition preserves lifecycle-first authority: the queued/completed-input read seam still identifies four requested removals, the tick outcome contains only the one lifecycle clear, no permanent-removal result is claimed, there is no second clear/revision, and all scheduler/handoff terminal latches remain stable.
6. Independent fresh-graph synthetic 30/60/144 render observations run the same fixed-tick script and compare at matching simulation ticks: both forecasts, both scheduler outcomes, active Transfer state/revision, four availability bits, boss snapshot/intents/handoffs, terminal state, and stable IDs.

M4B3B1 remains partial evidence for `AC-COM-002` and `AC-COM-004`. Hostile geometry/damage, pull movement, lifecycle consumption, teardown, and any later handoff-field additions remain unclaimed. The Approved SYSTEM-CONTRACTS amendment and `docs/README.md` index must state this split before implementation begins.

## Ollama utilization record

| Lane | Outcome | Sol screening |
|---|---|---|
| GLM 5.2 | used and accepted in part | Accepted stale-forecast, hidden-collider, movement-authority, teardown-order, and terminal-latch risks. Rejected its same-tick rollback recommendation because verified phase commits are not globally rolled back. |
| Kimi K3 | used and rejected | Its immutable/fixed-math direction was generic, but it invented time fields, identifiers, fixed-point types, and direct movement merge schemas outside this unit. No code or schema is adopted. |
| MiniMax M3 | used and rejected | Its fixture categories were useful prompts, but it invented random generation, polygon rules, render interpolation, phase meanings, GC/audio teardown, and visual pixel oracles outside the frozen system. No proposal code is adopted. |

All cloud prompts were abstract and non-sensitive. Terra or Luna must screen any later code-bearing output before Sol integration.

## Allowlist

- `Assets/AcadeGameMaker/Runtime/Combat/OrdanBossSession.cs`;
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossSimulationDriver.cs`;
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTransferExposureScheduler.cs`;
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossEncounterHandoffAdapter.cs`;
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferSimulationDriver.cs`;
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringBuilder.cs`;
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringValidator.cs`;
- `Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab` and `Assets/Scenes/OrdanBossEncounterSandbox.unity`, only through the approved builder, plus Unity-generated `.meta` changes if any;
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/OrdanBossSessionTests.cs`;
- `Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/OrdanBossTransferExposureTests.cs`;
- `Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/OrdanBossEncounterAuthoringTests.cs`;
- `Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/OrdanBossAuditExposureTests.cs` and `.meta`;
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossTransferExposurePlayModeTests.cs`;
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossEncounterHandoffAdapterPlayModeTests.cs`;
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossEncounterAuthoredGraphPlayModeTests.cs`;
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossAuditExposurePlayModeTests.cs` and `.meta`;
- this contract, its pre-gate and implementation evidence, `SYSTEM-CONTRACTS.md`, and the M4A/M4B2/M4B3A documentation amendments indexed from `docs/README.md`.

## Stop conditions

Stop and return to Sol if implementation requires snapshot/phase-age reconstruction outside M4A, a second Transfer owner, a second Combat owner, runtime registry freeze/rewrite, normal audit permanent removal, changing the existing payload terminal outcome's exact three-ID evidence, multiple queue replacements for terminal removal, same-tick rollback, Physics2D query/callback authority, hostile damage, movement pull, direct Transform/Rigidbody mutation, encounter teardown, public ABI, player/regular graph modification, project/build settings, or reward/room/run/input/scene consumption.

## References

- [M4A Ordan core](./2026-09-05-vd03-combat-m4a-ordan-boss-core.md)
- [M4B1 Unity Combat bridge](./2026-09-05-vd03-combat-m4b1-ordan-unity-combat-bridge.md)
- [M4B2 Transfer exposure](./2026-09-06-vd03-combat-m4b2-ordan-transfer-exposure.md)
- [M4B3A authored graph](./2026-09-06-vd03-combat-m4b3a-ordan-authored-graph.md)
- [System contracts](../vertical-demo/SYSTEM-CONTRACTS.md)
