# VD-03 M2A 기본 공격 코어 계약 Luna 사전 게이트

- 검증일: 2026-08-27
- 계약 소유·최종 승인: Sol
- 독립 검증: Luna
- 판정: **PASS — implementation-ready, P0/P1 없음**
- 관련 요구사항: `REQ-COM-001`, `REQ-COM-004`, `REQ-UX-007`, `REQ-UX-008`, `REQ-UX-013`
- 구현 후 부분 증적 대상: `AC-COM-003`, `AC-UX-007`, `AC-UX-012`

## 판정 요약

Luna는 engine-free target-acquired instantaneous shot, 공유 Q4096 aim, `BasicAttackPressed`, 독립 attack target selection, 6u·24px·18/26/4도 경계, 21-tick cooldown, miss/hit과 M1 `DamageRequest` 생성 계약을 VD-03·VD-07·SYSTEM-CONTRACTS·ADR-0019/0021에 대조했다.

첫 검토의 P1 세 건은 다음과 같이 닫혔다.

1. `BasicAttackPressed`를 SYSTEM-CONTRACTS의 정식 VD-07→VD-03 공개 명령으로 등록했다.
2. 구조 검증과 `StaleAim → InvalidAim → Cooldown → miss → hit` 의미 결과 우선순위, 모든 첫 semantic 결과의 sequence ID 소비를 고정했다.
3. 공격 전용 12u 제안을 철회하고 사용자 승인 공통 6u 경계로 복구했다.

Luna 재검토는 잔여 P0/P1 없음과 구현 준비 완료를 확인했다.

## 범위 경계

이 PASS는 순수 공격 선택·cooldown·damage-request builder만 허용한다. Unity target authoring, pose/camera capture, LOS query, fixed-phase batch merge, 실제 health 처리, enemy/boss/choice/lifecycle와 모든 시각 표현은 후속 Sol 계약 전까지 구현 금지다.
