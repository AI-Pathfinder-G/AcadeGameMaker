# VD-09 M5D7P-A UI semantic frame seam — Luna independent post-review

- Review date: 2026-09-20
- Reviewer: Luna (`gpt-5.6-luna`)
- Contract: [M5D7P-A UI semantic frame seam](../specs/work-contracts/2026-09-20-vd09-m5d7p-a-ui-semantic-frame-seam.md)
- Implementation evidence reviewed: [Terra implementation evidence](2026-09-20-vd09-m5d7p-a-implementation-evidence.md)
- Review scope: final source and test static review, AC-M5D7PA-001..008 traceability,
  independent XML counter/duration/hash recomputation, companion-log review, and
  cross-regression/full-suite evidence. No runtime, contract, README, or other
  documentation was changed by this review. No Unity test was run by Luna.

## Verdict

**PASS — P0=0, P1=0, P2=2. GO for Astra to mark M5D7P-A Verified**, provided the
two nonblocking P2 boundaries below remain explicit. The final focused, direct,
and full-suite XML files are valid and independently match Terra's ledger. All
six retained final runs report zero failed, skipped, or inconclusive cases.

R33 (`45/52`) and R34 (`49/52`) are failed intermediate runs and are not
acceptance evidence. R29–R31 produced no usable XML and are likewise not counted.

## Independent evidence recomputation

I parsed each retained XML `test-run` root and independently recomputed the
SHA-256 of each XML, its companion log, and the final source manifest. Results
match the Terra evidence record exactly.

| Run | Scope | Result | Duration (s) | XML SHA-256 | Log SHA-256 |
|---|---|---:|---:|---|---|
| R32 | Focused EditMode; AC006–007 | 5/5; failed/skip/inconclusive 0 | 0.048662 | `9C3140052C1B88DC770E0DBB4BC4AAFABEE72FB03399DAB533ED4E80B5B49958` | `28910E94CA0C549F991BF4F08393A245654E149A6BCCF552F2A928BFCF064C47` |
| R35 | Focused PlayMode; AC001–007 | 52/52; failed/skip/inconclusive 0 | 0.3228864 | `5809FC3B685E66270EC12D88DA696B526470CFC5208DC0EE7355D98CFCB88959` | `C89CBF09F6CDC6DB6AADDB3C480A2E8B893FEED8F86242000919CDC34012F909` |
| R36 | Direct M5B5 regression | 3/3; failed/skip/inconclusive 0 | 0.1571684 | `9461921E788C631E0A7932087B9728CDE0C55DFCD71447F8FF41642A81B8BB84` | `05A9D65A2F685149CF77903D1564E9E27D44B8F77A389B9065AE2D81F5EF0838` |
| R37 | Direct M5D7O regression | 19/19; failed/skip/inconclusive 0 | 881.7320685 | `A44B46924C1DBE8BE5A38FCBE69C902E5A49B7F09422CBB97AA223DC8CE334A2` | `06C16D59FFCF170CACEDDC42EFF14CDDF6311712D1561229150904F1638FD352` |
| R38 | Full EditMode | 693/693; failed/skip/inconclusive 0 | 862.7588396 | `F8F1A1A643888B3EF79A46C10020E55419E70B502CDE8103E05127440296A90A` | `C8F78B24D6AC393F1FC072B168F3082F3D0BE5682769F420D7C8A93A18B90BCB` |
| R39 | Full PlayMode | 855/855; failed/skip/inconclusive 0 | 3258.5359445 | `14D3A8214B10CFD83F70D522F8287696A76CEEDF43F9807C09B4462D0BDDEC55` | `8948A30E777C6AFE6B4FD9CF25C35865E6C8B011337AC6F71C1DBD8D98530CED` |

Artifacts: [R32 XML](../../qa/results/m5d7pa-r32-focused-editmode.xml) · [R35 XML](../../qa/results/m5d7pa-r35-focused-playmode.xml) · [R36 XML](../../qa/results/m5d7pa-r36-direct-m5b5.xml) · [R37 XML](../../qa/results/m5d7pa-r37-direct-m5d7o.xml) · [R38 XML](../../qa/results/m5d7pa-r38-full-editmode.xml) · [R39 XML](../../qa/results/m5d7pa-r39-full-playmode.xml).

