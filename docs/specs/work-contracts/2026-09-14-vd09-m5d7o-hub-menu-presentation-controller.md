# VD-09 M5D7O hub-menu presentation controller

- Status: Verified
- Owner: Astra
- Architecture counter-review: Sol
- Implementer: Terra after Approved
- Independent reviewer: Luna
- Dependencies: M5D7N Verified
- Parent requirements: `REQ-PLAT-002`, `REQ-PLAT-004`, `REQ-PLAT-006`, `REQ-UX-009`, `REQ-UX-014`
- Proposed acceptance IDs: `AC-M5D7O-001` through `AC-M5D7O-008`
- User decisions: independent Hub scene plus reusable root prefab; uGUI plus TextMeshPro; menu order Continue, New Game, Settings, Quit; initial focus Continue only for a valid persisted profile, otherwise New Game; notification at top-right; persistence warnings dismiss only by Click or UI Submit; 2560x1440 output baseline, 640x360 logical and minimum frame.
- Astra approval: Approved on 2026-09-14 after M5D7N reached Verified and Luna's amended independent pre-gate reported PASS (`P0=0`, `P1=0`, residual `P2=1`). The residual P2 requires the later M5D7P owner to retain controller-owned typed notification state rather than infer receipt identity or prose from the projected kind.
- Astra integration: Verified on 2026-09-20 after Terra recorded the final implementation evidence and Luna independently reported PASS (`P0=0`, `P1=0`, residual `P2=1`). Final XML-backed runs passed focused M5D7O EditMode `19/19`, direct M5D7N PlayMode `69/69`, full EditMode `688/688`, and full PlayMode `803/803`, each with failed/skipped/inconclusive `0`. The residual P2 is carried forward as a mandatory M5D7P ownership constraint and does not block this controller.

## Purpose and boundary

`HubMenuPresentationControllerV1` deterministically projects one valid M5D7N handoff and its already-transferred optional notification into an immutable hub-menu view and at most one one-shot menu intent. This bounded core fixes the semantic meaning of **Continue** as profile continuation: it preserves prior confirmed profile state while beginning from the hub/new-expedition flow and never resumes an active expedition.

This unit does not create, find, load, activate, render, or destroy scenes, prefabs, Canvas, controls, TMP text, EventSystem, or input actions; take a notification from M5D7N; localize prose; run timers; read files; modify profile, persistence, router, maps, settings, run, gameplay, or narrative state; or execute Quit/New Game/Continue/Settings effects. M5D7P owns authored presentation only after the UI semantic-input seam and compatible uGUI/TMP package version are separately Approved.

## API

The implementation remains in the existing `AcadeGameMaker.Input.Unity` assembly but its controller/value types must not depend on UnityEngine APIs.

```csharp
internal enum HubMenuItemV1 { Continue = 1, NewGame = 2, Settings = 3, Quit = 4 }
internal enum HubNoticeDismissalV1 { Click = 1, Submit = 2 }
internal enum HubMenuControllerStateV1 { Ready = 1, IntentPending = 2, IntentConsumed = 3, Failed = 4 }
internal enum HubNoticeStateV1 { Absent = 1, Visible = 2, Dismissed = 3 }
internal enum HubNotificationAnchorV1 { TopRight = 1 }

internal readonly struct HubMenuViewV1
{
    internal bool HasPersistedValidProfile { get; }
    internal bool ContinueInteractable { get; }
    internal HubMenuItemV1 InitialFocus { get; }
    internal HubMenuItemV1 GetMenuItem(int index);
    internal bool IsInteractable(HubMenuItemV1 item);
    internal ProfileLaunchNotificationKindV1? NotificationKind { get; }
    internal HubNoticeStateV1 NotificationState { get; }
    internal HubNotificationAnchorV1 NotificationAnchor { get; }
    internal int LogicalWidth { get; }        // 640
    internal int LogicalHeight { get; }       // 360
    internal int MinimumBodyPx { get; }       // 12
    internal int StandardBodyPx { get; }      // 14
    internal int HeadingPx { get; }           // 18
    internal int MinimumHitPx { get; }        // 24
    internal int MinimumEdgeMarginPx { get; } // 12
}

internal readonly struct HubMenuIntentV1
{
    internal HubMenuItemV1 Item { get; }
}

internal sealed class HubMenuPresentationControllerV1
{
    internal static HubMenuPresentationControllerV1 Create(
        HubEntryHandoffReceiptV1 handoff,
        ProfileLaunchNotificationV1? notification);
    internal HubMenuControllerStateV1 State { get; }
    internal HubMenuViewV1 CurrentView { get; }
    internal bool TryDismissNotification(HubNoticeDismissalV1 source);
    internal bool TryActivate(HubMenuItemV1 item);
    internal bool TryTakeIntent(out HubMenuIntentV1 intent);
}
```

