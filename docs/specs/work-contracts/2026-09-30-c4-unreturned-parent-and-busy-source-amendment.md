# C4 미반환 부모와 Busy 출처의 한정 개정

- 상태: **Approved — 부모 한 개와 Busy 출처 한 항목의 한정 증거 보정 구현 승인, 실제 실행·통합 수용 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 승인 근거: [독립 루나 설계 검수](../../verification/2026-09-30-c4-unreturned-parent-busy-design-luna-review.md) SHA `E455AE2922EC6099B1FA2526A4918D8B6C95B117233917960E8C027D2183BC7B`, 정적 P0/P1=0/0. 검수 원문 SHA `9042EF2F72EAF1D17B48E57527B3F01C7C737F1D24A373B6A8054D7F89CD08B4`는 `2026-09-30-c4-unreturned-parent-and-busy-source-reviewed-draft.md`에 정확 보존했다. 본문 승인 전 문장은 당시 이력이며 실제 Unity 실행은 별도 배분이다.
- 추적: `REQ-M5D7QC4-001/003/005/007`, `AC-M5D7QC4-003/005/008/009/010`.
- 선행 규범: Approved [발급 후 미반환 경계](2026-09-30-c4-issued-unreturned-boundaries-amendment.md) SHA `0AEC1429EB90B924218C070DF2C07816E4B1C9CE1F9B0E648F71D6F912AD1B6D`, [QA 증거 규약 r2](2026-09-29-c4-qa-evidence-protocol.md), [C4 r4](2026-09-29-c4-r4-exact-implementation-amendment.md). 선행 바이트와 나머지 일곱 경계 규칙은 보존한다.

## 실제 공백

`AC008_ActualC4CheckpointFailureNeverRestoresAuthority(17)`은 실제 root lease 작업자가 원본 root의 파일 lock을 보유한 동안 C4가 `BeforeFreshHandbackPublish=17`에 도달한다. 동결 C1 `Begin`은 lock 획득 실패를 `Busy/NoBarrier` 결과로 반환하고 C4는 그 결과를 지역 변수에서 분기한다. 그러나 checkpoint17 callback에 C1 반환 객체가 전달되거나 C4 Result/ResultRecord로 등록되지는 않는다. 따라서 해당 행의 `diagnostic.c1FreshOutcome=Busy`를 **Observed**라고 기록할 직접 관측은 없다. 실제 lock 보유와 checkpoint 도달을 관측하고, 동결 C1/C4 분기에서만 Busy를 추론한다.

한편 공개 `AC005_ActualProofPairRootDamageStopsBeforeCutover`는 허용된 `AC005_ActualNestedPreparedProofRootNullStopsBeforeFinalize`와 정확 같은 private `AssertActualPreparedProofRootDamage()` 본문을 호출한다. 다른 손상·실행 seam은 없다. 선행 허용 부모 목록은 두 번째 공개 시험 이름을 포함하지 않아 같은 실제 origin을 증거 행으로 사전 선언할 수 없다.

## 정확 두 보정

1. 선행 `PreparedProofRejectedAfterObserver` 변형의 허용 부모에 `AC005_ActualProofPairRootDamageStopsBeforeCutover`를 추가한다. 두 공개 시험은 각각 자기 NUnit 부모명/행 ID/Order와 별도 실제 실행·원본 CWT/fault/proof 손상·observer1/C1=1/C2=0 증거를 갖는다. 같은 private helper 호출이라는 source 동일성은 실제 결과 공유 또는 행 복제를 뜻하지 않는다. 일곱 **경계 유형**은 그대로이고 행 수는 실제 두 부모를 각각 센다. 이외 부모 이름과 손상 필드는 허용하지 않는다.
2. 정확 `AC008_ActualC4CheckpointFailureNeverRestoresAuthority(17)` 한 행에서 `diagnostic.c1FreshOutcome=Busy`의 RequiredCertainty와 실제 Fact.certainty를 `SourceEstablished`로 바꾼다. 값 Busy는 바꾸지 않는다. `diagnostic.actualRootLeaseHeldAtCheckpoint=Yes`·Observed를 그 직전에 필수로 추가한다. 실제 `RootLeaseWorker`의 획득 완료 신호·동일 root·checkpoint17 도달까지 미해제·finally 해제/Join을 같은 행에서 확인하고, 동결 `ProfileRootOperationLockV1.Acquire`의 경합 Busy→동결 `ProfileResetDiskTransactionV1.Begin`의 Busy/NoBarrier 반환→동결 C4의 `fresh` 분기→checkpoint17 순서를 정확 source binding으로 검증한다. actual C1 반환 객체·generation·proof를 관측했다고 발행하지 않는다. `diagnostic.unreturnedSourceBoundaryVerified=Yes`는 SourceEstablished를 유지한다.

새 `actualRootLeaseHeldAtCheckpoint`는 관측된 실제 worker 경합의 범위만 나타내고 C1 typed row의 검증이나 일반 writer 차단 권한을 대체하지 않는다. 기존 `MemoryNotIssuedBeforeFreshPublish`의 memoryGeneration null/Unknown, executionGeneration IssuedWithoutResult 원본 CWT, Result/ResultRecord null, C1=1/C2=0·preparedObserver=0/c2Observer=0·freshReserve/Complete=0, proof Unknown/checked=false, 원본 guard/barrier/history/cleanup과 정확 진단·순서는 유지한다. C4 12/13, Play lifecycle 및 기존 C1/C2 규칙에는 이 certainty 완화를 적용하지 않는다.

승인 뒤 변경 허용은 배분된 새 Edit 부모/fixture/ExpectedRows와 `c4-verify-required-rows.ps1`의 정확 두 보정뿐이다. 해당 source와 C1/lock source는 최종 SourceManifest.Files 및 InputPathList에 정확 바이트로 결속한다. 런타임8·C1/C2/lock 알고리즘·공개 API·observer·손상 seam·기존 시험은 바꾸지 않는다. 국소 verifier는 wrapper 외 부모 위조, C4 17 이외 Busy SourceEstablished, 미획득·이미 해제된 lease, 서로 다른 root, checkpoint 미도달, Busy를 Observed로 오기, 추가 진단 누락·certainty 오류, C1 result 발급 주장 및 기존 필수 조건 결손을 각각 거절한다. 합성 국소 검사만으로 실제 Unity·CWT 관측 또는 통합 수용을 주장하지 않는다.
