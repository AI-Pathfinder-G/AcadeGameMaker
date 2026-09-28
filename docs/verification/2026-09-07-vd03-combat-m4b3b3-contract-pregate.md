# VD-03 M4B3B3 계약 사전 게이트 증적

- Date: 2026-09-07
- Reviewer: Luna
- Integration authority: Sol
- Contract: `docs/specs/work-contracts/2026-09-07-vd03-combat-m4b3b3-ordan-audit-pull.md`
- Result: **PASS**
- Findings: `P0=0`, `P1=0`, `P2=0`

## Review coverage

Luna independently checked the contract against the existing M4A/M4B3B1/M4B3B2 boundaries, VD-01 movement ownership and the updated `SYSTEM-CONTRACTS` phase order.

- `AC-M4B3B3-001`: exact M4A projection, `t+1` horizon, age-0/final rules and `-165` phase are testable.
- `AC-M4B3B3-002`: `distanceSquared <= 410²` direct path, ceil integer sqrt, toward-zero scaling and radial postcondition close the `(410,1)` and `(600,600)` counterexamples without floating point.
- `AC-M4B3B3-003..006`: the owner-bound preflight/commit/cancel lane, pure motor preflight, ordinary-motion-first composition, collision ownership and Transfer modifier order preserve the Movement authority.
- `AC-M4B3B3-007..008`: age-0/later/final interrupt, boss/player death, the exact four Transfer lifecycle reasons, terminal latch and non-rollback precedence are explicit.
- `AC-M4B3B3-009..010`: every-tick Movement receipt, queued-next versus consumed-current handoff, 30/60/144 trace, builder/validator and regression/static guards are independently verifiable.

## Corrections made before PASS

The first review found unresolved physical values, movement ownership, same-tick interrupt precedence and tick horizon. Sol froze the Ordan-center anchor, maximum step `410` Q4096, continuous Execute window, source `t` to movement `t+1`, the `-165` producer and interrupt/death/lifecycle cancellation.

A later review found radial rounding counterexamples, unsynchronized system phase text and underspecified lifecycle/receipt bindings. Sol replaced nearest scaling with ceil-sqrt toward-zero scaling plus a radial postcondition, enumerated `RoomLeaving|RunFailed|Cutscene|DemoCompleted`, required an explicit Movement receipt on every successful tick, separated consumed-current from queued-next, and updated `SYSTEM-CONTRACTS`.

## Ollama utilization evidence

- GLM 5.2: `used and accepted in part`; retained strict tick expiry, interrupt priority, zero-distance, lifecycle and cadence cases; rejected saturation and guessed movement precedence.
- Kimi K3: `used and accepted in part`; retained immutable directive and preflight/commit/consume concepts; rejected invented public types, command rewriting and guessed constants.
- MiniMax M3: `used and accepted in part`; retained table fixtures, first-divergent-tick diagnostics, symmetry and cadence checks; rejected half-tick, random/contact and apply-then-nullify semantics.

All three prompts were abstract and non-sensitive. Ollama had no approval authority; Luna screened the contract and Sol alone approved it.

## Gate decision

Sol changed the work contract from `Review` to `Approved`. Terra may implement only the approved allowlist. Verification remains open until Luna reviews AC-linked Unity evidence.
