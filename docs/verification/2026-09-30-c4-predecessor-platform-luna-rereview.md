# C4 선행 선택 플랫폼 보정 독립 재검수

- 구현 대상: `artifacts/c4-verify-required-rows.ps1`
- 현재 SHA-256: `A022A2AB896910050570DF0A54266497446738D126C22379835F788E7C55E0CC`
- 기준 규범: Approved `docs/specs/work-contracts/2026-09-29-c4-predecessor-selection-format-amendment.md`, SHA-256 `26C180D05DEAA4DA59515BD891FF94638A5548E259F4BE18EC218EF90087A789`.
- 이전 독립 검수 원문은 `docs/verification/2026-09-30-c4-qa-tools-luna-review.md`, SHA-256 `5CF6508FF4E44837C7BE312F0BFFDB4276BC2A5C0E4D116C64C344AAF9571FAF`로 보존했다.
- 판정: 플랫폼/지도/부분 결속 P0=0, P1=0. 이 판정은 지정 함수의 지역 검사에 한정하며 Unity·runner·queue·실행 수용을 뜻하지 않는다.

## 코드 대조

`C4-RunSelection`의 고정 분기표는 실제 여섯 원본에 대해 다음을 강제한다.

| 정확 원본 경로 | 고정 지도 ID | 플랫폼 | 고정 PartId |
| --- | --- | --- | --- |
| `artifacts/c3-r11-edit-matrix-selection.json` | `Edit240` | `EditMode` | `Matrix91` |
| `artifacts/c3-r11-edit-remaining-selection.json` | `Edit240` | `EditMode` | `Remaining149` |
| `artifacts/c3-r11-playmode-focused-selection.json` | `Play15` | `PlayMode` | 전체 선택 |
| `artifacts/c3l-edit-regression-selection.json` | `Edit562` | `EditMode` | 전체 선택 |
| `artifacts/c2-r36-selection-preflight.json` | `Worker51` | `EditMode` | 전체 선택 |
| `artifacts/c3l-play-regression-selection.json` | `Play610` | `PlayMode` | 전체 선택 |

각 분기는 먼저 실행의 Platform 및 고정 Part를 비교하고, 지정 PartId·정확 원본 경로·원본 SHA에 맞는 단일 지도 소유자가 예상 고정 ID인지 확인한다. 이어 원래 이름 배열과 순서, Selector, Count를 비교한다. Matrix91/Remaining149는 각각 고정 Part를 별도 확인하며 두 부분을 같은 Edit240 원본 지도 아래 유지한다. 여섯 원본 JSON과 이름 배열은 변경하지 않았다.

## 지역 함수 검사

승인 규범에 고정된 여섯 실제 선택 파일을 읽고 SHA를 재확인한 뒤, 지도만 메모리 객체로 제공해 `C4-RunSelection`을 직접 호출했다. 정상 6건이 통과했다. 여섯 건 모두 잘못된 플랫폼 1회와 잘못된 지도 소유 ID 1회를 거절했고, 두 Edit240 부분은 서로의 PartId로 바꾼 경우 각각 거절했다. 결과는 정상 6/6, 잘못된 플랫폼 거절 6/6, 잘못된 지도 거절 6/6, 잘못된 Part 거절 2/2다. runner·queue·Unity는 호출하지 않았고 실행 원장을 쓰지 않았다.

검수 시점 QA 도구 7개 중 이 검증기만 새 SHA를 가졌다. 나머지 6개는 이전 독립 검수에 기록된 현재 지문과 같았다. 이는 선택 분기 보정이 할당 범위에 한정됐다는 확인이며 QA 도구 전체나 실제 프로세스 관측의 수용은 아니다. Approved 형식 규범과 자체 점검 문서의 원문도 수정하지 않았다.

기존 P1은 닫혔다. 최종 비재귀 소스 동결, 독립 사전 검수, 실제 QA/Unity 실행과 원시 증거 대조는 계속 필요하다.
