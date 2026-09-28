---
status: PASS (amendment re-review)
---

# VD-03 M4B3C 계약 사전 게이트 증적

- Date: 2026-09-08
- Reviewer: Luna (`gpt-5.6-luna`), independent pre-gate
- Contract: `docs/specs/work-contracts/2026-09-08-vd03-combat-m4b3c-ordan-terminal-teardown.md`
- Initial decision (before amendment): **CONDITIONAL HOLD** — retained below as review history.
- Current decision (after amendment re-review): **PASS** for the contract pre-gate; implementation and Unity execution remain separate downstream gates.
- Scope: contract/runtime/document read review only. No runtime, contract, prefab, scene or project-setting changes were made by this review.

## Initial gate summary — 2026-09-08 first review

| Priority | Count | Gate result |
|---|---:|---|
| P0 | 0 | No evidence of a root/player destruction or cross-system destructive path in the reviewed contract. |
| P1 | 3 | Must be corrected before `Approved` and Terra implementation. |
| P2 | 2 | Required clarification/documentation before final acceptance; not independently sufficient to block a safe implementation once P1 is closed. |

At the initial review point, the then-current draft had closed the polarity, lifecycle-reason, phase-order, one-shot identity, and death-t snapshot ambiguities, but remained on hold because the malformed-input seam could still occur after a possible committed `-200`, the exact empty Combat fingerprint omitted one runtime identity bit, and the normative system contract had not yet recorded the new `-195` conditional seam.

## Evidence read

- Contract: lines 18–47 define death-t publication, `+110` before teardown, `-200 → -195`, downstream suppression, player/root preservation, and frozen death-t Combat evidence.
- Contract: lines 51–68 distinguish request IDs from effect receipt, require normal registration absence plus durable removed-ID presence, and define lifecycle supersession.
- Contract: lines 89–98 define the exact empty `t+1` Combat closure and mutation-free rejection.
- Contract: lines 100–117 define receipt identity, one-shot ownership, and failure semantics.
- Runtime: `TransferSimulationDriver.cs:147–152, 260–288` exposes the exact terminal merge and publishes `LatestCompletedInputRemovalIds` after `TransferSession.Process`; that list is not itself an effect receipt.
- Runtime: `TransferSession.cs:42–49` gives lifecycle precedence over removals and removes registrations only on the normal path.
- Runtime: `OrdanBossCombatSimulationDriver.cs:194–200, 232–243, 356–366` preflights incoming delivery and includes `HostileAppendCommitted` in delivery equality.
- Normative system contract: `docs/specs/vertical-demo/SYSTEM-CONTRACTS.md:51–61` records M4B3B3 but has no M4B3C `-195` seam yet.

No Unity test result is claimed. Existing runtime lines above are read evidence, not newly executed verification.

## Acceptance-criterion review

| AC | Review result | Basis |
|---|---|---|
| AC-M4B3C-001 | PASS in contract shape | Death triplet, four-ID reservation, terminal suppression and final handoff-before-teardown are explicit (contract 28–33). Runtime integration remains untested. |
| AC-M4B3C-002 | PASS in contract shape | Request/effect distinction and normal `registration absent` / `durable removed-ID present` polarity are explicit (contract 51–64). Runtime integration remains untested. |
| AC-M4B3C-003 | **BLOCKED — P1-001** | Lifecycle-first and no-rollback wording is improved (contract 66, 130), but malformed/mixed input can still be discovered after a committed `-200`; see below. |
| AC-M4B3C-004 | **BLOCKED — P1-002** | Empty payload/hostile-count shape is explicit, but the runtime batch fingerprint also contains `HostileAppendCommitted`; the contract must require `false`. |
| AC-M4B3C-005 | PASS in contract shape | Preterminal/bootstrap no-op, `-200 → -195`, t+1 downstream suppression and instance-local one-shot are explicit (contract 40, 45, 115). |
| AC-M4B3C-006 | PASS with P2 scope correction | Exact disable set and root/player preservation are explicit (contract 70–87); static guard scope needs narrowing to new M4B3C code/diff. |
| AC-M4B3C-007 | PASS in contract shape | Two post-teardown Movement/Transfer ticks, player continuity, death-t snapshot freeze and no future Combat claim are explicit (contract 47, 134). Runtime integration remains untested. |
| AC-M4B3C-008 | **BLOCKED by P1-001** | Failure cases are listed, but the contract must separate pre-`-200` invalid-batch rejection from post-commit receipt mismatch. |
| AC-M4B3C-009 | **BLOCKED — P1-003 / P2-002** | The allowlist names `SYSTEM-CONTRACTS.md` and `docs/README.md`, but the normative M4B3C seam and index entry are not yet present. |

