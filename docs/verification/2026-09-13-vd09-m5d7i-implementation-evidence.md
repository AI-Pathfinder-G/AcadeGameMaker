# VD-09 M5D7I Input Binding Override Apply Adapter — Terra implementation evidence

Status: Verified — Astra accepted the final deterministic results after [Luna post-review](2026-09-13-vd09-m5d7i-luna-postreview.md) PASS (P0=0, P1=0; residual P2=3). This verifies isolated candidate override-application detection only, not repair transformation, persistence, launch coordination, or gameplay entry.

- Contract: [Approved M5D7I](../specs/work-contracts/2026-09-13-vd09-m5d7i-input-binding-override-apply-adapter.md); [Luna R2 pre-gate PASS](2026-09-13-vd09-m5d7i-contract-pregate.md).
- Scoped changes: candidate-only Input.Unity adapter/meta, its PlayMode fixture/meta, and only the two required Profile asmdef reference additions.
- REQ-M5D7I-001..005: PlayMode fixture covers the real empty/default and current generated override round trips; stale unknown binding-ID warning-only mismatch; current/default/reflection-invalid input and null/disposed/enabled/already-overridden/preflight candidate rejection; and a fresh instance port for exact `Save`, `Save,Load`, and `Save,Load,Save` paths.
- REQ-M5D7I-003..005: each closed recoverable type is injected independently at load and post-load save, with parse failure, mismatch, discard proof, unavailable verified-proof getter, and no-extra-call assertions. The legal five-row result matrix and reflection-invalid outcome/failure/disposition/requested/verified combinations are fail-closed in the fixture.
- REQ-M5D7I-006..007: unexpected and `OutOfMemoryException` port faults propagate, a following fresh call succeeds, and static review checks the concrete prohibited live-router type/member patterns rather than comment text. The adapter retains no test port or candidate state and adds no persistence, recovery, file, clock, or generated-wrapper authority.
- Diagnostic focused execution (Astra, pre-remediation test SHA `D233440D8C85EED45CB1875168E55722BF33A01A505A6C4CD26E1BF6D40272EB`): 6 passed / 2 failed. AC003 used an invalid top-level array rather than the pinned Input System `bindings` object and therefore correctly reached `LoadFailed`; AC005 supplied an injected port whose deliberately empty preflight report did not represent the manually overridden candidate. Neither result identifies a production behavior defect.
- Remediation changes the stale-ID fixture to start from a real generated override envelope, replace only its binding GUID with a valid unknown GUID, canonicalize through M5D5, and assert its expected warning with `LogAssert.Expect`. The already-overridden candidate check now uses the production two-argument adapter; the independent injected non-empty preflight assertion remains.
- Diagnostic focused execution r2 (Astra): 7 passed / 1 failed. The remaining AC005 disposed-candidate check called generated-wrapper `Dispose()`, which uses deferred `Destroy` in PlayMode; the asset therefore remained observable during the same frame. The fixture now uses isolated `DestroyImmediate(disposed.asset)` before applying and conditionally disposes only if Unity still reports the asset alive. This makes the documented destroyed-candidate precondition deterministic without changing the wrapper or adapter.
- Astra execution history after the r2 fixture correction: focused PlayMode r3 passed 8/8; full EditMode passed 647/647; full PlayMode recorded 583/1. The sole full-Play failure was a later generated-wrapper finalizer diagnostic (`Gameplay.Disable() has not been called.`) caused by AC005 intentionally enabling a candidate map and disposing it while still enabled. The fixture now disables that map in a `finally` before wrapper disposal. No production behavior changed and Terra did not rerun Unity.
- Final Astra runs (all with failed/skipped/inconclusive zero): focused PlayMode r4 `8/8`, artifact SHA-256 `d4b5abf53ca9f2e7c31223857ea900fe308331a21b92f8ca2b6f197fdcf2ba1c`; full EditMode r2 `647/647`, artifact SHA-256 `00281b4e3ab0846e2883454ffe12b8990cb60adad95b52a4a9c3a20d0749c8bb`; full PlayMode r2 `584/584`, artifact SHA-256 `e96b442744fed80cd3d8b5289c4ab13d208ea96e0123bdaacb3e1dc6b9c2c063`.

## Current scoped hashes

| Scope | SHA-256 |
| --- | --- |
| Runtime adapter | `92572bb90723c213194d3c9dd221c113bd135fcfd2f5ad091e9b27f4e9a473e1` |
| Runtime adapter meta | `30658173d410337dbd2fe36d1002d7a4418d056d7ae344cb2b0694d1061287f1` |
| Runtime asmdef | `cd3e47dd94bccb9e537ca7af4c6029fc8327ea0154c90ec7b6fd305218fe4c0e` |
| PlayMode fixture | `30654c5dd8555babe616ac484b4a3bf84bf34ca7b9ced774e1efdd72967c5cf4` |
| PlayMode fixture meta | `37e9373654891cf44188b55ac2baf601ad7d78c1d5185769408347c43a9e5203` |
| PlayMode test asmdef | `5f79096ca7294d07c77af7aa8612b439114148c415fe53a8b07dd19207b6595f` |
| Approved contract | `6d31da4dbb4a04eb521067e179578ddcf6fc6816e5f37319fed4c5a0898dd066` |
| Luna R2 pre-gate | `0fd1b8cb1ce78a07170e301c0fe3210b750f618dbd8937e408772e3bb2d8cca0` |

- Static checks completed: scoped diff whitespace check and runtime forbidden-authority scan. Terra did not run Unity from the sandbox. The final results above do not replace Luna post-review or Astra acceptance.
