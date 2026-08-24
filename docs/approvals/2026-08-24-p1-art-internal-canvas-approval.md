# P1 640×360 내부 픽셀 캔버스 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-ART-001`
- Owner: Sol

## Approved boundary

- gameplay world logical pixel canvas와 Pixel Perfect Camera reference resolution은 640×360
- 2560×1440 output에서 nearest-neighbor 정확히 4배 확대
- 1920×1080은 3배, 1280×720은 2배 정수 확대 가능
- 18 PPU 기준 frame은 약 35.56×20 world units
- pixel-art world·URP 2D lighting·world VFX는 내부 canvas에서 합성 뒤 정수 확대
- fractional pixel sampling과 bilinear world upscale 금지

## Remaining boundary

16:9가 아닌 viewport의 crop·letterbox, camera follow·look-ahead·dead zone·room bound, UI render scale, palette·outline·lighting과 reticle 수치를 확정하기 전에는 `OD-ART-001`을 해결 처리하지 않는다.
