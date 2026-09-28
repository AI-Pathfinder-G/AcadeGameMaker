# VD-03 M3B2A Contract Pre-Gate — Luna PASS

- Date: 2026-08-29
- Contract: [VD-03 M3B2A](../specs/work-contracts/2026-08-29-vd03-combat-m3b2a-regular-enemy-behavior-core.md)
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance scope: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`
- Independent verifier: Luna
- Final result: PASS — no remaining P0/P1

## Review history

Luna's first pass found six P1 ambiguities: phase-boundary ownership, premature M3B2B execution-order commitment, incomplete reset snapshot fields, incomplete M3A snapshot preflight, missing surveyor ordinal overflow, and dead-latch/external-candidate ownership. Sol resolved all six.

Luna's second pass found two remaining P1 gaps: transfer revision `0` was not explicitly invalid and first-tick reset was not explicitly forbidden. Sol aligned both with the verified M3A contract and added their required mutation cases.

The final pass confirmed:

- post-transition first/last/following-tick semantics for every 18/12/40-or-60 and 48/30/1/60 phase;
- engine-free recovery intent at `t` for `t+1`, with conversion and execution order deferred to a separate M3B2B contract;
- exact reset publication fields, no first-tick reset, no resurrection without reset;
- default/stale/malformed M3A snapshot rejection, including revision `-1|1..int.MaxValue` and Heavy-positive-revision consistency;
- atomic checked deadline, Q1000 long-delta and both ordinal overflow rules;
- clean authority boundaries across M1, M3A, M3B1 and VD-02;
- explicit three-lane Kimi/GLM/MiniMax utilization outcomes;
- no Unity, physics, public ABI, scene, prefab, asset or project-setting authority in M3B2A.

## Approval

Sol accepts Luna's final PASS and changes the M3B2A contract from `Review` to `Approved`. Terra may implement only the allowed files and behavior in the frozen contract. M3B2A remains partial substrate and does not close full `AC-COM-001` or `AC-COM-003` until a later Approved Unity binding is verified.
