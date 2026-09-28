# M5D7Q-A resolution-capture implementation pre-gate R4

Date: 2026-09-27  
Reviewer: Luna (independent static recheck)  
Scope: retry-c compiler-compatibility delta only; no Unity launch, capture execution, or implementation edit.

## Identity and delta proof

- Current generator SHA-256: `5B26C2D99BEC3C4A6984848532271AC60E098DEBB1F7B9E6516930A3E08940AE`
- Prior R3 generator SHA-256: `823D3E8562E32E3E80ADD7DB085B62FD8399F1B73BBA87D8C36B9C934B57DEDA`
- Removing `using UnityEngine.TextCore.LowLevel;` and replacing
  `SceneManager.GetSceneAt(i).isDirty` with the prior
  `EditorSceneManager.IsSceneDirty(SceneManager.GetSceneAt(i))` from the current
  bytes reproduces the prior R3 SHA exactly. This proves the retry-c delta is
  limited to the two reported compiler fixes.

## Recheck

- `using UnityEngine.TextCore.LowLevel;` now resolves `GlyphRenderMode.SDFAA`.
- `RequireCleanScenes` now uses the Unity 6000.6-supported `Scene.isDirty` property.
- No R3 publish/rollback, cleanup, target geometry, empty-setup restoration,
  dependency fingerprint, authoring/TMP assertion, manifest, GUID, or oracle
  logic changed.
- Prior R3 PASS therefore remains valid for the exact current implementation.

## Verdict

**PASS — P0=0, P1=0, P2=0.**

This is a static compatibility recheck only. Unity retry-c execution and the
required same/fresh capture evidence remain separate post-review gates.

