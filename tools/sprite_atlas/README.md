# Offline sprite-atlas tools

Implements the bounded offline workflow in `docs/specs/work-contracts/2026-09-09-offline-sprite-atlas-production.md` (REQ-SPR-001, REQ-SPR-006, REQ-SPR-007, REQ-SPR-010).

These tools do not modify Unity assets and never claim runtime integration, gameplay readiness, or playability.

## Read-only preflight

Run from the repository root:

```powershell
node tools/sprite_atlas/atlas.js inspect
node tools/sprite_atlas/atlas.js validate-plan
node --test tools/sprite_atlas/test/*.test.js
```

`inspect` hashes and records the raw PNGs without transforming them. It reports all 18 planned sources and marks missing, malformed, or unreviewed sources as `pending`/`needs-review`.

## Processing gate

Image processing is deliberately blocked unless the person running it supplies the explicit acknowledgement below. This guard is for the pending user-permission gate, not an approval of the resulting art.

```powershell
node tools/sprite_atlas/atlas.js pack --confirm-user-image-processing --review-config images/sprites/production-v1/reviewed-extraction.json
node tools/sprite_atlas/atlas.js validate --output images/sprites/production-v1/offline-draft
```

`reviewed-extraction.json` must bind every accepted raw source SHA-256 to complete integer `xEdges` and `yEdges`, reviewer, and reason. Its shape is:

```json
{"version":1,"sheets":[{"jobId":"01-doeon-idle","sourceSha256":"…","xEdges":[0,256,512,768,1024,1280,1536],"yEdges":[0,341,682,1024],"reviewer":"name","reason":"visual grid review","approved":true}]}
```

No default grid is accepted without this record. A rejected source is copied, hash-recorded, and labelled under `quarantine/YYYY-MM-DD/`; accepted output is never overwritten. The packer uses RGB(255,0,255) only as a binary alpha key, crops each reviewed source rectangle, applies a uniform nearest-neighbor scale, and packs exact 64×64/96×64/128×128/192×128 cells with declared baselines and pivots.
## Previously generated, near-magenta source diagnostic (2026-11-09)

The existing production-v1 image generator did not reliably emit exact RGB(255,0,255)
empty pixels, even when the image prompt requested that value. The existing atlas.js
packer is exact-key-only and cannot treat these generated PNGs as valid extracted
transparent sprites without a reviewed source matte.

The new source-only diagnostic is intentionally separate from atlas.js pack and
does NOT bypass its user/review configuration gate, approve an atlas or touch Unity:

    python tools/sprite_atlas/inspect_generated_keyposes.py --source-root "H:\GPT-workspace\AcadeGameMaker\images\sprites\production-v1\raw" --output-dir output/ch1-art-audit-20261109/source-inspection-new
    python tools/sprite_atlas/render_stage_previews.py --preflight output/ch1-art-audit-20261109/source-inspection-new --generation-plan "H:\GPT-workspace\AcadeGameMaker\images\sprites\production-v1\generation-plan.json" --output-dir output/ch1-art-audit-20261109/preview-new
    python -m unittest discover -s tools/sprite_atlas/test -p test_generated_preflight.py -v

A sanitized production-v1 generation-plan.json is retained under images/sprites/production-v1/ so that the existing Node plan tests remain reproducible; it contains only six-pose row definitions, not any original user paths, image-generation prompts, or bitmap assets.

Requires Python 3.12+, Pillow and NumPy. The tool reads the original source
archive but writes previews only into a NEW output directory. It reads the planned
row count, checks frame edges, source hashes, and notes overdense/chroma backgrounds.
Outputs are source_only_needs_visual_review; they never become accepted gameplay
media and must not be copied wholesale to Assets. The 18-sheet 2026-11-09 review
is described in docs/art/2026-11-09-character-sprite-inventory-and-worklist.md.
## Chapter 1 Doeon right-facing candidate — 2026-11-09

The user directed Package A (player graphics) after recovering existing character art.
This companion generator is separate from the 18-sheet offline-approved packing
pipeline above; it **does not** change its accepted media gates or convert
source-only images into approved all-facing character art.

Use the preserved originals 09-doeon-movement.png and 10-doeon-combat.png:

    python tools/sprite_atlas/build_ch1_doeon_candidate.py --source-root "PATH_TO_PRESERVED_SPRITE_RAW" --output output/ch1-package-A-test

Requires Python 3.12, NumPy, Pillow and OpenCV. The output directory must not
already exist. The generator uses fixed reviewed manual Y edges for movement
[0,284,533,780,1024], source/atlas hashes, a common 64px cell and 18PPU.
It strips chroma and disconnected source specks, saves 48 RGBA frame candidates,
and provides nearest-neighbor 4x checker previews plus an immutable manifest.

The active Unity project contains local right-facing candidate resources in
Assets/Resources/Chapter1Doeon/Candidate-{movement,combat}.png. They are
visual-only presentations driven from completed gameplay evidence.
Left movement/combat is not fabricated or mirrored: the existing individually
authored static left sprite remains the fallback. Original concept art is
never modified. New character art is still pending full visual acceptance,
independent review, directional coverage, manual animation QA and exact
combat effects alignment. This is not a finished character graphics package.

Local evidence (excluded from the public source release):
output/ch1-package-A-20261109/doeon-right-candidate/manifest.json
output/ch1-package-A-20261109/unity-targeted-v2/results.xml
output/ch1-package-A-20261109/visual-frame-{7,9,11}s.png

## Package A follow-up: conservative edge polish and rejected left experiment

