# VD09 M5D7-QA TMP label-layout PlayMode fixture — Luna static recheck

Date: 2026-09-27 (Asia/Seoul)
Reviewer: Luna (independent static review)
Scope: Narrow fixture-only recheck after the focused PlayMode failure. Unity was not run and no authored asset was modified.

## Exact input

| Item | SHA-256 |
|---|---|
| Current `Assets/AcadeGameMaker/Tests/PlayMode/HubPresentation/HubMenuResolutionPlayModeTests.cs` | `903E570AC1F01AD9B5B038BF083ABE0E5D9F637E81F65741D3CBE74288E8F1E2` |
| Prior fixture SHA | `9BA028A069D656A36594E12F2D33FB3C0A18D368528BB59AE0EADE4EF893F66C` |

## Narrow delta verification

The prefab-side branch now loads the same `HubMenuRoot.prefab`, creates an active runtime clone with `UnityEngine.Object.Instantiate`, forces canvas layout with `Canvas.ForceUpdateCanvases()`, executes the existing `AssertLabels` predicate, and always destroys the clone in `finally`. This replaces direct TMP geometry inspection on the prefab asset with an active instantiated object, so the assertion observes runtime-resolved canvas/TMP state without changing authored assets.

The scene-side branch remains the same additive load and exact prefab-source identity assertion. It now also forces canvases before the unchanged `AssertLabels(root)` call and still closes the scene in `finally`. Scene identity and cleanup boundaries are retained.

`AssertLabels` is semantically unchanged: the four Korean labels, anchored position `(12,5)`, size `(168,22)`, centered local geometry `(96,16)`, zero margin, no overflow, `firstOverflowCharacterIndex == -1`, exact character count, and every visible TMP mesh vertex inside the local rect remain strict assertions. No product behavior, layout values, scene topology, or contract IDs changed.

## Verdict

**PASS — P0=0, P1=0, P2=0.**

The fixture correction is narrowly scoped to active-clone/canvas evaluation and deterministic cleanup. It may proceed to the independent PlayMode execution gate; this record does not claim that execution passed.

