---
status: Verified
---

# VD-05 M5A 보스 이후 진행 판단 코어

- Date: 2026-09-08
- Owning Approved specs: [VD-03](../vertical-demo/03-combat-and-enemies.md), [VD-05](../vertical-demo/05-failure-and-persistence.md), [VD-06](../vertical-demo/06-humanity-choice-and-narrative.md), [SYSTEM-CONTRACTS](../vertical-demo/SYSTEM-CONTRACTS.md)
- Decision basis: ADR-0013, ADR-0018, ADR-0027; [승인된 성공 장면 순서](../../approvals/2026-08-25-p1-demo-success-scene-flow-approval.md)
- Dependency: [M4B3C Verified](./2026-09-08-vd03-combat-m4b3c-ordan-terminal-teardown.md)
- Assigned by / final authority: Astra
- Implementation: Terra; independent pre/post verification: Luna
- Requirements: `REQ-COM-004`, `REQ-RUN-001`, `REQ-RUN-004`, `REQ-CHOICE-004`, `REQ-CHOICE-005`
- Acceptance IDs: `AC-M5A-001` through `AC-M5A-009`; parent AC-COM-003, AC-RUN-003, AC-CHOICE-003 are partially exercised, not completed by this unit.
- Astra approval: 2026-09-08 after Luna independent pre-gate PASS (P0=0, P1=0, P2=0). Only the exact M5A allowlist below is authorized; this is not implementation verification.
- Verified: 2026-09-08 — Luna independent implementation verification PASS (AC001..009, no P0/P1/P2 blocker); Astra accepts the local integration. [Evidence](../../verification/2026-09-08-vd05-m5a-implementation-evidence.md): focused 10/10, full EditMode 440/440, full PlayMode 367/367. This verifies the pure core, not M5B Unity/scene integration.

## Purpose and impact

현재 Combat은 사망·보상·방 완료 요청을 발행하고 M4B3C는 보스 전용 실행기를 정지한다. 이를 소비하는 Run, Choice, InputMode, 실제 보상 지급 및 방 전환 runtime은 아직 없다. 같은 triplet을 다시 포장하는 adapter만 추가하지 않고, 다음에 해야 할 일을 판단하는 순수 Run 코어를 먼저 구현한다.

이 코어는 보스의 사망 확정과 정상 정리 완료를 구별하고, 실패 우선권과 정확히 한 번의 진행 결정을 소유한다. 무선택 프로필은 유담 선택 준비, 기확정 프로필은 해당 기술의 봉쇄선 준비로 분기한다. 둘 다 **진행 요청**이며 실제 장면이 열렸다는 결과가 아니다.

## Scope / non-scope

Scope: immutable normalized input, instance-local two-stage session, duplicate/conflict policy, pending reward-request record, boss-segment completion record and next-route intent. Runtime is engine-free and depends only on Core SimulationTick.

Non-scope: Unity adapter, actual input/failure-source locking, UI/cutscene/scene activation, room object removal, room-plan advancement, reward amount/item/inventory, choice confirmation, skill grant, save/load/retry, run start/end/return, `DemoCompleted`, heroine appearance, damage ownership, player movement. M5A does not complete VD-04/05/06 or make the current sandbox transition scenes. M5B must separately bind authoritative owners and prove actual input locking before any downstream gameplay command is consumed.

## Owned state and internal API

One `PostBossProgressionSession` belongs to exactly one boss segment in one run. No static/global dedupe or on-disk state. The caller creates a new instance for a new segment/run; there is no Reset method. An instance never accepts a second boss key even if the tick differs. Two independent instances may use the same authored key/tick without interfering.

Internal API:

- `ObserveDeath(PostBossDeathInput input)` returns an immutable result and current snapshot.
- `CompleteCleanup(PostBossCleanupInput input)` returns an immutable result and current snapshot.
- read-only `Snapshot` has no tick-advancing side effect.

