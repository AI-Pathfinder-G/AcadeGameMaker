# M5D7M R58 low-cost suffix diagnostic — execution evidence

- Date: 2026-09-28
- Contract: `docs/specs/work-contracts/2026-09-27-m5d7m-duplicate-liveness-recovery.md`
- Scope: Approved diagnostic child `REQ-M5D7M-005/006/008`, with evidence
  limited to `AC-M5D7M-R1-001..004` and parent `AC-M5D7M-005/006/009`.
- Status: **PASS for the R58 low-cost suffix selection; diagnostic only.
  AC-CUA-009 remains open.**

## Authorized execution and result

R58 ran once in Unity `6000.6.0f1` using the authorized R57 prefix plus the
cheap suffix selection. The task-created main Unity PID was `36788`, started
at 01:39:38 KST, with AssetImportWorker PID `17316`, started at 01:39:43 KST.
It exited naturally. The D4 duplicate-adapter test appeared at log line `1597`,
then completed; its same-log 180-second and increasing-CPU watchdog did not
fire. No process was stopped and no retry occurred.

| Metric | Result |
|---|---:|
| Total / passed | `985 / 985` |
| Failed / skipped / inconclusive | `0 / 0 / 0` |
| Duration | `944.3292552s` |
| XML SHA-256 | `5E9047F3AB928BB2EEF1B309F24AE9CC4D0D33CDC5FB77D3022DB8205D2FC889` |
| Log SHA-256 | `B2B12FD6C41F303196FE4663586783767C5777813419DD20B0D1D4068F4EE81E` |

The immutable outputs are
`artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r58-cheap-suffix.xml`
and `.log`.

## Fixture inventory

The result XML contains `42` fixtures whose totals sum to the expected `985`:

| Group | Passed / total |
|---|---:|
| Camera | `27 / 27` |
| Combat | `375 / 375` |
| HubPresentation | `4 / 4` |
| Input | `39 / 39` |
| Input.Unity | `285 / 285` |
| Movement | `29 / 29` |
| Transfer.Unity | `31 / 31` |
| Presentation.Unity (CUA selection) | `195 / 195` |

## Acceptance-criterion evidence

- `AC-M5D7M-R1-001`: PASS — complete XML was produced within the authorized
  bounded run, with `985/985` passing.
- `AC-M5D7M-R1-002`: PASS — no task process was stopped, no existing artifact
  was overwritten, and no retry was issued.
- `AC-M5D7M-R1-003`: PASS — the execution baseline source/test hashes were
  unchanged. Pre-R58 porcelain had `501` entries; post-run observation had
  `502` (`+1`, Luna R58 pre-gate). No R58-generated temporary test-scene pair
  was observed; authorized output was confined to the R58 artifact paths.
- `AC-M5D7M-R1-004`: PASS — R57 prefix plus R58 cheap suffix, including the
  CUA selection, completed. The finding narrows the remaining order/lifecycle
  investigation to the five Input.Unity sibling fixtures not selected here; it
  does not identify a specific culprit or authorize a behavioral fix.

For parent `AC-M5D7M-005/006/009`, this is bounded execution evidence that
the selected prefix/suffix order is live and that preservation boundaries held.
It does not replace the unfiltered full PlayMode result required by
`AC-CUA-009`, which remains **open**.

## Source/test baseline preservation

| Path | SHA-256 |
|---|---|
| `CostumeUnityMediaPackageV1.cs` | `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44` |
| `CostumeUnityPresentationAdapterV1.cs` | `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB` |
| `CostumeUnityPresentationAdapterV1Tests.cs` | `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F` |
| `CostumeUnityViewPresenterV1Tests.cs` | `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00` |

No source, test, package, ProjectSettings, scene, or existing evidence record
changed under this diagnostic. R58 supplies no CUA implementation correction
and makes no claim about a specific remaining Input.Unity sibling fixture.
