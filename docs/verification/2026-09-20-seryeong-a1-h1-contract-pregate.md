# Seryeong A1/H1 first-media contract pre-gate

- Date: 2026-09-20
- Contract: `docs/specs/work-contracts/2026-09-20-seryeong-a1-h1-first-media-package.md`
- Independent reviewer: Luna
- Final severity count: P0 `0`, P1 `0`, P2 `0`
- Astra disposition: Approved for offline tooling and non-wearable staging only

## Review history

The initial counter-review identified an over-broad action scope, ambiguity between APV3 combination acceptance and the vertical-demo media slice, an offline/catalog publication contradiction, missing default-grant semantics, incomplete action/facing vocabulary, insufficient source pinning, and an incomplete rights boundary. The contract was revised to an idle-only two-facing slice with exact hashes, geometry, package atomicity, exclusions, and separate offline and live-publication gates.

Luna's final recheck found no remaining P0, P1, or P2 issue. The CUA contract now carries the canonical `(actionId,facing)` seam, while the first-media contract explicitly separates offline authoring from the later Unity import and runtime-default decision.

## Approval boundary

This gate satisfies contract review for deterministic offline tooling and staging under `REQ-SM1-001..010`. It does not satisfy `AC-SM1-007`, because the rights chain is still `PendingUserAttestation`, and it does not satisfy the complete two-facing payload criteria while separately authored left-facing frames are absent.

Accordingly:

- staged output remains pending, non-addressable, and non-wearable;
- APV3 remains `0/432` and the runtime catalog remains `NoAcceptedDefault`;
- no Unity asset, metadata, catalog, scene, prefab, animator, renderer, or wardrobe binding may change;
- Terra may implement deterministic packers, validators, evidence generation, and source diagnostics only;
- final media acceptance requires user rights attestation plus Luna review of the complete payload;
- live publication additionally requires the separate default/wearability choice and all child dependency gates.

## Evidence pins checked

The pre-gate confirmed all six contract pins against the workspace: A1 costume, H1 hair, movement source, movement provenance, calibration, and the v4 64px diagnostic candidate. The OpenAI terms note is deliberately limited to the OpenAI-output leg and does not substitute for authorization of upstream references.

## Result

**PASS WITH DEFERRED ACCEPTANCE.** The contract is Approved for offline tooling and safe staging. No generated or derived media receives acceptance credit until the deferred gates above are closed.
