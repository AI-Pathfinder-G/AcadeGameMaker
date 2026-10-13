# Play15 선행 진단 JSON 경계 설계 재검수

재검수 대상 제안: `docs/proposals/2026-10-01-c4-play15-legacy-json-boundary-design.md`, SHA-256 `21E6D7FF6CD7FFA88C4D2F43F5E2FE25A8D7B5E567A9B4BE16DF92695E6A9419`. 첫 검토 기록은 [기존 검토](2026-10-01-c4-play15-legacy-json-boundary-luna-review.md)에 보존했다.

v14 Play15 실패는 NUnit/XML의 시험 이름을 괄호 때문에 거부한 문제가 아니다. 선택 SHA `C63B2946E83FF4F4862197E8C5EDFDC25B488C3AACC5C0628F612553A485CA01` 및 현재 시험 소스 SHA `2FA406515CEB7C885A5F41EC6438ACC85123714AB00302B520A58B36066C9E23`가 두 C5 진단 부모를 포함한다. 두 부모는 C4 행이 아닌 183필드 요약 JSON을 출력하고, 검증기는 접두 `{"id":`만으로 C4 레코드 처리에 들어가 `schemaVersion`이 없다고 거절했다. XML은 15/15 통과했지만 행 비교는 `InvalidIds=2`, `EvidenceMatched=false`, `WholeAccepted=false`다. 해당 XML SHA는 `680F8DEAFC605F10392E1972F1FDAF56242C7F7FDF1746937C8E2E4CC2875843`, 행 비교 SHA는 `55AB34E4CF7CB33A30EDA883521DF38E2B62CE17647DDE3D1C58D8EC553C4705`다.

새 제안은 앞 검토의 큐 결과 경로 P1을 닫았다. 검증기와 queue 도구 두 파일의 SHA를 새 manifest/input에 고정하고, queue가 새 `result-v13` 경로를 허용하도록 명시했다. 이미 존재하는 `result-v12`와 v14 계획·XML·선택·원장·제품 코드는 보존하며, 후속 계획은 기존 여섯 실행 출력을 재사용하지 않도록 요구한다. 이는 기존 result 경로 상수와 실제 v12 파일 존재에 대응한다.

남은 P1은 예외 진단과 NUnit 결과의 결속 누락이다. 시험 본문은 C5 JSON을 출력한 뒤 재무장 검증을 수행한다. 따라서 정확한 JSON 줄이 출력되어도 이후 assertion이 실패할 수 있다. 제안은 부모 전체 이름과 레코드 값은 고정하지만, XML에서 해당 부모 사례가 정확히 한 개이고 결과가 `Passed`인지 예외 경로에서 요구하지 않는다. 이에 따라 비교기가 실패한 사례의 선행 진단을 받아들일 수 있다. 예외 분기는 정확한 부모 전체 이름·유일성·`result=Passed`를 모두 확인해야 한다. 또한 다섯 키 외 추가 키, 중복 키, 잘못된 형식은 계속 거절해야 한다. 기대하는 정확 레코드 값은 `id` suffix와 부모 인자 대응, `status=passed`, `observedFields=183`, `currentMemoryAbsent=true`, `wholeGameplaySessionClaimed=false`다. `absent/present`는 부모의 `tempPresent` 인자와 ID suffix 대응이며, `currentMemoryAbsent` 값은 두 경우 모두 true다.

나머지 제안 경계는 적절하다. 예외는 빈 `RequiredRowIds`, 고정 Play15 선택과 시험 지문, 정확한 두 사례에 한정된다. 보조 JSON은 C4 `Rows`나 C4 계획·도달·통과 수에 포함하지 않는다. 예외에 속하지 않는 객체는 기존 strict C4 파서로 보내므로 0행 실행의 예상 밖 정상 C4 행과 형식 불량 C4 행은 계속 실패해야 한다. Remaining149 0행, 비영 Play 35행, Matrix91의 377행 회귀도 유지한다.

P0 0건, P1 1건(정확한 부모 NUnit 통과 결속). 설계 정적 검토만 수행했으며 구현·Unity 재실행·수용 판정은 하지 않았다.
