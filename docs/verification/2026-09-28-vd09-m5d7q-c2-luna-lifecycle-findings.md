# M5D7Q-C2 Luna lifecycle findings

Date: 2026-09-28  
Reviewer: Luna (bounded independent read-only review)  
Scope: `InputRouter.cs`, `DesktopProfileLaunchAdapterV1.cs`, and `ProfileResetMemoryCutoverV1.cs`; lifecycle containment, shared terminal truth, candidate ownership, exact pair, and final receipt authority.

This note is an independent finding record only. It is not an acceptance claim and does not close P0/P1 findings. No Unity execution or source edits were performed by this review. The focused Unity R5 run was already in progress under the main execution lane; the reviewer did not run it. The reported R5 state is therefore only the main lane's partial `8/8` observation, not independent evidence.

Main-recorded R5 review baseline hashes (SHA-256):

- `InputRouter.cs`: `6B3FF2C19408A1E2FE7B07A154D1767934B6DAEA72B945EF149F872260634CB6`
- `DesktopProfileLaunchAdapterV1.cs`: `4FDF71A85D7D5F2DF505EDDA7756DD103A543056848296DDFB8C0CA154A478BE`
- `ProfileResetMemoryCutoverV1.cs`: `FAD0E29D9AACDC6BD438DB06B38ED3B056C5FFCE816CB477EB3A71D067F6293B`

Astra evidence correction: the initial note captured later hashes
`58A848...`, `228F14...`, and `84A9A0...` after concurrent Terra edits. Those
are not a valid baseline for the earlier line-specific history-clearing
finding. Findings below describe the R5-era review; line numbers may shift.
Terra subsequently reported removing history clearing and adding assertions,
but that changed source has not yet been executed or independently accepted.

## Findings

### P1 — Candidate reference is not independently owner-bound

`DesktopProfileLaunchAdapterV1.cs:188-205` stores `_untransferredCandidate` as a mutable reference, without a private immutable/CWT witness tying it to the exact `staged` object adopted by the coordinator or to the router's candidate identity. A coordinated reflection replacement with another valid disabled `GameInputActions` can therefore be passed into `InputRouter.EnterResetCutoverTerminal`; the original staged candidate is then lost when `ProfileResetMemoryCutoverV1.cs:75` clears its local ownership after adoption. This risks foreign transfer and unproven exact disposal.

Mapping: **REQ-M5D7QC2-002/004**, **AC-M5D7QC2-002/004**.

### P1 — Cohort witnesses are equality checks, not immutable cohort binding

`DesktopProfileLaunchAdapterV1.cs:26-31,54-65,143-148` keeps duplicated root/router witnesses but does not bind them through a private immutable issuance witness to the original launch cohort, `_launchRootCandidate`, the serialized router, or a receipt-owned identity. A coordinated reflection mutation of the router, both router witnesses, and both root witnesses to another active UI-only cohort can satisfy `HasExactResetLaunchCohort`/`IsExactResetRouter`. C2 may then operate on a foreign router/root while the actual launch router remains live.

Mapping: **REQ-M5D7QC2-003/007**, **AC-M5D7QC2-003**.

### P1 — Failure latch trusts mutable router witness

`DesktopProfileLaunchAdapterV1.cs:224` routes failure containment through `_launchRouterWitness`. If that witness is corrupted after entry, the adapter can set its own terminal state while failing to latch the actual launch router. The actual router could consequently retain input/callback authority after a C2 failure.

Mapping: **REQ-M5D7QC2-003/005**, **AC-M5D7QC2-003/005**.

## Confirmed existing concern

`InputRouter.cs:323-324` clears `_currentReceipt`, `_currentUiFrame`, and their publication proofs on terminal entry. This removes forensic history that the contract requires to remain immutable evidence, even though terminal getters may reject it as current authority.

Mapping: **REQ-M5D7QC2-003/007**, **AC-M5D7QC2-003/008**.

No acceptance recommendation is made. The findings remain for Astra/Terra disposition and Luna's later independent verification.
