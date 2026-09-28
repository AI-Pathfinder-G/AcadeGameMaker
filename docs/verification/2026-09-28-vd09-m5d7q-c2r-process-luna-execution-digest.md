# C2R process AC-005/006 execution digest — Luna

Date: 2026-09-28. Independent read-only evidence review; no Unity execution or
source edits by Luna.

R15 stopped before tests due ILPP runner startup failure: no C# test result and
no XML; it is not a pass and is retained as harness history. The fresh phases
then produced:

- R16 Prepare: 1/1 passed, XML SHA
  `552F32DC9C8B6224C96D0FF9F582D68FAE5FD0CD0AFFA507D79E144BCCE44593`,
  PID 49420, `DiskPrepared`, exact transaction and non-default old hash.
- R17 Resume: 1/1 passed, XML SHA
  `5225233DBB1DF856DE23E122A5DE2F8E6ADD45ED55A9ABCB4252660BC4130FB7`,
  PID 51208, `Completed`, exact r0 hash, maps disabled, history none, one-shot
  receipt, old progress absent.
- R18 Prepare (separate death-boundary base): 1/1 passed, XML SHA
  `C927A38BDB4922D410EC9B7DFFDB02C1D1232FB8915C496D3A4B945EB4EACE09`,
  PID 50796, exact old hash/transaction.
- R19 DeleteGate: Main observed `deleted=true`, signal PID 44004 matched the
  actual child, and Main killed/confirmed termination of that exact PID. This
  phase intentionally has no NUnit/XML pass claim.
- R20 OrdinaryAfterDelete: 1/1 passed, XML SHA
  `4BDD28A76973100147005E51654044BEA83C6FD1E99E5FB9DA66D91E1C592FF7`,
  PID 51136, ordinary preparation=1, UTC=1, reset receipt none.

The evidence files match the XML outcomes and owned phase records. This closes
the process evidence for AC-M5D7QC2R-005/006 subject to Astra integration; it
does not replace the 73-row matrix or required non-process regression result.

## Correct focused filter namespaces

Use the existing assembly with these exact namespaces/classes, excluding only
the external process class:

- `AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetRestartBootstrapV1Tests`;
- `AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests`;
- `AcadeGameMaker.Input.Unity.PlayMode.Tests.DesktopProfileLaunchAdapterV1Tests`;
- `AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetMemoryCutoverFaultMatrixV1Tests`;
- EditMode profile reset legacy namespace: `AcadeGameMaker.Profile.Tests`;
- relevant Hub/InputUnity classes in
  `AcadeGameMaker.Tests.EditMode.InputUnity` and
  `AcadeGameMaker.Input.Unity.PlayMode.Tests`.

Explicitly exclude only
`AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetRestartProcessV1Tests`
from the non-process filter; do not call the result a full unfiltered pass.
