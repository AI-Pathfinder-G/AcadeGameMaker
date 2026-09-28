# Costume CUA R15 uninstrumented full PlayMode evidence

- Date: 2026-09-28 (Asia/Seoul)
- Traceability: `REQ-CUA-R15-001/002`, `AC-CUA-R15-001/002/003`; `AC-CUA-009`
- Scope: one uninstrumented, no-filter full PlayMode run on Unity 6000.6.0f1.

## Natural result

Unity main PID `39776` exited naturally with exit code `0`; no process was
stopped and no retry occurred. The XML reports `1142/1142` passed, `0` failed,
skipped, and inconclusive, duration `4890.2643413s`.

- XML: `artifacts/unity-results/costume-cua-20260927/costume-cua-r15-full-playmode.xml`
  — SHA-256 `4EFEACC7447BD272CB361E41F91CEE18B120FFB5973AC4BE501DC4EE24A8F196`
- log: `artifacts/unity-results/costume-cua-20260927/costume-cua-r15-full-playmode.log`
  — SHA-256 `7B0527F0C934437D50C997C59B5691C670CA03770D75D17C6491EAB0DAFA05B0`

The XML contains 47 `TestFixture` entries: Camera (1), Combat (16),
HubPresentation (3), Input (1), Input.Unity adapter/handoff/Hub UI (6),
InputUnity router/profile (9), Movement (3), PresentationUnity (2), and
TransferUnity (6). This is the complete selected fixture inventory in the R15
XML and totals the 1,142 executed test cases.

## Baseline and drift

The four CUA hashes remained at their approved values: media
`A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`, adapter
`52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`, adapter
tests `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`, and
view-presenter tests `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`.
The restored D4 source hash is
`3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3`.

Post-run porcelain count was `518`. No new R15 `InitTestScene` pair was found;
all previously generated scene pairs remain preserved.

R11 focused PlayMode `195/195` and R11 full EditMode `800/800` remain the
earlier reference passes. R15 is the new uninstrumented full PlayMode result.

## Status

The observed R15 execution result passed Luna's independent
[postreview](2026-09-28-costume-cua-r15-full-playmode-luna-postreview.md)
(SHA-256 `287F3A0E76C4C1D2613C507E9B65B0F85BB18E4E6C75FE665FBB569C66133BEA`).
Astra's final [implementation-evidence addendum](2026-09-20-costume-cua-implementation-evidence.md)
accepts **`AC-CUA-009: PASS`** with the preserved R11 focused PlayMode and
full EditMode results. This does not normalize existing workspace drift or
accept real media/catalog promotion or live-scene visual quality.
