# VD-03 M4B1 오르단 Unity Combat 브리지 구현 증적

- Date: 2026-09-05
- Contract: `docs/specs/work-contracts/2026-09-05-vd03-combat-m4b1-ordan-unity-combat-bridge.md`
- Requirements: `REQ-COM-001`, `REQ-COM-003`, `REQ-COM-004`, `REQ-COM-006`; affected `REQ-WT-001`, `REQ-WT-005`, `REQ-WT-006`
- Acceptance criteria: partial `AC-COM-002`, `AC-COM-003`, `AC-COM-004`; affected `AC-WT-005`
- Result: **PASS / Verified — Luna `P0=0`, `P1=0`, non-blocking `P2=1`; Sol integration approved**

## Implemented allowlist

- `Assets/AcadeGameMaker/Runtime/Combat/OrdanBossSession.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossCombatSimulationDriver.cs`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossSimulationDriver.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/OrdanBossSessionTests.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/OrdanBossCombatBridgeTests.cs`
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossCombatBridgePlayModeTests.cs`
- new Unity `.meta` files paired with the new runtime and test sources

The unit remains internal and is not added to a production scene. It adds no public ABI, asmdef reference, physics callback, wall-clock/RNG authority, room/reward/run mutation, asset, prefab, input action, package, or project-setting change.

## Behavioral evidence

- `AC-COM-002`: exact `-200 < -190 < -185` phase order is pinned; PlayMode manually advances the three owners in order, proves bootstrap promotion, mandatory empty-batch continuity, current-Combat death priority, and deterministic value traces without render-clock input.
- `AC-COM-003`: first Combat/M2A/M4A sessions remain provisional until bridge adoption. A missing current Transfer publication discards every first-use provisional session and publishes no latest value. A later invalid relevant edge preserves the prior M4A/queue/bridge publication while retaining the already completed current Combat publication.
- `AC-COM-004`: exact owner list, `ordan/player` roster and cross-wire rejection, receipt rejection before M1/M2A, immutable queue values, exact four-ID projection, and relevant modifier mismatch rejection are pinned. PlayMode drives the encounter through the Seizure payload and proves exact `t+1` singleton consumption, canonical external/basic/payload merge, aligned M1 result, and M4A vulnerability correlation.
- `AC-WT-005`: the M4A-owned next-tick forecast covers Seizure entry, Telegraph exclusive end and private payload cycle `0,1,2,0`; Unity forwards the value and contains no phase-duration or payload-ordinal reconstruction.

## Unity verification

Unity `6000.3.21f1` ran with a valid Unity Personal entitlement. The test runner was invoked without `-quit` so it could write NUnit XML before shutdown.

| Gate | Total | Passed | Failed | Skipped | Result | XML SHA-256 |
|---|---:|---:|---:|---:|---|---|
| M4A focused EditMode regression | 30 | 30 | 0 | 0 | Passed | `E7BFF7D0B1931A5F23C10D445DB3B8B4A8ADFA0DB50B2E83620D1DA1EDD95076` |
| M4B1 focused EditMode | 32 | 32 | 0 | 0 | Passed | `7BB95C280D6F48B8FA2E5D9D45F205BECE08A8860E46CCDD3A1A1DE2F6C70531` |
| M4B1 focused PlayMode | 9 | 9 | 0 | 0 | Passed | `C37D7103B0486A5EF1432C048BEF2ADDD167F039F77BA803B08066EE6EF1144B` |
| Full EditMode regression | 351 | 351 | 0 | 0 | Passed | `492674095C3C7D5A29947AAAD78559C0A270BCDCD4EF517158A994DF199024BD` |
| Full PlayMode regression | 297 | 297 | 0 | 0 | Passed | `2F1E4A8F82B8232778A60162391EC09084744411F805EACC8CBBDFDDED4DCEA5` |

Earlier superseded runs are retained as local execution history. The first focused Edit run failed one incorrect exact-exception assertion (`11/12`); the assertion was corrected. A later M4A regression exposed three forecast lookahead failures: the forecast executed a future transition and surfaced next-tick integer overflow one tick early. Sol narrowed it to exposure-only lookahead, preserving the verified M4A atomic boundary, and the final M4A/full ladders passed. A lifecycle-test draft was initially placed in the wrong test assembly and did not compile; it produced no XML, was moved to the existing internal-access PlayMode assembly without changing runtime visibility, and the final focused/full ladders passed.

## Ollama utilization record

- Kimi K3: **used and accepted in part**. Immutable DTO separation, preflight/commit staging and exact-tick queue ideas were retained. Invented health/direction/magnitude fields, float authority and external command ownership were rejected.
- GLM 5.2 pre-pass: **used and accepted in part**. Identity retention, incomplete first publication, tick drift, partial graph initialization, callback authority and death-handoff mutation risks were retained.
- GLM 5.2 post-pass: **used and accepted in part**. Missing receipt, premature publication, later bridge-failure preservation, stale forecast and phase-order checks were retained. Suggestions that assigned Transfer rollback/log behavior or a multi-owner payload lane were rejected as outside the frozen authority model.
- MiniMax M3: **failed and replaced**. It returned an empty final response without quota, authentication, network or model-availability error. Terra supplied the validation fixtures and Luna independently reviewed their coverage.
- Terra implemented the unit and expanded the graph, receipt and lifecycle fixtures. Sol screened the changes, corrected forecast overflow and first-use session adoption, and ran all Unity gates.

Luna's final independent review authorized `Verified`. Its sole non-blocking P2 notes that a separate adapter-specific queue/tick overflow test could make group 10 more explicit. Sol accepts the existing responsibility split: M4A's focused ladder pins forecast/ordinal overflow and repeated ticks, the boss bridge constructor bounds the valid start horizon, and M4B1 directly pins wrong-horizon and duplicate queue rejection. This P2 does not change runtime behavior or block the staged bridge.

No cloud model received repository contents, local paths, credentials, personal data, secrets, or approval authority. No Ollama output was adopted without GPT screening.

## Workspace safety

The pre-existing user-owned `ProjectSettings/URPProjectSettings.asset`, `ProjectSettings/SceneTemplateSettings.json`, and `Assets/Scenes/MovementSandbox.unity` were not staged, reverted, or edited for M4B1. Their SHA-256 values remained respectively `A3A626CB529CCFC0A82E388B8CD32BC60888F7226C33412FAD3BC50ABA802CD7`, `BB9098B3BFCDE78D93E264B96F7B77B5430E64E8C57A7AAD7F5A5D9C3945E16A`, and `1EF6378C459ACF3364E1F2D4AEA96D43C6E8A99942530B21030C8AF734BDE9A8`. Test XML and Unity logs are execution artifacts and are not part of the implementation allowlist.
