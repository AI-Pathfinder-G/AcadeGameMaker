# Work Contract: VD-02 Deterministic Weight Transfer

- Status: M1 Verified 2026-08-26; M2A fixed math and LOS endpoint feasibility Verified 2026-08-26; M2B1 target registry and production LOS Verified 2026-08-26; M2B2A observation and movement seam Verified 2026-08-26; M2B2B driver integration and the Unity M2 adapter milestone Verified 2026-08-27; actual enemy/boss consumer evidence remains deferred to VD-03
- Owning spec and revision: `VD-02`, Approved and precision-amended by Sol 2026-08-25
- Assigned by: Sol
- Implementer: Terra; Kimi K3 may draft isolated pure selectors, session transitions, adapters, and tests only inside this frozen contract, and Terra screens and integrates them
- Independent verifier: Luna
- Requirement IDs: `REQ-WT-001~008`; movement integration also cites `REQ-MOV-004`
- Acceptance-criterion IDs: `AC-WT-001~006`; VD-03 target behaviors may supply later completion evidence for the enemy and boss portions of `AC-WT-002` and `AC-WT-005`
- Allowed files/directories: `Assets/AcadeGameMaker/Runtime/Core/**`, `Assets/AcadeGameMaker/Runtime/Transfer/**`, `Assets/AcadeGameMaker/Editor/Transfer/**`, `Assets/AcadeGameMaker/Tests/EditMode/Transfer/**`, `Assets/AcadeGameMaker/Tests/PlayMode/Transfer/**`, `Assets/Scenes/WeightTransferSandbox.unity`, `Assets/Prefabs/Transfer/**`, their `.meta` and assembly definitions; `Assets/AcadeGameMaker/Runtime/Movement/AssemblyInfo.cs`, `Assets/AcadeGameMaker/Runtime/Movement/Unity/AssemblyInfo.cs`, and `PlayerMovementController.cs` only for the Sol-frozen internal modifier seam; `ProjectSettings/TagManager.asset` only for assigning user layer 8 exactly as described below
- Forbidden files/directories: `GameInput.inputactions`, `InputRouter`, device reads, combat health/damage/AI ownership, room assembly, persistence, narrative, final UI/art/audio, external assets, package changes, unrelated project settings, public movement-contract changes, and any gravity-direction feature
- Public interface or data contract: only the types and semantics frozen below; no public type, member, enum value, event payload, serialized schema, namespace, or modifier ID may be added without Sol approval
- Cross-part invariants: one authoritative signed `SimulationTick`; Q1000 target geometry; Q4096 aim vector; integer candidate keys; ordinal authored target IDs; at most one active target; player and target modifier transition committed atomically; no Unity reference in public payloads; target owner retains base mass/gravity/AI/health; transfer owns only reversible modifiers and references
- Rollback point: annotated tag `vd01-movement-adapter-m2-verified-v1`
- Required implementation evidence: pure EditMode selector/session tests, Unity PlayMode LOS/target/modifier/lifecycle tests, 5.999/6.000/6.001u and every specified pointer/angle/hysteresis boundary, atomic failure tests, 21-tick transition-lock boundary, three identical replays, and 30/60/144 render grouping over the same 60 Hz samples
- Required independent verification evidence: Luna reviews all Kimi-originated and Terra-integrated output, reruns relevant suites, checks immutable payload/API boundaries, and records evidence IDs against each claimed `AC-WT-*`
- Integration order and dependencies: M1 pure contracts/selector/session; M2 Unity target registry, LOS, aim projection, movement modifier bridge, sandbox and PlayMode; VD-03 later supplies real enemy/boss consumers; VD-07 later supplies the production `InputRouter` without changing these payloads
- Sol approval/date: Approved by Sol, 2026-08-25

## Frozen shared input payloads

Shared immutable payloads live in `AcadeGameMaker.Core` so VD-07 can publish them later without making VD-02 depend on an input implementation assembly.