Session phases: `AwaitingDeath`, `AwaitingCleanup`, `ChoiceRequired`, `BarrierPreparationRequired`, `Suppressed`. These are local segment phases, **not** replacements for VD-05 RunPhase or VD-07 InputMode.

Result disposition: `Applied` or `Duplicate`; contract violations throw before state/output mutation. Only `Applied` carries a new immutable intent batch; `Duplicate` carries an empty batch and preserves the previous snapshot. Reading a retained snapshot never delivers a new batch. No callbacks, event bus, async operation or consumer acknowledgement protocol in M5A.

Snapshot retains accepted death/cleanup evidence and exposes `RewardRequestPending` and `BossSegmentCompleted` only in the two normal completed phases. TransitionRequested carries `CleanupTick=null` because cleanup has not happened; the three completion intents carry exact `CleanupTick=t+1`. Only TransitionRequested has `BlockNewFailureRequests=true`. RewardRequestPending carries `SourceId=ordan`; no intent claims an operation was performed by a Unity consumer.

### Death input: one closed source-tick batch

`PostBossDeathInput` carries:

- encounter key exactly `OrdanBossEncounterGraph`, nonnegative `DeathTick=t` with checked representable `t+1`;
- exact ordered handoff entries `Defeated`, `RewardRequest`, `RoomCompletionRequest`, each with source tick `t`;
- explicit terminal boss evidence (`BossHealth=0`, `BossDead=true`), player final Combat health 0..5 and dead flag exactly equivalent to health 0;
- `RunActive` and optional already-accepted failure `(cause, tick)`; cause is exactly `HealthDepleted`, `KillPlane` or `LethalCrush`, accepted tick must be nonnegative and <= t;
- persisted choice/skill pair normalized from an already validated profile snapshot, frozen at observation.

The batch is closed **after** the source tick's authoritative Combat outcome and accepted-failure decision are known. Separate `ObserveFailure`/`ObserveDeath` methods are deliberately absent: invocation order must not decide a same-tick death race. If a future adapter cannot close this batch before consuming new gameplay input, it cannot use M5A as a substitute for that missing owner.

Pair validation is closed and does not invent a product decision:

| Choice | Skill | Destination after normal cleanup |
|---|---|---|
| None | None | ChoiceRequired |
| Extraction | CompressionVerdict | BarrierPreparationRequired |
| Solidarity | CommonReferencePlane | BarrierPreparationRequired |

Any mismatched, one-sided missing or unknown enum value is invalid, not silently repaired. M5A does not read the profile or claim that structural validation proves a successful disk load. None means no confirmed choice; a stored valid pair is not re-granted by this core.

On valid observation:

1. If player is dead, accepted failure exists or RunActive is false: commit `Suppressed` with the complete normalized suppression facts; emit no success/transition/reward/route intents. The existing failure owner retains failure and hub-return responsibility.
2. Otherwise commit `AwaitingCleanup`, retain the immutable source batch and emit exactly one `TransitionRequested` intent with `BlockNewFailureRequests=true`. This is a request for the future Input/Run adapter, not an already-applied lock. No reward/segment-completion/choice/barrier intent exists yet.

This respects same-source player death or already accepted failure; it does not retroactively revoke a valid death gate in response to a newly requested gameplay failure after success gating. A real owner-supplied lifecycle at cleanup remains authoritative as specified below.

### Cleanup input: source t, commit t+1

`PostBossCleanupInput` carries the same key/source tick and `CleanupTick=t+1`, a disposition (`Removed` or `LifecycleSuperseded`), optional lifecycle reason, `AllRegistrationsAbsent`, `DurableRemovalCommitted`, `PlayerBaseline`, `ActiveTargetEmpty`, and `BossTeardownCommitted` facts copied from the completed owner publications.

