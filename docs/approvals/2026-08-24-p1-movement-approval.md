# P1 이동 계약 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Decision: `OD-MOV-001`
- Design: Terra
- Integration approval: Sol

## Approval source

사용자는 Sol이 제출한 `균형 정밀형 + 가변 점프 + 공중 대시 1회 + 대시 무적 없음` 방향을 승인하고, 이어서 60Hz 수치 정합성을 위한 초기 점프 속도 14.39와 Unity Rigidbody2D 중력 사용을 승인했다.

## Approved boundary

- run 8.0, 지상 가속/감속 70/90, 공중 가속/감속 40/20
- `Physics2D.gravity=(0,-9.81)`, 주인공 `gravityScale=3.1315`, 초기 점프 속도 14.39
- 기준 점프 3.25u·28틱, release 상방 속도 ×0.45, coyote/buffer 각 6틱
- 기준 낙하 상한 22.0
- 대시 5.0u·15틱, 종료 뒤 36틱 cooldown, 안정 접지에서만 공중 charge 1회 복구
- 대시 중 무적·중력·방향 입력 없음; 충돌은 이동만 종료하고 cooldown 유지
- 벽 슬라이드 6.0, 벽차기 X 9.5·Y 13.5, 같은 벽 재부착 잠금 6틱
- 의미 이동·조준 radial deadzone 0.20
- 승인된 무게 전이 배율 적용 뒤 점프 약 5.00u·43틱, 공중 가속 50, 낙하 상한 15.4; 대시·벽차기는 불변
- VD-01의 수치 허용오차와 VD-04의 네 snapshot×3회 기본 이동 완주 검증

## Gate consequence

`OD-MOV-001`은 해결됐다. VD-01·VD-04는 Luna 독립 검토와 Sol 상태 전환 전까지 `Review`를 유지한다.
