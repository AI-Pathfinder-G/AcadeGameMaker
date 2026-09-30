# C4 발급 후 미반환 경계의 한정 증거 개정

- 상태: **Approved — 일곱 미반환 경계의 한정 증거 규칙 구현 승인, 실제 실행·통합 수용 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 승인 근거: [독립 루나 검수](../../verification/2026-09-30-c4-issued-unreturned-boundaries-luna-review.md) SHA `3BFD9475BDA9CD6C47E152A5272892D1D9E5C42BF3B426EB9B872DBDF73F8B89`, 정적 P0/P1=0/0. 검수 원문 SHA `5E8E7844117AB21AE14C6654105EAABF2897DAA567335ABB666C55C783E3669B`은 `2026-09-30-c4-issued-unreturned-boundaries-reviewed-draft.md`에 정확 보존했다. 본문 승인 전 문장은 당시 이력이고 실제 Unity 실행은 별도 배분이다.
- 추적: `REQ-M5D7QC4-001/003/005/006/007`, `AC-M5D7QC4-001/003/005/007/008/009/010`, 공동 `AC-M5D7QC3-007/008`.
- 선행 규범: Approved [QA 증거 규약 r2](2026-09-29-c4-qa-evidence-protocol.md), [발급 세대 증거](2026-09-30-c4-issued-generation-evidence-amendment.md), [C1 미호출](2026-09-30-c4-pre-c1-failure-evidence-amendment.md), [C2 미호출](2026-09-30-c4-pre-c2-failure-evidence-amendment.md), [C4 r4](2026-09-29-c4-r4-exact-implementation-amendment.md). 기존 문서 바이트와 기존 규칙은 보존한다.

## 정확한 공백과 허용 행

실제 Bridge는 C4 Executions CWT를 발급한 뒤에도 결과를 반환하기 전에 여러 원인으로 실패할 수 있다. 기존 두 미호출 개정은 각각 **해당 checkpoint callback 자체가 던진 예외**와 proof observer 미도달을 요구한다. 다음 원인은 그 조건을 충족하지 않는다. API 예외 또는 C1/C2 수치만으로 원인을 재분류하지 않는다.

| 변형 | 허용된 부모 사건 | checkpoint·도달 | 실제 원인 | C1/C2·prepared/C2 observer |
| --- | --- | --- | --- | --- |
| `ProjectionFailureAfterIssue` | `AC001_SingleRegisteredWitnessDamageClosesWithoutBegin`, `AC001_ActualBirthProjectionDamageClosesOnOriginalThread` | None/null/null, 해당 C4 checkpoint 미도달 | 원본 witness의 Root 한 필드 또는 BirthThreadProjection 한 필드 손상 뒤, CWT Add 이후 최초 projection 검증 실패 | 0/0·0/0 |
| `LifecycleFailureAfterReturnedCallback` | Play `AC008_ActualDisableDuringPendingOrFreshClosesOriginalCohort(false)` | C4/AfterRouterGuard/6, RequiredReached | callback에서 실제 Router Disable 후 정상 반환; 이어지는 원본 pair liveness/guard 검증에서 실패 | 0/0·0/0 |
| `PreparedProofRejectedAfterObserver` | `AC005_ActualNestedPreparedProofRootNullStopsBeforeFinalize` | None/null/null; 실제 prepared observer 도달1 | C1 반환→getter 정상→observer가 받은 **동일 실제 proof**의 `_root`만 null로 손상→후속 `ValidateExecutionDiskRow` 거절 | 1/0·1/0 |
| `ProofObservedCheckpointBeforeC2` | `AC008_ActualC4CheckpointFailureNeverRestoresAuthority(12/13)` 두 행만 | C4/해당 enum/12 또는 13, RequiredReached | getter/observer와 후속 disk 재검증 성공 뒤 해당 callback 자체가 던진 동일 예외 | 1/0·1/0 |
| `FreshPublishCheckpointBeforeResult` | `AC008_ActualC4CheckpointFailureNeverRestoresAuthority(17)` 한 행만 | C4/BeforeFreshHandbackPublish/17, RequiredReached | 실제 root lease 아래 C1 Busy/NoBarrier 정상 반환 뒤 callback 자체가 던진 동일 예외 | 1/0·0/0 |

이 표는 정확 일곱 **원인/경계 사건**만 다루며 부모 시험 수·ExpectedRows 수를 추정하지 않는다. Play fresh=true는 이미 결과가 발급된 별도 행이다. checkpoint 14..16·26과 C2 outcome 손상은 실제 receipt가 검증될 때 기존 `ReceiptWithoutExecutionResult`를 사용한다. checkpoint 18·27처럼 C4 Result/ResultRecord가 이미 발급된 경우 이 문서의 무결과 규칙을 사용하지 않는다.

