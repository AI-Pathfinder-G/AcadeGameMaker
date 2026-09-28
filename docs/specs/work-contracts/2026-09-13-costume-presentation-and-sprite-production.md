---
status: Approved
---

# Costume presentation and sprite-production work contract

- Date: 2026-09-13
- Status: Approved — Astra 2026-09-13, first pure-code slice only; subsequent IO/Unity slices retain their child gates.
- Assigned by / final authority: Astra
- Contract drafting support: Sol
- Intended implementation and implementation tests: Terra
- Independent pre/post verification: Luna
- Parents: `REQ-ART-002/005/008/009/014`, `REQ-PLAT-006/008`, Approved VD-08, ADR-0011/0012/0017/0020/0027/0031, and [offline character sprite-atlas production](./2026-09-09-offline-sprite-atlas-production.md)
- Requirement IDs: `REQ-COST-001..014`
- Acceptance IDs: `AC-COST-001..012`
- Rollback point: current repository state before this new unit

## Outcome and honest readiness

This unit specifies Seryeong's selectable existing-outfit presentation first, then a reusable data boundary for Doeon and later actors. It covers stable costume IDs, unlock/select behavior in Settings and the character screen, profile-owned persistence isolation, atomic portrait/gameplay-atlas mapping, action-driven clothed secondary motion, and the production manifest needed for future NPC, enemy, and boss sheets with skill/phase VFX descriptions.

The existing `32px` sole-to-crown target is a nominal gameplay body target inside a larger transparent cell. It is suitable for silhouette-scale gameplay art, but it may be too compressed for readable garment construction, facial identity, or subtle one-pixel settling. No source may be called a complete outfit sheet merely because it can be scaled into that target. Each outfit must pass native `1x` readability review. A failing outfit remains concept/reference or `production_pending`; the producer may propose a documented `48px` or `64px` body-height profile in an integer-sized cell, but adopting that profile requires a separate Astra-approved art-density/camera/collider impact contract. This contract does not silently change 18 PPU, collider geometry, camera framing, or authored gameplay scale.

The repository currently contains Profile value/codec/recovery units and player movement/combat tick owners, but no general costume catalog or character-presentation owner. Implementation therefore remains additive. It must not edit active Profile or Run sources and must not make a renderer, Animator, UI, or asset importer authoritative for unlocks.

## Scope

### Included in the first implementation slice

- An engine-free, immutable costume catalog and unlock/selection core with stable IDs.
- All 36 canonical Seryeong inventory entries from the Approved [source-generation contract](./2026-09-13-seryeong-costume-source-generation.md). Each remains `production_pending` until its own portrait/gameplay evidence is accepted; implementation never invents availability.
- Unlock grants from explicit, typed progression receipts or authored default grants; selection requests; locked-selection rejection; deterministic current-costume fallback.
- A versioned costume-only canonical document codec and pure recovery transformation, isolated from the active profile schema/files and from run state; actual file IO is deferred to a child contract.
- A presentation binding record that maps one costume ID atomically to its character-screen concept portrait and complete gameplay atlas/clip set.
- Settings and character-screen view models: locked/unlocked/current state, preview metadata, and selection outcome. The character screen uses concept art; gameplay uses the approved sprite atlas.
- A tick-keyed visual-motion descriptor that follows completed actor actions without changing their tick order or simulation.
- Deterministic offline manifests, validators, contact sheets, and native-scale previews for the accepted Seryeong outfit set.

### Deferred production sequence

After Seryeong has independent visual acceptance, the same schema may receive separately approved packages in this order: named NPCs, regular enemies, then bosses. Every actor/form package lists locomotion, interaction or combat action IDs; every combat action cites an Approved mechanic contract or is labeled `visual_proposal_only`/`combat_pending`. Boss packages additionally list phase ID, phase transition frames, telegraph/execute/recover frames, attachment points, sorting intent, and skill/phase VFX specification. VFX specs declare sprite sequence, logical-pixel envelope, pivot/attachment, palette role, sorting layer, blend/light behavior, tick-relative visual timing, loop rule, and mechanic citation. They do not define damage, hit geometry, invulnerability, AI, phase thresholds, or simulation timing.

