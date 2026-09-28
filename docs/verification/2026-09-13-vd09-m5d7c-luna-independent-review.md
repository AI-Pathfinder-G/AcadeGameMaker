# VD-09 M5D7C recovery transformation core — Luna independent implementation review

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7C profile recovery transformation core](../specs/work-contracts/2026-09-13-vd09-m5d7c-profile-recovery-transformation-core.md)
- Evidence: [Terra implementation evidence](2026-09-13-vd09-m5d7c-implementation-evidence.md)
- Compared against: Approved VD-09, VD-05, SYSTEM-CONTRACTS, M5D3–M5D7B Verified APIs/contracts, and the M5D7C second pre-gate
- Verdict: **R3 PASS — P0=0, P1=0; recommend Astra mark Verified**
- Scope: independent source, test, evidence, artifact, dependency, and allowlist recheck. Runtime/contract implementation was not modified and Unity was not rerun.

## R3 recheck

Terra's bounded test-only repair closes both first-pass P1 findings. The added focused matrix now drives malformed and noncanonical inner binding recovery through both `PlanInputRepair` and `PlanPreviousPromotion`, asserts the projection's non-input values and defensive collection behavior, checks the exact default payload and independent SHA-256, and exercises revision `0`, arbitrary positive, `long.MaxValue-1`, and overflow. The runtime implementation and its scoped hash are unchanged; no new authority or allowlist expansion was introduced.

The only remaining test precision note is that `long.MaxValue` is directly exercised on a recovery classification, not separately on a current-input previous-promotion classification. Both entries share the same private `Plan` overflow guard before classification-specific output, and current `long.MaxValue-1` is exercised; this is a non-blocking P2 test-clarity improvement, not a semantic or acceptance blocker.

## Evidence integrity

I parsed the supplied R2 and R3 NUnit XML files and recomputed their SHA-256 values. The earlier first full PlayMode run has exactly one failure, and its message/stack trace is the known unrelated `GameInputActions.Gameplay.Disable()` finalizer warning. The R3 result is clean.

| Suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused EditMode R2 | 8 | 8 | 0 | 0 | 0 | `EECFEEB61CDA87D9E73F3D0CA1EA8066B03A1FF42052645093BDF81A018D361C` |
| Full EditMode | 553 | 553 | 0 | 0 | 0 | `A418A474D7AD42BB8586FEC7C4DCF9E30E6E208D60C4CF3004F504138CD8EAB9` |
| Full PlayMode first run | 576 | 575 | 1 | 0 | 0 | `F4A8BF562DF58405534DD73F2F52C05D02AFD7CC0979938EDBD514F017F6C06A` |
| Full PlayMode clean recheck | 576 | 576 | 0 | 0 | 0 | `7E0EDAA4CCCFF0A8AD9ED188D0C4FF6EBB166B3096BF5EC5C4C2361D993E1589` |

The final R3 artifacts independently parse as follows and match the implementation evidence:

| Final suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused EditMode R3 | 10 | 10 | 0 | 0 | 0 | `D628E651301EA8ACCC6A20B85A37F0DCC20067F5670C3FEEE61FDC5F99371738` |
| Full EditMode R3 | 555 | 555 | 0 | 0 | 0 | `CB2DCDBC014965AC41FFBB49F18B209F6A14BA8006414D9D91856B239B2EF4B0` |
| Full PlayMode R3 | 576 | 576 | 0 | 0 | 0 | `0B4542CD5D566A1A7122AFD45DE166687342B9F7B70A1C81C54E3E8E8B831B44` |

The workspace hashes match the implementation evidence:

| File | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileRecoveryPlannerV1.cs` | `C99D1BB752924154F7A36D91C1F279376E422F2DF13B9BCDA2569FCEED28F663` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileRecoveryPlannerV1.cs.meta` | `044B6BCD6CE279110052A4692E8D680E7A7C19166E1EF4126FDF3766CAF96EEA` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileRecoveryPlannerV1Tests.cs` | `F58B4E9877D92D66AAF518795DA0927DEAEAC738C641C5F49223F4BF95FDC44A` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileRecoveryPlannerV1Tests.cs.meta` | `33858F59C5BBBC7CCA03EE30E5CEFBC5E02AEE026389CE6153FC674C834BCB28` |

