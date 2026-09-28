# InputRouter real-device integration — bounded design proposal

- Date: 2026-09-08
- Status: Draft for Astra approval; not an implementation contract
- Design: Sol (bounded design)
- Final authority: Astra under ADR-0027
- Candidate implementation / independent verification: Terra / Luna
- Parent authority: Approved VD-07, VD-09 and `SYSTEM-CONTRACTS`; Verified M5B1 and M5B2 gate sinks

## Outcome and honest boundary

The smallest cohesive useful slice is one real `GameInput.inputactions` asset, its checked-in Input System 1.20.0 generated wrapper, and one scene-bound `InputRouter` which exclusively owns `InputMode`, action-map activation, callback buffering and exact-next-tick publication to the already-real Movement, Transfer and Ordan Combat consumers.

This slice makes keyboard/mouse and XInput movement, jump, dash, basic attack and Transfer device input real. Attack and Transfer are emitted only when an exact prior-tick authoritative `SimulationCameraPoseSnapshot` is supplied; without that owner the router remains operational for Movement but publishes no aim-dependent press. This is a fail-stop dependency, not an invented camera implementation.

`ChoiceSkill`, `Interact`, `Pause/UI`, Run failure, boss-death transition coordination, rebinding/profile persistence and camera simulation remain unbound because no authoritative consumers/owners presently exist. Their actions and approved default bindings belong in the single asset now, but callbacks may only buffer router-private pending edges (or be explicitly ignored with diagnostics in development); they must not claim gameplay effects. No M5A binding or automatic boss-death lock is part of this unit.

## One owner, one phase

Extend the M5B3 `AcadeGameMaker.Input` assembly (or add a narrowly named Unity integration assembly if its dependency direction requires separation) to reference Core, Movement.Unity, Transfer.Unity, Combat.Unity and `Unity.InputSystem`. `InputRouter` is a single component with `[DefaultExecutionOrder(-210)]`, serialized exact references to the player Movement controller, one Transfer driver and one Ordan Combat driver, plus an explicitly registered camera snapshot provider. There is no scene discovery fallback, static singleton state or second mode latch.

At initialization, the router:

1. validates exact non-null graph references and that all three consumers bind the same player;
2. registers its own object identity once with all three input-gate sinks;
3. creates the generated `GameInput` wrapper and subscribes directly to action callbacks;
4. establishes `GameplayEnabled` as an authored initial mode only when explicitly configured; otherwise it starts locked in `Transition`;
5. enables exactly one map: `Gameplay` for `GameplayEnabled`, `UI` for `UIOnly`, and neither for `Cutscene`, `Transition` or `Ended`.

The camera provider may validly begin in `BootstrapPending` with no publication. The router must not treat the Movement motor's pristine `Snapshot.Tick=t0` as a completed `t0-1` pose or request a relabeled camera seed. At first input tick `t0`, Movement publication proceeds normally and a legal Transfer edge may be emitted as `TransferPressed(null,t0)`; camera/aim and basic attack remain absent. After Movement and camera complete `t0`, ordinary aim-dependent publication begins at `t0+1` from the exact completed pair.

Only `InputRouter` may change map enablement. External systems receive an owner-bound `RequestInputMode(requester, mode, effectiveTick)` seam after explicit requester registration; requests must target the current player `NextExpectedTick`. This design does not invent which future coordinator is entitled to request boss death, Run failure, pause or choice transitions.

Every `FixedUpdate` at `-210` uses `tick = player.NextExpectedTick`. It first atomically commits any exact pending mode transition, then freezes callback state into a tick-local immutable command batch, clears only the frozen callback accumulator, and submits the batch for this same tick before `-200`. Thus device callbacks observed since the previous input phase are consumed at most once by the next 60 Hz simulation tick. Render-frame grouping never creates additional command edges.

## Atomic cross-consumer input frame