### Excluded

- Changes to existing `Assets/AcadeGameMaker/Runtime/Profile/**`, `Runtime/Run/**`, movement, combat, transfer, input, camera, scenes, prefabs, colliders, hitboxes, stats, ability logic, or phase logic.
- A fake `unlock all`, debug bypass in player-facing UI, selection of locked/unknown content, inferred unlocks from filenames, or defaulting every catalog entry to unlocked.
- Runtime image generation, procedural anatomy, palette-only costume substitution, incomplete atlas substitution, or claiming concept art is a gameplay sheet.
- Script-based image processing until the separately pending user permission is received. Pure code for value models, validation, and deterministic manifest tooling may proceed only after this contract is Approved.

## Public data contract

### Stable identity and catalog

Canonical IDs are NFC, non-empty lowercase ASCII tokens matching `[a-z0-9][a-z0-9._-]*`, compared with `StringComparer.Ordinal`, and never derived from display text or asset paths. Initial namespaces are:

- actor: `seryeong`;
- costume: `<actor-id>.costume.<authored-slug>.v1`;
- portrait set: `<costume-id>.portrait.v1`;
- gameplay set: `<costume-id>.atlas.v1`;
- action: a shared closed vocabulary where a mechanic is already Approved, otherwise an actor-scoped authored token;
- VFX: `<actor-or-form-id>.vfx.<skill-or-phase-slug>.v1`.

`CostumeCatalogV1` is immutable, ordinal-sorted, duplicate-free, and versioned. Each `CostumeDefinitionV1` contains exact actor/costume IDs, availability disposition (`Accepted`, `ProductionPending`, `Rejected`), unlock-gate ID, default-grant flag, portrait-set ID, gameplay-atlas-set ID, presentation revision, source citations, content hashes, and the exact per-outfit reference-hair identity. Hair is inseparable presentation metadata: hair-only H variants are not costumes, no independent hair catalog or combinatorial hair/costume selector exists, and every portrait/atlas preserves the hair belonging to that outfit reference. `Accepted` requires both portrait and gameplay mappings; partial mappings cannot be selectable. When an actor has an accepted default it must be unique and default-granted.

The initial catalog contains exactly these 36 lowercase inventory slugs, each mapped to `seryeong.costume.<slug>.v1`: `story`; `a1`, `a2`, `a3`; `c1`, `c2`, `c3`; `d1`, `d2`, `d3`; `v1`, `v2`, `v3`, `v4`; `t1`, `t2`, `t3`, `t4`; `l1`, `l2`, `l3`, `l4`; `b1`, `b2`, `b3`, `b4`; `x1`, `x2`, `x3`, `x4`; `s1`, `s2`, `s3`; `r1`, `r2`, `r3`. Front/back views are one costume entry. Every entry appears exactly once as Accepted, ProductionPending, or Rejected with reason, exact source option, per-outfit reference hair, and source hash.

An actor whose catalog has no Accepted default has the explicit `NoAcceptedDefault` availability. Catalog construction and read-only listing remain valid, but selection preparation, binding publication, unlock granting, fallback, and persistence mutation for that actor are unavailable. Consumers show this typed unavailable state; they do not fabricate a default binding, promote a pending/rejected costume, or unlock anything.

### Unlock and selection core

`CostumeStateV1` contains schema version, monotonic nonnegative revision, ordinal-sorted unlocked costume IDs, and one current costume ID per actor. It is immutable and fail-validating at construction and every public getter.

