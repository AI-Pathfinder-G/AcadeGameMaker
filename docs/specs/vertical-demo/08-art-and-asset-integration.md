# [VD-08] Art and Asset Integration

- Status: Review
- Owner: Terra
- Direction and license approval: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Scope

- 공식 원출처 후보 조사와 에셋 등록부 기록
- Sol의 미술 적합성·라이선스 승인
- 승인된 원본 아카이브의 Git 제외 격리 다운로드
- 라이선스 증적, 다운로드 URL, 검증일, SHA-256 기록
- Approved 이후의 Unity 임포트와 대표 검수 장면 일관성 검사

## Non-scope while status is Review

- Unity 프로젝트 또는 `Assets/` 임포트
- 원본 파일 수정, 재배포 패키지 제작, 팔레트 변환
- OD-ART-001 이전의 픽셀 밀도·카메라·조명 통합 결정

## Contract

- **Input:** 공식 제작자·배포자 페이지, 라이선스 원문, 원본 다운로드 파일, 미술 캐논
- **Output:** 완전한 등록부 행, 버전 관리되는 라이선스 증적, Git에서 제외된 원본 아카이브와 SHA-256
- **Owned state:** `docs/assets/asset-register.md`, `docs/assets/evidence/`, `third_party/quarantine/`
- **Invariants:** 라이선스가 불명확한 파일은 다운로드하지 않는다. 다운로드 파일은 임포트·실행·수정하지 않는다. 등록부의 `Approved by Sol`과 증적·체크섬이 모두 존재하기 전에는 상태를 `Quarantined`로 바꾸지 않는다.

후보 조사와 격리 보관은 구현이 아니므로 이 스펙이 Review인 동안 수행할 수 있다. Unity 임포트와 파생 작업은 이 스펙이 Approved이고 OD-ART-001이 해결된 뒤에만 시작한다.

공통 환경 격자는 `18 pixels per unit`이며 18×18 source tile은 1×1 world unit에 native mapping한다. 다른 밀도의 후보는 automatic resample하지 않고 18픽셀 격자에 맞춘 padding·crop·redraw 계획을 등록부에 기록한다. Pixel Perfect Camera reference resolution과 gameplay 내부 render canvas는 640×360이며 nearest-neighbor integer scale로 2560×1440에서 정확히 4배 확대한다. 이 frame은 약 35.56×20 world units를 표시한다.

viewport에는 들어가는 가장 큰 integer scale의 640×360 gameplay rectangle을 중앙 배치한다. 비16:9 viewport는 더 많은 world를 노출하거나 crop·stretch하지 않고 넓은 화면은 좌우 pillarbox, 좁거나 더 높은 화면은 상하 letterbox를 사용한다. minimum window size는 640×360이며 scale 1을 허용한다. pixel-art world, HUD와 interactive UI는 gameplay rectangle/safe frame 안에만 배치한다. 여백의 정확한 색·장식은 palette 결정에 둔다.

Gameplay camera는 고정 orthographic size에서 player simulation position을 dead-zone follow한다. horizontal anticipation은 player movement direction만 사용하고 mouse pointer는 camera target에 영향을 주지 않는다. 상승은 작은 upward bias, 빠른 낙하는 더 큰 downward bias를 사용한다. dash는 zoom·hard snap을 유발하지 않는다. camera center는 authored room bounds 안에서 clamp하고 gameplay dynamic zoom을 금지한다. boss·choice·cutscene만 authored camera anchor를 사용할 수 있다. camera pose는 movement 완료 뒤 같은 60Hz tick에서 계산해 1/18 world-unit로 snap하고 그 완료 snapshot을 다음 tick aim이 사용한다. 별도 render-only smoothing pose는 허용하지 않는다.

GameplayFollow의 axis-aligned dead zone은 camera center 기준 6×4u다. horizontal offset target은 `|velocityX|≥2.0`이면 `sign(velocityX)×3.0u`, 아니면 0이다. vertical offset target은 `velocityY≥4.0`이면 +1.5u, `velocityY≤-6.0`이면 -3.0u, 그 사이는 0이다. target band가 바뀌면 현재 offset에서 새 target까지 정확히 12 tick linear transition을 시작하며 전환 중 다시 band가 바뀌면 현재 값을 새 start로 삼는다.

offset을 더한 player focus가 current camera dead zone 밖일 때만 desired center를 해당 nearest dead-zone edge까지 이동한다. camera center는 desired center를 향해 axis-independent `MoveTowards` max 0.5u/tick을 적용한 뒤 authored room bounds clamp, 각 축 1/18u AwayFromZero snap 순서로 확정한다. room entry, respawn, teleport, authored anchor 진입·해제만 이 추적을 생략하고 same-tick snap할 수 있다. dash와 단순 direction change는 snap 사유가 아니다.

HUD·menu layout은 640×360 logical safe frame을 사용하고 gameplay rectangle과 같은 integer scale·offset을 따른다. pixel frame·icon은 point filtering으로 확대한다. TextMeshPro SDF text는 logical position과 size를 사용하되 world upscale 뒤 final output resolution에서 render한다. logical font size는 minimum body 12px, standard body 14px, heading 18px이며 interactive hit area는 최소 24×24 logical px, safe margin은 frame edge에서 12 logical px다. 수직 데모는 user-adjustable UI scale을 제공하지 않는다.

