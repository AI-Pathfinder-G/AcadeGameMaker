# C4 발급 후 결과 미발급 세대 증거 개정

- 상태: **Approved — 한정 규칙 구현 승인, 실제 실행·통합 수용 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 승인 근거: [독립 r2 재검수](../../verification/2026-09-30-c4-issued-generation-luna-rereview.md) SHA `FC09020E5159BA1E062674308EF757EBCA5DA84244775F936D6E39F16F2C680A`, 정적 설계 P0/P1=0/0. 검수 원문 SHA `AF4051D04CCB629A2C0DE051C36D2258AAAB3E398DBDF014C6FD8481FE7F408E`을 `2026-09-30-c4-issued-generation-evidence-reviewed-draft.md`에 그대로 보존했다. 아래 승인 전·Draft 문장은 검수 원문의 당시 이력이다. 이 상태가 관련 한정 구현 권한이며 runtime/API/손상 범위 확대나 실제 Unity 실행 권한은 아니다.
- 작성: 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 개정: r2. r1 원문 SHA `148E66886B58584B2C0C5AC825FDF4FA1A4991C00F6FF789C53D4601A3EEF3B6`을 `2026-09-30-c4-issued-generation-evidence-r1-draft.md`로 보존했다. 독립 r1 검수 SHA `823FCB3E66FFC98791F9C285A574C53EDAB2708407DC0EE9552CF0042044EAF8`의 원본 session/pair 연결 P1을 아래 정확 경로로 보정하며 재검수 전에는 Draft다.
- 추적: `REQ-M5D7QC4-001/003/005/007`, `AC-M5D7QC4-001/003/005/007/009/010`, 공동 `AC-M5D7QC3-007/008`.
- 선행 소유 규범: [QA 증거 프로토콜 r2](2026-09-29-c4-qa-evidence-protocol.md), [C4 r4 계약](2026-09-29-c4-r4-exact-implementation-amendment.md). 기존 문서 바이트는 보존한다. 이 문서는 아래 두 GenerationRule만 추가하며 런타임 권한·시험 손상 범위·발급 API를 확대하지 않는다.

## 실제 문제와 범위

실제 `ProfileResetExecutionBridgeV1.cs`는 원본 confirmed를 키로 `Executions.Add`를 수행하고 readonly Generation을 발급한 뒤 전체 witness/history/root 검증과 consume을 수행한다. 후속 검증 또는 체크포인트가 예외를 던지면 실행 기록은 있지만 C4 결과는 없다. 이 세대를 미관측이나 부재로 바꾸거나, 실패한 전체 검증을 정상 `Positive`로 인정해서는 안 된다.

실제 C2 반환 다음 순서는 `AfterC2Finalize` → 실제 C2 결과 관찰자 → `BeforeC2ResultMap`이다. 첫 경계의 예외는 C2 영수증이 실제 발급됐어도 관찰자에 C2 결과가 전달되지 않은 상태다. Adapter의 `_resetReceipt`는 **변경 가능한 필드**이며 그 필드 또는 숫자만으로 원본 영수증을 입증하지 않는다. 실제 영수증의 정상 Validate와 발급 원본·원본 세션/pair를 독립 검증해야 한다.

반면 `AfterTerminalPublish` 또는 `AfterFreshHandbackPublish` 예외 전에는 실제 C4 결과와 `ExecutionResults` 결속이 이미 발급된다. API가 결과를 반환하지 않았다는 이유로 결과 미발급 규칙을 사용하지 않는다. 실제 발급 결과를 정상 검증할 수 있으면 기존 Positive를 사용한다. 이미 존재하지만 정상 검증에 실패한 C4 결과는 이번 개정의 새 규칙으로 수용하지 않는다.

## 추가 규칙과 정확 조건