R37–R39 outlasted the command wrapper's 120-second wait, but their retained
XML files parse, contain the complete expected counts, and agree with the
companion logs' successful completion. The wrapper timeout itself is not used
as evidence. Logs contain an “Access token is unavailable; failed to update”
licensing-module warning, followed by successfully resolved entitlement details
and `Test run completed` / exit code 0. No Unity `[Assert] Assertion failed`,
compiler error, unhandled test-run failure, or licensing initialization failure
was found in the accepted logs. R39 includes exception text from expected
negative-path tests; its XML reports all 855 tests passing.

## AC review

| Criterion | Independent disposition | Evidence and review note |
|---|---|---|
| AC-M5D7PA-001 | PASS | R35 exercises generated-wrapper Navigate performed/canceled callbacks, quantization, sticky changes including return-to-frame-start, same-epoch retention, and cleared change bits. |
| AC-M5D7PA-002 | PASS | R35 covers signed fractional/off-window coordinates, first-click retention, two Mouse devices, later Point divergence, and malformed expected Click control faulting before batch mutation. Click position is read from the exact callback device during that callback. |
| AC-M5D7PA-003 | PASS | R35 covers opposing signed Scroll samples, aggregation, and Submit/Cancel performed-then-canceled coalescing exactly once. |
| AC-M5D7PA-004 | PASS | R35 verifies non-UI suppression, empty re-entry, successful-exit clearing, and held-state quarantine: held D and the old Point initial-state are suppressed through the first `InputSystem.Update`; release does not replay; later fresh D+Point is accepted. The one-shot `onAfterUpdate` hook is removed on release, exit, fault, disable, and destroy. |
| AC-M5D7PA-005 | PASS | R35's 15 injected prepare/validate/commit/map-switch rows preserve the prior publication and pending facts with retryable versus terminal outcomes; R36 independently reruns the M5B5 failure/consumer boundary. |
| AC-M5D7PA-006 | PASS | R32 tests baseline, exact consecutive delivery, duplicate poll, UI exit, first non-UI publication, foreign/equal-value source, skip/replay, fault, epoch jump, closed cursor, and checked tick/ordinal/epoch successors. |
| AC-M5D7PA-007 | PASS | R32 tests every frame and cursor backing/proof row, raw backing preservation, source identity and external fault-latch single-field mutations. R35 tests router publication/frame proof and presence rows, valid forgery, already-faulted stage preservation, malformed click, actual callback non-finite values, pixel/scroll/aggregate and callback ordinal overflows. |
| AC-M5D7PA-008 | PASS | R32/R35 focused runs, R36 M5B5, R37 M5D7O, and R38/R39 complete full suites all report failure/skip/inconclusive zero. |

### R35 Navigate non-finite test determination

The test uses a test-only processor on the exact `<Gamepad>/leftStick`
`UI.Navigate` binding. A finite `Vector2.up` raw event selects the Value action;
the processor then returns the selected NaN/+Infinity/-Infinity value to the
real callback's `ReadValue<Vector2>()`. The test asserts both processor
invocation and non-finite output counters advanced before asserting terminal
`UiCapture` and unchanged pending/publication state. `CaptureUiBatchExcluding`
keeps keyboard D from winning the equal-magnitude control tie. This is adequate
actual-callback coverage; it does not rely on a reflection-only call to
`QuantizeAxis`, nor on Unity's `StickControl` accepting unsanitized state.

Four overflow target devices are created and settled before the pending batch
is frozen and `_callbackOrdinal` is set to `long.MaxValue`; only the named
target callback follows. This avoids the earlier fixture's device-addition
initial-state events and expected Unity internal assertion noise. R35's
companion log contains no engine assertion, and its 52 rows all pass. Accepting
the earlier generic assertion would have been unsafe; the corrected fixture
removes that ambiguity while retaining real callback evidence.

## Static source review

- The only semantic capture/action-map owner remains `InputRouter`; the new
  source and cursor expose read-only receipt-bound values and do not add a
  generated-action subscriber or UI/controller authority.
- UI candidate construction and validation precede consumer mutation. The
  non-throwing publication block copies the exact frame/proofs before assigning
  `CurrentReceipt` last; precommit failures preserve pending UI state, and
  partial-commit/map failures remain terminal under M5B5 semantics.
