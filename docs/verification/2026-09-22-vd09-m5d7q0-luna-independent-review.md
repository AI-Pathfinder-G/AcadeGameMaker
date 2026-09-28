# VD-09 M5D7Q0 Hub-UIOnly router graph — Luna bounded independent review

- Review date: 2026-09-22
- Reviewer: Luna (`gpt-5.6-luna`)
- Contract: `docs/specs/work-contracts/2026-09-20-vd09-m5d7q0-hub-ui-only-router-graph.md`
- Review scope: approved allowlist, current runtime/editor/test sources, canonical prefab and metadata, Q0 scope-audit implementation, M5D7M compatibility repair, and locally retained XML/log evidence.
- Review mode: static/evidence review only. No Unity Editor, licensing probe, or test runner was launched, per the bounded review instruction.
- Result: **CONDITIONAL — P0=0, P1=2, P2=0. Do not mark M5D7Q0 Verified yet.**

## Findings

### P1-01 — AC-M5D7Q0-011 does not establish the required before/after scope boundary

`HubUiOnlyQ0ScopeAuditEditModeTests.AC011_ActualRepositoryManifestClassifiesTheExactQ0DeltaWithoutFolderInference` enumerates Git's current changed/untracked paths, labels any path outside `DeclaredQ0Paths` as `PREEXISTING_DECLARED_EXTERNALLY_OR_UNATTRIBUTABLE`, and then asserts only that required Q0 paths appear. It does not compare those non-Q0 paths to a captured pre-implementation inventory, and it does not fail when an unexplained path appears. The metadata test likewise filters to paths already declared Q0 before checking `AllowedMeta`.

The Terra evidence explicitly says the initial worktree was already dirty and that a cryptographically authoritative pre-Q0 baseline cannot be reconstructed. This is an honest limitation, but it means AC-011's promised before/after “only allowlisted files” result is not proven. The present large dirty/untracked inventory cannot be attributed to Q0 or excluded as pre-existing from this workspace alone. This is an evidence/gate limitation, not proof that Q0 introduced any particular out-of-scope file.

The scope-audit code's `AllowedDocumentation` list also omits the post-review document explicitly permitted by the contract. Consequently, when this required Luna report is added to the actual Git inventory, the audit will classify it as external/unattributable rather than as an allowed Q0 evidence path.

**Required disposition:** keep AC-011 open. Do not describe the audit as proving that only allowlisted files changed. A future acceptance record needs a trustworthy captured baseline or another verifiable provenance boundary; retroactively relabeling the current non-Q0 inventory is insufficient.

### P1-02 — Claimed final test evidence cannot be reconciled to retained artifacts

All 16 source-file SHA-256 rows in the Terra manifest match the current files, and the canonical prefab and its three named metadata hashes also match the evidence. However, the reported final test-result hashes do not match the retained result artifacts found in the workspace:

- The retained builder XML `artifacts/m5d7q0-focused-editmode-r6.xml` is a valid 29/29 pass with zero failures/skips/inconclusives, but its hash is `466A1634256FA541938292EF82E372FED5E02A102193571650FF82CF2BB76E6A`, not the evidence's `66AA3508…`; its retained log hash is `77EC0FB3…`, not `0CF8FC5A…`. Its XML run time is 2026-09-20, not the evidence's stated 2026-09-22 run.
- The evidence's scope-audit result/log hashes (`7E2106EB…` / `290BD64D…`) were not found among retained `artifacts/`, `qa/results/`, or `qa/reviews/` XML/log files.
- The evidence's post-repair M5D7M result/log hashes (`EE24DE4B…` / `E427FBAC…`) were also not found. Retained similarly named M5D7M XML is dated 2026-09-13 and is not evidence for the later repair.
- Available Q0 PlayMode XML is dated 2026-09-20, before the documented M5D7M compatibility repair. Router `38/38`, handoff `5/5`, and remaining-runtime `18/18` are clean historical results, but cannot verify the current post-repair source. The available Coverage-B XML runs failed (`6` failures in r1; `4` in r2); later claimed 27/27 evidence hashes are not retained.

