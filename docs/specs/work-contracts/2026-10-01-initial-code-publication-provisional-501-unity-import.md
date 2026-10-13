# 최초 코드 게시 후보 501파일 격리 Unity 가져오기 진단

- 상태: **Approved — 한 번의 격리 진단 한정.** 2026-10-01 아스트라(`gpt-6-astra`). 루나의 v2·예비 조사 독립 검수는 사용자 지시에 따라 한도 회복 뒤 별도다. 이 계약은 파일별 포함 승인·깨끗한 Git 복제·게시 수용·기존 Unity 시험 재실행이 아니다.
- 기준: `artifacts/c4-initial-publication-forward-provenance-v2.json` SHA-256 `23F3060352AD230EE86FE480560A0591D72EC41ABCA18C421610257AEBF206FE`의 정확한 501개 `Path/ForwardBaselineSha256`; 잔여 GUID 조사（`../../../artifacts/c4-initial-publication-residual-guid-precheck-v1.json`；로컬 보존 자료） SHA-256 `EE593DBAEFF30B9354375B5A7F902E96FC323016A99757CC707D4A36C6836CB4`; [패키지 버전 문맥](../../verification/2026-10-01-package-version-authority-crosscheck.md).
- 부모 영향: `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`. 이 진단 고유 ID는 `REQ-PLAT-032..034`, `AC-PLAT-030..032`다.

## 정확 범위와 실행

원본 프로젝트의 501 실물 SHA를 먼저 기준과 전수 비교한다. 정확한 501파일만 새로운 임시 프로젝트 디렉터리의 같은 상대 경로로 복사하고, 복사본 501파일의 SHA를 전수 비교한다. `Library`, `.git`, 후보 밖 파일 또는 기존 원시 QA 결과는 복사하지 않는다. 임시 프로젝트는 사용자 작업공간의 원본 밖에 두고 경로·생성 UTC를 기록한다. 원본 501개는 실행 종료 뒤에도 전수 SHA를 대조한다. 원본에 불일치가 있으면 복사·Unity 실행을 중단한다.

설치된 Unity `6000.6.0f1` 개정 `f7f8ed4d1e24`가 정확한 경로에 있는지 확인하고, 다른 Unity 편집기가 원본 또는 임시 프로젝트를 사용 중이면 실행하지 않는다. 임시 복사본에서만 Unity를 한 번 `-batchmode -nographics -quit -projectPath <복사본> -logFile <새 로그>`로 실행한다. 시험, 자동 API 갱신, 파일 수정 허용 플래그, 패키지 수동 설치·버전 변경, 원격 쓰기·Git 변경은 수행하지 않는다. 실행에 시간 상한을 두고 종료 코드·실제 시작/끝 UTC·편집기 로그·프로세스 종료 상태를 기록한다. 라이선스·네트워크·패키지 오류나 시간 초과는 그 상태 그대로 기록하고 반복하지 않는다.

실행 전후 복사본의 501파일 SHA, `Packages/manifest.json`과 `packages-lock.json`의 바이트·직접 버전, Unity가 새로 만든 파일과 로그의 경로·SHA를 기록한다. 편집기가 복사본 원본 파일을 바꾸면 자동 성공으로 처리하지 않는다. 로그에서 패키지 해석, 가져오기/컴파일 오류, `Renderer2D`의 일곱 GUID 관련 진단을 원문 줄과 함께 추출한다. 로그에 해당 문자열이 없다는 사실은 참조 해석·렌더 동작의 성공 증거가 아니다. 전체 로그를 보존하고 요약만 별도 출력한다.

허용 출력은 임시 프로젝트와 그 안의 Unity 생성물, `artifacts/c4-initial-publication-provisional-501-unity-import-v1.json`, 같은 이름 `.md`, `docs/verification/2026-10-01-initial-code-publication-provisional-501-unity-import-terra-report.md`, `artifacts/c4-initial-publication-provisional-501-unity-import-v1.log`다. 원본 게임 코드·설정·자산·QA 입력·기존 증거·Git·원격은 수정하지 않는다. 임시 복사본은 로그 검토를 위해 보존하며 이 작업에서 재귀 삭제하지 않는다. 전체와 모든 결과 행의 `PublicationApproved=false`, `DependencyClosure=false`, `WholeAccepted=false`, `LunaReviewed=false`, `CleanCloneExecuted=false`를 유지한다.

| 요구 | 인수 기준 |
| --- | --- |
| `REQ-PLAT-032` — 정확한 입력 바이트를 격리한다. | `AC-PLAT-030`: 원본 조사 전후 501/501, Unity 실행 전 복사본 501/501을 전수 대조하고 후보 밖 파일이 처음 복사된 수는 0이다. 변경·누락·중복이 있으면 실행/성공을 차단한다. |
| `REQ-PLAT-033` — 단 한 번의 실제 편집기 진단을 증거로 남긴다. | `AC-PLAT-031`: 정확 편집기 경로·개정, 인수·임시 경로, 시작/끝 UTC, 바깥 종료 코드·시간 초과/라이선스/패키지 상태, 전체 로그 SHA, 복사본 전후 패키지 버전·바이트와 오류/경고 원문 위치가 있다. 실행 실패도 사실대로 판정한다. |
| `REQ-PLAT-034` — 진단과 게시 수용을 분리한다. | `AC-PLAT-032`: JSON·판독 문서·테라 보고의 수량/상태가 일치하고, 실제 GUID 해석/컴파일/패키지 상태의 증거 한계를 명시한다. `PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`, `CleanCloneExecuted`는 모두 거짓이다. |

테라 역할(`gpt-6-sol`)이 실행과 네 출력·로그를 소유한다. 루나 독립 검수와 아스트라 결과 수용은 이후 별도다. 임시 복사본 결과는 미래의 정확 게시 허용 목록을 사용한 깨끗한 Git 복제 시험을 대신하지 않는다.
