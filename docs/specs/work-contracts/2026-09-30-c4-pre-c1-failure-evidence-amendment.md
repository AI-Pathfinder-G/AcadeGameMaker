# C4 발급 후 C1 미호출 실패 증거 개정

- 상태: **Approved — C1 미호출 일곱 경계의 한정 증거 규칙 구현 승인, 실제 실행·통합 수용 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 승인 근거: [원문 독립 검수](../../verification/2026-09-30-c4-pre-c1-failure-evidence-luna-review.md) SHA `C65C62A9D67B9F2447AE9949C0E2C29ABE0CA071A2EB91DF7F0FA1FDB6F3F928` 및 [스키마 철자 한정 재검수](../../verification/2026-09-30-c4-pre-c1-failure-evidence-amendment-luna-rereview.md) SHA `4F42CDEAE8C6CE8DAD78CFF668CDB61D884340E4BB39B5F57CCE182DDE07B53F`, 모두 정적 P0/P1=0/0. 원문 초안 SHA `5AA8812873040272389EC46CBE761F30A8DCB98569685A056C8DB34AEF4995F3`을 `2026-09-30-c4-pre-c1-failure-evidence-reviewed-draft.md`에 정확 보존했다. 변경은 `RequiredReach`를 실제 schema 키 `ExpectedReach`로 고친 한 곳뿐이다. 본문 승인 전 문장은 당시 이력이며 별도 실제 실행 배분은 남아 있다.
- 작성·기술 선택: 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-001/002/003/006/007`, `AC-M5D7QC4-001/002/003/006/008/009/010`, 공동 `AC-M5D7QC3-007/008`.
- 근거: Approved [QA 증거 규약 r2](2026-09-29-c4-qa-evidence-protocol.md), [발급 세대 증거 개정](2026-09-30-c4-issued-generation-evidence-amendment.md), [C2 미호출 실패 증거 개정](2026-09-30-c4-pre-c2-failure-evidence-amendment.md), [C4 r4](2026-09-29-c4-r4-exact-implementation-amendment.md).

## 정확한 공백

실제 Bridge는 동일 원본 confirmed를 키로 Executions CWT record를 발급한 뒤, consume 이전 `BeforeConfirmedConsume`부터 C1 호출 직전 `BeforeC1Begin`까지 일곱 C4 checkpoint를 지난다. callback 예외가 나면 실제 C1은 호출되지 않았고 C4 Result/ResultRecord도 없다. 첫 경계에서는 consume 전이고 나머지 여섯 경계에서는 consume 후다. 기존 `executionGeneration=IssuedWithoutResult`가 원본 발급과 이 구별을 검증할 수 있지만, `memoryGeneration=Unobserved`의 기존 허용 범위는 실행 CWT 발급 뒤의 이 실패 경로를 열지 않는다. 미발급 C1 지역값과 proof를 Absent 또는 SameActual로 꾸미지 않는다. 기존 정상 결과·영수증 규칙은 변경하지 않는다.

## 단일 한정 규칙

GenerationRule 객체는 여전히 정확 `{Rule:string}`이다. 새 `C1NotInvokedAfterIssuedExecution`은 `ExpectedAuthorityCorrelation.memoryGeneration`과 대응 canonical RequiredFact에만 허용하며 다음 조건을 모두 요구한다.

1. EvidenceKind=FullBridge/ActualCallback/ActualWorker, CheckpointFamily=C4, `ExpectedReach=RequiredReached`이고 CheckpointName/CheckpointValue는 실제 enum의 `BeforeConfirmedConsume=1`, `AfterConfirmedConsume=2`, `BeforeAdapterGuard=3`, `AfterAdapterGuard=4`, `BeforeRouterGuard=5`, `AfterRouterGuard=6`, `BeforeC1Begin=7` 중 행별 사전 고정한 정확 값이다. 다른 checkpoint/값, C1/C2/None, 정상 결과, 순수 구조, 미도달은 금지한다. 실제 승인 6인자 제어에서 해당 checkpoint callback 도달과 그 callback이 던진 동일 예외를 관찰한다. 단순 API 예외만으로 원인을 대체하지 않는다.
2. 같은 행의 `executionGeneration=IssuedWithoutResult`의 **전체** 동일 confirmed-key 실제 Executions CWT/readonly 발급 역사·원본 thread·결과/ResultRecord 둘 다 null·실제 catch/fault 이력·소비 상태 조건을 유지한다. `BeforeConfirmedConsume=1`의 `diagnostic.executionConsumeState=BeforeConsume`, 2..7의 값은 `AfterConsume`이며 각각 실제 original `ConsumedHistory`/Consumed 사건과 동일성을 확인한다. checked=false, Outcome=null, Phase=null이다. 발급 기록 존재를 전체 payload 정상 검증으로 승격하지 않는다.
3. actual memoryGeneration은 JSON null, 대응 canonical Fact.value=`null`, certainty=Unknown이다. proof 상관은 정확 Unknown 및 대응 canonical certainty=Unknown이다. 이는 원본 발급 세대나 proof의 부재가 아니라 이 C4 실행에서 아직 C1/C2 지역 결과와 prepared observer에 도달하지 않은 한정 미관측이다. 별도 원본 owner/adapter/router/root/confirmed 상관은 실제로 검증한 범위만 유지한다.
4. actual `c1Begin=0`, `c2Finalize=0`, `preparedObserver=0`, `c2Observer=0`, `freshReserve=0`, `freshComplete=0`은 각각 행별 exact 값이다. 실제 6인자 control 계수기·source상 checkpoint 이후에 있는 유일 C1 호출과 후속 C2/observer/fresh 분기의 지배를 대조한다. 실제 제어 계수기 관측은 Observed, 동결 source의 미호출 지배는 SourceEstablished로 분리한다. 콜백 도달만으로 미관측 native callback 값을 0으로 발행하지 않는다. 기존 native 관측 필수 사례는 실제 관찰하고 비필수 사례의 exact null/범위 진단 규칙을 따른다.
5. 동일 C4 실행이 C1/새 barrier/receipt를 생성하지 않았다는 source 경계와, 실제 root/marker/lease의 사전·사후 상태를 구분한다. 원래 존재할 수 있는 barrier를 이 규칙만으로 NoBarrier·writer 가능으로 발행하지 않는다. 가드의 부분 진입/영구 fault/native 폐쇄·history·cleanup과 실제 root/marker/lease 장벽 의무는 기존 정확 관측을 유지한다. 이 규칙은 그 값들의 Unknown 면제가 아니다. proof가 미관측이면 `ActualTypedC1Row`로 장벽 증거를 표시하지 않는다. 필요한 장벽 근거는 FrozenSourceBoundary와 실제 root/marker/lease 결속 또는 기존 actual writer 관측을 따른다.
6. 실제 동결된 Bridge와 두 부모 시험 source를 SourceManifest.Files 및 InputPathList에 정확 SHA로 결속한다. source는 CWT 발급→1..7 checkpoint→C1 호출의 순서를 증명하며 실제 callback/예외/원본 CWT 관측을 대체하지 않는다. 결과 뒤 source/예상 원장 변경은 실패다.

기존 IssuedWithoutResult 필수 진단 뒤에 아래 여섯 진단을 이 순서로 추가하고 terminal after의 RequiredFacts 전체 배열에서 정확 값·certainty를 대조한다.

| 진단 이름 | 값 | certainty |
| --- | --- | --- |
| diagnostic.preC1FailureCheckpointObserved | 해당 행의 정확 checkpoint 이름 | Observed |
| diagnostic.preC1FailureExceptionObserved | Yes | Observed |
| diagnostic.preC1BeginNotInvoked | Yes | SourceEstablished |
| diagnostic.preparedProofObservation | NotReachedBeforeActualObserver | SourceEstablished |
| diagnostic.memoryGenerationObservation | C1NotInvokedAfterIssuedExecution | SourceEstablished |
| diagnostic.preC1FailureSourceBoundaryVerified | Yes | SourceEstablished |

canonical 36개, 중첩 키, SchemaVersion1, 실제 Int64 유형, enum·NUnit 이름·Order, 부모 Passed와 exact 행 비교는 그대로다. 다른 세대 규칙·정상 FullBridge의 Unknown 허용 범위를 넓히지 않는다. 이 규칙은 C2 미호출 개정의 네 C1 반환 경계에 적용하지 않는다.

## 승인 후 구현·검수 범위

독립 검수와 아스트라 승인 후 허용 변경은 기존 배분된 새 Edit/Play 부모 시험·두 fixture/공유 emitter·ExpectedRows, `c4-build-required-checkpoint-rows.ps1`과 `c4-verify-required-rows.ps1`의 이 단일 규칙·정확 진단 조건만이다. 런타임8·C1/Profile 알고리즘·public API·새 observer/factory·친구/asmdef·손상 경계·기존 회귀 시험은 바꾸지 않는다. 실제 원장 또는 Unity 실행은 최종 source/입력/계획 동결, 독립 검수와 아스트라의 별도 배분 전에는 하지 않는다.

국소 도구 검증은 일곱 정상 형태 및 각 이름·값 불일치, 다른 checkpoint, 잘못된 consume 상태, CWT/원본 thread/발급·fault 이력 결여, C4 결과 이미 발급, checked=true, non-null memory, proof Absent/SameActual, c1/C2/observer/fresh 수 양수, 미도달·다른 예외, 필요한 진단 누락·값·certainty 오류와 source 결속 누락을 거절한다. 합성 행 통과는 actual CWT·실제 예외나 전체 Unity 검증을 대체하지 않는다.
