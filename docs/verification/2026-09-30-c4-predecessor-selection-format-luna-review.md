# C4 선행 선택 형식 개정 독립 검수

- 검수 대상: `docs/specs/work-contracts/2026-09-29-c4-predecessor-selection-format-amendment.md`
- 검수 대상 SHA-256: `DFBF92EB722CEEEFBD8A16F85338B776F01C2AC692C4DA058FB6BC79F582B754`
- 검수: 실제 `gpt-6-luna`, 2026-09-30. 읽기 전용 정적 대조이며 실행·승인이 아니다.
- 판정: 설계 P0=0, P1=0. 개정은 Draft 상태이므로 이 검수만으로 예외 분기 구현·수용 권한이 생기지 않는다.

## 원본 여섯 선택 대조

각 실제 JSON의 현재 SHA, 최상위 키 순서/집합, `ExpectedCount`, 실제 이름 배열 개수를 대조했다. 여섯 모두 제안서의 경로·SHA·이름 수·형식과 일치한다.

| 경로 | 현재 SHA-256 | 실제 키 형식 | 예상/실제 이름 수 |
| --- | --- | --- | ---: |
| `artifacts/c3-r11-edit-matrix-selection.json` | `B04192B8627B3BC03F934B102ADA35D3CD095757C88DC4ED2AD9FD476F2ACBED` | A, 6키 | 91/91 |
| `artifacts/c3-r11-edit-remaining-selection.json` | `F8CCD04044B67A76E3033CD3D6A5A169B98BA5B25107801B26C667520B125EAF` | A, 6키 | 149/149 |
| `artifacts/c3-r11-playmode-focused-selection.json` | `C63B2946E83FF4F4862197E8C5EDFDC25B488C3AACC5C0628F612553A485CA01` | A, 6키 | 15/15 |
| `artifacts/c3l-edit-regression-selection.json` | `FBD1B5A6FE6AD6E96CA5CDAD4103C9DFA129B3F5F622859E11D0B43FD4D649F8` | B, 4키 | 562/562 |
| `artifacts/c2-r36-selection-preflight.json` | `B2CF18E155D98BA7ECAC1E8CABE5EAF90CF6C48CC0419D1ECC800758220120E3` | C, 11키 | 51/51 |
| `artifacts/c3l-play-regression-selection.json` | `FFFF443666F58B8518A6E89C206AF8BF23A0EF559888AAC8D528E3111591FB9E` | B, 4키 | 610/610 |

Worker51의 원래 키는 11개이며 원본은 `EditMode` 실행으로 귀속된다. 제안서는 실제 고정 큐 배분과 원시 실행 기록을 근거로 이 플랫폼을 명시하며, 12키라는 이전 요약을 반복하지 않는다. 나머지 플랫폼도 A의 원본 필드와 B/C의 고정 지도 규칙으로 구분한다.

정확 경로와 SHA를 함께 고정하고, 계획의 Platform·PartId·지도 ID·원본 Parts.Selection을 모두 결속하는 구조는 이름 모양만으로 legacy/new 형식을 선택하는 우회를 막는다. A/B/C 키 집합, 정수 필드의 문자열화 거절, 이름 순서·Selector·Count 일치, 원본 전체 선택과 91/149 부분 배열 각각의 순서 보존 및 서로소 합집합 요구도 기존 선택을 새 선택으로 재발급하지 않으면서 명확하다. 새 focused 형식은 계속 QA r2 규약을 따르며 이 예외 밖의 파일은 legacy fallback 대상이 아니다.

## 승인 경계와 구현 상태

QA r2는 신규 선택의 정확 스키마를 규범화한다. 따라서 여섯 기존 선택의 대체 형식을 허용하는 이 문서의 우선권은 Astra의 별도 Approved 전환 뒤에만 적용된다. 현재 `artifacts/c4-verify-required-rows.ps1`에는 여섯 경로·SHA 표와 A/B/C 키 집합 분기가 이미 있지만, 해당 코드는 이 Draft에 대한 승인이나 검증 결과가 아니다. 특히 실제 분기에서 B/C의 고정 플랫폼을 강제하는지 포함해 구현은 별도 독립 검수가 필요하다. 현재 Draft 자체는 고정 플랫폼 규칙을 명시하므로 설계 결함으로 분류하지 않았다.

P0/P1 설계 차단은 없다. 별도 Approved 전환, 실제 도구의 정확 여섯 경로·키·형식·플랫폼·계획 지도 검증, 최종 입력·소스 동결 및 독립 사전 검수는 실행 전 조건으로 남는다. Unity 실행과 실제 결과 수용은 이 검수 범위가 아니다.
