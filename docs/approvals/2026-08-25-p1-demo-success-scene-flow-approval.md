# P1 수직 데모 성공 장면 흐름 승인 기록

- Date: 2026-08-25
- Status: Approved by Sol under user-delegated orchestration authority
- Decision: `OD-SCENE-001`

## Approved success path

1. 중간보스 생명 상태가 확정되면 활성 전이를 정리하고 `Transition`으로 잠근다.
2. 아직 선택이 없으면 유담 선택 장면으로 진입한다. 취소는 장면을 닫지 않고 다시 선택 대기로 돌아가며 선택 확정 전 gameplay로 복귀하지 않는다.
3. 선택을 원자 저장한 뒤 해당 기술을 부여하고 비치명 봉쇄선 검수 구역으로 전환한다. 이전 실행에서 선택이 이미 확정된 경우 선택 장면을 반복하지 않고 저장된 기술로 봉쇄선 구역에 진입한다.
4. `BarrierCounterweight`에 전이를 성립시키고 부여된 기술을 한 번 성공시켜 봉쇄선을 통과해야 종결 trigger가 열린다.
5. 종결 trigger는 `DemoCompleted`를 한 번만 확정하고 `Cutscene`에서 세령의 추적자 첫 등장 장면을 재생한 뒤 `Ended`로 전환한다.

## Failure rule

- `RunFailed`, 거점 복귀, 선택 미확정, 봉쇄선 미통과에서는 세령 장면을 절대 재생하지 않는다.
- 선택 확정 뒤 실패하면 승인된 프로필 선택·기술은 유지하지만 세령 장면은 다음 성공 실행의 `DemoCompleted`까지 잠긴다.
- 세령은 전투 참여자나 선택 보상으로 등장하지 않고, 도언과 유담의 결과를 확인하러 온 독립 추적자로만 등장한다.

