---
status: Review
---

# Costume file IO and additive Unity presentation adapter

- Date: 2026-09-13
- Status: Review — neither child gate is implementation-authorized
- Parent: [Approved costume presentation and sprite-production contract](./2026-09-13-costume-presentation-and-sprite-production.md), `REQ-COST-005..009/012/014`
- Dependencies: implemented pure `AcadeGameMaker.Costumes` and `AcadeGameMaker.Presentation` APIs; source-generation inventory remains separate
- Assigned by / final authority: Astra
- Intended implementation/tests: Terra
- Independent pre/post verification: Luna
- Requirements: `REQ-CIO-001..008`, `REQ-CUA-001..010`
- Acceptance criteria: `AC-CIO-001..007`, `AC-CUA-001..009`
- Rollback point: verified pure costume/presentation core before either child begins

## Bounded outcome

This contract defines two separately approved additive children:

1. **CIO file adapter:** persist and recover the pure `CostumeStateV1` canonical bytes in a costume-only primary/previous/temp lane.
2. **CUA Unity adapter:** expose truthful Settings/character-preview view state and, only when a complete Accepted media package exists, atomically bind the concept portrait and gameplay sprite set while preserving the completed simulation action.

They do not edit the existing Profile, Run, movement, combat, transfer, input, camera, scene, or prefab owners. The current 36 Seryeong catalog entries are `ProductionPending` and there is no Accepted default. Therefore the only truthful present UI result is `NoAcceptedDefault`/unavailable, with no portrait, gameplay atlas, selection, unlock, current costume, or fallback synthesized. This contract authorizes the adapter structure and unavailable-state tests; it does not convert any asset to Accepted.

The repository has no existing Settings screen, character screen, or general character-renderer owner to attach to. Additive components can be built and tested in isolated PlayMode fixtures, but no player-visible scene connection is claimed by this unit.

## Existing public seams consumed without modification

The IO child consumes only `CostumeCanonicalCodecV1.Encode/TryDecode`, `CostumeRecoveryInputV1`, `CostumeRecoverySelectorV1.Recover`, `CostumeRecoveryPlanV1`, and immutable `CostumeStateV1` values. Similar Profile services are behavioral references, not reusable authorities, and are never called or modified.

The Unity child consumes only `CostumeCatalogV1`, `CostumeStateV1`, `CostumeAvailability`, `CostumeDefinitionV1`, `CostumePresentationServiceV1.BuildViews/TryCreateBinding/TrySwap`, `CostumePresentationBindingV1`, `CostumeViewModelV1`, and `CompletedActorSnapshotV1`. It may translate Unity observations into the completed-snapshot value, but it cannot manufacture action completion or reach into a simulation owner.

If implementation finds that an essential member is internal, missing, or semantically insufficient, it stops. This child may not widen or edit the pure-core API opportunistically; Astra must approve a distinct core amendment.

## Child A — CIO costume file adapter

### Path and ownership

The adapter receives one normalized absolute non-root directory from its composition caller and owns exactly three fixed filenames below it:

- `costume-state-v1.json` — primary;
- `costume-state-v1.previous.json` — previous;
- `costume-state-v1.temp.json` — temp.

It rejects empty, relative, filesystem-root, traversal, alternate-name, and role-aliasing inputs before IO. It does not derive its root from Profile paths, `persistentDataPath`, environment variables, current directory, or user ID. The eventual composition root choice belongs to a later integration contract.

`CostumeFileObservationV1` stores defensive bytes and typed per-role observation dispositions (`Missing`, `Read`, `ReadFailed`). It contains no open stream, exception text, path, clock, callback, Unity object, or mutable collection. A read failure is distinct from missing and cannot be treated as an empty/default document.

### Load and recovery semantics

The adapter reads each exact role at most once into a closed observation, passes defensive bytes to the pure recovery selector, and publishes one immutable `CostumeLoadReceiptV1` containing role observations, selected source role if any, recovery reason, source/result revision, canonical result bytes, and `HasBinding`. Unknown/corrupt bytes remain invalid candidates. No candidates are merged.

`NoAcceptedDefault` is a successful read-only unavailable result. It performs no save, promotion, unlock, current selection, binding publication, or media lookup. A role read failure yields a typed load failure if the selector cannot prove a valid result from the other completed role observations; it never silently bootstraps over uncertain unread data.

