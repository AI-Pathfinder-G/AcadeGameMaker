# Character / monster / NPC / boss art inventory — 2026-11-09

## Package A continuation — facing and edge polish (2026-11-09)

- Independent left-facing master exists at output/chapter1-playable-r1/project/Assets/Resources/Chapter1Doeon/Left.png, 1254x1254 RGBA. Full search of original images found no separately authored left-facing movement/combat keypose atlas.
- A bounded 24-pose **non-mirrored** deformation experiment was made only in local output/ch1-package-A-20261109/left-motion-prototype with tools/sprite_atlas/prototype_doeon_left_poses.py. Visual review showed that legs and shoulders remain substantially a static pose; the experiment is **REJECTED for runtime use**, not imported into Unity and not counted as animated left gameplay coverage. Preserving the correct left master is preferable to false locomotion.
- Right-facing 48 keypose frames were improved via tools/sprite_atlas/refine_ch1_doeon_edges.py: conservative magenta-edge recolor only when an adjacent non-magenta opaque pixel exists. 104 movement and 80 combat edge pixels were recolored, total 184; 14 unresolvable magenta-adjacent pixels retained for review rather than erased. Every alpha value, silhouette position, frame grid, pivot and gameplay state remained unchanged. No source sprite was overwritten.
- Synthetic refiner regression: 4/4 passed; two runs produced byte-identical refined PNGs and manifest. Unity Resources/Candidate-movement.png and Candidate-combat.png now hold the refined candidates, with updated SHA-256 in candidate-animation-profile.json; still not final accepted character media.
- Unity targeted PlayMode tests: 3/3 passed; one test explicitly checks that a rejected Candidate-left resource does not exist and the independently authored left static image remains the exact fallback. Unity Windows candidate attempt-012 compiled successfully (exit 0).
- Source provenance, before/after images, rejected prototype, tests and local receipts remain in output/ch1-package-A-20261109. Codex independent QA is deferred as instructed, and prior successful attempt-011 binaries are untouched.
- Remaining Package A acceptance: author separately viewed left idle/walk/run/jump/dash/attack/hurt animation poses (no blind mirroring); visually inspect sole/hand/weapon anchors, remove remaining effects as separate keyed VFX, verify at 1x/4x motion and both directions under real gameplay; rerun final full branch and release tests after approved final art.

## Package A local implementation — right-facing Doeon animation candidate (2026-11-09)

- **Implemented, not final accepted:** tools/sprite_atlas/build_ch1_doeon_candidate.py performs deterministic processing of preserved original 09-doeon-movement.png / 10-doeon-combat.png into two 384x256 RGBA candidate atlases. Each 6x4 sheet holds 24 keyed poses at 64x64px, PPU 18, common top-left foot baseline 52. Source 09 manually reviewed Y edges [0,284,533,780,1024] avoid the known fixed-grid row-overlap. Source 10 uses exact 256px row edges. Candidate images are NOT original sprite masters or independently approved final media.
- **Unity integration:** current active project includes Assets/Resources/Chapter1Doeon/Candidate-movement.png, Candidate-combat.png, candidate-animation-profile.json. Chapter1DoeonAnimation chooses right-facing idle / walk / run / jump / dash / basic attack / hit frames from actual completed player movement, input attack ordinal and health publications. Renderer and OnGUI use the same visual selection with UV-safe cropped sprites, not a second movement/attack simulation. Source left static Left.png remains exact fallback; there is no blind mirror.
- **Provenance:** originals remain read-only in H:\GPT-workspace\AcadeGameMaker\images\sprites\production-v1\raw. The source/atlas SHA-256, frame extraction geometry and candidate status live in output/ch1-package-A-20261109/doeon-right-candidate/manifest.json. No raw 1536x1024 concept sheet or full Seryeong archive was copied into Unity.
- **Unity test:** 2026-11-09 output/ch1-package-A-20261109/unity-targeted/results.xml reported 2/2 passed, exit 0 (candidate atlas format, point filter, state precedence and clip selection).
- **Actual Windows preview:** Unity 6000.6.0f1 built output/chapter1-autonomous-20261021/build/windows/attempt-009/AcadeGameMakerChapter1.exe, success exit 0. Launched an isolated player from output/ch1-package-A-20261109/game-smoke and captured true gameplay frames at 7s, 9s, 11s in output/ch1-package-A-20261109/visual-frame-{7,9,11}s.png. One R01 test was deliberately bounded; no full branch acceptance claim or independent graphic approval.
- **Final scoped code/test check:** the stricter candidate profile reader and SHA-provenance metadata passed the renewed targeted Unity test (2/2) under output/ch1-package-A-20261109/unity-targeted-v2/results.xml. A second deterministic atlas generation reproduced byte-for-byte identical movement.png, combat.png and manifest.json hashes. The latest attempt-010 Windows build succeeded, and its isolated real Windows branch1 autoplay recorded Extraction ChapterCompleted and final success=True with exit code 0; see output/ch1-package-A-20261109/branch1-full-game-result.json. A separate Solidarity / fail-retry rerun for attempt-010 has not been performed; prior attempt-008 results remain historical.
- **Latest Windows gameplay proof:** the edge-refined candidate build attempt-012 exited code 0 after actual Windows branch1 automated Extraction completion, with ExtractionCompleted=true and resultSuccess=true in output/ch1-package-A-20261109/edge-smoke-result.json. The PlayMode test selection Chapter1DoeonAnimationTests passed 3/3, and Python synthetic refinement tests passed 4/4. Neither test is a Codex independent acceptance or a complete two-facing art approval.
- **Rejected left experiment:** left-motion-prototype/left-movement-PROTOTYPE-4x.png shows mostly static silhouettes and unrealistic gait. The experiment was explicitly NOT imported into Unity; Left.png is preserved, and the Unity test proves it remains the directional fallback.
- **Still pending:** authored LEFT action atlas, clean art edge / magenta halo polish, per-frame foot/weapon/effect alignment on real stage, visual attack/hit timing review, more transition states and 1x/4x animation QA, independent review when Codex becomes available. The generated candidate remains reversible and must not be counted as completed all-direction character media.
## Actual status

