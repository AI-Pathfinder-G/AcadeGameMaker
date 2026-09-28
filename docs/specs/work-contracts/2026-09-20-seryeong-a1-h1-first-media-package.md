---
status: Approved
---

# Seryeong A1/H1 first vertical-demo media slice

- Date: 2026-09-20
- Status: Approved — offline tooling and non-wearable staging only; later Unity publication remains gated
- Decision authority: ADR-0033, ADR-0034 and the user's instructions to use the recommended density split and A1 initial default
- Design/counter-review: Sol
- Intended producer and implementation tests: Terra
- Independent verifier: Luna
- Parents: `REQ-COST-001/002/007..012/014`, `REQ-AP64-001..004`, `REQ-SPR-001/005..010`, `REQ-APV3-001..010`, ADR-0033, VD-06
- Requirement IDs: `REQ-SM1-001..010`
- Acceptance IDs: `AC-SM1-001..010`
- Rollback point: current `source_only` A1/H1 inventory with APV3 Accepted count `0/432`

## Bounded outcome and non-claims

Produce one hash-pinned, import-ready **vertical-demo-only** Seryeong media slice for `seryeong.costume.a1.v1` with its authored H1 hair identity. The indivisible offline package contains:

1. one 128×128 transparent PNG wardrobe preview, 64px sole-to-crown, using the exact reviewed idle source frame;
2. one transparent gameplay atlas with 64×64 cells and a 32px sole-to-crown body, containing exactly six nonduplicate `idle`/`left` frames and six nonduplicate `idle`/`right` frames;
3. one canonical clip-map JSON and package manifest binding identity, geometry, hashes, provenance and rights; and
4. native `1x`, nearest-neighbor `4x`, looping-idle, and 640×360 stage-review evidence.

The package disposition is `AcceptedForVerticalDemoMedia`, distinct from APV3 combination acceptance and COST catalog disposition. APV3 `acceptedCombinationCount` stays **0/432**. This slice does not complete `seryeong.appearance.a1.h1.v3`, which still lacks the broader locomotion, combat, village, relationship, reaction and shop matrix. It does not make Seryeong playable, a combat sidekick, selectable or wearable.

Offline acceptance changes no catalog row. Any future COST `ProductionPending → Accepted` change belongs only to a later hash-pinned Unity import/publication child and means “complete for that child's approved runtime required clip map”, not APV3 completion.

## Current runtime boundary

The approved vertical demo exposes Seryeong only after `DemoCompleted` in her first-tracker scene. No Approved current scene requires her locomotion or combat. Therefore this slice provides only idle presentation. `walk`, `run`, `jump`, `land`, evade, hit, bow, village, relationship and shop clips remain out of scope. Adding one requires a later scene/runtime contract and a new media revision.

The canonical clip map has two rows with the same `actionId=idle` and distinct `facing=left|right`; each row uses exact property order `actionId,facing,loop,frames`. The pure binding's required action vocabulary remains `idle`. The later presentation adapter resolves the completed snapshot tuple `(ActionId=idle,Facing=left|right)` to the matching row without manufacturing a compound action ID. Unknown action/facing combinations fail closed and retain the prior complete binding.

## Exact source pins

The only pixel-authority candidate for the idle slice is movement row 1 of:

- raw movement: `images/sprites/seryeong-appearance-v3/native/a1/h1/raw/movement-source-v1.png` — SHA-256 `a2939b925999aaf9634513862456e75912cb40fb4d638b2fe6584d28d3448dc6`;
- raw provenance: `images/sprites/seryeong-appearance-v3/native/a1/h1/raw/provenance.json` — SHA-256 `a68fce849cd740aaaed246de61c48ad024a780a65134e2b75eb765b1ea824766`;
- A1 costume reference/source option — SHA-256 `951310f8cb41f126e192ea6de74ab8b297a5f95d18be158d8d98503775da5cc1`;
- H1 reference `images/seryeong/shoulder-v2/hair-v1/01-loose-and-halfup.png`, figure index `0` — SHA-256 `da047fa24b06cfcb7eb6c3516a3e6b17145bf3cb954be416c8e2685f8b96b60d`;
- reviewed calibration — SHA-256 `414aaac2d134ff86c2ccfa645517ce606722a3b6f13ccfaac49cbb32b60735f9`;
- current v4 64px diagnostic candidate — SHA-256 `2564dd72375b2a4e9c4980cb9ac5d36cedd52db3df6509c6e338e43ed9a63b41`.

The last hash is a `source_only` diagnostic input, not an accepted output hash. Existing 32px drafts under `source-reference/movement/` and `source-reference-v3-components/movement/` are comparison inputs only. The earlier candidate-media record pointed to a nonexistent v4 32px path; its source-only pointer is corrected to the existing v3-components comparison, without acceptance credit.

