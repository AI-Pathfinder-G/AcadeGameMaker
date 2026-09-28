# Deterministic simulation camera — bounded design proposal

- Date: 2026-09-08
- Status: Draft for Astra approval; not an implementation contract
- Design: Sol (bounded design)
- Final authority: Astra under ADR-0027
- Parents: Approved camera behavior/value approvals, VD-07, VD-08, VD-09 and `SYSTEM-CONTRACTS`

## Outcome and boundary

The minimum useful camera unit is an engine-free deterministic `SimulationCameraSession` plus one Unity `SimulationCameraDriver`. The session solely owns camera mode, anticipation interpolation, snapped center, active camera-center bounds and exact tick. The driver binds the real Camera/Pixel Perfect Camera presentation, reads only completed Movement snapshots, validates viewport framing, and publishes the sole immutable camera snapshot consumed by InputRouter on the next tick.

This unit does not invent room progression, respawn, teleport, boss/choice/cutscene sequencing or anchor selection. It accepts those as owner-bound immutable context directives. Until a real context owner exists, one explicitly authored initial room bounds/mode and optional frozen anchor registry are sufficient for a playable room. Mouse position never influences camera state.

## Tick and bootstrap

`SimulationCameraDriver` runs at `[DefaultExecutionOrder(+10)]`: after default-order player Movement and before the existing `+100/+110` read-only late phases. For every normal global tick `t`, it requires Movement's completed `PlayerMotionSnapshot.Tick == t` and `player.NextExpectedTick == t+1`, advances the pure session once, applies the exact committed snapped pose to the Unity camera, and publishes `SimulationCameraPoseSnapshot.CameraPoseTick=t`. InputRouter at `-210` of tick `t+1` may consume only that publication.

Before the first simulation tick `t0` (normally zero), authoring validation may place the Unity camera for initial rendering from the movement owner's pristine authored pose, initial camera-center bounds and initial mode. This is an unnumbered presentation seed only: it does not advance the camera session and does not publish `SimulationCameraPoseSnapshot`. `PlayerMovementMotor` labels its pristine snapshot with `Tick=t0`, not `t0-1`, and `TransferObservationBuilder` requires an actually completed player snapshot at `t-1`; relabeling the seed would fabricate authority. Initialization rejects missing authored bounds or a movement snapshot incompatible with the pristine `t0` state.

The camera owner begins in explicit `BootstrapPending` with no latest publication. The first `-210(t0)` input phase therefore publishes Movement input and may publish nullable-aim Transfer, but publishes no `AimSample`, camera-coupled Transfer input or basic attack. After Movement genuinely completes `t0`, `+10(t0)` advances and publishes the first authoritative camera snapshot with `CameraPoseTick=t0`. Aim-dependent input first becomes eligible at `-210(t0+1)`, paired with completed Movement `t0`. Reinitialization cannot restart the global clock or manufacture a pre-first-tick publication.

## Pure state and arithmetic

The pure state contains:

- next expected camera tick;
- `GameplayFollow` or `AuthoredAnchor` mode and optional non-empty frozen anchor ID;
- active authored camera-center min/max bounds on both axes;
- current snapped center;
- current horizontal and vertical anticipation offsets;
- each axis' interpolation start, target band, start tick and 12-tick age;
- last accepted context-directive identity/revision for replay rejection.

Use integer/rational arithmetic only. Movement pose and velocity enter as Q4096. Velocity boundaries are inclusive and exact: horizontal target is `-3/0/+3u` at `velocityX <= -8192`, between, or `>=8192`; vertical target is `+1.5u` at `velocityY>=16384`, `-3u` at `velocityY<=-24576`, otherwise zero. Direction comes from velocity sign, never facing, pointer or dash state.

All approved camera distances are exact multiples of `1/18u`: dead-zone half extents `54/18` and `36/18`, horizontal offsets `±54/18`, vertical offsets `+27/18` and `-54/18`, and follow cap `9/18` per axis. Store snapped centers and band endpoints as signed checked pixel-grid integers (`world ×18`). For comparisons with Q4096 player values, lift both into the common exact numerator domain Q73728 (`Q4096×18`, cameraPixel18×4096); do not convert the player pose through float.

