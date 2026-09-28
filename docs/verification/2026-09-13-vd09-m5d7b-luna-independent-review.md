# VD-09 M5D7B canonical decoder — Luna independent implementation review

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7B profile v1 canonical decoder](../specs/work-contracts/2026-09-13-vd09-m5d7b-profile-v1-canonical-decoder.md)
- Evidence: [Terra implementation evidence](2026-09-13-vd09-m5d7b-implementation-evidence.md)
- Verdict: **R2 PASS — P0=0, P1=0; recommend Astra mark Verified**
- Scope: independent source, test, evidence, dependency, and allowlist review. No runtime/test/contract implementation was modified and no Unity run was initiated by Luna.

## R2 recheck

Terra's bounded correction closes P1-001. Result validation now checks every classification-inapplicable neutral field, rejects unsupported version `1`, binds metadata compatibility to the returned document snapshot, and byte/value-matches metadata recovery projections to their document's non-input state. The added focused matrix covers neutral-field reflection bypasses, mismatched projections, all unavailable getter states, combined binding+metadata reasons, and projection collection defense.

The final R2 artifacts were independently parsed and hashed:

| Suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused EditMode R2 | 16 | 16 | 0 | 0 | 0 | `C7BE1BE3B2B3D5EA1D071E1BF5626D888ED590BC871A25B9FCA045B2970EF4A5` |
| Full EditMode R2 | 545 | 545 | 0 | 0 | 0 | `B86B06D8CDD4AB50298967E942932DC589256B4FA7970F9DA29004228B01A5D8` |
| Full PlayMode R2 | 576 | 576 | 0 | 0 | 0 | `C01E06468A0FF2CA977C03A6F1CF9BCFAFF6F56068045BD815DE92A855C864E8` |

The current R2 source/test hashes match the updated implementation evidence:

| File | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalDecoderV1.cs` | `F361E860E09217B1DD32CEDD48980FE0E6F22F993800C20B6EF7AA13029EBE42` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalDecoderV1.cs.meta` | `1FD839D0480508A7F1F9DB9E81CD73AA5A58A49831E82B5CEBAC87543CF6AFDE` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalDecoderV1Tests.cs` | `A292D404C2E6F5BBE0534F1F0345350FEC64672A7BB269231531B333C3D540BB` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalDecoderV1Tests.cs.meta` | `6B72B98EA1C31096E9CE1BD473737711F8D414000052621AF2164DCA0120EB43` |

## Evidence integrity

I parsed all four supplied XML files and independently recomputed their SHA-256 values. The first full PlayMode run has exactly the documented unrelated finalizer-log failure; the clean recheck is green.

| Suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused EditMode | 14 | 14 | 0 | 0 | 0 | `DB796DA99BDD49404CFCEEEAD8BDA76BB5D29972285BDDE0F4631FAE1C95E646` |
| Full EditMode | 543 | 543 | 0 | 0 | 0 | `0E3261914752BDCEE4AD41C7C6F211CF62E69CAF271FA8306207C80F4C30C273` |
| Full PlayMode first run | 576 | 575 | 1 | 0 | 0 | `F296A10CA0B26EF5D44BD3C3A049B2B3E5F665C52E7D9EB77762F867BDF40989` |
| Full PlayMode clean recheck | 576 | 576 | 0 | 0 | 0 | `880D22DBF162DC2B504E0AF3542640324D94D7718DDEB9E56B4476ED3F1A5E7C` |

The failed first-run case is `OrdanBossTerminalTransitionRequesterPlayModeTests.AcM5D1004_RouterFaultNeverStagesARequestOrForgedCompletion`; its failure is solely the pre-existing `GameInputActions.Gameplay.Disable()` finalizer warning. The unchanged clean recheck passed 576/576, so this is a known unrelated P2 and not attributed to M5D7B.

The four scoped source/meta hashes also match the evidence:

| File | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalDecoderV1.cs` | `EAE26CA21FF1497CF5602A50CBAD16CEEA38AFFFAE6D3CBCE21DF944E349BBB5` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileCanonicalDecoderV1.cs.meta` | `1FD839D0480508A7F1F9DB9E81CD73AA5A58A49831E82B5CEBAC87543CF6AFDE` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalDecoderV1Tests.cs` | `F77E337D199A65257E306580930CA60290A7BF93EA8E3D6933377C7BDE0F81F9` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileCanonicalDecoderV1Tests.cs.meta` | `6B72B98EA1C31096E9CE1BD473737711F8D414000052621AF2164DCA0120EB43` |

## Independent AC review

