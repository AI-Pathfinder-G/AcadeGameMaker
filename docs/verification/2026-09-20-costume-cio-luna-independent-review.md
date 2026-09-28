# Costume CIO file adapter — Luna independent review

- Date: 2026-09-20
- Current status: **FOCUSED UNITY CIO TESTS PASS 9/9; AC-CIO-007 NOT SATISFIED — full EditMode/PlayMode suites hung; P0=0, P1=1, P2=0**
- Reviewer: Luna (independent post-implementation review)
- Contract: [Costume CIO file adapter](../specs/work-contracts/2026-09-20-costume-cio-file-adapter.md)
- Implementation evidence: [Terra CIO implementation evidence](./2026-09-20-costume-cio-implementation-evidence.md)
- Initial review scope: static/adversarial source and test review, allowlist/hash inspection, and engine-free Windows/.NET path-behavior probe. No runtime or test source was edited. At that initial review stage no Unity process was started because licensing was unavailable; the execution follow-up below records the later licensed Unity runs.

## Initial review (historical; superseded by final re-review below)

No P0 finding. Three P1 findings prevent acceptance; two P2 evidence/forensics clarifications remain. The source has a plausible ordered save path, exact internal role-name derivation, defensive copies, and a first-valid recovery selector handoff, but the current tests do not establish the contract's required transaction/fault matrix. This review is not an Astra acceptance decision.

Unity-focused, full EditMode, and full PlayMode results are **unexecuted**, not passing. `AC-CIO-007` therefore remains blocked regardless of the static findings below.

## Findings

### P1 — drive-relative Windows paths pass the absolute-path guard

`CostumeFileAdapterV1.NormalizeDirectory` checks `Path.IsPathRooted(directory)` before `Path.GetFullPath` (`CostumeFileAdapterV1.cs`, `NormalizeDirectory`). On Windows, `Path.IsPathRooted("C:relative")` is `true`, even though `C:relative` is drive-relative rather than fully qualified; `Path.GetFullPath` resolves it using that drive's current directory. The engine-free probe on this host reproduced `rooted=True` and a full path under the current working directory. The constructor therefore accepts an input that the contract says to reject as relative, and can bind persistence to a caller-unintended directory.

**Trace:** `REQ-CIO-001`, `AC-CIO-001`.

**Action:** reject drive-relative forms before normalization (or use a platform-supported fully-qualified-path predicate and retain the explicit UNC/root checks); add a test for `C:relative` and analogous drive-relative forms. Do not infer that `Path.IsPathRooted` means fully qualified on Windows.

### P1 — owned-role inspection exceptions escape the typed pre-commit result

`Save` invokes `_port.Inspect` for primary, previous, and temp outside any exception boundary (`CostumeFileAdapterV1.cs`, immediately before the temp-write stage). The live port maps common IO/access failures to `ReadFailed`, but does not catch every expected path-validation exception (`ArgumentException` and `NotSupportedException` are notably absent there); the public port seam may also surface the filesystem exceptions directly. Such a failure escapes `Save` instead of returning `FailedBeforeCommit` at `Validation`, contrary to the typed failure contract. There is no injected inspection-failure test.

**Trace:** `REQ-CIO-004/005`, `AC-CIO-003/004`.

**Action:** contain the approved expected path/IO exceptions around role inspection and return `FailedBeforeCommit/Validation`; add faults for each inspection and assert that no temp open or atomic operation occurs. Preserve programmer-fault exceptions rather than broadly swallowing all exceptions.

### P1 — transaction ordering and failure-boundary acceptance tests are incomplete

`AC-CIO-003` requires a trace proving the exact write → managed flush → durable flush → close → atomic operation → reopen → byte equality → decode sequence. The current success test only exercises a real first save and replacement, reads/decodes the resulting primary, and checks the backup exists. The fake port has no operation trace and its move path substitutes empty primary bytes, so it cannot test a successful adapter commit or assert ordering.

`AC-CIO-004` requires a fault at every stage. The current loop covers write, managed flush, durable flush, close, and move only. It does not exercise replace failure, any inspect failure, primary reopen failure, unequal bytes, primary decode/revision failure, or assert the corresponding `Stage`; the fake's `replace` fault is never selected by a test. Thus the evidence's “fault injected at every stage” statement exceeds the test code present.

