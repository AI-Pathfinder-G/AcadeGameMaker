# C1 continuation — disk-only implementation accepted

Date: 2026-09-28. No user decision is required for this approved work.
Trace: REQ-M5D7QC1-001..007 / AC-M5D7QC1-001..009.

The earlier R7 19/19 focused and 175/175 Profile regression results remain
evidence for their recorded source hashes only. They are not evidence for the
new continuation source. C1 is still Approved/WIP, not Verified or integrated.

The preceding paragraph records the starting state of this continuation.
Final status is disk-only Verified, as recorded in the acceptance section below;
historical partial results and open gates below remain chronological history.

## Independent findings and approved response

Luna's continuation audit found contract-blocking failure-result omissions and
missing actual issuing-lease instance binding. Astra approved the bounded
[test/result clarification](../specs/work-contracts/2026-09-28-vd09-m5d7q-c1-test-and-result-clarification.md).
This retains the closed result matrix, adds fixed failure evidence snapshots,
and adds internal-only real-BCL checkpoint testing without a second save core.
Proof observation is not consumption authority; the future C2 consumer must
atomically consume a validated proof. No receipt or memory authority exists.

Sol proposed an engine-free .NET child loading the actual compiled Profile DLL
to test genuine process termination. Astra approved the exact additional
[process fixture scope](../specs/work-contracts/2026-09-28-vd09-m5d7q-c1-process-crash-fixture.md).
Actual child transaction execution, lock-holder contention and simulated
checkpoint interruption must remain distinct in final evidence. Compatibility
and checkpoint execution are not yet verified.

Terra added adversarial tests, then began capability, result and checkpoint
implementation. A second Terra owns only the process worker and process tests;
their write sets do not overlap. Main alone runs Unity. A current host preflight
passed for pinned 6000.6.0f1, licensing clients 1.18.3, non-empty entitlement,
and no competing project Editor. Preflight is not a test execution or PASS.

The broad repository whitespace check reports an unrelated existing trailing
space in ProjectSettings.asset line 485. It is preserved; untracked new files
also require compilation/independent review, not a git-diff-only claim.

## GPT participation ledger

- Astra: bounded technical approvals, allocation, local evidence audit and host
  Unity preflight; final acceptance still pending.
- Sol (`gpt-5.6-sol`): fault-seam and actual child-process fixture design;
  follow-on C2 draft only, with no competing approval authority.
- Terra (`gpt-5.6-terra`): adversarial tests and runtime clarification; separate
  Terra worker owns process-fixture/test implementation.
- Luna (`gpt-5.6-luna`): independent runtime/contract gap audit and clarification
  review. Post-implementation review remains required.

No retired external model, recurring automation, paid API route or Pro-mode
execution was used. Continue approved gates before memory cutover/UI/scene work.

## R8/R9 actual execution

R8 stopped at compilation: two CS0051 accessibility errors in the new
adversarial test methods. Its log is retained at
`artifacts/c1-r8-all-reset.log`; no XML and no PASS. Terra corrected only the
nested test enum's visibility.

R9 compiled and actually executed 106 tests in the host Unity session:
`artifacts/c1-r9-all-reset.xml` and matching log. Overall **FAILED**, 104 passed,
2 failed, skipped/inconclusive 0. Both failures are process-fixture BuildWorker
compiler exit 1 before any checkpoint, not demonstrated production reset
failures. The output lacked build stdout, so root requested exact build-command
diagnostics and a harness correction before a fresh run.

- Adversarial: 50/50 passed (AC-001/002/004/007/009 subsets).
- Marker fault: 12/12 passed (AC-003 marker subset).
- Result: 7/7 passed (AC-009 and authenticated marker fault subset).
- Reset-only save fault: 16/16 passed (AC-005/006 subset).
- Existing C1 focused: 19/19 passed.
- Actual process fixture: 0/2 passed; AC-008 still unverified.

R9 XML SHA-256: `AC0DBF3921A8F98D2E4A34A35209C39ECC7D91ACE8062DB36CC43A63999000A0`.
Runtime reset SHA: `8D623D12B6642BC4B1BA7BCEAB06462A86AC2B335365AEA7F0E163B3B82D8BEB`.
Atomic save SHA: `80DDBF925C74A8A81DD3B3950F25B3843D76B81F62ED942F3F3585CFB52B3F87`.
Approved clarification SHA: `7817A480DC4AF3C9DB46570CBF839F7D5BC941FD5CE23AB7F353EBAE430CC3C8`.
Approved process amendment SHA: `20685A9E2FA0ADE6AF218A0FA9662717369E4B23EBF6B80E3854E13AB886C79A`.

