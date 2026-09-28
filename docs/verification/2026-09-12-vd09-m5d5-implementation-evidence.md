# VD-09 M5D5 binding override JCS 코어 — Terra 구현 증적

- 상태: **Implemented — Luna 독립 검토 및 Astra 통합 수용 대기**
- 기준 계약: [M5D5 binding override JCS core](../specs/work-contracts/2026-09-12-vd09-m5d5-binding-overrides-jcs-core.md)
- 구현자: Terra; 수용 권한: Astra

## REQ / AC 매핑

| 요구사항 / 수용 기준 | 구현 및 focused test |
|---|---|
| REQ-M5D5-001 / AC-M5D5-001 | explicit empty sentinel, strict non-empty parse, default revalidation; `AC001_EmptySentinelAndDefaultBoundariesAreExact`. |
| REQ-M5D5-002 / AC-M5D5-002..003 | strict `JsonDocumentOptions`, root max depth `64`, raw surrogate precheck, decoded ordinal duplicate scan at every object, NFC validation; `AC002`, `AC002_MaxDepth64IsAcceptedAndDepth65IsRejected`, and `AC003`. |
| REQ-M5D5-003 / AC-M5D5-002..005 | custom UTF-16 ordinal object sort, string escaping, binary64 shortest/threshold formatting, idempotence/hash; `AC002` through `AC005`. |
| REQ-M5D5-004 / AC-M5D5-006 | nonempty canonical text reparse/recanonicalize on every public boundary; `AC005`. |
| REQ-M5D5-005..006 / AC-M5D5-007 | static engine-free/dependency authority scan; `AC006`. |
| AC-M5D5-008 | Luna independent review and root focused/full execution remain required. |

## Luna P2 implementation gates

- `JsonDocumentOptions` disallows comments/trailing commas; `JsonDocument.Parse` provides one strict root.
- Decoded duplicate names use recursive `HashSet<string>(StringComparer.Ordinal)`.
- Custom `WriteString` and UTF-16 ordinal property sorting avoid default writer escaping/order behavior.
- `ShortestRoundTrip` obtains invariant binary64 candidates, then explicitly applies RFC decimal/scientific thresholds and spelling.
- Public nonempty representation is deterministically reparsed/recanonicalized; it is not trusted via a marker.

## R1 Luna P1 depth-bound correction

- Luna identified that the root `JsonDocumentOptions.MaxDepth=64` contract boundary needed focused proof. Astra resolved the contract: root depth `64` is legal and `65+` is invalid.
- `AC002_MaxDepth64IsAcceptedAndDepth65IsRejected` now parses/canonicalizes a 64-level nested array and requires `ArgumentException` for 65 levels. Runtime `MaxDepth=64` is unchanged.
- This correction changes only the focused test and its source hash. Root owns Unity execution.

## Static / execution handoff

- Scoped `git diff --check` and runtime authority scan passed. Static source confirms strict options, ordinal decoded duplicate set, custom string serializer, shortest-round-trip formatter, and deterministic revalidation.
- One focused Unity `6000.6.0f1` EditMode attempt was launched after a PASS licensing preflight. It stopped at `Licensing is not yet initialized`; no result XML was available. This is an environment block, not a compile/golden result.
- Terra did not retry or terminate processes. Visible residuals were Unity PID `65216` (`Responding=True`), UnityCrashHandler64 PID `63552`, UnityPackageManager PID `61564`, and existing Hub Licensing Client PID `46696` (`Responding=True`); root was notified. Full regression remains root-owned.
- Runtime performs no binding application, profile outer JSON/hash, IO, persistence, input asset compatibility, or source authentication.

## Source SHA-256

| File | SHA-256 |
|---|---|
| `ProfileBindingOverridesJson.cs` | `91E5EBD41A99ECDCC6B4F1EAE2122E4F23B8F0FD8AF084ADE00A85C5E5D8FD14` |
| `ProfileBindingOverridesJson.cs.meta` | `318BC91572265C141E1A33374DAC609699205438F30E0D12253F377E51F2FE8F` |
| `ProfileBindingOverridesJsonTests.cs` | `5FB2E8EEA4ED8264762794B9597AA3D9D67050DE19A7B9DB4EAF0AB8B4003D6E` |
| `ProfileBindingOverridesJsonTests.cs.meta` | `7E6FFD1F8EF0E5FCC98E088BC51D6C3E37BBC1A0BE454C0127A4E9CD3D964F76` |

## R2 root Unity execution

- Licensing preflight: PASS on Unity `6000.6.0f1`; no competing Unity editor or licensing client was present.
- Initial focused EditMode before depth test addition: `6/6` passed; XML SHA-256 `85C58D14D1A609BD8EF22C97A84495AAC248ADB1D9D59EEF2BA7D123DE7CDCD1`.
- Final focused EditMode including depth 64/65: `7/7` passed; failed/skipped/inconclusive `0`; XML SHA-256 `A71611D359CCE7C18D08578D82759722A00AACAC5AFAA3742E4B6E4F5F8D4BDF`.
- Final full EditMode: `516/516` passed; failed/skipped/inconclusive `0`; XML SHA-256 `44532BFF1ED8828EDDDF3227DB5A01F68EC3951E34C01749687FDA34E47857C4`.
- First full PlayMode: `575/576`; the sole failure was the previously classified existing M5D1 `GameInputActions.Gameplay.Disable()` finalizer warning delivered during `AcM5D1004_RouterFaultNeverStagesARequestOrForgedCompletion`. Profile/JCS code was absent from the stack. XML SHA-256 `DBA8824F113D87ABF9F03A4D4E122A6B0D63D595A08CEF198E5034AAC8BE46EA`.
- One classified full PlayMode recheck: `576/576` passed; failed/skipped/inconclusive `0`; XML SHA-256 `4E12B13B634862F555246B1B2BC40D6B2038C403D671B5774FBE6B71E76F32B4`.
- The first PlayMode failure remains visible evidence of the existing M5D1 teardown-order P2; no M5D1 source/test was modified under M5D5.
