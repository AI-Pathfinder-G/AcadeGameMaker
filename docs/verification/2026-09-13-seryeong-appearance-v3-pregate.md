# Seryeong appearance combinations v3 pre-gate

- Date: 2026-09-13
- Status: **CONDITIONAL / implementation blocked pending Astra approval**
- Contract: `docs/specs/work-contracts/2026-09-13-seryeong-appearance-combinations-v3.md` (Review)
- Scope: read-only contract and source audit; no image, runtime, Unity, or generated-output changes
- Traceability: `REQ-APV3-001..010`; `AC-APV3-001..010`

## Current source facts

The contract’s Cartesian matrix is `36 costumes × 12 hair figures = 432` ordered appearance identities. The first-slice filter is exactly `a1`, `a2`, `c2`, and `d3` × `h1..h12` = 48 combinations. Reference-hair provenance does not add a costume or a thirteenth hair style.

The costume inventory audit reports 36 unique IDs, 36 existing reference files, one default grant, and `ProductionPending=36`. Existing first-slice raw coverage is limited to the four costumes: 17 PNG files including revision history, with movement, combat, and village source sheets. The two hair-v1 PNGs are 1536×1024 source sheets; under the clarified contract they are roots containing twelve labeled figures (`h1..h12`), not twelve independent files. This is source-reference evidence only; no figure extraction, hair mapping, or 48-combination readiness is claimed.

The current source index resolves for the four movement references and all eight first-slice combat/village references. Existing source metadata does not yet constitute reviewed extraction bounds, per-figure hair records, native anchors, or accepted output hashes.

## Bounded gate checks

| Gate | Required diagnostic | Current result |
|---|---|---|
| `AC-APV3-001` inventory | Expand exact 36 costume IDs and 12 labeled hair IDs to 432 unique ordered pairs; first slice exactly 48; preserve reference hair as metadata only | **PASS (static inventory shape)**; source evidence is not an acceptance transition |
| Source-reference substep | Before hair mapping, verify the four existing raw costume sources, source hashes, canvas dimensions, figure/row recipe, and provenance; reject missing/ambiguous roots | **CONDITIONAL**: files and 1536×1024 dimensions resolve; reviewed extraction/figure records are absent |
| `AC-APV3-002` matrix | Every pair has normal, combat, village, Doeon relationship, lovers-shop, and non-lovers absence/staging rows; no silent aliases | **FAIL / pending**: no 432-entry coverage manifest exists |
| `AC-APV3-003/005` facing and anchors | Review both facings per output; reject blind mirroring; validate baseline, pivot, bow/hand, contact and partner anchors | **FAIL / pending**: no native flattened outputs or anchor records |
| `AC-APV3-004/006` composition diagnostics | Conservative masks must detect invalid alpha/background residue, crossed extraction edges, ambiguous occlusion, clipped limbs, outfit loss, and output collisions before publication | **FAIL / pending**: pipeline reports widespread exact-key failures and near-full backgrounds; flood-fill plus conservative classifier masks are required before acceptance |
| Native readability | Review every accepted frame at 1x, including 32 px body target, gutters, integer pixels, palette and silhouette | **FAIL / pending**: no native accepted frames |
| Baseline/airborne safety | Compare sole/floor contact and declared airborne offsets across all contexts; reject floating, penetrating, clipped or drifting limbs | **FAIL / pending**: no per-frame anchor manifest |
| Unsupported mirroring | Permit a mirror only when the complete composed result is proven symmetric for that action and staging | **FAIL closed / pending**: no reviewed symmetry records |
| Simultaneous outfit loss | Compare selected costume identity landmarks and hair figure against each flattened result; reject prior-outfit bleed, naked/base substitution, or mixed garment layers | **FAIL / pending**: no accepted flattened results or identity masks |
| `AC-APV3-007` determinism | Two clean builds from identical reviewed inputs must produce byte-identical manifests, atlases, previews and receipts | **NOT RUN**: no approved build slice or receipts |
| `AC-APV3-009/010` boundary | Diff limited to the four new allowlist roots plus the Astra status line; no runtime/Unity/media overwrite; no NPC/enemy/boss production before 432/432 Luna acceptance | **PASS as contract boundary; no implementation evidence** |

## Required corrections before implementation approval

1. Add the source-reference diagnostic as a mandatory first step for `a1`, `a2`, `c2`, and `d3`, before any H1–H12 mapping. It must bind source hash, root sheet, figure label/region, extraction recipe, and rejection reason without asserting that the 48 first-slice combinations are ready.
2. Implement conservative background diagnostics: flood-fill from declared exterior regions, record suspicious connected components and alpha bounds, and route ambiguous cases to quarantine/manual review. An exact color-key success must not be treated as clean extraction when the background is nearly full-frame.
3. Record per-frame/per-facing baseline, pivot, airborne, hand/bow, contact, partner, occlusion, selected-costume, and selected-hair diagnostics. Any failed frame blocks the combination.
4. Keep composition acceptance on the flattened final result. Layered composition is an allowed production method, not an acceptance shortcut; a valid source-generated flattened frame may pass if all identity and visual checks pass.

No runtime, Unity, image, or generated-output files were edited for this pregate. The contract remains Review and the repository remains `0/432` Accepted.
