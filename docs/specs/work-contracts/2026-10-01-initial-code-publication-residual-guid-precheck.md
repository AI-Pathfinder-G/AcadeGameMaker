# 최초 코드 게시 잔여 26개 식별자 참조 예비 조사

- 상태: **Approved — 읽기 전용 현 환경 조사 한정.** 2026-10-01 아스트라(`gpt-6-astra`). 루나의 독립 검수는 사용자 지시대로 한도 회복 뒤 별도다. 이 조사는 자산 변경·패키지 설치·Unity 실행·게시 허용이 아니다.
- 입력: v2 501행 JSON SHA-256 `23F3060352AD230EE86FE480560A0591D72EC41ABCA18C421610257AEBF206FE`; 패키지 캐시 조사 원장（`../../../artifacts/c4-initial-publication-package-owner-precheck-v1.json`；로컬 보존 자료） SHA-256 `ABF8ADA9B3C8E0445B1E0897527508BA849519F5AE32937819546E0E9AB54B92`의 `UnresolvedGuidReferences` 정확 26행·9고유 값. 부모 영향은 `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`. 이 단위 ID는 `REQ-PLAT-029..031`, `AC-PLAT-027..029`이다.

## 범위와 증거 경계

26개 참조 각각의 출발 파일·원문 줄·GUID·주변 직렬화 필드·`fileID`·`type`을 다시 읽는다. 값 `0000000000000000f000000000000000` 17행과 `0000000000000000e000000000000000` 2행은 영(零) 접두의 특수 값 **후보**로만 분류하고, 나머지 일곱 비영(非零) 값은 `Assets/Settings/Renderer2D.asset`의 실제 필드 문맥을 확인한다. 영 값이 엔진 내장이라는 해석이나 비영 값이 결함이라는 결론은 원문만으로 내리지 않는다.

정확 GUID 소유 `.meta` 파일을 `Assets`, `Packages`, 현 `Library/PackageCache`, 설치된 Unity `6000.6.0f1`의 `Editor/Data`에서 읽기 전용 검색한다. 각 검색 루트의 실제 존재, 조사 시각, 조사한 `.meta` 수와 정렬된 `상대 경로+SHA-256` 목록의 집계 지문을 남겨 **그 시점의 검색 범위**를 결속한다. 각 값의 발견 파일을 전부 기록하고, 없으면 `조사 범위 내 미발견`이라고만 쓴다. Unity 설치 위치가 다르거나 루트 접근에 실패하면 그 범위는 미조사로 명시한다. 검색 대상 밖의 자산·다른 Unity 설치·압축 내장 리소스까지 부재를 주장하지 않는다.

501 후보 실물의 조사 시작·끝 SHA를 v2 기준과 대조한다. 기존 501 파일, 패키지 캐시, Unity 설치, Git·원격·QA를 수정하거나 실행하지 않는다. 허용 새 출력은 `artifacts/c4-initial-publication-residual-guid-precheck-v1.json`, 같은 이름 `.md`, `docs/verification/2026-10-01-initial-code-publication-residual-guid-terra-report.md` 세 파일뿐이다. 전체·각 행의 `PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`, `PortablePublicationEvidence`는 거짓이다.

| 요구 | 인수 기준 |
| --- | --- |
| `REQ-PLAT-029` — 정확 입력과 변경 없는 후보를 확인한다. | `AC-PLAT-027`: 패키지 원장의 잔여 26행·9고유 GUID와 출력이 정확히 일치하고, 전체 501의 시작·끝 실물 지문이 기준과 각각 501/501 일치한다. |
| `REQ-PLAT-030` — 출발 문맥과 실제 검색 범위를 재현 가능하게 기록한다. | `AC-PLAT-028`: 26행 모두 원문 위치·필드 문맥·fileID/type과 루트별 검색 결과가 있다. 루트별 실제 `.meta` 집합 수·집계 지문, 발견/미발견/미조사를 분리하고, 한 값에 복수 소유가 있으면 모호함을 표시한다. 원문 필드 추정은 사실과 구분한다. |
| `REQ-PLAT-031` — 검색 부재와 게시 판단을 혼동하지 않는다. | `AC-PLAT-029`: JSON·판독 문서 수량이 같고, 검색 범위 밖·버전 차이 두 건·깨끗한 복제 미검증을 명시한다. 게시·의존 폐쇄·전체 수용·루나 검수·이식 가능 게시 증거 표시는 모두 거짓이다. |

테라 역할(`gpt-6-sol`)이 세 출력을 작성하고 자체 확인을 남긴다. 루나 독립 검수 및 아스트라 결과 수용은 이후 별도다. 이 계약은 501파일 내용·의존성 Draft의 승인이나 코드 게시 결정을 대신하지 않는다.
