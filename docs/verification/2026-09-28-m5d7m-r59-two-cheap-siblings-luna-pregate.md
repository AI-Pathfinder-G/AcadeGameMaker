# M5D7M R59 two-cheap-siblings — Luna procedural pre-gate

- Date: 2026-09-28 (Asia/Seoul)
- Scope: independent diagnostic pre-gate only; no Unity execution
- Approved contract SHA-256: `30C3FA508285C2EB72ADCBAF1E9713A7723685C6945B0E1C94EA84249DA10CA3`

## Checks

1. R58 evidence is preserved and independently recorded as `985/985`, with
   failed/skipped/inconclusive `0/0/0`, D4 natural completion, and no CUA source
   change. R58 XML/log/evidence hashes are respectively
   `5E9047F3AB928BB2EEF1B309F24AE9CC4D0D33CDC5FB77D3022DB8205D2FC889`,
   `B2B12FD6C41F303196FE4663586783767C5777813419DD20B0D1D4068F4EE81E`, and
   `806E9D0F99DDDDF2866F92487A25ECF52B0C389707D7595189FE224259E3429E`.
2. R59 is Approved for exactly one host-environment Unity `6000.6.0f1`
   PlayMode diagnostic without `-quit`, using the exact R58 filter plus only
   `HubUiOnlyQ0HandoffMatrixPlayModeTests` (5 historical tests) and
   `HubUiOnlyQ0RemainingRuntimePlayModeTests` (18 historical tests). Expected
   selection is `1008` tests (`985+5+18`), subject to XML inventory check.
3. Fresh outputs
   `artifacts/unity-results/m5d7m-liveness-20260927/m5d7m-r59-two-cheap-siblings.xml`
   and `.log` are absent. No Unity, crash-handler, or package-manager process is
   present at the gate.
4. The four CUA source/test hashes remain unchanged: media
   `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`,
   adapter
   `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`,
   adapter tests
   `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`, and
   presenter tests
   `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`.
5. The pre-amendment porcelain baseline is `503` entries with UTF-8 joined
   `git status --porcelain` SHA-256
   `E87E864BE80DC8A2297C164BAA390CC57AE14B6123884052F44C6378522A5CDC`.
   Existing dirty state is preserved and is not treated as R59 output.
6. The external global watchdog is `2200` seconds. The exact-D4 unchanged-log
   plus increasing-CPU watchdog remains `180` seconds. A trigger permits
   stopping only the exact task-created Unity process tree; no retry is allowed.
   Compile/authentication failure, missing XML after natural exit, nonzero
   failed/skipped/inconclusive counts, source drift, or unexpected workspace
   mutation is a hard stop. Partial logs and generated scenes must be retained.

## Verdict

**PASS — P0=0, P1=0, P2=0.** R59 is procedurally ready for the one approved
two-fixture selection diagnostic. A D4 timeout implicates only this selection
arm; a pass only rejects this arm as sufficient to reproduce the failure. No
outcome closes `AC-CUA-009`, replaces the required unfiltered full PlayMode
XML, or authorizes a production/test fix.
