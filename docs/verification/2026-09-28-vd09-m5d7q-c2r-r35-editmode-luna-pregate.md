# C2/C2R R35 complete EditMode partition — Luna independent pre-gate

Date: 2026-09-28  
Scope: read-only review and deterministic name/selector replay of Terra's R35
plan. No Unity, CIM, source, test, filter, timeout, asmdef, or licensing
operation was performed.

## Independent set reconstruction

Using the preserved R23 and R32 XMLs, I reconstructed the plan's exact sets:

- R23: `597` total, `595` passed; removing the three passed old Q0 rows leaves
  `592` retained names. The old aggregate row is itself a failed historical
  row and is not part of the retained-passed set.
- R32: `21` current replacement names, all passed in that historical XML.
- Retained/R32 overlap: `0`; union expected count: `613`.
- Whole-fixture cohorts: `29`.
- Controller cases: `35` (`18` retained plus `17` current).
- Current Q0 cases: `4`.
- `ProfileResetDiskProcessV1Tests` internal worker cases: `51`.

The exact positive leaf selector from the plan selects `613/613` reconstructed
names. Its terminal fixture dots prevent namespace or fixture-node ancestor
matches. The controller branch excludes the old singleton aggregate by exact
name while admitting all 17 generated current controller rows; the Q0 branch
admits the four R32 current Q0 names.

The three namespace/cohort boundaries are therefore selector-safe under the
installed Unity Test Framework ancestor/descendant `Pass` behavior, subject to
the mandatory post-discovery full-name equality check.

## Name-version ambiguity and source gate

Three of the four current Q0 full names are textually reused from old R23 Q0
rows; one Q0 method name is changed. This is not a selector-count defect,
because the old rows are removed before the R32 union, but full names alone
cannot prove that a reused name came from the current source. The R35 source
manifest is therefore a required closure condition, not optional evidence.

Before execution, Main must capture the complete current dependency inventory
covering all selected test/runtime/shared-helper `.cs`, `.cs.meta`, asmdef and
appropriate configuration/current fixture assets (the planned 570-file
code/meta/asmdef inventory plus the stated config/assets). It must record
byte-for-byte before/after equality after R35. The four runtime anchors and two
current replacement-test hashes in Terra's plan independently match the local
files. The historical 202-file serialized digest remains unreconstructed and
must not be claimed equivalent to the fresh manifest.

The 613 names are discovery expectations only; old 592 passing rows are not
reused as final evidence. Every selected current case must execute freshly.

## Verdict and execution recommendation

**Luna pre-gate: PASS, P0=0, P1=0, conditional on the fresh-manifest and
post-XML name-set gates above**, for `AC-M5D7QC2-010` and
`AC-M5D7QC2R-007/008` partition planning.

After Astra approval and after R34's Editor is gone, one unchanged EditMode
invocation is safe to run. Do not alter NUnit timeout/Skip behavior or invoke
the internal process-worker cases manually; they are ordinary EditMode rows.
Before interpreting the result, require exactly 613 qualified names, no
missing/extra/reused-stale rows, zero failed/skipped/inconclusive cases, and
matching fresh source manifests. This pre-gate does not accept R35 or close
C2/C2R; Astra remains the integration authority.