**Trace:** `REQ-CIO-004/005`, `AC-CIO-003/004`.

**Action:** add a recording port/session whose successful first-save and replacement traces assert exact order and paths, then inject each contract stage failure and assert outcome, stage, atomic-call count, and no retry/cleanup. Keep actual NTFS smoke coverage separate from the deterministic fake trace.

## P2 findings

1. **Rejected load fabricates per-role read failures.** `RejectedLoad()` returns three `ReadFailed` observations when `PathsAreSafe()` rejected the root before any role was opened. This conflates “not observed” with a real IO read failure and can mislead diagnostics. Consider an explicit `NotObserved` disposition or a result shape that distinguishes path rejection from role observations. Trace: `REQ-CIO-002/003`, `AC-CIO-002`.
2. **AC-CIO-005 fixture/assertion is narrower than its wording.** The current all-pending test uses missing primary/temp plus corrupt previous; it does not exercise a valid canonical empty state and does not assert empty unlocked/current collections. Add that case and assert `NoAcceptedDefault`, no binding, no fabricated unlock/current selection, and zero writes. Trace: `REQ-CIO-006`, `AC-CIO-005`.

## Acceptance-criterion disposition

| Criterion | Luna disposition | Evidence and remaining gate |
|---|---|---|
| `AC-CIO-001` | **FAIL / P1** | Role paths are derived from the three constants and fake load reads are under the root, but drive-relative input is accepted. The fake records `ReadOnce` paths only; it does not assert inspect/write/reopen path arguments. Unity test execution is unavailable. |
| `AC-CIO-002` | **STATIC PASS WITH TEST GAP** | Source retains typed dispositions, clones byte arrays, reads each role once, and guards unread candidates according to the selector's primary→previous→temp first-valid precedence. Tests cover representative cases but not the full disposition/permutation matrix; Unity execution is unexecuted. |
| `AC-CIO-003` | **INCONCLUSIVE / P1** | Source statement order is consistent with the intended transaction, but the tests do not record/prove the required complete stage trace. |
| `AC-CIO-004` | **FAIL / P1** | Fault matrix omits inspect, replace, reopen, equality, and decode failures and does not assert exact returned stage for the tested faults. |
| `AC-CIO-005` | **PARTIAL / P2** | The missing+corrupt all-pending case returns read-only `NoAcceptedDefault`; valid-empty and explicit no-fabrication assertions are absent. No Unity execution. |
| `AC-CIO-006` | **STATIC PASS** | Reviewed runtime source has no Unity/Profile/Run, delete/quarantine, clock/RNG, or network dependency. This is a source inspection only; test execution is unexecuted. |
| `AC-CIO-007` | **BLOCKED — NOT RUN** | Unity licensing is currently refusing runs. Focused/full EditMode and full PlayMode must later report zero failed, skipped, and inconclusive tests, followed by independent evidence refresh. No Unity attempt was made in this review. |

## Initial-review worktree and evidence integrity

- `HEAD` remains `309f2204cf19a321ae74c92f3be0e3fc94e3499e`.
- The implementation evidence's recorded hashes for both pure-core files and its five CIO implementation files match the current files. The pure-core source/test hashes therefore remain unchanged from Terra's stated baseline.
- Terra recorded the pre-implementation default `git status --porcelain=v1` as 358 entries with SHA-256 `065ACBF196D44865C123DBDAEC46E6E68A4D60171872B007C4548786D262FFB6`. At review start, before this review file and its one README status/link hunk were added, the default status was 361 entries with SHA-256 `D21826C6983EBFCB4F6B980DC21538E7E083319149E00567EDBD801CEB50DD39`; the shared worktree remains broadly dirty, so aggregate status does not attribute the delta.
- `docs/README.md` was already dirty in Terra's baseline, but its recorded baseline SHA-256 (`66BF4809C06198046745AAAB4097398C7EEDF170A468F36E938B57B71B6488D8`) differs from its review-start SHA-256 (`93C16468B2F65FFA562ECDA9725B7CF8D050441DF5F1B2D14464FC3C2DB12665`). This review then added one CIO review-link/status hunk. A path-level README diff to the pre-implementation content cannot be reconstructed from the hash alone; do not claim that its entire present diff is the CIO-related hunk. No unrelated dirty material was reset or rewritten.

