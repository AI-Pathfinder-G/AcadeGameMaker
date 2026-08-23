# [VD-08] Art and Asset Integration

- Status: Review
- Owner: Terra
- Direction and license approval: Sol
- Verification: Luna
- Last updated: 2026-08-24

## Contract

고해상도 픽셀 아트와 관료주의 디젤펑크 방향을 유지하면서 여러 무료 에셋을 일관되게 조합하는 수용 기준과 라이선스 추적 규칙을 정의한다. 실제 다운로드와 도입은 이 단계의 범위가 아니다.

## Requirements

- **REQ-ART-001:** 환경은 황동 계량기, 배관, 거대한 추, 붉은 체납 도장, 청회색 콘크리트의 시각 언어를 사용한다.
- **REQ-ART-002:** 에셋은 공통 픽셀 밀도, 팔레트, 윤곽선, 조명 레이어 규칙에 맞거나 수정 계획을 가져야 한다.
- **REQ-ART-003:** 모든 후보와 도입 에셋은 출처 URL, 제작자, 라이선스 원문 URL, 허용 범위, 표기 의무, 수정 여부, 검증일을 기록한다.
- **REQ-ART-004:** 라이선스가 불명확하거나 프로젝트 사용 조건과 충돌하는 에셋은 도입하지 않는다.
- **REQ-ART-005:** 캐릭터, 위험물, 전이 가능 대상은 배경과 구분되는 실루엣·대비·피드백을 가져야 한다.

## Acceptance criteria

### AC-ART-001 — 에셋 등록 완전성

- **Given** 도입 후보 에셋이 있고
- **When** [에셋 등록부](../../assets/asset-register.md)에 등록하면
- **Then** REQ-ART-003의 필드가 모두 채워지고 Sol의 라이선스 상태가 `Approved`이기 전에는 프로젝트 도입 상태로 바뀌지 않는다.

### AC-ART-002 — 씬 일관성과 가독성

- **Given** 서로 다른 출처의 후보 에셋을 합성한 검수 장면이 있고
- **When** 캐릭터가 이동·전투·무게 전이를 수행하면
- **Then** 핵심 상호작용 대상이 배경과 구분되고 아트 캐논의 재질·색·조명 규칙에서 벗어난 항목이 체크리스트에 남는다.

## Verification

에셋 등록부 완전성 검사, 라이선스 원문 수동 확인, 대표 검수 장면 캡처 비교를 사용한다.

## Traceability

[미술 캐논](../../canon/art-direction.md), [ADR-0010](../../adr/0010-unity-for-the-asset-first-prototype.md), [ADR-0011](../../adr/0011-high-resolution-pixel-art.md), [ADR-0012](../../adr/0012-bureaucratic-dieselpunk-art-direction.md)
