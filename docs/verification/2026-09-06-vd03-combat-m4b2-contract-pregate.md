# VD-03 M4B2 오르단 scripted Transfer 노출 계약 사전 게이트

- Date: 2026-09-06
- Contract: `docs/specs/work-contracts/2026-09-06-vd03-combat-m4b2-ordan-transfer-exposure.md`
- Reviewer: Luna
- Decision owner: Sol
- Result: **PASS — `P0=0`, `P1=0`, `P2=0` after cleanup**

## Gate result

Luna held the first draft until the contract distinguished ordinary temporary exposure expiry from terminal permanent removal, clarified that permanence belongs to the internal input command rather than the public `TransferCleared(TargetRemoved)` reason, required hidden sinks to remain unavailable while accepting `Clear`, and defined the exact same-ID temporary/permanent result.

Sol incorporated those corrections, plus Terra's implementation-impact findings:

- `-180(t)` consumes only M4A's exact forecast for `t+1` and merges only future Transfer input;
- `Awake` validates the graph, registers a private owner-token lane and hides/disables all payloads before the first `-200`;
- `TransferTargetExposureEnded` clears an active relation while retaining registration; `TransferTargetRemoved` remains permanent;
- exact defeated handoffs permanently remove all three payload IDs on the following `-200` and terminate the scheduler;
- both future merge paths preserve every queued input field independent of call order;
- no registry rewrite/re-freeze, M4A reconstruction, same-tick rollback, public ABI, production geometry or new physics/time/RNG authority is allowed.

Luna's final pre-gate found no P0/P1. Its three P2 items were resolved before approval: active/inactive same-ID wording is exact, terminal re-advance rejection is required by tests, and SYSTEM-CONTRACTS metadata records the amendment.

This PASS authorizes Terra implementation only within the Approved contract allowlist. It is not implementation or Unity execution evidence.
