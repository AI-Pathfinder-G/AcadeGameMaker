# Costume CUA Unity presentation adapter — final Luna contract pre-gate

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Review type: independent bounded contract/public-seam review; no implementation and no Unity execution
- Reviewed contract: `docs/specs/work-contracts/2026-09-20-costume-cua-unity-presentation-adapter.md`
- Contract SHA-256: `63840565A19BE20A09193437B7ECB2CBC5DE0E447D52C5500A6C78A428D613CA`
- Prior review: `docs/verification/2026-09-20-costume-cua-contract-pregate.md` SHA-256 `EE5E4D4D6ABB28FD414A034DEBC2633DF5DEBB1B90AD24D242CABFF29F038C31`

## Verdict

**PASS — P0=0, P1=0, P2=0.** The previously reported publication P1 and the two contract-clarification P2 findings are normatively closed in the reviewed contract. Astra may promote this contract from `Review` to `Approved` without a new user product decision. This is approval to authorize the bounded implementation gate only; it is not implementation acceptance and does not replace Terra implementation or Luna runtime/full-suite verification.

## Inputs and dependency gates

The dependency state required by the contract is present:

| Dependency | Current SHA-256 | Required state/evidence | Result |
|---|---|---|---|
| CIO file adapter contract | `CE6F7C57A09F57523A8BF12C919EADF32C25A8DCB5B5ABDA5736F7A2992AA1FB` | `Verified`; `AC-CIO-007` closure evidence `A0839B317261192EB48F505AF09BDC6DFFCE6898A7901B2E4530FA1C36467A91` | Pass |
| M5D7Q0 Hub-UIOnly router graph | `91F67947EE63E9E608DDAB32A345520FDA926A513C7D99578761CBE9022B629F` | `Verified` | Pass |
| M5D7Q-A resolution capture | `91C1B9E5A1F1294C14DFFB63623914FDA58BB9BD7C0EB1E3220C691F531B8D32` | `Verified`; final Luna postreview `B73F2AA693244B141244EE2F34EB12780DB059674BE057A07F5ED191019F3E67` | Pass |

The CIO public implementation reviewed for the save seam is `Assets/AcadeGameMaker/Runtime/Costumes/IO/CostumeFileAdapterV1.cs`, SHA-256 `67CCF0DE7E74B3FA9AE6582CCD1AF04D35F61A2135A029D0AA089A1A1B798A8E`. The pure presentation seam is `Assets/AcadeGameMaker/Runtime/Presentation/CostumePresentationV1.cs`, SHA-256 `600077CAE9B792451A37CC977FC19632E7AE2309428978CED4552937CB6C27DA`; the costume core is `Assets/AcadeGameMaker/Runtime/Costumes/CostumeCoreV1.cs`, SHA-256 `9149AD9E4D806F02811C0D85F5E1DB044E6380C70B5409C4929CA09913EF8178`.

## Closure of the prior findings

### Prior P1 — durable save/publication divergence: closed

The contract now requires staging the exact next state, canonical bytes, view, and complete media candidate, then constructing one immutable adapter-owned `PublishedCostumePresentationV1` tuple before persistence. The tuple includes state/current ID, pure binding, portrait reference, gameplay clip map, and correlated revisions, and every field/reference is validated before the save call. After a successful concrete save, publication is exactly one non-throwing reference assignment from the old tuple to the already-complete new tuple; readers snapshot that one reference and there are no intermediate component assignments or observation callbacks. A post-save projection failure leaves the durable/new tuple authoritative, enters terminal `ReloadRequired`, disables mutation, and labels retained old pixels non-authoritative. `AC-CUA-003` and `AC-CUA-006` require the corresponding observation/fault coverage. This closes the old durable-success/publication ambiguity without claiming rollback.

### Prior P2 — receipt correlation: closed

The contract names the actual public CIO seam: the same immutable staged state is passed synchronously to the injected concrete `CostumeFileAdapterV1.Save(stagedNextState)`. Only `CommittedFirst` or `CommittedReplacement` with `ExpectedRevision == stagedNextState.Revision` permits publication. Detached receipts, asynchronous callbacks, user-supplied results, and alternate save delegates are forbidden. The current CIO surface exposes exactly `Save(CostumeStateV1)` and the typed `CostumeFileSaveResultV1` properties `Outcome`, `Stage`, and `ExpectedRevision`; its receipt constructor is not public, so a same-revision/different-state detached result cannot be supplied through the seam. `AC-CUA-006` explicitly requires stale/wrong revision rejection and proves this boundary.

### Prior P2 — canonical hash inputs and snapshot/frame mapping: closed

The contract now binds `PortraitHash` to exact portrait PNG bytes, `AtlasHash` to exact gameplay-atlas PNG bytes, and `ClipMapHash` to exact UTF-8 canonical clip-map bytes. The source manifest binds IDs, revisions, all three hashes, required actions/facings, and schema version without replacing any payload hash. The clip-map schema fixes UTF-8/no-BOM/no-insignificant-whitespace, ordinal top-level property order, ordinal action sorting, closed `left,right` facing order, exact clip property order, and exact frame property order `x,y,width,height,pivotXQ1000,pivotYQ1000,baselineY,durationTicks`; unknown/duplicate properties, invalid rectangles, and invalid dimensions reject. The non-loop and loop `ActionAgeQ1000` formulas, including the exact `1000` loop endpoint, are normative. `AC-CUA-002` and `AC-CUA-004` cover corruption and endpoint matrices.