- `AimSource`: `MousePointer=0`, `GamepadStick=1`.
- `SimulationCameraPoseSnapshot`: `CameraPoseTick`, camera `PositionXQ100`/`PositionYQ100`, positive `OrthoSizeQ1000`, normalized `RotationQ10` in `[0,3599]`, viewport width/height of at least 640×360, the exact centered gameplay rectangle, and its exact positive integer scale. `integerScale=floor(min(viewportWidth/640,viewportHeight/360))`; width/height are `640×integerScale`/`360×integerScale`; X/Y are the floored half-remainders. Its constructor rejects any inconsistent derived rectangle and contains no `Camera` reference.
- `AimSample`: nonnegative signed-64 `SampleId`, `Source`, nullable normalized-grid `ScreenPixelX`/`ScreenPixelY`, nullable `IsPointerInsideGameplayRect`, clamped `AimVectorXQ4096`/`AimVectorYQ4096`, `CameraPoseTick`, and `SampleTick`. Both screen coordinates and pointer-inside are present exactly for mouse and absent exactly for gamepad. Mouse coordinates are in inclusive ranges X `0..1919`, Y `0..1079`; every source requires `CameraPoseTick=SampleTick-1` with checked signed-32 arithmetic. The aim vector may not quantize to `(0,0)`.
- `TransferPressed`: nullable nonnegative signed-64 `AimSampleId` and exact `Tick`. A non-null ID must match the sample consumed for that tick. VD-02 does not read devices or invent a missing sample.

Exact scalar member names and constructor order are normative:

- `SimulationCameraPoseSnapshot(SimulationTick cameraPoseTick, int positionXQ100, int positionYQ100, int orthoSizeQ1000, int rotationQ10, int viewportWidth, int viewportHeight, int gameplayRectX, int gameplayRectY, int gameplayRectWidth, int gameplayRectHeight, int integerScale)` with same PascalCase properties.
- `AimSample(long sampleId, AimSource source, int? screenPixelX, int? screenPixelY, bool? isPointerInsideGameplayRect, int aimVectorXQ4096, int aimVectorYQ4096, SimulationTick cameraPoseTick, SimulationTick sampleTick)` with same properties.
- `TransferPressed(long? aimSampleId, SimulationTick tick)` with `AimSampleId` and `Tick`.

## Frozen transfer public payloads

Types live in `AcadeGameMaker.Transfer`.

- `TransferTargetKind`: `Box=0`, `Enemy=1`, `BossPayload=2`.
- `TransferTargetDescriptor`: nonempty ordinal `TargetId`, kind, Q1000 local `AimPoint`, `AimShapeCenter`, strictly positive `AimShapeHalfExtents`, nonempty immutable `BaseModifierProfileId`, and `IsAvailable`.
- `TransferCandidateObservation`: immutable descriptor plus Q1000 world aim point and world aim-shape center/half-extents, `IsTrigger`, `IsLineOfSightOpen`, nonnegative `PlayerDistanceKey`, `MouseInsideShape`, nonnegative `ScreenDistanceSquaredKey`, and nonnegative `GamepadAngleKey`. The Unity adapter computes observations; the pure selector never reads transforms, colliders, cameras, or render state.
- `TransferSelectionSource`: `None`, `MousePointer`, `GamepadStick`.
- `TransferFailureReason`: `None`, `InvalidTarget`, `OutOfRange`, `LineOfSightBlocked`, `TargetUnavailable`, `TriggerTarget`, `Cooldown`, `StaleAimSample`, `ModifierRejected`. VD-07 filters disallowed input before VD-02, so M1 does not invent a separate `InputBlocked` attempt.
- `TransferSelectionSnapshot`: tick, source, selected/highlighted target ID or empty, `InsideRank`, `ScreenDistanceSquaredKey`, `PlayerDistanceKey`, `GamepadAngleKey`, and readiness/failure reason. A key not applicable to the source is `-1`; applicable keys are nonnegative. Mouse selected snapshots require inside rank `0|1`, nonnegative screen/player keys, and gamepad key `-1`; gamepad selected snapshots require inside rank and screen key `-1` plus nonnegative player/angle keys. Empty selections carry all `-1` keys. Empty target ID and all `-1` keys are required when source is `None`.
- `TransferPlayerState`: `Baseline=0`, `Lightweight=1`.
- `TransferSessionSnapshot`: tick, highlighted target ID or empty, active target ID or empty, player state, transition-lock end tick or `-1`, last failure reason, and monotonically increasing state revision.
- `TransferAttemptResult`: tick, nullable aim-sample ID, selected target ID or empty, success flag, failure reason, resulting session snapshot, and optional state-changed or cleared payload. Failure carries neither state event and never increments revision.
- `TransferClearReason`: `ManualRecall`, `TargetRemoved`, `RoomLeaving`, `RunFailed`, `Cutscene`, `DemoCompleted`.
- `TransferStateChanged`: target ID, exact player modifier ID, exact target modifier ID, tick, revision.
- `TransferCleared`: reason, previous target ID, tick, revision.

