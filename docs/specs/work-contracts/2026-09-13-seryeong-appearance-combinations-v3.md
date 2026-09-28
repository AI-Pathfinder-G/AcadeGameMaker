---
status: Approved
---

# Seryeong appearance combinations v3 child contract

- Date: 2026-09-13
- Status: Approved — Astra 2026-09-13 after the bounded Luna pregate; first-slice diagnostics, inventory and offline production only.
- Final authority: Astra
- Intended implementation / independent verification: Terra / Luna
- Parent: [costume presentation and sprite production](./2026-09-13-costume-presentation-and-sprite-production.md), [Seryeong costume source generation](./2026-09-13-seryeong-costume-source-generation.md), and [offline sprite-atlas production](./2026-09-09-offline-sprite-atlas-production.md)
- Supersession: the user's 2026-09-13 instruction supersedes the parent's prohibition on hair combinations and the source contract's single-reference-hair production assumption for this child only. It also grants script image-postprocessing permission within the exact allowlist below.
- Requirement IDs: `REQ-APV3-001..010`; acceptance IDs: `AC-APV3-001..010`
- Current readiness: source-only; `0/432` appearance combinations Accepted. Existing pose sheets, concepts, manifests, or filenames are not native-atlas acceptance.

## Outcome and scope

Produce and independently verify all `36 costumes × 12 hair styles = 432` Seryeong appearance combinations. The costume inventory remains exactly `story`; `a1..a3`; `c1..c3`; `d1..d3`; `v1..v4`; `t1..t4`; `l1..l4`; `b1..b4`; `x1..x4`; `s1..s3`; `r1..r3`. The hair inventory is exactly `h1..h12` from `images/seryeong/shoulder-v2/hair-v1/`.

An appearance identity is the ordered pair `(costumeId, hairId)`, with stable ID `seryeong.appearance.<costume-slug>.<hair-slug>.v3`. Hair does not become a costume, grant, unlock, or silent thirteenth style. Each costume's original reference hair is preserved as provenance metadata (`referenceHairId` or `referenceHairSource`) and may equal one H ID; it does not create another combination. Front/back views belong to one combination and both must be accounted for.

Required action coverage applies independently to every one of the 432 combinations:

- normal locomotion: idle, walk, run, jump, land, evade/dash and recover;
- combat: bow aim, draw/hold, release and recover, plus hit/recover; these visuals add no charge, damage, timing or invulnerability mechanic;
- village/solo: forward stretch, folded-arm gaze toward Doeon, writing and supply gesture;
- Doeon relationship: `arm_link`, `cheek_kiss_mutual_blush` (approach, contact, mutual reaction, recovery), `handhold`, `prelove_jealous`, and `prelove_confused`;
- Seoha shop when lovers: `lovers_shop_glare`, `lovers_shop_block_flirt`, and `lovers_shop_pull_doeon_outside`;
- Seoha shop solo visit: `nonlovers_shop_absent` applies only when an external authored scene explicitly states Seryeong is absent and Doeon visits alone; not-lovers status alone must never hide her. Pre-love accompanied visits retain Seryeong and use the authored jealousy/confusion reactions. Doeon/Seoha or other required actors remain staged. This is presentation coverage only and may not alter relationship, presence, dialogue, purchase, branch, or narrative state.

Every required action has authored left- and right-facing evidence. Blind mirroring is forbidden for asymmetric clothing, hair, bow/hand dominance, closures, trims, hems, accessories, exposed shoulders, or relationship staging. A deliberate mirror is acceptable only when manifest review proves the entire composed result symmetric for that action and preserves anchors.

Layered composition and deterministic script postprocessing are permitted. Acceptance remains per final flattened combination/action/facing output: hair/cloth/face occlusion order, silhouette, coverage, baselines, pivots, hand/bow/contact anchors, partner anchors, integer pixels, gutters, palette/readability, frame continuity, and action-driven cloth/hair settling must all pass. Component acceptance, a representative sample, or a clean layer does not imply combination acceptance. Failed or missing output remains `source_only`, `production_pending`, or `rejected`; it is never copied, mirrored, stretched, relabeled, or counted as complete.

Production order is Seryeong first. Named NPCs, regular enemies, and bosses may begin only after all 432 Seryeong combinations and all required outputs are independently accepted, unless a later Astra-approved contract changes the order. Read-only planning is allowed.

## Active production profile override

