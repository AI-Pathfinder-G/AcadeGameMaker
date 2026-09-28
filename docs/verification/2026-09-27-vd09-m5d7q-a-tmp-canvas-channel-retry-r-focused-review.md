# VD09 M5D7Q-A canvas-channel retry-r focused evidence — Luna independent review

Date: 2026-09-27 (Asia/Seoul)
Reviewer: Luna (independent evidence review)
Scope: Focused Unity EditMode evidence review only. No Unity execution was initiated by this review.

## Exact evidence and source hashes

| Item | SHA-256 |
|---|---|
| `artifacts/unity-results/m5d7qa-20260923/focused-editmode-canvas-channel-retry-r.xml` | `E3F8D1ABE1EAD52B635AEBA6520FC87513D764EE8F5F2A9829546AF6BE995CC5` |
| `artifacts/unity-results/m5d7qa-20260923/focused-editmode-canvas-channel-retry-r.log` | `ADE03C11EE14AC9EB4C695859CEB3C4384014BE0602373C9286BE57C8C2D4C6C` |
| Generator expected/current | `8DB852A5D6CAF1DA4883ED3D27DEB909CA5B877DA4D6BA10246E9CAE934E0C90` |
| Focused tests expected/current | `D7A41D0B6F4B6F703D02BC6CD0DF5316DFB95B338128A749928D2CAAA0A13208` |
| Immutable prior `resolution-capture-retry-q.log` | `0757D516E57C75B58ECA23E1491A812882778BDF1276F3D7607D6ACAE9A3C2CB` |

## Execution and case results

The XML reports Unity Test Framework result `Passed`, `61/61` passed, `0` failed, `0` skipped, and `0` inconclusive. The 12 new channel cases all passed:

- representative A/B lifecycle case;
- three independent missing-bit cases (TexCoord1, Normal, Tangent);
- two independent extra-bit cases (TexCoord2, TexCoord3);
- non-channel Canvas profile drift;
- four fault boundaries (after TMP, before render, after render, readback);
- cleanup-failure primary-fault aggregation.

The log contains the expected deterministic channel observations and failure codes: exact `mask=25`/`names=TexCoord1,Normal,Tangent` for successful after-TMP/render phases, `mask=0`/`names=None` for baseline/finally phases, three `AFTER_TMP_MISSING_BITS`, two `AFTER_TMP_EXTRA_BITS`, and one `CANVAS_PROFILE_DRIFT`. The focused suite is not the full capture run, so its 66 diagnostic observations are representative/probe evidence; the subsequent retry-r capture must independently produce the required 80 ordered observations.

## Command, freshness, and immutability

The log records Unity `6000.6.0f1`, `com.unity.ugui@2.6.0`, the exact focused test filter `AcadeGameMaker.Tests.EditMode.HubPresentation.HubPresentationAuthoringTests`, and both expected source hashes. The command line contains `-batchmode` and `-runTests` but no explicit `-quit`, and writes only the immutable retry-r XML/log stems. The files are fresh for this run (`2026-09-27 08:46:17Z`–`08:46:21Z` in XML; matching log mtime), and prior retry-q evidence remains present with the recorded unchanged hash.

## Gate verdict

**PASS — P0=0, P1=0, P2=0. Retry-r capture is authorized to proceed.**

This passes only the focused evidence gate. The retry-r capture must still prove 80 ordered channel markers, 120 strict TMP observations, unchanged layout/render/state/publish evidence, and process exit 0 before any fresh-process proof or integration.

