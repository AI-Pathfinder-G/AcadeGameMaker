# VD-09 M5D7K launch preservation sequence executor

- Status: Verified
- Owner: Astra
- Implementer: Terra after Approved
- Independent reviewer: Luna
- Dependencies: M5D7E, M5D7F, and M5D7J Verified
- Parent requirements: `REQ-PLAT-009`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D7K-001` through `AC-M5D7K-009`
- Astra verification: 2026-09-13 after focused M5D7K 11/11, M5D7E 10/10, M5D7F 18/18, full EditMode 663/663, full PlayMode 584/584, and Luna R2 post-review PASS (P0=0, P1=0; residual P2=2). This verifies preservation sequencing and the `MayPersist` gate only, not the later binding/apply/save launch coordinator.

## Purpose and boundary

M5D7K executes only the preservation intents already proven by an M5D7E selection plan. It quarantines required files in exact `Temp → Primary → Previous` order before any later save may overwrite stale temp, invalid primary, or invalid previous. It stops at the first unresolved preservation and returns `MayPersist=false`; it never performs the save itself.

This is not the launch coordinator. It does not observe/decode/select, apply bindings, transform profile state, save, choose the Unity persistent path, read the clock, log, notify, retry, dispose input candidates, or enter gameplay.

## Approved API and input identity

```csharp
public static ProfileLaunchPreservationResultV1 Execute(
    string rootDirectoryPath,
    ProfileLaunchObservationBatchV1 observation,
    ProfileLoadSelectionPlanV1 selection,
    DateTimeOffset utcNow);