The user subsequently selected 64px-or-higher detail. The Approved [64px production profile](./2026-09-13-seryeong-64px-production-profile.md) supersedes the 32px production target and comparison-only 64px status below. First-slice path limits and offline-only ownership remain; no Unity density setting is changed.

## Production contract

The manifest is the authority for the two inventories, Cartesian expansion, source hashes, reference-hair provenance, action/facing/frame recipe, layer order, occlusion masks, anchors, output hashes, review state, reviewer, and rejection reason. Recipes may reuse verified structural data, but every resulting PNG/frame entry has its own hash and review result. Native atlases use the parent contract's approved integer-cell, baseline, point-sampling and 18 PPU rules; this child does not change art density, camera framing, collider geometry, runtime timing, or sorting semantics.

Acceptance is transactional per appearance/action/facing set. A combination becomes `Accepted` only when its full required action matrix, both facings, contact sheets, native `1x` previews, deterministic rebuild receipt, and Luna review pass. Aggregate coverage is computed from accepted manifest records, never directory counts. Interrupted or invalid builds retain source hashes and diagnostics, do not overwrite accepted outputs, and publish no partial acceptance.

## Exact first-slice allowlist

Upon Astra changing this contract to `Approved`, the first production slice is exactly costumes `a1`, `a2`, `c2`, and `d3`, each with `h1..h12` (48 combinations), for the complete action/facing contract above. User permission for deterministic script image postprocessing is recorded; no further image-processing permission gate applies inside this slice.

The first diagnostic substep may process the existing 12 source sheets under each outfit/source-reference directory before H mapping. Such output is explicitly diagnostic, not one of the 48 accepted appearances. Background flood-fill masks and anchor proposals may be computed for review; a candidate preview may be written without claiming reviewed masks or final acceptance. Final atlas acceptance still requires hash-bound mask/anchor review. A separate comparison-only export at 64px standing body in 128px cells may accompany the required 32px export to evaluate readability; it is labeled density-study, is not the approved gameplay profile and changes no Unity/camera/PPU authoring. Use one standing-body-derived scale per source sheet, never per-pose bounding-box scaling; preserve airborne pose offsets with registered root anchors. Existing sources have near-magenta backgrounds; exact RGB key removal alone is invalid. Record classifier and residual-background checks. H1..H12 are labeled figures in two reference images, with explicit figure bounds and source hashes, not twelve separate source files.

Only new files under these four roots may be added:

1. `tools/seryeong_appearance_v3/**` — offline manifest expansion, reviewed extraction/composition, native atlas packing, validation, deterministic hashing, contact-sheet/preview generation, and tests;
2. `images/sprites/seryeong-appearance-v3/manifest/**` — complete 36-costume/12-hair/432-pair inventories and coverage recipes; extraction/occlusion/anchor records and build receipts limited to the 48 first-slice combinations;
3. `images/sprites/seryeong-appearance-v3/native/{a1,a2,c2,d3}/**` — generated native atlases, contact sheets, previews, and quarantine artifacts for only those 48 combinations;
4. `docs/verification/2026-09-13-seryeong-appearance-v3-*.md` — Terra evidence and Luna independent review citing the AC IDs below.

No existing file may be edited in the first slice except this contract's Astra status line. All Unity/runtime/Editor files, scenes, prefabs, import settings, `.meta` files, `Packages`, `ProjectSettings`, Profile, Run, movement, combat, input, camera, relationship logic, and narrative state are forbidden. Later costumes require a subsequent exact allowlist or an Astra-approved amendment after the 48-combination slice is independently reviewed.

## Requirements

- **REQ-APV3-001:** Enumerate exactly 36 costumes and H1..H12 and deterministically expand exactly 432 unique ordered appearance IDs; preserve reference hair solely as provenance metadata.
- **REQ-APV3-002:** Provide the complete normal, combat, village, Doeon relationship, lovers-shop, and non-lovers-shop presentation matrix for every combination without changing narrative or simulation behavior.
- **REQ-APV3-003:** Provide authored and verified left/right facings; prohibit blind mirrors wherever any visual or staging element is asymmetric.
- **REQ-APV3-004:** Permit deterministic extraction, alpha cleanup, layer composition, nearest-neighbor scaling and packing only from hash-bound reviewed sources and recipes; never overwrite sources or accepted output.
- **REQ-APV3-005:** Validate every flattened combination/action/facing for hair/cloth/face occlusion, coverage, anatomy/silhouette, baseline/pivot, hand/bow/contact/partner anchors, gutters, integer pixels, continuity and settling.
- **REQ-APV3-006:** Fail closed on missing, malformed, ambiguous or unreviewed inputs and outputs; do not substitute, duplicate, stretch, mirror, relabel or inflate completion counts.
- **REQ-APV3-007:** Publish native `1x` previews, contact sheets, hashes, deterministic rebuild receipts and per-output reviewer dispositions; directory presence is not acceptance.
- **REQ-APV3-008:** Keep all work offline and cosmetic, within the exact allowlist, with no Unity/runtime or gameplay/narrative authority.
- **REQ-APV3-009:** Complete and independently accept all 432 combinations before Seryeong production is called complete or NPC/enemy/boss image production begins.
- **REQ-APV3-010:** Limit the first slice to A1/A2/C2/D3 × H1..H12 = 48 combinations and the four allowed roots; retain current status as source-only with zero Accepted until evidence passes.

