# CUA R12 full PlayMode interruption

- Date: 2026-09-28
- Scope: `AC-CUA-009` execution evidence only
- Independent reviewer: Luna
- Astra disposition: invalid run; no CUA acceptance or source correction

R12 used the approved Unity `6000.6.0f1` full PlayMode command once, with no
test filter, assembly restriction, or `-quit`. PID `42436` started at
2026-09-27 23:56:23 KST. It exited before producing the required XML. Its
preserved log is
`artifacts/unity-results/costume-cua-20260927/costume-cua-r12-full-playmode.log`,
SHA-256 `666A74FA760407553FB1186022FEDB77258EBC4CDDE06E9C6995C5323E2EA906`.
The log ends at 23:59:56 KST after a Hub scene load with
`AssetImportWorkerHW0` transport error `10054 (possible crash)` and
`IPCStream (Upm-42436): ... Not connected`. Neither the editor log, import
worker log nor available event records identifies the faulting module or proves
the cause. There is no normal test-run completion record.

The last visible tests are Camera/Combat, including intentionally induced
exception and invalid-collider diagnostics. No CUA test failure appears in the
tail. CUA's R11 focused PlayMode `195/195` and full EditMode `800/800` remain
valid for the unchanged source hashes, but R12 supplies **no** full PlayMode
PASS. Luna independently classified the direct relation to CUA as low on
available evidence; this is not a finding that CUA is defect-free.

The R12 run created
`Assets/InitTestScene4c3f7ede-c74c-45a8-8c34-93f19204e4bd.unity` and
its `.meta`. They and the R12 log are preserved as evidence. No CUA source or
test file changed. `AC-CUA-009` stays open and the CUA contract remains
`Approved`, not `Verified`.
