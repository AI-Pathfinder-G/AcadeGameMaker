# VD-09 M5D7F profile quarantine adapter — Luna independent implementation review

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7F profile quarantine adapter](../specs/work-contracts/2026-09-13-vd09-m5d7f-profile-quarantine-adapter.md)
- Evidence: [Terra implementation evidence](2026-09-13-vd09-m5d7f-implementation-evidence.md)
- Compared against: Approved M5D7F, its Luna second pre-gate, VD-09 `REQ-PLAT-010`/load recovery, `SYSTEM-CONTRACTS`, P1 profile load-recovery approval, and Verified M5D7B/M5D7E/M5D7D boundaries.
- Scope: independent source, test, evidence, XML, authority, and allowlist review. Runtime/test/contract/evidence/README were not modified; Unity was not rerun.
- First-review verdict: **CONDITIONAL FAIL — P0=0, P1=1, P2=4.** The historical P1 and its required repair are retained below.

## Executive finding

The implementation follows the approved bounded shape: it validates the M5D7E candidate/reason before acquiring a normalized per-root lease, re-observes only the role-owned fixed leaf, performs classification-only revalidation, computes UTC/hash8 or `nohash`, allocates a collision-free recovery child, executes one non-overwrite `File.Move`, and post-probes source/destination with a full readable-byte digest check. It does not select a load source, transform profile data, save, log, notify, read the system clock, use Unity authority, or delete/copy/retry/rollback.

The primary implementation blocker is the public result proof. `ProfileQuarantineResultV1.Validate()` validates only a broad recovery-directory/`profile-` filename shape, not the exact reason-specific name, UTC token, hash suffix/path equality, or collision ordinal/path equality required by M5D7F. It also has no retained proof of whether `nohash` was justified by an unreadable pre-move source. The normal BCL path emits correct names, but a reflection/default-bypass result can pass a getter after these fields are forged, and AC-009's test does not exercise those fields.

## P1 finding

### P1-001 — Result validation accepts forged destination identity and unproven `nohash`

At `ProfileQuarantineServiceV1.cs:37`, `ValidDestination()` checks only that the path is fully qualified, its parent compares to `<root>\\recovery`, its extension is `.json`, and its basename starts with `profile-`. It does not verify the exact reason-derived stem (`profile-invalid-primary`, `profile-unsupported-v{version}`, etc.), the injected UTC token, that the filename hash token equals `HashSuffix`, or that the ordinal matches the `-N` suffix. For example, starting from a valid `Preserved` result, reflection can replace `_destinationPath` with `<root>\\recovery\\profile-not-the-reason.json`, replace `_hashSuffix` with another valid eight-character lowercase token, or set `_collisionOrdinal` to `1`; `Validate()` still accepts the result as long as the broad shape and states remain valid. The result getter then exposes a proof that does not represent the exact quarantine name required by AC-001/002/003/004/009.

The same proof gap affects `nohash`: `NoHashAllowed()` checks only the reason (line 42). The result does not retain an internal pre-move readability/digest-proof bit, so a reflection-built readable `StaleTemp` result can claim `nohash`; the focused reflection test mutates only `_outcome` and does not catch this. Production BCL execution chooses `nohash` only when its read failed, but the contract requires all getters and reflection-bypassed proof to fail closed.

Minimal fix: retain private validated destination identity/proof data sufficient for `Validate()` to reconstruct the exact basename, or parse and cross-check the basename against role/reason, UTC token, hash suffix, and ordinal (including the unsupported version, which currently is not stored in the result). Retain a private `hashWasProven`/`sourceWasUnreadable` proof bit and reject `nohash` unless that bit is true; ensure every getter validates the same proof. Add reflection cases for wrong reason prefix, wrong UTC, path/hash mismatch, ordinal mismatch, unsupported-version mismatch, and readable-stale `nohash`. Do not weaken the exact naming contract merely because production BCL output is correct.

## P2 findings and test-precision notes

### P2-001 — Hash operation is outside recoverable-failure containment

`Hash(bytes).Substring(0, 8)` at line 74 is outside the recoverable catch surrounding destination allocation. A recoverable `IOException` from the injectable hash operation propagates instead of becoming a typed `FailedBeforeMove/DestinationAllocation` result. A malformed or uppercase injected hash can also be used to build a destination before the result getter eventually rejects it. The production BCL hash implementation is deterministic and returns the expected lowercase 64-character value, so this is lower severity than P1-001; nevertheless, wrap expected recoverable hash faults at the chosen pre-move stage and validate the full lowercase digest token before any move.

### P2-002 — AC coverage does not exercise all exact result fields or unreadable roles

