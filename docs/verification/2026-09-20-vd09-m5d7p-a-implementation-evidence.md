# VD-09 M5D7P-A UI semantic frame seam — Terra implementation evidence

- Status: Verified — Luna independent PASS and Astra integration accepted
- Date: 2026-09-20
- Implementer: Terra (`gpt-5.6-terra`)
- Contract: [M5D7P-A UI semantic frame seam](../specs/work-contracts/2026-09-20-vd09-m5d7p-a-ui-semantic-frame-seam.md)
- Independent verifier: Luna (`gpt-5.6-luna`) — PASS (`P0=0`, `P1=0`, `P2=2`)

## Bounded implementation record

- `REQ-M5D7PA-001`: `InputRouter` remains the only generated callback and map
  owner. `IUiSemanticFrameSourceV1` exposes only immutable receipt-bound reads.
- `REQ-M5D7PA-002`: Navigate, Point, same-device Mouse-left Click, Scroll,
  Submit and Cancel pending facts are captured in the router and frozen into
  `UiSemanticFrameV1`.
- `REQ-M5D7PA-003` / `REQ-M5D7PA-004`: candidate construction precedes the
  existing consumer transaction; only its non-throwing success tail publishes
  frame/proof/receipt and clears the successful batch or epoch state.
- `REQ-M5D7PA-005` / `REQ-M5D7PA-006`: the cursor retains exact source identity
  and a proof-backed baseline; frame and router publication validation fail
  closed at their next read/commit boundary.
- `REQ-M5D7PA-007` / `REQ-M5D7PA-008`: no scene, package, UI owner,
  notification/controller, profile, run, or presentation authority was added.

## Final source manifest

| Artifact | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | `EAA9E9C84048F0DC0A6CDFB7824592050AC9EDC5B834244C9FECAF3F898D9121` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/UiSemanticFrameV1.cs` | `CCD7763A1103BA096DD64E02C5B91A99A3ADA21D49506CC81E5F0601E06D316F` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouterDiagnostic.cs` | `F88EEB8FFDE76E8F4EF4A7A7F9AE2DBBC6E4DF8B523C72807BAD65A652BF05E0` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/UiSemanticFrameRouterPlayModeTests.cs` | `102480263B811F0DE915679C4585970A21C1CF40D4197863A6EA05FFBF6727C9` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/UiSemanticFrameV1Tests.cs` | `000044B22F846550EC8DD6E92BD949E6BC1FAF257EA83719A3983C54A529BE4B` |

## Final execution ledger

All XML files below report zero failed, skipped, and inconclusive tests unless
otherwise stated. SHA-256 values cover the retained result XML and companion
log exactly as checked after the run.

