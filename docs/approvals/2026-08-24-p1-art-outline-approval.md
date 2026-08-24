# P1 픽셀 윤곽선 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-ART-001`
- Owner: Sol

## Approved boundary

- 두께 단위는 640×360 logical pixel이며 output에서 point-filtered integer scale
- player·enemy·major NPC·핵심 충돌 실루엣의 normal outline은 8방향 1px W01 `#090D12`
- heroine도 같은 normal outline을 사용하고 전용색은 내부 강조에만 사용
- AimAcquired는 1px S02 `#F7FFFC` 단일 형광선으로 normal을 대체
- ActiveTransfer는 안쪽 1px S01 `#20E0D0`와 바깥 1px S02의 지속 2px 이중선으로 하위 상태를 대체
- priority는 `ActiveTransfer > AimAcquired > Normal`; outline은 점멸하지 않음
- background는 전경 분리면·충돌 경계만 outline하고 tile 내부 seam과 non-colliding decoration 전체 outline 금지
- occluder 뒤 target을 투시하지 않고 visible sprite pixel boundary에만 적용
- S02 역할은 AimAcquired와 ActiveTransfer가 공유하며 single/double outline count로 구별

## Verification link

- Requirements: `REQ-ART-014`, `REQ-UX-010`
- Acceptance criteria: `AC-ART-011`, `AC-UX-009`

## Remaining boundary

URP 2D light layer·intensity·blend와 reticle base/acquired pixel size·glow·animation을 확정하기 전에는 `OD-ART-001`을 해결 처리하지 않는다.
