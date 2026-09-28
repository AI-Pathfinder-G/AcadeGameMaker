# VD-03 M3E1 Contract Pre-gate Evidence

- Date: 2026-09-01
- Contract: `docs/specs/work-contracts/2026-09-01-vd03-combat-m3e1-authored-encounter-composition.md`
- Requirements reviewed: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`; affected `REQ-MOV-001`, `REQ-WT-002`, `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Acceptance criteria traced, not closed by this review: `AC-COM-001`, `AC-COM-003`; affected `AC-WT-002`, `AC-WT-003`, `AC-WT-005`
- Reviewer: Luna
- Decision authority: Sol
- Result: **PASS — P0=0, P1=0**

## Review disposition

Luna rejected the first Review draft with eight P1 and three P2 findings. Sol corrected the complete contract and requested repeated whole-contract review. Subsequent review found and closed three additional P1 contradictions involving valid M1 duplicate results, exact health conservation and the missing Transfer graph-validation seam. Luna's final complete review returned `P0=0`, `P1=0`, PASS. Its last two nonblocking P2 hardening recommendations were incorporated before Sol approval.

## Closed findings

1. Post-death re-entry now requires destruction and a fresh scene instance; same-instance post-death reset rejects without mutation and cannot silently lose Transfer registrations.
2. The Editor authoring assembly has seven exact direct assembly references and seven narrow `InternalsVisibleTo` grants, avoiding an unrealizable transitive-reference assumption or public fallback.
3. First-use construction, exact role health, prior alive/dead state and reset seeds are explicit.
4. The pure input defensively copies canonical request/result arrays, permits M1's valid repeated request ID plus `Duplicate` result, validates exact index alignment and enforces checked per-role applied-damage conservation.
5. Death edges require exact same-tick Applied damage reaching zero; non-reset healing, resurrection, forged scalars, malformed resets and caller mutation reject atomically.
6. The `+110` adapter validates complete Player/Transfer/Combat/Reaction/Behavior/Locomotion/Threat source, graph, tick and reset alignment before staged first-owner adoption.
7. The production enemy Transfer sink, `t+1` removal ownership, collision-pair operation order, pair preflight/revalidation/post-call verification and fail-stop boundary are exact.
8. Evidence now includes affected Weight Transfer cases, protected-file status, no-engine source boundary, fresh-scene restoration and source/rollback scans.
9. Final hardening requires an empty canonical trace on Reset, and constrains every aligned Applied result to a literal roster target with `0 <= AppliedAmount <= request.Amount`.

## Implementation gate

Sol initially approved M3E1 implementation after Luna's final PASS and incorporation of both remaining P2 hardening recommendations. Luna's implementation-verification preparation then exposed a sequencing cycle: gate item 7 requires a fresh committed authored graph, while the initial wording blocked all authored-graph work until all eight items passed. Sol reopened the contract for a narrow sequencing addendum. The addendum requires items 1–6 and 8 before collision-neutral M3E1B0 authoring, provisional item 7 against that committed graph, and only then M3E1B1 or production pair-ignore code. It also requires item 7 to pass again after B1 changes the final committed graph. Luna's complete addendum rereview returned PASS with `P0=0`, `P1=0`; the only P2 was this pending evidence update. Sol incorporated it and reapproved the sequencing addendum on 2026-09-01.

## Ollama utilization record

The contract and UI/art/run outsourcing record report Kimi K3 as used and accepted in part, GLM 5.2 as used and accepted in part, and MiniMax M3 as failed and replaced for the M3E1 follow-up after an empty response without a quota/rate/auth/model/network error. Terra or Luna screens every retained point and Sol alone owns approval and integration.
