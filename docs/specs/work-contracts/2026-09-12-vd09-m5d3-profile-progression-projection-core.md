---
status: Verified
---

# VD-09 M5D3 프로필 진행 상태 투영 코어

- Date: 2026-09-12
- Owning Approved specs: [VD-04](../vertical-demo/04-authored-rooms-and-expedition.md), [VD-05](../vertical-demo/05-failure-and-persistence.md), [VD-06](../vertical-demo/06-humanity-choice-and-narrative.md), [VD-09](../vertical-demo/09-platform-and-quality.md), [SYSTEM-CONTRACTS](../vertical-demo/SYSTEM-CONTRACTS.md)
- Decision basis: ADR-0013, ADR-0018, ADR-0031
- Dependency: [M5A Verified](./2026-09-08-vd05-m5a-post-boss-progression-core.md), [M5D2 Verified](./2026-09-11-vd05-m5d2-run-failure-arbitration-core.md)
- Assigned by / final authority: Astra
- Implementation: Terra; independent pre/post verification: Luna
- Requirements: `REQ-ROOM-006`, `REQ-RUN-003`, `REQ-CHOICE-004`, `REQ-PLAT-006`, `REQ-PLAT-008`
- Acceptance IDs: `AC-M5D3-001` through `AC-M5D3-007`; parent ACs are only partially exercised.
- Astra approval: 2026-09-12 after Luna second pre-gate PASS (P0=0, P1=0). Only the exact M5D3 allowlist below is authorized; this is not implementation verification.
- Verified: 2026-09-12 — Luna independent implementation review PASS (P0=0, P1=0; one non-blocking existing M5D1 finalizer P2) and Astra accepts this bounded integration. See [implementation evidence](../../verification/2026-09-12-vd09-m5d3-implementation-evidence.md) and [Luna review](../../verification/2026-09-12-vd09-m5d3-luna-independent-review.md). This verifies only the immutable progression projection core, not profile file validation, persistence, source authentication or M5A playable connection.

## 목적과 경계

M5A는 이미 검증된 프로필 snapshot에서 고정한 `choice/skill` 쌍을 요구하지만, 현재 저장소에는 VD-09 프로필 상태를 표현하는 runtime 형식이 없다. M5D3는 파일 IO·JSON·hash보다 먼저 프로필의 progression block을 불변 값으로 표현하고, M5A가 소비할 수 있는 닫힌 선택–기술 투영을 제공한다.

이 단위는 **구조적으로 유효한 progression 값**만 보장한다. 디스크에서 읽었음, canonical JSON 검증, integrity hash 검증, 현재 input asset 일치, atomic save 성공 또는 source provenance를 인증하지 않는다. 후속 VD-09 codec/load owner가 전체 profile 검증을 통과한 뒤 이 값을 생성하고, 실제 M5A adapter는 그 owner identity와 revision을 별도로 대조해야 한다.

### 포함

- schema v1 progression field의 엔진 비의존 immutable value model
- nonnegative `ProfileRevision`
- `lastOfferedSeed`: null 또는 `101`, `202`, `303`, `404`
- `committedChoice`, `grantedSkill`, `consentState`, `completedBranches`
- exact closed choice/skill pair 및 M5A용 read-only pair projection
- completed branch의 ordinal canonical order와 uniqueness 검증
- constructor bypass/default/unknown enum/collection mutation 방어

### 비범위

- settings, input, tutorial block
- JSON parse/serialize, RFC 8785 inner binding canonicalization, UTF-8/NFC/escape/property-order 검사
- SHA-256, `integrity`, schemaVersion 판정
- persistentDataPath, primary/previous/temp, flush/replace/move, recovery/quarantine/notification
- revision 증가, default 저장, binding 부분 복구
- 선택 UI/확정, 기술 부여·효과·쿨다운, completed branch 기록 수행
- M5A/Run/Unity adapter, scene/input/Combat/Transfer 접근

## 승인 API와 상태

새 engine-free `AcadeGameMaker.Profile` assembly에 아래 public immutable value API를 둔다. 이는 새 assembly의 최초 API이며 기존 assembly public ABI를 변경하지 않는다.

