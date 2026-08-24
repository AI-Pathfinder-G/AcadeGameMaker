# P1 18 PPU·2560×1440 출력 기준 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-ART-001`; amends output portion of `REQ-PLAT-002`
- Owner: Sol

## Approved boundary

- environment·tile·pixel-art VFX 공통 밀도는 18 pixels per unit
- native 18×18px tile은 1×1 world unit
- 16×16 등 비18px source는 bilinear automatic resample하지 않고 padding·crop·redraw 계획 필요
- 최종 output baseline은 16:9 2560×1440, windowed·borderless fullscreen 지원
- 기존 1920×1080 output baseline은 본 승인으로 대체
- targeting은 기존 판정 비율을 보존하기 위해 1920×1080 normalized aim grid와 24 normalized-pixel halo를 유지; 2560×1440 output에서 32 output pixels

## Remaining boundary

Pixel Perfect Camera의 internal render canvas, camera framing·crop, palette·outline, URP 2D lighting과 reticle의 정확한 pixel size·glow를 확정하기 전에는 `OD-ART-001`을 해결 처리하지 않는다.
