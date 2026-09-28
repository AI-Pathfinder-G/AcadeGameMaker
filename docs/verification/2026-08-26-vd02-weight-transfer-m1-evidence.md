# VD-02 Deterministic Weight Transfer M1 Evidence

- Date: 2026-08-26
- Implementer: Terra; Kimi K3 supplied only rejected draft/decomposition input
- Integration and evidence owner: Sol
- Independent verification: Luna PASS, 2026-08-26
- Work contract: `docs/specs/work-contracts/2026-08-25-vd02-weight-transfer.md`
- Requirements: `REQ-WT-001~008`, `REQ-MOV-004`
- Acceptance criteria: pure M1 evidence toward `AC-WT-001`, `AC-WT-003`, `AC-WT-004`, `AC-WT-005`, and `AC-WT-006`; physical target behavior and Unity integration remain deferred

## Integrated unit

- Engine-free immutable aim, camera, transfer descriptor, observation, result, event, revision, and modifier-sink payloads.
- Checked Q1000 distance-key arithmetic and deterministic mouse/gamepad selection with ordinal target IDs.
- Single-target transfer session with atomic sink preflight/apply, exact modifier IDs, 21-tick transition lock, manual recall, lifecycle reset, removal ordering, and separate same-tick removal/press outcomes.
- Exact normalized-grid, camera-pose tick, source-specific selection-key, state/event, and modifier-kind constructor invariants.
- No Unity object, render frame, wall clock, float, or double controls pure M1 selection or state transitions.

## Evidence

### EV-2026-08-26-AC-WT-M1-EDITMODE

- Unity `6000.3.21f1` batch EditMode transfer suite.
- Result: PASS, 17/17, failed 0.
- Result XML SHA-256: `DCA20EF81FB5710621574B49E96CD505E40D25EE60C9019D1E15E2FF3A4F5672`.
- Coverage: Q1000 5.999/6.000/6.001u keys; mouse 23/24/25px; gamepad 17.9/18.0/18.1°, 25.9/26.0/26.1°, and 3.9/4.0/4.1° hysteresis; failure precedence; stale and rejected atomic failures; age 1/20/21 locks; exact IDs; payload invariants; overflow-before-sink behavior; default observation rejection; lifecycle idempotence; multi-removal order permutations; removed-observation filtering; same-tick removal plus press; and three full snapshot/event replays.

### EV-2026-08-26-AC-WT-M1-MOVEMENT-REGRESSION

- Existing VD-01 EditMode suite after shared Core aim payload integration.
- Result: PASS, 22/22, failed 0.
- Result XML SHA-256: `55582AB25461680CF7C86214E01FE8394ADBD1BF4CD21A83A4F1A444FE1C1593`.

### EV-2026-08-26-AC-WT-M1-STATIC

- `git diff --check`: PASS.
- Core/Transfer runtime search for Unity engine/editor/physics, wall-clock, float, and double dependencies: no matches.
- `AcadeGameMaker.Transfer.asmdef` uses `noEngineReferences: true`.
- Source SHA-256:
  - `AimContracts.cs`: `073EA586D10DD8EB7713966034BB42B39503B39210ABF15FFDB9EB73BE5B1353`
  - `TransferContracts.cs`: `21CEF5FB70B164009C483F5D34B1A6B4D4D9AED942615343C9A149892232FFC9`
  - `TransferSelector.cs`: `89AE360A2DD25DAEF8BC0162BB724E216E256DC7C6A38E601AB8CEABB4619DBF`
  - `TransferSession.cs`: `3B04C20F34B62D8D3FB51B79940AA6FEA7CA3AF2E5451B4D0A95E481CAE72F5B`
  - `TransferM1Tests.cs`: `8B873A84A93AC325E19006716BA4C91B4A1D1CF01CCFFC26A4F1691D093D7F42`

## Corrected issues and draft screening

1. Sol rejected Kimi's float/timestamp/object-dictionary implementation and retained only its file and test decomposition. No Kimi code was adopted.
2. Sol replaced the contradictory Q100 target boundary with Q1000 target geometry and froze the exact scalar ABI.
3. Luna's first review found incomplete input/key/modifier validation, unverifiable derived-key wording, and early return during multiple removals. Sol clarified the M1/M2 responsibility boundary and Terra corrected the implementation.
4. Sol found post-sink overflow risk, lifecycle snapshot/registry leakage, fabricated no-op failure results, removed-target registration residue, and same-tick removed observations re-entering highlight. Terra corrected each path and added regressions.
5. Intermediate Unity runs exposed two invalid hysteresis fixtures, one missing method argument, and one overly exact exception assertion. Those runs were rejected as evidence; only the final fresh 17/17 run is cited.

## Deferred criterion portions

- `AC-WT-001`: box physics and actual movement modifier bridge remain M2.
- `AC-WT-002`: enemy behavior remains VD-03.
- `AC-WT-003`: actual Unity LOS/removal adapters remain M2.
- `AC-WT-004`: scene transition and presentation cleanup remain M2/UI.
- `AC-WT-005`: physical box/enemy/boss modifier consumers and impact-damage use remain M2/VD-03.
- `AC-WT-006`: deterministic camera projection, LOS endpoints, and 30/60/144 integrated replay remain M2.

## Sol integration decision

Sol accepts the VD-02 pure M1 milestone. Luna independently reports PASS with no remaining M1 P0/P1 blocker. VD-02 remains `Approved`, not fully `Verified`; M2 stays blocked until Sol freezes its projection, LOS, world-observation, and movement-bridge addendum and Luna passes that pre-gate.