The current source hashes are internally consistent with the manifest, but result-hash provenance and the post-repair runtime state are not independently reproducible from retained artifacts. No Luna execution result exists for this review.

**Required disposition:** rerun the affected focused/direct/full gates with the current source, retain each exact XML and log at the paths written into the new evidence record, hash those retained files, and have Luna independently verify the same artifacts. Existing failed diagnostic runs remain failures and must not be counted as passes.

## Static review results

- **AC-M5D7Q0-001 — static acceptable, execution pending.** `_authoredHubUiOnly` defaults to `false`; the HubUIOnly path is explicitly selected and the legacy path remains the default. The M5D7M repair is narrowly scoped in the current source: disposed legacy containment runs through `ValidatePreparedInvariantOrClose()`, while the synthetic UI-unsubscribe witness is limited to the legacy `BeforeUiCallbackRegistration` test injection and is explicitly disabled for HubUIOnly. No M5D7M state enum/schema expansion was found. Current M5D7M runtime pass evidence is not retained/reconciled (P1-02).
- **AC-M5D7Q0-002 — execution pending.** Prepared owner/action lifecycle and close ownership are represented in code; boundary injection remains to be rerun against the current source.
- **AC-M5D7Q0-003 — static acceptable, execution pending.** The hub transaction builds/validates its candidate frame and checked successors before consuming the pending UI batch; clock/proof assignment follows; receipt publication is last.
- **AC-M5D7Q0-004 — static acceptable.** The branch does not enter gameplay candidate/consumer code. Runtime search found exactly one `InputRouter` implementing `IUiSemanticFrameSourceV1`, `GameInputActions.IGameplayActions`, and `GameInputActions.IUIActions`; no `InputSystemUIInputModule`, `Mouse.current`, or second `InputRouter` construction was found in the input runtime subtree. The prefab has no gameplay/camera references. Runtime-spy coverage remains execution-pending.
- **AC-M5D7Q0-005 — static acceptable, execution pending.** Hub requester and mode APIs reject via the hub discriminator before player access; command rejection behavior and no-mutation assertions require current PlayMode verification.
- **AC-M5D7Q0-006 — execution pending.** M5D7N notification/handoff integration is not currently proven by a retained current run.
- **AC-M5D7Q0-007 — static acceptable, execution pending.** Exact hub lifecycle/publication checks and terminal containment are present; corruption and overflow fixtures need current PlayMode reruns.
- **AC-M5D7Q0-008 — static acceptable, execution pending.** Current prefab SHA is `F5B0B347AFFF52821594B449B4184464C9DFE426ADABEF2D42B5F73C7CEC5720`; the metadata hashes match Terra's manifest. YAML has one active `HubRuntimeRoot`, no children, and exactly `Transform`, HubUIOnly `InputRouter`, sibling-bound `DesktopProfileLaunchAdapterV1`, and sibling-bound `HubEntryHandoffLatchV1`. The retained 29/29 builder result is pre-repair and its hash differs from the reported final artifact, so current deterministic/idempotence acceptance remains pending.
- **AC-M5D7Q0-009 — execution pending.** Execution-order attributes are adapter `-220`, router `-210`, latch `-190`; static ordering is correct, but cohort permutations, Start order, teardown, and reload need current runtime verification.
- **AC-M5D7Q0-010 — execution pending.** The required current focused/direct/full regression matrix has not been completed with verifiable retained artifacts. M5D7N and full EditMode/PlayMode are explicitly blocked by batch LicenseClient pipe failures; no Unity retries were performed for this review.
- **AC-M5D7Q0-011 — P1 finding above.** Source hashes and exact current named Q0 paths are reproducible, but the test's automatic “pre-existing/unattributable” label is not a baseline comparison and cannot prove the approved before/after scope.
- **AC-M5D7Q0-012 — static acceptable, execution pending.** The implementation preserves adopted action ownership on post-publication fault, immediately invalidates reads/disables maps through fault containment, and defers exact-once closure to teardown. Current fault-injection PlayMode evidence must be rerun independently.