The focused suite has 13 passing tests and covers representative real invalid/unsupported/stale moves, collision `2`, classification mismatch, typed stage failures, tampering, fatal cleanup, and the basic alias/different-root path. However, AC-009's reflection test mutates only `_outcome`; it does not mutate destination basename, UTC, hash suffix, ordinal, root, or `nohash` proof. AC-001/002/003 use representative primary/previous/stale cases rather than both invalid roles, both unsupported roles, and both unreadable roles. These omissions do not hide another normal-path source defect, but they prevent the claimed exhaustive reflection/name matrix from being independently demonstrated.

### P2-003 — Boundary cases are required but not executed by the focused test

The runtime uses checked ordinal logic and correctly refuses to wrap after `int.MaxValue`, and `ProbePresence` uses `File.GetAttributes` with only not-found mapped to missing. The focused suite does not inject `int.MaxValue` occupancy, destination access/presence uncertainty, or source presence access failure. Add those deterministic port cases before treating AC-003/004/006/009 as fully evidenced.

### P2-004 — Alias and waiter test is narrower than the amended AC

The runtime dictionary uses `StringComparer.OrdinalIgnoreCase`, and its `Acquire`/`Release` reference count spans the monitor operation. The test proves a trailing-separator waiter blocks and a different root proceeds, then observes an empty registry. It does not use a case-changing alias or a barrier proving every waiter has incremented the registry count before the active operation releases. Add `C:\\data`/`c:\\DATA\\`-style coverage on Windows and an explicit waiter-registration barrier. This is evidence precision, not a second source P1.

## Source and authority review

- **Revalidation:** `Eligible()` implements the amended classification-only policy: invalid reasons require `Invalid`; unsupported reasons require `UnsupportedProfileSchema` with exact version; unreadable reasons accept `Invalid` or any unsupported version and reject current/input-recovery; stale temp is unconditional by temp provenance. No earlier M5D7E bytes/hash are claimed.
- **Naming:** `Stem()` uses fixed role names, exact UTC formatting, invariant version formatting, and the newly observed digest prefix. Readable bytes use SHA-256; only an unreadable accepted source uses `nohash`.
- **Presence and move:** `ProbePresence()` uses `File.GetAttributes`, maps only `FileNotFoundException`/`DirectoryNotFoundException` to missing, and does not use `File.Exists`. The BCL port calls `File.Move(source, destination)` once, without overwrite/delete/copy/retry/fallback. Post-validation independently probes source/destination and compares full digests when readable.
- **Failures:** Recoverable `IOException`, `UnauthorizedAccessException`, and `SecurityException` are filtered at source, directory, allocation, move, and post-probe paths; unexpected/fatal exceptions escape and the outer `finally` releases the lease. The hash seam exception noted in P2-001 is the remaining containment precision issue.
- **Lease/path:** `ValidateArguments()` rejects null/blank/relative/root/bad-offset/malformed candidate/reason before lease/IO. `GetFullPath` plus trailing-separator trimming and the case-insensitive registry key match the amended contract; active-plus-waiting counts are released at zero. Symlink/junction identity and cross-process locking remain explicitly out of scope.
- **Authority/allowlist:** The runtime contains no Unity, persistent-data-path, wall-clock, gameplay, selection, transformation, save, logging, notification, network, RNG, delete, or copy authority. The scoped runtime/test/meta/evidence files match the contract allowlist; unrelated dirty-worktree files predate this review and were not changed.

## Evidence integrity

I parsed each final NUnit XML and recomputed its SHA-256. All three are `Passed` with zero failed, skipped, or inconclusive cases, matching Terra's evidence. The earlier focused compile failure is diagnostic history only; the final R2 focused run is the reviewed artifact.

| Suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused EditMode R2 | 13 | 13 | 0 | 0 | 0 | `93E8E1F4F8A256F894ABBED64EC07A7ED244CA778CA1846BE051EC6120567DA1` |
| Full EditMode | 607 | 607 | 0 | 0 | 0 | `86E83A1F6B336317D2A26F52CC55DC17822CAA91CD38BCF7F761F8DD211F5468` |
| Full PlayMode | 576 | 576 | 0 | 0 | 0 | `CA914D912BF2ACDE39190DBF1C45E5ABB35473833865B95D2D8E830C90C13CE0` |

Workspace hashes independently match the evidence:

| Scoped file | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileQuarantineServiceV1.cs` | `187FF1886E2FBA1E58CABD54EB90C0DFB352224E0EEB13243AAB88D7328540FE` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileQuarantineServiceV1.cs.meta` | `07A30262C8FDEB170D25B8546556E17E9E6F7BDB349555BB00DCBD029A311ED5` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileQuarantineServiceV1Tests.cs` | `8DA98EAC7CDB29DFDB2DEAC5D514CE08CCAD3D73D4DF8C09E5AF77966248F856` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileQuarantineServiceV1Tests.cs.meta` | `F01112A5674D6217FF5FA888566898F2BDE751F5ACE904B32F5CA1FC2016FE16` |
| `docs/verification/2026-09-13-vd09-m5d7f-implementation-evidence.md` | `148FFD2D22459246F5E1C5832AB2E6709500917057204BA2CFEE56F036E588BA` |

## AC status

| AC | Independent status | Basis |
|---|---|---|
| AC-M5D7F-001 | **CONDITIONAL** | Real invalid-primary bytes are exact and the move is single; previous/stale matrix is representative only, and result exact-name proof is blocked by P1-001. |
| AC-M5D7F-002 | **CONDITIONAL** | Unsupported previous/version and mismatch are covered; unsupported primary and full exact result-name/reflection proof remain incomplete. |
| AC-M5D7F-003 | **PASS with P2 coverage gap** | BCL hash/UTC output and representative hash8/nohash names are correct; all unreadable roles and culture/error cases are not focused-tested. |
| AC-M5D7F-004 | **PASS with P2 coverage gap** | Smallest ordinal `2` works and existing files remain; `int.MaxValue` is static-only. |
| AC-M5D7F-005 | **PASS by source review** | Stale temp is unconditional by temp provenance and never a load source; representative real stale move passes. |
| AC-M5D7F-006 | **PASS with P2 coverage gap** | Typed stage outcomes, mismatch no-move, and at-most-one move are implemented; access/presence cross-products are not all tested. |
| AC-M5D7F-007 | **PASS with P2 coverage gap** | Move/post tamper paths produce uncertain results without fallback; all independent probe variants are not focused-tested. |
| AC-M5D7F-008 | **PASS with P2 coverage gap** | Source lease implementation matches amended key/ref-count rules; case alias and waiter-registration evidence is narrow. |
| AC-M5D7F-009 | **FAIL — P1-001** | Normal argument/fatal cleanup works, but exact result destination identity and nohash proof are forgeable through reflection and not rejected by `Validate()`. |
| AC-M5D7F-010 | **CONDITIONAL** | Final XMLs are all green and static scope is bounded, but the contract requires Luna P0/P1 zero and P1-001 remains. |

## Recommendation

**Do not mark M5D7F Verified.** P0=0 and the runtime’s normal quarantine transaction is substantially aligned with the Approved contract, but P1-001 is a real public result-invariant/AC009 gap. Require exact destination identity and digest-proof validation, add the corresponding reflection regressions, then obtain a focused rerun and independent recheck. After that, the current P2 items can remain evidence-precision follow-ups if no new source issue appears.

## R2 independent recheck after Terra remediation

The R2 source and test changes directly address the prior P1. The corrected evidence record now contains one final scoped-hash table (the earlier duplicate stale table was removed), and its hashes match the current workspace. No Unity run was repeated by Luna; the supplied fresh XMLs were parsed and hashed independently.

- **P1-001 resolved:** `ProfileQuarantineResultV1` now retains private normalized-root, 19-character UTC token, optional unsupported version, and `hashWasProven` fields. `ValidDestinationProof()` reconstructs the exact reason-specific basename, UTC, digest suffix, collision ordinal, and recovery child path. `NoDestinationProof()` closes the no-path rows. Getters call full `Validate()` before returning. The focused test mutates destination path, UTC, hash suffix, ordinal, root, unsupported version, and hash proof independently, and rejects readable-stale `nohash`; it also keeps the original valid result assertions.
- **Hash exception boundary repaired:** readable-source hashing is inside a recoverable `DestinationAllocation` catch. `RequireDigest()` rejects null, short, uppercase, or non-hex injected digests as unexpected `InvalidOperationException` before any move. The focused matrix covers `hash-io` as typed pre-move failure and `hash-malformed` as escape with lease cleanup.
- **Unreadable and presence coverage repaired:** both unreadable primary and previous roles are tested with `nohash`; source presence access failure is a typed `SourceRevalidation` failure; missing remains `SourceMissing`. The BCL presence implementation still uses `File.GetAttributes`, mapping only file/directory-not-found to missing.
- **Alias/waiter coverage repaired:** the same-root waiter uses a case-changing plus trailing-separator alias, and the test waits until the internal lease reference count is `2` before releasing the active move. Different roots proceed concurrently and the final registry count is zero. Runtime keying and release remain ordinal-ignore-case and ref-counted around the whole transaction.
- **UTC correction verified:** the invariant token is exactly 19 characters (`yyyyMMdd'T'HHmmssfff'Z'`) and round-trip checked under invariant culture.