- `ProfileChoice`: legal values `Extraction=1`, `Solidarity=2`; underlying `0`과 그 밖의 값은 invalid sentinel/unknown이다.
- `ProfileSkill`: legal values `CompressionVerdict=1`, `CommonReferencePlane=2`; underlying `0`과 그 밖의 값은 invalid sentinel/unknown이다.
- `ProfileConsentState`: legal values `RefusesOwnershipTransfer=1`, `OffersScopedResonance=2`, `ResonanceLender=3`, `ImprintSevered=4`; underlying `0`과 그 밖의 값은 invalid sentinel/unknown이다.
- `ProfileBranch`: legal values `Extraction=1`, `Solidarity=2`; underlying `0`과 그 밖의 값은 invalid sentinel/unknown이다.
- `ProfileChoiceSkillPair`: nullable이 아닌 exact committed pair. 생성 가능한 값은 `Extraction/CompressionVerdict`, `Solidarity/CommonReferencePlane` 두 개뿐이다.
- `ProfileProgressionSnapshot`: `ProfileRevision`, nullable `LastOfferedSeed`, nullable `CommittedChoice`, `ConsentState`, nullable `GrantedSkill`, immutable `CompletedBranches`를 보유한다.
- `HasCommittedPair`: choice와 skill이 함께 존재하는 두 exact pair에서만 true.
- `TryGetCommittedPair(out ProfileChoiceSkillPair pair)`: no-choice이면 false와 default out value를 반환하고, committed pair이면 true와 exact pair를 반환한다. 잘못된 조합을 false로 숨기지 않고 snapshot 생성 시 거부한다.
- 두 value type은 exact public instance signature `public void Validate()`를 제공한다. 유효하면 반환하고, default/constructor-bypass/unknown 값이면 `InvalidOperationException`을 던진다. pair의 public `Choice`/`Skill` getter와 snapshot의 `HasCommittedPair`/`TryGetCommittedPair`는 먼저 같은 전체 재검증을 수행한다.

`ProfileRevision`은 이미 검증된 전체 profile의 source revision을 운반하는 correlation 값이며 `0..long.MaxValue`다. 이 API는 revision을 증가시키지 않는다. profile schema의 JSON integer가 C# `long` 범위를 넘어서는 입력은 후속 codec에서 invalid로 거부해야 하며 M5D3가 arbitrary precision을 도입하지 않는다.

`CompletedBranches`는 빈 배열 또는 `Extraction`, `Solidarity`의 ordinal enum 순서로 정렬된 unique 배열이다. 두 branch가 함께 존재하는 것은 schema상 합법이다. M5D3는 현재 committed choice와 completed history를 억지로 일치시키지 않으며, history를 지우거나 보충하지 않는다. null collection, unknown enum, 역순, duplicate는 거부한다. 반환 collection은 방어 복사되고 외부 변조가 snapshot을 바꿀 수 없다. 명시적인 no-choice snapshot도 생성자를 통해 유효한 consent와 **null이 아닌 빈** `CompletedBranches`를 제공해야 한다.

## 닫힌 선택–기술 규칙과 consent 경계

허용되는 choice/skill 조합은 정확히 세 상태다.

| CommittedChoice | GrantedSkill | Pair projection |
|---|---|---|
| null | null | 없음 (`TryGetCommittedPair=false`) |
| Extraction | CompressionVerdict | exact extraction pair |
| Solidarity | CommonReferencePlane | exact solidarity pair |

한쪽만 null, 교차 조합, unknown enum은 mutation-free validation error다. 이 규칙은 M5A와 VD-06의 기술 대응을 그대로 보존한다.

`ConsentState`는 VD-09가 승인한 네 값만 구조적으로 허용한다. M5D3는 consent transition owner가 아니므로 consent와 pair 사이에 parent spec에 없는 추가 교차 제약을 만들지 않는다. 특히 `OffersScopedResonance` 저장 시점의 허용 여부를 여기서 새로 결정하지 않는다. 후속 save-request owner는 VD-06의 실제 `ChoiceCommitted(choice, consentState, grantedSkill, tick)` receipt를 대조하고, 후속 codec은 unknown 값만 거부한다.

