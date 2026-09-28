# VD-09 M5D7F profile quarantine adapter — Luna independent contract pre-gate

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7F profile quarantine adapter](../specs/work-contracts/2026-09-13-vd09-m5d7f-profile-quarantine-adapter.md)
- Compared against: VD-09 platform/load-recovery contract (`REQ-PLAT-010`/`AC-PLAT-008`/`AC-PLAT-009`), VD-09 `SYSTEM-CONTRACTS`, traceability, the approved P1 load-recovery decision, and Verified M5D7B/M5D7E/M5D7D boundaries.
- Scope: independent contract-only pre-gate. No runtime, test, contract, README, or evidence files were modified; Unity was not run.
- First-pass verdict: **CONDITIONAL FAIL — P0=0, P1=3, P2=3.** The historical findings and their required fixes are retained below.

## Executive finding

The proposed boundary is directionally consistent with `REQ-PLAT-010`: it executes one caller-supplied non-`None` preservation intent, re-observes the exact role-owned source leaf, names the recovery file from an injected UTC value plus readable-byte SHA-256 prefix (or `nohash`), allocates the smallest free ordinal, and performs exactly one same-directory `File.Move`. It does not select a load source, transform profile state, save, log, notify, use Unity identity/path, or retry/rollback/delete/copy. The typed presence distinction and recoverable-only failure boundary follow the M5D7D safety lessons.

Three contract-level closures are still required before implementation is deterministic and fail-closed: the meaning of source change when M5D7E deliberately does not retain source bytes, the exact normalized key/lease identity required by the alias concurrency AC, and a closed result invariant matrix. These are P1 because each can change whether bytes are quarantined, whether concurrent moves can collide, or whether a forged/default/reflection result is accepted.

## P1 findings

### P1-001 — Source-change policy is not implementably defined for same-class byte replacement

M5D7F says the service must fail closed when the re-observed source is “changed/no longer” the preservation target, and it compares the newly decoded classification (and unsupported version where applicable) with the candidate. However, `ProfileLoadCandidateV1` from Verified M5D7E intentionally does not carry the original source bytes or a source digest. For an `Invalid` candidate, two different corrupt byte sequences both re-decode as `Invalid`; for an `Unreadable` candidate, the original bytes cannot be observed; an `Unsupported` candidate has a version but not the original bytes. Thus the contract cannot distinguish “the diagnosed source is unchanged” from “another file with the same eligible classification replaced it” before the move. An external writer can cause the adapter to quarantine a different file while the contract still describes source-change fail-closed behavior.

This is also an ambiguity in the `StaleTemp` exception: its provenance permits movement regardless of current classification, but the contract should explicitly say that it is not claiming byte identity with the earlier temp observation.

Minimal fix: choose and state one semantic boundary. The preferred bounded fix is to define source revalidation as role/reason/classification eligibility only (including exact unsupported version), explicitly disclaim exact pre-candidate byte identity, and state that the `hash8`/`nohash` is computed from the bytes observed immediately before this move. If exact original-byte preservation is required instead, M5D7E must provide an immutable pre-observation fingerprint (including an explicit representation for unreadable); that would be a dependency/API expansion and conflicts with its current no-source-bytes/hash ownership, so it should not be silently inferred here.

### P1-002 — AC-008 requires alias serialization, but the lock-key identity is underspecified

The contract requires same-root aliases and multiple waiters to serialize allocation through postcheck and to remove the final registry entry, while different roots may proceed in parallel. It only says to call `GetFullPath`, trim trailing separators, and use a “same root” in-process lock. It does not specify the key comparer or the alias set. On Windows, `C:\data`, `c:\DATA\\`, and normalized dot-segment forms must use one key; an ordinal case-sensitive dictionary can create split locks and permit duplicate destination allocation/moves. Conversely, silently resolving symlinks/junctions would add filesystem authority not stated by the contract.

Minimal fix: define the key as the normalized non-root full path after `Path.GetFullPath` and separator trimming, with `StringComparer.OrdinalIgnoreCase` on Windows (and a stated ordinal policy on other supported platforms), with no symlink/junction resolution. Require an active-plus-waiter reference count under a registry mutex: increment before waiting, hold the per-key lock from destination allocation through postcheck, then release the lock and decrement/remove only at zero. Add the exact alias forms to AC-008 and the injected test seam.

### P1-003 — Public result combinations are not closed enough to enforce the stated fail-closed invariants

