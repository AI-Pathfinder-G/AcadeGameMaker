# 최초 코드 게시 501파일 짧은 경로 Unity 진단 재현

- 상태: **Approved — 별도 한 번 재현 한정.** 2026-10-01 아스트라(`gpt-6-astra`). [첫 격리 진단](../../verification/2026-10-01-initial-code-publication-provisional-501-unity-import-terra-report.md)의 패키지 자산 경로 접근 오류가 깊은 절대 경로와 관련되는지 분리한다. 첫 결과 JSON SHA-256 `3784A44CC1C0779E7F51D783360919CBDAE17D47631B3C6A18622A956011CD5A`, 로그 SHA-256 `9DEA1F0C823E9A9441987333C3959F65A0B9810349105DF5B6619BEF26F8B99A`는 불변이다.
- 기준: v2 501행 JSON SHA-256 `23F3060352AD230EE86FE480560A0591D72EC41ABCA18C421610257AEBF206FE`; Unity `6000.6.0f1` 개정 `f7f8ed4d1e24`. 부모 영향 `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`. 이 재현의 고유 ID는 `REQ-PLAT-035..037`, `AC-PLAT-033..035`다.

## 분리 방법과 금지 범위

첫 복사본의 깊은 경로는 보존하고 수정하지 않는다. 원본 프로젝트의 501 실물 SHA를 다시 전수 확인한 뒤, **새** `output/i501-r2` 프로젝트 경로가 현재 비어 있음을 확인해 만든다. 이 경로는 작업공간 안이지만 원본 프로젝트의 `Assets`·`Packages`·`ProjectSettings` 바깥이며 501 정확 파일 경로에 속하지 않는다. 정확한 501파일만 새 프로젝트에 복사하고 `Library`, `.git`, 후보 밖 파일 또는 첫 진단 생성물은 복사하지 않는다. 새 프로젝트의 실행 전 501 SHA와 후보 밖 초기 복사 0을 확인한다. 기존에 열린 Unity 프로세스가 있으면 실행하지 않는다.

짧은 경로의 새 복사본에서만 Unity를 한 번 `-batchmode -nographics -quit -projectPath <새 복사본> -logFile <새 로그>`로 실행한다. 시험·API 자동 갱신·패키지 수동 설치·설정 변경·원본 코드/설정/자산/QA·Git/원격 변경은 금지한다. 로그의 실제 시작/끝 UTC·편집기 개정·종료 코드·시간 초과·라이선스·패키지/컴파일/자산 가져오기 오류를 기록한다. 첫 로그와 **정확 문자열/원문 위치/오류 종류**로 대조하되, 오류가 사라져도 경로가 유일 원인이었다고 단정하지 않는다. `Renderer2D` 일곱 GUID는 실제 해석 진단이 없으면 미해석으로 유지한다. 실행 실패나 새 오류가 나도 재시도하지 않는다.

원본 전후와 복사본 실행 전후 501 SHA, 복사본 `manifest`/`lock` 전후 SHA와 해석 버전, 로그 전체 SHA, 생성물의 수량·범위·오류를 기록한다. 복사본은 검토를 위해 보존하고 이 작업에서 재귀 삭제하지 않는다. 허용 출력은 새 복사본 및 그 생성물, `artifacts/c4-initial-publication-short-path-unity-recheck-v1.json`, 같은 이름 `.md`·`.log`, `docs/verification/2026-10-01-initial-code-publication-short-path-unity-recheck-terra-report.md`뿐이다. 전체와 모든 결과 행의 `PublicationApproved=false`, `DependencyClosure=false`, `WholeAccepted=false`, `LunaReviewed=false`, `CleanCloneExecuted=false`를 유지한다.

| 요구 | 인수 기준 |
| --- | --- |
| `REQ-PLAT-035` — 같은 501 바이트를 더 짧은 격리 경로에 복사한다. | `AC-PLAT-033`: 원본 사전/사후·새 복사본 실행 전/후의 501경로/SHA가 모두 501/501 일치하고 초기 후보 밖 복사 0이다. 첫 복사본·첫 원장·첫 로그의 SHA는 그대로다. |
| `REQ-PLAT-036` — 한 번의 재현에서 오류의 변화와 남은 오류를 정확 기록한다. | `AC-PLAT-034`: 실제 편집기·인수·시작/끝 UTC·종료 코드·로그 SHA, 첫/새 로그의 패키지 자산 접근·가져오기 실패·C# 컴파일·형식 오류·통신/라이선스 진단을 각각 정확 수량과 줄로 대조한다. 패키지 해석 버전과 `manifest`/`lock` 전후를 기록하고, 새 오류를 성공으로 숨기지 않는다. |
| `REQ-PLAT-037` — 환경 진단과 게시 수용을 분리한다. | `AC-PLAT-035`: 새 JSON·판독 문서·보고의 결과가 일치하고, 새 실행이 오류 없이 끝나더라도 Git 깨끗한 복제·501 내용 폐쇄·독립 검수·제품 시험을 수행한 것으로 쓰지 않는다. 다섯 금지 플래그는 모두 거짓이다. |

테라 역할(`gpt-6-sol`)이 이 재현과 네 출력을 작성한다. 루나 독립 검수와 아스트라 결과 수용은 이후 별도다. 사용자 원본 프로젝트는 읽기 대상만이며, 새 복사본은 작업공간 안의 독립 하위 프로젝트로서 Git 깨끗한 복제가 아니다.
