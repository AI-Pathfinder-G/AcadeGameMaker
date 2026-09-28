# VD-03 M3D2 Contract Pre-gate Evidence

- Date: 2026-08-31
- Contract: `docs/specs/work-contracts/2026-08-31-vd03-combat-m3d2-threat-delivery-unity-bridge.md`
- Requirements reviewed: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-MOV-001`, `REQ-WT-005`
- Acceptance criteria traced, not closed by this review: `AC-COM-001`, `AC-COM-003`; affected `AC-MOV-001`, `AC-WT-005`
- Reviewer: Luna
- Decision authority: Sol
- Result: **PASS — P0=0, P1=0**

## Review disposition

Luna first rejected the Review draft with five P1 findings and one P2 ambiguity. Sol amended the contract and requested a complete rereview rather than accepting partial deltas. The final rereview found no P0 or P1 contradiction. The remaining editorial P2 was also corrected before approval.

## Closed findings

1. Replaced the impossible `int.MaxValue-2` integrated boundary with `H=max(21,maxRegisteredInvulnerabilityTicks)`, latest M3D source `int.MaxValue-H-1`, and exact delivery `int.MaxValue-H`. Combat-only and isolated M3D1 boundaries remain distinct.
2. Fixed production registration to `RegularEnemyThreatSimulationDriver.Awake`, before every `FixedUpdate`, and required pristine Combat/M2A/player alignment, no latest outcome and no prior Combat attempt.
3. Applied the identical preview→delivery-preflight→revalidate→M3D1-commit→delivery-commit→view transaction and fail-stop boundary to encounter reset.
4. Pinned literal player/walker/surveyor scalar identities and required the bound player controller, frozen pose root, capsule and kinematic body to share the exact Transform.
5. Distinguished the legacy consume-on-attempt Combat input queue from the peek-before-preflight threat-delivery queue and made preservation claims queue-specific.
6. Split death evidence by role: player suppresses all new threats, walker suppresses contact, Surveyor suppresses only new line spawn, while an already active line may persist to expiry.
7. Corrected the final editorial phrase to “largest M3D-registration-admissible first source tick.”

## Implementation gate

Sol approved the corrected contract only after Luna's final `P0=0`, `P1=0` result. This document is contract-review evidence, not implementation verification. No acceptance criterion is closed until the Approved contract is implemented and Luna independently verifies the required Unity and regression evidence.

## Implementation-correction addendum gate

Luna's first implementation review rejected the code with four P1 findings. Sol reopened the contract rather than normalizing the deviations. The addendum gives M2A sole-authority mutation-free preview/exact-commit seams, separates Combat preparation from atomic lane registration, requires exact cross-driver graph validation, and permits only bounded per-root collider enumeration that validates an already frozen collider without selecting runtime roles or references.

Luna rejected the first addendum draft until reset staged and committed on the same new `BasicAttackSession(t)`, allowed graph seams and evidence were explicit, and the per-root enumeration count was exact. The final complete addendum rereview passed with `P0=0`, `P1=0`; its last singular/plural P2 wording was corrected before Sol reapproved the contract. Implementation correction may now resume, but integration still requires independent code review and executable Unity evidence.

## Ollama utilization record

The contract's required ledger records GLM 5.2 as used and accepted in part, Kimi K3 as used and accepted in part, and MiniMax M3 as failed and replaced after an empty bounded response without a quota/rate error. Luna reviewed the GPT-owned integrated contract; no Ollama output received approval authority.