No subset pass closes whole C1. Known open gates include full archive/default
checkpoint forwarding, actual process crash matrix, archive observation
classification/revision and row invariants, failure stage fidelity and ownership
of rejected-root diagnostic probes. The save-fault helper named
SimulatedCrashException derives IOException and is handled as a recoverable
after-IO fault; it is **not** an escaping simulated crash or actual process
death. Its evidence is classified accordingly pending label/test correction.

Sol also drafted C2 memory cutover in Review. Astra clarified terminal failure
gating and future request/callback invalidation; Luna found the two draft P1s
closed. C2 remains unapproved/unimplemented pending C1 gates; no live menu or
scene integration is authorized by this continuation.

## R10 real process execution

After the worker build argument quoting was corrected, main executed
`artifacts/c1-r10-process.xml` and matching log: **2/2 passed**, failed/skipped/
inconclusive 0, wrapper exit 0. These tests actually paused the transaction in
a child holding the root lock, proved second-owner Busy/no mutation, killed
that exact child and confirmed exit, and resumed in a distinct process.
BeforeMarkerWrite retains old active bytes without committed authorization;
AfterMarkerCommitMove resumes to DiskPrepared with exact old archives.
This proves these two process windows only, not the full AC-008 matrix.

The 104 passed R9 tests and the separate two-case R10 rerun are distinct runs,
not a fabricated fresh 106/106 full-suite run. Hardening and broader checkpoint
work continue under their Approved scope. No user product decision is pending.

## R11 hardening and widened actual execution

R11 host Unity execution `artifacts/c1-r11-all-reset.xml` and matching log:
**134/134 passed**, failed/skipped/inconclusive 0, wrapper exit 0.
Adversarial 50, marker fault 12, actual child-kill/restart marker cases 12,
result invariants 9, save IO/interruption/restart cases 32, original focused 19.
The save helper now separates escaping non-IO interruption from recoverable
after-IO faults; neither is falsely described as actual process death.

XML SHA-256: `B35A297CF4AAE2B20F1B18FF640B054B896032B6DC3D2F838AA590296F6B37C7`.
Runtime SHA: `09CAA4452DD57FF931BAA4A85D34D3FCD4BEA392ABD55C67A625324799E794E7`.
Result-test SHA: `02FFC930D16898D9E84FEF349EA43938B50991443F72E3DBB4AEC6E6D2A3874F`.

Luna independently inspected the frozen runtime and found the five prior
hardening P1s addressed: no pre-lease failure I/O, per-read archive containment,
readable archive classification/revision, closed proof/marker row binding, and
phase-specific failures. No P0 was found. This is partial review, not whole-C1
acceptance. Old-leaf/default/preservation checkpoint forwarding remains the
open P1 gate, alongside complete actual process coverage and explicit proof
double/concurrent consumption plus actual issuing-lease tests.

Astra allocated the full checkpoint forwarding to Terra, and the proof-only
class hunk/new proof tests to a separate Terra with nonoverlapping write sets.
No worker may independently accept its implementation. C2 remains Review.

## R12 full non-process continuation execution

Host Unity ran the frozen continuation source through the locally confirmed
regex filter excluding only `ProfileResetDiskProcessV1Tests`:
`artifacts/c1-r12-nonprocess.xml` and matching log. **212/212 passed**,
failed/skipped/inconclusive 0, wrapper exit 0.

- Adversarial 50; marker faults 12; original disk transaction 19.
- Full archive/preservation/default checkpoints: 78 (39 closed checkpoints,
  each with recoverable IO and escaping non-IO interruption).
- Reset-only save faults/interruption: 32.
- Result observations/invariants: 11, including missing-declared archive rows.
- Prepared proof and actual issuing-lease boundary: 10, including mutated
  payload rejection before consumption and 64 concurrent consumers/one winner.

IO injection is one-shot. Existing SaveHeld failure diagnostics may legitimately
reprobe the same file; those diagnostic operations do not become additional
injected failures. Test exceptions are not evidence of actual process death.