## 검증·실패·결정성

- 모든 입력 validation과 collection defensive copy가 객체 publication 전에 끝나야 한다.
- invalid revision/seed/enum/pair/branch collection은 `ArgumentException` 계열로 거부되고 부분 객체나 보정 결과를 내지 않는다.
- enum underlying value를 직접 cast한 unknown 값도 constructor boundary에서 재검증한다.
- default `ProfileProgressionSnapshot`은 유효한 default profile로 간주하지 않는다. null `CompletedBranches`와 invalid-zero consent를 가지므로 `Validate`, `HasCommittedPair`, `TryGetCommittedPair`가 각각 전체 값을 다시 검사한 뒤 `InvalidOperationException`으로 거부해야 한다. 두 projection member가 default를 단순 false/no-choice로 숨겨서는 안 된다.
- 합법적인 no-choice 상태는 생성자를 통해 유효한 consent, `choice=null`, `skill=null`, non-null empty branches를 명시한 값이며 default struct와 구별된다.
- default `ProfileChoiceSkillPair`는 두 invalid-zero enum을 가지므로 `Validate` 또는 public property 소비 경계에서 `InvalidOperationException`으로 거부한다. 유효한 첫 pair와 default의 field bit pattern이 같아서는 안 된다.
- nullable choice/skill이 `HasValue=true`이면서 underlying value가 0 또는 unknown cast이면 snapshot constructor와 재검증 경계 모두 거부한다.
- 같은 입력은 같은 field 값과 branch order를 가진 snapshot/pair를 만든다. clock, RNG, IO, callbacks 또는 mutable static state가 없다.
- 이 값들은 authenticated capability가 아니다. caller가 같은 값을 복사할 수 있으므로 future adapter의 실제 profile owner/source binding을 대체하지 않는다.

## 요구사항

- **REQ-M5D3-001:** 새 Profile assembly는 schema v1 progression field와 nonnegative source revision을 engine-free immutable snapshot으로 표현하며 default/bypass 값도 소비 경계에서 재검증한다.
- **REQ-M5D3-002:** last seed는 null 또는 네 승인 seed만 허용하고 completed branch는 exact sorted-unique immutable collection으로 보존하며 invalid/unknown/null/mutable 입력을 보정 없이 거부한다.
- **REQ-M5D3-003:** choice/skill은 no-choice 또는 승인된 두 exact pair만 허용하고 M5A용 pair projection은 no-choice와 committed pair를 명시적으로 구분한다.
- **REQ-M5D3-004:** consent는 승인 enum domain만 보존하며 이 코어가 새 저장 시점, transition 또는 pair-consent 교차 규칙을 발명하지 않는다.
- **REQ-M5D3-005:** 코어는 disk/canonical/hash/schema/input compatibility/save/recovery/source-authentication을 주장하거나 수행하지 않으며 이후 전체 profile validator와 source-bound adapter가 필요하다.
- **REQ-M5D3-006:** 기존 public ABI, Packages, ProjectSettings, scene/prefab/media를 변경하지 않고 UnityEngine, IO, cryptography, JSON, clock, RNG, network, callbacks를 참조하지 않는다.

## 수용 기준

