# C4 Play 집중 실행 실패의 제한 복구 계약

- 상태: **Approved — 아래 지정 구현만 승인, 실제 재검증·통합 수용 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 설계: `docs/proposals/2026-09-30-c4-play-r1-bounded-recovery-design.md`, SHA-256 `BD909D40F2F831DA494C9B3E456C374BC36E4EFB541EA85DEE1F106583853351`.
- 독립 설계 검수: `docs/verification/2026-09-30-c4-play-r1-bounded-recovery-luna-review.md`, SHA-256 `70C5A9E8B8AA84D8DF675161A65EC199BC3DCEAB9CFD84BDFFE02418E87A93CB`, P0/P1=0/0.
- 선행: Approved C4 r4, QA 증거 규약, 기존 한정 개정. 추적 `REQ-M5D7QC4-002/004/006/007`, `AC-M5D7QC4-002/004/007/008/009/010`.

계획 v8의 첫 `c4-r6-focused-edit`는 실제 Edit 139/139 및 필수 148/148 행이 통과했고, 입력 1008개 차이가 0이다. 다음 `c4-focused-play`는 XML 32건 중 19건 통과·13건 실패, 원시 종료 2로 큐가 중단됐다. Play XML SHA-256은 `92B143BEC6A26D89169CB07A5E1D8F9396362A3E647ED38EF532011CC92A0D7E`다. 기존 XML·원시 종료·입력 전후·큐 결과 v7은 불변으로 보존한다. 13건은 포인터 계측 11건, 반복 취소 시험 자료 1건, fresh 비활성화 시 양측 종료 결손 1건으로 분류한다. 앞선 Edit 통과는 Play 실패를 수용으로 바꾸지 않는다.

## 허용 구현

1. `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs`의 `FrozenNoSameCaseSavePointers`는 `ObserveActualBarrier`의 실제 관찰 오버로드 하나를 정확한 네 인자 선언과 본문으로 한정한다. 세 인자 전달 오버로드를 중복 선언으로 취급하지 않는다. 부모 사례 선언·fixture 실제 marker 조회행의 정확한 source pointer, 현재 소스 manifest의 SHA 확인, 같은 사례의 일반 `Save` 부재 확인, `NoSameCaseSaveObservation`의 제한된 `Unknown` 의미를 보존한다. 문자열 리터럴을 부모 선언으로 오인하지 않도록 실제 선언 시작을 확인한다. 포인터 판정을 건너뛰거나 기대 행을 완화하지 않는다.
2. `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs`의 `AC004_ActualFreshThenRepeatedCancelUsesLatestCursorAndPreservesHistory` 초기 fixture 호출 한 곳만 `Create(true,true)`로 바꾼다. 실제 기본 세 leaf를 최초 관찰 전에 준비하면서 초기 확인·커밋은 유지한다. 이후 실제 Busy·fresh, 첫 프레임 폐기, 새 요청, 세 번의 취소·재무장, 이력·참조 및 마지막 C1/C2 검증은 그대로 둔다. `DecisionRequired` 기대를 낮추지 않으며, 수정 후에도 분류가 다르면 별도 실패로 남긴다.
3. `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs`의 `RecordExecutionLifecycleFault`만 제한적으로 보정한다. 기존 원래 등록된 Adapter/Router의 `ExecutionRecord` 검색과 원래 Unity thread 확인을 먼저 유지한다. `e.Gate` 안에서 `RecordExecutionFault(e)`를 완료하고 잠금을 해제한 뒤, 실제 원래 실행쌍의 `!e.NativeClosed`인 경우에만 기존 `CloseOriginalExecution(e)`를 호출한다. 새 guard·권한·세션·C1/C2 호출·proof 소비·handback 발급을 만들지 않는다. `CloseOriginalExecution`의 선행 `NativeClosed/NativeFaultClosed` 기록과 양측 `ApplyExecutionGuard(Close)` 및 기존 latch를 재사용한다. 재귀 lifecycle 통지는 추가 native 종료를 반복하지 않아야 한다. 이미 닫힌 정상 C2 Completed 쌍에는 추가 native 종료를 하지 않는다. 원래 예외를 성공으로 삼키거나 가리지 않으며, 종료 자체가 실패하면 증거에 실패로 남긴다.

이 세 파일 밖의 Runtime, 일반 `InputRouter.OnDisable`, Adapter, C1/C2, Q-A/Q-B, generated 입력, 검증기 판정, 기존 Play/Edit 기대값, 사례 이름·순서·개수, 자산·설정은 수정하지 않는다. 구현자는 각 변경에 위 REQ ID를 표시한다. 구현 전후 Play 실패 13건과 기존 19건, Edit 139건의 필수 행 및 `AC-M5D7QC4-002/004/007/008/009/010`을 추적한다.

## 검증·동결 조건

Luna는 세 파일의 정확한 지문과 포인터 선택의 유일성, `Create(true,true)` 호출 한 곳, 원래 thread와 fault-before-native 순서, 잠금 밖 호출, 부분 guard·Busy handback·재귀 latch·이미 닫힌 C2 완료를 독립 정적 검수한다. 실제 Play 재실행에서 두 half terminal, Busy 결과의 forensic `Validate`, fresh 재사용 거절, 원래 예외·cleanup, C1/C2 호출 수 및 정상 완료 보존을 확인한다. 실패 사례가 뒤의 다른 불일치를 드러내면 별도로 분류하고 멈춘다.

정적 검수 뒤 바뀐 세 소스와 fixture의 현재 source manifest 참조를 새 원장에 동결한다. Play 선택의 부모 SHA, 필요한 Edit/Hub 선택·188 기대 행의 SourceFiles SHA, 전체 소스 manifest·입력 목록·실행 계획·큐 결과 경로를 새로운 버전으로 `CreateNew` 발급해 기존 9회 범위와 출력 불변 정책을 유지한다. 먼저 Play 32건을 별도 비수용 진단 실행할 수 있으나 그 결과를 최종 9회 큐 수용으로 대체하지 않는다. 최종 큐는 새 source/input 동결과 기존 필수 선택의 실제 통과 및 입력 전후 동일성을 확인해야 한다. 이전 실행 결과를 소급 수정하거나 13 실패를 정적 검토만으로 해소했다고 기록하지 않는다.
