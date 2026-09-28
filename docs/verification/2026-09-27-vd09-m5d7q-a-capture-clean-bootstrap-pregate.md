# M5D7Q-A clean-bootstrap addendum — Luna narrow pre-gate

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: proposal-only pre-gate; no Unity execution and no implementation edit
- Proposal: `docs/proposals/2026-09-27-vd09-m5d7q-a-capture-runtime-blocker-amendment.md`
- Proposal SHA-256: `26BD7211C3C68AB68ADEF0941DFD712A8A5BD2462ED89CB80AF1F1D3B4A6D22F`
- Review contract: `docs/specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md`
- Review contract SHA-256: `CF75AAAA7A56859ED0EAB0DC3E329B67C69EBA8DB42EA979DC80F5E5B0E7F54A`
- Implementation baseline (not accepted by this pre-gate): `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationCaptureGenerator.cs`
- Baseline SHA-256: `B499A12F1C8AA2E04CF330D6B3DBB557257ABA706E6DC8133A148ABF6AD96214`

## Verdict

**PASS — P0=0, P1=0, P2=0.** The bounded addendum is feasible and sufficiently
deterministic for Terra implementation after Astra restores the contract to
`Approved`. Astra may restore `Approved` without a new user product decision;
the proposal explicitly preserves product/runtime semantics and only changes
Editor evidence classification and process-boundary handling. This PASS is not
implementation acceptance or `Verified` status.

## Checks

### Pre-predicate snapshot and classification

- The proposal requires the snapshot before scene classification, scene opening,
  temporary-directory creation, or other mutation.
- Marker field order is fixed and complete: setup count, scene count, active
  validity/handle, JSON-escaped path/name, loaded/dirty, invariant build index,
  and root count. Invalid active scenes have deterministic handle/path/name,
  loaded/dirty, build-index, and root-count defaults.
- `TrueZeroScene` is exactly `sceneCount == 0` plus invalid active scene.
- `CleanUnsavedBootstrap` is exactly one valid, loaded, active scene matching
  the active handle, empty path, clean state, build index `-1`, and zero roots.
  The transient scene name is recorded but correctly not used as an admission
  predicate.
- All other states fail closed with sorted, enumerated command-boundary and
  scene-shape codes before Hub, files, or capture state are changed.

### Process boundary and immutability

- Terminal mode explicitly does not claim to restore the impossible zero-scene
  or transient bootstrap state. It leaves one clean in-memory Hub until the
  mandated `-quit` process boundary and records both restoration flags false.
- The proposal forbids save/set-dirty operations, requires authored dependency
  SHA-256 equality, retains global/view/temporary-object cleanup proofs, and
  keeps non-empty setup restoration unchanged.
- Failure before publication removes only temporary output; post-stage proof
  failures remove the new final and restore `.prev -> final`, including the
  no-prior-final case. Rollback failure remains an aggregate exit-1 stop.

### Evidence and traceability

- `resolution-capture-retry-h.log` is the single new allowlisted stem; prior
  retry-e/f/g evidence remains immutable and is not reinterpreted.
- The addendum retains the existing TMP diagnostics/predicate, two-pass
  same-process byte equality, fresh-process equality, capture set, manifest,
  dependency allowlist, and GUID/hash rules.
- REQ/AC IDs and mappings remain unchanged (`REQ-M5D7QA-001..010`,
  `AC-M5D7QA-001..010`); the changed evidence is correctly traced to
  `REQ-M5D7QA-006/008/009` and `AC-M5D7QA-007/010`.
- No copy, font, size, layout, color, interaction, runtime, capture-list, or
  manifest-schema decision is introduced. Any product-changing repair would
  require a separate Astra contract delta and, if applicable, user approval.

## Gate state and implementation note

The contract currently has `status: Review` specifically for this bounded
clean-bootstrap addendum. The existing generator baseline is intentionally
still true-zero-only and therefore is not evidence that the addendum is
implemented. Terra may implement only the exact proposal after Astra's
re-approval; Luna must then perform a source pre-gate and independent runtime
post-review.
