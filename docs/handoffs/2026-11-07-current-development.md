## 2026-11-09 CH1 V3 Walker/Surveyor phase-driven pressure/prism FX — CURRENT

User requested next implementation after Walker/Surveyor V3 original art.
Active development branch fix/ch1-unity-resume-20261108, workspace
H:\GPT-workspace\AcadeGameMaker-integration-20261107, prior head 2ff73bb.
New original 4x8x64px transparent RGBA effect generator:
tools/sprite_atlas/build_ch1_regular_enemy_v3_effects.py
and tests test_ch1_regular_enemy_v3_effects.py. 14 active frames
for published Walker DashTelegraph/DashActive/Recovery and Surveyor
FireTelegraph/Fire/Suppressed/Cooldown (R/L); all 18 idle/move/death/
unpublished Walker hurt frames blank. All original V3 sprites untouched.
Unity imported Candidate-RegularEnemyVfx.png and separate profile in
Assets/Resources/Chapter1Enemies. New Run.Unity visual-only
Chapter1RegularEnemyVfx.cs consumes completed CarriedBehaviorView, NO
simulation write, new hitbox, player damage, game saves or input.
Chapter1RoomInstance uses child VFX SpriteRenderer at original
foot pivot and corrects normal enemy drawMode=Tiled to Simple.
Art generator tests 4/4, Unity FX tests 3/3, existing enemy 5/5 and
Doeon 13/13 passed. Windows candidate attempt-037 build exit 0.
Actual isolated Windows attempt-037 game paths Extraction, Solidarity,
real damage -> failure -> hub -> retry all succeeded exit0 and cleanup.
70 native R01 screenshot frames show pressure and prism action cues.
Full source and route SHA proof: output/ch1-package-B-v3-20261109/
v3-action-vfx-full-provenance-audit.json. This is not final art QA.
Design and remaining issues documented at
docs/art/2026-11-09-walker-surveyor-v3-attack-feedback.md.
No main merge, remote push, release or Codex QA. Manual art not approved.
## 2026-11-09 CH1 Walker and Surveyor V3 redesign — CURRENT

Active worktree H:\GPT-workspace\AcadeGameMaker-integration-20261107
on branch fix/ch1-unity-resume-20261108. User rejected Walker/Surveyor
v2 visuals as generic, approved pressure ram + prism triangulation
machine direction and explicitly ordered actual development.
New editable original sprite generator: tools/sprite_atlas/
build_ch1_regular_enemy_v3.py. No source image mirroring, no reuse of
Doeon/Ordan visuals. Walker v3 boiler/cage/piston hoof guard; Surveyor
v3 monocular optic/triangulation halo/tripod projector. Four rows R/L
per enemy, eight keys, all 32 unique, native 1x plus 4x reviews.
V2 original backup and v3 source, before/after visual comparison and
integrity data at output/ch1-package-B-v3-20261109.
Active resource Candidate-RegularEnemies.png retains name and size,
profile artIteration=3, output SHA ceb05ecd519591f4b1781bfd435865b39d08e4bb14dfaf7101a07f6aecab766f
and no final manual art approval. Runtime Chapter1RegularEnemyArt and
published Combat phases UNCHANGED; no AI, hitbox, input, save edits.
Source test v3 5/5, Unity monster 5/5, Doeon regression 13/13 pass.
Windows candidate attempt-035 built exit0. Actual isolated auto runs:
Extraction, Solidarity, real damage -> failure -> hub -> retry all pass
success=True exit0 + owned input cleanup. R01 actual screenshots prove
V3 actors visible in the game. Full raw SHA audit at
output/ch1-package-B-v3-20261109/v3-three-route-integrity-audit.json.
Design detail: docs/art/2026-11-09-walker-surveyor-v3-design.md.
Still pending: native manual 1x quality acceptance, attack warning VFX,
Walker distinct hurt publish/animation binding, richer walk, Codex QA.
No public remote push, main merge or source-art overwrite permitted.
## 2026-11-09 CH1 original regular enemy actor art — CURRENT