On a target-band change at tick `t`, freeze the currently evaluated offset as the new interpolation start and set age zero. For age `a=1..12`, evaluate `start + RoundAwayFromZero((target-start)×a/12)` in pixel-grid units; age 12 is exactly target. An unchanged band continues its existing interpolation. This pins the otherwise ambiguous phrase “12-tick linear transition” without changing its approved value.

For `GameplayFollow`, calculate in the approved order:

1. focus = completed player position + current interpolated anticipation offset;
2. preserve the current center while focus remains inside the inclusive 6×4u dead zone; outside an axis, desired center is `focus - sign(delta)×halfExtent` for that axis;
3. move current center toward desired independently on X and Y by at most `9/18u`;
4. clamp each axis to the authored camera-center bounds;
5. round each axis AwayFromZero to the nearest `1/18u` and commit.

All intermediate additions, products, tick successors and rational divisions are checked before session mutation. X and Y are staged together; failure preserves the entire prior state/publication.

## Hard snaps and anchors

Only an exact context directive may request `RoomEntry`, `Respawn`, `Teleport`, `AnchorEnter` or `AnchorExit`; these are the five approved same-tick hard-snap reasons. Direction change and dash are never inferred as snap reasons.

- RoomEntry/Respawn/Teleport in GameplayFollow computes the current focus/dead-zone desired center, bypasses only the 0.5u follow cap, then performs room clamp and 1/18 snap. It does not bypass bounds.
- AnchorEnter carries a frozen authored anchor ID. The session resolves its immutable center from the validated registry, clamps it to the active room camera-center bounds, snaps it, enters `AuthoredAnchor`, and resets both interpolation starts at the current offsets without advancing them from player velocity.
- While anchored, player motion does not move the camera or advance anticipation bands. The published mode and anchor ID remain authoritative presentation state.
- AnchorExit returns to GameplayFollow and hard-snaps using the same completed player pose for that tick, then restarts both 12-tick transitions from their preserved current offsets toward the current velocity bands.

Boss, choice and cutscene remain the only legal authored anchor categories. The anchor registry freezes stable non-empty ordinal IDs, category and exact center before the first camera tick; duplicate IDs, unknown categories, nonfinite authoring conversions and mutation after freeze reject.

## Room/context owner protocol

The camera driver registers exactly one `cameraContextOwner` by reference identity before the first tick. A context directive is immutable and exact-tick: `(directiveId, revision, tick, roomId, centerBounds, snapReason?, modeRequest?, anchorId?)`. It is prepared mutation-free and committed together with that tick's camera advance. Duplicate identical delivery is idempotent only before consumption; conflicting ID/revision/tick or a second owner rejects without camera mutation.

Bounds are authored **camera-center bounds**, not raw room geometry: `minX<=maxX`, `minY<=maxY`, already accounting for the fixed 16:9 half-frame. This prevents the camera owner from guessing room collider topology. A room change requires RoomEntry and a new non-empty room ID. Same-room bounds changes are forbidden in this slice. Anchor enter requires AuthoredAnchor plus a registry ID; anchor exit requires GameplayFollow and no anchor ID. Other combinations reject.

The future VD-04/Run/choice coordinator decides which already-approved directive occurs and arbitrates simultaneous lifecycle causes before submission. The camera does not discover these facts from scene objects, Combat phase or UI state. Absence of that coordinator is an integration dependency, not a user product choice.

## Unity projection and viewport

The Unity driver uses one orthographic camera with fixed `orthographicSize=10`, rotation zero, and the approved 640×360 Pixel Perfect reference. It writes the committed center directly; there is no Cinemachine damping, `LateUpdate` smoothing, dynamic zoom or second render-only pose. The presentation Transform is a projection of session state and never feeds the next calculation.

