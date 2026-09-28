# VD-03 M4B2 오르단 scripted Transfer 노출 구현 증적

- Date: 2026-09-06
- Contract: `docs/specs/work-contracts/2026-09-06-vd03-combat-m4b2-ordan-transfer-exposure.md`
- Requirements: `REQ-COM-001`, `REQ-COM-003`, `REQ-COM-004`, `REQ-COM-006`; affected `REQ-WT-001`, `REQ-WT-002`, `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-006`, `REQ-WT-007`
- Acceptance criteria: partial `AC-COM-002`, `AC-COM-003`, `AC-COM-004`; affected `AC-WT-003`, `AC-WT-005`
- Result: **PASS / Verified — Luna `P0=0`, `P1=0`, non-blocking evidence/gate-hygiene `P2=2`; both P2 closed by this record and final-XML selection**

## Implemented allowlist

- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossSimulationDriver.cs` — internal bound-Transfer read seam
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossPayloadTransferModifierSink.cs` and `.meta`
- `Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTransferExposureScheduler.cs` and `.meta`
- `Assets/AcadeGameMaker/Runtime/Transfer/TransferContracts.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/TransferSession.cs`
- `Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferSimulationDriver.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/Transfer/TransferM1Tests.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/OrdanBossTransferExposureTests.cs` and `.meta`
- `Assets/AcadeGameMaker/Tests/PlayMode/TransferUnity/TransferSimulationDriverPlayModeTests.cs`
- `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/OrdanBossTransferExposurePlayModeTests.cs` and `.meta`
- the approved M4B2 contract, pre-gate, this evidence, `docs/README.md`, the narrow VD-02 clarification, and the SYSTEM-CONTRACTS amendment

No public ABI, asmdef, production scene/prefab, package, input action, art/audio asset or project setting changed. The unit adds no physics synchronization/query owner, render time, wall clock, coroutine or RNG authority.

## Behavioral evidence

- `AC-COM-002`: the exact `-200 Transfer → -190 boss Combat → -185 bridge → -180 exposure scheduler` order is pinned. The real four-owner PlayMode rig proves that an M4A forecast exposes exactly one payload before the next Transfer phase, Telegraph ages `0..59` retain exactly 60 exposure ticks, Execute hides all payloads, and 30/60/144 render grouping produces identical fixed-tick traces.
- `AC-COM-003`: the scheduler accepts only an exact current bridge publication with a `t+1` forecast, rejects repeated source ticks without mutation, emits immutable temporary/permanent ID lists, and distinguishes an ordinary empty forecast from the exact three-handoff defeated terminal. Terminal cleanup contains no temporary end, permanently removes all three payload IDs at the next Transfer phase, never rolls back the already completed same-tick Lightweight result, and rejects later scheduler advances.
- `AC-COM-004`: startup validates the exact shared driver, three ordinal target/sink/collider pairs, required base/modifier profiles, registry membership and the absence of additional `BossWeight-*` IDs. All three targets start hidden/disabled, and the scheduler reconstructs no phase, duration, slot, position or ordinal.
- `AC-WT-003`: a hidden registered payload is explicitly unavailable to selection. Exposure-end merge is restricted to one private owner and exact registered `BossWeight-0/1/2` IDs; forged owner, unregistered ID, current-tick request and repeated merge data reject or deduplicate before gameplay mutation.
- `AC-WT-005`: temporary exposure end clears active Transfer, highlight, player Lightweight state and sink handle exactly once while retaining registration. The same `BossWeight-0` is later selected and applied again in both engine-free and adapter-level tests. Same-ID temporary plus permanent removal clears once then unregisters; lifecycle remains authoritative. Merge order preserves camera, aim, press, lifecycle, permanent removals and temporary ends in both directions with defensive copies.

## Unity verification

Unity `6000.3.21f1` ran with a valid Unity Personal entitlement. The runner omitted `-quit` so NUnit XML was written before shutdown.