### Durable save transaction

Saving receives exact expected state revision, exact canonical bytes freshly reproduced from that state, and a normalized root. It serializes no live object during file mutation. The fixed order is:

1. validate root, state, expected revision, and canonical byte equality before opening a file;
2. create the temp role exclusively or truncate only the exact owned temp file;
3. write all bytes, flush managed buffers, then flush the temp file to durable storage;
4. if primary exists, atomically replace primary with temp while creating/replacing previous through the platform primitive;
5. if primary is absent, atomically move temp to primary without overwrite;
6. reopen primary once and require byte equality plus successful pure decode before reporting commit.

The receipt dispositions are `CommittedFirst`, `CommittedReplacement`, `FailedBeforeCommit`, and `CommitOutcomeUncertain`. Any failure before the platform move/replace begins is `FailedBeforeCommit`; primary/previous remain authoritative and a known temp may be left for later observation. An exception or verification failure after move/replace begins is `CommitOutcomeUncertain`; the adapter must not retry, overwrite, delete, promote, or report rollback. It returns/throws a typed result carrying the last proven stage, never a false success. Revision is owned by the pure state transition and is not incremented by IO.

Recovery never quarantines or deletes in this child. A future preservation/quarantine adapter requires its own contract. The adapter never writes Profile or Run roots and never uses broad directory cleanup.

## Child B — CUA Unity Settings, preview, and gameplay adapter

### Truthful view states

`CostumeUnityViewStateV1` is an immutable projection with actor ID, catalog/state revisions, availability, row view models, current ID if present, preview ID if present, selection eligibility, persistence status, and typed diagnostic. It contains no grant capability and cannot mutate the catalog/state.

For the current 36-entry all-pending catalog it must expose all entries as pending/unavailable and `NoAcceptedDefault`; `CanPreviewMedia=false` and `CanPrepareSelection=false` for every row. A label/icon may state that costume artwork is unavailable or in production, but no placeholder portrait or sprite may be passed off as that costume. Preview navigation may change only a transient highlighted row; it cannot create a current selection or durable write request.

Settings and character-preview presenter components accept already-built view state through an instance-bound port and render it into adapter-owned test views. They do not locate existing canvases, use global object search, create runtime menus, or alter InputMode. Real menu navigation/input ownership is deferred.

### Accepted-media package and atomic validation

Future Accepted content is admitted only through an authored `CostumeUnityMediaPackageV1` whose identity exactly matches one Accepted `CostumeDefinitionV1`. The package contains actor/costume IDs, catalog and presentation revisions, portrait-set/gameplay-set/clip-map IDs and expected lowercase SHA-256 values, one concept portrait reference, one gameplay sprite/clip map, required action IDs, frame/pivot/baseline metadata, and source-manifest bytes.

Validation stages the entire package without changing a renderer or preview:

- recompute SHA-256 over the exact canonical manifest/media byte payload supplied by the approved import pipeline and compare every catalog hash;
- require actor, costume, set IDs, catalog revision, and presentation revision to match the Accepted definition exactly;
- require one portrait and a complete ordinal unique clip map containing every declared required action;
- validate every sprite belongs to the declared atlas, uses integer rectangles, point filtering, approved pixels-per-unit/body profile, pivot/baseline, no missing frame, and no unexpected runtime Animator/controller dependency;
- call pure `TryCreateBinding` with the exact clip IDs and require a complete binding.

Unity object names, GUID strings, inspector order, and object instance IDs are not integrity evidence. The adapter may retain resolved Unity references only inside a staged one-owner candidate correlated to the validated pure binding and package revision. A missing/unreadable asset, hash mismatch, wrong ID/revision, pending/rejected catalog row, incomplete clip, invalid import setting, foreign/stale candidate, or destroyed Unity object rejects the whole candidate. It leaves both the last portrait and gameplay presentation untouched. There is no partial portrait-first or renderer-first update.

Because all 36 current rows are pending, accepted-media positive fixtures must use synthetic test-only definitions and in-memory test textures/sprites. They are not evidence that a real Seryeong outfit is Accepted.

### Completed-action gameplay binding

The gameplay component receives a completed immutable `CompletedActorSnapshotV1` for tick `T` through an explicit instance-bound observation port after the simulation owner has committed `T`. It has no `FixedUpdate` simulation ownership. It validates actor ID, monotonic completed tick, action ID, action age, normalized phase, facing, and anchor; duplicate `T` may be an exact idempotent render refresh, while decreasing, conflicting duplicate, future/uncommitted, or foreign actor snapshots fault the candidate without touching presentation.