## Failure-mode closure

| Failure mode | Result | Evidence / remaining action |
|---|---|---|
| FM-001 scheduler terminal causes a next-phase throw | Closed by contract | `-195` is conditional on terminal observation and suppresses t+1 boss downstream phases (contract 38–45). |
| FM-002 teardown precedes final handoff | Closed by contract | `+110` must publish on death tick before t+1 teardown (contract 33, 38). |
| FM-003 cleanup happens after boss-only shutdown | Closed by contract | Cleanup is effect-proven at t+1 `-200`, then teardown at `-195` (contract 35–40). |
| FM-004 request list mistaken for effect | Closed by contract | Dedicated immutable receipt is required; `LatestCompletedInputRemovalIds` is explicitly insufficient (contract 49–64; runtime line 46). |
| FM-005 lifecycle removal falsely reported as permanent removal | Closed by contract | `LifecycleSuperseded` has an exact lifecycle reason and no permanent-removal claim (contract 66, 115). |
| FM-006 mixed exposure-end/removal input leaves unsafe queue state | **Open — P1-001** | `TransferSimulationDriver.MergeExactBossTerminalRemovals` delegates to the existing merge lane (runtime 150–152); `TransferSession.Process` gives lifecycle precedence and processes exposure/removal inputs (runtime 42–49). Require malformed/mixed/stale preflight before the valid `-200` commit, or define a safe post-commit quiesce path. |
| FM-007 empty Combat delivery accidentally advances M1 | Closed by contract | Exact empty delivery/input only; no M1 recompute and non-empty rejection are explicit (contract 89–98). |
| FM-008 empty batch identity is under-specified | **Open — P1-002** | Runtime `OrdanBossDeliveryBatch` stores and compares `HostileAppendCommitted` even when hostile count is zero (runtime 356–366). Require exact empty fingerprint `HostileAppendCommitted=false`. |
| FM-009 root/player or shared registry is disabled | Closed by contract | Root, Systems, player, Transfer, Movement and registries are explicitly preserved (contract 79–87). |
| FM-010 duplicate/cross-graph teardown consumes another instance | Closed by contract | `(graph,t)` owner key is instance-local; static/global registry is forbidden (contract 100–117). |

## Required corrections

### P1-001 — preflight malformed or mixed Transfer input before `-200`

The contract currently says malformed, mixed, stale or wrongly ordered input produces no receipt/no teardown (lines 68, 130), while also correctly disclaiming rollback of an already started/completed `-200` (lines 130, 113). The runtime evidence shows why these must be separated: terminal merge is entered through `MergeExactBossTerminalRemovals` (`TransferSimulationDriver.cs:150–152`), while `TransferSession.Process` gives lifecycle precedence and otherwise processes exposure ends before removals (`TransferSession.cs:42–49`). A receipt mismatch discovered only at `-195` can therefore leave a committed Transfer attempt and a terminal scheduler without a teardown commit; “preserve prior Transfer publication” cannot mean rollback in that state.

Required contract correction: preflight all queue-shape defects, including mixed `TransferTargetExposureEnded`, foreign IDs, wrong tick/order, duplicate reservation, and lifecycle precedence, before the valid `-200` commit. State explicitly that the no-mutation/no-receipt guarantee applies to that pre-commit rejection; after a valid `-200` commit, any receipt-only mismatch preserves the committed Transfer result, makes no rollback claim, and follows an explicit fail-stop/quiesce outcome.

### P1-002 — include `HostileAppendCommitted=false` in the empty Combat fingerprint

