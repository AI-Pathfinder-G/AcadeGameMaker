# Costume CIO file adapter — Terra implementation evidence

- Date: 2026-09-20
- Status: **Verified by Astra on 2026-09-27; see the final closure record below**
- Implementer: Terra
- Contract: [Costume CIO file adapter](../specs/work-contracts/2026-09-20-costume-cio-file-adapter.md)
- Requirements: `REQ-CIO-001..008`
- Acceptance criteria: `AC-CIO-001..007`

## Pre-implementation shared-worktree baseline

This workspace was already materially dirty. No reset, clean, checkout, or edit of
unrelated material was performed. The following read-only baseline was captured
before creating any CIO file.

- Git `HEAD`: `309f2204cf19a321ae74c92f3be0e3fc94e3499e`
- `git status --porcelain=v1` entry count: `358`
- SHA-256 of the newline-joined status manifest: `065ACBF196D44865C123DBDAEC46E6E68A4D60171872B007C4548786D262FFB6`
- The five implementation paths below did not exist and had no Git status at
  capture time. They are the only runtime/test paths this implementation may add.

| Baseline path | Status | SHA-256 |
| --- | --- | --- |
| `Assets/AcadeGameMaker/Runtime/Costumes/IO/AcadeGameMaker.Costumes.IO.asmdef` | absent | n/a |
| `Assets/AcadeGameMaker/Runtime/Costumes/IO/CostumeFileAdapterV1.cs` | absent | n/a |
| `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/AcadeGameMaker.Costumes.IO.EditMode.Tests.asmdef` | absent | n/a |
| `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/CostumeFileAdapterV1Tests.cs` | absent | n/a |
| `docs/verification/2026-09-20-costume-cio-implementation-evidence.md` | absent | n/a |

The accepted-for-child-development pure-core files were pre-existing untracked
files in this shared worktree. Their capture hashes are recorded to make any
accidental core change detectable:

| Pre-existing path | Git status | SHA-256 |
| --- | --- | --- |
| `Assets/AcadeGameMaker/Runtime/Costumes/CostumeCoreV1.cs` | `??` | `9149AD9E4D806F02811C0D85F5E1DB044E6380C70B5409C4929CA09913EF8178` |
| `Assets/AcadeGameMaker/Runtime/Costumes/AcadeGameMaker.Costumes.asmdef` | `??` | `DF21E38D10C384F4FCF24A14EC379DD75BFB4C4ACBAE4A4195DB199785359EC2` |
| `Assets/AcadeGameMaker/Tests/EditMode/Costumes/CostumeCoreV1Tests.cs` | `??` | `53591F88A2A1CFF75D6F29D8CDDAF9A592340BC4723EFCCFBB5989B645F880B1` |
| `docs/README.md` | `M` | `66BF4809C06198046745AAAB4097398C7EEDF170A468F36E938B57B71B6488D8` |

## Approved local impact profile

The adapter will target only an already-existing caller-supplied Windows local
NTFS directory. It will use `FileStream.Flush(true)`, first-save `File.Move`, and
replacement `File.Replace`, returning a typed pre-commit failure if those
conditions cannot be established. It will not run Unity while the separate Q0
Unity process is active.

## Implementation and verification record

### Terra implementation

Added the isolated `AcadeGameMaker.Costumes.IO` assembly and its isolated
EditMode test assembly. The adapter derives its three owned paths internally,
retains one defensive observation per role, uses the pure codec/selector without
merging candidates, and carries the Windows NTFS transaction through exact-temp
write, managed flush, `Flush(true)`, one atomic operation, and reopened-primary
verification. It returns typed `ReadFailure`, `PathRejected`, pre-commit failure,
and uncertain-commit outcomes; it has no retry, cleanup, promotion, delete, or
quarantine path.

| Added or minimally changed path | SHA-256 |
| --- | --- |
| `Assets/AcadeGameMaker/Runtime/Costumes/IO/AcadeGameMaker.Costumes.IO.asmdef` | `920DD615F12F0086D978BB1E990B480ADE833F06491B0A2A58B4B41099AC3449` |
| `Assets/AcadeGameMaker/Runtime/Costumes/IO/AcadeGameMaker.Costumes.IO.asmdef.meta` | `6E5314FCFB71EFC4FB9CB2FC843D48AC340E1DBB7695F199CE45B64D5C5AE482` |
| `Assets/AcadeGameMaker/Runtime/Costumes/IO/CostumeFileAdapterV1.cs` | `67CCF0DE7E74B3FA9AE6582CCD1AF04D35F61A2135A029D0AA089A1A1B798A8E` |
| `Assets/AcadeGameMaker/Runtime/Costumes/IO/CostumeFileAdapterV1.cs.meta` | `22279AC6745358AE14B54290C35AA67883872CD9BE9DE38C01CA0873BA223B99` |
| `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/AcadeGameMaker.Costumes.IO.EditMode.Tests.asmdef` | `983E33D612DF31A07F2B37FF844802F7EDF692F8345980C31498AE08AB31BC85` |
| `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/AcadeGameMaker.Costumes.IO.EditMode.Tests.asmdef.meta` | `24C43D606721B56A2533019C434E76462132267548063A033AE3816843F980D5` |
| `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/CostumeFileAdapterV1Tests.cs` | `6A939599F0D55B03B4CBDCB939283BEB5F1B36506E815662CB125B9CD5622C23` |
| `Assets/AcadeGameMaker/Tests/EditMode/CostumesIO/CostumeFileAdapterV1Tests.cs.meta` | `31C4F428C458879BDF16792AB5D1B5FF668DC436AF8CF1413D3A92945947BC53` |

