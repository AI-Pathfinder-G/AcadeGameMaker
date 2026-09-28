# M5D7M R58 cheap-suffix diagnostic — Luna procedural pre-gate

- Date: 2026-09-28 (Asia/Seoul)
- Scope: independent diagnostic pre-gate only; no Unity execution
- Approved contract SHA-256: `43EC9C48F9DBCAD4BF408872CF112468A1A45FAFD8CF8A2043A36DD5C6CA9D0F`

## R57 post-review confirmation

R57's immutable diagnostic evidence is internally consistent:

- XML: `562/562`, failed/skipped/inconclusive `0/0/0`
- XML SHA-256: `440F7D7A36C10F78D9FFA5D8878DF7E016361FD01B2C733ED9564AE89AD27A8B`
- Log SHA-256: `6B9C025ED81628C429BA90345F553CBBBE8A3B3D94C1BBAB02E165FE2911D8A9`
- Evidence SHA-256: `DA97F6750461F2F0BAB66FC7BD7686EAE838BDF1EFBEF217D6D6E4A7CBCFFB5A`

The first 21 fixture suites match the prior successful full-PlayMode prefix,
including the D4 target. R57 therefore rejects the added HubPresentation-only
hypothesis. Its diagnostic acceptance criteria `AC-M5D7M-R1-001..004` are
independently satisfied, but parent `AC-CUA-009` remains open.

## R58 checks

1. R58 is approved for exactly one host-environment Unity `6000.6.0f1`
   PlayMode diagnostic with no `-quit`, using the contract's semicolon-separated
   filter. It includes the R57 prefix, low-cost suffix fixture indices 26–44
   (`228` tests), and the CUA `PresentationUnity` suite (`195` tests), for an
   expected `985` tests. The five costly sibling fixtures remain excluded.
2. Fresh outputs
   `artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r58-cheap-suffix.xml`
   and `.log` are absent. No Unity, crash-handler, or package-manager process is
   present at the gate.
3. The four CUA execution-baseline hashes remain unchanged: media
   `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`,
   adapter
   `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`,
   adapter tests
   `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`, and
   presenter tests
   `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`.
4. The pre-R58 porcelain baseline is `501` entries with UTF-8 joined
   `git status --porcelain` SHA-256
   `BB400A740F2B5FFAF00FFA242A837F4F3E2BA71FF4D4E711968D7D3B4F3C1269`.
5. The external monotonic watchdog is `1500` seconds. At D4, unchanged log for
   `180` seconds plus increasing CPU permits stopping only the exact
   task-created Unity process tree. Compile/authentication failure, missing XML
   after natural exit, nonzero counts, source drift, or unexpected workspace
   mutation is a hard stop. No retry is authorized; partial logs and generated
   scene pairs must be preserved.

## Verdict

**PASS — P0=0, P1=0, P2=0.** R58 is procedurally ready for the one approved
cheap-suffix diagnostic. A pass or timeout is diagnostic only: it cannot close
`AC-CUA-009`, replace the required unfiltered full PlayMode XML, or authorize a
production/test fix.
