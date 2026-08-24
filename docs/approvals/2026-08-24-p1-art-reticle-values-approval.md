# P1 조준기 exact values 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Resolves: `OD-ART-001`
- Owner: Sol

## Approved boundary

- 모든 크기·두께·halo는 640×360 logical pixel 단위이며 output에서 point-filtered integer scale을 따른다.
- 비포착 gameplay reticle의 외접 경계는 9×9px, 1px W11 `#CAC9B8`이다.
- 유효 대상 포착 시 reticle core와 대상의 1px 외곽선은 같은 SimulationTick에 S02 `#F7FFFC`로 전환되고 reticle 외접 경계는 정확히 3 SimulationTick에 13×13px 확대를 완료한다.
- 포착 해제 시 대상 외곽선은 같은 SimulationTick에 제거되고 reticle core는 W11로 복귀하며 외접 경계는 정확히 6 SimulationTick에 9×9px 축소를 완료한다.
- 포착 대상을 바꿀 때는 13×13px 상태를 유지하며 축소·재확대하지 않는다.
- reticle 선은 1px core와 동일 hue의 바깥 1px halo를 사용한다. semantic core는 승인 HEX를 unlit로 유지하고 emission은 1.25를 넘지 않는다.
- 내부 상태 ring은 1px이며 S01 `#20E0D0` 연속선=`TransferReady`, S03 `#FFAA2B` 연속선=`Cooldown`, S04 `#FF3B45` 단절선=`RangeOrLineOfSightBlocked`다.
- reticle·ring·대상 외곽선은 점멸하지 않는다. `UIOnly`, `Cutscene`, `Transition`, `Ended` 진입 또는 letterbox/pillarbox pointer 진입 시 같은 tick에 숨긴다.
- 활성 전이 대상의 2px 이중 외곽선과 상태 우선순위는 기존 승인대로 유지한다.

## Verification link

- Requirements: `REQ-UX-010`, `REQ-UX-013`, `REQ-ART-014`, `REQ-ART-015`
- Acceptance criteria: `AC-UX-009`, `AC-UX-012`, `AC-ART-011`, `AC-ART-012`

## Consequence

`OD-ART-001`은 해결됐다. VD-07·VD-08·VD-09는 관련 계약의 Luna 독립 문서 검토를 거쳐야 `Approved` 전환할 수 있다.
