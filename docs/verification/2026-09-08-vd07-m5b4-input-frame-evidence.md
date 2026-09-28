# M5B4 atomic input-frame evidence

- Status: Verified; Luna independent PASS and Astra integration approval, 2026-09-08
- Contract: [M5B4](../specs/work-contracts/2026-09-08-vd07-m5b4-atomic-input-frame-sinks.md)
- Requirements: REQ-UX-004/006/008, REQ-MOV-001/002/007/008, REQ-WT-005/006, REQ-COM-001/004

## Review ownership

Sol supplied bounded combined-frame design. Terra passed implementability; Luna independently passed pre-gate (P0/P1/P2 = 0). Astra approved the contract. Terra owns the three runtime sinks; Luna owns independent boundary fixtures; Astra reviews integration and runs sequential Unity after source freeze.

The coordinated fixture proves only the specified prepare/validate/commit protocol. It does not implement or accept an actual InputRouter, InputMode, camera, Run or scene coordinator.

## Pre-execution corrections

Astra rejected the first runtime draft before Unity execution for six contract violations: dictionary value replacement during enumeration, checked generation increments after mutation, incomplete own-phase validation, no-op staged swaps retaining an old generation, mutable candidate queue exposure, and potential Movement initialization from a supposedly read-only candidate path. Terra corrected these under the same Approved scope.

A local C# compile probe showed that an enclosing type cannot read a nested type's private fields; the subsequent candidate design uses a private consumer issuer and nested guarded installation, not public mutable queues. Further review required direct nested installation to validate the actual consumer issuer/owner/phase (not merely a caller-supplied matching fake token), and required Combat terminal discard to reserve its generation before removing either delivery or input. These were implementation review findings, not waived acceptance failures or gameplay changes.

## Execution

Initial focused execution passed 12/12. Astra added 33 direct boundary cases: every trio rejection position, same-generation parallel candidates, all-consumer overflow and invalid-tail preservation, system-only envelope composition, terminal delivery/input preservation, wrong tick/owner/initialization/locked input, own-phase advancement without input-generation changes, and default/system-authority ingress. Expanded focused runs passed 29/29 and finally 45/45. No runtime rule was weakened to satisfy these tests.

| Final result | Passed/total | SHA-256 |
|---|---|---|
| TestResults-Unity-PlayMode-20260908-182535.xml (focused) | 45/45 | F847007D17B03D66834F44D60591C06C522D977B17E67F7F123E70AF8F8C3FC2 |
| TestResults-Unity-EditMode-20260908-182614.xml (full) | 440/440 | E0B362925829C343B716DF175A80DF7CA429CA98ABC081E98B0365F9312145A4 |
| TestResults-Unity-PlayMode-20260908-182745.xml (full) | 474/474 | 07CB96495789B1CAD9565E292F5038AFF6F621266FCF33BCB3820FDFE4101EAB |

All final runs: failed 0, skipped 0, Unity exit 0. The full suites contain 914 cases; focused 45 are a subset. M5B4 adds 39 PlayMode cases to the prior M5B3 baseline of 875. Unity runs were sequential with all Assets writers frozen. Luna independently verified the focused/full results and hashes and reviewed the source: AC-M5B4-001..005 PASS, no remaining blocker. Astra accepts this bounded unit. Actual InputRouter and downstream coordination receipts remain M5B5 Draft; do not claim a failed real input phase currently prevents every downstream phase from advancing.
