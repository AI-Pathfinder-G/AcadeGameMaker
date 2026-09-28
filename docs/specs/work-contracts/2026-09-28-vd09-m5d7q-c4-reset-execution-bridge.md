---
status: Review
---

# VD-09 M5D7Q-C4 confirmed reset execution bridge

- Date: 2026-09-28
- Status: Review — bounded design only; no implementation authority
- Owner and approval: Astra
- Bounded design: Sol; intended implementation: Terra; independent QA: Luna
- Parent: `2026-09-28-vd09-m5d7q-c-new-game-reset.md` (Approved)
- Required predecessors: C1 Verified; C2 Approved and independently verified
  before C4 execution; C3 Approved with its independently accepted pre-C4 gate
  and frozen source/API before C4 intake, as specified by ADR-0036 below
- Parent trace: `REQ-M5D7QC-001/003/004/005/006/007`,
  `AC-M5D7QC-002/003/004/005/006/007`

## Purpose and boundary

C4 is the synthetic execution bridge from one C3-confirmed request through one
C1 Begin attempt and, only for C1's exact `DiskPrepared` result, one C2
`FinalizeReset` attempt on the same authenticated launch root and active
adapter/router pair. It owns no confirmation policy, profile interpretation,
disk algorithm or memory-cutover algorithm.

C4 does not create or wire live confirmation UI, prefabs, scenes, destination
acceptance, map enablement, gameplay, run, wardrobe/media, Settings, Quit,
ordinary save, archive cleanup/restoration or notification. A completed C4
receipt remains `UIOnlyBlocked`; no old menu is resumed after C2.

## Assembly direction and entry

The lower neutral executor is internal to `AcadeGameMaker.Input.Unity`:

```csharp
ProfileResetExecutionResultV1 ExecuteConfirmedReset(
    ConfirmedProfileResetRequestV1 confirmed,
    DesktopProfileLaunchAdapterV1 sessionOwner,
    InputRouter router);
```

`ConfirmedProfileResetRequestV1` is an Input.Unity-owned opaque, immutable,
one-consumer value created by C3's already-approved lower observation boundary.
It privately contains the exact C1 `ProfileResetConfirmationIdentityV1`,
authenticated launch root, adapter/router identities and C3 owner/interaction/
decision generations. Hub.Presentation can retain and return the opaque value
but cannot extract, replace or construct its C1 identity.

The C3 owner first performs `CommitExecution`, atomically moving exact
`ConfirmedReady -> ExecutionCommitted` and invalidating every confirm, cancel,
capture-retry and rearm capability. Only then does it call the lower executor
with the opaque confirmed value and the exact adapter/router. The executor has
no Hub request, presenter, handoff-owner, confirmation-owner, UI or callback
parameter and no reference to a Hub.Presentation type. Assembly direction
remains Hub.Presentation -> Input.Unity -> Profile; no public ABI, asmdef or
friend change is authorized.

## Exact eligibility and upfront execution guard

Before C1, the executor validates the confirmed value's complete invariants and
one-consumer state without consuming it, then validates:

- the exact active launch adapter/router reference pair and their independent
  launch-established identity witnesses;
- one valid immutable historical desktop launch receipt;
- the exact normalized launch-root witnesses equal the confirmed root;
- initialized, unfaulted HubUIOnly ownership with no pending launch, prior
  reset, C2 gate, foreign owner or disabled/destroyed object; and
- the exact C3 generation recorded as execution-committed by the opaque lower
  capability, without inspecting any Hub type.

Clean pre-entry rejection consumes nothing and changes no runtime state.
Reflection corruption may invoke existing fail-closed containment and is never
reported Busy, Stale or Completed.

