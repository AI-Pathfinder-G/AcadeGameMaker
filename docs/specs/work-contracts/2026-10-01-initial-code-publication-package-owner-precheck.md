# 최초 코드 게시 패키지 식별자 소유 예비 조사

- 상태: **Approved — 현 로컬 캐시 읽기 전용 조사 한정.** 2026-10-01 아스트라(`gpt-6-astra`). 사용자가 루나 검수를 한도 회복 뒤 별도로 진행하도록 지시한 동안의 예비 작업이다. 패키지 수정·설치·복제·Unity 실행이나 게임 코드 게시를 승인하지 않는다.
- 기준: v2 501행 JSON SHA-256 `23F3060352AD230EE86FE480560A0591D72EC41ABCA18C421610257AEBF206FE`; 기반 33 결과（`../../../artifacts/c4-initial-publication-foundation-33-precheck-v1.json`；로컬 보존 자료） SHA-256 `1CBA6A98A110B671BB85D53A4CE606D5D0F206E3F2132621090142F1D1688836`; 메타·조립 결과（`../../../artifacts/c4-initial-publication-meta-asmdef-precheck-v1.json`；로컬 보존 자료） SHA-256 `9CF7837E600EE3862EA88F7C01D5FB8BE0A69C9FF836A5C9C8F6067B23CC509A`.
- 부모 영향: `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`. 이 조사만의 고유 ID는 `REQ-PLAT-026..028`, `AC-PLAT-024..026`이다.

## 범위와 방법

메타·조립 결과의 `후보 소유 미발견` GUID 참조 291건과 범위 밖 `.asmdef` 참조 13건을 정확 입력 집합으로 삼는다. `Packages/manifest.json`, `Packages/packages-lock.json`, 현재 로컬 `Library/PackageCache`의 실제 패키지 디렉터리·`package.json`·`.meta`·`.asmdef`만 읽는다. GUID는 291개 **참조 행**과 고유 값을 구분해 조회하고, 발견하면 패키지 이름·캐시 디렉터리·패키지 선언 버전·잠금 버전·정확 메타 경로·GUID 줄·메타 SHA와 중복 소유자를 기록한다. 조립 이름은 캐시 `.asmdef`의 실제 `name` 및 위치로 결속한다. 캐시 경로가 없는 항목은 엔진 내장 추정과 증거 부재를 구분하되 소유 확정으로 처리하지 않는다.

패키지 요청과 잠금의 모든 직접 의존 버전을 비교한다. 사전 관측한 `com.unity.render-pipelines.universal` 요청 `17.3.0`/잠금 `17.6.0`, `com.unity.test-framework` 요청 `1.6.0`/잠금 `1.8.0`을 그대로 재검증하고 차이가 실제이면 위험·후속 재현 게이트로 기록한다. 현 로컬 캐시의 소유 일치는 **깨끗한 복제나 원격 게시의 재현성 증거가 아니다**. 캐시 안의 패키지가 잠금 내용과 다르거나 동일 GUID/조립 이름에 복수 소유가 있으면 모호 상태로 남긴다.

501 후보 실물의 조사 시작·끝 SHA를 v2 기준과 대조한다. 기존 501 파일, 기존 세 원장, 패키지 캐시, Git·원격·Unity·QA는 수정하거나 실행하지 않는다. 허용 새 출력은 `artifacts/c4-initial-publication-package-owner-precheck-v1.json`, 같은 이름 `.md`, `docs/verification/2026-10-01-initial-code-publication-package-owner-terra-report.md` 세 파일이다. 전체와 각 행의 `PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`, `PortablePublicationEvidence`는 거짓이다.

| 요구 | 인수 기준 |
| --- | --- |
| `REQ-PLAT-026` — 입력과 현재 실물의 지문·행 수를 고정한다. | `AC-PLAT-024`: 세 입력의 정확 SHA와 291 GUID 참조 행·13 조립 참조 행을 확인하고, 501 후보 실물의 시작·끝 SHA가 각각 501/501 일치한다. 어긋나면 성공으로 기록하지 않는다. |
| `REQ-PLAT-027` — 로컬 패키지 소유 증거와 불명 항목을 분리한다. | `AC-PLAT-025`: 291 GUID 참조 행과 13 조립 참조 행 각각에 현 캐시 단일 소유·복수 소유·소유 미발견 분류가 있고, 발견된 소유에는 패키지·버전·실물 파일·원문 위치·SHA가 있다. 직접 패키지 요청/잠금 버전 차이 전부와 캐시 선언 버전의 일치/차이를 출력한다. |
| `REQ-PLAT-028` — 캐시 관측을 이식 가능한 게시 근거로 승격하지 않는다. | `AC-PLAT-026`: JSON·판독 문서의 수량이 일치하며 모든 게시·의존 폐쇄·전체 수용·루나 검수·이식 가능 게시 증거 표시가 거짓이다. 미해결 소유, 버전 차이, 현 캐시 종속성, 원격·깨끗한 복제·Unity 미검증을 명시한다. |

테라 역할(`gpt-6-sol`)이 위 세 출력과 자체 확인을 작성한다. 루나 독립 검수와 아스트라 결과 수용은 사용량 한도 회복 뒤 별도다. 이 계약은 501파일 전체 내용·의존성 계약 초안의 승인이나 파일별 코드 게시 결정을 대신하지 않는다.
