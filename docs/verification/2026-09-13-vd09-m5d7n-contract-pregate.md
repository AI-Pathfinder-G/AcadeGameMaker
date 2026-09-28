# VD-09 M5D7N hub-entry handoff latch — Luna pre-gate

- Review date: 2026-09-13
- Reviewer: Luna (independent contract pre-gate)
- Contract reviewed: `docs/specs/work-contracts/2026-09-13-vd09-m5d7n-hub-entry-handoff-latch.md`
- Contract SHA-256 at review: `AD188C412FCB1B46C6F7B5756189CA91A4561F30AC591C5EB9AD4B2A84232AB6`
- Dependency: Verified M5D7M (`DesktopProfileLaunchAdapterV1` / `InputRouter` boundary)
- Verdict: **PASS pre-gate** — P0=0, P1=0, P2=1

## Scope and independent conclusion

The amended draft is implementable inside its allowlist and preserves the Verified M5D7M boundary. It consumes only the adapter's internal receipt/notification seams, reads `InputRouter.IsFaulted` and `EffectiveMode`, and does not require an `InputRouter` source, asmdef, action asset, scene, prefab, Profile, persistence, or UI change. The revised notification protocol closes the earlier loss race: the expected nullable payload is derived from the validated receipt before the single authorized source take; absence, unexpected presence, kind mismatch, and correlation mismatch are terminal and publish no handoff. `AwaitingStart`/`AwaitingFirstUpdate` makes the lifecycle gate explicit, and the map claim is correctly restricted to the immutable M5D7M publication snapshot plus the live `UIOnly` mode (no adopted-actions inspection).

No P0/P1 contradiction or authority expansion remains. No user decision is required for this code-only latch. The later visible hub UI owner still requires separate decisions for scene/prefab topology, menu order, focus, notification presentation, and UI technology as stated by the contract.

## Findings

### P2-001 — lifecycle provenance is a declared authoring precondition, not historical proof

The wording that the latch and adapter are enabled on the same active GameObject “before that lifecycle begins” is enforceable for the normal authored setup and for disable/destroy callbacks, but a component dynamically added after M5D7M publication could not prove historical cohort membership without a new registry/token or a receipt read in `Awake` (both outside this allowlist). This is non-blocking because the draft explicitly defines the supported topology as pre-lifecycle authored components, forbids scene/registry discovery, and makes first-`Update` acceptance the only source operation. Evidence should state that late component injection is outside the supported lifecycle cohort rather than claim a historical token proof.

## Acceptance-criterion feasibility

| Criterion | Status | Independent verification requirement |
|---|---|---|
| AC-M5D7N-001 | Feasible | Use real M5D7M preparation integration and prove clean-primary, default, previous-promotion, decode-repair, and binding-apply-repair each yields one receipt-correlated UI-only handoff. |
| AC-M5D7N-002 | Feasible | Exercise preservation denial, save-failure, and uncertain-commit receipts; assert `HubEntryAccepted=true` and exact pending evidence is retained. |
| AC-M5D7N-003 | Feasible | Cover all three M5D7M notification kinds and no-notification; cover prior-consumed/unexpected/kind/correlation mismatches as terminal; assert one local take then false/default. |
| AC-M5D7N-004 | Feasible | Use authored same-GameObject fixtures with supported Awake/Start permutations; prove `Pristine → AwaitingStart → AwaitingFirstUpdate`, one first-Update attempt, and no later receipt read (e.g. mutate source after publication and repeat Update). |
| AC-M5D7N-005 | Feasible | Reflection/configuration fixtures can provide null or malformed receipt, foreign/faulted/non-UI-only router, wrong object, disabled/inactive lifecycle, and illegal latch fields; assert no handoff, terminal state, and no source mutation. |
| AC-M5D7N-006 | Feasible | Disable/destroy before first Update and verify the source notification remains available; disable/destroy after publication and verify handoff plus adapter/router/source state are unchanged. |
| AC-M5D7N-007 | Feasible | Repeat Update, disable/enable, and local notification take; assert no republish, source retry, or second consumption. |
| AC-M5D7N-008 | Feasible | Reflection-mutate every handoff/state/notification-correlation field and assert getter validation; static scan must reject forbidden scene/UI/input/profile/save/run/logging dependencies. |
| AC-M5D7N-009 | Feasible | Record focused M5D7N, direct M5D7M, full EditMode, and full PlayMode XMLs with failed/skipped/inconclusive counts all zero, followed by Luna P0/P1=0. |

## Boundary and allowlist audit

- `HubEntryHandoffLatchV1` is internal and can live in the existing `AcadeGameMaker.Input.Unity` assembly, where the already-approved internal M5D7M types are available; no new friend or asmdef is needed.
- The latch stores an immutable receipt projection and local notification only. It never owns or closes adopted actions, changes router maps/mode, calls Profile/M5D7L, performs IO, retries, searches scenes, creates UI, or exposes Unity object/path/clock/exception/prose data.
- Receipt publication is staged after all fallible validation, with the non-throwing handoff assignment last. Source notification ownership is transferred only by the one authorized successful take; a later local failure does not restore or reconsume source state.
- The clarified live-state proof is implementable with existing M5D7M reads: exact adapter-bound router identity, active/enabled router, `IsFaulted == false`, and `EffectiveMode == UIOnly`; map booleans are the immutable M5D7M receipt snapshot and must not be represented as a live action-map claim.
- The allowlist contains only the new latch runtime/test files and documentation. It correctly excludes M5D7M/M5D7L/InputRouter source, asmdefs, generated input, scenes, prefabs, Profile runtime, and unrelated files.

## Recommendation

Recommend Astra advance this Draft to Approved, contingent on row-level evidence satisfying AC-M5D7N-001..009. Keep P2-001 as an evidence-language note: do not claim a historical runtime cohort token for late-added components; test and document the authored pre-lifecycle topology explicitly. This report does not mark the contract Approved or Verified.
