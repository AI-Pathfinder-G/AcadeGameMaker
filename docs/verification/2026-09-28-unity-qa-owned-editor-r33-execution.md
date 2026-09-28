# UQW R33 actual owned-Editor execution — Astra

Scope: REQ-UQW-001..007 / AC-UQW-001..007. User explicitly resumed work
after the pause checkpoint. This is QA-tool execution evidence, not C2/C2R
final acceptance, a full regression run or live menu/gameplay evidence.

## Frozen implementation

- Runner SHA-256:
  `A4DD378FF02105345B63B40C6F446B304B04F9E8B894CCD2218806F42DD7DA85`.
- Deterministic test SHA-256:
  `2FABA87A41D70167DACAA25B13808852F1A9BF8C32146E1053E5E14005CDADF2`.
- Main reran the 104 deterministic checks successfully before actual launch.
- Existing independent final post-review maps all seven UQW ACs with P0/P1=0.

## Actual invocation and outcome

Main alone invoked the exact frozen runner with pwsh, EditMode,
`ProfileResetMemoryAuthorityV1Tests`, fresh absolute results/log paths, and
AwaitSeconds 600. Ordinary sandbox preflight could not read process inventory
and correctly blocked before launch. The separately approved host-context
invocation passed pinned Editor/signature/version/license presence and
no-competing-project checks. No license/configuration/security-policy change
or termination occurred.

The actual runner tool session 41514 returned exit 0 with
`Completed: passed=32 total=32` and ancillary launcher exit 0. The production
ownership/attachment and actual-exit checks were used; no test bypass was used.
The run finished before the first 30-second progress message, so no actual
Editor PID was printed. Do not invent a PID or claim it was independently
recorded. Main subsequently verified no Unity process remained.

`artifacts/uqw-r33-owned-editor-smoke.xml` SHA-256:
`EF97051B8B506272C5679EAB00A57AB848CCCF3417C88768D665FEB8273B5E2B`.
XML: total 32 / passed 32 / failed 0 / skipped 0 / inconclusive 0;
duration 1.1674246 seconds; start 2026-09-28 09:57:19Z,
end 2026-09-28 09:57:20Z. Matching `.log` records:
`Test run completed. Exiting with code 0 (Ok). Run completed.`

This closes the missing actual new-runner invocation evidence, subject to
Luna independent review and Astra integration. Historical R22 used the old
runner; its 610/614 actual failure remains unchanged. The optional Windows
PowerShell 5.1 attempt remains unverified due to host policy before loading;
no bypass is made. No second Unity was launched while another was active.

## Astra acceptance

Luna independently verified all seven ACs with P0=0/P1=0 in
`2026-09-28-unity-qa-owned-editor-wait-luna-r33-actual-review.md`.
Astra accepted only UQW tooling. Approved-at-execution contract SHA is
`BCD5ABF85BDAC07662E162FDB85989C14081AC1FCBEE71321598E9900FCC3580`;
current Verified contract SHA is
`D312AAF5188017438DF54FFC6FB62F5395CB70F2EA0ECF80117D563628DDA1AD`.
C2/C2R final regression and live product authority remain open.
