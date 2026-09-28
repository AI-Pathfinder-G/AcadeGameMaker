# VD-09 M5D7A canonical encoder — Luna independent review

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7A profile v1 canonical encoder](../specs/work-contracts/2026-09-13-vd09-m5d7a-profile-v1-canonical-encoder.md)
- Evidence: [Terra implementation evidence](2026-09-13-vd09-m5d7a-implementation-evidence.md)
- Verdict: **PASS — P0=0, P1=0; recommend Astra accept as Verified**
- Scope: independent source, test, artifact, dependency, and allowlist review. No runtime/test implementation was modified and no Unity run was initiated by Luna.

## Evidence integrity

The three supplied XML result files were parsed independently. All test cases had `result="Passed"`; no failed, skipped, or inconclusive cases were present.

| Suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused EditMode | 8 | 8 | 0 | 0 | 0 | `113FC87F57D9D716C3720D8AEA87EB4C8F36FF199B76ADBCDAE07DB76FAD350E` |
| Full EditMode | 529 | 529 | 0 | 0 | 0 | `34B9FAF89D5959F67B8A41A005E4436325CF7C3F69B85D7092C97E2652DDFC98` |
| Full PlayMode | 576 | 576 | 0 | 0 | 0 | `1F04B216008D7AEF58E12B9485607128FA0C2648DAF219CAA4B304E4C7E91A27` |

The implementation evidence's four scoped file hashes also match the workspace:

| File | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalEncoder.cs` | `5326FDB385A058A4590B9BD16B07D78A0F0033FF485D4971C8C6FC56A3CABDF5` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalEncoder.cs.meta` | `8E926437FD6AF6473D39A4F76578F53D5BA98219B75256946A5BA1331B6EF306` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalEncoderTests.cs` | `0FBC8FABF8B14DC4D607DAE4D49AE3088E5CC0D6BD6D0B374857DC05326153F0` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalEncoderTests.cs.meta` | `647B0211D3DD9F8BFE46662E2D2CE8482406BD53BFA0B6F64B80341D7B4DEA1C` |

## Independent AC review

| AC | Result and source/test basis |
|---|---|
| AC-M5D7A-001 | **PASS.** `ProfileSnapshotV1` is a readonly composition of the four verified value cores. `ProfileRevision` projects `_progression.ProfileRevision`; no redundant revision or increment exists. Constructor and public validation reject default/nested-invalid values. Golden coverage includes `0`, `long.MaxValue`, null and committed progression forms. |
| AC-M5D7A-002 | **PASS.** The writer emits the exact VD-09 top-level and nested order, exact enum spellings, lowercase null/boolean literals, invariant integers, and existing validated array order. Mismatch candidates preserve wrong ID and schema `0`, `2`, and `int.MaxValue` without fallback. |
| AC-M5D7A-003 | **PASS.** `AppendQuoted` applies the approved quote/backslash/control rules while preserving solidus, BMP, supplementary, and NFC text. M5D5 `CanonicalText` is encoded as one outer JSON string; no parsing or normalization is added by M5D7A. |
| AC-M5D7A-004 | **PASS.** `BuildValidated` hashes the exact UTF-8 payload before integrity insertion. SHA output is lowercase 64-hex; final bytes append exactly one `integrity.payloadSha256` object. Tests independently calculate the hash and verify no BOM, whitespace, or final newline. |
| AC-M5D7A-005 | **PASS.** Nested constructors/getters and document revalidation preserve deterministic values. Repeated encoding and validation are idempotent, including source collection copies and canonical inner override text. |
| AC-M5D7A-006 | **PASS.** Document constructor/getters clone byte arrays. `ProfileCanonicalDocumentV1.Validate()` regenerates payload/hash/file and compares byte-exactly; default document, reflection-bypassed snapshot/hash/bytes, and mutated returned arrays are rejected or isolated. |
| AC-M5D7A-007 | **PASS.** Static review found no Unity, IO, clock, RNG, network, callback, active-run/runtime reference, parser, Input System application, persistence, or compatibility-field authority in the new runtime file. Only BCL UTF-8/SHA-256 are used. M5D7A references are limited to the two new source/test files, their metas, contract/evidence/pre-gate documents, artifacts, and the README index entry; no M5D3–M5D6 API, asmdef, package, project setting, or asset change is attributable to M5D7A. |
| AC-M5D7A-008 | **PASS.** Focused EditMode, full EditMode, and full PlayMode all have zero failed/skipped/inconclusive results. Evidence correctly limits the claim to encoder behavior/regression safety and does not claim decoder, persisted-file validity, atomic save/load, or recovery acceptance. |

## Findings

### P0/P1

None. The earlier input-mismatch ambiguity is resolved by the Approved contract: mismatch values are recovery-before canonical candidates, derived compatibility is not encoded, and normal commit remains a later repaired-snapshot responsibility.

### P2-001 — stale README status text (non-blocking)

`docs/README.md` currently says the focused M5D7A run stopped at licensing initialization and that review/integration are pending. The actual evidence and XMLs show the subsequent successful focused and full runs above. This is documentation drift only and does not invalidate AC-M5D7A-001..008, but the index sentence should be refreshed by the integration owner.

## Recommendation

Recommend Astra mark the M5D7A contract **Verified** after integrating this report. Preserve the encoder-only boundary: future work must own decoding, input-only recovery, revision advancement, and atomic persistence separately. Do not treat mismatch candidates emitted here as current-compatible normal saves.
