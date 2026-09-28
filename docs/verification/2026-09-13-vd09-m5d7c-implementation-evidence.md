# VD-09 M5D7C recovery transformation core — Terra implementation evidence

- Status: **Verified — Luna R3 PASS (P0=0, P1=0), Astra integration accepted**
- Contract: [M5D7C recovery transformation core](../specs/work-contracts/2026-09-13-vd09-m5d7c-profile-recovery-transformation-core.md)

## REQ / AC mapping

| Requirement / acceptance | Implementation and focused coverage |
|---|---|
| REQ-M5D7C-001 / AC-M5D7C-001 | Exact approved default creates deterministic current-compatible revision `0` document from source revision `-1`, with literal payload and independently calculated hash golden. |
| REQ-M5D7C-002 / AC-M5D7C-002,006 | Metadata/binding recovery projection preserves non-input semantics by value while replacing input with current empty defaults. Malformed and noncanonical inner binding, collection defense, seed/choice/skill/branches, and caller-owned primary provenance are covered. |
| REQ-M5D7C-003 / AC-M5D7C-003 | Current previous promotion preserves canonical override; recovery previous promotion combines `PreviousPromotion|InputRepair`. Caller owns previous-candidate provenance. |
| REQ-M5D7C-004 / AC-M5D7C-004,005,007 | Entry validates sources before overflow; `long.MaxValue`, misuse/default wrapping, exact reason closure, and reflection-bypassed plan fields—including all default/repaired neutral proof fields—fail closed. |
| REQ-M5D7C-005 / AC-M5D7C-008 | New engine-free planner has no source bytes, IO/path/selection/quarantine/save/input-apply/clock/RNG/network/callback authority. |

## Scoped file SHA-256

| File | SHA-256 |
|---|---|
| `ProfileRecoveryPlannerV1.cs` | `C99D1BB752924154F7A36D91C1F279376E422F2DF13B9BCDA2569FCEED28F663` |
| `ProfileRecoveryPlannerV1.cs.meta` | `044B6BCD6CE279110052A4692E8D680E7A7C19166E1EF4126FDF3766CAF96EEA` |
| `ProfileRecoveryPlannerV1Tests.cs` | `F58B4E9877D92D66AAF518795DA0927DEAEAC738C641C5F49223F4BF95FDC44A` |
| `ProfileRecoveryPlannerV1Tests.cs.meta` | `33858F59C5BBBC7CCA03EE30E5CEFBC5E02AEE026389CE6153FC674C834BCB28` |

## Focused execution

One Unity `6000.6.0f1` EditMode attempt targeted `ProfileRecoveryPlannerV1Tests`. Preflight passed editor/licensing-client/entitlement checks, with process-inventory warnings. Unity stopped before results XML at `Connection to channel LicenseClient-me refused` / `Licensing is not yet initialized` (spawned Licensing Client PID `64968`). Per contract, Terra did not retry, terminate processes, or run full suites. This is an environment block, not a test result; the core does not claim file selection, persistence, recovery execution, or input application.

## Astra execution

After identifying and terminating only the stalled project batch PID `60064`, Astra ran the approved wrapper. The first focused attempt exposed and corrected a test-only enum compile typo; no runtime behavior changed for that correction.

| Suite | Result | Failed / skipped / inconclusive | XML SHA-256 |
|---|---:|---:|---|
| Focused EditMode R2 | `8/8` passed | `0 / 0 / 0` | `EECFEEB61CDA87D9E73F3D0CA1EA8066B03A1FF42052645093BDF81A018D361C` |
| Full EditMode | `553/553` passed | `0 / 0 / 0` | `A418A474D7AD42BB8586FEC7C4DCF9E30E6E208D60C4CF3004F504138CD8EAB9` |
| Full PlayMode first run | `575/576` passed | `1 / 0 / 0` | `F4A8BF562DF58405534DD73F2F52C05D02AFD7CC0979938EDBD514F017F6C06A` |
| Full PlayMode clean recheck | `576/576` passed | `0 / 0 / 0` | `7E0EDAA4CCCFF0A8AD9ED188D0C4FF6EBB166B3096BF5EC5C4C2361D993E1589` |

The first PlayMode failure was contaminated by the known unrelated `GameInputActions.Gameplay.Disable() has not been called` finalizer warning. The unchanged clean recheck passed 576/576. Result files are under `artifacts/m5d7c-20260913/`.

## Luna P1 coverage repair and final R3

The final focused matrix adds malformed and noncanonical binding recovery through both planner entries, literal default payload/hash assertions, revision `0`/arbitrary/`long.MaxValue-1` and overflow, and non-empty tutorial/progression collection preservation with defensive source mutation.

| Final suite | Result | Failed / skipped / inconclusive | XML SHA-256 |
|---|---:|---:|---|
| Focused EditMode R3 | `10/10` passed | `0 / 0 / 0` | `D628E651301EA8ACCC6A20B85A37F0DCC20067F5670C3FEEE61FDC5F99371738` |
| Full EditMode R3 | `555/555` passed | `0 / 0 / 0` | `CB2DCDBC014965AC41FFBB49F18B209F6A14BA8006414D9D91856B239B2EF4B0` |
| Full PlayMode R3 | `576/576` passed | `0 / 0 / 0` | `0B4542CD5D566A1A7122AFD45DE166687342B9F7B70A1C81C54E3E8E8B831B44` |

R3 supersedes the earlier result set for final acceptance.
