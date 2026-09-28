# Costume CUA corrected implementation — Luna pre-execution review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Review type: independent adversarial static review; no Unity execution and no implementation edits
- Contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Closure proposal SHA-256: `C3315DE9CD5D6F3D1077B0258FD7C1DADD7BC9CDF5D905EE84EE093317C9F3F1`
- Closure re-gate SHA-256: `A33EC5EC8B4C24CC2A1B0B6101E7BF9B9E9F69F33E416DF8EA1F5B5372850175`

## Verdict

**BLOCKED — P0=0, P1=4, P2=0.** The corrected source removes the public snapshot-selection bypass, calls the pure swap gate, adds binding checks, widened bounds, and duplicate-rectangle rejection. However, the implementation/test delta still does not satisfy the amendment's required pre-execution matrix, and current-tuple corruption handling is incomplete. Unity execution is not authorized.

## Exact hash gate

The four corrected files match the supplied hashes:

| Path | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityMediaPackageV1.cs` | `7AE289742408AD3F6CBC2EFAD9C5D20DB86B68F687B13C1FC6E452B472308D79` |
| `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityPresentationAdapterV1.cs` | `F954AB1830F965481B49C8F4CD26ED5D55FBB84BEEC7A0AE37FF98E60014F25C` |
| `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityPresentationAdapterV1Tests.cs` | `387F780F25892D23C3B1346D6E92215E73073523A5EC49BBC40F70B68E4BFBAB` |
| `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityViewPresenterV1Tests.cs` | `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00` |

The unchanged presenter source, asmdefs, core and CIO also match the required baseline: presenter `B2BB6CFB83EF2812756ED837850C13E6135B0EFD53AE01DAB74FC196580E7587`, runtime asmdef `F4953F55EB97B770FA3DE150D3412F5AAB63B40DF0DA02DCAFAAC1A7742AB70D`, test asmdef `65BCEC274D49AE7F3EF00F8161ED2518D6F3236DD7E3B9842936D9D8A3632D86`, CostumeCore `9149AD9E4D806F02811C0D85F5E1DB044E6380C70B5409C4929CA09913EF8178`, pure presentation `600077CAE9B792451A37CC977FC19632E7AE2309428978CED4552937CB6C27DA`, and CIO `67CCF0DE7E74B3FA9AE6582CCD1AF04D35F61A2135A029D0AA089A1A1B798A8E`. The seven metas remain present and unchanged from the prior gate.

## Confirmed closures

- `TrySelect` now accepts only a media package and obtains snapshots from the explicit `CompletedSnapshots` port; the public `ObserveCompletedSnapshot` method is gone.
- First publication requires a port-observed snapshot and empty durable current ID, then uses the reflexive `TrySwap(proposed, proposed, snapshot, out committed)` bootstrap.
- Replacement calls `TrySwap(current, proposed, snapshot, out committed)` and requires scalar/hash/revision/ordinal-clip equality before constructing the candidate.
- Package validation compares package/binding identity and hashes, exact action set and both facings, uses widened `long` bounds, and rejects repeated rectangles within one clip.
- No forbidden scene/global/time/mechanics lookup or alternate save delegate was found. The only authoritative selection publication is `_published = candidate` after concrete synchronous CIO success; higher-tick observation uses a separate complete-tuple assignment.

These are necessary closures, but they do not by themselves satisfy the required evidence matrix.

## P1 findings

### CUA-IMPL-P1-001 — current tuple corruption is not fully closed

`CurrentTupleValid()` checks snapshot equality, state current ID versus binding costume ID, package validation, and the current action/facing clip. It does not explicitly require the current binding/media/snapshot actor to equal the adapter's `_actorId`, nor does it verify that the stored `PublishedCostumePresentationV1.Frame` equals the exact `FrameFor(_published.Snapshot.ActionAgeQ1000)` result. A corrupted foreign binding or frame can therefore pass the current-tuple predicate far enough to reach `TrySwap`, which returns ordinary `Selection` rejection rather than the amendment's terminal `ReloadRequired` for an untrustworthy current tuple.

Required correction: extend `CurrentTupleValid()` with exact adapter actor, package actor, binding actor and snapshot actor checks, and compare every stored frame field with the current media clip's deterministic frame. Any mismatch must enter `ReloadRequired` before Save or projection. Add reflection/injected corruption tests for actor, binding/media identity, state/current ID, snapshot and frame.

### CUA-IMPL-P1-002 — full package mutation table remains absent

The corrected test file has ten tests, but the amendment requires independent mutations for every identity, revision, binding field/hash, payload hash, canonical clip-map/source-manifest byte rule, required action/facing, reference lifetime, filter/PPU, pivot/baseline, ordinary overrun and exact edge. The current tests cover real pending rows, one filter mutation, constructor geometry/facing exceptions, one duplicate rectangle, and a non-executed `Int32.MaxValue` frame construction. They do not pass the huge frame through `Validate`, do not test each independent mutation, and do not assert no CIO/no projection/reference-preserved old pair for each row.

Required correction: add individually named or parameterized cases exactly matching the proposal table; construct each mutation from a known-valid fixture, invoke `Validate`/selection independently, and assert rejection before Save, exact old `Published` reference, unchanged status/view and no projection. Include maximum-integer right/top and exact-edge acceptance.

### CUA-IMPL-P1-003 — save, projection and terminal fault matrix remains incomplete

The tests cover first-save success, one `FailedBeforeCommit` open fault, and one move uncertainty. They do not cover true `CommittedReplacement`/`Replace`, wrong-primary or same-revision/different-state reopened bytes, post-save projection throw, one-reference observation boundary, old-pixels non-authoritative state, terminal mutation blocking for `TrySelect`, `CompletedSnapshots.Observe` and `Highlight`, or fresh-instance-only recovery semantics. The `Projection` fixture never throws and `MemoryPort` has no wrong-primary mutation path.

Required correction: add two-package first/replacement probes, a concrete CIO port that returns mismatched reopened bytes/wrong revision, a throwing projection, and assertions for every status/tuple/save/projection counter under the amendment's table. Add a save-boundary observation proving only the complete tuple is visible, and assert all terminal no-op methods.

### CUA-IMPL-P1-004 — action/facing/frame and mechanics invariants are not covered

The fixture defines eight action IDs with both facings, but tests exercise only an idle snapshot and the standalone two-frame endpoint helper. There is no per-action/per-facing `0`, interior, `999`, and `1000` matrix; no loop/non-loop interior mapping across the authored actions; and no complete idle/locomotion/airborne/landing/bow draw/release/recovery preservation probe. The required AC-CUA-005 isolated mechanics/physics-owner identity and call-counter probe is absent. The presenter test only checks a few reflection properties and does not prove no mechanics assembly/reference or terminal highlight behavior.

Required correction: add the complete action/facing/frame and monotonic/duplicate/conflict matrix through `CompletedSnapshots.Observe`, then add the isolated Transform/Rigidbody2D/collider/hit-hurt-aim-transfer/stats/cooldown/action-target-phase/AI/call-counter before/after probe required by the proposal. Extend the presenter tests with static/public-surface, no-global/no-mechanics and terminal mutation assertions.

## Compile/static assessment

The corrected C# uses existing public APIs and the unchanged asmdef references; no obvious namespace or signature error was found statically. The explicit interface implementation is callable through `CompletedSnapshots`, the concrete CIO constructor/save surface is unchanged, and Unity object checks are syntactically plausible. This is not a compilation or runtime result. The missing matrix is an acceptance blocker even if compilation succeeds.

## Execution authorization and stop conditions

**Execution authorization: NO.** Terra may continue only within the four corrected source/test files and the already allowlisted evidence files. No Unity run, no `Verified` claim, and no full-suite artifact is authorized until P1-001..004 are corrected, exact replacement hashes are supplied, and Luna reports P0/P1/P2=0. Stop on any allowlist/hash drift, public snapshot bypass, missing pure `TrySwap`, current tuple mismatch that does not close, partial publication, wrong-primary acceptance, projection rollback, mechanics drift, real media/catalog promotion, or any failed/skipped/inconclusive required test.

