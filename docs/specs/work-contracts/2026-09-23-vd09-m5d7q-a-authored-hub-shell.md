---
status: Verified
---

# VD-09 M5D7Q-A authored uGUI hub shell

- Date: 2026-09-23
- Status: Verified — the original revision was approved on 2026-09-23; after
  Terra's real uGUI 2.6.0 run exposed the package-mandatory TMP Settings
  prerequisite, Luna independently re-gated the narrow amendment at
  `PASS — P0=0, P1=0, P2=0` and Astra re-approved it on 2026-09-23. Terra's
  resumed real-package run then exposed the separately missing Mobile SDF
  shader prerequisite. Luna independently pre-gated the exact two-file shader
  amendment at `PASS — P0=0, P1=0, P2=0`, and Astra approved it on
  2026-09-27. Terra's resumed real-package run then proved that the former
  fixed 1024x1024 atlas cannot contain the exact 138-glyph Regular inventory:
  it deterministically misses exactly `하항했`. Luna independently pre-gated
  the resulting 2048x2048 single-atlas capacity amendment at
  `PASS — P0=0, P1=0, P2=0`, and Astra approved it on 2026-09-27;
  implementation reached passing authored assets and XML suites, but the
  contract-required deterministic resolution-capture execution path was found
  absent. Luna independently re-gated the exact Editor-only capture-tool
  amendment at `PASS — P0=0, P1=0, P2=0`, and Astra approved that bounded
  amendment on 2026-09-27. Real Unity execution subsequently exposed a TMP
  observation defect and Unity's inability to restore an initially empty
  batchmode scene setup in-process. Luna independently pre-gated the exact
  runtime-blocker amendment at `PASS — P0=0, P1=0, P2=0`, and Astra approved
  it on 2026-09-27. Warmed Unity then exposed a clean unsaved bootstrap scene
  not represented by the empty `SceneSetup[]`. Luna independently pre-gated
  the exact bootstrap-state addendum at `PASS — P0=0, P1=0, P2=0`, and Astra
  approved it on 2026-09-27. Real TMP observation then proved the shared
  168x20 menu-label rect violates TMP's no-overflow flag by 0.28 logical px.
  Luna independently pre-gated the exact centered 168x22 correction at
  `PASS — P0=0, P1=0, P2=0`, and Astra approved it on 2026-09-27. The later
  retry-q proved that TMP transiently enables its exact required
  Canvas vertex mask after the first mesh update and that the capture tool did
  not isolate that state between resolution specs. Astra approved the bounded
  exact-mask channel-isolation amendment on 2026-09-27; implementation and
  Luna verification completed at `PASS — P0=0, P1=0, P2=0`, and Astra marked
  the integrated contract `Verified` on 2026-09-27
- Owner and final approval authority: Astra
- Contract design: Sol (`gpt-5.6-sol`)
- Intended implementer after approval: Terra
- Independent reviewer: Luna
- Dependencies: M5D7O, M5D7P-A, M5D7P-B, and M5D7Q0 Verified
- Parent requirements: `REQ-UX-004`, `REQ-UX-006`, `REQ-UX-009`,
  `REQ-UX-013`, `REQ-UX-014`, `REQ-ART-010`, `REQ-ART-012`,
  `REQ-ART-013`, `REQ-PLAT-002`, `REQ-PLAT-004`, `REQ-PLAT-009`
- Requirements: `REQ-M5D7QA-001` through `REQ-M5D7QA-010`
- Acceptance criteria: `AC-M5D7QA-001` through `AC-M5D7QA-010`
- Source proposal:
  `docs/proposals/2026-09-23-vd09-m5d7q-authored-hub-presentation.md`
- Technical shader amendment proposal:
  `docs/proposals/2026-09-27-vd09-m5d7q-a-tmp-mobile-sdf-shader-amendment.md`
- Technical atlas-capacity amendment proposal:
  `docs/proposals/2026-09-27-vd09-m5d7q-a-static-atlas-capacity-amendment.md`
- Technical resolution-capture amendment proposal:
  `docs/proposals/2026-09-27-vd09-m5d7q-a-resolution-capture-tool-amendment.md`
- Technical capture-runtime blocker amendment proposal:
  `docs/proposals/2026-09-27-vd09-m5d7q-a-capture-runtime-blocker-amendment.md`
- Technical TMP menu-label layout correction addendum:
  `docs/proposals/2026-09-27-vd09-m5d7q-a-tmp-label-layout-correction-addendum.md`
- Technical TMP Canvas-channel isolation amendment:
  `docs/proposals/2026-09-27-vd09-m5d7q-a-tmp-canvas-channel-isolation-amendment.md`

## Approval gate

This document is `Verified`, including the bounded TMP menu-label layout
correction addendum. The previously approved
revision, minimal `TMP_Settings`, shader,
and atlas-capacity amendments retain their historical authority. The original
revision and the minimal `TMP_Settings`
amendment were each Approved after their recorded Luna gates,
but a resumed real-package run proved that uGUI 2.6.0's public static-SDF path
also requires `Shader.Find("TextMeshPro/Mobile/Distance Field")` to resolve.
The exact two-file shader amendment below passed Luna's independent pre-gate
at `PASS — P0=0, P1=0, P2=0`, and Astra restored `Approved` on 2026-09-27
without a new user product decision. That approval history remains valid.
Terra's next real-package run then proved that the former 1024x1024 Regular
atlas misses exactly `하항했` from the byte-locked 138-code-point population.
The approved response changes only both faces' fixed atlas dimensions to
2048x2048 and adds the deterministic capacity proofs below. Luna independently
pre-gated this exact delta at `PASS — P0=0, P1=0, P2=0`, and Astra restored
`Approved` on 2026-09-27. Terra then completed the authored assets and
XML-backed suites, but no admitted tool could create the required capture set.
Luna independently closed all six P1 and two P2 findings and re-gated the
exact capture-tool delta at `PASS — P0=0, P1=0, P2=0`; Astra restored
`Approved` on 2026-09-27 without a new user product decision. This approval is
not `Verified`: Terra evidence and Luna post-review remain mandatory.
The later real `resolution-capture-retry-e.log` proved that preferred-size/
`textBounds` are not authoritative local mesh containment predicates and that
Unity 6000.6 cannot close the last loaded Hub scene to recreate an initially
empty batchmode `SceneSetup[]`. Luna independently pre-gated the exact blocker
amendment at `PASS — P0=0, P1=0, P2=0`; Astra restored `Approved` on
2026-09-27 without a new user product decision. Terra may implement only that
exact delta. This approval is not `Verified`.
The later warmed retry proved that Unity may report an empty `SceneSetup[]`
while holding one clean unsaved zero-root bootstrap scene. Luna independently
pre-gated the exact addendum at `PASS — P0=0, P1=0, P2=0`; Astra restored
`Approved` on 2026-09-27 without a new user product decision. Terra may
implement only that exact delta. This approval is not `Verified`.
The later real capture observation proved the shared four-label `168x20` rect
sets TMP overflow true although the visible mesh remains inside. Terra must
not change authored layout or run the addendum's new stems until Luna pre-gates
the exact `x=12,y=5,w=168,h=22` centered correction. Luna independently
pre-gated it at `PASS — P0=0, P1=0, P2=0`; Astra restored `Approved` on
2026-09-27 without a new user product decision. This approval is not
`Verified`.
The later `resolution-capture-retry-q.log` proved that actual TMP mesh
generation changes the temporary capture Canvas from authored/pre-spec
`None (0)` to the exact render-required
`TexCoord1 | Normal | Tangent (25)`, and that this transient state leaked into
the next resolution spec. Astra approves the bounded channel-isolation
amendment SHA-256
`34D05E3E92668D97EB916C38FE770F1EB16AA80E22435C9ADCB3A479D916E600`
on 2026-09-27 without a new user product decision. For each of ten specs in
each of two passes, Terra must use one common path that proves exact
`None -> 25 -> 25 -> None` at baseline, after-TMP, render, and finally; it must
reject missing or extra bits, preserve the unchanged layout/TMP/render/
manifest predicates, and retain the outer exact restoration. The authored
Canvas remains `None`; the render-time mask is transient Editor-capture state.
This approval is not `Verified`.

Before approval, the selected font evidence must identify and verify **Noto
Sans CJK KR 2.004 Regular and Bold** under **SIL Open Font License 1.1**. The
evidence must contain authoritative source and final-download URLs, exact
version and filenames, byte sizes, acquisition date, SHA-256 for both font
files and the exact OFL text, redistribution/modification obligations, selected
glyph inventory, static TMP atlas/import settings, and proposed destination
paths. Unknown, moving, substituted, variable-font, or differently versioned
bytes fail the gate. Approval cannot rely on a font family name alone.

This pre-approval gate neither requires nor permits pretending that uncreated
Unity assets already have final hashes. Repository OTF/OFL copy hashes and
`.meta` GUIDs, plus generated SDF asset, embedded atlas, and material hashes,
are post-implementation evidence required by `AC-M5D7QA-008` and Luna's final
review. The recorded 1024x1024 failure is preserved as historical evidence and
is not grounds to alter the source, glyph inventory, point size, padding,
rendering mode, one-atlas rule, or static/runtime policy. Any capacity response
other than the exact 2048x2048 amendment below requires another contract
amendment.

The user-facing decision is closed: Korean-first UI with the exact labels,
copy, and typeface in this contract. Font evidence is a technical approval
prerequisite, not an open product decision.

## Bounded outcome and exclusions

Create one authored `Hub.unity` shell and one reusable `HubMenuRoot.prefab`.
The scene composes the unmodified verified Q0 `HubRuntimeRoot.prefab` with a
uGUI/TextMeshPro presentation that:

- takes the exact M5D7N handoff and optional typed notification once;
- creates one M5D7O controller and one M5D7P-A cursor;
- consumes UI semantic frames only after Q0's fixed-step publication;
- presents `계속하기`, `새 게임`, `설정`, `종료` in the approved order;
- supports deterministic focus, mouse hover/click, XInput/keyboard Submit, and
  manual notification dismissal; and
- takes and retains one typed menu intent, then locks without executing it.

This unit does not add a destination/effect executor. It does not load a
gameplay scene, start/continue a run, create/reset a profile, open Settings,
open Wardrobe, change bindings/settings, call `Application.Quit`, edit build
settings, or mutate gameplay/profile/persistence/costume state. It does not
connect CIO/CUA, import costume media, create a placeholder preview, or change
the Q0 prefab.