On fix/ch1-unity-resume-20261108 (H:\GPT-workspace\AcadeGameMaker-integration-20261107)
user requested continuing graphics after Doeon recovery commit 612165d.
NEW original Walker/Surveyor sprite art generated from editable
tools/sprite_atlas/build_ch1_regular_enemy_character_atlas.py:
4 rows (two enemies x two independent facings), eight pose keys,
32 distinct 64px art frames. Walker armored melee vs Surveyor cyan ranger.
V1 and v2 assets/source SHA evidence preserved locally in
output/ch1-package-A-20261109/regular-enemy-authored-v2.
Run.Unity now reads already completed immutable behavior publication
from Combat.Unity, selects real idle/walk/telegraph/execute/hurt/dead/
recover candidate sprites and uses existing enemy colliders as visual
foot anchors. No new gameplay attack/AI/collision/LOS/save authority.
Combat assembly producer friend visibility is enabled for player.
Offline source tests 4/4, final Unity tests 4/4, existing Doeon 13/13, player build attempt-033
success exit0, isolated Extraction completion success exit0. R01 native
game screenshot shows both new actors. All three Windows clean-room gameplay routes success=True exit0 and cleanup
confirmed in output/ch1-package-A-20261109/regular-enemy-authored-v2-full-audit.json.
Native R01 gameplay shows both enemies. This is candidate integration,
not manual final artwork acceptance or independent QA.
Visual art acceptance and Codex independent review remain pending.
No remote push or main merge; original separate game repository untouched.
## 2026-11-09 CH1 Doeon left hurt visual recovery snap correction — CURRENT

Active local dev worktree H:\GPT-workspace\AcadeGameMaker-integration-20261107,
branch fix/ch1-unity-resume-20261108, prior head 3959a9d.
After examining candidate left attack/hurt sprite source poses, discovered
left hurt animation had a redundant frame5 -> frame0 transition at ages
9-11. Frame5 is an exact source match to independently authored idle key1;
frame0 produces a gratuitous 78-pixel RGBA jump. Changed ONLY the
Chapter1DoeonAnimation.SelectHitFrame visual selector to hold frame5
through age11, with idle resumed at age12. No PNG or original source
modified, left attack/right hurt sequences unchanged, no damage/hitbox/
movement/input/save changes. Visual metrics and old/new contact sheet:
output/ch1-package-A-20261109/left-action-naturalness-stage.
Offline source-backed pixel regression 3/3 passed. Unity focused PlayMode
13/13 passed. Candidate Windows attempt-030 built exit0. Actual Windows attempt-030 checks for failure -> saved hub -> new-run retry,
Extraction and Solidarity all succeeded with exit0 and cleanup. See
output/ch1-package-A-20261109/left-action-naturalness-stage/
left-recoil-3path-audit.json for cryptographic proof and source matching.
Codex independent review and manual graphic acceptance are deferred;
no remote Git push or main merge. Original masters and previous RC
builds remain untouched. Next: left glove/gear authored art followed
by first distinctive Walker/Surveyor monster asset production.
## 2026-11-09 Doeon right swing wrist contact — CURRENT

User directed next ArcadeGameMaker Package A graphics improvement.
Active dev worktree H:\GPT-workspace\AcadeGameMaker-integration-20261107,
branch fix/ch1-unity-resume-20261108; prior local head eeaae8d.
Found the right swing trail first active key 4.39px from the right fist,
versus left 1px. New deterministic VFX-only tool translates the four
right swing keys -3px, reducing initial contact gap to approx 2px.
Original V1 source and all left/hurt VFX rows, character art, combat
physics, save/input ownership are preserved. Active PNG and SHA-bound
Unity candidate animation profile changed, signed shift is numeric.
Generator tests 4/4. After first JSON metadata test failure corrected,
Unity focused tests 12/12 passed. Windows attempt-028 built exit0.
Real Windows automated paths: Extraction/Solidarity both completion
success=True exit0; real failure -> hub -> new run retry also success=True
exit0; cleanup verified. Exact proof and 57 native screenshots in
output/ch1-package-A-20261109. R06 left/right attack effects were
visually identified in native boss-011 and boss-024 screenshots.
This proves actual rendering, not every moving phase/art acceptance.
Original archived art and previous RC untouched. Codex independent QA
and hand-retouch approval remain pending; no remote push/main merge.
## 2026-11-09 CH1 Doeon right swing to hand contact — CURRENT

