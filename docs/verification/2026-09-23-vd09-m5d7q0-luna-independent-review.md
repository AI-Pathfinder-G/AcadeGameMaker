# VD-09 M5D7Q0 Hub-UIOnly router graph — Luna independent post-review

- Review date: 2026-09-23
- Re-review basis: Astra's 2026-09-23 AC-M5D7Q0-011 amendment, final scope rerun, refreshed implementation evidence, and superseding `final-manifest-v2.json`.
- Reviewer: Luna (`gpt-5.6-luna`)
- Contract: `docs/specs/work-contracts/2026-09-20-vd09-m5d7q0-hub-ui-only-router-graph.md`
- Implementation evidence: `docs/verification/2026-09-20-vd09-m5d7q0-implementation-evidence.md`
- Review mode: independent static/evidence review; no production or test source was modified and no Unity runner was launched by this review.
- Result: **ACCEPT — P0=0, P1=0, P2=0, subject to Astra's final integration record.**

## Scope and independent checks

I read `AGENTS.md`, `docs/README.md`, `CONTEXT.md`, `docs/agent-operating-model.md`, ADR-0032, the Approved M5D7Q0 contract, the 2026-09-20 Luna pre-gate, the 2026-09-22 Luna review, and Terra's implementation evidence. I independently inspected:

- `HubRuntimeAuthoringBuilder.cs`, `HubRuntimeAuthoringValidator.cs`, the focused authoring/scope tests, the current `InputRouter.cs`, and the canonical prefab plus its folder/prefab metadata.
- Current hashes. The deterministic final bytes are `HubRuntimeRoot.prefab` `B1CFD731A4E177C37B59CEA0813E3B51E44DD10BA96AE38A1B15F15012438EEE`, `HubRuntimeRoot.prefab.meta` `B4C188F55CD7DA32649008AB5FA84CC434C04FF10BF64F1AB7C31071063D86AE`, `Hub.meta` `772772C20B2021B4094ACD0600624F13714D7EB3F8E7E8BB93FF0FBC5F1F6749`, and `HubAuthoring.meta` `A3539C6B6B0594852E4C779DF110B0E20B092A108FB8FDCB4429EC798DF6B5FC`.
- The retained XML under `artifacts/unity-results/m5d7q0-20260922-reboot` and their SHA-256 values. XML top-level counts are independently parseable and all listed runs have zero failed, skipped, and inconclusive tests.
- Static path/reference searches. The current runtime/PlayMode source trees contain no `HubRuntimeRoot` or `Assets/Prefabs/Hub` consumer; the prefab is used by Editor authoring tests/builder only. The current runtime still has exactly one `InputRouter` implementing `IUiSemanticFrameSourceV1`, `GameInputActions.IGameplayActions`, and `GameInputActions.IUIActions`, with no `InputSystemUIInputModule`, `Mouse.current`, or second router construction in the inspected input runtime subtree.
- The superseding `final-manifest-v2.json`: its SHA-256 is `B7D525428BE895E19BC42B77D260059833204386331C481716EDEA9D51B7BEEA`; it has 33 Q0 baseline/current rows, a 374-record immutable dirty-inventory commitment, and every referenced XML/log hash independently matches the retained files. The baseline rows match `baseline-manifest.json` exactly; four current Q0 rows differ as expected (builder, authoring tests, scope test, and canonical prefab).

## P1 findings

### P1-01 — Resolved: final manifest and evidence provenance

The prior finding applied to the historical `final-manifest.json`, which described the pre-repair prefab and stale result states. Astra's superseding `final-manifest-v2.json` now records all 33 baseline/current path rows, every retained result count and XML hash, explicit provenance, and `null` log fields where the Unity CLI produced no same-invocation log. Independent checks found zero missing files, hash mismatches, or XML/count mismatches. The refreshed implementation evidence source table also matches every current listed source hash (`evidence_mismatch_count=0`).

**Disposition:** **Resolved.** The historical manifest remains preserved as history; v2 is the authoritative final evidence manifest.

### P1-02 — Resolved: AC-M5D7Q0-011 honest dirty-inventory boundary

The contract is now explicitly amended so AC-011 does not claim a clean repository-wide before/after comparison. It requires the immutable 374-record dirty-worktree commitment, the exact 33-path Q0 baseline/current hash boundary, explicit non-Q0 pre-existing-or-unattributable classification, and the single-subscriber/frame-source checks. The current scope test comments and `AllowedDocumentation` now match that contract and admit both the 2026-09-22 prior review and this 2026-09-23 successor. The final scope run is independently verified as `4/4`, zero failed/skipped/inconclusive, duration `0.3514881`, XML SHA-256 `CC20E761D4ACD43D59ACBB8069EE060563BE87C2CD19E59D836A054F002D0733`, with same-invocation log SHA-256 `64AE48519D84B5C2485B48F518C3A0B33577BDB21D511D003FCBE72881D2D448`.

The v2 manifest independently confirms that all 33 baseline rows equal the immutable baseline manifest and that the current workspace has exactly four intended Q0 deltas. This is the approved honest boundary, not a clean-worktree claim.

**Disposition:** **Resolved.** AC-011 is accepted on the amended contract's explicit provenance terms.

## PlayMode reuse decision

**The historical full PlayMode `943/943` result may be reused for the final revision; a full PlayMode rerun is not required solely because of this deterministic Editor authoring repair.**

