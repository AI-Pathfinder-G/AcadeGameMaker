# VD-09 M5D7M desktop profile launch adapter — Luna contract pre-gate

- Contract: [Draft M5D7M](../specs/work-contracts/2026-09-13-vd09-m5d7m-desktop-profile-launch-adapter.md)
- Reviewer: Luna (independent; Astra owns approval)
- Review date: 2026-09-13
- Compared with: Verified M5B5 and M5D7L, M5D7D/E/G/H/I/J/K boundaries, current `InputRouter`, and the stated exact allowlist

## Scope conclusion

The revised draft closes the failure-bypass problem identified by Sol: reservation is acquired before profile preparation, the router's `Awake`/`Start` matrices suppress legacy fallback for Reserved/Failed states, and only the reservation owner may fail or adopt the launch. The proposed `-220` adapter / `-210` router ordering is implementable for active, enabled objects entering the same Unity lifecycle cohort; `_awakeEntered` and the explicit cohort checks correctly reject late, inactive, disabled, or cross-cohort activation.

The adapter remains a preparation-to-handoff boundary. It does not claim a hub scene, UI hierarchy, notification presentation, gameplay transition, profile/run mutation, or persistence retry. The new test seam is inside the existing Input.Unity assembly and does not require an asmdef or Profile API change.

## Adversarial closure review

- **Router Start matrix:** `Unreserved` retains M5B5 legacy initialization; `Initialized` is a no-op; `Reserved`/`Failed` never allocate or initialize; `Adopted` without prior owner initialization fails closed. This directly prevents a failed M5D7L preparation from falling through to defaults.
- **Active same-cohort ordering:** `DefaultExecutionOrder(-220)` plus active/enabled/cohort and router `_awakeEntered` proof is sufficient to make reservation/adoption precede router `Awake`; same-order competing adapters are explicitly winner-independent and the loser has no reservation authority.
- **Ownership and callback closure:** all-or-nothing adoption, exact owner/action binding, one-shot take, idempotent disable/unsubscribe/dispose, and preservation of an already-propagating exception over cleanup exceptions are closed. Adapter cleanup is forbidden after router ownership transfer.
- **M5D7L evidence:** the receipt must retain both initial and final binding results, all preservation and persistence fields, and a defensive final document. Typed preservation/save failures reach UIOnly as pending evidence; protocol/programmer/fatal errors produce no receipt or notification.
- **Notification:** priority is deterministic (`PersistenceDeferred` > `RecoveryArtifactPreservationFailed` > `RecoveryCompleted` > none), overlapping facts are covered, and consumption is exactly once with a default output on absence/replay. The payload has no path, exception, clock, action, scene, or prose authority.
- **Legacy/noninterference:** the current router's existing graph and callback ownership can be extended additively with the named private state/closure seams. No scene lookup, persistent-path access outside the supplied environment port, device inspection, logging, UI, gameplay, or new save authority is introduced.

No P0 or P1 contradiction remains in the revised contract. The named router methods and state transitions are implementable within the allowed InputRouter edit; the adapter and tests can use existing Input.Unity friend access. No user decision is required under the stated no-hub/no-UI boundary.

## P2 observations

### P2-001 — Post-initialization evidence return shape is left to implementation

The contract requires the adapter to validate exact adopted identity and UI/map evidence after `InitializePreparedHub(this)`, but does not prescribe whether that method returns a typed internal proof or performs the complete validation internally. This is not a blocker because either shape can remain within the allowlist; the implementation should make the proof explicit and test it without exposing live actions through the receipt.

### P2-002 — Notification-to-receipt field equality is descriptive rather than tabulated

The draft requires exact correlation evidence and priority validation but does not list a separate field-by-field equality table. Implementer/tests should construct notifications only from the same validated local receipt projection and mutate each correlation field in reflection tests. This is precision guidance, not an authority or implementability blocker.

## Acceptance matrix

