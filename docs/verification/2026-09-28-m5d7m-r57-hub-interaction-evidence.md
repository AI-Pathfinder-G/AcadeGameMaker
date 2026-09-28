# M5D7M R57 HubPresentation interaction diagnostic — execution evidence

- Date: 2026-09-28
- Contract: `docs/specs/work-contracts/2026-09-27-m5d7m-duplicate-liveness-recovery.md`
- Scope: Approved diagnostic child `REQ-M5D7M-005/006/008`; this record is
  limited to `AC-M5D7M-R1-001..004` and parent `AC-M5D7M-005/006/009`.
- Status: **PASS for the R57 added-prefix diagnostic; not a production fix,
  full-order acceptance, or closure of AC-CUA-009.**

## Authorized execution and result

R57 ran once in Unity `6000.6.0f1` with the authorized Camera, Combat,
HubPresentation, Input, and full `DesktopProfileLaunchAdapterV1Tests` regex
prefix. The exact task-created main Unity PID was `38724`, started at
01:16:09 KST; its AssetImportWorker child PID was `35404`.

The run exited naturally. No process was stopped, no retry occurred, and the
1,500-second global watchdog did not expire. The D4 duplicate-adapter test
appeared at log line `1598`; the log subsequently advanced and the complete
result XML was written before the same-log 180-second watchdog condition could
be met.

| Metric | Result |
|---|---:|
| Total / passed | `562 / 562` |
| Failed / skipped / inconclusive | `0 / 0 / 0` |
| Duration | `941.6734618s` |
| XML SHA-256 | `440F7D7A36C10F78D9FFA5D8878DF7E016361FD01B2C733ED9564AE89AD27A8B` |
| Log SHA-256 | `6B9C025ED81628C429BA90345F553CBBBE8A3B3D94C1BBAB02E165FE2911D8A9` |

The immutable R57 outputs are
`artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r57-hub-interaction.xml`
and `.log`.

## Acceptance-criterion evidence

- `AC-M5D7M-R1-001`: PASS — a complete XML was produced within the bounded
  diagnostic execution, with all 562 tests passing.
- `AC-M5D7M-R1-002`: PASS — neither the main Unity process nor its worker was
  stopped; only the authorized R57 outputs were created and prior artifacts
  were preserved.
- `AC-M5D7M-R1-003`: PASS — the four CUA execution-baseline hashes remained
  unchanged. The pre-R57 porcelain count was `499`; the post-run execution
  observation was `500` (`+1` Luna R57 pre-gate record). No R57-generated
  temporary `InitTestScene` pair was observed, and no existing scene pair was
  removed.
- `AC-M5D7M-R1-004`: PASS — the result rejects the hypothesis that the added
  HubPresentation prefix alone produces the D4 liveness condition. It requires
  broader full-order/lifecycle diagnosis and authorizes no behavioral fix.

Parent `AC-M5D7M-005/006/009` receive this bounded execution evidence only:
the specified interaction prefix completed without the liveness failure,
artifacts and workspace evidence were preserved, and the next diagnostic gate
remains necessary.

## Source/test baseline preservation

| Path | SHA-256 |
|---|---|
| `CostumeUnityMediaPackageV1.cs` | `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44` |
| `CostumeUnityPresentationAdapterV1.cs` | `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB` |
| `CostumeUnityPresentationAdapterV1Tests.cs` | `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F` |
| `CostumeUnityViewPresenterV1Tests.cs` | `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00` |

These unchanged hashes preserve the diagnostic boundary: R57 does not infer a
CUA implementation or test correction. The HubPresentation added prefix
passes, but the unfiltered full-suite D4 liveness cause remains unresolved and
`AC-CUA-009` remains **open**.
