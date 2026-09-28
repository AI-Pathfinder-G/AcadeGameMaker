# C2R startup-compatibility clarification — Luna contract pre-gate

Date: 2026-09-28  
Scope: independent review of the appended **“Startup compatibility clarification — Review only, implementation withheld”**. Runtime, tests, and Unity execution were not changed or run.

Reviewed contract SHA-256: `1B546FD7C7D670543A945A8A1BEACB5534334DFC841EA39D4E65D6C0FB00DECA`.

## Result

Contract P0: **0**. Contract P1: **0**. The appendix is technically coherent and closes the previously identified root-ordering incompatibility without weakening the global Approved status or authorizing implementation by itself.

The clarification is sound because it requires:

- an exact actionless owner reservation before the single root getter;
- one normalized root reused for barrier selection and the absent ordinary trace;
- irreversible promotion of only the exact pristine owner, with no observable
  `Unreserved` interval, ordinary fallback, actions, history, callbacks, or fake
  C2 identity;
- preservation of the historical exception path when reservation fails or the
  getter throws before returning a value;
- explicit C2R repair classification only after a returned value is known to be
  invalid/unsafe, while unexpected programmer exceptions retain their identity;
- private recovery identity and teardown that close the acquired owner without
  ordinary cleanup, retry, or re-minting.

This preserves `Reserve -> root -> UTC -> Prepare -> Take -> Adopt` for the
absent ordinary lane, and gives committed/unsafe returned-root states a bounded
private recovery/repair promotion. The proposed focused tests cover the material
remaining seams: A/B single-lookup protection, promotion before/after router
Awake, exact/foreign/duplicate/corrupt promotion, teardown, and no-actions/no-
history/no-fallback evidence.

## Gate

No new blocker is raised. This is a contract review only; Astra may approve this
appendix explicitly. Terra implementation remains withheld until that approval,
and the amended runtime must receive a fresh Luna source review plus required
execution. Existing C1/C2 algorithms, ordinary failure assertions, and the
global Approved status remain unchanged.
