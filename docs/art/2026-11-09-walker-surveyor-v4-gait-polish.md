# Chapter 1 Walker / Surveyor V4 locomotion polish

Status: **implemented and locally unit-verified candidate**; no independent art or Codex acceptance.

## Product change
- Same two original V3 actor designs and same four directional rows / eight existing state keys (32 authored 64px frames).
- Walker stride-A/B now has a wider opposed hoof phase, counterweight displacement, brass compression joints and a vent pulse; all feet remain grounded at y=51.
- Surveyor stride-A/B now articulates opposite tripod support phases, optical body sway and a small compensating reticle motion.
- The change updates only stride-A and stride-B pixels in each row. All 24 idle, telegraph, execute, hurt, dead and recover frames remain pixel-identical to the preceding V3 artwork.
- Runtime already selects stride-A/B from published behavior. No AI, collision, damage, attack timing, input, saves, VFX phase mapping or physics code was changed.
- Generated PNG: `output/ch1-package-B-v3-20261109/gait-polish-v4-source/Candidate-CH1-RegularEnemies-v3.png`.
- Unity resource: `output/chapter1-playable-r1/project/Assets/Resources/Chapter1Enemies/Candidate-RegularEnemies.png`.
- New SHA-256: `bc36c88c4e6f02eddd60d37e4fd8da07f7092532076b808ddf971bf4f20dd796`.
- Updated profile `artIteration=4`, manualArtAccepted=false.

## Checks (2026-11-09)
- Original pixel generator: 6/6 tests pass, including assertion that only 8 stride tiles changed and non-stride tiles match preserved V3 exactly.
- Full Python sprite atlas regression: completed exit 0 (output/ch1-package-B-v3-20261109/gait-polish-v4-unity/python-suite.log).
- Unity PlayMode `Chapter1RegularEnemyArtTests`: **5/5 passed**, `gait-polish-v4-unity/results.xml`.
- Standalone Windows64 build `output/chapter1-autonomous-20261021/build/windows/attempt-039`: **Succeeded**, errors=0, warnings=1.
- These findings do **not** imply independent manual gameplay aesthetic acceptance or new full two-branch natural-input proof.

## Remaining
- Inspect complete 1x moving game for visible foot-skating and fast attack-readiness against the industrial backdrop.
- Separate Walker damaged/HP publication is not present and cannot be faked by VFX or the authored hurt key.
- Independent Codex QA, natural boss completion, and final release acceptance remain outstanding.
