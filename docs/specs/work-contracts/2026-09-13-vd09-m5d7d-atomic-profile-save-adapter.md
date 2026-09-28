---
status: Verified
---

# VD-09 M5D7D atomic profile save adapter

- Date: 2026-09-13
- Owners: Astra approval; Terra implementation; Luna independent pre/post review
- Dependencies: M5D7A-M5D7C Verified
- Requirements: `REQ-PLAT-008`, `REQ-PLAT-009`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D7D-001` through `AC-M5D7D-010`
- Astra approval: 2026-09-13 after Luna second pre-gate PASS (P0=0, P1=0); exact allowlist only.
- Astra verification: 2026-09-13 after focused EditMode 29/29, full EditMode 584/584, full PlayMode 576/576, and Luna R2 post-review PASS (P0=0, P1=0).

## 목적과 경계

M5D7D는 caller가 이미 revision을 확정한 current-input-compatible `ProfileCanonicalDocumentV1`을 한 directory의 `profile.tmp.json`에 전체 기록·storage flush·close·재검증한 뒤 `profile.json`으로 원자 commit한다. 기존 primary가 있으면 Windows `File.Replace(temp, primary, previous, true)`, 없으면 same-directory move를 사용한다.

이 단위는 profile state/revision을 만들거나 증가시키지 않는다. primary/previous/temp load 선택, stale/invalid quarantine, default/previous/input recovery 계획, Input System 적용, Unity `Application.persistentDataPath`, 알림과 gameplay 진입은 후속 owner 책임이다.

## 승인 API

새 `AcadeGameMaker.Profile` engine-free file-I/O assembly file 하나에 다음을 추가한다.

- `ProfileAtomicSaveOutcome`: `CommittedFirst`, `CommittedReplacement`, `FailedBeforeCommit`, `CommitOutcomeUncertain`.
- `ProfileAtomicSaveStage`: `None`, `DirectoryPreparation`, `TempWriteFlushClose`, `TempReadValidation`, `ExistingPrimaryValidation`, `CommitCall`, `PostCommitValidation`.
- `ProfileStoredFileState`: `NotProbed`, `Missing`, `ValidCurrentInput`, `ValidInputRecoveryRequired`, `UnsupportedSchema`, `Invalid`, `Unreadable`.
- immutable `ProfileAtomicSaveResultV1` getters: outcome, failure stage, primary/previous/temp states, committed revision, and `Validate()`.
- `ProfileAtomicSaveServiceV1.Save(string directoryPath, ProfileCanonicalDocumentV1 document)` production overload.
- internal injectable file-operation port and internal overload for deterministic EditMode fault tests; it is not public gameplay API.

`directoryPath`는 nonempty이며 `Path.IsPathFullyQualified`가 true인 absolute path여야 한다. `Path.GetFullPath` 후 trailing directory separator를 root가 아닌 경우 제거하고 Windows `StringComparer.OrdinalIgnoreCase` key로 사용한다. filesystem root 자체와 normalization 실패는 거부한다. normalized directory 아래 exact three filenames만 `Path.Combine`하며 filenames/caller-supplied leaf/path traversal API는 없다. Directory가 없으면 생성한다. document는 entry에서 full validate하고 `Snapshot.Input.IsCurrentCompatible`을 요구한다. mismatch candidate는 `ArgumentException`. programmer argument/default 오류는 IO와 lock acquisition 전에 throw하며 어떤 file도 건드리지 않는다.

동일 normalized directory의 in-process Save는 per-directory ordinal-ignore-case lock으로 serialize한다. Registry entry는 monitor/semaphore와 active+waiting lease ref-count를 가진다. Registry mutex 아래 ref-count를 먼저 증가시킨 뒤 directory lock을 기다리고, transaction 종료 후 directory lock을 놓은 다음 registry mutex 아래 ref-count를 감소시켜 정확히 zero일 때만 같은 entry를 제거한다. waiter가 남아 있는 entry를 제거하거나 같은 key에 두 lock을 만들 수 없다. 다른 directory transaction은 registry book-keeping 순간 외에는 전역 직렬화하지 않는다. Cross-process locking은 범위가 아니며 후속 integration에서 single-process writer를 보장한다.

### 2026-09-28 M5D7Q-C integration amendment

위 문단은 M5D7D 단위가 최초 검증된 당시의 범위다. 승인된
`2026-09-28-vd09-m5d7q-c-new-game-reset.md`의 후속 통합에서는 production
overload가 같은 root의 `profile.operation.lock` 메타데이터 파일을 OS 배타
핸들로 열고, `profile.reset.json` barrier를 확인한 뒤 기존 M5D7D 저장
트랜잭션에 들어간다. 따라서 “exact three filenames”는 저장 데이터 후보
`profile.json`/`profile.prev.json`/`profile.tmp.json`에 관한 규칙이며,
Q-C의 고정 lock/barrier 메타데이터를 금지하지 않는다. 내부 주입 테스트
overload의 기존 file-operation fault seam과 과거 검증 기록은 그대로
유지한다. Q-C 전체 구현·독립 검증이 끝나기 전에는 이 통합 예외를
새 게임 reset 완료 증거로 취급하지 않는다.

## result invariants

`CommittedRevision`은 성공 outcome에서만 supplied nonnegative revision이며 실패/uncertain에서는 exact `-1` sentinel이다. Result getters는 모든 field를 `Validate()`한 뒤 반환한다.

- `CommittedFirst`: stage `None`, primary `ValidCurrentInput`, temp `Missing`, committed revision `>=0`; previous는 post-probed `Missing` 또는 기존 state이며 `NotProbed`/`Unreadable`은 허용하지 않는다.
- `CommittedReplacement`: stage `None`, primary와 previous `ValidCurrentInput`, temp `Missing`, committed revision `>=0`.
- `FailedBeforeCommit`: stage exact `DirectoryPreparation`, `TempWriteFlushClose`, `TempReadValidation`, 또는 `ExistingPrimaryValidation`; committed revision `-1`. 아직 안전하게 관찰하지 못한 file은 `NotProbed`, 관찰한 file은 exact state다.
- `CommitOutcomeUncertain`: stage exact `CommitCall` 또는 `PostCommitValidation`, committed revision `-1`; primary/previous/temp는 commit attempt 뒤 모두 probe되어 `NotProbed`가 아니며 unreadable은 `Unreadable`로 표현한다.

Unknown/default outcome/stage/state, success-stage mismatch, failure revision, impossible success file states와 reflection-bypass 조합은 `InvalidOperationException`이다.

## exact transaction

1. directory 준비 실패: `FailedBeforeCommit/DirectoryPreparation`, 세 file state는 가능한 범위에서 probe하되 어떤 delete/move도 하지 않는다.
2. `profile.tmp.json`을 create/truncate, exclusive write, `FileOptions.WriteThrough`, 전체 canonical file bytes write, `Flush(true)`, close한다. 실패는 `FailedBeforeCommit/TempWriteFlushClose`; primary/previous는 변경하지 않는다.
3. close 후 temp를 새로 read하고 M5D7B decode한다. exact `ValidCurrentInput`이고 bytes와 revision이 supplied document와 exact 일치해야 한다. 아니면 `FailedBeforeCommit/TempReadValidation`; commit call 금지.
4. commit 직전에 primary 존재를 한 번 판정한다. 존재하면 primary를 exclusive read-close하고 M5D7B exact `ValidCurrentInput`으로 검증해 old bytes/revision proof를 보존한다. 실패는 `FailedBeforeCommit/ExistingPrimaryValidation`이며 replace를 호출하지 않는다. 성공하면 exact Windows `File.Replace(temp, primary, previous, true)`를 호출한다. Primary가 없으면 same-directory `File.Move(temp, primary)`다. production adapter는 copy+delete/primary pre-delete/manual rename fallback을 절대 쓰지 않는다.
5. commit call 성공 뒤 primary를 다시 read/decode하고 supplied bytes/revision과 exact 일치해야 한다. Replacement면 previous도 probe하고, temp는 missing이어야 한다. 성공 조건을 모두 만족할 때만 `CommittedFirst`/`CommittedReplacement`, failure stage `None`을 반환한다.
6. commit call이 throw하거나 postcheck가 실패하면 성공을 발행하지 않는다. primary/previous/temp를 각각 독립 probe해 `CommitOutcomeUncertain`과 `CommitCall` 또는 `PostCommitValidation`을 반환한다. 자동 rollback/delete/retry/fallback은 없다.

Probe는 missing과 read exception을 구분하고, bytes를 읽으면 M5D7B classification을 state로 투영한다. `ValidCurrentInput`, 두 input recovery classification을 합친 `ValidInputRecoveryRequired`, unsupported, invalid를 구분한다. Probe는 file을 변경하지 않는다. Exception type/message/path는 결과의 deterministic persisted/gameplay data가 아니며 이 API에 보관하지 않는다.

First commit 성공에서는 primary exact valid, temp missing이어야 하고 previous state는 `Missing` 또는 commit 전 존재했던 file의 post-probe state다. Replacement 성공에서는 primary exact valid, temp missing, previous가 commit 전 primary exact bytes/revision으로 `ValidCurrentInput`이며 preserved old bytes/revision proof와 exact 일치해야 한다. Existing primary가 unreadable/invalid/recovery-required/unsupported면 `FailedBeforeCommit/ExistingPrimaryValidation`로 commit을 거부하며 load recovery owner에게 넘긴다.

Injected-port observable call order is exact: validate args → acquire lease → ensure directory → write/open-truncate → full write → flush-to-storage → close → read/decode temp → primary existence check → when present read/decode/close primary → replace, otherwise move → post-probe primary/previous/temp → release lease. A thrown write/flush/close/read/decode/primary-validation step skips every later mutating call.

## 요구사항

- **REQ-M5D7D-001:** current-compatible canonical document만 temp write-through/flush/close/redecode 후 commit한다.
- **REQ-M5D7D-002:** existing valid primary는 exact `File.Replace`로 previous에 보존하고 first save는 same-directory move하며 primary pre-delete/overwrite fallback을 금지한다.
- **REQ-M5D7D-003:** commit 전 실패는 primary/previous 불변, commit call/postcheck 불확실은 세 파일 재probe와 uncertain 결과로 fail closed한다.
- **REQ-M5D7D-004:** production file adapter와 deterministic injected fault port가 단계·call order·byte ownership을 보존한다.
- **REQ-M5D7D-005:** result/default/reflection invariants, exact revision/state/outcome relations와 per-directory serialization을 검증한다.
- **REQ-M5D7D-006:** no revision/state transformation, selection/quarantine/recovery retry/input apply/Unity/notification/network/clock/RNG 경계와 existing API/asmdef/settings/assets 무변경을 지킨다.

## 수용 기준

- **AC-M5D7D-001:** empty directory first save 후 primary bytes가 supplied final bytes와 exact, temp missing, previous unchanged/missing, outcome first/revision exact다.
- **AC-M5D7D-002:** valid primary A 뒤 B replacement는 primary B, previous exact A, temp missing이며 call order가 write-flush-close-read-decode-replace-postprobe다.
- **AC-M5D7D-003:** directory/create/write/flush/close/temp-read/temp-corruption/temp-decode 각 failure injection은 commit call 0회, primary/previous exact unchanged, typed precommit stage/state를 반환한다.
- **AC-M5D7D-004:** replace/move throw와 postcommit missing/corrupt/wrong-revision/temp-remains/previous-not-old-primary는 retry/rollback/delete 없이 `CommitOutcomeUncertain`, exact stage와 three-state probe를 반환한다.
- **AC-M5D7D-005:** existing primary invalid/unsupported/input-recovery/unreadable은 `ExistingPrimaryValidation`에서 replace 없이 fail before commit; stale temp는 새 candidate write에서 truncate되지만 primary/previous는 commit 전 그대로다. Injected call log가 승인 exact order와 later-call suppression을 증명한다.
- **AC-M5D7D-006:** relative/empty/null/root path, normalization failure, default/malformed/mismatch document는 any IO/lock 전에 argument error. case/trailing-separator aliases normalize to one key and exact leaf names prevent traversal.
- **AC-M5D7D-007:** active save와 복수 waiter가 있는 same-directory concurrent saves never interleave or split locks; case/separator alias도 같다. Different-directory saves는 registry 순간 외에 병행 가능하고 final lease 뒤 entry가 제거된다.
- **AC-M5D7D-008:** source/getter byte mutations cannot alter written or result proof; default/reflection-bypassed result, unknown enum, outcome/stage/state/committed-revision sentinel과 success file-state 조합 전체가 reject된다.
- **AC-M5D7D-009:** static review confirms production uses `FileMode.Create`, exclusive access, `FileOptions.WriteThrough`, full write, `Flush(true)`, close, `File.Replace(..., true)` or same-dir `File.Move`; no forbidden fallback/authority.
- **AC-M5D7D-010:** focused/full EditMode and full PlayMode fail/skip/inconclusive 0, Luna P0/P1 0. PASS is atomic save adapter only, not launch recovery selection.

## allowlist

- `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs` + `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileAtomicSaveServiceV1Tests.cs` + `.meta`
- this contract
- M5D7D pregate/implementation/Luna reports under `docs/verification/`
- minimal `docs/README.md` entry

Existing M5D3-M5D7C code/tests, asmdefs, Packages, ProjectSettings, scenes, prefabs and assets are read-only. Luna pre-gate → Astra Approved → Terra → root tests → Luna post-review → Astra Verified.
