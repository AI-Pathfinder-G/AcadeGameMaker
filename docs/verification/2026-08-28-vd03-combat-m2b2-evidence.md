# VD-03 M2B2 Unity 전투 고정 페이즈 통합 증적

- 검증일: 2026-08-28
- 구현: Terra, Sol 통합 보정
- 통합 판단: Sol
- 독립 검증: Luna
- 판정: **PASS — M2B2 Verified, P0/P1/P2 없음**
- 관련 요구사항: `REQ-COM-001`, `REQ-COM-004`, `REQ-COM-005`, `REQ-COM-006`, `REQ-UX-007`, `REQ-UX-008`, `REQ-UX-013`, `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-006`
- 부분 증적 인수 기준: `AC-COM-003`, `AC-UX-007`, `AC-UX-012`, `AC-WT-003`, `AC-WT-005`, `AC-WT-006`

## 수용 범위

- `TransferSimulationDriver=-200 → CombatSimulationDriver=-190 → movement 기본 순서`와 shared aim tick당 단일 `Physics2D.SyncTransforms`
- processing tick·aim ID·sample tick·camera tick을 담는 internal immutable capture receipt와 missing·stale·mismatch fail-stop
- exact-tick combat input buffer, defensive external-damage copy, reset 배타성과 conservative horizon 사전검증
- M2A의 선택적 요청을 외부 batch 뒤에 무정렬 추가하고 M1을 정확히 한 번 호출하는 피해 통합
- frozen roster만 재사용하는 explicit encounter reset과 health·cooldown·sequence ID·M1 tombstone 초기화
- 신규 사망한 co-authored target만 `t+1`에 한 번 제거하고 동일 removal은 멱등 축약하는 transfer handoff
- receipt·horizon·removal merge 실패가 선행 transfer publication과 M2A·M1·health·latest outcome 경계를 보존하는 원자성
- complete transfer/combat outcome, death handoff, combat·movement next tick, sync count와 receipt 전 필드를 비교하는 30/60/144 render-group replay
- 신규 public Unity 타입은 기존 M2B1의 `CombatTarget` 하나뿐이며 driver·input·outcome·receipt·removal seam은 internal

## 실행 결과

| 시험 | 결과 | SHA256 |
|---|---:|---|
| `TestResults/vd03-m2b2-focused-final.xml` | 15/15 PASS | `848AF5ABE001658FD8EA69A04A8325FA2CC543A868765D4388F6634B086C6867` |
| `TestResults/vd03-m2b2-full-editmode-final.xml` | 93/93 PASS | `0F17507508379F0AE2AAD5EA04E1B3053BF0445988322473AB3AEBE2F0DF607D` |
| `TestResults/vd03-m2b2-full-playmode-final.xml` | 48/48 PASS | `7D3E94C56F844D1635E3365DCD319B921CB3EC93E67F688EC14368DDD068CD2A` |

마지막 변경은 30/60/144 검증 trace에 combat·movement clock, sync count와 receipt scalar를 추가한 테스트 전용 보완이다. 이 경로는 새 focused 15/15로 재실행했다. 런타임이 동일한 전체 PlayMode 48/48과 EditMode 93/93 증적은 유지된다. 이후 전체 PlayMode 재실행 두 차례는 Unity headless entitlement가 없어 exit 198로 테스트 시작 전에 종료됐으며 제품 실패로 분류하지 않는다.

## 핵심 소스 고정값

- `CombatSimulationDriver.cs`: `7F8D7ADFA10687C8A7B2AF35048EB50C1F8C22419CA7A45CC8FA93467CA60DA4`
- `CombatTarget.cs`: `AC619B3C64E247D5B82408C7A608745740B5DAAFD031FB18574F0CE0D507A579`
- `TransferSimulationDriver.cs`: `B5DF0B99C3B78F47A1DB967BA5E404AF5890214AF423CD2F381081DF46B21012`
- `CombatSimulationDriverPlayModeTests.cs`: `ECF6DA42DF2E6115B95A7349F92971274480D4D6502018C9DFB8A61B47D408CF`

## Ollama 활용 기록

- Kimi K3: **used and accepted** — preflight·staging, M2A 1회·M1 1회와 신규 사망 handoff 구조를 설계 입력으로 수용했다. 계약과 다른 reset 제안은 Terra와 Sol이 반려하고 frozen-session 규칙으로 재작성했다.
- GLM 5.2: **used and accepted** — receipt temporal mismatch, horizon overflow, reset collision, duplicate removal과 실행 순서의 적대적 QA 항목을 테스트에 반영했다. 계약에 없는 가정은 반려했다.
- MiniMax M3: **failed and replaced** — quota/429가 아니라 reasoning·출력 길이 제어 실패와 임의 스키마 생성 때문에 결과를 반려했다. Terra가 정규화 replay fixture를 구현하고 Luna가 독립 검증했다.

세 Cloud 모델에는 추상 비민감 작업명세만 전달했으며 저장소 원문·경로·로그·자격 증명을 전달하지 않았다.

## Luna 독립 검증과 Sol 통합 판단

Luna의 첫 post-review는 실패 원자성, 사망 제거 음성 경로, reset tombstone·frozen authoring, complete next-tick trace의 네 P1 증적 결손을 발견했다. Terra는 실제 driver overflow와 complete publication boundary, 비전이·duplicate·invulnerable death 경로, tombstone 재사용, mutable authoring 비재독출을 추가했다. 마지막 trace에는 combat next tick, player/movement next tick, sync count와 receipt 네 필드를 포함했다.

Luna 최종 판정은 P0/P1/P2와 제품 결함이 모두 없는 PASS다. Sol은 M2B2를 수용하며 M2B1과 결합해 승인된 VD-03 Unity combat adapter milestone을 Verified로 통합한다. 이 증적은 enemy AI·boss pattern·reward·room completion·실제 VD-07 input producer·UI/VFX/audio 또는 전체 VD-03 완료를 주장하지 않는다.
