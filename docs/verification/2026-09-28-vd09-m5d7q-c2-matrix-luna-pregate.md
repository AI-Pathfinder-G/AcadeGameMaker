# M5D7Q-C2 fault-matrix Luna pre-gate

Date: 2026-09-28  
Reviewer: Luna (`gpt-5.6-luna`)  
Scope: read-only review of the frozen PlayMode fault-matrix fixture. No Unity
call, source edit, or self-acceptance was performed.

## Frozen fixture identity

- `ProfileResetMemoryCutoverFaultMatrixV1Tests.cs`:
  `16ACFB2885BB6AF2AF0823725852AE49E863856095EFD010B2943B59F91756D3`
- `.meta`:
  `304E91D4CC8CBA9C49D4366775E2C269C263274474D422BF65889972DB70EDCB`

The fixture enumerates 19 cutover checkpoints, 22 exact-owned operations,
8 Profile-authority checkpoints, 7 post-delete disk mutations, 3 pre-reauth
marker mutations, 3 authorized BeforeDelete marker mutations, 2 native junction
rows, 4 final-pair metadata rows and 4 other direct ownership/lock cases (73
total: 66 parameterized + 7 direct). It creates a new real Unity Hub cohort and a new OS-temp
C1 root per row; foreign rows use a distinct real cohort, not copied wrappers.
The controls only throw at named checkpoints or invoke the exact object passed
by production. The fixture does not mint fake proofs or implement a copied
reset algorithm. Reflection is limited to adversarial diagnostic corruption.

API references reviewed are present in the current assemblies: the checkpoint
and operation enums, `ConfigureHubUiOnlyForAuthoring()`, `Begin`,
`FinalizeReset`, real `GameInputActions`, and Profile/Input.Unity namespaces.
The native junction helper uses only the approved Windows BCL calls and keeps
target/sentinel state separate from the link.

## Prior P1 closure — fixture recursive cleanup

The prior review identified that ordinary cleanup validated only the root before
recursive deletion. The frozen correction now recursively validates every
descendant and ancestor, rejects any reparse link before `Directory.Delete`,
and has a direct nested-junction refusal test that checks the target sentinel.
The native owner removes links nonrecursively before deleting its owned base.
The prior fixture P1 is closed by source review; this is not runtime execution.

## Other review findings

No P0/P1 runtime or contract finding was raised from this fixture review.
Presence expectations use `File.Exists` only on ordinary-root rows; native
junction rows additionally assert the reparse tag and target sentinel, so they
do not rely on false absence/presence. Constructor-failure cleanup uses the same
descendant-safe validator. No evidence here independently accepts the WIP C2R
runtime files.

## C2R clarification

The now-Approved C2R contract is
`DA1BD70DBBD42DC6817847B63411DEACF18FEC448FEC547A441E91B4C6E66C2D`.
Its clarification is narrow and sound: `ProfileResetDiskResultV1` is a
readonly value result, so boxed object identity cannot prove same-call origin.
The private opaque execution witness, validated readonly result facts and
exact fresh PreparedProof reference from the adapter's real `Resume` invocation
supply that origin without a
caller boolean, copied scalar, or alternate reset algorithm. This preserves
`REQ-M5D7QC2R-002` / `AC-M5D7QC2R-002` and does not widen product authority.

R10's supplied `41/41` all-zero execution digest is recorded separately. C2R
remains a distinct implementation and two-process verification gate.

Astra corrected the row inventory and readonly-result wording above after
reading the frozen fixture and existing C1 value type. The cleanup correction
is source-reviewed here; nothing has executed.
