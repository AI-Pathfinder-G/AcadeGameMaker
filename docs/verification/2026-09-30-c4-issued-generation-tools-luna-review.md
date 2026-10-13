# C4 미발급 세대 도구 독립 검토

- 검토자: GPT-6 Luna, 2026-09-30.
- 범위: 승인 규격과 두 PowerShell 도구의 정적 대조, 구현자 국소검사 자료 확인. 런타임·fixture emitter·Unity 실행의 수용 판정은 포함하지 않는다.
- P0: 0.
- P1: 1 — 국소 회귀행의 잘못된 진단값 검증이 승인 규격이 요구하는 개별 음성 조건을 모두 덮지 않는다.

## 대조 결과

승인 규격 [세대 증거 개정](../specs/work-contracts/2026-09-30-c4-issued-generation-evidence-amendment.md)은 기존 `Positive/Absent/Unobserved` 규칙을 유지하고 `IssuedWithoutResult`와 `ReceiptWithoutExecutionResult`를 지정된 두 경로에만 허용한다. 원본 execution CWT, 발급 association/thread, 결과·결과기록 부재, consume 및 fault 이력과, 실제 receipt의 CWT·정상 Validate·원본 session/pair 검증을 서로 구분한다. receipt 발급 경로는 관찰자 호출 및 결과 검증과 분리하며, capability 경로는 관측했다고 주장하지 않고 SourceEstablished 진단으로 남긴다.

`c4-build-required-checkpoint-rows.ps1`는 기존 검증 라이브러리를 불러 공유 `C4-ExpectedRow`를 사용한다. 새 규칙 경로, checked=false 및 null outcome/phase, C1/C2 호출 수, receipt 출처와 관찰 상태 조합, 진단 값·certainty·순서를 행 계획에서 고정한다. `c4-verify-required-rows.ps1`는 JSON 정수의 Int64 제약을 적용하고, 두 규칙의 경로·양수 generation·미확정 상태·필수 진단을 검사한다. terminal facts는 36개 canonical prefix와 계획된 진단 전체를 순서·개수·값·certainty까지 비교하므로 추가·누락 진단이 통과하지 않는다. 기존 positive 규칙도 `checked=true`와 CWT 관측을 요구한다. 검토한 두 도구 본문에서는 승인된 범위를 넘는 새 issuer, 런타임 API, CWT 조회 권한 또는 손상 주입 경로를 찾지 못했다.

정확 지문은 다음과 같다.

- 행 생성기: `06B484BD4CF5815B03D73D4AEDE0958C0CAF8A1DE4450F42411CF5F7A868F338`.
- 행 검증기: `57F89ED5BD7D4BF004693BFCEFE0E4A0B10D0B421A151413364E90054476BD5C`.
- 승인 개정: `FF88BFC0471FB48C2828765A1C6FBD0F3913F0DD123CDEB16BCBADD4F8F2DCB6`.
- 구현자 국소검사 설명: `B4BA473B9A0A2B9A7B227A6CE5F189EF5BBB3DF93AAB9D35D48EFCAD11E1FC58`.
- 구현자 국소검사 JSON: `A3D51D15F14EA467C369450F3517EBB362A64A8062D7B4E53149293B3637BBDF`.
- 구현자 국소검사 스크립트: `63D96EFCA0D1F1679F9EB1D855823AADDB660342A92FCA3C01268EFCDA265116`.

## 남은 보정

구현자 JSON은 33/33 기대값 일치, `SyntheticLocalOnly=true`, `ActualUnityExecuted=false`, `WholeAccepted=false`를 기록한다. 이는 helper 함수의 합성 입력 검사 결과다. 실제 CWT 원본, fault 사건, 영수증 Validate 및 source 지배를 입증하는 실행 자료가 아니다.

승인 규격의 마지막 문단은 원본 CWT 존재, consume 상태, 실제 실패·fault, receipt 원본 발급 및 SourceEstablished 경로 등 조건의 누락이나 잘못된 값을 각각 거부하는 국소 검증을 요구한다. 현재 29개 음성 중 진단 누락 13개와 일부 값 변조 6개는 있으나, 잘못된 값에 대한 개별 음성행은 `executionGenerationSameActualCwt`, `executionConsumeState`, `executionFailureObserved`, `memoryGenerationSameActualCwt`, `memoryReceiptIssuePathSourceEstablished`에 없다. 따라서 33/33은 현재 준비된 사례가 기대와 맞았다는 사실만 보여 주며, 위 다섯 값의 변조 거부 회귀까지 입증하지 않는다. 최소 보정은 각 진단 값을 `No` 또는 허용 집합 밖 값으로 바꾼 음성 사례를 추가하고, receipt source 진단은 certainty 및 값 검사를 각각 분리해 확인하는 것이다. 기존 시험을 고치거나 추가 실행한 결과는 이 검토에 없다.

이 도구 검토는 생성기·검증기 경계에 한정된다. emitter가 실제 원본 증거를 올바르게 관찰하는지, 동결된 C4 source가 그 관찰을 지배하는지, 새 fixture와 실행 결과가 승인 규격을 만족하는지는 별도 정적 검수 및 Unity 실행 전까지 미확인이다. 전체 C4 또는 C3 선행 수용을 뜻하지 않는다.