Use after building candidate right atlases from the previously authored Doeon sources:

    python tools/sprite_atlas/refine_ch1_doeon_edges.py --candidate-root output/ch1-package-A-20261109/doeon-right-candidate --output output/ch1-package-A-20261109/refined-new
    python -m unittest discover -s tools/sprite_atlas/test -p test_ch1_doeon_edge_refinement.py -v

refine_ch1_doeon_edges.py changes only high-chroma boundary RGB pixels
having an existing nearby opaque non-magenta neighbor. Alpha, dimensions,
positions, status and authorial combat authority never change. The 2026-11-09
candidate replaced 104 movement and 80 combat fringe RGB values. It kept 14
unresolvable candidate border pixels for manual art/VFX cleanup.

prototype_doeon_left_poses.py is deliberately a REJECTED offline experiment:
it derives motion warps only from individually authored static Left.png without
ever mirroring the right costume. It does not output approved left animation.
The visual result was considered too static for live use, and no prototype
asset was bound to Unity. A separately authored left movement/combat sheet is
still required before two-facing animation can be accepted.
## Package A experimental articulated left-facing motion

Run only against the individually authored, existing Left.png source.

    python tools/sprite_atlas/rig_ch1_doeon_left_prototype.py --source "PATH_TO_ACTIVE_UNITY/Assets/Resources/Chapter1Doeon/Left.png" --output output/ch1-package-A-left-experiment
    python -m unittest discover -s tools/sprite_atlas/test -p test_ch1_doeon_left_rig.py -v

The generator makes an EXPLICITLY PROCEDURAL 6x4 movement candidate, with
input SHA and limb-mask analysis. It does not mirror right-facing costume
design and it does not invent hand-authored left combat or hit frames.
Its local evidence was output/ch1-package-A-20261109/left-joint-motion-experiment-v3.

When prototyping in Unity, use resource Chapter1Doeon/Candidate-left-movement
and the source/asset hashes in candidate-animation-profile.json. The runtime
is intentionally visual-only; attack and hurt return the individually authored
static Left.png fallback. The atlas is NOT approved final left animation:
seams and motion transitions still require independent visual/gameplay QA.
## Package A experimental articulated left combat

For an isolated, read-only original Left.png image:

    python tools/sprite_atlas/rig_ch1_doeon_left_combat.py --source "ACTIVE_UNITY/Assets/Resources/Chapter1Doeon/Left.png" --output output/CH1-left-combat-research
    python -m unittest discover -s tools/sprite_atlas/test -p test_ch1_doeon_left_combat.py -v

The original LEFT costume is not mirrored from right assets. The generator
creates an experimental 6x4 RGBA 64px atlas with fixed foot baseline and
per-frame SHA / bbox audit: rows0/1 are attack/hurt candidates and rows2/3
are unbound research poses. Assets/Resources/Chapter1Doeon/Candidate-left-combat
is optional visual-only Unity resource. It does not grant mechanics authority,
damage, original sprite approval, new skills or hand-drawn animation rights.
The original individually authored Left.png remains the fallback if needed.
## Package A conservative 1px left combat cleanup

Use only on a known, source-hash-provenanced left combat **candidate** atlas.
The original independently authored Left.png and prior candidate build remain
unchanged. Requires NumPy, Pillow, OpenCV.

    python tools/sprite_atlas/repair_ch1_doeon_left_combat_pixels.py --input "PATH_TO_384x256_LEFT_COMBAT_CANDIDATE" --output output/CH1-left-seam-cleanup
    python -m unittest discover -s tools/sprite_atlas/test -p test_ch1_doeon_left_combat_pixel_repair.py -v

Repairs only detached 1px components and fully enclosed single pixels in
torso/hip seam area on the first two (bound) animation rows. Unbound research
rows, opaque RGB values, atlas frame layout and gameplay mechanics remain
unaltered. A strict source SHA/output SHA and per-frame coordinate ledger
is emitted in manifest.json. Treat the result as visual-review candidate,
not independent acceptance of authored animation.
## Package A run shoulder-root reattachment

Source-derived left motion atlas row2 is RUN (the fourth row3 is JUMP).
The previously generated art had a 46/48-pixel detached shoulder/arm in
run frames 2/3. Fix only those two 64px frames if their major-component
geometry matches; one pixel per frame joins arm to torso. No blank gaps of
more than one pixel may be auto-filled. Preserved original Left.png is
never modified, and the tool never changes combat, pivot or hitbox logic.

    python tools/sprite_atlas/repair_ch1_doeon_left_run_shoulder.py --input "PATH_TO_PREVIOUS_LEFT_MOVEMENT_CANDIDATE" --output output/NEW_LEFT_RUN_ROOT_REVIEW
    python -m unittest discover -s tools/sprite_atlas/test -p test_ch1_doeon_left_run_shoulder.py -v

Review manifest.json source SHA, exact per-frame pixels and before/after
nearest-neighbor preview. The output remains a PROVISIONAL sprite candidate
until an artist verifies movement quality and user-play screen capture.
## Package A right-side WALK/RUN torso-root stabilization

Original right keyposes have a source-defined 3px WALK / 2px RUN
torso-root variation. The conservative candidate generator shifts
only those 12 tiles by at most 2 integer pixels, preserving exact
colors, alpha counts, 64px frame size, y51 foot height, all left
art and right jump/dash/hit rows.

    python tools/sprite_atlas/stabilize_ch1_doeon_right_torso.py --input "SOURCE_RIGHT_CANDIDATE.png" --output "NEW_OUTPUT_DIR" --expected-sha "SOURCE_SHA256"
    python -m unittest discover -s tools/sprite_atlas/test -p test_ch1_doeon_right_torso.py -v

The result has per-frame before/after anchors and SHA-256 ledger plus
nearest-neighbor side-by-side review. Alignment is not new authored
art and is not independent approval of all gait poses or planted feet.