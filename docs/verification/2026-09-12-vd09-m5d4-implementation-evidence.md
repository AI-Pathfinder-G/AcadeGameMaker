# VD-09 M5D4 프로필 설정·튜토리얼 값 코어 — Terra 구현 증적

- 상태: **Implemented — Luna 독립 검토 및 Astra 통합 수용 대기**
- 기준 계약: [M5D4 profile settings/tutorial core](../specs/work-contracts/2026-09-12-vd09-m5d4-profile-settings-tutorial-core.md)
- 계약 사전검토: [Luna PASS, P2 implementation direction](./2026-09-12-vd09-m5d4-contract-pregate.md)
- 구현자: Terra
- 수용 권한: Astra. 이 문서는 구현자 자기 수용이나 persisted profile, settings 적용, tutorial event를 주장하지 않는다.

## 구현 경계

`ProfileSettingsSnapshot`은 approved window mode, three Q1000 volumes, two gamepad invert flags만 구조적으로 보존한다. `ProfileTutorialSnapshot`은 surrogate-safe NFC string의 ordinal strict sorted/unique confirmed IDs를 immutable defensive copy로 보존한다. 두 value type의 모든 public getter는 `Validate()`를 먼저 수행한다.

JSON, input binding, progression, full-profile composition, hash, IO/save/load/recovery, actual setting application, tutorial confirmation event, Unity/scene/asset authority는 구현하지 않았다.

## REQ / AC 구현 매핑

| 요구사항 / 수용 기준 | 구현 및 전용 테스트 근거 |
|---|---|
| REQ-M5D4-001 / AC-M5D4-001 | `ProfileWindowMode`, Q1000 range, immutable settings getter revalidation; `AC001_SettingsPreserveExactModesVolumesAndInvertFlagsAndRejectInvalidValues`이 exact modes/bounds/boolean representatives와 constructor/bypass errors를 검증한다. |
| REQ-M5D4-002 / AC-M5D4-002 | `HasNoUnpairedSurrogates`를 normalization 전에 호출하고 NFC + ordinal canonicality를 no-repair로 검증한다; `AC002_TutorialIdsRequireSurrogateSafeNfcOrdinalCanonicalValuesWithoutRepair`이 empty list/empty ID, valid pair/composed NFC, unpaired high/low, decomposed NFC failure와 collision을 포함한다. |
| REQ-M5D4-003 / AC-M5D4-003 | source copy 및 cloned read-only return; `AC003_TutorialCollectionsAreDefensivelyCopiedFromSourceAndOnEveryGetter`이 source, non-generic/generic mutation, repeated getter isolation을 검증한다. |
| REQ-M5D4-001..003 / AC-M5D4-004 | default/bypass validation; `AC004_DefaultAndConstructorBypassValuesAreRejectedByEveryPublicGetter`이 settings의 six getters와 tutorial getter 전부의 full validation을 검증한다. |
| REQ-M5D4-004..006 / AC-M5D4-005 | `AC005_RuntimeSourceStaysEngineFreeAndSurrogateScanPrecedesNormalization`이 REQ trace, no-engine/no-authority scope, M5D3 progression API non-reference, surrogate-scan ordering을 정적 확인한다. |
| AC-M5D4-006 | Luna independent review 및 root-owned focused/full EditMode/PlayMode execution이 필요하며 아직 Terra가 수용하지 않는다. |

## Changed files

- `Assets/AcadeGameMaker/Runtime/Profile/ProfileSettingsTutorialSnapshots.cs`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileSettingsTutorialSnapshots.cs.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileSettingsTutorialSnapshotsTests.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileSettingsTutorialSnapshotsTests.cs.meta`
- `docs/verification/2026-09-12-vd09-m5d4-implementation-evidence.md`
- `docs/README.md`

M5D3 source/tests/asmdefs, Packages, ProjectSettings, prefab/scene/assets/media는 변경하지 않았다.

## Static verification

- Scoped `git diff --check`: PASS.
- Source authority scan: runtime has no UnityEngine, IO, JSON, cryptography, clock, RNG, network, callback, or M5D3 progression type reference.
- Luna P2 focused coverage is present: surrogate scan appears before `Normalize`; valid surrogate pair/composed NFC, unpaired high/low, decomposed non-NFC, collision/no-repair, empty list versus single empty ID, and all public getter full validation are explicit test cases.

## R1 Unity preflight and focused attempt

- Unity `6000.6.0f1` licensing preflight passed: Editor and embedded/Hub Licensing Client `1.18.3` were present, Authenticode-valid, aligned, and the entitlement file was non-empty. Process inventory warnings remained.
- One focused EditMode attempt was launched for `AcadeGameMaker.Tests.EditMode.Profile.ProfileSettingsTutorialSnapshotsTests`. The active Unity log reached `Licensing is not yet initialized` and no focused XML was available at the observation point.
- Terra did not retry or terminate processes. Visible residual process state was Unity PID `63588` (`Responding=True`), UnityCrashHandler64 PID `61612`, UnityPackageManager PID `62356`, and the existing Hub Licensing Client PID `46696` (`Responding=True`). This state was reported to root; full regressions were not run.

## R2 focused-compile correction

- Root's focused compilation identified CS1026 in `ProfileSettingsTutorialSnapshotsTests.cs`: the AC005 `Assert.That` call lacked its final closing parenthesis.
- The single test assertion now has balanced parentheses. Runtime and all other implementation files are unchanged.
- The test source SHA-256 below was updated. Terra did not rerun Unity; root owns the next focused execution.

## Source SHA-256

| File | SHA-256 |
|---|---|
| `ProfileSettingsTutorialSnapshots.cs` | `7018BE226C0FAF41BCF8ABA39FA76AF07EA89E70211D8E81965CB8A31504B938` |
| `ProfileSettingsTutorialSnapshots.cs.meta` | `35AC987D49F9637F51D344CB4C1E380BB6DF7E70892784A073BAFFFA8BE7AFBE` |
| `ProfileSettingsTutorialSnapshotsTests.cs` | `940CD278B2B3105E24314164BEECE7F75B12D273710B903085C170603C1681AC` |
| `ProfileSettingsTutorialSnapshotsTests.cs.meta` | `CE84400F454556CF01FBF1786723E197CB7706A52E03CB19E06CF3B68ACEB78C` |

## R3 root Unity execution

- Licensing preflight: PASS on Unity `6000.6.0f1`; no competing Unity editor or licensing client was present.
- Focused EditMode after the R2 syntax correction: `5/5` passed; failed/skipped/inconclusive `0`; XML SHA-256 `B6A35A5E2C1234554ED6D1F5A7DC1FA9ABC5D6DAE61D12E6F15DC96A1F2639EF`.
- Full EditMode: `509/509` passed; failed/skipped/inconclusive `0`; XML SHA-256 `F6525DA7238F0F984F6EB42384B076DE0549546388B080957D9FF8817686D95B`.
- Full PlayMode: `576/576` passed; failed/skipped/inconclusive `0`; XML SHA-256 `CE102B6247959244BD5A992CF8ECB6FC3E89E670A2671F59B1A677ABF8D85B8A`.

## Execution handoff

Root obtained the R3 focused and full regression evidence above. Luna must independently review the source/test boundary and execution evidence before Astra decides integration acceptance.
