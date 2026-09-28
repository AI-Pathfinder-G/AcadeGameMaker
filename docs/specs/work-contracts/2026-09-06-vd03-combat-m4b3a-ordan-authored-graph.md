# Work Contract: VD-03 M4B3A Ordan Authored Boss Graph

- Status: Verified
- Owner: Terra
- Contract approval/integration: Sol
- Verification: Luna
- Date: 2026-09-06
- Approved by: Sol
- Approved: 2026-09-06
- Parent: [VD-03 Combat and Enemies](../vertical-demo/03-combat-and-enemies.md)

## Purpose and split

M4B3A activates the Verified M4A/M4B1/M4B2 Ordan stack in one fresh, explicitly wired prefab and standalone Unity scene. It adds the exact authored boss graph, a production-safe audit-box modifier sink, a read-only authoring validator, and a presentation-neutral immutable handoff view.

M4B3 is intentionally split. M4B3A does **not** infer or add `BossAuditBox-0` exposure, move scripted payloads, produce DebtLine or BalanceAudit player damage, apply pull movement, consume reward/room requests, transition a scene, or supply final art/audio. M4B3B must separately freeze the M4A-owned audit forecast, the single Transfer exposure lane extension, authored geometry, occlusion, future player-damage merge lane, and teardown. This prevents scene authoring from silently changing M4A, M4B1, M4B2, VD-07 input, or M1 authority.

## Sol decisions frozen for M4B3A

### Fresh isolated artifacts

- Author `Assets/Scenes/OrdanBossEncounterSandbox.unity` from a new empty scene and `Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab` from a fresh root.
- Reuse `Assets/Prefabs/Player/CombatEncounterPlayer.prefab` read-only. Do not rebuild or modify it. The nested instance in the boss graph may contain only Unity's root-name and local-pose property modifications needed to parent and place it: `m_Name=CombatEncounterPlayer`; `m_LocalPosition=(-7,0.81,0)`; quaternion `m_LocalRotation=(0,0,0,1)`; `m_LocalEulerAnglesHint=(0,0,0)`. Unity versions may collapse redundant identity/zero entries when reading the outer scene instance; absent redundant entries are equivalent only when the connected source identity and direct authored pose are exact. Added/removed components or GameObjects, object-reference modifications, and every other property modification are forbidden.
- Do not modify `Bootstrap.unity`, `MovementSandbox.unity`, `CombatEncounterSandbox.unity`, any existing prefab, assembly definition, `ProjectSettings`, `EditorBuildSettings`, package manifest, public API, or verified regular-enemy pipeline.
- The scene contains exactly one linked, connected prefab instance of `OrdanBossEncounterGraph`; it is never unpacked. Unity serializes the root instance's exact default name/local-pose property set, so only `m_Name=OrdanBossEncounterGraph`, `m_LocalPosition=(0,0,0)`, identity `m_LocalRotation=(0,0,0,1)`, and zero `m_LocalEulerAnglesHint=(0,0,0)` are allowed. Object-reference modifications, added/removed components or GameObjects, reparenting, and every other property modification are forbidden. Validation proves `PrefabInstanceStatus.Connected`, exact source-prefab identity, and this exact default set. The scene is not added to build settings in this milestone.

### Exact root and authored geometry

The graph root is literal `OrdanBossEncounterGraph`, active, layer `Default`, local position/rotation zero and scale one. It has exactly eight direct children in this order:

1. prefab instance `CombatEncounterPlayer` at `(-7.0, 0.81, 0)`;
2. `Ordan` at `(7.0, 1.20, 0)`;
3. `BossAuditBox-0` at `(0.0, 0.75, 0)`;
4. `BossWeight-0` at `(3.0, 6.0, 0)`;
5. `BossWeight-1` at `(5.0, 6.0, 0)`;
6. `BossWeight-2` at `(7.0, 6.0, 0)`;
7. `Systems` at zero;
8. `Environment` at zero.

All authored rotations are zero and XY scales are one. All actor/target roots use layer `Default`; environment colliders use the existing literal layer `8` convention. Placeholder geometry is contract data for validation, not final art:

- Ordan: kinematic `Rigidbody2D`, frozen rotation, no interpolation, gravity scale `0`; non-trigger `BoxCollider2D` size `(1.6,2.4)`, offset zero; Combat aim point/shape center `(0,0)` Q1000 and half extents `(800,1200)` Q1000.
- Audit box: kinematic `Rigidbody2D`, frozen rotation, no interpolation, gravity scale `0`; non-trigger `BoxCollider2D` size `(1.2,1.2)`, offset zero; Transfer aim point/shape center `(0,0)` Q1000 and half extents `(600,600)` Q1000. Its collider starts disabled and its sink starts unavailable/unapplied.
- Each scripted payload: kinematic `Rigidbody2D`, frozen rotation, no interpolation, gravity scale `0`; non-trigger `BoxCollider2D` size `(1.0,1.0)`, offset zero; Transfer aim point/shape center `(0,0)` Q1000 and half extents `(500,500)` Q1000. Each collider and sink starts hidden/unavailable as required by M4B2.
- Environment has only its `Transform` component and exactly `Ground` at `(0,-0.5)` size `(24,1)`, `LeftBound` at `(-12,4.5)` size `(1,11)`, and `RightBound` at `(12,4.5)` size `(1,11)`. Each child has serialized component order exactly `Transform, BoxCollider2D`; the collider is non-trigger and no runtime behavior is present.

No renderer, animator, material, camera, light, audio source, tag, runtime-spawned object, physics material, trigger collider, or final asset is required or permitted by M4B3A.

### Exact Combat and Transfer identities

- `Ordan` has one `CombatTarget`: ID `ordan`, max health `60`, invulnerability `0`, player basic attack enabled, and the exact authored collider/pose geometry above. It has no `TransferTarget` and no transfer sink.
- The Combat registry has exactly `ordan`, then `player`. It contains no other target and freezes before first boss Combat publication.
- The Transfer registry has exactly `BossAuditBox-0`, `BossWeight-0`, `BossWeight-1`, `BossWeight-2` in ordinal order before the first Transfer session is created.
- The audit target kind is `Box`, base profile is exact literal `Boss.AuditBox.Scripted.v1`, and heavy modifier is exactly `Target.Box.Heavy.v1`.
- Each payload target kind is `BossPayload`, base profile `Boss.Payload.Scripted.v1`, and sink modifier `Target.BossPayload.ScriptedHeavy.v1`.
- `OrdanBossAuditTransferModifierSink` owns only the audit target's exposed/applied bits and last accepted Transfer revision. It mirrors M4B2 payload-sink validation: exact target identity, exact Box kind/base/modifier, strictly increasing revision, unavailable while hidden, idempotent state clear, and no Rigidbody mass/gravity, damage, movement, registry, phase, or exposure-policy authority. M4B3A leaves it hidden; only M4B3B may give the existing single scheduler an M4A-owned audit forecast and call its exposure commit.

### Exact Systems root

`Systems` has Transform and exactly one of each component below, in serialized component order:

1. `TransferTargetRegistry`;
2. `CombatTargetRegistry`;
3. `TransferSimulationDriver` (`DefaultExecutionOrder -200`);
4. `OrdanBossCombatSimulationDriver` (`-190`);
5. `OrdanBossSimulationDriver` (`-185`);
6. `OrdanBossTransferExposureScheduler` (`-180`);
7. `OrdanBossEncounterHandoffAdapter` (`+110`).

All bindings are explicit serialized references established by the builder through typed internal `ConfigureForAuthoring` seams that share the same assignment implementation as focused test seams. This includes the existing payload sink, which receives an additive `ConfigureForAuthoring(targetId, baseModifierProfileId)` seam and retains `ConfigureForTests` as a caller label over the same private assignment function. These additive seams may not initialize, validate, discover, freeze, publish, alter exposure/applied/revision state beyond the same pristine assignment reset already used by tests, or mutate gameplay state. The boss Combat owner list is exact length one and contains only its own `OrdanBossCombatSimulationDriver` instance. The three payload target/sink arrays are exact ordinal `0,1,2` and belong to the same Transfer registry.

The scheduler additively exposes internal read-only `BoundBridge` and `BoundTransferDriver` identity seams. They return only existing serialized references and may not initialize or mutate. The handoff adapter rejects any publication whose scheduler bindings do not reference its exact serialized bridge and Transfer driver, or whose drivers do not converge on the exact serialized player, registries, targets, sinks, and graph root.

No regular `CombatSimulationDriver`, regular reaction/behavior/locomotion/threat driver, regular encounter handoff, second boss driver, second scheduler, input reader, scene loader, lifecycle consumer, or unknown `MonoBehaviour` may exist below the graph.

Target component order is also exact. `Ordan` is Transform, Rigidbody2D, BoxCollider2D, CombatTarget. `BossAuditBox-0` is Transform, Rigidbody2D, BoxCollider2D, OrdanBossAuditTransferModifierSink, TransferTarget. Each `BossWeight-*` is Transform, Rigidbody2D, BoxCollider2D, OrdanBossPayloadTransferModifierSink, TransferTarget. The reused player nested prefab must retain its existing authored component order and may have only the exact name/local-pose property modifications frozen above.

### Presentation-neutral handoff

