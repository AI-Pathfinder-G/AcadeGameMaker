# VD-03 M3E1A Collision-Exclusion Feasibility Evidence

- Date: 2026-09-01
- Contract: `docs/specs/work-contracts/2026-09-01-vd03-combat-m3e1-authored-encounter-composition.md`
- Requirements exercised: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-MOV-001`, `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Acceptance criteria traced, not closed: `AC-COM-001`, `AC-COM-003`; affected `AC-MOV-001`, `AC-MOV-004`, `AC-MOV-005`
- Implementer: Terra
- Independent verifier: Luna
- Result: **FAIL / STOP — P0=0, P1=1, P2=0**

## Executable result

The focused Unity 6000.3.21f1 PlayMode run executed 6 tests: 3 passed, 3 failed, 0 skipped, duration `0.0885747s`. XML SHA-256 is `FE5D1628C56D5FA28B31FA4BFEF9B07B86AEC8BAE97743BE80F0219149D6EB1B`.

The exact probe source is `Assets/AcadeGameMaker/Tests/PlayMode/CombatUnity/RegularEnemyCollisionExclusionFeasibilityPlayModeTests.cs` with adjacent `.meta`. The run wrote workspace-local `TestResults-M3E1A.xml` and `Unity-M3E1A.log`. Its exact test filter was `AcadeGameMaker.Tests.PlayMode.CombatUnity.RegularEnemyCollisionExclusionFeasibilityPlayModeTests`, invoked as:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.21f1\Editor\Unity.exe' -batchmode -nographics -projectPath 'C:\Users\me\Documents\GPT-workspace\AcadeGameMaker' -runTests -testPlatform PlayMode -testFilter 'AcadeGameMaker.Tests.PlayMode.CombatUnity.RegularEnemyCollisionExclusionFeasibilityPlayModeTests' -testResults 'C:\Users\me\Documents\GPT-workspace\AcadeGameMaker\TestResults-M3E1A.xml' -logFile 'C:\Users\me\Documents\GPT-workspace\AcadeGameMaker\Unity-M3E1A.log'
```

The two role-specific failures proved that immediately after `Physics2D.IgnoreCollision(player, selected, true)` and `GetIgnoreCollision==true`, the production-identical attached-capsule Cast still returned the selected walker or surveyor stable ID. The environment case proved the real `PlayerMovementController` continued to select `walker` rather than `EnvironmentWall` after both exact player/enemy pair flags were true. Thus collision-gate items 2, 3 and 4 failed and M3E1's stop condition activated.

The fixture controlled initial non-overlap, both directions in separate fresh rigs, exact `NoFilter()` plus `useTriggers=false`, bounded nonzero distance, immediate pair-state reads and real Movement ticks. Luna found no setup, synchronization, direction, distance or obsolete-API explanation that could reverse the failure.

## Valid partial observations

- Gate 5 passed: ignored flags did not alter frozen collider geometry, and two actual M3C2/M3D2 source ticks revalidated and published successfully.
- Gate 6 passed: destroyed collider/root references became Unity-null and completely fresh references began with both pair flags false before the first Movement publication.
- Gate 8 passed only as determinism of the failing behavior: 30/60/144 traces were nonstatic and identical, but actors still blocked the player. It is not exclusion-success evidence.
- Gate 7 was correctly deferred to M3E1B0 and was not claimed.

## Disposition

No production `IgnoreCollision` adapter, M3E1 B0 scene/prefab or B1 handoff work may proceed. Sol reopens Movement ownership through `docs/specs/work-contracts/2026-09-01-vd01-movement-m2-ignore-pair-hit-filter-addendum.md`. The narrow proposed correction makes Movement skip raw Cast hits whose exact attached-player/hit pair reports `GetIgnoreCollision==true`; it requires a new Luna pre-gate before implementation.

The failing probe source SHA-256 is `A1162158114FC68174A0FDA59EC0FB41E3DE601DABBD85BE4192957C92C10D9F`; its `.meta` SHA-256 is `B608A86D34B05AA3A7D8C34D0530D68C84E860FBE1909CE9B0D732A860CED0FA`. It remains a failure probe and must not be integrated as a passing suite without the separately Approved Movement correction.

## Approved correction and retry

The historical FAIL/STOP above was not reclassified. Sol separately approved the VD-01 M2 ignored-pair raw-hit filter after Luna pre-gate PASS. After its implementation, the same M3E1A focused fixture passed 6/6 with 0 failed/skipped and XML SHA-256 `533A3BF37DA1CACADB2B257C15427194F865B046BD9E36791005B0E7140D8206`. Movement focused passed 15/15, CombatUnity passed 240/240, full EditMode passed 185/185 and full PlayMode passed 274/274. Luna independently returned PASS with `P0=0`, `P1=0`, `P2=0`.

Therefore M3E1A staged gates 1–6 and 8 are now PASS under the separately Approved correction, and M3E1B0 may begin. Gate 7 remains unclaimed until the collision-neutral committed authored graph is destroyed and freshly instantiated as required by the M3E1 contract.