| Criterion | Pre-gate status | Assessment |
|---|---|---|
| AC-M5D7M-001 | PASS by design | Literal Reserve→environment→Prepare order, single reads, validation, and forwarding are explicit. |
| AC-M5D7M-002 | PASS by design | All M5D7L source/recovery/preservation/save rows map to typed UIOnly receipt evidence. |
| AC-M5D7M-003 | PASS by design | Exact candidate/adoption identity and idempotent router closure are closed. |
| AC-M5D7M-004 | PASS by design | Every pre-receipt failure suppresses fallback/receipt/notification and closes owned resources. |
| AC-M5D7M-005 | PASS by design | Owner, state, cohort, lifecycle, action, and reflection-corruption rejection rules are closed. |
| AC-M5D7M-006 | PASS by design | Notification priority, overlap, one-shot consumption, and immutable correlation are explicit. |
| AC-M5D7M-007 | PASS by design | Active -220/-210 Awake/Start order and lifecycle interleavings are testable without assuming same-order winner. |
| AC-M5D7M-008 | PASS by boundary | M5B5/M5D7L regressions remain required; scope excludes all forbidden authority. |
| AC-M5D7M-009 | DOWNSTREAM | Requires implementation, focused/full execution, and Luna post-review; no execution is claimed here. |

## Verdict

**PASS — P0=0, P1=0, residual P2=2.** Recommend Astra advance the bounded draft to Approved. This is an independent pre-gate recommendation only; Luna does not change the contract status.

## R2 lifecycle re-gate after R5/R6 redesign

- Review date: 2026-09-13
- Review scope: lifecycle clauses revised after the real Unity R5/R6 ordering failures; no runtime, test, or contract implementation changes by Luna
- Compared against: revised M5D7M draft, current M5B5 lifecycle boundary, and the required prepared-launch ownership model

### Lifecycle assessment

The redesign closes the prior assumed-`Awake`-order defect. Making router `Awake` allocation-free and callback-free gives either component a valid first step: an adapter may reserve/adopt before router `Awake`, or router `Awake` may enter first and leave the router pristine for a later reservation. The hard boundary is now `Start`/default allocation/callback registration/initialization, which is implementable and is the right boundary for suppressing legacy fallback.

The revised state sequence is coherent:

`Unreserved → Reserved → Adopted → InitializedPendingPublication → Initialized`

`Reserved` and `Adopted` router `Start` are inert, so a router `Start` that precedes adapter `Start` cannot allocate defaults or initialize the graph. `InitializePreparedHub(owner)` is the only prepared initialization transition; callback registration occurs on the exact adopted wrapper there, after router `Awake`; `ConfirmPreparedHubPublication(owner)` is a non-allocating final transition after all fallible receipt/notification work. This closes the former callback-before-adoption and post-initialization-publication gaps while preserving the exact owner/action pair and fail-closed cleanup.

The four required permutations are meaningful and sufficient as a lifecycle matrix: same-object router-added-first, same-object adapter-added-first, separate-object router-created-first, and separate-object adapter-created-first. They must be activated as cohorts without asserting which `Awake` runs first. The additional late-reservation, router-`Start`-before-adapter-`Start`, callback-fault, adapter-failure, and disable/destroy rows make the fallback and ownership boundary testable. Relative execution-order attributes remain only a hint, not an authority.

Legacy compatibility is bounded honestly: `Unreserved` router `Start` and legacy `InitializeForTests` share `EnsureLegacyActionsAndCallbacks`; prepared states cannot call that helper; router `Awake` defers only internal action/callback allocation until the existing initialization entry point. No scene, UI, profile, save, or gameplay authority is added.

### Remaining lifecycle precision issue

### P1-R2-001 — router lifecycle hooks omit `InitializedPendingPublication`

The revised state/method matrix correctly says `FailPreparedHubLaunch(owner)` accepts `InitializedPendingPublication` and that AC-M5D7M-004 covers initialized-pending-publication abort. However, the general lifecycle sentence still says router `OnDisable`/`OnDestroy` fail the reservation only “while Reserved/Adopted” (line 35 of the revised draft), omitting `InitializedPendingPublication`. That is an ambiguity at the exact window between `InitializePreparedHub` and `ConfirmPreparedHubPublication`: a router disable/destroy could otherwise leave initialized maps/actions live without a receipt unless the implementer infers the missing third state.

Required amendment: explicitly include `InitializedPendingPublication` in the router `OnDisable`/`OnDestroy` fail/close rule, and state that the owner-only pending abort leaves both maps disabled, no receipt/notification, and exact-once closure. Keep the existing post-publication rule that adapter-only disable/destroy does not tear down an initialized router.