`docs/README.md` received only the contract-permitted link to this evidence. The
pre-existing pure-core hashes listed above were rechecked unchanged after the
implementation edit.

### Local checks performed (not acceptance)

| Contract coverage | Local check | Result |
| --- | --- | --- |
| `REQ-CIO-001/008`, `AC-CIO-001` | constructor rejects drive-relative `C:relative`, `C:.`, and `C:folder\\child`; recording port asserts every observed path remains one owned leaf | engine-free static pass; Unity execution deferred |
| `REQ-CIO-002/003`, `AC-CIO-002` | single-read recording port, defensive bytes, corrupt/precedence, unread-authority cases, and truthful `NotObserved` path rejection | test code added; execution deferred |
| `REQ-CIO-004`, `AC-CIO-003` | recording port proves first/replacement trace through write, managed/durable flush, close, atomic operation, reopen, equality, and decode/revision; source scan confirms the Windows primitives | engine-free static pass; Unity execution deferred |
| `REQ-CIO-005`, `AC-CIO-004` | inspection-primary/previous/temp, write, both flushes, close, move, replace, reopen, equality, and decode-stage injected faults assert typed stage, no retry, no cleanup, and owned paths | test code added; execution deferred |
| `REQ-CIO-006`, `AC-CIO-005` | all-pending missing/corrupt and valid canonical-empty cases assert `NoAcceptedDefault`, no binding, and empty unlock/current collections with zero writes | test code added; execution deferred |
| `REQ-CIO-007`, `AC-CIO-006` | source-token scan found no engine, unrelated storage authority, delete/quarantine, clock/RNG, or network token | static pass |

No Unity test was started because the concurrent Q0 verification owns that
environment. Terra does not self-accept this change. Remaining required work is
Luna's independent adversarial/replay and full-suite verification under
`AC-CIO-007`, followed by Astra's integration decision.

### Luna P1/P2 remediation — 2026-09-20

Luna's independent review reported three P1 and two P2 findings. Astra directed
their remediation inside the same Approved allowlist. Terra changed no pure-core,
scene, media, package, or unrelated file.

- `REQ-CIO-001` / `AC-CIO-001`: the input guard now explicitly rejects Windows
  drive-relative forms before `Path.GetFullPath`, instead of treating
  `Path.IsPathRooted` as a fully-qualified-path proof.
- `REQ-CIO-004/005` / `AC-CIO-003/004`: all three role inspections are inside a
  narrow expected-filesystem-exception boundary that returns
  `FailedBeforeCommit/Validation`; a test-only recording observer now captures
  the full transaction and fault stage without adding any storage authority.
- `REQ-CIO-002/003` / `AC-CIO-002`: `PathRejected` now records `NotObserved` for
  each role, rather than fabricating a read failure.
- `REQ-CIO-006` / `AC-CIO-005`: valid canonical empty state under the all-pending
  catalog is published as read-only `NoAcceptedDefault` with no binding or
  fabricated selection.

Engine-free compilation of `CostumeCoreV1.cs` plus `CostumeFileAdapterV1.cs`, a
syntax compile of the expanded test source against minimal NUnit-compatible
signatures, asmdef JSON parsing, brace/static-boundary scans, and pure-core
baseline-hash rechecks passed after these changes. Unity was not launched because
the active licensing blocker remains in force. These are implementation checks
only; Luna must independently rerun/review the expanded matrix before Astra
decides acceptance.

### Luna re-review minimal follow-up — 2026-09-20

The `CostumeFileObservationV1` constructor now admits the already-defined
no-byte `NotObserved` disposition, allowing a root-rejected load to return its
typed `PathRejected` result instead of throwing. The existing `RootIsSafe = false`
test represents that path and expects three `NotObserved` roles. An engine-free
runtime compile plus direct constructor probe passed.

The deterministic transaction observer is now an `internal` CIO seam, with
`InternalsVisibleTo` restricted to the exact CIO EditMode test assembly in the
same allowlisted runtime source. The runtime observer type is not public and does
not expose a production extension point. The engine-free visibility probe passed.
Unity remains unexecuted; this is not an acceptance claim.