## Acceptance criteria

- **AC-APV3-001 (REQ-APV3-001/010):** Given the canonical inventory, expansion produces exactly 36 costume IDs, 12 hair IDs and 432 unique ordered pairs; the first-slice filter produces exactly 48, and reference-hair metadata changes neither count.
- **AC-APV3-002 (REQ-APV3-002/009):** A coverage report for each of 432 pairs contains every required action and state, including non-lovers absence/staging, with no missing or silently aliased entry.
- **AC-APV3-003 (REQ-APV3-003/005):** Native review of both facings for every output records asymmetry and proves correct garment/hair/bow/staging orientation; an unapproved mirror fails the set.
- **AC-APV3-004 (REQ-APV3-004/006):** Mutated source hash, missing review record, crossed extraction edge, invalid alpha, ambiguous layer order, or output collision fails before publication and leaves accepted files unchanged.
- **AC-APV3-005 (REQ-APV3-005):** Per-output review records pass hair/cloth/face occlusion and all body, baseline, pivot, bow, hand, contact and partner anchors at native `1x`; any failed frame prevents that combination's acceptance.
- **AC-APV3-006 (REQ-APV3-002/005):** Contact sheets and playback show readable locomotion/combat/village actions, mutual blush and contact/recovery beats, lovers-shop glare/block/pull staging, and stable non-lovers absence staging without detached or continuing cloth/hair motion.
- **AC-APV3-007 (REQ-APV3-004/007):** Two clean builds from identical reviewed inputs produce byte-identical atlases, manifests and hashes, while generated previews and receipts trace every output to sources and recipe.
- **AC-APV3-008 (REQ-APV3-006/007/010):** Missing or rejected outputs remain explicitly non-Accepted, accepted count is manifest-derived, and the repository truth remains `0/432` until Luna-cited evidence supports transitions.
- **AC-APV3-009 (REQ-APV3-008/010):** First-slice diff contains only new files below the four allowed roots plus the Astra status-line change, and contains no Unity/runtime/Editor or existing-media modification.
- **AC-APV3-010 (REQ-APV3-009):** Before any NPC/enemy/boss production write, a Luna report demonstrates `432/432` complete combinations and Astra records final Seryeong acceptance; otherwise the write is rejected.

## Traceability and gates

| Requirement | Acceptance |
|---|---|
| REQ-APV3-001 | AC-APV3-001 |
| REQ-APV3-002 | AC-APV3-002, AC-APV3-006 |
| REQ-APV3-003 | AC-APV3-003 |
| REQ-APV3-004 | AC-APV3-004, AC-APV3-007 |
| REQ-APV3-005 | AC-APV3-003, AC-APV3-005, AC-APV3-006 |
| REQ-APV3-006 | AC-APV3-004, AC-APV3-008 |
| REQ-APV3-007 | AC-APV3-007, AC-APV3-008 |
| REQ-APV3-008 | AC-APV3-009 |
| REQ-APV3-009 | AC-APV3-002, AC-APV3-010 |
| REQ-APV3-010 | AC-APV3-001, AC-APV3-008, AC-APV3-009 |

Open P0 decisions: none. Astra confirms the exact 432 matrix, first-slice 48-combination allowlist and separation from runtime/narrative authority. Pregate evidence: docs/verification/2026-09-13-seryeong-appearance-v3-pregate.md. Visual acceptance remains separate from permission to generate diagnostic candidates. Terra then implements; Luna must independently verify ACs; Astra alone accepts integration. No generated count, tool success, or source-sheet existence may be reported as production completion.





