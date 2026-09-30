# C4 미반환 부모 및 Busy 출처 개정 설계 검토

- 검토 초안: `docs/specs/work-contracts/2026-09-30-c4-unreturned-parent-and-busy-source-amendment.md` SHA256 `9042EF2F72EAF1D17B48E57527B3F01C7C737F1D24A373B6A8054D7F89CD08B4`.
- 선행 규범: Approved `docs/specs/work-contracts/2026-09-30-c4-issued-unreturned-boundaries-amendment.md` SHA256 `0AEC1429EB90B924218C070DF2C07816E4B1C9CE1F9B0E648F71D6F912AD1B6D`; QA r2 certainty 규칙은 위 문서의 실제 다섯 callback 계수기와 `freshComplete`의 SourceEstablished 분리를 따른다.
- 확인한 실제 근거: `C4ActualExecutionEditFixtureV1.RootLeaseWorker`, `ProfileResetExecutionBridgeV1Tests.AC008_ActualC4CheckpointFailureNeverRestoresAuthority`, `ProfileRootOperationLockV1.Acquire`, `ProfileResetDiskTransactionV1.Begin`, `ProfileResetExecutionBridgeV1`의 checkpoint 17 분기.

## 판정

P0=0, P1=0 — 설계 문구 한정 검수 기준. 구현 승인이나 실제 실행 수용은 아니다.

17번 Busy 진단을 `Observed`에서 `SourceEstablished`로 낮추는 보정은 적절하다. 실제 C1 반환 객체나 C1 typed row가 callback 또는 결과 기록에 노출되지 않는다. 반면 시험 worker는 같은 임시 root로 실제 `Acquire`를 마친 뒤 `_ready` 신호를 보낸다. 생성자는 그 신호를 기다려야 반환하고, worker는 release 신호까지 실제 lease를 보유한다. 이 상태에서 원래 실행이 checkpoint 17에 도달했다는 같은 행의 증거를 결속하면 `actualRootLeaseHeldAtCheckpoint=Yes`는 관측 근거가 있다. 동결 `Begin`의 lease 경합 처리와 C4의 fresh 분기만으로 Busy 값을 `SourceEstablished`로 추론하는 것도 typed 결과 규칙을 우회하지 않는다.

설계 초안은 해당 관측을 위해 같은 root, 획득 완료, checkpoint 도달까지 미해제, finally 해제 및 Join을 같은 행에서 결속하도록 요구한다. 현재 시험은 checkpoint 17에서 row를 만들지 않고 worker도 콜백 작성 전에 지역 `using` 변수로 생성하므로, 구현 시 별도 예상행을 추가하고 worker 참조를 콜백에 안전하게 연결해야 한다. 초안은 새 Edit 부모/fixture/ExpectedRows 및 검증기 변경을 그 정확 범위에 허용하고, 임의의 부모·행 복제·일반 Busy 추론을 금지하므로 설계 누락으로 분류하지 않았다.

`AC005_ActualProofPairRootDamageStopsBeforeCutover` 추가는 기존 `AC005_ActualNestedPreparedProofRootNullStopsBeforeFinalize`와 동일 private helper를 쓰지만 별도 공개 NUnit 부모의 실제 실행이다. 초안은 각각의 정확 부모명, 행 ID, 순서와 별도 CWT/fault/proof 증거를 요구하고 행 수를 실제 부모별로 세며, 이외 이름과 손상은 거절하도록 범위를 닫았다. 따라서 사전 예상행 allowlist 개정은 필요하지만 새 손상 방식이나 권한 생성 경로의 확대는 아니다.

기존 verifier 바이트는 아직 초안 규칙을 반영하지 않는다. 현재 설정은 proof 손상 행의 부모를 기존 Nested 부모 하나로만 허용하고, checkpoint 17의 `c1FreshOutcome=Busy`를 `Observed`로 남긴다. 그러므로 이 초안만으로 도구나 실행이 적합하다고 말할 수 없다. QA 검증기·예상 원장에 두 제한 보정을 구현하고 독립 검수해야 한다. 실제 Unity 시험은 수행하지 않았다.
