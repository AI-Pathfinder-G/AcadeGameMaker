# Seryeong v8 bounded review

Date: 2026-09-14  
Reviewer: Luna (independent bounded QA)  
Scope: B1–B4 H1 V4 movement source-reference outputs only.

## B1–B4 H1 result

**Conditional source-only review; no v8 batch acceptance.** All four diagnostics report `source_only`. Independent checks found 4 atlases × 24 cells = **96/96 nonempty cells**, each atlas 768×512, with no alpha bounding box touching a 128×128 cell edge. The recorded scale is approximately 0.273504 for the reviewed 20/254 scalp/sole calibration. Per-costume source hashes are present in the diagnostics; B2 uses the separately preferred v2 single-cross-strap source.

At 4× checker scale, all four garments remain visually readable in the four movement rows, with opaque swimwear coverage and bare feet retained. B4’s gold top has weaker contrast against skin at 64 px; this is a readability limitation for later palette/manual review, not a cell or opacity failure. No obvious outfit loss or clipped limb was found in this bounded sample.

The V4 matte changed-pixel counts are B1 795, B2 819, B3 939, and B4 1,891. These are candidate correction diagnostics and do not establish clean or accepted status. Full v8 review still requires V1–V4, T1–T4, L1–L4, and B1–B4, with H1 movement as the initial diagnostic slice only.

Traceability: `REQ-V8-001`, `REQ-V8-002`, `REQ-V8-003`; `AC-V8-001`, `AC-V8-002`, `AC-V8-003`.

No full matrix, action, facing, runtime, or combination acceptance is claimed.

## All 16 H1 bounded integration review

The independent pass covered V1–V4, T1–T4, L1–L4, and B1–B4 H1 movement outputs after writer stop. For every costume, the diagnostic source path resolves to that costume’s own `raw/movement-source-vN.png` file, the recorded source hash matches the file, and the recorded atlas hash matches the generated `native-atlas-64body-6x4.png`. The observed version selections include V1/V3 v3, T4/L1/L2/L4 v2, B2 v2, and the remaining reviewed sources v1; no prior-costume source binding was detected.

Independent structural checks found **16 atlases, 384/384 nonempty cells, 0 edge-touching bounds**, and 128 row GIF files. Each row GIF has six frames, giving 768 encoded GIF frames across the four-row movement previews. Every diagnostic remains `source_only`; the full accepted-combination count remains zero.

The overview and sampled 4× contacts preserve the long-hair silhouette and readable garments across this H1 slice. The recurring limitation is vivid purple rim pixels whose source-authored plum versus matte spill status is unresolved; outputs are therefore not game-ready clean. B4’s gold top also has weaker 64 px skin contrast. These are bounded palette/readability follow-ups, not cell-integrity failures.

Traceability: `AC-V8-001` through `AC-V8-005` (and `REQ-V8-001` through `REQ-V8-005`). This evidence covers H1 movement only; it does not accept H2–H12, the remaining actions/facings, or the full 432-combination matrix.

## H2 movement bounded review

Independent checks cover V1–V4, T1–T4, L1–L4, and B1–B4 H2 movement outputs. Results are **16 atlases, 384/384 nonempty cells, 0 edge-touching bounds**, 128 row GIFs, and six frames per row; source and atlas hashes match each diagnostic. All remain `source_only`.

Visual samples of V1, T4, L1, and B4 show the H2 hair wave retained without detached fragments or wrong-row selection. T4 and L1 preserve their costume silhouettes and long cloth; B4 preserves opaque swimwear and bare feet. Purple rim pixels remain visible on some hair edges and are still a matte/palette follow-up, not evidence of clean output.

This is H2 normal-movement evidence only. It does not accept combat/village actions, either facing, H3–H12, or the full 432-combination matrix. Traceability: `AC-V8-001`, `AC-V8-002`, `AC-V8-003`, `AC-V8-005`, `AC-APV3-005`, `AC-APV3-006`, and `AC-AP64-001..004`.
