# C4 발급 세대 증거 개정 독립 검토

- 검토자: GPT-6 Luna
- 대상 개정안: `docs/specs/work-contracts/2026-09-30-c4-issued-generation-evidence-amendment.md` — SHA-256 `148E66886B58584B2C0C5AC825FDF4FA1A4991C00F6FF789C53D4601A3EEF3B6`
- 대조 계약: QA 증거 프로토콜 r2 SHA-256 `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`; C4 r4 SHA-256 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`.
- 대조 구현: `ProfileResetExecutionBridgeV1.cs` SHA-256 `410058195F108DC35E960F832FE0A0D9848C8D9FCBD40941FD2EFD844DB49919`; `ProfileNewGameConfirmationV1.cs` SHA-256 `8248404F3F237936954979F3FEC2848E9F419CF0AA2A781F732D5CA379FF9809`; `ProfileResetMemoryCutoverV1.cs` SHA-256 `55B8C19F88B07C5888128C86BF05A743D747AB206E6D44942110ABE0AAFDD38B`; `DesktopProfileLaunchAdapterV1.cs` SHA-256 `4B157B98511A0978D19B25434F6D00E23C0CC6FB53BA8A14969182865C3BDC0E`.
- 범위: 승인 전 규범의 정적 검토. Unity 실행, 컴파일, 런타임·QA 파일 수정은 하지 않았다. 이 문서는 개정안 승인이나 C4 수용 판정이 아니다.

## 판정

P0는 없다. `IssuedWithoutResult`를 기존 `Positive`와 분리하고 `checked=false`, 결과 두 저장소의 부재, 실제 CWT 발급 기록·thread·실패/consume 사건을 요구한 방향은 적절하다. `ReceiptWithoutExecutionResult`도 기존 memory-generation `Positive`를 대체하지 않고, receipt 검증과 C2 결과 관찰 상태를 나누며 observer 호출 수를 고정한다. 실제 C4 결과가 이미 발급된 뒤 API가 예외를 던진 경우 기존 `Positive`를 유지하는 구분도 맞다.

P1 한 건: `ReceiptWithoutExecutionResult`가 “원본 session/pair” 확인을 요구하지만, 어떤 불변 원장을 통해 그 receipt를 **현재 exact C4 `ExecutionRecord`와** 연결해야 하는지 규범상 충분히 고정하지 않았다. `ProfileResetMemoryReceiptV1.Witness`는 receipt 스냅샷과 C2 generation을 보존하지만 adapter/router/session 또는 C4 execution 참조를 담지 않는다(`ProfileResetMemoryCutoverV1.cs:81–87`). Adapter의 `_resetReceipt`도 변경 가능한 필드다. 특히 실패 뒤 `CurrentResetSession`/`CurrentSessionProfileV1.Validate()`를 호출하면 현재 Faulted 생명주기 검증으로 실패할 수 있어, 이를 발급 증거로 사용해서는 안 된다.

최소 보정은 새 권한이나 런타임 API 없이 기존 private 관리 기록을 명시하는 것이다. 동일 adapter와 router의 `GuardedAdapters`/`GuardedRouters` CWT가 같은 C4 `ExecutionRecord`를 가리키는지, 그 record의 confirmed/lifecycle/original pair가 현재 실패행의 원본과 일치하는지 읽기 전용으로 확인한다. Faulted session에서는 live getter 대신 `CurrentSessionProfileV1.Witnesses`의 불변 Owner/Root/Actions/Generation 복사본과 기존 adapter `ResetLaunchCohortsByAdapter` 및 router 쌍의 원본 기록을 대조하고, `ResetCutoverCapabilityV1.Witnesses`의 Owner/Router/Old/New/Root/Generation/Consumed 사건이 session·receipt와 연결되는지도 검사한다. Receipt 자체는 CWT `Witnesses` 조회와 `Validate()`를 통과해야 한다. 이 증명은 정상 현재 세션 권한이나 C2 정상 결과 검증을 의미하지 않는다.

## 규칙별 대조

`IssuedWithoutResult`는 실제 코드 순서와 부합한다. lower는 실제 confirmed/lifecycle을 재확인한 뒤 세대를 증가시키고 `Executions.Add(confirmed, execution)`으로 원본 CWT에 record를 발급한다(`ProfileResetExecutionBridgeV1.cs:214–233`). 이후 projection/history/root 검증, `BeforeConfirmedConsume`, consume 및 C1/C2 호출이 진행된다(234–295행). 따라서 발급 record가 있으면서 result/ResultRecord가 없는 실패가 가능하며, 이 행을 기존 `Positive`로 인정하거나 세대 미발급 `Unobserved`로 표현하지 않는 구분이 타당하다. 개정안의 exact confirmed key·동일 readonly record·양수 Generation·source/thread/issue association·Result와 ResultRecord 모두 null·실제 fault 및 consume 이력 조건은 임의 숫자나 객체 복제로 권한을 만들지 않는다.

다만 `ReceiptWithoutExecutionResult`에서 `memoryGeneration`은 C2 capability/receipt의 generation이며 C4 `ExecutionRecord.Generation`과 별개다. 규칙은 둘을 바꾸어 대조하지 않아야 한다. C2 receipt가 만들어지는 경로에서 `ResetCutoverCapabilityV1`은 CWT에 immutable owner/router/old/new/root/hash/length/generation와 consumed 상태를 보존하고, receipt 발급은 capability의 적용된 pair 검증과 `PublishResetReceipt`의 pair 검증 뒤 일어난다(`ProfileResetMemoryCutoverV1.cs:42–63, 84–87`; `DesktopProfileLaunchAdapterV1.cs:630–661`). 개정안은 이러한 실제 출처 검증을 요구하므로 구조적으로 구현 가능하다. 단, 위 P1의 정확한 same-C4-record 및 faulted-session 증거 경로를 규범으로 고정해야 verifier가 단순히 정상 receipt 또는 `_resetReceipt` 값만 보고 통과시키지 않는다.

observer 분기 `NotInspected`/`Rejected`/`Validated`는 C2 결과 검증과 receipt 검증을 분리한다. `AfterC2Finalize`는 결과 observer보다 먼저 실행되고, observer 뒤 `BeforeC2ResultMap`이 실행된다(`ProfileResetExecutionBridgeV1.cs:283–288`). 따라서 observer 호출 0에서 receipt가 실제 발급된 행과 호출 1에서 받은 memory 결과의 검증 결과를 나누는 것이 실제 순서에 맞다. 행별 observer 호출 수와 receipt source를 사전 고정하고 exact RequiredFacts 대조를 유지해야 한다.

QA r2의 기존 Positive/Absent/Unobserved 요건, canonical 사실·순서, exact diagnostic 비교를 바꾸지 않고 새 규칙만 보태는 구성은 승인 후 구현 가능한 범위다. 특히 새 규칙은 checked=false를 강제하고, 기존 Positive는 result/receipt 정상 검증 성공을 계속 요구한다. **승인 전 구현 또는 행 수용 근거로 쓰지 않는다.**

## 남은 조건

개정안은 Draft이며 P1의 불변 원본 상관 경로를 구체화한 뒤 독립 재검토와 Astra 승인이 필요하다. 이후 verifier의 정상·음성 국소 검증과 별도 실제 Unity 실행도 필요하다. 이번 검토는 어떤 행의 실제 실행·통과도 확인하지 않았다.
