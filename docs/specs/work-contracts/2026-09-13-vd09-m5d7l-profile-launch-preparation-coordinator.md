# VD-09 M5D7L profile launch preparation coordinator

- Status: Verified
- Owner: Astra
- Implementer: Terra after Approved
- Independent reviewer: Luna
- Architecture counter-review: Sol
- Dependencies: M5D7D/E/G/H/I/J/K Verified
- Parent requirements: `REQ-PLAT-009`, `REQ-PLAT-010`, `REQ-PLAT-011`
- Acceptance IDs: `AC-M5D7L-001` through `AC-M5D7L-012`
- Astra approval: 2026-09-13 after Luna R2 pre-gate PASS (P0=0, P1=0; residual P2=1). This authorizes only the exact allowlist and prepared-launch boundary below; it does not authorize live router, hub, scene, UI, notification, or logging integration.

## Purpose and boundary

M5D7L is the bounded `Input.Unity` launch-preparation coordinator. It performs one exact sequence: observe the three profile leaves, select an in-memory profile, preserve every selected abnormal file, apply its input block to an isolated fresh `GameInputActions`, repair a current non-empty override only when M5D7I proves application recovery is required, and perform the one applicable atomic save when preservation permits it. It returns a one-owner prepared profile/actions token even when preservation blocks saving or a typed save failure/uncertainty occurs, implementing VD-09 rule 9 without treating persistence failure as data loss or launch failure.

It does not choose `Application.persistentDataPath`, mutate or initialize `InputRouter`, enable an action map, transfer into gameplay, load a scene, enter the hub, display/log/notify, retry, quarantine-name, decode/select independently, or save active-run state. A later M5D7M adapter supplies the path/clock, adopts the prepared actions, enters the hub, and emits at most one recovery/persistence notification.

## Startup exclusivity invariant

The desktop startup owner guarantees that no independent profile writer is active from the first observation until preparation returns or throws. M5D7L additionally serializes its own same-root preparations with a normalized-path, `OrdinalIgnoreCase`, ref-counted launch lease; different roots may proceed. M5D7D is not a compare-and-swap save, so cross-process writers or unrelated direct save calls during this window are outside this contract. If that product requirement changes, preparation must stop pending a separate expected-generation/CAS decision.

## Internal API and ownership token

Inside `AcadeGameMaker.Input.Unity`:

```csharp
internal static PreparedProfileLaunchV1 Prepare(
    string rootDirectoryPath,
    DateTimeOffset utcNow);
```

`PreparedProfileLaunchV1` is a sealed `IDisposable` one-owner token, not an immutable struct containing a mutable action collection. It exposes validated immutable evidence getters plus:

```csharp
internal GameInputActions TakeActions();
```

Before transfer, the token owns exact one retained, disabled `GameInputActions`. `TakeActions` succeeds exactly once and atomically transfers ownership; a second call throws. `Dispose` before transfer disposes it exactly once; `Dispose` after transfer never disposes the transferred collection. Every exception before a token is returned disposes every created candidate exactly once. The poisoned M5D7I candidate is never retained or transferred.

Immutable evidence includes exact `Source`, `SourceRevision`, `FinalRevision`, defensively independent `FinalDocument`, `FinalRecoveryReasons`, binding outcome/failure, the exact validated M5D7K projection (three role states, `MayPersist`, nullable stopped role), `SaveKind`, `PersistenceOutcome`, `PersistenceStage`, primary/previous/temp stored states, nullable committed revision, `PersistencePending`, and `HubEntryAllowed` (always true for every legal returned token). Default/unknown/reflection-corrupted evidence, ownership-state corruption, or any cross-field mismatch fails from every getter/`Validate()`.

Closed enums:

- `ProfileLaunchSaveKindV1`: `None=1`, `Ordinary=2`, `PrimaryInputRecovery=3`.
- `ProfileLaunchPersistenceOutcomeV1`: `NotRequired=1`, `BlockedByPreservation=2`, `CommittedFirst=3`, `CommittedReplacement=4`, `CommittedPrimaryInputRecovery=5`, `FailedBeforeCommit=6`, `CommitOutcomeUncertain=7`.

For `None`, stage is `None`, all three stored states are `NotProbed`, and committed revision is absent. `BlockedByPreservation` requires a valid K result with `MayPersist=false`, a final document requiring persistence, the same no-call stored-state row, and `PersistencePending=true`. Committed rows require exact D/H success evidence and committed revision equal to `FinalRevision`; pending is false. Typed failed/uncertain rows retain exact D/H stage and all three observed states, have no committed revision, and set pending true. A direct current primary with quarantine work that resolves is still `NotRequired`; quarantine failure is not called save pending when no save was otherwise required.

## Exact preparation sequence

