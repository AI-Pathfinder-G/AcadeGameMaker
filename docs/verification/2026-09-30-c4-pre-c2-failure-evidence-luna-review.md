# C4 C2 미호출 실패 증거 개정 독립 검토

- 검토 모델: `gpt-6-luna`.
- 검토 대상: 초안 `docs/specs/work-contracts/2026-09-30-c4-pre-c2-failure-evidence-amendment.md`, SHA-256 `39DBF9B54C6722AD0AAF5705E5EE3FE4E4C2A28DA5373E52B8544BB6A2C95686`.
- 대조 규범: 승인 QA r2 `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`, 발급 세대 개정 `FF88BFC0471FB48C2828765A1C6FBD0F3913F0DD123CDEB16BCBADD4F8F2DCB6`, R4 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`.
- 판정: 정적 규범 P0 0건, P1 0건. 초안은 Draft이며 승인 전 구현·실행 권한이 없다.

## 판정 근거

초안은 `memoryGeneration`에 `C2NotInvokedAfterIssuedExecution`만 추가하고, 이를 정확한 경로와 네 사전 고정된 FullBridge/ActualCallback/ActualWorker 실패 행으로 한정한다. 허용 checkpoint는 `AfterC1Begin`, `BeforeC1ResultValidate`, `AfterC1ResultValidate`, `BeforePreparedProofTake`뿐이다. 각 행에서 실행 세대는 기존 `IssuedWithoutResult` 조건을 전부 유지하고, C1은 1회, C2 및 준비 proof 관찰자·C2 관찰자·fresh 호출은 0회로 고정한다. 실제 미관측 memory 세대와 proof를 null/Unknown으로 남겨 부재·정상 발급으로 오인하지 않게 한다.

이 범위는 기존 QA r2의 제한적 `Unobserved` 의미를 전역 완화하지 않고, 기존 발급 세대 개정의 `IssuedWithoutResult`와 `ReceiptWithoutExecutionResult` 조건도 바꾸지 않는다. C1 지역값의 실제 참조를 꾸며 내거나 약한 CWT를 열거하지 않으며, C2 미호출·proof 미관측·실제 세대 부재를 분리한다. outcome/phase null, `checked=false`, 원본 execution 기록 및 consume/fault 이력, 실행 중단과 actual callback 예외, native 관측 규칙, 기존 가드·장벽·정리·history 조건이 계속 필수다.

정확 checkpoint와 실제 enum 값의 사전 고정, 예외 종류·callback 일치 확인, actual call counter와 source-established 호출 지배의 certainty 구분, 동결 Bridge 입력과 source boundary의 독립 대조를 요구한다. 따라서 새 규칙은 런타임 API·observer·발급 권한이나 손상 seam을 추가하지 않는다. Invalid checkpoint/미도달/다른 예외/consume 전·실제 결과 존재/`checked=true`/non-null generation·proof/추가 C2 또는 observer 호출/진단 누락과 잘못된 certainty를 거절하도록 규정한다.

기존 가드·장벽·cleanup·history 및 나머지 canonical 사실에 Unknown 면제를 확대하지 않는 점도 승인 규범과 일치한다. 특히 C1 결과가 존재한다는 사실을 새 규칙이 typed C1 행 전체 관측이나 정상 proof 검증으로 바꾸지 않는다.

## 한계

검수는 초안 및 선행 승인 문서의 정적 대조만 수행했다. 네 행을 구현하거나 QA 도구에 반영하지 않았고 Unity·컴파일·실제 실행은 하지 않았다. 다음 구현은 Astra의 초안 승인과 정확한 source/fixture/도구 동결, 별도 독립 검수 및 실행 배분 이후에만 허용된다.