## Normative ownership and lifecycle

### Single presentation/input owner

Add one `AcadeGameMaker.Hub.Presentation.Unity` assembly. Its only runtime
owner, `HubMenuPresenterV1`, has `[DefaultExecutionOrder(-180)]`, after the Q0
router `-210` and M5D7N latch `-190`.

The presenter has exact serialized references to the scene's Q0 `InputRouter`
and `HubEntryHandoffLatchV1`, safe-frame scaler, inert `EventSystem`, four menu
views, notice view, TMP labels, and authored hit rectangles. Input.Unity may
add one exact friend-assembly line; no type, receipt, or behavior becomes
public.

The presenter owns exactly:

- one `UiSemanticFrameCursorV1` bound to that exact router;
- one `HubMenuPresentationControllerV1`;
- one exact copied handoff and optional `ProfileLaunchNotificationV1` plus
  equality proofs;
- viewport, focus, hover, and closed presentation state; and
- at most one exact retained `HubMenuIntentV1` taken from M5D7O.

It does not own generated actions, callbacks, maps, router mode, profile,
persistence, costume, scene, application, run, or gameplay state. No second
router, semantic-frame source/cursor owner, direct action subscriber,
`InputSystemUIInputModule`, `StandaloneInputModule`, `PlayerInput`, virtual
mouse, render-`Update` input polling, or input queue is allowed.

### Startup order

1. `Awake` validates exact authored references and keeps controls locked.
2. The first eligible `Update`, after the latch publishes, reads its exact
   handoff and performs its one authorized notification take. The same
   optional typed value is passed to `HubMenuPresentationControllerV1.Create`.
3. The presenter creates one cursor against the exact router. An existing UI
   receipt is a non-delivered baseline; otherwise it waits for the cursor's
   deliberate first baseline.
4. Interaction begins only when controller and cursor are Ready, the source is
   healthy, and the viewport has a valid stable integer frame.
5. Only `FixedUpdate` advances the cursor, after router `-210`. `Update`
   renders owned state but never reads or consumes input.
6. One accepted activation is immediately taken from M5D7O and stored in the
   presenter's retained-intent slot. Menu controls lock permanently. There is
   no callback, delegate, scene request, Quit call, or other effect.

The closed presentation states are `AwaitingHandoff`, `AwaitingBaseline`,
`Ready`, `IntentRetained`, `Failed`, and `Closed`.

## Exact authored assets

`Assets/Scenes/Hub.unity` contains exactly two active root prefab instances:

```text
HubRuntimeRoot   (canonical Q0 prefab; zero overrides)
HubMenuRoot      (M5D7Q-A prefab)
```

There is no Camera, gameplay object, scene executor, costume component, or
third root. The scene overrides only `HubMenuPresenterV1`'s router/latch
references so they point to the exact Q0 scene instance.

`HubMenuRoot.prefab` has this exact hierarchy and logical bottom-left layout:

```text
HubMenuRoot
├─ RectTransform
├─ Canvas                   ScreenSpaceOverlay
├─ CanvasScaler             ConstantPixelSize, scaleFactor 1
├─ HubSafeFrameScalerV1
├─ HubMenuPresenterV1
├─ EventSystem
│  └─ EventSystem only; no BaseInputModule
├─ OutputBackdrop
│  └─ Image                 full output, non-interactive, #090D12
└─ SafeFrame                640x360, centered, integer-scaled
   ├─ Surface               x=0   y=0   w=640 h=360
   ├─ MenuPanel             x=48  y=86  w=224 h=188
   │  ├─ Continue           x=16  y=146 w=192 h=32, `계속하기`
   │  ├─ NewGame            x=16  y=106 w=192 h=32, `새 게임`
   │  ├─ Settings           x=16  y=66  w=192 h=32, `설정`
   │  └─ Quit               x=16  y=26  w=192 h=32, `종료`
   └─ Notification         x=372 y=260 w=256 h=88
      ├─ Message            x=12  y=12  w=196 h=64
      └─ Dismiss            x=220 y=52  w=24  h=24
```

Each menu item is an inert uGUI `Selectable` view with `Navigation.None`, no
persistent UnityEvent, and presenter-owned visual state. No **interactive**
Graphic or hit rectangle extends outside the safe frame. `OutputBackdrop` is
the sole Graphic allowed outside it: the Image covers the full output, has
`raycastTarget=false`, owns no handler/Selectable/CanvasGroup, and is never
included in hit testing. Focus uses a persistent 2-pixel left rail plus full
border; hover uses a distinct 1-pixel border; disabled Continue uses a strike
plus lower contrast. No state is color-only and nothing blinks. Heroine
identity colors are not generic interaction colors.

Each of the four menu labels has bottom-left anchors/pivot, unit scale,
identity rotation, zero TMP margins, and exact local rect
`x=12,y=5,w=168,h=22` inside its unchanged `192x32` button. Its center remains
exactly `(96,16)`. The former shared `x=12,y=6,w=168,h=20` profile, any mixed
old/new profile, any other height/position, auto-size, tolerance, clipping, or
font/copy/size change is invalid. The deterministic builder may migrate only
an exact all-four legacy profile in one atomic prefab update; otherwise it is
a no-op or fails closed as fixed by the label-layout addendum.

## Korean copy and static TMP font

The primary labels are exact and may not be replaced by English, enum names,
or synonyms:

| Semantic item | Display text |
|---|---|
| `Continue` | `계속하기` |
| `NewGame` | `새 게임` |
| `Settings` | `설정` |
| `Quit` | `종료` |

Notification prose is selected only from the exact typed/correlated payload:

| Typed notification | Display text |
|---|---|
| `RecoveryCompleted` | `프로필 복구를 완료했습니다.` |
| `RecoveryArtifactPreservationFailed` | `프로필은 복구했지만 손상 파일을 보존하지 못했습니다.` |
| `PersistenceDeferred` | `프로필을 불러왔지만 변경 사항을 저장하지 못했습니다.` |

Every TMP component explicitly references a static SDF asset made from Noto
Sans CJK KR 2.004 Regular or Bold. Menu labels use Regular 14 logical pixels;
notice copy uses Regular 12; Bold is reserved for explicitly authored emphasis.
No TMP default, runtime fallback discovery, OS font lookup, dynamic atlas,
runtime glyph addition, missing-glyph substitution, or deprecated
`com.unity.textmeshpro` shim is permitted. `com.unity.ugui` remains exactly
2.6.0 and supplies the `Unity.TextMeshPro` assembly.

The sole allowed TMP Resources object is the project-owned
`Assets/UI/Resources/TMP Settings.asset`, type `TMPro.TMP_Settings`. It is not a
default font or package resource bundle. Its package schema version is `2`,
Clear Dynamic Data on Build is true, runtime font-feature retrieval is false,
the active font-feature list is non-null and empty, and Modern Hangul Line
Breaking Rules is true. Every default font, fallback-font list, sprite/emoji
fallback, style sheet, line-breaking text asset, and font/sprite/style/gradient
resource path is null/empty. `Normal` wrapping of the exact Korean strings must
take the modern-Hangul path and never dereference a null line-breaking TextAsset.
Every authored text component still directly references the verified
Regular/Bold static SDF. A missing, duplicate, substituted, reference-bearing,
null-feature-list, or non-modern-Hangul TMP Settings resource fails closed.

The only admitted TMP runtime shader surface is a byte-exact project-owned copy
of two members from uGUI 2.6.0's embedded
`Package Resources/TMP Essential Resources.unitypackage`:

| Project asset | Upstream member GUID | bytes | exact SHA-256 |
|---|---|---:|---|
| `Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader` | `fe393ace9b354375a9cb14cdbbc28be4` | 8,074 | `44C39AABC7E88E7E1FEFFC1880DF754F4ADF86B62E9512AA89E9D2D65171AFF4` |
| `Assets/UI/Shaders/Hub/TMPro_Properties.cginc` | `3997e2241185407d80309a82f9148466` | 2,707 | `66DB1F03E8D7A413EBA79BCB6602FDB2DE710B586F34A6F2584ED6F68F028E90` |

The source archive is 804,874 bytes with SHA-256
`26CDEE2072683CB25CEAFA4FAB23C93C35B2F692E5DAF9CA0F33D05C4E163274`;
the archive itself is neither imported nor copied into the repository. The
shader keeps the exact declared name `TextMeshPro/Mobile/Distance Field` and
the exact local `TMPro_Properties.cginc` include. Its other includes remain
Unity engine includes. No source edit, package-cache runtime dependency, or
second TMP runtime shader is admitted.

The package's exact 431-byte `LICENSE.md`, SHA-256
`3F8833F9736C0B5DB5076663BA5ABB0A33606712FA9128456AC834FC9D43FDD6`,
is preserved at `third_party/unity-ugui-2.6.0-LICENSE.md`. It records the uGUI
copyright and Unity Companion License governing these two copied source files.
`docs/assets/evidence/AST-UI-SHADER-001.md` records the installed
`com.unity.ugui` 2.6.0 fingerprint
`23caec89ae2780ae2aa9b14f95a19c03e3dcdf9e`, upstream/target hashes, official
license URL, and final Unity identity evidence.

The project-owned meta identities are fixed and do not reuse the upstream
member GUIDs:

| meta | fixed GUID | exact SHA-256 |
|---|---|---|
| `Assets/UI/Shaders.meta` | `070c509b184d2bca114677820f278a46` | `142F6FE56DA2CF181D4E55B0C86F0A10ECFD0A1CE266BFAAF1FCFC0ABB5C9BC8` |
| `Assets/UI/Shaders/Hub.meta` | `12819597576562f134f4225299f186fe` | `0441D34AA04ED1622960D9B115A249E452A1E75B97DB0E1DCF8D8A861EAAE5A9` |
| `TMP_SDF-Mobile.shader.meta` | `5d56ff4c414a9d872126e38007c1d57d` | `94D3127E88D565DE1A4D71F6FB8F4E08A2CAEAC3D2FD6D8BE0277C033E9BF91C` |
| `TMPro_Properties.cginc.meta` | `f0cdd71d926cc071f345d0b51adef566` | `E8B9FD72E432CE711D125188E548A461263E1A74FE673A01D736984BCFCCDAF8` |

The two folder metas use canonical empty `DefaultImporter`; the shader and
include metas use canonical empty `ShaderImporter`. All four are UTF-8 no-BOM,
LF, final-LF canonical YAML. Any missing, renamed, changed, duplicate-name,
wrong-GUID/importer/hash, compiler-error, or foreign-resolution case fails
before SDF generation. Importing all or any other member of TMP Essential
Resources remains forbidden.