Sequential calls to the current `ApplyInputGate` and `Submit` methods are unsafe: a later consumer rejection can leave an earlier consumer mutated. Separate gate and command transactions also create an avoidable intermediate generation and make a same-tick unlock-plus-fresh-input frame difficult to prove. The least risky API is therefore one combined staged frame per consumer.

- `PrepareInputFrame(owner, tick, locked, optionalInteractiveInput)` performs every existing owner, initialization/preparation, exact-tick, own-phase, queued-envelope and input validation. It constructs an opaque instance-bound candidate containing the complete resulting interactive queue, resulting latch, dropped count, and Movement's `resetLifecycleBuffers` bit. It performs no mutation. `optionalInteractiveInput` is absent in locked modes; in Gameplay it contains the consumer's exact tick command/fields.
- Transfer and Combat preparation rebuild their complete dictionaries once. They validate every old envelope, strip old interactive fields when entering/staying locked, preserve ordered maintenance/exposure/lifecycle or external damage, and consumer-own the merge of the new same-tick interactive fields with an existing system-only envelope. They reject an existing interactive conflict. Future keys and terminal semantics are preserved exactly.
- Movement preparation copies its sorted command dictionary, clears it on either gate-state transition as already required by M5B1, optionally inserts the fresh current-tick command only when the resulting mode is unlocked, and preserves external directive queues/receipts. Same-state unlocked preparation must retain existing queued commands and reject a duplicate current tick rather than overwrite it.
- `CommitInputFrame(owner, candidate)` is an internal trusted commit used only after all three preparations succeed. It swaps the already-created queue and latch and performs the precomputed Movement lifecycle reset on a transition into lock. It does no allocation, enumeration, input validation, provider access or user callback. Candidate ownership, consumer generation and single-use validity are established during preparation; they may be asserted for programmer-error diagnostics, but commit exposes no ordinary recoverable rejection branch.

Concrete overload shape (names may change without changing semantics): Movement accepts `MovementCommand?`; Transfer accepts `SimulationCameraPoseSnapshot?`, `AimSample?`, and `TransferPressed?`; Ordan Combat accepts the same camera/aim pair and `BasicAttackPressed?`. Each returns its own sealed nested candidate type, inaccessible to other consumers. The candidate captures consumer reference, registered owner reference, tick, source generation, complete replacement queue, resulting lock state, dropped count and any consumer-specific commit bit. Preparation rejects a source-generation mismatch before returning. Commit is `void`, marks the candidate consumed and performs only fixed field assignments plus Movement's existing nonthrowing reset. Public/test code cannot construct a candidate.

The router first freezes its proposed mode and semantic batch, then prepares Movement, Transfer and Combat candidates in that order. If any preparation fails it commits none, preserves its old `InputMode`, maps, callback accumulator and every consumer, and fail-stops the input phase. After all candidates exist, it commits all three at `-210`, then changes the router mode/map as the final local publication and clears the committed callback batch. Candidates are instance-bound and single-use. Every alternate producer mutation increments the relevant consumer generation; because preparation and the three commits execute synchronously on Unity's main thread with no callbacks between them, all expected generations remain fixed.

This gives all-or-none behavior for every anticipated contract/domain rejection. It does not claim rollback from process termination, out-of-memory during a field assignment, reflection, or arbitrary engine corruption. Tests must fault each prepare position and prove no consumer/router mutation, inspect that commit has no ordinary fallible work, and exercise stale/reused/cross-consumer candidates, combined lock, unlock-plus-fresh-command, same-state and system-envelope merge boundaries. Do not weaken the existing exact tick or own-phase checks. The older `ApplyInputGate`/`Submit` seams may remain for their already-verified fixtures and non-router producers, but once the router owner is registered its interactive lane uses only the combined frame API; every successful legacy/system queue mutation increments the same generation. The two paths share validators rather than duplicating behavior.

For all non-`GameplayEnabled` modes the consumer gate value is `locked=true`. Mode-to-mode changes among locked modes retain the three gate states but still clear router-local held vectors, edge buffers, pending aim and press data. Returning to Gameplay uses `locked=false`; pre-lock commands can never regenerate.

