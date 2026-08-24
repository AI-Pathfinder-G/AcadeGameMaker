# OD-ART-001 Reticle Contract — Luna Independent Review

- Date: 2026-08-24
- Verifier: GPT-5.6 Luna
- Integration owner: Sol
- Scope: `REQ-UX-010`, `REQ-UX-013`, `REQ-ART-014~015`; `AC-UX-009`, `AC-UX-012`, `AC-ART-011~012`
- Result: PASS

## Verified contract

- 640×360 logical pixel basis
- non-acquired W11 `#CAC9B8` 9×9px reticle
- acquired S02 `#F7FFFC` 13×13px reticle core and 1px target outline
- exact acquisition/release completion at 3/6 SimulationTick
- acquired size/color retained across target switch
- 1px S01/S03/S04 continuous/continuous/broken state ring
- same-hue outer 1px halo with emission ≤1.25 and exact core HEX preservation
- no blink; same-tick hide in UIOnly, Cutscene, Transition, Ended and letterbox/pillarbox pointer state

## Regression evidence

- validator: `PASS scenarios=13 coveredAC=68`, exit 0
- self-test: `PASS selfTest=7 scenarios=13 coveredAC=68`, exit 0
- `OD-ART-001` moved to Resolved
- aim/render catalog scenarios changed from `blocked` to `draft`
- only `OD-SCENE-001` remains an allowed QA blocker
- Unity/runtime files were not created

## Judgment

The approval record, consuming contracts, requirements, acceptance criteria, system contract, traceability and wiki are consistent. VD-07·VD-08·VD-09 remain Review pending their full-spec Luna review; this PASS resolves the OD-ART-001 decision only and is not implementation evidence.
