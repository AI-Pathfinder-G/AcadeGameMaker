# VD-03 M3D1 Contract Pre-Gate

- Date: 2026-08-31
- Reviewer: Luna
- Decision owner: Sol
- Result: PASS — P0 `0`, P1 `0`, P2 `0`
- Contract: [VD-03 M3D1 Deterministic Regular-Enemy Threat Core](../specs/work-contracts/2026-08-31-vd03-combat-m3d1-regular-enemy-threat-core.md)
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`
- Planned partial evidence: `AC-COM-001`, `AC-COM-003`

## Equal-X compatibility addendum rereview

Executable M3D2 integration later exposed that M3D1 rejected the upstream-authorized zero walker direction at exact equal X. Sol reopened only that compatibility boundary. Luna verified that Approved M3B2A owns direction `-1/0/1` with zero on equality and that Approved M3C1 already accepts the same domain through `DashTelegraph` and `DashActive`.

The addendum changes only those two M3D1 shape checks from nonzero direction to the full `IsDirection` domain, retaining exact active direction/latched equality and every phase, tick, movement, telegraph, contact and request rule. Luna's final addendum result was **PASS — P0=0, P1=0, P2=0**. Sol approved it only after this result; executable compatibility and regression evidence remain required before integration.

## Independent result

Luna's first pass found no P0 and three P1 ambiguities: repeated-dead wording contradicted posthumous denial-line lifetime, M3B2A's Fire intent ordinal `n` versus published snapshot ordinal `n+1` was not fixed, and Reset attempted to seed centers absent from its input. Luna also requested a concrete meaning for enum round-trip evidence.

Sol corrected the contract so that:

- player death suppresses all requests, walker death suppresses contact only, and Surveyor death suppresses only new shots while an older line keeps testing through its exact expiry;
- an alive Fire snapshot carries the exact intent/latch relation and publishes `expectedOrdinal+1`, while every other phase carries no intent and retains the expected ordinal;
- Reset clears sweep history without storing a center, and the next Gameplay input performs static reseeding;
- `EnemyRanged=6` evidence explicitly covers request property preservation, validation, unchanged canonical sorting and echoed result behavior.

The second pass returned PASS with no remaining finding. Luna also confirmed the M3D1/M3D2A/M3D2B split, signed tick horizons, exact BigInteger rational sweep feasibility, line geometry/lifetime, request IDs, atomic candidate model and allowlist.

## Sol approval

Sol accepts the corrected boundary and promotes M3D1 from `Review` to `Approved`. Terra may implement only inside the contract allowlist. This gate authorizes implementation but does not itself satisfy an acceptance criterion.

## Ollama disposition

The contract ledger is authoritative: GLM and Kimi were used and accepted in part after Sol screening; MiniMax returned empty non-thinking output without a quota/rate error and was failed and replaced. No cloud model received repository material or integration authority.
