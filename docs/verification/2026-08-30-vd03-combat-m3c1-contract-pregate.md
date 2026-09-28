# VD-03 M3C1 Contract Pre-Gate — Luna PASS

- Date: 2026-08-30
- Contract: [VD-03 M3C1](../specs/work-contracts/2026-08-30-vd03-combat-m3c1-enemy-locomotion-core.md)
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance mapping: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-005`
- Result: PASS — no remaining P0, P1 or actionable P2; Sol approved implementation.

## Review history

Luna's first pass rejected the Review draft with eight P1 ambiguities: Unity carried-view dependency in a pure core, conflicting Q1000/remainder math, incomplete grounded/resolution rules, incomplete phase-intent combinations, hidden validation-candidate ownership, reset/death collision semantics, unreachable evidence horizons and missing `REQ-WT-003` traceability.

Sol corrected the contract to use a Combat-only discriminated input; fixed multiplier rounding order and exact rates; retained `/60` remainders only; defined semi-implicit vertical integration; completed pair-resolution, support, reset and death transactions; enumerated every reachable phase-intent-reaction row; made validation return an immutable recomputed candidate; and replaced unreachable evidence with 60/12/48-tick reachable windows and the terminal `int.MaxValue-1` boundary.

The second pass found two further verified-upstream/collision-envelope issues. M3B2A legitimately permits a zero latched dash direction when player and walker X are equal, so M3C1 now accepts zero `DashHorizontal` and clears its X remainder. Alive grounding is now exact, negative travel uses magnitude, and dead identity resolution has an explicit exception that preserves prior grounded without claiming a support probe.

## Final Luna findings

- Exact derived rates `9216`, `3226`, `36864`, `12902` and `180224` follow the frozen Q1000 order.
- Core-only input direction, `/60` canonical remainders and semi-implicit vertical order are implementable without Unity dependencies.
- Full reachable phase tables, immutable candidate recomputation, collision-free reset, dead identity exception and terminal tick rules require no Terra invention.
- M3C1 validates integer identity/envelope/atomicity only; M3C2 remains sole owner of cast and stationary-support truth.
- Required evidence is reachable and appropriately partial; M3C1 alone does not close full acceptance criteria.

Sol accepts Luna's PASS and changes M3C1 from `Review` to `Approved`. Terra may implement only the contract allowlist. Unity locomotion, contact damage, denial lines, scenes and project settings remain unauthorized.
