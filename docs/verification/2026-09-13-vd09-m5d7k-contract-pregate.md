# VD-09 M5D7K launch preservation sequence executor — Luna independent contract pre-gate

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7K launch preservation sequence executor](../specs/work-contracts/2026-09-13-vd09-m5d7k-launch-preservation-sequence.md)
- Compared against: Verified M5D7E load selection, M5D7F quarantine adapter, M5D7J binding-apply-failure transform, M5D7I apply boundary, VD-09 load-recovery rule 7, SYSTEM-CONTRACTS/traceability, AGENTS.md, and current Profile runtime/tests.
- Scope: contract-only independent pre-gate. No contract, runtime, test, asmdef, asset, README, or Unity file was modified; no Unity test was run.

## Verdict

**PASS — P0=0, P1=0, residual P2=4. Recommend Astra advance M5D7K to Approved.**

The draft correctly isolates preservation orchestration between observation/selection and a later ordinary save. It revalidates the observation/selection pair before I/O, calls only required M5D7F intents in exact `Temp → Primary → Previous` order, treats `Preserved` and `SourceMissing` as resolved, and stops before later roles on failure or uncertainty. `MayPersist` is a typed proof gate only; the executor never saves. The design does not add selection, source-path, Unity, Input System, transform, quarantine naming, or persistence authority.

## Boundary and dependency review

- M5D7E's immutable selection plan is the source of preservation intents. Re-running `Select(observation.Primary, observation.Previous, observation.Temp)` and comparing source, revisions, save flag, reasons, all three intents, and exact in-memory canonical bytes is a feasible semantic identity check. The executor passes the exact observation candidate and requested reason to M5D7F; it does not choose a replacement candidate or infer new intent.
- M5D7F's `Preserve` API accepts the root, exact candidate, compatible non-`None` reason, and zero-offset UTC. It revalidates presence/read state and uses the same supplied UTC. M5D7K's UTC offset check and normalized non-root path check are compatible with M5D7F and prevent a call-time rejection from being misreported as a preservation result.
- `SourceMissing` is safely resolved for this boundary: the source no longer exists, so there is no file to quarantine and no stale source left for the later write to overwrite. `FailedBeforeMove` and `MoveOutcomeUncertain` are not resolved; later roles are not attempted and `MayPersist=false`.
- A later ordinary M5D7D save must consume the selection/M5D7J plan only when the executor returns `MayPersist=true`. K never calls Save, and no second save path is needed. For primary current binding failure, the ordinary replacement itself preserves the valid old primary as previous; for previous-source recovery, selector-owned primary quarantine/preservation is completed before the ordinary save.
- M5D7F's real BCL implementation is callable through the stated internal port. Its `File.GetAttributes` distinction (only not-found/ directory-not-found become Missing), exclusive read, typed recoverable failures, one move, and post-move proof remain inside M5D7F. K forwards typed outcomes and does not reinterpret unreadable access as missing.
- The result state model is implementable: a required role maps to `Preserved` or `SourceMissing`, a failed stop role maps to its exact failure/uncertainty state, later required roles become `NotAttempted`, and later `None` intents remain `NotRequired`. The stated `MayPersist` predicate closes the success gate.
- The new executor/test files require no asmdef or package change. Existing M5D7E/F runtime and tests remain read-only. An instance-scoped test port can inject M5D7F results without adding static state or changing M5D7F's public API.

## P0/P1 findings

No P0 or P1 findings. The following high-risk cases are explicitly bounded and should remain visible in implementation review:

1. **Source identity and selection proof:** K uses the exact observation candidates for the M5D7F calls and recomputes selection before any call. Public plan semantic equality is the defined proof boundary; K does not invent file-byte/path identity or retain source bytes.
2. **Exact stop policy:** no retry, rollback, deletion, fallback, re-observation, or post-failure continuation is permitted. A protocol-invalid M5D7F result is an exception, not a typed state that could accidentally permit persistence.
3. **MayPersist fail-closed:** only all-required `Preserved`/`SourceMissing` rows with no `NotAttempted` and no stop role may return true. `SourceMissing` must not be converted to `Preserved` or fabricate a destination.
4. **Exception scope:** only ordinary typed M5D7F outcomes become K states; unexpected/fatal port exceptions propagate and must not be converted to `MayPersist=false` success-like data. The later caller remains responsible for exception handling and candidate disposal.

## Residual P2 notes

