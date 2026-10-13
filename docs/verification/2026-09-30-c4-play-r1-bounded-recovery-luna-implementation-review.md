# C4 Play R1 제한 복구 구현 독립 정적 검토

- 승인 기준: `docs/specs/work-contracts/2026-09-30-c4-play-r1-bounded-recovery-amendment.md`, SHA-256 `2F07A599BCA19A515F9FD2644124D23480676B8FC74A5F84207C729BCD0DFC94`.
- 구현 입력: Play fixture SHA-256 `215A813596E2C19B8FBAA49B070CC30A4AA448A057F618E0ECF5ADA34198D6E3`; Play 부모 SHA-256 `BC0C22518CE1A267547F72B758160610464E70D053D3A08F01CE2B80A5624BE9`; 실행 브리지 SHA-256 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`.
- 원래 실패 증거는 `artifacts/c4-focused-play.xml` SHA-256 `92B143BEC6A26D89169CB07A5E1D8F9396362A3E647ED38EF532011CC92A0D7E`로 보존돼 있다. 이 검토는 코드 정적 검사이며 Unity·컴파일·재실행을 하지 않았다.

## 판정

검토한 세 변경에서 P0 0건, P1 0건을 확인했다. 승인된 포인터 결속·fixture 전제·lifecycle 종료 범위와 일치한다. 이는 실제 재검증이나 C4 수용 판정이 아니다.

## 대조 결과

`C4ActualExecutionPlayFixtureV1.cs`의 `FrozenNoSameCaseSavePointers`는 이제 네 인자 `ObserveActualBarrier(EvidenceRow, ProfileResetExecutionResultV1, RootLeaseWorker, ConfirmedProfileResetRequestV1)`의 선언 시작을 정확히 한 번 찾고, 그 메서드 범위 안에서 실제 marker 조회행을 고른다. 세 인자 전달 오버로드와 구분되며 부모의 정확한 사례 선언, manifest 지문 확인, 같은 사례의 일반 `Save` 부재 검사 및 제한된 Unknown 관측을 유지한다. 따라서 이전 11건의 오버로드 이름 충돌 원인을 제거하는 변경이다. 해당 11건이 실제로 통과했는지는 아직 확인되지 않았다.

Play 부모에서는 반복 취소 사례의 초기화 한 곳만 `Create(true,true)`로 변경됐다. `Create`의 첫 인자는 초기 확인 결과의 커밋을 유지하고, 둘째 인자는 최초 capture 전에 실제 Previous/Temp 파일도 준비한다. 이후 `DecisionRequired` 확인, 실제 취소·재무장 반복, history·cursor·take 보존 및 C1/C2 검증은 유지된다. 기존 기대를 약화하지 않았다.

Bridge는 실제 GuardedAdapter 또는 GuardedRouter에서 원래 `ExecutionRecord`를 찾고 원래 thread를 확인한 뒤, `e.Gate` 안에서 fault history를 먼저 기록한다. 잠금이 끝난 후 `!e.NativeClosed`인 경우에만 기존 `CloseOriginalExecution(e)`를 호출한다. 이 경로는 실행 record에 보존된 원래 Adapter/Router 참조와 guard를 사용하고 C1/C2, proof 소비, fresh handback 발급을 호출하지 않는다. 기존 종료 함수는 native 종료 플래그를 먼저 세운 뒤 Adapter와 Router 양쪽을 시도하고, 각각 실패하면 기존 latch로 수습한다. 재귀 lifecycle 통지는 fault 기록만 반복하고 native 종료를 재진입시키지 않는 구조다. 정상 C2 Completed 결과는 기존 비-fresh 완료 경로에서 이미 `NativeClosed`가 설정되므로 lifecycle fault가 추가 native 종료를 일으키지 않는다. Busy 결과는 완료 이력과 분리된 fresh 미완료 상태이며, fault 기록 뒤 재사용 가능 판정에서 거절된다.

이 구조의 관문 조건은 실제 실행으로 확인해야 한다. 특히 Router 비활성화 직후 양측 terminal, Busy 결과의 `Validate()`와 forensic 이력 유지, fresh 재사용 거절, lifecycle 중복 통지에서의 종료 1회, C1/C2 호출 수 불변, 정상 C2 Completed 이후의 이력 보존이 필요하다. 기존 종료 함수가 각 half의 latch 예외를 흡수하므로 실행 행은 부분 폐쇄나 cleanup 실패를 성공으로 오인하지 않도록 양측 상태를 직접 확인해야 한다.

fixture는 현재 `artifacts/c4-frozen-source-manifest-v11.json`을 읽도록 바뀌었으나 이 manifest는 아직 발급되지 않았다. 이는 의도된 다음 동결 단계의 미충족 전제다. v11 원장, 이에 결속된 입력·선택·행·계획을 CreateNew 발급하고 이 세 파일의 실제 SHA를 고정하기 전에는 Play 재실행을 시작할 수 없다. 기존 실패 XML과 큐 결과를 변경하거나 소급 수용하지 않는다.