Unlock authority accepts only an exact typed `CostumeUnlockGrantV1` created by an internal trusted issuer registered through an Astra-approved closed issuer allowlist, or the catalog's authored default-grant issuer. The issuer holds an internal opaque owner-bound capability; public IDs, reflection-manufactured values, matching strings, and caller-supplied authority labels cannot mint grants. A grant contains grant ID, costume ID, closed authority kind, issuer instance correlation, and a strictly monotonic nonnegative source revision for that issuer lane. This is in-process authority and ordering evidence, not a cryptographic signature or proof of durable provenance. Unknown issuer, foreign owner/capability, pending/rejected costume, replay with conflicting fields, non-increasing revision, stale revision, or cross-actor grant rejects without mutation. Idempotent replay of the exact already-applied grant returns an explicit no-change receipt and does not advance stored revision.

Selection is a two-stage prepare/commit operation against the exact state revision and catalog revision. `PrepareSelection(actorId, costumeId, expectedStateRevision, expectedCatalogRevision)` returns either an opaque one-owner candidate or a typed rejection (`UnknownActor`, `UnknownCostume`, `NotAccepted`, `Locked`, `StaleState`, `StaleCatalog`). Rejection changes no state and publishes no preview binding. Commit accepts only the exact fresh candidate once, increments the state revision once, and changes only that actor's current costume.

There is no player-facing unlock-all operation. Test fixtures may construct explicit states in test assemblies; production APIs cannot enumerate-and-grant the catalog.

### Persistence isolation, versioning, and recovery

Costume persistence uses its own canonical UTF-8 document and its own atomic-file lane, for example `costume-state-v1.json` with primary/previous/temp roles. It must not add fields to, edit, wrap, or reinterpret the active Profile v1 files or M5D3–M5D7 types. Its schema version starts at `1`; unknown versions fail closed and remain recoverable data rather than being overwritten.

Canonical bytes include schema version, state revision, unlocked IDs, current mappings, applied grant correlations, and integrity metadata. Decoder, integrity validation, load selection, quarantine, recovery transformation, and atomic save must be separately bounded subcontracts or exact new files under this unit; no reuse claim may be made merely because the Profile lane has similar behavior.

Recovery transformation selects the highest valid decoded candidate under documented primary/previous/temp precedence and never merges damaged states. In this first slice those roles are pure input values only; no filesystem is read or written. If no valid costume document exists, it bootstraps only catalog-declared accepted default grants. For each actor, an unknown, no-longer-Accepted, or locked current ID falls back deterministically to that actor's unique accepted default and records a typed recovery reason. If no accepted default exists, recovery returns `NoAcceptedDefault` and publishes no binding, grant, unlock, or fabricated current selection. Valid unlocks for other costumes survive fallback. Recovery never converts every catalog item to unlocked, never uses display order as authority, and never changes Profile or Run files.

### Atomic portrait/gameplay mapping

`CostumePresentationBindingV1` is one immutable tuple: actor ID, costume ID, portrait-set ID/hash, gameplay-atlas-set ID/hash, clip-map hash, presentation revision, and catalog revision. Both media halves and the complete required clip map validate before publication. UI and gameplay consumers obtain the same committed binding revision; a change cannot expose the new portrait with the old atlas or vice versa. Failed load, missing clip, hash mismatch, wrong actor, or stale revision retains the last complete binding or the actor default and emits a typed non-gameplay diagnostic.

Concept portraits may use a larger illustrative canvas and pose. They must preserve the selected outfit's identity landmarks but are never sliced into gameplay frames. Gameplay atlases obey the approved 18 PPU/cell/baseline/point-sampling manifest. The mapping is cosmetic: selecting or swapping a binding may change renderer assets, clips, portrait, garment-motion metadata, and cosmetic VFX only. It must leave transform, Rigidbody2D, collider, hurt/hit/aim/transfer geometry, stats, damage, cooldowns, movement constants, AI, phase, targeting identity, and sorting semantics unchanged.

### Tick phase and visual motion

