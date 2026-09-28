# M5D7Q-A capture-runtime implementation static gate

Date: 2026-09-27  
Reviewer: Luna (independent static implementation gate)  
Scope: exact source review only; no Unity launch, capture execution, or implementation edit.

## Identity

- Generator: `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationCaptureGenerator.cs`
- Generator SHA-256: `B499A12F1C8AA2E04CF330D6B3DBB557257ABA706E6DC8133A148ABF6AD96214`
- Approved contract SHA-256: `6ED5D7AE6D509433D6B008347417D4DBBF916069538360E40EB76CF36DD7801E`
- Runtime-blocker proposal SHA-256: `2E2ADA39EAC1283ABD53A2C6F86C3BEB9EE05EAB9CFC7EBD5253A5C56F5F4F70`

## Static checks

- TMP observation runs after the required two Canvas/TMP update passes. It emits
  the fixed invariant diagnostic before assertion, uses the component-local
  RectTransform as the only containment space, and checks finite rect/margin,
  overflow flags, exact source characters, canonical font/material ownership,
  actual mesh vertex equality, and no-epsilon vertex containment.
- Terminal preflight runs in `State.Take` before output directory creation or
  Hub opening. It requires batchmode, exactly one `-batchmode`, `-quit`, and
  `-executeMethod` with the exact method token, rejects `-runTests`, and requires
  zero initial scenes/invalid active scene. Terminal restore never calls
  `CloseScene`; it proves one clean active Hub and records the accepted/complete
  markers with `zeroSceneRestored=false`.
- Non-empty sessions retain exact scene-setup restoration; Canvas/SafeFrame,
  backdrop, selection, TMP/UI state, globals and authored hashes are restored
  and asserted.
- Pass A writes the ten PNG bytes; independent Pass B re-applies the authoring
  view/oracle and renders fresh targets/readbacks. PNG bytes and regenerated
  canonical manifest bytes are compared before publication, with the exact
  same-process marker. Fresh-process byte equality remains external evidence.
- `StagePublish`, `RollbackPublish`, and deferred `CommitPublish` preserve or
  restore the prior final set on all later failures. Manifest and result-set
  allowlists remain exact. No save/SetDirty/CloseScene call is present.
- Static API inspection against the local Unity 6000.6/uGUI 2.6.0 sources
  confirms the referenced TMP fields/enums and Scene API are available; no
  obvious C#9 compile blocker was found. Unity compilation/execution is not
  claimed by this gate.

## Verdict

**PASS — P0=0, P1=0, P2=0 (static implementation gate).**

This PASS is limited to the exact generator SHA. It does not accept retry-f or
fresh-process output; those runs, Luna post-review and Astra final integration
remain required.

