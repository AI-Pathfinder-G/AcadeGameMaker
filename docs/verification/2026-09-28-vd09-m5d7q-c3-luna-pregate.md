---
status: Review
---

# VD-09 M5D7Q-C3 Luna pre-gate review

- Date: 2026-09-28
- Reviewer: Luna (independent pre/post adversarial QA)
- Contract reviewed: `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c3-confirmation-owner.md`
- Contract status: Review; not Approved; no execution authority
- Scope: read-only contract/source audit; no Unity launch and no source changes

## Result

The amended contract closes the previously identified baseline-promotion gap at
the design level. The owner-bound successor-baseline-pending witness now
overrides the ordinary cursor `Ready` promotion; `FixedUpdate` must perform the
single exact `TryAdvance`, wait on `false`, validate and discard the first
`true` frame, and only then publish `Ready`. Foreign cursor, skipped tick,
corrupt witness, disable, and destroy are terminal. This is a closure of the
pre-review P1, not implementation evidence.

The current checked-in presenter still contains the old ordinary promotion at
`HubMenuPresenterV1.cs:103,106` and the ordinary `FixedUpdate` gate at
`HubMenuPresenterV1.cs:112-118`; therefore no runtime acceptance or P1=0 claim
is made until Terra implements and tests the amended witness. Current cursor
factory behavior remains relevant: `UiSemanticFrameV1.cs:124-130` may return
`Ready` immediately from an existing UI-only receipt.

## Acceptance-criterion trace

- **AC-M5D7QC3-001:** Contract covers exact request, receipt, owner/router/root,
  topology, epoch, duplicate, foreign, default, and reflection-corrupt rows.
  Static source supports exact active-cohort/root checks in
  `DesktopProfileLaunchAdapterV1.cs:50-64`; execution unverified.
- **AC-M5D7QC3-002:** Contract closes all Primary/Previous/Temp classifications,
  including missing, default, single-field non-default, recovery, invalid,
  unsupported, and unreadable rows; same-lease C1 projection is required.
  Tests unexecuted.
- **AC-M5D7QC3-003:** Contract separates meaningful/ambiguous classification
  from `DecisionRequired`, binds copy/labels and one-shot callbacks, and
  rejects forged/copied/late/concurrent/reentrant capabilities. Unverified.
- **AC-M5D7QC3-004:** Contract requires complete fresh three-leaf identity
  recapture and changed/default/ambiguous transition handling before C1.
  Unverified.
- **AC-M5D7QC3-005:** Contract preserves Q-A/Q-B history, retains notice state,
  and makes cancellation byte/profile/map/receipt/scene neutral. Unverified.
- **AC-M5D7QC3-006:** Amended successor witness closes the prior baseline P1;
  required discard/no-interpret/fault behavior is specified. Current source has
  not implemented this witness; Unity execution unverified.
- **AC-M5D7QC3-007:** Contract distinguishes initial Busy retry, confirmation
  Busy retry, stale, reload, manual-repair, and post-barrier terminal paths.
  Unverified.
- **AC-M5D7QC3-008:** Contract requires capability invalidation before C1 Begin
  and terminal closure on execution/rearm faults. Unverified.
- **AC-M5D7QC3-009:** Static review found no new C2/destination/scene/gameplay,
  map, save, wardrobe, network, clock, or random authority in the contract;
  implementation/API review remains required after Terra changes.
- **AC-M5D7QC3-010:** Not satisfied at this gate: no C3 Unity suite result is
  claimed. The reported C2 Profile R3 result (30/30, all zero) is not C3
  acceptance evidence, and a partial .NET run is not acceptance evidence.

## Reviewed source hashes

SHA-256:

- Contract: `423DE7B2ABCBB0267768EB596E9230840B289E0FDD1D257F5005944A62ABA999`
- `Input/Unity/UiSemanticFrameV1.cs`: `CCD7763A1103BA096DD64E02C5B91A99A3ADA21D49506CC81E5F0601E06D316F`
- `HubPresentation/Unity/HubMenuPresenterV1.cs`: `A45FCE08EA626F3EBEF4499C920FBF5694CF0CEB14B1008AC63105F3B9DC8096`
- `Input/Unity/DesktopProfileLaunchAdapterV1.cs`: `56648EB97046836011833C1A9A1AEA15BEB30182749A1EA1D0172BB7E873D748`
- `Input/Unity/ProfileResetMemoryCutoverV1.cs`: `F2DBDB3EE162A6E713F10454EFE042A956DD69C2B8BE2509873D2C75BDDB6BA4`

Luna pre-gate conclusion: **contract P1 closure noted; implementation and all
ACs remain unverified; Astra approval is required.**
