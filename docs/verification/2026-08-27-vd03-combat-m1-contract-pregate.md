# VD-03 M1 결정론적 피해 코어 계약 Luna 사전 게이트

- 검증일: 2026-08-27
- 계약 소유·최종 승인: Sol
- 독립 검증: Luna
- 판정: **PASS — implementation-ready, P0/P1 없음**
- 관련 요구사항: `REQ-COM-001`, `REQ-COM-004`, `REQ-COM-005`, `REQ-COM-006`
- 구현 후 부분 증적 대상: `AC-COM-003`

## 판정 요약

Luna는 engine-free `DamageRequest`·`DamageResult` ABI, canonical request ordering, encounter-lifetime dedupe, 결과 우선순위, health·invulnerability·death, reset과 overflow-before-mutation 계약을 SYSTEM-CONTRACTS와 VD-03에 대조했다.

첫 검토에서 다음 P1 두 건을 제기했다.

1. 같은 request ID의 서로 다른 payload에서 `Duplicate` 결과가 어느 target ID를 담는지 불명확했다.
2. encounter reset 이후 exact next tick과 signed-int exhaustion 의미가 불완전했다.

Sol은 모든 결과가 각 originating request의 request·target·tick을 echo하도록 고정하고, canonical ordering은 ID 소비자만 결정하게 했다. 세션 시작 tick은 `0..int.MaxValue-1`, 모든 처리 전 `checked(tick+1)`, `int.MaxValue` atomic rejection, parameterless reset의 exact `t+1` 보존과 no-wrap 규칙을 추가했다. Luna 재검토는 두 P1 종료와 잔여 P0/P1 없음을 확인했다.

## 범위 경계

이 PASS는 `docs/specs/work-contracts/2026-08-27-vd03-combat-m1-damage-core.md`의 순수 피해·체력 코어 구현만 허용한다. 기본 공격 명중, Unity 전투 phase, 일반 적 AI, box impact sampling, 오르단 패턴, transfer removal producer, ChoiceSkill stagger, reward·room·run 사건은 후속 Sol 계약 전까지 구현 금지다.
