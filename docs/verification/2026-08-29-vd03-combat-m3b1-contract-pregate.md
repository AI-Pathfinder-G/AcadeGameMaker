# VD-03 M3B1 Reaction Unity Adapter Contract Pre-gate

- Date: 2026-08-29
- Contract: [VD-03 M3B1](../specs/work-contracts/2026-08-29-vd03-combat-m3b1-reaction-unity-adapter.md)
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`, affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance scope: `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005`
- Independent reviewer: Luna
- Sol decision: PASS; contract advanced to `Approved`

## Gate result

Luna's first review found no P0 but identified eight P1 gaps. Sol revised the contract and `SYSTEM-CONTRACTS`; Luna's final independent review found no remaining P0/P1 contradiction.

The approved contract closes these boundaries:

- fixed order is `transfer -200 → combat -190 → reaction -180 → movement default`;
- combat tick `t` uses only the immutable carried reaction view from `t-1`, preserving exactly N later attackability observations for an N-tick walker window;
- structural validation runs before M1, while result-dependent M3A inputs are mutation-free validated for both enemies after M1 and before either reaction session commits;
- post-M1 reaction validation failure retains both reaction sessions, carried view and adapter queues, retains already completed upstream publications and fail-stops rather than claiming global rollback;
- transfer removal and apply result slots are validated independently, so one target may clear before another target applies in the same tick with increasing revisions;
- reset requires an entirely idle exact transfer input summary and an exact current combat reset discriminator;
- recovery candidates are future-only, consumed once and tombstoned even when impact or death suppresses them;
- startup requires the pinned walker and surveyor combat registrations with matching transfer co-authoring;
- M3B1 observes M2B's existing death-removal handoff and never schedules a duplicate.

## Independent review history

| Review | Result | Resolution |
|---|---|---|
| Luna pass 1 | FAIL, eight P1 | Conditional horizon, same-tick multi-edge transfer, reset markers, explicit seams, system order, recovery lifetime, roster and death-removal ownership were revised. |
| Luna pass 2 | FAIL, one P1 | Impossible pre-M1 result-dependent horizon test was replaced with structural preflight plus exact post-M1 `ValidateNext` on both sessions. |
| Luna final | PASS | All eight P1 findings closed; implementation must preserve the approved validate-both-before-process ordering. |

## Ollama contract-pass record

| Lane | Outcome | Sol screening |
|---|---|---|
| GLM 5.2 | used and accepted in part | Accepted the cross-phase rollback and reset/atomicity warnings. Rejected a nonexistent Unity main-thread snapshot race and an incorrect N-1 carried-window claim. |
| Kimi K3 | not applicable at contract gate | Required implementation experiment begins only from the frozen Approved abstract contract. |
| MiniMax M3 | not applicable at contract gate | Required validation-fixture proposal begins only from the frozen Approved abstract contract. |

No Ollama output has approval, canon, merge or integration authority. No repository content, local path, credential, personal data or secret was sent to a cloud model.

## Approval boundary

This PASS authorizes only the files and behavior listed by M3B1. It does not authorize enemy locomotion, actual recovery-cycle production, surveyor attacks, projectiles, collision sampling, scene/prefab work, presentation, assets or project settings. M3B1 remains partial substrate evidence and cannot close full enemy-distinction acceptance criteria.