`Create` validates the handoff and, when present, the exact notification kind and receipt correlation once before defensive-copy construction. M5D7P will be the lifecycle owner that performs M5D7N's one authorized notification take and passes the transferred value into this core.

## Projection and state

`HasPersistedValidProfile` is true if and only if `handoff.LaunchReceipt.Source` is `ProfileLaunchSource.Primary` or `ProfileLaunchSource.Previous`. It is false for `Default`, even when default bootstrap was successfully persisted. Persistence outcome, committed revision, filesystem state, and repair kind cannot promote or demote this predicate. A Primary/Previous source repaired during launch still represents a persisted profile.

The menu order is always Continue, New Game, Settings, Quit. `GetMenuItem` uses zero-based indices `0..3` in that exact order; any other index throws `ArgumentOutOfRangeException` and cannot alter controller state. Continue remains visible but is interactable only when `HasPersistedValidProfile` is true. New Game, Settings, and Quit are always interactable while the controller is Ready. `IsInteractable` returns those exact values and throws `ArgumentOutOfRangeException` for an unknown item without changing controller state. Initial focus is Continue when interactable and otherwise New Game.

Controller state is `Ready -> IntentPending -> IntentConsumed`; any malformed closed state, backing value, receipt, or correlation latches `Failed`. One valid activation in Ready publishes exactly one immutable intent. A disabled Continue activation returns false without changing state. Duplicate activation cannot replace or add an intent. `TryTakeIntent` succeeds once from IntentPending, then later calls return false/default without changing the consumed result. An unknown `HubMenuItemV1` passed to `TryActivate` and an unknown `HubNoticeDismissalV1` passed to `TryDismissNotification` each latch Failed and throw `ArgumentOutOfRangeException`; they are programmer/protocol faults, not ordinary false outcomes.

Notification state is orthogonal: `Absent`, or `Visible -> Dismissed`. All three typed notification kinds are manual-only in this core. Only `Click` and `Submit` are accepted dismissal sources. Menu activation, intent take, navigation, cancel, elapsed time, and controller lifecycle cannot dismiss it. No timer API exists. A visible warning cannot be silently discarded by a later scene transition; that remains a stop condition for the M5D7P/downstream contract.

The immutable view carries the authored presentation constraints: 640x360 logical frame, 12/14/18 logical-pixel text roles, minimum 24x24 logical-pixel hit area, minimum 12 logical-pixel safe-frame margin, and the closed `HubNotificationAnchorV1.TopRight` notification anchor. The anchor remains TopRight even when notification state is Absent so later authored layout has one invariant placement policy. It does not calculate an output scale or own visual assets.

## Requirements

- **REQ-M5D7O-001:** accept only a valid M5D7N handoff and an absent or exact-correlated optional notification, then defensively publish one immutable hub view.
- **REQ-M5D7O-002:** define a persisted valid profile exclusively as a Primary or Previous launch source; Default remains New Game regardless of bootstrap save result.
- **REQ-M5D7O-003:** expose the exact zero-based Continue/New Game/Settings/Quit order, keep Continue visible-disabled when unavailable, keep the other three items interactable in Ready, choose initial focus from Continue availability, and reject unknown indices/items without state ambiguity.
- **REQ-M5D7O-004:** preserve the approved 640x360 logical frame, 12/14/18 text sizes, 24x24 minimum hit area, 12-pixel minimum edge margin, and `TopRight` notification anchor as immutable view data.
- **REQ-M5D7O-005:** own an optional typed notification independently of menu intent, allow dismissal only by Click or Submit, and provide no timer or automatic-dismiss path.
- **REQ-M5D7O-006:** publish at most one exact menu intent per controller session and allow that intent to be taken at most once.
- **REQ-M5D7O-007:** fail closed on malformed handoff, notification correlation, enum, immutable value, or controller state without normalizing or replacing evidence.
- **REQ-M5D7O-008:** add no profile/persistence/router/map/input-action/run/gameplay/narrative/scene/UI/rendering/package/file/network/RNG/logging/Application.Quit authority.

## Acceptance criteria

