# VD-09 M5D7Q authored uGUI hub presentation — Sol proposal

- Status: Draft; not implementation authority
- Date: 2026-09-23
- Design/counter-review: Sol (`gpt-5.6-sol`)
- Final contract owner and approval authority: Astra
- Intended implementation/review after approval: Terra / Luna
- Dependencies: M5D7O, M5D7P-A, M5D7P-B, and M5D7Q0 Verified
- Proposed slice: M5D7Q-A first authored hub shell

## Bounded outcome

Author one `Hub.unity` scene and one reusable `HubMenuRoot.prefab` that compose
the verified Q0 `HubRuntimeRoot.prefab` with an actual uGUI/TextMeshPro menu.
One presentation owner takes the M5D7N handoff and optional typed notification,
creates the M5D7O controller, creates and advances exactly one M5D7P-A cursor,
and renders the approved Continue / New Game / Settings / Quit shell.

This first shell proves presentation, focus, pointer, notification, scaling,
and authoring behavior. It takes and retains at most one M5D7O intent, then
locks the menu. It does **not** execute that intent. It does not load gameplay,
start a run, create a profile, open Settings, open Wardrobe, quit the
application, or alter a scene/build list. Those effects require later approved
owners.

No second generated-action subscriber, map owner, semantic-frame source,
`InputSystemUIInputModule`, `StandaloneInputModule`, `PlayerInput`, virtual
mouse, input polling, or render-`Update` input queue is permitted.

## Ownership and interfaces

Add a dedicated `AcadeGameMaker.Hub.Presentation.Unity` runtime assembly. Its
single lifecycle/input owner is `HubMenuPresenterV1` at execution order `-180`,
after Q0 `InputRouter` `-210` and M5D7N latch `-190`.

The presenter has exact serialized references to the scene instances of:

- the Q0 `InputRouter` and `HubEntryHandoffLatchV1`;
- the Canvas safe-frame scaler and one inert `EventSystem`;
- the four button views and notification view;
- every required TMP label and authored hit rectangle.

The Input.Unity assembly grants only exact friend access to this assembly. No
public API or receipt schema is widened. The presenter owns:

- one `UiSemanticFrameCursorV1`, created once against the exact Q0 router;
- one `HubMenuPresentationControllerV1`;
- exact copied handoff and optional `ProfileLaunchNotificationV1` values plus
  equality proofs, so notification identity/correlation is never reconstructed
  from `NotificationKind` or displayed prose;
- current focus, pointer-hover target, stable viewport row, presentation state,
  and one retained `HubMenuIntentV1` after taking it from M5D7O.

The presenter owns no action, map, profile, persistence, costume, run,
gameplay, scene, or application authority. Buttons have no persistent
`onClick` listener; `EventSystem` has no `BaseInputModule`. The presenter calls
the M5D7O controller directly after interpreting a delivered semantic frame.

### Initialization and fixed-step flow

1. `Awake` validates exact authored references/topology and leaves every
   control non-interactive.
2. In `Update`, after the latch's first publication, the presenter reads the
   exact handoff, performs the latch's one authorized notification take, and
   passes that same optional typed value to
   `HubMenuPresentationControllerV1.Create`.
3. It creates one cursor against the exact router. A current UI receipt becomes
   a non-delivered baseline. If no receipt exists, the owner remains locked
   until the cursor observes its deliberate baseline.
4. Controls become interactive only when the controller is Ready, the cursor
   is `Ready`, the source is healthy, and a valid stable viewport exists.
5. Only `FixedUpdate` advances the cursor. It runs after router commit and may
   process one consecutive delivered frame. `Update` renders already-owned
   state but never polls or consumes input.
6. One accepted menu activation is immediately taken from M5D7O into the
   presenter's immutable retained-intent slot. Menu controls lock permanently;
   this slice exposes no executor callback and performs no effect.

If an intent is retained while a notice is visible, the notice remains visible
and may still be dismissed through its approved Click/Submit paths. The scene
and Canvas remain present. A later executor contract must either retain this
root or transfer the exact typed/correlated notice before unloading it; it may
not infer a warning from an enum or string.

## Exact authored topology

`Assets/Scenes/Hub.unity` has exactly two active root prefab instances and no
Camera or gameplay object:

```text
Hub.unity
├─ HubRuntimeRoot        (unmodified canonical Q0 prefab instance)
└─ HubMenuRoot           (M5D7Q prefab instance)
```

The scene overrides only the presenter's exact router/latch references to the
Q0 sibling instance. The Q0 source prefab receives no override or edit.