master palette는 world 21색과 semantic accent 11색의 32-color role set이다. 평균 장면의 목표 면적 비중은 cold blue-grey/charcoal 70%, brass/rust/concrete 20%, semantic accents 합계 10% 이하다. semantic accent는 ordinary background·prop decoration에 사용할 수 없다. 11개 accent slot은 TransferReady electric cyan, ActiveTransfer white, Cooldown amber orange, RangeOrLineOfSightBlocked debt-stamp red, Extraction crimson·ink violet, Solidarity pale cyan·ivory, heroine warm ivory·muted rose·gold에 하나씩 배정한다. Solidarity ivory와 heroine warm ivory는 서로 다른 색이다. heroine identity는 interactable·loot·reward indicator에 재사용하지 않는다. URP light는 value·saturation을 바꿀 수 있지만 semantic hue family를 다른 상태 family로 회전시키지 않는다. 모든 상태는 색과 함께 승인된 line shape·outline count를 사용한다.

## Requirements

- **REQ-ART-001:** 환경은 황동 계량기, 배관, 거대한 추, 붉은 체납 도장, 청회색 콘크리트의 시각 언어를 사용한다.
- **REQ-ART-002:** 에셋은 공통 픽셀 밀도, 팔레트, 윤곽선, 조명 레이어 규칙에 맞거나 수정 계획을 가져야 한다.
- **REQ-ART-003:** 모든 후보와 도입 에셋은 출처 URL, 제작자, 라이선스 원문 URL, 허용 범위, 표기 의무, 수정 여부, 검증일을 기록한다.
- **REQ-ART-004:** 라이선스가 불명확하거나 프로젝트 사용 조건과 충돌하는 에셋은 도입하지 않는다.
- **REQ-ART-005:** 캐릭터, 위험물, 전이 가능 대상은 배경과 구분되는 실루엣·대비·피드백을 가져야 한다.
- **REQ-ART-006:** 격리 다운로드는 원본 파일명, 최종 다운로드 URL, 다운로드 시각, 파일 크기, SHA-256, 로컬 격리 경로를 기록해야 한다.
- **REQ-ART-007:** `Needs clarification` 또는 `Rejected`인 후보는 다운로드·임포트하지 않으며, 격리 파일은 Unity나 기타 제작 도구에서 실행·열기·변환하지 않는다.
- **REQ-ART-008:** 환경·타일·pixel-art VFX는 18 PPU 공통 격자를 사용하고 비18px source는 자동 보간 확대 없이 명시된 수작업 적응 계획을 가져야 하며 최종 출력은 2560×1440을 기준으로 해야 한다.
- **REQ-ART-009:** gameplay world는 640×360 내부 pixel canvas를 사용해 2560×1440에서 nearest-neighbor 4배 정수 확대하고 18 PPU 기준 약 35.56×20 world-unit framing을 유지해야 한다.
- **REQ-ART-010:** 모든 지원 viewport는 중앙 고정 16:9 gameplay rectangle에 640×360의 최대 integer scale을 사용하고 비16:9 잔여 영역은 letterbox/pillarbox로 처리하며 world reveal·crop·stretch와 safe-frame 밖 UI를 금지해야 한다.
- **REQ-ART-011:** gameplay camera는 6×4u dead zone, ±3u horizontal·+1.5/-3u vertical anticipation, 12-tick offset transition과 max 0.5u/tick follow를 사용하고 mouse-driven pan·dynamic zoom·dash snap 없이 room bounds와 1/18-unit fixed-tick pose를 지키며 승인 생명주기에서만 snap·anchor를 허용해야 한다.
- **REQ-ART-012:** UI는 640×360 logical safe frame과 gameplay integer scale을 사용하고 pixel frame·icon은 point filtering, text는 final-output SDF로 렌더하며 승인된 font·hit-area·margin 최소값을 지켜야 한다.
- **REQ-ART-013:** master palette는 world 21색·semantic 11색 역할을 분리하고 11개 의미 역할에 독립 slot을 배정하며 semantic accent의 장식 사용과 heroine identity 색의 reward/interactable 재사용을 금지하고 상태는 색 외 line·outline 신호를 함께 사용해야 한다.

## Acceptance criteria

### AC-ART-001 — 에셋 등록 완전성

- **Given** 도입 후보 에셋이 있고
- **When** [에셋 등록부](../../assets/asset-register.md)에 등록하면
- **Then** REQ-ART-003과 REQ-ART-006의 필드가 해당 단계에 맞게 채워지고 Sol의 라이선스 상태가 `Approved by Sol`이기 전에는 다운로드 상태로 바뀌지 않는다.

### AC-ART-002 — 씬 일관성과 가독성

- **Given** 서로 다른 출처의 후보 에셋을 합성한 검수 장면이 있고
- **When** 캐릭터가 이동·전투·무게 전이를 수행하면
- **Then** 핵심 상호작용 대상이 배경과 구분되고 아트 캐논의 재질·색·조명 규칙에서 벗어난 항목이 체크리스트에 남는다.

