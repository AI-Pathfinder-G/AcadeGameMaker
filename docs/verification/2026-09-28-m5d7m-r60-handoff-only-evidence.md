# M5D7M R60 handoff-only separation — liveness evidence

- Date: 2026-09-28
- Contract: `docs/specs/work-contracts/2026-09-27-m5d7m-duplicate-liveness-recovery.md`
- Scope: Approved diagnostic child `REQ-M5D7M-005/006/008`; evidence is
  limited to `AC-M5D7M-R1-001..004` and parent `AC-M5D7M-005/006/009`.
- Status: **R60 watchdog timeout; no result XML and no retry.**

## R60 result

The one authorized Unity `6000.6.0f1` R60 PlayMode diagnostic used the R58
selection plus only `HubUiOnlyQ0HandoffMatrixPlayModeTests`. Main Unity PID
`15176` started at 02:24:40 KST. The task-created tree also contained worker
PID `7816`, crash handlers `35552` and `37800`, and Package Manager PID `7792`.

The log reached
`DesktopProfileLaunchAdapterV1Tests.PhaseD4_DuplicateAdapters_LoserDoesNoPreparationAndCannotCloseWinner`
at line `1590`; `InputRouter.cs:231` is recorded at line `1581`. Its last
write was 02:37:35 KST. At the decision point, the log had been unchanged for
about 201 seconds while main CPU rose from `811.14s` to `994.42s`. This met the
exact-D4, 180-second, increasing-CPU watchdog. At 02:41:12 KST, only the exact
R60 task-created process tree was stopped. No unrelated process was stopped,
no retry was issued, and the 1,900-second global watchdog did not expire.

No result XML exists. Therefore, expected `990` tests, fixture inventory,
duration, and any pass/fail result cannot be inferred. The preserved partial
log is
`artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r60-handoff-only.log`,
SHA-256 `C8EDCDBC87675B56FD7B5EFCF3DFDBB6290AC39E044A28A26503A52824EC5DB6`.

## Acceptance-criterion evidence

- `AC-M5D7M-R1-001`: timeout branch recorded; no complete XML was produced,
  and no diagnostic process remains.
- `AC-M5D7M-R1-002`: PASS — only the exact R60 task-created tree was stopped;
  prior artifacts were preserved and no retry occurred.
- `AC-M5D7M-R1-003`: PASS — source/test hashes and drift were recorded; the
  new run-generated scene pair is preserved.
- `AC-M5D7M-R1-004`: the handoff-matrix-only selection arm was sufficient to
  reach the D4 liveness condition in this environment. This does not prove the
  selected fixture or production code is defective and authorizes no fix.

For parent `AC-M5D7M-005/006/009`, R60 provides bounded timeout and
preservation evidence only. It neither verifies unfiltered full PlayMode nor
closes `AC-CUA-009`, which remains **open**.

## Baseline and drift preservation

The CUA execution baseline remained unchanged:

| Path | SHA-256 |
|---|---|
| `CostumeUnityMediaPackageV1.cs` | `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44` |
| `CostumeUnityPresentationAdapterV1.cs` | `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB` |
| `CostumeUnityPresentationAdapterV1Tests.cs` | `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F` |
| `CostumeUnityViewPresenterV1Tests.cs` | `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00` |

Pre-run porcelain had `508` entries; post-run it had `510`. The two additions
are the preserved R60-generated
`Assets/InitTestScene9851fddc-178b-4b17-b7b4-aca78a0b4bbc.unity` and `.meta`.
The final UTF-8 joined `git status --porcelain` SHA-256 was
`51AF50B1B42E47A49717C9E4E2AFB276C291792CBA865BA8A5E452D73EB59E17`.

No source, test, package, ProjectSettings, scene, or prior evidence was
changed by the R60 execution. The timeout supports this selection arm only;
it makes no specific root-cause claim.