`OrdanBossEncounterHandoffAdapter` is a read-only consumer at `+110`. It uses explicit bindings to the player, Transfer driver, boss Combat driver, boss bridge, exposure scheduler, Ordan target, audit target/sink, three payload targets/sinks, and their authored colliders.

- It publishes only after exact current-tick completed Transfer, Combat, bridge, and scheduler outputs agree on tick and graph identity.
- Its immutable view defensively copies the exact M4A snapshot, ordered intents, ordered handoffs, Combat health/death snapshots, active Transfer target ID/state, forecasted/exposed payload ID, audit/payload availability bits, and authored stable IDs. No Unity object reference escapes.
- It does not read render time, mutate Transform/Rigidbody/Collider/Renderer, perform a physics query, submit gameplay input, queue damage, change target availability, grant reward, complete a room, lock input, transition a scene, or synthesize a handoff.
- First-tick validation/publication is atomic. Missing, stale, skipped, duplicate, cross-wired, partially initialized, or foreign publication rejects before the adapter updates latest state.
- Defeated view preserves M4A's exact ordered one-shot `Defeated`, `RewardRequest`, `RoomCompletionRequest` values on the death tick. Verified M4B2 then latches terminal and rejects every later scheduler advance, so M4B3A retains that last immutable terminal view and must not synthesize a later empty publication. M4A's engine-free evidence remains the authority for later empty core handoffs; M4B3B/later lifecycle work owns object hiding and teardown.

### Builder and validator

- `OrdanBossEncounterAuthoringBuilder` is the only asset writer. It builds inactive, explicitly wires every object, validates before saving the prefab, instantiates the saved prefab into the named scene, validates again, and then saves. It contains no test seam and no runtime discovery.
- `OrdanBossEncounterAuthoringValidator` is read-only and root-bounded. It validates exact child order, names, sibling indices, transforms, layers, components and component order; prefab linkage and zero overrides; exact scalar geometry; registry order; cross-system reference identity; driver execution-order attributes; initial hidden states; forbidden components; and asset-path identity.
- Validation never calls gameplay initialization, `Awake`, `FixedUpdate`, registry freeze, asset save, runtime repair, `Find*`, `Resources.Load`, name-based fallback, or a broad scene search.

## Requirements covered

- `REQ-COM-001`: the authored graph preserves basic-attack access to Ordan and the separately targetable payload route.
- `REQ-COM-003`: the exact boss objects and presentation handoff expose the deterministic pattern's readable substrate without inventing behavior.
- `REQ-COM-004`: exact graph identity, atomic first publication, immutable handoffs, and one owner per authority prevent duplicate lifecycle effects.
- `REQ-COM-006`: authored Ordan health/identity and exact M4A/M4B execution stack are activated without changing timing.
- `REQ-WT-003`, `REQ-WT-005`: exact target geometry, registry identity, sink ownership, availability, and lifecycle evidence are preserved.

## Acceptance evidence required

### EditMode

1. `AC-COM-002`, `AC-COM-003`, `AC-WT-005`: build and validate the exact prefab and scene; verify only the exact allowlisted default name/local-pose set on the scene's boss-graph instance, only the exact allowlisted name/local-pose set on the nested player instance, and all literal geometry/component/order values.
2. `AC-COM-002`, `AC-WT-003`: prove exact Combat and Transfer roster orders, target kinds/profiles, Ordan transfer exclusion, shared registry bindings, and all four initially unavailable boss Transfer targets. Separately prove `BossAuditBox-0` is registered before first Transfer session creation while its descriptor reports unavailable and its collider stays disabled, and that M4B2 neither treats it as a payload nor removes it.
3. `AC-COM-003`: mutate every structural boundary independently—missing, duplicate, reordered, renamed, cross-wired, foreign behavior, wrong layer/shape/profile/owner/order—and prove fail-stop rejection without repair or publication.
4. `AC-COM-003`, `AC-WT-005`: audit-sink revision/apply/clear/hidden/forged-request matrix and state-preserving rejection.
5. Static checks prove builder has no test seam and runtime/validator source has no discovery, runtime repair, RNG, delta time, coroutine, physics callback/query, public ABI, scene transition, reward, room, or input authority. An inactive-graph mutation test proves every new `ConfigureForAuthoring` call is assignment-only and causes no initialization, registry freeze, gameplay publication, availability change, or latest-view mutation.

### PlayMode

