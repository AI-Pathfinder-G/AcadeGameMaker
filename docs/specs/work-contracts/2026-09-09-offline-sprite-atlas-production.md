# Offline character sprite-atlas production contract

- Status: Approved
- Date: 2026-09-09
- Authority/approval: Astra
- Intended production: Terra after this contract becomes `Approved`
- Independent verification: Luna; a producer cannot accept its own output
- Parents: `REQ-ART-002/005/008/009/014`, art-direction and narrative canon, ADR-0020/0021/0022/0027/0029

## Bounded outcome

Produce honest, inspectable pixel-art character sheets and offline previews from `images/characters/story-realignment-v1`, with Seoha boss appearance fixed to B6 v7 and village/NPC appearance fixed to B3 v5. This contract authorizes no Unity scene, prefab, importer, Animator, controller, runtime, collider, combat, input, camera, package, or project-setting change. Generated concept art is reference, not a frame grid and not evidence of gameplay.

The deliverable is an 18-sheet offline draft art package: lossless indexed/RGBA PNG sheets, a deterministic machine-readable manifest, contact-sheet previews, and validation evidence. Native sprites remain **offline draft, pending visual acceptance**. Source generations that cannot be separated into an exact grid are retained under a quarantined raw-source directory and never renamed or presented as gameplay-ready or playable sheets.

## Native scale and packing profile

The approved world density is 18 PPU on a 640x360 logical canvas. The currently authored player `CapsuleCollider2D` is `0.8x1.6u`, or `14.4x28.8` logical pixels. It is collision evidence, not permission to alter collision or force every silhouette inside it.

- Standard humanoid cell: `64x64px`; standing body target `32px` sole-to-crown, with the sole baseline at local `y=52`. Pivot metadata is `(32,52)` in top-left image coordinates. The remaining transparent margin belongs to hair, coat, weapon and anticipation poses; it must not be mistaken for a 64px-tall actor. At 18 PPU the nominal figure is about `1.78u` tall while collision remains independently owned.
- Wide humanoid/weapon cell: `96x64px`, same baseline and `32px` nominal body height.
- Large humanoid boss cell: `128x128px`; creature cell: `192x128px`. Larger source motion must use another documented integer multiple of 32px, never a hidden nonuniform scale.
- Every animation occupies one complete row. Cells are equal within a sheet, transparent padding is preserved, and unused trailing cells are explicit transparent cells in metadata. Pixel edges use nearest-neighbor only; no bilinear resampling, subpixel translation, rotation, antialiasing, or fractional frame bounds.
- A sheet may be split when the maximum texture width would exceed `2048px`; deterministic part order is lexical animation ID then frame index. Mirroring is metadata only and is forbidden where asymmetric costume, gear, injury, or hand use would become false.

Raw `1536x1024` generations are concept sources only, never accepted atlases. For a declared source with six columns and `R` rows, default extraction boundaries are integer half-open rectangles using `x_i=floor(i*width/6)` for `i=0..6` and `y_j=floor(j*height/R)` for `j=0..R`. A source manifest may replace these with explicit manually reviewed integer boundaries when generated gutters or layout drift require it; the override records every boundary, reviewer, reason and source hash. Extraction then isolates alpha, crops/pads, and applies one uniform nearest-neighbor scale chosen to reach the nominal body-height target. That scale need not be an integer because the raw image is concept material; the accepted result must still occupy the exact final cell, preserve aspect ratio and contain only integer-positioned final pixels. Per-axis scaling, interpolation and implicit boundary guessing are forbidden.

## Required animation vocabulary

This bounded delivery follows the 18-sheet action plan below, with six key poses per row and either three or four declared rows per source sheet. These are offline animation assets, not complete production coverage for an entire player controller. Unprovided left-facing, ladder, wall, defeat and transition clips are listed as pending instead of fabricated. Defeated living NPCs and displaced creatures must not receive an invented terminal outcome. Preview timing is manifest data in integer ticks and includes readable anticipation and recovery; art does not define hit, hurt, invulnerability, movement, or target geometry.

The exact generation plan is `images/sprites/production-v1/generation-plan.json`: 01 Doeon idle, 02 Seryeong idle, 03 Seoha NPC idle, 04 lovers, 05 lovers shop, 06 Ain/Yudam, 07 Iseon/tailor/courier, 08 Mujin hub, 09 Doeon movement, 10 Doeon combat, 11 Seryeong movement/combat, 12 Seoha boss, 13 Ordan, 14 Taegan, 15 Chaeryun, 16 Mujin combat, 17 Ragen, 18 ecological bosses. Rows with multiple phases receive semantic pose labels rather than being mislabeled as complete run loops. A sheet may carry different actors only when each row declares its actor; relationship rows explicitly contain a group.

