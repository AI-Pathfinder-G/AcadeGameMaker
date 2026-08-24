# P1 URP 2D 조명 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-ART-001`
- Owner: Sol

## Approved boundary

- light ownership layers: BackDecor, WorldGeometry, Actors, GameplaySemanticFX, UI, FrontOccluder
- Hub world W11 intensity 0.85 / actor W11 1.00
- Expedition world W10 intensity 0.75 / actor W11 0.95
- Boss world W09 intensity 0.65 / actor W10 0.90
- gameplay global values are minimums; cutscene override는 control 반환 전에 복구
- warm fixture W18 max 1.10 radius 5u; cold fixture W10 max 0.85 radius 7u
- warning fixture는 semantic red 대신 W16 사용
- environment local overlap max 3, combined contribution cap 1.35
- decorative modulation은 ±8%, max 3Hz; full-off blink 금지
- WorldGeometry·large machinery only hard pixel-aligned shadow, attenuation max 55%
- actor·enemy·transfer target의 shadow 완전 소실 금지
- GameplaySemanticFX·UI는 unlit; semantic core pixel은 approved HEX, emission max 1.25와 no hue rotation
- vertical demo sprite normal map과 soft shadow blur 금지

## Verification link

- Requirement: `REQ-ART-015`
- Acceptance criterion: `AC-ART-012`

## Remaining boundary

reticle base/acquired pixel size·glow·animation을 확정하기 전에는 `OD-ART-001`을 해결 처리하지 않는다.
