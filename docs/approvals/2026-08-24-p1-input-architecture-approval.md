# P1 입력 구조 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-PLAT-001`
- Owner: Sol

## Approval source

사용자는 `Input System 1.20.0 단독 + 생성 C# wrapper + InputRouter + Gameplay/UI 두 action map` 구조를 승인했다.

## Approved boundary

- `com.unity.inputsystem` 1.20.0 exact lock
- Active Input Handling은 Input System Package (New)만 사용
- 구형 Input Manager, Both 모드와 `PlayerInput.SendMessage` 금지
- 단일 `GameInput.inputactions`와 생성 C# wrapper
- 단일 `InputRouter`가 실제 기기 callback을 의미 명령으로 버퍼링
- 의미 명령은 다음 60Hz `SimulationTick`에서 최대 한 번 소비
- action map은 `Gameplay`와 `UI` 두 개뿐이며 `InputMode` 경로만 map 활성 상태를 변경
- control scheme 변화와 기기 연결 상태는 표시용이며 게임 시뮬레이션을 변경하지 않음

## Remaining boundary

포인터 조준 역할 배치는 후속 승인으로 확정됐다. 마우스·스틱 타겟 보정과 유지, runtime override 정책, 저장 schema와 손상 복구를 확정하기 전에는 `OD-PLAT-001`을 해결 처리하지 않는다.