## Callback accumulation and command publication

Callbacks never touch simulation state. They update only router-private values:

- `Move`: latest `Vector2`; sampled once at `-210`, clamped per axis and quantized using the existing Movement command contract.
- `Jump`, `Dash`, `Attack`, `Transfer`, `ChoiceSkill`, `Interact`, `Pause`: rising-edge counters/booleans, coalesced to at most one semantic press of each kind for the next tick. Cancel does not erase an already-recorded press.
- `Aim`: latest raw gamepad stick; values with magnitude squared below `0.04` do not replace the last valid aim. The callback must read the bound `StickControl.ReadUnprocessedValue()` (and validate that exact control type/path), not `CallbackContext.ReadValue<Vector2>()`, because the Input System stick control's default deadzone processor would otherwise change the approved raw 20% threshold and Q4096 result. No global Input System setting is changed.
- `Point`: latest mouse position converted with checked deterministic rounding to signed actual output pixel coordinates. A 2560×1440 pointer remains in that coordinate space in the callback accumulator; negative/off-window coordinates are preserved so they cannot be mistaken for an inside point. No callback-time viewport clamp or 1920×1080 normalization is allowed. Inside/outside, nearest-edge clamp, and conversion to the canonical `0..1919 × 0..1079` aim grid are evaluated at the next input tick against the exact completed camera snapshot's viewport and gameplay rectangle.

### Held buttons across a locked epoch

Do not add a parallel raw-event command path or a sticky per-control quarantine for the initial router slice. The installed Input System 1.20 documentation and implementation give the required minimum behavior directly: Button actions with `initialStateCheck=false` do not perform when their map is enabled while the physical button is already held; that button must be released and pressed again before the action triggers. All Gameplay edge actions (`Jump`, `Dash`, `Interact`, `ChoiceSkill`, `Attack`, `Transfer`, `Pause`) must remain Button actions with initial-state check disabled in the asset and generated wrapper.

On transition out of Gameplay, the router first clears its recorded button edges as part of the committed locked epoch, then disables the Gameplay map. On transition back, it enables Gameplay only after the three consumer frame commits. A button held throughout lock therefore produces no callback and no synthetic command. If it is released and freshly pressed after re-enable—even when both device events occur before the next FixedUpdate—the normal direct wrapper `performed` callback records exactly one fresh edge. A release and repress that both occurred while the map was disabled remain intentionally unavailable and cannot be replayed on unlock.

Value actions are deliberately different: Input System always performs an initial state check for `Move`/`Aim`. Current held Move may therefore resume after unlock, which is a fresh sampled axis state, not an old buffered button edge. This slice does not invent a neutral-before-move policy. Aim still passes the approved raw-stick threshold and last-valid-direction rules before becoming a semantic sample.

Only direct generated-wrapper callbacks create router pending gameplay values/edges. No `InputSystem.onEvent`, `onAfterUpdate`, global settings change, legacy polling or synthetic callback is needed. A callback whose `context.control` is null, no longer added/enabled, not a control bound to that exact generated action, or belongs to a disabled/nonactive map is rejected/ignored diagnostically before buffer mutation. Device removal clears that device's presentation identity and any uncommitted value state but never fabricates a cancel, release or gameplay press. Tests must disable/remove devices and prove queued state events cannot create commands.

At `-210`, Gameplay mode builds exactly one Movement command for `tick` and submits it to `PlayerMovementController`. The implementation must map the existing command fields rather than introduce a parallel movement DTO. Transfer and Combat receive separately constructed immutable inputs for the same tick and same optional `SimulationCameraPoseSnapshot`/`AimSample`. The same monotonically increasing sample ID and exact sample value are used by both consumers. Presses carry that sample ID through the existing contracts.