| Gate | Total | Passed | Failed | Skipped | Result | XML SHA-256 |
|---|---:|---:|---:|---:|---|---|
| M4B2 focused EditMode | 22 | 22 | 0 | 0 | Passed | `0A48D98272E5CCB160C025B96BACDD9E1DBD3DDC9C34A4A0058633C053DCDFD0` |
| M4B2 focused PlayMode | 17 | 17 | 0 | 0 | Passed | `58DF145B595C2310A609C042F0C64DFF6ED26BE3C76A55ED8FEABC6F1E607A5C` |
| M4A/M4B1 focused EditMode regression | 62 | 62 | 0 | 0 | Passed | `9F712D7B5C53153ED42599C61A511706471CBB1206081845B82DDFC30FD4055A` |
| M4B1 focused PlayMode regression | 9 | 9 | 0 | 0 | Passed | `D0F6096B994701DE81FFC89C7073FB7DAD6F67E6D2BAA42194EF79D8A787B119` |
| Full EditMode regression | 356 | 356 | 0 | 0 | Passed | `4D1139939A775486F231086B301D4041FFC7713C473933490309365A4627BEB5` |
| Full PlayMode regression | 305 | 305 | 0 | 0 | Passed | `EA5DBFD87B2ACE931A9F72A913284CAB682327E5359B9AD3D11C82164C1485A4` |

Earlier M4B2 XML/log files are retained only as local execution history and are not evidence. A first expanded death test incorrectly attempted to defeat Ordan before M4A had accepted its required full-health bootstrap. A later hidden-target test incorrectly supplied an aim at tick 0 before a completed tick `-1` player snapshot existed. Both were test-harness premise errors: the death test now bootstraps normally and proves same-tick Transfer-before-death behavior at the first payload window; the hidden-target test now performs tick 0 idle capture followed by a valid tick 1 aim. The superseded `TestResults-M4B2-Focused-Play-Verified.xml` result (`16/17`) is expressly excluded; `TestResults-M4B2-Focused-Play-Verified2.xml` (`17/17`) is the final focused PlayMode evidence.

## Ollama utilization record

- Kimi K3: **used and accepted in part**. Persistent registration, preflight/commit ordering and append-without-replacement guidance were retained. Invented exposure duration, mutable boolean-array state, sink-index identity, forecast generation outside M4A and public models were rejected.
- GLM 5.2 pre-pass: **used and accepted in part**. Queued-input overwrite, accidental unregister, duplicate application, lifecycle suppression, premature removal and cyclic reuse risks were retained. Dynamic re-registration and ambiguous hidden-target deferral were rejected.
- GLM 5.2 post-pass: **used and accepted in part**. Its same-ID stale-state and repeated scheduler concerns caused explicit adapter-level reapplication and repeated-source no-mutation tests. Simultaneous scheduler temporary/permanent output, multiple owner-token writers and frame-drop phase overlap were rejected because the frozen scheduler emits mutually exclusive terminal/temporary paths, registers exactly one owner and consumes fixed-phase publications rather than render frames. Engine-free same-ID temporary-plus-permanent processing remains directly covered.
- MiniMax M3: **used and accepted in part**. Baseline, first exposure, same-ID idempotence, change, cleanup, lifecycle preservation and cyclic-reuse fixture categories were retained. Generic expiry/magnitude fields and new runtime authority were rejected.
- Terra implemented the bounded subsystem and test fixtures. Sol screened and integrated the changes, corrected two test-harness assumptions, closed GLM/Luna gaps, and ran every Unity gate. Luna independently reviewed the final diff and results.

No cloud model received repository contents, local paths, credentials, personal data, secrets or approval authority. No Ollama output was adopted without GPT screening.

## Independent review and workspace safety

Luna's final independent verdict is `P0=0`, `P1=0`, `P2=2`, with both P2 limited to producing this evidence and selecting the final successful XML instead of a superseded failed harness run. This document closes both notes; Sol authorizes `Verified`.

The pre-existing user-owned `ProjectSettings/URPProjectSettings.asset`, `ProjectSettings/SceneTemplateSettings.json`, and `Assets/Scenes/MovementSandbox.unity` were not staged, reverted or edited for M4B2. Their SHA-256 values remained respectively `A3A626CB529CCFC0A82E388B8CD32BC60888F7226C33412FAD3BC50ABA802CD7`, `BB9098B3BFCDE78D93E264B96F7B77B5430E64E8C57A7AAD7F5A5D9C3945E16A`, and `1EF6378C459ACF3364E1F2D4AEA96D43C6E8A99942530B21030C8AF734BDE9A8`. Test XML and Unity logs are execution artifacts and are excluded from the implementation allowlist.
