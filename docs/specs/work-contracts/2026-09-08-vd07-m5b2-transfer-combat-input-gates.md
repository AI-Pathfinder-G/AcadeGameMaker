# M5B2 — Transfer and Ordan Combat input gate sinks

- Status: Verified
- Verified: 2026-09-08, Luna independent AC001–007 PASS and Astra integration acceptance; [evidence](../../verification/2026-09-08-vd07-m5b2-input-gate-evidence.md)
- Approved by: Astra, 2026-09-08 after Terra implementability PASS and Luna pre-gate PASS; bootstrap clarification below resolves the nonblocking P2.
- Authority/integration: Astra
- Implementation: Terra
- Independent review/tests: Luna
- Parent authority: Approved VD-02, VD-03, VD-07; ADR-0027; verified M4B3C and M5B1

## Contract

REQ-UX-004/008, REQ-WT-005/006 and REQ-COM-001/004: extend the real TransferSimulationDriver and OrdanBossCombatSimulationDriver input consumers with internal `RegisterInputGateOwner(object)` and `ApplyInputGate(object, SimulationTick, bool locked)` returning the count of queued envelopes stripped of any Camera/Aim/Press. These are sinks for a future single InputMode owner, not independent global mode owners. No InputRouter, automatic boss-death coordinator, Run/failure/profile facts, action maps or authored asset changes are claimed.

Registration rejects null/different owners and is idempotent for the same reference. Apply requires exact registered identity, initialized/prepared driver, nonnegative exact player NextExpectedTick and an unconsumed own phase: Transfer's latest completed publication must precede that tick, and Combat's active combat/attack sessions must both expect that tick. Reject before mutation, including a call after that consumer ran but before Movement advanced. Do not initialize or run a phase as a gate side effect.

Bootstrap clarification: an initialized Transfer with no completed publication is eligible before its first phase. A prepared Combat whose provisional active sessions both expect the player tick is likewise eligible. Missing initialization/preparation is never inferred or supplied by Apply.

Lock transitions stage a complete replacement input dictionary before any queue/latch change. For every current/future envelope preserve its exact key/tick and all system fields, but clear Camera/Aim/Press. Transfer preserves ordered Removals, ExposureEnds and Lifecycle; Combat preserves ordered ExternalDamageRequests. Count envelopes that originally contained at least one interactive field, not individual fields. Preserve empty envelopes and future keys. A malformed queued value or copy failure leaves the old dictionary, gate state, publications, sessions and system lanes unchanged. Only after successful staging swap the dictionary reference and latch locked. No cross-consumer atomicity is claimed.

Validate the original complete envelope through its existing constructor before stripping fields, both during staging and registered Submit; stripping must not hide invalid interactive tick/coupling data. Dictionary keys must equal each envelope Tick.

While locked, Submit canonicalizes a valid mixed envelope into system-only input rather than rejecting and losing its authoritative maintenance or damage. While a gate owner is registered, Submit validates and defensively copies the entire input before storage even when unlocked; default/malformed values fail before queue mutation. Copy collections into read-only defensive lists in the internal input DTO constructors so callers cannot edit pending maintenance/damage through arrays. Preserve the existing unregistered consumer's consume-on-attempt behavior; do not broaden validation of domain values beyond existing constructors.

Unlock preserves the already stripped system envelopes and keys, changes only the latch, returns 0 and never regenerates interactive fields. Repeated same-state Apply at a valid phase returns 0/no rewrite; fresh input queued after unlock must survive repeated unlock. Existing duplicate/late Submit rejection remains. No simulation pause, invulnerability, TransferClear event, selection reset or health change is introduced by the gate itself. Ordinary locked ticks still run system maintenance/damage through their existing owners.

M4B3C terminal removal reservation, four-ID order, lifecycle-first meaning, durable-removal evidence and all delivery/hostile/pull lanes remain untouched. Terminal discard still rejects a future input key even if empty and rejects retained external damage. Gate stripping before terminal preflight is an explicit input-consumer operation, not permission for the teardown to discard nonempty inputs. No gate auto-registration or automatic invocation in the prefab/scene is allowed here.

## Acceptance criteria

- AC-M5B2-001 (REQ-UX-004/008): both consumers strip queued exact/future Camera/Aim/Press, preserve keys, and cannot replay old actions after unlock; fresh post-unlock input survives repeated unlock.
- AC-M5B2-002 (REQ-WT-005/006): real Transfer phases retain ordered removal/exposure/lifecycle effects, avoid aim capture and transfer/recall while locked, and preserve exact boss terminal cleanup and lifecycle supersession.
- AC-M5B2-003 (REQ-COM-001/004): real Ordan Combat phases produce no basic attack from stripped inputs while external damage remains processed and delivery queues retain their original semantics.
- AC-M5B2-004 (REQ-UX-004/008): owner/null/unprepared/wrong-tick/already-consumed-phase errors preserve queue/latch/publications; same-owner registration and same-state apply are idempotent.
- AC-M5B2-005 (REQ-WT-005, REQ-COM-004): default/malformed staged values cause all-or-nothing failure; caller collection mutation cannot change stored system payloads; mixed locked submissions preserve system payloads; invalid/duplicate/late submissions do not corrupt valid queued work.
- AC-M5B2-006 (REQ-COM-004): terminal preflight still rejects retained nonempty external damage and future keys; exact t+1 terminal maintenance still succeeds through existing receipt/teardown contracts.
- AC-M5B2-007 (REQ-UX-008, REQ-WT-006): same scripted fixed ticks grouped at 30/60/144 yield exact outcome traces; focused tests and complete EditMode/PlayMode suites pass. No full parent UI/device acceptance claim.

## Allowlist / rollback / staffing

Runtime only the two named driver .cs files including their internal DTOs. New focused fixtures + .meta under existing TransferUnity and CombatUnity PlayMode test assemblies; existing terminal PlayMode fixture may add gated terminal scenarios without weakening existing tests. No other runtime, asmdef/friend, authoring, scenes, settings or media changes. Documentation: this contract, matching evidence, docs/README.md.

Rollback is inverse application of this unit's hunks only, preserving all pre-existing dirty changes. No reset, cleanup, deletion, commit or push is authorized. Terra implements (former Kimi/MiniMax scope); Luna owns adversarial analysis and independent tests (former GLM scope); Astra approves/integrates. No Ollama/Orca calls, probes or automations.