GenerationRule 객체 키는 여전히 `{Rule:string}` 하나다. `Positive/Absent/Unobserved`의 기존 조건은 바꾸지 않는다. 아래 규칙은 해당 한 경로에만 허용하며 다른 generation 경로·문자열·정상 결과 행에는 금지한다. actual 값은 기존과 같이 JSON 정수 token의 Int64 >0이며 canonical generation fact는 Observed다. canonical 36개, 모든 중첩 키, 직렬화·행 순서·확정 비교는 그대로다.

### executionGeneration의 IssuedWithoutResult

`ExpectedAuthorityCorrelation.executionGeneration.Rule="IssuedWithoutResult"`는 FullBridge/ActualCallback/ActualWorker의 사전 고정된 실제 실패 행에만 허용한다. checked는 false이며 Outcome/Phase는 null이다. 실제 호출이 던진 예외를 관찰하고, **동일 원본 confirmed 키의 실제 Executions CWT**에서 exact record를 읽어 다음을 모두 검증한다.

1. record.Confirmed가 원본 confirmed와 동일 참조이고, readonly Witness/Lifecycle 및 원본 발급 사건의 association이 동일하다. 발급 시 원본 adapter/router/owner/epoch/root와 두 원본 세대 및 원본 thread anchor를 실제 발급 기록에 대조한다. 손상된 mutable witness projection의 전체 Validate가 성공했다고 주장하지 않는다.
2. record의 원본 발급 Thread가 실제 현재 발급 thread와 동일하고 readonly Generation이 관측한 양수 Int64와 정확히 같다. CWT 존재 또는 숫자만으로 충분하지 않다.
3. record.Result와 record.ResultRecord가 **둘 다 null**이고, 그 record에 대한 실제 C4 결과 발급 결속이 없음을 읽기 전용으로 확인한다. API 반환값 null만으로 판단하지 않는다. 실제 결과 객체를 만들거나 새 issuer/getter를 추가하지 않는다.
4. 실제 실패 catch와 영구 fault 사건을 관찰한다. 실제 consume 전 실패와 consume 후 실패는 서로 다른 원장 행으로, 원본 ConsumedHistory와 원본 Consumed 사건 참조의 부재 또는 동일성을 정확 대조한다. consume이나 fault를 최종 boolean만으로 네이티브 사건 순서 증명으로 바꾸지 않는다.

RequiredFacts에는 아래 진단을 아래 순서로 사전 고정하고 terminal after에서 exact 값·certainty Observed로 대조한다.

| 이름 | 정확 기대값 |
| --- | --- |
| diagnostic.executionGenerationSameActualCwt | Yes |
| diagnostic.executionOriginalIssueAssociationVerified | Yes |
| diagnostic.executionOriginalThreadVerified | Yes |
| diagnostic.executionResultNotIssued | Yes |
| diagnostic.executionConsumeState | BeforeConsume 또는 AfterConsume 중 행별 하나 |
| diagnostic.executionFailureObserved | Yes |
| diagnostic.executionFaultHistoryObserved | Yes |

이는 원본 발급 역사 검증이며 전체 payload 정상 권한 검증이 아니다. 원본 관련 상관 문자열은 실제 입증 범위만 표시한다. 전체 상관 checked=false를 true로 바꾸지 않는다. 외부 thread의 사전 거절은 이 규칙으로 원본 thread payload를 읽지 않고 기존 명시적 Unobserved 조건을 따른다.

### memoryGeneration의 ReceiptWithoutExecutionResult

`ExpectedAuthorityCorrelation.memoryGeneration.Rule="ReceiptWithoutExecutionResult"`는 같은 행의 executionGeneration이 위 IssuedWithoutResult이고 checked=false인 경우에만 허용한다. 실제 c1Begin/c2Finalize는 각각 1이며, 실제 발급 영수증의 Validate가 성공해야 한다. original pair에 실제 발급된 세대와 정확히 같은 양수 Int64를 읽고 다음을 독립 검증한다.