```text
HubMenuRoot
├─ [RectTransform, Canvas(ScreenSpaceOverlay),
│   CanvasScaler(ConstantPixelSize=1), HubSafeFrameScalerV1,
│   HubMenuPresenterV1]
├─ EventSystem
│  └─ [EventSystem only; no BaseInputModule]
├─ OutputBackdrop
│  └─ [Image, full output, non-interactive W01 #090D12]
└─ SafeFrame (640x360, centered, integer-scaled)
   ├─ Surface [Image, x=0 y=0 w=640 h=360]
   ├─ MenuPanel [x=48 y=86 w=224 h=188]
  │  ├─ Continue / `계속하기` [x=16 y=146 w=192 h=32]
   │  │  └─ Label [TMP, 14 logical px]
  │  ├─ NewGame / `새 게임` [x=16 y=106 w=192 h=32]
   │  │  └─ Label [TMP, 14 logical px]
  │  ├─ Settings / `설정` [x=16 y=66  w=192 h=32]
   │  │  └─ Label [TMP, 14 logical px]
  │  └─ Quit / `종료` [x=16 y=26  w=192 h=32]
   │     └─ Label [TMP, 14 logical px]
   └─ Notification [x=372 y=260 w=256 h=88]
      ├─ Message [x=12 y=12 w=196 h=64, TMP, 12 logical px]
      └─ Dismiss [x=220 y=52 w=24 h=24, TMP/Image]
```

All coordinates are bottom-left logical safe-frame coordinates. Hit rectangles
equal the authored button/dismiss rectangles and never extend outside the
640x360 frame. Navigation on every uGUI `Selectable` is `None`, transitions
are presentation-owner driven, and persistent UnityEvents are empty.

The visual vocabulary uses world neutrals for surfaces and a shape-plus-value
focus treatment: a persistent 2 logical-pixel left rail and full border for
focus, a distinct 1-pixel hover border, and a strike/low-contrast treatment for
disabled Continue. No state is color-only, no element blinks, and heroine
identity colors are not reused as generic interaction colors.

## Resolution, text, and accessibility behavior

`HubSafeFrameScalerV1` computes
`scale=floor(min(outputWidth/640, outputHeight/360))`. For supported output it
sets a centered 640x360 root at exactly that integer scale. Thus 640x360,
1280x720, 1920x1080, and 2560x1440 are 1x/2x/3x/4x; 1366x768 and non-16:9
outputs retain a centered maximum-integer frame with inert letter/pillar bars.
No stretch, crop, fractional layout scale, extra UI, or interactive bar area is
allowed.

Outputs below 640x360 suspend all interaction without changing controller,
cursor, focus, notice, or intent state. On any output-size change, the layout
updates immediately but Point/Click are suppressed for that delivered frame
and until one later consecutive frame under the new stable viewport. This
prevents a click captured in old actual-pixel coordinates from being hit-tested
against a new rectangle. Focus navigation may resume only when the viewport is
valid.

Every TMP component uses explicitly assigned project-owned static SDF assets
generated from **Noto Sans CJK KR 2.004 Regular and Bold**, licensed under SIL
Open Font License 1.1. No default/fallback discovery, dynamic OS font lookup,
runtime atlas population, or missing-glyph substitution is allowed. Before
approval, evidence must record the authoritative source URL, final download
URL, version, exact source-file names and byte sizes, SHA-256 for both font
files and the OFL text, license obligations, acquisition date, generated glyph
set, and atlas/import settings. Repository-copy GUIDs and hashes plus generated
SDF/atlas/material hashes are post-implementation acceptance evidence; they
cannot be asserted before the Approved implementation creates them. Text roles remain
12/14/18 logical pixels; this shell uses Regular 14 for menu labels, Regular
12 for notice copy, and Bold only for authored emphasis. TMP renders at final
output through the integer-scaled Canvas; non-text pixel decoration uses exact
logical rectangles. Button/dismiss hit areas are at least 24x24 and all
content maintains the 12-pixel safe-frame margin.

The focus rail/border, disabled strike, and notice dismissal target are visible
at 1x. Text does not truncate or overlap at 640x360. The OS cursor remains the
ordinary visible, unlocked UI cursor for this active root; no custom or gamepad
cursor is created. Gamepad operation is focus-only.

## Semantic input behavior

Actual-pixel point coordinates are accepted only inside the exact centered
gameplay/UI rectangle: `left <= x < right`, `bottom <= y < top`. Logical hit
testing uses integer division by the current integer scale. Negative,
off-window, letterbox, and pillarbox points clear hover; their clicks do
nothing.

- Initial focus is exactly the M5D7O view's Continue or New Game choice.
- Focus traverses only interactable root items in menu order, clamps at each
  end, and never wraps. X navigation and Scroll have no effect in this shell.