Presentation observes a completed immutable actor snapshot for tick `T` after the actor's movement/combat/phase owner has committed `T`. It derives a visual pose for that same completed tick and may publish only after validating actor ID, action ID, facing, action age, and snapshot tick. It cannot call simulation owners, enqueue commands, consume input, modify action age, or advance its own gameplay clock. Swapping costume at a UI-authorized boundary replaces only the binding while retaining actor tick, action ID, normalized action phase, facing, and world anchor. If the new binding lacks the current required clip, the swap rejects atomically and the old binding continues.

Secondary motion is authored per garment and integrated into the whole clothed figure. It responds to torso acceleration, step impact, landing, bow draw/release, turn, lean, or recovery and settles within that action. At a 32px body target, breast/cloth contour or attached-garment delay is bounded to `0..1` native pixel from its registered rest contour; zero motion is valid and preferred where one pixel harms silhouette or coverage. There is no fractional rendered displacement: any internal normalized value quantizes to authored integer frames. Motion may not expose anatomy, detach a body region, use an isolated crop, create a standalone loop, continue oscillating after the parent action settles, or serve as combat/UI feedback. All frames preserve costume coverage and whole-body balance. The manifest stores driver action, onset tick, peak tick, settle tick, native-pixel maximum, garment region, and reviewer disposition.

## Exact first-slice implementation allowlist

The first implementation slice is pure core plus a simple deterministic canonical codec. Only new files under these paths may be added:

- `Assets/AcadeGameMaker/Runtime/Costumes/**` and required `.meta`: engine-free catalog, state, issuer/grant, selection, recovery transformation values, and simple canonical codec;
- `Assets/AcadeGameMaker/Runtime/Presentation/**` and required `.meta`, explicitly excluding `Runtime/Presentation/Unity/**`: engine-free atomic binding and completed-snapshot projection values;
- `Assets/AcadeGameMaker/Tests/EditMode/Costumes/**` and required `.meta`;
- `Assets/AcadeGameMaker/Tests/EditMode/Presentation/**` and required `.meta`;
- new `docs/verification/2026-09-13-costume-*.md` evidence and the minimum `docs/README.md` status/index hunk.

Everything else is forbidden in this slice, including existing files in Runtime/Tests, `Runtime/Presentation/Unity/**`, Editor tooling, images/media, scenes, prefabs, Packages, ProjectSettings, Profile/Run/movement/combat/input/camera sources, and actual filesystem adapters. The codec accepts/returns bytes or immutable text/value inputs only; it performs no path resolution, open/read/write/move/replace/quarantine operation. Actual primary/previous/temp file IO, Unity renderer/UI wiring, Editor tooling, and image production each require a later Approved child contract.

## Requirements