After all preflight succeeds, the executor consumes the confirmed value once
and enters one owner-authenticated `ResetExecutionPending` guard on both the
adapter and router before calling C1. The guard is bound to the confirmed
identity, root, pair and one shared execution generation. It immediately
refuses menu-intent/notification/ordinary-save production, semantic-frame
publication and further reset/launch entry. It suppresses callback capture but
does not enable/replace actions, change profile memory, publish a reset receipt,
or claim a durable barrier. Existing receipts/frames remain forensic history,
not current interaction authority.

The guard is an execution-safety boundary, not C2 success and not Cancel. Its
two halves validate together. Any partial entry, exception or reflected
generation mismatch fail-stops both owners. No caller can release it directly.

## C1 invocation and exact result ownership

The bridge passes only the confirmed request's private exact C1 identity and
authenticated root to the existing real `ProfileResetDiskTransactionV1.Begin`.
It calls Begin exactly once and validates the returned
`ProfileResetDiskResultV1` as a complete closed row. It never reconstructs a
result from outcome, transaction ID, hashes or snapshots.

The result matrix is:

- `Busy/NoBarrier`: C1 proved no mutation. C4 records the exact C1 result,
  releases only the pre-barrier execution guard into `FreshC3Required`, and
  returns `Busy`. C3 must open a fresh interaction/cursor baseline before an
  explicit new attempt; the consumed confirmed request is never retried.
- `ConfirmationStale/NoBarrier`: C1 proved no barrier publication. C4 records
  the exact result, releases only into `FreshC3Required`, and returns
  `ConfirmationStale`. C3 captures and re-gates a new epoch. No stale identity
  or confirmed request is reused.
- `ManualRepairRequired`: terminal, regardless of whether authenticated marker
  evidence is present. The execution guard becomes permanent fail-stop before
  returning; no Cancel, old-menu rearm or same-session retry is legal.
- `ReloadRequired`: terminal because C1 may have published a barrier or left a
  commit uncertain. The guard becomes permanent fail-stop before return.
- `DiskPrepared`: C4 retains the whole validated C1 result and
  extracts its internal prepared proof exactly once only for the immediate C2
  call below. No copied transaction/root/hash scalar is authority.

Default, unknown, mismatched, reflection-corrupt or exception rows are terminal
fail-stop, never Busy/Stale. C4 does not infer pre/post-barrier state from file
existence, exception type, transaction ID presence or a best-effort reprobe.

## C1-to-C2 handoff and lease-gap safety

C1 releases its root lease before returning. C4 does not widen C1 to retain or
transfer that lease. The authenticated durable reset barrier is the disk safety
boundary during the intentional C1-to-C2 lease gap: ordinary load/save and old
automatic recovery remain blocked, and C2 must reacquire and fully
reauthenticate its own live lease.

The in-session execution guard is the memory/input safety boundary during the
same gap. Unity does not receive a frame between the synchronous C1 return and
C2 invocation. Even if result validation, proof access or C2 entry throws, the
guard prevents the old session from publishing input/menu/save authority.

For an exact `DiskPrepared` row, C4 validates that the complete result, prepared
proof, confirmed root and guarded adapter/router generation agree, then calls:

```csharp
ProfileResetMemoryCutoverV1.FinalizeReset(
    exactRoot,
    diskResult.PreparedProof,
    sessionOwner,
    router);
```

C2 remains the sole authority to reacquire the root, authenticate the proof and
live marker/archive/active row, stage defaults, enter the irreversible terminal
cutover, apply memory, remove the barrier, final-reprobe and mint its receipt.
C4 cannot inspect or manufacture the private proof and cannot substitute a
different pair/root between C1 and C2.

The C2 result maps without reinterpretation:

- `Completed` -> C4 `Completed`, carrying the exact C2 receipt and complete C1
  result correlation; the session remains `UIOnlyBlocked`;
- `Busy`, `ReloadRequired` or `ManualRepairRequired` after C1 DiskPrepared ->
  terminal C4 `ReloadRequired` or `ManualRepairRequired` as applicable. Even a
  C2 Busy is post-barrier and cannot rearm the old menu or retry in-session.

