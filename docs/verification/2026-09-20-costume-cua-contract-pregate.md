# Costume CUA Unity presentation adapter — Luna contract pre-gate

- Date: 2026-09-20
- Reviewer: Luna (`gpt-5.6-luna`)
- Reviewed: `docs/specs/work-contracts/2026-09-20-costume-cua-unity-presentation-adapter.md`
- Review type: bounded contract/public-seam review; no implementation, source/test edits, or Unity execution
- Status: **HOLD — P0=0, P1=1, P2=2; contract remains Review and grants no implementation authority**

## Decision

The pure costume/catalog/selection/presentation APIs are sufficient for an isolated additive CUA adapter without changing the pure core. The CUA contract's truthful all-pending behavior, synthetic-only positive fixtures, and no-scene/no-InputMode boundary are sound. However, one persistence-to-visual publication failure boundary is not closed, so this is not ready for Astra approval. The two P2 clarifications below should also be incorporated before implementation. No implementation or Unity work is authorized by this review.

## Findings

### P0 — none

### P1 — durable selection can diverge from the published visual pair

The contract correctly stages next state and the complete media candidate before invoking CIO, then publishes only for `CommittedFirst`/`CommittedReplacement`. It does not define what happens if the durable save succeeds but publication of the in-memory state, portrait, gameplay map, and pure binding then fails or throws. Retaining the old visual pair would contradict the newly durable selected state; claiming the new pair would be false. `CommitOutcomeUncertain` covers an uncertain CIO result, not this post-success publication boundary. This leaves the central durable-selection/presentation invariant underspecified (`REQ-CUA-004/007`, `AC-CUA-003/006`).

**Required closure:** make the final publication one prepared, adapter-owned immutable tuple (at minimum state/current ID, pure binding, portrait reference, gameplay clip map, and revision) and expose it through one no-fail publication step after the exact successful CIO result. Specify that no callback/read surface may observe intermediate assignments. If that single publication cannot be guaranteed after durable success, enter terminal `ReloadRequired`, disable further mutation, retain old media only as non-authoritative display, and require fresh CIO recovery; do not claim rollback or old durable state. Add a fault/observation test at the save-success/publication boundary and assert the defined terminal or single-publication result.

### P2 — exact save-receipt correlation should name the existing CIO contract

`CostumeFileSaveResultV1` exposes outcome, stage, and `ExpectedRevision`; it has no content digest or operation token. The CIO adapter itself synchronously encodes the supplied immutable `CostumeStateV1` and verifies reopened primary bytes against those exact bytes, so a direct synchronous `Save(stagedNextState)` call can provide the intended correlation. The CUA phrase “receipt correlates to those exact bytes and revision” should explicitly define this as the same synchronous call and expected revision, not imply a digest/token the current public receipt does not expose. Exercise a stale/wrong-revision receipt and same-revision/different-state case through the injected port boundary (`REQ-CUA-007`, `AC-CUA-006`).

### P2 — package hash inputs and snapshot-to-frame mapping need canonical test semantics

The pure definition carries `PortraitHash`, `AtlasHash`, and `ClipMapHash`; the CUA package adds source-manifest bytes and refers to canonical manifest/media hashes, but does not state the byte inputs each expected hash covers or how the combined manifest binds the three values. `AC-CUA-002` mutates “manifest payload” but lacks a byte-level canonical rule. Also, `CompletedActorSnapshotV1` exposes `ActionAgeQ1000` (0–1000), not a separate phase or clip duration. Specify the canonical hash-input mapping for the three catalog hashes and define how `ActionAgeQ1000` maps onto a clip frame, including endpoint/loop behavior. The existing `TrySwap` validates binding/catalog/actor/current clip but does not enforce monotonic tick, duplicate-tick identity, or snapshot provenance; keep those checks in the CUA adapter's explicit instance-bound post-commit observation port and cover them in AC-CUA-004. No pure API widening is needed.

## Public seam sufficiency

The current public pure seams are enough for the bounded implementation:

- `CostumeCatalogV1.SeryeongInventoryPending`, `Availability`, and `CostumePresentationServiceV1.BuildViews` support the truthful 36-row projection.
- `CostumeSelectionServiceV1.PrepareSelection/Commit` supports revision-bound, one-use selection; the adapter can stage the resulting immutable state before persistence.
- `CostumePresentationServiceV1.TryCreateBinding/TrySwap` and immutable `CostumePresentationBindingV1` support Accepted-only binding identity checks. The Unity adapter must additionally validate Unity references/package payloads and preserve/render the snapshot frame; `TrySwap` does not perform those Unity duties.
- `CompletedActorSnapshotV1` supplies tick, actor, action, bounded `ActionAgeQ1000`, facing, and anchor. Per the parent Child B contract, it must arrive through an explicit instance-bound observation port after the simulation owner commits the tick. The value is publicly constructible and is not itself proof of post-commit origin; this CUA slice may use synthetic snapshots only and must not claim live simulation integration.
- `CostumeFileAdapterV1.Save` is synchronous and verifies exact encoded/reopened bytes. Inject the already-constructed CIO adapter/port; CUA must not choose a persistence root. CIO reports typed commit outcomes and expected revision, not a content digest.

