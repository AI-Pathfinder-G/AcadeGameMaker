# M5C2 — Fixed-room completed Unity camera provider

- Status: Verified
- Verified: Astra local integration acceptance, 2026-09-08 after Luna independent final AC001..005/code/XML/hash review PASS. Full EditMode480 + PlayMode567 =1047, zero failed/skipped; focused runtime28 and Editor6. See M5C2 camera-driver evidence. No playable-scene or GPU visual acceptance is implied.
- Approved: Astra, 2026-09-08 after Terra implementability and Luna independent pre-gate PASS (P0/P1/P2=0). M5B5 and M5C1 are Verified. Only this bounded allowlist is authorized.
- Authority: Astra; implementation: Terra; independent verification: Luna
- Prerequisites: M5B5 and M5C1 must be Verified before implementation.
- Parents: VD-08 REQ-ART-009/010/011; VD-07 REQ-UX-013; SYSTEM-CONTRACTS completed camera ownership; ADR-0027.

## Small integration outcome

A single fixed-room Unity driver runs at +10 after completed Movement and implements the existing IInputCameraSnapshotProvider used by InputRouter. It drives one explicitly bound orthographic Camera from M5C1 state and publishes the existing immutable camera DTO. Initial bounds/center remain frozen for this unit. This connects actual prior-tick camera data, but does not yet author a playable scene or claim rendered pixel-art acceptance.

No anchors, context directives, room changes, respawn/teleport, dynamic zoom, pointer pan, Cinemachine, render smoothing, UI/Run effects, package/settings changes or Core DTO expansion. The earlier camera proposal is supporting design, not authority over this contract.

## Lifecycle and fixed phases

Create AcadeGameMaker.Camera.Unity (Runtime/Camera/Unity), depending on Camera, Core, Movement/Movement.Unity, Input.Unity and the already installed URP runtime. Use fully qualified UnityEngine.Camera to avoid collision with the project's Camera namespace. Grant only Camera.Unity and its dedicated tests the required named friends in Camera, Input.Unity and Movement.Unity. No global FindObject/static fallback.

SimulationCameraDriver is an internal MonoBehaviour at DefaultExecutionOrder(+10). Serialized authoring binds exactly one player, Unity Camera, co-located PixelPerfectCamera and initial inclusive pixel18 bounds/center. ConfigureForAuthoring only assigns before initialization. BoundPlayer is readable before Start, allowing the earlier InputRouter Start to validate the service identity without a fake camera publication.

Initialize after authored Awake preparation but before the first Movement tick. Require the player's pristine Snapshot.Tick == NextExpectedTick; a late attachment after a completed Movement tick rejects rather than fabricating history. Construct the core at that explicit first tick. The initial Camera Transform may be set from the validated authored center for rendering, but Latest camera DTO remains absent. Never create tick-minus-one data.

Each +10 advance requires completed Movement Snapshot.Tick == core.NextExpectedTick and player.NextExpectedTick == checked(snapshot.Tick+1). Duplicate, skipped, uncompleted seed and cross-wired bindings reject before core/Transform/publication changes. Advance the core exactly once with the actual completed snapshot. Only the driver's immutable completed publication is returned by TryGetCompletedCamera; a request matches its exact tick, otherwise returns false. No caller-supplied player pose or render Transform becomes simulation input.

OnDisable makes the provider unavailable and permanently faults an initialized driver. Re-enable cannot rebootstrap, rewind or silently resume. Dispose/teardown does not alter Movement or InputMode. Unexpected engine failure after core advance latches Faulted, publishes no new camera DTO and makes the old one unavailable; no rollback is claimed.

## Camera and Pixel Perfect profile

Validate exact Camera/PixelPerfectCamera co-location, active/enabled required components, orthographic mode, fixed orthographic size10, zero world rotation and unit world scale. Preserve an authored fixed finite Z; calculations and snapshots use X/Y only. No camera Transform is read to compute follow targets. Configuration cannot drift silently during runtime.

The actually resolved URP17.6.0 (package-lock and installed package.json; manifest retains its prior17.3.0 request) exposes public PixelPerfectCamera assetsPPU=18, refResolutionX=640, refResolutionY=360, cropFrame=Windowbox and gridSnapping=UpscaleRenderTexture. Authoring must explicitly configure/validate these values. Point filtering is a serialized authoring requirement: the installed package exposes only private serialized m_FilterMode, not a public runtime setter. A narrowly scoped Editor profile helper uses SerializedObject to set/validate that property against the installed PixelPerfectFilterMode.Point enum; a missing/type-mismatched property fails closed. No runtime reflection, private-field runtime access, obsolete booleans or dependency installation. Runtime validates the public profile only and does not claim to detect private serialized filter drift; Editor tests certify that boundary, with rendered acceptance deferred. PixelPerfectCamera owns its rendering crop; the driver calculates the approved corresponding integer rectangle from output dimensions, not from an alleged public PPC rectangle API. This is snapshot projection, not a second rendering crop or smoothing pose.

