# P1 포인터·스틱 타겟 판정 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-PLAT-001`
- Owner: Sol

## Approved boundary

- 공통 후보는 플레이어에서 6유닛 이내, `TransferLineOfSight`가 열린 단일 대상
- 마우스는 투영된 TargetAimShape 직접 hit 또는 1920×1080 기준 aim point 24px 이내 후보를 사용
- 마우스 정렬은 shape 내부 → screen distance → player distance → TransferTargetId
- gamepad는 오른쪽 스틱 0.20 이상에서 조준을 갱신하고 neutral 동안 마지막 유효 방향 유지
- gamepad 신규 후보 18도, 현재 강조 대상 26도 유지, 새 후보가 4도 이상 정확할 때 교체
- active transfer의 Transfer 입력은 조준과 무관하게 즉시 회수
- 유효 aim 이력이 없거나 후보가 없으면 `InvalidTarget`; facing fallback 없음
- mouse 정수 pixel, 직전 simulation camera pose, Q4096 aim vector와 양자화 정렬 key로 판정
- 방 이탈·컷신·실패·종료에서 aim·강조·press buffer 초기화
- 같은 입력에서 render FPS와 무관하게 AimSample·target ID·발동 tick exact match

## Determinism clarification

- range는 `Round(distanceSquared×1000, AwayFromZero)≤36000`, mouse halo는 `Round(pixelDistanceSquared, AwayFromZero)≤576`으로 포함 판정
- gamepad는 `angleKey≤180` 획득, `≤260` 유지, `newAngleKey+40≤currentAngleKey` 교체
- Q4096은 정규화 성분의 AwayFromZero 반올림·-4096~4096 clamp이며 영벡터는 invalid
- aim은 직전 완료 SimulationTick의 camera pose snapshot ID를 참조하고 render camera를 사용하지 않음
- LOS mask가 target 이전 또는 같은 경계에서 맞으면 차단하며 target 자체 collider는 mask에서 제외

## Remaining boundary

runtime binding override, 저장 schema·경로·원자 저장·시작 복구는 후속 승인으로 확정됐으며 `OD-PLAT-001`은 해결됐다.