Exact geometry and result ABI is scalar; no new public point/vector type is allowed:

- `TransferTargetDescriptor(string targetId, TransferTargetKind kind, int aimPointXQ1000, int aimPointYQ1000, int aimShapeCenterXQ1000, int aimShapeCenterYQ1000, int aimShapeHalfExtentXQ1000, int aimShapeHalfExtentYQ1000, string baseModifierProfileId, bool isAvailable)` with same PascalCase properties.
- `TransferCandidateObservation(TransferTargetDescriptor descriptor, SimulationTick observationTick, int worldAimPointXQ1000, int worldAimPointYQ1000, int worldAimShapeCenterXQ1000, int worldAimShapeCenterYQ1000, int worldAimShapeHalfExtentXQ1000, int worldAimShapeHalfExtentYQ1000, bool isTrigger, bool isLineOfSightOpen, int playerDistanceKey, bool mouseInsideShape, int screenDistanceSquaredKey, int gamepadAngleKey)` with same properties.
- `TransferSelectionSnapshot(SimulationTick tick, TransferSelectionSource source, string selectedTargetId, int insideRank, int screenDistanceSquaredKey, int playerDistanceKey, int gamepadAngleKey, TransferFailureReason failureReason)` with same properties.
- `TransferSessionSnapshot(SimulationTick tick, string highlightedTargetId, string activeTargetId, TransferPlayerState playerState, int transitionLockEndTick, TransferFailureReason lastFailureReason, int revision)` with same properties. Revision starts at 0 and increments with checked signed-32 arithmetic only for a committed apply or clear.
- `TransferAttemptResult(SimulationTick tick, long? aimSampleId, string selectedTargetId, bool success, TransferFailureReason failureReason, TransferSessionSnapshot session, TransferStateChanged? stateChanged, TransferCleared? cleared)` with same properties. A success has exactly one event payload; a failure has neither.
- `TransferStateChanged(string targetId, string playerModifierId, string targetModifierId, SimulationTick tick, int revision)` and `TransferCleared(TransferClearReason reason, string previousTargetId, SimulationTick tick, int revision)` with same properties.

Exact modifier IDs are `Player.Lightweight.v1`, `Target.Box.Heavy.v1`, `Target.Enemy.Heavy.v1`, and `Target.BossPayload.ScriptedHeavy.v1`. `TransferStateChanged` rejects any other player ID and any target ID outside that set; `TransferModifierRequest` rejects a target modifier ID that does not exactly match its target kind. IDs are payload identity, not permission for VD-02 to own target base state.