The static glyph set must include every Hangul syllable, punctuation, digit,
space, and Latin/debug glyph actually present in the exact authored shell and
tests. Atlas generation uses a recorded deterministic population set and
settings; the validator rejects a missing required glyph, dynamic population,
foreign source font, or mismatched material/atlas reference.

The exact source, license, hashes, internal font names/version, glyph inventory,
generation settings, and proposed destinations are fixed by
`docs/assets/evidence/AST-UI-FONT-001.md`. Its population is exactly 138 sorted
code points: printable ASCII `U+0020-U+007E`, `U+00D7`, and the 42 enumerated
Hangul syllables in that evidence. Regular and Bold each use Static population,
sampling point size 90, padding 9, `SDFAA`, one 2048x2048 atlas, multi-atlas
disabled, no fallback, and no runtime glyph addition. Terra appends generated
Unity asset hashes and GUID evidence after implementation; those generated
values are an `AC-M5D7QA-008` acceptance input, not a pre-approval artifact.

Project-owned static TMP atlases admit only one identical square power-of-two
dimension profile across all faces. This is a project policy that bounds
cross-platform texture import/serialization/compression behavior and removes
width/height-oriented packing variants; it does not claim that uGUI rejects
NPOT or rectangular textures. Because the selected 1024x1024 profile has
failed in the real fixed-package run, 2048x2048 is the next and smallest
admitted larger profile. It is not asserted to be the mathematical minimum
among arbitrary rectangles. Actual Regular/Bold capacity at 2048x2048 remains
an implementation acceptance proof, not a pre-approval assumption.

Each embedded atlas texture must be `TextureFormat.Alpha8`, have no mip chain
(`mipmapCount == 1`), and contain exactly one texture for its face. Its raw
level-zero payload is exactly 4,194,304 bytes per face and 8,388,608 bytes for
the pair, an exact 6,291,456-byte increase over the former 1024x1024 pair.
Those figures are raw Alpha8 payload, not a claim about total resident memory,
serialized asset size, or build size; Terra records the generated asset byte
size/hash and a stable Unity memory measurement when the API supplies one,
with the API and build/editor context named. `SystemInfo.maxTextureSize` must
be at least 2048 before generation. The material must report
`_TextureWidth == 2048`, `_TextureHeight == 2048`, and `_GradientScale == 10`.
After Astra approval, `AST-UI-FONT-001.md` must preserve the historical
1024x1024 miss and replace only the normative atlas width/height and derived
raw-payload figures with these 2048 values. Terra's implementation evidence
must add the exact per-face texture structure, glyph-set/missing-set result,
material values, generated asset/meta hashes and sizes, pair-atomic/no-op
results, maximum-texture-size observation, and named memory-measurement API and
context when stable. It may not rewrite earlier blocker evidence.

## Resolution and accessibility

The safe-frame scaler computes
`floor(min(outputWidth / 640, outputHeight / 360))`, centers the 640x360 root,
and applies only that positive integer scale. Required results are 1x, 2x, 3x,
and 4x at 640x360, 1280x720, 1920x1080, and 2560x1440. 1366x768, ultrawide,
and narrow outputs use the largest centered integer frame with inert letter/
pillar bars. There is no crop, stretch, fractional layout scale, added world/UI,
or bar interaction.

Output below 640x360 suspends all interaction without changing controller,
cursor, focus, notice, or retained intent. After any output-size change,
Point/Click are suppressed for the changed delivered frame and one later
consecutive stable-viewport frame. This prevents old actual-pixel coordinates
from being hit-tested against a new frame.

TMP remains sharp at final output. Text does not truncate, overlap, or escape
its box at 640x360. Text roles remain at least 12/14/18 logical pixels; every
interactive rectangle is at least 24x24; all content keeps at least 12 logical
pixels from the safe-frame edge. Focus, hover, disabled, and visible notice
states each have a non-color signal at 1x. The ordinary OS cursor is visible
and unlocked while this root is active; gamepad uses focus only and creates no
cursor.

## Navigation, pointer, and command semantics

Actual-point coordinates hit UI only when
`left <= x < right && bottom <= y < top`. Logical coordinates use integer
division by the current integer scale. Negative, off-window, letterbox, and
pillarbox positions clear hover and their clicks do nothing.

- Initial focus is exactly M5D7O's Continue/New Game result.
- Vertical focus visits only interactable items in menu order, one item per
  delivered frame, and clamps without wrapping. X and Scroll do nothing.
- A navigation change acts only when final `abs(YQ4096) >= 2048`; zero,
  release, and sub-threshold values do nothing.
- Pointer movement owns hover only and never silently moves focus. A valid
  button click moves focus to that interactable button before activation.
- Disabled Continue may hover but cannot focus or activate.
- Top-level Cancel is a no-op; HubUIOnly cannot request Gameplay.
- At most one M5D7O command occurs per delivered frame. Apply navigation first,
  then use priority: notice-dismiss Click; visible-notice Submit dismissal;
  interactable menu Click; focused-item Submit.
- Click dismissal requires the click-time point inside the 24x24 dismiss
  target. Submit dismisses a visible notice regardless of menu focus and
  consumes that edge.
- No timer, Cancel, navigation, activation, intent take, disable, or destroy
  dismisses a notice.

Menu Click while a notice is visible may retain an intent but cannot dismiss
or hide the notice. After intent retention only notice dismissal remains
interactive. This unit keeps the scene/Canvas present; a later executor must
retain the root or transfer the exact typed/correlated notice before unload.

## Notification correlation

The presenter derives the display token once from the exact validated handoff
and exact optional `ProfileLaunchNotificationV1`, stores value/proof copies,
and revalidates correlation before display or dismissal. It does not choose
copy from `HubMenuViewV1.NotificationKind` alone. M5D7O owns
`Visible -> Dismissed`; the presenter only renders that state and requests the
approved Click/Submit dismissal.

## Wardrobe boundary

`설정` retains `HubMenuItemV1.Settings` and stops. This unit authors no Settings
screen or Wardrobe entry and adds no CIO/CUA reference.

Any future wardrobe composition requires CIO `AC-CIO-007` acceptance, CUA
Approved/Verified, an exact storage root/recovered state/catalog/completed-
action publisher/portrait target/gameplay renderer inventory, and either an
Accepted real media package or the truthful 36-row pending/
`NoAcceptedDefault` presentation with no selectable fallback. It must reuse or
approvedly replace this single cursor owner; it may not add another cursor,
router subscriber, input module, or direct callback.

## Failure and teardown

Invalid topology/reference, foreign router/latch, notification mismatch,
second cursor/controller creation, cursor skip/replay/close/failure, router
fault, malformed M5D7O view, viewport proof corruption, or presenter state
corruption latches `Failed` before a command or intent publication.

Failure clears hover/EventSystem selection and disables all interaction while
preserving the last complete visual, exact typed notice, controller evidence,
and retained intent. It cannot normalize evidence, recreate a controller or
cursor, request mode, alter maps, dispose actions, dismiss, invoke an effect,
or retry.

`OnDisable`/`OnDestroy` clears presentation-owned selection/references only.
It never takes a notification late, dismisses, consumes another frame, changes
a map, or closes the adopted actions. Q0 remains action-owner teardown.
Disable/destroy before handoff/baseline publishes no controller command or
intent.

## Deterministic authoring

Add `HubPresentationAuthoringBuilder` and observational
`HubPresentationAuthoringValidator` to the existing Hub Authoring Editor
assembly.

- Valid assets are byte-for-byte builder no-ops.
- Repair writes canonical LF UTF-8 YAML with fixed local IDs and exact
  references; it does not rely on process-random prefab/scene local IDs.
- Same-process and fresh-process second runs preserve prefab/scene bytes and
  existing folder/asset `.meta` GUIDs.
- Project component wiring uses typed assignment-only seams. Builder code does
  not reflect private runtime fields, execute lifecycle, allocate actions,
  read a profile, take a notification, or touch Q0.
- The source-controlled canonical `Assets/UI/Resources/TMP Settings.asset` and
  fixed `.meta` are preconditions. The builder never creates, changes, deletes,
  or imports a TMP resource bundle; before SDF creation it proves that exactly
  this one typed settings object resolves as `Resources/TMP Settings`. The
  validator's schema inspection is restricted to this package-version-locked
  settings asset and does not authorize runtime reflection or private-field
  access to project components.
- The source-controlled Mobile SDF shader, its sole local include, their four
  fixed metas, the exact uGUI notice, and `AST-UI-SHADER-001` evidence are also
  immutable preconditions. Before either face is generated, the validator
  proves exact version/path/GUID/importer/hash/name/include, zero compiler
  errors, no `Assets/TextMesh Pro/` import, and that
  `Shader.Find("TextMeshPro/Mobile/Distance Field")` is reference-equal to the
  exact canonical shader loaded from its asset path. It also proves no second
  project shader has that name.
- The builder never creates, copies, modifies, deletes, or imports those
  shader/license prerequisites and never calls `AssetDatabase.ImportPackage`.
  On preflight failure it writes no SDF, atlas, material, prefab, or scene. On
  a later creation failure it may destroy only its unsaved in-memory objects
  and remove only an SDF target newly created by that invocation; it cannot
  repair or rewrite a canonical prerequisite or an already-valid asset.
- Font authoring is pair-atomic. After proving `SystemInfo.maxTextureSize >=
  2048`, the builder creates, populates, and validates both Regular and Bold
  2048x2048 candidates in memory before persisting either face. The only
  amended generator constants are atlas width and height, each 1024 to 2048;
  each face receives one public creation call and one exact 138-glyph
  population attempt. `TryAddCharacters == false`, any non-empty missing set,
  or any structural mismatch fails without retry. Only after both candidates
  pass may population be frozen to Static and both canonical SDF targets be
  saved. If either face fails, neither new SDF/atlas/material candidate becomes
  durable. Cleanup may
  destroy only unsaved objects and newly created SDF targets from that exact
  invocation; it cannot delete or rewrite a pre-existing valid face, source
  OTF, source license, TMP Settings, shader, include, notice, or evidence.
  There is no adaptive retry, atlas growth, point-size/padding reduction,
  subset split, face-specific profile, fallback, dynamic population, or
  multi-atlas escape path.