Active dev branch fix/ch1-unity-resume-20261108 at
H:\GPT-workspace\AcadeGameMaker-integration-20261107.
User asked to continue Doeon Package A graphics. The prior right
swing trail was 4.39px from the active right fist on strike frame3.
A strict visual-only per-frame -3px X translation of four right swing
effect keys brings it to ~2px. Left swing and left/right recoil effect
frames, actual character art, game mechanics and all other atlas keys
were preserved. V1 source and V2 refined SHA manifest stored under
output/ch1-package-A-20261109/doeon-action-feedback-hand-contact-v2.
Unity profile SHA and numeric right contact metadata refreshed.
Offline negative/fidelity tests 4/4 passed. Focused Unity 12/12 pass
after fixing JSON metadata shift -3 that PowerShell initially emitted
as a string. Windows candidate build and gameplay receipts to be
confirmed separately before claiming whole Chapter1 acceptance.
Manual moving-art QA and Codex independent review still pending.
Original H:\GPT-workspace\AcadeGameMaker and prior RC not modified.
Do not push remote, merge main or treat source art as approved.
## 2026-11-09 CH1 Doeon action feedback visual overlay — CURRENT

Following local dev commit fb61f6c, user directed continuing the Package A
visual art polish on active branch fix/ch1-unity-resume-20261108,
H:\GPT-workspace\AcadeGameMaker-integration-20261107.
A new visual-only pixel-art 4x6 RGBA sprite atlas contains left/right
swing trails and actual-health-loss recoil sparks. Completed FiredHit AND
FiredMiss produce a short four-tick directional SWING; only confirmed HP
loss produces a five-tick damage spark. Both overlays share the Doeon
64px/18PPU foot pivot. The effects are selected by Chapter1DoeonAnimation
and drawn via world room SpriteRenderer plus OnGUI. No movement, attack
hitboxes, combat timing, save or input ownership was modified.
The source generator/tests, Unity PNG/meta, profile and render code were
changed locally. Offline tests 4/4, Unity focused tests 11/11 and
Windows attempt-026 build exit0 passed. Extraction and Solidarity actual
automated Windows paths passed exit0. Actual Windows failure -> hub -> new run retry also passed exit0
with real enemy damage. All three candidate traces are verified in
output/ch1-package-A-20261109/doeon-action-feedback-three-path-audit.json. Original H:\GPT-workspace\AcadeGameMaker
and old release RC assets are unchanged. Codex independent review and manual
art acceptance remain pending; do not push to remote or merge main.
## 2026-11-09 CH1 Doeon right WALK/RUN root alignment — CURRENT

Active dev branch fix/ch1-unity-resume-20261108 on
H:\GPT-workspace\AcadeGameMaker-integration-20261107.
User continued Package A (Doeon character art). Source right WALK/RUN
torso root varies 3px/2px, while all feet end at y51. New visual-only
generator tools/sprite_atlas/stabilize_ch1_doeon_right_torso.py uses a
reversible <=2px integer X shift for right WALK row0 / RUN row1.
Eight of twelve frames shifted; right jump/dash/hit and all left art
remain unchanged. Unity Resources/Candidate-movement.png and profile
SHA were updated. Review evidence at output/ch1-package-A-20261109/
right-torso-alignment-v1. Offline tests 3/3, Unity tests 10/10 pass,
and candidate attempt-024 Windows build succeeded (exit0).
All three isolated attempt-024 Windows game-path checks completed success=True exit0 (Extraction, Solidarity, real failure/hub/retry); confirmed by output/ch1-package-A-20261109/right-torso-all-paths-audit.json. This is automated functional evidence, not manual art acceptance.
Original H:\GPT-workspace\AcadeGameMaker is read-only and preserved.
Manual visual quality and Codex independent QA deferred, no remote push.
## 2026-11-09 CH1 position-derived visual gait cadence — CURRENT

