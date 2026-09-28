# Seryeong appearance v3 first-four technical review

- Date: 2026-09-13
- Scope: independent read-only review of first-four source-reference diagnostics
- Contract: `2026-09-13-seryeong-appearance-combinations-v3`
- Disposition: **CONDITIONAL — diagnostic candidates only; no combination accepted**

The generated inventory and coverage manifests pass their bounded structural checks: 36 costumes, 12 hair figures, 432 appearance pairs, 25 action definitions, 21,168 coverage rows, and zero accepted combinations. The non-lovers absence remains declarative coverage and does not produce a sprite.

The first-four source-reference slice has exactly 12 diagnostic records (A1/A2/C2/D3 × movement/combat/village), 24 frames per record, 288 frames total, and zero empty frame bounds. Every record remains `source_only`.

The existing inventory test module passes 6/6 under the bundled Python runtime, including count, duplicate rejection, action-matrix rejection, source-hash validation, and manifest consistency. The full two-clean-build diagnostic test did not complete within the bounded 30-second execution window; deterministic rebuild parity is therefore unverified here.

The native contact sheet visibly shows bright magenta edge fringes around hair, limbs, hems, and footwear. The current diagnostic records report `residualNearKeyOpaquePixels` from 322 to 1,130 across the 12 records. This is evidence of a classifier/cleanup defect or unresolved source contamination, not a clean-alpha result. The narrow classifier must not be reported as residual-zero merely because it removes a large connected background region.

The 32-body native study is useful for checking anchor and silhouette loss, but the comparison-only 64-body study is materially more readable. It does not authorize a 64 px body profile, alter 18 PPU, or support gameplay acceptance. At 32 px, review must explicitly check hair/face/garment landmarks, feet and floor contact, airborne offsets, bow/hand placement, and clipped or detached pixels frame by frame.

Required action before native acceptance:

1. Correct the flood-fill/classifier pipeline until edge-fringe diagnostics are either zero or explicitly quarantined with a reviewed reason; preserve garment pixels that resemble the key colour.
2. Add a neutral checkerboard/contact preview and a per-frame fringe count so magenta contamination is visible at both 1x and 4x.
3. Record baseline, pivot, airborne, bow/hand, and costume/hair identity masks for each frame; any failed frame blocks its action/facing set and therefore the appearance combination.
4. Re-run two clean builds and compare all diagnostic JSON, hashes, native atlases, density studies, and previews byte-for-byte.

## V3 component and 64 px follow-up

The approved offline profile sets a 64 px body target in 128×128 cells with anchor `(64,104)` and retains the 32 px result for comparison. The V3 component proposal now provides 12 first-four source-reference records, 288 frames total, 64 px native atlases, 32 px comparisons, checker previews, GIF previews, component overlays, and source-only diagnostics. All 12 records remain `source_only`; no H mapping or appearance acceptance is implied.

Independent structural checks found no missing selected component, no selected component touching its expanded ROI edge, and no selected component smaller than the largest candidate in any of the 288 frames. Source anchors remain integer-valued. The V3 component diagnostics still report `residualNearKeyOpaquePixels=63..259` and `opaqueEdgeChromaSpillPixels=200..1302`, so alpha/fringe cleanup is improved but not clean. These nonzero values block native acceptance until each residual is either removed or explicitly reviewed as a preserved garment/hair color.

Two isolated V3 rebuilds were run against identical reviewed inputs in separate temporary output roots. Both emitted 492 files and their complete relative-path SHA-256 trees matched with zero differences. This proves bounded tool determinism for the current component proposal only; it does not prove visual correctness or acceptance.

Native 64 px checker review improves face, hair, garment, hand and boot readability compared with the 32 px comparison. It still shows residual magenta pixels around hair and footwear in close inspection. The checker preview is now a required review artifact; a transparent-black preview alone is insufficient for fringe review.

The actionable quality gate is therefore: **determinism PASS; structural component diagnostics PASS as proposals; native alpha/fringe and visual acceptance BLOCKED**. This review does not accept source references, H mappings, native atlases, facings, actions, or any of the 48 first-slice combinations.
