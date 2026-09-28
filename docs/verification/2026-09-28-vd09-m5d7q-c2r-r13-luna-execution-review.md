# C2R R13 execution review — Luna AC-scoped digest

Date: 2026-09-28. Runtime remained frozen; no Luna source edits or Unity
execution.

R13 XML: `c2-r13-playmode.xml`, SHA-256
`A9753FD7F5C5059381D4ED91B6AAB1FDC72DBEB520A378B60DE31C03790B199F`.
Result: 38 passed, 14 failed, 0 skipped/inconclusive; duration 83.9884688 s;
Editor PID 12228 logged and exited. Current runtime Adapter SHA is
`C9D2CAE724113BEF186EC82FC5761367534F21A0ADAF7D3437765050279746E8`; frozen
bootstrap source SHA is
`B74F01633BFFFEEC9BE3FAF4E53B451F85C2D21A448896A2E12AEB059ABE263E`.

## Failure classification

The 13 checkpoint failures at bootstrap source line 199 are test expectation
defects: `FileAttributes` enum values are compared to integer zero. The
underlying AC-004/005 barrier-state assertion remains valid; correct the
assertion to compare the enum/flag representation without weakening the
before/after-delete policy.

The reflected-corruption row at line 135 invokes Router Start after deliberately
setting the recovery reservation to `None`. The actual `TargetInvocationException`
with inner `InvalidOperationException("Failed launch state is malformed")` is
the intended fail-closed result. Update the test to assert that wrapped
exception and then verify no actions, no UTC/Prepare, no receipt, no fallback,
and quiet teardown. Do not turn malformed-state rejection into a no-throw
success or relax private-witness validation.

The legacy 12 rows, strict trace, and prior three ManualRepair rows passed in
R13. No runtime P0/P1 is inferred from these 14 test-expectation failures; the
bootstrap test file requires Terra-only assertion corrections and a fresh run.

## Gate

R13 is not acceptance evidence. After the bootstrap-only correction, rerun the
37-row bootstrap set and required non-process filter, preserving zero failed,
skipped, and inconclusive requirements. Process AC-005/006 still require the
four separately orchestrated Main phases.