## Public-seam and feasibility review

- `CostumeCatalogV1.SeryeongInventoryPending()` supplies the real 36-row pending catalog; `CostumeCatalogV1.Availability`, `CostumePresentationServiceV1.BuildViews`, and the existing state APIs are sufficient for the truthful all-pending view and `NoAcceptedDefault` result.
- `CostumePresentationServiceV1.TryCreateBinding` and `TrySwap` provide the existing pure binding identity checks. The Unity child may add package/reference/geometry validation inside its allowlist, while preserving action vocabulary in `ClipIds` and resolving `(ActionId,Facing)` only at presentation time.
- `CompletedActorSnapshotV1` is publicly constructible and carries tick, actor, action, bounded action age, facing, and anchor. The CUA contract correctly treats it as valid only from an explicit instance-bound post-commit observation port; it does not claim that the value object itself proves live simulation provenance.
- `CostumeFileAdapterV1.Save` is synchronous and owns canonical encode, durable transaction, reopen, exact-byte comparison, and decode verification. The CUA call site can use the concrete public API without widening the CIO contract or choosing a persistence root.
- The immutable tuple/reference publication and terminal `ReloadRequired` behavior are implementable without a fallible post-save construction step, provided Terra keeps construction before Save and uses only the named instance-bound ports. No pure API widening or existing-owner edit is required.

## Scope and media boundary

The positive fixtures are explicitly synthetic. The real Seryeong media inventory remains `source_only` with `0/432` Accepted combinations; this unit does not import, accept, promote, or bind real media. It does not compose a Hub wardrobe screen, alter InputMode, edit scenes/prefabs, choose a storage root, connect a live gameplay renderer, or claim live simulation integration. PPU/internal-render choice is intentionally deferred to the separate real-media/import gate.

The exact additive allowlist is sufficiently narrow: four new Unity runtime paths plus metadata, one isolated PlayMode test assembly and two test files plus metadata, the named CUA evidence files, this contract, and the minimum `docs/README.md` status/link hunk. Existing source/test/asmdef files, catalog rows, scenes, prefabs, real media, Profile/Run, movement/combat/transfer/input/camera, package, and ProjectSetting files remain forbidden. The README's current Review link is consistent with the contract's pre-approval status; it must be updated only as part of Astra's status integration, not treated as an authority override.

## REQ/AC traceability check

| Requirements | Acceptance coverage | Pre-gate result |
|---|---|---|
| `REQ-CUA-001..002` | `AC-CUA-001` | 36 pending rows, `NoAcceptedDefault`, transient highlight only |
| `REQ-CUA-003..004`, `REQ-CUA-008` | `AC-CUA-002`, `AC-CUA-003`, `AC-CUA-008` | package identity/hash/geometry/reference rejection, atomic tuple, synthetic-only truth |
| `REQ-CUA-005` | `AC-CUA-004` | post-commit snapshot port, action/facing/frame mapping, invalid snapshot matrix |
| `REQ-CUA-006` | `AC-CUA-005` | visual-only before/after mechanics and simulation-owner invariants |
| `REQ-CUA-007` | `AC-CUA-006` | exact synchronous CIO correlation, pre-commit/uncertain/post-save failure closure |
| `REQ-CUA-009..010` | `AC-CUA-007` | explicit instance ports, no global lookup/owner mutation, allowlist-only additions |
| All `REQ-CUA-001..010` | `AC-CUA-009` | future focused/full EditMode and PlayMode zero-failure gate plus Luna verification and Astra acceptance |

The requirements are each mapped to at least one acceptance criterion, and the amended P1/P2 semantics are present in both the normative sections and their ACs. No user product choice is hidden in the contract: synthetic fixtures, no live renderer, and deferred real-media import are already selected boundaries.

## Stop conditions

Implementation must stop and return to Astra for an amendment if any of the following occurs:

1. The immutable publication cannot be fully constructed and validated before Save, or publication requires multiple fallible/component assignments.
2. The concrete synchronous CIO Save call cannot receive the exact immutable staged state, or a public/typed revision correlation is unavailable.
3. A positive test requires real media, catalog promotion, a live scene, a live renderer, or a real simulation source.
4. A snapshot cannot be proven to arrive through the explicit instance-bound post-commit observation port, or is decreasing, conflicting, future/uncommitted, or foreign.
5. Integration needs any unnamed existing owner, scene/prefab, InputMode, renderer, persistence-root, or other forbidden path change.
6. Any implementation/test change exceeds the exact allowlist, or later verification cannot prove failed/skipped/inconclusive `0` for `AC-CUA-009`.

The contract's stated PPU/internal-render deferral and real-media block are not approval blockers for this synthetic-only child; crossing either boundary is a separate contract decision.

## Final disposition

**Astra Approved eligibility: YES.** No P0, P1, or P2 remains in this final pre-gate. Astra may set the contract status to `Approved` and authorize Terra to implement the exact additive slice. Terra may not self-accept; Luna must perform the independent implementation/public-surface and focused/full regression review, and Astra retains final integration authority. No implementation, source/test/asset edit, Unity execution, or user product decision was made by this review.