The result section gives several valid examples and rejects “outcome-stage/state/path/suffix/ordinal mismatch,” but it does not normatively close all combinations. In particular, it does not state that `Preserved` must be exactly `Stage.None`, that `SourceMissing` must be exactly `SourceRevalidation`, or that `MoveOutcomeUncertain` must be exactly `MoveCall`/`PostMoveValidation` with a chosen destination path and valid ordinal. It also leaves the destination path/ordinal and destination state constraints for a `FailedBeforeMove` that occurs after allocation partly open. A default/reflection-built result can therefore sit in an unlisted outcome/stage/state/path combination unless an implementation invents stricter rules. AC-009's “reflection matrix” is not a substitute for defining that matrix.

Minimal fix: add an explicit closed table (or equivalent normative bullets) for every outcome/stage pair. At minimum: `Preserved/None` requires source `Missing`, destination `Present`, exact recovery-child filename, non-`nohash` readable digest or `nohash` unreadable proof, and ordinal >= 0; `SourceMissing/SourceRevalidation` requires empty destination and ordinal `-1`; `FailedBeforeMove` is limited to the three pre-move stages with stage-specific probe states and zero move calls; `MoveOutcomeUncertain` is limited to `MoveCall` or `PostMoveValidation`, requires both post-attempt states probed and no success claim, and retains the allocated path/ordinal. Require role/reason-to-filename consistency and reject all other/default/unknown combinations through `Validate()` and every getter.

## P2 findings

### P2-001 — Collision exhaustion needs an explicit overflow-safe probe rule

The contract correctly requires the smallest nonnegative ordinal and failure when `int.MaxValue` is occupied or presence is uncertain, but does not state the overflow guard after probing `int.MaxValue`. Specify that the next ordinal is never computed after the maximum and that exhaustion returns `FailedBeforeMove/DestinationAllocation` without a move. The injected port should test `0`, `1`, `int.MaxValue-1`, and `int.MaxValue` occupied/presence-error cases without creating billions of files.

### P2-002 — Presence/error mapping should be made test-observable for both source and destination

The source rule explicitly makes only `FileNotFoundException`/`DirectoryNotFoundException` missing and excludes boolean `File.Exists`; destination presence is also said not to use `File.Exists`, but the precise mapping of access/security/other probe errors to `Unreadable`, uncertain presence, or typed allocation failure is left to implementation. Add injected `GetAttributes`/read cases proving inaccessible existing source is not treated as missing, inaccessible destination blocks allocation, and unexpected/fatal exceptions escape after lease cleanup.

### P2-003 — Platform/path and single-move assertions should be explicit in the implementation AC

The recovery child construction implies same-volume movement, and the no-fallback language is strong, but AC-001/003/007 should assert the exact `File.Move(source, destination)` call (non-overwrite overload), source/destination byte identity for readable sources, full post-move digest comparison, and no extra move/delete/copy/retry. State that symlink/junction resolution and cross-process writers are outside this bounded adapter, as M5D7D did for its single-process lease boundary.

## AC pre-gate status

| AC | Status | Independent basis |
|---|---|---|
| AC-M5D7F-001 | **CONDITIONAL** | Exact names, readable-byte preservation, and single move are specified; P1-001 must define what candidate/source identity means across a same-class replacement. |
| AC-M5D7F-002 | **CONDITIONAL** | Unsupported version equality is exact, but source-change semantics remain unresolved by P1-001. |
| AC-M5D7F-003 | **PASS by contract; P2 test precision** | UTC offset/format, lowercase SHA prefix, and exact `nohash` rule are present; add explicit probe/error tests. |
| AC-M5D7F-004 | **CONDITIONAL** | Smallest ordinal and unchanged existing files are specified; P1-002 and P2-001 must close lock/exhaustion determinism. |
| AC-M5D7F-005 | **PASS** | Stale temp is role-owned and never a load source; movement is explicitly independent of current classification. |
| AC-M5D7F-006 | **CONDITIONAL** | Missing/access/read/recovery/allocation failures are typed and no-move; P1-001 affects the source-changed branch. |
| AC-M5D7F-007 | **PASS by contract; P2 test precision** | One move, no fallback, independent probes, and post-digest are required; exact call-log/probe mapping should be asserted. |
| AC-M5D7F-008 | **BLOCKED** | AC requires same-root aliases, active+waiter ref-counting, and different-root parallelism, but P1-002 does not yet define a deterministic key/lease identity. |
| AC-M5D7F-009 | **BLOCKED** | Argument-before-IO and exception containment are clear, but P1-003 leaves public result combinations open. |
| AC-M5D7F-010 | **NOT YET EXECUTABLE** | This is a pre-gate; no implementation/evidence exists to run or hash. The contract must not claim a final Luna P0/P1 result before post-implementation review. |

## Boundary and allowlist review