A costume swap first validates durable/current state correlation, the complete media candidate, and the current completed snapshot, then calls pure `TrySwap`. Commit changes the adapter-owned portrait reference, gameplay sprite/clip map, and pure binding as one logical publication. The selected clip begins at the frame derived from the preserved completed action ID and normalized phase; it does not restart at frame zero. Tick, action age, phase, facing, transform, Rigidbody2D, collider, hit/hurt/aim/transfer geometry, stats, abilities, cooldowns, AI, and phase logic are read-only invariants. A missing current action clip rejects the swap and keeps the old complete binding.

Visual frame progression is a projection of supplied completed snapshots. `Update`, `FixedUpdate`, Animator state-machine time, wall clock, and frame rate cannot advance gameplay action phase independently. Cosmetic interpolation may not choose an action, cross a mechanic phase boundary, or feed simulation.

### Selection and persistence handoff

The UI adapter may request pure selection preparation only when the view model says the Accepted costume is unlocked. It stages the resulting state and canonical bytes, asks the CIO port to save them, and publishes the new current selection/media binding only after a `CommittedFirst` or `CommittedReplacement` receipt correlates to those exact bytes and revision.

`FailedBeforeCommit` retains old durable state, old current selection, portrait, and gameplay binding. `CommitOutcomeUncertain` enters a terminal `ReloadRequired` presentation state: it disables further costume mutation, retains the last visually complete binding only as non-authoritative display, and requires a fresh CIO observation/recovery before another selection. It never claims either old or new selection is durable. `NoAcceptedDefault` never reaches save or bind.

## Exact implementation allowlists and independent gates

### Gate CIO — file adapter

Only these new paths may be added:

- `Assets/AcadeGameMaker/Runtime/Costumes/IO/AcadeGameMaker.Costumes.IO.asmdef` and `.meta`;
- `Assets/AcadeGameMaker/Runtime/Costumes/IO/CostumeFileAdapterV1.cs` and `.meta`;
- `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/AcadeGameMaker.Costumes.IO.EditMode.Tests.asmdef` and `.meta`;
- `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/CostumeFileAdapterV1Tests.cs` and `.meta`;
- `docs/verification/2026-09-13-costume-io-contract-pregate.md`;
- later `docs/verification/2026-09-13-costume-io-implementation-evidence.md` and `docs/verification/2026-09-13-costume-io-luna-independent-review.md`;
- minimum link/status hunk in `docs/README.md`.

No existing source/test/asmdef, Profile/Run file, scene, prefab, asset/media, Package, or ProjectSetting may change. Gate CIO needs Luna pre-gate and separate Astra Approved status before Terra begins.

### Gate CUA — additive Unity adapter

Only these new paths may be added:

- `Assets/AcadeGameMaker/Runtime/Presentation/Unity/AcadeGameMaker.Presentation.Unity.asmdef` and `.meta`;
- `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityMediaPackageV1.cs` and `.meta`;
- `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityPresentationAdapterV1.cs` and `.meta`;
- `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityViewPresenterV1.cs` and `.meta`;
- `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/AcadeGameMaker.Presentation.Unity.PlayMode.Tests.asmdef` and `.meta`;
- `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityPresentationAdapterV1Tests.cs` and `.meta`;
- `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityViewPresenterV1Tests.cs` and `.meta`;
- `docs/verification/2026-09-13-costume-unity-adapter-contract-pregate.md`;
- later `docs/verification/2026-09-13-costume-unity-adapter-implementation-evidence.md` and `docs/verification/2026-09-13-costume-unity-adapter-luna-independent-review.md`;
- minimum link/status hunk in `docs/README.md`.

Tests construct objects in isolated temporary scenes and destroy only objects they created. No existing source/test/asmdef, Profile/Run/movement/combat/transfer/input/camera file, scene, prefab, real media, Package, or ProjectSetting may change. Gate CUA depends on a Verified pure core and at least an implemented CIO port contract, receives its own Luna pre-gate, and needs separate Astra Approved status. CIO approval does not approve CUA.

## Requirements