| Run | Scope and AC evidence | Result | Duration (s) | XML / log SHA-256 |
|---|---|---:|---:|---|
| R32 | focused EditMode; `AC-M5D7PA-006..007` value/proof contract | `5/5` | `0.048662` | [`xml`](../../qa/results/m5d7pa-r32-focused-editmode.xml) `9C3140052C1B88DC770E0DBB4BC4AAFABEE72FB03399DAB533ED4E80B5B49958` / [`log`](../../qa/results/m5d7pa-r32-focused-editmode.log) `28910E94CA0C549F991BF4F08393A245654E149A6BCCF552F2A928BFCF064C47` |
| R35 | focused PlayMode; `AC-M5D7PA-001..007` callback, epoch, transaction, cursor, and fail-closed matrix | `52/52` | `0.3228864` | [`xml`](../../qa/results/m5d7pa-r35-focused-playmode.xml) `5809FC3B685E66270EC12D88DA696B526470CFC5208DC0EE7355D98CFCB88959` / [`log`](../../qa/results/m5d7pa-r35-focused-playmode.log) `C89CBF09F6CDC6DB6AADDB3C480A2E8B893FEED8F86242000919CDC34012F909` |
| R36 | direct M5B5 regression; `AC-M5D7PA-005`, inherited commit/failure boundary | `3/3` | `0.1571684` | [`xml`](../../qa/results/m5d7pa-r36-direct-m5b5.xml) `9461921E788C631E0A7932087B9728CDE0C55DFCD71447F8FF41642A81B8BB84` / [`log`](../../qa/results/m5d7pa-r36-direct-m5b5.log) `05A9D65A2F685149CF77903D1564E9E27D44B8F77A389B9065AE2D81F5EF0838` |
| R37 | direct M5D7O regression; `AC-M5D7PA-008` downstream boundary | `19/19` | `881.7320685` | [`xml`](../../qa/results/m5d7pa-r37-direct-m5d7o.xml) `A44B46924C1DBE8BE5A38FCBE69C902E5A49B7F09422CBB97AA223DC8CE334A2` / [`log`](../../qa/results/m5d7pa-r37-direct-m5d7o.log) `06C16D59FFCF170CACEDDC42EFF14CDDF6311712D1561229150904F1638FD352` |
| R38 | full EditMode; `AC-M5D7PA-008` regression gate | `693/693` | `862.7588396` | [`xml`](../../qa/results/m5d7pa-r38-full-editmode.xml) `F8F1A1A643888B3EF79A46C10020E55419E70B502CDE8103E05127440296A90A` / [`log`](../../qa/results/m5d7pa-r38-full-editmode.log) `C8F78B24D6AC393F1FC072B168F3082F3D0BE5682769F420D7C8A93A18B90BCB` |
| R39 | full PlayMode; `AC-M5D7PA-008` regression gate | `855/855` | `3258.5359445` | [`xml`](../../qa/results/m5d7pa-r39-full-playmode.xml) `14D3A8214B10CFD83F70D522F8287696A76CEEDF43F9807C09B4462D0BDDEC55` / [`log`](../../qa/results/m5d7pa-r39-full-playmode.log) `8948A30E777C6AFE6B4FD9CF25C35865E6C8B011337AC6F71C1DBD8D98530CED` |

R37, R38, and R39 exceeded the command wrapper's 120-second wait. That wrapper
early return is not acceptance evidence on its own. The already-running Unity
process was recovered and observed to complete without a rerun; its retained,
valid XML is the execution evidence recorded above.

## AC coverage conclusion

- `AC-M5D7PA-001..004`: R35 exercises the real generated-wrapper UI callback
  route, sticky current/change semantics, click-time device point, scroll and
  edge coalescing, successful exit, and empty re-entry epoch.
- `AC-M5D7PA-005`: R35's complete router failure-injection matrix is paired
  with R36's direct inherited M5B5 failure-boundary regression.
- `AC-M5D7PA-006..007`: R32 covers immutable frame/proof values; R35 covers
  cursor identity/baseline/consecutive behavior and malformed publication or
  callback capture fail-closed behavior.
- `AC-M5D7PA-008`: R32/R35 focused evidence, R36/R37 direct regressions, and
  R38/R39 full suites complete the required execution matrix.

## GPT participation ledger

| Responsibility | Model | Recorded state |
|---|---|---|
| Implementation and implementation tests | Terra (`gpt-5.6-terra`) | completed; this evidence record |
| Independent verification | Luna (`gpt-5.6-luna`) | PASS; independent post-review completed |
| Contract/integration acceptance | Astra (`gpt-6-astra`) | accepted; contract advanced to Verified |

## Retained non-acceptance and stabilization history

- R01/R02: compile preflight found a missing test-only Core reference, then a
  `SimulationTick` fixture conversion; neither is pass evidence.
- R03: cursor failed-row observability correction; R05: test-fixture
  coordinated-frame sequencing correction. These earlier diagnostics remain
  historical stabilization records, not acceptance evidence.
- R29/R30/R31: Unity licensing/authentication instability; no valid result XML
  was produced, so none is counted as an execution pass.
- R33: focused PlayMode `45/52` passed and `7/52` failed; retained as a failed
  intermediate result.
- R34: focused PlayMode `49/52` passed and `3/52` failed; retained as a failed
  intermediate result.

Luna independently recomputed the retained evidence and reported
**PASS — P0=0, P1=0, P2=2**. Astra accepted that verdict on 2026-09-20 and
advanced the bounded contract and implementation to **Verified**. The two
nonblocking P2 boundaries remain recorded in the Luna post-review and carry
forward to the downstream authored-presentation gate.
