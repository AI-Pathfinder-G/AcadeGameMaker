# [VD-09] Platform and Quality Baseline

- Status: Draft
- Owner: Sol
- Unit implementation: Terra
- Verification: Luna
- Last updated: 2026-08-24

## Contract

수직 데모의 엔진, 렌더링, 대상 플랫폼, 입력·성능·저장·빌드 기준을 소유한다. 패키지 추가, 전역 설정, 공개 스키마 변경은 Sol 승인 없이 할 수 없다.

## Requirements

- **REQ-PLAT-001:** Unity 6.3 LTS, C#, URP 2D로 Windows PC 데모를 만든다.
- **REQ-PLAT-002:** 지원 입력 기기, 기준 해상도, 화면 모드, 목표 프레임률을 구현 전 고정한다.
- **REQ-PLAT-003:** 저장 데이터는 버전을 포함하고 실패 시 플레이 가능한 초기 상태로 안전하게 복구한다.
- **REQ-PLAT-004:** 개발 경고를 제외한 처리되지 않은 예외와 누락 참조 없이 기준 경로를 완주한다.
- **REQ-PLAT-005:** 편집기와 Windows 빌드에서 같은 게임플레이 결과와 상태 전이를 재현한다.

## Acceptance criteria

### AC-PLAT-001 — 환경 식별

- **Given** 검수 대상 프로젝트와 빌드가 있고
- **When** 프로젝트 설정과 빌드 정보를 확인하면
- **Then** 엔진 버전, 렌더 파이프라인, 플랫폼, 입력·성능 기준이 이 스펙과 일치한다.

### AC-PLAT-002 — 기준 경로 안정성

- **Given** 깨끗한 로그와 신규 상태가 있고
- **When** 편집기와 Windows 빌드에서 VD-10 기준 경로를 각각 실행하면
- **Then** 처리되지 않은 예외, 누락 참조, 진행 불능 없이 끝나고 동일한 필수 결과를 남긴다.

### AC-PLAT-003 — 저장 복구

- **Given** 정상 저장과 지원하지 않는 버전 또는 손상된 저장의 검수 표본이 있고
- **When** 각각 로드하면
- **Then** 정상 저장은 보존 상태를 복구하고 비정상 저장은 진단 기록 후 플레이 가능한 안전 상태로 전환된다.

## Verification

프로젝트 설정 스냅샷, 빌드 로그, 성능 캡처, 저장 호환성 테스트로 검증한다. 입력·해상도·프레임·저장 포맷은 `OD-PLAT-001` 해결 전까지 미승인이다.

## Traceability

[ADR-0010](../../adr/0010-unity-for-the-asset-first-prototype.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [VD-10](./10-verification-script.md)
