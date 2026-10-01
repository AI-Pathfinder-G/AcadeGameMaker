# 최초 게임 코드 게시 바이트 근거 원장의 제한 수용

2026-10-01, 아스트라, 실제 `gpt-6-astra`.

[Approved 파일별 조회 계약](../specs/work-contracts/2026-10-01-initial-code-publication-byte-closure.md) SHA-256 `98025CC092B945622D26C07334D46D96BD08473C12DE3CC0068577229CE13B7C` 아래 작성한 **읽기 전용 원장의 분류·바이트 대조 결과만 수용한다.** 테라 보고 `docs/verification/2026-10-01-initial-code-publication-byte-closure-terra-report.md` SHA-256 `86867B75787581AF34553230EDAE53C885E76EEE00BEB3A949ADCE71DC8CA9FF`와 [루나 독립 검토](../verification/2026-10-01-initial-code-publication-byte-closure-luna-result-review.md) SHA-256 `9609BCBA56D404D1DBA74890F68CA5D29943ADC0382C713D5B3B9A52F77E2840`, P0/P1=0을 확인했다. 추적은 `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`이다.

정확 파일별 판정의 기준은 원본 `artifacts/c4-initial-publication-byte-closure-v1.json` SHA-256 `E43A4268ABC87A56AD4F4A198D96893135706EB1364ABB33D70BA27741765B17`의 고유 501경로다. 초기 후보489·추가 C4 후보12, 원래 직접 근거61·공백428, 현재 직접 동일60, C4 기존 겹침21·변경10·신규12, 상태 60/24/159/258과 중복 경로·메타 GUID 0을 독립 대조했다. 이 원본의 **정확 허용 목록은 빈 집합**, **정확 제외 목록은 501개 모든 행의 경로**로 수용한다. 모든 행의 `PublicationApproved=false`를 유지한다. 이 판정은 원장이 누락 없이 공백을 드러냈다는 수용이며 개별 파일의 게시 허용이 아니다.

원장에 기록된 원격 `main`은 당시 `4d864484f75939dcf9c93c055d296613cf1ba1dd` 스냅샷이다. 그 기준으로 501개 중 존재7·동일5·상이2를 대조했다. 이후 문서 변경 요청 #4가 병합되어 원격 `main`은 `84f7d2e96530eee1047069f089bb03b7364ad84b`가 됐다. 따라서 위 원격 존재 수치는 현재 게시 시점의 판정으로 재사용하지 않는다. 후속 코드 게시 검토는 새 원격 트리와 모든 선택 파일 지문을 다시 확인해야 한다.

428개 근거 공백, C4 변경·신규 22개, `Renderer2D` GUID 출처, 패키지·설정·자산·조립 참조, Q0의 Git 상태 시험과 깨끗한 복제 조건은 열려 있다. 소스·메타·자산·설정·QA를 이 승인으로 변경하거나 게시하지 않는다. C4 v15 마지막 실행과 C3/C4 공동 최종 수용도 별개다. 정확한 게시 허용 파일이 생기려면 새 Approved 계약, 파일별 독립 검수, 최신 원격 대조, 깨끗한 복제의 실제 Unity 검증 및 아스트라의 별도 게시 승인이 필요하다.
