# VD-01 M2 Ignored-Pair Cast-Hit Filter Addendum Pre-gate

- Date: 2026-09-01
- Addendum: `docs/specs/work-contracts/2026-09-01-vd01-movement-m2-ignore-pair-hit-filter-addendum.md`
- Requirements reviewed: `REQ-MOV-001`, `REQ-MOV-004`, `REQ-MOV-006`; affected `REQ-COM-001`, `REQ-COM-004`, `REQ-COM-005`
- Acceptance criteria traced, not closed: `AC-MOV-001`, `AC-MOV-004`, `AC-MOV-005`; affected `AC-COM-001`, `AC-COM-003`
- Reviewer: Luna
- Decision authority: Sol
- Result: **PASS — P0=0, P1=0**

## Review disposition

Luna reviewed the failed M3E1A executable evidence and the complete narrow Movement addendum. The read-only filter sits inside the existing `FindBestHitAt` query transaction after null/self/trigger rejection and before predicate, quantization and ordering. It neither mutates pair state nor adds a query, sync, clock, public seam or second gameplay owner. M3E1's `+110` mutation after completed tick `t` first affects Movement at `t+1`, matching the staged contract.

The addendum limits its raw 32-entry buffer claim to verified count below 32 rather than inventing a saturation recovery or changing frozen M2 behavior. False restoration affects the next query, same-instance post-death restoration remains forbidden by M3E1, and fresh collider references begin false. Required evidence covers every shared probe/resolution path, nonignored actor/environment preservation, M3C2/M3D2 geometry, a pair-state-changing 30/60/144 trace and full regressions.

## Closed nonblocking recommendations

Before approval, Sol replaced the unrelated affected `REQ-COM-003` citation with lifecycle/target requirements `REQ-COM-004` and `REQ-COM-005`. The failure evidence now records exact source, XML and log paths plus the precise Unity executable arguments and test filter.

## Approval boundary

Sol approves only the addendum allowlist. Passing implementation evidence reopens the M3E1A collision feasibility retry; it does not authorize M3E1 B0/B1 by itself and closes no full acceptance criterion.

## Ollama utilization record

The addendum records Kimi K3 and GLM 5.2 as used and accepted in part after Sol screening. MiniMax M3 returned empty final content twice while consuming output in hidden thinking, without a quota/rate/auth/model/network error, and is recorded as failed and replaced. Luna reviewed the GPT-owned contract and retained no Ollama decision authority.