- Active Unity project: output/chapter1-playable-r1/project
- Original art archive, READ ONLY: H:\GPT-workspace\AcadeGameMaker\images
- Initial inventory baseline used Doeon and Ordan static left/right sprites; later Package A candidate integration is recorded above.
- Walker/Surveyor currently show temporary actor geometry. No character-specific accepted raster atlas was found in the scanned original images or current Unity Resources.
- Existing production-v1 raw files are magenta-key concept/keypose sheets. They are NOT transparent, completed, synchronized animation clips.
- Seoha approved appearance: boss B6 v7 and NPC B3 v5 (LEFT figure in original paired sheet). Do not silently import discarded styles.
- Seryeong A1/H1 media package reports zero accepted appearance combinations, and other Seryeong source sheets have outstanding apparel/hair/left-direction reviews.
- Each proposed attack/motion must use the authored gameplay state as a visual input. Neither sprite clips nor the visual tool may change movement, damage, collider or attack timing.

## This session's completed source work

1. Restored the source inventory: 18 raw sheets / 384 nonempty SOURCE keypose cells. Six poses per row, eight 3-row sheets and ten 4-row sheets. Source cells are not accepted sprites.
2. Built deterministic non-destructive magenta-source diagnostics in tools/sprite_atlas/inspect_generated_keyposes.py. This detects the original background near RGB(251,3,251), logs source hashes, uses actual 3- or 4-row geometry, measures cut edges, and produces RGBA / nearest-neighbor 1x and 4x source-only previews.
3. Built tools/sprite_atlas/render_stage_previews.py: 14 animated six-keypose GIFs across Doeon idle/combat, Seoha NPC, Ordan combat; plus offline 640x360 logical-stage composite and 4x view. These are NOT Unity captures.
4. All four synthetic image preflight regression tests pass. Originals were never modified and none of these drafts was imported into Unity.

## Source-specific extraction results (all remain SOURCE ONLY)

This table counts nonempty candidate pose cells, border-touch cells on the declared grid and normalization overflows. Zero border touches does NOT mean visual acceptance.

| Planned source | Source cells | Edge-touch cells | Overflows | Immediate task |
|---|---:|---:|---:|---|| 01-doeon-idle | 18 | 0 | 0 | Review silhouette/alpha/left action before Unity binding |
| 02-seryeong-idle | 18 | 2 | 0 | Repair slight border contamination, inspect every frame |
| 03-seoha-npc-idle | 18 | 0 | 0 | Review silhouette/alpha/left action before Unity binding |
| 04-lovers-interactions | 18 | 18 | 0 | Split group actors / manual source boundaries |
| 05-lovers-shop | 18 | 13 | 0 | Split group actors / manual source boundaries |
| 06-ain-yudam-idle | 18 | 18 | 0 | Split group actors / manual source boundaries |
| 07-village-roles | 18 | 15 | 0 | Split group actors / manual source boundaries |
| 08-mujin-hub | 18 | 9 | 0 | Manual source rect/row layout review and clipping repair |
| 09-doeon-movement | 24 | 22 | 0 | Manual source rect/row layout review and clipping repair |
| 10-doeon-combat | 24 | 2 | 0 | Repair slight border contamination, inspect every frame |
| 11-seryeong-movement-combat | 24 | 17 | 0 | Manual source rect/row layout review and clipping repair |
| 12-seoha-boss | 24 | 7 | 0 | Manual source rect/row layout review and clipping repair |
| 13-ordan-combat | 24 | 0 | 0 | Review silhouette/alpha/left action before Unity binding |
| 14-taegan-combat | 24 | 14 | 0 | Manual source rect/row layout review and clipping repair |
| 15-chaeryun-combat | 24 | 2 | 0 | Repair slight border contamination, inspect every frame |
| 16-mujin-combat | 24 | 24 | 0 | Manual source rect/row layout review and clipping repair |
| 17-ragen-combat | 24 | 14 | 0 | Manual source rect/row layout review and clipping repair |
| 18-boss-creatures | 24 | 7 | 5 | Quarantine overflow/rebuild pose |
### Findings for the actual CH1 cast