- `TransferModifierRequest(string targetId, TransferTargetKind kind, string baseModifierProfileId, string targetModifierId, SimulationTick tick, int proposedRevision)` with same properties.
- `ITransferTargetModifierSink`: engine-free target-owner boundary with exact members `string BoundTargetId { get; }`, `bool IsStillAvailable()`, `bool TryApply(TransferModifierRequest request)`, and `void Clear(TransferClearReason reason, SimulationTick tick, int revision)`. One sink instance is bound to exactly one registered target ID for its registration lifetime. `TryApply` returns true only after one reversible modifier handle is installed and must leave no change on false. `Clear` is idempotent. Availability preflight may not mutate state.
- `TransferTargetRemoved`: target ID and exact tick; it is consumed before same-tick transfer input.

Modifier values use Q1000 multipliers. Player Lightweight is gravity `650`, air acceleration `1250`, maximum fall speed `700`. Box Heavy is mass `3000`, gravity `2000`, explicit impact-damage multiplier `1500`. Enemy Heavy is gravity `2200`, move speed `750`, air control `350`, knockback resistance `2000`. `BossPayload` has no generic numeric physics profile: its owner performs the authored scripted reaction for `Target.BossPayload.ScriptedHeavy.v1`. The box owner exposes the explicit impact multiplier to VD-03; VD-02 never derives damage from Rigidbody mass and never emits `DamageRequest` itself.

## Frozen key and selection rules

- World transfer origin, target aim point, and target aim shape are Q1000. `playerDistanceKey` is `Round((dx²+dy²)/1000, AwayFromZero)` using signed-64 intermediates and nonnegative checked conversion. Keys `≤36000` are in range. This makes 5.999/6.000/6.001u produce distinct keys.
- Mouse observations are eligible only when source is mouse, the pointer-inside flag is true, screen coordinates exist, target is available, non-trigger, in range, and LOS open. Aim compensation accepts `MouseInsideShape` or `ScreenDistanceSquaredKey≤576`. Ordering is inside rank (`inside=0`) → screen-distance key → player-distance key → ordinal target ID.
- Gamepad observations are eligible only for `GamepadAngleKey≤180` under the same availability/trigger/range/LOS rules. Ordering is angle key → player-distance key → ordinal target ID.
- A currently highlighted gamepad target is retained while angle key `≤260`, distance key `≤36000`, availability and LOS remain valid. A new best target replaces it only if `newAngleKey+40≤currentAngleKey`. Exact 18.0°, 26.0°, and 4.0° boundaries are inclusive.
- Invalid candidates retain a deterministic best failure for feedback in this precedence: `TargetUnavailable`, `TriggerTarget`, `OutOfRange`, `LineOfSightBlocked`, `InvalidTarget`. This never mutates session state.
- Candidate observations with duplicate target IDs, negative keys, wrong observation ticks, or null/default descriptors fail fast as M1 contract errors and produce no selection or session mutation. M1 cannot independently recompute projection-derived keys because the frozen observation ABI intentionally contains no player-origin or camera object. Exact key recomputation and inconsistent-derived-value rejection belong to the Sol-frozen M2 observation builder before a public observation is constructed; M2 must test-pin that responsibility and may not move it into the selector.
- Authored target IDs are 1–96 ASCII characters from `[A-Za-z0-9._:/-]`, compared with `StringComparer.Ordinal`, and unique within the active room registry. Empty, duplicate, non-ASCII, or invalid-character IDs are rejected before a tick is evaluated. Registration and removal are explicit tick commands, not object-discovery order.

## M1 pure session rules

