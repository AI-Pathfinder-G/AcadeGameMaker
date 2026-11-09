# CH1 Package B V3 — Pressure Ram / Prism Action Feedback

**Status:** original visual-only candidate implemented and tested in Unity; no artist approval or Codex independent QA yet.

## Source and design

The V3 Walker and Surveyor art from commit 2ff73bb was far more distinguishable than generic V2 humanoid enemies, but the action keyframes lacked external attack/readiness cues. This patch adds original 64px transparent pixel-art overlays without replacing V3 character sprites.

- Walker: pressure vent, overheat/piston warning (published DashTelegraph); warm compressed pressure burst from the massive ram (published DashActive); low steam and ember residual (published Recovery).
- Surveyor: directional triangulation rays and optic convergence (published FireTelegraph); cold refracted flash next to the optical projector (published Fire); discontinuous diagnostic optical signals (published Suppressed), fading reticle (published Cooldown). No extra invented projectile/hit/damage claim.
- Left/right sources are authored in separate coordinates, not mirrored finished images.
- Idle, normal movement, dead, and Walker hurt are TRANSPARENT. Walker has no separate currently published hurt event and thus must not fabricate a hit reaction. Surveyor Suppressed is optical instability rather than proof the player was damaged.

## Files and unchanged gameplay contract

- New editable VFX generator: tools/sprite_atlas/build_ch1_regular_enemy_v3_effects.py
- Generator regression: tools/sprite_atlas/test/test_ch1_regular_enemy_v3_effects.py
- Generated 512x256 transparent RGBA atlas, 4 rows x 8 fixed keys, 64px each, 18 PPU, same normalized pivot (.5,.1875): output/ch1-package-B-v3-20261109/v3-action-vfx/Candidate-CH1-RegularEnemyVfx.png
- Unity VFX texture at output/chapter1-playable-r1/project/Assets/Resources/Chapter1Enemies/Candidate-RegularEnemyVfx.png
- Exact SHA and permission/status profile at Assets/Resources/Chapter1Enemies/candidate-enemy-vfx-profile.json.
- New behavior-only visual selector at output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Runtime/Run/Unity/Chapter1RegularEnemyVfx.cs. It accepts nullable already published CarriedBehaviorView and returns only the corresponding original effect sprite. No simulation is advanced or modified.
- Modified Chapter1RoomInstance creates a child effect renderer on each existing regular enemy sprite, preserving original collider position, foot pivot and higher sorting layer. It also fixes existing erroneous Tiled draw mode on normal enemy sprites; industrial background textures remain Tiled.
- New Unity PlayMode phase-safety tests at Assets/AcadeGameMaker/Tests/PlayMode/RunUnity/Chapter1RegularEnemyVfxTests.cs.
- No attack, AI, behavior phase, damage, cooldown, target hitbox, move, player input, loot, or save source changed.

## Review and acceptance

The original generator emits 14 active effect sprites out of 32 keys and rejects clipped pixels. Compare the raw 4x8 preview and source-character-composited preview at output/ch1-package-B-v3-20261109/v3-action-fx-on-character-preview-4x.png.

Offline behavior/pixel tests 4/4 passed. Unity VFX focused 3/3, existing enemy art 5/5 and Doeon regression 13/13 passed. Candidate Windows build attempt-037 succeeded exit code 0. Actual isolated Windows candidate attempt-037 passed three routes with resultSuccess=True, exitCode=0 and owned input cleanup: Extraction, Solidarity, and real CommittedFailure -> ReturnedToHub -> new-run retry. Native captured R01 gameplay contains 70 frames and shows both pressure and prism visual cues (including r01-032.jpg and r01-055.jpg). Source-art hashes, frame authorization, Unity suites and route logs cross-checked in output/ch1-package-B-v3-20261109/v3-action-vfx-full-provenance-audit.json. These are automated functional and static visual evidence, NOT final manual 1x art approval.

**Open:** manual 1x gameplay aesthetic judgement, stronger stride intermediate keyframes, broader attack-readiness and VFX sound design, Walker actual hurt published phase (absent), independent Codex audit after allowance recovery. No push to remote/main or final release approval.