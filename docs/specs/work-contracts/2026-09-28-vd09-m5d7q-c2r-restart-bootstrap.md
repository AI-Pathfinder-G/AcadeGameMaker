---
status: Verified
---

# VD-09 M5D7Q-C2R reset restart bootstrap

- Date: 2026-09-28
- Status: Verified — Astra 2026-09-28; bounded technical restart recovery
- Owner and approval: Astra
- Bounded design: Sol; intended implementation: Terra; independent QA: Luna
- Parent: `2026-09-28-vd09-m5d7q-c2-memory-cutover.md` (Approved)
- Required predecessor: C1 disk transaction Verified; C2 same-process behavior
  stable for its non-restart rows. C2 need not be declared Verified first because
  its `AC-M5D7QC2-007` remains open pending this contract.

## Purpose and unresolved parent criterion

C2R supplies the missing true-process-restart path from a durable C1
`DiskPrepared` row to C2 completion. Current ordinary launch preparation rejects
the required durable barrier, while current C2 accepts only a cohort published
by ordinary launch `Start`. Consequently, safe blocked reload is existing
behavior but is not full evidence for `AC-M5D7QC2-007`.

This contract does not waive or reinterpret that criterion. Parent
`AC-M5D7QC2-007` remains **OPEN** until C2R is Approved, implemented, and proven
with distinct process A/process B evidence. C2R is a technical recovery
lifecycle, not a user-facing product or destination decision.

## Startup selection and authority

`DesktopProfileLaunchAdapterV1` has execution order `-220`, before the router at
`-210`. Its `Awake` therefore observes the actual normalized persistent profile
root before any ordinary preparation or UTC read and chooses exactly one path:

1. no committed barrier: yield permanently to the unchanged ordinary launch;
2. committed barrier: suppress ordinary preparation for this startup and create
   one private `RecoveryReservedNoActions` reservation on the exact authored
   router, without actions, callbacks, maps, receipt, or reset proof;
3. root/marker state that cannot be classified safely: enter a closed repair
   result without profile load, save, input publication, or launch retry.

The reservation is not C2 eligibility. Router `Awake`, `Start`, `Update`, and
`FixedUpdate` recognize only its private identity: they validate the authored
graph, allocate/initialize/publish no actions, callbacks, frame, or receipt, and
never enable a map or enter legacy launch-failure handling. After router
`Awake`, the adapter's Unity `Start` callback may run, but it selects a distinct
recovery-only body. The ordinary launch `Start` body and
`ConfirmPreparedHubPublication` are never called.

Selection is a read-only safety classification, not disk authority. Reuse the
existing Profile-owned pre-lease root/ancestor/lock containment check, then
inspect both barrier paths with real BCL attributes and regular-file reads;
`File.Exists` alone cannot prove absence or safety. Missing-file/directory
exceptions are absence only after safe ancestor validation; unreadable,
directory, reparse, or reset-temp-only rows are repair-blocked. No directory or
lock is created during selection. Resume revalidates the live state under its
actual lease and remains the only source of prepared-proof authority.
Disable/destroy before Resume or mint irrevocably closes the private reservation
and cannot enter ordinary launch cleanup or restore legacy input authority.

That recovery-only body invokes the existing real
`ProfileResetDiskTransactionV1.Resume(root)`. It accepts only the exact
`DiskPrepared` result returned by that same invocation. Its fresh prepared-proof
reference and result identity promote the reservation into a private recovery
witness keyed to the exact authored adapter, exact authored router, and exact
normalized root, then immediately enter the recovery-specific C2 path. No
caller boolean, enum, root string, copied fingerprint, reflection-mutated
diagnostic, foreign proof, previously returned proof, reservation alone, or
synthetic preparation token can mint or recover this authority.

`ProfileResetDiskResultV1` is an existing readonly value type, not a reference
object. Same-call result identity means a private registration made only by the
real recovery Start/Resume branch: validated DiskPrepared value facts, the exact
fresh PreparedProof reference, and a private invocation/one-shot execution
witness. Copying or boxing equal result values cannot mint that registration
or replay execution. No C1 type change or accidental boxed-value reference
comparison is authorized. Recovery entry validates that private registration
and consumes its execution claim before disk or memory effects; mutable startup
flags alone cannot rearm it. This is the concrete meaning of the already-required
same-invocation authority, not an additional caller-supplied token route.

