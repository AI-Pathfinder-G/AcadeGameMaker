# VD-09 M5D7M — Luna independent post-review

- Reviewer: Luna (independent; 2026-09-13)
- Contract: `docs/specs/work-contracts/2026-09-13-vd09-m5d7m-desktop-profile-launch-adapter.md` (Approved)
- Verdict: **PASS recommendation — P0=0, P1=0**
- Scope: read-only runtime/test/evidence review; no Unity rerun and no implementation, contract, or evidence edits

## Final execution evidence

I independently parsed each requested NUnit XML. Every test case was `Passed`; failed, skipped, and inconclusive counts were zero.

| Run | XML | Total / passed | Failed / skipped / inconclusive | SHA-256 |
|---|---|---:|---:|---|
| R50 focused M5D7M | `artifacts/unity-results/m5d7m-r50-focused-final.xml` | 117 / 117 | 0 / 0 / 0 | `B363048753EE89007615B16C52F713FC4BC431C32852AC5D93C67A4B1F80F08A` |
| R51 M5B5 regression | `artifacts/unity-results/m5d7m-r51-m5b5-final.xml` | 3 / 3 | 0 / 0 / 0 | `9639DA60D39720C877647897EA50FB81D28677F586175E86AD4CCA4AC5255472` |
| R37 M5D7L regression | `artifacts/unity-results/m5d7m-r37-m5d7l.xml` | 33 / 33 | 0 / 0 / 0 | `753FF66D62EA7C675275DEC115DEE0DBF108C6C6F392ED498961F3C6A92960D8` |
| R52 full EditMode | `artifacts/unity-results/m5d7m-r52-full-edit-final.xml` | 663 / 663 | 0 / 0 / 0 | `7EDDF68EEFA278621456F1A1A317374853A0D7DF4C88E85AA5D1643B25674FDD` |
| R53 full PlayMode | `artifacts/unity-results/m5d7m-r53-full-play-final.xml` | 734 / 734 | 0 / 0 / 0 | `0AAC9B3AF919E4B56ACF88C2CE4190294B80C90439D2269150ACD2A7F0DDD747` |

Unity 6000.6.0f1 licensing/entitlement resolution is present in the final logs; these runs are not license-blocked. The implementation-evidence document was not changed under this review scope and still contains superseded R14–R18 text; the final XMLs and hashes above are the independently verified execution identity.

## Reviewed source identity

| Item | SHA-256 |
|---|---|
| Approved contract | `A046DA3FDC58B83C86A64632652613C65D33D90AFB2147C3121607CCD59D464A` |
| `InputRouter.cs` | `1ABA6873EDD3F90050B07B57A61F609F22A98EDCB629269A62E2E8D8D2F84E69` |
| `DesktopProfileLaunchAdapterV1.cs` | `C1B27EE416315666FA2E7673433735CC78B2DA410D5438D98B48C947075E5C0F` |
| `DesktopProfileLaunchAdapterV1Tests.cs` | `3C02A8387A8221953DDB03A40F55373BD420CA896373FBECD51D41F409CE63D3` |
| Input.Unity asmdef | `CD3E47DD94BCCB9E537CA7AF4C6029FC8327EA0154C90EC7B6FD305218FE4C0E` |
| Input.Unity AssemblyInfo | `F41E2E8228CDCFD2314316F6F806489A6FA8585D4C1D0F07F989F9AB4301B90A` |

The source is within the Approved allowlist. The router’s central validator now covers lifecycle entry/exit, unknown and malformed state matrices, deferred legacy allocation, closed-fault configuration guards, exact callback/action cleanup, and the final `_faultStage == Initialization` distinction. The adapter remains limited to root/UTC capture, one M5D7L preparation, exact action transfer, existing M5B5 UI-only initialization, immutable receipt/notification publication, and fail-closed cleanup.

## Acceptance status

| Criterion | Independent status | Basis |
|---|---|---|
| AC-M5D7M-001 | PASS | Exact reservation-observation → root → UTC → preparation trace, normalized forwarding, and real isolated-root coordinator rows. |
| AC-M5D7M-002 | PASS | Focused default/current/previous/decode-repair/binding-apply/preservation/save outcome matrix with final UI-only receipts. |
| AC-M5D7M-003 | PASS | Exact prepared candidate identity, one-shot Take/re-Take rejection, transferred ownership, callback/map closure counters, and repeated-teardown proof. |
| AC-M5D7M-004 | PASS | Root/UTC/preparation, token/take/adoption, callback, initialization/publication, and pre-receipt teardown failures fail closed with original exceptions and no receipt/notification. |
| AC-M5D7M-005 | PASS | Foreign/duplicate owner, wrong mode/cohort, late reservation, enabled/reused candidates, reflection corruption, duplicate adapters, and closed legacy seams are covered. |
| AC-M5D7M-006 | PASS | Notification priority, overlap, one-shot consumption, receipt correlation, nested correlation mutation, and kind validation are covered. |
| AC-M5D7M-007 | PASS | All four component/object lifecycle permutations, inert prepared router Start, legacy compatibility, pre-Awake rejection, and pending teardown are covered. |
| AC-M5D7M-008 | PASS | R51 M5B5, R37 M5D7L, full EditMode, full PlayMode, static dependency, and allowlist checks are clean. |
| AC-M5D7M-009 | PASS | R50/R51/R37/R52/R53 all have zero failed/skipped/inconclusive tests; independent review is P0=0/P1=0. |

## Residual P2 notes

- P2-001: the focused reflection matrix uses null/field corruption for owner evidence rather than a separate non-null foreign-owner replacement case. Public owner-bound operations still fail closed, and no authority leak is present.
- P2-002: explicit nested trailing-separator, UNC, and alternate-separator normalization cases are less exhaustive than the implementation’s fully-qualified/GetFullPath/OrdinalIgnoreCase root checks. This is coverage precision only, not a runtime blocker.

No P0 or P1 finding remains. I recommend Astra integrate this independent PASS recommendation; Astra retains final acceptance/status authority.
