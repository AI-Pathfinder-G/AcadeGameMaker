# C2 Unity memory-cutover implementation handoff

Date: 2026-09-28. Status: implementation handoff; not verified or accepted.

Trace: `REQ-M5D7QC2-001..007`; intended acceptance coverage
`AC-M5D7QC2-001..009`.

## Bounded implementation

- `ProfileResetMemoryCutoverV1` reacquires C1's held-root authority only after
  launch-cohort/root witness checks and C1's pre-lease root validation.
- The router has an owner-bound terminal-only lane: it disables both maps,
  removes callbacks, suppresses semantic publication, transfers a fresh disabled
  action collection, and records the old disposal attempt before invoking it.
- The desktop launch adapter keeps the historical launch receipt unchanged,
  retains two launch-root and two router witnesses, never follows a reflected
  foreign router for containment, and owns the new current-session cell and
  take-once reset receipt.
- C2 outcomes and all receipt getters revalidate their closed rows. The final
  receipt retains only Profile-minted detached final-disk proof plus copied
  transaction/marker/target/archive/default-document/generation evidence; it
  retains no live lease, prepared proof, router, capability or C1 authority.
- The terminal capability has a private constructor, router mint witness and
  consume-on-transfer check. Router/adapter each retain an independent
  terminal lifecycle witness; failure containment disables/removes/disposes
  only the exact owned wrappers and retains historical launch evidence.
- Receipt construction and validation occur after final disk and pair reprobes;
  receipt assignment is the final successful in-memory assignment.
- `CurrentSessionProfileV1` reconstructs and checks the exact planner default
  canonical document on every consumption, with an independent weak-table
  witness for root/action/generation/document correlation. The capability uses
  a separate weak-table atomic-consumption word, so reflection of its instance
  fields cannot reopen transfer.
- The coordinator has a closed observation/throw control context covering all
  C2 high-level boundaries and Profile forwarding checkpoints. Exact map,
  callback and dispose operation members are separately enumerated for the
  router/adapter owned-operation wrappers; production calls their real action
  exactly once.
- No UI, scene, gameplay, save, archive restore, notification, OS setting,
  network, clock, random, or general action-swap authority was added.

## Remaining delivery gates

No Unity Editor test run was started by this worker. The focused PlayMode test
now includes closed-result/receipt reflection and detached-evidence checks, but
is source-only pending main's compile/run and Luna independent review. The full
disk/lifecycle/reflection/fault/restart matrix in `AC-M5D7QC2-001..010` remains
an execution and independent-verification gate; this file does not report PASS.