- After persisting a candidate pair, the builder must save, refresh, reload
  both root font assets from their canonical paths, and prove that each root
  contains its atlas texture and material as durable subassets before creating
  or retaining the menu prefab or Hub scene. An in-memory reference alone does
  not satisfy pair-atomicity. Missing reloaded atlas/material subassets fail
  closed and remove that invocation's pair plus its newly created dependent
  prefab/scene.
- Every completed Regular/Bold SDF material directly references the canonical
  Mobile SDF shader. Same-process and fresh-process no-op passes preserve
  shader/include/license/meta and completed authored bytes.
- The validator never repairs. It checks exact scene roots/prefab sources, zero
  Q0 overrides, hierarchy/component order, active/enabled state, references,
  execution order, Canvas/scaler/EventSystem/no-input-module setup, layout and
  hit rectangles, navigation, empty UnityEvents, copy, font/material/atlas/
  glyph/settings/shader/license hashes, material shader identity,
  palette/state visuals, missing scripts, prefab overrides, and forbidden
  components/children.
- The proposed Editor-only capture generator opens the canonical `Hub.unity`
  only in memory, validates it first, and never saves it. It renders the actual
  scene `HubMenuRoot` through a temporary Camera and RenderTexture after
  applying only the fixed authoring view (Continue enabled/focused, exact four
  Korean labels, notification absent). It runs no presenter/router/latch/input
  lifecycle and uses no reflection or private-state injection. For every exact
  output it applies the runtime integer-scale formula, asserts layout/hit/copy/
  TMP/font/material/shader/state cues, writes the exact manifest and digest
  defined by the capture amendment, and restores the original Editor scene
  setup. A temporary sibling directory makes the ten-PNG set pair-atomic;
  failure removes only temporary capture files and preserves any prior valid
  set. Same-process and fresh-process runs must be byte-identical and preserve
  scene/prefab/font/settings/shader bytes.
- Capture dimensions never flow through `Screen.width/height` or
  `HubSafeFrameScalerV1.Refresh`; the RenderTexture dimensions feed an
  independent copy of the exact integer-scale oracle, which is applied to the
  actual scene SafeFrame only in memory. Runtime resize/input semantics remain
  exclusively XML-backed. Camera, Canvas, ARGB32/depth-24 sRGB RenderTexture,
  RGBA32 readback, MSAA/mip/HDR/culling/projection/pixelRect and global render
  state use the exact fixed values in the capture amendment. A Null graphics
  device, unsupported surface, dirty loaded scene, or stale capture tmp/prev
  directory fails before mutation.
- One `finally` restores and proves Selection, Canvas/SafeFrame,
  RenderTexture.active and GL.sRGBWrite state and destroys all temporary
  objects. A non-empty initial scene setup additionally restores and proves
  the exact setup/active scene. An empty initial setup is admitted only for an
  exact terminal `-batchmode -quit -executeMethod
  AcadeGameMaker.Hub.Authoring.Editor.HubPresentationCaptureGenerator.Capture`
  process with no `-runTests`. Its initial scene state must be either true zero
  scenes with an invalid active scene or exactly one active, loaded, clean,
  unsaved, zero-root bootstrap scene with empty path and build index `-1`.
  Before classifying it, the tool emits the exact invariant snapshot marker
  from the blocker amendment including setup/scene counts and active validity,
  handle, escaped path/name, load/dirty/build-index/root-count. Any extra,
  saved, dirty, rooted, inactive, or otherwise different scene fails with the
  amendment's exact sorted scene-preflight failure codes before mutation.
  That terminal mode does not claim an impossible zero-scene restoration: it
  replaces the nonpersistent bootstrap state when present, leaves exactly the
  canonical clean unsaved Hub scene loaded until immediate
  process exit, records `zeroSceneRestored=false`, and proves no Save/SetDirty
  call, authored dependency hash equality, global/view restoration, and the
  exact accepted/complete terminal markers fixed by the blocker amendment.
  Any other empty-setup invocation fails before mutation. The generator hashes
  authored dependencies before and after. It publishes through exact
  `.tmp -> final` and prior `final -> .prev` directory moves with rollback; a
  partial set is never accepted. The fixed generator GUID must be absent
  repository-wide before creation and resolve only to its canonical path
  afterward.
- Manifest bytes are UTF-8 no-BOM, two-space indented, LF-only, invariant
  decimal, lowercase booleans, exact field/order, no trailing spaces, and one
  final LF. Every exact TMP path/copy checks Regular Static font, canonical
  shader/material, Normal wrapping, configured size, `isTextOverflowing=false`,
  `firstOverflowCharacterIndex==-1`, exact source-character records, canonical
  font/material ownership, visible non-space glyphs, actual mesh quads wholly
  contained in the same RectTransform-local rect, and no fallback/dynamic
  path. Preferred size and `textBounds` remain invariant-culture diagnostics,
  not standalone pass/fail bounds. Each path emits the exact observation and
  sorted failure-code grammar fixed by the blocker amendment before assertion.
  No epsilon, font/copy/size/rect/wrapping/atlas change, or coordinate-space
  substitution is admitted. `CONTINUE_ENABLED_FOCUSED|NOTICE_ABSENT` is
  explicitly the authoring capture view, never evidence that runtime
  handoff/input/cursor executed.

For `TerminalEmptyBatchSession`, same-process determinism means two independent
ten-spec render/assert/encode passes inside one entrypoint after reapplying and
reasserting the canonical authoring view. All ten PNG bytes and canonical
manifest bytes must match before publication, and the log records the exact
`M5D7QA_SAME_PROCESS_CAPTURE|passes=2|png=10/10|manifest=true|equal=true`
marker. The separate authorized fresh Unity process must then reproduce the
existing final set byte-for-byte. This terminal specialization does not weaken
the ordinary non-empty setup restoration or fresh-process requirement.

## Requirements

- **REQ-M5D7QA-001:** compose the unchanged canonical Q0 prefab and one exact
  menu prefab as the only two active roots of `Hub.unity`.
- **REQ-M5D7QA-002:** make one presenter the sole M5D7P-A cursor and M5D7O
  lifecycle/input interpreter, consuming consecutive frames only after the
  router's fixed commit without any second subscriber/module/queue.
- **REQ-M5D7QA-003:** project the exact M5D7O order, availability, initial
  focus, notice state, and one retained intent without executing an effect.
- **REQ-M5D7QA-004:** preserve exact typed notification/receipt correlation and
  exact Korean copy with Click/Submit-only dismissal and no warning loss.
- **REQ-M5D7QA-005:** implement the closed focus, hover, pointer hit, command
  priority, safe-frame rejection, Cancel, and no-virtual-mouse rules above.
- **REQ-M5D7QA-006:** preserve 640x360 logical layout, integer output scaling,
  2560x1440 baseline, 640x360 minimum, text/hit/margin minima, and non-color
  state cues; generate the exact real-scene resolution evidence with paired
  deterministic assertions rather than screenshot-only inference.
- **REQ-M5D7QA-007:** use only uGUI 2.6.0's included TMP assembly, the exact
  two-file project-owned Mobile SDF shader surface under its preserved Unity
  Companion License notice, and verified Noto Sans CJK KR 2.004 Regular/Bold
  static SDF assets under SIL OFL 1.1; each face uses the exact 138-glyph,
  point-90, padding-9, SDFAA, Alpha8/no-mip, single 2048x2048 Static profile
  with multi-atlas, fallback, and runtime addition disabled. Project static TMP
  atlas dimensions are identical across faces, square, and power-of-two only;
  after the proven 1024x1024 failure, 2048x2048 is the next and smallest
  admitted larger profile, without any claim about arbitrary rectangles.
- **REQ-M5D7QA-008:** fail closed and preserve input, notification, controller,
  action-owner, and forensic boundaries on every failure/teardown path.
- **REQ-M5D7QA-009:** deterministically build and observationally validate only
  the exact scene/prefab/font/settings/shader/license assets without changing
  Q0, package contents, project settings, or unrelated paths; generate and
  validate the Regular/Bold pair atomically, leave no durable partial pair on
  failure, and clean only targets created by the current invocation; the
  capture tool likewise leaves authored assets immutable and publishes only a
  complete deterministic capture set.
- **REQ-M5D7QA-010:** stop at retained menu intents and the Settings/Wardrobe
  boundary; add no menu, scene, Quit, profile, costume, or gameplay effect.

## Acceptance criteria

- **AC-M5D7QA-001:** editor validation proves the exact two-root scene,
  unmodified/zero-override Q0 instance, exact menu hierarchy/references, and
  absence of Camera/gameplay/effect/costume/extra owners. It proves that the
  full-output `OutputBackdrop` is the sole safe-frame-external Graphic, has
  `raycastTarget=false`, and has no input handler, `Selectable`, or
  `CanvasGroup`; every interactive Graphic and hit rectangle is inside the
  safe frame.
- **AC-M5D7QA-002:** same-process and fresh-process builder runs produce exact
  prefab/scene bytes and preserve every pre-existing folder/asset GUID; each
  independent topology/layout/reference/copy/font/forbidden-component mutation
  is rejected without repair. They preserve exact TMP Settings asset/meta bytes
  and independently reject missing, duplicate, wrong-type/script/GUID/version,
  default/fallback/reference/path, runtime-feature, null/non-empty active-feature
  list, modern-Hangul false, clear-dynamic false, and extra TMP Resources
  mutations. They also preserve the exact shader/include/license/meta bytes and
  independently reject missing/renamed/drifted assets, wrong path/GUID/importer/
  hash/name/include, shader compiler errors, null/foreign/duplicate-name
  resolution, package-version drift, any `Assets/TextMesh Pro/` or other
  Essential Resources import, and a failed preflight that writes authored
  outputs. Font-capacity mutations independently reject dimensions 1024x1024,
  4096x4096, non-square `1024x2048`, NPOT square `1536x1536`, any other
  non-square/non-power-of-two profile, or any dimension other than exact
  2048x2048; point size 89; padding 8; non-SDFAA;
  RGBA32; `mipmapCount > 1`; texture count zero or two; multi-atlas true;
  `atlasIndex == 1`; missing `하`, `항`, or `했`; an extra or duplicate glyph;
  material dimension, gradient, or texture drift; foreign source, shader, or
  fallback; and either-face failure that leaves a durable partial pair.
  Failure cleanup preserves every pre-existing valid asset and removes only
  outputs newly created by that invocation.
