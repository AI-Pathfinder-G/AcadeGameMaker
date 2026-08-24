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

공통 환경 격자는 `18 pixels per unit`이며 18×18 source tile은 1×1 world unit에 native mapping한다. 다른 밀도의 후보는 automatic resample하지 않고 18픽셀 격자에 맞춘 padding·crop·redraw 계획을 등록부에 기록한다. Pixel Perfect Camera reference resolution과 gameplay 내부 render canvas는 640×360이며 nearest-neighbor integer scale로 2560×1440에서 정확히 4배 확대한다. 이 frame은 약 35.56×20 world units를 표시한다. aspect 처리와 camera follow/framing은 `OD-ART-001`의 남은 결정이다.

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

## Verification

에셋 등록부 완전성 검사, 라이선스 원문 수동 확인, SHA-256 재계산, Git 추적 제외 검사, 대표 검수 장면 캡처 비교를 사용한다.

## Traceability

[미술 캐논](../../canon/art-direction.md), [ADR-0010](../../adr/0010-unity-for-the-asset-first-prototype.md), [ADR-0011](../../adr/0011-high-resolution-pixel-art.md), [ADR-0012](../../adr/0012-bureaucratic-dieselpunk-art-direction.md), [ADR-0017](../../adr/0017-quarantine-assets-before-import.md)