### AC-ART-003 — 격리 아카이브 추적성

- **Given** Sol이 승인한 후보 에셋이 있고
- **When** 원본 파일을 `third_party/quarantine/`에 다운로드하면
- **Then** 등록부와 증적에 최종 URL·다운로드 시각·파일 크기·SHA-256·격리 경로가 일치하고 파일은 Git 추적 및 Unity 임포트 대상에서 제외된다.

### AC-ART-004 — 미승인 파일 차단

- **Given** 라이선스 상태가 `Needs clarification` 또는 `Rejected`인 후보가 있고
- **When** 확보 작업을 검수하면
- **Then** 해당 후보의 격리 파일과 Unity 임포트 파일이 존재하지 않는다.

### AC-ART-005 — 18 PPU와 출력 기준

- **Given** native 18×18 tile, 16×16 후보, pixel-art VFX와 2560×1440 검수 화면이 있고
- **When** import 설정·world grid·screen capture를 검사하면
- **Then** native tile은 18 PPU에서 정확히 1×1 unit이고 16×16 후보는 자동 bilinear resample 없이 승인된 padding·crop·redraw 계획을 따르며 최종 출력은 2560×1440이다.

### AC-ART-006 — 내부 캔버스와 정수 확대

- **Given** 18 PPU pixel-art 검수 장면과 1280×720, 1920×1080, 2560×1440 viewport가 있고
- **When** 각 viewport에서 같은 simulation camera pose를 렌더하면
- **Then** gameplay world의 logical canvas는 모두 640×360이고 각각 nearest-neighbor 2×·3×·4× 정수 확대되며 2560×1440 frame은 약 35.56×20 world units이고 pixel edge에 fractional sampling이 없다.

### AC-ART-007 — 종횡비와 최소 창

- **Given** 640×360 minimum, 1366×768, 2560×1440과 ultrawide·narrow viewport가 있고
- **When** 같은 simulation camera pose와 UI를 각각 렌더하면
- **Then** 중앙 gameplay rectangle은 가능한 최대 integer scale의 640×360이며 16:9가 아닌 잔여 영역만 letterbox/pillarbox가 되고 world framing·HUD 위치는 같고 crop·stretch·추가 world reveal이 없다.

### AC-ART-008 — 카메라 모드와 pixel snap

- **Given** dead-zone 6×4u의 안팎, X 속도 1.999/2.000/2.001, Y 속도 3.999/4.000/4.001과 -5.999/-6.000/-6.001, 걷기·방향 전환·상승·낙하·대시, mouse edge, room bound와 승인 snap lifecycle이 있고
- **When** 같은 movement·mouse 기록을 30/60/144 render FPS에서 재생하면
- **Then** horizontal target은 0/±3u, vertical target은 0/+1.5/-3u 경계에 맞고 offset은 band 전환 뒤 12 tick에 도달하며 camera는 dead-zone 보정 뒤 각 축 max 0.5u/tick·room clamp·1/18u snap 순서로 exact match하고 mouse·대시는 pan·zoom·snap을 만들지 않으며 room entry·respawn·teleport·anchor 진입/해제에서만 same-tick snap한다.

### AC-ART-009 — UI 정수 배율과 텍스트 가독성

- **Given** minimum·standard·heading text, pixel frame·icon, 24×24 hit target과 12px safe margin을 포함한 HUD·menu가 있고
- **When** 640×360, 1280×720, 1920×1080, 2560×1440과 letterbox/pillarbox viewport에서 렌더하면
- **Then** layout은 같은 640×360 logical 좌표이고 pixel asset은 1×·2×·3×·4× point scale, SDF text는 final output에서 선명하며 font size는 12/14/18 logical px 이상, hit area와 margin은 각각 24×24·12 logical px 이상이고 safe frame 밖 interactive UI가 없다.

### AC-ART-010 — 팔레트 역할과 의미색 보호

- **Given** hub·expedition·boss·choice·heroine 장면과 모든 target/skill/UI state의 색상 사용표가 있고
- **When** 32색 role assignment와 representative capture를 검사하면
- **Then** world 21색·semantic 11색 밖의 무승인 base color가 없고 11개 의미 역할이 서로 지정된 slot을 사용하며 semantic accent 면적은 평균 장면 10% 이하이고 일반 장식은 의미색을, reward/interactable은 heroine identity 색을 사용하지 않으며 각 gameplay state는 색 외 승인된 shape 신호를 함께 가진다.

## Verification

에셋 등록부 완전성 검사, 라이선스 원문 수동 확인, SHA-256 재계산, Git 추적 제외 검사, 대표 검수 장면 캡처 비교를 사용한다.

## Traceability

[미술 캐논](../../canon/art-direction.md), [ADR-0010](../../adr/0010-unity-for-the-asset-first-prototype.md), [ADR-0011](../../adr/0011-high-resolution-pixel-art.md), [ADR-0012](../../adr/0012-bureaucratic-dieselpunk-art-direction.md), [ADR-0017](../../adr/0017-quarantine-assets-before-import.md)