- **AC-M5D7QA-003:** Primary, Previous, and Default plus all notification kinds
  and true absence through the real Q0/M5D7N route create exactly one
  controller/cursor, remain locked through baseline, and deliver each
  consecutive semantic frame once.
- **AC-M5D7QA-004:** keyboard, mouse, and XInput scripts prove exact initial
  focus, interactable skip/clamp, hover precedence, click-time hit testing,
  simultaneous-edge priority, disabled Continue, top-level Cancel no-op, and
  no second subscriber/module/queue. Presentation activation also proves
  `Cursor.visible=true` and `Cursor.lockState=None`; gamepad-only operation
  creates no cursor object and does not hide or lock the OS cursor.
- **AC-M5D7QA-005:** all four menu actions retain one exact intent, lock, and
  execute no effect; duplicate frame/activation/take cannot replace or repeat
  it.
- **AC-M5D7QA-006:** all notice kinds/absence show the exact Korean strings and
  TopRight placement from exact typed correlation; only dismiss-target Click
  or visible-notice Submit dismisses, with no timer/lifecycle/menu dismissal
  and no loss after intent retention.
- **AC-M5D7QA-007:** layout/hit tests and the exact named captures below at
  640x360, 1280x720, 1920x1080, 2560x1440, 1366x768, ultrawide, narrow,
  resize, and below-minimum outputs prove integer framing, inert bars,
  `OutputBackdrop` non-interaction, pointer quarantine, readable TMP, no
  truncation, non-color cues, and 24/12 minima. The captures and assertions
  also prove that the fixed 2048x2048 atlas amendment changes no logical
  layout/copy and keeps every Korean string readable at the four integer
  baseline sizes. The generator renders the actual scene/menu/TMP assets,
  records exact PNG and state-digest hashes, and rejects any failed assertion,
  extra/missing file, text overflow, or authored-asset mutation. It also proves
  target-dimension oracle independence from Screen, the fixed Camera/Canvas/RT
  profile, exact per-path TMP observations, canonical JSON bytes, directory-
  atomic publication, fixed-GUID uniqueness, and finally restoration of all
  observed Editor/global state.
- **AC-M5D7QA-008:** static/evidence audit proves uGUI 2.6.0, included
  `Unity.TextMeshPro`, no shim/default/dynamic/runtime font path, exact Noto
  2.004 Regular/Bold source and generated-asset hashes, complete required
  glyphs, deterministic atlas settings, SIL OFL 1.1 evidence, and the sole
  minimal null/empty project-owned TMP Settings policy above, including modern
  Hangul wrapping and the non-null empty active-feature list. It also proves the
  exact uGUI package fingerprint/archive/member/source/meta/license provenance,
  canonical `Shader.Find` identity, zero shader compiler errors, and direct
  canonical shader references from both generated SDF materials, with no full
  bundle/default/fallback/additional TMP shader import. It additionally proves
  `SystemInfo.maxTextureSize >= 2048`; point 90, padding 9, SDFAA, Static,
  2048x2048; exactly one Alpha8 texture with `mipmapCount == 1` per face; exact
  raw payload 4,194,304 bytes per face; exact 138-glyph set with every glyph on
  atlas index zero and in bounds; multi-atlas false; material texture size
  2048x2048 and gradient scale 10; generated asset bytes/hashes; and the named,
  contextual Unity memory measurement when that measurement is stable. The
  audit also proves the identical-across-faces, square-POT-only project policy
  and rejects any inference that 2048x2048 is a mathematical minimum among
  package-supported arbitrary rectangles.
- **AC-M5D7QA-009:** corruption and every disable/destroy boundary prove no
  late take/frame/dismissal/intent/effect/map mutation/action disposal and no
  recovery from `Failed`.
- **AC-M5D7QA-010:** focused EditMode includes the full independent shader
  and atlas-capacity mutation matrices. An in-memory negative fixture proves
  that the former exact 1024x1024 Regular profile reports exactly `하항했` as
  missing, while 2048x2048 Regular and Bold report no missing glyph. Focused
  PlayMode and captures prove every rendered Korean TMP material has the
  canonical non-missing/non-error shader. Same-process and fresh-process
  no-op passes preserve completed bytes. Focused suites,
  direct M5D7O, M5D7P-A, M5D7Q0, and M5D7N regressions, then full EditMode and
  PlayMode finish with failed, skipped, and inconclusive zero. Capture
  same-process/fresh-process runs reproduce identical PNG/manifest bytes and
  preserve authored bytes; the manifest authoring-state token is never used as
  runtime input evidence; Luna reports
  `P0=0` and `P1=0`.

### Traceability

| Requirement | Acceptance evidence |
|---|---|
| `REQ-M5D7QA-001` | `AC-M5D7QA-001`, `AC-M5D7QA-002` |
| `REQ-M5D7QA-002` | `AC-M5D7QA-003`, `AC-M5D7QA-004`, `AC-M5D7QA-009` |
| `REQ-M5D7QA-003` | `AC-M5D7QA-003`, `AC-M5D7QA-005` |
| `REQ-M5D7QA-004` | `AC-M5D7QA-006`, `AC-M5D7QA-009` |
| `REQ-M5D7QA-005` | `AC-M5D7QA-004`, `AC-M5D7QA-007` |
| `REQ-M5D7QA-006` | `AC-M5D7QA-007` |
| `REQ-M5D7QA-007` | `AC-M5D7QA-008`, `AC-M5D7QA-010` |
| `REQ-M5D7QA-008` | `AC-M5D7QA-009`, `AC-M5D7QA-010` |
| `REQ-M5D7QA-009` | `AC-M5D7QA-001`, `AC-M5D7QA-002`, `AC-M5D7QA-008` |
| `REQ-M5D7QA-010` | `AC-M5D7QA-005`, `AC-M5D7QA-009` plus static audit |

## Exact proposed implementation allowlist

Runtime and assembly boundary:

- `Assets/AcadeGameMaker/Runtime/HubPresentation.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/AcadeGameMaker.Hub.Presentation.Unity.asmdef` and `.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/AssemblyInfo.cs` and `.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuCopyV1.cs` and `.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs` and `.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubSafeFrameScalerV1.cs` and `.meta`
- `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuButtonViewV1.cs` and `.meta`
- `Assets/AcadeGameMaker/Runtime/Input/Unity/AssemblyInfo.cs`, limited to one
  exact friend-assembly line

Authoring and authored assets:

- `Assets/AcadeGameMaker/Editor/HubAuthoring/AcadeGameMaker.Hub.Authoring.Editor.asmdef`, limited to exact presentation/uGUI/TMP references
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringBuilder.cs` and `.meta`
- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringValidator.cs` and `.meta`
- proposed `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationCaptureGenerator.cs`
  and `.meta`, fixed GUID `4f0211caf1394d76a959b6df9ab89477`
- `Assets/Prefabs/Hub/HubMenuRoot.prefab` and `.meta`
- `Assets/Scenes/Hub.unity` and `.meta`

Font, TMP shader, and license assets and evidence:

- `Assets/UI.meta`
- `Assets/UI/Resources.meta`
- `Assets/UI/Resources/TMP Settings.asset` and `.meta`
- `Assets/UI/Fonts.meta`
- `Assets/UI/Fonts/Hub.meta`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular.otf` and `.meta`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold.otf` and `.meta`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset` and `.meta`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold-SDF.asset` and `.meta`
- `Assets/UI/Fonts/Hub/OFL-1.1.txt` and `.meta`
- `Assets/UI/Shaders.meta`
- `Assets/UI/Shaders/Hub.meta`
- `Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader` and `.meta`
- `Assets/UI/Shaders/Hub/TMPro_Properties.cginc` and `.meta`
- `third_party/unity-ugui-2.6.0-LICENSE.md`
- one exact Noto Sans CJK KR 2.004 row in `docs/assets/asset-register.md`
- `docs/assets/evidence/AST-UI-FONT-001.md`
- one exact `AST-UI-SHADER-001` row in `docs/assets/asset-register.md`
- `docs/assets/evidence/AST-UI-SHADER-001.md`

Tests:

- new `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/` folder/meta,
  asmdef/meta, `HubPresentationAuthoringTests.cs`,
  `HubPresentationScopeAuditTests.cs`, and their `.meta` files
- new `Assets/AcadeGameMaker/Tests/PlayMode/HubPresentation/` folder/meta,
  asmdef/meta, `HubMenuPresenterPlayModeTests.cs`,
  `HubMenuFailureTeardownPlayModeTests.cs`,
  `HubMenuResolutionPlayModeTests.cs`, and their `.meta` files

Documentation/evidence:

- this contract, its source proposal, and the exact Mobile SDF shader amendment
  proposal, plus the exact static-atlas capacity amendment proposal
- one Luna pre-gate, Terra implementation evidence, Luna post-review, GPT
  participation ledger entry, and minimal `docs/README.md`/
  `docs/specs/README.md` index hunks