The private witness is either `OriginalLaunch` or `RestartRecovery`, never both.
Existing same-process C2 continues to require `OriginalLaunch` exactly as now.
C2R adds no broad relaxation to `FinalizeReset` eligibility and cannot turn an
ordinary failed/pending launch into a recovery cohort.

## Restart-recovery cohort

`RestartRecovery` is not a launch and creates no
`PreparedProfileLaunchV1`, `DesktopProfileLaunchReceiptV1`, hub handoff receipt,
notification, or fabricated historical evidence. The exact authored
`InputRouter` must be pristine, unfaulted, authored `UIOnly`, and privately
reserved by the earlier adapter `Awake`. Router `Awake` validates that reserved
graph without adopting or publishing actions. The later adapter `Start`
callback executes only the recovery body; it never calls the ordinary launch
body or `ConfirmPreparedHubPublication`.

The router remains noninteractive with every map disabled. C2 stages and applies
the same fresh default actions and exact r0 session document used by its
same-process path. Because a fresh process has no published old action
collection, old-action disposal is explicitly `NotApplicable`; it must not be
invented or counted. Candidate ownership, exact-once close attempts after
allocation, fail-stop pair publication, authenticated barrier deletion, final
reprobe, and one-shot C2 receipt remain unchanged. Same-process
`OriginalLaunch` retains its original-old-action exact-once disposal rule.

Any historical receipt that really exists remains byte/value-identical. In the
fresh recovery process none exists, so C2R mints none. The completed C2 receipt
still grants only `UIOnlyBlocked` memory/disk agreement and no menu, scene,
gameplay, save, notification, or destination authority.

## Closed outcomes and failure policy

The bootstrap result is closed to `OrdinaryLaunchRequired`, `Completed`,
`BusyBlocked`, `ReloadRequired`, and `ManualRepairRequired`.

- `OrdinaryLaunchRequired` is legal only when both barrier names are proven
  absent before any recovery witness or action allocation.
- `Completed` contains the existing exact C2 receipt and is legal only after
  C1 `Resume -> DiskPrepared`, private recovery mint, memory pair publication,
  authenticated barrier removal, and final reprobe.
- root-lease `Busy` becomes `BusyBlocked`: no automatic or in-session retry,
  no fallback to ordinary launch, no action allocation, and the barrier stays.
- recoverable pre-delete recovery/C2 faults become `ReloadRequired` or
  `ManualRepairRequired` under existing C2 classification and keep the session
  noninteractive. No ordinary launch fallback or same-session retry is legal.
- if the process dies after authenticated barrier deletion but before receipt
  publication, the next startup follows ordinary exact-r0 launch and cannot
  replay a lost receipt or resurrect old progress.

C2R performs no map enable, ordinary profile load, profile save, archive restore
or cleanup, reset-temp cleanup, scene/UI operation, clock/random/network call,
or filesystem mutation other than real C1 `Resume` and existing authenticated
C2 operations.

## Requirements

- **REQ-M5D7QC2R-001:** In adapter `Awake`, select recovery before ordinary
  preparation/UTC access only from the actual root's barrier state and create
  only a private no-actions router reservation; never bypass, hide, or
  temporarily remove the barrier.
- **REQ-M5D7QC2R-002:** Mint one private recovery cohort only from the exact
  `DiskPrepared` result/proof returned by the same real C1 `Resume` call and the
  exact pristine authored adapter/router/root.
- **REQ-M5D7QC2R-003:** Keep the recovery router disabled and noninteractive,
  create no fake launch history, and treat absent old actions as
  `NotApplicable` without weakening original-cohort disposal rules.
- **REQ-M5D7QC2R-004:** In the adapter's recovery-only `Start` body after router
  `Awake`, feed the fresh proof into existing C2 disk
  reauthentication, pair publication, barrier removal, final reprobe, and
  receipt semantics without alternate reset algorithms.
- **REQ-M5D7QC2R-005:** Return one closed startup outcome with no automatic
  Busy/failure retry or ordinary-launch fallback after recovery selection.
