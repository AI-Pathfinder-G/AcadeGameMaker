# SPARK-01-R2 recovery handoff

- Date: 2026-09-10
- Governing contract: `docs/specs/work-contracts/2026-09-09-spark01-r2-final-fixes.md`
- Status: **Implementation complete / independent review pending.** This report does not mark the work Verified or approve any production asset.
- PowerShell: 7.6.5
- Modified files: `qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1`, this handoff only.

## Scope and integrity

- The production tool `qa/tools/sprite-preflight/Test-SpriteSheetPreflight.ps1` was not edited. Its SHA-256 before the recovery (from the R2 independent recheck) and after all runs is unchanged: `0DE445E1AA5662913692340CA4C4254EDC91F7ADF169FB4691D341A91498109A`.
- SelfTest SHA-256 before recovery: `A00EA5E2742E984829A178775E61A120B2A513D6C2638C2A208AE1570F13F537`.
- SelfTest SHA-256 after recovery: `C71B34B7C898E8953921222EB083350473886DA540DD40014F4197F25E400A61`.
- No prior independent review/recheck, product tool, README, contract, review script, Unity, asset, or unrelated dirty worktree file was edited.

## AC mapping and implementation

| Acceptance criterion | Recovery evidence |
| --- | --- |
| AC-SPRPF-R2-002 | Normal SelfTest creates and exercises direct and ancestor junction paths. Creation is isolated in a dedicated `try/catch`; unavailable creation is recorded `SKIPPED`. Later copy/tool/assert exceptions are `FAIL`. Final status derives only from `FAIL`/`SKIPPED` counters. |
| AC-SPRPF-R2-002 | `-ReproLinkCreationFailure` places a real ordinary file at the requested junction path, then attempts `New-Item -ItemType Junction`. The genuine creation error is counted as `SKIPPED=1`; no mode-based exit override exists. |
| AC-SPRPF-R2-002 | `-ReproLinkAssertionFailure` creates a real junction, invokes the product tool, then deliberately supplies the wrong expected exit code to `Assert-Result`. Its actual assertion mismatch is counted as `FAIL=1` with the result report path and detail. |
| AC-SPRPF-007 / R1-005 | `-InjectAssertionFailure` retains a real `Assert-Result` mismatch (`INVALID_MANIFEST` expected from a passing result), records `FAIL=1`, and prints its temporary evidence directory and complete counters before exiting nonzero. Unexpected exceptions use the same summary path without terminating `Write-Error`. |
| AC-SPRPF-R2-001 / R2-003 | Existing exact-number and bounded-manifest-read cases remain exercised by normal SelfTest and the unchanged production tool hash. |
| AC-SPRPF-001 through AC-SPRPF-006; R1-001 through R1-004 | Existing SelfTest coverage remains in the normal 33-case run; unchanged independent reproducers cover the documented 8 general and 2 numeric cases below. |

## Commands and actual results

All commands were run from the repository root with PowerShell 7.6.5.

| Command | Exit | Actual counts / outcome | Evidence |
| --- | ---: | --- | --- |
| `pwsh -NoProfile -File qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1` | 0 | `PASS=33 FAIL=0 SKIPPED=0`; direct and ancestor junction rejections both passed with tool exit 2. | `C:\Users\me\AppData\Local\Temp\sprite-preflight-selftest-fbf041a33aba4509bae181c78772d665` |
| `pwsh -NoProfile -File qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1 -InjectAssertionFailure` | 1 | `PASS=33 FAIL=1 SKIPPED=0`; actual assertion detail: `Missing expected error codes: INVALID_MANIFEST`. | `C:\Users\me\AppData\Local\Temp\sprite-preflight-selftest-b0d897daabef4fb59e15a125f22499e9` |
| `pwsh -NoProfile -File qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1 -ReproLinkCreationFailure` | 1 | `PASS=33 FAIL=0 SKIPPED=1`; actual link-creation-unavailable detail records that the requested junction path is an ordinary file. | `C:\Users\me\AppData\Local\Temp\sprite-preflight-selftest-210c5a90f8f34af3aa05842c63ad4d5c` |
| `pwsh -NoProfile -File qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1 -ReproLinkAssertionFailure` | 1 | `PASS=33 FAIL=1 SKIPPED=0`; actual assertion detail: `ExitCode mismatch. expected=0 actual=2`. | `C:\Users\me\AppData\Local\Temp\sprite-preflight-selftest-e37c90ad31ba474e8a17d49ade539b16` |
| `pwsh -NoProfile -File qa/reviews/2026-09-09-spark01-repro.ps1` | 0 | All 8/8 expected codes matched; every input remained unchanged. | `C:\Users\me\AppData\Local\Temp\spark01-independent-cfa1c769d8a0415dade9b288aff77096` |
| `pwsh -NoProfile -File qa/reviews/2026-09-09-spark01-r1-numeric-repro.ps1` | 0 | Both 2/2 exact-number counterexamples returned expected exit 1. | `C:\Users\me\AppData\Local\Temp\spark01-r1-numeric-08fd74d946b84231a3adbc02757d6524` |

## Remaining limits and review request

The tool remains limited to the approved HeaderAndGrid scope; PNG decode/CRC/pixel quality, license, Unity import, gameplay, and real art assets were not verified. The temporary fixture and evidence directories are intentionally retained. An independent reviewer should inspect the counter-only exit behavior and rerun the listed commands before acceptance.
