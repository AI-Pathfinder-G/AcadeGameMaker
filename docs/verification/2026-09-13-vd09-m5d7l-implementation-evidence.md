# VD-09 M5D7L implementation evidence

- Status: Verified after Luna R2 post-review PASS (P0=0, P1=0, residual P2=2).
- Implemented `REQ-M5D7L-001` through `REQ-M5D7L-008` within the Approved allowlist.
- Added coordinator-local validated observation, preservation, and persistence projections; same-root ref-counted launch leasing; exact candidate ownership transfer; and typed persistence-pending evidence.

## Final deterministic results

- `AC-M5D7L-001` through `AC-M5D7L-011`, focused PlayMode R9: `33/33`, failed/skipped/inconclusive `0` (`artifacts/qa/m5d7l-focused-r9.xml`, SHA-256 `2bc15b1b74656070d3359c21dfaac219d019ff54a86bbd9ff1a8f327ad9b0d51`).
- Direct M5D7I dependency regression: `8/8`, failed/skipped/inconclusive `0` (`artifacts/qa/m5d7l-m5d7i-regression.xml`, SHA-256 `eec8e0ee69e3673222ee741b89513bf533ad1e415562b9e8da473a8afdc9f6d9`).
- Full EditMode R3: `663/663`, failed/skipped/inconclusive `0` (`artifacts/qa/m5d7l-full-edit-r3.xml`, SHA-256 `108a73001ec3ea9fbc15c587cd93b439e8f35173f9466d89db6c805c76f6d248`).
- Full PlayMode R3: `617/617`, failed/skipped/inconclusive `0` (`artifacts/qa/m5d7l-full-play-r3.xml`, SHA-256 `40bfa9aaf4de9cbb820b49df2854dcf266fff1b6e5482aa5e94ae19ed7b2f8dc`).
- `AC-M5D7L-012` is satisfied by the refreshed zero-failure runs and Luna R2 post-review PASS with P0=0/P1=0.

## Artifact identity

- Runtime: `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileLaunchPreparationCoordinatorV1.cs`, SHA-256 `4f29227aad122e89cd71ea2e0447c2b8c7dced59b2616a06d587c5ea1618efc5`.
- Runtime meta: SHA-256 `3775f07a8bd98f84aa0f87f77e446e3079d3b2b4dc884684eaa34e0e9f6159a6`.
- PlayMode fixture: `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileLaunchPreparationCoordinatorV1Tests.cs`, SHA-256 `81d9d630d026ae0d62f23745ab2b40987c7e671e056f040b82b6bb527c9c1ff1`.
- Fixture meta: SHA-256 `a6c6bd9f8dd041d65ff05c7f4f82647da46fa93e578df8db85744f3bc85d661c`.
- Profile friend declaration: `Assets/AcadeGameMaker/Runtime/Profile/AssemblyInfo.cs`, SHA-256 `89652373f2bb5b965eb2353b9708821decd19941094825566af584859fbd96f4`.

## Iteration record

- Luna first post-review P1-001/P1-002 remediation was fixture-only: the fixture now asserts K observation/selection and normalized root/UTC, D/H roots and exact payload/candidate, every actions-port Apply input/candidate identity, cleanup exception precedence, and bounded lease task completion/fault state. Focused R9 and full R3 reruns passed with zero failed/skipped/inconclusive.

- Early focused runs established compilation and the original 12-case baseline; R3 expanded to `30/30`.
- R4/R5 exposed test-fixture expectation defects while exercising hostile action-port and reflection rows. R6/R7 isolated omitted injected exceptions and a fixture-owned enabled-action cleanup warning. These were fixture-only corrections: the hostile port now injects the intended failures and disables its deliberately enabled candidate before destruction.
- R8 passed all 33 focused cases without unhandled Unity logs. The expanded matrix covers action ownership/exception precedence (`AC-M5D7L-006`, `007`, `008`), local proof corruption (`AC-M5D7L-007`, `009`), same-root/different-root lease behavior (`AC-M5D7L-010`), normalized K/D/H seam identity (`AC-M5D7L-006`), and real generated-actions/filesystem paths (`AC-M5D7L-011`).