No `Assert.Ignore`, `[Explicit]`, or `Assert.Inconclusive` suppression was found in the new Q0 test sources inspected. Because the relevant project files are untracked and the pre-Q0 worktree baseline is unavailable, this review cannot make a before/after assertion that existing tests or assertions were not weakened; the M5D7M regression result must be checked from a retained current XML/log pair.

## Exact remaining verification gates after batch licensing works

Run serially against the current source; preserve a unique XML and log for every invocation. Use the Editor's existing batch test-run command shape (`-accept-apiupdate -batchmode -nographics -projectPath <project> -runTests -testPlatform <EditMode|PlayMode> -testFilter <fully-qualified-test-type> -testResults <unique.xml> -logFile <unique.log>`). Do not treat a license-pipe failure before discovery as a test result.

1. Luna independently runs EditMode filters:
   - `AcadeGameMaker.Tests.EditMode.InputUnity.HubRuntimeAuthoringBuilderEditModeTests`
   - `AcadeGameMaker.Tests.EditMode.InputUnity.HubUiOnlyQ0ScopeAuditEditModeTests` (record AC-011 as blocked unless the baseline/provenance defect is resolved)
2. Luna independently runs Q0 PlayMode filters:
   - `AcadeGameMaker.Input.Unity.PlayMode.Tests.HubUiOnlyInputRouterPlayModeTests`
   - `AcadeGameMaker.Input.Unity.PlayMode.Tests.HubUiOnlyQ0CoverageBPlayModeTests`
   - `AcadeGameMaker.Input.Unity.PlayMode.Tests.HubUiOnlyQ0HandoffMatrixPlayModeTests`
   - `AcadeGameMaker.Input.Unity.PlayMode.Tests.HubUiOnlyQ0RemainingRuntimePlayModeTests`
3. Luna reruns direct regressions required by the contract:
   - M5B5: `AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests`
   - M5D7M: `AcadeGameMaker.Input.Unity.PlayMode.Tests.DesktopProfileLaunchAdapterV1Tests` (117 current cases)
   - M5D7N: `AcadeGameMaker.Input.Unity.PlayMode.Tests.HubEntryHandoffLatchV1Tests` (69 current cases)
   - M5D7P-A: `AcadeGameMaker.Tests.PlayMode.InputUnity.UiSemanticFrameRouterPlayModeTests`
4. Terra runs the full EditMode suite, then full PlayMode suite; both must report zero failure, skip, and inconclusive. Luna independently verifies the retained XML/log counts and hashes. Do not overlap Unity invocations against this project.
5. Astra resolves P1-01/P1-02, reviews Luna's AC map and retained evidence, and alone decides whether to mark the contract `Verified`.

## Evidence integrity digest

- Terra's 16-row Q0 source manifest: all 16 current SHA-256 values match.
- Canonical prefab and metadata: prefab, prefab `.meta`, `Assets/Prefabs/Hub.meta`, and `Assets/AcadeGameMaker/Editor/HubAuthoring.meta` match the reported SHA-256 values.
- Locally retained current-looking historical evidence: Q0 builder r6 `29/29` (hash mismatch vs final evidence claim); router r3 `38/38`; handoff r1 `5/5`; remaining-runtime r2 `18/18`; Coverage-B r1 and r2 failed; all Q0 PlayMode outputs predate the M5D7M repair.
- Final Q0 scope-audit and post-repair M5D7M exact artifacts: not found by their reported SHA-256 in retained `artifacts/`, `qa/results/`, or `qa/reviews/` files.
- No implementation files were modified by this review.
