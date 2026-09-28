# VD-09 M5D7G profile launch observation adapter — Luna independent contract pre-gate

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7G profile launch observation adapter](../specs/work-contracts/2026-09-13-vd09-m5d7g-profile-launch-observation-adapter.md)
- Compared against: VD-09 `REQ-PLAT-008`/`REQ-PLAT-010`/`REQ-PLAT-011`, `AC-PLAT-008`/`AC-PLAT-009`, `SYSTEM-CONTRACTS`, the approved P1 load-recovery decision, and Verified M5D7B/M5D7E/M5D7F boundaries.
- Scope: independent contract-only pre-gate. No runtime, test, contract, README, or Unity files were modified; Unity was not run.
- First-pass verdict: **CONDITIONAL FAIL — P0=0, P1=1, P2=3.** The historical read-completeness finding and required amendment are retained below.

## Executive finding

The proposed boundary is correctly narrow for VD-09: it observes only `profile.json`, `profile.prev.json`, and `profile.tmp.json` in fixed order; maps missing/unreadable/decoded results to the existing M5D7E candidate; and leaves selection, quarantine, save, logging, notification, binding application, clock, Unity path, and gameplay authority to later owners. The explicit absence of a per-root lease is acceptable because the batch disclaims cross-file atomicity and M5D7F revalidates the selected preservation intent under its own lease.

One P1 ambiguity prevents a deterministic, testable read-completeness boundary: the contract simultaneously says recoverable read faults become `Unreadable` and that impossible short/zero-progress reads escape, but the proposed full-read port returns only a `byte[]` and provides no completion/protocol marker. An adapter receiving a truncated non-null array cannot know whether it is a genuine empty file (which M5D7B should classify as `Invalid`) or an incomplete read that AC-005 requires not to decode.

## P1 finding

### P1-001 — Full-read port cannot distinguish a valid empty file from an incomplete/short read

The algorithm says a present file is read completely from one `FileShare.Read` handle. It then says recoverable access/read/probe exceptions map only that role to `Unreadable`, while “impossible short reads” and invalid injected byte results escape. AC-004 separately requires a file that disappears after a present probe/read race to become `Unreadable`, and AC-005 requires injected short/zero-progress reads to fail closed rather than decode a prefix. However, the deterministic port is described as a typed-presence plus “single-handle full-read” operation with no explicit completion result or expected-length contract. A `ReadAll` method returning `byte[0]` is indistinguishable at the adapter boundary from a complete zero-byte file; a short non-null array is indistinguishable from a complete file of that length. Passing either to M5D7B can produce a normal `Invalid` candidate, violating the short-read requirement.

The contract also leaves precedence unclear if the port reports an incomplete read by throwing `EndOfStreamException`/`IOException`: AC-004's recoverable-read rule says `Unreadable`, while the short-read rule says escape. This is not merely test wording; it determines whether a race is represented as a typed candidate or aborts the entire batch.

Minimal amendment: define the port as an operation that owns the handle and guarantees “return only after exact EOF/expected-length completion and close,” with an explicit `ReadIncomplete` protocol outcome (or a dedicated exception) for a short/zero-progress result. State the classification: an actual BCL file disappearance/access/read error after a present probe is `Unreadable`; an injected/port-protocol violation is an escaping `InvalidOperationException` (or another named non-recoverable protocol error). A genuine complete zero-byte file remains a normal M5D7B `Invalid` decode. Add an AC-005 test that injects a short return and a zero-progress return at the port's read boundary and asserts no decoder call/no partial candidate; add a separate empty-file test asserting `Decoded(Invalid)`. If the preferred behavior is that incomplete BCL reads are `Unreadable`, state that explicitly and remove the “short reads escape” exception for that path.

## P2 findings and precision notes

### P2-001 — Handle-close and `FileShare.Read` proof should be observable at the seam

The contract requires one read-only handle, `FileShare.Read`, complete read, close before the next role, and no simultaneous profile handles. A single `ReadAllExclusive(path)` abstraction can implement this correctly, but a call log alone cannot prove the handle was closed before the next role. Require the injected port's full-read call to return only after close, or expose test-only `Open/Read/Close` events while keeping the production API synchronous and internal. AC-003/005 should assert `probe(primary) → read/close(primary) → probe(previous) → ...`.

