# P1 고정 16:9 화면 프레임 승인 기록

- Date: 2026-08-24
- Status: Approved by user with minimum-size amendment
- Partial decision: `OD-ART-001`; amends `REQ-PLAT-002` and pointer transform contract
- Owner: Sol

## Approved boundary

- viewport 중앙에 들어가는 가장 큰 integer scale의 640×360 gameplay rectangle 사용
- minimum window size는 640×360이며 scale 1 허용
- ultrawide는 좌우 pillarbox, narrow/tall viewport는 상하 letterbox
- 추가 world reveal, crop, stretch와 fractional scale 금지
- HUD와 interactive UI는 16:9 safe frame 안에만 배치
- 여백 mouse pointer는 nearest gameplay edge로 aim direction만 clamp
- 여백에서는 target acquisition, Attack·Transfer mouse press와 UI Click을 생성하지 않음
- gameplay rectangle 좌표·크기·integer scale을 simulation camera snapshot에 포함
- mouse AimSample에 gameplay-rectangle 내부 여부를 포함하고 normalized aim grid는 양끝을 정확히 0..1919·0..1079에 대응

## Remaining boundary

camera follow 방향은 후속 승인으로 확정됐다. look-ahead·dead-zone exact 수치, UI render scale, palette·outline·URP 2D lighting과 reticle pixel/glow 수치를 확정하기 전에는 `OD-ART-001`을 해결 처리하지 않는다.
