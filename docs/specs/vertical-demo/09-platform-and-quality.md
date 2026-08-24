# [VD-09] Platform and Quality Baseline

- Status: Review
- Owner: Sol
- Unit implementation: Terra
- Verification: Luna
- Last updated: 2026-08-24

## Scope

Unity·URP 2D·Windows 기준, 입력 기기·화면·프레임 제품 기준, 저장 어댑터와 빌드 품질을 포함한다.

## Non-scope

출시 인증, 콘솔·모바일·웹, 온라인 서비스, 활성 런 저장과 보류된 제작 도구는 포함하지 않는다.

## Contract

수직 데모의 엔진, 렌더링, 대상 플랫폼, 입력·성능·저장·빌드 기준을 소유한다. 패키지 추가, 전역 설정, 공개 스키마 변경은 Sol 승인 없이 할 수 없다.

### Inputs

승인된 제품 기준, 프로필 snapshot 요청, 빌드 대상, 프로젝트 설정과 검증 실행 요청.

### Outputs

버전 저장·로드 결과, 안전 복구 결과, 환경 식별 정보, 편집기·Windows 빌드와 품질 증적을 제공한다.

### Owned state

Unity 전역 설정, 패키지 잠금, 입력·표시·성능 기준, 버전 저장 스키마와 원자 파일 트랜잭션을 소유한다. 라이브 런·선택·전이 상태는 소유하지 않는다.

### Invariants

- Unity 온라인 기능은 기본 비활성이다.
- 저장 payload에는 활성 scene object 또는 런타임 Component 참조가 없다.
- 비정상 저장은 기존 정상 snapshot을 훼손하지 않고 플레이 가능한 거점 상태로 복구한다.

### Versioned profile schema

영구 상태는 UTF-8 JSON 단일 snapshot `profile.json`에만 기록한다. `payloadSha256`은 `integrity` object를 제외한 canonical payload byte의 lowercase SHA-256이며 보안 서명이 아니라 손상 검출 값이다.

profile canonical byte 규칙은 다음과 같다.

- object property는 아래 schema table 순서로 한 번씩만 쓰며 duplicate·unknown property는 invalid다.
- 모든 number는 base-10 integer ASCII로 쓰고 leading zero, plus sign, exponent, decimal point를 금지한다. boolean과 null은 lowercase literal이다.
- 모든 property name과 string value는 Unicode NFC로 정규화한다. unpaired surrogate는 invalid다.
- string은 quote·backslash와 U+0000~001F만 escape한다. `\b`, `\t`, `\n`, `\f`, `\r`을 우선하고 나머지 control은 lowercase `\u00xx`를 사용한다. solidus와 그 밖의 Unicode scalar는 escape하지 않고 UTF-8로 쓴다.
- ID array는 NFC 뒤 ordinal 오름차순·중복 제거한다. JSON은 insignificant whitespace 없이 쓰고 파일 끝에 newline 없이 저장한다.
- 비어 있지 않은 `bindingOverridesJson`은 arbitrary text로 hash하지 않는다. 먼저 JSON parse와 duplicate-key 검사를 거쳐 RFC 8785 JSON Canonicalization Scheme byte로 만든 뒤 그 UTF-8 text를 outer string value로 위 규칙에 따라 encode한다. 빈 문자열은 `no override` sentinel로서 inner parse·canonicalize를 생략한다. 비어 있지 않은 값의 parse·canonicalize 실패는 binding 부분 복구 대상이다.
- hash 입력은 `integrity` property 전체를 생략한 나머지 top-level object의 위 canonical byte다. 최종 파일은 같은 규칙으로 `integrity`까지 포함해 작성한다.

| Path | Type and contract |
|---|---|
| `schemaVersion` | integer `1` |
| `profileRevision` | integer `>=0`; 성공한 저장마다 직전 정상 snapshot보다 1 증가 |
| `settings.windowMode` | `Windowed` 또는 `BorderlessFullscreen` |
| `settings.masterVolumeQ1000` | integer `0..1000` |
| `settings.musicVolumeQ1000` | integer `0..1000` |
| `settings.sfxVolumeQ1000` | integer `0..1000` |
| `settings.gamepadAimInvertX/Y` | boolean; binding override와 분리 |
| `input.inputActionsAssetId` | 비어 있지 않은 저작 stable ID; 현재 `GameInput.inputactions`와 exact match |
| `input.bindingSchemaVersion` | integer `1` |
| `input.bindingOverridesJson` | Input System이 생성한 override JSON string; override가 없으면 빈 문자열이며 parse·canonicalize를 생략 |
| `tutorial.confirmedIds` | sorted unique string array |
| `progression.lastOfferedSeed` | null 또는 `101`, `202`, `303`, `404` |
| `progression.committedChoice` | null, `Extraction`, `Solidarity` |
| `progression.consentState` | `RefusesOwnershipTransfer`, `OffersScopedResonance`, `ResonanceLender`, `ImprintSevered` |
| `progression.grantedSkill` | null, `CompressionVerdict`, `CommonReferencePlane` |
| `progression.completedBranches` | sorted unique array of `Extraction`, `Solidarity` |
| `integrity.payloadSha256` | 64-character lowercase hexadecimal string |

