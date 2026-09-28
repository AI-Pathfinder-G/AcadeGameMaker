# VD-02 Unity M2B2A observation·movement seam 증적

- 검증일: 2026-08-26
- 구현: Terra
- 통합 판단: Sol
- 독립 검증: Luna
- 판정: **PASS — M2B2A Verified**
- 관련 요구사항: `REQ-WT-001`, `REQ-WT-003`, `REQ-WT-004`, `REQ-WT-006`, `REQ-WT-008`, `REQ-MOV-004`
- 부분 증적 인수 기준: `AC-WT-003`, `AC-WT-006`, `AC-MOV-004`

## 수용 범위

- tick `t`가 완료된 `t-1` player·camera pose와 exact `t` aim sample만 소비
- player Q4096→Q1000 `RoundDivAway`
- target XY pose의 축별 단 한 번 AwayFromZero Q1000 양자화와 checked local addition
- removed TargetId의 ordinal 제외
- projection·ellipse·distance·CORDIC·production LOS로 observation 구성
- 모든 checked 계산 성공 전 observation 목록 미발행
- builder 내부 `Physics2D.SyncTransforms`, discovery, public provider 없음
- movement tick preflight 무변이와 final existing modifier·compatibility gravity 비거부 반영
- Transfer core·Movement.Unity의 최소 internal friend 경계

## 실행 결과

| 시험 | 결과 | SHA256 |
|---|---:|---|
| `TestResults/vd02-m2b2a-observation-editmode.xml` | 3/3 PASS | `B616B4DBE53D9DB15D64D785F077BEBC55624BE624E7043F60A6CFE35D01A35E` |
| `TestResults/vd02-m2b2a-observation-playmode.xml` | 1/1 PASS | `048DA45A57270748C0236B8C1F47326A7A4F44481538D6C2D69922E4C54F6FF2` |
| `TestResults/vd02-m2b2a-movement-playmode.xml` | 12/12 PASS | `BABAB2C88B3F1DA35C7D754D37099B8C2A8B9C2AEBF9EDF3FB3FE85AEF68BBF6` |
| `TestResults/vd02-m2b2a-transferunity-editmode.xml` | 18/18 PASS | `1FB09BCD66FCE71A0ADFB3AA8BF044E49D34FC13C3F4C34706C155702BCED24C` |
| `TestResults/vd02-m2b2a-transferunity-playmode.xml` | 10/10 PASS | `DCE0DAD10DB11A5B44B0FA3DD3581F6DCF22A719B107A6207A736E00870CCC9C` |

## 핵심 소스 고정값

- `TransferObservationBuilder.cs`: `8DA52624593FBA523904B03B64A05D495E46C8E7F17785FC0A59C2278CE4B187`
- `PlayerMovementController.cs`: `E9619847BFCC4A1A8203583067766593BA53023DE4E301497170B0FBF8621950`
- `TransferObservationBuilderTests.cs`: `D4C88A064F62D7A6155E40ABF72AF8D8E08BAE79CA411DF761D50F8748AA4EB0`
- `TransferObservationBuilderPlayModeTests.cs`: `A6F99897A289470CC954BE1165E96373A51DBBF18E3161E9486E51C7A6AA256C`

## 통합 판단

Sol은 승인 계약보다 강하지만 불필요한 double/stale apply 거부 조건을 철회했다. preflight만 tick mismatch를 거부하고, 성공한 preflight 이후 동일 `-200` transfer phase의 final reflection은 target mutation 뒤 실패하지 않도록 검사를 반복하지 않는다. 별도 token이나 controller gameplay state는 추가하지 않았다.

Kimi K3 제한 호출은 usable output 없이 prompt를 반복해 어떤 코드·assertion·API 판단도 채택하지 않았다. Luna는 구현과 결과를 독립 검토해 runtime P0/P1이 없다고 판정했다. Unity가 생성한 `SceneTemplateSettings.json`은 gate hygiene P1으로 식별돼 통합 전에 제거했다.

## 보류 범위

- execution order `-200` session driver와 한 capture-phase `Physics2D.SyncTransforms`
- target sink apply/clear·session publication·movement reflection 원자성
- lifecycle/active removal/same-tick removal+press
- box owner apply/clear base mass·gravity 복구와 enemy/boss stub
- 30/60/144 render-grouped transfer replay