## Initial-review method and participation

Static inspection covered the complete allowlisted runtime adapter and EditMode tests, both asmdefs, the approved CIO child contract, Luna's pre-gate, Terra's implementation evidence, and the pure recovery selector. The deterministic .NET probe confirmed the Windows drive-relative path behavior described in P1. The Unity test suite was not run because of the active licensing refusal; no alternate or repeated Unity launch was attempted. No runtime or test file was changed by Luna. Astra owns disposition of these findings and any subsequent acceptance.

## Final re-review — 2026-09-20

**Disposition before Unity execution:** no remaining source-level P0/P1/P2 findings in the static/adversarial-probe scope. The initial three P1 and two P2 findings are closed by the bounded Terra changes; the subsequently identified custom-port `NotObserved`/default-result protocol gap is also closed. Unity execution has since occurred; the focused CIO suite passes, while the required full suites did not complete. See the post-run follow-up below. This is not full acceptance.

### Closure and independent probe

- Drive-relative roots (`C:relative`, `C:.`, `C:folder\\child`) are explicitly rejected before port access; the adversarial test asserts this.
- Expected owned-role inspection faults now return typed `FailedBeforeCommit/Validation`; the transaction test covers all three inspection positions.
- Recording-port tests assert first-save and replacement transaction traces and the full pre-/post-commit fault matrix, including exact stage, no retry/cleanup, and owned paths.
- Path rejection now reports three `NotObserved` observations; valid canonical empty state under an all-pending catalog is tested as read-only `NoAcceptedDefault` with no binding or fabricated selection.
- A port result can be constructed only as `Missing`, `Read`, or `ReadFailed`. `Observe` fails closed for default, invalid, or corrupted `NotObserved` raw results. The added adversarial test checks those cases.
- Independent engine-free compile/probe of the final pure core and adapter used a custom port returning `default(CostumeFilePortReadResultV1)`. It returned `ReadFailure`, `HasRecoveryPlan=false`, `Primary=ReadFailed`, after three one-time role reads and zero writes. The public port-result constructor rejected `NotObserved`; `C:relative` was rejected by the adapter constructor. No repository files were created by the probe.
- Static inspection confirms the test transaction observer is internal and `InternalsVisibleTo` names only the exact CIO EditMode test assembly. No Unity execution is inferred from compile/probe results.

### Final AC disposition

| Criterion | Luna re-review disposition | Evidence and remaining gate |
|---|---|---|
| `AC-CIO-001` | **STATIC/PROBE PASS** | Drive-relative and invalid-root guards, exact owned-role derivation, recorded owned paths, and adversarial tests reviewed. Unity execution remains deferred. |
| `AC-CIO-002` | **STATIC/PROBE PASS** | Precedence/unread-authority tests plus constructor and malformed custom-port fail-closed coverage reviewed; independent default-result probe cannot bootstrap or publish a plan. Unity execution remains deferred. |
| `AC-CIO-003` | **STATIC PASS** | First-save/replacement traces assert the specified transaction order and exact owned paths. Unity execution remains deferred. |
| `AC-CIO-004` | **STATIC PASS** | Full injected-fault test matrix and typed outcomes/stages reviewed; tests have not been executed in Unity. |
| `AC-CIO-005` | **STATIC PASS** | Missing/corrupt and canonical-empty all-pending assertions cover read-only `NoAcceptedDefault`, no binding/selection, and zero writes. Unity execution remains deferred. |
| `AC-CIO-006` | **STATIC PASS** | Source and allowlist review found no Unity/Profile/Run authority, deletion/quarantine, root discovery, mutable static port, clock/RNG, or network dependency. |
| `AC-CIO-007` | **INCOMPLETE — P1 GATE** | Focused CIO EditMode is 9/9 pass with failed/skipped/inconclusive all zero. Full EditMode and full PlayMode were terminated after hangs and have no passing result; the required complete suite set remains outstanding. |

