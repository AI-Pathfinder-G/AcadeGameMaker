# P1 UI·조준 피드백 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-PLAT-001`, visual values remain under `OD-ART-001`
- Owner: Sol

## Approved UI boundary

- Navigate: WASD·방향키 / XInput D-pad·왼쪽 스틱
- Point·Click·ScrollWheel: mouse
- Submit: Enter·Space / gamepad A
- Cancel: Esc / gamepad B
- Pause: Gameplay의 Esc / gamepad Start
- Gameplay/UI map은 상호 배타적이며 UIOnly는 focus navigation을 사용하고 gamepad virtual mouse를 만들지 않음
- 일시정지 최상위에서 Cancel은 플레이 복귀, 하위 화면에서는 상위 화면으로 한 단계 복귀

## Approved aim feedback boundary

- Gameplay mouse pointer 위치를 총구의 화면 공간 조준 reticle로 표시
- 유효 대상 포착 시 reticle 확대와 해당 대상의 형광 외곽선을 동시에 표시
- 조준 포착은 공격 조준을 의미하며 무게 전이 가능 여부와 독립적으로 유지
- reticle 내부 ring은 청록 연속선=전이 가능, 주황 연속선=Cooldown, 적색 단절선=거리·LOS 차단
- 활성 전이 대상은 지속 이중 외곽선으로 별도 표시
- UIOnly에서는 gameplay reticle·대상 외곽선을 숨기고 일반 UI cursor 표시
- Cutscene·Transition·Ended에서는 gameplay pointer feedback을 표시하지 않음

## Remaining boundary

reticle의 정확한 기본/포착 크기, 상태색의 RGB·발광 강도, 점멸·보간 방식은 `OD-ART-001`에서 고정한다. runtime binding override 범위·충돌 정책은 후속 승인으로 확정됐으며 저장 schema·손상 복구를 확정하기 전에는 `OD-PLAT-001`을 해결 처리하지 않는다.
