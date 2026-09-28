# VD-03 M2B1 Unity 전투 저작·관찰 캡처 증적

- 검증일: 2026-08-27
- 구현: Terra, Sol 통합 보정
- 통합 판단: Sol
- 독립 검증: Luna
- 판정: **PASS — M2B1 Verified, P0/P1 없음**
- 관련 요구사항: `REQ-COM-001`, `REQ-COM-004`, `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-006`, `REQ-UX-007`, `REQ-UX-008`, `REQ-UX-013`
- 부분 증적 인수 기준: `AC-COM-003`, `AC-WT-003`, `AC-WT-005`, `AC-WT-006`, `AC-UX-007`, `AC-UX-012`

## 수용 범위

- 유일한 신규 public Unity 타입 `CombatTarget`과 internal registry·frozen descriptor·observation builder
- `player`, `walker`, `surveyor`, `ordan` 고정 역할 ID 및 체력·무적·공격 가능 경계
- 명시적 ordinal registry의 null·중복 ID·비정렬·stable collider 중복 거부와 원자적 frozen roster 게시
- 선택적 `TransferTarget` 공동 저작의 ID·collider·pose root·aim point·aim shape 전체 일치와 Ordan body 금지
- runtime transfer availability를 읽지 않는 internal immutable authored-geometry accessor
- 완료된 `t-1` player/camera pose와 exact-`t` aim을 사용하는 Q1000 AwayFromZero 캡처
- 5.999/6.000/6.001u, 23/24/25px 및 gamepad 17.9/18.0/18.1·25.9/26.0/26.1도 키 경계
- mask 256 LOS의 trigger·player root·target root 제외와 일반 blocker 차단
- player 후보 제외 및 combat-owned alive snapshot·authored attackability만으로 `IsAttackable` 결정
- 잘못된 tick·checked overflow 시 결과 collection 게시 전 원자 거부

## Sol 통합 판단

- 공동 저작 검증에는 `TransferTarget`의 새 internal immutable geometry accessor만 허용했다. 이 경로는 `ValidateForAuthoring`만 수행하며 `BuildDescriptor`, `IsStillAvailable`, modifier 또는 session state를 읽지 않는다.
- Luna가 지적한 문자열 역할 모호성은 승인 계약에 네 literal target ID를 명문화해 해소했다.
- 공동 저작 테스트는 한 scalar mismatch를 대표 표본으로 사용하지만 구현은 모든 scalar/reference를 명시적으로 비교한다. field별 parameterized 확대는 P2 hardening으로 남긴다.
- Combat 전용 LOS 시험은 핵심 open/blocked/trigger/root 경로를 검증한다. equal-fraction diagnostics와 64-hit saturation은 동일 `TransferLineOfSight.Evaluate` 구현을 직접 재사용하며 Transfer.Unity 37개 EditMode/PlayMode 회귀증적으로 고정한다.

## 실행 결과

| 시험 | 결과 | SHA256 |
|---|---:|---|
| `TestResults/vd03-m2b1-editmode-v4.xml` | 8/8 PASS | `87BE198D80FCCB41D08368A7CC49E088AAB9D24DFD2A1F53E61A3927669F649B` |
| `TestResults/vd03-m2b1-playmode-v1.xml` | 2/2 PASS | `66E483309E29FA88871BE9F1E96D42B9852FC9766F8D56C0007534FC714A1022` |
| `TestResults/vd03-m2b1-transfer-unity-editmode.xml` | 18/18 PASS | `DCC1B00631BCEF63C3C1A846250B5D0D51DC604555C11C55F018BDB733A86734` |
| `TestResults/vd03-m2b1-transfer-unity-playmode.xml` | 19/19 PASS | `F6220EE808BB020FDBC72BD1E525D1A31B9B7B921DAC501A9A422EDE473715AB` |
| `TestResults/vd03-m2b1-movement-editmode.xml` | 22/22 PASS | `8FF9EFC2CD5C05D7D0FA9693E363551F5DD7436D57DFF14EC40787297BB45848` |
| `TestResults/vd03-m2b1-movement-playmode.xml` | 12/12 PASS | `6570900E18AC9D516A6D54798288A632347F6845211770725AE338691A88EFB9` |
| `TestResults/vd03-m2b1-full-editmode.xml` | 93/93 PASS | `84BBFC674EEC1C10C2484CF4650B50C952A56AD2C0A60CB6A69C854B6C940C25` |

## 소스 고정값

- `CombatTarget.cs`: `3038042EA88C27123CCAE1A67CCE3F517ABB8AEF90A6CAB1B0AAB674D1D2B59B`
- `CombatObservationBuilder.cs`: `3725D0F7D217C8AEAFDEF8633079144CAA55C60F540897CCCE66D57DB9B6AFA3`
- `TransferTarget.cs`: `A065FD60EEFBD1B2068DF97639C4E9525D1C9A2EDC6C46D3ADAECA256064FF39`

## 범위 경계

이 문서는 M2B1의 저작·registry·관찰 캡처만 검증한다. 단일 physics sync receipt, `-190` combat driver, M2A→M1 batch merge, encounter reset, 죽음의 `t+1` transfer 제거와 30/60/144 전체 통합 trace는 M2B2의 별도 구현·검증 범위다. 따라서 전체 M2B 및 VD-03은 아직 Verified가 아니다.