wall-clock 저장 시각은 payload에 넣지 않고 진단 로그에만 기록한다. 알 수 없는 field를 gameplay 의미로 추측하지 않는다. 활성 런·위치·속도·체력·방·적·보상·원정 자산·활성 전이와 runtime object reference는 schema에 존재할 수 없다.

### Atomic write contract

세 파일은 모두 `Application.persistentDataPath` 바로 아래의 sibling이다.

- `profile.json`: 현재 정상 snapshot
- `profile.prev.json`: 직전 정상 snapshot
- `profile.tmp.json`: 작성·검증 중인 후보

저장은 다음 순서를 지키며 어느 실패 단계에서도 기존 `profile.json`을 먼저 삭제·truncate·overwrite하지 않는다.

1. 직전 정상 revision보다 1 큰 `profileRevision`으로 canonical payload와 hash를 만든다.
2. 새 byte 전체를 `profile.tmp.json`에 truncate-write하고 file buffer를 storage까지 flush한다.
3. file handle을 닫은 뒤 temp를 다시 읽어 schema, field constraint, canonical byte와 `payloadSha256`를 검증한다.
4. 현재 `profile.json`이 있으면 Windows에서 `File.Replace(profile.tmp.json, profile.json, profile.prev.json, ignoreMetadataErrors: true)` 또는 byte-equivalent 단일 OS replace를 호출한다. 기존 previous가 있으면 이 호출이 교체 전 primary byte로 previous를 대체한다.
5. primary가 없는 최초 저장이면 검증된 temp를 같은 directory 안에서 `profile.json`으로 move한다.
6. replace 호출 전 단계의 실패는 정상 primary와 previous를 변경하지 않는다. replace 호출이 예외·부분 오류를 반환하면 성공을 발행하지 않고 primary·previous·temp를 모두 재검증하며, 최소 하나의 직전 정상 snapshot byte가 primary 또는 previous에 남아 있어야 한다. 다음 load recovery가 그 snapshot을 선택한다. 원인·경로·revision과 관찰된 세 파일 상태를 진단 로그에 남긴다.

성공은 replacement/move가 끝난 뒤에만 발행한다. 시작 시 남은 temp, 손상 primary와 previous 중 선택·격리는 별도 load recovery contract에서 정한다.

### Load recovery contract

앱 시작 시 파일을 아래 순서로 판정한다. 모든 candidate는 JSON parse, `schemaVersion`, exact field set·constraint, canonical byte와 hash를 통과해야 유효하다.

1. primary·previous·temp를 모두 독립 검증한다. 유효한 `profile.json`을 항상 선택하고, 더 높은 revision의 valid temp가 있어도 temp는 commit되지 않은 candidate이므로 로드하지 않는다. primary가 유효해도 invalid previous는 `recovery/profile-invalid-previous-{UTC}-{hash8}.json`으로 격리·기록한다.
2. stale temp는 `recovery/profile-stale-temp-{UTC}-{hash8}.json`으로 격리한다. hash를 계산할 수 없으면 suffix는 `nohash`다.
3. primary가 없거나 무효면 `profile.prev.json`을 같은 validator로 검사한다.
4. previous revision `r`이 유효하면 무효 primary를 recovery directory에 보존하고 previous 상태를 메모리에 로드한 뒤 `resultRevision=r+1`로 atomic save해 새 primary를 만든다. previous 원본은 유지한다.
5. primary와 previous가 모두 없거나 무효면 모든 읽을 수 있는 비정상 파일을 격리한다. default는 committed source가 아니므로 `sourceRevision=-1`로 취급하고 첫 atomic save 결과를 revision 0으로 만든 뒤 거점에서 시작한다.
6. 지원하지 않는 schema version은 v1로 추측·migration하지 않고 `profile-unsupported-v{version}-{UTC}-{hash8}.json`으로 격리한 뒤 previous 또는 default 경로를 따른다.
7. profile revision `r` payload는 유효하지만 `inputActionsAssetId`, `bindingSchemaVersion`이 현재 asset과 다르거나 override JSON 적용이 실패하면 settings·tutorial·progression은 유지하고 input block만 현재 기본값으로 교체해 `resultRevision=r+1`로 atomic save한다.
8. 복구·부분 복구는 원인, 선택 source, 원본 revision, 결과 revision과 격리 경로를 진단 로그에 남기고 플레이어에게 launch당 한 번 비차단 알림을 표시한다.
9. 복구 후 atomic save가 실패해도 검증된 previous 또는 default in-memory profile로 거점 진입을 허용하되, 보존되지 않았음을 알리고 다음 저장에서 다시 시도한다.