`CheckpointFamily=None`인 세 사건도 실패 자체를 실제 관측하는 필수 행이므로 `ExpectedReach=RequiredReached`다. 그 행의 `Reached`는 해당 실패와 발급 CWT를 검증한 뒤 기록한다. `firstC4CheckpointReached=No`는 **C4 제어 callback만** 미도달이라는 진단이고 필수 행의 도달 상태를 `RequiredNotReached`로 바꾸지 않는다.

## 새 memory 세대 규칙과 공통 필수 조건

위 다섯 변형의 `GenerationRule`은 각각 정확 `{Rule:string}`이고 `ExpectedAuthorityCorrelation.memoryGeneration` 및 대응 canonical RequiredFact에만 `MemoryNotIssuedAfterProjectionFailure`, `MemoryNotIssuedAfterLifecycleFailure`, `MemoryNotIssuedAfterRejectedProof`, `MemoryNotIssuedAfterObservedProofCheckpoint`, `MemoryNotIssuedBeforeFreshPublish` 순서대로 대응한다. `executionGeneration=IssuedWithoutResult`의 기존 **전체** 조건을 먼저 통과해야 한다. 같은 원본 confirmed-key Executions CWT/readonly 발급·실제 원본 thread·Result와 ResultRecord 둘 다 null·catch/fault 이력을 대조한다. consumed 상태는 첫 변형 `BeforeConsume`, 나머지 네 변형 `AfterConsume`으로 사전 고정하고 실제 `ConsumedHistory` 또는 비소비 이력을 확인한다. C4 결과가 발급됐거나 CWT 원본 검증이 실패하면 수용하지 않는다.

actual memoryGeneration은 JSON null, canonical 값 `null`/certainty Unknown이며 checked=false, Outcome=null, Phase=null이다. 이는 이 C4 실행에서 C2 receipt가 발급되지 않았거나 C1 자체가 호출되지 않은 **동결 source 경계와 실제 계수**의 한정 진술이지 전역 memory 세대 부재나 정상 결과 인증이 아니다. c1Begin/c2Finalize/preparedObserver/c2Observer는 표의 exact 정수이며 freshReserve=0/freshComplete=0이다. 실제 제어 계수기는 Observed로, C1/C2/발급 지배는 SourceEstablished로 따로 기록한다. 단순 checkpoint 도달로 관측하지 않은 native callback을 0으로 채우지 않는다. 필수 native 관측은 실제 관측하고 비필수 행은 기존 정확 null/범위 진단을 따른다.

각 행은 실제 예외 종류, 그 원인과 반환/throw 순서, 실제 원본 root/marker/lease, guard 양측 상태, permanent fault, barrier, history, cleanup을 독립 검증한다. 새 규칙은 다른 canonical 값의 Unknown 면제가 아니다. C1 미호출 두 변형에서 proof=Unknown/certainty Unknown이며 준비 proof가 없었다고 `Absent`로 단정하지 않는다. `FreshPublishCheckpointBeforeResult`도 prepared observer에 도달하지 않았으므로 proof=Unknown/certainty Unknown이다. `ProofObservedCheckpointBeforeC2`는 실제 전달된 proof 참조와 원본 C1 result의 동일성을 관찰·Validate한 뒤 proof=SameActual/certainty Observed로 기록한다. `PreparedProofRejectedAfterObserver`는 **전달된 원본 proof 참조의 동일성만** proof=SameActual/certainty Observed로 기록하고 별도 거절 진단을 필수화한다. 이 행의 checked=false 및 proof 정상 Validate 실패를 유지하며 SameActual을 정상 proof authority로 해석하거나 `ActualTypedC1Row` 정상 장벽 증거로 사용하지 않는다. 장벽은 실제 marker/lease와 동결 source 경계 또는 same-case writer 관측으로 입증한다. 손상 후 proof를 정상 검증했다고 발행하면 실패다.

## 변형별 정확 진단

기존 `IssuedWithoutResult` 일곱 진단 뒤에 아래 진단을 표 순서로 추가한다. 이름·값·certainty는 RequiredFacts 전체 배열에서 정확 비교한다. 공통 마지막 `diagnostic.unreturnedSourceBoundaryVerified=Yes`는 SourceEstablished이고 동결된 Bridge, 두 부모 시험, 두 fixture의 실제 바이트 및 SourceManifest/InputPathList 결속이 지배한다.

