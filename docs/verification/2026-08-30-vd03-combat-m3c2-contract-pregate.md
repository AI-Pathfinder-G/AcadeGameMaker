# VD-03 M3C2 Contract Pre-Gate

- Date: 2026-08-30
- Reviewer: Luna
- Decision owner: Sol
- Result: PASS
- Contract: [VD-03 M3C2 Regular-Enemy Locomotion Unity Bridge](../specs/work-contracts/2026-08-30-vd03-combat-m3c2-enemy-locomotion-unity-bridge.md)
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Planned partial evidence: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`

## Independent result

Luna's first pass found no P0 and three P1 contract ambiguities: incomplete Rigidbody2D invariants, underspecified BoxCast distance/normal quantization, and an initialization-grounded probe that depended on a nonexistent prior grounded state. Luna also recommended an exact paired-reset discriminator.

Sol corrected the contract and `SYSTEM-CONTRACTS.md` to fix:

- exact active, simulated kinematic, constraint, interpolation, contact, rotation and velocity invariants before preview and immediately before commit;
- the installed Unity 6000.3.21f nonalloc BoxCast overload, `(requestedQ4096 + 82) / 4096f` envelope, exact filter, saturation, finite-hit, signed-normal floor-Q4096 and stable-order rules;
- unconditional walker-then-surveyor initialization support probes, distinct from conditional gameplay stationary-support probes;
- paired exact Reset selection, mixed-pair rejection and query-free reset restoration.

Luna's second pass returned `PASS`, with P0 `0` and P1 `0`. Two nonblocking test-wording recommendations were also incorporated into the required evidence list: malformed Rigidbody variants are enumerated, and death-query evidence is explicitly after successful initialization.

## Sol approval

Sol accepts the corrected boundary and promotes the contract from `Review` to `Approved`. Terra may implement only within the contract's allowed-file list. This gate authorizes implementation but does not itself satisfy the listed acceptance criteria.

## Ollama utilization disposition

The contract's three-lane ledger remains authoritative: Kimi and GLM were used and accepted in part after Sol screening; MiniMax returned an empty non-thinking response without a quota signal and was recorded as failed and replaced. No Ollama output has approval or integration authority.