- A Y navigation change moves at most one item when the final absolute Q4096
  value is at least 2048. Zero/release and sub-threshold values do nothing.
- Focus persists across pointer movement. A later valid `PointChanged` owns
  hover visuals only; it does not silently move focus. A valid button click
  moves focus to that interactable button immediately before activation.
- Disabled Continue may hover but cannot take focus or activate.
- `Cancel` at this HubUIOnly top level is a no-op. It cannot request Gameplay
  or close the application. A future Settings child may define one-level-back
  behavior while retaining this same input owner.
- At most one M5D7O command is attempted per delivered frame. Navigation is
  applied first. Command priority is: notice Dismiss click; visible-notice
  Submit dismissal; interactable menu Click; focused-item Submit. This closes
  simultaneous Click/Submit ambiguity.
- Click dismissal requires the click-time point to hit the authored 24x24
  notice dismissal target. Submit dismisses a visible notice regardless of
  menu focus and consumes that Submit edge. No timer, Cancel, navigation,
  menu activation, intent take, disable, or scene lifecycle dismisses it.
- Menu Click while a notice is visible may retain an intent, but does not
  dismiss or hide the notice. After intent retention, menu navigation and
  activation remain locked; only notice dismissal remains active.

The frame's callback ordering is never reconstructed. Only its published
coalesced values and the closed priority above are used.

## Notification presentation and correlation

The presenter derives a display message token once from the exact validated
handoff plus exact optional `ProfileLaunchNotificationV1`, stores both values
and proofs, and verifies the same correlation before every display/dismiss
command. It never uses `HubMenuViewV1.NotificationKind` alone to recreate the
payload or choose prose. The view's `Visible/Dismissed/Absent` state controls
visibility; the controller alone authorizes dismissal.

There is no automatic timeout. The notification remains top-right and
non-modal. The Korean-first message mapping is exact:

| Typed notification | Primary displayed copy |
|---|---|
| `RecoveryCompleted` | `프로필 복구를 완료했습니다.` |
| `RecoveryArtifactPreservationFailed` | `프로필은 복구했지만 손상 파일을 보존하지 못했습니다.` |
| `PersistenceDeferred` | `프로필을 불러왔지만 변경 사항을 저장하지 못했습니다.` |

The four exact primary menu labels are `계속하기`, `새 게임`, `설정`, and
`종료`. Implementation may not substitute programmer enum names, alternate
copy, an English fallback, or TMP's default font.

## Wardrobe integration boundary

This slice authors only the approved root `Settings` action. It does not
author a Settings submenu, Wardrobe row, costume storage root, CIO service,
portrait target, preview, gameplay renderer target, or CUA port.

The future player-visible costume composition begins only after:

1. CIO `AC-CIO-007` is accepted and CUA is Approved/Verified;
2. at least one real complete media package is Accepted, or the view is
   explicitly limited to the truthful 36-row pending/`NoAcceptedDefault`
   state with no selectable fallback;
3. a later contract names the exact storage root, recovered state, catalog,
   completed-action publisher, portrait target, gameplay renderer, and scene/
   prefab files; and
4. the same `HubMenuPresenterV1` (or an approved successor replacing it) keeps
   the single cursor/input-owner role. Wardrobe may not add another cursor,
   router subscriber, EventSystem input module, or direct action callback.

Until then, retained `HubMenuItemV1.Settings` is only a typed boundary. No
placeholder/concept image is presented as wearable, and highlighting cannot
unlock, select, save, bind, or preview-commit a costume.

## Failure and teardown

The presenter has closed states `AwaitingHandoff`, `AwaitingBaseline`, `Ready`,
`IntentRetained`, `Failed`, and `Closed`. Invalid topology, foreign router/
latch, notification mismatch, second cursor/controller creation, cursor skip/
replay/close/failure, source fault, malformed view, viewport proof corruption,
or reflected presenter-state corruption latches `Failed` before a controller
command or intent publication.

Failure clears hover/selection and disables all menu/dismiss interaction while
preserving the last complete visual, exact typed notice, controller evidence,
and retained intent for diagnosis. It does not normalize or replace forensic
state, request a mode, disable a map, dispose actions, dismiss a notice, invoke
an effect, or rebuild a controller/cursor. There is no retry/reopen path.

`OnDisable`/`OnDestroy` clears the local EventSystem selection and closes only
presentation-owned references/state. It never takes a notification late,
dismisses one, consumes another frame, changes a map, or disposes the adopted
actions. Q0 remains the action-owner teardown authority. Disable/destroy before
handoff or baseline publishes no controller command or intent.