- Click reads `Mouse.position` from `context.control.device` only after
  validating that the exact expected callback control is that same Mouse's
  `leftButton`; later/global pointer state cannot replace the click-time copy.
- The held-state quarantine is narrowly scoped to map re-entry; the same
  `InputRouter` subscribes to one `InputSystem.onAfterUpdate` callback solely to
  release suppression. It reads, stages, publishes, and consumes no input, and
  removes the hook at the required lifecycle boundaries.
- Frame validation checks the private immutable value proof; router getters
  compare exact receipt and frame copies, including value and presence; cursor
  failure preserves the raw corrupted fields and keeps terminal state outside
  the cursor's mutable instance proof row. Existing fault stage/diagnostic is
  preserved when an already-faulted getter detects later publication
  corruption.
- `UiSemanticFrameV1Tests.cs` is strongly typed against Core, and the named
  EditMode assembly contains the single allowlisted direct Core reference.
  The reviewed source and test hashes match Terra's final manifest.

## P0/P1/P2 findings

### P0: none

No ownership, publication atomicity, or player-visible input replay blocker was
found.

### P1: none

All previously raised P1s have corresponding contract changes and final source
or test evidence: independent publication proofs (including valid same-mode
replacement), exact same-device click point, raw-preserving proof corruption,
held-state quarantine, AC trace coverage, and actual Navigate non-finite
callback injection.

### P2-001 — arbitrary private reflection can remove the external cursor latch

The cursor's `ConditionalWeakTable` terminal entry detects and stays terminal
under the in-scope cursor/source instance-field mutations, including individual
`CursorFaultBox` owner/token/proof mutations. Code able to reflectively invoke
the private static `ConditionalWeakTable.Remove(cursor)` could delete the
out-of-band latch. This is outside the contract's instance-field mutation
threat model: allowing arbitrary private method invocation would invalidate
any managed in-process proof store. No change is required for M5D7P-A; keep that
threat-model boundary explicit.

### P2-002 — downstream authored-presentation continuity remains a separate gate

The later M5D7P presentation owner must preserve M5D7O's controller-owned
typed-notification payload and receipt correlation, and must keep the visible
warning continuous across scene transitions. M5D7P-A adds no notification,
controller, or presentation authority and does not itself prove that downstream
behavior. This carried pregate item is not a blocker to verifying the bounded
input seam, but must remain a downstream acceptance check.

## Recomputed source and evidence digests

| Artifact | SHA-256 |
|---|---|
| `InputRouter.cs` | `EAA9E9C84048F0DC0A6CDFB7824592050AC9EDC5B834244C9FECAF3F898D9121` |
| `UiSemanticFrameV1.cs` | `CCD7763A1103BA096DD64E02C5B91A99A3ADA21D49506CC81E5F0601E06D316F` |
| `InputRouterDiagnostic.cs` | `F88EEB8FFDE76E8F4EF4A7A7F9AE2DBBC6E4DF8B523C72807BAD65A652BF05E0` |
| `UiSemanticFrameRouterPlayModeTests.cs` | `102480263B811F0DE915679C4585970A21C1CF40D4197863A6EA05FFBF6727C9` |
| `UiSemanticFrameV1Tests.cs` | `000044B22F846550EC8DD6E92BD949E6BC1FAF257EA83719A3983C54A529BE4B` |
| Approved contract | `FF59BFCC4D28847B1B5D206473777CE9DBD0E0FC3EDDF201952B96507700A49C` |
| Luna pre-gate | `62B6A710C73048B3B565FB22AC048FFDDD1B26B0C45A77492F1EF8D11A384416` |
| Terra implementation evidence (reviewed snapshot) | `5DC859B15C78C1E02F7172125D2297B170E75D9CA803B4DB2AB80D15BBB8749D` |

## Final handoff

Luna's independent recommendation is **Verified-ready: P0=0, P1=0, P2=2**.
The focused, direct regression, and full-suite evidence satisfies AC-M5D7PA-008.
Astra retains final integration authority and may mark the bounded M5D7P-A
contract `Verified`; the downstream P2-002 continuity requirement must be
carried into the later authored-presentation gate.
