---
status: Verified
---

# VD-09 M5D7F profile quarantine adapter

- Date: 2026-09-13
- Owners: Astra contract/approval; Terra implementation; Luna independent pre/post review
- Dependencies: M5D7B, M5D7E Verified
- Requirements: `REQ-PLAT-010`
- Acceptance IDs: `AC-M5D7F-001` through `AC-M5D7F-010`
- Astra approval: 2026-09-13 after Luna second pre-gate PASS (P0=0, P1=0); exact allowlist only.
- Astra verification: 2026-09-13 after Luna R2 implementation PASS (P0=0, P1=0); residual P2=3 accepted as non-blocking test-precision notes.

## 목적과 경계

M5D7F는 M5D7E가 발행한 non-`None` 파일 보존 의도 하나를 실행한다. caller가 준 persistent root 바로 아래 exact source leaf를 재관찰하고, `recovery` 하위 폴더의 충돌 없는 진단명으로 same-volume `File.Move`해 원본 bytes를 보존한다. 이동 전 source가 더 이상 보존 대상이 아니면 fail closed하며 정상 profile을 격리하지 않는다.

이 단위는 load 선택, profile 변환, atomic save, 복구 순서 orchestration, 진단 로그 sink, launch 알림, retry, 삭제, copy, Unity path 조회, gameplay/hub 진입을 소유하지 않는다. Caller가 UTC와 M5D7E candidate/reason을 제공하며 system clock을 읽지 않는다.

## 승인 API

- `ProfileQuarantineOutcome`: `Preserved`, `SourceMissing`, `FailedBeforeMove`, `MoveOutcomeUncertain`.
- `ProfileQuarantineStage`: `None`, `SourceRevalidation`, `RecoveryDirectoryPreparation`, `DestinationAllocation`, `MoveCall`, `PostMoveValidation`.
- `ProfileQuarantineFileState`: `NotProbed`, `Missing`, `Present`, `Unreadable`.
- immutable `ProfileQuarantineResultV1` getters: outcome, failure stage, source role/reason, source/destination states, destination full path, hash suffix, collision ordinal, and `Validate()`.
- `ProfileQuarantineServiceV1.Preserve(string rootDirectoryPath, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, DateTimeOffset utcNow)`.
- internal injectable file/hash operation port and overload for deterministic fault tests; no public caller-supplied source leaf or destination name exists.

`rootDirectoryPath`는 nonempty fully-qualified non-root path이고 `Path.GetFullPath` 후 root가 아닌 trailing separator를 제거한다. 이 normalized full path 자체를 Windows `StringComparer.OrdinalIgnoreCase` lease key로 사용하며 symlink/junction을 resolve하지 않는다. `utcNow.Offset`은 exact zero여야 하며 format은 invariant UTC `yyyyMMdd'T'HHmmssfff'Z'`다. Candidate는 full validate하며 reason은 non-`None`이고 role/kind/classification과 M5D7E `PreservationFor`/`TempPreservationFor` 결과에 exact 일치해야 한다. Programmer/default/role/reason/time/path 오류는 lock/IO 전에 `ArgumentException`이다.

Source leaf는 role로만 고정한다: primary=`profile.json`, previous=`profile.prev.json`, temp=`profile.tmp.json`. Recovery directory는 exact sibling child `recovery`; caller leaf, traversal, absolute destination, alternate extension은 없다.

## 재관찰과 명명

Source presence는 `File.GetAttributes`로 확인하고 `FileNotFoundException`/`DirectoryNotFoundException`만 missing이다. Missing은 move 없이 `SourceMissing/SourceRevalidation`이다. 접근·read 실패는 unreadable observation이며 candidate가 `Unreadable`일 때만 계속할 수 있다; 그 외에는 `FailedBeforeMove/SourceRevalidation`이다.

읽을 수 있는 source는 새 bytes를 M5D7B로 decode한다.

- `InvalidPrimary`/`InvalidPrevious`는 재decode `Invalid`만 허용한다.
- `UnsupportedPrimary`/`UnsupportedPrevious`는 재decode `UnsupportedProfileSchema`이고 candidate의 unsupported version과 exact 같아야 한다.
- `UnreadablePrimary`/`UnreadablePrevious`가 이제 읽히면 `Invalid` 또는 임의 version의 `UnsupportedProfileSchema`만 계속 허용한다. current/input-recoverable이면 source changed로 move를 금지한다.
- `StaleTemp`는 temp라는 provenance 자체가 stale이므로 현재 decode classification과 무관하게 이동한다.

