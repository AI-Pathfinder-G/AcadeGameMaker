# Seryeong appearance v3 remaining-costume review

Date: 2026-09-14  
Reviewer: Luna (independent bounded QA)  
Scope: D3 V4 source-reference movement slice only (`h1`–`h12`); A2/C2 remain in production.

## D3 V4 bounded result

**Conditional source-only review; no appearance-combination acceptance.** The twelve D3 outputs are marked `source_only` in their diagnostics. This review covers the source-reference movement diagnostic and does not accept the full action/facing matrix or any runtime asset.

The reviewed calibration manifest records the approved D3 source scalp/sole pairs (20/254 for H1–H4, H6, H8, H9, H11; 24/254 for H5, H7, H10, H12), source anchor `(150,254)`, target body height 64, cell 128×128, and baseline 104. The generated V4 diagnostics preserve the corresponding per-hair source hashes and use the expected scale: `64/(254-20)=0.2735042735` or `64/(254-24)=0.2782608696`.

Independent atlas checks found 12 atlases × 24 cells = **288/288 nonempty cells**, all within their 128×128 cells with no bounding box touching a cell edge. This catches the earlier local/global anchor and truncated-cell regressions. The output files and source references are present for H1–H12.

The matte diagnostics report intentional changed-pixel corrections. They do not by themselves indicate failure. Residual checks remain bounded but nonzero: per atlas, opaque near-key pixels are 0–6 and chromatic edge-spill counts are 23–100. These residuals require material/manual disposition before acceptance; this review does not claim residual zero.

At 4× checker scale, H1 remains readable across standing, walk, run, and jump rows, with the long rear hair silhouette retained. H12 remains readable with its tied-up hair identity and expected compact silhouette. H8 and H9 retain distinguishable side/rear strands, but running/jump frames have a possible duplicate rear strand; the corrective v2 files are separate and pending comparison. This is an identity review flag, not an acceptance claim.

Traceability: `AC-APV3-004`, `AC-APV3-005`, `AC-APV3-006`, `AC-APV3-007`, `AC-APV3-008`, `AC-APV3-010`, `AC-AP64-001`, and `AC-AP64-002`.

## Remaining gate

Hold D3 at source-only until the H8/H9 corrective comparison and residual edge review are complete. Full acceptance still requires all required actions and facings for all 36 costumes × 12 hair labels, plus independent native playback and combination checks.

## C2 V4 bounded result

**Conditional source-only review; no appearance-combination acceptance.** C2 H1–H12 V4 movement outputs are present and each diagnostic reports `source_only`. The approved C2 calibration uses the same 64 px body, 128×128 cell, baseline 104, source anchor `(150,254)`, and scalp/sole calibration family used for the reviewed slice. H11’s crown calibration was specifically approved by Astra.

Independent atlas checks found 12 atlases × 24 cells = **288/288 nonempty cells**, with all atlases at 768×512 and no cell bounding box touching an edge. The diagnostics retain per-hair source hashes and expected scales of approximately 0.273504 (20/254 scalp/sole) or 0.278261 (24/254).

The V4 matte changed-pixel counts range from 420 to 1,670 across the twelve C2 atlases. These are candidate matte corrections and are not evidence of acceptance or failure by themselves. Visual 4× checker review of H1 and H12 shows readable C2 clothing and preserved long-hair versus tied-up-hair identities across the four movement rows; no new cropped cell or obvious outfit loss was observed in this bounded sample.

Traceability: `AC-APV3-004`, `AC-APV3-005`, `AC-APV3-006`, `AC-APV3-007`, `AC-APV3-008`, `AC-APV3-010`, `AC-AP64-001`, and `AC-AP64-002`.

Hold C2 at source-only pending the same residual matte disposition, full action/facing coverage, and independent native playback/combination checks.

## D3 H8/H9 revision-2 comparison

The separate `movement-v2` outputs for D3 H8 and H9 are stable and remain `source_only`. Both have 24/24 nonempty cells, 768×512 atlases, no edge-touching cell bounds, and the expected 0.273504 scale from the 20/254 calibration. Changed-pixel counts are 334 (H8) and 294 (H9).

The corrective pass materially reduces the suspected duplicate rear strands in the run/jump rows. At 4× checker scale the silhouettes are cleaner, but the shortened/reduced rear-hair mass needs source identity comparison before selection as the preferred revision. Keep v2 separate from the stable v1/V4 outputs until that comparison is dispositioned. No acceptance is claimed.

## A2 V4 bounded result

**Conditional source-only review; no appearance-combination acceptance.** A2 H1–H12 V4 movement outputs are stable, present, and marked `source_only`. Independent checks found 12 atlases × 24 cells = **288/288 nonempty cells**, all 768×512, with no cell bounding box touching an edge. The calibrated scales are approximately 0.273504 or 0.278261 as expected from the reviewed 20/254 and 24/254 scalp/sole pairs.

The diagnostics retain the per-hair source hashes, including the approved A2 H12 v2 source hash beginning `3b83c230`. Candidate matte changed-pixel counts range from 563 to 1,857; these are correction diagnostics and are not treated as clean or accepted status. At 4× checker scale, H1 preserves the long rear-hair identity and A2 clothing remains readable across the four movement rows. H12 preserves the approved high-bun identity and readable clothing; the previously rejected long-hair source is not present in this reviewed output.

Traceability: `AC-APV3-004`, `AC-APV3-005`, `AC-APV3-006`, `AC-APV3-007`, `AC-APV3-008`, `AC-APV3-010`, `AC-AP64-001`, and `AC-AP64-002`.

Hold A2 at source-only pending residual matte disposition, full action/facing coverage, and independent native playback/combination checks. The full 36-costume × 12-hair matrix remains unaccepted.