1. `AC-COM-002`, `AC-COM-004`, `AC-WT-005`: load a fresh prefab graph and prove exact first-tick trace `-200 -> -190 -> -185 -> -180 -> +110`, pre-registered rosters, and initially hidden audit/payload targets.
2. `AC-COM-002`, `AC-COM-003`: advance through DebtLine and the first SeizureWeight Telegraph; prove view grouping, exact forecast/exposure identity, payload `0` becoming targetable only through M4B2, no audit exposure, and no presentation-side mutation.
3. `AC-COM-003`: prove immutable collection copies, stale/skipped/cross-wired output rejection—including a scheduler bound to a different same-tick bridge or Transfer driver—failed first publication atomicity, and no latest-view mutation after rejection.
4. `AC-COM-003`: drive exact boss death and prove one death-tick ordered handoff triplet, no reward/room/scene/input side effect, and terminal payload removal evidence. Prove a later scheduler/adapter attempt rejects without replacing the last terminal view; do not claim or synthesize a post-terminal M4B3A publication because Verified M4B2 accepts no later scheduler advance.
5. Use synthetic render-frame groupings of `2`, `1`, and alternating `0/1` completed fixed ticks per render observation to represent 30/60/144 Hz while the simulation remains exact 60 Hz. Run the same fixed-tick input script and compare every immutable view field—tick/next tick, boss snapshot, intent and handoff order/content, Combat snapshots, Transfer active state, forecast/exposed ID, four availability bits, terminal state, and stable IDs—field-for-field at matching simulation ticks. Wall-clock render timing is not an authority and is not measured.

M4B3A remains partial evidence for `AC-COM-002` and `AC-COM-004`. Actual hostile hit geometry, audit interruption, paired six-run timing, room/reward consumption, and teardown cannot be claimed until M4B3B and later integration are Verified.

## Ollama utilization record

| Lane | Outcome | Sol screening |
|---|---|---|
| Kimi K3 | used and rejected | The proposal correctly favored immutable deterministic trajectory and explicit bindings, but invented unrelated public debt-ledger schemas and fixed-point types outside the frozen contract. No code or schema is adopted. Its useful class-boundary ideas are independently restated here under existing project types. |
| GLM 5.2 | used and accepted | Accepted as adversarial input for tick-order, body-transfer contamination, duplicate damage, scene/prefab drift, hidden collider, teardown and presentation-authority tests. Suggestions that assumed mutable intent structs or a particular adapter order were normalized to this contract. |
| MiniMax M3 | used and rejected | The validator categories and Edit/Play split were useful, but names, roster counts, tags, ScriptableObject hashes and phase values contradicted the frozen graph. No invented schema is adopted; only the independently verified drift-test categories remain. |

## Allowlist

- `Assets/Scenes/OrdanBossEncounterSandbox.unity` and `.meta`;
- `Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab` and `.meta`;
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringBuilder.cs` and `.meta`;
- `Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringValidator.cs` and `.meta`;
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossAuditTransferModifierSink.cs` and `.meta`;
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossEncounterHandoffAdapter.cs` and `.meta`;
- additive typed `ConfigureForAuthoring` assignment seams only in `OrdanBossCombatSimulationDriver.cs`, `OrdanBossSimulationDriver.cs`, `OrdanBossTransferExposureScheduler.cs`, and `OrdanBossPayloadTransferModifierSink.cs`, plus read-only scheduler `BoundBridge`/`BoundTransferDriver` identity properties;
- `Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/OrdanBossEncounterAuthoringTests.cs` and `.meta`;
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossEncounterAuthoredGraphPlayModeTests.cs` and `.meta`;
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossEncounterHandoffAdapterPlayModeTests.cs` and `.meta`;
- this contract, its Luna pre-gate, implementation evidence, `docs/README.md`, and the M4B3A additive `SYSTEM-CONTRACTS.md` amendment.

Generated scene/prefab YAML and `.meta` files are allowed only when written by Unity after the builder and focused tests compile. Test XML and Unity logs are execution evidence, not implementation files.

## Stop conditions

Stop and return to Sol if implementation requires audit exposure inferred from phase age, a second Transfer/Combat owner, runtime registry mutation or re-freeze, same-tick rollback, geometry damage submission, movement pull, physics callback authority, scene discovery/repair, public ABI, modification of the regular graph or existing player prefab, build/project settings, final assets, reward/room/run/input consumers, or any file outside the allowlist.

## References

- [VD-03](../vertical-demo/03-combat-and-enemies.md)
- [VD-02](../vertical-demo/02-weight-transfer.md)
- [M4A Ordan core](./2026-09-05-vd03-combat-m4a-ordan-boss-core.md)
- [M4B1 Unity bridge](./2026-09-05-vd03-combat-m4b1-ordan-unity-combat-bridge.md)
- [M4B2 transfer exposure](./2026-09-06-vd03-combat-m4b2-ordan-transfer-exposure.md)
- [System contracts](../vertical-demo/SYSTEM-CONTRACTS.md)
- [Luna contract pre-gate](../../verification/2026-09-06-vd03-combat-m4b3a-contract-pregate.md)
