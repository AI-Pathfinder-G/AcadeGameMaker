# VD-03 M1 결정론적 피해·체력 코어 증적

- 검증일: 2026-08-27
- 구현: Terra
- 통합 판단: Sol
- 독립 검증: Luna
- 판정: **PASS — M1 Verified, P0/P1 없음**
- 관련 요구사항: `REQ-COM-001`, `REQ-COM-004`, `REQ-COM-005`, `REQ-COM-006`
- 부분 증적 인수 기준: `AC-COM-003`

## 수용 범위

- engine-free public `DamageRequest`·`DamageResult` ABI와 모든 ID·enum·amount 검증
- request ID → source ID → target ID → kind → amount의 canonical same-tick ordering
- 동일 ID의 서로 다른 payload, cross-tick duplicate와 encounter-lifetime tombstone
- `Duplicate → Invalid → TargetDead → Invulnerable → Applied` 처리 우선순위
- player 45-tick invulnerability의 age 44/45 경계, overkill clamp와 one-time dead state
- exact global tick, reset 후 exact `t+1`, 재등록 의무와 signed-int exhaustion
- 모든 checked 계산과 validation의 mutation 전 수행
- 30/60/144 render grouping에서 전체 내부 snapshot·result trace 일치
- Unity·physics·render time·scene object·검색 기반 identity 의존 없음

## 실행 결과

| 시험 | 결과 | SHA256 |
|---|---:|---|
| `TestResults/vd03-m1-combat-editmode-final2.xml` | 13/13 PASS | `2A51734CC875633C8D0527482D636E31C2AECA165A1E7998DCA5697261BE91A8` |
| `TestResults/vd03-m1-full-editmode-final.xml` | 71/71 PASS | `96B0E21D63628295E5BD421CF43902130DD9DD605DC38042866FDAB1F6DE2B76` |

## 핵심 소스 고정값

- `DamageContracts.cs`: `D2D5E4DFE5D80E4347869E470A26296671EF99214EE85EC6654DE55B102CC2D9`
- `CombatSession.cs`: `916682D9FFDBCE3083653495CBBA569C2DE290279AC4C6FCFAA46B582A6CB46A`
- `CombatSessionTests.cs`: `5D479DF658AE6FEF4C685D884EB71B67A60E5E0E0B4525FE4D9943AB2B236C84`

## 검토 이력과 통합 판단

첫 Unity 실행은 피해량 범위 위반이 정확히 `ArgumentOutOfRangeException`을 내는 데 테스트 두 곳이 상위 `ArgumentException`을 기대해 12/13이었다. 런타임은 유지하고 테스트 기대 타입을 교정해 13/13을 얻었다.

Luna의 첫 구현 검토는 encounter reset 뒤 재등록 없이 빈 batch로 tick을 전진시킬 수 있는 경로를 P1으로 판정했다. Terra·Sol은 reset 전용 재등록 필요 상태를 추가하고, 재등록 전 동일 next tick 원자 거부·tick 불변과 재등록 후 같은 tick 성공을 테스트했다. 재실행 13/13과 전체 EditMode 71/71 이후 Luna는 잔여 P0/P1 없음으로 최종 PASS했다.

Sol은 VD-03 M1을 수용한다. 이 증적은 순수 피해·체력 기반과 부분 `AC-COM-003`만 검증한다. 기본 공격 명중, 실제 적 유형, Unity 전투 phase, 무게 전이 enemy/boss consumer, 오르단, 보상·방 완료는 후속 Approved 계약 전까지 미완료다.
