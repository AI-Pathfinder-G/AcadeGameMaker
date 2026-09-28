# C4 forward Q0 maintenance boundary — Luna

Date: 2026-09-28  
Scope: bounded forward review of the C4 Review allowlist against Q0 audit
authority. No contract-status change, implementation, Q0 test/source edit,
Unity execution, or runtime acceptance.

Reviewed amended C4 draft SHA-256:
`9D3284A144D2073C9A55D5CB7D7A11519AC9461782C77B542297F8484DBF03F7`.

## Finding

The C4 allowlist permits narrow edits to both
`DesktopProfileLaunchAdapterV1.cs` and `InputRouter.cs`. The current Q0 audit
strictly pins the approved C2/C2R current hashes, while preserving historical
rows. Therefore a C4 implementation that changes either file would make the
current Q0 hash assertion stale. The existing C3 forward review already
defines this maintenance pattern for a later Adapter successor; C4 needs the
same explicit boundary before any Q0 allowlist edit.

This is a test-evidence maintenance issue, not a C4 runtime or criteria
authority change. It does not justify a compatibility fallback or weakening
the Q0 assertion.

## Exact recommended boundary

Add a separate Astra-approved, test-only Q0 amendment before Terra edits the
Q0 allowlist, and only if the independently reviewed C4 implementation
actually changes a pinned file:

1. Preserve all nine historical evidence rows, the seven strict legacy-current
   rows, the two existing strict C2/C2R successor rows, and any independently
   accepted C3 successor evidence unchanged.
2. Append one exact C4 successor row for each changed file—Adapter and/or
   Router—pinning the complete current-file SHA-256, C4 source/contract
   provenance, and Luna's independent post-implementation review.
3. If a file is unchanged by C4, retain its most recent independently accepted
   predecessor pin (including a C3 successor where applicable), not an assumed
   C2 pin. Do not add ranges, old-or-new alternatives, inferred aliases, or
   fallback acceptance.
4. Require the Q0 test correction to be the only test-allowlist change, with
   its own source hash and fresh deterministic/full-regression evidence.

If C4 does not modify a file, no successor row or Q0 test edit is authorized
for that file. The amendment must not alter the C4 product criteria,
execution-guard authority, C1/C2 result policy, or AC interpretation.

## Verdict

**Luna forward-review verdict: conditional PASS, P0=0/P1=0** for
`AC-M5D7QC4-009/010`'s API-boundary and regression-evidence planning, subject
to Astra adding the narrow Q0 amendment gate before any affected Q0 test edit.
No new user decision is needed. Astra retains contract and integration
authority; Terra may not implement or revise the Q0 allowlist under the
current C4 Review text alone.