No existing pure API change is justified. If the implementation cannot satisfy the correlation, snapshot-source, or atomic-publication constraints inside the CUA allowlist, stop and request a separate Astra-approved amendment rather than widening existing owners.

## Authority, dependencies, and stop conditions

- **CUA authority:** contract status remains `Review`. Do not begin CUA implementation until Astra resolves P1/P2, records an explicit `Approved` status, and confirms the dependency gates below. This report is a pre-gate review only.
- **CIO:** Luna's latest CIO review reports no static/probe P0/P1/P2 findings, but `AC-CIO-007` Unity focused/full EditMode/PlayMode execution remains blocked/unrun. CUA must not represent CIO as fully verified or integrated until Astra closes that gate.
- **Q0/UI boundary:** M5D7Q0 is the Hub-UIOnly router/prefab contract, not a wardrobe screen. Its implementation evidence records focused Unity execution and Luna review pending. CUA may build only its isolated adapter-owned test views; no Canvas, EventSystem, screen/scene/prefab, navigation, InputMode, or existing owner connection is authorized here.
- **Real media:** the independent sprite audit records 0/432 Accepted Seryeong combinations, no imported character rasters, and unresolved use-rights evidence. CUA positive fixtures must remain synthetic; no real outfit preview, import, acceptance, or live sprite binding may be claimed. A real-media package requires its separate acceptance/import and rights gate.
- Do not run Unity under this task. Stop if correctness requires any non-allowlisted source/asmdef/test, existing owner edit, live simulation connection, media/catalog promotion, composition root, or persistence-root selection.

## Requirement / acceptance traceability

| Requirement | Contract AC mapping | Luna pre-gate disposition |
|---|---|---|
| `REQ-CUA-001` | `AC-CUA-001`, `AC-CUA-008` | Pass for truthful all-pending projection; no real selection/default. |
| `REQ-CUA-002` | `AC-CUA-001` | Pass; highlight remains transient and must not reach save/media binding. |
| `REQ-CUA-003` | `AC-CUA-002` | Conditional on P2 canonical hash-input mapping and deterministic frame/package validation semantics. |
| `REQ-CUA-004` | `AC-CUA-002`, `AC-CUA-003` | **P1:** atomic pair behavior is normative, but durable-success-to-publication failure is unresolved. |
| `REQ-CUA-005` | `AC-CUA-004` | Conditional on explicit post-commit instance-bound source and the stated P2 tick/duplicate/frame checks. |
| `REQ-CUA-006` | `AC-CUA-005` | Pass as bounded visual-only invariant; tests must compare existing mechanics/physics identities. |
| `REQ-CUA-007` | `AC-CUA-006` | **P1:** publication closure unresolved; P2 also clarifies receipt correlation. |
| `REQ-CUA-008` | `AC-CUA-002`, `AC-CUA-008` | Pass; synthetic fixtures cannot promote real catalog/media. |
| `REQ-CUA-009` | `AC-CUA-007` | Pass with explicit instance-bound dependencies and no global discovery or existing-owner edits. |
| `REQ-CUA-010` | `AC-CUA-007`, `AC-CUA-009` | Pass as written allowlist; implementation and full-suite proof remain future gates. |
| All CUA requirements | `AC-CUA-009` | Not run. Unity licensing/Q0 verification are outstanding; no focused/full result is claimed. |

## Participation

Luna reviewed the named CUA contract against the current pure Costume/Presentation APIs, final CIO public surface and review, parent Child B boundaries, Q0 evidence, and sprite readiness audit. No runtime/test source was edited; no test or Unity process was run. Astra retains approval and integration authority.

## Amendment re-review — 2026-09-20

- Reviewed only the amended CUA contract; no runtime/test files were inspected or changed, and Unity was not run.
- **Updated disposition: P0=0, P1=0, P2=1.** The P1 durable-save/publication gap is closed by the prepared immutable tuple, one non-throwing reference publication, and terminal `ReloadRequired` behavior after a projection failure. The earlier receipt-correlation P2 is closed by requiring the exact synchronous concrete `CostumeFileAdapterV1.Save(stagedNextState)` call and matching `ExpectedRevision`. The hash-input and frame-mapping P2 is materially closed by the explicit PNG/clip-map byte rules and loop/non-loop endpoint formulas.
- **Residual P2:** canonical clip-map JSON defines the top-level key order and the clip's `actionId,loop,frames` order, but does not enumerate the exact frame-object property names/order for rectangle, pivot, baseline, and duration values. That small schema detail should be fixed before claiming cross-producer canonical bytes; it does not reopen the prior P1.
- **Authority/dependencies unchanged:** CUA remains `Review` and not implementation-authorized. The amended contract explicitly gates approval on CIO `AC-CIO-007` and M5D7Q0 verification. Real media remains source-only; the audit still records 0/432 accepted combinations. No live UI or gameplay integration is authorized by this slice.
