# VD-02 Unity M2 계약 Luna 사전 게이트

- 검토일: 2026-08-26
- 검토자: Luna
- 판정: **PASS — M2 구현 시작 가능**
- 권위 계약: [`2026-08-25-vd02-weight-transfer.md`](../specs/work-contracts/2026-08-25-vd02-weight-transfer.md)
- 관련 요구사항: `REQ-WT-001`, `REQ-WT-003~008`, `REQ-MOV-004`
- 관련 인수 기준: `AC-WT-001`, `AC-WT-003~006`

## 검토 결론

Luna는 Sol이 동결한 M2 카메라 투영, 정수 조준 계산, 고정 틱 관측, LOS, Unity target, movement bridge 계약에서 P0 또는 P1 모순을 발견하지 않았다. Unity 6000.3.21f1과 현재 프로젝트 설정에서 구현 가능하므로 Sol은 M2 구현을 승인한다. 이 판정은 구현 결과의 검증 완료를 의미하지 않는다.

## 확인한 계약

- 18 PPU·640×360 framing에 맞춘 고정 orthographic projection과 inverse, endpoint grid, checked `RoundDivAway`
- 24회 정수 CORDIC, 고정 microdegree 표, Q4096 mouse normalization, 0·18°·26°·4° 경계 판정
- tick `t`가 완료된 `t-1` player/camera/target pose를 소비하는 실행 순서
- layer 8 `TransferLineOfSight`, mask 256, trigger 제외, origin/interior/endpoint 포함, self/child 제외, composite 및 64-hit 포화 규칙
- 유일한 새 public Unity component인 `TransferTarget`과 private serialized sink binding
- target mutation 전 movement preflight, 이후 non-rejecting reflection, 한 번의 final snapshot 반영으로 유지하는 원자성
- 기존 Unity 6000.3.21f1, disabled auto transform sync, 비어 있는 layer 8, 변경 없는 Physics2D gravity와의 호환성

## 구현 후 필수 증적

M2 완료 판정에는 다음 증적이 필요하다.

- 투영·역투영·signed half rounding·overflow-before-mutation EditMode 결과
- Q4096 normalization·CORDIC table/value 경계 EditMode 결과
- LOS origin/interior/endpoint/behind/trigger/self-child/target-child/composite/equal-hit/64-hit PlayMode 결과
- 세 target modifier ID, box apply/clear 복구, target rejection 원자성, movement Lightweight→Baseline 반영 결과
- lifecycle·active removal·same-tick removal/press 결과
- 같은 60 Hz 입력을 30/60/144 render grouping으로 실행한 동일 snapshot/event replay 결과

`AC-WT-002`의 실제 enemy behavior는 VD-03으로 유보한다.

## 구현 중지 조건

Unity linecast가 tolerance 없이 endpoint 계약을 재현하지 못하거나, CORDIC 경계값이 계약과 다르거나, registry가 unordered discovery를 요구하거나, target mutation 뒤 movement reflection이 실패할 수 있거나, 새 public runtime 계약·추가 project/package setting이 필요하면 즉시 구현을 중지하고 Sol에 상향한다.
