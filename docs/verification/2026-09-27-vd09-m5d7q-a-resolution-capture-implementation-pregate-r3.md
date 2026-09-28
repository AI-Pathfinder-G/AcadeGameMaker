# M5D7Q-A resolution-capture implementation pre-gate R3

Date: 2026-09-27  
Reviewer: Luna (independent static review)  
Scope: static source/evidence review only; no Unity launch, capture execution, or implementation edit.

## Identity

- Reviewed generator: `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationCaptureGenerator.cs`
- Exact SHA-256: `823D3E8562E32E3E80ADD7DB085B62FD8399F1B73BBA87D8C36B9C934B57DEDA`
- Proposal SHA reviewed: `8DD84F02FEEADB67EE792511EDA00E298DA8A53B99349E4F80D2F81BC4708FFF`
- Contract bytes present in workspace: `A359B23776EB127295C4D52AF6ED25078C4C3A056377A70DB79E015B8E5E9EFC`

## R2 closure checks

- **P1-001 publish/rollback:** closed. `StagePublish` validates the staged final before returning, restores `.prev` on move/validation failure, and records `publishStaged`. Later GUID, final-set, restoration, or state failures roll back the staged final through `RollbackPublish`; `CommitPublish` is deferred until all restoration checks pass and also rolls back on failure.
- **P1-002 cleanup:** closed. Each frame detaches `camera.targetTexture`, restores the prior `RenderTexture.active` and `GL.sRGBWrite`, then destroys readback/RT. The outer finally detaches the camera and restores globals before destroying temporary objects.
- **P1-003 target geometry:** closed statically. `ConfigureRootForTarget` sets the in-memory root to the exact RT width/height, unit scale and origin; the backdrop is stretched with zero offsets; `AssertLayout` verifies root and backdrop target dimensions plus the scaled centered SafeFrame.
- **P1-004 empty setup/restoration:** closed statically. `View` captures/restores/asserts Canvas, root, SafeFrame, backdrop, selection controls, images and TMP copy; `State` restores globals and either restores the complete non-empty scene setup or closes the capture scene for an empty initial setup, then asserts zero loaded/invalid active scene for that case.
- **P1-005 dependency fingerprint:** closed for the approved implementation surface. The explicit 69-path set was checked read-only; every literal path exists and the set is bounded to the contract’s runtime boundary, authoring assets, tests, asset evidence, and proposal/contract inputs. No broad recursive hash is used.
- **P2-001 authoring state:** closed. All four labels, interactability, focus rail/border, disabled strike, dismiss/message copy, and inactive notification are asserted before each render.
- **P2-002 TMP invariants:** closed. All six exact paths assert Regular Static font, empty settings/font fallback lists, single atlas/no multi-atlas, canonical shader/material, atlas dimensions/gradient values, Normal wrapping, bounds/overflow, and non-space mesh coverage.

## Additional static checks

- Oracle uses only the target spec dimensions; no `Screen.width/height` or runtime scaler call is present.
- Manifest writer retains the exact field order, filename order, uppercase SHA-256 values, two-space/LF/no-BOM formatting and one final LF. `ValidateSet` rejects extra/missing files and non-canonical bytes.
- `VerifyGuid` enforces one canonical generator `.meta` occurrence and exact `AssetDatabase.AssetPathToGUID` identity.
- Empty-setup handling avoids the previously rejected zero-entry `RestoreSceneManagerSetup` call.
- C#9/Unity 6000.6 static inspection found no obvious syntax or API-shape blocker in the revised use of `StagePublish`, `SceneSetup`, `RenderTextureDescriptor`, `TMP_Settings.fallbackFontAssets`, or `TMP_FontAsset.fallbackFontAssetTable`. Unity compilation and runtime execution remain unverified by this static gate.

## Verdict

**PASS — P0=0, P1=0, P2=0 (static implementation pre-gate).**

This PASS is limited to the exact generator SHA above. It does not accept the unaccepted historical PNG set, does not replace the required same-process/fresh-process Unity capture runs, and does not grant `Verified` or final integration approval. Those remain Luna post-run evidence and Astra integration decisions.

