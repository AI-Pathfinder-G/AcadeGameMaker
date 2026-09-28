# VD-09 M5D7A profile v1 canonical encoder — Terra implementation evidence

- Status: **Verified — Luna PASS (P0=0, P1=0), Astra integration accepted**
- Contract: [M5D7A canonical encoder](../specs/work-contracts/2026-09-13-vd09-m5d7a-profile-v1-canonical-encoder.md)

## REQ / AC mapping

| Requirement / acceptance criterion | Implemented scope and focused coverage |
|---|---|
| REQ-M5D7A-001 / AC-M5D7A-001 | `ProfileSnapshotV1` composes the four verified immutable values, projects the progression revision without a redundant field, and structurally preserves metadata-mismatched input candidates. Golden cases cover revision `0` and `long.MaxValue`, null and committed progression forms, and default/nested rejection. |
| REQ-M5D7A-002 / AC-M5D7A-002..003 | Fixed-order payload writer emits exact field order, enum spellings, lowercase null/boolean literals, invariant integers, UTF-8 strings, arrays, and outer escaping for M5D5 canonical override text. Golden tests cover wrong asset/schema `0`, `2`, and `int.MaxValue` candidates: each retains exact mismatched fields, has expected source mismatch flags, encodes without fallback, and excludes derived compatibility metadata. Additional goldens cover committed branches, quote/backslash/control/solidus/BMP/supplementary tutorial text, and Turkish-culture independence. |
| REQ-M5D7A-003 / AC-M5D7A-004 | SHA-256 is calculated from the exact payload bytes as lowercase 64-hex text. The final bytes insert exactly one trailing `integrity.payloadSha256` object. Focused coverage independently calculates the hash and checks field presence/order, UTF-8 no-BOM, and no final newline. |
| REQ-M5D7A-004 / AC-M5D7A-005..006 | Snapshot and document public boundaries fully revalidate nested/default/reflection-bypassed representations. Document arrays are cloned at construction and each getter; tests mutate source/returned/reflection-injected arrays and verify idempotence or rejection. |
| REQ-M5D7A-005..006 / AC-M5D7A-007 | New engine-free file has no parser, file IO, recovery, input application, clock, runtime/active-run, path, or persistence authority. It uses BCL UTF-8 and SHA-256 only; no existing runtime/test/asmdef/package/project-setting/asset was modified. |
| AC-M5D7A-008 | Astra reran the focused and full Unity suites successfully; Luna independent review remains required. This implementation does not claim persistence/file-validity, atomic save/load, or recovery acceptance. |

## Scoped files and SHA-256

| File | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalEncoder.cs` | `5326FDB385A058A4590B9BD16B07D78A0F0033FF485D4971C8C6FC56A3CABDF5` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalEncoder.cs.meta` | `8E926437FD6AF6473D39A4F76578F53D5BA98219B75256946A5BA1331B6EF306` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalEncoderTests.cs` | `0FBC8FABF8B14DC4D607DAE4D49AE3088E5CC0D6BD6D0B374857DC05326153F0` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalEncoderTests.cs.meta` | `647B0211D3DD9F8BFE46662E2D2CE8482406BD53BFA0B6F64B80341D7B4DEA1C` |

## Focused EditMode attempt

- One attempt used `qa/tools/Invoke-UnityQa.ps1` for `AcadeGameMaker.Tests.EditMode.Profile.ProfileCanonicalEncoderTests` on Unity `6000.6.0f1`.
- Preflight passed editor version, embedded/Hub client signature and version alignment, and entitlement-file checks. Process inventory was unavailable, so competing-editor/client checks were warnings.
- Unity stopped before test XML creation with `Licensing is not yet initialized` after `Connection to channel LicenseClient-me refused`. The log recorded spawned Licensing Client PID `59808`; no Unity test result XML was produced.
- Per contract stop condition, Terra did not retry, terminate processes, or run full EditMode/PlayMode. This is an environment licensing block, not a passing or failing test result.

## Astra deterministic re-run

After confirming that PID `16036` was the exact stalled Unity batch process for this project, Astra terminated only that process and reran through the approved QA wrapper on Unity `6000.6.0f1`.

| Suite | Result | Failed / skipped / inconclusive | XML SHA-256 |
|---|---:|---:|---|
| Focused EditMode `ProfileCanonicalEncoderTests` | `8/8` passed | `0 / 0 / 0` | `113FC87F57D9D716C3720D8AEA87EB4C8F36FF199B76ADBCDAE07DB76FAD350E` |
| Full EditMode | `529/529` passed | `0 / 0 / 0` | `34B9FAF89D5959F67B8A41A005E4436325CF7C3F69B85D7092C97E2652DDFC98` |
| Full PlayMode | `576/576` passed | `0 / 0 / 0` | `1F04B216008D7AEF58E12B9485607128FA0C2648DAF219CAA4B304E4C7E91A27` |

Result files are under `artifacts/m5d7a-20260913/`. These results exercise `AC-M5D7A-001..008`; they establish encoder behavior and regression safety only.

## Handoff boundary

The encoder creates a recovery-pre-canonical candidate only. It neither parses nor authenticates a source file, performs IO/atomic commit/load/recovery, applies input bindings, increases revisions, or claims persisted-profile validity. Luna owns independent post-review; Astra owns integration acceptance.