Contract line 93 requires null payload and hostile count zero, but the runtime constructor permits an empty hostile list with either value of `HostileAppendCommitted`, and equality includes that flag (`OrdanBossCombatSimulationDriver.cs:356–366`). The exact empty candidate and receipt must require `HostileAppendCommitted=false`; otherwise a previously host-append-committed batch with zero requests can be misidentified as the teardown-owned empty delivery.

### P1-003 — record M4B3C in the normative system contract before approval

`SYSTEM-CONTRACTS.md:51–61` still describes the M4B3B3 order ending at `+110` and contains no conditional `-200 → -195` terminal seam, t+1 downstream suppression, or player/root preservation rule. Add the bounded M4B3C normative paragraph before changing this contract status to `Approved`; the work contract already lists this file in its evidence allowlist (lines 165–173).

### P2-001 — narrow the static destructive-action guard scope

AC-M4B3C-006 asks for a `SetActive`/Destroy/scene-load static guard (contract line 133), but the existing authoring builder legitimately contains `DestroyImmediate` and root `SetActive` calls (`OrdanBossEncounterAuthoringBuilder.cs:33, 42, 74`) in its pre-existing asset-build path. Scope the guard to the new teardown runtime and M4B3C diff/allowlisted mutation sites, or it will report unrelated authoring behavior as a false defect.

### P2-002 — add navigation/index entries

The contract and this evidence file should be indexed in `docs/README.md` before final integration so the new gate and normative seam are discoverable. This is documentation completeness, not a runtime safety defect.

## Amendment re-review — current decision

The amended contract was reread once after Sol's completion notice. Contract SHA256 was verified as `51FF7E609A298CD68B9B0F4D383DC320EBB4CE8AD8EDF5174BF5512ADB5D48D5`, matching the supplied handoff hash. The companion `SYSTEM-CONTRACTS.md` now contains `Pending M4B3C terminal seam — Review only` (lines 63–69), explicitly non-authoritative while the work contract remains `Review` and conditional on Astra moving it to `Approved`.

| Prior finding | Current result | Evidence |
|---|---|---|
| P1-001: malformed/mixed/stale Transfer envelope could be discovered after `-200` | **CLOSED** | Contract lines 70–80 require terminal-lane preflight before `ConsumeOrEmpty`/`TransferSession.Process`, reject mixed exposure ends, foreign/duplicate/wrong-order IDs and lifecycle mismatch without consuming the input, then explicitly define valid-commit receipt mismatch as non-rollback fail-stop. Generic Transfer consume-on-attempt semantics remain unchanged. |
| P1-002: exact empty Combat fingerprint omitted `HostileAppendCommitted` | **CLOSED** | Contract lines 103–109 require the four-field fingerprint including `HostileAppendCommitted=false`; AC-M4B3C-004 repeats it. |
| P1-003: normative-document seam missing | **CLOSED for Review stage** | `SYSTEM-CONTRACTS.md:63–69` records the bounded pending seam, its `Review only` status, and the Astra-Approved condition without granting implementation authority prematurely. |
| P2-001: static guard could flag existing builder operations | **CLOSED** | AC-M4B3C-006 now scopes the guard to the new teardown runtime and M4B3C runtime hunks; existing authoring rebuild code is excluded and validator coverage is separate (contract line 145). |
| P2-002: navigation/index missing | **CLOSED** | `docs/README.md:152–153` links the M4B3C contract and this pre-gate evidence. |

Current AC disposition is P0=0, P1=0, P2=0. AC-M4B3C-001/002/005/007 were contract-shape PASS in the first review; AC-M4B3C-003/004/006/008/009 are now closed by the amendments above. This is a documentation/contract gate PASS only: no runtime implementation, Unity run, or test result is claimed.

## Decision and next gate

Initial HOLD is preserved in the first-review sections above. Current decision is **PASS** for the amended contract pre-gate. Astra still owns the transition from `Review` to `Approved`; only after that may Terra implement within the allowlist. Luna will independently verify AC-tagged runtime/build evidence after implementation. This review performed no Unity run and records no fabricated test result.
