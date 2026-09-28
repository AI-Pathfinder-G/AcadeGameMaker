# VD-03 M3E1B1 Lifecycle and Presentation Handoff Evidence

- Date: 2026-09-05
- Contract: `docs/specs/work-contracts/2026-09-01-vd03-combat-m3e1-authored-encounter-composition.md`
- Stage: M3E1B1 lifecycle and presentation handoff integration
- Status: **PASS** — Luna `P0=0`, `P1=0`; Sol integration approved
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`
- Intended partial acceptance evidence: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-003`, `AC-WT-005`

## Implemented boundary

The uncommitted B1 change adds an engine-free encounter lifecycle session, immutable defeat/reward/clear/completion request values, one `[DefaultExecutionOrder(110)]` Unity handoff adapter, narrow production-named upstream read/graph-validation seams, explicit B1 authoring and a final one-adapter encounter prefab. It does not grant rewards, mutate inventory or room state, transition scenes, add production input, or claim final UI/art.

The lifecycle owner validates exact ticks, literal `player`, `surveyor`, `walker` role order, health conservation, alive-to-dead proof, no resurrection, pre-death-only reset, one-shot regular-enemy handoffs and immutable caller-owned collections. `CommitNext` recomputes the candidate from the exact input before adopting state.

The adapter captures and revalidates Player, Transfer, Combat, Reaction, Behavior, Locomotion and Threat publications; validates exact Transfer/Combat object identity and retained collider/body geometry; checks both exact collision pairs before commit; applies `surveyor` then `walker`; verifies each pair immediately; and fail-stops after a post-core pair failure. First-use source, graph, geometry or pair rejection leaves no lifecycle owner, retained geometry, pair latch or presentation view.

## Verified behavior

Licensed Unity `6000.3.21f1` executed the following focused coverage:

- engine-free lifecycle EditMode cases for sequential and simultaneous deaths, player death, duplicate and malformed traces, conservation, reset, tick boundaries, forged/stale previews and caller-mutation resistance;
- B1 authoring EditMode validation and missing/duplicate/cross-wired/foreign handoff mutations;
- final-graph PlayMode cases for neutral boot, 60 consecutive authored ticks, sequential/simultaneous deaths, `t+1` continuation, player death, pre/post-death reset, external pair drift, pristine first-use rejection, missing bindings and immutable presentation;
- authored Surveyor `Relocate(48) -> FireTelegraph(30) -> Fire` evidence at tick 78, exact denial-line center/end projection, next-tick `EnemyRanged` delivery, persistence after Surveyor death and exclusive-end expiry;
- repeated final B1 fresh-instance Gate 7 coverage.

Walker-contact authority remains in the verified M3D fixture `RegularEnemyThreatSimulationDriverPlayModeTests.AC_COM_001_StationaryThenDashCrossingContactDeliversOnlyNextTick`. The B1 authored test distinguishes its observed `EnemyContact` request from Surveyor Fire/denial-line presentation without adding request-kind fields to the B1 view.

## Review and correction history

Terra implemented the unit and two static correction passes. Luna's pre-execution static review reported `P0=0`; its only remaining `P1` was absent executable evidence. Licensed execution exposed two test-harness defects, not production defects: an ineffective duplicate-adapter mutation caused by `[DisallowMultipleComponent]`, and unconsumed expected downstream reset exceptions after the required post-death reset rejection. The mutation now adds the duplicate at the root, and the PlayMode test explicitly expects the complete deterministic rejection cascade.

Luna's final correction review reported `P0=0`, `P1=0`, `P2=2` and recommended integration PASS. Sol closed the documentation-count P2 in this update; the earlier strict Transfer-publication P2 was closed by requiring recursive comparison of nullable `RemovalResult` and `PressResult`, including attempt, nested session, state-change and clear fields. The added mutation and static enumerator tests pass in the final suites. One residual non-blocking `P2` remains: the authored contact assertion observes an `EnemyContact` request but relies on the already verified M3D fixture for exact contact-transition authority. That authority remains covered by `RegularEnemyThreatSimulationDriverPlayModeTests.AC_COM_001_StationaryThenDashCrossingContactDeliversOnlyNextTick` and the final full PlayMode regression.

`git diff --check` passes. Static scans found no new public runtime ABI, runtime discovery, added physics query/sync, wall-clock, delta-time or random authority in the B1 runtime source.

## Current source hashes

