# Costume source review — 2026-09-13

Status: SourceOnly; no native atlas or animation acceptance.
Owner: Astra visual inspection. Independent acceptance remains separate.
Traceability: REQ-COST-001/010/011; AC-COST-001/008 remain pending.

The inventory contains 36 outfits. The first four (A1, A2, C2, D3) have movement, combat and village source sheets: 12 logical sheets, 24 source poses per sheet. Revision files are alternative sources, not additional costumes or completed animations. The remaining 32 outfits have reference metadata, not generated gameplay sheets.

Original movement sheets fail a convincing walk cycle: repeated extended-leg poses lack a sufficient passing phase. Movement-v2 introduces narrower passing poses, but opposite-leg contact and looping still require frame playback. No native-size readability, alpha, palette, anchor, or secondary-motion measurement has passed.

D3 movement-v2 is rejected for garment inconsistency: row 1 retains full-length charcoal trousers, whereas rows 2–4 change into a short lower garment with exposed legs. Preserve the file as revision history; never bind it as an accepted costume. The targeted movement-v3 repair restores full-length trousers in all 24 source poses; it still awaits native gait playback.

A2 movement-v2 preserves the ivory shirt and dark asymmetric lower garment. C2 movement-v2 better preserves the ochre jacket and asymmetric skirt. Both remain pending visual playback and native reduction; neither is accepted solely from this contact-sheet inspection.

Combat and village source sheets provide bow preparation/release, evasive step, hurt recovery, stretch, folded gaze, notebook and supply gestures. Bow-hold poses are visual references, not approval of a new charged-attack mechanic. Source rows do not establish simulation timing, damage or invulnerability.

The requested action-driven clothed secondary motion is specified but has not been verified in playback. Static sheets are insufficient evidence of a visible settling effect. Do not label it implemented or guaranteed until native frames, action timing, coverage and whole-body alignment pass AC-COST-008.

Image postprocessing permission remains pending. No script transparency, crop, scale, palette conversion, pixel displacement, GIF encoding or atlas packing has been performed in this unit. Existing source images are preserved.

