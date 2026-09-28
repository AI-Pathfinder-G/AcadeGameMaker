# C2R actual process execution — Main evidence

Date: 2026-09-28. Approved scope: REQ-M5D7QC2R-006,
AC-M5D7QC2R-005/006; parent AC-M5D7QC2-007 remains pending final review.
This is real distinct-Unity-process evidence, not fresh objects in one process.
No overall C2/C2R acceptance is recorded here.

## Frozen source

- Adapter: `C9D2CAE724113BEF186EC82FC5761367534F21A0ADAF7D3437765050279746E8`.
- Router: `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`.
- Cutover: `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`.
- Process fixture: `F612D9E042D2748BA2D23E2C35CCCA693C1225EA0356A8A9AF41F9D2E0545AD4`.

Main created two separate, uniquely named OS-temp bases, direct children of
OS temp, in the approved `AcadeGameMaker-C2R-Process-<32 hex>` form. The
fixture validates the actual base and non-reparse ancestry. No real user
profile is used. Source workers remained frozen throughout execution.

## Fresh-process resume

The first launch R15 failed before tests: Unity's IL post-processing runner
failed to start and its named pipe was absent. The log is preserved; no XML,
test pass or source compiler error is claimed. Main confirmed the Editor had
exited and the owned base was empty, then made one unchanged fresh launch.

R16 Prepare: actual PID **49420**, logged exit 0, XML 1/1 passed with zero
failed/skipped/inconclusive. `artifacts/c2-r16-process-prepare.xml` SHA-256
`552F32DC9C8B6224C96D0FF9F582D68FAE5FD0CD0AFFA507D79E144BCCE44593`.
The production C1 transaction preserved exact non-default revision-7
Solidarity/CommonReferencePlane profile bytes in its manual archive and left
a regular committed barrier. Evidence: `artifacts/c2-r16-prepare.evidence.txt`.

R17 Resume: distinct actual PID **51208**, logged exit 0, XML 1/1 passed
with zero failed/skipped/inconclusive.
`artifacts/c2-r17-process-resume.xml` SHA-256
`5225233DBB1DF856DE23E122A5DE2F8E6ADD45ED55A9ABCB4252660BC4130FB7`.
Actual authored Adapter Awake/Start invoked production Resume and C2R, proving
exact-r0 disk/current-cell agreement, matching root/actions/generation,
disabled maps, no ordinary UTC/Prepare/history/notification, old disposal
NotApplicable, barrier absence and one-shot receipt. Evidence:
`artifacts/c2-r17-resume.evidence.txt`.

## Death after delete, before receipt

The second owned base was independently seeded by R18 Prepare: PID **50796**,
logged exit 0, XML 1/1 passed with zero failed/skipped/inconclusive.
`artifacts/c2-r18-process-prepare-death.xml` SHA-256
`C927A38BDB4922D410EC9B7DFFDB02C1D1232FB8915C496D3A4B945EB4EACE09`.
Evidence: `artifacts/c2-r18-prepare.evidence.txt`.

R19 DeleteGate reached the real AfterDelete boundary. Its signal was
`pid=44004 phase=DeleteGate deleted=true`. Main matched PID **44004** against
the live CIM record: Unity.exe, the exact R19 log path, parent 50388, creation
2026-09-28 13:39:00 +09:00. Only then Main terminated that exact test PID and
confirmed it no longer existed. No licensing process or unrelated Editor was
terminated. The fixture never spawns/kills workers.
Evidence: `artifacts/c2-r19-deleted.signal` and
`artifacts/c2-r19-deletegate.evidence.txt`.
Intentional termination has **no passing NUnit XML** and is not counted as a
test pass or a missing-XML success.

R20 OrdinaryAfterDelete: distinct actual PID **51136**, logged exit 0, XML
1/1 passed with zero failed/skipped/inconclusive.
`artifacts/c2-r20-process-ordinary-after-delete.xml` SHA-256
`4BDD28A76973100147005E51654044BEA83C6FD1E99E5FB9DA66D91E1C592FF7`.
Actual ordinary launch observed exact r0, UTC=1, Prepare=1, no barrier and no
reset receipt/current-reset cell. The seeded old progress was not resurrected.
Evidence: `artifacts/c2-r20-ordinary.evidence.txt`.