### P2-R2-001 — legacy test-entry timing should be stated explicitly

The draft now gives `InitializeForTests` the common legacy ensure helper but does not state whether a call before router `Awake` is supported or must reject. Existing fixtures generally configure an inactive object, activate it, then initialize; the contract should document that supported timing so tests cannot accidentally create callbacks before the `Awake` proof. This is not a blocker because the prepared state is explicitly forbidden and the legacy seam is otherwise closed.

### R2 verdict and AC impact

The redesigned ordering itself has no P0. With the omission above, this lifecycle re-gate is **FAIL — P0=0, P1=1, P2=1**. AC-M5D7M-001/002/003/005/006/008 remain unaffected by the redesign; AC-M5D7M-004 and AC-M5D7M-007 are not ready to pass until the pending-publication disable/destroy rule is explicit and implemented/tested. After that one textual closure, the revised lifecycle design is otherwise suitable for Astra’s approval gate; Luna does not change contract status.

## R3 lifecycle closure gate

- Review date: 2026-09-13
- Scope: verification of Astra’s two R2 lifecycle amendments only; no runtime, test, or contract edits by Luna

The amended contract now explicitly includes `InitializedPendingPublication` in router `OnDisable`/`OnDestroy` handling. The exact owner-bound abort enters `Failed`, disables both maps, closes the exact callback/action ownership once, and leaves no receipt or notification. This closes the pre-publication teardown window identified in P1-R2-001 while preserving the post-publication rule that adapter-only teardown does not dismantle an initialized router.

The amended legacy seam is also unambiguous: `InitializeForTests` requires router `Awake` to have entered, uses the common legacy ensure helper only in `Unreserved`, and rejects before `Awake` and in every prepared state. The focused test requirement now explicitly covers post-`Awake` allocation/register-once, pre-`Awake` rejection without allocation, and pending-publication disable/destroy closure.

No new lifecycle contradiction was found. The allocation-free/callback-free router `Awake`, either-side-of-`Awake` reservation/adoption before `Start`, inert prepared router `Start`, owner-only pending confirmation/abort, common legacy helper, and all four ordering permutations remain coherent and implementable within the allowlist. Legacy M5B5 behavior is preserved within the contract’s expressly allowed internal allocation deferral.

### R3 verdict

**PASS — P0=0, P1=0, P2=0.** The two R2 findings are resolved by the stated amendments. Recommend Astra advance the lifecycle-revised draft to Approved, subject to Terra’s implementation and Unity evidence; Luna does not change contract status.

## R4 remediation gate — central state matrix and coverage closure

- Review date: 2026-09-13
- Scope: contract-only re-review of Sol’s P1 remediation; no runtime, test, or contract edits by Luna
- Compared against: revised M5D7M, Verified M5B5 lifecycle/fault behavior, and the prior Luna post-review P1-001/P1-002 findings

### Matrix consistency

The central validator and explicit `Start` switch are compatible with M5B5 when the prepared and legacy namespaces remain distinct:

- Ownerless legacy startup/fault remains `Unreserved`; a legacy allocation or callback-registration failure is not converted into a prepared `Failed` row. This preserves M5B5’s existing fault path while preventing a failed prepared launch from falling through to defaults.
- `Reserved` has an owner and no actions; at `Start` it becomes terminal `Failed` without allocation or fallback. `Adopted` has the exact owner/action pair and disabled maps with no callbacks; router `Start` remains inert so adapter `Start` may still initialize it. This is coherent with the four active ordering permutations and exact-once transfer boundary.
- `InitializedPendingPublication` is the only pre-publication row with callbacks/actions/maps; its owner-only abort disables both maps and closes the exact wrapper. `ConfirmPreparedHubPublication` is the sole non-allocating publication transition.
- Historical `Initialized` is allowed to carry later M5B5 mode/map/fault state different from the UIOnly receipt snapshot. This does not rewrite the immutable receipt and does not grant M5D7M gameplay authority; it preserves the existing router’s later lifecycle behavior.
- Terminal prepared `Failed` requires a non-null owner, faulted state, no actions/callbacks/maps, closed ownership, and a defined non-None prepared fault stage. The separate ownerless legacy `Unreserved` fault row avoids conflating ordinary M5B5 failures with prepared-launch failures.

