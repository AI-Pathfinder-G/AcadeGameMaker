# VD-03 M3D2 Threat Delivery and Unity Bridge — Implementation Evidence

- Date: 2026-09-01
- Contract: [VD-03 M3D2](../specs/work-contracts/2026-08-31-vd03-combat-m3d2-threat-delivery-unity-bridge.md)
- Status: PASS — Terra implementation, Sol executable regression, and Luna independent verification complete
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-MOV-001`, `REQ-WT-005`
- Partial acceptance evidence only: `AC-COM-001`, `AC-COM-003`; affected `AC-MOV-001`, `AC-WT-005`

## Implemented boundary

Combat now owns one optional, explicitly registered future threat lane. Registration is separate from preparation, preserves the legacy unregistered path, and accepts exactly one immutable M3D1 batch for source tick `t` and delivery tick `t+1`. Combat peeks, validates and atomically merges that batch with the existing attack request without adding a shadow attack-sequence authority. `BasicAttackSession` now exposes mutation-free preview and exact owner-bound commit while legacy `Process` retains identical behavior.

`RegularEnemyThreatSimulationDriver` runs at execution order `+100`. It validates the exact player/Combat/Behavior/Motion graph, freezes the literal player/walker/surveyor roster and player/walker collider geometry before lane registration, consumes only completed tick-`t` value publications, stages M3D1, preflights delivery, revalidates the transaction, commits the core and delivery, and publishes only an immutable carried threat view. Reset follows the same staged-owner transaction. No physics query, callback, body pose or additional transform sync determines threat contact.

Executable integration also exposed an upstream-domain mismatch in M3D1. The separately approved equal-X compatibility addendum now accepts walker direction `0` in `DashTelegraph` and `DashActive`, matching M3B/M3C's frozen `-1/0/1` domain without changing geometry-authoritative contact behavior.

## Acceptance trace

| Evidence | Requirement / acceptance trace |
|---|---|
| Explicit lane preparation/registration, pristine-first-source and signed-horizon matrix | `REQ-COM-004`; partial `AC-COM-003` |
| Immutable owner-bound preflight/commit, stale/forged/duplicate preservation and defensive copies | `REQ-COM-004`; partial `AC-COM-003` |
| Exact next-tick merge of attack, walker contact and Surveyor line requests | `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-005`; partial `AC-COM-001`, `AC-COM-003` |
| Complete malformed request ID/source/target/amount/kind/tick/order matrix | `REQ-COM-004`; partial `AC-COM-003` |
| Exact `-200 -> -190 -> -180 -> -170 -> -160 -> default -> +100` carried-publication boundary | `REQ-COM-001`, `REQ-MOV-001`, `REQ-WT-005`; affected `AC-MOV-001`, `AC-WT-005` |
| First-use, retry, graph crosswire, geometry/scalar mutation and transaction revalidation matrix | `REQ-COM-002`, `REQ-COM-004`; partial `AC-COM-003` |
| Paired reset transaction and player/walker/Surveyor death controls | `REQ-COM-004`, `REQ-COM-005`; partial `AC-COM-003` |
| 30/60/144 logical grouping, zero physics query/sync, and render-phase independence | `REQ-COM-004`; partial `AC-COM-003` |
| Equal-X telegraph-to-active acceptance, overlap/miss, and `±2` atomic rejection | `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`; partial `AC-COM-001`, `AC-COM-003` |

## Verification results

| Run | Result | Result artifact SHA-256 |
|---|---:|---|
| Focused M3D2 PlayMode final | 137/137 passed, 0 skipped, 0.194223 s | `abe73655fbb21aa0e8eff9f08e70c6966e5b1572cd816f9d9dec1d8890c0c41c` |
| Full project PlayMode final | 265/265 passed, 0 skipped, 0.3191144 s | `150434c660cd2def7a6aa53b75ebd83e40586d7ff8fb4e47cf5c065c1d0eb1ff` |
| Focused M3D1 Threat EditMode | 31/31 passed, 0 skipped | `e877db172698a2faf046d7fa5070ee089aab048cf113deb7d825576acb79eb62` |
| Focused BasicAttack EditMode | 17/17 passed, 0 skipped | `f9b005c7c24d87b3162d49f55c1651675bf719bacfd8f685e6e0dad5d4d54de3` |
| Full project EditMode final | 185/185 passed, 0 skipped | `f9a167d7ffa74cd6b3a511f9330a6a7d7f664df7f5fea7910dae5ca070173650` |

Direct Roslyn compilation passed for all four affected assemblies: Combat, Combat.Unity, Combat.EditMode.Tests and Combat.Unity.PlayMode.Tests. `git diff --check` passed. Static scans found no new public surface, runtime discovery, physics query, additional `Physics2D.SyncTransforms`, wall-clock or random dependency. Unity-boundary tests explicitly distinguish values Unity refuses or sanitizes from malformed states the threat driver can actually observe.

## Review and correction record

The first executable focused run reported 122/138. Six failures exposed the M3D1 equal-X consumer mismatch; the remaining failures were invalid recovery setup, a nondeterministic crossing fixture, stale transform setup, or attempts to author non-finite Unity values that the engine normalizes before the driver can observe them. Sol approved the narrow M3D1 compatibility addendum after Luna's pre-gate PASS. Terra corrected only the authorized consumer checks and executable oracles, preserving runtime guards.

The corrected M3D2 suite reached 137/137. A later full EditMode run found one BasicAttack test whose `Press(sequenceId, aimSampleId, tick)` arguments did not actually repeat the sequence ID; the fixture was corrected and the full suite reached 185/185. Luna's final independent implementation review returned **PASS — P0=0, P1=0, P2=0** after one unused non-finite test enum/injection branch was removed.

## Ollama utilization ledger

| Lane | Outcome | GPT screening disposition |
|---|---|---|
| GLM 5.2 | used and accepted in part | Retained adversarial registration, queue, reset, death, malformed request and journey-gap cases. Rejected any change to GPT-owned runtime authority or contract semantics. |
| Kimi K3 | used and accepted in part | Retained bounded implementation ideas for immutable delivery, exact preflight/commit and isolated Unity fixtures after Terra review and rewrite. No repository text, path or secret was supplied. |
| MiniMax M3 | failed and replaced | Returned an empty bounded non-thinking response without quota/rate evidence. Terra supplied the validation fixtures and Luna independently reviewed them; no empty output was adopted. |

No Ollama model received repository contents, local paths, credentials, personal data or secrets. Ollama output had no decision, approval, merge or canon authority.

## Scope and preservation

Implementation changes are limited to the M2A preview/commit seam, Combat's future threat lane, exact cross-driver graph seams, the new `+100` threat bridge and their EditMode/PlayMode tests. M3D1 changes are limited to the approved equal-X compatibility checks and tests. No scene, prefab, assembly definition, project setting, package or public API was added or changed.

Pre-existing user-owned changes in `Assets/Scenes/MovementSandbox.unity`, `ProjectSettings/URPProjectSettings.asset`, and `ProjectSettings/SceneTemplateSettings.json` were neither staged nor reverted.