- All completion paths require registration absence, Baseline/empty active target and completed boss-only teardown. These flags describe normalized upstream evidence; M5A does not inspect Unity objects.
- `Removed` requires lifecycle null and durable removal true.
- `LifecycleSuperseded` requires durable removal false and one exact reason: RoomLeaving, RunFailed, Cutscene or DemoCompleted. It commits `Suppressed` with that reason and emits no reward/segment/route intents. Even DemoCompleted here does not authorize M5A to declare success or show the heroine.
- Missing/false/inconsistent normal proof, wrong key/tick, unknown enum and mismatched disposition/reason are **invalid** and leave `AwaitingCleanup` unchanged. Invalid input is not converted to lifecycle suppression or a partial success.
- CompleteCleanup before ObserveDeath, or after source-batch suppression, is invalid with no mutation.

For valid normal completion, commit one immutable snapshot and ordered intent batch:

1. `RewardRequestPending`: source `ordan`, key/source tick/cleanup tick; pending request only, no amount, item, grant, persistent flag or reward-consumed acknowledgement.
2. `BossSegmentCompleted`: exactly this boss segment, not R06 completion, room-plan index advance or run success.
3. `PrepareChoice` when frozen pair is None/None, otherwise `PrepareBarrier` carrying exactly the persisted pair.

No skill or currency is created. No `RunEndCommitted`, `DemoCompleted`, choice confirmation or heroine intent exists in this API. A future reward owner may consume a pending request only under its own approved reward contract; this core must not discard it as already paid. Choice-based skill grant remains after ChoiceCommitted and successful atomic save under VD-06.

### Replay, atomicity and provenance

Validation includes defensive copies, exact ordered enum/ID/tick fields and all nested values. Default outer death/cleanup inputs, missing required fields, null lists and unknown enum values must fail validation at the session boundary even if a constructor was bypassed. The valid normalized `Choice=None, Skill=None` pair remains legal even when represented by its C# default value; it is not an uninitialized outer input. Results and snapshots expose immutable values, never mutable lists or scene references.

- A byte-semantically identical replay of the accepted death batch at any later local phase returns Duplicate without changing the phase or re-emitting TransitionRequested.
- A byte-semantically identical replay of the accepted cleanup batch after completion/suppression returns Duplicate with no repeated reward/completion/route batch.
- Same key with changed tick, life/failure facts, pair or cleanup proof is a conflict and throws without altering the first accepted values. Wrong/second key is likewise invalid. Invalid attempts do not reserve a key or consume the next valid attempt.
- All validation and result allocation complete before publishing any new state. No rollback of already completed Combat/Transfer is attempted or claimed.
- The internal value DTOs are not authenticated capabilities. Equal copied values from another source cannot be distinguished by this pure core. A future adapter must verify bound graph/receipt identity and instance ownership; do not label M5A tests as a production cross-graph provenance guarantee.

## Acceptance criteria

- `AC-M5A-001` — valid alive/Active death source batch emits one TransitionRequested, freezes pair/tick, and stays AwaitingCleanup with no reward/completion/route. No observation leaves AwaitingDeath mutation-free.
- `AC-M5A-002` — valid exact normal t+1 cleanup emits the ordered three-intent batch once; None/None routes to ChoiceRequired. No skill, amount, run success or heroine state is created.
- `AC-M5A-003` — the two valid saved pairs route to BarrierPreparationRequired and retain exactly one corresponding skill identity; all invalid pair combinations/unknown enums are rejected before mutation, including malformed default inputs.
- `AC-M5A-004` — player-dead, prior/same-tick accepted failure (all three causes) and inactive-run cases suppress success; outcomes are independent of unrelated render-frame grouping. Future failure tick and inconsistent health/dead facts are rejected. No failure is silently reclassified as success.
- `AC-M5A-005` — each of the four exact lifecycle reasons suppresses normal completion with no pending reward/route and no durable-removal claim; invalid lifecycle/disposition/proof combinations preserve AwaitingCleanup.
- `AC-M5A-006` — identical death/cleanup replays are diagnostic Duplicate with empty new intents; changed source/pair/proof, wrong key/tick, pre-death cleanup and second-key attempts are mutation-free errors. Failed validation leaves a subsequent valid input usable; independent sessions do not share a latch.
- `AC-M5A-007` — input/output mutation attempts, omitted proof, false teardown/baseline/registration flags, t overflow and wrong t+1 cannot alter accepted state or produce partial intent batches. Source and completion ticks remain distinct.
- `AC-M5A-008` — dependency/static checks prove Core-only runtime assembly without UnityEngine, Combat/Transfer import, clock/RNG/IO/network, callbacks, scene/input/inventory/save operations. No public existing ABI, asset, media or project setting changes.
- `AC-M5A-009` — Luna reviews all AC mappings; focused and full EditMode pass, prior full PlayMode remains green. Synthetic 30/60/144 grouping preserves exact death/cleanup intent trace, duplicate counts and final snapshot; this is not an FPS performance claim. Evidence records first divergence (or none), exact input sequence, final snapshot and XML SHA-256.

