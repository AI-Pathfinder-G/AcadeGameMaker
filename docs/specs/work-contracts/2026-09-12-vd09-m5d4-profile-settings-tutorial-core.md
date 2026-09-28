---
status: Verified
---

# VD-09 M5D4 프로필 설정·튜토리얼 값 코어

- Date: 2026-09-12
- Owning Approved specs: [VD-05](../vertical-demo/05-failure-and-persistence.md), [VD-09](../vertical-demo/09-platform-and-quality.md), [SYSTEM-CONTRACTS](../vertical-demo/SYSTEM-CONTRACTS.md)
- Decision basis: ADR-0010, ADR-0013, ADR-0018, ADR-0031
- Dependency: [M5D3 Verified](./2026-09-12-vd09-m5d3-profile-progression-projection-core.md)
- Assigned by / final authority: Astra
- Implementation: Terra; independent pre/post verification: Luna
- Requirements: `REQ-RUN-003`, `REQ-PLAT-002`, `REQ-PLAT-006`, `REQ-PLAT-008`
- Acceptance IDs: `AC-M5D4-001` through `AC-M5D4-006`; parent ACs are partially exercised only.
- Astra approval: 2026-09-12 after Luna pre-gate PASS (P0=0, P1=0). Only the exact allowlist below is authorized; this is not implementation verification.
- Verified: 2026-09-12 — Luna independent implementation review PASS (P0=0, P1=0; one non-blocking reflection-matrix P2) and Astra accepts this bounded integration. See [implementation evidence](../../verification/2026-09-12-vd09-m5d4-implementation-evidence.md) and [Luna review](../../verification/2026-09-12-vd09-m5d4-luna-independent-review.md). This verifies only settings/tutorial immutable values, not application events or persistence.

## 목적과 경계

VD-09 profile v1의 settings와 tutorial block은 이미 필드·범위·canonical array 규칙이 승인돼 있다. M5D4는 이를 M5D3 Profile assembly 안의 engine-free immutable 값으로 구현해 후속 full profile codec이 임의 dictionary나 Unity object를 저장하지 않게 한다.

이 단위는 JSON, 파일, hash, input binding 또는 전체 profile 유효성을 주장하지 않는다. ID를 새로 발급하거나 tutorial 확인을 수행하지 않고, caller가 제공한 already-authored 값을 구조적으로 검증한다.

### 포함

- `ProfileWindowMode`: legal `Windowed=1`, `BorderlessFullscreen=2`; `0`/unknown invalid
- immutable `ProfileSettingsSnapshot`
- immutable `ProfileTutorialSnapshot`
- Q1000 master/music/sfx 각각 `0..1000`
- gamepad aim invert X/Y booleans
- tutorial confirmed ID의 NFC, ordinal strictly sorted, unique, non-null collection/value 규칙
- default/constructor-bypass/public getter 재검증과 defensive copy

### 비범위

- input asset ID, binding schema, binding override JSON/JCS
- progression 변경(M5D3 API를 재사용하며 수정하지 않음)
- 전체 `ProfileSnapshotV1` composition, schemaVersion/revision 증가
- JSON/canonical bytes/UTF-8 escape/property order/hash
- atomic write/load/recovery/quarantine/notification
- 실제 화면 모드·볼륨·입력 설정 적용, tutorial UI/확정 사건
- UnityEngine, scene/prefab, Packages, ProjectSettings

## 승인 API

기존 `AcadeGameMaker.Profile` assembly에 아래 새 public immutable value API를 추가한다.

- `ProfileWindowMode` with exact legal numeric values above.
- `ProfileSettingsSnapshot(ProfileWindowMode windowMode, int masterVolumeQ1000, int musicVolumeQ1000, int sfxVolumeQ1000, bool gamepadAimInvertX, bool gamepadAimInvertY)`.
- settings public getters for the six exact fields and `public void Validate()`.
- `ProfileTutorialSnapshot(IReadOnlyList<string> confirmedIds)`.
- `IReadOnlyList<string> ConfirmedIds` and `public void Validate()`.

두 struct의 public getter는 전체 `Validate()`를 먼저 수행한다. default settings는 invalid-zero mode, default tutorial은 null backing collection으로 invalid다. constructor는 invalid argument에 `ArgumentException` 계열, constructor-bypass/default를 public consumption에서 찾으면 `InvalidOperationException`을 던진다.

## Tutorial ID canonical value rule

각 ID는 null이 아닌 .NET string이어야 하고 unpaired surrogate가 없어야 하며 `NormalizationForm.FormC`와 ordinal-equal한 NFC여야 한다. 승인된 parent schema가 별도 ID 문법이나 non-empty 제한을 정의하지 않았으므로 M5D4는 빈 string을 새로 금지하지 않는다.

collection은 `StringComparer.Ordinal` 기준 strict ascending이어야 한다. 따라서 duplicate, reverse/unsorted, NFC normalization 뒤 같은 두 ID는 invalid다. constructor와 `Validate()`는 입력 순서를 정렬하거나 중복을 제거하지 않는다. 이 코어는 canonical 상태를 보존하되 잘못된 persisted order를 보정하지 않는다.

