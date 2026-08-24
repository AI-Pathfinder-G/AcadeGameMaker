# P1 포인터 조준 조작 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-PLAT-001`
- Decision: [ADR-0019](../adr/0019-pointer-aimed-sidescroller-controls.md)
- Owner: Sol

## Approval source

사용자는 중력 공간의 대상을 직접 가리키는 조작이 게임 주제에 더 적합하다고 판단하고 `포인터 조준형 횡스크롤 액션`을 승인했다.

## Approved boundary

- 키보드·마우스: WASD 이동, Space 점프, Shift 대시, E 상호작용, Q 선택 기술, 좌클릭 기본 공격, 우클릭 무게 전이·회수
- 게임패드: 왼쪽 스틱 이동, 오른쪽 스틱 조준, RT 공격, LT 전이, RB 선택 기술, A 점프, B 대시, X 상호작용
- JKL 전투, 클릭 이동과 바라보는 방향만을 쓰는 자동 타겟을 사용하지 않음
- 기본 공격과 무게 전이는 같은 포인터·스틱 조준 의도를 사용
- 선택 기술은 현재 활성 전이 대상을 사용
- 키보드·마우스와 게임패드는 같은 의미 명령과 게임 결과를 제공

## Remaining boundary

마우스·게임패드 타겟 판정은 후속 승인으로 확정됐다. runtime binding override와 저장 계약은 아직 열려 있다.
