# Kimi K3 VD-02 M2A fixed-math test-vector screening

- Draft source: Ollama Cloud `kimi-k3:cloud`, bounded non-secret test-vector request
- Scope supplied: fixed `RoundDivAway`, projection/inverse, screen distance, ellipse, mouse threshold/Q4096 normalization, and 24-step CORDIC only
- Reviewer: Terra
- Status: screened; no Kimi code copied

## Accepted input

- The implementation uses the suggested remainder comparison `abs(remainder) >= denominator - abs(remainder)` rather than doubling the remainder. This preserves the approved signed half-away rule without creating a `2 * remainder` overflow path. It is independently required and covered by `AC-WT-001` tests.
- The draft correctly called out signed integer division truncation and `long.MinValue` absolute-value hazards. Terra implemented checked handling directly from the Approved M2 addendum.

## Rejected input

- The draft did not receive the exact frozen projection formulas and consequently speculated about alternate orthographic maps, camera pitch, and parameterized geometry. Those proposals are incompatible with the Approved fixed zero-rotation Q1000 formulas and were not used.
- Its output was reasoning-heavy rather than the requested compact test-vector list. It supplied no code and no authoritative CORDIC expected values.

## Terra implementation decision

All constants, API shapes, projection formulas, CORDIC table, and test expectations are implemented only from the Approved work contract and Luna pre-gate. Kimi made no architecture, contract, or acceptance decision.