constructor는 source list와 각 string reference를 새 배열로 복사한다. string은 immutable이므로 text clone은 필요 없다. `ConfirmedIds`는 backing array clone의 read-only view를 반환하며 caller cast/mutation이 내부 상태를 바꾸지 못한다.

## 요구사항

- **REQ-M5D4-001:** Profile settings는 exact legal window mode, 세 Q1000 범위와 두 invert boolean을 engine-free immutable 값으로 보존하며 default/unknown/out-of-range를 거부한다.
- **REQ-M5D4-002:** tutorial confirmed IDs는 null/unpaired-surrogate/non-NFC/unsorted/duplicate 없이 ordinal strict sorted immutable collection으로 보존하며 입력을 암묵 보정하지 않는다.
- **REQ-M5D4-003:** settings/tutorial struct는 public `Validate()`와 getter 재검증으로 default/constructor-bypass 값을 거부하고 입력·반환 collection mutation에서 격리된다.
- **REQ-M5D4-004:** parent가 정의하지 않은 ID syntax/non-empty 제한, 실제 setting 적용 또는 tutorial state transition을 발명하지 않는다.
- **REQ-M5D4-005:** JSON/input/progression/full-profile/hash/IO/save/load/recovery/source authentication을 주장하거나 수행하지 않는다.
- **REQ-M5D4-006:** 기존 public API를 변경하지 않고 새 파일만 추가하며 UnityEngine, IO, JSON, crypto, clock, RNG, network, callback, Packages/ProjectSettings/assets를 건드리지 않는다.

## 수용 기준

- **AC-M5D4-001:** 두 window mode와 Q1000 `0`/`1000`, 네 boolean 조합의 대표값이 exact 보존되고 mode zero/unknown 및 각 volume `-1`/`1001`이 constructor와 bypass revalidation에서 거부된다.
- **AC-M5D4-002:** empty, single, ordinal sorted multi-ID와 빈 string ID가 exact 보존되고 null list/item, duplicate, reverse/unsorted, unpaired surrogate, decomposed non-NFC가 보정 없이 거부된다.
- **AC-M5D4-003:** source list 변경, returned collection의 비제네릭·제네릭 mutation 시도와 반복 getter가 내부 ID/order를 바꾸지 않는다.
- **AC-M5D4-004:** default settings/tutorial의 `Validate()`와 모든 public getter가 throw하고, reflection으로 삽입한 invalid mode/volume/list/order/text도 재검증에서 거부된다. 실패 후 별도 valid 생성은 정상이다.
- **AC-M5D4-005:** static review가 새 runtime source의 engine-free 경계, exact API/REQ trace, 입력·progression·JSON/hash/IO authority 부재와 allowlist 준수를 확인한다.
- **AC-M5D4-006:** Luna 독립 review와 focused/full EditMode, full PlayMode가 failed/skipped/inconclusive `0`으로 통과한다. value-core PASS를 실제 setting 적용, tutorial event 또는 persisted profile PASS로 확대하지 않는다.

## 정확한 구현 allowlist

Runtime:

- new `Assets/AcadeGameMaker/Runtime/Profile/ProfileSettingsTutorialSnapshots.cs` and `.meta`

Tests:

- new `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileSettingsTutorialSnapshotsTests.cs` and `.meta`

Documents:

- this contract
- later `docs/verification/2026-09-12-vd09-m5d4-contract-pregate.md`
- later `docs/verification/2026-09-12-vd09-m5d4-implementation-evidence.md`
- later `docs/verification/2026-09-12-vd09-m5d4-luna-independent-review.md`
- `docs/README.md` minimum index/status link

M5D3 파일과 기존 asmdef/test, 그 밖의 Runtime/Tests, Packages, ProjectSettings, prefab/scene/assets/media 변경은 금지한다.

## 검증 순서와 중단 조건

Luna pre-gate → Astra Approved → Terra allowlist implementation → root Unity execution → Luna independent post-review → Astra integration 순서다. ID 문법/non-empty 요구, 다른 window mode/volume representation, actual settings/tutorial owner, input binding, full codec/IO 또는 allowlist 밖 변경이 필요하면 구현하지 않고 Astra로 반환한다.

## 롤백과 참여 기록

Rollback point는 M5D3 Verified다. 되돌리기는 승인 시 M5D4의 새 파일과 문서/index hunk만 대상으로 하며 reset/clean/checkout/다른 dirty 파일 삭제를 사용하지 않는다. commit과 remote publication은 암시되지 않는다.

Astra가 Approved VD-09 field constraints를 기계적 값 경계로 분리해 초안을 작성했다. Luna pre-gate가 P0=0/P1=0으로 통과했고 surrogate 선검사·normalization collision·empty list와 empty ID 구분을 focused test에 포함하라는 P2를 남겼다. Astra가 이를 검증 지시로 수용해 Approved로 전환했다. 구현/검증은 아직 수행되지 않았다. 외부 모델, Ollama, 자동화, network call과 Pro 실행은 사용하거나 주장하지 않는다.