- **REQ-COST-001:** Inventory exactly the approved 36 Seryeong outfit IDs into a stable-ID manifest with source option/hash, per-outfit reference hair, and Accepted/ProductionPending/Rejected disposition; do not add hair combinations or silently omit a source.
- **REQ-COST-002:** The immutable catalog shall require one unique accepted default per actor and complete portrait/gameplay bindings before a costume is selectable.
- **REQ-COST-003:** Unlocks shall originate only from authored defaults or an internal opaque owner-bound trusted issuer on the closed Approved allowlist, with monotonic per-issuer revisions; no public minting, cryptographic claim, production unlock-all, or filename/display-order inference exists.
- **REQ-COST-004:** Locked, unknown, pending, rejected, stale, cross-actor, and forged selection attempts shall reject without state or presentation mutation; exact candidate commit is single-use and revision-bound.
- **REQ-COST-005:** Costume state shall persist in a versioned, canonical, atomic, integrity-checked lane isolated from active Profile and Run files, with explicit primary/previous/temp recovery and quarantine.
- **REQ-COST-006:** Recovery shall preserve valid unlocks, fall back invalid current selections to the unique accepted actor default, return read-only `NoAcceptedDefault` without any fabricated binding/unlock when none exists, record typed reasons, and never unlock the catalog wholesale.
- **REQ-COST-007:** Character-screen concept art and gameplay atlas shall publish as one revision-correlated atomic binding; partial, missing, mismatched, or stale mappings never publish.
- **REQ-COST-008:** Costume changes shall be visual-only and preserve all gameplay geometry, physics, stats, abilities, target identity, and simulation state.
- **REQ-COST-009:** Presentation shall consume completed actor tick snapshots, preserve action phase across a swap, and never reorder, advance, or mutate actor tick ownership.
- **REQ-COST-010:** Seryeong outfit sheets shall provide clear natural whole-body action poses and per-garment action-driven secondary motion bounded to `0..1` native pixel at the 32px body target, settling within the action with coverage preserved and no isolated erotic loop.
- **REQ-COST-011:** Native `1x` review shall fail closed when 32px cannot honestly convey an outfit or motion; a higher body-height profile remains a separately approved proposal with no silent scale/collider/camera change.
- **REQ-COST-012:** Settings and character-screen projections shall show exact locked/unlocked/current states and concept previews without granting, inferring, or preview-committing unavailable content.
- **REQ-COST-013:** Future NPC/enemy/boss packages shall be data-ready through actor/form/action manifests; boss and skill/phase VFX rows must declare authority, phase, telegraph/execute/recover, attachment, palette/sorting/light, tick-relative visual timing, and `combat_pending` where mechanics are unapproved.
- **REQ-COST-014:** Implementation shall be additive and respect the exact allowlist approved for each slice; active Profile/Run and existing movement/combat/input/camera ownership remain untouched.

## Acceptance criteria

- **AC-COST-001 (REQ-COST-001/002/011):** A read-only inventory report contains exactly the 36 declared lowercase IDs once each, accounts for source option/hash and per-outfit reference hair, and rejects an H variant or front/back view as an added costume. Native `1x`/nearest `4x` review verifies accepted outfit landmarks; unreadable 32px candidates remain pending and no rescale claim is made.
- **AC-COST-002 (REQ-COST-002/003):** Catalog tests reject duplicate/unsorted/invalid IDs, multiple accepted defaults, partial media mappings, and Accepted entries without hashes. A catalog with no accepted default remains valid only as `NoAcceptedDefault`; production API inspection proves no enumerate-and-grant/unlock-all route.
- **AC-COST-003 (REQ-COST-003/004):** Tests cover default grant, each closed issuer kind, valid monotonic grant, exact idempotent replay, conflicting replay, repeated/decreasing/overflow revision, public/string/reflection forgery, foreign capability/owner, locked/unknown/pending/rejected/cross-actor/stale selection, and forged/consumed candidate. Every rejection leaves byte-equivalent state and the prior presentation binding.
- **AC-COST-004 (REQ-COST-005/006):** Two identical pure-code encodes are byte-identical and decode to equal values. Immutable primary/previous/temp corruption/interruption fixtures prove deterministic transformation, default-only bootstrap, preservation of unrelated valid unlocks, typed current fallback, and `NoAcceptedDefault` with no binding/grant/unlock/current selection. Static and dynamic checks prove zero filesystem API use and zero writes to Profile/Run roots.
- **AC-COST-005 (REQ-COST-007):** Fault fixtures remove or mismatch portrait, atlas, clip, actor, hash, catalog revision, and presentation revision independently; no test observes a mixed portrait/atlas pair, and the last complete/default binding remains active.
- **AC-COST-006 (REQ-COST-008):** Before/after snapshots across every accepted costume compare exact transform, collider and Rigidbody2D authoring, hurt/hit/aim/transfer geometry, stats, cooldowns, movement values, action identity, target identity, and phase state; only approved visual fields differ.
- **AC-COST-007 (REQ-COST-009):** Fixed-tick tests swap during idle, locomotion, airborne/landing, bow draw/release/recover, turn, and recovery. Actor tick `T`, action ID/age/normalized phase, facing, and anchor remain exact; presentation publishes only after completed `T`, missing-current-clip swaps reject, and no extra simulation call occurs.
- **AC-COST-008 (REQ-COST-010):** Native contact sheets and frame-difference review measure every registered secondary region at `0` or `1` native pixel, correlate it to torso/garment action onset, and show settling by the registered tick. Static and visual review find no standalone loop, isolated crop, detached region, repeated post-settle oscillation, or coverage break.
- **AC-COST-009 (REQ-COST-012):** Settings and character-screen view-model tests show locked, unlocked, selected, pending, and load-fallback states. Concept preview and selection are distinct; previewing locked/pending content never yields a selection candidate or changes persisted state.
- **AC-COST-010 (REQ-COST-013):** Schema fixtures validate one NPC, one regular enemy, and one multi-phase boss package. Missing authority, phase transition, telegraph/execute/recover, attachment, palette/sorting/light, tick-relative visual timing, or pending marker fails validation; the fixture cannot express damage/hitbox/stat ownership.
- **AC-COST-011 (REQ-COST-014):** Static diff review proves only the Approved additive allowlist changed and finds no edit/reference cycle into existing Profile/Run/movement/combat/input/camera sources. Focused tests and full EditMode/PlayMode complete with failed/skipped/inconclusive `0`.
- **AC-COST-012 (all):** Luna independently reviews the contract mapping, adversarial fixtures, native visual evidence, and regression results. Terra's own implementation or art review cannot satisfy independent acceptance; Astra alone changes status to Verified.

