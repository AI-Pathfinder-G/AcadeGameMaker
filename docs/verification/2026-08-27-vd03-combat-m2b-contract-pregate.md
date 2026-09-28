# VD-03 M2B Unity Combat Adapter 계약 사전 게이트

- Date: 2026-08-27
- Reviewer: Luna
- Result: PASS — 계약 기준 P0/P1 없음
- Scope: `docs/specs/work-contracts/2026-08-27-vd03-combat-m2b-unity-adapter.md`의 구현 전 독립 검토
- 검토 범위: Approved VD-02/VD-03/VD-07, `SYSTEM-CONTRACTS`, 검증된 VD-02 M2 및 VD-03 M1/M2A 계약·코드와의 정합성

## 판정

현재 M2B 계약은 구현 시작을 위한 독립 pre-gate를 통과한다. 계약 상태는 Sol이 `Approved`로 전환하기 전까지 `Review`로 유지되어야 하며, 이 문서는 M2B 구현 또는 Verified 완료를 승인하지 않는다.

- 고정 phase 순서와 one-sync receipt: PASS (`REQ-WT-006`, `REQ-COM-001`, `REQ-UX-007`, `AC-WT-003`, `AC-WT-006`, `AC-COM-003`, `AC-UX-007`)
- `CombatTarget`/`TransferTarget` 공동 저작 및 session 생성 전 검증: PASS (`REQ-WT-003`, `REQ-WT-005`, `REQ-COM-004`, `AC-WT-005`)
- M2A 결과의 단일 M1 batch 병합, 정렬·health 권한 보존: PASS (`REQ-COM-004`, `REQ-COM-005`, `AC-COM-003`)
- 사망 시 `t+1` 제거 병합과 same-tick 전이 보존: PASS (`REQ-WT-005`, `REQ-WT-006`, `REQ-COM-004`, `AC-WT-005`, `AC-COM-003`)
- immutable descriptor 재사용 reset 및 global tick 보존: PASS (`REQ-COM-004`, `AC-COM-003`)
- 신규 public surface가 `CombatTarget` 하나로 제한됨: PASS (M2B contract §Allowed/Forbidden)

## 이전 findings의 closure

1. **P1 거리 키 단위 모순 — 해결됨.** M2B `:79`가 승인 VD-02 `:71` 및 실제 `TransferSelector.PlayerDistanceKey`(`Assets/AcadeGameMaker/Runtime/Transfer/TransferSelector.cs:10-14`)와 동일한 `checked((dx²+dy²+500)/1000)`로 교정되었고 경계값을 `35,988 / 36,000 / 36,012`로 고정했다. 이는 M2A의 `PlayerDistanceKey≤36000` 및 `AC-WT-006`과 일치한다.
2. **P1 horizon preflight 부족 — 해결됨.** M2B `:99`가 검증된 모든 전투원의 `MaxRegisteredInvulnerabilityTicks`를 동결하고, M2A 소비 전 `checked(tick + max(21, MaxRegisteredInvulnerabilityTicks))`를 검사한다. 이는 M1의 실제 invulnerability preflight(`Assets/AcadeGameMaker/Runtime/Combat/CombatSession.cs:150-158`)와 `InvulnerabilityTicks=0/45/600` 경계를 덮으며 `AC-COM-003` 원자성을 보장한다. `:135`에 overflow/no-publication 증적도 명시되었다.
3. **one-sync 범위 모호성 — 해결됨.** M2B `:91`이 sync count를 `-200` authoritative aim capture로 한정하고, default-order Movement의 query-only 임시 pose sync는 receipt를 생성하거나 만족시키지 않는다고 명시했다. 이는 `SYSTEM-CONTRACTS:49`의 `-200 → -190 → Movement` 순서와 일치한다.

## 구현 단계에서 필요한 확인

다음은 잔여 P0/P1이 아니라 계약상 요구된 M2B2 증적이다: 공동 저작 mismatch가 두 session 생성 전에 실패하는지, 이미 큐에 있는 transfer input에 `t+1` 제거가 all-or-nothing으로 병합되는지, reset·death·receipt mismatch 및 horizon overflow가 M1/M2A/Transfer의 선행 publication boundary를 보존하는지, 그리고 `AC-UX-007`에 따라 동일 60Hz 기록이 30/60/144 render grouping에서 완전히 일치하는지 검증해야 한다.
