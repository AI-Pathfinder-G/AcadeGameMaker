# VD-09 M5D7Q-C3 epoch-handshake counter-review

- Date: 2026-09-28
- Reviewer: Sol (`gpt-5.6-sol`), bounded architecture counter-review
- Authority: advisory only; Astra retains contract and integration authority
- Scope: current Q-A/Q-B ordering against `REQ-M5D7QC3-001/005/006` and
  `AC-M5D7QC3-001/005/006/008`
- Claim: no implementation, execution, contract amendment or acceptance

## Finding

No behavioral-contract blocker was found. The existing Q-A/Q-B fields cannot,
however, be reused as the successor lifecycle: Q-A's `_state` is already
`IntentRetained`, Q-B's `_state` is already `RequestTaken`, and their retained
intent/taken-request fields are history. Terra should add an orthogonal
successor state plus append-only epoch records while leaving those existing
states and fields unchanged. This is the concrete condition for satisfying
`REQ-M5D7QC3-005` and `AC-M5D7QC3-005/006`.

## Private actual-take seam

Add a C3-only internal Q-B take overload; names below are illustrative. It must
receive the exact configured `NewGameConfirmationOwnerV1`, presenter and router
references and return one private reference-type issuance, not a request-shaped
proof. Do not discover an owner from a GameObject or authenticate it by an equal
`HubMenuIntentRequestV1` value.

Inside the existing `TryTakeRequest` transition, the C3 overload should order
work as follows:

1. Validate Q-B topology/state and exact reference bindings, then read and
   validate the real `_request`; reject any item other than `NewGame`.
2. Build locally an issuance record bound to Q-B, Q-A, C3, router, current
   epoch and the real request. Its constructor is private to Q-B. Request
   equality is payload integrity only; authority is the exact issuance object
   still registered by Q-B.
3. Publish `_takenRequestProof`, `_takenRequest`, issuance proof/reference,
   clear only the live `_request` slot, and set `_state = RequestTaken` last.
   Assign the out value only after final validation. Any exception closes the
   participants and exposes no issuance; it never rolls the request back.

The initial `_takenRequest` row remains the original immutable Q-B history.
Later epochs use append-only immutable nodes containing epoch, request and
issuance references; they never overwrite `_takenRequest`. Q-A should use the
same pattern for retired controller/cursor/intent rows. This gives repeated
Cancel cycles monotonic evidence without copying request authority
(`REQ-M5D7QC3-001`, `AC-M5D7QC3-001`).

## Exact cancellation-successor call order

Keep decision generation and Q-B interaction epoch separate. Busy retry and a
fresh decision after recapture do not advance Q-B. For an accepted Cancel:

1. C3 wins the Pending-to-consumed CAS, records `Cancelled`, and creates one
   rearm capability bound by reference to the cancelled issuance and exact
   C3/Q-A/Q-B/router cohort.
2. C3 enters `Rearming` and calls one Q-B-owned rearm seam. Q-B validates the
   capability against its registered issuance, calculates `checked(epoch + 1)`
   locally, then publishes the new counter/proof and a reserved successor node.
   Overflow or mismatch is terminal; the counter is never decremented.
3. Q-B synchronously calls Q-A with the exact reserved successor token. Q-A
   validates the same references and epoch, snapshots the old controller's
   notice state, creates a controller from the unchanged `_handoff` and
   `_notification`, and, only when the old notice is `Dismissed`, applies the
   existing dismissal method to the fresh controller. It creates a cursor from
   the exact existing `_router`, sets fresh focus to `NewGame`, clears only
   fresh hover/pending input, and publishes an owner-bound baseline-pending
   witness. The old controller, cursor and intents are linked into immutable
   history before active references change.
4. Q-A returns a private acknowledgment for that exact token. Q-B validates it
   and moves only its orthogonal successor state to `AwaitingIntent`; its main
   `_state` remains `RequestTaken`. C3 then validates the acknowledgment,
   consumes the rearm capability and moves to `AwaitingRequest`. No Unity
   callback can interleave this synchronous sequence.
5. A later successor selection is retained in Q-A's current epoch node. Q-B's
   `LateUpdate` polls that orthogonal slot, not the old
   `TryTakeRetainedIntent` history, and the C3-only take overload issues the
   next actual-take capability for the already-published epoch.

