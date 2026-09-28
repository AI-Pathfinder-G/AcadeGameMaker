# C4 reset-execution bridge — Luna contract pre-gate

Date: 2026-09-28  
Contract SHA-256: `8E1C43C188D460F9CF61CC68BA4333AF486CEC5223C3BDA454B8AF5E79ACA036`  
Status: Review only. No approval, implementation, or runtime verification.

Editorial correction review: current contract SHA-256
`AB3AD44BA6FE4C4A92186AFAD28390F60562894622771068EF2C94B9BF0AE721`.
The only reviewed change is the non-normative duplicate result-matrix wording
correction from `DiskPrepared/DiskPrepared` to `DiskPrepared`; requirements,
acceptance criteria, allowlist, and stop conditions are unchanged.

## Independent result

Contract P0: **0**. Contract P1: **0**. The bounded design is internally
coherent, subject to its explicitly stated C1/C2/C3 prerequisites. The C1-C2
lease gap has two complementary safety boundaries: the durable C1 barrier and
the pair/root/generation-bound execution guard. Only validated C1 Busy or
ConfirmationStale/NoBarrier permits FreshC3Required; DiskPrepared and every
uncertain/terminal result cannot rearm, Cancel, retry, or publish old writers.

The assembly direction is sound: an opaque one-consumer C3 value enters the
neutral Input.Unity executor, no Hub type crosses downward, the exact private
C1 identity/result/proof is preserved, and C2 remains the sole proof/lease/
receipt authority. The guard blocks callbacks, semantic frames, menu intent,
ordinary save, and reset reentry without enabling/replacing actions. Disable /
Destroy and programmer exceptions retain fail-stop semantics.

One editorial typo is non-blocking: the result matrix says
`DiskPrepared/DiskPrepared`; it should read the single exact C1
`DiskPrepared` row.

## AC map and evidence gate

- **AC-M5D7QC4-001–003:** Design covers committed one-shot consumption, exact
  guard latching, one real C1 Begin, typed result validation, and no scalar or
  proof substitution. Requires focused implementation tests.
- **AC-M5D7QC4-004:** Busy and ConfirmationStale/NoBarrier are the only fresh-C3
  handbacks; all other outcomes terminal. Requires exact C1 result seams.
- **AC-M5D7QC4-005–006:** Exact same root/proof/pair handoff and lease-gap
  safety are specified; requires frozen C2 FinalizeReset boundary and injected
  pre/post-C1/C2 evidence.
- **AC-M5D7QC4-007–008:** Result mapping, post-DiskPrepared terminal policy,
  reentry/concurrency, Disable/Destroy, and exact-once behavior are bounded;
  requires implementation matrices.
- **AC-M5D7QC4-009:** Assembly/API/product boundary is explicit and reviewable;
  no public ABI, friend, Profile/C1/C2 algorithm, UI, or unrelated authority is
  authorized.
- **AC-M5D7QC4-010:** Remains entirely pending implementation and required
  focused/regression execution.

C1's disk-only scope is already independently Verified; its use here still
requires fresh C4 integration evidence. C2/C3 frozen prerequisite acceptance
remains pending. Therefore this pre-gate does not mark C4 Approved, implemented,
or verified; Astra must resolve the remaining prerequisite/integration gates
before any Terra work.