The proposed scope is consistent with VD-09 and the P1 approval: no load precedence, previous/default promotion, input repair, atomic save, notification, logging sink, clock read, Unity path lookup, gameplay state, or run/scene identity is introduced. M5D7E remains the source of the preservation intent; M5D7F owns only filesystem observation, recovery naming, one move, and typed result reporting. The exact allowlist is appropriately limited to one runtime file/meta, one EditMode test/meta, this contract, its evidence/review reports, and a minimal README link. Existing M5D3–M5D7E sources, asmdefs, packages, settings, scenes, prefabs, input assets, and other files remain read-only.

## Recommendation

**Do not approve yet.** There is no P0, but the contract has three P1 gaps: clarify semantic versus byte identity during source revalidation, specify the Windows normalized lock key and ref-counted lease lifecycle, and publish a closed result invariant matrix. After those repairs, a bounded implementation can be reviewed for exact `GetAttributes` presence handling, filtered recoverable exceptions, collision overflow, one-move/post-digest behavior, and allowlist compliance.

## Second-pass review after contract amendment

The amended contract closes all three historical P1s and also promotes the requested P2 details into normative text/AC coverage. This second pass is contract-only; no Unity or implementation tests were run.

- **P1-001 resolved:** the contract now states that revalidation is intentionally classification-only, compares unsupported versions exactly, allows an unreadable primary/previous that becomes `Invalid` or any `UnsupportedProfileSchema`, rejects current/input-recovery transitions, and explicitly disclaims identity with the M5D7E-observed bytes. The hash is the digest of the bytes read immediately before the move. Stale-temp movement is explicitly provenance-based and makes no earlier-byte identity claim. This is implementable without expanding M5D7E's no-byte/no-hash API.
- **P1-002 resolved:** the normalized full root is the lease key; Windows uses `StringComparer.OrdinalIgnoreCase`; symlink/junction resolution is excluded. The active-plus-waiting ref-count is incremented under the registry mutex before waiting, the single per-root monitor spans source revalidation through postcheck, and the final entry is removed only at zero. AC-008 now names aliases, waiter serialization, different-root parallelism, and the excluded scopes.
- **P1-003 resolved:** the four-row outcome/stage/source-state/destination-state/path/suffix/ordinal matrix is now normative, all getters call full `Validate()`, compatible role/reason is required, and all other/default/unknown/reflection combinations are rejected. The earlier open result cross-product is closed.
- **Prior P2-001 resolved:** collision increment is checked; `int.MaxValue` occupancy cannot wrap, go negative, retry, or move, and AC-009 requires the boundary test.
- **Prior P2-002 resolved:** destination presence is a typed `File.GetAttributes` probe, only not-found is free, and AC-009 requires source/destination access and unexpected-presence cases plus fatal cleanup behavior.
- **Prior P2-003 resolved:** AC-007/010 now require at most one exact `File.Move`, independent typed post-probes, full readable digest proof, and static absence of delete/copy/overwrite/retry/fallback/clock/Unity authority. Symlink/junction and cross-process serialization are explicitly outside scope.

### Second-pass AC matrix

| AC | Second-pass status | Basis |
|---|---|---|
| AC-M5D7F-001 | **PASS by contract** | Exact invalid/stale-temp names, current observed bytes, and one move are defined; classification-only source identity is explicit. |
| AC-M5D7F-002 | **PASS by contract** | Unsupported version equality and classification/source-change rejection are precise. |
| AC-M5D7F-003 | **PASS by contract** | UTC, lowercase SHA-256 prefix, `nohash`, and culture independence are exact. |
| AC-M5D7F-004 | **PASS by contract** | Smallest ordinal, typed presence, checked max boundary, and unchanged existing files are specified. |
| AC-M5D7F-005 | **PASS by contract** | Stale temp is never a load source and is provenance-only preservation. |
| AC-M5D7F-006 | **PASS by contract** | Missing/access/read/recovery/allocation and classification transitions have typed no-move outcomes. |
| AC-M5D7F-007 | **PASS by contract** | Single move, no fallback, independent probes, and readable full-digest postcheck are required. |
| AC-M5D7F-008 | **PASS by contract** | Exact normalized Windows key and ref-counted source-revalidation-to-postcheck lease are now fixed. |
| AC-M5D7F-009 | **PASS by contract** | Closed four-row result matrix, getter revalidation, checked overflow, typed presence, and exception cleanup are required. |
| AC-M5D7F-010 | **PASS as a future gate** | Static/focused/full test requirements and the no-orchestration boundary are explicit; execution remains for post-implementation review. |

### Second-pass verdict

**PASS — P0=0, P1=0, residual P2=1. Recommend Astra advance M5D7F to Approved.** The remaining P2 is a deployment-boundary note: because symlink/junction resolution is intentionally out of scope, the later production owner should either ensure the `recovery` child remains on the root volume or add a separate path-integrity contract; it does not block this bounded adapter pre-gate.
