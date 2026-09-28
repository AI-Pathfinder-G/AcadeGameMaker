# C2/C2R final independent acceptance digest — Luna

Date: 2026-09-28. Read-only independent verification; no source, test, spec,
Assets, Unity, or runtime changes. This digest reports Luna’s evidence verdict;
Astra retains final contract integration authority.

## Fresh closure checks

R36 independently verifies the previously open worker-provenance slice:

- `artifacts/c2-r36-final-editmode.xml`: SHA-256
  `6ADC451890CADDE6729E0D654D122042CFC12FB7B3ED2D1309ED2643C12A76B8`;
  51/51 passed, failed/skipped/inconclusive 0, exact expected names 51/51.
- `artifacts/c2-r36-final-editmode.log`: SHA-256
  `641D6DA917936736F35FBEA110AEC3C414DB2779DA75347DAB082C2AAD330EE2`;
  launcher exit 0 and owned editor PID 39752 was observed gone.
- Before manifest `artifacts/c2-r36-source-before.json`: SHA-256
  `2CAB80CC20A01165778905F67161866906F439C6E5AAFC5E316D1813628F5EA4`.
  All 669 declared paths rehashed successfully: missing 0, changed 0.
  After manifest SHA-256 is
  `B0D8D917D1A9D65B0D4B2B66EDEDAEB4C097BE7F2A36FA01FDFE603E8303E218`;
  before/after paths and hashes match exactly. The shared R35 667 entries
  remain unchanged; the two additional paths are the worker `Program.cs` and
  `.csproj`, with no applicable repository config files found.
- R35’s non-worker complement is 562 names. `562 + 51 = 613` distinct names,
  with missing 0 and extra 0 against the current R35/R36 union. The historical
  592 rows and unreproduced historical 202-file digest are not reused.

R34 independently established the fresh PlayMode partition at 536/536 with
zero failed/skipped/inconclusive cases and exact selector names; its disjoint
R25 matrix contribution is 74, yielding 610 distinct names. R31 contributes
45/45 focused corrective rows, and R32 contributes 21/21 corrective rows
including the current 17 controller and four Q0 rows. R26–R30 process evidence
is retained as phase evidence: Prepare/Resume/OrdinaryAfterDelete passed in
their exact XMLs, while DeleteGate is a real PID/death observation and is not
reported as an NUnit/XML pass. R29 therefore remains factual harness history,
not a fabricated test success.

## Criterion verdict

| Criterion | Independent evidence | Verdict |
|---|---|---|
| AC-M5D7QC2-001 | Existing source review plus final fresh regression partitions | PASS |
| AC-M5D7QC2-002–006 | R25 74-row matrix, R34 PlayMode 536, exact current EditMode union 613, and source reviews | PASS |
| AC-M5D7QC2-007 | R26–R30 process phases, R31/R32 corrective evidence, and C2R phase review | PASS |
| AC-M5D7QC2-008–009 | Focused launch/handoff, Q0, and static/API reviews; no UI/scene/gameplay scope expansion | PASS |
| AC-M5D7QC2-010 | R34 536/536, R35 613/613, R36 51/51, exact names/manifests, and zero F/S/I | PASS |
| AC-M5D7QC2R-001–004 | R31/R32 corrective rows and existing bootstrap/source reviews | PASS |
| AC-M5D7QC2R-005–006 | R26–R30 exact process-phase evidence; DeleteGate explicitly non-NUnit | PASS |
| AC-M5D7QC2R-007 | Static/API, exact selector, provenance, and source-hash reviews | PASS |
| AC-M5D7QC2R-008 | This Luna report records the final independent P0/P1 result | PASS |

Historical R22 `614/610/4` and R23 failure rows remain preserved and are not
rewritten as current passes. No product/live-UI acceptance is inferred from
these tests. No old/new compatibility fallback or unbounded full-suite claim
is made.

**Luna independent final verdict: PASS; P0=0, P1=0.** This is an evidence
acceptance recommendation for the cited C2/C2R criteria, pending Astra’s final
integration and contract-status recording.
