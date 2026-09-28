# VD-09 M5D3 프로필 진행 상태 투영 코어 — Terra 구현 증적

- 상태: **Implemented — Luna 독립 검토 및 Astra 통합 수용 대기**
- 기준 계약: [M5D3 profile progression projection core](../specs/work-contracts/2026-09-12-vd09-m5d3-profile-progression-projection-core.md)
- 계약 사전검토: [Luna second-pass PASS](./2026-09-12-vd09-m5d3-contract-pregate.md)
- 구현자: Terra
- 수용 권한: Astra. 이 문서는 구현자 자기 수용이나 profile file·codec·M5A adapter 연결을 주장하지 않는다.

## 구현 경계

`AcadeGameMaker.Profile`은 engine-free progression value assembly다. `ProfileProgressionSnapshot`은 nonnegative source revision, approved seed, consent, closed nullable choice/skill pair, canonical completed branches만 보유한다. `ProfileChoiceSkillPair`와 snapshot은 public `Validate()` 및 소비 경계 재검증으로 default/constructor-bypass 값을 거부하고 branch collection은 입력·반환 양쪽에서 방어 복사한다.

JSON/canonicalization/hash, disk/load/save/recovery, schema authentication, revision 증가, source provenance, choice/skill effect와 transition, M5A/Run/Unity adapter는 구현하지 않았다.

## REQ / AC 구현 매핑

| 요구사항 / 수용 기준 | 구현 및 전용 테스트 근거 |
|---|---|
| REQ-M5D3-001 / AC-M5D3-001 | public immutable snapshot 및 pair의 `Validate()` revalidation과 revision/seed validation; `AC001_RevisionAndApprovedSeedValuesAreExactAtConstructionAndRevalidation`이 `0`, `long.MaxValue`, null/4 approved seeds, negative and bypassed revision/seed를 검증한다. |
| REQ-M5D3-003 / AC-M5D3-002 | `IsValidChoiceSkill`, `HasCommittedPair`, `TryGetCommittedPair`; `AC002_NoChoiceAndBothExactPairsProjectWithoutPartialOrInvalidFallback`이 no-choice, exact two pairs, one-sided/crossed/zero/unknown 및 bypass rejection을 검증한다. |
| REQ-M5D3-004 / AC-M5D3-003 | closed consent domain만 보존하고 pair-consent inference를 만들지 않는다; `AC003_AllConsentValuesArePreservedWithoutPairConsentInferenceOrTransitions`이 four consent values와 both pairs의 독립성을 검증한다. |
| REQ-M5D3-002 / AC-M5D3-004 | `CopyAndValidateBranchesArgument`, ordinal sorted-unique branch revalidation 및 read-only defensive return; `AC004_CompletedBranchesAreCanonicalUniqueAndDefensivelyCopied`이 exact order, null/reverse/duplicate/unknown, source/return mutation을 검증한다. |
| REQ-M5D3-001 / AC-M5D3-005 | default snapshot/pair와 bypass values의 public consumption revalidation; `AC005_DefaultAndBypassedValuesThrowRatherThanMasqueradingAsNoChoice`이 false/no-choice masking 부재와 post-failure valid construction을 검증한다. |
| REQ-M5D3-005..006 / AC-M5D3-006 | new Profile asmdef의 `noEngineReferences: true` 및 runtime authority token 부재; `AC006_ProfileRuntimeStaysEngineFreeAndOmitsPersistenceAuthority`가 source/asmdef 경계를 확인한다. |

## Changed files

- `Assets/AcadeGameMaker/Runtime/Profile/AcadeGameMaker.Profile.asmdef`
- `Assets/AcadeGameMaker/Runtime/Profile/AcadeGameMaker.Profile.asmdef.meta`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileProgressionSnapshot.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileProgressionSnapshot.cs.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/AcadeGameMaker.Profile.EditMode.Tests.asmdef`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/AcadeGameMaker.Profile.EditMode.Tests.asmdef.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileProgressionSnapshotTests.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileProgressionSnapshotTests.cs.meta`
- `docs/specs/work-contracts/2026-09-12-vd09-m5d3-profile-progression-projection-core.md`
- `docs/verification/2026-09-12-vd09-m5d3-implementation-evidence.md`
- `docs/README.md`

No existing runtime/test/asmdef, Packages, ProjectSettings, prefab, scene, asset, or media file was changed for M5D3.

## Static verification

- Scoped `git diff --check`: PASS.
- Source review: Profile asmdef is `noEngineReferences: true`; runtime source has no `UnityEngine`, IO, cryptography, JSON, clock, RNG, network, or callback authority token.
- Source review: both value types expose exact public `void Validate()` and the snapshot projection members call it before producing values.

## R1 Unity preflight and focused attempt

