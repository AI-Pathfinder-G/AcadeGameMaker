# C4 선행 선택 원본 형식의 한정 결속

- 상태: **Approved — 정확 원본 선택 형식 분기 구현 승인, 도구·실행 수용 미완료**.
- 승인: 아스트라, 실제 `gpt-6-astra`, 2026-09-30. 독립 검수 `2026-09-30-c4-predecessor-selection-format-luna-review.md` SHA `B27EA96872216350E0EF3CC8E368728BD3E2C07EAD73DBB81F86239DE5CDB071`의 정적 설계 P0/P1=0/0을 확인했다. 검수 원문 SHA `DFBF92EB722CEEEFBD8A16F85338B776F01C2AC692C4DA058FB6BC79F582B754`는 `2026-09-29-c4-predecessor-selection-format-reviewed-draft.md`에 정확 보존한다. 본문의 Draft·승인 대기는 작성 시점의 이력이며 현재 구현 권한은 이 승인 상태를 따른다. 고정 플랫폼을 강제하지 않는 도구 구현 잔여 P1은 별도 보정·독립 검수하며 이 승인으로 도구 구현 또는 실제 실행을 수용하지 않는다.
- 작성: 아스트라, 실제 `gpt-6-astra`, 2026-09-29.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-010`, 공동 `AC-M5D7QC3-007/008`.
- 선행: Approved r4 실행 계약·QA r2·선행 내부 행 출력 개정. 선행 원문은 수정하지 않는다.

QA r2의 신규 선택 스키마는 신규 FocusedEdit/FocusedPlay/FocusedHub에 그대로 적용한다. 선행 지도 Parts.Selection이 요구하는 아래 실제 원본 여섯 파일에는 신규 선택 스키마를 소급 요구하지 않는다. Approved 전환 후 이 문서는 정확 여섯 파일의 원래 키 집합·자료형 결속에만 우선하는 한정 개정이다. 원본을 신규 선택 파일로 복제하거나 스키마를 다시 발급하지 않는다. 관측된 키 모양만으로 버전을 고르는 fallback은 금지한다.

| 고정 지도/부분 | 정확 경로 | 원본 SHA-256 | 이름 수 | 형식 |
| --- | --- | --- | --- | --- |
| Edit240/Matrix91 | `artifacts/c3-r11-edit-matrix-selection.json` | `B04192B8627B3BC03F934B102ADA35D3CD095757C88DC4ED2AD9FD476F2ACBED` | 91 | A |
| Edit240/Remaining149 | `artifacts/c3-r11-edit-remaining-selection.json` | `F8CCD04044B67A76E3033CD3D6A5A169B98BA5B25107801B26C667520B125EAF` | 149 | A |
| Play15 | `artifacts/c3-r11-playmode-focused-selection.json` | `C63B2946E83FF4F4862197E8C5EDFDC25B488C3AACC5C0628F612553A485CA01` | 15 | A |
| Edit562 | `artifacts/c3l-edit-regression-selection.json` | `FBD1B5A6FE6AD6E96CA5CDAD4103C9DFA129B3F5F622859E11D0B43FD4D649F8` | 562 | B |
| Worker51 | `artifacts/c2-r36-selection-preflight.json` | `B2CF18E155D98BA7ECAC1E8CABE5EAF90CF6C48CC0419D1ECC800758220120E3` | 51 | C |
| Play610 | `artifacts/c3l-play-regression-selection.json` | `FFFF443666F58B8518A6E89C206AF8BF23A0EF559888AAC8D528E3111591FB9E` | 610 | B |

정확 키 집합은 다음과 같다. 원문 SHA 확인이 자료형 검사를 면제하지 않는다. 원래 고정 값과 구조는 변경 없이 읽으며 승인 범위 밖 원본 및 새 원장은 기존 규약대로 거절한다.

- A: `Trace,Platform,Selector,ExpectedCount,ExpectedQualifiedNames,SourceFiles`.
- B: `Trace,Selector,ExpectedCount,ExpectedQualifiedNames`.
- C: `Selector,ExpectedCount,ExpectedQualifiedNames,R35RetainedCount,SelectedOtherCount,MatchesFixtureParent,BuildConfigScanRoots,CurrentObservedRepositoryBuildConfigs,ObservedSDK,RequiredWorkerInputs,FreshBeforeAfterRequired`.

판별은 계획의 Platform·PartId·선행 지도 ID와 실제 Parts.Selection 원본 경로·SHA를 모두 확인한 뒤 수행한다. 같은 SHA의 다른 경로나 비슷한 파일명, 임의 ID, 다른 이름 배열은 허용하지 않는다. 이름·원래 배열 순서·원래 Selector·정확 ExpectedCount는 원본과 실행 계획에 각각 같아야 한다. 중복·누락·추가 이름·string 숫자·임의 null은 거절한다. A의 Platform은 실제 계획과 동일해야 하고 B/C의 Platform은 정확 고정 지도 Edit562=EditMode, Worker51=EditMode, Play610=PlayMode에서만 정한다. Worker51의 실제 선행 실행 플랫폼은 원래 `artifacts/c3-r11-final-validation-queue.ps1`의 고정 배분과 원시 실행 증거에서 확인한 EditMode다. C의 부가 키는 원래 증거이며 새 실행의 SDK·빌드 구성 검증을 완료했다고 주장하지 않는다.

전체 Edit240 원본 결속은 `artifacts/c3-r11-editmode-focused-selection.json` SHA `66B62A29BC1626A72D60607CA92DA6C880D09870D16CE0D16201DC117CAC4CF6`를 유지한다. 전체 원본과 각 부분 원본 배열은 각각 원래 순서를 보존하고 91/149 서로소 합집합만 전체240과 비교한다. 나머지 선행 선택의 이름을 임의 파싱·축소하거나 새 C4 행으로 대체하지 않는다. 여섯 파일은 기존 원본 그대로 최종 입력 목록·선행 지도·계획에 결속한다. 모든 실행에는 동일 source/입력·부모 사례·원시 XML·실제 native/QA/outer 종료값의 검증이 필수다.

이 개정은 기존 Approved 새 도구7개의 제한 분기만 허용하며 추가 도구·출력·선택 생성·Unity 실행 권한을 만들지 않는다. 실제 소스·도구·입력·선택·계획 동결과 독립 사전 검수 후 별도 실행 배분을 유지한다. 독립 루나 검수와 아스트라 승인 전에는 이 초안을 새 분기 수용 근거로 삼지 않는다.
