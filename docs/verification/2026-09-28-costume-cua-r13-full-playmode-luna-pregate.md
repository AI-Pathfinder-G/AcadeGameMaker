# Costume CUA R13 full PlayMode — Luna procedural pre-gate

- Date: 2026-09-28 (Asia/Seoul)
- Scope: independent procedural pre-gate only; no Unity execution
- Contract SHA-256: `5004243136D1E68C8CD1B980DECA83634691347C2DF03EBEC91F44892316DD3C`
- R12 interruption evidence SHA-256: `C908FAD6F9A57A00ACEABEB68BCB78331C82983012509B0DB8FDA5CCB0D4714B`

## Checks

1. The contract's final R13 authorization is internally consistent: exactly one
   Unity `6000.6.0f1` full PlayMode invocation, `-runTests -testPlatform
   PlayMode`, no filter, no assembly restriction, and no `-quit`. No other Unity
   run or retry is authorized by that section.
2. Required fresh stems are absent before execution:
   `costume-cua-r13-full-playmode.xml` and
   `costume-cua-r13-full-playmode.log` under
   `artifacts/unity-results/costume-cua-20260927/`.
3. No Unity, UnityCrashHandler64, or UnityPackageManager process is present at
   the gate.
4. The four required CUA source/test hashes match the execution baseline:
   - media `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`
   - adapter `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`
   - adapter tests `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`
   - presenter tests `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`
5. R12 evidence remains preserved: its log SHA-256 is
   `666A74FA760407553FB1186022FEDB77258EBC4CDDE06E9C6995C5323E2EA906`; the
   R12 and R11 generated test-scene pairs remain present. No CUA source/test
   file was changed for this gate.
6. The pre-R13 porcelain baseline is `493` entries with UTF-8 joined
   `git status --porcelain` SHA-256
   `FFC171B3E06CB54C852681A798F327E50946BD68270FF9DAFCFE80A20ACF0462`.
7. The `6600` second global watchdog and the conditional `180` second
   M5D7M duplicate-adapter stale-log/CPU-increase watchdog are explicit and
   preserve the exact task-created process tree and partial evidence on stop.

## Verdict

**PASS — P0=0, P1=0, P2=0.** R13 is procedurally ready for the one authorized
full PlayMode execution. This is a procedural gate, not a test-result or
production-correctness PASS. A missing XML, compile failure, failed/skipped/
inconclusive test, source/hash drift, or unexpected workspace mutation remains
a hard stop under the contract.
