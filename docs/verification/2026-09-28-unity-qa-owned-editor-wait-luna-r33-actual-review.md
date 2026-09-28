# Unity QA owned-Editor wait — R33 actual invocation review — Luna

Date: 2026-09-28  
Scope: independent verification of the resumed exact-runner invocation and
deterministic UQW tooling; no source, test, filter, Unity, CIM, licensing, or
live-UI changes.

## Frozen inputs and artifacts

The reviewed frozen inputs remain unchanged:

- Runner `qa/tools/Invoke-UnityQa.ps1`: SHA-256
  `A4DD378FF02105345B63B40C6F446B304B04F9E8B894CCD2218806F42DD7DA85`.
- Deterministic self-test `qa/tools/Test-UnityQaOwnedEditorWait.ps1`: SHA-256
  `2FABA87A41D70167DACAA25B13808852F1A9BF8C32146E1053E5E14005CDADF2`.
- Actual result XML `artifacts/uqw-r33-owned-editor-smoke.xml`: SHA-256
  `EF97051B8B506272C5679EAB00A57AB848CCCF3417C88768D665FEB8273B5E2B`.
- Actual log `artifacts/uqw-r33-owned-editor-smoke.log`: SHA-256
  `2886486DE20AE78510C2ABFAA137AD6C9C5AC0A6F2D2AE72BA3608F19E1B651A`.

The main invocation used the authorized `pwsh` runner with the
`TestPlatformEditMode` platform, filter
`ProfileResetMemoryAuthorityV1Tests`, `AwaitSeconds 600`, and the R33 result
and log paths. The coordinating CLI session exited 0 with
`[PASS] Completed32/32` and launcher exit 0. The actual Unity process ID was
not printed by the completed invocation and is intentionally not inferred.

## Independent result inspection

The XML `test-run` is `result="Passed"`, `testcasecount="32"`, `total="32"`,
`passed="32"`, `failed="0"`, `skipped="0"`, and `inconclusive="0"`, with
start `2026-09-28 09:57:19Z`, end `2026-09-28 09:57:20Z`, and duration
`1.1674246`. The selected fixture is
`AcadeGameMaker.Profile.Tests.ProfileResetMemoryAuthorityV1Tests`.

The log records the R33 XML path, `Test run completed. Exiting with code 0
(Ok). Run completed.`, and normal Unity shutdown. The resumed run therefore
provides actual owned-Editor completion evidence for this exact frozen runner;
it does not establish product, scene, menu, live UI, or C2/C2R acceptance.

An independent local deterministic invocation of the frozen self-test returned:

```text
PASS: 104 deterministic owned-Editor wait self-tests.
```

No Unity or licensing operation was performed for that deterministic check.

## AC-UQW closure scope

- **AC-UQW-001:** PASS for the previously reviewed deterministic exact-path,
  prefix/quoted-path, case/separator, flag, worker, duplicate, and unavailable
  inventory matrices; the frozen source hash matches.
- **AC-UQW-002:** PASS for the previously reviewed launcher/actual-Editor,
  early-XML, exit, attach, creation-identity, and PID-reuse matrices; this R33
  invocation additionally demonstrates an actual completed run. No PID is
  asserted from absent output.
- **AC-UQW-003:** PASS for the previously reviewed bounded clock, discovery,
  progress, single-launch, and no-kill/relaunch matrices.
- **AC-UQW-004:** PASS for the previously reviewed closed XML counters and
  failure/ambiguity/zero-discovery classifications; R33 independently meets
  the valid nonzero all-passed XML condition.
- **AC-UQW-005:** PASS for the previously reviewed license/transient refusal,
  missing/unreadable XML, and secret-redaction distinctions. R33 log review
  found no basis to reclassify its valid result as a licensing failure.
- **AC-UQW-006:** PASS for the 104 deterministic self-tests and the frozen
  tool scope; no production bypass, dependency, Assets, runtime, or
  configuration change was used by this verification.
- **AC-UQW-007:** PASS within its stated evidence scope: exact frozen hashes,
  independent 104-check result, and one later actual invocation of that exact
  runner are now recorded. This is UQW-tooling evidence, not Astra integration
  acceptance and not live product/UI acceptance.

Verdict: **Luna independent verification PASS, P0=0, P1=0** for the UQW
contract evidence scope above. Astra remains the sole integration authority.

## R22 fixture-selection diagnostic — pending review boundary

The preserved R22 result remains historical and unchanged: 614 total, 610
passed, four external-process environment failures, zero skipped/inconclusive.
The intended exclusion failed for both the external process class and the
R25-owned 74-row memory-cutover matrix. It must not be converted into a pass.

No new Terra fixture-selection diagnostic artifact was present at this review
boundary. When supplied, Luna will review it read-only against these minimum
conditions before any new execution is considered:

1. Selection is an exact fully qualified-name set, with explicit discovered
   and selected counts and names; substring or broad regex matching is not
   sufficient.
2. The R22 set contains the three approved PlayMode namespace surfaces and all
   446 scoped historical baseline names, while excluding both
   `ProfileResetRestartProcessV1Tests` and the 74 R25 matrix names.
3. Reconciliation reports set differences and duplicate overlap against R25;
   no zero-discovery or silently omitted class is accepted.
4. The diagnostic is read-only and makes no source, filter, runtime, fixture,
   license, or Unity changes. A diagnostic count cannot be treated as a test
   pass or as C2/C2R acceptance.

This preserves the R22 `614/610/4` historical record and leaves C2/C2R's final
gate open pending the separately authorized corrected run and review.
