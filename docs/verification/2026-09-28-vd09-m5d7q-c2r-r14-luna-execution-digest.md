# C2R R14 bootstrap execution digest — Luna

Date: 2026-09-28. R14 executed the frozen 37-row bootstrap scope.

Result: **37/37 passed**, 0 failed, 0 skipped/inconclusive; duration
27.8096041 s; Unity Editor PID 37956 exited 0 with no active editor remaining.
XML SHA-256: `5B2099E3AA12D8804E0647B573B1E560BD53B5DD48CADD9BF8A2A64E59CA13F5`.
Runtime hashes remained Adapter `C9D2CAE724113BEF186EC82FC5761367534F21A0ADAF7D3437765050279746E8`,
Router `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`,
Cutover `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`.

This closes only the bootstrap execution evidence for its applicable
AC-M5D7QC2R-001/002/003/004/007/008 rows. It does not close the separate
four-process AC-M5D7QC2R-005/006 fixture, the 73-row memory fault matrix, or
the remaining required legacy regression filter.

## Minimal required non-process filters

Run the existing assembly with these exact focused namespaces/classes:

- `AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetRestartBootstrapV1Tests`;
- `AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests`;
- `AcadeGameMaker.Input.Unity.PlayMode.Tests.DesktopProfileLaunchAdapterV1Tests`;
- `AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetMemoryCutoverFaultMatrixV1Tests`;
- profile reset EditMode namespace `AcadeGameMaker.Profile.Tests`, plus relevant
  Hub/InputUnity legacy classes required by the C2 AC matrix.

Explicitly exclude only
`AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetRestartProcessV1Tests`
from this non-process filter. Report selected XML and zero failed,
skipped/inconclusive rows; do not label it an unfiltered full-suite pass.

The process class must instead run as four separately orchestrated Main phases:
Prepare, Resume, DeleteGate, and OrdinaryAfterDelete, with external PID/base
ownership evidence. AC-005/006 remain open until those phases pass.