## Remaining gate

Luna independently checks XML, source and distinct-process evidence. The
73-row C2 fault matrix and required non-process regressions remain separate
execution gates. Neither these phase passes nor R14's 37 bootstrap passes
alone make C2/C2R Verified or grant interactive menu, scene or gameplay authority.
Owned temp bases are retained until evidence review; no recursive deletion or
archive cleanup occurred in this record.

## Final adapter snapshot repeat — R26–R30

Main repeated actual processes after R31 focused 45/45, with adapter
`0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`,
router `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`,
cutover `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`,
C1 `168D1539EAF359C3242ECABADAC0776956370E52105007D9224566F7F9C3FDC4`,
and process fixture `F612D9E042D2748BA2D23E2C35CCCA693C1225EA0356A8A9AF41F9D2E0545AD4`.
Earlier phases remain evidence for their earlier snapshot, not overwritten.

Fresh base A:
`C:\Users\me\AppData\Local\Temp\AcadeGameMaker-C2R-Process-40193418085748579f600cf46215d5bc`.
R26 Prepare PID 50812, 1/1, duration 0.1206251 s, XML
`artifacts/c2-r26-final-prepare.xml` SHA
`B89D8EC7EEE3D9C31A50FC0CCDCAC3546316CFE4F2FD1C0E2F5CF5AD8EAA995C`.
Transaction `7516f9bc5edbdc20981cd0b92bcb61b7`, old hash
`a97eeb88a787cf5acb5830999c48209eefcf8df369e51d14e8a5e31739ffe802`.
R27 Resume distinct PID 49072, 1/1, duration 0.1545958 s, XML
`artifacts/c2-r27-final-resume.xml` SHA
`E02F60AF39B8033259AA387EEFF70A89CEEDAAF6954E9FF15DB0A981B5A14349`.
Exact default hash `a658eaee8b36690a716eb04c52a547ee6cb434d383aedbe46fd1a4bdaa1ae367`,
disabled maps, no launch history, one-shot receipt and no old progress verified.

Fresh base B:
`C:\Users\me\AppData\Local\Temp\AcadeGameMaker-C2R-Process-f89371ee72984e5085d429fbd428bc1b`.
R28 Prepare PID 1296, 1/1, duration 0.1080939 s, XML
`artifacts/c2-r28-final-prepare-death.xml` SHA
`95969B585DAF1F33DB6BBDB2BD587F1E3B6B5A179B1AF15D1976C5F706DFB0FB`.
Transaction `3eda2e5b87de30f831a582e6d3e54c92`, same non-default seed hash.
R29 actual AfterDelete signal: `pid=47288 phase=DeleteGate deleted=true`.
Main validated CIM Unity.exe PID 47288, parent 51584, creation
2026-09-28 14:40:11.319638 +09:00, exact project and R29 log command-line
identity, then terminated only that PID and confirmed absence at
14:40:44.5055127 +09:00. No unrelated Editor/licensing process was killed.
No R29 passing XML is claimed: the wrapper's missing XML is expected for this
intentional death boundary, not an NUnit success.
R30 fresh OrdinaryAfterDelete PID 28508, 1/1, duration 4.1903509 s, XML
`artifacts/c2-r30-final-ordinary.xml` SHA
`F419263D9784547719B8B59BD9C633450946BE78D354B30D95CF4852ABA0F406`.
Ordinary Prepare=1, UTC=1, no reset receipt and no old-progress resurrection.
All completed XML phases have failed/skipped/inconclusive 0 and wrapper exit 0.

Six phase/signal files were copied without overwrite to
`artifacts/c2-r26-final-prepare.evidence.txt`,
`artifacts/c2-r27-final-resume.evidence.txt`,
`artifacts/c2-r28-final-prepare.evidence.txt`,
`artifacts/c2-r29-final-deleted.signal`,
`artifacts/c2-r29-final-deletegate.evidence.txt`,
`artifacts/c2-r30-final-ordinary.evidence.txt`.
Both owned bases remain retained. AC-M5D7QC2R-005/006 await Luna's final-snapshot
independent digest; full matrix, regressions and final integration remain open.
