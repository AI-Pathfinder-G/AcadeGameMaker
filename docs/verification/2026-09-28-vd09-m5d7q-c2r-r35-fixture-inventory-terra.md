# R35 EditMode fixture and configuration inventory — Terra

Date: 2026-09-28  
Scope: read-only dependency-inventory review for the planned 613-case R35
partition. This is neither an execution record nor an acceptance claim.

In addition to the planned complete 570-file `Assets/AcadeGameMaker`
`.cs`/`.cs.meta`/`.asmdef`/`.asmdef.meta` dependency inventory, Main's fresh
before/after R35 manifest should include the following non-code inputs. These
are the authored/configuration files directly loaded, byte-compared, GUID
checked, or resolved by the selected InputUnity and HubPresentation fixtures.

| Root or exact files | Why it is an R35 dependency |
| --- | --- |
| `Assets/Prefabs.meta`, `Assets/Prefabs/Hub.meta`, `Assets/Prefabs/Hub/HubRuntimeRoot.prefab` and `.meta` | The selected 31-case `HubRuntimeAuthoringBuilderEditModeTests` loads, corrupts/restores, validates GUID stability, and exercises deterministic repair of this Q0 runtime prefab. |
| `Assets/Prefabs/Hub/HubMenuRoot.prefab` and `.meta` | Hub authoring and intent-handoff fixtures load/clone/temporarily mutate/restore this menu prefab and assert asset identity and bytes. |
| `Assets/Scenes.meta`, `Assets/Scenes/Hub.unity` and `.meta` | Selected HubPresentation fixtures open the scene additively and validate the exact two-root/prefab-override matrix; migration tests snapshot its bytes and GUID. |
| `Assets/GameInput.inputactions` and `.meta` | The selected profile-input snapshot fixture reads the input-actions meta GUID; generated action behavior otherwise comes from the already-included code closure. |
| `Assets/UI.meta`, `Assets/UI/Resources.meta`, `Assets/UI/Resources/TMP Settings.asset` and `.meta` | Hub scope/authoring validation loads this sole `Resources` TMP settings object and requires it to remain byte-identical. |
| `Assets/UI/Fonts.meta`, `Assets/UI/Fonts/Hub.meta`, `NotoSansCJKkr-Regular.otf`/`.meta`, `NotoSansCJKkr-Bold.otf`/`.meta`, `NotoSansCJKkr-Regular-SDF.asset`/`.meta`, `NotoSansCJKkr-Bold-SDF.asset`/`.meta`, and `OFL-1.1.txt`/`.meta` under `Assets/UI/Fonts/Hub/` | `HubPresentationAuthoringBuilder` validates the verified source fonts and static SDF pair used by the canonical menu label profile. |
| `Assets/UI/Shaders.meta`, `Assets/UI/Shaders/Hub.meta`, `Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader`/`.meta`, `Assets/UI/Shaders/Hub/TMPro_Properties.cginc`/`.meta`, and `third_party/unity-ugui-2.6.0-LICENSE.md` | The selected authoring validator pins these prerequisite hashes/GUIDs and validates the sole Mobile SDF shader resolution. |
| `Packages/manifest.json` and `Packages/packages-lock.json` | Resolve the Test Framework, Input System, TMP/UGUI, and editor API package graph used to discover/run/import these fixtures. |
| `ProjectSettings/ProjectVersion.txt`, `ProjectSettings/ProjectSettings.asset`, `ProjectSettings/GraphicsSettings.asset`, `ProjectSettings/QualitySettings.asset`, and `ProjectSettings/URPProjectSettings.asset` | Preserve the Unity version and the project graphics/quality/URP state under which the selected asset imports, `Shader.Find`, TMP, canvas, and temporary camera checks run. |

## Q0 audit provenance and folder-meta closure

The four current selected `HubUiOnlyQ0ScopeAuditEditModeTests` also directly
read these immutable evidence inputs; include their exact bytes in the same
before/after manifest:

- `docs/verification/2026-09-20-vd09-m5d7q0-implementation-evidence.md`
- `docs/verification/2026-09-28-vd09-m5d7q-c2-r25-luna-digest.md`
- `docs/verification/2026-09-28-vd09-m5d7q-c2r-process-execution.md`

The 570-file code inventory covers file metas but not directory metas. Include
every existing directory `.meta` file beneath `Assets/AcadeGameMaker/` as a
conservative GUID-identity closure (currently 52 paths), including the
Q0-allowlisted `Assets/AcadeGameMaker/Editor/HubAuthoring.meta`. This is
metadata provenance only; it does not authorize folder scans as a test
selection rule or add unrelated source files.

Do not add transient test-created files, generated `Assets/InitTestScene*.unity`
outputs, temp profile roots, Library cache, logs, licensing data, or unrelated
media to the before/after equality manifest. The tests themselves may make
controlled temporary prefab/scene mutations and restore them; the listed
source-controlled files must compare equal before versus after R35.

This inventory supports the fresh full-partition evidence required by
`AC-M5D7QC2-010` and the C2R regression/evidence boundary in
`AC-M5D7QC2R-007/008`. Main must capture the actual manifest immediately
before the future R35 run and verify it afterwards; Luna independently reviews
the resulting XML/name set and inventory before Astra considers integration.