- one exact persistent result subtree,
  `artifacts/unity-results/m5d7qa-20260923/`, containing only these admitted
  result stems (each test run has `.xml` and `.log`):
  `font-package-smoke`, `builder-pass-a`, `builder-pass-b`,
  `builder-fresh-process`, `scope-audit`, `focused-editmode`,
  `focused-playmode`, `regression-m5d7o`, `regression-m5d7pa`,
  `regression-m5d7q0`, `regression-m5d7n`, `full-editmode`, and
  `full-playmode`; plus the log-only resumed-builder records
  `builder-pass-c.log`, `builder-fresh-process-2048.log`, and the log-only
  Astra-authorized recovery runs `builder-pass-d.log` and
  `builder-pass-e.log`,
  plus the Astra-authorized XML-backed test recovery pair
  `focused-editmode-retry.xml/.log`,
  and `focused-playmode-retry.xml/.log`,
  and `regression-m5d7o-retry.xml/.log`,
  and `regression-m5d7o-longrun.xml/.log`,
  log-only `resolution-capture.log` and the Astra-authorized corrected
  recovery runs `resolution-capture-retry.log` and
  `resolution-capture-retry-b.log`, `resolution-capture-retry-c.log`, and
  `resolution-capture-retry-d.log`, `resolution-capture-retry-e.log`, and the
  corrected execution `resolution-capture-retry-f.log` plus its unchanged
  warm-cache recovery `resolution-capture-retry-g.log` and proposed
  bootstrap-state recovery `resolution-capture-retry-h.log` plus its
  compile-corrected recoveries `resolution-capture-retry-i.log` and
  `resolution-capture-retry-j.log`, `resolution-capture-retry-k.log`, and
  `resolution-capture-retry-l.log`, `resolution-capture-retry-m.log`, and
  `resolution-capture-retry-n.log` and `resolution-capture-retry-o.log`, plus the fresh-process proof
  `resolution-capture-fresh.log`,
  `resolution-captures/` for the named PNG captures and their assertion
  manifest, `scope-before.json`, `scope-after.json`, and `final-manifest.json`.
  The label-layout correction additionally proposes log-only
  `builder-pass-f.log`, the Astra-authorized compile-corrected recovery
  `builder-pass-g.log`, the Astra-authorized scene-validator recovery
  `builder-pass-h.log`, the Astra-authorized Q0 semantic-zero recovery
  `builder-pass-i.log`, and `builder-fresh-process-layout.log`; XML/log pairs
  `focused-editmode-layout`, `focused-playmode-layout`,
  `focused-editmode-layout-retry`,
  `focused-editmode-layout-retry-b`,
  `focused-playmode-layout-retry`,
  `focused-playmode-layout-retry-b`,
  `focused-playmode-layout-retry-c`,
  `regression-m5d7o-layout`, `regression-m5d7pa-layout`,
  `regression-m5d7q0-layout`, `regression-m5d7n-layout`,
  `regression-m5d7n-layout-longrun`,
  `full-editmode-layout`, `full-playmode-layout`, and the channel-isolation
  focused pair `focused-editmode-canvas-channel-retry-r`; and log-only
  `resolution-capture-retry-p.log` and the Astra-authorized diagnostic recovery
  `resolution-capture-retry-q.log` plus the channel-isolation recovery
  `resolution-capture-retry-r.log`. These names are not executable authority
  after this addendum's approval and remain subject to Luna post-review.
  Historical blocker logs are immutable and are not overwritten. Same-process
  second-invocation and mutation results remain within
  `focused-editmode.xml/.log`. No other result name is admitted without an
  Astra contract amendment before it is produced.

The only admitted files under `resolution-captures/` are
`640x360.png`, `1280x720.png`, `1920x1080.png`, `2560x1440.png`,
`1366x768.png`, `3440x1440-ultrawide.png`, `720x1280-narrow.png`,
`639x359-below-minimum.png`, `resize-quarantine-frame0.png`,
`resize-quarantine-frame1.png`, and `manifest.json`. `manifest.json` has exact
top-level fields `schemaVersion` (integer `1`) and `captures` (array in the
filename order above). Every capture entry has only `file`, `outputWidth`,
`outputHeight`, `supported`, `integerScale`, `safeRect` (`left`, `bottom`,
`width`, `height`), `pointClickSuppressed`, `stateDigestSha256`,
`pngSha256`, and `assertionPassed`. Hashes are uppercase 64-hex SHA-256;
`assertionPassed` must be true. The two resize entries use the same chosen
post-resize output: a previously committed 640x360 viewport changes to
1280x720. They differ only in the required quarantine-frame state: frame 0 is
suppressed and frame 1 is the first stable eligible frame. The
automated layout/hit assertions, not the PNG alone, remain authoritative.

Everything else is forbidden, including the Q0 prefab, Q0/runtime dependency
sources and tests other than the one friend line, generated input/actions,
Packages, ProjectSettings/EditorBuildSettings, Bootstrap/gameplay scenes and
prefabs, profile/persistence, CIO/CUA/costume catalog/media, Run/gameplay/
camera/narrative, localization packages, import of the TMP Essential Resources
bundle or any of its members other than the exact two byte-locked shader files
above, any TMP Resources object other than the exact minimal settings asset,
any additional TMP shader, and effect executors.

## Required test sequence

1. Verify the font and shader source/license/hash evidence, package fingerprint,
   exact immutable prerequisites, `SystemInfo.maxTextureSize >= 2048`, and
   allowlist baseline.
2. Compile the new assembly against uGUI 2.6.0's included TMP assembly; run
   copy/glyph/package/static/shader-identity/compiler scope tests. The
   in-memory capacity fixture must reproduce exact 1024 Regular missing
   `하항했`, then prove empty missing sets for both 2048 faces without persisting
   fixture outputs.
3. Run focused EditMode builder/validator tests twice in one process and once
   in a fresh process, including all independent settings and shader drift,
   duplicate-name, import, material-reference, atlas dimension/point/padding/
   format/mipmap/count/multi/glyph/index mutations, pair-atomic failure cleanup,
   preflight-write mutations, exact raw-payload assertions, and same/fresh
   process byte preservation.
4. Run focused PlayMode real-Q0 startup/baseline, input, notice/intent,
   resolution, corruption, and teardown matrices.
5. Run direct M5D7O, M5D7P-A, M5D7Q0, and M5D7N regressions serially.
6. Run full EditMode and full PlayMode with XML-backed counts and zero failed,
   skipped, or inconclusive tests.
7. Run the proposed real-scene Editor capture tool twice in one process and
   once in a fresh process. It must pair every PNG with deterministic layout,
   hit, Korean-copy, TMP material/shader, and resize-quarantine assertions,
   reproduce identical PNG/manifest bytes, and preserve authored scene/prefab/
   font/settings/shader bytes before scope and final manifests are published.
8. Luna independently audits/reruns the bounded evidence and maps every
   `AC-M5D7QA-*`; Astra alone accepts integration and status.

No screenshot alone proves input or state behavior. Resolution captures must
be paired with deterministic layout/hit assertions. No filtered or reused full
suite satisfies `AC-M5D7QA-010` unless Luna records a bounded impact ruling and
Astra explicitly accepts it.

## Stop and rollback conditions

Stop before implementation while this contract is not Approved or font/shader/
atlas source, capacity, license, hash, or platform evidence is incomplete.
Stop if `SystemInfo.maxTextureSize < 2048`, if either 2048 face fails exact
population/validation, if a failed invocation leaves a durable partial pair,
or if correct work needs:

- a second router, generated-action subscriber, map owner, semantic source or
  cursor owner;
- any input module, virtual mouse, `Mouse.current`, polling, timestamp replay,
  or render-frame input queue;
- Q0 prefab/source behavior, generated input, package, project/build setting,
  profile, persistence, costume, gameplay, camera, or narrative changes;
- notification identity/prose reconstructed from the projected kind or a
  warning lost on activation/disable/destroy;
- a default, fallback, dynamic, OS-resolved, substituted, unlicensed, or
  unverified font;
- any change to the exact 138-glyph inventory, point size 90, padding 9,
  SDFAA, Static, Alpha8/no-mip, 2048x2048, one-atlas-per-face, multi-atlas false
  profile; any face-specific profile; or any adaptive retry/growth/shrink path;
- a shader/include edit, another TMP shader, package-cache runtime dependency,
  full/partial Essential Resources import beyond the two exact admitted source
  files, reflection/private-static shader injection, or a material whose shader
  is not the canonical project asset;
- fractional scaling, safe-frame-external interaction, truncated Korean copy,
  or missing required glyphs;
- Settings/Wardrobe UI, CIO/CUA wiring, placeholder media, scene transition,
  run/profile effect, or `Application.Quit`;
- runtime reflection/private-field strings, a repairing validator, an unnamed
  file, or nondeterministic prefab/scene bytes/GUIDs.

Rollback removes only the new presentation runtime/authoring/tests, menu
prefab, Hub scene, amended generated SDF/atlas/material assets, admitted font
and exact shader/license assets/evidence, exact friend/reference/index/register
lines, and this unit's evidence. A capacity-only rollback removes only SDF
targets created by the amended invocation and preserves source OTF/OFL, TMP
Settings, shader/include/license prerequisites, and every historical blocker
log. Rollback does not rewrite or regenerate Q0, InputRouter behavior,
M5D7O/P-A/P-B, packages, project settings, profile, costumes, gameplay assets,
or historical evidence.

## User-decision and participation record

- The user selected Korean-first UI on 2026-09-23, fixed the four primary
  labels, and selected Noto Sans CJK KR 2.004 Regular/Bold under SIL OFL 1.1.
- This contract fixes the three concise Korean notification strings above.
- Sol converted the verified seams and user decision into the bounded contract
  design. Luna pre-gated the amended contract at `PASS — P0=0, P1=0, P2=0`.
- Astra approved the original and TMP Settings-amended contract on 2026-09-23
  and retains final integration authority. Luna independently pre-gated the
  newly exposed Mobile SDF shader amendment at `PASS — P0=0, P1=0, P2=0`, and
  Astra approved that exact amendment on 2026-09-27.
- Sol bounded the shader amendment on 2026-09-27 without changing the product
  decision. Luna's shader-amendment pre-gate and Astra approval are complete;
  generated Unity hashes, all AC evidence, Luna post-review, and Astra final
  integration remain.
- Terra's subsequent real 1024x1024 generation miss (`하항했`) is preserved as
  implementation evidence. Sol bounded the 2048x2048 capacity amendment on
  2026-09-27; Luna independently pre-gated it at
  `PASS — P0=0, P1=0, P2=0`, and Astra approved the exact delta on the same
  date without revoking any prior approval history.
- The first two admitted 2048 builder logs remain immutable failure evidence:
  `builder-pass-c.log` records the pre-amendment capacity stop and
  `builder-fresh-process-2048.log` records a validator defect that treated
  uGUI's normal Static-mode clearing of `sourceFontFile` as malformed despite
  the preserved serialized `m_SourceFontFileGUID`. On 2026-09-27 Astra amended
  only the result allowlist to admit `builder-pass-d.log` for one corrected
  recovery run. This evidence-only name addition changes no product behavior,
  atlas profile, requirement, acceptance criterion, or prior result bytes.
- `builder-pass-d.log` then proved that its nominally successful invocation
  failed to embed the generated atlas textures and materials as durable font
  subassets; the resulting two approximately 76 KiB font roots, their metas,
  `HubMenuRoot.prefab` and meta, and `Hub.unity` and meta are invalid outputs
  of that exact invocation. On 2026-09-27 Astra authorized one bounded
  recovery: remove only those eight `builder-pass-d` outputs, add post-save
  reload/subasset validation, and record one corrected run in the new immutable
  `builder-pass-e.log`. Canonical font sources, TMP settings, shader/include,
  license, prior logs, Q0, and unrelated assets remain immutable.
