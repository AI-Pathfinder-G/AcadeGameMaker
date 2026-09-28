# VD-03 M3A Regular Enemy Reaction Core Contract Pre-gate

- Date: 2026-08-28
- Contract: [VD-03 M3A](../specs/work-contracts/2026-08-28-vd03-combat-m3a-regular-enemy-reaction-core.md)
- Requirements: `REQ-COM-001`, `REQ-COM-002`, `REQ-COM-004`, `REQ-COM-005`, affected `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- Partial acceptance scope: `AC-COM-001`, `AC-COM-003`, affected `AC-WT-002`, `AC-WT-005`
- Independent reviewer: Luna
- Sol decision: PASS; contract advanced to `Approved`

## Gate result

Luna's final review found no remaining P0 or P1 issue. The approved contract keeps all new types internal, leaves M1/M2A and Unity execution order unchanged, and returns all regular-enemy Heavy physical modifiers to VD-02's sole authority.

The gate specifically closed these boundaries:

- transfer revisions are exactly `1..int.MaxValue`, with `-1` reserved only as the unseen sentinel;
- walker natural 30-tick opening is a Sol-owned local balance value required for the `REQ-COM-001` basic-only completion path;
- 90-tick walker impact opening and 75-tick surveyor fire lock remain the direct `REQ-COM-005` values;
- raw Heavy state is separate from effective directives on death and reset;
- reset consumes one global tick only after a completed gameplay tick and a VD-02 baseline clear;
- singular event input is owned by M3A, while actual same-tick producer arbitration is deferred to M3B;
- all window ends are exclusive and all failed inputs preserve the complete prior state.

## Ollama contract-pass record

| Lane | Outcome | Sol screening |
|---|---|---|
| Kimi K3 | used and rejected | Suggested public surface and threshold-equality behavior conflicted with the frozen internal exact-tick contract; Terra will implement from the Approved GPT contract. |
| GLM 5.2 | used and accepted in part | Retained its generic same-call ordering, reset/death priority and duplicate-guard concerns; rejected unrelated threshold-crossing assumptions. |
| MiniMax M3 | failed and replaced | Returned no usable final fixture proposal because output budget was consumed by uncontrolled reasoning; no HTTP 429 or quota failure. Luna/Terra own the replacement matrix. |

No Ollama output has approval, canon, merge or integration authority. No repository content, local path, credential or secret was sent to a cloud model.

## Approval boundary

This PASS authorizes only the engine-free M3A files named in the contract. It does not authorize a Unity adapter, AI movement/attacks, M2A observation integration, project settings, scenes or assets. Final implementation verification must cite the acceptance-criterion IDs above and record all three Ollama lane outcomes again for the code-bearing milestone.
