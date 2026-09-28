# M5D7M R57 HubPresentation interaction — Luna procedural pre-gate

- Date: 2026-09-28 (Asia/Seoul)
- Scope: independent diagnostic pre-gate only; no Unity execution
- R57 contract SHA-256: `CDA9031B6C014D83D449178AD3DBBC41E6BFB200D48F5FEA83E7955D9769A434`
- Parent CUA contract remains Approved; `AC-CUA-009` remains open.

## R14 post-review checks

R14 is independently consistent with the recorded liveness disposition. Its
immutable full-PlayMode log SHA-256 is
`EB686AA217A17231DF882954B4A3451587A2F4B6A2880E99CEA415C0E1774105`; no XML
exists. The log reached
`PhaseD4_DuplicateAdapters_LoserDoesNoPreparationAndCannotCloseWinner` and
`InputRouter.cs:231`, then remained stale while CPU increased. R14 therefore
met its 180-second same-test liveness stop condition and is invalid evidence,
not a CUA product defect or acceptance. The R14 liveness record SHA is
`70E869F5A8150FC38BF6BFB23C87E60D3086BDE9239643ECBA0980FD01B36CD6`.

R54 (`1/1`), R55 (`117/117`), and R56 (`558/558`) remain successful isolated or
preceding-group evidence. The implementation evidence and README hashes are
unchanged at the contract-recorded values
`28C208EC58C3DAE90C5B8B87801A5C54607191C1FBBD15CDFFD6970578C3ECD8` and
`A162E86103E0B75A0103050B1B5206AAD43DC9BDCD86B702ED590DAE98C79AD5`.

## R57 pre-gate checks

1. R57 is Approved for one diagnostic run only. Its semicolon-separated filter
   adds exactly the `HubPresentation` group to the previously exercised
   Camera/Combat/Input/DesktopProfile selection; it does not claim fixture
   order or identify a production owner.
2. Fresh outputs
   `artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r57-hub-interaction.xml`
   and `.log` are absent. No Unity, crash-handler, or package-manager process
   is present at the gate.
3. The four CUA source/test hashes remain unchanged: media
   `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`,
   adapter
   `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`,
   adapter tests
   `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`, and
   presenter tests
   `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`.
4. The pre-R57 porcelain baseline is `499` entries with UTF-8 joined
   `git status --porcelain` SHA-256
   `1B5A62B0874465B786FAF32F7FCDBD45D264C0E2D990CFCF80106AFFED45ACC4`.
5. The run is one Unity `6000.6.0f1` PlayMode diagnostic, without `-quit`,
   with a `1500` second external watchdog. At the D4 duplicate test, 180
   seconds of unchanged log plus increasing CPU permits stopping only the
   exact task-created process tree. Missing XML after natural exit, auth or
   compile failure, nonzero counts, source drift, or workspace mutation is a
   hard stop. No retry is authorized.

## Verdict

**PASS — P0=0, P1=0, P2=0.** R57 is procedurally ready for the one approved
diagnostic execution. Its outcome can only support or reject the
`HubPresentation` group-interaction hypothesis; it authorizes no source/test
fix and cannot close `AC-CUA-009` or change the CUA contract's Approved status.