### P2-002 — Race and mutation tests should cover each role and all five decoder classifications

The contract correctly fixes present→read failure as `Unreadable` and missing→later appearance as `Missing`, and requires all five non-default M5D7B classifications. To make AC-002/004/006 independently meaningful, inject each race and each recoverable probe/read fault once for primary, previous, and temp, and verify that a later role still observes unless an unexpected/fatal fault escapes. Verify that mutation of the source byte array after decoder return cannot alter the candidate/document/projection, relying on the already Verified M5D7B/E defensive-copy guarantees.

### P2-003 — Repeated observation and lease scope need an explicit consumer warning

No per-root lease is the right bounded choice for a read-only observer, but two concurrent `Observe` calls may return batches from different file generations and a writer may interleave between roles. The contract disclaims a stable or cross-file atomic snapshot; keep that disclaimer adjacent to the future launch-orchestrator handoff and require the consumer to pass the selected M5D7E intent to M5D7F for source revalidation. No observer-level lease should be added unless a later contract explicitly owns cross-service serialization.

## Boundary, authority, and allowlist review

The fixed sibling leaves and role order align with VD-09 load recovery: primary/previous/temp are independently validated; stale temp is never selected by this adapter; invalid/unsupported files remain typed candidates for M5D7E/F owners. The adapter performs no write, move, quarantine, save, deletion, copy, retry, transformation, selection, notification, logging sink, wall-clock read, Unity lookup, scene/gameplay operation, network, RNG, or cross-service lease. A normalized non-root path is validated before any port call and is not retained in the public batch. The allowlist is appropriately limited to one engine-free runtime file/meta, one EditMode test/meta, this contract and its reports/evidence, plus a minimal README entry; existing M5D3–M5D7F sources/tests/assets/asmdefs remain read-only.

## AC pre-gate status

| AC | Status | Independent basis |
|---|---|---|
| AC-M5D7G-001 | **PASS by contract** | Exact three leaves, fixed role order, typed missing, and no read after missing probe are clear. |
| AC-M5D7G-002 | **PASS by contract** | M5D7B's five non-default classifications map to M5D7E decoded candidates without selection/transformation. |
| AC-M5D7G-003 | **PASS with P2 seam note** | One-call role sequencing and temp non-selection are clear; close ordering needs observable seam evidence. |
| AC-M5D7G-004 | **CONDITIONAL — P1-001** | Presence/read race categories are specified, but short-read precedence/protocol is ambiguous. |
| AC-M5D7G-005 | **BLOCKED — P1-001** | The current `byte[]` full-read seam cannot prove complete versus partial/zero-progress input. |
| AC-M5D7G-006 | **PASS by dependency boundary** | M5D7B/E defensive-copy and repeated-getter guarantees are already Verified; M5D7G must not retain the source buffer. |
| AC-M5D7G-007 | **PASS by contract** | Pre-port path validation, full-path normalization, fixed leaves, and no traversal inputs are explicit. |
| AC-M5D7G-008 | **PASS by contract** | Unexpected/fatal/invalid-port failures escape without a partial public batch; role/candidate validation is delegated to M5D7E. |
| AC-M5D7G-009 | **PASS by contract** | Exact filenames, BCL presence/read boundary, no mutation/clock/Unity, and no atomicity/lease overclaim are explicit. |
| AC-M5D7G-010 | **NOT YET EXECUTABLE** | This is a contract pre-gate; implementation and focused/full evidence do not yet exist. |

## Recommendation

**Do not approve yet.** There is no P0, but P1-001 must resolve the full-read protocol and exception precedence before Terra implementation. After defining complete-read versus empty-file versus protocol-invalid short-read semantics, the contract is otherwise a safe, engine-free observation seam; no per-root observer lease should be introduced. PASS is available after that amendment and a later implementation review confirms the exact three-role order, defensive ownership, and allowlist.

## Second-pass review after full-read amendment

