# VD-09 M5D7O implementation evidence

- Contract: `docs/specs/work-contracts/2026-09-14-vd09-m5d7o-hub-menu-presentation-controller.md` (Verified)
- Implementer: Terra
- Status: final execution evidence accepted. Luna independently reported PASS
  (`P0=0`, `P1=0`, residual `P2=1`), and Astra advanced the contract to
  `Verified` on 2026-09-20.

## Changed bounded implementation

- `Assets/AcadeGameMaker/Runtime/Input/Unity/HubMenuPresentationControllerV1.cs`
  implements the closed M5D7O presentation vocabulary and controller for
  `REQ-M5D7O-001..008`. It is engine-free and has no scene, router, profile,
  persistence, run, gameplay, narrative, file, network, RNG, logging, timer,
  or menu-effect authority.
- `Assets/AcadeGameMaker/Runtime/Input/Unity/AssemblyInfo.cs` adds only the
  approved friend-assembly line for the new EditMode fixture.
- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/` adds the declared test
  assembly and injected-port fixture. The fixture constructs receipts through
  `ProfileLaunchPreparationCoordinatorV1`'s existing injected ports rather
  than a production test factory.

## AC-M5D7O-006 fixture optimization

- The reflection matrix still mutates every private backing field of
  `HubMenuViewV1`, `ProfileLaunchNotificationV1`, `HubMenuIntentV1`, and the
  controller-owned handoff/notification/view/state/pending-intent/
  consumed-intent fields. This preserves the complete mutation inventory for
  `REQ-M5D7O-007` and `AC-M5D7O-006`.
- View, notification, and intent mutations now invoke the mutated immutable
  value's own next `Validate()` boundary. Controller-owned fields still invoke
  `CurrentView` and assert a `Failed` latch. This is the ownership split
  required by `AC-M5D7O-006`; a nested immutable value does not need repeated
  traversal of the unrelated M5D7N receipt graph to demonstrate its own
  fail-closed validation.
- Controller variants are made with `MemberwiseClone` from one validated
  baseline per state (Ready, IntentPending, IntentConsumed). The controller
  contains only value snapshots, so the clone introduces no new ownership or
  runtime side effect. This removes repeated deep M5D7N construction and
  validation from the field matrix while retaining the controller boundary for
  all controller-owned correlation/state fields.
- No production runtime source or approved-scope authority changed. This
  fixture optimization has static checks only; it supplies no Unity pass
  claim.

## Create-boundary cache correction

- `HubMenuPresentationControllerV1.Create` now performs the complete M5D7N
  handoff and optional typed-notification correlation validation before it
  constructs the controller. It derives the closed persisted-profile predicate
  and optional notification kind at that one incoming boundary, then copies
  both scalar facts and independent proofs into the controller. This preserves
  `REQ-M5D7O-001`, `REQ-M5D7O-002`, `REQ-M5D7O-004`, and `REQ-M5D7O-007`.
- Later `ValidateState`, getters, and commands validate only the M5D7N
  handoff's own O(1) receipt proof plus the controller's exact copied handoff,
  notification, scalar-projection, view, intent, and state proofs. They no
  longer call `LaunchReceipt` getters, `ProfileLaunchNotificationV1.Validate`,
  or `ProfileLaunchNotificationV1.FromReceipt` after `Create`. This matches
  the M5D7N handoff's established defensive-copy model and removes repeated
  traversal of the profile document/preparation graph at every menu boundary.
- The reflection matrix now includes `_persistedValidProfile`,
  `_persistedValidProfileProof`, `_notificationKind`, and
  `_notificationKindProof`. Every controller-owned backing mutation still
  reaches `CurrentView`, throws, and latches `Failed`; direct nested
  `HubMenuViewV1`, `ProfileLaunchNotificationV1`, and `HubMenuIntentV1`
  mutations retain their own immediate validation coverage. This preserves
  `REQ-M5D7O-006`/`REQ-M5D7O-007` and `AC-M5D7O-006` without weakening nested
  immutable validation.
- A wall-clock performance assertion was deliberately not added: Unity batch
  startup, licensing, import, and test-runner scheduling are external to this
  pure controller and make such a threshold non-deterministic. The structural
  boundary above is reviewable in source; the next XML-backed focused run must
  record its actual duration before any performance/pass claim is made.

## Local checks

- `git diff --check` on all M5D7O allowlisted source and test files: PASS.
- Static forbidden-authority scan of `HubMenuPresentationControllerV1.cs` for
  Unity UI/scene/TMP/EventSystem, profile preparation/persistence, IO, router
  map/action ownership, run/gameplay/narrative, Quit, timer, network, RNG and
  logging tokens: PASS. This supports `AC-M5D7O-007` but is not a replacement
  for Luna's independent scope review.
- Unity compiled `AcadeGameMaker.Input.Unity.dll` and
  `AcadeGameMaker.Input.Unity.EditMode.Tests.dll` after the M5D7O additions at
  2026-09-14 08:00 local time; no C# compiler diagnostic was emitted in the
  batch log.
- After the AC-M5D7O-006 fixture optimization, the M5D7O allowlisted test
  source has no stale helper references and passes `git diff --check`.
- After the Create-boundary cache correction, a static source inspection
  confirms `ValidateState` contains no `LaunchReceipt`,
  `ProfileLaunchNotificationV1.FromReceipt`, or
  `_notification.Value.Validate` call. The only retained deep incoming calls
  are in `Create`, as required by `REQ-M5D7O-001`/`007`.

## Focused Unity QA status

The intended command was:

```powershell
powershell -ExecutionPolicy Bypass -File qa/tools/Invoke-UnityQa.ps1 -ProjectPath 'C:\Users\me\Documents\GPT-workspace\AcadeGameMaker' -TestPlatform EditMode -TestFilter 'AcadeGameMaker.Tests.EditMode.InputUnity.HubMenuPresentationControllerV1Tests'
```

- R1 reached the fixture but the first cleanup used delayed destruction for a
  generated Input System asset in EditMode; Unity emitted the expected cleanup
  error and no result XML. The fixture now uses immediate EditMode cleanup.
- R2 is non-final and excluded after coverage was expanded.
- R3 compiled the bounded runtime and test assemblies but remained in Unity
  PerformanceTesting prebuild/asset-import setup without starting the filtered
  tests or producing XML for an extended interval. That process was stopped;
  it supplies no pass/fail evidence.
- R9 (`artifacts/unity-results/m5d7o-r9-cache-focused.xml`, SHA-256
  `977CE6F5884548671678F8BFC532F6452F1F18639A98222551865E4646729A10`,
  companion log `artifacts/unity-results/m5d7o-r9-cache-focused.log`) reached
  the focused fixture after the Create-boundary cache correction in
  `612.3339305 s` and reports `14` total, `13` passed, `1` failed, with no
  skipped or inconclusive case. It is non-final failed evidence and does not
  support any acceptance claim.
- The sole R9 failure was
  `AC004_AllNotificationKindsAndAbsencePreserveExactCorrelation`: the fixture
  incorrectly expected an absent notification for `Default` plus a successful
  bootstrap save. `ProfileLaunchNotificationV1.FromReceipt` correctly maps a
  Default source to `RecoveryCompleted` because it is not Primary. The
  test-only absence row now uses a clean Primary revision `4` paired with a
  committed-first revision `4`, which is the exact no-recovery/no-pending/no-
  preservation-attempt receipt required for an absent notification. Primary,
  Previous, and Default coverage remains present in the focused fixture. No
  runtime behavior changed.
- R10 (`artifacts/unity-results/m5d7o-r10-ac004.xml`, SHA-256
  `4E7686B80E6AF483C1876707F603CCE915ECEF4A5404023CBD59B6A0EACBC7C5`,
  companion log `artifacts/unity-results/m5d7o-r10-ac004.log`) reran only the
  corrected aggregate AC004 method. It reports `1` total, `0` passed, `1`
  failed, `0` skipped, `0` inconclusive, and `317.4307654 s`. The sole result
  is the Unity/NUnit default timeout: `Timeout value of 180000 ms was
  exceeded`. R10 is non-final failed infrastructure/timing evidence, not a
  behavior regression and not an acceptance claim.
- The timeout is not increased. Instead, the former aggregate is split into
  six independent, fresh injected-port tests: clean-Primary absence,
  RecoveryCompleted, PersistenceDeferred, recovery-artifact-preservation
  failure, foreign notification rejection, and missing required notification
  rejection. Each now has its own default timeout and failure attribution; no
  shared mutable or static fixture environment was introduced. The focused
  fixture's expected test-case count becomes `19` (`14 - 1 + 6`). This is a
  test-only scheduling/diagnostic correction preserving `AC-M5D7O-004`.

## Final Unity execution evidence — independently accepted

R9 and R10 above are preserved as superseded/non-acceptance diagnostics. They
do not contribute to the final acceptance set. The following executions used
the final controller and focused-fixture sources:

| Run | Contract AC mapping | Result | Duration | Artifact / SHA-256 |
|---|---|---:|---:|---|
| R12 focused EditMode | `AC-M5D7O-001` through `AC-M5D7O-007`, and the focused-run portion of `AC-M5D7O-008` | 19/19 passed; failed/skipped/inconclusive 0 | 873.3354881 s | `artifacts/unity-results/m5d7o-r12-focused-final.xml` / `86F3BDDE3BE26EFE51E7047A894BB68162AB99550D190C2A1520BD9B73734835` |
| R13 direct M5D7N PlayMode regression | `AC-M5D7O-008` dependency regression | 69/69 passed; failed/skipped/inconclusive 0 | 2613.9075525 s | `artifacts/unity-results/m5d7o-r13-m5d7n-regression.xml` / `D407CCF9F007A3E973ACB8D219026D809E5CFB76B417AFA174B9DB267684BCB5` |
| R14 full EditMode | `AC-M5D7O-008` full EditMode gate | 688/688 passed; failed/skipped/inconclusive 0 | 890.3190608 s | `artifacts/unity-results/m5d7o-r14-full-editmode.xml` / `8EBF5E7CEBA24B4E70BD99470A9AD90554A5401CC65B7351A0C3AB0300F0A292` |
| R15 full PlayMode | `AC-M5D7O-008` full PlayMode gate | 803/803 passed; failed/skipped/inconclusive 0 | 3316.0791578 s | `artifacts/unity-results/m5d7o-r15-full-playmode.xml` / `EEC564A3F860A2FF5BEAD5F3CDF305AD289A73B909FEC51EA89D5722192E359D` |

The final exercised source hashes are:

- `HubMenuPresentationControllerV1.cs`: `F4E9CD4CEEB704CC26869750EF14FEA4A3402055A1F2D1F51BA40F7333CA60C9`
  (`REQ-M5D7O-001` through `REQ-M5D7O-008`).
- `HubMenuPresentationControllerV1Tests.cs`: `218EB71BDD168C59A9DBF03082A37F31741A94AE1A360D62BA4D15C89099FA5F`
  (`AC-M5D7O-001` through `AC-M5D7O-007`).

These executions complete the numerical run prerequisites for
`AC-M5D7O-008` and provide focused evidence for `AC-M5D7O-001` through
`AC-M5D7O-007`. Luna independently recomputed the hashes and counts, accepted
all eight criteria with `P0=0`/`P1=0`, and retained one downstream P2 requiring
M5D7P to preserve typed-notification and receipt-correlation ownership. Astra
accepted that verdict and advanced M5D7O to `Verified` on 2026-09-20.