User requested next art task after 1e79bcc4, dev worktree
H:\GPT-workspace\AcadeGameMaker-integration-20261107
and branch fix/ch1-unity-resume-20261108. The published player
MotionSnapshot.PositionXQ4096 is now read-only exposed to Doeon visuals.
A six-key stride phase accumulates actual pixel displacement, not fixed
simulation tick intervals; grounded feet freeze at zero movement and do
not advance through teleport or duplicate renderer sampling. Walk has
5.5 sprite-pixels per step and run/dash 9, PPU 18; source and right/left
facing atlas content unchanged. Movement/attack/hitbox/input/save code
is unchanged apart from exposing read-only published position.
Focused Unity art tests 9/9 pass and Windows candidate attempt-022 built.
Extraction branch1 success=True exit0 in isolated build. Actual Windows Solidarity branch2 and failure->hub->retry tests completed
success=True exit0, all along with Extraction. Cross-path proof:
output/ch1-package-A-20261109/physical-gait-all-paths-audit.json.
These are not Codex independent QA or final manual foot-slip acceptance. Original archive
H:\GPT-workspace\AcadeGameMaker, previous packages, remote main and
Codex independent QA remain untouched/deferred. Manual art acceptance
not achieved, especially right RUN torso source wobble and left hand poses.
## 2026-11-09 CH1 Doeon left RUN shoulder bridge — CURRENT

The user requested continuing package A. The last accepted local baseline
was 9013f41e on fix/ch1-unity-resume-20261108, with all other repos untouched.
Image component audit found a left arm detached by one 64px pixel in RUN row2,
columns 2 and 3 (not jump row3). A strict 2-pixel bridge script and 4 offline
regression cases were added; one pixel at x28 y21 in each affected frame.
Original Left.png master, right-facing art, input/combat/physics/save logic
and prior release ZIP were not modified. Verified connected-components and
exact original-art palette retention under output/ch1-package-A-20261109.
Revised Candidate-left-movement.png and SHA-bound candidate-animation-profile
are in active Unity Resources. A targeted Unity 8/8 test passed, and Windows
candidate attempt-020 built exit 0. Isolated Extraction autoplay completed success=True exit=0 on attempt-020;
receipt left-run-joint-game-branch1-result.json confirms pass; no manual art approval.
No main/remote push. Final authored left art & independent Codex review pending.
## 2026-11-09 CH1 Doeon left combat pixel cleanup — CURRENT

The user asked to continue Package A. Worktree:
H:\GPT-workspace\AcadeGameMaker-integration-20261107
Branch: fix/ch1-unity-resume-20261108. Original preserved source:
H:\GPT-workspace\AcadeGameMaker (read-only).

Added deterministic procedural pixel cleanup tool and synthetic tests.
On candidate LEFT combat frames only (6 attack + 6 hit), removed 10 detached
single-pixel artifacts and filled 10 enclosed 1px torso/hip seam gaps.
Original graphic source and unbound research rows 2/3 remain unchanged.
The refined atlas candidate was copied to Unity Resources/Chapter1Doeon
and the profile SHA-256 refreshed. Right sprite, combat/physics/input/save
and previous RC zip untouched. Pixel tests 4/4, reproduction 3/3 hashes,
Unity focused tests 7/7 passed. This does not replace separately authored
final left animation; manual visual and Codex independent QA remain pending.
Windows build attempt-019 succeeded, and isolated Extraction branch1 autoplay completed (success=True, exit code 0). Receipts: output/ch1-package-A-20261109/build-left-seam-cleanup-result.json and left-seam-branch1-smoke-result.json. Full independent/manual visual QA is still pending.
## 2026-11-09 CH1 Doeon actual-fired combat presentation — ACTIVE