| AC | Result and basis |
|---|---|
| AC-M5D7B-001 | **PASS.** Current/default and committed Extraction/Solidarity M5D7A documents reconstruct through the exact payload/hash/final-byte path. Current input returns `ValidCurrentInput`. |
| AC-M5D7B-002 | **PASS.** Metadata mismatch ID/schema combinations preserve the exact document, M5D6 flags, projection, and reason bits. R2 also independently asserts binding-plus-metadata reason composition through `ReasonsForBinding`. |
| AC-M5D7B-003 | **PASS.** Full JSON is parsed before schema classification; malformed suffixes remain `MalformedJson`, escaped duplicate `schemaVersion` is `SchemaViolation`, and unique representable negative versions are returned as exact unsupported versions without v1 semantic interpretation. |
| AC-M5D7B-004 | **PASS.** Null is the only argument exception. Empty/BOM/strict UTF-8/whitespace/comment/trailing/non-object/truncated cases are typed invalid results. |
| AC-M5D7B-005 | **PASS.** Exact ordered field sets, decoded duplicates, integer token grammar, enum/nullability, array constraints, NFC/surrogate checks, and M5D3–M5D6 constructor validation reject malformed v1 data without defaults/coercion. Depth 64/65 and the escaped-backslash-versus-actual-surrogate distinction are covered. |
| AC-M5D7B-006 | **PASS.** Raw outer binding text is reconstructed and payload-only hash/final bytes are checked before M5D5. Integrity shape, alternate outer encoding/order/whitespace, malformed inner JSON, and noncanonical inner JCS are separated as specified. Inner-only failure returns a projection and no document. |
| AC-M5D7B-007 | **PASS after R2.** Caller bytes are cloned; result validation now rejects classification-inapplicable neutral-field mutation, document/projection value mismatch, unknown reason bits, default/bypass values, and unavailable getter access. Projection collection defense and combined reason flags are covered by the added focused cases. |
| AC-M5D7B-008 | **PASS.** Static review found no new IO/path/selection/quarantine/revision mutation/recovery write/input apply/clock/RNG/network/callback authority. The new files use only the approved engine-free boundary and references are limited to the exact M5D7B source/test/meta, contract/evidence/review/pre-gate, artifacts, and README index scope. |
| AC-M5D7B-009 | **PASS for execution.** Focused/full EditMode and the clean full PlayMode recheck have zero failed/skipped/inconclusive cases. The first PlayMode failure is separately classified above and does not hide a M5D7B failure. |

## P1 finding history and resolution

### P1-001 — result `Validate()` accepts reflection-bypassed neutral fields

The first review found that `ProfileCanonicalDecodeResultV1.Validate()` checked the fields relevant to each classification, but not all fields that had to be neutral when unavailable:

- `ValidCurrentInput`, `ValidInputMetadataRecoveryRequired`, and `ValidBindingRecoveryRequired` do not require `_unsupportedSchemaVersion == 0`.
- `ValidBindingRecoveryRequired` does not require the unavailable `_inputCompatibility == ProfileInputCompatibility.Current`.
- `UnsupportedProfileSchema` and `Invalid` do not require the unavailable `_inputCompatibility == Current` and `_unsupportedSchemaVersion == 0`.
- `UnsupportedProfileSchema` does not reject `_unsupportedSchemaVersion == 1`, despite the classification requiring a version different from v1.

For example, reflection-setting `_unsupportedSchemaVersion` to `123` on a valid current result, or `_inputCompatibility` to `AssetIdMismatch` on an unsupported result, leaves `Validate()` successful and all currently available getters usable. These are malformed programmer/reflection-bypass result representations under the contract's default/reflection and getter-availability invariants, and AC-M5D7B-007 requires rejection.

R2 applies the required narrow fix: every classification now validates all neutral/inapplicable fields (`Current` compatibility, zero unsupported-version sentinel, default document/projection, exact reason/failure values), rejects unsupported version equal to `ProfileContract.SchemaVersion`, and checks metadata document/projection value equality. The added reflection tests exercise the former repros and the focused suite is green. **P1-001 resolved.**

## Remaining P2 findings

- The first full PlayMode failure is the documented pre-existing `GameInputActions` finalizer warning; the clean recheck is green and this remains unrelated to the engine-free decoder.
- No M5D7B implementation blocker remains. The added combined binding+metadata, unavailable-getter, and tutorial projection collection checks close the prior coverage notes. Continue to retain the contract's inner decomposed/unpaired outer-precedence, unsupported malformed-suffix, integrity-shape, strict UTF-8, and depth-boundary gates in future changes.

## Recommendation

Recommend Astra mark M5D7B **Verified**. P0/P1 are zero after R2, all final suites are green, and the first-run PlayMode finalizer warning is independently classified as an unrelated known P2 with a clean 576/576 recheck. Preserve the decoder-only boundary: file selection/IO, quarantine, revision mutation, input application, and atomic recovery remain future owners.
