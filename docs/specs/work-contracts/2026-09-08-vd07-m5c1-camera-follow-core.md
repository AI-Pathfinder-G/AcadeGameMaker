# M5C1 — Deterministic GameplayFollow camera core

- Status: Verified
- Verified: Astra integration acceptance, 2026-09-08, after Luna independent runtime and XML review PASS; focused34, full EditMode474 + PlayMode539 =1013, zero failures/skips. See camera-follow evidence.
- Approved by: Astra, 2026-09-08; Terra implementability PASS and Luna independent pre-gate PASS (P0/P1/P2 = 0).
- Authority: Astra; implementation: Terra; independent verification: Luna
- Parents: Approved VD-08 REQ-ART-009/010/011; VD-07 REQ-UX-013; SYSTEM-CONTRACTS camera ownership; ADR-0027
- Design inputs: Sol simulation-camera proposal and Terra implementation-impact assessment, 2026-09-08
- Implementation is authorized only within the bounded allowlist below. M5B5 is Verified with 979 full-suite tests.

## Bounded result

One engine-free GameplayFollow session computes the approved follow motion from completed player snapshots. It has fixed authored camera-center bounds and an explicit initial center; it does not publish a Core camera pose, read Unity state or pretend to establish completed bootstrap authority. This is a small prerequisite for the actual camera driver, not completion of every camera acceptance criterion.

No Unity driver, scene/prefab, viewport, Pixel Perfect Camera configuration, anchors, context owner, room change, respawn, teleport, hard snap, input callback or Run behavior is implemented here. Later contracts integrate those existing approved behaviors. No change to Core AimSample, SimulationCameraPoseSnapshot, PlayerMotionSnapshot or Movement semantics. Camera never depends on mouse position.

## Assembly and API

Use a new engine-free `AcadeGameMaker.Camera` assembly in Runtime/Camera, referencing Core and Movement. Do not place a Movement-dependent session in Core (that would create a dependency cycle). Its internal namespace is AcadeGameMaker.Camera. Named friend access initially grants only AcadeGameMaker.Camera.EditMode.Tests; a future Unity adapter requires a later contract.

`CameraCenterBounds` is an immutable inclusive min/max X/Y in signed integer pixel18 coordinates (world units multiplied by 18). Both axes require min <= max. All bounds endpoints must be representable by the existing Q100 publication conversion below. These are already authored camera-center bounds, not raw room geometry.

`SimulationCameraSession(firstTick, initialCenterXPixel18, initialCenterYPixel18, bounds)` validates nonnegative first tick and initial center inside bounds. It begins with no completed result, zero anticipation offsets/targets and settled transition ages 12. No tick-minus-one or seed snapshot is created. The session exposes NextExpectedTick, HasLatestSnapshot and a getter that rejects before the first successful result.

`Advance(PlayerMotionSnapshot completedPlayer)` requires exactly the next expected tick. The future driver, not this pure session, must prove the snapshot actually came from a completed Movement phase. A successful call returns immutable `CameraFollowSnapshot`: processed tick, center X/Y pixel18, center X/Y Q100, current X/Y offset pixel18, target X/Y pixel18 and transition ages. No reference to mutable session state escapes.

## Exact arithmetic and order

REQ-ART-011 and REQ-UX-013 apply to all rules below.

1. Velocity enters unchanged as signed Q4096. Horizontal target is -54 pixel18 at X <= -8192, +54 at X >= 8192, otherwise zero. Vertical target is +27 at Y >= 16384, -54 at Y <= -24576, otherwise zero.
2. A changed target freezes the **last committed offset** as interpolation start and uses age 1 in this same processed tick. Thus the changed-band tick is the first of exactly 12 ticks. An unchanged target increments its current age through 12; settled ages remain 12. For age a, offset = start + RoundAway((target-start)*a/12). Retargeting never evaluates an extra hidden old-band step first. Both axis transitions are independent.
3. Keep player position exact until the final snap: lift player Q4096 by multiplying by 18; lift camera/offset pixel18 by multiplying by 4096. This common signed numerator domain represents world units multiplied by 73728. Do not round player/focus/desired positions into pixel18 early.
4. Focus is player plus the current interpolated offset. Inclusive dead-zone half extents are 54 pixel18 in X and 36 in Y. An inside focus preserves the center on that axis; outside focus sets desired center to the nearest dead-zone edge.
5. In the common exact domain, move each axis independently toward desired by at most 9 pixel18 (0.5 world unit), clamp to fixed authored bounds, then RoundAway(numerator/4096) to integer pixel18. Bounds on the pixel18 lattice ensure final snapping cannot escape the bounds.
6. Convert the final pixel18 center to existing Q100 with RoundAway(pixel18*100/18), using checked intermediates and final int conversion. It is a quantized representation, not a second camera state. No ABI expansion or targeting math change is needed.
7. Validate/check all next-tick arithmetic, both axis transitions, result construction and both Q100 conversions before changing any session field. A failed call preserves the entire prior snapshot, transitions and next tick. No floating point, Unity API, wall clock or render-frame identity is used.

Direction reversal and dash are not hard snaps. This unit intentionally has no hard-snap entry point. The future driver publishes the one resulting completed pose only after Movement, using the existing camera DTO and viewport contract.

## Acceptance

- AC-M5C1-001 (REQ-ART-011): exact target thresholds cover X +/-8191/8192/8193 and Y 16383/16384/16385, -24575/-24576/-24577. First offset 0→54 at ages 1/6/12 is 5/27/54. After ages 1..3 (5/9/14), retarget -54 produces age1 offset8 and reaches -54 at new age12. Vertical 0→27 starts2 and reaches27; -54 uses symmetric rounding.
- AC-M5C1-002 (REQ-ART-011): inclusive X dead-zone at player X=+/-12288 Q4096 leaves center zero with zero velocity; Y +/-8192 does likewise. X=12288+113 remains pixel18 zero after final snap, while X=12288+114 yields +1; negative symmetry yields -1. No early position quantization. Player (4u,3u) with zero velocity and center0 produces (9,9) pixel18, proving independent caps.
- AC-M5C1-003 (REQ-ART-010/011): fixed inclusive bounds clamp after follow cap. Example initial center0, X bounds[-4,4], player X=4u -> center4, not9. Equal min/max is valid. Invalid reversed/out-of-range bounds, outside initial center and negative first tick reject. No pointer/dash parameter exists.
- AC-M5C1-004 (REQ-UX-013): pixel18 -1/+1/+9 projects to Q100 -6/+6/+50. Exact integer error inequality is abs(Q100*18-pixel18*100) <9, equivalent to <0.005u (the difference is even, so magnitude9 cannot occur). Initial snapshot unavailable; tick0 becomes first result. Repeated/stale/skipped ticks and successor overflow leave all prior state unchanged.
- AC-M5C1-005 (REQ-ART-011/REQ-UX-013): identical completed player traces under scripted 30/60/144 render grouping yield identical snapshot sequences. Full regression remains green; no Core DTO, Movement, InputRouter, packages, settings, media or scenes changed. This does not claim physical FPS, Unity rendering, viewport or anchor acceptance.

## Allowlist and verification ownership

New Runtime/Camera folder/asmdef/AssemblyInfo and camera contracts/session files with metas; new Tests/EditMode/Camera folder/test assembly/fixtures and metas. The owning contract, evidence document and docs/README may be updated. No other implementation files. Terra implements; Luna independently reviews exact arithmetic and AC oracles; Astra approves before implementation and accepts integration only after Unity EditMode and full regression evidence.