R12 XML SHA: `D48BCB48B5C880D42301AA0E3F9EF480AC087AF7303845ED204E95D45A15CA1E`.
Runtime SHA: `90EFBE7A71C20241BD9CF792DE42BFD1B2142BCCB7AD2A146215D62ECC194743`.
Checkpoint test SHA: `B1422C20E89F9156E9598ACACA497049760BCA0E90B9E9DDF02CA8B9F1661EF5`.
Proof test SHA: `3371AB3B5E1F7961C93D64DC9565DB658139ADC6AF9F12164AAC0D46D123CB9B`.
Result test SHA: `F9C5C6E00D32AA2F0008BC89B0D282F891B2BA9903E5F9ACF40C18318658EBD0`.

Luna independently audited the frozen runtime, new proof/checkpoint tests and
full 51-case process harness: P0=0/P1=0 found in source review. Confirmation
identity is consumed before the fresh observation; prepared proof payload is
validated before its atomic consumption. These are distinct boundaries.
Main found and Terra corrected fixture compiler/child cleanup and the
interrupted archive assertion path before the process run. Neither source review
nor the worker-only build is counted as a process PASS.

## R13 actual full process matrix

The wrapper ended its 120-second XML wait before this deliberately longer run
finished, reporting missing XML (launcher exit 0). The same actual Unity Editor
PID 45296 was still executing; main did not relaunch or terminate it. A read-only
observer followed that exact process and the subsequently flushed evidence.
Actual `artifacts/c1-r13-process.xml`: **51/51 passed**, failed/skipped/
inconclusive 0. The matching log line 406 records the actual test runner exiting
with code 0; the process is no longer alive. Wrapper timeout and successful actual
test execution are recorded separately, not represented as wrapper success.

Every closed checkpoint from 1 through 51 ran in a real child loading the compiled
Profile DLL. Each case confirmed its PID/point ready signal, five-second Busy
and unchanged durable snapshot, exact-child termination/confirmed release, and
restart in a distinct child. The nine pre-publication rows retain old active
bytes and produce ManualRepairRequired; all 42 committed-marker rows converge
to DiskPrepared with the barrier retained and exact old archives. Eight of those
rows additionally prove interrupted Temp/corrupt Primary preservation.

R13 XML SHA: `01F6061BB55971120626BE731A6C6BD8E2CB039AFEFD7371CD7AC59B2DCFB4FE`.
Process test SHA: `FD33B463615DC3D1A6BCAB6D8425992E5925D662BC62FD1CEF44CC9E636E0E90`.
Worker SHA: `740C37EE597749C0E3B3BA346A01940555F7CD09096B06E018E757FC426AAEA0`.

C1 still needs current-source Profile regression and the final independent
evidence digest before Astra may mark it Verified. C2 remains Review. Sol/Luna
pre-review found technical C2 gaps (proof-CAS ordering, deterministic seams,
terminal failure gating, launch-root authority); Astra amended the Review draft
without granting implementation authority or requiring a new user product choice.

## R14 regression and Astra acceptance

Current-source `artifacts/c1-r14-profile-regression.xml` and matching log:
**175/175 passed**, failed/skipped/inconclusive 0, wrapper exit 0.
XML SHA: `2F09B383BAF2A3289A44A56B454449F9E3692B85095309AE6F4E1B230F905320`.
The runtime/atomic/root-lock hashes remain respectively 90EFBE…194743,
80DDBF…B3F87 and 2E1DCE…54098 as fully recorded in the
[final Luna review](./2026-09-28-vd09-m5d7q-c1-final-luna-review.md).

Luna independently checked all three XMLs and the exact frozen source, found
P0=0/P1=0, and mapped AC-M5D7QC1-001..009. Astra checked the mapping and corrected
an initial AC-004 attribution and an unsupported executed-reparse claim before
acceptance. Root/reparse containment remains static source evidence, not a
dedicated executed Windows reparse fixture. This limitation is explicit.

Astra accepts and integrates the bounded C1 disk-only scope on 2026-09-28.
C1 is Verified; parent Q-C is not complete. No UI confirmation, request
consumption, memory cutover, barrier removal, receipt, scene, gameplay or archive
cleanup/restore has been integrated. Existing unrelated dirty work is preserved.
C2 may proceed only under its separately approved contract and independent gates.