- 동일 영수증 키의 private 실제 receipt Witnesses CWT/readonly witness가 존재하며 영수증의 정상 Validate와 세대·최종 disk proof·canonical document 결속이 모두 성공한다.
- mutable Adapter `_resetReceipt` 또는 실제 B 관찰자가 받은 C2 결과에서 찾은 객체를 원본 세션/원본 adapter/router 및 실제 발급 세대와 동일 참조·역사로 대조한다. 이 출처 필드 자체는 권한이 아니다. 두 출처가 모두 관측됐으면 동일 참조여야 한다. 현재 fault containment를 원본 발급 세션 역사로 오인하지 않는다.
- 실제 C2 결과가 관찰자에 전달됐는지와 정상 C2 결과 Validate가 성공했는지를 영수증 검증과 분리한다. 관찰 전 예외는 NotInspected, 허용된 actual C2 outcome 손상은 Rejected, 정상 관찰 결과는 Validated로 행별 사전 고정한다. Rejected를 C2 정상 결과 상관으로 보고하지 않는다. 미검사를 Absent로 기록하지 않는다.

위 원본 session/pair 연결은 다음 경로 **전체**를 읽기 전용으로 대조한다. 정상 현재 세션 getter/`CurrentSessionProfileV1.Validate()`는 faulted live 권한을 검사하므로 사용하지 않는다.

1. 동일 원본 adapter와 router 키의 `GuardedAdapters`/`GuardedRouters`는 IssuedWithoutResult에서 검증한 **동일 execution record**를 가리켜야 한다. 그 record의 readonly pair/root/원본 confirmed/lifecycle은 위의 실제 원본 발급 사건과 일치해야 한다. memory 세대와 execution 세대를 서로 같다고 요구하지 않는다.
2. 동일 원본 adapter/router 키의 Adapter `ResetLaunchCohortsByAdapter`와 `ResetLaunchCohortsByRouter`는 동일 cohort witness를 가리켜야 한다. 그 readonly Owner/Router/Root/Actions 및 원래 launch Receipt는 execution record·원본 launch/old actions와 일치해야 한다. cohort의 변경 가능한 terminal/Generation/OriginalNewActions만을 원본 pair 발급 권한으로 사용하지 않는다.
3. 원본 Adapter `_currentSession` 객체 키의 `CurrentSessionProfileV1.Witnesses`와 원본 `_resetReceipt` 객체 키의 receipt `Witnesses`가 실제 존재해야 한다. session의 readonly Owner/Root/Actions/Generation/Canonical과 해당 객체의 readonly 사본을 정확 대조한다. Owner/Root는 앞 두 원장과 같고, Actions는 실제 original-new candidate witness의 readonly Candidate와 같아야 한다. 원본 Adapter 키의 `ResetCandidateWitnesses`는 실제 해당 candidate witness여야 하며 원래 candidate의 Claimed=1을 관찰한다. 서로 다른 세션·candidate·영수증을 정수만으로 연결하지 않는다.
4. session Witness의 Generation/Canonical/Root는 정상 Validate를 통과한 receipt의 readonly witness Generation/Document/Root와 정확 일치해야 한다. 원본 Router의 `_resetNewActions/_actions`와 cohort OriginalNewActions, 원본 Adapter `_resetGeneration` 및 Router `_resetCutoverGeneration`/cohort Generation도 같은 실제 후보와 memory 세대인지 일관성으로 대조한다. 변경 가능한 숫자·참조만으로 앞의 readonly/CWT 검증을 대신하지 않는다. disposed actions는 참조 동일성만 읽고 Unity/native 속성이나 정상 입력 권한을 검사하지 않는다.
5. `ResetCutoverCapabilityV1`은 실제 C2 내부 지역 변수이며 receipt/session은 그 객체를 보존하지 않는다. 따라서 약한 CWT의 객체가 우연히 남아 있다고 가정하거나 런타임 내부 CWT 저장소를 열거해 capability를 복구하지 않는다. 실제 mint→capability Validate/Consume→원본 session 생성→최종 receipt 생성·Publish 경로의 지배는 **동결된 원본 source의 독립 검수**로 증명하고 SourceEstablished로 분리한다. 정상 private constructor 이외에 session/receipt를 만드는 경로가 없고 각 실제 source 경계의 exact pair/proof 검증이 지배함을 확인한다. 이 source 증명을 actual capability 객체 관측·현재 정상 pair 검증으로 표시하지 않는다. 새 capability observer나 보존 필드/API는 추가하지 않는다.

