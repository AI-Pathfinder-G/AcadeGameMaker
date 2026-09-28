# VD-09 M5D7M pre-execution implementation audit

- Contract: `docs/specs/work-contracts/2026-09-13-vd09-m5d7m-desktop-profile-launch-adapter.md` (Approved)
- Reviewer: Luna (independent; 2026-09-13)
- Scope: static review before Unity execution; no implementation or test edits
- Files reviewed: `DesktopProfileLaunchAdapterV1.cs`, the M5D7M additions in `InputRouter.cs`, `DesktopProfileLaunchAdapterV1Tests.cs`, and the M5D7M implementation evidence

## Verdict

**FAIL pending remediation — P0=0, P1=5, P2=2.** The router reservation shape is directionally correct, but the current implementation is not ready for execution or acceptance. Two state/lifecycle bypasses and incomplete receipt validation are runtime defects; the current three-test fixture does not exercise the contract's required adapter lifecycle. The evidence is explicitly pending Unity rerun, so AC-M5D7M-009 cannot pass yet.

## Findings

### P1-001 — existing test/configuration seams bypass the prepared-launch state machine

`InputRouter.InitializeForTests()` (current line 179) rejects only `_initialized`; it calls `Initialize()` directly while the router is `Reserved` or `Adopted`. That bypasses the sole `Adopted -> Initialized` transition (`InitializePreparedHub(owner)`), does not establish the owner proof, and can leave `_preparedLaunchState == Adopted` while `_initialized == true`. A reserved router instead reaches initialization with no adopted action collection. `ConfigureForAuthoring` and `ConfigureTerminalTeardownForAuthoring` (lines 105–117) likewise reject only initialized/disposed state, so a caller can mutate the graph after reservation/adoption and before initialization.

Required fix: reject configuration whenever a prepared launch is reserved/adopted/initialized/failed (and reject post-Awake configuration as appropriate), and make `InitializeForTests()` reject every prepared-launch state other than legacy `Unreserved`. Add tests that reserve/adopt, invoke each alternate seam, and prove no graph, state, action, callback, or map mutation.

### P1-002 — partial callback registration is not closed exactly once

`RegisterCallbacks()` sets one `_callbacksRegistered` flag only after both `Gameplay.AddCallbacks(this)` and `UI.AddCallbacks(this)` succeed (lines 434–437). If the first add succeeds and the second throws, `CloseActionsOnce()` sees `false` and removes neither callback set. This violates the contract's partial-subscription cleanup and can leave the failed router's callback target attached to a disposed/closed action object.

Required fix: track each callback set independently (or record registration before each fallible add with idempotent removal), and test first-add/second-add failure, original-exception preservation, exact removal count, exact disposal, and later independent-router startup.

### P1-003 — receipt validation is not fail-validating across the copied evidence

`DesktopProfileLaunchReceiptV1.Validate()` (line 147) validates only the document, preservation, save projection, source enum range, final revision/document equality, final outcome lower bound, and final disposition. It does not validate `_hubProof` at all, nor the source-revision/reason relations, initial/final binding failure/disposition relations, or the cross-field binding and save rules that were validated by the consumed M5D7L token. Consequently, reflection mutation of fields such as `_sourceRevision`, `_reasons`, `_initialFailure`, `_initialOutcome`, or `_finalFailure` can remain accepted; notification validation inherits the same gap because it delegates to the receipt.

Required fix: retain or reconstruct a complete immutable internal evidence projection and run the full M5D7L cross-field proof from every receipt/notification getter. Validate the publication proof (`_hubProof`) for the published receipt, and add reflection mutation coverage for every receipt and stored-notification correlation field, including proof and both binding rows. A receipt created as a private pending projection may use a separate internal validation path, but it must not be publishable without the hub proof.

### P1-004 — post-initialization failures cannot satisfy the no-receipt/no-enabled-map guarantee

Adapter `Start()` calls `InitializePreparedHub`, which changes the router to `Initialized`, then performs receipt/notification construction and validation. Its catch still calls `FailPreparedHubLaunch(this)`, but that method accepts only `Reserved` or `Adopted` (lines 150–155). If any proof, receipt, or notification validation fails after router initialization, the failure path cannot close the initialized actions/maps; the exception is swallowed by the catch around `FailPreparedHubLaunch`, leaving a live initialized router with no published receipt. This contradicts the contract's fail-closed publication boundary and AC-M5D7M-004.

Required fix: provide a narrowly scoped owner-only abort/close path for the initialized-but-not-published window, or stage the fallible proof/projection work before the irreversible initialized state. Test injected post-initialization proof/receipt/notification failures and assert no receipt, no notification, both maps disabled, exact closure, and no fallback.

### P1-005 — current tests do not cover the approved AC surface

`DesktopProfileLaunchAdapterV1Tests.cs` contains only three local tests: one positive proof, one proof reflection mutation, and one foreign-owner reservation check. They do not instantiate/configure the adapter, exercise either port, call M5D7L, inspect a receipt or notification, exercise `Awake`/`Start` ordering, verify action identity/disposal/callback closure, cover failure/interleaving matrices, or test legacy fallback suppression. The implementation evidence itself says Unity was not run and no AC is claimed PASS. This is an acceptance blocker for AC-M5D7M-001 through AC-M5D7M-009, not merely a missing convenience test.

Required fix: add the contract's focused PlayMode matrix (including real M5D7L integration through isolated injected roots/UTC, all legal and failure rows, competing active adapters, lifecycle interleavings, receipt/notification field mutation, and bounded completion assertions), then provide focused/direct/full Unity evidence with zero failed/skipped/inconclusive tests.

## Residual P2 findings

### P2-001 — adapter cleanup may replace an earlier exception

The adapter `Awake()` catch calls `prepared.Dispose()` without a cleanup guard (line 57). An injected token whose disposer throws can replace the original preparation/validation exception. M5D7L's own coordinator protects its failure cleanup, but the adapter boundary should preserve the original cause and attempt token cleanup exactly once.

### P2-002 — cohort proof is implicit rather than independently observable

`ValidateCohort()` checks active/enabled state, `-220`/`-210` execution orders, and router `_awakeEntered`, which is plausible for Unity scene startup. The implementation has no typed cohort token or test seam proving both objects entered the same lifecycle cohort; focused tests should cover late activation, inactive/disabled components, and cross-cohort activation explicitly. This remains a precision risk rather than a current P1 because the execution-order and `_awakeEntered` gate reject the principal late-router cases.

## AC status at this pre-execution point

| Criterion | Independent status | Reason |
|---|---|---|
| AC-M5D7M-001 | **BLOCKED** | No adapter port-order/integration test; evidence pending |
| AC-M5D7M-002 | **BLOCKED** | No M5D7L outcome-row lifecycle tests |
| AC-M5D7M-003 | **FAIL** | Partial callback cleanup and incomplete receipt proof |
| AC-M5D7M-004 | **FAIL** | Initialized post-publication failure cannot close |
| AC-M5D7M-005 | **FAIL** | Alternate router seams bypass reservation state |
| AC-M5D7M-006 | **BLOCKED** | No receipt/notification correlation or one-shot tests |
| AC-M5D7M-007 | **BLOCKED** | No active same-cohort lifecycle execution test |
| AC-M5D7M-008 | **BLOCKED** | Regression/static checks not supplied in current evidence |
| AC-M5D7M-009 | **BLOCKED** | Unity has not been run; no focused/direct/full results |

No P0 was found. Astra should not accept the implementation until P1-001 through P1-005 are addressed and independently exercised; this report does not change the Approved contract status.
