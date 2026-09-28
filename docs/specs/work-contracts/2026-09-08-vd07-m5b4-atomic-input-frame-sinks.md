# M5B4 — Atomic input-frame consumer candidates

- Status: Verified
- Integrated by: Astra, 2026-09-08 after Luna independent AC-M5B4-001..005 PASS; full EditMode 440/440 and PlayMode 474/474.
- Approved by: Astra, 2026-09-08 after Terra implementability PASS and Luna independent pre-gate PASS (P0/P1/P2 = 0).
- Authority: Astra; implementation: Terra; independent verification: Luna
- Parents: Approved VD-07 REQ-UX-004/006/008, VD-01 REQ-MOV-001/002/007/008, VD-02 REQ-WT-005/006, VD-03 REQ-COM-001/004; ADR-0027
- Design basis: bounded Sol proposal; M5B1/M5B2 remain authoritative for existing entry points.

## Scope

Provide the three existing Movement, Transfer and Ordan Combat consumers with mutation-free staging for a combined gate change and optional exact-current-tick interactive input. This is the prerequisite for one InputRouter transaction. No device callback, camera, mode owner, scene, reward, Run or UI implementation belongs to this unit.

## Contract

Each consumer exposes internal opaque instance/owner-bound `PrepareInputFrame(owner, tick, locked, optionalInput)`, `ValidateInputFrameCandidate(owner, candidate)` and `CommitInputFrame(owner, candidate)`. Existing input DTOs remain unchanged; optional input is nullable MovementCommand, TransferSimulationInput or OrdanBossCombatInput respectively. A candidate records the exact phase tick, private input-state generation, precomputed next generation, staged full input queue, resulting latch, dropped/stripped count and optional Movement lifecycle-reset flag. It has no caller-mutable collection. Preparing and validating never initialize an owner/session, consume input, change motor state or publish a phase. Candidates require an already registered gate owner and initialized/prepared unconsumed exact phase, just as M5B1/M5B2 require.

The coordinator protocol is prepare all three, validate all three immediately, then synchronously commit all three on the main thread with no callbacks or other producers between validations and commits. Commit checks candidate identity, phase, generation and single-use before its own first assignment, then performs only the prebuilt reference/latch/generation assignments and prevalidated Movement lifecycle reset. No collection iteration, allocation, engine query, external callback or new domain decision occurs after the first assignment. Ordinary contract failures are all eliminated before the first consumer commit; process corruption and arbitrary out-of-band concurrent mutation are not rollback promises. A failed preparation or candidate validation preserves every consumer. Reusing, cross-wiring or submitting after preparation invalidates the candidate before mutation.

Every successful mutation of the relevant input queue or input-gate latch advances a private checked generation. Reserve the next generation before mutation so overflow fails unchanged. This includes legacy Submit, gate application when it changes state, consumption/removal, and Transfer maintenance merges. Same-state no-op gates need not invalidate. Phase advancement is separately checked; no-input advancement still makes a candidate stale. External movement directives and Combat delivery lanes are not copied or changed by these candidates and retain their existing owners and validation. No new test setter bypasses a registered owner.

Movement stages a full queue copy, verifies each key equals command.Tick and rejects late keys. Either latch transition clears old commands, matching M5B1. Entry into lock stages the existing lifecycle reset; unlock does not reset motor state. Same-state unlocked retains future commands. Optional command is legal only unlocked and must have exact requested tick; a still-occupied same tick rejects, never overwrites. Same-state locked cannot accept a command. Motor position/velocity/tick/modifier and external directive lanes remain untouched by preparation; commit changes only the existing lock lifecycle buffers allowed in M5B1.

Transfer/Combat validate and defensively rebuild every queued envelope with the existing DTO constructor and exact dictionary key. Entering lock strips only Camera/Aim/Press from all queued keys, preserving keys (including empty/future keys), ordered maintenance/exposure/lifecycle or external damage. Same-state locked remains canonical through the existing protected ingress. Unlock preserves every system field/key. Optional incoming DTO must be exact tick and interactive-only: Transfer has empty removals/exposure ends and null lifecycle; Combat has empty external damage. Supplying system fields through this new interactive lane rejects rather than merging authority. Locked optional input rejects even if empty.

When unlocked and incoming input is present, consumer-owned composition can add Camera/Aim/Press to a same-tick system-only envelope while preserving that envelope's ordered system fields. Any existing Camera/Aim/Press conflicts and rejects; there is no replay overwrite. Existing lifecycle data retains its normal phase priority; this staging does not evaluate or weaken lifecycle/terminal semantics. Null incoming input creates no new key. An explicitly supplied empty valid envelope may preserve/create its exact key, and must never bypass terminal future-key checks.

Legacy Submit/ApplyInputGate and their verified exception/no-op/consume-on-attempt contracts remain intact, except private generation bookkeeping. Do not turn legacy duplicate Submit into merge. No relaxing M4B3C exact empty teardown or future-key rejection.

## Acceptance and traceability

- AC-M5B4-001 (REQ-UX-004/006/008): all three prepare/validate paths are mutation-free; absent/wrong owner, wrong tick, uninitialized/unprepared/current-completed phase, default/malformed input, queue-key mismatch, locked optional input and noninteractive authority injection reject unchanged.
- AC-M5B4-002 (REQ-UX-004/008, REQ-MOV-001/002/007/008): lock clears queued Movement and resets only allowed lifecycle buffers at commit; unlock-plus-fresh exact command succeeds, repeat unlocked preserves future commands and duplicate rejects. External directives remain intact.
- AC-M5B4-003 (REQ-WT-005/006, REQ-COM-001/004): exact system-only merge retains all ordered maintenance/exposure/lifecycle/damage; lock strips only interactive fields; explicit empty/future keys survive. Invalid tail entry leaves earlier entries unchanged.
- AC-M5B4-004 (REQ-UX-006/008): foreign instance/owner, consumed candidate, intervening Submit/gate/maintenance merge/input consumption and phase advancement invalidate a candidate before mutation; generation exhaustion rejects before mutation. Commit has no fallible work after its first state assignment.
- AC-M5B4-005 (all parents): a coordinated three-consumer fixture injects rejection at each prepare/validate position and proves no partial queue/latch/motor mutation; successful trio publishes one coherent frame. Existing M5B1/M5B2 and terminal maintenance/teardown tests plus full EditMode/PlayMode pass.

## Allowlist and ownership

Only PlayerMovementController.cs, TransferSimulationDriver.cs and OrdanBossCombatSimulationDriver.cs for runtime (candidate types may be nested); new/extended tests and metas in the existing Movement, TransferUnity and CombatUnity PlayMode test assemblies; this contract, evidence and docs/README. No Core DTO, assembly friendship, package/settings, scenes/prefabs, media or other runtime edits. Terra owns runtime; Luna owns independent tests and reviews. Root runs sequential Unity verification with writers frozen. Inverse own hunks only for rollback; preserve unrelated dirty work. No Ollama/Orca/automation/commit/push.