- **REQ-M5D7QC2R-006:** Prove the restart boundary with two actual processes and
  preserve every C2 exclusion and historical-evidence invariant.

## Acceptance criteria

- **AC-M5D7QC2R-001:** Barrier-absent startup reaches only unchanged ordinary
  launch. Barrier-present adapter `Awake` reserves `RecoveryReservedNoActions`
  before ordinary Prepare/UTC access; router `Awake` validates it without
  actions/maps/callbacks. The later adapter `Start` callback runs only the
  recovery body and never invokes the ordinary launch body or router
  launch-confirm publication.
- **AC-M5D7QC2R-002:** Exact/foreign roots, stale/foreign/consumed proofs,
  sibling owners/routers, duplicate bootstrap, reflected diagnostics, and
  malformed/default result rows prove that only the same actual C1 Resume result
  can mint one private recovery witness.
- **AC-M5D7QC2R-003:** Pristine-router recovery proves all maps remain disabled,
  no launch/hub receipt or notification is created, old-action disposal is
  `NotApplicable`, and every allocated candidate has one authoritative owner and
  at most one close attempt. Router `Start`, `Update`, and `FixedUpdate` remain
  quiescent in both recovery-reserved and completed-blocked states and never
  route them through legacy failure or map-enable behavior.
- **AC-M5D7QC2R-004:** Busy, marker/archive/active-row mutation, staging,
  binding, transfer, deletion, and final-reprobe fault matrices prove the exact
  closed outcomes, barrier policy, terminal containment, and absence of
  automatic retry or ordinary-launch fallback.
- **AC-M5D7QC2R-005:** Process A creates and exits with a real exact C1
  `DiskPrepared` row. A distinct process B, with fresh static state, runs real
  C1 Resume, mints the recovery cohort, completes existing C2, and proves exact
  r0 disk/current-session agreement, barrier absence, disabled maps, and one
  completed receipt.
- **AC-M5D7QC2R-006:** A separate death-after-delete/before-receipt row proves
  the next fresh process performs ordinary exact-r0 launch with no receipt replay
  or old-progress resurrection.
- **AC-M5D7QC2R-007:** Static/API review proves no fake preparation/history,
  generalized hot-swap/recovery mint, map enable, profile load/save, archive or
  temp cleanup, scene/UI/destination authority, network, clock, random, new
  friend assembly, asmdef, or project/package configuration.
- **AC-M5D7QC2R-008:** Luna reports contract P0=0/P1=0 before Astra contract
  approval. After Approved implementation, focused and required regressions
  have zero failed, skipped, and inconclusive tests, and Luna independently
  reports implementation P0=0/P1=0 before final integration. Only then may parent
  `AC-M5D7QC2-007` close.

Traceability: `001 -> AC-001`; `002 -> AC-002/005`; `003 -> AC-003/007`;
`004 -> AC-004/005/006`; `005 -> AC-004`; `006 -> AC-005/006/007/008`.

## Exact implementation and evidence allowlist

After Astra changes this contract to Approved, implementation may make only
narrow recovery-lifecycle additions to:

- `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs`;
- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`;
- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs`;
- focused existing-assembly EditMode/PlayMode tests and `.meta` files;
- one focused process-A/process-B fixture or main-agent runner under
  `Assets/AcadeGameMaker/Tests/` or `qa/fixtures/`, plus `.meta` when applicable;
- C2R verification evidence and one minimal `docs/README.md` link.

`ProfileResetDiskTransactionV1.Resume` is used exactly as it exists and is not
modified by C2R. The process fixture may use uniquely owned, validated OS-temp
roots and either a small .NET worker or two Unity Editor subprocess invocations.
Only the main test runner/agent may create those processes; runtime/test code may
not spawn nested workers. Cleanup must validate the exact owned temp descendant,
remove links nonrecursively first if any exist, and treat fixture inability as a
failed/unverified gate, never Skip/Ignore or a fabricated pass.

No new asmdef, friend assembly, public ABI, generated actions, input asset,
scene, prefab, package, or project setting is authorized. Test seams are
internal closed-enum checkpoints only: they may observe/throw at named
boundaries but cannot substitute Resume results, proofs, barrier observations,
actions, binding results, disk operations, or lifecycle outcomes.

