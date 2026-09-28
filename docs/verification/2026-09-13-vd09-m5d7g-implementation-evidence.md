# VD-09 M5D7G Profile Launch Observation Adapter — Terra implementation evidence

Status: **Verified — Astra accepted after Luna implementation PASS (P0=0, P1=0); residual P2=3 are non-blocking evidence-precision notes.**

## Scope

- Contract: [Verified M5D7G](../specs/work-contracts/2026-09-13-vd09-m5d7g-profile-launch-observation-adapter.md).
- Pre-gate: [Luna second-pass PASS](2026-09-13-vd09-m5d7g-contract-pregate.md).
- Independent review: [Luna implementation PASS](2026-09-13-vd09-m5d7g-luna-independent-review.md).
- Added only the M5D7G runtime adapter/meta, EditMode tests/meta, this evidence record, and the minimal README entry. Verified M5D3–M5D7F sources, tests, asmdefs, settings, and assets remain unchanged.

## Requirement and AC mapping

| Coverage | Evidence |
| --- | --- |
| REQ-M5D7G-001..003 | Fixed primary/previous/temp sibling observation, typed `GetAttributes` presence, read-only `FileShare.Read` complete-close port, and exact M5D7E role candidates. |
| REQ-M5D7G-004..005 | Per-role recoverable faults map to unreadable; malformed typed port data and immutable/default/reflection-forged batches fail closed. |
| REQ-M5D7G-006 | No selector, repair, quarantine, save/write/delete/copy/retry, Unity, clock, network, RNG, notification, or lease authority. |
| AC-M5D7G-001..005 | Isolated missing/decoder/order/fault/empty/protocol fixtures cover every role, explicit five-classification expected values, complete close event order, present→disappear/premature-EOF recovery, and default/unknown/null/negative/overflow/short/zero-progress protocol rejection. |
| AC-M5D7G-006..009 | Shared injected-buffer mutation, null/empty/root path rejection, malformed/role-swapped/duplicated/unknown candidates, invalid presence, unexpected/fatal, and static authority checks are in the focused fixture. |
| AC-M5D7G-010 | Fresh focused/full suites are recorded below with zero failed/skipped/inconclusive; independent Luna review and Astra acceptance remain pending. |

## Verification state

The initial sandbox Unity attempt stalled at `[Licensing::Module] Licensing is not yet initialized` before producing NUnit XML. It is diagnostic history only. Astra subsequently ran the fresh focused and full suites in the approved general Windows user session; those current results are recorded below. Luna/Astra review remains pending.

## Focused EditMode R2 diagnostic result

The earlier approved-session focused run compiled and executed `10` tests: `8` passed and `2` failed. Both failures were test-fixture defects, not a runtime finding: the intended count mismatch accidentally supplied equal expected/read/array counts, and the unexpected/fatal tests used a missing source so never reached the read seam. The fixture was corrected without changing runtime behavior. This diagnostic run is not an acceptance result.

| Run | Result | NUnit XML | SHA-256 |
| --- | ---: | --- | --- |
| Focused EditMode R2 | 8 passed, 2 failed | `artifacts/m5d7g-20260913/focused-editmode-r2.xml` | `0F7DA01E71144BE7ED10447FFBBA19246F6C85871683425FBC7F3CEAFF6446ED` |

## Fresh Astra execution

| Run | Result | NUnit XML | SHA-256 |
| --- | ---: | --- | --- |
| Focused EditMode R3 | 10 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7g-20260913/focused-editmode-r3.xml` | `29B2F11F934066668B0FFEEC8616CE92AB2A27489DFDAB097A11BF23970EC9DE` |
| Full EditMode | 622 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7g-20260913/full-editmode.xml` | `5D9D25AC839F54FB4089F6E6E957CC13B97D19EE5F04BADA74CE1C207B89B3D2` |
| Full PlayMode | 576 passed, 0 failed/skipped/inconclusive | `artifacts/m5d7g-20260913/full-playmode.xml` | `6791C7DB50652569040CB33336823C25DDEA5F5042EDAC575A439DFBF0B91B99` |

## Final scoped hashes

| File | SHA-256 |
| --- | --- |
| `ProfileLaunchObservationAdapterV1.cs` | `36663BA4B61F7A31B56BE07689132E04B94B70F3572F5CE3F70AA756FB16582E` |
| `ProfileLaunchObservationAdapterV1.cs.meta` | `8CEA63C429C974B654579EE713C066BB5736FA3454F59DE22845FC65170431F0` |
| `ProfileLaunchObservationAdapterV1Tests.cs` | `22DFFFCD03ADF1AA3E390540A2C7649AD9E3E321CF1310BDCDF7504A04F0B0C1` |
| `ProfileLaunchObservationAdapterV1Tests.cs.meta` | `BBE68988BB8B3735EC9560FF115B6C336EAC290AF7C0090E2449EF644E7CA25B` |
