# C3 lower opaque confirmed-request 경계 검토

- 날짜: 2026-09-29
- 상태: bounded counter-design — 구현·수용·C4 승인 근거 아님
- 범위: `ProfileNewGameConfirmationV1.cs`와 그 lower 단위 시험만

## 결론

Hub 형식을 Input.Unity에 전달할 필요가 없다. lower는 비공개 `ownerToken`과
`epochToken`, 실제 adapter/router 참조, 정규화 root, 양의 interaction/decision
generation만 opaque evidence로 묶는다. Q-B issuance, presenter topology, 실제 사용자
Confirm CAS는 다음 owner 단위가 증명해야 하며 이 lower 단위만으로 전체 C3 caller
authority를 주장할 수 없다.

각 `ProfileNewGameCaptureResultV1`의 private registry witness에는 기존 값 외에 exact
adapter/router와 confirmed-request mint CAS를 둔다. request는 오직 registry에
등록된 `Captured` 결과의 private `ProfileResetConfirmationCaptureV1`에서 한 번만
발급한다. 직접 생성한 결과, 같은 값의 복제, foreign pair/token/root, Busy,
Unreadable, reflection-corrupt capture는 발급할 수 없다.

## 확인 후 재관찰 경계

권장 lower API 의미는 다음 두 개로 제한한다.

```csharp
IssueNoConfirmationRequired(capture, adapter, router,
    ownerToken, epochToken, interactionGeneration, decisionGeneration)

ResolveConfirmedRecapture(displayedCapture, freshCapture, adapter, router,
    ownerToken, epochToken, interactionGeneration, decisionGeneration)
```

첫 경계는 actual private capture의 classification이 정확히
`NoConfirmationRequired`일 때만 그 capture의 mint CAS를 소비하고 opaque
`ConfirmedProfileResetRequestV1`을 발급한다. 두 번째 경계는 서로 다른 두 실제
capture 참조가 같은 owner/epoch/pair/root에서 발급됐는지 먼저 검증한다. complete
identity 비교는 세 leaf 각각의 role, name, presence, classification, byte length,
hash, revision 전체를 비교한다. revision이나 hash 하나, decoded 문서의 값 동등성,
caller boolean으로 대체하지 않는다. 각 capture의 기존 `Validate()`가 identity와
decoded projection의 일치를 먼저 닫아야 한다.

- identity가 같으면 fresh capture의 mint CAS를 소비하고 그 **fresh identity 참조**를
  request witness에 넣는다.
- identity가 달라도 fresh classification이 `NoConfirmationRequired`이면 stale
  displayed capture를 버리고 fresh capture에서 no-prompt request를 한 번 발급한다.
- 달라진 fresh classification이 meaningful/ambiguous이면 request를 발급하지 않고
  `FreshDecisionRequired`만 반환한다. fresh capture 자체는 다음 decision generation의
  표시 근거로 남는다.
- Busy/Unreadable은 이 비교 경계에 들어오지 않고 기존 typed capture 결과로 owner가
  처리한다.

반환 result는 closed disposition, fresh classification, 선택적 opaque request만
노출하며 identity/root/document getter를 두지 않는다. `Confirmed`만 request를
가지고 `FreshDecisionRequired`는 request를 가질 수 없다. 기존 `IdentityFor`를 새
owner가 호출해 비교하거나 request를 조립하지 않도록 owner 정적 시험에 금지 항목을
둔다.

## reciprocal execution-commit witness

lower request registry state는 `Issued -> CommitReserved -> Committed ->
ExecutorConsumed`와 terminal `Closed`로 닫는다. 현재 C3 단위가 추가할 수 있는 것은
다음 의미까지다.

1. `ReserveExecutionCommit`은 exact request/token/pair/generation을 검사하고
   `Issued -> CommitReserved` CAS를 한 번 수행해 opaque lower reservation을 돌려준다.
   이 단계는 identity 읽기 권한을 주지 않는다.
2. Hub owner는 reservation 뒤 confirm/cancel/retry/rearm과 Q-A/Q-B interaction을
   모두 닫고 자신의 `ConfirmedReady -> ExecutionCommitted` 전이를 완료한다.
3. `CompleteExecutionCommit`은 같은 request/reservation/ownerToken/epochToken/
   generation을 다시 검사해 `CommitReserved -> Committed`로 바꾼다. 성공 후에만
   owner가 동일 opaque request를 미래 executor에 반환한다.
4. 어느 단계든 예외·불일치·teardown이면 lower state를 `Closed`로 보내며
   `Issued`로 되돌리지 않는다. 부분 커밋은 request 재발급이나 identity 읽기를
   허용하지 않는다.

lower는 Hub 상태를 볼 수 없으므로 `Committed` bit 하나가 모든 상위 권한 폐쇄를
증명한다고 주장하면 안 된다. reciprocal proof는 lower reservation/committed
registry와 Hub owner의 exact state·capability closure 증거가 함께 성립할 때만
완성된다. owner 단위 시험은 호출 순서와 중간 fault의 fail-stop을 별도로 증명한다.

미래 C4 executor의 `IdentityForExecution`은 `Committed` 상태, exact pair/root/
generation과 request one-consumer CAS를 모두 검사한 뒤에만 원래 fresh identity
참조를 한 번 반환해야 한다. 그러나 이 추출 경계와
`ExecutorConsumed` 전이는 **현재 추가하지 않는다**. C4는 Review이므로 별도 Astra
승인 뒤 실제 guard와 C1 `Begin`을 구현할 때만 추가한다. 현재 lower 양성 시험은
confirmed 발급, commit reservation/completion, 중복·foreign·fault 거부까지만
증명하며 C1 호출이나 실행 report를 제조하지 않는다.

## C3L 결과 보존

기존 `artifacts/c3l-final-regression-evidence.json`, C3L 검수·수용 문서와 그 exact
source hash는 당시 Verified predecessor 증거로 그대로 보존한다. 새 lower source를
그 원장에 덮어쓰거나 과거 결과를 새 source 통과로 소급하지 않는다. strict current
source registry가 실제로 이 파일을 pin한다면 Astra의 별도 test-only successor
승인 후 exact 새 source/test hash와 독립 검수·재실행 근거를 append하며, old-or-new
fallback은 허용하지 않는다. pin이 없다면 registry 변경은 필요 없다.

새 lower 변경은 C3L 관찰 의미를 유지하더라도 현재 source의 focused lower 시험과
필요한 영향 회귀를 다시 실행해야 한다. 그 결과는 lower successor 증거일 뿐 Q-B,
presenter, owner/rearm, 전체 `AC-M5D7QC3-010`, C4를 수용하지 않는다.