Mouse is the active aim source after a Gameplay `Point` event; gamepad is active after a Gameplay `Aim` event at or above the raw deadzone. Device/control-scheme display state is non-authoritative. A mouse aim sample requires a camera snapshot whose `CameraPoseTick == tick - 1` and the exact completed player pose/aim origin for that same source tick. The camera owner supplies the frozen snapshot through a bound provider. `InputRouter` first maps the signed actual pixel through the snapshot's integer scale/offset to the approved normalized endpoint grid `0..1919 × 0..1079` with AwayFromZero rounding; outside points retain `IsPointerInsideGameplayRect=false` while their conversion uses the nearest gameplay edge. It then reuses the existing `TransferFixedMath.InverseGridToWorld` Q1000 path (including its fixed `OrthoSizeQ1000=10000`, rotation `0` validation) and derives the player-to-pointer Q4096 direction. The integration assembly receives only the narrow internal visibility needed to call that canonical helper; it must not copy the formula. It must not introduce a divergent projection, call `Camera.ScreenToWorldPoint`, read a render-smoothed Transform, or put actual output pixels in `AimSample.ScreenPixel`.

A missing never-yet-valid aim does not suppress a legal Transfer press: the router emits `TransferPressed(null, tick)`, which gives `InvalidTarget` when inactive and still permits the authoritative active-transfer recall path. An invalid or stale required camera/pose is never paired with aim and is diagnosed; Attack is suppressed because its contract requires aim. Transfer still carries a nullable-aim press so active recall remains possible and inactive behavior remains the existing deterministic failure. A mouse press originating outside the gameplay rectangle is suppressed entirely, including during an active transfer, as required by REQ-UX-013. No stale sample is reused and Movement continues.

The combined `PrepareInputFrame` stages submission before the first consumer mutation, so Movement publication cannot precede a later Transfer duplicate failure. After successful preparation the router commits Transfer, Combat and Movement candidates. This order only swaps queued frame state; simulation remains `-200 Transfer -> -190 Combat -> default Movement`.

System-owned Transfer maintenance and Combat external damage are not created or interpreted by the router. Existing producers retain their lanes. The consumer-owned combined candidate composes router fields with an already queued same-tick system envelope, preserving maintenance/damage and rejecting conflicting interactive fields; it never overwrites a whole dictionary entry or treats a duplicate as harmless.

`AimSample.ScreenPixel` therefore remains the existing normalized-grid coordinate and Core `AimContracts` remains unchanged. Actual output pixels exist only in the router's private pending callback state.

## `GameInput.inputactions` and generated wrapper

The asset contains exactly two maps and enables C# generation (`GameInput`, stable namespace chosen by the implementation contract):

- `Gameplay`: `Move` Vector2; gamepad-only `Aim` Vector2; mouse-only `Point` pass-through Vector2; buttons `Jump`, `Dash`, `Interact`, `ChoiceSkill`, `Attack`, `Transfer`, `Pause`.
- `UI`: `Navigate` Vector2, `Point` Vector2, `Click`, `ScrollWheel` Vector2, `Submit`, `Cancel`.

Defaults are the Approved VD-07 bindings expressed by M5B3: WASD/left stick Move; Gameplay Point is mouse position and Gameplay Aim is right stick; Space/A Jump; left+right Shift/B Dash; E/X Interact; Q/RB ChoiceSkill; left mouse/RT Attack; right mouse/LT Transfer; Esc/Start Pause. UI uses WASD+arrows/D-pad+left stick Navigate, mouse Point/Click/ScrollWheel, Enter+Space/A Submit and Esc/B Cancel. No JKL combat, click movement or virtual mouse exists. Control schemes are `KeyboardMouse` and `Gamepad` with paired keyboard+mouse requirements for the former. The wrapper is the M5B3-fixed `AcadeGameMaker.Input.GameInputActions` at `Assets/AcadeGameMaker/Runtime/Input/GameInputActions.cs`.

