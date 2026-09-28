# P1 AimArc 형상·생명주기 승인 기록

- Date: 2026-08-25
- Status: Approved by Sol under user-delegated orchestration authority
- Decision: `OD-AIMARC-001`
- Basis: ADR-0021 and approved 640×360 logical canvas

## Approved contract

- AimArc의 중심은 완료된 SimulationTick의 player aim origin을 1/18u 격자로 snap한 위치다.
- 반원은 조준 방향을 중심으로 `-90°..+90°`, 반지름 18 logical px, 두께 1 logical px다.
- 방향 화살표는 반원 중심선에서 시작해 반지름 18px 지점부터 27px 지점까지 이어지는 9px shaft와 5×5px head를 사용한다.
- 기본 선은 W11, 유효 대상 포착은 S02이며 reticle과 같은 tick에 바뀐다. halo·점멸·추가 의미색은 사용하지 않는다.
- 차지는 화살표 내부를 꼬리에서 촉 방향으로 16단계 floor 양자화해 채우며 색 변화만으로 진행량을 표현하지 않는다. 비차지 상태는 빈 내부를 유지한다.
- `GameplayEnabled`이고 유효한 aim sample이 있을 때만 표시한다. UIOnly·Cutscene·Transition·Ended·화면 여백·aim invalid에서는 같은 tick에 arc·arrow·fill을 모두 숨긴다.
- mouse와 gamepad는 같은 Q4096 aim direction과 같은 completed simulation pose를 사용한다. render FPS와 render smoothing은 geometry를 바꾸지 않는다.

활 무기와 실제 곡사 탄도는 ADR-0021에 따라 후속 범위이며 수직 데모 AimArc 구현에 포함하지 않는다.

