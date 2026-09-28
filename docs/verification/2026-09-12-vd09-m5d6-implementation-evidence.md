# VD-09 M5D6 프로필 입력 블록·호환성 코어 — Terra 구현 증적

- 상태: **Implemented — Luna 독립 검토 및 Astra 통합 수용 대기**
- 계약: [M5D6 input compatibility core](../specs/work-contracts/2026-09-12-vd09-m5d6-profile-input-compatibility-core.md)

## REQ / AC 매핑

| 요구사항 / AC | 구현 및 focused test |
|---|---|
| REQ-M5D6-001 / AC001 | immutable ID/version/nested M5D5 validated override; `AC001_CurrentMetadataAndOverridesAreExactAndStructuralInvalidsReject`. |
| REQ-M5D6-002..003 / AC002..003 | exact asset/schema constants and independent ordinal mismatch flags; `AC002`, `AC003` cover current/asset/schema/both and `0`/`2`/`int.MaxValue`. |
| REQ-M5D6-004 / AC004 | default/reflection nested override and every public getter full validation. |
| REQ-M5D6-005..006 / AC005 | meta GUID read-only comparison and outer-schema/binding-schema non-confusion static boundary. |
| AC006 | Luna independent review plus root focused/full execution remains required. |

## Luna P2 coverage

Exact meta GUID, ordinal culture independence, binding schema `0`/`2`/`int.MaxValue`, default/malformed nested override, all getters, and no outer schema authority are focused test cases. Runtime never reads the meta or applies overrides.

## Source SHA-256

| File | SHA-256 |
|---|---|
| `ProfileInputSnapshot.cs` | `A58B024F5E288739342603AAC77F1A32C6DFAF66CB40853F364F0AD196522644` |
| `ProfileInputSnapshotTests.cs` | `F57A0CB4191CAC84C6429F8FA475A529378E7996C9B96EC2AE8B29A089975A77` |

## R1 root pre-run test correction

- AC003 now holds `ProfileInputContract.CurrentInputActionsAssetId` fixed and asserts schema `0`, `2`, and `int.MaxValue` each produces exactly `BindingSchemaMismatch` (`2`), isolating the schema axis.
- AC004 now reflection-injects null, empty, and decomposed non-NFC asset IDs plus schema `-1`; `Validate`, a representative field getter, and `Compatibility` each reject them. Existing nested override and all-getter cases remain.
- Runtime was not changed and Unity rerun remains root-owned.

## Execution handoff

Scoped static boundary checks passed. One focused Unity `6000.6.0f1` EditMode attempt followed a PASS preflight but stopped at `Licensing is not yet initialized`; no XML was available. Terra did not retry or terminate Unity PID `67992` (`Responding=True`), UnityCrashHandler64 PID `55988`, UnityPackageManager PID `65188`, or Hub Licensing Client PID `46696` (`Responding=True`); root was notified. Root owns full regression. Luna/Astra acceptance remains pending; this core does not apply bindings or validate persisted profile recovery.

## R2 root Unity execution

- Licensing preflight: PASS on Unity `6000.6.0f1`; no competing editor or licensing client was present.
- Focused EditMode: `5/5` passed; failed/skipped/inconclusive `0`; XML SHA-256 `D830214BC0D88088F9579711A7D0D9F5521E2C02A7C9AFF21E71DAC03CCCECE3`.
- Full EditMode: `521/521` passed; failed/skipped/inconclusive `0`; XML SHA-256 `725CCF8C3885BDF1550337053F17BF438B1BD29D6CF6D84DD6B268E6355B89B4`.
- Full PlayMode: `576/576` passed; failed/skipped/inconclusive `0`; XML SHA-256 `70487F80E9AC4760C19FD7C75A9521E8498B97A4AFEFBAADED25673A66AA1103`.
- Final test source SHA-256 after the root pre-run matrix correction is `F57A0CB4191CAC84C6429F8FA475A529378E7996C9B96EC2AE8B29A089975A77`.
