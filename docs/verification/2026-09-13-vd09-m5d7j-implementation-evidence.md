# VD-09 M5D7J binding-apply-failure recovery transform — Terra implementation evidence

Status: Verified — Astra accepted the final deterministic results after [Luna post-review](2026-09-13-vd09-m5d7j-luna-postreview.md) PASS (P0=0, P1=0; residual P2=3). This verifies binding-apply-failure transformation only, not failure authentication, persistence, or launch sequencing.

- Contract: [Approved M5D7J](../specs/work-contracts/2026-09-13-vd09-m5d7j-binding-apply-failure-recovery-transform.md); [Luna pre-gate PASS](2026-09-13-vd09-m5d7j-contract-pregate.md).
- REQ-M5D7J-001..005: two additive planner entries accept only `ValidCurrentInput` sources with nonempty canonical overrides. They replace only input with the current empty default, preserve immutable non-input snapshots, and bind the original nonempty input to both distinct private proof kinds and a private canonical-text identity proof. They produce exact primary `InputRepair` or previous `PreviousPromotion|InputRepair` at one checked increment.
- REQ-M5D7J-005..007: dedicated EditMode fixture covers revision boundaries; current-empty, metadata-recovery, and rehashed malformed-inner-binding recovery inputs; invalid/unsupported/default/reflection-invalid decode boundaries for both entries; proof and plausible result-document reflection mutations with getter fail-closure; public reason numeric compatibility; and static no-authority scan. Existing M5D7C test source is unchanged.
- Diagnostic focused execution (Astra, pre-remediation): 1 passed / 5 total; four fixtures constructed `ProfileProgressionSnapshot` with seed `123`, outside the approved seed domain, so they failed before planner invocation. The fixture now uses valid seed `101` and a valid distinct proof-mutation seed `202`; choice, skill, and branch combinations remain valid. This was a test-fixture defect, not a runtime finding.
- Final Astra runs (all with failed/skipped/inconclusive zero): M5D7J focused EditMode r2 `5/5`, artifact SHA-256 `d98487558b6f6912412e12b0b81c7601b5debbad7c97392fc8c5b734f6dc04bb`; unchanged M5D7C focused regression `10/10`, artifact SHA-256 `50a085a180d7efd7a8567ae00e0749be87c7697104596caafa07c654a88dbedf`; full EditMode `652/652`, artifact SHA-256 `98f568a924705be95d8822347f31578e0924fc93bb0f17e907e05da3c0ac91c6`; full PlayMode `584/584`, artifact SHA-256 `380927491dd1460694076ad1b2055637e79c473122b95df67d3f2520e2b6255e`.

## Current scoped hashes

| Scope | SHA-256 |
| --- | --- |
| Runtime planner | `e72fe64417385170391e2b7ee6dc7740e448ac1828dc77bff7a603db235f56f8` |
| M5D7J EditMode fixture | `d7b2bb12686cafc0ac5caed5483b01240c27d309401cd50d49f526b02ea43505` |
| M5D7J EditMode fixture meta | `351e39332fab391efb44a66fd093aac020cb45294d96a951536a3a5e1f456c6c` |
| Approved contract | `b85e4a4b23932d1d346f94363d40eb9008b7fc78ef94c2b9c31917f306895f98` |
| Luna pre-gate | `0d3ea5e0799064c542f796df2311019129c51fb7e59f3daaa3aeecc2853f3ec7` |

- Static checks completed: scoped diff whitespace check, constructor-call audit, and runtime forbidden-authority scan. Terra did not run Unity from the sandbox. The final results above do not replace Luna post-review or Astra acceptance.
