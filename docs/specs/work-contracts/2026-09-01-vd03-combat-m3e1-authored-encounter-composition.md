# Work Contract: VD-03 M3E1 Authored Regular-Enemy Encounter Composition and Presentation Handoff

- Status: Approved — collision-gate sequencing addendum
- Initially approved by Sol: 2026-09-01 after Luna pre-gate PASS (`P0=0`, `P1=0`)
- Sequencing addendum reapproved by Sol: 2026-09-01 after Luna rereview PASS (`P0=0`, `P1=0`)
- Owner: Sol
- Unit design and implementation: Terra
- Independent verification: Luna
- Owning specs: VD-03, with affected VD-01, VD-02, VD-04, VD-07 and VD-08 boundaries
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-MOV-001`, `REQ-WT-002`, `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-003`, `AC-WT-005`

## Purpose and truthful milestone boundary

M3E1 authors one standalone regular-enemy encounter sandbox that composes the already verified Movement, Transfer, Combat, Reaction, Behavior, Locomotion and Threat pipeline with literal `player`, `walker` and `surveyor` roles. It adds an immutable presentation/lifecycle handoff and proves that the committed asset graph boots, advances, distinguishes both regular enemies and latches defeat/clear requests once. Same-instance post-death reset/re-entry is not claimed because the verified Transfer owner removes dead co-authored registrations; post-death re-entry creates a fresh scene instance under a later VD-04 lifecycle contract.

M3E1 does not implement production device input, simulation camera publication, HUD, final art, reward inventory, VD-04 room ownership or scene transition. Therefore it cannot close `AC-COM-001` or `AC-COM-003`; it supplies partial authored-composition evidence for those criteria. Human-controlled completion remains blocked on VD-07 and actual reward/room completion consumption remains blocked on VD-04.

## Milestone parts

1. **M3E1A — lifecycle and physical collision-exclusion feasibility:** an engine-free encounter lifecycle session previews/commits exact one-shot defeat, reward-request and encounter-clear handoffs from immutable Combat values. Before authored-graph work, a Unity 6.3 PlayMode spike proves collision-gate items 1–6 and 8: exact `Physics2D.IgnoreCollision(playerCollider, deadEnemyCollider)` pair state removes only that enemy from the existing player `Collider2D.Cast(ContactFilter2D.NoFilter)` path, preserves environment and nonignored hits, preserves frozen geometry/revalidation and produces a nonvacuous cadence-independent trace. This spike contains no production pair-ignore code and cannot claim item 7.
2. **M3E1B0 — collision-neutral provisional authored graph:** only after M3E1A passes, the new scene/prefab graph, production enemy sink, Editor builder and validator may be authored with both pair flags required false, no production pair-ignore mutation and exactly zero Encounter Handoff drivers. The explicit B0 validation stage requires the complete upstream Player/Transfer/Combat/Reaction/Behavior/Locomotion/Threat graph and both sinks but cannot reference or infer the absent B1 component. The committed provisional graph is then destroyed and freshly instantiated to prove collision-gate item 7: complete Transfer registration and false pair state exist before the first movement publication.
3. **M3E1B1 — lifecycle and presentation handoff integration:** only after provisional B0 item 7 passes, a `+110` adapter may consume exact completed publications, commit M3E1A lifecycle state, reflect the already proven collision-pair lifecycle and publish immutable view-only presentation state. The Editor builder explicitly upgrades/regenerates the new allowlisted assets, and the explicit B1 validation stage requires exactly one Encounter Handoff driver and the final complete graph. After that asset change, item 7 must pass again against the final committed B1 graph before implementation evidence or integration may close; the final fresh instance must still expose complete Transfer registrations and both pair flags false before its first movement publication.

M3E1A, M3E1B0 and M3E1B1 may be reviewed separately but integrate only together. This order is mandatory and breaks the otherwise circular requirement to prove fresh committed-scene restoration before the committed scene exists. If items 1–6 or 8 fail, M3E1B0 does not begin. If provisional B0 item 7 fails, M3E1B1 and all production pair-ignore code remain blocked. If the repeated final B1 item 7 fails, integration remains blocked and the final asset change must be corrected without treating the provisional evidence as a substitute. No production fallback is invented under this contract; Sol must reopen Movement ownership in a separate addendum before M3E1 implementation continues.

## Exact final authored composition

The following is the final B1 committed sandbox shape. The B0 provisional committed shape is identical except that it contains exactly zero Encounter Handoff drivers and no B1 adapter reference. The internal Editor builder/validator receives an explicit `B0` or `B1` stage argument; it never infers stage from scene contents, and B1 regeneration plus validation replaces the provisional allowlisted assets before integration. Neither stage modifies `Bootstrap.unity`, `MovementSandbox.unity`, their existing player prefab, build settings or project settings.

The encounter graph contains exactly:

- one player root with the verified kinematic `PlayerMovementController`, vertical `CapsuleCollider2D`, `Rigidbody2D`, literal Combat target `player` and no Transfer target;
- one walker root with literal Combat/Transfer target `walker`, max health `9`, invulnerability `0`, basic-attack authoring enabled, one kinematic nontrigger `BoxCollider2D`, one production enemy modifier sink and no extra enabled solid collider;
- one surveyor root with literal Combat/Transfer target `surveyor`, max health `6`, invulnerability `0`, basic-attack authoring enabled, one kinematic nontrigger `BoxCollider2D`, one production enemy modifier sink and no extra enabled solid collider;
- exact Combat target registry order `player`, `surveyor`, `walker` and Transfer target registry order `surveyor`, `walker`;
- exactly one Transfer, Combat, Reaction, Behavior, Locomotion, Threat and Encounter Handoff driver bound by serialized references;
- environment-only authored ground/bounds and the existing layer-8 Transfer LOS convention.

Every cross-driver reference must be the same exact graph used by the frozen registries. The authoring validator receives one explicit root from the Editor builder/test and may enumerate only beneath that root. Runtime discovery, role selection by discovery, auto-repair and fallback binding are forbidden.

## Production enemy transfer sink

The existing sandbox owner stub is not authored into M3E1. A production-named internal enemy modifier sink implements the existing transfer sink interface for one serialized literal `walker` or `surveyor` role and the exact serialized base profile `Enemy.Regular.Baseline.v1`. `BoundTargetId` returns that immutable role. `IsStillAvailable` remains true while the exact authored root and sink are active, including the death tick and interval before Combat's scheduled `t+1` removal; the sink never reads or predicts death. `TryApply` succeeds only once for exact target ID, `TransferTargetKind.Enemy`, base profile, `Target.Enemy.Heavy.v1`, a nonnegative tick and proposed revision greater than every prior applied/cleared revision. Any mismatch or repeated apply returns false without mutation. `Clear` validates an approved reason, nonnegative tick and a revision greater than the active revision, returns the sink to Baseline exactly once, and is an idempotent no-op when already clear. The sink owns only its applied Heavy bit and last accepted revision; it does not own health, death, behavior, motion, threat, collider state or damage. Existing Combat-owned next-tick `TargetRemoved` merging remains the sole transfer-registration removal path after death.

## Encounter lifecycle core

The engine-free lifecycle session is constructed with a nonnegative `firstExpectedTick` that permits `t+1`, literal maximum health `player=5`, `surveyor=6`, `walker=9`, invulnerability `45/0/0`, and prior-alive seeds for all three roles. It consumes a new immutable pure `RegularEnemyEncounterLifecycleInput`, never `CombatSimulationOutcome`, the existing test-only `CombatantSnapshot`, or any Unity-assembly type. The input contains the exact global tick, Gameplay or EncounterReset kind, and new minimal `RegularEnemyEncounterRoleState(targetId,currentHealth,isDead)` values for literal `player`, `surveyor`, `walker` in ordinal order. Each health is within `0..maxHealth` and `isDead` is true exactly when health is zero.

Gameplay defensively copies the already canonical Unity outcome's index-aligned `DamageRequest` trace and `DamageResult` order. Counts must match; at each index request/result IDs, target IDs and tick `t` must match exactly, and every request/result field must be in its approved domain. An `Applied` result must target one of the literal roster roles and its nonnegative `AppliedAmount` must not exceed the aligned request amount. Repeated request IDs are valid because M1 may publish the original result and a later `Duplicate` result in the same canonical array; `Duplicate` is a normal non-Applied result. For each literal role, current health must equal prior health minus the checked sum of that role's same-tick `AppliedAmount` values, with no underflow, overflow or non-reset healing. Constructor prior health is exactly `player=5`, `surveyor=6`, `walker=9`; a previously dead role remains at zero. Only an accepted pre-death EncounterReset may restore exact full health `5/6/9`. For presentation the core derives a stable distinct applied-hit target list by testing literal role order `player`, `surveyor`, `walker` and including a role once when at least one copied aligned result for that role is `Applied`; results for other valid IDs and all non-Applied results create no hit target. Any alive-to-dead role edge is valid only when the exact same-tick applied-damage conservation reaches zero. Any dead-to-alive role transition, including player resurrection, rejects atomically. Caller mutation cannot change preview, commit or view.

The `+110` adapter alone projects the already completed Unity `CombatSimulationOutcome` and paired reset publications into this pure input. Consecutive gameplay inputs use exact global ticks. On the first Gameplay input, a regular enemy already dead is treated as the constructor's prior-alive to current-dead edge and must have the same Applied proof. Reset contains an empty canonical request trace, no damage results and exact full-health alive role states `5/6/9`. The core validates the unique roster and frozen identities without referencing a Unity registry.

Owned state is limited to:

- latest exact lifecycle snapshot and next expected global tick;
- prior exact health and alive/dead state for player, walker and surveyor;
- per-encounter defeated/reward-request latches for both roles;
- one encounter-clear/room-completion-request latch.

For each valid alive-to-dead regular-enemy edge, it emits exactly one immutable internal `RegularEnemyDefeatedNotice(targetId,t)` and one immutable internal `RegularEnemyRewardRequest(targetId,t)`. When both roles are dead after the same committed step, it emits exactly one immutable internal `RegularEnemyEncounterClearedNotice(t)` and one immutable internal `RegularEnemyRoomCompletionRequest(t)`. These M3E1 types are frozen future-consumer handoffs, not SYSTEM-CONTRACTS run events: no reward, inventory, room state or scene transition is mutated or implied. A later VD-04/reward-consumer contract must explicitly map or replace them before external consumption. Player death never creates a regular-enemy completion handoff. Repeated dead outcomes, M1 `Duplicate` results and later ticks emit nothing new.

Reset is an M3E1 presentation/lifecycle clear only. It is accepted only before either regular enemy has died, before any defeat/reward/clear/completion latch or collision ignore exists, and with the upstream pipeline's already completed exact reset publications. It clears transient applied-hit presentation and publishes an empty reset snapshot at the unchanged global clock. It does not reset M1, M3A, M3B, M3C, M3D or Transfer. Once either enemy death is committed, same-instance reset rejects without mutation; post-death re-entry requires destruction and fresh instantiation of the complete authored encounter. M3E1 does not define Transfer re-registration.

`PreviewNext` is mutation-free. `CommitNext` recomputes every input-derived field and replaces all lifecycle state atomically. Forged, stale, skipped, duplicate-role, wrong-scalar, resurrection, overflow and malformed reset inputs preserve all prior state.

## `+110` Unity handoff adapter

The adapter runs after the verified `+100` threat bridge at `[DefaultExecutionOrder(110)]` and has explicit serialized Player, Transfer, Combat, Reaction, Behavior, Locomotion and Threat driver bindings. Each upstream driver exposes or reuses a production-named internal immutable read/graph-validation seam; test-named members are never used by production. For source tick `t` it requires all of the following before a lifecycle core is created or advanced:

- player snapshot `Tick=t` and `NextExpectedTick=t+1`;
- present Transfer phase publication `Tick=t` from the bound driver and exact frozen Transfer registry identity; this is a read-only graph/publication consistency check, not a second Transfer consumer and never replays or projects Transfer transitions;
- present Combat outcome `Tick=t`, exact frozen roster, exact canonical request/result index alignment and matching player/regular-enemy role states;
- present Reaction pair `Tick=t`, both role snapshots `Tick=t`, each `NextExpectedTick=t+1`;
- present Behavior pair `Tick=t`, snapshot `Tick=t`, `NextExpectedTick=t+1`;
- present Motion pair `Tick=t`, snapshot `Tick=t`, `NextExpectedTick=t+1`;
- present Threat carried view `Tick=t`, complete snapshot `Tick=t`, `NextExpectedTick=t+1`;
- Combat alive states exactly agree with Reaction/Behavior/Motion/Threat death/alive fields for each literal role;
- Gameplay requires every upstream discriminator to be Gameplay; EncounterReset requires Combat reset and every downstream pair/snapshot to have its already approved exact Reset shape;
- every binding belongs to the same frozen Player/Combat/Transfer/regular-enemy graph, retained collider geometry is unchanged, and no source changes across preflight;
- after first use, lifecycle session latest snapshot, prior carried presentation view and next expected tick are exact and field-for-field aligned.

On first use, every source, binding, graph, geometry and pair-collision state validates before a staged lifecycle core is created. Preview and all preflight checks occur on that staged owner; only after successful core commit and pair-state application is it adopted. Any pre-adoption failure leaves no lifecycle owner, pair-state latch or carried presentation view. It then:

1. captures and validates every immutable source;
2. requires both exact player/enemy pairs to match retained expected `GetIgnoreCollision` flags, initially false;
3. obtains an M3E1 lifecycle preview whose complete desired collision operations use fixed ordinal role order `surveyor`, then `walker`;
4. preflights every operation against current pair flags without mutation;
5. revalidates bindings, geometry, source publications and both pair flags immediately before core commit;
6. commits M3E1 once;
7. if and only if the collision feasibility spike passed, applies each changed pair in the fixed order, verifies `GetIgnoreCollision` immediately after each call, and adopts that desired flag only after verification;
8. publishes one immutable `RegularEnemyEncounterPresentationView` for `t`.

The presentation view contains only exact source tick, player/walker/surveyor alive states, walker and Surveyor raw Heavy/reaction directives, walker phase/telegraph, Surveyor phase/telegraph, active denial-line center/lifetime, the stable distinct same-tick applied-hit target IDs, defeat/reward/clear/completion handoffs and next expected tick. It carries no Transform, Rigidbody, Collider, Renderer, queue or mutable collection.

The adapter never predicts damage, advances gameplay, selects targets, creates a same-tick threat, repairs bindings, disables/destroys/moves roots, changes collider shape/trigger/enabled state, changes body simulation, or issues a physics query/sync. `Physics2D.IgnoreCollision` and `GetIgnoreCollision` are allowed only for the two exact frozen player/enemy collider pairs and only after the spike proves the existing player cast path. External pair-state drift rejects before core mutation. A thrown pair call or failed post-call verification after core commit is fail-stop and publishes no view; no guessed rollback occurs. If the first operation of a simultaneous death succeeded, its verified desired flag remains retained and the process stops before ordinary retry. M3E1 reset is pre-death only and therefore restores no ignored pair; fresh scene destruction owns nonpersistent pair teardown.

## Placeholder and final presentation boundary

M3E1 publishes presentation state but does not claim final UI or art. A project-authored placeholder renderer may be present only to prove that the immutable handoff can drive walker telegraph, Surveyor denial-line, applied-hit pulse and dead visibility without feeding gameplay authority. It uses the existing Default sorting layer and creates no palette, lighting, sprite, material or semantic-color compliance claim. VD-07/VD-08 later own the approved reticle, AimArc, HUD, outline, palette, sorting and lighting result.

## Authoring and runtime invariants

- No runtime `Find*`, tag search, name fallback or hierarchy repair.
- No public ABI, package, assembly-reference cycle, project setting or build-scene change.
- No additional `Physics2D.SyncTransforms`, cast, overlap, raycast or callback-derived damage authority.
- No wall clock, render delta, random, Unity instance ID or unordered collection determines simulation or handoff order.
- Authoring validation never saves, rewrites or repairs the asset it inspects.
- The committed scene is loaded read-only by verification; the Editor builder generates only the new allowlisted assets when explicitly invoked.
- Existing user-owned dirty files are never staged, reverted or regenerated. Implementation evidence records exact pre/post status for `Assets/Scenes/MovementSandbox.unity`, `ProjectSettings/URPProjectSettings.asset` and `ProjectSettings/SceneTemplateSettings.json`.

## Collision-exclusion staged feasibility gate

Before M3E1B0 authored-graph work begins, the focused PlayMode spike must prove items 1–6 and 8 against the exact Unity version and existing movement controller. M3E1B0 then authors only the collision-neutral committed graph and must prove item 7 as the B1-start gate. Only after those eight items pass may M3E1B1 or any production pair-ignore code be written. Because B1 changes the committed asset graph, item 7 must then pass a second time against the final B1 graph before implementation evidence and integration:

1. alive walker and surveyor solids appear in the player's existing horizontal collider cast;
2. ignoring exactly the player/walker pair removes only walker from that cast;
3. ignoring exactly the player/surveyor pair removes only surveyor from that cast;
4. ground, wall and every nonignored collider remain blocking;
5. enemy M3C2/M3D2 frozen geometry revalidation still passes while ignored;
6. `GetIgnoreCollision` exactly reflects apply and a fresh encounter instance starts false;
7. destroying the post-death encounter and instantiating a fresh committed scene graph restores complete Transfer registrations with both pair states false before its first movement publication;
8. 30/60/144 render grouping yields identical logical traces.

Failure of any item stops M3E1. Collider disable/destroy, trigger conversion, body unsimulation, root deactivation, teleport and layer mutation are not fallbacks.

## Evidence matrix

- committed scene/prefab read-only load: exact hierarchy, GUID references, registries, scalar identities, geometry, layer and complete driver graph;
- missing, duplicate, cross-wired and foreign-graph mutation matrix for every role and driver;
- direct scene boot with exact execution order `-200 -> -190 -> -180 -> -170 -> -160 -> default -> +100 -> +110`, threat lane registration and at least 60 neutral ticks without exception;
- scripted test-only command/aim injection against the authored targets proving walker contact and Surveyor denial-line distinction, basic-attack defeat and Heavy reaction differences without production device input;
- authored Transfer evidence for ready apply, out-of-range, LOS-blocked, removed-target and repeated-script cases: failures preserve state, the same script selects the same target/modifier, and death removal remains exact `t+1`;
- one-shot defeat/reward per role, one-shot clear/completion after both deaths, no completion on player death or one enemy death, simultaneous deaths and repeated dead outcomes;
- pre-death same-instance reset is an empty lifecycle/presentation clear; post-death reset rejects, while destroyed/fresh scene instantiation restores complete Transfer registrations and has no stale handoff or collision-pair state;
- exact denial-line spawn, active lifetime, post-Surveyor-death persistence and expiry in the immutable presentation view;
- collision-exclusion feasibility gate and environment preservation;
- focused core EditMode, authoring EditMode, scene PlayMode, all relevant Movement/Transfer/Combat regressions, full EditMode and full PlayMode;
- source scans for public surface, runtime discovery, extra physics query/sync, time/RNG and protected-file changes.

Every verification result cites the relevant acceptance-criterion IDs and records the exact Unity XML count, failed/skipped count and SHA-256.

## Allowlist

- a new engine-free encounter lifecycle source in the existing no-engine `AcadeGameMaker.Combat` assembly and focused EditMode tests;
- narrow production-named immutable read and exact graph-validation seams on already verified Transfer/Combat/Reaction/Behavior/Locomotion/Threat drivers when the handoff cannot consume an existing production seam; the Transfer seam validates the exact bound player plus ordered `surveyor`, `walker` registry identity without exposing a mutable registry or replaying a publication;
- one production enemy transfer modifier sink inside the existing Transfer.Unity assembly;
- one `+110` encounter handoff adapter and immutable internal presentation/event contracts inside Combat.Unity;
- one Editor-only `AcadeGameMaker.Combat.Authoring.Editor` assembly with exact direct references to `AcadeGameMaker.Core`, `AcadeGameMaker.Movement`, `AcadeGameMaker.Movement.Unity`, `AcadeGameMaker.Transfer`, `AcadeGameMaker.Transfer.Unity`, `AcadeGameMaker.Combat` and `AcadeGameMaker.Combat.Unity`, with no runtime assembly referencing it;
- narrow `InternalsVisibleTo("AcadeGameMaker.Combat.Authoring.Editor")` additions in Core, Movement, Movement.Unity, Transfer, Transfer.Unity, Combat and Combat.Unity `AssemblyInfo.cs` solely for type-safe authoring/configuration and direct use of the internal configuration signatures they expose, with no public fallback;
- one builder and validator in that Editor assembly;
- `Assets/Scenes/CombatEncounterSandbox.unity`;
- new `Assets/Prefabs/Combat`, `Assets/Prefabs/Enemies` and `Assets/Prefabs/Player/CombatEncounterPlayer.prefab` assets, with the existing movement prefab treated as read-only source material;
- focused EditMode/PlayMode tests and required `.meta` files;
- this contract, pre-gate, implementation evidence and documentation index.

## Denylist

- changes to verified M1/M2A/M3A/M3B/M3C/M3D semantics, tick order, damage, health, threat, movement or transfer removal;
- production InputRouter, camera publisher, device bindings, HUD, menu, final art or audio;
- boss, humanity choice, room assembly, run, persistence, actual reward grant or scene transition;
- any existing scene/prefab modification, including `Bootstrap.unity` and `MovementSandbox.unity`;
- `EditorBuildSettings`, `TagManager`, URP, package, player or other project settings;
- public types/members, external assets, runtime role discovery, physics callbacks as authority, collision-layer mutation or renderer-to-gameplay feedback.

## Ollama utilization record

| Lane | Outcome | Sol screening disposition |
|---|---|---|
| Kimi K3 | used and accepted in part | Retained immutable view mapping, explicit graph validator and missing/duplicate/cross-wire fixture decomposition. Rejected public types, invented stamina/combo/score, Q1000 UI math, incomplete approved presentation rules and any repair behavior. |
| GLM 5.2 | used and accepted in part | Retained `t -> t+1`, stale reset/re-entry, extra physics/sync, role cross-wire and presentation-authority inversion mutations. Rejected view-owned reaction/reset/queue clearing and unsourced presentation mechanics. |
| MiniMax M3 | failed and replaced | The general UI/art/run tool packet produced useful advisory decomposition, but the bounded M3E1 scene-validator follow-up returned empty text without quota/rate/auth/model/network error. Terra supplies the local fixture design and Luna reviews it; no empty output is adopted. |

No cloud model received repository text, paths, credentials, personal data or secrets. Ollama outputs have no decision, approval, merge or canon authority.

## Stop conditions and approval gate

Stop and report to Sol if implementation requires Movement query changes, a failed collision spike fallback, a public or cross-assembly cyclic API, scene transition, actual reward ownership, project settings, final art/UI, modified verified tick semantics, or edits to protected user files.

Implementation may begin only after Luna independently reports no P0/P1 contradiction and Sol changes this contract to `Approved`. Approval does not close any full acceptance criterion.