### Final implementation/evidence hashes

Current hashes were independently recalculated after Terra reported the remediation complete and match the refreshed implementation evidence. The accepted pure-core files remain unchanged.

| Path | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Costumes/IO/AcadeGameMaker.Costumes.IO.asmdef` | `920DD615F12F0086D978BB1E990B480ADE833F06491B0A2A58B4B41099AC3449` |
| `Assets/AcadeGameMaker/Runtime/Costumes/IO/AcadeGameMaker.Costumes.IO.asmdef.meta` | `6E5314FCFB71EFC4FB9CB2FC843D48AC340E1DBB7695F199CE45B64D5C5AE482` |
| `Assets/AcadeGameMaker/Runtime/Costumes/IO/CostumeFileAdapterV1.cs` | `67CCF0DE7E74B3FA9AE6582CCD1AF04D35F61A2135A029D0AA089A1A1B798A8E` |
| `Assets/AcadeGameMaker/Runtime/Costumes/IO/CostumeFileAdapterV1.cs.meta` | `22279AC6745358AE14B54290C35AA67883872CD9BE9DE38C01CA0873BA223B99` |
| `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/AcadeGameMaker.Costumes.IO.EditMode.Tests.asmdef` | `939382127FA662D3CF4B93F08C02EABF03061884FC6DC7925A0B3A3D0C05309D` |
| `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/AcadeGameMaker.Costumes.IO.EditMode.Tests.asmdef.meta` | `24C43D606721B56A2533019C434E76462132267548063A033AE3816843F980D5` |
| `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/CostumeFileAdapterV1Tests.cs` | `6A939599F0D55B03B4CBDCB939283BEB5F1B36506E815662CB125B9CD5622C23` |
| `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/CostumeFileAdapterV1Tests.cs.meta` | `31C4F428C458879BDF16792AB5D1B5FF668DC436AF8CF1413D3A92945947BC53` |
| `Assets/AcadeGameMaker/Runtime/Costumes/CostumeCoreV1.cs` | `9149AD9E4D806F02811C0D85F5E1DB044E6380C70B5409C4929CA09913EF8178` |
| `Assets/AcadeGameMaker/Runtime/Costumes/AcadeGameMaker.Costumes.asmdef` | `DF21E38D10C384F4FCF24A14EC379DD75BFB4C4ACBAE4A4195DB199785359EC2` |
| `Assets/AcadeGameMaker/Runtime/Costumes/AcadeGameMaker.Costumes.asmdef.meta` | `870EF3EEF26C4F71D4DEB320EC8561A9E8AD3C1F20219A83593B62C81AE5589B` |
| `Assets/AcadeGameMaker/Tests/EditMode/Costumes/CostumeCoreV1Tests.cs` | `53591F88A2A1CFF75D6F29D8CDDAF9A592340BC4723EFCCFBB5989B645F880B1` |

The shared worktree remains broadly dirty. Immediately before this final review edit, `git status --porcelain=v1` contained 363 entries; the newline-joined status-manifest SHA-256 was `25B054D67664DAF3FFD8221642E83866BDCF74F405046CAA32374E5053BBCC5C`. This snapshot is attribution context, not a claim that every dirty entry belongs to CIO. Luna changed no runtime or test file; only this review and the single CIO README status phrase are updated by this re-review.

## Unity execution follow-up — 2026-09-20

**Current disposition:** `P0=0`, `P1=1` (AC-CIO-007 incomplete), `P2=0`. Terra has now reconciled the implementation evidence with the focused result and both terminated full-suite attempts. The focused test artifact is independently readable and valid; the full-suite hangs are not evidence of a CIO defect, but neither can they be called unrelated without a completed run or a clean-baseline comparison.

- `TestResults-CIO-Focused-Edit-20260920.xml` SHA-256: `29AA096D9F47E162EB57DC8DC68F7AA771B0A5C60659E703FC432B8B29662595`. The XML reports `Passed`, total `9`, passed `9`, failed/skipped/inconclusive `0`. It contains the CIO AC-CIO-001..006 cases, including the local NTFS smoke, transaction trace, and full fault matrix.
- The current test asmdef SHA-256 is `939382127FA662D3CF4B93F08C02EABF03061884FC6DC7925A0B3A3D0C05309D`; its only direct references are `AcadeGameMaker.Costumes.IO` and `AcadeGameMaker.Costumes`, with `Editor`-only and `noEngineReferences`. Runtime adapter hash remains `67CCF0DE7E74B3FA9AE6582CCD1AF04D35F61A2135A029D0AA089A1A1B798A8E`; test source hash remains `6A939599F0D55B03B4CBDCB939283BEB5F1B36506E815662CB125B9CD5622C23`.
- `Unity-CIO-Full-Edit-20260920.log` shows an unfiltered EditMode run (`assemblyNames=null`) reached test execution and repeatedly imported temporary scenes from existing `CombatObservationBuilderTests`, `CombatTargetAuthoringTests`, `OrdanBossCombatBridgeTests`, and `OrdanBossTerminalTeardownContractTests`. Its last named fixture activity is another import of `__TempOrdanBossTerminalScene.unity` at log line 1303, followed by Unity cleanup/unused-scene messages. The editor log contains no per-test start/finish events and no corresponding full-suite XML was produced, so this is the last observable fixture boundary, not proof that a particular NUnit case hung there. In the successful 2026-09-12 full EditMode XML, `CombatObservationBuilderTests` passed 3/3 in 0.434 s and `OrdanBossTerminalTeardownContractTests` passed 12/12 in 1.380 s. These scene-writing fixtures are pre-existing and unchanged in the current worktree.
- `Unity-CIO-Full-Play-20260920.log` ends at `RegularEnemyThreatSimulationDriverPlayModeTests.AC_COM_003_UnitySanitizedOrPreventedAuthoringIsNotAThreatDriverRejection(WalkerLinearVelocityNaN)`, with the expected invalid `Rigidbody2D.linearVelocity` assignment from source line 932. The test explicitly registers a matching `LogAssert.Expect(LogType.Error, ...)` at line 1124, then checks Unity sanitized the value and continues; the same seven-case method passed 7/7 in the successful 2026-09-12 full PlayMode XML. Thus the NaN trace is a deliberate negative test and not, by itself, a failure or hang. Because the log has no result event/XML, it does not prove whether that case or what followed was where progress stopped.
- Both logs report the initial access-token refresh warning but later report successful entitlement resolution and begin the requested test run; the evidence does not point to licensing as the hang boundary. For this regression delta, the CIO runtime adapter and test source are unchanged; the latest correction is test-only and adds a direct reference to the existing pure-core assembly. The two candidate areas above passed in a prior full-suite baseline. A test-assembly discovery or runner interaction from the new asmdef is theoretically possible, but currently has no supporting signal; the most likely class is a full-suite/Editor harness or order-dependent existing test interaction, with exact cause unproven.
- `AC-CIO-007` requires focused **and full** EditMode plus full PlayMode to complete with failed/skipped/inconclusive `0`. Thus the focused pass closes only its scoped executable checks; the criterion remains **not satisfied** until complete full-suite results exist.
- The implementation evidence now records the focused 9/9 result, both terminated full-suite attempts, and unresolved attribution. Its current SHA-256 is `368D5838E06413FDE98163CF63B2A045C2E34D68728F12C78D916A79E77D4A14`. This closes the review's former P2 evidence mismatch; it does not satisfy AC-CIO-007.

**Smallest controlled isolation sequence (not run by Luna):**

1. Run EditMode with `-testFilter "AcadeGameMaker.Tests.EditMode.CombatUnity.OrdanBossTerminalTeardownContractTests"` and a fresh `-testResults`/`-logFile` path; expected historical scope is 12 cases. If it stalls, bisect that fixture's test names from the prior successful XML. If it passes, run `CombatObservationBuilderTests` (3 cases), then `CombatTargetAuthoringTests`, then `OrdanBossCombatBridgeTests`, each as a separate process/run with fresh output paths.
2. Run PlayMode with `-testFilter "AcadeGameMaker.Tests.PlayMode.CombatUnity.RegularEnemyThreatSimulationDriverPlayModeTests.AC_COM_003_UnitySanitizedOrPreventedAuthoringIsNotAThreatDriverRejection(WalkerLinearVelocityNaN)"`; if it passes, run the full seven-case method, then the whole `RegularEnemyThreatSimulationDriverPlayModeTests` fixture.
3. Only after the focused filters finish, retry full EditMode and full PlayMode serially, each with a new result XML and log. Use the same Unity version and licensed project state, and keep the Editor closed while batch runs execute. Do not treat either filtered pass or a terminated run as AC-CIO-007 completion.

The most likely EditMode last-visible boundary is the pre-existing `OrdanBossTerminalTeardownContractTests` temporary-scene fixture; the most likely PlayMode last-visible boundary is the deliberate `WalkerLinearVelocityNaN` negative-test assignment. Neither log identifies a proven stalled NUnit case. The current best attribution is test-runner/Editor workload or a run-order-sensitive existing test, not the CIO adapter, but a controlled rerun is needed to decide.

## Unity isolation and repeat follow-up — 2026-09-20

**Disposition remains:** `P0=0`, `P1=1` (`AC-CIO-007` pending), `P2=0`. The focused isolation confirms that the last-visible fixtures and the threat geometry test area pass by themselves. A second unfiltered full EditMode attempt stopped at the same observable fixture boundary without writing a result XML. The evidence now favors an order-dependent shared Editor/test-runner interaction; it still does not identify the exact stalled NUnit test or prove that the CIO test-asmdef reference correction caused it.

| Isolated run | Result | SHA-256 | Review use |
|---|---:|---|---|
| `TestResults-CIO-Isolate-OrdanTerminal-Edit-20260920.xml` | 12/12 pass; failed/skipped/inconclusive 0 | `4DC3040819A00975E449272BCB318995FA7FBCA95C46087F73DB101C00BA63F8` | Valid evidence: the last-visible EditMode fixture passes alone. |
| `TestResults-CIO-Isolate-ThreatGeometryMethod-Play-20260920.xml` | 7/7 pass; failed/skipped/inconclusive 0 | `CA2D05AE0B2B3F1A33138D0E9C37193016C794CBE4F82B708A71E2DE16386771` | Valid evidence: the full parameterized method passes. |
| `TestResults-CIO-Isolate-ThreatFixture-Play-20260920.xml` | 137/137 pass; failed/skipped/inconclusive 0 | `107826EED373F12A08CF4FD2F5BAB9A3D5A0D04BDE4D990B4A342896A154329F` | Valid evidence: the complete `RegularEnemyThreatSimulationDriverPlayModeTests` fixture passes. |

The attempted exact `WalkerLinearVelocityNaN` single-case filter produced `TestResults-CIO-Isolate-WalkerNaN-Play-20260920.xml` with `result=Passed` but `total=0`, `passed=0`. Its SHA-256 is `E00E1E5094544C2C652A77638BF9238CA4E68B2771CBB57B072844E1B4E9E200`. This empty selection is retained only to explain the filter mismatch and is excluded from pass evidence and all test counts. The successful seven-case method and 137-case fixture runs above supersede it for diagnosis.

The second full EditMode run is recorded in `Unity-CIO-Full-Edit-20260920-r2.log` (SHA-256 `EE5E4F2B0BC8F247EB3A80778220F4BBA51E50B961A260A9EA3C341DC86A7B01`). It again ends after imports of `__TempOrdanBossTerminalScene.unity` and Unity cleanup messages; `TestResults-CIO-Full-Edit-20260920-r2.xml` was not created. The run remained CPU-busy and was stopped. Since the isolated 12-case fixture passes, that fixture itself is not a sufficient explanation; the reproducible symptom is specific to the unfiltered full EditMode run or its accumulated test order/shared Editor state.

No second full PlayMode run was performed. Its previous full-suite attempt remains incomplete; the 7/7 method and 137/137 fixture results do not satisfy the full PlayMode portion of `AC-CIO-007`. Therefore neither CIO acceptance nor full-suite regression clearance is claimed.
