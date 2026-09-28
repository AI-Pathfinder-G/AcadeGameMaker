# VD-09 M5D7E profile load selection core — Terra implementation evidence

- Status: **Verified — Astra accepted after Luna PASS (P0=0, P1=0)**
- Contract: [M5D7E profile load selection core](../specs/work-contracts/2026-09-13-vd09-m5d7e-profile-load-selection-core.md)
- Dependencies: M5D7A–M5D7D Verified
- Independent review: [Luna M5D7E implementation review](2026-09-13-vd09-m5d7e-luna-independent-review.md)

## REQ / AC mapping

| Requirement / acceptance | Implementation and focused coverage |
|---|---|
| REQ-M5D7E-001 / AC-M5D7E-001,006,009 | Immutable observed candidates have exact roles/kinds. The selector chooses current primary without increment regardless of prior/temp revision, including `long.MaxValue`; temp is never a source. |
| REQ-M5D7E-002 / AC-M5D7E-002..005,009 | Primary input recovery delegates only to M5D7C input repair; selectable previous delegates only to previous promotion; otherwise the M5D7C approved default is used. Recovery overflow propagates without fallback. |
| REQ-M5D7E-003 / AC-M5D7E-005..007 | Role-specific preservation intents are derived from observed candidate kind/classification. Every existing temp is `StaleTemp`; no preservation action is executed. |
| REQ-M5D7E-004 / AC-M5D7E-008,009 | Candidate and plan getters revalidate closed role/kind/classification/result/proof relations. Private candidate/recovery/direct-document proofs are recomputed against M5D7B/M5D7C and reflection/default/mismatch cases fail closed. |
| REQ-M5D7E-005 / AC-M5D7E-010 | New engine-free source holds no file/path/byte/hash/time/quarantine/save/log/notification/input-apply/Unity/network/RNG/callback authority. |

## Focused matrix

`ProfileLoadSelectorV1Tests` covers AC-M5D7E-001 through AC-M5D7E-010: direct-primary precedence; metadata/binding repairs; primary failure and previous promotion combinations; default cross-product; temp irrelevance; preservation roles; unavailable decoded getters; reflection/default/proof rejection; byte defensive ownership; revision boundaries; determinism; and static authority review.

## Scoped file SHA-256

| File | SHA-256 |
|---|---|
| `ProfileLoadSelectorV1.cs` | `706B01B9FCE5299B2EE32E6BE3356AB1B04ACC3922C85FBC2C054FA89B12FCAB` |
| `ProfileLoadSelectorV1.cs.meta` | `2F22B0E888BCB3233C0367ED8DA669B8B001B729DFCA5263C29E01C6C92CD388` |
| `ProfileLoadSelectorV1Tests.cs` | `1C84DB7296CCC9256289DC7BB4375A05A4FDC7005953DCF230F9BE41C24CF550` |
| `ProfileLoadSelectorV1Tests.cs.meta` | `461F25582A3A3637B8171744DA795991DCA7750CD40BA62FBA37BDFCA805CF86` |

## Root execution

All final runs used Unity `6000.6.0f1` after editor/licensing/entitlement and competing-process preflights passed.

| Run | Result | NUnit XML | SHA-256 |
|---|---:|---|---|
| Focused EditMode `ProfileLoadSelectorV1Tests` | 10 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7e-20260913/focused-editmode.xml` | `4E9E8D592ECDC01F0A6BBCE14E01A4D66B737529E7EF9C0DD1FFB3199B8965E9` |
| Full EditMode | 594 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7e-20260913/full-editmode.xml` | `6E371257D6934FD813294DCAE550284F4351541AA9797AB4343E833CE695F390` |
| Full PlayMode | 576 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7e-20260913/full-playmode.xml` | `CEB8B5E34D94CC8CBF982BAE96027A2EF3D1DFAC850B8669DE5B0B0143FF7E85` |

This artifact makes no claim about actual file loading, quarantine, atomic-save execution, notification, input application, or launch entry.
