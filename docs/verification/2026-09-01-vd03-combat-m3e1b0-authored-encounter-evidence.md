# VD-03 M3E1B0 Authored Regular-Enemy Encounter Evidence

- Date: 2026-09-01
- Contract: `docs/specs/work-contracts/2026-09-01-vd03-combat-m3e1-authored-encounter-composition.md`
- Stage: M3E1B0 collision-neutral provisional authored graph
- Status: Verified — Luna PASS (`P0=0`, `P1=0`, `P2=0`); B1 implementation authorized
- Requirements: `REQ-COM-001`, `REQ-COM-004`; affected `REQ-WT-002`, `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance evidence: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-003`, `AC-WT-005`

## Result

The allowlisted `CombatEncounterGraph` prefab and `CombatEncounterSandbox` scene now persist one exact collision-neutral B0 graph with literal `player`, `surveyor` and `walker` roles. The graph contains the complete Player/Transfer/Combat/Reaction/Behavior/Locomotion/Threat chain, two production enemy modifier sinks, exact ordered registries and zero Encounter Handoff drivers. B1 remains explicitly unavailable in both the builder and validator.

The corrected Gate 7 test first instantiates the committed graph, submits exact lethal Combat inputs at `t0`, and lets the production phase order register Transfer targets, kill both regular enemies and schedule removal. At `t1`, production Transfer consumes exactly two removals; the test proves both enemies are dead, the removal publication is exact and the former two-registration set is gone. It then places both exact player/enemy collision pairs in their post-death ignored state, verifies them and destroys the instance. After a fresh instantiation, a test-only `[DefaultExecutionOrder(-195)]` observer captures state after production Transfer `-200` and before default-order Movement: the Transfer owner again holds exactly `surveyor`, `walker` registrations, Player `NextExpectedTick` is still `0`, and both pair flags are false. The observer captures exactly once and does not initialize or mutate production state. This supplies provisional collision-gate item 7 evidence; it does not implement the B1 `+110` adapter or production pair-ignore mutation.

## Implementation boundary

- The Editor-only builder creates only the new allowlisted player, walker, surveyor, encounter graph and encounter scene assets.
- The root-bounded validator checks the exact five direct children and order, literal names, active/layer state, local transforms, component membership, scalar/body/collider profiles, sole-solid actor geometry, registry order, cross-driver identity, execution order, exact environment children/positions/sizes, collision-neutral pair state and the exact 16 allowed B0 `MonoBehaviour` instances.
- Production drivers gained internal noninitializing authoring and exact graph-validation seams. The builder contains no `ConfigureForTests` call.
- `RegularEnemyTransferModifierSink` owns only the Heavy modifier bit and last accepted revision. It does not own health, death, movement, threat, collision or Combat state.
- Transfer fresh-instance verification exposes only a read-only exact-registration predicate. Combat authoring validation builds a defensive immutable roster and does not populate the runtime frozen-roster cache.
- The authoring assembly has exactly seven explicit runtime references and seven corresponding `InternalsVisibleTo` grants. It creates no public ABI and no assembly-reference cycle.
- No runtime discovery, role fallback, authoring repair, new physics query, `SyncTransforms`, `IgnoreCollision`, `DefaultExecutionOrder(110)`, package, build-scene or project-setting change was introduced.

## Unity serialization corrections found during construction

The first builder attempt correctly rejected an unnamed working scene because stable Combat/Transfer identities require a named scene and hierarchy. The builder was corrected to save the allowlisted final scene path before constructing targets; a prohibited `SceneManager.CreateScene` retry was removed.

The next attempt exposed two Unity-specific persistence defects rather than gameplay defects:

1. `MonoImporter.GetExecutionOrder` returned only an Editor override and reported `0` despite runtime `[DefaultExecutionOrder]` attributes. The validator now checks the exact attributes on each runtime component type and requires the Player attribute to be absent for default order `0`.
2. `TransferTargetRegistry` and `CombatTargetRegistry` were secondary `MonoBehaviour` classes in other source files, so the saved prefab contained `m_Script: {fileID: 0}` and fresh instantiation lost both components. Their class bodies were moved unchanged into matching `TransferTargetRegistry.cs` and `CombatTargetRegistry.cs` assets. Regression tests verify exact `MonoScript` asset paths, and the final graph contains zero missing script references.

These corrections did not change registry API, state, validation, configuration or runtime semantics. The final builder run completed successfully and saved the graph and scene.

## Authored asset hashes

| Asset | SHA-256 |
|---|---|
| `Assets/Prefabs/Combat/CombatEncounterGraph.prefab` | `D926020A53F6597FB615873B0D87291D13B9940EAD45E53C5E6F1DAF7B89733E` |
| `Assets/Prefabs/Player/CombatEncounterPlayer.prefab` | `68FFFA9384412ED8C1326E2928515145251E22AD6AADBA3584E56E736AE07247` |
| `Assets/Prefabs/Enemies/Walker.prefab` | `60811E65A9ABC2D384BA16F6945AC187FE0DDD9BB00AA21FAA48E4CC7EC9398A` |
| `Assets/Prefabs/Enemies/Surveyor.prefab` | `D547112A521F0EF9574C14C71C443A6C07C2DF369495DB8AD12F7C1D6D640909` |
| `Assets/Scenes/CombatEncounterSandbox.unity` | `E5A302CFD146AE660F343D3E96B66B4AEF103E2EBD20B73498F197BC889024EE` |

## Executable evidence