Village-only characters receive their listed six-frame role actions; additional generic idle/walk/talk clips are pending unless present in the plan. Dual-role characters keep separate costume/form sheets and stable identity; costume changes are not palette swaps. Every produced row has exactly six distinct source key poses; do not duplicate frames to simulate missing coverage.

| Character/form | Required role-aware additions |
|---|---|
| Doeon | weight-transfer aim/commit/recover; gear repair, thinking, and look upward toward Ragen, each six frames |
| Seryeong | bow aim/draw/release/recover; forward stretch, arms folded while looking toward Doeon, and writing, each six frames |
| Seoha B6 v7 boss | gravity-reel attacks with two distinct telegraph/execute/recover sets; phase transition |
| Seoha B3 v5 NPC | slight forward flirt 6; table lean 6; shop idle/talk/react |
| Ordan | seizure-weight signature and recovery, preserving square enforcement gear |
| Taegan | rescue staff/winch action and recovery, preserving triangular rescue silhouette |
| Chaeryun | only a signature sequence grounded in the approved story/design source; otherwise mark `combat_pending` and do not invent an ability |
| Mujin common/boss/released | distinct form sheets; supply-apparatus boss sequence and transition grounded in the adopted reference |
| Ragen ruler/final duel | ruler idle; direct-duel locomotion, attack, stagger and recovery, preserving white forelock, right-knee brace and rescue handle |
| Ain past/axis | past walk/react; axis sustain, refusal/react and release recovery; no helpless-loop framing |
| Yudam | ledger guard/write/present/refuse; combat rows only if an approved combat contract exists |
| Iseon | facility inspect/operate/react; `combat_pending` unless approved elsewhere |
| Gravity tailor | measure, stitch, fitting react |
| Vertical courier | load parcel, brace, dispatch/return |
| Baekgak, wind-sac mother | one planned ecological-boss sheet: six-frame rows for idle/locomotion and telegraph/execute/recover; hurt, defeat and further coverage are pending; preserve anatomy and warning-pose continuity from `07-boss-creatures.png` |

`combat_pending` is a valid manifest state and is preferable to fabricating a move. “All skill-aware” means each produced combat motion is traceable to an authority class below; it does not authorize new mechanics.

| Motion source | Citation and authority class |
|---|---|
| Doeon basic attack and weight transfer | Approved vertical-demo combat/weight-transfer contracts: `docs/specs/vertical-demo/03-combat-and-enemies.md`, `docs/specs/vertical-demo/02-weight-transfer.md`, and applicable Approved VD-02/VD-03 work contracts. Visual poses may illustrate these mechanics without changing them. |
| Ordan | Approved `docs/specs/work-contracts/2026-09-05-vd03-combat-m4a-ordan-boss-core.md` and subsequent Approved/Verified M4B contracts. Visual timing does not redefine their tick/geometry authority. |
| Seryeong, Seoha, Taegan, Mujin, Ragen and ecological bosses | Adopted character concept sheets plus `docs/proposals/2026-09-09-eight-chapter-synopsis-realignment.md` and applicable current story/art proposals. These are **visual-proposal citations only**, not Approved runtime skills or combat behavior. |
| Chaeryun | Six-frame mechanical keyplate manipulation/attack-read poses grounded only in the adopted concept and latest story proposal. This is **visual proposal only**; runtime ability, effect, target and timing remain `combat_pending`. |
| Ain, Yudam, Iseon, tailor and courier | Role/interaction poses only under this plan. Combat remains pending unless a future Approved contract supplies it. |

## Relationship and village interaction clips

Pair animations use shared `96x64px` cells, shop triplets use `128x64px`. They retain nominal actor anchors in metadata and are explicitly flattened group art, not independently movable actor layers. Required lovers variants are `arm_link`, `surprise_cheek_kiss` with both visibly blushing, and `handhold`, each with six source key poses. The kiss has approach, contact, mutual reaction, and recovery; it is affectionate and non-looping. The lovers-shop clip shows Seryeong arms folded glaring at Seoha with Doeon/Seoha readable. Metadata gates `lovers` and `lovers_shop` do not decide bond, purchase, dialogue, or branch state.

Seryeong and Seoha may have restrained secondary breast/clothing settling integrated into whole-body anticipation, locomotion, landing, leaning, and recovery. Seoha may read fuller than Seryeong, consistent with the adopted designs. At the 32px body target this is normally a `0..1px` delayed settle accompanying torso/clothing motion. Secondary motion must follow torso acceleration, settle within the same action, preserve costume coverage, and never exist as a standalone loop, isolated crop, exaggerated repeated oscillation, or gameplay cue.

## Manifest and provenance

`atlas-manifest.json` uses UTF-8, sorted keys, LF, and stable lexical ordering. It records schema version; character/form/costume IDs; reference paths and SHA-256; canvas/cell/sheet dimensions; part, row, animation and frame indices; frame rectangle; baseline/pivot and pair anchors; duration ticks; loop flag; facing and mirror permission; source ability/role citation; narrative gate label; palette/outline profile; generator/tool/version; output SHA-256; and validation disposition. Packing repeated from identical source bytes, manifest, and tool version must produce byte-identical PNG and manifest outputs.