## P1 findings

### P1-001 — RESOLVED: M5D7B binding-recovery path was not exercised by the M5D7C transformation tests

The M5D7C focused test has three metadata-mismatch inputs (`asset`, `schema`, and both), but no `ValidBindingRecoveryRequired` source. The previous-promotion test similarly covers one metadata mismatch only; it does not pass a malformed or noncanonical inner binding result to `PlanPreviousPromotion`. M5D7B already provides a helper that can construct an outer-canonical file with `{bad}` or noncanonical inner JSON, but the M5D7C tests never invoke the planner with that result.

This leaves the two central projection-only branches unproven at this milestone even though the runtime branch is visibly implemented:

1. `PlanInputRepair(ProfileCanonicalDecodeResultV1)` must consume a binding-recovery result with no document, preserve the projection's settings/tutorial/progression, replace the entire input with current ID/schema/empty sentinel, and emit exact `InputRepair` with revision `r+1`.
2. `PlanPreviousPromotion(ProfileCanonicalDecodeResultV1)` must consume the same binding-recovery classification as a caller-selected previous candidate, preserve the projection, replace input defaults, and emit exact `PreviousPromotion|InputRepair`.

R3 adds malformed and noncanonical inner binding cases, invokes both planner entries for each, and asserts source revision, preserved seed/choice/skill/branches/tutorial values, current input, exact reasons, and defensive source collection behavior. **P1-001 resolved.**

### P1-002 — RESOLVED: required exact-default/revision/defensive-collection matrix was incomplete

The implementation statically contains the approved values and correct `long` increment guard, but the M5D7C test evidence does not cover all contract-required boundaries:

- AC-M5D7C-001 checks source/result/reason, settings volumes/window, current compatibility and empty override, but not both aim-invert values, empty tutorial, all null progression fields, `RefusesOwnershipTransfer`, empty branches, or an independently expected canonical payload/hash.
- AC-M5D7C-004 tests one arbitrary revision (`7`) and `long.MaxValue` rejection, but does not test source revision `0` or `long.MaxValue-1 → long.MaxValue`, nor explicitly verify mutation-free behavior at the latter boundary.
- AC-M5D7C-006 checks current compatibility and empty override but uses only empty tutorial/branch collections and does not prove non-input collection value preservation plus defensive independence through a repaired/promoted plan.

R3 adds the exact default payload and independently calculated hash, revision `0`/`37`/`long.MaxValue-1` checks across applicable normal/recovery entries, and non-empty tutorial/completed-branch preservation after source-array mutation. The common overflow guard is exercised at `long.MaxValue` for recovery entries and is statically shared by both methods. **P1-002 resolved.**

## P2 findings

- The first full PlayMode failure is unrelated to M5D7C: XML identifies `AcM5D1004_SourceTickOverflowClosesWithoutRequestOrCompletion`, but the only failure is the known `GameInputActions.Gameplay.Disable()` finalizer warning. The clean recheck is 576/576 with zero failed/skipped/inconclusive, so this does not block M5D7C after the P1 evidence gaps are closed.
- The reflection matrix covers default proof fields, repaired proof input, reason closure, document and result revision, but does not directly inject an invalid `_proofKind` or mutate a non-default settings/tutorial/progression proof. Add those if the existing reflection harness is retained; this is lower risk because the runtime maps each legal reason to an explicit proof kind and validates the semantic fields.
- `docs/README.md` still says Luna/Astra acceptance is pending. That is integration documentation drift, not a runtime blocker.

## Independent source and boundary review