## Stop conditions and approval gate

Stop for Astra if the recovery lane requires changing C1 Resume or ordinary
launch preparation/coordinator barrier checks, fabricating a launch
receipt/token, removing the barrier before memory pair publication, entering
the ordinary adapter Start body, enabling a map, loading/saving a profile,
cleaning reset/archive files, widening public ABI, or adding assembly/config
access. Do not infer a user-visible retry, progress screen, destination, or menu
policy from this technical startup correction.

Luna must pre-review every `AC-M5D7QC2R-*`, the reservation-to-private-mint
boundary, exact adapter/router Awake and Start ordering, quiescent router
lifecycle, and process fixture before runtime work. Astra alone may change
Review to Approved; Terra then implements; Luna independently verifies; Astra
alone integrates and records closure of parent `AC-M5D7QC2-007`.

### Astra approval — 2026-09-28

Sol identified the genuine restart/ordinary-launch incompatibility and drafted
the distinct private recovery lane. Astra corrected actual Awake/Start ordering,
selection containment and disable/destroy closure. Luna independently reviewed
all eight ACs and the amended Review contract SHA-256
`995816524904EA8A4ADC804E4688E7163740C198EF06A5E7CDA062F237E29A4E`
without a P0/P1 contract blocker. Evidence is
`docs/verification/2026-09-28-vd09-m5d7q-c2r-luna-pregate.md`.

Astra approves only the exact bounded source/test allowlist. Allocation of
runtime edits must wait until the active C2 R10 Editor exits and its unchanged
source/result baseline is recorded. The ordinary same-process lane stays
unchanged; genuine restart implementation and separate-process evidence remain
pending. Neither C2R nor parent AC-007 is Verified. No live menu, destination,
map enabling, archive restoration or new user product decision is implied.

### Startup compatibility clarification — Approved, Astra 2026-09-28

Sol's bounded counter-review and Luna's corrective pre-gate identified a
single-root/reservation compatibility defect. Luna independently reviewed the
Review appendix at SHA-256
`1B546FD7C7D670543A945A8A1BEACB5534334DFC841EA39D4E65D6C0FB00DECA`
with P0=0/P1=0; evidence is
`2026-09-28-vd09-m5d7q-c2r-startup-compatibility-luna-pregate.md`.
Astra approves only the bounded correction below. It
clarifies REQ-M5D7QC2R-001/003/005 and AC-001/002/003/004/007/008;
it changes no C1/C2 disk algorithm or user product direction.

Adapter Awake first validates the authored cohort and acquires the existing
exact-owner, pristine, actionless prepared reservation. It then reads the
environment root exactly once. Normalize/validate that returned value once;
use that same normalized root for read-only barrier selection and, if both
barriers are safely absent, the unchanged ordinary preparation. The absent
trace remains `Reserve -> root -> UTC -> Prepare -> Take -> Adopt ->
transferred-token Dispose`. No second environment root lookup is permitted.
This provisional reservation is not an OriginalLaunch cohort: it has no
actions, callbacks, preparation token, receipt, history or C2 eligibility.

Committed barrier or safely detected unsafe/ambiguous returned-root state
promotes that exact owner's pristine unadopted Reserved row once, through a
private irreversible transition, into RecoveryReservedNoActions or
RecoveryRepairBlockedNoActions. Promotion is legal before router Start,
allocation, adoption, callbacks, initialization, fault, disposal or any map
enable, whether router Awake has or has not entered. There must be no
observable intermediate Unreserved row and no fallback. It atomically revokes
ordinary Adopt/Initialize/Confirm/FailPrepared eligibility. Private recovery
registration and terminal/quiescent routing, not reflected diagnostic enums,
own the promoted identity. Foreign/duplicate/corrupt promotion rejects before
affecting a legitimate different owner. Disable/destroy closes that original
promoted reservation only; it never invokes ordinary cleanup or mints recovery.