Continued Package A on dev worktree. InputRouter now distinguishes completed
Combat BasicAttackAttempt.Fired (FiredHit/FiredMiss) from merely issued presses
which may be rejected for cooldown/aim. Animations start on the completed fire
tick, while health-drop reactions use completed health publication. Both actual
left and right candidate sprite rows remain 64x64; right hit sequence reverted
to the authored 4/5 frames; all attack/hurt display stages settle within 12
ticks. SpriteRenderer and GUI gain short, renderer-only attack/hurt color flash.
No attack damage/collider/timing is changed. Unity focused PlayMode tests 7/7
passed, Windows candidate attempt-017 built exit0, and actual Windows branch1
Extraction ended success=true exit0. Data at output/ch1-package-A-20261109.
Actual Windows Solidarity branch2 succeeded (exit0, success=True). The same build also completed failure -> hub/save -> retry with success=True, exit0. Corresponding branch2 and failure-retry receipts are under output/ch1-package-A-20261109. All are bounded automated game-path evidence, not independent QA or final visual acceptance.
Manual visual QA and Codex independent review still deferred; no main merge.
The preserved original H:\GPT-workspace\AcadeGameMaker was not modified.
## 2026-11-09 Package A left combat — local provisional implementation

Still on fix/ch1-unity-resume-20261108. User instructed continuing art
development. Added separate source-derived left combat frame atlas from
original Left.png with NO mirrored right art. The 384x256 sheet has 24
jointed pixels candidates, of which rows0/1 (6 attack, 6 hit) are bound to
visual-only completed gameplay state; rows2/3 are research poses only.
The candidate is not final authored media. Existing left static art is kept
as fallback; no movement/combat physics, input ownership or save changes.

Synthetic tests 4/4 and Unity focused tests 5/5 passed. Unity Windows
candidate attempt-015 succeeded. The isolated real Windows branch1 Extraction test completed with success=True and exit code 0. Evidence: output/ch1-package-A-20261109/left-combat-game-result.json and left-combat-frame-*.png.
Original H:\GPT-workspace\AcadeGameMaker remains untouched. Independent
Codex QA deferred; main and public remote are unchanged. Do not misstate
art-direction approval or complete both-facing release acceptance.
## 2026-11-09 Package A left motion — CURRENT

Active Git branch fix/ch1-unity-resume-20261108. Continued package A after
right candidate commit 78b11d6: independently left-authored Left.png has been
used to build a new 24-frame PROVISIONAL articulated movement atlas (not mirrored
right sprites), imported to Chapter1Doeon/Candidate-left-movement. Rendering
source Chapter1DoeonAnimation uses completed motion state and returns the true
left static master for missing left attack/hit poses. No mechanics changed.
Focused Unity animation tests 4/4 passed after compiler fix. Windows candidate
attempt-013 compiled exit 0; isolated Extraction branch1 autoplay completed
with resultSuccess=true, exit 0. Raw proof and gameplay stills:
output/ch1-package-A-20261109. Do not say all facing animations are accepted:
manual left gait realism and left combat/hurt sheets remain missing. Codex
independent review deferred, no main merge or source art publication. Original
H:\GPT-workspace\AcadeGameMaker remains read-only. Later standalone work
must preserve the previous build attempt-012 and current quarantined outputs.
## 2026-11-08 chat session restoration verification (user paused audit gates)
The current GPT-6 chat added a test-only Unity selected case, ChatSessionRecoverySmokeTests.cs under the active Unity project Tests/PlayMode/InputUnity. It does not alter the protected original owner fixture, nor claim a Luna approval. The new synthetic test lease is confined to test assembly, batchmode and temporary input devices. No natural boss test was invoked.
- Unity 6000.6.0f1, test AcadeGameMaker.Tests.PlayMode.InputUnity.ChatSessionRecoverySmokeTests.RestoresAfterGeneralBudgetExhaustion, XML Passed, total=1 passed=1 failed=0 skipped=0; process exit 0.
- Final stronger run evidence: output/ch1-standalone-owner-platform-20261104/attempts/chat-session-restore-devices-20261108/ (local raw logs, excluded from publication). Test-owned keyboard+mouse: 2 devices, 161 controls. The actual OwnedBatchInputIsolation logged restored with valueEquivalentRestored=true,currentRestored=true; probe-restored with original dynamic ticks=2 and clockReadRejected=true.
- Shared 64 KiB budget: general evidence saturated after 37 added entries; two production cleanup records used the reserved rows, a third terminal AddCleanup call succeeded and a fourth was rejected. Temporary devices were removed; original device count restored. No OS security prompts, user saved game data or persistent input settings files were altered.
- Earlier zero-device baseline smoke in attempts/chat-session-restore-20261108/ also passed 1/1 but the stronger device-backed run supersedes it for this local conclusion. These tests do NOT verify the protected owner-manifest/gate path, natural boss completion, same-build distribution, or Codex independent review.
- The protected original natural boss tests remain 2/2 failed; latest draft PR #13 must not be represented as integrated/independently accepted. Await an available Codex independent verification pass at a later session per user request, without holding up unrelated local implementation.
### 현재 관측과 원인 보정 — 2026-11-08