- M1 contains no Unity reference and owns one active target ID, one highlighted ID, transition-lock tick, player transfer state, last failure, and revision.
- `TransferPressed` with an active transfer never recomputes candidates. It recalls the active target after the transition lock; while locked it returns `Cooldown` without change.
- A successful transfer or recall at tick `t` blocks further state changes at ages 1 through 20 and accepts the next change at age 21 (`tick≥t+21`). Highlight evaluation may continue during the lock.
- A non-active press during an unexpired transition lock returns `Cooldown` before aim or selection validation. Outside that lock it requires an exact same-tick, matching `AimSampleId` and a Ready selection; null/mismatched/stale samples return `InvalidTarget` or `StaleAimSample` without partial mutation.
- Apply is a two-phase internal transaction: validate core state and target sink preconditions, request the target's reversible modifier, then atomically publish player Lightweight, active ID, event, revision, and lock. A rejected target modifier leaves player, target, revision, and lock unchanged and reports `ModifierRejected`.
- Recall clears the target modifier and publishes player Baseline, empty active ID, `TransferCleared(ManualRecall)`, revision, and a new 21-tick lock in the same commit.
- Target removal, room leaving, run failure, cutscene, and demo completion are idempotent. Clearing an active target calls the registered sink's idempotent `Clear` to remove only the VD-02 modifier handle; it never asks the target owner to restore or recreate base state. Every same-tick removal is processed before candidate evaluation, regardless of input order. The permanent `TransferTargetRemoved` command emits one `TransferCleared(TargetRemoved)` for an active target, removes the registration, does not start a new lock, and does not consume a same-tick press; any unexpired lock created by the prior successful apply remains authoritative. M4B2's internal `TransferTargetExposureEnded` may emit the same public clear reason when a scripted target leaves current targetability while retaining its pre-registered descriptor. Permanence is therefore owned by the internal input command, not inferred from `TransferCleared.Reason` alone.
- Lifecycle reset also clears the latest aim/highlight. No command, sample, target reference, or cooldown survives room/run/demo boundaries.
- Same-tick order is lifecycle clear (`RoomLeaving`, `RunFailed`, `Cutscene`, `DemoCompleted`) → temporary scripted exposure ends → permanent target removals → candidate/highlight evaluation → `TransferPressed`. An active same-ID temporary/permanent pair clears once and permanently unregisters afterward. Observations for IDs no longer present in the registry after the removal phase are excluded before selection and cannot restore a removed highlight. A lifecycle clear consumes transfer input and clears every registration plus aim, selection, highlight, active reference, and lock; its returned snapshot must equal the actual reset session. Active removal and a following press are both retained as distinct internal tick outcomes so neither event nor failure feedback is lost. Removing a non-active highlighted target clears highlight without a state revision. No press means no `TransferAttemptResult`; code must not fabricate `Success=false, FailureReason=None`. Choice-skill consumption occurs upstream under SYSTEM-CONTRACTS and means VD-02 receives no same-tick press.

## M2 Unity adapter rules

- A public `TransferTarget` MonoBehaviour is the only new public Unity component. Its serialized authored fields produce the descriptor; it contains no health, AI, base physics ownership, or device input.
- `ITransferTargetModifierSink` is a public engine-free interface implemented by target-owner adapters. It exposes deterministic availability/preflight, one reversible apply by modifier ID/tick, and idempotent clear. Runtime object references never enter events or snapshots.
- LOS uses a dedicated `TransferLineOfSight` layer mask, ignores triggers and the candidate's own collider, and linecasts from Q1000 player transfer origin to Q1000 aim point. Any other hit at or before the endpoint blocks. Equal hit fractions are ordered by stable scene plus hierarchy path, never instance ID or discovery order.
- Authored target transforms in the vertical demo use zero Z rotation and unit XY scale. The authoring validator rejects duplicate/empty IDs, nonpositive aim extents, target colliders marked trigger, target colliders included in the LOS mask, nonzero rotation, or non-unit scale.
- The simulation-camera projector consumes only `SimulationCameraPoseSnapshot`, never a live/render-smoothed Camera. M2 supports the approved orthographic pose and exact 1920×1080 normalized grid. Projection/key generation uses checked fixed/rational math; any trigonometric lookup or rounding table becomes test-pinned implementation data.
- The movement bridge maps `Player.Lightweight.v1` to the existing internal `PlayerMovementModifierKind.Lightweight` and maps clear to Baseline in the same transfer commit. It may add an internal movement friend/sink but no public movement member or second movement state.
- Unity scene objects are adapters around the pure selector/session. No callback, render frame, `Time.time`, instance ID, unordered object discovery, or solver state may decide selection or session transitions.