Reservation rejection, or an exception thrown by the environment root getter
before a value is returned, remains the historical M5D7M failure: preserve and
propagate/log the original exception, close only the acquired exact owner,
and allow no fallback, action allocation, receipt or retry. Once a value is
returned, null/whitespace/relative/filesystem-root/unsafe-containment values
and expected read-only classification errors (ArgumentException, IOException,
UnauthorizedAccessException, unsafe directory/reparse/unreadable/temp rows)
are the explicit C2R override: privately promote to a closed readable
ManualRepairRequired result, with no UTC, Prepare, ordinary exception logging
or retry. Unexpected programmer/invariant exceptions preserve their original
identity and contain the exact acquired owner; do not mislabel them as repair
or Busy. Ordinary UTC/preparation/adoption/publication failures remain M5D7M.

The bounded runtime allowlist for this correction is the existing adapter and
router only. Focused existing launch tests and recovery tests may be updated
only for this explicitly superseded unsafe-returned-root behavior; the absent
trace, getter-thrown exception, ownership, cleanup, and remaining old failure
rows retain their assertions. Add single-lookup A/B-environment protection,
promotion before/after router Awake, exact/foreign/duplicate/corrupt promotion,
teardown, no actions/history/fallback and readable repair-result evidence.
No existing acceptance is silently waived or reported as newly executed.

Stop for Astra if promotion requires a public ABI, new assembly/configuration,
C1 Resume/C2 algorithm change, transient Unreserved row, ordinary fallback,
fake action/history/token, or loss of original unexpected exception identity.
The amended source must receive fresh Luna review and required execution.

#### Authored Hub-only compatibility scope — Approved, Astra 2026-09-28

R12 and Luna's independent execution review
`2026-09-28-vd09-m5d7q-c2r-r12-luna-execution-review.md` expose an unintended
application of the recovery lane to historical non-Hub routers. The recovery
reservation, selection, returned-root repair override, private reset-cohort
mint and bootstrap result in this contract apply only to the actual authored
pristine HubUIOnly variant. This is the original recovery eligibility scope,
not permission to generalize promotion or weaken Hub ownership proofs.

A non-Hub historical desktop launch retains M5D7M's exact actionless Reserve,
single environment root read, legacy Normalize/root-failure behavior, UTC,
Prepare, Take/Adopt/Dispose, initialization and historical receipt publication.
It does not mint an OriginalLaunch reset cohort or recovery witness, and cannot
acquire C2/C2R eligibility. Do not fabricate an OrdinaryLaunchRequired recovery
result for an unobserved non-Hub barrier state; unavailable recovery-only getters
may continue to reject outside their domain. Existing historical non-Hub
unsafe-root logged-exception assertions must be restored, not weakened or
converted into unauthorized recovery. Separate production-authored Hub tests
verify the already-approved readable ManualRepairRequired override.

The correction is bounded to the adapter/router and applicable launch/recovery
tests already allowlisted. C1, ordinary preparation, proof checks, public ABI,
assembly/configuration and product behavior outside this scope remain unchanged.
Astra approves this narrowing after Luna's independent source/execution
counter-review; fresh source review and successful execution are still required.

### Astra final integration — Verified, 2026-09-28

Approved execution snapshot SHA-256:
`2BB53A16B9E7D9FA6B9A8C50CA928BEFEF353C137E0DD97C03BF2FE430112E74`.
Astra accepts all AC-M5D7QC2R-001 through -008 on the unchanged frozen source,
after Luna's final independent acceptance digest P0=0/P1=0. This closes parent
AC-M5D7QC2-007; the earlier OPEN paragraphs are preserved historical design and
approval records, superseded by this evidence-backed integration only.

Actual R26–R30 distinct-process evidence proves prepared restart and ordinary
startup after death following barrier deletion. R29 is an owned-PID death
observation, not an XML/NUnit pass. Current fresh regressions are R34 536 plus
disjoint R25 74 PlayMode names, and R35 non-worker 562 plus R36 fresh worker 51
EditMode names, all failed/skipped/inconclusive 0 with exact names and unchanged
source closure. R31/R32/R33 remain focused evidence, not inflated distinct
totals. Historical failed executions and old manifests are not rewritten.

The accepted result remains UIOnlyBlocked. No live menu, destination, map enable,
scene, gameplay, archive restoration or user-facing retry is authorized.
