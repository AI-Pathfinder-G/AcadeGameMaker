# C2R R13 Terra test correction — Luna pre-gate

Date: 2026-09-28. Test-only review; no runtime edits or Unity execution.

## Frozen test inputs

- `ProfileResetRestartBootstrapV1Tests.cs`: `41A9F04F76C9771CB44FC1A35A0A3823952101C8B4ACF28AB4E575268A67E320`
- `ProfileResetRestartProcessV1Tests.cs`: `F612D9E042D2748BA2D23E2C35CCCA693C1225EA0356A8A9AF41F9D2E0545AD4`

## Review result

P0: 0. P1: 0 in the reviewed test corrections.

The checkpoint barrier assertions now compare `FileAttributes` against a typed
zero, preserving the directory/reparse safety checks and the before/after-delete
policy. The reflected-corruption row now asserts the exact wrapped
`TargetInvocationException` and inner malformed-state `InvalidOperationException`,
then retains no-actions, no-UTC, no-Prepare, no-receipt, no-fallback, and
teardown logging checks. This is an intentional fail-closed expectation, not a
suppression of the error.

The process fixture’s corresponding enum comparison is corrected without
weakening its external-environment failure behavior. Bootstrap cleanup remains
ever-issued and idempotent: it validates owned temp ancestry and descendant
reparse absence before deletion, while a second cleanup sees the absent root.

These are test/evidence corrections only. A fresh R13 rerun must still produce
zero failed, skipped, or inconclusive rows before any AC closure; process
AC-005/006 remain dependent on Main’s four separately orchestrated phases.