## Stop conditions

Terra must stop and report if camera projection cannot preserve the approved normalized-grid boundaries, LOS endpoint semantics differ from the contract, target modifier application cannot remain atomic and reversible, a real target requires VD-02 to own health/AI/base physics, a public movement change seems necessary, or any package/project-setting change is requested.

M1 may start after Luna confirms this pure contract has no P0/P1 contradiction. M2 may not start until Sol freezes the orthographic world-to-grid pixel-center/rounding sequence, rotation lookup data, mouse world-vector threshold quantization, LOS layer number and self/child/composite endpoint behavior, world-pose observation tick, and movement production sink seam in an addendum, followed by Luna pre-gate PASS.

## Sol M2 projection, LOS, observation, and movement-bridge addendum

This addendum is normative for the vertical demo only. It does not add a public payload type, enable gravity-direction changes, or authorize VD-07 device input, VD-03 health/AI, final UI, or package changes.

### Fixed camera and rational rounding

- M2 accepts only `OrthoSizeQ1000=10000` and `RotationQ10=0`. This is the approved 18 PPU, 640×360 logical canvas: 20 world units high and about 35.56 units wide. Nonzero rotation or another orthographic size is a contract error. The broader public snapshot range remains reserved for a later approved feature; the vertical demo uses no rotation lookup table because rotated gameplay cameras are explicitly unsupported.
- Camera center converts from Q100 to Q1000 by checked multiplication by 10. Target/player/shape geometry remains Q1000.
- `RoundDivAway(n,d)` requires positive `d`, uses checked signed-64 intermediates, and returns the nearest integer; an exact half increments magnitude away from zero. Overflow is a contract error and produces no observation or session mutation.
- The normalized aim grid is an endpoint-sampled integer lattice: left/bottom camera boundaries are coordinates `0`, right/top boundaries are `1919`/`1079`, and the even-sized camera center rounds to `960`/`540`.
- For world point `(x,y)`, camera center `(cx,cy)`, and `h=OrthoSizeQ1000`, projection is:
  - `gridX = RoundDivAway(((9×(x-cx)+16×h)×1919), 32×h)`
  - `gridY = RoundDivAway(((y-cy+h)×1079), 2×h)`
  Target projections are not clamped; checked conversion fails fast. Pointer samples are already clamped by the shared input contract.
- Inverse projection of pointer `(sx,sy)` to Q1000 world space is:
  - `pointerX = cx + RoundDivAway((2×sx-1919)×16×h, 9×1919)`
  - `pointerY = cy + RoundDivAway((2×sy-1079)×h, 1079)`
- `screenDistanceSquaredKey` is the checked signed-64 sum of squared differences between the integer pointer grid coordinate and integer projected aim point, followed by checked nonnegative signed-32 conversion. No square root or float participates.
- `MouseInsideShape` uses the inverse-projected Q1000 pointer and the axis-aligned world ellipse. If either absolute axis delta exceeds its positive half extent, the result is false. Otherwise each delta is normalized as `RoundDivAway(delta×1000000,halfExtent)` and the point is inside exactly when `nx²+ny²≤1000000000000`. Vertical-demo target transform Z rotation must be zero and XY scale exactly one, so no ellipse rotation is inferred.

### Mouse aim and gamepad angle keys

