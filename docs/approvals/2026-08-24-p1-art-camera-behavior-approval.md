# P1 이동 예측형 고정 배율 카메라 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-ART-001`
- Owner: Sol

## Approved boundary

- gameplay camera는 fixed orthographic zoom과 player dead-zone follow 사용
- horizontal anticipation은 player movement direction을 사용하고 mouse pointer는 camera를 이동시키지 않음
- 상승은 작은 upward bias, 빠른 낙하는 더 큰 downward bias
- dash는 zoom·hard snap을 유발하지 않음
- camera center는 authored room bounds 안에서 clamp
- gameplay dynamic zoom 금지
- boss·choice·cutscene에서만 authored camera anchor 허용
- movement 완료 뒤 같은 60Hz tick에서 camera 계산, 1/18 world-unit pixel snap
- 완료된 simulation camera pose를 다음 tick aim에 사용하며 별도 render-only smoothing pose 금지

## Remaining boundary

dead-zone 크기, horizontal·vertical look-ahead 거리·전환 tick과 hard-snap 허용 생명주기, UI render scale, palette·outline·lighting과 reticle exact 수치를 확정하기 전에는 `OD-ART-001`을 해결 처리하지 않는다.
