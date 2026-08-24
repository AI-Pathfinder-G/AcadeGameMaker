# [VD-09] Platform and Quality Baseline

- Status: Draft
- Owner: Sol
- Unit implementation: Terra
- Verification: Luna
- Last updated: 2026-08-24

## Scope

Unity·URP 2D·Windows 기준, 입력 기기·화면·프레임 제품 기준, 저장 어댑터와 빌드 품질을 포함한다.

## Non-scope

출시 인증, 콘솔·모바일·웹, 온라인 서비스, 활성 런 저장과 보류된 제작 도구는 포함하지 않는다.

## Contract

수직 데모의 엔진, 렌더링, 대상 플랫폼, 입력·성능·저장·빌드 기준을 소유한다. 패키지 추가, 전역 설정, 공개 스키마 변경은 Sol 승인 없이 할 수 없다.

### Inputs

승인된 제품 기준, 프로필 snapshot 요청, 빌드 대상, 프로젝트 설정과 검증 실행 요청.

### Outputs

버전 저장·로드 결과, 안전 복구 결과, 환경 식별 정보, 편집기·Windows 빌드와 품질 증적을 제공한다.

### Owned state

Unity 전역 설정, 패키지 잠금, 입력·표시·성능 기준, 버전 저장 스키마와 원자 파일 트랜잭션을 소유한다. 라이브 런·선택·전이 상태는 소유하지 않는다.

### Invariants

- Unity 온라인 기능은 기본 비활성이다.
- 저장 payload에는 활성 scene object 또는 런타임 Component 참조가 없다.
- 비정상 저장은 기존 정상 snapshot을 훼손하지 않고 플레이 가능한 거점 상태로 복구한다.

## Requirements

- **REQ-PLAT-001:** Unity 6.3 LTS, C#, URP 2D로 Windows PC 데모를 만든다.
- **REQ-PLAT-002:** Windows 10/11 x64에서 키보드와 XInput 게임패드를 지원하고 마우스를 UI·조준에 허용하며, 16:9 1920×1080 기준의 창/전체 화면 전환, 60 FPS, 1/60초 고정 물리 간격을 사용한다.
- **REQ-PLAT-003:** 저장 데이터는 버전을 포함하고 실패 시 플레이 가능한 초기 상태로 안전하게 복구한다.
- **REQ-PLAT-004:** 개발 경고를 제외한 처리되지 않은 예외와 누락 참조 없이 기준 경로를 완주한다.
- **REQ-PLAT-005:** 편집기와 Windows 빌드에서 같은 게임플레이 결과와 상태 전이를 재현한다.
- **REQ-PLAT-006:** 활성 런은 저장하거나 재개하지 않고, 설정·입력 바인딩·튜토리얼 확인과 확정된 선택·해금 기술·완료 분기만 각 승인 시점에 원자 저장한다.
- **REQ-PLAT-007:** `com.unity.inputsystem` 1.20.0을 고정하고 Active Input Handling은 Input System Package (New)만 사용하며 구형 Input Manager와 Both 모드를 금지한다.

## Acceptance criteria

### AC-PLAT-001 — 환경 식별

- **Given** 검수 대상 프로젝트와 빌드가 있고
- **When** 프로젝트 설정과 빌드 정보를 확인하면
- **Then** 엔진 버전, 렌더 파이프라인, Windows x64 대상, 입력 기기, 1920×1080 기준, 화면 모드, 60 FPS와 1/60초 물리 간격이 이 스펙과 일치한다.

### AC-PLAT-002 — 기준 경로 안정성

- **Given** 깨끗한 로그와 신규 상태가 있고
- **When** 편집기와 Windows 빌드에서 VD-10 기준 경로를 각각 실행하면
- **Then** 처리되지 않은 예외, 누락 참조, 진행 불능 없이 끝나고 동일한 필수 결과를 남긴다.

### AC-PLAT-003 — 저장 복구

- **Given** 정상 저장과 지원하지 않는 버전 또는 손상된 저장의 검수 표본이 있고
- **When** 각각 로드하면
- **Then** 정상 저장은 보존 상태를 복구하고 비정상 저장은 진단 기록 후 플레이 가능한 안전 상태로 전환된다.

### AC-PLAT-004 — 활성 런 비보존

- **Given** 원정 중 방·적·보상·활성 전이 상태와 별도로 확정된 프로필 상태가 있고
- **When** 앱을 종료한 뒤 다시 시작하면
- **Then** 활성 런 상태는 복원되지 않고 거점의 새 원정 상태가 되며 승인 시점까지 확정된 프로필 상태만 복원된다.

### AC-PLAT-005 — 입력 패키지 고정

- **Given** 프로젝트 manifest, packages lock과 Player Settings가 있고
- **When** 입력 의존성과 Active Input Handling을 검사하면
- **Then** `com.unity.inputsystem`은 정확히 1.20.0이고 신규 Input System만 활성화되며 레거시 입력 API 의존성이 없다.

## Verification

프로젝트 설정 스냅샷, package lock, 빌드 로그, 성능 캡처, 저장 호환성 테스트로 검증한다. Input System 패키지·map 수·Gameplay/UI 기본 binding과 runtime rebind 범위는 확정됐지만 override 충돌 정책과 구체 저장 스키마는 `OD-PLAT-001` 해결 전까지 미승인이다.

## Traceability

[환경·제품 기준 승인 기록](../../approvals/2026-08-24-environment-and-product-baseline.md), [ADR-0010](../../adr/0010-unity-for-the-asset-first-prototype.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md), [VD-05](./05-failure-and-persistence.md), [VD-10](./10-verification-script.md)