- **P2-001 — semantic versus historical observation identity:** two different observations can produce equal public selection semantics while containing different nonselected file bytes. The contract intentionally compares plan semantics and preserves the currently supplied observation's files; it does not claim historical byte identity. Keep this caller/observation boundary explicit.
- **P2-002 — real unreadable-file fixture portability:** Windows can distinguish presence from read failure, but deterministic real unreadable fixtures may require controlled ACL/share-lock setup. Keep injected M5D7F-result coverage alongside any real isolated-file test and do not weaken the production `GetAttributes`/read classification.
- **P2-003 — exact private result proof shape:** the result names public state/stopped-role getters but leaves private intent/prefix proof field names to implementation. Tests must exercise every legal prefix and reflection-corrupted state/intent/stopped-role/MayPersist combination, not only the happy path.
- **P2-004 — downstream caller sequencing:** `MayPersist=true` is permission evidence, not execution. The future launch owner must pass the unchanged selected or M5D7J-transformed plan to ordinary M5D7D Save only after this executor completes successfully; K itself must never persist or decide launch entry.

## Pre-gate AC status

| AC | Status | Independent basis |
|---|---|---|
| AC-M5D7K-001 | **PASS by contract; implementation pending** | All-`None` intents produce three `NotRequired` states, no port calls, absent stop role, and `MayPersist=true`. |
| AC-M5D7K-002 | **PASS by contract; implementation pending** | Required intents are invoked once in Temp→Primary→Previous order; `Preserved` and `SourceMissing` both resolve and permit continuation. |
| AC-M5D7K-003 | **PASS by contract; implementation pending** | Failure/uncertainty at each required position stops exactly there, marks later required roles `NotAttempted`, leaves later `None` as `NotRequired`, and sets `MayPersist=false`. |
| AC-M5D7K-004 | **PASS with P2 fixture note** | M5D7F supplies real isolated-file movement, exact names, and typed unreadable/nohash semantics; a Windows-capable fixture strategy must be retained for the unreadable representative. |
| AC-M5D7K-005 | **PASS by contract; implementation pending** | Root/UTC/observation/selection/intent/candidate/proof mismatches are pre-port and fail closed. |
| AC-M5D7K-006 | **PASS by contract; implementation pending** | Mismatched/default/reflection-invalid M5D7F results are protocol exceptions; unexpected/fatal exceptions propagate and later valid calls remain independent. |
| AC-M5D7K-007 | **PASS by contract; implementation pending** | Closed role-state/prefix/MayPersist/stopped-role and private intent proof are explicitly required to validate from every getter. |
| AC-M5D7K-008 | **PASS by contract** | One M5D7F delegation port, exact order, no recomputation beyond selection identity, no save or forbidden authority, and unchanged M5D7E/F scope are defined. |
| AC-M5D7K-009 | **BLOCKED — implementation gate** | No execution evidence exists yet; focused/full EditMode/PlayMode and Luna post-review remain required. |

## Required implementation checks

Before recommending Verified, inspect that Terra:

- validates root, UTC, observation, selection, and the recomputed semantic plan before acquiring/calling the port;
- does not call the port for `None`, does not call any role twice, and records the exact prefix when a stop occurs;
- verifies every returned M5D7F result fully before mapping it, including exact source role and reason, and propagates protocol-invalid results;
- never converts `SourceMissing` into a fabricated destination or treats failed/uncertain results as resolved;
- uses an instance-only deterministic port with no retained static test state and leaves M5D7F unchanged;
- verifies real Temp/Primary/Previous path order, including a defensible Windows unreadable representative, while maintaining injected protocol/failure coverage;
- never calls Save, M5D7J/M5D7I, InputRouter, Unity path discovery, logging, notification, clock, or gameplay code from the new engine-free boundary.

## Final recommendation

**PASS — P0=0, P1=0, residual P2=4.** Astra may advance M5D7K to Approved. This pre-gate approves only preservation sequencing and the `MayPersist` proof; it does not approve persistence execution or claim launch recovery completion.

## Bounded amendment review

The contract is now `Approved` and narrows AC-M5D7K-004 to real readable stale-temp/invalid/unsupported movement plus a deterministic injected unreadable branch. This closes the portability concern noted as P2-002 without weakening M5D7F's production presence/read classification. The added allowlist entry `Assets/AcadeGameMaker/Runtime/Profile/AssemblyInfo.cs` contains only `InternalsVisibleTo("AcadeGameMaker.Profile.EditMode.Tests")` (with its meta), enabling the dedicated fixture to exercise the existing internal instance port without widening the product API or modifying an existing friend declaration.

No P0/P1 was introduced and the prior PASS remains valid: **P0=0, P1=0, residual P2=3** (the amended AC004 fixture portability note is resolved). Approved may stand; implementation review must still verify the friend file is exactly one declaration and the injected unreadable result matches the requested M5D7F role/reason.
