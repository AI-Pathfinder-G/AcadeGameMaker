# M5B1 — Movement command gate prerequisite

- Status: Verified
- Verified: 2026-09-08, Luna independent PASS and Astra integration acceptance; [evidence](../../verification/2026-09-08-vd07-m5b1-input-gate-evidence.md)
- Approved by: Astra, 2026-09-08, after Terra impact PASS and Luna independent pre-gate PASS (P0/P1/P2 = 0)
- Contract/integration: Astra
- Implementation: Terra
- Independent review/tests: Luna
- Parent specs: Approved VD-01 and VD-07; ADR-0024, ADR-0027

## Boundary

Implement the real Movement command consumer's internal input-lock sink before binding M5A to the encounter. This is not an InputRouter, global InputMode owner, Run failure owner or automatic boss-death integration. No scene/prefab or action-map changes. Transfer/Combat queues remain a subsequent contract. Do not infer run-active, profile choice, KillPlane or LethalCrush facts.

REQ-UX-004/008 and REQ-MOV-001/002/007/008: add an instance-bound internal gate owner registration and exact-next-movement-tick apply method to PlayerMovementController. These are consumption endpoints for a future InputMode owner, not a second mode state machine. RegisterInputGateOwner(object) rejects null and different owners; same reference is idempotent. ApplyInputGate(object, SimulationTick, bool locked) rejects an unregistered/wrong owner or any tick other than NextExpectedTick before mutation. It returns the number of dropped queued MovementCommand values.

On a transition into locked: clear the entire tick-indexed movement command queue (including future commands), reset existing motor lifecycle buffers/dash through its existing ResetForLifecycle, and latch the existing _inputLocked flag. Do not reset position, tick, modifier, health or external movement directive queues/receipts. Subsequent fixed ticks continue existing locked physics, not GameObject disable or teleport. While locked, SubmitCommand rejects all new commands without queuing them. Unlock by the same owner at the exact next tick clears any command queue and restores acceptance; it never replays pre-lock input. Repeating the same gate state at a valid tick returns zero and performs no motor reset. A lock/unlock pulse before any fixed step must still clear jump/dash motor buffers through lock-time reset. Test-only lock setters must not bypass a registered production owner.

No preflight token or cross-owner atomic transaction is claimed in this unit. Future multi-consumer integration must preflight all consumers before applying any mutation; do not connect the partial sink to boss death here.

## Acceptance criteria

- AC-M5B1-001: exact and distant-future queued move/jump/dash commands disappear on lock; locked submissions are rejected and do not reappear after unlock.
- AC-M5B1-002: null/different/unregistered owners and past/future apply ticks are mutation-free failures; same-owner registration and repeated same-state application are idempotent.
- AC-M5B1-003: locked fixed ticks continue monotonically using InputLocked motor physics, preserve position continuity and modifier ownership, and do not disable the player.
- AC-M5B1-004: unlock accepts a fresh command once; repeated unlock does not clear that fresh queued command. Old commands never fire at their former distant-future tick.
- AC-M5B1-005: an existing buffered jump and active dash cannot resume after lock/unlock, including a pulse without an intervening fixed tick; no external directive queue is cleared by the gate.
- AC-M5B1-006: 30/60/144 render grouping of the same fixed-tick script yields identical motion/action traces; full EditMode and PlayMode suites remain green.

## Allowlist and evidence

Runtime: PlayerMovementController.cs only. No motor algorithm changes or friend/asmdef expansion. Tests: new Movement PlayMode input-gate fixture and meta under the existing Movement test assembly. Documentation: this contract, pre-gate/evidence, docs/README.md. Every source change cites REQ IDs; tests/results cite AC IDs.

Traceability: AC001/002/004 cover REQ-UX-004/008; AC003/006 cover REQ-MOV-001/002 and REQ-UX-004; AC005 covers REQ-MOV-007/008 and REQ-UX-008. These are bounded subcriteria, not completion of the full parent UX acceptance criteria.

Rollback boundary: remove only this unit's controller gate additions and new test fixture if integration is rejected; preserve all earlier dirty changes and artifacts. No automatic rollback, git reset, scene revert, project setting change or deletion is authorized.

GPT participation: Terra replaces former Kimi/MiniMax implementation lanes; Luna replaces GLM adversarial analysis and independently writes/verifies tests; Astra integrates. No Ollama calls or new automations.