This unit changes no package versions, manifest or lockfile. It records the existing resolved source accurately rather than certifying an uninstalled17.3 API. Validate public profile/binding/rotation/scale/Z invariants at initialization and before every core advance. Drift fails closed and makes the provider unavailable; this is not a recoverable viewport gap. X/Y render position is deliberately not a follow input or drift fault: the next valid tick overwrites it from core state.

Capture actual output size once per +10 from the bound camera's target RenderTexture dimensions when explicitly assigned, otherwise Screen.width/height. For supported output, scale=min(width/640,height/360), rect size=(640*scale,360*scale), origin=((width-rectWidth)/2,(height-rectHeight)/2), integer division with odd spare pixel on right/top. Validate all viewport arithmetic before core advance.

Write Camera world XY from the committed pixel18 center divided by18, never from Q100. Publish the same result's Q100 fields in SimulationCameraPoseSnapshot with ortho10000, rotation0 and the captured viewport. Rendering floats are an engine projection only; neither Camera nor PixelPerfectCamera feeds a new pose back into the core. The existing Q100 quantization boundary remains unchanged.

## Explicit unsupported-viewport gap policy

This contract deliberately replaces the draft proposal's “freeze camera session on invalid viewport” suggestion. Movement continues ticking under M5B5 even without aim; freezing the camera session would leave its next tick permanently behind and require an unauthorized reset/catch-up protocol.

Therefore when output is temporarily below640x360, continue the pure follow core on each genuinely completed Movement tick, but publish **no usable camera DTO** and do not write a new Camera Transform. Mark a bounded OutputUnavailable diagnostic. The last historical snapshot may remain diagnostic-only; TryGetCompletedCamera returns false throughout the gap. This is not support for a smaller gameplay viewport and never fabricates scale1/cropped framing.

On a later supported output, apply and publish only that current completed core tick. No seed relabel, frame replay, skipped core tick, gameplay reset or second smoothing path. The transform catch-up reflects unchanged simulation history, not an approved gameplay hard-snap event. InputRouter naturally suppresses camera-dependent commands while unavailable; it does not infer Run failure or modify mode.

Malformed graph/profile is distinct from unsupported output size and fails closed. Diagnostic state is a single immutable latest record with disposition and tick, not an event log or per-frame log spam. Pure core or profile failure cannot masquerade as viewport recovery.

## Narrow verification seams

Tests may use ConfigureForAuthoring, explicit InitializeForTests and AdvanceForTests delegating to the same production routines, read-only core/publication/diagnostic state and a fixed failure-stage enum (BeforeCoreAdvance, AfterCoreAdvance, BeforeTransformWrite, BeforePublication). No arbitrary callbacks, fabricated pose setter or output-dimension override is allowed. Viewport fixtures bind real RenderTexture objects with the tested dimensions; they need not allocate GPU surfaces. Driver ConfigureForAuthoring only assigns dependencies and initial state; external authoring/test setup applies Camera/PPC settings before initialization. A successful manually initialized driver is not initialized again by Start; explicit repeated initialization rejects. Tests may compare the actual Transform and existing DTO without changing the pure core contract.

## Acceptance

- AC-M5C2-001 (REQ-ART-011/REQ-UX-013): no seed DTO; after real Movement0, +10 publishes camera0 and InputRouter1 uses that exact camera/player pair. Reject duplicate, skipped and seed-as-completed calls before mutation.
- AC-M5C2-002 (REQ-ART-009/010): actual target output640x360,1280x720,1920x1080,2560x1440,1366x768 and odd letter/pillarbox dimensions yield exact centered integer rectangles, fixed ortho and expected PixelPerfect settings. No claim of GPU-rendered visual acceptance without capture evidence.
- AC-M5C2-003 (REQ-ART-011/REQ-UX-013): Camera XY matches pixel18 state; DTO Q100 matches M5C1 conversion. Mutating a render Transform cannot steer the next logical camera calculation. No pointer/zoom/dash smoothing path.
- AC-M5C2-004 (REQ-ART-010/REQ-UX-013): supported→unsupported→supported output advances every core tick, returns no usable DTO during the gap, retains prior Transform while unavailable and recovers exactly the current tick without reset/catch-up. Actual router aim is suppressed during the gap and resumes with fresh current-tick samples.
- AC-M5C2-005 (REQ-ART-011/REQ-UX-013): wrong player/camera/PPC/profile, late initialization, disable/re-enable and injected engine-publication failure cannot publish misleading completed camera data. Full regression remains green.

## Allowlist

New Camera/Unity assembly/driver/helper/metas; narrow Camera/Input.Unity/Movement.Unity AssemblyInfo friend additions; new Editor/CameraAuthoring profile helper/assembly/metas and dedicated EditMode profile tests; new Tests/PlayMode/CameraUnity assembly/fixtures/metas (and targeted input integration fixture if needed); this contract/evidence/docs README. No action asset/wrapper, Core DTO, Movement algorithm, camera core behavior, scene/prefab, package/settings, media or existing gameplay lifecycle changes. Astra approves only after independent pre-gate and completion of M5C1.
