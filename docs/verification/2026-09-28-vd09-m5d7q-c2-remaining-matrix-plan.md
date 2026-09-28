# M5D7Q-C2 remaining matrix plan

Status: planning only; not execution evidence or acceptance.

This closes the observable gaps against the Approved C2 contract without
inventing a rollback row.  Each faulting case uses a newly created real hub
launch, a newly created C1 `DiskPrepared` proof, and a fresh OS-temp root.  A
terminal case never supplies its proof, adapter, router, candidate, or root to
another case.  Controls only throw at the named production boundary or wrap the
exact owned action; they never manufacture a proof, binding result, action, disk
observation, or lifecycle flag.

## C2 checkpoints — 19 fresh cases

For each `ProfileResetMemoryCutoverCheckpointV1` value, inject once and assert
the closed result (`ReloadRequired`, except the existing post-lease fake-Busy
row, which must still be `ReloadRequired`), no reset receipt, both original
owners terminal, no usable current cell/frame/notification, and one-shot
candidate ownership.  Assert the historical launch/router frames are unchanged.

| Phase | checkpoint cases | disk assertion |
|---|---|---|
| stage/defaults | `BeforeStaging`, `AfterCandidateCreate`, `BeforeBindingApply`, `AfterBindingApply` | barrier present |
| terminal/pair | `BeforeTerminalGate`, `AfterTerminalGate`, `BeforeTransfer`, `AfterTransfer`, `BeforeCell`, `AfterCell`, `BeforePair`, `AfterPair` | barrier present; no usable partial pair |
| durable finalization | `BeforeDelete`, `AfterDelete`, `BeforeFinalProbe`, `AfterFinalProbe`, `BeforeReceiptStage` | present before a successful delete; **absent** after `AfterDelete` and later cases; never recreate it |

`BeforeOldDispose` and `AfterOldDispose` are included in the terminal/pair
matrix, but retain the existing real-CWT claim test as the ownership-specific
proof.  The completed row has no after-publication checkpoint: receipt/result
staging is validated before the final receipt assignment by contract.

## Exact owned operations — 22 fresh before/after cases

For `CancelUiEnableQuarantine`, `DisableOldGameplay`, `DisableOldUi`,
`RemoveOldGameplayCallbacks`, `RemoveOldUiCallbacks`, `DisposeOld`,
`DisableNewGameplay`, `DisableNewUi`, `RemoveNewGameplayCallbacks`, and
`RemoveNewUiCallbacks`, run the closed operation control once before and once
after its supplied exact delegate.  It records at most one wrapper call and at
most one exact delegate invocation; foreign cohort maps/callbacks stay
untouched.  These are terminal pre-delete rows, so their barrier remains.

`DisposeNew` is cleanup-only rather than a successful transfer operation.  Use
forced post-claim/pre-transfer faults to cause its real cleanup path, then
inject before/after its exact `Dispose` delegate.  Assert one close attempt,
`ReloadRequired`, no receipt and retained barrier.  Do not count it as a normal
successful operation or substitute a candidate/action in the control.

Approximate additions: 19 checkpoint cases + 20 old/new map/callback operation
cases + 2 cleanup-dispose cases = 41 rows; existing operation/CAS rows remain
regressions, not replacements.

## Durable and final-pair rows

Drive the existing real Profile authority controls through `FinalizeReset` for
before/after delete and final probe.  Add real marker changed/missing/locked/
directory/reparse cases at C2 level.  Before-delete failures retain the barrier;
after a real delete or final-probe failure it stays absent, while the terminal
memory owners expose no receipt or retry.

Add one fresh C2 row for each final reprobe mutation: primary, previous, temp,
both barrier names, each declared archive presence/content, and a post-transfer
memory disagreement (router action/generation, cell/default binding).  Each
must close the original cohort, return no completed evidence, and make no
barrier-rebuild claim.  Final proof/receipt reflection remains detached-data
testing and must not be used to fake these disk rows.

## Restart/resume assessment

The frozen C1 API exposes `ProfileResetDiskTransactionV1.Resume(root)`, which
can issue a fresh `DiskPrepared` proof; this is the valid restart input.  The
existing adapter test preparation calls `ProfileLaunchPreparationCoordinatorV1`
and no inspected public launch result was found that itself hands C2 a resumed
proof.  Therefore the proposed bounded PlayMode row is: create C1 barrier,
discard the first launch/cohort as a simulated process boundary, call
`Resume(root)`, construct one new real hub launch against that root, and call C2
once with the resumed proof.  If the new launch is blocked by the barrier before
that proof can be passed, stop for Astra: a C2-compatible resume handoff is
missing and must not be fabricated in a fixture.  A second fresh launch after a
successful C2 must observe ordinary exact-r0 state and have no C2 receipt to
replay.

## Requirement map

- REQ-M5D7QC2-001: fresh proof/root/lease and restart rows — AC-001/007.
- REQ-M5D7QC2-002: stage/default checkpoint and cleanup-dispose rows — AC-002.
- REQ-M5D7QC2-003/004: gate, pair, all owned-operation and containment rows —
  AC-003/004/008.
- REQ-M5D7QC2-005/006: delete/final-reprobe matrix and receipt-stage row —
  AC-005/006.
- REQ-M5D7QC2-007: history/no-replay assertions on every terminal and restart
  row — AC-008/009.  Suite execution and independent review remain AC-010 work.
