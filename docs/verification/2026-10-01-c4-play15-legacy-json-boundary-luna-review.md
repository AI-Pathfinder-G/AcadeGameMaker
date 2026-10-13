# Play15 선행 진단 JSON 경계 설계 검수

검수 대상 제안: `docs/proposals/2026-10-01-c4-play15-legacy-json-boundary-design.md`, SHA-256 `C884CDA9E4D041DC3B25BBDE5EF106769760D5DFFB62C82838364D5209E81E8C`. 제안 상태이며 구현·실행 승인이 아니다.

실패 자료와 대조했다. Play15 XML에는 선택에 포함된 정확한 두 부모 `AC005_SameActualCancelPreservesCompleteObservedFixtureSnapshot(False/True)`가 각각 한 번 있고 둘 다 NUnit 통과다. 두 부모가 내보내는 JSON은 C4 행 스키마가 아니라 `id/status/observedFields/currentMemoryAbsent/wholeGameplaySessionClaimed`의 183개 필드 요약이다. 현재 비교기는 `{"id":` 접두만 보고 행 후보로 읽으며 `schemaVersion` 부재 시 `authorityChecked` 예외만 건너뛰기 때문에 이 둘을 형식 오류로 기록한다. 실제 비교 결과는 C4 행 0개, `InvalidIds` 2개, `EvidenceMatched=false`; 이는 C4 제품 반례가 아니라 선행 진단 객체를 C4 행으로 분류한 도구 실패다.

제안은 범위를 적절히 제한한다. 예외를 `RequiredRowIds` 빈 배열, 계획에 결속된 Play15 선택 경로와 SHA, 정확한 두 전체 NUnit 이름, 동결된 부모 소스 경로와 SHA가 모두 맞을 때만 적용한다. 각 부모에 대응하는 진단 ID·상태·183 필드 수·부재 표지·게임 전체 상태 주장을 확인하고, 각 행이 한 번씩만 나오는지 검증한 뒤 C4 행 집계에서는 제외한다. 다른 `{"id":` 객체는 기존 C4 파싱·엄격 스키마 검증을 통과해야 하므로, 괄호 표시명이나 `schemaVersion` 부재 전반을 허용하는 완화가 아니다. 0행 실행의 예상 밖 정상 C4 행은 `UnexpectedIds`, 잘못된 C4 객체는 `InvalidIds`로 계속 실패해야 한다. 이는 기존 0행 검증 규칙과 맞는다.

검토한 선택 SHA `C63B2946E83FF4F4862197E8C5EDFDC25B488C3AACC5C0628F612553A485CA01`, 부모 소스 SHA `2FA406515CEB7C885A5F41EC6438ACC85123714AB00302B520A58B36066C9E23`, 실제 XML SHA `680F8DEAFC605F10392E1972F1FDAF56242C7F7FDF1746937C8E2E4CC2875843`, 행 비교 SHA `55AB34E4CF7CB33A30EDA883521DF38E2B62CE17647DDE3D1C58D8EC553C4705`를 확인했다. XML은 15/15 통과, native·QA·외부 종료 코드는 0이지만 행 비교는 실패하고 전체 수용은 거짓이다. 과거 XML·도구·선택·원장은 수정하거나 재실행하지 않았다.

필수 구현 회귀는 제안대로 정확한 두 정상 진단만 수용하고, 각 진단의 값 변경·상호 교환·누락·중복·잘못된 부모 귀속을 거부하는 것이다. 추가로 예외가 적용된 부모 안에 형식이 완전한 C4 행이 섞이면 이를 건너뛰지 않고 예상 밖 행으로 실패시켜야 한다. 이는 제안의 “분기에 들지 않은 모든 JSON은 기존 파서로 처리”를 구현에서 명시적으로 보장해야 하는 세부 조건이다. Remaining149 0행, Play35행, Matrix91 377행 회귀도 유지한다.

**후속 지적:** 큐 결과 경로 제한을 놓쳤으므로 위 최초 설계 판정의 P1 0건은 정정한다. 현재 `artifacts/c4-final-validation-queue.ps1`(SHA-256 `13484AAA0C5CAF60FFA53BB674D02B3F79B0BEF3055BDC91950C63F9D44455E7`) 11행은 결과 경로를 `artifacts/c4-final-validation-queue-result-v12.json`로 고정하고, 해당 v12 결과 파일이 이미 존재한다. 제안은 새 큐 결과 경로를 발급한다고 하지만 구현 허용 파일을 verifier 한 개로 한정한다. 따라서 후속 전체 큐를 실행하려면 이 한정과 양립하지 않는 경로가 된다. 검증기 예외 설계는 유지할 수 있으나, 새 결과 경로를 허용하는 별도 승인된 queue 한 줄 변경/새 원장·입력·계획 결속이 필요하다. 이 큐 결속 결함은 P1이다.

초기 검토 시 XML 표시명/보조 행 분석에서 이 큐 출력 경로 의존성을 누락했다. P0 0건, P1 1건(큐 결과 경로 결속)으로 정정한다. 정적 설계 검토만 수행했으며 도구 변경, Unity 재실행 및 수용 판정은 하지 않았다.