- Mouse pointer world position is inverse-projected and compared with the Q1000 player transfer origin. A new mouse direction exists exactly when `dx²+dy²≥2500`, corresponding to the inclusive `distanceSquared≥0.0025`; below it the caller retains the last valid sample and emits no replacement sample.
- Each Q4096 normalized mouse component is computed without floating point. For absolute component `a` and `s=dx²+dy²`, let `T=a²×4096²`. Find the largest `q` in `0..4096` for which `q²×s≤T`; increment `q` when `(2q+1)²×s≤4T`, including the exact half, then restore the component sign. All arithmetic is checked signed-64; overflow rejects the sample before state mutation.
- `gamepadAngleKey` uses integer CORDIC on `dot=aimXQ4096×targetDxQ1000+aimYQ4096×targetDyQ1000` and `absCross=abs(aimXQ4096×targetDyQ1000-aimYQ4096×targetDxQ1000)`. A zero target vector has key 0. Otherwise calculate the acute angle from `abs(dot),absCross`; if dot is negative, subtract it from `180000000` microdegrees. Round microdegrees to tenths of a degree with `RoundDivAway(angleMicrodegrees,100000)`.
- CORDIC vectoring runs iterations `i=0..23` using arithmetic signed right shift and the immutable microdegree table below. For `y>0`, update from the old pair as `x'=x+(y>>i)`, `y'=y-(x>>i)`, and add the table value; for `y<0`, use `x'=x-(y>>i)`, `y'=y+(x>>i)`, and subtract it; zero ends early. Every operation is checked signed-64.
- The table for `i=0..23` is exactly: `45000000, 26565051, 14036243, 7125016, 3576334, 1789911, 895174, 447614, 223811, 111906, 55953, 27976, 13988, 6994, 3497, 1749, 874, 437, 219, 109, 55, 27, 14, 7`.

### Fixed-tick pose and observation sequence

- The player transfer origin is the authoritative `PlayerMotionSnapshot` position converted Q4096→Q1000 with `RoundDivAway(value×1000,4096)`. No muzzle, reticle, render-transform, or facing offset changes range or LOS.
- At the start of simulation tick `t`, the transfer adapter runs before default-order movement, combat, target-owner, and interaction `FixedUpdate` work. It requires the player snapshot, simulation camera snapshot, and live target physics/transform pose to represent completed tick `t-1`; the camera tick must equal `AimSample.CameraPoseTick=t-1` and the sample tick must equal `t`.
- The adapter quantizes each target pose to Q1000 with `Math.Round(value×1000, AwayFromZero)` exactly once, then adds the descriptor's local aim point and shape center with checked arithmetic. Unit scale preserves half extents. The resulting public observation uses `ObservationTick=t`.
- `Physics2D.SyncTransforms()` runs once before registry pose capture and LOS queries because automatic transform synchronization is disabled. No render callback, `Update`, live `Camera.ScreenToWorldPoint`, instance ID, or unordered discovery contributes to an observation.
- Runtime target registration comes from an authoring-validated serialized target list sorted by ordinal `TargetId`; scene discovery order is forbidden. Observations whose IDs were removed earlier in the same tick are excluded as established by M1.

### LOS layer and hit semantics

- Sol assigns user layer slot **8** the exact name `TransferLineOfSight`; its mask is exactly `1<<8` (`256`). This is the only authorized `TagManager.asset` change. Target colliders remain outside this layer. The existing collision matrix is unchanged.
- LOS uses an explicit `ContactFilter2D` with layer mask 256 and `useTriggers=false`, independent of global `Physics2D.queriesHitTriggers`. The finite line segment runs from dequantized Q1000 player origin to dequantized Q1000 target aim point.
- A non-trigger layer-8 hit at fraction 0 while the origin is inside a blocker, at any interior fraction, or touching the endpoint at fraction 1 blocks. A collider beyond the endpoint is not queried. There is no epsilon shortening or extension.
- Colliders on the player root or its descendants and the candidate target root or its descendants are explicitly ignored even if mislayered. A `CompositeCollider2D` or Tilemap collider is otherwise one blocker and follows the same rule. The authoring validator still rejects target/self colliders on layer 8.
- For deterministic diagnostics, `fractionKey=Clamp(Round(hit.fraction×1000000,AwayFromZero),0,1000000)`. Hits sort by fraction key, then ordinal scene name, hierarchy path, collider type full name, and zero-based same-GameObject collider component order. Duplicate stable collider keys are authoring errors. Boolean LOS is blocked if any valid hit exists, regardless of returned query order.
- The nonalloc hit buffer contains 64 entries. Saturation is a fail-closed contract error with target ID and tick; it may not silently report open LOS.

