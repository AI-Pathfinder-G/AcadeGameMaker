# C2R R13 prefreeze correction — Luna review

Date: 2026-09-28. Source/static review only; no code edits or Unity execution.

## Corrected review

My prior R13 warning that Adapter Awake lacked a HubUIOnly discriminator was
based on the preceding snapshot and is withdrawn for the current frozen files.
The current `DesktopProfileLaunchAdapterV1.cs` has
`recoveryEligible = _router.IsAuthoredHubUiOnlyForRecovery` before selection.
It captures the root exactly once outside the classification try; non-Hub
variants use legacy `Normalize`/ordinary behavior, while only eligible
HubUIOnly variants validate/probe barriers and may promote private recovery.
Getter-thrown exceptions therefore retain the historical path. The current
runtime hashes are Adapter
`C9D2CAE724113BEF186EC82FC5761367534F21A0ADAF7D3437765050279746E8`, Router
`66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`, and
unchanged Cutover `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`.

## Remaining static gate

No new runtime P0/P1 is found in this corrected snapshot. The 37-row bootstrap
fixture adds actual HubUIOnly unsafe-returned-root ManualRepair rows and its
ever-issued, idempotent cleanup validates ownership, ancestry, and descendant
reparse refusal. Direct promotion rows remain correctly labeled router
transition-unit coverage; production Adapter Awake/private-CWT evidence still
depends on R13 execution. Legacy non-Hub root/error expectations remain
historical and are not weakened.

R13 execution must still report XML and zero failed/skipped/inconclusive rows;
this prefreeze record is not acceptance.