### R2 residual P2 notes

- The source has checked `int.MaxValue` collision exhaustion, but no focused injected test actually simulates the maximum occupied candidate; add one if stronger boundary evidence is desired.
- Some exact-name assertions for unsupported/stale cases use `EndsWith` rather than comparing the full normalized path, and the focused suite does not enumerate every role/classification cross-product. The normal source paths and representative real moves are correct; these are evidence-precision gaps, not new source blockers.
- The injected call log proves at-most-one move and stage outcomes but does not assert every post-probe call ordering detail. This remains a low-risk test precision improvement.

## R2 evidence integrity

I parsed the final R2 NUnit XML files and recomputed their SHA-256 values. Every test case is `Passed`; failed, skipped, and inconclusive counts are all zero. The final R2 focused/full runs use Unity `6000.6.0f1` after the entitlement preflight passed. The earlier focused compile failure and the separate sandbox licensing stall are diagnostic history only; neither is present in the reviewed final artifacts.

| Suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused EditMode R5 | 18 | 18 | 0 | 0 | 0 | `264127DB457F2321FC94B36E76E6C7E20B7276EF0A547B5939D65D1363EB45DB` |
| Full EditMode R2 | 612 | 612 | 0 | 0 | 0 | `F28E332B12EE4D209F55740A13BCBE8E21D1D059856E3C613ACC0DE329DDAF08` |
| Full PlayMode R2 | 576 | 576 | 0 | 0 | 0 | `F2CDAB5FA842B6A06927A8F3573A331C0EE4A85AA3BA071D774A56AB84CDEB8B` |

Corrected workspace/evidence hashes:

| Scoped file | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileQuarantineServiceV1.cs` | `CF73B2452B1F7839A346F13E65E4076F2B688F067DD2B66ED931F7D3810FF162` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileQuarantineServiceV1.cs.meta` | `07A30262C8FDEB170D25B8546556E17E9E6F7BDB349555BB00DCBD029A311ED5` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileQuarantineServiceV1Tests.cs` | `8FCDB5CE41671C4CC991C525F25043690674DB7BE74CAD1B35762D455CAEFDAA` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileQuarantineServiceV1Tests.cs.meta` | `F01112A5674D6217FF5FA888566898F2BDE751F5ACE904B32F5CA1FC2016FE16` |
| `docs/verification/2026-09-13-vd09-m5d7f-implementation-evidence.md` | `A9F5CD6C47FD1D51FDC4BB4BC7F735B5C0D9C45F69C79F9D63FC25F0CD4CA762` |

## R2 AC status

| AC | R2 independent status | Basis |
|---|---|---|
| AC-M5D7F-001 | **PASS** | Real invalid-primary move preserves exact bytes; exact proof now rejects forged destination identity. Shared role-specific code covers previous/stale paths. |
| AC-M5D7F-002 | **PASS** | Unsupported version is retained in private proof and encoded exactly; classification/version mismatch remains zero-move. |
| AC-M5D7F-003 | **PASS with P2 precision note** | Lowercase SHA-256 prefix, `nohash`, UTC format, and both unreadable roles are covered; some name assertions are suffix-based. |
| AC-M5D7F-004 | **PASS with P2 boundary note** | Smallest ordinal and unchanged collisions pass; checked maximum is source-correct but not focused-injected. |
| AC-M5D7F-005 | **PASS** | Stale temp remains provenance-only and never load source; readable/nohash proof is tested. |
| AC-M5D7F-006 | **PASS** | Classification-only transitions, missing, presence access, read, directory, allocation, and hash-I/O faults return typed no-move results. |
| AC-M5D7F-007 | **PASS** | Move/post faults return uncertain independently probed states without fallback; readable post-digest tampering is covered. |
| AC-M5D7F-008 | **PASS** | Case/trailing aliases serialize with observed waiter registration; different roots proceed and final lease registry is removed. |
| AC-M5D7F-009 | **PASS** | Closed four-row result proof, all targeted reflection fields, malformed digest, unexpected/fatal propagation, and lease cleanup are covered. |
| AC-M5D7F-010 | **PASS** | Focused 18/18, full EditMode 612/612, and full PlayMode 576/576 are clean; static authority/allowlist boundary is preserved. |

## R2 verdict and recommendation

**PASS — P0=0, P1=0, residual P2=3. Recommend Astra mark M5D7F Verified.** The previous P1 exact destination/nohash proof is closed in both source and focused reflection tests. Final XML counts/hashes and corrected evidence hashes match independently. Remaining P2 items are test precision improvements only and do not block acceptance of the bounded quarantine adapter.