- Unity `6000.6.0f1` licensing preflight passed: Editor and embedded/Hub Licensing Client `1.18.3` were present, Authenticode-valid, aligned, and the entitlement file was non-empty. Process inventory was unavailable, so competing process/client warnings were retained.
- One focused EditMode attempt was made for `AcadeGameMaker.Tests.EditMode.Profile.ProfileProgressionSnapshotTests` after that preflight. It produced no test-result XML and its Unity log stopped at `Licensing is not yet initialized`.
- Per the licensing runbook, this is an environment block rather than a test result. Terra did not retry, did not run full EditMode/PlayMode, and makes no focused compile/pass claim from this attempt.

## R2 focused-result test correction

- Root's focused EditMode result `artifacts/m5d3-20260912/focused-editmode-r2.xml` was `4/11` passed with `7` test-oracle failures, not a runtime failure.
- `AC001` incorrectly included seed `0` in its legal matrix although the contract permits only null/`101`/`202`/`303`/`404`; that case was removed.
- Unity's NUnit 1.6 `Assert.Throws<T>` matches the exact exception type. Constructor assertions for revision, seed, and consent now expect the runtime's intentional `ArgumentOutOfRangeException`; closed pair assertions remain `ArgumentException`.
- Runtime and asmdef files were not changed. This R2 correction has static checks only; root owns the next Unity execution.

## Source SHA-256

| File | SHA-256 |
|---|---|
| `AcadeGameMaker.Profile.asmdef` | `D4C8C4E694046072EDE986EE14C2AEB1AAC8A57A761487245BC1764EBA10B99B` |
| `AcadeGameMaker.Profile.asmdef.meta` | `F49083744D8931D6FE88051BDD673BCD6E3B42ABD0FE85CCE018329AFEF40EA1` |
| `ProfileProgressionSnapshot.cs` | `B3B4BA3912F4D3B25C894C362940EF1518594DAAC6F36E6FC89CD68F5BDB440C` |
| `ProfileProgressionSnapshot.cs.meta` | `5F2D060F014F93D8A4FE42AA7D1C148DC8CEEC5A24B7777276119C2144FB2854` |
| `AcadeGameMaker.Profile.EditMode.Tests.asmdef` | `DC1E6DD360DBE6785D6611677C47F8624A92A874DEFBEB521F86CA52E097D97F` |
| `AcadeGameMaker.Profile.EditMode.Tests.asmdef.meta` | `E9EA0DBDB986B6E50B167907CD9A08D63BC4EA90337BCEFBE77FE7C5BC0EC09F` |
| `ProfileProgressionSnapshotTests.cs` | `E8E296D216EB05370731B52188276BAE2AE8EF3784134BFDBEE276F753AF430D` |
| `ProfileProgressionSnapshotTests.cs.meta` | `BC80C06F61A687AECEEC83AFF143F8EB60F1E02D20C8169CF3241D8D2208E138` |

## R3 root Unity execution

- Licensing preflight: PASS on Unity `6000.6.0f1`; no competing editor or licensing client was present.
- Focused EditMode after the R2 oracle correction: `10/10` passed; failed/skipped/inconclusive `0`; XML SHA-256 `BBDA2704EFE6BD922799A791CF8AA1E755310BD856788B8ECDCED6DD8172AAA3`.
- Full EditMode: `504/504` passed; failed/skipped/inconclusive `0`; XML SHA-256 `77F253FBA92833090F3EF599F562EBA690EAAEA412DCEBC29E7B1F63D1619B3C`.
- First full PlayMode: `575/576` passed. The sole failure was an asynchronous existing M5D1 `GameInputActions.Gameplay.Disable()` finalizer warning delivered during `AcM5D1004`; no M5D3 Profile code was on its stack. XML SHA-256 `78A64401486F48738515698C25DC12433077FF90CAC27AF9DCC95B6021AB7D73`.
- The affected existing M5D1 test run alone passed `1/1`, failed/skipped/inconclusive `0`; XML SHA-256 `EC206A4C118F5A63477A3905053BCEBE648A3A078F5DC57E31EB860C7715D99B`.
- One classified full PlayMode recheck passed `576/576`; failed/skipped/inconclusive `0`; XML SHA-256 `BA2185D4E891B9BE4FE8F6EFBB8BA567A9D2E2F0A198FEA9898CE954C5ED2D56`.
- The transient finalizer warning remains evidence of an existing teardown-order risk, not a suppressed result. No M5D1 source/test was changed under the M5D3 allowlist.

## Execution handoff

Root obtained the R3 focused and full regression evidence above. Luna must independently review default/bypass revalidation, nullable enum counterexamples, defensive branch copy, no pair-consent inference, engine-free boundary, the classified first PlayMode failure and the clean recheck before Astra decides integration acceptance.
