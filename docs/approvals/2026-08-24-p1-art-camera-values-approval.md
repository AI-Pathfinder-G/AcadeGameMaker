# P1 카메라 추적 수치 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-ART-001`
- Owner: Sol

## Approved values

- axis-aligned dead zone: 6×4 world units
- horizontal target: `|velocityX|≥2.0`에서 movement sign 방향 3.0u, 아니면 0
- vertical target: `velocityY≥4.0`에서 +1.5u, `velocityY≤-6.0`에서 -3.0u, 그 사이는 0
- target band 변화는 current offset에서 new target까지 12-tick linear transition; 전환 중 변화는 현재 값에서 재시작
- desired center follow cap: axis-independent max 0.5u/tick
- 처리 순서: anticipation offset → dead-zone correction → MoveTowards → room bounds clamp → 1/18u AwayFromZero snap
- same-tick hard snap: room entry, respawn, teleport, authored anchor enter/exit만 허용
- dash와 direction change는 hard snap 사유가 아님

## Remaining boundary

UI render scale은 후속 승인으로 확정됐다. palette·outline·URP 2D lighting, reticle의 base/acquired pixel size·glow·animation을 확정하기 전에는 `OD-ART-001`을 해결 처리하지 않는다.