The `.inputactions` source is authoritative and the generated `.cs` is checked in and must match a clean Unity 6000.6.0f1 / Input System 1.20.0 regeneration. Handwritten lookalike wrappers and placeholder facade classes are forbidden. Static tests inspect map/action/binding topology, generator metadata, package version and `activeInputHandler: 1` (Input System only).

## Lifecycle and failure semantics

- `OnDisable` unsubscribes callbacks, disables both maps and clears only router-local buffers. It must not silently unlock consumers.
- Destruction disposes the generated wrapper after unsubscription. Re-enable either revalidates the same registered owner graph and reapplies the already-authoritative mode, or rejects; it does not create a new owner identity.
- A mode request, gate prepare, command prepare or provider validation failure preserves the previous committed mode, all consumer queues/latches and the callback accumulator so the exact batch can be retried or explicitly cleared by a lifecycle decision. No partial map switch occurs.
- `Ended` is terminal for this router instance. Unlock from `Ended` requires a new explicitly approved run/bootstrap lifecycle contract, not a convenient method call.

## M5B5 preflight closures for Astra

### Atomic registration

Initialization needs read-only registration preflight on all three consumers. Add allowlisted `CanRegisterInputGateOwner(object owner)` methods which validate non-null owner, pristine-or-same-owner state, required initialized/prepared graph state, and no incompatible terminal state without mutation. InputRouter validates its entire serialized graph, then calls all three `CanRegister...` methods, and only if all pass invokes the existing registrations synchronously on Unity's main thread. Registration performs no allocation or callback and cannot ordinarily reject after successful preflight with no intervening mutation. A retry may accept the same router already registered on a subset, but a different owner anywhere rejects before changing the remaining consumers. Fixtures inject each failure position and prove zero partial registrations.

Registration itself is not rolled back and no unregister API is introduced. Once any consumer accepts the router identity, that identity is durable for the graph lifetime.

### Terminally retired Ordan Combat

The boss Combat session intentionally stops advancing after M4B3C and its component is disabled, while the player clock continues. Requiring `Combat.NextExpectedTick == player.NextExpectedTick` forever would turn successful teardown into a permanent InputRouter exception. Conversely, treating a merely disabled component as retired would weaken teardown evidence.

Bind the exact authored `OrdanBossTerminalTeardown` as part of this boss-router graph (narrow friend access only). Add a read-only post-terminal `MatchesRetiredInputGraph(player, transfer, combat)` seam because the current receipt contains value evidence but not component references and the existing authoring validator is not a post-disable identity API. The router may latch `CombatRetired` exactly once only when that identity seam passes, teardown has its exact terminal receipt and cleanup horizon, Combat is disabled, the player has advanced beyond the cleanup tick, and the router's committed mode and remaining consumer gates are locked. From then on it omits Combat from frame preparation/validation/commit and never calls its stale exact-phase candidate seam. It continues only locked Movement/Transfer maintenance frames. `GameplayEnabled` is rejected while `CombatRetired`; a later playable segment must explicitly replace/register a new Combat consumer under a separate Approved lifecycle contract. Missing, cross-wired, partial or merely disabled teardown is a fail-stop and preserves router state.

This does not make the router a boss-death coordinator and does not consume reward/Run/choice facts. It only acknowledges the already-authoritative terminal receipt so input plumbing cannot resurrect or continually address a quiesced owner.

### Mixed-source aim ordering and IDs

Accepted Gameplay `Point` and raw-stick `Aim` callbacks share one checked, router-local, monotonically increasing callback ordinal within the current mode epoch. Each source retains its latest value and ordinal. At `-210`, the qualifying source with the greater ordinal wins; this represents Input System's finite callback processing order and uses no callback timestamp, wall clock, Unity instance ID, device ID, render frame number or control-scheme display state. Cross-device callbacks arise from distinct processed events, so equal ordinals are impossible; overflow rejects the callback before pending-state mutation and fail-stops the epoch rather than wrapping. Lock/mode change clears both source records and resets the epoch only after the atomic gate commit.