- **REQ-CIO-001:** IO owns only three fixed costume filenames below a caller-supplied validated non-root directory and never reads/writes Profile or Run storage.
- **REQ-CIO-002:** Each role is observed at most once into defensive immutable evidence; missing, read failure, invalid bytes, and valid bytes remain distinct.
- **REQ-CIO-003:** Load delegates byte interpretation/recovery to the pure core, never merges candidates, and never bootstraps over an uncertain unread role.
- **REQ-CIO-004:** Save follows the exact temp durable-flush then move/replace then primary-reopen verification sequence and never increments state revision.
- **REQ-CIO-005:** Pre-commit and uncertain-commit failures remain distinct; uncertain commit never triggers retry, cleanup, overwrite, promotion, or false rollback/success.
- **REQ-CIO-006:** `NoAcceptedDefault` is read-only unavailable and causes no save, grant, unlock, selection, binding, or media operation.
- **REQ-CIO-007:** This child does not quarantine/delete files, choose a composition root, modify pure APIs, or reuse Profile authority.
- **REQ-CIO-008:** CIO implementation stays inside its exact additive allowlist.

- **REQ-CUA-001:** The Unity view exposes exact pending/locked/unlocked/current/unavailable state and truthfully reports all current 36 Seryeong entries as pending with `NoAcceptedDefault`.
- **REQ-CUA-002:** Preview navigation is transient and cannot grant, select, save, bind, or present placeholder media as an outfit.
- **REQ-CUA-003:** An Accepted media package validates IDs, revisions, canonical payload hashes, portrait, complete clips, sprite import geometry/settings, and pure binding before any publication.
- **REQ-CUA-004:** Portrait and gameplay media publish atomically; every validation/swap failure retains the old complete pair.
- **REQ-CUA-005:** Gameplay presentation consumes only completed snapshots, preserves tick/action/phase/facing/anchor across swap, and never owns or advances simulation time.
- **REQ-CUA-006:** Costume presentation remains visual-only and cannot alter physics, collision, combat, transfer, stats, abilities, AI, targeting, or mechanic phase.
- **REQ-CUA-007:** Selection publishes only after exact correlated durable-save success; failed-before-commit and uncertain-commit produce the closed behavior above.
- **REQ-CUA-008:** Synthetic Accepted test assets are fixture evidence only and cannot change the real catalog or claim a real outfit is Accepted.
- **REQ-CUA-009:** UI components use explicit instance-bound ports and do not discover/modify existing menus, scenes, prefabs, InputMode, or renderer owners.
- **REQ-CUA-010:** CUA implementation stays inside its exact additive allowlist.

## Acceptance criteria

- **AC-CIO-001 (REQ-CIO-001/008):** path tests reject null/empty/relative/root/traversal/role aliasing and prove all opened paths are the three exact names beneath the isolated test root; static diff proves no existing or Profile/Run file changed.
- **AC-CIO-002 (REQ-CIO-002/003):** missing/read-failed/corrupt/valid permutations prove one read per role, defensive bytes, deterministic precedence, no merge, and no bootstrap when unread data leaves authority uncertain.
- **AC-CIO-003 (REQ-CIO-004):** first-save and replacement traces prove exact stage order, durable temp flush, atomic operation, primary reopen, byte equality, decode, and unchanged state revision.
- **AC-CIO-004 (REQ-CIO-005):** a fault at every stage proves the correct `FailedBeforeCommit` or `CommitOutcomeUncertain` boundary, no automatic retry/cleanup/promotion, and no success claim without reopened-primary equality.
- **AC-CIO-005 (REQ-CIO-006):** all-pending catalog plus missing/corrupt/valid empty state yields `NoAcceptedDefault`, zero writes, and no fabricated current/unlock/binding.
- **AC-CIO-006 (REQ-CIO-007):** static review finds no Profile service call, quarantine/delete, root discovery, Unity API, mutable static port, clock, RNG, network, or existing-core edit.
- **AC-CIO-007 (all CIO):** focused and full EditMode plus full PlayMode pass with failed/skipped/inconclusive `0`; Luna independently verifies evidence and Astra decides acceptance.

