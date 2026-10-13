# C4 QA 증거 규약 독립 검토

검토 대상은 Draft 규약 `docs/specs/work-contracts/2026-09-29-c4-qa-evidence-protocol.md`이며 SHA-256은 `A883D0AF0FAC7D62BA5840676EB09377EC9F59CADAAC3F7DE770F248F932BCB1`이다. 별도 소유인 C4 R4 계약은 `EF54AE99EEF6E482C267C949A0C1B53709FF19611E9791300E07764EBD9256C0`이다. 이 문서는 두 계약의 테스트 실행·구현 승인이나 C4 수용을 뜻하지 않는다.

## 검토 결과

P0: 0. P1: 1 — 행 비교기가 RequiredFacts를 중첩 증거 객체와 정확히 연결하는 규칙이 부족하다.

규약은 TestContext 출력에 `before/after`, `authorityCorrelation`, `guard`, `barrier`, `history`, `cleanup`을 별도 필드로 둔다(51–55행). ExpectedRows에는 RequiredFacts와 ExpectedCallCounts가 있으나 해당 중첩 객체의 기대값은 별도 항목으로 없다(57행). RequiredFacts는 `{Name, ExpectedValue, RequiredCertainty}`를 실제 Fact와 대응하라고 규정하지만, Fact 이름의 고유성·정확한 개수, 어느 경로의 중첩 필드를 대표하는지, 실제 nested value와 일치해야 하는지까지 정의하지 않는다(115행). 따라서 RequiredFacts에 정상처럼 보이는 사실 레코드가 있어도 실제 `guard`, `barrier`, `history`, `cleanup` 또는 `authorityCorrelation`은 Unknown/미검증일 수 있는 해석 여지가 남는다. 117–119행의 `checked=true`, Unknown 금지 및 전체 상관 검증 원칙은 이를 보강하지만, RequiredFacts만 확인하는 구현이 중첩 필드까지 검증했는지 판별할 정확 규칙으로 연결되지는 않는다.

P1을 닫으려면 규약에서 둘 중 하나를 고정해야 한다. (1) RequiredFacts 이름을 중첩 객체의 정규 경로에 일대일로 매핑하고, 이름 중복·누락·예상 밖 사실을 거부하며, 필요한 각 경로의 값·certainty와 실제 nested 필드 일치를 필수화한다. 또는 (2) ExpectedRows에 해당 nested 객체의 정확 기대값을 추가하고 실제 객체와 구조·값을 재귀 비교한다. 어느 방식을 택하든 필요한 authority correlation은 실제 참조와 `checked=true`, 필수 guard/barrier/history/cleanup 값은 Unknown이 아닌 기대값과 실제 값의 일치를 요구해야 한다. 이 보정은 기존 증거 분류나 행 의미를 넓히지 않고 검사 가능성을 명확히 해야 한다.

그 외 규약은 정확한 선택 fullname 파싱, 불변 입력·소스 결속, mutex 단일 실행, runner의 실제 종료 관측 후 queue 단일 `CreateNew` 최종 반환 작성, 그 파일 종료 뒤 검증기 호출, 부모 NUnit 결과가 Passed가 아니면 내부 행을 수용하지 않는 순서를 명시한다. 따라서 이전 QA 설계 검토에서 남은 이름/중복과 종료 반환 순서의 P1은 규범 문구상 폐쇄 가능하다. TestContext 출력과 실제 XML의 보존 여부, NUnit fullname의 실제 지원은 규약이 기존 근거를 언급하더라도 새 구현·실행으로 확인되지 않았다.

검토는 규약 텍스트와 기존 R11 증거 경계만 대상으로 했다. 기존 QA 도구 `qa/tools/Invoke-UnityQa.ps1`의 기준 SHA는 `A4DD378FF02105345B63B40C6F446B304B04F9E8B894CCD2218806F42DD7DA85`, 참조한 기존 Play XML SHA는 `F69B7F15B4C826F96CF3E68727443F45F6D4D4D4A234F72B3CB0688D26D7830A`다. 새 source, emitter, generator, verifier 또는 Unity 실행은 검증하지 않았고 어떤 결과도 생성·수용하지 않았다. 이 프로토콜은 Draft이며 P1이 닫히고 아스트라 승인이 있기 전에는 구현 기준으로 사용할 수 없다.
