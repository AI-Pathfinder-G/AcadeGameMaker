# M5D7M R59 two-cheap-siblings diagnostic — liveness evidence

- Date: 2026-09-28
- Contract: `docs/specs/work-contracts/2026-09-27-m5d7m-duplicate-liveness-recovery.md`
- Scope: Approved diagnostic child `REQ-M5D7M-005/006/008`; evidence is
  limited to `AC-M5D7M-R1-001..004` and parent `AC-M5D7M-005/006/009`.
- Status: **R59 watchdog timeout; no result XML, no retry, and no production
  or test correction.**

## Authorized R59 execution and watchdog stop

The one authorized Unity `6000.6.0f1` R59 PlayMode diagnostic began with
task-created main PID `38620` at 02:03:40 KST. Its task-created crash-handler
PID was `37204` and Package Manager PID was `37544`. The exact R59 filter
reached `DesktopProfileLaunchAdapterV1Tests.PhaseD4_DuplicateAdapters_LoserDoesNoPreparationAndCannotCloseWinner`
at partial-log line `1593`; the associated `InputRouter.cs:231` call site is
at line `1584`.

The partial log last advanced at 02:16:38 KST. At 02:20:05 KST, it had remained
unchanged for more than 207 seconds while the main Unity CPU increased from
`843.14s` to `1007.72s`. This meets the contract's exact-D4 same-log,
180-second, increasing-CPU watchdog. At 02:20:23 KST, only the exact
task-created R59 tree (PIDs `38620`, `37204`, and `37544`) was stopped. No
unrelated process was stopped and no retry occurred.

No R59 result XML was written; consequently, no test count, fixture inventory,
or duration may be inferred. The preserved partial log is
`artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r59-two-cheap-siblings.log`,
SHA-256 `A5583325AB96178D92F7FB4F5413F3644802856E78EAD93CEA87814F596567CF`.

## Startup transport observation

During startup, the log recorded an AssetImportWorker transport error (`10054`)
and then remained unchanged after a 02:07:15 cache-trim entry. The main editor
remained live and consuming CPU. The log later recovered, advanced to the D4
test at 02:16:38, and only then met the separate D4 watchdog. This observation
is preserved as chronology; it does not establish a cause for the R59 timeout.

## Acceptance-criterion evidence

- `AC-M5D7M-R1-001`: timeout branch recorded — no indefinite process remains;
  no complete XML was produced before the authorized watchdog stop.
- `AC-M5D7M-R1-002`: PASS — only the exact task-created R59 process tree was
  stopped, prior artifacts were preserved, and no retry was issued.
- `AC-M5D7M-R1-003`: PASS — source/test hashes and workspace drift were
  recorded; the generated scene pair is preserved rather than removed.
- `AC-M5D7M-R1-004`: the two-cheap-sibling *selection arm* reached the D4
  liveness condition. This requires narrower diagnostic analysis before any
  behavioral change; it does not attribute a specific fixture as the cause.

For parent `AC-M5D7M-005/006/009`, R59 supplies a bounded timeout result and
preservation evidence only. It neither verifies the unfiltered full PlayMode
suite nor closes `AC-CUA-009`.

## Baseline preservation and drift

The four CUA execution-baseline hashes remained unchanged:

| Path | SHA-256 |
|---|---|
| `CostumeUnityMediaPackageV1.cs` | `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44` |
| `CostumeUnityPresentationAdapterV1.cs` | `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB` |
| `CostumeUnityPresentationAdapterV1Tests.cs` | `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F` |
| `CostumeUnityViewPresenterV1Tests.cs` | `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00` |

The pre-run porcelain count was `504`; post-run it was `506`. The two added
entries are the preserved run-generated pair
`Assets/InitTestScenef1491828-1804-41ce-8c66-dc49220dc8b4.unity` and `.meta`.
The final UTF-8 joined `git status --porcelain` SHA-256 was
`BADFE36671A101A2F49CDB2D2587B20BBAD2DD24765C7E74AE8E27180865684B`.

## Incidental version probe disclosure

Immediately after starting R59, a version-print attempt unintentionally created
a separate Unity PID `26464` and its crash-handler PID `37672`. It created no
test result, log output, or authorized diagnostic artifact. Those two exact
incidental processes were stopped immediately; the approved R59 process tree
remained running until its watchdog decision above. This disclosure does not
expand R59's authorized execution scope or alter its partial-result finding.

No source, test, package, ProjectSettings, scene, or prior evidence changed as
part of R59. The timeout is evidence for the selection arm only; no specific
root cause is asserted and `AC-CUA-009` remains **open**.