### Unity target and movement seams

- `TransferTarget` is the sole new public Unity component. Its private serialized schema is target ID, kind, local aim point, local aim-shape center/positive half extents, base modifier profile ID, target collider, target pose root, and a `MonoBehaviour` target-owner sink reference that must implement `ITransferTargetModifierSink`. Public gameplay methods or device reads are forbidden.
- The authoring validator rejects missing/mismatched sink binding, empty/duplicate IDs, duplicate stable collider keys, missing scene/hierarchy names, nonpositive extents, trigger target colliders, target/self colliders on layer 8, nonzero Z rotation, non-unit XY scale, or an unsorted/implicit registry.
- A sandbox box-owner sink may cache and restore its own base Rigidbody2D mass/gravity scale and expose the explicit Q1000 impact multiplier; enemy and boss-payload sandbox sinks are isolated scripted owner stubs. VD-02 never owns health, AI, damage emission, or a generic boss physics profile.
- `AcadeGameMaker.Transfer.Unity` references Core, Transfer, Movement, and Movement.Unity. Transfer and Movement.Unity grant `InternalsVisibleTo` only to the transfer Unity/editor/test assemblies needed by this contract.
- `PlayerMovementController` gains an internal, production-named preflight and apply seam. Preflight requires the transfer tick to equal `NextExpectedTick` and mutates nothing. After the session returns, the bridge reflects only the outcome's final `TransferPlayerState` as existing `PlayerMovementModifierKind.Baseline|Lightweight`, then updates compatibility gravity scale before the movement motor begins that tick. It creates no second gameplay state, public member, or alternate gravity source.
- The transfer driver has execution order `-200`; the existing movement controller remains at default order. It preflights the movement seam before any target sink call. Once preflight succeeds, final modifier reflection is non-rejecting for that tick. Target apply/clear, session publication, and movement reflection therefore finish in one transfer phase before movement; a rejected target sink leaves all three unchanged.
- The internal fixed-tick outcome preserves both target-removal and same-tick press results. The bridge applies the final snapshot once, so a removal followed by an allowed new apply in the same tick does not expose an intermediate movement modifier to the motor.

### M2 authoring and verification gate

- Required files are the bounded Transfer Unity/editor/PlayMode paths, sandbox scene/prefabs, exact movement friend/seam files, and layer-8 `TagManager.asset` change listed at the top. No Input System asset, camera owner, combat, AI, persistence, UI, package, physics-setting, or unrelated scene change is allowed.
- EditMode tests pin every rational formula, signed half rounding, Q4096 normalization, CORDIC table/value boundary, invalid camera rotation/size, target authoring rule, registry ordering, and overflow-before-mutation behavior.
- PlayMode tests cover 23/24/25px, 17.9/18.0/18.1°, 25.9/26.0/26.1°, 3.9/4.0/4.1°, LOS origin/interior/endpoint/behind/trigger/self-child/target-child/composite/equal-hit/buffer cases, all three target modifier IDs, box apply/clear restoration, movement Lightweight→Baseline reflection, target rejection atomicity, active removal/lifecycle/same-tick press, scene/prefab authoring, and identical snapshots/events under 30/60/144 render grouping over the same 60Hz samples.
- M2 stops immediately if Unity's linecast cannot reproduce the endpoint cases without tolerance, CORDIC boundary tests disagree with the approved keys, the serialized registry requires unordered discovery, movement reflection can reject after a target sink mutation, or any implementation needs a new public runtime contract or another project/package setting.
