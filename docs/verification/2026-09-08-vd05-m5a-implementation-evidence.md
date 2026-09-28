# M5A post-boss progression core — implementation evidence

- Date: 2026-09-08
- Status: **Verified** — Luna independent verification PASS and Astra local integration acceptance, 2026-09-08.
- Contract: [M5A](../specs/work-contracts/2026-09-08-vd05-m5a-post-boss-progression-core.md)
- Requirements: REQ-COM-004, REQ-RUN-001, REQ-RUN-004, REQ-CHOICE-004, REQ-CHOICE-005.
- Terra: new engine-free Run runtime, assembly definitions and metadata.
- Luna: independent test implementation, contract pre-gate and read-only runtime/AC review.
- Astra: contract decisions, runtime and test counter-review, sequential Unity execution and final integration authority.
- Sol: not required for this bounded core. Ollama, Orca, external calls, probes, schedules and Pro execution were not used or claimed.

## Implemented boundary

`PostBossProgressionSession` receives one closed death-tick batch. Alive/Active with no accepted failure creates a TransitionRequested intent; player death, already accepted failure or inactive Run suppresses success. Only exact normal next-tick cleanup plus completed teardown creates the ordered pending reward request, boss-segment completion and choice/barrier preparation intents. The three valid saved pairs and all four lifecycle reasons are closed mappings. Identical replay yields Duplicate with no new intents; conflicting/invalid input cannot partially mutate state.

All accepted lists and result lists are copied and read-only. The session normalizes values and stages the complete immutable result before publishing state. Transition has no cleanup tick; normal completion retains separate death/cleanup ticks. Pending/completed snapshot flags do not claim inventory payment or room/run success.

New files are confined to `Assets/AcadeGameMaker/Runtime/Run` and `Assets/AcadeGameMaker/Tests/EditMode/Run` plus their folder metadata and allowlisted documentation. Run references Core only and has `noEngineReferences=true`. Existing Combat/Transfer/Movement, authoring assets, scene/input/profile code and project settings were not intentionally changed for M5A.

## Executed results

Unity 6000.6.0f1 normal-user licensing preflight passed. Source writers were frozen during each run; Unity runs were sequential. All final rows have exit 0 and skipped 0. XML/log files are local ignored evidence.

| Run | Passed / failed | XML | SHA-256 |
|---|---|---|---|
| Focused M5A EditMode | 10 / 0 | `TestResults-Unity-EditMode-20260908-142933.xml` | `FDE2D322241A05223E7ED138AD4A912386F077A8CA4267F4BFD632E6F41C5903` |
| Full EditMode | 440 / 0 | `TestResults-Unity-EditMode-20260908-143000.xml` | `BBB76E713964E6DA9DCFD23FEFC22A224C52EFAEB7065ADFEE673DCE04D89DE8` |
| Full PlayMode regression | 367 / 0 | `TestResults-Unity-PlayMode-20260908-143035.xml` | `C87502525E783FC6F622F59CA47F1786F0873C9B415B03993DF364A688881982` |

First focused run `142739` compiled successfully and returned 9/10. Its only failure expected ArgumentException for a checked integer-overflow guard that correctly raised OverflowException. The test assertion was corrected; runtime rejection was not relaxed. Earlier source-review corrections are also retained in this record: result-before-state staging, immutable result lists, per-handoff ticks, nullable transition cleanup tick, explicit request source/lock intent fields, nested failure validation, checked cleanup successor and diagnostic/readability improvements. Test drafts were corrected for valid None/None, genuine cadence grouping and element replacement (not merely fixed-size-list Add) immutability checks before execution.

## Acceptance mapping

All test names below belong to `PostBossProgressionSessionTests`; several contain multiple closed-table cases, not ten single input scenarios.

| Criterion | Evidence |
|---|---|
| AC-M5A-001 | AliveDeathEmitsOneTransitionAndFreezesSource: source copy, AwaitingCleanup, one lock-policy request, null cleanup tick, no completion flags. |
| AC-M5A-002 | NormalCleanupEmitsOrderedThreeIntentsForAllRoutes: exact normal completion, ordered requests, source `ordan`, pending/completed flags; no reward amount/grant/RunEnd API. |
| AC-M5A-003 | All three valid pair routes, default None/None acceptance, mismatched/unknown pairs and default outer death rejection. |
| AC-M5A-004 | FailurePrecedenceAndHealthFactsSuppressOrReject: player-dead/inactive Run/prior and same-tick failure causes, unknown/negative/future failure and inconsistent health facts. |
| AC-M5A-005 | AllLifecycleReasonsSuppressNormalCompletionAndRejectBadProof: all four lifecycle reasons, no new intents or completion flags, invalid durable/reason combinations. |
| AC-M5A-006 | ReplaysAreDuplicatesConflictsAreAtomicAndSessionsAreIndependent: identical death/cleanup replay, changed payload/tick/lifecycle, pre-death and suppressed cleanup, independent sessions; retained completed phase. |
| AC-M5A-007 | DefaultOverflowFalseProofAndWrongTickInputsNeverPartiallyMutate and InputsAndResultsExposeDefensiveImmutableValues: default ingress, valid retry, missing/false proofs, handoff order/ticks, integer bounds and immutable list element replacement. Runtime staging and nested-value revalidation additionally inspected. |
| AC-M5A-008 | Assembly-reference test plus independent runtime/asmdef review: Core-only, no Unity/Combat/Transfer/clock/RNG/IO/network/scene/save operations; no old ABI/asset integration. Scoped diff whitespace check passed. |
| AC-M5A-009 | Focused/full results above and actual integer fixed-tick accumulator at 30/60/144 synthetic render grouping; identical full trace and final flags below. |

## Deterministic trace

The executed test emitted `firstDivergence=none`, `tracesEqual=True`. Death tick 10 and cleanup tick 11 may share a render frame at 30 grouping but not at 60/144; the same ordered decisions are retained. This is synthetic cadence testing, not a live-FPS performance claim.

```text
TransitionRequested@10/null
RewardRequestPending@10/11
BossSegmentCompleted@10/11
PrepareChoice@10/11
Duplicate=Duplicate
Phase=ChoiceRequired
RewardPending=True
SegmentCompleted=True
FixedTick=13
```

## Remaining integration boundary

This is a verified **pure decision core**, not a playable boss-to-choice transition. Value inputs are not authenticated graph capabilities. M5B needs an Approved contract for authoritative graph binding, actual input/failure-source locking, effect acknowledgement and UI/scene integration. Choice save/skill grant, reward content/payment, barrier completion, DemoCompleted and heroine appearance remain outside M5A. No active-run saving/resume, Git commit or remote wiki publication is claimed. Parent VD-04/05/06 and whole-game completion are not asserted.

## Final decision

Luna independently confirmed the three XML hashes, counts, cadence output and current runtime/static boundaries, reporting AC-M5A-001..009 PASS with P0/P1/P2=0/0/0. Astra accepts the reviewed local M5A integration and marks the contract Verified. The initial pre-gate default-pair clarification and all initial implementation/test corrections above remain part of the evidence history.