The amended contract closes P1-001. This second pass remains contract-only; no implementation or Unity tests were run.

- The internal read port now returns a typed full-read-and-close result. `CompletedAndClosed` with a non-null byte array is the sole accepted success state, including a genuine zero-byte file. The production operation owns `FileAccess.Read`/`FileShare.Read`, expected-length looping, premature-zero handling, and close-before-return.
- The latest precision amendment adds nonnegative `ExpectedLength` and `BytesRead` fields. The adapter requires `ExpectedLength == BytesRead == Bytes.Length` and each count `<= int.MaxValue` before decode; exact empty is `0 == 0 == byte[0].Length`. Negative, over-limit, count-mismatch, incomplete, or not-closed results are now unambiguously protocol violations.
- The classification precedence is now explicit: actual BCL disappearance/access/read errors after a present probe, including production premature EOF, become per-role `Unreadable`; a missing-after-probe remains `Missing` for that observation; injected default/unknown/incomplete/not-closed/null/short protocol results throw `InvalidOperationException` before decoder invocation and cannot produce a partial batch. This removes the previous `EndOfStreamException` ambiguity and prevents prefix decoding.
- The seam now names `probe → open/read → close → return` events and AC-005 requires these before the next role probe. AC-004/005 separately require per-role race/fault behavior, empty-file `Invalid`, and no later-role observation after an escaping fault.
- The no-lease choice is safe and explicit: the batch makes no cross-file atomicity or generation claim, and the future launch consumer must pass preservation intents to M5D7F for mandatory source revalidation. M5D7G does not infer coordination from M5D7D/F leases.

### Second-pass residual P2

- The complete role/classification/race matrix and `FileShare.Read` seam are contractually required but await implementation evidence; later tests should cover all three roles and all five M5D7B classifications rather than rely only on representative cases.
- The no-observer-lease boundary should remain visible in the eventual launch-orchestration contract so a caller cannot accidentally treat this batch as an atomic snapshot. This is a consumer-documentation follow-up, not a M5D7G blocker.

## Second-pass AC status

| AC | Second-pass status | Basis |
|---|---|---|
| AC-M5D7G-001 | **PASS by contract** | Fixed three leaves, order, typed missing, and no read after missing are exact. |
| AC-M5D7G-002 | **PASS by contract** | All five M5D7B classifications map to role-correct M5D7E candidates without selection/transformation. |
| AC-M5D7G-003 | **PASS by contract** | Role call order, close-before-next probe, selection absence, and temp non-selection are explicit. |
| AC-M5D7G-004 | **PASS by contract** | Per-role recoverable faults and present/disappear, premature EOF, and missing/appear races have distinct classifications. |
| AC-M5D7G-005 | **PASS by contract** | Typed `CompletedAndClosed`, non-null bytes, exact `ExpectedLength == BytesRead == Bytes.Length` including empty `0/0/0`, no prefix decode, and exact FileAccess/FileShare/close events are closed. |
| AC-M5D7G-006 | **PASS by dependency boundary** | Defensive transfer and getter repeatability rely on Verified M5D7B/E guarantees and are explicitly required. |
| AC-M5D7G-007 | **PASS by contract** | Pre-port non-root path validation, normalization, fixed leaves, and traversal exclusion are explicit. |
| AC-M5D7G-008 | **PASS by contract** | Invalid port/result/default/fatal paths escape without a partial batch or later-role observation. |
| AC-M5D7G-009 | **PASS by contract** | Read-only BCL operation, no mutation/clock/Unity, and no atomicity/lease overclaim are explicit. |
| AC-M5D7G-010 | **PASS as a future gate** | Focused/full execution and Luna P0/P1 zero remain implementation-stage checks; the contract does not claim them prematurely. |

## Second-pass verdict and recommendation

**PASS — P0=0, P1=0, residual P2=2. Recommend Astra advance M5D7G to Approved.** The amended typed read protocol resolves the only acceptance-blocking ambiguity. Preserve the explicit no-lease/non-atomic boundary and require the later implementation review to verify the complete event order, defensive ownership, and exact allowlist.
