# Costume CUA — Luna R10 NonJson P1 closure

Date: 2026-09-27 (Asia/Seoul)

## Scope

Narrow static recheck of the sole P1 from the R10 NonJson boundary review: preservation of `MemoryPort.ReplaceCalls` across Routes A, B, and C. Unity was not run and no implementation file was modified.

## Input

- Corrected adapter-test SHA-256: `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`
- Prior P1 review: `docs/verification/2026-09-27-costume-cua-luna-r10-nonjson-boundary-review.md`
- R10 focused XML SHA-256: `C2AFA45FE75BDF9CC1E8A9515C0117AC822C9D5B5E4DB6E9F108C2771BAB7DBD`
- R10 focused log SHA-256: `2D2C9E45A5275333AD20686FA56C51A8057C97B7CD2F37050954086848B309A0`

## Closure checks

The corrected test captures `var replaceCalls = port.ReplaceCalls` immediately after the initial published fixture/view setup. Both branches of the common NonJson test then pass that baseline to `AssertNonJsonPreserved`:

- binding-only Route B returns through the helper without `ReplaceDefinition` or `TrySelect`;
- catalog/selection Route A and remaining Package Route C use the same helper after their candidate operation and post-injection view capture.

`AssertNonJsonPreserved` now asserts `port.ReplaceCalls == expectedReplaceCalls` alongside the existing old published/media/binding identity, move count, projection count, persistence status, and deep view equality. This closes the exact missing count invariant without changing route membership, expected `Identity`/`Revision`/`Clips`/`Selection`/`Package` results, public surface, or runtime code.

## Verdict

**PASS — P0=0, P1=0, P2=0. R10-P1-001 CLOSED.**

R10 focused XML/log are consumed historical evidence and remain immutable. Fresh R11 focused/full stems will still be required for execution after a new authorization; this closure review does not authorize R11.

