---
status: Verified
---

# VD-09 M5D7E profile load selection core

- Date: 2026-09-13
- Owners: Astra contract/approval; Terra implementation; Luna independent pre/post review
- Dependencies: M5D7A-M5D7D Verified
- Requirements: `REQ-PLAT-010`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D7E-001` through `AC-M5D7E-010`
- Astra approval: 2026-09-13 after Luna second pre-gate PASS (P0=0, P1=0); exact allowlist only.
- Astra verification: 2026-09-13 after focused EditMode 10/10, full EditMode 594/594, full PlayMode 576/576, and Luna post-review PASS (P0=0, P1=0).

## 목적과 경계

M5D7E는 앱 시작 시 이미 읽기·decode가 끝난 exact primary/previous/temp 관찰 세 개를 받아, 로드할 in-memory profile과 필요한 M5D7C 변환, 저장 필요 여부, 파일별 보존 의도를 결정하는 engine-free 순수 선택 코어다. 유효 primary가 언제나 우선이고 temp는 revision이나 유효성에 관계없이 절대 선택하지 않는다.

이 단위는 file/path/IO, hash 계산, recovery filename/UTC/충돌 ordinal, 실제 격리, M5D7D 저장 실행, 진단 기록, launch 알림, input override 적용, `Application.persistentDataPath`, hub/scene 진입을 소유하지 않는다. 이 계약의 `PreservationIntent`는 후속 adapter가 수행·기록할 명령이 아니라 검증 가능한 불변 계획이다.

## 승인 입력 API

새 engine-free runtime file 하나에 다음 public immutable API를 추가한다.

- `ProfileLoadFileRole`: exact `Primary`, `Previous`, `Temp`.
- `ProfileLoadCandidateKind`: exact `Missing`, `Unreadable`, `Decoded`.
- `ProfileLoadCandidateV1` factories:
  - `Missing(ProfileLoadFileRole role)`
  - `Unreadable(ProfileLoadFileRole role)`
  - `Decoded(ProfileLoadFileRole role, ProfileCanonicalDecodeResultV1 decodeResult)`
  - getters `Role`, `Kind`, `DecodeResult`와 `Validate()`.
- `ProfileLaunchSource`: exact `Primary`, `Previous`, `Default`.
- flags가 아닌 `ProfileFilePreservationReason`: exact `None`, `StaleTemp`, `InvalidPrimary`, `UnsupportedPrimary`, `UnreadablePrimary`, `InvalidPrevious`, `UnsupportedPrevious`, `UnreadablePrevious`.
- `ProfileLoadSelectionPlanV1` getters:
  - `Source`, `SourceRevision`, `ResultRevision`
  - `ProfileCanonicalDocumentV1 InMemoryDocument`
  - `bool RequiresAtomicSave`
  - `ProfileRecoveryPlanReason RecoveryReasons` (`0`은 direct primary에서만 허용)
  - `PrimaryPreservation`, `PreviousPreservation`, `TempPreservation`
  - `Validate()`.
- `ProfileLoadSelectorV1.Select(ProfileLoadCandidateV1 primary, ProfileLoadCandidateV1 previous, ProfileLoadCandidateV1 temp)`.

Candidate role은 argument position과 exact 일치해야 한다. `Missing`/`Unreadable`에는 decode payload가 없으며 `DecodeResult` getter는 candidate 전체를 먼저 검증한 뒤 exact `InvalidOperationException`을 throw한다. `Decoded`에만 getter가 허용되고, revalidation을 통과하는 non-default M5D7B result와 exact 다섯 classification(`ValidCurrentInput`, `ValidInputMetadataRecoveryRequired`, `ValidBindingRecoveryRequired`, `UnsupportedProfileSchema`, `Invalid`) 중 하나를 요구한다. Caller byte array, parse tree, path, exception, clock 또는 mutable collection은 보존하지 않는다. Candidate와 plan의 모든 getter는 전체 `Validate()`를 먼저 수행하고 default/reflection-invalid/unknown enum/role mismatch를 `InvalidOperationException` 또는 entry `ArgumentException`으로 fail closed한다.

## 선택·변환 행렬

Decoder classification은 다음 세 그룹으로만 해석한다.

- current: `ValidCurrentInput`
- input-recoverable: `ValidInputMetadataRecoveryRequired`, `ValidBindingRecoveryRequired`
- unusable: `Invalid`, `UnsupportedProfileSchema`

선택 순서는 고정한다.

1. Primary current이면 그대로 선택한다. source/result revision은 같은 `r`, save 없음, recovery reasons `0`이다.
2. Primary input-recoverable이면 primary를 선택하고 M5D7C `PlanInputRepair` 결과를 사용한다. source `r`, result `r+1`, save 필요, reason exact `InputRepair`다.
3. Primary가 missing/unreadable/unusable이면 previous current 또는 input-recoverable을 선택하고 M5D7C `PlanPreviousPromotion` 결과를 사용한다. source `r`, result `r+1`, save 필요, reasons는 exact `PreviousPromotion` 또는 `PreviousPromotion|InputRepair`다.
4. Previous도 선택할 수 없으면 M5D7C `PlanDefaultBootstrap`을 사용한다. source `-1`, result `0`, save 필요, reason exact `DefaultBootstrap`이다.

Primary가 선택되면 previous는 선택 결과를 바꾸지 않는다. Temp는 어떤 경우에도 선택·변환 source가 아니며 higher revision current temp도 무시한다. Direct-current primary revision `long.MaxValue`는 증가나 저장이 없으므로 그대로 허용한다. 반면 `long.MaxValue` input-recoverable primary 또는 selectable previous는 M5D7C overflow를 그대로 `InvalidOperationException`으로 전파하며 default로 강등하지 않는다.

## 파일 보존 의도

- 존재하는 temp(`Unreadable` 또는 모든 `Decoded`)는 언제나 exact `StaleTemp`; missing temp만 `None`이다. Temp decode classification/version/revision은 reason을 바꾸지 않는다.
- 선택되지 못한 primary는 `Invalid`→`InvalidPrimary`, unsupported→`UnsupportedPrimary`, unreadable→`UnreadablePrimary`, missing/current/input-recoverable→`None`이다. Primary input-recoverable은 전체 파일 격리 대상이 아니라 input-only repair source다.
- Previous는 `Invalid`→`InvalidPrevious`, unsupported→`UnsupportedPrevious`, unreadable→`UnreadablePrevious`, missing/current/input-recoverable→`None`이다. 이 규칙은 valid primary가 선택되어도 동일하다.
- 보존 reason은 원본 삭제 성공이나 recovery path 생성을 주장하지 않는다. 후속 adapter는 readable content에는 hash8, 읽을 수 없는 content에는 `nohash`를 사용하고 실패를 진단해야 한다.

Plan은 selected source와 M5D7C result를 private proof로 방어 복사해 재검증한다. Direct primary plan은 primary current document와 revision exact equality를 증명한다. Recovery plan은 M5D7C plan의 source/result/reasons/document exact equality를 증명한다. In-memory document는 항상 current-compatible이고 canonical validation을 통과한다.

## 요구사항

- **REQ-M5D7E-001:** 유효 primary를 revision과 무관하게 항상 우선하고 temp는 절대 source로 선택하지 않는다.
- **REQ-M5D7E-002:** primary input recovery, previous promotion, previous+input recovery와 default bootstrap을 M5D7C exact 변환/revision 규칙으로 계획한다.
- **REQ-M5D7E-003:** unusable/unreadable primary·previous와 모든 존재 temp에 역할별 exact 보존 의도를 발행한다.
- **REQ-M5D7E-004:** candidate/plan role·kind·decode·source·revision·reason·document 관계를 full revalidation하고 overflow/unknown/default/reflection misuse를 fail closed한다.
- **REQ-M5D7E-005:** no IO/path/hash/time/quarantine execution/save execution/log/notification/input apply/Unity/network/RNG/callback authority와 기존 API/asmdef/assets 무변경을 지킨다.

## 수용 기준

- **AC-M5D7E-001:** primary current의 revision `r`이 previous/temp의 모든 조합과 더 높은 revision에도 exact direct primary, source/result `r`, save false, reason `0`이다.
- **AC-M5D7E-002:** metadata/binding recovery primary는 non-input semantics를 보존한 current-default input, `r→r+1`, save true, exact `InputRepair`다.
- **AC-M5D7E-003:** missing/invalid/unsupported/unreadable primary 각각에서 current previous는 전체 semantics를 보존한 `PreviousPromotion`, `r→r+1`이다.
- **AC-M5D7E-004:** 같은 primary 조합에서 metadata/binding recovery previous는 non-input semantics와 `PreviousPromotion|InputRepair`, `r→r+1`이다.
- **AC-M5D7E-005:** primary/previous가 선택 불가능한 cross-product는 exact approved default, `-1→0`, save true, `DefaultBootstrap`이며 temp가 current여도 달라지지 않는다.
- **AC-M5D7E-006:** 존재 temp의 모든 classification/revision과 unreadable은 `StaleTemp`, missing만 `None`; temp는 source/document/revision에 영향을 주지 않는다.
- **AC-M5D7E-007:** primary/previous invalid·unsupported·unreadable reason은 정확히 역할별로 발행되고 missing/current/input-recoverable은 `None`; valid primary 옆 비정상 previous도 보존 의도를 유지한다.
- **AC-M5D7E-008:** candidate argument 위치/role mismatch, kind/decode mismatch, default/reflection-invalid decode/candidate/plan, unknown enum/reason, source/save/revision/document/private-proof mismatch는 mutation 없이 거부된다. Missing/Unreadable `DecodeResult` getter는 exact `InvalidOperationException`, Decoded getter는 exact 다섯 classification만 반환한다. 모든 public getter의 revalidation과 source arrays/bytes mutation 독립성을 검증한다.
- **AC-M5D7E-009:** selectable recovery source revision `0`, positive, `long.MaxValue-1`의 exact 증가와 `long.MaxValue` throw/no-fallback을 검증한다. Direct-current primary `long.MaxValue`는 save 없이 exact 보존한다. Repeated selection은 byte-identical document와 같은 plan이다.
- **AC-M5D7E-010:** focused/full EditMode와 full PlayMode fail/skip/inconclusive 0, Luna P0/P1 0이며 실제 load/quarantine/save/notification PASS로 과장하지 않는다.

## 정확한 allowlist

- `Assets/AcadeGameMaker/Runtime/Profile/ProfileLoadSelectorV1.cs` + `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileLoadSelectorV1Tests.cs` + `.meta`
- this contract
- `docs/verification/2026-09-13-vd09-m5d7e-contract-pregate.md`
- `docs/verification/2026-09-13-vd09-m5d7e-implementation-evidence.md`
- `docs/verification/2026-09-13-vd09-m5d7e-luna-independent-review.md`
- minimal `docs/README.md` entry

Existing M5D3-M5D7D source/tests, asmdefs, Packages, ProjectSettings, scenes, prefabs, input/actions and other assets are read-only. Luna pre-gate → Astra Approved → Terra → root tests → Luna post-review → Astra Verified 순서다.
