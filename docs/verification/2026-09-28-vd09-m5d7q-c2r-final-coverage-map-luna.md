# C2/C2R final coverage map — Luna bounded review

Date: 2026-09-28. Read-only evidence map; no Unity execution or source edits.

## Current evidence by AC

| AC | Evidence status |
|---|---|
| C2 `001` | Existing C2/R10 evidence plus legacy regression; final required regression result remains pending. |
| C2 `002–006` | 73-row fault matrix is the applicable closure set; R21 is active/pending. |
| C2 `007` | R14/R15–R20 prove the restart lane pieces; final static/API and required-regression integration remain pending. |
| C2 `008–009` | Existing focused launch/handoff and static review evidence; final required filter still pending. |
| C2 `010` | Cannot close until R21 and required non-process regression XML have zero failed/skipped/inconclusive rows. |
| C2R `001–004,007,008` | R14 bootstrap 37/37 passed (XML SHA `5B2099E3AA12D8804E0647B573B1E560BD53B5DD48CADD9BF8A2A64E59CA13F5`); production HubUIOnly happy path and safety rows passed. |
| C2R `005–006` | R16/R17/R18/R19/R20 independent phase evidence reviewed and matched; DeleteGate has Main PID/death proof, intentionally no NUnit pass. |

Concrete remaining gates are R21’s 73-row matrix and the final required
non-process regression XML. No additional contract case was identified.

## Exact non-process filter

Run the existing test assembly using these namespaces:

- PlayMode: `AcadeGameMaker.Tests.PlayMode.InputUnity`;
- PlayMode: `AcadeGameMaker.Input.Unity.PlayMode.Tests`;
- PlayMode: `AcadeGameMaker.Tests.PlayMode.HubPresentation`;
- EditMode: `AcadeGameMaker.Profile.Tests`;
- EditMode: `AcadeGameMaker.Tests.EditMode.InputUnity`;
- EditMode: `AcadeGameMaker.Tests.EditMode.HubPresentation`.

Exclude exactly
`AcadeGameMaker.Tests.PlayMode.InputUnity.ProfileResetRestartProcessV1Tests`
from the non-process selection. This is runner isolation, not a pass or waiver.
Report discovered and executed counts per namespace; zero discovered is a
filter/setup failure, never a successful empty run. Do not call the result an
unfiltered full-suite pass.

The only concrete pre-acceptance omission is the pending R21 matrix and this
required regression XML. Process evidence is separate and already has the
phase-specific records above; no speculative new AC is required.