C4 never treats a post-C1 `DiskPrepared` failure as pre-barrier Busy/Stale and
never calls C1 Resume in the same session.

## Exception and teardown boundary

Before calling C1, all C3 decision authority and the confirmed value are
consumed and both execution-guard halves are latched. Any exception after that
point is handled as follows:

- only a fully validated C1 `Busy/NoBarrier` or
  `ConfirmationStale/NoBarrier` result permits `FreshC3Required`;
- every other exception or uncertain result permanently fail-stops the exact
  adapter/router pair before control returns or their ownership is released;
- if C1 may have published a barrier, old profile/actions may remain as
  forensic references but cannot publish, save, process input, rearm, retry or
  become writers again;
- Disable/Destroy preserves terminal history and cannot manufacture a
  pre-barrier result, dispose foreign actions, or double-run C1/C2.

Programmer/protocol exceptions may escape after fail-stop containment. Fatal
CLR/process termination relies on C1's durable barrier and restart rules; C4
does not claim that managed cleanup ran after process death.

## Closed output

`ProfileResetExecutionOutcomeV1` is closed to `Completed`, `Busy`,
`ConfirmationStale`, `ReloadRequired`, and `ManualRepairRequired`.

`ProfileResetExecutionResultV1` always carries the exact execution generation
and a closed phase (`PreC1`, `C1Returned`, `C2Invoked`, `Completed`). Only:

- `Busy` carries the exact validated C1 Busy row and fresh-C3 handback proof;
- `ConfirmationStale` carries the exact validated C1 stale row and fresh-C3
  handback proof;
- `Completed` carries the exact validated C1 DiskPrepared correlation and exact
  C2 completed receipt;
- terminal failures carry bounded phase/correlation evidence but no C1 identity,
  prepared proof, current document or success-like receipt.

Every getter revalidates enum, nullability, pair/root/generation correlation
and nested typed results. The prepared proof never escapes. Default, unknown,
foreign, duplicated or reflection-corrupt rows throw. Receipt publication in
C4 is only result composition; the authoritative reset receipt remains C2's
one-shot adapter publication.

## Requirements

- **REQ-M5D7QC4-001:** Consume one exact C3 confirmed request only after C3
  execution commit invalidates all decision/rearm authority.
- **REQ-M5D7QC4-002:** Enter an exact pair/root/generation-bound execution guard
  before C1 so exceptions or a durable barrier cannot revive old writers.
- **REQ-M5D7QC4-003:** Invoke C1 Begin exactly once with the original identity
  and preserve its complete typed result without scalar reconstruction.
- **REQ-M5D7QC4-004:** Permit fresh C3 re-gating only for validated C1 Busy or
  ConfirmationStale NoBarrier rows; all uncertain/potential-barrier rows are
  terminal with no Cancel, rearm or retry.
- **REQ-M5D7QC4-005:** Pass an exact C1 DiskPrepared proof/root/pair directly to
  one C2 Finalize attempt and preserve C1/C2 typed correlation.
- **REQ-M5D7QC4-006:** Keep the C1-C2 lease gap safe through the durable barrier
  plus execution guard and never make old memory/profile authority usable.
- **REQ-M5D7QC4-007:** Grant no UI, destination, scene, map enable, gameplay,
  media, ordinary-save, archive cleanup or unrelated product authority.

## Acceptance criteria

- **AC-M5D7QC4-001:** Exact, duplicate, consumed, foreign-root/pair/generation,
  default and reflection-corrupt confirmed requests plus C3-not-committed rows
  prove no C1 call before one valid execution commit.
- **AC-M5D7QC4-002:** Faults before/between/after both execution-guard latches
  prove no semantic frame, menu intent, notification or save publication and
  permanent containment on partial entry.
- **AC-M5D7QC4-003:** C1 call-count and identity-reference tests prove one real
  Begin with the original identity; outcome/state/nested-result corruption and
  scalar substitution cannot advance or reach C2.