```

Before any preservation call, root is normalized with the same non-root fully-qualified rules as M5D7F, `utcNow.Offset` must be zero, both values fully validate, and `ProfileLoadSelectorV1.Select(observation.Primary, observation.Previous, observation.Temp)` must be semantically identical to `selection`: source/revisions/save flag/reasons/three intents and exact in-memory canonical bytes. Any mismatch/default/reflection-invalid input throws before the port is called.

## Closed role state and sequence

`ProfileLaunchPreservationStateV1`: `NotRequired=1`, `Preserved=2`, `SourceMissing=3`, `FailedBeforeMove=4`, `MoveOutcomeUncertain=5`, `NotAttempted=6`.

The immutable result exposes `TempState`, `PrimaryState`, `PreviousState`, `MayPersist`, `StoppedRole` (nullable), and `Validate()`, plus private proof of the three exact intents and attempted prefix. Every getter fully validates.

- Intent `None` maps to `NotRequired` without a port call.
- Required intent calls M5D7F once with the exact observation candidate, reason, normalized root, and the same UTC instant.
- M5D7F `Preserved` maps to `Preserved`; `SourceMissing` maps to `SourceMissing`. Both resolve the intent and sequencing continues.
- `FailedBeforeMove` maps to `FailedBeforeMove`; `MoveOutcomeUncertain` maps to `MoveOutcomeUncertain`. Either stops immediately, sets `StoppedRole` to that role, marks every later required role `NotAttempted` (later `None` remains `NotRequired`), and makes `MayPersist=false`.
- `MayPersist=true` only when every required intent is `Preserved` or `SourceMissing`, no state is `NotAttempted`, and `StoppedRole` is absent.

The exact call order is Temp, then Primary, then Previous, skipping `None`. No retry, rollback, deletion, fallback, re-observation, or continuing after unresolved preservation is permitted. One role cannot be called twice.

An instance-scoped internal port wraps only `ProfileQuarantineServiceV1.Preserve`; production uses the Verified M5D7F service and holds no mutable/static test state. Returned M5D7F result must fully validate and match the requested role/reason. Unknown/reflection-invalid or mismatched port results throw as protocol violations; ordinary typed failure outcomes remain result states. Unexpected/fatal exceptions propagate.

## Requirements

- **REQ-M5D7K-001:** accept only an exact observation/selection proof pair and normalized non-root path with UTC instant.
- **REQ-M5D7K-002:** execute required preservation exactly once in Temp→Primary→Previous order with exact candidate/reason/UTC.
- **REQ-M5D7K-003:** treat Preserved and SourceMissing as resolved, but stop on FailedBeforeMove or MoveOutcomeUncertain and forbid later persistence.
- **REQ-M5D7K-004:** expose a closed immutable three-role result with exact attempted-prefix and stop-role proof.
- **REQ-M5D7K-005:** protocol-invalid/mismatched quarantine results and invalid caller inputs fail before being accepted; no retry or hidden continuation.
- **REQ-M5D7K-006:** no observation/selection/apply/transform/save/path-source/clock/log/notification/Unity/gameplay/network/RNG authority is added.
- **REQ-M5D7K-007:** Verified M5D7E/F public ABI and behavior remain unchanged.

## Acceptance criteria

- **AC-M5D7K-001:** no-intent plan returns all `NotRequired`, no calls, `MayPersist=true`, no stopped role.
- **AC-M5D7K-002:** all three required intents call exact Temp→Primary→Previous once and all preserved results yield `MayPersist=true`; mixed SourceMissing remains resolved.
- **AC-M5D7K-003:** failure/uncertainty at each required position stops the call log exactly there, marks later required roles NotAttempted, preserves later None as NotRequired, and yields exact stopped role/MayPersist false.
- **AC-M5D7K-004:** real isolated readable files for stale temp plus invalid/unsupported role representatives are moved to M5D7F recovery names in exact order; originals are absent and no save/temp rewrite occurs. The platform-dependent unreadable branch is covered deterministically through the instance port with an exact M5D7F unreadable candidate/result, while M5D7F retains its Verified real-I/O unreadable classification tests.
- **AC-M5D7K-005:** invalid/default/reflection-corrupted observation candidates, observation/selection semantic mismatch in source, revision, document, save flag, reasons, or intents, invalid path/UTC, and null port all throw before a call. As defined by the API identity section, two separately produced plans with identical public selection semantics are equivalent here; M5D7K does not claim historical identity of M5D7E's private candidate proof.
- **AC-M5D7K-006:** mismatched role/reason, default/reflection-invalid result, and unexpected/fatal port exception propagate without later calls; a following valid call succeeds.
- **AC-M5D7K-007:** every legal state/prefix/result row and reflection mutation of role states, MayPersist, stopped role, intents, or prefix proof fails closed as applicable.
- **AC-M5D7K-008:** static review proves one M5D7F production delegation, exact order, instance-only test port, no save/observation/selection recomputation beyond identity proof, no forbidden authority, and unchanged M5D7E/F files/tests.
- **AC-M5D7K-009:** focused/full EditMode and full PlayMode have failed/skipped/inconclusive zero and Luna reports P0=0/P1=0. PASS is preservation sequencing only, not launch recovery completion or permission to save when `MayPersist=false`.

## Exact allowlist

- new `Assets/AcadeGameMaker/Runtime/Profile/ProfileLaunchPreservationExecutorV1.cs` and `.meta`
- new `Assets/AcadeGameMaker/Runtime/Profile/AssemblyInfo.cs` and `.meta`, containing only `InternalsVisibleTo("AcadeGameMaker.Profile.EditMode.Tests")`
- new `Assets/AcadeGameMaker/Tests/EditMode/Profile/ProfileLaunchPreservationExecutorV1Tests.cs` and `.meta`
- this contract, M5D7K pre-gate/implementation/review evidence, and one minimal `docs/README.md` entry

All existing runtime/tests/asmdefs, Packages, ProjectSettings, scenes, prefabs, and assets remain read-only. Luna pre-gate → Astra Approved → Terra implementation → Astra Unity runs → Luna post-review → Astra Verified integration.

## Stop and rollback conditions

Stop if exact sequencing requires changing M5D7E/F APIs or if cross-operation atomicity is claimed. External file races remain observable through M5D7F typed results and are not hidden. Rollback removes only the new executor/tests/docs; never reset/clean/checkout unrelated dirty work.

## Participation record

Astra separated preservation from save because a stale temp can be destroyed by the later temp write and an abnormal previous can be replaced by atomic save. The fixed order and fail-closed `MayPersist` gate make those destructive edges explicit before the later launch coordinator is allowed to persist.

Luna independently pre-gated the contract PASS with P0=0/P1=0 and four residual P2 notes. Astra accepted the recommendation and narrowed AC-M5D7K-004 so portable real-file movement and deterministic unreadable classification are proved without relying on unstable Windows ACL/share-lock behavior. During implementation inspection Astra also added a new, single-purpose `AssemblyInfo` friend declaration to the exact allowlist so the dedicated EditMode fixture can exercise the internal instance port without widening the public product API or modifying an existing file.

Terra implemented the runtime boundary and initial fixture. Astra withheld acceptance, expanded the adversarial fixture after Terra's execution budget ended, and corrected two fixture-only failures. Luna's first post-review withheld PASS until the unreadable candidate used an exact unreadable-state M5D7F result; Astra added that case and a reflected observation-candidate case, reran the focused and full suites, and accepted Luna's R2 PASS recommendation.