Evidence for that decision:

1. The final-revision source deltas are confined to `HubRuntimeAuthoringBuilder.cs`, the Editor authoring tests/scope audit, and canonical prefab serialization/local IDs. The runtime `InputRouter.cs`, adapter/latch runtime sources, Q0 PlayMode tests, direct dependency tests, and generated input sources are unchanged from the recorded baseline hashes.
2. Static search finds no `HubRuntimeRoot`/`Assets/Prefabs/Hub` reference in the runtime or PlayMode test trees. Q0 PlayMode fixtures construct their own test cohort; the canonical prefab is exercised by Editor authoring/validator tests only. The fixed local IDs preserve the same four-component graph and sibling references, so this Editor-only serialization change has no observed PlayMode execution consumer.
3. The retained `full-playmode.xml` is a valid Unity result: start `2026-09-22 19:23:24Z`, `943/943` passed, failed/skipped/inconclusive `0`, duration `4997.7963525`, SHA-256 `E28011588D077077FF629C7651E06DB6ACDCFA7493AB40DA0E90FE6BF0293A78`. It predates the deterministic repair, so it remains historical evidence rather than a fresh post-revision run; reuse is based on the verified impact boundary above.

This reuse decision does not waive the required current-source Editor evidence. The final deterministic builder/process-B XML is `31/31`, SHA-256 `0AD583AA8066B864187A3AA82C55FE53ECB9FC0849D139F64C7A28814A4A2F4E`; the final amended scope run is `4/4`, SHA-256 `CC20E761D4ACD43D59ACBB8069EE060563BE87C2CD19E59D836A054F002D0733`; and deterministic full EditMode is `737/737`, SHA-256 `DEDFFC470FA86A40ACD6321154AF57D75FB5231F852641F2FBC4A6DE1B1A5B4D`.

If Astra requires a literal post-revision PlayMode execution despite this impact proof, the exact bounded fallback is the four current Q0 PlayMode filters (`HubUiOnlyInputRouterPlayModeTests`, `HubUiOnlyQ0CoverageBPlayModeTests`, `HubUiOnlyQ0HandoffMatrixPlayModeTests`, and `HubUiOnlyQ0RemainingRuntimePlayModeTests`) followed by direct M5B5/M5D7M/M5D7N/M5D7P-A and then full PlayMode. That is a policy fallback, not a technical necessity identified by this review.

## Full EditMode reuse decision

**The prior deterministic full EditMode `737/737` result is sufficient; a second full EditMode rerun is not required for the final scope-test-only amendment.**

The final amendment changes only the focused `HubUiOnlyQ0ScopeAuditEditModeTests` allowlist/comments and the corresponding documentation/manifest evidence. It does not change production/runtime code, the builder, the validator, any PlayMode test, or any other EditMode test. The final scope test itself was rerun after the amendment (`4/4`, zero failed/skipped/inconclusive, XML `CC20E761…`, same-invocation log `64AE4851…`). The earlier deterministic full EditMode XML (`737/737`, zero failed/skipped/inconclusive, XML `DEDFFC47…`) covers the unchanged suite; composition of that full result with the final focused `4/4` rerun covers the only executable test code that changed. A literal full-suite rerun would be redundant impact-wise, though Astra may request it as a procedural preference.

## AC/REQ disposition

| Acceptance criteria | Independent disposition | Evidence |
|---|---|---|
| AC-M5D7Q0-001..007, 009, 012 | Current focused/direct runtime evidence is consistent with the claims; no P0/P1 code defect found statically. | Focused router `38/38` (`C0959748…`), Coverage-B `27/27` (`E55336D9…`), handoff `5/5` (`E7A66E68…`), remaining `18/18` (`6D4F82DD…`), direct M5B5 `3/3`, M5D7M `117/117`, M5D7N `69/69`, M5D7P-A `52/52`. |
| AC-M5D7Q0-008 | Accepted on current deterministic evidence. | Builder/process-B `31/31`; current prefab/meta hashes above; validator is observational and fixed-YAML repair is only in the builder. |
| AC-M5D7Q0-010 | Accepted with bounded full-suite reuse decisions. | Current deterministic full EditMode `737/737`; final scope amendment rerun `4/4`; historical full PlayMode `943/943`; direct/current XMLs above. |
| AC-M5D7Q0-011 | Accepted on the amended honest dirty-inventory/33-path boundary. | Final scope `4/4` (`CC20E761…`), v2 manifest with 33 baseline/current rows and exact 374-record inventory commitment. |

Requirement traceability follows the contract's table: REQ-M5D7Q0-001..008 are exercised by AC-001..007/009/012 evidence; REQ-M5D7Q0-009 by AC-008/011; REQ-M5D7Q0-010 by AC-001/004/010/011; and REQ-M5D7Q0-011 by the Approved contract/dependency record. All listed requirements are accepted by this Luna review on the amended evidence boundary; Astra alone records final integration status.

## Recommendation

**ACCEPT.** No P0, P1, or P2 finding remains. The stale historical manifest is superseded by independently verified `final-manifest-v2.json`; AC-011 is resolved by the approved honest 374-record/33-path amendment; the final scope test is `4/4`; full EditMode and full PlayMode are covered by bounded impact-based reuse decisions. Astra alone records final integration acceptance. No production/test source was changed by Luna.