The validator’s entry/committed-exit coverage, explicit unknown-state switch, and resilient cleanup for a corrupt closed flag close the earlier state-field gap without weakening callback/action exact-once ownership. The matrix is implementable in the allowed private `InputRouter` seams and does not require an ABI, scene, Profile, or M5D7L change.

### Coverage sufficiency

The revised focused-test requirements directly address both prior P1s. They now require exact reservation/root/UTC/preparation/take/adopt/dispose sequence and values/identities; every stable-state field correlation; unknown state before `Awake` and after `Awake` before `Start`; legacy pre/post-`Awake` initialization timing; Reserved-at-Start fail-close; all four component/object permutations; callback and publication fault cleanup; prepublication teardown; competing-adapter isolation; and complete M5D7L outcome/notification overlap rows. The requirement to assert bounded completion remains important for Unity lifecycle tests.

These requirements are sufficient in the contract to close P1-001 and P1-002, provided implementation evidence actually demonstrates each row rather than relying only on aggregate full-suite counts. No P0 or P1 contradiction remains in the revised design.

### Residual P2 precision

### P2-R4-001 — Reserved-at-Start fault-stage naming

The matrix requires terminal `Failed` to carry a defined non-None prepared fault stage, while the `Reserved`-at-`Start` rule does not name the exact stage value. This is not a safety blocker because any defined prepared abandonment/closure stage can satisfy the closed matrix, but implementation/tests should select one stable value and assert it, including original-exception precedence if the close operation itself faults.

### R4 verdict

**PASS — P0=0, P1=0, residual P2=1.** Sol’s central matrix is consistent with M5B5 legacy behavior and exact-once ownership, and the expanded test requirements are sufficient to close the earlier P1s. Recommend Astra advance the remediation draft to Approved, conditional on Terra’s implementation and row-level Unity evidence; Luna does not change contract status.

## R5 deterministic evidence precision gate

- Review date: 2026-09-13
- Scope: contract-only review of the evidence-fixture amendments; no runtime, test, contract, or README edits by Luna
- Compared against: the R4 central state matrix, Verified M5D7L coordinator/ports, M5B5 boundaries, and the prior post-review P1-002

The revised fixture boundary does not weaken the earlier P1 closure. Requiring the adapter outcome rows to call the real M5D7L coordinator with scripted lower profile/actions ports preserves the actual coordinator validation, source/revision/binding/preservation/save proof, and token ownership lifecycle while making all nine legal outcomes deterministic. It correctly forbids handwritten `PreparedProfileLaunchV1` fabrication. The isolated-filesystem smoke rows remain a separate, honest check of default/current/previous/decode-repair integration; avoiding OS permissions/locks for failure synthesis prevents flaky or environment-dependent evidence.

The valid `HubEntryAllowed=false` row is correctly classified N/A because the sealed M5D7L token API always returns `true`. Retaining the adapter defensive branch and covering null/throw/reflection-malformed tokens preserves fail-closed behavior without inventing an impossible legal token. Similarly, narrowing “reused action” to same-router duplicate/retake/discarded-candidate cases matches the actual absence of a cross-router action registry and makes no unsupported global-ownership claim.

The private close-attempt diagnostics are non-authoritative: each is write-only implementation evidence assigned immediately before the corresponding best-effort operation, with no getter, delegate, branch, reset, or service-locator effect. Reflection-read access therefore proves exact attempted unsubscribe/dispose identity/counts without adding runtime behavior or authority. The active+enabled+pre-`Start` cohort definition is an intentional lifecycle fact boundary and does not smuggle in scene, parent, topology, or cross-router identity.

The expanded focused-test requirement now directly demands the rows needed to close the earlier implementation/evidence gap: literal reservation/root/UTC/preparation/take/adopt/dispose values and identities; stable-state and unknown-state reflection rejection; all four permutations; all M5D7L outcomes; notification overlap priority; callback/publication faults; pending teardown; competing same-router adapters; and bounded completion. These are sufficient in the contract, subject to actual row-level execution rather than aggregate counts.

### R5 verdict

**PASS — P0=0, P1=0, P2=0.** The fixture architecture is implementable, preserves real M5D7L semantics, and does not weaken P1-002 or add forbidden behavior authority. Recommend Astra advance this precision revision to Approved, conditional on Terra’s complete focused/direct/full evidence; Luna does not change contract status.