| 변형 | 추가 진단 이름=값·certainty |
| --- | --- |
| `ProjectionFailureAfterIssue` | `diagnostic.unreturnedOrigin=ProjectionFailureAfterIssue`·Observed; `diagnostic.firstC4CheckpointReached=No`·Observed; `diagnostic.projectionField=Root` 또는 `BirthThreadProjection`·Observed; `diagnostic.projectionRejectedAfterCwtIssue=Yes`·Observed; `diagnostic.memoryGenerationObservation=MemoryNotIssuedAfterProjectionFailure`·SourceEstablished |
| `LifecycleFailureAfterReturnedCallback` | `diagnostic.unreturnedOrigin=LifecycleFailureAfterReturnedCallback`·Observed; `diagnostic.lifecycleCheckpointObserved=AfterRouterGuard`·Observed; `diagnostic.lifecycleCallbackReturned=Yes`·Observed; `diagnostic.routerDisableObserved=Yes`·Observed; `diagnostic.postCallbackLivenessRejected=Yes`·Observed; `diagnostic.memoryGenerationObservation=MemoryNotIssuedAfterLifecycleFailure`·SourceEstablished |
| `PreparedProofRejectedAfterObserver` | `diagnostic.unreturnedOrigin=PreparedProofRejectedAfterObserver`·Observed; `diagnostic.preparedProofSameReferenceObserved=Yes`·Observed; `diagnostic.preparedProofRootDamageObserved=Yes`·Observed; `diagnostic.preparedProofRevalidation=Rejected`·Observed; `diagnostic.memoryGenerationObservation=MemoryNotIssuedAfterRejectedProof`·SourceEstablished |
| `ProofObservedCheckpointBeforeC2` | `diagnostic.unreturnedOrigin=ProofObservedCheckpointBeforeC2`·Observed; `diagnostic.proofCheckpointObserved=AfterPreparedProofTake` 또는 `BeforeC2Finalize`·Observed; `diagnostic.proofCallbackExceptionSame=Yes`·Observed; `diagnostic.preparedProofValidation=Succeeded`·Observed; `diagnostic.memoryGenerationObservation=MemoryNotIssuedAfterObservedProofCheckpoint`·SourceEstablished |
| `FreshPublishCheckpointBeforeResult` | `diagnostic.unreturnedOrigin=FreshPublishCheckpointBeforeResult`·Observed; `diagnostic.freshPublishCheckpointObserved=BeforeFreshHandbackPublish`·Observed; `diagnostic.freshPublishCallbackExceptionSame=Yes`·Observed; `diagnostic.c1FreshOutcome=Busy`·Observed; `diagnostic.memoryGenerationObservation=MemoryNotIssuedBeforeFreshPublish`·SourceEstablished |

실제 no-result/fault/consume 진단 및 위 변형 진단을 보존한다. 진단별 source pointer는 actual source line/typed 원본의 결속이어야 하며 원시 주소·root·proof 본문을 출력하지 않는다. synthetic callback throw로 lifecycle/projection/proof 거절을 바꾸거나, 실제 실패를 예상 행에 맞춰 실행 후 재해석하는 것은 금지한다.

## 변경 및 검수 범위

독립 Luna 검수와 아스트라 승인 뒤에만 기존 배분된 새 Edit/Play 부모·두 fixture/공유 emitter/ExpectedRows 및 `c4-build-required-checkpoint-rows.ps1`, `c4-verify-required-rows.ps1`의 **위 다섯 규칙·진단 조건**을 구현한다. 추가한 각 부모 행은 실행 전에 exact case/Order/ExpectedCallCounts/RequiredFacts/적용성을 고정한다. 런타임8·C1/C2 알고리즘·API·observer·friend/asmdef·손상 seam·기존 회귀 시험은 변경하지 않는다. 실제 Unity/컴파일/큐는 최종 source·입력·selection·rows·plan 동결, 독립 검수, 아스트라의 별도 배분 전에는 시작하지 않는다.

국소 검증은 일곱 허용 사건의 정상 형태와 다른 부모/경계/enum 값, checkpoint 미도달·도달 위조, callback 반환과 throw의 뒤바뀜, CWT/원본 thread/fault/consume 불일치, 실제 C4 결과 발급, C1/C2/observer/fresh 수 오기, proof 정상/손상 판정 뒤바뀜, 비관측 proof를 Absent/SameActual로 둔갑, barrier/guard/cleanup 결손, 진단 이름·값·certainty 오류 및 동결 source 누락을 각각 거절한다. 합성 행 통과는 실제 원본 CWT 또는 Unity 결과를 대신하지 않는다.
