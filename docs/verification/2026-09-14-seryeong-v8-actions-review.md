# Seryeong v8 action source review

Date: 2026-09-14  
Reviewer: Luna (independent bounded source review)  
Scope: V1–V4, T1–T4, L1–L4, B1–B4 H1 raw village sheets; native processing pending.

## Village source-only result

All 16 `village-source-v1.png` files are present at the costume-specific H1 raw paths and are 1536×1024 four-row/six-frame sheets. The reviewed calibration manifest identifies `kind: village`, the exact source filename, per-sheet scalp/sole coordinates, anchor, and source hash. The four rows visually read as forward stretch, folded-arm gaze toward Doeon, writing, and supply gesture. Writing books and supply pouches are visible where authored; no row was relabeled as locomotion.

Costume identities remain distinct in the standing and action contacts. Long skirts and hems remain visibly long through the stretch, writing, and supply rows. Bikini rows retain opaque swimwear and bare feet. L1 and L4 are true RGBA sources and must preserve source alpha during extraction; L1’s alpha bounds reach the source image’s left edge, so native extraction must prove padding and must not hide a source-edge crop.

This is a source diagnostic only. It does not establish native 64 px calibration, matte cleanliness, 24-cell nonempty/bounds integrity, GIF playback, hair combinations, or full action/facing acceptance. The known purple/background edge ambiguity remains a postprocessing review item.

Traceability: `AC-V8-001`, `AC-V8-002`, `AC-V8-003`, and `AC-V8-005`; related presentation rows are governed by `AC-APV3-002`, `AC-APV3-005`, and `AC-APV3-006`.

## Required native gate

Before any village output is considered beyond `source_only`, independently verify source hash binding and row kind, one fixed scalp-to-sole scale per sheet, all 24 cells nonempty and inside 128×128 bounds, integer baseline/root anchors, transparent PNG plus six-frame-per-row GIFs, long-cloth continuity, Doeon-gaze/writing/supply props, preserved alpha for L1/L4, and residual edge/palette review. Any failed row keeps the costume/action set non-Accepted.

## Village native bounded result

The stopped native pass covers all 16 costume/H1 village outputs. Independent checks found **16 atlases, 384/384 nonempty cells, 0 edge-touching bounds**, and 128 row GIF files containing six frames each (768 encoded GIF frames). Every diagnostic source hash matches its referenced raw village source, and every atlas hash matches its diagnostic output hash. The calibration manifest binds each record to `kind: village` and the exact `village-source-v1.png` filename.

L1 and L4 source alpha remains present in the raw inputs (`RGBA`); L1’s source alpha bounds touch x=0 as previously flagged, while the generated native cells have no edge-touching bounds. This demonstrates bounded extraction containment but does not erase the need to review the source-edge padding and transparent edge matte visually. Long skirts, authored props, and village row identities remain source-only review obligations. Purple/background rims remain unresolved palette/matte evidence.

**Visual isolation failure:** native L1 shows detached head fragments below multiple feet and detached boot/sole fragments above neighboring heads across the contact sheet. L4 shows the same class of detached head/sole strips in several rows. These are material frame-isolation failures despite 24/24 nonempty cells and zero cell-edge touches; the likely cause is source-alpha/background bridges allowing disconnected neighboring fragments into a selected component. This is not a purple-rim cosmetic issue.

L1 and L4 must remain failed/source-only until the pipeline removes the cross-frame fragments and an independent contact-sheet review confirms every frame is a single isolated character with its authored props. The regression review must explicitly test component isolation, not only dimensions, alpha occupancy, hashes, or edge bounds.

All native village diagnostics remain `source_only`. This pass does not accept H2–H12, combat, both facings, the remaining action matrix, or any of the 432 appearance combinations. Traceability: `AC-V8-001` through `AC-V8-005`, `AC-APV3-002`, `AC-APV3-005`, `AC-APV3-006`, and `AC-AP64-001..004`.

## L1/L4 alpha-bridge correction review

The earlier L1/L4 native outputs failed visual isolation because detached heads and boots crossed frame boundaries. Those outputs are retained under `village-rejected-alpha-bridge`. The replacement outputs use the V3 alpha-preserving extraction with a 64-core plus 2 px halo and are separate from the rejected files.

Independent recheck of the replacement L1 and L4 contact sheets finds the detached fragments gone across all four village rows. L1 and L4 each retain 24/24 cells, the expected 0.273504 scale, and their source hashes; both remain `source_only`. The source-edge condition for L1 is therefore handled by the replacement extraction, but the old failure remains a required regression fixture.

## V/B combat visual sample

Native V1 H1 and B2 H1 combat contacts were sampled for bow and arrow isolation. The bow remains attached to the character across the four combat rows, arrows appear in the authored draw/hold/release poses without detached neighboring-frame fragments, and B2 retains bare feet and opaque swimwear. This is a visual sample only; the remaining V/B combat outputs and all combat native bounds/playback still require the standard independent gate.

## H1 combat native bounded result

Independent checks cover V1–V4, T1–T4, L1–L4, and B1–B4 H1 combat outputs. Results are **16 atlases, 384/384 nonempty cells, 0 edge-touching bounds**, 128 row GIFs, and six frames per row. Every diagnostic source and atlas hash matches the referenced files; all outputs remain `source_only`.

Visual samples of V1, L2, and B2 contacts confirm the bow stays attached through aim/draw/hold/release/recover rows, authored arrows remain attached to the bow or character when present, and no detached neighboring-frame prop was observed. L2 keeps its long-skirt silhouette during combat poses; B2 retains opaque swimwear and bare feet. This sample does not prove every costume’s prop isolation or either-facing correctness.

Combat remains bounded evidence only. H2–H12, both facings, all required combat states including hit/recover, full native playback review, and the 432-combination matrix remain unaccepted. Traceability: `AC-V8-001` through `AC-V8-005`, `AC-APV3-002`, `AC-APV3-003`, `AC-APV3-005`, `AC-APV3-006`, and `AC-AP64-001..004`.
