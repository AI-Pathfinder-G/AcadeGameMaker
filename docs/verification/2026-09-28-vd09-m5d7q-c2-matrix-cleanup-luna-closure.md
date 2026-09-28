# M5D7Q-C2 fault-matrix cleanup Luna closure

Date: 2026-09-28  
Reviewer: Luna (`gpt-5.6-luna`)  
Scope: independent read-only source review; no Unity execution, source edits,
or acceptance.

## Fixture before/after

`ProfileResetMemoryCutoverFaultMatrixV1Tests.cs` changed from
`16ACFB2885BB6AF2AF0823725852AE49E863856095EFD010B2943B59F91756D3` to
`F1EAA0EB7E7E955A1C57C2C230A55FAED3D6FD3AB18169E68333AFA08D0562D9`.
The `.meta` remains `304E91D4CC8CBA9C49D4366775E2C269C263274474D422BF65889972DB70EDCB`.

The frozen fixture now has 73 cases: 66 parameterized and 7 direct. Its
ordinary cleanup recursively validates every descendant and ancestor, rejects
reparse links before recursive deletion, and includes a direct nested-junction
refusal test with target-sentinel preservation. Native links are removed
nonrecursively before owned-base cleanup.

## Finding disposition

The prior fixture P1—descendant-unsafe recursive cleanup—is closed by source
review. The corrected validator checks path confinement and attributes before
`Directory.Delete`; the nested-junction test verifies refusal rather than
following the link. This closes the fixture safety risk, not a runtime C2
finding, and has not been executed by this review.

No new P0/P1 was found. The fixture uses fresh real cohorts and roots, exact
production operation delegates, real C1 `Begin`/proof flow, and no copied reset
algorithm or fake proof. Native junction rows retain independent tag/sentinel
checks; ordinary `File.Exists` expectations are confined to ordinary roots.

The earlier R10 digest remains factually bounded: its existing two fixtures ran
41/41 with zero failures/skips/inconclusive; the new 73-case matrix was not in
that XML. The C2R wording remains limited to readonly Resume result facts plus
the exact fresh PreparedProof reference and private opaque execution witness,
not boxed result-object identity.

Astra corrected the omitted final hash character against a fresh SHA-256
read of the unchanged fixture. This is a transcription correction, not source
execution or a new acceptance claim.
