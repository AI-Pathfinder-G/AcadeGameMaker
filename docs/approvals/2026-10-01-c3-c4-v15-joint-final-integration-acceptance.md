# C3·C4 v15 공동 최종 통합 수용

- 결정일: 2026-10-01
- 결정자: 아스트라 (`gpt-6-astra`)
- 상태: **Verified — 현재 v19 동결 소스의 C3·C4 승인 계약 범위**
- 추적: `REQ-M5D7QC3-001..007`, `REQ-M5D7QC4-001..007`; `AC-M5D7QC3-007/008`, `AC-M5D7QC4-001..010`.

## 판정

C3 선행 단계의 [기존 수용](2026-09-29-c3-r11-pre-c4-integration-acceptance.md)에서 열어 둔 `AC-M5D7QC3-007/008`을 현재 C4 실제 실행 연결 증거와 함께 **Verified**로 닫는다. `AC-M5D7QC4-001..010`도 현재 v19 소스와 v15 실행 계획에 한하여 **Verified**로 수용한다. 이 문서가 아스트라의 별도 최종 판정이며, 실행 도구의 `WholeAccepted=false` 원본을 수정하거나 소급해 참으로 해석하지 않는다.

`AC-M5D7QC3-007`의 Busy 이전·이후 재시도 범위와 ConfirmationStale 소비, Reload/Manual 및 barrier 이후의 취소·재무장·동일 세션 재시도 금지는 실제 C1/C2 결과와 필수 행으로 검증됐다. `AC-M5D7QC3-008`의 커밋 전 권한 폐쇄 및 예외·종료 후 권한 비재생은 소유자 시험과 C4 양측 가드·부분 진입·종료 행으로 검증됐다. C3 소유자 관련 다섯 시험은 모두 통과했다. 과거의 합성 단계 부분 결과만으로 두 기준을 닫은 것이 아니다.

C4의 실제 identity·proof 결속과 C1/C2 호출 횟수, Busy·Stale·Reload·Manual·Completed·예외 분기, durable checkpoint, 실패 시 발행 차단, 종료·재진입·경합을 188개 필수 행과 집중 시험으로 대조했다. `AC-M5D7QC4-009`의 권한·API 제한은 [현재 핵심 정적 검수](../verification/2026-10-01-c4-v19-core-static-luna-review.md)와 동일 지문인 나머지 파일의 [선행 정적 검수](../verification/2026-09-30-c4-runtime-eight-luna-prereview.md)를 함께 근거로 삼는다. 이는 해당 정적 경계의 수용이며 게임 전체 기능이나 원격 코드 게시 승인이 아니다.

## 정확한 실행 근거

- 계획 `artifacts/c4-final-validation-queue-plan-v15.json` SHA-256 `43125A3558F6B1F3FDDC595FB47F602883B4861D0EF24690F1FEE27083EC9082`.
- 소스 원장 v19 SHA-256 `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`의 193파일, 입력 목록 v19 SHA-256 `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C`의 1084경로.
- 최종 결과 `artifacts/c4-final-validation-queue-result-v13.json` SHA-256 `56F3931E7128218F1E4A57AA22CBDC9D6852A2F3E1354F78E7BEAEAF8D3C830A`.
- 9회 실행의 시험 건수 합계 1654/1654 통과. 실패·건너뜀·판정보류, 예상 이름 누락·추가·중복은 모두 0이다. 실행들은 서로 겹치므로 1654를 고유 시험 수로 표시하지 않는다.
- C4 집중 편집·실행·허브 행 148/148, 35/35, 5/5로 합계 188/188. 선행 C3 편집 행렬 내부 행 377/377. 다른 선택의 C4 필수 행 계획은 0이며, Play15의 별도 C5 진단 경계는 별도 검증기에 의해 일치했다.
- 매 실행의 실제 Unity·QA 도구·외부 종료값은 각각 0이고, 1084개 입력 경로와 SHA는 실행 전후 및 실행 간 동일하다. 원시 XML·행 비교·검증 결과의 지문이 계획과 맞는다.
- [테라 결과 대조](../verification/2026-10-01-c4-final-v15-qa-results-terra-report.md) SHA-256 `8517DC2283938CE3FADB4CA2CC1C03D5B964AC78CC9F1CEE1D09FD945617D583`, [루나 독립 검토](../verification/2026-10-01-c3-c4-v15-final-independent-luna-review.md) SHA-256 `EFF7385E6591CE61238C4EBA0E0452E61F397B170ECFD2A11D1593B475B49965`; 독립 검토 P0/P1=0/0.

## 게시와 후속 범위

이 결정은 로컬 동결 소스와 해당 승인 계약의 실행·정적 증거를 수용한다. 최초 게임 코드 게시의 파일별 바이트 출처 428개 공백, 기존 C4 변경 이력의 실제 바이트 결손, 새 원격 원본 대조, 깨끗한 복제에서의 Unity 검증, 별도 파일별 게시 허용 목록은 열려 있다. 게임 코드·시험·메타·자산·설정·QA 도구를 이 결정만으로 원격에 올리지 않는다. 원시 실행 자료는 로컬에 보존하고, 문서 게시에는 지문과 결과만 싣는다.
