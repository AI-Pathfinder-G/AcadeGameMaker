# C4 발급 후 C2 미호출 실패 증거 개정

- 상태: **Approved — 네 경계의 한정 증거 규칙 구현 승인, 실제 실행·통합 수용 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 승인 근거: [독립 검수](../../verification/2026-09-30-c4-pre-c2-failure-evidence-luna-review.md) SHA `65C7D8CEA657634AF6234B10C28508A75480BA88BE8B204EA4DC467583BFB790`, 정적 규범 P0/P1=0/0. 검수 초안 SHA `39DBF9B54C6722AD0AAF5705E5EE3FE4E4C2A28DA5373E52B8544BB6A2C95686`을 `2026-09-30-c4-pre-c2-failure-evidence-reviewed-draft.md`에 그대로 보존했다. 본문 중 승인 전 문장은 해당 검수 이력이며 현재 한정 구현은 승인됐다. 별도 실제 실행 배분은 남아 있다.
- 작성·기술 선택: 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-001/003/005/007`, `AC-M5D7QC4-001/003/005/007/009/010`, 공동 `AC-M5D7QC3-007/008`.
- 근거: Approved [QA 증거 규약 r2](2026-09-29-c4-qa-evidence-protocol.md), [발급 세대 증거 개정](2026-09-30-c4-issued-generation-evidence-amendment.md), [C4 r4](2026-09-29-c4-r4-exact-implementation-amendment.md).
- 해소 대상: [런타임 8개 독립 사전 검수](../../verification/2026-09-30-c4-runtime-eight-luna-prereview.md) SHA `7B7731B4F887A4C2DBFDE26FF0CA7543390B05CFCAB80161571D7988BCA8FFB9`의 C2 미호출 예외행 증거 P1. 런타임 정적 P0/P1=0/0 판정과 이 증거 공백은 별도다.

## 한정된 문제

실제 C4 실행 기록과 consume은 발급됐지만 C1 정상 반환 뒤 C2 호출 전에 시험 callback이 예외를 던지면, C1 반환값과 proof는 아직 지역 변수에만 있다. 실제 prepared observer나 ResultRecord가 이 참조를 보존하지 않는다. 미관측 proof를 실제 부재로 바꾸거나 약한 CWT를 열거하여 복원해서는 안 된다. 기존 executionGeneration의 IssuedWithoutResult 조건은 유지하지만 memoryGeneration의 기존 Unobserved 허용 범위는 이 경로를 포함하지 않는다.

## 추가 규칙과 전체 조건

GenerationRule의 키는 기존 `{Rule:string}` 하나이며, 새 값 `C2NotInvokedAfterIssuedExecution`은 정확 `ExpectedAuthorityCorrelation.memoryGeneration` 및 대응 canonical RequiredFact에만 허용한다. 기존 Positive/Absent/Unobserved/IssuedWithoutResult/ReceiptWithoutExecutionResult의 조건은 바꾸지 않는다. canonical 36개, 중첩 키, SchemaVersion1, exact 비교와 진단 순서를 유지한다.

다음 조건을 **모두** 만족하는 사전 고정된 네 실패행에만 허용한다.

1. EvidenceKind는 FullBridge/ActualCallback/ActualWorker 중 하나이며 CheckpointFamily=C4다. CheckpointName/CheckpointValue는 실제 enum의 `AfterC1Begin`, `BeforeC1ResultValidate`, `AfterC1ResultValidate`, `BeforePreparedProofTake` 중 정확 하나와 그 실제 값이다. 다른 이름·enum 값·정상 행·순수 구조 행에 금지한다. 실제 6인자 제어의 해당 checkpoint callback 도달과 그 callback이 던진 정확 예외 관측이 필요하다. 미도달을 도달로 바꾸지 않는다.
2. 같은 행의 executionGeneration은 Approved IssuedWithoutResult의 **전체** 실제 CWT/readonly 원본 발급·원본 thread·결과와 ResultRecord 둘 다 null·실제 catch/fault 이력 조건을 만족한다. 실제 original consume 참조를 확인하여 `diagnostic.executionConsumeState=AfterConsume`·Observed다. checked=false, Outcome=null, Phase=null이며 정상 전체 payload 검증을 통과했다고 주장하지 않는다.
3. actual memoryGeneration은 JSON null이고 대응 canonical Fact.value는 정확 `null`, certainty=Unknown이다. 새 규칙은 미관측을 표현하며 세대 발급 부재나 정상 memory 권한의 증명이 아니다. 원본 adapter/router/owner/root 및 confirmed 상관은 기존 실제 원본 검증 범위를 유지한다.
4. actual c1Begin=1, c2Finalize=0, preparedObserver=0, c2Observer=0, freshReserve=0, freshComplete=0이다. 호출 수 certainty는 실제 관찰한 제어 계수기에 Observed, 고정 소스의 단일 정상 반환·checkpoint 이전/이후 호출 지배로 확인한 수에 SourceEstablished를 사용하며 행별 사전 고정한다. 내부 C1 진입과 반환 뒤 checkpoint는 구분한다. 실제 checkpoint 도달만으로 관찰하지 않은 내부 native callback 수를 0으로 쓰지 않는다. nativeCallback은 해당 행의 기존 필수 관측 범위 및 exact null 규칙을 그대로 따른다.
5. authorityCorrelation.proof는 정확 Unknown이며 대응 canonical certainty=Unknown이다. 실제 proof 참조가 아직 관찰자에게 전달되지 않았다는 범위에만 적용한다. 실제 C1 Begin 호출 정상 반환과 해당 checkpoint 도달은 관찰하고, 실제 prepared observer 호출0을 계수기에서 확인한다. 지역 disk/proof를 읽었다고 주장하지 않는다. proof 부재·외부 proof·정상 proof 검증의 증거로 사용하지 않는다.
6. 실제 동결된 Bridge 소스 바이트와 exact source 경계로 C1 정상 반환 뒤 해당 callback, proof 관찰자 이전, C2 호출 이전의 지배를 확인한다. SourceManifest와 InputPathList에 그 실제 파일을 결속하고 독립 검수한다. source 증명은 actual proof 관측 또는 actual C2 authority를 대체하지 않는다. barrier의 실제 상태·writer 차단·history·guard·cleanup 및 다른 canonical 사실에는 이 규칙의 Unknown 면제를 확대하지 않는다. 장벽 참조는 기존 규약의 실제 root/marker/lease 관측과 FrozenSourceBoundary 결속으로 검증하며 미관측 proof를 ActualTypedC1Row로 표시하지 않는다.

기존 IssuedWithoutResult 진단 뒤에 다음 진단을 아래 순서로 exact 추가한다. 다른 행에 이 규칙의 면제를 암시하지 않는다.

| 이름 | 정확 값 | certainty |
| --- | --- | --- |
| diagnostic.preC2FailureCheckpointObserved | 해당 행의 정확 checkpoint 이름 | Observed |
| diagnostic.preC2FailureExceptionObserved | Yes | Observed |
| diagnostic.preparedProofObservation | NotReachedBeforeActualObserver | SourceEstablished |
| diagnostic.memoryGenerationObservation | C2NotInvokedAfterIssuedExecution | SourceEstablished |
| diagnostic.preC2FailureSourceBoundaryVerified | Yes | SourceEstablished |

실제 예외는 fixture의 정상 제어 callback과 호출 catch에서 대조하고 다른 예외·미도달·후속 C2 호출을 이 규칙으로 수용하지 않는다. terminal Passed는 음성 시험의 정확 관측·검증 성공 및 실제 XML 부모 Passed를 뜻하며 제품 실행 성공을 뜻하지 않는다. RequiredFacts 전체 exact 배열·nested canonical 결속·실제 정수 타입·checked/outcome/phase 비교를 유지한다.

## 구현·검수 경계

Approved 뒤 변경 허용은 기존 배분된 두 fixture/shared emitter·ExpectedRows와 `c4-build-required-checkpoint-rows.ps1`, `c4-verify-required-rows.ps1`의 한정 규칙 처리다. 기존 시험 본문 연결이 필요하면 이미 배분된 새 Edit/Play 시험 파일에 한정하고 실제 경로·REQ/AC를 기록한다. 런타임8·C1 알고리즘·API·observer·friend/asmdef·손상 seam·기존 회귀 시험을 변경하지 않는다. 최종 source/input/selection/rows/plan 동결과 독립 검수 및 별도 실제 실행 배분 요건은 유지한다.

독립 검수는 네 실제 callback의 source 지배와 emitter의 원본·도달·예외 관측이 진단 발행을 지배함을 확인한다. 국소 도구 검증은 네 허용 경계별 정상 입력과 각각의 잘못된 경계 이름/enum 값, 미도달, 다른 예외 관측, consume 전, 실제 결과 존재, checked=true, non-null memory, proof Absent/SameActual, C1=0, C2/각 observer/fresh 호출>0, 누락·잘못된 진단 값·certainty, source 결속 누락을 거절해야 한다. 국소 합성 입력 검증은 실제 Unity 실행이나 원본 CWT 관측을 대신하지 않는다. 실행 뒤 예상 사유나 규칙을 변경하지 않는다.