M5D7E candidate에는 원본 bytes/hash가 없으므로 재관찰은 의도적으로 classification-only다. Invalid가 다른 invalid bytes로, unsupported가 같은 version의 다른 bytes로 바뀐 경우에도 현재 파일은 여전히 같은 승인 보존 범주이므로 이동할 수 있다. 이 adapter는 M5D7E 관찰 시점과 byte identity가 같다고 주장하지 않으며, 이동 직전 새로 읽은 exact bytes와 그 digest만 post-move proof로 사용한다.

읽은 bytes의 SHA-256 lowercase hexadecimal 앞 8자를 hash suffix로 사용한다. 읽을 수 없는 허용 source만 exact `nohash`다. Bytes/hash는 result나 mutable shared state에 보존하지 않는다.

Base leaf는 다음과 같다.

- `InvalidPrimary`: `profile-invalid-primary-{UTC}-{hash8}.json`
- `InvalidPrevious`: `profile-invalid-previous-{UTC}-{hash8}.json`
- `UnsupportedPrimary`/`UnsupportedPrevious`: `profile-unsupported-v{version}-{UTC}-{hash8}.json`
- `UnreadablePrimary`: `profile-unreadable-primary-{UTC}-{hash8|nohash}.json`
- `UnreadablePrevious`: `profile-unreadable-previous-{UTC}-{hash8|nohash}.json`
- `StaleTemp`: `profile-stale-temp-{UTC}-{hash8|nohash}.json`

Base destination이 존재하면 `.json` 앞에 `-1`, `-2` 순서의 smallest nonnegative collision ordinal suffix를 붙인다. Result collision ordinal은 base `0`, suffixed filename은 해당 양수다. 증가는 `checked`이며 `int.MaxValue` 후보도 점유되면 wrap/negative/재시도 없이 destination-allocation failure다. Presence 판정이 불확실해도 move하지 않는다. Destination presence는 `File.GetAttributes` 의미의 typed probe를 사용하고 only not-found를 free로 보며 boolean `File.Exists`를 사용하지 않는다.

## 이동과 결과

Recovery directory 준비와 destination allocation이 끝나면 exact `File.Move(source, destination)`를 한 번만 호출한다. overwrite/delete/copy/retry/rename fallback은 금지한다. 동일 normalized root의 in-process 작업은 source revalidation부터 postcheck까지 하나의 per-root monitor로 serialize한다. Registry mutex 아래 active+waiting ref-count를 먼저 증가시킨 뒤 root monitor를 기다리고, transaction이 monitor를 놓은 다음 registry mutex 아래 감소시켜 exact zero일 때만 같은 entry를 제거한다. Case/trailing-separator aliases는 같은 entry이며 waiter가 남은 entry 제거와 split lock을 금지한다. 다른 root는 registry book-keeping 순간 외에 병행 가능하다. Cross-process와 symlink/junction identity serialization은 범위가 아니며 후속 launch coordinator가 single-process writer를 보장한다.

Move가 정상 반환하면 source missing, destination present를 재probe하고 읽을 수 있었던 경우 destination bytes의 SHA-256 전체가 pre-move digest와 exact 같아야 한다. Unreadable/nohash 경우 destination의 존재만 증명하고 content-valid를 주장하지 않는다. 조건이 모두 맞을 때만 `Preserved/None`과 destination path를 반환한다.

Move가 throw하면 source/destination을 독립 probe해 `MoveOutcomeUncertain/MoveCall`; 성공 반환 뒤 postcheck 실패는 `MoveOutcomeUncertain/PostMoveValidation`이다. 자동 retry/rollback/delete는 없다. Move 전 recoverable failure는 `FailedBeforeMove`와 exact stage다. 예상 가능한 `IOException`, `UnauthorizedAccessException`, `SecurityException`만 typed failure로 포착하고 unexpected/fatal exception은 lease를 `finally`로 해제한 뒤 전파한다.

Result의 허용 행렬은 다음 네 행뿐이다. 모든 getter는 먼저 전체 `Validate()`를 수행한다.

| Outcome | Stage | Source state | Destination state | Destination path / hash suffix / ordinal |
|---|---|---|---|---|
| `Preserved` | `None` | exact `Missing` | exact `Present` | normalized root의 exact `recovery` child인 nonempty fully-qualified path / lowercase 8-hex 또는 `nohash` / `>=0` |
| `SourceMissing` | exact `SourceRevalidation` | exact `Missing` | exact `NotProbed` | empty / empty / exact `-1` |
| `FailedBeforeMove` | exact `SourceRevalidation`, `RecoveryDirectoryPreparation`, 또는 `DestinationAllocation` | `Present` 또는 `Unreadable` | exact `NotProbed` | empty / empty / exact `-1` |
| `MoveOutcomeUncertain` | exact `MoveCall` 또는 `PostMoveValidation` | `Missing`, `Present`, 또는 `Unreadable` | `Missing`, `Present`, 또는 `Unreadable` | chosen normalized recovery child full path / valid suffix / `>=0` |