- Doeon 01-idle: 18 candidate source poses, no cropped-edge touches. Doeon 09-movement: 22 edge-touch cells with the assumed grid, NOT suitable for import. Reviewed Y-edge candidate [0,284,533,780,1024] still clips four cells; more precise manual source correction is mandatory. Doeon 10-combat: 24 source poses, two edge-touch cells, plus LEFT authored source not yet present.
- Ordan 13-combat: 24 source poses and zero edge-touch cells. Most promising first 1CH boss pose preview. Guard-step, DebtLine, SeizureWeight and BalanceAudit each have six key poses. The existing live LEFT render is not replaced; no blind leftward flip of asymmetric armor/device allowed.
- Walker and Surveyor: actual gameplay enemies still need distinct sprites, anticipation/execute/recover, hit/defeat and facing coverage. Do not substitute the future CRX creature prototypes without authoring.
- NPCs: Seryeong, Ain, Yudam, Iseon, Mujin, gravity tailor and vertical courier require individual crop, alpha, expression/idle/interaction and stage placement. Mixed source sheets cannot be treated as one actor. Seoha B3 NPC is separate from B6 boss.
- Later bosses: Seoha B6 v7, Taegan, Chaeryun, Mujin, Ragen, Baekgak/wind-sac mother have raw concept poses; each requires form-specific left/right, timed action visual states and geometry alignment. The creature-boss source currently exceeds preview cell geometry in five cells; do not silently crop these.
- Full combat animations also need separate effects, weapon/hand anchors, sound, attack anticipation and reaction clips. Raw source cells include baked effect fragments requiring separation.

## Prioritized next production / verification

| Priority | Target | Completion criteria |
|---|---|---|
| P0 | Doeon movement/attack | Manual row grid; alpha clean; feet/pivot; right and separately authored left poses; actual movement/attack state drives frames; 1x/4x game QA |
| P0 | Walker / Surveyor | Distinct approved visuals; 2 directions; idle/move/telegraph/execute/hurt/defeat; replaces TemporaryActorView; shot/LOS readability |
| P0 | Ordan CH1 boss | Use reviewed 13 source for proto; author left; four pattern clip groups synced to authored Telegraph/Execute/Recovery, no gameplay authority changes |
| P1 | CH1 NPC portraits/actors | Seryeong apparel/hair validation; Ain/Yudam/Mujin/etc individual pose/face, interaction and dialogue stage placement |
| P1 | Later bosses + 6 creatures | Individually approved actor/color/weapon/phase sprites, no invented skills; cleanup quarantined cells |
| P1 | Pixel VFX / UI | Separate baked weapons/projectiles/effects, point filter, 18PPU logical frame, URP lighting and accessibility QA |
| P2 | Voice/audio and release art QA | BGM/SFX/cinematics and repeatable actual gameplay performance checks |

## Local evidence & limitations

- Correct 18-sheet source manifest: output/ch1-art-audit-20261109/chroma-preflight-v2-all18/manifest.json
- Original vs current game comparison: output/ch1-art-audit-20261109/reference-vs-current-contact-sheet.png
- Animation GIFs and 640x360 stage: output/ch1-art-audit-20261109/animated-previews
- First preflight under chroma-preflight/ had an erroneous four-row assumption for some NPC sheets. It was superseded by chroma-preflight-v2-all18, and must NOT be used for cross-source comparison.
- CH1 Windows build attempt-008 previously passed two complete story branches and one natural failure/retry in auto-play. The separate broader PlayMode regression timed out after 210 seconds without XML. Codex independent QA is deferred per user instruction; this is not a full product/art release approval.
- Do not publish preview contact sheets or raw user assets to the public GitHub repository as accepted gameplay art. Raw images, logs and the large Seryeong source corpus remain in the original archive/local output.

This report is a measured art-work backlog and source-only processing record, not an assertion of gameplay art completion.