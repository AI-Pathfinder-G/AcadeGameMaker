# Chapter 1 — Walker / Surveyor V3 Redesign & Implementation

**Current status:** New original pixel-art V3 has been generated, asset imported into Unity and PlayMode tested. Candidate visual quality, not independently approved. No game mechanics changes.

## Why V2 was replaced

V2 was a conventional armored human melee guard and a teal rifleman. Palette alone was not an adequate signature. V3 uses an abandoned industrial containment system: pressure crushing technology for close-range defense and angular measurement optics for ranged interdiction. These two characters share dark iron, external asymmetrical mechanisms, accent warning lights and industrial-age wear but are meant to be distinguishable at native 64px scale.

## Walker — Pressure Ram Warden

- Silhouette: broad boiler abdomen, tiny cage-head, unequal rear pressure cylinders, prominent exhaust stack, separate hydraulic strut, enormous one-sided wedge ram and shock absorbing hoof legs. It is a pressure unit, not a person wearing armor.
- Palette: soot/rust iron and aged brass with amber/gold overpressure core.
- Idle: ram and boiler sealed. Stride A/B: opposing hoof phases, visible heavy weight transfer.
- Telegraph: the piston retracts and overpressure dial heats up; Execute: pressure ram extends with bounded warning sparks, never a new damage collider.
- Hurt: unstable pressure glow; Death: collapsed boiler and inert piston at ground; Recover: dimmed core and settled ram.
- No gameplay attack, hitbox, enemy AI or collider changes; existing Walker behavior key map stays in control.

## Surveyor — Prism Triangulation Unit

- Silhouette: giant single faceted optic, open triangular measurement halo with 3 instrument vertices, off-axis sensor, narrow instrument mast, tripod-like legs, compact angular measurement projector rather than a generic rifle.
- Palette: near-black industrial blue/steel with cyan and pale mint refraction highlights.
- Idle: closed measuring geometry; Stride A/B: short controlled tripod adjustments.
- Telegraph: measuring halo opens and projector prepares signal; Execute: lens/projector emits small precise cyan marks.
- Hurt/Suppression: misaligned optic with diagnostic red signal; Death: fallen measuring rig and extinguished lens; Recover: lowered projector, reduced optical pulse.
- Visual R/L poses are authored via independent geometric coordinates, not flipped character sprite bitmaps.

## Produced assets and unchanged runtime contract

- Editable authoring source: tools/sprite_atlas/build_ch1_regular_enemy_v3.py
- Python source tests: tools/sprite_atlas/test/test_ch1_regular_enemy_v3.py
- Existing game texture path: output/chapter1-playable-r1/project/Assets/Resources/Chapter1Enemies/Candidate-RegularEnemies.png
- Existing profile path: output/chapter1-playable-r1/project/Assets/Resources/Chapter1Enemies/candidate-enemy-profile.json
- V3 generated sheet: output/ch1-package-A-20261109/regular-enemy-v3-redesign-final/Candidate-CH1-RegularEnemies-v3.png
- 32 unique frames: Walker R/L and Surveyor R/L by eight pose keys (idle, stride A, stride B, telegraph, execute, hurt, dead, recover). 512x256 RGBA atlas, point filter, 18 PPU, normalized pivot (0.5,0.1875).
- Native 1x and nearest-neighbor 4x reviews stored with generated V3 source.
- Existing original V2 raster backup: output/ch1-package-B-v3-20261109/Walker-Surveyor-v2-BEFORE.png
- Actual V2 vs V3 before/after comparisons: output/ch1-package-B-v3-20261109/Walker-Surveyor-v2-v3-comparison-1x.png and comparison-4x.png.
- Runtime interface remains Chapter1RegularEnemyArt, driven solely by the already published behavior. No animation key remapping, collider or hitbox changes, no new input authority, gameplay damage, saves or enemy AI alterations.
- Walker hurt key5 is a produced art key but is not separately bound to a published Walker injury state; do not claim complete hurt/impact integration.

## Quality gates and open work

Offline source tests require all 32 cell hashes unique, no clipped silhouettes, consistent foot anchors, visibly distinct attack readiness/death poses and nontrivial geometry change from V2. Unity PlayMode must confirm existing resource/phase mappings and V3 metadata, plus Doeon regression. Isolated Windows candidate paths must cover Extraction and Solidarity endings and genuine damage/failure -> hub -> retry. Real screenshot review should verify both creatures render at native gameplay scale.

Final manual art approval is NOT granted by passing tests. Further work: more expressive leg cycles and impact frames, attack warning VFX synchronized to enemy actual phases, damage/defeat transitions, stable on-screen native-scale silhouettes, later independent Codex audit and explicit approval before remote push or main integration.