Generated references are project-authored inputs but still retain prompt/reference provenance. Existing files and media are immutable inputs. Raw generations, extraction attempts, masks, and rejected variants remain in a dated quarantine subtree with hashes and disposition; production output never overwrites them.

## Requirements

- **REQ-SPR-001:** All sheets shall conform to the 18 PPU, 640x360, cell, baseline, pivot, point-sampling, outline, and deterministic row-packing rules above.
- **REQ-SPR-002:** The exact 18-sheet plan shall contain six distinct key poses in every produced row. Every produced combat form shall include visually labeled telegraph, execute and recovery poses and cite either an Approved mechanic contract or an explicitly labeled visual-proposal source; all unprovided coverage shall remain pending.
- **REQ-SPR-003:** Doeon, Seryeong, Seoha and pair/shop interactions shall include the exact listed village and relationship actions and explicit gate labels.
- **REQ-SPR-004:** Seoha shall use B6 v7 for boss frames and B3 v5 for NPC/shop frames; discarded Seoha variants shall not be silently mixed into either form.
- **REQ-SPR-005:** Seryeong/Seoha secondary motion shall remain action-driven, clothed, settling, and non-standalone as defined above.
- **REQ-SPR-006:** Each accepted offline-draft sheet shall use the declared floor-derived or reviewed manual integer extraction bounds, uniform nearest-neighbor normalization into exact cells, transparent separation, stable pivots/anchors, no fused figures or partial adjacent frames, and no text, captions, background, watermark, or concept-layout remnants.
- **REQ-SPR-007:** The manifest and packer shall be deterministic, provenance-complete, hash-verified, and able to distinguish offline-draft, pending, `combat_pending`, and quarantined outputs.
- **REQ-SPR-008:** Offline previews shall show every frame at native 1x and nearest-neighbor 4x, plus representative animations composited on a 640x360 logical stage with pixel grid and pivot/baseline overlay toggles.
- **REQ-SPR-009:** Evidence and native assets shall be labeled `offline draft — pending visual acceptance` and describe only offline file/grid/visual results. They shall not claim gameplay-ready status, Unity import, Animator wiring, collider fit, runtime playback, combat timing, GPU capture, or playability.
- **REQ-SPR-010:** Any generation/extraction failure shall fail closed: preserve and hash raw material in quarantine, mark the missing animation, and publish no substituted, duplicated, stretched, or misleading “complete” sheet.

## Acceptance criteria

- **AC-SPR-001 (REQ-SPR-001/006):** A validator enumerates every PNG rectangle, reproduces default floor boundaries or verifies the complete manual override record, and proves exact final cell bounds, uniform nearest-neighbor normalization, alpha-separated cells, stable baseline/pivot, approved palette/outline profile, and zero interpolated or out-of-grid pixels; visual review confirms silhouettes at 1x and 4x.
- **AC-SPR-002 (REQ-SPR-002/004):** The manifest reports exactly 18 planned sheets and six source poses per produced row, flags all omitted coverage as pending, distinguishes Approved-mechanic citations from visual-proposal citations, and image review confirms B6 v7 boss and B3 v5 NPC costume landmarks without cross-form contamination.
- **AC-SPR-003 (REQ-SPR-003/005):** Contact sheets and animated previews show every exact solo/pair/shop action, mutual blush in the surprise kiss, correct pair anchors, and action-driven secondary settling with no standalone erotic loop.
- **AC-SPR-004 (REQ-SPR-007):** Two clean offline packs from identical inputs compare byte-for-byte for manifests and PNG hashes; changing one source changes only its declared dependent outputs and package index.
- **AC-SPR-005 (REQ-SPR-008/009):** Preview evidence includes native and 4x contact sheets plus 640x360 stage captures, labels every native/preview output `offline draft — pending visual acceptance`, and contains no gameplay-ready, runtime or playable claim.
- **AC-SPR-006 (REQ-SPR-010):** Fault fixtures for fused frames, missing alpha, wrong canvas, wrong Seoha reference, text remnants, nondeterministic ordering, and incomplete generation are rejected into hashed quarantine without altering accepted outputs.

## Approval and implementation gate

Astra approved bounded offline production on 2026-09-09 after source-authority and extraction review. Script-based image processing remains pending explicit user permission; preparing the packer and read-only inspection are authorized. Astra may approve only after confirming the animation matrix against current canon and applicable Approved ability contracts. After approval, Terra may implement only the offline generator/packer/validator and art outputs named here; Luna independently reviews AC-SPR-001..006 evidence. A separate Approved Unity integration contract is required before any import settings, Sprite metadata, Animator, scene, prefab, renderer, collider, or runtime change.