`AimSample.SampleId` is a separate checked monotonically increasing publication sequence. It advances exactly once only after a valid completed camera/player pair and a nonzero sample have passed all three frame preparations and the frame commits. Missing camera, neutral stick, invalid mouse distance, suppressed bar press, failed candidate, or a diagnostic-only unbound action consumes no sample ID. Render grouping of the same ordered device-event script therefore yields the same source and sample IDs.

### Source-specific missing-camera behavior

Missing/stale camera or completed-player provenance is a `NoAimPublication` disposition, not a global frame failure. Mouse Point cannot establish canonical grid, inside status or direction, so mouse Attack and Transfer presses are suppressed. Gamepad Attack is also suppressed because current Combat input requires an exact camera/aim pair even though stick direction itself is available. Gamepad Transfer remains `TransferPressed(null,tick)`, preserving active manual recall and inactive `InvalidTarget`. A valid camera with never-valid/neutral gamepad aim has the same nullable Transfer behavior. No source may borrow the other source's stale aim after a mode-epoch clear.

Diagnostics are bounded immutable last-disposition/counter state, not gameplay events. Unbound ChoiceSkill, Interact and UI actions may update only that bounded diagnostic and are never assigned gameplay command IDs, queued across ticks or reported as implemented effects.

### Cross-phase fail-stop receipt

Unity does not abort the remaining fixed-update callbacks when `InputRouter.FixedUpdate(-210)` throws. Without an additional seam, Transfer `-200`, Combat `-190` and Movement default order would consume empty/old queues and advance their clocks while the router retained the frozen callback batch for retry, causing that batch to target the wrong tick. Candidate atomicity alone does not close this failure.

Add a separate opt-in coordinated-input lane to each consumer. Existing gate registration, standalone `Submit`, `ApplyInputGate`, test entry points and uncoordinated graphs retain their verified behavior until this lane is explicitly registered. Initialization performs read-only `CanRegisterCoordinatedInputOwner(owner)` on all three alongside gate preflight, then durably registers the same router identity only after every preflight succeeds.

Add a minimal engine-free Core contract: immutable `InputFrameCommitReceipt(ownerEpoch, frameOrdinal, tick)` plus read-only `IInputFrameCommitSource.CurrentReceipt`. Epoch/ordinal are checked nonnegative `long` values and tick is the exact nonnegative `SimulationTick`; equality is value-only. The interface has no mutation method, callback, Unity type or device data. The exact InputRouter implements it. Each consumer registers the same source reference together with the same owner reference and rejects a different source/owner pairing.

For each router tick, construct one shared receipt value. `ownerEpoch` and `frameOrdinal` are router-local monotonic values, never wall-clock, render-frame, device or instance IDs. The same value is passed into all three `PrepareInputFrame` calls and captured by their private candidates. Each local commit records only a private pending receipt key and resulting locked state, even when the frame contains no interactive input. It does **not** make that consumer phase-eligible by itself.

Only after all local commits, authoritative mode assignment, exclusive map switching and callback-suppression exit succeed does InputRouter atomically replace `CurrentReceipt` with the shared value as its final field assignment. This read-only publication is the global barrier. A callback occurring during map switching cannot enter the frozen frame and is handled only after suppression ends under the newly committed mode.

At the very start of each phase—before `_attempted`, queue removal, terminal preflight, physics query, motor prediction, session mutation or publication—an opted-in consumer requires (a) its unconsumed private pending key, (b) `registeredCommitSource.CurrentReceipt`, and (c) exact equality between both values, the phase tick and the next epoch/ordinal expected by that consumer. It then enters its existing phase path. Receipt consumption follows that consumer's already-approved phase/consume-on-attempt boundary; this seam does not promise retry after a later domain failure or weaken existing fail-stop behavior. Missing, stale, future, duplicate, cross-source or unequal global/local receipts cannot advance a consumer.

