# VD-09 M5D7B canonical decoder — Terra implementation evidence

- Status: **Verified — Luna R2 PASS (P0=0, P1=0), Astra integration accepted**
- Contract: [M5D7B canonical decoder](../specs/work-contracts/2026-09-13-vd09-m5d7b-profile-v1-canonical-decoder.md)

## REQ / AC mapping

| Requirement / acceptance | Implementation and focused coverage |
|---|---|
| REQ-M5D7B-001 / AC-M5D7B-001..004 | Strict no-replacement UTF-8 and depth-64 JSON boundary classifies current, metadata recovery, binding recovery, unsupported outer schema, and typed invalid results. Focused cases cover parseable unique `0`/`2`/`int.MaxValue`/negative unsupported schemas, malformed-suffix `MalformedJson` precedence, null/BOM/invalid UTF-8/whitespace/comment/trailing/non-object/truncation, and exact depth-64 `SchemaViolation` versus depth-65 `MalformedJson`. |
| REQ-M5D7B-002 / AC-M5D7B-003..005 | Exact ordered object/property/type/value parser rejects duplicate, escaped duplicate schema, noninteger/overflow schema, invalid enum/nullability/arrays, and NFC/surrogate violations without defaults or coercion. The raw JSON string scanner distinguishes an actual unpaired `\uD800` escape (schema violation) from a literal escaped-backslash `\\uD800` text, which reaches inner binding recovery. |
| REQ-M5D7B-003 / AC-M5D7B-006 | Private raw-binding outer reconstruction checks payload hash and final bytes before M5D5. Tests cover canonical final return, integrity shape precedence, and reconstructed malformed/noncanonical inner binding. |
| REQ-M5D7B-004 / AC-M5D7B-002,006 | Metadata mismatch returns the exact document plus flags/reasons/value-matched non-input projection. Raw malformed/noncanonical inner binding returns `ValidBindingRecoveryRequired`, binding reason plus combined metadata bits when applicable, projection, and no document. |
| REQ-M5D7B-005 / AC-M5D7B-007 | Caller bytes are cloned; every classification validates neutral/unavailable fields, document/projection consistency, default/reflection-bypassed result/projection/reason/classification values, all unavailable getters, and defensive projection collections with `InvalidOperationException` or read-only collection rejection. |
| REQ-M5D7B-006 / AC-M5D7B-008 | New profile-only runtime file has no Unity, IO/path, persistence, recovery mutation, input-apply, clock, RNG, network, or callback authority. Existing dependencies remain unchanged. |
| AC-M5D7B-009 | Astra completed focused/full EditMode and a clean full PlayMode recheck; Luna independent review remains required. Decoder success is not a persistence, recovery-write, input-apply, or authenticity claim. |

## Scoped file SHA-256

| File | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalDecoderV1.cs` | `F361E860E09217B1DD32CEDD48980FE0E6F22F993800C20B6EF7AA13029EBE42` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalDecoderV1.cs.meta` | `1FD839D0480508A7F1F9DB9E81CD73AA5A58A49831E82B5CEBAC87543CF6AFDE` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalDecoderV1Tests.cs` | `A292D404C2E6F5BBE0534F1F0345350FEC64672A7BB269231531B333C3D540BB` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalDecoderV1Tests.cs.meta` | `6B72B98EA1C31096E9CE1BD473737711F8D414000052621AF2164DCA0120EB43` |

## Focused EditMode attempt

- One attempt used `qa/tools/Invoke-UnityQa.ps1` for `AcadeGameMaker.Tests.EditMode.Profile.ProfileCanonicalDecoderV1Tests` on Unity `6000.6.0f1`.
- Editor version, embedded/Hub licensing-client signature and alignment, and entitlement-file preflight passed. Process inventory was unavailable, so competing process checks were warnings.
- Unity stopped before test XML creation: `Connection to channel LicenseClient-me refused` followed by `Licensing is not yet initialized`. The same attempt recorded spawned Licensing Client PIDs `62920` and `67616`; no test-result XML was produced.
- Per the stop condition, Terra did not retry, terminate processes, or run full EditMode/PlayMode. This is an environment licensing block, not a test pass/fail result. The decoder performs no file selection, IO, mutation, atomic save/load, actual input apply, or authenticity assertion.

## Astra deterministic re-run and regression classification

After confirming PID `64072` was the exact stalled Unity batch process for this project, Astra terminated only that process and reran through the approved QA wrapper on Unity `6000.6.0f1`.

| Suite | Result | Failed / skipped / inconclusive | XML SHA-256 |
|---|---:|---:|---|
| Focused EditMode `ProfileCanonicalDecoderV1Tests` | `14/14` passed | `0 / 0 / 0` | `DB796DA99BDD49404CFCEEEAD8BDA76BB5D29972285BDDE0F4631FAE1C95E646` |
| Full EditMode | `543/543` passed | `0 / 0 / 0` | `0E3261914752BDCEE4AD41C7C6F211CF62E69CAF271FA8306207C80F4C30C273` |
| Full PlayMode first run | `575/576` passed | `1 / 0 / 0` | `F296A10CA0B26EF5D44BD3C3A049B2B3E5F665C52E7D9EB77762F867BDF40989` |
| Full PlayMode clean recheck | `576/576` passed | `0 / 0 / 0` | `880D22DBF162DC2B504E0AF3542640324D94D7718DDEB9E56B4476ED3F1A5E7C` |

The first PlayMode failure was `OrdanBossTerminalTransitionRequesterPlayModeTests.AcM5D1004_RouterFaultNeverStagesARequestOrForgedCompletion`, contaminated by the pre-existing nondeterministic `GameInputActions.Gameplay.Disable() has not been called` finalizer log. M5D7B is engine-free and has no Input/PlayMode code path. The unchanged clean recheck passed all 576 tests, so this is retained as a known unrelated P2 rather than hidden or attributed to the decoder.

Result files are under `artifacts/m5d7b-20260913/`. These runs exercise `AC-M5D7B-001..009` at the codec/regression boundary only.

## Luna P1 invariant repair and final rerun

Luna's first post-review found that reflection-bypassed, classification-unavailable neutral fields were not all revalidated. Terra tightened every classification's neutral sentinel checks, bound current/metadata compatibility to the document snapshot, and value-matched metadata recovery projections to their document non-input state. The focused matrix now also covers combined binding+metadata reasons, all unavailable getters, projection collection defense, unsupported version `1`, and mismatched valid projections.

| Final suite | Result | Failed / skipped / inconclusive | XML SHA-256 |
|---|---:|---:|---|
| Focused EditMode R2 | `16/16` passed | `0 / 0 / 0` | `C7BE1BE3B2B3D5EA1D071E1BF5626D888ED590BC871A25B9FCA045B2970EF4A5` |
| Full EditMode R2 | `545/545` passed | `0 / 0 / 0` | `B86B06D8CDD4AB50298967E942932DC589256B4FA7970F9DA29004228B01A5D8` |
| Full PlayMode R2 | `576/576` passed | `0 / 0 / 0` | `C01E06468A0FF2CA977C03A6F1CF9BCFAFF6F56068045BD815DE92A855C864E8` |

These R2 artifacts supersede the earlier pass set for final acceptance; the earlier first-run PlayMode P2 record is retained for audit history.
