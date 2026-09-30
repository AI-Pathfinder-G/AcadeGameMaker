# C3·C4 문서 게시 근거 색인

이 색인은 2026-10-01 현재의 문서 게시 범위와 실제 검증 상태를 구분한다. 캐논·결정 기록·Approved 명세의 권한을 대체하지 않는다. 추적 대상은 `REQ-M5D7QC3-001..007`, `REQ-M5D7QC4-001..007`, 공동 `AC-M5D7QC3-007/008` 및 `AC-M5D7QC4-001..010`이다.

## 승인과 역사적 결과

- [C3 R11의 C4 진입 전 선행 수용](../approvals/2026-09-29-c3-r11-pre-c4-integration-acceptance.md)은 당시의 집중 91·149·15와 필수 562·51·610 통과를 수용했다. 이 610건은 당시 C3 소스의 역사적 실행이며 현재 C4 소스의 실행을 대신하지 않는다. 공동 `AC-M5D7QC3-007/008`은 그 기록에서 열려 있다.
- [C4 r4 정확 구현 계약](../specs/work-contracts/2026-09-29-c4-r4-exact-implementation-amendment.md)과 [QA 증거 프로토콜](../specs/work-contracts/2026-09-29-c4-qa-evidence-protocol.md)은 [제한 구현 승인](../approvals/2026-09-29-c4-r4-implementation-contract-approval.md)에 따라 Approved다. 이후 정확 범위의 개정 문서와 독립 검토는 각 문서의 상태·지문을 보존한다. Approved는 실행 통과나 전체 수용이 아니다.
- [현재 핵심 소스 정적 독립 검수](2026-10-01-c4-v19-core-static-luna-review.md)는 v19의 핵심 파일을 대조해 P0/P1=0을 보고했다. 정적 검수만으로 런타임 전체를 판정하지 않는다.

## C4 v15 순차 실행

동결 기준은 `artifacts/c4-frozen-source-manifest-v19.json` SHA-256 `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`의 소스 193파일, `artifacts/c4-frozen-input-paths-v19.json` SHA-256 `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C`의 입력 1084경로, `artifacts/c4-final-validation-queue-plan-v15.json` SHA-256 `43125A3558F6B1F3FDDC595FB47F602883B4861D0EF24690F1FEE27083EC9082`다. 실행 출력은 로컬 원본으로 보존하고 문서 변경 요청에 원시 XML·로그·JSON·실행 스크립트를 일괄 게시하지 않는다.

| 실행 | 현재 확인 | 독립 검토 |
|---|---|---|
| 집중 편집 139, 집중 실행 32, 허브 5 | 각각 139/139, 32/32, 5/5 통과; C4 요구 행 148/35/5 대조 | [첫 세 실행](2026-10-01-c4-v15-first-three-runs-luna-review.md) |
| 기존 C3 편집 행렬 91 | 91/91 통과, 내부 377행 대조 | [행렬 검수](2026-10-01-c3-r11-edit-matrix91-luna-review.md) |
| 기존 C3 편집 나머지 149 | 149/149 통과, C4 기대 행 0 | [나머지 검수](2026-10-01-c4-r11-edit-remaining149-luna-review.md) |
| 기존 C3 실행 집중 15 | 15/15 통과, C4 기대 행 0 | [실행 집중 검수](2026-10-01-c4-r11-play15-luna-review.md) |
| 필수 편집 562 | 562/562 통과, C4 기대 행 0 | [편집 회귀 검수](2026-10-01-c4-r11-edit562-luna-review.md) |
| 필수 작업자 51 | 51/51 통과, C4 기대 행 0 | [작업자 검수](2026-10-01-c4-worker51-luna-result-review.md) |
| 필수 실행 모드 610 | 2026-10-01 현재 실행 중. 완료와 전체 수용을 주장하지 않는다 | 완료 후 별도 검수 |

앞선 여덟 실행에서 원시 XML의 실패·건너뜀·판정보류, 실제 Unity·QA 도구·바깥 종료값, 실행 전후 입력 지문 차이는 모두 0으로 보고됐다. 이는 각 실행의 부분 결과이며 v15 전체 수용은 마지막 실행·순차 결과·루나 전수 검토·아스트라 별도 판정 뒤에만 기록한다.

## 최초 게임 코드 게시의 별도 문턱

[최초 게시 권한 색인 제한 수용](../approvals/2026-09-29-c3-initial-publication-gap-authority-limited-acceptance.md)은 후보 489개 중 직접 바이트 근거 61개와 공백 428개를 보존한다. [파일별 바이트 근거 폐쇄 계약](../specs/work-contracts/2026-10-01-initial-code-publication-byte-closure.md)은 읽기 전용 원장 작성만 Approved다. 게임 코드·시험·메타·자산·설정·QA 도구는 이번 문서 게시 대상이 아니다. 파일별 독립 검수, 정확 게시 허용 목록, 깨끗한 복제의 실제 Unity 검증과 별도 통합 승인이 남아 있다.

이 게시에서는 역사적 원시 증거의 지문·결과를 문서에 남기고, 로컬 실행 경로와 명령 정보가 포함된 원본 자료는 공개하지 않는다. 문서의 파일 경로 표기는 게시 파일 링크가 아니라 로컬 원본의 식별자다. 위키는 이 색인을 안내하는 보기이며 승인 명세의 상태를 바꾸지 않는다.
