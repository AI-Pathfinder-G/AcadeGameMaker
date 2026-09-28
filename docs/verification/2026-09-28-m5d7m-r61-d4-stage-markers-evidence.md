# M5D7M R61 D4 stage-marker diagnostic evidence

- Date: 2026-09-28 (Asia/Seoul)
- Contract: `docs/specs/work-contracts/2026-09-27-m5d7m-duplicate-liveness-recovery.md`, Astra R61
- Requirements / criteria: `REQ-M5D7M-R61-001/002`; `AC-M5D7M-R61-002/003`
- Executor: Terra; implementation pre-gate: Luna PASS
- Scope: one host-environment Unity `6000.6.0f1` PlayMode diagnostic only.

## Run record

The one permitted R61 Unity main process was PID `38656`, started at
`2026-09-28 02:53:57 KST`. It used the exact R60 selection filter, no
`-quit`, and no version probe. The fresh output stem was
`artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r61-d4-stage-markers`.

No result XML was produced. The preserved partial editor log is `363064`
bytes, last written at `2026-09-28 03:07:12.946 KST`, with SHA-256
`060B1753603C0E8221C4B5B4CD659DEB88BF834FBD009BBF5D26F3C7615B3E78`.

## Observed D4 sequence

The log recorded this complete marker sequence in
`PhaseD4_DuplicateAdapters_LoserDoesNoPreparationAndCannotCloseWinner`:

1. `M5D7M-R61 D4 expect:before`
2. `M5D7M-R61 D4 expect:after`
3. `M5D7M-R61 D4 context:create:before`
4. `M5D7M-R61 duplicate:set-active:before`
5. the expected `InvalidOperationException: Prepared hub reservation is not available.`
6. `M5D7M-R61 duplicate:set-active:after`
7. `M5D7M-R61 duplicate:context:return`
8. `M5D7M-R61 D4 context:create:after`
9. `M5D7M-R61 D4 winner:start:before`
10. `M5D7M-R61 D4 winner:start:after`
11. `M5D7M-R61 D4 receipt-assertions:before`
12. `M5D7M-R61 D4 receipt-assertions:after`
13. `M5D7M-R61 D4 dispose:before`
14. `M5D7M-R61 D4 dispose:after`

After `dispose:after`, the log remained unchanged for more than `180` seconds
while the main-process CPU increased from `901.12s` at `03:08:57 KST` to
`996.77s` at `03:10:33 KST`. No XML appeared. This met the R61 exact-D4
stale-log plus rising-CPU watchdog condition.

Only the verified task-created process tree was stopped: Unity main PID
`38656`, its crash handlers, Unity Package Manager, Unity Data Store, Burst
and ILPP helpers, AssetImportWorker, shader compilers, auto-quit helpers, and
their console hosts. No unrelated Unity or user process was stopped, and there
was no retry.

## Source and drift record

The temporary marker test source is
`Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/DesktopProfileLaunchAdapterV1Tests.cs`,
SHA-256 `30CC6D547F56B7C5F4596535B00DD481FBC8EB41170717BE9C7446AADAA5647E`.
Its marker-only removal check previously reproduced the required original
SHA-256 `3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3`;
the temporary markers remain in place pending Astra's restoration instruction.

The four CUA baseline hashes were unchanged:

- media: `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`
- adapter: `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`
- adapter tests: `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`
- view-presenter tests: `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`

The run-generated scene pair remains preserved:

- `Assets/InitTestScene7f40e625-1410-4205-b683-4cda70517bfa.unity`, SHA-256
  `3A8EC0A72A0E381F33637FB537ED089B555CF69BED9FD0E0CDC1FC35835C97BB`
- its `.meta`, SHA-256
  `A36866B15A033EB4A7FC61DB8776FD7E42CF5761C64DD74E2350CFC2523EE36B`

The contract's pre-amendment porcelain baseline was `511` entries; immediately
after the run it was `514` entries. Existing dirty state and the preserved
run-generated scene pair are not deleted or normalized by this record.

## Result and boundary

`AC-M5D7M-R61-002` is evidenced as a bounded run with a complete recorded D4
marker sequence and a watchdog-conformant exact-task-tree stop. `AC-M5D7M-R61-003`
is evidenced by the preserved partial log, hashes, process scope, and drift
record.

The supported inference is limited: the D4 method body completed through its
`finally` disposal in this run. The subsequent runner finalization versus
next-fixture path remains unresolved. This diagnostic does not identify a
defective fixture or runtime component, does not authorize a behavioral fix,
and does not close `AC-CUA-009`.

## Luna restoration audit — AC-M5D7M-R61-004

After the diagnostic, Luna independently verified that
`Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/DesktopProfileLaunchAdapterV1Tests.cs`
is restored to the exact original SHA-256
`3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3` and
contains zero `M5D7M-R61` markers. The four CUA baseline hashes remain
unchanged, the R61 partial log and preserved scene pair remain present, and
no Unity/CrashHandler/PackageManager process remains. Existing workspace
dirty state was preserved; no source, test, artifact, or scene was deleted or
normalized by this audit.

**AC-M5D7M-R61-004: PASS.** Temporary marker restoration is independently
verified. This closes only the R61 cleanup criterion; it does not close
`AC-CUA-009` or convert the partial diagnostic into full-suite acceptance.
