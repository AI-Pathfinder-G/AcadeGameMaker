# B1 lifecycle 정리 원문 읽기 접근 보정

- 상태: Approved. 아래 독립 검토와 아스트라 한정 승인 기록에 따라 정확 두 파일의 테라 보정 및 명시 후속 관문만 허용한다. 구현·컴파일·제품 수용은 아니며 C1C 추가 권한과 분리한다.
- 사용자 산출물: 네 방 이동과 실패 후 거점 복귀에서 실제 전이 정리를 유지하는 현재 소스 빌드.
- 담당: 테라 구현, Root 최소 컴파일, 루나 독립 검수, 아스트라 명시 승인·통합.
- 근거: 최소 컴파일 `Library/b1-minimal-csc-r1/result.json` SHA `8AA8C4CE26253B19245AE153E93389F0932DAD1B69FADAD90FCF91A504735050`, 독립568 검수 `evidence/b1-final-source-review-luna6.v1.json` SHA `0259B549FF17B6DA525F5338FFD2003AD17C98F80C5873695E353D5D96452E22`. 지문은 Root 인계값이며 아래 실제 소스도 직접 대조했다.

## 원인과 정확 변경 후보

`Chapter1EncounterLifetimeOwner.ValidateLifecycle`의 `phase.Value.Outcome.RemovalResult`는 순수 Transfer의 내부 TransferTickOutcome/RemovalResult를 Input.Unity에서 접근한다. Transfer.Unity에는 기존 Input.Unity friend가 있지만 순수 Transfer에는 없다. CS1061을 멤버 부재로 보고 새 friend를 추가하거나 검사를 삭제하지 않는다.

대상 루트 `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/`에서 정확 기존 파일 둘만 변경한다.

1. `Runtime/Transfer/Unity/TransferSimulationDriver.cs`의 기존 `TransferPhasePublication` 안에 읽기 전용 `internal TransferCleared? LifecycleClear => Outcome.RemovalResult.HasValue ? Outcome.RemovalResult.Value.Cleared : null;`을 추가한다. TransferCleared는 이미 public readonly 값 타입이며 새 타입·필드·생성자·source 발급경로를 추가하지 않는다. 같은 publication의 실제 Outcome만 투영한다. 이름이 LifecycleClear라고 해서 lifecycle 유효성을 보증하지 않으며 caller의 이유/tick/source 검증은 필수다.
2. `Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs`의 ValidateLifecycle 한 소비점에서 `var clear=phase.Value.LifecycleClear;`를 읽는다. 이전 target이 비어 있지 않으면 `clear.HasValue` 및 `clear.Value.PreviousTargetId==_lifecyclePreviousTarget`을 요구한다. clear가 있으면 `Reason==reason` 및 `Tick==receipt.Tick`을 요구한다. phase 존재·tick·Input.LifecycleReason·Transition receipt·현재 router receipt 동일성·Baseline·ActiveTargetId 없음의 기존 모든 검사를 유지한다. absence를 성공 bool로 바꾸거나 기본 Clear를 만들어 넣지 않는다.

이 보정은 새 clear 발급권이 아니다. 최신 실제 BoundTransfer publication을 읽는 기존 경로를 그대로 유지한다. 신규 friend·assembly 참조·순수 Transfer 파일·다른 caller·C1C 함수 변경은0이다. 기존 B1 승인 경로 두 파일 내 접근 보정이지만 정확 새 서명이 없었으므로 독립 대조 및 아스트라 명시 승인 전 구현하지 않는다.

## 요구사항·검사·종료

REQ-C1B-EVIDENCE-001/ROOM-001/FAIL-001/RETURN-001에 결속한다. AC-C1B-EVIDENCE-001/INPUT-001/E2E-001/E2E-002 및 AC-C1B-ROOM-001/FAIL-001/RETURN-001의 기존 의미를 유지한다. 구현 보고는 기존 ROOM/FAIL/RETURN AC 누락을 보완하고 시험31개 미실행을 통과로 표시하지 않는다.