- **AC-M5D7QC4-004:** Exact C1 Busy and ConfirmationStale rows alone produce a
  fresh-C3 handback and fresh cursor baseline; the consumed request/identity is
  never retried, while ManualRepair/Reload/exception rows never rearm.
- **AC-M5D7QC4-005:** Exact DiskPrepared passes the same root, proof object and
  adapter/router identities to one C2 call; foreign/replaced/consumed proofs,
  pair drift and root drift fail terminally before C2 mutation.
- **AC-M5D7QC4-006:** Injected faults after every C1 durable checkpoint/result
  return and before/at C2 entry prove the barrier blocks disk writers and the
  guard blocks old memory/input publication throughout the lease gap.
- **AC-M5D7QC4-007:** C2 Completed/Busy/ReloadRequired/ManualRepairRequired and
  thrown-entry matrices prove only Completed carries the exact receipt; every
  post-DiskPrepared noncompletion is terminal and cannot Cancel/rearm/retry.
- **AC-M5D7QC4-008:** Disable/destroy/reentry and concurrent/reentrant execution
  attempts prove exact-once C1/C2 invocation, no double proof consumption and
  immutable forensic history.
- **AC-M5D7QC4-009:** Static/API review proves the executor references no Hub
  type and adds no public ABI/asmdef/friend; it cannot reach UI/prefab/scene,
  map enable, gameplay/run, wardrobe/media, Settings/Quit, ordinary save,
  archive restore/cleanup, network, clock or random authority.
- **AC-M5D7QC4-010:** Focused and required regressions have zero failed,
  skipped and inconclusive tests; Luna reports P0=0/P1=0 before integration.

Traceability: `001 -> AC-001/008`; `002 -> AC-002/006`; `003 -> AC-003`;
`004 -> AC-004/007`; `005 -> AC-005/007`; `006 -> AC-006/008`;
`007 -> AC-009/010`. All identifiers use full `AC-M5D7QC4-*` form.

## Proposed implementation allowlist after approval

After Astra changes this contract to Approved, implementation may only:

- add
  `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs`
  and `.meta` for the lower neutral executor/result/guard capability;
- modify C3's allowlisted Input.Unity confirmation source only to expose the
  opaque one-consumer confirmed value's private execution-consumption boundary;
- modify `DesktopProfileLaunchAdapterV1.cs` only for the execution-pending/
  fresh-C3/terminal guard half and exact generation validation;
- modify `InputRouter.cs` only for the reciprocal guard half, callback capture/
  semantic-publication refusal and exact generation validation;
- modify `ProfileResetMemoryCutoverV1.cs` only if necessary to accept and
  validate the already-guarded exact pair as C2's same terminalizing entry; C2
  behavior, proof authentication, result and receipt do not change;
- modify C3's allowlisted Hub.Presentation owner source only for
  `CommitExecution`, lower-executor invocation and typed Busy/Stale/terminal
  report intake; no Hub type may move into Input.Unity;
- add focused Input.Unity EditMode/PlayMode and Hub.Presentation.Unity synthetic
  tests, `.meta`, evidence and one minimal `docs/README.md` link.

### Conditional Q0 evidence-maintenance gate

C4 remains Review. Its eventual runtime allowlist does not by itself authorize
editing the Q0 test audit. If the independently reviewed implementation changes
a pinned Adapter or Router file, Astra must separately approve a narrow
amendment for
`Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs`
before Terra edits that audit. Append exactly one strict current-file successor
per changed file, with its exact SHA-256, C4 contract/source provenance and
independent Luna review. Preserve all nine original historical rows, seven
legacy-current rows, both C2/C2R provenance rows and any independently accepted
C3 successor evidence unchanged. Current-file verification must select only
the exact reviewed current successor, never accept old-or-new hashes, ranges,
aliases or fallback rows. An unchanged file retains its most recent independently
accepted predecessor pin (including C3 where applicable), not an assumed C2 pin.
No successor or audit change is authorized for an unchanged file. Record the
audit's own exact hash and fresh required regression evidence for
`AC-M5D7QC4-009/010`. This gate changes neither runtime authority nor acceptance
criteria and introduces no product decision.

