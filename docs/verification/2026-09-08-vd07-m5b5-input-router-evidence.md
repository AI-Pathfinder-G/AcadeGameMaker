# M5B5 InputRouter implementation and independent verification

- Status: Verified — Astra acceptance after Luna independent final confirmation, 2026-09-08
- Contract: [Approved M5B5](../specs/work-contracts/2026-09-08-vd07-m5b5-device-input-router.md)
- Authority/integration: Astra; implementation: Terra (consumer/terminal lane and device-router lane); independent tests/review: Luna
- Last Verified baseline: M5B4 full EditMode 440/440 and PlayMode 474/474 (914 total). These are not M5B5 results.

## Static review before first Unity run

Astra rejected the first runtime checkpoint pending these contract corrections. This is an issue record, not a claim that fixes or acceptance tests have passed.

- AC-M5B5-002/007/008: checked frame/sample/attack ID successors were calculated after local commits, and CurrentReceipt was assigned before later fallible operations. All reservations must be computed first and global receipt assigned last.
- AC-M5B5-002/004: a mode-change frame was built from the old epoch's pending aim/edges before clearing them. New-epoch staging must be isolated without clearing the original batch on a failed frame.
- AC-M5B5-003: missing-camera mouse Transfer could be emitted as nullable aim; device provenance, completed Movement pose, viewport validity, signed actual coordinate conversion and minimum mouse-direction validation were incomplete.
- AC-M5B5-005/008: router terminal-transition rejection and receipt-proven Combat retirement were absent. A disabled component alone is not retirement authority.
- AC-M5B5-007: Combat set its attempt latch before checking the coordinated receipt. Rejected proof must leave that latch untouched.
- AC-M5B5-005/007/008: terminal proof closure occurred before the existing discard commit's remaining validations. Proof close and discard must share the prevalidated mutation boundary, including generation-overflow rejection.
- AC-M5B5-004/007/008: exact callback source checks, disable/unsubscribe behavior, fixed fault injection seams and bounded diagnostic details require completion.
- AC-M5B5-006: initial assembly/test checkpoint required a direct Input System assembly reference, test friend access and compile-level fixture corrections. No Unity success is claimed here.

## Final test runs

| Run | Passed/total | SHA-256 |
|---|---:|---|
| TestResults-Unity-PlayMode-20260908-192050.xml (focused InputUnity) | 65/65 | 85397033D68A11B18B08F34295D1C9685F6FDCA662A3CC143506E382561FE774 |
| TestResults-Unity-EditMode-20260908-192133.xml (full) | 440/440 | EAD67C3CAA147A0E5CCF6F6AAFCD66135547200352E536FDD6E7DDC015459799 |
| TestResults-Unity-PlayMode-20260908-192211.xml (full) | 539/539 | FD09C95F30589F57BC43C941BCE3358959EAC139F3B1D592DBDB41873B5FC0E7 |

All three runs have zero failed/skipped tests and Unity exit code 0. Final full-suite total is **979**, an increase of 65 over the Verified M5B4 baseline. Focused results are a subset, not additional tests. The root executed Unity sequentially with all Assets writers frozen. Licensing preflight passed; no installation or external model call was used.

## AC mapping and independent ownership

- AC-M5B5-001: actual virtual Keyboard/Mouse/Gamepad callbacks, no callback-time movement, next-tick consumer input and identical held-input replay under scripted 30/60/144 render grouping. Grouping tests do not claim physical frame-rate or device certification.
- AC-M5B5-002: exact five-mode maps/gates, global receipt publication, mode epochs, unchanged precommit rejection and same-tick corrected retry; bootstrap clarification independently reviewed by Luna and implemented/tested.
- AC-M5B5-003: prior completed pose, normalized endpoint grid, signed actual pointer outside flags, letterbox boundaries, raw-stick threshold/retention, fresh per-tick shared samples, absent/stale camera suppression, nullable gamepad Transfer and too-short mouse direction without an orphan camera payload.
- AC-M5B5-004: held-through-lock Jump does not replay; release-old then fresh press/release retains both new edges. Real queued KeyboardState events are ignored after device disable/removal before input processing. Exact foreign action/map callback rejection is tested.
- AC-M5B5-005: full existing system-input/terminal regressions, diagnostic-only unbound actions, completed boss death transition requirement, locked cleanup and exact receipt-proven Combat retirement. Neutral Movement frames remain normal gameplay behavior, not an unbound action effect.
- AC-M5B5-006: full 979-test regression and existing official-wrapper regeneration/asset-shape checks pass. No action asset/wrapper, package, settings, scene/prefab or media was edited by M5B5. Final input asset SHA-256: 8E23A5A3AE1D514596A1FC8C61017F2740664904741D5A95383074A06E840E76; wrapper: 730BE8CAD941CD4567BBEA9B844FE33673FEA49842E28CB53C2F088D3F528C89.
- AC-M5B5-007: actual router-source stale/future/ordinal/epoch/mode mismatch, duplicate consumption, foreign source identity, absent/faulted proof and every prepare/validate/post-local-commit/map boundary. Failed proof leaves phase state and pending proof intact; partial failures latch Faulted with old global receipt and no false rollback.
- AC-M5B5-008: mixed-owner preflight across all three sinks, frame/epoch/sample/attack counter overflow, disable/re-enable rejection, closed Combat proof, exact retirement matching and terminal-discard overflow before proof/queue mutation.

Terra implemented both runtime lanes and the initial authored terminal test. Luna independently authored the base/adversarial fixtures, reviewed contracts/runtime and identified additional acceptance gaps. Astra added integration/aim/failure-boundary tests, repaired test setup and narrow compile/integration defects, and ran Unity. Luna independently confirmed the final corrected 65-case focused result and both full-suite XML hashes/counts with no remaining AC001..008 blocker. Astra accepts this bounded unit as Verified; deferred product integrations below remain unfinished.

## Corrections retained in the audit

The initial runtime issues above were corrected rather than relaxed. Compiler failures exposed missing using directives, backward-compatible candidate constructor defaults, and the inaccessible internal Transfer outcome returned to the new test assembly. A narrow void phase-test seam was added instead of granting broad Transfer-core friendship.

Test failures were also kept distinct from runtime failures: the first keyboard fixture placed an obstacle touching the player; mouse (0,0) required a real changed event; a held mouse button had to be released before a fresh same-action gamepad press; a neutral Movement frame was initially misclassified as an unbound action effect. The terminal helper originally returned after consuming cleanup Transfer and later had its handoff ordered before Movement; both were corrected without phase rewinding or weaker runtime checks. Exception wrapping was normalized through the base exception type. A retained native InputAction.CallbackContext was invalid after callback/device removal; the test now queues a real event and retires the device before processing. The held-through-lock release is untracked by design, so the JumpReleased oracle now uses the fresh press's own release.

The whole dirty worktree was preserved. A pre-existing trailing-space finding in ProjectSettings/ProjectSettings.asset was not cleaned up; the scoped M5B5 runtime/test diff check passes.

This unit does not implement Pause/menu/clock effects, a real camera driver, Run/reward/Choice binding, playable-scene authoring, hardware certification or final reticle art. No Ollama, automation, package installation, commit or push is part of this work.