### Luna port-protocol follow-up — 2026-09-20

`CostumeFilePortReadResultV1` now accepts only `Missing`, `Read`, and
`ReadFailed`; `NotObserved` remains an adapter-only observation for root rejection.
`Observe` fail-closes default, invalid, or fault-corrupted custom-port result
dispositions to a `ReadFailed` observation before the selector receives any null
candidate. Focused adversarial tests cover default, corrupted `NotObserved`, and
invalid custom results. An engine-free combined runtime/helper probe confirmed a
default custom port returns typed `ReadFailure`, has no recovery plan, and cannot
bootstrap; the public raw-result constructor rejects `NotObserved`. Unity remains
unexecuted and no acceptance is claimed.

### Unity compilation dependency correction — 2026-09-20

When the user reopened the licensed Unity Editor, the first CIO test-assembly
compile exposed `CS0246` for `CostumeCatalogV1` and `CostumeStateV1` in the
test fixture. The fixture imports and constructs those public pure-core types
directly, but its asmdef referenced only `AcadeGameMaker.Costumes.IO`. Unity
does not make a referenced assembly's own references available as direct
compile references to a consuming asmdef. The focused test asmdef now declares
both `AcadeGameMaker.Costumes.IO` and `AcadeGameMaker.Costumes` explicitly.

This is a test-only assembly-reference correction within the Approved CIO
allowlist. It neither changes the IO runtime's dependency direction nor widens
runtime authority: `AcadeGameMaker.Costumes.IO` still references the pure core,
and the test assembly merely names the same public pure-core assembly already
used by its source. It restores the direct compilation seam required to execute
the existing `REQ-CIO-001..008` fixture. The amended asmdef parses as JSON and
has exactly the two intended references. A deterministic compiler-only replay
of Unity's generated CIO test response file, with the pure-core reference added
exactly as the amended asmdef requires, produced the isolated test DLL with no
diagnostics. This checks the restored compilation seam but does not execute any
test. No second Unity process or batch test run was started while the user's
interactive Editor is active. Focused and full Unity verification remains
pending under `AC-CIO-007`.

The corrected `AcadeGameMaker.Costumes.IO.EditMode.Tests.asmdef` SHA-256 is
`939382127FA662D3CF4B93F08C02EABF03061884FC6DC7925A0B3A3D0C05309D`.

### Licensed Unity focused result and remaining regression gate — 2026-09-20

After the user reopened the licensed Unity Editor, the first post-fix run was a
recompile-only check. It completed cleanly but deliberately produced no result
XML and is not represented as a test pass. The subsequent focused EditMode r2
run produced `TestResults-CIO-Focused-Edit-20260920.xml` with the following
actual result for `CostumeFileAdapterV1Tests`:

| Scope | Total | Passed | Failed | Skipped | Inconclusive | Duration |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| CIO focused EditMode r2 | 9 | 9 | 0 | 0 | 0 | 0.0979477 s |

The result XML SHA-256 is
`29AA096D9F47E162EB57DC8DC68F7AA771B0A5C60659E703FC432B8B29662595`.
Its nine passing cases cover `AC-CIO-001..006`; this is real Unity execution
evidence for the focused CIO fixture, not a full-regression result.

`AC-CIO-007` remains **pending**. The attempted full EditMode run made no
result/log progress for more than three minutes and was stopped; no pass is
claimed. The attempted full PlayMode run emitted the pre-existing
`RegularEnemyThreatSimulationDriverPlayModeTests.cs` line 932 `NaN`
`linearVelocity` warning and then stopped making progress, so it too was
terminated without a result. Neither interrupted full-suite attempt establishes
failed/skipped/inconclusive counts, and neither is attributed to CIO without a
separate diagnosis. Luna's independent post-run verification and a completed
focused/full EditMode plus full PlayMode run with the contract-required zero
failed/skipped/inconclusive counts remain required before Astra acceptance.

## Final AC-CIO-007 closure — 2026-09-27

The preceding paragraph is the immutable 2026-09-20 execution history. The
CIO adapter/test/asmdef bytes remain identical to the corrected hashes Luna
reviewed then. Later Unity 6000.6 complete-suite evidence under
`artifacts/unity-results/m5d7qa-20260923/` passes 788/788 EditMode and 947/947
PlayMode with failed/skipped/inconclusive all zero. The full EditMode XML
contains the exact nine `CostumeFileAdapterV1Tests`, all passed.

Luna independently closed the former sole P1 at `P0=0, P1=0, P2=0` in
`docs/verification/2026-09-27-costume-cio-ac007-closure-review.md`, SHA-256
`A0839B317261192EB48F505AF09BDC6DFFCE6898A7901B2E4530FA1C36467A91`.
Astra accepted `REQ-CIO-001..008` and `AC-CIO-001..007` and marked the CIO
contract `Verified` on 2026-09-27. Earlier interrupted runs remain immutable
historical evidence and are not reclassified.
