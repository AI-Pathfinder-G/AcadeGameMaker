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