1. Validate and normalize a fully qualified non-root path and exact zero-offset UTC; acquire the same-root launch lease.
2. Call M5D7G `Observe` once, then M5D7E `Select` once with exact observed Primary/Previous/Temp.
3. Call M5D7K once with the same root/observation/selection/UTC. A protocol exception aborts preparation. A typed `MayPersist=false` does not abort; it only forbids every D/H call.
4. Create one fresh disabled default `GameInputActions`; call M5D7I once with `selection.InMemoryDocument.Snapshot.Input`.
5. `DefaultsReady` or `OverridesApplied` must pair with `Retain`; this candidate becomes the owned candidate. `RecoveryRequired` must pair with `Discard`, selected input must be exact current/non-empty, and selected source must be Primary or Previous. Dispose the poisoned candidate once, call the matching M5D7J entry on the exact selected current decode result, create a second fresh disabled candidate, apply the repaired empty input once, and require exact `DefaultsReady/None/Retain`; the second candidate becomes owned. RecoveryRequired for Default, already decode-repaired input, empty input, or a second apply is a protocol violation and returns no token.
6. The final document is the selection document unless step 5 used M5D7J, in which case it is that exact recovery result. `FinalRecoveryReasons` are selection reasons plus the exact apply-failure repair reasons without double revision increment.
7. Save only when the final plan requires it and K returned `MayPersist=true`. Dispatch exact M5D7H `SaveInputRecovery(root, observation.Primary)` only for an originally selected Primary whose decode classification was `ValidInputMetadataRecoveryRequired` or `ValidBindingRecoveryRequired`. Dispatch ordinary M5D7D `Save(root, finalDocument)` for default bootstrap, previous promotion (with or without decode repair), and current Primary/Previous M5D7I apply-failure repair. Direct current Primary success has no save. There is never both D and H, and K false calls neither.
8. Convert only validated typed D/H results to the closed persistence evidence. Committed results must prove exact final revision. Failed/uncertain results still return a hub-ready token with the verified in-memory document and retained disabled candidate. Release the launch lease on every path.

No retry, fallback save, second observation/selection/preservation, apply to live actions, reuse of a poisoned candidate, cleanup of profile leaves, or continuing after a protocol/programmer/fatal exception is permitted.

## Coordinator-local projections and test seams

The new coordinator file owns internal immutable projection types so its PlayMode friend assembly never needs access to Profile internals:

- an observation projection contains exact validated Primary/Previous/Temp `ProfileLoadCandidateV1` values and enforces their roles; production M5D7G output is mapped into it, and the coordinator reconstructs the Profile batch through its single approved `InternalsVisibleTo("AcadeGameMaker.Input.Unity")` access;
- a preservation projection contains exact K Temp/Primary/Previous states, `MayPersist`, and nullable stopped role. Production validates K first, maps every field, and the projection revalidates the same closed prefix/state matrix;
- a unified save projection contains exact save kind, committed/failed/uncertain category, stage, three stored states, and nullable committed revision. Production validates D or H first and maps without collapsing stage/state distinctions. The projection validates only rows legal for its declared kind.

These projections expose internal constructors only, retain no Profile internal result object, bytes, path, exception, callback, or mutable collection, and every getter fully validates. The token retains the validated projections as its structural proof. Default/unknown/reflection-corrupted projections fail before the coordinator accepts them.

Two fresh instance ports are allowed through one internal overload: a profile port returning only the coordinator-local projections while wrapping M5D7G, M5D7K, M5D7D and M5D7H, and an actions port wrapping only candidate creation, M5D7I apply, and candidate disposal. Pure M5D7E/J calls remain direct. Ports retain no static test state. The production launch lease may use a private static registry, but it contains only per-root gates/refcounts and releases in `finally`.

The actions port contract is exact: `Create()` either throws before ownership exists or returns one non-null fresh disabled default candidate; `Apply(input,candidate)` returns the exact validated M5D7I result for that same object; `Dispose(candidate)` records one disposal attempt for that exact owned object. A dispose exception propagates immediately, is never retried, and prevents token creation or later save; cleanup code must not replace an already-propagating fatal exception with a second injected cleanup exception. Tests use instance identity to prove create/apply/dispose order and ownership.

Typed K/D/H outcomes become result evidence exactly as stated. M5D7I typed recovery selects the repair path. Invalid port results, impossible cross-module combinations, ownership violations, invalid typed values, and all unexpected/fatal exceptions propagate after exact candidate cleanup; they are never converted to a hub-ready result. A later valid call succeeds.

## Requirements

- **REQ-M5D7L-001:** serialize same-root launch preparation and execute exact G→E→K→I→optional J→optional D/H order under the startup exclusivity invariant.
- **REQ-M5D7L-002:** forward the exact observed/selected input into a fresh disabled candidate and retain it only after a validated M5D7I success.
- **REQ-M5D7L-003:** on authenticated current/non-empty apply recovery, dispose the poisoned candidate, perform one exact source-specific M5D7J transform, and retain only a fresh default candidate proven by a second apply.
- **REQ-M5D7L-004:** forbid D/H when K denies persistence and dispatch exactly one correct save kind when final persistence is required and permitted.
- **REQ-M5D7L-005:** return a hub-ready, in-memory preparation on typed preservation/save failure while exposing exact pending evidence; protocol/programmer/fatal failure returns no token.
- **REQ-M5D7L-006:** enforce one-shot actions ownership transfer and exact-once cleanup on every return/throw/dispose path.
- **REQ-M5D7L-007:** expose a closed, fail-validating source/recovery/binding/preservation/persistence evidence model with exact revision relations.
- **REQ-M5D7L-008:** add no path-source, router, map-enable, gameplay, scene, UI, notification, log, retry, network, RNG, or active-run authority and preserve all dependency ABIs/behavior.