- The first `focused-editmode.log` invocation combined Unity Test Framework's
  `-runTests` with `-quit`, exited after compilation before the test runner
  began, produced no XML, and executed zero tests. The log remains immutable
  failure evidence. On 2026-09-27 Astra admitted exactly
  `focused-editmode-retry.xml/.log` for the same approved suite with `-quit`
  omitted; the retry must prove XML-backed test counts and does not replace or
  reinterpret the zero-test launch.
- The first XML-backed `focused-playmode` run passed two of three tests and
  correctly rejected one test's use of `EditorSceneManager.OpenScene` during
  PlayMode. Its XML/log remain immutable failure evidence. Astra admitted
  exactly `focused-playmode-retry.xml/.log` for the corrected runtime-safe
  asset-presence assertion; the retry must execute the same approved behavior
  surface with zero failures and does not alter production assets.
- The first `regression-m5d7o.log` run remained CPU-active but made no log
  progress and produced no XML for seven minutes after entering the globally
  discovered `Unity.PerformanceTesting.Editor.TestRunBuilder` prebuild setup;
  no M5D7O test executed. Its headless Editor and same-run asset worker were
  terminated, and the immutable log is retained as hang evidence. Astra
  admitted one warm-cache retry under exactly
  `regression-m5d7o-retry.xml/.log`, with the same approved filter, no package
  or project-setting change, and the same seven-minute bounded timeout. A
  repeated prebuild hang is a stop condition rather than authority to disable
  or bypass the global prebuild setup.
- Historical XML from the unchanged M5D7O fixture subsequently proved that
  successful direct runs normally take approximately 14–15 minutes and remain
  silent between the global prebuild message and final XML. The two seven-
  minute terminations therefore remain immutable evidence of an undersized
  timeout, not proof of a prebuild hang. Astra admitted exactly
  `regression-m5d7o-longrun.xml/.log` with the unchanged approved filter and a
  25-minute timeout. While the same-run Editor remains responsive and CPU time
  advances, silence before that bound is expected; package, project settings,
  fixture code, and global prebuild behavior remain unchanged.
- No open user-facing decision remains. The exact 6 MiB raw atlas-payload
  increase is a technical implementation cost that preserves the selected
  Korean copy, typeface, quality, single-atlas/static policy, and runtime
  behavior.
- On 2026-09-27 the complete XML-backed suite passed, but the capture generator
  required by `AC-M5D7QA-007/010` was found absent. Astra bounded an
  Editor-only real-scene render proposal without changing product behavior and
  returned only the latest contract status to `Review`. Luna then closed all
  six P1 and two P2 findings and issued `PASS — P0=0, P1=0, P2=0`; Astra
  restored `Approved` on 2026-09-27. Terra capture implementation and Luna
  post-review remain pending and are required before `Verified`.
- The first `resolution-capture.log` invocation stopped at compilation before
  capture execution because the generator's nested `Capture` record collided
  with its static `Capture()` entrypoint (`CS0102`). The immutable failure log
  is preserved. On 2026-09-27 Astra admitted exactly
  `resolution-capture-retry.log` for one corrected run after renaming only the
  nested record to `CaptureSpec`; this evidence-only filename addition changes
  no product behavior, capture oracle, result set, requirement, or acceptance
  criterion.
- The corrected `resolution-capture-retry.log` invocation rendered and
  published the capture directory but then failed its Editor-state restoration
  because batchmode began with an empty `SceneSetup[]`, for which Unity rejects
  `RestoreSceneManagerSetup` because no active scene exists. That log and its
  unaccepted published set remain failure evidence. Astra admitted exactly
  `resolution-capture-retry-b.log` for one recovery that restores a prior scene
  setup only when the captured setup is non-empty and otherwise closes the
  in-memory Hub scene without attempting an impossible zero-scene restore.
  This changes no capture, runtime, product, REQ, or AC semantics; the recovery
  must republish and revalidate the complete set before it can be accepted.
- Adversarial review of the implementation exercised by
  `resolution-capture-retry-b.log` rejected that implementation at
  `BLOCKED — P0=0, P1=14, P2=2`; its log and capture set remain unaccepted
  failure evidence. Terra rewrote the generator and closed a further static
  `P1=5, P2=2` correction round. Luna independently pre-gated exact generator
  SHA-256
  `823D3E8562E32E3E80ADD7DB085B62FD8399F1B73BBA87D8C36B9C934B57DEDA`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `resolution-capture-retry-c.log` for this reviewed implementation's first
  real Unity execution. This is an implementation-evidence recovery only and
  changes no product behavior, capture semantics, REQ, or AC.
- `resolution-capture-retry-c.log` then stopped at compilation before invoking
  the generator because Unity 6000.6 required the exact
  `UnityEngine.TextCore.LowLevel.GlyphRenderMode` namespace and exposes scene
  dirtiness as `Scene.isDirty`, not `EditorSceneManager.IsSceneDirty`. Luna
  independently proved that exact generator SHA-256
  `5B26C2D99BEC3C4A6984848532271AC60E098DEBB1F7B9E6516930A3E08940AE`
  differs from the prior static-PASS bytes only by those two compatibility
  corrections and re-gated it at `PASS — P0=0, P1=0, P2=0`. Astra admitted
  exactly `resolution-capture-retry-d.log` for the corrected real execution
  and `resolution-capture-fresh.log` for the later clean-process byte-identity
  proof. Both names are evidence-only and change no product, capture, REQ, or
  AC semantics.
- `resolution-capture-retry-d.log` compiled and reloaded the exact Luna-R4-PASS
  generator but exited successfully before `-executeMethod` invocation, with
  no capture directory, exception, or compiler error. Astra admitted exactly
  `resolution-capture-retry-e.log` for the unchanged warm-cache execution.
  The immutable warm-up log remains evidence and this name addition changes no
  source, product, capture, REQ, or AC semantics.
- `resolution-capture-retry-e.log` reached the generator but stopped before
  PNG creation: its combined TMP preferred/text-bounds predicate failed without
  identifying the observation, and Unity rejected closing the final loaded Hub
  scene for an initially empty batchmode setup. Sol bounded exact local mesh/
  rect/TMP diagnostics and a terminal process-boundary no-save policy without
  changing product or runtime behavior. Luna pre-gated that exact amendment at
  `PASS — P0=0, P1=0, P2=0`, and Astra restored `Approved` on 2026-09-27.
  `resolution-capture-retry-f.log` is now admitted for the corrected run;
  implementation evidence, fresh-process proof, Luna post-review, and Astra
  final integration remain mandatory.
- `resolution-capture-retry-f.log` compiled and reloaded the exact
  Luna-approved blocker-corrected generator but exited successfully before
  `-executeMethod`, with no capture directory, marker, exception, or compiler
  error. Astra admitted exactly `resolution-capture-retry-g.log` for the
  unchanged warm-cache execution. This evidence-only name addition changes no
  source, product, capture, REQ, or AC semantics.
- `resolution-capture-retry-g.log` reached the generator and correctly failed
  before mutation because warmed Unity paired an empty `SceneSetup[]` with a
  nonzero scene state that the then-approved true-zero-only predicate rejected.
  Sol bounded a pre-predicate snapshot and exact clean unsaved zero-root
  bootstrap classification while preserving the same terminal no-save/process
  boundary. Luna independently pre-gated the addendum at
  `PASS — P0=0, P1=0, P2=0`, and Astra restored `Approved` on 2026-09-27.
  `resolution-capture-retry-h.log` is admitted for the corrected implementation;
  execution evidence, fresh proof, Luna post-review, and Astra integration
  remain mandatory.
