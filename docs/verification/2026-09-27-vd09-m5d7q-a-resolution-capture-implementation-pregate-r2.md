# M5D7Q-A resolution-capture implementation pre-gate R2

Date: 2026-09-27  
Reviewer: Luna (independent static review)  
Scope: static review only; no Unity launch, capture execution, or implementation edit.

## Identity and review base

- Requested Terra implementation SHA: `E5E5723DC9B89926804E537B7DB56B310FB407C8FC7FCBAE9D42E9079F6579FE`
- Actual `HubPresentationCaptureGenerator.cs` SHA at review time: `E296A352BD9050E363F4490958E9AFC128B3883798B5CD641F4EF131E8F66E1C`
- Proposal SHA: `8DD84F02FEEADB67EE792511EDA00E298DA8A53B99349E4F80D2F81BC4708FFF`
- Requested Review-contract SHA: `A53BE2B0F749D41619FC344E2E1C42B6D036873299216727E04E81D586FCE797`
- Actual Review-contract SHA: `A359B23776EB127295C4D52AF6ED25078C4C3A056377A70DB79E015B8E5E9EFC`

The requested implementation and contract identities are not the bytes present in the shared workspace. This review is therefore bound to the actual file/hash above and cannot be treated as a review of the requested SHA until the parent reconciles the identity drift.

## Findings

### P1-IMPL-R2-001 — publish is not failure-atomic after the directory swap

`Publish` moves `final` to `.prev`, moves `.tmp` to `final`, deletes `.prev`, and only then calls `ValidateSet(final, null)`. The outer call sets `published` only after `Publish` returns, so failures in the post-publish `VerifyGuid`, the `finally` restoration/equality checks, or the final validation leave the new final set on disk with no previous set available for rollback. The no-existing-final case likewise leaves a final directory after a later failure. Complete all post-publish validation and restoration gates before deleting `.prev`, or retain a rollback journal that restores/removes final on every later failure.

Evidence: `HubPresentationCaptureGenerator.cs:61,68-70,126-128`.
Traceability: proposal P1-003/P1-004; `REQ-M5D7QA-008/009`, `AC-M5D7QA-007/009/010`.

### P1-IMPL-R2-002 — render target is destroyed while still attached to the camera

The per-frame cleanup destroys the `RenderTexture` before assigning `camera.targetTexture = null`; the `finally` path can also destroy an attached target before destroying the camera. `RenderTexture.active` is set to null rather than restored to the captured value until a later state-restore step. This violates the fixed cleanup/restoration boundary and can reproduce the prior Unity “Releasing render texture that is set to RenderTexture.active/Camera.targetTexture” warnings. Detach the camera first, restore the prior active RT/`GL.sRGBWrite`, then destroy readback/RT/camera in a `finally`-safe order.

Evidence: `HubPresentationCaptureGenerator.cs:58,68`.
Traceability: proposal render-lifecycle/global-state requirements; `AC-M5D7QA-007/009`.

### P1-IMPL-R2-003 — actual Canvas/output sizing is not established or asserted

The implementation applies scale only to the child `SafeFrame` (`ApplyOracle`) and configures the Canvas render mode, but never sizes or normalizes the actual root Canvas/RectTransform for the target RT and never proves that `OutputBackdrop` covers the full target. The canonical prefab currently serializes `HubMenuRoot` with zero local scale and zero size. `AssertLayout` checks anchors, not the resulting root/canvas pixel coverage. Camera `pixelRect` alone is insufficient evidence that the real uGUI geometry fills the RT. Configure the in-memory root/canvas sizing required by the capture contract and assert target-sized canvas geometry plus full-output backdrop coverage for every scale, while preserving authored bytes.

Evidence: `HubPresentationCaptureGenerator.cs:49,93,106-110`; `Assets/Prefabs/Hub/HubMenuRoot.prefab` root RectTransform (`m_LocalScale: {x: 0, y: 0, z: 0}`, `m_SizeDelta: {x: 0, y: 0}`).
Traceability: proposal render-lifecycle/independent-oracle requirements; `REQ-M5D7QA-006/009`, `AC-M5D7QA-001/007/010`.

### P1-IMPL-R2-004 — restoration equality omits Canvas/SafeFrame proof

`View.Restore` writes back the saved Canvas/SafeFrame, selection, image, selectable, and TMP values, but there is no `View.Assert` after restoration. `State.Assert` verifies only selected global values and partial `SceneSetup` fields (`path`, `isLoaded`, `isActive`), not the captured Canvas/SafeFrame values or the complete setup record. Add a post-restore structural equality assertion for all captured View fields and all required `SceneSetup` fields; retain the empty-initial-setup close path, then prove zero loaded scenes/active-scene state in that case.

Evidence: `HubPresentationCaptureGenerator.cs:69,153,158`.
Traceability: proposal restoration/equality requirement; `REQ-M5D7QA-008/009`, `AC-M5D7QA-009/010`.

### P1-IMPL-R2-005 — authored dependency fingerprint set is incomplete

`HashDependencies` omits the contract-allowlisted folder metas (`Assets/UI.meta`, `Assets/UI/Resources.meta`, `Assets/UI/Fonts.meta`, `Assets/UI/Fonts/Hub.meta`, `Assets/UI/Shaders.meta`, `Assets/UI/Shaders/Hub.meta`), generator source/meta, builder/validator/asmdef sources and metas, and the registered asset/evidence bytes. Consequently the before/after hash proof does not cover the full admitted authored implementation surface and cannot detect drift in those dependencies. Define the exact contract allowlist and hash every required source/asset/meta/evidence member before and after capture.

Evidence: `HubPresentationCaptureGenerator.cs:133`; contract allowlist at `docs/specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md:705-758`.
Traceability: proposal immutability delta; `AC-M5D7QA-002/007/008/009/010`.

### P2-IMPL-R2-001 — exact authoring-state assertion is incomplete

`ApplyAuthoringView` writes the expected four selectable/image states, dismiss/message copy, and inactive notification, but `AssertAuthoringView` proves only Continue interactability/focus and notice inactivity. It does not independently assert the other three interactability states, non-Continue focus/disabled cues, or the exact notification subtree state. Add complete assertions for the fixed authoring token before each capture.

Evidence: `HubPresentationCaptureGenerator.cs:101-104`.
Traceability: proposal authoring-view assertions; `REQ-M5D7QA-006/009`, `AC-M5D7QA-007/010`.

### P2-IMPL-R2-002 — per-component TMP fallback/material invariants are under-asserted

The six paths and Regular Static font are enumerated, but the implementation does not assert that fallback assets are absent, runtime glyph addition is disabled at the component/font/settings boundary, or that each component’s shared material carries the canonical Mobile-SDF texture dimensions/gradient values. It checks shader identity and `_MainTex` only. Add the exact no-fallback/no-dynamic and material-property checks required by the approved contract.

Evidence: `HubPresentationCaptureGenerator.cs:113-116`; contract TMP observations at `docs/proposals/2026-09-27-vd09-m5d7q-a-resolution-capture-tool-amendment.md:88-99`.
Traceability: `REQ-M5D7QA-007/009`, `AC-M5D7QA-007/008/010`.

## Static verdict

**CONDITIONAL — P0=0, P1=5, P2=2.** The actual file is C#9-shaped with no obvious syntax-level blocker from inspection, but Unity compilation and runtime behavior were intentionally not executed. Terra must reconcile the SHA drift and close all P1/P2 findings; Luna must then re-review the exact implementation hash and run the bounded evidence. Astra may not mark this implementation `PASS`, `Verified`, or accept the capture set from this review.