For the unnumbered initial render placement and each authoritative advance, capture the actual integer viewport width/height once. For `>=640×360`, calculate exactly:

`integerScale=floor(min(width/640,height/360))`, rectangle `640×scale` by `360×scale`, and centered floor offsets. Odd remainder belongs to right/top. Construct the snapshot only after the session candidate and viewport values validate. Pointer conversion continues to use the canonical normalized endpoint grid and existing `TransferFixedMath`.

A viewport below 640×360 violates the Approved platform minimum and the current DTO rejects it. The driver must not coerce scale to one, crop, reveal extra world, or publish a fabricated snapshot. It rejects the camera frame before session/Transform/publication mutation, retains the last completed pose for diagnostics, and marks current camera input unavailable. Window configuration is responsible for enforcing the minimum; recovery resumes only on a later valid viewport with an explicit diagnostic gap policy approved in the implementation contract. This is not a new supported resolution.

## Intentional camera publication quantization

Counter-review finds no approved requirement that the targeting projection reproduce the internal/render center with zero numerical error. The normative documents explicitly retain camera `positionQ100` as the presentation snapshot contract, distinguish it from Q1000 target geometry, and require deterministic exact-match replay rather than zero-error inversion. The current DTO and `TransferFixedMath` tests consistently implement that boundary. Therefore no Core ABI or targeting-math expansion is justified.

The session still owns and renders the exact `1/18u` snapped center as signed pixel-grid integers. When publishing the completed camera snapshot, derive each existing Q100 field exactly as `RoundAwayFromZero(pixel18 * 100 / 18)` with checked multiplication and division. Maximum representation error is below `0.005u`. `TransferFixedMath.ProjectWorldToGrid` and `InverseGridToWorld` continue to consume this approved Q100 snapshot unchanged. InputRouter uses that same published value for next-tick mouse aim; it must not reach back into the session's finer internal state.

This Q100 publication is a quantized representation of the one authoritative completed camera state, not a second render pose and not smoothing. The Unity Camera Transform is driven from the exact internal `pixel18/18` center, while all snapshot consumers receive the same deterministic Q100 projection. Tests pin negative and positive AwayFromZero conversion, sub-Q100 adjacent pixel-grid positions, the `<0.005u` bound and exact replay. Q4096 Movement remains exact in the camera-state calculation domain before final snap; Q1000 target geometry and existing projection helpers remain unchanged.

## Acceptance shape for a future Approved contract

- Explicit `BootstrapPending` has no published camera pose; tick `t0` aim-dependent input is absent, then exact camera publications `t0,t0+1...` feed InputRouter from tick `t0+1` with no relabeled seed, same-tick or render-pose read.
- Boundary fixtures cover velocity `8191/8192/8193`, `16383/16384/16385`, `-24575/-24576/-24577`, every anticipation band change/restart and exact age 1/12/12.
- Dead-zone inclusive edges, independent 0.5u caps, room clamp, negative AwayFromZero snap and hard-snap allowlist match exact integer oracles at 30/60/144 render grouping.
- GameplayFollow, each legal hard snap, valid anchor enter/hold/exit, invalid category/ID, stale/cross-owner/conflicting directives and overflow are mutation-free where rejected.
- 640×360, 1280×720, 1920×1080, 2560×1440 and odd letter/pillarbox dimensions produce exact rectangles; below-minimum dimensions publish no fabricated pose.
- The Camera Transform uses exact internal pixel18 state; snapshot Q100 uses the single approved AwayFromZero quantization and existing `TransferFixedMath`, with no dynamic zoom, pointer pan, scene discovery or render-only smoothing.

## Decision boundary

Phase `+10`, pure/Unity separation, explicit no-publication bootstrap, owner-bound context protocol, rational internal calculation, approved Q100 publication and below-minimum fail-stop are technical consequences of Approved behavior. They require Astra contract approval and Luna adversarial review, not a user product decision. No Core/motor semantic change, ABI expansion, targeting-math change, approved product value or behavior change is proposed.
