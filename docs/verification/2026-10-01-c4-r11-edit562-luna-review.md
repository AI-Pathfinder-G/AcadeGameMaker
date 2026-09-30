# R11 필수 Edit 562개 독립 실행 결과

- 검토 범위: v15 큐의 일곱 번째 실행 `c4-edit562`만 확인했다. 실행 검증은 AC-M5D7QC4-010, 공동 회귀 범위는 AC-M5D7QC3-007/008이다.
- 실제 XML `artifacts/c4-edit562.xml` SHA `ADF033B4069CDFF51467332D8A7504BA7FB4A53C3E9DBE594DCFAF3033A00ECA`에는 계획된 시험 562개가 정확히 한 번씩 있고 전부 Passed였다. 실패·건너뜀·미분류·누락·초과·중복은 0이다.
- 이 회귀 묶음에 C4 필수 행은 0개다. `artifacts/c4-edit562-required-row-comparison.json` SHA `1E529BD72FB23587AF50F97A6F4E69BFA91D7B27B259D8FA85C78929C92F646F`에서 계획·도달·통과·실패 및 누락·예상 밖·잘못된 ID가 모두 0이고 `SourceMatched=true`, `EvidenceMatched=true`다. XML 경로와 SHA에 결속했다.
- 실제 종료 자료: native 관측 `artifacts/c4-edit562-native-exit-observation.json` SHA `545B26736940A2FE5555B4B49E7F114E96BEEAD2EBEA506D33827A23BD8BE3E3`의 실제 종료 코드 0, QA 반환 `artifacts/c4-edit562-qa-tool-return.json` SHA `1176812AD5CC00FD39249E2F5908DDB32DC9BA0D077C890308FFCF52DFA518FB`의 QA·외부 종료 코드 0이다. `artifacts/c4-edit562-verification.json` SHA `0AD870B9538995129577EFEDE3500845EB13707F48A22AC1C07CA22583E8710E`는 `Verified=true`, 입력/동결 차이 0, `WholeAccepted=false`를 기록한다.
- 전후 입력 스냅샷 SHA는 각각 `93DE764576AFCDC48108C1CF4AC434C6A303EABF758CC4A6BE0D0814972B1751`, `737FA67B0A79FD481F9B623AA9069EB9FCCE5312C3FD84D5FF1F8D2AF59F3FD7`다. 각 1084개 경로의 파일 SHA를 직접 비교했으며 차이는 0이다.
- 판정: 이 필수 회귀 묶음은 AC-M5D7QC4-010 실행 조건을 통과했다. 다음 Worker51 실행이 진행 중이므로 이 기록은 한 묶음의 부분 결과다. 전체 C4 및 통합 수용은 판정하지 않았다.