## Traceability

| Requirement | Acceptance evidence |
|---|---|
| REQ-COST-001 | AC-COST-001 |
| REQ-COST-002 | AC-COST-001, AC-COST-002 |
| REQ-COST-003 | AC-COST-002, AC-COST-003 |
| REQ-COST-004 | AC-COST-003 |
| REQ-COST-005 | AC-COST-004 |
| REQ-COST-006 | AC-COST-004 |
| REQ-COST-007 | AC-COST-005 |
| REQ-COST-008 | AC-COST-006 |
| REQ-COST-009 | AC-COST-007 |
| REQ-COST-010 | AC-COST-008 |
| REQ-COST-011 | AC-COST-001 |
| REQ-COST-012 | AC-COST-009 |
| REQ-COST-013 | AC-COST-010 |
| REQ-COST-014 | AC-COST-011, AC-COST-012 |

## Gate, sequencing, and stop conditions

1. Astra confirms this exact `seryeong`/36-entry contract and the stated first-slice allowlist against the Approved source-generation inventory.
2. Luna pre-gates ID stability, unlock forgery/replay, recovery, atomic media mapping, tick order, visual-only invariants, and native-scale motion evidence.
3. Astra resolves findings and changes this contract or a narrower child contract to Approved.
4. Terra implements pure catalog/state/selection and tests first. Persistence, Unity UI/presentation wiring, and image production remain separate child slices so concurrent Profile/Run work is untouched.
5. Image processing begins only after the pending user permission. Every sheet remains `offline draft — pending visual acceptance` until the user/Astra visual gate and Luna evidence review.
6. Terra supplies REQ/AC-linked evidence; Luna independently reruns relevant checks; Astra decides integration and status.

Stop and return to Astra if a source cannot be mapped to exactly one of the 36 IDs, 32px review fails, a required action has no Approved mechanic authority, a presentation adapter would need to edit an existing simulation owner, Profile schema modification or actual filesystem IO appears necessary, or the exact first-slice allowlist cannot avoid concurrently active files.

## Participation record

Sol drafted this Review contract from the current documentation authority and a read-only inspection of Profile, Run, movement, and presentation-related repository seams. No runtime, test, asset, image, scene, prefab, Profile, or Run file was edited. No implementation, visual acceptance, Unity execution, external model call, network call, or image processing is claimed. Astra approved the first pure-code slice after the pre-gate corrections. Subsequent implementation, post-review, image production and integration evidence are recorded separately; this drafting record is not acceptance of those results.