- `resolution-capture-retry-h.log` stopped at compilation before entrypoint
  execution because Unity 6000.6 forbids the obsolete implicit conversion from
  `SceneHandle` to `int`. Terra changed only the bootstrap identity comparison
  to explicit `SceneHandle.GetRawData()` equality. Luna independently proved
  exact generator SHA-256
  `F271925AA5CCFA112C371352DFBC3B3EAA40276A82BF701F2C61FBA8B52CF021`
  preserves all prior behavior and re-gated it at
  `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `resolution-capture-retry-i.log` for that compatibility-corrected execution;
  this changes no product, capture, REQ, or AC semantics.
- `resolution-capture-retry-i.log` exposed the remaining implicit
  `SceneHandle` conversion when the active handle was stored in the diagnostic
  snapshot. Terra changed only that constructor argument to
  `active.handle.GetRawData()` and proved all three handle uses explicit. Luna
  independently re-gated exact generator SHA-256
  `601B0A73FEBCEB8FFB9B482D721FED8A0CA9BD649CE1864649E7EAB9865F6ADD`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `resolution-capture-retry-j.log`; the immutable compile-failure log remains
  evidence and no product, capture, REQ, or AC semantics change.
- `resolution-capture-retry-j.log` proved that Unity 6000.6
  `SceneHandle.GetRawData()` returns `ulong`, not `int`. Terra changed only the
  diagnostic snapshot's `ActiveHandle` field/constructor to `ulong` and its
  invalid sentinel to `0UL`. Luna independently re-gated exact generator
  SHA-256
  `E1A66E592982AEAA5F60DD1AC6CC1A8C5FA17BE4F73F85C3750D49CF9C440692`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `resolution-capture-retry-k.log`; this type-compatibility recovery changes no
  product, capture, REQ, or AC semantics.
- `resolution-capture-retry-k.log` passed terminal bootstrap classification but
  stopped at the fixed RenderTexture predicate before any capture because the
  combined assertion did not identify the differing runtime property. Terra
  added only a deterministic pre-assert requested/actual RenderTexture
  observation marker; Luna independently re-gated exact generator SHA-256
  `5561A418B351EC2AC4CC39C66772F467AABC5B256D021AE7AE30A29FEB1AB101`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `resolution-capture-retry-l.log` for this diagnostic execution; the predicate
  and all product/capture/REQ/AC semantics remain unchanged.
- `resolution-capture-retry-l.log` proved that Unity's implicit depth-bits
  request selected `D32_SFloat_S8_UInt` on D3D12. To preserve rather than relax
  the contract's depth-24 surface, Terra now preflights and explicitly requests
  `R8G8B8A8_SRGB` with `D24_UNorm_S8_UInt` before tmp creation/scene open and
  asserts the actual descriptor/depth. Luna independently re-gated exact
  generator SHA-256
  `18FA31363366275B0CCE2701D9E6EF36B0C9E5D98A2648A6C6E30955514DFC1F`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `resolution-capture-retry-m.log`; this strict-format recovery changes no
  product, capture, REQ, or AC semantics.
- `resolution-capture-retry-m.log` passed the exact depth-24 surface and then
  stopped at the combined Camera target assertion. Terra changed only the
  deprecated format-support overloads to `GraphicsFormatUsage.Render` and
  added a deterministic Camera rect/pixelRect/target-presence/equality
  observation before the unchanged predicate. Luna independently re-gated
  exact generator SHA-256
  `B1E49A32ECCD79D35FD2AA87F30B49819E7774F5FFECC241E0193E35AD172091`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `resolution-capture-retry-n.log`; this diagnostic/compatibility recovery
  changes no product, capture, REQ, or AC semantics.
- `resolution-capture-retry-n.log` proved that assigning `pixelRect` directly
  caused Unity to convert through the current display aspect, yielding a
  640x270 subrect on a 640x360 target. Terra now attaches the target first,
  sets normalized `Camera.rect` to the full unit rect, and asserts the derived
  pixel rect exactly matches the target without assigning it. Luna independently
  re-gated exact generator SHA-256
  `6368164FC84F1AA7B0E0F4A4C4F9C49A11E88E4DD829A07ACA00B01D7471E6CD`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `resolution-capture-retry-o.log`; this full-target correction changes no
  product, capture, REQ, or AC semantics.
- `resolution-capture-retry-o.log` passed terminal, exact depth-24 RT and full
  Camera target gates, then stopped before PNG creation because the Continue
  label reported preferred height `20.28` in its `168x20` rect and exact
  `OVERFLOW_FLAG,OVERFLOW_INDEX`. Sol bounded an all-four label correction from
  `(12,6,168,20)` to `(12,5,168,22)` that preserves the exact button and label
  center. Luna independently pre-gated the exact correction at
  `PASS — P0=0, P1=0, P2=0`, and Astra restored `Approved` on 2026-09-27.
  Terra migration/evidence, Luna post-review, and Astra final integration
  remain mandatory.
- The first label-layout migration attempt, `builder-pass-f.log`, stopped at
  compilation before its entrypoint because four test fixture calls used the
  ambiguous short type name `Object`. No authored asset changed. Terra
  qualified only those calls as `UnityEngine.Object`; Luna independently
  re-gated exact test SHA-256
  `71087281070DF4F4EF2C7251E4A1FDCFFB593FBF9B47050511B1D2741369BD09`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `builder-pass-g.log` for the corrected migration execution. The failure log
  remains immutable and no product, layout, REQ, or AC semantics change.
- `builder-pass-g.log` compiled and entered the migration but stopped before
  authored writes because the scene validator incorrectly required zero
  HubMenuRoot modifications, contradicting the already-approved exact scene
  `_latch`/`_router` references. Terra replaced that check with the exact
  serialized 23-row target/path/value/object-reference matrix while preserving
  Q0 zero overrides. Luna independently re-gated validator SHA-256
  `BD8E8B8F4110183D30B2E8A0B1AFFBBEBA9FCA9615C267036D2BE78676E17CF2`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `builder-pass-h.log`; no product, layout, REQ, or AC semantics change.
- `builder-pass-h.log` then stopped before authored writes because Unity also
  serializes eleven canonical Q0 instance-root name/transform defaults although
  they carry no semantic override. Terra replaced literal zero-count checking
  with the exact eleven-row prefab-source target/path/value/null-reference
  matrix and rejects all other modifications. Luna independently re-gated
  validator SHA-256
  `477420AF499066D311ADB008AF889D9A90691F2E162045F001326439CCC878CA`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `builder-pass-i.log`; Q0 behavior and all product/layout/REQ/AC semantics
  remain unchanged.
- `builder-pass-i.log` completed the exact four-label migration and
  `builder-fresh-process-layout.log` proved the fresh builder no-op with the
  scene and all metas/GUIDs unchanged. The first
  `focused-editmode-layout.log` invocation then performed only Unity's
  post-source-change assembly reload, exited zero, ran no tests, and produced
  no XML. Astra admitted exactly
  `focused-editmode-layout-retry.xml/.log` for the unchanged approved suite;
  the warm-up log remains immutable and is not test evidence.
- `focused-editmode-layout-retry.log` repeated the same known-invalid
  `-runTests -quit` launch shape and again exited after domain reload with no
  tests or XML. Astra admitted exactly
  `focused-editmode-layout-retry-b.xml/.log` for the unchanged suite with
  `-quit` omitted. All subsequent label-layout Test Runner invocations must
  omit `-quit`; both zero-test logs remain immutable failure evidence.
- `focused-editmode-layout-retry-b.xml/.log` passed 51/51. The first focused
  PlayMode layout run then passed 3/4 but its new geometry test incorrectly
  forced TMP mesh data on the AssetDatabase prefab object and observed zero
  characters. Terra changed only the fixture to instantiate an active prefab
  clone, force Canvas layout, preserve the real scene-instance branch and all
  strict assertions, and clean up in `finally`. Luna independently re-gated
  exact test SHA-256
  `903E570AC1F01AD9B5B038BF083ABE0E5D9F637E81F65741D3CBE74288E8F1E2`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `focused-playmode-layout-retry.xml/.log`; the 3/4 result remains immutable.
- `focused-playmode-layout-retry.xml/.log` again passed 3/4 but proved
  `EditorSceneManager.OpenScene` is forbidden during PlayMode. Terra rewrote
  only the real-scene fixture as a `UnityTest` using Unity 6000.6's verified
  `LoadSceneAsyncInPlayMode` additive path and guaranteed runtime unload while
  preserving all strict assertions. Luna independently re-gated exact test
  SHA-256
  `22FCB024EC59E60CEBAD176685EF12ABD06920DA525FBD43ADB2C78F3EC2690A`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `focused-playmode-layout-retry-b.xml/.log`; both earlier 3/4 results remain
  immutable failure evidence.
- `focused-playmode-layout-retry-b.xml/.log` loaded the exact Hub scene but
  passed 3/4 because PlayMode strips Editor prefab linkage, making the
  `PrefabUtility` path empty. Terra retained exact prefab provenance in the
  passing EditMode validator/mutation matrix and changed only the runtime
  branch to assert exact scene path, two active roots, required HubMenu
  components/EventSystem, no Camera/missing scripts, and all strict TMP rules.
  Luna independently re-gated exact test SHA-256
  `6A80228FDEF42941919B4DD0E4BD601EDEDF4A15FFC94BF8FB5EC758618E1966`
  at `PASS — P0=0, P1=0, P2=0`. Astra admitted exactly
  `focused-playmode-layout-retry-c.xml/.log`; the prior 3/4 result remains
  immutable failure evidence.
- The unchanged label-layout regression sequence passed the focused suites and
  M5D7O/P-A/Q0 runs, but `regression-m5d7n-layout.log` reached the 25-minute
  bound without XML while its responsive worker continued accumulating CPU.
  Historical unchanged M5D7N evidence establishes approximately 40–44 minutes
  as a normal successful duration. Astra admitted exactly
  `regression-m5d7n-layout-longrun.xml/.log` with the same filter, no source or
  setting change, `-quit` omitted, and a 60-minute bound. The early-termination
  log remains immutable timeout evidence.
- The long-run M5D7N recovery passed 69/69, the complete EditMode suite passed
  788/788, and the complete PlayMode suite passed 947/947. The subsequent
  `resolution-capture-retry-p.log` passed terminal bootstrap, exact depth-24
  RenderTexture, full Camera target, and all six strict TMP observations with
  `failures=NONE`, then stopped before PNG creation at the unchanged combined
  scene-layout oracle predicate. Terra added only an invariant, identity-free
  `M5D7QA_LAYOUT_OBSERVATION` immediately before that predicate; the predicate,
  render path, publication, restoration, product behavior, REQ, and AC
  semantics are unchanged. Astra admitted exactly
  `resolution-capture-retry-q.log` for the diagnostic execution of generator
  SHA-256
  `1B775BF2FB4011FEB2A57FEBAB3FF0F81D40C8A26BBD3AFCD597A0E5EF72F67F`,
  subject to Luna's exact-SHA static gate before execution. The retry-p log
  remains immutable failure evidence.
- Luna pre-gated the diagnostic-only generator SHA above at
  `PASS — P0=0, P1=0, P2=0`. `resolution-capture-retry-q.log` then proved the
  640x360 spec completed with all six TMP observations at `failures=NONE`,
  while its TMP mesh update changed `Canvas.additionalShaderChannels` from
  `None (0)` to exact `TexCoord1 | Normal | Tangent (25)`; because that
  transient mask was not reset, the 1280x720 pre-TMP layout assertion failed.
  No capture directory was published. Sol bounded the exact per-spec
  `None -> 25 -> 25 -> None` isolation amendment at proposal SHA-256
  `34D05E3E92668D97EB916C38FE770F1EB16AA80E22435C9ADCB3A479D916E600`,
  and Astra approved it on 2026-09-27. Terra must first produce the admitted
  focused EditMode channel suite and obtain Luna's exact-SHA pre-gate; only
  then may it execute `resolution-capture-retry-r.log`. Retry-q remains
  immutable failure evidence and no product, authored asset, manifest,
  requirement, or acceptance-criterion semantics change.

## Final integration — 2026-09-27

Terra completed the approved implementation and evidence boundary. The final
focused and regression results are 51/51 EditMode, 4/4 PlayMode, 19/19 M5D7O,
5/5 M5D7P-A, 4/4 M5D7Q0, and 69/69 M5D7N; the complete suites pass 788/788
EditMode and 947/947 PlayMode. The channel-isolation focused suite passes
61/61. Both `resolution-capture-retry-r.log` and a separate clean-process
`resolution-capture-fresh.log` prove the exact 80 Canvas-channel and 120 TMP
observations, same-process equality, fresh-process equality, restoration, and
an exact ten-PNG plus canonical-manifest allowlist with no `.tmp` or `.prev`.

Luna independently reviewed the implementation, XML/log evidence, scope and
final manifest, all `REQ-M5D7QA-001..010` to `AC-M5D7QA-001..010` mappings,
and every resolution capture visually. The final post-review is
`PASS — P0=0, P1=0, P2=0` at
`docs/verification/2026-09-27-vd09-m5d7q-a-final-luna-postreview.md`, SHA-256
`B73F2AA693244B141244EE2F34EB12780DB059674BE057A07F5ED191019F3E67`.
No technical blocker or user product decision remains. Astra therefore marks
this work contract `Verified` on 2026-09-27. Historical failed and recovery
evidence remains immutable and is not reclassified.
