# C4 최종 큐 계획 v13 독립 검수

검토 대상 계획은 `artifacts/c4-final-validation-queue-plan-v13.json`(SHA-256 `04CE5EB81B5D30BC03B29AAD574832043273D150ADDEA34505B408423930B766`)이다. Approved 계약 `docs/specs/work-contracts/2026-09-30-c4-play-r2-three-row-source-alignment-amendment.md`(SHA-256 `AA9FDB5A6055991E9726891167EE16209578862E3178C587CB44903E8505ADCD`) 및 직전 계획 v12와 정적으로 대조했다.

v12에서 바뀐 상위 결속은 v17 source manifest `E3D8626C7DA9F058D228022ED357110A7FF6B0A14A05A15B88FC4FA8C8E668D6`, v17 input `DF6B55D8C9D2620D6AB20013B14A1F45B9CB0843DD4DA631F5F503F04AD2FA2D`, rows v9 `2769FA04AF42317D116C9C28D26EDA7948B58ECB598778EC9FE204538EB07EDE`, 큐 도구 `60BA312E15CF8F0823AA47F281F789BC1D8DB64C47939EED06AACBEBC6E64446`의 네 지문이다. 첫 두 실행은 각각 `c4-r9-focused-edit`와 `c4-r3-focused-play`로 바뀌었고 `NativeWitnessRunStem`도 해당 실행과 일치한다. 각 실행의 8개 출력 경로만 새 이름으로 바뀌었다. 선택 파일, 이름·순서·선택식·예상 수, 행 ID, 실행 제한은 두 실행 모두 v12와 동일하다. 나머지 7개 실행은 계획 JSON상 변경되지 않았다.

9개 선택을 현재 파일과 대조한 결과 선택 지문, 모든 `SourceFiles`와 `ParserEvidence.EnumSources`, 10개 도구 지문이 일치했다. 계획의 예상 이름 1,654개는 선택 파일의 전체 이름과 각각 일치하고 전체 중복이 없다. v17 원장의 171개 파일은 모두 1,060개 입력 경로에 포함된다. rows v9에는 188개 행이 있다. 계획에는 9개 실행, 총 72개 출력 경로가 있으며 출력 경로 중복이나 기존 출력이 없고 `c4-final-validation-queue-result-v11.json`도 아직 없다. 최대 명령줄 길이는 24,161자로 32,767자 제한 아래다. 계획은 `WholeAccepted=false`이며 추적에 `REQ-M5D7QC4-007`, `AC-M5D7QC4-009`, `AC-M5D7QC4-010`을 포함한다.

## 판정

P0 0, P1 0. 계획의 정적 결속과 실행 전 출력 안전 조건에서 차단 결함을 찾지 못했다. 이 검수는 계획 정합성만 확인했으며 큐 또는 Unity 실행 결과나 C4 전체 수용을 판정하지 않는다.