## Deterministic authoring and validator expectations

Extend the existing Hub Authoring Editor assembly with a separate
`HubPresentationAuthoringBuilder` and observational
`HubPresentationAuthoringValidator`.

- The builder writes/replaces only the exact menu prefab and Hub scene. Valid
  assets are byte-for-byte no-ops. Repair uses canonical LF UTF-8 YAML with
  fixed local IDs and stable explicit references; it does not rely on random
  `SaveAsPrefabAsset` local IDs.
- A second build in the same and a fresh Unity process preserves prefab/scene
  bytes and all existing folder/asset `.meta` GUIDs.
- The builder uses typed assignment-only seams for project components. It does
  not use runtime reflection/private-field strings, execute lifecycle code,
  instantiate input actions, read profiles, take notifications, or repair the
  Q0 prefab.
- The validator never writes. It checks exact scene roots/prefab sources,
  absence of Q0 overrides, hierarchy/component order, execution order,
  references, Canvas/scaler/EventSystem configuration, absence of every input
  module/raycaster listener, layout rectangles, TMP/font/material assignment,
  text sizes/wrapping/overflow, empty UnityEvents, palette values, selectable
  navigation, no missing scripts, no prefab override, and no extra child.
- It also rejects a Camera, gameplay component, scene executor, costume/CUA
  component, custom cursor, direct input callback owner, or safe-frame-external
  interactive Graphic.

## Proposed implementation allowlist

The contract should name only:

- new `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/` asmdef,
  `HubMenuPresenterV1.cs`, `HubSafeFrameScalerV1.cs`, presentation view helper,
  and their `.meta` files;
- one exact friend-assembly line in
  `Assets/AcadeGameMaker/Runtime/Input/Unity/AssemblyInfo.cs`;
- minimal reference additions to the existing Hub Authoring Editor asmdef and
  new builder/validator source plus `.meta` files;
- `Assets/Prefabs/Hub/HubMenuRoot.prefab` and `.meta`;
- `Assets/Scenes/Hub.unity` and `.meta`;
- exact Noto Sans CJK KR 2.004 source-font, static TMP SDF asset/material, SIL
  OFL 1.1 license, asset-register, and evidence paths under the contract's
  fixed `Assets/UI/Fonts/Hub/` and documentation allowlist;
- new focused EditMode authoring/static tests and PlayMode presentation tests,
  their asmdefs and `.meta` files;
- this proposal, one future Approved contract, Terra evidence, Luna pre/post
  reviews, GPT participation ledger, and minimal indexes.

Explicitly excluded are the Q0 prefab/source/tests, generated input and action
asset, Packages/manifest/lock, ProjectSettings and EditorBuildSettings,
Bootstrap and gameplay scenes/prefabs, M5D7O behavior, profile/persistence,
CIO/CUA/costume catalog/media, Run/gameplay/camera/narrative, and any effect
executor.

## Requirement and acceptance candidates

- **REQ-M5D7Q-001:** compose the unmodified Q0 runtime prefab and one exact
  authored menu prefab in a two-root Hub scene.
- **REQ-M5D7Q-002:** keep one presenter as the sole cursor/controller/input
  interpretation owner and consume semantic frames only after fixed commit.
- **REQ-M5D7Q-003:** project the exact M5D7O menu order, availability, initial
  focus, notice anchor/state, and one retained intent without executing it.
- **REQ-M5D7Q-004:** preserve exact typed notification/receipt correlation and
  manual-only Click/Submit dismissal with no visible-warning loss.
- **REQ-M5D7Q-005:** implement the closed navigation, hover, click, simultaneous
  edge priority, safe-frame rejection, and no-virtual-mouse rules above.
- **REQ-M5D7Q-006:** preserve 640x360 logical layout and integer scaling through
  2560x1440 baseline/non-16:9 outputs with approved text/hit/margin minima.
- **REQ-M5D7Q-007:** use only uGUI 2.6.0's included TMP assembly and exact
  project-owned SDF/font assets; do not add the deprecated TMP shim.
- **REQ-M5D7Q-008:** fail closed and preserve owner boundaries on lifecycle,
  source, viewport, authoring, and reflected-corruption failures.
- **REQ-M5D7Q-009:** deterministically build and observationally validate the
  exact prefab/scene without changing Q0 or unrelated assets.
- **REQ-M5D7Q-010:** stop at the Settings/wardrobe and all menu-effect
  boundaries; add no scene transition, Quit, profile, costume, or gameplay
  effect authority.

Candidate acceptance:

- **AC-M5D7Q-001:** editor validation proves exact two-root composition,
  unmodified Q0 instance, exact UI hierarchy/references, and forbidden-owner
  absence; same/fresh-process builder runs preserve bytes and GUIDs.
- **AC-M5D7Q-002:** real Q0 startup for Primary/Previous/Default and all notice
  kinds/absence creates one controller and one cursor, stays locked through
  deliberate baseline, and delivers each consecutive frame once.
- **AC-M5D7Q-003:** keyboard, mouse, and XInput semantic-frame scripts prove
  exact focus order/skip/clamp, hover precedence, click-time hit testing,
  simultaneous-edge priority, disabled Continue, top-level Cancel no-op, and
  no second subscriber/module/queue.
- **AC-M5D7Q-004:** each menu item retains one exact intent and locks without
  executing an effect; duplicate frames/activation/take cannot replace it.
- **AC-M5D7Q-005:** all notice kinds and absence prove exact correlation,
  top-right placement, Click/Submit-only dismissal, no timer/lifecycle/menu
  dismissal, and continuity after intent retention.
- **AC-M5D7Q-006:** automated layout/hit tests and captures at 640x360,
  1280x720, 1920x1080, 2560x1440, 1366x768, ultrawide, and narrow outputs prove
  integer framing, inert bars, one-frame resize pointer quarantine, readable
  TMP, non-color-only state, and 24/12 minima.
- **AC-M5D7Q-007:** corruption/failure and every disable/destroy boundary prove
  no late frame/notification take, dismissal, intent, effect, map mutation, or
  action disposal by the presenter.
- **AC-M5D7Q-008:** static package/assembly/font audit proves uGUI 2.6.0,
  included `Unity.TextMeshPro`, no shim/default font/runtime atlas/input module,
  and complete font-license evidence.
- **AC-M5D7Q-009:** focused EditMode and PlayMode, direct M5D7O/P-A/Q0/M5D7N
  regressions, then full EditMode and PlayMode finish with failure/skip/
  inconclusive zero; Luna reports `P0=0` and `P1=0`.

| Requirement | Candidate evidence |
|---|---|
| `REQ-M5D7Q-001` | `AC-M5D7Q-001`, `AC-M5D7Q-009` |
| `REQ-M5D7Q-002` | `AC-M5D7Q-002`, `AC-M5D7Q-003`, `AC-M5D7Q-007` |
| `REQ-M5D7Q-003` | `AC-M5D7Q-002`, `AC-M5D7Q-004` |
| `REQ-M5D7Q-004` | `AC-M5D7Q-005`, `AC-M5D7Q-007` |
| `REQ-M5D7Q-005` | `AC-M5D7Q-003`, `AC-M5D7Q-006` |
| `REQ-M5D7Q-006` | `AC-M5D7Q-006`, `AC-M5D7Q-008` |
| `REQ-M5D7Q-007` | `AC-M5D7Q-008`, `AC-M5D7Q-009` |
| `REQ-M5D7Q-008` | `AC-M5D7Q-007`, `AC-M5D7Q-009` |
| `REQ-M5D7Q-009` | `AC-M5D7Q-001`, `AC-M5D7Q-009` |
| `REQ-M5D7Q-010` | `AC-M5D7Q-004`, `AC-M5D7Q-007` plus static audit |

## Phased verification

1. Luna pre-gates the exact contract and font-acquisition/evidence plan.
2. Terra runs package/assembly/font compile smoke and focused pure mapping tests.
3. Terra runs deterministic builder/validator and mutation tests in two Unity
   processes.
4. Terra runs real Q0-lifecycle presentation PlayMode tests, input matrices,
   failure/teardown cases, and resolution capture checks.
5. Direct M5D7O, P-A, Q0, and M5D7N regressions run before full suites.
6. Luna independently repeats adversarial authoring/runtime checks and audits
   every candidate AC; Astra alone approves and integrates.

## Stop conditions

Stop if implementation needs a second input owner/cursor, an input module,
render-frame consumption, Q0 changes, notification reconstruction, fractional
safe-frame scaling, a default/dynamic font, an unlicensed or unverified font,
a visible warning teardown, a wardrobe placeholder, or any menu/gameplay/
application effect. Stop on any unnamed file or any inability to keep the Q0
prefab byte-stable.

## User-decision record

The user resolved the sole gate on 2026-09-23: Korean-first UI; exact primary
labels and notification copy are the strings recorded above; Noto Sans CJK KR
2.004 Regular/Bold under SIL OFL 1.1 is the static TMP source. Font source,
license, SHA-256, glyph-set, atlas-setting, and generated-asset evidence remain
an approval prerequisite, not an open product decision. No user-facing
decision remains.
