# VD-09 M5D7O hub-menu presentation controller — Luna final post-review

- Review date: 2026-09-20
- Reviewer: Luna (`gpt-5.6-luna`), independent of Terra implementation
- Contract: [M5D7O hub-menu presentation controller](../specs/work-contracts/2026-09-14-vd09-m5d7o-hub-menu-presentation-controller.md)
- Compared against: `AGENTS.md`, `docs/README.md`, `CONTEXT.md`, ADR-0032, `docs/agent-operating-model.md`, the Approved M5D7O contract, amended pre-gate, Terra implementation evidence, M5D7N Verified handoff, implementation/test sources, Unity test XML, and M5D7O allowlist.
- Review scope: independent AC-by-AC source, test, execution evidence, authority-boundary, and allowlist audit. No runtime, test, contract, or README file was changed.
- Supersedes: the 2026-09-14 licensing-blocked reserve review below. Its environment history remains factual but is no longer the current verification result.

## Verdict

**PASS — P0=0, P1=0, P2=1.** All required Unity runs report zero failures, skips, and inconclusive cases. AC-M5D7O-001 through AC-M5D7O-007 pass independent source/test review; AC-M5D7O-008 passes the four XML-backed execution gates. M5D7O is ready for Astra's final integration/status decision.

## Acceptance-criterion review

| Criterion | Result | Independent basis |
|---|---|---|
| AC-M5D7O-001 | **PASS** | `Create` validates the incoming handoff and derives profile continuation only from `Primary`/`Previous`. Focused tests cover Primary with failed save, Previous with uncertain commit, Default with successful bootstrap save, and Default with preservation denied. The clean Primary receipt is separately exercised for notification absence. |
| AC-M5D7O-002 | **PASS** | The view returns the exact zero-based Continue/New Game/Settings/Quit order, rejects invalid indices, keeps the Default Continue entry disabled, and leaves the other entries available. Disabled Continue leaves view and controller state unchanged. |
| AC-M5D7O-003 | **PASS** | Each of the four interactable menu items is exercised. Activation publishes its exact intent once; duplicate activation cannot replace it; the first take succeeds and subsequent takes return false/default. |
| AC-M5D7O-004 | **PASS** | All three typed notification kinds and absence are exercised. Tests assert `TopRight`, visible/absent projection, and exact receipt correlation; foreign and missing required notifications are rejected. Notification state is independent of menu intent. |
| AC-M5D7O-005 | **PASS** | Click and Submit each dismiss once. Menu activation and intent consumption do not dismiss a notice. Source inspection confirms no timer, cancel, navigation, or lifecycle dismissal path. Unknown activation/dismissal enums throw and latch `Failed`. |
| AC-M5D7O-006 | **PASS** | Reflection tests mutate every backing field of the view, typed notification, and intent and require that value's next `Validate` boundary to reject it. They separately mutate controller-owned receipt, projection, view, state, pending-intent, and consumed-intent fields; the next controller boundary throws and latches `Failed`. The runtime getters and commands invoke those validation boundaries. |
| AC-M5D7O-007 | **PASS** | Independent forbidden-authority scan of the controller found zero hits. Source/API inspection confirms no scene/UI/EventSystem/TMP, input-map/router, profile/persistence mutation, file, run/gameplay/narrative, timer, network, RNG, logging, or Quit authority. The implementation/test/asmdef and friend-assembly seam remain within the M5D7O allowlist. |
| AC-M5D7O-008 | **PASS** | Focused M5D7O EditMode, direct M5D7N regression, full EditMode, and full PlayMode XML all have zero failed, skipped, and inconclusive cases. |

### Create-boundary cache and typed-notification integrity

`Create` performs the deep incoming handoff validation, derives the Primary/Previous predicate, computes the expected notification from the exact launch receipt, validates a supplied notification, and rejects a missing or foreign transfer before constructing the controller. It retains the typed notification and independent proof value. Later controller checks compare those immutable copies and their scalar projection proofs without recalculating kind or prose from an enum. The persisted-profile and notification-kind cache fields are included in the reflection mutation matrix. This is fail-closed under the contract's mutation model and preserves the receipt-correlation value for the downstream presentation owner.

`AC-M5D7O-006` mutates one backing field at a time, matching the approved proof-copy contract. It does not model an actor simultaneously rewriting both a value and its independent proof; that is outside the stated acceptance criterion.

## Recomputed Unity execution evidence

The result XML values and SHA-256 hashes below were independently read and recomputed. Each XML root reports `result="Passed"`; test-case counts match the root totals.

| Run | Scope | Passed | Failed / skipped / inconclusive | Duration | XML SHA-256 |
|---|---|---:|---:|---:|---|
| R12 | Focused M5D7O EditMode | 19/19 | 0 / 0 / 0 | 873.3354881 s | `86F3BDDE3BE26EFE51E7047A894BB68162AB99550D190C2A1520BD9B73734835` |
| R13 | Direct M5D7N PlayMode regression | 69/69 | 0 / 0 / 0 | 2613.9075525 s | `D407CCF9F007A3E973ACB8D219026D809E5CFB76B417AFA174B9DB267684BCB5` |
| R14 | Full EditMode | 688/688 | 0 / 0 / 0 | 890.3190608 s | `8EBF5E7CEBA24B4E70BD99470A9AD90554A5401CC65B7351A0C3AB0300F0A292` |
| R15 | Full PlayMode | 803/803 | 0 / 0 / 0 | 3316.0791578 s | `EEC564A3F860A2FF5BEAD5F3CDF305AD289A73B909FEC51EA89D5722192E359D` |

R13 contains exactly 69 `HubEntryHandoffLatchV1Tests` cases, confirming that it is the direct M5D7N regression suite. The focused XML contains the 19 final AC-M5D7O test cases, including the six separately timed AC004 scenarios.

The final exercised source hashes also match Terra's implementation evidence:

- `HubMenuPresentationControllerV1.cs`: `F4E9CD4CEEB704CC26869750EF14FEA4A3402055A1F2D1F51BA40F7333CA60C9`
- `HubMenuPresentationControllerV1Tests.cs`: `218EB71BDD168C59A9DBF03082A37F31741A94AE1A360D62BA4D15C89099FA5F`

## Severity and residual note

- **P0: 0**
- **P1: 0**
- **P2: 1 — downstream M5D7P ownership.** The authored presentation owner must carry the controller-owned typed notification and its receipt-correlation evidence forward. It must not reconstruct notification identity or prose from `NotificationKind` alone. M5D7O itself retains the typed value and is not blocked.

Historical reserve-review note: the earlier review stopped before acceptance because Unity licensing IPC prevented XML production. It correctly made no pass claim at that time; R12–R15 now supply the previously missing execution evidence.
