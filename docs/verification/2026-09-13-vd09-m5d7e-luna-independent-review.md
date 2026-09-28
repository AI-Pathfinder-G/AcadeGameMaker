# VD-09 M5D7E profile load selection core — Luna independent implementation review

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7E profile load selection core](../specs/work-contracts/2026-09-13-vd09-m5d7e-profile-load-selection-core.md)
- Evidence: [Terra implementation evidence](2026-09-13-vd09-m5d7e-implementation-evidence.md)
- Compared against: Approved M5D7E, its Luna second pre-gate, VD-09 platform/load recovery, vertical-demo SYSTEM-CONTRACTS and TRACEABILITY, P1 profile load-recovery approval, and Verified M5D7B–M5D7D APIs
- Scope: independent source, test, evidence, XML, dependency, authority, and allowlist review. Runtime/test/contract/evidence/README were not modified and Unity was not rerun.
- Verdict: **PASS — P0=0, P1=0; recommend Astra mark M5D7E Verified.**

## Executive finding

The implementation is a bounded pure selector over already-observed primary/previous/temp candidates. It validates argument-position roles and candidate kinds, gives current primary precedence, delegates primary input repair and previous promotion exactly to M5D7C, preserves the approved `-1→0` default behavior, ignores temp as a source under every revision/classification, and emits only immutable preservation intent. The amended candidate getter rule is implemented: Missing/Unreadable decode getters validate then throw, while Decoded accepts only a revalidated non-default result from the five M5D7B classifications.

Private plan proofs revalidate candidate roles, direct document equality, M5D7C recovery plan equality, source/result revisions, reason flags, current-compatible result documents, and preservation intent. Reflection/default/unknown/mismatch cases fail closed. The source retains no IO/path/hash/time/quarantine/save/notification/input-application/Unity authority.

## Source and dependency review

- **Precedence:** `Select` validates all three candidates, then chooses current primary, primary input repair, selectable previous promotion, or default in that exact order. Previous and temp cannot override a current primary. This matches REQ-PLAT-010/011 and the P1 load-recovery approval.
- **M5D7C delegation:** Input-recoverable primary calls `PlanInputRepair`; selectable previous calls `PlanPreviousPromotion`; no-selectable candidates call `PlanDefaultBootstrap`. The resulting plan copies exact source/result/reason/document values and revalidates the recovery proof.
- **Overflow and direct maximum:** M5D7C overflow exceptions propagate for input-repair or previous-promotion sources; no fallback to default is attempted. A direct current primary at `long.MaxValue` is accepted without increment/save, as required.
- **Candidate invariants:** Factories validate roles/results. Missing/Unreadable carry default no-payload proof and their `DecodeResult` getter throws exact `InvalidOperationException`; Decoded revalidates and restricts classification to current, metadata recovery, binding recovery, unsupported, or invalid. Selector argument positions are checked independently.
- **Plan invariants:** All public getters call `Validate()`. Direct plans prove primary current document and same source/result revision with zero recovery reasons and no save. Recovery plans prove exact M5D7C output and non-default recovery proof. Result documents are revalidated and always current-compatible.
- **Temp/preservation:** Temp presence alone yields `StaleTemp`; missing temp yields `None`, regardless of temp revision or decode classification. Invalid/unsupported/unreadable primary and previous map to exact role-specific reasons; missing/current/input-recoverable candidates map to `None`. Intent does not execute deletion, quarantine, or hash naming.
- **Ownership and authority:** No file/path/IO/hash/time, source provenance, save execution, launch notification, input binding application, Unity, network, RNG, or callback dependency exists in the runtime file. M5D7B–M5D7D are read-only value/planner dependencies.
- **Allowlist:** Scoped implementation consists of the one selector runtime file and meta, one EditMode test file and meta, the approved contract, evidence/reports, and existing minimal documentation entry. No asmdef/package/project/scene/prefab/input asset change is required by the source.

## Focused test review

The focused suite has ten tests covering the ten AC groups. It exercises current-primary precedence against all previous/temp groups, metadata and binding repair, unusable-primary previous promotion, default cross-product, every temp classification, role-specific preservation, unavailable decode getters, all five decoded classifications, reflection/default/private-proof rejection, source byte/array defensive ownership, direct-current `long.MaxValue`, recovery overflow, deterministic repeat selection, and static authority restrictions.