손상·미지원 파일은 삭제하지 않는다. recovery filename의 UTC는 진단용이며 gameplay 상태와 revision 결정에 사용하지 않는다. 동일 이름 충돌 시 ordinal suffix를 붙인다.

## Requirements

- **REQ-PLAT-001:** Unity 6.3 LTS, C#, URP 2D로 Windows PC 데모를 만든다.
- **REQ-PLAT-002:** Windows 10/11 x64에서 키보드와 XInput 게임패드를 지원하고 마우스를 UI·조준에 허용하며, 16:9 1920×1080 기준의 창/전체 화면 전환, 60 FPS, 1/60초 고정 물리 간격을 사용한다.
- **REQ-PLAT-003:** 저장 데이터는 버전을 포함하고 실패 시 플레이 가능한 초기 상태로 안전하게 복구한다.
- **REQ-PLAT-004:** 개발 경고를 제외한 처리되지 않은 예외와 누락 참조 없이 기준 경로를 완주한다.
- **REQ-PLAT-005:** 편집기와 Windows 빌드에서 같은 게임플레이 결과와 상태 전이를 재현한다.
- **REQ-PLAT-006:** 활성 런은 저장하거나 재개하지 않고, 설정·입력 바인딩·튜토리얼 확인과 확정된 선택·해금 기술·완료 분기만 각 승인 시점에 원자 저장한다.
- **REQ-PLAT-007:** `com.unity.inputsystem` 1.20.0을 고정하고 Active Input Handling은 Input System Package (New)만 사용하며 구형 Input Manager와 Both 모드를 금지한다.
- **REQ-PLAT-008:** 영구 상태는 schemaVersion 1의 canonical 단일 `profile.json`에만 기록하고 settings·input override·tutorial·확정 progression과 integrity hash만 포함하며 활성 런 상태를 배제해야 한다.
- **REQ-PLAT-009:** 저장은 persistentDataPath의 temp를 완전히 기록·flush·재검증한 뒤에만 primary를 원자 교체하고 직전 primary를 previous로 보존하며 실패 시 기존 정상본을 변경하지 않아야 한다.
- **REQ-PLAT-010:** 시작 시 유효 primary를 우선하고 stale temp를 로드하지 않으며 primary 실패 시 유효 previous, 그마저 없으면 default profile로 복구하고 비정상 파일을 보존해야 한다.
- **REQ-PLAT-011:** revision `r` profile이 유효하되 input asset·binding schema·override 적용만 실패하면 비입력 상태를 보존하고 input block만 기본값으로 복구해 result revision `r+1`로 원자 저장해야 한다.

## Acceptance criteria

### AC-PLAT-001 — 환경 식별

- **Given** 검수 대상 프로젝트와 빌드가 있고
- **When** 프로젝트 설정과 빌드 정보를 확인하면
- **Then** 엔진 버전, 렌더 파이프라인, Windows x64 대상, 입력 기기, 1920×1080 기준, 화면 모드, 60 FPS와 1/60초 물리 간격이 이 스펙과 일치한다.

### AC-PLAT-002 — 기준 경로 안정성

- **Given** 깨끗한 로그와 신규 상태가 있고
- **When** 편집기와 Windows 빌드에서 VD-10 기준 경로를 각각 실행하면
- **Then** 처리되지 않은 예외, 누락 참조, 진행 불능 없이 끝나고 동일한 필수 결과를 남긴다.

### AC-PLAT-003 — 저장 복구

