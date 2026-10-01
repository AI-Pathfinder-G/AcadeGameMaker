# C4 v15 최종 순차 QA 결과: 테라 대조 기록

Approved C4 QA 증거 규약에 따라 큐 결과와 아홉 실행의 계획·XML·검증·행 비교·종료 관측·입력 포착을 읽기 전용으로 대조했다. 이 기록은 실행 증거의 자체 재확인이며 루나 독립 검수나 아스트라의 전체 수용은 아니다.

- 큐 결과: `artifacts/c4-final-validation-queue-result-v13.json` SHA-256 `56F3931E7128218F1E4A57AA22CBDC9D6852A2F3E1354F78E7BEAEAF8D3C830A`.
- 계획: `artifacts/c4-final-validation-queue-plan-v15.json` SHA-256 `43125A3558F6B1F3FDDC595FB47F602883B4861D0EF24690F1FEE27083EC9082`.
- 동결 소스 v19: `artifacts/c4-frozen-source-manifest-v19.json` SHA-256 `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`.
- 동결 입력 v19: `artifacts/c4-frozen-input-paths-v19.json` SHA-256 `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C`; 전후 포착 각 1084개, 아홉 실행의 경로·SHA 배열 일치.

| 실행 | 방식 | 시험 통과/전체 | 행 통과/계획 | 실제 편집기·QA·외부 종료 |
| --- | --- | ---: | ---: | --- |
| `c4-r11-focused-edit` | EditMode | 139/139 | 148/148 | 0/0/0 |
| `c4-r5-focused-play` | PlayMode | 32/32 | 35/35 | 0/0/0 |
| `c4-r3-focused-hub` | EditMode | 5/5 | 5/5 | 0/0/0 |
| `c4-r3-edit-matrix91` | EditMode | 91/91 | 377/377 (기존 377) | 0/0/0 |
| `c4-r3-edit-remaining149` | EditMode | 149/149 | 0/0 | 0/0/0 |
| `c4-r2-play15` | PlayMode | 15/15 | 0/0 | 0/0/0 |
| `c4-edit562` | EditMode | 562/562 | 0/0 | 0/0/0 |
| `c4-worker51` | EditMode | 51/51 | 0/0 | 0/0/0 |
| `c4-play610` | PlayMode | 610/610 | 0/0 | 0/0/0 |

검증 결과: 각 실행의 시험 이름 집합은 계획과 일치한다. XML의 실행 순서는 계획 선언 순서와 동일하다고 주장하지 않는다. 실패·건너뜀·미확정·누락·추가·중복 이름은 0이다. 각 XML 결속 SHA와 행 비교 SHA는 실제 파일과 맞고 아홉 `Verified` 및 행 `EvidenceMatched`가 모두 참이다. 집중 Edit/Play/Hub는 C4 행 148/35/5, Matrix91은 보존된 기존 C3 형식 377/377, 나머지 다섯 실행의 필수 C4 행 계획은 0이다. Play15의 별도 C5 JSON 경계는 해당 검증기의 `EvidenceMatched` 결과에 포함되며 C4 행 수로 합산하지 않았다.

각 실행에서 native 실제 종료값, QA 반환 종료값, 외부 종료값은 독립 기록상 모두 0이다. native 관측은 `Observed`이며 종료 전 attach가 기록됐다. 전후 입력은 각 1084경로에서 경로·SHA가 같고 실행 간에도 동일하다. 큐는 아홉 실행을 완료했으며 중단 stem·실패 이유가 없다. `SameInputs=true`, `ExecutionComparisonPassed=true`, `WholeAccepted=false`를 원문 그대로 유지한다.

추적: REQ-M5D7QC4-001..007, AC-M5D7QC4-001..010 및 공동 AC-M5D7QC3-007/008. 이 자체 대조는 원시 XML 내부 동작의 독립 재실행이나 제품 전체 수용을 의미하지 않는다. 동결 입력·코드·기존 결과·Git·원격은 수정하지 않았다.