Thus, if `-210` rejects before the trio commit, all later consumers reject the same global tick before advancing. If an allegedly nonfallible later local commit or map operation unexpectedly throws after an earlier local commit, the router never publishes the new global receipt; even the earlier consumer is therefore ineligible. This is fail-stop containment, not rollback: partial local pending keys/generations require an explicit diagnostic discard/rebootstrap path before retry, and the router must not claim ordinary automatic recovery. Frozen callbacks and sample IDs remain unconsumed until the global receipt publication succeeds.

Bootstrap registration activates receipt enforcement before the first `t0` phases. The router commits an explicit empty/nullable bootstrap frame receipt for all three at `t0`; Movement and nullable Transfer may proceed while aim remains BootstrapPending. No consumer is allowed a one-tick uncoordinated bootstrap exception.

For terminal cleanup tick `d+1`, `-210` still commits the exact locked local keys for all three and publishes their shared global receipt. Transfer consumes its receipt at `-200`; M4B3C teardown at `-195` must validate both the global receipt and Combat's exact still-unconsumed local key, then terminal-close Combat's key as part of the existing delivery/input discard transaction before disabling Combat. This terminal close is not a Combat simulation advance and publishes no Combat outcome. Movement consumes its own locked `d+1` receipt normally. On later ticks the router's receipt roster omits Combat only after the previously specified receipt- and identity-proven `CombatRetired` latch; Movement and Transfer continue to require matching local/global receipts. A disabled Combat component with no exact terminal close remains an error, not retirement.

Minimal runtime allowlist for this closure is the additive engine-free Core receipt/interface file, the router, the three consumers' coordinated-lane registration/local-key checks, and the Ordan terminal teardown/terminal-discard seam needed to close Combat's last key. Existing camera/Aim/Transfer/Combat public DTOs remain unchanged. No simulation algorithm, target selection, damage, Transfer maintenance, Movement physics, action asset, settings, scene discovery or Run/choice behavior changes.

## Acceptance shape for a future Approved contract

- Real Keyboard, Mouse and Gamepad Input System device events drive Movement and, with a valid camera provider, the existing Transfer and Ordan Combat phases exactly once on the next tick at 30/60/144 render grouping.
- Static inspection proves one asset, exactly two maps, exact approved bindings, generated-wrapper provenance, direct callbacks, Input System-only settings and no legacy/SendMessage/virtual-mouse path.
- Injected failure at each gate and command prepare boundary is mutation-free across all consumers and router state; successful prepare/commit changes all intended consumers together.
- Lock strips old exact/future interactive work across all three consumers, preserves Transfer maintenance, Combat damage and Movement external directives, and never replays old work after unlock.
- Missing/stale camera suppresses only aim-dependent publication and cannot fabricate aim; outside-rectangle mouse presses are suppressed while edge-clamped direction remains available for presentation/aim.
- Bootstrap explicitly proves no camera/AimSample/basic attack at `t0`, successful Movement and nullable Transfer behavior at `t0`, first camera publication after completed Movement `t0`, and first aim-dependent consumer input at `t0+1`.
- Unowned `ChoiceSkill`, `Interact`, Pause/UI, Run and boss-death paths are demonstrably not claimed by tests or evidence.

## Decisions within delegation and actual escalation boundary

The `-210` phase, candidate/commit atomicity, consumer-owned merge, generated-wrapper requirement, exact camera dependency and bounded non-ownership are technical consequences of existing Approved contracts and current code; they do not require a new product decision. Astra must approve their normative work-contract form before Terra implementation.

No genuine user product decision is required for this slice. Initial-mode authoring can safely default to locked `Transition`, and unavailable consumers can remain unbound without changing approved behavior. A user decision becomes necessary only if scope is expanded to define presently absent product behavior (for example what Interact does, a new Pause flow beyond the already approved top-level rule, or a change to the approved bindings/maps). Selecting which existing future subsystem implements camera, Choice or Run is an architecture allocation for Astra, not a product choice.
