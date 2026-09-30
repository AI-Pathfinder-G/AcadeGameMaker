# C4 발급 세대 증거 개정 r2 독립 재검토

- 검토자: GPT-6 Luna
- 대상 규범: `docs/specs/work-contracts/2026-09-30-c4-issued-generation-evidence-amendment.md` — SHA-256 `AF4051D04CCB629A2C0DE051C36D2258AAAB3E398DBDF014C6FD8481FE7F408E`.
- 기존 P1 원문 보존: r1 검토는 별도 기록으로 유지한다. 이번 판정은 r2의 새 문구만 대상으로 한다.
- 대조 규범: QA r2 SHA-256 `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`; C4 r4 SHA-256 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`.
- 대조 구현 지문: `ProfileResetExecutionBridgeV1.cs` `410058195F108DC35E960F832FE0A0D9848C8D9FCBD40941FD2EFD844DB49919`; `ProfileResetMemoryCutoverV1.cs` `55B8C19F88B07C5888128C86BF05A743D747AB206E6D44942110ABE0AAFDD38B`; `DesktopProfileLaunchAdapterV1.cs` `4B157B98511A0978D19B25434F6D00E23C0CC6FB53BA8A14969182865C3BDC0E`.
- 범위: 읽기 전용 규범·소스 검토. 코드, 원장, 시험, Unity를 수정하거나 실행하지 않았다. 실행 성공·수용 판정은 하지 않았다.

## 판정

P0=0, P1=0. r2는 r1의 receipt/session 상관 공백을 실제 존재하는 불변 원장 경로로 좁혀 닫았다. 새 진단은 capability 객체를 사후 복구하거나 관측했다고 주장하지 않고, private mint부터 receipt publish까지 동결 source 경로가 증명하는 사실을 `SourceEstablished`로 분리한다. 실행/메모리 receipt 발급 세대는 서로 동일하다고 요구하지 않으며, receipt만 유효하다는 사실을 C4 결과 검증이나 정상 현재 세션 권한으로 승격하지 않는다.

## 주요 근거

`IssuedWithoutResult`는 실제 C4 실행 원장 순서와 맞는다. Bridge는 원본 confirmed·lifecycle 및 재진입 가능성을 확인한 뒤 generation을 발급하고 `Executions` CWT에 `ExecutionRecord`를 추가한다. 그 후 mutable request projection, history/root, consume, C1/C2 단계를 검증·실행한다(`ProfileResetExecutionBridgeV1.cs:214–248`). 예외 시 실제 record는 남고 `Result`/`ResultRecord`가 null일 수 있다. r2가 둘 다 null, exact confirmed 키, 동일 record·readonly issue/witness/lifecycle, 현재 발급 thread, 실제 failure/fault와 consume 전후의 원본 사건을 강제하고 `checked=false`를 유지한 것은 적합하다. 양수 카운터만 읽거나 거짓 result를 발급하는 우회는 이 규칙의 조건을 만족하지 않는다.

`ReceiptWithoutExecutionResult`는 r1에서 빠졌던 연결을 다섯 단계의 exact 경로로 보완한다. lower 실행의 `GuardedAdapters`와 `GuardedRouters`가 동일 execution record를 가리키고, Adapter의 양쪽 원본 cohort CWT가 같은 readonly owner/router/root/actions/launch receipt를 가리키며, 실제 `_currentSession`과 `_resetReceipt` 객체를 각 CWT 키로 읽어 immutable witness 사본을 대조한다. Adapter의 `ResetCandidateWitnesses`에서 실제 새 actions candidate와 `Claimed=1`도 확인한다. fault 뒤 `CurrentResetSession` getter 또는 `CurrentSessionProfileV1.Validate()`를 사용하지 말라는 제한은 실제 구현과 일치한다. 해당 getter는 `Validate()`를 거쳐 현재 live reset-pair를 재검사하는 반면, session CWT witness는 immutable Owner/Root/Canonical/Actions/Generation을 보존한다(`ProfileResetMemoryCutoverV1.cs:66–76`; adapter 원장 `DesktopProfileLaunchAdapterV1.cs:75–94, 137–141`).

실제 receipt의 CWT witness는 canonical receipt payload와 memory generation을 저장하고, `Validate()`는 이를 readonly 사본 및 final disk proof와 비교한다(`ProfileResetMemoryCutoverV1.cs:79–87`). 원본 capability는 C2 내부 지역 변수이며 receipt/session에 저장되지 않는다. r2는 그 weak CWT를 열거해 capability를 찾는 비현실적 가정을 금지하고, capability의 정확한 owner/router/actions/root/generation/consume 및 `ValidateAppliedPairForReceipt`·adapter `ValidateResetPair`·`PublishResetReceipt`의 지배를 동결 소스 검토로 따로 기록하게 한다(`ProfileResetMemoryCutoverV1.cs:42–63, 84–85`; `DesktopProfileLaunchAdapterV1.cs:630–661`). 이는 실제 immutable source로 가능한 경계이며 런타임 API·새 보존 필드를 만들지 않는다.

Observer 시점 분리도 실행 순서에 맞다. `AfterC2Finalize` checkpoint 다음에 실제 C2 결과 observer가 불리고 그 다음 `BeforeC2ResultMap`이 호출된다(`ProfileResetExecutionBridgeV1.cs:283–288`). 따라서 observer 0회/receipt만 관측한 경로와 observer 1회/실제 C2 결과를 관측한 경로를 구분하고, receipt 정상 검증과 C2 결과 `NotInspected`·`Rejected`·`Validated`를 따로 고정한다. Observer 수, receipt 출처, certainty가 행별 사전 선언되어 exact 비교되는 조건도 유지된다.

새 `memoryReceiptIssuePathSourceEstablished` 사실은 r2의 남은 capability 관측 한계를 정직하게 표시한다. `SourceEstablished`만으로 actual capability 관측이나 정상 pair 검증을 주장하지 않으며, actual CWT 동일성·receipt 검증 사실은 별도 `Observed` 진단이 모두 성공해야 한다. 따라서 old QA r2 `Positive/Absent/Unobserved`, canonical facts, 행·필드 순서, runtime 권한 경계가 무단 완화되지 않았다.

## 한계

개정안은 아직 Draft다. emitter/validator의 exact 구현, 음성 경로 거절, freeze된 소스에 대한 독립 지배 검토, 별도 Unity 실행은 이 검토에서 수행하지 않았다. 이 문서는 설계 P0/P1=0 판정일 뿐 구현 가능성이 실행으로 증명되었거나 QA 규칙이 승인되었다는 뜻이 아니다.