첫 실제 Unity 컴파일과 정확 EditMode28/PlayMode8/입력 복구2는 통과했다. 자연 보스1은 정상 원정 피해로 HP1에 진입하고 DebtLine을 지난 뒤 tick2017 SeizureWeight 예고 age13에서 Pilot의 후보 증명이 거절되어 실패했다. 생산게임 결함이나 전투 완주로 표시하지 않는다. 지상 중립의 2D certificate 시도는 전체 몸체 포락선 문제로 독립 미수용되어 코드에서 제거했으며 기존 일반 후보 거절을 유지한다.

현재 보정은 승인된 소유 컨텍스트에서만 수행하는 단일 Recovery 탐색 입력이다. 실제 Stage1 DebtLineA Recovery의 x3.0~3.25/오른쪽 속도7.5~8.001/지상 Space 해제 확인 뒤 D+Space 한 edge를 발행한다. 실제 동일 owner·소비 sequence·완료tick·focus·phase 연속성과 상승·몸체/경기장 관측을 검사하고, 보스 우측 도달 시 중립으로 해제한 뒤 기존 선택자로 돌아간다. 접촉/예약피해/당김/벽/가속/신원 손실/누락된 edge/60tick 기한/잘못된 phase는 실패 폐쇄한다. 미래 무피해·충돌 안전의 증명이나 HP/좌표/시간 조작이 아니다. 새 NUnit2를 포함한 현재 정확30은 실제 Unity 통과했다. 앞선 신규 NUnit assertion의 컴파일 실패1회와 미수용 후보 결과는 원문으로 보존하며 현재 성공에 합산하지 않는다.

자연 실행은 누적2/최대2를 소진했다. 두번째 현재 소스 관측에서는 tick1980에 Recovery 탐색을 시작하고, tick2019 실제 x8.260986/y3.512939에서 Space를 해제했다. tick2025 정상 기본공격 피해3이 적용되어 보스 HP60→57, 플레이어 HP1을 유지한 채 Seizure Execute tick2064/2100 NoContact와 tick2160 Recovery까지 진행했다. 이는 실제 이동·정상 공격·해당 공격 생존의 부분 관측이다. 전투 완주는 실패이며 자연 추가 실행0·현재 Windows 빌드0·main 병합0이다.

두번째 실패는 기존 shared 기록 예산 소진과 teardown 증거 기록 실패다. `Chapter1VerificationBossBudget.Add`가 `OwnedBatchInputIsolation.Log(restored)`에서 예외를 냈고 confirm/case-witness가 실패했다. 실제 설정·장치·current·control 비교는 restored 기록보다 먼저 실행되므로 직접 물리 복원 불일치가 입증된 것은 아니다. 그럼에도 복귀 확인과 완주 수용은 불합격으로 보존한다. 로그의 성공 기록87행과 JSON 상계를 재구성하면 약130,585 bytes지만 런타임 counter를 기록하지 않아 정확 소진 counter로 단정하지 않는다. 이번 diagnostic-natural caller는131,072 bytes, recovery smoke caller는65,536 bytes이며 각 기존 값을 유지한다.

