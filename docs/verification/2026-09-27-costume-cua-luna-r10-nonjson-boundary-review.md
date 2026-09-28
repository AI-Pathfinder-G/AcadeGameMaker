# Costume CUA — Luna R10 NonJson boundary correction review

Date: 2026-09-27 (Asia/Seoul)

## Scope and inputs

Static-only review of the test-source correction against the R10 focused failure. Unity was not run and no implementation file was modified.

- Corrected adapter-test SHA-256: `DD9AED2D18E1747029B13CFD185A7DC59F6278FD94DC7863A8F902CADD4C9D2C`
- R10 XML SHA-256: `C2AFA45FE75BDF9CC1E8A9515C0117AC822C9D5B5E4DB6E9F108C2771BAB7DBD`
- R10 log SHA-256: `2D2C9E45A5275333AD20686FA56C51A8057C97B7CD2F37050954086848B309A0`

The XML records 195 tests with 185 passed and 10 failed. The ten failures are exactly six catalog/selection rows (`definitionActor`, `definitionCostume`, `definitionPending`, `definitionRejected`, `bindingActor`, `bindingCostume`) and four binding-only rows (`bindingCatalogRevision`, `bindingMissingAction`, `bindingExtraAction`, `bindingEmpty`); the remaining eleven NonJson rows passed.

## Route verification

### Route A — six catalog/selection rows

The six rows first call `Validate` with exact `Identity`, replace the catalog definition, capture a post-injection view, call the real adapter with media only, and require exact `Selection`. The preservation oracle retains the old published object/media/binding, captures the post-injection view, and checks move count, projection count, persistence status, and all view fields. This route has the intended result boundary and no expected-result weakening.

### Route B — four binding-only rows

The four rows call `Validate` directly with exact `Revision` (`bindingCatalogRevision`) or `Clips` (`bindingMissingAction`, `bindingExtraAction`, `bindingEmpty`). They return before `DefinitionFromBinding`, `ReplaceDefinition`, or `TrySelect`, use the pre-injection view, and run `AssertNoPublicBindingIngress`. The reflection checks all declared public constructors/method parameters, public fields, public setters, and events for binding ingress. Tuple, view, move/projection counts, and status are preserved by the helper.

### Route C — eleven remaining rows

The remaining eleven rows reach the real adapter and require `Package`: definition portrait/gameplay-set, synthetic, package revision, binding set/revision/hash variants. They perform the appropriate definition projection, capture the post-injection baseline, and use the same preservation oracle. Existing R3–R6 test methods and matrices remain present.

## Finding

**P1-001 — preservation oracle omits `MemoryPort.ReplaceCalls`.** `AssertNonJsonPreserved` hardcodes `MoveCalls == 1` and `Projection.Count == 1`, but does not capture or assert the pre-call `ReplaceCalls` count. The route requirements explicitly require tuple/count/status preservation; the existing stronger B1a oracle already demonstrates the required pattern by capturing `moves`, `replaces`, `projections`, and `status`. This is a test-evidence gap, not a runtime behavior finding, but it prevents independent closure of all count invariants for Routes A–C.

Required narrow correction: capture `var replaces = port.ReplaceCalls` before the candidate operation in the Route A/B/C setup, pass it to the preservation helper, and assert equality. Keep all route boundaries and expected rejection values unchanged.

## Verdict

**BLOCKED — P0=0, P1=1, P2=0.**

The route partition and expected outcomes are otherwise correct, but the missing replace-count assertion must be closed before accepting this boundary correction. R10 focused XML/log are consumed evidence and must remain immutable; fresh R11 focused/full stems will be required after the test-only correction and a new execution authorization.

