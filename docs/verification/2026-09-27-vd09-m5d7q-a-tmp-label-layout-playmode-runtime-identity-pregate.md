# VD09 M5D7-QA PlayMode runtime identity correction — Luna static pre-gate

Date: 2026-09-27 (Asia/Seoul)
Reviewer: Luna (independent static review)
Scope: Narrow PlayMode fixture identity correction only. Unity was not run and no production or authored asset was modified.

## Exact inputs

| Item | SHA-256 |
|---|---|
| Current `HubMenuResolutionPlayModeTests.cs` | `6A80228FDEF42941919B4DD0E4BD601EDEDF4A15FFC94BF8FB5EC758618E1966` |
| Prior real-scene rewrite | `22FCB024EC59E60CEBAD176685EF12ABD06920DA525FBD43ADB2C78F3EC2690A` |
| EditMode provenance validator | `477420AF499066D311ADB008AF889D9A90691F2E162045F001326439CCC878CA` |
| EditMode provenance/matrix tests | `AF15506C9AAC4C4C34F3F55AB5AA78315873A6FF05CC6181DE140DFBE2BEE3DB` |

## Runtime identity assertions

The PlayMode branch now validates the loaded scene itself rather than relying on a runtime `PrefabUtility` linkage result that was empty in the prior fixture:

- exact `scene.path == "Assets/Scenes/Hub.unity"`;
- exactly two root objects;
- exactly one active `HubRuntimeRoot` and exactly one active `HubMenuRoot` by unique name, with all roots `activeSelf`;
- `HubMenuRoot` has `Canvas`, `HubSafeFrameScalerV1`, and `HubMenuPresenterV1`, and its hierarchy contains exactly one `EventSystem`;
- no `Camera` exists below either root;
- no null `MonoBehaviour` entry exists, rejecting missing scripts;
- existing active-clone Canvas/TMP checks and all strict label assertions remain unchanged.

The old runtime `PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot` assertion was intentionally removed from this PlayMode branch because it did not provide a meaningful runtime identity result. Prefab provenance remains strict in the EditMode path: `ValidateSceneInstanceOverrideMatrix` checks both exact source-prefab paths, the 11-row Q0 semantic-zero matrix, the 23-row HubMenu matrix, and exact Q0 latch/router object references; the focused EditMode tests retain positive and negative matrix coverage.

Scene load/unload and assertion rethrow behavior from the prior real-scene rewrite are unchanged. No build-settings edit, scene save, production code, or product behavior is introduced.

## Verdict

**PASS — P0=0, P1=0, P2=0.**

The runtime identity correction removes an unusable linkage assertion and replaces it with direct scene/topology/component/missing-script checks, while the authoritative EditMode provenance checks remain intact. This is a static pre-gate only; PlayMode execution remains the next gate.

