# C4 0행 복구 최종 큐 계획 v14 독립 검수

검토 대상 `artifacts/c4-final-validation-queue-plan-v14.json`의 SHA-256은 `8478870AD578FBC2E1D9CCC72895B78FF2756B535F65808D99BC77D77E1DFE6C`다. Approved 계약은 0행 배열 복구 `94E8B2F782C70DFE00E3299690B590BDDCBF181F20B68240DAADA8292252C59D`와 예상 밖 ID 보고 후속 `7EFA4CCCE58378125531FB47E13840C527411C75793EEFE59B511D521CCF11F9`다.

## 정적 대조

- v13 대비 상위 변경은 v18 source manifest `637CB631D8F25F630139F26A2B4AEA552D64F8E64EBA7526AAB3825079BC4569`, v18 input `69DE040ACA02BB02F454B45D09AF418E942278E395E366E54BFFE60AA54F0ABC`, 그리고 verifier `A947C9C6D51C94D177F8A8268B4930607C1669232602B0F4C1C6E960FA16D9DC`·queue `13484AAA0C5CAF60FFA53BB674D02B3F79B0BEF3055BDC91950C63F9D44455E7`를 포함하는 도구 지문이다. rows v9 `2769FA04AF42317D116C9C28D26EDA7948B58ECB598778EC9FE204538EB07EDE`는 유지됐다.
- 9개 선택은 순서대로 139/32/5/91/149/15/562/51/610개 사례이며 모든 `NativeWitnessRunStem`이 실행 stem과 같다. v13 대비 첫 5개 실행의 stem과 대응 8개 출력 경로, 명령줄 길이만 새 값으로 바뀌었다: `c4-r10-focused-edit`, `c4-r4-focused-play`, `c4-r2-focused-hub`, `c4-r2-edit-matrix91`, `c4-r2-edit-remaining149`. 나머지 4개 실행의 계획 객체는 v13과 동일하다. C4 행 배분은 148+35+5=188개이며 `Matrix91`의 별도 377행 특례와 뒤 실행의 0개 C4 행 선택도 유지된다.
- 9개 선택 파일의 지문, 전체 정식 이름 및 선택식, 모든 `SourceFiles`·`ParserEvidence.EnumSources`의 현재 지문이 일치한다. 10개 도구도 원장과 현재 파일이 일치한다. manifest의 186개 파일은 모두 input 1,076개 경로 안에 있다.
- 출력은 9×8=72개이고 중복이 없으며 현재 모두 부재한다. queue result v12도 부재한다. 최대 명령줄 길이는 24,167자로 32,767자 제한보다 작다. 큐 잠금과 `FailStop`, 사례 제한 180초, 관찰 제한 21,600초, `WholeAccepted=false`가 유지된다.
- 기존 v13 계획과 queue result v11의 지문은 바뀌지 않았다. 이미 출력된 첫 5개 v13 실행 산출물은 기존 경로에 보존되고 v14의 새 출력 경로는 비어 있다.

## 판정

P0 0, P1 0. v14 계획의 선택·도구·동결 자료 결속과 출력 분리가 승인 범위에 맞는다. 계획은 실행되지 않았고 이 검토는 AC-M5D7QC4-009/010의 실행 결과, Unity 성공 또는 C4 전체 수용을 뜻하지 않는다.