- **Given** 정상 저장과 지원하지 않는 버전 또는 손상된 저장의 검수 표본이 있고
- **When** 각각 로드하면
- **Then** 정상 저장은 보존 상태를 복구하고 비정상 저장은 진단 기록 후 플레이 가능한 안전 상태로 전환된다.

### AC-PLAT-004 — 활성 런 비보존

- **Given** 원정 중 방·적·보상·활성 전이 상태와 별도로 확정된 프로필 상태가 있고
- **When** 앱을 종료한 뒤 다시 시작하면
- **Then** 활성 런 상태는 복원되지 않고 거점의 새 원정 상태가 되며 승인 시점까지 확정된 프로필 상태만 복원된다.

### AC-PLAT-005 — 입력 패키지 고정

- **Given** 프로젝트 manifest, packages lock과 Player Settings가 있고
- **When** 입력 의존성과 Active Input Handling을 검사하면
- **Then** `com.unity.inputsystem`은 정확히 1.20.0이고 신규 Input System만 활성화되며 레거시 입력 API 의존성이 없다.

### AC-PLAT-006 — 단일 프로필 스키마와 canonical payload

- **Given** 같은 보존 상태를 다른 삽입 순서로 만든 두 snapshot과 활성 런 상태가 있고
- **When** 각각 `profile.json`으로 직렬화해 field·byte·hash를 비교하면
- **Then** 두 파일은 schema 순서·sorted unique array·LF·UTF-8과 payloadSha256가 exact match하고 승인 필드만 존재하며 활성 런·runtime reference·wall-clock 시각은 포함되지 않는다.

### AC-PLAT-007 — 원자 저장과 실패 격리

- **Given** 정상 primary·previous와 다음 revision snapshot이 있고 temp write·flush·재검증·replace 각 단계의 주입 가능한 실패가 있고
- **When** 각 단계에서 저장을 중단한 뒤 primary·previous byte와 반환 결과를 확인하면
- **Then** 성공한 경우만 새 revision이 primary이고 직전 primary가 previous이며, replace 전 실패에서는 둘의 byte가 exact match하고 replace 단계 오류에서는 성공 사건 없이 세 파일을 재검증해 최소 하나의 직전 정상 snapshot이 primary 또는 previous에 남고 원인·경로·revision·파일 상태가 기록된다.

### AC-PLAT-008 — 시작 파일 선택과 손상 복구

- **Given** valid·missing·corrupt·unsupported primary, valid·invalid previous와 더 높은 revision의 valid stale temp 조합이 있고
- **When** 각 조합에서 profile load를 실행하면
- **Then** valid primary→valid previous→default 순서만 사용하고 temp는 절대 로드하지 않으며 valid primary 옆 invalid previous를 포함한 비정상 파일은 recovery에 보존되고, previous `r`은 `r+1`, uncommitted default `-1`은 `0`으로 atomic save되며 launch당 알림은 한 번뿐이다.

### AC-PLAT-009 — binding 부분 복구

- **Given** settings·tutorial·progression과 hash는 유효하지만 input asset ID mismatch, binding schema mismatch 또는 override 적용 실패가 각각 있고
- **When** profile을 로드하면
- **Then** 비입력 field는 byte-equivalent 의미로 유지되고 input block만 현재 기본값이 되며 source `r`에서 result `r+1` 저장과 단일 비차단 알림·진단 기록이 발생한다.

## Verification

프로젝트 설정 스냅샷, package lock, 빌드 로그, 성능 캡처, canonical 직렬화, 단계별 I/O 실패 주입, primary/previous/temp 조합과 binding 부분 복구 테스트로 검증한다. `OD-PLAT-001`의 입력·저장·복구 계약은 해결됐고 [Luna 독립 문서 검토](../../verification/2026-08-24-od-plat-001-luna-review.md)는 PASS다. 실제 구현 뒤에는 AC별 실행 증적이 별도로 필요하다.

기술 근거: [RFC 8785 JSON Canonicalization Scheme](https://www.rfc-editor.org/rfc/rfc8785), [Microsoft .NET File.Replace](https://learn.microsoft.com/dotnet/api/system.io.file.replace).

## Traceability

[환경·제품 기준 승인 기록](../../approvals/2026-08-24-environment-and-product-baseline.md), [ADR-0010](../../adr/0010-unity-for-the-asset-first-prototype.md), [ADR-0013](../../adr/0013-fifteen-minute-vertical-slice.md), [ADR-0018](../../adr/0018-vertical-demo-p0-integration.md), [VD-05](./05-failure-and-persistence.md), [VD-10](./10-verification-script.md)
