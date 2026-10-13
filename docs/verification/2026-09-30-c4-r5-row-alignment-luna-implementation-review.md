# C4 R5 행 정렬 구현 독립 정적 검토

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-r5-evidence-row-alignment-amendment.md` — SHA-256 `18CF0BC99C0709220022E3998D003264F8B95A89FD1F5D5AA28DE71805DBA88C`
- Edit fixture: `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs` — SHA-256 `A9677499FC8528656387CBA8926C030589A7E7522162C2031F90DAE4E1902F5B`
- 부모 시험: `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs` — SHA-256 `8299C67885CFA63BD3B1A09AAC355A669F21862A8DA5643A90B23EAB008979E6`
- 범위: 허용된 두 시험 파일의 정적 내용만 검토했다. Unity·컴파일·큐 실행은 하지 않았다.

두 파일은 승인 범위와 일치한다. 부모 시험에서는 `Create(false)`로 미커밋 거절과 비-fault를 확인하고, 실제 `CommitForExecution(f.Confirmed)` 반환이 같은 확인 요청인지 단언한 직후, 실제 실행 호출 전에 `CaptureActualCommitBaselineAfterUncommittedStart()`를 한 번 호출한다. 다른 동작이나 순서는 추가되지 않았다.

fixture의 기준점 함수는 처음 저장된 fixture 기준이 null임을 확인한 다음 실제 Owner의 private `_commitHistory`를 읽는다. 기록 형식이 정확한 내부 `CommitHistory`인지 확인하고, 기록이 존재하며 `Previous`는 null, `Confirmed`는 실제 fixture 확인 요청과 동일하고 `Reservation`은 null이 아님을 확인한 뒤 그 객체 참조를 fixture의 `_initialCommitHistory`에 저장한다. 이는 fixture가 기대 비교 기준을 보관하는 일이며 제품 Owner, 승인 상태, commit 기록을 쓰거나 만들지 않는다. 기존 `ObserveActualHistory`는 최종 Owner commit 연결에서 그 동일 객체 참조를 찾아 `history.originalCommitPreserved=Yes`를 기록한다. 이에 따라 기존 행의 Yes는 초안에서 설명한 의미로 검증된다.

공유 기대 행은 148개다. v4와 비교해 85개가 변경되고 63개는 동일하다. 변경은 승인된 C1 fault 55행, 정상 완료 24행, 최종 장벽 6행에 한정된다. `AC006_ActualOrdinaryWriterCannotCrossPreparedBarrier`의 실제 저장 차단 Yes와 증거는 유지되고 완료 뒤 상태만 NoBarrier다. CP17의 Bridge 경계 포인터도 `ObserveUnreturnedFreshPublishCheckpoint` 진단에만 사용되며, CWT·예외 동일성·결과 부재 검증을 대체하지 않는다. 변경하지 않은 필드와 사례 이름·순서는 그대로다.

fixture에는 동결 manifest v10 참조가 두 군데 있으며 부모 시험은 해당 fixture를 통해 사용한다. 런타임, 일반 저장 호출, 검증기 규칙, 선택된 시험 이름은 바뀌지 않았다. 다만 검수 대상은 아직 두 파일의 구현 단계다. 선택·행 자료의 새 지문, 동결 manifest·입력 목록·실행 계획의 완전한 결속과 후속 Unity 행 비교는 이 검토로 입증되지 않는다. 앞서 보존된 R5 실행의 86개 행 불일치도 소급해 해소된 것으로 취급할 수 없다.

판정: P0=0, P1=0. 승인된 1단계 두 파일은 정적 범위 검수를 통과한다. 동결, 실행 허가 또는 C3/C4 전체 수용 판정은 아니다.