| Artifact | SHA-256 |
|---|---|
| lifecycle source | `D2FFCFC1174CDB5E75E7A12A674175A0FFC1190345144D9168106A0455210095` |
| `+110` adapter source | `0B62C1C6FBA06FD84213235DFF02B40B2831EAF8A774C4D0170E24860581867D` |
| lifecycle EditMode tests | `2C6C94C8CB936CDBFA91FF9E825F707DB190DBA88171542C26A4B74E628C7BB4` |
| adapter PlayMode tests | `48407AFFCB5CA1ECE3281EA26359B66DF89E047A6D9240036C700934AE026A29` |
| B1 authoring EditMode tests | `BA8162CD012E76755CF0CBB8D4A25F1B16C3196541DDBC22F56CB999435FCDA1` |
| final B1 graph prefab | `4550E60216C9B65D913B4360E8B8DB2FAC0CB9B47424FE3D5FD9A7E41F50BDAC` |
| encounter scene | `5ACB2995F9A2E5092EF1BF7FA42922FB1B2C0F778F84D5B3B0B5F7DB98751D5B` |

## Executable verification

Unity Personal activation restored licensed batch execution. Compilation/import and explicit `RegularEnemyEncounterAuthoringBuilder.BuildB1FromMenu` regeneration completed successfully before the final suites. The normalized final prefab was then reloaded and revalidated by the final 96-case authoring run.

| Scope | AC evidence | Total | Passed | Failed | Skipped | XML SHA-256 |
|---|---|---:|---:|---:|---:|---|
| lifecycle EditMode | `AC-COM-003`; affected `AC-WT-005` | 7 | 7 | 0 | 0 | `9F5E0B2499DA0C3DDADE228DD507F4907E7D409370054F801F3203E549B8DE4D` |
| B1 authoring EditMode, final normalized asset | partial `AC-COM-001`, `AC-COM-003`; affected `AC-WT-003` | 96 | 96 | 0 | 0 | `A9A6C821F877B5711C3743012731A802B0B06BE48436221F48A189682AFA1C2E` |
| handoff PlayMode | partial `AC-COM-001`, `AC-COM-003`; affected `AC-WT-003`, `AC-WT-005` | 13 | 13 | 0 | 0 | `AE3581C4AF368ED8A2A7A2790517F53356FB71206A07A64B107C84DF3D81B9E0` |
| repeated final-B1 Gate 7 PlayMode | partial `AC-COM-001`; affected `AC-WT-003` | 1 | 1 | 0 | 0 | `194419CEAE0B208BAF1F4B42A60FC47BEB5E3BF7EC57178E489CE023FFF40F23` |
| CombatUnity EditMode regression | `AC-COM-001`, `AC-COM-003` regression | 104 | 104 | 0 | 0 | `A788FA664EEE60FE14FAA153604D91771C51A6432B0F45F013CBB739A2FB3460` |
| CombatUnity PlayMode regression | `AC-COM-001`, `AC-COM-003` regression | 254 | 254 | 0 | 0 | `8EBB0CFF874FBAC35450CC4BE76480355C5FE39BB26198CA9AC03335D8761FA8` |
| full EditMode regression | affected `AC-WT-003`, `AC-WT-005` | 289 | 289 | 0 | 0 | `551753FE6C00DCC47C852A11ED8296C2BBD7F88A7C7EE2B0C513ED44E45FE88E` |
| full PlayMode regression | affected `AC-WT-003`, `AC-WT-005` | 288 | 288 | 0 | 0 | `F1CFCA3866B19D93288FF39C5F86D0E6783C3ECF7B494E80A4CFBE4DAD60C818` |

All eight XML artifacts report `Passed`, with zero failure and zero skip. Final `git diff --check` passes. No full acceptance criterion is closed by this partial authored-composition milestone; VD-07 human-controlled completion and VD-04 reward/room consumption remain outside M3E1.

## Ollama utilization record

- **Kimi K3 — used and accepted in part:** bounded invariant, atomic preview and isolated implementation suggestions informed the Terra implementation. Session sealing after clear, incorrect reward aggregation and invented abstractions were rejected.
- **GLM 5.2 — used and accepted in part:** source/pair/geometry drift, simultaneous-death, first-frame and cadence mutations informed negative coverage. Incorrect HP, reset and player-ownership assumptions were rejected.
- **MiniMax M3 — failed and replaced:** the bounded validation-tool request returned an empty final response after its output budget without quota, authentication, network or model-availability error. Terra authored the fixtures and Luna reviewed their source.

No cloud model received repository content, local paths, credentials, personal data, secrets or decision authority.

## Protected workspace state

The B1 allowlist excludes the user-owned paths below. Their hashes remain unchanged through implementation and static review:

| Path | Status | SHA-256 |
|---|---|---|
| `Assets/Scenes/MovementSandbox.unity` | committed user-approved baseline | `1EF6378C459ACF3364E1F2D4AEA96D43C6E8A99942530B21030C8AF734BDE9A8` |
| `ProjectSettings/URPProjectSettings.asset` | pre-existing `M`, excluded | `A3A626CB529CCFC0A82E388B8CD32BC60888F7226C33412FAD3BC50ABA802CD7` |
| `ProjectSettings/SceneTemplateSettings.json` | pre-existing `??`, excluded | `BB9098B3BFCDE78D93E264B96F7B77B5430E64E8C57A7AAD7F5A5D9C3945E16A` |