| Run | Result | XML SHA-256 | Criteria |
|---|---:|---|---|
| Authored asset and 70-case mutation EditMode focus | 79/79 passed, 0 failed, 0 skipped | `55C71D75FC0D668801F41CC0DA1409112C60E68A72FC76F9956E186CC8315D52` | `AC-COM-001`, `AC-COM-003`, affected `AC-WT-003`, `AC-WT-005` |
| Production death/removal and fresh graph Gate 7 PlayMode focus | 1/1 passed, 0 failed, 0 skipped | `F45D2A15B21D74489B91034388584C2D44AFD2C452B8C53F414274BF0CD0CDC8` | provisional collision gate 7; `AC-COM-001`, `AC-COM-003`, affected `AC-WT-003`, `AC-WT-005` |
| CombatUnity EditMode | 87/87 passed, 0 failed, 0 skipped | `30D6B7DF0807977AE323AB02D7B13F45EBB2A103C37D92C31E6D6CC0DD3B1833` | focused regression |
| CombatUnity PlayMode | 241/241 passed, 0 failed, 0 skipped | `482E4A516AA92B09A0DA21A2303B1DD58A3DAC7A079AECBA8CDA75C45EEE6905` | focused regression |
| Full EditMode after Movement test isolation | 265/265 passed, 0 failed, 0 skipped | `50624AC92D3C358225482ED3C853E1585BB46B57BA46437B16FA999BBF7D6305` | project regression and protected-scene preservation |
| Full PlayMode | 275/275 passed, 0 failed, 0 skipped | `05372DF4B4BCD3D7243DC017F1C2581154C321721A006E0B7350B607A552BB83` | project regression |
| Movement authoring isolation focus | 3/3 passed, 0 failed, 0 skipped | `9167B43749B6FD48B750C0ADB9200BE96684FAD19D1F060464992C205D2558C2` | `AC-MOV-001`, `AC-MOV-002`, `AC-MOV-005`; production scene/prefab hashes unchanged |

## Ollama utilization record

- **Kimi K3 — used and accepted in part:** its bounded explicit graph-validator experiment informed missing/duplicate/cross-wired binding cases. Terra rewrote the implementation against the approved contract; no repository material or authority was delegated.
- **GLM 5.2 — used and accepted in part:** its adversarial `t -> t+1`, stale state, cross-wiring, extra physics/sync and presentation-authority cases were screened into the contract and source/test checks. Invented ownership and reset behavior was rejected.
- **MiniMax M3 — failed and replaced:** the bounded Editor-validator request returned an empty final response without quota, authentication, availability or network error. Terra supplied the deterministic authoring fixture and Luna reviews it independently.

## Protected workspace state

The implementation allowlist excluded the preexisting user-owned paths below. The correction audit found that the legacy full-EditMode suite nevertheless invoked `MovementSandboxAuthoringBuilder.Build()` and rewrote the protected Movement scene. The exact state is:

| Protected path | Pre status | Post status | Working SHA-256 | HEAD state |
|---|---|---|---|---|
| `Assets/Scenes/MovementSandbox.unity` | legacy test changed SHA from `6843D9FFCC74E553FF337167A42E397842AC305A4100584D25E04E70B4A7D4E9` to `1EF6378C459ACF3364E1F2D4AEA96D43C6E8A99942530B21030C8AF734BDE9A8`; user approved current bytes as new baseline | `M`, SHA `1EF6378C459ACF3364E1F2D4AEA96D43C6E8A99942530B21030C8AF734BDE9A8` before and after focused plus full EditMode isolation runs | approved current Git blob `4d3beb2dbd0011a4874d53d6e25e1b61eb4766c6` | HEAD blob `1c04252cd41e71aeb23c3d624400edeba1da8dc5` |
| `ProjectSettings/URPProjectSettings.asset` | `M` | `M` | `A3A626CB529CCFC0A82E388B8CD32BC60888F7226C33412FAD3BC50ABA802CD7` | blob `4d385d1c495f6d0891aed16670197dd420bea5c1` |
| `ProjectSettings/SceneTemplateSettings.json` | `??` | `??` | `BB9098B3BFCDE78D93E264B96F7B77B5430E64E8C57A7AAD7F5A5D9C3945E16A` | untracked before and after |

Unity XML and log files remain untracked execution artifacts and are excluded from integration.

## Reopened review correction

An initial Luna review passed the B0 diff and six executable artifacts. A second, stricter contract audit then found two blocking evidence gaps, which Sol accepts:

1. the destroyed first instance only used a test-only collision-pair stand-in and did not execute the production registration, scripted enemy death and Combat-owned `t+1` Transfer removal path; therefore the test did not yet prove restoration after the actual post-death registration loss;
2. the validator and nine-test authoring suite did not yet exhaust the contract's exact direct-child hierarchy/environment geometry and every-role/every-driver missing, duplicate, cross-wired and foreign mutation matrix.

The second review result was `P0=0`, `P1=2`, `P2=1`. The functional correction now executes real production death and `t+1` registration removal before fresh restoration and adds a 70-case mutation matrix covering missing, duplicate, cross-wired, foreign, hierarchy, transform, active/layer and geometry changes. All six corrected executable runs pass, and both Luna reviewers accept the functional Gate 7 evidence.

The protected-scene mismatch caused a blocking `P1`. The user approved the regenerated current scene as the new baseline. The legacy Movement EditMode test now routes builder verification to a fail-closed temporary test root and checks production scene/prefab SHA-256 before and after. Its focused 3/3 run and the complete 265/265 EditMode run preserve the approved scene hash, preserve the player-prefab hash and leave no temporary root.

Luna's final independent rereview found `P0=0`, `P1=0`, `P2=0`. Gate 7, the B0 authored graph, and the protected-path isolation correction all pass. Sol accepts B0 integration and authorizes B1 implementation; final M3E1 acceptance remains contingent on the approved B1 contract and its own executable evidence.
