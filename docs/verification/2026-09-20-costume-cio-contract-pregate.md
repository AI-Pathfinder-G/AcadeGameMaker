# Costume CIO file adapter — Luna contract pre-gate

- Date: 2026-09-20
- Status: **PASS WITH P2 CLARIFICATIONS — suitable for Astra approval; no implementation authority granted by this review**
- Reviewer: Luna (independent contract review)
- Reviewed: `docs/specs/work-contracts/2026-09-20-costume-cio-file-adapter.md`
- Parent references: `docs/specs/work-contracts/2026-09-13-costume-presentation-and-sprite-production.md` and `docs/specs/work-contracts/2026-09-13-costume-io-and-unity-adapter.md`
- Implemented seam: `Assets/AcadeGameMaker/Runtime/Costumes/CostumeCoreV1.cs`
- Excluded from this review: runtime edits, filesystem implementation, execution of tests, media or Unity acceptance.

## Decision

**PASS for Astra's bounded contract approval, with the P2 clarifications below.** No P0 or P1 finding blocks this child. The CIO contract is sufficiently separated from the still-Review Unity adapter and wardrobe composition. This report is not CIO approval or evidence of implementation.

## Findings

### P0 — none

### P1 — none

### P2 — clarify during approval or implementation handoff

1. **Read failure is not representable by the pure recovery input.** `CostumeRecoveryInputV1` carries only nullable byte arrays; `CostumeRecoverySelectorV1.Recover` treats null, invalid, and absent bytes alike and bootstraps from accepted defaults when none decode. The CIO contract correctly requires `Missing`, `Read`, and `ReadFailed` to remain distinct and forbids bootstrap over uncertain unread data (REQ-CIO-002/003, AC-CIO-002), so the adapter must retain role dispositions outside the core input. If any role is `ReadFailed` and the selector's result depends on treating that role as absent, return the typed load failure rather than publishing the selector's bootstrap result. A successfully decoded other role may still be passed to the selector and used under its normal precedence. This does not require a core API change.

2. **Directory validation needs a concrete platform rule.** The contract requires a caller-supplied normalized absolute non-root directory and rejects traversal before mutation (REQ-CIO-001, AC-CIO-001), but does not define canonicalization or reparse/symlink handling. The implementation handoff should state whether a reparse-point root is rejected and how a trailing separator, drive root, UNC path, and case-insensitive aliases are classified. At minimum, resolve/validate the directory once before deriving the three fixed filenames and never accept caller-supplied role filenames. This tightens the meaning of “exact paths under the root”; it does not change ownership scope.

3. **Durability and atomic replacement depend on the selected platform primitive.** The transaction order is explicit (REQ-CIO-004/005, AC-CIO-003/004), but the contract leaves the supported Unity/runtime filesystem primitive implicit. Before implementation, Terra's impact brief should name the target platform and the primitive used for durable flush and primary/previous atomic replacement; unsupported or non-atomic cases must fail closed before claiming commit. Fault injection can be implemented inside the single allowlisted adapter source/test assembly without widening the approved public core API.

## Public seam sufficiency

The pure API is sufficient for this child if IO keeps the typed observations and enforces the no-bootstrap guard described above:

- `CostumeCanonicalCodecV1.Encode/TryDecode` provides deterministic canonical bytes and validation without filesystem ownership.
- `CostumeRecoveryInputV1` defensively copies primary/previous/temp byte arrays; `CostumeRecoverySelectorV1.Recover` supplies deterministic candidate selection and state transformation.
- `CostumeRecoveryPlanV1` and immutable `CostumeStateV1` expose the recovered result, reason, and binding availability.
- `CostumeStateV1.Revision` lets CIO correlate a save to the expected state revision without the adapter changing it.

The seam intentionally does not represent filesystem role dispositions, save receipts, or commit stages; those belong in the new CIO adapter. No existing pure API widening is required. The contract's rule to stop and request an Astra-approved amendment if a required member proves insufficient is consistent with the parent contract.

## Authority, allowlist, and stop conditions

- CIO is a separate additive child of the Approved pure-core slice. The parent CIO/CUA document remains `Review`; the reviewed child authorizes only a future CIO-specific Astra approval and does not authorize CUA, wardrobe UI, sprite import, or gameplay binding.
- `NoAcceptedDefault` remains a read-only unavailable result. The 36 real Seryeong rows are all `ProductionPending`; no save, grant, selection, binding, or media lookup can be inferred from this child (REQ-CIO-006, AC-CIO-005).
- The exact CIO allowlist in the child is bounded to its new IO asmdef/source/tests and named evidence plus the minimum README link/status hunk. No Profile/Run, existing runtime/test/asmdef, scene/prefab, media, package, or ProjectSetting edits are authorized (REQ-CIO-007/008, AC-CIO-001/006).
- Continue to stop and return to Astra if implementation needs to widen or edit a pure API, choose a composition root, touch Profile/Run, use Unity, or add paths outside the allowlist. An uncertain commit must not trigger retry, cleanup, overwrite, promotion, rollback claims, or success claims (REQ-CIO-005).

## Traceability

| Contract requirement | Reviewed acceptance evidence | Result |
|---|---|---|
| REQ-CIO-001 / 008: isolated fixed-path ownership and allowlist | AC-CIO-001 | PASS; clarify root canonicalization as P2 |
| REQ-CIO-002 / 003: typed single observations and deterministic recovery | AC-CIO-002 | PASS; adapter must retain `ReadFailed` outside the nullable core input |
| REQ-CIO-004: durable atomic transaction and unchanged revision | AC-CIO-003 | PASS; identify supported platform primitive before implementation |
| REQ-CIO-005: pre-commit vs uncertain commit | AC-CIO-004 | PASS; required no-retry/no-cleanup boundary is explicit |
| REQ-CIO-006: no accepted default is read-only unavailable | AC-CIO-005 | PASS; matches current 36-row pending inventory |
| REQ-CIO-007: no unrelated authority or APIs | AC-CIO-006 | PASS; stop boundaries are explicit |
| All CIO requirements | AC-CIO-007 | PASS as a future verification plan; implementation and regression evidence remain outstanding |

## Dirty-worktree constraint

The inspected worktree is broadly dirty, and the costume pure-core files are untracked in Git even though the dated handoff and Luna review describe their bounded accepted state. A plain later `git diff` will therefore not prove the CIO allowlist by itself. Before Terra starts, record a read-only baseline of the allowlisted and non-allowlisted paths (for example, a path/status and content-hash manifest) and compare the post-change tree to that baseline. Do not reset, clean, or otherwise rewrite the shared worktree. This is an evidence-handling constraint, not a blocker to approving the contract.

## Participation

Luna reviewed the named contract against the public pure APIs, parent contracts, bounded implementation handoff, Luna pure-core review, Astra bounded integration status, and current worktree status. No runtime or test file was changed and no tests were run.
