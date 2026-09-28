# Costume CUA R15 full PlayMode Luna postreview

- Date: 2026-09-28 (Asia/Seoul)
- Reviewer: Luna (independent QA)
- Traceability: `AC-CUA-R15-001/002/003`, `AC-CUA-009`
- Scope: read-only postreview of one uninstrumented, unfiltered full PlayMode run.

## Direct result verification

The preserved R15 XML reports `1142/1142` passed, `0` failed, skipped, or
inconclusive, with duration `4890.2643413s` and natural Unity exit code `0`.
The direct artifact hashes are:

- XML `artifacts/unity-results/costume-cua-20260927/costume-cua-r15-full-playmode.xml`:
  `4EFEACC7447BD272CB361E41F91CEE18B120FFB5973AC4BE501DC4EE24A8F196`
- log `artifacts/unity-results/costume-cua-20260927/costume-cua-r15-full-playmode.log`:
  `7B0527F0C934437D50C997C59B5691C670CA03770D75D17C6491EAB0DAFA05B0`

The XML contains 47 fixture entries. The CUA `PresentationUnity` assembly and
suite each report `195/195` passed. The log records result saving and
`Test run completed. Exiting with code 0 (Ok). Run completed.` No process was
stopped and no retry occurred.

## Baseline and retained references

The restored D4 source and all four CUA execution baselines remain unchanged:

- D4 source: `3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3`
- media: `A4F52C4A3D468C2AAD57E07C27196CF31CF36AABEE977C9310E293F554609F44`
- adapter: `52B4B36F66FDF1DBAC49BB056E204F5918BAFD7BB2B6E7E0A0E6A6569CFD85EB`
- adapter tests: `16FC44301D96D035FCBF30D57AA25ED9EA9E7755E9FFBA4B58F654A31245EE4F`
- view-presenter tests: `6BFE90A9FEBC201750B186C84DEF0B2B4593C013F022EB621545E49D5E681D00`

The R15 evidence records post-run porcelain count `518`, with existing dirty
state preserved and no new R15 scene pair. Earlier immutable reference passes
remain intact:

- R11 focused PlayMode `195/195`: XML
  `0485ADB16440963B553A6C4D48191D47F1C4C736624B3402D6D9632414740F19`, log
  `36B510E577231DC3E8E757063BB3C6FC96291F090F7334B66FB271990FE4E91C`
- R11 full EditMode `800/800`: XML
  `D4E18A69084A2E0140E19D503C734AC7DD68C30AE1FF723D5BD13CC7E3A38C7C`, log
  `86251F0306CC575C86F617E9313CE0740FDB3507290863F4014E0EB990FD2DF0`

## Independent boundary

`AC-CUA-R15-001/002/003` are supported by the natural-exit run, complete XML,
zero non-passing states, inventory, hashes, and preserved baseline/drift
records. Therefore the evidence is **eligible for Astra's final
`AC-CUA-009` acceptance review**. Luna does not mark the parent acceptance
closed. This verification makes no claim about real media, visual quality, or
product behavior beyond the recorded test evidence.
