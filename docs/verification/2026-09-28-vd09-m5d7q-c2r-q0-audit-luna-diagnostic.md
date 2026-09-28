# Q0 audit hash diagnostic — Luna read-only review

Date: 2026-09-28. R23 is still running; this is not a failure result and no
source/test edits were made.

`HubUiOnlyQ0ScopeAuditEditModeTests.AC011_EvidenceBackedCurrentSourceHashesMatchTheAuthoritativeManifest`
(`Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs:94–102`)
requires the current `InputRouter.cs` and
`DesktopProfileLaunchAdapterV1.cs` hashes to equal the historical Q0 manifest:

- Router `6C1BBAA1F9AB4000F492233E96AC69D6B93DD5B32378F58C9EC92F13C4EA8341`;
- Adapter `3A3110407949A42FA172EBF0A14F4508364FF1C1D6B739B3EF62246912F08A3D`.

The current approved C2/C2R snapshot intentionally has different hashes
(Router `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`,
Adapter `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`).
Thus, if R23 includes AC011, this is a stale audit-manifest expectation rather
than a runtime regression. The Q0 Verified contract and historical evidence
must remain unchanged; a future bounded test-only correction should preserve
the historical hash rows/provenance while separately recognizing the approved
C2 current-source manifest, with Astra review before changing any gate.

Do not skip or waive AC011, rewrite historical evidence, or infer failure before
R23 produces XML. All other R23 namespace coverage remains required.
