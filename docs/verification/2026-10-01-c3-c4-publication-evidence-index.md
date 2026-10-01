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
| 필수 실행 모드 610 | 610/610 통과, C4 기대 행 0 | [최종 독립 검수](2026-10-01-c3-c4-v15-final-independent-luna-review.md) |

아홉 실행의 시험 건수 합계 1654/1654가 통과했다. 서로 겹치는 회귀 선택을 합친 값이므로 고유 시험 수는 아니다. 원시 XML의 실패·건너뜀·판정보류, 실제 Unity·QA 도구·바깥 종료값, 실행 전후 입력 지문 차이는 모두 0이다. C4 행 188/188 및 선행 C3 내부 행 377/377이 맞는다. [테라 결과 대조](2026-10-01-c4-final-v15-qa-results-terra-report.md)와 [루나 최종 독립 검수](2026-10-01-c3-c4-v15-final-independent-luna-review.md)는 실행 결과의 근거를 보존한다. 아스트라의 [C3·C4 공동 최종 수용](../approvals/2026-10-01-c3-c4-v15-joint-final-integration-acceptance.md)은 현재 v19 소스의 `AC-M5D7QC3-007/008`, `AC-M5D7QC4-001..010`을 명시적으로 닫았다. 실행 도구 원본의 `WholeAccepted=false`는 수정하지 않았다.

## 최초 게임 코드 게시의 별도 문턱

[최초 게시 권한 색인 제한 수용](../approvals/2026-09-29-c3-initial-publication-gap-authority-limited-acceptance.md)은 후보 489개 중 직접 바이트 근거 61개와 공백 428개를 보존한다. [파일별 바이트 근거 폐쇄 계약](../specs/work-contracts/2026-10-01-initial-code-publication-byte-closure.md)은 읽기 전용 원장 작성만 Approved다. 이후 [원장 독립 검토](2026-10-01-initial-code-publication-byte-closure-luna-result-review.md) P0/P1=0과 [아스트라 제한 수용](../approvals/2026-10-01-initial-code-publication-byte-closure-limited-acceptance.md)에 따라 초기 489개·C4 신규 12개의 정확 경로를 분류했다. 현재 정확 게시 허용 목록은 비어 있고 501개 전부 제외 상태다. 게임 코드·시험·메타·자산·설정·QA 도구는 이번 문서 게시 대상이 아니다. 근거 공백 폐쇄, 새 원격 대조, 깨끗한 복제의 실제 Unity 검증과 별도 통합 승인이 남아 있다.

이후 [C4 변경 22개 파일 대조](2026-10-01-c4-changed-file-evidence-closure-luna-result-review.md), [새 메타 6개 바이트 대조](2026-10-01-c4-meta6-exact-byte-evidence-luna-result-review.md), [기존 변경 10개 이력 대조](2026-10-01-c4-existing10-change-chain-luna-review.md)를 각각 독립 검토해 제한 수용했다. 현재 바이트·메타 GUID·어셈블리 연결의 일부 근거는 보강됐으나 기존 10개 파일에 대해 v1..v19의 14개 서로 다른 지문 전이를 재구성할 과거 구현 바이트는 없다. 이력 공백을 지우거나 원격 게시 허용으로 바꾸지 않았다. [향후 출처 처리 제안](../proposals/2026-10-01-initial-code-publication-forward-provenance-decision-draft.md)은 결정을 기다리는 비규범 문서다.

이 게시에서는 역사적 원시 증거의 지문·결과를 문서에 남기고, 로컬 실행 경로와 명령 정보가 포함된 원본 자료는 공개하지 않는다. 문서의 파일 경로 표기는 게시 파일 링크가 아니라 로컬 원본의 식별자다. [게시 바이트 대조](2026-10-01-c3-c4-final-publication-byte-map.md)는 원본 지문과 게시본 지문이 다른 문서를 추적한다. 위키는 이 색인을 안내하는 보기이며 승인 명세의 상태를 바꾸지 않는다.