The 64px wardrobe preview uses movement row 1, column 1 — zero-based source frame index `0` — unless independent review rejects that exact frame, in which case the contract stops for amendment rather than silently choosing another frame.

## Exclusions and missing source

No pixels or acceptance credit may come from:

- `combat-source-v1.png` or any bow/combat source;
- `candidate.a1.h1.shop`, which is manifest-rejected;
- relationship v1 (`RejectedScale`), shop v1 (`RejectedHairRow2`), or source-only relationship v2/shop v2;
- reaction/group diagnostics, another hair/costume/palette, or a substituted pose;
- an automatic downscale, background-removal result, or blind horizontal mirror without frame-by-frame manual correction and review.

The raw source is authored right-facing only. The complete six-frame `idle.left` row is a missing production input. It must be separately authored from the same A1/H1 identity and reviewed for shirt closure, belt, H1 side part, silhouette and motion continuity. Runtime/importer mirroring is forbidden for this package.

## Exact geometry and clip scope

- Gameplay atlas: exactly `384×128` pixels, 6 columns × 2 rows, 64×64 cells; row 0 is `(idle,left)`, row 1 is `(idle,right)`.
- Each row has exactly six visually distinct frames in chronological order. Duplicate pixels may occur only where a documented hold is visibly intentional; a copied frame cannot fill missing production.
- Gameplay body target: exactly 32 pixels sole-to-crown; source top-left baseline `y=52`; source top-left pivot `(32,52)`; normalized Unity bottom-left pivot `(0.5,0.1875)`; 18 PPU.
- Wardrobe preview: exactly `128×128`, 64px sole-to-crown; source top-left baseline `y=104`; source top-left pivot `(64,104)`; normalized pivot `(0.5,0.1875)`.
- In this package the existing `CostumePresentationBindingV1.PortraitSetId` payload is the canonical 64px wardrobe preview, not unrelated concept art.
- Unity settings for the later child are Sprite/Multiple, point filtering, no compression, no mipmaps, alpha preserved, explicit custom pivots. The 64px preview is UI-only and cannot be bound to a world renderer.
- 32px output may begin from a nearest-neighbor reduction but requires native manual correction for face, H1 hair, blouse collar/cuffs, shorts/belt, boots, silhouette, feet and frame continuity.
- Hair/cloth settling follows the whole idle action, is limited to `0..1` native gameplay pixel, and cannot be a detached loop or gameplay cue.

## Canonical package, provenance and rights

The package uses portrait set `seryeong.costume.a1.v1.portrait.v1`, gameplay set `seryeong.costume.a1.v1.atlas.v1`, media revision `1`, lowercase SHA-256 of the exact preview PNG, atlas PNG and canonical UTF-8 clip-map bytes, plus all source hashes above.

The rights chain is `docs/evidence/2026-09-20-seryeong-a1-h1-rights-chain.md` and is `ProjectUseAuthorized` for this project's use and modification. The final use-right record is mandatory and contains at least:

- creator/provider, model/tool/version, generation or acquisition date;
- terms/license identifier and a retained evidence path plus SHA-256; the current OpenAI-output basis is pinned by the rights-chain record;
- internal-use and modification permission basis;
- attribution and restriction fields;
- every reference image's creator/source/hash and rights chain;
- producer, independent reviewer, review date, rejection and supersession links.

Missing, contradictory or unverifiable rights data keeps the slice pending. Exact historical model/version may be recorded as `historical-unavailable` only when prompt, generated-image path, reference path and immutable hashes survive; it must not be guessed. Any later-introduced external reference requires its own evidence and cannot inherit this authorization.

Preview, atlas, clip map and manifest form one logical publication revision. Offline files may be written separately, but all remain non-wearable, non-addressable staging until every package validation passes. The later Unity child stages unreferenced assets first and publishes only one catalog/binding revision after media, importer metadata, manifest, clip map and hashes all pass. Failure leaves the catalog pending, renderer/preview on the prior complete binding, and staging assets unreachable.

## Default and wearability decision

Every row in the current runtime `CostumeCatalogV1.SeryeongInventoryPending()`, including `story` and A1, has `defaultGrant=false`; the runtime catalog therefore truthfully remains `NoAcceptedDefault`. The separate APV3 inventory's authored `story.defaultGrant=true` is source-planning metadata and does not mutate the runtime catalog. Even a later A1 media import cannot make it player-wearable by itself.

ADR-0034 resolves the choice: A1 is the initial default costume for the vertical demo. A later Approved Unity import/publication child may set the A1 row to `Accepted` and `defaultGrant=true` only after this media package and every import/publication gate pass. Until then the runtime catalog remains `NoAcceptedDefault`; this offline contract does not alter catalog code. The `story` costume remains a future wardrobe/story option.

## Phase allowlist and dependencies

Under this Approved contract, Terra may change only:

- `tools/seryeong_first_media/**`;
- `images/sprites/seryeong-appearance-v3/production/a1-h1-v1/**`;
- `images/sprites/seryeong-appearance-v3/manifest/a1-h1-package-v1.json`;
- `docs/verification/2026-09-20-seryeong-a1-h1-*.md`;
- the minimum index/status links in `docs/README.md`.

Raw/source-only inputs are immutable. Rejected attempts go under `production/a1-h1-v1/quarantine/` with hashes and reasons.

No Unity raster, `.meta`, catalog row, scene, prefab, animator, renderer, wardrobe screen or gameplay binding may change in this offline phase. The later import child remains blocked until CUA, CIO and M5D7Q0 each reach independently reviewed `Verified` status; the default/wearability choice itself is resolved by ADR-0034.

Approval does not authorize a fabricated left-facing row. Project-use rights are recorded, but until the separately authored left row and all package checks pass, Terra may implement deterministic packers/validators, reproduce hash-pinned diagnostics from existing sources, and stage incomplete candidates as `pending`; it may not mark the package `AcceptedForVerticalDemoMedia`, publish it to Unity, or make it addressable or wearable.

## Requirements

- **REQ-SM1-001:** The slice identity is exactly Seryeong A1/H1 and preserves all pinned visual landmarks and sources.
- **REQ-SM1-002:** It contains one 64px UI preview and one manually corrected 32px idle-only two-facing atlas under ADR-0033 without PPU, camera, collider or resolution changes.
- **REQ-SM1-003:** The atlas contains exactly six `(idle,left)` and six `(idle,right)` frames; missing facing coverage is never blindly mirrored, duplicated or fabricated.
- **REQ-SM1-004:** Geometry, pivot, baseline, alpha, sampling and cell bounds follow the exact rules above.
- **REQ-SM1-005:** Preview, atlas, clip map, identity, media revision and hashes form one logical package and publication boundary.
- **REQ-SM1-006:** Complete source provenance and project-use rights evidence exists for every contributing source.
- **REQ-SM1-007:** Native review rejects identity drift, unreadable 32px landmarks, contamination, clipping, detached settling or mismatched directions.
- **REQ-SM1-008:** Combat, relationship, reaction, shop, other-hair and other-costume media contribute no pixels or acceptance credit.
- **REQ-SM1-009:** Offline completion is `AcceptedForVerticalDemoMedia` only; APV3 stays `0/432`, catalog stays pending, and the later import/publication remains separately Approved and hash-pinned.
- **REQ-SM1-010:** Work stays inside the phase allowlist and preserves raw inputs.

## Acceptance criteria

- **AC-SM1-001 (REQ-SM1-001/008):** Manifest and contact-sheet audit proves all 13 payload frames are A1/H1 and contain no excluded source or identity drift.
- **AC-SM1-002 (REQ-SM1-002/004):** Validators prove exact preview/atlas dimensions, body targets, pivots, baselines, transparent bounds and point-sampled integer placement; stage review proves unchanged 640×360/18 PPU world scale.
- **AC-SM1-003 (REQ-SM1-003):** The matrix contains exactly two canonical rows and twelve ordered frames: `(idle,left)=6`, `(idle,right)=6`; missing, duplicate, unknown or noncanonical action/facing rows reject.
- **AC-SM1-004 (REQ-SM1-003/007):** Native `1x`, 4×, animation and overlay review confirms left frames are separately corrected and preserve closure, belt, hair-part and motion continuity.
- **AC-SM1-005 (REQ-SM1-004/007):** Frame review confirms stable feet, no cell bleed/crop/magenta fringe, readable A1/H1 landmarks and bounded settling.
- **AC-SM1-006 (REQ-SM1-005):** Two clean builds are byte-identical; corruption of any payload/hash/revision/identity rejects the whole package without changing an accepted set.
- **AC-SM1-007 (REQ-SM1-006):** Rights audit accounts for every source hash and has no unresolved third-party contribution or missing authorization.
- **AC-SM1-008 (REQ-SM1-008/009):** Static audit proves excluded media is absent, APV3 remains `0/432`, the COST catalog remains pending/`NoAcceptedDefault`, and no Unity/catalog file changed.
- **AC-SM1-009 (REQ-SM1-009):** A later import child stages unreferenced assets, verifies exact hashes/settings, then logically publishes at most one catalog/binding revision; no placeholder or partial package becomes wearable.
- **AC-SM1-010 (all):** Terra supplies evidence, Luna independently reports P0/P1 zero against `AC-SM1-001..009`, and Astra records final disposition.

## Stop conditions

Stop offline production and keep all runtime/catalog truth pending if left-facing authoring is unavailable, a newly introduced source/right cannot be resolved, A1/H1 is unreadable at 32px, or a clean rebuild is nondeterministic. CUA/CIO/M5D7Q0 `Verified` status is a later Unity-import/publication gate; it does not block approved offline art generation.