실제 선행은 독립 검수된568 변경 전 지문과 두 파일 원문 대조, 본 Review의 루나 검수·아스트라 Approved 전환이다. 승인 뒤 테라가 정확 두 파일만 변경하여 전후 byteLength/SHA·차분을 인계한다. 루나는 직접 getter가 같은 실제 Outcome만 읽고 모든 caller 검사가 유지됐는지 검수한다. Root는 기존 실패 원시 결과를 보존한 새 이름의 최소 컴파일을 한 번 수행하고 접근 오류 및 막혔던 후속 조립체 결과를 기록한다. 같은 오류에서 자동 반복0이다.

검사 연결은 기존 `Chapter1RoomPlanTests.RoomExitRequiresActualCompletion`, `Chapter1ExpeditionPlayModeTests.FailureCommitBeforeDestroy`, `SingleReturnForConcurrentCauses`, `FailedSaveBlocksReturn`, `FourRoomsThenBossWithCarriedHealth` 및 기존 B1 집중31·필수 회귀를 유지한다. 유효 실제 clear, 활성 대상인데 clear 없음, 다른 target/reason/tick 및 외부 publication 거절을 해당 기존 fixture에서 확인한다. 컴파일 통과는 runtime AC 통과나 전체 B1 수용이 아니다.

종료 조건은 두 파일 정확 차분·새 최소 컴파일 결과·루나 소스 검수·기존 미실행 검사의 실제 상태와 AC 추적을 인계하고 아스트라가 한정 판단하는 것이다. Unity 큰 실행·프로필·Q0·원격·C1C 공유 쓰기 권한은 추가하지 않는다.

## 아스트라 한정 승인 기록

2026-10-03 아스트라는 루나 독립 검토 `output/chapter1-playable-r1/evidence/b1-lifecycle-clear-projection-review-luna6.v1.json` SHA `E9AE54633A1CF722B1A5A5564A8EEFB0DCDDA03B812D31869715845E389F2086`를 직접 읽고 위 정확 두 파일·투영1·소비점1의 설계를 승인한다. 루나가 검토한 Review 원문 SHA `7C7527CC2E39E67E86111046B188D8B5944728B8D797D6C201031BA467D01DA3`와 P0=0/P1=1 기록을 보존한다. P1 `B1-LIFECYCLE-PROJECTION-P1-001`은 기존 REQ에 대응하는 AC-C1B-EVIDENCE-001을 위 추적 목록 및 아래 증거 연결에 명시해 아스트라가 닫는다. 루나가 보정 후 P1=0을 다시 보고했다고 주장하지 않는다.

AC-C1B-EVIDENCE-001은 같은 실제 완료 publication·receipt·tick의 원문을 읽는 검사에 결속한다. 기존 `Chapter1RoomPlanTests.RoomExitRequiresActualCompletion` 및 `Chapter1ExpeditionPlayModeTests.FailureCommitBeforeDestroy`의 실제 lifecycle 증거, 외부/누락/다른 tick·reason·target 음성 관측을 이 AC와 함께 실행 원장에 적는다. 이 보정은 기존 의미나 검사 수를 줄이지 않는다.

실행 순서는 바이트 전수 대조된 기존568 입력 재고정 → 별도 테라의 두 파일 보정·전후 지문/차분 → 루나의 실제 델타 독립 대조 → Root의 새 이름 최소 컴파일 한 번 → 결과 독립 검수 및 기존 미실행 집중31·필수 회귀·종단 관문이다. 실패 원시 결과와 이전568 코드 P0는 보존한다. 코드 보정과 새 컴파일·독립 검수가 실제 끝나기 전 접근 결함을 해결했다고 표기하지 않는다. 컴파일 후속 조립체가 통과해도 runtime AC나 B1 전체 수용은 아니다. 기존 ROOT/LUNA/테라의 담당을 바꾸지 않는다.

새 friend·assembly·순수 Transfer 수정·C1C 공유 쓰기·Unity 큰 실행·프로필·Q0·원격 권한은0이다. C1C UI/회수 초안은 별도 Review를 유지한다. 이번 승인 작업의 제품 소스 쓰기·컴파일·Unity·시험 실행은0이다.
