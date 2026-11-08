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