- **Default and `-1/0`:** `PlanDefaultBootstrap()` constructs BorderlessFullscreen, 1000/700/800 volumes, both aim-invert flags false, current input ID/schema `1` with empty override, empty tutorial, revision `0`, null seed/choice/skill, `RefusesOwnershipTransfer`, and empty branches. `ProfileRecoveryPlanV1.Validate()` checks the exact default and source `-1`/result `0` relation.
- **Input repair and promotion:** metadata recovery consumes the M5D7B projection; binding recovery consumes projection-only values; current previous promotion retains canonical override text. Every non-default plan checks source revision, result `source+1`, semantic settings/tutorial/progression equality, and either input equality or current default input.
- **Overflow/misuse:** entry validation precedes increment; `long.MaxValue` throws `InvalidOperationException` without creating a result. Well-formed wrong classifications throw `ArgumentException`; default/reflection-invalid source results are wrapped as `ArgumentException` with an inner `InvalidOperationException`; malformed plans/getters remain `InvalidOperationException`.
- **Reason closure/proof:** `IsAllowedReason` accepts exactly `DefaultBootstrap`, `InputRepair`, `PreviousPromotion`, or `PreviousPromotion|InputRepair`; reason-to-proof-kind checks and neutral proof checks fail closed. The added reflection cases cover the principal default/repaired neutral-field injections.
- **No authority/allowlist:** the new runtime assembly has `noEngineReferences` and the planner source uses only `System`/`System.Collections.Generic` plus verified Profile value APIs. It contains no source bytes, parser, IO/path/file selection, quarantine, persistence/save, Input System apply, clock, RNG, network, callback, Unity, or scene authority. Scoped source/meta hashes match evidence. The broader worktree is already dirty from other milestones; no unrelated M5D7C runtime/test file is attributable from the scoped evidence.

## AC status

| AC | Independent result | Basis |
|---|---|---|
| AC-M5D7C-001 | **PASS** | R3 asserts every approved default field through the exact payload literal, source `-1`, result `0`, repeatability, and independently calculated payload hash. |
| AC-M5D7C-002 | **PASS** | Metadata asset/schema/both and malformed/noncanonical binding recovery are exercised; both planner paths preserve non-input values and replace input defaults. |
| AC-M5D7C-003 | **PASS** | Current previous canonical override and previous metadata/binding recovery combined reasons/current defaults are exercised. |
| AC-M5D7C-004 | **PASS** | Revision `0`, arbitrary positive, `long.MaxValue-1`, and `long.MaxValue` overflow behavior are covered; shared guard is statically verified across entries. |
| AC-M5D7C-005 | **PASS** | Focused test covers current/invalid/unsupported/default misuse and overflow exception distinctions; shared entry path is visible. |
| AC-M5D7C-006 | **PASS** | R3 checks current-compatible empty input and value-preserved non-empty tutorial/branch collections after source mutation; dependency getters provide defensive values. |
| AC-M5D7C-007 | **PASS with P2 extension** | Exact reason rejection and primary neutral proof injections pass; `_proofKind`/non-default proof mutation cases are a lower-priority extension. |
| AC-M5D7C-008 | **PASS for execution/boundary** | Focused/full EditMode and clean full PlayMode recheck have zero failed/skipped/inconclusive; first-run warning is separately classified and no persistence/recovery execution overclaim appears. |

## P0/P1/P2 summary

- P0: none.
- P1: none remaining. P1-001 and P1-002 are resolved by the R3 test additions and evidence.
- P2: known unrelated first-run PlayMode finalizer warning (clean recheck/R3 is green); direct current-classification `long.MaxValue` overflow test and additional proof-kind reflection cases would improve precision; stale README status text.

## Recommendation

Recommend Astra mark M5D7C **Verified**. P0/P1 are zero: the R3 focused/full evidence is clean, the scoped implementation/test hashes match evidence, and the repaired matrix covers the previously missing binding-recovery, exact-default/hash, revision, and collection-preservation cases. The earlier one-case PlayMode finalizer warning is unrelated and is superseded by clean R3 full PlayMode. Preserve caller-owned primary/previous selection and the engine-free/no-persistence boundary; no runtime authority expansion is needed.