- **AC-M5D7O-001:** Primary, Previous, and Default sources across clean, recovery, failed-save, preservation-denied, and commit-uncertain receipts produce the exact persisted predicate and initial focus without consulting save outcome or committed revision.
- **AC-M5D7O-002:** indices 0..3 return Continue/New Game/Settings/Quit exactly; negative and 4-or-greater indices throw `ArgumentOutOfRangeException` without controller mutation; Default keeps Continue visible-disabled while New Game/Settings/Quit are interactable, and disabled Continue activation returns false with state/view unchanged.
- **AC-M5D7O-003:** each interactable item publishes one exact intent; duplicate activation cannot replace it, and the first take succeeds while all later takes return false/default.
- **AC-M5D7O-004:** all three notification kinds and absence project with exact receipt correlation, immutable `TopRight` anchor, and no change to menu availability or focus.
- **AC-M5D7O-005:** Click and Submit dismiss a visible notification once; a second dismissal, absent-notice dismissal, menu activation, intent take, and the absence of any time/cancel/navigation dismissal API satisfy the manual-only policy; unknown activation or dismissal enums latch Failed and throw `ArgumentOutOfRangeException`.
- **AC-M5D7O-006:** reflection mutation of every view, intent, notification, and controller state/correlation backing field fails at the next getter or command boundary and latches the controller Failed where it owns the boundary.
- **AC-M5D7O-007:** static scope checks find Unity UI/scene/EventSystem/TMP, IO, profile save/mutation, RequestMode/action/map operations, run/gameplay/narrative, Quit, timer, network, RNG, and logging calls absent.
- **AC-M5D7O-008:** focused M5D7O EditMode, direct M5D7N, full EditMode, and full PlayMode runs finish with failure/skip/inconclusive zero; Luna reports P0=0/P1=0.

## Proposed implementation allowlist

- new `Assets/AcadeGameMaker/Runtime/Input/Unity/HubMenuPresentationControllerV1.cs` and `.meta`
- new `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/AcadeGameMaker.Input.Unity.EditMode.Tests.asmdef`, controller test fixture, and their `.meta` files
- one exact `InternalsVisibleTo("AcadeGameMaker.Input.Unity.EditMode.Tests")` line in `Assets/AcadeGameMaker/Runtime/Input/Unity/AssemblyInfo.cs`
- this contract, M5D7O pre-gate/implementation/review evidence, and one minimal `docs/README.md` entry after approval

No M5D7N/M5D7M/InputRouter source, scene, prefab, Canvas, EventSystem, UI/TMP asset, manifest, packages lock, ProjectSettings, profile/persistence/run/gameplay/narrative code, or unrelated file may change.

The new EditMode test assembly must directly reference `AcadeGameMaker.Input.Unity`, `AcadeGameMaker.Profile`, `AcadeGameMaker.Input`, `Unity.InputSystem`, and the Unity test assemblies it actually uses. Fixtures construct receipts through the existing instance-scoped `ProfileLaunchPreparationCoordinatorV1` injected-port path; no test-only production factory, static environment state, or new runtime construction seam is allowed.

## Stop and rollback conditions

Stop before approval if the controller requires a Unity UI/package dependency, a live router/profile/file query, a second M5D7N notification take, actual menu side effects, notification prose or automatic dismissal, or any file outside the allowlist. Stop M5D7P if it requires `InputSystemUIInputModule` to own/enable the adopted actions, a lightweight replacement router, an incomplete M5B5 graph, fractional logical scaling, safe-frame-external interaction, visible-warning loss, or an unapproved uGUI/TMP version.

Rollback removes only new M5D7O source/tests/evidence, the exact friend-assembly line, and its documentation index entry.

## Participation record

- The user approved the visible hub defaults summarized above.
- Sol supplied the bounded M5D7O/M5D7P decomposition, API, state machine, profile-source predicate, REQ/AC draft, and stop conditions on 2026-09-14.
- Astra converted the recommendation into this contract, accepted Luna's amendments, and approved the bounded M5D7O implementation on 2026-09-14.
- Terra is assigned only after this approval. Luna remains independent and may not accept its own implementation.
- Terra implemented and recorded the final execution evidence; Luna independently accepted `AC-M5D7O-001..008`; Astra integrated the evidence and advanced the contract to Verified on 2026-09-20.
- No Ollama model or external cloud prompt participated.

## User-decision audit

No additional user decision is required for M5D7O. Continue means profile continuation from the hub, never active-expedition resume, because the already Approved persistence contract forbids active-run saving. M5D7P remains blocked on technical seam and compatible-package contracts rather than a new product decision.
