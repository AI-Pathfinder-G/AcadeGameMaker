# Costume CUA Unity presentation adapter — Luna static pre-execution review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Review type: independent adversarial static review; no Unity execution and no implementation edits
- Approved contract SHA-256: `8F18474B8D966BCD688ABE3DBFB723872D341868FAD552583E430C081C689B1A`
- Contract-gate evidence SHA-256: `AC90107EFB11D75D2D15BEB71906F69A8A80A28B9D246B4596CD5F69B5F9C822`
- Terra implementation evidence SHA-256: `3071602AEF8143946E6D7C650BBD631C1A4F0ADBB1F6C7335F5972D6A0B10EA6`

## Verdict

**BLOCKED — P0=0, P1=4, P2=2.** The additive file boundary and assembly references are plausible, but the implementation is not eligible for the authorized Unity run yet. The blockers are actionable implementation/test gaps, not licensing or product decisions. Terra must correct the P1 items and add the required negative/fault evidence; Luna must re-review the exact replacement hashes before any Unity execution is authorized.

## Reviewed implementation hashes

| Path | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityMediaPackageV1.cs` | `E77192DDF0DD12F758652277AB48DD23F1CA25779890C5357D19DA7840F22865` |
| `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityPresentationAdapterV1.cs` | `AD12C248D4978DDAECE072BB169C68FF407F940CD1CF6C3321DA150265813127` |
| `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityViewPresenterV1.cs` | `B2BB6CFB83EF2812756ED837850C13E6135B0EFD53AE01DAB74FC196580E7587` |
| `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityPresentationAdapterV1Tests.cs` | `668D6C5E1CC6F6E3D5634B985DEFBC0A7283BD22D31117A5DFBAEA4411D2CDAC` |
| `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityViewPresenterV1Tests.cs` | `E9FD6D7DAA4F1E792F3B7D27752DAFE0EC2C09B3D4F9AA664FE9944F98784913` |
| `Assets/AcadeGameMaker/Runtime/Presentation/Unity/AcadeGameMaker.Presentation.Unity.asmdef` | `F4953F55EB97B770FA3DE150D3412F5AAB63B40DF0DA02DCAFAAC1A7742AB70D` |
| `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/AcadeGameMaker.Presentation.Unity.PlayMode.Tests.asmdef` | `65BCEC274D49AE7F3EF00F8161ED2518D6F3236DD7E3B9842936D9D8A3632D86` |

All seven required `.meta` files are present. Their current hashes are recorded in the implementation ledger; no scene, prefab, real media, ProjectSettings, existing owner, or forbidden source path was modified by this review.

## P1 findings — required before execution

### CUA-IMPL-P1-001 — selection accepts an unproven snapshot source

`CostumeUnityPresentationAdapterV1.TrySelect(CostumeUnityMediaPackageV1 media, CompletedActorSnapshotV1 snapshot)` accepts a caller-created value directly. It only checks actor identity and `snapshot.Tick >= LastTick`; it does not require that the snapshot first passed through the explicit `ICostumeUnityCompletedSnapshotPortV1` post-commit sink, nor does it retain a port-issued provenance/commit receipt. A future, uncommitted, or otherwise forged value can therefore seed the first published tuple. The focused test calls `TrySelect` directly with a newly constructed snapshot, so it confirms the gap rather than the contract boundary.

Required correction: make selection consume the adapter's last accepted instance-bound post-commit snapshot (or a non-forgeable port-issued observation token) and reject selection until one exists. Keep `ObserveCompletedSnapshot` as the only source that can advance the snapshot; do not add global discovery or a simulation dependency. Add tests for direct/uncommitted, future, foreign, decreasing, and same-tick-conflicting inputs at the selection boundary.

### CUA-IMPL-P1-002 — the required pure swap gate is never called

The contract and parent spec require a costume swap to validate the current durable/presentation binding and proposed binding through pure `CostumePresentationServiceV1.TrySwap` before publication. `TrySelect` calls `TryCreateBinding` and package validation, then constructs `PublishedCostumePresentationV1` directly; there is no `TrySwap` call anywhere in the adapter. This bypasses the named pure identity/current-clip gate and makes the claimed `REQ-CUA-004/005/007` trace incomplete.

Required correction: add the pure `TrySwap` gate on the initial/transition path as applicable, with an explicit no-current-binding rule for the first synthetic publication if needed. Preserve the existing pure API; do not widen it. Add assertions that a foreign/stale/missing-action current/proposed pair cannot publish and that the completed action/facing/frame is preserved.

### CUA-IMPL-P1-003 — AC-CUA-002 validation matrix is not implemented or tested

The implementation evidence claims complete identity/hash/geometry/reference coverage, but the test file has only six tests. It mutates only `FilterMode` after one successful validation and checks two constructor rejections. There are no independent probes for actor/costume/set IDs, catalog/presentation revisions, each portrait/atlas/clip-map hash, source-manifest bytes, portrait/atlas lifetime, missing required action, missing opposite facing, duplicate/unknown facing, out-of-atlas rectangle, PPU, pivot, baseline, or old-pair preservation after each rejection. `CostumeUnityMediaPackageV1.Validate` also does not independently compare the supplied binding's actor/costume/set IDs, hashes, or completeness; it relies on the caller having just created the binding.

Required correction: implement the full AC-CUA-002 mutation matrix and make package validation itself enforce the pure binding identity/completeness fields before publication. Every negative case must assert no tuple/half-pair publication and preservation of the prior complete pair; reference-destroyed cases must be included.

### CUA-IMPL-P1-004 — atomic-save/projection/snapshot coverage is materially incomplete

The tests cover one first-save success, one pre-commit open fault, one atomic-move uncertainty, and two frame endpoints. They do not cover replacement commit, wrong/stale `ExpectedRevision`, projection failure after durable success, one-reference observation of the complete tuple, direct `ReloadRequired` mutation blocking after projection failure, duplicate/decreasing/conflicting/future snapshots, both facings/action matrix, missing-action swap, or the required idle/locomotion/airborne/landing/bow draw/release/recovery cases. There is also no AC-CUA-005 before/after mechanics/physics-owner invariant probe. Consequently the implementation evidence's “AC-CUA-001..009 implementation coverage” statement is not substantiated.

Required correction: add focused tests for every save outcome and post-save projection fault, assert durable/new tuple authority plus non-authoritative old pixels, assert terminal mutation blocking, and cover the full AC-CUA-004/005 snapshot and visual-only matrix. Only after those tests are present and pass should the focused/full Unity command be considered.

## P2 findings — close during the same correction pass

### CUA-IMPL-P2-001 — rectangle arithmetic can overflow

`Validate` tests `frame.X + frame.Width > Atlas.width` and the analogous Y expression using unchecked `int` addition. A large positive coordinate/width can wrap negative and evade the atlas-boundary check. Replace with subtraction-safe or widened arithmetic and add a near-`Int32.MaxValue` negative case.

### CUA-IMPL-P2-002 — duplicate rectangle intent is not represented

The contract permits duplicate rectangles only when the authored clip intentionally holds a frame. The package model has no explicit hold marker and `Validate` accepts every duplicate rectangle. Either add a bounded, canonical hold declaration to the package/manifest and validate it, or reject duplicate rectangles in this synthetic slice and add the corresponding negative test. Do not silently treat all duplicates as intentional.

## Positive static observations

- The new asmdefs reference only `AcadeGameMaker.Costumes`, `AcadeGameMaker.Costumes.IO`, `AcadeGameMaker.Presentation`, and the new runtime assembly for tests; no forbidden production assembly is referenced.
- The runtime scan found no scene/global lookup, `Update`/`FixedUpdate`, Animator/time source, renderer discovery, Input, mechanics, or persistence delegate. The only save call is the synchronous concrete `_cio.Save(staged)`.
- Byte arrays, clip arrays, and published package fields are defensively copied or read-only at their public seams. The candidate is constructed before Save, and the post-save tuple assignment is separate from the projection call; the projection-failure terminal state is directionally consistent with the contract.
- The 36-row all-pending projection and synthetic-only constructor guard are present, and the exact approved allowlist files/metas exist.

These positives do not waive the P1 findings or constitute compilation/runtime evidence.

## Required stop conditions

Do not run Unity or claim implementation acceptance while any P1/P2 above remains. Stop immediately if correction requires a non-allowlisted owner/scene/prefab/input/renderer/persistence-root change, real media or catalog promotion, a detached save receipt/delegate, a snapshot source outside the instance-bound port, or a fallback that publishes only portrait or gameplay media. After correction, Luna must recheck exact source/test/meta/asmdef hashes, static allowlist, and the complete negative/fault matrix before Terra runs focused tests; AC-CUA-009 still requires focused and full EditMode/PlayMode failed/skipped/inconclusive `0`.

