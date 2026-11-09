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