기록 도구의 최소 보정은 같은 shared 예산 안에 restored/probe-restored/teardown 필수3행의 최대24,576 bytes와3행을 예약한다. 전체131,072/65,536 bytes,256행,개별8192 bytes 상한은 그대로다. 일반 기록은 나머지만 소비하고 소진 시 여전히 예외로 실패하며, 예외·원시·실패 결과를 지우거나 성공으로 바꾸지 않는다. 초과 예외에는 값 대신 counter만 추가한다. 전략·공격·생산 HP/좌표/시간을 바꾸지 않는다. 예약만으로 보스 완주를 입증하지 않으며 수정 후 자연 재실행은 하지 않았다.

| 실제 Unity 검증 | 결과와 소스 범위 |
| --- | --- |
| 초기 소유 정책28 / Driver8 / 복구2 | 정확28/28,8/8,2/2 통과. 복구2는 보정 전 격리 fixture 범위이며 standalone 양성 권한 증거가 아니다. |
| Recovery 탐색 소스 집중30 | 30/30 통과. 새2는 상태 전이 반례이며 물리 증명을 대신하지 않는다. |
| 기존 최소 Edit10 + Play11 | 10/10 + 원정7/7 + 양분기 persistence4/4 통과. 최초 Play 필터는 실제 namespace 차이로4가 누락되어7만 실행했고, 누락4만 단회 보완했다. 기존7은 반복하지 않았다. 이21은 기록 예약 보정 전 소스 결과다. |
| 최종 기록 예약 소스 집중4 | 신규 예산 경계 + 기존 피해/예산 + 탐색2의 정확4/4 통과. 64/128 KiB 각각 일반 예산 정확 소진 후 필수3행 기록 가능, 개별8192/전체 bytes/예약행 초과는 계속 거절한다. 현재 프로젝트 전체 컴파일은 성공했고 전체 테스트를 다시 실행하지 않았다. |
| 자연 보스2 / 릴리스 | 누적2/2 실패 보존. 후반 이동·공격·생존 부분 관측만 수용하며 완주/복원 confirm 실패가 남아 릴리스 빌드와 배포본 smoke는 실행하지 않는다. |

최종 변경은 안전한 별도 검토 브랜치에 보존한다. 필수 natural gate가 실패한 새 변경을 main에 병합하지 않는다. 원본 보존본 HEAD와9 dirty 파일은 의도된 원본 AGENTS 안내 prefix를 제외하고 유지한다. 다음 제품 단계는 기록 예약의 실제 복구 재검증과 기존 유한 실행 한도에 대한 별도 통합 판정이며, 새 evidence 이름으로 자연 횟수를 초기화하지 않는다. 기존 실패·소진 원문과 source별 검증 범위를 유지한다.

## 최신 사용자 지시 — 2026-11-08 Unity 개발 재개

사용자가 “유니티 프로그램 호출해서 개발 시작해”라고 명시하여 11월7일 Unity 실행 보류를 해제했다. 현재 활성 worktree의 output/chapter1-playable-r1/project에서 실제 Unity 컴파일·집중 검사·필요한 구현을 재개한다. 이전 실패 원문과 원본 보존본은 유지하고 다른 프로젝트 프로세스·파일은 변경하지 않는다.

### 2026-11-08 실제 실행 작업판