## Exact allowlist for later implementation

New runtime only:

- `Assets/AcadeGameMaker/Runtime/Run.meta`
- `Assets/AcadeGameMaker/Runtime/Run/PostBossProgressionSession.cs` + `.meta`
- `Assets/AcadeGameMaker/Runtime/Run/AssemblyInfo.cs` + `.meta` — friend to the new Run EditMode test assembly only
- `Assets/AcadeGameMaker/Runtime/Run/AcadeGameMaker.Run.asmdef` + `.meta` — references `AcadeGameMaker.Core` only; `noEngineReferences=true`

New tests only:

- `Assets/AcadeGameMaker/Tests/EditMode/Run.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Run/PostBossProgressionSessionTests.cs` + `.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/Run/AcadeGameMaker.Tests.EditMode.Run.asmdef` + `.meta` — Editor-only, references Run and Core plus standard test assemblies

Documents: this contract, `docs/verification/2026-09-08-vd05-m5a-contract-pregate.md`, later `docs/verification/2026-09-08-vd05-m5a-implementation-evidence.md`, `docs/README.md`, and the M5A section only in SYSTEM-CONTRACTS.

Everything else is forbidden, particularly old assembly visibility, Combat/Transfer/Movement runtime, Unity wrappers, prefab/scene generation, Packages/ProjectSettings, rewards/choice/profile implementations and external media.

## Integration, ownership and evidence

1. Terra read-only impact: no current runtime consumer; avoid a redundant adapter. Root selected a pure progression owner with stateful validation rather than inventing rewards or modifying M4B3C.
2. Luna pre-gate reviews this exact document. Astra resolves comments and records Approved before code changes.
3. Terra implements only listed files with REQ IDs and implementation tests. Luna independently reviews code/tests and final execution evidence; Astra performs final integration acceptance only after AC001..009 PASS.
4. M5B (not authorized by this contract) must implement actual input/failure locking and bound receipt normalization, then choice/scene integration. M5A verification alone must never be reported as a playable boss-to-choice transition.

Sol complex-design support: not required for this bounded engine-free core; Astra owns the cross-system contract, Terra supplies unit impact and Luna provides independent failure-mode review. Escalate if an actual public interface, ordering conflict or persistence transaction is required. GPT-only participation under ADR-0027; Ollama/Orca/probes/automations not used.

Rollback point: current worktree after M4B3C verification (full EditMode 430/430, PlayMode 367/367). Preserve all existing dirty changes/media. Reversal, if approved, touches only new M5A files and its documentation hunks; no reset/clean/checkout/asset deletion. No commit or remote publication is implicit.

Stop if implementation needs reward definitions, actual skill grant, save fields, new failure cause, input/scene owner, M4B3C behavior change, public API exposure or external calls. Return that precise issue to Astra; do not implement a hidden fallback.
