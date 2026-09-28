# VD-03 M2A 결정론적 기본 공격 코어 증적

- 검증일: 2026-08-27
- 구현: Terra
- 통합 판단: Sol
- 독립 검증: Luna
- 판정: **PASS — M2A Verified, P0/P1 없음**
- 관련 요구사항: `REQ-COM-001`, `REQ-COM-004`, `REQ-UX-007`, `REQ-UX-008`, `REQ-UX-013`
- 부분 증적 인수 기준: `AC-COM-003`, `AC-UX-007`, `AC-UX-012`

## 수용 범위

- `BasicAttackPressed`의 단일 engine-free public ABI와 0..`long.MaxValue` ID 검증
- 6u·24px·18/26/4도 후보 경계, LOS·attackability·canonical ID 정렬
- exact tick 및 observation/sample/press/duplicate preflight
- `StaleAim → InvalidAim → Cooldown → fired miss → fired hit` 우선순위
- 모든 semantic 결과의 sequence ID 소비, 21틱 cooldown과 hit/miss 동일 경계
- M1 `DamageRequest`의 exact request ID/source/target/amount/kind/tick 생성
- checked global/cooldown overflow의 publication 전 원자 거부
- 30/60/144 render grouping의 selection·attempt·request·전체 snapshot trace 일치
- Unity, Transfer state, projectile, input device, lifecycle, health mutation 의존 없음

## 독립 검토 포인트

- `BasicAttackContracts.cs:6-19`는 계약된 유일한 신규 public type이며 signed-64 ID와 immutable properties를 정확히 구현한다.
- `BasicAttackSession.cs:143-178`은 tick/observation/sample/press/sequence preflight와 staged publication을 유지한다. semantic 실패도 ID를 소비하지만 cooldown은 hit/miss에만 시작한다.
- `BasicAttackSession.cs:181-228`은 defensive observation copy, exact tick/키 검증, M1 request construction을 수행한다.
- `BasicAttackSession.cs:231-272`는 mouse/gamepad 선택·hysteresis와 승인된 6u 공통 범위를 Transfer state와 독립적으로 적용한다.
- `BasicAttackSessionTests.cs:114-155`는 stale+invalid+cooldown 조합과 ID 소비/무변이, hit/miss 경계를 검증한다.
- `BasicAttackSessionTests.cs:167-194`는 global 및 hit/miss cooldown overflow 원자성을 검증한다.
- `BasicAttackSessionTests.cs:226-247`는 selection, attempt 전체 scalar, request, snapshot 전체 scalar를 30/60/144 trace에 포함한다.

## 실행 결과

| 시험 | 결과 | SHA256 |
|---|---:|---|
| `TestResults/vd03-m2a-filtered-v2.xml` | 14/14 PASS | `42EFB0CBEE5B0BCD00BB99C6B8A41D912FD956DF540D4B91BA06C172E29CCEE5` |
| `TestResults/vd03-m2a-full-editmode-final.xml` | 85/85 PASS | `2F374E965608A224BF201BABA6F3CAF7F79C4362593201FED16CC3C02218AA6A` |
| `TestResults/vd03-m2a-combat-regression-final.xml` | 27/27 PASS | `4E56E46A70BD0F9FCC71CEC674271A6784C2404AB8DCD557C4CB2059C01C3701` |

## 소스 고정값

- `BasicAttackContracts.cs`: `CB5FA09CB890CA306CFC8D149A9857D97AC57E580913CC225D5B9123D286CE67`
- `BasicAttackSession.cs`: `93DEBC242E0C78A338774B57384CD7424DC22A665DA66CD13D3E6629A345D371`
- `BasicAttackSessionTests.cs`: `ACC6DA8B7521B3DB63761C06B3D439666A8B7275CBA8F9E4B3B1BF0C4172DCE8`

## 범위 경계

이 문서는 M2A의 순수 공격 선택·cooldown·request builder와 부분 `AC-COM-003`, `AC-UX-007`, `AC-UX-012`만 검증한다. 실제 입력 callback, camera/pose projection, Unity target authoring·LOS, fixed-phase batch merge, health 적용, enemy/boss, lifecycle 및 전체 VD-03/VD-07 완료는 후속 계약과 증적 범위다.