## Acceptance criteria

- **AC-M5D7L-001:** current empty and current valid non-empty Primary success execute exact calls and return no-save hub-ready tokens with one retained disabled candidate and exact source/revision/document evidence.
- **AC-M5D7L-002:** primary metadata/binding decode recovery dispatches only H after K success; default and every previous promotion dispatch only ordinary D; committed result/revision/state mappings are exact.
- **AC-M5D7L-003:** current non-empty Primary and Previous M5D7I recovery each dispose the first candidate once, call the exact J entry with no double increment, prove defaults on a distinct second candidate, and dispatch ordinary D once.
- **AC-M5D7L-004:** K denial at every stop role calls neither D nor H yet returns hub-ready with exact preservation evidence; if no save was required, persistence remains `NotRequired` rather than pending.
- **AC-M5D7L-005:** every D/H failed-before-commit and uncertain stage maps exactly, returns the final in-memory document/candidate, and sets pending; committed mappings set pending false.
- **AC-M5D7L-006:** exact call logs prove no retry/duplicate/reordering, correct root/UTC/candidate/document identity, and later-call suppression after protocol or fatal failure.
- **AC-M5D7L-007:** first/second create/apply failure, impossible apply rows, wrong candidate identity/disposition, invalid K/D/H results, revision mismatch, and unexpected/fatal exceptions dispose all owned candidates once and return no token; a following call succeeds.
- **AC-M5D7L-008:** `TakeActions` is exact once; dispose-before-transfer disposes once; dispose-after-transfer does not; double-dispose and double-take are safe/closed; maps remain disabled.
- **AC-M5D7L-009:** representative pairwise coverage spans source×input classification×apply×K gate×save rows; every legal projection/result row validates, and default/reflection mutation of every projection/result/ownership/proof field fails from every getter. Exhaustive Cartesian multiplication is not required when rows are behaviorally equivalent.
- **AC-M5D7L-010:** concurrent same-root deterministic calls never interleave and different roots can overlap; fault paths release the lease. Static inspection proves only the private ref-counted product registry is static and test ports are instance-only.
- **AC-M5D7L-011:** real isolated PlayMode cases cover generated actions empty/known override/stale-ID recovery plus real G/E/K/D/H filesystem paths; no live router, enabled map, persistentDataPath, scene, or notification reference exists.
- **AC-M5D7L-012:** focused PlayMode, direct dependency regressions, full EditMode and full PlayMode have failed/skipped/inconclusive zero, and Luna reports P0=0/P1=0. PASS is prepared launch ownership only, not router adoption, hub entry side effects, or player notification.

## Exact allowlist

- new `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileLaunchPreparationCoordinatorV1.cs` and `.meta`
- modify only `Assets/AcadeGameMaker/Runtime/Profile/AssemblyInfo.cs` to add `InternalsVisibleTo("AcadeGameMaker.Input.Unity")`
- new `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileLaunchPreparationCoordinatorV1Tests.cs` and `.meta`
- this contract, M5D7L pre-gate/implementation/review evidence, and one minimal `docs/README.md` entry

All other runtime/tests/asmdefs, generated actions, input asset, Packages, ProjectSettings, scenes, prefabs, and assets remain read-only. Luna pre-gate → Astra Approved → Terra implementation → Astra Unity runs → Luna post-review → Astra Verified integration.

## Stop and rollback conditions

Stop if correct preparation requires live `InputRouter` mutation, map enablement, a public M5D7H API, dependency ABI changes, a second persistence algorithm, cross-process writer safety, or scene/UI authority. Rollback removes only the new coordinator/tests/docs and the one added friend declaration; never reset/clean/checkout unrelated dirty work.

## Participation record

Astra integrated Sol's bounded architecture counter-review: preparation and ownership transfer are separated from live router/hub effects; typed save failure still yields a safe in-memory launch; a poisoned actions candidate is never reused; and startup exclusivity is explicit because ordinary M5D7D is not compare-and-swap. No user decision is required under the current single-process desktop startup model.

Luna's first pre-gate withheld approval because the PlayMode fixture could not construct Profile-internal G/K/D/H results. Astra introduced coordinator-local validated projections and closed the actions-port exception rules; Luna R2 returned PASS with P0=0/P1=0 and one residual scope-breadth P2. Astra accepted the recommendation.

Luna's first post-review found two test-proof P1 gaps without a runtime defect. Terra added exact K/D/H/Apply argument and disposal/exception-precedence assertions plus bounded lease-worker completion assertions. Astra reran focused R9 `33/33`, direct M5D7I `8/8`, full EditMode R3 `663/663`, and full PlayMode R3 `617/617`, all with failed/skipped/inconclusive zero. Luna R2 independently matched the evidence and returned PASS with P0=0/P1=0 and residual P2=2. Astra accepts that result and marks M5D7L Verified.