모든 outcome에서 role/reason은 known non-`None`이며 서로 exact compatible해야 한다. `Preserved`의 `nohash`는 unreadable candidate/reason 또는 unreadable stale temp에만 허용하고 readable proof에서는 8-hex만 허용한다. Default/unknown enum, 다른 outcome-stage-state-path-suffix-ordinal 조합, traversal/non-recovery destination, reflection-bypassed proof는 `InvalidOperationException`이다.

## 요구사항

- **REQ-M5D7F-001:** M5D7E role/reason과 exact source leaf를 재검증해 정상/current/input-recoverable primary·previous를 격리하지 않는다.
- **REQ-M5D7F-002:** 승인 reason·UTC·hash8/nohash와 smallest collision ordinal로 recovery child destination을 결정한다.
- **REQ-M5D7F-003:** source를 delete/copy/overwrite 없이 단일 same-volume move하고 성공 뒤 source/destination/digest를 검증한다.
- **REQ-M5D7F-004:** pre-move/uncertain 결과, recoverable-only exception containment와 per-root serialization을 fail closed한다.
- **REQ-M5D7F-005:** no selection/transformation/save/log/notification/retry/Unity/gameplay/network/RNG authority와 기존 API/asmdef/assets 무변경을 지킨다.

## 수용 기준

- **AC-M5D7F-001:** invalid primary/previous와 stale temp의 real isolated-directory move는 exact source bytes를 exact base destination에 보존하고 source를 제거한다.
- **AC-M5D7F-002:** unsupported primary/previous는 exact decoded version을 filename에 넣고 migration 없이 이동한다. Version mismatch/source change는 move 0회다.
- **AC-M5D7F-003:** readable bytes hash8는 full SHA-256 lowercase 첫 8자이며 unreadable 허용 source는 `nohash`; UTC format/offset과 culture 독립성을 검증한다.
- **AC-M5D7F-004:** base와 `-1..n` 충돌에서 smallest free ordinal을 선택하고 기존 recovery files를 변경하지 않는다.
- **AC-M5D7F-005:** stale temp는 current/higher revision/input-recovery/invalid/unsupported 모두 보존하지만 source로 채택하거나 profile 의미를 주장하지 않는다.
- **AC-M5D7F-006:** source missing, access/read/recovery-directory/allocation failure는 move 0회와 typed pre-move result; current/input-recovery로 바뀐 primary/previous는 source-changed fail closed다.
- **AC-M5D7F-007:** move throw와 postcheck source-remains/destination-missing/digest-mismatch는 retry/rollback/delete 없이 uncertain 및 독립 typed presence state probe다. Call log는 `File.Move` 상당 호출이 최대 1회이고 readable 성공의 full digest postcheck를 증명한다.
- **AC-M5D7F-008:** normalized full path+Windows ordinal-ignore-case same-root aliases와 barrier로 등록된 복수 waiter는 source revalidation→destination allocation→move→postcheck를 serialize하고 final registry를 제거하며 different-root는 병행한다. Symlink/cross-process serialization을 주장하지 않는다.
- **AC-M5D7F-009:** null/relative/root/bad UTC/default·role·reason mismatch와 네 행 전체 result reflection/getter matrix는 IO 전 또는 getter에서 거부된다. Source/destination access·unexpected presence 오류, collision `int.MaxValue` overflow, unexpected/fatal exception과 lease cleanup을 각각 검증한다.
- **AC-M5D7F-010:** static source review는 exact one-move path, `File.GetAttributes`, SHA-256, no delete/copy/overwrite/retry/fallback/clock/Unity를 확인하고 focused/full EditMode/full PlayMode가 fail/skip/inconclusive 0, Luna P0/P1 0이며 launch recovery orchestration PASS로 과장하지 않는다.

## 정확한 allowlist

- `Assets/AcadeGameMaker/Runtime/Profile/ProfileQuarantineServiceV1.cs` + `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileQuarantineServiceV1Tests.cs` + `.meta`
- this contract
- `docs/verification/2026-09-13-vd09-m5d7f-contract-pregate.md`
- `docs/verification/2026-09-13-vd09-m5d7f-implementation-evidence.md`
- `docs/verification/2026-09-13-vd09-m5d7f-luna-independent-review.md`
- minimal `docs/README.md` entry

Existing M5D3-M5D7E source/tests, asmdefs, Packages, ProjectSettings, scenes, prefabs, input/actions and other assets are read-only. Luna pre-gate → Astra Approved → Terra → root tests → Luna post-review → Astra Verified 순서다.