Two non-blocking precision notes remain: the recovery max boundary is not repeated for every individual recovery classification, and AC-007's preservation matrix is compact rather than exhaustive for every winner/reason cross-product. The shared M5D7C planner paths and explicit selector branches are directly visible and covered by representative cases.

## Evidence integrity

I parsed the three final NUnit XML files and recomputed their SHA-256 values. Each is `Passed` with zero failed, skipped, or inconclusive tests, matching Terra's evidence.

| Suite | Total | Passed | Failed | Skipped | Inconclusive | SHA-256 |
|---|---:|---:|---:|---:|---:|---|
| Focused EditMode | 10 | 10 | 0 | 0 | 0 | `4E9E8D592ECDC01F0A6BBCE14E01A4D66B737529E7EF9C0DD1FFB3199B8965E9` |
| Full EditMode | 594 | 594 | 0 | 0 | 0 | `6E371257D6934FD813294DCAE550284F4351541AA9797AB4343E833CE695F390` |
| Full PlayMode | 576 | 576 | 0 | 0 | 0 | `CEB8B5E34D94CC8CBF982BAE96027A2EF3D1DFAC850B8669DE5B0B0143FF7E85` |

Workspace hashes match the evidence:

| Scoped file | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileLoadSelectorV1.cs` | `706B01B9FCE5299B2EE32E6BE3356AB1B04ACC3922C85FBC2C054FA89B12FCAB` |
| `Assets/AcadeGameMaker/Runtime/Profile/ProfileLoadSelectorV1.cs.meta` | `2F22B0E888BCB3233C0367ED8DA669B8B001B729DFCA5263C29E01C6C92CD388` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileLoadSelectorV1Tests.cs` | `1C84DB7296CCC9256289DC7BB4375A05A4FDC7005953DCF230F9BE41C24CF550` |
| `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileLoadSelectorV1Tests.cs.meta` | `461F25582A3A3637B8171744DA795991DCA7750CD40BA62FBA37BDFCA805CF86` |

All final runs used Unity `6000.6.0f1` after licensing/entitlement and competing-process preflights passed. No earlier diagnostic run changes this final assessment.

## AC status

| AC | Independent status | Basis |
|---|---|---|
| AC-M5D7E-001 | **PASS** | Current primary wins all previous/temp combinations and preserves source/result revision, including direct `long.MaxValue`. |
| AC-M5D7E-002 | **PASS** | Metadata/binding primary recovery delegates exact M5D7C input repair with current defaults and `r→r+1`. |
| AC-M5D7E-003 | **PASS** | Missing/invalid/unsupported/unreadable primary promotes current previous with exact previous-promotion semantics. |
| AC-M5D7E-004 | **PASS** | Metadata/binding previous recovery produces exact combined promotion/input-repair reason and increment. |
| AC-M5D7E-005 | **PASS** | Unselectable primary/previous produces exact M5D7C default `-1→0`; temp cannot change it. |
| AC-M5D7E-006 | **PASS** | Every existing temp is stale intent and never source; missing temp is the only `None`. |
| AC-M5D7E-007 | **PASS** | Role-specific invalid/unsupported/unreadable preservation reasons and winner independence are implemented and representative-tested. |
| AC-M5D7E-008 | **PASS** | Candidate/plan role, kind, getter, default/reflection, private-proof, document, revision, reason, and mutation invariants fail closed. |
| AC-M5D7E-009 | **PASS** | Revision `0`, positive, max-1, direct max, overflow/no-fallback, and deterministic repeat selection are covered. |
| AC-M5D7E-010 | **PASS** | Focused 10/10, full EditMode 594/594, and full PlayMode 576/576 have zero failed/skipped/inconclusive; no prohibited load execution claim is made. |

## P0/P1/P2 summary

- P0: none.
- P1: none. The prior pre-gate P1-001 is resolved by the amended contract and represented in the implementation/tests.
- P2: extend exhaustive preservation cross-products and repeat max-overflow cases for each recovery classification if desired; no acceptance blocker.

## Recommendation

Recommend Astra mark M5D7E **Verified**. The final implementation and evidence satisfy AC-M5D7E-001..010 within the engine-free selector boundary, with P0/P1 zero. Preserve the explicit separation: later owners perform file observation, quarantine/preservation, M5D7D save, notifications, and launch integration; M5D7E only returns the validated in-memory selection plan and preservation intent.