No Profile source, C1 algorithm/result/proof, C2 Profile authority, Q-A/Q-B
source, asmdef, friend, public ABI, generated actions/input asset, scene,
prefab/UI/media, Packages, ProjectSettings or unrelated file may change.

Internal deterministic checkpoints may throw only at confirmed consumption,
both guard latches, before/after real C1 Begin, result validation/proof take,
before/after real C2 Finalize, result mapping and terminal/fresh-C3 publication.
They may not substitute identities, roots, pairs, C1/C2 results or receipts.
Production calls each real lower operation exactly once; no fake filesystem,
alternate reset algorithm or global mutable hook is allowed.

## Feasibility and stop conditions

The current lower direction is feasible: Input.Unity already has Profile friend
access, owns the adapter/router and C2 coordinator, and Hub.Presentation already
references/friends into Input.Unity. C1 exposes a validated result whose
`PreparedProof` is internal, and C2 accepts that exact internal proof. Keeping
the bridge in Input.Unity avoids exposing either proof or C1 identity publicly.

Implementation must stop for Astra if the final C2 implementation does not
offer a stable exact `FinalizeReset(root, proof, adapter, router)` boundary, if
its eligibility cannot distinguish the C4 execution guard without weakening
C2, if C3 cannot provide a one-consumer opaque lower confirmed value, or if
correctness requires changing C1/Profile, adding a reverse Hub reference,
keeping the C1 lease across calls, re-enabling a map, or widening public ABI.
C2 is currently under implementation and is not a frozen prerequisite; C4 may
not be Approved or implemented against an assumed final source shape.

## Product decisions

No new user product decision is required for this bridge. The parent already
fixes confirmation, manual-only indefinite archive, exact default reset and no
success/destination before validation. Astra owns the technical decision to use
the execution guard and typed bridge.

A new user decision would be required to allow Cancel or old-menu reuse after a
possible barrier, retry C1/C2 in-session, clean/restore archives, reset costume
storage, or treat C2 completion as permission to enter a scene/gameplay flow.

## Delivery gate

Luna pre-review must cite every `AC-M5D7QC4-*`, verify lower assembly direction,
the C1-C2 lease-gap guard, exact result/proof flow and pre/post-barrier outcome
split. Astra alone may change Review to Approved, and only after C2's exact
implemented boundary is frozen and independently accepted. Terra then
implements with `REQ-M5D7QC4-*` citations; Luna independently verifies; Astra
alone accepts integration. This draft authorizes no runtime edit.

### 아스트라 선행 조건 명확화 — 2026-09-29, 상태 Review 유지

[ADR-0036](../../adr/0036-staged-confirmation-and-execution-acceptance.md)과 Approved C3의 단계적 수용 보정에 따라 C4 준비의 C3 선행 조건은 합성 선행 단계의 독립 수용 및 정확한 소스·API 동결이다. 이 단계는 C3 전체 Verified가 아니며 C3 AC-M5D7QC3-007/008은 Open/Not Verified로 남는다. C3 AC-M5D7QC3-010의 해당 집중·필수 회귀와 루나 P0/P1=0, 아스트라 선행 단계 수용을 생략할 수 없다.

C4 자체는 Review이며 별도 아스트라 승인 전 구현을 허용하지 않는다. Approved C4의 실제 실행 결과가 준비된 뒤 C3 잔여 AC-M5D7QC3-007/008 및 C4 AC-M5D7QC4-001..010 전체를 공동 최종 검증한다. AC-009 정적/API·권한 검수와 AC-010의 현재 소스 집중·필수 편집 모드 및 실행 모드 회귀도 유지한다. 기존 결과 출처, 실행 가드, C1/C2 경계와 제품 동작은 바꾸지 않는다.