- **AC-CUA-001 (REQ-CUA-001/002):** isolated PlayMode UI shows exactly 36 pending rows and `NoAcceptedDefault`; every row has preview/select false, and highlight changes leave state bytes and save/media call counts unchanged.
- **AC-CUA-002 (REQ-CUA-003/004/008):** synthetic fixtures independently corrupt actor/costume/set IDs, revisions, each hash, manifest payload, portrait, required clip, sprite rectangle, filtering, PPU, pivot, baseline, and Unity reference lifetime; every case publishes neither half and preserves the old pair. Real catalog remains all pending.
- **AC-CUA-003 (REQ-CUA-004):** observation hooks around successful synthetic commit never see new portrait with old gameplay media or the reverse; foreign/stale/consumed candidates reject.
- **AC-CUA-004 (REQ-CUA-005):** duplicate/decreasing/conflicting/future/foreign snapshots and missing-action swaps reject. Valid idle/locomotion/airborne/landing/bow draw/release/recovery swaps preserve tick, action age, normalized phase, facing, anchor, and mapped frame without frame-zero restart or extra simulation call.
- **AC-CUA-005 (REQ-CUA-006):** before/after evidence proves transform, Rigidbody2D, collider, hurt/hit/aim/transfer geometry, stats, cooldowns, action/target/phase identity and simulation-owner call counts unchanged.
- **AC-CUA-006 (REQ-CUA-007):** save success publishes exact selected state and binding once; `FailedBeforeCommit` retains old state/media; `CommitOutcomeUncertain` enters `ReloadRequired`, blocks mutation, claims no durable winner, and requires a fresh recovery receipt.
- **AC-CUA-007 (REQ-CUA-009/010):** static diff and runtime probes prove explicit ports, no global lookup, no existing scene/prefab/menu/InputMode/owner mutation, and allowlist-only additions.
- **AC-CUA-008 (REQ-CUA-008):** evidence labels every Accepted positive case `synthetic test fixture`; repository inventory and UI continue to report all 36 real entries pending.
- **AC-CUA-009 (all CUA):** focused PlayMode and full EditMode/PlayMode pass with failed/skipped/inconclusive `0`; Luna independently verifies and Astra decides acceptance.

## Traceability

| Requirement | Acceptance criteria |
|---|---|
| REQ-CIO-001 | AC-CIO-001 |
| REQ-CIO-002 | AC-CIO-002 |
| REQ-CIO-003 | AC-CIO-002 |
| REQ-CIO-004 | AC-CIO-003 |
| REQ-CIO-005 | AC-CIO-004 |
| REQ-CIO-006 | AC-CIO-005 |
| REQ-CIO-007 | AC-CIO-006 |
| REQ-CIO-008 | AC-CIO-001, AC-CIO-007 |
| REQ-CUA-001 | AC-CUA-001 |
| REQ-CUA-002 | AC-CUA-001 |
| REQ-CUA-003 | AC-CUA-002 |
| REQ-CUA-004 | AC-CUA-002, AC-CUA-003 |
| REQ-CUA-005 | AC-CUA-004 |
| REQ-CUA-006 | AC-CUA-005 |
| REQ-CUA-007 | AC-CUA-006 |
| REQ-CUA-008 | AC-CUA-002, AC-CUA-008 |
| REQ-CUA-009 | AC-CUA-007 |
| REQ-CUA-010 | AC-CUA-007, AC-CUA-009 |

## Later integration slice

The smallest useful player-visible follow-up is one exact hub/settings scene or prefab composition change that injects: the caller-approved costume storage root, CIO service, current recovered state, catalog, completed-action observation source, the existing UI navigation owner, portrait target, and gameplay renderer target into the Verified CUA ports. Before drafting that contract, Astra must inventory the actual scene/prefab and identify the single menu/InputMode owner and completed-action publisher. Its allowlist should name those exact existing files plus one integration test; it must not authorize broad scene searches or changes to simulation owners. Until that later contract is Approved and verified, CIO/CUA are library and isolated-fixture results only, not a reachable Settings screen, character screen, or playable costume switch.

## Stop conditions and participation

Stop and return to Astra if the pure public APIs cannot express a required correlation, platform atomic replacement semantics cannot be proved, the storage root would overlap Profile/Run, a real media package remains pending, a snapshot is not demonstrably post-commit, or connection requires an unnamed existing scene/prefab/owner edit.

Sol drafted this Review child contract from read-only inspection of the implemented pure costume/presentation interfaces and current UI-related repository seams. No implementation, existing runtime/test edit, scene/prefab/media change, IO execution, Unity execution, asset acceptance, or player-visible integration is claimed.
