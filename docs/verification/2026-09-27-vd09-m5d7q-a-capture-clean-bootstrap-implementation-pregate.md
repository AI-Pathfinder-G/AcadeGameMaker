# M5D7Q-A clean-bootstrap implementation static gate

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: exact source review only; no Unity launch, capture execution, or implementation edit
- Generator: `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationCaptureGenerator.cs`
- Generator SHA-256: `03D2C94B162EBB567EBF4AA9B94F5DEB0A8E89D72F45FFD9351A62D732058AEE`
- Approved-contract SHA-256: `BE5EA92B56A85A0B217CBCEAF5FF37A0DAA861AB2E33F8C080D87A0AB5CE07BC`
- Proposal SHA-256: `26BD7211C3C68AB68ADEF0941DFD712A8A5BD2462ED89CB80AF1F1D3B4A6D22F`

## Verdict

**BLOCKED — P0=0, P1=1, P2=0.** The exact generator hash is identified, but
the implementation cannot pass the clean-bootstrap gate yet.

## P1 finding

**CB-IMPL-P1-001 — pre-predicate snapshot is not first terminal preflight.**
`Capture()` calls `RequireCleanScenes()`, stale `.tmp/.prev` checks, and
`VerifyGuid(project)` before `State.Take()` (current source lines 45–46).
`State.Take()` is where `GetSceneManagerSetup()` reaches
`TerminalPreflight()`, which emits the required snapshot and exact
`BOOTSTRAP_DIRTY`/other scene-shape codes. Therefore a dirty bootstrap (or any
earlier failing precondition) throws before the required snapshot and before
the exact sorted terminal classification. This violates the proposal's
pre-predicate requirement and makes the mandated failure evidence incomplete.

Required narrow correction: enter the terminal snapshot/classification before
`RequireCleanScenes()` and other throwing preflight checks (or otherwise make
the snapshot unconditional immediately after observing `setupCount == 0`),
while retaining non-terminal dirty-scene rejection. Recompute the generator
SHA and repeat this static gate.

## Closed checks

- When reached, `InitialTerminalSceneSnapshot` has the required fixed fields,
  deterministic invalid-scene defaults, invariant numeric formatting,
  lowercase booleans, and complete JSON string escaping for path/name.
- `Classify()` implements exact TrueZero and one clean unsaved bootstrap rules,
  including loaded/active identity, empty path, clean state, build index `-1`,
  and zero roots; failure codes are deduplicated and ordinal-sorted.
- Accepted/complete terminal markers carry `initialSceneKind`; completion
  explicitly records `initialSceneRestored=false` and `zeroSceneRestored=false`.
  The command boundary rejects non-batchmode, token multiplicity, `-runTests`,
  and execute-method mismatch.
- No `SaveScene`, `SaveOpenScenes`, `AssetDatabase.SaveAssets`,
  `EditorUtility.SetDirty`, or `CloseScene` call is present. Terminal cleanup
  keeps the process-boundary/no-save claim and non-terminal setup restoration.
- Existing TMP observation, authoritative local-mesh predicate, independent
  pass A/B byte comparison, canonical manifest, GUID/hash checks, and staged
  publish/rollback logic remain present and structurally unchanged.
- No obvious C#9/Unity 6000.6 API incompatibility was found by static review;
  Unity compilation and execution are not claimed here.

No user product decision is implicated. After the narrow ordering correction,
the source must receive a fresh SHA and Luna re-gate before runtime execution.
