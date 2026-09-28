# VD09 M5D7-QA actual-scene PlayMode rewrite — Luna static pre-gate

Date: 2026-09-27 (Asia/Seoul)
Reviewer: Luna (independent static review)
Scope: Review of the real-scene PlayMode fixture rewrite only. Unity was not run and no scene, build setting, or production asset was modified.

## Exact input

| Item | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Tests/PlayMode/HubPresentation/HubMenuResolutionPlayModeTests.cs` | `22FCB024EC59E60CEBAD176685EF12ABD06920DA525FBD43ADB2C78F3EC2690A` |

## Static verification

- The prior Editor-only `EditorSceneManager.OpenScene` path is removed from the actual-scene test. The test is now a `[UnityTest] IEnumerator` and uses Unity 6000.6-compatible `EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/Scenes/Hub.unity", new LoadSceneParameters(LoadSceneMode.Additive))`, then yields the returned `AsyncOperation`.
- It resolves the loaded scene through `SceneManager.GetSceneByPath` and rejects an invalid or unloaded result. It retains the exact `HubMenuRoot` lookup and `PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot` identity assertion.
- The prefab branch still uses an active clone, forces canvases, runs the strict label predicate, and destroys the clone in `finally`.
- The actual-scene assertion is wrapped so NUnit/assertion failures are captured, the loaded scene is always passed to `SceneManager.UnloadSceneAsync`, the unload operation is yielded, and the original assertion exception is rethrown afterward. This preserves failure semantics while guaranteeing scene cleanup for the loaded-scene assertion path.
- `AssertLabels` is unchanged: Korean copy, position/size/centering, zero margin, no TMP overflow, `firstOverflowCharacterIndex == -1`, exact character count, and visible mesh vertices inside the local rect remain strict.
- No build-settings edit, scene save, production code, scene topology, or product behavior is introduced. The test remains scoped to REQ-M5D7QA-006/009 and AC-M5D7QA-006/010.

## Verdict

**PASS — P0=0, P1=0, P2=0.**

The rewrite is a bounded test-fixture correction that removes the forbidden Editor-only scene-open path without weakening the assertions or cleanup boundary. It may proceed to the independent PlayMode execution gate; this record does not claim execution success.