No step replaces actions, enables a map, reloads the handoff/notification, or
loads a scene/prefab. Q-B may call Q-A because both already share the Hub
Presentation assembly and reciprocal reference; no reverse assembly reference
is introduced (`REQ-M5D7QC3-005`, `AC-M5D7QC3-005`).

## Cursor quarantine must precede ordinary promotion

The present `Update` promotion
`AwaitingBaseline && _cursor.State == Ready -> Ready` is valid only for the
initial presenter path. The successor-pending branch must run first and return;
ordinary promotion must be guarded by “no successor exists.” In particular, a
fresh cursor created already `Ready` remains quarantined.

`FixedUpdate` must likewise dispatch successor state before the old
`Ready/IntentRetained` branch:

- while the exact baseline witness is pending, call only
  `freshCursor.TryAdvance(exactRouter, out frame)`;
- `false` keeps the successor quarantined, including the cursor's own first
  baseline acquisition;
- `true` requires a valid consecutive frame, which is deliberately discarded;
  do not call `Interpret`, `Activate`, hit testing or notification dismissal;
- only after discarding that frame consume the witness, publish successor
  `Ready`, and render it interactable. The following frame is the first one
  eligible for delivery.

A foreign cursor/router, nonconsecutive or malformed frame, mutated witness,
disable/destroy, or Q-A/Q-B epoch disagreement terminally closes the successor.
There is no fallback to the old cursor merely because Q-A's historical
`_state` is still `IntentRetained` (`REQ-M5D7QC3-005`,
`AC-M5D7QC3-006`).

## Failure and execution stop rules

- Failure before Q-B publishes the next epoch closes the attempted handshake
  without a successor. Failure after publication preserves the advanced epoch
  and every history node, closes C3/Q-A/Q-B, and never rolls back.
- Failure after Q-A creates its controller/cursor but before the final
  acknowledgment retains them as failed forensic history and never exposes
  them to `Update`, `FixedUpdate` or `LateUpdate`.
- A `false` cursor advance is waiting, not failure. A thrown/skipped/malformed
  advance is terminal. Partial peer closure is not recoverable by another
  rearm capability.
- Execution commit must validate and consume the exact confirmed request, CAS
  C3 to `ExecutionCommitted`, invalidate decision/retry/rearm records, and
  close Q-A/Q-B interaction before returning any private executor permit. If
  closure or permit publication fails, C1 is not callable and authority is not
  restored. Only the returned exact permit may precede a later C1 `Begin`;
  C3 itself does not call C1 (`REQ-M5D7QC3-006`,
  `AC-M5D7QC3-008`).
- Pre-barrier Busy/ConfirmationStale may enter only their typed owner path.
  ReloadRequired, ManualRepairRequired, or any possible barrier publication
  terminally forbids Cancel, rearm and old-menu interaction
  (`REQ-M5D7QC3-006`).

## Blockers

The design has no new contract or assembly blocker. Implementation remains
blocked from touching the shared Q-A/Q-B/Profile/launch sources while the
current C2/C2R run owns its frozen Assets snapshot; Astra must release that
allocation first. Stop for Astra if Terra cannot preserve the original
`IntentRetained`/`RequestTaken` rows, needs value-equality owner checks, cannot
place successor-pending ahead of ordinary `Update` promotion, or needs any
public ABI, reverse assembly reference, map enable, scene/prefab load, or C1
call before execution authority is invalidated.

## Luna counter-review — C4 interaction (2026-09-28)

Against Approved C3 `REQ-M5D7QC3-001/005/006/007` and
`AC-M5D7QC3-001/005/006/007/008`, plus C4
`REQ-M5D7QC4-001/002/004/006` and `AC-M5D7QC4-001/004/006/008`, no new P0/P1
blocker was found. The proposed private Q-B take seam preserves actual
`TryTakeRequest` provenance through exact owner/router/presenter bindings and
the real request; payload equality is not used as authority. The immutable
original `_takenRequest`/`RequestTaken` history remains untouched while the
successor epoch is orthogonal and append-only.

The stated cursor quarantine is contract-compatible: `false` is waiting,
`true` is the first consecutive frame and is discarded before interaction;
the next frame is the first deliverable frame. The execution-commit sequence
also correctly closes peers before producing the opaque one-consumer executor
permit, while retaining only typed pre-barrier Busy/ConfirmationStale paths
for a fresh epoch. It does not permit double consumption of the confirmed
lower value or reuse of the cancelled epoch. This is a design review only;
C3/C4 implementation and acceptance remain pending.