Sol 단일 실행 소유. 현재 원격/로컬 부모 814ac33b5887a58ccc4ae711884e7eb492a45082에서 시작한다. 사용자 재개 지시에 따라 기존 소유/저장 범위의 EditMode28(기존24+새4) → PlayMode Driver8 → EditMode 복구2 → 자연 보스1을 순차 실행한다. 각 후속 시험은600초 단회이며 같은 원인 두 번이면 재실행을 멈춘다. 최초 import/compile 관측은15분이며 새 진행이 있을 때 이유를 보고한다. 첫28/8은 실제 Unity PASS이며 후속 결과는 위 현재 관측 표를 따른다. 자연 완주·새 빌드는 아직 미수용이다. 원시 증거는 공개 Git에서 제외한다.

기존 fixture의 보존본 경로3곳/동일파일 검증 예제 경로1곳과 옛 부모 핀만 현재 활성 경로/부모로 맞춘다. 기존 Approved scope 지문은 실제 preparation/astra-scope-decision.md와 일치한다. 새 현재 manifest와 독립 Luna gate를 정상 발급하고 실제 PID/기동시각/CLI/선택/격리·복원 검증을 유지한다. 과거 gate·실패·소진 원문은 재사용·변조하지 않고 이전 시도의 횟수나 성공을 새로 꾸미지 않는다.

# 현재 개발 경로와 비Unity 구현 — 2026-11-07

PR #11은 `a11d75270fc87c60220dcee1e04b16dbd37af6f4`로 원격 main에 병합됐다. 현재 개발 작업공간은 `H:/GPT-workspace/AcadeGameMaker-integration-20261107`, 활성 제품 상대 경로는 `output/chapter1-playable-r1/project`다. 이전 `H:/GPT-workspace/AcadeGameMaker`는 기존 main·dirty 수정·미게시 80커밋 이력·원시 자료의 보존본이다. 원본을 force/reset하거나 새 제품 수정을 역복사하지 않는다.

산사 자원을 우선하여 Unity Editor·Hub·batchmode·Player·빌드는 실행 보류다. 설계·코드 구현과 가벼운 비Unity 검증은 계속한다. 이 단계는 기존 소유 점프 fallback의 빠진 안전 조건을 보완한다. 일반/미소유 판정, 생산 HP·좌표·시간·전투와 기존 승인·실패·소진 원문은 바꾸지 않는다.

DebtLine은 Execute age0 단발이며 hostile 접촉은 그 tick의 직전 완료 pose로 평가하고 피해만 t+1에 배송한다. 포락선은 이 실제 접촉 시점과 추가 보수 샘플의 하한을 검사한다. 최대 상승·느린 중력/낙하 상한으로 계산한 착지, 벽·보스 몸체, Stage3 최단 실행18+복구33tick·다음 예고60tick·직전 pose1tick과 입력 지연을 함께 제한한다. 실제 WallSide/WallId 관측도 소유 계속을 철회한다. 신원·lease·sequence·focus·phase/slot·접촉/예약/당김 거절은 유지한다. E2 y/vy는 순수 계산 반례이며 과거 실행의 현재 소유 자격이나 실제 전투 성공을 입증하지 않는다.

재현은 저장소 루트에서 `powershell.exe -NoProfile -File qa/tools/Test-Ch1VerificationPolicy.ps1`이다. 설치된 .NET SDK/reference pack만 사용하고 다운로드나 Unity 어셈블리 로드는 없다. 실제 소스 policy/상수/시기/저장 메서드를 추출해 계산·상태·파일시스템을 검사한다. 값 어댑터는 enum 이름과 항상 비활성인 단독 세션 형식만 제공하며 Unity 물리 모사가 아니다. 양성 소유 검사는 기존 fixture lease 값 입력이고 실제 단독 Driver 권한 양성의 증거가 아니다.

현재 Unity NUnit 실행, 실제 물리·충돌·동일 배포본 보스 전투·전체 회귀·새 빌드·실제 CH1 완주는 미검증이다. 다음 실행 재개 시 현재 활성 경로와 새 커밋을 사용하고 과거 Source 핀이나 보존 원본에서 Unity를 시작하지 않는다. CH1 패키지 성숙도와 최종 수용 수치는 이전 감사의 별도 지표이며 이번 정적 보정을 전체 게임 완료율로 환산하지 않는다.
