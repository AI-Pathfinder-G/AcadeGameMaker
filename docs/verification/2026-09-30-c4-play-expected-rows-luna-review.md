# C4 Play 예상 행 독립 정적 검토

## 범위와 지문

승인된 C4 r4, QA r2, 발급 세대·미반환 규칙과 Play 시험/fixture를 읽기 전용으로 대조했다. Unity 실행은 하지 않았다.

| 대상 | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs` | `26CBFE74189034091C9D09BE90038971B4FEC3989B82A00ADFC11A15E42775DF` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs` | `353F11612B96678A1BEE2FF01D5B2B02DDC5053866CB28B265F313D87E0B7A4F` |

fixture 내 ExpectedRowsJson은 35행, Order 148–182이다. 각 행에 canonical 사실 36개가 있고 nested 값과 `RequiredFacts`의 문자열 표현이 일치함을 정적으로 확인했다.

## 판정 및 보정

최초 대조에서 P1 한 건을 발견했다. C2 checkpoint 1–19 행(Order 162–180)이 모두 `guard.permanentFaultRecorded=No`로 기대했으나, C2의 각 checkpoint 실패는 `Fail(owner, ReloadRequired)`를 거쳐 `LatchResetCutoverFailure`와 실제 execution lifecycle fault 기록으로 이어진다. fixture의 `ObserveActualGuard`는 `FaultHistory` 존재를 영구 실패로 읽는다.

현재 지문에서는 해당 19행이 모두 `Yes`로 보정됐고, 나머지 행의 기대값은 그대로다. 이 P1은 현재 ExpectedRows에서 폐쇄됐다. 구현·Unity 실행 결과를 뜻하지 않는다.

중점 행 대조 결과:

- C2 checkpoint 19는 실제 memory 결과가 발급되지만 receipt 발급 전 실패하므로 outcome/phase `ReloadRequired`/`C2Invoked`, C2 결과 `SameActual`, receipt와 memory generation `Absent`가 일치한다.
- Busy/Stale 원본과 successor는 서로 다른 행(Order 155–158)으로 구분되고, successor에는 원본 행 ID 결속 사실이 있다.
- 반복 취소 원본 Busy 행(Order 159)은 첫 관측 뒤 Completed successor로 다시 관측되지 않는다. 본문은 successor 완료를 실제로 단언하며 원본 증거를 보존한다.
- lifecycle 미반환 행의 결과/phase는 미발급 상태로 두며 memory generation과 proof는 각각 승인 규칙에 맞춰 `Unknown`으로 둔다. checkpoint 6 뒤 liveness 거절을 관찰하고 execution fault를 기록한다.

## 한계

35행의 정적 literal 비교와 소스 경로 대조만 했다. 생성 도구의 전체 검증, 실제 NUnit/XML, Unity 실행 및 최종 통합 수용은 포함하지 않는다.