- **AC-M5D3-001:** revision `0`과 `long.MaxValue`, seed null/101/202/303/404를 포함한 합법 snapshot이 exact 값을 보존하고 negative revision 및 다른 seed는 생성·재검증 경계에서 거부된다.
- **AC-M5D3-002:** 유효 consent와 non-null empty branches를 거친 명시적 no-choice snapshot은 `HasCommittedPair=false`/`TryGetCommittedPair=false`로 투영되고 두 committed pair는 exact choice/skill을 한 번에 제공한다. one-sided null, crossed pair, nullable unknown/zero choice·skill은 constructor와 재검증 모두에서 partial projection 없이 거부된다.
- **AC-M5D3-003:** 네 consent 값은 그대로 보존되고 unknown consent는 거부된다. 테스트와 코드가 M5D3 안에 pair-consent 교차 보정·추론·transition을 두지 않았음을 확인한다.
- **AC-M5D3-004:** 빈 branch, Extraction, Solidarity, 두 값의 canonical 순서는 exact 보존된다. null, 역순, duplicate, unknown branch는 거부되고 source/returned collection 변조가 snapshot을 바꾸지 않는다.
- **AC-M5D3-005:** 생성자를 우회한 default snapshot에 대해 `Validate`, `HasCommittedPair`, `TryGetCommittedPair`가 모두 throw하며 false/no-choice로 숨기지 않는다. default pair의 `Validate`와 public choice/skill property 소비도 throw한다. 명시적 no-choice의 non-null empty branches는 정상이고, 실패 뒤 별도 valid 생성은 정상 동작한다.
- **AC-M5D3-006:** 정적 검사가 새 Profile runtime assembly의 no-engine 경계와 UnityEngine/IO/crypto/JSON/clock/RNG/network/callback 부재, 기존 ABI·Packages·ProjectSettings·asset 무변경을 증명한다.
- **AC-M5D3-007:** Luna가 `AC-M5D3-001..006` mapping과 source/test를 독립 검토하고 focused 및 전체 EditMode/PlayMode가 zero failure/skip/inconclusive로 통과한다. pure projection PASS를 disk profile 검증 또는 M5A playable 연결로 보고하지 않는다.

## 정확한 구현 allowlist

Runtime:

- new `Assets/AcadeGameMaker/Runtime/Profile/AcadeGameMaker.Profile.asmdef` and `.meta`
- new `Assets/AcadeGameMaker/Runtime/Profile/ProfileProgressionSnapshot.cs` and `.meta`

Tests:

- new `Assets/AcadeGameMaker/Tests/EditMode/Profile/AcadeGameMaker.Profile.EditMode.Tests.asmdef` and `.meta`
- new `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileProgressionSnapshotTests.cs` and `.meta`

Documents:

- this contract
- later `docs/verification/2026-09-12-vd09-m5d3-contract-pregate.md`
- later `docs/verification/2026-09-12-vd09-m5d3-implementation-evidence.md`
- later `docs/verification/2026-09-12-vd09-m5d3-luna-independent-review.md`
- `docs/README.md` minimum index/status link

그 밖의 Runtime/Tests/asmdef, existing verification, Packages, ProjectSettings, prefab/scene/assets/media는 변경 금지다.

## 검증 순서와 후속 작업

1. Luna가 Review 문서의 parent mapping, nullable/default distinction, enum bypass, branch canonicality, false provenance claim을 반례 중심으로 pre-gate한다.
2. Astra가 수정사항을 통합하고 이 문서만 Approved로 전환한다.
3. Terra가 allowlist만 구현하고 focused/full EditMode 및 full PlayMode 결과와 REQ/AC mapping을 남긴다.
4. Luna가 구현자와 독립적으로 전체 소스·테스트·회귀 증적을 검토한다.
5. Astra만 Verified와 통합을 결정한다.

다음 단계는 전체 schema의 deterministic canonical codec/hash, atomic file transaction/load recovery, validated in-memory profile owner, actual M5A adapter 순서다. M5D3 값만으로 파일이 유효하다거나 선택이 저장됐다고 주장해서는 안 된다.

## 롤백과 참여 기록

Rollback point는 M5D2 Verified 상태다. 되돌리기는 승인 시 M5D3의 새 runtime/test 파일과 이 계약·후속 증적·docs index hunk만 대상으로 하며 reset/clean/checkout/다른 dirty 파일 삭제를 사용하지 않는다. commit과 remote publication은 암시되지 않는다.

Astra가 승인 규칙과 현 저장 의존성을 대조해 초안을 작성했고 Terra는 JSON/crypto 가용성만 읽기 전용으로 조사했다. Luna의 1차 pre-gate P1을 반영한 뒤 second pre-gate가 P0=0/P1=0으로 통과하여 Astra가 이 allowlist를 Approved로 전환했다. 구현 후 검증은 아직 수행되지 않았다. 외부 모델, Ollama, 자동화, network call과 Pro 실행은 사용하거나 주장하지 않는다.