5번의 source 경로는 `diagnostic.memoryReceiptIssuePathSourceEstablished=Yes`·SourceEstablished를 memory 새 규칙의 마지막 RequiredFact로 추가한다. 앞 1..4의 실제 CWT·readonly 동일성 전체 성공은 아래 `diagnostic.memoryOriginalSessionPairVerified=Yes`·Observed의 필수 지배 조건이다. source 경로가 다른 최종 byte에서는 독립 재검수 전 통과시키지 않는다.

RequiredFacts에 다음 진단을 아래 순서로 사전 고정하고 terminal after에서 exact 값과 아래 certainty로 대조한다. 앞 다섯 진단은 Observed이며 마지막 source 진단만 SourceEstablished다.

| 이름 | 정확 기대값 |
| --- | --- |
| diagnostic.memoryGenerationSameActualCwt | Yes |
| diagnostic.memoryReceiptValidateSucceeded | Yes |
| diagnostic.memoryOriginalSessionPairVerified | Yes |
| diagnostic.memoryReceiptSource | AdapterPublishedReceipt 또는 ActualC2ObserverAndAdapterPublishedReceipt 중 행별 하나 |
| diagnostic.memoryC2ResultValidation | NotInspected/Rejected/Validated 중 행별 하나 |
| diagnostic.memoryReceiptIssuePathSourceEstablished | Yes, SourceEstablished |

memoryReceiptSource의 첫 값은 실제 B observer call0, 두 번째 값은 실제 B observer call1 및 두 출처 동일 참조를 요구한다. NotInspected는 첫 값과만 결합하며 Rejected/Validated는 두 번째 값과만 결합한다. expected callCounts.c2Observer를 그 값으로 정확 고정한다. 영수증 자체가 없으면 실제 권한 부재를 관찰한 기존 Absent만 사용한다. 실제 영수증 또는 발급 원본을 검증할 수 없으면 행은 Failed이며 이 규칙으로 수용하지 않는다.

## 구현·검수 경계

변경 허용은 기존 배분된 두 fixture와 공유 row emitter/ExpectedRows 정의, `c4-build-required-checkpoint-rows.ps1` 및 `c4-verify-required-rows.ps1`의 규칙 허용·조건·정확 진단 검증뿐이다. 다른 도구가 동일 규칙을 직접 검사한다면 그 정확 분기만 변경 이유와 경로를 추가 기록하고 독립 검수한다. 런타임·정상 issuer·공개 API·friend/asmdef·손상 seam은 추가하지 않는다. 읽기는 기존 실제 private 원장에 대한 fixture의 반사 관찰이며 원장 값을 수정하지 않는다.

진단은 RequiredFacts 전체 배열의 정확 비교를 따른다. 두 새 규칙의 진단은 해당 규칙에만 필수이고 다른 규칙에 그 범위를 암시적으로 확장하지 않는다. 기존 positive 진단 이름이 있어도 checked=false에서 Positive를 인정하지 않는다. source 기반 행 계획은 사전 동결하며 실행 뒤 사유·규칙을 바꾸지 않는다. raw object 주소/root/identity/proof 원문은 출력하지 않는다.

독립 검수는 actual CWT/발급 원본 검사·정상 receipt Validate가 emitter의 Yes 발행을 지배함을 확인한다. 조건의 누락/위조 숫자/결과 이미 발급/잘못된 thread/잘못된 원본 pair/검증 실패 영수증/observer call 불일치/checked=true/부정확한 diagnostic 값 또는 certainty를 각각 거절하는 국소 검증을 수행한다. 국소 도구 검증은 실제 Unity 검증이나 전체 수용을 대신하지 않는다. 최종 byte freeze와 별도 실제 실행 배분 요건은 유지한다.
